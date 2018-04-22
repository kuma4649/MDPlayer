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
using Driver.libsidplayfp.sidplayfp;

namespace Driver.libsidplayfp.sidtune
{



    //# include <stdint.h>
    //# include <memory>
    //# include <vector>
    //# include <string>
    //# include "sidplayfp/SidTuneInfo.h"
    //# include "sidplayfp/siddefs.h"
    //# include "SmartPtr.h"
    //# include "SidTuneInfoImpl.h"
    //# include "sidcxx11.h"
    //class sidmemory;
    //template<class T> class SmartPtr_sidtt;

    /**
     * loadError
     */
    public class loadError : Exception
    {
        private string m_msg;
        public loadError(string msg) {
            m_msg = msg;
        }
        public string message() { return m_msg; }
    }

    /**
     * SidTuneBaseBase
     */
    public class SidTuneBase
    {
        //typedef std::vector<uint8_t> List<byte>;

        /// Also PSID file format limit.
        public const UInt32 MAX_SONGS = 256;

        // Generic error messages
        //private string ERR_TRUNCATED;
        //private string ERR_INVALID;

        // ----------------------------------------------------------------
        ~SidTuneBase() { }

        /**
         * Load a sidtune from a file.
         *
         * To retrieve data from standard input pass in filename "-".
         * If you want to override the default filename extensions use this
         * contructor. Please note, that if the specified "sidTuneFileName"
         * does exist and the loader is able to determine its file format,
         * this function does not try to append any file name extension.
         * See "SidTune.cpp" for the default list of file name extensions.
         *
         * @param fileName
         * @param fileNameExt
         * @param separatorIsSlash
         * @return the sid tune
         * @throw loadError
         */
        //public SidTuneBase load(string fileName, string[] fileNameExt, bool separatorIsSlash) { return null; }

        /**
         * Load a single-file sidtune from a memory buffer.
         * Currently supported: PSID format
         *
         * @param sourceBuffer
         * @param bufferLen
         * @return the sid tune
         * @throw loadError
         */
        //public SidTuneBase read(byte[] sourceBuffer, UInt32 bufferLen) { return null; }

        /**
         * Select sub-song (0 = default starting song)
         * and return active song number out of [1,2,..,SIDTUNE_MAX_SONGS].
         *
         * @param songNum
         * @return the active song
         */
        //public UInt32 selectSong(UInt32 songNum) { return 0; }

        /**
         * Retrieve sub-song specific information.
         */
        //public SidTuneInfo getInfo() { return null; }

        /**
         * Select sub-song (0 = default starting song)
         * and retrieve active song information.
         *
         * @param songNum
         */
        //public SidTuneInfo getInfo(UInt32 songNum) { return null; }

        /**
         * Copy sidtune into C64 memory (64 KB).
         *
         * @param mem
         */
        //public virtual void placeSidTuneInC64mem(sidmemory mem) { }

        /**
         * Calculates the MD5 hash of the tune.
         * Not providing an md5 buffer will cause the internal one to be used.
         * If provided, buffer must be MD5_LENGTH + 1
         *
         * @return a pointer to the buffer containing the md5 string.
         */
        public virtual byte[] createMD5(byte[] n) { return null; }

        /**
         * Get the pointer to the tune data.
         */
        public byte c64Data() { return cache[(Int32)fileOffset]; }

        // -------------------------------------------------------------

        protected SidTuneInfoImpl info;

        protected byte[] songSpeed = new byte[MAX_SONGS];
        protected Int64[] clockSpeed = new Int64[MAX_SONGS];

        /// For files with header: offset to real data
        protected UInt32 fileOffset;

        protected List<byte> cache;

        //protected SidTuneBase() { }

        /**
         * Does not affect status of object, and therefore can be used
         * to load files. Error string is put into info.statusString, though.
         *
         * @param fileName
         * @param bufferRef
         * @throw loadError
         */
        //protected void loadFile(string fileName, ref List<byte> bufferRef) { }

