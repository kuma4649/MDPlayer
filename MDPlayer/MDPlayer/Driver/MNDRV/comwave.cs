using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class comwave
    {
        public reg reg;
        public ab ab;
        public MXDRV.xMemory mm;
        public comlfo comlfo;

        private UInt32 cf = 0;
        private byte val = 0;

        //
        //	part of wavememory
        //

        //─────────────────────────────────────
        //
        public void _wave_init_kon()
        {
            _weffect_init_kon();

            reg.D0_L = 0x7e;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.lfo);
            if (reg.D0_B == 0)
            {
                return;
            }

            cf = reg.D0_B & 2;
            reg.D0_B >>= 2;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wp_pattern1;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kon_common(val);
                }
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wp_pattern2;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kon_common(val);
                }
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wp_pattern3;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kon_common(val);
                }
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wv_pattern1;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kon_common_a(val);
                }
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wv_pattern2;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kon_common_a(val);
                }
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf == 0)
            {
                return;
            }

            reg.a3 = reg.a5 + w.wv_pattern3;
            val = mm.ReadByte(reg.a3 + w_w.use_flag);
            if (val != 0)
            {
                _wave_init_kon_common_a(val);
                return;
            }
        }

        public void _wave_init_kon_common_a(byte val)
        {
            if ((sbyte)val >= 0)
            {
                _wave_init_compatible_a();
                return;
            }

            mm.Write(reg.a3 + w_w.exec_flag, (byte)0);
            mm.Write(reg.a3 + w_w.loop_flag, (byte)0);

            mm.Write(reg.a3 + w_w.adrs_work, mm.ReadUInt32(reg.a3 + w_w.start));
            mm.Write(reg.a3 + w_w.start_adrs_work, mm.ReadUInt32(reg.a3 + w_w.loop_start));
            mm.Write(reg.a3 + w_w.end_adrs_work, mm.ReadUInt32(reg.a3 + w_w.loop_end));
            mm.Write(reg.a3 + w_w.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_w.loop_count));
            if (mm.ReadUInt32(reg.a3 + w_w.loop_count) == 0)
            {
                return;
            }
            mm.Write(reg.a3 + w_w.loop_flag, 0xff);
        }

        public void _wave_init_compatible_a()
        {
            if ((mm.ReadByte(reg.a6 + w.flag3) & 0x40) == 0)
            {
                if ((mm.ReadByte(reg.a6 + w.flag) & 0x40) != 0) return;
            }
            mm.Write(reg.a3 + w_w.ko_start, mm.ReadUInt16(reg.a3 + w_w.start));
            mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.loop_start));
            mm.Write(reg.a3 + w_w.ko_loop_end, mm.ReadUInt16(reg.a3 + w_w.loop_end));

        }

        public void _wave_init_kon_common(byte val)
        {
            if ((sbyte)val >= 0)
            {
                _wave_init_compatible();
                return;
            }

            mm.Write(reg.a3 + w_w.exec_flag, (byte)0);
            mm.Write(reg.a3 + w_w.loop_flag, (byte)0);

            mm.Write(reg.a3 + w_w.adrs_work, mm.ReadUInt32(reg.a3 + w_w.start));
            mm.Write(reg.a3 + w_w.start_adrs_work, mm.ReadUInt32(reg.a3 + w_w.loop_start));
            mm.Write(reg.a3 + w_w.end_adrs_work, mm.ReadUInt32(reg.a3 + w_w.loop_end));
            mm.Write(reg.a3 + w_w.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_w.loop_count));
            if (mm.ReadUInt32(reg.a3 + w_w.loop_count) == 0)
            {
                return;
            }
            mm.Write(reg.a3 + w_w.loop_flag, (byte)0xff);
        }

        public void _wave_init_compatible()
        {
            if ((mm.ReadByte(reg.a6 + w.flag3) & 0x40) == 0)
            {
                if ((mm.ReadByte(reg.a6 + w.flag) & 0x40) != 0) return;
            }
            mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.loop_start));
            mm.Write(reg.a3 + w_w.ko_loop_end, mm.ReadUInt32(reg.a3 + w_w.loop_end));
            mm.Write(reg.a3 + w_w.ko_loop_count, mm.ReadUInt32(reg.a3 + w_w.loop_count));

        }

        //─────────────────────────────────────
        //
        public void _wave_init_kof()
        {
            _weffect_init_kof();

            reg.D0_L = 0x7e;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.lfo);
            if (reg.D0_B == 0) return;

            cf = reg.D0_B & 0x2;
            reg.D0_B >>= 2;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wp_pattern1;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kof_common(val);
                }
            }

            cf = reg.D0_B & 0x1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wp_pattern2;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kof_common(val);
                }
            }

            cf = reg.D0_B & 0x1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wp_pattern3;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kof_common(val);
                }
            }

            cf = reg.D0_B & 0x1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wv_pattern1;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kof_common(val);
                }
            }

            cf = reg.D0_B & 0x1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.wv_pattern2;
                val = mm.ReadByte(reg.a3 + w_w.use_flag);
                if (val != 0)
                {
                    _wave_init_kof_common(val);
                }
            }

            cf = reg.D0_B & 0x1;
            reg.D0_B >>= 1;
            if (cf == 0)
            {
                return;
            }

            reg.a3 = reg.a5 + w.wv_pattern3;
            val = mm.ReadByte(reg.a3 + w_w.use_flag);
            if (val == 0) return;// goto L9;

            _wave_init_kof_common(val);
        }

        public void _wave_init_kof_common(byte val)
        {
            if ((sbyte)val >= 0)
            {
                return;
            }

            if (mm.ReadByte(reg.a3 + w_w.ko_flag) == 0)
            {
                return;
            }

            mm.Write(reg.a3 + w_w.exec_flag, (byte)0);
            mm.Write(reg.a3 + w_w.loop_flag, (byte)0);

            mm.Write(reg.a3 + w_w.adrs_work, mm.ReadUInt32(reg.a3 + w_w.ko_start));
            mm.Write(reg.a3 + w_w.start_adrs_work, mm.ReadUInt32(reg.a3 + w_w.ko_loop_start));
            mm.Write(reg.a3 + w_w.end_adrs_work, mm.ReadUInt32(reg.a3 + w_w.ko_loop_end));
            mm.Write(reg.a3 + w_w.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_w.ko_loop_count));
            if (mm.ReadUInt32(reg.a3 + w_w.ko_loop_count) != 0)
            {
                mm.Write(reg.a3 + w_w.loop_flag, (byte)0xff);
            }
        }


        //─────────────────────────────────────
        //
        public void _weffect_init_kon()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.weffect);
            if ((sbyte)reg.D0_B >= 0) return;

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern1;
                _weffect_init_kon_common();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern2;
                _weffect_init_kon_common();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern3;
                _weffect_init_kon_common();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern4;
                _weffect_init_kon_common();
            }
        }

        public void _weffect_init_kon_common()
        {
            reg.D1_B = mm.ReadByte(reg.a3 + w_we.mode);
            if ((sbyte)reg.D1_B < 0) return;

            cf = reg.D1_B & 1;
            reg.D1_B >>= 1;
            if (cf != 0)
            {
                _effect_exec();
                return;
            }

            mm.Write(reg.a3 + w_we.exec_flag, (byte)0);
            mm.Write(reg.a3 + w_we.loop_flag, (byte)0);

            mm.Write(reg.a3 + w_we.adrs_work, mm.ReadUInt32(reg.a3 + w_we.start));
            mm.Write(reg.a3 + w_we.start_adrs_work, mm.ReadUInt32(reg.a3 + w_we.loop_start));
            mm.Write(reg.a3 + w_we.end_adrs_work, mm.ReadUInt32(reg.a3 + w_we.loop_end));
            mm.Write(reg.a3 + w_we.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_we.loop_count));
            if (mm.ReadUInt32(reg.a3 + w_we.loop_count) != 0)
            {
                mm.Write(reg.a3 + w_we.loop_flag, (byte)0xff);
            }
        }

        //─────────────────────────────────────
        //
        public void _weffect_init_kof()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.weffect);
            if ((sbyte)reg.D0_B >= 0) return;

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern1;
                _weffect_init_kof_common();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern2;
                _weffect_init_kof_common();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern3;
                _weffect_init_kof_common();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf == 0)
            {
                return;
            }

            reg.a3 = reg.a5 + w.we_pattern4;
            _weffect_init_kof_common();
        }

        public void _weffect_init_kof_common()
        {
            reg.D1_B = mm.ReadByte(reg.a3 + w_we.mode);
            if ((sbyte)reg.D1_B < 0) return;

            cf = reg.D1_B & 1;
            reg.D1_B >>= 1;
            if (cf != 0)
            {
                return;
            }

            mm.Write(reg.a3 + w_we.exec_flag, (byte)0);
            mm.Write(reg.a3 + w_we.loop_flag, (byte)0);

            mm.Write(reg.a3 + w_we.adrs_work, mm.ReadUInt32(reg.a3 + w_we.ko_start));
            mm.Write(reg.a3 + w_we.start_adrs_work, mm.ReadUInt32(reg.a3 + w_we.ko_loop_start));
            mm.Write(reg.a3 + w_we.end_adrs_work, mm.ReadUInt32(reg.a3 + w_we.ko_loop_end));
            mm.Write(reg.a3 + w_we.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_we.ko_loop_count));
            if (mm.ReadUInt32(reg.a3 + w_we.ko_loop_count) != 0)
            {
                mm.Write(reg.a3 + w_we.loop_flag, (byte)0xff);
            }
        }

        //─────────────────────────────────────
        //─────────────────────────────────────
        //	wavememory effect
        //
        public void _ch_effect()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.weffect);
            if ((sbyte)reg.D0_B >= 0) return;

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern1;
                _ch_effect_exec();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern2;
                _ch_effect_exec();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern3;
                _ch_effect_exec();
            }

            cf = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (cf != 0)
            {
                reg.a3 = reg.a5 + w.we_pattern4;
                _ch_effect_exec();
            }

        }

        public void _ch_effect_exec()
        {
            reg.D4_B = mm.ReadByte(reg.a3 + w_we.mode);
            if ((sbyte)reg.D4_B >= 0)
            {
                cf = reg.D4_B & 1;
                reg.D4_B >>= 1;
                if (cf != 0)
                {
                    return;
                }
            }

            reg.D4_B = mm.ReadByte(reg.a3 + w_we.exec);
            if ((sbyte)reg.D4_B >= 0)
            {
                _effect_exec();
                return;
            }
            cf = reg.D4_B & 1;
            reg.D4_B >>= 1;
            if (cf == 0)
            {
                if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0)
                {
                    _effect_exec();
                }
                return;
            }

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0)
            {
                _effect_exec();
            }
        }

        //─────────────────────────────────────
        public void _effect_exec()
        {
            UInt16 sp = (UInt16)reg.D0_W;

            mm.Write(reg.a3 + w_we.delay_work, (byte)(mm.ReadByte(reg.a3 + w_we.delay_work) - 1));
            if (mm.ReadByte(reg.a3 + w_we.delay_work) != 0) goto _weffect_exit;
            mm.Write(reg.a3 + w_we.delay_work, mm.ReadByte(reg.a3 + w_we.speed));

            if (mm.ReadByte(reg.a3 + w_we.exec_flag) != 0) goto _weffect_exit;

            reg.a0 = mm.ReadUInt32(reg.a3 + w_we.adrs_work);
            reg.D0_W = mm.ReadUInt16(reg.a0); reg.a0 += 2;
            if (mm.ReadUInt32(reg.a3 + w_we.end_adrs_work) - reg.a0 != 0) goto _weffect_10;

            if (mm.ReadByte(reg.a3 + w_we.count) == 0)
            {
                mm.Write(reg.a3 + w_we.exec_flag, 0xff);
                goto _weffect_10;
            }

            reg.a0 = mm.ReadUInt32(reg.a3 + w_we.start_adrs_work);

            if (mm.ReadByte(reg.a3 + w_we.loop_flag) == 0) goto _weffect_10;

            mm.Write(reg.a3 + w_we.lp_cnt_work, (UInt32)(mm.ReadUInt32(reg.a3 + w_we.lp_cnt_work) - 1));
            if (mm.ReadUInt32(reg.a3 + w_we.lp_cnt_work) == 0)
            {
                mm.Write(reg.a3 + w_we.exec_flag, (byte)0xff);
            }

            _weffect_10:
            mm.Write(reg.a3 + w_we.adrs_work, reg.a0);
            reg.a0 = mm.ReadUInt32(reg.a3 + w_we.exec_adrs);
            ab.hlw_we_exec_adrs[reg.a0]();

            _weffect_exit:
            reg.D0_W = sp;
        }

        //─────────────────────────────────────
        //─────────────────────────────────────
        //	wavememory
        //
        public void _com_wavememory()
        {
            reg.D4_W = mm.ReadUInt16(reg.a4 + w_l.flag);
            if ((Int16)reg.D4_W >= 0)
            {
                _com_wave_exec();
                return;
            }

            cf = reg.D4_B & 1;
            reg.D4_B >>= 1;
            if (cf == 0)
            {
                if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0)
                {
                    _com_wave_exec();
                }
                return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0)
            {
                _com_wave_exec();
            }
        }

        //─────────────────────────────────────
        public void _com_wave_exec()
        {
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a3 + w_w.type);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_W)
            {
                case 2:
                    _com_wave_normal();     // $00 repeat
                    break;
                case 4:
                    _com_wave_normal();	    // $01 1shot
                    break;
                case 6:
                    _com_wave_nop();		// $02
                    break;
                case 8:
                    _com_wave_nop(); 		// $03
                    break;
                case 10:
                    _com_wave_nop(); 		// $04
                    break;
                case 12:
                    _com_wave_nop();  		// $05
                    break;
                case 14:
                    _com_wave_nop();  		// $06
                    break;
                case 16:
                    _com_wave_nop();  		// $07
                    break;
                case 18:
                    _com_wave_nop();  		// $08
                    break;
                case 20:
                    _com_wave_nop();   		// $09
                    break;
                case 22:
                    _com_wave_nop(); 		// $0A
                    break;
                case 24:
                    _com_wave_nop();  		// $0B
                    break;
                case 26:
                    _com_wave_nop(); 		// $0C
                    break;
                case 28:
                    _com_wave_nop();  		// $0D
                    break;
                case 30:
                    _com_wave_nop();  		// $0E
                    break;
                case 32:
                    _com_wave_lw();	    // $0F
                    break;

                case 34:
                    _com_wave_saw();      	// $10
                    break;
                case 36:
                    _com_wave_square();   	// $11
                    break;
                case 38:
                    _com_wave_triangle();  	// $12
                    break;
                case 40:
                    _com_wave_randome();  	// $13
                    break;
                case 42:
                    _com_wave_saw_a();    	// $14
                    break;
                case 44:
                    _com_wave_square_a(); 	// $15
                    break;
                case 46:
                    _com_wave_triangle_a();	// $16
                    break;
                case 48:
                    _com_wave_randome_a();	// $17
                    break;
                case 50:
                    _com_wave_nop();      	// $18
                    break;
                case 52:
                    _com_wave_nop();      	// $19
                    break;
                case 54:
                    _com_wave_nop();      	// $1A
                    break;
                case 56:
                    _com_wave_nop();       	// $1B
                    break;
                case 58:
                    _com_wave_nop();     	// $1C
                    break;
                case 60:
                    _com_wave_nop();       	// $1D
                    break;
                case 62:
                    _com_wave_nop();      	// $1E
                    break;
                case 64:
                    _com_wave_nop();      	// $1F
                    break;
            }
        }

        //─────────────────────────────────────
        public void _com_wave_nop()
        {
            reg.D0_L = 0;
        }

        //─────────────────────────────────────
        public void _com_wave_normal()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            if (mm.ReadByte(reg.a3 + w_w.exec_flag) != 0) goto _com_wave_exit;

            reg.a0 = mm.ReadUInt32(reg.a3 + w_w.adrs_work);
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a3 + w_w.depth);
            reg.D0_L = (UInt16)((Int16)mm.ReadUInt16(reg.a0) * (Int16)reg.D0_W);// 68020未満のCPUはw*w=lのみ?

            if (reg.a0 - mm.ReadUInt32(reg.a3 + w_w.end_adrs_work) != 0) goto _com_w10;

            if (mm.ReadByte(reg.a3 + w_w.type) == 0)
            {
                mm.Write(reg.a3 + w_w.exec_flag, 0xff);
                goto _com_w10;
            }
            reg.a0 = mm.ReadUInt32(reg.a3 + w_w.start_adrs_work);
            if (mm.ReadByte(reg.a3 + w_w.loop_flag) == 0) goto _com_w10;

            mm.Write(reg.a3 + w_w.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_w.lp_cnt_work) - 1);
            if (mm.ReadUInt32(reg.a3 + w_w.lp_cnt_work) == 0)
            {
                mm.Write(reg.a3 + w_w.exec_flag, (byte)0xff);
            }

            _com_w10:
            mm.Write(reg.a3 + w_w.adrs_work, reg.a0);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D0_W);
            return;

            _com_wave_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        public void _com_wave_lw()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_k_exit;

            reg.a0 = mm.ReadUInt32(reg.a3 + w_w.adrs_work);
            _com_wave_k_loop:
            reg.D0_B = mm.ReadByte(reg.a0++);
            if (reg.D0_B - 0x80 == 0) goto _com_wave_k_loop;
            if (reg.D0_B >= 0xf0) goto _com_wave_k_command;

            //_com_wave_k1:
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D1_B = reg.D0_B;
            reg.D0_W &= 0x7f;
            reg.D2_L = 0;
            reg.D2_B = mm.ReadByte(reg.a3 + w_w.depth);
            reg.D0_L = reg.D2_W * reg.D0_W;
            if ((sbyte)reg.D1_B < 0)
            {
                reg.D0_W = (UInt16)(-(Int16)reg.D0_W);
            }
            mm.Write(reg.a3 + w_w.adrs_work, reg.a0);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D0_W);
            return;

            _com_wave_k_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
            return;

            _com_wave_k_command:
            if (reg.D0_B - 0xff == 0) goto _com_wave_k_command_end;

            reg.D0_B &= 0xf;
            if (reg.D0_B == 0) goto _com_wave_k_command_go_loop;

            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D0_B);
            goto _com_wave_k_loop;

            _com_wave_k_command_end:
            return;

            _com_wave_k_command_go_loop:
            mm.Write(reg.a3 + w_w.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_w.lp_cnt_work) & 1);
            if (mm.ReadUInt32(reg.a3 + w_w.lp_cnt_work) == 0)
            {
                mm.Write(reg.a3 + w_w.loop_start, reg.a0);
                goto _com_wave_k_loop;
            }
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a0++);
            if (reg.D0_L - mm.ReadUInt32(reg.a3 + w_w.lp_cnt_work) == 0)
            {
                mm.Write(reg.a3 + w_w.lp_cnt_work, (UInt32)0);
                if (reg.D0_B - 0xff != 0)
                {
                    reg.D0_L = 0xff;
                    mm.Write(reg.a3 + w_w.lp_cnt_work, reg.D0_L);
                }
            }
            reg.a0 = mm.ReadUInt32(reg.a3 + w_w.loop_start);
            goto _com_wave_k_loop;
        }

        //─────────────────────────────────────
        //
        public void _com_wave_saw()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_saw_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            reg.D0_L = mm.ReadUInt32(reg.a3 + w_w.ko_loop_end);
            mm.Write(reg.a3 + w_w.ko_loop_count, (UInt32)(mm.ReadUInt32(reg.a3 + w_w.ko_loop_count) + (Int32)reg.D0_L));
            mm.Write(reg.a3 + w_w.ko_loop_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.start));
                mm.Write(reg.a3 + w_w.ko_loop_count, (UInt32)(-(Int32)mm.ReadByte(reg.a3 + w_w.ko_loop_count)));
            }
            mm.Write(reg.a4 + w_l.bendwork, mm.ReadUInt16(reg.a3 + w_w.ko_loop_count));
            _com_wave_saw_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_square()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_square_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            reg.D0_L = mm.ReadUInt32(reg.a3 + w_w.ko_loop_end);
            mm.Write(reg.a3 + w_w.ko_loop_count, reg.D0_L);
            mm.Write(reg.a3 + w_w.ko_loop_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.start));
                mm.Write(reg.a3 + w_w.ko_loop_end, (UInt32)(-(Int32)mm.ReadUInt32(reg.a3 + w_w.ko_loop_end)));
            }
            mm.Write(reg.a4 + w_l.bendwork, mm.ReadUInt16(reg.a3 + w_w.ko_loop_count));
            _com_wave_square_exit:
            reg.D0_W= mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_triangle()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_triangle_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            reg.D0_L = mm.ReadUInt32(reg.a3 + w_w.ko_loop_end);
            mm.Write(reg.a3 + w_w.ko_loop_count, mm.ReadUInt32(reg.a3 + w_w.ko_loop_count) +reg.D0_L);
            mm.Write(reg.a3 + w_w.ko_loop_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.start));
                mm.Write(reg.a3 + w_w.ko_loop_end, (UInt32)(-(Int32)mm.ReadUInt32(reg.a3 + w_w.ko_loop_end)));
            }
            mm.Write(reg.a4 + w_l.bendwork, mm.ReadUInt16(reg.a3 + w_w.ko_loop_count));
            _com_wave_triangle_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_randome()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_randome_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            mm.Write(reg.a3 + w_w.ko_loop_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_loop_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.start));
                comlfo.GETRND();
                reg.D1_L = mm.ReadUInt32(reg.a3 + w_w.ko_loop_end);
                reg.D0_L = (UInt16)((Int16)reg.D1_W * (Int16)reg.D0_W);
                mm.Write(reg.a3 + w_w.ko_loop_count, reg.D0_L);
            }
            mm.Write(reg.a4 + w_l.bendwork, mm.ReadUInt16(reg.a3 + w_w.ko_loop_count));
            _com_wave_randome_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_saw_a()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_saw_a_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            reg.D0_W = mm.ReadUInt16(reg.a3 + w_w.ko_loop_start);
            mm.Write(reg.a3 + w_w.ko_loop_end, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_end) + (Int16)reg.D0_W));
            mm.Write(reg.a3 + w_w.ko_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_start, mm.ReadUInt16(reg.a3 + w_w.start));
                mm.Write(reg.a3 + w_w.ko_loop_end, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.loop_end)));
            }
            reg.D0_B = mm.ReadByte(reg.a3 + w_w.ko_loop_end);
            reg.D0_W = (UInt16)(sbyte)reg.D0_B;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D0_W);
            return;
            _com_wave_saw_a_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_square_a()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_square_a_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            reg.D0_W = mm.ReadUInt16(reg.a3 + w_w.ko_loop_start);
            mm.Write(reg.a3 + w_w.ko_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_start, mm.ReadUInt16(reg.a3 + w_w.start));
                mm.Write(reg.a3 + w_w.ko_loop_end, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_end) + (Int16)reg.D0_W));
                mm.Write(reg.a3 + w_w.ko_loop_start, (UInt16)(-(Int16)mm.ReadUInt16(reg.a3 + w_w.ko_loop_start)));
            }
            reg.D0_B = mm.ReadByte(reg.a3 + w_w.ko_loop_end);
            reg.D0_W = (UInt16)(sbyte)reg.D0_B;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D0_W);
            return;
            _com_wave_square_a_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_triangle_a()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_triangle_a_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            reg.D0_W = mm.ReadUInt16(reg.a3 + w_w.ko_loop_start);
            mm.Write(reg.a3 + w_w.ko_loop_end, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_loop_end) + (Int16)reg.D0_W));
            mm.Write(reg.a3 + w_w.ko_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_start, mm.ReadUInt16(reg.a3 + w_w.start));
                mm.Write(reg.a3 + w_w.ko_loop_start, (UInt16)(-(Int16)mm.ReadUInt16(reg.a3 + w_w.ko_loop_start)));
            }
            reg.D0_B = mm.ReadByte(reg.a3 + w_w.ko_loop_end);
            reg.D0_W = (UInt16)(sbyte)reg.D0_B;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D0_W);
            return;

            _com_wave_triangle_a_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

        //─────────────────────────────────────
        //
        public void _com_wave_randome_a()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) goto _com_wave_randome_a_exit;
            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));

            mm.Write(reg.a3 + w_w.ko_start, (UInt16)(mm.ReadUInt16(reg.a3 + w_w.ko_start) - 1));
            if (mm.ReadUInt16(reg.a3 + w_w.ko_start) == 0)
            {
                mm.Write(reg.a3 + w_w.ko_start, mm.ReadUInt16(reg.a3 + w_w.start));
                comlfo.GETRND();
                reg.D1_W = mm.ReadUInt16(reg.a3 + w_w.ko_loop_start);
                reg.D0_L = (UInt16)((Int16)reg.D1_W * (Int16)reg.D0_W);
                mm.Write(reg.a3 + w_w.ko_loop_end, (UInt16)reg.D0_W);
            }
            reg.D0_B = mm.ReadByte(reg.a3 + w_w.ko_loop_end);
            reg.D0_W = (UInt16)(sbyte)reg.D0_B;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D0_W);
            return;

            _com_wave_randome_a_exit:
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.bendwork);
        }

    }
}
