/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2009-2014 VICE Project
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

namespace Driver.libsidplayfp.c64.VIC_II
{
    /**
     * MOS 6567/6569/6572 emulation.
     * Not cycle exact but good enough for SID playback.
     */
    public class MOS656X : Event
    {



        //# include <stdint.h>
        //# include "lightpen.h"
        //# include "sprites.h"
        //# include "Event.h"
        //# include "EventCallback.h"
        //# include "EventScheduler.h"
        //# include "sidcxx11.h"
        public enum model_t
        {
            MOS6567R56A = 0  ///< OLD NTSC CHIP
        , MOS6567R8       ///< NTSC-M
        , MOS6569         ///< PAL-B
        , MOS6572         ///< PAL-N
        }

        //private typedef event_clock_t(MOS656X::* ClockFunc)();
        private delegate Int64 ClockFunc();

        private class model_data_t
        {
            public UInt32 rasterLines;
            public UInt32 cyclesPerLine;
            public ClockFunc clock;

            public model_data_t(UInt32 rasterLines, UInt32 cyclesPerLine, ClockFunc clock)
            {
                this.rasterLines = rasterLines;
                this.cyclesPerLine = cyclesPerLine;
                this.clock = clock;
            }
        }

        //private model_data_t[] modelData;

        /// raster IRQ flag
        private const int IRQ_RASTER = 1 << 0;

        /// Light-Pen IRQ flag
        private const int IRQ_LIGHTPEN = 1 << 3;

        /// First line when we check for bad lines
        private const UInt32 FIRST_DMA_LINE = 0x30;

        /// Last line when we check for bad lines
        private const UInt32 LAST_DMA_LINE = 0xf7;


        /// Current model clock function.
        private ClockFunc clock;

        /// Current raster clock.
        private Int64 rasterClk;

        /// System's event scheduler.
        private EventScheduler eventScheduler;

        /// Number of cycles per line.
        private UInt32 cyclesPerLine;

        /// Number of raster lines.
        private UInt32 maxRasters;

        /// Current visible line
        private UInt32 lineCycle;

        /// current raster line
        private UInt32 rasterY;

        /// vertical scrolling value
        private UInt32 yscroll;

        /// are bad lines enabled for this frame?
        private bool areBadLinesEnabled;

        /// is the current line a bad line
        private bool isBadLine;

        /// Is rasterYIRQ condition true?
        private bool rasterYIRQCondition;

        /// Set when new frame starts.
        private bool vblanking;

        /// Is CIA asserting lightpen?
        private bool lpAsserted;

        /// internal IRQ flags
        private byte irqFlags;

        /// masks for the IRQ flags
        private byte irqMask;

        /// Light pen
        private Lightpen lp=new Lightpen();

        /// the 8 sprites data
        private Sprites sprites;

        /// memory for chip registers
        private byte[] regs = new byte[0x40];

        private EventCallback<MOS656X> badLineStateChangeEvent;

        private EventCallback<MOS656X> rasterYIRQEdgeDetectorEvent;

        //private Int64 clockPAL() { }
        //private Int64 clockNTSC() { }
        //private Int64 clockOldNTSC() { }

        /**
         * Signal CPU interrupt if requested by VIC.
         */
        //private void handleIrqState() { }

        /**
         * AEC state was updated.
         */
        private void badLineStateChange() { setBA(!isBadLine); }

        /**
         * RasterY IRQ edge detector.
         */
        private void rasterYIRQEdgeDetector()
        {
            bool oldRasterYIRQCondition = rasterYIRQCondition;
            rasterYIRQCondition = rasterY == readRasterLineIRQ();
            if (!oldRasterYIRQCondition && rasterYIRQCondition)
                activateIRQFlag(IRQ_RASTER);
        }

        /**
         * Set an IRQ flag and trigger an IRQ if the corresponding IRQ mask is set.
         * The IRQ only gets activated, i.e. flag 0x80 gets set, if it was not active before.
         */
        private void activateIRQFlag(int flag)
        {
            irqFlags |= (byte)flag;
            handleIrqState();
        }

        /**
         * Read the value of the raster line IRQ
         *
         * @return raster line when to trigger an IRQ
         */
        private UInt32 readRasterLineIRQ()
        {
            return (UInt32)((regs[0x12] & 0xff) + ((regs[0x11] & 0x80) << 1));
        }

        /**
         * Read the DEN flag which tells whether the display is enabled
         *
         * @return true if DEN is set, otherwise false
         */
        private bool readDEN()
        {
            return (regs[0x11] & 0x10) != 0;
        }

