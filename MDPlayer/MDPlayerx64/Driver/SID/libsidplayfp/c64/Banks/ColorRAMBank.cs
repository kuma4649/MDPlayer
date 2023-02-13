/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.c64.Banks
{
    /**
     * Color RAM.
     *
     * 1K x 4-bit Static RAM that stores text screen color information.
     *
     * Located at $D800-$DBFF (last 24 bytes are unused)
     */
    public sealed class ColorRAMBank : IBank
    {




        //# include <stdint.h>
        //# include <cstring>
        //# include "Bank.h"
        //# include "sidcxx11.h"

        private byte[] ram = new byte[0x400];

        public void reset()
        {
            for (int i = 0; i < ram.Length; i++) ram[i] = 0;
        }

        public  void poke(UInt16 address, byte value)
        {
            ram[address & 0x3ff] = (byte)(value & 0xf);
        }

        public  byte peek(UInt16 address)
        {
            return ram[address & 0x3ff];
        }




    }
}
