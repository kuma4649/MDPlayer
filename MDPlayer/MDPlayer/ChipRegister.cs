using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Midi;
using MDSound.np;
using MDSound.np.memory;
using MDSound.np.cpu;
using MDSound.np.chip;
using NScci;
using MDSound;

namespace MDPlayer
{
    public class ChipRegister
    {

        private Setting setting = null;
        private MDSound.MDSound mds = null;
        private midiOutInfo[] midiOutInfos = null;
        private List<NAudio.Midi.MidiOut> midiOuts = null;
        private List<int> midiOutsType = null;
        private List<vstInfo2> vstMidiOuts = null;
        private List<int> vstMidiOutsType = null;
        //private NX68Sound.X68Sound x68Sound = null;
        //private NX68Sound.sound_iocs sound_iocs = null;

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
        private RSoundChip[] scSN76489 =  new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2612 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2608 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2151 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2203 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610EA = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610EB = new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF262 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF271 =   new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF278B =  new RSoundChip[2] { null, null };
        private RSoundChip[] scYMZ280B =  new RSoundChip[2] { null, null };
        private RSoundChip[] scSEGAPCM =  new RSoundChip[2] { null, null };
        private RSoundChip[] scC140 =     new RSoundChip[2] { null, null };

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

        public int[][] fmRegisterYM2151 = new int[][] { null, null };
        public int[][] fmKeyOnYM2151 = new int[][] { null, null };
        public int[][] fmVolYM2151 = new int[][] {
            new int[8] { 0,0,0,0,0,0,0,0 }
            , new int[8] { 0,0,0,0,0,0,0,0 }
        };

        private int[] nowYM2151FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2151 = new bool[][] {
            new bool[8] { false, false, false, false, false, false, false, false }
            ,new bool[8] { false, false, false, false, false, false, false, false }
        };
        public int[] fmAMDYM2151 = new int[] { -1, -1 };
        public int[] fmPMDYM2151 = new int[] { -1, -1 };

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

        public int[][] sn76489Register = new int[][] { null, null };
        public int[] sn76489RegisterGGPan = new int[] { 0xff, 0xff };
        public int[][][] sn76489Vol = new int[][][] {
            new int[4][] { new int[2], new int[2], new int[2], new int[2] }
            ,new int[4][] { new int[2], new int[2], new int[2], new int[2] }
        };
        public int[] nowSN76489FadeoutVol = new int[] { 0, 0 };
        public bool[][] maskChSN76489 = new bool[][] {
            new bool[4] {false,false,false,false }
            ,new bool[4] {false,false,false,false }
        };

        public int[][] psgRegisterAY8910 = new int[][] { null, null };
        public int[][] psgKeyOnAY8910 = new int[][] { null, null };
        private int[] nowAY8910FadeoutVol = new int[] { 0, 0 };
        public int[][] psgVolAY8910 = new int[][] { new int[3], new int[3] };
        private bool[][] maskPSGChAY8910 = new bool[][] {
            new bool[3] { false, false, false }
            ,new bool[3] { false, false, false }
        };

        private bool[] maskOKIM6258 = new bool[2] { false, false };
        public bool[] okim6258Keyon = new bool[2] { false, false };

        private bool[][] maskOKIM6295 = new bool[2][] {
            new bool[] { false, false, false, false},
            new bool[] { false, false, false, false}
        };

