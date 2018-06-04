using System;

namespace NX68Sound
{
    public class sound_iocs
    {
        private X68Sound x68Sound = null;

        public sound_iocs(X68Sound x68Sound)
        {
            this.x68Sound = x68Sound;
        }


        // 16bit値のバイトの並びを逆にして返す
        public static UInt16 bswapw(UInt16 data)
        {
            return (UInt16)((data >> 8) + ((data & 0xff) << 8));
        }

        // 32bit値のバイトの並びを逆にして返す
        public static UInt32 bswapl(UInt32 adrs)
        {
            return (UInt32)((adrs >> 24) + ((adrs & 0xff0000) >> 8) + ((adrs & 0xff00) << 8) + ((adrs & 0xff) << 24));
        }

        private byte AdpcmStat = 0; // $02:adpcmout $12:adpcmaot $22:adpcmlot $32:adpcmcot
        private byte OpmReg1B = 0;  // OPM レジスタ $1B の内容
        private byte DmaErrCode = 0;

        private UInt32 Adpcmcot_adrs;
        private UInt16 Adpcmcot_len;

        // OPMのBUSY待ち
        private void OpmWait()
        {
            while ((x68Sound.X68Sound_OpmPeek() & 0x80) != 0) ;
        }

        // IOCS _OPMSET ($68) の処理
        // [引数]
        //   int addr : OPMレジスタナンバー(0～255)
        //   int data : データ(0～255)
        public void _iocs_opmset(Int32 addr, Int32 data)
        {
            if (addr == 0x1B)
            {
                OpmReg1B = (byte)((OpmReg1B & 0xC0) | (data & 0x3F));
                data = OpmReg1B;
            }

            OpmWait();
            x68Sound.X68Sound_OpmReg((byte)addr);
            OpmWait();
            x68Sound.X68Sound_OpmPoke((byte)data);

        }

        // IOCS _OPMSNS ($69) の処理
        // [戻り値]
        //   bit 0 : タイマーAオーバーフローのとき1になる
        //   bit 1 : タイマーBオーバーフローのとき1になる
        //   bit 7 : 0ならばデータ書き込み可能
        public Int32 _iocs_opmsns()
        {
            return x68Sound.X68Sound_OpmPeek();
        }

        public Action OpmIntProc = null;		// OPMのタイマー割り込み処理アドレス

        // IOCS _OPMINTST ($6A) の処理
        // [引数]
        //   void *addr : 割り込み処理アドレス
        //                0のときは割り込み禁止
        // [戻り値]
        //   割り込みが設定された場合は 0
        //   既に割り込みが設定されている場合はその割り込み処理アドレスを返す
        public Action _iocs_opmintst(Action addr)
        {
            if (addr == null)
            {               // 引数が0の時は割り込みを禁止する
                OpmIntProc = null;
                x68Sound.X68Sound_OpmInt(OpmIntProc);
                return null;
            }
            if (OpmIntProc != null)
            {       // 既に設定されている場合は、その処理アドレスを返す
                return OpmIntProc;
            }
            OpmIntProc = addr;
            x68Sound.X68Sound_OpmInt(OpmIntProc);    // OPMの割り込み処理アドレスを設定
            return null;
        }

