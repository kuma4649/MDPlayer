using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MoonDriver
{
    public class MoonDriver : baseDriver
    {
        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip[] useChip, uint latency)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            throw new NotImplementedException();
        }



        private const UInt16 MOON_VERNUM = 0x0002;

        private const UInt16 MOON_BASE = 0x00C4;
        private const UInt16 MOON_REG1 = MOON_BASE;
        private const UInt16 MOON_DAT1 = MOON_BASE + 1;
        private const UInt16 MOON_REG2 = MOON_BASE + 2;
        private const UInt16 MOON_DAT2 = MOON_BASE + 3;
        private const UInt16 MOON_STAT = MOON_BASE;

        // I/O
        private const UInt16 MOON_WREG = 0x7E;
        private const UInt16 MOON_WDAT = MOON_WREG + 1;

        private const UInt16 RAM_PAGE3 = 0xFE;


        private const UInt16 USE_CH = 24 + 18;
        private const UInt16 FM_BASECH = 24;

        //********************************************
        // MDR file format
        //********************************************

        private const UInt16 MDR_ID = 0x8000;
        private const UInt16 MDR_PACKED = 0x802A;// 1 if packed

        private const UInt16 MDR_DSTPCM = 0x8030;// destination address of PCM
        private const UInt16 MDR_STPCM = 0x8031;// PCM start bank
        private const UInt16 MDR_PCMBANKS = 0x8032;// PCM banks
        private const UInt16 MDR_LASTS = 0x8033;// PCM size of lastbank


        private const UInt16 S_DEVICE_FLAGS = 0x8007;

        private const UInt16 S_TRACK_TABLE = 0x8010;
        private const UInt16 S_TRACK_BANK = S_TRACK_TABLE + 2;
        private const UInt16 S_LOOP_TABLE = S_TRACK_TABLE + 4;
        private const UInt16 S_LOOP_BANK = S_TRACK_TABLE + 6;
        private const UInt16 S_VENV_TABLE = S_TRACK_TABLE + 8;
        private const UInt16 S_VENV_LOOP = S_TRACK_TABLE + 10;
        private const UInt16 S_PENV_TABLE = S_TRACK_TABLE + 12;
        private const UInt16 S_PENV_LOOP = S_TRACK_TABLE + 14;
        private const UInt16 S_NENV_TABLE = S_TRACK_TABLE + 16;
        private const UInt16 S_NENV_LOOP = S_TRACK_TABLE + 18;
        private const UInt16 S_LFO_TABLE = S_TRACK_TABLE + 20;
        private const UInt16 S_INST_TABLE = S_TRACK_TABLE + 22;
        private const UInt16 S_OPL3_TABLE = S_TRACK_TABLE + 24;

        public class Work
        {
            public byte seq_cur_ch = 0x00;
            public byte seq_use_ch = 0x00;
            public byte seq_start_fm = 0x00;
            public byte seq_cur_bank = 0x00;
            public byte seq_opsel = 0x00;
            public byte seq_reg_bd = 0x00;
            public byte seq_jump_flag = 0x00;
            public byte seq_tmp_note = 0x00;
            public byte seq_tmp_ch = 0x00;
            public byte seq_tmp_oct = 0x00;
            public UInt16 seq_tmp_fnum = 0x0000;

            //********************************************
            //Workarea for channels in the driver
            //
            //seq_work:
            public byte seq_ch1_dsel = 0x00;
            public byte seq_ch1_opsel = 0x00;
            public byte seq_ch1_synth = 0x00;
            public byte seq_ch1_efx1 = 0x00;
            public byte seq_ch1_cnt = 0x00;
            public byte seq_ch1_loop = 0x00;
            public byte seq_ch1_bank = 0x00;
            public UInt16 seq_ch1_addr = 0x0000;
            public UInt16 seq_ch1_tadr = 0x0000;
            public UInt16 seq_ch1_tone = 0x0000;
            public byte seq_ch1_key = 0x00;
            public byte seq_ch1_damp = 0x00;
            public byte seq_ch1_lfo = 0x00;
            public byte seq_ch1_lfo_vib = 0x00;
            public byte seq_ch1_ar_d1r = 0x00;
            public byte seq_ch1_dl_d2r = 0x00;
            public byte seq_ch1_rc_rr = 0x00;
            public byte seq_ch1_am = 0x00;
            public byte seq_ch1_note = 0x00;
            public UInt16 seq_ch1_pitch = 0x0000;
            public UInt16 seq_ch1_p_ofs = 0x0000;
            public byte seq_ch1_oct = 0x00;
            public UInt16 seq_ch1_fnum = 0x0000;
            public byte seq_ch1_reverb = 0x00;
            public byte seq_ch1_vol = 0x00;
            public byte seq_ch1_pan = 0x00;
            public byte seq_ch1_detune = 0x00;
            public byte seq_ch1_venv = 0x00;
            public byte seq_ch1_nenv = 0x00;
            public byte seq_ch1_penv = 0x00;
            public UInt16 seq_ch1_nenv_adr = 0x0000;
            public UInt16 seq_ch1_penv_adr = 0x0000;
            public UInt16 seq_ch1_venv_adr = 0x0000;
            //seq_work_end:

            public int IDX_DSEL = 0;//equ(seq_ch1_dsel    - seq_work); Device Select
            public int IDX_OPSEL = 1;//equ(seq_ch1_opsel   - seq_work); Operator Select
            public int IDX_SYNTH = 2;//equ(seq_ch1_synth   - seq_work); FeedBack,Synth and OpMode
            public int IDX_EFX1 = 3;//equ(seq_ch1_efx1    - seq_work); Effect flags
            public int IDX_CNT = 4;//equ(seq_ch1_cnt     - seq_work); Counter
            public int IDX_LOOP = 5;//equ(seq_ch1_loop    - seq_work); Loop
            public int IDX_BANK = 6;//equ(seq_ch1_bank    - seq_work); Which bank
            public int IDX_ADDR = 7;//equ(seq_ch1_addr    - seq_work); Address to data
            public int IDX_TADR = 9;//equ(seq_ch1_tadr    - seq_work); Address of a Tone Table
            public int IDX_TONE = 11;//equ(seq_ch1_tone    - seq_work); Tone number in OPL4
            public int IDX_KEY = 13;//equ(seq_ch1_key     - seq_work); data in Key register
            public int IDX_DAMP = 14;//equ(seq_ch1_damp    - seq_work); Damp switch
            public int IDX_LFO = 15;//equ(seq_ch1_lfo     - seq_work); LFO switch
            public int IDX_LFO_VIB = 16;//equ(seq_ch1_lfo_vib - seq_work); LFO and VIB
            public int IDX_AR_D1R = 17;//equ(seq_ch1_ar_d1r  - seq_work); AR and D1R
            public int IDX_DL_D2R = 18;//equ(seq_ch1_dl_d2r  - seq_work); DL and D2R
            public int IDX_RC_RR = 19;//equ(seq_ch1_rc_rr   - seq_work); RC and RR
            public int IDX_AM = 20;//equ(seq_ch1_am      - seq_work); AM
            public int IDX_NOTE = 21;//equ(seq_ch1_note    - seq_work); Note data
            public int IDX_PITCH = 22;//equ(seq_ch1_pitch   - seq_work); Pitch data
            public int IDX_P_OFS = 24;//equ(seq_ch1_p_ofs   - seq_work); Offset for pitch
            public int IDX_OCT = 26;//equ(seq_ch1_oct     - seq_work); Octave in OPL4
            public int IDX_FNUM = 27;//equ(seq_ch1_fnum    - seq_work); F-number in OPL4
            public int IDX_REVERB = 29;//equ(seq_ch1_reverb  - seq_work); Pseudo reverb
            public int IDX_VOL = 30;//equ(seq_ch1_vol     - seq_work); Volume in OPL4
            public int IDX_PAN = 31;//equ(seq_ch1_pan     - seq_work); Pan in OPL4
            public int IDX_DETUNE = 32;//equ(seq_ch1_detune  - seq_work); Detune
            public int IDX_VENV = 33;//equ(seq_ch1_venv    - seq_work); Volume envelope in data
            public int IDX_NENV = 34;//equ(seq_ch1_nenv    - seq_work); Vote envelope  in data
            public int IDX_PENV = 35;//equ(seq_ch1_penv    - seq_work); Pitch envelope in data
            public int IDX_NENV_ADR = 36;//equ(seq_ch1_nenv_adr - seq_work)
            public int IDX_PENV_ADR = 38;//equ(seq_ch1_penv_adr - seq_work)
            public int IDX_VENV_ADR = 40;//equ(seq_ch1_venv_adr - seq_work)

            public int IDX_VOLOP = 29;//equ(seq_ch1_reverb  - seq_work); Volume Operator in connect
            public int IDX_OLDAT1 = 17;//equ(seq_ch1_ar_d1r  - seq_work); Volume Data for 1stOP

            //;
            //; Note : IDX_SYNTH OxxFFFSS
            //;
            //; O : 4OP mode
            //; F : FeedBack
            //; S : SynthType(bit0 for 1st&2nd bit1 for 3rd&4th)
            //;

            //SEQ_WORKSIZE: equ(seq_work_end - seq_work)


            //ds(SEQ_WORKSIZE* (USE_CH-1))


        }
        private Work[] work = null;

        private void moon_init_all()
        {
            work = new Work[USE_CH];
            moon_init();
        }

        private void moon_init()
        {
            // CONNECTION SEL
            moon_fm2_out(0x0400);

            // set 1 to NEW2, NEW
            moon_fm2_out(0x0503);

            // RHYTHM
            moon_fm1_out(0xbd00);

            // Set WaveTable header
            moon_wave_out(0x0210);

        }

        private void moon_fm1_out(UInt16 de)
        {

        }

        private void moon_fm2_out(UInt16 de)
        {

        }

        private void moon_wave_out(UInt16 de)
        {

        }
    }
}
