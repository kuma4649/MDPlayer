using System;
using System.Collections.Generic;
using System.Text;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class Memory68
    {
        public byte[] mem = null;
        public List<memhook> hookList;
        private memhook cachedHook = null;
        private uint cachedAdr = uint.MaxValue;
        private uint addressMask;

        public Memory68(uint size = 16 * 1024 * 1024)
        {
            mem = new byte[size];
            hookList = new List<memhook>();
            addressMask = size - 1;  // 2^24 - 1 = 0xFFFFFF
        }

                 public void PokeB(uint ptr, byte dat)
                 {
        #if DEBUG
                     //if (ptr >= 0x00033D0F && ptr<= 0x00033D0F)
                     //{
                     //    ;
                     //}
        #endif
                     uint adr = ptr & addressMask;
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
                     mem[ptr & addressMask] = (byte)(dat >> 8);
                     mem[(ptr + 1) & addressMask] = (byte)dat;
                 }

                 public void PokeL(uint ptr, UInt32 dat)//BE
                 {
        #if DEBUG
                     //if (ptr >= 0x00033D0F && ptr <= 0x00033D0F + 4)
                     //{
                     //    ;
                     //}
        #endif
                     mem[ptr & addressMask] = (byte)(dat >> 24);
                     mem[(ptr + 1) & addressMask] = (byte)(dat >> 16);
                     mem[(ptr + 2) & addressMask] = (byte)(dat >> 8);
                     mem[(ptr + 3) & addressMask] = (byte)dat;
                 }

        public byte PeekB(uint ptr)
        {
            uint adr = ptr & addressMask;
            if (CheckAndReadHookAddressByte(adr, out byte m))
            {
                return m;
            }
            return mem[adr];
        }

        public UInt16 PeekW(uint ptr)//BE
        {
            uint adr1 = ptr & addressMask;
            uint adr2 = (ptr + 1) & addressMask;
            if (CheckAndReadHookAddressWord(adr1, out ushort m))
            {
                return m;
            }
            return (UInt16)(
                (mem[adr1] << 8)
                + (mem[adr2]));
        }

        public UInt32 PeekL(uint ptr)//BE
        {
            uint adr1 = ptr & addressMask;
            uint adr2 = (ptr + 1) & addressMask;
            uint adr3 = (ptr + 2) & addressMask;
            uint adr4 = (ptr + 3) & addressMask;
            if (CheckAndReadHookAddressLong(adr1, out uint m))
            {
                return m;
            }
            return (UInt32)(
                (mem[adr1] << 24)
                + (mem[adr2] << 16)
                + (mem[adr3] << 8)
                + (mem[adr4])
                );
        }

        public void SetHookAddress(uint startAdr, uint endAdr, Func<uint, byte, bool> write, Func<uint,uint> read)
        {
            hookList.Add(new memhook(startAdr, endAdr, read, write));
            // ソート化（startAdr でソート）
            hookList.Sort((a, b) => a.startAdr.CompareTo(b.startAdr));
        }

        private bool CheckAndReadHookAddressByte(uint adr, out byte retVal)
        {
            // キャッシュ確認
            if (cachedAdr == adr && cachedHook != null && cachedHook.startAdr <= adr && cachedHook.endAdr >= adr)
            {
                if (cachedHook.read != null)
                {
                    retVal = cachedHook.ReadB(adr);
                    return true;
                }
            }

            foreach (var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.read == null) continue;
                    cachedHook = hook;
                    cachedAdr = adr;
                    retVal = hook.ReadB(adr);
                    return true;
                }
            }

            retVal = 0;
            return false;
        }

        private bool CheckAndReadHookAddressWord(uint adr, out ushort retVal)
        {
            // キャッシュ確認
            if (cachedAdr == adr && cachedHook != null && cachedHook.startAdr <= adr && cachedHook.endAdr >= adr)
            {
                if (cachedHook.read != null)
                {
                    retVal = cachedHook.ReadW(adr);
                    return true;
                }
            }

            foreach (var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.read == null) continue;
                    cachedHook = hook;
                    cachedAdr = adr;
                    retVal = hook.ReadW(adr);
                    return true;
                }
            }

            retVal = 0;
            return false;
        }

        private bool CheckAndReadHookAddressLong(uint adr, out uint retVal)
        {
            // キャッシュ確認
            if (cachedAdr == adr && cachedHook != null && cachedHook.startAdr <= adr && cachedHook.endAdr >= adr)
            {
                if (cachedHook.read != null)
                {
                    retVal = cachedHook.ReadL(adr);
                    return true;
                }
            }

            foreach (var hook in hookList)
            {
                if (hook.startAdr <= adr && hook.endAdr >= adr)
                {
                    if (hook.read == null) continue;
                    cachedHook = hook;
                    cachedAdr = adr;
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
                             return hook.write(adr, val);
                         }
                     }

                     return false;
                 }
            }
        }
