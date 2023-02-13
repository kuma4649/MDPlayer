using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class comanalyze
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public ab ab;

        //
        //	part of track analyze
        //

        //─────────────────────────────────────
        public void _track_ana_quit()
        {
            //	rts
        }

        public void _track_analyze()
        {

            if ((sbyte)mm.ReadByte(reg.a5 + w.flag4) < 0)
            {
                return;
            }

            if ((sbyte)mm.ReadByte(reg.a5 + w.flag) >= 0)
            {
                return;
            }

            if ((sbyte)mm.ReadByte(reg.a5 + w.reverb) >= 0)
            {
                _track_ana_normal();
                return;
            }

            //─────────────────────────────────────
            reg.D4_B = mm.ReadByte(reg.a5 + w.len);
            reg.D4_B -= 1;

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x40) != 0)
            {
                _track_echo_next();
                return;
            }

            reg.D0_L = 3;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.effect);
            if (reg.D0_B - 2 != 0)
            {
                _track_ana_echo_atq();
                return;
            }

            if (reg.D4_B - mm.ReadByte(reg.a5 + w.rct) != 0)
            {
                _track_ana_echo_atq();
                return;
            }
            reg.a0 = mm.ReadUInt32(reg.a5 + w.rrcut_adrs);
            ab.hlw_rrcut_adrs[reg.a5]();
            _track_ana_echo_atq();
        }

        public void _track_ana_echo_atq()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.at_q_work);
            if (reg.D0_B == 0) goto L1;
            reg.D0_B -= 1;
            if (reg.D0_B != 0) goto L2;
            L1:
            reg.a0 = mm.ReadUInt32(reg.a5 + w.echo_adrs);
            ab.hlw_echo_adrs[reg.a5]();
            reg.D0_L = 0xffffffff;// -1;
            L2:
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
            _track_echo_next();
        }

        public void _track_echo_next()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.reverb_time_work);
            if (reg.D0_B != 0)
            {
                reg.D0_B -= 1;
                if (reg.D0_B == 0)
                {
                    reg.a0 = mm.ReadUInt32(reg.a5 + w.keyoff_adrs2);
                    ab.hlw_keyoff_adrs2[reg.a5]();
                    reg.D0_L = 0;
                }
                mm.Write(reg.a5 + w.reverb_time_work, (byte)reg.D0_B);
            }

            mm.Write(reg.a5 + w.len, (byte)reg.D4_B);
            if (reg.D4_B != 0)
            {
                return;
            }

            if ((mm.ReadByte(reg.a5 + w.flag) & 0x40) != 0)
            {
                _track_ana_fetch();
                return;
            }

            reg.a0 = mm.ReadUInt32(reg.a5 + w.echo_adrs);
            ab.hlw_echo_adrs[reg.a5]();
            _track_ana_fetch();
        }

        //─────────────────────────────────────
        public void _track_ana_normal()
        {
            reg.D4_B = mm.ReadByte(reg.a5 + w.len);
            reg.D4_B -= 1;
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x40) != 0)
            {
                _track_ana_next();
                return;
            }
            reg.D0_L = 3;
            reg.D0_B &= mm.ReadByte(reg.a5 + w.effect);
            if (reg.D0_B - 2 != 0)
            {
                _track_ana_normal_atq();
                return;
            }
            if (reg.D4_B - mm.ReadByte(reg.a5 + w.rct) != 0)
            {
                _track_ana_normal_atq();
                return;
            }
            reg.a0 = mm.ReadUInt32(reg.a5 + w.rrcut_adrs);
            ab.hlw_rrcut_adrs[reg.a5]();
            _track_ana_normal_atq();
        }

        public void _track_ana_normal_atq()
        {
            reg.D0_B = mm.ReadByte(reg.a5 + w.at_q_work);
            if (reg.D0_B == 0) goto L1;
            reg.D0_B -= 1;
            if (reg.D0_B != 0) goto L2;
            L1:
            reg.a0 =mm.ReadUInt32(reg.a5 + w.keyoff_adrs);
            ab.hlw_keyoff_adrs[reg.a5]();
            reg.D0_L = 0xffffffff;// -1;
            L2:
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
            _track_ana_next();
        }

        public void _track_ana_next()
        {
            mm.Write(reg.a5 + w.len, (byte)reg.D4_B);
            if (reg.D4_B != 0)
            {
                _track_ana_quit();
                return;
            }
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x40) == 0) goto L1;

            reg.D0_L = 6;
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & (~(1 << (int)reg.D0_B))));
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & (~(1 << (int)reg.D0_B))));
            reg.a0 = mm.ReadUInt32(reg.a5 + w.keyoff_adrs);
            ab.hlw_keyoff_adrs[reg.a5]();
            _track_ana_fetch();
            return;

            L1:
            if ((mm.ReadByte(reg.a5 + w.flag) & 0x40) != 0)
            {
                _track_ana_fetch();
                return;
            }
            reg.a0 = mm.ReadUInt32(reg.a5 + w.keyoff_adrs);
            ab.hlw_keyoff_adrs[reg.a5]();
            _track_ana_fetch();
        }

        //─────────────────────────────────────
        public void _track_ana_fetch()
        {
            ///HACK: (MNDRV) トラックフェッチ
            mm.Write(reg.a5 + w.lfo, (byte)(mm.ReadByte(reg.a5 + w.lfo) & 0x7f));
            reg.a1 = mm.ReadUInt32(reg.a5 + w.dataptr);
            //goto _track_ana_fetch_L1;
            bool dmyFlg = true;

            _track_loop:

            do
            {
                if (!dmyFlg)
                {
                    if ((sbyte)mm.ReadByte(reg.a5 + w.flag4) < 0)
                    {
                        return;
                    }
                    if ((sbyte)mm.ReadByte(reg.a5 + w.flag) >= 0)
                    {
                        return;
                    }
                }

                //L1:

                dmyFlg = false;
                reg.D0_L = 0;
                reg.D0_B = mm.ReadByte(reg.a1++);
                if ((sbyte)reg.D0_B >= 0) goto _track_ana_mml;

                reg.D0_B -= 0x80;
                if (reg.D0_B == 0) goto _track_ana_rest_exit;

                reg.a0 = mm.ReadUInt32(reg.a5 + w.subcmd_adrs);
                ab.hlw_subcmd_adrs[reg.a5]();
            } while (true);
            //_track_loop();


            _track_ana_mml:

            if ((byte)(mm.ReadByte(reg.a6 + dw.DRV_STATUS) & 0x8) != 0) goto _track_ana_exit_jump;

            reg.D1_B = mm.ReadByte(reg.a5 + w.key_trans);
            if ((sbyte)reg.D0_B >= 0) goto _track_ana_mml_plus;

            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D0_B >= 0) goto _track_ana_mml_;
            reg.D0_L = 0;
            goto _track_ana_mml_;


            _track_ana_mml_plus:

            reg.D0_B += (UInt32)(sbyte)reg.D1_B;
            if ((sbyte)reg.D0_B >= 0) goto _track_ana_mml_;
            reg.D0_L = 0x7f;


            _track_ana_mml_:

            if ((byte)(mm.ReadByte(reg.a5 + w.flag3) & 0x40) != 0) goto _track_ana_mml1;
            reg.D1_B = mm.ReadByte(reg.a5 + w.flag);
            if ((reg.D1_L & 0x40) == 0) goto _track_ana_mml1;
            if ((reg.D1_L & 0x02) != 0) goto _track_ana_mml1;
            if (reg.D0_B - mm.ReadByte(reg.a5 + w.key) == 0) goto _track_ana_exit;
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x1));

            _track_ana_mml1:
            reg.a0 = mm.ReadUInt32(reg.a5 + w.setnote_adrs);
            ab.hlw_setnote_adrs[reg.a5]();

            _track_ana_exit:


            reg.a0 = mm.ReadUInt32(reg.a5 + w.inithlfo_adrs);
            ab.hlw_inithlfo_adrs[reg.a5]();


            _track_ana_exit_jump:

            reg.D0_L = 0;
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0)
            {
                goto _track_loop;
            }
            mm.Write(reg.a5 + w.len, (byte)reg.D0_B);
            if ((byte)(mm.ReadByte(reg.a5 + w.flag2) & 0x40) != 0)
            {
                _track_ana_exit_atq();
                return;
            }

            reg.a0 = mm.ReadUInt32(reg.a5 + w.qtjob);
            ab.hlw_qtjob[reg.a5]();
            _track_ana_exit_();

            return;


            _track_ana_rest_exit:

            reg.D0_B = mm.ReadByte(reg.a6 + dw.DRV_FLAG2);
            if ((sbyte)reg.D0_B < 0) goto L1;
            if ((reg.D0_L & 0x40) != 0) goto L1;
            reg.a0 = mm.ReadUInt32(reg.a5 + w.keyoff_adrs);
            ab.hlw_keyoff_adrs[reg.a5]();
            L1:
            reg.D0_B = mm.ReadByte(reg.a1++);
            if (reg.D0_B == 0) goto _track_loop;
            mm.Write(reg.a5 + w.len, (byte)reg.D0_B);
            reg.a0 = mm.ReadUInt32(reg.a6 + dw.TRKANA_RESTADR);
            ab.hlTRKANA_RESTADR[reg.a6]();//a6の位置にあるActionを実行
            return;
        }

        public void _track_ana_exit_atq()
        {
            if ((byte)(mm.ReadByte(reg.a6 + dw.DRV_FLAG2) & 0x10) != 0)
            {
                _track_ana_exit_atq_new();
                return;
            }
            reg.D0_B -= mm.ReadByte(reg.a5 + w.at_q);
            if (reg.D0_B == 0)
            {
                reg.D0_L = 0xffffffff;//-1
            }
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
            _track_ana_exit_atq_final();
        }

        public void _track_ana_exit_atq_new()
        {
            reg.D1_L = 0;
            reg.D1_B = mm.ReadByte(reg.a5 + w.at_q);
            reg.D0_W -= (UInt32)(Int16)reg.D1_W;
            if ((Int16)reg.D0_W > 0)
            {
                mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
            }
            else
            {
                mm.Write(reg.a5 + w.at_q_work, (byte)1);
            }
            _track_ana_exit_atq_final();
        }

        public void _track_ana_exit_atq_final()
        {
            if ((byte)(mm.ReadByte(reg.a5 + w.flag3) & 0x20) != 0)
            {
                mm.Write(reg.a5 + w.at_q_work, mm.ReadByte(reg.a5 + w.at_q));
            }
            _track_ana_exit_();
        }

        public void _track_ana_exit_()
        {
            if (mm.ReadByte(reg.a1) - 0x81 != 0)
            {
                if ((byte)(mm.ReadByte(reg.a5 + w.flag3) & 0x40) == 0)
                {
                    mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xbf));
                }
                mm.Write(reg.a5 + w.dataptr, reg.a1);
                return;
            }
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x40));
            reg.a1 += 1;
            mm.Write(reg.a5 + w.dataptr, reg.a1);
        }

        public void _track_ana_rest_old()
        {
            if ((byte)(mm.ReadByte(reg.a5 + w.flag2) & 0x40) == 0)
            {
                mm.Write(reg.a5 + w.at_q, (byte)0);
            }
            if (mm.ReadByte(reg.a1) - 0x81 != 0)
            {
                if ((byte)(mm.ReadByte(reg.a5 + w.flag3) & 0x40) == 0)
                {
                    mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xbf));
                }
                mm.Write(reg.a5 + w.dataptr, reg.a1);
                return;
            }
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x40));
            reg.a1 += 1;
            mm.Write(reg.a5 + w.dataptr, reg.a1);
        }

        public void _track_ana_rest_new()
        {
            if ((byte)(mm.ReadByte(reg.a5 + w.flag2) & 0x40) == 0)
            {
                mm.Write(reg.a5 + w.at_q, (byte)reg.D0_B);
                mm.Write(reg.a5 + w.at_q_work, (byte)reg.D0_B);
            }
            if (mm.ReadByte(reg.a1) - 0x81 != 0)
            {
                if ((byte)(mm.ReadByte(reg.a5 + w.flag3) & 0x40) == 0)
                {
                    mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) & 0xbf));
                }
                mm.Write(reg.a5 + w.dataptr, reg.a1);
                return;
            }
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x40));
            reg.a1 += 1;
            mm.Write(reg.a5 + w.dataptr, reg.a1);
        }



    }
}
