using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    //
    //	part of common commands
    //
    public class comcmds
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public mndrv mndrv;
        public comlfo comlfo;
        public ab ab;

        //	タイ
        //		[$81]
        public void _COM_81()
        {
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x40) != 0) return;
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xbf));
        }

        //	スラー
        //		[$83]
        public void _COM_83()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x40));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x40));
        }

        //	同期信号送信
        //		[$86] + [track] b
        public void _COM_86()
        {
            reg.D0_L = 0;
            reg.a0 = dw.TRACKWORKADR + reg.a6;

            reg.D0_B = mm.ReadByte(reg.a1++);
            while (true)
            {
                reg.D0_W--;
                if (reg.D0_W == 0) break;

                reg.a0 = w._track_work_size + reg.a0;
            }

            int a = mm.ReadByte(reg.a0 + w.flag4) & 0x80;
            mm.Write(reg.a0 + w.flag4, (byte)(mm.ReadByte(reg.a0 + w.flag4) & 0x7f));
            if (a == 0)
            {
                mm.Write(reg.a0 + w.flag4, (byte)(mm.ReadByte(reg.a0 + w.flag4) | 0x40));
            }

        }

        //	同期信号待ち
        //		[$87]
        public void _COM_87()
        {
            int a = mm.ReadByte(reg.a5 + w.flag4) & 0x40;
            mm.Write(reg.a5 + w.flag4, (byte)(mm.ReadByte(reg.a5 + w.flag4) & 0xbf));
            if (a != 0) return;

            mm.Write(reg.a5 + w.dataptr, (uint)reg.a1);

            mm.Write(reg.a5 + w.flag4, (byte)(mm.ReadByte(reg.a5 + w.flag4) | 0x80));
            mm.Write(reg.a5 + w.len, (byte)1);
        }

        //	q 設定
        //[$90] + [DATA]b
        //				$1 ～ $10 まで[16段階]
        public void _COM_90()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.q, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xbf));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x20));

            if (mm.ReadByte(reg.a6 + dw.MND_VER) < 8)
            {
                reg.a0 = ab.dummyAddress;// _atq_old;
                mm.Write(reg.a5 + w.qtjob, reg.a0);//_atq_old = 0 とする
                if (ab.hlw_qtjob.ContainsKey(reg.a5)) ab.hlw_qtjob.Remove(reg.a5);
                ab.hlw_qtjob.Add(reg.a5, _atq_old);
                return;
            }

            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            //reg.D0_W = mm.ReadUInt16(_com_90_table);
            //reg.a0 = _com_90_table;
            reg.a0 = ab.dummyAddress;
            mm.Write(reg.a5 + w.qtjob, reg.a0);//d0(q:0x01-0x10)
            if (ab.hlw_qtjob.ContainsKey(reg.a5)) ab.hlw_qtjob.Remove(reg.a5);
            switch (reg.D0_W / 2)
            {
                case 1:
                    ab.hlw_qtjob.Add(reg.a5, _atq_01);
                    break;
                case 2:
                    ab.hlw_qtjob.Add(reg.a5, _atq_02);
                    break;
                case 3:
                    ab.hlw_qtjob.Add(reg.a5, _atq_03);
                    break;
                case 4:
                    ab.hlw_qtjob.Add(reg.a5, _atq_04);
                    break;
                case 5:
                    ab.hlw_qtjob.Add(reg.a5, _atq_05);
                    break;
                case 6:
                    ab.hlw_qtjob.Add(reg.a5, _atq_06);
                    break;
                case 7:
                    ab.hlw_qtjob.Add(reg.a5, _atq_07);
                    break;
                case 8:
                    ab.hlw_qtjob.Add(reg.a5, _atq_08);
                    break;
                case 9:
                    ab.hlw_qtjob.Add(reg.a5, _atq_09);
                    break;
                case 10:
                    ab.hlw_qtjob.Add(reg.a5, _atq_10);
                    break;
                case 11:
                    ab.hlw_qtjob.Add(reg.a5, _atq_11);
                    break;
                case 12:
                    ab.hlw_qtjob.Add(reg.a5, _atq_12);
                    break;
                case 13:
                    ab.hlw_qtjob.Add(reg.a5, _atq_13);
                    break;
                case 14:
                    ab.hlw_qtjob.Add(reg.a5, _atq_14);
                    break;
                case 15:
                    ab.hlw_qtjob.Add(reg.a5, _atq_15);
                    break;
                case 16:
                    ab.hlw_qtjob.Add(reg.a5, _atq_16);
                    break;

            }

        }

        public void _atq_01()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W = reg.D1_W >> 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_02()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W = reg.D1_W >> 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_03()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_04()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W >>= 2;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_05()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_06()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_07()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W <<= 3;
            reg.D1_W -= (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_08()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W >>= 1;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_09()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W <<= 3;
            reg.D1_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_10()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_11()
        {
            reg.D1_W = reg.D0_W;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = reg.D1_W;
            reg.D1_W <<= 4;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W -= (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_12()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W += (UInt32)(Int16)reg.D1_W;
            reg.D0_W += (UInt32)(Int16)reg.D1_W;
            reg.D1_W >>= 2;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_13()
        {
            reg.D1_W = reg.D0_W;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = reg.D1_W;
            reg.D1_W <<= 4;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D1_W -= (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_14()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W <<= 3;
            reg.D1_W -= (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_15()
        {
            reg.D1_W = reg.D0_W;
            reg.D1_W <<= 4;
            reg.D1_W -= (UInt32)(Int16)reg.D0_W;
            reg.D1_W >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D1_B);
        }
        public void _atq_16()
        {
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
        }

        public void _atq_old()
        {
            reg.D3_B = reg.D0_B;
            reg.D1_L = 0x10;
            reg.D2_L = 0;
            reg.D1_B -= mm.ReadByte(reg.a5 + w.q);
            if (reg.D1_B != 0)
            {
                reg.D3_B >>= 4;
                do
                {
                    reg.D2_B += (UInt32)(sbyte)reg.D3_B;
                } while (reg.D1_W-- != 0);
            }
            reg.D0_B -= (UInt32)(sbyte)reg.D2_B;
            mm.Write(reg.a5 + w.at_q, (byte)reg.D0_B);
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
        }

        //	@q 設定
        //			[$91] + [DATA]b
        public void _COM_91()
        {
            mm.Write(reg.a5 + w.at_q, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x40));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & 0xdf));
        }

        //	ネガティブ @q 設定
        //			[$93] + [DATA]b
        public void _COM_93()
        {
            mm.Write(reg.a5 + w.at_q, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x40));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x20));
        }

        //	キーオフモード
        //			[$94] + [switch]b
        public void _COM_94()
        {
            mm.Write(reg.a5 + w.kom, 0);
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0) mm.Write(reg.a5 + w.kom, (byte)0xff);
        }

        //	擬似リバーブ
        //		switch = $80 = ON
        //			 $81 = OFF
        //			 $82 = volume を直接指定にする
        //			 $83 = volume を相対指定にする
        //			 $84 = リバーブ動作は相対音量モードに依存する
        //			 $85 = リバーブ動作は常に @v 単位
        //
        //			 $00 = + [volume]b
        //			 $01 = + [volume]b + [pan]b
        //			 $02 = + [volume]b + [tone]b
        //			 $03 = + [volume]b + [panpot]b + [tone]b
        //			 $04 = + [volume]b ( 微調整 )
        //	work
        //		bit4 1:常に @v
        //		bit3 1:@v直接
        //		bit2 1:微調整
        //		bit1 1:音色変更
        //		bit0 1:定位変更
        //
        public void _COM_98()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B >= 0)
            {
                mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) & 0x10));
                reg.D0_B += 1;
                reg.D0_B += (UInt32)(sbyte)reg.D0_B;
                switch (reg.D0_B)
                {
                    case 2:
                        _COM_98_0();
                        break;
                    case 4:
                        _COM_98_1();
                        break;
                    case 6:
                        _COM_98_2();
                        break;
                    case 8:
                        _COM_98_3();
                        break;
                    case 10:
                        _COM_98_4();
                        break;
                }
                return;
            }

            reg.D0_B &= 0xf;
            reg.D0_B += 1;
            reg.D0_B += (UInt32)(sbyte)reg.D0_B;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_98_80();
                    break;
                case 4:
                    _COM_98_81();
                    break;
                case 6:
                    _COM_98_82();
                    break;
                case 8:
                    _COM_98_83();
                    break;
                case 10:
                    _COM_98_84();
                    break;
                case 12:
                    _COM_98_85();
                    break;
            }
        }

        public void _COM_98_80()
        {
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x80));
        }

        public void _COM_98_81()
        {
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) & 0x7f));
        }

        public void _COM_98_82()
        {
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x08));
        }

        public void _COM_98_83()
        {
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) & 0xf7));
        }

        public void _COM_98_84()
        {
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) & 0xef));
        }

        public void _COM_98_85()
        {
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x10));
        }

        // volume
        public void _COM_98_0()
        {
            mm.Write(reg.a5 + w.reverb_vol, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x80));
        }

        // volume + pan
        public void _COM_98_1()
        {
            mm.Write(reg.a5 + w.reverb_vol, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb_pan, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x81));
        }

        // volume + tone
        public void _COM_98_2()
        {
            mm.Write(reg.a5 + w.reverb_vol, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb_tone, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x82));
        }

        // volume + panpot + tone
        public void _COM_98_3()
        {
            mm.Write(reg.a5 + w.reverb_vol, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb_pan, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb_tone, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x83));
        }

        // volume
        public void _COM_98_4()
        {
            mm.Write(reg.a5 + w.reverb_vol, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.reverb, (byte)(mm.ReadByte(reg.a5 + w.reverb) | 0x84));
        }

        //	擬似エコー(廃止コマンド？)
        public void _COM_99()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B >= 0)
            {
                reg.D0_W += (UInt32)(Int16)reg.D0_W;
                return;
            }

            if (reg.D0_B == 0x80)
            {
                return;
            }

        }

        //	擬似動作 step time
        public void _COM_9A()
        {
            mm.Write(reg.a5 + w.reverb_time, mm.ReadByte(reg.a1++));
        }

        //	音量テーブル切り替え
        public void _COM_A3()
        {
            reg.D1_B = mm.ReadByte(reg.a1++);
            reg.D5_B = mm.ReadByte(reg.a1++);

            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.VOL_PTR);
            if (reg.D0_L == 0) return;

            reg.a2 = reg.D0_L;
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) | (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D2_L = reg.D0_L;
            if (reg.D2_L == 0) return;

            reg.D4_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;

            //_com_a3_ana_loop:
            while (true)
            {
                if (mm.ReadByte(reg.a2 + 2) == reg.D1_B)
                {
                    if (mm.ReadByte(reg.a2 + 3) == reg.D5_B)
                    {
                        _com_a3_set();
                        return;
                    }
                }
                reg.D4_W -= 1;
                if (reg.D4_W == 0)
                {
                    return;
                }
                reg.D0_W = mm.ReadUInt16(reg.a2); //reg.a2 += 1;
                reg.a2 = (reg.a2 + (UInt32)(Int16)reg.D0_W) & 0x00ffffff;
            }
        }

        public void _com_a3_set()
        {
            reg.a2 += 4;
            reg.D0_L = 0x7f;
            reg.D0_B &= mm.ReadByte(reg.a2);
            mm.Write(reg.a5 + w.volcount, (byte)reg.D0_B);
            reg.a2 += 2;// (reg.a2 & 0xffff0000) + (ushort)((ushort)reg.a2 + 2);

            reg.a0 = reg.a5 + w.voltable;
            do
            {
                mm.Write(reg.a0, mm.ReadByte(reg.a2)); reg.a0++; reg.a2++;
            } while (reg.D0_W-- != 0);
        }

        //	相対音量モード
        public void _COM_A8()
        {
            mm.Write(reg.a5 + w.volmode, mm.ReadByte(reg.a1++));
        }

        //	ドライバ動作モード変更
        public void _COM_B0()
        {
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_B0_0();
                    break;
                case 4:
                    _COM_B0_1();
                    break;
                case 6:
                    _COM_B0_2();
                    break;
                case 8:
                    _COM_B0_3();
                    break;
            }
            return;
        }

        public void _COM_B0_0()
        {
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0x7f));
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0) return;
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) | 0x80));
        }

        public void _COM_B0_1()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) & 0x7f));
                return;
            }
            reg.D0_B -= 1;
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) | 0x80));
                _COM_B0_start_timer_a();
                return;
            }
            reg.D0_B -= 1;
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) & 0xbf));
                return;
            }
            mm.Write(reg.a6 + dw.DRV_FLAG3, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG3) | 0x40));
            _COM_B0_start_timer_a();
        }

        public void _COM_B0_start_timer_a()
        {
            UInt32 sd = reg.D7_L;
            reg.D7_L = 3;
            reg.D1_L = 0x10;
            reg.D0_L = 0x1c;
            mndrv._OPN_WRITE();
            reg.D7_L = sd;
        }

        public void _COM_B0_2()
        {
            mm.Write(reg.a6 + dw.VOLMODE, 0);
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0) mm.Write(reg.a6 + dw.VOLMODE, (byte)0xff);
        }

        // PSG LFO MODE
        public void _COM_B0_3()
        {
            mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0xfc));
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0) return;
            reg.D0_B -= 1;
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) + 1));
                return;
            }
            reg.D0_B -= 1;
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a6 + dw.DRV_FLAG2, (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) + 2));
                return;
            }
        }



        //	トラックジャンプ
        public void _COM_BE()
        {
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) ^ 0x08));
        }

        //	フェードアウト
        public void _COM_BF_exit()
        {
            reg.a1++;
        }

        public void _COM_BF()
        {
            byte b = (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x10);
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) | 0x10));
            if (b != 0)
            {
                _COM_BF_exit();
                return;
            }
            mm.Write(reg.a6 + dw.FADESPEED, 1);
            mm.Write(reg.a6 + dw.FADESPEED_WORK, 3);

            b = (byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG) & 0x01);
            if (b != 0)
            {
                _COM_BF_no_opn();
                return;
            }
            UInt32 sd = reg.D7_L;
            reg.D7_L = 3;
            reg.D1_L = 0x10;
            reg.D0_L = 0x1c;
            mndrv._OPN_WRITE();
            reg.D7_L = sd;
            _COM_BF_no_opn();
        }

        public void _COM_BF_no_opn()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_BF_normal();
                return;
            }
            mm.Write(reg.a6 + dw.FADESPEED, (byte)reg.D0_B);
            mm.Write(reg.a6 + dw.FADESPEED_WORK, (byte)reg.D0_B);
        }

        public void _COM_BF_normal()
        {
            mm.Write(reg.a6 + dw.FADESPEED, 7);
            mm.Write(reg.a6 + dw.FADESPEED_WORK, 7);
        }

        //	ソフトウェアエンベロープ
        //		[$C0] + [SV]b + [AR]b + [DR]b + [SL]b + [SR]b + [RR]b
        public void _COM_C0()
        {
            mm.Write(reg.a5 + w.e_sv, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_ar, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_dr, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_sl, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_sr, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_rr, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_sub, (byte)0);
            mm.Write(reg.a5 + w.e_p, (byte)4);
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));
        }

        //	ソフトウェアエンベロープ 2
        //		[$C1] + [AL]b + [DD]b + [SR]b + [RR]b
        public void _COM_C1()
        {
            mm.Write(reg.a5 + w.e_al, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_dd, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_sr, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_rr, mm.ReadByte(reg.a1++));
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x81));
        }

        //	・ソフトウェアエンベロープスイッチ
        //		[$C3] + [switch]
        public void _COM_C3()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) & 0x7f));
                return;
            }
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));
        }

        //	・エンベロープ切り替え
        //		[$C4] + [num]
        public void _COM_C4()
        {
            reg.D5_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.envnum, (byte)reg.D5_B);
            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.ENV_PTR);
            if (reg.D0_L == 0) return;

            reg.a2 = reg.D0_L;
            reg.a2 += 6;
            reg.D4_W = mm.ReadUInt16(reg.a6 + dw.ENVNUM);
            if (reg.D4_W == 0) return;

            reg.D1_B = mm.ReadByte(reg.a5 + w.envbank);
            while (true)
            {
                if (mm.ReadByte(reg.a2 + 2) == reg.D1_B)
                {
                    if (mm.ReadByte(reg.a2 + 3) == reg.D5_B)
                    {
                        _COM_C4_set();
                        return;
                    }
                }
                reg.D4_W -= 1;
                if (reg.D4_W == 0) return;

                reg.D0_W = mm.ReadUInt16(reg.a2);
                reg.a2 = (reg.a2 + (UInt32)(Int16)reg.D0_W) & 0x00ffffff;
            }
        }

        public void _COM_C4_set()
        {
            reg.a2 += 4;
            mm.Write(reg.a5 + w.e_sv, mm.ReadByte(reg.a2 + 3));
            mm.Write(reg.a5 + w.e_ar, mm.ReadByte(reg.a2 + 0));
            mm.Write(reg.a5 + w.e_dr, mm.ReadByte(reg.a2 + 4));
            mm.Write(reg.a5 + w.e_sl, mm.ReadByte(reg.a2 + 6));
            mm.Write(reg.a5 + w.e_sr, mm.ReadByte(reg.a2 + 8));
            mm.Write(reg.a5 + w.e_rr, mm.ReadByte(reg.a2 + 12));
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));

            byte b = (byte)(mm.ReadByte(reg.a5 + w.flag) & 0x20);
            if (b != 0) return;

            mm.Write(reg.a5 + w.e_sub, 0);
            mm.Write(reg.a5 + w.e_p, 4);
        }

        //	・バンク&エンベロープ切り替え
        //		[$C5] + [bank] + [num]
        public void _COM_C5()
        {
            reg.D1_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.envbank, (byte)reg.D1_B);
            reg.D5_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.envbank, (byte)reg.D5_B);
            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.ENV_PTR);
            if (reg.D0_L == 0) return;
            reg.a2 = reg.D0_L;
            reg.a2 += 6;
            reg.D4_W = mm.ReadUInt16(reg.a6 + dw.ENVNUM);
            if (reg.D4_W == 0) return;
            while (true)
            {
                if (mm.ReadByte(reg.a2 + 2) == reg.D1_B)
                {
                    if (mm.ReadByte(reg.a2 + 3) == reg.D5_B)
                    {
                        _COM_C5_set();
                        return;
                    }
                }
                reg.D4_W -= 1;
                if (reg.D4_W == 0) return;
                reg.D0_W = mm.ReadUInt16(reg.a2);
                reg.a2 = (reg.a2 + (UInt32)(Int16)reg.D0_W) & 0x00ffffff;
            }
        }

        public void _COM_C5_set()
        {
            reg.a2 += 4;
            mm.Write(reg.a5 + w.e_sv, mm.ReadByte(reg.a2 + 3));
            mm.Write(reg.a5 + w.e_ar, mm.ReadByte(reg.a2 + 0));
            mm.Write(reg.a5 + w.e_dr, mm.ReadByte(reg.a2 + 4));
            mm.Write(reg.a5 + w.e_sl, mm.ReadByte(reg.a2 + 6));
            mm.Write(reg.a5 + w.e_sr, mm.ReadByte(reg.a2 + 8));
            mm.Write(reg.a5 + w.e_rr, mm.ReadByte(reg.a2 + 12));
            mm.Write(reg.a5 + w.e_sub, 0);
            mm.Write(reg.a5 + w.e_p, 4);
            mm.Write(reg.a5 + w.e_sw, (byte)(mm.ReadByte(reg.a5 + w.e_sw) | 0x80));
        }

        //	キートランスポーズ
        public void _COM_D0()
        {
            mm.Write(reg.a5 + w.key_trans, mm.ReadByte(reg.a1++));
        }

        //	相対キートランスポーズ
        public void _COM_D1()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.key_trans, (byte)((sbyte)mm.ReadByte(reg.a5 + w.key_trans) + (sbyte)reg.D0_B));
        }

        //	detune 設定
        public void _COM_D8()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a5 + w.detune, (UInt16)reg.D0_W);
        }

        //	detune 設定
        public void _COM_D9()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a5 + w.detune, (UInt16)((Int16)mm.ReadUInt16(reg.a5 + w.detune) + (Int16)reg.D0_W));
        }

        //	pitch LFO
        //
        //	$E2,num,wave,speed,count,delay,henka_w
        public void _COM_E2()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_E2_0();
                    break;
                case 4:
                    _COM_E2_1();
                    break;
                case 6:
                    _COM_E2_2();
                    break;
                case 8:
                    _COM_E2_3();
                    break;
            }
            return;
        }

        //	pitch LFO ALL
        public void _COM_E2_0()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0xe));
            reg.a0 = reg.a1;
            reg.a4 = w.p_pattern1 + reg.a5;
            reg.a3 = w.wp_pattern1 + reg.a5;
            _COM_E2_common();
            if ((Int32)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xfd));
            }
            reg.a1 = reg.a0;
            reg.a4 = w.p_pattern2 + reg.a5;
            reg.a3 = w.wp_pattern2 + reg.a5;
            _COM_E2_common();
            if ((Int32)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xfb));
            }
            reg.a1 = reg.a0;
            reg.a4 = w.p_pattern3 + reg.a5;
            reg.a3 = w.wp_pattern3 + reg.a5;
            _COM_E2_common();
            if ((Int32)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xf7));
            }
        }

        //	pitch LFO 1
        public void _COM_E2_1()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x02));

            reg.a4 = w.p_pattern1 + reg.a5;
            reg.a3 = w.wp_pattern1 + reg.a5;
            _COM_E2_common();
            if ((Int32)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xfd));
            }
        }

        //	pitch LFO 2
        public void _COM_E2_2()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x04));

            reg.a4 = w.p_pattern2 + reg.a5;
            reg.a3 = w.wp_pattern2 + reg.a5;
            _COM_E2_common();
            if ((Int32)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xfb));
            }
        }

        //	pitch LFO 3
        public void _COM_E2_3()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x08));

            reg.a4 = w.p_pattern3 + reg.a5;
            reg.a3 = w.wp_pattern3 + reg.a5;
            _COM_E2_common();
            if ((Int32)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xf7));
            }
        }

        //	LFO SET COMMON
        public void _COM_E2_common()
        {
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a3 + w_w.use_flag, (byte)0);
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.pattern, (byte)reg.D0_B);
            if ((sbyte)reg.D0_B < 0)
            {
                _COM_E2_wavememory();
                return;
            }
            if (reg.D0_B >= 0x10)
            {
                _COM_E2_compatible();
                return;
            }

            reg.D1_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D1_B);
            mm.Write(reg.a4 + w_l.count, mm.ReadByte(reg.a1++));
            reg.D0_B = mm.ReadByte(reg.a1++);

            if (reg.D0_B - 0xff != 0)
            {
                mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D0_B);
                reg.D1_B += (UInt32)(sbyte)reg.D0_B;
                mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D1_B);
            }
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;

            mm.Write(reg.a4 + w_l.henka, (UInt16)reg.D0_W);
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)reg.D0_W);
            reg.D0_B = mm.ReadByte(reg.a4 + w_l.count);
            reg.D0_B >>= 1;
            mm.Write(reg.a4 + w_l.count_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.flag, (UInt16)(mm.ReadUInt16(reg.a4 + w_l.flag) & 0xdfff));
            reg.D2_L = 0;
        }

        public void _COM_E2_compatible()
        {
            mm.Write(reg.a3 + w_w.use_flag, (byte)1);
            mm.Write(reg.a4 + w_l.pattern, (byte)0xff);

            reg.D0_B &= 7;
            reg.D1_B = reg.D0_B;
            reg.D1_B &= 3;
            reg.D2_B = reg.D1_B;
            reg.D1_B += 0x10;
            mm.Write(reg.a3 + w_w.type, (byte)reg.D1_B);

            mm.Write(reg.a4 + w_l.lfo_sp, (byte)mm.ReadByte(reg.a1++));

            reg.D1_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a3 + w_w.start, (UInt16)reg.D1_W);//周期1

            if (reg.D2_B != 0)
            {
                reg.D1_W >>= 1;
            }
            mm.Write(reg.a3 + w_w.loop_start, (UInt16)reg.D1_W);//周期2

            reg.D1_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            reg.D1_L = (UInt32)(Int16)reg.D1_W;
            reg.D1_L = (UInt32)((Int16)reg.D1_L << 8);
            if (reg.D0_B >= 4)
            {
                reg.D1_L = (UInt32)((Int16)reg.D1_L << 8);
            }
            mm.Write(reg.a3 + w_w.loop_end, reg.D1_L);//増減1
            if (reg.D2_B != 2)
            {
                reg.D1_L = 0;
            }
            mm.Write(reg.a3 + w_w.loop_count, reg.D1_L);//増減2
            reg.D2_L = 0;
        }

        public void _COM_E2_wavememory()
        {
            reg.D0_W &= 0x7f;
            _get_wave_memory_e2();

            reg.D1_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D1_B);
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a3 + w_w.depth, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.count, (byte)reg.D0_B);
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0xff)
            {
                mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D0_B);
                reg.D1_B += (UInt32)(sbyte)reg.D0_B;
                mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D1_B);
            }
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.flag, (UInt16)0);
        }

        public void _get_wave_memory_e2()
        {
            reg.D1_L = mm.ReadUInt32(reg.a6 + dw.WAVE_PTR);
            if (reg.D1_L == 0)
            {
                reg.D2_L = 0xffffffff;//-1;
                return;
            }
            reg.a2 = reg.D1_L;
            reg.D5_L = reg.D1_L;

            reg.D1_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D1_L = (reg.D1_L << 16) + (reg.D1_L >> 16);
            reg.D1_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            if (reg.D1_L == 0)
            {
                reg.D2_L = 0xffffffff;//-1;
                return;
            }

            reg.D1_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            if (reg.D1_W == 0)
            {
                reg.D2_L = 0xffffffff;//-1;
                return;
            }

            _com_e2_wm10:
            reg.D2_W = mm.ReadUInt16(reg.a2);
            reg.D2_L = (reg.D2_L << 16) + (reg.D2_L >> 16);
            reg.D2_W = mm.ReadUInt16(reg.a2 + 2);
            if (mm.ReadUInt16(reg.a2 + 4) != 0)
            {
                reg.a2 = (reg.a2 + reg.D2_L) & 0xffffff;
                if ((reg.D1_W--) != 0) goto _com_e2_wm10;
                reg.D2_L = 0xffffffff;//-1;
                return;
            }

            reg.a2 += 6;

            reg.D0_B = mm.ReadByte(reg.a2++);
            reg.D1_B = reg.D0_B;
            reg.D1_B &= 0xf;
            mm.Write(reg.a3 + w_w.type, (byte)reg.D1_B);
            reg.D0_B >>= 4;
            mm.Write(reg.a3 + w_w.ko_flag, (byte)reg.D0_B);

            reg.D0_B = mm.ReadByte(reg.a2++);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_w.start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_w.loop_start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_w.loop_end, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            mm.Write(reg.a3 + w_w.loop_count, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_w.ko_start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_w.ko_loop_start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_w.ko_loop_end, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L << 16) + (reg.D0_L >> 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            mm.Write(reg.a3 + w_w.ko_loop_count, reg.D0_L);

            mm.Write(reg.a3 + w_w.use_flag, (byte)0xff);
            reg.D2_L = 0;

        }

        //	pitch LFO on /off
        //
        //	$E3,num,switch
        public void _COM_E3()
        {
            reg.D0_L = 0;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D1_L = reg.D0_L;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 0:
                    _COM_E3_0();
                    break;
                case 2:
                    _COM_E3_1();
                    break;
                case 4:
                    _COM_E3_2();
                    break;
                case 6:
                    _COM_E3_3();
                    break;
            }
        }

        // bit15		0:enable 1:disable
        // bit1 keyoff	0:enable 1:disable
        // bit0 keyon	0:enable 1:disable
        // $80  = at keyon
        // $81  = at keyoff
        // $82  = always
        // $83  = async
        // $84  = stop & init
        //
        public void _COM_E3_0()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0xf1));
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0) return;
            if ((sbyte)reg.D0_B >= 0)
            {
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x0e));
                return;
            }

            if (reg.D0_B == 0x82)
            {
                reg.a4 = w.p_pattern1 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                reg.a4 = w.p_pattern2 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                reg.a4 = w.p_pattern3 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x0e));
                return;
            }
            if (reg.D0_B == 0x83)
            {
                reg.a4 = w.p_pattern1 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                reg.a4 = w.p_pattern2 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                reg.a4 = w.p_pattern3 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x0e));
                return;
            }
            if (reg.D0_B == 0x84)
            {
                comlfo._init_lfo2();
                return;
            }

            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f != 0)
            {
                reg.a4 = w.p_pattern1 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                reg.a4 = w.p_pattern2 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                reg.a4 = w.p_pattern3 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x0e));
                return;
            }

            reg.a4 = w.p_pattern1 + reg.a5;
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            reg.a4 = w.p_pattern2 + reg.a5;
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            reg.a4 = w.p_pattern3 + reg.a5;
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x0e));

        }

        public void _COM_E3_1()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_E3_OFF();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _COM_E3_ON();
                return;
            }
            reg.a4 = w.p_pattern1 + reg.a5;
            _COM_E3_common();
        }

        public void _COM_E3_2()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_E3_OFF();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _COM_E3_ON();
                return;
            }
            reg.a4 = w.p_pattern2 + reg.a5;
            _COM_E3_common();
        }

        public void _COM_E3_3()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_E3_OFF();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _COM_E3_ON();
                return;
            }
            reg.a4 = w.p_pattern3 + reg.a5;
            _COM_E3_common();
        }

        public void _COM_E3_common()
        {
            if (reg.D0_B == 0x82)
            {
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                _COM_E3_ON();
                return;
            }
            if (reg.D0_B == 0x83)
            {
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                _COM_E3_ON();
                return;
            }
            if (reg.D0_B == 0x84)
            {
                comlfo._init_lfo2();
                _COM_E3_OFF();
                return;
            }
            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f != 0)
            {
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
                _COM_E3_ON();
                return;
            }

            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
            _COM_E3_ON();
        }


        public void _COM_E3_ON()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            reg.D0_L |= (UInt32)(1 << (Int32)reg.D1_L);
            mm.Write(reg.a5 + w.lfo, (byte)reg.D0_B);
        }

        public void _COM_E3_OFF()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            reg.D0_L &= ~(UInt32)(1 << (Int32)reg.D1_L);
            mm.Write(reg.a5 + w.lfo, (byte)reg.D0_B);
        }

        //	pitch LFO delay
        //
        //	$E4,num,delay
        public void _COM_E4()
        {
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_E4_0();
                    break;
                case 4:
                    _COM_E4_1();
                    break;
                case 6:
                    _COM_E4_2();
                    break;
                case 8:
                    _COM_E4_3();
                    break;
            }
        }

        //	pitch LFO ALL
        public void _COM_E4_0()
        {
            reg.a0 = reg.a1;
            reg.a4 = w.p_pattern1 + reg.a5;
            _COM_E49_common();
            reg.a1 = reg.a0;
            reg.a4 = w.p_pattern2 + reg.a5;
            _COM_E49_common();
            reg.a1 = reg.a0;
            reg.a4 = w.p_pattern3 + reg.a5;
            _COM_E49_common();
        }

        //	pitch LFO 1
        public void _COM_E4_1()
        {
            reg.a4 = w.p_pattern1 + reg.a5;
            _COM_E49_common();
        }

        //	pitch LFO 2
        public void _COM_E4_2()
        {
            reg.a4 = w.p_pattern2 + reg.a5;
            _COM_E49_common();
        }

        //	pitch LFO 3
        public void _COM_E4_3()
        {
            reg.a4 = w.p_pattern3 + reg.a5;
            _COM_E49_common();
        }

        public void _COM_E49_common()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0xff)
            {
                _COM_E49_add();
                return;
            }
            mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D0_B);
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.lfo_sp);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
        }

        public void _COM_E49_add()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.keydelay);
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.lfo_sp);
            mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
        }

        //	音量 LFO
        //
        //	$E7,num,wave,delay,count,speed,henka_w
        public void _COM_E7()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_E7_0();
                    break;
                case 4:
                    _COM_E7_1();
                    break;
                case 6:
                    _COM_E7_2();
                    break;
                case 8:
                    _COM_E7_3();
                    break;
            }
        }

        //	音量 LFO
        public void _COM_E7_0()
        {
            reg.a0 = reg.a1;
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x70));
            reg.a4 = w.v_pattern1 + reg.a5;
            reg.a3 = w.wv_pattern1 + reg.a5;
            _COM_E7_common();
            reg.a1 = reg.a0;
            reg.a4 = w.v_pattern2 + reg.a5;
            reg.a3 = w.wv_pattern2 + reg.a5;
            _COM_E7_common();
            reg.a1 = reg.a0;
            reg.a4 = w.v_pattern3 + reg.a5;
            reg.a3 = w.wv_pattern3 + reg.a5;
            _COM_E7_common();
        }

        //	音量 LFO 1
        public void _COM_E7_1()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x10));
            reg.a4 = w.v_pattern1 + reg.a5;
            reg.a3 = w.wv_pattern1 + reg.a5;
            _COM_E7_common();
        }

        //	音量 LFO 2
        public void _COM_E7_2()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x20));
            reg.a4 = w.v_pattern2 + reg.a5;
            reg.a3 = w.wv_pattern2 + reg.a5;
            _COM_E7_common();
        }

        //	音量 LFO 3
        public void _COM_E7_3()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x40));
            reg.a4 = w.v_pattern3 + reg.a5;
            reg.a3 = w.wv_pattern3 + reg.a5;
            _COM_E7_common();
        }

        //	LFO SET COMMON
        public void _COM_E7_common()
        {
            mm.Write(reg.a4 + w_l.bendwork, (UInt16)0);
            mm.Write(reg.a3 + w_w.use_flag, (byte)0);
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.pattern, (byte)reg.D0_B);
            if ((sbyte)reg.D0_B < 0)
            {
                _COM_E2_wavememory();
                return;
            }
            if (reg.D0_B >= 0x10)
            {
                _COM_E7_compatible();
                return;
            }

            reg.D1_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)reg.D1_B);
            mm.Write(reg.a4 + w_l.count, mm.ReadByte(reg.a1++));
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B - 0xff != 0)
            {
                mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D0_B);
                reg.D1_B += (UInt32)(sbyte)reg.D0_B;
                mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D1_B);
            }
            reg.a1++;
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D0_W = (UInt16)(Int16)reg.D0_B; //byte to short cast(signed)
            mm.Write(reg.a4 + w_l.henka, (UInt16)reg.D0_W);
            mm.Write(reg.a4 + w_l.henka_work, (UInt16)reg.D0_W);
        }

        public void _COM_E7_compatible()
        {
            mm.Write(reg.a3 + w_w.use_flag, (byte)1);
            mm.Write(reg.a4 + w_l.pattern, (byte)0xff);
            reg.D0_B &= 7;
            reg.D1_B = reg.D0_B;
            reg.D1_B &= 3;
            reg.D2_B = reg.D1_B;
            reg.D1_B += 0x14;
            mm.Write(reg.a3 + w_w.type, (byte)reg.D1_B);
            mm.Write(reg.a4 + w_l.lfo_sp, (byte)mm.ReadByte(reg.a1++));
            reg.D1_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a3 + w_w.start, (UInt16)reg.D1_W);//周期
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            mm.Write(reg.a3 + w_w.loop_start, (UInt16)reg.D0_W);//増減

            uint f = reg.D2_B & 1;
            reg.D2_B >>= 1;
            if (f == 0)
            {
                reg.D0_L = (UInt32)((Int16)reg.D1_W * (Int16)reg.D0_W);
            }
            reg.D0_W = (UInt16)(-((Int16)reg.D0_W));
            if ((Int16)reg.D0_W < 0)
            {
                reg.D0_L = 0;
            }
            mm.Write(reg.a3 + w_w.loop_end, (UInt16)reg.D0_W);//最大振幅

            mm.Write(reg.a3 + w_w.ko_start, mm.ReadUInt16(reg.a3 + w_w.start));
            mm.Write(reg.a3 + w_w.ko_loop_start, mm.ReadUInt16(reg.a3 + w_w.loop_start));
            mm.Write(reg.a3 + w_w.ko_loop_end, mm.ReadUInt16(reg.a3 + w_w.loop_end));
            reg.D2_L = 0;
        }

        //	音量 LFO on /off
        //
        //	$E8,num,switch
        public void _COM_E8()
        {

            reg.D0_L = 0;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D1_L = reg.D0_L;
            reg.D1_B += 3;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 0:
                    _COM_E8_0();
                    break;
                case 2:
                    _COM_E8_1();
                    break;
                case 4:
                    _COM_E8_2();
                    break;
                case 6:
                    _COM_E8_3();
                    break;
            }
        }

        public void _COM_E8_0()
        {
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x8f));
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x70));
                return;
            }

            if (reg.D0_B == 0x82)
            {
                reg.a4 = w.v_pattern1 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                reg.a4 = w.v_pattern2 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                reg.a4 = w.v_pattern3 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x70));
                return;
            }
            if (reg.D0_B == 0x83)
            {
                reg.a4 = w.v_pattern1 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                reg.a4 = w.v_pattern2 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                reg.a4 = w.v_pattern3 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x70));
                return;
            }

            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f == 0)
            {
                reg.a4 = w.v_pattern1 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                reg.a4 = w.v_pattern2 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                reg.a4 = w.v_pattern3 + reg.a5;
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
                mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x70));
                return;
            }

            reg.a4 = w.v_pattern1 + reg.a5;
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            reg.a4 = w.v_pattern2 + reg.a5;
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            reg.a4 = w.v_pattern3 + reg.a5;
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) | 0x70));

        }

        public void _COM_E8_1()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_E8_OFF();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _COM_E8_ON();
                return;
            }
            reg.a4 = w.v_pattern1 + reg.a5;
            _COM_E8_common();
        }

        public void _COM_E8_2()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_E8_OFF();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _COM_E8_ON();
                return;
            }
            reg.a4 = w.v_pattern2 + reg.a5;
            _COM_E8_common();
        }

        public void _COM_E8_3()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                _COM_E8_OFF();
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                _COM_E8_ON();
                return;
            }
            reg.a4 = w.v_pattern3 + reg.a5;
            _COM_E8_common();
        }

        public void _COM_E8_common()
        {
            if (reg.D0_B == 0x82)
            {
                mm.Write(reg.a4 + w_l.flag, (UInt16)0);
                _COM_E8_ON();
                return;
            }
            if (reg.D0_B == 0x83)
            {
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x4000);
                _COM_E8_ON();
                return;
            }
            uint f = reg.D0_B & 1;
            reg.D0_B >>= 1;
            if (f == 0)
            {
                mm.Write(reg.a4 + w_l.flag, (UInt16)0x8001);
                _COM_E8_ON();
                return;
            }
            mm.Write(reg.a4 + w_l.flag, (UInt16)0x8002);
            _COM_E8_ON();
        }

        public void _COM_E8_ON()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            reg.D0_L |= (UInt32)(1 << (Int32)reg.D1_L);
            mm.Write(reg.a5 + w.lfo, (byte)reg.D0_B);
        }

        public void _COM_E8_OFF()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.lfo);
            reg.D0_L &= ~(UInt32)(1 << (Int32)reg.D1_L);
            mm.Write(reg.a5 + w.lfo, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	音量 LFO delay
        //
        public void _COM_E9()
        {
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_E9_0();
                    break;
                case 4:
                    _COM_E9_1();
                    break;
                case 6:
                    _COM_E9_2();
                    break;
                case 8:
                    _COM_E9_3();
                    break;
            }
        }

        //─────────────────────────────────────
        //	音量 LFO
        //
        public void _COM_E9_0()
        {
            reg.a0 = reg.a1;
            reg.a4 = w.v_pattern1 + reg.a5;
            _COM_E49_common();
            reg.a1 = reg.a0;
            reg.a4 = w.v_pattern2 + reg.a5;
            _COM_E49_common();
            reg.a1 = reg.a0;
            reg.a4 = w.v_pattern3 + reg.a5;
            _COM_E49_common();
        }

        //─────────────────────────────────────
        //	音量 LFO 1
        public void _COM_E9_1()
        {
            reg.a4 = w.v_pattern1 + reg.a5;
            _COM_E49_common();
        }

        //─────────────────────────────────────
        //	音量 LFO 2
        public void _COM_E9_2()
        {
            reg.a4 = w.v_pattern2 + reg.a5;
            _COM_E49_common();
        }

        //─────────────────────────────────────
        //	音量 LFO 3
        public void _COM_E9_3()
        {
            reg.a4 = w.v_pattern3 + reg.a5;
            _COM_E49_common();
        }

        //─────────────────────────────────────
        //	音量 LFO switch 2
        //
        public void _COM_EA()
        {
            reg.D0_L = 1;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _COM_EA_0();
                    break;
                case 4:
                    _COM_EA_1();
                    break;
                case 6:
                    _COM_EA_2();
                    break;
                case 8:
                    _COM_EA_3();
                    break;
            }
        }

        //─────────────────────────────────────
        //	音量 LFO
        //
        public void _COM_EA_0()
        {
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0)
            {
                reg.D0_B |= 0x80;
            }
            reg.a3 = w.wv_pattern1 + reg.a5;
            mm.Write(reg.a3 + w_w.slot, (byte)reg.D0_B);
            reg.a3 = w.wv_pattern2 + reg.a5;
            mm.Write(reg.a3 + w_w.slot, (byte)reg.D0_B);
            reg.a3 = w.wv_pattern3 + reg.a5;
            mm.Write(reg.a3 + w_w.slot, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	音量 LFO 1
        public void _COM_EA_1()
        {
            reg.a3 = w.wv_pattern1 + reg.a5;
            _COM_EA_common();
        }

        //─────────────────────────────────────
        //	音量 LFO 2
        public void _COM_EA_2()
        {
            reg.a3 = w.wv_pattern2 + reg.a5;
            _COM_EA_common();
        }

        //─────────────────────────────────────
        //	音量 LFO 3
        public void _COM_EA_3()
        {
            reg.a3 = w.wv_pattern3 + reg.a5;
            _COM_EA_common();
        }

        public void _COM_EA_common()
        {
            reg.D0_L = 0;
            reg.D0_B += mm.ReadByte(reg.a1++);
            if (reg.D0_B != 0)
            {
                reg.D0_B |= 0x80;
            }
            mm.Write(reg.a3 + w_w.slot, (byte)reg.D0_B);
        }

        //─────────────────────────────────────
        //	わうわう
        //
        public void _COM_EB()
        {
            reg.a4 = w.ww_pattern1 + reg.a5;

            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) & 0xdf));
                return;
            }
            if ((sbyte)reg.D0_B >= 0)
            {
                mm.Write(reg.a4 + w_ww.delay, mm.ReadByte(reg.a1++));
                mm.Write(reg.a4 + w_ww.speed, mm.ReadByte(reg.a1++));
                mm.Write(reg.a4 + w_ww.rate, mm.ReadByte(reg.a1++));
                mm.Write(reg.a4 + w_ww.depth, mm.ReadByte(reg.a1++));
                mm.Write(reg.a4 + w_ww.slot, mm.ReadByte(reg.a1++));
            }
            mm.Write(reg.a4 + w_ww.sync, (byte)0);
            mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) | 0x20));

            reg.D0_B = mm.ReadByte(reg.a4 + w_ww.speed);
            reg.D0_B += mm.ReadByte(reg.a4 + w_ww.delay);
            mm.Write(reg.a4 + w_ww.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a4 + w_ww.rate_work, mm.ReadByte(reg.a4 + w_ww.rate));
            mm.Write(reg.a4 + w_ww.depth_work, mm.ReadByte(reg.a4 + w_ww.depth));
            mm.Write(reg.a4 + w_ww.work, (byte)0);

            if ((sbyte)mm.ReadByte(reg.a4 + w_ww.slot) < 0)
            {
                mm.Write(reg.a4 + w_ww.sync, (byte)0xff);
            }


        }

        //─────────────────────────────────────
        //	wavememory effect
        //
        //	[$ED] + [num]b + [switch]b ...
        //	num
        //		$00 ～ $03
        //	switch
        //	minus
        //		$80 = ON
        //		$81 = OFF
        //		$82 = always
        //		$83 = at keyon
        //		$84 = at keyoff
        //	plus = + [switch2]b + [wave]w + [delay]b + [speed]b + [sync]b + [reset]w
        //	$00 = y command
        //		波形データの上位バイトのレジスタに
        //		下位バイトのデータを書き込む
        //	$01 = tone
        //	$02 = panpot
        //
        public void _COM_ED()
        {
            reg.D0_L = 0;
            reg.D0_B += mm.ReadByte(reg.a1++);
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _com_ed_1();
                    break;
                case 4:
                    _com_ed_2();
                    break;
                case 6:
                    _com_ed_3();
                    break;
                case 8:
                    _com_ed_4();
                    break;
            }
        }

        public void _com_ed_1()
        {
            reg.a3 = w.we_pattern1 + reg.a5;
            mm.Write(reg.a3 + w_we.exec, (byte)0);
            reg.D1_L = 0;
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B < 0)
            {
                _com_ed_sw_common();
                return;
            }

            if ((sbyte)_com_ed_common() < 0)
            {
                mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) & 0xfe));
                return;
            }
            mm.Write(reg.a5 + w.effect, (byte)(mm.ReadByte(reg.a5 + w.effect) | 0x81));
        }

        public void _com_ed_2()
        {
            reg.a3 = w.we_pattern2 + reg.a5;
            mm.Write(reg.a3 + w_we.exec, (byte)0);
            reg.D1_L = 1;
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B < 0)
            {
                _com_ed_sw_common();
                return;
            }

            _com_ed_common();
            if ((sbyte)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) & 0xfd));
                return;
            }
            mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) | 0x82));
        }

        public void _com_ed_3()
        {
            reg.a3 = w.we_pattern3 + reg.a5;
            mm.Write(reg.a3 + w_we.exec, (byte)0);
            reg.D1_L = 2;
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B < 0)
            {
                _com_ed_sw_common();
                return;
            }

            _com_ed_common();
            if ((sbyte)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) & 0xfb));
                return;
            }
            mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) | 0x84));
        }

        public void _com_ed_4()
        {
            reg.a3 = w.we_pattern4 + reg.a5;
            mm.Write(reg.a3 + w_we.exec, (byte)0);
            reg.D1_L = 3;
            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if ((sbyte)reg.D0_B < 0)
            {
                _com_ed_sw_common();
                return;
            }

            _com_ed_common();
            if ((sbyte)reg.D2_L < 0)
            {
                mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) & 0xf7));
                return;
            }
            mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) | 0x88));
        }

        public void _com_ed_sw_common()
        {
            reg.D0_W &= 0x7;
            reg.D0_W += 1;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            switch (reg.D0_B)
            {
                case 2:
                    _com_ed_80();
                    break;
                case 4:
                    _com_ed_81();
                    break;
                case 6:
                    _com_ed_82();
                    break;
                case 8:
                    _com_ed_83();
                    break;
                case 10:
                    _com_ed_84();
                    break;
            }
        }

        //
        //	ON
        //
        public void _com_ed_80()
        {
            reg.D0_L = 1;
            reg.D0_L <<= (int)reg.D1_L;
            reg.D0_B |= 0x80;
            mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) | reg.D0_B));
            reg.D0_L = 0;
        }

        //
        //	OFF
        //
        public void _com_ed_81()
        {
            reg.D0_L = 1;
            reg.D0_L <<= (int)reg.D1_L;
            reg.D0_B = ~reg.D0_B;
            mm.Write(reg.a5 + w.weffect, (byte)(mm.ReadByte(reg.a5 + w.weffect) & reg.D0_B));

            reg.a0 = reg.a5 + w.we_ycom_adrs;
            reg.D0_W &= 3;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.a0 = reg.a0 + (UInt32)(Int16)reg.D0_W;
            reg.a0 = mm.ReadUInt32(reg.a0);
            reg.D0_W = mm.ReadUInt16(reg.a3 + w_we.reset);
            ab.hlw_we_ycom_adrs[reg.a0]();
            reg.D0_L = 0;
        }

        //
        //	always
        //
        public void _com_ed_82()
        {
            mm.Write(reg.a3 + w_we.exec, (byte)0);
            reg.D0_L = 0;
        }

        //
        //	at keyon
        //
        public void _com_ed_83()
        {
            mm.Write(reg.a3 + w_we.exec, (byte)0x81);
            reg.D0_L = 0;
        }

        //
        //	at keyoff
        //
        public void _com_ed_84()
        {
            mm.Write(reg.a3 + w_we.exec, (byte)0x82);
            reg.D0_L = 0;
        }

        public UInt32 _com_ed_common()
        {
            reg.a0 = reg.a5 + w.we_ycom_adrs;
            reg.D0_W &= 3;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.D0_W += (UInt32)(Int16)reg.D0_W;
            reg.a0 = mm.ReadUInt32(reg.a0 + (UInt32)(Int16)reg.D0_W);
            reg.a0 = mm.ReadUInt32(reg.a0);
            mm.Write(reg.a3 + w_we.exec_adrs, reg.a0);
            ab.hlw_we_exec_adrs.Add(reg.a0, null);

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);

            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            _get_wave_memory_ed();

            mm.Write(reg.a3 + w_we.delay, mm.ReadByte(reg.a1++));
            reg.D1_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a3 + w_we.speed, (byte)reg.D1_B);

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            mm.Write(reg.a3 + w_we.mode, _com_ed_sync_table[reg.D0_W]);

            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            reg.D0_W = mm.ReadUInt16(reg.a3 + w_we.reset);

            mm.Write(reg.a3 + w_we.exec_flag, (byte)0x00);
            mm.Write(reg.a3 + w_we.loop_flag, (byte)0x00);
            reg.D0_B = mm.ReadByte(reg.a3 + w_we.delay);
            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            mm.Write(reg.a3 + w_we.delay_work, (byte)reg.D0_B);
            mm.Write(reg.a3 + w_we.adrs_work, mm.ReadUInt32(reg.a3 + w_we.start));
            mm.Write(reg.a3 + w_we.start_adrs_work, mm.ReadUInt32(reg.a3 + w_we.loop_start));
            mm.Write(reg.a3 + w_we.end_adrs_work, mm.ReadUInt32(reg.a3 + w_we.loop_end));
            mm.Write(reg.a3 + w_we.lp_cnt_work, mm.ReadUInt32(reg.a3 + w_we.loop_count));
            reg.D0_L = 0;

            return reg.D0_L;
        }

        private byte[] _com_ed_sync_table = new byte[]
        {
            0x80,0x00,0x01,0x02
        };

        public void _get_wave_memory_ed()
        {
            reg.D0_L = mm.ReadUInt32(reg.a6 + dw.WAVE_PTR);
            if (reg.D0_L == 0)
            {
                _com_ed_wm_err_exit();
                return;
            }
            reg.a2 = reg.D1_L;
            reg.D5_L = reg.D1_L;

            reg.D1_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D1_L = (reg.D1_L >> 16) + (reg.D1_L << 16);
            reg.D1_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            if (reg.D1_L == 0)
            {
                _com_ed_wm_err_exit();
                return;
            }

            reg.D1_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            if (reg.D1_L == 0)
            {
                _com_ed_wm_err_exit();
            }
        }

        public void _com_ed_wm10()
        {
            do
            {
                reg.D2_W = mm.ReadUInt16(reg.a2);
                reg.D2_L = (reg.D2_L >> 16) + (reg.D2_L << 16);
                reg.D2_W = mm.ReadUInt16(reg.a2 + 2);
                if (reg.D0_W - mm.ReadUInt16(reg.a2 + 4) == 0)
                {
                    _com_ed_wm20();
                    return;
                }
                reg.a2 = mm.ReadUInt32((UInt32)(reg.a2 + (Int32)reg.D2_L));
            } while (reg.D1_W-- != 0);
            _com_ed_wm_err_exit();
        }

        public void _com_ed_wm20()
        {
            reg.a2 += 6;

            reg.D0_B = mm.ReadByte(reg.a2++);
            reg.D1_B = reg.D0_B;
            reg.D1_B &= 0xf;
            mm.Write(reg.a3 + w_we.count, (byte)reg.D1_B);
            reg.D0_B >>= 4;
            mm.Write(reg.a3 + w_we.ko_flag, (byte)reg.D1_B);

            reg.D0_B = mm.ReadByte(reg.a2++);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_we.start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_we.loop_start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_we.loop_end, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            mm.Write(reg.a3 + w_we.loop_count, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_we.ko_start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_we.ko_loop_start, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L += (UInt32)(Int32)reg.D5_L;
            mm.Write(reg.a3 + w_we.ko_loop_end, reg.D0_L);

            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            reg.D0_L = (reg.D0_L >> 16) + (reg.D0_L << 16);
            reg.D0_W = mm.ReadUInt16(reg.a2); reg.a2 += 2;
            mm.Write(reg.a3 + w_we.ko_loop_count, reg.D0_L);
        }

        public void _com_ed_wm_err_exit()
        {
            reg.D2_L = 0xffffffff;// -1;
        }

        //─────────────────────────────────────
        //	hardware LFO delay
        //		[$EF] + [delay]b
        //
        public void _COM_EF()
        {
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x02));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x04));
            reg.a4 = reg.a5 + w.v_pattern4;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B - 0xff == 0)
            {
                _COM_EF_add();
                return;
            }

            mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D0_B);
            reg.D0_B += mm.ReadByte(reg.a4 + w_l.lfo_sp);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D0_B);
        }

        public void _COM_EF_add()
        {
            reg.D0_B = mm.ReadByte(reg.a1++);
            reg.D1_B = mm.ReadByte(reg.a4 + w_l.keydelay);
            reg.D1_B += (UInt32)(sbyte)reg.D0_B;
            reg.D1_B += mm.ReadByte(reg.a4 + w_l.lfo_sp);
            mm.Write(reg.a4 + w_l.keydelay, (byte)reg.D1_B);
            mm.Write(reg.a4 + w_l.delay_work, (byte)reg.D1_B);
        }

        //─────────────────────────────────────
        //	永久ループポイントマーク
        //			[$F9]
        public void _COM_F9()
        {
            mm.Write(reg.a5 + w.loop, reg.a1);
        }

        //─────────────────────────────────────
        //	リピート抜け出し
        //			[$FB] + [終端コマンドへのオフセット]w
        public void _COM_FB()
        {
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            reg.a0 = reg.a1 + (UInt32)(Int16)reg.D0_W + 1;
            reg.D0_W = mm.ReadUInt16(reg.a0); reg.a0 += 2;
            if ((mm.ReadByte(reg.a0 + (UInt32)(Int16)reg.D0_W + 2) - 1) != 0) return;
            reg.a1 = reg.a0;
        }

        //─────────────────────────────────────
        //	リピート開始
        //			[$FC] + [リピート回数]b + [$00]b
        public void _COM_FC()
        {
            mm.Write(reg.a1 + 1, mm.ReadByte(reg.a1));
            reg.a1 += 2;
        }

        //─────────────────────────────────────
        //	リピート終端
        //			[$FD] + [開始コマンドへのオフセット]w
        public void _COM_FD()
        {
            reg.D0_W = mm.ReadUInt16(reg.a1); reg.a1 += 2;
            if (mm.ReadByte(reg.a1 + (UInt32)(Int16)reg.D0_W + 1) == 0)
            {
                reg.a1 = reg.a1 + (UInt32)(Int16)reg.D0_W + 3;
                return;
            }
            mm.Write(reg.a1 + (UInt32)(Int16)reg.D0_W + 2, (byte)(mm.ReadByte(reg.a1 + (UInt32)(Int16)reg.D0_W + 2) - 1));
            if (mm.ReadByte(reg.a1 + (UInt32)(Int16)reg.D0_W + 2) != 0)
            {
                reg.a1 = reg.a1 + (UInt32)(Int16)reg.D0_W + 3;
            }
        }

        //─────────────────────────────────────
        //	tempo 設定
        //
        public void _COM_FE()
        {
            mm.Write(reg.a6 + dw.TEMPO, mm.ReadByte(reg.a1++));
        }

        //─────────────────────────────────────
        public void _all_end_check()
        {
            reg.D0_W = mm.ReadUInt16(reg.a6 + dw.USE_TRACK);
            reg.a0 = reg.a6 + dw.TRACKWORKADR;
            do
            {
                if ((sbyte)mm.ReadByte(reg.a0 + w.flag) < 0)
                {
                    return;
                }
                reg.a0 = reg.a0 + w._track_work_size;// dw._work_size;
                reg.D0_W--;
            } while (reg.D0_W != 0);
            mm.Write(reg.a6 + dw.DRV_STATUS, (byte)0x20);
            mm.Write(reg.a6 + dw.LOOP_COUNTER, (UInt16)0xffff);// -1
            reg.D0_L = 2;
            mndrv.SUBEVENT();
        }


    }
}