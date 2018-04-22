/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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
    /**
     * ReSID Builder Class
     */
    public class ReSIDBuilder : sidplayfp.sidbuilder
    {



        //# include "sidplayfp/sidbuilder.h"
        //# include "sidplayfp/siddefs.h"

        public ReSIDBuilder(string name) : base(name)
        {
        }
        //~ReSIDBuilder() { }

        /**
         * Available sids.
         *
         * @return the number of available sids, 0 = endless.
         */
        public override UInt32 availDevices()
        {
            return 0;
        }

        //public override UInt32 create(UInt32 sids){ return 0; }

        //public override string credits() { return ""; }

        /// @name global settings
        /// Settings that affect all SIDs
        //@{
        /**
         * enable/disable filter.
         */
        //public override void filter(bool enable) { }

        /**
         * The bias is given in millivolts, and a maximum reasonable
         * control range is approximately -500 to 500.
         */
        //public void bias(double dac_bias) { }
        //@}




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //# include "resid.h"
        //# include <algorithm>
        //# include <new>
        //# include "resid-emu.h"

        ~ReSIDBuilder()
        {   // Remove all SID emulations
            remove();
        }

        // Create a new sid emulation.
        public override UInt32 create(UInt32 sids)
        {
            m_status = true;

            // Check available devices
            UInt32 count = availDevices();

            if (count != 0 && (count < sids))
                sids = count;

            for (count = 0; count < sids; count++)
            {
                try
                {
                    sidobjs.Add(new ReSID(this));
                }
                // Memory alloc failed?
                catch //(bad_alloc )
                {
                    m_errorBuffer = name() + " ERROR: Unable to create ReSID object";
                    m_status = false;
                    break;
                }
            }
            return count;
        }

        public override string credits()
        {
            return ReSID.getCredits();
        }

        public override void filter(bool enable)
        {
            //std::for_each(sidobjs.begin(), sidobjs.end(), applyParameter<libsidplayfp::ReSID, bool>(&libsidplayfp::ReSID::filter, enable));
            foreach(sidemu o in sidobjs)
            {
                //applyParameter_LibsidplayfpReSID_bool ap = new applyParameter_LibsidplayfpReSID_bool(((ReSID)o).filter, enable);
                ((ReSID)o).filter(enable);//ようはこういうこと？
            }
        }

        public void bias(double dac_bias)
        {
            //std::for_each(sidobjs.begin(), sidobjs.end(), applyParameter<libsidplayfp::ReSID, double>(&libsidplayfp::ReSID::bias, dac_bias));
            foreach (sidemu o in sidobjs)
            {
                //applyParameter_LibsidplayfpReSID_double ap = new applyParameter_LibsidplayfpReSID_double(((ReSID)o).bias, dac_bias);
                ((ReSID)o).bias(dac_bias);//ようはこういうこと？
            }
        }

    }
}
