using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class mndrv : baseDriver
    {
        public List<Tuple<string, byte[]>> ExtendFile = null;


        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
        {
            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            GD3 = getGD3Info(vgmBuf, 0);
            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;

            for (int chipID = 0; chipID < 2; chipID++)
            {
                YM2151Hosei[chipID] = Common.GetYM2151Hosei(4000000, 3579545);
                //if (model == EnmModel.RealModel)
                //{
                //    YM2151Hosei[chipID] = 0;
                //    int clock = chipRegister.getYM2151Clock((byte)chipID);
                //    if (clock != -1)
                //    {
                //        YM2151Hosei[chipID] = Common.GetYM2151Hosei(4000000, clock);
                //    }
                //}
            }

            uint memPtr = (uint)(0x03_0000);
            mm.alloc((int)(memPtr + vgmBuf.Length * 2 + 4));
            for (int i = 0; i < vgmBuf.Length; i++)
            {
                mm.Write((uint)(memPtr + vgmBuf.Length + i), vgmBuf[i]);
            }
            
            //デバッグ向け
            //if (model == enmModel.RealModel) return true;

            //mndrvの起動
            start();

            reg.D0_B = 0x01;//MND データ転送
            reg.a1 = (uint)(memPtr + vgmBuf.Length);
            reg.D1_L = (uint)vgmBuf.Length;
            _trap4_entry();
            if ((Int32)reg.D0_L < 0)
            {
                Stopped = true;
                return false;
            }
            memPtr += (uint)vgmBuf.Length;

            //pcm転送
            if (ExtendFile != null)// && model!= EnmModel.RealModel)
            {
                for (int j = 0; j < ExtendFile.Count; j++)
                {
                    mm.realloc((uint)(memPtr + ExtendFile[j].Item2.Length * 2 + 4));
                    //pcmファイルをx68メモリにコピー
                    for (int i = 0; i < ExtendFile[j].Item2.Length; i++)
                    {
                        mm.Write((uint)(memPtr + ExtendFile[j].Item2.Length + i), ExtendFile[j].Item2[i]);
                    }
                    reg.D0_B = 0x02;//PCM データ転送
                    reg.a1 = (uint)(memPtr + ExtendFile[j].Item2.Length);
                    reg.D1_L = (uint)ExtendFile[j].Item2.Length;
                    _trap4_entry();
                    if ((Int32)reg.D0_L < 0)
                    {
                        Stopped = true;
                        return false;
                    }
                    memPtr += (uint)ExtendFile[j].Item2.Length;
                }
            }

            reg.D0_B = 0x03;//MND 演奏開始
            _trap4_entry();
            if ((Int32)reg.D0_L < 0)
            {
                Stopped = true;
                return false;
            }

            return true;
        }

        public override void oneFrameProc()
        {
            //デバッグ向け
            //if (model == enmModel.RealModel) return;

            if (mm.mm == null)
            {
                return;
            }

            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x20) == 0)
                    {
                        timerOPN.timer();
                        if ((timerOPN.ReadStatus() & 3) != 0) interrupt._opn_entry();
                    }
                    else
                    {
                        timerOPM.timer();
                        if ((timerOPM.ReadStatus() & 3) != 0) interrupt._opm_entry();
                    }
                    Counter++;
                    vgmFrameCounter++;
                }

                if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x20) != 0)
                {
                    Stopped = true;
                }
                vgmCurLoop = mm.ReadUInt16(reg.a6 + dw.LOOP_COUNTER);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 gd3 = new GD3();

            int i = buf[6] * 0x100 + buf[7];
            List<byte> lst = new List<byte>();
            while (i < buf.Length && buf[i] != 0x0 && i + 1 < buf.Length && buf[i + 1] != 0x0)
            {
                lst.Add(buf[i]);
                i++;
            }
            string n = System.Text.Encoding.GetEncoding(932).GetString(lst.ToArray());
            gd3.TrackName = n;
            gd3.TrackNameJ = n;

            return gd3;
        }

        public mndrv()
        {
            reg = new reg();
            ab = new ab();
            mm = new MXDRV.xMemory();
            comanalyze = new comanalyze();
            comcmds = new comcmds();
            comlfo = new comlfo();
            comwave = new comwave();
            devmpcm = new devmpcm();
            devopm = new devopm();
            devopn = new devopn();
            devopnemu = new devopnemu();
            devpsg = new devpsg();
            devpsgemu = new devpsgemu();
            devrhy = new devrhy();
            interrupt = new interrupt();
            timerOPM = new FMTimer(true, null, 4000000);
            timerOPN = new FMTimer(false, null, 8000000);

            comanalyze.reg = reg;
            comanalyze.ab = ab;
            comanalyze.mm = mm;
            comcmds.reg = reg;
            comcmds.ab = ab;
            comcmds.mm = mm;
            comcmds.mndrv = this;
            comcmds.comlfo = comlfo;
            comlfo.reg = reg;
            comlfo.mm = mm;
            comlfo.devopm = devopm;
            comwave.reg = reg;
            comwave.ab = ab;
            comwave.mm = mm;
            comwave.comlfo = comlfo;
            devmpcm.reg = reg;
            devmpcm.mm = mm;
            devmpcm.mndrv = this;
            devmpcm.comanalyze = comanalyze;
            devmpcm.comcmds = comcmds;
            devmpcm.comlfo = comlfo;
            devmpcm.comwave = comwave;
            devmpcm.devopm = devopm;
            devopm.reg = reg;
            devopm.mm = mm;
            devopm.mndrv = this;
            devopm.comanalyze = comanalyze;
            devopm.comcmds = comcmds;
            devopm.comlfo = comlfo;
            devopm.comwave = comwave;
            devopn.reg = reg;
            devopn.mm = mm;
            devopn.mndrv = this;
            devopn.comanalyze = comanalyze;
            devopn.comcmds = comcmds;
            devopn.comlfo = comlfo;
            devopn.comwave = comwave;
            devopnemu.reg = reg;
            devopnemu.mm = mm;
            devopnemu.mndrv = this;
            devopnemu.comanalyze = comanalyze;
            devopnemu.comcmds = comcmds;
            devopnemu.comlfo = comlfo;
            devopnemu.comwave = comwave;
            devopnemu.devopn = devopn;
            devopnemu.devopm = devopm;
            devpsg.reg = reg;
            devpsg.ab = ab;
            devpsg.mm = mm;
            devpsg.mndrv = this;
            devpsg.comanalyze = comanalyze;
            devpsg.comcmds = comcmds;
            devpsg.comlfo = comlfo;
            devpsg.comwave = comwave;
            devpsg.devopn = devopn;
            devpsgemu.reg = reg;
            devpsgemu.mm = mm;
            devpsgemu.mndrv = this;
            devpsgemu.comanalyze = comanalyze;
            devpsgemu.comcmds = comcmds;
            devpsgemu.comlfo = comlfo;
            devpsgemu.comwave = comwave;
            devpsgemu.devpsg = devpsg;
            devpsgemu.devopm = devopm;
            devrhy.reg = reg;
            devrhy.mm = mm;
            devrhy.mndrv = this;
            devrhy.comcmds = comcmds;
            devrhy.devopn = devopn;
            interrupt.reg = reg;
            interrupt.ab = ab;
            interrupt.mm = mm;
            interrupt.mndrv = this;
            interrupt.devpsg = devpsg;
            interrupt.devopn = devopn;
            interrupt.devopm = devopm;
            interrupt.devmpcm = devmpcm;
            interrupt.timerOPM = timerOPM;
            interrupt.timerOPN = timerOPN;

        }





        public reg reg;
        public MXDRV.xMemory mm;
        public comanalyze comanalyze;
        public comcmds comcmds;
        public comlfo comlfo;
        public comwave comwave;
        public devmpcm devmpcm;
        public devopm devopm;
        public devopn devopn;
        public devopnemu devopnemu;
        public devpsg devpsg;
        public devpsgemu devpsgemu;
        public devrhy devrhy;
        public interrupt interrupt;
        public ab ab;
        public FMTimer timerOPM;
        public FMTimer timerOPN;

        MDSound.mpcmX68k.SETPCM tbl = new MDSound.mpcmX68k.SETPCM();
        UInt16[] vtbl = new UInt16[128];
        public MDSound.mpcmX68k m_MPCM;

        //トラップ処理(実質MPCM制御)
        public void trap(int n)
        {
            //if (model == EnmModel.RealModel) return;

            int ch = (int)reg.D0_B;

            if (m_MPCM == null) return;

            switch ((reg.D0_W >> 8) & 0xff)
            {
                case 0x00:
                    m_MPCM.KeyOn(0,ch);
                    break;
                case 0x01:
                    m_MPCM.KeyOff(0,ch);
                    break;
                case 0x02:
                    tbl.type = mm.ReadByte(0x00 + reg.a1);
                    tbl.orig = mm.ReadByte(0x01 + reg.a1);
                    tbl.adrs_buf = mm.mm;
                    tbl.adrs_ptr = (int)mm.ReadUInt32(0x04 + reg.a1);
                    tbl.size = mm.ReadUInt32(0x08 + reg.a1);
                    tbl.start = mm.ReadUInt32(0x0c + reg.a1);
                    tbl.end = mm.ReadUInt32(0x10 + reg.a1);
                    tbl.count = mm.ReadUInt32(0x14 + reg.a1);
                    m_MPCM.SetPcm(0, ch, tbl);
                    break;
                case 0x04:
                    m_MPCM.SetPitch(0,ch, (int)reg.D1_L);
                    break;
                case 0x05:
                    m_MPCM.SetVol(0, ch, (int)(reg.D1_B));
                    break;
                case 0x06:
                    m_MPCM.SetPan(0, ch, (int)(reg.D1_B));
                    break;
                case 0x80:
                    switch (reg.D0_B)
                    {
                        case 0x02:
                            m_MPCM.Reset(0);
                            break;
                        case 0x05:
                            for (int i = 0; i < 128; i++)
                            {
                                vtbl[i] = mm.ReadUInt16((uint)(reg.a1 + (i * 2)));
                            }
                            m_MPCM.SetVolTable(0, (int)reg.D1_L, vtbl);
                            break;
                    }
                    break;
            }
        }

        //
        //	mndrv music driver
        //	Copyright(C)1997,1998,1999,2000 S.Tsuyuzaki
        //
        //	参考:	SORCERIAN for X680x0        - PROPN
        //		    music creative driver       - MCDRV
        //		    FMP SYSTEM                  - FMP
        //		    Professional Music Driver   - PMD
        //		    Z-MUSIC PERFORMANCE MANAGER - ZMSC3
        //		    MXDRV music driver          - mxdrv16y
        //

        public const int MNDVER = 17 + 1;
        public const string DRVVER = "1.37";

        //─────────────────────────────────────
        //top:
        //	.dc.w	$0137
        //	.dc.w	$0000
        //	.dc.b	'-mndrv-',0
        //─────────────────────────────────────
        //─────────────────────────────────────
        //	trap 4 entry
        //
        public void _trap4_entry()
        {
            reg spReg = new reg();
            spReg.D1_L = reg.D1_L;
            spReg.D2_L = reg.D2_L;
            spReg.D3_L = reg.D3_L;
            spReg.D4_L = reg.D4_L;
            spReg.D5_L = reg.D5_L;
            spReg.D6_L = reg.D6_L;
            spReg.D7_L = reg.D7_L;
            spReg.a0 = reg.a0;
            spReg.a2 = reg.a2;
            spReg.a3 = reg.a3;
            spReg.a4 = reg.a4;
            spReg.a5 = reg.a5;
            spReg.a6 = reg.a6;

            reg.a6 = _work_top;// mm.ReadUInt32(_work_top);
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x04));
            reg.D0_W &= 0xff;
            reg.D0_W += 1;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;

            //_trap_table
            switch (reg.D0_W)
            {
                case 2:
                    _t_release();           // 00 driver release
                    break;
                case 4:
                    _t_trans_mnd();         // 01 copy to buffer
                    break;
                case 6:
                    _t_trans_pcm();		    // 02 copy to buffer
                    break;
                case 8:
                    _t_play_music();		// 03 演奏開始
                    break;
                case 10:
                    _t_pause();		        // 04 一時停止
                    break;
                case 12:
                    _t_stop_music();		// 05 演奏停止
                    break;
                case 14:
                    _t_get_title();	// 06 タイトル取得
                    break;
                case 16:
                    _t_get_work();		// 07 システムワーク取得
                    break;
                case 18:
                    _t_get_track_work();	// 08 トラックワークアドレス取得
                    break;
                case 20:
                    _t_get_trwork_size();// 09 track work size 取得
                    break;
                case 22:
                    _t_set_master_vol(); // 0A マスターボリューム
                    break;
                case 24:
                    _t_track_mask();     // 0B トラックマスク
                    break;
                case 26:
                    _t_key_mask();		// 0C キーコントロール制御
                    break;
                case 28:
                    _t_fadeout();        // 0D FADEOUT
                    break;
                case 30:
                    _t_purge();		    // 0E memory purge
                    break;
                case 32:
                    _t_set_pcmname();    // 0F pcmname set
                    break;
                case 34:
                    _t_get_pcmname();    // 10 pcmname get
                    break;
                case 36:
                    _t_chk_pcmname();    // 11 pcmname check
                    break;
                case 38:
                    _t_get_loopcount();  // 12 __MN_GETLOOPCOUNT
                    break;
                case 40:
                    _t_intexec();		// 13 __MN_INTEXEC
                    break;
                case 42:
                    _t_set_subevent();	// 14 __MN_SETSUBEVENT
                    break;
                case 44:
                    _t_unremove();		// 15 __MN_UNREMOVE
                    break;
                case 46:
                    _t_get_status();		// 16 __MN_GETSTATUS
                    break;
                case 48:
                    _t_get_tempo();		// 17 __MN_GETTEMPO
                    break;
                case 50:
                    _t_nop();	    // 18
                    break;
                case 52:
                    _t_nop();	    // 19
                    break;
                case 54:
                    _t_nop();	    // 1A
                    break;
                case 56:
                    _t_nop();	    // 1B
                    break;
                case 58:
                    _t_nop();	    // 1C
                    break;
                case 60:
                    _t_nop();	    // 1D
                    break;
                case 62:
                    _t_nop();	    // 1E
                    break;
                case 64:
                    _t_nop();	    // 1F
                    break;
            }

            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0xfb));
            reg.D1_L = spReg.D1_L;
            reg.D2_L = spReg.D2_L;
            reg.D3_L = spReg.D3_L;
            reg.D4_L = spReg.D4_L;
            reg.D5_L = spReg.D5_L;
            reg.D6_L = spReg.D6_L;
            reg.D7_L = spReg.D7_L;
            reg.a0 = spReg.a0;
            reg.a2 = spReg.a2;
            reg.a3 = spReg.a3;
            reg.a4 = spReg.a4;
            reg.a5 = spReg.a5;
            reg.a6 = spReg.a6;
        }

        public void _t_nop()
        {
            reg.D0_L = 0xffffffff;//-1
        }

        public const string M_keeptitle = "mndrv music driver";

        //─────────────────────────────────────
        //	MNCALL 0
        //	常駐解除(実際は再生停止と初期化のみ)
        //
        public void _t_release()
        {
            if (mm.ReadUInt16(reg.a6 + dw.UNREMOVE) != 0)
            {
                reg.D0_L = 0xffffffff;//-1
                return;
            }

            UInt32 sp = reg.a1;
            _d_stop_music();
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x20));
            _dev_reset();
            reg.D0_L = 0xffffffff;//-1
            SUBEVENT();
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) != 0)
            {
                reg.D0_W = 0x8001;
                reg.a1 = 0;//M_keeptitle(pc)
                trap(1);
            }

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x08) != 0)
            {
                reg.D1_L = 0xffffffff;//-1
                reg.D0_W = 0x8001;
                trap(3);
            }

            _vec_release();
            reg.a1 = sp;

            //(常駐解除処理)

            reg.D0_L = 0;
        }

        //─────────────────────────────────────
        //	MNCALL 1
        //	データを内部バッファにコピー
        //
        //	in	d1 : len
        //		a1 : pointer
        //
        public void _t_trans_mnd()
        {
            UInt32 sp = reg.D1_L;
            _d_stop_music();
            _reset_work();
            _dev_reset();
            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.MMLBUFADR);
            if (reg.D0_L != 0)
            {
                reg.D1_L = reg.D0_L;
                _MCMFREE();
            }

            reg.D1_L = sp;
            if (reg.D1_L == 0)
            {
                reg.D0_L = 0;//lenが0の場合はコピーせずに成功として処理終了
                return;
            }
            if ((sbyte)_MCMALLOC() < 0)
            {
                //確保失敗
                reg.D0_L = 0xffffffff;//-1
                return;
            }

            sp = reg.a1;
            mm.Write(reg.a6 + dw.MMLBUFADR, reg.D0_L);
            reg.a0 = reg.a1;
            reg.a1 = reg.D0_L;
            reg.D0_L = reg.D1_L;
            HSCOPY();
            reg.a1 = sp;
            reg.D0_L = 0;
        }

        //─────────────────────────────────────
        //	MNCALL 2
        //	PCMデータを内部バッファにコピー
        //
        //	in	d1 : len
        //		a1 : pointer
        //
        public void _t_trans_pcm()
        {
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0xed));
            if (reg.D1_L != 0)
            {
                _t_trans_pcm_();
                return;
            }

            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.PCMBUFADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.PCMBUFADR, (UInt32)0);
            }
            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.MPCMWORKADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.MPCMWORKADR, (UInt32)0);
            }
            reg.D0_L = 0;
        }

        public void _t_trans_pcm_()
        {
            UInt32 sp = reg.D1_L;

            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.PCMBUFADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.PCMBUFADR, (UInt32)0);
            }
            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.MPCMWORKADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.MPCMWORKADR, (UInt32)0);
            }
            reg.D1_L = sp;
            if ((sbyte)_MCMALLOC() < 0)
            {
                //_t_trans_pcm_err:
                reg.D0_L = 0xffffffff;
                return;
            }

            mm.Write(reg.a6 + dw.PCMBUFADR, reg.D0_L);
            mm.Write(reg.a6 + dw.PCMBUF_ENDADR, reg.D0_L);
            mm.Write(reg.a6 + dw.PCMBUF_ENDADR, (UInt32)(mm.ReadUInt32(reg.a6 + dw.PCMBUF_ENDADR) + (Int32)reg.D1_L));
            sp = reg.a1;
            reg.a0 = reg.a1;
            reg.a1 = reg.D0_L;
            reg.D0_L = reg.D1_L;
            HSCOPY();
            reg.a1 = sp;

            // PCM data analyze
            reg.a0 = mm.ReadUInt32(reg.a6 + dw.PCMBUFADR);
            UInt32 vl = mm.ReadUInt32(reg.a0); reg.a0 += 4;
            if (vl - 0x1a5a6d61 != 0)
            {
                _trans_nozpd();
                return;
            }
            vl = mm.ReadUInt32(reg.a0); reg.a0 += 4;
            if (vl - 0x4450634d != 0)// 'DPcM' = 0x4450634d
            {
                _trans_nozpd();
                return;
            }

            reg.a0 += 4;
            reg.D1_L = mm.ReadUInt32(reg.a0); reg.a0 += 4;
            mm.Write(reg.a6 + dw.ZPDCOUNT, reg.D1_L);
            reg.D7_L = reg.D1_L;
            reg.D1_L = P._pcm_work_size * reg.D1_L;
            if ((sbyte)_MCMALLOC() < 0)
            {
                reg.D0_L = 0xffffffff;
                return;
            }

            mm.Write(reg.a6 + dw.MPCMWORKADR, reg.D0_L);
            reg.a2 = reg.D0_L;
            reg.D7_L -= 1;

            _t_trans_pcm_ana:
            mm.Write(reg.a2, (UInt16)mm.ReadUInt16(reg.a0)); reg.a0 += 2; reg.a2 += 2;
            mm.Write(reg.a2, (UInt32)mm.ReadUInt32(reg.a0)); reg.a0 += 4; reg.a2 += 4;
            reg.D0_L = mm.ReadUInt32(reg.a0); reg.a0 += 4;
            reg.D0_L += reg.a0;
            mm.Write(reg.a2, reg.D0_L); reg.a2 += 4;
            mm.Write(reg.a2, (UInt32)mm.ReadUInt32(reg.a0)); reg.a0 += 4; reg.a2 += 4;
            mm.Write(reg.a2, (UInt32)mm.ReadUInt32(reg.a0)); reg.a0 += 4; reg.a2 += 4;
            mm.Write(reg.a2, (UInt32)mm.ReadUInt32(reg.a0)); reg.a0 += 4; reg.a2 += 4;
            mm.Write(reg.a2, (UInt32)mm.ReadUInt32(reg.a0)); reg.a0 += 4; reg.a2 += 4;
            reg.D0_L = mm.ReadUInt32(reg.a0); reg.a0 += 4;
            reg.D0_L = mm.ReadUInt32(reg.a0); reg.a0 += 4;

            byte vb;
            do
            {
                vb = mm.ReadByte(reg.a0++);
            } while (vb != 0);

            reg.D0_L = reg.a0;
            byte cf = (byte)(reg.D0_B & 1);
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a0 += 1;
            }
            if (reg.D7_W-- != 0) goto _t_trans_pcm_ana;

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x02));
            }
            reg.D0_L = 0;
        }

        //─────────────────────────────────────
        public void _trans_nozpd()
        {
            reg.D1_L = P._pcm_work_size;
            if ((sbyte)_MCMALLOC() < 0)
            {
                reg.D0_L = 0xffffffff;
                return;
            }
            mm.Write(reg.a6 + dw.MPCMWORKADR, reg.D0_L);
            reg.a3 = reg.D0_L;
            reg.D0_L = 0;
            mm.Write(reg.a6 + dw.ZPDCOUNT, reg.D0_L);
            mm.Write(reg.a3, (UInt16)reg.D0_W); reg.a3 += 2;// 登録番号
            mm.Write(reg.a3, 0xff); reg.a3++;//登録タイプ (ADPCMと仮定)
            mm.Write(reg.a3, 0xff); reg.a3++;//オリジナルキー
            mm.Write(reg.a3, (byte)reg.D0_B); reg.a3++;//属性
            mm.Write(reg.a3, (byte)reg.D0_B); reg.a3++;//reserved
            mm.Write(reg.a3, reg.D0_L); reg.a3 += 4;//データアドレス
            mm.Write(reg.a3, reg.D0_L); reg.a3 += 4;//データサイズ
            mm.Write(reg.a3, reg.D0_L); reg.a3 += 4;//ループ開始ポイント
            mm.Write(reg.a3, reg.D0_L); reg.a3 += 4;//ループ終了ポイント
            mm.Write(reg.a3, (UInt32)1);//ループ回数

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x12));
            }
            reg.D0_L = 0;
        }

        //============================================================
        //		高速データ転送ルーチン
        //	in	d0.l	データのバイト数
        //		a0	転送元アドレス(絶対偶数)
        //		a1	転送先アドレス(   〃   )
        //============================================================
        //					from MCDRV.s (MCDRV)

        public void HSCOPY()
        {
            reg spReg = new reg();
            spReg.D1_L = reg.D1_L;
            spReg.D2_L = reg.D2_L;
            spReg.D3_L = reg.D3_L;
            spReg.D4_L = reg.D4_L;
            spReg.D5_L = reg.D5_L;
            spReg.D6_L = reg.D6_L;
            spReg.D7_L = reg.D7_L;
            spReg.a0 = reg.a0;
            spReg.a1 = reg.a1;
            spReg.a2 = reg.a2;
            spReg.a3 = reg.a3;

            reg.D1_L = reg.D0_L;
            if (reg.a1 > reg.a0) goto hscopy_rf;// 転送元が転送先より下位にある場合の転送へ
            if (reg.a1 == reg.a0) goto hscopy90;

            //hscopy_fr:
            reg.D0_W &= 0x7f;// 余り128バイトの転送準備
            reg.D1_L >>= 7;// 128バイト単位の転送準備
            goto hscopy18;

            hscopy10:
            for (int i = 0; i < 32; i++)
            {
                mm.Write(reg.a1, mm.ReadUInt32(reg.a0));
                reg.a0 += 4;
                reg.a1 += 4;
            }

            hscopy18:
            reg.D1_L--;
            if ((Int32)reg.D1_L >= 0) goto hscopy10;
            goto hscopy28;

            hscopy20:
            mm.Write(reg.a1, mm.ReadUInt32(reg.a0));
            reg.a1++; reg.a0++;
            hscopy28:
            if ((reg.D0_W--) != 0) goto hscopy20;
            goto hscopy90;// 転送終了


            hscopy_rf:
            reg.a0 += (UInt32)(Int32)reg.D0_L;// ブロック後方から転送をする
            reg.a1 += (UInt32)(Int32)reg.D0_L;
            reg.D0_W &= 0x7f;// 余り128バイトの転送準備
            reg.D1_L >>= 7;// 128バイト単位の転送準備

            goto hscopy68;

            hscopy60:
            reg.a1--; reg.a0--;// 余り128バイトをコピーする
            mm.Write(reg.a1, mm.ReadUInt32(reg.a0));
            hscopy68:
            if ((reg.D0_W--) != 0) goto hscopy60;

            goto hscopy58;

            hscopy50:
            for (int i = 0; i < 32; i++)
            {
                mm.Write(reg.a1, mm.ReadUInt32(reg.a0));
                reg.a0 -= 4;
                reg.a1 -= 4;
            }

            hscopy58:
            reg.D1_L--;
            if ((Int32)reg.D1_L >= 0) goto hscopy50;

            hscopy90:
            reg.D1_L = spReg.D1_L;
            reg.D2_L = spReg.D2_L;
            reg.D3_L = spReg.D3_L;
            reg.D4_L = spReg.D4_L;
            reg.D5_L = spReg.D5_L;
            reg.D6_L = spReg.D6_L;
            reg.D7_L = spReg.D7_L;
            reg.a0 = spReg.a0;
            reg.a1 = spReg.a1;
            reg.a2 = spReg.a2;
            reg.a3 = spReg.a3;

        }

        //─────────────────────────────────────
        //	MNCALL 3
        //	データを解析し，演奏を開始する
        //
        public void _t_play_music()
        {
            _d_stop_music();
            _reset_work();
            _dev_reset();

            reg.a0 = mm.ReadUInt32(reg.a6 + dw.MMLBUFADR);
            if (mm.ReadUInt32(reg.a0) - 0x4d4e441a == 0) goto L1;

            _play_music_error:
            reg.D0_L = 0xffffffff;//-1
            return;
            _play_music_ver_err:
            reg.D0_L = 0xfffffffe;//-2
            return;

            L1:
            //	ori.w	#$700,sr		; Wed Mar 22 06:09 JST 2000 (saori)
            UInt32 sr = 0; sr |= 0x700; // ?

            reg.D0_L = 0;
            mm.Write(reg.a6 + dw.LFO_FLAG, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.VOL_PTR, reg.D0_L);
            mm.Write(reg.a6 + dw.ENV_PTR, reg.D0_L);
            mm.Write(reg.a6 + dw.WAVE_PTR, reg.D0_L);
            mm.Write(reg.a6 + dw.TITLE_PTR, reg.D0_L);
            mm.Write(reg.a6 + dw.TONE_PTR, reg.D0_L);
            mm.Write(reg.a6 + dw.SEQ_DATA_PTR, reg.D0_L);
            mm.Write(reg.a6 + dw.FADEFLAG, reg.D0_L);
            mm.Write(reg.a6 + dw.MASTER_VOL_FM, reg.D0_L);
            mm.Write(reg.a6 + dw.CH3KOM, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.CH3KOS, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.CH3MODEM, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.CH3MODES, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.TEMPO2, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.LOOP_COUNTER, (UInt16)reg.D0_W);
            mm.Write(reg.a6 + dw.VOLMODE, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.EMUMODE, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.NOISE_M, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.NOISE_S, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.NOISE_O, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.PSGMIX_M, (UInt16)0x3838);
            mm.Write(reg.a6 + dw.DIV, (UInt16)0xc0);
            mm.Write(reg.a6 + dw.TEMPO, (byte)0xc6);
            mm.Write(reg.a6 + dw.MUTE, (byte)0xff);
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0xfe));
            mm.Write(reg.a6 + dw.RHY_TV, (byte)63);
            mm.Write(reg.a6 + dw.RHY_DAT, (byte)0);
            mm.Write(reg.a6 + dw.RHY_DAT2, (byte)0);

            reg.D0_L = 4;//mnd version
            reg.D2_W = mm.ReadUInt16(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D2_W == 0) goto _play_music_error;
            if (reg.D2_B >= MNDVER) goto _play_music_ver_err;
            if (reg.D2_B - 1 == 0) goto _play_music_ver_err;
            mm.Write(reg.a6 + dw.MND_VER, (byte)reg.D2_B);

            Action act = comanalyze._track_ana_rest_old;
            if (mm.ReadByte(reg.a6 + dw.MND_VER) >= 13)
            {
                act = comanalyze._track_ana_rest_new;
            }
            mm.Write(reg.a6 + dw.TRKANA_RESTADR, ab.dummyAddress);//dummy
            if (ab.hlTRKANA_RESTADR.ContainsKey(reg.a6)) ab.hlTRKANA_RESTADR.Remove(reg.a6);
            ab.hlTRKANA_RESTADR.Add(reg.a6, act);

            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x20));
            if ((reg.D2_L & 0x4000) != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)0x80);
            }
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x1) == 0)
            {
                if (reg.D2_B < 6)
                {
                    mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0xdf));
                }
                if ((reg.D2_L & 0x8000) != 0)
                {
                    mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0xdf));
                }
            }

            reg.D0_W += 2;
            reg.D3_W = mm.ReadUInt16(reg.a0 + (UInt32)(Int16)reg.D0_W);
            reg.D0_W += 2;
            if (reg.D0_W - reg.D3_W == 0) goto _play_music_error;

            //
            // tone
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            reg.D0_W += 4;
            reg.a2 = reg.a0 + reg.D1_L;
            mm.Write(reg.a6 + dw.TONE_PTR, reg.a2);
            reg.a2 += 4;
            reg.D4_W = mm.ReadUInt16(reg.a2);reg.a2 += 2;
            mm.Write(reg.a6 + dw.VOICENUM, (UInt16)reg.D4_W);
            //
            // title
            //
            if (reg.D0_W - reg.D3_W == 0) goto _play_music_error;
            reg.a2 -= reg.a2;
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
            }
            mm.Write(reg.a6 + dw.TITLE_PTR, reg.a2);
            reg.D0_W += 4;
            if (reg.D0_W - reg.D3_W == 0) goto _play_music_error;
            //
            // sequence data
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
                mm.Write(reg.a6 + dw.SEQ_DATA_PTR, reg.a2);
            }
            reg.D0_W += 4;
            if (reg.D0_W - reg.D3_W == 0) goto _track_ana;
            //
            // pcm table
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
            }
            reg.D0_W += 4;
            if (reg.D0_W - reg.D3_W == 0) goto _track_ana;
            //
            // wave data
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
                mm.Write(reg.a6 + dw.WAVE_PTR, reg.a2);
            }
            reg.D0_W += 4;
            if (reg.D0_W - reg.D3_W == 0) goto _track_ana;
            //
            // envelope data
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
                mm.Write(reg.a6 + dw.ENV_PTR, reg.a2);
                reg.a2 += 4;
                reg.D4_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
                mm.Write(reg.a6 + dw.ENVNUM, (UInt16)reg.D4_W);
            }
            reg.D0_W += 4;
            if (reg.D0_W - reg.D3_W == 0) goto _track_ana;
            //
            // volume table
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
                mm.Write(reg.a6 + dw.VOL_PTR, reg.a2);
            }
            reg.D0_W += 4;
            if (reg.D0_W - reg.D3_W == 0) goto _track_ana;
            //
            // common command
            //
            reg.D1_L = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            if (reg.D1_L != 0)
            {
                reg.a2 = reg.a0 + reg.D1_L;
                _common_analyze();
            }

            //─────────────────────────────────────
            _track_ana:
            reg.a2 = mm.ReadUInt32(reg.a6 + dw.SEQ_DATA_PTR);
            reg.a0 = reg.a2;
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            mm.Write(reg.a6 + dw.USE_TRACK, (UInt16)reg.D0_W);
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.D3_B = reg.D2_B;

            _track_ana_loop:
            reg.D1_L = mm.ReadUInt32(reg.a2); reg.a2 += 4;
            reg.a3 = reg.a0 + reg.D1_L;
            mm.Write(reg.a5 + w.dataptr, reg.a3);

