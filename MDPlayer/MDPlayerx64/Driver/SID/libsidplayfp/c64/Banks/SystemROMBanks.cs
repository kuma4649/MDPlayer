/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2010 Antti Lankila
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

namespace Driver.libsidplayfp.c64.Banks
{
    /**
     * ROM bank base class.
     * N must be a power of two.
     */
    public class romBank : IBank
    {


        //# include <stdint.h>
        //# include <cstring>
        //# include "Bank.h"
        //# include "c64/CPU/opcodes.h"
        //# include "sidcxx11.h"

        //template<int N>
        private int N = 0x10000;//dummy
        public romBank(int N)
        {
            this.N = N;
            rom = new Ptr<byte>(N);
        }

        /// The ROM array
        protected Ptr<byte> rom;

        /**
         * Set value at memory address.
         */
        protected void setVal(UInt16 address, byte val)
        {
            rom[address & (N - 1)] = val;
        }

        /**
         * Return value from memory address.
         */
        protected byte getVal(UInt16 address)
        {
            return rom[address & (N - 1)];
        }

        /**
         * Return pointer to memory address.
         */
        protected Ptr<byte> getPtr(UInt16 address)
        {
            //TODO: kuma アドレス自体を返しているっぽい
            return new Ptr<byte>(rom.buf, rom.ptr + (address & (N - 1)));
        }

        /**
         * Copy content from source buffer.
         */
        public virtual void set(Ptr<byte> source)
        {
            if (source != null)
            {
                //memcpy(rom, source, N);
                Array.Copy(source.buf, source.ptr, rom.buf, rom.ptr, N);
            }
        }

        /**
         * Writing to ROM is a no-op.
         */
        public  void poke(UInt16 a, byte b) { }

        /**
         * Read from ROM.
         */
        public  byte peek(UInt16 address)
        {
            return rom[address & (N - 1)];
        }
    }

    /**
     * Kernal ROM
     *
     * Located at $E000-$FFFF
     */
    public sealed class KernalRomBank : romBank // public romBank<0x2000>
    {
        public KernalRomBank() : base(0x2000)
        {
        }

        private byte resetVectorLo;  // 0xfffc
        private byte resetVectorHi;  // 0xfffd

        public override void set(Ptr<byte> kernal)
        {
            //romBank < 0x2000 >::set(kernal);
            base.set(kernal);

            if (kernal == null)
            {
                // IRQ entry point
                setVal(0xffa0, CPU.opcodes.PHAn); // Save regs
                setVal(0xffa1, CPU.opcodes.TXAn);
                setVal(0xffa2, CPU.opcodes.PHAn);
                setVal(0xffa3, CPU.opcodes.TYAn);
                setVal(0xffa4, CPU.opcodes.PHAn);
                setVal(0xffa5, CPU.opcodes.JMPi); // Jump to IRQ routine
                setVal(0xffa6, 0x14);
                setVal(0xffa7, 0x03);

                // Halt
                setVal(0xea39, 0x02);

                // Hardware vectors
                setVal(0xfffa, 0x39); // NMI vector
                setVal(0xfffb, 0xea);
                setVal(0xfffc, 0x39); // RESET vector
                setVal(0xfffd, 0xea);
                setVal(0xfffe, 0xa0); // IRQ/BRK vector
                setVal(0xffff, 0xff);
            }

            // Backup Reset Vector
            resetVectorLo = getVal(0xfffc);
            resetVectorHi = getVal(0xfffd);
        }

        public void reset()
        {
            // Restore original Reset Vector
            setVal(0xfffc, resetVectorLo);
            setVal(0xfffd, resetVectorHi);
        }

        /**
         * Change the RESET vector.
         *
         * @param addr the new addres to point to
         */
        public void installResetHook(UInt16 addr)
        {
            setVal(0xfffc, sidendian.endian_16lo8(addr));
            setVal(0xfffd, sidendian.endian_16hi8(addr));
        }
    }

    /**
     * BASIC ROM
     *
     * Located at $A000-$BFFF
     */
    public sealed class BasicRomBank : romBank //<0x2000>
    {
        public BasicRomBank() : base(0x2000)
        {
        }

        private byte[] trap = new byte[3];
        private byte[] subTune = new byte[11];

        public override void set(Ptr<byte> basic)
        {
            //romBank < 0x2000 >::set(basic);
            base.set(basic);

            // Backup BASIC Warm Start
            //memcpy(trap, getPtr(0xa7ae), sizeof(trap));
            for (int i = 0; i < trap.Length; i++) trap[i] = getVal((UInt16)(0xa7ae + i));

            //memcpy(subTune, getPtr(0xbf53), sizeof(subTune));
            for (int i = 0; i < subTune.Length; i++) subTune[i] = getVal((UInt16)(0xbf53 + i));
        }

        public void reset()
        {
            // Restore original BASIC Warm Start
            //memcpy(getPtr(0xa7ae), trap, sizeof(trap));
            for (int i = 0; i < trap.Length; i++) setVal((UInt16)(0xa7ae + i), trap[i]);

            //memcpy(getPtr(0xbf53), subTune, sizeof(subTune));
            for (int i = 0; i < subTune.Length; i++) setVal((UInt16)(0xbf53 + i),  subTune[i]);
        }

        /**
         * Set BASIC Warm Start address.
         *
         * @param addr
         */
        public void installTrap(UInt16 addr)
        {
            setVal(0xa7ae, CPU.opcodes.JMPw);
            setVal(0xa7af, sidendian.endian_16lo8(addr));
            setVal(0xa7b0, sidendian.endian_16hi8(addr));
        }

        public void setSubtune(byte tune)
        {
            setVal(0xbf53, CPU.opcodes.LDAb);
            setVal(0xbf54, tune);
            setVal(0xbf55, CPU.opcodes.STAa);
            setVal(0xbf56, 0x0c);
            setVal(0xbf57, 0x03);
            setVal(0xbf58, CPU.opcodes.JSRw);
            setVal(0xbf59, 0x2c);
            setVal(0xbf5a, 0xa8);
            setVal(0xbf5b, CPU.opcodes.JMPw);
            setVal(0xbf5c, 0xb1);
            setVal(0xbf5d, 0xa7);
        }
    }

    /**
     * Character ROM
     *
     * Located at $D000-$DFFF
     */
    public sealed class CharacterRomBank : romBank//<0x1000>
    {
        public CharacterRomBank() : base(0x1000)
        {
        }
    }




}
