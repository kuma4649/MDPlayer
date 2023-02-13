//  ---------------------------------------------------------------------------
//  This file is part of reSID, a MOS6581 SID emulator engine.
//  Copyright (C) 2010  Dag Lem <resid@nimrod.no>
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//  ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.builders.resid_builder.reSID
{

    //# include "resid-config.h"


    // ----------------------------------------------------------------------------
    // The audio output stage in a Commodore 64 consists of two STC networks,
    // a low-pass filter with 3-dB frequency 16kHz followed by a high-pass
    // filter with 3-dB frequency 1.6Hz (the latter provided an audio equipment
    // input impedance of 10kOhm).
    // The STC networks are connected with a BJT supposedly meant to act as
    // a unity gain buffer, which is not really how it works. A more elaborate
    // model would include the BJT, however DC circuit analysis yields BJT
    // base-emitter and emitter-base impedances sufficiently low to produce
    // additional low-pass and high-pass 3dB-frequencies in the order of hundreds
    // of kHz. This calls for a sampling frequency of several MHz, which is far
    // too high for practical use.
    // ----------------------------------------------------------------------------
    public class ExternalFilter
    {
        //public ExternalFilter() { }

        //public void enable_filter(bool enable) { }

        //public void clock(Int16 Vi) { }
        //public void clock(Int32 delta_t, Int16 Vi) { }
        //public void reset() { }

        // Audio output (16 bits).
        //public short output() { return 0; }


        // Filter enabled.
        protected bool enabled;

        // State of filters (27 bits).
        protected Int32 Vlp; // lowpass
        protected Int32 Vhp; // highpass

        // Cutoff frequencies.
        protected Int32 w0lp_1_s7;
        protected Int32 w0hp_1_s17;

        //friend class SID;


        // ----------------------------------------------------------------------------
        // Inline functions.
        // The following functions are defined inline because they are called every
        // time a sample is calculated.
        // ----------------------------------------------------------------------------

        //#if RESID_INLINING || defined(RESID_EXTFILT_CC)

        // ----------------------------------------------------------------------------
        // SID clocking - 1 cycle.
        // ----------------------------------------------------------------------------
        public void clock(Int16 Vi)
        {
            // This is handy for testing.
            if (!enabled)
            {
                // Vo  = Vlp - Vhp;
                Vlp = Vi << 11;
                Vhp = 0;
                return;
            }

            // Calculate filter outputs.
            // Vlp = Vlp + w0lp*(Vi - Vlp)*delta_t;
            // Vhp = Vhp + w0hp*(Vlp - Vhp)*delta_t;
            // Vo  = Vlp - Vhp;

            int dVlp = w0lp_1_s7 * ((Vi << 11) - Vlp) >> 7;
            int dVhp = w0hp_1_s17 * (Vlp - Vhp) >> 17;
            Vlp += dVlp;
            Vhp += dVhp;
        }

        // ----------------------------------------------------------------------------
        // SID clocking - delta_t cycles.
        // ----------------------------------------------------------------------------
        public void clock(Int32 delta_t, Int16 Vi)
        {
            // This is handy for testing.
            if (!enabled)
            {
                // Vo  = Vlp - Vhp;
                Vlp = Vi << 11;
                Vhp = 0;
                return;
            }

            // Maximum delta cycles for the external filter to work satisfactorily
            // is approximately 8.
            Int32 delta_t_flt = 8;

            while (delta_t != 0)
            {
                if (delta_t < delta_t_flt)
                {
                    delta_t_flt = delta_t;
                }

                // Calculate filter outputs.
                // Vlp = Vlp + w0lp*(Vi - Vlp)*delta_t;
                // Vhp = Vhp + w0hp*(Vlp - Vhp)*delta_t;
                // Vo  = Vlp - Vhp;

                int dVlp = (w0lp_1_s7 * delta_t_flt >> 3) * ((Vi << 11) - Vlp) >> 4;
                int dVhp = (w0hp_1_s17 * delta_t_flt >> 3) * (Vlp - Vhp) >> 14;
                Vlp += dVlp;
                Vhp += dVhp;

                delta_t -= delta_t_flt;
            }
        }


        // ----------------------------------------------------------------------------
        // Audio output (16 bits).
        // ----------------------------------------------------------------------------
        const Int32 half = 1 << 15;
        public short output()
        {
            // Saturated arithmetics to guard against 16 bit sample overflow.
            Int32 Vo = (Vlp - Vhp) >> 11;
            if (Vo >= half)
            {
                Vo = half - 1;
            }
            else if (Vo < -half)
            {
                Vo = -half;
            }
            return (Int16)Vo;
        }

        //#endif // RESID_INLINING || defined(RESID_EXTFILT_CC)





        //  ---------------------------------------------------------------------------
        //  This file is part of reSID, a MOS6581 SID emulator engine.
        //  Copyright (C) 2010  Dag Lem <resid@nimrod.no>
        //
        //  This program is free software; you can redistribute it and/or modify
        //  it under the terms of the GNU General Public License as published by
        //  the Free Software Foundation; either version 2 of the License, or
        //  (at your option) any later version.
        //
        //  This program is distributed in the hope that it will be useful,
        //  but WITHOUT ANY WARRANTY; without even the implied warranty of
        //  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        //  GNU General Public License for more details.
        //
        //  You should have received a copy of the GNU General Public License
        //  along with this program; if not, write to the Free Software
        //  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
        //  ---------------------------------------------------------------------------

        //# include "extfilt.h"

        // ----------------------------------------------------------------------------
        // Constructor.
        // ----------------------------------------------------------------------------
        public ExternalFilter()
        {
            reset();
            enable_filter(true);

            // Low-pass:  R = 10 kOhm, C = 1000 pF; w0l = 1/RC = 1/(1e4*1e-9) = 100 000
            // High-pass: R =  1 kOhm, C =   10 uF; w0h = 1/RC = 1/(1e3*1e-5) =     100

            // Assume a 1MHz clock.
            // Cutoff frequency accuracy (4 bits) is traded off for filter signal
            // accuracy (27 bits). This is crucial since w0lp and w0hp are so far apart.
            w0lp_1_s7 = (Int32)(100000 * 1.0e-6 * (1 << 7) + 0.5);
            w0hp_1_s17 = (Int32)(100 * 1.0e-6 * (1 << 17) + 0.5);
        }


        // ----------------------------------------------------------------------------
        // Enable filter.
        // ----------------------------------------------------------------------------
        public void enable_filter(bool enable)
        {
            enabled = enable;
        }


        // ----------------------------------------------------------------------------
        // SID reset.
        // ----------------------------------------------------------------------------
        public void reset()
        {
            // State of filter.
            Vlp = 0;
            Vhp = 0;
        }


    }
}