        private bool evaluateIsBadLine()
        {
            return areBadLinesEnabled
                && rasterY >= FIRST_DMA_LINE
                && rasterY <= LAST_DMA_LINE
                && (rasterY & 7) == yscroll;
        }

        /**
         * Get previous value of Y raster
         */
        private UInt32 oldRasterY()
        {
            return (rasterY > 0 ? rasterY : maxRasters) - 1;
        }

        private void sync()
        {
            eventScheduler.cancel(this);
            event_();
        }

        /**
         * Check for vertical blanking.
         */
        private void checkVblank()
        {
            // IRQ occurred (xraster != 0)
            if (rasterY == (maxRasters - 1))
            {
                vblanking = true;
            }

            // Check DEN bit on first cycle of the line following the first DMA line
            if (rasterY == FIRST_DMA_LINE
                && !areBadLinesEnabled
                && readDEN())
            {
                areBadLinesEnabled = true;
            }

            // Disallow bad lines after the last possible one has passed
            if (rasterY == LAST_DMA_LINE)
            {
                areBadLinesEnabled = false;
            }

            isBadLine = false;

            if (!vblanking)
            {
                rasterY++;
                rasterYIRQEdgeDetector();
            }

            if (evaluateIsBadLine())
                isBadLine = true;
        }

        /**
         * Vertical blank (line 0).
         */
        private void vblank()
        {
            if (vblanking)
            {
                vblanking = false;
                rasterY = 0;
                rasterYIRQEdgeDetector();
                lp.untrigger();
                if (lpAsserted && lp.retrigger(lineCycle, rasterY))
                {
                    activateIRQFlag(IRQ_LIGHTPEN);
                }
            }
        }

        ///**
        // * Start DMA for sprite n.
        // */
        ////template<int n>
        //private void startDma()
        //{
        //    if (sprites.isDma(0x01 << n))
        //        setBA(false);
        //}
        ///**
        // * Start DMA for sprite 0.
        // */
        ////template<>
        //public void startDma<0>()
        //{
        //    setBA(!sprites.isDma(0x01));
        //}

        public void startDma(int n)
        {
            if (n == 0) setBA(!sprites.isDma(0x01));
            else
            {
                if (sprites.isDma((UInt32)(0x01 << n)))
                    setBA(false);
            }
        }

        ///**
        // * End DMA for sprite n.
        // */
        ////template<int n>
        //private void endDma()
        //{
        //    if (!sprites.isDma(0x06 << n))
        //        setBA(true);
        //}

        ///**
        // * End DMA for sprite 7.
        // */
        ////template<>
        //public void endDma<7>()
        //{
        //    setBA(true);
        //}

        public void endDma(int n)
        {
            if (n == 7) setBA(true);
            else
            {
                if (!sprites.isDma((UInt32)(0x06 << n)))
                    setBA(true);
            }
        }


        /**
         * Start bad line.
         */
        private void startBadline()
        {
            if (isBadLine)
                setBA(false);
        }

        //protected MOS656X(EventScheduler scheduler) { }
        ~MOS656X() { }

        // Environment Interface
        protected virtual void interrupt(bool state) { }
        protected virtual void setBA(bool state) { }

        /**
         * Read VIC register.
         *
         * @param addr
         *            Register to read.
         */
        //protected byte read(byte addr) { }

        /**
         * Write to VIC register.
         *
         * @param addr
         *            Register to write to.
         * @param data
         *            Data byte to write.
         */
        //protected void write(byte addr, byte data) { }

        //public override void event_() { }

        /**
         * Set chip model.
         */
        //public void chip(model_t model) { }

        /**
         * Trigger the lightpen. Sets the lightpen usage flag.
         */
        //public void triggerLightpen() { }

        /**
         * Clears the lightpen usage flag.
         */
        //public void clearLightpen() { }

        /**
         * Reset VIC II.
         */
        //public void reset() { }

        //public string credits() { }

        // Template specializations



        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2016 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2009-2014 VICE Project
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

        // References below are from:
        //     The MOS 6567/6569 video controller (VIC-II)
        //     and its application in the Commodore 64
        //     http://www.uni-mainz.de/~bauec002/VIC-Article.gz
        //
        // MOS 6572 info taken from http://solidstate.com.ar/wp/?p=200

        //# include "mos656x.h"
        //# include <cstring>
        //# include "sidendian.h"

        /// Cycle # at which the VIC takes the bus in a bad line (BA goes low).
        private const UInt32 VICII_FETCH_CYCLE = 11;

        private const UInt32 VICII_SCREEN_TEXTCOLS = 40;

        private model_data_t[] modelData = null;

