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
     * An implementation of of this class can be created to perform the C64
     * specifics.  A pointer to this child class can then be passed to
     * each of the components so they can interact with it.
     */
    public class c64env
    {



        //# include "EventScheduler.h"
        //# ifdef HAVE_CONFIG_H
        //# include "config.h"
        //#endif

        public EventScheduler eventScheduler;

        public c64env(EventScheduler scheduler) {
            eventScheduler = scheduler;
        }

        public EventScheduler scheduler()
        {
            return eventScheduler;
        }

        public virtual byte cpuRead(UInt16 addr) { return 0; }
        public virtual void cpuWrite(UInt16 addr, byte data) { }

#if PC64_TESTSUITE
    public virtual void loadFile(string file) {}
#endif

        public virtual void interruptIRQ(bool state) { }
        public virtual void interruptNMI() { }
        public virtual void interruptRST() { }

        public virtual void setBA(bool state) { }
        public virtual void lightpen(bool state) { }

        ~c64env() { }





    }
}
