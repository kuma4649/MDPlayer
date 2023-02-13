/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2000 Simon White
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp
{
    public class sidendian
    {

//#ifdef HAVE_CONFIG_H
//#  include "config.h"
//#endif

//#include <stdint.h>

        /*
        Labeling:
        0 - LO
        1 - HI
        2 - HILO
        3 - HIHI
        */

        ///////////////////////////////////////////////////////////////////
        // INT16 FUNCTIONS
        ///////////////////////////////////////////////////////////////////
        // Set the lo byte (8 bit) in a word (16 bit)
        public static void endian_16lo8(ref UInt16 word, byte _byte)
        {
            word &= 0xff00;
            word |= _byte;
        }

        // Get the lo byte (8 bit) in a word (16 bit)
        public static byte endian_16lo8(UInt16 word)
        {
            return (byte)word;
        }

        // Set the hi byte (8 bit) in a word (16 bit)
        public static void endian_16hi8(ref UInt16 word, byte _byte)
        {
            word &= 0x00ff;
            word |= (UInt16)(_byte << 8);
        }

        // Set the hi byte (8 bit) in a word (16 bit)
        public static byte endian_16hi8(UInt16 word)
        {
            return (byte)(word >> 8);
        }

        // Swap word endian.
        public static void endian_16swap8(ref UInt16 word)
        {
            byte lo = endian_16lo8(word);
            byte hi = endian_16hi8(word);
            endian_16lo8(ref word, hi);
            endian_16hi8(ref word, lo);
        }

        // Convert high-byte and low-byte to 16-bit word.
        public static UInt16 endian_16(byte hi, byte lo)
        {
            UInt16 word = 0;
            endian_16lo8(ref word, lo);
            endian_16hi8(ref word, hi);
            return word;
        }

        // Convert high-byte and low-byte to 16-bit little endian word.
        public static void endian_16(byte[] ptr, UInt16 word)
        {
#if SID_WORDS_BIGENDIAN
            ptr[0] = endian_16hi8 (word);
            ptr[1] = endian_16lo8 (word);
#else
            ptr[0] = endian_16lo8(word);
            ptr[1] = endian_16hi8(word);
#endif
        }

        //ポインター対策版
        public static void endian_16(Ptr<byte> buf, UInt16 word)
        {
#if SID_WORDS_BIGENDIAN
            buf[0] = endian_16hi8 (word);
            buf[1] = endian_16lo8 (word);
#else
            buf[0] = endian_16lo8(word);
            buf[1] = endian_16hi8(word);
#endif
        }

        public static void endian_16(sbyte[] ptr, UInt16 word)
        {
            //endian_16((uint8_t*)ptr, word);
#if SID_WORDS_BIGENDIAN
            ptr[0] = endian_16hi8 (word);
            ptr[1] = endian_16lo8 (word);
#else
            ptr[0] = (sbyte)endian_16lo8(word);
            ptr[1] = (sbyte)endian_16hi8(word);
#endif
        }

        //ポインター対策版
        public static void endian_16(Ptr<sbyte> ptr, UInt16 word)
        {
            //endian_16((uint8_t*)ptr, word);
#if SID_WORDS_BIGENDIAN
            ptr[0] = endian_16hi8 (word);
            ptr[1] = endian_16lo8 (word);
#else
            ptr[0] = (sbyte)endian_16lo8(word);
            ptr[1] = (sbyte)endian_16hi8(word);
#endif
        }

        // Convert high-byte and low-byte to 16-bit little endian word.
        public static UInt16 endian_little16(byte[] ptr)
        {
            return endian_16(ptr[1], ptr[0]);
        }

        //ポインター対策版
        public static UInt16 endian_little16(Ptr<byte> ptr)
        {
            return endian_16(ptr[1], ptr[0]);
        }

        // Write a little-endian 16-bit word to two bytes in memory.
        public static void endian_little16(byte[] ptr, UInt16 word)
        {
            ptr[0] = endian_16lo8(word);
            ptr[1] = endian_16hi8(word);
        }

        //ポインター対策版
        public static void endian_little16(Ptr<byte> ptr, UInt16 word)
        {
            ptr[0] = endian_16lo8(word);
            ptr[1] = endian_16hi8(word);
        }

        // Convert high-byte and low-byte to 16-bit big endian word.
        public static UInt16 endian_big16(byte[] ptr)
        {
            return endian_16(ptr[0], ptr[1]);
        }

        //ポインター対策版
        public static UInt16 endian_big16(Ptr<byte> ptr)
        {
            return endian_16(ptr[0], ptr[1]);
        }

        // Write a little-big 16-bit word to two bytes in memory.
        public static void endian_big16(byte[] ptr, UInt16 word)
        {
            ptr[0] = endian_16hi8(word);
            ptr[1] = endian_16lo8(word);
        }

        //ポインター対策版
        public static void endian_big16(Ptr<byte> ptr, UInt16 word)
        {
            ptr[0] = endian_16hi8(word);
            ptr[1] = endian_16lo8(word);
        }


        ///////////////////////////////////////////////////////////////////
        // INT32 FUNCTIONS
        ///////////////////////////////////////////////////////////////////
        // Set the lo word (16bit) in a dword (32 bit)
        public static void endian_32lo16(ref UInt32 dword, UInt16 word)
        {
            dword &= (UInt32)0xffff0000;
            dword |= word;
        }

        // Get the lo word (16bit) in a dword (32 bit)
        public static UInt16 endian_32lo16(UInt32 dword)
        {
            return (UInt16)(dword & 0xffff);
        }

        // Set the hi word (16bit) in a dword (32 bit)
        public static void endian_32hi16(ref UInt32 dword, UInt16 word)
        {
            dword &= (UInt32)0x0000ffff;
            dword |= (UInt32)(word << 16);
            //#endif
        }

        // Get the hi word (16bit) in a dword (32 bit)
        public static UInt16 endian_32hi16(UInt32 dword)
        {
            return (UInt16)(dword >> 16);
        }

        // Set the lo byte (8 bit) in a dword (32 bit)
        public static void endian_32lo8(ref UInt32 dword, byte _byte)
        {
            dword &= (UInt32)0xffffff00;
            dword |= (UInt32)_byte;
        }

        // Get the lo byte (8 bit) in a dword (32 bit)
        public static byte endian_32lo8(UInt32 dword)
        {
            return (byte)dword;
        }

        // Set the hi byte (8 bit) in a dword (32 bit)
        public static void endian_32hi8(ref UInt32 dword, byte _byte)
        {
            dword &= (UInt32)0xffff00ff;
            dword |= (UInt32)(_byte << 8);
        }

        // Get the hi byte (8 bit) in a dword (32 bit)
        public static byte endian_32hi8(UInt32 dword)
        {
            return (byte)(dword >> 8);
        }

        // Swap hi and lo words endian in 32 bit dword.
        public static void endian_32swap16(ref UInt32 dword)
        {
            UInt16 lo = endian_32lo16(dword);
            UInt16 hi = endian_32hi16(dword);
            endian_32lo16(ref dword, hi);
            endian_32hi16(ref dword, lo);
        }

        // Swap word endian.
        public static void endian_32swap8(ref UInt32 dword)
        {
            UInt16 lo, hi;
            lo = endian_32lo16(dword);
            hi = endian_32hi16(dword);
            endian_16swap8(ref lo);
            endian_16swap8(ref hi);
            endian_32lo16(ref dword, hi);
            endian_32hi16(ref dword, lo);
        }

        // Convert high-byte and low-byte to 32-bit word.
        public static UInt32 endian_32(byte hihi, byte hilo, byte hi, byte lo)
        {
            UInt32 dword = 0;
            UInt16 word = 0;
            endian_32lo8(ref dword, lo);
            endian_32hi8(ref dword, hi);
            endian_16lo8(ref word, hilo);
            endian_16hi8(ref word, hihi);
            endian_32hi16(ref dword, word);
            return dword;
        }

        // Convert high-byte and low-byte to 32-bit little endian word.
        public static UInt32 endian_little32(byte[] ptr)
        {
            return endian_32(ptr[3], ptr[2], ptr[1], ptr[0]);
        }

        //ポインター対策版
        public static UInt32 endian_little32(Ptr<byte> ptr)
        {
            return endian_32(ptr[3], ptr[2], ptr[1], ptr[0]);
        }

        // Write a little-endian 32-bit word to four bytes in memory.
        public static void endian_little32(byte[] ptr, UInt32 dword)
        {
            UInt16 word;
            ptr[0] = endian_32lo8(dword);
            ptr[1] = endian_32hi8(dword);
            word = endian_32hi16(dword);
            ptr[2] = endian_16lo8(word);
            ptr[3] = endian_16hi8(word);
        }

        //ポインター対策版
        public static void endian_little32(Ptr<byte> ptr, UInt32 dword)
        {
            UInt16 word;
            ptr[0] = endian_32lo8(dword);
            ptr[1] = endian_32hi8(dword);
            word = endian_32hi16(dword);
            ptr[2] = endian_16lo8(word);
            ptr[3] = endian_16hi8(word);
        }

        // Convert high-byte and low-byte to 32-bit big endian word.
        public static UInt32 endian_big32(byte[] ptr)
        {
            return endian_32(ptr[0], ptr[1], ptr[2], ptr[3]);
        }

        //ポインター対策版
        public static UInt32 endian_big32(Ptr<byte> ptr)
        {
            return endian_32(ptr[0], ptr[1], ptr[2], ptr[3]);
        }

        // Write a big-endian 32-bit word to four bytes in memory.
        public static void endian_big32(byte[] ptr, UInt32 dword)
        {
            UInt16 word;
            word = endian_32hi16(dword);
            ptr[1] = endian_16lo8(word);
            ptr[0] = endian_16hi8(word);
            ptr[2] = endian_32hi8(dword);
            ptr[3] = endian_32lo8(dword);
        }

        //ポインター対策版
        public static void endian_big32(Ptr<byte> ptr, UInt32 dword)
        {
            UInt16 word;
            word = endian_32hi16(dword);
            ptr[1] = endian_16lo8(word);
            ptr[0] = endian_16hi8(word);
            ptr[2] = endian_32hi8(dword);
            ptr[3] = endian_32lo8(dword);
        }

    }
}
