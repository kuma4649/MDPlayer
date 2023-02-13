using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MXDRV
{
    public class xMemory
    {
        public byte[] mm;

        public void alloc(int size)
        {
            mm = new byte[size];
        }

        public void realloc(uint size)
        {
            byte[] m = new byte[size];
            if (mm != null && mm.Length > 0)
            {
                int s = (mm.Length < size ? mm.Length : (int)size);
                Array.Copy(mm, m, s);
                mm = m;
            }
        }

        public void Write(UInt32 v1, byte v2)
        {
            //Console.WriteLine("{0:x08}:{1:x02}", v1, v2);
            mm[v1] = v2;
        }

        public void Write(UInt32 v1, UInt16 v2)
        {
            Write(v1, (byte)(v2 >> 8));
            Write(v1+1, (byte)(v2 >> 0));
        }

        public void Write(UInt32 v1, UInt32 v2)
        {
            Write(v1, (byte)(v2 >> 24));
            Write(v1 + 1, (byte)(v2 >> 16));
            Write(v1 + 2, (byte)(v2 >> 8));
            Write(v1 + 3, (byte)(v2 >> 0));
        }

        public byte ReadByte(UInt32 v1)
        {
            return mm[v1];
        }

        public UInt16 ReadUInt16(UInt32 v1)
        {
            return (UInt16)((mm[v1] << 8) + (mm[v1 + 1] << 0));
        }

        public UInt32 ReadUInt32(UInt32 v1)
        {
            return (UInt32)(mm[v1] << 24)
            + (UInt32)(mm[v1 + 1] << 16)
            + (UInt32)(mm[v1 + 2] << 8)
            + (UInt32)(mm[v1 + 3] << 0);
        }
    }
}