        /**
         * Convert 32-bit PSID-style speed word to internal tables.
         *
         * @param speed
         * @param clock
         */
        //protected void convertOldStyleSpeedToTables(UInt32 speed, Int64 clock = CLOCK_PAL) { }

        /**
         * Check if compatibility constraints are fulfilled.
         */
        //protected bool checkCompatibility() { return false; }

        /**
         * Check for valid relocation information.
         */
        //protected bool checkRelocInfo() { return false; }

        /**
         * Common address resolution procedure.
         *
         * @param c64data
         */
        //protected void resolveAddrs(byte[] c64data) { }

        /**
         * Cache the data of a single-file or two-file sidtune and its
         * corresponding file names.
         *
         * @param dataFileName
         * @param infoFileName
         * @param buf
         * @param isSlashedFileName If your opendir() and readdir()->d_name return path names
         * that contain the forward slash (/) as file separator, but
         * your operating system uses a different character, there are
         * extra functions that can deal with this special case. Set
         * separatorIsSlash to true if you like path names to be split
         * correctly.
         * You do not need these extra functions if your systems file
         * separator is the forward slash.
         * @throw loadError
         */
        //protected virtual void acceptSidTune(string dataFileName, string infoFileName, ref List<byte> buf, bool isSlashedFileName) { }

        /**
         * Petscii to Ascii converter.
         */
        //protected string petsciiToAscii(ref SmartPtr_sidtt<byte> spPet) { return null; }

        // ---------------------------------------------------------------

        //#if !defined(SIDTUNE_NO_STDIN_LOADER)
        //private SidTuneBase getFromStdIn() { return null; }
        //#endif
        //private SidTuneBase getFromFiles(string name, string[] fileNameExtensions, bool separatorIsSlash) { return null; }

        /**
         * Try to retrieve single-file sidtune from specified buffer.
         */
        //private SidTuneBase getFromBuffer(byte[] buffer, UInt32 bufferLen) { return null; }

        /**
         * Get new file name with specified extension.
         *
         * @param destString destinaton string
         * @param sourceName original file name
         * @param sourceExt new extension
         */
        //private void createNewFileName(ref string destString, byte[] sourceName, byte[] sourceExt) { }

        // prevent copying
        public SidTuneBase(SidTuneBase a) { }
        private SidTuneBase opeEquel(ref SidTuneBase a) { return null; }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2016 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //#include "SidTuneBase.h"
        //#include <cstring>
        //#include <climits>
        //#include <iostream>
        //#include <iomanip>
        //#include <algorithm>
        //#include <iterator>
        //#include <fstream>
        //#include "SmartPtr.h"
        //#include "SidTuneTools.h"
        //#include "SidTuneInfoImpl.h"
        //#include "sidendian.h"
        //#include "sidmemory.h"
        //#include "stringutils.h"
        //#include "MUS.h"
        //#include "p00.h"
        //#include "prg.h"
        //#include "PSID.h"

        // Error and status message strings.
        private string ERR_EMPTY = "SIDTUNE ERROR: No data to load";
        private string ERR_UNRECOGNIZED_FORMAT = "SIDTUNE ERROR: Could not determine file format";
        //private string ERR_CANT_LOAD_FILE = "SIDTUNE ERROR: Could not load input file";
        private string ERR_CANT_OPEN_FILE = "SIDTUNE ERROR: Could not open file for binary input";
        private string ERR_FILE_TOO_LONG = "SIDTUNE ERROR: Input data too long";
        private string ERR_DATA_TOO_LONG = "SIDTUNE ERROR: Size of music data exceeds C64 memory";
        private string ERR_BAD_ADDR = "SIDTUNE ERROR: Bad address data";
        private string ERR_BAD_RELOC = "SIDTUNE ERROR: Bad reloc data";
        private string ERR_CORRUPT = "SIDTUNE ERROR: File is incomplete or corrupt";
        //const char ERR_NOT_ENOUGH_MEMORY[]   = "SIDTUNE ERROR: Not enough free memory";

