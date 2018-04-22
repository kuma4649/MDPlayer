/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000 Simon White
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

namespace Driver.libsidplayfp.sidplayfp
{
    /**
     * SidTune
     */
    public class SidTune
    {



        //# include <stdint.h>
        //# include <memory>
        //# include "sidplayfp/siddefs.h"

        //class SidTuneInfo;

        //class SidTuneBase;
        //class sidmemory;

        public const Int32 MD5_LENGTH = 32;

        /// Filename extensions to append for various file types.
        //private String[] fileNameExtensions;

        // -------------------------------------------------------------
        private libsidplayfp.sidtune.SidTuneBase tune = new sidtune.SidTuneBase(null);

        private string m_statusString;

        private bool m_status;

        // ----------------------------------------------------------------

        /**
         * Load a sidtune from a file.
         *
         * To retrieve data from standard input pass in filename "-".
         * If you want to override the default filename extensions use this
         * contructor. Please note, that if the specified "fileName"
         * does exist and the loader is able to determine its file format,
         * this function does not try to append any file name extension.
         * See "SidTune.cpp" for the default list of file name extensions.
         * You can specify "fileName = 0", if you do not want to
         * load a sidtune. You can later load one with open().
         *
         * @param fileName
         * @param fileNameExt
         * @param separatorIsSlash
         */
        //public SidTune(string fileName, string[] fileNameExt = null, bool separatorIsSlash = false) { }

        /**
         * Load a single-file sidtune from a memory buffer.
         * Currently supported: PSID and MUS formats.
         *
         * @param oneFileFormatSidtune the buffer that contains song data
         * @param sidtuneLength length of the buffer
         */
        //public SidTune(byte[] oneFileFormatSidtune, UInt32 sidtuneLength) { }

        //~SidTune() { }

        /**
         * The SidTune class does not copy the list of file name extensions,
         * so make sure you keep it. If the provided pointer is 0, the
         * default list will be activated. This is a static list which
         * is used by all SidTune objects.
         *
         * @param fileNameExt
         */
        //public void setFileNameExtensions(string[] fileNameExt) { }

        /**
         * Load a sidtune into an existing object from a file.
         *
         * @param fileName
         * @param separatorIsSlash
         */
        //public void load(string fileName, bool separatorIsSlash = false) { }

        /**
         * Load a sidtune into an existing object from a buffer.
         *
         * @param sourceBuffer the buffer that contains song data
         * @param bufferLen length of the buffer
         */
        //public void read(byte[] sourceBuffer, UInt32 bufferLen) { }

        /**
         * Select sub-song.
         *
         * @param songNum the selected song (0 = default starting song)
         * @return active song number, 0 if no tune is loaded.
         */
        //public UInt32 selectSong(UInt32 songNum) { return 0; }

        /**
         * Retrieve current active sub-song specific information.
         *
         * @return a pointer to #SidTuneInfo, 0 if no tune is loaded. The pointer must not be deleted.
         */
        //public SidTuneInfo getInfo() { return null; }

        /**
         * Select sub-song and retrieve information.
         *
         * @param songNum the selected song (0 = default starting song)
         * @return a pointer to #SidTuneInfo, 0 if no tune is loaded. The pointer must not be deleted.
         */
        //public SidTuneInfo getInfo(UInt32 songNum) { return null; }

        /**
         * Determine current state of object.
         * Upon error condition use #statusString to get a descriptive
         * text string.
         *
         * @return current state (true = okay, false = error)
         */
        //public bool getStatus() { return false; }

        /**
         * Error/status message of last operation.
         */
        //public string statusString() { return null; }

        /**
         * Copy sidtune into C64 memory (64 KB).
         */
        //public bool placeSidTuneInC64mem(ref libsidplayfp.sidmemory mem) { return false; }

