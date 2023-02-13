using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver
{
    public delegate void dlgRelease(object ctx);
    public delegate void dlgReset(object ctx, UInt32 clock, UInt32 freq);
    public delegate void dlgSynth(object ctx, Int32[] p);
    public delegate void dlgVolume(object ctx, Int32 v);
    public delegate void dlgWrite(object ctx, UInt32 a, UInt32 v);
    public delegate UInt32 dlgRead(object ctx, UInt32 a);
    public delegate void dlgSetinst(object ctx, UInt32 n, byte[] p, UInt32 l);

    public class KMIF_SOUND_DEVICE
    {
        public object ctx;// void* ctx;
        public dlgRelease release;
        public dlgReset reset;
        public dlgSynth synth;
        public dlgVolume volume;
        public dlgWrite write;
        public dlgRead read;
        public dlgSetinst setinst;
        //#if 0
        //void (*setrate)(void *ctx, Uint32 clock, Uint32 freq);
        //void (*getinfo)(void *ctx, KMCH_INFO *cip, );
        //void (*volume2)(void *ctx, Uint8 *volp, Uint32 numch);
        ///* 0x00(mute),0x70(x1/2),0x80(x1),0x90(x2) */
        //#endif
    }

    public class s_deltat : KMIF_SOUND_DEVICE
    {
        //# include "kmsnddev.h"


        public byte[] chmask = new byte[0x80]{
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
        };
        //チャンネルマスク用
        public enum enmChMask : Int32
        {//順番を変えたら恐ろしいことになる
            DEV_2A03_SQ1 = 0,
            DEV_2A03_SQ2,
            DEV_2A03_TR,
            DEV_2A03_NOISE,
            DEV_2A03_DPCM,

            DEV_FDS_CH1,

            DEV_MMC5_SQ1,
            DEV_MMC5_SQ2,
            DEV_MMC5_DA,

            DEV_VRC6_SQ1,
            DEV_VRC6_SQ2,
            DEV_VRC6_SAW,

            DEV_N106_CH1,
            DEV_N106_CH2,
            DEV_N106_CH3,
            DEV_N106_CH4,
            DEV_N106_CH5,
            DEV_N106_CH6,
            DEV_N106_CH7,
            DEV_N106_CH8,

            DEV_DMG_SQ1,
            DEV_DMG_SQ2,
            DEV_DMG_WM,
            DEV_DMG_NOISE,

            DEV_HUC6230_CH1,
            DEV_HUC6230_CH2,
            DEV_HUC6230_CH3,
            DEV_HUC6230_CH4,
            DEV_HUC6230_CH5,
            DEV_HUC6230_CH6,

            DEV_AY8910_CH1,
            DEV_AY8910_CH2,
            DEV_AY8910_CH3,

            DEV_SN76489_SQ1,
            DEV_SN76489_SQ2,
            DEV_SN76489_SQ3,
            DEV_SN76489_NOISE,

            DEV_SCC_CH1,
            DEV_SCC_CH2,
            DEV_SCC_CH3,
            DEV_SCC_CH4,
            DEV_SCC_CH5,

            DEV_YM2413_CH1,
            DEV_YM2413_CH2,
            DEV_YM2413_CH3,
            DEV_YM2413_CH4,
            DEV_YM2413_CH5,
            DEV_YM2413_CH6,
            DEV_YM2413_CH7,
            DEV_YM2413_CH8,
            DEV_YM2413_CH9,
            DEV_YM2413_BD,
            DEV_YM2413_HH,
            DEV_YM2413_SD,
            DEV_YM2413_TOM,
            DEV_YM2413_TCY,

            DEV_ADPCM_CH1,

            DEV_MSX_DA,

            DEV_MAX,
        };

        //# include "divfix.h"
        private UInt32 DivFix(UInt32 p1, UInt32 p2, UInt32 fix)
        {
            UInt32 ret;
            ret = p1 / p2;
            p1 = p1 % p2;/* p1 = p1 - p2 * ret; */
            while (fix-- != 0)
            {
                p1 += p1;
                ret += ret;
                if (p1 >= p2)
                {
                    p1 -= p2;
                    ret++;
                }
            }
            return ret;
        }

        //# include "s_logtbl.h"
        public const Int32 LOG_BITS = 12;
        public const Int32 LIN_BITS = 7;
        public const Int32 LOG_LIN_BITS = 30;

        public class KMIF_LOGTABLE
        {
            public object ctx;// void* ctx;
            public delegate void dlgRelease(object ctx);
            public dlgRelease release;
            public UInt32[] lineartbl = new UInt32[(1 << LIN_BITS) + 1];
            public UInt32[] logtbl = new UInt32[1 << LOG_BITS];
        }

        public object log_tables_mutex = new object();
        public UInt32 log_tables_refcount = 0;
        public KMIF_LOGTABLE log_tables = null;

        public void LogTableRelease(object ctx)
        {
            lock (log_tables_mutex)
            {
                //while (log_tables_mutex != 1)
                //{
                    //XSLEEP(0);
                //}
                log_tables_refcount--;
                if (log_tables_refcount==0)
                {
                    //XFREE(ctx);
                    log_tables = null;
                }
            }
        }

        private void LogTableCalc(KMIF_LOGTABLE kmif_lt)
        {
            UInt32 i;
            double a;
            for (i = 0; i < (1 << LOG_BITS); i++)
            {
                a = (1 << LOG_LIN_BITS) / Math.Pow(2, i / (double)(1 << LOG_BITS));
                kmif_lt.logtbl[i] = (UInt32)a;
            }
            kmif_lt.lineartbl[0] = LOG_LIN_BITS << LOG_BITS;
            for (i = 1; i < (1 << LIN_BITS) + 1; i++)
            {
                UInt32 ua;
                a = i << (LOG_LIN_BITS - LIN_BITS);
                ua = (UInt32)((LOG_LIN_BITS - (Math.Log(a) / Math.Log(2))) * (1 << LOG_BITS));
                kmif_lt.lineartbl[i] = ua << 1;
            }
        }

        public KMIF_LOGTABLE LogTableAddRef()
        {
            lock (log_tables_mutex)
            {
                //while (log_tables_mutex != 1)
                //{
                //    XSLEEP(0);
                //}
                if (log_tables_refcount==0)
                {
                    //log_tables = XMALLOC(sizeof(KMIF_LOGTABLE));
                    log_tables = new KMIF_LOGTABLE();
                    if (log_tables!=null)
                    {
                        //XMEMSET(log_tables, 0, sizeof(KMIF_LOGTABLE));
                        log_tables.ctx = log_tables;
                        log_tables.release = LogTableRelease;
                        LogTableCalc(log_tables);
                    }
                }
                if (log_tables!=null) log_tables_refcount++;
            }
            return log_tables;
        }

        public Int32 LogToLin(KMIF_LOGTABLE kmif_lt, Int32 l, UInt32 sft)
        {
            Int32 ret;
            UInt32 ofs;
            ofs = (UInt32)(l + (sft << (LOG_BITS + 1)));
            sft = ofs >> (LOG_BITS + 1);
            if (sft >= LOG_LIN_BITS) return 0;
            ofs = (ofs >> 1) & ((1 << LOG_BITS) - 1);
            ret = (Int32)(kmif_lt.logtbl[ofs] >> (Int32)sft);
            return (l & 1) != 0 ? -ret : ret;
        }

        //# include "s_deltat.h"

        private const Int32 CPS_SHIFT = 16;
        private const Int32 PHASE_SHIFT = 16; /* 16(fix) */

        public class YMDELTATPCMSOUND_ {

            public KMIF_SOUND_DEVICE kmif;
            public KMIF_LOGTABLE logtbl;
            public class YMDELTATPCMSOUND_COMMON_TAG
            {
                public Int32 mastervolume;
                public Int32 step;
                public Int32 output;
                public UInt32 cnt;
                public UInt32 cps;
                public UInt32 phase;
                public UInt32 deltan;
                public Int32 scale;
                public UInt32 mem;
                public UInt32 play;
                public UInt32 start;
                public UInt32 stop;
                public UInt32 level32;
                public byte key;
                public byte level;
                public byte granuality;
                public byte pad4_3;
                public byte[] regs = new byte[0x10];
            }
            public YMDELTATPCMSOUND_COMMON_TAG common=new YMDELTATPCMSOUND_COMMON_TAG();
            public byte[] romrambuf;
            public UInt32 romrammask;
            public byte[] rambuf;
            public UInt32 rammask;
            public byte[] rombuf;
            public UInt32 rommask;
            public byte ymdeltatpcm_type;
            public byte memshift;
            public UInt32 ram_size;
        }

        public YMDELTATPCMSOUND_ YMDELTATPCMSOUND;

        public sbyte[] table_step = new sbyte[16]
        {
    1,  3,  5,  7,  9,  11, 13, 15,
    -1, -1, -1, -1, 2,  4,  6,  8
        };

        public byte[] table_scale = new byte[16]
        {
     57,  57,  57,  57,  77, 102, 128, 153,
     57,  57,  57,  57,  77, 102, 128, 153
        };

        public Int32[] scaletable = new Int32[49 * 16]{
    2,    6,   10,   14,   18,   22,   26,   30,   -2,   -6,  -10,  -14,  -18,  -22,  -26,  -30,
    2,    6,   10,   14,   19,   23,   27,   31,   -2,   -6,  -10,  -14,  -19,  -23,  -27,  -31,
    2,    6,   11,   15,   21,   25,   30,   34,   -2,   -6,  -11,  -15,  -21,  -25,  -30,  -34,
    2,    7,   12,   17,   23,   28,   33,   38,   -2,   -7,  -12,  -17,  -23,  -28,  -33,  -38,
    2,    7,   13,   18,   25,   30,   36,   41,   -2,   -7,  -13,  -18,  -25,  -30,  -36,  -41,
    3,    9,   15,   21,   28,   34,   40,   46,   -3,   -9,  -15,  -21,  -28,  -34,  -40,  -46,
    3,   10,   17,   24,   31,   38,   45,   52,   -3,  -10,  -17,  -24,  -31,  -38,  -45,  -52,
    3,   10,   18,   25,   34,   41,   49,   56,   -3,  -10,  -18,  -25,  -34,  -41,  -49,  -56,
    4,   12,   21,   29,   38,   46,   55,   63,   -4,  -12,  -21,  -29,  -38,  -46,  -55,  -63,
    4,   13,   22,   31,   41,   50,   59,   68,   -4,  -13,  -22,  -31,  -41,  -50,  -59,  -68,
    5,   15,   25,   35,   46,   56,   66,   76,   -5,  -15,  -25,  -35,  -46,  -56,  -66,  -76,
    5,   16,   27,   38,   50,   61,   72,   83,   -5,  -16,  -27,  -38,  -50,  -61,  -72,  -83,
    6,   18,   31,   43,   56,   68,   81,   93,   -6,  -18,  -31,  -43,  -56,  -68,  -81,  -93,
    6,   19,   33,   46,   61,   74,   88,  101,   -6,  -19,  -33,  -46,  -61,  -74,  -88, -101,
    7,   22,   37,   52,   67,   82,   97,  112,   -7,  -22,  -37,  -52,  -67,  -82,  -97, -112,
    8,   24,   41,   57,   74,   90,  107,  123,   -8,  -24,  -41,  -57,  -74,  -90, -107, -123,
    9,   27,   45,   63,   82,  100,  118,  136,   -9,  -27,  -45,  -63,  -82, -100, -118, -136,
   10,   30,   50,   70,   90,  110,  130,  150,  -10,  -30,  -50,  -70,  -90, -110, -130, -150,
   11,   33,   55,   77,   99,  121,  143,  165,  -11,  -33,  -55,  -77,  -99, -121, -143, -165,
   12,   36,   60,   84,  109,  133,  157,  181,  -12,  -36,  -60,  -84, -109, -133, -157, -181,
   13,   39,   66,   92,  120,  146,  173,  199,  -13,  -39,  -66,  -92, -120, -146, -173, -199,
   14,   43,   73,  102,  132,  161,  191,  220,  -14,  -43,  -73, -102, -132, -161, -191, -220,
   16,   48,   81,  113,  146,  178,  211,  243,  -16,  -48,  -81, -113, -146, -178, -211, -243,
   17,   52,   88,  123,  160,  195,  231,  266,  -17,  -52,  -88, -123, -160, -195, -231, -266,
   19,   58,   97,  136,  176,  215,  254,  293,  -19,  -58,  -97, -136, -176, -215, -254, -293,
   21,   64,  107,  150,  194,  237,  280,  323,  -21,  -64, -107, -150, -194, -237, -280, -323,
   23,   70,  118,  165,  213,  260,  308,  355,  -23,  -70, -118, -165, -213, -260, -308, -355,
   26,   78,  130,  182,  235,  287,  339,  391,  -26,  -78, -130, -182, -235, -287, -339, -391,
   28,   85,  143,  200,  258,  315,  373,  430,  -28,  -85, -143, -200, -258, -315, -373, -430,
   31,   94,  157,  220,  284,  347,  410,  473,  -31,  -94, -157, -220, -284, -347, -410, -473,
   34,  103,  173,  242,  313,  382,  452,  521,  -34, -103, -173, -242, -313, -382, -452, -521,
   38,  114,  191,  267,  345,  421,  498,  574,  -38, -114, -191, -267, -345, -421, -498, -574,
   42,  126,  210,  294,  379,  463,  547,  631,  -42, -126, -210, -294, -379, -463, -547, -631,
   46,  138,  231,  323,  417,  509,  602,  694,  -46, -138, -231, -323, -417, -509, -602, -694,
   51,  153,  255,  357,  459,  561,  663,  765,  -51, -153, -255, -357, -459, -561, -663, -765,
   56,  168,  280,  392,  505,  617,  729,  841,  -56, -168, -280, -392, -505, -617, -729, -841,
   61,  184,  308,  431,  555,  678,  802,  925,  -61, -184, -308, -431, -555, -678, -802, -925,
   68,  204,  340,  476,  612,  748,  884, 1020,  -68, -204, -340, -476, -612, -748, -884,-1020,
   74,  223,  373,  522,  672,  821,  971, 1120,  -74, -223, -373, -522, -672, -821, -971,-1120,
   82,  246,  411,  575,  740,  904, 1069, 1233,  -82, -246, -411, -575, -740, -904,-1069,-1233,
   90,  271,  452,  633,  814,  995, 1176, 1357,  -90, -271, -452, -633, -814, -995,-1176,-1357,
   99,  298,  497,  696,  895, 1094, 1293, 1492,  -99, -298, -497, -696, -895,-1094,-1293,-1492,
  109,  328,  547,  766,  985, 1204, 1423, 1642, -109, -328, -547, -766, -985,-1204,-1423,-1642,
  120,  360,  601,  841, 1083, 1323, 1564, 1804, -120, -360, -601, -841,-1083,-1323,-1564,-1804,
  132,  397,  662,  927, 1192, 1457, 1722, 1987, -132, -397, -662, -927,-1192,-1457,-1722,-1987,
  145,  436,  728, 1019, 1311, 1602, 1894, 2185, -145, -436, -728,-1019,-1311,-1602,-1894,-2185,
  160,  480,  801, 1121, 1442, 1762, 2083, 2403, -160, -480, -801,-1121,-1442,-1762,-2083,-2403,
  176,  528,  881, 1233, 1587, 1939, 2292, 2644, -176, -528, -881,-1233,-1587,-1939,-2292,-2644,
  194,  582,  970, 1358, 1746, 2134, 2522, 2910, -194, -582, -970,-1358,-1746,-2134,-2522,-2910
    };


        private void writeram(YMDELTATPCMSOUND_ sndp, UInt32 v)
        {
            sndp.rambuf[(sndp.common.mem >> 1) & sndp.rammask] = (byte)v;
            sndp.common.mem += 1 << 1;
        }

        private UInt32 readram(YMDELTATPCMSOUND_ sndp)
        {
            UInt32 v;
            v = sndp.romrambuf[(sndp.common.play >> 1) & sndp.romrammask];
            if ((sndp.common.play & 1) != 0)
                v &= 0x0f;
            else
                v >>= 4;
            sndp.common.play += 1;
            if (sndp.common.play >= sndp.common.stop)
            {
                if ((sndp.common.regs[0] & 0x10) != 0)
                {
                    sndp.common.play = sndp.common.start;
                    sndp.common.step = 0;
                    if (sndp.ymdeltatpcm_type == 3)//MSM5205)
                    {
                        sndp.common.scale = 0;
                    }
                    else
                    {
                        sndp.common.scale = 127;
                    }
                }
                else
                {
                    sndp.common.key = 0;
                }
            }
            return v;
        }

        private void DelrtatStep(YMDELTATPCMSOUND_ sndp, UInt32 data)
        {
            if (sndp.ymdeltatpcm_type == 3)//MSM5205)
            {
                sndp.common.scale = sndp.common.scale + scaletable[(sndp.common.step << 4) + (data & 0xf)];
                if (sndp.common.scale > 2047) sndp.common.scale = 2047;
                if (sndp.common.scale < -2048) sndp.common.scale = -2048;

                sndp.common.step += table_step[(data & 7) + 8];
                if (sndp.common.step > 48) sndp.common.step = 48;
                if (sndp.common.step < 0) sndp.common.step = 0;
            }
            else
            {
                if ((data & 8) != 0)
                    sndp.common.step -= (table_step[data & 7] * sndp.common.scale) >> 3;
                else
                    sndp.common.step += (table_step[data & 7] * sndp.common.scale) >> 3;
                if (sndp.common.step > ((1 << 15) - 1)) sndp.common.step = ((1 << 15) - 1);
                if (sndp.common.step < -(1 << 15)) sndp.common.step = -(1 << 15);
                sndp.common.scale = (sndp.common.scale * table_scale[data]) >> 6;
                if (sndp.common.scale > 24576) sndp.common.scale = 24576;
                if (sndp.common.scale < 127) sndp.common.scale = 127;
            }
        }

        //#if (((-1) >> 1) == -1)
        public Int32 SSR(Int32 x, Int32 y) {
            return (((Int32)x) >> (y));
        }
        //#else
        public Int32 SSR1(Int32 x, Int32 y)
        {
            return (((x) >= 0) ? ((x) >> (y)) : (-((-(x) - 1) >> (y)) - 1));
        }
        //#endif


        public void sndsynth(object sndp, Int32[] p)
        {
            YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
            if (s.common.key != 0)
            {
                UInt32 step;
                s.common.cnt += s.common.cps;
                step = s.common.cnt >> CPS_SHIFT;
                s.common.cnt &= (1 << CPS_SHIFT) - 1;
                s.common.phase += step * s.common.deltan;
                step = s.common.phase >> PHASE_SHIFT;
                s.common.phase &= (1 << PHASE_SHIFT) - 1;
                if (step != 0)
                {
                    do
                    {
                        DelrtatStep(s, readram(s));
                    } while (--step != 0);
                    if (s.ymdeltatpcm_type == 3)//MSM5205)
                    {
                        s.common.output = (Int32)(s.common.scale * s.common.level32);
                    }
                    else
                    {
                        s.common.output = (Int32)(s.common.step * s.common.level32);
                    }
                    s.common.output = SSR(s.common.output, 8 + 2);
                }
                if (chmask[(int)enmChMask.DEV_ADPCM_CH1] != 0)
                {
                    p[0] += s.common.output;
                    p[1] += s.common.output;
                }
            }
        }



        public void sndwrite(object sndp, UInt32 a, UInt32 v)
        {
            YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
            s.common.regs[a] = (byte)v;
            switch (a)
            {
                /* START,REC,MEMDATA,REPEAT,SPOFF,--,--,RESET */
                case 0x00:  /* Control Register 1 */
                    if ((v & 0x80) != 0 && s.common.key == 0)
                    {
                        s.common.key = 1;
                        s.common.play = s.common.start;
                        s.common.step = 0;
                        if (s.ymdeltatpcm_type == 3)//MSM5205)
                        {
                            s.common.scale = 0;
                        }
                        else
                        {
                            s.common.scale = 127;
                        }
                    }
                    if ((v & 1) != 0) s.common.key = 0;
                    break;
                /* L,R,-,-,SAMPLE,DA/AD,RAMTYPE,ROM */
                case 0x01:  /* Control Register 2 */    //MSX-AUDIOにADPCM用ROMは無いはずなので無効化
                                                        //			sndp.romrambuf  = (sndp.common.regs[1] & 1) ? sndp.rombuf  : sndp.rambuf;
                                                        //			sndp.romrammask = (sndp.common.regs[1] & 1) ? sndp.rommask : sndp.rammask;
                    break;
                case 0x02:  /* Start Address L */
                case 0x03:  /* Start Address H */
                    s.common.granuality = (byte)((v & 2) != 0 ? 1 : 4);
                    s.common.start = (UInt32)(((s.common.regs[3] << 8) + s.common.regs[2]) << (s.memshift + 1));
                    s.common.mem = s.common.start;
                    break;
                case 0x04:  /* Stop Address L */
                case 0x05:  /* Stop Address H */
                    s.common.stop = (UInt32)(((s.common.regs[5] << 8) + s.common.regs[4]) << (s.memshift + 1));
                    break;
                case 0x06:  /* Prescale L */
                case 0x07:  /* Prescale H */
                    break;
                case 0x08:  /* Data */
                    if ((s.common.regs[0] & 0x60) == 0x60) writeram(s, v);
                    break;
                case 0x09:  /* Delta-N L */
                case 0x0a:  /* Delta-N H */
                    s.common.deltan = (UInt32)((s.common.regs[0xa] << 8) + s.common.regs[0x9]);
                    if (s.common.deltan < 0x100) s.common.deltan = 0x100;
                    break;
                case 0x0b:  /* Level Control */
                    s.common.level = (byte)v;
                    s.common.level32 = (UInt32)(((Int32)(s.common.level * LogToLin(s.logtbl, s.common.mastervolume, LOG_LIN_BITS - 15))) >> 7);
                    if (s.ymdeltatpcm_type == 3)//MSM5205)
                    {
                        s.common.output = (Int32)(s.common.scale * s.common.level32);
                    }
                    else
                    {
                        s.common.output = (Int32)(s.common.step * s.common.level32);
                    }
                    s.common.output = SSR(s.common.output, 8 + 2);
                    break;
            }
        }

        public UInt32 sndread(object sndp, UInt32 a)
        {
            YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
            return 0;
        }

        public void sndreset(object sndp, UInt32 clock, UInt32 freq)
        {
            YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
            //XMEMSET(&sndp.common, 0, sizeof(sndp.common));
            s.common = new YMDELTATPCMSOUND_.YMDELTATPCMSOUND_COMMON_TAG();
            s.common.cps = DivFix(clock, 72 * freq, CPS_SHIFT);
            s.romrambuf = (s.common.regs[1] & 1)!=0 ? s.rombuf : s.rambuf;
            s.romrammask = (s.common.regs[1] & 1)!=0 ? s.rommask : s.rammask;
            s.common.granuality = 4;
        }

        public void sndvolume(object sndp, Int32 volume)
        {
            YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
            volume = (volume << (LOG_BITS - 8)) << 1;
            s.common.mastervolume = volume;
            s.common.level32 = (UInt32)(((Int32)(s.common.level * LogToLin(s.logtbl, s.common.mastervolume, LOG_LIN_BITS - 15))) >> 7);
            s.common.output = (Int32)(s.common.step * s.common.level32);
            s.common.output = SSR(s.common.output, 8 + 2);
        }

        public void sndrelease(object sndp)
        {
            if (sndp != null)
            {
                YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
                if (s.logtbl!=null) s.logtbl.release(s.logtbl.ctx);
                sndp = null;
                //XFREE(sndp);
            }
        }

        public new void setinst(object sndp, UInt32 n, byte[] p, UInt32 l)
        {
            YMDELTATPCMSOUND_ s = (YMDELTATPCMSOUND_)sndp;
            if (n != 0) return;
            if (p != null)
            {
                s.rombuf = p;
                s.rommask = l - 1;
                s.romrambuf = (s.common.regs[1] & 1) != 0 ? s.rombuf : s.rambuf;
                s.romrammask = (s.common.regs[1] & 1) != 0 ? s.rommask : s.rammask;
            }
            else
            {
                s.rombuf = null;
                s.rommask = 0;
            }

        }
        //ここからレジスタビュアー設定
        private YMDELTATPCMSOUND_ sndpr;
        private delegate UInt32 ioview_ioread_DEV_ADPCM(UInt32 a);
        private ioview_ioread_DEV_ADPCM ioview_ioread_DEV_ADPCM_;
        private delegate UInt32 ioview_ioread_DEV_ADPCM2(UInt32 a);
        private ioview_ioread_DEV_ADPCM2 ioview_ioread_DEV_ADPCM2_;

        private UInt32 ioview_ioread_bf(UInt32 a)
        {
            if (a <= 0xb) return sndpr.common.regs[a]; else return 0x100;
        }

        private UInt32 ioview_ioread_bf2(UInt32 a)
        {
            if (a < sndpr.ram_size) return sndpr.rambuf[a]; else return 0x100;
        }
        //ここまでレジスタビュアー設定

        private KMIF_SOUND_DEVICE YMDELTATPCMSoundAlloc(UInt32 ymdeltatpcm_type, byte[] pcmbuf)
        {
            UInt32 ram_size;
            YMDELTATPCMSOUND_ sndp;
            switch (ymdeltatpcm_type)
            {
                case 0://                    YMDELTATPCM_TYPE_Y8950:
                    ram_size = 32 * 1024;
                    break;
                case 1://                    YMDELTATPCM_TYPE_YM2608:
                    ram_size = 256 * 1024;
                    break;
                case 3://                    MSM5205:
                    ram_size = 256 * 256;
                    break;
                default:
                    ram_size = 0;
                    break;
            }
            sndp = new YMDELTATPCMSOUND_();// XMALLOC(sizeof(YMDELTATPCMSOUND) + ram_size);
            sndp.rambuf = new byte[ram_size];
            if (sndp == null) return null;
            sndp.ram_size = ram_size;
            sndp.ymdeltatpcm_type = (byte)ymdeltatpcm_type;
            switch (ymdeltatpcm_type)
            {
                case 0://                    YMDELTATPCM_TYPE_Y8950:
                    sndp.memshift = 2;
                    break;
                case 1://                    YMDELTATPCM_TYPE_YM2608:
                    /* OPNA */
                    sndp.memshift = 6;
                    break;
                case 2://                    YMDELTATPCM_TYPE_YM2610:
                    sndp.memshift = 9;
                    break;
                case 3://                    MSM5205:
                    sndp.memshift = 0;
                    break;
            }
            sndp.kmif.ctx = sndp;
            sndp.kmif.release = sndrelease;
            sndp.kmif.synth = sndsynth;
            sndp.kmif.volume = sndvolume;
            sndp.kmif.reset = sndreset;
            sndp.kmif.write = sndwrite;
            sndp.kmif.read = sndread;
            sndp.kmif.setinst = setinst;
            /* RAM */
            if (pcmbuf != null)
                sndp.rambuf = pcmbuf;
            else
                sndp.rambuf = ram_size != 0 ? sndp.rambuf : null;// (Uint8*)(sndp + 1) : 0;
            sndp.rammask = ram_size != 0 ? (ram_size - 1) : 0;
            /* ROM */
            sndp.rombuf = null;
            sndp.rommask = 0;
            sndp.logtbl = LogTableAddRef();
            if (sndp.logtbl==null)
            {
                sndrelease(sndp);
                return null;
            }
            //ここからレジスタビュアー設定
            sndpr = sndp;
            if (ioview_ioread_DEV_ADPCM_ == null) ioview_ioread_DEV_ADPCM_ = ioview_ioread_bf;
            if (ioview_ioread_DEV_ADPCM2_ == null) ioview_ioread_DEV_ADPCM2_ = ioview_ioread_bf2;
            //ここまでレジスタビュアー設定
            return sndp.kmif;
        }

    }
}
