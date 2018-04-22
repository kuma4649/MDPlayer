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
     * CIA 1
     *
     * Generates IRQs
     *
     * Located at $DC00-$DCFF
     */
    public sealed class c64cia1 : CIA.MOS6526, Banks.IBank
    {



        // The CIA emulations are very generic and here we need to effectively
        // wire them into the computer (like adding a chip to a PCB).

        //# include "Banks/Bank.h"
        //# include "c64/c64env.h"
        //# include "sidendian.h"
        //# include "CIA/mos6526.h"
        //# include "sidcxx11.h"

        private c64env m_env;
        private UInt16 last_ta;

        public override void interrupt(bool state)
        {
            m_env.interruptIRQ(state);
        }

        protected override void portB()
        {
            m_env.lightpen(((prb | ~ddrb) & 0x10) != 0);
        }

        public c64cia1(c64env env) : base(env.scheduler())
        {
            m_env = env;
        }

        public void poke(UInt16 address, byte value)
        {
            write(sidendian.endian_16lo8(address), value);

            // Save the value written to Timer A
            if (address == 0xDC04 || address == 0xDC05)
            {
                if (timerA.getTimer() != 0)
                    last_ta = timerA.getTimer();
            }
        }

        public byte peek(UInt16 address)
        {
            return read(sidendian.endian_16lo8(address));
        }

        public override void reset()
        {
            last_ta = 0;
            base.reset();
        }

        public UInt16 getTimerA()
        {
            return last_ta;
        }




    }

    /**
     * CIA 2
     *
     * Generates NMIs
     *
     * Located at $DD00-$DDFF
     */
    public class c64cia2 : CIA.MOS6526, Banks.IBank
    {
        private c64env m_env;

        public override void interrupt(bool state)
        {
            if (state)
                m_env.interruptNMI();
        }

        public c64cia2(c64env env) : base(env.scheduler())
        {
            m_env = env;
        }

        public void poke(UInt16 address, byte value)
        {
            write((byte)address, value);
        }

        public byte peek(UInt16 address)
        {
            return read((byte)address);
        }

    }



    
}