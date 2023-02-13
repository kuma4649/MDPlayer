using Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MXDRV
{
    public class depend
    {
        public const int FALSE = 0;
        public const int TRUE = 1;

        public const int SET = 255;
        public const int CLR = 0;
        public static UInt16 GETBWORD(xMemory mm, UInt32 a) {
            return (UInt16)(
                (mm.ReadByte((UInt32)(a + 0)) * 256) 
                + mm.ReadByte((UInt32)(a + 1))
                );
        }
        public static UInt32 GETBLONG(xMemory mm, UInt32 a)
        {
            return (UInt32)(
                (mm.ReadByte((UInt32)(a + 0)) * 16777216)
                + (mm.ReadByte((UInt32)(a + 1)) * 65536)
                + (mm.ReadByte((UInt32)(a + 2)) * 256)
                + mm.ReadByte((UInt32)(a + 3)));
        }

        public static void PUTBWORD(Ptr<byte> a, UInt16 b) {
            a[0] = (byte)(b >> 8);
            a[1] = (byte)(b >> 0);
        }
        public static void PUTBLONG(xMemory mm,UInt32 a, UInt32 b) {
            mm.Write((UInt32)(a + 0), (byte)(b >> 24));
            mm.Write((UInt32)(a + 1), (byte)(b >> 16));
            mm.Write((UInt32)(a + 2), (byte)(b >> 8));
            mm.Write((UInt32)(a + 3), (byte)(b >> 0));
        }

    }

    
    public class X68REG
    {
        public UInt32 d0;
        public UInt32 d1;
        public UInt32 d2;
        public UInt32 d3;
        public UInt32 d4;
        public UInt32 d5;
        public UInt32 d6;
        public UInt32 d7;
        public UInt32 a0;
        public UInt32 a1;
        public UInt32 a2;
        public UInt32 a3;
        public UInt32 a4;
        public UInt32 a5;
        public UInt32 a6;
        public UInt32 a7;
    }

}
