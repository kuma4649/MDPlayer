using Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MXDRV
{
    //public class X68Sound //: NX68Sound.NX68Sound
    //{
    //    public enum X68SNDERR
    //    {
    //        PCMOUT = (-1),
    //        TIMER = (-2),
    //        MEMORY = (-3),
    //        NOTACTIVE = (-4),
    //        ALREADYACTIVE = (-5),
    //        BADARG = (-6),

    //        DLL = (-1),
    //        FUNC = (-2)

    //    }

    //    // DMAに16bit値をビッグエンディアン(68オーダー)で書き込む
    //    public void DmaPokeW(byte adrs, UInt16 data) {
    //        DmaPoke(adrs, (byte)(data >> 8));
    //        DmaPoke((byte)(adrs + 1), (byte)(data));
    //    }
    //    // DMAに32bit値をビッグエンディアン(68オーダー)で書き込む
    //    public void DmaPokeL(byte adrs, Ptr<byte> data)
    //    {
    //        DmaPoke(adrs, (byte)data[0]);// (byte)((UInt32)data >> 24));
    //        DmaPoke((byte)(adrs + 1), (byte)data[1]);// (byte)(data >> 16));
    //        DmaPoke((byte)(adrs + 2), (byte)data[2]);// (byte)(data >> 8));
    //        DmaPoke((byte)(adrs + 3), (byte)data[3]);// (byte)(data));
    //    }
    //    public void DmaPokeL(byte adrs, UInt32 data)
    //    {
    //        DmaPoke(adrs, (byte)((UInt32)data >> 24));
    //        DmaPoke((byte)(adrs + 1), (byte)(data >> 16));
    //        DmaPoke((byte)(adrs + 2), (byte)(data >> 8));
    //        DmaPoke((byte)(adrs + 3), (byte)(data));
    //    }
    //    public void y(byte no,byte data) {
    //        OpmReg(no);
    //        OpmPoke(data);
    //    }


    //    public Int32 Load() { return 0; }

    //    public Int32 Start(Int32 a, Int32 b, Int32 c, Int32 d, Int32 e, Int32 f, double g) { return 0; }
    //    public Int32 Samprate(Int32 a) { return 0; }
    //    public void Reset() { }
    //    public void Free() { }
    //    public delegate void dlgCallBack();
    //    public void BetwInt(dlgCallBack cb) { }

    //    public Int32 StartPcm(Int32 a, Int32 b, Int32 c, Int32 d) { return 0; }
    //    public Int32 GetPcm(ref byte[] a, Int32 b) { return 0; }

    //    public byte OpmPeek() { return 0; }
    //    public void OpmReg(byte a) { }
    //    public void OpmPoke(byte a) { }
    //    public void OpmInt(Action cb) { }
    //    public Int32 OpmWait(Int32 a) { return 0; }
    //    public Int32 OpmClock(Int32 a) { return 0; }

    //    public byte AdpcmPeek() { return 0; }
    //    public void AdpcmPoke(byte a) { }
    //    public byte PpiPeek() { return 0; }
    //    public void PpiPoke(byte a) { }
    //    public void PpiCtrl(byte a) { }
    //    public byte DmaPeek(byte a) { return 0; }
    //    public void DmaPoke(byte a, byte b) { }
    //    public void DmaInt(dlgCallBack cb) { }
    //    public void DmaErrInt(dlgCallBack cb) { }
    //    public delegate Int32 dlgCB1(byte[] a);
    //    public void MemReadFunc(dlgCB1 a) { }

    //    public delegate Int32 dlgCB2();
    //    public void WaveFunc(dlgCB2 a) { }

    //    public Int32 Pcm8_Out(UInt32 a, object b, UInt32 c, UInt32 d) { return 0; }
    //    public Int32 Pcm8_Aot(UInt32 a, object b, UInt32 c, UInt32 d) { return 0; }
    //    public Int32 Pcm8_Lot(UInt32 a, object b, UInt32 c) { return 0; }
    //    public Int32 Pcm8_SetMode(UInt32 a, UInt32 b) { return 0; }
    //    public Int32 Pcm8_GetRest(UInt32 a) { return 0; }
    //    public Int32 Pcm8_GetMode(UInt32 a) { return 0; }
    //    public Int32 Pcm8_Abort() { return 0; }

    //    public Int32 TotalVolume(Int32 v) { return 0; }

    //    public Int32 ErrorCode() { return 0; }
    //    public Int32 DebugValue() { return 0; }

    //}
}
