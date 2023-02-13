/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 *  Copyright 2011-2017 Leandro Nini
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

namespace Driver.libsidplayfp.sidplayfp
{
    /**
     * This interface is used to get sid engine informations.
     */
    public class SidInfo
    {



        //# include <stdint.h>
        //# include "sidplayfp/siddefs.h"

        /// Library name
        //public string name() { return null; }

        /// Library version
        //public string version() { return null; }

        /// Library credits
        //@{
        //public UInt32 numberOfCredits() { return 0; }
        //public string credits(UInt32 i) { return null; }
        //@}

        /// Number of SIDs supported by this library
        //public UInt32 maxsids() { return 0; }

        /// Number of output channels (1-mono, 2-stereo)
        //public UInt32 channels() { return 0; }

        /// Address of the driver
        //public UInt16 driverAddr() { return 0; }

        /// Size of the driver in bytes
        //public UInt16 driverLength() { return 0; }

        /// Describes the speed current song is running at
        //public string speedString() { return null; }

        /// Description of the laoded ROM images
        //@{
        //public string kernalDesc() { return null; }
        //public string basicDesc() { return null; }
        //public string chargenDesc() { return null; }
        //@}


        public virtual string getName() { return null; }

        public virtual string getVersion() { return null; }

        public virtual UInt32 getNumberOfCredits() { return 0; }
        public virtual string getCredits(UInt32 i) { return null; }

        public virtual UInt32 getMaxsids() { return 0; }

        public virtual UInt32 getChannels() { return 0; }

        public virtual UInt16 getDriverAddr() { return 0; }

        public virtual UInt16 getDriverLength() { return 0; }

        public virtual string getSpeedString() { return null; }

        public virtual string getKernalDesc() { return null; }
        public virtual string getBasicDesc() { return null; }
        public virtual string getChargenDesc() { return null; }


        ~SidInfo() { }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 *  Copyright 2011-2017 Leandro Nini
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

        //# include "SidInfo.h"


        public string name() { return getName(); }

        public string version() { return getVersion(); }

        public UInt32 numberOfCredits() { return getNumberOfCredits(); }
        public string credits(UInt32 i) { return getCredits(i); }

        public UInt32 maxsids() { return getMaxsids(); }

        public UInt32 channels() { return getChannels(); }

        public UInt16 driverAddr() { return getDriverAddr(); }

        public UInt16 driverLength() { return getDriverLength(); }

        public string speedString() { return getSpeedString(); }

        public string kernalDesc() { return getKernalDesc(); }
        public string basicDesc() { return getBasicDesc(); }
        public string chargenDesc() { return getChargenDesc(); }

    }
}