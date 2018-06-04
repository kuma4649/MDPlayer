namespace NX68Sound
{
    public class lfo
    {
        public global global = null;

        private const int SIZELFOTBL = 512;             // 2^9
        private const int SIZELFOTBL_BITS = 9;
        private const int LFOPRECISION = 4096;	// 2^12
        //#define	PMTBLMAXVAL	(128)
        //#define	PMTBLMAXVAL_BITS	(7)
        //#define	AMTBLMAXVAL	(256)
        //#define	AMTBLMAXVAL_BITS	(8)
        //#define	LFOTIMECYCLE	 1073741824		// 2^30
        //#define LFOTIMECYCLE_BITS	30
        //#define	LFORNDTIMECYCLE	 (LFOTIMECYCLE>>8)		// 2^22
        //#define	CYCLE2PMAM	(30-8)				// log2(LFOTIMECYCLE/SIZEPMAMTBL)
        //#define	LFOHZ		0.0009313900811
        //int		LFOSTEPTBL[256];
        //int		LFOSTEPTBL3[256];		// Wave form 3 用
        //short	PMSTBL[8]={ 0,1,2,4,8,16,64,128 };
        private int[] PMSMUL = new int[8] { 0, 1, 2, 4, 8, 16, 32, 32 };
        private int[] PMSSHL = new int[8] { 0, 0, 0, 0, 0, 0, 1, 2 };

        private int[] Pmsmul = new int[global.N_CH];   // 0, 1, 2, 4, 8, 16, 32, 32
        private int[] Pmsshl = new int[global.N_CH];   // 0, 0, 0, 0, 0,  0,  1,  2
        private int[] Ams = new int[global.N_CH];  // 左シフト回数 31(0), 0(1), 1(2), 2(3)
        private int[] PmdPmsmul = new int[global.N_CH];    // Pmd*Pmsmul[]
        private int Pmd;
        private int Amd;

        private int LfoStartingFlag;    // 0:LFO停止中  1:LFO動作中
        private int LfoOverFlow;    // LFO tのオーバーフロー値
        private int LfoTime;    // LFO専用 t
        private int LfoTimeAdd; // LFO専用Δt
        private int LfoIdx; // LFOテーブルへのインデックス値
        private int LfoSmallCounter;    // LFO周期微調整カウンタ (0～15の値をとる)
        private int LfoSmallCounterStep;    // LFO周期微調整カウンタ用ステップ値 (16～31)
        private int Lfrq;       // LFO周波数設定値 LFRQ
        private int LfoWaveForm;    // LFO wave form

        private int PmTblValue, AmTblValue;
        private int[] PmValue = new int[global.N_CH], AmValue = new int[global.N_CH];

        private sbyte[] PmTbl0 = new sbyte[SIZELFOTBL], PmTbl2 = new sbyte[SIZELFOTBL];
        private byte[] AmTbl0 = new byte[SIZELFOTBL], AmTbl2 = new byte[SIZELFOTBL];

        //inline void CulcTblValue();
        //inline void CulcPmValue(int ch);
        //inline void CulcAmValue(int ch);
        //inline void CulcAllPmValue();
        //inline void CulcAllAmValue();

        //           public:
        //Lfo(void);
        //           ~Lfo() { };

        //           inline void Init();
        //           inline void InitSamprate();

        //           inline void LfoReset();
        //           inline void LfoStart();
        //           inline void SetLFRQ(int n);
        //           inline void SetPMDAMD(int n);
        //           inline void SetWaveForm(int n);
        //           inline void SetPMSAMS(int ch, int n);

        //           inline void Update();
        //           inline int GetPmValue(int ch);
        //           inline int GetAmValue(int ch);


        public lfo(global global)
        {
            this.global = global;

            int i;

            for (i = 0; i < global.N_CH; ++i)
            {
                Pmsmul[i] = 0;
                Pmsshl[i] = 0;
                Ams[i] = 31;
                PmdPmsmul[i] = 0;

                PmValue[i] = 0;
                AmValue[i] = 0;
            }
            Pmd = 0;
            Amd = 0;

            LfoStartingFlag = 0;
            LfoOverFlow = 0;
            LfoTime = 0;
            LfoTimeAdd = 0;
            LfoIdx = 0;
            LfoSmallCounter = 0;
            LfoSmallCounterStep = 0;
            Lfrq = 0;
            LfoWaveForm = 0;

            PmTblValue = 0;
            AmTblValue = 255;

            // PM Wave Form 0,3
            for (i = 0; i <= 127; ++i)
            {
                PmTbl0[i] = (sbyte)i;
                PmTbl0[i + 128] = (sbyte)(i - 127);
                PmTbl0[i + 256] = (sbyte)i;
                PmTbl0[i + 384] = (sbyte)(i - 127);
            }
            // AM Wave Form 0,3
            for (i = 0; i <= 255; ++i)
            {
                AmTbl0[i] = (byte)(255 - i);
                AmTbl0[i + 256] = (byte)(255 - i);

            }

            // PM Wave Form 2
            for (i = 0; i <= 127; ++i)
            {
                PmTbl2[i] = (sbyte)i;
                PmTbl2[i + 128] = (sbyte)(127 - i);
                PmTbl2[i + 256] = (sbyte)-i;
                PmTbl2[i + 384] = (sbyte)(i - 127);
            }
            // AM Wave Form 2
            for (i = 0; i <= 255; ++i)
            {
                AmTbl2[i] = (byte)(255 - i);
                AmTbl2[i + 256] = (byte)i;
            }

        }

        public void Init()
        {
            LfoTimeAdd = LFOPRECISION * global.OpmRate / global.Samprate;

            LfoSmallCounter = 0;

            SetLFRQ(0);
            SetPMDAMD(0);
            SetPMDAMD(128 + 0);
            SetWaveForm(0);
            {
                int ch;
                for (ch = 0; ch < global.N_CH; ++ch)
                {
                    SetPMSAMS(ch, 0);
                }
            }
            LfoReset();
            LfoStart();
        }

        public void InitSamprate()
        {
            LfoTimeAdd = LFOPRECISION * global.OpmRate / global.Samprate;
        }

        public void LfoReset()
        {
            LfoStartingFlag = 0;

            //	LfoTime はリセットされない！！
            LfoIdx = 0;

            CulcTblValue();
            CulcAllPmValue();
            CulcAllAmValue();
        }

        public void LfoStart()
        {
            LfoStartingFlag = 1;
        }

        public void SetLFRQ(int n)
        {
            Lfrq = n & 255;

            LfoSmallCounterStep = 16 + (Lfrq & 15);
            int shift = 15 - (Lfrq >> 4);
            if (shift == 0)
            {
                shift = 1;
                LfoSmallCounterStep <<= 1;
            }
            LfoOverFlow = (8 << shift) * LFOPRECISION;

            //	LfoTime はリセットされる
            LfoTime = 0;
        }

        public void SetPMDAMD(int n)
        {
            if ((n & 0x80) != 0)
            {
                Pmd = n & 0x7F;
                int ch;
                for (ch = 0; ch < global.N_CH; ++ch)
                {
                    PmdPmsmul[ch] = Pmd * Pmsmul[ch];
                }
                CulcAllPmValue();
            }
            else
            {
                Amd = n & 0x7F;
                CulcAllAmValue();
            }
        }

        public void SetWaveForm(int n)
        {
            LfoWaveForm = n & 3;

            CulcTblValue();
            CulcAllPmValue();
            CulcAllAmValue();
        }

        public void SetPMSAMS(int ch, int n)
        {
            int pms = (n >> 4) & 7;
            Pmsmul[ch] = PMSMUL[pms];
            Pmsshl[ch] = PMSSHL[pms];
            PmdPmsmul[ch] = Pmd * Pmsmul[ch];
            CulcPmValue(ch);

            Ams[ch] = ((n & 3) - 1) & 31;
            CulcAmValue(ch);
        }

        public void Update()
        {
            if (LfoStartingFlag == 0)
            {
                return;
            }

            LfoTime += LfoTimeAdd;
            //2008.4.19 sam 修正 LfoTimeの誤差を小さくするため,残余を保存する
            //	if (LfoTime >= LfoOverFlow) {
            //		LfoTime = 0;
            while (LfoTime >= LfoOverFlow)
            {
                LfoTime -= LfoOverFlow;
                LfoSmallCounter += LfoSmallCounterStep;
                switch (LfoWaveForm)
                {
                    case 0:
                        {
                            int idxadd = LfoSmallCounter >> 4;
                            LfoIdx = (LfoIdx + idxadd) & (SIZELFOTBL - 1);
                            PmTblValue = PmTbl0[LfoIdx];
                            AmTblValue = AmTbl0[LfoIdx];
                            break;
                        }
                    case 1:
                        {
                            int idxadd = LfoSmallCounter >> 4;
                            LfoIdx = (LfoIdx + idxadd) & (SIZELFOTBL - 1);
                            if ((LfoIdx & (SIZELFOTBL / 2 - 1)) < SIZELFOTBL / 4)
                            {
                                PmTblValue = 128;
                                AmTblValue = 256;
                            }
                            else
                            {
                                PmTblValue = -128;
                                AmTblValue = 0;
                            }
                        }
                        break;
                    case 2:
                        {
                            int idxadd = LfoSmallCounter >> 4;
                            LfoIdx = (LfoIdx + idxadd + idxadd) & (SIZELFOTBL - 1);
                            PmTblValue = PmTbl2[LfoIdx];
                            AmTblValue = AmTbl2[LfoIdx];
                            break;
                        }
                    case 3:
                        {
                            LfoIdx = (int)(global.irnd() >> (32 - SIZELFOTBL_BITS));
                            PmTblValue = PmTbl0[LfoIdx];
                            AmTblValue = AmTbl0[LfoIdx];
                            break;
                        }
                }
                LfoSmallCounter &= 15;

                CulcAllPmValue();
                CulcAllAmValue();
            }
        }

        public int GetPmValue(int ch)
        {
            return PmValue[ch];
        }

        public int GetAmValue(int ch)
        {
            return AmValue[ch];
        }

        public void CulcTblValue()
        {
            switch (LfoWaveForm)
            {
                case 0:
                    PmTblValue = PmTbl0[LfoIdx];
                    AmTblValue = AmTbl0[LfoIdx];
                    break;
                case 1:
                    if ((LfoIdx & (SIZELFOTBL / 2 - 1)) < SIZELFOTBL / 4)
                    {
                        PmTblValue = 128;
                        AmTblValue = 256;
                    }
                    else
                    {
                        PmTblValue = -128;
                        AmTblValue = 0;
                    }
                    break;
                case 2:
                    PmTblValue = PmTbl2[LfoIdx];
                    AmTblValue = AmTbl2[LfoIdx];
                    break;
                case 3:
                    PmTblValue = PmTbl0[LfoIdx];
                    AmTblValue = AmTbl0[LfoIdx];
                    break;
            }
        }

        public void CulcPmValue(int ch)
        {
            if (PmTblValue >= 0)
            {
                PmValue[ch] = ((PmTblValue * PmdPmsmul[ch]) >> (7 + 5)) << Pmsshl[ch];
            }
            else
            {
                PmValue[ch] = -((((-PmTblValue) * PmdPmsmul[ch]) >> (7 + 5)) << Pmsshl[ch]);
            }
        }

        public void CulcAmValue(int ch)
        {
            AmValue[ch] = (((AmTblValue * Amd) >> 7) << Ams[ch]) & (int)0x7FFFFFFF;
        }

        public void CulcAllPmValue()
        {
            int ch;
            for (ch = 0; ch < global.N_CH; ++ch)
            {
                CulcPmValue(ch);
            }
        }

        public void CulcAllAmValue()
        {
            int ch;
            for (ch = 0; ch < global.N_CH; ++ch)
            {
                CulcAmValue(ch);
            }
        }

    }
}
