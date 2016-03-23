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

        public class YM2612
        {

            public class Channel
            {

                public int pan = -1;
                public int note = -1;
                public int volumeL = -1;
                public int volumeR = -1;
                public int freq = -1;
                public int pcmMode = -1;
                public bool mask = false;

                public int[] inst = new int[48];

                public Channel()
                {
                    for(int i=0;i<inst.Length;i++)
                    {
                        inst[i] = -1;
                    }
                }
            }
            public Channel[] channels = new Channel[9] { new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public YM2612 ym2612 = new YM2612();

        public class SN76489
        {

            public class Channel
            {

                public int pan = -1;
                public int note = -1;
                public int volume = -1;
                public int freq = -1;
                public bool mask = false;

            }
            public Channel[] channels = new Channel[4] { new Channel(), new Channel(), new Channel(), new Channel() };

        }
        public SN76489 sn76489 = new SN76489();

    }
}