        public const string ERR_TRUNCATED = "SIDTUNE ERROR: File is most likely truncated";
        public const string ERR_INVALID = "SIDTUNE ERROR: File contains invalid data";

        /**
         * Petscii to Ascii conversion table (0x01 = no output).
         */
        private byte[] CHR_tab = new byte[256]
        {
  0x00,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x0d,0x01,0x01,
  0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
  0x20,0x21,0x01,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2a,0x2b,0x2c,0x2d,0x2e,0x2f,
  0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3a,0x3b,0x3c,0x3d,0x3e,0x3f,
  0x40,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4a,0x4b,0x4c,0x4d,0x4e,0x4f,
  0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5a,0x5b,0x24,0x5d,0x20,0x20,
  // alternative: CHR$(92=0x5c) => ISO Latin-1(0xa3)
  0x2d,0x23,0x7c,0x2d,0x2d,0x2d,0x2d,0x7c,0x7c,0x5c,0x5c,0x2f,0x5c,0x5c,0x2f,0x2f,
  0x5c,0x23,0x5f,0x23,0x7c,0x2f,0x58,0x4f,0x23,0x7c,0x23,0x2b,0x7c,0x7c,0x26,0x5c,
  // 0x80-0xFF
  0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
  0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
  0x20,0x7c,0x23,0x2d,0x2d,0x7c,0x23,0x7c,0x23,0x2f,0x7c,0x7c,0x2f,0x5c,0x5c,0x2d,
  0x2f,0x2d,0x2d,0x7c,0x7c,0x7c,0x7c,0x2d,0x2d,0x2d,0x2f,0x5c,0x5c,0x2f,0x2f,0x23,
  0x2d,0x23,0x7c,0x2d,0x2d,0x2d,0x2d,0x7c,0x7c,0x5c,0x5c,0x2f,0x5c,0x5c,0x2f,0x2f,
  0x5c,0x23,0x5f,0x23,0x7c,0x2f,0x58,0x4f,0x23,0x7c,0x23,0x2b,0x7c,0x7c,0x26,0x5c,
  0x20,0x7c,0x23,0x2d,0x2d,0x7c,0x23,0x7c,0x23,0x2f,0x7c,0x7c,0x2f,0x5c,0x5c,0x2d,
  0x2f,0x2d,0x2d,0x7c,0x7c,0x7c,0x7c,0x2d,0x2d,0x2d,0x2f,0x5c,0x5c,0x2f,0x2f,0x23
        };

        /// The Commodore 64 memory size
        private const UInt32 MAX_MEMORY = 65536;

        /// C64KB + LOAD + PSID
        private const UInt32 MAX_FILELEN = MAX_MEMORY + 2 + 0x7C;

        /// Minimum load address for real c64 only tunes
        private const UInt16 SIDTUNE_R64_MIN_LOAD_ADDR = 0x07e8;

        public SidTuneBase load(string fileName, string[] fileNameExt, bool separatorIsSlash)
        {
            if (fileName == null)
                return null;

#if !SIDTUNE_NO_STDIN_LOADER
            // Filename "-" is used as a synonym for standard input.
            if (fileName == "-") return getFromStdIn();
#endif
            return getFromFiles(fileName, fileNameExt, separatorIsSlash);
        }

        public SidTuneBase read(byte[] sourceBuffer, UInt32 bufferLen)
        {
            return getFromBuffer(sourceBuffer, bufferLen);
        }

        public SidTuneInfo getInfo()
        {
            return info;//.get();
        }

        public SidTuneInfo getInfo(UInt32 songNum)
        {
            selectSong(songNum);
            return info;//.get();
        }

