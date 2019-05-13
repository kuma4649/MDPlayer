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
        public byte I = 0;//dummy

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
            //Ac = A; A = Ab; Ab = Ac;
            Bc = B; B = Bb; Bb = Bc;
            Cc = C; C = Cb; Cb = Cc;
            Dc = D; D = Db; Db = Dc;
            Ec = E; E = Eb; Eb = Ec;
            //Fc = F; F = Fb; Fb = Fc;
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
            Carry = (A & 0x80) != 0;
            A = (byte)((A << 1) | ((A & 0x80) != 0 ? 1 : 0));
        }

        public void RRCA()
        {
            Carry = (A & 0x01) != 0;
            A = (byte)((A >> 1) | ((A & 0x01) != 0 ? 0x80 : 0));
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
        private byte[][] gvram = null;
        private byte[] tvram = null;
        public enum EnmGVRAM : int
        {
            MainRAM=0,
            BPLANE
        }
        public EnmGVRAM GVRAM_SW = EnmGVRAM.MainRAM;
        public bool MMODE = false; //false:ROM/RAM mode   true:64K RAM mode
        public bool TMODE = false;//false:TextVRAM  true:MainVRAM

        public Mem()
        {
            mainMemory = new byte[0x10000];
            gvram = new byte[3][]
            {
                new byte[0x10000-0xc000],
                null,
                null
            };
            tvram = new byte[0x10000 - 0xf000];
        }

        private byte this[ushort index]
        {
            set
            {
                byte[] buf = mainMemory;
                ushort adr = index;

                if (index < 0x8000)
                {
                    //MMODEに関係なく書き込みはmainMemoryに行われる
                    //if (!MMODE)
                    //{
                    //}
                }
                else if (index >= 0xc000)
                {
                    switch (GVRAM_SW)
                    {
                        case EnmGVRAM.MainRAM:
                            if (index >= 0xf000 && !TMODE)
                            {
                                buf = tvram;
                                adr -= 0xf000;
                            }
                            break;
                        case EnmGVRAM.BPLANE:
                            buf = gvram[0];
                            adr -= 0xc000;
                            break;
                    }
                }

                buf[adr] = value;
            }

            get
            {
                byte[] buf = mainMemory;
                ushort adr = index;

                if (index < 0x8000)
                {
                    if (!MMODE)
                    {
                        throw new NotSupportedException("MDPlayerはPC88のROM領域の読み込みに対応していません");
                    }
                }
                if (index >= 0xc000)
                {
                    switch (GVRAM_SW)
                    {
                        case EnmGVRAM.MainRAM:
                            if (index >= 0xf000 && !TMODE)
                            {
                                buf = tvram;
                                adr -= 0xf000;
                            }
                            break;
                        case EnmGVRAM.BPLANE:
                            buf = gvram[0];
                            adr -= 0xc000;
                            break;
                    }
                }

                return buf[adr];
            }
        }

        public byte LD_8(ushort adr)
        {
            return this[adr];
        }

        public void LD_8(ushort adr,byte val)
        {
            this[adr] = val;
            //if(adr>=0xf3c8 && adr < 0xfe80)
            //{
            //    int loc = (adr - 0xf3c8) / 120;
            //    log.ForcedWrite(Encoding.GetEncoding("Shift_JIS").GetString(mainMemory, 0xf3c8 + loc * 120, 80));
            //}
        }

        public ushort LD_16(ushort adr)
        {
            return (ushort)((this[(ushort)(adr + 1)] << 8) | this[adr]);
        }

        public void LD_16(ushort adr, ushort val)
        {
            this[adr] = (byte)val;
            this[(ushort)(adr + 1)] = (byte)(val >> 8);
            //if (adr >= 0xf3c8 && adr < 0xfe80)
            //{
            //    int loc = (adr - 0xf3c8) / 120;
            //    log.ForcedWrite(Encoding.GetEncoding("Shift_JIS").GetString(mainMemory, 0xf3c8 + loc * 120, 80));
            //}
        }

        public Stack<ushort> stack = new Stack<ushort>(0x100);
    }

    public class PC88
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public ChipRegister ChipRegister = null;
        public MNDRV.FMTimer fmTimer = null;
        public byte port0Adr = 0;
        public byte port1Adr = 0;
        public EnmModel model = EnmModel.VirtualModel;

        public PC88()
        {
            port0Adr = 0;
            port1Adr = 0;
        }

        public byte IN(byte adr)
        {
            //log.Write(string.Format("Port In:Adr[{0:x02}]", adr));
            switch (adr)
            {
                case 0x44:
                case 0x45:
                    return 1;
                case 0x46:
                    return 1;
                case 0x47:
                    return 1;
            }
            return 0;
        }

        public void OUT(byte adr,byte val)
        {
            //log.Write(string.Format("Port Out:Adr[{0:x02}] val[{1:x02}]", adr, val));
            switch (adr)
            {
                case 0x31:
                    Mem.MMODE = ((val & 0x2) != 0);
                    break;
                case 0x32:
                    Mem.TMODE = ((val & 0x10) != 0);
                    break;
                case 0x44:
                    port0Adr = val;
                    break;
                case 0x45:
                    //log.Write(string.Format("FM P0 Out:Adr[{0:x02}] val[{1:x02}]", (int)port0Adr, (int)val));
                    ChipRegister.YM2608SetRegister(0,0, 0, (int)port0Adr, (int)val);
                    fmTimer.WriteReg(port0Adr, val);
                    break;
                case 0x46:
                    port1Adr = val;
                    break;
                case 0x47:
                    //log.Write(string.Format("FM P1 Out:Adr[{0:x02}] val[{1:x02}]", (int)port1Adr, (int)val));
                    ChipRegister.YM2608SetRegister(0,0, 1, (int)port1Adr, (int)val);
                    break;
                case 0x5c:
                    Mem.GVRAM_SW = Mem.EnmGVRAM.BPLANE;
                    break;
                case 0x5f:
                    Mem.GVRAM_SW = Mem.EnmGVRAM.MainRAM;
                    break;
                case 0xa8:
                case 0xa9:
                case 0xac:
                case 0xad:
                    break;
                case 0xe4://割り込みレベル
                case 0xe6://割り込みマスク
                    //何もしない
                    break;
                default:
                    throw new NotSupportedException(string.Format("ポート${0:x02}は未対応です",adr));
            }
        }

        public void CALL(ushort adr)
        {
            if (adr == 0x3b3) {
                log.Write("!! Error Trap !!");
                return;
            }
            else if (adr == 0xb036)
            {
                Z80.IX = 0xc9bf;
                return;
            }
            else if(adr>=0xb000 && adr < 0xb040)
            {
                return;
            }
        }

        public void RST(ushort adr,params object[] option)
        {

        }

    }

}
