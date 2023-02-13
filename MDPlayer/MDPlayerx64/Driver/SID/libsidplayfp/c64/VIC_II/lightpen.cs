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
     * Lightpen emulation.
     * Does not reflect model differences.
     */
    public class Lightpen
    {



        /// Last VIC raster line
        private UInt32 lastLine;

        /// VIC cycles per line
        private UInt32 cyclesPerLine;

        /// X coordinate
        private UInt32 lpx;

        /// Y coordinate
        private UInt32 lpy;

        /// Has light pen IRQ been triggered in this frame already?
        private bool isTriggered;


        /**
         * Set VIC screen size.
         *
         * @param height number of raster lines
         * @param width number of cycles per line
         */
        public void setScreenSize(UInt32 height, UInt32 width)
        {
            lastLine = height - 1;
            cyclesPerLine = width;
        }

        /**
         * Reset the lightpen.
         */
        public void reset()
        {
            lpx = 0;
            lpy = 0;
            isTriggered = false;
        }

        /**
         * Return the low byte of x coordinate.
         */
        public byte getX() { return (byte)lpx; }

        /**
         * Return the low byte of y coordinate.
         */
        public byte getY() { return (byte)lpy; }

        /**
         * Retrigger lightpen on vertical blank.
         *
         * @param lineCycle current line cycle
         * @param rasterY current y raster position
         * @return true if an IRQ should be triggered
         */
        public bool retrigger(UInt32 lineCycle, UInt32 rasterY)
        {
            bool triggered = trigger(lineCycle, rasterY);
            switch (cyclesPerLine)
            {
                case 63:
                default:
                    lpx = 0xd1;
                    break;
                case 65:
                    lpx = 0xd5;
                    break;
            }
            return triggered;
        }

        /**
         * Trigger lightpen from CIA.
         *
         * @param lineCycle current line cycle
         * @param rasterY current y raster position
         * @return true if an IRQ should be triggered
         */
        public bool trigger(UInt32 lineCycle, UInt32 rasterY)
        {
            if (!isTriggered)
            {
                // don't trigger on the last line, except on the first cycle
                if ((rasterY == lastLine) && (lineCycle > 0))
                {
                    return false;
                }

                isTriggered = true;

                // Latch current coordinates
                lpx = (lineCycle << 2) + 2;
                lpy = rasterY;
                return true;
            }
            return false;
        }

        /**
         * Untrigger lightpen from CIA.
         */
        public void untrigger()
        {
            isTriggered = false;
        }




    }
}
