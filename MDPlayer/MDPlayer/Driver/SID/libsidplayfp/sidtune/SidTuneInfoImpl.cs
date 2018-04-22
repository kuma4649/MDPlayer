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
using Driver.libsidplayfp.sidplayfp;

namespace Driver.libsidplayfp.sidtune
{
    /**
     * The implementation of the SidTuneInfo interface.
     */
    public sealed class SidTuneInfoImpl : SidTuneInfo
    {



        //# include <stdint.h>
        //# include <vector>
        //# include <string>
        //# include "sidplayfp/SidTuneInfo.h"
        //# include "sidcxx11.h"

        public string m_formatString;

        public UInt32 m_songs;
        public UInt32 m_startSong;
        public UInt32 m_currentSong;

        public Int32 m_songSpeed;

        public clock_t m_clockSpeed;

        public compatibility_t m_compatibility;

        public UInt32 m_dataFileLen;

        public UInt32 m_c64dataLen;

        public UInt16 m_loadAddr;
        public UInt16 m_initAddr;
        public UInt16 m_playAddr;

        public byte m_relocStartPage;

        public byte m_relocPages;

        public string m_path;

        public string m_dataFileName;

        public string m_infoFileName;

        public List<model_t> m_sidModels;//vector

        public List<UInt16> m_sidChipAddresses;//vector

        public List<string> m_infoString=new List<string>();//vector

        public List<string> m_commentString;//vector

        public bool m_fixLoad;

        // prevent copying
        private SidTuneInfoImpl(ref SidTuneInfoImpl s) { }
        private SidTuneInfoImpl opeEquel(ref SidTuneInfoImpl s) { return null; }

        public SidTuneInfoImpl()
        {
            m_formatString = "N/A";
            m_songs = 0;
            m_startSong = 0;
            m_currentSong = 0;
            m_songSpeed = SPEED_VBI;
            m_clockSpeed = clock_t.CLOCK_UNKNOWN;
            m_compatibility = compatibility_t.COMPATIBILITY_C64;
            m_dataFileLen = 0;
            m_c64dataLen = 0;
            m_loadAddr = 0;
            m_initAddr = 0;
            m_playAddr = 0;
            m_relocStartPage = 0;
            m_relocPages = 0;
            m_fixLoad = false;

            m_sidModels = new List<model_t>();
            m_sidModels.Add(model_t.SIDMODEL_UNKNOWN);
            m_sidChipAddresses = new List<UInt16>();
            m_sidChipAddresses.Add(0xd400);
        }

        public override UInt16 getLoadAddr() { return m_loadAddr; }

        public override UInt16 getInitAddr() { return m_initAddr; }

        public override UInt16 getPlayAddr() { return m_playAddr; }

        public override UInt32 getSongs() { return m_songs; }

        public override UInt32 getStartSong() { return m_startSong; }

        public override UInt32 getCurrentSong() { return m_currentSong; }

        public override UInt16 getSidChipBase(UInt32 i)
        {
            return (UInt16)(i < m_sidChipAddresses.Count ? m_sidChipAddresses[(Int32)i] : 0);
        }

        public override int getSidChips() { return m_sidChipAddresses.Count; }

        public override int getSongSpeed() { return m_songSpeed; }

        public override byte getRelocStartPage() { return m_relocStartPage; }

        public override byte getRelocPages() { return m_relocPages; }

        public override model_t getSidModel(UInt32 i)
        {
            return i < m_sidModels.Count ? m_sidModels[(Int32)i] : model_t.SIDMODEL_UNKNOWN;
        }

        public override compatibility_t getCompatibility() { return m_compatibility; }

        public override UInt32 getNumberOfInfoStrings() { return (UInt32)m_infoString.Count; }
        public override string getInfoString(UInt32 i) { return i < getNumberOfInfoStrings() ? m_infoString[(Int32)i] : ""; }

        public override UInt32 getNumberOfCommentStrings() { return (UInt32)m_commentString.Count; }
        public override string getCommentString(UInt32 i) { return i < getNumberOfCommentStrings() ? m_commentString[(Int32)i] : ""; }

        public override UInt32 getDataFileLen() { return m_dataFileLen; }

        public override UInt32 getC64dataLen() { return m_c64dataLen; }

        public override clock_t getClockSpeed() { return m_clockSpeed; }

        public override string getFormatString() { return m_formatString; }

        public override bool getFixLoad() { return m_fixLoad; }

        public override string getPath() { return m_path; }

        public override string getDataFileName() { return m_dataFileName; }

        public override string getInfoFileName() { return m_infoFileName != "" ? m_infoFileName : null; }





    }
}