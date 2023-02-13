using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class interrupt
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public mndrv mndrv;
        public devpsg devpsg;
        public devopn devopn;
        public devopm devopm;
        public devmpcm devmpcm;
        public ab ab;
        public FMTimer timerOPM;
        public FMTimer timerOPN;

        //
        //	part of INTERRUPT MAIN
        //

        //─────────────────────────────────────
        public byte[] _opn_irq = new byte[] {
            0x30,0x1F,0x2F,0x3F
        };
        public UInt16[] _opn_intmask = new ushort[] {
            0x200,0x200,0x200,0x300,0x400,0x500,0x600,0x700
        };

        //─────────────────────────────────────
        //	OPN timer
        //
        //public void _opn_entry_short_exit()
        //{
        //    //reg.a6=spA6;
        //}

        public void _opn_entry()
        {
            reg.SR |= 0x700;

            uint spA6 = reg.a6;

            reg.a6 = mndrv._work_top;// mm.ReadUInt32(mndrv._work_top);//
            byte f = (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 1);
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) | 1));
            if (f != 0)
            {
                //	_opn_entry_short_exit
                reg.a6 = spA6;
                return;
            }

            reg spReg = new reg();
            spReg.D0_L = reg.D0_L;
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
            spReg.a4 = reg.a4;
            spReg.a5 = reg.a5;

            reg.a0 = 0xecc0c1;
            reg.D0_L = 3;
            do
            {
                //reg.D3_B = mm.ReadByte(reg.a0);
                reg.D3_B = (byte)timerOPN.ReadStatus();
            } while ((sbyte)reg.D3_B < 0);
            
            reg.D3_W &= reg.D0_W;

            reg.D0_L = 7;
            reg.D0_L &= 0;// reg.Arg[0];//4*15(sp) -> 割り込みレベルの調整(下げている)を行っている(再現不要箇所)
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D4_W = reg.SR_W;
            reg.D4_W &= 0xf8ff;
            reg.D4_W |= _opn_intmask[reg.D0_W / 2];

            _opn_recall:

            reg.D7_L = 0;
            reg.D1_L = 0x27;
            reg.D0_B = _opn_irq[reg.D3_W];
            reg.D0_B |= mm.ReadByte(reg.a6 + dw.CH3MODEM);
            mndrv._OPN_WRITE();
            reg.SR_W = reg.D4_W;

            UInt32 sp = reg.D4_W;
            reg.D3_W += (UInt32)(Int16)reg.D3_W;
            switch (reg.D3_W)
            {
                case 0:
                    goto _opn_entry_exit;
                case 2:
                    _timer_a_job();
                    break;
                case 4:
                    _timer_b_job();
                    break;
                case 6:
                    _timer_ab_job();
                    break;
            }
            reg.D4_W = sp;

            reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO3);
            if (mm.ReadByte(reg.a6 + dw.TEMPO2) - reg.D0_B != 0)
            {
                mm.Write(reg.a6 + dw.TEMPO2, (byte)reg.D0_B);
                reg.D7_L = 0;
                reg.D1_L = 0x26;
                mndrv._OPN_WRITE();
            }

            reg.SR |= 0x700;
            reg.a0 = 0xecc0c1;
            reg.D0_L = 3;
            do
            {
                //reg.D3_B = mm.ReadByte(reg.a0);
                reg.D3_B = (byte)timerOPN.ReadStatus();
            } while ((sbyte)reg.D3_B < 0);
            reg.D3_W &= reg.D0_W;
            if (reg.D3_W != 0) goto _opn_recall;

            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0xfe));
            reg.SR = reg.D4_W;

            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.INTEXECNUM);
            if (reg.D7_W != 0)
            {
                reg.a0 = reg.a6 + dw.INTEXECBUF;
                mm.Write(reg.a6 + dw.INTEXECNUM, (UInt16)0);
                reg.D7_W -= 1;
                do
                {
                    reg.a1 = mm.ReadUInt32(reg.a0); reg.a0 += 4;
                    reg spReg2 = new reg();
                    spReg2.D7_L = reg.D7_L;
                    spReg2.a0 = reg.a0;
                    spReg2.a6 = reg.a6;
                    ab.hlINTEXECBUF[reg.a1]();
                    reg.D7_L = spReg2.D7_L;
                    reg.a0 = spReg2.a0;
                    reg.a6 = spReg2.a6;
                } while (reg.D7_W-- != 0);
            }

            _opn_entry_exit:

            reg.D0_L = spReg.D0_L;
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
            reg.a4 = spReg.a4;
            reg.a5 = spReg.a5;
        }

        //─────────────────────────────────────
        public UInt16[] _intmask = new UInt16[] {
            0x200,0x200,0x200,0x300,0x400,0x500,0x600,0x700
        };
        public byte[] _dev_irq = new byte[] {
            0x30,0x1F,0x2F,0x3F
        };

        //─────────────────────────────────────
        //	OPM timer
        //
        //public void _opm_entry_short_exit()
        //{
        //    reg.a6 = spA6;
        //}

        public void _opm_entry()
        {
            reg.SR |= 0x700;
            uint spA6 = reg.a6;
            reg.a6 = mndrv._work_top;// mm.ReadUInt32(mndrv._work_top);
            byte f = (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 1);
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) | 1));
            if (f != 0)
            {
                //	_opm_entry_short_exit
                reg.a6 = spA6;
                return;
            }

            reg spReg = new reg();
            spReg.D0_L = reg.D0_L;
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
            spReg.a4 = reg.a4;
            spReg.a5 = reg.a5;

            reg.a0 = 0xe90003;
            reg.D0_L = 3;
            do
            {
                //reg.D3_B = mm.ReadByte(reg.a0);
                reg.D3_B = (byte)timerOPM.ReadStatus();
            } while ((sbyte)reg.D3_B < 0);
            reg.D3_W &= reg.D0_W;

            reg.D0_L = 7;
            reg.D0_L &= 0;// reg.Arg[0];//4*15(sp) -> 割り込みレベルの調整(下げている)を行っている(再現不要箇所)
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D4_W = (UInt32)(Int16)reg.SR_W;
            reg.D4_W &= 0xf8ff;
            reg.D4_W |= _intmask[reg.D0_W / 2];

            _opm_recall:

            reg.D1_L = 0x14;
            reg.D0_B = _dev_irq[reg.D3_W];
            mndrv._OPM_WRITE();
            reg.SR_W = reg.D4_W;

            UInt32 sp = reg.D4_W;
            reg.D3_W += reg.D3_W;
            switch (reg.D3_W)
            {
                case 0:
                    goto _opm_entry_exit;
                case 2:
                    _timer_a_job();
                    break;
                case 4:
                    _timer_b_job();
                    break;
                case 6:
                    _timer_ab_job();
                    break;
            }
            reg.D4_W = sp;

            reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO3);
            if (mm.ReadByte(reg.a6 + dw.TEMPO2) - reg.D0_B != 0)
            {
                mm.Write(reg.a6 + dw.TEMPO2, (byte)reg.D0_B);
                reg.D1_L = 0x12;
                mndrv._OPM_WRITE();
            }

            reg.SR |= 0x700;
            reg.a0 = 0xe90003;
            reg.D0_L = 3;
            do
            {
                //reg.D3_B = mm.ReadByte(reg.a0);
                reg.D3_B = (byte)timerOPM.ReadStatus();
            } while ((sbyte)reg.D3_B < 0);
            reg.D3_W &= reg.D0_W;
            if (reg.D3_W != 0) goto _opm_recall;

            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0xfe));
            reg.SR = reg.D4_W;

            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.INTEXECNUM);
            if (reg.D7_W != 0)
            {
                reg.a0 = reg.a6 + dw.INTEXECBUF;
                mm.Write(reg.a6 + dw.INTEXECNUM, (UInt16)0);
                reg.D7_W -= 1;
                do
                {
                    reg.a1 = mm.ReadUInt32(reg.a0); reg.a0 += 4;
                    reg spReg2 = new reg();
                    spReg2.D7_L = reg.D7_L;
                    spReg2.a0 = reg.a0;
                    spReg2.a6 = reg.a6;
                    ab.hlINTEXECBUF[reg.a1]();
                    reg.D7_L = spReg2.D7_L;
                    reg.a0 = spReg2.a0;
                    reg.a6 = spReg2.a6;
                } while (reg.D7_W-- != 0);
            }

            _opm_entry_exit:

            reg.D0_L = spReg.D0_L;
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
            reg.a4 = spReg.a4;
            reg.a5 = spReg.a5;
        }

        //─────────────────────────────────────
        //─────────────────────────────────────
        //	TIMER-A/B
        //
        public void _timer_ab_job()
        {
            _timer_b_job();
            _timer_a_job();
        }

        //─────────────────────────────────────
        //	TIMER-A JOB
        //
        public void _timer_a_job()
        {
            _fade_entry();

            reg.D1_B = mm.ReadByte(reg.a6 + dw.DRV_STATUS);
            if ((sbyte)reg.D1_B < 0)
            {
                reg.D1_B += (UInt32)(sbyte)reg.D1_B;
                if ((sbyte)reg.D1_B >= 0)
                {
                    if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG3) & 0x40) != 0)
                    {
                        _ch_ana_tma_env();
                    }
                    if ((sbyte)mm.ReadByte(reg.a6 + dw.DRV_FLAG3) < 0)
                    {
                        _ch_ana_tma_lfo();
                    }
                }
            }
        }

        //─────────────────────────────────────
        //	TIMER-B JOB
        //
        public void _timer_b_job()
        {
            reg.D1_B = mm.ReadByte(reg.a6 + dw.DRV_STATUS);
            if ((sbyte)reg.D1_B >= 0) goto _timer_b_job_pause;
            reg.D1_B += (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D1_B < 0) goto _timer_b_job_pause;
            do
            {
                _ch_ana();
            } while ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x08) != 0);
            _timer_b_job_pause:
            reg.D3_L = 0;
            reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO);

            if ((sbyte)mm.ReadByte(reg.a6 + dw.DRV_FLAG) < 0)
            {
                _key_ctrl_disable();
                return;
            }

            reg.D2_L = 0xf;
            reg.D2_B &= mm.ReadByte(0x80e);
            if (reg.D2_B == 0)
            {
                _key_ctrl_disable();
                return;
            }
            reg.D2_W += (UInt32)(Int16)reg.D2_W;
            switch (reg.D2_W)
            {
                case 0:
                case 2:// SHIFT
                case 4:// CTRL
                case 6:// SHIFT + CTRL
                case 14:// SHIFT + CTRL + OPT.1
                case 22:// SHIFT + CTRL + OPT.2
                case 24:
                case 26:
                case 28:
                case 30:
                    _key_nop();
                    break;
                case 8://OPT.1
                    _key_OPT1();
                    break;
                case 10://SHIFT + OPT.1
                    _key_SOPT1();
                    break;
                case 12://CTRL + OPT.1
                    _key_COPT1();
                    break;
                case 16://OPT.2
                    _key_OPT2();
                    break;
                case 18://SHIFT + OPT.2
                    _key_SOPT2();
                    break;
                case 20://CTRL + OPT.2
                    _key_COPT2();
                    break;
            }
            mm.Write(reg.a6 + dw.TEMPO3, (byte)reg.D0_B);

            reg.D1_B = mm.ReadByte(reg.a6 + dw.DRV_STATUS);
            if ((sbyte)reg.D1_B >= 0) return;
            reg.D1_B += (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D1_B < 0) return;

            if (reg.D3_W != 0)
            {
                _ch_ana();
            }
            reg.D0_B = mm.ReadByte(reg.a6 + dw.RHY_DAT);
            if (reg.D0_B != 0)
            {
                mm.Write(reg.a6 + dw.RHY_DAT, 0);
                reg.D7_L = 0;
                reg.D1_L = 0x10;
                mndrv._OPN_WRITE();
            }
            reg.D0_B = mm.ReadByte(reg.a6 + dw.RHY_DAT2);
            if (reg.D0_B != 0)
            {
                mm.Write(reg.a6 + dw.RHY_DAT2, 0);
                reg.D7_L = 6;
                reg.D1_L = 0x10;
                mndrv._OPN_WRITE();
            }
            //_timer_b_job_exit:
        }

        //─────────────────────────────────────
        public void _key_ctrl_disable()
        {
            mm.Write(reg.a6 + dw.TEMPO3, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.SP_KEY, 0);
            if (mm.ReadByte(reg.a6 + dw.MUTE) == 0)
            {
                if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x40) == 0)
                {
                    _ff_mute_return();
                }
            }
            reg.D0_B = mm.ReadByte(reg.a6 + dw.RHY_DAT);
            if (reg.D0_B != 0)
            {
                mm.Write(reg.a6 + dw.RHY_DAT, 0);
                reg.D7_L = 0;
                reg.D1_L = 0x10;
                mndrv._OPN_WRITE();
            }
            reg.D0_B = mm.ReadByte(reg.a6 + dw.RHY_DAT2);
            if (reg.D0_B != 0)
            {
                mm.Write(reg.a6 + dw.RHY_DAT2, 0);
                reg.D7_L = 6;
                reg.D1_L = 0x10;
                mndrv._OPN_WRITE();
            }
        }

        //─────────────────────────────────────
        public void _key_nop()
        {
            mm.Write(reg.a6 + dw.SP_KEY, 0);
            if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x40) == 0)
            {
                if (mm.ReadByte(reg.a6 + dw.MUTE) == 0)
                {
                    _ff_mute_return();
                }
            }
        }

        public void _key_OPT1()
        {
            reg.D2_B = mm.ReadByte(0x80b);
            uint f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f != 0)
            {
                _key_OPT1_XF4();
                return;
            }
            f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f != 0)
            {
                _key_OPT1_XF5();
                return;
            }
            if ((sbyte)mm.ReadByte(0x80a) < 0)
            {
                _key_OPT1_XF3();
                return;
            }
            _key_nop();
        }

        public void _key_OPT2()
        {
            if ((mm.ReadByte(0x80b) & 0x2) != 0)
            {
                _key_OPT2_XF5();
                return;
            }
            if ((sbyte)(mm.ReadByte(0x80b) & 0x2) < 0)
            {
                _key_OPT2_XF3();
                return;
            }
            _key_nop();
        }

        public void _key_OPT1_XF3()
        {
            if (mm.ReadByte(reg.a6 + dw.SP_KEY) == 0)
            {
                int f = mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x10;
                mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) | 0x10));
                if (f == 0)
                {
                    mm.Write(reg.a6 + dw.FADEFLAG, 1);
                    mm.Write(reg.a6 + dw.FADESPEED, 7);
                    mm.Write(reg.a6 + dw.FADESPEED_WORK, 7);
                    mm.Write(reg.a6 + dw.FADECOUNT, 3);
                    mm.Write(reg.a6 + dw.SP_KEY, 0xff);
                    if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) == 0)
                    {
                        reg spReg = new reg();
                        spReg.D0_L = reg.D0_L;
                        spReg.D1_L = reg.D1_L;
                        spReg.D7_L = reg.D7_L;
                        reg.D7_L = 3;
                        reg.D1_L = 0x10;
                        reg.D0_L = 0x1c;
                        mndrv._OPN_WRITE();
                        reg.D0_L = spReg.D0_L;
                        reg.D1_L = spReg.D1_L;
                        reg.D7_L = spReg.D7_L;
                    }
                }
            }
        }

        public void _key_OPT2_XF3()
        {
            if (mm.ReadByte(reg.a6 + dw.SP_KEY) == 0)
            {
                mm.Write(reg.a6 + dw.FADEFLAG, 0xff);
                mm.Write(reg.a6 + dw.FADESPEED, 5);
                mm.Write(reg.a6 + dw.FADESPEED_WORK, 5);
                mm.Write(reg.a6 + dw.FADECOUNT, 3);
                mm.Write(reg.a6 + dw.SP_KEY, 0xff);
            }
        }

        public void _key_OPT1_XF4()
        {
            if (mm.ReadByte(reg.a6 + dw.SP_KEY) == 0)
            {
                mm.Write(reg.a6 + dw.SP_KEY, 0xff);
                _get_track_mask();
                mndrv._t_play_music();
                _set_track_mask();
                reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO);
                reg.D3_L = 0;
            }
        }

        public void _key_OPT1_XF5()
        {
            if (mm.ReadByte(reg.a6 + dw.SP_KEY) == 0)
            {
                mm.Write(reg.a6 + dw.SP_KEY, 0xff);
                mndrv._t_play_music();
                reg.D0_B = mm.ReadByte(reg.a6 + dw.TEMPO);
                reg.D3_L = 0;
            }
        }

        public void _key_OPT2_XF5()
        {
            if (mm.ReadByte(reg.a6 + dw.SP_KEY) == 0)
            {
                mndrv._t_pause();
                reg.D3_L = 0;
                mm.Write(reg.a6 + dw.SP_KEY, 0xff);
            }
        }

        public void _key_COPT1()
        {
            reg.D0_L = 0;
        }

        public void _key_SOPT1()
        {
            reg.D0_B >>= 1;
        }

        public void _key_COPT2()
        {
            reg.D3_B = 0xff;
            _key_SOPT2();
        }

        public void _key_SOPT2()
        {
            reg.D0_L = 0xf0;
            _ff_mute();
        }

        public void _get_track_mask()
        {
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.a0 = reg.a6 + dw.MASKDATA;
            reg.D1_L = 0;
            reg.D1_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);

            do
            {
                reg.D0_B = mm.ReadByte(reg.a5 + w.flag2);
                reg.D0_B &= 0x80;
                mm.Write(reg.a0++, (byte)reg.D0_B);
                reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
                reg.D1_W--;
            } while (reg.D1_W != 0);
        }

        public void _set_track_mask()
        {
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            reg.a0 = reg.a6 + dw.MASKDATA;

            reg.D1_L = 0;
            reg.D1_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            do
            {
                reg.D0_B = mm.ReadByte(reg.a0++);
                mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | reg.D0_B));
                reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
                reg.D1_W--;
            } while (reg.D1_W != 0);
        }

        public void _ff_mute()
        {
            if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x10) != 0) return;

            mm.Write(reg.a6 + dw.MUTE, 0);

            reg spReg = new reg();
            spReg.D0_L = reg.D0_L;
            spReg.D3_L = reg.D3_L;

            mm.Write(reg.a6 + dw.MASTER_VOL_FM, 14);
            mm.Write(reg.a6 + dw.MASTER_VOL_PCM, 40);
            mm.Write(reg.a6 + dw.MASTER_VOL_RHY, 25);
            mm.Write(reg.a6 + dw.MASTER_VOL_PSG, 5);
            _ch_fade();

            reg.D0_L = spReg.D0_L;
            reg.D3_L = spReg.D3_L;
        }

        public void _ff_mute_return()
        {
            mm.Write(reg.a6 + dw.MUTE, 0xff);
            reg spReg = new reg();
            spReg.D0_L = reg.D0_L;
            spReg.D3_L = reg.D3_L;
            reg.D0_L = 0;
            mm.Write(reg.a6 + dw.MASTER_VOL_FM, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.MASTER_VOL_PCM, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.MASTER_VOL_RHY, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.MASTER_VOL_PSG, (byte)reg.D0_B);
            _ch_fade();
            reg.D0_L = spReg.D0_L;
            reg.D3_L = spReg.D3_L;
        }

        //─────────────────────────────────────
        //	FADEIN/OUT
        //
        public void _fade_entry()
        {
            reg.D1_B = mm.ReadByte(reg.a6 + dw.FADEFLAG);
            if (reg.D1_B == 0) return;

            if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x40) != 0) return;
            mm.Write(reg.a6 + dw.FADESPEED_WORK, (byte)(mm.ReadByte(reg.a6 + dw.FADESPEED_WORK) - 1));
            if (mm.ReadByte(reg.a6 + dw.FADESPEED_WORK) != 0) return;

            mm.Write(reg.a6 + dw.FADESPEED_WORK, mm.ReadByte(reg.a6 + dw.FADESPEED));
            //	tst.b	d1
            //	bpl	_fade_out
            if ((sbyte)reg.D1_B >= 0)
            {
                _fade_out();
                return;
            }

            _ch_fadein_calc();
            _fade_exec();
        }

        public void _fade_out()
        {
            _ch_fadeout_calc();
            _fade_exec();
        }

        public void _fade_exec()
        {
            uint sp = reg.D7_L;
            _ch_fade();
            reg.D7_L = sp;

            if ((mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x20) == 0) return;

            mm.Write(reg.a6 + dw.FADEFLAG, 0);
            mndrv._d_stop_music();
            reg.D0_L = 5;
            mndrv.SUBEVENT();
            mm.Write(reg.a6 + dw.DRV_STATUS, 0x20);
        }

        //─────────────────────────────────────
        // Mon Aug 14 17:34 JST 2000 (saori)
        // 各 MASTER_VOLはここでは 127以上にはならない為
        // エラーチェックを省いた。
        //
        public void _ch_fadeout_calc()
        {
            mm.Write(reg.a6 + dw.MASTER_VOL_FM, (byte)(mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM) + 1));
            mm.Write(reg.a6 + dw.MASTER_VOL_PCM, (byte)(mm.ReadByte(reg.a6 + dw.MASTER_VOL_PCM) + 1));
            mm.Write(reg.a6 + dw.MASTER_VOL_RHY, (byte)(mm.ReadByte(reg.a6 + dw.MASTER_VOL_RHY) + 1));

            mm.Write(reg.a6 + dw.FADECOUNT, (byte)(mm.ReadByte(reg.a6 + dw.FADECOUNT) - 1));
            if (mm.ReadByte(reg.a6 + dw.FADECOUNT) != 0) return;
            mm.Write(reg.a6 + dw.FADECOUNT, 3);

            mm.Write(reg.a6 + dw.MASTER_VOL_PCM, (byte)(mm.ReadByte(reg.a6 + dw.MASTER_VOL_PCM) + 2));

            reg.D0_B = mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            reg.D0_B += 1;
            if (reg.D0_B >= 25)
            {
                mm.Write(reg.a6 + dw.DRV_STATUS, 0x20);
            }
            mm.Write(reg.a6 + dw.MASTER_VOL_PSG, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        public void _ch_fadein_calc()
        {
            reg.D0_B = mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM);
            reg.D0_B -= 1;
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a6 + dw.MASTER_VOL_FM, (byte)reg.D0_B);

            reg.D0_B = mm.ReadByte(reg.a6 + dw.MASTER_VOL_PCM);
            reg.D0_B -= 2;
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a6 + dw.MASTER_VOL_PCM, (byte)reg.D0_B);

            reg.D0_B = mm.ReadByte(reg.a6 + dw.MASTER_VOL_RHY);
            reg.D0_B -= 1;
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a6 + dw.MASTER_VOL_RHY, (byte)reg.D0_B);

            mm.Write(reg.a6 + dw.FADECOUNT , (byte)(mm.ReadByte(reg.a6 + dw.FADECOUNT) - 1));
            if (mm.ReadByte(reg.a6 + dw.FADECOUNT) != 0) return;

            mm.Write(reg.a6 + dw.FADECOUNT, 3);
            reg.D0_B = mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            reg.D0_B -= 1;
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a6 + dw.MASTER_VOL_PSG, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        public void _ch_fade()
        {
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            _ch_fade_loop:
            reg.D0_B = mm.ReadByte(reg.a5 + w.ch);
            if ((sbyte)reg.D0_B < 0) goto _ch_fade_loop_check;

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 1) != 0) goto _ch_fade_next;

            if (reg.D0_B >= 0x40) goto _ch_fade_loop_rhythm;
            if (reg.D0_B >= 0x20) goto _ch_fade_loop_psg;

            reg.D4_B = mm.ReadByte(reg.a6 + dw.MUTE);
            reg.D4_B |= mm.ReadByte(reg.a5 + w.flag);
            if ((reg.D4_L & 0x20) != 0)
            {
                if (mm.ReadByte(reg.a5 + w.revexec) == 0)
                {
                    reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
                    goto L2;
                }
            }
            reg.D4_L = 0x7f;
            L2:
            devopn._FM_F2_softenv();

            _ch_fade_next:
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            reg.D7_W--;
            if (reg.D7_W != 0) goto _ch_fade_loop;
            return;

            _ch_fade_loop_psg:
            if ((sbyte)mm.ReadByte(reg.a5 + w.e_sw) >= 0)
            {

                reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
                reg.D0_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
                if ((sbyte)reg.D0_B < 0)
                {
                    reg.D0_L = 0;
                }
                devpsg._psg_lfo();
            }
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            if (reg.D7_W-- != 0) goto _ch_fade_loop;
            return;

            _ch_fade_loop_rhythm:
            reg.D0_B = mm.ReadByte(reg.a6 + dw.RHY_TV);
            reg.D0_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_RHY);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            reg.D1_L = 0x11;
            mndrv._OPN_WRITE2();
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            if (reg.D7_W-- != 0) goto _ch_fade_loop;
            return;

            _ch_fade_loop_check:
            if (reg.D0_B >= 0xa0) goto _ch_fade_loop_pcm;

            reg.D4_B = mm.ReadByte(reg.a6 + dw.MUTE);
            reg.D4_B |= mm.ReadByte(reg.a5 + w.flag);
            if ((reg.D4_L & 0x20) != 0)
            {
                if (mm.ReadByte(reg.a5 + w.revexec) == 0)
                {
                    reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
                    goto L2b;
                }
            }
            reg.D4_L = 0x7f;
            L2b:
            devopm._OPM_F2_softenv();
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            if (reg.D7_W-- != 0) goto _ch_fade_loop;
            return;

            _ch_fade_loop_pcm:
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x40) == 0) goto L9;

            reg.D4_B = mm.ReadByte(reg.a6 + dw.MUTE);
            reg.D4_B |= mm.ReadByte(reg.a5 + w.flag);
            if ((reg.D4_L & 0x20) != 0)
            {
                if (mm.ReadByte(reg.a5 + w.revexec) == 0)
                {
                    reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
                    goto L2c;
                }
            }
            reg.D4_L = 0;
            L2c:
            devmpcm._MPCM_F2_softenv();
            L9:
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            if (reg.D7_W-- != 0) goto _ch_fade_loop;
        }

        //─────────────────────────────────────
        public void _ch_ana()
        {
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a5 = reg.a6 + dw.TRACKWORKADR;

            _ch_ana_loop:
            reg.a0 = mm.ReadUInt32(reg.a5 + w.mmljob_adrs);
            ab.hlw_mmljob_adrs[reg.a5]();

            if ((sbyte)mm.ReadByte(reg.a6 + dw.DRV_FLAG3) >= 0)
            {
                reg.a0 = mm.ReadUInt32(reg.a5 + w.lfojob_adrs);
                ab.hlw_lfojob_adrs[reg.a5]();
            }
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG3) & 0x40) == 0)
            {
                if ((sbyte)mm.ReadByte(reg.a5 + w.revexec) >= 0)
                {
                    reg.D0_B = mm.ReadByte(reg.a5 + w.e_sw);
                    if ((sbyte)reg.D0_B < 0)
                    {
                        reg.a0 = mm.ReadUInt32(reg.a5 + w.softenv_adrs);
                        ab.hlw_softenv_adrs[reg.a5]();
                    }
                }
            }
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;

            reg.D7_W--;
            if (reg.D7_W != 0) goto _ch_ana_loop;
        }

        //─────────────────────────────────────
        //	TIMER-A lfo
        //
        public void _ch_ana_tma_lfo()
        {
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a5 = reg.a6 + dw.TRACKWORKADR;

            _ch_ana_tma_lfo_loop:
            reg.a0 = mm.ReadUInt32(reg.a5 + w.lfojob_adrs);
            ab.hlw_lfojob_adrs[reg.a5]();
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            reg.D7_W--;
            if (reg.D7_W != 0) goto _ch_ana_tma_lfo_loop;
        }

        //─────────────────────────────────────
        //	TIMER-A software envelope
        //
        public void _ch_ana_tma_env()
        {
            reg.D7_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a5 = reg.a6 + dw.TRACKWORKADR;
            _ch_ana_tma_env_loop:
            if ((sbyte)mm.ReadByte(reg.a5 + w.revexec) >= 0)
            {
                if ((sbyte)mm.ReadByte(reg.a5 + w.e_sw) < 0)
                {
                    reg.a0 = mm.ReadUInt32(reg.a5 + w.softenv_adrs);
                    ab.hlw_softenv_adrs[reg.a5]();
                }
            }
            reg.a5 = reg.a5 + w._track_work_size;// dw._trackworksize;
            reg.D7_W--;
            if (reg.D7_W != 0) goto _ch_ana_tma_env_loop;
        }

    }
}
