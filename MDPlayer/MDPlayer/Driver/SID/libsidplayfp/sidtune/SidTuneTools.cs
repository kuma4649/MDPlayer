/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright (C) Michael Schwendt <mschwendt@yahoo.com>
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.sidtune
{
    public class SidTuneTools
    {



        //# include <cstring>

        /**
         * Return pointer to file name position in complete path.
         */
        //public UInt64 fileNameWithoutPath(string s) { return 0; }

        /**
         * Return pointer to file name position in complete path.
         * Special version: file separator = forward slash.
         */
        //public UInt64 slashedFileNameWithoutPath(string s) { return 0; }

        /**
         * Return pointer to file name extension in path.
         * Searching backwards until first dot is found.
         */
        //public string fileExtOfPath(string s) { return null; }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright (C) Michael Schwendt <mschwendt@yahoo.com>
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

        //#include "SidTuneTools.h"
        //#include "SidTuneCfg.h"

        // Return pointer to file name position in complete path.
        public static UInt64 fileNameWithoutPath(string s)
        {
            Int64 last_slash_pos = -1;
            Int64 length = s.Length;
            for (Int32 pos = 0; pos < length; pos++)
            {
                //#if defined(SID_FS_IS_COLON_AND_BACKSLASH_AND_SLASH)
                if (s[pos] == ':' || s[pos] == '\\'
                    || s[pos] == '/')
                //#elif defined(SID_FS_IS_COLON_AND_SLASH)
                //        if (s[pos] == ':' || s[pos] == '/')
                //#elif defined(SID_FS_IS_SLASH)
                //        if (s[pos] == '/')
                //#elif defined(SID_FS_IS_BACKSLASH)
                //        if (s[pos] == '\\')
                //#elif defined(SID_FS_IS_COLON)
                //        if (s[pos] == ':')
                //#else
                //#error Missing file/path separator definition.
                //#endif
                {
                    last_slash_pos = pos;
                }
            }
            return (UInt64)(last_slash_pos + 1);
        }

        // Return pointer to file name position in complete path.
        // Special version: file separator = forward slash.
        public static UInt64 slashedFileNameWithoutPath(string s)
        {
            Int64 last_slash_pos = -1;
            Int64 length = s.Length;
            for (Int32 pos = 0; pos < length; pos++)
            {
                if (s[pos] == '/')
                {
                    last_slash_pos = pos;
                }
            }
            return (UInt64)(last_slash_pos + 1);
        }

        // Return pointer to file name extension in path.
        // The backwards-version.
        public static string fileExtOfPath(string s)
        {
            Int32 last_dot_pos = s.Length;  // assume no dot and append
            for (Int32 pos = last_dot_pos; pos > 0; pos--)
            {
                if (s[pos - 1] == '.')
                {
                    last_dot_pos = pos - 1;
                    break;
                }
            }
            return s.Substring(last_dot_pos);
        }

    }

}