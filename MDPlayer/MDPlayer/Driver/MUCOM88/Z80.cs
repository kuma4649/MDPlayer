using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88
{
    public static class Z80
    {
        //regs

        //8bit
        public static byte A;
        public static byte B;
        public static byte C;
        public static byte D;
        public static byte E;
        public static byte F;
        public static byte H;
        public static byte L;

        //16bit
        public static ushort AF
        {
            get
            {
                return (ushort)((A << 8) | F);
            }
            set
            {
                A = (byte)(value >> 8);
                F = (byte)value;
            }
        }
        public static ushort BC
        {
            get
            {
                return (ushort)((B << 8) | C);
            }
            set
            {
                B = (byte)(value >> 8);
                C = (byte)value;
            }
        }
        public static ushort DE
        {
            get
            {
                return (ushort)((D << 8) | E);
            }
            set
            {
                D = (byte)(value >> 8);
                E = (byte)value;
            }
        }
        public static ushort HL
        {
            get
            {
                return (ushort)((H << 8) | L);
            }
            set
            {
                H = (byte)(value >> 8);
                L = (byte)value;
            }
        }

        public static bool Carry
        {
            get
            {
                return (F & 0x1) != 0;
            }
            set
            {
                F = (byte)(value ? (F | 1) : (F & 0xfe));
            }
        }

        public static bool Zero
        {
            get
            {
                return (F & 0x40) != 0;
            }
            set
            {
                F = (byte)(value ? (F | 0x40) : (F & 0xbf));
            }
        }

    }

    public static class Mem
    {
        private static byte[] mainMemory = null;

        public static void Init()
        {
            mainMemory = new byte[0x10000];
        }

        public static byte LD_8(ushort adr)
        {
            return mainMemory[adr];
        }

        public static void LD_8(ushort adr,byte val)
        {
            mainMemory[adr] = val;
        }

        public static ushort LD_16(ushort adr)
        {
            return (ushort)((mainMemory[adr] << 8) | mainMemory[adr + 1]);
        }

        public static void LD_16(ushort adr, ushort val)
        {
            mainMemory[adr] = (byte)(val >> 8);
            mainMemory[adr + 1] = (byte)val;
        }
    }

    public static class PC88
    {
        public static byte IN(byte adr)
        {
            return 0;
        }

        public static void OUT(byte adr,byte val)
        {

        }
    }

}
