/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2009-2014 VICE Project
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
    //# include <memory>
    //# include <stdint.h>
    //# include "interrupt.h"
    //# include "timer.h"
    //# include "tod.h"
    //# include "SerialPort.h"
    //# include "EventScheduler.h"
    //# include "sidcxx11.h"

    public enum Reg
    {
        PRA = 0,
        PRB = 1,
        DDRA = 2,
        DDRB = 3,
        TAL = 4,
        TAH = 5,
        TBL = 6,
        TBH = 7,
        TOD_TEN = 8,
        TOD_SEC = 9,
        TOD_MIN = 10,
        TOD_HR = 11,
        SDR = 12,
        ICR = 13,
        IDR = 13,
        CRA = 14,
        CRB = 15
    };


    /**
     * This is the timer A of this CIA.
     *
     * @author Ken Händel
     *
     */
    public sealed class TimerA : Timer
    {
        /**
         * Signal underflows of Timer A to Timer B.
         */
        public override void underFlow()
        {
            parent.underflowA();
        }

        public override void serialPort()
        {
            parent.handleSerialPort();
        }

        /**
         * Create timer A.
         */
        public TimerA(EventScheduler scheduler, MOS6526 parent) : base("CIA Timer A", scheduler, parent)
        {
        }

    }

    /**
 * This is the timer B of this CIA.
 *
 * @author Ken Händel
 *
 */
    public sealed class TimerB : Timer
    {
        public override void underFlow()
        {
            parent.underflowB();
        }

        /**
         * Create timer B.
         */
        public TimerB(EventScheduler scheduler, MOS6526 parent) : base("CIA Timer B", scheduler, parent)
        {
        }

        /**
         * Receive an underflow from Timer A.
         */
        public void cascade()
        {
            // we pretend that we are CPU doing a write to ctrl register
            syncWithCpu();
            state |= CIAT_STEP;
            wakeUpAfterSyncWithCpu();
        }

        /**
         * Check if start flag is set.
         *
         * @return true if start flag is set, false otherwise
         */
        public bool started() { return (state & CIAT_CR_START) != 0; }
    }


    /**
     * InterruptSource that acts like new CIA
     */
    public sealed class InterruptSource6526A : InterruptSource
    {
        public InterruptSource6526A(EventScheduler scheduler, MOS6526 parent) : base(scheduler, parent)
        {
        }

        public override void trigger(byte interruptMask)
        {
            base.trigger(interruptMask);

            if (interruptMasked() && interruptTriggered())
            {
                triggerInterrupt();
                parent.interrupt(true);
            }
        }

        public override byte clear()
        {
            if (!interruptTriggered())
            {
                parent.interrupt(false);
            }

            return base.clear();
        }

        public override void event_()
        {
            throw new Exception("6526A event called unexpectedly");
        }
    }


    /**
     * InterruptSource that acts like old CIA
     */
    public sealed class InterruptSource6526 : InterruptSource
    {
        /// Have we already scheduled CIA->CPU interrupt transition?
        private bool scheduled;

        /**
         * Schedules an IRQ asserting state transition for next cycle.
         */
        private void schedule()
        {
            if (!scheduled)
            {
                eventScheduler.schedule(this, 1, event_phase_t.EVENT_CLOCK_PHI1);
                scheduled = true;
            }
        }

        public InterruptSource6526(EventScheduler scheduler, MOS6526 parent) : base(scheduler, parent)
        {
        }

        public override void trigger(byte interruptMask)
        {
            base.trigger(interruptMask);

            if (interruptMasked() && interruptTriggered())
            {
                schedule();
            }
        }

        public override byte clear()
        {
            if (scheduled)
            {
                eventScheduler.cancel(this);
                scheduled = false;
            }

            if (!interruptTriggered())
            {
                parent.interrupt(false);
            }

            return base.clear();
        }

        /**
         * Signal interrupt to CPU.
         */
        public override void event_()
        {
            triggerInterrupt();
            parent.interrupt(true);

            scheduled = false;
        }

        public override void reset()
        {
            base.reset();

            scheduled = false;
        }

    }

    /**
     * This class is heavily based on the ciacore/ciatimer source code from VICE.
     * The CIA state machine is lifted as-is. Big thanks to VICE project!
     *
     * @author alankila
     */
    public class MOS6526
    {
        //friend class InterruptSource6526;
        //friend class InterruptSource6526A;
        //friend class TimerA;
        //friend class TimerB;
        //friend class Tod;

        //private string credit;

        /// Event context.
        protected EventScheduler eventScheduler;

        /// Ports
        //@{
        protected byte pra, prb, ddra, ddrb;
        //@}

        /// These are all CIA registers.
        protected byte[] regs = new byte[0x10];

        /// Timers A and B.
        //@{
        protected TimerA timerA;
        protected TimerB timerB;
        //@}

        /// Interrupt Source
        //std::unique_ptr<InterruptSource> interruptSource;
        protected InterruptSource interruptSource;

        /// TOD
        protected Tod tod;

        /// Serial Data Registers
        protected SerialPort serialPort;

        /// Have we already scheduled CIA->CPU interrupt transition?
        protected bool triggerScheduled;

        /// Events
        //@{
        protected EventCallback<MOS6526> bTickEvent;
        //@}

        /**
         * Trigger an interrupt from TOD.
         */
        public void todInterrupt()
        {
            interruptSource.trigger((byte)INTERRUPT.INTERRUPT_ALARM);
        }

        /**
         * This event exists solely to break the ambiguity of what scheduling on
         * top of PHI1 causes, because there is no ordering between events on
         * same phase. Thus it is scheduled in PHI2 to ensure the b.event() is
         * run once before the value changes.
         *
         * - PHI1 a.event() (which calls underFlow())
         * - PHI1 b.event()
         * - PHI2 bTick.event()
         * - PHI1 a.event()
         * - PHI1 b.event()
         */
        private void bTick() {
            timerB.cascade();
        }

        /**
         * Timer A underflow.
         */
        public void underflowA()
        {
            interruptSource.trigger((byte)INTERRUPT.INTERRUPT_UNDERFLOW_A);

            if ((regs[(int)Reg.CRB] & 0x41) == 0x41)
            {
                if (timerB.started())
                {
                    eventScheduler.schedule(bTickEvent, 0, event_phase_t.EVENT_CLOCK_PHI2);
                }
            }
        }

        /** Timer B underflow. */
        public void underflowB()
        {
            interruptSource.trigger((byte)INTERRUPT.INTERRUPT_UNDERFLOW_B);
        }

        /**
         * Handle the serial port.
         */
        public void handleSerialPort()
        {
            if ((regs[(int)Reg.CRA] & 0x40)!=0)
            {
                serialPort.handle(regs[(int)Reg.SDR]);
            }
        }

        /**
         * Create a new CIA.
         *
         * @param context the event context
         */
        protected MOS6526(EventScheduler scheduler)
        {
            eventScheduler = scheduler;
            pra = regs[(int)Reg.PRA];
            prb = regs[(int)Reg.PRB];
            ddra = regs[(int)Reg.DDRA];
            ddrb = regs[(int)Reg.DDRB];
            timerA = new TimerA(scheduler, this);
            timerB = new TimerB(scheduler, this);
            interruptSource = new InterruptSource6526(scheduler, this);
            tod = new Tod(scheduler, this, regs);
            serialPort = new SerialPort(interruptSource);
            bTickEvent=new EventCallback<MOS6526>("CIA B counts A", this, bTick);

            reset();
        }

        ~MOS6526()
        {
        }

        /**
         * Signal interrupt.
         *
         * @param state
         *            interrupt state
         */
        public virtual void interrupt(bool state) { }

        protected virtual void portA() { }
        protected virtual void portB() { }

        /**
         * Read CIA register.
         *
         * @param addr
         *            register address to read (lowest 4 bits)
         */
        protected byte read(byte addr)
        {
            addr &= 0x0f;

            timerA.syncWithCpu();
            timerA.wakeUpAfterSyncWithCpu();
            timerB.syncWithCpu();
            timerB.wakeUpAfterSyncWithCpu();

            switch (addr)
            {
                case (int)Reg.PRA: // Simulate a serial port
                    return (byte)(regs[(int)Reg.PRA] | ~regs[(int)Reg.DDRA]);
                case (int)Reg.PRB:
                    {
                        byte data = (byte)(regs[(int)Reg.PRB] | ~regs[(int)Reg.DDRB]);
                        // Timers can appear on the port
                        if ((regs[(int)Reg.CRA] & 0x02) != 0)
                        {
                            data &= 0xbf;
                            if (timerA.getPb(regs[(int)Reg.CRA]))
                                data |= 0x40;
                        }
                        if ((regs[(int)Reg.CRB] & 0x02) != 0)
                        {
                            data &= 0x7f;
                            if (timerB.getPb(regs[(int)Reg.CRB]))
                                data |= 0x80;
                        }
                        return data;
                    }
                case (int)Reg.TAL:
                    return sidendian.endian_16lo8(timerA.getTimer());
                case (int)Reg.TAH:
                    return sidendian.endian_16hi8(timerA.getTimer());
                case (int)Reg.TBL:
                    return sidendian.endian_16lo8(timerB.getTimer());
                case (int)Reg.TBH:
                    return sidendian.endian_16hi8(timerB.getTimer());
                case (int)Reg.TOD_TEN:
                case (int)Reg.TOD_SEC:
                case (int)Reg.TOD_MIN:
                case (int)Reg.TOD_HR:
                    return tod.read((byte)(addr - (int)Reg.TOD_TEN));
                case (int)Reg.IDR:
                    return interruptSource.clear();
                case (int)Reg.CRA:
                    return (byte)((regs[(int)Reg.CRA] & 0xfe) | (byte)(timerA.getState() & 1));
                case (int)Reg.CRB:
                    return (byte)((regs[(int)Reg.CRB] & 0xfe) | (byte)(timerB.getState() & 1));
                default:
                    return regs[addr];
            }
        }

        /**
         * Write CIA register.
         *
         * @param addr
         *            register address to write (lowest 4 bits)
         * @param data
         *            value to write
         */
        protected void write(byte addr, byte data) {
            addr &= 0x0f;

            timerA.syncWithCpu();
            timerB.syncWithCpu();

            byte oldData = regs[addr];
            regs[addr] = data;

            switch (addr)
            {
                case (int)Reg.PRA:
                case (int)Reg.DDRA:
                    portA();
                    break;
                case (int)Reg.PRB:
                case (int)Reg.DDRB:
                    portB();
                    break;
                case (int)Reg.TAL:
                    timerA.latchLo(data);
                    break;
                case (int)Reg.TAH:
                    timerA.latchHi(data);
                    break;
                case (int)Reg.TBL:
                    timerB.latchLo(data);
                    break;
                case (int)Reg.TBH:
                    timerB.latchHi(data);
                    break;
                case (int)Reg.TOD_TEN:
                case (int)Reg.TOD_SEC:
                case (int)Reg.TOD_MIN:
                case (int)Reg.TOD_HR:
                    tod.write((byte)(addr - (int)Reg.TOD_TEN), data);
                    break;
                case (int)Reg.SDR:
                    if ((regs[(int)Reg.CRA] & 0x40)!=0)
                        serialPort.setBuffered();
                    break;
                case (int)Reg.ICR:
                    interruptSource.set(data);
                    break;
                case (int)Reg.CRA:
                    if ((data & 1)!=0 && (oldData & 1)==0)
                    {
                        // Reset the underflow flipflop for the data port
                        timerA.setPbToggle(true);
                    }
                    timerA.setControlRegister(data);
                    break;
                case (int)Reg.CRB:
                    if ((data & 1)!=0 && (oldData & 1)==0)
                    {
                        // Reset the underflow flipflop for the data port
                        timerB.setPbToggle(true);
                    }
                    timerB.setControlRegister((byte)(data | (data & 0x40) >> 1));
                    break;
            }

            timerA.wakeUpAfterSyncWithCpu();
            timerB.wakeUpAfterSyncWithCpu();
        }

        /**
         * Reset CIA.
         */
        public virtual void reset()
        {
            for (int i = 0; i < regs.Length; i++) regs[i] = 0;

            serialPort.reset();

            // Reset timers
            timerA.reset();
            timerB.reset();

            // Reset interruptSource
            interruptSource.reset();

            // Reset tod
            tod.reset();

            triggerScheduled = false;

            eventScheduler.cancel(bTickEvent);
        }

        /**
         * Get the credits.
         *
         * @return the credits
         */
        public string credits()
        {
            return
                    "MOS6526/6526A (CIA) Emulation:\n"
            + "\tCopyright (C) 2001-2004 Simon White\n"
            + "\tCopyright (C) 2007-2010 Antti S. Lankila\n"
            + "\tCopyright (C) 2009-2014 VICE Project\n"
            + "\tCopyright (C) 2011-2015 Leandro Nini\n";
        }

        /**
         * Set day-of-time event occurence of rate.
         *
         * @param clock
         */
        public void setDayOfTimeRate(UInt32 clock)
        {
            tod.setPeriod(clock);
        }





    }
}