        public string credits()
        {
            return
                    "MOS6567/6569/6572 (VIC II) Emulation:\n"
                    + "\tCopyright (C) 2001 Simon White\n"
                    + "\tCopyright (C) 2007-2010 Antti Lankila\n"
                    + "\tCopyright (C) 2009-2014 VICE Project\n"
                    + "\tCopyright (C) 2011-2016 Leandro Nini\n";
        }


        protected MOS656X(EventScheduler scheduler) : base("VIC Raster")
        {
            modelData = new model_data_t[]
            {
    new  model_data_t(262, 64, clockOldNTSC),  // Old NTSC (MOS6567R56A)
    new  model_data_t(263, 65, clockNTSC),     // NTSC-M   (MOS6567R8)
    new  model_data_t(312, 63, clockPAL),      // PAL-B    (MOS6569R1, MOS6569R3)
    new  model_data_t(312, 65, clockNTSC),     // PAL-N    (MOS6572)
            };

            eventScheduler = (scheduler);
            sprites = new Sprites(regs);
            badLineStateChangeEvent = new EventCallback<MOS656X>("Update AEC signal", this, badLineStateChange);
            rasterYIRQEdgeDetectorEvent = new EventCallback<MOS656X>("RasterY changed", this, rasterYIRQEdgeDetector);
            chip(model_t.MOS6569);
        }

        public void reset()
        {
            irqFlags = 0;
            irqMask = 0;
            yscroll = 0;
            rasterY = maxRasters - 1;
            lineCycle = 0;
            areBadLinesEnabled = false;
            isBadLine = false;
            rasterYIRQCondition = false;
            rasterClk = 0;
            vblanking = false;
            lpAsserted = false;

            mem.memset(ref regs, 0, regs.Length);

            lp.reset();
            sprites.reset();

            eventScheduler.cancel(this);
            eventScheduler.schedule(this, 0, event_phase_t.EVENT_CLOCK_PHI1);
        }

        public void chip(model_t model)
        {
            maxRasters = modelData[(int)model].rasterLines;
            cyclesPerLine = modelData[(int)model].cyclesPerLine;
            clock = modelData[(int)model].clock;

            lp.setScreenSize(maxRasters, cyclesPerLine);

            reset();
        }

        protected byte read(byte addr)
        {
            addr &= 0x3f;

            // Sync up timers
            sync();

            switch (addr)
            {
                case 0x11:
                    // Control register 1
                    return (byte)((regs[addr] & 0x7f) | (byte)((rasterY & 0x100) >> 1));
                case 0x12:
                    // Raster counter
                    return (byte)(rasterY & 0xFF);
                case 0x13:
                    return lp.getX();
                case 0x14:
                    return lp.getY();
                case 0x19:
                    // Interrupt Pending Register
                    return (byte)(irqFlags | 0x70);
                case 0x1a:
                    // Interrupt Mask Register
                    return (byte)(irqMask | 0xf0);
                default:
                    // for addresses < $20 read from register directly
                    if (addr < 0x20)
                        return regs[addr];
                    // for addresses < $2f set bits of high nibble to 1
                    if (addr < 0x2f)
                        return (byte)(regs[addr] | 0xf0);
                    // for addresses >= $2f return $ff
                    return 0xff;
            }
        }

