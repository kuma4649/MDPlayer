using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Midi;
using MDSound.np;
using MDSound.np.memory;
using MDSound.np.cpu;
using MDSound.np.chip;
using NScci;
using SoundManager;

namespace MDPlayer
{
    public class ChipRegister
    {

        private MDSound.MDSound mds = null;
        private midiOutInfo[] midiOutInfos = null;
        private List<NAudio.Midi.MidiOut> midiOuts = null;
        private List<int> midiOutsType = null;
        private List<vstInfo2> vstMidiOuts = null;
        private List<int> vstMidiOutsType = null;


        private Dictionary<MDSound.MDSound.enmInstrumentType, MDSound.MDSound.Chip> dicChipsInfo = new Dictionary<MDSound.MDSound.enmInstrumentType, MDSound.MDSound.Chip>();

        private Setting.ChipType[] ctSN76489 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2612 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2608 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2151 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2203 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2610 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMF262 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMF271 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMF278B = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMZ280B = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctAY8910 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2413 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctHuC6280 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM3526 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctY8950 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctSEGAPCM = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctC140 = new Setting.ChipType[2] { null, null };

        private RealChip realChip = null;
        private RSoundChip[] scAY8910 = new RSoundChip[2] { null, null };
        private RSoundChip[] scSEGAPCM = new RSoundChip[2] { null, null };
        private RSoundChip[] scC140 = new RSoundChip[2] { null, null };
        private RSoundChip[] scSN76489 =  new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2151 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2203 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2413 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2612 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2608 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610EA = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610EB = new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF262 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF271 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF278B =  new RSoundChip[2] { null, null };
        private RSoundChip[] scYMZ280B =  new RSoundChip[2] { null, null };

        /// <summary>
        /// AlgMask (Alg:4はop2 op3が逆に並んでいます)
        /// </summary>
        private byte[] algM = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x0c, 0x0e, 0x0e, 0x0f };
        private int[] opN = new int[] { 0, 2, 1, 3 };
        private int[] noteTbl = new int[] { 2, 4, 5, -1, 6, 8, 9, -1, 10, 12, 13, -1, 14, 0, 1, -1 };
        private int[] noteTbl2 = new int[] { 13, 14, 0, -1, 1, 2, 4, -1, 5, 6, 8, -1, 9, 10, 12, -1 };

        private int nsfAPUmask = 0;
        private int nsfDMCmask = 0;
        private int nsfFDSmask = 0;
        private int nsfMMC5mask = 0;
        private int nsfVRC7mask = 0;

        public ChipLEDs chipLED = new ChipLEDs();
        public Enq enq;

        public int[][] YM2151FmRegister = new int[][] { null, null };
        public int[][] YM2151FmKeyOn = new int[][] { null, null };
        public int[][] YM2151FmVol = new int[][] {
            new int[8] { 0,0,0,0,0,0,0,0 }
            , new int[8] { 0,0,0,0,0,0,0,0 }
        };
        private int[] YM2151NowFadeoutVol = new int[] { 0, 0 };
        private bool[][] YM2151MaskFMCh = new bool[][] {
            new bool[8] { false, false, false, false, false, false, false, false }
            ,new bool[8] { false, false, false, false, false, false, false, false }
        };
        public int[] YM2151FmAMD = new int[] { -1, -1 };
        public int[] YM2151FmPMD = new int[] { -1, -1 };

        public int[][] fmRegisterYM2203 = new int[][] { null, null };
        public int[][] fmKeyOnYM2203 = new int[][] { null, null };
        public int[][] fmCh3SlotVolYM2203 = new int[][] { new int[4], new int[4] };
        private int[] nowYM2203FadeoutVol = new int[] { 0, 0 };
        public int[][] fmVolYM2203 = new int[][] { new int[9], new int[9] };
        private bool[][] maskFMChYM2203 = new bool[][] {
            new bool[9] { false, false, false, false, false, false, false, false , false }
            ,new bool[9] { false, false, false, false, false, false, false, false , false }
        };

        public int[][] fmRegisterYM2413 = new int[][] { null, null };
        //private int[] fmRegisterYM2413RyhthmB = new int[2] { 0, 0 };
        //private int[] fmRegisterYM2413Ryhthm = new int[2] { 0, 0 };
        private ChipKeyInfo[] kiYM2413 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiYM2413ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private bool[][] maskFMChYM2413 = new bool[][] {
            new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
            , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
        };
        private int[] nowYM2413FadeoutVol = new int[] { 0, 0 };

        public int[][][] fmRegisterYM2612 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        public int[][] fmKeyOnYM2612 = new int[][] { null, null };
        public int[][] fmVolYM2612 = new int[][] {
            new int[9] { 0,0,0,0,0,0,0,0,0 }
            ,new int[9] { 0,0,0,0,0,0,0,0,0 }
        };
        public int[][] fmCh3SlotVolYM2612 = new int[][] { new int[4], new int[4] };
        private int[] nowYM2612FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2612 = new bool[][] { new bool[6] { false, false, false, false, false, false }, new bool[6] { false, false, false, false, false, false } };

        public int[][][] fmRegisterYM2608 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        public int[][] fmKeyOnYM2608 = new int[][] { null, null };
        public int[][] fmVolYM2608 = new int[][] {
            new int[9] { 0,0,0,0,0,0,0,0,0 }
            ,new int[9] { 0,0,0,0,0,0,0,0,0 }
        };
        public int[][] fmCh3SlotVolYM2608 = new int[][] { new int[4], new int[4] };
        public int[][][] fmVolYM2608Rhythm = new int[][][] {
            new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
            , new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
        };
        public int[][] fmVolYM2608Adpcm = new int[][] { new int[2], new int[2] };
        public int[] fmVolYM2608AdpcmPan = new int[] { 0, 0 };

