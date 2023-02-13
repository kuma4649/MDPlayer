using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver
{
    public static class mem
    {
        public static void memset(ref byte[] des, byte val, int length)
        {
            for (int i = 0; i < length; i++) des[i] = val;
        }

        public static void memset(ref Ptr<byte> des, byte val, int length)
        {
            for (int i = 0; i < length; i++) des[i] = val;
        }

        public static void memcpy(ref byte[] des, byte[] src, int length)
        {
            for (int i = 0; i < length; i++) des[i] = src[i];
        }

        public static void memcpy(ref byte[] des, Ptr<byte> src, int length)
        {
            for (int i = 0; i < length; i++) des[i] = src[i];
        }

        public static void memcpy(ref Ptr<byte> des, byte[] src, int length)
        {
            for (int i = 0; i < length; i++) des[i] = src[i];
        }

        public static void memcpy(ref Ptr<byte> des, Ptr<byte> src, int length)
        {
            for (int i = 0; i < length; i++) des[i] = src[i];
        }

        public static int memcmp(byte[] srcA, byte[] srcB, int len)
        {
            for (int i = 0; i < len; i++)
            {
                int n = srcA[i] - srcB[i];
                if (n != 0) { return n; }
            }

            return 0;
        }

        public static int memcmp(byte[] srcA, int indA, byte[] srcB, int indB, int len)
        {
            for (int i = 0; i < len; i++)
            {
                int n = srcA[i + indA] - srcB[i + indB];
                if (n != 0) { return n; }
            }

            return 0;
        }

    }
}
