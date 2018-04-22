/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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
    /**
     * An interface that allows access to c64 memory
     * for loading tunes and apply sid specific hacks.
     */
    public class sidmemory
    {




        //# include <stdint.h>

        /**
         * Read one byte from memory.
         *
         * @param addr the memory location from which to read from
         */
        public virtual byte readMemByte(UInt16 addr) { return 0; }

        /**
         * Read two contiguous bytes from memory.
         *
         * @param addr the memory location from which to read from
         */
        public virtual UInt16 readMemWord(UInt16 addr) { return 0; }

        /**
         * Write one byte to memory.
         *
         * @param addr the memory location where to write
         * @param value the value to write
         */
        public virtual void writeMemByte(UInt16 addr, byte value) { }

        /**
         * Write two contiguous bytes to memory.
         *
         * @param addr the memory location where to write
         * @param value the value to write
         */
        public virtual void writeMemWord(UInt16 addr, UInt16 value) { }

        /**
         * Fill ram area with a constant value.
         *
         * @param start the start of memory location where to write
         * @param value the value to write
         * @param size the number of bytes to fill
         */
        public virtual void fillRam(UInt16 start, byte value, UInt32 size) { }
        public virtual void fillRam(UInt16 start, Ptr<byte> value, UInt32 size) { }

        /**
         * Copy a buffer into a ram area.
         *
         * @param start the start of memory location where to write
         * @param source the source buffer
         * @param size the number of bytes to copy
         */
        public virtual void fillRam(UInt16 start, byte[] source, UInt32 size) { }

        /**
         * Change the RESET vector.
         *
         * @param addr the new addres to point to
         */
        public virtual void installResetHook(UInt16 addr) { }

        /**
         * Set BASIC Warm Start address.
         *
         * @param addr the new addres to point to
         */
        public virtual void installBasicTrap(UInt16 addr) { }

        /**
         * Set the start tune.
         *
         * @param tune the tune number
         */
        public virtual void setBasicSubtune(byte tune) { }


        ~sidmemory() { }

    }
}