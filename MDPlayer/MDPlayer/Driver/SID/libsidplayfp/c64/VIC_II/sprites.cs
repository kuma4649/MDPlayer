/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2014 Leandro Nini <drfiemost@users.sourceforge.net>
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
     * Sprites handling.
     */
    public class Sprites
    {



        //# include <stdint.h>
        //# include <cstring>
        public const int SPRITES = 8;

        private byte enable, y_expansion;

        private byte exp_flop;
        private byte dma;
        private byte[] mc_base = new byte[SPRITES];
        private byte[] mc = new byte[SPRITES];



        public Sprites(byte[] regs) //[0x40]) :
        {
            enable = regs[0x15];
            y_expansion = regs[0x17];
        }

        public void reset()
        {
            exp_flop = 0xff;
            dma = 0;

            mem.memset(ref mc_base, 0, mc_base.Length);
            mem.memset(ref mc, 0, mc.Length);
        }

        /**
         * Update mc values in one pass
         * after the dma has been processed
         */
        public void updateMc()
        {
            byte mask = 1;
            for (UInt32 i = 0; i < SPRITES; i++, mask <<= 1)
            {
                if ((dma & mask) != 0)
                    mc[i] = (byte)((mc[i] + 3) & 0x3f);
            }
        }

        /**
         * Update mc base value.
         */
        public void updateMcBase()
        {
            byte mask = 1;
            for (UInt32 i = 0; i < SPRITES; i++, mask <<= 1)
            {
                if ((exp_flop & mask) != 0)
                {
                    mc_base[i] = mc[i];
                    if (mc_base[i] == 0x3f)
                        dma &= (byte)~mask;
                }
            }
        }

        /**
         * Calculate sprite expansion.
         */
        public void checkExp()
        {
            exp_flop ^= (byte)(dma & y_expansion);
        }

        /**
         * Check if sprite is displayed.
         */
        public void checkDisplay()
        {
            for (UInt32 i = 0; i < SPRITES; i++)
            {
                mc[i] = mc_base[i];
            }
        }

        /**
         * Calculate sprite DMA.
         *
         * @rasterY y raster position
         * @regs the VIC registers
         */
        public void checkDma(UInt32 rasterY, byte[] regs)//[0x40])
        {
            byte y = (byte)(rasterY & 0xff);
            byte mask = 1;
            for (UInt32 i = 0; i < SPRITES; i++, mask <<= 1)
            {
                if ((enable & mask) != 0 && (y == regs[(i << 1) + 1]) && (dma & mask) == 0)
                {
                    dma |= mask;
                    mc_base[i] = 0;
                    exp_flop |= mask;
                }
            }
        }

        /**
         * Calculate line crunch.
         *
         * @param data the data written to the register
         * @param lineCycle current line cycle
         */
        public void lineCrunch(byte data, UInt32 lineCycle)
        {
            byte mask = 1;
            for (UInt32 i = 0; i < SPRITES; i++, mask <<= 1)
            {
                if ((data & mask) == 0 && (exp_flop & mask) == 0)
                {
                    // sprite crunch
                    if (lineCycle == 14)
                    {
                        byte mc_i = mc[i];
                        byte mcBase_i = mc_base[i];

                        mc[i] = (byte)((0x2a & (mcBase_i & mc_i)) | (0x15 & (mcBase_i | mc_i)));

                        // mcbase will be set from mc on the following clock call
                    }

                    exp_flop |= mask;
                }
            }
        }

        /**
         * Check if dma is active for sprites.
         *
         * @param val bitmask for selected sprites
         */
        public bool isDma(UInt32 val)
        {
            return (dma & val) != 0;
        }





    }
}