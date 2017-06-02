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

        public int ChipPriOPN = -1;
        public int ChipPriOPN2 = -1;
        public int ChipPriOPNA = -1;
        public int ChipPriOPNB = -1;
        public int ChipPriOPM = -1;
        public int ChipPriDCSG = -1;
        public int ChipPriRF5C = -1;
        public int ChipPriPWM = -1;
        public int ChipPriOKI5 = -1;
        public int ChipPriOKI9 = -1;
        public int ChipPriC140 = -1;
        public int ChipPriSPCM = -1;
        public int ChipPriAY10 = -1;
        public int ChipPriOPLL = -1;
        public int ChipPriHuC8 = -1;

        public int ChipSecOPN = -1;
        public int ChipSecOPN2 = -1;
        public int ChipSecOPNA = -1;
        public int ChipSecOPNB = -1;
        public int ChipSecOPM = -1;
        public int ChipSecDCSG = -1;
        public int ChipSecRF5C = -1;
        public int ChipSecPWM = -1;
        public int ChipSecOKI5 = -1;
        public int ChipSecOKI9 = -1;
        public int ChipSecC140 = -1;
        public int ChipSecSPCM = -1;
        public int ChipSecAY10 = -1;
        public int ChipSecOPLL = -1;
        public int ChipSecHuC8 = -1;

        public enmFileFormat fileFormat = enmFileFormat.unknown;


        public class Channel
        {

            public int pan = -1;
            public int pantp = -1;
            public int note = -1;
            public int volume = -1;
            public int volumeL = -1;
            public int volumeR = -1;
            public int freq = -1;
            public int pcmMode = -1;
            public bool mask = false;
            public int tp = -1;
            public int kf = -1;//OPM only
            public int tn = 0;//PSG only
            public int tntp = -1;
            public bool dda = false;//HuC6280
            public bool noise = false;//HuC6280
            public int nfrq = -1;//HuC6280

            public int[] inst = new int[48];

            public Channel()
            {
                for (int i = 0; i < inst.Length; i++)
                {
                    inst[i] = -1;
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
            public bool lfoSw = false;
            public int lfoFrq = -1;
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

        public class C140
        {
            public Channel[] channels = new Channel[24] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public C140[] c140 = new C140[] { new C140(), new C140() };

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

            public Channel[] channels = new Channel[19] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //FM 0
                ,new Channel(), new Channel(), new Channel() //SSG 9
                ,new Channel() //ADPCM 12
                ,new Channel(), new Channel(), new Channel(),new Channel(), new Channel(), new Channel() //RHYTHM 13
            };

        }
        public YM2608[] ym2608 = new YM2608[] { new YM2608(), new YM2608() };

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

            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

            public int[][] noteLog = new int[6][] { new int[10], new int[10], new int[10], new int[10], new int[10], new int[10] };
            public bool[] useChannel = new bool[6] { false, false, false, false, false, false };
        }
        public YM2612MIDI ym2612Midi = new YM2612MIDI();

    }
}
