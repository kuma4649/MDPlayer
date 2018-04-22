/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2016 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.c64.CPU
{
    /**
     * Processor Status Register
     */
    public class Flags
    {



        //# include <stdint.h>

        private bool C; ///< Carry
        private bool Z; ///< Zero
        private bool I; ///< Interrupt disabled
        private bool D; ///< Decimal
        private bool B; ///< Break
        private bool V; ///< Overflow
        private bool N; ///< Negative


        public void reset()
        {
            C = Z = I = D = V = N = false;
            B = true;
        }

        /**
         * Set N and Z flag values.
         *
         * @param value to set flags from
         */
        public void setNZ(byte value)
        {
            Z = value == 0;
            N = (value & 0x80) != 0;
        }

        /**
         * Get status register value.
         */
        public byte get()
        {
            byte sr = 0x20;

            if (C) sr |= 0x01;
            if (Z) sr |= 0x02;
            if (I) sr |= 0x04;
            if (D) sr |= 0x08;
            if (B) sr |= 0x10;
            if (V) sr |= 0x40;
            if (N) sr |= 0x80;

            return sr;
        }

        /**
         * Set status register value.
         */
        public void set(byte sr)
        {
            Z = (sr & 0x02) != 0;
            C = (sr & 0x01) != 0;
            I = (sr & 0x04) != 0;
            D = (sr & 0x08) != 0;
            B = (sr & 0x10) != 0;
            V = (sr & 0x40) != 0;
            N = (sr & 0x80) != 0;
        }

        public bool getN() { return N; }
        public bool getC() { return C; }
        public bool getD() { return D; }
        public bool getZ() { return Z; }
        public bool getV() { return V; }
        public bool getI() { return I; }
        public bool getB() { return B; }

        public void setN(bool f) { N = f; }
        public void setC(bool f) { C = f; }
        public void setD(bool f) { D = f; }
        public void setZ(bool f) { Z = f; }
        public void setV(bool f) { V = f; }
        public void setI(bool f) { I = f; }
        public void setB(bool f) { B = f; }




    }
}
