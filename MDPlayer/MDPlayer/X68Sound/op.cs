using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NX68Sound
{
    public class op
    {
        public global global = null;

        public const int KEYON = -1;
        public const int ATACK = 0;
        public const int DECAY = 1;
        public const int SUSTAIN = 2;
        public const int SUSTAIN_MAX = 3;
        public const int RELEASE = 4;
        public const int RELEASE_MAX = 5;

        public const int CULC_DELTA_T = (0x7FFFFFFF);
        public const int CULC_ALPHA = (0x7FFFFFFF);

        public static int IS_ZERO_CLOSS(int a, int b) { return ((a < 0 && b >= 0) || (a > 0 && b <= 0)) ? 1 : 0; }

        public int[] NEXTSTAT = new int[RELEASE_MAX + 1] {
            DECAY, SUSTAIN, SUSTAIN_MAX, SUSTAIN_MAX, RELEASE_MAX, RELEASE_MAX,
        };
        public int[] MAXSTAT = new int[RELEASE_MAX + 1] {
            ATACK, SUSTAIN_MAX, SUSTAIN_MAX, SUSTAIN_MAX, RELEASE_MAX, RELEASE_MAX,
        };

        public int[] inp=new int[1];           // FM変調の入力
        private int LfoPitch;   // 前回のlfopitch値, CULC_DELTA_T値の時はDeltaTを再計算する。
        private int T;      // 現在時間 (0 <= T < SIZESINTBL*PRECISION)
        private int DeltaT; // Δt
        private int Ame;        // 0(トレモロをかけない), -1(トレモロをかける)
        private int LfoLevel;   // 前回のlfopitch&Ame値, CULC_ALPHA値の時はAlphaを再計算する。
        private int Alpha;  // 最終的なエンベロープ出力値
                            //追加 2006.03.26 sam Lfoの更新をSinテーブルの0クロス時に修正するため
        private bool LfoLevelReCalc;
        private short SinBf;

        public int[] out1 = new int[1];          // オペレータの出力先
        public int[] out2 = new int[1];         // オペレータの出力先(alg=5時のM1用)
        public int[] out3 = new int[1];         // オペレータの出力先(alg=5時のM1用)

        private int Pitch;  // 0<=pitch<10*12*64
        private int Dt1Pitch;   // Step に対する補正量
        private int Mul;    // 0.5*2 1*2 2*2 3*2 ... 15*2
        private int Tl;     // (128-TL)*8

        private int Out2Fb; // フィードバックへの出力値
        private int Inp_last;   // 最後の入力値
        private int Fl;     // フィードバックレベルのシフト値(31,7,6,5,4,3,2,1)
        private int Fl_mask;    // フィードバックのマスク(0,-1)
        public int ArTime=0; // AR専用 t

        private int NoiseCounter;   // Noise用カウンタ
        private int NoiseStep;  // Noise用カウントダウン値
        private int NoiseCycle; // Noise周期 32*2^25(0) ～ 1*2^25(31) NoiseCycle==0の時はノイズオフ
        private int NoiseValue; // ノイズ値  1 or -1

        // エンベロープ関係
        private int Xr_stat;
        private int Xr_el;
        private int Xr_step;
        private int Xr_and;
        private int Xr_cmp;
        private int Xr_add;
        private int Xr_limit;

        private int Note;   // 音階 (0 <= Note < 10*12)
        private int Kc;     // 音階 (1 <= Kc <= 128)
        private int Kf;     // 微調整 (0 <= Kf < 64)
        private int Ar;     // 0 <= Ar < 31
        private int D1r;    // 0 <= D1r < 31
        private int D2r;    // 0 <= D2r < 31
        private int Rr;     // 0 <= Rr < 15
        private int Ks;     // 0 <= Ks <= 3
        private int Dt2;    // Pitch に対する補正量(0, 384, 500, 608)
        private int Dt1;    // DT1の値(0～7)
        private int Nfrq;   // Noiseflag,NFRQの値

        public class _StatTbl
        {
            public int and, cmp, add, limit;
        }
        private _StatTbl[] StatTbl = new _StatTbl[RELEASE_MAX + 1]; // 状態推移テーブル
                                                                    //           ATACK     DECAY   SUSTAIN     SUSTAIN_MAX RELEASE     RELEASE_MAX
                                                                    // and     :                               4097                    4097
                                                                    // cmp     :                               2048                    2048
                                                                    // add     :                               0                       0
                                                                    // limit   : 0         D1l     63          63          63          63
                                                                    // nextstat: DECAY     SUSTAIN SUSTAIN_MAX SUSTAIN_MAX RELEASE_MAX RELEASE_MAX

        private int keyon;
        private int csmkeyon;

        //inline void CulcArStep();
        //inline void CulcD1rStep();
        //inline void CulcD2rStep();
        //inline void CulcRrStep();
        //inline void CulcPitch();
        //inline void CulcDt1Pitch();
        //inline void CulcNoiseCycle();

        //       public:
        //Op(void);
        //       ~Op() { };
        //       inline void Init();
        //       inline void InitSamprate();
        //       inline void SetFL(int n);
        //       inline void SetKC(int n);
        //       inline void SetKF(int n);
        //       inline void SetDT1MUL(int n);
        //       inline void SetTL(int n);
        //       inline void SetKSAR(int n);
        //       inline void SetAMED1R(int n);
        //       inline void SetDT2D2R(int n);
        //       inline void SetD1LRR(int n);
        //       inline void KeyON(int csm);
        //       inline void KeyOFF(int csm);
        //       inline void Envelope(int env_counter);
        //       inline void SetNFRQ(int nfrq);

        //       inline void Output0(int lfopitch, int lfolevel);        // オペレータ0用
        //       inline void Output(int lfopitch, int lfolevel);     // 一般オペレータ用
        //       inline void Output32(int lfopitch, int lfolevel);       // スロット32用







        public op(global global)
        {
            this.global = global;
        }

        public void Init()
        {
            Note = 5 * 12 + 8;
            Kc = 5 * 16 + 8 + 1;
            Kf = 5;
            Ar = 10;
            D1r = 10;
            D2r = 5;
            Rr = 12;
            Ks = 1;
            Dt2 = 0;
            Dt1 = 0;

            ArTime = 0;
            Fl = 31;
            Fl_mask = 0;
            Out2Fb = 0;
            inp[0] = 0;
            Inp_last = 0;
            DeltaT = 0;
            LfoPitch = CULC_DELTA_T;
            T = 0;
            LfoLevel = CULC_ALPHA;
            Alpha = 0;
            Tl = (128 - 127) << 3;
            Xr_el = 1024;
            Xr_step = 0;
            Mul = 2;
            Ame = 0;

            NoiseStep = (int)((Int64)(1 << 26) * (Int64)global.OpmRate / global.Samprate);
            SetNFRQ(0);
            NoiseValue = 1;

            // 状態推移テーブルを作成
            //	StatTbl[ATACK].nextstat = DECAY;
            //	StatTbl[DECAY].nextstat = SUSTAIN;
            //	StatTbl[SUSTAIN].nextstat = SUSTAIN_MAX;
            //	StatTbl[SUSTAIN_MAX].nextstat = SUSTAIN_MAX;
            //	StatTbl[RELEASE].nextstat = RELEASE_MAX;
            //	StatTbl[RELEASE_MAX].nextstat = RELEASE_MAX;

            StatTbl[0] = new _StatTbl();
            StatTbl[1] = new _StatTbl();
            StatTbl[2] = new _StatTbl();
            StatTbl[3] = new _StatTbl();
            StatTbl[4] = new _StatTbl();
            StatTbl[5] = new _StatTbl();
            StatTbl[ATACK].limit = 0;
            StatTbl[DECAY].limit = global.D1LTBL[0];
            StatTbl[SUSTAIN].limit = 63;
            StatTbl[SUSTAIN_MAX].limit = 63;
            StatTbl[RELEASE].limit = 63;
            StatTbl[RELEASE_MAX].limit = 63;

            StatTbl[SUSTAIN_MAX].and = 4097;
            StatTbl[SUSTAIN_MAX].cmp = 2048;
            StatTbl[SUSTAIN_MAX].add = 0;
            StatTbl[RELEASE_MAX].and = 4097;
            StatTbl[RELEASE_MAX].cmp = 2048;
            StatTbl[RELEASE_MAX].add = 0;

            Xr_stat = RELEASE_MAX;
            Xr_and = StatTbl[Xr_stat].and;
            Xr_cmp = StatTbl[Xr_stat].cmp;
            Xr_add = StatTbl[Xr_stat].add;
            Xr_limit = StatTbl[Xr_stat].limit;

            keyon = 0;
            csmkeyon = 0;

            CulcArStep();
            CulcD1rStep();
            CulcD2rStep();
            CulcRrStep();
            CulcPitch();
            CulcDt1Pitch();

            //2006.03.26 追加 sam lfo更新タイミング修正のため
            SinBf = 0;
            LfoLevelReCalc = true;
        }

        public void InitSamprate()
        {
            LfoPitch = CULC_DELTA_T;

            NoiseStep = (int)((Int64)(1 << 26) * (Int64)global.OpmRate / global.Samprate);
            CulcNoiseCycle();

            CulcArStep();
            CulcD1rStep();
            CulcD2rStep();
            CulcRrStep();
            CulcPitch();
            CulcDt1Pitch();
        }

        private void CulcArStep()
        {
            if (Ar != 0)
            {
                int ks = (Ar << 1) + (Kc >> (5 - Ks));
                StatTbl[ATACK].and = global.XRTBL[ks].and;
                StatTbl[ATACK].cmp = global.XRTBL[ks].and >> 1;
                if (ks < 62)
                {
                    StatTbl[ATACK].add = global.XRTBL[ks].add;
                }
                else
                {
                    StatTbl[ATACK].add = 128;
                }
            }
            else
            {
                StatTbl[ATACK].and = 4097;
                StatTbl[ATACK].cmp = 2048;
                StatTbl[ATACK].add = 0;
            }
            if (Xr_stat == ATACK)
            {
                Xr_and = StatTbl[Xr_stat].and;
                Xr_cmp = StatTbl[Xr_stat].cmp;
                Xr_add = StatTbl[Xr_stat].add;
            }
        }

        private void CulcD1rStep()
        {
            if (D1r != 0)
            {
                int ks = (D1r << 1) + (Kc >> (5 - Ks));
                StatTbl[DECAY].and = global.XRTBL[ks].and;
                StatTbl[DECAY].cmp = global.XRTBL[ks].and >> 1;
                StatTbl[DECAY].add = global.XRTBL[ks].add;
            }
            else
            {
                StatTbl[DECAY].and = 4097;
                StatTbl[DECAY].cmp = 2048;
                StatTbl[DECAY].add = 0;
            }
            if (Xr_stat == DECAY)
            {
                Xr_and = StatTbl[Xr_stat].and;
                Xr_cmp = StatTbl[Xr_stat].cmp;
                Xr_add = StatTbl[Xr_stat].add;
            }
        }

        private void CulcD2rStep()
        {
            if (D2r != 0)
            {
                int ks = (D2r << 1) + (Kc >> (5 - Ks));
                StatTbl[SUSTAIN].and = global.XRTBL[ks].and;
                StatTbl[SUSTAIN].cmp = global.XRTBL[ks].and >> 1;
                StatTbl[SUSTAIN].add = global.XRTBL[ks].add;
            }
            else
            {
                StatTbl[SUSTAIN].and = 4097;
                StatTbl[SUSTAIN].cmp = 2048;
                StatTbl[SUSTAIN].add = 0;
            }
            if (Xr_stat == SUSTAIN)
            {
                Xr_and = StatTbl[Xr_stat].and;
                Xr_cmp = StatTbl[Xr_stat].cmp;
                Xr_add = StatTbl[Xr_stat].add;
            }
        }

        private void CulcRrStep()
        {
            int ks = (Rr << 2) + 2 + (Kc >> (5 - Ks));
            StatTbl[RELEASE].and = global.XRTBL[ks].and;
            StatTbl[RELEASE].cmp = global.XRTBL[ks].and >> 1;
            StatTbl[RELEASE].add = global.XRTBL[ks].add;
            if (Xr_stat == RELEASE)
            {
                Xr_and = StatTbl[Xr_stat].and;
                Xr_cmp = StatTbl[Xr_stat].cmp;
                Xr_add = StatTbl[Xr_stat].add;
            }
        }

        private void CulcPitch()
        {
            Pitch = (Note << 6) + Kf + Dt2;
        }

        private void CulcDt1Pitch()
        {
            Dt1Pitch = global.DT1TBL[(Kc & 0xFC) + (Dt1 & 3)];
            if ((Dt1 & 0x04) != 0)
            {
                Dt1Pitch = -Dt1Pitch;
            }
        }

        public void SetFL(int n)
        {
            n = (n >> 3) & 7;
            if (n == 0)
            {
                Fl = 31;
                Fl_mask = 0;
            }
            else
            {
                Fl = (7 - n + 1 + 1);
                Fl_mask = -1;
            }
        }

        public void SetKC(int n)
        {
            Kc = n & 127;
            int note = Kc & 15;
            Note = ((Kc >> 4) + 1) * 12 + note - (note >> 2);
            ++Kc;
            CulcPitch();
            CulcDt1Pitch();
            LfoPitch = CULC_DELTA_T;
            CulcArStep();
            CulcD1rStep();
            CulcD2rStep();
            CulcRrStep();
        }

        public void SetKF(int n)
        {
            Kf = (n & 255) >> 2;
            CulcPitch();
            LfoPitch = CULC_DELTA_T;
        }

        public void SetDT1MUL(int n)
        {
            Dt1 = (n >> 4) & 7;
            CulcDt1Pitch();
            Mul = (n & 15) << 1;
            if (Mul == 0)
            {
                Mul = 1;
            }
            LfoPitch = CULC_DELTA_T;
        }

        public void SetTL(int n)
        {
            Tl = (128 - (n & 127)) << 3;
            //	LfoLevel = CULC_ALPHA;
            LfoLevelReCalc = true;
        }

        public void SetKSAR(int n)
        {
            Ks = (n & 255) >> 6;
            Ar = n & 31;
            CulcArStep();
            CulcD1rStep();
            CulcD2rStep();
            CulcRrStep();
        }

        public void SetAMED1R(int n)
        {
            D1r = n & 31;
            CulcD1rStep();
            Ame = 0;
            if ((n & 0x80) != 0)
            {
                Ame = -1;
            }
        }

        public void SetDT2D2R(int n)
        {
            Dt2 = global.DT2TBL[(n & 255) >> 6];
            CulcPitch();
            LfoPitch = CULC_DELTA_T;
            D2r = n & 31;
            CulcD2rStep();
        }

        public void SetD1LRR(int n)
        {
            StatTbl[DECAY].limit = global.D1LTBL[(n & 255) >> 4];
            if (Xr_stat == DECAY)
            {
                Xr_limit = StatTbl[DECAY].limit;
            }

            Rr = n & 15;
            CulcRrStep();
        }

        public void KeyON(int csm)
        {
            if (keyon == 0)
            {
                if (Xr_stat >= RELEASE)
                {
                    // KEYON
                    T = 0;

                    if (Xr_el == 0)
                    {
                        Xr_stat = DECAY;
                        Xr_and = StatTbl[Xr_stat].and;
                        Xr_cmp = StatTbl[Xr_stat].cmp;
                        Xr_add = StatTbl[Xr_stat].add;
                        Xr_limit = StatTbl[Xr_stat].limit;
                        if ((Xr_el >> 4) == Xr_limit)
                        {
                            Xr_stat = NEXTSTAT[Xr_stat];
                            Xr_and = StatTbl[Xr_stat].and;
                            Xr_cmp = StatTbl[Xr_stat].cmp;
                            Xr_add = StatTbl[Xr_stat].add;
                            Xr_limit = StatTbl[Xr_stat].limit;
                        }
                    }
                    else
                    {
                        Xr_stat = ATACK;
                        Xr_and = StatTbl[Xr_stat].and;
                        Xr_cmp = StatTbl[Xr_stat].cmp;
                        Xr_add = StatTbl[Xr_stat].add;
                        Xr_limit = StatTbl[Xr_stat].limit;
                    }
                }

                if (csm == 0)
                {
                    keyon = 1;
                    csmkeyon = 0;
                }
                else
                {
                    csmkeyon = 1;
                }
            }
        }

        public void KeyOFF(int csm)
        {
            if (keyon > 0 || csmkeyon > 0)
            {

                if (csm == 0)
                {
                    keyon = 0;
                }
                else
                {
                    csmkeyon = 0;
                }

                if (keyon == 0 && csmkeyon == 0)
                {
                    Xr_stat = RELEASE;
                    Xr_and = StatTbl[Xr_stat].and;
                    Xr_cmp = StatTbl[Xr_stat].cmp;
                    Xr_add = StatTbl[Xr_stat].add;
                    Xr_limit = StatTbl[Xr_stat].limit;
                    if ((Xr_el >> 4) >= 63 || csm > 0)
                    {
                        Xr_el = 1024;
                        Xr_stat = MAXSTAT[Xr_stat];
                        Xr_and = StatTbl[Xr_stat].and;
                        Xr_cmp = StatTbl[Xr_stat].cmp;
                        Xr_add = StatTbl[Xr_stat].add;
                        Xr_limit = StatTbl[Xr_stat].limit;
                    }
                }
            }
        }

        public void Envelope(int env_counter)
        {
            if ((env_counter & Xr_and) == Xr_cmp)
            {

                if (Xr_stat == ATACK)
                {
                    // ATACK
                    Xr_step += Xr_add;
                    Xr_el += ((~Xr_el) * (Xr_step >> 3)) >> 4;
                    //			LfoLevel = CULC_ALPHA;
                    LfoLevelReCalc = true;
                    Xr_step &= 7;

                    if (Xr_el <= 0)
                    {
                        Xr_el = 0;
                        Xr_stat = DECAY;
                        Xr_and = StatTbl[Xr_stat].and;
                        Xr_cmp = StatTbl[Xr_stat].cmp;
                        Xr_add = StatTbl[Xr_stat].add;
                        Xr_limit = StatTbl[Xr_stat].limit;
                        if ((Xr_el >> 4) == Xr_limit)
                        {
                            Xr_stat = NEXTSTAT[Xr_stat];
                            Xr_and = StatTbl[Xr_stat].and;
                            Xr_cmp = StatTbl[Xr_stat].cmp;
                            Xr_add = StatTbl[Xr_stat].add;
                            Xr_limit = StatTbl[Xr_stat].limit;
                        }
                    }
                }
                else
                {
                    // DECAY, SUSTAIN, RELEASE
                    Xr_step += Xr_add;
                    Xr_el += Xr_step >> 3;
                    //			LfoLevel = CULC_ALPHA;
                    LfoLevelReCalc = true;
                    Xr_step &= 7;

                    int e = Xr_el >> 4;
                    if (e == 63)
                    {
                        Xr_el = 1024;
                        Xr_stat = MAXSTAT[Xr_stat];
                        Xr_and = StatTbl[Xr_stat].and;
                        Xr_cmp = StatTbl[Xr_stat].cmp;
                        Xr_add = StatTbl[Xr_stat].add;
                        Xr_limit = StatTbl[Xr_stat].limit;
                    }
                    else if (e == Xr_limit)
                    {
                        Xr_stat = NEXTSTAT[Xr_stat];
                        Xr_and = StatTbl[Xr_stat].and;
                        Xr_cmp = StatTbl[Xr_stat].cmp;
                        Xr_add = StatTbl[Xr_stat].add;
                        Xr_limit = StatTbl[Xr_stat].limit;
                    }
                }
            }
        }

        public void SetNFRQ(int nfrq)
        {
            if (((Nfrq ^ nfrq) & 0x80) != 0)
            {
                //		LfoLevel = CULC_ALPHA;
                LfoLevelReCalc = true;
            }
            Nfrq = nfrq;
            CulcNoiseCycle();
        }

        private void CulcNoiseCycle()
        {
            if ((Nfrq & 0x80) != 0)
            {
                NoiseCycle = (32 - (Nfrq & 31)) << 25;
                if (NoiseCycle < NoiseStep)
                {
                    NoiseCycle = NoiseStep;
                }
                NoiseCounter = NoiseCycle;
            }
            else
            {
                NoiseCycle = 0;
            }
        }

        public void Output0(int lfopitch, int lfolevel)
        {
            if (LfoPitch != lfopitch)
            {
                //		DeltaT = ((STEPTBL[Pitch+lfopitch]+Dt1Pitch)*Mul)>>1;
                DeltaT = ((global.STEPTBL[Pitch + lfopitch] + Dt1Pitch) * Mul) >> (6 + 1);
                LfoPitch = lfopitch;
            }
            T += DeltaT;
            short Sin = (global.SINTBL[(((T + Out2Fb) >> global.PRECISION_BITS)) & (global.SIZESINTBL - 1)]);

            int lfolevelame = lfolevel & Ame;
            if ((LfoLevel != lfolevelame || LfoLevelReCalc) && IS_ZERO_CLOSS(SinBf, Sin) != 0)
            {
                Alpha = (int)(global.ALPHATBL[global.ALPHAZERO + Tl - Xr_el - lfolevelame]);
                LfoLevel = lfolevelame;
                LfoLevelReCalc = false;
            }
            int o = (Alpha)
                * (int)Sin;
            SinBf = Sin;

            //	int o2 = (o+Inp_last) >> 1;
            //	Out2Fb = (o+o) >> Fl;
            Out2Fb = ((o + Inp_last) & Fl_mask) >> Fl;
            Inp_last = o;

            out1[0] = o;
            out2[0] = o;  // alg=5用
            out3[0] = o; // alg=5用
                       //	*out = o2;
                       //	*out2 = o2;	// alg=5用
                       //	*out3 = o2; // alg=5用
        }

        public void Output(int lfopitch, int lfolevel)
        {
            if (LfoPitch != lfopitch)
            {
                //		DeltaT = ((STEPTBL[Pitch+lfopitch]+Dt1Pitch)*Mul)>>1;
                DeltaT = ((global.STEPTBL[Pitch + lfopitch] + Dt1Pitch) * Mul) >> (6 + 1);
                LfoPitch = lfopitch;
            }
            T += DeltaT;
            short Sin = (global.SINTBL[(((T + inp[0]) >> global.PRECISION_BITS)) & (global.SIZESINTBL - 1)]);

            int lfolevelame = lfolevel & Ame;
            if ((LfoLevel != lfolevelame || LfoLevelReCalc) && IS_ZERO_CLOSS(SinBf, Sin) != 0)
            {
                Alpha = (int)(global.ALPHATBL[global.ALPHAZERO + Tl - Xr_el - lfolevelame]);
                LfoLevel = lfolevelame;
                LfoLevelReCalc = false;
            }
            int o = (Alpha)
                * (int)Sin;
            SinBf = Sin;

            out1[0] += o;
        }

        public void Output32(int lfopitch, int lfolevel)
        {
            if (LfoPitch != lfopitch)
            {
                //		DeltaT = ((STEPTBL[Pitch+lfopitch]+Dt1Pitch)*Mul)>>1;
                DeltaT = ((global.STEPTBL[Pitch + lfopitch] + Dt1Pitch) * Mul) >> (6 + 1);
                LfoPitch = lfopitch;
            }
            T += DeltaT;

            int o;
            short Sin = (global.SINTBL[(((T + inp[0]) >> global.PRECISION_BITS)) & (global.SIZESINTBL - 1)]);
            if (NoiseCycle == 0)
            {
                int lfolevelame = lfolevel & Ame;
                if ((LfoLevel != lfolevelame || LfoLevelReCalc) && IS_ZERO_CLOSS(SinBf, Sin) != 0)
                {
                    Alpha = (int)(global.ALPHATBL[global.ALPHAZERO + Tl - Xr_el - lfolevelame]);
                    LfoLevel = lfolevelame;
                    LfoLevelReCalc = false;
                }
                o = (Alpha)
                    * (int)Sin;
                SinBf = Sin;
            }
            else
            {
                NoiseCounter -= NoiseStep;
                if (NoiseCounter <= 0)
                {
                    NoiseValue = (int)((global.irnd() >> 30) & 2) - 1;
                    NoiseCounter += NoiseCycle;
                }

                int lfolevelame = lfolevel & Ame;
                if (LfoLevel != lfolevelame || LfoLevelReCalc)
                {
                    Alpha = (int)(global.NOISEALPHATBL[global.ALPHAZERO + Tl - Xr_el - lfolevelame]);
                    LfoLevel = lfolevelame;
                    LfoLevelReCalc = false;
                }
                o = (Alpha)
                    * NoiseValue * global.MAXSINVAL;
            }

            out1[0] += o;
        }
    }
}
