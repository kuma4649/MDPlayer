using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class Memory98
    {
        private byte[] mem = null;

        public Memory98(int size)
        {
            mem = new byte[size];
        }

        public void PokeB(int ptr, byte dat)
        {
            mem[ptr % mem.Length] = dat;
        }

        public void PokeW(int ptr, Int16 dat)
        {
            mem[(uint)ptr % mem.Length] = (byte)dat;
            mem[((uint)ptr + 1) % mem.Length] = (byte)(dat >> 8);
        }

        public byte PeekB(int ptr)
        {
            return mem[((uint)ptr % mem.Length)];
        }

        public Int16 PeekW(int ptr)
        {
            return (Int16)(
                mem[(uint)ptr % mem.Length]
                + (mem[((uint)ptr + 1) % mem.Length] << 8));
        }

        public void SetHookAddress(int startAdr, int endAdr, Action<int, byte> write, Func<int> read)
        {

        }
    }
}
