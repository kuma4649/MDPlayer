/*
 * This code has been derived by Michael Schwendt <mschwendt@yahoo.com>
 * from original work by L. Peter Deutsch <ghost@aladdin.com>.
 *
 * The original C code (md5.c, md5.h) is available here:
 * ftp://ftp.cs.wisc.edu/ghost/packages/md5.tar.gz
 */

/*
  Copyright (C) 1999 Aladdin Enterprises.  All rights reserved.

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.

  L. Peter Deutsch
  ghost@aladdin.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.utils.MD5
{
    public class MD5
    {




        //# include <stdint.h>
        //# include "MD5_Defs.h"

        //typedef uint8_t md5_byte_t;
        //typedef uint32_t md5_word_t;

        // Initialize the algorithm. Reset starting values.
        //public MD5() { }

        // Append a string to the message.
        //public void append(object data, Int32 nbytes) { }

        // Finish the message.
        //public void finish() { }

        // Return pointer to 16-byte fingerprint.
        //public byte[] getDigest() { return null; }

        // Initialize the algorithm. Reset starting values.
        //public void reset() { }



        /* Define the state of the MD5 Algorithm. */
        private UInt32[] count = new UInt32[2];    /* message length in bits, lsw first */
        private UInt32[] abcd = new UInt32[4];     /* digest buffer */
        private byte[] buf = new byte[64];     /* accumulate block */

        private byte[] digest = new byte[16];

        private UInt32[] tmpBuf = new UInt32[16];
        private UInt32[] X;

        //private void process(byte[] data) { }//[64]);

        //private UInt32 ROTATE_LEFT(UInt32 x, int n) { return 0; }

        //private UInt32 F( UInt32 x,  UInt32 y,  UInt32 z) { return 0; }

        //private UInt32 G(UInt32 x, UInt32 y, UInt32 z) { return 0; }

        //private UInt32 H(UInt32 x, UInt32 y, UInt32 z) { return 0; }

        //private UInt32 I(UInt32 x, UInt32 y, UInt32 z) { return 0; }

        //  typedef UInt32(MD5::* md5func)( UInt32 x,  UInt32 y,  UInt32 z);
        public delegate UInt32 md5func(UInt32 x, UInt32 y, UInt32 z);

        //private void SET(md5func func, ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, int k, int s, UInt32 Ti) { }

        private UInt32 ROTATE_LEFT(UInt32 x, int n)
        {
            return ((x << n) | (x >> (32 - n)));
        }

        private UInt32 F(UInt32 x, UInt32 y, UInt32 z)
        {
            return ((x & y) | (~x & z));
        }

        private UInt32 G(UInt32 x, UInt32 y, UInt32 z)
        {
            return ((x & z) | (y & ~z));
        }

        private UInt32 H(UInt32 x, UInt32 y, UInt32 z)
        {
            return (x ^ y ^ z);
        }

        private UInt32 I(UInt32 x, UInt32 y, UInt32 z)
        {
            return (y ^ (x | ~z));
        }

        private void SET(md5func func, ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, int k, int s, UInt32 Ti)
        {
            UInt32 t = a + func(b, c, d) + X[k] + Ti;
            a = ROTATE_LEFT(t, s) + b;
        }





        /*
 * This code has been derived by Michael Schwendt <mschwendt@yahoo.com>
 * from original work by L. Peter Deutsch <ghost@aladdin.com>.
 *
 * The original C code (md5.c, md5.h) is available here:
 * ftp://ftp.cs.wisc.edu/ghost/packages/md5.tar.gz
 */

        /*
         * The original code is:

          Copyright (C) 1999 Aladdin Enterprises.  All rights reserved.

          This software is provided 'as-is', without any express or implied
          warranty.  In no event will the authors be held liable for any damages
          arising from the use of this software.

          Permission is granted to anyone to use this software for any purpose,
          including commercial applications, and to alter it and redistribute it
          freely, subject to the following restrictions:

          1. The origin of this software must not be misrepresented; you must not
             claim that you wrote the original software. If you use this software
             in a product, an acknowledgment in the product documentation would be
             appreciated but is not required.
          2. Altered source versions must be plainly marked as such, and must not be
             misrepresented as being the original software.
          3. This notice may not be removed or altered from any source distribution.

          L. Peter Deutsch
          ghost@aladdin.com

         */

        //# include "MD5.h"
        //# include <string.h>
        //# ifdef HAVE_CONFIG_H
        //# include "config.h"
        //#endif

        //#if defined(HAVE_MSWINDOWS) || defined(DLL_EXPORT)
        //// Support for DLLs
        //#define MD5_EXPORT __declspec(dllexport)
        //#endif

        /*
         * Compile with -DMD5_TEST to create a self-contained executable test program.
         * The test program should print out the same values as given in section
         * A.5 of RFC 1321, reproduced below.
         */

        //# include <iostream.h>
        //# include <iomanip.h>

        public int main()
        {
            string[] strTest = new string[7] {
    "", /*d41d8cd98f00b204e9800998ecf8427e*/
	"a", /*0cc175b9c0f1b6a831c399e269772661*/
	"abc", /*900150983cd24fb0d6963f7d28e17f72*/
	"message digest", /*f96b697d7cb7938d525a2f31aaf161d0*/
	"abcdefghijklmnopqrstuvwxyz", /*c3fcd3d76192e4007dfb496cca67e13b*/
	"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
				/*d174ab98d277d9f5a5611c2c9f419d9f*/
	"12345678901234567890123456789012345678901234567890123456789012345678901234567890" /*57edf4a22be3c955ac49da2e2107b67a*/
    };
            byte[][] test = new byte[7][];
            for (int i = 0; i < 7; i++) test[i] = Encoding.ASCII.GetBytes(strTest[i]);

            for (int i = 0; i < 7; ++i)
            {
                MD5 myMD5 = new MD5();
                myMD5.append((byte[])test[i], test[i].Length);
                myMD5.finish();
                //Console.Write("MD5 (\"" + test[i] + "\") = ");
                //for (int di = 0; di < 16; ++di)
                    //Console.Write("{0:X02}", (int)(myMD5.getDigest()[di]));
                //Console.WriteLine("");
            }
            return 0;
        }
        //#endif  /* MD5_TEST */

        private const UInt32 T1 = 0xd76aa478;
        private const UInt32 T2 = 0xe8c7b756;
        private const UInt32 T3 = 0x242070db;
        private const UInt32 T4 = 0xc1bdceee;
        private const UInt32 T5 = 0xf57c0faf;
        private const UInt32 T6 = 0x4787c62a;
        private const UInt32 T7 = 0xa8304613;
        private const UInt32 T8 = 0xfd469501;
        private const UInt32 T9 = 0x698098d8;
        private const UInt32 T10 = 0x8b44f7af;
        private const UInt32 T11 = 0xffff5bb1;
        private const UInt32 T12 = 0x895cd7be;
        private const UInt32 T13 = 0x6b901122;
        private const UInt32 T14 = 0xfd987193;
        private const UInt32 T15 = 0xa679438e;
        private const UInt32 T16 = 0x49b40821;
        private const UInt32 T17 = 0xf61e2562;
        private const UInt32 T18 = 0xc040b340;
        private const UInt32 T19 = 0x265e5a51;
        private const UInt32 T20 = 0xe9b6c7aa;
        private const UInt32 T21 = 0xd62f105d;
        private const UInt32 T22 = 0x02441453;
        private const UInt32 T23 = 0xd8a1e681;
        private const UInt32 T24 = 0xe7d3fbc8;
        private const UInt32 T25 = 0x21e1cde6;
        private const UInt32 T26 = 0xc33707d6;
        private const UInt32 T27 = 0xf4d50d87;
        private const UInt32 T28 = 0x455a14ed;
        private const UInt32 T29 = 0xa9e3e905;
        private const UInt32 T30 = 0xfcefa3f8;
        private const UInt32 T31 = 0x676f02d9;
        private const UInt32 T32 = 0x8d2a4c8a;
        private const UInt32 T33 = 0xfffa3942;
        private const UInt32 T34 = 0x8771f681;
        private const UInt32 T35 = 0x6d9d6122;
        private const UInt32 T36 = 0xfde5380c;
        private const UInt32 T37 = 0xa4beea44;
        private const UInt32 T38 = 0x4bdecfa9;
        private const UInt32 T39 = 0xf6bb4b60;
        private const UInt32 T40 = 0xbebfbc70;
        private const UInt32 T41 = 0x289b7ec6;
        private const UInt32 T42 = 0xeaa127fa;
        private const UInt32 T43 = 0xd4ef3085;
        private const UInt32 T44 = 0x04881d05;
        private const UInt32 T45 = 0xd9d4d039;
        private const UInt32 T46 = 0xe6db99e5;
        private const UInt32 T47 = 0x1fa27cf8;
        private const UInt32 T48 = 0xc4ac5665;
        private const UInt32 T49 = 0xf4292244;
        private const UInt32 T50 = 0x432aff97;
        private const UInt32 T51 = 0xab9423a7;
        private const UInt32 T52 = 0xfc93a039;
        private const UInt32 T53 = 0x655b59c3;
        private const UInt32 T54 = 0x8f0ccc92;
        private const UInt32 T55 = 0xffeff47d;
        private const UInt32 T56 = 0x85845dd1;
        private const UInt32 T57 = 0x6fa87e4f;
        private const UInt32 T58 = 0xfe2ce6e0;
        private const UInt32 T59 = 0xa3014314;
        private const UInt32 T60 = 0x4e0811a1;
        private const UInt32 T61 = 0xf7537e82;
        private const UInt32 T62 = 0xbd3af235;
        private const UInt32 T63 = 0x2ad7d2bb;
        private const UInt32 T64 = 0xeb86d391;

        public MD5()
        {
            reset();
        }

        public void reset()
        {
            count[0] = count[1] = 0;
            abcd[0] = 0x67452301;
            abcd[1] = 0xefcdab89;
            abcd[2] = 0x98badcfe;
            abcd[3] = 0x10325476;
            mem.memset(ref digest, 0, 16);
            mem.memset(ref buf, 0, 64);
        }

        private void process(Ptr<byte> data)//[64])
        {
            UInt32 a = abcd[0], b = abcd[1], c = abcd[2], d = abcd[3];

#if MD5_WORDS_BIG_ENDIAN

    /*
     * On big-endian machines, we must arrange the bytes in the right
     * order.  (This also works on machines of unknown byte order.)
     */
    Ptr<byte> xp =new Ptr<byte>(data,0);
            for (int i = 0; i < 16; ++i, xp.AddPtr(4))
            {
                tmpBuf[i] = (UInt32)((xp[0] & 0xFF) + ((xp[1] & 0xFF) << 8) +
                            ((xp[2] & 0xFF) << 16) + ((xp[3] & 0xFF) << 24));
            }
    X = tmpBuf;

#else  
            /* !MD5_IS_BIG_ENDIAN */

            /*
             * On little-endian machines, we can process properly aligned data
             * without copying it.
             */
            //if (((data - (byte[])0) & 3) == 0)
            if ((data[0] & 3) == 0)

            {
                /* data are properly aligned */
                X = new uint[data.buf.Length / 4];
                for (int i = 0; i < 64 / 4; i++)
                {
                    X[i] = (UInt32)((data[3 + i * 4] & 0xFF) + ((data[2 + i * 4] & 0xFF) << 8) +
                                ((data[1 + i * 4] & 0xFF) << 16) + ((data[0 + i * 4] & 0xFF) << 24));
                }
            }
            else
            {
                /* not aligned */
                //mem.memcpy(ref tmpBuf, data, 64);
                //tmpBuf = new uint[data.buf.Length / 4];
                for (int i = 0; i < 64 / 4; i++)
                {
                    tmpBuf[i] = (UInt32)((data[3 + i * 4] & 0xFF) + ((data[2 + i * 4] & 0xFF) << 8) +
                                ((data[1 + i * 4] & 0xFF) << 16) + ((data[0 + i * 4] & 0xFF) << 24));
                }
                X = tmpBuf;
            }
#endif

            /* Round 1. */
            /* Let [abcd k s i] denote the operation
               a = b + ((a + F(b,c,d) + X[k] + T[i]) <<< s). */
            /* Do the following 16 operations. */
            SET(F, ref a, ref b, ref c, ref d, 0, 7, T1);
            SET(F, ref d, ref a, ref b, ref c, 1, 12, T2);
            SET(F, ref c, ref d, ref a, ref b, 2, 17, T3);
            SET(F, ref b, ref c, ref d, ref a, 3, 22, T4);
            SET(F, ref a, ref b, ref c, ref d, 4, 7, T5);
            SET(F, ref d, ref a, ref b, ref c, 5, 12, T6);
            SET(F, ref c, ref d, ref a, ref b, 6, 17, T7);
            SET(F, ref b, ref c, ref d, ref a, 7, 22, T8);
            SET(F, ref a, ref b, ref c, ref d, 8, 7, T9);
            SET(F, ref d, ref a, ref b, ref c, 9, 12, T10);
            SET(F, ref c, ref d, ref a, ref b, 10, 17, T11);
            SET(F, ref b, ref c, ref d, ref a, 11, 22, T12);
            SET(F, ref a, ref b, ref c, ref d, 12, 7, T13);
            SET(F, ref d, ref a, ref b, ref c, 13, 12, T14);
            SET(F, ref c, ref d, ref a, ref b, 14, 17, T15);
            SET(F, ref b, ref c, ref d, ref a, 15, 22, T16);

            /* Round 2. */
            /* Let [abcd k s i] denote the operation
                 a = b + ((a + G(b,c,d) + X[k] + T[i]) <<< s). */
            /* Do the following 16 operations. */
            SET(G, ref a, ref b, ref c, ref d, 1, 5, T17);
            SET(G, ref d, ref a, ref b, ref c, 6, 9, T18);
            SET(G, ref c, ref d, ref a, ref b, 11, 14, T19);
            SET(G, ref b, ref c, ref d, ref a, 0, 20, T20);
            SET(G, ref a, ref b, ref c, ref d, 5, 5, T21);
            SET(G, ref d, ref a, ref b, ref c, 10, 9, T22);
            SET(G, ref c, ref d, ref a, ref b, 15, 14, T23);
            SET(G, ref b, ref c, ref d, ref a, 4, 20, T24);
            SET(G, ref a, ref b, ref c, ref d, 9, 5, T25);
            SET(G, ref d, ref a, ref b, ref c, 14, 9, T26);
            SET(G, ref c, ref d, ref a, ref b, 3, 14, T27);
            SET(G, ref b, ref c, ref d, ref a, 8, 20, T28);
            SET(G, ref a, ref b, ref c, ref d, 13, 5, T29);
            SET(G, ref d, ref a, ref b, ref c, 2, 9, T30);
            SET(G, ref c, ref d, ref a, ref b, 7, 14, T31);
            SET(G, ref b, ref c, ref d, ref a, 12, 20, T32);

            /* Round 3. */
            /* Let [abcd k s t] denote the operation
                 a = b + ((a + H(b,c,d) + X[k] + T[i]) <<< s). */
            /* Do the following 16 operations. */
            SET(H, ref a, ref b, ref c, ref d, 5, 4, T33);
            SET(H, ref d, ref a, ref b, ref c, 8, 11, T34);
            SET(H, ref c, ref d, ref a, ref b, 11, 16, T35);
            SET(H, ref b, ref c, ref d, ref a, 14, 23, T36);
            SET(H, ref a, ref b, ref c, ref d, 1, 4, T37);
            SET(H, ref d, ref a, ref b, ref c, 4, 11, T38);
            SET(H, ref c, ref d, ref a, ref b, 7, 16, T39);
            SET(H, ref b, ref c, ref d, ref a, 10, 23, T40);
            SET(H, ref a, ref b, ref c, ref d, 13, 4, T41);
            SET(H, ref d, ref a, ref b, ref c, 0, 11, T42);
            SET(H, ref c, ref d, ref a, ref b, 3, 16, T43);
            SET(H, ref b, ref c, ref d, ref a, 6, 23, T44);
            SET(H, ref a, ref b, ref c, ref d, 9, 4, T45);
            SET(H, ref d, ref a, ref b, ref c, 12, 11, T46);
            SET(H, ref c, ref d, ref a, ref b, 15, 16, T47);
            SET(H, ref b, ref c, ref d, ref a, 2, 23, T48);

            /* Round 4. */
            /* Let [abcd k s t] denote the operation
                 a = b + ((a + I(b,c,d) + X[k] + T[i]) <<< s). */
            /* Do the following 16 operations. */
            SET(I, ref a, ref b, ref c, ref d, 0, 6, T49);
            SET(I, ref d, ref a, ref b, ref c, 7, 10, T50);
            SET(I, ref c, ref d, ref a, ref b, 14, 15, T51);
            SET(I, ref b, ref c, ref d, ref a, 5, 21, T52);
            SET(I, ref a, ref b, ref c, ref d, 12, 6, T53);
            SET(I, ref d, ref a, ref b, ref c, 3, 10, T54);
            SET(I, ref c, ref d, ref a, ref b, 10, 15, T55);
            SET(I, ref b, ref c, ref d, ref a, 1, 21, T56);
            SET(I, ref a, ref b, ref c, ref d, 8, 6, T57);
            SET(I, ref d, ref a, ref b, ref c, 15, 10, T58);
            SET(I, ref c, ref d, ref a, ref b, 6, 15, T59);
            SET(I, ref b, ref c, ref d, ref a, 13, 21, T60);
            SET(I, ref a, ref b, ref c, ref d, 4, 6, T61);
            SET(I, ref d, ref a, ref b, ref c, 11, 10, T62);
            SET(I, ref c, ref d, ref a, ref b, 2, 15, T63);
            SET(I, ref b, ref c, ref d, ref a, 9, 21, T64);

            /* Then perform the following additions. (That is increment each
               of the four registers by the value it had before this block
               was started.) */
            abcd[0] += a;
            abcd[1] += b;
            abcd[2] += c;
            abcd[3] += d;
        }

        public void append(object data, Int32 nbytes)
        {
            Ptr<byte> p = new Ptr<byte>((byte[])data, 0);
            int left = nbytes;
            int offset = (Int32)((count[0] >> 3) & 63);
            UInt32 nbits = (UInt32)(nbytes << 3);

            if (nbytes <= 0)
                return;

            /* Update the message length. */
            count[1] += (UInt32)(nbytes >> 29);
            count[0] += nbits;
            if (count[0] < nbits)
                count[1]++;

            /* Process an initial partial block. */
            Ptr<byte> pBuf = new Ptr<byte>(buf,0);
            if (offset != 0)
            {
                int copy = (offset + nbytes > 64) ? (64 - offset) : nbytes;
                pBuf.AddPtr(offset);
                mem.memcpy(ref pBuf, p, copy);
                pBuf.AddPtr(-offset);
                if (offset + copy < 64)
                    return;
                p .AddPtr(copy);
                left -= copy;
                process(pBuf);
            }

            /* Process full blocks. */
            for (; left >= 64; p.AddPtr(64), left -= 64)
                process(p);

            /* Process a final partial block. */
            if (left != 0)
                mem.memcpy(ref pBuf, p, left);
        }

        public void finish()
        {
            byte[] pad = new byte[64] {
        0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
           0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
           0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
           0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };
            byte[] data = new byte[8];
            int i;
            /* Save the length before padding. */
            for (i = 0; i < 8; ++i)
                data[i] = (byte)(count[i >> 2] >> ((i & 3) << 3));
            /* Pad to 56 bytes mod 64. */
            append(pad, (Int32)(((55 - (count[0] >> 3)) & 63) + 1));
            /* Append the length. */
            append(data, 8);
            for (i = 0; i < 16; ++i)
                digest[i] = (byte)(abcd[i >> 2] >> ((i & 3) << 3));
        }

        public byte[] getDigest()
        {
            return digest;
        }

    }
}