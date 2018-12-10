/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
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

namespace Driver.libsidplayfp
{
    /**
     * Inherit this class to create a new SID emulation.
     */
    public class sidemu : c64.c64sid
    {



        //# include <string>
        //# include "sidplayfp/SidConfig.h"
        //# include "sidplayfp/siddefs.h"
        //# include "Event.h"
        //# include "EventScheduler.h"
        //# include "c64/c64sid.h"
        //# include "sidcxx11.h"
        //class sidbuilder;

        /**
         * Buffer size. 5000 is roughly 5 ms at 96 kHz
         */
        public static class output
        {
            static public int OUTPUTBUFFERSIZE = 5000;
        }

        private sidplayfp.sidbuilder m_builder;

        //protected string ERR_UNSUPPORTED_FREQ;
        //protected string ERR_INVALID_SAMPLING;
        //protected string ERR_INVALID_CHIP;

        protected EventScheduler eventScheduler;

        protected Int64 m_accessClk;

        /// The sample buffer
        protected Int16[] m_buffer;

        /// Current position in buffer
        protected Int32 m_bufferpos;

        protected bool m_status;
        protected bool isLocked;

        protected string m_error;

        public sidemu(sidplayfp.sidbuilder builder)
        {
            m_builder = (builder);
            eventScheduler = (null);
            m_buffer = (null);
            m_bufferpos = (0);
            m_status = (true);
            isLocked = (false);
            m_error = ("N/A");
        }

        ~sidemu() { }

        /**
         * Clock the SID chip.
         */
        public virtual void clock() { }

        /**
         * Set execution environment and lock sid to it.
         */
        //public virtual bool lock_(EventScheduler scheduler) { return false; }

        /**
         * Unlock sid.
         */
        //public virtual void unlock() { }

        // Standard SID functions

        /**
         * Mute/unmute voice.
         */
        public virtual void voice(UInt32 num, bool mute) { }

        /**
         * Set SID model.
         */
        public virtual void model(sidplayfp.SidConfig.sid_model_t model) { }

        /**
         * Set the sampling method.
         *
         * @param systemfreq
         * @param outputfreq
         * @param method
         * @param fast
         */
        //virtual void sampling(float systemfreq SID_UNUSED, float outputfreq SID_UNUSED,
        //SidConfig::sampling_method_t method SID_UNUSED, bool fast SID_UNUSED)
        //{ }
        public virtual void sampling(float systemfreq, float outputfreq,
        sidplayfp.SidConfig.sampling_method_t method, bool fast)
        { }

        /**
         * Get a detailed error message.
         */
        public string error() { return m_error; }

        public sidplayfp.sidbuilder builder() { return m_builder; }

        /**
         * Get the current position in buffer.
         */
        public Int32 bufferpos() { return m_bufferpos; }

        /**
         * Set the position in buffer.
         */
        public void bufferpos(Int32 pos) { m_bufferpos = pos; }

        /**
         * Get the buffer.
         */
        public Int16[] buffer() { return m_buffer; }



        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
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

        //#include "sidemu.h"

        protected string ERR_UNSUPPORTED_FREQ = "Unable to set desired output frequency.";
        protected string ERR_INVALID_SAMPLING = "Invalid sampling method.";
        protected string ERR_INVALID_CHIP = "Invalid chip model.";

        public virtual bool lock_(EventScheduler scheduler)
        {
            if (isLocked)
                return false;

            isLocked = true;
            eventScheduler = scheduler;

            return true;
        }

        public virtual void unlock()
        {
            isLocked = false;
            eventScheduler = null;
        }

    }

}