        protected void write(byte addr, byte data)
        {
            addr &= 0x3f;

            regs[addr] = data;

            // Sync up timers
            sync();

            switch (addr)
            {
                case 0x11: // Control register 1
                    {
                        UInt32 oldYscroll = yscroll;
                        yscroll = (UInt32)(data & 0x7);

                        // This is the funniest part... handle bad line tricks.
                        bool wasBadLinesEnabled = areBadLinesEnabled;

                        if (rasterY == FIRST_DMA_LINE && lineCycle == 0)
                        {
                            areBadLinesEnabled = readDEN();
                        }

                        if (oldRasterY() == FIRST_DMA_LINE && readDEN())
                        {
                            areBadLinesEnabled = true;
                        }

                        if ((oldYscroll != yscroll || areBadLinesEnabled != wasBadLinesEnabled)
                            && rasterY >= FIRST_DMA_LINE
                            && rasterY <= LAST_DMA_LINE)
                        {
                            // Check whether bad line state has changed.
                            bool wasBadLine = (wasBadLinesEnabled && (oldYscroll == (rasterY & 7)));
                            bool nowBadLine = (areBadLinesEnabled && (yscroll == (rasterY & 7)));

                            if (nowBadLine != wasBadLine)
                            {
                                bool oldBadLine = isBadLine;

                                if (wasBadLine)
                                {
                                    if (lineCycle < VICII_FETCH_CYCLE)
                                    {
                                        isBadLine = false;
                                    }
                                }
                                else
                                {
                                    // Bad line may be generated during fetch interval
                                    //   (VICII_FETCH_CYCLE <= lineCycle < VICII_FETCH_CYCLE + VICII_SCREEN_TEXTCOLS + 3)
                                    // or outside the fetch interval but before raster ycounter is incremented
                                    //   (lineCycle <= VICII_FETCH_CYCLE + VICII_SCREEN_TEXTCOLS + 6)
                                    if (lineCycle <= VICII_FETCH_CYCLE + VICII_SCREEN_TEXTCOLS + 6)
                                    {
                                        isBadLine = true;
                                    }
                                }

                                if (isBadLine != oldBadLine)
                                    eventScheduler.schedule(badLineStateChangeEvent, 0, event_phase_t.EVENT_CLOCK_PHI1);
                            }
                        }
                    }
                    // fall-through
                    eventScheduler.schedule(rasterYIRQEdgeDetectorEvent, 0, event_phase_t.EVENT_CLOCK_PHI1);
                    break;

                case 0x12: // Raster counter
                           // check raster Y irq condition changes at the next PHI1
                    eventScheduler.schedule(rasterYIRQEdgeDetectorEvent, 0, event_phase_t.EVENT_CLOCK_PHI1);
                    break;

                case 0x17:
                    sprites.lineCrunch(data, lineCycle);
                    break;

                case 0x19:
                    // VIC Interrupt Flag Register
                    irqFlags &= (byte)((~data & 0x0f) | 0x80);
                    handleIrqState();
                    break;

                case 0x1a:
                    // IRQ Mask Register
                    irqMask = (byte)(data & 0x0f);
                    handleIrqState();
                    break;
            }
        }

        private void handleIrqState()
        {
            // signal an IRQ unless we already signaled it
            if ((irqFlags & irqMask & 0x0f) != 0)
            {
                if ((irqFlags & 0x80) == 0)
                {
                    interrupt(true);
                    irqFlags |= 0x80;
                }
            }
            else
            {
                if ((irqFlags & 0x80) != 0)
                {
                    interrupt(false);
                    irqFlags &= 0x7f;
                }
            }
        }

        public override void event_()
        {
            Int64 cycles = eventScheduler.getTime(rasterClk, eventScheduler.phase());

            Int64 delay;

            if (cycles != 0)
            {
                // Update x raster
                rasterClk += cycles;
                lineCycle += (UInt32)cycles;
                lineCycle %= cyclesPerLine;

                delay = (this.clock)();
            }
            else
                delay = 1;

            eventScheduler.schedule(this, (UInt32)(delay - (int)eventScheduler.phase()), event_phase_t.EVENT_CLOCK_PHI1);
        }

        private Int64 clockPAL()
        {
            Int64 delay = 1;

            switch (lineCycle)
            {
                case 0:
                    checkVblank();
                    endDma(2);
                    break;

                case 1:
                    vblank();
                    startDma(5);

                    // No sprites before next compulsory cycle
                    if (!sprites.isDma(0xf8))
                        delay = 10;
                    break;

                case 2:
                    endDma(3);
                    break;

                case 3:
                    startDma(6);
                    break;

                case 4:
                    endDma(4);
                    break;

                case 5:
                    startDma(7);
                    break;

                case 6:
                    endDma(5);

                    delay = sprites.isDma(0xc0) ? 2 : 4;
                    break;

                case 7:
                    break;

                case 8:
                    endDma(6);

                    delay = 2;
                    break;

                case 9:
                    break;

                case 10:
                    endDma(7);
                    break;

                case 11:
                    startBadline();

                    delay = 3;
                    break;

                case 12:
                    delay = 2;
                    break;

                case 13:
                    break;

                case 14:
                    sprites.updateMc();
                    break;

                case 15:
                    sprites.updateMcBase();

                    delay = 39;
                    break;

                case 54:
                    sprites.checkDma(rasterY, regs);
                    startDma(0);
                    break;

                case 55:
                    sprites.checkDma(rasterY, regs);    // Phi1
                    sprites.checkExp();                 // Phi2
                    startDma(0);
                    break;

                case 56:
                    startDma(1);
                    break;

                case 57:
                    sprites.checkDisplay();

                    // No sprites before next compulsory cycle
                    if (!sprites.isDma(0x1f))
                        delay = 6;
                    break;

                case 58:
                    startDma(2);
                    break;

                case 59:
                    endDma(0);
                    break;

                case 60:
                    startDma(3);
                    break;

                case 61:
                    endDma(1);
                    break;

                case 62:
                    startDma(4);
                    break;

                default:
                    delay = 54 - lineCycle;
                    break;
            }

            return delay;
        }

