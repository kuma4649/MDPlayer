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
        public MXDRV.X68REG reg;
        public MXDRV.xMemory mm;

        //	タイ
        //		[$81]
        public void _COM_81()
        {
            //	    btst.b	#6,w_flag3(a5)
            //	    bne 	1f
            //      bclr.b	#6,w_flag(a5)
            //1:	rts
            if ((mm.ReadByte(reg.a5 + w.flag3) & 0x40) != 0) return;
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x40));
        }

        //	スラー
        //		[$83]
        public void _COM_83()
        {
            //	bset.b	#6,w_flag(a5)
            //	bset.b	#6,w_flag3(a5)
            //	rts
            mm.Write(reg.a5 + w.flag, (byte)(mm.ReadByte(reg.a5 + w.flag) | 0x40));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x40));
        }

        //	同期信号送信
        //		[$86] + [track] b
        public void _COM_86()
        {
            //	moveq.l	#0,d0
            //	lea.l   TRACKWORKADR(a6), a0
            reg.d0 = 0;
            reg.a0 = dw.TRACKWORKADR + reg.a6;

            //    move.b  (a1)+, d0
            reg.d0 = mm.ReadByte(reg.a1++);
            while (true)
            {
                //1:	subq.w	#1,d0
                //	    beq	2f
                //      lea.l _track_work_size(a0), a0
                //      bra	1b
                reg.d0--;
                if (reg.d0 == 0) break;

                reg.a0 = w._work_size + reg.a0;
            }
            //2:

            //  bclr.b	#7,w_flag4(a0)
            //	bne	1f
            //  bset.b	#6,w_flag4(a0)
            //1:
            //  rts
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
            //	bclr.b	#6,w_flag4(a5)
            //	bne	9f
            int a = mm.ReadByte(reg.a5 + w.flag4) & 0x40;
            mm.Write(reg.a5 + w.flag4, (byte)(mm.ReadByte(reg.a5 + w.flag4) & 0xbf));
            if (a != 0) return;

            //    move.l a1, w_dataptr(a5)
            mm.Write(reg.a5 + w.dataptr, (uint)reg.a1);

            //  ori.b	#$80,w_flag4(a5)
            //	move.b	#1,w_len(a5)
            mm.Write(reg.a6 + w.flag4, (byte)(mm.ReadByte(reg.a6 + w.flag4) | 0x80));
            mm.Write(reg.a5 + w.len, (byte)1);
            //9:
            //	rts
        }

        //	q 設定
        //[$90] + [DATA]b
        //				$1 ～ $10 まで[16段階]
        public void _COM_90()
        {
            //	moveq.l	#0,d0
            //	move.b	(a1)+,d0
            //	move.b	d0,w_q(a5)
            //	bclr.b	#6,w_flag2(a5)
            //	bset.b	#5,w_flag3(a5)
            reg.d0 = 0;
            reg.d0 = mm.ReadByte(reg.a1++);
            mm.Write(reg.a5 + w.q, (byte)reg.d0);
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) & 0xbf));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) | 0x20));

            //	cmpi.b	#8,MND_VER(a6)
            //	bcc	1f
            if (mm.ReadByte(reg.a6 + dw.MND_VER) < 8)
            {
                //	lea.l	_atq_old(pc),a0
                //	move.l	a0,w_qtjob(a5)
                //	rts
                mm.Write(reg.a5 + w.qtjob, 0);//_atq_old = 0 とする
                return;
            }

            //1:
            //	add.w	d0,d0
            //	move.w	_com_90_table(pc,d0.w),d0
            //	lea.l	_com_90_table(pc,d0.w),a0
            //	move.l	a0,w_qtjob(a5)
            //	rts
            mm.Write(reg.a5 + w.qtjob, reg.d0);//d0(q:0x01-0x10)
        
            //_com_90_table:
            //	.dc.w	0
            //	.dc.w	_atq_01-_com_90_table
            //	.dc.w	_atq_02-_com_90_table
            //	.dc.w	_atq_03-_com_90_table
            //	.dc.w	_atq_04-_com_90_table
            //	.dc.w	_atq_05-_com_90_table
            //	.dc.w	_atq_06-_com_90_table
            //	.dc.w	_atq_07-_com_90_table
            //	.dc.w	_atq_08-_com_90_table
            //	.dc.w	_atq_09-_com_90_table
            //	.dc.w	_atq_10-_com_90_table
            //	.dc.w	_atq_11-_com_90_table
            //	.dc.w	_atq_12-_com_90_table
            //	.dc.w	_atq_13-_com_90_table
            //	.dc.w	_atq_14-_com_90_table
            //	.dc.w	_atq_15-_com_90_table
            //	.dc.w	_atq_16-_com_90_table
        }

        public void _atq_01()
        {
            //	move.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_02()
        {
            //	move.w	d0,d1
            //	lsr.w	#3,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_03()
        {
            //	move.w	d0,d1
            //	add.w	d1,d1
            //	add.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 += (ushort)reg.d1;
            reg.d1 += (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_04()
        {
            //	move.w	d0,d1
            //	lsr.w	#2,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 >>= 2;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_05()
        {
            //	move.w	d0,d1
            //	add.w	d1,d1
            //	add.w	d1,d1
            //	add.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 += (ushort)reg.d1;
            reg.d1 += (ushort)reg.d1;
            reg.d1 += (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_06()
        {
            //	move.w	d0,d1
            //	add.w	d1,d1
            //	add.w	d0,d1
            //	lsr.w	#3,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 += (ushort)reg.d1;
            reg.d1 += (ushort)reg.d0;
            reg.d1 >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_07()
        {
            //	move.w	d0,d1
            //	lsl.w	#3,d1
            //	sub.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 <<= 3;
            reg.d1 -= (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_08()
        {
            //	move.w	d0,d1
            //	lsr.w	#1,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 >>= 1;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_09()
        {
            //	move.w	d0,d1
            //	lsl.w	#3,d1
            //	add.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 <<= 3;
            reg.d1 += (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_10()
        {
            //	move.w	d0,d1
            //	add.w	d1,d1
            //	add.w	d1,d1
            //	add.w	d0,d1
            //	lsr.w	#3,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 += (ushort)reg.d1;
            reg.d1 += (ushort)reg.d1;
            reg.d1 += (ushort)reg.d0;
            reg.d1 >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_11()
        {
            //	move.w	d0,d1
            //	swap	d0
            //	move.w	d1,d0
            //	lsl.w	#4,d1
            //	add.w	d0,d0
            //	add.w	d0,d0
            //	sub.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d0 = (reg.d0 >> 8) + (reg.d0 << 8);
            reg.d0 = (ushort)reg.d1;
            reg.d1 <<= 4;
            reg.d0 += (ushort)reg.d0;
            reg.d0 += (ushort)reg.d0;
            reg.d1 -= (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_12()
        {
            //	move.w	d0,d1
            //	add.w	d1,d1
            //	add.w	d0,d1
            //	lsr.w	#2,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 += (ushort)reg.d1;
            reg.d0 += (ushort)reg.d1;
            reg.d1 >>= 2;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_13()
        {
            //	move.w	d0,d1
            //	swap	d0
            //	move.w	d1,d0
            //	lsl.w	#4,d1
            //	add.w	d0,d0
            //	sub.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d0 = (reg.d0 >> 8) + (reg.d0 << 8);
            reg.d0 = (ushort)reg.d1;
            reg.d1 <<= 4;
            reg.d0 += (ushort)reg.d0;
            reg.d1 -= (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_14()
        {
            //	move.w	d0,d1
            //	lsl.w	#3,d1
            //	sub.w	d0,d1
            //	lsr.w	#3,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 <<= 3;
            reg.d1 -= (ushort)reg.d0;
            reg.d1 >>= 3;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_15()
        {
            //	move.w	d0,d1
            //	lsl.w	#4,d1
            //	sub.w	d0,d1
            //	lsr.w	#4,d1
            //	move.b	d1,w_at_q_work(a5)
            //	rts
            reg.d1 = (ushort)reg.d0;
            reg.d1 <<= 4;
            reg.d1 -= (ushort)reg.d0;
            reg.d1 >>= 4;
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d1);
        }
        public void _atq_16()
        {
            //	move.b	d0,w_at_q_work(a5)
            //	rts
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d0);
        }

        public void _atq_old()
        {
            //	move.b	d0,d3
            //	moveq.l	#$10,d1
            //	moveq.l	#0,d2
            //	sub.b	w_q(a5),d1
            //	beq	2f
            //	lsr.b	#4,d3
            //1:	add.b	d3,d2
            //	dbra	d1,1b
            //2:
            //	sub.b	d2,d0
            //	move.b	d0,w_at_q(a5)
            //	move.b	d0,w_at_q_work(a5)
            //	rts
            reg.d3 = (byte)reg.d0;
            reg.d1 = 0x10;
            reg.d2 = 0;
            reg.d1 -= mm.ReadByte(reg.a5 + w.q);
            if (reg.d1 != 0)
            {
                reg.d3 = (byte)(reg.d3 >> 4);
                do
                {
                    reg.d2 += (byte)reg.d3;
                    reg.d1--;
                } while (reg.d1 != 0);
            }
            reg.d0 -= (byte)reg.d2;
            mm.Write(reg.a5 + w.at_q, (byte)reg.d0);
            mm.Write(reg.a5 + w.at_q_work, (byte)reg.d0);
        }

        //	@q 設定
        //			[$91] + [DATA]b
        public void _COM_91()
        {
            //	move.b	(a1)+,w_at_q(a5)
            //	bset.b	#6,w_flag2(a5)
            //	bclr.b	#5,w_flag3(a5)
            //	rts
            mm.Write(reg.a5 + w.at_q, mm.ReadByte(reg.a1));
            mm.Write(reg.a5 + w.flag2, (byte)(mm.ReadByte(reg.a5 + w.flag2) | 0x40));
            mm.Write(reg.a5 + w.flag3, (byte)(mm.ReadByte(reg.a5 + w.flag3) & 0xdf));
        }

        //;─────────────────────────────────────
        //;	ネガティブ @q 設定
        //;			[$93] + [DATA]b
        //;
        //_COM_93:
        //	move.b	(a1)+,w_at_q(a5)
        //	bset.b	#6,w_flag2(a5)
        //	bset.b	#5,w_flag3(a5)
        //	rts

        //;─────────────────────────────────────
        //;	キーオフモード
        //;			[$94] + [switch]b
        //;
        //_COM_94:
        //	clr.b	w_kom(a5)
        //	move.b	(a1)+,d0
        //	sne.b	w_kom(a5)
        //	rts

        //;─────────────────────────────────────
        //;	擬似リバーブ
        //;		switch = $80 = ON
        //;			 $81 = OFF
        //;			 $82 = volume を直接指定にする
        //;			 $83 = volume を相対指定にする
        //;			 $84 = リバーブ動作は相対音量モードに依存する
        //;			 $85 = リバーブ動作は常に @v 単位
        //;
        //;			 $00 = + [volume]b
        //;			 $01 = + [volume]b + [pan]b
        //;			 $02 = + [volume]b + [tone]b
        //;			 $03 = + [volume]b + [panpot]b + [tone]b
        //;			 $04 = + [volume]b ( 微調整 )
        //;	work
        //;		bit4 1:常に @v
        //;		bit3 1:@v直接
        //;		bit2 1:微調整
        //;		bit1 1:音色変更
        //;		bit0 1:定位変更
        //;
        //_COM_98:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	bmi	_COM_98_onoff
        //;	andi.b	#8,w_reverb(a5)		; Thu Jun 29 17:39 JST 2000 (saori)
        //	andi.b	#$10,w_reverb(a5)
        //	addq.b	#1,d0
        //	add.w	d0,d0
        //	move.w	_COM_98_table(pc,d0.w),d0
        //	jmp	_COM_98_table(pc,d0.w)

        //_COM_98_onoff:
        //	andi.b	#$F,d0
        //	addq.b	#1,d0
        //	add.w	d0,d0
        //	move.w	_COM_98_table2(pc,d0.w),d0
        //	jmp	_COM_98_table2(pc,d0.w)

        //_COM_98_table:
        //	.dc.w	0
        //	.dc.w	_COM_98_0-_COM_98_table
        //	.dc.w	_COM_98_1-_COM_98_table
        //	.dc.w	_COM_98_2-_COM_98_table
        //	.dc.w	_COM_98_3-_COM_98_table
        //	.dc.w	_COM_98_4-_COM_98_table

        //_COM_98_table2:
        //	.dc.w	0
        //	.dc.w	_COM_98_80-_COM_98_table2
        //	.dc.w	_COM_98_81-_COM_98_table2
        //	.dc.w	_COM_98_82-_COM_98_table2
        //	.dc.w	_COM_98_83-_COM_98_table2
        //	.dc.w	_COM_98_84-_COM_98_table2
        //	.dc.w	_COM_98_85-_COM_98_table2

        //_COM_98_80:
        //	bset.b	#7,w_reverb(a5)
        //	rts

        //_COM_98_81:
        //	bclr.b	#7,w_reverb(a5)
        //	rts

        //_COM_98_82:
        //	bset.b	#3,w_reverb(a5)
        //	rts

        //_COM_98_83:
        //	bclr.b	#3,w_reverb(a5)
        //	rts

        //_COM_98_84:
        //	bclr.b	#4,w_reverb(a5)
        //	rts

        //_COM_98_85:
        //	bset.b	#4,w_reverb(a5)
        //	rts

        //;
        //; volume
        //;
        //_COM_98_0:
        //	move.b	(a1)+,w_reverb_vol(a5)
        //	ori.b	#$80,w_reverb(a5)
        //	rts

        //;
        //; volume + pan
        //;
        //_COM_98_1:
        //	move.b	(a1)+,w_reverb_vol(a5)
        //	move.b	(a1)+,w_reverb_pan(a5)
        //	ori.b	#$81,w_reverb(a5)
        //	rts

        //;
        //; volume + tone
        //;
        //_COM_98_2:
        //	move.b	(a1)+,w_reverb_vol(a5)
        //	move.b	(a1)+,w_reverb_tone(a5)
        //	ori.b	#$82,w_reverb(a5)
        //	rts

        //;
        //; volume + panpot + tone
        //;
        //_COM_98_3:
        //	move.b	(a1)+,w_reverb_vol(a5)
        //	move.b	(a1)+,w_reverb_pan(a5)
        //	move.b	(a1)+,w_reverb_tone(a5)
        //	ori.b	#$83,w_reverb(a5)
        //	rts

        //;
        //; volume
        //;
        //_COM_98_4:
        //	move.b	(a1)+,w_reverb_vol(a5)
        //	ori.b	#$84,w_reverb(a5)
        //	rts


        //;─────────────────────────────────────
        //;	擬似エコー
        //;
        //_COM_99:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	bmi	_COM_99_onoff
        //	add.w	d0,d0
        //	move.w	_COM_99_table(pc,d0.w),d0
        //	jmp	_COM_99_table(pc,d0.w)

        //_COM_99_onoff:
        //	cmpi.b	#$80,d0
        //	bne	1f
        //				; on
        //	rts

        //1:				; off
        //	rts

        //_COM_99_table:
        //	.dc.w	_COM_99_0-_COM_99_table		; one shot
        //	.dc.w	_COM_99_1-_COM_99_table		; continue

        //_COM_99_0:
        //	rts

        //_COM_99_1:
        //	rts

        //;─────────────────────────────────────
        //;	擬似動作 step time
        //;
        //_COM_9A:
        //	move.b	(a1)+,w_reverb_time(a5)
        //	rts

        //;─────────────────────────────────────
        //;	音量テーブル切り替え
        //;
        //_COM_A3:
        //	move.b	(a1)+,d1
        //	move.b	(a1)+,d5

        //	move.l	VOL_PTR(a6),d0
        //	beq	_com_a3_exit

        //	movea.l	d0,a2
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	move.l	d0,d2
        //	beq	_com_a3_exit

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d4
        //	move.b	(a2)+,d4

        //_com_a3_ana_loop:
        //	cmp.b	2(a2),d1
        //	bne	1f
        //	cmp.b	3(a2),d5
        //	beq	_com_a3_set
        //1:
        //	subq.w	#1,d4
        //	beq	_com_a3_exit
        //	move.b	(a2),-(sp)
        //	move.w	(sp)+,d0
        //	move.b	1(a2),d0
        //	lea.l	(a2,d0.w),a2
        //	bra	_com_a3_ana_loop

        //_com_a3_exit:
        //	rts

        //_com_a3_set:
        //	addq.w	#4,a2
        //	moveq.l	#$7F,d0
        //	and.b	(a2),d0
        //	move.b	d0,w_volcount(a5)
        //	addq.w	#2,a2

        //	lea.l	w_voltable(a5),a0
        //1:	move.b	(a2)+,(a0)+
        //	dbra	d0,1b
        //	rts


        //;─────────────────────────────────────
        //;	相対音量モード
        //;
        //_COM_A8:
        //	move.b	(a1)+,w_volmode(a5)
        //	rts

        //;─────────────────────────────────────
        //;	ドライバ動作モード変更
        //;
        //_COM_B0:
        //	moveq.l	#1,d0
        //	add.b	(a1)+,d0
        //	add.w	d0,d0
        //	move.w	_COM_B0_table(pc,d0.w),d0
        //	jmp	_COM_B0_table(pc,d0.w)

        //_COM_B0_table:
        //	.dc.w	0
        //	.dc.w	_COM_B0_0-_COM_B0_table
        //	.dc.w	_COM_B0_1-_COM_B0_table
        //	.dc.w	_COM_B0_2-_COM_B0_table
        //	.dc.w	_COM_B0_3-_COM_B0_table

        //_COM_B0_0:
        //	bclr.b	#7,DRV_FLAG2(a6)
        //	move.b	(a1)+,d0
        //	beq	1f
        //	bset.b	#7,DRV_FLAG2(a6)
        //1:
        //	rts

        //_COM_B0_1:
        //	move.b	(a1)+,d0
        //	beq	_COM_B0_1_0
        //	subq.b	#1,d0
        //	beq	_COM_B0_1_1
        //	subq.b	#1,d0
        //	beq	_COM_B0_1_2
        //	bset.b	#6,DRV_FLAG3(a6)
        //	bra	_COM_B0_start_timer_a

        //_COM_B0_1_0:
        //	andi.b	#$7F,DRV_FLAG3(a6)
        //	rts

        //_COM_B0_1_1:
        //	ori.b	#$80,DRV_FLAG3(a6)
        //	bra	_COM_B0_start_timer_a

        //_COM_B0_1_2:
        //	bclr.b	#6,DRV_FLAG3(a6)
        //	rts

        //_COM_B0_start_timer_a:
        //	move.l	d7,-(sp)
        //	moveq.l	#3,d7
        //	moveq.l	#$10,d1
        //	moveq.l	#$1C,d0
        //	bsr	_OPN_WRITE
        //	move.l	(sp)+,d7
        //	rts



        //_COM_B0_2:
        //	clr.b	VOLMODE(a6)
        //	move.b	(a1)+,d0
        //	sne.b	VOLMODE(a6)
        //	rts

        //; PSG LFO MODE
        //_COM_B0_3:
        //	andi.b	#$FC,DRV_FLAG2(a6)
        //	move.b	(a1)+,d0
        //	beq	_COM_B0_3_0
        //	subq.b	#1,d0
        //	beq	_COM_B0_3_1
        //	subq.b	#1,d0
        //	beq	_COM_B0_3_2
        //_COM_B0_3_0:
        //	rts

        //; mako
        //_COM_B0_3_1:
        //	addq.b	#1,DRV_FLAG2(a6)
        //	rts

        //; old pmd
        //_COM_B0_3_2:
        //	addq.b	#2,DRV_FLAG2(a6)
        //	rts

        //;─────────────────────────────────────
        //;	トラックジャンプ
        //;
        //_COM_BE:
        //	bchg.b	#3,DRV_STATUS(a6)
        //	rts

        //;─────────────────────────────────────
        //;	フェードアウト
        //;
        //_COM_BF_exit:
        //	addq.w	#1,a1
        //	rts

        //_COM_BF:
        //	bset.b	#4,DRV_STATUS(a6)
        //	bne	_COM_BF_exit
        //	move.b	#1,FADEFLAG(a6)
        //	move.b	#3,FADECOUNT(a6)

        //	btst.b	#0,DRV_FLAG(a6)
        //	bne	_COM_BF_no_opn
        //	move.l	d7,-(sp)
        //	moveq.l	#3,d7
        //	moveq.l	#$10,d1
        //	moveq.l	#$1C,d0
        //	bsr	_OPN_WRITE
        //	move.l	(sp)+,d7
        //_COM_BF_no_opn:
        //	move.b	(a1)+,d0
        //	beq	_COM_BF_normal
        //	move.b	d0,FADESPEED(a6)
        //	move.b	d0,FADESPEED_WORK(a6)
        //	rts

        //_COM_BF_normal:
        //	move.b	#7,FADESPEED(a6)
        //	move.b	#7,FADESPEED_WORK(a6)
        //	rts

        //;─────────────────────────────────────
        //;	ソフトウェアエンベロープ
        //;		[$C0] + [SV]b + [AR]b + [DR]b + [SL]b + [SR]b + [RR]b
        //;
        //_COM_C0:
        //	move.b	(a1)+,w_e_sv(a5)
        //	move.b	(a1)+,w_e_ar(a5)
        //	move.b	(a1)+,w_e_dr(a5)
        //	move.b	(a1)+,w_e_sl(a5)
        //	move.b	(a1)+,w_e_sr(a5)
        //	move.b	(a1)+,w_e_rr(a5)
        //	clr.b	w_e_sub(a5)
        //	move.b	#4,w_e_p(a5)
        //	ori.b	#$80,w_e_sw(a5)
        //	rts

        //;─────────────────────────────────────
        //;	ソフトウェアエンベロープ 2
        //;		[$C1] + [AL]b + [DD]b + [SR]b + [RR]b
        //;
        //_COM_C1:
        //	move.b	(a1)+,w_e_al(a5)
        //	move.b	(a1)+,w_e_dd(a5)
        //	move.b	(a1)+,w_e_sr(a5)
        //	move.b	(a1)+,w_e_rr(a5)
        //	ori.b	#$81,w_e_sw(a5)
        //	rts

        //;─────────────────────────────────────
        //;	・ソフトウェアエンベロープスイッチ
        //;		[$C3] + [switch]
        //;
        //_COM_C3:
        //	move.b	(a1)+,d0
        //	beq	1f
        //	ori.b	#$80,w_e_sw(a5)
        //	rts
        //1:
        //	bclr.b	#7,w_e_sw(a5)
        //	rts

        //;─────────────────────────────────────
        //;	・エンベロープ切り替え
        //;		[$C4] + [num]
        //;
        //_COM_C4:
        //	move.b	(a1)+,d5
        //	move.b	d5,w_envnum(a5)

        //	move.l	ENV_PTR(a6),d0
        //	beq	_com_c4_exit

        //	movea.l	d0,a2
        //	addq.w	#6,a2
        //	move.w	ENVNUM(a6),d4
        //	beq	_com_c4_exit

        //	move.b	w_envbank(a5),d1
        //_COM_C4_ana_loop:
        //	cmp.b	2(a2),d1
        //	bne	1f
        //	cmp.b	3(a2),d5
        //	beq	_COM_C4_set
        //1:
        //	subq.w	#1,d4
        //	beq	_com_c4_exit
        //	move.b	(a2),-(sp)
        //	move.w	(sp)+,d0
        //	move.b	1(a2),d0
        //	lea.l	(a2,d0.w),a2
        //	bra	_COM_C4_ana_loop

        //_COM_C4_set:
        //	addq.w	#4,a2

        //	move.b	3(a2),w_e_sv(a5)
        //	move.b	(a2),w_e_ar(a5)
        //	move.b	4(a2),w_e_dr(a5)
        //	move.b	6(a2),w_e_sl(a5)
        //	move.b	8(a2),w_e_sr(a5)
        //	move.b	12(a2),w_e_rr(a5)
        //	ori.b	#$80,w_e_sw(a5)
        //	btst.b	#5,w_flag(a5)
        //	bne	_com_c4_exit
        //	clr.b	w_e_sub(a5)
        //	move.b	#4,w_e_p(a5)
        //_com_c4_exit:
        //	rts

        //;─────────────────────────────────────
        //;	・バンク&エンベロープ切り替え
        //;		[$C5] + [bank] + [num]
        //;
        //_COM_C5:
        //	move.b	(a1)+,d1
        //	move.b	d1,w_envbank(a5)
        //	move.b	(a1)+,d5
        //	move.b	d5,w_envnum(a5)

        //	move.l	ENV_PTR(a6),d0
        //	beq	_com_c5_exit

        //	movea.l	d0,a2
        //	addq.w	#6,a2
        //	move.w	ENVNUM(a6),d4
        //	beq	_com_c5_exit

        //_COM_C5_ana_loop:
        //	cmp.b	2(a2),d1
        //	bne	1f
        //	cmp.b	3(a2),d5
        //	beq	_COM_C5_set
        //1:
        //	subq.w	#1,d4
        //	beq	_com_c5_exit
        //	move.b	(a2),-(sp)
        //	move.w	(sp)+,d0
        //	move.b	1(a2),d0
        //	lea.l	(a2,d0.w),a2
        //	bra	_COM_C5_ana_loop

        //_COM_C5_set:
        //	addq.w	#4,a2

        //	move.b	3(a2),w_e_sv(a5)
        //	move.b	(a2),w_e_ar(a5)
        //	move.b	4(a2),w_e_dr(a5)
        //	move.b	6(a2),w_e_sl(a5)
        //	move.b	8(a2),w_e_sr(a5)
        //	move.b	12(a2),w_e_rr(a5)
        //	clr.b	w_e_sub(a5)
        //	move.b	#4,w_e_p(a5)
        //	ori.b	#$80,w_e_sw(a5)
        //_com_c5_exit:
        //	rts


        //;─────────────────────────────────────
        //;	キートランスポーズ
        //;
        //_COM_D0:
        //	move.b	(a1)+,w_key_trans(a5)
        //	rts

        //;─────────────────────────────────────
        //;	相対キートランスポーズ
        //;
        //_COM_D1:
        //	move.b	(a1)+,d0
        //	add.b	d0,w_key_trans(a5)
        //	rts


        //;─────────────────────────────────────
        //;	detune 設定
        //;
        //_COM_D8:
        //	bset.b	#1,w_flag(a5)

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	move.w	d0,w_detune(a5)
        //	rts

        //;─────────────────────────────────────
        //;	detune 設定
        //;
        //_COM_D9:
        //	bset.b	#1,w_flag(a5)

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	add.w	d0,w_detune(a5)
        //	rts


        //;─────────────────────────────────────
        //;	pitch LFO
        //;
        //;	$E2,num,wave,speed,count,delay,henka_w
        //;
        //_COM_E2:
        //	bset.b	#1,w_flag(a5)
        //	moveq.l	#1,d0
        //	add.b	(a1)+,d0
        //	add.w	d0,d0
        //	move.w	_COM_E2_table(pc,d0.w),d0
        //	jmp	_COM_E2_table(pc,d0.w)

        //_COM_E2_table:
        //	.dc.w	0
        //	.dc.w	_COM_E2_0-_COM_E2_table
        //	.dc.w	_COM_E2_1-_COM_E2_table
        //	.dc.w	_COM_E2_2-_COM_E2_table
        //	.dc.w	_COM_E2_3-_COM_E2_table

        //;─────────────────────────────────────
        //;	pitch LFO ALL
        //_COM_E2_0:
        //	or.b	#$E,w_lfo(a5)
        //	movea.l	a1,a0
        //	lea.l	w_p_pattern1(a5),a4
        //	lea.l	w_wp_pattern1(a5),a3
        //	bsr	_COM_E2_common
        //	tst.l	d2
        //	bpl	1f
        //	bclr.b	#1,w_lfo(a5)
        //1:
        //	movea.l	a0,a1
        //	lea.l	w_p_pattern2(a5),a4
        //	lea.l	w_wp_pattern2(a5),a3
        //	bsr	_COM_E2_common
        //	tst.l	d2
        //	bpl	1f
        //	bclr.b	#2,w_lfo(a5)
        //1:
        //	movea.l	a0,a1
        //	lea.l	w_p_pattern3(a5),a4
        //	lea.l	w_wp_pattern3(a5),a3
        //	bsr	_COM_E2_common
        //	tst.l	d2
        //	bpl	1f
        //	bclr.b	#3,w_lfo(a5)
        //1:
        //	rts

        //;─────────────────────────────────────
        //;	pitch LFO 1
        //_COM_E2_1:
        //	bset.b	#1,w_lfo(a5)

        //	lea.l	w_p_pattern1(a5),a4
        //	lea.l	w_wp_pattern1(a5),a3
        //	bsr	_COM_E2_common
        //	tst.l	d2
        //	bpl	1f
        //	bclr.b	#1,w_lfo(a5)
        //1:
        //	rts

        //;─────────────────────────────────────
        //;	pitch LFO 2
        //_COM_E2_2:
        //	bset.b	#2,w_lfo(a5)

        //	lea.l	w_p_pattern2(a5),a4
        //	lea.l	w_wp_pattern2(a5),a3
        //	bsr	_COM_E2_common
        //	tst.l	d2
        //	bpl	1f
        //	bclr.b	#2,w_lfo(a5)
        //1:
        //	rts

        //;─────────────────────────────────────
        //;	pitch LFO 3
        //_COM_E2_3:
        //	bset.b	#3,w_lfo(a5)

        //	lea.l	w_p_pattern3(a5),a4
        //	lea.l	w_wp_pattern3(a5),a3
        //	bsr	_COM_E2_common
        //	tst.l	d2
        //	bpl	1f
        //	bclr.b	#3,w_lfo(a5)
        //1:
        //	rts

        //;─────────────────────────────────────
        //;	LFO SET COMMON
        //;
        //_COM_E2_common:
        //	clr.w	w_l_bendwork(a4)
        //	clr.b	w_w_use_flag(a3)
        //	move.b	(a1)+,d0
        //	move.b	d0,w_l_pattern(a4)
        //	bmi	_COM_E2_wavememory
        //	cmpi.b	#$10,d0
        //	bcc	_COM_E2_compatible

        //	move.b	(a1)+,d1
        //	move.b	d1,w_l_lfo_sp(a4)
        //	move.b	(a1)+,w_l_count(a4)
        //	move.b	(a1)+,d0
        //	cmpi.b	#$FF,d0
        //	beq	1f
        //	move.b	d0,w_l_keydelay(a4)
        //	add.b	d0,d1
        //	move.b	d1,w_l_delay_work(a4)
        //1:
        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	move.w	d0,w_l_henka(a4)
        //	move.w	d0,w_l_henka_work(a4)
        //	move.b	w_l_count(a4),d0
        //	lsr.b	#1,d0
        //	move.b	d0,w_l_count_work(a4)

        //	andi.w	#$DFFF,w_l_flag(a4)
        //	moveq.l	#0,d2
        //	rts

        //;─────────────────────────────────────
        //_COM_E2_compatible:
        //	move.b	#1,w_w_use_flag(a3)
        //	st.b	w_l_pattern(a4)

        //	andi.b	#7,d0
        //	move.b	d0,d1
        //	andi.b	#3,d1
        //	move.b	d1,d2
        //	add.b	#$10,d1
        //	move.b	d1,w_w_type(a3)

        //	move.b	(a1)+,w_l_lfo_sp(a4)

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a1)+,d1
        //	move.w	d1,w_w_start(a3)	; 周期1

        //	cmpi.b	#1,d2
        //	beq	1f
        //	lsr.w	#1,d1
        //	bra	1f
        //	cmpi.b	#3,d2
        //	bne	1f
        //	moveq.l	#1,d1
        //1:
        //	move.w	d1,w_w_loop_start(a3)	; 周期2

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a1)+,d1
        //	ext.l	d1
        //	asl.l	#8,d1
        //	cmpi.b	#4,d0
        //	bcs	1f
        //	asl.l	#8,d1
        //1:
        //	move.l	d1,w_w_loop_end(a3)	; 増減1
        //	cmpi.b	#2,d2			; wave2
        //	beq	1f
        //	moveq.l	#0,d1
        //1:
        //	move.l	d1,w_w_loop_count(a3)	; 増減2
        //	moveq.l	#0,d2
        //	rts

        //;─────────────────────────────────────
        //_COM_E2_wavememory:
        //	andi.w	#$7F,d0
        //	bsr	_get_wave_memory_e2

        //	move.b	(a1)+,d1
        //	move.b	d1,w_l_lfo_sp(a4)
        //	move.b	(a1)+,d0
        //	move.b	d0,w_w_depth(a3)
        //	move.b	d0,w_l_count(a4)
        //	move.b	(a1)+,d0
        //	cmpi.b	#$FF,d0
        //	beq	1f
        //	move.b	d0,w_l_keydelay(a4)
        //	add.b	d0,d1
        //	move.b	d1,w_l_delay_work(a4)
        //1:
        //	move.b	(a1)+,d0
        //	move.b	(a1)+,d0
        //	clr.w	w_l_flag(a4)
        //	rts

        //;─────────────────────────────────────
        //_get_wave_memory_e2:
        //	move.l	WAVE_PTR(a6),d1
        //	beq	_com_e2_wm_err_exit
        //	movea.l	d1,a2
        //	move.l	d1,d5

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a2)+,d1
        //	swap	d1
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a2)+,d1
        //	tst.l	d1
        //	beq	_com_e2_wm_err_exit

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a2)+,d1
        //	tst.w	d1
        //	beq	_com_e2_wm_err_exit

        //_com_e2_wm10:
        //	move.b	(a2),-(sp)
        //	move.w	(sp)+,d2
        //	move.b	1(a2),d2
        //	swap	d2
        //	move.b	2(a2),-(sp)
        //	move.w	(sp)+,d2
        //	move.b	3(a2),d2
        //	cmp.w	4(a2),d0
        //	beq	_com_e2_wm20
        //	lea.l	(a2,d2.l),a2
        //	dbra	d1,_com_e2_wm10
        //	bra	_com_e2_wm_err_exit

        //_com_e2_wm20:
        //	addq.w	#6,a2

        //	move.b	(a2)+,d0
        //	move.b	d0,d1
        //	andi.b	#$F,d1
        //	move.b	d1,w_w_type(a3)
        //	lsr.b	#4,d0
        //	move.b	d0,w_w_ko_flag(a3)

        //	move.b	(a2)+,d0

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_w_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_w_loop_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_w_loop_end(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	move.l	d0,w_w_loop_count(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_w_ko_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_w_ko_loop_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_w_ko_loop_end(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	move.l	d0,w_w_ko_loop_count(a3)

        //	st.b	w_w_use_flag(a3)
        //	moveq.l	#0,d2
        //	rts

        //_com_e2_wm_err_exit:
        //	moveq.l	#-1,d2
        //	rts

        //;─────────────────────────────────────
        //;	pitch LFO on /off
        //;
        //;	$E3,num,switch
        //;
        //_COM_E3:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	move.l	d0,d1
        //	add.w	d0,d0
        //	move.w	_COM_E3_table(pc,d0.w),d0
        //	jmp	_COM_E3_table(pc,d0.w)

        //_COM_E3_table:
        //	.dc.w	_COM_E3_0-_COM_E3_table
        //	.dc.w	_COM_E3_1-_COM_E3_table
        //	.dc.w	_COM_E3_2-_COM_E3_table
        //	.dc.w	_COM_E3_3-_COM_E3_table

        //; bit15		0:enable 1:disable
        //; bit1 keyoff	0:enable 1:disable
        //; bit0 keyon	0:enable 1:disable
        //; $80  = at keyon
        //; $81  = at keyoff
        //; $82  = always
        //; $83  = async
        //; $84  = stop & init
        //;
        //_COM_E3_0:
        //	andi.b	#$F1,w_lfo(a5)
        //	move.b	(a1)+,d0
        //	beq	_COM_E3_ALL_OFF
        //	bpl	_COM_E3_ALL_ON

        //	cmpi.b	#$82,d0
        //	beq	2f
        //	cmpi.b	#$83,d0
        //	beq	3f
        //	cmpi.b	#$84,d0
        //	beq	4f

        //	lsr.b	#1,d0
        //	bcc	1f
        //	lea.l	w_p_pattern1(a5),a4
        //	move.w	#$8002,w_l_flag(a4)
        //	lea.l	w_p_pattern2(a5),a4
        //	move.w	#$8002,w_l_flag(a4)
        //	lea.l	w_p_pattern3(a5),a4
        //	move.w	#$8002,w_l_flag(a4)
        //	bra	_COM_E3_ALL_ON
        //1:
        //	lea.l	w_p_pattern1(a5),a4
        //	move.w	#$8001,w_l_flag(a4)
        //	lea.l	w_p_pattern2(a5),a4
        //	move.w	#$8001,w_l_flag(a4)
        //	lea.l	w_p_pattern3(a5),a4
        //	move.w	#$8001,w_l_flag(a4)
        //	bra	_COM_E3_ALL_ON
        //2:
        //	lea.l	w_p_pattern1(a5),a4
        //	clr.w	w_l_flag(a4)
        //	lea.l	w_p_pattern2(a5),a4
        //	clr.w	w_l_flag(a4)
        //	lea.l	w_p_pattern3(a5),a4
        //	clr.w	w_l_flag(a4)
        //	bra	_COM_E3_ALL_ON
        //3:
        //	lea.l	w_p_pattern1(a5),a4
        //	move.w	#$4000,w_l_flag(a4)
        //	lea.l	w_p_pattern2(a5),a4
        //	move.w	#$4000,w_l_flag(a4)
        //	lea.l	w_p_pattern3(a5),a4
        //	move.w	#$4000,w_l_flag(a4)
        //	bra	_COM_E3_ALL_ON
        //4:
        //	bsr	_init_lfo2
        //	bra	_COM_E3_ALL_OFF

        //_COM_E3_ALL_ON:
        //	bset.b	#1,w_flag(a5)
        //	ori.b	#$E,w_lfo(a5)
        //_COM_E3_ALL_OFF:
        //	rts

        //_COM_E3_1:
        //	move.b	(a1)+,d0
        //	beq	_COM_E3_OFF
        //	bpl	_COM_E3_ON
        //	lea.l	w_p_pattern1(a5),a4
        //	bra	_COM_E3_common

        //_COM_E3_2:
        //	move.b	(a1)+,d0
        //	beq	_COM_E3_OFF
        //	bpl	_COM_E3_ON
        //	lea.l	w_p_pattern2(a5),a4
        //	bra	_COM_E3_common

        //_COM_E3_3:
        //	move.b	(a1)+,d0
        //	beq	_COM_E3_OFF
        //	bpl	_COM_E3_ON
        //	lea.l	w_p_pattern3(a5),a4
        //;	bra	_COM_E3_common

        //_COM_E3_common:
        //	cmpi.b	#$82,d0
        //	beq	2f
        //	cmpi.b	#$83,d0
        //	beq	3f
        //	cmpi.b	#$84,d0
        //	beq	4f

        //	lsr.b	#1,d0
        //	bcc	1f
        //	move.w	#$8002,w_l_flag(a4)
        //	bra	_COM_E3_ON
        //1:
        //	move.w	#$8001,w_l_flag(a4)
        //	bra	_COM_E3_ON
        //2:
        //	clr.w	w_l_flag(a4)
        //	bra	_COM_E3_ON
        //3:
        //	move.w	#$4000,w_l_flag(a4)
        //	bra	_COM_E3_ON
        //4:
        //	bsr	_init_lfo2
        //	bra	_COM_E3_OFF

        //_COM_E3_ON:
        //	bset.b	#1,w_flag(a5)
        //	move.b	w_lfo(a5),d0
        //	bset.l	d1,d0
        //	move.b	d0,w_lfo(a5)
        //	rts

        //_COM_E3_OFF:
        //	move.b	w_lfo(a5),d0
        //	bclr.l	d1,d0
        //	move.b	d0,w_lfo(a5)
        //	rts

        //;─────────────────────────────────────
        //;	pitch LFO delay
        //;
        //;	$E4,num,delay
        //;
        //_COM_E4:
        //	moveq.l	#1,d0
        //	add.b	(a1)+,d0
        //	add.w	d0,d0
        //	move.w	_COM_E4_table(pc,d0.w),d0
        //	jmp	_COM_E4_table(pc,d0.w)

        //_COM_E4_table:
        //	.dc.w	0
        //	.dc.w	_COM_E4_0-_COM_E4_table
        //	.dc.w	_COM_E4_1-_COM_E4_table
        //	.dc.w	_COM_E4_2-_COM_E4_table
        //	.dc.w	_COM_E4_3-_COM_E4_table

        //;─────────────────────────────────────
        //;	pitch LFO ALL
        //_COM_E4_0:
        //;	or.b	#$E,w_lfo(a5)
        //	movea.l	a1,a0
        //	lea.l	w_p_pattern1(a5),a4
        //	bsr	_COM_E49_common
        //	movea.l	a0,a1
        //	lea.l	w_p_pattern2(a5),a4
        //	bsr	_COM_E49_common
        //	movea.l	a0,a1
        //	lea.l	w_p_pattern3(a5),a4
        //	bra	_COM_E49_common

        //;─────────────────────────────────────
        //;	pitch LFO 1
        //_COM_E4_1:
        //	lea.l	w_p_pattern1(a5),a4
        //;	bset.b	#1,w_lfo(a5)
        //	bra	_COM_E49_common

        //;─────────────────────────────────────
        //;	pitch LFO 2
        //_COM_E4_2:
        //	lea.l	w_p_pattern2(a5),a4
        //;	bset.b	#2,w_lfo(a5)
        //	bra	_COM_E49_common

        //;─────────────────────────────────────
        //;	pitch LFO 3
        //_COM_E4_3:
        //	lea.l	w_p_pattern3(a5),a4
        //;	bset.b	#3,w_lfo(a5)

        //_COM_E49_common:
        //	move.b	(a1)+,d0
        //	cmpi.b	#$FF,d0
        //	beq	_COM_E49_add
        //	move.b	d0,w_l_keydelay(a4)
        //	add.b	w_l_lfo_sp(a4),d0
        //	move.b	d0,w_l_delay_work(a4)
        //	rts

        //_COM_E49_add:
        //	move.b	(a1)+,d0
        //	add.b	w_l_keydelay(a4),d0
        //	add.b	w_l_lfo_sp(a4),d0
        //	move.b	d0,w_l_keydelay(a4)
        //	move.b	d0,w_l_delay_work(a4)
        //	rts

        //;─────────────────────────────────────
        //;	音量 LFO
        //;
        //;	$E7,num,wave,delay,count,speed,henka_w
        //;
        //_COM_E7:
        //	bset.b	#1,w_flag(a5)
        //	moveq.l	#1,d0
        //	add.b	(a1)+,d0
        //	add.w	d0,d0
        //	move.w	_COM_E7_table(pc,d0.w),d0
        //	jmp	_COM_E7_table(pc,d0.w)

        //_COM_E7_table:
        //	.dc.w	0
        //	.dc.w	_COM_E7_0-_COM_E7_table
        //	.dc.w	_COM_E7_1-_COM_E7_table
        //	.dc.w	_COM_E7_2-_COM_E7_table
        //	.dc.w	_COM_E7_3-_COM_E7_table

        //;─────────────────────────────────────
        //;	音量 LFO
        //;
        //_COM_E7_0:
        //	movea.l	a1,a0
        //	or.b	#$70,w_lfo(a5)
        //	lea.l	w_v_pattern1(a5),a4
        //	lea.l	w_wv_pattern1(a5),a3
        //	bsr	_COM_E7_common
        //	movea.l	a0,a1
        //	lea.l	w_v_pattern2(a5),a4
        //	lea.l	w_wv_pattern2(a5),a3
        //	bsr	_COM_E7_common
        //	movea.l	a0,a1
        //	lea.l	w_v_pattern3(a5),a4
        //	lea.l	w_wv_pattern3(a5),a3
        //	bra	_COM_E7_common

        //;─────────────────────────────────────
        //;	音量 LFO 1
        //;
        //_COM_E7_1:
        //	bset.b	#4,w_lfo(a5)
        //	lea.l	w_v_pattern1(a5),a4
        //	lea.l	w_wv_pattern1(a5),a3
        //	bra	_COM_E7_common

        //;─────────────────────────────────────
        //;	音量 LFO 2
        //;
        //_COM_E7_2:
        //	bset.b	#5,w_lfo(a5)

        //	lea.l	w_v_pattern2(a5),a4
        //	lea.l	w_wv_pattern2(a5),a3
        //	bra	_COM_E7_common

        //;─────────────────────────────────────
        //;	音量 LFO 3
        //;
        //_COM_E7_3:
        //	bset.b	#6,w_lfo(a5)
        //	lea.l	w_v_pattern3(a5),a4
        //	lea.l	w_wv_pattern3(a5),a3
        //;	bra	_COM_E7_common

        //;─────────────────────────────────────
        //;	LFO SET COMMON
        //;
        //_COM_E7_common:
        //	clr.w	w_l_bendwork(a4)
        //	clr.b	w_w_use_flag(a3)
        //	move.b	(a1)+,d0
        //	move.b	d0,w_l_pattern(a4)
        //	bmi	_COM_E2_wavememory
        //	cmpi.b	#$10,d0
        //	bcc	_COM_E7_compatible

        //	move.b	(a1)+,d1
        //	move.b	d1,w_l_lfo_sp(a4)
        //	move.b	(a1)+,w_l_count(a4)
        //	move.b	(a1)+,d0
        //	cmpi.b	#$FF,d0
        //	beq	1f
        //	move.b	d0,w_l_keydelay(a4)
        //	add.b	d0,d1
        //	move.b	d1,w_l_delay_work(a4)
        //1:
        //	addq.w	#1,a1
        //	move.b	(a1)+,d0
        //	ext.w	d0
        //	move.w	d0,w_l_henka(a4)
        //	move.w	d0,w_l_henka_work(a4)
        //	rts

        //;─────────────────────────────────────
        //_COM_E7_compatible:
        //	move.b	#1,w_w_use_flag(a3)
        //	st.b	w_l_pattern(a4)

        //	andi.b	#7,d0
        //	move.b	d0,d1
        //	andi.b	#3,d1
        //	move.b	d1,d2
        //	add.b	#$14,d1
        //	move.b	d1,w_w_type(a3)

        //	move.b	(a1)+,w_l_lfo_sp(a4)

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a1)+,d1
        //	move.w	d1,w_w_start(a3)	; 周期
        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	move.w	d0,w_w_loop_start(a3)	; 増減
        //	lsr.b	#1,d2
        //	bcs	1f
        //	muls.w	d1,d0
        //1:
        //	neg.w	d0
        //	bpl	1f
        //	moveq.l	#0,d0
        //1:
        //	move.w	d0,w_w_loop_end(a3)	; 最大振幅

        //	move.w	w_w_start(a3),w_w_ko_start(a3)
        //	move.w	w_w_loop_start(a3),w_w_ko_loop_start(a3)
        //	move.w	w_w_loop_end(a3),w_w_ko_loop_end(a3)
        //	moveq.l	#0,d2
        //	rts

        //;─────────────────────────────────────
        //;	音量 LFO on /off
        //;
        //;	$E8,num,switch
        //_COM_E8:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	move.l	d0,d1
        //	addq.b	#3,d1
        //	add.w	d0,d0
        //	move.w	_COM_E8_table(pc,d0.w),d0
        //	jmp	_COM_E8_table(pc,d0.w)

        //_COM_E8_table:
        //	.dc.w	_COM_E8_0-_COM_E8_table
        //	.dc.w	_COM_E8_1-_COM_E8_table
        //	.dc.w	_COM_E8_2-_COM_E8_table
        //	.dc.w	_COM_E8_3-_COM_E8_table


        //_COM_E8_0:
        //	andi.b	#$8F,w_lfo(a5)
        //	move.b	(a1)+,d0
        //	beq	_COM_E8_ALL_OFF
        //	bpl	_COM_E8_ALL_ON

        //	cmpi.b	#$82,d0
        //	beq	2f
        //	cmpi.b	#$83,d0
        //	beq	3f

        //	lsr.b	#1,d0
        //	bcc	1f
        //	lea.l	w_v_pattern1(a5),a4
        //	move.w	#$8002,w_l_flag(a4)
        //	lea.l	w_v_pattern2(a5),a4
        //	move.w	#$8002,w_l_flag(a4)
        //	lea.l	w_v_pattern3(a5),a4
        //	move.w	#$8002,w_l_flag(a4)
        //	bra	_COM_E8_ALL_ON
        //1:
        //	lea.l	w_v_pattern1(a5),a4
        //	move.w	#$8001,w_l_flag(a4)
        //	lea.l	w_v_pattern2(a5),a4
        //	move.w	#$8001,w_l_flag(a4)
        //	lea.l	w_v_pattern3(a5),a4
        //	move.w	#$8001,w_l_flag(a4)
        //	bra	_COM_E8_ALL_ON
        //2:
        //	lea.l	w_v_pattern1(a5),a4
        //	clr.w	w_l_flag(a4)
        //	lea.l	w_v_pattern2(a5),a4
        //	clr.w	w_l_flag(a4)
        //	lea.l	w_v_pattern3(a5),a4
        //	clr.w	w_l_flag(a4)
        //	bra	_COM_E8_ALL_ON
        //3:
        //	lea.l	w_v_pattern1(a5),a4
        //	move.w	#$4000,w_l_flag(a4)
        //	lea.l	w_v_pattern2(a5),a4
        //	move.w	#$4000,w_l_flag(a4)
        //	lea.l	w_v_pattern3(a5),a4
        //	move.w	#$4000,w_l_flag(a4)

        //_COM_E8_ALL_ON:
        //	bset.b	#1,w_flag(a5)
        //	ori.b	#$70,w_lfo(a5)
        //_COM_E8_ALL_OFF:
        //	rts

        //_COM_E8_1:
        //	move.b	(a1)+,d0
        //	beq	_COM_E8_OFF
        //	bpl	_COM_E8_ON
        //	lea.l	w_v_pattern1(a5),a4
        //	bra	_COM_E8_common

        //_COM_E8_2:
        //	move.b	(a1)+,d0
        //	beq	_COM_E8_OFF
        //	bpl	_COM_E8_ON
        //	lea.l	w_v_pattern2(a5),a4
        //	bra	_COM_E8_common

        //_COM_E8_3:
        //	move.b	(a1)+,d0
        //	beq	_COM_E8_OFF
        //	bpl	_COM_E8_ON
        //	lea.l	w_v_pattern3(a5),a4
        //;	bra	_COM_E8_common

        //_COM_E8_common:
        //	cmpi.b	#$82,d0
        //	beq	2f
        //	cmpi.b	#$83,d0
        //	beq	3f

        //	lsr.b	#1,d0
        //	bcc	1f
        //	move.w	#$8002,w_l_flag(a4)
        //	bra	_COM_E8_ON
        //1:
        //	move.w	#$8001,w_l_flag(a4)
        //	bra	_COM_E8_ON
        //2:
        //	clr.w	w_l_flag(a4)
        //	bra	_COM_E8_ON
        //3:
        //	move.w	#$4000,w_l_flag(a4)

        //_COM_E8_ON:
        //	bset.b	#1,w_flag(a5)
        //	move.b	w_lfo(a5),d0
        //	bset.l	d1,d0
        //	move.b	d0,w_lfo(a5)
        //	rts

        //_COM_E8_OFF:
        //	move.b	w_lfo(a5),d0
        //	bclr.l	d1,d0
        //	move.b	d0,w_lfo(a5)
        //	rts

        //;─────────────────────────────────────
        //;	音量 LFO delay
        //;
        //_COM_E9:
        //	moveq.l	#1,d0
        //	add.b	(a1)+,d0
        //	add.w	d0,d0
        //	move.w	_COM_E9_table(pc,d0.w),d0
        //	jmp	_COM_E9_table(pc,d0.w)

        //_COM_E9_table:
        //	.dc.w	0
        //	.dc.w	_COM_E9_0-_COM_E9_table
        //	.dc.w	_COM_E9_1-_COM_E9_table
        //	.dc.w	_COM_E9_2-_COM_E9_table
        //	.dc.w	_COM_E9_3-_COM_E9_table

        //;─────────────────────────────────────
        //;	音量 LFO
        //;
        //_COM_E9_0:
        //	movea.l	a1,a0
        //;	or.b	#$70,w_lfo(a5)
        //	lea.l	w_v_pattern1(a5),a4
        //	bsr	_COM_E49_common
        //	movea.l	a0,a1
        //	lea.l	w_v_pattern2(a5),a4
        //	bsr	_COM_E49_common
        //	movea.l	a0,a1
        //	lea.l	w_v_pattern3(a5),a4
        //	bra	_COM_E49_common

        //;─────────────────────────────────────
        //;	音量 LFO 1
        //_COM_E9_1:
        //	lea.l	w_v_pattern1(a5),a4
        //;	bset.b	#4,w_lfo(a5)
        //	bra	_COM_E49_common

        //;─────────────────────────────────────
        //;	音量 LFO 2
        //_COM_E9_2:
        //	lea.l	w_v_pattern2(a5),a4
        //;	bset.b	#5,w_lfo(a5)
        //	bra	_COM_E49_common

        //;─────────────────────────────────────
        //;	音量 LFO 3
        //_COM_E9_3:
        //	lea.l	w_v_pattern3(a5),a4
        //;	bset.b	#6,w_lfo(a5)
        //	bra	_COM_E49_common


        //;─────────────────────────────────────
        //;	音量 LFO switch 2
        //;
        //_COM_EA:
        //	moveq.l	#1,d0
        //	add.b	(a1)+,d0
        //	add.w	d0,d0
        //	move.w	_COM_EA_table(pc,d0.w),d0
        //	jmp	_COM_EA_table(pc,d0.w)

        //_COM_EA_table:
        //	.dc.w	0
        //	.dc.w	_COM_EA_0-_COM_EA_table
        //	.dc.w	_COM_EA_1-_COM_EA_table
        //	.dc.w	_COM_EA_2-_COM_EA_table
        //	.dc.w	_COM_EA_3-_COM_EA_table

        //;─────────────────────────────────────
        //;	音量 LFO
        //;
        //_COM_EA_0:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	beq	1f
        //	ori.b	#$80,d0
        //1:
        //	lea.l	w_wv_pattern1(a5),a3
        //	move.b	d0,w_w_slot(a3)
        //	lea.l	w_wv_pattern2(a5),a3
        //	move.b	d0,w_w_slot(a3)
        //	lea.l	w_wv_pattern3(a5),a3
        //	move.b	d0,w_w_slot(a3)
        //	rts

        //;─────────────────────────────────────
        //;	音量 LFO 1
        //_COM_EA_1:
        //	lea.l	w_wv_pattern1(a5),a3
        //	bra	_COM_EA_common

        //;─────────────────────────────────────
        //;	音量 LFO 2
        //_COM_EA_2:
        //	lea.l	w_wv_pattern2(a5),a3
        //	bra	_COM_EA_common

        //;─────────────────────────────────────
        //;	音量 LFO 3
        //_COM_EA_3:
        //	lea.l	w_wv_pattern3(a5),a3
        //_COM_EA_common:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	beq	1f
        //	ori.b	#$80,d0
        //1:
        //	move.b	d0,w_w_slot(a3)
        //	rts


        //;─────────────────────────────────────
        //;	わうわう
        //;
        //_COM_EB:
        //	lea.l	w_ww_pattern1(a5),a4

        //	move.b	(a1)+,d0
        //	beq	_COM_EB_END
        //	bmi	_COM_EB_ON

        //	move.b	(a1)+,w_ww_delay(a4)
        //	move.b	(a1)+,w_ww_speed(a4)
        //	move.b	(a1)+,w_ww_rate(a4)
        //	move.b	(a1)+,w_ww_depth(a4)
        //	move.b	(a1)+,w_ww_slot(a4)

        //_COM_EB_ON:
        //	clr.b	w_ww_sync(a4)
        //	bset.b	#5,w_effect(a5)

        //	move.b	w_ww_speed(a4),d0
        //	add.b	w_ww_delay(a4),d0
        //	move.b	d0,w_ww_delay_work(a4)
        //	move.b	w_ww_rate(a4),w_ww_rate_work(a4)
        //	move.b	w_ww_depth(a4),w_ww_depth_work(a4)
        //	clr.b	w_ww_work(a4)

        //	tst.b	w_ww_slot(a4)
        //	smi.b	w_ww_sync(a4)
        //	rts

        //_COM_EB_END:
        //	bclr.b	#5,w_effect(a5)
        //	rts



        //;─────────────────────────────────────
        //;	wavememory effect
        //;
        //;	[$ED] + [num]b + [switch]b ...
        //;	num
        //;		$00 ～ $03
        //;	switch
        //;	minus
        //;		$80 = ON
        //;		$81 = OFF
        //;		$82 = always
        //;		$83 = at keyon
        //;		$84 = at keyoff
        //;	plus = + [switch2]b + [wave]w + [delay]b + [speed]b + [sync]b + [reset]w
        //;	$00 = y command
        //;		波形データの上位バイトのレジスタに
        //;		下位バイトのデータを書き込む
        //;	$01 = tone
        //;	$02 = panpot
        //;
        //_COM_ED:
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	add.w	d0,d0
        //	lea.l	_com_ed_num_table(pc),a0
        //	move.w	(a0,d0.w),d0
        //	jmp	(a0,d0.w)

        //_com_ed_num_table:
        //	.dc.w	0
        //	.dc.w	_com_ed_1-_com_ed_num_table
        //	.dc.w	_com_ed_2-_com_ed_num_table
        //	.dc.w	_com_ed_3-_com_ed_num_table
        //	.dc.w	_com_ed_4-_com_ed_num_table

        //_com_ed_1:
        //	lea.l	w_we_pattern1(a5),a3
        //	clr.b	w_we_exec(a3)
        //	moveq.l	#0,d1
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	bmi	_com_ed_sw_common

        //	bsr	_com_ed_common
        //	bmi	1f
        //	ori.b	#$81,w_weffect(a5)
        //	rts
        //1:
        //	andi.b	#$FE,w_weffect(a5)
        //	rts

        //_com_ed_2:
        //	lea.l	w_we_pattern2(a5),a3
        //	clr.b	w_we_exec(a3)
        //	moveq.l	#1,d1
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	bmi	_com_ed_sw_common

        //	bsr	_com_ed_common
        //	tst.l	d2
        //	bmi	1f
        //	ori.b	#$82,w_weffect(a5)
        //	rts
        //1:
        //	andi.b	#$FD,w_weffect(a5)
        //	rts

        //_com_ed_3:
        //	lea.l	w_we_pattern3(a5),a3
        //	clr.b	w_we_exec(a3)
        //	moveq.l	#2,d1
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	bmi	_com_ed_sw_common

        //	bsr	_com_ed_common
        //	tst.l	d2
        //	bmi	1f
        //	ori.b	#$84,w_weffect(a5)
        //	rts
        //1:
        //	andi.b	#$FB,w_weffect(a5)
        //	rts

        //_com_ed_4:
        //	lea.l	w_we_pattern4(a5),a3
        //	clr.b	w_we_exec(a3)
        //	moveq.l	#3,d1
        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	bmi	_com_ed_sw_common

        //	bsr	_com_ed_common
        //	tst.l	d2
        //	bmi	1f
        //	ori.b	#$88,w_weffect(a5)
        //	rts
        //1:
        //	andi.b	#$F7,w_weffect(a5)
        //	rts

        //_com_ed_sw_common:
        //	andi.w	#7,d0
        //	addq.w	#1,d0
        //	add.w	d0,d0
        //	lea.l	_com_ed_sw_table(pc),a0
        //	move.w	(a0,d0.w),d0
        //	jmp	(a0,d0.w)

        //_com_ed_sw_table:
        //	.dc.w	0
        //	.dc.w	_com_ed_80-_com_ed_sw_table
        //	.dc.w	_com_ed_81-_com_ed_sw_table
        //	.dc.w	_com_ed_82-_com_ed_sw_table
        //	.dc.w	_com_ed_83-_com_ed_sw_table
        //	.dc.w	_com_ed_84-_com_ed_sw_table

        //;
        //;	ON
        //;
        //_com_ed_80:
        //	moveq.l	#1,d0
        //	lsl.l	d1,d0
        //	ori.b	#$80,d0
        //	or.b	d0,w_weffect(a5)
        //	moveq.l	#0,d0
        //	rts

        //;
        //;	OFF
        //;
        //_com_ed_81:
        //	moveq.l	#1,d0
        //	lsl.l	d1,d0
        //	not.b	d0
        //	and.b	d0,w_weffect(a5)

        //	lea.l	w_we_ycom_adrs(a5),a0
        //	andi.w	#3,d0
        //	add.w	d0,d0
        //	add.w	d0,d0
        //	lea.l	(a0,d0.w),a0
        //	movea.l	(a0),a0
        //	move.w	w_we_reset(a3),d0
        //	jsr	(a0)
        //	moveq.l	#0,d0
        //	rts

        //;
        //;	always
        //;
        //_com_ed_82:
        //	clr.b	w_we_exec(a3)
        //	moveq.l	#0,d0
        //	rts

        //;
        //;	at keyon
        //;
        //_com_ed_83:
        //	move.b	#$81,w_we_exec(a3)
        //	moveq.l	#0,d0
        //	rts

        //;
        //;	at keyoff
        //;
        //_com_ed_84:
        //	move.b	#$82,w_we_exec(a3)
        //	moveq.l	#0,d0
        //	rts

        //_com_ed_common:
        //	lea.l	w_we_ycom_adrs(a5),a0
        //	andi.w	#3,d0
        //	add.w	d0,d0
        //	add.w	d0,d0
        //	lea.l	(a0,d0.w),a0
        //	movea.l	(a0),a0
        //	move.l	a0,w_we_exec_adrs(a3)

        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	bsr	_get_wave_memory_ed

        //	move.b	(a1)+,w_we_delay(a3)
        //	move.b	(a1)+,d1
        //	move.b	d1,w_we_speed(a3)

        //	moveq.l	#0,d0
        //	move.b	(a1)+,d0
        //	move.b	_com_ed_sync_table(pc,d0.w),w_we_mode(a3)

        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	move.w	d0,w_we_reset(a3)

        //	clr.b	w_we_exec_flag(a3)
        //	clr.b	w_we_loop_flag(a3)
        //	move.b	w_we_delay(a3),d0
        //	add.b	d1,d0
        //	move.b	d0,w_we_delay_work(a3)
        //	move.l	w_we_start(a3),w_we_adrs_work(a3)
        //	move.l	w_we_loop_start(a3),w_we_start_adrs_work(a3)
        //	move.l	w_we_loop_end(a3),w_we_end_adrs_work(a3)
        //	move.l	w_we_loop_count(a3),w_we_lp_cnt_work(a3)
        //	moveq.l	#0,d0
        //	rts

        //_com_ed_sync_table:
        //	.dc.b	$80,$00,$01,$02

        //	.quad
        //;─────────────────────────────────────
        //_get_wave_memory_ed:
        //	move.l	WAVE_PTR(a6),d1
        //	beq	_com_ed_wm_err_exit
        //	movea.l	d1,a2
        //	move.l	d1,d5

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a2)+,d1
        //	swap	d1
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a2)+,d1
        //	tst.l	d1
        //	beq	_com_ed_wm_err_exit

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d1
        //	move.b	(a2)+,d1
        //	tst.w	d1
        //	beq	_com_ed_wm_err_exit

        //_com_ed_wm10:
        //	move.b	(a2),-(sp)
        //	move.w	(sp)+,d2
        //	move.b	1(a2),d2
        //	swap	d2
        //	move.b	2(a2),-(sp)
        //	move.w	(sp)+,d2
        //	move.b	3(a2),d2
        //	cmp.w	4(a2),d0
        //	beq	_com_ed_wm20
        //	lea.l	(a2,d2.l),a2
        //	dbra	d1,_com_ed_wm10
        //	bra	_com_ed_wm_err_exit

        //_com_ed_wm20:
        //	addq.w	#6,a2

        //	move.b	(a2)+,d0
        //	move.b	d0,d1
        //	andi.b	#$F,d1
        //	move.b	d1,w_we_count(a3)
        //	lsr.b	#4,d0
        //	move.b	d0,w_we_ko_flag(a3)

        //	move.b	(a2)+,d0

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_we_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_we_loop_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_we_loop_end(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	move.l	d0,w_we_loop_count(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_we_ko_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_we_ko_loop_start(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	add.l	d5,d0
        //	move.l	d0,w_we_ko_loop_end(a3)

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	move.l	d0,w_we_ko_loop_count(a3)
        //	moveq.l	#0,d2
        //	rts

        //_com_ed_wm_err_exit:
        //	moveq.l	#-1,d2
        //	rts

        //;─────────────────────────────────────
        //;	hardware LFO delay
        //;		[$EF] + [delay]b
        //;
        //_COM_EF:
        //	bset.b	#1,w_flag(a5)
        //	bset.b	#2,w_flag2(a5)
        //	lea.l	w_v_pattern4(a5),a4
        //	move.b	(a1)+,d0
        //	cmpi.b	#$FF,d0
        //	beq	_COM_EF_add

        //	move.b	d0,w_l_keydelay(a4)
        //	add.b	w_l_lfo_sp(a4),d0
        //	move.b	d0,w_l_delay_work(a4)
        //	rts

        //_COM_EF_add:
        //	move.b	(a1)+,d0
        //	move.b	w_l_keydelay(a4),d1
        //	add.b	d0,d1
        //	add.b	w_l_lfo_sp(a4),d1
        //	move.b	d1,w_l_keydelay(a4)
        //	move.b	d1,w_l_delay_work(a4)
        //	rts


        //;─────────────────────────────────────
        //;	永久ループポイントマーク
        //;			[$F9]
        //_COM_F9:
        //	move.l	a1,w_loop(a5)
        //	rts

        //;─────────────────────────────────────
        //;	リピート抜け出し
        //;			[$FB] + [終端コマンドへのオフセット]w
        //_COM_FB:
        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	lea.l	1(a1,d0.w),a0
        //	move.b	(a0)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a0)+,d0
        //	cmpi.b	#1,2(a0,d0.w)
        //	bne	1f
        //	movea.l	a0,a1
        //1:	rts

        //;─────────────────────────────────────
        //;	リピート開始
        //;			[$FC] + [リピート回数]b + [$00]b
        //_COM_FC:
        //	move.b	(a1)+,(a1)+
        //	rts

        //;─────────────────────────────────────
        //;	リピート終端
        //;			[$FD] + [開始コマンドへのオフセット]w
        //_COM_FD:
        //	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	tst.b	1(a1,d0.w)
        //	beq	1f
        //	subq.b	#1,2(a1,d0.w)
        //	bne	1f
        //	rts
        //1:	lea.l	3(a1,d0.w),a1
        //	rts

        //;─────────────────────────────────────
        //;	tempo 設定
        //;
        //_COM_FE:
        //	move.b	(a1)+,TEMPO(a6)
        //	rts


        //;─────────────────────────────────────
        //_all_end_check:
        //	move.w	USE_TRACK(a6),d0
        //	lea.l	TRACKWORKADR(a6),a0
        //1:	tst.b	w_flag(a0)
        //	bmi	2f
        //	lea.l	_track_work_size(a0),a0
        //	subq.w	#1,d0
        //	bne	1b
        //	move.b	#$20,DRV_STATUS(a6)
        //	move.w	#-1,LOOP_COUNTER(a6)
        //	moveq.l	#2,d0
        //	bra	SUBEVENT
        //2:
        //	rts

        //	.quad


    }
}
