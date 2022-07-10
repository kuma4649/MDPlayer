using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class MDChipParams
    {
        public int Cminutes = -1;
        public int Csecond = -1;
        public int Cmillisecond = -1;

        public int TCminutes = -1;
        public int TCsecond = -1;
        public int TCmillisecond = -1;

        public int LCminutes = -1;
        public int LCsecond = -1;
        public int LCmillisecond = -1;

        public ChipLEDs chipLED = new ChipLEDs();



        public class Channel
        {

            public int pan = -1;
            public int pantp = -1;
            public int note = -1;
            public int volume = -1;
            public int volumeL = -1;
            public int volumeR = -1;
            public int volumeRL = -1;
            public int volumeRR = -1;
            public int flg16 = -1;
            public int srcFreq = -1;
            public int freq = -1;
            public int bank = -1;
            public int sadr = -1;
            public int eadr = -1;
            public int ladr = -1;
            public int leadr = -1;
            public int pcmMode = -1;
            public int pcmBuff = 0;
            public bool? mask = false;
            public byte slot = 0;
            public int tp = -1;
            public int kf = -1;//OPM only
            public int tn = 0;//PSG only
            public bool ex = false;//OPN/2/A/B
            public int tntp = -1;
            public bool dda = false;//HuC6280
            public bool noise = false;//HuC6280
            public int nfrq = -1;//HuC6280
            public bool loopFlg = false;//YMZ280B
            public int echo = -1;

            public int[] inst = new int[67];
            public int[] typ = new int[67];
            public bool[] bit = new bool[67];
            public short[] aryWave16bit = null;
            public byte[] PSGWave = null;

            public Channel()
            {
                aryWave16bit = null;
                for (int i = 0; i < inst.Length; i++)
                {
                    inst[i] = -1;
                    typ[i] = 0;
                    bit[i] = false;
                }
            }
        }

        public class YM2203
        {
            public int nfrq = -1;
            public int efrq = -1;
            public int etype = -1;
            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public YM2203[] ym2203 = new YM2203[] { new YM2203(), new YM2203() };

        public class YM2612
        {
            public EnmFileFormat fileFormat = EnmFileFormat.unknown;
            public bool lfoSw = false;
            public int lfoFrq = -1;
            public int timerA = -1;
            public int timerB = -1;
            public int[] xpcmVolL = new int[4] { -1, -1, -1, -1 };
            public int[] xpcmVolR = new int[4] { -1, -1, -1, -1 };
            public int[] xpcmInst = new int[4] { -1, -1, -1, -1 };

            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public YM2612[] ym2612 = new YM2612[] { new YM2612(), new YM2612() };

        public class SN76489
        {

            public Channel[] channels = new Channel[4] { new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public SN76489[] sn76489 = new SN76489[] { new SN76489(), new SN76489() };

        public class RF5C164
        {
            public Channel[] channels = new Channel[8] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public RF5C164[] rf5c164 = new RF5C164[] { new RF5C164(), new RF5C164() };

        public class RF5C68
        {
            public Channel[] channels = new Channel[8] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public RF5C68[] rf5c68 = new RF5C68[] { new RF5C68(), new RF5C68() };

        public class C140
        {
            public Channel[] channels = new Channel[24] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public C140[] c140 = new C140[] { new C140(), new C140() };

        public class C352
        {
            public Channel[] channels = new Channel[32] {
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),

                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel()
            };
        }
        public C352[] c352 = new C352[] { new C352(), new C352() };

        public class MultiPCM
        {
            public Channel[] channels = new Channel[28] {
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),

                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel()
            };
        }
        public MultiPCM[] multiPCM = new MultiPCM[] { new MultiPCM(), new MultiPCM() };

        public class YMZ280B
        {
            public Channel[] channels = new Channel[8] {
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel()
            };
        }
        public YMZ280B[] ymz280b = new YMZ280B[] { new YMZ280B(), new YMZ280B() };

        public class QSound
        {
            public Channel[] channels = new Channel[19] {
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),

                new Channel(), new Channel(), new Channel()
            };
        }
        public QSound[] qSound = new QSound[] { new QSound(), new QSound() };

        public class OKIM6258
        {
            public int pan = -1;
            public int pantp = -1;
            public int masterFreq = -1;
            public int divider=-1;
            public int pbFreq = -1;
            public int volumeL = -1;
            public int volumeR = -1;
            public bool keyon = false;
            public bool? mask = false;

        }
        public OKIM6258[] okim6258 = new OKIM6258[] { new OKIM6258(), new OKIM6258() };

        public class OKIM6295
        {
            public Channel[] channels = new Channel[4] { new Channel(), new Channel(), new Channel(), new Channel() };

            public uint masterClock = 0;
            public int pin7State = 0;
            public int[] nmkBank = new int[4];
        }
        public OKIM6295[] okim6295 = new OKIM6295[] { new OKIM6295(), new OKIM6295() };

        public class SegaPcm
        {
            public Channel[] channels = new Channel[16] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public SegaPcm[] segaPcm = new SegaPcm[] { new SegaPcm(), new SegaPcm() };

        public class AY8910
        {
            public int nfrq = -1;
            public int efrq = -1;
            public int etype = -1;
            public Channel[] channels = new Channel[3] { new Channel(), new Channel(), new Channel() };
        }
        public AY8910[] ay8910 = new AY8910[] { new AY8910(), new AY8910() };

        public class S5B
        {
            public int nfrq = -1;
            public int efrq = -1;
            public int etype = -1;
            public Channel[] channels = new Channel[3] { new Channel(), new Channel(), new Channel() };
        }
        public S5B[] s5b = new S5B[] { new S5B(), new S5B() };

        public class DMG
        {
            public byte[] wf = new byte[32];
            public Channel[] channels = new Channel[4] { new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public DMG[] dmg = new DMG[] { new DMG(), new DMG() };

        public class YM2151
        {
            public int ne = -1;
            public int nfrq = -1;
            public int lfrq = -1;
            public int pmd = -1;
            public int amd = -1;
            public int waveform = -1;
            public int lfosync = -1;
            public Channel[] channels = new Channel[8] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()};

        }
        public YM2151[] ym2151 = new YM2151[] { new YM2151(), new YM2151() };

        public class YM2608
        {
            public bool lfoSw = false;
            public int lfoFrq = -1;
            public int nfrq = -1;
            public int efrq = -1;
            public int etype = -1;
            public int timerA = -1;
            public int timerB = -1;
            public int rhythmTotalLevel = -1;
            public int adpcmLevel = -1;
            
            public Channel[] channels = new Channel[19] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //FM 0
                ,new Channel(), new Channel(), new Channel() //SSG 9
                ,new Channel() //ADPCM 12
                ,new Channel(), new Channel(), new Channel(),new Channel(), new Channel(), new Channel() //RHYTHM 13
            };

        }
        public YM2608[] ym2608 = new YM2608[] { new YM2608(), new YM2608() };

        public class YM2609
        {
            public bool lfoSw = false;
            public int lfoFrq = -1;
            public int nfrq = -1;
            public int efrq = -1;
            public int etype = -1;
            public int timerA = -1;
            public int timerB = -1;
            public int rhythmTotalLevel = -1;
            public int adpcmLevel = -1;

            public Channel[] channels = new Channel[] {
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(), new Channel(),
                new Channel(), new Channel(),new Channel() //FM 0-11 12-17
                ,new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel() //SSG 18-30
                ,new Channel(), new Channel(), new Channel(),new Channel(), new Channel(), new Channel() //RHYTHM 
                ,new Channel(),new Channel(),new Channel() //ADPCM012
                ,new Channel(), new Channel(), new Channel(),new Channel(), new Channel(), new Channel() //ADPCM-A
            };

        }
        public YM2609[] ym2609 = new YM2609[] { new YM2609(), new YM2609() };

        public class YM2610
        {
            public bool lfoSw = false;
            public int lfoFrq = -1;
            public int nfrq = -1;
            public int efrq = -1;
            public int etype = -1;

            public Channel[] channels = new Channel[19] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //FM 0
                ,new Channel(), new Channel(), new Channel() //SSG 9
                ,new Channel() //ADPCM 12
                ,new Channel(), new Channel(), new Channel(),new Channel(), new Channel(), new Channel() //RHYTHM 13
            };

        }
        public YM2610[] ym2610 = new YM2610[] { new YM2610(), new YM2610() };

        public class HuC6280
        {
            public int mvolL = -1;
            public int mvolR = -1;
            public int LfoCtrl = -1;
            public int LfoFrq = -1;

            public Channel[] channels = new Channel[6] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public HuC6280[] huc6280 = new HuC6280[] { new HuC6280(), new HuC6280() };

        public class K051649
        {

            public Channel[] channels = new Channel[5] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public K051649[] k051649 = new K051649[] { new K051649(), new K051649() };

        public class YM2413
        {

            public Channel[] channels = new Channel[14] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //FM 9
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //Ryhthm 5
            };

        }
        public YM2413[] ym2413 = new YM2413[] { new YM2413(), new YM2413() };

        public class YM2612MIDI
        {
            public bool lfoSw = false;
            public int lfoFrq = -1;
            public bool IsMONO = true;
            public int useFormat = 0;
            public int selectCh = -1;
            public int selectParam = -1;

            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

            public int[][] noteLog = new int[6][] { new int[10], new int[10], new int[10], new int[10], new int[10], new int[10] };
            public bool[] useChannel = new bool[6] { false, false, false, false, false, false };
        }
        public YM2612MIDI ym2612Midi = new YM2612MIDI();

        public class YM3526
        {
            public Channel[] channels = new Channel[9 + 5] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel() //FM 9
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //Ryhthm 5
            };

        }
        public YM3526[] ym3526 = new YM3526[] { new YM3526(), new YM3526() };

        public class Y8950
        {
            public Channel[] channels = new Channel[9 + 5+1] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel() //FM 9
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //Ryhthm 5
                ,new Channel()//ADPCM
            };

        }
        public Y8950[] y8950 = new Y8950[] { new Y8950(), new Y8950() };

        public class YM3812
        {
            public Channel[] channels = new Channel[9 + 5] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel() //FM 9
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //Ryhthm 5
            };

        }
        public YM3812[] ym3812 = new YM3812[] { new YM3812(), new YM3812() };

        public class YMF262
        {

            public Channel[] channels = new Channel[18 + 5 ] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel() //FM 18
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //Ryhthm 5
            };

        }
        public YMF262[] ymf262 = new YMF262[] { new YMF262(), new YMF262() };

        public class YMF271
        {
            public Channel[] channels = new Channel[48] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
            };
        }
        public YMF271[] ymf271 = new YMF271[] { new YMF271(), new YMF271() };

        public class YMF278B
        {

            public Channel[] channels = new Channel[18 + 5 + 24] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                , new Channel(), new Channel(), new Channel() //FM 18
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //Ryhthm 5
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel() //PCM 24
            };

        }
        public YMF278B[] ymf278b = new YMF278B[] { new YMF278B(), new YMF278B() };

        public MIDIParam[] midi = new MIDIParam[] { new MIDIParam(), new MIDIParam() };

        public class NESDMC
        {
            public Channel[] sqrChannels = new Channel[2] { new Channel(), new Channel() };
            public Channel triChannel = new Channel();
            public Channel noiseChannel = new Channel();
            public Channel dmcChannel = new Channel();
        }
        public NESDMC[] nesdmc = new NESDMC[] { new NESDMC(), new NESDMC() };

        public class FDS
        {
            public Channel channel = new Channel();
            public int[] wave = new int[32];
            public int[] mod = new int[32];

            public bool VolDir = false;
            public int VolSpd = 0;
            public int VolGain = 0;
            public bool VolDi = false;
            public int VolFrq = 0;
            public bool VolHlR = false;

            public bool ModDir = false;
            public int ModSpd = 0;
            public int ModGain = 0;
            public bool ModDi = false;
            public int ModFrq = 0;
            public int ModCnt = 0;

            public int EnvSpd = 0;
            public bool EnvVolSw = false;
            public bool EnvModSw = false;

            public int MasterVol = 0;
            public bool WE = false;
        }
        public FDS[] fds = new FDS[] { new FDS(), new FDS() };

        public class MMC5
        {
            public Channel[] sqrChannels = new Channel[2] { new Channel(), new Channel() };
            public Channel pcmChannel = new Channel();
        }
        public MMC5[] mmc5 = new MMC5[] { new MMC5(), new MMC5() };

        public class VRC6
        {

            public Channel[] channels = new Channel[3] {
                new Channel(), new Channel(), new Channel()
            };

        }
        public VRC6[] vrc6 = new VRC6[] { new VRC6(), new VRC6() };

        public class VRC7
        {

            public Channel[] channels = new Channel[6] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //FM 6
            };

        }
        public VRC7[] vrc7 = new VRC7[] { new VRC7(), new VRC7() };

        public class N106
        {

            public Channel[] channels = new Channel[8] {
                new Channel(), new Channel(), new Channel(), new Channel()
                ,new Channel(), new Channel(), new Channel(), new Channel()
            };

        }
        public N106[] n106 = new N106[] { new N106(), new N106() };

        public class PPZ8
        {
            public Channel[] channels = new Channel[8] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }

        public PPZ8[] ppz8 = new PPZ8[] { new PPZ8(), new PPZ8() };




        public class Mixer
        {
            public class VolumeInfo
            {
                public int Volume = -9999;
                public int VisVolume1 = -1;
                public int VisVolume2 = -1;
                public int VisVol2Cnt = 30;
            }

            public VolumeInfo Master = new VolumeInfo();
            public VolumeInfo YM2151 = new VolumeInfo();
            public VolumeInfo YM2203 = new VolumeInfo();
            public VolumeInfo YM2203FM = new VolumeInfo();
            public VolumeInfo YM2203PSG = new VolumeInfo();
            public VolumeInfo YM2608Adpcm = new VolumeInfo();
            public VolumeInfo YM2608FM = new VolumeInfo();
            public VolumeInfo YM2608PSG = new VolumeInfo();
            public VolumeInfo YM2608Rhythm = new VolumeInfo();
            public VolumeInfo YM2608 = new VolumeInfo();
            public VolumeInfo YM2610AdpcmA = new VolumeInfo();
            public VolumeInfo YM2610AdpcmB = new VolumeInfo();
            public VolumeInfo YM2610FM = new VolumeInfo();
            public VolumeInfo YM2610PSG = new VolumeInfo();
            public VolumeInfo YM2610 = new VolumeInfo();
            public VolumeInfo YM2612 = new VolumeInfo();

            public VolumeInfo YM2413 = new VolumeInfo();
            public VolumeInfo YM3526 = new VolumeInfo();
            public VolumeInfo Y8950 = new VolumeInfo();
            public VolumeInfo YM3812 = new VolumeInfo();
            public VolumeInfo YMF262 = new VolumeInfo();//OPL3
            public VolumeInfo YMF278B = new VolumeInfo();//OPL4
            public VolumeInfo YMF271 = new VolumeInfo();//OPX
            public VolumeInfo AY8910 = new VolumeInfo();
            public VolumeInfo SN76489 = new VolumeInfo();
            public VolumeInfo HuC6280 = new VolumeInfo();
            public VolumeInfo SAA1099 = new VolumeInfo();

            public VolumeInfo RF5C164 = new VolumeInfo();
            public VolumeInfo RF5C68 = new VolumeInfo();
            public VolumeInfo PWM = new VolumeInfo();
            public VolumeInfo OKIM6258 = new VolumeInfo();
            public VolumeInfo OKIM6295 = new VolumeInfo();
            public VolumeInfo C140 = new VolumeInfo();
            public VolumeInfo C352 = new VolumeInfo();
            public VolumeInfo SEGAPCM = new VolumeInfo();
            public VolumeInfo MultiPCM = new VolumeInfo();//MPCM
            public VolumeInfo YMZ280B = new VolumeInfo();//YMZ
            public VolumeInfo K051649 = new VolumeInfo();//K051
            public VolumeInfo K053260 = new VolumeInfo();//K051
            public VolumeInfo K054539 = new VolumeInfo();
            public VolumeInfo QSound = new VolumeInfo();//QSND
            public VolumeInfo GA20 = new VolumeInfo();

            public VolumeInfo APU = new VolumeInfo();
            public VolumeInfo DMC = new VolumeInfo();
            public VolumeInfo FDS = new VolumeInfo();
            public VolumeInfo MMC5 = new VolumeInfo();
            public VolumeInfo N160 = new VolumeInfo();
            public VolumeInfo VRC6 = new VolumeInfo();
            public VolumeInfo VRC7 = new VolumeInfo();
            public VolumeInfo FME7 = new VolumeInfo();
            public VolumeInfo DMG = new VolumeInfo();

            public VolumeInfo PPZ8 = new VolumeInfo();
            public VolumeInfo GimicOPN = new VolumeInfo();
            public VolumeInfo GimicOPNA = new VolumeInfo();
        }
        public Mixer mixer = new Mixer();

    }
}