        public UInt32 selectSong(UInt32 selectedSong)
        {
            // Check whether selected song is valid, use start song if not
            UInt32 song = (selectedSong == 0 || selectedSong > info.m_songs) ? info.m_startSong : selectedSong;

            // Copy any song-specific variable information
            // such a speed/clock setting to the info structure.
            info.m_currentSong = song;

            // Retrieve song speed definition.
            switch (info.m_compatibility)
            {
                case SidTuneInfo.compatibility_t.COMPATIBILITY_R64:
                    info.m_songSpeed = SidTuneInfo.SPEED_CIA_1A;
                    break;
                case SidTuneInfo.compatibility_t.COMPATIBILITY_PSID:
                    // This does not take into account the PlaySID bug upon evaluating the
                    // SPEED field. It would most likely break compatibility to lots of
                    // sidtunes, which have been converted from .SID format and vice versa.
                    // The .SID format does the bit-wise/song-wise evaluation of the SPEED
                    // value correctly, like it is described in the PlaySID documentation.
                    info.m_songSpeed = songSpeed[(song - 1) & 31];
                    break;
                default:
                    info.m_songSpeed = songSpeed[song - 1];
                    break;
            }

            info.m_clockSpeed = (SidTuneInfo.clock_t)clockSpeed[song - 1];

            return info.m_currentSong;
        }

        // ------------------------------------------------- private member functions

        public virtual void placeSidTuneInC64mem(ref sidmemory mem)
        {
            // The Basic ROM sets these values on loading a file.
            // Program end address
            UInt16 start = info.m_loadAddr;
            UInt16 end = (UInt16)(start + info.m_c64dataLen);
            mem.writeMemWord(0x2d, end); // Variables start
            mem.writeMemWord(0x2f, end); // Arrays start
            mem.writeMemWord(0x31, end); // Strings start
            mem.writeMemWord(0xac, start);
            mem.writeMemWord(0xae, end);

            // Copy data from cache to the correct destination.
            mem.fillRam(info.m_loadAddr,new Ptr<byte>(cache.ToArray(),(Int32)fileOffset), info.m_c64dataLen);
        }

        protected void loadFile(string fileName, ref List<byte> bufferRef)
        {
            bufferRef = null;
            try
            {
                using (System.IO.FileStream inFile = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    inFile.Seek(0, System.IO.SeekOrigin.End);
                    long fileLen = inFile.Position;
                    if (fileLen < 0) throw new loadError(ERR_EMPTY);
                    inFile.Seek(0, System.IO.SeekOrigin.Begin);

                    byte[] fileBuf = new byte[fileLen];
                    inFile.Read(fileBuf, 0, (Int32)fileLen);
                    bufferRef = new List<byte>(fileBuf);
                }
            }
            catch
            {
                throw new loadError(ERR_CANT_OPEN_FILE);
            }

            //std::ifstream inFile(fileName, std::ifstream::binary);

            //if (!inFile.is_open())
            //{
            //    throw new loadError(ERR_CANT_OPEN_FILE);
            //}

            //inFile.seekg(0, inFile.end);
            ////int fileLen = inFile.tellg();

            //if (fileLen <= 0)
            //{
            //    throw new loadError(ERR_EMPTY);
            //}

            //inFile.seekg(0, inFile.beg);

            ////List<byte> fileBuf;
            //fileBuf.reserve(fileLen);

            //try
            //{
            //    fileBuf.assign(std::istreambuf_iterator<char>(inFile), std::istreambuf_iterator<char>());
            //}
            //catch (Exception ex)
            //{
            //    throw new loadError(ex.Message);
            //}

            //if (inFile.bad())
            //{
            //    throw new loadError(ERR_CANT_LOAD_FILE);
            //}

            //inFile.close();

            //bufferRef.swap(fileBuf);
        }

        protected SidTuneBase()
        {
            info = new SidTuneInfoImpl();
            fileOffset = 0;
            // Initialize the object with some safe defaults.
            for (UInt32 si = 0; si < MAX_SONGS; si++)
            {
                songSpeed[si] = (byte)info.m_songSpeed;
                clockSpeed[si] = (byte)info.m_clockSpeed;
            }
        }

#if !SIDTUNE_NO_STDIN_LOADER

