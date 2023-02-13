/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
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

namespace Driver.libsidplayfp.c64.CIA
{
    public enum INTERRUPT : int
    {
        INTERRUPT_NONE = 0,
        INTERRUPT_UNDERFLOW_A = 1 << 0,
        INTERRUPT_UNDERFLOW_B = 1 << 1,
        INTERRUPT_ALARM = 1 << 2,
        INTERRUPT_SP = 1 << 3,
        INTERRUPT_FLAG = 1 << 4,
        INTERRUPT_REQUEST = 1 << 7
    }

    /**
     * This is the base class for the MOS6526 interrupt sources.
     */
    public class InterruptSource : Event
    {



        //# include "Event.h"
        //# include "EventScheduler.h"
        //# include <stdint.h>
        //# include "sidcxx11.h"

        /// Pointer to the MOS6526 which this Interrupt belongs to.
        protected MOS6526 parent;

        /// Event scheduler.
        protected EventScheduler eventScheduler;

        /// Interrupt control register
        private byte icr;

        /// Interrupt data register
        private byte idr;

        protected bool interruptMasked() { return (icr & idr) != 0; }

        protected bool interruptTriggered() { return (idr & (byte)INTERRUPT.INTERRUPT_REQUEST) == 0; }

        protected void triggerInterrupt() { idr |= (byte)INTERRUPT.INTERRUPT_REQUEST; }


        /**
         * Create a new InterruptSource.
         *
         * @param scheduler event scheduler
         * @param parent the MOS6526 which this Interrupt belongs to
         */
        protected InterruptSource(EventScheduler scheduler, MOS6526 parent) : base("CIA Interrupt")
        {
            this.parent = parent;
            eventScheduler = scheduler;
            icr = 0;
            idr = 0;
        }

        /**
         * Trigger an interrupt.
         * 
         * @param interruptMask Interrupt flag number
         */
        public virtual void trigger(byte interruptMask) { idr |= interruptMask; }

        /**
         * Clear interrupt state.
         * 
         * @return old interrupt state
         */
        public virtual byte clear()
        {
            byte old = idr;
            idr = 0;
            return old;
        }

        /**
         * Clear pending interrupts, but do not signal to CPU we lost them.
         * It is assumed that all components get reset() calls in synchronous manner.
         */
        public virtual void reset()
        {
            icr = 0;
            idr = 0;
            eventScheduler.cancel(this);
        }

        /**
         * Set interrupt control mask bits.
         *
         * @param interruptMask control mask bits
         */
        public void set(byte interruptMask)
        {
            if ((interruptMask & 0x80) != 0)
            {
                icr |= (byte)(interruptMask & 0x7f);// ~INTERRUPT.INTERRUPT_REQUEST);
                trigger((byte)INTERRUPT.INTERRUPT_NONE);
            }
            else
            {
                icr &= (byte)~interruptMask;
            }
        }




    }
}