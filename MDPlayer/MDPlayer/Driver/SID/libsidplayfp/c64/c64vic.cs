/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2001 Simon White
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
     * VIC-II
     *
     * Located at $D000-$D3FF
     */
    public sealed class c64vic : VIC_II.MOS656X, Banks.IBank
    {




        // The VIC emulation is very generic and here we need to effectively
        // wire it into the computer (like adding a chip to a PCB).

        //# include "Banks/Bank.h"
        //# include "c64/c64env.h"
        //# include "sidendian.h"
        //# include "VIC_II/mos656x.h"
        //# include "sidcxx11.h"

        private c64env m_env;

        protected override void interrupt(bool state)
        {
            m_env.interruptIRQ(state);
        }

        protected override void setBA(bool state)
        {
            m_env.setBA(state);
        }

        public c64vic(c64env env) : base(env.scheduler())
        {
            m_env = env;
        }

        public void poke(UInt16 address, byte value)
        {
            write(sidendian.endian_16lo8(address), value);
        }

        public byte peek(UInt16 address)
        {
            return read(sidendian.endian_16lo8(address));
        }





    }
}