        private SidTuneBase getFromStdIn()
        {
            List<byte> fileBuf = new List<byte>();

            // We only read as much as fits in the buffer.
            // This way we avoid choking on huge data.
            int datb;
            while ((datb = System.Console.Read()) != -1 && fileBuf.Count < MAX_FILELEN)
            {
                fileBuf.Add((byte)datb);
            }

            return getFromBuffer(fileBuf.ToArray(), (UInt32)fileBuf.Count);
        }

#endif

        private SidTuneBase getFromBuffer(byte[] buffer, UInt32 bufferLen)
        {
            if (buffer == null || bufferLen == 0)
            {
                throw new loadError(ERR_EMPTY);
            }

            if (bufferLen > MAX_FILELEN)
            {
                throw new loadError(ERR_FILE_TOO_LONG);
            }

            byte[] buf1 = buffer;//, buffer +bufferLen);

            // Here test for the possible single file formats.
            SidTuneBase s = PSID.load(ref buf1);
            if (s == null) s = (new MUS()).load(buf1.ToArray(), true);
            if (s == null) throw new loadError(ERR_UNRECOGNIZED_FORMAT);

            List<byte> lstBuf1 = new List<byte>(buf1);
            s.acceptSidTune("-", "-", ref lstBuf1, false);
            return s;
        }

        protected virtual void acceptSidTune(string dataFileName, string infoFileName, ref List<byte> buf, bool isSlashedFileName)
        {
            // Make a copy of the data file name and path, if available.
            if (dataFileName != null)
            {
                Int32 fileNamePos = (Int32)(isSlashedFileName ?
                    SidTuneTools.slashedFileNameWithoutPath(dataFileName) :
                    SidTuneTools.fileNameWithoutPath(dataFileName));
                info.m_path = dataFileName.Substring(0, fileNamePos);
                info.m_dataFileName = dataFileName.Substring(fileNamePos);
            }

            // Make a copy of the info file name, if available.
            if (infoFileName != null)
            {
                Int32 fileNamePos = (Int32)(isSlashedFileName ?
                    SidTuneTools.slashedFileNameWithoutPath(infoFileName) :
                    SidTuneTools.fileNameWithoutPath(infoFileName));
                info.m_infoFileName = infoFileName.Substring(fileNamePos);
            }

            // Fix bad sidtune set up.
            if (info.m_songs > MAX_SONGS)
            {
                info.m_songs = MAX_SONGS;
            }
            else if (info.m_songs == 0)
            {
                info.m_songs = 1;
            }

            if (info.m_startSong == 0
                || info.m_startSong > info.m_songs)
            {
                info.m_startSong = 1;
            }

            info.m_dataFileLen = (UInt32)buf.Count;
            info.m_c64dataLen = (UInt32)(buf.Count - fileOffset);

            // Calculate any remaining addresses and then
            // confirm all the file details are correct
            resolveAddrs(buf, (Int32)fileOffset);

            if (checkRelocInfo() == false)
            {
                throw new loadError(ERR_BAD_RELOC);
            }
            if (checkCompatibility() == false)
            {
                throw new loadError(ERR_BAD_ADDR);
            }

            if (info.m_dataFileLen >= 2)
            {
                // We only detect an offset of two. Some position independent
                // sidtunes contain a load address of 0xE000, but are loaded
                // to 0x0FFE and call player at 0x1000.
                info.m_fixLoad = (sidendian.endian_little16(new Ptr<byte>(buf.ToArray(), (Int32)fileOffset)) == (info.m_loadAddr + 2));
            }

            // Check the size of the data.
            if (info.m_c64dataLen > MAX_MEMORY)
            {
                throw new loadError(ERR_DATA_TOO_LONG);
            }
            else if (info.m_c64dataLen == 0)
            {
                throw new loadError(ERR_EMPTY);
            }

            cache = new List<byte>(buf);
        }

