using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class devmpcm
    {
        public reg reg;
        public MXDRV.xMemory mm;
        public mndrv mndrv;
        public comanalyze comanalyze;
        public comcmds comcmds;
        public comlfo comlfo;
        public comwave comwave;

        // 未解決ジャンプアドレス

        //;
        //;	part of MPCM
        //;
        //;─────────────────────────────────────
        //_mpcm_note_set:
        //	move.b d0, w_key(a5)

        //    moveq.l	#0,d2
        //	move.b  d0, d2

        //    tst.b w_pcm_tone(a5)
        //    beq _mpcm_note_keyon

        //    lsl.w	#6,d2
        //	add.w w_detune(a5), d2
        //    bpl @f
        //    moveq.l	#0,d2
        //@@:

        //    move.w d2, w_keycode2(a5)
        //    move.w d2, w_keycode3(a5)
        //    pea _mpcm_keyon(pc)
        //    bsr _mpcm_freq
        //    bsr _init_lfo
        //    bra _init_lfo_mpcm

        //;─────────────────────────────────────
        //;
        //_init_lfo_mpcm:
        //	tst.b FADEFLAG(a6)

        //    bne	1f

        //	moveq.l	#$70,d5
        //	and.b   w_lfo(a5), d5
        //    beq	9f


        //    move.b w_vol(a5), d4
        //    bsr _MPCM_F2_lfo
        //1:

        //    move.b w_lfo(a5), d5
        //    lsl.b	#2,d5
        //	bcc	1f

        //    lea.l w_v_pattern3(a5), a4
        //    bsr _init_lfo_common_a
        //1:

        //    lsl.b	#1,d5
        //	bcc	1f

        //    lea.l w_v_pattern2(a5), a4
        //    bsr _init_lfo_common_a
        //1:

        //    lsl.b	#1,d5
        //	bcc	1f

        //    lea.l w_v_pattern1(a5), a4
        //    bra _init_lfo_common_a
        //1:
        //9:

        //    rts

        //;─────────────────────────────────────
        //;
        //_mpcm_freq:
        //	move.w d2, w_keycode(a5)

        //    move.w d2, d1


        //    btst.b	#1,DRV_FLAG(a6)
        //	beq	1f


        //    move.w	#$400,d0
        //	move.b  w_dev(a5), d0
        //    trap	#1
        //1:

        //    rts


        //;─────────────────────────────────────
        //;	NOTE KEY ON
        //;
        //_mpcm_note_keyon_pdx:
        //	move.l PCMBUFADR(a6),d5
        //   beq _mpcm_note_keyon_set_noload
        //   movea.l d5, a2
        //   moveq.l	#0,d4
        //	moveq.l	#0,d5
        //	move.b w_bank(a5), d5
        //   move.b d5, d0
        //   lsl.w	#8,d0
        //	move.b d2, d0
        //   cmp.w w_banktone(a5), d0
        //   beq _mpcm_pdx_keyon
        //   move.w d0, w_banktone(a5)

        //   move.b d2, d4
        //   lsl.w	#5,d5
        //	add.l d5, d4
        //   add.l d5, d5
        //   add.l d5, d4
        //   lsl.l	#3,d4
        //	move.l  (a2, d4.l), d0
        //   move.l  4(a2, d4.l), d1
        //   add.l a2, d0
        //   cmp.l a2, d0
        //   bcs _mpcm_note_keyon_set_noload
        //   cmp.l PCMBUF_ENDADR(a6), d0
        //   bcc _mpcm_note_keyon_set_noload

        //   movea.l MPCMWORKADR(a6), a2
        //   move.b w_pcmmode(a5), P_SEL(a2)
        //   move.l d0, P_ADDRESS(a2)
        //   move.l d1, P_LENGTH(a2)
        //   move.l d1, P_LOOP_END(a2)
        //   addq.w	#2,a2
        //	exg.l a1, a2
        //   move.w	#$200,d0
        //	move.b w_dev(a5), d0
        //   moveq.l	#0,d1
        //	trap	#1
        //	movea.l a2, a1

        //   lsl.w	#6,d2
        //	move.w d2, w_keycode2(a5)
        //   move.w d2, w_keycode(a5)

        //_mpcm_pdx_keyon:

        //   btst.b	#1,DRV_FLAG(a6)
        //	beq _mpcm_keyon

        //   move.b w_pan_ampm(a5), d1
        //   move.w	#$600,d0
        //	move.b w_dev(a5), d0
        //   trap	#1
        //	bra _mpcm_keyon


        //;─────────────────────────────────────
        //;	NOTE KEY ON
        //;
        //_mpcm_note_keyon:
        //	move.b DRV_FLAG(a6),d5
        //   btst.l	#1,d5
        //	beq _mpcm_note_keyon_set_noload

        //   btst.l	#4,d5
        //	bne _mpcm_note_keyon_pdx

        //   moveq.l	#0,d5
        //	move.b w_bank(a5), d5
        //   lsl.w	#7,d5
        //	or.b d2, d5

        //   cmp.w w_banktone(a5), d5
        //   beq _mpcm_keyon

        //   movea.l MPCMWORKADR(a6), a2
        //   move.l ZPDCOUNT(a6), d0
        //   beq _mpcm_note_keyon_set_noload
        //_mnk_loop

        //   cmp.w   (a2), d5
        //   bne	1f

        //   addq.w	#2,a2
        //	move.w d5, w_banktone(a5)
        //   bra _mpcm_note_keyon_set
        //1:

        //   lea.l _pcm_work_size(a2), a2
        //   subq.l	#1,d0
        //	bne _mnk_loop
        //   rts


        //_mpcm_note_keyon_set:

        //   exg.l a1, a2
        //   move.w	#$200,d0
        //	move.b w_dev(a5), d0
        //   moveq.l	#0,d1
        //	trap	#1
        //	movea.l a2, a1

        //   lsl.w	#6,d2
        //	move.w d2, w_keycode2(a5)
        //   move.w d2, w_keycode(a5)


        //; key on へ
        //;─────────────────────────────────────
        //;	KEY ON
        //;
        //_mpcm_keyon:
        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f

        //	bsr	_wave_init_kon
        //	clr.b	w_revexec(a5)

        //	tst.b	w_flag2(a5)
        //	bpl	_mpcm_keyon_nomask

        //	move.w	#$100,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	rts

        //_mpcm_keyon_nomask:
        //	bset.b	#5,w_flag(a5)
        //	btst.b	#6,w_flag3(a5)
        //	bne	1f
        //	btst.b	#6,w_flag(a5)
        //	beq	1f
        //	rts
        //1:
        //	bclr.b	#2,w_flag(a5)

        //	tst.b	w_reverb(a5)
        //	bpl	1f
        //	bsr	_mpcm_echo_ret
        //1:
        //	move.b	#5,w_e_p(a5)

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f
        //	moveq.l	#0,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	rts


        //;─────────────────────────────────────
        //_mpcm_note_keyon_set_noload:
        //	lsl.w	#6,d2
        //	move.w	d2,w_keycode2(a5)
        //	move.w	d2,w_keycode(a5)
        //	rts

        //;─────────────────────────────────────
        //;	KEY OFF
        //;
        //_mpcm_keyoff:
        //	bsr	_wave_init_kof
        //	bclr.b	#7,w_lfo(a5)

        //	tst.b	w_kom(a5)
        //	bne	1f
        //_mpcm_keyoff2:

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f

        //	move.w	#$100,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	move.b	#4,w_e_p(a5)
        //	andi.b	#$98,w_flag(a5)
        //	rts

        //;─────────────────────────────────────
        //;
        //_mpcm_echo:
        //	btst.b	#5,w_flag(a5)
        //	bne	1f
        //	rts
        //1:
        //	move.w	d4,-(sp)

        //	st.b	w_revexec(a5)
        //	andi.b	#$98,w_flag(a5)
        //	move.b	w_reverb_time(a5),w_reverb_time_work(a5)

        //	moveq.l	#7,d5
        //	and.b	w_reverb(a5),d5
        //	addq.w	#1,d5
        //	add.w	d5,d5
        //	move.w	_mpcm_echo_table(pc,d5.w),d5
        //	jsr	_mpcm_echo_table(pc,d5.w)

        //	move.w	(sp)+,d4
        //	rts

        //;─────────────────────────────────────
        //_mpcm_echo_table:
        //	.dc.w	0
        //	.dc.w	_mpcm_echo_volume-_mpcm_echo_table
        //	.dc.w	_mpcm_echo_volume_pan-_mpcm_echo_table
        //	.dc.w	_mpcm_echo_volume_tone-_mpcm_echo_table
        //	.dc.w	_mpcm_echo_volume_pan_tone-_mpcm_echo_table
        //	.dc.w	_mpcm_echo_volume_-_mpcm_echo_table

        //;─────────────────────────────────────
        //_mpcm_echo_volume:
        //	btst.b	#4,w_reverb(a5)
        //	bne	_mpcm_echo_common_atv
        //	btst.b	#4,w_flag3(a5)
        //	beq	_mpcm_echo_common_atv
        //	bra	_mpcm_echo_common_v

        //_mpcm_echo_volume_pan_tone:
        //	bsr	_mpcm_echo_tone
        //_mpcm_echo_volume_pan:
        //	bsr	_mpcm_echo_pan
        //	bra	_mpcm_echo_volume

        //_mpcm_echo_volume_tone:
        //	bsr	_mpcm_echo_tone
        //	bra	_mpcm_echo_volume

        //_mpcm_echo_volume_:
        //	btst.b	#4,w_reverb(a5)
        //	bne	_mpcm_echo_volume_atv
        //	btst.b	#4,w_flag3(a5)
        //	beq	_mpcm_echo_volume_atv
        //	bra	_mpcm_echo_volume_v



        //;─────────────────────────────────────
        //_mpcm_echo_pan:
        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	_mpcm_echo_pan_exit
        //	move.b	w_reverb_pan(a5),d1
        //	move.w	#$600,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //_mpcm_echo_pan_exit:
        //	rts

        //;─────────────────────────────────────
        //_mpcm_echo_tone:
        //	moveq.l	#0,d5
        //	move.b	w_reverb_tone(a5),d5
        //	bra	_mpcm_echo_tone_change

        //;─────────────────────────────────────
        //; v通常
        //;
        //_mpcm_echo_common_v:
        //	btst.b	#3,w_reverb(a5)
        //	bne	_mpcm_echo_direct_v

        //	moveq.l	#0,d4
        //	move.b	w_volume(a5),d4

        //	move.b	w_reverb_vol(a5),d0
        //	bpl	_mpcm_echo_plus_v

        //	add.b	d0,d4
        //	bpl	1f
        //	moveq.l	#0,d4
        //1:
        //	lea.l	w_voltable(a5),a0
        //	move.b	(a0,d4.w),d4
        //	bra	_MPCM_F2_softenv

        //_mpcm_echo_plus_v:
        //	add.b	d0,d4
        //	bpl	1f
        //	moveq.l	#$7F,d4
        //1:
        //	cmp.b	w_volcount(a5),d4
        //	bcs	1f
        //	move.b	w_volcount(a5),d4
        //	subq.b	#1,d4
        //1:
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //; @v通常
        //;
        //_mpcm_echo_common_atv:
        //	btst.b	#3,w_reverb(a5)
        //	bne	_mpcm_echo_direct_atv

        //	move.b	w_vol(a5),d4

        //	move.b	w_reverb_vol(a5),d0
        //	bpl	_mpcm_echo_plus

        //	add.b	d0,d4
        //	bpl	_MPCM_F2_softenv
        //	moveq.l	#0,d4
        //	bra	_MPCM_F2_softenv

        //_mpcm_echo_plus:
        //	add.b	d0,d4
        //	bpl	_MPCM_F2_softenv
        //	moveq.l	#$7F,d4
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //; v微調整
        //;
        //_mpcm_echo_volume_v:
        //	moveq.l	#0,d4
        //	move.b	w_volume(a5),d4
        //	move.b	w_reverb_vol(a5),d0
        //	bpl	_mpcm_echo_vol_plus

        //	add.b	d0,d4
        //	bpl	1f
        //	moveq.l	#0,d4
        //1:
        //	lsr.b	#1,d4
        //	lea.l	w_voltable(a5),a0
        //	move.b	(a0,d4.w),d4
        //	bra	_MPCM_F2_softenv

        //_mpcm_echo_vol_plus:
        //	add.b	d0,d4
        //	lsr.b	#1,d4
        //	andi.w	#$7F,d4
        //	lea.l	w_voltable(a5),a0
        //	move.b	(a0,d4.w),d4
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //; @v微調整
        //;
        //_mpcm_echo_volume_atv:
        //	move.b	w_vol(a5),d4
        //	move.b	w_reverb_vol(a5),d0
        //	add.b	d0,d4
        //	lsr.b	#1,d4
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //; v直接
        //;
        //_mpcm_echo_direct_v:
        //	moveq.l	#0,d4
        //	move.b	w_reverb_vol(a5),d4
        //	lea.l	w_voltable(a5),a0
        //	move.b	(a0,d4.w),d4
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //; @v直接
        //;
        //_mpcm_echo_direct_atv:
        //	move.b	w_reverb_vol(a5),d4
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //_mpcm_echo_ret:
        //	move.w	d4,-(sp)

        //	clr.b	w_revexec(a5)
        //	clr.b	w_reverb_time_work(a5)

        //	btst.b	#1,w_reverb(a5)
        //	beq	1f

        //	move.b	w_program2(a5),d5
        //	bsr	_mpcm_echo_tone_change
        //1:
        //	move.b	w_vol(a5),d4
        //	btst.b	#4,w_flag3(a5)
        //	beq	_mpcm_echo_ret_atv

        //	moveq.l	#0,d4
        //	move.b	w_volume(a5),d4
        //	lea.l	w_voltable(a5),a0
        //	move.b	(a0,d4.w),d4

        //_mpcm_echo_ret_atv:
        //	bsr	_MPCM_F2_softenv

        //	move.b	w_reverb(a5),d5
        //	lsr.b	#1,d5
        //	bcc	1f

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f

        //	move.b	w_pan_ampm(a5),d1
        //	move.w	#$600,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	move.w	(sp)+,d4
        //	rts

        //;─────────────────────────────────────
        //;	MML コマンド処理 ( PCM 部 )
        //;
        //_mpcm_command:
        //	add.w	d0,d0
        //	move.w	_mpcmc(pc,d0.w),d0
        //	jmp	_mpcmc(pc,d0.w)

        //_mpcmc:
        //	.dc.w	0
        //	.dc.w	_COM_81-_mpcmc		; 81
        //	.dc.w	_MPCM_82-_mpcmc		; 82	key off
        //	.dc.w	_COM_83-_mpcmc		; 83	すらー
        //	.dc.w	_MPCM_NOP-_mpcmc	; 84
        //	.dc.w	_MPCM_NOP-_mpcmc	; 85
        //	.dc.w	_COM_86-_mpcmc		; 86	同期信号送信
        //	.dc.w	_COM_87-_mpcmc		; 87	同期信号待ち
        //	.dc.w	_OPM_88-_mpcmc		; 88	ぴっちべんど
        //	.dc.w	_OPM_89-_mpcmc		; 89	ぽるためんと
        //	.dc.w	_OPM_8A-_mpcmc		; 8A	ぽるためんと係数変更
        //	.dc.w	_MPCM_NOP-_mpcmc	; 8B
        //	.dc.w	_MPCM_NOP-_mpcmc	; 8C
        //	.dc.w	_MPCM_NOP-_mpcmc	; 8D
        //	.dc.w	_MPCM_NOP-_mpcmc	; 8E
        //	.dc.w	_MPCM_NOP-_mpcmc	; 8F

        //	.dc.w	_COM_90-_mpcmc		; 90	q
        //	.dc.w	_COM_91-_mpcmc		; 91	@q
        //	.dc.w	_COM_94-_mpcmc		; 92	ノートオフモード
        //	.dc.w	_COM_93-_mpcmc		; 93	negative @q
        //	.dc.w	_COM_94-_mpcmc		; 94	keyoff mode
        //	.dc.w	_MPCM_NOP-_mpcmc	; 95
        //	.dc.w	_MPCM_NOP-_mpcmc	; 96
        //	.dc.w	_MPCM_NOP-_mpcmc	; 97
        //	.dc.w	_MPCM_98-_mpcmc		; 98	擬似リバーブ
        //	.dc.w	_MPCM_99-_mpcmc		; 99
        //	.dc.w	_COM_9A-_mpcmc		; 9A	擬似動作 step time
        //	.dc.w	_MPCM_NOP-_mpcmc	; 9B
        //	.dc.w	_MPCM_NOP-_mpcmc	; 9C
        //	.dc.w	_MPCM_NOP-_mpcmc	; 9D
        //	.dc.w	_MPCM_NOP-_mpcmc	; 9E
        //	.dc.w	_MPCM_NOP-_mpcmc	; 9F

        //	.dc.w	_MPCM_F0-_mpcmc		; A0	音色切り替え
        //	.dc.w	_MPCM_A1-_mpcmc		; A1	バンク&音色切り替え
        //	.dc.w	_MPCM_A2-_mpcmc		; A2	モード切り替え
        //	.dc.w	_MPCM_A3-_mpcmc		; A3	音量テーブル
        //	.dc.w	_MPCM_F2-_mpcmc		; A4	音量
        //	.dc.w	_MPCM_F5-_mpcmc		; A5
        //	.dc.w	_MPCM_F6-_mpcmc		; A6
        //	.dc.w	_MPCM_A7-_mpcmc		; A7	127段階音量テーブル切り替え
        //	.dc.w	_COM_A8-_mpcmc		; A8	相対音量モード
        //	.dc.w	_MPCM_NOP-_mpcmc	; A9
        //	.dc.w	_MPCM_NOP-_mpcmc	; AA
        //	.dc.w	_MPCM_NOP-_mpcmc	; AB
        //	.dc.w	_MPCM_NOP-_mpcmc	; AC
        //	.dc.w	_MPCM_NOP-_mpcmc	; AD
        //	.dc.w	_MPCM_NOP-_mpcmc	; AE
        //	.dc.w	_MPCM_NOP-_mpcmc	; AF

        //	.dc.w	_COM_B0-_mpcmc		; B0
        //	.dc.w	_MPCM_NOP-_mpcmc	; B1
        //	.dc.w	_MPCM_NOP-_mpcmc	; B2
        //	.dc.w	_MPCM_NOP-_mpcmc	; B3
        //	.dc.w	_MPCM_NOP-_mpcmc	; B4
        //	.dc.w	_MPCM_NOP-_mpcmc	; B5
        //	.dc.w	_MPCM_NOP-_mpcmc	; B6
        //	.dc.w	_MPCM_NOP-_mpcmc	; B7
        //	.dc.w	_MPCM_NOP-_mpcmc	; B8
        //	.dc.w	_MPCM_NOP-_mpcmc	; B9
        //	.dc.w	_MPCM_NOP-_mpcmc	; BA
        //	.dc.w	_MPCM_NOP-_mpcmc	; BB
        //	.dc.w	_MPCM_NOP-_mpcmc	; BC
        //	.dc.w	_MPCM_NOP-_mpcmc	; BD
        //	.dc.w	_COM_BE-_mpcmc		; BE	ジャンプ
        //	.dc.w	_COM_BF-_mpcmc		; BF

        //	; PSG 系
        //	.dc.w	_COM_C0-_mpcmc		; C0	ソフトウェアエンベロープ 1
        //	.dc.w	_COM_C1-_mpcmc		; C1	ソフトウェアエンベロープ 2
        //	.dc.w	_MPCM_NOP-_mpcmc	; C2
        //	.dc.w	_COM_C3-_mpcmc		; C3	switch
        //	.dc.w	_COM_C4-_mpcmc		; C4	env (num)
        //	.dc.w	_COM_C5-_mpcmc		; C5	env (bank + num)
        //	.dc.w	_MPCM_NOP-_mpcmc	; C6
        //	.dc.w	_MPCM_NOP-_mpcmc	; C7
        //	.dc.w	_MPCM_NOP-_mpcmc	; C8
        //	.dc.w	_MPCM_NOP-_mpcmc	; C9
        //	.dc.w	_MPCM_NOP-_mpcmc	; CA
        //	.dc.w	_MPCM_NOP-_mpcmc	; CB
        //	.dc.w	_MPCM_NOP-_mpcmc	; CC
        //	.dc.w	_MPCM_NOP-_mpcmc	; CD
        //	.dc.w	_MPCM_NOP-_mpcmc	; CE
        //	.dc.w	_MPCM_NOP-_mpcmc	; CF

        //	; KEY 系
        //	.dc.w	_COM_D0-_mpcmc		; D0	キートランスポーズ
        //	.dc.w	_COM_D1-_mpcmc		; D1	相対キートランスポーズ
        //	.dc.w	_MPCM_NOP-_mpcmc	; D2
        //	.dc.w	_MPCM_NOP-_mpcmc	; D3
        //	.dc.w	_MPCM_NOP-_mpcmc	; D4
        //	.dc.w	_MPCM_NOP-_mpcmc	; D5
        //	.dc.w	_MPCM_NOP-_mpcmc	; D6
        //	.dc.w	_MPCM_NOP-_mpcmc	; D7
        //	.dc.w	_COM_D8-_mpcmc		; D8	ディチューン
        //	.dc.w	_COM_D9-_mpcmc		; D9	相対ディチューン
        //	.dc.w	_MPCM_NOP-_mpcmc	; DA
        //	.dc.w	_MPCM_NOP-_mpcmc	; DB
        //	.dc.w	_MPCM_NOP-_mpcmc	; DC
        //	.dc.w	_MPCM_NOP-_mpcmc	; DD
        //	.dc.w	_MPCM_NOP-_mpcmc	; DE
        //	.dc.w	_MPCM_NOP-_mpcmc	; DF

        //	; LFO 系
        //	.dc.w	_MPCM_NOP-_mpcmc	; E0
        //	.dc.w	_MPCM_NOP-_mpcmc	; E1
        //	.dc.w	_COM_E2-_mpcmc		; E2	pitch LFO
        //	.dc.w	_COM_E3-_mpcmc		; E3	pitch LFO switch
        //	.dc.w	_COM_E4-_mpcmc		; E4	pitch LFO delay
        //	.dc.w	_MPCM_NOP-_mpcmc	; E5
        //	.dc.w	_MPCM_NOP-_mpcmc	; E6
        //	.dc.w	_COM_E7-_mpcmc		; E7	amp LFO
        //	.dc.w	_MPCM_E8-_mpcmc		; E8	amp LFO switch
        //	.dc.w	_COM_E9-_mpcmc		; E9	amp LFO delay
        //	.dc.w	_MPCM_NOP-_mpcmc	; EA
        //	.dc.w	_MPCM_NOP-_mpcmc	; EB
        //	.dc.w	_MPCM_NOP-_mpcmc	; EC
        //	.dc.w	_COM_ED-_mpcmc		; ED
        //	.dc.w	_MPCM_NOP-_mpcmc	; EE
        //	.dc.w	_MPCM_NOP-_mpcmc	; EF

        //	; システムコントール系
        //	.dc.w	_MPCM_F0-_mpcmc		; F0	@
        //	.dc.w	_MPCM_NOP-_mpcmc	; F1
        //	.dc.w	_MPCM_F2-_mpcmc		; F2	volume
        //	.dc.w	_MPCM_F3-_mpcmc		; F3	F
        //	.dc.w	_MPCM_F4-_mpcmc		; F4	pan
        //	.dc.w	_MPCM_F5-_mpcmc		; F5	)	くれ
        //	.dc.w	_MPCM_F6-_mpcmc		; F6	(	でくれ
        //	.dc.w	_MPCM_NOP-_mpcmc	; F7
        //	.dc.w	_MPCM_NOP-_mpcmc	; F8
        //	.dc.w	_COM_F9-_mpcmc		; F9	永久ループポイントマーク
        //	.dc.w	_OPM_FA-_mpcmc		; FA	Y COMMAND
        //	.dc.w	_COM_FB-_mpcmc		; FB	リピート抜け出し
        //	.dc.w	_COM_FC-_mpcmc		; FC	リピート開始
        //	.dc.w	_COM_FD-_mpcmc		; FD	リピート終端
        //	.dc.w	_COM_FE-_mpcmc		; FE	tempo
        //	.dc.w	_MPCM_FF-_mpcmc		; FF	end of data

        //;─────────────────────────────────────
        //;
        //_MPCM_NOP:
        //	andi.b	#$7F,w_flag(a5)
        //	bra	_mpcm_keyoff2


        //;─────────────────────────────────────
        //;	強制キーオフ
        //;
        //_MPCM_82:
        //	bra	_mpcm_keyoff2


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
        //_MPCM_98:
        //	bsr	_COM_98

        //	btst.b	#7,w_reverb(a5)
        //	beq	_mpcm_keyoff2
        //	rts

        //;─────────────────────────────────────
        //;	擬似エコー
        //;
        //_MPCM_99:
        //	bra	_COM_99

        //;─────────────────────────────────────
        //;	bank & tone set
        //;		[$A1] + [bank]b + [tone]b
        //_MPCM_A1:
        //	move.b	(a1)+,w_bank(a5)
        //	bra	_MPCM_F0

        //;─────────────────────────────────────
        //;	TONE / TIMBRE
        //;		[$A2] + [switch]b
        //_MPCM_A2:
        //	clr.b	w_bank(a5)
        //	clr.b	w_program(a5)
        //	clr.b	w_program2(a5)
        //	clr.b	w_pcm_tone(a5)
        //	st.b	w_banktone(a5)
        //	st.b	w_pcmmode(a5)

        //	move.b	(a1)+,d0
        //	bmi	_MPCM_A2_mx
        //	sne.b	w_pcm_tone(a5)
        //	rts

        //_MPCM_A2_mx:
        //	addq.b	#1,d0
        //	beq	_MPCM_A2_8
        //	addq.b	#1,d0
        //	beq	_MPCM_A2_16
        //	rts
        //_MPCM_A2_8:
        //	move.b	#2,w_pcmmode(a5)
        //	rts
        //_MPCM_A2_16:
        //	move.b	#1,w_pcmmode(a5)
        //	rts

        //;─────────────────────────────────────
        //;	音量テーブル
        //;
        //_MPCM_A3:
        //	bsr	_COM_A3

        //	btst.b	#4,w_flag3(a5)
        //	beq	_MPCM_A3_exit
        //	moveq.l	#0,d4
        //	move.b	w_volume(a5),d4
        //	lea.l	w_voltable(a5),a0
        //	move.b	(a2,d4.w),d4
        //	bra	_MPCM_F2_v
        //_MPCM_A3_exit:
        //	rts

        //;─────────────────────────────────────
        //;	127段階音量テーブル切り替え
        //;
        //_MPCM_A7:
        //	move.b	(a1)+,d1
        //	move.b	(a1)+,d5

        //	move.l	VOL_PTR(a6),d0
        //	beq	_mpcm_a7_exit

        //	movea.l	d0,a2
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	swap	d0
        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a2)+,d0
        //	move.l	d0,d2
        //	beq	_mpcm_a7_exit

        //	move.b	(a2)+,-(sp)
        //	move.w	(sp)+,d4
        //	move.b	(a2)+,d4

        //_mpcm_a7_ana_loop:
        //	cmp.b	2(a2),d1
        //	bne	1f
        //	cmp.b	3(a2),d5
        //	beq	_mpcm_a7_set
        //1:
        //	subq.w	#1,d4
        //	beq	_mpcm_a7_exit
        //	move.b	(a2),-(sp)
        //	move.w	(sp)+,d0
        //	move.b	1(a2),d0
        //	lea.l	(a2,d0.w),a2
        //	bra	_mpcm_a7_ana_loop

        //_mpcm_a7_set:
        //	addq.w	#4,a2
        //	exg.l	a1,a2
        //	move.w	#$8005,d0
        //	moveq.l	#-1,d1
        //	trap	#1
        //	movea.l	a2,a1
        //_mpcm_a7_exit:
        //	rts

        //;─────────────────────────────────────
        //;	音量 LFO on /off
        //;
        //;	$E8,num,switch
        //_MPCM_E8:
        //	move.b	w_vol(a5),d4
        //	pea	_COM_E8(pc)
        //	bra	_MPCM_F2_lfo


        //;─────────────────────────────────────
        //;	tone set
        //;		[$F0] + [num]b
        //_MPCM_F0:
        //	tst.b	w_reverb(a5)
        //	bpl	1f
        //	bsr	_mpcm_keyoff
        //1:
        //	moveq.l	#0,d5
        //	move.b	(a1)+,d5
        //	move.b	d5,w_program(a5)
        //	move.b	d5,w_program2(a5)
        //	move.w	#-1,w_banktone(a5)

        //	tst.b	w_pcm_tone(a5)
        //	bne	_mpcm_echo_tone_change
        //	move.b	d5,w_bank(a5)
        //	rts

        //_mpcm_echo_tone_change:
        //	move.b	d5,w_program(a5)

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	_mpcm_tone_ana_set_exit

        //	moveq.l	#0,d0
        //	move.b	w_bank(a5),d0
        //	lsl.w	#7,d0
        //	or.b	d5,d0
        //	move.w	d0,d5
        //	bset.l	#15,d5

        //	move.l	MPCMWORKADR(a6),d0
        //	beq	_mpcm_tone_ana_set_exit
        //	movea.l	d0,a2
        //	move.l	ZPDCOUNT(a6),d0
        //	beq	_mpcm_tone_ana_set_exit
        //_mpcm_f0_timbre_ana:
        //	cmp.w	(a2),d5
        //	bne	1f
        //	addq.w	#2,a2
        //	bra	_mpcm_tone_ana_set
        //1:	lea.l	_pcm_work_size(a2),a2
        //	subq.l	#1,d0
        //	bne	_mpcm_f0_timbre_ana
        //	rts

        //_mpcm_tone_ana_set:
        //	exg.l	a1,a2
        //	move.w	#$200,d0
        //	move.b	w_dev(a5),d0
        //	moveq.l	#0,d1
        //	trap	#1
        //	movea.l	a2,a1

        //_mpcm_tone_ana_set_exit:
        //	rts

        //;─────────────────────────────────────
        //;	volume
        //;		[$F2] + [volume]b
        //_MPCM_F2:
        //	bclr.b	#4,w_flag3(a5)
        //	moveq.l	#0,d4
        //	move.b	(a1)+,d4
        //	bpl	1f
        //	neg.b	d4
        //	subq.b	#1,d4
        //	bra	_MPCM_F2_v
        //1:
        //	tst.b	w_volmode(a5)
        //	beq	1f
        //	bset.b	#4,w_flag3(a5)
        //1:
        //	cmp.b	w_volcount(a5),d4
        //	bcs	1f
        //	move.b	w_volcount(a5),d4
        //	subq.b	#1,d4
        //1:
        //	move.b	d4,w_volume(a5)
        //	lea.l	w_voltable(a5),a2
        //	move.b	(a2,d4.w),d4
        //_MPCM_F2_v:
        //	move.b	d4,w_vol(a5)
        //_MPCM_F2_softenv:
        //	move.b	w_track_vol(a5),d0
        //	bpl	1f
        //	add.b	d0,d4
        //	bpl	2f
        //	moveq.l	#0,d4
        //	bra	2f
        //1:
        //	add.b	d0,d4
        //	bpl	2f
        //	moveq.l	#127,d4
        //2:
        //_MPCM_F2_lfo:
        //	sub.b	MASTER_VOL_PCM(a6),d4
        //	bpl	1f
        //	moveq.l	#0,d4
        //1:
        //	move.b	d4,w_vol2(a5)

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f

        //	move.w	#$500,d0
        //	move.b	w_dev(a5),d0
        //	move.b	d4,d1
        //	trap	#1
        //1:
        //	rts

        //;─────────────────────────────────────
        //;	frequency
        //;		[$F3] + [freq]b
        //_MPCM_F3:
        //	moveq.l	#0,d1
        //	move.b	(a1)+,d1

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f
        //	move.w	#$300,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	rts


        //;─────────────────────────────────────
        //;	panpot
        //;		[$F4] + [pan]b
        //_MPCM_F4:
        //	move.b	(a1)+,d1
        //	move.b	d1,w_pan_ampm(a5)

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f
        //	move.w	#$600,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	rts

        //;─────────────────────────────────────
        //;	volup
        //;			[$F5] + [DATA]b
        //;
        //_MPCM_F5:
        //	tst.b	w_volmode(a5)
        //	beq	_MPCM_F5_normal
        //	btst.b	#4,w_flag3(a5)
        //	beq	_MPCM_F5_normal

        //	moveq.l	#0,d4
        //	move.b	w_volume(a5),d4
        //	add.b	(a1)+,d4
        //	cmp.b	w_volcount(a5),d4
        //	bcs	1f
        //	move.b	w_volcount(a5),d4
        //	subq.b	#1,d4
        //1:
        //	move.b	d4,w_volume(a5)
        //	lea.l	w_voltable(a5),a2
        //	move.b	(a2,d4.w),d4
        //	bra	_MPCM_F2_v

        //_MPCM_F5_normal:
        //	move.b	w_vol(a5),d4
        //	add.b	(a1)+,d4
        //	bpl	_MPCM_F2_v
        //	moveq.l	#$7F,d4
        //	bra	_MPCM_F2_v

        //;─────────────────────────────────────
        //;	voldown
        //;			[$F6] + [DATA]b
        //;
        //_MPCM_F6:
        //	tst.b	w_volmode(a5)
        //	beq	_MPCM_F6_normal
        //	btst.b	#4,w_flag3(a5)
        //	beq	_MPCM_F6_normal

        //	moveq.l	#0,d4
        //	move.b	w_volume(a5),d4
        //	sub.b	(a1)+,d4
        //	bpl	1f
        //	moveq.l	#0,d4
        //1:
        //	move.b	d4,w_volume(a5)
        //	lea.l	w_voltable(a5),a2
        //	move.b	(a2,d4.w),d4
        //	bra	_MPCM_F2_v

        //_MPCM_F6_normal:
        //	move.b	w_vol(a5),d4
        //	sub.b	(a1)+,d4
        //	bpl	_MPCM_F2_v
        //	moveq.l	#0,d4
        //	bra	_MPCM_F2_v

        //;─────────────────────────────────────
        //;
        //_MPCM_FF:
        //	bclr.b	#0,w_flag2(a5)

        //	move.w	USE_TRACK(a6),d0
        //	lea.l	TRACKWORKADR(a6),a0
        //1:	btst.b	#0,w_flag2(a0)
        //	bne	3f
        //	lea.l	_track_work_size(a0),a0
        //	subq.w	#1,d0
        //	bne	1b

        //	addq.w	#1,LOOP_COUNTER(a6)
        //	move.w	LOOP_COUNTER(a6),d1
        //	cmpi.w	#-1,d1
        //	bne	1f
        //	moveq.l	#0,d1
        //	move.w	d1,LOOP_COUNTER(a6)
        //1:
        //	moveq.l	#1,d0
        //	bsr	SUBEVENT

        //	move.w	USE_TRACK(a6),d0
        //	lea.l	TRACKWORKADR(a6),a0
        //1:	tst.b	w_flag(a0)
        //	bpl	2f
        //	bset.b	#0,w_flag2(a0)
        //2:	lea.l	_track_work_size(a0),a0
        //	subq.w	#1,d0
        //	bne	1b

        //3:	move.b	(a1)+,-(sp)
        //	move.w	(sp)+,d0
        //	move.b	(a1)+,d0
        //	tst.w	d0
        //	beq	1f
        //	cmpi.w	#$FFFF,d0
        //	beq	2f
        //	lea.l	(a1,d0.w),a1
        //	rts

        //1:
        //	bclr.b	#7,w_flag(a5)
        //	bclr.b	#0,w_flag2(a5)
        //	clr.b	w_lfo(a5)
        //	clr.b	w_weffect(a5)
        //	pea	_all_end_check(pc)
        //	bra	_mpcm_keyoff2

        //2:
        //	movea.l	w_loop(a5),a1
        //	rts

        //;─────────────────────────────────────
        //_ch_mpcm_plfo_table:
        //	.dc.w	0
        //	.dc.w	_ch_mpcm_plfo_1-_ch_mpcm_plfo_table
        //	.dc.w	_ch_mpcm_plfo_2-_ch_mpcm_plfo_table
        //	.dc.w	_ch_mpcm_plfo_3-_ch_mpcm_plfo_table
        //	.dc.w	_ch_mpcm_plfo_4-_ch_mpcm_plfo_table
        //	.dc.w	_ch_mpcm_plfo_5-_ch_mpcm_plfo_table
        //	.dc.w	_ch_mpcm_plfo_6-_ch_mpcm_plfo_table
        //	.dc.w	_ch_mpcm_plfo_7-_ch_mpcm_plfo_table

        //_ch_mpcm_alfo_table:
        //	.dc.w	0
        //	.dc.w	_ch_mpcm_alfo_1-_ch_mpcm_alfo_table
        //	.dc.w	_ch_mpcm_alfo_2-_ch_mpcm_alfo_table
        //	.dc.w	_ch_mpcm_alfo_3-_ch_mpcm_alfo_table
        //	.dc.w	_ch_mpcm_alfo_4-_ch_mpcm_alfo_table
        //	.dc.w	_ch_mpcm_alfo_5-_ch_mpcm_alfo_table
        //	.dc.w	_ch_mpcm_alfo_6-_ch_mpcm_alfo_table
        //	.dc.w	_ch_mpcm_alfo_7-_ch_mpcm_alfo_table

        //;─────────────────────────────────────
        //_ch_mpcm_lfo_job:
        //	bsr	_ch_effect
        //_ch_mpcm_lfo:
        //	move.b	w_lfo(a5),d0
        //	bne	1f
        //	rts
        //1:
        //	clr.w	w_addkeycode(a5)
        //	clr.w	w_addvolume(a5)

        //	moveq.l	#$E,d1
        //	and.b	d0,d1
        //	beq	1f
        //	move.w	d0,-(sp)
        //	move.w	_ch_mpcm_plfo_table(pc,d1.w),d1
        //	jsr	_ch_mpcm_plfo_table(pc,d1.w)
        //	move.w	(sp)+,d0
        //1:
        //	lsr.b	#4,d0
        //	andi.w	#7,d0
        //	beq	_ch_mpcm_lfo_end
        //	add.w	d0,d0
        //	move.w	_ch_mpcm_alfo_table(pc,d0.w),d0
        //	jsr	_ch_mpcm_alfo_table(pc,d0.w)

        //_ch_mpcm_lfo_end:
        //	move.w	w_keycode3(a5),d2

        //	move.w	w_addkeycode(a5),d1
        //	cmp.w	w_addkeycode2(a5),d1
        //	beq	_ch_mpcm_lfo_a
        //	move.w	d1,w_addkeycode2(a5)
        //	bmi	_ch_mpcm_lfo_end_minus

        //	add.w	d1,d2
        //	cmpi.w	#$2000,d2
        //	bcs	_ch_mpcm_lfo_end_common
        //	move.w	#$1FFF,d2
        //	bra	_ch_mpcm_lfo_end_common

        //_ch_mpcm_lfo_end_minus:
        //	add.w	d1,d2
        //	bpl	_ch_mpcm_lfo_end_common
        //	moveq.l	#0,d2

        //_ch_mpcm_lfo_end_common:
        //	bsr	_mpcm_freq

        //_ch_mpcm_lfo_a:
        //	move.w	w_addvolume(a5),d0
        //	cmp.w	w_addvolume2(a5),d0
        //	beq	_ch_mpcm_lfo_a_exit
        //	move.w	d0,w_addvolume2(a5)
        //	bmi	_ch_mpcm_lfo_a_minus

        //	moveq.l	#0,d4
        //	move.b	w_vol(a5),d4
        //	add.w	d0,d4
        //	bpl	_MPCM_F2_lfo
        //	moveq.l	#$7F,d4
        //	bra	_MPCM_F2_lfo

        //_ch_mpcm_lfo_a_minus:
        //	moveq.l	#0,d4
        //	move.b	w_vol(a5),d4
        //	add.w	d0,d4
        //	bpl	_MPCM_F2_lfo
        //	moveq.l	#0,d4
        //	bra	_MPCM_F2_lfo

        //_ch_mpcm_lfo_a_exit:
        //	rts

        //;─────────────────────────────────────
        //_ch_mpcm_plfo_1:
        //	lea.l	w_p_pattern1(a5),a4
        //	lea.l	w_wp_pattern1(a5),a3
        //	bra	_ch_mpcm_p_common

        //_ch_mpcm_plfo_2:
        //	lea.l	w_p_pattern2(a5),a4
        //	lea.l	w_wp_pattern2(a5),a3
        //	bra	_ch_mpcm_p_common

        //_ch_mpcm_plfo_3:
        //	lea.l	w_p_pattern1(a5),a4
        //	lea.l	w_wp_pattern1(a5),a3
        //	bsr	_ch_mpcm_p_common
        //	lea.l	w_p_pattern2(a5),a4
        //	lea.l	w_wp_pattern2(a5),a3
        //	bra	_ch_mpcm_p_common

        //_ch_mpcm_plfo_4:
        //	lea.l	w_p_pattern3(a5),a4
        //	lea.l	w_wp_pattern3(a5),a3
        //	bra	_ch_mpcm_p_common

        //_ch_mpcm_plfo_5:
        //	lea.l	w_p_pattern1(a5),a4
        //	lea.l	w_wp_pattern1(a5),a3
        //	bsr	_ch_mpcm_p_common
        //	lea.l	w_p_pattern3(a5),a4
        //	lea.l	w_wp_pattern3(a5),a3
        //	bra	_ch_mpcm_p_common

        //_ch_mpcm_plfo_6:
        //	lea.l	w_p_pattern2(a5),a4
        //	lea.l	w_wp_pattern2(a5),a3
        //	bsr	_ch_mpcm_p_common
        //	lea.l	w_p_pattern3(a5),a4
        //	lea.l	w_wp_pattern3(a5),a3
        //	bra	_ch_mpcm_p_common

        //_ch_mpcm_plfo_7:
        //	lea.l	w_p_pattern1(a5),a4
        //	lea.l	w_wp_pattern1(a5),a3
        //	bsr	_ch_mpcm_p_common
        //	lea.l	w_p_pattern2(a5),a4
        //	lea.l	w_wp_pattern2(a5),a3
        //	bsr	_ch_mpcm_p_common
        //	lea.l	w_p_pattern3(a5),a4
        //	lea.l	w_wp_pattern3(a5),a3
        //	bra	_ch_mpcm_p_common

        //;─────────────────────────────────────
        //_ch_mpcm_alfo_1:
        //	lea.l	w_v_pattern1(a5),a4
        //	lea.l	w_wv_pattern1(a5),a3
        //	bra	_ch_mpcm_a_common

        //_ch_mpcm_alfo_2:
        //	lea.l	w_v_pattern2(a5),a4
        //	lea.l	w_wv_pattern2(a5),a3
        //	bra	_ch_mpcm_a_common

        //_ch_mpcm_alfo_3:
        //	lea.l	w_v_pattern1(a5),a4
        //	lea.l	w_wv_pattern1(a5),a3
        //	bsr	_ch_mpcm_a_common
        //	lea.l	w_v_pattern2(a5),a4
        //	lea.l	w_wv_pattern2(a5),a3
        //	bra	_ch_mpcm_a_common

        //_ch_mpcm_alfo_4:
        //	lea.l	w_v_pattern3(a5),a4
        //	lea.l	w_wv_pattern3(a5),a3
        //	bra	_ch_mpcm_a_common

        //_ch_mpcm_alfo_5:
        //	lea.l	w_v_pattern1(a5),a4
        //	lea.l	w_wv_pattern1(a5),a3
        //	bsr	_ch_mpcm_a_common
        //	lea.l	w_v_pattern3(a5),a4
        //	lea.l	w_wv_pattern3(a5),a3
        //	bra	_ch_mpcm_a_common

        //_ch_mpcm_alfo_6:
        //	lea.l	w_v_pattern2(a5),a4
        //	lea.l	w_wv_pattern2(a5),a3
        //	bsr	_ch_mpcm_a_common
        //	lea.l	w_v_pattern3(a5),a4
        //	lea.l	w_wv_pattern3(a5),a3
        //	bra	_ch_mpcm_a_common

        //_ch_mpcm_alfo_7:
        //	lea.l	w_v_pattern1(a5),a4
        //	lea.l	w_wv_pattern1(a5),a3
        //	bsr	_ch_mpcm_a_common
        //	lea.l	w_v_pattern2(a5),a4
        //	lea.l	w_wv_pattern2(a5),a3
        //	bsr	_ch_mpcm_a_common
        //	lea.l	w_v_pattern3(a5),a4
        //	lea.l	w_wv_pattern3(a5),a3
        //	bra	_ch_mpcm_a_common


        //;─────────────────────────────────────
        //_ch_mpcm_a_common:
        //	moveq.l	#1,d0
        //	move.b	w_l_pattern(a4),d1
        //	bpl	1f
        //	bsr	_com_wavememory
        //	add.w	d0,w_addvolume(a5)
        //	rts
        //1:
        //	move.w	w_l_flag(a4),d4
        //	bpl	_ch_mpcm_v_com_exec
        //	lsr.b	#1,d4
        //	bcs	_ch_mpcm_v_keyon_only

        //	btst.b	#5,w_flag(a5)
        //	beq	_ch_mpcm_v_com_exec
        //	rts

        //_ch_mpcm_v_keyon_only:
        //	btst.b	#5,w_flag(a5)
        //	bne	_ch_mpcm_v_com_exec
        //	rts

        //_ch_mpcm_v_com_exec:
        //	add.b	d1,d0
        //	add.w	d0,d0
        //	move.w	_mpcm_velocity_pattern(pc,d0.w),d0
        //	jsr	_mpcm_velocity_pattern(pc,d0.w)
        //	sub.w	d1,w_addvolume(a5)
        //	rts

        //_mpcm_velocity_pattern:
        //	.dc.w	0
        //	.dc.w	_com_lfo_saw-_mpcm_velocity_pattern
        //	.dc.w	_com_lfo_portament-_mpcm_velocity_pattern
        //	.dc.w	_com_lfo_triangle-_mpcm_velocity_pattern

        //;─────────────────────────────────────
        //_ch_mpcm_p_common:
        //	moveq.l	#1,d0
        //	move.b	w_l_pattern(a4),d1
        //	bpl	1f
        //	bsr	_com_wavememory
        //	add.w	d0,w_addkeycode(a5)
        //	rts
        //1:
        //	move.w	w_l_flag(a4),d4
        //	bpl	_ch_mpcm_p_com_exec
        //	lsr.b	#1,d4
        //	bcs	_ch_mpcm_p_keyon_only

        //	btst.b	#5,w_flag(a5)
        //	beq	_ch_mpcm_p_com_exec
        //	rts

        //_ch_mpcm_p_keyon_only:
        //	btst.b	#5,w_flag(a5)
        //	bne	_ch_mpcm_p_com_exec
        //	rts

        //_ch_mpcm_p_com_exec:
        //	add.b	d1,d0
        //	add.w	d0,d0

        //	tst.b	LFO_FLAG(a6)
        //	bmi	1f
        //	move.w	_mpcm_pitch_pattern(pc,d0.w),d0
        //	jsr	_mpcm_pitch_pattern(pc,d0.w)
        //	add.w	d1,w_addkeycode(a5)
        //	rts
        //1:
        //	lea.l	_pitch_extend(pc),a0
        //	move.w	(a0,d0.w),d0
        //	jsr	(a0,d0.w)
        //	add.w	d1,w_addkeycode(a5)
        //	rts

        //_mpcm_pitch_pattern:
        //	.dc.w	0
        //	.dc.w	_com_lfo_saw-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_portament-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_triangle-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_portament-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_triangle-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_triangle-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_oneshot-_mpcm_pitch_pattern
        //	.dc.w	_com_lfo_oneshot-_mpcm_pitch_pattern

        //;─────────────────────────────────────
        //_ch_mpcm_mml_job:
        //	bsr	_track_analyze
        //_ch_mpcm_bend_job:
        //	move.b	w_lfo(a5),d0
        //	bmi	1f
        //	rts
        //1:
        //	btst.b	#1,w_flag2(a5)
        //	bne	_ch_mpcm_porta
        //;	bra	_ch_mpcm_bend

        //;─────────────────────────────────────
        //;	pitch bend
        //;
        //_ch_mpcm_bend:
        //	lea.l	w_p_pattern4(a5),a4
        //	subq.b	#1,w_l_delay_work(a4)
        //	bne	_ch_mpcm_bend_no_job

        //	move.b	w_l_lfo_sp(a4),w_l_delay_work(a4)
        //	move.w	w_keycode3(a5),d2
        //	move.w	w_l_henka(a4),d1
        //	bpl	_ch_mpcm_bend_plus

        //	add.w	d1,w_l_bendwork(a4)
        //	add.w	d1,d2
        //	bpl	_ch_mpcm_bend_common_minus
        //	move.w	#$1FFF,d2
        //_ch_mpcm_bend_common_minus:
        //	cmp.w	w_l_mokuhyou(a4),d2
        //	bcs	_ch_mpcm_bend_end
        //	move.w	d2,w_keycode3(a5)
        //	bra	_mpcm_freq

        //_ch_mpcm_bend_plus:
        //	add.w	d1,w_l_bendwork(a4)
        //	add.w	d1,d2
        //	bpl	_ch_mpcm_bend_common_plus
        //	moveq.l	#0,d2
        //_ch_mpcm_bend_common_plus:
        //	cmp.w	w_l_mokuhyou(a4),d2
        //	bcc	_ch_mpcm_bend_end
        //	move.w	d2,w_keycode3(a5)
        //	bra	_mpcm_freq

        //_ch_mpcm_bend_end:
        //	bclr.b	#7,w_lfo(a5)
        //	clr.w	w_l_bendwork(a4)

        //	moveq.l	#0,d2
        //	move.b	w_key2(a5),d2
        //	move.b	d2,w_key(a5)
        //	lsl.w	#6,d2
        //	add.w	w_detune(a5),d2
        //	bpl	@f
        //	moveq.l	#0,d2
        //@@:
        //	move.w	d2,w_keycode2(a5)
        //	move.w	d2,w_keycode3(a5)
        //	bra	_mpcm_freq

        //_ch_mpcm_bend_no_job:
        //	rts

        //;─────────────────────────────────────
        //;	portament
        //;
        //_ch_mpcm_porta:
        //	lea.l	w_p_pattern4(a5),a4

        //	btst.b	#4,w_flag2(a5)
        //	bne	_ch_mpcm_konami_port

        //	move.w	w_keycode3(a5),d2
        //	move.w	w_l_henka(a4),d1
        //	add.w	d1,w_l_bendwork(a4)
        //	add.w	d1,d2
        //	tst.w	w_l_henka_work(a4)
        //	beq	_ch_mpcm_porta_common
        //	bmi	_ch_mpcm_porta_minus

        //	subq.w	#1,w_l_henka_work(a4)
        //	addq.w	#1,w_l_bendwork(a4)
        //	addq.w	#1,d2
        //	bra	_ch_mpcm_porta_common

        //_ch_mpcm_porta_minus:
        //	addq.w	#1,w_l_henka_work(a4)
        //	addq.w	#1,w_l_bendwork(a4)
        //	subq.w	#1,d2

        //_ch_mpcm_porta_common:
        //	move.w	d2,w_keycode3(a5)
        //	bra	_mpcm_freq

        //_ch_mpcm_konami_port:
        //	moveq.l	#0,d1
        //	moveq.l	#0,d2
        //	move.b	w_l_lfo_sp(a4),d1
        //	beq	_ch_mpcm_konami_porta_end
        //	move.w	w_l_mokuhyou(a4),d0
        //	sub.w	w_keycode(a5),d0
        //	beq	_ch_mpcm_konami_porta_end
        //	bcc	1f
        //	moveq.l	#$FF,d2
        //	neg.w	d0
        //1:
        //	mulu.w	d1,d0
        //	lsr.l	#8,d0
        //	bne	1f
        //	moveq.l	#1,d0
        //1:
        //	tst.b	d2
        //	beq	1f
        //	neg.w	d0
        //1:
        //	move.w	w_keycode(a5),d2
        //	add.w	d0,w_l_bendwork(a4)
        //	add.w	d0,d2
        //	bra	_mpcm_freq

        //_ch_mpcm_konami_porta_end:
        //	bclr.b	#7,w_lfo(a5)
        //	andi.b	#$FD,w_flag2(a5)
        //	rts

        //;─────────────────────────────────────
        //_ch_mpcm_softenv_job:
        //	bsr	_soft_env
        //	bra	_MPCM_F2_softenv

        //;─────────────────────────────────────
        //;	effect execute
        //;
        //_mpcm_effect_tone:
        //	move.w	d0,-(sp)
        //	move.b	(sp)+,w_bank(a5)
        //	move.b	d0,d5
        //	bra	_mpcm_echo_tone_change

        //_mpcm_effect_pan:
        //	move.b	d0,d1
        //	andi.b	#3,d1
        //	move.b	d1,w_pan_ampm(a5)

        //	btst.b	#1,DRV_FLAG(a6)
        //	beq	1f
        //	move.w	#$600,d0
        //	move.b	w_dev(a5),d0
        //	trap	#1
        //1:
        //	rts

        //	.quad

    }
}
