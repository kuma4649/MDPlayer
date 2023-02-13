/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 *  Copyright 2011-2015 Leandro Nini
 *  Copyright 2007-2010 Antti Lankila
 *  Copyright 2000 Simon White
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp
{
    /**
     * The implementation of the SidInfo interface.
     */
    public sealed class SidInfoImpl : Driver.libsidplayfp.sidplayfp.SidInfo
    {




        //# include <stdint.h>
        //# include <vector>
        //# include <string>
        //# include "sidplayfp/SidInfo.h"
        //# include "mixer.h"
        //# include "sidcxx11.h"


        //# ifndef PACKAGE_NAME
        //#define PACKAGE_NAME PACKAGE
        //#endif

        //# ifndef PACKAGE_VERSION
        //#define PACKAGE_VERSION VERSION
        //#endif


        public string m_name;
        public string m_version;
        public List<string> m_credits=new List<string>();

        public string m_speedString;

        public string m_kernalDesc;
        public string m_basicDesc;
        public string m_chargenDesc;

        public UInt32 m_maxsids;

        public UInt32 m_channels;

        public UInt16 m_driverAddr;
        public UInt16 m_driverLength;


        // prevent copying
        private SidInfoImpl(ref SidInfoImpl s) { }
        private SidInfoImpl opeEquel(ref SidInfoImpl s) { return null; }


        public SidInfoImpl()
        {
            m_name = Const.PACKAGE_NAME;
            m_version = Const.PACKAGE_VERSION;
            m_maxsids = (Mixer.MAX_SIDS);
            m_channels = (1);
            m_driverAddr = (0);
            m_driverLength = (0);

            m_credits.Add(Const.PACKAGE_NAME + " V" + Const.PACKAGE_VERSION + " Engine:\n"
                + "\tCopyright (C) 2000 Simon White\n"
                + "\tCopyright (C) 2007-2010 Antti Lankila\n"
                + "\tCopyright (C) 2010-2015 Leandro Nini\n"
                + "\t" + Const.PACKAGE_URL + "\n");
        }

        public override string getName() { return m_name; }
        public override string getVersion() { return m_version; }

        public override UInt32 getNumberOfCredits() { return (UInt32)m_credits.Count; }
        public override string getCredits(UInt32 i) { return i < m_credits.Count ? m_credits[(int)i] : ""; }

        public override UInt32 getMaxsids() { return m_maxsids; }

        public override UInt32 getChannels() { return m_channels; }

        public override UInt16 getDriverAddr() { return m_driverAddr; }
        public override UInt16 getDriverLength() { return m_driverLength; }

        public override string getSpeedString() { return m_speedString; }

        public override string getKernalDesc() { return m_kernalDesc; }
        public override string getBasicDesc() { return m_basicDesc; }
        public override string getChargenDesc() { return m_chargenDesc; }





    }
}