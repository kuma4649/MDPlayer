using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class devpsg
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public mndrv mndrv;
        public comanalyze comanalyze;
        public comcmds comcmds;
        public comlfo comlfo;
        public comwave comwave;
        public devopn devopn;
        public ab ab;

        //
        //	part of YM2608 - PSG
        //

        //─────────────────────────────────────
        public void _psg_note_set()
        {
            mm.Write(reg.a5 + w.key, (byte)reg.D0_B);
            //    pea _psg_env_keyon(pc)
            _psg_freq();
            comlfo._init_lfo();
            _init_lfo_psg();
            _psg_env_keyon();
        }

        //─────────────────────────────────────
        public void _psg_freq()
        {
            reg.D1_L = 0;
            reg.D2_L = 12;

            while (reg.D0_B >= reg.D2_B)
            {
                reg.D0_B -= (UInt32)(sbyte)reg.D2_B;
                reg.D1_B++;
            }

            mm.Write(reg.a5 + w.octave, (byte)reg.D1_B);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D0_W = _psg_table[reg.D0_W / 2];
            mm.Write(reg.a5 + w.makotune, (UInt16)reg.D0_W);
            reg.D2_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG2);
            uint f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f == 0)
            {
                _set_psg_();
                return;
            }
            f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f == 0)
            {
                _set_psg_mako();
                return;
            }

            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            mm.Write(reg.a5 + w.freqwork, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D0_W < 0)
            {
                reg.D0_L = 0;
            }

            mm.Write(reg.a5 + w.keycode2, (UInt16)reg.D0_W);
            _set_psg_bend();
        }

        public void _set_psg_mako()
        {
            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            mm.Write(reg.a5 + w.freqwork, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            reg.D1_W = mm.ReadUInt16(reg.a5 + w.detune);
            reg.D1_W = (UInt16)(-(Int16)reg.D1_W);

            reg.D1_W = (UInt16)((Int16)reg.D1_W >> 2);
            reg.D0_W += (UInt32)(Int16)reg.D1_W;
            mm.Write(reg.a5 + w.keycode2, (UInt16)reg.D0_W);
            _set_psg_bend();
        }

        public void _set_psg_()
        {
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D0_W < 0)
            {
                reg.D0_L = 0;
            }

            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            mm.Write(reg.a5 + w.freqwork, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            mm.Write(reg.a5 + w.keycode2, (UInt16)reg.D0_W);
            _set_psg_bend();
        }

        public void _set_psg_bend()
        {
            mm.Write(reg.a5 + w.keycode, (UInt16)reg.D0_W);
            reg.D1_B = mm.ReadByte(reg.a5 + w.dev);
            reg.D1_B += (UInt32)(sbyte)reg.D1_B;

            if (reg.D0_W - mm.ReadUInt16(reg.a5 + w.tune) == 0) return;

            mm.Write(reg.a5 + w.tune, (UInt16)reg.D0_W);
            UInt16 sp = (UInt16)reg.D0_W;
            mndrv._OPN_WRITE2();
            reg.D0_B = (byte)(sp >> 8);
            reg.D1_B++;
            mndrv._OPN_WRITE2();
        }

        //─────────────────────────────────────
        public UInt16[] _psg_table = new UInt16[] {
             0x0EE8,0x0E12,0x0D48,0x0C89
            ,0x0BD5,0x0B2B,0x0A8A,0x09F3
            ,0x0964,0x08DD,0x085E,0x07E6
        };

        //─────────────────────────────────────
        public void _psg_env_keyon()
        {
            if ((sbyte)mm.ReadByte(reg.a5 + w.flag2) < 0) return;
            comwave._wave_init_kon();

            mm.Write(reg.a5 + w.reverb_time_work, 0);
            mm.Write(reg.a5 + w.revexec, 0);

            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x20));
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x40) == 0)
            {
                if ((mm.ReadByte(reg.a5 + w.flag) & 0x40) != 0) return;
            }

            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xfb));

            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sw);
            if ((sbyte)reg.D0_B < 0)
            {
                _psg_env_keyon_();
                return;
            }

            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D1_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D1_B >= 0)
            {
                _psg_env_keyon1();
                return;
            }
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D0_B >= 0)
            {
                _psg_env_keyon2();
                return;
            }
            reg.D0_L = 0;
            _psg_env_keyon2();
        }

        public void _psg_env_keyon1()
        {
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            if (reg.D0_B >= 16)
            {
                reg.D0_L = 15;
            }
            _psg_env_keyon2();
        }

        public void _psg_env_keyon2()
        {
            reg.D0_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }

            reg.D0_W &= 0xf;
            reg.a2 = reg.a5 + w.voltable;
            reg.D0_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D0_W);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D0_B);
            reg.D1_L = 8;
            mndrv._OPN_WRITE4();
        }

        public void _psg_env_keyon_()
        {
            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f != 0)
            {
                mm.Write(reg.a5 + w.e_p, 5);
                return;
            }

            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            reg.D0_B &= 0xf0;
            mm.Write(reg.a5 + w.e_p, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_dl, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W));
            reg.D1_L = 0x7f;
            reg.D1_B &= mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 1);
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            mm.Write(reg.a5 + w.e_lm, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 2));
            mm.Write(reg.a5 + w.e_ini, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 3));
        }

        //─────────────────────────────────────
        public void _psg_env_keyoff()
        {
            if (mm.ReadByte(reg.a5 + w.rct) == 0)
            {
                _psg_keyoff();
                return;
            }

            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x98));
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            comwave._wave_init_kof();
        }

        public void _psg_keyoff()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            comwave._wave_init_kof();
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x98));

            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sw);
            if ((sbyte)reg.D0_B >= 0)
            {
                reg.D0_L = 0;
                reg.D1_L = 8;
                mndrv._OPN_WRITE4();
                return;
            }

            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f != 0)
            {
                mm.Write(reg.a5 + w.e_p, 4);
                return;
            }

            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            reg.D0_B &= 0xf0;
            reg.D0_B += 0xc;
            mm.Write(reg.a5 + w.e_p, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_dl, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 0));
            reg.D1_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 1);
            reg.D1_B &= 0x7f;
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            mm.Write(reg.a5 + w.e_lm, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 2));
        }

        //─────────────────────────────────────
        //
        public void _psg_echo()
        {
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0) return;

            mm.Write(reg.a5 + w.revexec, 0xff);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x98));
            mm.Write(reg.a5 + w.reverb_time_work, (byte)(mm.ReadByte(reg.a5 + w.reverb_time)));

            reg.D5_L = 7;
            reg.D5_B &= mm.ReadByte(reg.a5 + w.reverb);
            reg.D5_W++;
            reg.D5_W += (UInt32)(Int16)reg.D5_W;
            switch (reg.D5_W)
            {
                case 2:
                case 4:
                case 6:
                case 8:
                    _psg_echo_volume();
                    //_psg_echo_volume_pan();
                    //_psg_echo_volume_tone();
                    //_psg_echo_volume_pan_tone();
                    break;
                case 10:
                    _psg_echo_volume_();
                    break;
            }
        }

        //─────────────────────────────────────
        public void _psg_echo_volume()
        {
            //_psg_echo_volume_pan
            //_psg_echo_volume_tone
            //_psg_echo_volume_pan_tone
            _psg_echo_common_v();
        }

        public void _psg_echo_volume_()
        {
            _psg_echo_volume_v();
        }

        //─────────────────────────────────────
        // v通常
        public void _psg_echo_common_v()
        {
            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x08) != 0)
            {
                _psg_echo_direct_v();
                return;
            }

            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D1_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            if ((sbyte)reg.D1_B >= 0)
            {
                _psg_echo_plus();
                return;
            }

            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            _psg_f2_softenv();
        }

        public void _psg_echo_plus()
        {
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            if (reg.D0_B >= 0xf)
            {
                reg.D0_L = 0xf;
            }
            _psg_f2_softenv();
        }

        //─────────────────────────────────────
        // v微調整
        //
        public void _psg_echo_volume_v()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D1_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D1_B < 0)
            {
                reg.D0_B += (UInt32)(sbyte)reg.D1_B;
                if ((sbyte)reg.D0_B < 0)
                {
                    reg.D0_L = 0;
                }
            }
            else
            {
                reg.D0_B += (UInt32)(sbyte)reg.D1_B;
                if (reg.D0_B >= 16) 
                {
                    reg.D0_L = 15;
                }
            }

            reg.D0_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }

            reg.D1_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            if ((sbyte)reg.D1_B < 0)
            {
                _psg_echo_vol_plus();
                return;
            }
            reg.D0_B -= (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            _psg_echo_vol_1();
        }

        public void _psg_echo_vol_plus()
        {
            reg.D0_B -= (UInt32)(sbyte)reg.D1_B;
            if (reg.D0_B >= 0xf)
            {
                reg.D0_L = 0xf;
            }
            _psg_echo_vol_1();
        }

        public void _psg_echo_vol_1()
        {
            reg.D0_B >>= 1;
            _psg_lfo();
        }

        //─────────────────────────────────────
        // v直接
        //
        public void _psg_echo_direct_v()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.reverb_vol);
            _psg_f2_softenv();
        }

        //─────────────────────────────────────
        public void _psg_env()
        {
            if ((sbyte)mm.ReadByte(reg.a5 + w.e_sw) >= 0) return;
            mm.Write(reg.a5 + w.e_sp, (byte)(mm.ReadByte(reg.a5 + w.e_sp) - 1));
            if (mm.ReadByte(reg.a5 + w.e_sp) == 0)
            {
                _psg_env_next();
            }
        }

        public void _psg_env_next()
        {
            reg.a4 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            reg.D1_B = mm.ReadByte(reg.a4 + (UInt32)(Int16)reg.D0_W + 1);
            if ((sbyte)reg.D1_B < 0)
            {
                _psg_env_minus();
                return;
            }

            reg.D1_B &= 0x7f;
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_ini);
            bool f = reg.cryADD((byte)reg.D0_B, mm.ReadByte(reg.a5 + w.e_dl));
            reg.D0_B += mm.ReadByte(reg.a5 + w.e_dl);
            if (f)
            {
                _psg_env_common();
                return;
            }
            if (reg.D0_B >= mm.ReadByte(reg.a5 + w.e_lm))
            {
                _psg_env_common();
                return;
            }
            _psg_volume_set();
        }

        public void _psg_env_minus()
        {
            reg.D1_B &= 0x7f;
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_ini);
            bool cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_dl);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_dl);
            if (cf)
            {
                _psg_env_common();
                return;
            }
            if (reg.D0_B <= mm.ReadByte(reg.a5 + w.e_lm))
            {
                _psg_env_common();
                return;
            }
            _psg_volume_set();
        }

        public void _psg_env_common()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_lm);
            _psg_volume_set();
            if (mm.ReadByte(reg.a5 + w.e_ini) == 0)
            {
                _psg_volume_0();
                return;
            }

            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            if ((reg.D0_L & 0x8) != 0)
            {
                _psg_volume_0();
                return;
            }
            reg.D0_B += 4;
            mm.Write(reg.a5 + w.e_p, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_dl, mm.ReadByte(reg.a4 + (UInt32)(Int16)reg.D0_W + 0));
            reg.D1_L = 0x7f;
            reg.D1_B &= mm.ReadByte(reg.a4 + (UInt32)(Int16)reg.D0_W + 1);
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            mm.Write(reg.a5 + w.e_lm, mm.ReadByte(reg.a4 + (UInt32)(Int16)reg.D0_W + 2));
        }

        public void _psg_volume_0()
        {
            reg.D0_L = 0;
            _psg_volume_set();
        }

        public void _psg_volume_set()
        {
            mm.Write(reg.a5 + w.e_ini, (byte)reg.D0_B);
            _psg_volume_set2();
        }

        public void _psg_volume_set2()
        {
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D2_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D2_B < 0)
            {
                reg.D1_B += (UInt32)(sbyte)reg.D2_B;
                if ((sbyte)reg.D1_B < 0)
                {
                    reg.D1_L = 0;
                }
            }
            else
            {
                reg.D1_B += (UInt32)(sbyte)reg.D2_B;
                if (reg.D1_B >= 16) 
                {
                    reg.D1_L = 15;
                }
            }
            reg.D1_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            if ((sbyte)reg.D1_B < 0)
            {
                reg.D1_L = 0;
            }
            reg.D1_B++;
            reg.D1_W *= reg.D0_W;
            UInt16 sp = (UInt16)reg.D1_W;
            reg.D0_L = 0;
            reg.D0_B = (byte)(sp >> 8);
            _psg_volume_set3();
        }

        public void _psg_volume_set3()
        {
            reg.a2 = reg.a5 + w.voltable;
            reg.D0_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D0_W);

            if (reg.D0_B - mm.ReadByte(reg.a5 + w.vol2) != 0)
            {
                mm.Write(reg.a5 + w.vol2, (byte)reg.D0_B);
                reg.D1_L = 8;
                mndrv._OPN_WRITE4();
            }
        }

        //─────────────────────────────────────
        //
        public void _init_lfo_psg()
        {
            if ((sbyte)mm.ReadByte(reg.a5 + w.e_sw) >= 0) return;
            if (mm.ReadByte(reg.a6 + dw.FADEFLAG) == 0)
            {
                reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
                _psg_lfo();
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
        //	MML コマンド処理 ( PSG 部 )
        //
        public void _psg_command()
        {
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            //_psgc:
            switch (reg.D0_W / 2)
            {

                case 0x00: break;
                case 0x01: comcmds._COM_81(); break;// 81
                case 0x02: _PSG_82(); break;// 82
                case 0x03: comcmds._COM_83(); break;// 83	すらー
                case 0x04: _PSG_NOP(); break;// 84
                case 0x05: _PSG_NOP(); break;// 85
                case 0x06: comcmds._COM_86(); break;// 86	同期信号送信
                case 0x07: comcmds._COM_87(); break;// 87	同期待ち
                case 0x08: _PSG_88(); break;// 88	ぴっちべんど
                case 0x09: _PSG_89(); break;// 89	ぽるためんと
                case 0x0a: _PSG_NOP(); break;// 8A
                case 0x0b: _PSG_NOP(); break;// 8B
                case 0x0c: _PSG_NOP(); break;// 8C
                case 0x0d: _PSG_NOP(); break;// 8D
                case 0x0e: _PSG_NOP(); break;// 8E
                case 0x0f: _PSG_NOP(); break;// 8F

                case 0x10: comcmds._COM_90(); break;// 90	q
                case 0x11: comcmds._COM_91(); break;// 91	@q
                case 0x12: _PSG_NOP(); break;// 92
                case 0x13: comcmds._COM_93(); break;// 93	neg @q
                case 0x14: comcmds._COM_94(); break;// 94	keyoff mode
                case 0x15: _PSG_NOP(); break;// 95
                case 0x16: _PSG_NOP(); break;// 96
                case 0x17: _PSG_NOP(); break;// 97
                case 0x18: _PSG_98(); break;// 98	擬似リバーブ
                case 0x19: _PSG_NOP(); break;// 99
                case 0x1a: comcmds._COM_9A(); break;// 9A	擬似 step time
                case 0x1b: _PSG_NOP(); break;// 9B
                case 0x1c: _PSG_NOP(); break;// 9C
                case 0x1d: _PSG_NOP(); break;// 9D
                case 0x1e: _PSG_NOP(); break;// 9E
                case 0x1f: _PSG_NOP(); break;// 9F

                case 0x20: _PSG_A0(); break;// A0	wavenum
                case 0x21: _PSG_A1(); break;// A1	bank + wavenum
                case 0x22: _PSG_A2(); break;// A2	書き換え & 再定義
                case 0x23: _PSG_A3(); break;// A3	音量テーブル
                case 0x24: _PSG_F2(); break;// A4
                case 0x25: _PSG_F5(); break;// A5
                case 0x26: _PSG_F6(); break;// A6
                case 0x27: _PSG_NOP(); break;// A7
                case 0x28: _PSG_NOP(); break;// A8
                case 0x29: _PSG_NOP(); break;// A9
                case 0x2a: _PSG_NOP(); break;// AA
                case 0x2b: _PSG_NOP(); break;// AB
                case 0x2c: _PSG_NOP(); break;// AC
                case 0x2d: _PSG_NOP(); break;// AD
                case 0x2e: _PSG_NOP(); break;// AE
                case 0x2f: _PSG_NOP(); break;// AF

                case 0x30: comcmds._COM_B0(); break;// B0
                case 0x31: _PSG_NOP(); break;// B1
                case 0x32: _PSG_NOP(); break;// B2
                case 0x33: _PSG_NOP(); break;// B3
                case 0x34: _PSG_NOP(); break;// B4
                case 0x35: _PSG_NOP(); break;// B5
                case 0x36: _PSG_NOP(); break;// B6
                case 0x37: _PSG_NOP(); break;// B7
                case 0x38: _PSG_NOP(); break;// B8
                case 0x39: _PSG_NOP(); break;// B9
                case 0x3a: _PSG_NOP(); break;// BA
                case 0x3b: _PSG_NOP(); break;// BB
                case 0x3c: _PSG_NOP(); break;// BC
                case 0x3d: _PSG_NOP(); break;// BD
                case 0x3e: comcmds._COM_BE(); break;// BE	ジャンプ
                case 0x3f: comcmds._COM_BF(); break;// BF

                case 0x40: _PSG_C0(); break;// C0	ソフトウェアエンベロープ
                case 0x41: _PSG_C1(); break;// C1	ソフトウェアエンベロープ
                case 0x42: _PSG_C2(); break;// C2	キーオフボリューム
                case 0x43: _PSG_C3(); break;// C3	ソフトウェアエンベロープスイッチ
                case 0x44: _PSG_A0(); break;// C4	エンベロープ切り替え (@e)
                case 0x45: _PSG_A1(); break;// C5	エンベロープ切り替え (@e bank)
                case 0x46: _PSG_NOP(); break;// C6
                case 0x47: _PSG_NOP(); break;// C7
                case 0x48: _PSG_C8(); break;// C8	ノイズ周波数
                case 0x49: _PSG_C9(); break;// C9	ミキサー
                case 0x4a: _PSG_NOP(); break;// CA
                case 0x4b: _PSG_NOP(); break;// CB
                case 0x4c: _PSG_NOP(); break;// CC
                case 0x4d: _PSG_NOP(); break;// CD
                case 0x4e: _PSG_NOP(); break;// CE
                case 0x4f: _PSG_CF(); break;// CF	エンベロープ2

                case 0x50: comcmds._COM_D0(); break;// D0	キートランスポーズ
                case 0x51: comcmds._COM_D1(); break;// D1	相対キートランスポーズ
                case 0x52: _PSG_NOP(); break;// D2
                case 0x53: _PSG_NOP(); break;// D3
                case 0x54: _PSG_NOP(); break;// D4
                case 0x55: _PSG_NOP(); break;// D5
                case 0x56: _PSG_NOP(); break;// D6
                case 0x57: _PSG_NOP(); break;// D7
                case 0x58: comcmds._COM_D8(); break;// D8	ディチューン
                case 0x59: comcmds._COM_D9(); break;// D9	相対ディチューン
                case 0x5a: _PSG_NOP(); break;// DA
                case 0x5b: _PSG_NOP(); break;// DB
                case 0x5c: _PSG_NOP(); break;// DC
                case 0x5d: _PSG_NOP(); break;// DD
                case 0x5e: _PSG_NOP(); break;// DE
                case 0x5f: _PSG_NOP(); break;// DF

                case 0x60: _PSG_NOP(); break;// E0
                case 0x61: _PSG_NOP(); break;// E1
                case 0x62: comcmds._COM_E2(); break;// E2	pitch LFO
                case 0x63: comcmds._COM_E3(); break;// E3	pitch LFO switch
                case 0x64: comcmds._COM_E4(); break;// E4	pitch LFO delay
                case 0x65: _PSG_NOP(); break;// E5
                case 0x66: _PSG_NOP(); break;// E6
                case 0x67: _PSG_E7(); break;// E7	amp LFO
                case 0x68: _PSG_E8(); break;// E8	amp LFO switch
                case 0x69: _PSG_E9(); break;// E9	amp LFO delay
                case 0x6a: _PSG_NOP(); break;// EA
                case 0x6b: _PSG_NOP(); break;// EB
                case 0x6c: _PSG_NOP(); break;// EC
                case 0x6d: comcmds._COM_ED(); break;// ED
                case 0x6e: _PSG_C8(); break;// EE
                case 0x6f: _PSG_C9(); break;// EF

                case 0x70: _PSG_NOP(); break;// F0
                case 0x71: comcmds._COM_D8(); break;// F1
                case 0x72: _PSG_F2(); break;// F2	volume
                case 0x73: comcmds._COM_91(); break;// F3	@q
                case 0x74: _PSG_NOP(); break;// F4
                case 0x75: _PSG_F5(); break;// F5	)
                case 0x76: _PSG_F6(); break;// F6	(
                case 0x77: _PSG_NOP(); break;// F7
                case 0x78: _PSG_NOP(); break;// F8
                case 0x79: comcmds._COM_F9(); break;// F9	永久ループポイントマーク
                case 0x7a: devopn._FM_FA(); break;// FA	Y command
                case 0x7b: comcmds._COM_FB(); break;// FB	リピート抜け出し
                case 0x7c: comcmds._COM_FC(); break;// FC	リピート開始
                case 0x7d: comcmds._COM_FD(); break;// FD	リピート終端
                case 0x7e: comcmds._COM_FE(); break;// FE	tempo
                case 0x7f: _PSG_FF(); break;// FF	end of data
            }
        }

        //─────────────────────────────────────
        //
        public void _PSG_NOP()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x7f));
            _psg_keyoff();
        }

        //─────────────────────────────────────
        //
        public void _PSG_82()
        {
            _psg_keyoff();
        }

        //─────────────────────────────────────
        //	・ピッチベンド
        //		[$88] + [目標音程]b + [delay]b + [speed]b + [rate]w
        //
        public void _PSG_88()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x80));
            reg.a4 = reg.a5 + w.p_pattern4;

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B += mm.ReadByte(reg.a5 + w.key_trans);
            mm.Write(reg.a5 + w.key2, (byte)reg.D0_B);
            _get_freq();
            mm.Write(reg.a4 + w_l.mokuhyou, (UInt16)reg.D0_W);

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_B = mm.ReadByte(reg.a1++);
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D1_B);

            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a4 + w_l.henka, (UInt16)reg.D0_W);
        }

        //─────────────────────────────────────
        //	・ポルタメント
        //		[$89] + [switch]b + [先note]b + [元note]b + [step]b
        //
        public void _PSG_89()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x80));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x02));
            reg.a4 = reg.a5 + w.p_pattern4;
            reg.D0_B = mm.ReadByte(reg.a1++);

            _PSG_89_normal();
        }

        public void _PSG_89_normal()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B += mm.ReadByte(reg.a5 + w.key_trans);
            _get_freq();
            reg.D1_W = reg.D0_W;

            reg.D0_B = mm.ReadByte(reg.a1);
            reg.D0_B += mm.ReadByte(reg.a5 + w.key_trans);
            _get_freq();

            reg.D2_L = 0;
            reg.D2_B = mm.ReadByte(reg.a1 + 1);
            mm.Write(reg.a4 + w_l.count, (byte)reg.D2_B);

            reg.D1_W -= (UInt32)(Int16)reg.D0_W;
            reg.D1_L = (UInt32)(Int16)reg.D1_W;
            reg.D1_L = (UInt32)((UInt16)((Int32)reg.D1_L / (Int16)reg.D2_W) | (UInt32)(((UInt16)((Int32)reg.D1_L % (Int16)reg.D2_W)) << 16));
            mm.Write(reg.a4 + w_l.henka, (UInt16)reg.D1_W);
            reg.D1_L = (reg.D1_L << 16) | (reg.D1_L >> 16);
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)reg.D1_W);
        }

        //─────────────────────────────────────
        //	frequency
        //
        public void _get_freq()
        {
            reg spReg = new reg();
            spReg.D1_L = reg.D1_L;
            spReg.D2_L = reg.D2_L;
            spReg.a0 = reg.a0;

            reg.D0_W &= 0xff;
            reg.D1_L = 0;
            reg.D2_L = 0xc;
            while (reg.D0_B >= reg.D2_B)
            {
                reg.D0_B -= (UInt32)(sbyte)reg.D2_B;
                reg.D1_B++;
            }
            //reg.D0_W += (UInt32)(Int16)reg.D0_W;
            //reg.a0 = _psg_table;
            reg.D0_W = _psg_table[reg.D0_W];
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D0_W < 0)
            {
                reg.D0_L = 0;
            }
            reg.D0_W >>= (int)reg.D1_W;

            reg.D1_L = spReg.D1_L;
            reg.D2_L = spReg.D2_L;
            reg.a0 = spReg.a0;
        }

        //─────────────────────────────────────
        //	擬似リバーブ
        //		switch = $80 = ON
        //			 $81 = OFF
        //			 $00 = + [volume]b
        //			 $01 = + [volume]b + [pan]b
        //			 $02 = + [volume]b + [tone]b
        //			 $03 = + [volume]b + [panpot]b + [tone]b
        //			 $04 = + [volume]b
        public void _PSG_98()
        {
            comcmds._COM_98();

            if ((mm.ReadByte(reg.a5 + w.reverb) & 0x80) == 0)
            {
                _psg_keyoff();
            }
        }

        //─────────────────────────────────────
        //	・ソフトウェアエンベロープ
        //		[$A0] + [NUM]b
        //
        public void _PSG_A0()
        {
            reg.D5_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.program, (byte)reg.D5_B);

            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.ENV_PTR);
            if (reg.D0_L == 0) return;

            reg.a2 = reg.D0_L;
            reg.a2 += 6;
            reg.D4_W = mm.ReadUInt16(reg.a6 + dw.ENVNUM);
            if (reg.D4_W == 0) return;

            reg.D1_B = mm.ReadByte(reg.a5 + w.bank);
            _psg_a0_ana_loop:
            if (reg.D1_B - mm.ReadByte(reg.a2 + 2) == 0)
            {
                if (reg.D5_B - mm.ReadByte(reg.a2 + 3) == 0) goto _psg_a0_set;
            }
            reg.D4_W--;
            if (reg.D4_W == 0) return;
            reg.D0_W = mm.ReadUInt16(reg.a2);
            reg.a2 = reg.a2 + (UInt32)(Int16)reg.D0_W;
            goto _psg_a0_ana_loop;

            _psg_a0_set:
            reg.D5_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.a2 += 2;

            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);

            reg.D0_L = 16 - 1;
            do
            {
                mm.Write(reg.a0, mm.ReadByte(reg.a2)); reg.a2++; reg.a0++;
            } while (reg.D0_W-- != 0);

            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));

            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a2++);
            if ((sbyte)reg.D1_B >= 0)
            {
                _psg_c9_();
            }
            reg.D0_B = mm.ReadByte(reg.a2); reg.a2++;
            if ((sbyte)reg.D0_B >= 0)
            {
                _psg_c8_();
            }

            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0x4) == 0)
            {
                if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0) return;

                reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
                reg.D0_L = 0;
                reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
                mm.Write(reg.a5 + w.e_dl, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W));
                reg.D1_L = 0x7f;
                reg.D1_B &= mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 1);
                mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);

                mm.Write(reg.a5 + w.e_lm, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 2));

                return;
            }

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0)
            {
                mm.Write(reg.a5 + w.e_p, 0);
                mm.Write(reg.a5 + w.e_sub, 0);
            }
            mm.Write(reg.a5 + w.e_sw, 0x81);
            UInt16 sp = (UInt16)reg.SR_W;
            reg.SR_W |= 0x700;
            //reg.a3 = _ex_soft4;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            ab.hlw_softenv_adrs.Add(reg.a5, _ex_soft4);
            reg.SR_W = sp;

            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
            mm.Write(reg.a5 + w.e_sv, mm.ReadByte(reg.a0 + 3));
            mm.Write(reg.a5 + w.e_ar, mm.ReadByte(reg.a0 + 0));
            mm.Write(reg.a5 + w.e_dr, mm.ReadByte(reg.a0 + 4));
            mm.Write(reg.a5 + w.e_sl, mm.ReadByte(reg.a0 + 6));
            mm.Write(reg.a5 + w.e_sr, mm.ReadByte(reg.a0 + 8));
            mm.Write(reg.a5 + w.e_rr, mm.ReadByte(reg.a0 + 12));
        }

        //─────────────────────────────────────
        //
        //
        public void _PSG_A1()
        {
            mm.Write(reg.a5 + w.bank, mm.ReadByte(reg.a1++));
            _PSG_A0();
        }

        //─────────────────────────────────────
        //
        //
        public void _PSG_A2()
        {
            reg.D1_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.bank, (byte)reg.D1_B);
            reg.D5_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.program, (byte)reg.D5_B);

            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.ENV_PTR);
            if (reg.D0_L == 0) return;

            reg.a2 = reg.D0_L;
            reg.a2 += 6;
            reg.D4_W = mm.ReadUInt16(reg.a6 + dw.ENVNUM);
            if (reg.D4_W == 0) return;

            _psg_a2_ana_loop:
            if (reg.D1_B - mm.ReadByte(reg.a2 + 2) == 0)
            {
                if (reg.D5_B - mm.ReadByte(reg.a2 + 3) == 0)
                    goto _psg_a2_set;
            }
            reg.D4_W--;
            if (reg.D4_W == 0) return;
            reg.D0_W = mm.ReadUInt16(reg.a2);
            reg.a2 = reg.a2 + (UInt32)(Int16)reg.D0_W;
            goto _psg_a2_ana_loop;

            _psg_a2_set:
            reg.D5_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.a2 += 2;

            reg.a0 = 0;// _psg_env_default;

            _psg_env_default[3] = mm.ReadByte(reg.a1++);//mm.Write(reg.a0 + 3, mm.ReadByte(reg.a1++));
            _psg_env_default[0] = mm.ReadByte(reg.a1++);//mm.Write(reg.a0 + 0, mm.ReadByte(reg.a1++));
            _psg_env_default[4] = mm.ReadByte(reg.a1++);//mm.Write(reg.a0 + 4, mm.ReadByte(reg.a1++));
            _psg_env_default[6] = mm.ReadByte(reg.a1++);//mm.Write(reg.a0 + 6, mm.ReadByte(reg.a1++));
            _psg_env_default[8] = mm.ReadByte(reg.a1++);//mm.Write(reg.a0 + 8, mm.ReadByte(reg.a1++));
            _psg_env_default[12] = mm.ReadByte(reg.a1++);//mm.Write(reg.a0 + 12, mm.ReadByte(reg.a1++));

            reg.D0_L = 16 - 1;
            do
            {
                mm.Write(reg.a2, _psg_env_default[reg.a0]);// mm.ReadByte(reg.a0));
                reg.a2++; reg.a0++;
            } while (reg.D0_W-- != 0);

            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
            reg.a2 = 0;// _psg_env_default;

            reg.D0_L = 16 - 1;
            do
            {
                mm.Write(reg.a0, _psg_env_default[reg.a2]);// mm.ReadByte(reg.a2));
                reg.a2++; reg.a0++;
            } while (reg.D0_W-- != 0);

            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));
            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            mm.Write(reg.a5 + w.e_dl, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 0));
            reg.D1_L = 0x7f;
            reg.D1_B &= mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 1);
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            mm.Write(reg.a5 + w.e_lm, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 2));

        }

        //─────────────────────────────────────
        //	音量テーブル
        //
        public void _PSG_A3()
        {
            comcmds._COM_A3();
            _psg_lfo();
        }

        //─────────────────────────────────────
        //	・ソフトウェアエンベロープ
        //		[$C0] + [SV]b + [AR]b + [DR]b + [SL]b + [SR]b + [RR]b
        //
        public void _PSG_C0()
        {
            mm.Write(reg.a5 + w.program, 0xff);
            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
            //reg.a2 = _psg_env_default;
            _psg_env_default[3] = mm.ReadByte(reg.a1++);//mm.Write(reg.a2 + 3, mm.ReadByte(reg.a1++));
            _psg_env_default[0] = mm.ReadByte(reg.a1++);//mm.Write(reg.a2 + 0, mm.ReadByte(reg.a1++));
            _psg_env_default[4] = mm.ReadByte(reg.a1++);//mm.Write(reg.a2 + 4, mm.ReadByte(reg.a1++));
            _psg_env_default[6] = mm.ReadByte(reg.a1++);//mm.Write(reg.a2 + 6, mm.ReadByte(reg.a1++));
            _psg_env_default[8] = mm.ReadByte(reg.a1++);//mm.Write(reg.a2 + 8, mm.ReadByte(reg.a1++));
            _psg_env_default[12] = mm.ReadByte(reg.a1++);//mm.Write(reg.a2 + 12, mm.ReadByte(reg.a1++));
            _psg_env_default[14] = mm.ReadByte(reg.a5 + w.kov);//mm.Write(reg.a2 + 14, mm.ReadByte(reg.a5 + w.kov));
            reg.a2 = 0;

            reg.D0_L = 16 - 1;
            do
            {
                mm.Write(reg.a0, _psg_env_default[reg.a2]);// mm.ReadByte(reg.a2));
                reg.a2++; reg.a0++;
            } while (reg.D0_W-- != 0);

            mm.Write(reg.a5 + w.e_sw, 0x80);
            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            mm.Write(reg.a5 + w.e_dl, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 0));
            reg.D1_L = 0x7f;
            reg.D1_B &= mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 1);
            mm.Write(reg.a5 + w.e_sp, (byte)reg.D1_B);
            mm.Write(reg.a5 + w.e_lm, mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 2));
        }

        public byte[] _psg_env_default = new byte[] {
            0x00,0x01,0xFF,0xFF,0x00,0x81,0x00,0x00,0x00,0x81,0x00,0x00,0xFF,0x81,0x00,0x00
        };

        //─────────────────────────────────────
        //	・ソフトウェアエンベロープ
        //		[$C1] + [AL]b + [DD]b + [SR]b + [RR]b
        //
        public void _PSG_C1()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.e_al, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_alw, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_dd, mm.ReadByte(reg.a1++));
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.e_sr, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_srw, (byte)reg.D0_B);
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.e_rr, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_rrw, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_p, 0);

            mm.Write(reg.a5 + w.e_sw, 0x81);
            UInt16 sp = (UInt16)reg.SR_W;
            reg.SR_W |= 0x700;
            //reg.a3 = _soft2;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            ab.hlw_softenv_adrs.Add(reg.a5, _soft2);
            reg.SR_W = sp;
        }

        //─────────────────────────────────────
        //	・キーオフボリューム
        //		[$C2] + [KOV]
        //
        public void _PSG_C2()
        {
            reg.a0 = mm.ReadUInt32(reg.a5 + w.psgenv_adrs);
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.kov, (byte)reg.D0_B);
            mm.Write(reg.a0 + 14, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	・ソフトウェアエンベロープスイッチ
        //		[$C3] + [switch]
        //
        public void _PSG_C3()
        {
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) & 0x7f));
            reg.D0_B = mm.ReadByte(reg.a1++);

            if (reg.D0_B != 0)
            {
                mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));
            }
        }

        //─────────────────────────────────────
        //	noise 周波数
        //
        public void _PSG_C8()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            _psg_c8_();
        }

        public void _psg_c8_()
        {
            if (mm.ReadByte(reg.a5 + w.ch) < 0x23)
            {
                mm.Write(reg.a6 + dw.NOISE_M, (byte)reg.D0_B);
            }
            else
            {
                mm.Write(reg.a6 + dw.NOISE_S, (byte)reg.D0_B);
            }
            reg.D1_L = 6;
            mndrv._OPN_WRITE2();
        }

        //─────────────────────────────────────
        //	ミキサー設定
        //
        public void _PSG_C9()
        {
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a1++);
            _psg_c9_();
        }

        public void _psg_c9_()
        {
            reg.D3_L = 0;
            reg.D3_B = mm.ReadByte(reg.a5 + w.ch);
            //reg.a0 = _ch_table;
            reg.D6_B = mndrv._ch_table[reg.D3_W + 0];// mm.ReadByte(reg.a0 + reg.D3_W + 0);
            reg.D3_B = mndrv._ch_table[reg.D3_W + 8];// mm.ReadByte(reg.a0 + reg.D3_W + 8);

            if (reg.D3_B < 0x30)
            {
                reg.D0_L = 0;
                reg.D0_B = mm.ReadByte(reg.a6 + dw.PSGMIX_M);
            }
            else
            {
                reg.D0_B = mm.ReadByte(reg.a6 + dw.PSGMIX_S);
            }
            reg.D2_L = 9;
            reg.D2_W <<= (int)reg.D6_W;
            reg.D0_B |= reg.D2_B;
            reg.D1_L <<= (int)reg.D6_L;
            reg.D1_B ^= reg.D2_B;
            reg.D0_B ^= reg.D1_B;

            if (reg.D3_B < 0x30)
            {
                mm.Write(reg.a6 + dw.PSGMIX_M, (byte)reg.D0_B);
            }
            else
            {
                mm.Write(reg.a6 + dw.PSGMIX_S, (byte)reg.D0_B);
            }
            reg.D1_L = 7;
            mndrv._OPN_WRITE2();
        }

        //─────────────────────────────────────
        //	ソフトウェアエンベロープ
        //
        public void _PSG_CF()
        {
            reg.D1_L = 0x1f;
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B &= reg.D1_B;
            mm.Write(reg.a5 + w.eenv_ar, (byte)reg.D0_B);

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B &= reg.D1_B;
            mm.Write(reg.a5 + w.eenv_dr, (byte)reg.D0_B);

            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B &= reg.D1_B;
            mm.Write(reg.a5 + w.eenv_sr, (byte)reg.D0_B);

            reg.D1_B = mm.ReadByte(reg.a1++);
            reg.D0_L = 0xf;
            reg.D0_B &= reg.D1_B;
            mm.Write(reg.a5 + w.eenv_rr, (byte)reg.D0_B);

            reg.D1_B = ~reg.D1_B;
            reg.D1_B >>= 4;
            mm.Write(reg.a5 + w.eenv_sl, (byte)reg.D1_B);

            reg.D0_L = 0xf;
            reg.D0_B &= mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.eenv_al, (byte)reg.D0_B);

            mm.Write(reg.a5 + w.e_sw, 0x81);

            UInt16 sp = (UInt16)reg.SR_W;
            reg.SR_W |= 0x700;
            //reg.a3 = _ex_soft2;
            reg.a3 = ab.dummyAddress;
            mm.Write(reg.a5 + w.softenv_adrs, reg.a3);
            ab.hlw_softenv_adrs.Add(reg.a5, _ex_soft2);
            reg.SR_W = sp;
        }

        //─────────────────────────────────────
        //	音量LFO
        //
        public void _PSG_E7()
        {
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) & 0x7f));
            comcmds._COM_E7();
        }

        //─────────────────────────────────────
        //	音量LFO switch
        //
        public void _PSG_E8()
        {
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) & 0x7f));
            comcmds._COM_E8();
        }

        //─────────────────────────────────────
        //	音量LFO delay
        //
        public void _PSG_E9()
        {
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) & 0x7f));
            comcmds._COM_E9();
        }

        //─────────────────────────────────────
        //	volume
        //
        public void _PSG_F2()
        {
            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.vol, (byte)reg.D0_B);
