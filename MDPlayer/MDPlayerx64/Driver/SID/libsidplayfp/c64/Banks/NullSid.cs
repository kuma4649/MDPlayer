/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2014 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2010 Antti Lankila
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

namespace Driver.libsidplayfp.c64.Banks
{
    /**
     * SID chip placeholder which does nothing and returns 0xff on reading.
     */
    public sealed class NullSid : c64sid
    {

        private static NullSid nullsid;



        //# include "c64/c64sid.h"
        //# include "sidcxx11.h"

        private NullSid() { }
        ~NullSid() { }

        /**
         * Returns singleton instance.
         */
        public static NullSid getInstance()
        {
            if (nullsid == null) nullsid = new NullSid();
            return nullsid;
        }

        public override void reset(byte a) { }
        public override void write(byte a, byte b) { }
        public override byte read(byte a) { return 0xff; }

    }
}
