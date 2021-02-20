using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MGSDRV
{
    public class SCCCartridge : Cartridge
    {
        private bool readOnly = true;
        private byte[] mem = new byte[65536];
        private ChipRegister chipRegister;

        public SCCCartridge(ChipRegister chipRegister)
        {
            this.chipRegister = chipRegister;
        }

        public override byte this[ushort address]
        {
            get { return Read(address); }
            set { Write(address, value); }
        }

        private void Write(ushort address, byte data)
        {
            //log.Write(string.Format("SCC Write : Adr:{0:x04} Dat:{1:x02}", address, data));
            if (address == 0x9000)
            {
                if (data == 0) readOnly = true;
                else if (data == 0x3f) readOnly = false;
            }

            if((address & 0xff00) == 0x9800)
            {
                if (address >= 0x9800 && address < 0x9880)
                {
                    int scc1_port = 0;// vgmBuf[vgmAdr + 1] & 0x7f;
                    byte scc1_offset = (byte)address;
                    byte rDat = data;
                    byte scc1_chipid = 0;
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x00), scc1_offset, EnmModel.VirtualModel);
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x01), rDat, EnmModel.VirtualModel);
                }
                else if (address < 0x988a)
                {
                    int scc1_port = 1;
                    byte scc1_offset = (byte)(address-0x9880);
                    byte rDat = data;
                    byte scc1_chipid = 0;
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x00), scc1_offset, EnmModel.VirtualModel);
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x01), rDat, EnmModel.VirtualModel);
                }
                else if (address < 0x988f)
                {
                    int scc1_port = 2;
                    byte scc1_offset = (byte)(address-0x988a);
                    byte rDat = data;
                    byte scc1_chipid = 0;
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x00), scc1_offset, EnmModel.VirtualModel);
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x01), rDat, EnmModel.VirtualModel);
                }
                else if (address == 0x988f)
                {
                    int scc1_port = 3;
                    byte scc1_offset = (byte)(address-0x988f);
                    byte rDat = data;
                    byte scc1_chipid = 0;
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x00), scc1_offset, EnmModel.VirtualModel);
                    chipRegister.writeK051649(scc1_chipid, (uint)((scc1_port << 1) | 0x01), rDat, EnmModel.VirtualModel);
                }

            }

            if (readOnly) return;
            mem[address] = data;
        }

        private byte Read(ushort address)
        {
            //Console.WriteLine("SCC Read : Adr:{0:x04} Dat:{1:x02}", address, 0);
            return mem[address];
        }
    }
}
