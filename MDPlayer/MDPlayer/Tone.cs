using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class Tone
    {
        public string name="(NO NAME)";

        public int AL = 0;
        public int FB = 0;
        public int AMS = 0;
        public int PMS = 0;
        public Op[] OPs = new Op[4] { new Op(), new Op(), new Op(), new Op() };

        public class Op
        {
            public int AR = 0;
            public int DR = 0;
            public int SR = 0;
            public int RR = 0;
            public int SL = 0;
            public int TL = 127;
            public int KS = 0;
            public int ML = 0;
            public int DT = 0;
            public int AM = 0;
            public int SG = 0;
            public int DT2 = 0;
        }
    }
}