        private void createNewFileName(out string destString, byte[] sourceName, byte[] sourceExt)
        {
            destString = Encoding.ASCII.GetString(sourceName);
            destString = destString.Substring(0, destString.LastIndexOf('.'));
            destString += Encoding.ASCII.GetString(sourceExt);
        }

        // Initializing the object based upon what we find in the specified file.

        private SidTuneBase getFromFiles(string fileName, string[] fileNameExtensions, bool separatorIsSlash)
        {
            List<byte> fileBuf1 = new List<byte>();

            loadFile(fileName, ref fileBuf1);

            // File loaded. Now check if it is in a valid single-file-format.
            byte[] aryFileBuf1 = fileBuf1.ToArray();
            SidTuneBase s = PSID.load(ref aryFileBuf1);
            fileBuf1 = new List<byte>(aryFileBuf1);
            if (s == null)
            {
                // Try some native C64 file formats
                s = (new MUS()).load(fileBuf1.ToArray(), true);
                if (s != null)
                {
                    // Try to find second file.
                    string fileName2;
                    int n = 0;
                    while (fileNameExtensions[n] != null)
                    {
                        createNewFileName(out fileName2, Encoding.ASCII.GetBytes(fileName), Encoding.ASCII.GetBytes(fileNameExtensions[n]));
                        // 1st data file was loaded into "fileBuf1",
                        // so we load the 2nd one into "fileBuf2".
                        // Do not load the first file again if names are equal.
                        if (fileName.Substring(0, fileName2.Length) != fileName2.Substring(0, fileName2.Length))
                        {
                            try
                            {
                                List<byte> fileBuf2 = new List<byte>();

                                loadFile(fileName2, ref fileBuf2);
                                // Check if tunes in wrong order and therefore swap them here
                                if (fileNameExtensions[n] == ".mus")
                                {
                                    SidTuneBase s2 = (new MUS()).load(fileBuf2.ToArray(), fileBuf1.ToArray(), 0, true);
                                    if (s2 != null)
                                    {
                                        s2.acceptSidTune(fileName2, fileName, ref fileBuf2, separatorIsSlash);
                                        return s2;
                                    }
                                }
                                else
                                {
                                    SidTuneBase s2 = (new MUS()).load(fileBuf1.ToArray(), true);
                                    if (s2 != null)
                                    {
                                        s2.acceptSidTune(fileName, fileName2, ref fileBuf1, separatorIsSlash);
                                        return s2;
                                    }
                                }
                                // The first tune loaded ok, so ignore errors on the
                                // second tune, may find an ok one later
                            }
                            catch (loadError)
                            {
                            }
                        }
                        n++;
                    }
                }
            }
            if (s == null) s = p00.load(fileName, aryFileBuf1);
            if (s == null) s = prg.load(fileName, aryFileBuf1);
            if (s == null) throw new loadError(ERR_UNRECOGNIZED_FORMAT);

            s.acceptSidTune(fileName, null, ref fileBuf1, separatorIsSlash);
            return s;
        }

        protected void convertOldStyleSpeedToTables(UInt32 speed, Int64 clock = (Int64)SidTuneInfo.clock_t.CLOCK_PAL)
        {
            // Create the speed/clock setting tables.
            //
            // This routine implements the PSIDv2NG compliant speed conversion. All tunes
            // above 32 use the same song speed as tune 32
            // NOTE: The cast here is used to avoid undefined references
            // as the std::min function takes its parameters by reference
            UInt32 toDo = Math.Min(info.m_songs, (UInt32)MAX_SONGS);
            for (UInt32 s = 0; s < toDo; s++)
            {
                clockSpeed[s] = clock;
                songSpeed[s] = (byte)((speed & 1) != 0 ? SidTuneInfo.SPEED_CIA_1A : SidTuneInfo.SPEED_VBI);

                if (s < 31)
                {
                    speed >>= 1;
                }
            }
        }

