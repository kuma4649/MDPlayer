using System;
using System.Collections.Generic;
using System.Text;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class Memory68
    {
        public byte[] mem = null;
        public List<memhook> hookList;

        public Memory68(uint size = 16 * 1024 * 1024)
        {
            mem = new byte[size];
            hookList = new List<memhook>();
        }

        public void PokeB(uint ptr, byte dat)
        {
#if DEBUG
            //if (ptr >= 0x00033D0F && ptr<= 0x00033D0F)
            //{
            //    ;
            //}
#endif
            uint adr = (uint)(ptr % mem.Length);
            if (CheckAndWriteHookAddressByte(adr, dat)) return;
            mem[adr] = dat;
        }

        public void PokeW(uint ptr, UInt16 dat)//BE
        {
#if DEBUG
            //if (ptr >= 0x00033D0F && ptr <= 0x00033D0F + 2)
            //{
            //    ;
            //}
#endif
            mem[(uint)ptr % mem.Length] = (byte)(dat >> 8);
            mem[((uint)ptr + 1) % mem.Length] = (byte)dat;
        }

        public void PokeL(uint ptr, UInt32 dat)//BE
        {
#if DEBUG
            //if (ptr >= 0x00033D0F && ptr <= 0x00033D0F + 4)
            //{
            //    ;
            //}
#endif
            mem[(uint)ptr % mem.Length] = (byte)(dat >> 24);
            mem[((uint)ptr + 1) % mem.Length] = (byte)(dat >> 16);
            mem[((uint)ptr + 2) % mem.Length] = (byte)(dat >> 8);
            mem[((uint)ptr + 3) % mem.Length] = (byte)dat;
        }

        public byte PeekB(uint ptr)
        {
            uint adr = (uint)(ptr % mem.Length);
            if (CheckAndReadHookAddressByte(adr, out byte m))
            {
                return m;
            }
            return mem[(uint)adr];
        }

        public UInt16 PeekW(uint ptr)//BE
        {
            uint adr1 = (uint)(ptr % mem.Length);
            uint adr2 = (uint)((ptr + 1) % mem.Length);
            if (CheckAndReadHookAddressWord(adr1, out ushort m))
            {
                return m;
            }
            return (UInt16)(
                (mem[(uint)adr1] << 8)
                + (mem[(uint)adr2]));
        }

        public UInt32 PeekL(uint ptr)//BE
        {
            uint adr1 = (uint)(ptr % mem.Length);
            uint adr2 = (uint)((ptr + 1) % mem.Length);
            uint adr3 = (uint)((ptr + 2) % mem.Length);
            uint adr4 = (uint)((ptr + 3) % mem.Length);
            if (CheckAndReadHookAddressLong(adr1, out uint m))
            {
                return m;
            }
            return (UInt32)(
                (mem[(uint)adr1] << 24)
                + (mem[(uint)adr2] << 16)
                + (mem[(uint)adr3] << 8)
                + (mem[(uint)adr4])
                );
        }

        public void SetHookAddress(uint startAdr, uint endAdr, Func<uint, byte, bool> write, Func<uint,uint> read)
        {
            hookList.Add(new memhook(startAdr, endAdr, read, write));

        }

        private bool CheckAndReadHookAddressByte(uint adr, out byte retVal)
        {
            foreach(var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.read == null) continue;
                    retVal = hook.ReadB(adr);
                    return true;
                }
            }

            retVal = 0;
            return false;
        }

        private bool CheckAndReadHookAddressWord(uint adr, out ushort retVal)
        {
            foreach (var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.read == null) continue;
                    retVal = hook.ReadW(adr);
                    return true;
                }
            }

            retVal = 0;
            return false;
        }

        private bool CheckAndReadHookAddressLong(uint adr, out uint retVal)
        {
            foreach (var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.read == null) continue;
                    retVal = hook.ReadL(adr);
                    return true;
                }
            }

            retVal = 0;
            return false;
        }

        private bool CheckAndWriteHookAddressByte(uint adr, byte val)
        {
            foreach (var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.write == null) continue;
                    return hook.write(adr,val);
                }
            }

            return false;
        }
    }
}