#if false
            reg.D1_B = reg.D0_B;
            reg.D1_B++;
            reg.D0_W <<= 8;
            reg.D0_W /= reg.D1_W;
            reg.D0_L = (UInt32)((UInt16)((UInt32)reg.D0_L / (UInt16)reg.D1_W) | (UInt32)(((UInt16)((UInt32)reg.D0_L % (UInt16)reg.D1_W)) << 16));
            mm.Write(reg.a5 + w.eenv_limit, reg.D0_B);
#endif
        }

        public void _psg_f2_softenv()
        {
            reg.D1_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D1_B < 0)
            {
                reg.D0_B += (UInt32)(sbyte)reg.D1_B;
                if ((sbyte)reg.D0_B < 0)
                {
                    reg.D0_L = 0;
                }
            }
            else
            {
                reg.D0_B += (UInt32)(sbyte)reg.D1_B;
                if (reg.D0_B >= 16)
                {
                    reg.D0_L = 15;
                }
            }
            reg.D0_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            _psg_lfo();
        }

        public void _psg_lfo()
        {
            if ((sbyte)mm.ReadByte(reg.a5 + w.reverb) >= 0)
            {
                if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0) return;
            }
            reg.D0_W &= 0xf;
            reg.a2 = reg.a5 + w.voltable;
            reg.D0_B = mm.ReadByte(reg.a2 + (UInt32)(Int16)reg.D0_W);
            mm.Write(reg.a5 + w.vol2, (byte)reg.D0_B);
            reg.D1_L = 8;
            mndrv._OPN_WRITE4();
        }

        //─────────────────────────────────────
        //	volup
        //
        public void _PSG_F5()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D0_B += mm.ReadByte(reg.a1++);
            if (reg.D0_B >= 0xf)
            {
                reg.D0_L = 0xf;
            }
            mm.Write(reg.a5 + w.vol, (byte)reg.D0_B);
            if ((sbyte)mm.ReadByte(reg.a5 + w.e_sw) >= 0)
            {
                _psg_lfo();
            }
        }

        //─────────────────────────────────────
        //	voldown
        //
        public void _PSG_F6()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D0_B -= mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a5 + w.vol, (byte)reg.D0_B);
            if ((sbyte)mm.ReadByte(reg.a5 + w.e_sw) >= 0)
            {
                _psg_lfo();
            }
        }

        //─────────────────────────────────────
        //
        public void _PSG_FF()
        {
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfe));

            reg.D0_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a0 = reg.a6 + dw.TRACKWORKADR;
            do
            {
                if ((mm.ReadByte(reg.a0 + w.flag2) & 1) != 0) goto L3;
                reg.a0 = reg.a0 + w._track_work_size;// dw._trackworksize;
                reg.D0_W--;
            } while (reg.D0_W != 0);

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
                    mm.Write(reg.a0 + w.flag2, (byte)(mm.ReadByte(reg.a0 + w.flag2) | 1));
                }
                reg.a0 = reg.a0 + w._track_work_size;// dw._trackworksize;
                reg.D0_W--;
            } while (reg.D0_W != 0);

            L3:
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
                mm.Write(reg.a5 + w.e_p, 0);
                mm.Write(reg.a5 + w.lfo, 0);
                mm.Write(reg.a5 + w.weffect, 0);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x7f));
                mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfe));
                //	pea	_all_end_check(pc)
                _psg_volume_0();
                comcmds._all_end_check();
            }
            reg.a1 = mm.ReadUInt32(reg.a5 + w.loop);
        }

        //─────────────────────────────────────
        //
        public void _ch_psg_lfo_job()
        {
            comwave._ch_effect();
            //_ch_psg_lfo:
            mm.Write(reg.a5 + w.addkeycode, (UInt16)0);
            mm.Write(reg.a5 + w.addvolume, (UInt16)0);

            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            if (reg.D0_B == 0) return;
            reg.D1_L = 0xe;
            reg.D1_B &= reg.D0_B;
            if (reg.D1_B != 0)
            {
                UInt16 sp = (UInt16)reg.D0_W;
                switch (reg.D1_W)
                {
                    case 2:
                        _ch_psg_plfo_1();
                        break;
                    case 4:
                        _ch_psg_plfo_2();
                        break;
                    case 6:
                        _ch_psg_plfo_3();
                        break;
                    case 8:
                        _ch_psg_plfo_4();
                        break;
                    case 10:
                        _ch_psg_plfo_5();
                        break;
                    case 12:
                        _ch_psg_plfo_6();
                        break;
                    case 14:
                        _ch_psg_plfo_7();
                        break;
                }
                reg.D0_W = sp;
            }
            reg.D0_B >>= 4;
            reg.D0_W &= 0x7;

            if (reg.D0_W != 0)
            {
                reg.D0_W += (UInt32)(Int16)reg.D0_W;
                switch (reg.D0_W)
                {
                    case 2:
                        _ch_psg_alfo_1();
                        break;
                    case 4:
                        _ch_psg_alfo_2();
                        break;
                    case 6:
                        _ch_psg_alfo_3();
                        break;
                    case 8:
                        _ch_psg_alfo_4();
                        break;
                    case 10:
                        _ch_psg_alfo_5();
                        break;
                    case 12:
                        _ch_psg_alfo_6();
                        break;
                    case 14:
                        _ch_psg_alfo_7();
                        break;
                }
            }
            //_ch_psg_lfo_end:
            reg.D1_W = mm.ReadUInt16(reg.a5 + w.addvolume);
            if (reg.D1_W == 0) goto _ch_psg_a_exit;
            if ((Int16)reg.D1_W < 0) goto _ch_psg_a_minus;

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D0_W -= (UInt32)(Int16)reg.D1_W;
            if ((Int16)reg.D0_W < 0)
            {
                reg.D0_L = 0;
            }
            _psg_lfo();
            goto _ch_psg_a_exit;

            _ch_psg_a_minus:
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D0_W -= (UInt32)(Int16)reg.D1_W;
            if ((Int16)(reg.D0_W - 0xf) < 0)
            {
                reg.D0_L = 0xf;
            }
            _psg_lfo();

            _ch_psg_a_exit:
            reg.D2_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG2);
            uint f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f != 0) goto _ch_psg_lfo_end2;
            f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f != 0) goto _ch_psg_lfo_end3;

            reg.D0_W = mm.ReadUInt16(reg.a5 + w.freqbase);
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.addkeycode);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D0_W >>= (int)reg.D1_W;
            _set_psg_bend();
            return;

            _ch_psg_lfo_end2:
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.makotune);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D0_W >>= (int)reg.D1_W;
            reg.D1_W = mm.ReadUInt16(reg.a5 + w.detune);

            //; Wed Mar  8 08:23 JST 2000 (saori)
