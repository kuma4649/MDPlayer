/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
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

namespace Driver.libsidplayfp.utils
{
    /**
     * SidDatabase
     * An utility class to deal with the songlength DataBase.
     */
    public class SidDatabase
    {



        //# include <stdint.h>
        //# include <memory>
        //# include "sidplayfp/siddefs.h"

        //class SID_EXTERN SidDatabase
        private iniParser m_parser;
        private string errorString;

        //public SidDatabase() { }
        //~SidDatabase() { }

        /**
         * Open the songlength DataBase.
         *
         * @param filename songlengthDB file name with full path.
         * @return false in case of errors, true otherwise.
         */
        //public bool open(string filename) { return false; }

        /**
         * Close the songlength DataBase.
         */
        //public void close() { }

        /**
         * Get the length of the current subtune.
         *
         * @param tune
         * @return tune length in seconds, -1 in case of errors.
         */
        //public Int32 length(SidTune tune) { return 0; }

        /**
         * Get the length of the selected subtune.
         *
         * @param md5 the md5 hash of the tune.
         * @param song the subtune.
         * @return tune length in seconds, -1 in case of errors.
         */
        //public Int32 length(byte[] md5, UInt32 song) { return 0; }

        /**
         * Get descriptive error message.
         */
        public string error() { return errorString; }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2016 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
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

        //# include <cctype>
        //# include <cstdlib>
        //# include "SidDatabase.h"
        //# include "sidplayfp/SidTune.h"
        //# include "sidplayfp/SidTuneInfo.h"
        //# include "iniParser.h"
        //# include "sidcxx11.h"

        private const string ERR_DATABASE_CORRUPT = "SID DATABASE ERROR: Database seems to be corrupt.";
        private const string ERR_NO_DATABASE_LOADED = "SID DATABASE ERROR: Songlength database not loaded.";
        private const string ERR_NO_SELECTED_SONG = "SID DATABASE ERROR: No song selected for retrieving song length.";
        private const string ERR_UNABLE_TO_LOAD_DATABASE = "SID DATABASE ERROR: Unable to load the songlegnth database.";

        private class parseError : Exception { };

        public SidDatabase()
        {
            m_parser = null;
            errorString = ERR_NO_DATABASE_LOADED;
        }

        ~SidDatabase()
        {
            // Needed to delete auto_ptr with complete type
        }

        public string parseTime(string str, long result)
        {
            string end;
            long minutes = strtol(str, out end, 10);

            if (end[0] != ':')
            {
                throw new parseError();
            }

            end = end.Substring(1);
            long seconds = strtol(end, out end, 10);
            result = (minutes * 60) + seconds;

            while (end[0] != ' ')
            {
                end = end.Substring(1);
            }

            return end;
        }

        private long strtol(string src, out string des, int p)
        {
            long ret = 0, n;
            int i;
            for (i = 0; i < src.Length; i++)
            {
                if (long.TryParse(src.Substring(0, 1 + i), out n))
                {
                    ret = n;
                    continue;
                }
                break;
            }

            des = src.Substring(i);
            return ret;
        }

        public bool open(string filename)
        {
            m_parser=new iniParser();

            if (!m_parser.open(filename))
            {
                close();
                errorString = ERR_UNABLE_TO_LOAD_DATABASE;
                return false;
            }

            return true;
        }

        public void close()
        {
            m_parser = null;
        }

        public Int32 length(sidplayfp.SidTune tune)
        {
            UInt32 song = tune.getInfo().currentSong();

            if (song == 0)
            {
                errorString = ERR_NO_SELECTED_SONG;
                return -1;
            }

            byte[] md5 = new byte[32 + 1];// MD5_LENGTH + 1];
            tune.createMD5(md5);
            return length(md5, song);
        }

        public Int32 length(byte[] md5, UInt32 song)
        {
            if (m_parser == null)
            {
                errorString = ERR_NO_DATABASE_LOADED;
                return -1;
            }

            // Read Time (and check times before hand)
            if (!m_parser.setSection("Database"))
            {
                errorString = ERR_DATABASE_CORRUPT;
                return -1;
            }

            string timeStamp = m_parser.getValue(md5);

            // If return is null then no entry found in database
            if (timeStamp == null)
            {
                errorString = ERR_DATABASE_CORRUPT;
                return -1;
            }

            string str = timeStamp;
            Int32 time = 0;

            for (UInt32 i = 0; i < song; i++)
            {
                // Validate Time
                try
                {
                    str = parseTime(str, time);
                }
                catch (parseError )
                {
                    errorString = ERR_DATABASE_CORRUPT;
                    return -1;
                }
            }

            return time;
        }




    }
}