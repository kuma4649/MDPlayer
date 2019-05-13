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

            GD3 gd3 = new GD3();

            uint adrTag;
            adrTag = (UInt16)(buf[0x2e] + buf[0x2f] * 0x100);
            if (adrTag != 0)
            {
                adrTag -= 0x8000;
                gd3.TrackName = Common.getNRDString(buf, ref adrTag);
                gd3.TrackNameJ = Common.getNRDString(buf, ref adrTag);
                gd3.GameName = Common.getNRDString(buf, ref adrTag);
                gd3.GameNameJ = Common.getNRDString(buf, ref adrTag);
                gd3.SystemName = Common.getNRDString(buf, ref adrTag);
                gd3.SystemNameJ = Common.getNRDString(buf, ref adrTag);
                gd3.Composer = Common.getNRDString(buf, ref adrTag);//Track author
                gd3.ComposerJ = Common.getNRDString(buf, ref adrTag);//Track author(jp)
                gd3.Version = Common.getNRDString(buf, ref adrTag);//Release date
                gd3.Converted = Common.getNRDString(buf, ref adrTag);//Programmer
                gd3.Notes = Common.getNRDString(buf, ref adrTag);//Notes
            }

            return gd3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
        {

            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            GD3 = getGD3Info(vgmBuf, 0);
            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;

            try
            {
                a = 0;
                for (int i = 0; i < vgmBuf.Length;i++)
                {
                    if (i % 0x4000 == 0)
                    {
                        byte af = a;
                        change_page3();
                        a = af;
                        a += 2;
                    }
                    WriteMemory((UInt16)(0x8000 + (i % 0x4000)), vgmBuf[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Driverの初期化に失敗しました。", ex);
            }

            a = 0;
            change_page3();
            a = ReadMemory(MDR_PACKED);
            if (a == 0)
            {
                if (ExtendFile != null)
                {
                    d = 0x05;
                    e = 0x03;
                    moon_fm2_out();
                    //memory write mode
                    d = 0x02;
                    e = 0x11;
                    moon_wave_out();
                    d = 0x03;
                    e = 0x20;
                    moon_wave_out();
                    d = 0x04;
                    e = 0x00;
                    moon_wave_out();
                    d = 0x05;
                    e = 0x00;
                    moon_wave_out();

                    foreach(byte dat in ExtendFile.Item2)
                    {
                        d = 0x06;
                        e = dat;
                        moon_wave_out();
                    }

                    //normal mode
                    d = 0x02;
                    e = 0x10;
                    moon_wave_out();

                }
            }
            else
            {
                //LoadPackedPCM
                EntryPoints(0x4013);
            }

            //Driverの初期化
            EntryPoints(0x4000);

            //if (model == EnmModel.RealModel)
            //{
                //chipRegister.sendDataYM2151(0, model);
                //chipRegister.setYM2151SyncWait(0, 1);
                //chipRegister.sendDataYM2151(1, model);
                //chipRegister.setYM2151SyncWait(1, 1);
            //}

            return true;
        }

        public override void oneFrameProc()
        {
            //if (model == EnmModel.RealModel)
            //{
            //    Stopped = true;
            //    vgmCurLoop = int.MaxValue;
            //    return;
            //}

            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        public MoonDriver()
        {
            seq_jmptable = new dlgSeqFunc[]
           {
              seq_drumnote    // $e0 : Set drum note
            , seq_drumbit     // $e1 : Set drum bits
            , seq_jump        // $e2 :
        	, seq_fbs         // $e3 : Set FBS
            , seq_tvp         // $e4 : Set TVP
            , seq_ld2ops      // $e5 : Load 2OP Instrument
            , seq_setop       // $e6 : Set opbase
            , seq_nop         // $e7 : Pitch shift
            , seq_nop         // $e8 :
        	, seq_slar        // $e9 : Slar switch
        	, seq_revbsw      // $ea : Reverb switch / VolumeOP
            , seq_damp        // $eb : Damp switch / OPMODE
            , seq_nop         // $ec : LFO freq
            , seq_nop         // $ed : LFO mode
            , seq_bank        // $ee : Bank change
            , seq_lfosw       // $ef : Mode change
            , seq_pan         // $f0 : Set Pan
            , seq_inst        // $f1 : Load Instrument(4OP or OPL4)
            , seq_drum        // $f2 : Set Drum
            , seq_nop         // $f3 :
        	, seq_wait        // $f4 : Wait
            , seq_data_write  // $f5 : Data Write
            , seq_nop         // $f6
            , seq_nenv        // $f7 : Note envelope
            , seq_penv        // $f8 : Pitch envelope
            , seq_skip_1      // $f9
            , seq_detune      // $fa : Detune
            , seq_nop         // $fb : LFO
            , seq_rest        // $fc : Rest
            , seq_volume      // $fd : Volume
            , seq_skip_1      // $fe : Not used
            , seq_loop        // $ff : Loop
            };


        }

        private double ntscStep = 0.0;
        private double ntscCounter = 0.0;
        private bool nextFlg = false;
        public Tuple<string, byte[]> ExtendFile = null;
        private int[] pcmKeyon = new int[24] {
         -1, -1, -1, -1, -1, -1,
         -1, -1, -1, -1, -1, -1,
         -1, -1, -1, -1, -1, -1,
         -1, -1, -1, -1, -1, -1
        };
        private int[] pcmKeyonB = new int[24];
        public int[] GetPCMKeyOn()
        {
            for(int i = 0; i < pcmKeyonB.Length; i++)
            {
                pcmKeyonB[i] = pcmKeyon[i];
                pcmKeyon[i] = -2;
            }
            return pcmKeyonB;
        }



        private void oneFrameMain()
        {
            try
            {
                Counter++;
                vgmFrameCounter++;
                ntscCounter--;
                if (ntscCounter <= 0)
                {
                    EntryPoints(0x4003);
                    ntscStep = Common.SampleRate / 60.0;
                    ntscCounter += ntscStep;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
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

        private const byte RAM_PAGE3 = 0xFE;


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
            public class Ch
            {
                public byte dsel = 0x00;
                public byte opsel = 0x00;
                public byte synth = 0x00;
                public byte efx1 = 0x00;
                public byte cnt = 0x00;
                public byte loop = 0x00;
                public int loopCnt = 0x00;
                public byte bank = 0x00;
                public UInt16 addr = 0x0000;
                public byte stBank = 0x00;
                public UInt16 stAddr = 0x0000;
                public bool endFlg = false;
                public UInt16 tadr = 0x0000;
                public UInt16 tone = 0x0000;
                public byte key = 0x00;
                public byte damp = 0x00;
                public byte lfo = 0x00;
                public byte lfo_vib = 0x00;
                public byte[] ol = new byte[4];
                //public byte ar_d1r = 0x00;
                //public byte dl_d2r = 0x00;
                //public byte rc_rr = 0x00;
                //public byte am = 0x00;
                public byte note = 0x00;
                public UInt16 pitch = 0x0000;
                public UInt16 p_ofs = 0x0000;
                public byte oct = 0x00;
                public UInt16 fnum = 0x0000;
                public byte reverb = 0x00;
                public byte vol = 0x00;
                public byte pan = 0x00;
                public byte detune = 0x00;
                public byte venv = 0x00;
                public byte nenv = 0x00;
                public byte penv = 0x00;
                public UInt16 nenv_adr = 0x0000;
                public UInt16 penv_adr = 0x0000;
                public UInt16 venv_adr = 0x0000;
            }
            //seq_work_end:
            public Ch[] ch;

            //public int IDX_DSEL = 0;//equ(seq_ch1_dsel    - seq_work); Device Select
            //public int IDX_OPSEL = 1;//equ(seq_ch1_opsel   - seq_work); Operator Select
            //public int IDX_SYNTH = 2;//equ(seq_ch1_synth   - seq_work); FeedBack,Synth and OpMode
            //public int IDX_EFX1 = 3;//equ(seq_ch1_efx1    - seq_work); Effect flags
            //public int IDX_CNT = 4;//equ(seq_ch1_cnt     - seq_work); Counter
            //public int IDX_LOOP = 5;//equ(seq_ch1_loop    - seq_work); Loop
            //public int IDX_BANK = 6;//equ(seq_ch1_bank    - seq_work); Which bank
            //public int IDX_ADDR = 7;//equ(seq_ch1_addr    - seq_work); Address to data
            //public int IDX_TADR = 9;//equ(seq_ch1_tadr    - seq_work); Address of a Tone Table
            //public int IDX_TONE = 11;//equ(seq_ch1_tone    - seq_work); Tone number in OPL4
            //public int IDX_KEY = 13;//equ(seq_ch1_key     - seq_work); data in Key register
            //public int IDX_DAMP = 14;//equ(seq_ch1_damp    - seq_work); Damp switch
            //public int IDX_LFO = 15;//equ(seq_ch1_lfo     - seq_work); LFO switch
            //public int IDX_LFO_VIB = 16;//equ(seq_ch1_lfo_vib - seq_work); LFO and VIB
            //public int IDX_AR_D1R = 17;//equ(seq_ch1_ar_d1r  - seq_work); AR and D1R
            //public int IDX_DL_D2R = 18;//equ(seq_ch1_dl_d2r  - seq_work); DL and D2R
            //public int IDX_RC_RR = 19;//equ(seq_ch1_rc_rr   - seq_work); RC and RR
            //public int IDX_AM = 20;//equ(seq_ch1_am      - seq_work); AM
            //public int IDX_NOTE = 21;//equ(seq_ch1_note    - seq_work); Note data
            //public int IDX_PITCH = 22;//equ(seq_ch1_pitch   - seq_work); Pitch data
            //public int IDX_P_OFS = 24;//equ(seq_ch1_p_ofs   - seq_work); Offset for pitch
            //public int IDX_OCT = 26;//equ(seq_ch1_oct     - seq_work); Octave in OPL4
            //public int IDX_FNUM = 27;//equ(seq_ch1_fnum    - seq_work); F-number in OPL4
            //public int IDX_REVERB = 29;//equ(seq_ch1_reverb  - seq_work); Pseudo reverb
            //public int IDX_VOL = 30;//equ(seq_ch1_vol     - seq_work); Volume in OPL4
            //public int IDX_PAN = 31;//equ(seq_ch1_pan     - seq_work); Pan in OPL4
            //public int IDX_DETUNE = 32;//equ(seq_ch1_detune  - seq_work); Detune
            //public int IDX_VENV = 33;//equ(seq_ch1_venv    - seq_work); Volume envelope in data
            //public int IDX_NENV = 34;//equ(seq_ch1_nenv    - seq_work); Vote envelope  in data
            //public int IDX_PENV = 35;//equ(seq_ch1_penv    - seq_work); Pitch envelope in data
            //public int IDX_NENV_ADR = 36;//equ(seq_ch1_nenv_adr - seq_work)
            //public int IDX_PENV_ADR = 38;//equ(seq_ch1_penv_adr - seq_work)
            //public int IDX_VENV_ADR = 40;//equ(seq_ch1_venv_adr - seq_work)

            //public int IDX_VOLOP = 29;//equ(seq_ch1_reverb  - seq_work); Volume Operator in connect
            //public int IDX_OLDAT1 = 17;//equ(seq_ch1_ar_d1r  - seq_work); Volume Data for 1stOP

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
        private Work work = null;

        private UInt16[] fm_fnumtbl = new UInt16[] {
            345 // C 523.300000
	        ,365 // C+ 554.400000
	        ,387 // D 587.300000
	        ,410 // D+ 622.300000
	        ,435 // E 659.300000
	        ,460 // F 698.500000
	        ,488 // F+ 740.000000
	        ,517 // G 784.000000
	        ,547 // G+ 830.600000
	        ,580 // A 880.000000
	        ,614 // A+ 932.300000
	        ,651 // B 987.800000
	        ,690 // C 1046.500000
        };

        private byte[] fm_testtone = new byte[] {
             0x00 // FBS
            ,0x00 // FBS2
            ,0x00 // BD
            
            ,0x01 // TREMOLO VIB SUS KSR MUL
            ,0x00 // KSL OL
            ,0x11 // AR DR
            ,0x13 // SL RR
            ,0x01 // WF
            
            ,0x04 //
            ,0x00 //
            ,0x11 //
            ,0x18 //
            ,0x00 // WF
            
            ,0x01
            ,0x3f
            ,0x55
            ,0x55
            ,0x00

            ,0x01
            ,0x3f
            ,0x55
            ,0x55
            ,0x00
        };

        private byte[] fm_op2reg_tbl = new byte[] {
            0x00 // 0
            ,0x01 //
            ,0x02 // 2
            ,0x03 //
            ,0x04 // 4
            ,0x05 //
            ,0x08 // 6
            ,0x09 //
            ,0x0A // 8
            ,0x0B //
            ,0x0C // 10
            ,0x0D //
            ,0x10 // 12
            ,0x11 //
            ,0x12 // 14
            ,0x13 //
            ,0x14 // 16
            ,0x15 // 17
        };

        private byte[] fm_opbtbl = new byte[] {
            0x00 // CH0
            ,0x01 // CH1
            ,0x02 // CH2
            ,0x06 // CH3
            ,0x07 // CH4
            ,0x08 // CH5
            ,0x0c // CH6
            ,0x0d // CH7
            ,0x0e // CH8
            ,0x12 // CH9
            ,0x13 // CH10
            ,0x14 // CH11
            ,0x18 // CH12
            ,0x19 // CH13
            ,0x1a // CH14
            ,0x1e // CH15
            ,0x1f // CH16
            ,0x20 // CH17
        };

        //********************************************
        // BSMCH
        private UInt16[] fm_drum_fnum = new UInt16[] {
    0x0120 // B
    ,0x0150 // S
    ,0x01c0 // M
    ,0x01c0 // C
    ,0x0150 // H
        };

        private byte[] fm_drum_fnum_map = new byte[] {
    0x06 // B
    ,0x07 // S
    ,0x08 // M
    ,0x08 // C
    ,0x07 // H
        };

        private byte[] fm_drum_oct = new byte[] {
    0x02 // B
    ,0x02 // S
    ,0x00 // M
    ,0x00 // C
    ,0x02 // H
        };

        private UInt16[] freq_table = new UInt16[]
        {
            0x0000,0x0000,0x0000,0x0001,0x0001,0x0002,0x0002,0x0003 // $00 
	        ,0x0003,0x0004,0x0004,0x0005,0x0005,0x0006,0x0006,0x0006 // $08 
	        ,0x0007,0x0007,0x0008,0x0008,0x0009,0x0009,0x000A,0x000A // $10 
	        ,0x000B,0x000B,0x000C,0x000C,0x000D,0x000D,0x000D,0x000E // $18 
	        ,0x000E,0x000F,0x000F,0x0010,0x0010,0x0011,0x0011,0x0012 // $20 
	        ,0x0012,0x0013,0x0013,0x0014,0x0014,0x0015,0x0015,0x0015 // $28 
	        ,0x0016,0x0016,0x0017,0x0017,0x0018,0x0018,0x0019,0x0019 // $30 
	        ,0x001A,0x001A,0x001B,0x001B,0x001C,0x001C,0x001D,0x001D // $38 
	        ,0x001E,0x001E,0x001E,0x001F,0x001F,0x0020,0x0020,0x0021 // $40 
	        ,0x0021,0x0022,0x0022,0x0023,0x0023,0x0024,0x0024,0x0025 // $48 
	        ,0x0025,0x0026,0x0026,0x0027,0x0027,0x0028,0x0028,0x0029 // $50 
	        ,0x0029,0x0029,0x002A,0x002A,0x002B,0x002B,0x002C,0x002C // $58 
	        ,0x002D,0x002D,0x002E,0x002E,0x002F,0x002F,0x0030,0x0030 // $60 
	        ,0x0031,0x0031,0x0032,0x0032,0x0033,0x0033,0x0034,0x0034 // $68 
	        ,0x0035,0x0035,0x0036,0x0036,0x0037,0x0037,0x0038,0x0038 // $70 
	        ,0x0038,0x0039,0x0039,0x003A,0x003A,0x003B,0x003B,0x003C // $78 
	        ,0x003C,0x003D,0x003D,0x003E,0x003E,0x003F,0x003F,0x0040 // $80 
	        ,0x0040,0x0041,0x0041,0x0042,0x0042,0x0043,0x0043,0x0044 // $88 
	        ,0x0044,0x0045,0x0045,0x0046,0x0046,0x0047,0x0047,0x0048 // $90 
	        ,0x0048,0x0049,0x0049,0x004A,0x004A,0x004B,0x004B,0x004C // $98 
	        ,0x004C,0x004D,0x004D,0x004E,0x004E,0x004F,0x004F,0x0050 // $a0
            ,0x0050,0x0051,0x0051,0x0052,0x0052,0x0053,0x0053,0x0054 // $a8 
	        ,0x0054,0x0055,0x0055,0x0056,0x0056,0x0057,0x0057,0x0058 // $b0 
	        ,0x0058,0x0059,0x0059,0x005A,0x005A,0x005B,0x005B,0x005C // $b8 
	        ,0x005C,0x005D,0x005D,0x005E,0x005E,0x005F,0x005F,0x0060 // $c0 
	        ,0x0060,0x0061,0x0061,0x0062,0x0062,0x0063,0x0063,0x0064 // $c8 
	        ,0x0064,0x0065,0x0065,0x0066,0x0066,0x0067,0x0067,0x0068 // $d0 
	        ,0x0068,0x0069,0x0069,0x006A,0x006A,0x006B,0x006B,0x006C // $d8 
	        ,0x006C,0x006D,0x006D,0x006E,0x006E,0x006F,0x006F,0x0070 // $e0 
	        ,0x0071,0x0071,0x0072,0x0072,0x0073,0x0073,0x0074,0x0074 // $e8 
	        ,0x0075,0x0075,0x0076,0x0076,0x0077,0x0077,0x0078,0x0078 // $f0 
	        ,0x0079,0x0079,0x007A,0x007A,0x007B,0x007B,0x007C,0x007C // $f8 
	        ,0x007D,0x007D,0x007E,0x007E,0x007F,0x007F,0x0080,0x0081 // $100 
	        ,0x0081,0x0082,0x0082,0x0083,0x0083,0x0084,0x0084,0x0085 // $108 
	        ,0x0085,0x0086,0x0086,0x0087,0x0087,0x0088,0x0088,0x0089 // $110 
	        ,0x0089,0x008A,0x008A,0x008B,0x008C,0x008C,0x008D,0x008D // $118 
	        ,0x008E,0x008E,0x008F,0x008F,0x0090,0x0090,0x0091,0x0091 // $120 
	        ,0x0092,0x0092,0x0093,0x0093,0x0094,0x0094,0x0095,0x0096 // $128 
	        ,0x0096,0x0097,0x0097,0x0098,0x0098,0x0099,0x0099,0x009A // $130 
	        ,0x009A,0x009B,0x009B,0x009C,0x009C,0x009D,0x009E,0x009E // $138 
	        ,0x009F,0x009F,0x00A0,0x00A0,0x00A1,0x00A1,0x00A2,0x00A2 // $140 
	        ,0x00A3,0x00A3,0x00A4,0x00A4,0x00A5,0x00A6,0x00A6,0x00A7 // $148 
	        ,0x00A7,0x00A8,0x00A8,0x00A9,0x00A9,0x00AA,0x00AA,0x00AB // $150 
	        ,0x00AB,0x00AC,0x00AD,0x00AD,0x00AE,0x00AE,0x00AF,0x00AF // $158 
	        ,0x00B0,0x00B0,0x00B1,0x00B1,0x00B2,0x00B3,0x00B3,0x00B4 // $160 
	        ,0x00B4,0x00B5,0x00B5,0x00B6,0x00B6,0x00B7,0x00B7,0x00B8 // $168 
	        ,0x00B8,0x00B9,0x00BA,0x00BA,0x00BB,0x00BB,0x00BC,0x00BC // $170 
	        ,0x00BD,0x00BD,0x00BE,0x00BF,0x00BF,0x00C0,0x00C0,0x00C1 // $178 
	        ,0x00C1,0x00C2,0x00C2,0x00C3,0x00C3,0x00C4,0x00C5,0x00C5 // $180 
	        ,0x00C6,0x00C6,0x00C7,0x00C7,0x00C8,0x00C8,0x00C9,0x00CA // $188 
	        ,0x00CA,0x00CB,0x00CB,0x00CC,0x00CC,0x00CD,0x00CD,0x00CE // $190 
	        ,0x00CF,0x00CF,0x00D0,0x00D0,0x00D1,0x00D1,0x00D2,0x00D2 // $198 
	        ,0x00D3,0x00D4,0x00D4,0x00D5,0x00D5,0x00D6,0x00D6,0x00D7 // $1a0 
	        ,0x00D7,0x00D8,0x00D9,0x00D9,0x00DA,0x00DA,0x00DB,0x00DB // $1a8 
	        ,0x00DC,0x00DC,0x00DD,0x00DE,0x00DE,0x00DF,0x00DF,0x00E0 // $1b0 
	        ,0x00E0,0x00E1,0x00E2,0x00E2,0x00E3,0x00E3,0x00E4,0x00E4 // $1b8 
	        ,0x00E5,0x00E5,0x00E6,0x00E7,0x00E7,0x00E8,0x00E8,0x00E9 // $1c0 
	        ,0x00E9,0x00EA,0x00EB,0x00EB,0x00EC,0x00EC,0x00ED,0x00ED // $1c8 
	        ,0x00EE,0x00EF,0x00EF,0x00F0,0x00F0,0x00F1,0x00F1,0x00F2 // $1d0 
	        ,0x00F3,0x00F3,0x00F4,0x00F4,0x00F5,0x00F5,0x00F6,0x00F7 // $1d8 
	        ,0x00F7,0x00F8,0x00F8,0x00F9,0x00F9,0x00FA,0x00FB,0x00FB // $1e0 
	        ,0x00FC,0x00FC,0x00FD,0x00FD,0x00FE,0x00FF,0x00FF,0x0100 // $1e8 
	        ,0x0100,0x0101,0x0102,0x0102,0x0103,0x0103,0x0104,0x0104 // $1f0 
	        ,0x0105,0x0106,0x0106,0x0107,0x0107,0x0108,0x0108,0x0109 // $1f8 
	        ,0x010A,0x010A,0x010B,0x010B,0x010C,0x010D,0x010D,0x010E // $200 
	        ,0x010E,0x010F,0x010F,0x0110,0x0111,0x0111,0x0112,0x0112 // $208 
	        ,0x0113,0x0114,0x0114,0x0115,0x0115,0x0116,0x0117,0x0117 // $210 
	        ,0x0118,0x0118,0x0119,0x0119,0x011A,0x011B,0x011B,0x011C // $218 
	        ,0x011C,0x011D,0x011E,0x011E,0x011F,0x011F,0x0120,0x0121 // $220 
	        ,0x0121,0x0122,0x0122,0x0123,0x0124,0x0124,0x0125,0x0125 // $228 
	        ,0x0126,0x0127,0x0127,0x0128,0x0128,0x0129,0x0129,0x012A // $230 
	        ,0x012B,0x012B,0x012C,0x012C,0x012D,0x012E,0x012E,0x012F // $238 
	        ,0x012F,0x0130,0x0131,0x0131,0x0132,0x0132,0x0133,0x0134 // $240 
	        ,0x0134,0x0135,0x0135,0x0136,0x0137,0x0137,0x0138,0x0138 // $248 
	        ,0x0139,0x013A,0x013A,0x013B,0x013C,0x013C,0x013D,0x013D // $250 
	        ,0x013E,0x013F,0x013F,0x0140,0x0140,0x0141,0x0142,0x0142 // $258 
	        ,0x0143,0x0143,0x0144,0x0145,0x0145,0x0146,0x0146,0x0147 // $260 
	        ,0x0148,0x0148,0x0149,0x0149,0x014A,0x014B,0x014B,0x014C // $268 
	        ,0x014D,0x014D,0x014E,0x014E,0x014F,0x0150,0x0150,0x0151 // $270 
	        ,0x0151,0x0152,0x0153,0x0153,0x0154,0x0155,0x0155,0x0156 // $278 
	        ,0x0156,0x0157,0x0158,0x0158,0x0159,0x0159,0x015A,0x015B // $280 
	        ,0x015B,0x015C,0x015D,0x015D,0x015E,0x015E,0x015F,0x0160 // $288 
	        ,0x0160,0x0161,0x0162,0x0162,0x0163,0x0163,0x0164,0x0165 // $290 
	        ,0x0165,0x0166,0x0167,0x0167,0x0168,0x0168,0x0169,0x016A // $298 
	        ,0x016A,0x016B,0x016C,0x016C,0x016D,0x016D,0x016E,0x016F // $2a0 
	        ,0x016F,0x0170,0x0171,0x0171,0x0172,0x0172,0x0173,0x0174 // $2a8 
	        ,0x0174,0x0175,0x0176,0x0176,0x0177,0x0177,0x0178,0x0179 // $2b0 
	        ,0x0179,0x017A,0x017B,0x017B,0x017C,0x017D,0x017D,0x017E // $2b8 
	        ,0x017E,0x017F,0x0180,0x0180,0x0181,0x0182,0x0182,0x0183 // $2c0 
	        ,0x0184,0x0184,0x0185,0x0185,0x0186,0x0187,0x0187,0x0188 // $2c8 
	        ,0x0189,0x0189,0x018A,0x018B,0x018B,0x018C,0x018C,0x018D // $2d0 
	        ,0x018E,0x018E,0x018F,0x0190,0x0190,0x0191,0x0192,0x0192 // $2d8 
	        ,0x0193,0x0194,0x0194,0x0195,0x0195,0x0196,0x0197,0x0197 // $2e0 
	        ,0x0198,0x0199,0x0199,0x019A,0x019B,0x019B,0x019C,0x019D // $2e8 
	        ,0x019D,0x019E,0x019F,0x019F,0x01A0,0x01A0,0x01A1,0x01A2 // $2f0 
	        ,0x01A2,0x01A3,0x01A4,0x01A4,0x01A5,0x01A6,0x01A6,0x01A7 // $2f8 
	        ,0x01A8,0x01A8,0x01A9,0x01AA,0x01AA,0x01AB,0x01AC,0x01AC // $300 
	        ,0x01AD,0x01AE,0x01AE,0x01AF,0x01B0,0x01B0,0x01B1,0x01B1 // $308 
	        ,0x01B2,0x01B3,0x01B3,0x01B4,0x01B5,0x01B5,0x01B6,0x01B7 // $310 
	        ,0x01B7,0x01B8,0x01B9,0x01B9,0x01BA,0x01BB,0x01BB,0x01BC // $318 
	        ,0x01BD,0x01BD,0x01BE,0x01BF,0x01BF,0x01C0,0x01C1,0x01C1 // $320 
	        ,0x01C2,0x01C3,0x01C3,0x01C4,0x01C5,0x01C5,0x01C6,0x01C7 // $328 
	        ,0x01C7,0x01C8,0x01C9,0x01C9,0x01CA,0x01CB,0x01CB,0x01CC // $330 
	        ,0x01CD,0x01CD,0x01CE,0x01CF,0x01CF,0x01D0,0x01D1,0x01D1 // $338 
	        ,0x01D2,0x01D3,0x01D3,0x01D4,0x01D5,0x01D5,0x01D6,0x01D7 // $340 
	        ,0x01D7,0x01D8,0x01D9,0x01DA,0x01DA,0x01DB,0x01DC,0x01DC // $348 
	        ,0x01DD,0x01DE,0x01DE,0x01DF,0x01E0,0x01E0,0x01E1,0x01E2 // $350 
	        ,0x01E2,0x01E3,0x01E4,0x01E4,0x01E5,0x01E6,0x01E6,0x01E7 // $358 
	        ,0x01E8,0x01E8,0x01E9,0x01EA,0x01EB,0x01EB,0x01EC,0x01ED // $360 
	        ,0x01ED,0x01EE,0x01EF,0x01EF,0x01F0,0x01F1,0x01F1,0x01F2 // $368 
	        ,0x01F3,0x01F3,0x01F4,0x01F5,0x01F5,0x01F6,0x01F7,0x01F8 // $370 
	        ,0x01F8,0x01F9,0x01FA,0x01FA,0x01FB,0x01FC,0x01FC,0x01FD // $378 
	        ,0x01FE,0x01FE,0x01FF,0x0200,0x0201,0x0201,0x0202,0x0203 // $380 
	        ,0x0203,0x0204,0x0205,0x0205,0x0206,0x0207,0x0207,0x0208 // $388 
	        ,0x0209,0x020A,0x020A,0x020B,0x020C,0x020C,0x020D,0x020E // $390 
	        ,0x020E,0x020F,0x0210,0x0211,0x0211,0x0212,0x0213,0x0213 // $398 
	        ,0x0214,0x0215,0x0215,0x0216,0x0217,0x0218,0x0218,0x0219 // $3a0 
	        ,0x021A,0x021A,0x021B,0x021C,0x021D,0x021D,0x021E,0x021F // $3a8 
	        ,0x021F,0x0220,0x0221,0x0221,0x0222,0x0223,0x0224,0x0224 // $3b0 
	        ,0x0225,0x0226,0x0226,0x0227,0x0228,0x0229,0x0229,0x022A // $3b8 
	        ,0x022B,0x022B,0x022C,0x022D,0x022E,0x022E,0x022F,0x0230 // $3c0 
	        ,0x0230,0x0231,0x0232,0x0233,0x0233,0x0234,0x0235,0x0235 // $3c8 
	        ,0x0236,0x0237,0x0238,0x0238,0x0239,0x023A,0x023A,0x023B // $3d0 
	        ,0x023C,0x023D,0x023D,0x023E,0x023F,0x0240,0x0240,0x0241 // $3d8 
	        ,0x0242,0x0242,0x0243,0x0244,0x0245,0x0245,0x0246,0x0247 // $3e0 
	        ,0x0247,0x0248,0x0249,0x024A,0x024A,0x024B,0x024C,0x024D // $3e8 
	        ,0x024D,0x024E,0x024F,0x024F,0x0250,0x0251,0x0252,0x0252 // $3f0 
	        ,0x0253,0x0254,0x0255,0x0255,0x0256,0x0257,0x0258,0x0258 // $3f8 
	        ,0x0259,0x025A,0x025A,0x025B,0x025C,0x025D,0x025D,0x025E // $400 
	        ,0x025F,0x0260,0x0260,0x0261,0x0262,0x0263,0x0263,0x0264 // $408 
	        ,0x0265,0x0266,0x0266,0x0267,0x0268,0x0268,0x0269,0x026A // $410 
	        ,0x026B,0x026B,0x026C,0x026D,0x026E,0x026E,0x026F,0x0270 // $418 
	        ,0x0271,0x0271,0x0272,0x0273,0x0274,0x0274,0x0275,0x0276 // $420 
	        ,0x0277,0x0277,0x0278,0x0279,0x027A,0x027A,0x027B,0x027C // $428 
	        ,0x027D,0x027D,0x027E,0x027F,0x0280,0x0280,0x0281,0x0282 // $430 
	        ,0x0283,0x0283,0x0284,0x0285,0x0286,0x0286,0x0287,0x0288 // $438 
	        ,0x0289,0x0289,0x028A,0x028B,0x028C,0x028C,0x028D,0x028E // $440 
	        ,0x028F,0x028F,0x0290,0x0291,0x0292,0x0292,0x0293,0x0294 // $448 
	        ,0x0295,0x0296,0x0296,0x0297,0x0298,0x0299,0x0299,0x029A // $450 
	        ,0x029B,0x029C,0x029C,0x029D,0x029E,0x029F,0x029F,0x02A0 // $458 
	        ,0x02A1,0x02A2,0x02A2,0x02A3,0x02A4,0x02A5,0x02A6,0x02A6 // $460 
	        ,0x02A7,0x02A8,0x02A9,0x02A9,0x02AA,0x02AB,0x02AC,0x02AC // $468 
	        ,0x02AD,0x02AE,0x02AF,0x02B0,0x02B0,0x02B1,0x02B2,0x02B3 // $470 
	        ,0x02B3,0x02B4,0x02B5,0x02B6,0x02B7,0x02B7,0x02B8,0x02B9 // $478 
	        ,0x02BA,0x02BA,0x02BB,0x02BC,0x02BD,0x02BE,0x02BE,0x02BF // $480 
	        ,0x02C0,0x02C1,0x02C1,0x02C2,0x02C3,0x02C4,0x02C5,0x02C5 // $488 
	        ,0x02C6,0x02C7,0x02C8,0x02C8,0x02C9,0x02CA,0x02CB,0x02CC // $490 
	        ,0x02CC,0x02CD,0x02CE,0x02CF,0x02D0,0x02D0,0x02D1,0x02D2 // $498 
	        ,0x02D3,0x02D3,0x02D4,0x02D5,0x02D6,0x02D7,0x02D7,0x02D8 // $4a0 
	        ,0x02D9,0x02DA,0x02DB,0x02DB,0x02DC,0x02DD,0x02DE,0x02DF // $4a8 
	        ,0x02DF,0x02E0,0x02E1,0x02E2,0x02E3,0x02E3,0x02E4,0x02E5 // $4b0 
	        ,0x02E6,0x02E7,0x02E7,0x02E8,0x02E9,0x02EA,0x02EB,0x02EB // $4b8 
	        ,0x02EC,0x02ED,0x02EE,0x02EF,0x02EF,0x02F0,0x02F1,0x02F2 // $4c0 
	        ,0x02F3,0x02F3,0x02F4,0x02F5,0x02F6,0x02F7,0x02F7,0x02F8 // $4c8 
	        ,0x02F9,0x02FA,0x02FB,0x02FB,0x02FC,0x02FD,0x02FE,0x02FF // $4d0 
	        ,0x02FF,0x0300,0x0301,0x0302,0x0303,0x0303,0x0304,0x0305 // $4d8 
	        ,0x0306,0x0307,0x0308,0x0308,0x0309,0x030A,0x030B,0x030C // $4e0 
	        ,0x030C,0x030D,0x030E,0x030F,0x0310,0x0310,0x0311,0x0312 // $4e8 
	        ,0x0313,0x0314,0x0315,0x0315,0x0316,0x0317,0x0318,0x0319 // $4f0 
	        ,0x0319,0x031A,0x031B,0x031C,0x031D,0x031E,0x031E,0x031F // $4f8 
	        ,0x0320,0x0321,0x0322,0x0323,0x0323,0x0324,0x0325,0x0326 // $500 
	        ,0x0327,0x0327,0x0328,0x0329,0x032A,0x032B,0x032C,0x032C // $508 
	        ,0x032D,0x032E,0x032F,0x0330,0x0331,0x0331,0x0332,0x0333 // $510 
	        ,0x0334,0x0335,0x0336,0x0336,0x0337,0x0338,0x0339,0x033A // $518 
	        ,0x033B,0x033B,0x033C,0x033D,0x033E,0x033F,0x0340,0x0340 // $520 
	        ,0x0341,0x0342,0x0343,0x0344,0x0345,0x0345,0x0346,0x0347 // $528 
	        ,0x0348,0x0349,0x034A,0x034B,0x034B,0x034C,0x034D,0x034E // $530 
	        ,0x034F,0x0350,0x0350,0x0351,0x0352,0x0353,0x0354,0x0355 // $538 
	        ,0x0356,0x0356,0x0357,0x0358,0x0359,0x035A,0x035B,0x035B // $540 
	        ,0x035C,0x035D,0x035E,0x035F,0x0360,0x0361,0x0361,0x0362 // $548 
	        ,0x0363,0x0364,0x0365,0x0366,0x0367,0x0367,0x0368,0x0369 // $550 
	        ,0x036A,0x036B,0x036C,0x036D,0x036D,0x036E,0x036F,0x0370 // $558 
	        ,0x0371,0x0372,0x0373,0x0373,0x0374,0x0375,0x0376,0x0377 // $560 
	        ,0x0378,0x0379,0x0379,0x037A,0x037B,0x037C,0x037D,0x037E // $568 
	        ,0x037F,0x0380,0x0380,0x0381,0x0382,0x0383,0x0384,0x0385 // $570 
	        ,0x0386,0x0386,0x0387,0x0388,0x0389,0x038A,0x038B,0x038C // $578 
	        ,0x038D,0x038D,0x038E,0x038F,0x0390,0x0391,0x0392,0x0393 // $580 
	        ,0x0394,0x0394,0x0395,0x0396,0x0397,0x0398,0x0399,0x039A // $588 
	        ,0x039B,0x039B,0x039C,0x039D,0x039E,0x039F,0x03A0,0x03A1 // $590 
	        ,0x03A2,0x03A2,0x03A3,0x03A4,0x03A5,0x03A6,0x03A7,0x03A8 // $598 
	        ,0x03A9,0x03AA,0x03AA,0x03AB,0x03AC,0x03AD,0x03AE,0x03AF // $5a0 
	        ,0x03B0,0x03B1,0x03B2,0x03B2,0x03B3,0x03B4,0x03B5,0x03B6 // $5a8 
	        ,0x03B7,0x03B8,0x03B9,0x03BA,0x03BA,0x03BB,0x03BC,0x03BD // $5b0 
	        ,0x03BE,0x03BF,0x03C0,0x03C1,0x03C2,0x03C3,0x03C3,0x03C4 // $5b8 
	        ,0x03C5,0x03C6,0x03C7,0x03C8,0x03C9,0x03CA,0x03CB,0x03CB // $5c0 
	        ,0x03CC,0x03CD,0x03CE,0x03CF,0x03D0,0x03D1,0x03D2,0x03D3 // $5c8 
	        ,0x03D4,0x03D5,0x03D5,0x03D6,0x03D7,0x03D8,0x03D9,0x03DA // $5d0 
	        ,0x03DB,0x03DC,0x03DD,0x03DE,0x03DE,0x03DF,0x03E0,0x03E1 // $5d8 
	        ,0x03E2,0x03E3,0x03E4,0x03E5,0x03E6,0x03E7,0x03E8,0x03E9 // $5e0 
	        ,0x03E9,0x03EA,0x03EB,0x03EC,0x03ED,0x03EE,0x03EF,0x03F0 // $5e8 
	        ,0x03F1,0x03F2,0x03F3,0x03F4,0x03F4,0x03F5,0x03F6,0x03F7 // $5f0 
	        ,0x03F8,0x03F9,0x03FA,0x03FB,0x03FC,0x03FD,0x03FE,0x03FF // $5f8 
        };

        //********************************************
        // Entry points
        //********************************************
        public void EntryPoints(UInt16 adr)
        {
            switch (adr)
            {
                //	; $4000 Initialize
                case 0x4000:
                    moon_init_all();
                    break;

                //	; $4003 Execute 1frame(1/60)
                case 0x4003:
                    moon_proc_tracks();
                    break;

                //    ; $4006 All key-off
                case 0x4006:
                    moon_seq_all_keyoff();
                    break;

                //	; $4009 Set H.TIMI for timing
                case 0x4009:
                    //    ret
                    //    ret
                    //    ret
                    break;

                //    ; $400C Restore H.TIMI
                case 0x400c:
                    //    ret
                    //    ret
                    //    ret
                    break;

                //    ; $400F MOONDRIVER version number
                case 0x400f:
                    //    dw  MOON_VERNUM
                    break;
                //	; $4011 MOONDRIVER version string
                case 0x4011:
                    //    dw  str_moondrv
                    break;

                //	; $4013 LoadPCM
                case 0x4013:
                    moon_load_pcm();
                    break;
            }
        }

        //    org	$4020

        //str_moondrv:
        //private string str_moondrv = "MOONDRIVER "
        //+ "VER 160305"
        //+ "\0d\0a$";

        //********************************************
        // work for debug
#if MOON_HOOT
        private UInt16 MDB_BASE=0x2F0;
#else
        private byte[] MDB_BASE = new byte[0x008];
#endif

        //********************************************
        // Initialises all driver things
        //
        private void moon_init_all()
        {
            work = new Work();
            work.ch = new Work.Ch[USE_CH];
            for (int i = 0; i < USE_CH; i++) work.ch[i] = new Work.Ch();
            moon_init();
            moon_seq_init();
        }

        //********************************************
        // moon_init
        // initialize MoonSound
        //
        private void moon_init()
        {
            // CONNECTION SEL
            d = 0x4;
            e = 0;
            moon_fm2_out();

            // set 1 to NEW2, NEW
            d = 0x05;
            e = 0x03;
            moon_fm2_out();

            // RHYTHM
            d = 0xbd;
            e = 0;
            moon_fm1_out();

            // Set WaveTable header
            d = 0x02;
            e = 0x10;
            moon_wave_out();

        }


        //////////////////////////////////////
        // Memory access routines
        //////////////////////////////////////

        //********************************************
        // set page3 to the bank of current channel
        // dest : AF
        private void set_page3_ch()
        {

            a = work.ch[ix].bank;
            change_page3();
        }

        //********************************************
        // changes page3
        // in   : A = page
        // dest : AF
        private void change_page3()
        {
            //Console.WriteLine("ChangePage3:{0}", a);
            a >>= 1;//srl
            a += 0x04;// The system uses 4pages for initial work area
            outport(RAM_PAGE3, a);
        }

        //********************************************
        // get_table
        // in   : A = index , HL = address
        // out  : HL = (HL + (A* 2) )
        // dest : AF,DE
        private void get_table()
        {
            e = a;
            d = 0;

            get_table_hl_2de();
        }

        //********************************************
        // get_hl_table
        // in   : HL = address
        // out  : HL = (HL + (cur_ch* 2) )
        // dest : AF,DE
        private void get_hl_table()
        {
            a = work.seq_cur_ch;

            e = a;
            d = 0x00;

            get_table_hl_2de();
        }

        private void get_table_hl_2de()
        {
            a = ReadMemory(hl);
            hl++;
            hl = (UInt16)(ReadMemory(hl) * 0x100 + a);

            hl += e;
            hl += e;

            a = ReadMemory(hl);
            hl++;
            hl = (UInt16)(ReadMemory(hl) * 0x100 + a);
        }

        //********************************************
        // get_a_table
        // in   : HL = address
        // out  : A = (HL + (cur_ch* 2) )
        // dest : HL,DE
        private void get_a_table()
        {
            a = work.seq_cur_ch;
            e = a;
            d = 0x00;
            a = ReadMemory(hl);
            hl++;

            hl = (UInt16)(ReadMemory(hl) * 0x100 + a);
            hl += e;

            a = ReadMemory(hl);
        }

        //********************************************
        // moon_seq_init
        // initializes all channel's work
        // dest : ALL
        private void moon_seq_init()
        {
            a = 0;
            work.seq_use_ch = a;
            work.seq_cur_ch = a;
            change_page3(); //Page to Top


            a = ReadMemory(S_DEVICE_FLAGS);
            if (a == 0) a = 1;//OPL4 by default
            b = a;
            iy = 0;// fm_opbtbl;
            ix = 0;// seq_work;

            d = 0x00;
            e = 0x18;//24channels; OPL4
            bool cry = (b & 1)!=0;
            b >>= 1;
            b |= (byte)(cry ? 0x80 : 0);
            if (cry)
            {
                seq_init_chan();
            }

            d = 0x01;
            e = 0x12;//18channels
            cry = (b & 1) != 0;
            b >>= 1;
            b |= (byte)(cry ? 0x80 : 0);
            if (cry)
            {
                seq_init_chan();
            }

            ix = 0;// seq_work;
        }

        //********************************************
        // seq_init_chan
        // in   : D = device, E = channels
        // dest : AF,E
        private void seq_init_chan()
        {
            if (d != 0)
            {
                work.seq_start_fm = work.seq_use_ch;
            }
            work.seq_use_ch += e;

            //seq_init_chan_lp:
            do
            {
                work.ch[ix].cnt = 0;
                work.ch[ix].dsel = d;
                byte db = d;
                byte eb = e;
                work.ch[ix].venv = 0xff;
                work.ch[ix].penv = 0xff;
                work.ch[ix].nenv = 0xff;
                work.ch[ix].detune = 0xff;

                if (work.ch[ix].dsel != 0)
                {
                    //init_fmtone:
                    work.ch[ix].tadr = 0;// fm_testtone;
                    work.ch[ix].pan = 0x30;
                    work.ch[ix].reverb = 0x02;//IDX_VOLOP
                    work.ch[ix].vol = 0x3f;
                    work.ch[ix].opsel = fm_opbtbl[iy];
                    iy++;
                }
                else
                {
                    //init_op4tone:
                    work.ch[ix].tadr = 0;// piano_tone;
                    work.ch[ix].pan = 0x00;
                }

                //init_tone_fin:

                hl = S_TRACK_TABLE;
                get_hl_table();
                work.ch[ix].addr = hl;

                hl = S_TRACK_BANK;
                get_a_table();
                work.ch[ix].bank = a;

                work.ch[ix].stBank = work.ch[ix].bank;
                work.ch[ix].stAddr = work.ch[ix].addr;
                work.ch[ix].endFlg = false;

                //next work
                ix++;

                d = db;
                e = eb;

                //next channel
                work.seq_cur_ch += 1;

                e--;
            } while (e > 0);

        }

        //********************************************
        // seq_init_fmbase
        // initializes all fmbase
        //dest : ALL
        private void seq_init_fmbase()
        {
            ix = 0;// seq_work

            b = 18;// num of fmchan
            hl = 0;// fm_opbtbl;
            //seq_init_fmbase_lp1:
            do
            {
                a = fm_opbtbl[hl];
                work.ch[ix].opsel = a;
                hl++;
                ix++;
                b--;
            } while (b > 0);
        }

        //********************************************
        //seq_all_keyoff
        //this makes all keys off
        //
        private void moon_seq_all_keyoff()
        {

            moon_seq_all_release_fm();

            ix = 0;// seq_work;
            work.seq_cur_ch = 0;

            //seq_all_keyoff_lp:
            do
            {
                moon_key_off();

                //de = SEQ_WORKSIZE;
                ix++;//+= de;
                work.seq_cur_ch++;

            } while (work.seq_cur_ch < work.seq_use_ch);
            //seq_all_keyoff_end:

        }

        //
        // set RR to all fm channnels.
        private void moon_seq_all_release_fm()
        {
            // D = $80(reg adrs) E = (sl = $00, rr = $0f)
            d = 0x80;
            e = 0x0f;

            b = 18;

            hl = 0;// fm_opbtbl;

            // channel loop
            //moon_set_rr_ch_lp:
            do
            {
                // read opsel tbl
                a = fm_opbtbl[hl];
                work.seq_opsel = a;
                hl++;

                c = 4;
                //moon_set_rr_op_lp:
                do
                {
                    // write fm op
                    UInt16 de = (UInt16)(d * 0x100 + e);
                    moon_write_fmop();
                    d = (byte)(de >> 8);
                    e = (byte)(de & 0xff);

                    // add opsel

                    a = work.seq_opsel;
                    a += 3;
                    work.seq_opsel = a;

                    //
                    c--;
                } while (c > 0);

                b--;
            } while (b > 0);

        }


        //********************************************
        // moon_proc_tracks
        // Process tracks in 1/60 interrupts
        //
        private void moon_proc_tracks()
        {
            do
            {
                if (work == null)
                {
                    return;
                }

                // reset mapper
                a = 0;
                change_page3();

                ix = 0;//seq_work
                a = 0;

                work.seq_cur_ch = a;
                //proc_tracks_lp:
                int endCnt = 0;
                int loop = int.MaxValue;

                do
                {
                    if (work.ch[ix].endFlg)
                    {
                        endCnt++;
                        ix++;
                        work.seq_cur_ch++;
                        a = work.seq_cur_ch;
                        e = work.seq_use_ch;
                        continue;
                    }

                    proc_venv();
                    proc_penv();
                    proc_nenv();

                    proc_venv_reg();
                    proc_freq_reg();

                    seq_track();

                    if (ix < work.ch.Length && !work.ch[ix].endFlg && work.ch[ix].addr != 0x0)
                    {
                        loop = Math.Min(work.ch[ix].loopCnt, loop);
                    }

                    //ld de, SEQ_WORKSIZE
                    ix++;//add ix, de

                    a = work.seq_use_ch;
                    e = a;
                    a = work.seq_cur_ch;
                    a++;

                    if (CP_CF(e))
                    {
                        work.seq_cur_ch = a;
                    }
                    //Console.WriteLine("a:{0}", a);

                } while (CP_CF(e));

                if (endCnt == ix)
                {
                    Stopped = true;
                }

                vgmCurLoop = (uint)loop;

                //proc_tracks_end:
                a = work.seq_jump_flag;

            } while (a != 0);
            //jr moon_proc_tracks
        }

        //********************************************
        //seq_track
        //Count down and process in a channel
        //
        private void seq_track()
        {
            do
            {
                nextFlg = false;

                a = work.ch[ix].cnt;

                if (a != 0)
                {
                    a--;
                    work.ch[ix].cnt = a;
                    return;
                }

                //seq_cnt_zero:
                hl = work.ch[ix].addr;

                //seq_track_lp:
                do
                {
                    // Read command from memory
                    set_page3_ch();
                    a = ReadMemory(hl);
                    hl++;

                    if (CP_CF(0xe0))
                    {
                        seq_repeat_or_note();
                        break;
                    }

                    //seq_command:
                    //bc = seq_track_lp;
                    //push bc; < - return address
                    //  push hl; < -Preserve HL as pointer
                    //a += 0x20;
                    //a <<= 1;
                    //hl = a;
                    //bc = seq_jmptable;
                    //hl += (UInt16)((b << 8) + c);
                    // Read address from table
                    //a = ReadMemory(hl);
                    //hl++;
                    //hl = (UInt16)(ReadMemory(hl) * 0x100);
                    //hl = (UInt16)((hl & 0xff00) + a);

                    seq_jmptable[a - 0xe0]();
                    if (nextFlg) break;
                } while (!work.ch[ix].endFlg);
            } while (nextFlg);
        }

        //********************************************
        // seq_next
        // preserves address and do next
        // in   : HL
        // dest : AF
        private void seq_next()
        {

            work.ch[ix].addr = hl;
            //seq_track();
            nextFlg = true;
        }

        private void seq_repeat_or_note()
        {
            if (CP_CF(0x90))
            {
                seq_note();
                return;
            }
            if (CP_ZF(0xa1))
            {
                seq_repeat_esc();
                return;
            }

            // ********************************************
            //seq_repeat_end:
            a = work.ch[ix].loop;
            if (CP_ZF(0x01))
            {
                seq_skip_rep_jmp();
                return;
            }
            if (a == 0)
            {
                a = ReadMemory(hl);// read repeat counter
            }

            //seq_skip_set_repcnt_end:
            seq_rep_jmp();

        }

        //********************************************
        private void seq_repeat_esc()
        {
            a = work.ch[ix].loop;

            if (CP_ZF(0x01))
            {
                seq_rep_jmp();
                return;
            }

            if (a == 0)
            {
                a = ReadMemory(hl);// read repeat counter
            }

            //seq_skip_set_repcnt_esc:
            seq_skip_rep_jmp();
        }

        private void seq_skip_rep_jmp()
        {
            hl++;
            a--;
            work.ch[ix].loop = a;
            hl++;// bank
            hl++;// addr l
            hl++;// addr h

            seq_next();
        }

        private void seq_rep_jmp()
        {
            hl++;
            a--;

            work.ch[ix].loop = a;

            //bc = seq_next;
            //push    bc
            //push    hl

            //hl = seq_bank;// go to address
            //jp(hl)
            seq_bank();
            seq_next();
        }

        //********************************************
        private void seq_note()
        {

            UInt16 hlb = hl;
            byte af = a;

            a = 0;
            change_page3();

            a = work.ch[ix].dsel;
            if (a == 0)
            {
                seq_note_opl4(hlb, af);
                return;
            }

            //seq_note_fm:
            a = af;

            work.ch[ix].note = a;
            moon_set_fmnote();
            set_note_fin(hlb);
            return;
        }

        private void seq_note_opl4(UInt16 hlb, byte af)
        {
            //seq_note_opl4:
            a = af;
            conv_data_to_midi();

            work.ch[ix].note = a;
            moon_set_midinote();
            set_note_fin(hlb);
        }

        private void set_note_fin(UInt16 hlb)
        {
            //set_note_fin:
            set_page3_ch();
            moon_key_on();

            hl = hlb;

            // note length

            a = ReadMemory(hl);

            work.ch[ix].cnt = a;
            hl++;
            seq_next();
            return;

        }

        private void read_cmd_length()
        {
            // pop af
            a = ReadMemory(hl);
            work.ch[ix].cnt = a;
            hl++;
            seq_next();
        }


        private void start_venv()
        {
            a = work.ch[ix].venv;
            if (CP_ZF(0xff)) return;
            set_venv_head();
            proc_venv_start();
        }

        private void proc_venv()
        {
            a = work.ch[ix].venv;
            if (CP_ZF(0xff)) return;
            proc_venv_start();
        }

        private void proc_venv_start()
        {
            hl = work.ch[ix].venv_adr;
            a = (byte)(hl & 0xff);
            a |= (byte)(hl >> 8);

            if (a == 0) return;

            read_effect_value();

            if (CP_ZF(0xff))
            {
                //proc_venv_end:
                set_venv_loop();
                return;
            }
            hl++;
            work.ch[ix].venv_adr = hl;
            work.ch[ix].vol = a;

        }

        private void proc_venv_reg()
        {
            a = work.ch[ix].venv;

            if (CP_ZF(0xff)) return;
            moon_set_vol_ch();
        }


        private void start_penv()
        {
            a = work.ch[ix].penv;
            if (CP_ZF(0xff)) return;
            set_penv_head();
            proc_penv_start();
        }

        private void proc_penv()
        {
            a = work.ch[ix].penv;
            if (CP_ZF(0xff)) return;
            proc_penv_start();
        }

        private void proc_penv_start()
        {

            hl = work.ch[ix].penv_adr;
            read_effect_value();

            if (CP_ZF(0xff))
            {
                //proc_penv_end:
                set_penv_loop();
                return;
            }

            hl++;
            work.ch[ix].penv_adr = hl;

            byte af = a;//  push    af
            a = work.ch[ix].dsel;
            if (a == 0)
            {
                //proc_penv_opl4:
                a = af;

                hl = work.ch[ix].pitch;
                add_freq_offset();
                work.ch[ix].pitch = hl;

                moon_calc_opl4freq();
                return;
            }

            //proc_penv_fm:
            a = af;

            hl = work.ch[ix].fnum;
            add_freq_offset();

            a = (byte)(hl >> 8);
            if (!CP_CF(0x80))
            {
                //penv_fm_set_fnum:
                work.ch[ix].fnum = hl;
                moon_key_fmfreq();
                return;
            }

            d = 1;
            e = 0x5a;//de= 346;

            int ans = comp_hl_de();
            if (ans <= 0)
            {
                //penv_fm_dec_oct:
                //	; hl < de
                do
                {
                    work.ch[ix].oct--;
                    hl += (UInt16)((d << 8) + e);

                    ans = comp_hl_de();
                } while (ans < 0);

            }
            else
            {
                d = 2;
                e = 0xb5;//de=693

                ans = comp_hl_de();
                if (ans >= 0)
                {
                    //penv_fm_inc_oct:
                    //            ; hl > de

                    b = 1;
                    c = 0x5a;//bc= 346;

                    //penv_fm_inc_oct_lp:
                    do
                    {
                        work.ch[ix].oct++;

                        a = 0;
                        //    sbc hl, bc
                        hl -= (UInt16)((b << 8) + c);
                        ans = comp_hl_de();
                    } while (ans >= 0);
                }
            }

            //penv_fm_set_fnum:
            work.ch[ix].fnum = hl;
            moon_key_fmfreq();

        }

        private int comp_hl_de()
        {
            return hl - ((d << 8) + e);
        }

        private void start_nenv()
        {
            a = work.ch[ix].nenv;
            if (CP_ZF(0xff)) return;
            set_nenv_head();
            proc_nenv_start();
        }

        private void proc_nenv()
        {
            a = work.ch[ix].nenv;

            if (CP_ZF(0xff)) return;
            proc_nenv_start();
        }

        private void proc_nenv_start()
        {
            hl = work.ch[ix].nenv_adr;
            read_effect_value();

            if (CP_ZF(0xff))
            {
                set_nenv_loop();
                return;
            }

            hl++;
            work.ch[ix].nenv_adr = hl;

            byte af = a;//  push    af
            a = work.ch[ix].dsel;
            if (a != 0)
            {
                a = af;
                proc_nenv_fm();
            }
            else
            {
                a = af;
                proc_nenv_opl4();
            }
        }

        private void proc_nenv_opl4()
        {
            if ((a & 0x80) == 0)
            {

                a += work.ch[ix].note;
                work.ch[ix].note = a;

            }
            else
            {
                //proc_nenv_nega_opl4:
                a &= 0x7f;
                e = a;
                a = work.ch[ix].note;
                a -= e;
                work.ch[ix].note = a;
            }

            //proc_nenv_opl4_setnote
            moon_calc_midinote();
            moon_calc_opl4freq();
        }

        private void proc_nenv_fm()
        {

            b = 0x00;

            if ((a & 0x80) != 0)
            {
                proc_nenv_fm_nega();
                return;
            }
            //proc_nenv_fm_lp1:
            while (!CP_CF(0xc))
            {
                a -= 0xc;
                b++;
            }
            //proc_nenv_fm_add:
            c = a;// C = (note % 12)
            a = work.ch[ix].note;
            a &= 0xf;
            a += c;

            if (!CP_CF(0xc))
            {
                a += 0x4;
                a &= 0xf;
                b++;
            }
            //skip_nenv_inc_oct:
            c = a;// C = (note & 0x0f)

            a = b;// B = oct

            a = (byte)((a << 1) + (a >> 7));//rlca
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));

            b = a;
            a = work.ch[ix].note;

            a &= 0xf0;
            a += b;
            a |= c;

            work.ch[ix].note = a;
            moon_set_fmnote();
            moon_key_fmfreq();
        }

        private void proc_nenv_fm_nega()
        {
            a &= 0x7f;
            //proc_nenv_fm_nega_lp1
            do
            {
                if (CP_CF(0xc))
                {
                    break;
                }
                a -= 0xc;
                b++;
            } while (true);
            //proc_nenv_fm_sub:
            c = a;// C = (note % 12)

            a = work.ch[ix].note;
            a &= 0xf;
            int ai = a - c;
            a = (byte)ai;
            if (ai < 0)
            {
                a -= 0x04;
                a &= 0xf;
                b++;
            }
            //skip_nenv_dec_oct:
            c = a;// C = (note & 0x0f)
            a = b;// B = oct

            a = (byte)((a << 1) + (a >> 7));//rlca
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));

            b = a;
            a = work.ch[ix].note;
            a &= 0xf0;
            a -= b;
            a |= c;

            work.ch[ix].note = a;
            moon_set_fmnote();
            moon_key_fmfreq();

        }

        //********************************************
        //Set frequency to registers actually
        //
        private void proc_freq_reg()
        {
            a = work.ch[ix].penv;

            if (!CP_ZF(0xff))
            {
                moon_set_freq_ch();
                return;
            }
            a = work.ch[ix].nenv;

            if (!CP_ZF(0xff))
            {
                moon_set_freq_ch();
                return;
            }
            return;
            //proc_freq_to_moon:
            //jp moon_set_freq_ch
        }

        private void seq_nop()
        {
            //nothing
        }

        private void seq_skip_1()
        {
            //pop hl
            hl++;
        }

        // cmd $FF : loop point
        private void seq_loop()
        {

            a = 0;
            change_page3();

            hl = S_LOOP_TABLE;
            get_hl_table();

            UInt16 hl1 = hl;

            hl = S_LOOP_BANK;
            get_a_table();

            work.ch[ix].bank = a;
            change_page3();

            hl = hl1;

        }

        // cmd $FD : volume
        private void seq_volume()
        {

            a = ReadMemory(hl);
            work.ch[ix].venv = a;

            if ((0x80 & a) == 0)
            {
                seq_venv();
                return;
            }

            a &= 0x7f;
            work.ch[ix].vol = a;

            a = 0xff;
            work.ch[ix].venv = a;// venv = off

            moon_set_vol_ch();

            hl++;

        }

        private void seq_venv()
        {
            set_venv_head();
            hl++;
        }

        private void seq_rest()
        {
            moon_key_off();
            read_cmd_length();

            if (work.ch[ix].cnt == 255)
            {
                byte vee = ReadMemory(hl);
                byte v00 = ReadMemory((UInt16)(hl+1));
                UInt16 adr = (UInt16)(ReadMemory((UInt16)(hl + 2)) + ReadMemory((UInt16)(hl + 3)) * 0x100);
                if (vee==0xee && v00==0x00 && hl - 2 == adr)
                {
                    work.ch[ix].endFlg = true;
                }
            }
        }

        private void seq_detune()
        {
            a = ReadMemory(hl);

            work.ch[ix].detune = a;
            hl++;
        }

        private void seq_penv()
        {
            a = ReadMemory(hl);

            hl++;
            work.ch[ix].penv = a;
            if (!CP_ZF(0xff))
            {
                set_penv_head();
            }
        }

        private void seq_nenv()
        {
            a = ReadMemory(hl);

            hl++;
            work.ch[ix].nenv = a;
            if (!CP_ZF(0xff))
            {
                set_nenv_head();
            }

        }

        private void seq_data_write()
        {
            a = ReadMemory(hl);
            d = a;// Address Low
            hl++;

            a = ReadMemory(hl);// Address High
            if (a == 0)
            {
                write_data_cur_fm();// (a >> 8) == 0
                return;
            }

            a--;
            if (a == 0)
            {
                write_data_fm1();// (a >> 8) == 1
                return;
            }

            a--;
            if (a == 0)
            {
                write_data_fm2();// (a >> 8) == 2
                return;
            }

            hl++;
        }

        private void write_data_cur_fm()
        {
            hl++;

            a = ReadMemory(hl);

            e = a;// Data
            hl++;

            a = work.seq_cur_ch;
            if (CP_CF(9))
                moon_fm1_out();
            else
                moon_fm2_out();
        }

        private void write_data_fm1()
        {
            hl++;

            a = ReadMemory(hl);

            e = a;// Data
            hl++;
            moon_fm1_out();
        }

        private void write_data_fm2()
        {
            hl++;

            a = ReadMemory(hl);

            e = a;// Data
            hl++;
            moon_fm2_out();
        }

        private void seq_wait()
        {
            read_cmd_length();
        }

        private void seq_drum()
        {
            a = ReadMemory(hl);

            a &= 0x1f;

            e = a;

            a = work.seq_reg_bd;

            a &= 0xe0;
            a |= e;

            work.seq_reg_bd = a;
            e = a;
            d = 0xbd;
            moon_fm1_out();
            hl++;
        }

        private void seq_drumbit()
        {
            // drums key-off

            a = ReadMemory(hl);
            a &= 0x1f;
            a ^= 0xff;

            // e = mask bits of drums
            e = a;
            a = work.seq_reg_bd;
            a &= e;

            work.seq_reg_bd = a;
            e = a;
            d = 0xbd;
            moon_fm1_out();

            // set fnum
            UInt16 hlb = hl;
            a = ReadMemory(hl);
            a &= 0x1f;

            c = 0;
            b = 5;

            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));//rlca
            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));
            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));

            // A = drum bits, BC = count
            //drumbit_fnum_lp:
            do
            {
                bool cry = (a & 0x80) != 0;
                a = (byte)((a << 1) + (cry ? 1 : 0));//rlca
                if (cry)
                {
                    byte af = a;
                    byte bb = b;
                    byte cb = c;
                    drumbit_set_fnum();// set Fnum for drums
                    b = bb;
                    c = cb;
                    a = af;
                }
                //drumbit_fnum_next:
                c++;
                b--;
            } while (b > 0);

            hl = hlb;

            // skip if jump flag is true
            a = work.seq_jump_flag;
            if (a == 0)
            {
                // drums key-on
                a = ReadMemory(hl);
                a &= 0x1f;
                e = a;
                a = work.seq_reg_bd;
                a &= 0xe0;
                a |= e;
                work.seq_reg_bd = a;
                e = a;
                d = 0xbd;
                moon_fm1_out();
            }

            //drumbit_skip_keyon:
            // length check
            a = ReadMemory(hl);
            a &= 0x80;// Lxxxxxxx L = the command has length
            if (a != 0)
            {
                // drumbit with length
                //drumbit_with_length:
                hl++;
                read_cmd_length();
                return;
            }
            hl++;

        }

        // Set Fnum for drum
        // C = index
        // dest : almost all
        private void drumbit_set_fnum()
        {
            // fnum
            hl = 0;//fm_drum_fnum
            b = 0;
            work.seq_tmp_fnum = fm_drum_fnum[c];

            // oct
            //fm_drum_oct
            work.seq_tmp_oct = fm_drum_oct[c];

            // ch
            //fm_drum_fnum_map
            work.seq_tmp_ch = fm_drum_fnum_map[c];

            // write FnumL
            moon_key_write_fmfreq_base();

            e = a;
            d = 0xb0;
            a = work.seq_tmp_ch;

            // write FnumH and BLK
            moon_write_fmreg_nch();

        }

        private void seq_drumnote()
        {
            // set fnum
            a = ReadMemory(hl);
            a &= 0x1f;

            byte af = a;
            hl++;
            a = ReadMemory(hl);
            work.seq_tmp_note = a;
            hl++;
            a = af;

            UInt16 hlb = hl;
            c = 0;
            b = 5;

            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));//    rlca
            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));//    rlca
            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));//    rlca

            // A = drum bits, BC = count
            //drumnote_lp:
            do
            {
                bool cry = (a & 0x80) != 0;
                a = (byte)((a << 1) + (cry ? 1 : 0));//    rlca

                if (!cry)
                {
                    //drumnote_next:
                    c++;
                }
                else
                {
                    af = a;
                    byte bb = b;
                    byte cb = c;
                    drumnote_fnum();// set Fnum for drums
                    b = bb;
                    c = cb;
                    a = af;
                }
                b--;
            } while (b > 0);

            hl = hlb;
            hl++;
        }

        // C = index
        // dest: AF, BC, HL
        private void drumnote_fnum()
        {
            byte bb = b;
            byte cb = c;
            a = work.seq_tmp_note;
            moon_calc_opl3note();
            b = bb;
            c = cb;

            // oct
            b = 0;

            hl = 0;//fm_drum_oct
            hl += c;
            a = work.seq_tmp_oct;
            fm_drum_oct[hl] = a;

            // fnum
            hl = 0;//fm_drum_fnum
            hl += c;
            fm_drum_fnum[hl] = work.seq_tmp_fnum;

        }

        private void seq_inst()
        {
            //pop hl
            a = ReadMemory(hl);
            byte af = a;
            UInt16 hlb;
            a = 0;
            change_page3();

            // Device select
            a = work.ch[ix].dsel;
            if (a == 0)
            {
                //seq_inst_opl4:
                a = af;
                hlb = hl;
                hl = S_INST_TABLE;
                get_table();
                work.ch[ix].tadr = hl;

                //seq_inst_fin:
                set_page3_ch();
                hl = hlb;
                hl++;
                return;
            }

            // Load OPL3 instrument
            a = af;
            hlb = hl;
            hl = S_OPL3_TABLE;
            get_table();
            work.ch[ix].tadr = hl;
            // set tone to FM
            moon_set_fmtone();

            //seq_inst_fin:
            set_page3_ch();
            hl = hlb;
            hl++;
        }

        private void seq_pan()
        {
            // Device select
            a = work.ch[ix].dsel;

            if (a == 0)
            {
                //seq_pan_opl4:
                a = ReadMemory(hl);
                work.ch[ix].pan = a;
            }
            else
            {
                //seq_pan_fm:
                a = ReadMemory(hl);
                a &= 0xf;
                a = (byte)((a << 1) + (((a & 0x80) != 0) ? 1 : 0)); //rlca
                a = (byte)((a << 1) + (((a & 0x80) != 0) ? 1 : 0)); //rlca
                a = (byte)((a << 1) + (((a & 0x80) != 0) ? 1 : 0)); //rlca
                a = (byte)((a << 1) + (((a & 0x80) != 0) ? 1 : 0)); //rlca

                work.ch[ix].pan = a; // PPPPxxxx
                moon_write_fmpan();// Write PAN to FM
            }

            //seq_pan_fin:
            hl++;
        }

        private void seq_lfosw()
        {
            a = ReadMemory(hl);
            work.ch[ix].lfo = a;
            hl++;
        }

        private void seq_bank()
        {
            a = ReadMemory(hl);
            hl++;

            byte af = a;

            a = ReadMemory(hl);
            hl++;
            hl = (UInt16)(ReadMemory(hl) * 0x100 + a);

            a=0;
            change_page3();
            UInt16 ltbl = (UInt16)(ReadMemory(S_LOOP_TABLE) + ReadMemory(S_LOOP_TABLE + 1) * 0x100 + ix * 2);
            ltbl = (UInt16)(ReadMemory(ltbl) + ReadMemory((UInt16)(ltbl + 1)) * 0x100);
            if (hl == ltbl)
            {
                work.ch[ix].loopCnt += (work.ch[ix].loopCnt == int.MaxValue) ? 0 : 1;
            }

            a = af;

            work.ch[ix].bank = a;
            change_page3();

        }

        private void seq_damp()
        {
            // Device select
            a = work.ch[ix].dsel;
            if (a == 0)
            {
                //seq_damp_opl4:
                a = ReadMemory(hl);
                work.ch[ix].damp = a;
            }
            else
            {
                a = ReadMemory(hl);
                a &= 0x3f;
                e = a;
                d = 0x4;

                moon_fm2_out();
            }
            //seq_damp_fin:
            hl++;
        }

        private void seq_revbsw()
        {
            a = ReadMemory(hl);
            work.ch[ix].reverb = a;
            hl++;
        }

        private void seq_slar()
        {
            work.ch[ix].efx1 |= 1;
        }

        private void seq_setop()
        {
            a = ReadMemory(hl);
            work.ch[ix].opsel = a;
            hl++;
        }

        private void seq_ld2ops()
        {
            a = work.ch[ix].dsel;
            if (a == 0)
            {
                hl++;
                return;
            }

            //seq_ld2ops_fm:
            a = ReadMemory(hl);
            byte af = a;
            a = 0;
            change_page3();
            a = af;

            UInt16 hlb = hl;
            hl = S_OPL3_TABLE;

            get_table();

            work.ch[ix].tadr = hl;

            // Set 2OP tone to FM
            moon_set_fmtone2();
            set_page3_ch();

            hl = hlb;

            hl++;
        }

        private void seq_tvp()
        {
            a = ReadMemory(hl);
            a &= 0x7;

            a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));//rrca
            a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));
            a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));
            e = a;
            a = work.seq_reg_bd;
            a &= 0x1f;
            a |= e;

            work.seq_reg_bd = a;
            e = a;
            d = 0xbd;
            moon_fm1_out();
            hl++;
        }

        private void seq_fbs()
        {
            a = ReadMemory(hl);
            a &= 0x7;

            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));//rlca
            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));

            e = a;
            a = work.ch[ix].synth;
            a &= 0xe3;
            a |= e;

            work.ch[ix].synth = a;
            hl++;
        }

        private void seq_jump()
        {
            a = ReadMemory(hl);
            work.seq_jump_flag = a;
            hl++;
        }

        //********************************************
        // pause_venv
        // dest : AF
        //pause_venv:

        //xor a

        //ld(ix + IDX_VENV_ADR), a
        //ld(ix + IDX_VENV_ADR+1), a
        //ret

        //********************************************
        // read_effect_table
        // in   : A  = index
        //      : HL = table address
        //      : DE = pointer to value in work
        // out  : (ix + de) = (HL + 2A)
        // dest : AF
        private void read_effect_table(ref UInt16 adr)
        {
            byte af = a;
            a = 0;
            change_page3();
            a = af;

            get_table();

            adr = hl;

            set_page3_ch();
        }

        private void set_venv_loop()
        {
            UInt16 hlb = hl;

            hl = S_VENV_LOOP;

            //jr set_venv_hl
            //set_venv_hl:
            //de = IDX_VENV_ADR;

            a = work.ch[ix].venv;
            a &= 0x7f;
            read_effect_table(ref work.ch[ix].venv_adr);

            hl = hlb;
        }

        private void set_venv_head()
        {
            UInt16 hlb = hl;

            hl = S_VENV_TABLE;

            //set_venv_hl:
            //de = IDX_VENV_ADR;
            a = work.ch[ix].venv;
            a &= 0x7f;
            read_effect_table(ref work.ch[ix].venv_adr);

            hl = hlb;
        }


        //********************************************
        // set_penv_loop
        // dest : AF, DE
        private void set_penv_loop()
        {
            UInt16 hlb = hl;

            hl = S_PENV_LOOP;

            //jr set_penv_hl
            //set_penv_hl:
            //de = IDX_PENV_ADR;

            a = work.ch[ix].penv;
            read_effect_table(ref work.ch[ix].penv_adr);

            hl = hlb;
        }

        private void set_penv_head()
        {
            UInt16 hlb = hl;

            hl = S_PENV_TABLE;

            //set_penv_hl:
            //de = IDX_PENV_ADR;
            a = work.ch[ix].penv;
            read_effect_table(ref work.ch[ix].penv_adr);

            hl = hlb;
        }

        //********************************************
        // set_nenv_loop
        // dest : AF, DE
        private void set_nenv_loop()
        {
            //    push hl
            UInt16 hl1 = hl;
            hl = S_NENV_LOOP;
            //set_nenv_hl:
            //de = IDX_NENV_ADR;
            a = work.ch[ix].nenv;
            read_effect_table(ref work.ch[ix].nenv_adr);
            hl = hl1;

        }

        //********************************************
        // set_nenv_head
        // dest : AF, DE
        private void set_nenv_head()
        {
            //    push hl
            UInt16 hl1 = hl;
            hl = S_NENV_TABLE;
            //set_nenv_hl:
            //de = IDX_NENV_ADR;
            a = work.ch[ix].nenv;
            read_effect_table(ref work.ch[ix].nenv_adr);
            hl = hl1;

        }

        //********************************************
        // read_effect_value
        // read_effect_value
        // in  : HL = address
        // out : A = data
        private void read_effect_value()
        {

            a = 0;
            change_page3();

            a = ReadMemory(hl);
            byte af = a;
            set_page3_ch();
            a = af;

        }

        //********************************************
        // conv_data_to_midi
        // Converts data to midi note
        // in   : A = data($40 = o4c)
        // out  : A = midi note
        // dest : AF,DE
        private void conv_data_to_midi()
        {
            d = 0x00;
            e = a;

            //a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));
            //a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));
            //a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));
            //a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0));
            a >>= 4;
            a &= 0xf;
            d = a;
            if (a != 0)
            {
                a = 0;
                //conv_midi_lp:
                do
                {
                    a += 0x0c;
                    d--;
                } while (d != 0);
            }
            //skip_conv_midi_lp:
            d = a;
            a = e;
            a &= 0xf;
            a += d;
            a += 0xc;

        }

        //********************************************
        // oct_div
        // octave divider
        // in   : H = pitch / 0x100
        // out  : L = octave
        // dest : AF
        private void oct_div()
        {
            a = (byte)(hl >> 8);
            hl &= 0xff00;

            //oct_div_lp:
            do
            {
                int ac = a + 0xfa;
                a = (byte)(ac);
                if (ac < 0x100) return;
                byte l = (byte)(hl & 0xff);
                l++;
                hl = (UInt16)((hl & 0xff00) + l);
            } while (true);
        }

        //********************************************
        // make_fnum
        // Make F-number from pitch
        // F-num = pitch / $600
        // in   : HL = pitch
        // out  : HL = f-num
        // dest : AF, DE
        private void make_fnum()
        {

            d = 0xfa;
            e = 0;
            //make_fnum_lp:
            int hlc;
            do
            {
                hlc = hl + ((d << 8) + e);
                hl = (UInt16)(hlc);
            } while (hlc > 0xffff);

            d = 0x06;
            e = 0;
            hl += (UInt16)((d << 8) + e);
            UInt16 de = hl;
            hl = (UInt16)((d << 8) + e);
            d = (byte)(de >> 8);
            e = (byte)(de & 0xff);

            hl = 0;//    ld hl, freq_table
            hl += (UInt16)((d << 8) + e);
            //hl += (UInt16)((d << 8) + e);
            hl = freq_table[hl];//    ld a, (hl)
            //hl++;
            //    ld h, (hl)
            //    ld l, a

        }

        //********************************************
        // moon_set_fmtone2
        // load and set 2 oprators from data in table
        // in   : work
        // dest : DE
        //
        private void moon_set_fmtone2()
        {
            //push af
            //push bc
            //push hl
            // repeat 2times
            moon_set_fmtone_start_lp(2);
        }

        //********************************************
        // moon_set_fmtone
        // load and set 4 oprators from data in table
        // in   : work
        // dest : DE
        //
        private void moon_set_fmtone()
        {
            //push af
            //push bc
            //push hl
            // repeat 4 times
            moon_set_fmtone_start_lp(4);
        }

        private void moon_set_fmtone_start_lp(byte rt)
        {
            byte af = a;
            byte bb = b;
            byte cb = c;
            UInt16 hlb = hl;
            c = rt;

            hl = work.ch[ix].tadr;
            a = work.ch[ix].opsel;
            work.seq_opsel = a;

            // FBS store to IDX_SYNTH(OxxFFFSS )
            a = ReadMemory(hl);
            a &= 0x0e;
            a = (byte)((a << 1) + (((a & 0x80) != 0) ? 1 : 0)); //rlca
            e = a;
            a = ReadMemory(hl);
            a &= 0x01;
            a |= e;
            e = a;
            hl++;

            a = work.ch[ix].synth;
            a &= 0xe2;
            a |= e;
            work.ch[ix].synth = a;// xxxFFFxS

            a = c;
            if (!CP_ZF(0x04))
            {
                //fmtone_skip_set_fbs2:
                a = work.ch[ix].synth;
                a &= 0x7f;
                work.ch[ix].synth = a;
                hl++;
            }
            else
            {
                // FBS-2  Store SynthType for 4OP
                a = ReadMemory(hl);
                a &= 0x01;
                a = (byte)((a << 1) + (((a & 0x80) != 0) ? 1 : 0)); //rlca
                a |= 0x80;// 4OP flag
                e = a;
                a = work.ch[ix].synth;
                a &= 0x7d;// mask for 4OP and 2nd SynthType
                a |= e;

                work.ch[ix].synth = a;// OxxFFFSS
                hl++;
            }

            //fmtone_set_tvp:
            // TYP in W / WX is removed
            hl++;
            moon_load_fmvol();

            //moon_set_fmtone_lp1:
            do
            {
                moon_set_fmop();
                a = work.seq_opsel;
                a += 0x03;
                work.seq_opsel = a;
                c--;
            } while (c != 0);

            moon_write_fmpan();

            hl = hlb;
            b = bb;
            c = cb;
            a = af;
        }

        private void moon_load_fmvol()
        {
            UInt16 hlb = hl;
            byte bb = b;
            byte cb = c;
            UInt16 ixb = ix;

            hl++;// skip Reg.$20
            d = 0;
            e = 5;

            //moon_load_fmvol_lp:
            int i = 0;
            do
            {
                a = ReadMemory(hl);
                work.ch[ix].ol[i] = a;//.ar_d1r = a;
                i++;// ix++;
                hl += (UInt16)((d << 8) + e);
                c--;
            } while (c != 0);

            ix = ixb;
            b = bb;
            c = cb;
            hl = hlb;
        }

        private void moon_set_fmop()
        {
            a = ReadMemory(hl);
            e = a;
            d = 0x20;
            moon_write_fmop();
            hl++;

            a = ReadMemory(hl);
            e = a;
            d = 0x40;
            moon_write_fmop();// OL
            hl++;

            a = ReadMemory(hl);
            e = a;
            d = 0x60;
            moon_write_fmop();
            hl++;

            a = ReadMemory(hl);
            e = a;
            d = 0x80;
            moon_write_fmop();
            hl++;

            a = ReadMemory(hl);
            e = a;
            d = 0xe0;
            moon_write_fmop();
            hl++;
        }

        //********************************************
        // moon_tonesel
        // Select tone number from table(OPL4 )
        // in   : work
        // dest : flags, HL
        //
        private void moon_tonesel()
        {
            hl = work.ch[ix].tadr;
            byte af;
            //tonesel_lp01:
            do
            {
                af = a;
                UInt16 hlb = hl;

                a = ReadMemory(hl);
                hl++;
                a |= ReadMemory(hl);

                if (a == 0)
                {
                    //tonesel_fin();//; if min == 0 && max == 0
                    hl = hlb;
                    a = af;
                    return;
                }

                hl = hlb;
                a = af;

                if (a - ReadMemory(hl) < 0)
                {
                    //tonesel_skip01();// if a<(hl)
                    hl++;
                    hl += 0x000a;
                }
                else
                {
                    hl++;
                    if (a - ReadMemory(hl) <= 0)
                    {
                        //tonesel_loadtone();// if a<(hl)
                        break;
                    }
                    else
                    {
                        //tonesel_skip02();
                        hl += 0x000a;
                    }
                }
            } while (true);

            //tonesel_loadtone:
            af = a;
            hl++;
            a = ReadMemory(hl);
            work.ch[ix].tone = a;
            hl++;
            a = ReadMemory(hl);
            work.ch[ix].tone += (UInt16)(a << 8);
            hl++;

            a = ReadMemory(hl);
            work.ch[ix].p_ofs = a;
            hl++;
            a = ReadMemory(hl);
            work.ch[ix].p_ofs += (UInt16)(a << 8);
            hl++;

            a = ReadMemory(hl);
            work.ch[ix].lfo_vib = a;
            hl++;

            a = ReadMemory(hl);
            work.ch[ix].ol[0] = a;//.ar_d1r = a;
            hl++;

            a = ReadMemory(hl);
            work.ch[ix].ol[1] = a;//.dl_d2r = a;
            hl++;

            a = ReadMemory(hl);
            work.ch[ix].ol[2] = a;//.rc_rr = a;
            hl++;

            a = ReadMemory(hl);
            work.ch[ix].ol[3] = a;//.am = a;
            hl++;

            a = af;
        }

        //********************************************
        // moon_calc_opl4freq
        // in   : HL = pitch
        // dest : AF,HL
        private void moon_calc_opl4freq()
        {
            UInt16 hl1 = hl;

            oct_div();
            a = (byte)(hl & 0xff);
            a += 0xf8;
            work.ch[ix].oct = a;

            hl = hl1;

            make_fnum();

            work.ch[ix].fnum = hl;
        }

        //********************************************
        // moon_calc_midinote
        // calclulates freq from note
        // in   : A = note( 04c = 0x3c)
        // out  : work
        // dest : AF,DE
        private void moon_calc_midinote()
        {
            a += 0xc4;// a -= $3c
            hl &= 0xff00;
            bool cry = (a & 1) != 0;
            a >>= 1;
            hl = (UInt16)((hl & 0xff) + (a << 8));
            byte l = (byte)(hl & 0xff);
            bool cry2 = (l & 0x1) != 0;
            l =(byte)((l >> 1) + (cry ? 0x80 : 0));
            if (cry2) l |= 0x80;
            hl = (UInt16)((hl & 0xff00) + l);

            a &= 0x40;
            if (a != 0)
            {
                a = (byte)(hl >> 8);
                a |= 0x80;
                hl = (UInt16)((hl & 0xff) + (a << 8));
            }
            //skip_set_nega; if a >= $80 then it's negative
            d = 0x1e;// 7680
            e = 0;
            hl += (UInt16)((d << 8) + e);

            e = (byte)(work.ch[ix].p_ofs & 0xff);
            d = (byte)(work.ch[ix].p_ofs >> 8);
            hl += (UInt16)((d << 8) + e);

            a = work.ch[ix].detune;
            add_freq_offset();

            //skip_detune:
            a = (byte)(hl >> 8);

            if (!CP_CF(0x60))
            {
                hl = 0x5fff;
            }
            //skip_set_pitch:

            work.ch[ix].pitch = hl;
        }

        //********************************************
        // add_freq_offset
        // HL = HL + VALUE
        // in : A(detune data)
        // dest : DE
        private void add_freq_offset()
        {
            if (CP_ZF(0xff)) return;

            if ((a & 0x80) != 0)
            {
                //add_freq_nega:
                a &= 0x7f;
                d = 0;
                e = a;
                a = 0;
                int ac = (int)a - (int)e;
                a = (byte)ac;
                if (ac < 0)
                {
                    d--;
                }
            }
            else
            {
                e = a;
                d = 0;
            }

            //add_freq_de:
            e = a;
            hl += (UInt16)((d << 8) + e);
            hl += (UInt16)((d << 8) + e);
        }

        //********************************************
        // moon_set_midinote
        // in : A = note
        //
        private void moon_set_midinote()
        {
            if ((work.ch[ix].efx1 & 1) == 0)
            {
                moon_tonesel();
            }

            moon_calc_midinote();

            moon_calc_opl4freq();
        }

        //********************************************
        // moon_set_fmnote
        // in   : A = note
        // dest : AF, HL, BC
        private void moon_set_fmnote()
        {
            moon_calc_opl3note();

            // oct
            a = work.seq_tmp_oct;
            work.ch[ix].oct = a;

            // Fnum
            hl = work.seq_tmp_fnum;
            work.ch[ix].fnum = hl;
        }

        //********************************************
        // moon_calc_opl3note
        // in : A = note , (ix + IDX_DETUNE)
        // out : (seq_tmp_fnum), (seq_tmp_oct)
        // dest AF, BC, HL
        private void moon_calc_opl3note()
        {
            byte af = a;//	push af

            a &= 0xf;
            if (!CP_CF(0xc))
            {
                a -= 0xc;
            }
            //fm_load_fnumtbl:

            //   ld hl, fm_fnumtbl

            c = a;
            b = 0;
            hl = (UInt16)c;
            //hl += (UInt16)((b << 8) + c);
            //hl += (UInt16)((b << 8) + c);

            //    ld a, (hl)
            work.seq_tmp_fnum = fm_fnumtbl[hl];
            //hl++;
            //  ld  a, (hl)
            //  ld(seq_tmp_fnum + 1), a

            a = af;

            a >>= 4;
            //bool cry = false;
            //bool cryB;
            //cryB = (a & 1) != 0; a = (byte)((a >> 1) | (cry ? 0x80 : 0x00)); cry = cryB;
            //cryB = (a & 1) != 0; a = (byte)((a >> 1) | (cry ? 0x80 : 0x00)); cry = cryB;
            //cryB = (a & 1) != 0; a = (byte)((a >> 1) | (cry ? 0x80 : 0x00)); cry = cryB;
            //cryB = (a & 1) != 0; a = (byte)((a >> 1) | (cry ? 0x80 : 0x00)); cry = cryB;
            //a &= 0xf;
            work.seq_tmp_oct = a;

            // Add detune effect
            hl = work.seq_tmp_fnum;

            a = work.ch[ix].detune;
            add_freq_offset();

            work.seq_tmp_fnum = hl;
        }

        //********************************************
        // moon_write_fmop
        // Write an OPL3 reg for op
        // (opsel)
        // in   : seq_opsel, D = addr, E = data
        // dest : AF, DE
        private void moon_write_fmop()
        {

            a = work.seq_opsel;
            if (a >= 0x12)
            {
                a -= 0x12;
            }

            //skip_sub_a:
            UInt16 hl2 = 0;//fm_op2reg_tbl; HL = HL + A
            hl2 += a;
            //add_hl_fin:

            a = fm_op2reg_tbl[hl2];
            a += d;
            d = a;


            a = work.seq_opsel;
            if (CP_CF(0x12))
            {
                moon_fm1_out();
            }
            else
            {
                moon_fm2_out();
            }
            //moon_write_fmop_1:
        }

        //********************************************
        // moon_write_fmreg
        // moon_write_fmreg_ch
        // Write to OPL3 register
        // in : D = addr, E = data
        // dest : AF, DE
        //
        private void moon_write_fmreg()
        {
            a = work.seq_cur_ch;
            moon_write_fmreg_nch();
        }

        private void moon_write_fmreg_nch()
        {
            //; A = ch
            //moon_write_fmreg_nch:

            byte e1, d1;
            e1 = a;
            a = work.seq_start_fm;
            d1 = a;
            a = e1;
            a -= d1; // a = ch, d = start_fm

            //jr moon_write_fm_ch
            //; Write fm(A = ch)
            //moon_write_fm_ch:

            // first or second FM register
            if (CP_CF(9))
            {
                // first register
                //moon_write_fm1:
                a += d;
                d = a;
                moon_fm1_out();
                return;
            }

            // second register
            a -= 9;
            a += d;
            d = a;
            moon_fm2_out();
        }

        private void moon_wait()
        {
            //in	a, (MOON_STAT)
            //and $01
            //jr nz, moon_wait
            //ret
        }

        private void moon_fm1_out()
        {
            //call moon_wait
            //ld a, d
            //out	(MOON_REG1), a
            //call    moon_wait
            //ld  a, e
            //out	(MOON_DAT1), a
            //ret
            chipRegister.setYMF278BRegister(0, 0, d, e);
            //Console.WriteLine("fm1out:{0:x02}:{1:x02}:", d, e);
        }

        private void moon_fm2_out()
        {
            //call moon_wait
            //ld a, d
            //out	(MOON_REG2), a
            //call    moon_wait
            //ld  a, e
            //out	(MOON_DAT2), a
            //ret
            chipRegister.setYMF278BRegister(0, 1, d, e);
            //Console.WriteLine("fm2out:{0:x02}:{1:x02}:", d, e);
        }

        private void moon_wave_out()
        {
            //call moon_wait
            //ld a, d
            //out	(MOON_WREG),a
            //call    moon_wait
            //ld  a, e
            //out	(MOON_WDAT),a
            //ret
            chipRegister.setYMF278BRegister(0, 2, d, e);
            backDat = e;
            //Console.WriteLine("wave out:{0:x02}:{1:x02}:", d, e);
        }

        private byte backDat=0;

        private byte moon_wave_in()
        {
            //call moon_wait
            //ld a, d
            //out	(MOON_WREG),a
            //call    moon_wait
            //in	a, (MOON_WDAT)
            //ret
            a = backDat;
            return a;
        }

        private void moon_add_reg_ch()
        {
            a = work.seq_cur_ch;
            a += d;
            d = a;
        }

        //********************************************
        // set frequency on the channel
        // in   : work
        // dest : AF,DE
        private void moon_set_freq_ch()
        {

            a = work.ch[ix].dsel;
            if (a != 0) return;

            //set_freq_ch_opl4:
            //; ocatve and f - number(hi)
            a = (byte)(work.ch[ix].fnum >> 8);

            a = (byte)((a << 1) + (a >> 7));//rlca
            a &= 0x0e;
            e = a;
            a = (byte)(work.ch[ix].fnum & 0xff);
            a = (byte)((a << 1) + (a >> 7));//rlca
            a &= 0x01;
            a |= e;
            e = a;

            a = work.ch[ix].oct;
            a &= 0xf;
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));
            a = (byte)((a << 1) + (a >> 7));
            a |= e;
            e = a;

            a = work.ch[ix].reverb;
            if (a != 0)
            {
                e |= 0x8;
            }

            //moon_set_freq_ch_skip_reverb:
            d = 0x38;
            moon_add_reg_ch();
            moon_wave_out();

            // f - number(lo)
            a = (byte)(work.ch[ix].tone >> 8);
            a &= 1;
            e = a;

            a = (byte)(work.ch[ix].fnum & 0xff);
            a = (byte)((a << 1) + (a >> 7));
            a &= 0xfe;
            a |= e;
            e = a;
            d = 0x20;

            moon_add_reg_ch();
            moon_wave_out();

        }

        private void moon_set_vol_ch()
        {
            a = work.ch[ix].dsel;
            if (a != 0)
            {
                moon_set_fmvol_ch();
                return;
            }
            a = work.ch[ix].vol;
            a ^= 0x7f;
            a = (byte)((a << 1) + (a >> 7));
            a |= 1;
            e = a;
            d = 0x50;
            moon_add_reg_ch();
            moon_wave_out();
        }

        private void moon_set_fmvol_ch()
        {
            UInt16 ixb = ix;
            UInt16 hlb = hl;
            byte bb = b;
            byte cb = c;

            a = work.ch[ix].opsel;
            work.seq_opsel = a;

            b = work.ch[ix].vol;
            hl = (UInt16)((hl & 0xff)+ (UInt16)(work.ch[ix].reverb * 0x100));//.volop;

            c = 0x02;
            a = work.ch[ix].synth;
            a &= 0x80;
            if (a != 0)
            {
                c = 0x04;
            }

            //moon_set_fmvol_lp
            int i = 0;
            do
            {
                byte h = (byte)(hl >> 8);
                bool cry = false, cryb;
                cryb = (h & 1) != 0;
                h = (byte)((h >> 1) + (cry ? 0x80 : 0x00));
                hl = (UInt16)((h << 8) + (hl & 0xff));
                cry = cryb;
                if (cry)
                {
                    moon_calc_current_fmvol(i);
                    e = a;
                    d = 0x40;
                    moon_write_fmop();
                }
                //skip_set_fmvol:
                i++;// ix++;

                a = work.seq_opsel;
                a += 3;
                work.seq_opsel = a;

                c--;
            } while (c != 0);

            b = bb;
            c = cb;
            hl = hlb;
            ix = ixb;
        }

        private void moon_calc_current_fmvol(int i)
        {
            // A = (OL + (63 - VOL))

            a = b;
            a &= 0x3f;
            a ^= 0x3f;
            e = a;
            a = work.ch[ix].ol[i];//.ar_d1r;
            a &= 0x3f;
            a += e;

            if (!CP_CF(0x40))
            {
                //set_fmvol_min:
                e = 0x3f;
            }
            else
            {
                e = a;
            }

            //set_fmvol_ks:
            a = work.ch[ix].ol[i];//.ar_d1r;
            a &= 0xc0;
            a |= e;
        }

        //********************************************
        // set ADSR regs
        // in   : work
        // dest : AF,DE
        private void moon_set_adsr()
        {
            e = work.ch[ix].lfo_vib;
            d = 0x80;
            moon_add_reg_ch();
            moon_wave_out();

            e = work.ch[ix].ol[0];//.ar_d1r;
            d = 0x98;
            moon_add_reg_ch();
            moon_wave_out();

            e = work.ch[ix].ol[1];//.dl_d2r;
            d = 0xB0;
            moon_add_reg_ch();
            moon_wave_out();

            e = work.ch[ix].ol[2];//.rc_rr;
            d = 0xc8;
            moon_add_reg_ch();
            moon_wave_out();

            e = work.ch[ix].ol[3];//.am;
            d = 0xe0;
            moon_add_reg_ch();
            moon_wave_out();
        }

        //********************************************
        // moon_key_data
        // make data for key-on/off
        // in   : work
        // out  : E = data for key
        // dest : almost all
        private void moon_key_data()
        {
            a = work.ch[ix].pan;
            a &= 0xf;

            //; ; or  $10; PCM - MIX
            e = a;
            a = work.ch[ix].lfo;

            if (a == 0)
            {
                e |= 0x20;// LFO deactive
            }

            //moon_key_data_lfo_on:
            a = work.ch[ix].damp;

            if (a != 0)
            {
                e |= 0x40;// Damp on
            }
            //moon_key_data_damp_off:
        }

        //********************************************
        // moon_key_off
        // this function does : key-off
        // in   : work
        // dest : almost all
        private void moon_key_off()
        {

            // key off
            a = work.ch[ix].dsel;
            if (a != 0)
            {
                //moon_key_fmoff:
                a = work.ch[ix].key;
                a &= 0x20;
                if (a == 0) return;
                a = work.ch[ix].key;
                a &= 0xdf;
                e = a;
                d = 0xb0;
                work.ch[ix].key = e;
                // skip if jump flag is true
                a = work.seq_jump_flag;
                if (a != 0) return;
                moon_write_fmreg();// key - off
                return;
            }

            //moon_key_opl4off:
            a = work.ch[ix].key;
            a &= 0x80;
            if (a == 0) return;
            a = work.ch[ix].key;
            a &= 0x7f;
            e = a;
            d = 0x68;
            work.ch[ix].key = e;
            moon_add_reg_ch();
            moon_wave_out();// key - off
            pcmKeyon[ix] = -1;
        }

        //********************************************
        // moon_write_fmpan
        // calc and write Reg $Cx
        // dest AF, DE
        private void moon_write_fmpan()
        {
            a = work.ch[ix].synth;
            d = a;
            a &= 0x1c; // 000FFF00
            a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0)); //rrca
            e = a;
            a = d;
            a &= 0x01; // SynthType
            a |= e;
            a |= work.ch[ix].pan;

            // E -> $C0
            e = a;
            d = 0xc0;
            moon_write_fmreg();

            // 4OP
            a = work.ch[ix].synth;
            d = a;
            a &= 0x80;

            // skip if not 4op
            if (a != 0)
            {
                a = d;
                a = (byte)((a >> 1) + ((a & 1) != 0 ? 0x80 : 0)); //rrca
                a &= 0x01; // 2nd SynthType

                a |= work.ch[ix].pan;
                e = a;
                d = 0xc0;

                // E -> $C0 + 3 + ch
                a = work.seq_cur_ch;
                a += 0x03;
                moon_write_fmreg_nch();
                return;
            }

            //moon_write_fmpan_4op_fin:
        }

        //********************************************
        // moon_key_on
        // Set tone number, frequency and key-on
        // in   : work
        // dest : almost all
        //
        private void moon_key_on()
        {
            a = work.ch[ix].dsel;
            if (a == 0)
            {
                moon_key_opl4on();
                return;
            }

            //moon_key_fmon:
            if ((work.ch[ix].efx1 & 1) == 0)
            {
                moon_key_off();
                start_venv();
                start_penv();
                start_nenv();
            }

            //slar_fm_on:
            work.ch[ix].efx1 &= 0xfe;
            moon_key_write_fmfreq();
            a |= 0x20;//  key on

            e = a;
            d = 0xb0;
            work.ch[ix].key = e;

            // skip if jump flag is true
            a = work.seq_jump_flag;
            if (a != 0) return;

            moon_write_fmreg();// key-on
        }

        private void moon_key_opl4on()
        {
            if ((work.ch[ix].efx1 & 1) != 0)
            {
                //slar_opl4_on:
                work.ch[ix].efx1 &= 0xfe;
                pcmKeyon[ix] = work.ch[ix].note + 12 * 2;
                moon_set_freq_ch();
                return;
            }

            moon_key_off();
            start_venv();
            start_penv();
            start_nenv();

            // tone number(hi)
            a = (byte)(work.ch[ix].tone >> 8);
            a &= 0x1;
            e = a;
            d = 0x20;

            moon_add_reg_ch();
            moon_wave_out();

            // tone number(lo)
            e = (byte)(work.ch[ix].tone & 0xff);
            d = 0x08;

            moon_add_reg_ch();
            moon_wave_out();

            //moon_wavechg_lp:
            do
            {
                a = inport(MOON_STAT);
                a &= 0x02;
            } while (a != 0);

            pcmKeyon[ix] = work.ch[ix].note + 12 * 2;
            moon_set_freq_ch();
            moon_set_vol_ch();
            moon_set_adsr();

            //moon_opl4_set_keyreg:
            // OPL4 key-on
            // skip if jump flag is true
            a = work.seq_jump_flag;
            if (a != 0) return;

            moon_key_data();

            a = e;
            a |= 0x80; // key-on
            e = a;
            d = 0x68;
            work.ch[ix].key = e;

            moon_add_reg_ch();
            moon_wave_out();// key-on
            return;

        }
        
        //********************************************
        // moon_key_write_fmfreq
        // calculates and writes related FM frequency
        // out : A = Reg.$B0(FnumH + BLK )
        // dest : almost all
        private void moon_key_write_fmfreq()
        {
            a = work.seq_cur_ch;

            work.seq_tmp_ch = a;

            a = work.ch[ix].oct;
            work.seq_tmp_oct = a;

            hl = work.ch[ix].fnum;
            work.seq_tmp_fnum = hl;

            moon_key_write_fmfreq_base();
        }

        private void moon_key_write_fmfreq_base()
        {
            a = (byte)work.seq_tmp_fnum;
            e = a;
            d = 0xa0;
            a = work.seq_tmp_ch;
            moon_write_fmreg_nch();

            a = (byte)(work.seq_tmp_fnum >> 8);
            a &= 0x03;
            e = a;

            a = work.seq_tmp_oct;

            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));
            a = (byte)((a << 1) + ((a & 0x80) != 0 ? 1 : 0));

            a &= 0x1c; // mask for Octave
            a |= e; // F-Number
        }

        //********************************************
        // moon_key_fmfreq
        // calculates and writes related regs with keeping key
        private void moon_key_fmfreq()
        {
            moon_key_write_fmfreq();

            e = a;
            a = work.ch[ix].key;
            a &= 0x20;
            a |= e;

            work.ch[ix].key = a;
            e = a;
            d = 0xb0;

            // skip if jump flag is true

            a = work.seq_jump_flag;

            if (a != 0) return;

            moon_write_fmreg();// key-on
        }












        //********************************************
        //  load pcm
        private const UInt16 MDB_LDFLAG = 0;// MDB_BASE;
        private const UInt16 MDB_ADRHI = 1;// MDB_BASE + 1;
        private const UInt16 MDB_ADRMI = 2;// MDB_BASE + 2;
        private const UInt16 MDB_ADRLO = 3;// MDB_BASE + 3;
        private const UInt16 MDB_RESULT = 4;// MDB_BASE + 4;
        private const UInt16 MDB_ROM = 5;// MDB_BASE + 5;

        // reset R/W address pointer
        private void moon_reset_sram_adrs()
        {

            a = ReadMemory(MDR_DSTPCM);

            MDB_BASE[MDB_ADRHI] = a;
            a = 0;
            MDB_BASE[MDB_ADRMI] = a;
            MDB_BASE[MDB_ADRLO] = a;
            moon_set_sram_adrs();
        }

        // incliments address pointer
        private void moon_inc_sram_adrs()
        {
            a = MDB_BASE[MDB_ADRLO];
            a++;
            MDB_BASE[MDB_ADRLO] = a;
            if (a != 0) return;

            a = MDB_BASE[MDB_ADRMI];
            a++;
            MDB_BASE[MDB_ADRMI] = a;
            if (a != 0) return;

            a = MDB_BASE[MDB_ADRHI];
            a++;
            MDB_BASE[MDB_ADRHI] = a;
        }

        // set SRAM address
        private void moon_set_sram_adrs()
        {
            a = MDB_BASE[MDB_ADRHI];
            e = a;
            d = 0x03;
            moon_wave_out();

            a = MDB_BASE[MDB_ADRMI];
            e = a;
            d = 0x04;
            moon_wave_out();

            // the last should be lowest to set chip's internal pointer.
            // (trigger to set)

            a = MDB_BASE[MDB_ADRLO];
            e = a;
            d = 0x05;
            moon_wave_out();
        }

        // check ROM
        private void moon_check_rom()
        {
            a = 0;

            MDB_BASE[MDB_ADRHI] = a;
            MDB_BASE[MDB_ADRLO] = a;
            a = 0x12;
            MDB_BASE[MDB_ADRMI] = a;

            // A<- (001200h)
            moon_set_sram_adrs();

            b = 0x08;

            //string str_romchk = "Copyright";
            hl = 0;

            // check loop
            //moon_check_rom_lp:

            //スキップ
            //do
            //{
            //    a = (byte)str_romchk[hl];
            //    e = a;

            //    // A<- (SRAM)
            //    d = 0x06;
            //    moon_wave_in();

            //    MDB_BASE[MDB_ROM] = a;
            //    if (a - e != 0) return;
            //    hl++;
            //    b--;
            //} while (b > 0);// djnz moon_check_rom_lp
            a = 0;
        }

        // check SRAM
        private bool moon_check_sram()
        {
            //スキップ
            return false;
            //// $77-> ($200000)
            //moon_reset_sram_adrs();

            //d = 0x06;
            //e = 0x77;
            //moon_wave_out();

            //// ok if $77 < -($200000)
            //moon_set_sram_adrs();

            //d = 0x06;
            //moon_wave_in();

            //if (!CP_ZF(0x77)) return true;

            //// $88-> ($200000)
            //moon_set_sram_adrs();

            //d = 0x06;
            //e = 0x88;
            //moon_wave_out();

            //// ok if $88 < -($200000)
            //moon_set_sram_adrs();

            //d = 0x06;
            //moon_wave_in();

            //if (!CP_ZF(0x88)) return true;

            //// $99-> ($200001)
            //a = 0x01;

            //MDB_BASE[MDB_ADRLO] = a;
            //moon_set_sram_adrs();

            //d = 0x06;
            //e = 0x99;
            //moon_wave_out();

            //// ok if $99 < -($200001)
            //moon_set_sram_adrs();

            //d = 0x06;
            //moon_wave_in();

            //if (!CP_ZF(0x99)) return true;

            //// ok if $88 < -($200000)
            //a = 0;

            //MDB_BASE[MDB_ADRLO] = a;
            //moon_set_sram_adrs();

            //d = 0x06;
            //moon_wave_in();

            //if (!CP_ZF(0x88)) return true;
            //return false;
        }


        // Load User PCM
        private void moon_load_pcm()
        {
            a = 0;
            change_page3();

            // is PCM packed song file?
            a = ReadMemory(MDR_PACKED);
            // output status for debug
            MDB_BASE[MDB_LDFLAG] = a;

            if (a == 0) return;

            // initialize to enable OPL4 function
            moon_init();

            // memory write mode
            d = 0x02;
            e = 0x11;

            moon_wave_out();

            // check ROM
            moon_check_rom();

            if (a == 0)
            {
                check_sram_start();
                return;
            }

            // failed to check ROM

            a = 0x03;

            MDB_BASE[MDB_LDFLAG] = a;
            // address reset

            moon_reset_sram_adrs();
        }

        private void check_sram_start()
        {
            // check sram

            bool flg = moon_check_sram();
            if (!flg)
            {
                sram_found();
                return;
            }

            // result
            MDB_BASE[MDB_RESULT] = a;

            // SRAM is not found
            a = 0x02;
            MDB_BASE[MDB_LDFLAG] = a;

        }

        private void sram_found()
        {
            byte moon_pcm_bank_count = 0;

            byte moon_pcm_bank = 0;

            byte moon_pcm_numbanks = 0;

            byte moon_pcm_lastsize = 0;

            // reset SRAM address
            moon_reset_sram_adrs();

            // PCM number of banks
            a = ReadMemory(MDR_PCMBANKS);

            moon_pcm_numbanks = a;
            moon_pcm_bank_count = a;

            // size of lastbank
            a = ReadMemory(MDR_LASTS);
            moon_pcm_lastsize = a;

            // size of start page
            a = ReadMemory(MDR_STPCM);
            moon_pcm_bank = a;

            // start of source address
            hl = 0x8000;

            // address = $A000 if (start_bank & 1) != 0
            a &= 1;
            if (a != 0)
            {
                // bank1 = $A000
                hl = (UInt16)(0xA000 + (hl & 0xff));
            }

            // RAM to PCM
            //pcm_copy_bank:
            do
            {
                // bank size = $2000
                b = 0x20;
                c = 0x00;

                // Change to user pcm bank
                a = moon_pcm_bank;
                byte a1 = a;
                change_page3();
                a = a1;

                a++;
                moon_pcm_bank = a;

                // reset address if HL >= $C000
                a = (byte)(hl >> 8);
                if (!CP_CF(0xc0))
                {
                    // reset source address
                    hl = 0x8000;
                }

                // is the bank last?
                //pcm_chk_last:

                a = moon_pcm_bank_count;
                if (a == 0)
                {
                    // use lastsize if the bank is last one.
                    a = moon_pcm_lastsize;
                    b = a;
                }

                //pcm_copy_lp:
                do
                {
                    a = ReadMemory(hl);
                    e = a;
                    d = 0x06;

                    // A -> (PCM SRAM)
                    moon_wave_out();

                    hl++;
                    UInt16 bc = (UInt16)((b * 0x100 + c) - 1);
                    b = (byte)(bc >> 8);
                    c = (byte)(bc & 0xff);

                    // loop if BC > 0
                    a = b;
                    a |= c;
                } while (a != 0);

                // end if count is 0
                a = moon_pcm_bank_count;
                if (a == 0)
                {
                    break;
                    //    jr z, pcm_copy_end
                }

                a--;
                moon_pcm_bank_count = a;

            } while (true);

            // end of PCM copy
            //pcm_copy_end:

            // normal mode

            d = 0x02;
            e = 0x10;
            moon_wave_out();

        }




        private byte inport(UInt16 adr)
        {
            return 0;
        }

        private void outport(byte adr, byte data)
        {
            if (adr == 0xfe)
            {
                seg0x8000 = data;
            }
        }

        private byte[] mem = new byte[1024 * 64];
        private byte[][] extMem = new byte[256][];
        private byte? seg0x0000 = null;
        private byte? seg0x4000 = null;
        private byte? seg0x8000 = null;
        private byte? seg0xc000 = null;
        private byte a = 0;
        private byte b = 0;
        private byte c = 0;
        private byte d = 0;
        private byte e = 0;
        private UInt16 hl = 0;
        private UInt16 ix = 0;
        private UInt16 iy = 0;
        private delegate void dlgSeqFunc();
        private dlgSeqFunc[] seq_jmptable = null;

        private byte ReadMemory(UInt16 adr)
        {
            switch (adr >> 14)
            {
                case 0://0x0000 - 0x3fff
                default:
                    if (seg0x0000 == null)
                        return mem[adr];
                    if (extMem[(byte)seg0x0000] == null)
                        extMem[(byte)seg0x0000] = new byte[1024 * 16];
                    return extMem[(byte)seg0x0000][adr & 0x3fff];
                case 1://0x4000 - 0x7fff
                    if (seg0x4000 == null)
                        return mem[adr];
                    if (extMem[(byte)seg0x4000] == null)
                        extMem[(byte)seg0x4000] = new byte[1024 * 16];
                    return extMem[(byte)seg0x4000][adr & 0x3fff];
                case 2://0x8000 - 0xbfff
                    if (seg0x8000 == null || seg0x8000==0)
                        return mem[adr];
                    if (extMem[(byte)seg0x8000] == null)
                        extMem[(byte)seg0x8000] = new byte[1024 * 16];
                    return extMem[(byte)seg0x8000][adr & 0x3fff];
                case 3://0xc000 - 0xffff
                    if (seg0xc000 == null)
                        return mem[adr];
                    if (extMem[(byte)seg0xc000] == null)
                        extMem[(byte)seg0xc000] = new byte[1024 * 16];
                    return extMem[(byte)seg0xc000][adr & 0x3fff];
            }
        }

        private void WriteMemory(UInt16 adr,byte dat)
        {
            switch (adr >> 14)
            {
                case 0://0x0000 - 0x3fff
                default:
                    if (seg0x0000 == null || seg0x0000 == 0)
                    {
                        mem[adr] = dat;
                    }
                    else
                    {
                        if (extMem[(byte)seg0x0000] == null)
                            extMem[(byte)seg0x0000] = new byte[1024 * 16];
                        extMem[(byte)seg0x0000][adr & 0x3fff] = dat;
                    }
                    break;
                case 1://0x4000 - 0x7fff
                    if (seg0x4000 == null || seg0x4000 == 0)
                    {
                        mem[adr] = dat;
                    }
                    else
                    {
                        if (extMem[(byte)seg0x4000] == null)
                            extMem[(byte)seg0x4000] = new byte[1024 * 16];
                        extMem[(byte)seg0x4000][adr & 0x3fff] = dat;
                    }
                    break;
                case 2://0x8000 - 0xbfff
                    if (seg0x8000 == null || seg0x8000 == 0)
                    {
                        mem[adr] = dat;
                    }
                    else
                    {
                        if (extMem[(byte)seg0x8000] == null)
                            extMem[(byte)seg0x8000] = new byte[1024 * 16];
                        extMem[(byte)seg0x8000][adr & 0x3fff] = dat;
                    }
                    break;
                case 3://0xc000 - 0xffff
                    if (seg0xc000 == null || seg0xc000 == 0)
                    {
                        mem[adr] = dat;
                    }
                    else
                    {
                        if (extMem[(byte)seg0xc000] == null)
                            extMem[(byte)seg0xc000] = new byte[1024 * 16];
                        extMem[(byte)seg0xc000][adr & 0x3fff]=dat;
                    }
                    break;
            }
        }

        private bool CP_CF(byte n)
        {
            int ans = (int)a - (int)n;
            if (ans < 0) return true;
            return false;
        }

        private bool CP_ZF(byte n)
        {
            int ans = (int)a - (int)n;
            if (ans == 0) return true;
            return false;
        }


    }
}
