using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MGSDRV
{
    public class MSXMusicCartridge : Cartridge
    {
        private readonly byte[] memory = new byte[65536];

        public MSXMusicCartridge()
        {

            memory[0x4018] = (byte)'A';
            memory[0x4019] = (byte)'P';
            memory[0x401a] = (byte)'R';
            memory[0x401b] = (byte)'L';
            memory[0x401c] = (byte)'O';
            memory[0x401d] = (byte)'P';
            memory[0x401e] = (byte)'L';
            memory[0x401f] = (byte)'L';

        }

        public override byte this[ushort address]
        {
            get => memory[address];
            set
            {
                Write(address, value);
            }
        }


        private void Write(ushort adr, byte dat)
        {

        }

    }
}
