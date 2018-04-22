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

namespace Driver.libsidplayfp.builders.resid_builder
{




    //# include <stdint.h>
    //# include "sidplayfp/SidConfig.h"
    //# include "sidemu.h"
    //# include "Event.h"
    //# include "resid/sid.h"
    //# include "sidcxx11.h"


    public sealed class ReSID : sidemu
    {
        private reSID.SID m_sid;
        private byte m_voiceMask;

        //public static string getCredits() { return ""; }

        //public ReSID(libsidplayfp.sidplayfp.sidbuilder builder) : base(builder) { }
        //~ReSID() { }

        public bool getStatus() { return m_status; }

        //public override byte read(byte addr) { return 0; }
        //public override void write(byte addr, byte data) { }

        // c64sid functions
        //public override void reset(byte volume) { }

        // Standard SID emu functions
        //public override void clock() { }

        //public void sampling(float systemclock, float freq, sidplayfp.SidConfig.sampling_method_t method, bool fast){ }

        //public override void voice(UInt32 num, bool mute) { }

        //public void model(sidplayfp.SidConfig.sid_model_t model) { }

        // Specific to resid
        //public void bias(double dac_bias) { }
        //public void filter(bool enable) { }




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

        //#include "resid-emu.h"
        //#include <cstdio>
        //#include <cstring>
        //#include <sstream>
        //#include <string>
        //#include "resid/siddefs.h"
        //#include "resid/spline.h"

        public static string getCredits()
        {
            string credits = "";

            if (credits == "")
            {
                // Setup credits
                string ss;
                ss = "ReSID V" + Const.VERSION + " Engine:\n";
                ss = "\t(C) 1999-2002 Simon White\n";
                ss = "MOS6581 (SID) Emulation (ReSID V" + reSID.siddefs.resid_version_string + "):\n";
                ss = "\t(C) 1999-2010 Dag Lem\n";
                credits = ss;
            }

            return credits;
        }

        public ReSID(libsidplayfp.sidplayfp.sidbuilder builder) : base(builder)
        {
            m_sid = new reSID.SID();
            m_voiceMask = 0x07;
            m_buffer = new short[(int)libsidplayfp.sidemu.output.OUTPUTBUFFERSIZE];
            reset(0);
        }

        ~ReSID()
        {
            m_sid = null;
            m_buffer = null;
        }

        public void bias(double dac_bias)
        {
            m_sid.adjust_filter_bias(dac_bias);
        }

        // Standard component options
        public override void reset(byte volume)
        {
            m_accessClk = 0;
            m_sid.reset();
            m_sid.write(0x18, volume);
        }

        public override byte read(byte addr)
        {
            //Console.WriteLine("[{0:d010}]read  addr[{1:x02}]", m_accessClk, addr);
            clock();
            return (byte)m_sid.read((UInt32)addr);
        }

        public override void write(byte addr, byte data)
        {
            //Console.WriteLine("[{0:d010}]write addr[{1:x02}] data[{2:x02}]", m_accessClk, addr,data);
            clock();
            m_sid.write(addr, data);
        }

        public override void clock()
        {
            Int32 cycles = (Int32)eventScheduler.getTime(m_accessClk, event_phase_t.EVENT_CLOCK_PHI1);
            m_accessClk += cycles;
            m_bufferpos += (int)m_sid.clock(ref cycles, m_buffer , m_bufferpos, (uint)((uint)libsidplayfp.sidemu.output.OUTPUTBUFFERSIZE - m_bufferpos), 1);
        }

        public void filter(bool enable)
        {
            m_sid.enable_filter(enable);
        }

        public override void sampling(float systemclock, float freq, sidplayfp.SidConfig.sampling_method_t method, bool fast)
        {
            reSID.siddefs.sampling_method sampleMethod;
            switch (method)
            {
                case sidplayfp.SidConfig.sampling_method_t.INTERPOLATE:
                    sampleMethod = fast ? reSID.siddefs.sampling_method.SAMPLE_FAST : reSID.siddefs.sampling_method.SAMPLE_INTERPOLATE;
                    break;
                case sidplayfp.SidConfig.sampling_method_t.RESAMPLE_INTERPOLATE:
                    sampleMethod = fast ? reSID.siddefs.sampling_method.SAMPLE_RESAMPLE_FASTMEM : reSID.siddefs.sampling_method.SAMPLE_RESAMPLE;
                    break;
                default:
                    m_status = false;
                    m_error = ERR_INVALID_SAMPLING;
                    return;
            }

            if (!m_sid.set_sampling_parameters(systemclock, sampleMethod, freq))
            {
                m_status = false;
                m_error = ERR_UNSUPPORTED_FREQ;
                return;
            }

            m_status = true;
        }

        public override void voice(UInt32 num, bool mute)
        {
            if (mute)
                m_voiceMask &= (byte)~(1 << (Int32)num);
            else
                m_voiceMask |= (byte)(1 << (Int32)num);

            m_sid.set_voice_mask(m_voiceMask);
        }

        // Set the emulated SID model
        public override void model(sidplayfp.SidConfig.sid_model_t model)
        {
            reSID.siddefs.chip_model chipModel;
            switch (model)
            {
                case sidplayfp.SidConfig.sid_model_t.MOS6581:
                    chipModel = reSID.siddefs.chip_model.MOS6581;
                    break;
                case sidplayfp.SidConfig.sid_model_t.MOS8580:
                    chipModel = reSID.siddefs.chip_model.MOS8580;
                    break;
                /* MOS8580 + digi boost
                *      chipModel = (RESID_NS::MOS8580);
                *      m_sid.set_voice_mask(0x0f);
                *      m_sid.input(-32768);
                */
                default:
                    m_status = false;
                    m_error = ERR_INVALID_CHIP;
                    return;
            }

            m_sid.set_chip_model(chipModel);
            m_status = true;
        }

    }

}
