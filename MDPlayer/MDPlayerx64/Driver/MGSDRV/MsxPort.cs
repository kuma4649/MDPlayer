using Konamiman.Z80dotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MGSDRV
{
    public class MsxPort : IMemory
    {
        private MSXSlot slot;
        private ChipRegister chipRegister;
        private MSXVDP vdp;
        private EnmModel model;
        private byte opllAdr;
        private byte ay8910Adr;

        public MsxPort(MSXSlot slot, ChipRegister chipRegister,MSXVDP vdp,EnmModel model)
        {
            this.slot = slot;
            this.chipRegister= chipRegister;
            this.vdp = vdp;
            this.model = model;
        }

        public byte this[int address]
        {
            get
            {
                return InPort(address);
            }

            set
            {
                OutPort(address, value);
            }
        }

        public int Size => throw new System.NotImplementedException();

        public byte[] GetContents(int startAddress, int length)
        {
            throw new System.NotImplementedException();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            throw new System.NotImplementedException();
        }

        private void OutPort(int address, byte value)
        {

            switch (address)
            {
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                    vdp?.Write(address, value);
                    break;
                case 0xa0:
                    ay8910Adr = value;
                    break;
                case 0xa1:
                    chipRegister?.setAY8910Register(0, ay8910Adr, value, model);
                    break;
                case 0xa2:
                    //log.Write("PSG Port Adr:{0:x04} Dat:{1:x02}", address, value);
                    break;
                case 0x7c:
                    opllAdr = value;
                    break;
                case 0x7d:
                    chipRegister?.setYM2413Register(0, opllAdr, value, model);
                    //log.Write("OPLL Port Adr:{0:x04} Dat:{1:x02}", address, value);
                    break;
                case 0xa8:
                    //log.Write("ChangeSlot Port Adr:{0:x04} Dat:{1:x02}", address, value);
                    ChangeSlot(value);
                    break;
                default:
                    //log.Write(LogLevel.Trace, "Port out Adr:{0:x04} Dat:{1:x02}", address, value);
                    break;
            }

        }

        private byte InPort(int address)
        {

            switch(address)
            {
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                    if (vdp == null) return 0;
                    return vdp.Read(address);
                case 0xa8:
                    //log.Write("ChangeSlot Port in  Adr:{0:x04}", address);
                    return ReadSlot();
            }

            //log.Write(LogLevel.Trace, "Port in  Adr:{0:x04}", address);
            return 0;
        }

        private void ChangeSlot(byte value)
        {
            int bs;

            for (int p = 0; p < 4; p++)
            {
                bs = (value >> (p * 2)) & 0x3;
                slot.SetPageFromSlot(p, bs);
            }
        }

        private byte ReadSlot()
        {
            return (byte)(
                ((slot.pagesSlotPos[0].basic & 3) << 0)
                | ((slot.pagesSlotPos[1].basic & 3) << 2)
                | ((slot.pagesSlotPos[2].basic & 3) << 4)
                | ((slot.pagesSlotPos[3].basic & 3) << 6)
                );
        }
    }
}