        public byte[][] pcmRegisterC140 = new byte[2][] { null, null };
        public bool[][] pcmKeyOnC140 = new bool[2][] { null, null };
        private bool[][] maskChC140 = new bool[][] {
            new bool[24] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false
            }
            ,new bool[24] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false
            }
        };

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

        private bool[][] maskChRF5C164 = new bool[][] {
            new bool[8] {
                false, false, false, false, false, false, false, false
            }
            ,new bool[8] {
                false, false, false, false, false, false, false, false
            }
        };

        private bool[][] maskChHuC6280 = new bool[][] {
            new bool[6] {
                false, false, false, false, false, false
            }
            ,new bool[6] {
                false, false, false, false, false, false
            }
        };

        private bool[][] maskChSegaPCM = new bool[][] {
            new bool[16] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
            }
            ,new bool[16] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
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

        private int volF = 1;
        private MIDIExport midiExport = null;

        public ChipRegister(Setting setting
            , MDSound.MDSound mds
            , RealChip nScci
            , RSoundChip[] scYM2612
            , RSoundChip[] scSN76489
            , RSoundChip[] scYM2608
            , RSoundChip[] scYM2151
            , RSoundChip[] scYM2203
            , RSoundChip[] scYM2610
            , RSoundChip[] scYM2610EA
            , RSoundChip[] scYM2610EB
            , RSoundChip[] scC140
            , RSoundChip[] scSEGAPCM
            )
        {
            this.setting = setting;
            this.mds = mds;

            this.realChip = nScci;
            this.scYM2612 = scYM2612;
            this.scYM2608 = scYM2608;
            this.scYM2151 = scYM2151;
            this.scSN76489 = scSN76489;
            this.scYM2203 = scYM2203;
            this.scYM2610 = scYM2610;
            this.scYM2610EA = scYM2610EA;
            this.scYM2610EB = scYM2610EB;
            this.scC140 = scC140;
            this.scSEGAPCM = scSEGAPCM;

            this.ctYM2612 = new Setting.ChipType[] { setting.YM2612Type, setting.YM2612SType };
            this.ctSN76489 = new Setting.ChipType[] { setting.SN76489Type, setting.SN76489SType };
            this.ctYM2608 = new Setting.ChipType[] { setting.YM2608Type, setting.YM2608SType };
            this.ctYM2151 = new Setting.ChipType[] { setting.YM2151Type, setting.YM2151SType };
            this.ctYM2203 = new Setting.ChipType[] { setting.YM2203Type, setting.YM2203SType };
            this.ctYM2610 = new Setting.ChipType[] { setting.YM2610Type, setting.YM2610SType };
            this.ctYMF262 = new Setting.ChipType[] { setting.YMF262Type, setting.YMF262SType };
            this.ctYMF271 = new Setting.ChipType[] { setting.YMF271Type, setting.YMF271SType };
            this.ctYMF278B = new Setting.ChipType[] { setting.YMF278BType, setting.YMF278BSType };
            this.ctYMZ280B = new Setting.ChipType[] { setting.YMZ280BType, setting.YMZ280BSType };
            this.ctAY8910 = new Setting.ChipType[] { setting.AY8910Type, setting.AY8910SType };
            this.ctYM2413 = new Setting.ChipType[] { setting.YM2413Type, setting.YM2413SType };
            this.ctHuC6280 = new Setting.ChipType[] { setting.HuC6280Type, setting.HuC6280SType };
            this.ctYM3526 = new Setting.ChipType[] { setting.YM3526Type, setting.YM3526SType };
            this.ctY8950 = new Setting.ChipType[] { setting.Y8950Type, setting.Y8950SType };
            this.ctC140 = new Setting.ChipType[] { setting.C140Type, setting.C140SType };
            this.ctSEGAPCM = new Setting.ChipType[] { setting.SEGAPCMType, setting.SEGAPCMSType };

            initChipRegister(null);

            //x68Sound = new NX68Sound.X68Sound();
            //sound_iocs = new NX68Sound.sound_iocs(x68Sound);

            midiExport = new MIDIExport(setting);
            midiExport.fmRegisterYM2612 = fmRegisterYM2612;
            midiExport.fmRegisterYM2151 = fmRegisterYM2151;
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

                fmRegisterYM2151[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2151[chipID][i] = 0;
                }
                fmKeyOnYM2151[chipID] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2203[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2203[chipID][i] = 0;
                }
                fmKeyOnYM2203[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                sn76489Register[chipID] = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

                fmRegisterYM2413[chipID] = new int[0x39];
                for (int i = 0; i < 0x39; i++)
                {
                    fmRegisterYM2413[chipID][i] = 0;
                }
                //fmRegisterYM2413Ryhthm[0] = 0;
                //fmRegisterYM2413Ryhthm[1] = 0;
                //fmRegisterYM2413RyhthmB[0] = 0;
                //fmRegisterYM2413RyhthmB[1] = 0;

                psgRegisterAY8910[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    psgRegisterAY8910[chipID][i] = 0;
                }
                psgKeyOnAY8910[chipID] = new int[3] { 0, 0, 0 };

                pcmRegisterC140[chipID] = new byte[0x200];
                pcmKeyOnC140[chipID] = new bool[24];

                pcmRegisterC352[chipID] = new ushort[0x203];
                pcmKeyOnC352[chipID] = new ushort[32];

                pcmRegisterSEGAPCM[chipID] = new byte[0x200];
                pcmKeyOnSEGAPCM[chipID] = new bool[16];

                midiParams[chipID] = new MIDIParam();

                nowAY8910FadeoutVol[chipID] = 0;
                nowSN76489FadeoutVol[chipID] = 0;
                nowYM2151FadeoutVol[chipID] = 0;
                nowYM2203FadeoutVol[chipID] = 0;
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

                fmRegisterYM2151[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2151[chipID][i] = 0;
                }
                fmKeyOnYM2151[chipID] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2203[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2203[chipID][i] = 0;
                }
                fmKeyOnYM2203[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                sn76489Register[chipID] = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

                fmRegisterYM2413[chipID] = new int[0x39];
                for (int i = 0; i < 0x39; i++)
                {
                    fmRegisterYM2413[chipID][i] = 0;
                }
                //fmRegisterYM2413Ryhthm[0] = 0;
                //fmRegisterYM2413Ryhthm[1] = 0;
                //fmRegisterYM2413RyhthmB[0] = 0;
                //fmRegisterYM2413RyhthmB[1] = 0;

                psgRegisterAY8910[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    psgRegisterAY8910[chipID][i] = 0;
                }
                psgKeyOnAY8910[chipID] = new int[3] { 0, 0, 0 };

                pcmRegisterC140[chipID] = new byte[0x200];
                pcmKeyOnC140[chipID] = new bool[24];

                pcmRegisterC352[chipID] = new ushort[0x203];
                pcmKeyOnC352[chipID] = new ushort[32];

                pcmRegisterSEGAPCM[chipID] = new byte[0x200];
                pcmKeyOnSEGAPCM[chipID] = new bool[16];

                midiParams[chipID] = new MIDIParam();

                nowAY8910FadeoutVol[chipID] = 0;
                nowSN76489FadeoutVol[chipID] = 0;
                nowYM2151FadeoutVol[chipID] = 0;
                nowYM2203FadeoutVol[chipID] = 0;
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

        public void resetChips()
        {
            for (int chipID = 0; chipID < 2; chipID++)
            {
                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        setYM2612Register((byte)chipID, p, 0x40 + c, 127, EnmModel.RealModel, -1);
                        setYM2612Register((byte)chipID, p, 0x44 + c, 127, EnmModel.RealModel, -1);
                        setYM2612Register((byte)chipID, p, 0x48 + c, 127, EnmModel.RealModel, -1);
                        setYM2612Register((byte)chipID, p, 0x4c + c, 127, EnmModel.RealModel, -1);
                    }
                }


                for (int c = 0; c < 4; c++)
                {
                    setSN76489Register(chipID, 0x90 + (c << 5) + 0xf, EnmModel.RealModel);
                }

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        setYM2608Register((byte)chipID, p, 0x40 + c, 127, EnmModel.RealModel);
                        setYM2608Register((byte)chipID, p, 0x44 + c, 127, EnmModel.RealModel);
                        setYM2608Register((byte)chipID, p, 0x48 + c, 127, EnmModel.RealModel);
                        setYM2608Register((byte)chipID, p, 0x4c + c, 127, EnmModel.RealModel);
                    }
                }

                //ssg
                setYM2608Register((byte)chipID, 0, 0x08, 0, EnmModel.RealModel);
                setYM2608Register((byte)chipID, 0, 0x09, 0, EnmModel.RealModel);
                setYM2608Register((byte)chipID, 0, 0x0a, 0, EnmModel.RealModel);

                //rhythm
                setYM2608Register((byte)chipID, 0, 0x11, 0, EnmModel.RealModel);

                //adpcm
                setYM2608Register((byte)chipID, 1, 0x0b, 0, EnmModel.RealModel);


                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        setYM2610Register((byte)chipID, p, 0x40 + c, 127, EnmModel.RealModel);
                        setYM2610Register((byte)chipID, p, 0x44 + c, 127, EnmModel.RealModel);
                        setYM2610Register((byte)chipID, p, 0x48 + c, 127, EnmModel.RealModel);
                        setYM2610Register((byte)chipID, p, 0x4c + c, 127, EnmModel.RealModel);
                    }
                }

                //ssg
                setYM2610Register((byte)chipID, 0, 0x08, 0, EnmModel.RealModel);
                setYM2610Register((byte)chipID, 0, 0x09, 0, EnmModel.RealModel);
                setYM2610Register((byte)chipID, 0, 0x0a, 0, EnmModel.RealModel);

                //rhythm
                setYM2610Register((byte)chipID, 0, 0x11, 0, EnmModel.RealModel);

                //adpcm
                setYM2610Register((byte)chipID, 1, 0x0b, 0, EnmModel.RealModel);


                for (int c = 0; c < 8; c++)
                {
                    setYM2151Register((byte)chipID, 0, 0x60 + c, 127, EnmModel.RealModel, 0, -1);
                    setYM2151Register((byte)chipID, 0, 0x68 + c, 127, EnmModel.RealModel, 0, -1);
                    setYM2151Register((byte)chipID, 0, 0x70 + c, 127, EnmModel.RealModel, 0, -1);
                    setYM2151Register((byte)chipID, 0, 0x78 + c, 127, EnmModel.RealModel, 0, -1);
                }
            }

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

        public int getMIDIoutCount()
        {
            if (midiOuts == null) return 0;
            return midiOuts.Count;
        }

        public void sendMIDIout(EnmModel model, int num, byte cmd, byte prm1, byte prm2, int deltaFrames = 0)
        {
            if (model == EnmModel.RealModel)
            {
                if (midiOuts == null) return;
                if (num >= midiOuts.Count) return;
                if (midiOuts[num] == null) return;

                midiOuts[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
                if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
                return;
            }

            if (vstMidiOuts == null) return;
            if (num >= vstMidiOuts.Count) return;
            if (vstMidiOuts[num] == null) return;

            Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
                deltaFrames
                , 0//noteLength
                , 0//noteOffset
                , new byte[] { cmd, prm1, prm2 }
                , 0//detune
                , 0//noteOffVelocity
                );
            vstMidiOuts[num].AddMidiEvent(evt);
            if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
        }

        public void sendMIDIout(EnmModel model, int num, byte cmd, byte prm1, int deltaFrames = 0)
        {
            if (model == EnmModel.RealModel)
            {
                if (midiOuts == null) return;
                if (num >= midiOuts.Count) return;
                if (midiOuts[num] == null) return;

                midiOuts[num].SendBuffer(new byte[] { cmd, prm1 });
                if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
                return;
            }

            if (vstMidiOuts == null) return;
            if (num >= vstMidiOuts.Count) return;
            if (vstMidiOuts[num] == null) return;

            Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
                deltaFrames
                , 0//noteLength
                , 0//noteOffset
                , new byte[] { cmd, prm1 }
                , 0//detune
                , 0//noteOffVelocity
                );
            vstMidiOuts[num].AddMidiEvent(evt);
            if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
        }

        public void sendMIDIout(EnmModel model, int num, byte[] data, int deltaFrames = 0)
        {
            if (model == EnmModel.RealModel)
            {
                if (midiOuts == null) return;
                if (num >= midiOuts.Count) return;
                if (midiOuts[num] == null) return;

                midiOuts[num].SendBuffer(data);
                if (num < midiParams.Length) midiParams[num].SendBuffer(data);
                return;
            }

            if (vstMidiOuts == null) return;
            if (num >= vstMidiOuts.Count) return;
            if (vstMidiOuts[num] == null) return;

            Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
                deltaFrames
                , 0//noteLength
                , 0//noteOffset
                , data
                , 0//detune
                , 0//noteOffVelocity
                );
            vstMidiOuts[num].AddMidiEvent(evt);
            if (num < midiParams.Length) midiParams[num].SendBuffer(data);
        }

        public void resetAllMIDIout()
        {
            if (midiOuts != null)
            {
                for (int i = 0; i < midiOuts.Count; i++)
                {
                    if (midiOuts[i] == null) continue;
                    midiOuts[i].Reset();
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
                            sendMIDIout(EnmModel.VirtualModel, i, new byte[] { (byte)(0xb0 + ch), 120, 0x00 }, 0);
                            sendMIDIout(EnmModel.VirtualModel, i, new byte[] { (byte)(0xb0 + ch), 64, 0x00 }, 0);
                        }

                    }
                    catch { }
                }
            }
        }


        public void setYM2151Register(int chipID, int dPort, int dAddr, int dData, EnmModel model, int hosei, long vgmFrameCounter)
        {
            if (ctYM2151 == null) return;

            if (chipID == 0) chipLED.PriOPM = 2;
            else chipLED.SecOPM = 2;

            if (
                (model == EnmModel.VirtualModel && (ctYM2151[chipID] == null || !ctYM2151[chipID].UseScci))
                || (model == EnmModel.RealModel && (scYM2151 != null && scYM2151[chipID] != null))
                )
            {
                fmRegisterYM2151[chipID][dAddr] = dData;
                midiExport.outMIDIData(EnmChip.YM2151, chipID, dPort, dAddr, dData, hosei, vgmFrameCounter);
            }

            if ((model == EnmModel.RealModel && ctYM2151[chipID].UseScci) || (model == EnmModel.VirtualModel && !ctYM2151[chipID].UseScci))
            {
                if (dAddr == 0x08) //Key-On/Off
                {
                    int ch = dData & 0x7;
                    if (ch >= 0 && ch < 8)
                    {
                        if ((dData & 0x78) != 0)
                        {
                            byte con = (byte)(dData & 0x78);
                            fmKeyOnYM2151[chipID][ch] = con | 1;
                            fmVolYM2151[chipID][ch] = 256 * 6;
                        }
                        else
                        {
                            fmKeyOnYM2151[chipID][ch] &= 0xfe;
                        }
                    }
                }
            }

            //AMD/PMD
            if (dAddr == 0x19)
            {
                if ((dData & 0x80) != 0)
                {
                    fmPMDYM2151[chipID] = dData & 0x7f;
                }
                else
                {
                    fmAMDYM2151[chipID] = dData & 0x7f;
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
                        if (maskFMChYM2151[chipID][ch])
                        {
                            if (model == EnmModel.VirtualModel)
                            {
                                if (!ctYM2151[chipID].UseScci)
                                {
                                    if (ctYM2151[chipID].UseEmu) mds.WriteYM2151((byte)chipID, (byte)(0x60 + i * 8 + ch), (byte)127);
                                    if (ctYM2151[chipID].UseEmu2) mds.WriteYM2151mame((byte)chipID, (byte)(0x60 + i * 8 + ch), (byte)127);
                                    if (ctYM2151[chipID].UseEmu3) mds.WriteYM2151x68sound((byte)chipID, (byte)(0x60 + i * 8 + ch), (byte)127);
                                }
                            }
                            else
                            {
                                if (scYM2151 != null && scYM2151[chipID] != null) scYM2151[chipID].setRegister(0x60 + i * 8 + ch, 127);
                            }
                        }
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x60 || (dAddr & 0xf0) == 0x70)//TL
            {
                int ch = (dAddr & 0x7);
                dData &= 0x7f;

                dData = Math.Min(dData + nowYM2151FadeoutVol[chipID], 127);
                dData = maskFMChYM2151[chipID][ch] ? 127 : dData;
            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2151[chipID].UseScci)
                {
                    if (ctYM2151[chipID].UseEmu) mds.WriteYM2151((byte)chipID, (byte)dAddr, (byte)dData);
                    if (ctYM2151[chipID].UseEmu2) mds.WriteYM2151mame((byte)chipID, (byte)dAddr, (byte)dData);
                    if (ctYM2151[chipID].UseEmu3) mds.WriteYM2151x68sound((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2151[chipID] == null) return;

                if (dAddr >= 0x28 && dAddr <= 0x2f)
                {
                    if (hosei == 0)
                    {
                        scYM2151[chipID].setRegister(dAddr, dData);
                    }
                    else
                    {
                        int oct = (dData & 0x70) >> 4;
                        int note = dData & 0xf;
                        note = (note < 3) ? note : ((note < 7) ? (note - 1) : ((note < 11) ? (note - 2) : (note - 3)));
                        note += hosei - 1;
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
                        if (scYM2151[chipID] != null)
                            scYM2151[chipID].setRegister(dAddr, (oct << 4) | note);
                    }
                }
                else
                {
                    scYM2151[chipID].setRegister(dAddr, dData);
                }
            }

        }

        private void writeYM2151(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2151[chipID].UseScci)
                {
                    if (ctYM2151[chipID].UseEmu) mds.WriteYM2151((byte)chipID, (byte)dAddr, (byte)dData);
                    if (ctYM2151[chipID].UseEmu2) mds.WriteYM2151mame((byte)chipID, (byte)dAddr, (byte)dData);
                    if (ctYM2151[chipID].UseEmu3) mds.WriteYM2151x68sound((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2151[chipID] != null)
                    scYM2151[chipID].setRegister(dAddr, dData);
            }
        }

        public void softResetYM2151(int chipID, EnmModel model)
        {

            //FM全チャネルキーオフ
            for (int i = 0; i < 8; i++)
            {
                // note off
                writeYM2151(chipID, 0, 0x08, 0x00 + i, model);
            }

            writeYM2151(chipID, 0, 0x0f, 0x00, model); //  FM NOISE ENABLE/NOISE FREQ
            writeYM2151(chipID, 0, 0x18, 0x00, model); //  FM HW LFO FREQ
            writeYM2151(chipID, 0, 0x19, 0x80, model); //  FM PMD/VALUE
            writeYM2151(chipID, 0, 0x19, 0x00, model); //  FM AMD/VALUE
            writeYM2151(chipID, 0, 0x1b, 0x00, model); //  FM HW LFO WAVEFORM

            //FM HW LFO RESET
            writeYM2151(chipID, 0,0x01, 0x02, model);
            writeYM2151(chipID, 0, 0x01, 0x00, model);

            writeYM2151(chipID, 0,0x10, 0x00, model); // FM Timer-A(H)
            writeYM2151(chipID, 0,0x11, 0x00, model); // FM Timer-A(L)
            writeYM2151(chipID, 0,0x12, 0x00, model); // FM Timer-B
            writeYM2151(chipID, 0, 0x14, 0x00, model); // FM Timer Control

            for (int i = 0; i < 8; i++)
            {
                //  FB/ALG/PAN
                writeYM2151(chipID, 0, 0x20 + i, 0x00, model);
                // KC
                writeYM2151(chipID, 0, 0x28 + i, 0x00, model);
                // KF
                writeYM2151(chipID, 0, 0x30 + i, 0x00, model);
                // PMS/AMS
                writeYM2151(chipID, 0, 0x38 + i, 0x00, model);
            }
            for (int i = 0; i < 0x20; i++)
            {
                // DT1/ML
                writeYM2151(chipID, 0, 0x40 + i, 0x00, model);
                // TL=127
                writeYM2151(chipID, 0, 0x60 + i, 0x7f, model);
                // KS/AR
                writeYM2151(chipID, 0, 0x80 + i, 0x1F, model);
                // AMD/D1R
                writeYM2151(chipID, 0, 0xa0 + i, 0x00, model);
                // DT2/D2R
                writeYM2151(chipID, 0, 0xc0 + i, 0x00, model);
                // D1L/RR
                writeYM2151(chipID, 0, 0xe0 + i, 0x0F, model);
            }
        }

        public void setAY8910Register(int chipID, int dAddr, int dData, EnmModel model)
        {
            if (ctAY8910 == null) return;

            if (chipID == 0) chipLED.PriAY10 = 2;
            else chipLED.SecAY10 = 2;

            if (model == EnmModel.VirtualModel) psgRegisterAY8910[chipID][dAddr] = dData;

            //psg level
            if ((dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowAY8910FadeoutVol[chipID] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskPSGChAY8910[chipID][dAddr - 0x08] ? 0 : dData;
            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctAY8910[chipID].UseScci)
                {
                    mds.WriteAY8910((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                //if (scAY8910[chipID] == null) return;
                //scAY8910[chipID].setRegister(dAddr, dData);
            }
        }

        public void setDMGRegister(int chipID, int dAddr, int dData, EnmModel model)
        {
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

        public void setNESRegister(int chipID, int dAddr, int dData, EnmModel model)
        {
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

        public byte[] getNESRegister(int chipID, EnmModel model)
        {
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

        public void setMultiPCMRegister(int chipID, int dAddr, int dData, EnmModel model)
        {
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

        public void setMultiPCMSetBank(int chipID, int dCh, int dAddr, EnmModel model)
        {
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

        public void setQSoundRegister(int chipID, byte mm, byte ll, byte rr, EnmModel model)
        {
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

        public void setGA20Register(int chipID, Int32 Adr, byte Dat, EnmModel model)
        {
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

        public void setYM2413Register(int chipID, int dAddr, int dData, EnmModel model)
        {
            if (ctYM2413 == null) return;

            if (chipID == 0) chipLED.PriOPLL = 2;
            else chipLED.SecOPLL = 2;

            if (model == EnmModel.VirtualModel) fmRegisterYM2413[chipID][dAddr] = dData;

            if (dAddr >= 0x20 && dAddr <= 0x28)
            {
                int ch = dAddr - 0x20;
                int k = dData & 0x10;
                if (k == 0)
                {
                    kiYM2413[chipID].Off[ch] = true;
                }
                else
                {
                    if (kiYM2413[chipID].Off[ch]) kiYM2413[chipID].On[ch] = true;
                    kiYM2413[chipID].Off[ch] = false;
                }

                //mask適用
                if (maskFMChYM2413[chipID][ch]) dData &= 0xef;
            }

            if (dAddr == 0x0e)
            {
                for (int c = 0; c < 5; c++)
                {
                    if ((dData & (0x10 >> c)) == 0)
                    {
                        kiYM2413[chipID].Off[c + 9] = true;
                    }
                    else
                    {
                        if (kiYM2413[chipID].Off[c + 9]) kiYM2413[chipID].On[c + 9] = true;
                        kiYM2413[chipID].Off[c + 9] = false;
                    }
                }

                dData = (dData & 0x20)
                    | (maskFMChYM2413[chipID][9] ? 0 : (dData & 0x10))
                    | (maskFMChYM2413[chipID][10] ? 0 : (dData & 0x08))
                    | (maskFMChYM2413[chipID][11] ? 0 : (dData & 0x04))
                    | (maskFMChYM2413[chipID][12] ? 0 : (dData & 0x02))
                    | (maskFMChYM2413[chipID][13] ? 0 : (dData & 0x01))
                    ;
            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2413[chipID].UseScci)
                {
                    mds.WriteYM2413((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                //if (scYM2413[chipID] == null) return;
                //scYM2413[chipID].setRegister(dAddr, dData);
            }

        }

        public void setYM3812Register(int chipID, int dAddr, int dData, EnmModel model)
        {
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
                        if(kiYM3812[chipID].Off[ch]) kiYM3812[chipID].On[ch] = true;
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

        public void setHuC6280Register(int chipID, int dAddr, int dData, EnmModel model)
        {
            if (ctHuC6280 == null) return;

            if (chipID == 0) chipLED.PriHuC = 2;
            else chipLED.SecHuC = 2;

            if (model == EnmModel.VirtualModel)
            {
                if (!ctHuC6280[chipID].UseScci)
                {
                    if (dAddr == 0)
                    {
                        HuC6280CurrentCh[chipID] = dData & 7;
                    }
                    if (dAddr == 4)
                    {
                        dData = (int)(maskChHuC6280[chipID][HuC6280CurrentCh[chipID]] ? 0 : dData);
                    }
                    //System.Console.WriteLine("chipID:{0} Adr:{1} Dat:{2}", chipID, dAddr, dData);
                    mds.WriteHuC6280((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                //if (scHuC6280[chipID] == null) return;
            }
        }

        public void setYM2203Register(int chipID, int dAddr, int dData, EnmModel model)
        {
            if (ctYM2203 == null) return;

            if (chipID == 0) chipLED.PriOPN = 2;
            else chipLED.SecOPN = 2;

            if (model == EnmModel.VirtualModel)
            {
                if (dAddr != 0x2d && dAddr != 0x2e && dAddr != 0x2f)
                {
                    fmRegisterYM2203[chipID][dAddr] = dData;
                }
                else
                {
                    fmRegisterYM2203[chipID][0x2d] = dAddr - 0x2d;
                }
            }

            if ((model == EnmModel.RealModel && ctYM2203[chipID].UseScci) || (model == EnmModel.VirtualModel && !ctYM2203[chipID].UseScci))
            {
                if (dAddr == 0x28)
                {
                    int ch = dData & 0x3;
                    if (ch >= 0 && ch < 3)
                    {
                        if (ch != 2 || (fmRegisterYM2203[chipID][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2203[chipID][ch] = (dData & 0xf0) | 1;
                                fmVolYM2203[chipID][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2203[chipID][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2203[chipID][2] = (dData & 0xf0);
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2203[chipID][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2203[chipID][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2203[chipID][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2203[chipID][3] = 256 * 6;
                        }
                    }
                }

            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int slot = (dAddr & 0xc) >> 2;
                int al = fmRegisterYM2203[chipID][0xb0 + ch] & 0x7;
                dData &= 0x7f;

                if (ch != 3)
                {
                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2203FadeoutVol[chipID], 127);
                        dData = maskFMChYM2203[chipID][ch] ? 127 : dData;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0xb0)//AL
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                if (ch != 3)// && maskFMChYM2203[chipID][ch])
                {
                    for (int slot = 0; slot < 4; slot++)
                    {
                        if ((algM[al] & (1 << slot)) != 0)
                        {
                            setYM2203Register(
                                chipID
                                , 0x40 + ch + slot * 4
                                , fmRegisterYM2203[chipID][0x40 + ch + slot * 4]
                                , model);
                        }
                    }
                }
            }

            //ssg level
            if ((dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2203FadeoutVol[chipID] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2203[chipID][dAddr - 0x08 + 3] ? 0 : dData;
            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2203[chipID].UseScci)
                {
                    mds.WriteYM2203((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2203[chipID] == null) return;

                scYM2203[chipID].setRegister(dAddr, dData);
            }
        }

        private void writeYM2203(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2203[chipID].UseScci)
                {
                    mds.WriteYM2203((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2203[chipID] == null) return;

                scYM2203[chipID].setRegister(dAddr, dData);
            }
        }

        public void softResetYM2203(int chipID, EnmModel model)
        {
            int i;

            // FM全チャネルキーオフ
            writeYM2203(chipID, 0, 0x28, 0x00, model);
            writeYM2203(chipID, 0, 0x28, 0x01, model);
            writeYM2203(chipID, 0, 0x28, 0x02, model);

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x7f, model);
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x0, model);
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x0, model);
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0xff, model);
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x0, model);
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0xc0, model);
            }
            writeYM2203(chipID, 0, 0x22, 0x00, model); // HW LFO
            writeYM2203(chipID, 0, 0x24, 0x00, model); // Timer-A(1)
            writeYM2203(chipID, 0, 0x25, 0x00, model); // Timer-A(2)
            writeYM2203(chipID, 0, 0x26, 0x00, model); // Timer-B
            writeYM2203(chipID, 0, 0x27, 0x30, model); // Timer Control

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x00, model);
            }
            writeYM2203(chipID, 0, 0x06, 0x00, model); // SSG ノイズ周波数
            writeYM2203(chipID, 0, 0x07, 0x38, model); // SSG ミキサ
                                                       // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x00, model);
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                writeYM2203(chipID, 0, i, 0x00, model);
            }

        }

        public void setYM2608Register(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
            //if (chipID == 0 && dPort == 1 && dAddr < 0x11 && dAddr != 0x08 && model== enmModel.VirtualModel)
            //{
            //    log.Write(string.Format("FM P1 Out:Adr[{0:x02}] val[{1:x02}]", (int)dAddr, (int)dData));
            //}
            if (ctYM2608 == null) return;

            if (chipID == 0) chipLED.PriOPNA = 2;
            else chipLED.SecOPNA = 2;

            if (
                (model == EnmModel.VirtualModel && (ctYM2608[chipID] == null || !ctYM2608[chipID].UseScci))
                || (model == EnmModel.RealModel && (scYM2608 != null && scYM2608[chipID] != null))
                )
            {
                if (dPort == 0 && (dAddr == 0x2d || dAddr == 0x2e || dAddr == 0x2f))
                {
                    fmRegisterYM2608[chipID][0][0x2d] = dData - 0x2d;
                }
                else
                {
                    fmRegisterYM2608[chipID][dPort][dAddr] = dData;
                }
            }

            if ((model == EnmModel.RealModel && ctYM2608[chipID].UseScci) || (model == EnmModel.VirtualModel && !ctYM2608[chipID].UseScci))
            {
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        if (ch != 2 || (fmRegisterYM2608[chipID][0][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2608[chipID][ch] = (dData & 0xf0) | 1;
                                fmVolYM2608[chipID][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2608[chipID][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2608[chipID][2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2608[chipID][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2608[chipID][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2608[chipID][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2608[chipID][3] = 256 * 6;
                        }
                    }
                }

                if (dPort == 1 && dAddr == 0x01)
                {
                    fmVolYM2608AdpcmPan[chipID] = (dData & 0xc0) >> 6;
                    if (fmVolYM2608AdpcmPan[chipID] > 0)
                    {
                        fmVolYM2608Adpcm[chipID][0] = (int)((256 * 6.0 * fmRegisterYM2608[chipID][1][0x0b] / 64.0) * ((fmVolYM2608AdpcmPan[chipID] & 0x02) > 0 ? 1 : 0));
                        fmVolYM2608Adpcm[chipID][1] = (int)((256 * 6.0 * fmRegisterYM2608[chipID][1][0x0b] / 64.0) * ((fmVolYM2608AdpcmPan[chipID] & 0x01) > 0 ? 1 : 0));
                    }
                }

                if (dPort == 0 && dAddr == 0x10)
                {
                    int tl = fmRegisterYM2608[chipID][0][0x11] & 0x3f;
                    for (int i = 0; i < 6; i++)
                    {
                        if ((dData & (0x1 << i)) > 0)
                        {
                            int il = fmRegisterYM2608[chipID][0][0x18 + i] & 0x1f;
                            int pan = (fmRegisterYM2608[chipID][0][0x18 + i] & 0xc0) >> 6;
                            fmVolYM2608Rhythm[chipID][i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                            fmVolYM2608Rhythm[chipID][i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                        }
                    }
                }

            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int al = fmRegisterYM2608[chipID][dPort][0xb0 + ch] & 0x07;//AL
                int slot = (dAddr & 0xc) >> 2;
                dData &= 0x7f;

                if (ch != 3)
                {
                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2608FadeoutVol[chipID], 127);
                        dData = maskFMChYM2608[chipID][dPort * 3 + ch] ? 127 : dData;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0xb0)//AL
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                if (ch != 3)// && maskFMChYM2608[chipID][ch])
                {
                    for (int slot = 0; slot < 4; slot++)
                    {
                        if ((algM[al] & (1 << slot)) > 0)
                        {
                            setYM2608Register(
                                chipID
                                , dPort
                                , 0x40 + ch + slot * 4
                                , fmRegisterYM2608[chipID][dPort][0x40 + ch + slot * 4]
                                , model);
                        }
                    }
                }
            }

            //ssg level
            if (dPort == 0 && (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2608FadeoutVol[chipID] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2608[chipID][dAddr - 0x08 + 6] ? 0 : dData;
            }

            //rhythm level
            if (dPort == 0 && dAddr == 0x11)
            {
                int d = nowYM2608FadeoutVol[chipID] >> 1;
                dData = Math.Max(dData - d, 0);
            }

            //adpcm level
            if (dPort == 1 && dAddr == 0x0b)
            {
                int d = nowYM2608FadeoutVol[chipID] * 2;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2608[chipID][12] ? 0 : dData;
            }

            //adpcm start
            if (dPort == 1 && dAddr == 0x00)
            {
                if ((dData & 0x80) != 0 && maskFMChYM2608[chipID][12])
                {
                    dData &= 0x7f;
                }
            }

            //Ryhthm
            if (dPort == 0 && dAddr == 0x10)
            {
                if (maskFMChYM2608[chipID][13])
                {
                    dData = 0;
                }
            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2608[chipID].UseScci && ctYM2608[chipID].UseEmu)
                {
                    //if(dAddr==0x29) Console.Write("{0:x2}:{1:x2}:{2:x2}  ", dPort, dAddr, dData);
                    mds.WriteYM2608((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2608[chipID] == null) return;

                scYM2608[chipID].setRegister(dPort * 0x100 + dAddr, dData);
            }

        }

        public byte getYM2608Register(int chipID, int dPort, int dAddr, EnmModel model)
        {
            if (ctYM2608 == null) return 0;

            if (model == EnmModel.VirtualModel)
            {
                return 0;
            }
            else
            {
                if (scYM2608[chipID] == null) return 0;

                return (byte)scYM2608[chipID].getRegister(dPort * 0x100 + dAddr);
            }

        }

        private void writeYM2608(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2608[chipID].UseScci && ctYM2608[chipID].UseEmu)
                {
                    mds.WriteYM2608((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2608[chipID] == null) return;

                scYM2608[chipID].setRegister(dPort * 0x100 + dAddr, dData);
            }
        }

        public void softResetYM2608(int chipID, EnmModel model)
        {
            int i;

            // FM全チャネルキーオフ
            writeYM2608(chipID, 0, 0x28, 0x00, model);
            writeYM2608(chipID, 0, 0x28, 0x01, model);
            writeYM2608(chipID, 0, 0x28, 0x02, model);
            writeYM2608(chipID, 0, 0x28, 0x04, model);
            writeYM2608(chipID, 0, 0x28, 0x05, model);
            writeYM2608(chipID, 0, 0x28, 0x06, model);

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x7f, model);
                writeYM2608(chipID, 1, i, 0x7f, model);
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x0, model);
                writeYM2608(chipID, 1, i, 0x0, model);
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x0, model);
                writeYM2608(chipID, 1, i, 0x0, model);
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0xff, model);
                writeYM2608(chipID, 1, i, 0xff, model);
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x0, model);
                writeYM2608(chipID, 1, i, 0x0, model);
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0xc0, model);
                writeYM2608(chipID, 1, i, 0xc0, model);
            }
            writeYM2608(chipID, 0, 0x22, 0x00, model); // HW LFO
            writeYM2608(chipID, 0, 0x24, 0x00, model); // Timer-A(1)
            writeYM2608(chipID, 0, 0x25, 0x00, model); // Timer-A(2)
            writeYM2608(chipID, 0, 0x26, 0x00, model); // Timer-B
            writeYM2608(chipID, 0, 0x27, 0x30, model); // Timer Control
            writeYM2608(chipID, 0, 0x29, 0x80, model); // FM4-6 Enable

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x00, model);
            }
            writeYM2608(chipID, 0, 0x06, 0x00, model); // SSG ノイズ周波数
            writeYM2608(chipID, 0, 0x07, 0x38, model); // SSG ミキサ
                                                       // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x00, model);
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                writeYM2608(chipID, 0, i, 0x00, model);
            }

            // RHYTHM
            writeYM2608(chipID, 0, 0x10, 0xBF, model); // 強制発音停止
            writeYM2608(chipID, 0, 0x11, 0x00, model); // Total Level
            writeYM2608(chipID, 0, 0x18, 0x00, model); // BD音量
            writeYM2608(chipID, 0, 0x19, 0x00, model); // SD音量
            writeYM2608(chipID, 0, 0x1A, 0x00, model); // CYM音量
            writeYM2608(chipID, 0, 0x1B, 0x00, model); // HH音量
            writeYM2608(chipID, 0, 0x1C, 0x00, model); // TOM音量
            writeYM2608(chipID, 0, 0x1D, 0x00, model); // RIM音量

            // ADPCM
            writeYM2608(chipID, 1, 0x00, 0x21, model); // ADPCMリセット
            writeYM2608(chipID, 1, 0x01, 0x06, model); // ADPCM消音
            writeYM2608(chipID, 1, 0x10, 0x9C, model); // FLAGリセット        }
        }

        public void setYM2610Register(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
            if (ctYM2610 == null) return;

            if (chipID == 0) chipLED.PriOPNB = 2;
            else chipLED.SecOPNB = 2;


            if (
                (model == EnmModel.VirtualModel && (ctYM2610[chipID] == null || !ctYM2610[chipID].UseScci))
                || (model == EnmModel.RealModel && (scYM2610 != null && scYM2610[chipID] != null))
                )
            {
                if (dPort == 0 && (dAddr == 0x2d || dAddr == 0x2e || dAddr == 0x2f))
                {
                    fmRegisterYM2610[chipID][0][0x2d] = dData - 0x2d;
                }
                else
                {
                    fmRegisterYM2610[chipID][dPort][dAddr] = dData;
                }
            }

            if ((model == EnmModel.RealModel && ctYM2610[chipID].UseScci) || (model == EnmModel.VirtualModel && !ctYM2610[chipID].UseScci))
            {
                //fmRegisterYM2610[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        if (ch != 2 || (fmRegisterYM2610[chipID][0][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2610[chipID][ch] = (dData & 0xf0)|1;
                                fmVolYM2610[chipID][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2610[chipID][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2610[chipID][2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2610[chipID][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2610[chipID][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2610[chipID][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2610[chipID][3] = 256 * 6;
                        }
                    }
                }

                // ADPCM B KEYON
                if (dPort == 0 && dAddr == 0x10)
                {
                    if ((dData & 0x80) != 0)
                    {
                        int p = (fmRegisterYM2610[chipID][0][0x11] & 0xc0) >> 6;
                        p = p == 0 ? 3 : p;
                        if (fmVolYM2610AdpcmPan[chipID] != p)
                            fmVolYM2610AdpcmPan[chipID] = p;

                        //if (fmVolYM2610AdpcmPan > 0)
                        //{
                        fmVolYM2610Adpcm[chipID][0] = (int)((256 * 6.0 * fmRegisterYM2610[chipID][0][0x1b] / 64.0) * ((fmVolYM2610AdpcmPan[chipID] & 0x02) > 0 ? 1 : 0));
                        fmVolYM2610Adpcm[chipID][1] = (int)((256 * 6.0 * fmRegisterYM2610[chipID][0][0x1b] / 64.0) * ((fmVolYM2610AdpcmPan[chipID] & 0x01) > 0 ? 1 : 0));
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
                        int tl = fmRegisterYM2610[chipID][1][0x01] & 0x3f;
                        for (int i = 0; i < 6; i++)
                        {
                            if ((dData & (0x1 << i)) > 0)
                            {
                                int il = fmRegisterYM2610[chipID][1][0x08 + i] & 0x1f;
                                int pan = (fmRegisterYM2610[chipID][1][0x08 + i] & 0xc0) >> 6;
                                fmVolYM2610Rhythm[chipID][i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                                fmVolYM2610Rhythm[chipID][i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                            }
                        }
                    }
                }

            }



            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int slot = (dAddr & 0xc) >> 2;
                int al = fmRegisterYM2610[chipID][dPort][0xb0 + ch] & 0x07;//AL
                dData &= 0x7f;

                if (ch != 3)
                {
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        dData = Math.Min(dData + nowYM2610FadeoutVol[chipID], 127);
                        dData = maskFMChYM2610[chipID][dPort * 3 + ch] ? 127 : dData;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0xb0)//AL
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                if (ch != 3)// && maskFMChYM2610[chipID][ch])
                {
                    for (int slot = 0; slot < 4; slot++)
                    {
                        if ((algM[al] & (1 << slot)) != 0)
                        {
                            setYM2610Register(
                                chipID
                                , dPort
                                , 0x40 + ch + slot * 4
                                , fmRegisterYM2610[chipID][dPort][0x40 + ch + slot * 4]
                                , model);
                        }
                    }
                }
            }

            //ssg level
            if (dPort == 0 && (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2610FadeoutVol[chipID] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[chipID][dAddr - 0x08 + 6] ? 0 : dData;
            }

            //rhythm level
            if (dPort == 1 && dAddr == 0x01)
            {
                int d = nowYM2610FadeoutVol[chipID] >> 1;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[chipID][12] ? 0 : dData;
            }

            //Rhythm
            if (dPort == 1 && dAddr == 0x00)
            {
                if (maskFMChYM2610[chipID][12])
                {
                    dData = 0;
                }
            }

            //adpcm level
            if (dPort == 0 && dAddr == 0x1b)
            {
                int d = nowYM2610FadeoutVol[chipID] * 2;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[chipID][13] ? 0 : dData;
            }

            //adpcm start
            if (dPort == 0 && dAddr == 0x10)
            {
                if ((dData & 0x80) != 0 && maskFMChYM2610[chipID][13])
                {
                    dData &= 0x7f;
                }
            }



            if (model == EnmModel.VirtualModel)
            {
                if (!ctYM2610[chipID].UseScci && !ctYM2610[chipID].UseScci2)
                {
                    mds.WriteYM2610((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2610[chipID] != null) scYM2610[chipID].setRegister(dPort * 0x100 + dAddr, dData);
                if (scYM2610EA[chipID] != null)
                {
                    int dReg = (dPort << 8) | dAddr;
                    bool bSend = true;
                    // レジスタをマスクして送信する
                    if (dReg >= 0x100 && dReg <= 0x12d)
                    {
                        // ADPCM-A
                        bSend = false;
                    }
                    else if (dReg >= 0x010 && dReg <= 0x01c)
                    {
                        // ADPCM-B
                        bSend = false;
                    }
                    if (bSend)
                    {
                        scYM2610EA[chipID].setRegister((dPort << 8) | dAddr, dData);
                    }
                }
                if (scYM2610EB[chipID] != null)
                {
                    scYM2610EB[chipID].setRegister((dPort << 8) | dAddr | 0x10000, dData);
                }
            }

        }

        public void WriteYM2610_SetAdpcmA(int chipID, byte[] ym2610AdpcmA, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
                mds.WriteYM2610_SetAdpcmA((byte)chipID, ym2610AdpcmA);
            }
            else
            {
                if (scYM2610[chipID] != null)
                {
                    byte dPort = 2;
                    int startAddr = 0;
                    scYM2610[chipID].setRegister((dPort << 8) | 0x00, 0x00);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x01, (startAddr >> 8) & 0xff);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x02, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x03, 0x01);
                    // データ転送
                    for (int cnt = 0; cnt < ym2610AdpcmA.Length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610[chipID].setRegister((dPort << 8) | 0x04, ym2610AdpcmA[cnt]);
                    }

                    realChip.SendData();
                }
                if (scYM2610EB[chipID] != null)
                {
                    byte dPort = 2;
                    int startAddr = 0;
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10000, 0x00);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10001, (startAddr >> 8) & 0xff);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10002, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10003, 0x01);
                    // データ転送
                    for (int cnt = 0; cnt < ym2610AdpcmA.Length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610EB[chipID].setRegister((dPort << 8) | 0x10004, ym2610AdpcmA[cnt]);
                    }

                    realChip.SendData();
                }
            }
        }

        public void WriteYM2610_SetAdpcmA(int chipID, EnmModel model, int startAddr, int length, byte[] buf, int srcStartAddr)
        {
            if (model == EnmModel.VirtualModel)
            {
                return;
            }
            else
            {
                if (scYM2610[chipID] != null)
                {
                    byte dPort = 2;
                    scYM2610[chipID].setRegister((dPort << 8) | 0x00, 0x00);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x01, (startAddr >> 8) & 0xff);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x02, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x03, 0x01);
                    // データ転送
                    for (int cnt = 0; cnt < length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610[chipID].setRegister((dPort << 8) | 0x04, buf[srcStartAddr + cnt]);
                    }

                    realChip.SendData();
                }
                if (scYM2610EB[chipID] != null)
                {
                    byte dPort = 2;
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10000, 0x00);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10001, (startAddr >> 8) & 0xff);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10002, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10003, 0x01);
                    // データ転送
                    for (int cnt = 0; cnt < length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610EB[chipID].setRegister((dPort << 8) | 0x10004, buf[srcStartAddr + cnt]);
                    }

                    realChip.SendData();
                }
            }
        }

        public void WriteYM2610_SetAdpcmB(int chipID, byte[] ym2610AdpcmB, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
                mds.WriteYM2610_SetAdpcmB((byte)chipID, ym2610AdpcmB);
            }
            else
            {
                if (scYM2610[chipID] != null)
                {
                    byte dPort = 2;
                    int startAddr = 0;
                    scYM2610[chipID].setRegister((dPort << 8) | 0x00, 0x00);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x01, (startAddr >> 8) & 0xff);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x02, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x03, 0x00);
                    // データ転送
                    for (int cnt = 0; cnt < ym2610AdpcmB.Length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610[chipID].setRegister((dPort << 8) | 0x04, ym2610AdpcmB[cnt]);
                    }

                    realChip.SendData();
                }
                if (scYM2610EB[chipID] != null)
                {
                    byte dPort = 2;
                    int startAddr = 0;
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10000, 0x00);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10001, (startAddr >> 8) & 0xff);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10002, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10003, 0x00);
                    // データ転送
                    for (int cnt = 0; cnt < ym2610AdpcmB.Length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610EB[chipID].setRegister((dPort << 8) | 0x10004, ym2610AdpcmB[cnt]);
                    }

                    realChip.SendData();
                }
            }
        }

        public void WriteYM2610_SetAdpcmB(int chipID, EnmModel model, int startAddr, int length, byte[] buf, int srcStartAddr)
        {
            if (model == EnmModel.VirtualModel)
            {
                return;
            }
            else
            {
                if (scYM2610[chipID] != null)
                {
                    byte dPort = 2;
                    scYM2610[chipID].setRegister((dPort << 8) | 0x00, 0x00);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x01, (startAddr >> 8) & 0xff);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x02, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610[chipID].setRegister((dPort << 8) | 0x03, 0x00);
                    // データ転送
                    for (int cnt = 0; cnt < length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610[chipID].setRegister((dPort << 8) | 0x04, buf[srcStartAddr + cnt]);
                    }

                    realChip.SendData();
                }
                if (scYM2610EB[chipID] != null)
                {
                    byte dPort = 2;
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10000, 0x00);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10001, (startAddr >> 8) & 0xff);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10002, (startAddr >> 16) & 0xff);

                    // pushReg(CMD_YM2610|0x02,0x03,0x01);
                    scYM2610EB[chipID].setRegister((dPort << 8) | 0x10003, 0x00);
                    // データ転送
                    for (int cnt = 0; cnt < length; cnt++)
                    {
                        // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                        scYM2610EB[chipID].setRegister((dPort << 8) | 0x10004, buf[srcStartAddr + cnt]);
                    }

                    realChip.SendData();
                }
            }
        }

        public void setYMF262Register(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
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

        public void setYMF271Register(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
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

        public void setYMF278BRegister(int chipID, int dPort, int dAddr, int dData, EnmModel model)
        {
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

        public void setYM3526Register(int chipID, int dAddr, int dData, EnmModel model)
        {
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

        public void setY8950Register(int chipID, int dAddr, int dData, EnmModel model)
        {
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

        public void setYMZ280BRegister(int chipID, int dAddr, int dData, EnmModel model)
        {
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

        public void setYM2612Register(int chipID, int dPort, int dAddr, int dData, EnmModel model, long vgmFrameCounter)
        {
            if (ctYM2612 == null) return;

            if (chipID == 0) chipLED.PriOPN2 = 2;
            else chipLED.SecOPN2 = 2;

            if (model == EnmModel.VirtualModel)
            {
                fmRegisterYM2612[chipID][dPort][dAddr] = dData;
                midiExport.outMIDIData(EnmChip.YM2612, chipID, dPort, dAddr, dData, 0, vgmFrameCounter);
            }

            if ((model == EnmModel.RealModel && ctYM2612[chipID].UseScci) || (model == EnmModel.VirtualModel && !ctYM2612[chipID].UseScci))
            {
                //fmRegister[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0)>0)
                    {
                        if (ch != 2 || (fmRegisterYM2612[chipID][0][0x27] & 0xc0) != 0x40)
                        {
                            if (ch != 5 || (fmRegisterYM2612[chipID][0][0x2b] & 0x80) == 0)
                            {
                                if ((dData & 0xf0) != 0)
                                {
                                    fmKeyOnYM2612[chipID][ch] = (dData & 0xf0) | 1;
                                    fmVolYM2612[chipID][ch] = 256 * 6;
                                }
                                else
                                {
                                    fmKeyOnYM2612[chipID][ch] = (dData & 0xf0) | 0;
                                }
                            }
                        }
                        else
                        {
                            fmKeyOnYM2612[chipID][2] = (dData & 0xf0);
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2612[chipID][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2612[chipID][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2612[chipID][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2612[chipID][3] = 256 * 6;
                        }
                    }
                }

                //PCM
                if ((fmRegisterYM2612[chipID][0][0x2b] & 0x80) > 0)
                {
                    if (fmRegisterYM2612[chipID][0][0x2a] > 0)
                    {
                        fmVolYM2612[chipID][5] = Math.Abs(fmRegisterYM2612[chipID][0][0x2a]-0x7f) * 20;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int slot = (dAddr & 0xc) >> 2;
                int al = fmRegisterYM2612[chipID][dPort][0xb0 + ch] & 0x07;
                dData &= 0x7f;

                if (ch != 3)
                {
                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2612FadeoutVol[chipID], 127);
                        dData = maskFMChYM2612[chipID][dPort * 3 + ch] ? 127 : dData;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0xb0)//AL
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                if (ch != 3)// && maskFMChYM2612[chipID][dPort * 3 + ch])
                {
                    //CarrierのTLを再設定する
                    for (int slot = 0; slot < 4; slot++)
                    {
                        if ((algM[al] & (1 << slot)) != 0)
                        {
                            setYM2612Register(
                                chipID
                                , dPort
                                , 0x40 + ch + slot * 4
                                , fmRegisterYM2612[chipID][dPort][0x40 + ch + slot * 4]
                                , model
                                , vgmFrameCounter);
                        }
                    }
                }
            }

            if (dAddr == 0x2a)
            {
                //PCMデータをマスクする
                if (maskFMChYM2612[chipID][5]) dData = 0x00;
            }

            if (model == EnmModel.VirtualModel)
            {

                //仮想音源の処理

                if (ctYM2612[chipID].UseScci)
                {
                    //Scciを使用する場合でも
                    //PCM(6Ch)だけエミュで発音するとき
                    if (ctYM2612[chipID].OnlyPCMEmulation)
                    {
                        if (dPort == 0 && dAddr == 0x2b)
                        {
                            if (ctYM2612[chipID].UseEmu) mds.WriteYM2612((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                            if (ctYM2612[chipID].UseEmu2) mds.WriteYM3438((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                        }
                        else if (dPort == 0 && dAddr == 0x2a)
                        {
                            if (ctYM2612[chipID].UseEmu) mds.WriteYM2612((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                            if (ctYM2612[chipID].UseEmu2) mds.WriteYM3438((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                        }
                        else if (dPort == 1 && dAddr == 0xb6)
                        {
                            if (ctYM2612[chipID].UseEmu) mds.WriteYM2612((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                            if (ctYM2612[chipID].UseEmu2) mds.WriteYM3438((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                        }
                    }
                }
                else
                {
                    //エミュを使用する場合のみMDSoundへデータを送る
                    if (ctYM2612[chipID].UseEmu) mds.WriteYM2612((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                    if (ctYM2612[chipID].UseEmu2) mds.WriteYM3438((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {

                //実音源(Scci)

                if (scYM2612[chipID] == null) return;

                //PCM(6Ch)だけエミュで発音するとき
                if (ctYM2612[chipID].OnlyPCMEmulation)
                {
                    //アドレスを調べてPCMにはデータを送らない
                    if (dPort == 0 && dAddr == 0x2b)
                    {
                        scYM2612[chipID].setRegister(dPort * 0x100 + dAddr, dData);
                    }
                    else if (dPort == 0 && dAddr == 0x2a)
                    {
                    }
                    else
                    {
                        scYM2612[chipID].setRegister(dPort * 0x100 + dAddr, dData);
                    }
                }
                else
                {
                    //Scciへデータを送る
                    scYM2612[chipID].setRegister(dPort * 0x100 + dAddr, dData);
                }
            }

        }

        public void setMaskAY8910(int chipID,int ch,bool mask)
        {
            maskPSGChAY8910[chipID][ch] = mask;
        }

        public void setMaskRF5C164(int chipID, int ch, bool mask)
        {
            maskChRF5C164[chipID][ch] = mask;
        }

        public void setMaskSN76489(int chipID, int ch, bool mask)
        {
            maskChSN76489[chipID][ch] = mask;
        }

        public void setMaskYM2151(int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2151[chipID][ch] = mask;

            if (noSend) return;

            setYM2151Register((byte)chipID, 0, 0x60 + ch, fmRegisterYM2151[chipID][0x60 + ch], EnmModel.VirtualModel, 0, -1);
            setYM2151Register((byte)chipID, 0, 0x68 + ch, fmRegisterYM2151[chipID][0x68 + ch], EnmModel.VirtualModel, 0, -1);
            setYM2151Register((byte)chipID, 0, 0x70 + ch, fmRegisterYM2151[chipID][0x70 + ch], EnmModel.VirtualModel, 0, -1);
            setYM2151Register((byte)chipID, 0, 0x78 + ch, fmRegisterYM2151[chipID][0x78 + ch], EnmModel.VirtualModel, 0, -1);

            setYM2151Register((byte)chipID, 0, 0x60 + ch, fmRegisterYM2151[chipID][0x60 + ch], EnmModel.RealModel, 0, -1);
            setYM2151Register((byte)chipID, 0, 0x68 + ch, fmRegisterYM2151[chipID][0x68 + ch], EnmModel.RealModel, 0, -1);
            setYM2151Register((byte)chipID, 0, 0x70 + ch, fmRegisterYM2151[chipID][0x70 + ch], EnmModel.RealModel, 0, -1);
            setYM2151Register((byte)chipID, 0, 0x78 + ch, fmRegisterYM2151[chipID][0x78 + ch], EnmModel.RealModel, 0, -1);
        }

        public void setMaskYM2203(int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2203[chipID][ch] = mask;

            if (noSend) return;

            int c = ch;
            if (ch < 3)
            {
                setYM2203Register((byte)chipID, 0x40 + c, fmRegisterYM2203[chipID][0x40 + c], EnmModel.VirtualModel);
                setYM2203Register((byte)chipID, 0x44 + c, fmRegisterYM2203[chipID][0x44 + c], EnmModel.VirtualModel);
                setYM2203Register((byte)chipID, 0x48 + c, fmRegisterYM2203[chipID][0x48 + c], EnmModel.VirtualModel);
                setYM2203Register((byte)chipID, 0x4c + c, fmRegisterYM2203[chipID][0x4c + c], EnmModel.VirtualModel);

                setYM2203Register((byte)chipID, 0x40 + c, fmRegisterYM2203[chipID][0x40 + c], EnmModel.RealModel);
                setYM2203Register((byte)chipID, 0x44 + c, fmRegisterYM2203[chipID][0x44 + c], EnmModel.RealModel);
                setYM2203Register((byte)chipID, 0x48 + c, fmRegisterYM2203[chipID][0x48 + c], EnmModel.RealModel);
                setYM2203Register((byte)chipID, 0x4c + c, fmRegisterYM2203[chipID][0x4c + c], EnmModel.RealModel);
            }
            else
            {
                setYM2203Register((byte)chipID, 0x08 + c - 3, fmRegisterYM2203[chipID][0x08 + c - 3], EnmModel.VirtualModel);
                setYM2203Register((byte)chipID, 0x08 + c - 3, fmRegisterYM2203[chipID][0x08 + c - 3], EnmModel.RealModel);
            }
        }

        public void setMaskYM2413(int chipID, int ch, bool mask)
        {
            maskFMChYM2413[chipID][ch] = mask;

            if (ch < 9)
            {
                setYM2413Register((byte)chipID, 0x20 + ch, fmRegisterYM2413[chipID][0x20 + ch], EnmModel.VirtualModel);
                setYM2413Register((byte)chipID, 0x20 + ch, fmRegisterYM2413[chipID][0x20 + ch], EnmModel.RealModel);
            }
            else if (ch < 14)
            {
                setYM2413Register((byte)chipID, 0x0e, fmRegisterYM2413[chipID][0x0e], EnmModel.VirtualModel);
                setYM2413Register((byte)chipID, 0x0e, fmRegisterYM2413[chipID][0x0e], EnmModel.RealModel);
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

        public void setMaskC140(int chipID, int ch, bool mask)
        {
            maskChC140[chipID][ch] = mask;
        }

        public void setMaskC352(int chipID, int ch, bool mask)
        {
            maskChC352[chipID][ch] = mask;
        }

        public void setMaskHuC6280(int chipID, int ch, bool mask)
        {
            maskChHuC6280[chipID][ch] = mask;
        }

        public void setMaskSegaPCM(int chipID, int ch, bool mask)
        {
            maskChSegaPCM[chipID][ch] = mask;
        }

        public void setMaskYM2608(int chipID, int ch, bool mask,bool noSend=false)
        {
            maskFMChYM2608[chipID][ch] = mask;
            if (ch >= 9 && ch < 12)
            {
                maskFMChYM2608[chipID][2] = mask;
                maskFMChYM2608[chipID][9] = mask;
                maskFMChYM2608[chipID][10] = mask;
                maskFMChYM2608[chipID][11] = mask;
            }

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            if (noSend) return;

            if (ch < 6)
            {
                setYM2608Register((byte)chipID, p, 0x40 + c, fmRegisterYM2608[chipID][p][0x40 + c], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, p, 0x44 + c, fmRegisterYM2608[chipID][p][0x44 + c], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, p, 0x48 + c, fmRegisterYM2608[chipID][p][0x48 + c], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, p, 0x4c + c, fmRegisterYM2608[chipID][p][0x4c + c], EnmModel.VirtualModel);

                setYM2608Register((byte)chipID, p, 0x40 + c, fmRegisterYM2608[chipID][p][0x40 + c], EnmModel.RealModel);
                setYM2608Register((byte)chipID, p, 0x44 + c, fmRegisterYM2608[chipID][p][0x44 + c], EnmModel.RealModel);
                setYM2608Register((byte)chipID, p, 0x48 + c, fmRegisterYM2608[chipID][p][0x48 + c], EnmModel.RealModel);
                setYM2608Register((byte)chipID, p, 0x4c + c, fmRegisterYM2608[chipID][p][0x4c + c], EnmModel.RealModel);
            }
            else if (ch < 9)
            {
                setYM2608Register((byte)chipID, 0, 0x08 + ch - 6, fmRegisterYM2608[chipID][0][0x08 + ch - 6], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, 0, 0x08 + ch - 6, fmRegisterYM2608[chipID][0][0x08 + ch - 6], EnmModel.RealModel);
            }
            else if (ch < 12)
            {
                setYM2608Register((byte)chipID, 0, 0x40 + 2, fmRegisterYM2608[chipID][0][0x40 + 2], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, 0, 0x44 + 2, fmRegisterYM2608[chipID][0][0x44 + 2], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, 0, 0x48 + 2, fmRegisterYM2608[chipID][0][0x48 + 2], EnmModel.VirtualModel);
                setYM2608Register((byte)chipID, 0, 0x4c + 2, fmRegisterYM2608[chipID][0][0x4c + 2], EnmModel.VirtualModel);

                setYM2608Register((byte)chipID, 0, 0x40 + 2, fmRegisterYM2608[chipID][0][0x40 + 2], EnmModel.RealModel);
                setYM2608Register((byte)chipID, 0, 0x44 + 2, fmRegisterYM2608[chipID][0][0x44 + 2], EnmModel.RealModel);
                setYM2608Register((byte)chipID, 0, 0x48 + 2, fmRegisterYM2608[chipID][0][0x48 + 2], EnmModel.RealModel);
                setYM2608Register((byte)chipID, 0, 0x4c + 2, fmRegisterYM2608[chipID][0][0x4c + 2], EnmModel.RealModel);
            }
        }

        public void setMaskYM2610(int chipID, int ch, bool mask)
        {
            maskFMChYM2610[chipID][ch] = mask;
            if (ch >= 9 && ch < 12)
            {
                maskFMChYM2610[chipID][2] = mask;
                maskFMChYM2610[chipID][9] = mask;
                maskFMChYM2610[chipID][10] = mask;
                maskFMChYM2610[chipID][11] = mask;
            }

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            if (ch < 6)
            {
                setYM2610Register((byte)chipID, p, 0x40 + c, fmRegisterYM2610[chipID][p][0x40 + c], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, p, 0x44 + c, fmRegisterYM2610[chipID][p][0x44 + c], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, p, 0x48 + c, fmRegisterYM2610[chipID][p][0x48 + c], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, p, 0x4c + c, fmRegisterYM2610[chipID][p][0x4c + c], EnmModel.VirtualModel);

                setYM2610Register((byte)chipID, p, 0x40 + c, fmRegisterYM2610[chipID][p][0x40 + c], EnmModel.RealModel);
                setYM2610Register((byte)chipID, p, 0x44 + c, fmRegisterYM2610[chipID][p][0x44 + c], EnmModel.RealModel);
                setYM2610Register((byte)chipID, p, 0x48 + c, fmRegisterYM2610[chipID][p][0x48 + c], EnmModel.RealModel);
                setYM2610Register((byte)chipID, p, 0x4c + c, fmRegisterYM2610[chipID][p][0x4c + c], EnmModel.RealModel);
            }
            else if (ch < 9)
            {
                setYM2610Register((byte)chipID, 0, 0x08 + ch - 6, fmRegisterYM2610[chipID][0][0x08 + ch - 6], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, 0, 0x08 + ch - 6, fmRegisterYM2610[chipID][0][0x08 + ch - 6], EnmModel.RealModel);
            }
            else if (ch < 12)
            {
                setYM2610Register((byte)chipID, 0, 0x40 + 2, fmRegisterYM2610[chipID][0][0x40 + 2], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, 0, 0x44 + 2, fmRegisterYM2610[chipID][0][0x44 + 2], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, 0, 0x48 + 2, fmRegisterYM2610[chipID][0][0x48 + 2], EnmModel.VirtualModel);
                setYM2610Register((byte)chipID, 0, 0x4c + 2, fmRegisterYM2610[chipID][0][0x4c + 2], EnmModel.VirtualModel);

                setYM2610Register((byte)chipID, 0, 0x40 + 2, fmRegisterYM2610[chipID][0][0x40 + 2], EnmModel.RealModel);
                setYM2610Register((byte)chipID, 0, 0x44 + 2, fmRegisterYM2610[chipID][0][0x44 + 2], EnmModel.RealModel);
                setYM2610Register((byte)chipID, 0, 0x48 + 2, fmRegisterYM2610[chipID][0][0x48 + 2], EnmModel.RealModel);
                setYM2610Register((byte)chipID, 0, 0x4c + 2, fmRegisterYM2610[chipID][0][0x4c + 2], EnmModel.RealModel);
            }
        }

        public void setMaskYM2612(int chipID, int ch, bool mask)
        {
            maskFMChYM2612[chipID][ch] = mask;

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            setYM2612Register((byte)chipID, p, 0x40 + c, fmRegisterYM2612[chipID][p][0x40 + c], EnmModel.VirtualModel, -1);
            setYM2612Register((byte)chipID, p, 0x44 + c, fmRegisterYM2612[chipID][p][0x44 + c], EnmModel.VirtualModel, -1);
            setYM2612Register((byte)chipID, p, 0x48 + c, fmRegisterYM2612[chipID][p][0x48 + c], EnmModel.VirtualModel, -1);
            setYM2612Register((byte)chipID, p, 0x4c + c, fmRegisterYM2612[chipID][p][0x4c + c], EnmModel.VirtualModel, -1);

            setYM2612Register((byte)chipID, p, 0x40 + c, fmRegisterYM2612[chipID][p][0x40 + c], EnmModel.RealModel, -1);
            setYM2612Register((byte)chipID, p, 0x44 + c, fmRegisterYM2612[chipID][p][0x44 + c], EnmModel.RealModel, -1);
            setYM2612Register((byte)chipID, p, 0x48 + c, fmRegisterYM2612[chipID][p][0x48 + c], EnmModel.RealModel, -1);
            setYM2612Register((byte)chipID, p, 0x4c + c, fmRegisterYM2612[chipID][p][0x4c + c], EnmModel.RealModel, -1);
        }

        public void setMaskOKIM6258(int chipID, bool mask)
        {
            maskOKIM6258[chipID] = mask;

            writeOKIM6258((byte)chipID, 0, 1, EnmModel.VirtualModel);
            writeOKIM6258((byte)chipID, 0, 1, EnmModel.RealModel);
        }

        public void setMaskOKIM6295(int chipID,int ch, bool mask)
        {
            maskOKIM6295[chipID][ch] = mask;
            if (mask) mds.setOKIM6295Mask(0, chipID, 1 << ch);
            else mds.resetOKIM6295Mask(0, chipID, 1 << ch);
        }

        internal okim6295.okim6295Info GetOKIM6295Info(int chipID)
        {
            return mds.GetOKIM6295Info(0, chipID);
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
            writeK051649((byte)chipID, (3 << 1) | 1, K051649tKeyOnOff[chipID],EnmModel.VirtualModel);
        }

        public void resetK051649Mask(int chipID, int ch)
        {
            maskChK051649[chipID][ch] = false;
            writeK051649((byte)chipID, (3 << 1) | 1, K051649tKeyOnOff[chipID], EnmModel.VirtualModel);
        }



        public void setFadeoutVolYM2151(int chipID, int v)
        {
            nowYM2151FadeoutVol[chipID] = v;
            for (int c = 0; c < 8; c++)
            {
                setYM2151Register((byte)chipID, 0, 0x60 + c, fmRegisterYM2151[chipID][0x60 + c], EnmModel.RealModel, 0, -1);
                setYM2151Register((byte)chipID, 0, 0x68 + c, fmRegisterYM2151[chipID][0x68 + c], EnmModel.RealModel, 0, -1);
                setYM2151Register((byte)chipID, 0, 0x70 + c, fmRegisterYM2151[chipID][0x70 + c], EnmModel.RealModel, 0, -1);
                setYM2151Register((byte)chipID, 0, 0x78 + c, fmRegisterYM2151[chipID][0x78 + c], EnmModel.RealModel, 0, -1);
            }
        }

        int[] algVolTbl = new int[8] { 8, 8, 8, 8, 0xa, 0xe, 0xe, 0xf };
        private int[] HuC6280CurrentCh = new int[2] { 0, 0 };

        public void setFadeoutVolYM2203(int chipID, int v)
        {
            nowYM2203FadeoutVol[chipID] = v;
            for (int c = 0; c < 3; c++)
            {
                int alg = fmRegisterYM2203[chipID][0xb0 + c] & 0x7;
                if ((algVolTbl[alg] & 1) != 0) setYM2203Register((byte)chipID, 0x40 + c, fmRegisterYM2203[chipID][0x40 + c], EnmModel.RealModel);
                if ((algVolTbl[alg] & 4) != 0) setYM2203Register((byte)chipID, 0x44 + c, fmRegisterYM2203[chipID][0x44 + c], EnmModel.RealModel);
                if ((algVolTbl[alg] & 2) != 0) setYM2203Register((byte)chipID, 0x48 + c, fmRegisterYM2203[chipID][0x48 + c], EnmModel.RealModel);
                if ((algVolTbl[alg] & 8) != 0) setYM2203Register((byte)chipID, 0x4c + c, fmRegisterYM2203[chipID][0x4c + c], EnmModel.RealModel);
            }
        }

        public void setFadeoutVolYM2608(int chipID, int v)
        {

            nowYM2608FadeoutVol[chipID] = v;

            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2608Register((byte)chipID, p, 0x40 + c, fmRegisterYM2608[chipID][p][0x40 + c], EnmModel.RealModel);
                    setYM2608Register((byte)chipID, p, 0x44 + c, fmRegisterYM2608[chipID][p][0x44 + c], EnmModel.RealModel);
                    setYM2608Register((byte)chipID, p, 0x48 + c, fmRegisterYM2608[chipID][p][0x48 + c], EnmModel.RealModel);
                    setYM2608Register((byte)chipID, p, 0x4c + c, fmRegisterYM2608[chipID][p][0x4c + c], EnmModel.RealModel);
                }
            }

            //ssg
            setYM2608Register((byte)chipID, 0, 0x08, fmRegisterYM2608[chipID][0][0x08], EnmModel.RealModel);
            setYM2608Register((byte)chipID, 0, 0x09, fmRegisterYM2608[chipID][0][0x09], EnmModel.RealModel);
            setYM2608Register((byte)chipID, 0, 0x0a, fmRegisterYM2608[chipID][0][0x0a], EnmModel.RealModel);

            //rhythm
            setYM2608Register((byte)chipID, 0, 0x11, fmRegisterYM2608[chipID][0][0x11], EnmModel.RealModel);

            //adpcm
            setYM2608Register((byte)chipID, 1, 0x0b, fmRegisterYM2608[chipID][1][0x0b], EnmModel.RealModel);
        }

        public void setFadeoutVolYM2610(int chipID, int v)
        {
            nowYM2610FadeoutVol[chipID] = v;
            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2610Register((byte)chipID, p, 0x40 + c, fmRegisterYM2610[chipID][p][0x40 + c], EnmModel.RealModel);
                    setYM2610Register((byte)chipID, p, 0x44 + c, fmRegisterYM2610[chipID][p][0x44 + c], EnmModel.RealModel);
                    setYM2610Register((byte)chipID, p, 0x48 + c, fmRegisterYM2610[chipID][p][0x48 + c], EnmModel.RealModel);
                    setYM2610Register((byte)chipID, p, 0x4c + c, fmRegisterYM2610[chipID][p][0x4c + c], EnmModel.RealModel);
                }
            }

            //ssg
            setYM2610Register((byte)chipID, 0, 0x08, fmRegisterYM2610[chipID][0][0x08], EnmModel.RealModel);
            setYM2610Register((byte)chipID, 0, 0x09, fmRegisterYM2610[chipID][0][0x09], EnmModel.RealModel);
            setYM2610Register((byte)chipID, 0, 0x0a, fmRegisterYM2610[chipID][0][0x0a], EnmModel.RealModel);

            //rhythm
            setYM2610Register((byte)chipID, 0, 0x11, fmRegisterYM2610[chipID][0][0x11], EnmModel.RealModel);

            //adpcm
            setYM2610Register((byte)chipID, 1, 0x0b, fmRegisterYM2610[chipID][1][0x0b], EnmModel.RealModel);
        }

        public void setFadeoutVolYM2612(int chipID, int v)
        {
            nowYM2612FadeoutVol[chipID] = v;
            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2612Register((byte)chipID, p, 0x40 + c, fmRegisterYM2612[chipID][p][0x40 + c], EnmModel.RealModel, -1);
                    setYM2612Register((byte)chipID, p, 0x44 + c, fmRegisterYM2612[chipID][p][0x44 + c], EnmModel.RealModel, -1);
                    setYM2612Register((byte)chipID, p, 0x48 + c, fmRegisterYM2612[chipID][p][0x48 + c], EnmModel.RealModel, -1);
                    setYM2612Register((byte)chipID, p, 0x4c + c, fmRegisterYM2612[chipID][p][0x4c + c], EnmModel.RealModel, -1);
                }
            }
        }


        public void setYM2151SyncWait(byte chipID, int wait)
        {
            if (scYM2151[chipID] != null && ctYM2151[chipID].UseWait)
            {
                scYM2151[chipID].setRegister(-1, (int)(wait * (ctYM2151[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void setYM2608SyncWait(byte chipID, int wait)
        {
            if (scYM2608[chipID] != null && ctYM2608[chipID].UseWait)
            {
                scYM2608[chipID].setRegister(-1, (int)(wait * (ctYM2608[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void setYM2612SyncWait(byte chipID, int wait)
        {
            if (scYM2612[chipID] != null && ctYM2612[chipID].UseWait)
            {
                scYM2612[chipID].setRegister(-1, (int)(wait * (ctYM2612[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }


        public void sendDataYM2151(byte chipID, EnmModel model)
        {
            if (model == EnmModel.VirtualModel) return;

            if (scYM2151[chipID] != null && ctYM2151[chipID].UseWait)
            {
                realChip.SendData();
                while (!scYM2151[chipID].isBufferEmpty()) { }
            }
        }

        public void sendDataYM2608(byte chipID, EnmModel model)
        {
            if (model == EnmModel.VirtualModel) return;

            if (scYM2608[chipID] != null && ctYM2608[chipID].UseWait)
            {
                realChip.SendData();
                while (!scYM2608[chipID].isBufferEmpty()) { }
            }
        }


        public int getYM2151Clock(byte chipID)
        {
            if (scYM2151[chipID] == null) return -1;

            return (int)scYM2151[chipID].dClock;
        }


        public void setSN76489RegisterGGpanning(int chipID, int dData, EnmModel model)
        {
            if (ctSN76489 == null) return;

            if (chipID == 0) chipLED.PriDCSG = 2;
            else chipLED.SecDCSG = 2;

            if (model == EnmModel.RealModel)
            {
                if (ctSN76489[chipID].UseScci)
                {
                    if (scSN76489[chipID] == null) return;
                }
            }
            else
            {
                if (!ctSN76489[chipID].UseScci && ctSN76489[chipID].UseEmu)
                {
                    mds.WriteSN76489GGPanning((byte)chipID, (byte)dData);
                    sn76489RegisterGGPan[chipID] = dData;
                }
            }
        }

        public void setSN76489Register(int chipID, int dData, EnmModel model)
        {
            if (ctSN76489 == null) return;

            if (chipID == 0) chipLED.PriDCSG = 2;
            else chipLED.SecDCSG = 2;

            SN76489_Write(chipID, dData);

            if ((dData & 0x90) == 0x90)
            {
                sn76489Vol[chipID][(dData & 0x60) >> 5][0] = (15 - (dData & 0xf)) * ((sn76489RegisterGGPan[chipID] >> (((dData & 0x60) >> 5) + 4)) & 0x1);
                sn76489Vol[chipID][(dData & 0x60) >> 5][1] = (15 - (dData & 0xf)) * ((sn76489RegisterGGPan[chipID] >> ((dData & 0x60) >> 5)) & 0x1);

                int v = dData & 0xf;
                v = v + nowSN76489FadeoutVol[chipID];
                v += maskChSN76489[chipID][(dData & 0x60) >> 5] ? 15 : 0;
                v = Math.Min(v, 15);
                dData = (dData & 0xf0) | v;
            }

            if (model == EnmModel.RealModel)
            {
                if (ctSN76489[chipID].UseScci)
                {
                    if (scSN76489[chipID] == null) return;
                    scSN76489[chipID].setRegister(0, dData);
                }
            }
            else
            {
                if (!ctSN76489[chipID].UseScci && ctSN76489[chipID].UseEmu)
                {
                    mds.WriteSN76489((byte)chipID, (byte)dData);
                }
            }
        }

        public void setSN76489SyncWait(byte chipID, int wait)
        {
            if (scSN76489 != null && ctSN76489[chipID].UseWait)
            {
                scSN76489[chipID].setRegister(-1, (int)(wait * (ctSN76489[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void setFadeoutVolSN76489(byte chipID, int v)
        {
            nowSN76489FadeoutVol[chipID] = (v & 0x78) >> 3;
            for (int c = 0; c < 4; c++)
            {

                setSN76489Register(chipID, 0x90 + (c << 5) + sn76489Register[chipID][1 + (c << 1)], EnmModel.RealModel);
            }
        }

        private void SN76489_Write(int chipID, int data)
        {
            if ((data & 0x80) > 0)
            {
                /* Latch/data byte  %1 cc t dddd */
                LatchedRegister[chipID] = (data >> 4) & 0x07;
                sn76489Register[chipID][LatchedRegister[chipID]] =
                    (sn76489Register[chipID][LatchedRegister[chipID]] & 0x3f0) /* zero low 4 bits */
                    | (data & 0xf);                            /* and replace with data */
            }
            else
            {
                /* Data byte        %0 - dddddd */
                if ((LatchedRegister[chipID] % 2) == 0 && (LatchedRegister[chipID] < 5))
                    /* Tone register */
                    sn76489Register[chipID][LatchedRegister[chipID]] =
                        (sn76489Register[chipID][LatchedRegister[chipID]] & 0x00f) /* zero high 6 bits */
                        | ((data & 0x3f) << 4);                 /* and replace with data */
                else
                    /* Other register */
                    sn76489Register[chipID][LatchedRegister[chipID]] = data & 0x0f; /* Replace with data */
            }
            switch (LatchedRegister[chipID])
            {
                case 0:
                case 2:
                case 4: /* Tone channels */
                    //if (sn76489Register[chipID][LatchedRegister[chipID]] == 0)
                        //sn76489Register[chipID][LatchedRegister[chipID]] = 1; /* Zero frequency changed to 1 to avoid div/0 */
                    break;
                case 6: /* Noise */
                    NoiseFreq[chipID] = 0x10 << (sn76489Register[chipID][6] & 0x3); /* set noise signal generator frequency */
                    break;
            }
        }


        public void writeRF5C68PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr, EnmModel model)
        {
            //if (chipid == 0) chipLED.PriRF5C = 2;
            //else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C68PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeRF5C68(byte chipid, byte adr, byte data, EnmModel model)
        {

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteRF5C68(chipid, adr, data);
            }
        }

        public void writeRF5C68MemW(byte chipid, uint offset, byte data, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C68MemW(chipid, offset, data);
        }

        public void writeRF5C164PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C164PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeNESPCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteNESRam(chipid, (int)stAdr, (int)dataSize, vgmBuf, (int)vgmAdr);
        }

        public void writeRF5C164(byte chipid, byte adr, byte data, EnmModel model)
        {
            if (chipid == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
            {
                if (adr == 0x08)
                {
                    data = (byte)(
                        (maskChRF5C164[chipid][0] ? 0 : (data & 0x01))
                        | (maskChRF5C164[chipid][1] ? 0 : (data & 0x02))
                        | (maskChRF5C164[chipid][2] ? 0 : (data & 0x04))
                        | (maskChRF5C164[chipid][3] ? 0 : (data & 0x08))
                        | (maskChRF5C164[chipid][4] ? 0 : (data & 0x10))
                        | (maskChRF5C164[chipid][5] ? 0 : (data & 0x20))
                        | (maskChRF5C164[chipid][6] ? 0 : (data & 0x40))
                        | (maskChRF5C164[chipid][7] ? 0 : (data & 0x80))
                        );
                }

                mds.WriteRF5C164(chipid, adr, data);
            }
        }

        public void writeRF5C164MemW(byte chipid, uint offset, byte data, EnmModel model)
        {
            if (chipid == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteRF5C164MemW(chipid, offset, data);
        }

        public void writePWM(byte chipid, byte adr, uint data, EnmModel model)
        {
            if (chipid == 0) chipLED.PriPWM = 2;
            else chipLED.SecPWM = 2;

            if (model == EnmModel.VirtualModel)
                mds.WritePWM(chipid, adr, data);
        }

        public void writeK051649(byte chipid, uint adr, byte data, EnmModel model)
        {
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

        public void writeK053260(byte chipid, uint adr, byte data, EnmModel model)
        {
            if (chipid == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK053260(chipid, (byte)adr, data);
        }

        public void writeK054539(byte chipid, uint adr, byte data, EnmModel model)
        {
            if (chipid == 0) chipLED.PriK054539 = 2;
            else chipLED.SecK054539 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK054539(chipid, (int)adr, data);
        }

        public void writeK053260PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK053260PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeK054539PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriK054539 = 2;
            else chipLED.SecK054539 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteK054539PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeQSoundPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriQsnd = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteQSoundPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeC352(byte chipid, uint adr, uint data, EnmModel model)
        {
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

        public void writeC352PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriC352 = 2;
            else chipLED.SecC352 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteC352PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeGA20PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriGA20 = 2;
            else chipLED.SecGA20 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteGA20PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeOKIM6258(byte ChipID, byte Port, byte Data, EnmModel model)
        {
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

        public void writeOKIM6295(byte ChipID, byte Port, byte Data, EnmModel model)
        {
            if (ChipID == 0) chipLED.PriOKI9 = 2;
            else chipLED.SecOKI9 = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteOKIM6295(ChipID, Port, Data);
                //System.Console.WriteLine("ChipID={0} Port={1:X} Data={2:X} ",ChipID,Port,Data);
            }
        }

        public void writeOKIM6295PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriOKI9 = 2;
            else chipLED.SecOKI9 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteOKIM6295PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeMultiPCMPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteMultiPCMPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMF271PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriOPX = 2;
            else chipLED.SecOPX = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteYMF271PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMF278BPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriOPL4 = 2;
            else chipLED.SecOPL4 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteYMF278BPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMZ280BPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriYMZ = 2;
            else chipLED.SecYMZ = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteYMZ280BPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeY8950PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipid == 0) chipLED.PriY8950 = 2;
            else chipLED.SecY8950 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteY8950PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeSEGAPCM(byte chipID, int offset, byte data, EnmModel model)
        {
            if (chipID == 0) chipLED.PriSPCM = 2;
            else chipLED.SecSPCM = 2;

            if (
                (model == EnmModel.VirtualModel && (ctSEGAPCM[chipID] == null || !ctSEGAPCM[chipID].UseScci))
                || (model == EnmModel.RealModel && (scSEGAPCM != null && scSEGAPCM[chipID] != null))
                )
            {
                pcmRegisterSEGAPCM[chipID][offset & 0x1ff] = data;

                if ((offset & 0x87) == 0x86)
                {
                    byte ch = (byte)((offset >> 3) & 0xf);
                    if ((data & 0x01) == 0) pcmKeyOnSEGAPCM[chipID][ch] = true;
                    data = (byte)(maskChSegaPCM[chipID][ch] ? (data | 0x01) : data);
                }
            }

            if (model == EnmModel.VirtualModel)
            {
                if (!ctSEGAPCM[chipID].UseScci)
                    mds.WriteSEGAPCM(chipID, offset, data);
                //System.Console.WriteLine("ChipID={0} Offset={1:X} Data={2:X} ", ChipID, Offset, Data);
            }
            else
            {
                if (scSEGAPCM != null && scSEGAPCM[chipID] != null) scSEGAPCM[chipID].setRegister(offset, data);
            }
        }

        public void writeSEGAPCMPCMData(byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipID == 0) chipLED.PriSPCM = 2;
            else chipLED.SecSPCM = 2;

            if (model == EnmModel.VirtualModel)
            {
                mds.WriteSEGAPCMPCMData(chipID, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            }
            else
            {
                if (scSEGAPCM != null && scSEGAPCM[chipID] != null)
                {
                    // スタートアドレス設定
                    scSEGAPCM[chipID].setRegister(0x10000, (byte)(DataStart));
                    scSEGAPCM[chipID].setRegister(0x10001, (byte)(DataStart >> 8));
                    scSEGAPCM[chipID].setRegister(0x10002, (byte)(DataStart >> 16));
                    // データ転送
                    for (int cnt = 0; cnt < DataLength; cnt++)
                    {
                        scSEGAPCM[chipID].setRegister(0x10004, romdata[SrcStartAdr + cnt]);
                    }
                    scSEGAPCM[chipID].setRegister(0x10006, (int)ROMSize);

                    realChip.SendData();
                }
            }
        }

        public void writeYM2151Clock(byte chipID, int clock, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scYM2151 != null && scYM2151[chipID] != null)
                {
                    scYM2151[chipID].dClock = scYM2151[chipID].SetMasterClock((uint)clock);                    
                }
            }
        }

        public void writeYM2203Clock(byte chipID, int clock, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scYM2203 != null && scYM2203[chipID] != null)
                {
                    if(scYM2203[chipID] is RC86ctlSoundChip)
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
        }

        public void writeYM2608Clock(byte chipID, int clock, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scYM2608 != null && scYM2608[chipID] != null)
                {
                    scYM2608[chipID].dClock = scYM2608[chipID].SetMasterClock((uint)clock);
                }
            }
        }

        public void setYM2203SSGVolume(byte chipID, int vol, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scYM2203 != null && scYM2203[chipID] != null)
                {
                    scYM2203[chipID].setSSGVolume((byte)vol);
                }
            }
        }

        public void setYM2608SSGVolume(byte chipID, int vol, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scYM2608 != null && scYM2608[chipID] != null)
                {
                    scYM2608[chipID].setSSGVolume((byte)vol);
                }
            }
        }

        public void writeSEGAPCMClock(byte chipID, int clock, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scSEGAPCM != null && scSEGAPCM[chipID] != null)
                {
                    scSEGAPCM[chipID].setRegister(0x10005, (int)clock);
                }
            }
        }

        public void writeC140(byte chipID, uint adr, byte data, EnmModel model)
        {
            if (chipID == 0) chipLED.PriC140 = 2;
            else chipLED.SecC140 = 2;

            if (
                (model == EnmModel.VirtualModel && (ctC140[chipID] == null || !ctC140[chipID].UseScci))
                || (model == EnmModel.RealModel && (scC140 != null && scC140[chipID] != null))
                )
            {
                pcmRegisterC140[chipID][adr] = data;
                byte ch = (byte)(adr >> 4);
                switch (adr & 0xf)
                {
                    case 0x05:
                        if ((data & 0x80) != 0)
                        {
                            pcmKeyOnC140[chipID][ch] = true;
                            data = (byte)(maskChC140[chipID][ch] ? (data & 0x7f) : data);
                        }
                        break;
                }
            }

            if (model == EnmModel.VirtualModel)
            {
                if (ctC140[chipID] == null || !ctC140[chipID].UseScci)
                    mds.WriteC140(chipID, adr, data);
            }
            else
            {
                if (scC140 != null && scC140[chipID] != null) scC140[chipID].setRegister((int)adr, data);
            }
        }

        public void writeC140PCMData(byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, EnmModel model)
        {
            if (chipID == 0) chipLED.PriC140 = 2;
            else chipLED.SecC140 = 2;

            if (model == EnmModel.VirtualModel)
                mds.WriteC140PCMData(chipID, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            else
            {
                if (scC140 != null && scC140[chipID] != null)
                {
                    // スタートアドレス設定
                    scC140[chipID].setRegister(0x10000, (byte)(DataStart));
                    scC140[chipID].setRegister(0x10001, (byte)(DataStart >> 8));
                    scC140[chipID].setRegister(0x10002, (byte)(DataStart >> 16));
                    // データ転送
                    for (int cnt = 0; cnt < DataLength; cnt++)
                    {
                        scC140[chipID].setRegister(0x10004, romdata[SrcStartAdr + cnt]);
                    }
                    //scC140[chipID].setRegister(0x10006, (int)ROMSize);

                    realChip.SendData();
                }
            }
        }

        public void writeC140Type(byte chipID, MDSound.c140.C140_TYPE type, EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
            }
            else
            {
                if (scC140 != null && scC140[chipID] != null)
                {
                    switch (type)
                    {
                        case MDSound.c140.C140_TYPE.SYSTEM2:
                            scC140[chipID].setRegister(0x10008, 0);
                            break;
                        case MDSound.c140.C140_TYPE.SYSTEM21:
                            scC140[chipID].setRegister(0x10008, 1);
                            break;
                        case MDSound.c140.C140_TYPE.ASIC219:
                            scC140[chipID].setRegister(0x10008, 2);
                            break;
                    }
                }
            }
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
                    if (fmVolYM2151[chipID][i] > 0) { fmVolYM2151[chipID][i] -= 50; if (fmVolYM2151[chipID][i] < 0) fmVolYM2151[chipID][i] = 0; }
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
            return fmVolYM2151[chipID];
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

            return sn76489Vol[chipID];

        }

        internal byte[] getVRC7Register(int chipID)
        {
            if (nes_vrc7 == null) return null;
            if (chipID != 0) return null;

            return nes_vrc7.GetVRC7regs();
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
