/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2014 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.c64.Banks
{
    /**
     * Extra SID bank.
     */
    public sealed class ExtraSidBank : IBank
    {




        //# include "Bank.h"
        //# include <vector>
        //# include <algorithm>
        //# include "c64/c64sid.h"
        //# include "sidcxx11.h"

        /**
         * Size of mapping table. Each 32 bytes another SID chip base address
         * can be assigned to.
         */
        private const int MAPPER_SIZE = 8;


        /**
         * SID mapping table.
         * Maps a SID chip base address to a SID
         * or to the underlying bank.
         */
        private IBank[] mapper = new IBank[MAPPER_SIZE];

        private List<c64sid> sids=new List<c64sid>();

        private static void resetSID(c64sid e)
        {
            e.reset(0xf);
        }

        private UInt32 mapperIndex(Int32 address)
        {
            return (UInt32)(address >> 5 & (MAPPER_SIZE - 1));
        }

        ~ExtraSidBank() { }

        public void reset()
        {
            foreach (c64sid v in sids)
            {
                resetSID(v);
            }
        }

        public void resetSIDMapper(IBank bank)
        {
            for (int i = 0; i < MAPPER_SIZE; i++)
                mapper[i] = bank;
        }

        public byte peek(UInt16 addr)
        {
            return mapper[mapperIndex(addr)].peek(addr);
        }

        public void poke(UInt16 addr, byte data)
        {
            mapper[mapperIndex(addr)].poke(addr, data);
        }

        /**
         * Set SID emulation.
         *
         * @param s the emulation
         * @param address the address where to put the chip
         */
        public void addSID(c64sid s, int address)
        {
            sids.Add(s);
            mapper[mapperIndex(address)] = s;
        }




    }
}