        // DMA転送終了割り込み処理ルーチン
        private void DmaIntProc()
        {
            if (AdpcmStat == 0x32 && (x68Sound.X68Sound_DmaPeek(0x00) & 0x40) != 0)
            {   // コンティニューモード時の処理
                x68Sound.X68Sound_DmaPoke(0x00, 0x40);   // BTCビットをクリア
                if (Adpcmcot_len > 0)
                {
                    UInt16 dmalen;
                    dmalen = Adpcmcot_len;
                    if (dmalen > 0xFF00)
                    {   // 1度に転送できるバイト数は0xFF00
                        dmalen = 0xFF00;
                    }
                    x68Sound.X68Sound_DmaPokeL(0x1C, Adpcmcot_adrs); // BARに次のDMA転送アドレスをセット
                    x68Sound.X68Sound_DmaPokeW(0x1A, dmalen);    // BTCに次のDMA転送バイト数をセット
                    Adpcmcot_adrs += dmalen;
                    Adpcmcot_len -= dmalen;

                    x68Sound.X68Sound_DmaPoke(0x07, 0x48);   // コンティニューオペレーション設定
                }
                return;
            }
            if ((AdpcmStat & 0x80) == 0)
            {
                x68Sound.X68Sound_PpiCtrl(0x01); // ADPCM右出力OFF
                x68Sound.X68Sound_PpiCtrl(0x03); // ADPCM左出力OFF
                x68Sound.X68Sound_AdpcmPoke(0x01);   // ADPCM再生動作停止
            }
            AdpcmStat = 0;
            x68Sound.X68Sound_DmaPoke(0x00, 0xFF);   // DMA CSR の全ビットをクリア
        }

        // DMAエラー割り込み処理ルーチン
        private void DmaErrIntProc()
        {
            DmaErrCode = x68Sound.X68Sound_DmaPeek(0x01);    // エラーコードを DmaErrCode に保存

            x68Sound.X68Sound_PpiCtrl(0x01); // ADPCM右出力OFF
            x68Sound.X68Sound_PpiCtrl(0x03); // ADPCM左出力OFF
            x68Sound.X68Sound_AdpcmPoke(0x01);   // ADPCM再生動作停止

            AdpcmStat = 0;
            x68Sound.X68Sound_DmaPoke(0x00, 0xFF);   // DMA CSR の全ビットをクリア
        }

        private byte[] PANTBL = new byte[4] { 3, 1, 2, 0 };

        // サンプリング周波数とPANを設定してDMA転送を開始するルーチン
        // [引数]
        //   unsigned short mode : サンプリング周波数*256+PAN
        //   unsigned char ccr : DMA CCR に書き込むデータ
        private void SetAdpcmMode(UInt16 mode, byte ccr)
        {
            if (mode >= 0x0200)
            {
                mode -= 0x0200;
                OpmReg1B &= 0x7F;   // ADPCMのクロックは8MHz
            }
            else
            {
                OpmReg1B |= 0x80;   // ADPCMのクロックは4MHz
            }
            OpmWait();
            x68Sound.X68Sound_OpmReg(0x1B);
            OpmWait();
            x68Sound.X68Sound_OpmPoke(OpmReg1B); // ADPCMのクロック設定(8or4MHz)
            byte ppireg;
            ppireg = (byte)(((mode >> 6) & 0x0C) | PANTBL[mode & 3]);
            ppireg |= (byte)(x68Sound.X68Sound_PpiPeek() & 0xF0);
            x68Sound.X68Sound_DmaPoke(0x07, ccr);    // DMA転送開始
            x68Sound.X68Sound_PpiPoke(ppireg);   // サンプリングレート＆PANをPPIに設定
        }

        // _iocs_adpcmoutのメインルーチン
        // [引数]
        //   unsigned char stat : ADPCMを停止させずに続けてDMA転送を行う場合は$80
        //                        DMA転送終了後ADPCMを停止させる場合は$00
        //   unsigned short len : DMA転送バイト数
        //   unsigned char *adrs : DMA転送アドレス
        private void AdpcmoutMain(byte stat, UInt16 mode, UInt16 len, UInt32 adrs)
        {
            while (AdpcmStat != 0) ; // DMA転送終了待ち
            AdpcmStat = (byte)(stat + 2);
            x68Sound.X68Sound_DmaPoke(0x05, 0x32);   // DMA OCR をチェイン動作なしに設定

            x68Sound.X68Sound_DmaPoke(0x00, 0xFF);   // DMA CSR の全ビットをクリア
            x68Sound.X68Sound_DmaPokeL(0x0C, adrs);  // DMA MAR にDMA転送アドレスをセット
            x68Sound.X68Sound_DmaPokeW(0x0A, len);   // DMA MTC にDMA転送バイト数をセット
            SetAdpcmMode(mode, 0x88);   // サンプリング周波数とPANを設定してDMA転送開始

            x68Sound.X68Sound_AdpcmPoke(0x02);   // ADPCM再生開始
        }

