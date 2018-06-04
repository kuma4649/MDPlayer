using System;

namespace NX68Sound
{
    public class X68Sound
    {

        public const int X68SNDERR_PCMOUT = (-1);
        public const int X68SNDERR_TIMER = (-2);
        public const int X68SNDERR_MEMORY = (-3);
        public const int X68SNDERR_NOTACTIVE = (-4);
        public const int X68SNDERR_ALREADYACTIVE = (-5);
        public const int X68SNDERR_BADARG = (-6);
        public const int X68SNDERR_DLL = (-1);
        public const int X68SNDERR_FUNC = (-2);

        //# include	"x68sound.h"

        //# ifdef ROMEO
        //# include	"JULIET.H"
        //#endif
        //# ifdef C86CTL
        //# include	"c86ctl.h"
        //#endif

        //# include	"global.h"
        //# include	"op.h"
        //# include	"lfo.h"
        //# include	"adpcm.h"
        //# include	"pcm8.h"
        //# include	"opm.h"



        global global = null;

        public Opm opm;

        public X68Sound()
        {
            global = new global();
            opm = new Opm(global);
            global.opm = opm;
        }

        public void MountMemory(byte[] mem)
        {
            global.mountMemory(mem);
        }






        public int X68Sound_Start(int samprate = 44100, int opmflag = 1, int adpcmflag = 1,
                          int betw = 5, int pcmbuf = 5, int late = 200, double rev = 1.0)
        {
            return opm.Start(samprate, opmflag, adpcmflag, betw, pcmbuf, late, rev);
        }

        public int X68Sound_Samprate(int samprate=44100)
        {
            return opm.SetSamprate(samprate);
        }

        public int X68Sound_OpmClock(int clock)
        {
            return opm.SetOpmClock(clock);
        }

        public void X68Sound_Reset()
        {
            opm.Reset();
        }

        public void X68Sound_Free()
        {
            opm.Free();
        }

        public void X68Sound_BetwInt(Action proc)
        {
            opm.BetwInt(proc);
        }

        public int X68Sound_StartPcm(int samprate = 44100, int opmflag = 1, int adpcmflag = 1, int pcmbuf = 5)
        {
            return opm.StartPcm(samprate, opmflag, adpcmflag, pcmbuf);
        }

        public int X68Sound_GetPcm(short[] buf, int len)
        {
            return opm.GetPcm(buf, len);
        }

        public byte X68Sound_OpmPeek()
        {
            return opm.OpmPeek();
        }

        public void X68Sound_OpmReg(byte no)
        {
            opm.OpmReg(no);
        }

        public void X68Sound_OpmPoke(byte data)
        {
            opm.OpmPoke(data);
        }

        public void X68Sound_OpmInt(Action proc)
        {
            opm.OpmInt(proc);
        }

        public int X68Sound_OpmWait(int wait)
        {
            return opm.SetOpmWait(wait);
        }

        public byte X68Sound_AdpcmPeek()
        {
            return opm.AdpcmPeek();
        }

        public void X68Sound_AdpcmPoke(byte data)
        {
            opm.AdpcmPoke(data);
        }

        public byte X68Sound_PpiPeek()
        {
            return opm.PpiPeek();
        }

        public void X68Sound_PpiPoke(byte data)
        {
            opm.PpiPoke(data);
        }

        public void X68Sound_PpiCtrl(byte data)
        {
            opm.PpiCtrl(data);
        }

        public byte X68Sound_DmaPeek(byte adrs)
        {
            return opm.DmaPeek(adrs);
        }

        public void X68Sound_DmaPoke(byte adrs, byte data)
        {
            opm.DmaPoke(adrs, data);
        }

        public void X68Sound_DmaPokeW(byte adrs, ushort data)
        {
            opm.DmaPoke(adrs, (byte)(data >> 8));
            opm.DmaPoke((byte)(adrs + 1), (byte)data);
        }

        public void X68Sound_DmaPokeL(byte adrs,UInt32 dataPtr)
        {
            opm.DmaPoke(adrs, (byte)(dataPtr >> 24));
            opm.DmaPoke((byte)(adrs + 1), (byte)(dataPtr >> 16));
            opm.DmaPoke((byte)(adrs + 2), (byte)(dataPtr >> 8));
            opm.DmaPoke((byte)(adrs + 3), (byte)(dataPtr));
        }

        public void X68Sound_y(byte no, byte data)
        {
            opm.OpmReg(no);
            opm.OpmPoke(data);
        }

        public void X68Sound_DmaInt(Action proc)
        {
            opm.DmaInt(proc);
        }

        public void X68Sound_DmaErrInt(Action proc)
        {
            opm.DmaErrInt(proc);
        }

        public void X68Sound_MemReadFunc(Func< uint, int> func)
        {
            opm.MemReadFunc(func);
        }

        public void X68Sound_WaveFunc(Func<int> func)
        {
            opm.SetWaveFunc(func);
        }

        public int X68Sound_Pcm8_Out(int ch, byte[] adrsBuf, uint adrsPtr, int mode, int len)
        {
            return opm.Pcm8_Out(ch, adrsBuf, adrsPtr, mode, len);
        }

        public int X68Sound_Pcm8_Aot(int ch, byte[] tblBuf, uint tblPtr, int mode, int cnt)
        {
            return opm.Pcm8_Aot(ch, tblBuf, tblPtr, mode, cnt);
        }

        public int X68Sound_Pcm8_Lot(int ch, byte[] tblBuf, uint tblPtr, int mode)
        {
            return opm.Pcm8_Lot(ch, tblBuf, tblPtr, mode);
        }

        public int X68Sound_Pcm8_SetMode(int ch, int mode)
        {
            return opm.Pcm8_SetMode(ch, mode);
        }

        public int X68Sound_Pcm8_GetRest(int ch)
        {
            return opm.Pcm8_GetRest(ch);
        }

        public int X68Sound_Pcm8_GetMode(int ch)
        {
            return opm.Pcm8_GetMode(ch);
        }

        public int X68Sound_Pcm8_Abort()
        {
            return opm.Pcm8_Abort();
        }


        public int X68Sound_TotalVolume(int v)
        {
            return opm.SetTotalVolume(v);
        }

        public void X68Sound_SetMask(int v)
        {
            opm.SetMask(v);
        }




        public int X68Sound_ErrorCode()
        {
            return global.ErrorCode;
        }

        public int X68Sound_DebugValue()
        {
            return global.DebugValue;
        }

        public void X68Sound_TimerA()
        {
            opm.CsmKeyOn();
        }
    }
}
