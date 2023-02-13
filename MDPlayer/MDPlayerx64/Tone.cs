using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class Tone
    {

        private int _AL = 0;
        private int _FB = 0;
        private int _AMS = 0;
        private int _PMS = 0;

        public string name="(NO NAME)";

        public int AL
        {
            set { this._AL = value & 0x7; }
            get { return this._AL; }
        }
        public int FB
        {
            set { this._FB = value & 0x7; }
            get { return this._FB; }
        }
        public int AMS
        {
            set { this._AMS = value & 0x3; }
            get { return this._AMS; }
        }
        public int PMS
        {
            set { this._PMS = value & 0x7; }
            get { return this._PMS; }
        }

        public Op[] OPs = new Op[4] { new Op(), new Op(), new Op(), new Op() };

        public class Op
        {
            private int _AR = 0;
            private int _DR = 0;
            private int _SR = 0;
            private int _RR = 0;
            private int _SL = 0;
            private int _TL = 127;
            private int _KS = 0;
            private int _ML = 0;
            private int _DT = 0;
            private int _AM = 0;
            private int _SG = 0;
            private int _DT2 = 0;

            public int AR
            {
                set { this._AR = value & 0x1f; }
                get { return this._AR; }
            }
            public int DR
            {
                set { this._DR = value & 0x1f; }
                get { return this._DR; }
            }
            public int SR
            {
                set { this._SR = value & 0x1f; }
                get { return this._SR; }
            }
            public int RR
            {
                set { this._RR = value & 0xf; }
                get { return this._RR; }
            }
            public int SL
            {
                set { this._SL = value & 0xf; }
                get { return this._SL; }
            }
            public int TL
            {
                set { this._TL = value & 0x7f; }
                get { return this._TL; }
            }
            public int KS
            {
                set { this._KS = value & 0x3; }
                get { return this._KS; }
            }
            public int ML
            {
                set { this._ML = value & 0xf; }
                get { return this._ML; }
            }
            public int DT
            {
                set { this._DT = value & 0x7; }
                get { return this._DT; }
            }
            public int AM
            {
                set { this._AM = value & 0x1; }
                get { return this._AM; }
            }
            public int SG
            {
                set { this._SG = value & 0xf; }
                get { return this._SG; }
            }
            public int DT2
            {
                set { this._DT2 = value & 0x3; }
                get { return this._DT2; }
            }
        }
    }
}
