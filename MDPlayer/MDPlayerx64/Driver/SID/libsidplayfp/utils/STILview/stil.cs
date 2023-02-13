/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 1998, 2002 by LaLa <LaLa@C64.org>
 * Copyright 2012-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.utils.STILview
{
    public class STIL
    {




        //# include <string>
        //# include <algorithm>
        //# include <map>
        //# include <iosfwd>
        //# include "stildefs.h"

        /**
         * STIL class
         *
         * @author LaLa <LaLa@C64.org>
         * @copyright 1998, 2002 by LaLa
         *
         *
         * Given the location of HVSC this class can extract STIL information for a
         * given tune of a given SID file. (Sounds simple, huh?)
         *
         * PLEASE, READ THE ACCOMPANYING README.TXT FILE BEFORE PROCEEDING!!!!
         */

        /// Enum to use for asking for specific fields.
        public enum STILField
        {
            all,
            name,
            author,
            title,
            artist,
            comment
        };

        /// Enum that describes the possible errors this class may encounter.
        public enum STILerror
        {
            NO_STIL_ERROR = 0,
            BUG_OPEN,           ///< INFO ONLY: failed to open BUGlist.txt.
            WRONG_DIR,          ///< INFO ONLY: path was not within HVSC base dir.
            NOT_IN_STIL,        ///< INFO ONLY: requested entry was not found in STIL.txt.
            NOT_IN_BUG,         ///< INFO ONLY: requested entry was not found in BUGlist.txt.
            WRONG_ENTRY,        ///< INFO ONLY: section-global comment was asked for with get*Entry().
            CRITICAL_STIL_ERROR = 10,
            BASE_DIR_LENGTH,    ///< The length of the HVSC base dir was wrong (empty string?)
            STIL_OPEN,          ///< Failed to open STIL.txt.
            NO_EOL,             ///< Failed to determine EOL char(s).
            NO_STIL_DIRS,       ///< Failed to get sections (subdirs) when parsing STIL.txt.
            NO_BUG_DIRS         ///< Failed to get sections (subdirs) when parsing BUGlist.txt.
        };

        /// To turn debug output on
        public bool STIL_DEBUG;

        //----//

        /**
         * Allocates necessary memory.
         *
         * @param stilPath relative path to STIL file
         * @param bugsPath relative path to BUG file
         */
        //public STIL(string stilPath = stildefs.DEFAULT_PATH_TO_STIL, string bugsPath = stildefs.DEFAULT_PATH_TO_BUGLIST) { }

        /**
         * Returns a formatted string telling what the version
         * number is for the STIL class and other info.
         * If it is called after setBaseDir(), the string also
         * has the STIL.txt file's version number in it.
         *
         * @return
         *     printable formatted string with version and copyright
         *     info
         *     (It's kinda dangerous to return a pointer that points
         *     to an internal structure, but I trust you. :)
         */
        //public string getVersion() { return null; }

        /**
         * Returns a floating number telling what the version
         * number is of this STIL class.
         *
         * @return
         *     version number
         */
        //public float getVersionNo() { return 0; }

        /**
         * Tell the object where the HVSC base directory is - it
         * figures that the STIL should be in /DOCUMENTS/STIL.txt
         * and that the BUGlist should be in /DOCUMENTS/BUGlist.txt.
         * It should not matter whether the path is given in UNIX,
         * WinDOS, or Mac format (ie. '\' vs. '/' vs. ':')
         *
         * @param  pathToHVSC = HVSC base directory in your machine's format
         * @return
         *      - false - Problem opening or parsing STIL/BUGlist
         *      - true  - All okay
         */
        //public bool setBaseDir(string pathToHVSC) { return false; }

        /**
         * Returns a floating number telling what the version
         * number is of the STIL.txt file.
         * To be called only after setBaseDir()!
         *
         * @return
         *     version number (0.0 if setBaseDir() was not called, yet)
         */
        //public float getSTILVersionNo() { return 0; }

        /**
         * Given an HVSC pathname, a tune number and a
         * field designation, it returns a formatted string that
         * contains the STIL field for the tune number (if exists).
         * If it doesn't exist, returns a NULL.
         *
         * @param relPathToEntry = relative to the HVSC base dir, starting with
         *                         a slash
         * @param tuneNo         = song number within the song (default=0).
         * @param field          = which field to retrieve (default=all).
         *
         * What the possible combinations of tuneNo and field represent:
         *
         * - tuneNo = 0, field = all : all of the STIL entry is returned.
         * - tuneNo = 0, field = comment : the file-global comment is returned.
         *   (For single-tune entries, this returns nothing!)
         * - tuneNo = 0, field = <other> : INVALID! (NULL is returned)
         * - tuneNo != 0, field = all : all fields of the STIL entry for the
         *   given tune number are returned. (For single-tune entries, this is
         *   equivalent to saying tuneNo = 0, field = all.)
         *   However, the file-global comment is *NOT* returned with it any
         *   more! (Unlike in versions before v2.00.) It led to confusions:
         *   eg. when a comment was asked for tune #3, it returned the
         *   file-global comment even if there was no specific entry for tune #3!
         * - tuneNo != 0, field = <other> : the specific field of the specific
         *   tune number is returned. If the tune number doesn't exist (eg. if
         *   tuneNo=2 for single-tune entries, or if tuneNo=2 when there's no
         *   STIL entry for tune #2 in a multitune entry), returns NULL.
         *
         * NOTE: For older versions of STIL (older than v2.59) the tuneNo and
         * field parameters are ignored and are assumed to be tuneNo=0 and
         * field=all to maintain backwards compatibility.
         *
         * @return
         *      - pointer to a printable formatted string containing
         *        the STIL entry
         *        (It's kinda dangerous to return a pointer that points
         *        to an internal structure, but I trust you. :)
         *      - NULL if there's absolutely no STIL entry for the tune
         */
        //public string getEntry(string relPathToEntry, int tuneNo = 0, STILField field = STILField.all) { return null; }

        /**
         * Same as #getEntry, but with an absolute path given
         * given in your machine's format.
         */
        //public string getAbsEntry(string absPathToEntry, int tuneNo = 0, STILField field = STILField.all) { return null; }

        /**
         * Given an HVSC pathname and tune number it returns a
         * formatted string that contains the section-global
         * comment for the tune number (if it exists). If it
         * doesn't exist, returns a NULL.
         *
         * @param relPathToEntry = relative to the HVSC base dir starting with
         *                       a slash
         * @return
         *      - pointer to a printable formatted string containing
         *        the section-global comment
         *        (It's kinda dangerous to return a pointer that points
         *        to an internal structure, but I trust you. :)
         *      - NULL if there's absolutely no section-global comment
         *        for the tune
         */
        //public string getGlobalComment(string relPathToEntry) { return null; }

        /**
         * Same as #getGlobalComment, but with an absolute path
         * given in your machine's format.
         */
        //public string getAbsGlobalComment(string absPathToEntry) { return null; }

        /**
         * Given an HVSC pathname and tune number it returns a
         * formatted string that contains the BUG entry for the
         * tune number (if exists). If it doesn't exist, returns
         * a NULL.
         *
         * @param relPathToEntry = relative to the HVSC base dir starting with
         *                         a slash
         * @param tuneNo         = song number within the song (default=0)
         *                         If tuneNo=0, returns all of the BUG entry.
         *
         *      NOTE: For older versions of STIL (older than v2.59) tuneNo is
         *      ignored and is assumed to be 0 to maintain backwards
         *      compatibility.
         *
         * @return
         *      - pointer to a printable formatted string containing
         *        the BUG entry
         *        (It's kinda dangerous to return a pointer that points
         *        to an internal structure, but I trust you. :)
         *      - NULL if there's absolutely no BUG entry for the tune
         */
        //public string getBug(string relPathToEntry, int tuneNo = 0) { return null; }

        /**
         * Same as #getBug, but with an absolute path
         * given in your machine's format.
         */
        //public string getAbsBug(string absPathToEntry, int tuneNo = 0) { return null; }

        /**
         * Returns a specific error number identifying the problem
         * that happened at the last invoked public method.
         *
         * @return
         *      STILerror - an enumerated error value
         */
        public STILerror getError() { return (lastError); }

        /**
         * Returns true if the last error encountered was critical
         * (ie. not one that the STIL class can recover from).
         *
         * @return
         *      true if the last error encountered was critical
         */
        public bool hasCriticalError()
        {
            return ((lastError >= STILerror.CRITICAL_STIL_ERROR) ? true : false);
        }

        /**
         * Returns an ASCII error string containing the
         * description of the error that happened at the last
         * invoked public method.
         *
         * @return
         *      pointer to string with the error description
         */
        public string getErrorStr() { return STIL_ERROR_STR[(int)lastError]; }

        //typedef std::map<std::string, std::streampos> dirList;

        /// Path to STIL.
        private string PATH_TO_STIL;

        /// Path to BUGlist.
        private string PATH_TO_BUGLIST;

        /// Version number/copyright string
        private string versionString;

        /// STIL.txt's version number
        private float STILVersion;

        /// Base dir
        private string baseDir;

        /// Maps of sections (subdirs) for easier positioning.
        //@{
        //dirList stilDirs;
        //dirList bugDirs;
        private List<Tuple<string, int>> stilDirs;
        private List<Tuple<string, int>> bugDirs;
        //@}

        /**
         * This tells us what the line delimiter is in STIL.txt.
         * (It may be two chars!)
         */
        private byte STIL_EOL;
        private byte STIL_EOL2;

        /// Error number of the last error that happened.
        private STILerror lastError;

        /// Error strings containing the description of the possible errors in STIL.
        //private string[] STIL_ERROR_STR;

        ////////////////

        /// The last retrieved entry
        private string entrybuf;

        /// The last retrieved section-global comment
        private string globalbuf;

        /// The last retrieved BUGentry
        private string bugbuf;

        /// Buffers to hold the resulting strings
        private string resultEntry = null;
        private string resultBug = null;

        ////////////////

        //private void setVersionString() { }

        /**
         * Determines what the EOL char is (or are) from STIL.txt.
         * It is assumed that BUGlist.txt will use the same EOL.
         *
         * @return
         *      - false - something went wrong
         *      - true  - everything is okay
         */
        //private bool determineEOL(System.IO.FileStream stilFile) { return false; }

        /**
         * Populates the given dirList array with the directories
         * obtained from 'inFile' for faster positioning within
         * 'inFile'.
         *
         * @param inFile - where to read the directories from
         * @param dirs   - the dirList array that should be populated with the
         *                 directory list
         * @param isSTILFile - is this the STIL or the BUGlist we are parsing
         * @return
         *      - false - No entries were found or otherwise failed to process
         *                inFile
         *      - true  - everything is okay
         */
        //private bool getDirs(System.IO.FileStream inFile, List<Tuple<string, int>> dirs, bool isSTILFile) { return false; }

        /**
         * Positions the file pointer to the given entry in 'inFile'
         * using the 'dirs' dirList for faster positioning.
         *
         * @param entryStr - the entry to position to
         * @param inFile   - position the file pointer in this file
         * @param dirs     - the list of dirs in inFile for easier positioning
         * @return
         *      - true - if successful
         *      - false - otherwise
         */
        //private bool positionToEntry(string entryStr, System.IO.FileStream inFile, List<Tuple<string, int>> dirs) { return false; }

        /**
         * Reads the entry from 'inFile' into 'buffer'. 'inFile' should
         * already be positioned to the entry to be read.
         *
         * @param inFile   - filehandle of file to read from
         * @param entryStr - the entry needed to be read
         * @param buffer   - where to put the result to
         */
        //private void readEntry(System.IO.FileStream inFile, string buffer) { }

        /**
         * Given a STIL formatted entry in 'buffer', a tune number,
         * and a field designation, it returns the requested
         * STIL field into 'result'.
         * If field=all, it also puts the file-global comment (if it exists)
         * as the first field into 'result'.
         *
         * @param result - where to put the resulting string to (if any)
         * @param buffer - pointer to the first char of what to search for
         *                 the field. Should be a buffer in standard STIL
         *                 format.
         * @param tuneNo - song number within the song (default=0)
         * @param field  - which field to retrieve (default=all).
         * @return
         *      - false - if nothing was put into 'result'
         *      - true  - 'result' has the resulting field
         */
        //private bool getField(string result, string buffer, int tuneNo = 0, STILField field = STILField.all) { return false; }

        /**
         * @param result - where to put the resulting string to (if any)
         * @param start  - pointer to the first char of what to search for
         *                 the field. Should be a buffer in standard STIL
         *                 format.
         * @param end    - pointer to the last+1 char of what to search for
         *                 the field. ('end-1' should be a '\n'!)
         * @param field  - which specific field to retrieve
         * @return
         *      - false - if nothing was put into 'result'
         *      - true  - 'result' has the resulting field
         */
        //private bool getOneField(string result, string start, string end, STILField field) { return false; }

        /**
         * Extracts one line from 'infile' to 'line[]'. The end of
         * the line is marked by endOfLineChar. Also eats up
         * additional EOL-like chars.
         *
         * @param infile - filehandle (streampos should already be positioned
         *                 to the start of the desired line)
         * @param line   - char array to put the line into
         */
        //private void getStilLine(System.IO.FileStream infile, string line) { }






        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 1998, 2002 by LaLa <LaLa@C64.org>
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

        //
        // STIL class - Implementation file
        //
        // AUTHOR: LaLa
        // Email : LaLa@C64.org
        // Copyright (C) 1998, 2002 by LaLa
        //

        //#include "stil.h"
        //#include <cstdlib>
        //#include <cstring>
        //#include <cstdio>      // For snprintf() and NULL
        //#include <iostream>
        //#include <iomanip>
        //#include <fstream>
        //#include <sstream>
        //#include <utility>
        //#include "stringutils.h"

        //const ios_base::openmode STILopenFlags = ios::in | ios::binary;
        public System.IO.FileMode STILopenFlags = System.IO.FileMode.Open; //| ios::binary;

        public const float VERSION_NO = 3.0f;

        public void CERR_STIL_DEBUG(string str) {
            if (STIL_DEBUG) {
                //cerr << "Line #" << __LINE__ << " STIL::"
                Console.Write("Line #" + str);
            }
        }

        // These are the hardcoded STIL/BUG field names.
        private string _NAME_STR = "   NAME: ";
        private string _AUTHOR_STR = " AUTHOR: ";
        private string _TITLE_STR = "  TITLE: ";
        private string _ARTIST_STR = " ARTIST: ";
        private string _COMMENT_STR = "COMMENT: ";
        //const char     _BUG_STR[] = "BUG: ";

        private string[] STIL_ERROR_STR = new string[] {
    "No error.",
    "Failed to open BUGlist.txt.",
    "Base dir path is not the HVSC base dir path.",
    "The entry was not found in STIL.txt.",
    "The entry was not found in BUGlist.txt.",
    "A section-global comment was asked for in the wrong way.",
    "",
    "",
    "",
    "",
    "CRITICAL ERROR",
    "Incorrect HVSC base dir length!",
    "Failed to open STIL.txt!",
    "Failed to determine EOL from STIL.txt!",
    "No STIL sections were found in STIL.txt!",
    "No STIL sections were found in BUGlist.txt!"
    };

        /**
         * Converts slashes to the one the OS uses to access files.
         *
         * @param
         *      str - what to convert
         */
        private void convertSlashes(ref string str)
        {
            //std::replace(str.begin(), str.end(), '/', SLASH);
            str = str.Replace('/', stildefs.SLASH);
        }

        /**
         * Converts OS specific dir separators to slashes.
         *
         * @param
         *      str - what to convert
         */
        private void convertToSlashes(ref string str) {
            //std::replace(str.begin(), str.end(), SLASH, '/');
            str = str.Replace(stildefs.SLASH, '/');
        }


        // CONSTRUCTOR
        public STIL(string stilPath = stildefs.DEFAULT_PATH_TO_STIL, string bugsPath = stildefs.DEFAULT_PATH_TO_BUGLIST)
        {
            STIL_DEBUG = false;
            PATH_TO_STIL = stilPath;
            PATH_TO_BUGLIST = bugsPath;
            STILVersion = 0.0f;
            STIL_EOL = (byte)'\n';
            STIL_EOL2 = 0;
            lastError = STILerror.NO_STIL_ERROR;
            setVersionString();
        }

        private void setVersionString()
        {
            string ss = string.Format(
                "STILView v{0:F2}\r\n"
                + "\tCopyright (C) 1998, 2002 by LaLa (LaLa@C64.org)\r\n"
                + "\tCopyright (C) 2012-2015 by Leandro Nini <drfiemost@users.sourceforge.net>\r\n"
                , VERSION_NO
                );
            //ss << fixed << setw(4) << setprecision(2);
            //ss << "STILView v" << VERSION_NO << endl;
            //ss << "\tCopyright (C) 1998, 2002 by LaLa (LaLa@C64.org)" << endl;
            //ss << "\tCopyright (C) 2012-2015 by Leandro Nini <drfiemost@users.sourceforge.net>" << endl;
            versionString = ss;
        }

        private string getVersion()
        {
            lastError = STILerror.NO_STIL_ERROR;
            return versionString;
        }

        public float getVersionNo()
        {
            lastError = STILerror.NO_STIL_ERROR;
            return VERSION_NO;
        }

        public float getSTILVersionNo()
        {
            lastError = STILerror.NO_STIL_ERROR;
            return STILVersion;
        }

        public bool setBaseDir(string pathToHVSC)
        {
            // Temporary placeholder for STIL.txt's version number.
            float tempSTILVersion = STILVersion;

            // Temporary placeholders for lists of sections.
            List<Tuple<string, int>> tempStilDirs=new List<Tuple<string, int>>();
            List<Tuple<string, int>> tempBugDirs = new List<Tuple<string, int>>();

            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("setBaseDir() called, pathToHVSC=" + pathToHVSC + "\r\n");

            string tempBaseDir = (pathToHVSC);

            // Sanity check the length.
            if (string.IsNullOrEmpty(tempBaseDir))
            {
                CERR_STIL_DEBUG("setBaseDir() has problem with the size of pathToHVSC" + "\r\n");
                lastError = STILerror.BASE_DIR_LENGTH;
                return false;
            }

            // Chop the trailing slash
            char lastChar = tempBaseDir[tempBaseDir.Length - 1];

            if (lastChar == stildefs.SLASH)
            {
                tempBaseDir = tempBaseDir.Substring(0, tempBaseDir.Length - 2);
            }

            // Attempt to open STIL

            // Create the full path+filename
            string tempName = tempBaseDir;
            tempName = tempName + PATH_TO_STIL;
            convertSlashes(ref tempName);

            //ifstream stilFile(tempName, STILopenFlags);
            System.IO.FileStream stilFile;
            try
            {
                stilFile = new System.IO.FileStream(tempName, STILopenFlags);
            }catch
            //if (stilFile.fail())
            {
                CERR_STIL_DEBUG("setBaseDir() open failed for " + tempName + "\r\n");
                lastError = STILerror.STIL_OPEN;
                return false;
            }

            CERR_STIL_DEBUG("setBaseDir(): open succeeded for " + tempName + "\r\n");

            // Attempt to open BUGlist

            // Create the full path+filename
            tempName = tempBaseDir;
            tempName = tempName + PATH_TO_BUGLIST;
            convertSlashes(ref tempName);

            //ifstream bugFile(tempName.c_str(), STILopenFlags);
            System.IO.FileStream bugFile=null;
            try
            {
                bugFile = new System.IO.FileStream(tempName, STILopenFlags);
                CERR_STIL_DEBUG("setBaseDir(): open succeeded for " + tempName + "\r\n");
            }
            catch
            //if (bugFile.fail())
            {
                // This is not a critical error - some earlier versions of HVSC did
                // not have a BUGlist.txt file at all.

                if (bugFile != null) bugFile.Close();
                CERR_STIL_DEBUG("setBaseDir() open failed for " + tempName + "\r\n");
                lastError = STILerror.BUG_OPEN;
                bugFile = null;
            }

            // Find out what the EOL really is
            if (determineEOL(stilFile) != true)
            {
                CERR_STIL_DEBUG("determinEOL() failed" + "\r\n");
                lastError = STILerror.NO_EOL;
                return false;
            }

            // Save away the current string so we can restore it if needed.
            string tempVersionString = versionString;

            setVersionString();

            // This is necessary so the version number gets scanned in from the new
            // file, too.
            STILVersion = 0.0f;

            // These will populate the tempStilDirs and tempBugDirs maps (or not :)

            if (getDirs(stilFile, tempStilDirs, true) != true)
            {
                CERR_STIL_DEBUG("getDirs() failed for stilFile" + "\r\n");
                lastError = STILerror.NO_STIL_DIRS;

                // Clean up and restore things.
                STILVersion = tempSTILVersion;
                versionString = tempVersionString;
                return false;
            }

            if (bugFile!=null)
            {
                if (getDirs(bugFile, tempBugDirs, false) != true)
                {
                    // This is not a critical error - it is possible that the
                    // BUGlist.txt file has no entries in it at all (in fact, that's
                    // good!).

                    CERR_STIL_DEBUG("getDirs() failed for bugFile" + "\r\n");
                    lastError = STILerror.BUG_OPEN;
                }
            }

            if (bugFile != null) bugFile.Close();

            // Now we can copy the stuff into private data.
            // NOTE: At this point, STILVersion and the versionString should contain
            // the new info!

            // Copy.
            baseDir = tempBaseDir;
            stilDirs = tempStilDirs;
            bugDirs = tempBugDirs;

            // Clear the buffers (caches).
            entrybuf = "";
            globalbuf = "";
            bugbuf = "";

            CERR_STIL_DEBUG("setBaseDir() succeeded" + "\r\n");

            return true;
        }

        private bool stringCmp(string a,string b,int len)
        {
            if (a == null && b == null && len == 0) return true;
            if (a == null || b == null) return false;
            if (a.Length < len) return false;
            if (b.Length < len) return false;

            for (int i = 0; i < len; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

        public string getAbsEntry(string absPathToEntry, int tuneNo = 0, STILField field = STILField.all)
        {
            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("getAbsEntry() called, absPathToEntry=" + absPathToEntry + "\r\n");

            if (string.IsNullOrEmpty(baseDir))
            {
                CERR_STIL_DEBUG("HVSC baseDir is not yet set!" + "\r\n");
                lastError = STILerror.STIL_OPEN;
                return null;
            }

            // Determine if the baseDir is in the given pathname.

            //if (!stringutils::equal(absPathToEntry, baseDir.data(), baseDir.size()))
            if (!stringCmp(absPathToEntry, baseDir, baseDir.Length))
            {
                CERR_STIL_DEBUG("getAbsEntry() failed: baseDir=" + baseDir + ", absPath=" + absPathToEntry + "\r\n");
                lastError = STILerror.WRONG_DIR;
                return null;
            }


            string tempDir = absPathToEntry + baseDir.Length;
            convertToSlashes(ref tempDir);

            return getEntry(tempDir, tuneNo, field);
        }

        public string getEntry(string relPathToEntry, int tuneNo = 0, STILField field = STILField.all)
        {
            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("getEntry() called, relPath=" + relPathToEntry + ", rest=" + tuneNo + "," + field + "\r\n");

            if (string.IsNullOrEmpty(baseDir))
            {
                CERR_STIL_DEBUG("HVSC baseDir is not yet set!" + "\r\n");
                lastError = STILerror.STIL_OPEN;
                return null;
            }

            int relPathToEntryLen = relPathToEntry.Length;

            // Fail if a section-global comment was asked for.

            if (relPathToEntry[relPathToEntryLen - 1] == '/')
            {
                CERR_STIL_DEBUG("getEntry() section-global comment was asked for - failed" + "\r\n");
                lastError = STILerror.WRONG_ENTRY;
                return null;
            }

            if (STILVersion < 2.59f)
            {
                // Older version of STIL is detected.

                tuneNo = 0;
                field = STILField.all;
            }

            // Find out whether we have this entry in the buffer.

            //if ((!stringutils::equal(entrybuf.data(), relPathToEntry, relPathToEntryLen))
            //|| ((entrybuf.find_first_of('\n') != relPathToEntryLen)
            //&& (STILVersion > 2.59f)))
            if ((!stringCmp(entrybuf, relPathToEntry, relPathToEntryLen))
                || ((entrybuf.IndexOf('\n') != relPathToEntryLen)
                && (STILVersion > 2.59f)))
            {
                // The relative pathnames don't match or they're not the same length:
                // we don't have it in the buffer, so pull it in.

                CERR_STIL_DEBUG("getEntry(): entry not in buffer" + "\r\n");

                // Create the full path+filename
                string tempName = baseDir;
                tempName += PATH_TO_STIL;
                convertSlashes(ref tempName);

                //ifstream stilFile(tempName.c_str(), STILopenFlags);

                System.IO.FileStream stilFile=null;
                try
                {
                    stilFile = new System.IO.FileStream(tempName, STILopenFlags);
                }
                catch
                //if (stilFile.fail())
                {
                    if (stilFile != null) stilFile.Close();
                    CERR_STIL_DEBUG("getEntry() open failed for stilFile" + "\r\n");
                    lastError = STILerror.STIL_OPEN;
                    return null;
                }

                CERR_STIL_DEBUG("getEntry() open succeeded for stilFile" + "\r\n");

                if (positionToEntry(new Ptr<byte>(Encoding.ASCII.GetBytes(relPathToEntry),0), stilFile, stilDirs) == false)
                {
                    // Copy the entry's name to the buffer.
                    entrybuf = relPathToEntry + "\n";
                    CERR_STIL_DEBUG("getEntry() posToEntry() failed" + "\r\n");
                    lastError = STILerror.NOT_IN_STIL;
                }
                else
                {
                    entrybuf = "";
                    readEntry(stilFile, entrybuf);
                    CERR_STIL_DEBUG("getEntry() entry read" + "\r\n");
                }

                if (stilFile != null) stilFile.Close();

            }

            // Put the requested field into the result string.
            return getField(resultEntry, entrybuf, tuneNo, field) ? resultEntry : null;
        }

        public string getAbsBug(string absPathToEntry, int tuneNo = 0)
        {
            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("getAbsBug() called, absPathToEntry=" + absPathToEntry + "\r\n");

            if (string.IsNullOrEmpty(baseDir))
            {
                CERR_STIL_DEBUG("HVSC baseDir is not yet set!" + "\r\n");
                lastError = STILerror.BUG_OPEN;
                return null;
            }

            // Determine if the baseDir is in the given pathname.

            if (!stringCmp(absPathToEntry, baseDir, baseDir.Length))
            {
                CERR_STIL_DEBUG("getAbsBug() failed: baseDir=" + baseDir + ", absPath=" + absPathToEntry + "\r\n");
                lastError = STILerror.WRONG_DIR;
                return null;
            }

            string tempDir = absPathToEntry + baseDir.Length;
            convertToSlashes(ref tempDir);

            return getBug(tempDir, tuneNo);
        }

        public string getBug(string relPathToEntry, int tuneNo = 0)
        {
            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("getBug() called, relPath=" + relPathToEntry + ", rest=" + tuneNo + "\r\n");

            if (string.IsNullOrEmpty(baseDir))
            {
                CERR_STIL_DEBUG("HVSC baseDir is not yet set!" + "\r\n");
                lastError = STILerror.BUG_OPEN;
                return null;
            }

            // Older version of STIL is detected.

            if (STILVersion < 2.59f)
            {
                tuneNo = 0;
            }

            // Find out whether we have this bug entry in the buffer.
            // If the baseDir was changed, we'll have to read it in again,
            // even if it might be in the buffer already.

            int relPathToEntryLen = relPathToEntry.Length;

            if ((!stringCmp(bugbuf, relPathToEntry, relPathToEntryLen)) ||
                ((bugbuf.IndexOf('\n') != relPathToEntryLen) &&
                 (STILVersion > 2.59f)))
            {
                // The relative pathnames don't match or they're not the same length:
                // we don't have it in the buffer, so pull it in.

                CERR_STIL_DEBUG("getBug(): entry not in buffer" + "\r\n");

                // Create the full path+filename
                string tempName = baseDir;
                tempName += PATH_TO_BUGLIST;
                convertSlashes(ref tempName);

                //ifstream bugFile(tempName, STILopenFlags);
                System.IO.FileStream bugFile=null;
                try
                {
                    bugFile = new System.IO.FileStream(tempName, STILopenFlags);
                } catch
                //if (bugFile.fail())
                {
                    if (bugFile != null) bugFile.Close();
                    CERR_STIL_DEBUG("getBug() open failed for bugFile" + "\r\n");
                    lastError = STILerror.BUG_OPEN;
                    return null;
                }

                CERR_STIL_DEBUG("getBug() open succeeded for bugFile" + "\r\n");

                if (positionToEntry(new Ptr<byte>(Encoding.ASCII.GetBytes(relPathToEntry), 0), bugFile, bugDirs) == false)
                {
                    // Copy the entry's name to the buffer.
                    bugbuf = relPathToEntry + "\n";
                    CERR_STIL_DEBUG("getBug() posToEntry() failed" + "\r\n");
                    lastError = STILerror.NOT_IN_BUG;
                }
                else
                {
                    bugbuf = "";
                    readEntry(bugFile, bugbuf);
                    CERR_STIL_DEBUG("getBug() entry read" + "\r\n");
                }
                if (bugFile != null) bugFile.Close();
            }

            // Put the requested field into the result string.
            return getField(resultBug, bugbuf, tuneNo) ? resultBug : null;
        }

        public string getAbsGlobalComment(string absPathToEntry)
        {
            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("getAbsGC() called, absPathToEntry=" + absPathToEntry + "\r\n");

            if (string.IsNullOrEmpty(baseDir))
            {
                CERR_STIL_DEBUG("HVSC baseDir is not yet set!" + "\r\n");
                lastError = STILerror.STIL_OPEN;
                return null;
            }

            // Determine if the baseDir is in the given pathname.

            if (!stringCmp(absPathToEntry, baseDir, baseDir.Length))
            {
                CERR_STIL_DEBUG("getAbsGC() failed: baseDir=" + baseDir + ", absPath=" + absPathToEntry + "\r\n");
                lastError = STILerror.WRONG_DIR;
                return null;
            }

            string tempDir = absPathToEntry + baseDir.Length;
            convertToSlashes(ref tempDir);

            return getGlobalComment(tempDir);
        }

        public string getGlobalComment(string relPathToEntry)
        {
            lastError = STILerror.NO_STIL_ERROR;

            CERR_STIL_DEBUG("getGC() called, relPath=" + relPathToEntry + "\r\n");

            if (string.IsNullOrEmpty(baseDir))
            {
                CERR_STIL_DEBUG("HVSC baseDir is not yet set!" + "\r\n");
                lastError = STILerror.STIL_OPEN;
                return null;
            }

            // Save the dirpath.

            string lastSlash = relPathToEntry.Substring(relPathToEntry.LastIndexOf('/'));

            if (lastSlash == null)
            {
                lastError = STILerror.WRONG_DIR;
                return null;
            }

            //int pathLen = lastSlash - relPathToEntry + 1;
            string dir = relPathToEntry.Substring(0, relPathToEntry.LastIndexOf('/'));

            // Find out whether we have this global comment in the buffer.
            // If the baseDir was changed, we'll have to read it in again,
            // even if it might be in the buffer already.

            if ((!stringCmp(globalbuf, dir, dir.Length)) ||
                ((globalbuf.IndexOf('\n') != dir.Length) &&
                 (STILVersion > 2.59f)))
            {
                // The relative pathnames don't match or they're not the same length:
                // we don't have it in the buffer, so pull it in.

                CERR_STIL_DEBUG("getGC(): entry not in buffer" + "\r\n");

                // Create the full path+filename
                string tempName = baseDir;
                tempName += PATH_TO_STIL;
                convertSlashes(ref tempName);

                //ifstream stilFile(tempName.c_str(), STILopenFlags);
                System.IO.FileStream stilFile;
                try
                {
                    stilFile = new System.IO.FileStream(tempName, STILopenFlags);
                } catch
                //if (stilFile.fail())
                {
                    CERR_STIL_DEBUG("getGC() open failed for stilFile" + "\r\n");
                    lastError = STILerror.STIL_OPEN;
                    return null;
                }

                if (positionToEntry(new Ptr<byte>(Encoding.ASCII.GetBytes(dir), 0), stilFile, stilDirs) == false)
                {
                    // Copy the dirname to the buffer.
                    globalbuf = dir + "\n";
                    CERR_STIL_DEBUG("getGC() posToEntry() failed" + "\r\n");
                    lastError = STILerror.NOT_IN_STIL;
                }
                else
                {
                    globalbuf = "";
                    readEntry(stilFile, globalbuf);
                    CERR_STIL_DEBUG("getGC() entry read" + "\r\n");
                }

                stilFile.Close();
            }

            CERR_STIL_DEBUG("getGC() globalbuf=" + globalbuf + "\r\n");
            CERR_STIL_DEBUG("-=END=-" + "\r\n");

            // Position pointer to the global comment field.

            int temp = globalbuf.IndexOf('\n') + 1;

            // Check whether this is a NULL entry or not.
            return (temp != globalbuf.Length || temp != 0) ? globalbuf + temp : null;
        }

        //////// PRIVATE

        private bool determineEOL(System.IO.FileStream stilFile)
        {
            CERR_STIL_DEBUG("detEOL() called" + "\r\n");

            if (stilFile == null)
            {
                CERR_STIL_DEBUG("detEOL() open failed" + "\r\n");
                return false;
            }

            stilFile.Seek(0, System.IO.SeekOrigin.Begin);

            STIL_EOL = 0;
            STIL_EOL2 = 0;

            // Determine what the EOL character is
            // (it can be different from OS to OS).
            //istream::sentry se(stilFile, true);
            //if (se)
            //{
            //streambuf* sb = stilFile.rdbuf();

            //int eof = char_traits < char >::eof();

            int c;
            while ((c = stilFile.ReadByte()) != -1)
            {
                if ((c == '\n') || (c == '\r'))
                {
                    STIL_EOL = (byte)c;

                    if (c == '\r')
                    {
                        if (stilFile.ReadByte() == '\n')
                            STIL_EOL2 = (byte)'\n';
                    }
                    break;
                }
            }
            //}

            if (STIL_EOL == '\0')
            {
                // Something is wrong - no EOL-like char was found.
                CERR_STIL_DEBUG("detEOL() no EOL found" + "\r\n");
                return false;
            }

            CERR_STIL_DEBUG("detEOL() EOL1=0x" + string.Format("{0:x}", (int)STIL_EOL) + " EOL2=0x" + string.Format("{0:x}", (int)STIL_EOL2) + "\r\n");

            return true;
        }

        private bool getDirs(System.IO.FileStream inFile, List<Tuple<string, int>> dirs, bool isSTILFile)
        {
            bool newDir = !isSTILFile;

            CERR_STIL_DEBUG("getDirs() called" + "\r\n");

            inFile.Seek(0, System.IO.SeekOrigin.Begin);

            while (inFile!=null)
            {
                string line="";

                getStilLine(inFile,ref line);

                if (!isSTILFile) { CERR_STIL_DEBUG(line + '\n'); }

                // Try to extract STIL's version number if it's not done, yet.

                if (isSTILFile && (STILVersion == 0.0f))
                {
                    if (stringCmp(line, "#  STIL v", 9))
                    {
                        // Get the version number
                        STILVersion = float.Parse(line);// , 9);

                        // Put it into the string, too.
                        string ss;
                        //ss = fixed << setw(4) << setprecision(2);
                        ss = string.Format("SID Tune Information List (STIL) v{0:f2}\r\n", STILVersion);
                        versionString += ss;

                        CERR_STIL_DEBUG("getDirs() STILVersion=" + STILVersion + "\r\n");

                        continue;
                    }
                }

                // Search for the start of a dir separator first.

                if (isSTILFile && !newDir && stringCmp(line, "### ", 4))
                {
                    newDir = true;
                    continue;
                }

                // Is this the start of an entry immediately following a dir separator?

                if (newDir && (line[0] == '/'))
                {
                    // Get the directory only
                    string dirName = line.Substring(0, line.LastIndexOf('/') + 1);

                    if (!isSTILFile)
                    {
                        // Compare it to the stored dirnames
                        newDir = false;
                        foreach (Tuple<string,int> c in dirs)
                        {
                            if(c.Item1==dirName)
                            {
                                newDir = true;
                                break;
                            }
                        }
                    }

                    // Store the info
                    if (newDir)
                    {
                        int position = (int)(inFile.Position - line.Length - 1L);

                        CERR_STIL_DEBUG("getDirs() dirName=" + dirName + ", pos=" + position + "\r\n");

                        dirs.Add(new Tuple<string, int>(dirName, position));
                    }

                    newDir = !isSTILFile;
                }
            }

            if (dirs == null)
            {
                // No entries found - something is wrong.
                // NOTE: It's perfectly valid to have a BUGlist.txt file with no
                // entries in it!
                CERR_STIL_DEBUG("getDirs() no dirs found" + "\r\n");
                return false;
            }

            CERR_STIL_DEBUG("getDirs() successful" + "\r\n");

            return true;
        }

        private bool positionToEntry(Ptr<byte> entryStr, System.IO.FileStream inFile, List<Tuple<string, int>> dirs)
        {
            CERR_STIL_DEBUG("pos2Entry() called, entryStr=" + entryStr + "\r\n");

            inFile.Seek(0, System.IO.SeekOrigin.Begin);

            // Get the dirpath.

            Ptr<byte> chrptr = Ptr<byte>.strrchr(entryStr, (byte)'/');

            // If no slash was found, something is screwed up in the entryStr.

            if (chrptr == null)
            {
                return false;
            }

            int pathLen = chrptr.ptr - entryStr.ptr + 1;

            // Determine whether a section-global comment is asked for.

            int entryStrLen = entryStr.buf.Length - entryStr.ptr;
            bool globComm = (pathLen == entryStrLen);

            // Find it in the table.
            string entry = entryStr.ToString(pathLen);
            //dirList::iterator elem = dirs.find(entry);
            Tuple<string, int> elem = null;
            foreach (Tuple<string, int> t in dirs)
            {
                if (t.Item1 == entry)
                {
                    elem = t;
                    break;
                }
            }
            if (elem == null)
            {
                // The directory was not found.
                CERR_STIL_DEBUG("pos2Entry() did not find the dir" + "\r\n");
                return false;
            }

            // Jump to the first entry of this section.
            inFile.Seek(elem.Item2, System.IO.SeekOrigin.Begin);
            bool foundIt = false;

            // Now find the desired entry

            string line = null;

            do
            {
                getStilLine(inFile, ref line);

                if (inFile.Length==inFile.Position)
                {
                    break;
                }

                // Check if it is the start of an entry

                if (line[0] == '/')
                {
                    if (!stringCmp(elem.Item1, line, pathLen))
                    {
                        // We are outside the section - get out of the loop,
                        // which will fail the search.
                        break;
                    }

                    // Check whether we need to find a section-global comment or
                    // a specific entry.

                    if (globComm || (STILVersion > 2.59f))
                    {
                        foundIt = line == entryStr.ToString();
                    }
                    else
                    {
                        // To be compatible with older versions of STIL, which may have
                        // the tune designation on the first line of a STIL entry
                        // together with the pathname.
                        foundIt = stringCmp(line, entryStr.ToString(), entryStrLen);
                    }

                    CERR_STIL_DEBUG("pos2Entry() line=" + line + "\r\n");
                }
            }
            while (!foundIt);

            if (foundIt)
            {
                // Reposition the file pointer back to the start of the entry.
                inFile.Seek(inFile.Position - line.Length - 1L, System.IO.SeekOrigin.Begin);
                CERR_STIL_DEBUG("pos2Entry() entry found" + "\r\n");
                return true;
            }
            else
            {
                CERR_STIL_DEBUG("pos2Entry() entry not found" + "\r\n");
                return false;
            }
        }

        private void readEntry(System.IO.FileStream inFile, string buffer)
        {
            string line="";

            for (;;)
            {
                getStilLine(inFile, ref line);

                if (line.Length == 0)
                    break;

                buffer += line;
                buffer += "\n";
            }
        }

        private bool getField(string result, string buffer, int tuneNo = 0, STILField field = STILField.all)
        {
            CERR_STIL_DEBUG("getField() called, buffer=" + buffer + ", rest=" + tuneNo + "," + field + "\r\n");

            // Clean out the result buffer first.
            result = "";

            // Position pointer to the first char beyond the file designation.

            Ptr<byte> start = Ptr<byte>.strchr(new Ptr<byte>(Encoding.ASCII.GetBytes(buffer), 0), (byte)'\n');
            if (start != null) start.AddPtr(1);

            // Check whether this is a NULL entry or not.

            if (start == null)
            {
                CERR_STIL_DEBUG("getField() null entry" + "\r\n");
                return false;
            }

            // Is this a multitune entry?
            Ptr<byte> firstTuneNo =Ptr<byte>.strstr(start, "(#");

            // This is a tune designation only if the previous char was
            // a newline (ie. if the "(#" is on the beginning of a line).
            if ((firstTuneNo != null) && (firstTuneNo.buf[firstTuneNo.buf.Length - 1] != '\n'))
            {
                firstTuneNo = null;
            }

            if (firstTuneNo == null)
            {
                //-------------------//
                // SINGLE TUNE ENTRY //
                //-------------------//

                // Is the first thing in this STIL entry the COMMENT?

                Ptr<byte> temp = Ptr<byte>.strstr(start, _COMMENT_STR);
                Ptr<byte> temp2 = null;

                // Search for other potential fields beyond the COMMENT.
                if (temp.ptr == start.ptr)
                {
                    temp2 = Ptr<byte>.strstr(start, _NAME_STR);

                    if (temp2 == null)
                    {
                        temp2 = Ptr<byte>.strstr(start, _AUTHOR_STR);

                        if (temp2 == null)
                        {
                            temp2 = Ptr<byte>.strstr(start, _TITLE_STR);

                            if (temp2 == null)
                            {
                                temp2 = Ptr<byte>.strstr(start, _ARTIST_STR);
                            }
                        }
                    }
                }

                if (temp.ptr == start.ptr)
                {
                    // Yes. So it's assumed to be a file-global comment.

                    CERR_STIL_DEBUG("getField() single-tune entry, COMMENT only" + "\r\n");

                    if ((tuneNo == 0) && ((field == STIL.STILField.all) || ((field == STIL.STILField.comment) && (temp2 == null))))
                    {
                        // Simply copy the stuff in.
                        result += start;
                        CERR_STIL_DEBUG("getField() copied to resultbuf" + "\r\n");
                        return true;
                    }

                    else if ((tuneNo == 0) && (field == STIL.STILField.comment))
                    {
                        // Copy just the comment.
                        result += start.ToString(temp2.ptr - start.ptr);
                        CERR_STIL_DEBUG("getField() copied to just the COMMENT to resultbuf" + "\r\n");
                        return true;
                    }

                    else if ((tuneNo == 1) && (temp2 != null))
                    {
                        // A specific field was asked for.

                        CERR_STIL_DEBUG("getField() copying COMMENT to resultbuf" + "\r\n");
                        return getOneField(
                            ref result,
                            temp2.ToString(temp2.buf.Length),
                            temp2.ptr,
                            temp2.buf.Length,
                            field);
                    }

                    else
                    {
                        // Anything else is invalid as of v2.00.

                        CERR_STIL_DEBUG("getField() invalid parameter combo: single tune, tuneNo=" + tuneNo + ", field=" + field + "\r\n");
                        return false;
                    }
                }
                else
                {
                    // No. Handle it as a regular entry.

                    CERR_STIL_DEBUG("getField() single-tune regular entry" + "\r\n");

                    if ((field == STIL.STILField.all) && ((tuneNo == 0) || (tuneNo == 1)))
                    {
                        // The complete entry was asked for. Simply copy the stuff in.
                        result += start;
                        CERR_STIL_DEBUG("getField() copied to resultbuf" + "\r\n");
                        return true;
                    }

                    else if (tuneNo == 1)
                    {
                        // A specific field was asked for.

                        CERR_STIL_DEBUG("getField() copying COMMENT to resultbuf" + "\r\n");
                        return getOneField(
                            ref result,
                            start.ToString(start.buf.Length),
                            start.ptr,
                            start.ptr + start.buf.Length,
                            field);
                    }

                    else
                    {
                        // Anything else is invalid as of v2.00.

                        CERR_STIL_DEBUG("getField() invalid parameter combo: single tune, tuneNo=" + tuneNo + ", field=" + field + "\r\n");
                        return false;
                    }
                }
            }
            else
            {
                //-------------------//
                // MULTITUNE ENTRY
                //-------------------//

                CERR_STIL_DEBUG("getField() multitune entry" + "\r\n");

                // Was the complete entry asked for?

                if (tuneNo == 0)
                {
                    switch (field)
                    {
                        case STIL.STILField.all:
                            // Yes. Simply copy the stuff in.
                            result += start;
                            CERR_STIL_DEBUG("getField() copied all to resultbuf" + "\r\n");
                            return true;

                        case STIL.STILField.comment:
                            // Only the file-global comment field was asked for.

                            if (firstTuneNo != start)
                            {
                                CERR_STIL_DEBUG("getField() copying file-global comment to resultbuf" + "\r\n");
                                return getOneField(
                                    ref result,
                                    start.ToString(start.buf.Length),
                                    start.ptr,
                                    firstTuneNo.ptr,
                                    STIL.STILField.comment);
                            }
                            else
                            {
                                CERR_STIL_DEBUG("getField() no file-global comment" + "\r\n");
                                return false;
                            }

                            //break;

                        default:
                            // If a specific field other than a comment is
                            // asked for tuneNo=0, this is illegal.

                            CERR_STIL_DEBUG("getField() invalid parameter combo: multitune, tuneNo=" + tuneNo + ", field=" + field + "\r\n");
                            return false;
                    }
                }

                byte[] tuneNoStr = new byte[8];

                // Search for the requested tune number.

                tuneNoStr = Encoding.ASCII.GetBytes(string.Format("(#{0})", tuneNo));
                tuneNoStr[7] = (byte)'\0';
                Ptr<byte> myTuneNo = Ptr<byte>.strstr(start, tuneNoStr.ToString());

                if (myTuneNo != null)
                {
                    // We found the requested tune number.
                    // Set the pointer beyond it.
                    myTuneNo = Ptr<byte>.strchr(myTuneNo, (byte)'\n');
                    myTuneNo.AddPtr(1);

                    // Where is the next one?

                    Ptr<byte> nextTuneNo = Ptr<byte>.strstr(myTuneNo, "\n(#");

                    if (nextTuneNo == null)
                    {
                        // There is no next one - set pointer to end of entry.
                        nextTuneNo = new Ptr<byte>(start.buf, start.buf.Length - start.ptr);
                    }
                    else
                    {
                        // The search included the \n - go beyond it.
                        nextTuneNo.AddPtr(1);
                    }

                    // Put the desired fields into the result (which may be 'all').

                    CERR_STIL_DEBUG("getField() myTuneNo=" + myTuneNo + ", nextTuneNo=" + nextTuneNo + "\r\n");
                    return getOneField(
                        ref result,
                        myTuneNo.ToString(myTuneNo.buf.Length),
                        myTuneNo.ptr,
                        nextTuneNo.ptr,
                        field);
                }

                else
                {
                    CERR_STIL_DEBUG("getField() nothing found" + "\r\n");
                    return false;
                }
            }
        }

        private bool getOneField(ref string result, string src, int start, int end, STILField field)
        {
            // Sanity checking

            if ((end < start) || (src[end - 1] != '\n'))
            {
                CERR_STIL_DEBUG("getOneField() illegal parameters" + "\r\n");
                return false;
            }

            CERR_STIL_DEBUG("getOneField() called, start=" + start + ", rest=" + field + "\r\n");

            string temp = null;
            int tempInd = 0;

            switch (field)
            {
                case STILField.all:
                    result += src.Substring(start, end - start);
                    return true;

                case STILField.name:
                    tempInd = src.IndexOf(_NAME_STR, start);
                    if (tempInd != -1) temp = src.Substring(tempInd);
                    break;

                case STILField.author:
                    tempInd = src.IndexOf(_AUTHOR_STR, start);
                    if (tempInd != -1) temp = src.Substring(tempInd);
                    break;

                case STILField.title:
                    tempInd = src.IndexOf(_TITLE_STR, start);
                    if (tempInd != -1) temp = src.Substring(tempInd);
                    break;

                case STILField.artist:
                    tempInd = src.IndexOf(_ARTIST_STR, start);
                    if (tempInd != -1) temp = src.Substring(tempInd);
                    break;

                case STILField.comment:
                    tempInd = src.IndexOf(_COMMENT_STR, start);
                    if (tempInd != -1) temp = src.Substring(tempInd);
                    break;

                default:
                    break;
            }

            // If the field was not found or it is not in between 'start'
            // and 'end', it is declared a failure.

            //if ((temp == null) || (temp.ptr < start.ptr) || (temp.ptr > end.ptr))
            if (temp == null)
            {
                return false;
            }

            // Search for the end of this field. This is done by finding
            // where the next field starts.

            string nextName = null;
            string nextAuthor = null;
            string nextTitle = null;
            string nextArtist = null;
            string nextComment = null;

            // If any of these fields is beyond 'end', they are ignored.

            int nameInd = temp.IndexOf(_NAME_STR, start + 1);
            if (nameInd != -1) nextName = temp.Substring(nameInd);
            int authorInd = temp.IndexOf(_AUTHOR_STR, start + 1);
            if (authorInd != -1) nextAuthor = temp.Substring(authorInd);
            int titleInd = temp.IndexOf(_TITLE_STR, start + 1);
            if (titleInd != -1) nextTitle = temp.Substring(titleInd);
            int artistInd = temp.IndexOf(_ARTIST_STR, start + 1);
            if (artistInd != -1) nextArtist = temp.Substring(artistInd);
            int commentInd = temp.IndexOf(_COMMENT_STR, start + 1);
            if (commentInd != -1) nextComment = temp.Substring(commentInd);

            // Now determine which one is the closest to our field - that one
            // will mark the end of the required field.

            string nextField = nextName;
            int nextFieldInd = nameInd;

            if (nextField == null)
            {
                nextField = nextAuthor;
                nextFieldInd = authorInd;
            }
            else if ((nextAuthor != null) && (authorInd < nextFieldInd))
            {
                nextField = nextAuthor;
                nextFieldInd = authorInd;
            }

            if (nextField == null)
            {
                nextField = nextTitle;
                nextFieldInd = titleInd;
            }
            else if ((nextTitle != null) && (titleInd < nextFieldInd))
            {
                nextField = nextTitle;
                nextFieldInd = titleInd;
            }

            if (nextField == null)
            {
                nextField = nextArtist;
                nextFieldInd = artistInd;
            }
            else if ((nextArtist != null) && (artistInd < nextFieldInd))
            {
                nextField = nextArtist;
                nextFieldInd = artistInd;
            }

            if (nextField == null)
            {
                nextField = nextComment;
                nextFieldInd = commentInd;
            }
            else if ((nextComment != null) && (commentInd < nextFieldInd))
            {
                nextField = nextComment;
                nextFieldInd = commentInd;
            }

            if (nextField == null)
            {
                nextField = src.Substring(end);
                nextFieldInd = end;
            }

            // Now nextField points to the last+1 char that should be copied to
            // result. Do that.

            result += temp.Substring(0, nextFieldInd);
            return true;
        }

        private void getStilLine(System.IO.FileStream infile,ref string line)
        {
            if (STIL_EOL2 != '\0')
            {
                // If there was a remaining EOL char from the previous read, eat it up.

                int temp = infile.ReadByte();

                if ((temp == 0x0d) || (temp == 0x0a))
                {
                    //infile.get(temp);
                }
                else
                {
                    infile.Seek(-1, System.IO.SeekOrigin.Current);
                }
            }

            //getline(infile, ref line, STIL_EOL);
            line = "";
            int ch = 0;
            while ((ch = infile.ReadByte()) != STIL_EOL)
            {
                line += (char)ch;
            }
        }

    }
}