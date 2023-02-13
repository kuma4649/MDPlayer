using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class devpsgemu
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public mndrv mndrv;
        public comanalyze comanalyze;
        public comcmds comcmds;
        public comlfo comlfo;
        public comwave comwave;
        public devpsg devpsg;
        public devopm devopm;

        //
        //	part of YM2608 - PSG emulation
        //

        //─────────────────────────────────────
        public void _psge_note_set()
        {
            mm.Write(reg.a5 + w.key, (byte)reg.D0_B);

            //    pea _opm_keyon(pc)
            _emu_psg_freq();
            comlfo._init_lfo();
            devopm._opm_keyon();
        }

        //─────────────────────────────────────
        public void _emu_psg_freq()
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
            reg.a0 = ab.dummyAddress;// _psg_table;
            reg.D0_W = devpsg._psg_table[reg.D0_W / 2];
            mm.Write(reg.a5 + w.makotune, (UInt16)reg.D0_W);
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 1) == 0)
            {
                _emu_set_psg_();
                return;
            }

            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            mm.Write(reg.a5 + w.freqwork, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            reg.D1_W = mm.ReadUInt16(reg.a5 + w.detune);
            if ((Int16)reg.D1_W < 0)
            {
                reg.D1_L = 0;
            }

            reg.D1_W = (UInt16)(-(Int16)reg.D1_W);

            reg.D1_W = (UInt16)((Int16)reg.D1_W >> 2);
            reg.D0_W += (UInt32)(Int16)reg.D1_W;
            mm.Write(reg.a5 + w.keycode2, (UInt16)reg.D0_W);
            _emu_set_psg_bend();
        }

        public void _emu_set_psg_()
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

            _emu_set_psg_bend();
        }

        public void _emu_set_psg_bend()
        {
            mm.Write(reg.a5 + w.keycode, (UInt16)reg.D0_W);
            reg.D0_W &= 0xfff;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.a0 = reg.a6 + dw.FREQ_KC_TABLE;
            reg.D2_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 1);
            reg.D0_B = mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W );
            reg.D1_L = 0x28;
            mndrv._OPM_WRITE4();
            reg.D1_L = 0x30;
            reg.D0_B = reg.D2_B;
            mndrv._OPM_WRITE4();
        }

        //─────────────────────────────────────
        //	MML コマンド処理(PSG 部)
        //
        public void _psge_command()
        {
            reg.D0_W += (UInt32)(Int16)reg.D0_W;

            //_psgec:
            switch (reg.D0_W / 2)
            {
                case 0x00: break;
                case 0x01: comcmds._COM_81(); break;// 81
                case 0x02: devopm._OPM_82(); break;// 82	key off
                case 0x03: comcmds._COM_83(); break;// 83	すらー
                case 0x04: _PSGE_NOP(); break;// 84
                case 0x05: _PSGE_NOP(); break;// 85
                case 0x06: comcmds._COM_86(); break;// 86	同期信号送信
                case 0x07: comcmds._COM_87(); break;// 87	同期信号待ち
                case 0x08: devpsg._PSG_88(); break;// 88	ぴっちべんど
                case 0x09: devpsg._PSG_89(); break;// 89	ぽるためんと
                case 0x0a: _PSGE_NOP(); break;// 8A
                case 0x0b: _PSGE_NOP(); break;// 8B
                case 0x0c: _PSGE_NOP(); break;// 8C
                case 0x0d: _PSGE_NOP(); break;// 8D
                case 0x0e: _PSGE_NOP(); break;// 8E
                case 0x0f: _PSGE_NOP(); break;// 8F

                case 0x10: comcmds._COM_90(); break;// 90	q
                case 0x11: comcmds._COM_91(); break;// 91	@q
                case 0x12: devopm._OPM_92(); break;// 92	keyoff rr cut switch
                case 0x13: comcmds._COM_93(); break;// 93	neg @q
                case 0x14: comcmds._COM_94(); break;// 94	keyoff mode
                case 0x15: _PSGE_NOP(); break;// 95
                case 0x16: _PSGE_NOP(); break;// 96
                case 0x17: _PSGE_NOP(); break;// 97
                case 0x18: devopm._OPM_98(); break;// 98	擬似リバーブ
                case 0x19: devopm._OPM_99(); break;// 99	擬似エコー
                case 0x1a: comcmds._COM_9A(); break;// 9A 擬似step time
                case 0x1b: _PSGE_NOP(); break;// 9B
                case 0x1c: _PSGE_NOP(); break;// 9C
                case 0x1d: _PSGE_NOP(); break;// 9D
                case 0x1e: _PSGE_NOP(); break;// 9E
                case 0x1f: _PSGE_NOP(); break;// 9F

                case 0x20: devopm._OPM_F0(); break;// A0 音色切り替え
                case 0x21: devopm._OPM_A1(); break;// A1 バンク&音色切り替え
                case 0x22: devpsg._PSG_A2(); break;// A2
                case 0x23: devopm._OPM_A3(); break;// A3 音量テーブル切り替え
                case 0x24: devopm._OPM_F2(); break;// A4 音量
                case 0x25: devopm._OPM_F5(); break;// A5
                case 0x26: devopm._OPM_F6(); break;// A6
                case 0x27: _PSGE_NOP(); break;// A7
                case 0x28: comcmds._COM_A8(); break;// A8 相対音量モード
                case 0x29: _PSGE_NOP(); break;// A9
                case 0x2a: _PSGE_NOP(); break;// AA
                case 0x2b: _PSGE_NOP(); break;// AB
                case 0x2c: _PSGE_NOP(); break;// AC
                case 0x2d: _PSGE_NOP(); break;// AD
                case 0x2e: _PSGE_NOP(); break;// AE
                case 0x2f: _PSGE_NOP(); break;// AF

                case 0x30: comcmds._COM_B0(); break;// B0
                case 0x31: _PSGE_NOP(); break;// B1
                case 0x32: _PSGE_NOP(); break;// B2
                case 0x33: _PSGE_NOP(); break;// B3
                case 0x34: _PSGE_NOP(); break;// B4
                case 0x35: _PSGE_NOP(); break;// B5
                case 0x36: _PSGE_NOP(); break;// B6
                case 0x37: _PSGE_NOP(); break;// B7
                case 0x38: _PSGE_NOP(); break;// B8
                case 0x39: _PSGE_NOP(); break;// B9
                case 0x3a: _PSGE_NOP(); break;// BA
                case 0x3b: _PSGE_NOP(); break;// BB
                case 0x3c: _PSGE_NOP(); break;// BC
                case 0x3d: _PSGE_NOP(); break;// BD
                case 0x3e: comcmds._COM_BE(); break;// BE ジャンプ
                case 0x3f: comcmds._COM_BF(); break;// BF

                // PSG 系                    
                case 0x40: comcmds._COM_C0(); break;// C0 ソフトウェアエンベロープ 1
                case 0x41: comcmds._COM_C1(); break;// C1 ソフトウェアエンベロープ 2
                case 0x42: _PSGE_NOP(); break;// C2
                case 0x43: comcmds._COM_C3(); break;// C3	switch
                case 0x44: comcmds._COM_C4(); break;// C4 env(num)
                case 0x45: comcmds._COM_C5(); break;// C5 env(bank + num)
                case 0x46: _PSGE_NOP(); break;// C6
                case 0x47: _PSGE_NOP(); break;// C7
                case 0x48: devopm._OPM_C8(); break;// C8
                case 0x49: _PSGE_C9(); break;// C9
                case 0x4a: _PSGE_NOP(); break;// CA
                case 0x4b: _PSGE_NOP(); break;// CB
                case 0x4c: _PSGE_NOP(); break;// CC
                case 0x4d: _PSGE_NOP(); break;// CD
                case 0x4e: _PSGE_NOP(); break;// CE
                case 0x4f: _PSGE_NOP(); break;// CF

                // KEY 系                    
                case 0x50: comcmds._COM_D0(); break;// D0 キートランスポーズ
                case 0x51: comcmds._COM_D1(); break;// D1 相対キートランスポーズ
                case 0x52: _PSGE_NOP(); break;// D2
                case 0x53: _PSGE_NOP(); break;// D3
                case 0x54: _PSGE_NOP(); break;// D4
                case 0x55: _PSGE_NOP(); break;// D5
                case 0x56: _PSGE_NOP(); break;// D6
                case 0x57: _PSGE_NOP(); break;// D7
                case 0x58: comcmds._COM_D8(); break;// D8 ディチューン
                case 0x59: comcmds._COM_D9(); break;// D9 相対ディチューン
                case 0x5a: _PSGE_NOP(); break;// DA スロットディチューン
                case 0x5b: _PSGE_NOP(); break;// DB 相対スロットディチューン
                case 0x5c: _PSGE_NOP(); break;// DC
                case 0x5d: _PSGE_NOP(); break;// DD
                case 0x5e: _PSGE_NOP(); break;// DE
                case 0x5f: _PSGE_NOP(); break;// DF

                // LFO 系                    
                case 0x60: devopm._OPM_E0(); break;// E0 hardware LFO
                case 0x61: devopm._OPM_E1(); break;// E1 hardware LFO switch
                case 0x62: comcmds._COM_E2(); break;// E2 pitch LFO
                case 0x63: comcmds._COM_E3(); break;// E3 pitch LFO switch
                case 0x64: comcmds._COM_E4(); break;// E4 pitch LFO delay
                case 0x65: _PSGE_NOP(); break;// E5
                case 0x66: _PSGE_NOP(); break;// E6
                case 0x67: comcmds._COM_E7(); break;// E7 amp LFO
                case 0x68: devopm._OPM_E8(); break;// E8 amp LFO switch
                case 0x69: comcmds._COM_E9(); break;// E9 amp LFO delay
                case 0x6a: comcmds._COM_EA(); break;// EA amp switch 2
                case 0x6b: comcmds._COM_EB(); break;// EB
                case 0x6c: _PSGE_NOP(); break;// EC
                case 0x6d: comcmds._COM_ED(); break;// ED
                case 0x6e: devpsg._PSG_C8(); break;// EE LW type LFO
                case 0x6f: devpsg._PSG_C9(); break;// EF hardware LFO delay

                // システコル系              
                case 0x70: devopm._OPM_F0(); break;// F0	@
                case 0x71: comcmds._COM_D8(); break;// F1
                case 0x72: devopm._OPM_F2(); break;// F2 volume
                case 0x73: comcmds._COM_91(); break;// F3
                case 0x74: devopm._OPM_F4(); break;// F4 pan
                case 0x75: devopm._OPM_F5(); break;// F5	) volup
                case 0x76: devopm._OPM_F6(); break;// F6(voldown
                case 0x77: _PSGE_NOP(); break;// F7 効果音モード切り替え
                case 0x78: devopm._OPM_F8(); break;// F8 スロットマスク変更
                case 0x79: comcmds._COM_F9(); break;// F9 永久ループポイントマーク
                case 0x7a: devopm._OPM_FA(); break;// FA y command
                case 0x7b: comcmds._COM_FB(); break;// FB リピート抜け出し
                case 0x7c: comcmds._COM_FC(); break;// FC リピート開始
                case 0x7d: comcmds._COM_FD(); break;// FD リピート終端
                case 0x7e: comcmds._COM_FE(); break;// FE tempo
                case 0x7f: devopm._OPM_FF(); break;// FF end of data
            }
        }

        //─────────────────────────────────────
        public void _PSGE_C9()
        {
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a1++);
        }

        //─────────────────────────────────────
        //
        public void _PSGE_NOP()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x7f));
            devopm._opm_keyoff();
        }

        //─────────────────────────────────────
        //
        public void _ch_psge_lfo_job()
        {
            comwave._ch_effect();
            //_ch_psge_lfo:
            mm.Write(reg.a5 + w.addkeycode, (UInt16)0);
            mm.Write(reg.a5 + w.addvolume, (UInt16)0);

            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            if (reg.D0_B == 0) return;
            reg.D1_L = 0xe;
            reg.D1_B &= reg.D0_B;
            if (reg.D1_B != 0)
            {
                reg.a4 = ab.dummyAddress;
                switch (reg.D1_W)
                {
                    case 2:
                        devpsg._ch_psg_plfo_1();
                        break;
                    case 4:
                        devpsg._ch_psg_plfo_2();
                        break;
                    case 6:
                        devpsg._ch_psg_plfo_3();
                        break;
                    case 8:
                        devpsg._ch_psg_plfo_4();
                        break;
                    case 10:
                        devpsg._ch_psg_plfo_5();
                        break;
                    case 12:
                        devpsg._ch_psg_plfo_6();
                        break;
                    case 14:
                        devpsg._ch_psg_plfo_7();
                        break;
                }
            }

            //_ch_psge_lfo_end:
            if ((mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 1) != 0) goto _ch_psge_lfo_end2;

            reg.D0_W = mm.ReadUInt16(reg.a5 + w.freqbase);
            reg.D0_W += mm.ReadUInt16(reg.a5 + w.addkeycode);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D0_W >>= (int)reg.D1_W;
            _emu_set_psg_bend();
            return;

            _ch_psge_lfo_end2:
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.makotune);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D0_W >>= (int)reg.D1_W;
            reg.D1_W = mm.ReadUInt16(reg.a5 + w.detune);
            reg.D1_W += mm.ReadUInt16(reg.a5 + w.addkeycode);
            reg.D1_W = (UInt16)(-(Int16)reg.D1_W);
            reg.D1_W = (UInt16)((Int16)reg.D1_W >> 2);
            reg.D0_W += (UInt32)(Int16)reg.D1_W;
            _emu_set_psg_bend();
        }

        //─────────────────────────────────────
        public void _ch_psge_mml_job()
        {
            comanalyze._track_analyze();
            //_ch_psge_bend_job:
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            if ((sbyte)reg.D0_B >= 0) return;
            if ((mm.ReadByte(reg.a5 + w.flag2) & 2) !=0)
            {
                _ch_psge_porta();
                return;
            }
            _ch_psge_bend();
        }

        //─────────────────────────────────────
        //	pitch bend
        //
        public void _ch_psge_bend()
        {
            reg.a4 = reg.a5 + w.p_pattern4;
            mm.Write(reg.a4 + w_l.delay_work, (byte)(mm.ReadByte(reg.a4 + w_l.delay_work) - 1));
            if (mm.ReadByte(reg.a4 + w_l.delay_work) != 0) return;

            mm.Write(reg.a4 + w_l.delay_work, mm.ReadByte(reg.a4 + w_l.lfo_sp));
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.freqbase);
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D2_W = mm.ReadUInt16(reg.a4 + w_l.henka);
            if ((Int16)(reg.D2_W) < 0) goto _ch_psge_bend_minus;
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - reg.D2_W));
            reg.D0_W -= (UInt32)(Int16)reg.D2_W;

            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            if (reg.D0_W < mm.ReadUInt16(reg.a4 + w_l.mokuhyou)) goto _ch_psge_bend_end;
            _emu_set_psg_bend();
            return;

            _ch_psge_bend_minus:
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) - reg.D2_W));
            reg.D0_W -= (UInt32)(Int16)reg.D2_W;
            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D0_W);
            reg.D0_W >>= (int)reg.D1_W;
            if (reg.D0_W >= mm.ReadUInt16(reg.a4 + w_l.mokuhyou)) goto _ch_psge_bend_end;
            _emu_set_psg_bend();
            return;

            _ch_psge_bend_end:
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a5 + w.key2);
            mm.Write(reg.a5 + w.key, (byte)reg.D0_B);
            _emu_psg_freq();
        }

        //─────────────────────────────────────
        //	portament
        //
        public void _ch_psge_porta()
        {
            reg.a4 = reg.a5 + w.p_pattern4;
            mm.Write(reg.a4 + w_l.count, (byte)(mm.ReadByte(reg.a4 + w_l.count) - 1));
            if (mm.ReadByte(reg.a4 + w_l.count) == 0) goto _ch_psge_porta_end;

            reg.D2_W = mm.ReadUInt16(reg.a4 + w_l.henka);
            goto L1;
            _ch_psge_porta_end:
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xfd));
            reg.D2_W = mm.ReadUInt16(reg.a4 + w_l.henka_work);
            L1:
            reg.D0_W = mm.ReadUInt16(reg.a5 + w.keycode);
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.bendwork) + (Int16)reg.D2_W));
            reg.D0_W += (UInt32)(Int16)reg.D2_W;
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.octave);
            reg.D2_W = reg.D0_W;
            reg.D2_W <<= (int)reg.D1_W;
            mm.Write(reg.a5 + w.freqbase, (UInt16)reg.D2_W);
            _emu_set_psg_bend();
        }
    }
}