        private int[] nowYM2608FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2608 = new bool[][] {
            new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
            , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public int[][][] fmRegisterYM2610 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        public int[][] fmKeyOnYM2610 = new int[][] { null, null };
        public int[][] fmVolYM2610 = new int[][] {
            new int[9] { 0,0,0,0,0,0,0,0,0 }
            ,new int[9] { 0,0,0,0,0,0,0,0,0 }
        };
        public int[][] fmCh3SlotVolYM2610 = new int[][] { new int[4], new int[4] };
        public int[][][] fmVolYM2610Rhythm = new int[][][] {
            new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
            , new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
        };
        public int[][] fmVolYM2610Adpcm = new int[][] { new int[2], new int[2] };
        public int[] fmVolYM2610AdpcmPan = new int[] { 0, 0 };
        private int[] nowYM2610FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2610 = new bool[][] {
            new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false }
            , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false }
        };

        public int[][] fmRegisterYM3526 = new int[][] { null, null };
        private ChipKeyInfo[] kiYM3526 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiYM3526ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private bool[][] maskFMChYM3526 = new bool[2][] {
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            },
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            }
        };

        public int[][] fmRegisterYM3812 = new int[][] { null, null };
        private ChipKeyInfo[] kiYM3812 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiYM3812ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private bool[][] maskFMChYM3812 = new bool[2][] {
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            },
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            }
        };

        private ChipKeyInfo[] kiVRC7 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiVRC7ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };

        public int[][][] fmRegisterYMF262 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        private int[] fmRegisterYMF262FM = new int[2] { 0, 0 };
        private int[] fmRegisterYMF262RyhthmB = new int[2] { 0, 0 };
        private int[] fmRegisterYMF262Ryhthm = new int[2] { 0, 0 };
        private bool[][] maskFMChYMF262 = new bool[][] {
            new bool[23] {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,
                false, false, false
                }
            , new bool[23] {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,
                false, false, false
                }
        };

        public int[][][] fmRegisterYMF271 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };

        public int[][][] fmRegisterYMF278B = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        private int[] fmRegisterYMF278BFM = new int[2] { 0, 0 };
        private int[][] fmRegisterYMF278BPCM = new int[2][] { new int[24], new int[24] };
        private int[] fmRegisterYMF278BRyhthmB = new int[2] { 0, 0 };
        private int[] fmRegisterYMF278BRyhthm = new int[2] { 0, 0 };
        private bool[][] maskFMChYMF278B = new bool[][] {
            new bool[47] {
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false  }
            , new bool[47] {
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false  }
        };
        private byte[] YMF278BCh = new byte[]
        {
                0,3,1,4,2,5,6,7,8,9,12,10,13,11,14,15,16,17,
                18,19,20,21,22,
                23,24,25,26,27,28, 29,30,31,32,33,34,
                35,36,37,38,39,40, 41,42,43,44,45,46
        };

        public int[][] YMZ280BRegister = new int[][] { null, null };

        public int[][] fmRegisterY8950 = new int[][] { null, null };
        private ChipKeyInfo[] kiY8950 = new ChipKeyInfo[2] { new ChipKeyInfo(15), new ChipKeyInfo(15) };
        private ChipKeyInfo[] kiY8950ret = new ChipKeyInfo[2] { new ChipKeyInfo(15), new ChipKeyInfo(15) };
        private bool[][] maskFMChY8950 = new bool[2][] {
            new bool[9 + 5+1]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
                , false
            },
            new bool[9 + 5+1]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
                , false
            }
        };

        public int[][] SN76489Register = new int[][] { null, null };
        public int[] SN76489RegisterGGPan = new int[] { 0xff, 0xff };
        public int[][][] SN76489Vol = new int[][][] {
            new int[4][] { new int[2], new int[2], new int[2], new int[2] }
            ,new int[4][] { new int[2], new int[2], new int[2], new int[2] }
        };
        public int[] SN76489NowFadeoutVol = new int[] { 0, 0 };
        public bool[][] SN76489MaskCh = new bool[][] {
            new bool[4] {false,false,false,false }
            ,new bool[4] {false,false,false,false }
        };

        private int[] AY8910ChFreq = new int[] { 0, 0, 0 };
        public int[][] AY8910PsgRegister = new int[][] { null, null };
        public int[][] AY8910PsgKeyOn = new int[][] { null, null };
        private int[] AY8910NowFadeoutVol = new int[] { 0, 0 };
        public int[][] AY8910PsgVol = new int[][] { new int[3], new int[3] };
        private bool[][] AY8910MaskPSGCh = new bool[][] {
            new bool[3] { false, false, false }
            ,new bool[3] { false, false, false }
        };

        private int[] C140NowFadeoutVol = new int[] { 0, 0 };
        private int[] SEGAPCMNowFadeoutVol = new int[] { 0, 0 };

        private bool[] maskOKIM6258 = new bool[2] { false, false };
        public bool[] okim6258Keyon = new bool[2] { false, false };

        public byte[][] pcmRegisterC140 = new byte[2][] { null, null };
        public bool[][] pcmKeyOnC140 = new bool[2][] { null, null };

        public ushort[][] pcmRegisterC352 = new ushort[2][] { null, null };
        public ushort[][] pcmKeyOnC352 = new ushort[2][] { null, null };
        private bool[][] maskChC352 = new bool[][] {
            new bool[32] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false
            }
            ,new bool[32] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false
            }
        };

        public byte[] K051649tKeyOnOff = new byte[] { 0, 0 };
        public bool[][] maskChK051649 = new bool[][]
        {
            new bool[]{ false, false, false, false, false},
            new bool[]{ false, false, false, false, false}
        };

        public byte[][] pcmRegisterSEGAPCM = new byte[2][] { null, null };
        public bool[][] pcmKeyOnSEGAPCM = new bool[2][] { null, null };

        public MIDIParam[] midiParams = new MIDIParam[] { null, null };

        public nes_bank nes_bank = null;
        public nes_mem nes_mem = null;
        public km6502 nes_cpu = null;
        public nes_apu nes_apu = null;
        public nes_dmc nes_dmc = null;
        public nes_fds nes_fds = null;
        public nes_n106 nes_n106 = null;
        public nes_vrc6 nes_vrc6 = null;
        public nes_mmc5 nes_mmc5 = null;
        public nes_fme7 nes_fme7 = null;
        public nes_vrc7 nes_vrc7 = null;


        private int[] LatchedRegister = new int[] { 0, 0 };
        private int[] NoiseFreq = new int[] { 0, 0 };

        /// <summary>
        /// ワーク用Chip(Chipはインスタンスの中身だけやりとりされる)
        /// </summary>
        private Chip dummyChip = new Chip();

        private int volF = 1;
        private MIDIExport midiExport = null;

        int[] algVolTbl = new int[8] { 8, 8, 8, 8, 0xa, 0xe, 0xe, 0xf };



        public Chip[] AY8910 = new Chip[] { new Chip(), new Chip() };
        public Chip[] C140 = new Chip[] { new Chip(), new Chip() };
        public Chip MIDI = new Chip();
        public Chip[] SEGAPCM = new Chip[] { new Chip(), new Chip() };
        public Chip[] SN76489 = new Chip[] { new Chip(), new Chip() };
        public Chip[] YM2151 = new Chip[] { new Chip(), new Chip() };
        public Chip[] YM2203 = new Chip[] { new Chip(), new Chip() };
        public Chip[] YM2413 = new Chip[] { new Chip(), new Chip() };
        public Chip[] YM2608 = new Chip[] { new Chip(), new Chip() };
        public Chip[] YM2610 = new Chip[] { new Chip(), new Chip() };
        public Chip[] YM2612 = new Chip[] { new Chip(), new Chip() };



        public ChipRegister(Setting setting, MDSound.MDSound mds, RealChip nScci)
        {
            ClearChipParam();

            this.mds = mds;
            this.realChip = nScci;
            initChipRegister(null);

            midiExport = new MIDIExport(setting);
            midiExport.fmRegisterYM2612 = fmRegisterYM2612;
            midiExport.fmRegisterYM2151 = YM2151FmRegister;
        }

        public void SetRealChipInfo(EnmDevice dev, Setting.ChipType chipTypeP, Setting.ChipType chipTypeS, int LatencyEmulation,int LatencyReal)
        {
            int LEmu = SoundManager.SoundManager.DATA_SEQUENCE_FREQUENCE * LatencyEmulation / 1000;
            int LReal = SoundManager.SoundManager.DATA_SEQUENCE_FREQUENCE * LatencyReal / 1000;

            switch (dev)
            {
                case EnmDevice.AY8910:
                    ctAY8910 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scAY8910[i] = realChip.GetRealChip(ctAY8910[i]);
                        if (scAY8910[i] != null) scAY8910[i].init();
                        AY8910[i].Model = ctAY8910[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        AY8910[i].Delay = (AY8910[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.C140:
                    ctC140 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scC140[i] = realChip.GetRealChip(ctC140[i]);
                        if (scC140[i] != null) scC140[i].init();
                        C140[i].Model = ctC140[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        C140[i].Delay = (C140[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.SegaPCM:
                    ctSEGAPCM = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scSEGAPCM[i] = realChip.GetRealChip(ctSEGAPCM[i]);
                        if (scSEGAPCM[i] != null) scSEGAPCM[i].init();
                        SEGAPCM[i].Model = ctSEGAPCM[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        SEGAPCM[i].Delay = (SEGAPCM[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.SN76489:
                    ctSN76489 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scSN76489[i] = realChip.GetRealChip(ctSN76489[i]);
                        if (scSN76489[i] != null) scSN76489[i].init();
                        SN76489[i].Model = ctSN76489[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        SN76489[i].Delay = (SN76489[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.YM2151:
                    ctYM2151 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scYM2151[i] = realChip.GetRealChip(ctYM2151[i]);
                        if (scYM2151[i] != null) scYM2151[i].init();
                        YM2151[i].Model = (ctYM2151[i].UseEmu || ctYM2151[i].UseEmu2 || ctYM2151[i].UseEmu3) ? EnmModel.VirtualModel : EnmModel.RealModel;
                        YM2151[i].Delay = (YM2151[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.YM2203:
                    ctYM2203 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scYM2203[i] = realChip.GetRealChip(ctYM2203[i]);
                        if (scYM2203[i] != null) scYM2203[i].init();
                        YM2203[i].Model = ctYM2203[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        YM2203[i].Delay = (YM2203[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.YM2413:
                    ctYM2413 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scYM2413[i] = realChip.GetRealChip(ctYM2413[i]);
                        if (scYM2413[i] != null) scYM2413[i].init();
                        YM2413[i].Model = ctYM2413[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        YM2413[i].Delay = (YM2413[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.YM2608:
                    ctYM2608 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scYM2608[i] = realChip.GetRealChip(ctYM2608[i]);
                        if (scYM2608[i] != null) scYM2608[i].init();
                        YM2608[i].Model = ctYM2608[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        YM2608[i].Delay = (YM2608[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.YM2610:
                    ctYM2610 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scYM2610[i] = realChip.GetRealChip(ctYM2610[i]);
                        if (scYM2610[i] != null) scYM2610[i].init();
                        scYM2610EA[i] = realChip.GetRealChip(ctYM2610[i], 1);
                        if (scYM2610EA[i] != null) scYM2610EA[i].init();
                        scYM2610EB[i] = realChip.GetRealChip(ctYM2610[i], 2);
                        if (scYM2610EB[i] != null) scYM2610EB[i].init();
                        YM2610[i].Model = ctYM2610[i].UseEmu ? EnmModel.VirtualModel : EnmModel.RealModel;
                        YM2610[i].Delay = (YM2610[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmDevice.YM2612:
                    ctYM2612 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < 2; i++)
                    {
                        scYM2612[i] = realChip.GetRealChip(ctYM2612[i]);
                        if (scYM2612[i] != null) scYM2612[i].init();
                        YM2612[i].Model = ctYM2612[i].UseScci ? EnmModel.RealModel : EnmModel.VirtualModel;
                        YM2612[i].Delay = (YM2612[i].Model == EnmModel.VirtualModel ? LEmu : LReal);
                    }
                    break;

                case EnmDevice.HuC6280:
                    ctHuC6280 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmDevice.Y8950:
                    ctY8950 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmDevice.YM3526:
                    ctYM3526 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmDevice.YMF262:
                    ctYMF262 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmDevice.YMF271:
                    ctYMF271 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmDevice.YMF278B:
                    ctYMF278B = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmDevice.YMZ280B:
                    ctYMZ280B = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
            }

        }

        public void ClearChipParam()
        {
            for (int i = 0; i < 2; i++)
            {
                AY8910[i].Use = false;
                AY8910[i].Model = EnmModel.None;
                AY8910[i].Device = EnmDevice.AY8910;
                AY8910[i].Number = i;
                AY8910[i].Hosei = 0;

                C140[i].Use = false;
                C140[i].Model = EnmModel.None;
                C140[i].Device = EnmDevice.C140;
                C140[i].Number = i;
                C140[i].Hosei = 0;

                SEGAPCM[i].Use = false;
                SEGAPCM[i].Model = EnmModel.None;
                SEGAPCM[i].Device = EnmDevice.SegaPCM;
                SEGAPCM[i].Number = i;
                SEGAPCM[i].Hosei = 0;

                YM2151[i].Use = false;
                YM2151[i].Model = EnmModel.None;
                YM2151[i].Device = EnmDevice.YM2151;
                YM2151[i].Number = i;
                YM2151[i].Hosei = 0;

                YM2203[i].Use = false;
                YM2203[i].Model = EnmModel.None;
                YM2203[i].Device = EnmDevice.YM2203;
                YM2203[i].Number = i;
                YM2203[i].Hosei = 0;

                YM2413[i].Use = false;
                YM2413[i].Model = EnmModel.None;
                YM2413[i].Device = EnmDevice.YM2413;
                YM2413[i].Number = i;
                YM2413[i].Hosei = 0;

                YM2608[i].Use = false;
                YM2608[i].Model = EnmModel.None;
                YM2608[i].Device = EnmDevice.YM2608;
                YM2608[i].Number = i;
                YM2608[i].Hosei = 0;

                YM2610[i].Use = false;
                YM2610[i].Model = EnmModel.None;
                YM2610[i].Device = EnmDevice.YM2610;
                YM2610[i].Number = i;
                YM2610[i].Hosei = 0;

                YM2612[i].Use = false;
                YM2612[i].Model = EnmModel.None;
                YM2612[i].Device = EnmDevice.YM2612;
                YM2612[i].Number = i;
                YM2612[i].Hosei = 0;

                SN76489[i].Use = false;
                SN76489[i].Model = EnmModel.None;
                SN76489[i].Device = EnmDevice.SN76489;
                SN76489[i].Number = i;
                SN76489[i].Hosei = 0;
            }

            MIDI.Use = false;
            MIDI.Model = EnmModel.None;
            MIDI.Device = EnmDevice.MIDIGM;
            MIDI.Number = 0;
            MIDI.Hosei = 0;
        }

        public bool ProcessingData(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            if (Type != EnmDataType.Normal) return true;

            switch (Chip.Device)
            {
                case EnmDevice.AY8910:
                    AY8910SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.C140:
                    C140SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.SegaPCM:
                    SEGAPCMSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.SN76489:
                    SN76489SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.YM2151:
                    YM2151SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.YM2203:
                    YM2203SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.YM2413:
                    YM2413SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.YM2608:
                    YM2608SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.YM2610:
                    YM2610SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmDevice.YM2612:
                    YM2612SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
            }

            return true;
        }

        public void SendChipData(long packCounter, Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            switch (Chip.Device)
            {
                case EnmDevice.AY8910:
                    AY8910WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.C140:
                    C140WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.MIDIGM:
                    MIDIWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.SegaPCM:
                    SEGAPCMWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.YM2151:
                    YM2151WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.YM2203:
                    YM2203WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.YM2413:
                    YM2413WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.YM2608:
                    YM2608WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.YM2610:
                    YM2610WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.YM2612:
                    YM2612WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmDevice.SN76489:
                    SN76489WriteRegisterControl(Chip, type, address, data, exData);
                    break;
            }
        }

        public void SetFadeoutVolume(long counter, double fadeoutCounter)
        {
            AY8910SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            C140SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 255.0));
            SEGAPCMSetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 255.0));
            SN76489SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            YM2151SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2203SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2413SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            YM2608SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2610SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2612SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
        }

        public void LoopCountUp(long Counter)
        {
            dummyChip.Model = EnmModel.None;
            enq(Counter, dummyChip, EnmDataType.Loop, -1, -1, null);
        }

        public void initChipRegister(MDSound.MDSound.Chip[] chipInfos)
        {

            dicChipsInfo.Clear();
            if (chipInfos != null)
            {
                foreach (MDSound.MDSound.Chip c in chipInfos)
                {
                    if (!dicChipsInfo.ContainsKey(c.type))
                    {
                        dicChipsInfo.Add(c.type, c);
                    }
                }
            }

            for (int chipID = 0; chipID < 2; chipID++)
            {

                fmRegisterYM2612[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2612[chipID][0][i] = 0;
                    fmRegisterYM2612[chipID][1][i] = 0;
                }
                fmRegisterYM2612[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2612[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2612[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2612[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2608[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2608[chipID][0][i] = 0;
                    fmRegisterYM2608[chipID][1][i] = 0;
                }
                fmRegisterYM2608[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2608[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2608[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2608[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2610[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2610[chipID][0][i] = 0;
                    fmRegisterYM2610[chipID][1][i] = 0;
                }
                fmRegisterYM2610[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2610[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2610[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2610[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM3526[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM3526[chipID][i] = 0;
                    fmRegisterYM3526[chipID][i] = 0;
                }

                fmRegisterYM3812[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM3812[chipID][i] = 0;
                    fmRegisterYM3812[chipID][i] = 0;
                }

                fmRegisterYMF262[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF262[chipID][0][i] = 0;
                    fmRegisterYMF262[chipID][1][i] = 0;
                }

                fmRegisterYMF271[chipID] = new int[7][] { new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF271[chipID][0][i] = 0;
                    fmRegisterYMF271[chipID][1][i] = 0;
                    fmRegisterYMF271[chipID][2][i] = 0;
                    fmRegisterYMF271[chipID][3][i] = 0;
                    fmRegisterYMF271[chipID][4][i] = 0;
                    fmRegisterYMF271[chipID][5][i] = 0;
                    fmRegisterYMF271[chipID][6][i] = 0;
                }

                fmRegisterYMF278B[chipID] = new int[3][] { new int[0x100], new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF278B[chipID][0][i] = 0;
                    fmRegisterYMF278B[chipID][1][i] = 0;
                    fmRegisterYMF278B[chipID][2][i] = 0;
                }
                fmRegisterYMF278BRyhthm[0] = 0;
                fmRegisterYMF278BRyhthm[1] = 0;
                fmRegisterYMF278BRyhthmB[0] = 0;
                fmRegisterYMF278BRyhthmB[1] = 0;

                fmRegisterY8950[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterY8950[chipID][i] = 0;
                }

                YMZ280BRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    YMZ280BRegister[chipID][i] = 0;
                }

                YM2151FmRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    YM2151FmRegister[chipID][i] = 0;
                }
                YM2151FmKeyOn[chipID] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2203[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2203[chipID][i] = 0;
                }
                fmKeyOnYM2203[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                SN76489Register[chipID] = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

                fmRegisterYM2413[chipID] = new int[0x39];
                for (int i = 0; i < 0x39; i++)
                {
                    fmRegisterYM2413[chipID][i] = 0;
                }
                //fmRegisterYM2413Ryhthm[0] = 0;
                //fmRegisterYM2413Ryhthm[1] = 0;
                //fmRegisterYM2413RyhthmB[0] = 0;
                //fmRegisterYM2413RyhthmB[1] = 0;

                AY8910PsgRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    AY8910PsgRegister[chipID][i] = 0;
                }
                AY8910PsgKeyOn[chipID] = new int[3] { 0, 0, 0 };

                pcmRegisterC140[chipID] = new byte[0x200];
                pcmKeyOnC140[chipID] = new bool[24];

                pcmRegisterC352[chipID] = new ushort[0x203];
                pcmKeyOnC352[chipID] = new ushort[32];

                pcmRegisterSEGAPCM[chipID] = new byte[0x200];
                pcmKeyOnSEGAPCM[chipID] = new bool[16];

                midiParams[chipID] = new MIDIParam();

                AY8910NowFadeoutVol[chipID] = 0;
                C140NowFadeoutVol[chipID] = 0;
                SEGAPCMNowFadeoutVol[chipID] = 0;
                SN76489NowFadeoutVol[chipID] = 0;
                YM2151NowFadeoutVol[chipID] = 0;
                nowYM2203FadeoutVol[chipID] = 0;
                nowYM2413FadeoutVol[chipID] = 0;
                nowYM2608FadeoutVol[chipID] = 0;
                nowYM2610FadeoutVol[chipID] = 0;
                nowYM2612FadeoutVol[chipID] = 0;

            }

            nes_bank = null;
            nes_mem = null;
            nes_cpu = null;
            nes_apu = null;
            nes_dmc = null;
            nes_fds = null;
            nes_n106 = null;
            nes_vrc6 = null;
            nes_mmc5 = null;
            nes_fme7 = null;
            nes_vrc7 = null;

        }

        public void initChipRegisterNSF(MDSound.MDSound.Chip[] chipInfos)
        {

            dicChipsInfo.Clear();
            if (chipInfos != null)
            {
                foreach (MDSound.MDSound.Chip c in chipInfos)
                {
                    dicChipsInfo.Add(c.type, c);
                }
            }

            for (int chipID = 0; chipID < 2; chipID++)
            {

                fmRegisterYM2612[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2612[chipID][0][i] = 0;
                    fmRegisterYM2612[chipID][1][i] = 0;
                }
                fmRegisterYM2612[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2612[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2612[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2612[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2608[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2608[chipID][0][i] = 0;
                    fmRegisterYM2608[chipID][1][i] = 0;
                }
                fmRegisterYM2608[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2608[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2608[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2608[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2610[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2610[chipID][0][i] = 0;
                    fmRegisterYM2610[chipID][1][i] = 0;
                }
                fmRegisterYM2610[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2610[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2610[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2610[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM3526[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM3526[chipID][i] = 0;
                    fmRegisterYM3526[chipID][i] = 0;
                }

                fmRegisterYM3812[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM3812[chipID][i] = 0;
                    fmRegisterYM3812[chipID][i] = 0;
                }

                fmRegisterYMF262[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF262[chipID][0][i] = 0;
                    fmRegisterYMF262[chipID][1][i] = 0;
                }

                fmRegisterYMF271[chipID] = new int[7][] { new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF271[chipID][0][i] = 0;
                    fmRegisterYMF271[chipID][1][i] = 0;
                    fmRegisterYMF271[chipID][2][i] = 0;
                    fmRegisterYMF271[chipID][3][i] = 0;
                    fmRegisterYMF271[chipID][4][i] = 0;
                    fmRegisterYMF271[chipID][5][i] = 0;
                    fmRegisterYMF271[chipID][6][i] = 0;
                }

                fmRegisterYMF278B[chipID] = new int[3][] { new int[0x100], new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF278B[chipID][0][i] = 0;
                    fmRegisterYMF278B[chipID][1][i] = 0;
                    fmRegisterYMF278B[chipID][2][i] = 0;
                }
                fmRegisterYMF278BRyhthm[0] = 0;
                fmRegisterYMF278BRyhthm[1] = 0;
                fmRegisterYMF278BRyhthmB[0] = 0;
                fmRegisterYMF278BRyhthmB[1] = 0;

                fmRegisterY8950[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterY8950[chipID][i] = 0;
                }

                YMZ280BRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    YMZ280BRegister[chipID][i] = 0;
                }

                YM2151FmRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    YM2151FmRegister[chipID][i] = 0;
                }
                YM2151FmKeyOn[chipID] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2203[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2203[chipID][i] = 0;
                }
                fmKeyOnYM2203[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                SN76489Register[chipID] = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

                fmRegisterYM2413[chipID] = new int[0x39];
                for (int i = 0; i < 0x39; i++)
                {
                    fmRegisterYM2413[chipID][i] = 0;
                }
                //fmRegisterYM2413Ryhthm[0] = 0;
                //fmRegisterYM2413Ryhthm[1] = 0;
                //fmRegisterYM2413RyhthmB[0] = 0;
                //fmRegisterYM2413RyhthmB[1] = 0;

                AY8910PsgRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    AY8910PsgRegister[chipID][i] = 0;
                }
                AY8910PsgKeyOn[chipID] = new int[3] { 0, 0, 0 };

                pcmRegisterC140[chipID] = new byte[0x200];
                pcmKeyOnC140[chipID] = new bool[24];

                pcmRegisterC352[chipID] = new ushort[0x203];
                pcmKeyOnC352[chipID] = new ushort[32];

                pcmRegisterSEGAPCM[chipID] = new byte[0x200];
                pcmKeyOnSEGAPCM[chipID] = new bool[16];

                midiParams[chipID] = new MIDIParam();

                AY8910NowFadeoutVol[chipID] = 0;
                C140NowFadeoutVol[chipID] = 0;
                SEGAPCMNowFadeoutVol[chipID] = 0;
                SN76489NowFadeoutVol[chipID] = 0;
                YM2151NowFadeoutVol[chipID] = 0;
                nowYM2203FadeoutVol[chipID] = 0;
                nowYM2413FadeoutVol[chipID] = 0;
                nowYM2608FadeoutVol[chipID] = 0;
                nowYM2610FadeoutVol[chipID] = 0;
                nowYM2612FadeoutVol[chipID] = 0;

            }

        }

        public MDSound.MDSound.Chip GetChipInfo(MDSound.MDSound.enmInstrumentType typ)
        {
            if (dicChipsInfo.ContainsKey(typ)) return dicChipsInfo[typ];
            return null;
        }

        public void Close()
        {
            midiExport.Close();
        }




        public midiOutInfo[] GetMIDIoutInfo()
        {
            return midiOutInfos;
        }

        public void setMIDIout(midiOutInfo[] midiOutInfos, List<NAudio.Midi.MidiOut> midiOuts, List<int> midiOutsType, List<vstInfo2> vstMidiOuts, List<int> vstMidiOutsType)
        {
            this.midiOutInfos = null;
            if (midiOutInfos != null && midiOutInfos.Length > 0)
            {
                this.midiOutInfos = new midiOutInfo[midiOutInfos.Length];
                for (int i = 0; i < midiOutInfos.Length; i++)
                {
                    this.midiOutInfos[i] = new midiOutInfo();
                    this.midiOutInfos[i].beforeSendType = midiOutInfos[i].beforeSendType;
                    this.midiOutInfos[i].fileName = midiOutInfos[i].fileName;
                    this.midiOutInfos[i].id = midiOutInfos[i].id;
                    this.midiOutInfos[i].isVST = midiOutInfos[i].isVST;
                    this.midiOutInfos[i].manufacturer = midiOutInfos[i].manufacturer;
                    this.midiOutInfos[i].name = midiOutInfos[i].name;
                    this.midiOutInfos[i].type = midiOutInfos[i].type;
                    this.midiOutInfos[i].vendor = midiOutInfos[i].vendor;
                }
            }
            this.midiOuts = midiOuts;
            this.midiOutsType = midiOutsType;
            this.vstMidiOuts = vstMidiOuts;
            this.vstMidiOutsType = vstMidiOutsType;

            if (midiParams == null && midiParams.Length < 1) return;
            if (midiOutsType == null && vstMidiOutsType == null) return;
            if (midiOuts == null && vstMidiOuts == null) return;

            if (midiOutsType.Count > 0) midiParams[0].MIDIModule = Math.Min(midiOutsType[0], 2);
            if (midiOutsType.Count > 1) midiParams[1].MIDIModule = Math.Min(midiOutsType[1], 2);

            if (vstMidiOutsType.Count > 0)
            {
                if (midiOutsType.Count < 1 || (midiOutsType.Count > 0 && midiOuts[0] == null)) midiParams[0].MIDIModule = Math.Min(vstMidiOutsType[0], 2);
            }
            if (vstMidiOutsType.Count > 1)
            {
                if (midiOutsType.Count < 2 || (midiOutsType.Count > 1 && midiOuts[1] == null)) midiParams[1].MIDIModule = Math.Min(vstMidiOutsType[1], 2);
            }
        }

        internal void SetFileName(string fn)
        {
            midiExport.PlayingFileName = fn;
        }

        private void MIDIWriteRegisterControl(Chip chip, EnmDataType type, int address, int data, object exData)
        {
            if (MIDI.Model == EnmModel.RealModel)
            {
                switch (data)
                {
                    case -1:
                        if (exData != null && exData is byte[])
                        {
                            midiOuts[address].SendBuffer((byte[])exData);
                            if (address < midiParams.Length) midiParams[address].SendBuffer((byte[])exData);
                        }
                        break;
                    case -2:
                        try
                        {
                            //resetできない機種もある?
                            midiOuts[address].Reset();
                        }
                        catch { }
                        break;
                    case -3:
                        midiParams[address].Lyric = (string)exData;
                        break;
                }
            }
        }

        public int getMIDIoutCount()
        {
            if (midiOuts == null) return 0;
            return midiOuts.Count;
        }

        public void sendMIDIout(long Counter,int chipID, byte cmd, byte prm1, byte prm2)//, int deltaFrames = 0)
        {
            if (MIDI.Model == EnmModel.RealModel)
            {
                if (midiOuts == null) return;
                if (chipID >= midiOuts.Count) return;
                if (midiOuts[chipID] == null) return;

                enq(Counter, MIDI, EnmDataType.Block, chipID, -1, new byte[] { cmd, prm1, prm2 });
                //midiOuts[chipID].SendBuffer(data);
                //if (chipID < midiParams.Length) midiParams[chipID].SendBuffer(data);
                return;
            }

            //EnmModel model = EnmModel.RealModel;
            //if (model == EnmModel.RealModel)
            //{
            //    if (midiOuts == null) return;
            //    if (num >= midiOuts.Count) return;
            //    if (midiOuts[num] == null) return;

            //    midiOuts[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
            //    if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
            //    return;
            //}

            //if (vstMidiOuts == null) return;
            //if (num >= vstMidiOuts.Count) return;
            //if (vstMidiOuts[num] == null) return;

            //Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
            //    deltaFrames
            //    , 0//noteLength
            //    , 0//noteOffset
            //    , new byte[] { cmd, prm1, prm2 }
            //    , 0//detune
            //    , 0//noteOffVelocity
            //    );
            //vstMidiOuts[num].AddMidiEvent(evt);
            //if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
        }

        public void sendMIDIout(long Counter,int chipID, byte cmd, byte prm1)//, int deltaFrames = 0)
        {
            if (MIDI.Model == EnmModel.RealModel)
            {
                if (midiOuts == null) return;
                if (chipID >= midiOuts.Count) return;
                if (midiOuts[chipID] == null) return;

                enq(Counter, MIDI, EnmDataType.Block, chipID, -1, new byte[] { cmd, prm1 });
                //midiOuts[chipID].SendBuffer(data);
                //if (chipID < midiParams.Length) midiParams[chipID].SendBuffer(data);
                return;
            }

            //EnmModel model = EnmModel.RealModel;
            //if (model == EnmModel.RealModel)
            //{
            //    if (midiOuts == null) return;
            //    if (num >= midiOuts.Count) return;
            //    if (midiOuts[num] == null) return;

            //    midiOuts[num].SendBuffer(new byte[] { cmd, prm1 });
            //    if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
            //    return;
            //}

            //if (vstMidiOuts == null) return;
            //if (num >= vstMidiOuts.Count) return;
            //if (vstMidiOuts[num] == null) return;

            //Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
            //    deltaFrames
            //    , 0//noteLength
            //    , 0//noteOffset
            //    , new byte[] { cmd, prm1 }
            //    , 0//detune
            //    , 0//noteOffVelocity
            //    );
            //vstMidiOuts[num].AddMidiEvent(evt);
            //if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
        }

        public void sendMIDIout(long Counter,int chipID, byte[] data)
        {
            if (MIDI.Model == EnmModel.RealModel)
            {
                if (midiOuts == null) return;
                if (chipID >= midiOuts.Count) return;
                if (midiOuts[chipID] == null) return;

                enq(Counter, MIDI, EnmDataType.Block, chipID, -1, data);
                //midiOuts[chipID].SendBuffer(data);
                //if (chipID < midiParams.Length) midiParams[chipID].SendBuffer(data);
                return;
            }

            //if (vstMidiOuts == null) return;
            //if (num >= vstMidiOuts.Count) return;
            //if (vstMidiOuts[num] == null) return;

            //Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
            //    deltaFrames
            //    , 0//noteLength
            //    , 0//noteOffset
            //    , data
            //    , 0//detune
            //    , 0//noteOffVelocity
            //    );
            //vstMidiOuts[num].AddMidiEvent(evt);
            //if (num < midiParams.Length) midiParams[num].SendBuffer(data);
        }

        public void sendMIDIoutLyric(long Counter, int chipID, string comment)
        {
            if (midiOuts == null) return;
            if (chipID >= midiOuts.Count) return;
            if (midiOuts[chipID] == null) return;

            enq(Counter, MIDI, EnmDataType.Block, chipID, -3, comment);

        }

        public void resetAllMIDIout()
        {
            if (midiOuts != null)
            {
                for (int i = 0; i < midiOuts.Count; i++)
                {
                    if (midiOuts[i] == null) continue;
                    try
                    {
                        //resetできない機種もある?
                        midiOuts[i].Reset();
                    }
                    catch { }
                }
            }
            if (vstMidiOuts != null)
            {
                for (int i = 0; i < vstMidiOuts.Count; i++)
                {
                    if (vstMidiOuts[i] == null) continue;
                    if (vstMidiOuts[i].vstPlugins == null) continue;
                    if (vstMidiOuts[i].vstPlugins.PluginCommandStub == null) continue;
                    try
                    {
                        List<byte> dat = new List<byte>();
                        for (int ch = 0; ch < 16; ch++)
                        {
                            sendMIDIout(0, i, new byte[] { (byte)(0xb0 + ch), 120, 0x00 });
                            sendMIDIout(0, i, new byte[] { (byte)(0xb0 + ch), 64, 0x00 });
                        }

                    }
                    catch { }
                }
            }
        }

        public void MIDISoftReset(long Counter, int ChipID)
        {
            List<PackData> data = MIDIMakeSoftReset(ChipID);
            enq(Counter, MIDI, EnmDataType.Block, ChipID, -2, data.ToArray());
        }

        public List<PackData> MIDIMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            for (i = 0; i < midiOuts.Count; i++)
            {
                data.Add(new PackData(MIDI, EnmDataType.Block, i, -2, null));//naudio midi reset
                for (int ch = 0; ch < 16; ch++)
                {
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 11, 0x00 }));//Volume 0
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch),  7, 0x00 }));//Exp 0
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 64, 0x00 }));//Hold off
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 91, 0x00 }));//Rev 0
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 93, 0x00 }));//Cho 0
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 94, 0x00 }));//Var 0
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 0x7b, 0x00 }));//all note off
                    data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 0x78, 0x00 }));//all sound off
                }
            }

            return data;
        }



        private void AY8910WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctAY8910[Chip.Number].UseScci && ctAY8910[Chip.Number].UseEmu)
                        mds.WriteAY8910((byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scAY8910[Chip.Number] != null)
                        scAY8910[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteAY8910((byte)Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scAY8910[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scAY8910[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void AY8910SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctAY8910 == null) return;
            if (!AY8910[Chip.Number].Use) return;

            if (Chip.Number == 0) chipLED.PriAY10 = 2;
            else chipLED.SecAY10 = 2;

            int dAddr = (Address & 0xff);

            AY8910PsgRegister[Chip.Number][dAddr] = dData;

            //ssg level
            if ((dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = AY8910NowFadeoutVol[Chip.Number];// >> 3;
                dData = Math.Max(dData - d, 0);
                dData = AY8910MaskPSGCh[Chip.Number][dAddr - 0x08] ? 0 : dData;
            }

            if (AY8910[Chip.Number].Model == EnmModel.RealModel && scAY8910[Chip.Number].mul != 1.0)
            {
                if (dAddr >= 0x00 && dAddr <= 0x05)
                {
                    int ch = dAddr >> 1;
                    int b = dAddr & 1;
                    int nowFreq = AY8910ChFreq[ch];
                    dData &= 0xff;
                    if (b == 0)
                    {
                        AY8910ChFreq[ch] = (AY8910ChFreq[ch] & 0xf00) | dData;
                    }
                    else
                    {
                        AY8910ChFreq[ch] = (AY8910ChFreq[ch] & 0x0ff) | (dData << 8);
                    }

                    if (nowFreq != AY8910ChFreq[ch])
                    {
                        nowFreq = (int)(AY8910ChFreq[ch] * scAY8910[Chip.Number].mul);
                        PackData[] pdata = new PackData[2] { new PackData(), new PackData() };

                        pdata[0].Address = ch * 2;
                        pdata[0].Data = (byte)nowFreq;
                        pdata[1].Address = ch * 2 + 1;
                        pdata[1].Data = (byte)(nowFreq >> 8);

                        Type = EnmDataType.Block;
                        ExData = pdata;
                    }
                }
                if (dAddr == 0x06)
                {

                }
            }

        }

        public void AY8910SetRegister(long Counter, int ChipID, int dAddr, int dData)
        {
            enq(Counter, AY8910[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void AY8910SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, AY8910[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void AY8910SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = AY8910MakeSoftReset(ChipID);
            AY8910SetRegister(Counter, ChipID, data.ToArray());
        }

        public List<PackData> AY8910MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(AY8910[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(AY8910[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(AY8910[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                          // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(AY8910[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(AY8910[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            return data;
        }

        public void AY8910SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            AY8910MaskPSGCh[chipID][ch] = mask;

            if (noSend) return;

            int c = ch;
            if (ch < 3)
            {
                AY8910SetRegister(Counter, (byte)chipID, 0x08 + c , AY8910PsgRegister[chipID][0x08 + c ]);
            }
        }

        public void AY8910WriteClock(byte chipID, int clock)
        {
            if (scAY8910 != null && scAY8910[chipID] != null)
            {
                if (scAY8910[chipID] is RC86ctlSoundChip)
                {
                    Nc86ctl.ChipType ct = ((RC86ctlSoundChip)scAY8910[chipID]).chiptype;
                    //OPNA/OPN3Lが選ばれている場合は周波数を2倍にする
                    if (ct == Nc86ctl.ChipType.CHIP_OPN3L || ct == Nc86ctl.ChipType.CHIP_OPNA)
                    {
                        clock *= 4;
                    }
                }

                scAY8910[chipID].dClock = scAY8910[chipID].SetMasterClock((uint)clock);
                scAY8910[chipID].mul = (double)scAY8910[chipID].dClock / (double)clock;
            }
        }

        public void AY8910SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < AY8910.Length; i++)
            {
                if (!AY8910[i].Use) continue;
                if (AY8910[i].Model == EnmModel.VirtualModel) continue;
                if (AY8910NowFadeoutVol[i]  == v) continue;

                AY8910NowFadeoutVol[i] = v;
                for (int c = 0; c < 3; c++)
                {
                    AY8910SetRegister(Counter, i, 0x08 + c , AY8910PsgRegister[i][0x08 + c ]);
                }
            }
        }

        public void AY8910SetSSGVolume(byte chipID, int vol)
        {
            if (scAY8910 != null && scAY8910[chipID] != null)
            {
                scAY8910[chipID].setSSGVolume((byte)vol);
            }
        }



        private void C140WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctC140[Chip.Number].UseScci && ctC140[Chip.Number].UseEmu)
                        mds.WriteC140((byte)Chip.Number, (uint)address, (byte)data);
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scC140[Chip.Number] != null)
                        scC140[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteC140((byte)Chip.Number, (uint)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmModel.RealModel)
                        {
                            if (scC140[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scC140[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmModel.VirtualModel)
                            mds.WriteC140PCMData((byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                            // ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
                        else
                        {
                            if (scC140 != null && scC140[Chip.Number] != null)
                            {
                                // スタートアドレス設定
                                scC140[Chip.Number].setRegister(0x10000, (byte)((uint)((object[])exData)[1]));
                                scC140[Chip.Number].setRegister(0x10001, (byte)((uint)((object[])exData)[1] >> 8));
                                scC140[Chip.Number].setRegister(0x10002, (byte)((uint)((object[])exData)[1] >> 16));
                                // データ転送
                                for (int cnt = 0; cnt < (uint)((object[])exData)[2]; cnt++)
                                {
                                    scC140[Chip.Number].setRegister(0x10004, ((byte[])((object[])exData)[3])[(uint)((object[])exData)[4] + cnt]);
                                }
                                //scC140[chipID].setRegister(0x10006, (int)ROMSize);

                                realChip.SendData();
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void C140SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctC140 == null) return;

            if (Chip.Number == 0) chipLED.PriC140 = 2;
            else chipLED.SecC140 = 2;

            int dAddr = (Address & 0x1ff);
            if (dAddr < 0x180)
            {
                pcmRegisterC140[Chip.Number][dAddr] = (byte)dData;

                if (
                    (Chip.Model == EnmModel.VirtualModel && (ctC140[Chip.Number] == null || !ctC140[Chip.Number].UseScci))
                    || (Chip.Model == EnmModel.RealModel && (scC140 != null && scC140[Chip.Number] != null))
                    )
                {
                    byte ch = (byte)(dAddr >> 4);
                    switch (dAddr & 0xf)
                    {
                        case 0x05:
                            if ((dData & 0x80) != 0)
                            {
                                pcmKeyOnC140[Chip.Number][ch] = true;
                            }
                            break;
                    }
                }

                if((dAddr & 0xf) == 0 || (dAddr & 0xf) == 1)
                {
                    int d = C140NowFadeoutVol[Chip.Number];// >> 3;
                    dData = Math.Max(dData - d, 0);
                    //dData = dData;
                }
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (ctC140[Chip.Number] == null || !ctC140[Chip.Number].UseScci)
            //        mds.WriteC140((byte)Chip.Number, (uint)dAddr, (byte)dData);
            //}
            //else
            //{
            //    if (scC140 != null && scC140[Chip.Number] != null) scC140[Chip.Number].setRegister((int)dAddr, dData);
            //}
        }

        public void C140SetRegister(long Counter, int ChipID, int dAddr, int dData)
        {
            enq(Counter, C140[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void C140SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, C140[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void C140WritePCMData(long Counter,byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(Counter, C140[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });

            //if (model == EnmModel.VirtualModel)
            //    mds.WriteC140PCMData(chipID, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            //else
            //{
            //    if (scC140 != null && scC140[chipID] != null)
            //    {
            //        // スタートアドレス設定
            //        scC140[chipID].setRegister(0x10000, (byte)(DataStart));
            //        scC140[chipID].setRegister(0x10001, (byte)(DataStart >> 8));
            //        scC140[chipID].setRegister(0x10002, (byte)(DataStart >> 16));
            //        // データ転送
            //        for (int cnt = 0; cnt < DataLength; cnt++)
            //        {
            //            scC140[chipID].setRegister(0x10004, romdata[SrcStartAdr + cnt]);
            //        }
            //        //scC140[chipID].setRegister(0x10006, (int)ROMSize);

            //        realChip.SendData();
            //    }
            //}
        }

        public void C140WriteType(Chip Chip, MDSound.c140.C140_TYPE type)
        {
            if (Chip.Model == EnmModel.RealModel)
            {
                if (scC140 != null && scC140[Chip.Number] != null)
                {
                    switch (type)
                    {
                        case MDSound.c140.C140_TYPE.SYSTEM2:
                            scC140[Chip.Number].setRegister(0x10008, 0);
                            break;
                        case MDSound.c140.C140_TYPE.SYSTEM21:
                            scC140[Chip.Number].setRegister(0x10008, 1);
                            break;
                        case MDSound.c140.C140_TYPE.ASIC219:
                            scC140[Chip.Number].setRegister(0x10008, 2);
                            break;
                    }
                }
            }
        }

        public void C140SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = C140MakeSoftReset(ChipID);
            C140SetRegister(Counter, ChipID, data.ToArray());
        }

        public List<PackData> C140MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            for (i = 0; i < 24; i++)
            {
                data.Add(new PackData(C140[chipID], EnmDataType.Normal, (i << 4) + 0x05, 0x00, null));// KeyOff
                data.Add(new PackData(C140[chipID], EnmDataType.Normal, (i << 4) + 0x00, 0x00, null));// L vol
                data.Add(new PackData(C140[chipID], EnmDataType.Normal, (i << 4) + 0x01, 0x00, null));// R vol
            }

            return data;
        }

        public void C140WriteClock(byte chipID, int clock)
        {
            if (scC140 != null && scC140[chipID] != null)
            {
                scC140[chipID].dClock = scC140[chipID].SetMasterClock((uint)clock);
                scC140[chipID].mul = (double)scC140[chipID].dClock / (double)clock;
            }
        }

        public void C140SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < C140.Length; i++)
            {
                if (!C140[i].Use) continue;
                if (C140[i].Model == EnmModel.VirtualModel) continue;
                if (C140NowFadeoutVol[i] >> 4 == v >> 4) continue;

                C140NowFadeoutVol[i] = v;
                for (int c = 0; c < 24; c++)
                {
                    C140SetRegister(Counter, i, (c << 4) + 0, pcmRegisterC140[i][(c << 4) + 0]);
                    C140SetRegister(Counter, i, (c << 4) + 1, pcmRegisterC140[i][(c << 4) + 1]);
                }
            }
        }



        private void YM2151WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctYM2151[Chip.Number].UseScci)
                    {
                        if (ctYM2151[Chip.Number].UseEmu) mds.WriteYM2151((byte)Chip.Number, (byte)address, (byte)data);
                        if (ctYM2151[Chip.Number].UseEmu2) mds.WriteYM2151mame((byte)Chip.Number, (byte)address, (byte)data);
                        if (ctYM2151[Chip.Number].UseEmu3) mds.WriteYM2151x68sound((byte)Chip.Number, (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scYM2151[Chip.Number] != null)
                    {
                        if (Chip.Hosei == 0 || (address < 0x28 || address > 0x2f))
                        {
                            scYM2151[Chip.Number].setRegister(address, data);
                        }
                        else
                        {
                            int oct = (data & 0x70) >> 4;
                            int note = data & 0xf;
                            note = (note < 3) ? note : ((note < 7) ? (note - 1) : ((note < 11) ? (note - 2) : (note - 3)));
                            note += Chip.Hosei - 1;
                            if (note < 0)
                            {
                                oct += (note / 12) - 1;
                                note = (note % 12) + 12;
                            }
                            else
                            {
                                oct += (note / 12);
                                note %= 12;
                            }

                            note = (note < 3) ? note : ((note < 6) ? (note + 1) : ((note < 9) ? (note + 2) : (note + 3)));
                            scYM2151[Chip.Number].setRegister(address, (oct << 4) | note);
                        }
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        if (ctYM2151[Chip.Number].UseEmu) foreach (PackData dat in pdata) mds.WriteYM2151((byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        if (ctYM2151[Chip.Number].UseEmu2) foreach (PackData dat in pdata) mds.WriteYM2151mame((byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        if (ctYM2151[Chip.Number].UseEmu3) foreach (PackData dat in pdata) mds.WriteYM2151x68sound((byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scYM2151[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM2151[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2151SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {

            if (ctYM2151 == null) return;

            if (Chip.Number == 0) chipLED.PriOPM = 2;
            else chipLED.SecOPM = 2;

            int dPort = (byte)(Address >> 8);
            int dAddr = (byte)Address;

            YM2151FmRegister[Chip.Number][dAddr] = dData;

            if (Chip.Model == EnmModel.VirtualModel)
            {
                midiExport.outMIDIData(EnmChip.YM2151, Chip.Number, dPort, dAddr, dData, Chip.Hosei, Counter);
            }

            if ((Chip.Model == EnmModel.RealModel && ctYM2151[Chip.Number].UseScci) || (Chip.Model == EnmModel.VirtualModel && !ctYM2151[Chip.Number].UseScci))
            {
                if (dAddr == 0x08) //Key-On/Off
                {
                    int ch = dData & 0x7;
                    if (ch >= 0 && ch < 8)
                    {
                        if ((dData & 0x78) != 0)
                        {
                            byte con = (byte)(dData & 0x78);
                            YM2151FmKeyOn[Chip.Number][ch] = con | 1;
                            YM2151FmVol[Chip.Number][ch] = 256 * 6;
                        }
                        else
                        {
                            YM2151FmKeyOn[Chip.Number][ch] &= 0xfe;
                        }
                    }
                }
            }

            //AMD/PMD
            if (dAddr == 0x19)
            {
                if ((dData & 0x80) != 0)
                {
                    YM2151FmPMD[Chip.Number] = dData & 0x7f;
                }
                else
                {
                    YM2151FmAMD[Chip.Number] = dData & 0x7f;
                }
            }

            if ((dAddr & 0xf8) == 0x20)
            {
                int al = dData & 0x07;//AL
                int ch = (dAddr & 0x7);

                for (int i = 0; i < 4; i++)
                {
                    int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        if (YM2151MaskFMCh[Chip.Number][ch])
                        {
                            if (Chip.Model == EnmModel.VirtualModel)
                            {
                                if (!ctYM2151[Chip.Number].UseScci)
                                {
                                    if (ctYM2151[Chip.Number].UseEmu) mds.WriteYM2151((byte)Chip.Number, (byte)(0x60 + i * 8 + ch), (byte)127);
                                    if (ctYM2151[Chip.Number].UseEmu2) mds.WriteYM2151mame((byte)Chip.Number, (byte)(0x60 + i * 8 + ch), (byte)127);
                                    if (ctYM2151[Chip.Number].UseEmu3) mds.WriteYM2151x68sound((byte)Chip.Number, (byte)(0x60 + i * 8 + ch), (byte)127);
                                }
                            }
                            else
                            {
                                if (scYM2151 != null && scYM2151[Chip.Number] != null) scYM2151[Chip.Number].setRegister(0x60 + i * 8 + ch, 127);
                            }
                        }
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x60 || (dAddr & 0xf0) == 0x70)//TL
            {
                int ch = (dAddr & 0x7);

                int al = YM2151FmRegister[Chip.Number][0x20 + ch] & 0x07;
                int slot = (dAddr & 0x18) >> 3;
                //slot = (slot == 0) ? 0 : ((slot == 1) ? 2 : ((slot == 2) ? 1 : 3));
                if ((algM[al] & (1 << slot)) > 0)
                {
                    dData = Math.Min(dData + YM2151NowFadeoutVol[Chip.Number], 127);
                    dData = YM2151MaskFMCh[Chip.Number][ch] ? 127 : dData;
                }
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2151[Chip.Number].UseScci)
            //    {
            //        if (ctYM2151[Chip.Number].UseEmu) mds.WriteYM2151((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //        if (ctYM2151[Chip.Number].UseEmu2) mds.WriteYM2151mame((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //        if (ctYM2151[Chip.Number].UseEmu3) mds.WriteYM2151x68sound((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2151[Chip.Number] == null) return;

            //    if (dAddr >= 0x28 && dAddr <= 0x2f)
            //    {
            //        if (hosei == 0)
            //        {
            //            scYM2151[Chip.Number].setRegister(dAddr, dData);
            //        }
            //        else
            //        {
            //            int oct = (dData & 0x70) >> 4;
            //            int note = dData & 0xf;
            //            note = (note < 3) ? note : ((note < 7) ? (note - 1) : ((note < 11) ? (note - 2) : (note - 3)));
            //            note += hosei - 1;
            //            if (note < 0)
            //            {
            //                oct += (note / 12) - 1;
            //                note = (note % 12) + 12;
            //            }
            //            else
            //            {
            //                oct += (note / 12);
            //                note %= 12;
            //            }

            //            note = (note < 3) ? note : ((note < 6) ? (note + 1) : ((note < 9) ? (note + 2) : (note + 3)));
            //            if (scYM2151[Chip.Number] != null)
            //                scYM2151[Chip.Number].setRegister(dAddr, (oct << 4) | note);
            //        }
            //    }
            //    else
            //    {
            //        scYM2151[Chip.Number].setRegister(dAddr, dData);
            //    }
            //}

        }

        public void YM2151SetRegister(long Counter, int ChipID, int dAddr, int dData)
        {
            enq(Counter, YM2151[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM2151SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, YM2151[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2151SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2151MakeSoftReset(chipID);
            YM2151SetRegister(Counter, chipID, data.ToArray());
        }

        public List<PackData> YM2151MakeSoftReset(int ChipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            //FM全チャネルキーオフ
            for (i = 0; i < 8; i++)
            {
                // note off
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x08, 0x00 + i, null));
            }

            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x0f, 0x00, null)); //  FM NOISE ENABLE/NOISE FREQ
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x18, 0x00, null)); //  FM HW LFO FREQ
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x19, 0x80, null)); //  FM PMD/VALUE
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x19, 0x00, null)); //  FM AMD/VALUE
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x1b, 0x00, null)); //  FM HW LFO WAVEFORM

            //FM HW LFO RESET
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x01, 0x02, null));
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x01, 0x00, null));

            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x10, 0x00, null)); // FM Timer-A(H)
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x11, 0x00, null)); // FM Timer-A(L)
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x12, 0x00, null)); // FM Timer-B
            data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x14, 0x00, null)); // FM Timer Control

            for (i = 0; i < 8; i++)
            {
                //  FB/ALG/PAN
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x20 + i, 0x00, null));
                // KC
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x28 + i, 0x00, null));
                // KF
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x30 + i, 0x00, null));
                // PMS/AMS
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x38 + i, 0x00, null));
            }
            for (i = 0; i < 0x20; i++)
            {
                // DT1/ML
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x40 + i, 0x00, null));
                // TL=127
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x60 + i, 0x7f, null));
                // KS/AR
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0x80 + i, 0x1F, null));
                // AMD/D1R
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0xa0 + i, 0x00, null));
                // DT2/D2R
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0xc0 + i, 0x00, null));
                // D1L/RR
                data.Add(new PackData(YM2151[ChipID], EnmDataType.Normal, 0xe0 + i, 0x0F, null));
            }

            return data;
        }

        public void YM2151SetMask(long Counter, int chipID, int ch, bool mask)
        {
            YM2151MaskFMCh[chipID][ch] = mask;

            YM2151SetRegister(Counter, (byte)chipID, 0x60 + ch, YM2151FmRegister[chipID][0x60 + ch]);
            YM2151SetRegister(Counter, (byte)chipID, 0x68 + ch, YM2151FmRegister[chipID][0x68 + ch]);
            YM2151SetRegister(Counter, (byte)chipID, 0x70 + ch, YM2151FmRegister[chipID][0x70 + ch]);
            YM2151SetRegister(Counter, (byte)chipID, 0x78 + ch, YM2151FmRegister[chipID][0x78 + ch]);

        }

        public void YM2151WriteClock(byte chipID, int clock)
        {
            if (scYM2151 != null && scYM2151[chipID] != null)
            {
                scYM2151[chipID].dClock = scYM2151[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2151SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2151.Length; i++)
            {
                if (!YM2151[i].Use) continue;
                if (YM2151[i].Model == EnmModel.VirtualModel) continue;
                if (YM2151NowFadeoutVol[i] >> 3 == v >> 3) continue;

                YM2151NowFadeoutVol[i] = v;

                for (int c = 0; c < 8; c++)
                {
                    YM2151SetRegister(Counter, i, 0x60 + c, YM2151FmRegister[i][0x60 + c]);
                    YM2151SetRegister(Counter, i, 0x68 + c, YM2151FmRegister[i][0x68 + c]);
                    YM2151SetRegister(Counter, i, 0x70 + c, YM2151FmRegister[i][0x70 + c]);
                    YM2151SetRegister(Counter, i, 0x78 + c, YM2151FmRegister[i][0x78 + c]);
                }
            }
        }

        public void YM2151SetSyncWait(byte chipID, int wait)
        {
            if (scYM2151[chipID] != null && ctYM2151[chipID].UseWait)
            {
                scYM2151[chipID].setRegister(-1, (int)(wait * (ctYM2151[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public int YM2151GetClock(byte chipID)
        {
            if (scYM2151[chipID] == null) return -1;

            return (int)scYM2151[chipID].dClock;
        }



        private void YM2203WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctYM2203[Chip.Number].UseScci && ctYM2203[Chip.Number].UseEmu)
                        mds.WriteYM2203((byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scYM2203[Chip.Number] != null)
                        scYM2203[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM2203((byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scYM2203[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM2203[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2203SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2203 == null) return;

            if (Chip.Number == 0) chipLED.PriOPN = 2;
            else chipLED.SecOPN = 2;

            int dAddr = (Address & 0xff);

            fmRegisterYM2203[Chip.Number][dAddr] = dData;

            if ((Chip.Model == EnmModel.RealModel && ctYM2203[Chip.Number].UseScci) || (Chip.Model == EnmModel.VirtualModel && !ctYM2203[Chip.Number].UseScci))
            {
                if (dAddr == 0x28)
                {
                    int ch = dData & 0x3;
                    if (ch >= 0 && ch < 3)
                    {
                        if (ch != 2 || (fmRegisterYM2203[Chip.Number][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2203[Chip.Number][ch] = (dData & 0xf0) | 1;
                                fmVolYM2203[Chip.Number][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2203[Chip.Number][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2203[Chip.Number][2] = (dData & 0xf0);
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2203[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2203[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2203[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2203[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    int al = fmRegisterYM2203[Chip.Number][0xb0 + ch] & 0x7;
                    int slot = (dAddr & 0xc) >> 2;
                    dData &= 0x7f;

                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2203FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2203[Chip.Number][ch] ? 127 : dData;
                    }
                }
            }

            //if ((dAddr & 0xf0) == 0xb0)//AL
            //{
            //    int ch = (dAddr & 0x3);
            //    int al = dData & 0x07;//AL

            //    if (ch != 3 && maskFMChYM2203[Chip.Number][ch])
            //    {
            //        for (int i = 0; i < 4; i++)
            //        {
            //            int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
            //            if ((algM[al] & (1 << slot)) > 0)
            //            {
            //                setYM2203Register(Chip.Number, 0x40 + ch + slot * 4, fmRegisterYM2203[Chip.Number][0x40 + ch]);
            //            }
            //        }
            //    }
            //}

            //ssg level
            if ((dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2203FadeoutVol[Chip.Number] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2203[Chip.Number][dAddr - 0x08 + 3] ? 0 : dData;
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2203[Chip.Number].UseScci)
            //    {
            //        mds.WriteYM2203((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2203[Chip.Number] == null) return;

            //    scYM2203[Chip.Number].setRegister(dAddr, dData);
            //}
        }

        public void YM2203SetRegister(long Counter, int ChipID, int dAddr, int dData)
        {
            enq(Counter, YM2203[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM2203SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, YM2203[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2203SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = YM2203MakeSoftReset(ChipID);
            YM2203SetRegister(Counter, ChipID, data.ToArray());
        }

        public List<PackData> YM2203MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x28, 0x02, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0xc0, null));
            }
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                       // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(YM2203[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            return data;
        }

        public void YM2203SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2203[chipID][ch] = mask;
            if (ch >= 6 && ch < 9)
            {
                maskFMChYM2203[chipID][2] = mask;
                maskFMChYM2203[chipID][6] = mask;
                maskFMChYM2203[chipID][7] = mask;
                maskFMChYM2203[chipID][8] = mask;
            }

            if (noSend) return;

            int c = ch;
            if (ch < 3)
            {
                YM2203SetRegister(Counter, (byte)chipID, 0x40 + c, fmRegisterYM2203[chipID][0x40 + c]);
                YM2203SetRegister(Counter, (byte)chipID, 0x44 + c, fmRegisterYM2203[chipID][0x44 + c]);
                YM2203SetRegister(Counter, (byte)chipID, 0x48 + c, fmRegisterYM2203[chipID][0x48 + c]);
                YM2203SetRegister(Counter, (byte)chipID, 0x4c + c, fmRegisterYM2203[chipID][0x4c + c]);

            }
            else
            {
                YM2203SetRegister(Counter, (byte)chipID, 0x08 + c - 3, fmRegisterYM2203[chipID][0x08 + c - 3]);
            }
        }

        public void YM2203WriteClock(byte chipID, int clock)
        {
            if (scYM2203 != null && scYM2203[chipID] != null)
            {
                if (scYM2203[chipID] is RC86ctlSoundChip)
                {
                    Nc86ctl.ChipType ct = ((RC86ctlSoundChip)scYM2203[chipID]).chiptype;
                    //OPNA/OPN3Lが選ばれている場合は周波数を2倍にする
                    if (ct == Nc86ctl.ChipType.CHIP_OPN3L || ct == Nc86ctl.ChipType.CHIP_OPNA)
                    {
                        clock *= 2;
                    }
                }
                scYM2203[chipID].dClock = scYM2203[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2203SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2203.Length; i++)
            {
                if (!YM2203[i].Use) continue;
                if (YM2203[i].Model == EnmModel.VirtualModel) continue;
                if ((nowYM2203FadeoutVol[i] >> 3) == (v >> 3)) continue;

                nowYM2203FadeoutVol[i] = v;
                for (int c = 0; c < 3; c++)
                {
                    YM2203SetRegister(Counter, i, 0x40 + c, fmRegisterYM2203[i][0x40 + c]);
                    YM2203SetRegister(Counter, i, 0x44 + c, fmRegisterYM2203[i][0x44 + c]);
                    YM2203SetRegister(Counter, i, 0x48 + c, fmRegisterYM2203[i][0x48 + c]);
                    YM2203SetRegister(Counter, i, 0x4c + c, fmRegisterYM2203[i][0x4c + c]);
                }
            }
        }

        public void YM2203SetSSGVolume(byte chipID, int vol)
        {
            if (scYM2203 != null && scYM2203[chipID] != null)
            {
                scYM2203[chipID].setSSGVolume((byte)vol);
            }
        }



        private void YM2413WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctYM2413[Chip.Number].UseScci && ctYM2413[Chip.Number].UseEmu)
                        mds.WriteYM2413((byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scYM2413[Chip.Number] != null)
                        scYM2413[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM2413((byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scYM2413[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM2413[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2413SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2413 == null) return;

            if (Chip.Number == 0) chipLED.PriOPLL = 2;
            else chipLED.SecOPLL = 2;

            int dAddr = (Address & 0xff);

            fmRegisterYM2413[Chip.Number][dAddr] = dData;

            //if ((Chip.Model == EnmModel.RealModel && ctYM2413[Chip.Number].UseScci) || (Chip.Model == EnmModel.VirtualModel && !ctYM2413[Chip.Number].UseScci))
            //{
            if (dAddr >= 0x20 && dAddr <= 0x28)
            {
                int ch = dAddr - 0x20;
                int k = dData & 0x10;
                if (k == 0)
                {
                    kiYM2413[Chip.Number].Off[ch] = true;
                }
                else
                {
                    if (kiYM2413[Chip.Number].Off[ch]) kiYM2413[Chip.Number].On[ch] = true;
                    kiYM2413[Chip.Number].Off[ch] = false;
                }

                //mask適用
                if (maskFMChYM2413[Chip.Number][ch]) dData &= 0xef;
            }

            if (dAddr == 0x0e)
            {
                for (int c = 0; c < 5; c++)
                {
                    if ((dData & (0x10 >> c)) == 0)
                    {
                        kiYM2413[Chip.Number].Off[c + 9] = true;
                    }
                    else
                    {
                        if (kiYM2413[Chip.Number].Off[c + 9]) kiYM2413[Chip.Number].On[c + 9] = true;
                        kiYM2413[Chip.Number].Off[c + 9] = false;
                    }
                }

                dData = (dData & 0x20)
                    | (maskFMChYM2413[Chip.Number][9] ? 0 : (dData & 0x10))
                    | (maskFMChYM2413[Chip.Number][10] ? 0 : (dData & 0x08))
                    | (maskFMChYM2413[Chip.Number][11] ? 0 : (dData & 0x04))
                    | (maskFMChYM2413[Chip.Number][12] ? 0 : (dData & 0x02))
                    | (maskFMChYM2413[Chip.Number][13] ? 0 : (dData & 0x01))
                    ;
            }

            if (dAddr >= 0x30 && dAddr <= 0x38)
            {
                int v = (dData & 0xf) + nowYM2413FadeoutVol[Chip.Number];
                v = Math.Max(Math.Min(v, 15), 0);
                dData = (dData & 0xf0) | (v & 0xf);
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2413[Chip.Number].UseScci)
            //    {
            //        mds.WriteYM2413((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2413[Chip.Number] == null) return;
            //    scYM2413[Chip.Number].setRegister(dAddr, dData);
            //}
        }

        public void YM2413SetRegister(long Counter, int ChipID, int dAddr, int dData)
        {
            enq(Counter, YM2413[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM2413SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, YM2413[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2413SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = YM2413MakeSoftReset(ChipID);
            YM2413SetRegister(Counter, ChipID, data.ToArray());
        }

        public List<PackData> YM2413MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(YM2413[chipID], EnmDataType.Normal, 0x20 + i, 0x00, null));
            }
            // FM Vol=15(min)
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(YM2413[chipID], EnmDataType.Normal, 0x30 + i, 0x0f, null));
            }

            return data;
        }

        public void YM2413SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2413[chipID][ch] = mask;

            if (ch < 9)
            {
                YM2413SetRegister(Counter, (byte)chipID, 0x20 + ch, fmRegisterYM2413[chipID][0x20 + ch]);
            }
            else if (ch < 14)
            {
                YM2413SetRegister(Counter, (byte)chipID, 0x0e, fmRegisterYM2413[chipID][0x0e]);
            }
        }

        public void YM2413WriteClock(byte chipID, int clock)
        {
            if (scYM2413 != null && scYM2413[chipID] != null)
            {
                scYM2413[chipID].dClock = scYM2413[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2413SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2413.Length; i++)
            {
                if (!YM2413[i].Use) continue;
                if (YM2413[i].Model == EnmModel.VirtualModel) continue;
                if (nowYM2413FadeoutVol[i] == v) continue;

                nowYM2413FadeoutVol[i] = v;
                for (int c = 0; c < 9; c++)
                {
                    YM2413SetRegister(Counter, i, 0x30 + c, fmRegisterYM2413[i][0x30 + c]);
                }
            }
        }




        private void YM2608WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctYM2608[Chip.Number].UseScci && ctYM2608[Chip.Number].UseEmu)
                    {
                        mds.WriteYM2608((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                    else if (ctYM2610[Chip.Number].OnlyPCMEmulation)
                    {
                        {
                            bool bSend = false;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x110)
                            {
                                // ADPCM
                                bSend = true;
                            }

                            if (bSend)
                            {
                                mds.WriteYM2608((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                        }
                    }
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scYM2608[Chip.Number] != null)
                    {
                        if (scYM2608[Chip.Number] is RC86ctlSoundChip && ((RC86ctlSoundChip)scYM2608[Chip.Number]).Type == (int)Nc86ctl.ChipType.CHIP_OPL3)
                        {
                            bool bSend = true;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x110)
                            {
                                // ADPCM
                                bSend = false;
                            }
                            if (bSend)
                            {
                                scYM2608[Chip.Number].setRegister(address, data);
                            }
                        }
                        else
                        {
                            scYM2608[Chip.Number].setRegister(address, data);
                        }
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        log.Write("Sending YM2608(Emu) ADPCM");
                        foreach (PackData dat in pdata)
                            mds.WriteYM2608((byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scYM2608[Chip.Number] != null)
                        {
                            log.Write("Sending YM2608 ADPCM");
                            foreach (PackData dat in pdata)
                                scYM2608[Chip.Number].setRegister(dat.Address, dat.Data);
                            Audio.realChip.WaitOPNADPCMData();
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2608SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2608 == null) return;

            if (Chip.Number == 0) chipLED.PriOPNA = 2;
            else chipLED.SecOPNA = 2;

            int dPort = (Address >> 8) & 1;
            int dAddr = (Address & 0xff);

            fmRegisterYM2608[Chip.Number][dPort][dAddr] = dData;

            //if (
            //    (Chip.Model == EnmModel.VirtualModel && (ctYM2608[Chip.Number] == null || !ctYM2608[Chip.Number].UseScci))
            //    || (Chip.Model == EnmModel.RealModel && (scYM2608 != null && scYM2608[Chip.Number] != null))
            //    )
            //{
            //}

            if ((Chip.Model == EnmModel.RealModel && ctYM2608[Chip.Number].UseScci) || (Chip.Model == EnmModel.VirtualModel && !ctYM2608[Chip.Number].UseScci))
            {
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        if (ch != 2 || (fmRegisterYM2608[Chip.Number][0][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2608[Chip.Number][ch] = (dData & 0xf0) | 1;
                                fmVolYM2608[Chip.Number][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2608[Chip.Number][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2608[Chip.Number][2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2608[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2608[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2608[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2608[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

                if (dPort == 1 && dAddr == 0x01)
                {
                    fmVolYM2608AdpcmPan[Chip.Number] = (dData & 0xc0) >> 6;
                    if (fmVolYM2608AdpcmPan[Chip.Number] > 0)
                    {
                        fmVolYM2608Adpcm[Chip.Number][0] = (int)((256 * 6.0 * fmRegisterYM2608[Chip.Number][1][0x0b] / 64.0) * ((fmVolYM2608AdpcmPan[Chip.Number] & 0x02) > 0 ? 1 : 0));
                        fmVolYM2608Adpcm[Chip.Number][1] = (int)((256 * 6.0 * fmRegisterYM2608[Chip.Number][1][0x0b] / 64.0) * ((fmVolYM2608AdpcmPan[Chip.Number] & 0x01) > 0 ? 1 : 0));
                    }
                }

                if (dPort == 0 && dAddr == 0x10)
                {
                    int tl = fmRegisterYM2608[Chip.Number][0][0x11] & 0x3f;
                    for (int i = 0; i < 6; i++)
                    {
                        if ((dData & (0x1 << i)) > 0)
                        {
                            int il = fmRegisterYM2608[Chip.Number][0][0x18 + i] & 0x1f;
                            int pan = (fmRegisterYM2608[Chip.Number][0][0x18 + i] & 0xc0) >> 6;
                            fmVolYM2608Rhythm[Chip.Number][i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                            fmVolYM2608Rhythm[Chip.Number][i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                        }
                    }
                }

            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    int al = fmRegisterYM2608[Chip.Number][dPort][0xb0 + ch] & 0x07;//AL
                    int slot = (dAddr & 0xc) >> 2;
                    dData &= 0x7f;

                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2608FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2608[Chip.Number][dPort * 3 + ch] ? 127 : dData;
                    }
                }
            }

            //if ((dAddr & 0xf0) == 0xb0)//AL
            //{
            //    int ch = (dAddr & 0x3);
            //    int al = dData & 0x07;//AL

            //    if (ch != 3 && maskFMChYM2608[Chip.Number][ch])
            //    {
            //        for (int i = 0; i < 4; i++)
            //        {
            //            int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
            //            if ((algM[al] & (1 << slot)) > 0)
            //            {
            //                //setYM2608Register(Counter, ChipID, dPort, 0x40 + ch + slot * 4, fmRegisterYM2608[Chip.Number][dPort][0x40 + ch]);
            //            }
            //        }
            //    }
            //}

            //ssg level
            if (dPort == 0 && (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2608FadeoutVol[Chip.Number] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2608[Chip.Number][dAddr - 0x08 + 6] ? 0 : dData;
            }

            //rhythm level
            if (dPort == 0 && dAddr == 0x11)
            {
                int d = nowYM2608FadeoutVol[Chip.Number] >> 1;
                dData = Math.Max(dData - d, 0);
            }

            //adpcm level
            if (dPort == 1 && dAddr == 0x0b)
            {
                int d = nowYM2608FadeoutVol[Chip.Number] * 2;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2608[Chip.Number][12] ? 0 : dData;
            }

            //adpcm start
            if (dPort == 1 && dAddr == 0x00)
            {
                if ((dData & 0x80) != 0 && maskFMChYM2608[Chip.Number][12])
                {
                    dData &= 0x7f;
                }
            }

            //Ryhthm
            if (dPort == 0 && dAddr == 0x10)
            {
                if (maskFMChYM2608[Chip.Number][13])
                {
                    dData = 0;
                }
            }


            Address = dPort * 0x100 + dAddr;

        }

        public void YM2608SetRegister(long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Model = YM2608[ChipID].Model;
            dummyChip.Delay = YM2608[ChipID].Delay;
            dummyChip.Device = YM2608[ChipID].Device;
            dummyChip.Number = YM2608[ChipID].Number;
            dummyChip.Use = YM2608[ChipID].Use;

            int address = dPort * 0x100 + dAddr;
            if ((address >= 0x100 && address <= 0x110) && ctYM2608[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmModel.VirtualModel;
            }

            enq(Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
            //enq(Counter, YM2608[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2608SetRegister(long Counter, Chip chip, PackData[] data)
        {
            enq(Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void YM2608SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2608MakeSoftReset(chipID);
            YM2608SetRegister(Counter, YM2608[chipID], data.ToArray());
        }

        public List<PackData> YM2608MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
            }
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                          // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            // RHYTHM
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x10, 0xBF, null)); // 強制発音停止
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x11, 0x00, null)); // Total Level
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x18, 0x00, null)); // BD音量
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x19, 0x00, null)); // SD音量
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x1A, 0x00, null)); // CYM音量
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x1B, 0x00, null)); // HH音量
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x1C, 0x00, null)); // TOM音量
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x1D, 0x00, null)); // RIM音量

            // ADPCM
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + 0x00, 0x21, null)); // ADPCMリセット
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + 0x01, 0x06, null)); // ADPCM消音
            data.Add(new PackData(YM2608[chipID], EnmDataType.Normal, 0x100 + 0x10, 0x9C, null)); // FLAGリセット        

            return data;
        }

        public void YM2608SetMask(long Counter, int ChipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2608[ChipID][ch] = mask;
            if (ch >= 9 && ch < 12)
            {
                maskFMChYM2608[ChipID][2] = mask;
                maskFMChYM2608[ChipID][9] = mask;
                maskFMChYM2608[ChipID][10] = mask;
                maskFMChYM2608[ChipID][11] = mask;
            }

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            if (noSend) return;

            if (ch < 6)
            {
                YM2608SetRegister(Counter, ChipID, p, 0x40 + c, fmRegisterYM2608[ChipID][p][0x40 + c]);
                YM2608SetRegister(Counter, ChipID, p, 0x44 + c, fmRegisterYM2608[ChipID][p][0x44 + c]);
                YM2608SetRegister(Counter, ChipID, p, 0x48 + c, fmRegisterYM2608[ChipID][p][0x48 + c]);
                YM2608SetRegister(Counter, ChipID, p, 0x4c + c, fmRegisterYM2608[ChipID][p][0x4c + c]);

            }
            else if (ch < 9)
            {
                YM2608SetRegister(Counter, ChipID, 0, 0x08 + ch - 6, fmRegisterYM2608[ChipID][0][0x08 + ch - 6]);
            }
            else if (ch < 12)
            {
                YM2608SetRegister(Counter, ChipID, 0, 0x40 + 2, fmRegisterYM2608[ChipID][0][0x40 + 2]);
                YM2608SetRegister(Counter, ChipID, 0, 0x44 + 2, fmRegisterYM2608[ChipID][0][0x44 + 2]);
                YM2608SetRegister(Counter, ChipID, 0, 0x48 + 2, fmRegisterYM2608[ChipID][0][0x48 + 2]);
                YM2608SetRegister(Counter, ChipID, 0, 0x4c + 2, fmRegisterYM2608[ChipID][0][0x4c + 2]);

            }
        }

        public void YM2608WriteClock(byte chipID, int clock)
        {
            if (scYM2608 != null && scYM2608[chipID] != null)
            {
                scYM2608[chipID].dClock = scYM2608[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2608SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2608.Length; i++)
            {
                if (!YM2608[i].Use) continue;
                if (YM2608[i].Model == EnmModel.VirtualModel) continue;
                if ((nowYM2608FadeoutVol[i] >> 3) == (v >> 3)) continue;

                nowYM2608FadeoutVol[i] = v;

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        YM2608SetRegister(Counter, i, p, 0x40 + c, fmRegisterYM2608[i][p][0x40 + c]);
                        YM2608SetRegister(Counter, i, p, 0x44 + c, fmRegisterYM2608[i][p][0x44 + c]);
                        YM2608SetRegister(Counter, i, p, 0x48 + c, fmRegisterYM2608[i][p][0x48 + c]);
                        YM2608SetRegister(Counter, i, p, 0x4c + c, fmRegisterYM2608[i][p][0x4c + c]);
                    }
                }

                //ssg
                YM2608SetRegister(Counter, i, 0, 0x08, fmRegisterYM2608[i][0][0x08]);
                YM2608SetRegister(Counter, i, 0, 0x09, fmRegisterYM2608[i][0][0x09]);
                YM2608SetRegister(Counter, i, 0, 0x0a, fmRegisterYM2608[i][0][0x0a]);
                                           
                //rhythm                   
                YM2608SetRegister(Counter, i, 0, 0x11, fmRegisterYM2608[i][0][0x11]);
                                           
                //adpcm                    
                YM2608SetRegister(Counter, i, 1, 0x0b, fmRegisterYM2608[i][1][0x0b]);
            }
        }

        public void YM2608SetSyncWait(byte chipID, int wait)
        {
            if (scYM2608[chipID] != null && ctYM2608[chipID].UseWait)
            {
                scYM2608[chipID].setRegister(-1, (int)(wait * (ctYM2608[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void YM2608SetSSGVolume(byte chipID, int vol)
        {
            if (scYM2608 != null && scYM2608[chipID] != null)
            {
                scYM2608[chipID].setSSGVolume((byte)vol);
            }
        }

        /// <summary>
        /// 不要かも
        /// </summary>
        /// <param name="chipID"></param>
        public void YM2608SendData(byte chipID)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (model == EnmModel.VirtualModel) return;

            if (scYM2608[chipID] != null && ctYM2608[chipID].UseWait)
            {
                realChip.SendData();
                while (!scYM2608[chipID].isBufferEmpty()) { }
            }
        }



        private void YM2610WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctYM2610[Chip.Number].UseScci && ctYM2610[Chip.Number].UseEmu)
                    {
                        mds.WriteYM2610((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                    else if (ctYM2610[Chip.Number].OnlyPCMEmulation)
                    {
                        //if (ctYM2610[Chip.Number].UseEmu)
                        {
                            bool bSend = false;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x12d)
                            {
                                // ADPCM-A
                                bSend = true;
                            }
                            else if (address >= 0x010 && address <= 0x01c)
                            {
                                // ADPCM-B
                                bSend = true;
                            }

                            if (bSend)
                            {
                                mds.WriteYM2610((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                        }
                    }

                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scYM2610[Chip.Number] != null)
                    {
                        if (scYM2610[Chip.Number] is RScciSoundChip && ((RScciSoundChip)scYM2610[Chip.Number]).Type == (int)EnmRealChipType.YM2610)
                        {
                            scYM2610[Chip.Number].setRegister(address, data);
                        }
                        else
                        {
                            bool bSend = true;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x12d)
                            {
                                // ADPCM-A
                                bSend = false;
                            }
                            else if (address >= 0x010 && address <= 0x01c)
                            {
                                // ADPCM-B
                                bSend = false;
                            }
                            if (bSend)
                            {
                                scYM2610[Chip.Number].setRegister(address, data);
                            }

                            if (scYM2610EB[Chip.Number] != null
                                && !ctYM2610[0].OnlyPCMEmulation)
                            {
                                scYM2610EB[Chip.Number].setRegister(address | 0x10000, data);
                            }
                        }
                    }
                    else
                    {
                        if (scYM2610EA[Chip.Number] != null)
                        {
                            bool bSend = true;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x12d)
                            {
                                // ADPCM-A
                                bSend = false;
                            }
                            else if (address >= 0x010 && address <= 0x01c)
                            {
                                // ADPCM-B
                                bSend = false;
                            }
                            if (bSend)
                            {
                                scYM2610EA[Chip.Number].setRegister(address, data);
                            }
                        }
                        if (scYM2610EB[Chip.Number] != null && !ctYM2610[0].OnlyPCMEmulation)
                        {
                            scYM2610EB[Chip.Number].setRegister(address | 0x10000, data);
                        }
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (exData is PackData[])
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteYM2610((byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmModel.RealModel)
                        {
                            if (scYM2610[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scYM2610[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                        return;
                    }

                    byte[] adpcmData = (byte[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        log.Write(string.Format("Sending YM2610(Emu) ADPCM-{0}", (data == -1) ? "A" : "B"));
                        if (data == -1)
                        {
                            mds.WriteYM2610_SetAdpcmA((byte)Chip.Number, adpcmData);
                        }
                        else
                        {
                            mds.WriteYM2610_SetAdpcmB((byte)Chip.Number, adpcmData);
                        }
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scYM2610[Chip.Number] != null)
                        {
                            //実OPNBのみ
                            if ((scYM2610[Chip.Number] is RScciSoundChip) && scYM2610[Chip.Number].Type == (int)EnmRealChipType.YM2610)
                            {
                                byte dPort = 2;
                                int startAddr = 0;
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x00, 0x00);
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x01, (startAddr >> 8) & 0xff);
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x02, (startAddr >> 16) & 0xff);

                                // pushReg(CMD_YM2610|0x02,0x03,0x01);
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x03, (data == -1) ? 0x01 : 0x00);
                                // データ転送
                                log.Write(string.Format("Sending YM2610 ADPCM-{0}", (data == -1) ? "A" : "B"));
                                for (int cnt = 0; cnt < adpcmData.Length; cnt++)
                                {
                                    // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                                    scYM2610[Chip.Number].setRegister((dPort << 8) | 0x04, adpcmData[cnt]);
                                }

                                realChip.SendData();
                            }
                        }
                        if (scYM2610EB[Chip.Number] != null && !ctYM2610[0].OnlyPCMEmulation)
                        {
                            byte dPort = 2;
                            int startAddr = 0;
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10000, 0x00);
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10001, (startAddr >> 8) & 0xff);
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10002, (startAddr >> 16) & 0xff);

                            // pushReg(CMD_YM2610|0x02,0x03,0x01);
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10003, (data == -1) ? 0x01 : 0x00);
                            // データ転送
                            log.Write(string.Format("Sending YM2610 ADPCM-{0}", (data == -1) ? "A" : "B"));
                            for (int cnt = 0; cnt < adpcmData.Length; cnt++)
                            {
                                // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                                scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10004, adpcmData[cnt]);
                            }

                            realChip.SendData();
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2610SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2610 == null) return;

            if (Chip.Number == 0) chipLED.PriOPNB = 2;
            else chipLED.SecOPNB = 2;

            int dPort = (Address >> 8) & 1;
            int dAddr = (Address & 0xff);

            fmRegisterYM2610[Chip.Number][dPort][dAddr] = dData;

            if ((Chip.Model == EnmModel.RealModel && (ctYM2610[Chip.Number].UseScci || ctYM2610[Chip.Number].UseScci2)) || (Chip.Model == EnmModel.VirtualModel && !ctYM2610[Chip.Number ].UseScci))
            {
                //fmRegisterYM2610[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        if (ch != 2 || (fmRegisterYM2610[Chip.Number ][0][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2610[Chip.Number ][ch] = (dData & 0xf0) | 1;
                                fmVolYM2610[Chip.Number ][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2610[Chip.Number ][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2610[Chip.Number ][2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2610[Chip.Number ][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2610[Chip.Number ][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2610[Chip.Number ][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2610[Chip.Number ][3] = 256 * 6;
                        }
                    }
                }

                // ADPCM B KEYON
                if (dPort == 0 && dAddr == 0x10)
                {
                    if ((dData & 0x80) != 0)
                    {
                        int p = (fmRegisterYM2610[Chip.Number ][0][0x11] & 0xc0) >> 6;
                        p = p == 0 ? 3 : p;
                        if (fmVolYM2610AdpcmPan[Chip.Number ] != p)
                            fmVolYM2610AdpcmPan[Chip.Number ] = p;

                        //if (fmVolYM2610AdpcmPan > 0)
                        //{
                        fmVolYM2610Adpcm[Chip.Number ][0] = (int)((256 * 6.0 * fmRegisterYM2610[Chip.Number ][0][0x1b] / 64.0) * ((fmVolYM2610AdpcmPan[Chip.Number ] & 0x02) > 0 ? 1 : 0));
                        fmVolYM2610Adpcm[Chip.Number ][1] = (int)((256 * 6.0 * fmRegisterYM2610[Chip.Number ][0][0x1b] / 64.0) * ((fmVolYM2610AdpcmPan[Chip.Number ] & 0x01) > 0 ? 1 : 0));
                        //                        System.Console.WriteLine("{0:X2}:{1:X2}", 0x09, fmRegisterYM2610[1][0x09]);
                        //                        System.Console.WriteLine("{0:X2}:{1:X2}", 0x0A, fmRegisterYM2610[1][0x0A]);
                        //}
                    }
                }

                // ADPCM A KEYON
                if (dPort == 1 && dAddr == 0x00)
                {
                    if ((dData & 0x80) == 0)
                    {
                        int tl = fmRegisterYM2610[Chip.Number ][1][0x01] & 0x3f;
                        for (int i = 0; i < 6; i++)
                        {
                            if ((dData & (0x1 << i)) > 0)
                            {
                                int il = fmRegisterYM2610[Chip.Number ][1][0x08 + i] & 0x1f;
                                int pan = (fmRegisterYM2610[Chip.Number ][1][0x08 + i] & 0xc0) >> 6;
                                fmVolYM2610Rhythm[Chip.Number ][i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                                fmVolYM2610Rhythm[Chip.Number ][i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                            }
                        }
                    }
                }

            }



            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    int al = fmRegisterYM2610[Chip.Number][dPort][0xb0 + ch] & 0x07;//AL
                    int slot = (dAddr & 0xc) >> 2;
                    dData &= 0x7f;

                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2610FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2610[Chip.Number][dPort * 3 + ch] ? 127 : dData;
                    }
                }
                //}
            }

            //if ((dAddr & 0xf0) == 0xb0)//AL
            //{
            //    int ch = (dAddr & 0x3);
            //    int al = dData & 0x07;//AL

            //    if (ch != 3 && maskFMChYM2610[Chip.Number ][ch])
            //    {
            //        for (int i = 0; i < 4; i++)
            //        {
            //            int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
            //            if ((algM[al] & (1 << slot)) > 0)
            //            {
            //                setYM2610Register(Chip.Number , dPort, 0x40 + ch + slot * 4, fmRegisterYM2610[Chip.Number ][dPort][0x40 + ch]);
            //            }
            //        }
            //    }
            //}

            //ssg level
            if (dPort == 0 && (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2610FadeoutVol[Chip.Number ] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[Chip.Number ][dAddr - 0x08 + 6] ? 0 : dData;
            }

            //rhythm level
            if (dPort == 1 && dAddr == 0x01)
            {
                int d = nowYM2610FadeoutVol[Chip.Number ] >> 1;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[Chip.Number ][12] ? 0 : dData;
            }

            //Rhythm
            if (dPort == 1 && dAddr == 0x00)
            {
                if (maskFMChYM2610[Chip.Number ][12])
                {
                    dData = 0;
                }
            }

            //adpcm level
            if (dPort == 0 && dAddr == 0x1b)
            {
                int d = nowYM2610FadeoutVol[Chip.Number ] * 2;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[Chip.Number ][13] ? 0 : dData;
            }

            //adpcm start
            if (dPort == 0 && dAddr == 0x10)
            {
                if ((dData & 0x80) != 0 && maskFMChYM2610[Chip.Number ][13])
                {
                    dData &= 0x7f;
                }
            }



            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2610[Chip.Number ].UseScci && !ctYM2610[Chip.Number ].UseScci2)
            //    {
            //        mds.WriteYM2610((byte)Chip.Number , (byte)dPort, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2610[Chip.Number ] != null) scYM2610[Chip.Number ].setRegister(dPort * 0x100 + dAddr, dData);
            //    if (scYM2610EA[Chip.Number ] != null)
            //    {
            //        int dReg = (dPort << 8) | dAddr;
            //        bool bSend = true;
            //        // レジスタをマスクして送信する
            //        if (dReg >= 0x100 && dReg <= 0x12d)
            //        {
            //            // ADPCM-A
            //            bSend = false;
            //        }
            //        else if (dReg >= 0x010 && dReg <= 0x01c)
            //        {
            //            // ADPCM-B
            //            bSend = false;
            //        }
            //        if (bSend)
            //        {
            //            scYM2610EA[Chip.Number ].setRegister((dPort << 8) | dAddr, dData);
            //        }
            //    }
            //    if (scYM2610EB[Chip.Number ] != null)
            //    {
            //        scYM2610EB[Chip.Number ].setRegister((dPort << 8) | dAddr | 0x10000, dData);
            //    }
            //}

        }

        public void YM2610SetRegister(long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Model = YM2610[ChipID].Model;
            dummyChip.Delay = YM2610[ChipID].Delay;
            dummyChip.Device = YM2610[ChipID].Device;
            dummyChip.Number = YM2610[ChipID].Number;
            dummyChip.Use = YM2610[ChipID].Use;

            int address = dPort * 0x100 + dAddr;
            if ((
                (address >= 0x100 && address <= 0x12d) 
                || (address >= 0x010 && address <= 0x01c) 
                //|| ((address >= 0x200 && address <= 0x204) || address == 0x210)
                ) 
                && ctYM2610[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmModel.VirtualModel;
            }

            enq(Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
            //enq(Counter, YM2610[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2610SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, YM2610[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2610WriteSetAdpcmA(long Counter,int ChipID, byte[] ym2610AdpcmA)
        {
            dummyChip.Model = YM2610[ChipID].Model;
            dummyChip.Delay = YM2610[ChipID].Delay;
            dummyChip.Device = YM2610[ChipID].Device;
            dummyChip.Number = YM2610[ChipID].Number;
            dummyChip.Use = YM2610[ChipID].Use;
            
            if (ctYM2610[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmModel.VirtualModel;
            }

            enq(Counter, dummyChip, EnmDataType.Block, -1, -1, ym2610AdpcmA );

        }

        public void YM2610WriteSetAdpcmB(long Counter, int ChipID, byte[] ym2610AdpcmB)
        {
            dummyChip.Model = YM2610[ChipID].Model;
            dummyChip.Delay = YM2610[ChipID].Delay;
            dummyChip.Device = YM2610[ChipID].Device;
            dummyChip.Number = YM2610[ChipID].Number;
            dummyChip.Use = YM2610[ChipID].Use;
            if (ctYM2610[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmModel.VirtualModel;
            }

            enq(Counter, dummyChip, EnmDataType.Block, -1, -2, ym2610AdpcmB);
        }

        public void YM2610SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2610MakeSoftReset(chipID);
            YM2610SetRegister(Counter, chipID, data.ToArray());
        }

        public List<PackData> YM2610MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
            }
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                          // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            // ADPCM-A
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x00, 0xBF, null)); // 強制発音停止
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x01, 0x00, null)); // Total Level
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x08, 0x00, null)); // BD音量
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x09, 0x00, null)); // SD音量
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0A, 0x00, null)); // CYM音量
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0B, 0x00, null)); // HH音量
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0C, 0x00, null)); // TOM音量
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0D, 0x00, null)); // RIM音量

            // ADPCM-B
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x000 + 0x10, 0x01, null)); // ADPCMリセット
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x000 + 0x1B, 0x00, null)); // ADPCM消音
            data.Add(new PackData(YM2610[chipID], EnmDataType.Normal, 0x000 + 0x1C, 0x00, null)); // FLAGリセット        

            return data;
        }

        public void YM2610SetMask(long Counter, int ChipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2610[ChipID][ch] = mask;
            if (ch >= 9 && ch < 12)
            {
                maskFMChYM2610[ChipID][2] = mask;
                maskFMChYM2610[ChipID][9] = mask;
                maskFMChYM2610[ChipID][10] = mask;
                maskFMChYM2610[ChipID][11] = mask;
            }

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            if (ch < 6)
            {
                YM2610SetRegister(Counter, ChipID, p, 0x40 + c, fmRegisterYM2610[ChipID][p][0x40 + c]);
                YM2610SetRegister(Counter, ChipID, p, 0x44 + c, fmRegisterYM2610[ChipID][p][0x44 + c]);
                YM2610SetRegister(Counter, ChipID, p, 0x48 + c, fmRegisterYM2610[ChipID][p][0x48 + c]);
                YM2610SetRegister(Counter, ChipID, p, 0x4c + c, fmRegisterYM2610[ChipID][p][0x4c + c]);

            }
            else if (ch < 9)
            {
                YM2610SetRegister(Counter, ChipID, 0, 0x08 + ch - 6, fmRegisterYM2610[ChipID][0][0x08 + ch - 6]);
            }                     
            else if (ch < 12)     
            {                     
                YM2610SetRegister(Counter, ChipID, 0, 0x40 + 2, fmRegisterYM2610[ChipID][0][0x40 + 2]);
                YM2610SetRegister(Counter, ChipID, 0, 0x44 + 2, fmRegisterYM2610[ChipID][0][0x44 + 2]);
                YM2610SetRegister(Counter, ChipID, 0, 0x48 + 2, fmRegisterYM2610[ChipID][0][0x48 + 2]);
                YM2610SetRegister(Counter, ChipID, 0, 0x4c + 2, fmRegisterYM2610[ChipID][0][0x4c + 2]);

            }
        }

        public void YM2610WriteClock(byte chipID, int clock)
        {
            if (scYM2610 != null && scYM2610[chipID] != null)
            {
                scYM2610[chipID].dClock = scYM2610[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2610SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2610.Length; i++)
            {
                if (!YM2610[i].Use) continue;
                if (YM2610[i].Model == EnmModel.VirtualModel) continue;
                if ((nowYM2610FadeoutVol[i] >> 3) == (v >> 3)) continue;

                nowYM2610FadeoutVol[i] = v;

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        YM2610SetRegister(Counter, i, p, 0x40 + c, fmRegisterYM2610[i][p][0x40 + c]);
                        YM2610SetRegister(Counter, i, p, 0x44 + c, fmRegisterYM2610[i][p][0x44 + c]);
                        YM2610SetRegister(Counter, i, p, 0x48 + c, fmRegisterYM2610[i][p][0x48 + c]);
                        YM2610SetRegister(Counter, i, p, 0x4c + c, fmRegisterYM2610[i][p][0x4c + c]);
                    }
                }

                //ssg
                YM2610SetRegister(Counter, i, 0, 0x08, fmRegisterYM2610[i][0][0x08]);
                YM2610SetRegister(Counter, i, 0, 0x09, fmRegisterYM2610[i][0][0x09]);
                YM2610SetRegister(Counter, i, 0, 0x0a, fmRegisterYM2610[i][0][0x0a]);

                //rhythm
                YM2610SetRegister(Counter, i, 1, 0x01, fmRegisterYM2610[i][1][0x01]);

                //adpcm
                YM2610SetRegister(Counter, i, 0, 0x1b, fmRegisterYM2610[i][0][0x1b]);
            }
        }

        public void YM2610SetSyncWait(byte chipID, int wait)
        {
            if (scYM2610[chipID] != null && ctYM2610[chipID].UseWait)
            {
                scYM2610[chipID].setRegister(-1, (int)(wait * (ctYM2610[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void YM2610SetSSGVolume(byte chipID, int vol)
        {
            if (scYM2610 != null && scYM2610[chipID] != null)
            {
                scYM2610[chipID].setSSGVolume((byte)vol);
            }
        }



        private void YM2612WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctYM2612[Chip.Number].UseScci)
                    {
                        if (ctYM2612[Chip.Number].UseEmu)
                        {
                            mds.WriteYM2612((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                        }
                        if (ctYM2612[Chip.Number].UseEmu2)
                        {
                            mds.WriteYM3438((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                        }
                    }
                    else if (ctYM2612[Chip.Number].OnlyPCMEmulation)
                    {
                        if (address == 0x2a || address == 0x2b)
                        {
                            if (ctYM2612[Chip.Number].UseEmu)
                            {
                                mds.WriteYM2612((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                            if (ctYM2612[Chip.Number].UseEmu2)
                            {
                                mds.WriteYM3438((byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                        }
                    }
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scYM2612[Chip.Number] != null)
                        scYM2612[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM2612((byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        //foreach (PackData dat in pdata)
                        //scYM2612[Chip.Number].setRegister(dat.Address, dat.Data);
                        //Audio.realChip.WaitOPN2DPCMData();
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2612SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2612 == null) return;

            if (Chip.Number == 0) chipLED.PriOPN2 = 2;
            else chipLED.SecOPN2 = 2;

            int dPort = (Address >> 8);
            int dAddr = (Address & 0xff);
            //int pddata = dData;

            fmRegisterYM2612[Chip.Number][dPort][dAddr] = dData;

            if (Chip.Model == EnmModel.VirtualModel)
            {
                midiExport.outMIDIData(EnmChip.YM2612, Chip.Number, dPort, dAddr, dData, 0, Counter);
            }

            if ((Chip.Model == EnmModel.RealModel && ctYM2612[Chip.Number].UseScci) || (Chip.Model == EnmModel.VirtualModel && !ctYM2612[Chip.Number].UseScci))
            {
                //fmRegister[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0)>0)
                    {
                        if (ch != 2 || (fmRegisterYM2612[Chip.Number][0][0x27] & 0xc0) != 0x40)
                        {
                            if (ch != 5 || (fmRegisterYM2612[Chip.Number][0][0x2b] & 0x80) == 0)
                            {
                                if ((dData & 0xf0) != 0)
                                {
                                    fmKeyOnYM2612[Chip.Number][ch] = (dData & 0xf0) | 1;
                                    fmVolYM2612[Chip.Number][ch] = 256 * 6;
                                }
                                else
                                {
                                    fmKeyOnYM2612[Chip.Number][ch] = (dData & 0xf0) | 0;
                                }
                            }
                        }
                        else
                        {
                            fmKeyOnYM2612[Chip.Number][2] = (dData & 0xf0);
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2612[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2612[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2612[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2612[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

                //PCM
                if ((fmRegisterYM2612[Chip.Number][0][0x2b] & 0x80) > 0)
                {
                    if (fmRegisterYM2612[Chip.Number][0][0x2a] > 0)
                    {
                        fmVolYM2612[Chip.Number][5] = Math.Abs(fmRegisterYM2612[Chip.Number][0][0x2a] - 0x7f) * 20;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                dData &= 0x7f;

                if (ch != 3)
                {
                    int al = fmRegisterYM2612[Chip.Number][dPort][0xb0 + ch] & 0x07;
                    int i = (dAddr & 0xc) >> 2;
                    int slot = (i == 0) ? 0 : ((i == 1) ? 1 : ((i == 2) ? 2 : 3));
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        dData = Math.Min(dData + nowYM2612FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2612[Chip.Number][dPort * 3 + ch] ? 127 : dData;
                        //if (nowYM2612FadeoutVol[Chip.Number] != 0)
                        //{
                            //log.Write(string.Format("fv{0}", nowYM2612FadeoutVol[Chip.Number]));
                            //log.Write(string.Format("ddata{0}", dData));
                        //}
                    }
                }
            }

            if ((dAddr & 0xf0) == 0xb0)//AL
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                if (ch != 3 && maskFMChYM2612[Chip.Number][ch])
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
                        if ((algM[al] & (1 << slot)) > 0)
                        {
                            //setYM2612Register(Counter,Chip.Number, dPort, 0x40 + ch + slot * 4, fmRegisterYM2612[Chip.Number][dPort][0x40 + ch]);
                        }
                    }
                }
            }

            if (dPort == 0 && dAddr == 0x2a)
            {
                if (maskFMChYM2612[Chip.Number][5]) dData = 0x80;
            }

            //if(dPort==0 && dAddr == 0x4c)
            //{
            //    log.Write(string.Format("Counter {0}  Ch1 op4 ddata{1}", Counter, dData));
            //    if (dData < oldddata)
            //    {
            //        ;
            //    }
            //    oldddata = dData;
            //    oldpddata = pddata;
            //}



            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    //仮想音源の処理
            //    if (ctYM2612[Chip.Number].UseScci)
            //    {
            //        //Scciを使用する場合でも
            //        //PCM(6Ch)だけエミュで発音するとき
            //        if (ctYM2612[Chip.Number].OnlyPCMEmulation)
            //        {
            //            //if (dPort == 0 && dAddr == 0x2b)
            //            //{
            //            //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //}
            //            //else if (dPort == 0 && dAddr == 0x2a)
            //            //{
            //            //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //}
            //            //else if (dPort == 1 && dAddr == 0xb6)
            //            //{
            //            //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //}
            //        }
            //    }
            //    //else
            //    //{
            //    //    //エミュを使用する場合のみMDSoundへデータを送る
            //    //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //    //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //    //}
            //}
            //else
            //{

            //    //実音源(Scci)

            //    if (scYM2612[Chip.Number] == null) return;

            //    //PCM(6Ch)だけエミュで発音するとき
            //    if (ctYM2612[Chip.Number].OnlyPCMEmulation)
            //    {
            //        //アドレスを調べてPCMにはデータを送らない
            //        if (dPort == 0 && dAddr == 0x2b)
            //        {
            //            //scYM2612[Chip.Number].setRegister(dPort * 0x100 + dAddr, dData);
            //            Chip.Model = EnmModel.VirtualModel;
            //        }
            //        else if (dPort == 0 && dAddr == 0x2a)
            //        {
            //            Chip.Model = EnmModel.VirtualModel;
            //        }
            //        else
            //        {
            //            //scYM2612[Chip.Number].setRegister(dPort * 0x100 + dAddr, dData);
            //        }
            //    }
            //    //else
            //    //{
            //    //    //Scciへデータを送る
            //    //    scYM2612[Chip.Number].setRegister(dPort * 0x100 + dAddr, dData);
            //    //}
            //}

        }

        public void YM2612SetRegister(long Counter,int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Model = YM2612[ChipID].Model; 
            dummyChip.Delay = YM2612[ChipID].Delay;
            dummyChip.Device = YM2612[ChipID].Device;
            dummyChip.Number = YM2612[ChipID].Number;
            dummyChip.Use = YM2612[ChipID].Use;

            if (dPort == 0x00 && (dAddr == 0x2a || dAddr == 0x2b) && ctYM2612[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmModel.VirtualModel;
            }

            enq(Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2612SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, YM2612[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2612SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2612MakeSoftReset(chipID);
            YM2612SetRegister(Counter, chipID, data.ToArray());
        }

        public void YM2612SetMask(long Counter, int chipID, int ch, bool mask)
        {
            maskFMChYM2612[chipID][ch] = mask;

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            YM2612SetRegister(Counter, (byte)chipID, p, 0x40 + c, fmRegisterYM2612[chipID][p][0x40 + c]);
            YM2612SetRegister(Counter, (byte)chipID, p, 0x44 + c, fmRegisterYM2612[chipID][p][0x44 + c]);
            YM2612SetRegister(Counter, (byte)chipID, p, 0x48 + c, fmRegisterYM2612[chipID][p][0x48 + c]);
            YM2612SetRegister(Counter, (byte)chipID, p, 0x4c + c, fmRegisterYM2612[chipID][p][0x4c + c]);

        }

        public void YM2612WriteClock(byte chipID, int clock)
        {
            if (scYM2612 != null && scYM2612[chipID] != null)
            {
                scYM2612[chipID].dClock = scYM2612[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2612SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2612.Length; i++)
            {
                if (!YM2612[i].Use) continue;
                if (YM2612[i].Model == EnmModel.VirtualModel) continue;
                if (nowYM2612FadeoutVol[i] == v) continue;

                nowYM2612FadeoutVol[i] = v;

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        YM2612SetRegister(Counter, i, p, 0x40 + c, fmRegisterYM2612[i][p][0x40 + c]);
                        YM2612SetRegister(Counter, i, p, 0x44 + c, fmRegisterYM2612[i][p][0x44 + c]);
                        YM2612SetRegister(Counter, i, p, 0x48 + c, fmRegisterYM2612[i][p][0x48 + c]);
                        YM2612SetRegister(Counter, i, p, 0x4c + c, fmRegisterYM2612[i][p][0x4c + c]);
                        //if (c == 0 && p == 0)
                        //{
                        //    log.Write(string.Format("send Counter{0} Ch1 op4 ddata{1} v:{2}", Counter, fmRegisterYM2612[Chip.Number][p][0x4c], v));
                        //}
                    }
                }
            }
        }

        public List<PackData> YM2612MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
            }
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable
            data.Add(new PackData(YM2612[chipID], EnmDataType.Normal, 0x2a, 0x80, null)); // PCM 0

            return data;
        }

        public void YM2612SetSyncWait(byte chipID, int wait)
        {
            if (scYM2612[chipID] != null && ctYM2612[chipID].UseWait)
            {
                scYM2612[chipID].setRegister(-1, (int)(wait * (ctYM2612[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }



        private void SEGAPCMWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                try
                {
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        if (!ctSEGAPCM[Chip.Number].UseScci && ctSEGAPCM[Chip.Number].UseEmu)
                            mds.WriteSEGAPCM((byte)Chip.Number, (int)address, (byte)data);
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        if (scSEGAPCM[Chip.Number] != null)
                            scSEGAPCM[Chip.Number].setRegister(address, data);
                    }
                }
                catch
                {

                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteSEGAPCM((byte)Chip.Number, (int)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmModel.RealModel)
                        {
                            if (scSEGAPCM[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scSEGAPCM[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmModel.VirtualModel)
                            mds.WriteSEGAPCMPCMData((byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                        // ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
                        else
                        {
                            if (scSEGAPCM != null && scSEGAPCM[Chip.Number] != null)
                            {
                                // スタートアドレス設定
                                scSEGAPCM[Chip.Number].setRegister(0x10000, (byte)((uint)((object[])exData)[1]));
                                scSEGAPCM[Chip.Number].setRegister(0x10001, (byte)((uint)((object[])exData)[1] >> 8));
                                scSEGAPCM[Chip.Number].setRegister(0x10002, (byte)((uint)((object[])exData)[1] >> 16));
                                // データ転送
                                for (int cnt = 0; cnt < (uint)((object[])exData)[2]; cnt++)
                                {
                                    scSEGAPCM[Chip.Number].setRegister(0x10004, ((byte[])((object[])exData)[3])[(uint)((object[])exData)[4] + cnt]);
                                }
                                scSEGAPCM[Chip.Number].setRegister(0x10006, (int)(uint)((object[])exData)[0]);

                                realChip.SendData();
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void SEGAPCMSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctSEGAPCM == null) return;

            if (Chip.Number == 0) chipLED.PriSPCM = 2;
            else chipLED.SecSPCM = 2;

            pcmRegisterSEGAPCM[Chip.Number][Address & 0x1ff] = (byte)dData;

            if (
                (Chip.Model == EnmModel.VirtualModel && (ctSEGAPCM[Chip.Number] == null || !ctSEGAPCM[Chip.Number].UseScci))
                || (Chip.Model == EnmModel.RealModel && (scSEGAPCM != null && scSEGAPCM[Chip.Number] != null))
                )
            {

                if ((Address & 0x87) == 0x86)
                {
                    byte ch = (byte)((Address >> 3) & 0xf);
                    if ((dData & 0x01) == 0) pcmKeyOnSEGAPCM[Chip.Number][ch] = true;
                }

                if (Address < 0x86 && ((Address & 0x03) == 2 || (Address & 0x03) == 3))
                {
                    int d = SEGAPCMNowFadeoutVol[Chip.Number];// >> 3;
                    dData = Math.Max(dData - d, 0);
                }
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctSEGAPCM[Chip.Number].UseScci)
            //        mds.WriteSEGAPCM((byte)Chip.Number, Address, (byte)dData);
            //    //System.Console.WriteLine("ChipID={0} Offset={1:X} Data={2:X} ", ChipID, Offset, Data);
            //}
            //else
            //{
            //    if (scSEGAPCM != null && scSEGAPCM[Chip.Number] != null) scSEGAPCM[Chip.Number].setRegister(Address, (byte)dData);
            //}
        }

        public void SEGAPCMSetRegister(long Counter, int ChipID, int dAddr, int dData)
        {
            enq(Counter, SEGAPCM[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void SEGAPCMSetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, SEGAPCM[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void SEGAPCMWritePCMData(long Counter,byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(Counter, SEGAPCM[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });

            //EnmModel model = EnmModel.VirtualModel;
            //if (chipID == 0) chipLED.PriSPCM = 2;
            //else chipLED.SecSPCM = 2;

            //if (model == EnmModel.VirtualModel)
            //{
            //    mds.WriteSEGAPCMPCMData(chipID, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            //}
            //else
            //{
            //    if (scSEGAPCM != null && scSEGAPCM[chipID] != null)
            //    {
            //        // スタートアドレス設定
            //        scSEGAPCM[chipID].setRegister(0x10000, (byte)(DataStart));
            //        scSEGAPCM[chipID].setRegister(0x10001, (byte)(DataStart >> 8));
            //        scSEGAPCM[chipID].setRegister(0x10002, (byte)(DataStart >> 16));
            //        // データ転送
            //        for (int cnt = 0; cnt < DataLength; cnt++)
            //        {
            //            scSEGAPCM[chipID].setRegister(0x10004, romdata[SrcStartAdr + cnt]);
            //        }
            //        scSEGAPCM[chipID].setRegister(0x10006, (int)ROMSize);

            //        realChip.SendData();
            //    }
            //}
        }

        public void SEGAPCMSoftReset(long Counter, int ChipID)
        {
            List<PackData> data = SEGAPCMMakeSoftReset(ChipID);
            SEGAPCMSetRegister(Counter, ChipID, data.ToArray());
        }

        public List<PackData> SEGAPCMMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            for (i = 0; i < 16; i++)
            {
                data.Add(new PackData(SEGAPCM[chipID], EnmDataType.Normal, (i * 8) + 0x86, 0x01, null));// KeyOff
                data.Add(new PackData(SEGAPCM[chipID], EnmDataType.Normal, (i * 8) + 0x02, 0x00, null));// L vol
                data.Add(new PackData(SEGAPCM[chipID], EnmDataType.Normal, (i * 8) + 0x03, 0x00, null));// R vol
            }

            return data;
        }

        public void SEGAPCMWriteClock(byte chipID, int clock)
        {
            if (scSEGAPCM != null && scSEGAPCM[chipID] != null)
            {
                scSEGAPCM[chipID].setRegister(0x10005, (int)clock);
                scSEGAPCM[chipID].dClock = scSEGAPCM[chipID].SetMasterClock((uint)clock);
                scSEGAPCM[chipID].mul = (double)scSEGAPCM[chipID].dClock / (double)clock;
            }
        }

        public void SEGAPCMSetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < SEGAPCM.Length; i++)
            {
                if (!SEGAPCM[i].Use) continue;
                if (SEGAPCM[i].Model == EnmModel.VirtualModel) continue;
                if (SEGAPCMNowFadeoutVol[i] >> 4 == v >> 4) continue;

                SEGAPCMNowFadeoutVol[i] = v;
                for (int c = 0; c < 16; c++)
                {
                    SEGAPCMSetRegister(Counter, i, (c * 8) + 2, pcmRegisterSEGAPCM[i][(c * 8) + 2]);
                    SEGAPCMSetRegister(Counter, i, (c * 8) + 3, pcmRegisterSEGAPCM[i][(c * 8) + 3]);
                }
            }
        }



        private void SN76489WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmModel.VirtualModel)
                {
                    if (!ctSN76489[Chip.Number].UseScci)
                    {
                        if (ctSN76489[Chip.Number].UseEmu)
                        {
                            if (address != 0x100)
                            {
                                mds.WriteSN76489((byte)Chip.Number, (byte)data);
                            }
                            else
                            {
                                mds.WriteSN76489GGPanning((byte)Chip.Number, (byte)data);
                            }
                        }
                    }
                }
                if (Chip.Model == EnmModel.RealModel)
                {
                    if (scSN76489[Chip.Number] != null)
                    {
                        scSN76489[Chip.Number].setRegister(address, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            if (dat.Address != 0x100)
                            {
                                mds.WriteSN76489((byte)dat.Chip.Number, (byte)dat.Data);
                            }
                            else
                            {
                                mds.WriteSN76489GGPanning((byte)dat.Chip.Number, (byte)dat.Data);
                            }
                    }
                    if (Chip.Model == EnmModel.RealModel)
                    {
                        foreach (PackData dat in pdata)
                        {
                            scSN76489[dat.Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void SN76489SetRegisterGGpanning(long Counter, int ChipID, int dData)
        {
            enq(Counter, SN76489[ChipID], EnmDataType.Normal, 0x100, dData, null);

            //EnmModel model = EnmModel.VirtualModel;
            //if (ctSN76489 == null) return;

            //if (chipID == 0) chipLED.PriDCSG = 2;
            //else chipLED.SecDCSG = 2;

            //if (model == EnmModel.RealModel)
            //{
            //    if (ctSN76489[chipID].UseScci)
            //    {
            //        if (scSN76489[chipID] == null) return;
            //    }
            //}
            //else
            //{
            //    if (!ctSN76489[chipID].UseScci && ctSN76489[chipID].UseEmu)
            //    {
            //        mds.WriteSN76489GGPanning((byte)chipID, (byte)dData);
            //        sn76489RegisterGGPan[chipID] = dData;
            //    }
            //}
        }

        public void SN76489SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctSN76489 == null) return;

            if (Chip.Number == 0) chipLED.PriDCSG = 2;
            else chipLED.SecDCSG = 2;

            if (Address == 0x100)
            {
                SN76489RegisterGGPan[Chip.Number] = dData;
                return;
            }

            SN76489Write(Chip.Number, dData);

            if ((dData & 0x90) == 0x90)
            {
                SN76489Vol[Chip.Number][(dData & 0x60) >> 5][0] = (15 - (dData & 0xf)) * ((SN76489RegisterGGPan[Chip.Number] >> (((dData & 0x60) >> 5) + 4)) & 0x1);
                SN76489Vol[Chip.Number][(dData & 0x60) >> 5][1] = (15 - (dData & 0xf)) * ((SN76489RegisterGGPan[Chip.Number] >> ((dData & 0x60) >> 5)) & 0x1);

                int v = dData & 0xf;
                v = v + SN76489NowFadeoutVol[Chip.Number];
                v += SN76489MaskCh[Chip.Number][(dData & 0x60) >> 5] ? 15 : 0;
                v = Math.Min(v, 15);
                dData = (dData & 0xf0) | v;
            }

            //if (Chip.Model == EnmModel.RealModel)
            //{
                //if (ctSN76489[Chip.Number].UseScci)
                //{
                //    if (scSN76489[Chip.Number] == null) return;
                //    scSN76489[Chip.Number].setRegister(0, dData);
                //}
            //}
            //else
            //{
                //if (!ctSN76489[Chip.Number].UseScci && ctSN76489[Chip.Number].UseEmu)
                //{
                //    mds.WriteSN76489((byte)Chip.Number, (byte)dData);
                //}
            //}
        }

        public void SN76489SetRegister(long Counter, int ChipID, int dData)
        {
            enq(Counter, SN76489[ChipID], EnmDataType.Normal, 0, dData, null);
        }

        public void SN76489SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(Counter, SN76489[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void SN76489SoftReset(long Counter, int chipID)
        {
            List<PackData> data = SN76489MakeSoftReset(chipID);
            SN76489SetRegister(Counter, chipID, data.ToArray());
        }

        public void SN76489SetMask(int chipID, int ch, bool mask)
        {
            SN76489MaskCh[chipID][ch] = mask;
        }

        public void SN76489WriteClock(byte chipID, int clock)
        {
            if (scSN76489 != null && scSN76489[chipID] != null)
            {
                scSN76489[chipID].dClock = scSN76489[chipID].SetMasterClock((uint)clock);
            }
        }

        public void SN76489SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < SN76489.Length; i++)
            {
                if (!SN76489[i].Use) continue;
                if (SN76489[i].Model == EnmModel.VirtualModel) continue;
                if (SN76489NowFadeoutVol[i] == v) continue;

                SN76489NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 4; c++)
                {
                    SN76489SetRegister(Counter,i, 0x90 + (c << 5) + SN76489Register[i][1 + (c << 1)]);
                }
            }
        }

        public List<PackData> SN76489MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();

            //vol off
            data.Add(new PackData(SN76489[chipID], EnmDataType.Normal, 0, 0x9f, null));
            data.Add(new PackData(SN76489[chipID], EnmDataType.Normal, 0, 0xbf, null));
            data.Add(new PackData(SN76489[chipID], EnmDataType.Normal, 0, 0xdf, null));
            data.Add(new PackData(SN76489[chipID], EnmDataType.Normal, 0, 0xff, null));

            return data;
        }

        private void SN76489Write(int chipID, int data)
        {
            if ((data & 0x80) > 0)
            {
                /* Latch/data byte  %1 cc t dddd */
                LatchedRegister[chipID] = (data >> 4) & 0x07;
                SN76489Register[chipID][LatchedRegister[chipID]] =
                    (SN76489Register[chipID][LatchedRegister[chipID]] & 0x3f0) /* zero low 4 bits */
                    | (data & 0xf);                            /* and replace with data */
            }
            else
            {
                /* Data byte        %0 - dddddd */
                if ((LatchedRegister[chipID] % 2) == 0 && (LatchedRegister[chipID] < 5))
                    /* Tone register */
                    SN76489Register[chipID][LatchedRegister[chipID]] =
                        (SN76489Register[chipID][LatchedRegister[chipID]] & 0x00f) /* zero high 6 bits */
                        | ((data & 0x3f) << 4);                 /* and replace with data */
                else
                    /* Other register */
                    SN76489Register[chipID][LatchedRegister[chipID]] = data & 0x0f; /* Replace with data */
            }
            switch (LatchedRegister[chipID])
            {
                case 0:
                case 2:
                case 4: /* Tone channels */
                    if (SN76489Register[chipID][LatchedRegister[chipID]] == 0)
                        SN76489Register[chipID][LatchedRegister[chipID]] = 1; /* Zero frequency changed to 1 to avoid div/0 */
                    break;
                case 6: /* Noise */
                    NoiseFreq[chipID] = 0x10 << (SN76489Register[chipID][6] & 0x3); /* set noise signal generator frequency */
                    break;
            }
        }

        public void SN76489SetSyncWait(byte chipID, int wait)
        {
            if (scSN76489 != null && ctSN76489[chipID].UseWait)
            {
                scSN76489[chipID].setRegister(-1, (int)(wait * (ctSN76489[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }




        public void setMaskYM3526(int chipID, int ch, bool mask)
        {
            maskFMChYM3526[chipID][ch] = mask;
        }

        public void setMaskY8950(int chipID, int ch, bool mask)
        {
            maskFMChY8950[chipID][ch] = mask;
        }

        public void setMaskYM3812(int chipID, int ch, bool mask)
        {
            maskFMChYM3812[chipID][ch] = mask;
        }

        public void setMaskYMF262(int chipID, int ch, bool mask)
        {
            maskFMChYMF262[chipID][YMF278BCh[ch]] = mask;
        }

        public void setMaskYMF278B(int chipID, int ch, bool mask)
        {
            maskFMChYMF278B[chipID][YMF278BCh[ch]] = mask;
        }

        public void setMaskC352(int chipID, int ch, bool mask)
        {
            maskChC352[chipID][ch] = mask;
        }

        public void setMaskOKIM6258(int chipID, bool mask)
        {
            maskOKIM6258[chipID] = mask;

            writeOKIM6258((byte)chipID, 0, 1);
        }

        public void setNESMask(int chipID, int ch)
        {
            switch (ch)
            {
                case 0:
                case 1:
                    nsfAPUmask |= 1 << ch;
                    if (nes_apu != null) nes_apu.SetMask(nsfAPUmask);
                    break;
                case 2:
                case 3:
                case 4:
                    nsfDMCmask |= 1 << (ch - 2);
                    if (nes_dmc != null) nes_dmc.SetMask(nsfDMCmask);
                    break;
            }
            mds.setNESMask(chipID, ch);
        }

        public void resetNESMask(int chipID, int ch)
        {
            switch (ch)
            {
                case 0:
                case 1:
                    nsfAPUmask &= ~(1 << ch);
                    if (nes_apu != null) nes_apu.SetMask(nsfAPUmask);
                    break;
                case 2:
                case 3:
                case 4:
                    nsfDMCmask &= ~(1 << (ch - 2));
                    if (nes_dmc != null) nes_dmc.SetMask(nsfDMCmask);
                    break;
            }
            mds.resetNESMask(chipID, ch);
        }

        public void setFDSMask(int chipID)
        {
            nsfFDSmask |= 1;
            if (nes_fds != null) nes_fds.SetMask(nsfFDSmask);
            mds.setFDSMask(chipID);
        }

        public void resetFDSMask(int chipID)
        {
            nsfFDSmask &= ~1;
            if (nes_fds != null) nes_fds.SetMask(nsfFDSmask);
            mds.resetFDSMask(chipID);
        }

        public void setMMC5Mask(int chipID, int ch)
        {
            nsfMMC5mask |= 1 << ch;
            if (nes_mmc5 != null) nes_mmc5.SetMask(nsfMMC5mask);
        }

        public void resetMMC5Mask(int chipID, int ch)
        {
            nsfMMC5mask &= ~(1 << ch);
            if (nes_mmc5 != null) nes_mmc5.SetMask(nsfMMC5mask);
        }

        public void setVRC7Mask(int chipID, int ch)
        {
            nsfVRC7mask |= 1 << ch;
            if (nes_vrc7 != null) nes_vrc7.SetMask(nsfVRC7mask);
        }

        public void resetVRC7Mask(int chipID, int ch)
        {
            nsfVRC7mask &= ~(1 << ch);
            if (nes_vrc7 != null) nes_vrc7.SetMask(nsfVRC7mask);
        }

        public void setK051649Mask(int chipID, int ch)
        {
            maskChK051649[chipID][ch] = true;
            writeK051649((byte)chipID, (3 << 1) | 1, K051649tKeyOnOff[chipID]);
        }

        public void resetK051649Mask(int chipID, int ch)
        {
            maskChK051649[chipID][ch] = false;
            writeK051649((byte)chipID, (3 << 1) | 1, K051649tKeyOnOff[chipID]);
        }



        public void setDMGRegister(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriDMG = 2;
            else chipLED.SecDMG = 2;

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                mds.WriteDMG((byte)chipID, (byte)dAddr, (byte)dData);
                //}
            }
            else
            {
                //if (scNES[chipID] == null) return;

                //scNES[chipID].setRegister(dAddr, dData);
            }
        }

        public void setNESRegister(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                mds.WriteNES((byte)chipID, (byte)dAddr, (byte)dData);
                //}
            }
            else
            {
                //if (scNES[chipID] == null) return;

                //scNES[chipID].setRegister(dAddr, dData);
            }
        }

        public byte[] getNESRegister(int chipID)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                return mds.ReadNES((byte)chipID);
                //}
            }
            else
            {
                return null;
                //if (scNES[chipID] == null) return;

                //scNES[chipID].setRegister(dAddr, dData);
            }
        }

        public MDSound.np.np_nes_fds.NES_FDS getFDSRegister(int chipID, EnmModel model)
        {
            if (chipID == 0) chipLED.PriFDS = 2;
            else chipLED.SecFDS = 2;

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                return mds.ReadFDS((byte)chipID);
                //}
            }
            else
            {
                return null;
                //if (scFDS[chipID] == null) return;

                //scFDS[chipID].setRegister(dAddr, dData);
            }

        }

        public MDSound.np.chip.nes_mmc5 getMMC5Register(int chipID, EnmModel model)
        {
            if (chipID == 0) chipLED.PriMMC5 = 2;
            else chipLED.SecMMC5 = 2;

            if (model == EnmModel.VirtualModel)
            {
                return null;// mds.ReadMMC5((byte)chipID);
            }
            else
            {
                return null;
            }

        }

        public byte[] getVRC7Register(int chipID)
        {
            if (nes_vrc7 == null) return null;
            if (chipID != 0) return null;

            return nes_vrc7.GetVRC7regs();
        }

        public void setMultiPCMRegister(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteMultiPCM((byte)chipID, (byte)dAddr, (byte)dData);
            }
            else
            {
            }
        }

        public void setMultiPCMSetBank(int chipID, int dCh, int dAddr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteMultiPCMSetBank((byte)chipID, (byte)dCh, (int)dAddr);
            }
            else
            {
            }
        }

        public void setQSoundRegister(int chipID, byte mm, byte ll, byte rr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriQsnd = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteQSound((byte)chipID, 0, mm);
                mds.WriteQSound((byte)chipID, 1, ll);
                mds.WriteQSound((byte)chipID, 2, rr);
            }
            else
            {
            }
        }

        public void setGA20Register(int chipID, Int32 Adr, byte Dat)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipID == 0) chipLED.PriGA20 = 2;
            else chipLED.SecGA20 = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteGA20((byte)chipID, (byte)Adr, Dat);
            }
            else
            {
            }
        }

        public void setYM3812Register(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            //if (ctYM3812 == null) return;

            if (chipID == 0) chipLED.PriOPL2 = 2;
            else chipLED.SecOPL2 = 2;

            if (model == EnmModel.VirtualModel)
            {
                fmRegisterYM3812[chipID][dAddr] = dData;

                if (dAddr >= 0xb0 && dAddr <= 0xb8)
                {
                    int ch = dAddr - 0xb0;
                    int k = (dData >> 5) & 1;
                    if (k == 0)
                    {
                        kiYM3812[chipID].Off[ch] = true;
                    }
                    else
                    {
                        if (kiYM3812[chipID].Off[ch]) kiYM3812[chipID].On[ch] = true;
                        kiYM3812[chipID].Off[ch] = false;
                    }
                    if (maskFMChYM3812[chipID][ch]) dData &= 0x1f;
                }

                if (dAddr == 0xbd)
                {

                    for (int c = 0; c < 5; c++)
                    {
                        if ((dData & (0x10 >> c)) == 0)
                        {
                            kiYM3812[chipID].Off[c + 9] = true;
                        }
                        else
                        {
                            if (kiYM3812[chipID].Off[c + 9]) kiYM3812[chipID].On[c + 9] = true;
                            kiYM3812[chipID].Off[c + 9] = false;
                        }
                    }

                    if (maskFMChYM3812[chipID][9]) dData &= 0xef;
                    if (maskFMChYM3812[chipID][10]) dData &= 0xf7;
                    if (maskFMChYM3812[chipID][11]) dData &= 0xfb;
                    if (maskFMChYM3812[chipID][12]) dData &= 0xfd;
                    if (maskFMChYM3812[chipID][13]) dData &= 0xfe;

                }

            }

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctYM3812[chipID].UseScci)
                {
                    mds.WriteYM3812((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
            }

        }

        public void setHuC6280Register(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ctHuC6280 == null) return;

            if (chipID == 0) chipLED.PriHuC = 2;
            else chipLED.SecHuC = 2;

            if (model == EnmModel.VirtualModel)
            {
                if (!ctHuC6280[chipID].UseScci)
                {
                    //System.Console.WriteLine("chipID:{0} Adr:{1} Dat:{2}", chipID, dAddr, dData);
                    mds.WriteHuC6280((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                //if (scHuC6280[chipID] == null) return;
            }
        }

        public void setYMF262Register(int chipID, int dPort, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ctYMF262 == null) return;

            if (chipID == 0) chipLED.PriOPL3 = 2;
            else chipLED.SecOPL3 = 2;

            if (model == EnmModel.VirtualModel)
            {
                fmRegisterYMF262[chipID][dPort][dAddr] = dData;

                if (dAddr >= 0xb0 && dAddr <= 0xb8)
                {
                    int ch = dAddr - 0xb0 + dPort * 9;
                    int k = (dData >> 5) & 1;
                    if (k == 0)
                    {
                        fmRegisterYMF262FM[chipID] &= ~(1 << ch);
                    }
                    else
                    {
                        fmRegisterYMF262FM[chipID] |= (1 << ch);
                    }
                    fmRegisterYMF262FM[chipID] &= 0x3ffff;
                    if (maskFMChYMF262[chipID][ch]) dData &= 0x1f;
                }

                if (dAddr == 0xbd && dPort == 0)
                {
                    if ((fmRegisterYMF262RyhthmB[chipID] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYMF262Ryhthm[chipID] |= 0x10;
                    if ((fmRegisterYMF262RyhthmB[chipID] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYMF262Ryhthm[chipID] |= 0x08;
                    if ((fmRegisterYMF262RyhthmB[chipID] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYMF262Ryhthm[chipID] |= 0x04;
                    if ((fmRegisterYMF262RyhthmB[chipID] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYMF262Ryhthm[chipID] |= 0x02;
                    if ((fmRegisterYMF262RyhthmB[chipID] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYMF262Ryhthm[chipID] |= 0x01;
                    fmRegisterYMF262RyhthmB[chipID] = dData;

                    if (maskFMChYMF262[chipID][18]) dData &= 0xef;
                    if (maskFMChYMF262[chipID][19]) dData &= 0xf7;
                    if (maskFMChYMF262[chipID][20]) dData &= 0xfb;
                    if (maskFMChYMF262[chipID][21]) dData &= 0xfd;
                    if (maskFMChYMF262[chipID][22]) dData &= 0xfe;

                }

                if (!ctYMF262[chipID].UseScci)
                {
                    mds.WriteYMF262((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYMF262[chipID] == null) return;
                scYMF262[chipID].setRegister(dPort * 0x100 + dAddr, dData);
            }

        }

        public void setYMF271Register(int chipID, int dPort, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ctYMF271 == null) return;

            if (chipID == 0) chipLED.PriOPX = 2;
            else chipLED.SecOPX = 2;

            if (model == EnmModel.VirtualModel) fmRegisterYMF271[chipID][dPort][dAddr] = dData;

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYMF271[chipID].UseScci)
                {
                    mds.WriteYMF271((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYMF271[chipID] == null) return;
                scYMF271[chipID].setRegister(dPort * 0x100 + dAddr, dData);
            }

        }

        public void setYMF278BRegister(int chipID, int dPort, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;

            if (ctYMF278B == null) return;

            if (chipID == 0) chipLED.PriOPL4 = 2;
            else chipLED.SecOPL4 = 2;

            if (model == EnmModel.VirtualModel)
            {
                fmRegisterYMF278B[chipID][dPort][dAddr] = dData;

                //if (dPort == 2)
                //{
                //Console.WriteLine("p=2:adr{0:x02} dat{1:x02}", dAddr, dData);
                //}

                if (dAddr >= 0xb0 && dAddr <= 0xb8)
                {
                    int ch = dAddr - 0xb0 + dPort * 9;
                    int k = (dData >> 5) & 1;
                    if (k == 0)
                    {
                        fmRegisterYMF278BFM[chipID] &= ~(1 << ch);
                    }
                    else
                    {
                        fmRegisterYMF278BFM[chipID] |= (1 << ch);
                    }
                    fmRegisterYMF278BFM[chipID] &= 0x3ffff;
                    if (maskFMChYMF278B[chipID][ch]) dData &= 0x1f;
                }

                if (dAddr == 0xbd && dPort == 0)
                {
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x10;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x08;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x04;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x02;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x01;
                    fmRegisterYMF278BRyhthmB[chipID] = dData;

                    if (maskFMChYMF278B[chipID][18]) dData &= 0xef;
                    if (maskFMChYMF278B[chipID][19]) dData &= 0xf7;
                    if (maskFMChYMF278B[chipID][20]) dData &= 0xfb;
                    if (maskFMChYMF278B[chipID][21]) dData &= 0xfd;
                    if (maskFMChYMF278B[chipID][22]) dData &= 0xfe;
                }

                if (dPort == 2 && (dAddr >= 0x68 && dAddr <= 0x7f))
                {
                    int k = dData >> 7;
                    if (k == 0)
                    {
                        fmRegisterYMF278BPCM[chipID][dAddr - 0x68] = 2;
                    }
                    else
                    {
                        fmRegisterYMF278BPCM[chipID][dAddr - 0x68] = 1;
                    }
                    if (maskFMChYMF278B[chipID][dAddr - 0x68 + 23]) dData &= 0x7f;
                }

            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYMF278B[chipID].UseScci)
                {
                    mds.WriteYMF278B((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYMF278B[chipID] == null) return;
                scYMF278B[chipID].setRegister(dPort * 0x100 + dAddr, dData);
            }

        }

        public void setYM3526Register(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ctYM3526 == null) return;

            if (chipID == 0) chipLED.PriOPL = 2;
            else chipLED.SecOPL = 2;

            if (model == EnmModel.VirtualModel)
            {
                fmRegisterYM3526[chipID][dAddr] = dData;
                if (dAddr >= 0xb0 && dAddr <= 0xb8)
                {
                    int ch = dAddr - 0xb0;
                    int k = (dData >> 5) & 1;
                    if (k == 0)
                    {
                        kiYM3526[chipID].On[ch] = false;
                        kiYM3526[chipID].Off[ch] = true;
                    }
                    else
                    {
                        kiYM3526[chipID].On[ch] = true;
                    }
                    if (maskFMChYM3526[chipID][ch]) dData &= 0x1f;
                }

                if (dAddr == 0xbd)
                {

                    for (int c = 0; c < 5; c++)
                    {
                        if ((dData & (0x10 >> c)) == 0)
                        {
                            kiYM3526[chipID].Off[c + 9] = true;
                        }
                        else
                        {
                            if (kiYM3526[chipID].Off[c + 9]) kiYM3526[chipID].On[c + 9] = true;
                            kiYM3526[chipID].Off[c + 9] = false;
                        }
                    }

                    if (maskFMChYM3526[chipID][9]) dData &= 0xef;
                    if (maskFMChYM3526[chipID][10]) dData &= 0xf7;
                    if (maskFMChYM3526[chipID][11]) dData &= 0xfb;
                    if (maskFMChYM3526[chipID][12]) dData &= 0xfd;
                    if (maskFMChYM3526[chipID][13]) dData &= 0xfe;

                }

            }

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctYM3526[chipID].UseScci)
                {
                    mds.WriteYM3526((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
            }

        }

        public void setY8950Register(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ctY8950 == null) return;

            if (chipID == 0) chipLED.PriY8950 = 2;
            else chipLED.SecY8950 = 2;

            if (model == EnmModel.VirtualModel)
            {
                fmRegisterY8950[chipID][dAddr] = dData;
                if (dAddr >= 0xb0 && dAddr <= 0xb8)
                {
                    int ch = dAddr - 0xb0;
                    int k = (dData >> 5) & 1;
                    if (k == 0)
                    {
                        kiY8950[chipID].On[ch] = false;
                        kiY8950[chipID].Off[ch] = true;
                    }
                    else
                    {
                        kiY8950[chipID].On[ch] = true;
                    }
                    if (maskFMChY8950[chipID][ch]) dData &= 0x1f;
                }

                if (dAddr == 0xbd)
                {

                    for (int c = 0; c < 5; c++)
                    {
                        if ((dData & (0x10 >> c)) == 0)
                        {
                            kiY8950[chipID].Off[c + 9] = true;
                        }
                        else
                        {
                            if (kiY8950[chipID].Off[c + 9]) kiY8950[chipID].On[c + 9] = true;
                            kiY8950[chipID].Off[c + 9] = false;
                        }
                    }

                    if (maskFMChY8950[chipID][9]) dData &= 0xef;
                    if (maskFMChY8950[chipID][10]) dData &= 0xf7;
                    if (maskFMChY8950[chipID][11]) dData &= 0xfb;
                    if (maskFMChY8950[chipID][12]) dData &= 0xfd;
                    if (maskFMChY8950[chipID][13]) dData &= 0xfe;

                }

                //ADPCM
                if (dAddr == 0x07)
                {
                    int k = (dData & 0x80);
                    if (k == 0)
                    {
                        kiY8950[chipID].On[14] = false;
                        kiY8950[chipID].Off[14] = true;
                    }
                    else
                    {
                        kiY8950[chipID].On[14] = true;
                    }
                    if (maskFMChY8950[chipID][14]) dData &= 0x7f;
                }

            }

            if (model == EnmModel.VirtualModel)
            {
                //if (!ctY8950[chipID].UseScci)
                {
                    mds.WriteY8950((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
            }

        }

        public void setYMZ280BRegister(int chipID, int dAddr, int dData)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ctYMZ280B == null) return;

            if (chipID == 0) chipLED.PriYMZ = 2;
            else chipLED.SecYMZ = 2;

            if (model == EnmModel.VirtualModel) YMZ280BRegister[chipID][dAddr] = dData;

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYMZ280B[chipID].UseScci)
                {
                    mds.WriteYMZ280B((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYMZ280B[chipID] == null) return;
                scYMZ280B[chipID].setRegister(dAddr, dData);
            }

        }

        public void writeRF5C68PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            //if (chipid == 0) chipLED.PriRF5C = 2;
            //else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C68PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeRF5C68(byte chipid, byte adr, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteRF5C68(chipid, adr, data);
            }
        }

        public void writeRF5C68MemW(byte chipid, uint offset, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C68MemW(chipid, offset, data);
        }

        public void writeRF5C164PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C164PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeNESPCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteNESRam(chipid, (int)stAdr, (int)dataSize, vgmBuf, (int)vgmAdr);
        }

        public void writeRF5C164(byte chipid, byte adr, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteRF5C164(chipid, adr, data);
            }
        }

        public void writeRF5C164MemW(byte chipid, uint offset, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C164MemW(chipid, offset, data);
        }

        public void writePWM(byte chipid, byte adr, uint data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriPWM = 2;
            else chipLED.SecPWM = 2;

            if (model == EnmModel.VirtualModel)
                mds.WritePWM(chipid, adr, data);
        }

        public void writeK051649(byte chipid, uint adr, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriK051649 = 2;
            else chipLED.SecK051649 = 2;

            if (model == EnmModel.VirtualModel)
            {
                if ((adr & 1) != 0)
                {
                    if ((adr >> 1) == 3)//keyonoff
                    {
                        K051649tKeyOnOff[chipid] = data;
                        data &= (byte)(maskChK051649[chipid][0] ? 0xfe : 0xff);
                        data &= (byte)(maskChK051649[chipid][1] ? 0xfd : 0xff);
                        data &= (byte)(maskChK051649[chipid][2] ? 0xfb : 0xff);
                        data &= (byte)(maskChK051649[chipid][3] ? 0xf7 : 0xff);
                        data &= (byte)(maskChK051649[chipid][4] ? 0xef : 0xff);
                    }
                }
                mds.WriteK051649(chipid, (int)adr, data);
            }
        }

        public void writeK053260(byte chipid, uint adr, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK053260(chipid, (byte)adr, data);
        }

        public void writeK054539(byte chipid, uint adr, byte data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriK054539 = 2;
            else chipLED.SecK054539 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK054539(chipid, (int)adr, data);
        }

        public void writeK053260PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK053260PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeK054539PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriK054539 = 2;
            else chipLED.SecK054539 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK054539PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeQSoundPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriQsnd = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteQSoundPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeC352(byte chipid, uint adr, uint data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriC352 = 2;
            else chipLED.SecC352 = 2;

            pcmRegisterC352[chipid][adr] = (ushort)data;
            int c = (int)adr / 8;
            if (adr < 0x100 && (adr % 8) == 3 && maskChC352[chipid][adr / 8])
            {
                data &= 0xbfff;
            }
            if (model == EnmModel.VirtualModel)
                mds.WriteC352(chipid, adr, data);
        }

        public ushort[] readC352(byte chipid)
        {
            return mds.ReadC352Flag(chipid);
        }

        public void writeC352PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriC352 = 2;
            else chipLED.SecC352 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteC352PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeGA20PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriGA20 = 2;
            else chipLED.SecGA20 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteGA20PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeOKIM6258(byte ChipID, byte Port, byte Data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ChipID == 0) chipLED.PriOKI5 = 2;
            else chipLED.SecOKI5 = 2;

            if (Port == 0x00)
            {
                if ((Data & 0x2) != 0) okim6258Keyon[ChipID] = true;

                if (maskOKIM6258[ChipID])
                {
                    if ((Data & 0x2) != 0) return;
                }
            }
            if (Port == 0x1)
            {
                if (maskOKIM6258[ChipID]) return;
            }

            if (model == EnmModel.VirtualModel)
                mds.WriteOKIM6258(ChipID, Port, Data);
        }

        public void writeOKIM6295(byte ChipID, byte Port, byte Data)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (ChipID == 0) chipLED.PriOKI9 = 2;
            else chipLED.SecOKI9 = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteOKIM6295(ChipID, Port, Data);
                //System.Console.WriteLine("ChipID={0} Port={1:X} Data={2:X} ",ChipID,Port,Data);
            }
        }

        public void writeOKIM6295PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriOKI9 = 2;
            else chipLED.SecOKI9 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteOKIM6295PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeMultiPCMPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteMultiPCMPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMF271PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriOPX = 2;
            else chipLED.SecOPX = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteYMF271PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMF278BPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriOPL4 = 2;
            else chipLED.SecOPL4 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteYMF278BPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMZ280BPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriYMZ = 2;
            else chipLED.SecYMZ = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteYMZ280BPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeY8950PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmModel model = EnmModel.VirtualModel;
            if (chipid == 0) chipLED.PriY8950 = 2;
            else chipLED.SecY8950 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteY8950PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }


        //
        // 鍵盤のボリューム表示のため音量を取得する
        //

        /// <summary>
        /// ボリューム情報の更新
        /// </summary>
        public void updateVol()
        {
            volF--;
            if (volF > 0) return;

            volF = 1;

            for (int chipID = 0; chipID < 2; chipID++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (fmVolYM2612[chipID][i] > 0) { fmVolYM2612[chipID][i] -= 50; if (fmVolYM2612[chipID][i] < 0) fmVolYM2612[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2612[chipID][i] > 0) { fmCh3SlotVolYM2612[chipID][i] -= 50; if (fmCh3SlotVolYM2612[chipID][i] < 0) fmCh3SlotVolYM2612[chipID][i] = 0; }
                }
                for (int i = 0; i < 8; i++)
                {
                    if (YM2151FmVol[chipID][i] > 0) { YM2151FmVol[chipID][i] -= 50; if (YM2151FmVol[chipID][i] < 0) YM2151FmVol[chipID][i] = 0; }
                }
                for (int i = 0; i < 9; i++)
                {
                    if (fmVolYM2608[chipID][i] > 0) { fmVolYM2608[chipID][i] -= 50; if (fmVolYM2608[chipID][i] < 0) fmVolYM2608[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2608[chipID][i] > 0) { fmCh3SlotVolYM2608[chipID][i] -= 50; if (fmCh3SlotVolYM2608[chipID][i] < 0) fmCh3SlotVolYM2608[chipID][i] = 0; }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (fmVolYM2608Rhythm[chipID][i][0] > 0) { fmVolYM2608Rhythm[chipID][i][0] -= 50; if (fmVolYM2608Rhythm[chipID][i][0] < 0) fmVolYM2608Rhythm[chipID][i][0] = 0; }
                    if (fmVolYM2608Rhythm[chipID][i][1] > 0) { fmVolYM2608Rhythm[chipID][i][1] -= 50; if (fmVolYM2608Rhythm[chipID][i][1] < 0) fmVolYM2608Rhythm[chipID][i][1] = 0; }
                }

                if (fmVolYM2608Adpcm[chipID][0] > 0) { fmVolYM2608Adpcm[chipID][0] -= 50; if (fmVolYM2608Adpcm[chipID][0] < 0) fmVolYM2608Adpcm[chipID][0] = 0; }
                if (fmVolYM2608Adpcm[chipID][1] > 0) { fmVolYM2608Adpcm[chipID][1] -= 50; if (fmVolYM2608Adpcm[chipID][1] < 0) fmVolYM2608Adpcm[chipID][1] = 0; }

                for (int i = 0; i < 9; i++)
                {
                    if (fmVolYM2610[chipID][i] > 0) { fmVolYM2610[chipID][i] -= 50; if (fmVolYM2610[chipID][i] < 0) fmVolYM2610[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2610[chipID][i] > 0) { fmCh3SlotVolYM2610[chipID][i] -= 50; if (fmCh3SlotVolYM2610[chipID][i] < 0) fmCh3SlotVolYM2610[chipID][i] = 0; }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (fmVolYM2610Rhythm[chipID][i][0] > 0) { fmVolYM2610Rhythm[chipID][i][0] -= 50; if (fmVolYM2610Rhythm[chipID][i][0] < 0) fmVolYM2610Rhythm[chipID][i][0] = 0; }
                    if (fmVolYM2610Rhythm[chipID][i][1] > 0) { fmVolYM2610Rhythm[chipID][i][1] -= 50; if (fmVolYM2610Rhythm[chipID][i][1] < 0) fmVolYM2610Rhythm[chipID][i][1] = 0; }
                }

                if (fmVolYM2610Adpcm[chipID][0] > 0) { fmVolYM2610Adpcm[chipID][0] -= 50; if (fmVolYM2610Adpcm[chipID][0] < 0) fmVolYM2610Adpcm[chipID][0] = 0; }
                if (fmVolYM2610Adpcm[chipID][1] > 0) { fmVolYM2610Adpcm[chipID][1] -= 50; if (fmVolYM2610Adpcm[chipID][1] < 0) fmVolYM2610Adpcm[chipID][1] = 0; }

                for (int i = 0; i < 6; i++)
                {
                    if (fmVolYM2203[chipID][i] > 0) { fmVolYM2203[chipID][i] -= 50; if (fmVolYM2203[chipID][i] < 0) fmVolYM2203[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2203[chipID][i] > 0) { fmCh3SlotVolYM2203[chipID][i] -= 50; if (fmCh3SlotVolYM2203[chipID][i] < 0) fmCh3SlotVolYM2203[chipID][i] = 0; }
                }
            }

        }

        public int[] GetYM2151Volume(int chipID)
        {
            return YM2151FmVol[chipID];
        }

        public int[] GetYM2203Volume(int chipID)
        {
            return fmVolYM2203[chipID];
        }

        public int[] GetYM2608Volume(int chipID)
        {
            return fmVolYM2608[chipID];
        }

        public int[] GetYM2610Volume(int chipID)
        {
            return fmVolYM2610[chipID];
        }

        public int[] GetYM2612Volume(int chipID)
        {
            return fmVolYM2612[chipID];
        }

        public int[] GetYM2203Ch3SlotVolume(int chipID)
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2203[chipID];
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2608Ch3SlotVolume(int chipID)
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2608[chipID];
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2610Ch3SlotVolume(int chipID)
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2610[chipID];
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2612Ch3SlotVolume(int chipID)
        {
            return fmCh3SlotVolYM2612[chipID];
        }

        public int[][] GetYM2608RhythmVolume(int chipID)
        {
            return fmVolYM2608Rhythm[chipID];
        }

        public int[][] GetYM2610RhythmVolume(int chipID)
        {
            return fmVolYM2610Rhythm[chipID];
        }

        public int[] GetYM2608AdpcmVolume(int chipID)
        {
            return fmVolYM2608Adpcm[chipID];
        }

        public int[] GetYM2610AdpcmVolume(int chipID)
        {
            return fmVolYM2610Adpcm[chipID];
        }

        public int[][] GetPSGVolume(int chipID)
        {

            return SN76489Vol[chipID];

        }


        public ChipKeyInfo getYM2413KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiYM2413[chipID].Off.Length; ch++)
            {
                kiYM2413ret[chipID].Off[ch] = kiYM2413[chipID].Off[ch];
                kiYM2413ret[chipID].On[ch] = kiYM2413[chipID].On[ch];
                kiYM2413[chipID].On[ch] = false;
            }
            return kiYM2413ret[chipID];
        }

        public ChipKeyInfo getYM3526KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiYM3526[chipID].Off.Length; ch++)
            {
                kiYM3526ret[chipID].Off[ch] = kiYM3526[chipID].Off[ch];
                kiYM3526ret[chipID].On[ch] = kiYM3526[chipID].On[ch];
                kiYM3526[chipID].On[ch] = false;
            }
            return kiYM3526ret[chipID];
        }

        public ChipKeyInfo getY8950KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiY8950[chipID].Off.Length; ch++)
            {
                kiY8950ret[chipID].Off[ch] = kiY8950[chipID].Off[ch];
                kiY8950ret[chipID].On[ch] = kiY8950[chipID].On[ch];
                kiY8950[chipID].On[ch] = false;
            }
            return kiY8950ret[chipID];
        }

        public ChipKeyInfo getYM3812KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiYM3812[chipID].Off.Length; ch++)
            {
                kiYM3812ret[chipID].Off[ch] = kiYM3812[chipID].Off[ch];
                kiYM3812ret[chipID].On[ch] = kiYM3812[chipID].On[ch];
                kiYM3812[chipID].On[ch] = false;
            }
            return kiYM3812ret[chipID];
        }

        public ChipKeyInfo getVRC7KeyInfo(int chipID)
        {
            if (nes_vrc7 == null) return null;
            if (chipID != 0) return null;

            MDSound.np.chip.nes_vrc7.ChipKeyInfo ki = nes_vrc7.getVRC7KeyInfo(chipID);

            for (int ch = 0; ch < 6; ch++)
            {
                kiVRC7ret[chipID].On[ch] = ki.On[ch];
                kiVRC7ret[chipID].Off[ch] = ki.Off[ch];
            }
            return kiVRC7ret[chipID];
        }

        //public int getYM2413RyhthmKeyON(int chipID)
        //{
        //    int r = fmRegisterYM2413Ryhthm[chipID];
        //    fmRegisterYM2413Ryhthm[chipID] = 0;
        //    return r;
        //}

        public int getYMF262RyhthmKeyON(int chipID)
        {
            int r = fmRegisterYMF262Ryhthm[chipID];
            fmRegisterYMF262Ryhthm[chipID] = 0;
            return r;
        }

        public int getYMF278BRyhthmKeyON(int chipID)
        {
            return fmRegisterYMF278BRyhthm[chipID];
        }

        public void resetYMF278BRyhthmKeyON(int chipID)
        {
            fmRegisterYMF278BRyhthm[chipID] = 0;
        }

        public int[] getYMF278BPCMKeyON(int chipID)
        {
            return fmRegisterYMF278BPCM[chipID];
        }

        public void resetYMF278BPCMKeyON(int chipID)
        {
            for (int i = 0; i < 24; i++)
                fmRegisterYMF278BPCM[chipID][i] = 0;
        }

        public int getYMF262FMKeyON(int chipID)
        {
            return fmRegisterYMF262FM[chipID];
        }

        public int getYMF278BFMKeyON(int chipID)
        {
            return fmRegisterYMF278BFM[chipID];
        }

        public void resetYMF278BFMKeyON(int chipID)
        {
            fmRegisterYMF278BFM[chipID] = 0;
        }



        //public int x68Sound_TotalVolume(int vol, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return 0;

        //    return x68Sound.X68Sound_TotalVolume(vol);
        //}

        //public int x68Sound_GetPcm(short[] buf, int len, enmModel model, Action<Action, bool> oneFrameProc = null)
        //{
        //    if (model == enmModel.RealModel) return 0;
        //    return x68Sound.X68Sound_GetPcm(buf, len, oneFrameProc);
        //}

        //public void x68Sound_Free(enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    x68Sound.X68Sound_Free();
        //}

        //public void x68Sound_OpmInt(Action p, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    x68Sound.X68Sound_OpmInt(p);
        //}

        //public int x68Sound_StartPcm(int samprate, int v1, int v2, int pcmbuf, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return 0;
        //    return x68Sound.X68Sound_StartPcm(samprate, v1, v2, pcmbuf);
        //}

        //public int x68Sound_Start(int samprate, int v1, int v2, int betw, int pcmbuf, int late, double v3, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return 0;
        //    return x68Sound.X68Sound_Start(samprate, v1, v2, betw, pcmbuf, late, v3);
        //}

        //public int x68Sound_OpmWait(int v, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return 0;
        //    return x68Sound.X68Sound_OpmWait(v);
        //}

        //public void x68Sound_Pcm8_Out(int v, byte[] p, uint a1, int d1, int d2, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    x68Sound.X68Sound_Pcm8_Out(v, p, a1, d1, d2);
        //}

        //public void x68Sound_Pcm8_Abort(enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    x68Sound.X68Sound_Pcm8_Abort();
        //}

        //internal void x68Sound_MountMemory(byte[] mm, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    x68Sound.MountMemory(mm);
        //}

        //public void sound_iocs_init(enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    sound_iocs.init();
        //}

        //public void sound_iocs_iocs_adpcmmod(int v, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    sound_iocs._iocs_adpcmmod(v);
        //}

        //public void sound_iocs_iocs_adpcmout(uint a1, int d1, int d2, enmModel model)
        //{
        //    if (model == enmModel.RealModel) return;
        //    sound_iocs._iocs_adpcmout(a1, d1, d2);
        //}

        //public void sound_iocs_iocs_opmset(byte d1, byte d2, enmModel model)
        //{
        //    //if (model == enmModel.RealModel) return;
        //    setYM2151Register(0, 0, d1, d2, model, 0, 0);
        //    sound_iocs._iocs_opmset(d1, d2);
        //}


    }

    public class ChipKeyInfo
    {
        public bool[] On = null;
        public bool[] Off = null;

        public ChipKeyInfo(int n)
        {
            On = new bool[n];
            Off = new bool[n];
            for (int i = 0; i < n; i++) Off[i] = true;
        }
    }

}