        // IOCS _ADPCMOUT ($60) の処理
        // [引数]
        //   void *addr : ADPCMデータアドレス
        //   int mode : サンプリング周波数(0～4)*256+PAN(0～3)
        //   int len : ADPCMデータのバイト数
        public void _iocs_adpcmout(UInt32 addr, Int32 mode, Int32 len)
        {
            UInt32 dmalen;
            UInt32 dmaadrsPtr = addr;
            while (AdpcmStat != 0) ; // DMA転送終了待ち
            while (len > 0x0000FF00)
            {   // ADPCMデータが0xFF00バイト以上の場合は
                dmalen = 0x0000FF00;    // 0xFF00バイトずつ複数回に分けてDMA転送を行う
                AdpcmoutMain(0x80, (UInt16)mode, (UInt16)dmalen, dmaadrsPtr);
                dmaadrsPtr += dmalen;
                len -= (Int32)dmalen;
            }
            AdpcmoutMain(0x00, (UInt16)mode, (UInt16)len, dmaadrsPtr);
        }

        // IOCS _ADPCMAOT ($62) の処理
        // [引数]
        //   struct _chain *tbl : アレイチェインテーブルのアドレス
        //   int mode : サンプリング周波数(0～4)*256+PAN(0～3)
        //   int cnt : アレイチェインテーブルのブロック数
        public void _iocs_adpcmaot(UInt32 tblPtr, int mode, int cnt)
        {
            while (AdpcmStat != 0) ;    // DMA転送終了待ち

            AdpcmStat = 0x12;
            x68Sound.X68Sound_DmaPoke(0x05, 0x3A);   // DMA OCR をアレイチェイン動作に設定

            x68Sound.X68Sound_DmaPoke(0x00, 0xFF);   // DMA CSR の全ビットをクリア
            x68Sound.X68Sound_DmaPokeL(0x1C, tblPtr);   // DMA BAR にアレイチェインテーブルアドレスをセット
            x68Sound.X68Sound_DmaPokeW(0x1A, (ushort)cnt);   // DMA BTC にアレイチェインテーブルの個数をセット
            SetAdpcmMode((ushort)mode, 0x88);   // サンプリング周波数とPANを設定してDMA転送開始

            x68Sound.X68Sound_AdpcmPoke(0x02);   // ADPCM再生開始
        }

        // IOCS _ADPCMAOT ($64) の処理
        // [引数]
        //   struct _chain2 *tbl : リンクアレイチェインテーブルのアドレス
        //   int mode : サンプリング周波数(0～4)*256+PAN(0～3)
        public void _iocs_adpcmlot(UInt32 tblPtr, int mode)
        {
            while (AdpcmStat != 0) ;    // DMA転送終了待ち

            AdpcmStat = 0x22;
            x68Sound.X68Sound_DmaPoke(0x05, 0x3E);   // DMA OCR をリンクアレイチェイン動作に設定

            x68Sound.X68Sound_DmaPoke(0x00, 0xFF);   // DMA CSR の全ビットをクリア
            x68Sound.X68Sound_DmaPokeL(0x1C, tblPtr);   // DMA BAR にリンクアレイチェインテーブルアドレスをセット
            SetAdpcmMode((ushort)mode, 0x88);   // サンプリング周波数とPANを設定してDMA転送開始

            x68Sound.X68Sound_AdpcmPoke(0x02);   // ADPCM再生開始
        }


