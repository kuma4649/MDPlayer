using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.GBS
{
    public class Memory
    {
        public byte[][] cartROM;
        private List<byte[]> vRAM;
        private List<byte[]> exRAM;
        private List<byte[]> wkRAM;
        private byte[] spriteAtrTbl;
        private byte[] hiRAM;
        public int cartROMBank = 1;
        public int vRAMBank = 0;
        public int exRAMBank = 0;
        public int wkRAMBank = 1;
        public byte ier = 0;

        public IO io;

        public static Memory Empty => new(new byte[1][], null);

        public Memory(byte[][] memoryArray, IO io)
        {
            cartROM = memoryArray;
            this.io = io;

            cartROMBank = 1;
            vRAMBank = 0;
            exRAMBank = 0;
            wkRAMBank = 1;

            vRAM = new List<byte[]>();
            vRAM.Add(new byte[0x2000]);

            exRAM = new List<byte[]>();
            exRAM.Add(new byte[0x2000]);

            wkRAM = new List<byte[]>();
            wkRAM.Add(new byte[0x1000]);
            wkRAM.Add(new byte[0x1000]);

            spriteAtrTbl = new byte[0xa0];
            hiRAM = new byte[0x80];

            ier = 0;
        }

        public byte PeekB(int pc)
        {
            pc = (ushort)pc;

            if (pc >= 0x0000 && pc < 0x4000)
            {
                return cartROM[0][(ushort)(pc - 0x0000)];
            }
            else if (pc >= 0x4000 && pc < 0x8000)
            {
                return cartROM[cartROMBank][(ushort)(pc - 0x4000)];
            }
            else if (pc >= 0x8000 && pc < 0xa000)
            {
                return vRAM[vRAMBank][(ushort)(pc - 0x8000)];
            }
            else if (pc >= 0xa000 && pc < 0xc000)
            {
                return exRAM[exRAMBank][(ushort)(pc - 0xa000)];
            }
            else if (pc >= 0xc000 && pc < 0xd000)
            {
                return wkRAM[0][(ushort)(pc - 0xc000)];
            }
            else if (pc >= 0xd000 && pc < 0xe000)
            {
                return wkRAM[wkRAMBank][(ushort)(pc - 0xd000)];
            }
            else if (pc >= 0xe000 && pc < 0xf000)
            {
                return wkRAM[0][(ushort)(pc - 0xe000)];
            }
            else if (pc >= 0xf000 && pc < 0xfe00)
            {
                return wkRAM[wkRAMBank][(ushort)(pc - 0xf000)];
            }
            else if (pc >= 0xfe00 && pc < 0xfea0)
            {
                return spriteAtrTbl[(ushort)(pc - 0xfe00)];
            }
            else if (pc >= 0xfea0 && pc < 0xff00)
            {
                //return 0;//Not use
                throw new ArgumentOutOfRangeException("Not use area.");
            }
            else if (pc >= 0xff00 && pc < 0xff80)
            {
                return io.Read((byte)pc);
            }
            else if (pc >= 0xff80 && pc < 0xffff)
            {
                return hiRAM[(ushort)(pc - 0xff80)];
            }
            else if (pc == 0xffff)
            {
                return ier;//Interrupt Enable Register
            }
            else
            {
                throw new ArgumentOutOfRangeException("Not written to RAM area.");
            }
        }

        public void PokeB(int pc, byte dat,long vgmFrameCounter)
        {
            if (pc >= 0x2000 && pc < 0x4000)
            {
                cartROMBank = dat;
                if (cartROMBank >= cartROM.Length) throw new ArgumentOutOfRangeException("Switching to a non-existent Bank.");
            }
            else if ((pc >= 0x4000 && pc < 0x6000) || pc == 0xff70)
            {
                ;//無視する
            }
            else if (pc >= 0x8000 && pc < 0xa000)
            {
                vRAM[vRAMBank][(ushort)(pc - 0x8000)] = dat;
            }
            else if (pc >= 0xa000 && pc < 0xc000)
            {
                exRAM[exRAMBank][(ushort)(pc - 0xa000)] = dat;
            }
            else if (pc >= 0xc000 && pc < 0xd000)
            {
                wkRAM[0][(ushort)(pc - 0xc000)] = dat;
            }
            else if (pc >= 0xd000 && pc < 0xe000)
            {
                wkRAM[wkRAMBank][(ushort)(pc - 0xd000)] = dat;
            }
            else if (pc >= 0xe000 && pc < 0xf000)
            {
                wkRAM[0][(ushort)(pc - 0xe000)] = dat;
            }
            else if (pc >= 0xf000 && pc < 0xfe00)
            {
                wkRAM[wkRAMBank][(ushort)(pc - 0xf000)] = dat;
            }
            else if (pc >= 0xfe00 && pc < 0xfea0)
            {
                spriteAtrTbl[(ushort)(pc - 0xfe00)] = dat;
            }
            else if (pc >= 0xfea0 && pc < 0xff00)
            {
                ;//Not use
            }
            else if (pc >= 0xff00 && pc < 0xff80)
            {
                io.Write((byte)pc, dat, vgmFrameCounter);
            }
            else if (pc >= 0xff80 && pc < 0xffff)
            {
                hiRAM[(ushort)(pc - 0xff80)] = dat;
            }
            else if (pc == 0xffff)
            {
                ier = dat;//Interrupt Enable Register
            }
            else
            {
#if DEBUG
                Console.WriteLine("Written to ROM area ${0:X04}.", pc);
#endif
            }
        }

        public ushort PeekW(int pc)
        {
            return (ushort)(PeekB(pc) + (PeekB(pc + 1) << 8));
        }

        public void PokeW(int pc, ushort dat, long vgmFrameCounter)
        {
            PokeB(pc, (byte)dat, vgmFrameCounter);
            PokeB(pc + 1, (byte)(dat >> 8), vgmFrameCounter);
        }

        public string GetBank()
        {
            return string.Format("bnk {0}:{1}:{2}:{3}", cartROMBank, vRAMBank, exRAMBank, wkRAMBank);
        }
    }
}
