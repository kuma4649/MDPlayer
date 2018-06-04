using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NX68Sound
{
    public class Pcm8
    {
        private global global = null;

        private int Scale;      // 
        private int Pcm;        // 16bit PCM Data
        private int Pcm16Prev;  // 16bit,8bitPCMの1つ前のデータ
        private int InpPcm, InpPcm_prev, OutPcm;        // HPF用 16bit PCM Data
        private int OutInpPcm, OutInpPcm_prev;      // HPF用
        private int AdpcmRate;  // 187500(15625*12), 125000(10416.66*12), 93750(7812.5*12), 62500(5208.33*12), 46875(3906.25*12), ...
        private int RateCounter;
        private int N1Data; // ADPCM 1サンプルのデータの保存
        private int N1DataFlag; // 0 or 1

        private int Mode;
        private int Volume;    // x/16
        private int PcmKind;   // 0～4:ADPCM  5:16bitPCM  6:8bitPCM  7:謎

        //inline void adpcm2pcm(unsigned char adpcm);
        //inline void pcm16_2pcm(int pcm16);

        //inline int DmaGetByte();
        public byte DmaLastValue;
        public byte AdpcmReg;

        public byte[] DmaMarBuf;
        public uint DmaMarPtr;

        public uint DmaMtc;
        public byte[] DmaBarBuf;
        public uint DmaBarPtr;
        public uint DmaBtc;
        public int DmaOcr;                // 0:チェイン動作なし 0x08:アレイチェイン 0x0C:リンクアレイチェイン


        //inline int DmaArrayChainSetNextMtcMar();
        //inline int DmaLinkArrayChainSetNextMtcMar();

        //Pcm8(void);
        //~Pcm8() { };
        //inline void Init();
        //inline void InitSamprate();
        //inline void Reset();
        //inline int GetPcm();
        //inline int GetPcm62();

        //inline int Out(void* adrs, int mode, int len);
        //inline int Aot(void* tbl, int mode, int cnt);
        //inline int Lot(void* tbl, int mode);
        //inline int SetMode(int mode);
        //inline int GetRest();
        //inline int GetMode();


        public Pcm8(global global)
        {
            this.global = global;
            Mode = 0x00080403;
            SetMode(Mode);
        }



        public void Init()
        {
            AdpcmReg = 0xC7;    // ADPCM動作停止

            Scale = 0;
            Pcm = 0;
            Pcm16Prev = 0;
            InpPcm = InpPcm_prev = OutPcm = 0;
            OutInpPcm = OutInpPcm_prev = 0;
            AdpcmRate = 15625 * 12;
            RateCounter = 0;
            N1Data = 0;
            N1DataFlag = 0;
            DmaLastValue = 0;

            DmaMarBuf = null;
            DmaMarPtr = 0;
            DmaMtc = 0;
            DmaBarBuf = null;
            DmaBarPtr = 0;
            DmaBtc = 0;
            DmaOcr = 0;
        }

        private void InitSamprate()
        {
            RateCounter = 0;
        }

        public void Reset()
        {       // ADPCM キーオン時の処理
            Scale = 0;
            Pcm = 0;
            Pcm16Prev = 0;
            InpPcm = InpPcm_prev = OutPcm = 0;
            OutInpPcm = OutInpPcm_prev = 0;

            N1Data = 0;
            N1DataFlag = 0;

        }

        public int DmaArrayChainSetNextMtcMar()
        {
            if (DmaBtc == 0)
            {
                return 1;
            }
            --DmaBtc;

            int mem0, mem1, mem2, mem3, mem4, mem5;
            mem0 = global.MemRead(DmaBarPtr++);
            mem1 = global.MemRead(DmaBarPtr++);
            mem2 = global.MemRead(DmaBarPtr++);
            mem3 = global.MemRead(DmaBarPtr++);
            mem4 = global.MemRead(DmaBarPtr++);
            mem5 = global.MemRead(DmaBarPtr++);
            if ((mem0 | mem1 | mem2 | mem3 | mem4 | mem5) == -1)
            {
                // バスエラー(ベースアドレス/ベースカウンタ)
                return 1;
            }
            //DmaMarBuf = DmaBarBuf;
            DmaMarPtr = (uint)((mem0 << 24) | (mem1 << 16) | (mem2 << 8) | (mem3)); // MAR
            DmaMtc = (uint)((mem4 << 8) | (mem5));  // MTC

            if (DmaMtc == 0)
            {   // MTC == 0 ?
                // カウントエラー(メモリアドレス/メモリカウンタ)
                return 1;
            }
            return 0;
        }

        public int DmaLinkArrayChainSetNextMtcMar()
        {
            if (DmaBarPtr == 0)
            {
                return 1;
            }

            int mem0, mem1, mem2, mem3, mem4, mem5;
            int mem6, mem7, mem8, mem9;
            mem0 = global.MemRead( DmaBarPtr++);
            mem1 = global.MemRead( DmaBarPtr++);
            mem2 = global.MemRead( DmaBarPtr++);
            mem3 = global.MemRead( DmaBarPtr++);
            mem4 = global.MemRead( DmaBarPtr++);
            mem5 = global.MemRead( DmaBarPtr++);
            mem6 = global.MemRead( DmaBarPtr++);
            mem7 = global.MemRead( DmaBarPtr++);
            mem8 = global.MemRead( DmaBarPtr++);
            mem9 = global.MemRead( DmaBarPtr++);
            if ((mem0 | mem1 | mem2 | mem3 | mem4 | mem5 | mem6 | mem7 | mem8 | mem9) == -1)
            {
                // バスエラー(ベースアドレス/ベースカウンタ)
                return 1;
            }
            //DmaMarBuf = DmaBarBuf;
            DmaMarPtr = (uint)((mem0 << 24) | (mem1 << 16) | (mem2 << 8) | (mem3)); // MAR
            DmaMtc = (uint)((mem4 << 8) | (mem5));  // MTC
            DmaBarPtr = (uint)((mem6 << 24) | (mem7 << 16) | (mem8 << 8) | (mem9)); // BAR

            if (DmaMtc == 0)
            {   // MTC == 0 ?
                // カウントエラー(メモリアドレス/メモリカウンタ)
                return 1;
            }
            return 0;
        }

        public int DmaGetByte()
        {
            if (DmaMtc == 0)
            {
                return -2147483648;// 0x80000000;
            }
            {
                int mem;
                mem = global.MemRead(DmaMarPtr);
                if (mem == -1)
                {
                    // バスエラー(メモリアドレス/メモリカウンタ)
                    return -2147483648;// 0x80000000;
                }
                DmaLastValue = (byte)mem;
                DmaMarPtr++;
            }

            --DmaMtc;

            try
            {
                if (DmaMtc == 0)
                {
                    if ((DmaOcr & 0x08) != 0)
                    {   // チェイニング動作
                        if ((DmaOcr & 0x04) == 0)
                        {   // アレイチェイン
                            if (DmaArrayChainSetNextMtcMar() != 0)
                            {
                                throw new Exception("");
                            }
                        }
                        else
                        {                       // リンクアレイチェイン
                            if (DmaLinkArrayChainSetNextMtcMar() != 0)
                            {
                                throw new Exception("");
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return DmaLastValue;
        }







        private const int MAXPCMVAL = (2047);
        private int[] HPF_shift_tbl = new int[16 + 1] { 0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, };


        // adpcmを入力して InpPcm の値を変化させる
        // -2047<<(4+4) <= InpPcm <= +2047<<(4+4)
        public void adpcm2pcm(byte adpcm)
        {


            int dltL;
            dltL = global.dltLTBL[Scale];
            dltL = (dltL & ((adpcm & 4) != 0 ? -1 : 0))
                        + ((dltL >> 1) & ((adpcm & 2) != 0 ? -1 : 0))
                        + ((dltL >> 2) & ((adpcm & 1) != 0 ? -1 : 0)) + (dltL >> 3);
            int sign = (adpcm & 8) != 0 ? -1 : 0;
            dltL = (dltL ^ sign) + (sign & 1);
            Pcm += dltL;


            if ((uint)(Pcm + MAXPCMVAL) > (uint)(MAXPCMVAL * 2))
            {
                if ((int)(Pcm + MAXPCMVAL) >= (int)(MAXPCMVAL * 2))
                {
                    Pcm = MAXPCMVAL;
                }
                else
                {
                    Pcm = -MAXPCMVAL;
                }
            }

            InpPcm = (Pcm & -4)//(int)0xFFFFFFFC) 
                << (4 + 4);

            Scale += global.DCT[adpcm];
            if ((uint)Scale > (uint)48)
            {
                if ((int)Scale >= (int)48)
                {
                    Scale = 48;
                }
                else
                {
                    Scale = 0;
                }
            }
        }

        // pcm16を入力して InpPcm の値を変化させる
        // -2047<<(4+4) <= InpPcm <= +2047<<(4+4)
        private void pcm16_2pcm(int pcm16)
        {
            Pcm += pcm16 - Pcm16Prev;
            Pcm16Prev = pcm16;


            if ((uint)(Pcm + MAXPCMVAL) > (uint)(MAXPCMVAL * 2))
            {
                if ((int)(Pcm + MAXPCMVAL) >= (int)(MAXPCMVAL * 2))
                {
                    Pcm = MAXPCMVAL;
                }
                else
                {
                    Pcm = -MAXPCMVAL;
                }
            }

            InpPcm = (Pcm & -4)//(int)0xFFFFFFFC) 
                << (4 + 4);
        }

        // -32768<<4 <= retval <= +32768<<4
        public int GetPcm()
        {
            if ((AdpcmReg & 0x80) != 0)
            {       // ADPCM 停止中
                return -2147483648;// 0x80000000;
            }
            RateCounter -= AdpcmRate;
            while (RateCounter < 0)
            {
                if (PcmKind == 5)
                {   // 16bitPCM
                    int dataH, dataL;
                    dataH = DmaGetByte();
                    if (dataH == -2147483648)//0x80000000)
                    {
                        RateCounter = 0;
                        AdpcmReg = 0xC7;    // ADPCM 停止
                        return -2147483648;// 0x80000000;
                    }
                    dataL = DmaGetByte();
                    if (dataL == -2147483648)//0x80000000)
                    {
                        RateCounter = 0;
                        AdpcmReg = 0xC7;    // ADPCM 停止
                        return -2147483648;// 0x80000000;
                    }
                    pcm16_2pcm((int)(short)((dataH << 8) | dataL)); // OutPcm に値が入る
                }
                else if (PcmKind == 6)
                {   // 8bitPCM
                    int data;
                    data = DmaGetByte();
                    if (data == -2147483648)//0x80000000)
                    {
                        RateCounter = 0;
                        AdpcmReg = 0xC7;    // ADPCM 停止
                        return -2147483648;// 0x80000000;
                    }
                    pcm16_2pcm((int)(char)data);    // InpPcm に値が入る
                }
                else
                {
                    if (N1DataFlag == 0)
                    {       // 次のADPCMデータが内部にない場合
                        int N10Data;    // (N1Data << 4) | N0Data
                        N10Data = DmaGetByte(); // DMA転送(1バイト)
                        if (N10Data == -2147483648)//0x80000000)
                        {
                            RateCounter = 0;
                            AdpcmReg = 0xC7;    // ADPCM 停止
                            return -2147483648;// 0x80000000;
                        }
                        adpcm2pcm((byte)(N10Data & 0x0F));  // InpPcm に値が入る
                        N1Data = (N10Data >> 4) & 0x0F;
                        N1DataFlag = 1;
                    }
                    else
                    {
                        adpcm2pcm((byte)N1Data);          // InpPcm に値が入る
                        N1DataFlag = 0;
                    }
                }
                RateCounter += 15625 * 12;
            }
            OutPcm = ((InpPcm << 9) - (InpPcm_prev << 9) + 459 * OutPcm) >> 9;
            InpPcm_prev = InpPcm;

            return (((OutPcm * Volume) >> 4) * global.TotalVolume) >> 8;
        }

        // -32768<<4 <= retval <= +32768<<4
        public int GetPcm62()
        {
            if ((AdpcmReg & 0x80) != 0)
            {       // ADPCM 停止中
                return -2147483648;// 0x80000000;
            }
            RateCounter -= AdpcmRate;
            while (RateCounter < 0)
            {
                if (PcmKind == 5)
                {   // 16bitPCM
                    int dataH, dataL;
                    dataH = DmaGetByte();
                    if (dataH == -2147483648)//0x80000000)
                    {
                        RateCounter = 0;
                        AdpcmReg = 0xC7;    // ADPCM 停止
                        return -2147483648;// 0x80000000;
                    }
                    dataL = DmaGetByte();
                    if (dataL == -2147483648)//0x80000000)
                    {
                        RateCounter = 0;
                        AdpcmReg = 0xC7;    // ADPCM 停止
                        return -2147483648;// 0x80000000;
                    }
                    pcm16_2pcm((int)(short)((dataH << 8) | dataL)); // OutPcm に値が入る
                }
                else if (PcmKind == 6)
                {   // 8bitPCM
                    int data;
                    data = DmaGetByte();
                    if (data == -2147483648)//0x80000000)
                    {
                        RateCounter = 0;
                        AdpcmReg = 0xC7;    // ADPCM 停止
                        return -2147483648;// 0x80000000;
                    }
                    pcm16_2pcm((int)(char)data);    // InpPcm に値が入る
                }
                else
                {
                    if (N1DataFlag == 0)
                    {       // 次のADPCMデータが内部にない場合
                        int N10Data;    // (N1Data << 4) | N0Data
                        N10Data = DmaGetByte(); // DMA転送(1バイト)
                        if (N10Data == -2147483648)//0x80000000)
                        {
                            RateCounter = 0;
                            AdpcmReg = 0xC7;    // ADPCM 停止
                            return -2147483648;// 0x80000000;
                        }
                        adpcm2pcm((byte)(N10Data & 0x0F));  // InpPcm に値が入る
                        N1Data = (N10Data >> 4) & 0x0F;
                        N1DataFlag = 1;
                    }
                    else
                    {
                        adpcm2pcm((byte)N1Data);          // InpPcm に値が入る
                        N1DataFlag = 0;
                    }
                }
                RateCounter += 15625 * 12 * 4;
            }
            OutInpPcm = (InpPcm << 9) - (InpPcm_prev << 9) + OutInpPcm - (OutInpPcm >> 5) - (OutInpPcm >> 10);
            InpPcm_prev = InpPcm;
            OutPcm = OutInpPcm - OutInpPcm_prev + OutPcm - (OutPcm >> 8) - (OutPcm >> 9) - (OutPcm >> 12);
            OutInpPcm_prev = OutInpPcm;
            return ((OutPcm >> 9) * Volume) >> 4;
        }

        public int Out(byte[] adrsBuf, uint adrsPtr, int mode, int len)
        {
            if (len <= 0)
            {
                if (len < 0)
                {
                    return GetRest();
                }
                else
                {
                    DmaMtc = 0;
                    return 0;
                }
            }
            AdpcmReg = 0xC7;    // ADPCM 停止
            DmaMtc = 0;
            DmaMarBuf = adrsBuf;//ここで代入してもどこからも参照されない
            DmaMarPtr = adrsPtr;
            SetMode(mode);
            if ((mode & 3) != 0)
            {
                DmaMtc = (uint)len;
                Reset();
                AdpcmReg = 0x47;    // ADPCM 動作開始
                DmaOcr = 0;         // チェイン動作なし
            }
            return 0;
        }

        public int Aot(byte[] tblBuf, uint tblPtr, int mode, int cnt)
        {
            if (cnt <= 0)
            {
                if (cnt < 0)
                {
                    return GetRest();
                }
                else
                {
                    DmaMtc = 0;
                    return 0;
                }
            }
            AdpcmReg = 0xC7;    // ADPCM 停止
            DmaMtc = 0;
            DmaBarBuf = tblBuf;//ここで代入してもどこからも参照されない
            DmaBarPtr = tblPtr;
            DmaBtc = (uint)cnt;
            SetMode(mode);
            if ((mode & 3) != 0)
            {
                DmaArrayChainSetNextMtcMar();
                Reset();
                AdpcmReg = 0x47;    // ADPCM 動作開始
                DmaOcr = 0x08;      // アレイチェイン
            }
            return 0;
        }

        public int Lot(byte[] tblBuf, uint tblPtr, int mode)
        {
            AdpcmReg = 0xC7;    // ADPCM 停止
            DmaMtc = 0;
            DmaBarBuf = tblBuf;//ここで代入してもどこからも参照されない
            DmaBarPtr = tblPtr;
            SetMode(mode);
            if ((mode & 3) != 0)
            {
                DmaLinkArrayChainSetNextMtcMar();
                Reset();
                AdpcmReg = 0x47;    // ADPCM 動作開始
                DmaOcr = 0x0c;      // リンクアレイチェイン
            }
            return 0;
        }

        public int SetMode(int mode)
        {
            int m;
            m = (mode >> 16) & 0xFF;
            if (m != 0xFF)
            {
                m &= 15;
                Volume = global.PCM8VOLTBL[m];
                Mode = (int)(((uint)Mode & 0xFF00FFFF) | (uint)(m << 16));
            }
            m = (mode >> 8) & 0xFF;
            if (m != 0xFF)
            {
                m &= 7;
                AdpcmRate = global.ADPCMRATEADDTBL[m];
                PcmKind = m;
                Mode = (int)(((uint)Mode & 0xFFFF00FF) | (uint)(m << 8));
            }
            m = (mode) & 0xFF;
            if (m != 0xFF)
            {
                m &= 3;
                if (m == 0)
                {
                    AdpcmReg = 0xC7;    // ADPCM 停止
                    DmaMtc = 0;
                }
                else
                {
                    Mode = (int)(((uint)Mode & 0xFFFFFF00) | (uint)(m));
                }
            }
            return 0;
        }

        public int GetRest()
        {
            if (DmaMtc == 0)
            {
                return 0;
            }
            if ((DmaOcr & 0x08) != 0)
            {   // チェイニング動作
                if ((DmaOcr & 0x04) == 0)
                {   // アレイチェイン
                    return -1;
                }
                else
                {                       // リンクアレイチェイン
                    return -2;
                }
            }
            return (int)DmaMtc;
        }

        public int GetMode()
        {
            return Mode;
        }
    }
}