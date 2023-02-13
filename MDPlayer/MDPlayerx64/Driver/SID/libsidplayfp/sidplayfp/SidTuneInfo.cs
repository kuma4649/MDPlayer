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
     * This interface is used to get values from SidTune objects.
     *
     * You must read (i.e. activate) sub-song specific information
     * via:
     *        const SidTuneInfo* tuneInfo = SidTune.getInfo();
     *        const SidTuneInfo* tuneInfo = SidTune.getInfo(songNumber);
     */
    public class SidTuneInfo
    {



        //# include <stdint.h>
        //# include "sidplayfp/siddefs.h"

        public enum clock_t {
            CLOCK_UNKNOWN,
            CLOCK_PAL,
            CLOCK_NTSC,
            CLOCK_ANY
        }

        public enum model_t {
            SIDMODEL_UNKNOWN,
            SIDMODEL_6581,
            SIDMODEL_8580,
            SIDMODEL_ANY
        }

        public enum compatibility_t {
            COMPATIBILITY_C64,   ///< File is C64 compatible
            COMPATIBILITY_PSID,  ///< File is PSID specific
            COMPATIBILITY_R64,   ///< File is Real C64 only
            COMPATIBILITY_BASIC  ///< File requires C64 Basic
        }

        /// Vertical-Blanking-Interrupt
        public const Int32 SPEED_VBI = 0;

        /// CIA 1 Timer A
        public const int SPEED_CIA_1A = 60;

        /**
         * Load Address.
         */
        //public UInt16 loadAddr() { return 0; }

        /**
         * Init Address.
         */
        //public UInt16 initAddr() { return 0; }

        /**
         * Play Address.
         */
        //public UInt16 playAddr() { return 0; }

        /**
         * The number of songs.
         */
        //public UInt32 songs() { return 0; }

        /**
         * The default starting song.
         */
        //public UInt32 startSong() { return 0; }

        /**
         * The tune that has been initialized.
         */
        //public UInt32 currentSong() { return 0; }

        /**
         * @name Base addresses
         * The SID chip base address(es) used by the sidtune.
         * - 0xD400 for the 1st SID
         * - 0 if the nth SID is not required
         */
        //public UInt16 sidChipBase(UInt32 i) { return 0; }

        /**
         * The number of SID chips required by the tune.
         */
        //public Int32 sidChips() { return 0; }

        /**
         * Intended speed.
         */
        //public Int32 songSpeed() { return 0; }

        /**
         * First available page for relocation.
         */
        // public byte relocStartPage() { return 0; }

        /**
         * Number of pages available for relocation.
         */
        //public byte relocPages() { return 0; }

        /**
         * @name SID model
         * The SID chip model(s) requested by the sidtune.
         */
        //public model_t sidModel(UInt32 i) { return 0; }

        /**
         * Compatibility requirements.
         */
        //public compatibility_t compatibility() { return 0; }

        /**
         * @name Tune infos
         * Song title, credits, ...
         * - 0 = Title
         * - 1 = Author
         * - 2 = Released
         */
        //@{
        //public UInt32 numberOfInfoStrings() { return 0; }     ///< The number of available text info lines
        //public string infoString(UInt32 i) { return null; } ///< Text info from the format headers etc.
        //@}

        /**
         * @name Tune comments
         * MUS comments.
         */
        //@{
        //public UInt32 numberOfCommentStrings() { return 0; }     ///< Number of comments
        //public string commentString(UInt32 i) { return null; } ///< Used to stash the MUS comment somewhere
        //@}

        /**
         * Length of single-file sidtune file.
         */
        //public UInt32 dataFileLen() { return 0; }

        /**
         * Length of raw C64 data without load address.
         */
        //public UInt32 c64dataLen() { return 0; }

        /**
         * The tune clock speed.
         */
        //public clock_t clockSpeed() { return 0; }

        /**
         * The name of the identified file format.
         */
        //public string formatString() { return null; }

        /**
         * Whether load address might be duplicate.
         */
        //public bool fixLoad() { return false; }

        /**
         * Path to sidtune files.
         */
        //public string path() { return null; }

        /**
         * A first file: e.g. "foo.sid" or "foo.mus".
         */
        //public string dataFileName() { return null; }

        /**
         * A second file: e.g. "foo.str".
         * Returns 0 if none.
         */
        //public string infoFileName() { return null; }


        public virtual UInt16 getLoadAddr() { return 0; }

        public virtual UInt16 getInitAddr() { return 0; }

        public virtual UInt16 getPlayAddr() { return 0; }

        public virtual UInt32 getSongs() { return 0; }

        public virtual UInt32 getStartSong() { return 0; }

        public virtual UInt32 getCurrentSong() { return 0; }

        public virtual UInt16 getSidChipBase(UInt32 i) { return 0; }

        public virtual Int32 getSidChips() { return 0; }

        public virtual Int32 getSongSpeed() { return 0; }

        public virtual byte getRelocStartPage() { return 0; }

        public virtual byte getRelocPages() { return 0; }

        public virtual model_t getSidModel(UInt32 i) { return 0; }

        public virtual compatibility_t getCompatibility() { return 0; }

        public virtual UInt32 getNumberOfInfoStrings() { return 0; }
        public virtual string getInfoString(UInt32 i) { return null; }

        public virtual UInt32 getNumberOfCommentStrings() { return 0; }
        public virtual string getCommentString(UInt32 i) { return null; }

        public virtual UInt32 getDataFileLen() { return 0; }

        public virtual UInt32 getC64dataLen() { return 0; }

        public virtual clock_t getClockSpeed() { return 0; }

        public virtual string getFormatString() { return null; }

        public virtual bool getFixLoad() { return false; }

        public virtual string getPath() { return null; }

        public virtual string getDataFileName() { return null; }

        public virtual string getInfoFileName() { return null; }

        ~SidTuneInfo() { }




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

        //# include "SidTuneInfo.h"

        public UInt16 loadAddr() {
            return getLoadAddr();
        }

        public UInt16 initAddr() {
            return getInitAddr();
        }

        public UInt16 playAddr()
        {
            return getPlayAddr();
        }

        public UInt32 songs()
        {
            return getSongs();
        }

        public UInt32 startSong()
        {
            return getStartSong();
        }

        public UInt32 currentSong()
        {
            return getCurrentSong();
        }

        public UInt16 sidChipBase(UInt32 i)
        {
            return getSidChipBase(i);
        }

        public Int32 sidChips()
        {
            return getSidChips();
        }

public Int32 songSpeed()
        {
            return getSongSpeed();
        }

        public byte relocStartPage()
        {
            return getRelocStartPage();
        }

        public byte relocPages()
        {
            return getRelocPages();
        }

        public model_t sidModel(UInt32 i)
        {
            return getSidModel(i);
        }

        public compatibility_t compatibility()
        {
            return getCompatibility();
        }

        public UInt32 numberOfInfoStrings()
        {
            return getNumberOfInfoStrings();
        }
        public string infoString(UInt32 i)
        {
            return getInfoString(i);
        }


        public UInt32 numberOfCommentStrings()
        {
            return getNumberOfCommentStrings();
        }
        public string commentString(UInt32 i)
        {
            return getCommentString(i);
        }

        public UInt32 dataFileLen()
        {
            return getDataFileLen();
        }

        public UInt32 c64dataLen()
        {
            return getC64dataLen();
        }

        public clock_t clockSpeed()
        {
            return getClockSpeed();
        }

        public string formatString()
        {
            return getFormatString();
        }

        public bool fixLoad()
        {
            return getFixLoad();
        }

        public string path()
        {
            return getPath();
        }

        public string dataFileName()
        {
            return getDataFileName();
        }

        public string infoFileName()
        {
            return getInfoFileName();
        }




    }
}