#if false
            if ((Int16)reg.D1_W < 0)
            {
                reg.D1_L = 0;
            }
#endif

            reg.D1_W += mm.ReadUInt16(reg.a5 + w.addkeycode);
            reg.D1_W = (UInt16)(-(Int16)reg.D1_W);
            reg.D1_W = (UInt16)((Int16)reg.D1_W >> 2);
            reg.D0_W += (UInt32)(Int16)reg.D1_W;
            _set_psg_bend();
            return;

            _ch_psg_lfo_end3:
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.makotune);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D0_W >>= (int)reg.D1_W;
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D0_W < 0)
            {
                reg.D0_L = 0;
            }
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.addkeycode);
            _set_psg_bend();
        }

        //─────────────────────────────────────
        public void _ch_psg_plfo_1()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_psg_p_common();
        }

        public void _ch_psg_plfo_2()
        {
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_psg_p_common();
        }

        public void _ch_psg_plfo_3()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_psg_p_common();
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_psg_p_common();
        }

        public void _ch_psg_plfo_4()
        {
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_psg_p_common();
        }

        public void _ch_psg_plfo_5()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_psg_p_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_psg_p_common();
        }

        public void _ch_psg_plfo_6()
        {
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_psg_p_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_psg_p_common();
        }

        public void _ch_psg_plfo_7()
        {
            reg.a4 = reg.a5 + w.p_pattern1;
            reg.a3 = reg.a5 + w.wp_pattern1;
            _ch_psg_p_common();
            reg.a4 = reg.a5 + w.p_pattern2;
            reg.a3 = reg.a5 + w.wp_pattern2;
            _ch_psg_p_common();
            reg.a4 = reg.a5 + w.p_pattern3;
            reg.a3 = reg.a5 + w.wp_pattern3;
            _ch_psg_p_common();
        }

        //─────────────────────────────────────
        public void _ch_psg_alfo_1()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_psg_a_common();
        }

        public void _ch_psg_alfo_2()
        {
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_psg_a_common();
        }

        public void _ch_psg_alfo_3()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_psg_a_common();
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_psg_a_common();
        }

        public void _ch_psg_alfo_4()
        {
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_psg_a_common();
        }

        public void _ch_psg_alfo_5()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_psg_a_common();
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_psg_a_common();
        }

        public void _ch_psg_alfo_6()
        {
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_psg_a_common();
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_psg_a_common();
        }

        public void _ch_psg_alfo_7()
        {
            reg.a4 = reg.a5 + w.v_pattern1;
            reg.a3 = reg.a5 + w.wv_pattern1;
            _ch_psg_a_common();
            reg.a4 = reg.a5 + w.v_pattern2;
            reg.a3 = reg.a5 + w.wv_pattern2;
            _ch_psg_a_common();
            reg.a4 = reg.a5 + w.v_pattern3;
            reg.a3 = reg.a5 + w.wv_pattern3;
            _ch_psg_a_common();
        }

        //─────────────────────────────────────
        public void _ch_psg_a_common()
        {
            reg.D0_L = 1;
            reg.D1_B = mm.ReadByte(reg.a4 + w_l.pattern);
            if ((sbyte)reg.D1_B < 0)
            {
                comwave._com_wavememory();
                mm.Write(reg.a5 + w.addvolume, (UInt16)(mm.ReadUInt16(reg.a5 + w.addvolume) + (Int16)reg.D0_W));
                return;
            }
            reg.D4_W = mm.ReadUInt16(reg.a4 + w_l.flag);
            if ((Int16)reg.D4_W < 0)
            {
                uint f = reg.D4_B & 1;
                reg.D4_B >>= 1;
                if (f == 0)
                {
                    if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0) return;
                }
                else
                {
                    if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0) return;
                }
            }

            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            reg.D0_B += (UInt32)(sbyte)reg.D0_B;

            //_psg_velocity_pattern:
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
            }

            mm.Write(reg.a5 + w.addvolume, (UInt16)(mm.ReadUInt16(reg.a5 + w.addvolume) + (Int16)reg.D1_W));
        }

        //─────────────────────────────────────
        public void _ch_psg_p_common()
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
            if ((Int16)reg.D4_W < 0)
            {
                uint f = reg.D4_B & 1;
                reg.D4_B >>= 1;
                if (f == 0)
                {
                    if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) != 0) return;
                }
                else
                {
                    if ((mm.ReadByte(reg.a5 + w.flag) & 0x20) == 0) return;
                }
            }

            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            reg.D0_B += (UInt32)(sbyte)reg.D0_B;

            if ((sbyte)mm.ReadByte(reg.a6 + dw.LFO_FLAG) >= 0)
            {
                //_psg_pitch_pattern:
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
                        comlfo._com_lfo_portament();
                        break;
                    case 10:
                        comlfo._com_lfo_triangle();
                        break;
                    case 12:
                        comlfo._com_lfo_triangle();
                        break;
                    case 14:
                        comlfo._com_lfo_oneshot();
                        break;
                    case 16:
                        comlfo._com_lfo_oneshot();
                        break;
                }
                mm.Write(reg.a5 + w.addkeycode, (UInt16)(mm.ReadUInt16(reg.a5 + w.addkeycode) + (Int16)reg.D1_W));
                return;
            }

            //_pitch_extend:
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
        public void _ch_psg_mml_job()
        {
            comanalyze._track_analyze();

            //_ch_psg_bend_job:
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            if ((sbyte)reg.D0_B >= 0) return;
            //	btst.b	#1,w_flag2(a5)
            if ((mm.ReadByte(reg.a5 + w.flag2) & 0x02) != 0)
            {
                _ch_psg_porta();
                return;
            }
            _ch_psg_bend();
        }

        //─────────────────────────────────────
        //	pitch bend
        //
        public void _ch_psg_bend()
        {
            reg.a4 = reg.a5 + w.p_pattern4;
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) return;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.freqbase);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D2_W = mm.ReadUInt16(reg.a4 + w_l.henka);

            if ((Int16)reg.D2_W >= 0)
            {
                mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - reg.D2_W));
                reg.D0_W -= (UInt32)(Int16)reg.D2_W;
                mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
                reg.D0_W >>= (int)reg.D1_W;
                if (reg.D0_W < mm.ReadUInt16(reg.a4 + w_l.mokuhyou))
                {
                    _ch_psg_bend_end();
                    return;
                }
                _set_psg_bend();
                return;
            }

            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - reg.D2_W));
            reg.D0_W -= (UInt32)(Int16)reg.D2_W;
            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            if (reg.D0_W >= mm.ReadUInt16(reg.a4 + w_l.mokuhyou))
            {
                _ch_psg_bend_end();
                return;
            }
            _set_psg_bend();
        }

        public void _ch_psg_bend_end()
        {
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.key2);
            mm.Write(reg.a5 + w.key, (byte)reg.D0_B);
            _psg_freq();
        }

        //─────────────────────────────────────
        //	portament
        //
        public void _ch_psg_porta()
        {
            reg.a4 = reg.a5 + w.p_pattern4;
            mm.Write(reg.a4 + w_l.count, (byte)(mm.ReadByte(reg.a4 + w_l.count) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count) == 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
                mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfd));
                return;
            }
            reg.D2_W = mm.ReadUInt16(reg.a4 + w_l.henka);

            reg.D0_W = mm.ReadUInt16(reg.a5 + w.keycode);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D2_W));
            reg.D0_W += (UInt32)(Int16)reg.D2_W;

            if (mm.ReadUInt16(reg.a4 + w_l.henka_work) == 0) goto _ch_psg_porta_common;
            if ((Int16)mm.ReadUInt16(reg.a4 + w_l.henka_work) < 0) goto _ch_psg_porta_minus;

            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.henka_work) - 1));
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + 1));
            reg.D0_W++;
            goto _ch_psg_porta_common;

            _ch_psg_porta_minus:
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.henka_work) + 1));
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - 1));
            reg.D0_W--;

            _ch_psg_porta_common:
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D2_W = reg.D0_W;
            reg.D2_W <<= (int)reg.D1_W;
            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D2_W);
            mm.Write(reg.a5 + w.freqwork, (UInt16)reg.D0_W);
            mm.Write(reg.a5 + w.makotune, (UInt16)reg.D2_W);
            _set_psg_bend();

        }

        //─────────────────────────────────────
        //
        //
        public void _soft2()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            if (reg.D0_B == 0) return;
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.vol2);
            //_soft2_al:
            reg.D0_B--;//1
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a5 + w.e_alw, (byte)(mm.ReadByte(reg.a5 + w.e_alw) - 1));
                if (mm.ReadByte(reg.a5 + w.e_alw) != 0) goto _soft2_ok;
                mm.Write(reg.a5 + w.e_p, 2);
                goto _soft2_ok;
            }
            reg.D0_B--;//2
            if (reg.D0_B == 0)
            {
                if (mm.ReadByte(reg.a5 + w.e_dd) == 0) goto _soft2_ok;//dr=0 は減衰無し
                mm.Write(reg.a5 + w.e_p, 3);
                reg.D1_B += mm.ReadByte(reg.a5 + w.e_dd);
                if ((sbyte)reg.D1_B >= 0) goto _soft2_ok;
                reg.D1_L = 0;
                mm.Write(reg.a5 + w.e_p, 4);
                goto _soft2_ok;
            }
            //_soft2_sr:
            reg.D0_B--;//3
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a5 + w.e_srw, (byte)(mm.ReadByte(reg.a5 + w.e_srw) - 1));
                if (mm.ReadByte(reg.a5 + w.e_srw) != 0) goto _soft2_ok;
                mm.Write(reg.a5 + w.e_srw, mm.ReadByte(reg.a5 + w.e_sr));
                bool cf = reg.D1_B < 1;
                reg.D1_B--;
                if (!cf) goto _soft2_ok;
                reg.D1_L = 0;
                mm.Write(reg.a5 + w.e_p, 4);
                goto _soft2_ok;
            }
            //_soft2_rr:
            reg.D0_B--;//4
            if (reg.D0_B == 0)
            {
                if (mm.ReadByte(reg.a5 + w.e_rr) != 0)// rr = 0 は消音
                {
                    mm.Write(reg.a5 + w.e_rrw, (byte)(mm.ReadByte(reg.a5 + w.e_rrw) - 1));
                    if (mm.ReadByte(reg.a5 + w.e_rrw) != 0) goto _soft2_ok;
                    mm.Write(reg.a5 + w.e_rrw, mm.ReadByte(reg.a5 + w.e_rr));
                    reg.D1_B--;
                    goto _soft2_ok;
                }
                reg.D1_L = 0;
                mm.Write(reg.a5 + w.e_p, 4);
                goto _soft2_ok;
            }
            //_soft2_ko:
            mm.Write(reg.a5 + w.e_p, 1);
            mm.Write(reg.a5 + w.e_alw, mm.ReadByte(reg.a5 + w.e_al));
            mm.Write(reg.a5 + w.e_srw, mm.ReadByte(reg.a5 + w.e_sr));
            mm.Write(reg.a5 + w.e_rrw, mm.ReadByte(reg.a5 + w.e_rr));
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            _soft3_0();
            return;

            _soft2_ok:
            reg.D0_B = reg.D1_B;
            _soft3_0();

        }

        //─────────────────────────────────────
        //	extend software envelop
        //
        public void _ex_soft2()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            if (reg.D0_B == 0) return;
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.vol2);
            //_soft3_ar:
            reg.D0_B--;
            if (reg.D0_B != 0) goto esm_dr_check;
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_arc);
            reg.D2_B--;
            if ((sbyte)reg.D2_B < 0) goto arc_count_check;
            reg.D2_B++;
            mm.Write(reg.a5 + w.eenv_volume, (byte)(mm.ReadByte(reg.a5 + w.eenv_volume) + (sbyte)reg.D2_B));
            if (mm.ReadByte(reg.a5 + w.eenv_volume) >= 15) goto esm_ar_next;
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_ar);
            reg.D2_B -= 16;
            mm.Write(reg.a5 + w.eenv_arc, (byte)reg.D2_B);
            goto _soft3_ok;

            esm_ar_next:
            mm.Write(reg.a5 + w.eenv_volume, 15);
            mm.Write(reg.a5 + w.e_p, 2);//DR
            if (mm.ReadByte(reg.a5 + w.eenv_sl) - 15 != 0) goto _soft3_ok;
            mm.Write(reg.a5 + w.e_p, 3);//SR
            goto _soft3_ok;

            arc_count_check:
            if (mm.ReadByte(reg.a5 + w.eenv_ar) == 0) goto _soft3_ok;
            mm.Write(reg.a5 + w.eenv_arc, (byte)(mm.ReadByte(reg.a5 + w.eenv_arc) + 1));
            goto _soft3_ok;

            esm_dr_check:
            reg.D0_B--;//2
            if (reg.D0_B != 0) goto esm_sr_check;

            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_drc);
            reg.D2_B--;
            if ((sbyte)reg.D2_B < 0) goto drc_count_check;
            reg.D2_B++;
            bool cf = mm.ReadByte(reg.a5 + w.eenv_volume) < reg.D2_B;
            mm.Write(reg.a5 + w.eenv_volume, (byte)(mm.ReadByte(reg.a5 + w.eenv_volume) - reg.D2_B));
            //uint sp = reg.SR_W;
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_sl);
            //reg.SR_W = (reg.SR_W & 0xff00) | (sp & 0xff);
            if (cf) goto dr_slset;
            reg.D3_B = mm.ReadByte(reg.D5_B + w.eenv_volume);
            if (reg.D3_B < reg.D2_B) goto dr_slset;

            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_dr);
            reg.D2_B -= 16;
            if ((sbyte)reg.D2_B >= 0) goto esm_dr_notx;
            reg.D2_B += (UInt32)(sbyte)reg.D2_B;

            esm_dr_notx:
            mm.Write(reg.a5 + w.eenv_drc, (byte)reg.D2_B);
            goto _soft3_ok;

            dr_slset:
            mm.Write(reg.a5 + w.eenv_volume, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.e_p, 3);
            goto _soft3_ok;

            drc_count_check:
            if (mm.ReadByte(reg.a5 + w.eenv_dr) == 0) goto _soft3_ok;
            mm.Write(reg.a5 + w.eenv_drc, (byte)(mm.ReadByte(reg.a5 + w.eenv_drc) + 1));
            goto _soft3_ok;

            esm_sr_check:
            reg.D0_B--;//3
            if (reg.D0_B != 0) goto esm_rr;

            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_src);
            reg.D2_B--;
            if ((sbyte)reg.D2_B < 0) goto src_count_check;
            reg.D2_B++;
            cf = mm.ReadByte(reg.a5 + w.eenv_volume) < reg.D2_B;
            mm.Write(reg.a5 + w.eenv_volume, (byte)(mm.ReadByte(reg.a5 + w.eenv_volume) - reg.D2_B));
            if (!cf) goto esm_sr_exit;
            mm.Write(reg.a5 + w.eenv_volume, 0);

            esm_sr_exit:
            mm.Write(reg.a5 + w.eenv_sr, (byte)reg.D2_B);
            reg.D2_B -= 16;
            if ((sbyte)reg.D2_B >= 0) goto esm_sr_notx;
            reg.D2_B += (UInt32)(sbyte)reg.D2_B;

            esm_sr_notx:
            mm.Write(reg.a5 + w.eenv_src, (byte)reg.D2_B);
            goto _soft3_ok;

            src_count_check:
            if (mm.ReadByte(reg.a5 + w.eenv_sr) == 0) goto _soft3_ok;
            mm.Write(reg.a5 + w.eenv_src, (byte)(mm.ReadByte(reg.a5 + w.eenv_src) + 1));
            goto _soft3_ok;

            esm_rr:
            reg.D0_B--;//4
            if (reg.D0_B != 0) goto esm_ko;

            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_rrc);
            if (reg.D2_B == 0) goto rrc_count_check;
            cf = mm.ReadByte(reg.a5 + w.eenv_volume) < reg.D2_B;
            mm.Write(reg.a5 + w.eenv_volume, (byte)(mm.ReadByte(reg.a5 + w.eenv_volume) - reg.D2_B));
            if (!cf) goto esm_rr_exit;
            mm.Write(reg.a5 + w.eenv_volume, 0);

            esm_rr_exit:
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_rr);
            reg.D2_B += (UInt32)(sbyte)reg.D2_B;
            reg.D2_B -= 16;
            mm.Write(reg.a5 + w.eenv_rrc, (byte)reg.D2_B);
            goto _soft3_ok;

            rrc_count_check:
            if (mm.ReadByte(reg.a5 + w.eenv_rr) == 0) goto _soft3_ok;
            mm.Write(reg.a5 + w.eenv_rrc, (byte)(mm.ReadByte(reg.a5 + w.eenv_rrc) + 1));
            goto _soft3_ok;

            esm_ko:
            reg.D0_B--;
            if (reg.D0_B != 0)
            {
                _psg_volume_0();
                return;
            }

            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_ar);
            reg.D2_B -= 16;
            mm.Write(reg.a5 + w.eenv_arc, (byte)reg.D2_B);
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_dr);
            reg.D2_B -= 16;
            if ((sbyte)reg.D2_B >= 0) goto eei_dr_notx;
            reg.D2_B += (UInt32)(sbyte)reg.D2_B;

            eei_dr_notx:
            mm.Write(reg.a5 + w.eenv_drc, (byte)reg.D2_B);
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_sr);
            reg.D2_B -= 16;
            if ((sbyte)reg.D2_B >= 0) goto eei_sr_notx;
            reg.D2_B += (UInt32)(sbyte)reg.D2_B;

            eei_sr_notx:
            mm.Write(reg.a5 + w.eenv_src, (byte)reg.D2_B);
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_rr);
            reg.D2_B += (UInt32)(sbyte)reg.D2_B;
            reg.D2_B -= 16;
            mm.Write(reg.a5 + w.eenv_rrc, (byte)reg.D2_B);
            reg.D2_B = mm.ReadByte(reg.a5 + w.eenv_al);
            mm.Write(reg.a5 + w.eenv_volume, (byte)reg.D2_B);
            mm.Write(reg.a5 + w.e_p, 1);
            //mm.Write(reg.a5 + w.eenv_sl, mm.ReadByte(reg.a5 + w.vol));

            _soft3_ok:
            //
            // 拡張版 音量=dl*(eenv_vol+1)/16
            //
            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.eenv_volume);
            if (reg.D1_B == 0)
            {
                _soft3_0();
                return;
            }
            reg.D1_B++;
            reg.D0_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D0_W *= reg.D1_W;
            uint f = reg.D0_B & 0x8;
            reg.D0_B >>= 4;
            if (f != 0)
            {
                reg.D0_B++;
            }

            _soft3_0();
        }

        public void _soft3_0()
        {
            reg.D2_B = mm.ReadByte(reg.a5 + w.track_vol);
            if ((sbyte)reg.D2_B < 0)
            {
                reg.D0_B += (UInt32)(sbyte)reg.D2_B;
                if ((sbyte)reg.D0_B < 0)
                {
                    reg.D0_L = 0;
                }
            }
            else
            {
                reg.D0_B += (UInt32)(sbyte)reg.D2_B;
                if (reg.D0_B >= 16)
                {
                    reg.D0_L = 15;
                }
            }
            reg.D0_B -= mm.ReadByte(reg.a6 + dw.MASTER_VOL_PSG);
            if ((sbyte)reg.D0_B >= 0)
            {
                _psg_volume_set3();
                return;
            }
            reg.D0_L = 0;
            _psg_volume_set3();
            return;
        }

        //─────────────────────────────────────
        //	extend software envelop
        //
        public void _ex_soft4()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_p);
            if (reg.D0_B == 0) goto _ex_soft4_end;

            //_ex_soft4_ar:				// 1
            reg.D0_B--;
            if (reg.D0_B != 0) goto _ex_soft4_dr;

            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            reg.D0_B += mm.ReadByte(reg.a5 + w.e_ar);
            if (reg.D0_B < mm.ReadByte(reg.a5 + w.eenv_limit)) goto _ex_soft4_ok;
            reg.D0_B = mm.ReadByte(reg.a5 + w.eenv_limit);
            mm.Write(reg.a5 + w.e_p, 2);
            goto _ex_soft4_ok;

            _ex_soft4_dr:				// 2
            reg.D0_B--;
            if (reg.D0_B != 0) goto _ex_soft4_sr;

            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            bool cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_dr);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_dr);
            if (cf) goto _ex_soft4_dr2;
            if (reg.D0_B >= mm.ReadByte(reg.a5 + w.e_sl)) goto _ex_soft4_ok;

            _ex_soft4_dr2:
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sl);
            mm.Write(reg.a5 + w.e_p, 3);
            goto _ex_soft4_ok;

            _ex_soft4_sr:				// 3
            reg.D0_B--;
            if (reg.D0_B != 0) goto _ex_soft4_rr;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_sr);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_sr);
            if ((sbyte)reg.D0_B == 0) goto _ex_soft4_end;
            if (!cf) goto _ex_soft4_ok;
            goto _ex_soft4_end;

            _ex_soft4_rr:				// 4
            reg.D0_B--;
            if (reg.D0_B != 0) goto _ex_soft4_ko;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            cf = reg.D0_B < mm.ReadByte(reg.a5 + w.e_rr);
            reg.D0_B -= mm.ReadByte(reg.a5 + w.e_rr);
            if ((sbyte)reg.D0_B == 0) goto _ex_soft4_end;
            if (!cf) goto _ex_soft4_ok;
            if (cf) goto _ex_soft4_end;

            _ex_soft4_ko:				// 5
            mm.Write(reg.a5 + w.e_p, 1);
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sv);
            if (reg.D0_B < mm.ReadByte(reg.a5 + w.eenv_limit)) goto _ex_soft4_ok;
            reg.D0_B = mm.ReadByte(reg.a5 + w.eenv_limit);

            _ex_soft4_ok:
            mm.Write(reg.a5 + w.e_sub, (byte)reg.D0_B);
            goto _ex_soft4_volume_set;

            _ex_soft4_end:
            reg.D0_L = 0;
            mm.Write(reg.a5 + w.e_sub, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.e_p, (byte)reg.D0_B);

            _ex_soft4_volume_set:
            reg.D0_L = 0;
            reg.D1_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.e_sub);
            if (reg.D0_B == 0)
            {
                _soft3_0();
                return;
            }
            reg.D1_B = mm.ReadByte(reg.a5 + w.vol);
            reg.D1_B++;
            reg.D0_W *= reg.D1_W;
            reg.D0_L = reg.D0_W >> 8;
            _soft3_0();
        }

    }
}