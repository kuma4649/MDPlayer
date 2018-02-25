using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sidplayFpNET.libsidplayfp.c64.Banks
{
    public interface IPLA
    {
        void setCpuPort(byte state);
        byte getLastReadByte();
        Int64 getPhi2Time();
    }
}
