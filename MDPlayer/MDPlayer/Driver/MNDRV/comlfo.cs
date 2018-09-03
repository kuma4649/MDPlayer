using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class comlfo
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public devopm devopm;

        private UInt32 sp1 = 0;

        //
        //	part of LFO
        //

        //─────────────────────────────────────
        public void _init_lfo2()
        {
            sp1 = reg.D1_L;
            if (mm.ReadByte(reg.a5 + w.ch) < 0x80)
            {
                _init_lfo2_exit();
                return;
            }

            if (mm.ReadByte(reg.a5 + w.ch) >= 0x88)
            {
                _init_lfo2_exit();
                return;
            }

            mm.Write(reg.a5 + w.addkeycode, (UInt16)0);
            reg.D2_W = mm.ReadUInt16(reg.a5 + w.keycode3);
            devopm._set_kckf();
            _init_lfo2_exit();
        }

        public void _init_lfo2_exit()
        {
            reg.D1_L = sp1;
        }


        public void _init_lfo()
        {
            if ((mm.ReadByte(reg.a5 + w.effect) & 0x20) == 0) goto L9;
            reg.a4 = reg.a5 + w.ww_pattern1;
            if (mm.ReadByte(reg.a4 + w_ww.sync) == 0) goto L9;

            reg.D0_B = mm.ReadByte(reg.a4 + w_ww.speed);
            mm.Write(reg.a4 + w_ww.rate_work, mm.ReadByte(reg.a4 + w_ww.rate));
            reg.D0_B += mm.ReadByte(reg.a4 + w_ww.delay);
            mm.Write(reg.a4 + w_ww.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_ww.depth_work, mm.ReadByte(reg.a4 + w_ww.depth));
            mm.Write(reg.a4 + w_ww.work, (byte)0x00);
            L9:
            reg.D5_L = 0xe;
            reg.D5_B &= mm.ReadByte(reg.a5 + w.lfo);
            if (reg.D5_B != 0)
            {
                switch (reg.D5_B)
                {
                    case 2:
                        _init_lfo_1();
                        break;
                    case 4:
                        _init_lfo_2();
                        break;
                    case 6:
                        _init_lfo_3();
                        break;
                    case 8:
                        _init_lfo_4();
                        break;
                    case 10:
                        _init_lfo_5();
                        break;
                    case 12:
                        _init_lfo_6();
                        break;
                    case 14:
                        _init_lfo_7();
                        break;
                }
            }
            reg.a4 = reg.a5 + w.p_pattern4;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xfc));

        }

        //─────────────────────────────────────
        public void _init_lfo_1()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            _init_lfo_common();
        }

        public void _init_lfo_2()
        {
            reg.a4 = reg.a5 + w.p_pattern2;
            _init_lfo_common();
        }

        public void _init_lfo_3()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            _init_lfo_common();
            reg.a4 = reg.a5 + w.p_pattern2;
            _init_lfo_common();
        }

        public void _init_lfo_4()
        {
            reg.a4 = reg.a5 + w.p_pattern3;
            _init_lfo_common();
        }

        public void _init_lfo_5()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            _init_lfo_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            _init_lfo_common();
        }

        public void _init_lfo_6()
        {
            reg.a4 = reg.a5 + w.p_pattern2;
            _init_lfo_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            _init_lfo_common();
        }

        public void _init_lfo_7()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            _init_lfo_common();
            reg.a4 = reg.a5 + w.p_pattern2;
            _init_lfo_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            _init_lfo_common();
        }

        //─────────────────────────────────────
        public void _init_lfo_common()
        {
            if ((mm.ReadByte(reg.a4 + w_l.flag) & 0x40) != 0)
            {
                return;
            }

            mm.Write(reg.a4 + w_l.bendwork,(UInt16)0);
            mm.Write(reg.a4 + w_l.flag, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.flag) & 0xdfff));

            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG2);
            if ((sbyte)reg.D0_B < 0)
            {
                _init_lfo_truetie();
                return;
            }
            if((reg.D0_L & 0x40) != 0)
            {
                _init_lfo_fmptie();
                return;
            }

            reg.D0_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x01) == 0)
            {
                mm.Write(reg.a4 + w_l.henka_work, mm.ReadUInt16(reg.a4 + w_l.henka));
                reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
            }

            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
            reg.D0_B = mm.ReadByte(reg.a4 + w_l.count);
            reg.D0_B >>= 1;
            mm.Write(reg.a4 + w_l.count_work, (byte)reg.D0_B);
        }

        public void _init_lfo_truetie()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.flag);
            if ((reg.D0_B & 0x1) != 0)
            {
                reg.D0_B = mm.ReadByte(reg.a4 + w_l.count);
                reg.D0_B >>= 1;
                mm.Write(reg.a4 + w_l.count_work, (byte)reg.D0_B);
                return;
            }
            if ((reg.D0_L & 0x40) != 0) return;
            _init_lfo_fmptie();
        }

        public void _init_lfo_fmptie()
        {
            reg.D0_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
            mm.Write(reg.a4 + w_l.henka_work, mm.ReadUInt16(reg.a4 + w_l.henka));
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
            reg.D0_B = mm.ReadByte(reg.a4 + w_l.count);
            reg.D0_B >>= 1;
            mm.Write(reg.a4 + w_l.count_work, (byte)reg.D0_B);
        }

        public void _init_lfo_common_a()
        {
            if ((mm.ReadByte(reg.a4 + w_l.flag) & 0x40) != 0)
            {
                return;
            }

            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a4 + w_l.flag, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.flag) & 0xdfff));

            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG2);
            if ((sbyte)reg.D0_B < 0)
            {
                _init_lfo_truetie_a();
                return;
            }
            if ((reg.D0_L & 0x40) != 0)
            {
                _init_lfo_fmptie_a();
                return;
            }

            reg.D0_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x01) == 0)
            {
                mm.Write(reg.a4 + w_l.henka_work, mm.ReadUInt16(reg.a4 + w_l.henka));
                reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
            }

            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
        }

        public void _init_lfo_truetie_a()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.flag);
            if ((reg.D0_B & 0x1) != 0)
            {
                mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
                return;
            }

            if ((reg.D0_L & 0x40) != 0) return;
            _init_lfo_fmptie_a();
        }

        public void _init_lfo_fmptie_a()
        {
            reg.D0_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
            mm.Write(reg.a4 + w_l.henka_work, mm.ReadUInt16(reg.a4 + w_l.henka));
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
        }

        //─────────────────────────────────────
        //	LFO 鋸波
        //
        public void _com_lfo_saw()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_lfo_saw_end;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork)+reg.D1_W));

            mm.Write(reg.a4 + w_l.count_work, (byte)(mm.ReadByte(reg.a4 + w_l.count_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count_work) != 0) goto _com_lfo_saw_end;

            mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - reg.D1_W));

            _com_lfo_saw_end:
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //	LFO portament
        //
        public void _com_lfo_portament()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_lfo_portament_end;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D1_W));

            _com_lfo_portament_end:
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //	LFO triangle
        //
        public void _com_lfo_triangle()
        {
            //if(reg.a4 == 0x14cf0)            log.Write(string.Format("adr:{0:x} bendwork:{1}",reg.a4, mm.ReadUInt16(reg.a4 + w_l.bendwork)));

            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_lfo_triangle_end;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D1_W));

            mm.Write(reg.a4 + w_l.count_work, (byte)(mm.ReadByte(reg.a4 + w_l.count_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count_work) != 0) goto _com_lfo_triangle_end;
            mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(-(Int16)mm.ReadUInt16(reg.a4 + w_l.henka_work)));

            _com_lfo_triangle_end:
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //	LFO 1shot
        //
        public void _com_lfo_oneshot()
        {
            reg.D2_W = mm.ReadUInt16(reg.a4 + w_l.flag);
            if (reg.D2_W != 0) goto _com_lfo_oneshot_end;

            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_lfo_oneshot_end;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D1_W));

            mm.Write(reg.a4 + w_l.count_work, (byte)(mm.ReadByte(reg.a4 + w_l.count_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count_work) != 0) goto _com_lfo_oneshot_end;
            mm.Write(reg.a4 + w_l.flag, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.flag) | 0x2000));

            _com_lfo_oneshot_end:
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //	LFO square
        //
        public void _com_lfo_square()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_lfo_square_end;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            mm.Write(reg.a4 + w_l.bendwork, mm.ReadUInt16(reg.a4 + w_l.henka_work));

            mm.Write(reg.a4 + w_l.count_work, (byte)(mm.ReadByte(reg.a4 + w_l.count_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count_work) != 0) goto _com_lfo_square_end;
            mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(-(Int16)mm.ReadUInt16(reg.a4 + w_l.henka_work)));

            _com_lfo_square_end:
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
        }

        //─────────────────────────────────────
        //	LFO randome
        //
        public void _com_lfo_randome()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_lfo_randome_end;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            GETRND();
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a4 + w_l.count);
            reg.D1_L = reg.D1_W * mm.ReadUInt16(reg.a4 + w_l.henka_work);
            reg.D0_L = (UInt32)((UInt16)reg.D0_W * (UInt16)reg.D1_W);
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);

            mm.Write(reg.a4 + w_l.count_work, (byte)(mm.ReadByte(reg.a4 + w_l.count_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count_work) != 0) goto _com_lfo_randome_end;
            mm.Write(reg.a4 + w_l.count_work, mm.ReadByte(reg.a4 + w_l.count));
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(-(Int16)mm.ReadUInt16(reg.a4 + w_l.henka_work)));

            _com_lfo_randome_end:
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
        }

        //========================================
        //	乱数を得る
        // out
        //	d0.l	乱数
        public void GETRND()
        {
            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.RANDOMESEED);
            reg.D1_L = reg.D0_L;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_L = (reg.D1_L >> 16) + (reg.D1_L << 16);
            reg.D1_W = 0;
            reg.D0_L ^= reg.D1_L;
            reg.D1_L = reg.D0_L;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W = 0;
            reg.D1_L = (reg.D1_L >> 16) + (reg.D1_L << 16);
            reg.D1_L += (UInt32)(Int32)reg.D1_L;
            reg.D0_L ^= reg.D1_L;
            reg.D0_L = ~reg.D0_L;
            mm.Write(reg.a6 + dw.RANDOMESEED, reg.D0_L);
        }

        //─────────────────────────────────────
        //	ソフトウェアエンベロープ
        //
        public void _soft_env()
        {
            //_soft1:
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            if (reg.D0_B == 0) goto _soft1_end;
            //_soft1_ar:
            reg.D0_B -= 1;
            if (reg.D0_B != 0) goto _soft1_dr;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            bool cf = reg.cryADD((byte)reg.D0_B , mm.ReadByte(reg.a5 + w.e_ar));
            reg.D0_B += mm.ReadByte(reg.a5 + w.e_ar);
            if (!cf) goto _soft1_ok;
            reg.D0_B = 0xff;
            mm.Write(reg.a5 + w.e_p, (byte)2);
            goto _soft1_ok;

            _soft1_dr:
            reg.D0_B -= 1;
            if (reg.D0_B != 0) goto _soft1_sr;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_dr);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_dr);
            if (cf) goto _soft1_dr2;
            if (reg.D0_B >= mm.ReadByte(reg.a5 + w.e_sl)) goto _soft1_ok;

            _soft1_dr2:
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sl);
            mm.Write(reg.a5 + w.e_p, (byte)3);
            goto _soft1_ok;

            _soft1_sr:
            reg.D0_B -= 1;
            if (reg.D0_B != 0) goto _soft1_rr;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_sr);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_sr);
            if (reg.D0_B == 0) goto _soft1_end;
            if (!cf) goto _soft1_ok;
            goto _soft1_end;

            _soft1_rr:
            reg.D0_B -= 1;
            if (reg.D0_B != 0) goto _soft1_ko;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_rr);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_rr);
            if (reg.D0_B == 0) goto _soft1_end;
            if (!cf) goto _soft1_ok;
            if (cf) goto _soft1_end;

            _soft1_ko:
            mm.Write(reg.a5 + w.e_p, (byte)1);
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sv);

            _soft1_ok:
            mm.Write(reg.a5 + w.e_sub, (byte)reg.D0_B);
            goto _soft1_volume_set;

            _soft1_end:
            reg.D4_L = 0;
            mm.Write(reg.a5 + w.e_sub, (byte)reg.D4_B);
            mm.Write(reg.a5 + w.e_p, (byte)reg.D4_B);
            reg.a2 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D4_W);
            return;

            _soft1_volume_set:
            reg.D2_L = 0;
            reg.D2_B = reg.D0_B;
            if (reg.D2_B == 0) goto _soft1_vol_ok;
            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.volume);
            if (reg.D1_B == 0) goto _soft1_vol_ok;
            do
            {
                reg.D0_W += (UInt32)(Int16)reg.D2_W;
            } while ((reg.D1_W--) != 0);
            reg.D0_B = reg.D0_W >> 8;
            _soft1_vol_ok:
            reg.D0_W &= 0xff;
            reg.a2 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D0_W);
        }
    }
}