#if DEBUG
            log.Write(string.Format("TrackWorkAdr:{0:x}", reg.a5));
            log.Write(string.Format("DataPtr:{0:x}", reg.a3));
#endif

            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a2++);
            mm.Write(reg.a5 + w.ch, (byte)reg.D1_B);
            reg.a3 = cw._ch_table;
            //mm.Write(reg.a5 + w.dev, mm.ReadByte(reg.a3 + reg.D1_W));
            mm.Write(reg.a5 + w.dev, _ch_table[reg.D1_W]);

            reg.D2_B = mm.ReadByte(reg.a2++);
            if (reg.D3_B >= 7)
            {
                mm.Write(reg.a5 + w.track_vol, (byte)reg.D2_B);
            }
            reg.a2 += 2;
            //
            //	set track work
            //
            mm.Write(reg.a5 + w.len, 2);
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 1));
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x10));
            mm.Write(reg.a5 + w.q, 0x10);
            mm.Write(reg.a5 + w.banktone, 0xff);
            act = comcmds._atq_16;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.qtjob, reg.a3);
            if (ab.hlw_qtjob.ContainsKey(reg.a5)) ab.hlw_qtjob.Remove(reg.a5);
            ab.hlw_qtjob.Add(reg.a5, act);
            mm.Write(reg.a5 + w.volmode, mm.ReadByte(reg.a6 + dw.VOLMODE));

            reg.D4_L = 0;
            mm.Write(reg.a5 + w.addkeycode2, (UInt16)reg.D4_W);
            mm.Write(reg.a5 + w.addvolume2, (UInt16)reg.D4_W);

            _track_init();

            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            reg.D0_W -= 1;
            if (reg.D0_W != 0) goto _track_ana_loop;

            //─────────────────────────────────────
            reg.D0_L = 0xc0;
            reg.a4 = reg.a6 + dw.M_BD;

            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);

            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mm.Write(reg.a4++, (byte)reg.D0_B);

            //_timer_start:
            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG);
            reg.D0_B &= 0b0010_0001;
            if (reg.D0_B != 0) goto _start_opm;

            reg.D7_L = 0;
            reg.D1_L = 0x26; //timer-B
            reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO);
            _OPN_WRITE();

            reg.D1_L = 0x29;
            reg.D0_L = 0x83;
            _OPN_WRITE();

            reg.D1_L = 0x27;
            reg.D0_L = 0x3f;
            _OPN_WRITE();

            reg.D0_L = 0x1c;
            reg.D1_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG3);
            reg.D1_B &= 0xc0;
            if (reg.D1_B == 0)
            {
                reg.D0_L = 0x1d;
            }
            reg.D7_L = 0x03;
            reg.D1_L = 0x10;
            _OPN_WRITE();

            goto _play_exit;

            _start_opm:
            reg.D1_L = 0x12;
            reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO);
            _OPM_WRITE();

            reg.D1_L = 0x14;
            reg.D0_L = 0x3f;
            _OPM_WRITE();

            _play_exit:
            reg.D0_L = 3;
            SUBEVENT();

            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)0x80);
            reg.D0_L = 0;
        }

        //─────────────────────────────────────
        public void _track_init()
        {
            if (reg.D1_B >= 0xB0)
            {
                _track_nop();
                return;
            }
            reg.D1_W += 1;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            switch (reg.D1_W)
            {
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                case 12:
                case 14:
                case 16:
                case 18:
                case 20:
                case 22:
                case 24:
#if DEBUG
                    log.Write(string.Format("Track : OPN {0}", reg.D1_W / 2));
#endif
                    _track_opn();
                    break;
                case 26:
                case 28:
                case 30:
                case 32:
                    _track_nop();
                    break;
                case 34:
                case 36:
                case 38:
                case 40:
                case 42:
                case 44:
                case 46:
                case 48:
                case 50:
                case 52:
                case 54:
                case 56:
                case 58:
                case 60:
                case 62:
                case 64:
                    _track_nop();
                    break;
                case 66:
                case 68:
                case 70:
                case 72:
                case 74:
                case 76:
#if DEBUG
                    log.Write(string.Format("Track : PSG {0}", (reg.D1_W - 64) / 2));
#endif
                    _track_psg();
                    break;
                case 78:
                case 80:
                case 82:
                case 84:
                case 86:
                case 88:
                case 90:
                case 92:
                case 94:
                case 96:
                    _track_nop();
                    break;
                case 98:
                case 100:
                case 102:
                case 104:
                case 106:
                case 108:
                case 110:
                case 112:
                case 114:
                case 116:
                case 118:
                case 120:
                case 122:
                case 124:
                case 126:
                case 128:
                    _track_nop();
                    break;
                case 130:
                case 132:
#if DEBUG
                    log.Write(string.Format("Track : RHY {0}", (reg.D1_W - 128) / 2));
#endif
                    _track_rhy();
                    break;
                case 134:
                case 136:
                case 138:
                case 140:
                case 142:
                case 144:
                case 146:
                case 148:
                case 150:
                case 152:
                case 154:
                case 156:
                case 158:
                case 160:
                    _track_nop();
                    break;
                case 162:
                case 164:
                case 166:
                case 168:
                case 170:
                case 172:
                case 174:
                case 176:
                case 178:
                case 180:
                case 182:
                case 184:
                case 186:
                case 188:
                case 190:
                case 192:
                    _track_nop();
                    break;
                case 194:
                case 196:
                case 198:
                case 200:
                case 202:
                case 204:
                case 206:
                case 208:
                case 210:
                case 212:
                case 214:
                case 216:
                case 218:
                case 220:
                case 222:
                case 224:
                    _track_nop();
                    break;
                case 226:
                case 228:
                case 230:
                case 232:
                case 234:
                case 236:
                case 238:
                case 240:
                case 242:
                case 244:
                case 246:
                case 248:
                case 250:
                case 252:
                case 254:
                case 256:
                    _track_nop();
                    break;
                case 258:
                case 260:
                case 262:
                case 264:
                case 266:
                case 268:
                case 270:
                case 272:
#if DEBUG
                    log.Write(string.Format("Track : OPM {0}", (reg.D1_W - 256) / 2));
#endif
                    _track_opm();
                    break;
                case 274:
                case 276:
                case 278:
                case 280:
                case 282:
                case 284:
                case 286:
                case 288:
                    _track_nop();
                    break;
                case 290:
                case 292:
                case 294:
                case 296:
                case 298:
                case 300:
                case 302:
                case 304:
                case 306:
                case 308:
                case 310:
                case 312:
                case 314:
                case 316:
                case 318:
                case 320:
                    _track_nop();
                    break;
                case 322:
                case 324:
                case 326:
                case 328:
                case 330:
                case 332:
                case 334:
                case 336:
                case 338:
                case 340:
                case 342:
                case 344:
                case 346:
                case 348:
                case 350:
                case 352:
#if DEBUG
                    log.Write(string.Format("Track : PCM {0}", (reg.D1_W - 320) / 2));
#endif
                    _track_pcm();
                    break;
            }
        }

        public void _track_nop()
        {
        }

        public void _track_rhy()
        {
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) != 0) return;

            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            mm.Write(reg.a5 + w.pan_ampm, 0xc0);

            reg.a3 = ab.dummyAddress;
            Action act = devrhy._ch_rhythm;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devopn._fm_effect_ycommand;
            mm.Write(reg.a5 + w.we_ycom_adrs, reg.a3);
            if (ab.hlw_we_ycom_adrs.ContainsKey(reg.a5)) ab.hlw_we_ycom_adrs.Remove(reg.a5);
            ab.hlw_we_ycom_adrs.Add(reg.a5, act);

            act = _track_nop;
            mm.Write(reg.a5 + w.we_tone_adrs, reg.a3);
            if (ab.hlw_we_tone_adrs.ContainsKey(reg.a5)) ab.hlw_we_tone_adrs.Remove(reg.a5);
            ab.hlw_we_tone_adrs.Add(reg.a5, act);

            mm.Write(reg.a5 + w.we_pan_adrs, reg.a3);
            if (ab.hlw_we_pan_adrs.ContainsKey(reg.a5)) ab.hlw_we_pan_adrs.Remove(reg.a5);
            ab.hlw_we_pan_adrs.Add(reg.a5, act);

        }

        public void _track_opn()
        {
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) != 0) return;

            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            reg.D2_L = 16;
            mm.Write(reg.a5 + w.vol, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.smask, 0xf0);
            mm.Write(reg.a5 + w.pan_ampm, 0xc0);

            Action act = devopn. _ch_fm_mml_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devopn._ch_fm_lfo_job;
            mm.Write(reg.a5 + w.lfojob_adrs, reg.a3);
            if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
            ab.hlw_lfojob_adrs.Add(reg.a5, act);

            act = devopn._ch_fm_softenv_job;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            if (ab.hlw_softenv_adrs.ContainsKey(reg.a5)) ab.hlw_softenv_adrs.Remove(reg.a5);
            ab.hlw_softenv_adrs.Add(reg.a5, act);

            act = devopn._FM_RR_cut;
            mm.Write(reg.a5 + w.rrcut_adrs, reg.a3);
            if (ab.hlw_rrcut_adrs.ContainsKey(reg.a5)) ab.hlw_rrcut_adrs.Remove(reg.a5);
            ab.hlw_rrcut_adrs.Add(reg.a5, act);

            act = devopn._FM_echo;
            mm.Write(reg.a5 + w.echo_adrs, reg.a3);
            if (ab.hlw_echo_adrs.ContainsKey(reg.a5)) ab.hlw_echo_adrs.Remove(reg.a5);
            ab.hlw_echo_adrs.Add(reg.a5, act);

            act = devopn._fm_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs, reg.a3);
            if (ab.hlw_keyoff_adrs.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs.Remove(reg.a5);
            ab.hlw_keyoff_adrs.Add(reg.a5, act);

            act = devopn._fm_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs2, reg.a3);
            if (ab.hlw_keyoff_adrs2.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs2.Remove(reg.a5);
            ab.hlw_keyoff_adrs2.Add(reg.a5, act);

            act = devopn._fm_command;
            mm.Write(reg.a5 + w.subcmd_adrs, reg.a3);
            if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
            ab.hlw_subcmd_adrs.Add(reg.a5, act);

            act = devopn._fm_note_set;
            mm.Write(reg.a5 + w.setnote_adrs, reg.a3);
            if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
            ab.hlw_setnote_adrs.Add(reg.a5, act);

            act = devopn._init_hlfo;
            mm.Write(reg.a5 + w.inithlfo_adrs, reg.a3);
            if (ab.hlw_inithlfo_adrs.ContainsKey(reg.a5)) ab.hlw_inithlfo_adrs.Remove(reg.a5);
            ab.hlw_inithlfo_adrs.Add(reg.a5, act);

            act = devopn._fm_effect_ycommand;
            mm.Write(reg.a5 + w.we_ycom_adrs, reg.a3);
            if (ab.hlw_we_ycom_adrs.ContainsKey(reg.a5)) ab.hlw_we_ycom_adrs.Remove(reg.a5);
            ab.hlw_we_ycom_adrs.Add(reg.a5, act);

            act = devopn._fm_effect_tone;
            mm.Write(reg.a5 + w.we_tone_adrs, reg.a3);
            if (ab.hlw_we_tone_adrs.ContainsKey(reg.a5)) ab.hlw_we_tone_adrs.Remove(reg.a5);
            ab.hlw_we_tone_adrs.Add(reg.a5, act);

            act = devopn._fm_effect_pan;
            mm.Write(reg.a5 + w.we_pan_adrs, reg.a3);
            if (ab.hlw_we_pan_adrs.ContainsKey(reg.a5)) ab.hlw_we_pan_adrs.Remove(reg.a5);
            ab.hlw_we_pan_adrs.Add(reg.a5, act);


            reg.a3 = reg.a5 + w.voltable;
            //reg.a4 = _fm_volume_table;
            reg.D1_L = 16 - 1;
            int i = 0;
            do
            {
                mm.Write(reg.a3++, _fm_volume_table[i++]);
            } while (reg.D1_W-- != 0);
            mm.Write(reg.a5 + w.volcount, 16);

        }

        public void _track_psg()
        {
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) != 0) return;

            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            reg.D2_L = 8;
            mm.Write(reg.a5 + w.vol, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));

            Action act =devpsg. _ch_psg_mml_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devpsg._ch_psg_lfo_job;
            mm.Write(reg.a5 + w.lfojob_adrs, reg.a3);
            if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
            ab.hlw_lfojob_adrs.Add(reg.a5, act);

            act = devpsg._psg_env;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            if (ab.hlw_softenv_adrs.ContainsKey(reg.a5)) ab.hlw_softenv_adrs.Remove(reg.a5);
            ab.hlw_softenv_adrs.Add(reg.a5, act);

            act = _track_nop;
            mm.Write(reg.a5 + w.rrcut_adrs, reg.a3);
            if (ab.hlw_rrcut_adrs.ContainsKey(reg.a5)) ab.hlw_rrcut_adrs.Remove(reg.a5);
            ab.hlw_rrcut_adrs.Add(reg.a5, act);

            act = devpsg._psg_echo;
            mm.Write(reg.a5 + w.echo_adrs, reg.a3);
            if (ab.hlw_echo_adrs.ContainsKey(reg.a5)) ab.hlw_echo_adrs.Remove(reg.a5);
            ab.hlw_echo_adrs.Add(reg.a5, act);

            act = devpsg._psg_env_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs, reg.a3);
            if (ab.hlw_keyoff_adrs.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs.Remove(reg.a5);
            ab.hlw_keyoff_adrs.Add(reg.a5, act);

            act = devpsg._psg_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs2, reg.a3);
            if (ab.hlw_keyoff_adrs2.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs2.Remove(reg.a5);
            ab.hlw_keyoff_adrs2.Add(reg.a5, act);

            act = devpsg._psg_command;
            mm.Write(reg.a5 + w.subcmd_adrs, reg.a3);
            if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
            ab.hlw_subcmd_adrs.Add(reg.a5, act);

            act = devpsg._psg_note_set;
            mm.Write(reg.a5 + w.setnote_adrs, reg.a3);
            if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
            ab.hlw_setnote_adrs.Add(reg.a5, act);

            act = _track_nop;
            mm.Write(reg.a5 + w.inithlfo_adrs, reg.a3);
            if (ab.hlw_inithlfo_adrs.ContainsKey(reg.a5)) ab.hlw_inithlfo_adrs.Remove(reg.a5);
            ab.hlw_inithlfo_adrs.Add(reg.a5, act);

            act = devopn._fm_effect_ycommand;
            mm.Write(reg.a5 + w.we_ycom_adrs, reg.a3);
            if (ab.hlw_we_ycom_adrs.ContainsKey(reg.a5)) ab.hlw_we_ycom_adrs.Remove(reg.a5);
            ab.hlw_we_ycom_adrs.Add(reg.a5, act);

            act = _track_nop;
            mm.Write(reg.a5 + w.we_tone_adrs, reg.a3);
            if (ab.hlw_we_tone_adrs.ContainsKey(reg.a5)) ab.hlw_we_tone_adrs.Remove(reg.a5);
            ab.hlw_we_tone_adrs.Add(reg.a5, act);
            mm.Write(reg.a5 + w.we_pan_adrs, reg.a3);
            if (ab.hlw_we_pan_adrs.ContainsKey(reg.a5)) ab.hlw_we_pan_adrs.Remove(reg.a5);
            ab.hlw_we_pan_adrs.Add(reg.a5, act);

            reg.D2_L = 0;
            reg.D2_B = mm.ReadByte(reg.a5 + w.ch);
            reg.D2_B += 8;
            reg.a3 = cw._ch_table;
            //reg.D2_B = mm.ReadByte(reg.a3 + reg.D2_W);
            reg.D2_B = _ch_table[reg.D2_W];

            reg.a3 = reg.a6 + dw.SOFTENV_PATTERN;
            reg.a3 = reg.a3 + (UInt32)(Int16)reg.D2_W;
            mm.Write(reg.a5 + w.psgenv_adrs, reg.a3);

            reg.a3 = reg.a5 + w.voltable;
            //reg.a4 = _psg_volume_table;
            reg.D1_L = 16 - 1;
            int i = 0;
            do
            {
                mm.Write(reg.a3++, _psg_volume_table[i++]);
            } while (reg.D1_W-- != 0);
            mm.Write(reg.a5 + w.volcount, 16);

        }

        public void _track_opm()
        {
            reg.D2_B = mm.ReadByte(reg.a6 + dw.EMUMODE);
            if (reg.D2_B == 0) goto _track_opm_normal;

            reg.D2_B -= 3;
            if (reg.D2_B == 0) goto _track_opm_fm7;
            reg.D2_B += 1;
            if (reg.D2_B == 0) goto _track_opm_fm6;

            reg.D2_B = mm.ReadByte(reg.a5 + w.dev);
            if (reg.D2_B >= (4 + 1)) goto _track_opm_psg_emu;
            goto _track_opm_fm_emu;

            _track_opm_fm6:
            reg.D2_B = mm.ReadByte(reg.a5 + w.dev);
            if (reg.D2_B >= (5 + 1)) goto _track_opm_psg_emu;
            goto _track_opm_fm_emu;

            _track_opm_fm7:
            reg.D2_B = mm.ReadByte(reg.a5 + w.dev);
            if (reg.D2_B >= (6 + 1)) goto _track_opm_psg_emu;
            goto _track_opm_fm_emu;

            _track_opm_fm_emu:
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x01));
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            reg.D2_L = 16;
            mm.Write(reg.a5 + w.vol, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.smask, (byte)4);
            mm.Write(reg.a5 + w.pan_ampm, (byte)0xc0);

            Action act =devopnemu._ch_fme_mml_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devopnemu._ch_fme_lfo_job;
            mm.Write(reg.a5 + w.lfojob_adrs, reg.a3);
            if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
            ab.hlw_lfojob_adrs.Add(reg.a5, act);

            act = devopnemu._fme_command;
            mm.Write(reg.a5 + w.subcmd_adrs, reg.a3);
            if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
            ab.hlw_subcmd_adrs.Add(reg.a5, act);

            act = devopnemu._fme_note_set;
            mm.Write(reg.a5 + w.setnote_adrs, reg.a3);
            if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
            ab.hlw_setnote_adrs.Add(reg.a5, act);

            goto _track_opm_common;


            _track_opm_psg_emu:
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x03));
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            reg.D2_L = 8;
            mm.Write(reg.a5 + w.vol, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.smask, (byte)4);
            mm.Write(reg.a5 + w.pan_ampm, (byte)0xc0);

            act = devpsgemu._ch_psge_mml_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devpsgemu._ch_psge_lfo_job;
            mm.Write(reg.a5 + w.lfojob_adrs, reg.a3);
            if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
            ab.hlw_lfojob_adrs.Add(reg.a5, act);

            act = devpsgemu._psge_command;
            mm.Write(reg.a5 + w.subcmd_adrs, reg.a3);
            if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
            ab.hlw_subcmd_adrs.Add(reg.a5, act);

            act = devpsgemu._psge_note_set;
            mm.Write(reg.a5 + w.setnote_adrs, reg.a3);
            if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
            ab.hlw_setnote_adrs.Add(reg.a5, act);

            goto _track_opm_common;

            _track_opm_normal:
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            reg.D2_L = 16;
            mm.Write(reg.a5 + w.vol, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.smask, (byte)4);
            mm.Write(reg.a5 + w.pan_ampm, (byte)0xc0);

            act = devopm._ch_opm_mml_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devopm._ch_opm_lfo_job;
            mm.Write(reg.a5 + w.lfojob_adrs, reg.a3);
            if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
            ab.hlw_lfojob_adrs.Add(reg.a5, act);

            act = devopm._opm_command;
            mm.Write(reg.a5 + w.subcmd_adrs, reg.a3);
            if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
            ab.hlw_subcmd_adrs.Add(reg.a5, act);

            act = devopm._opm_note_set;
            mm.Write(reg.a5 + w.setnote_adrs, reg.a3);
            if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
            ab.hlw_setnote_adrs.Add(reg.a5, act);

            _track_opm_common:
            reg.a3 = reg.a5 + w.voltable;
            //reg.a4 = _fm_volume_table;
            reg.D1_L = 16 - 1;
            int i = 0;
            do
            {
                mm.Write(reg.a3++, _fm_volume_table[i++]);
            } while (reg.D1_W-- > 0);
            mm.Write(reg.a5 + w.volcount, 16);

            act = devopm._ch_opm_softenv_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            if (ab.hlw_softenv_adrs.ContainsKey(reg.a5)) ab.hlw_softenv_adrs.Remove(reg.a5);
            ab.hlw_softenv_adrs.Add(reg.a5, act);

            act = devopm._OPM_RR_cut;
            mm.Write(reg.a5 + w.rrcut_adrs, reg.a3);
            if (ab.hlw_rrcut_adrs.ContainsKey(reg.a5)) ab.hlw_rrcut_adrs.Remove(reg.a5);
            ab.hlw_rrcut_adrs.Add(reg.a5, act);

            act = devopm._OPM_echo;
            mm.Write(reg.a5 + w.echo_adrs, reg.a3);
            if (ab.hlw_echo_adrs.ContainsKey(reg.a5)) ab.hlw_echo_adrs.Remove(reg.a5);
            ab.hlw_echo_adrs.Add(reg.a5, act);

            act = devopm._opm_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs, reg.a3);
            if (ab.hlw_keyoff_adrs.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs.Remove(reg.a5);
            ab.hlw_keyoff_adrs.Add(reg.a5, act);

            act = devopm._opm_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs2, reg.a3);
            if (ab.hlw_keyoff_adrs2.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs2.Remove(reg.a5);
            ab.hlw_keyoff_adrs2.Add(reg.a5, act);

            act = devopm._init_opm_hlfo;
            mm.Write(reg.a5 + w.inithlfo_adrs, reg.a3);
            if (ab.hlw_inithlfo_adrs.ContainsKey(reg.a5)) ab.hlw_inithlfo_adrs.Remove(reg.a5);
            ab.hlw_inithlfo_adrs.Add(reg.a5, act);

            act = devopm._opm_effect_tone;
            mm.Write(reg.a5 + w.we_tone_adrs, reg.a3);
            if (ab.hlw_we_tone_adrs.ContainsKey(reg.a5)) ab.hlw_we_tone_adrs.Remove(reg.a5);
            ab.hlw_we_tone_adrs.Add(reg.a5, act);

            act = devopm._opm_effect_pan;
            mm.Write(reg.a5 + w.we_pan_adrs, reg.a3);
            if (ab.hlw_we_pan_adrs.ContainsKey(reg.a5)) ab.hlw_we_pan_adrs.Remove(reg.a5);
            ab.hlw_we_pan_adrs.Add(reg.a5, act);

            act = devopm._opm_effect_ycommand;
            mm.Write(reg.a5 + w.we_ycom_adrs, reg.a3);
            if (ab.hlw_we_ycom_adrs.ContainsKey(reg.a5)) ab.hlw_we_ycom_adrs.Remove(reg.a5);
            ab.hlw_we_ycom_adrs.Add(reg.a5, act);
        }

        public void _track_pcm()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x80));
            mm.Write(reg.a5 + w.pcmmode, (byte)0xff);
            reg.D2_L = 64;
            mm.Write(reg.a5 + w.vol, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.pan_ampm, (byte)3);

            Action act = devmpcm._ch_mpcm_mml_job;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.mmljob_adrs, reg.a3);
            if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
            ab.hlw_mmljob_adrs.Add(reg.a5, act);

            act = devmpcm._ch_mpcm_lfo_job;
            mm.Write(reg.a5 + w.lfojob_adrs, reg.a3);
            if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
            ab.hlw_lfojob_adrs.Add(reg.a5, act);

            act = devmpcm._ch_mpcm_softenv_job;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            if (ab.hlw_softenv_adrs.ContainsKey(reg.a5)) ab.hlw_softenv_adrs.Remove(reg.a5);
            ab.hlw_softenv_adrs.Add(reg.a5, act);

            act = _track_nop;
            mm.Write(reg.a5 + w.rrcut_adrs, reg.a3);
            if (ab.hlw_rrcut_adrs.ContainsKey(reg.a5)) ab.hlw_rrcut_adrs.Remove(reg.a5);
            ab.hlw_rrcut_adrs.Add(reg.a5, act);

            act = devmpcm._mpcm_echo;
            mm.Write(reg.a5 + w.echo_adrs, reg.a3);
            if (ab.hlw_echo_adrs.ContainsKey(reg.a5)) ab.hlw_echo_adrs.Remove(reg.a5);
            ab.hlw_echo_adrs.Add(reg.a5, act);

            act = devmpcm._mpcm_keyoff;
            mm.Write(reg.a5 + w.keyoff_adrs, reg.a3);
            if (ab.hlw_keyoff_adrs.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs.Remove(reg.a5);
            ab.hlw_keyoff_adrs.Add(reg.a5, act);

            act = devmpcm._mpcm_keyoff2;
            mm.Write(reg.a5 + w.keyoff_adrs2, reg.a3);
            if (ab.hlw_keyoff_adrs2.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs2.Remove(reg.a5);
            ab.hlw_keyoff_adrs2.Add(reg.a5, act);

            act = devmpcm._mpcm_command;
            mm.Write(reg.a5 + w.subcmd_adrs, reg.a3);
            if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
            ab.hlw_subcmd_adrs.Add(reg.a5, act);

            act = devmpcm._mpcm_note_set;
            mm.Write(reg.a5 + w.setnote_adrs, reg.a3);
            if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
            ab.hlw_setnote_adrs.Add(reg.a5, act);

            act = _track_nop;
            mm.Write(reg.a5 + w.inithlfo_adrs, reg.a3);
            if (ab.hlw_inithlfo_adrs.ContainsKey(reg.a5)) ab.hlw_inithlfo_adrs.Remove(reg.a5);
            ab.hlw_inithlfo_adrs.Add(reg.a5, act);

            act = devopm._opm_effect_ycommand;
            mm.Write(reg.a5 + w.we_ycom_adrs, reg.a3);
            if (ab.hlw_we_ycom_adrs.ContainsKey(reg.a5)) ab.hlw_we_ycom_adrs.Remove(reg.a5);
            ab.hlw_we_ycom_adrs.Add(reg.a5, act);

            act = devmpcm._mpcm_effect_tone;
            mm.Write(reg.a5 + w.we_tone_adrs, reg.a3);
            if (ab.hlw_we_tone_adrs.ContainsKey(reg.a5)) ab.hlw_we_tone_adrs.Remove(reg.a5);
            ab.hlw_we_tone_adrs.Add(reg.a5, act);

            act = devmpcm._mpcm_effect_pan;
            mm.Write(reg.a5 + w.we_pan_adrs, reg.a3);
            if (ab.hlw_we_pan_adrs.ContainsKey(reg.a5)) ab.hlw_we_pan_adrs.Remove(reg.a5);
            ab.hlw_we_pan_adrs.Add(reg.a5, act);

            reg.a3 = reg.a5 + w.voltable;
            //reg.a4 = _mpcm_vol_table;
            reg.D1_L = 16 - 1;
            int i = 0;
            do
            {
                mm.Write(reg.a3++, _mpcm_vol_table[i++]);
            } while (reg.D1_W-- > 0);
            mm.Write(reg.a5 + w.volcount, 16);

        }

        //;─────────────────────────────────────
        public byte[] _fm_volume_table = new byte[] {
            0x2A,0x28,0x25,0x22,0x20,0x1D,0x1A,0x18,0x15,0x12,0x10,0x0D,0x0A,0x08,0x05,0x02
        };
        public byte[] _mpcm_vol_table = new byte[] {
            0x01,0x08,0x10,0x18,0x20,0x28,0x30,0x38,0x40,0x48,0x50,0x58,0x60,0x68,0x70,0x78
        };
        public byte[] _psg_volume_table = new byte[] {
            0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F
        };

        //─────────────────────────────────────
        //	共通コマンド解析
        //
        public void _common_analyze()
        {
            do
            {
                reg.D4_L = 0;
                reg.D4_B = mm.ReadByte(reg.a2++);
                if (reg.D4_B == 0) return;
                reg.D4_W += (UInt32)(Int16)reg.D4_W;

                switch (reg.D4_W)
                {
                    case 2:
                        _common_timer();//  01: 駆動タイマー
                        break;
                    case 4:
                        _common_lfotimer();//  02: LFOタイマー
                        break;
                    case 6:
                        _common_psgtimer();//  03: PSGタイマー
                        break;
                    case 8:
                        _common_tempo();//  04: てんぽ
                        break;
                    case 10:
                        _common_tie();//  05: タイ方式
                        break;
                    case 12:
                        _common_lfo();//  06: LFO方式
                        break;
                    case 14:
                        _common_clock();//  07: 全音符clock
                        break;
                    case 16:
                        _common_volume();//  08: 音量モード
                        break;
                    case 18:
                        _common_opnemu();//  09: OPNエミュモード
                        break;
                    case 20:
                        _common_q_mode();//  0A: @qのモード
                        break;
                    case 22:
                        _common_env_mode();//  0B: env mode
                        break;
                    case 24:
                    case 26:
                    case 28:
                    case 30:
                        _common_nop();
                        break;
                }
            } while (true);
        }

        public void _common_nop()
        {
            //	rts
        }

        //
        //	TEMPO 駆動タイマー
        //
        public void _common_timer()
        {
            reg.D4_B = mm.ReadByte(reg.a2++);
            if (reg.D4_B != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0xdf));
                return;
            }
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x20));
        }

        //
        //	LFO 駆動タイマー
        //
        public void _common_lfotimer()
        {
            reg.D4_B = mm.ReadByte(reg.a2++);
            if (reg.D4_B != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) | 0x80));// use TIMER-A
                return;
            }
            mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) & 0x7f));// use TIEMR-B
        }

        //
        //	PSG 駆動タイマー
        //
        public void _common_psgtimer()
        {
            reg.D4_B = mm.ReadByte(reg.a2++);
            if (reg.D4_B != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) | 0x40));// use TIMER-A
                return;
            }
            mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) & 0xbf));// use TIEMR-B
        }

        //
        //	初期テンポ
        //
        public void _common_tempo()
        {
            mm.Write(reg.a6 + dw.TEMPO, mm.ReadByte(reg.a2++));
        }

        //
        //	タイ動作モード
        //
        public void _common_tie()
        {
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0x3f));
            reg.D4_B = mm.ReadByte(reg.a2++);
            if (reg.D4_B == 0) return;
            reg.D4_B -= 1;
            if (reg.D4_B == 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) | 0x80));
                return;
            }
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) | 0x40));
        }

        //
        //	LFO 動作モード
        //
        public void _common_lfo()
        {
            reg.D4_B = mm.ReadByte(reg.a2++);
            if (reg.D4_B == 0) return;
            reg.D4_B -= 1;
            if (reg.D4_B == 0)
            {
                mm.Write(reg.a6 + dw.LFO_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.LFO_FLAG) | 0x80));
                return;
            }
            mm.Write(reg.a6 + dw.LFO_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.LFO_FLAG) | 0xc0));
        }

        //
        //	全音符のクロック
        //
        public void _common_clock()
        {
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            mm.Write(reg.a6 + dw.DIV, (UInt16)reg.D0_W);
        }

        //
        //	相対音量モード
        //
        public void _common_volume()
        {
            reg.D4_B = mm.ReadByte(reg.a2++);
            if (reg.D4_B != 0)
            {
                mm.Write(reg.a6 + dw.VOLMODE, 0xff);
            }
        }

        //
        //	OPNエミュレーションモード
        //
        public void _common_opnemu()
        {
            mm.Write(reg.a6 + dw.EMUMODE, mm.ReadByte(reg.a2++));
        }

        //
        //	クオンタイズモード
        //
        public void _common_q_mode()
        {
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0xef));
            if (mm.ReadByte(reg.a2++) == 0) return;
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) | 0x10));
        }

        //
        //	ソフトウェアエンベロープ
        //
        public void _common_env_mode()
        {
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0xfb));
            if (mm.ReadByte(reg.a2++) == 0) return;
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) | 0x04));
        }

        //─────────────────────────────────────
        //	MNCALL 4
        //	演奏一時停止
        public void _t_pause()
        {
            if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x20) != 0) return;
            int v = mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x40;
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) ^ 0x40));
            if (v != 0)
            {
                _pause_release();
                return;
            }
            reg.D0_L = 4;
            SUBEVENT();
            _all_mute();
        }

        public void _all_mute()
        {
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);

            //_pause_loop:
            while (reg.D7_W != 0)
            {
                if ((sbyte)(mm.ReadByte(reg.a5 + w.ch) - 0xa0) >= 0) goto L9;
                if ((sbyte)(mm.ReadByte(reg.a5 + w.ch) - 0x80) >= 0) goto L2;
                if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x1) != 0) goto L9;
                if ((sbyte)(mm.ReadByte(reg.a5 + w.ch) - 0x40) >= 0) goto L9;
                if ((sbyte)(mm.ReadByte(reg.a5 + w.ch) - 0x20) >= 0) goto L1;

                reg.D4_L = 0x7f;
                devopn._FM_F2_set();
                goto L9;

                L1:
                reg.D0_L = 0;
                devpsg._psg_volume_set2();
                goto L9;

                L2:
                reg.D4_L = 0x7f;
                devopm._OPM_F2_set();

                L9:
                reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
                reg.D7_W--;
            }
           

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) == 0) return;

            reg.D0_W = 0x01ff;
            trap(1);
            reg.D0_W = 0x13ff;
            trap(1);
        }

        public void _pause_release()
        {
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            _pause_rel_loop:
            if (mm.ReadByte(reg.a5 + w.ch) >= 0x80) goto L2b;
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x1) != 0) goto L9b;
            if (mm.ReadByte(reg.a5 + w.ch) >= 0x40) goto L9b;
            if (mm.ReadByte(reg.a5 + w.ch) >= 0x20) goto L1b;

            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D4_B += mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM);
            if ((sbyte)reg.D4_B >= 0) goto L8b;
            reg.D4_L = 0x7f;
            L8b:
            devopn._FM_F2_set();
            goto L9b;

            L1b:
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_ini);
            devpsg._psg_volume_set2();
            goto L9b;

            L2b:
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D4_B += mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0x7f;
            }
            devopm._OPM_F2_set();

            L9b:
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            reg.D7_W--;
            if (reg.D7_W != 0) goto _pause_rel_loop;

        }

        //─────────────────────────────────────
        //	MNCALL 5
        //	演奏停止
        public void _t_stop_music()
        {
            reg.D0_L = 2;
            SUBEVENT();
            _d_stop_music();
        }

        public void _d_stop_music()
        {
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)0x20);
            mm.Write(reg.a6 + dw.TEMPO, 0);

            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG);
            reg.D0_B &= 0b0010_0001;
            if (reg.D0_B != 0) goto _t_stop_music_opm;

            reg.D7_L = 0;
            reg.D1_L = 0x27;
            reg.D0_L = 0x30;
            _OPN_WRITE();
            reg.D1_L = 0x28;
            reg.D0_L = 0x80;
            _OPN_WRITE();
            _all_mute();
            return;

            _t_stop_music_opm:
            reg.D0_L = 0x30;
            reg.D1_L = 0x14;
            _OPN_WRITE();
            _all_mute();
        }

        //─────────────────────────────────────
        //	MNCALL 6
        //	タイトルデータへのポインタを取得
        //	out	a1 : title pointer
        //
        public void _t_get_title()
        {
            reg.a1 = mm.ReadUInt32(reg.a6 + dw.TITLE_PTR);
            reg.D0_L = reg.a1;
        }

        //─────────────────────────────────────
        //	MNCALL 7
        //	ワークアドレス取得
        //	out	a1 : work pointer
        //
        public void _t_get_work()
        {
            reg.a1 = _work_top;// mm.ReadUInt32(_work_top);
            reg.D0_L = reg.a1;
        }

        //─────────────────────────────────────
        //	MNCALL 8
        //	トラックワークアドレス取得
        //	out	a1 : work pointer
        //
        public void _t_get_track_work()
        {
            reg.a1 = reg.a6 + dw.TRACKWORKADR;
            reg.D0_L = reg.a1;
        }

        //─────────────────────────────────────
        //	MNCALL 9
        //	トラックワークサイズ取得
        //	out	d0 : work pointer
        //
        public void _t_get_trwork_size()
        {
            reg.D0_L = w._track_work_size;// dw._trackworksize;
        }

        //─────────────────────────────────────
        //	MNCALL $0A
        //	マスターボリューム設定
        //	in	d1 : device
        //		d2 : volume
        //
        public void _t_set_master_vol()
        {
            reg.D6_L = 1;
            reg.D6_B += (UInt32)(sbyte)reg.D1_B;
            reg.D6_W += (UInt32)(Int16)reg.D6_W;
            switch (reg.D6_W)
            {
                case 2:
                    mm.Write(reg.a6 + dw.MASTER_VOL_FM, (byte)reg.D2_B);
                    break;
                case 4:
                    mm.Write(reg.a6 + dw.MASTER_VOL_PSG, (byte)reg.D2_B);
                    break;
                case 6:
                    mm.Write(reg.a6 + dw.MASTER_VOL_RHY, (byte)reg.D2_B);
                    break;
                case 8:
                    mm.Write(reg.a6 + dw.MASTER_VOL_PCM, (byte)reg.D2_B);
                    break;
            }

        }

        //─────────────────────────────────────
        //	MNCALL $0B
        //	トラックマスク
        //	in	d1 : track
        //
        public void _t_track_mask()
        {
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.D1_W -= 1;
            L1:
            if (reg.D1_B == 0) goto L2;
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            if (reg.D1_W-- != 0) goto L1;

            L2:
            int v = mm.ReadByte(reg.a5 + w.flag2) & 0x80;
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) ^ 0x80));
            if (v != 0) return;

            if (mm.ReadByte(reg.a5 + w.ch) >= 0x40) return;
            if (mm.ReadByte(reg.a5 + w.ch) >= 0x20)
            {
                devpsg._psg_env_keyoff();
                return;
            }
            devopn._fm_keyoff();
            return;
        }

        //─────────────────────────────────────
        //	MNCALL $0C
        //	キーコントロール制御
        //	in	d1 : enabe / disable
        public void _t_key_mask()
        {
            if (reg.D1_B != 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x80));
                return;
            }
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x7f));
        }

        //─────────────────────────────────────
        //	MNCALL $0D
        //	FADEOUT
        //
        public void _t_fadeout()
        {
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) | 0x10));
            mm.Write(reg.a6 + dw.FADEFLAG, 1);
            mm.Write(reg.a6 + dw.FADESPEED, 7);
            mm.Write(reg.a6 + dw.FADESPEED_WORK, 7);
            mm.Write(reg.a6 + dw.FADECOUNT, 3);

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x01) != 0) return;

            while ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x01) != 0) ;

            //	ori.w	#$700,sr

            reg.D7_L = 3;
            reg.D1_L = 0x10;
            reg.D0_L = 0x1c;
            _OPN_WRITE();
        }

        //─────────────────────────────────────
        //	MNCALL $0E
        //	memory purge
        //
        public void _t_purge()
        {
            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.MMLBUFADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.MMLBUFADR, (UInt32)0);
            }
            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.PCMBUFADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.PCMBUFADR, (UInt32)0);
            }
            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.MPCMWORKADR);
            if (reg.D1_L != 0)
            {
                _MCMFREE();
                mm.Write(reg.a6 + dw.MPCMWORKADR, (UInt32)0);
            }
        }

        //─────────────────────────────────────
        //	MNCALL $0F
        //	name set
        //
        public void _t_set_pcmname()
        {
            reg.a2 = reg.a1;
            reg.a0 = reg.a6 + dw.ADPCMNAME;
            reg.D0_L = 96 - 1;
            //L1:
            do
            {
                mm.Write(reg.a0, mm.ReadByte(reg.a1)); reg.a0++; reg.a1++;
                reg.D0_L--;
            } while (reg.D0_L != 0); //違うかも
            //	dbeq	d0,1b
            if (reg.D0_L != 0)
            {
                reg.a0--;
                mm.Write(reg.a0, (byte)0);
            }
            reg.a1 = reg.a2;
        }

        //─────────────────────────────────────
        //	MNCALL $10
        //	name get
        //
        public void _t_get_pcmname()
        {
            reg.a1 = reg.a6 + dw.ADPCMNAME;
            reg.D0_L = reg.a1;
        }


        //─────────────────────────────────────
        //	MNCALL $11
        //	name check
        //
        public void _t_chk_pcmname()
        {
            UInt32 sp = reg.a1;
            reg.a0 = reg.a6 + dw.ADPCMNAME;
            reg.D7_L = 0xdf;
            reg.D0_L = 96 - 1;

            cmpapn10:
            reg.D1_B = mm.ReadByte(reg.a1++);
            if (reg.D1_B == 0) goto cmpapn50;
            if ((sbyte)reg.D1_B < 0)
            {
                reg.D3_B = 0xff;
            }
            reg.D2_B = mm.ReadByte(reg.a0++);
            if ((sbyte)reg.D2_B < 0)
            {
                reg.D4_B = 0xff;
            }
            reg.D3_B ^= reg.D4_B;
            if (reg.D3_B != 0) goto cmpadpn80;
            if ((sbyte)reg.D4_B >= 0)
            {
                if (reg.D1_B >= 0x61)
                {
                    if (reg.D1_B <= 0x7a)
                    {
                        reg.D1_B &= reg.D7_B;
                        reg.D2_B &= reg.D7_B;
                    }
                }
            }
            bool f = (reg.D2_B - reg.D1_B != 0);
            reg.D0_L--;
            if (reg.D0_L != 0 && f) goto cmpapn10;

            if (f) goto cmpadpn80;
            goto cmpapn60;
            cmpapn50:
            if (mm.ReadByte(reg.a0) != 0) goto cmpadpn80;
            cmpapn60:
            reg.D0_L = 0;
            goto cmpadpn90;

            cmpadpn80:
            reg.D0_L = 0xffffffff;//-1
            cmpadpn90:
            reg.a1 = sp;
        }

        //─────────────────────────────────────
        //	MNCALL $12
        //	get loopcount
        //
        public void _t_get_loopcount()
        {
            reg.D0_W = mm.ReadUInt16(reg.a6 + dw.LOOP_COUNTER);
            reg.D0_L = (UInt32)(Int16)reg.D0_W;
        }

        //─────────────────────────────────────
        //	MNCALL $13
        //	set intexec
        //
        public void _t_intexec()
        {
            reg.D1_W = mm.ReadUInt16(reg.a6 + dw.INTEXECNUM);
            reg.D0_L = 0xffffffff;
            if (reg.D1_W - 8 != 0)
            {
                reg.D1_W += (UInt32)(Int16)reg.D1_W;
                reg.D1_W += (UInt32)(Int16)reg.D1_W;
                reg.a5 = reg.a6 + dw.INTEXECBUF;
                mm.Write(reg.a5 + (UInt32)(Int16)reg.D1_W, reg.a1);
                mm.Write(reg.a6 + dw.INTEXECNUM, (UInt16)(mm.ReadUInt16(reg.a6 + dw.INTEXECNUM) + 1));
                reg.D0_L = 0;
            }
        }

        //─────────────────────────────────────
        //	MNCALL $14
        //	set subevent
        //
        public void _t_set_subevent()
        {
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            switch (reg.D1_W)
            {
                case 0:
                    goto _t_se_mode0;
                case 2:
                    goto _t_se_mode1;
                case 4:
                    goto _t_se_mode2;
            }

            _t_se_mode0:
            SRCHSSEID();
            reg.a1 = reg.a0;
            return;

            _t_se_mode1:
            bool pl = SRCHSSEID();
            if (pl) goto L9;
            reg.a0 = reg.a6 + dw.SUBEVENTADR;
            reg.D0_L = 8 - 1;
            UInt32 v;
            do
            {
                v = mm.ReadUInt32(reg.a0);
                reg.a0 += 4;
                reg.D0_L--;
                if (v == 0) break;
            } while (reg.D0_L != 0);

            if (v != 0) goto L9;
            reg.a0 -= 4;
            mm.Write(reg.a0, reg.a1);
            mm.Write(reg.a0 + 8 * 4, reg.D2_L);
            mm.Write(reg.a6 + dw.SUBEVENTNUM, (UInt16)(mm.ReadUInt16(reg.a6 + dw.SUBEVENTNUM) + 1));
            reg.D0_L = 0;
            return;

            _t_se_mode2:
            pl = SRCHSSEID();
            if (!pl) return;
            mm.Write(reg.a6 + dw.SUBEVENTNUM, (UInt16)(mm.ReadUInt16(reg.a6 + dw.SUBEVENTNUM) - 1));
            mm.Write(reg.a0, 0);
            mm.Write(reg.a0 - 8 * 4, 0);
            //L1:
            return;

            L9:
            reg.D0_L = 0xffff_ffff;
        }

        //=======================================	from MCDRV
        //		ID 検索
        // in	d2.l	ID ネーム
        // out	d1.l	アドレス
        //	a0	ID の入っているアドレス
        public bool SRCHSSEID()
        {
            reg.a0 = reg.a6 + dw.SUBEVENTID;
            reg.D0_L = 0xffff_ffff;//-1
            reg.D1_L = 8 - 1;

            //srchsseid10:
            bool flg = false;
            do
            {
                UInt32 v = mm.ReadUInt32(reg.a0);
                reg.a0 += 4;
                if (reg.D2_L - v == 0)
                {
                    flg = true;
                    break;
                }
                reg.D1_L--;
            } while (reg.D1_L != 0);
            if (!flg) goto srchsseid90;//見つからなかった

            reg.a0 -= 4;
            reg.D0_L = mm.ReadUInt32(reg.a0 - 8 * 4);//アドレス入れて終わる
            srchsseid90:
            return (Int32)reg.D0_L >= 0;//ccr へ ?
        }

        //========================================	form MCDRV
        //	サブイベントコール
        // in	d0	イベント番号
        public void SUBEVENT()
        {
            reg spReg = new reg();
            spReg.D6_L = reg.D6_L;
            spReg.D7_L = reg.D7_L;
            spReg.a0 = reg.a0;
            spReg.a2 = reg.a2;

            reg.a2 = reg.a6 + dw.SUBEVENTADR;
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.SUBEVENTNUM);
            goto subevent20;
            subevent10:
            do
            {
                reg.D6_L = mm.ReadUInt32(reg.a2);
                if (reg.D6_L != 0) break;
                reg.D7_W--;
            } while (reg.D7_W > 0);//0=未登録

            if (reg.D6_L == 0) goto subevent90;
            reg.a0 = reg.D6_L;
            //actSUBEVENT(reg.a0);// サブルーチンコール
            subevent20:
            if (reg.D7_W-- > 0) goto subevent10;

            subevent90:
            reg.D6_L = spReg.D6_L;
            reg.D7_L = spReg.D7_L;
            reg.a0 = spReg.a0;
            reg.a2 = spReg.a2;
        }

        //─────────────────────────────────────
        //	MNCALL $15
        //	unremove
        //
        public void _t_unremove()
        {
            mm.Write(reg.a6 + dw.UNREMOVE, (UInt16)(mm.ReadUInt16(reg.a6 + dw.UNREMOVE) + (Int16)reg.D1_W));
            reg.D0_W = mm.ReadUInt16(reg.a6 + dw.UNREMOVE);
            reg.D0_L = (UInt32)(Int16)reg.D0_W;
        }

        //─────────────────────────────────────
        //	MNCALL $16
        //	get status
        //
        public void _t_get_status()
        {
            reg.D0_W = (UInt16)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) * 0x100);
            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG2);
            reg.D0_L = (reg.D0_L >> 16) | (reg.D0_L << 16);
            reg.D0_W = (UInt16)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) * 0x100);
            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG);
        }

        //─────────────────────────────────────
        //	MNCALL $17
        //	get tempo
        //
        public void _t_get_tempo()
        {
            reg.D0_L = 0;
            reg.D0_W = mm.ReadUInt16(reg.a6 + dw.DIV);
            reg.D0_L = (reg.D0_L >> 16) | (reg.D0_L << 16);
            reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO);
        }

        //;─────────────────────────────────────
        public byte[] _ch_table = new byte[] {
            0x00,0x01,0x02,0x00,0x01,0x02			// FM MASTER
            ,0x00,0x01,0x02,0x00,0x01,0x02			// FM SLAVE
            ,0x00,0x00,0x00,0x00
            //				0x10～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x20～
            ,0x00,0x01,0x02,0x00,0x01,0x02,0x00,0x00		// PSG
            ,0x00,0x10,0x20,0x30,0x40,0x50,0x00,0x00		// softenv 用
            //				0x30～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x40～
            ,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00		// RHYTHM
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x50～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x60～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x70～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x80～
            ,0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07		// OPM
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0x90～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0xA0～
            ,0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07		// PCM
            ,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F
            //				0xB0～
            ,0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07		// PCM
            ,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F
            //				0xC0～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0xD0～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0xE0～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            //				0xF0～
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        };

        //─────────────────────────────────────
        //	OPN3 書き込み
        //
        // break d6-d7/a0/a3
        //
        //
        public void _OPN_WRITE4()
        {
            reg.D6_L = 0;
            reg.D6_B = mm.ReadByte(reg.a5 + w.ch);
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            _OPN_WRITE_();
        }

        public void _OPN_WRITE3()
        {
            reg.a0 = 0xecc0c1;
            reg.D6_L = 0;
            if (mm.ReadByte(reg.a5 + w.ch) < 6)
            {
                _opn_write_direct();
                return;
            }
            reg.a0 += 8;
            reg.D6_W = 0x200;
            _opn_write_direct();
        }

        public void _OPN_WRITE()
        {
            reg.D6_L = reg.D7_L;
            _OPN_WRITE_();
        }

        public void _OPN_WRITE2()
        {
            reg.D6_L = 0;
            reg.D6_B = mm.ReadByte(reg.a5 + w.ch);
            _OPN_WRITE_();
        }
        public void _OPN_WRITE_()
        {
            reg.D6_W += (UInt32)(Int16)reg.D6_W;
            reg.D6_W = _reg_table[reg.D6_W / 2];
            reg.a0 = reg.D5_L;
            reg.D5_L = 0xecc0c0;
            reg.D5_B += (UInt32)(sbyte)reg.D6_B;
            uint a = reg.a0;
            reg.a0 = reg.D5_L;
            reg.D5_L = a;
            _opn_write_direct();
        }

        public void _opn_write_direct()
        {
            reg.D6_B = reg.D1_B;
            reg.a3 = reg.a6 + dw.REGWORKADR;
            mm.Write(reg.a3 + (UInt32)(Int16)reg.D6_W, (byte)reg.D0_B);

            //while ((sbyte)mm.ReadByte(reg.a0) < 0) ; //OPN wait?
            //mm.Write(reg.a0, (byte)reg.D1_B);
            //log.Write(string.Format("adr:{0:x} dat:{1:x}", reg.a0, reg.D1_B));
            //while ((sbyte)mm.ReadByte(reg.a0) < 0) ; //OPN wait?
            //mm.Write(reg.a0 + 2, (byte)reg.D0_B);
            //log.Write(string.Format("adr:{0:x} dat:{1:x}", reg.a0+2, reg.D0_B));

            switch (reg.a0)
            {
                case 0xecc0c1:
                    chipRegister.YM2608SetRegister(0, 0, 0, (int)reg.D1_B, (int)reg.D0_B);
                    timerOPN.WriteReg((byte)reg.D1_B, (byte)reg.D0_B);
                    //log.Write(string.Format("DEV:0 PRT:0 radr:{0:x} rdat:{1:x}", reg.D1_B, reg.D0_B));
                    //if (reg.D1_B < 0x10)
                    //{
                    //log.Write(string.Format("SSG : radr:{0:x} rdat:{1:x}", reg.D1_B, reg.D0_B));
                    //}
                    break;
                case 0xecc0c5:
                    chipRegister.YM2608SetRegister(0, 0, 1, (int)reg.D1_B, (int)reg.D0_B);
                    //log.Write(string.Format("DEV:0 PRT:1 radr:{0:x} rdat:{1:x}", reg.D1_B, reg.D0_B));
                    break;
                case 0xecc0c9:
                    chipRegister.YM2608SetRegister(0, 1, 0, (int)reg.D1_B, (int)reg.D0_B);
                    //log.Write(string.Format("DEV:1 PRT:0 radr:{0:x} rdat:{1:x}", reg.D1_B, reg.D0_B));
                    break;
                case 0xecc0cd:
                    chipRegister.YM2608SetRegister(0, 1, 1, (int)reg.D1_B, (int)reg.D0_B);
                    //log.Write(string.Format("DEV:1 PRT:1 radr:{0:x} rdat:{1:x}", reg.D1_B, reg.D0_B));
                    break;
            }
        }

        public UInt16[] _reg_table = new UInt16[] {
            0x0001		// 00 FM1
            ,0x0001		// 01 FM2
            ,0x0001		// 02 FM3
            ,0x0105		// 03 FM4
            ,0x0105		// 04 FM5
            ,0x0105		// 05 FM6
            ,0x0209		// 06 FM1
            ,0x0209		// 07 FM2
            ,0x0209		// 08 FM3
            ,0x030D		// 09 FM4
            ,0x030D		// 0A FM5
            ,0x030D		// 0B FM6
            ,0x0000		// 0C
            ,0x0000		// 0D
            ,0x0000		// 0E
            ,0x0000		// 0F
                        //
            ,0x0000		// 10
            ,0x0000		// 11
            ,0x0000		// 12
            ,0x0000		// 13
            ,0x0000		// 14
            ,0x0000		// 15
            ,0x0000		// 16
            ,0x0000		// 17
            ,0x0000		// 18
            ,0x0000		// 19
            ,0x0000		// 1A
            ,0x0000		// 1B
            ,0x0000		// 1C
            ,0x0000		// 1D
            ,0x0000		// 1E
            ,0x0000		// 1F
                        //
            ,0x0001		// 20 PSG1
            ,0x0001		// 21 PSG2
            ,0x0001		// 22 PSG3
            ,0x0209		// 23 PSG1
            ,0x0209		// 24 PSG2
            ,0x0209		// 25 PSG3
            ,0x0000		// 26
            ,0x0000		// 27
            ,0x0000		// 28
            ,0x0000		// 29
            ,0x0000		// 2A
            ,0x0000		// 2B
            ,0x0000		// 2C
            ,0x0000		// 2D
            ,0x0000		// 2E
            ,0x0000		// 2F
                        //
            ,0x0000		// 30
            ,0x0000		// 31
            ,0x0000		// 32
            ,0x0000		// 33
            ,0x0000		// 34
            ,0x0000		// 35
            ,0x0000		// 36
            ,0x0000		// 37
            ,0x0000		// 38
            ,0x0000		// 39
            ,0x0000		// 3A
            ,0x0000		// 3B
            ,0x0000		// 3C
            ,0x0000		// 3D
            ,0x0000		// 3E
            ,0x0000		// 3F
                        //
            ,0x0001		// 40 RYTHM1
            ,0x0209		// 41 RYTHM2
        };

        //─────────────────────────────────────
        //	OPM 書き込み
        //
        // break a3
        //
        public void _OPM_WRITE4()
        {
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            _OPM_WRITE();
        }
        public void _OPM_WRITE()
        {
            reg.a0 = 0xe90003;

            //while ((sbyte)mm.ReadByte(reg.a0) < 0) ; //wait?
            //mm.Write(reg.a0 - 2, (byte)reg.D1_B);
            //log.Write(string.Format("adr:{0:x} dat:{1:x}", reg.a0-2, reg.D1_B));

            reg.a3 = reg.a6 + dw.OPMREGWORK;
            reg.D1_W &= 0xff;
            mm.Write(reg.a3 + (UInt32)(Int16)reg.D1_W, (byte)reg.D0_B);

            //while ((sbyte)mm.ReadByte(reg.a0) < 0) ; //wait?
            //mm.Write(reg.a0, (byte)reg.D0_B);
            //log.Write(string.Format("adr:{0:x} dat:{1:x}", reg.a0, reg.D0_B));
            //chipRegister.YM2151SetRegister(0, 0, (int)reg.D1_B, (int)reg.D0_B, YM2151Hosei[0], 0);
            timerOPM.WriteReg((byte)reg.D1_B, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	OPN/OPM RESET
        //
        //SAVEREG:	.reg	d0-d1/d5-d7/a0-a1/a3
        public void _dev_reset()
        {
            reg spReg = new reg();
            spReg.D0_L = reg.D0_L;
            spReg.D1_L = reg.D1_L;
            spReg.D5_L = reg.D5_L;
            spReg.D6_L = reg.D6_L;
            spReg.D7_L = reg.D7_L;
            spReg.a0 = reg.a0;
            spReg.a1 = reg.a1;
            spReg.a3 = reg.a3;

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x1) != 0) goto _opn_reset_end;

            reg.D7_L = 3;
            reg.D1_L = 0x10;
            reg.D0_L = 0x9f;
            _OPN_WRITE();

            reg.a1 = 0;//_opn_reset_table1;
            L1a:
            reg.D1_B = _opn_reset_table1[reg.a1++];//mm.ReadByte(reg.a1++);
            if (reg.D1_B - 0xff == 0) goto L1b;
            reg.D0_B = _opn_reset_table1[reg.a1++];//mm.ReadByte(reg.a1++);
            reg.D7_L = 0;
            _OPN_WRITE();
            reg.D7_W += 6;
            _OPN_WRITE();
            goto L1a;
            L1b:
            reg.a1 = 0;// _opn_reset_table2;
            L1c:
            reg.D1_B = _opn_reset_table2[reg.a1++];//mm.ReadByte(reg.a1++);
            if (reg.D1_B - 0xff == 0) goto _opn_reset_end;
            reg.D0_B = _opn_reset_table2[reg.a1++];//mm.ReadByte(reg.a1++);
            reg.D7_L = 0;
            _OPN_WRITE();
            reg.D7_W += 3;
            _OPN_WRITE();
            reg.D7_W += 3;
            _OPN_WRITE();
            reg.D7_W += 3;
            _OPN_WRITE();

            goto L1c;

            _opn_reset_end:
            reg.a1 = 0;// _opm_reset_table;
            L1d:
            reg.D1_B = _opm_reset_table[reg.a1++];// mm.ReadByte(reg.a1++);
            if (reg.D1_B - 0xff == 0) goto L1e;
            reg.D0_B = _opm_reset_table[reg.a1++];// mm.ReadByte(reg.a1++);
            _OPM_WRITE();
            goto L1d;
            L1e:
            reg.D1_L = 0x60;
            reg.D0_L = 0x7f;
            L1f:
            _OPM_WRITE();
            reg.D1_B += 1;
            if ((sbyte)reg.D1_B >= 0) goto L1f;

            reg.D1_L = 0xe0;
            reg.D0_L = 0xff;
            L1g:
            _OPM_WRITE();
            reg.D1_B += 1;
            if ((sbyte)reg.D1_B < 0) goto L1g;

            //	move.w	sr,-(sp)  ?
            //	ori.w	#$700,sr  ?
            reg.D0_B = mm.ReadByte(0x9da);
            reg.D0_B &= 0x40;
            mm.Write(0x9da, (byte)reg.D0_B);
            //	move.w	(sp)+,sr  ?
            reg.D1_L = 0x1b;
            _OPM_WRITE();

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) != 0)
            {
                _mpcm_init();
            }

            reg.D0_L = spReg.D0_L;
            reg.D1_L = spReg.D1_L;
            reg.D5_L = spReg.D5_L;
            reg.D6_L = spReg.D6_L;
            reg.D7_L = spReg.D7_L;
            reg.a0 = spReg.a0;
            reg.a1 = spReg.a1;
            reg.a3 = spReg.a3;
        }

        public byte[] _opn_reset_table1 = new byte[] {
            0x28,0x00,0x28,0x01,0x28,0x02
            ,0x28,0x04,0x28,0x05,0x28,0x06

            ,0x06,0x00
            ,0x07,0x38
            ,0x08,0x00,0x09,0x00,0x0A,0x00
            ,0x0B,0x00,0x0C,0x00,0x0D,0x00

            ,0x10,0xBF
            ,0x11,0x3F
            ,0x18,0xC0,0x19,0xC0,0x1A,0xC0
            ,0x1B,0xC0,0x1C,0xC0,0x1D,0xC0

            ,0x20,0x00
            ,0x22,0x00
            ,0x24,0x00
            ,0x25,0x00
            ,0x26,0x00
            ,0x27,0x30
            ,0x29,0x80

            ,0xFF,0x00
        };

        public byte[] _opn_reset_table2 = new byte[] {
            0x40,0x7F,0x41,0x7F,0x42,0x7F
            ,0x44,0x7F,0x45,0x7F,0x46,0x7F
            ,0x48,0x7F,0x49,0x7F,0x4A,0x7F
            ,0x4C,0x7F,0x4D,0x7F,0x4E,0x7F

            ,0x80,0xFF,0x81,0xFF,0x82,0xFF
            ,0x84,0xFF,0x85,0xFF,0x86,0xFF
            ,0x88,0xFF,0x89,0xFF,0x8A,0xFF
            ,0x8C,0xFF,0x8D,0xFF,0x8E,0xFF

            ,0x90,0x00,0x91,0x00,0x92,0x00
            ,0x94,0x00,0x95,0x00,0x96,0x00
            ,0x98,0x00,0x99,0x00,0x9A,0x00
            ,0x9C,0x00,0x9D,0x00,0x9E,0x00

            ,0xB4,0xC0,0xB5,0xC0,0xB6,0xC0

            ,0xFF,0x00
        };

        public byte[] _opm_reset_table = new byte[] {
            0x01,0x02,0x01,0x00
            ,0x08,0x00,0x08,0x01,0x08,0x02,0x08,0x03
            ,0x08,0x04,0x08,0x05,0x08,0x06,0x08,0x07

            ,0x0F,0x00
            ,0x10,0x00,0x11,0x00
            ,0x12,0x00,0x14,0x30

            ,0x18,0x00,0x19,0x00,0x19,0x80

            ,0x38,0x00,0x39,0x00,0x3A,0x00,0x3B,0x00
            ,0x3C,0x00,0x3D,0x00,0x3E,0x00,0x3F,0x00

            ,0xFF,0x00
        };

        //─────────────────────────────────────
        public uint _data_work_size = 384 * 1024;
        public uint _work_top = 0x01_0000;
        public uint _buffer_top = 0x03_0000;
        public uint _old_trap4_vec = 0;
        public uint _old_opn_vec = 0;
        public uint _old_opm_vec = 0;
        public byte _old_merc_vec = 0;

        //─────────────────────────────────────
        //	program start
        //
        public void start()
        {
            log.Write(M_title);

            //スーパーバイザ処理　不要

            //	ori.w	#$700,sr ?

            _sw_chk();
            if (_mndrv_check() == 0)
            {
                _mndrv_already();
                return;
            }
            if (_trap4_check() != 0)
            {
                _trap4_already();
                return;
            }
            if (_opm_check() != 0)
            {
                _opm_used();
                return;
            }
            if (_get_mem() != 0)
            {
                _memory_err();
                return;
            }
            _work_init();
            _make_table();
            _mercury_check();
            _mpcm_check();
            _zdd_check();
            _vec_set();
            _dev_reset();
            _print_information();

            //スーパーバイザ処理　不要

            //常駐処理　不要
        }

        //─────────────────────────────────────
        public void putdec()
        {
            log.Write(string.Format("{0:d}", reg.D0_L));
        }

        //─────────────────────────────────────
        public void _sw_chk()
        {
            reg.D7_L = 0;
            reg.a2 += 1;

            _sw_chk_loop:
            reg.D0_B = mm.ReadByte(reg.a2++);
            if (reg.D0_B == 0) goto sw_end;
            if (reg.D0_B - ' ' == 0) goto _sw_chk_loop;
            //if (reg.D0_B - ' ' == 0) goto _sw_chk_loop; //全角スペースのスキップは不要
            if (reg.D0_B - '-' == 0) goto sw_set;
            goto sw_help;

            sw_set:
            reg.D0_B = mm.ReadByte(reg.a2++);
            if (reg.D0_B == 0) goto sw_end;
            if (reg.D0_B - 'h' == 0) goto sw_help;
            if (reg.D0_B - 'r' == 0) goto sw_release;
            if (reg.D0_B - 'k' == 0) goto sw_keyoff;
            if (reg.D0_B - 'b' == 0) goto sw_bufsize;
            goto sw_help;
            sw_end:
            return;

            sw_keyoff:
            reg.D7_B |= 0x80;
            goto _sw_chk_loop;

            sw_bufsize:
            GETNUM();
            reg.D1_L = 1024 * reg.D1_W;
            reg.a6 = _data_work_size;
            mm.Write(reg.a6, reg.D1_L);
            if (reg.D1_L != 0) goto _sw_chk_loop;
            _numover();
            goto sw_end;

            sw_help:
            _help_exit();
            goto sw_end;

            sw_release:
            if (_mndrv_check() != 0) goto _mndrv_not_kept;
            reg.D0_L = 0;
            trap(4);
            if (reg.D0_L != 0) goto _release_false;

            //スーパーバイザ処理　不要

            log.Write(M_release);

            return;//本来はプログラム終了

            _mndrv_not_kept:
            _not_kept();
            goto sw_end;

            _release_false:
            _not_remove();
            goto sw_end;
        }

        //─────────────────────────────────────
        //	from option.s (MCDRV)
        //
        public void GETNUM()
        {
            reg.D1_L = 0;
            reg.D0_L = 0;
            getnum10:
            reg.D0_B = mm.ReadByte(reg.a2++);
            if (reg.D0_B == 0) goto getnum20;
            if (reg.D0_B - ':' == 0) goto getnum10;
            bool cf = reg.D0_B < 0x30;
            reg.D0_B -= 0x30;
            if (cf) goto getnum20;
            if (reg.D0_B >= 10) goto getnum20;
            reg.D1_L *= 10;
            reg.D1_L += (UInt32)(Int32)reg.D0_L;
            if (reg.D1_L < 65535) goto getnum10;
            _numover();
            return;
            getnum20:
            reg.a2--;
        }

        //─────────────────────────────────────
        public uint _get_mem()
        {
            // _SETBLOCK処理不要

            reg.a0 = _work_top;
            mm.Write(reg.a0, reg.a1);
            reg.a1 += dw._work_size;
            reg.a0 = _buffer_top;
            mm.Write(reg.a0, reg.a1);

            reg.a0 = _work_top;// mm.ReadUInt32(_work_top);
            reg.D1_W >>= 1;
            do
            {
                mm.Write(reg.a0, (UInt16)0); reg.a0 += 2;
            } while (reg.D1_W-- != 0);

            // make memory block
            reg.D0_L = 0;
            reg.a0 = reg.a1;
            mm.Write(reg.a0, reg.D0_L); reg.a0 += 4;
            mm.Write(reg.a0, reg.D0_L); reg.a0 += 4;
            reg.D1_L = _data_work_size;
            reg.D1_L -= 16;
            mm.Write(reg.a0, reg.D1_L); reg.a0 += 4;
            mm.Write(reg.a0, reg.D0_L); reg.a0 += 4;
            reg.D0_L = 0;
            return reg.D0_L;
        }

        //─────────────────────────────────────
        //─────────────────────────────────────
        // from mem.s (MCDRV)
        //============================================================
        //		メモリブロックの取得
        // in	d1.l	サイズ
        // out	d0.l	確保したメモリブロック+$10 のアドレス
        //============================================================
        uint bufferPtr = 0;
        public byte _MCMALLOC()
        {
            if (bufferPtr == 0)
            {
                bufferPtr = _buffer_top;
            }
            reg.D0_L = bufferPtr;
            if ((reg.D1_L & 1) != 0)//奇数サイズか？
            {
                reg.D1_L++;//そうなら +1
            }
            bufferPtr += (UInt32)(Int32)reg.D1_L;
            return 0;

            //reg spReg = new reg();
            //spReg.D1_L = reg.D1_L;
            //spReg.a0 = reg.a0;
            //spReg.a1 = reg.a1;
            //spReg.a6 = reg.a6;

            //reg.a0 = _buffer_top;
            //if ((reg.D1_L & 1) != 0)//奇数サイズか？
            //{
            //    reg.D1_L++;//そうなら +1
            //}
            //mcmalloc10:
            //if (mm.ReadByte(reg.a0 + 13) == 0)
            //{
            //    if (reg.D1_L - mm.ReadUInt32(reg.a0 + 8) <= 0) goto mcmalloc30;//サイズは足りるか？
            //}
            ////mcmalloc20:
            //reg.D0_L = mm.ReadUInt32(reg.a0 + 4);//前から空ブロックをたどる
            //if (reg.D0_L == 0) goto mcmalloc80;//メモリに空きがない
            //reg.a0 = reg.D0_L;
            //goto mcmalloc10;
            //mcmalloc30:
            //reg.a1 = reg.a0 + reg.D1_L + 16;//a1 = 次のメモリブロックのアドレス
            //reg.D0_L = (unchecked((UInt32)(-16)));//サイズ
            //reg.D0_L += mm.ReadUInt32(reg.a0 + 8);
            //reg.D0_L -= reg.D1_L;//管理エリアが作れるか？
            //if ((Int32)reg.D0_L < 0) goto mcmalloc40;//作れないならそのまま終わる
            //mm.Write(reg.a1 + 8, reg.D0_L);
            //mm.Write(reg.a0 + 8, reg.D1_L);
            //reg.D0_L = 0;
            //mm.Write(reg.a1 + 12, reg.D0_L);

            //reg.D0_L = mm.ReadUInt32(reg.a0 + 4);
            //mm.Write(reg.a0 + 4, reg.a1);
            //mm.Write(reg.a1, reg.a0);
            //mm.Write(reg.a1 + 4, reg.D0_L);//リンク
            //if (mm.ReadUInt32(reg.a1 + 4) == 0) goto mcmalloc40;
            //UInt32 v = reg.a0;
            //reg.a0 = reg.D0_L;
            //reg.D0_L = v;
            //mm.Write(reg.a0, reg.a1);
            //reg.a0 = reg.D0_L;
            //mcmalloc40:
            //mm.Write(reg.a0 + 12, 0);
            //mm.Write(reg.a0 + 13, 0xff);
            //reg.D0_L = 0x16;
            //reg.D0_L += reg.a0;
            //goto mcmalloc90;
            //mcmalloc80:
            //reg.D0_L = 0xffffffff;//-1
            //mcmalloc90:
            //reg.D1_L = spReg.D1_L;
            //reg.a0 = spReg.a0;
            //reg.a1 = spReg.a1;
            //reg.a6 = spReg.a6;

            //return (byte)((reg.D0_L == 0xffffffff) ? -1 : 0);
        }

        //============================================================
        //		メモリブロックの開放
        // in	d1.l	開放するメモリブロックのアドレス
        //============================================================

        public void _MCMFREE()
        {
            reg spReg = new reg();
            spReg.D1_L = reg.D1_L;
            spReg.a0 = reg.a0;
            spReg.a1 = reg.a1;
            spReg.a6 = reg.a6;

            reg.a6 = _buffer_top;
            reg.a0 = unchecked((UInt32)(-16));
            reg.a0 += (UInt32)(Int32)reg.D1_L;
            if (mm.ReadByte(reg.a0 + 12) != 0) goto mcmfree80;//ロック状態ならエラー
            reg.D1_L = 0;
            //mcmfree10:
            reg.D0_L = mm.ReadUInt32(reg.a0);
            if (reg.D0_L == 0) goto mcmfree20;
            reg.a1 = reg.D0_L;
            if (reg.a0 - mm.ReadUInt32(reg.a1 + 4) != 0) goto mcmfree80;//自分と前がちゃんとリンクしていないならエラー

            mcmfree20:
            reg.D0_L = mm.ReadUInt32(reg.a0 + 4);
            if (reg.D0_L == 0) goto mcmfree30;
            reg.a1 = reg.D0_L;
            if (reg.a0 - mm.ReadUInt32(reg.a1) != 0) goto mcmfree80;//自分と後がちゃんとリンクしていないならエラー
            mcmfree30:
            mm.Write(reg.a0 + 13, 0);//自分を空ブロックに変更
            reg.D0_L = mm.ReadUInt32(reg.a0);
            if (reg.D0_L == 0) goto mcmfree40;
            reg.a1 = reg.D0_L;
            if (mm.ReadByte(reg.a1 + 13) != 0) goto mcmfree40;// 前は空ブロックか？
            reg.D0_L = 16;
            reg.D0_L += mm.ReadUInt32(reg.a1 + 8);
            reg.D0_L += reg.a1;
            if (reg.a0 - reg.D0_L != 0) goto mcmfree40;//また連続のブロックであるか？
            reg.D0_L = 0x16;//空ならくっつける
            reg.D0_L += mm.ReadUInt32(reg.a0 + 8);
            mm.Write(reg.a1 + 8, (UInt32)(mm.ReadUInt32(reg.a1 + 8) + (Int32)reg.D0_L));//サイズを足す
            reg.a0 = mm.ReadUInt32(reg.a0 + 4);
            mm.Write(reg.a1 + 4, reg.a0);//リンク
            reg.D0_L = reg.a0;
            if (reg.D0_L == 0) goto mcmfree35;
            mm.Write(reg.a0, reg.a1);
            mcmfree35:
            reg.a0 = reg.a1;
            mcmfree40:
            reg.D0_L = mm.ReadUInt32(reg.a0 + 4);
            if (reg.D0_L == 0) goto mcmfree50;
            reg.a1 = reg.D0_L;
            if (mm.ReadByte(reg.a1 + 13) != 0) goto mcmfree50;//後ろは空ブロックか？
            reg.D0_L = 16;
            reg.D0_L += mm.ReadUInt32(reg.a0 + 8);
            reg.D0_L += reg.a0;
            if (reg.a1 - reg.D0_L != 0) goto mcmfree50;//また連続のブロックであるか？
            reg.D0_L = 16;//空ならくっつける
            reg.D0_L += mm.ReadUInt32(reg.a1 + 8);
            mm.Write(reg.a1 + 8, (UInt32)(mm.ReadUInt32(reg.a1 + 8) + (Int32)reg.D0_L));//サイズを足す
            reg.a1 = mm.ReadUInt32(reg.a1 + 4);
            mm.Write(reg.a0 + 4, reg.a1);//リンク
            reg.D0_L = reg.a1;
            if (reg.D0_L == 0) goto mcmfree45;
            mm.Write(reg.a1, reg.a0);
            mcmfree45:
            mcmfree50:
            reg.D0_L = 0;
            goto mcmfree90;
            mcmfree80:
            reg.D0_L = 0xffffffff;//-1
            mcmfree90:
            reg.D1_L = spReg.D1_L;
            reg.a0 = spReg.a0;
            reg.a1 = spReg.a1;
            reg.a6 = spReg.a6;
        }

        //─────────────────────────────────────
        //─────────────────────────────────────
        public void _reset_work()
        {
            //	move.w	sr,-(sp) 
            //	ori.w	#$700,sr

            reg.a6 = _work_top;// mm.ReadUInt32(_work_top);
            reg.a5 = reg.a6 + dw.OPMREGWORK;
            reg.D1_W = dw._trackworksize / 2 - 1;
            reg.D1_W += 1024;
            reg.D0_L = 0;
            do
            {
                mm.Write(reg.a5, (UInt16)reg.D0_W); reg.a5 += 2;
            } while (reg.D1_W-- != 0);
            _work_init_env_();
        }

        public void _work_init()
        {
            //	move.w	sr,-(sp)
            //	ori.w	#$700,sr

            reg.a6 = _work_top;// mm.ReadUInt32(_work_top);
            reg.a5 = reg.a6;
            reg.D1_W = dw._work_size / 2 - 1;
            reg.D0_L = 0;
            do
            {
                mm.Write(reg.a5, (UInt16)reg.D0_W); reg.a5 += 2;
            } while (reg.D1_W-- != 0);
            mm.Write(reg.a6 + dw.RANDOMESEED, 0x12345678);
            _work_init_env_();
        }

        public void _work_init_env_()
        {
            reg.a5 = reg.a6 + dw.SOFTENV_PATTERN;
            reg.D1_L = 7 - 1;
            //_work_init_env:
            do
            {
                reg.a4 = 0;// _psg_env_pattern;
                reg.D0_L = 16 - 1;
                do
                {
                    mm.Write(reg.a5, _psg_env_pattern[reg.a4]);// mm.ReadByte(reg.a4));
                    reg.a4++; reg.a5++;
                } while (reg.D0_W-- != 0);
            } while (reg.D1_W-- != 0);

            reg.D0_L = mnwork.TRACK - 1;
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.a4 = ab.dummyAddress;// _work_init_nop;
            do
            {
                //1:
                mm.Write(reg.a5 + w.mmljob_adrs, reg.a4);
                if (ab.hlw_mmljob_adrs.ContainsKey(reg.a5)) ab.hlw_mmljob_adrs.Remove(reg.a5);
                ab.hlw_mmljob_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.softenv_adrs, reg.a4);
                if (ab.hlw_softenv_adrs.ContainsKey(reg.a5)) ab.hlw_softenv_adrs.Remove(reg.a5);
                ab.hlw_softenv_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.lfojob_adrs, reg.a4);
                if (ab.hlw_lfojob_adrs.ContainsKey(reg.a5)) ab.hlw_lfojob_adrs.Remove(reg.a5);
                ab.hlw_lfojob_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.psgenv_adrs, reg.a4);
                //if (ab.hlw_psgenv_adrs.ContainsKey(reg.a5)) ab.hlw_psgenv_adrs.Remove(reg.a5);
                //ab.hlw_psgenv_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.qtjob, reg.a4);
                if (ab.hlw_qtjob.ContainsKey(reg.a5)) ab.hlw_qtjob.Remove(reg.a5);
                ab.hlw_qtjob.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.rrcut_adrs, reg.a4);
                if (ab.hlw_rrcut_adrs.ContainsKey(reg.a5)) ab.hlw_rrcut_adrs.Remove(reg.a5);
                ab.hlw_rrcut_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.echo_adrs, reg.a4);
                if (ab.hlw_echo_adrs.ContainsKey(reg.a5)) ab.hlw_echo_adrs.Remove(reg.a5);
                ab.hlw_echo_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.keyoff_adrs, reg.a4);
                if (ab.hlw_keyoff_adrs.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs.Remove(reg.a5);
                ab.hlw_keyoff_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.keyoff_adrs2, reg.a4);
                if (ab.hlw_keyoff_adrs2.ContainsKey(reg.a5)) ab.hlw_keyoff_adrs2.Remove(reg.a5);
                ab.hlw_keyoff_adrs2.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.subcmd_adrs, reg.a4);
                if (ab.hlw_subcmd_adrs.ContainsKey(reg.a5)) ab.hlw_subcmd_adrs.Remove(reg.a5);
                ab.hlw_subcmd_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.setnote_adrs, reg.a4);
                if (ab.hlw_setnote_adrs.ContainsKey(reg.a5)) ab.hlw_setnote_adrs.Remove(reg.a5);
                ab.hlw_setnote_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.inithlfo_adrs, reg.a4);
                if (ab.hlw_inithlfo_adrs.ContainsKey(reg.a5)) ab.hlw_inithlfo_adrs.Remove(reg.a5);
                ab.hlw_inithlfo_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.we_ycom_adrs, reg.a4);
                if (ab.hlw_we_ycom_adrs.ContainsKey(reg.a5)) ab.hlw_we_ycom_adrs.Remove(reg.a5);
                ab.hlw_we_ycom_adrs.Add(reg.a5, _work_init_nop);

                mm.Write(reg.a5 + w.we_tone_adrs, reg.a4);
                if (ab.hlw_we_tone_adrs.ContainsKey(reg.a5)) ab.hlw_we_tone_adrs.Remove(reg.a5);
                ab.hlw_we_tone_adrs.Add(reg.a5, _work_init_nop);

                reg.a5 = reg.a5 + w._track_work_size;
            } while (reg.D0_W-- != 0);

            mm.Write(reg.a6 + dw.TRKANA_RESTADR, reg.a4);
            if (ab.hlTRKANA_RESTADR.ContainsKey(reg.a6)) ab.hlTRKANA_RESTADR.Remove(reg.a6);
            ab.hlTRKANA_RESTADR.Add(reg.a6, _work_init_nop);
        }

        public void _work_init_nop()
        {
            //	rts
        }

        //─────────────────────────────────────
        public byte[] _psg_env_pattern = new byte[] {
            0x00,0x01,0xFF,0xFF,0x00,0x81,0x00,0x00,0x00,0x81,0x00,0x00,0xFF,0x81,0x00,0x00
        };

        //─────────────────────────────────────
        public void _vec_set()//多分使用しない
        {
            return;
            //_old_trap4_vec = mm.ReadUInt32(0x90);
            //mm.Write(0x90, ab.dummyAddress);// _trap4_entry);
            //_old_opm_vec = mm.ReadUInt32(0x10c);
            //mm.Write(0x10c, ab.dummyAddress);//_opm_entry);

            //if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) == 0)
            //{
            //    _old_opn_vec = mm.ReadUInt32(0x3fc);
            //    _old_merc_vec = mm.ReadByte(0xecc0b1);
            //    mm.Write(0x3fc, ab.dummyAddress);// _opn_entry);
            //    mm.Write(0xecc0b1, 0xff);
            //}
            ////_vec_set_no_opn:
            //mm.Write(0xe88009, (byte)(mm.ReadByte(0xe88009) | 8));
            //mm.Write(0xe88015, (byte)(mm.ReadByte(0xe88015) | 8));
        }

        //─────────────────────────────────────
        public void _vec_release()//多分使用しない
        {
            mm.Write(0xe88009, (byte)(mm.ReadByte(0xe88009) & 0xf7));
            mm.Write(0xe88015, (byte)(mm.ReadByte(0xe88015) & 0xf7));

            //	move.w	sr,-(sp)
            //	ori.w	#$700,sr

            //wait 多分不要
            //while (mm.ReadByte(0xe9a001) != 0 || (mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 1) != 0) ;

            mm.Write(0x10c, _old_opm_vec);
            mm.Write(0x90, _old_trap4_vec);

            //wait 多分不要
            //while ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) != 0) ;

            mm.Write(0xecc0b1, _old_merc_vec);
            mm.Write(0x3fc, _old_opn_vec);

            //	move.w	(sp)+,sr
        }

        //─────────────────────────────────────
        //	trap check
        //
        public int _trap4_check()
        {
            return 0;
            //return (mm.ReadByte(0x90) - 0x24) == 0 ? 0 : 1;
        }

        //─────────────────────────────────────
        //	driver check
        //
        public int _mndrv_check()
        {
            //チェック不要(そもそも常駐しない)
            return 1;
            //reg spReg = new reg();
            //spReg.a0 = reg.a0;
            //spReg.a1 = reg.a1;

            //reg.a1 = mm.ReadUInt32(0x90);
            //reg.a1 = mm.ReadUInt32(reg.a1 - 8);
            //reg.a0 = top + 4;
            //reg.D0_L = 8 - 1;

            //do
            //{
            //    if (mm.ReadByte(reg.a0) - mm.ReadByte(reg.a1) != 0)
            //    {
            //        reg.a0 = spReg.a0;
            //        reg.a1 = spReg.a1;
            //        return 1;
            //    }
            //    reg.D0_L--;
            //} while (reg.D0_L != 0);
            //reg.a0 = spReg.a0;
            //reg.a1 = spReg.a1;

            //return 0;
        }

        //─────────────────────────────────────
        //
        public void _print_information()
        {
            if ((sbyte)reg.D7_B < 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x80));
            }
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) == 0)
            {
                log.Write(M_merc);
            }
            reg.D1_L = 0;
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) != 0)
            {
                log.Write(M_MPCM);
                reg.D1_B = 0xff;
            }
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x08) == 0) goto L2;
            if (reg.D1_B == 0) goto L1;
            log.Write(",");
            L1:
            log.Write(M_zdd);
            reg.D1_B = 0xff;
            L2:
            if (reg.D1_B != 0)
            {
                log.Write(M_PCMOUT);
            }

            reg.D0_L = _data_work_size;
            reg.D0_L /= 1024;
            putdec();

            log.Write(M_buf);
        }

        //─────────────────────────────────────
        //	OPM割り込みちぇっく
        //
        public int _opm_check()
        {
            return 0;
            //return mm.ReadByte(0x10c) - 0x43;
        }

        //─────────────────────────────────────
        //	PCM ドライバ常駐チェック
        //
        public void _mpcm_check()
        {
            //reg.a1 = mm.ReadUInt32(0x84);
            //if (mm.ReadUInt32(reg.a1 - 8) - 0x4d50434d != 0) return;
            //if ((Int32)(mm.ReadUInt32(reg.a1 - 4) - 0x2f303430) < 0) return;

            reg.D0_W = 0x8000;
            //reg.a1 = M_keeptitle;
            trap(1);

            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x40));

            reg.D0_W = 0x8002;
            trap(1);

            _mpcm_init();
        }

        public void _mpcm_init()
        {
            reg.D0_W = 0x01ff;
            trap(1);

            reg.D0_W = 0x03ff;
            reg.D1_L = 4;
            trap(1);

            reg.D0_W = 0x05ff;
            reg.D1_L = 0x40;
            trap(1);

            reg.D0_W = 0x06ff;
            reg.D1_L = 0x03;
            trap(1);

            reg.D0_W = 0x8005;
            reg.D1_L = 0xffffffff;
            //reg.a1 = _mpcm_volume_table;
            trap(1);

        }

        //─────────────────────────────────────
        // zdd 常駐チェック
        //
        public void _zdd_check()
        {
            return;
            //reg.a1 = mm.ReadUInt32(0x8c);
            //if (mm.ReadUInt32(reg.a1 - 8) - 0x7a64642f != 0) return;

            //mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x08));

            //reg.D0_W = 0x8001;//zdd占有
            //reg.D1_L = 1;
            //trap(4);
        }

        //─────────────────────────────────────
        //	まーきゅりー存在チェック
        //	Xellent30 / YMF288 検出付き
        //
        public void _mercury_check()
        {
            reg spReg = new reg();
            spReg.a0 = reg.a0;
            spReg.a1 = reg.a1;
            spReg.a2 = reg.a2;

            if (_unit_check() != 0) goto _notmerc;
            if (_opn_check() != 0) goto _notmerc;
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x20));
            reg.a0 = spReg.a0;
            reg.a1 = spReg.a1;
            reg.a2 = spReg.a2;
            return;
            _notmerc:
            mm.Write(reg.a6 + dw.DRV_FLAG, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) | 0x21));
            reg.a0 = spReg.a0;
            reg.a1 = spReg.a1;
            reg.a2 = spReg.a2;
            return;
        }

        public int _opn_check()
        {
            return 0;
            //reg.a0 = 0xecc0c1;
            //mm.Write(reg.a0, (byte)0x20);
            //_wait();
            //mm.Write(reg.a0 + 2, 0);
            //_wait();
            //mm.Write(reg.a0, 0xff);
            //_wait();
            //return mm.ReadByte(reg.a0 + 2) - 1;
        }

        public void _wait()
        {
            mm.ReadByte(0xe9a001);
            mm.ReadByte(0xe9a001);
            mm.ReadByte(0xe9a001);
            mm.ReadByte(0xe9a001);
        }

        public int _unit_check()
        {
            reg.a0 = 0xecc080;
            if (_bus_check() != 0) goto _unit_check_notmerc;
            reg.a0 = 0xecc100;
            if (_bus_check() == 0) goto _unit_check_notmerc;
            return 0;// ccr ?

            _unit_check_notmerc:
            reg.D0_B = 0xff;
            return 1;
        }

        public int _bus_check()
        {
            // 0xecc080 -> mercがない 場合だけL1以降が実行
            // 0xecc100 -> mercがある 場合だけL1以降が実行
            if (reg.a0==0xecc080) return 0;
            return -1;

            //reg.D6_L = sp;
            //reg.a1 = L1;
            //reg.a2 = mm.ReadUInt32(8);
            //mm.Write(8, reg.a1);
            //reg.D0_W = mm.ReadUInt16(reg.a0);
            //mm.Write(8, reg.a2);
            //reg.D0_L = 0;
            //return;

            //L1:
            //sp = reg.D6_L;
            //mm.Write(8, reg.a2);
            //reg.D0_L = 0xffffffff;
            //return;
        }

        //─────────────────────────────────────
        //
        public void _make_table()
        {
            //
            // MAKE F-Number → OPM KC/KF CONVERT TABLE
            //
            //MAKE_FNUM_TABLE:
            reg.a0 = 0;// FNUM_BASE;
            reg.a1 = 0;// FNUM_KC_BASE;
            reg.a2 = reg.a6 + dw.FNUM_KC_TABLE;
            reg.D0_W = 0xc000;
            reg.D1_W = FNUM_BASE[reg.a0];// mm.ReadUInt16(reg.a0);

            MAKE_FNUMTBL2:
            mm.Write(reg.a2, (UInt16)reg.D0_W); reg.a2 += 2;  // !
            reg.D1_W -= 1;
            if (reg.D1_W != 0) goto MAKE_FNUMTBL2;

            reg.D5_L = 0;

            MAKE_FNUMTBL3:
            reg.D6_B = FNUM_KC_BASE[reg.a1 + reg.D5_W];//mm.ReadByte(reg.a1 + reg.D5_W);
            reg.D5_B += (UInt32)(sbyte)reg.D5_B;
            reg.D1_W = FNUM_BASE[reg.a0 + (reg.D5_W) / 2]; //mm.ReadUInt16(reg.a0 + reg.D5_W);
            reg.D3_W = reg.D1_W;
            reg.D2_W = FNUM_BASE[reg.a0 + (reg.D5_W + 2) / 2]; //mm.ReadUInt16(reg.a0 + reg.D5_W + 2);

            if (reg.D2_W >= 0x0800) goto MAKE_FNUMTBL5;

            reg.D5_B >>= 1;
            reg.D2_W -= (UInt32)(Int16)reg.D1_W;
            reg.D1_L = 0;

            MAKE_FNUMTBL4:
            mm.Write(reg.a2, (byte)reg.D6_B); reg.a2++;
            reg.D0_L = 0;
            reg.D0_W = reg.D1_W;
            reg.D0_L <<= 8;
            reg.D0_L = (UInt32)((UInt16)((Int32)reg.D0_L / (Int16)reg.D2_W) | (UInt32)(((UInt16)((Int32)reg.D0_L % (Int16)reg.D2_W)) << 16));
            mm.Write(reg.a2, (byte)reg.D0_B); reg.a2++;
            reg.D1_W += 1;

            if (reg.D1_W - reg.D2_W != 0) goto MAKE_FNUMTBL4;

            reg.D5_B += 1;
            goto MAKE_FNUMTBL3;

            MAKE_FNUMTBL5:
            mm.Write(reg.a2++, (byte)reg.D6_B);
            mm.Write(reg.a2++, 0);
            reg.D0_W = 0xc000;
            reg.D1_W = reg.D3_W;
            reg.D1_W += 1;

            MAKE_FNUMTBL6:
            mm.Write(reg.a2, (UInt16)reg.D0_W);reg.a2 += 2;
            reg.D1_W += 1;
            if (reg.D1_W - 0x0800 != 0) goto MAKE_FNUMTBL6;

            //
            // MAKE Frequency → OPM KC/KF CONVERT TABLE
            //
            //MAKE_FREQ_TABLE:
            reg.a0 = 0;// FREQ_BASE;
            reg.a1 = 0;// FREQ_KC_BASE;
            reg.a2 = reg.a6 + dw.FREQ_KC_TABLE;
            reg.D0_W = 0x7efc;
            reg.D1_W = FREQ_BASE[reg.a0];//mm.ReadUInt16(reg.a0);

            MAKE_FREQTBL2:
            mm.Write(reg.a2, (UInt16)reg.D0_W); reg.a2 += 2;  // !
            reg.D1_W -= 1;
            if (reg.D1_W != 0) goto MAKE_FREQTBL2;

            reg.D5_L = 0;

            MAKE_FREQTBL3:
            reg.D6_B = FREQ_KC_BASE[reg.a1 + reg.D5_W];//mm.ReadByte(reg.a1 + reg.D5_W);
            reg.D5_W += (UInt32)(Int16)reg.D5_W;
            reg.D1_W = FREQ_BASE[reg.a0 + reg.D5_W / 2];//mm.ReadUInt16(reg.a0 + reg.D5_W);
            reg.D3_W = reg.D1_W;
            reg.D2_W = FREQ_BASE[reg.a0 + (reg.D5_W + 2) / 2];//mm.ReadUInt16(reg.a0 + reg.D5_W + 2);

            if (reg.D2_W >= 0x1000) goto MAKE_FREQTBL6;

            reg.D5_W >>= 1;
            reg.D2_W -= (UInt32)(Int16)reg.D1_W;
            reg.D1_L = 0;
            mm.Write(reg.a2++, (byte)reg.D6_B);
            mm.Write(reg.a2++, (byte)reg.D1_B);
            reg.D6_B = FREQ_KC_BASE[reg.a1 + reg.D5_W + 1];//mm.ReadByte(reg.a1 + reg.D5_W + 1);
            reg.D1_W = reg.D2_W;
            if (reg.D1_W != 0) goto MAKE_FREQTBL5;

            reg.D5_B += 1;
            goto MAKE_FREQTBL3;

            MAKE_FREQTBL4:
            mm.Write(reg.a2, (byte)reg.D6_B); reg.a2++;
            reg.D0_L = 0;
            reg.D0_W = reg.D1_W;
            reg.D0_L <<= 8;
            reg.D0_L = (UInt32)((UInt16)((Int32)reg.D0_L / (Int16)reg.D2_W) | (UInt32)(((UInt16)((Int32)reg.D0_L % (Int16)reg.D2_W)) << 16));
            mm.Write(reg.a2, (byte)reg.D0_B); reg.a2++;
            MAKE_FREQTBL5:
            reg.D1_W -= 1;
            if (reg.D1_W != 0) goto MAKE_FREQTBL4;

            reg.D5_B += 1;
            goto MAKE_FREQTBL3;

            MAKE_FREQTBL6:
            mm.Write(reg.a2++, (byte)reg.D6_B);
            mm.Write(reg.a2++, 0);
            reg.D0_L = 0;
            reg.D1_W = reg.D3_W;
            reg.D1_W += 1;
            MAKE_FREQTBL7:
            mm.Write(reg.a2, (UInt16)reg.D0_W); reg.a2 += 2;
            reg.D1_W += 1;

            if (reg.D1_W - 0x1000 != 0) goto MAKE_FREQTBL7;

        }

        //─────────────────────────────────────
        //	error exit
        //
        public void _not_remove()
        {
            log.Write(M_notremove);
        }
        public void _not_kept()
        {
            log.Write(M_notkept);
        }
        public void _help_exit()
        {
            log.Write(M_help);
        }
        public void _mndrv_already()
        {
            log.Write(M_already);
        }
        public void _trap4_already()
        {
            log.Write(M_trap4err);
        }
        public void _opm_used()
        {
            log.Write(M_opmerr);
        }
        public void _numover()
        {
            log.Write(M_numover);
        }
        public void _memory_err()
        {
            log.Write(M_memory_msg);
        }

        //─────────────────────────────────────
        //	data section
        //
        public UInt16[] _mpcm_volume_table = new ushort[] {
               0, 17, 18, 19, 20, 21, 22, 23
            , 24, 25, 26, 27, 28, 29, 30, 31
            , 32, 33, 34, 35, 36, 37, 38, 39
            , 40, 41, 42, 43, 44, 45, 46, 47
            , 48, 50, 52, 54, 56, 58, 60, 62
            , 64, 66, 68, 70, 72, 74, 76, 78
            , 80, 82, 84, 86, 88, 90, 92, 94
            , 96,100,104,108,112,116,120,124
            ,128,132,136,140,144,148,152,156
            ,160,164,168,172,176,180,184,188
            ,192,200,208,216,224,232,240,248
            ,256,264,272,280,288,296,304,312
            ,320,328,336,344,352,360,368,376
            ,384,400,416,432,448,464,480,496
            ,512,528,544,560,576,592,608,624
            ,640,656,672,688,704,720,736,752
        };

        //
        // OPN → OPM TUNE CONVERT TABLE (DEFAULT)
        //
        public UInt16[] FNUM_BASE = new UInt16[] {
            0x00A3,0x00AD,0x00B7,0x00C2
            ,0x00CD,0x00DA,0x00E7,0x00F4
            ,0x0103,0x0112,0x0123,0x0134
            ,0x0146,0x015A,0x016F,0x0184
            ,0x019B,0x01B4,0x01CE,0x01E9
            ,0x0207,0x0225,0x0246,0x0269
            ,0x028D,0x02B4,0x02DE,0x0309
            ,0x0337,0x0368,0x039C,0x03D3
            ,0x040E,0x044B,0x048D,0x04D2
            ,0x051A,0x0568,0x05BC,0x0612
            ,0x066E,0x06D0,0x0738,0x07A6
            ,0x081C,0x0896,0x091A,0x09A4
        };

        public byte[] FNUM_KC_BASE = new byte[] {
            0xDD,0xDE,0xE0,0xE1,0xE2,0xE4,0xE5,0xE6
            ,0xE8,0xE9,0xEA,0xEC,0xED,0xEE,0xF0,0xF1
            ,0xF2,0xF4,0xF5,0xF6,0xF8,0xF9,0xFA,0xFC
            ,0xFD,0xFE,0x00,0x01,0x02,0x04,0x05,0x06
            ,0x08,0x09,0x0A,0x0C,0x0D,0x0E,0x10,0x11
            ,0x12,0x14,0x15,0x16,0x18,0x19,0x1A,0x1C
        };

        public UInt16[] FREQ_BASE = new UInt16[] {
            0x000E,0x000F,0x0010,0x0011
            ,0x0012,0x0013,0x0015,0x0016
            ,0x0017,0x0019,0x001A,0x001C
            ,0x001D,0x001F,0x0021,0x0023
            ,0x0025,0x0027,0x002A,0x002C
            ,0x002F,0x0032,0x0035,0x0038
            ,0x003B,0x003F,0x0043,0x0047
            ,0x004B,0x004F,0x0054,0x0059
            ,0x005E,0x0064,0x006A,0x0070
            ,0x0077,0x007E,0x0086,0x008E
            ,0x0096,0x009F,0x00A8,0x00B2
            ,0x00BD,0x00C8,0x00D4,0x00E1
            ,0x00EE,0x00FD,0x010C,0x011C
            ,0x012C,0x013E,0x0151,0x0165
            ,0x017B,0x0191,0x01A9,0x01C2
            ,0x01DC,0x01FA,0x0218,0x0238
            ,0x0258,0x027C,0x02A2,0x02CA
            ,0x02F6,0x0322,0x0352,0x0384
            ,0x03B8,0x03F4,0x0430,0x0470
            ,0x04B0,0x04F8,0x0544,0x0594
            ,0x05EC,0x0644,0x06A4,0x0708
            ,0x0770,0x07E8,0x0860,0x08E0
            ,0x0960,0x09F0,0x0A88,0x0B28
            ,0x0BD8,0x0C88,0x0D48,0x0E10
            ,0x0EE0,0x0FD0,0x10C0,0x11C0
            ,0x12C0,0x13E0,0x1510,0x1650
            ,0x17B0,0x1910,0x1A90,0x1C20
        };

        public byte[] FREQ_KC_BASE = new byte[] {
            0x8C,0x8A,0x89,0x88,0x86,0x85,0x84,0x82
            ,0x81,0x80,0x7E,0x7D,0x7C,0x7A,0x79,0x78
            ,0x76,0x75,0x74,0x72,0x71,0x70,0x6E,0x6D
            ,0x6C,0x6A,0x69,0x68,0x66,0x65,0x64,0x62
            ,0x61,0x60,0x5E,0x5D,0x5C,0x5A,0x59,0x58
            ,0x56,0x55,0x54,0x52,0x51,0x50,0x4E,0x4D
            ,0x4C,0x4A,0x49,0x48,0x46,0x45,0x44,0x42
            ,0x41,0x40,0x3E,0x3D,0x3C,0x3A,0x39,0x38
            ,0x36,0x35,0x34,0x32,0x31,0x30,0x2E,0x2D
            ,0x2C,0x2A,0x29,0x28,0x26,0x25,0x24,0x22
            ,0x21,0x20,0x1E,0x1D,0x1C,0x1A,0x19,0x18
            ,0x16,0x15,0x14,0x12,0x11,0x10,0x0E,0x0D
            ,0x0C,0x0A,0x09,0x08,0x06,0x05,0x04,0x02
            ,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        };

        public string M_title =
            "X68k mndrv mania driver version "
            + DRVVER
            + " (c)1997-2000 BEL.\r\n";
        public string M_merc = "まーきゅりーゆにっとから出力が可能です\r\n";
        public string M_MPCM = "MPCM";
        public string M_zdd = "zdd";
        public string M_PCMOUT = "から多重/音程,音量変換出力が可能です\r\n";
        public string M_buf = "KBのバッファを確保しました\r\n";
        public string M_release = "mndrvを解除しました\r\n";
        public string M_already = "すでに常駐しています\r\n";
        public string M_notkept = "mndrvは常駐していません\r\n";
        public string M_notremove = "占有されているので解除出来ません\r\n";
        public string M_trap4err = "trap #4がすでに使われています\r\n";
        public string M_opmerr = "OPM割り込みがすでに使われています\r\n";
        public string M_memory_msg = "メモリが足りません\r\n";
        public string M_numover = "数値が範囲外です\r\n";
        public string M_help = "usage: mndrv [option]\r\n"
            + "	-b[num]	バッファサイズ指定\r\n"
            + "	-k	キーコントロール無効\r\n"
            + "	-r	常駐解除\r\n";

    }
}
