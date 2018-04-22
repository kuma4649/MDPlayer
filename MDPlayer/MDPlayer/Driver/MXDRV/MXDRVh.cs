using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MXDRV
{
    public partial class MXDRV
    {

        //// MXDRV.DLL header
        //// Copyright (C) 2000 GORRY.

        //# ifndef __MXDRV_H__
        //#define __MXDRV_H__

        //# include "depend.h"
        
        //
        //UWORD UInt16
        //ULONG UInt32
        //UBYTE byte
        //

        public struct MXWORK_CH
        {
            public const int S0000= 0; // volatile public byte[] S0000; // Ptr
            public const int S0004_b = 4;// public byte S0004_b;  // PCM bank
            public const int S0004 = 5; //byte[] S0004; // voice ptr
            public const int S0008 = 9; //public UInt32 S0008;    // bend delta
            public const int S000c = 13; //public UInt32 S000c;    // bend offset
            public const int S0010 = 17; //public UInt16 S0010;    // D
            public const int S0012 = 19; //public UInt16 S0012;    // note+D
            public const int S0014 = 21; //public UInt16 S0014;    // note+D+bend+Pitch LFO offset
            public const int S0016 = 23; //public byte S0016;    // flags b3=keyon/off
            public const int S0017 = 24; //public byte S0017;    // flags b0:volChg
            public const int S0018 = 25; //public byte S0018;    // ch
            public const int S0019 = 26; //public byte S0019;    // carrier slot
            public const int S001a = 27; //public byte S001a;    // len
            public const int S001b = 28; //public byte S001b;    // gate
            public const int S001c = 29; //public byte S001c;    // p + fb + con
            public const int S001d = 30; //public byte S001d;    // keyon slot
            public const int S001e = 31; //public byte S001e;    // Q
            public const int S001f = 32; //public byte S001f;    // Keyon delay
            public const int S0020 = 33; //public byte S0020;    // Keyon delay counter
            public const int S0021 = 34; //public byte S0021;    // PMS/AMS
            public const int S0022 = 35; //public byte S0022;    // v
            public const int S0023 = 36; //public byte S0023;    // v last
            public const int S0024 = 37; //public byte S0024;    // LFO delay
            public const int S0025 = 38; //public byte S0025;    // LFO delay counter
            public const int S0026 = 39; //volatile public byte[] S0026; // Pitch LFO Type
            public const int S002a = 43; //public UInt32 S002a;    // Pitch LFO offset start
            public const int S002e = 47; //public UInt32 S002e;    // Pitch LFO delta start
            public const int S0032 = 51; //public UInt32 S0032;    // Pitch LFO delta
            public const int S0036 = 55; //public UInt32 S0036;    // Pitch LFO offset
            public const int S003a = 59; //public UInt16 S003a;    // Pitch LFO length (cooked)
            public const int S003c = 61; //public UInt16 S003c;    // Pitch LFO length
            public const int S003e = 63; //public UInt16 S003e;    // Pitch LFO length counter
            public const int S0040 = 65; //volatile public byte[] S0040; // Volume LFO Type
            public const int S0044 = 69; //public UInt16 S0044;    // Volume LFO delta start
            public const int S0046 = 71; //public UInt16 S0046;    // Volume LFO delta (cooked)
            public const int S0048 = 73; //public UInt16 S0048;    // Volume LFO delta
            public const int S004a = 75; //public UInt16 S004a;    // Volume LFO offset
            public const int S004c = 77; //public UInt16 S004c;    // Volume LFO length
            public const int S004e = 79; //public UInt16 S004e;    // Volume LFO length counter
            public const int Length = 81;
        }


        public struct MXWORK_GLOBAL
        {
            public const int L001ba6 = 0; //public UInt16 L001ba6;
            public const int L001ba8 = 2; //public UInt32 L001ba8;
            public const int L001bac = 6; //volatile public byte[] L001bac;
            public const int L001bb4 = 10; //public byte[] L001bb4 = new byte[16];
            public const int L001df4 = 26; //byte L001df4;
            public const int L001df6 = 27; //new byte[16];
            public const int L001e06 = 43; //public UInt16 L001e06;  // Channel Mask (true)
            public const int L001e08 = 45; //public byte L001e08;
            public const int L001e09 = 46; //public byte L001e09;
            public const int L001e0a = 47; //public byte L001e0a;
            public const int L001e0b = 48; //public byte L001e0b;
            public const int L001e0c = 49; //byte L001e0c;  // @t
            public const int L001e0d = 50; //public byte L001e0d;
            public const int L001e0e = 51; //public Ref<byte> L001e0e;
            public const int L001e10 = 52; //public byte L001e10;
            public const int L001e12 = 53; //public byte L001e12;  // Paused
            public const int L001e13 = 54; //public byte L001e13;  // End
            public const int L001e14 = 55; //byte L001e14;  // Fadeout Offset
            public const int L001e15 = 56; //public byte L001e15;
            public const int L001e17 = 57; //public byte L001e17;  // Fadeout Enable
            public const int L001e18 = 58; //public byte L001e18;
            public const int L001e19 = 59; //public byte L001e19;
            public const int L001e1a = 60; //public UInt16 L001e1a;  // Channel Enable
            public const int L001e1c = 62; //public UInt16 L001e1c;  // Channel Mask
            public const int L001e1e = 64; //UInt16[2];   // Fadeout Speed
            public const int L001e22 = 72; //public UInt16 L001e22;
            public const int L001e24 = 74; //volatile public byte[] L001e24;
            public const int L001e28 = 78; //volatile public byte[] L001e28;
            public const int L001e2c = 82; //volatile public byte[] L001e2c;
            public const int L001e30 = 86; //volatile public byte[] L001e30;
            public const int L001e34 = 90; //volatile public byte[] L001e34;
            public const int L001e38 = 94; //volatile public byte[] L001e38;
            public const int L00220c = 98; //public UInt32 L00220c;
            public const int L002218 = 102; //volatile public byte[] L002218;
            public const int L00221c = 106; //volatile public byte[] L00221c;
            public const int L002220 = 110; //public UInt32 L002220; // L_MDXSIZE
            public const int L002224 = 114; //public UInt32 L002224; // L_PDXSIZE
            public const int L002228 = 118; //volatile public byte[] L002228;   // voice data
            public const int L00222c = 122; //volatile public byte[] L00222c;
            public const int L002230 = 126; //public byte L002230;
            public const int L002231 = 127; //public byte L002231;
            public const int L002232 = 128; //public byte L002232;
            public const int L002233 = 129; //public byte[] L002233 = new byte[9];
            public const int L00223c = 138; //new byte[12];
            public const int L002245 = 150; //public byte L002245;
            public const int L002246 = 151; //public UInt16 L002246; // loop count
            public const int FATALERROR = 153; //UInt32 FATALERROR;
            public const int FATALERRORADR = 157; //UInt32 FATALERRORADR;
            public const int PLAYTIME = 161; //public UInt32 PLAYTIME; // 演奏時間
            public const int MUSICTIMER = 165; //public byte MUSICTIMER;  // 演奏時間タイマー定数
            public const int STOPMUSICTIMER = 166; //public byte STOPMUSICTIMER;  // 演奏時間タイマー停止
            public const int MEASURETIMELIMIT = 167; //public UInt32 MEASURETIMELIMIT; // 演奏時間計測中止時間
            public const int Length = 171;
        }


        public struct MXWORK_KEY
        {
            public const int OPT1=0;
            public const int OPT2=1;
            public const int SHIFT=2;
            public const int CTRL=3;
            public const int XF3=4;
            public const int XF4=5;
            public const int XF5=6;
            public const int Length = 7;
        }

        public struct MXWORK_OPM
        {
            public const int Length = 256;
        }

        public delegate void MXCALLBACK_OPMINTFUNC();

        public enum MXDRV_WORK
        {
            FM = 0,      // FM8ch+PCM1ch
            PCM,         // PCM7ch
            GLOBAL,
            KEY,
            OPM,
            PCM8,
            CREDIT,
            CALLBACK_OPMINT,
        };

        public enum MXDRV_ERR
        {
            MEMORY = 1,
        };

        //# ifndef DLLEXPORT
        //#define DLLEXPORT
        //#endif

        //# ifdef __cplusplus
        //extern "C" {
        //#endif

        //#ifndef __MXDRV_LOADMODULE

        //DLLEXPORT int MXDRV_Start(int samprate, int betw, int pcmbuf, int late, int mdxbuf, int pdxbuf, int opmmode);
        //DLLEXPORT void MXDRV_End(void);
        //DLLEXPORT int MXDRV_GetPCM(void* buf, int len);
        //DLLEXPORT void MXDRV_Play(void* mdx, DWORD mdxsize, void* pdx, DWORD pdxsize);
        //DLLEXPORT void volatile * MXDRV_GetWork(int i);
        //DLLEXPORT void MXDRV(X68REG* reg);
        //DLLEXPORT DWORD MXDRV_MeasurePlayTime(void* mdx, DWORD mdxsize, void* pdx, DWORD pdxsize, int loop, int fadeout);
        //DLLEXPORT void MXDRV_PlayAt(DWORD playat, int loop, int fadeout);
        //DLLEXPORT int MXDRV_TotalVolume(int vol);

        //#endif // __MXDRV_LOADMODULE

        //# ifdef __cplusplus
        //}
        //#endif // __cplusplus

        private void MXDRV_Call(UInt32 a)
        {
            X68REG reg=new X68REG();

            reg.d0 = (a);
            reg.d1 = 0x00;
            MXDRV_(reg);
        }


        private void MXDRV_Call_2(UInt32 a, UInt32 b)
        {
            X68REG reg=new X68REG();

            reg.d0 = (a);
            reg.d1 = (b);
            MXDRV_(reg);
        }


        private void MXDRV_Replay() { MXDRV_Call(0x0f); }
        private void MXDRV_Stop() { MXDRV_Call(0x05); }
        private void MXDRV_Pause() { MXDRV_Call(0x06); }
        private void MXDRV_Cont() { MXDRV_Call(0x07); }
        private void MXDRV_Fadeout() { MXDRV_Call_2(0x0c, 19); }
        private void MXDRV_Fadeout2(UInt32 a) { MXDRV_Call_2(0x0c, (a)); }



        //#endif //__MXDRV_H__

    }
}
