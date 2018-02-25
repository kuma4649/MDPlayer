using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sidplayFpNET.libsidplayfp.c64.Banks
{
    public interface IBank
    {
        void poke(UInt16 address, byte value);
        byte peek(UInt16 address);
    }
}
