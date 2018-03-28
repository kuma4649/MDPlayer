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

        public class MXWORK_CH
        {
            volatile public byte[] S0000; // Ptr
            public byte S0004_b;  // PCM bank
            volatile public byte[] S0004; // voice ptr
            public UInt32 S0008;    // bend delta
            public UInt32 S000c;    // bend offset
            public UInt16 S0010;    // D
            public UInt16 S0012;    // note+D
            public UInt16 S0014;    // note+D+bend+Pitch LFO offset
            public byte S0016;    // flags b3=keyon/off
            public byte S0017;    // flags
            public byte S0018;    // ch
            public byte S0019;    // carrier slot
            public byte S001a;    // len
            public byte S001b;    // gate
            public byte S001c;    // p
            public byte S001d;    // keyon slot
            public byte S001e;    // Q
            public byte S001f;    // Keyon delay
            public byte S0020;    // Keyon delay counter
            public byte S0021;    // PMS/AMS
            public byte S0022;    // v
            public byte S0023;    // v last
            public byte S0024;    // LFO delay
            public byte S0025;    // LFO delay counter
            volatile public byte[] S0026; // Pitch LFO Type
            public UInt32 S002a;    // Pitch LFO offset start
            public UInt32 S002e;    // Pitch LFO delta start
            public UInt32 S0032;    // Pitch LFO delta
            public UInt32 S0036;    // Pitch LFO offset
            public UInt16 S003a;    // Pitch LFO length (cooked)
            public UInt16 S003c;    // Pitch LFO length
            public UInt16 S003e;    // Pitch LFO length counter
            volatile public byte[] S0040; // Volume LFO Type
            public UInt16 S0044;    // Volume LFO delta start
            public UInt16 S0046;    // Volume LFO delta (cooked)
            public UInt16 S0048;    // Volume LFO delta
            public UInt16 S004a;    // Volume LFO offset
            public UInt16 S004c;    // Volume LFO length
            public UInt16 S004e;    // Volume LFO length counter
        }


        public class MXWORK_GLOBAL
        {
            public UInt16 L001ba6;
            public UInt32 L001ba8;
            volatile public byte[] L001bac;
            public byte[] L001bb4 = new byte[16];
            public byte L001df4;
            public byte[] L001df6 = new byte[16];
            public UInt16 L001e06;  // Channel Mask (true)
            public byte L001e08;
            public byte L001e09;
            public byte L001e0a;
            public byte L001e0b;
            public byte L001e0c;  // @t
            public byte L001e0d;
            public byte L001e0e;
            public byte L001e10;
            public byte L001e12;  // Paused
            public byte L001e13;  // End
            public byte L001e14;  // Fadeout Offset
            public byte L001e15;
            public byte L001e17;  // Fadeout Enable
            public byte L001e18;
            public byte L001e19;
            public UInt16 L001e1a;  // Channel Enable
            public UInt16 L001e1c;  // Channel Mask
            public UInt16[] L001e1e = new UInt16[2];   // Fadeout Speed
            public UInt16 L001e22;
            volatile public byte[] L001e24;
            volatile public byte[] L001e28;
            volatile public byte[] L001e2c;
            volatile public byte[] L001e30;
            volatile public byte[] L001e34;
            volatile public byte[] L001e38;
            public UInt32 L00220c;
            volatile public byte[] L002218;
            volatile public byte[] L00221c;
            public UInt32 L002220; // L_MDXSIZE
            public UInt32 L002224; // L_PDXSIZE
            volatile public byte[] L002228;   // voice data
            volatile public byte[] L00222c;
            public byte L002230;
            public byte L002231;
            public byte L002232;
            public byte[] L002233 = new byte[9];
            public byte[] L00223c = new byte[12];
            public byte L002245;
            public UInt16 L002246; // loop count
            public UInt32 FATALERROR;
            public UInt32 FATALERRORADR;
            public UInt32 PLAYTIME; // 演奏時間
            public byte MUSICTIMER;  // 演奏時間タイマー定数
            public byte STOPMUSICTIMER;  // 演奏時間タイマー停止
            public UInt32 MEASURETIMELIMIT; // 演奏時間計測中止時間
        }


        public class MXWORK_KEY
        {
            public byte OPT1;
            public byte OPT2;
            public byte SHIFT;
            public byte CTRL;
            public byte XF3;
            public byte XF4;
            public byte XF5;
        }

        public byte[] MXWORK_OPM = new byte[256];
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
            //X68REG reg;

            //reg.d0 = (a);
            //reg.d1 = 0x00;
            //MXDRV(&reg);
        }


        private void MXDRV_Call_2(UInt32 a, UInt32 b)
        {
            //X68REG reg;

            //reg.d0 = (a);
            //reg.d1 = (b);
            //MXDRV(&reg);
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
