/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.c64.CIA
{
    /**
     * This is the base class for the MOS6526 timers.
     */
    public class Timer : Event
    {



        //# include <stdint.h>
        //# include "Event.h"
        //# include "EventCallback.h"
        //# include "EventScheduler.h"
        //# include "sidcxx11.h"

        protected const UInt32 CIAT_CR_START = 0x01;
        protected const UInt32 CIAT_STEP = 0x04;
        protected const UInt32 CIAT_CR_ONESHOT = 0x08;
        protected const UInt32 CIAT_CR_FLOAD = 0x10;
        protected const UInt32 CIAT_PHI2IN = 0x20;
        protected const UInt32 CIAT_CR_MASK = CIAT_CR_START | CIAT_CR_ONESHOT | CIAT_CR_FLOAD | CIAT_PHI2IN;
        protected const UInt32 CIAT_COUNT2 = 0x100;
        protected const UInt32 CIAT_COUNT3 = 0x200;
        protected const UInt32 CIAT_ONESHOT0 = 0x08 << 8;
        protected const UInt32 CIAT_ONESHOT = 0x08 << 16;
        protected const UInt32 CIAT_LOAD1 = 0x10 << 8;
        protected const UInt32 CIAT_LOAD = 0x10 << 16;
        protected const UInt32 CIAT_OUT = 0x80000000;

        private EventCallback<Timer> m_cycleSkippingEvent;

        /// Event context.
        private EventScheduler eventScheduler;

        /**
         * This is a tri-state:
         *
         * - when -1: cia is completely stopped
         * - when 0: cia 1-clock events are ticking.
         * - otherwise: cycle skip event is ticking, and the value is the first
         *   phi1 clock of skipping.
         */
        private Int64 ciaEventPauseTime;

        /// PB6/PB7 Flipflop to signal underflows.
        private bool pbToggle;

        /// Current timer value.
        private UInt16 timer;

        /// Timer start value (Latch).
        private UInt16 latch;

        /// Copy of regs[CRA/B]
        private byte lastControlValue;


        /// Pointer to the MOS6526 which this Timer belongs to.
        protected MOS6526 parent;

        /// CRA/CRB control register / state.
        protected UInt32 state;


        /**
         * Perform scheduled cycle skipping, and resume.
         */
        //private void cycleSkippingEvent() { }

        /**
         * Execute one CIA state transition.
         */
        //private void clock() { }

        /**
         * Reschedule CIA event at the earliest interesting time.
         * If CIA timer is stopped or is programmed to just count down,
         * the events are paused.
         */
        //private void reschedule() { }

        /**
         * Timer ticking event.
         */
        //private override void event_() { }

        /**
         * Signal timer underflow.
         */
        public virtual void underFlow() { }

        /**
         * Handle the serial port.
         */
        public virtual void serialPort() { }


        /**
         * Create a new timer.
         *
         * @param name component name
         * @param context event context
         * @param parent the MOS6526 which this Timer belongs to
         */
        protected Timer(string name, EventScheduler scheduler, MOS6526 parent) : base(name)
        {
            m_cycleSkippingEvent=new EventCallback<Timer>("Skip CIA clock decrement cycles", this, cycleSkippingEvent);
            eventScheduler = (scheduler);
            pbToggle = (false);
            timer = (0);
            latch = (0);
            lastControlValue = (0);
            this.parent = (parent);
            state = (0);
        }


        /**
         * Set CRA/CRB control register.
         *
         * @param cr control register value
         */
        //public void setControlRegister(byte cr) { }

        /**
         * Perform cycle skipping manually.
         *
         * Clocks the CIA up to the state it should be in, and stops all events.
         */
        //public void syncWithCpu() { }

        /**
         * Counterpart of syncWithCpu(),
         * starts the event ticking if it is needed.
         * No clock() call or anything such is permissible here!
         */
        //public void wakeUpAfterSyncWithCpu() { }

        /**
         * Reset timer.
         */
        //public void reset() { }

        /**
         * Set low byte of Timer start value (Latch).
         *
         * @param data
         *            low byte of latch
         */
        //public void latchLo(byte data) { }

        /**
         * Set high byte of Timer start value (Latch).
         *
         * @param data
         *            high byte of latch
         */
        //public void latchHi(byte data) { }

        /**
         * Set PB6/PB7 Flipflop state.
         *
         * @param state
         *            PB6/PB7 flipflop state
         */
        public void setPbToggle(bool state) { pbToggle = state; }

        /**
         * Get current state value.
         *
         * @return current state value
         */
        public UInt32 getState() { return state; }

        /**
         * Get current timer value.
         *
         * @return current timer value
         */
        public UInt16 getTimer() { return timer; }

        /**
         * Get PB6/PB7 Flipflop state.
         *
         * @param reg value of the control register
         * @return PB6/PB7 flipflop state
         */
        public bool getPb(byte reg) { return (reg & 0x04) != 0 ? pbToggle : (state & CIAT_OUT) != 0; }

        private void reschedule()
        {
            // There are only two subcases to consider.
            //
            // - are we counting, and if so, are we going to
            //   continue counting?
            // - have we stopped, and are there no conditions to force a new beginning?
            //
            // Additionally, there are numerous flags that are present only in passing manner,
            // but which we need to let cycle through the CIA state machine.
            const Int64 unwanted = (Int64)(CIAT_OUT | CIAT_CR_FLOAD | CIAT_LOAD1 | CIAT_LOAD);
            if ((state & unwanted) != 0)
            {
                eventScheduler.schedule(this, 1);
                return;
            }

            if ((state & CIAT_COUNT3) != 0)
            {
                // Test the conditions that keep COUNT2 and thus COUNT3 alive, and also
                // ensure that all of them are set indicating steady state operation.

                const UInt32 wanted = CIAT_CR_START | CIAT_PHI2IN | CIAT_COUNT2 | CIAT_COUNT3;
                if (timer > 2 && (state & wanted) == wanted)
                {
                    // we executed this cycle, therefore the pauseTime is +1. If we are called
                    // to execute on the very next clock, we need to get 0 because there's
                    // another timer-- in it.
                    ciaEventPauseTime = eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI1) + 1;
                    // execute event slightly before the next underflow.
                    eventScheduler.schedule(m_cycleSkippingEvent, (UInt32)(timer - 1));
                    return;
                }

                // play safe, keep on ticking.
                eventScheduler.schedule(this, 1);
            }
            else
            {
                // Test conditions that result in CIA activity in next clocks.
                // If none, stop.
                const UInt32 unwanted1 = CIAT_CR_START | CIAT_PHI2IN;
                const UInt32 unwanted2 = CIAT_CR_START | CIAT_STEP;

                if ((state & unwanted1) == unwanted1
                    || (state & unwanted2) == unwanted2)
                {
                    eventScheduler.schedule(this, 1);
                    return;
                }

                ciaEventPauseTime = -1;
            }
        }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //#include "timer.h"
        //#include "sidendian.h"

        public void setControlRegister(byte cr)
        {
            state &= 0xffffffc6;// ~CIAT_CR_MASK (0x39);
            state |= (cr & CIAT_CR_MASK) ^ CIAT_PHI2IN;
            lastControlValue = cr;
        }

        public void syncWithCpu()
        {
            if (ciaEventPauseTime > 0)
            {
                eventScheduler.cancel(m_cycleSkippingEvent);
                Int64 elapsed = eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI2) - ciaEventPauseTime;

                // It's possible for CIA to determine that it wants to go to sleep starting from the next
                // cycle, and then have its plans aborted by CPU. Thus, we must avoid modifying
                // the CIA state if the first sleep clock was still in the future.
                if (elapsed >= 0)
                {
                    timer -= (UInt16)elapsed;
                    clock();
                }
            }
            if (ciaEventPauseTime == 0)
            {
                eventScheduler.cancel(this);
            }
            ciaEventPauseTime = -1;
        }

        public void wakeUpAfterSyncWithCpu()
        {
            ciaEventPauseTime = 0;
            eventScheduler.schedule(this, 0, event_phase_t.EVENT_CLOCK_PHI1);
        }

        public override void event_()
        {
            clock();
            reschedule();
        }

        private void cycleSkippingEvent()
        {
            Int64 elapsed = eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI1) - ciaEventPauseTime;
            ciaEventPauseTime = 0;
            timer -= (UInt16)elapsed;
            event_();
        }

        private void clock()
        {
            if (timer != 0 && (state & CIAT_COUNT3) != 0)
            {
                timer--;
            }

            /* ciatimer.c block start */
            UInt32 adj = state & (CIAT_CR_START | CIAT_CR_ONESHOT | CIAT_PHI2IN);
            if ((state & (CIAT_CR_START | CIAT_PHI2IN)) == (CIAT_CR_START | CIAT_PHI2IN))
            {
                adj |= CIAT_COUNT2;
            }
            if ((state & CIAT_COUNT2) != 0
                    || (state & (CIAT_STEP | CIAT_CR_START)) == (CIAT_STEP | CIAT_CR_START))
            {
                adj |= CIAT_COUNT3;
            }
            // CR_FLOAD -> LOAD1, CR_ONESHOT -> ONESHOT0, LOAD1 -> LOAD, ONESHOT0 -> ONESHOT
            adj |= (state & (CIAT_CR_FLOAD | CIAT_CR_ONESHOT | CIAT_LOAD1 | CIAT_ONESHOT0)) << 8;
            state = adj;
            /* ciatimer.c block end */

            if (timer == 0 && (state & CIAT_COUNT3) != 0)
            {
                state |= (CIAT_LOAD | CIAT_OUT);

                if ((state & (CIAT_ONESHOT | CIAT_ONESHOT0)) != 0)
                {
                    state &= 0xfffffefe; //(UInt32)(~(CIAT_CR_START | CIAT_COUNT2));
                }

                // By setting bits 2&3 of the control register,
                // PB6/PB7 will be toggled between high and low at each underflow.
                bool toggle = (lastControlValue & 0x06) == 6;
                pbToggle = toggle && !pbToggle;

                // Implementation of the serial port
                serialPort();

                // Timer A signals underflow handling: IRQ/B-count
                underFlow();
            }

            if ((state & CIAT_LOAD) != 0)
            {
                timer = latch;
                state &= 0xfffffdff; //~CIAT_COUNT3;
            }
        }

        public void reset()
        {
            eventScheduler.cancel(this);
            timer = latch = 0xffff;
            pbToggle = false;
            state = 0;
            lastControlValue = 0;
            ciaEventPauseTime = 0;
            eventScheduler.schedule(this, 1, event_phase_t.EVENT_CLOCK_PHI1);
        }

        public void latchLo(byte data)
        {
            sidendian.endian_16lo8(ref latch, data);
            if ((state & CIAT_LOAD) != 0)
                sidendian.endian_16lo8(ref timer, data);
        }

        public void latchHi(byte data)
        {
            sidendian.endian_16hi8(ref latch, data);
            // Reload timer if stopped
            if ((state & CIAT_LOAD) != 0 || (state & CIAT_CR_START) == 0)
                timer = latch;
        }

    }

}



