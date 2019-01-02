using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88
{
    public class Z80
    {
        public Mem Mem = null;

        //8bit
        public byte A;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte F;
        public byte H;
        public byte L;

        private byte Ab;
        private byte Bb;
        private byte Cb;
        private byte Db;
        private byte Eb;
        private byte Fb;
        private byte Hb;
        private byte Lb;
        private byte Ac;
        private byte Bc;
        private byte Cc;
        private byte Dc;
        private byte Ec;
        private byte Fc;
        private byte Hc;
        private byte Lc;

        //16bit
        public ushort AF
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
        public ushort BC
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
        public ushort DE
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
        public ushort HL
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

        public ushort IX;
        public ushort IY;


        public void EXX()
        {
            Ac = A; A = Ab; Ab = Ac;
            Bc = B; B = Bb; Bb = Bc;
            Cc = C; C = Cb; Cb = Cc;
            Dc = D; D = Db; Db = Dc;
            Ec = E; E = Eb; Eb = Ec;
            Fc = F; F = Fb; Fb = Fc;
            Hc = H; H = Hb; Hb = Hc;
            Lc = L; L = Lb; Lb = Lc;
        }

        public void EX_DE_HL()
        {
            ushort dmy = DE;
            DE = HL;
            HL = dmy;
        }

        public void EX_AF_AF()
        {
            Ac = A; A = Ab; Ab = Ac;
            Fc = F; F = Fb; Fb = Fc;
        }

        public void LDI()
        {
            Mem.LD_8(DE, Mem.LD_8(HL));
            DE++;
            HL++;
            BC--;
        }

        public void LDI(byte[] buf)
        {
            Mem.LD_8(DE, buf[HL]);
            DE++;
            HL++;
            BC--;
        }

        public void LDIR()
        {
            do
            {
                Mem.LD_8(DE, Mem.LD_8(HL));
                DE++;
                HL++;
                BC--;
            } while (BC != 0);
        }

        public void LDIR_HL(byte[] buf)
        {
            do
            {
                Mem.LD_8(DE, buf[HL]);
                DE++;
                HL++;
                BC--;
            } while (BC != 0);
        }

        public void LDIR_DE(byte[] buf)
        {
            do
            {
                buf[DE] = Mem.LD_8(HL);
                DE++;
                HL++;
                BC--;
            } while (BC != 0);
        }

        public void LDDR()
        {
            do
            {
                Mem.LD_8(DE, Mem.LD_8(HL));
                DE--;
                HL--;
                BC--;
            } while (BC != 0);
        }

        public void RLCA()
        {
            A = (byte)((A << 1) | ((A & 0x80) != 0 ? 1 : 0));
            Carry = (A & 0x80) != 0;
        }

        public void RRCA()
        {
            A = (byte)((A >> 1) | ((A & 0x01) != 0 ? 0x80 : 0));
            Carry = (A & 0x01) != 0;
        }

        public bool Carry
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

        public bool Zero
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

    public class Mem
    {
        private byte[] mainMemory = null;

        public Mem()
        {
            mainMemory = new byte[0x10000];
        }

        public byte LD_8(ushort adr)
        {
            return mainMemory[adr];
        }

        public void LD_8(ushort adr,byte val)
        {
            mainMemory[adr] = val;
        }

        public ushort LD_16(ushort adr)
        {
            return (ushort)((mainMemory[adr] << 8) | mainMemory[adr + 1]);
        }

        public void LD_16(ushort adr, ushort val)
        {
            mainMemory[adr] = (byte)(val >> 8);
            mainMemory[adr + 1] = (byte)val;
        }

        public Stack<ushort> stack = new Stack<ushort>(0x100);
    }

    public class PC88
    {
        public Mem Mem = new Mem();
        public Z80 Z80 = new Z80();

        public PC88()
        {
            Z80.Mem = Mem;
        }

        public byte IN(byte adr)
        {
            return 0;
        }

        public void OUT(byte adr,byte val)
        {

        }

        public void CALL(ushort adr)
        {

        }

        public void RST(ushort adr,params object[] option)
        {

        }

    }

}
