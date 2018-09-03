using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class devopm
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public mndrv mndrv;
        public comanalyze comanalyze;
        public comcmds comcmds;
        public comlfo comlfo;
        public comwave comwave;

        //
        //	part of YM2151
        //

        //─────────────────────────────────────
        public void _opm_note_set()
        {
            mm.Write(reg.a5 + w.key, (byte)reg.D0_B);
            reg.D2_L = 0;
            reg.D2_B = reg.D0_B;

            reg.D2_W <<= 6;
            reg.D2_W += 5;
            reg.D2_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D2_W < 0)
            {
                reg.D2_L = 0;
            }

            mm.Write(reg.a5 + w.keycode2, (UInt16)reg.D2_W);
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            mm.Write(reg.a5 + w.keycode_s4, (UInt16)reg.D2_W);
            //    pea _opm_keyon(pc)
            _set_kckf();
            comlfo._init_lfo();
            _init_lfo_opm();
            _opm_keyon();
        }

        //─────────────────────────────────────
        //	SET KC/KF
        //
        public void _set_kckf()
        {
            mm.Write(reg.a5 + w.keycode, (UInt16)reg.D2_W);
            //_set_kckf2:
            reg.D0_W = reg.D2_W;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;

            reg.D1_L = 0x30;
            mndrv._OPM_WRITE4();
            reg.D0_L = reg.D0_W >> 8;
            if (reg.D0_B - mm.ReadByte(reg.a5 + w.ch3) != 0)
            {
                mm.Write(reg.a5 + w.ch3, (byte)reg.D0_B);
                reg.D0_B = _kc_table[reg.D0_W];// mm.ReadByte(_kc_table + reg.D0_W);
                reg.D1_L = 0x28;
                mndrv._OPM_WRITE4();
            }
        }

        //─────────────────────────────────────
        public byte[] _kc_table = new byte[] {
                0x00,0x00,0x00,0x00,0x01,0x02,0x04,0x05,0x06,0x08,0x09,0x0A
                ,0x0C,0x0D,0x0E,0x10,0x11,0x12,0x14,0x15,0x16,0x18,0x19,0x1A
                ,0x1C,0x1D,0x1E,0x20,0x21,0x22,0x24,0x25,0x26,0x28,0x29,0x2A
                ,0x2C,0x2D,0x2E,0x30,0x31,0x32,0x34,0x35,0x36,0x38,0x39,0x3A
                ,0x3C,0x3D,0x3E,0x40,0x41,0x42,0x44,0x45,0x46,0x48,0x49,0x4A
                ,0x4C,0x4D,0x4E,0x50,0x51,0x52,0x54,0x55,0x56,0x58,0x59,0x5A
                ,0x5C,0x5D,0x5E,0x60,0x61,0x62,0x64,0x65,0x66,0x68,0x69,0x6A
                ,0x6C,0x6D,0x6E,0x70,0x71,0x72,0x74,0x75,0x76,0x78,0x79,0x7A
                ,0x7C,0x7D,0x7E,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F
                ,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F
                ,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F
        };

        //─────────────────────────────────────
        //	KEY ON
        //
        public void _opm_keyon()
        {
            reg.D0_L = 3;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.effect);
            if (reg.D0_B - 3 == 0)
            {
                _OPM_RR_cut();
            }
            comwave._wave_init_kon();

            if ((sbyte)mm.ReadByte(reg.a5 + w.flag2) < 0)
            {
                reg.D0_B = mm.ReadByte(reg.a5 + w.dev);
                reg.D1_L = 8;
                mndrv._OPM_WRITE();
                return;
            }
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x20));

            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x40) == 0)
            {
                if ((mm.ReadByte(reg.a5 + w.flag) & 0x40) != 0) return;
            }
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xfb));

            if ((sbyte)mm.ReadByte(reg.a5 + w.reverb) < 0)
            {
                _OPM_echo_ret();
            }

            mm.Write(reg.a5 + w.e_p, (byte)5);
            reg.D0_B = mm.ReadByte(reg.a5 + w.dev);
            reg.D0_B |= mm.ReadByte(reg.a5 + w.smask);
            reg.D1_L = 8;
            mndrv._OPM_WRITE();

            reg.D0_L = 3;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.effect);
            if (reg.D0_B != 0)
            {
                _OPM_RR_ret();
            }
        }

        //─────────────────────────────────────
        //	KEY OFF
        //
        public void _opm_keyoff()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            comwave._wave_init_kof();

            reg.D0_B = mm.ReadByte(reg.a5 + w.effect);
            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f != 0)
            {
                f = reg.D0_B & 1;
                reg.D0_B >>= 1;
                if (f == 0)
                {
                    _OPM_RR_cut();
                }
            }

            if (mm.ReadByte(reg.a5 + w.kom) == 0)
            {
                _opm_keyoff2();
                return;
            }
            mm.Write(reg.a5 + w.e_p, 4);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x98));
        }

        public void _opm_keyoff2()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.dev);
            reg.D1_L = 8;
            mndrv._OPM_WRITE();
            mm.Write(reg.a5 + w.e_p, 4);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x98));
        }

        //─────────────────────────────────────
        //
        public void _OPM_RR_cut()
        {
            reg.D1_L = 0xe0;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            reg.D2_L = 0xf;
            reg.a2 = reg.a5 + w.tone_rr;
            reg.D0_B = mm.ReadByte(reg.a2++);
            reg.D0_B |= reg.D2_B;
            mndrv._OPM_WRITE();
            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            reg.D0_B |= reg.D2_B;
            mndrv._OPM_WRITE();
            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            reg.D0_B |= reg.D2_B;
            mndrv._OPM_WRITE();
            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            reg.D0_B |= reg.D2_B;
            mndrv._OPM_WRITE();
        }

        //─────────────────────────────────────
        //
        public void _OPM_RR_ret()
        {
            reg.D1_L = 0xe0;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            reg.a2 = reg.a5 + w.tone_rr;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();

            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();

            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();

            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();
        }

        //─────────────────────────────────────
        //
        public void _OPM_echo()
        {
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0) return;

            uint sp = reg.D4_W;

            mm.Write(reg.a5 + w.revexec, 0xff);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x98));
            mm.Write(reg.a5 + w.reverb_time_work, mm.ReadByte(reg.a5 + w.reverb_time));

            reg.D5_L = 7;
            reg.D5_B &= mm.ReadByte(reg.a5 + w.reverb);
            reg.D5_W += 1;
            reg.D5_W += (UInt32)(Int16)reg.D5_W;

            switch (reg.D5_W / 2)
            {
                case 1:
                    _opm_echo_volume();
                    break;
                case 2:
                    _opm_echo_volume_pan();
                    break;
                case 3:
                    _opm_echo_volume_tone();
                    break;
                case 4:
                    _opm_echo_volume_pan_tone();
                    break;
                case 5:
                    _opm_echo_volume();
                    break;
            }

            reg.D4_W = sp;
        }

        //─────────────────────────────────────
        public void _opm_echo_volume()
        {
            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x10) != 0)
            {
                _opm_echo_common_atv();
                return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x10) == 0)
            {
                _opm_echo_common_atv();
                return;
            }
            _opm_echo_common_v();
        }

        public void _opm_echo_volume_pan_tone()
        {
            _opm_echo_tone();
            _opm_echo_volume_pan();
        }

        public void _opm_echo_volume_pan()
        {
            _opm_echo_pan();
            _opm_echo_volume();
        }

        public void _opm_echo_volume_tone()
        {
            _opm_echo_tone();
            _opm_echo_volume();
        }

        public void _opm_echo_volume_()
        {
            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x10) != 0)
            {
                _opm_echo_volume_atv();
                return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x10) == 0)
            {
                _opm_echo_volume_atv();
                return;
            }
            _opm_echo_volume_v();
        }

        //─────────────────────────────────────
        public void _opm_echo_pan()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.D0_B &= 0x3f;
            reg.D0_B |= mm.ReadByte(reg.a5 + w.reverb_pan);
            reg.D1_L = 0x20;
            mndrv._OPM_WRITE4();
        }

        //─────────────────────────────────────
        public void _opm_echo_tone()
        {
            reg.D5_L = 0;
            reg.D5_B = mm.ReadByte(reg.a5 + w.reverb_tone);
            _opm_echo_tone_change();
        }

        //─────────────────────────────────────
        // v通常
        //
        public void _opm_echo_common_v()
        {
            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x08) != 0)
            {
                _opm_echo_direct_v();
                return;
            }

            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.volume);

            reg.D0_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            if ((sbyte)reg.D0_B >= 0)
            {
                _opm_echo_common_plus_v();
                return;
            }

            reg.D4_B += (UInt32)(sbyte)reg.D0_B;
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0;
            }
            reg.a0 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_softenv();
        }

        public void _opm_echo_common_plus_v()
        {
            reg.D4_B += (UInt32)(sbyte)reg.D0_B;
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0x7f;
            }
            if (reg.D4_B >= mm.ReadByte(reg.a5 + w.volcount))
            {
                reg.D4_B = mm.ReadByte(reg.a5 + w.volcount);
                reg.D4_B--;
            }
            reg.a0 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        // @v通常
        //
        public void _opm_echo_common_atv()
        {
            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x08) != 0)
            {
                _opm_echo_direct_atv();
                return;
            }

            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);

            reg.D0_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            if ((sbyte)reg.D0_B >= 0)
            {
                _opm_echo_common_plus();
                return;
            }

            reg.D4_B += (UInt32)(sbyte)reg.D0_B;
            if ((sbyte)reg.D4_B >= 0)
            {
                _OPM_F2_softenv();
            }
            reg.D4_L = 0;
            _OPM_F2_softenv();
        }

        public void _opm_echo_common_plus()
        {
            reg.D4_B += (UInt32)(sbyte)reg.D0_B;
            if ((sbyte)reg.D4_B >= 0)
            {
                _OPM_F2_softenv();
            }
            reg.D4_L = 0x7f;
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        // v微調整
        //
        public void _opm_echo_volume_v()
        {
            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.volume);
            reg.D0_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            if ((sbyte)reg.D0_B < 0)
            {
                _opm_echo_vol_v_plus();
                return;
            }

            reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0;
            }
            reg.D4_B >>= 1;
            reg.a0 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_softenv();
        }

        public void _opm_echo_vol_v_plus()
        {
            reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
            reg.D4_B >>= 1;
            reg.D4_W &= 0x7f;
            reg.a0 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        // @v微調整
        //
        public void _opm_echo_volume_atv()
        {
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D0_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            reg.D4_B += (UInt32)(sbyte)reg.D0_B;
            reg.D0_L = 127;
            reg.D0_B = (byte)((sbyte)reg.D0_B - (sbyte)reg.D4_B);
            reg.D0_B >>= 1;
            reg.D4_L = 127;
            reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        // v直接
        //
        public void _opm_echo_direct_v()
        {
            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            reg.a0 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        // @v直接
        //
        public void _opm_echo_direct_atv()
        {
            reg.D4_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        public void _OPM_echo_ret()
        {
            uint sp = reg.D4_W;

            mm.Write(reg.a5 + w.revexec, 0);
            mm.Write(reg.a5 + w.reverb_time_work, 0);

            reg.D0_L = 7;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.ch);
            reg.D1_L = 8;
            mndrv._OPM_WRITE();

            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x02) != 0)
            {
                reg.D5_B = mm.ReadByte(reg.a5 + w.program2);
                _opm_echo_tone_change();
                reg.D4_W = sp;
                return;
            }
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x10) != 0)
            {
                reg.D4_L = 0;
                reg.D4_B = mm.ReadByte(reg.a5 + w.volume);
                reg.a0 = reg.a5 + w.voltable;
                reg.D4_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D4_W);
            }

            _OPM_F2_softenv();
            reg.D5_B = mm.ReadByte(reg.a5 + w.reverb);
            uint f = reg.D5_B & 1;
            reg.D5_B >>= 1;
            if (f != 0)
            {
                reg.D0_B = mm.ReadByte(reg.a5 + w.pan_ampm);
                reg.D1_L = 0x20;
                mndrv._OPM_WRITE4();
            }
            reg.D4_W = sp;

        }

        //─────────────────────────────────────
        //
        public void _init_lfo_opm()
        {
            if (mm.ReadByte(reg.a6 + dw.FADEFLAG) == 0)
            {
                if ((mm.ReadByte(reg.a5 + w.effect) & 0x20) != 0) return;

                reg.D5_L = 0x70;
                reg.D5_B &= mm.ReadByte(reg.a5 + w.lfo);
                if (reg.D5_B == 0) return;

                reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
                _OPM_F2_init();
            }

            reg.D5_B = mm.ReadByte(reg.a5 + w.lfo);
            uint f = reg.D5_B & 0x40;
            reg.D5_B <<= 2;
            if (f != 0)
            {
                reg.a4 = reg.a5 + w.v_pattern3;
                comlfo._init_lfo_common_a();
            }
            f = reg.D5_B & 0x80;
            reg.D5_B <<= 1;
            if (f != 0)
            {
                reg.a4 = reg.a5 + w.v_pattern2;
                comlfo._init_lfo_common_a();
            }
            f = reg.D5_B & 0x80;
            reg.D5_B <<= 1;
            if (f != 0)
            {
                reg.a4 = reg.a5 + w.v_pattern1;
                comlfo._init_lfo_common_a();
            }
        }

        //─────────────────────────────────────
        //
        public void _init_opm_hlfo()
        {
            if ((mm.ReadByte(reg.a5 + w.lfo) & 1) != 0)
            {


                ///HACK MNDRV:LFO syncの適用
                reg.D0_B = mm.ReadByte(reg.a5 + w.sdetune2);
                ////////
                //original Code
                //if (reg.D0_B != 0)
                //{
                //    reg.D1_L = 0x01;
                //    mndrv._OPM_WRITE();
                //    reg.D0_L = 0;
                //    mndrv._OPM_WRITE();
                //}
                //kuma Code
                reg.D1_L = 0x01;
                mndrv._OPM_WRITE();
                reg.D0_L = 0;
                if (reg.D0_B != 0)
                {
                    reg.D0_L = 2;
                }
                mndrv._OPM_WRITE();
                ////////

                reg.D0_L = 0xc;
                reg.D0_B &= mm.ReadByte(reg.a5 + w.flag2);
                if (reg.D0_B != 0)
                {
                    reg.a4 = reg.a5 + w.v_pattern4;
                    mm.Write(reg.a4 + w_l.henka_work, (byte)0);
                    reg.D0_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
                    reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
                    mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
                    reg.D0_L = 0x00;
                    reg.D1_L = 0x38;
                    mndrv._OPM_WRITE4();
                }
            }
        }

        //─────────────────────────────────────
        //	MML コマンド処理 ( FM 部 )
        //
        public void _opm_command()
        {
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            //_opmc:
            switch (reg.D0_W / 2)
            {
                //case 0x00:0
                case 0x01: comcmds._COM_81(); break;// 81
                case 0x02: _OPM_82(); break;// 82	key off
                case 0x03: comcmds._COM_83(); break;// 83	すらー
                case 0x04: _OPM_NOP(); break;// 84
                case 0x05: _OPM_NOP(); break;// 85
                case 0x06: comcmds._COM_86(); break;// 86	同期信号送信
                case 0x07: comcmds._COM_87(); break;// 87	同期信号待ち
                case 0x08: _OPM_88(); break;// 88	ぴっちべんど
                case 0x09: _OPM_89(); break;// 89	ぽるためんと
                case 0x0a: _OPM_8A(); break;// 8A	ぽるためんと係数変更
                case 0x0b: _OPM_8B(); break;// 8B	ぽるためんと2
                case 0x0c: _OPM_NOP(); break;// 8C
                case 0x0d: _OPM_NOP(); break;// 8D
                case 0x0e: _OPM_NOP(); break;// 8E
                case 0x0f: _OPM_NOP(); break;// 8F

                case 0x10: comcmds._COM_90(); break;// 90	q
                case 0x11: comcmds._COM_91(); break;// 91	@q
                case 0x12: _OPM_92(); break;// 92	keyoff rr cut switch
                case 0x13: comcmds._COM_93(); break;// 93	neg @q
                case 0x14: comcmds._COM_94(); break;// 94	keyoff mode
                case 0x15: _OPM_NOP(); break;// 95
                case 0x16: _OPM_NOP(); break;// 96
                case 0x17: _OPM_NOP(); break;// 97
                case 0x18: _OPM_98(); break;// 98	擬似リバーブ
                case 0x19: _OPM_99(); break;// 99	擬似エコー
                case 0x1a: comcmds._COM_9A(); break;// 9A	擬似リバーブタイム
                case 0x1b: _OPM_NOP(); break;// 9B
                case 0x1c: _OPM_NOP(); break;// 9C
                case 0x1d: _OPM_NOP(); break;// 9D
                case 0x1e: _OPM_NOP(); break;// 9E
                case 0x1f: _OPM_NOP(); break;// 9F

                case 0x20: _OPM_F0(); break;// A0	音色切り替え
                case 0x21: _OPM_A1(); break;// A1	バンク&音色切り替え
                case 0x22: _OPM_NOP(); break;// A2
                case 0x23: _OPM_A3(); break;// A3	音量テーブル切り替え
                case 0x24: _OPM_F2(); break;// A4	音量
                case 0x25: _OPM_F5(); break;// A5
                case 0x26: _OPM_F6(); break;// A6
                case 0x27: _OPM_NOP(); break;// A7
                case 0x28: comcmds._COM_A8(); break;// A8	相対音量モード
                case 0x29: _OPM_NOP(); break;// A9
                case 0x2a: _OPM_NOP(); break;// AA
                case 0x2b: _OPM_NOP(); break;// AB
                case 0x2c: _OPM_NOP(); break;// AC
                case 0x2d: _OPM_NOP(); break;// AD
                case 0x2e: _OPM_NOP(); break;// AE
                case 0x2f: _OPM_NOP(); break;// AF

                case 0x30: comcmds._COM_B0(); break;// B0
                case 0x31: _OPM_NOP(); break;// B1
                case 0x32: _OPM_NOP(); break;// B2
                case 0x33: _OPM_NOP(); break;// B3
                case 0x34: _OPM_NOP(); break;// B4
                case 0x35: _OPM_NOP(); break;// B5
                case 0x36: _OPM_NOP(); break;// B6
                case 0x37: _OPM_NOP(); break;// B7
                case 0x38: _OPM_NOP(); break;// B8
                case 0x39: _OPM_NOP(); break;// B9
                case 0x3a: _OPM_NOP(); break;// BA
                case 0x3b: _OPM_NOP(); break;// BB
                case 0x3c: _OPM_NOP(); break;// BC
                case 0x3d: _OPM_NOP(); break;// BD
                case 0x3e: comcmds._COM_BE(); break;// BE	ジャンプ
                case 0x3f: comcmds._COM_BF(); break;// BF

                // PSG 系
                case 0x40: comcmds._COM_C0(); break;// C0	ソフトウェアエンベロープ 1
                case 0x41: comcmds._COM_C1(); break;// C1	ソフトウェアエンベロープ 2
                case 0x42: _OPM_NOP(); break;// C2
                case 0x43: comcmds._COM_C3(); break;// C3	switch
                case 0x44: comcmds._COM_C4(); break;// C4	env set
                case 0x45: comcmds._COM_C5(); break;// C5	env set (bank + num)
                case 0x46: _OPM_NOP(); break;// C6
                case 0x47: _OPM_NOP(); break;// C7
                case 0x48: _OPM_C8(); break;// C8	ノイズ周波数
                case 0x49: _OPM_NOP(); break;// C9
                case 0x4a: _OPM_NOP(); break;// CA
                case 0x4b: _OPM_NOP(); break;// CB
                case 0x4c: _OPM_NOP(); break;// CC
                case 0x4d: _OPM_NOP(); break;// CD
                case 0x4e: _OPM_NOP(); break;// CE
                case 0x4f: _OPM_NOP(); break;// CF

                // KEY 系
                case 0x50: comcmds._COM_D0(); break;// D0	キートランスポーズ
                case 0x51: comcmds._COM_D1(); break;// D1	相対キートランスポーズ
                case 0x52: _OPM_NOP(); break;// D2
                case 0x53: _OPM_NOP(); break;// D3
                case 0x54: _OPM_NOP(); break;// D4
                case 0x55: _OPM_NOP(); break;// D5
                case 0x56: _OPM_NOP(); break;// D6
                case 0x57: _OPM_NOP(); break;// D7
                case 0x58: comcmds._COM_D8(); break;// D8	ディチューン
                case 0x59: comcmds._COM_D9(); break;// D9	相対ディチューン
                case 0x5a: _OPM_NOP(); break;// DA
                case 0x5b: _OPM_NOP(); break;// DB
                case 0x5c: _OPM_NOP(); break;// DC
                case 0x5d: _OPM_NOP(); break;// DD
                case 0x5e: _OPM_NOP(); break;// DE
                case 0x5f: _OPM_NOP(); break;// DF

                // LFO 系
                case 0x60: _OPM_E0(); break;// E0	hardware LFO
                case 0x61: _OPM_E1(); break;// E1	hardware LFO switch
                case 0x62: comcmds._COM_E2(); break;// E2	pitch LFO
                case 0x63: comcmds._COM_E3(); break;// E3	pitch LFO switch
                case 0x64: comcmds._COM_E4(); break;// E4	pitch LFO delay
                case 0x65: _OPM_NOP(); break;// E5
                case 0x66: _OPM_NOP(); break;// E6
                case 0x67: comcmds._COM_E7(); break;// E7	amp LFO
                case 0x68: _OPM_E8(); break;// E8	amp LFO switch
                case 0x69: comcmds._COM_E9(); break;// E9	amp LFO delay
                case 0x6a: comcmds._COM_EA(); break;// EA	amp LFO switch 2
                case 0x6b: comcmds._COM_EB(); break;// EB
                case 0x6c: _OPM_NOP(); break;// EC
                case 0x6d: comcmds._COM_ED(); break;// ED
                case 0x6e: _OPM_EE(); break;// EE	LW type LFO
                case 0x6f: comcmds._COM_EF(); break;// EF	hardware LFO delay

                // システムコントール系
                case 0x70: _OPM_F0(); break;// F0	@
                case 0x71: comcmds._COM_D8(); break;// F1
                case 0x72: _OPM_F2(); break;// F2	volume
                case 0x73: comcmds._COM_91(); break;// F3
                case 0x74: _OPM_F4(); break;// F4	pan
                case 0x75: _OPM_F5(); break;// F5	) volup
                case 0x76: _OPM_F6(); break;// F6	( voldown
                case 0x77: _OPM_NOP(); break;// F7
                case 0x78: _OPM_F8(); break;// F8
                case 0x79: comcmds._COM_F9(); break;// F9	永久ループポイントマーク
                case 0x7a: _OPM_FA(); break;// FA	y command
                case 0x7b: comcmds._COM_FB(); break;// FB	リピート抜け出し
                case 0x7c: comcmds._COM_FC(); break;// FC	リピート開始
                case 0x7d: comcmds._COM_FD(); break;// FD	リピート終端
                case 0x7e: comcmds._COM_FE(); break;// FE	tempo
                case 0x7f: _OPM_FF(); break;// FF	end of data
            }
        }

        //─────────────────────────────────────
        //
        public void _OPM_NOP()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x7f));
            _opm_keyoff2();
        }

        //─────────────────────────────────────
        //
        public void _OPM_82()
        {
            _opm_keyoff2();
        }

        //─────────────────────────────────────
        //	ピッチベンド
        //		[$88] + [目標音程]b + [delay]b + [speed]b + [rate]w
        //
        public void _OPM_88()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x80));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfd));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & 0xfb));
            reg.a4 = reg.a5 + w.p_pattern4;

            reg.D0_L = 0;
            reg.D2_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D2_B += mm.ReadByte(reg.a5 + w.key_trans);
            if ((sbyte)reg.D2_B >= 0)
            {
                reg.D0_B += (UInt32)(sbyte)reg.D2_B;
                if ((sbyte)reg.D0_B < 0)
                {
                    reg.D0_L = 0x7f;
                }
            }
            else
            {
                reg.D0_B = reg.D2_B;
                if ((sbyte)reg.D0_B < 0)
                {
                    reg.D0_L = 0;
                }
            }
            mm.Write(reg.a5 + w.key2, (byte)reg.D0_B);
            reg.D2_B = reg.D0_B;
            reg.D2_W <<= 6;
            reg.D2_W += 5;
            reg.D2_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D2_W < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a4 + w_l.mokuhyou, (UInt16)reg.D2_W);

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_B = mm.ReadByte(reg.a1++);
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D1_B);

            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a4 + w_l.henka, (UInt16)reg.D0_W);
        }

        //─────────────────────────────────────
        //	ポルタメント
        //		[$89] + [switch]b + [先note]b + [元note]b + [step]b
        //
        public void _OPM_89()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x80));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x02));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & 0xfb));
            reg.a4 = reg.a5 + w.p_pattern4;

            reg.D0_B = mm.ReadByte(reg.a1++);

            //_OPM_89_normal:
            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D2_L = 0;

            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.key2, (byte)reg.D0_B);
            reg.D2_B = reg.D0_B;
            reg.D2_W <<= 6;
            reg.D2_W += 5;
            reg.D2_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D2_W < 0)
            {
                reg.D2_L = 0;
            }
            mm.Write(reg.a4 + w_l.mokuhyou, (UInt16)reg.D2_W);

            reg.D2_L = 0;
            reg.D1_B = mm.ReadByte(reg.a1);
            reg.D2_B = mm.ReadByte(reg.a1 + 1);
            reg.D0_L = (UInt32)((Int32)reg.D0_L - (Int32)reg.D1_L);
            reg.D0_W <<= 6;
            reg.D0_L = (UInt32)((UInt16)((Int32)reg.D0_L / (Int16)reg.D2_W) | (UInt32)(((UInt16)((Int32)reg.D0_L % (Int16)reg.D2_W)) << 16));
            mm.Write(reg.a4 + w_l.henka, (UInt16)reg.D0_W);
            reg.D0_L = (reg.D0_L >> 16) | (reg.D0_L << 16);
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)reg.D0_W);
        }

        //─────────────────────────────────────
        //	ポルタメント係数変更
        //		[$8A] + [num]b
        //
        public void _OPM_8A()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0)
            {
                reg.a4 = reg.a5 + w.p_pattern4;
                mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D0_B);
                mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x10));
                return;
            }
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xef));
        }

        //─────────────────────────────────────
        //	ポルタメント
        //		[$89] + [変化値]w + [元note]b + [step]b
        //
        public void _OPM_8B()
        {
            reg.a4 = reg.a5 + w.p_pattern4;
            reg.a3 = reg.a5 + w.wp_pattern4;

            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            reg.D0_L = (UInt32)(Int16)reg.D0_W;
            reg.D2_L = reg.D0_L;

            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a1 + 1);
            mm.Write(reg.a4 + w_l.count, (byte)reg.D1_B);

            reg.D0_L = reg.D0_W * reg.D1_W;
            reg.D0_L = (UInt32)((Int32)reg.D0_L << 8);
            mm.Write(reg.a3 + w_w.loop_start, reg.D0_L);

            reg.D2_L = (UInt32)((Int32)reg.D2_L << 8);
            mm.Write(reg.a3 + w_w.start, reg.D2_L);
            mm.Write(reg.a3 + w_w.ko_start, (UInt32)0);

            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x80));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfd));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x04));
        }

        //─────────────────────────────────────
        //	RR cut設定
        //			[$92] + [switch]b
        //
        public void _OPM_92()
        {
            mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) | 0x3));
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0)
            {
                if ((sbyte)reg.D0_B >= 0)
                {
                    if (reg.D0_B - 2 == 0) return;
                    mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) & 0xfd));
                    return;
                }
                mm.Write(reg.a5 + w.rct, mm.ReadByte(reg.a1++));
                mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) & 0xfe));
                return;
            }
            mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) & 0xfc));
            _OPM_RR_ret();
        }

        //;─────────────────────────────────────
        //;	擬似リバーブ
        //;		switch = $80 = ON
        //;			 $81 = OFF
        //;			 $00 = + [volume]b
        //;			 $01 = + [volume]b + [pan]b
        //;			 $02 = + [volume]b + [tone]b
        //;			 $03 = + [volume]b + [panpot]b + [tone]b
        //;	work
        //;		bit1 1:tone change
        //;		bit0 1:panpot change
        //;
        public void _OPM_98()
        {
            comcmds._COM_98();

            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x80) == 0)
            {
                _opm_keyoff();
            }
        }

        //─────────────────────────────────────
        //	擬似エコー
        //
        public void _OPM_99()
        {
            comcmds._COM_99();
        }

        //─────────────────────────────────────
        //	バンク&音色切り替え
        //
        public void _OPM_A1()
        {
            mm.Write(reg.a5 + w.bank, mm.ReadByte(reg.a1++));
            _OPM_F0();
        }

        //─────────────────────────────────────
        //	音量テーブル
        //
        public void _OPM_A3()
        {
            comcmds._COM_A3();

            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x10) == 0) return;
            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.volume);
            reg.a0 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_v();
        }

        //─────────────────────────────────────
        //	OPM noise
        //		[$C8] + [freq]b
        public void _OPM_C8()
        {
            reg.D0_L = 0;
            reg.D1_B = mm.ReadByte(reg.a1++);
            if (reg.D1_B - 0xff != 0)
            {
                reg.D0_B = reg.D1_B;
                mm.Write(reg.a6 + dw.NOISE_O, (byte)reg.D0_B);
                reg.D0_B |= 0x80;
            }
            reg.D1_L = 0x0f;
            mndrv._OPM_WRITE();
        }

        //─────────────────────────────────────
        //	hardware LFO
        //
        //		[$E0] + [wf/sync] + [freq] + [pmd] + [amd] + [pms/ams]
        //
        public void _OPM_E0()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x01));

            ///HACK MNDRV:syncの保存できてない?
            //original code
            //reg.D1_B = mm.ReadByte(reg.a1++);
            //reg.D2_L = 3;
            //reg.D2_B &= reg.D1_B;
            //uint sp = reg.SR_W;
            //reg.SR_W |= 0x700;
            //reg.D0_B = mm.ReadByte(0x9da);
            //reg.D0_B &= 0xc0;
            //reg.D0_B += (UInt32)(sbyte)reg.D2_B;
            //mm.Write(0x9da, (byte)reg.D0_B);
            //reg.SR_W = sp;
            //reg.D1_L = 0x1b;
            //mndrv._OPM_WRITE();

            //kuma code
            reg.D1_B = mm.ReadByte(reg.a1++);
            byte sync = (byte)(reg.D1_B >> 4);
            sync <<= 5;
            reg.D2_L = 3;
            reg.D2_B &= reg.D1_B;
            uint sp = reg.SR_W;
            reg.SR_W |= 0x700;
            reg.D0_B = mm.ReadByte(0x9da);
            reg.D0_B &= 0xc0;
            reg.D0_B += (UInt32)(sbyte)reg.D2_B;
            mm.Write(0x9da, (byte)reg.D0_B);
            reg.SR_W = sp;
            reg.D1_L = 0x1b;
            mndrv._OPM_WRITE();

            reg.D2_B = sync;
            ////////

            reg.D2_B >>= 4;
            reg.D2_B &= 2;
            mm.Write(reg.a5 + w.sdetune2, (byte)reg.D2_B);
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_L = 0x18;
            mndrv._OPM_WRITE();

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_L = 0x19;
            mndrv._OPM_WRITE();

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_L = 0x19;
            mndrv._OPM_WRITE();

            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.fbcon, (byte)reg.D0_B);
            reg.D1_L = 0x38;
            mndrv._OPM_WRITE4();
        }

        //─────────────────────────────────────
        //	hardware LFO ON / OFF
        //
        //	$E1,[on / off]
        //
        public void _OPM_E1()
        {
            reg.a4 = reg.a5 + w.v_pattern4;

            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _OPM_E1_off();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _OPM_E1_normal();
                return;
            }

            if (reg.D0_B - 0xff == 0)
            {
                _OPM_E1_time();
                return;
            }
            if (reg.D0_B - 0x82 != 0)
            {
                uint f = reg.D0_B & 1;
                reg.D0_B >>= 1;
                if (f != 0)
                {
                    mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
                    _OPM_E1_normal();
                    return;
                }
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                _OPM_E1_normal();
                return;
            }
            //2:
            mm.Write(reg.a4 + w_l.mokuhyou, (UInt16)0);
            _OPM_E1_normal();
        }

        public void _OPM_E1_normal()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x01));
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));

            if ((mm.ReadByte(reg.a5 + w.flag2) & 0x08) != 0)
            {
                _OPM_E1_LW();
                return;
            }
            reg.D0_B = mm.ReadByte(reg.a5 + w.fbcon);
            reg.D1_L = 0x38;
            mndrv._OPM_WRITE4();
        }

        public void _OPM_E1_off()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xfe));
            _OPM_E1_LW();
        }

        public void _OPM_E1_LW()
        {
            reg.D0_L = 0x0;
            reg.D1_L = 0x38;
            mndrv._OPM_WRITE4();
        }

        public void _OPM_E1_time()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0)
            {
                mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x08));
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x01));

                reg.a4 = reg.a5 + w.v_pattern4;
                mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D0_B);
                reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
                mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
                return;
            }
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xf7));
            reg.D0_L = 0;
            reg.D1_L = 0x38;
            mndrv._OPM_WRITE4();
        }

        //─────────────────────────────────────
        //	音量 LFO on /off
        //
        //	$E8,num,switch
        public void _OPM_E8()
        {
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            //	pea	_COM_E8
            _OPM_F2_init();
            comcmds._COM_E8();
        }

        //─────────────────────────────────────
        //	LW type LFO
        //		[$EE] + [wf/sync] + [freq] + [pmd] + [amd] + [speed]b
        //
        public void _OPM_EE()
        {
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x08));
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x01));

            reg.D1_L = 0x03;
            reg.D1_B &= mm.ReadByte(reg.a1++);

            ///HACK MNDRV:syncの保存できてない?
            //original code
            //uint sp = reg.SR_W;
            //reg.SR_W |= 0x700;
            //reg.D0_B = mm.ReadByte(0x9da);
            //reg.D0_B &= 0xc0;
            //reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            //mm.Write(0x9da, (byte)reg.D0_B);
            //reg.SR_W = sp;
            //reg.D1_L = 0x1b;
            //mndrv._OPM_WRITE();

            //kuma code
            byte sync = (byte)(reg.D1_B >> 4);
            sync <<= 5;
            uint sp = reg.SR_W;
            reg.SR_W |= 0x700;
            reg.D0_B = mm.ReadByte(0x9da);
            reg.D0_B &= 0xc0;
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            mm.Write(0x9da, (byte)reg.D0_B);
            reg.SR_W = sp;
            reg.D1_L = 0x1b;
            mndrv._OPM_WRITE();
            reg.D0_B = sync;
            ////////

            reg.D0_B >>= 4;
            reg.D0_B &= 2;
            mm.Write(reg.a5 + w.sdetune2, (byte)reg.D0_B);

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_L = 0x18;
            mndrv._OPM_WRITE();

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_L = 0x19;
            mndrv._OPM_WRITE();

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_L = 0x19;
            mndrv._OPM_WRITE();

            reg.a4 = reg.a5 + w.v_pattern4;
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D0_B);
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	音色設定
        //
        public void _OPM_F0()
        {
            if ((sbyte)mm.ReadByte(reg.a5 + w.reverb) < 0)
            {
                _opm_keyoff();
            }
            reg.D5_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.program2, (byte)reg.D5_B);

            _opm_echo_tone_change();
        }

        public void _opm_echo_tone_change()
        {
            mm.Write(reg.a5 + w.program, (byte)reg.D5_B);
            reg.D3_B = mm.ReadByte(reg.a5 + w.dev);
            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.TONE_PTR);
            if (reg.D0_L == 0) return;
            reg.a2 = reg.D0_L;
            reg.a2 += 6;
            reg.D4_W = mm.ReadUInt16(reg.a6 + dw.VOICENUM);
            reg.D1_B = mm.ReadByte(reg.a5 + w.bank);
            _opm_voice_ana_loop();
        }

        public void _opm_voice_ana_loop()
        {
            if (reg.D1_B - mm.ReadByte(reg.a2 + 2) == 0)
            {
                if (reg.D5_B - mm.ReadByte(reg.a2 + 3) == 0)
                {
                    _opm_voice_ana_set();
                    return;
                }
            }
            reg.D4_W--;
            if (reg.D4_W != 0)
            {
                reg.D0_W = mm.ReadUInt16(reg.a2);
                reg.a2 = reg.a2 + (UInt32)(Int16)reg.D0_W;
                _opm_voice_ana_loop();
            }
        }

        public void _opm_voice_ana_set()
        {
            reg.D1_L = 0x78;
            reg.D1_B += (UInt32)(sbyte)reg.D3_B;
            reg.D0_L = 0x7f;
            reg.D2_L = 4 - 1;
            do
            {
                mndrv._OPM_WRITE();
                reg.D1_B -= 8;
            } while (reg.D2_W-- != 0);

            reg.D1_L = 0xf8;
            reg.D1_B += (UInt32)(sbyte)reg.D3_B;
            reg.D0_L = 0xff;
            reg.D2_L = 4 - 1;
            do
            {
                mndrv._OPM_WRITE();
                reg.D1_B -= 8;
            } while (reg.D2_W-- != 0);

            reg.D1_B = mm.ReadByte(reg.a2 + 4);
            reg.D0_B = mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.D0_B &= 0xc0;
            reg.D0_B |= reg.D1_B;
            mm.Write(reg.a5 + w.pan_ampm, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.reverb_pan_work, (byte)reg.D0_B);
            reg.D1_L = 0x20;
            reg.D1_B += (UInt32)(sbyte)reg.D3_B;
            mndrv._OPM_WRITE();
            reg.D0_W &= 7;
            reg.a0 = ab.dummyAddress;// _opm_vol_con_pat;
            reg.D5_B = _opm_vol_con_pat[reg.D0_W];// mm.ReadByte(reg.a0 + reg.D0_W);

            if ((mm.ReadByte(reg.a5 + w.flag2) & 0x20) == 0)
            {
                reg.D0_B = mm.ReadByte(reg.a2 + 5);
                reg.D0_W <<= 3;
                mm.Write(reg.a5 + w.smask, (byte)reg.D0_B);
            }

            reg.D1_L = 0x40;
            reg.D1_B += (UInt32)(sbyte)reg.D3_B;
            reg.a2 += 6;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();
            reg.D1_W += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();
            reg.D1_W += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();
            reg.D1_W += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mndrv._OPM_WRITE();

            reg.D1_W += 8;
            mm.Write(reg.a5 + w.voiceptr, reg.a2);
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);

            reg.D0_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
                if ((sbyte)reg.D4_B < 0)
                {
                    reg.D4_L = 0x7f;
                }
            }
            else
            {
                reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
                if ((sbyte)reg.D4_B < 0)
                {
                    reg.D4_L = 0x00;
                }
            }
            reg.D4_B += mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0x7f;
            }

            mm.Write(reg.a5 + w.vol2, (byte)reg.D4_B);
            reg.D2_L = 4 - 1;
            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D5_B & 1;
                reg.D5_B >>= 1;
                if (f != 0)
                {
                    reg.D0_B += (UInt32)(sbyte)reg.D4_B;
                    if ((sbyte)reg.D0_B < 0)
                    {
                        reg.D0_L = 0x7f;
                    }
                }
                mndrv._OPM_WRITE();
                reg.D1_B += 8;
            } while (reg.D2_W-- != 0);

            reg.D2_L = 12 - 1;
            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                mndrv._OPM_WRITE();
                reg.D1_B += 8;
            } while (reg.D2_B-- != 0);

            reg.a4 = reg.a5 + w.tone_rr;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mndrv._OPM_WRITE();
            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mndrv._OPM_WRITE();
            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mndrv._OPM_WRITE();
            reg.D1_B += 8;
            reg.D0_B = mm.ReadByte(reg.a2++);
            mm.Write(reg.a4++, (byte)reg.D0_B);
            mndrv._OPM_WRITE();
        }

        public byte[] _opm_vol_con_pat = new byte[] {
            0x08,0x08,0x08,0x08,0x0C,0x0E,0x0E,0x0F
        };

        //─────────────────────────────────────
        //	volume 設定
        //
        //	FM COMMAND
        //	$F2 [ volume ]
        //			[$F2] + [$00～$7F]b    vコマンド
        //			[$F2] + [$80～$FF]b    @vコマンド（ビット7無効）
        public void _OPM_F2()
        {
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & 0xef));
            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_B &= 0x7f;
                _OPM_F2_v();
                return;
            }
            if (mm.ReadByte(reg.a5 + w.volmode) != 0)
            {
                mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x10));
            }
            if (reg.D4_B >= mm.ReadByte(reg.a5 + w.volcount))
            {
                reg.D4_B = mm.ReadByte(reg.a5 + w.volcount);
                reg.D4_B--;
            }
            mm.Write(reg.a5 + w.volume, (byte)reg.D4_B);
            reg.a2 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_v();
        }

        public void _OPM_F2_v()
        {
            mm.Write(reg.a5 + w.vol, (byte)reg.D4_B);
            _OPM_F2_softenv();
        }

        public void _OPM_F2_softenv()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
                if ((sbyte)reg.D4_B < 0)
                {
                    reg.D4_L = 0x7f;
                }
            }
            else
            {
                reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)reg.D0_B);
                if ((sbyte)reg.D4_B < 0)
                {
                    reg.D4_L = 0;
                }
            }
            reg.D4_B += mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0x7f;
            }
            _OPM_F2_set();
        }

        public void _OPM_F2_set()
        {
            reg.D0_L = 0;
            mm.Write(reg.a5 + w.vol2, (byte)reg.D4_B);
            reg.D1_L = mm.ReadUInt32(reg.a5 + w.voiceptr);
            if (reg.D1_L == 0) return;
            reg.a2 = reg.D1_L;

            reg.D0_L = 7;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.D3_B = _opm_vol_con_pat[reg.D0_W];// mm.ReadByte(_opm_vol_con_pat);

            reg.D1_L = 0x60;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);

            reg.D2_L = 4 - 1;
            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D3_B & 1;
                reg.D3_B >>= 1;
                if (f != 0)
                {
                    reg.D0_B += (UInt32)(sbyte)reg.D4_B;
                    if ((sbyte)reg.D0_B < 0)
                    {
                        reg.D0_L = 0x7f;
                    }
                    mndrv._OPM_WRITE();
                }
                reg.D1_W += 8;
            } while (reg.D2_W-- != 0);
        }

        public void _OPM_F2_init()
        {
            reg.D0_L = 0;
            mm.Write(reg.a5 + w.vol2, (byte)reg.D4_B);
            reg.D1_L = mm.ReadUInt32(reg.a5 + w.voiceptr);
            if (reg.D1_L == 0) return;

            reg.a2 = reg.D1_L;
            reg.D0_L = 7;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.a4 = ab.dummyAddress;// _opm_vol_con_pat;
            reg.D3_B = _opm_vol_con_pat[reg.D0_W];// mm.ReadByte(reg.a4 + reg.D0_W);

            reg.D1_L = 0x60;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);

            reg.D2_L = 4 - 1;
            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D3_B & 1;
                reg.D3_B >>= 1;
                if (f != 0)
                {
                    reg.D0_B += (UInt32)(sbyte)reg.D4_B;
                    if ((sbyte)reg.D0_B < 0)
                    {
                        reg.D0_L = 0x7f;
                    }
                }
                mndrv._OPM_WRITE();
                reg.D1_W += 8;
            } while (reg.D2_W-- != 0);
        }

        //─────────────────────────────────────
        //	pan 設定
        //			[$F4] + [DATA]b
        //
        public void _OPM_F4()
        {
            reg.D0_L = 0x3f;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.D0_B |= mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.pan_ampm, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.reverb_pan_work, (byte)reg.D0_B);
            reg.D1_L = 0x20;
            mndrv._OPM_WRITE4();
        }

        //─────────────────────────────────────
        //	volup
        //			[$F5] + [DATA]b
        //
        public void _OPM_F5()
        {
            if (mm.ReadByte(reg.a5 + w.volmode) == 0)
            {
                _OPM_F5_normal();
                return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x10) == 0)
            {
                _OPM_F5_normal();
                return;
            }

            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.volume);
            reg.D4_B += mm.ReadByte(reg.a1++);
            if (reg.D4_B >= mm.ReadByte(reg.a5 + w.volcount))
            {
                reg.D4_B = mm.ReadByte(reg.a5 + w.volcount);
                reg.D4_B--;
            }
            mm.Write(reg.a5 + w.volume, (byte)reg.D4_B);
            reg.a2 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_v();
        }

        public void _OPM_F5_normal()
        {
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)mm.ReadByte(reg.a1++));
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0;
            }
            _OPM_F2_v();
        }

        //─────────────────────────────────────
        //	voldown
        //			[$F6] + [DATA]b
        //
        public void _OPM_F6()
        {
            if (mm.ReadByte(reg.a5 + w.volmode) == 0)
            {
                _OPM_F6_normal();
                return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x10) == 0)
            {
                _OPM_F6_normal();
                return;
            }

            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.volume);
            reg.D4_B = (byte)((sbyte)reg.D4_B - (sbyte)mm.ReadByte(reg.a1++));
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0;
            }
            mm.Write(reg.a5 + w.volume, (byte)reg.D4_B);
            reg.a2 = reg.a5 + w.voltable;
            reg.D4_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D4_W);
            _OPM_F2_v();
        }

        public void _OPM_F6_normal()
        {
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D4_B += mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0x7f;
            }
            _OPM_F2_v();
        }

        //─────────────────────────────────────
        //	スロットマスク設定
        //			[$F8] + [data]b
        public void _OPM_F8()
        {
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x20));
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B >>= 1;
            mm.Write(reg.a5 + w.smask, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	y command
        //			[$FA] + [REG] + [DATA]
        public void _OPM_FA()
        {
            reg.D2_L = 0x12;
            reg.D1_B = mm.ReadByte(reg.a1++);
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D1_B - reg.D2_B != 0)
            {
                mndrv._OPM_WRITE();
                return;
            }

            mm.Write(reg.a6 + dw.TEMPO, (byte)reg.D0_B);
            mndrv._OPM_WRITE();
        }

        //─────────────────────────────────────
        //
        public void _OPM_FF()
        {
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfe));

            reg.D0_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a0 = reg.a6 + dw.TRACKWORKADR;

            L1:
            if ((mm.ReadByte(reg.a0 + w.flag2) & 0x01) == 0)
            {
                reg.a0 = reg.a0 + w._track_work_size;
                reg.D0_W--;
                if (reg.D0_W != 0) goto L1;

                mm.Write(reg.a6 + dw.LOOP_COUNTER, (UInt16)(mm.ReadUInt16(reg.a6 + dw.LOOP_COUNTER) + 1));
                reg.D1_W = mm.ReadUInt16(reg.a6 + dw.LOOP_COUNTER);
                if (reg.D1_W - (-1) == 0)
                {
                    reg.D1_L = 0;
                    mm.Write(reg.a6 + dw.LOOP_COUNTER, (UInt16)reg.D1_W);
                }
                reg.D0_L = 1;
                mndrv.SUBEVENT();

                reg.D0_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
                reg.a0 = reg.a6 + dw.TRACKWORKADR;
                do
                {
                    if ((sbyte)mm.ReadByte(reg.a0 + w.flag) < 0)
                    {
                        mm.Write(reg.a0 + w.flag2, (byte)(mm.ReadByte(reg.a0 + w.flag2) | 0x01));
                    }
                    reg.a0 = reg.a0 + w._track_work_size;
                    reg.D0_W--;
                } while (reg.D0_W != 0);
            }

            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            if (reg.D0_W != 0)
            {
                if (reg.D0_W - 0xffff != 0)
                {
                    reg.a1 = reg.a1 + (UInt32)(Int16)reg.D0_W;
                    return;
                }
            }
            else
            {
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x7f));
                mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfe));
                mm.Write(reg.a5 + w.lfo, 0x00);
                mm.Write(reg.a5 + w.weffect, 0x00);
                _opm_keyoff();
                comcmds._all_end_check();
                return;
            }
            reg.a1 = mm.ReadUInt32(reg.a5 + w.loop);
        }

        //─────────────────────────────────────
        //
        public void _ch_opm_lfo_job()
        {
            comwave._ch_effect();
            //_ch_opm_lfo:
            if ((mm.ReadByte(reg.a5 + w.effect) & 0x20) != 0)
            {
                reg.a4 = reg.a5 + w.ww_pattern1;
                _ch_opm_ww();
            }
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            if (reg.D0_B == 0) return;
            mm.Write(reg.a5 + w.addkeycode, (UInt16)0);
            mm.Write(reg.a5 + w.addvolume, (UInt16)0);

            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f != 0)
            {
                _ch_opm_HLFO();
            }

            //_ch_opm_p1:
            reg.D1_L = 7;
            reg.D1_B &= reg.D0_B;
            if (reg.D1_B != 0)
            {
                uint sp = reg.D0_W;
                reg.D1_W += (UInt32)(Int16)reg.D1_W;
                switch (reg.D1_W / 2)
                {
                    case 1:
                        _ch_opm_plfo_1();
                        break;
                    case 2:
                        _ch_opm_plfo_2();
                        break;
                    case 3:
                        _ch_opm_plfo_3();
                        break;
                    case 4:
                        _ch_opm_plfo_4();
                        break;
                    case 5:
                        _ch_opm_plfo_5();
                        break;
                    case 6:
                        _ch_opm_plfo_6();
                        break;
                    case 7:
                        _ch_opm_plfo_7();
                        break;
                }
                reg.D0_W = sp;
            }

            reg.D0_B >>= 3;
            reg.D0_W &= 7;
            if (reg.D0_W != 0)
            {
                reg.D0_W += (UInt32)(Int16)reg.D0_W;
                switch (reg.D0_W/2)
                {
                    case 1:
                        _ch_opm_alfo_1();
                        break;
                    case 2:
                        _ch_opm_alfo_2();
                        break;
                    case 3:
                        _ch_opm_alfo_3();
                        break;
                    case 4:
                        _ch_opm_alfo_4();
                        break;
                    case 5:
                        _ch_opm_alfo_5();
                        break;
                    case 6:
                        _ch_opm_alfo_6();
                        break;
                    case 7:
                        _ch_opm_alfo_7();
                        break;
                }
            }

            //_ch_opm_lfo_end:
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.addvolume);
            if (reg.D0_W - mm.ReadUInt16(reg.a5 + w.addvolume2) != 0)
            {
                mm.Write(reg.a5 + w.addvolume2, (UInt16)reg.D0_W);
                _opm_v_calc();
            }

            //_ch_opm_lfo_a_exit:
            reg.D2_W = mm.ReadUInt16(reg.a5 + w.keycode3);
            reg.D1_W = mm.ReadUInt16(reg.a5 + w.addkeycode);
            if (reg.D1_W - mm.ReadUInt16(reg.a5 + w.addkeycode2) == 0) return;
            mm.Write(reg.a5 + w.addkeycode2, (UInt16)reg.D1_W);
            if ((Int16)reg.D1_W >= 0)
            {
                reg.D2_W += (UInt32)(Int16)reg.D1_W;
                if (reg.D2_W >= 0x2000)
                {
                    reg.D2_W = mm.ReadUInt16(0x1fff);
                }
            }
            else
            {
                reg.D2_W += (UInt32)(Int16)reg.D1_W;
                if ((Int16)reg.D2_W < 0)
                {
                    reg.D2_L = 0;
                }
            }
            _set_kckf();
        }

        //;─────────────────────────────────────
        public void _ch_opm_plfo_1()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_opm_p_common();
        }

        public void _ch_opm_plfo_2()
        {
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_opm_p_common();
        }

        public void _ch_opm_plfo_3()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_opm_p_common();
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_opm_p_common();
        }

        public void _ch_opm_plfo_4()
        {
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_opm_p_common();
        }

        public void _ch_opm_plfo_5()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_opm_p_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_opm_p_common();
        }

        public void _ch_opm_plfo_6()
        {
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_opm_p_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_opm_p_common();
        }

        public void _ch_opm_plfo_7()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_opm_p_common();
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_opm_p_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_opm_p_common();
        }

        //─────────────────────────────────────
        public void _ch_opm_alfo_1()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_opm_a_common();
        }

        public void _ch_opm_alfo_2()
        {
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_opm_a_common();
        }

        public void _ch_opm_alfo_3()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_opm_a_common();
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_opm_a_common();
        }

        public void _ch_opm_alfo_4()
        {
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_opm_a_common();
        }

        public void _ch_opm_alfo_5()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_opm_a_common();
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_opm_a_common();
        }

        public void _ch_opm_alfo_6()
        {
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_opm_a_common();
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_opm_a_common();
        }

        public void _ch_opm_alfo_7()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_opm_a_common();
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_opm_a_common();
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_opm_a_common();
        }

        //─────────────────────────────────────
        //	HARD WARE LFO delay & LW type modulation
        //
        public void _ch_opm_HLFO()
        {
            reg.D5_W = reg.D0_W;
            reg.D0_L = 0xc;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.flag2);
            if (reg.D0_B == 0)
            {
                reg.D0_W = reg.D5_W;
                return;
            }

            reg.a4 = reg.a5 + w.v_pattern4;
            reg.D4_W = mm.ReadUInt16(reg.a4 + w_l.flag);
            if ((Int16)reg.D4_W >= 0)
            {
                _ch_opm_HLFO_exec();
                return;
            }
            uint f = reg.D4_B & 1;
            reg.D4_B >>= 1;
            if (f != 0)
            {
                _ch_opm_h_keyon_only();
                return;
            }

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0)
            {
                _ch_opm_HLFO_exec();
            }
            reg.D0_W = reg.D5_W;
        }

        public void _ch_opm_h_keyon_only()
        {
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0)
            {
                _ch_opm_HLFO_exec();
            }
            reg.D0_W = reg.D5_W;
        }

        public void _ch_opm_HLFO_exec()
        {
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0)
            {
                reg.D0_W = reg.D5_W;
                return;
            }

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            if ((reg.D0_L & 8) != 0)
            {
                _ch_opm_HLFO_lw();
                return;
            }

            //	pea	_ch_opm_HLFO_exit(pc)
            reg.D0_B = mm.ReadByte(reg.a5 + w.fbcon);
            reg.D1_L = 0x38;
            mndrv._OPM_WRITE4();
            reg.D0_W = reg.D5_W;
        }

        public void _ch_opm_HLFO_lw()
        {
            reg.D0_B = mm.ReadByte(reg.a4 + w_l.henka_work);
            reg.D0_B += 0x11;
            if ((sbyte)reg.D0_B >= 0)
            {
                mm.Write(reg.a4 + w_l.henka_work, (byte)reg.D0_B);
                reg.D1_B = reg.D0_B;
                reg.D0_B &= 0x70;
                reg.D1_B >>= 1;
                reg.D1_B &= 0x3;
                reg.D0_B |= reg.D1_B;
                reg.D1_L = 0x38;
                mndrv._OPM_WRITE4();
            }

            reg.D0_W = reg.D5_W;
        }

        //─────────────────────────────────────
        public void _ch_opm_a_common()
        {
            reg.D0_L = 1;
            reg.D1_B = mm.ReadByte(reg.a4 + w_l.pattern);
            if ((sbyte)reg.D1_B < 0)
            {
                comwave._com_wavememory();
                mm.Write(reg.a5 + w.addvolume, (UInt16)reg.D0_W);
                return;
            }

            reg.D4_W = mm.ReadUInt16(reg.a4 + w_l.flag);
            if ((Int16)reg.D4_W >= 0)
            {
                _ch_opm_v_com_exec();
                return;
            }
            uint f = reg.D4_B & 1;
            reg.D4_B >>= 1;
            if (f != 0)
            {
                _ch_opm_v_keyon_only();
                return;
            }

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0)
            {
                _ch_opm_v_com_exec();
            }
        }

        public void _ch_opm_v_keyon_only()
        {
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0)
            {
                _ch_opm_v_com_exec();
            }
        }

        public void _ch_opm_v_com_exec()
        {
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_W / 2)
            {
                case 1:
                    comlfo._com_lfo_saw();
                    break;
                case 2:
                    comlfo._com_lfo_portament();
                    break;
                case 3:
                    comlfo._com_lfo_triangle();
                    break;
            }
            mm.Write(reg.a5 + w.addvolume, (UInt16)reg.D1_W);
        }

        //─────────────────────────────────────
        public void _opm_v_calc()
        {
            reg.D3_B = mm.ReadByte(reg.a3 + w_w.slot);
            if ((sbyte)reg.D3_B < 0)
            {
                _opm_v_calc_slot();
                return;
            }

            if ((Int16)reg.D0_W < 0)
            {
                _ch_opm_lfo_a_minus();
                return;
            }

            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D4_W += (UInt32)(Int16)reg.D0_W;
            if (reg.D4_W >= 0x80)
            {
                reg.D4_L = 0x7f;
            }
            _OPM_F2_lfo();
        }

        public void _ch_opm_lfo_a_minus()
        {
            reg.D4_L = 0;
            reg.D4_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D4_W += (UInt32)(Int16)reg.D0_W;
            if ((Int16)reg.D4_W < 0)
            {
                reg.D4_L = 0x00;
            }
            _OPM_F2_lfo();
        }

        //─────────────────────────────────────
        public void _OPM_F2_lfo()
        {
            reg.D4_B += mm.ReadByte(reg.a6 + dw.MASTER_VOL_FM);
            if ((sbyte)reg.D4_B < 0)
            {
                reg.D4_L = 0x7f;
            }
            mm.Write(reg.a5 + w.vol2, (byte)reg.D4_B);
            reg.D0_L = mm.ReadUInt32(reg.a5 + w.voiceptr);
            if (reg.D0_L == 0) return;

            reg.D1_L = 7;
            reg.D1_B &= mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.a2 = ab.dummyAddress;// _opm_vol_con_pat;
            reg.D5_B = _opm_vol_con_pat[reg.D1_W];// mm.ReadByte(reg.a2 + reg.D1_W);
            reg.a2 = reg.D0_L;
            reg.D1_L = 0x60;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            reg.D2_L = 4 - 1;

            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D5_B & 1;
                reg.D5_B >>= 1;
                if (f != 0)
                {
                    reg.D0_B += (UInt32)(sbyte)reg.D4_B;
                    if ((sbyte)reg.D0_B < 0)
                    {
                        reg.D0_L = 0x7f;
                    }
                    mndrv._OPM_WRITE();
                }
                reg.D1_W += 8;
            } while (reg.D2_W-- != 0);
        }

        //─────────────────────────────────────
        public void _opm_v_calc_slot()
        {
            reg.D4_W = reg.D0_W;
            reg.D0_L = mm.ReadUInt32(reg.a5 + w.voiceptr);
            if (reg.D0_L == 0) return;

            reg.a2 = reg.D0_L;
            reg.D1_L = 0x60;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            reg.D2_L = 4 - 1;
            reg.D0_L = 0;

            if ((Int16)reg.D4_W < 0)
            {
                _opm_v_slot_minus(); return;
            }
            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D3_B & 1;
                reg.D3_B >>= 1;
                if (f != 0)
                {
                    reg.D0_W += (UInt32)(Int16)reg.D4_W;
                    if ((Int16)reg.D0_W < 0)
                    {
                        reg.D0_L = 0x7f;
                    }
                    mndrv._OPM_WRITE();
                }
                reg.D1_W += 8;
            } while (reg.D2_W-- != 0);
        }

        public void _opm_v_slot_minus()
        {
            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D3_B & 1;
                reg.D3_B >>= 1;
                if (f != 0)
                {
                    reg.D0_W += (UInt32)(Int16)reg.D4_W;
                    if ((Int16)reg.D0_W < 0)
                    {
                        reg.D0_L = 0x0;
                    }
                    mndrv._OPM_WRITE();
                }
                reg.D1_W += 8;
            } while (reg.D2_W-- != 0);
        }

        //─────────────────────────────────────
        public void _ch_opm_p_common()
        {
            reg.D0_L = 1;
            reg.D1_B = mm.ReadByte(reg.a4 + w_l.pattern);
            if ((sbyte)reg.D1_B < 0)
            {
                comwave._com_wavememory();
                mm.Write(reg.a5 + w.addkeycode, (UInt16)(mm.ReadUInt16(reg.a5 + w.addkeycode) + (Int16)reg.D0_W));
                return;
            }
            reg.D4_W = mm.ReadUInt16(reg.a4 + w_l.flag);
            if ((Int16)reg.D4_W >= 0)
            {
                _ch_opm_p_com_exec();
                return;
            }
            uint f = reg.D4_B & 1;
            reg.D4_B >>= 1;
            if (f != 0)
            {
                _ch_opm_p_keyon_only();
                return;
            }

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0)
            {
                _ch_opm_p_com_exec();
            }
        }

        public void _ch_opm_p_keyon_only()
        {
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0)
            {
                _ch_opm_p_com_exec();
            }
        }

        public void _ch_opm_p_com_exec()
        {
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;

            if ((sbyte)mm.ReadByte(reg.a6 + dw.LFO_FLAG) >= 0)
            {
                switch (reg.D0_W / 2)
                {
                    case 1:
                        comlfo._com_lfo_saw();
                        break;
                    case 2:
                        comlfo._com_lfo_portament();
                        break;
                    case 3:
                        comlfo._com_lfo_triangle();
                        break;
                    case 4:
                        comlfo._com_lfo_portament();
                        break;
                    case 5:
                        comlfo._com_lfo_triangle();
                        break;
                    case 6:
                        comlfo._com_lfo_triangle();
                        break;
                    case 7:
                        comlfo._com_lfo_oneshot();
                        break;
                    case 8:
                        comlfo._com_lfo_oneshot();
                        break;
                }
                mm.Write(reg.a5 + w.addkeycode, (UInt16)(mm.ReadUInt16(reg.a5 + w.addkeycode) + (Int16)reg.D1_W));
                return;
            }
            reg.a0 = ab.dummyAddress;// _pitch_extend;
            switch (reg.D0_W)
            {
                case 2:
                    comlfo._com_lfo_saw();
                    break;
                case 4:
                    comlfo._com_lfo_portament();
                    break;
                case 6:
                    comlfo._com_lfo_triangle();
                    break;
                case 8:
                    comlfo._com_lfo_oneshot();
                    break;
                case 10:
                    comlfo._com_lfo_square();
                    break;
                case 12:
                    comlfo._com_lfo_randome();
                    break;
            }
            mm.Write(reg.a5 + w.addkeycode, (UInt16)(mm.ReadUInt16(reg.a5 + w.addkeycode) + (Int16)reg.D1_W));
        }

        //─────────────────────────────────────
        public void _ch_opm_mml_job()
        {
            comanalyze._track_analyze();

            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x04) == 0)
            {
                reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
                if ((sbyte)reg.D0_B >= 0) return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag2) & 0x2) != 0)
            {
                _ch_opm_porta();
                return;
            }
            _ch_opm_bend();
        }

        //─────────────────────────────────────
        //	pitch bend
        //
        public void _ch_opm_bend()
        {
            reg.a4 = reg.a5 + w.p_pattern4;

            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x04) != 0)
            {
                _ch_opm_mx_bend();
                return;
            }

            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0)
            {
                //_ch_opm_bend_no_job();
                return;
            }

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D2_W = mm.ReadUInt16(reg.a5 + w.keycode3);
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka);
            if ((Int16)reg.D1_W >= 0)
            {
                _ch_opm_bend_plus();
                return;
            }

            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D1_W));
            reg.D2_W += (UInt32)(Int16)reg.D1_W;
            if ((Int16)reg.D2_W < 0)
            {
                reg.D2_W = 0x1fff;
            }
            //_ch_opm_bend_common_minus:
            if (reg.D2_W < mm.ReadUInt16(reg.a4 + w_l.mokuhyou))
            {
                _ch_opm_bend_end();
                return;
            }
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        public void _ch_opm_bend_plus()
        {
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D1_W));
            reg.D2_W += (UInt32)(Int16)reg.D1_W;
            if ((Int16)reg.D2_W < 0)
            {
                reg.D2_W = 0;
            }
            //_ch_opm_bend_common_plus:
            if (reg.D2_W >= mm.ReadUInt16(reg.a4 + w_l.mokuhyou))
            {
                _ch_opm_bend_end();
                return;
            }
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        public void _ch_opm_bend_end()
        {
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));

            reg.D2_L = 0;
            reg.D2_B = mm.ReadByte(reg.a5 + w.key2);
            mm.Write(reg.a5 + w.key, (byte)reg.D2_B);
            reg.D2_W <<= 6;
            reg.D2_W += 5;
            reg.D2_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D2_W < 0)
            {
                reg.D2_L = 00;
            }
            mm.Write(reg.a5 + w.keycode2, (UInt16)reg.D2_W);
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        //─────────────────────────────────────
        public void _ch_opm_mx_bend()
        {
            mm.Write(reg.a4 + w_l.count, (byte)(mm.ReadByte(reg.a4 + w_l.count) - 1));
            if(mm.ReadByte(reg.a4 + w_l.count) == 0)
            {
                _ch_opm_mx_bend_end();
                return;
            }
            reg.a3 = reg.a5 + w.wp_pattern4;

            if ((mm.ReadByte(reg.a5 + w.flag2) & 0x10) != 0)
            {
                _ch_opm_mx_lw_port();
                return;
            }

            reg.D2_L = mm.ReadUInt32(reg.a3 + w_w.start);
            reg.D1_W = mm.ReadUInt16(reg.a3 + w_w.start);
            mm.Write(reg.a3 + w_w.ko_start, (UInt32)(mm.ReadUInt32(reg.a3 + w_w.ko_start) + (Int16)reg.D2_L));

            reg.D2_W = mm.ReadUInt16(reg.a3 + w_w.ko_start);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)reg.D2_W);
            reg.D2_W += mm.ReadUInt16(reg.a5 + w.keycode_s4);
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        public void _ch_opm_mx_bend_end()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & 0xfb));
            mm.Write(reg.a5 + w.key, 0xff);
        }

        public void _ch_opm_mx_lw_port()
        {
            reg.D3_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
            reg.D0_L = mm.ReadUInt32(reg.a3 + w_w.loop_start);
            reg.D1_L = reg.D0_L;
            reg.D1_L = (UInt32)((Int32)reg.D1_L >> 2);
            reg.D2_L = 0;
            reg.D4_L = 7;
            do
            {
                reg.D1_L = (UInt32)((Int32)reg.D1_L >> 1);
                uint cf = reg.D3_B & 0x80;
                reg.D3_B <<= 1;
                if (cf!=0)
                {
                    reg.D2_L += (UInt32)(Int32)reg.D1_L;
                }
            } while (reg.D4_W-- != 0);

            mm.Write(reg.a3 + w_w.ko_start, (UInt32)(mm.ReadUInt32(reg.a3 + w_w.ko_start) + (Int16)reg.D2_L));
            reg.D0_L = (UInt32)((Int32)reg.D0_L - (Int32)reg.D2_L);
            mm.Write(reg.a3 + w_w.loop_start, reg.D0_L);
            reg.D2_W = mm.ReadUInt16(reg.a3 + w_w.ko_start);
            reg.D2_W += mm.ReadUInt16(reg.a5 + w.keycode_s4);
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        //─────────────────────────────────────
        //	portament
        //
        public void _ch_opm_porta()
        {
            reg.a4 = reg.a5 + w.p_pattern4;

            if ((mm.ReadByte(reg.a5 + w.flag2) & 0x10) != 0)
            {
                _ch_opm_lw_port();
                return;
            }
            reg.D2_W = mm.ReadUInt16(reg.a5 + w.keycode3);
            reg.D1_W = mm.ReadUInt16(reg.a4 + w_l.henka);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D1_W));
            reg.D2_W += (UInt32)(Int16)reg.D1_W;
            if (mm.ReadUInt16(reg.a4 + w_l.henka_work) == 0)
            {
                _ch_opm_porta_common();
                return;
            }
            if ((Int16)mm.ReadUInt16(reg.a4 + w_l.henka_work) < 0)
            {
                _ch_opm_porta_minus();
                return;
            }

            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.henka_work) - 1));
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + 1));
            reg.D2_W++;
            _ch_opm_porta_common();
        }

        public void _ch_opm_porta_minus()
        {
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.henka_work) + 1));
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - 1));
            reg.D2_W = (UInt32)((Int16)reg.D2_W - 1);
            _ch_opm_porta_common();
        }

        public void _ch_opm_porta_common()
        {
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        public void _ch_opm_lw_port()
        {
            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D2_L = 0;
            reg.D0_W = mm.ReadUInt16(reg.a4 + w_l.mokuhyou);
            bool cf = (Int16)reg.D0_W - (Int16)mm.ReadUInt16(reg.a5 + w.keycode3) < 0;
            reg.D0_W =(UInt16)((Int16)reg.D0_W- (Int16)mm.ReadUInt16(reg.a5 + w.keycode3));
            if (reg.D0_W == 0)
            {
                _ch_opm_lw_porta_end();
                return;
            }
            if (cf)
            {
                reg.D2_L = 1;
                reg.D0_W = (UInt16)(-(Int16)reg.D0_W);
            }
            reg.D1_B = mm.ReadByte(reg.a4 + w_l.lfo_sp);
            reg.D0_W *= reg.D1_W;
            reg.D0_L >>= 8;
            if (reg.D0_L == 0)
            {
                reg.D0_L = 1;
            }
            if (reg.D2_B != 0)
            {
                reg.D0_W = (UInt16)(-(Int16)reg.D0_W);
            }
            reg.D2_W = mm.ReadUInt16(reg.a5 + w.keycode3);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D0_W));
            reg.D2_W = (UInt32)((Int16)reg.D2_W + (Int16)reg.D0_W);
            mm.Write(reg.a5 + w.keycode3, (UInt16)reg.D2_W);
            _set_kckf();
        }

        public void _ch_opm_lw_porta_end()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfd));
        }

        //─────────────────────────────────────
        //	わうわう
        //
        public void _ch_opm_ww()
        {
            mm.Write(reg.a4 + w_ww.delay_work, (byte)(mm.ReadByte(reg.a4 + w_ww.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_ww.delay_work) != 0) return;

            mm.Write(reg.a4 + w_ww.delay_work, mm.ReadByte(reg.a4 + w_ww.speed));
            reg.D4_B = mm.ReadByte(reg.a4 + w_ww.work);
            reg.D4_B += mm.ReadByte(reg.a4 + w_ww.rate_work);
            mm.Write(reg.a4 + w_ww.work, (byte)reg.D4_B);

            reg.a2 = mm.ReadUInt32(reg.a5 + w.voiceptr);
            reg.D1_L = 0x60;
            reg.D1_B += mm.ReadByte(reg.a5 + w.dev);
            reg.D2_L = 4 - 1;
            reg.D3_B = mm.ReadByte(reg.a4 + w_ww.slot);

            do
            {
                reg.D0_B = mm.ReadByte(reg.a2++);
                uint f = reg.D3_B & 1;
                reg.D3_B >>= 1;
                if (f != 0)
                {
                    reg.D0_B += (UInt32)(sbyte)reg.D4_B;
                    if ((sbyte)reg.D0_B < 0)
                    {
                        reg.D0_L = 0x7f;
                    }
                    mndrv._OPM_WRITE();
                }
                reg.D1_W += 8;
            } while (reg.D2_W-- != 0);

            mm.Write(reg.a4 + w_ww.depth_work, (byte)(mm.ReadByte(reg.a4 + w_ww.depth_work) - 1));
            if (mm.ReadByte(reg.a4 + w_ww.depth_work) != 0) return;
            mm.Write(reg.a4 + w_ww.depth_work, mm.ReadByte(reg.a4 + w_ww.depth));
            mm.Write(reg.a4 + w_ww.rate_work, (byte)(-(sbyte)mm.ReadByte(reg.a4 + w_ww.rate_work)));
        }

        //─────────────────────────────────────
        public void _ch_opm_softenv_job()
        {
            comlfo._soft_env();
            _OPM_F2_softenv();
        }

        //─────────────────────────────────────
        //	effect execute
        //
        public void _opm_effect_ycommand()
        {
            reg.D1_B = (byte)(reg.D0_W >> 8);
            mndrv._OPM_WRITE();
        }

        public void _opm_effect_tone()
        {
            mm.Write(reg.a5 + w.bank, (byte)(reg.D0_W >> 8));
            reg.D5_B = reg.D0_B;
            _opm_echo_tone_change();
        }

        public void _opm_effect_pan()
        {
            reg.D0_W &= 3;
            reg.D0_B = _opm_effect_pan_table[reg.D0_W];
            reg.D1_L = 0x3f;
            reg.D1_B &= mm.ReadByte(reg.a5 + w.pan_ampm);
            reg.D0_B |= reg.D1_B;
            mm.Write(reg.a5 + w.pan_ampm, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.reverb_pan_work, (byte)reg.D0_B);
            reg.D1_L = 0x20;
            mndrv._OPM_WRITE4();
        }

        public byte[] _opm_effect_pan_table = new byte[] {
            0x00,0x40,0x80,0xC0
        };

    }
}