        /**
         * Calculates the MD5 hash of the tune.
         * Not providing an md5 buffer will cause the internal one to be used.
         * If provided, buffer must be MD5_LENGTH + 1
         *
         * @return a pointer to the buffer containing the md5 string, 0 if no tune is loaded.
         */
        //public string createMD5(byte[] md5 = null) { return null; }

        //public byte[] c64Data() { return null; }

        // prevent copying
        private SidTune(ref SidTune s) {
        }
        private SidTune opeEquel(ref SidTune s) { return null; }




        /*
        * This file is part of libsidplayfp, a SID player engine.
        *
        * Copyright 2012-2015 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //#include "SidTune.h"
        //#include "sidtune/SidTuneBase.h"
        //#include "sidcxx11.h"

        private string MSG_NO_ERRORS = "No errors";

        // Default sidtune file name extensions. This selection can be overriden
        // by specifying a custom list in the constructor.
        private static string[] defaultFileNameExt = new string[]
{
    // Preferred default file extension for single-file sidtunes
    // or sidtune description files in SIDPLAY INFOFILE format.
    ".sid", ".SID",
    // File extensions used (and created) by various C64 emulators and
    // related utilities. These extensions are recommended to be used as
    // a replacement for ".dat" in conjunction with two-file sidtunes.
    ".c64", ".prg", ".p00", ".C64", ".PRG", ".P00",
    // Stereo Sidplayer (.mus/.MUS ought not be included because
    // these must be loaded first; it sometimes contains the first
    // credit lines of a MUS/STR pair).
    ".str", ".STR", ".mus", ".MUS",
    // End.
    null
};

        private string[] fileNameExtensions = defaultFileNameExt;

        public SidTune(string fileName, string[] fileNameExt = null, bool separatorIsSlash = false)
        {
            setFileNameExtensions(fileNameExt);
            load(fileName, separatorIsSlash);
        }

        public SidTune(byte[] oneFileFormatSidtune, UInt32 sidtuneLength)
        {
            read(oneFileFormatSidtune, sidtuneLength);
        }

        ~SidTune()
        {
            // Needed to delete auto_ptr with complete type
        }

        public void setFileNameExtensions(string[] fileNameExt)
        {
            fileNameExtensions = ((fileNameExt != null) ? fileNameExt : defaultFileNameExt);
        }

        public void load(string fileName, bool separatorIsSlash = false)
        {
            try
            {
                tune = tune.load(fileName, fileNameExtensions, separatorIsSlash);
                m_status = true;
                m_statusString = MSG_NO_ERRORS;
            }
            catch (libsidplayfp.sidtune.loadError e)
            {
                m_status = false;
                m_statusString = e.message();
            }
        }

        public void read(byte[] sourceBuffer, UInt32 bufferLen)
        {
            try
            {
                tune= tune.read(sourceBuffer, bufferLen);
                m_status = true;
                m_statusString = MSG_NO_ERRORS;
            }
            catch (libsidplayfp.sidtune.loadError e)
            {
                m_status = false;
                m_statusString = e.message();
            }
        }

        public UInt32 selectSong(UInt32 songNum)
        {
            return tune != null ? tune.selectSong(songNum) : 0;
        }

        public SidTuneInfo getInfo()
        {
            return tune != null ? tune.getInfo() : null;
        }

        public SidTuneInfo getInfo(UInt32 songNum)
        {
            return tune != null ? tune.getInfo(songNum) : null;
        }

        public bool getStatus() { return m_status; }

        public string statusString() { return m_statusString; }

        public bool placeSidTuneInC64mem(ref libsidplayfp.sidmemory mem)
        {
            if (tune == null)
                return false;

            tune.placeSidTuneInC64mem(ref mem);
            return true;
        }

        public byte[] createMD5(byte[] md5 = null)
        {
            return tune != null ? tune.createMD5(md5) : null;
        }

        public byte c64Data()
        {
            return (byte)(tune != null ? tune.c64Data() : 0);
        }

    }
}