        protected bool checkRelocInfo()
        {
            // Fix relocation information
            if (info.m_relocStartPage == 0xFF)
            {
                info.m_relocPages = 0;
                return true;
            }
            else if (info.m_relocPages == 0)
            {
                info.m_relocStartPage = 0;
                return true;
            }

            // Calculate start/end page
            byte startp = info.m_relocStartPage;
            byte endp = (byte)((startp + info.m_relocPages - 1) & 0xff);
            if (endp < startp)
            {
                return false;
            }

            {    // Check against load range
                byte startlp = (byte)(info.m_loadAddr >> 8);
                byte endlp = (byte)(startlp + (byte)((info.m_c64dataLen - 1) >> 8));

                if (((startp <= startlp) && (endp >= startlp))
                    || ((startp <= endlp) && (endp >= endlp)))
                {
                    return false;
                }
            }

            // Check that the relocation information does not use the following
            // memory areas: 0x0000-0x03FF, 0xA000-0xBFFF and 0xD000-0xFFFF
            if ((startp < 0x04)
                || ((0xa0 <= startp) && (startp <= 0xbf))
                || (startp >= 0xd0)
                || ((0xa0 <= endp) && (endp <= 0xbf))
                || (endp >= 0xd0))
            {
                return false;
            }

            return true;
        }

        protected void resolveAddrs(List<byte> c64data, Int32 ptr = 0)
        {
            // Originally used as a first attempt at an RSID
            // style format. Now reserved for future use
            if (info.m_playAddr == 0xffff)
            {
                info.m_playAddr = 0;
            }

            // loadAddr = 0 means, the address is stored in front of the C64 data.
            if (info.m_loadAddr == 0)
            {
                if (info.m_c64dataLen < 2)
                {
                    throw new loadError(ERR_CORRUPT);
                }

                info.m_loadAddr = sidendian.endian_16(c64data[ptr+1], c64data[ptr+0]);
                fileOffset += 2;
                info.m_c64dataLen -= 2;
            }

            if (info.m_compatibility == SidTuneInfo.compatibility_t.COMPATIBILITY_BASIC)
            {
                if (info.m_initAddr != 0)
                {
                    throw new loadError(ERR_BAD_ADDR);
                }
            }
            else if (info.m_initAddr == 0)
            {
                info.m_initAddr = info.m_loadAddr;
            }
        }

        protected bool checkCompatibility()
        {
            if (info.m_compatibility == SidTuneInfo.compatibility_t.COMPATIBILITY_R64)
            {
                // Check valid init address
                switch (info.m_initAddr >> 12)
                {
                    case 0x0A:
                    case 0x0B:
                    case 0x0D:
                    case 0x0E:
                    case 0x0F:
                        return false;
                    default:
                        if ((info.m_initAddr < info.m_loadAddr)
                            || (info.m_initAddr > (info.m_loadAddr + info.m_c64dataLen - 1)))
                        {
                            return false;
                        }
                        break;
                }

                // Check tune is loadable on a real C64
                if (info.m_loadAddr < SIDTUNE_R64_MIN_LOAD_ADDR)
                {
                    return false;
                }
            }

            return true;
        }

        protected string petsciiToAscii(ref Ptr<byte> spPet)
        {
            List<byte> buffer = new List<byte>();

            do
            {
                byte petsciiChar = spPet.buf[spPet.ptr];
                spPet.AddPtr(1);

                if ((petsciiChar == 0x00) || (petsciiChar == 0x0d))
                    break;

                // If character is 0x9d (left arrow key) then move back.
                if ((petsciiChar == 0x9d) && buffer.Count != 0)
                {
                    buffer.RemoveAt(buffer.Count - 1);
                }
                else
                {
                    // ASCII CHR$ conversion
                    byte asciiChar = CHR_tab[petsciiChar];
                    if ((asciiChar >= 0x20) && (buffer.Count <= 31))
                        buffer.Add(asciiChar);
                }
            }
            while (spPet.buf.Length > spPet.ptr);

            return Encoding.ASCII.GetString(buffer.ToArray());
        }

    }

}