        // コンティニューモードを利用してADPCM出力を行うサンプル
        // IOCS _ADPCMOUT と同じ処理を行うが、データバイト数が0xFF00バイト以上でも
        // すぐにリターンする。
        // [引数]
        //   void *addr : ADPCMデータアドレス
        //   int mode : サンプリング周波数(0～4)*256+PAN(0～3)
        //   int len : ADPCMデータのバイト数
        public void _iocs_adpcmcot(uint addr, int mode, int len)
        {
            int dmalen;
            Adpcmcot_adrs = (uint)addr;
            Adpcmcot_len = (ushort)len;
            while (AdpcmStat != 0) ; // DMA転送終了待ち
            AdpcmStat = 0x32;

            x68Sound.X68Sound_DmaPoke(0x05, 0x32);   // DMA OCR をチェイン動作なしに設定

            dmalen = Adpcmcot_len;
            if (dmalen > 0xFF00)
            {   // ADPCMデータが0xFF00バイト以上の場合は
                dmalen = 0xFF00;    // 0xFF00バイトずつ複数回に分けてDMA転送を行う
            }

            x68Sound.X68Sound_DmaPoke(0x00, 0xFF);   // DMA CSR の全ビットをクリア
            x68Sound.X68Sound_DmaPokeL(0x0C, Adpcmcot_adrs); // DMA MAR にDMA転送アドレスをセット
            x68Sound.X68Sound_DmaPokeW(0x0A, (ushort)dmalen);    // DMA MTC にDMA転送バイト数をセット
            Adpcmcot_adrs += (uint)dmalen;
            Adpcmcot_len -= (ushort)dmalen;
            if (Adpcmcot_len <= 0)
            {
                SetAdpcmMode((ushort)mode, 0x88);   // データバイト数が0xFF00以下の場合は通常転送
            }
            else
            {
                dmalen = Adpcmcot_len;
                if (dmalen > 0xFF00)
                {
                    dmalen = 0xFF00;
                }
                x68Sound.X68Sound_DmaPokeL(0x1C, Adpcmcot_adrs); // BARに次のDMA転送アドレスをセット
                x68Sound.X68Sound_DmaPokeW(0x1A, (ushort)dmalen);    // BTCに次のDMA転送バイト数をセット
                Adpcmcot_adrs += (uint)dmalen;
                Adpcmcot_len -= (ushort)dmalen;
                SetAdpcmMode((ushort)mode, 0xC8);   // DMA CNTビットを1にしてDMA転送開始
            }

            x68Sound.X68Sound_AdpcmPoke(0x02);	// ADPCM再生開始
        }

        // IOCS _ADPCMSNS ($66) の処理
        // [戻り値]
        //   0 : 何もしていない
        //   $02 : _iocs_adpcmout で出力中
        //   $12 : _iocs_adpcmaot で出力中
        //   $22 : _iocs_adpcmlot で出力中
        //   $32 : _iocs_adpcmcot で出力中
        public int _iocs_adpcmsns()
        {
            return (AdpcmStat & 0x7F);
        }

        // IOCS _ADPCMMOD ($67) の処理
        // [引数]
        //   0 : ADPCM再生 終了
        //   1 : ADPCM再生 一時停止
        //   2 : ADPCM再生 再開
        public void _iocs_adpcmmod(Int32 mode)
        {
            switch (mode)
            {
                case 0:
                    AdpcmStat = 0;
                    x68Sound.X68Sound_PpiCtrl(0x01); // ADPCM右出力OFF
                    x68Sound.X68Sound_PpiCtrl(0x03); // ADPCM左出力OFF
                    x68Sound.X68Sound_AdpcmPoke(0x01);   // ADPCM再生動作停止
                    x68Sound.X68Sound_DmaPoke(0x07, 0x10);   // DMA SAB=1 (ソフトウェアアボート)
                    break;
                case 1:
                    x68Sound.X68Sound_DmaPoke(0x07, 0x20);   // DMA HLT=1 (ホルトオペレーション)
                    break;
                case 2:
                    x68Sound.X68Sound_DmaPoke(0x07, 0x08);   // DMA HLT=0 (ホルトオペレーション解除)
                    break;
            }
        }


        // IOCSコールの初期化
        // DMAの割り込みを設定する
        public void init()
        {
            x68Sound.X68Sound_DmaInt(DmaIntProc);
            x68Sound.X68Sound_DmaErrInt(DmaErrIntProc);
        }

    }
}
