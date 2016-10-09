using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    class MDChipParams
    {
        public int Cminutes = 0;
        public int Csecond = 0;
        public int Cmillisecond = 0;

        public int TCminutes = 0;
        public int TCsecond = 0;
        public int TCmillisecond = 0;

        public int LCminutes = 0;
        public int LCsecond = 0;
        public int LCmillisecond = 0;

        public int ChipPriOPN = 0;
        public int ChipPriOPN2 = 0;
        public int ChipPriOPNA = 0;
        public int ChipPriOPNB = 0;
        public int ChipPriOPM = 0;
        public int ChipPriDCSG = 0;
        public int ChipPriRF5C = 0;
        public int ChipPriPWM = 0;
        public int ChipPriOKI5 = 0;
        public int ChipPriOKI9 = 0;
        public int ChipPriC140 = 0;
        public int ChipPriSPCM = 0;
        public int ChipSecOPN = 0;
        public int ChipSecOPN2 = 0;
        public int ChipSecOPNA = 0;
        public int ChipSecOPNB = 0;
        public int ChipSecOPM = 0;
        public int ChipSecDCSG = 0;
        public int ChipSecRF5C = 0;
        public int ChipSecPWM = 0;
        public int ChipSecOKI5 = 0;
        public int ChipSecOKI9 = 0;
        public int ChipSecC140 = 0;
        public int ChipSecSPCM = 0;


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

            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public YM2203[] ym2203 = new YM2203[] { new YM2203(), new YM2203() };

        public class YM2612
        {

            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public YM2612 ym2612 = new YM2612();

        public class SN76489
        {

            public Channel[] channels = new Channel[4] { new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public SN76489 sn76489 = new SN76489();

        public class RF5C164
        {
            public Channel[] channels = new Channel[8] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public RF5C164 rf5c164 = new RF5C164();

        public class C140
        {
            public Channel[] channels = new Channel[24] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };
        }
        public C140 c140 = new C140();

        public class YM2151
        {

            public Channel[] channels = new Channel[8] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel()};

        }
        public YM2151 ym2151 = new YM2151();

        public class YM2608
        {

            public Channel[] channels = new Channel[19] {
                new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() //FM 0
                ,new Channel(), new Channel(), new Channel() //SSG 9
                ,new Channel() //ADPCM 12
                ,new Channel(), new Channel(), new Channel(),new Channel(), new Channel(), new Channel() //RHYTHM 13
            };

        }
        public YM2608 ym2608 = new YM2608();

    }
}