        private Int64 clockNTSC()
        {
            Int64 delay = 1;

            switch (lineCycle)
            {
                case 0:
                    checkVblank();
                    startDma(5);
                    break;

                case 1:
                    vblank();
                    endDma(3);

                    // No sprites before next compulsory cycle
                    if (!sprites.isDma(0xf8))
                        delay = 10;
                    break;

                case 2:
                    startDma(6);
                    break;

                case 3:
                    endDma(4);
                    break;

                case 4:
                    startDma(7);
                    break;

                case 5:
                    endDma(5);

                    delay = sprites.isDma(0xc0) ? 2 : 4;
                    break;

                case 6:
                    break;

                case 7:
                    endDma(6);

                    delay = 2;
                    break;

                case 8:
                    break;

                case 9:
                    endDma(7);

                    delay = 2;
                    break;

                case 10:
                    break;

                case 11:
                    startBadline();

                    delay = 3;
                    break;

                case 12:
                    delay = 2;
                    break;

                case 13:
                    break;

                case 14:
                    sprites.updateMc();
                    break;

                case 15:
                    sprites.updateMcBase();

                    delay = 40;
                    break;

                case 55:
                    sprites.checkDma(rasterY, regs);    // Phi1
                    sprites.checkExp();                 // Phi2
                    startDma(0);
                    break;

                case 56:
                    sprites.checkDma(rasterY, regs);
                    startDma(0);
                    break;

                case 57:
                    startDma(1);
                    break;

                case 58:
                    sprites.checkDisplay();

                    // No sprites before next compulsory cycle
                    if (!sprites.isDma(0x1f))
                        delay = 7;
                    break;

                case 59:
                    startDma(2);
                    break;

                case 60:
                    endDma(0);
                    break;

                case 61:
                    startDma(3);
                    break;

                case 62:
                    endDma(1);
                    break;

                case 63:
                    startDma(4);
                    break;

                case 64:
                    endDma(2);
                    break;

                default:
                    delay = 55 - lineCycle;
                    break;
            }

            return delay;
        }

        private Int64 clockOldNTSC()
        {
            Int64 delay = 1;

            switch (lineCycle)
            {
                case 0:
                    checkVblank();
                    endDma(2);
                    break;

                case 1:
                    vblank();
                    startDma(5);

                    // No sprites before next compulsory cycle
                    if (!sprites.isDma(0xf8))
                        delay = 10;
                    break;

                case 2:
                    endDma(3);
                    break;

                case 3:
                    startDma(6);
                    break;

                case 4:
                    endDma(4);
                    break;

                case 5:
                    startDma(7);
                    break;

                case 6:
                    endDma(5);

                    delay = sprites.isDma(0xc0) ? 2 : 4;
                    break;

                case 7:
                    break;

                case 8:
                    endDma(6);

                    delay = 2;
                    break;

                case 9:
                    break;

                case 10:
                    endDma(7);
                    break;

                case 11:
                    startBadline();

                    delay = 3;
                    break;

                case 12:
                    delay = 2;
                    break;

                case 13:
                    break;

                case 14:
                    sprites.updateMc();
                    break;

                case 15:
                    sprites.updateMcBase();

                    delay = 40;
                    break;

                case 55:
                    sprites.checkDma(rasterY, regs);    // Phi1
                    sprites.checkExp();                 // Phi2
                    startDma(0);
                    break;

                case 56:
                    sprites.checkDma(rasterY, regs);
                    startDma(0);
                    break;

                case 57:
                    sprites.checkDisplay();
                    startDma(1);

                    // No sprites before next compulsory cycle
                    delay = (!sprites.isDma(0x1f)) ? 7 : 2;
                    break;

                case 58:
                    break;

                case 59:
                    startDma(2);
                    break;

                case 60:
                    endDma(0);
                    break;

                case 61:
                    startDma(3);
                    break;

                case 62:
                    endDma(1);
                    break;

                case 63:
                    startDma(4);
                    break;

                default:
                    delay = 55 - lineCycle;
                    break;
            }

            return delay;
        }

        public void triggerLightpen()
        {
            // Synchronise simulation
            sync();

            lpAsserted = true;

            if (lp.trigger(lineCycle, rasterY))
            {
                activateIRQFlag(IRQ_LIGHTPEN);
            }
        }

        public void clearLightpen()
        {
            lpAsserted = false;
        }




    }
}