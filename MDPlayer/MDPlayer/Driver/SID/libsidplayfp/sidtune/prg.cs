/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2015 Leandro Nini <drfiemost@users.sourceforge.net>
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
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
using Driver.libsidplayfp.sidplayfp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.sidtune
{
    public sealed class prg : SidTuneBase
    {



        //# include "SidTuneBase.h"
        //# include "sidcxx11.h"

        prg() { }

        //private void load() { }

        /**
         * @return pointer to a SidTune or 0 if not a prg file
         * @throw loadError if prg file is corrupt
         */
        //public static SidTuneBase load(string fileName, byte[] dataBuf) { return null; }

        ~prg() { }

        // prevent copying
        prg(ref prg p) { }
        private prg opeEquel(ref prg p) { return null; }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
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
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

        //#include "prg.h"
        //#include <memory>
        //#include "sidplayfp/SidTuneInfo.h"
        //#include "SidTuneTools.h"
        //#include "stringutils.h"

        // Format strings
        public const string TXT_FORMAT_PRG = "Tape image file (PRG)";

        public static SidTuneBase load(string fileName, byte[] dataBuf)
        {
            string ext = SidTuneTools.fileExtOfPath(fileName);
            if ((ext != ".prg")
                && (ext != ".c64"))
            {
                return null;
            }

            if (dataBuf.Length < 2)
            {
                throw new loadError(ERR_TRUNCATED);
            }

            prg tune = new prg();
            tune.load();

            return tune;
        }

        private void load()
        {
            info.m_formatString = TXT_FORMAT_PRG;

            // Automatic settings
            info.m_songs = 1;
            info.m_startSong = 1;
            info.m_compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_BASIC;

            // Create the speed/clock setting table.
            convertOldStyleSpeedToTables((UInt32)0xffffffff, (Int64)info.m_clockSpeed);// ~0, info.m_clockSpeed);
        }

    }

}