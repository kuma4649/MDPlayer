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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.c64.Banks
{
    /**
     * Area backed by RAM.
     *
     * @author Antti Lankila
     */
    public sealed class SystemRAMBank : IBank
    {



        //# include <stdint.h>
        //# include <cstring>
        //# include "Bank.h"
        //# include "sidcxx11.h"
        //friend class MMU;


        /// C64 RAM area
        public byte[] ram = new byte[0x10000];

        /**
         * Initialize RAM with powerup pattern.
         */
        public void reset()
        {
            for (int i = 0; i < ram.Length; i++) ram[i] = 0;
            for (int i = 0x40; i < 0x10000; i += 0x80)
            {
                for (int j = 0; j < 0x40; j++) ram[i + j] = 0xff;
            }
        }

        public  byte peek(UInt16 address)
        {
            return ram[address];
        }

        public  void poke(UInt16 address, byte value)
        {
            ram[address] = value;
        }




    }
}
