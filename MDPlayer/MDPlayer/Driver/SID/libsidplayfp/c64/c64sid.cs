/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2013-2015 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.c64
{
    /**
     * SID interface.
     */
    public class c64sid : Banks.IBank
    {



        //# include "Banks/Bank.h"
        //# include "sidcxx11.h"
        //# include <stdint.h>

        ~c64sid() { }

        public virtual byte read(byte addr) { return 0; }
        public virtual void write(byte addr, byte data) { }

        public virtual void reset(byte volume) { }

        public void reset()
        {
            reset(0);
        }

        // Bank functions
        public void poke(UInt16 address, byte value)
        {
            write((byte)(address & 0x1f), value);
        }

        public byte peek(UInt16 address)
        {
            return read((byte)(address & 0x1f));
        }





    }
}