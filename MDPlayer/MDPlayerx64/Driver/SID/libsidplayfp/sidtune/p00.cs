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
    public sealed class p00 : SidTuneBase
    {



        //# include "SidTuneBase.h"
        //# include "sidcxx11.h"
        //struct X00Header;

        p00() { }

        //private void load(byte[] format, X00Header pHeader) { }

        /**
         * @return pointer to a SidTune or 0 if not a PC64 file
         * @throw loadError if PC64 file is corrupt
         */
        //public static SidTuneBase load(string fileName, byte[] dataBuf) { return null; }

        ~p00() { }

        // prevent copying
        private p00(ref p00 p) { }
        private p00 opeEquel(ref p00 p) { return null; }



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

        //#include "p00.h"
        //#include <stdint.h>
        //#include <cstring>
        //#include <cctype>
        //#include <memory>
        //#include "sidplayfp/SidTuneInfo.h"
        //#include "SmartPtr.h"
        //#include "SidTuneTools.h"

        public const Int32 X00_ID_LEN = 8;
        public const Int32 X00_NAME_LEN = 17;

        // File format from PC64. PC64 automatically generates
        // the filename from the cbm name (16 to 8 conversion)
        // but we only need to worry about that when writing files
        // should we want pc64 compatibility.  The extension numbers
        // are just an index to try to avoid repeats.  Name conversion
        // works by creating an initial filename from alphanumeric
        // and ' ', '-' characters only with the later two being
        // converted to '_'.  Then it parses the filename
        // from end to start removing characters stopping as soon
        // as the filename becomes <= 8.  The removal of characters
        // occurs in three passes, the first removes all '_', then
        // vowels and finally numerics.  If the filename is still
        // greater than 8 it is truncated.

        public class X00Header
        {
            public byte[] id = new byte[X00_ID_LEN];     // 'C64File' (ASCII)
            public byte[] name = new byte[X00_NAME_LEN]; // C64 name (PETSCII)
            public byte length;             // Rel files only (Bytes/Record),
                                            // should be 0 for all other types
        };

        public enum X00Format
        {
            X00_DEL,
            X00_SEQ,
            X00_PRG,
            X00_USR,
            X00_REL
        }

        // Format strings
        private const string TXT_FORMAT_DEL = "Unsupported tape image file (DEL)";
        private const string TXT_FORMAT_SEQ = "Unsupported tape image file (SEQ)";
        private const string TXT_FORMAT_PRG = "Tape image file (PRG)";
        private const string TXT_FORMAT_USR = "Unsupported USR file (USR)";
        private const string TXT_FORMAT_REL = "Unsupported tape image file (REL)";

        // Magic field
        private const string P00_ID = "C64File";


        public static SidTuneBase load(string fileName, byte[] dataBuf)
        {
            string ext = SidTuneTools.fileExtOfPath(fileName);

            // Combined extension & magic field identification
            if (ext.Length != 4)
                return null;

            //if (!isdigit(ext[2]) || !isdigit(ext[3]))
                //return null;
            if ("0123456789".IndexOf(ext[2]) < 0 || "0123456789".IndexOf(ext[3]) < 0)
                return null;

            string format = null;
            X00Format type;

            switch (ext[1].ToString().ToUpper()[0])
            {
                case 'D':
                    type = X00Format.X00_DEL;
                    format = TXT_FORMAT_DEL;
                    break;
                case 'S':
                    type = X00Format.X00_SEQ;
                    format = TXT_FORMAT_SEQ;
                    break;
                case 'P':
                    type = X00Format.X00_PRG;
                    format = TXT_FORMAT_PRG;
                    break;
                case 'U':
                    type = X00Format.X00_USR;
                    format = TXT_FORMAT_USR;
                    break;
                case 'R':
                    type = X00Format.X00_REL;
                    format = TXT_FORMAT_REL;
                    break;
                default:
                    return null;
            }

            // Verify the file is what we think it is
            Int32 bufLen = dataBuf.Length;
            if (bufLen < X00_ID_LEN)
                return null;

            X00Header pHeader = new X00Header();
            mem.memcpy(ref pHeader.id, dataBuf, X00_ID_LEN);
            Ptr<byte> p = new Ptr<byte>(pHeader.name, 0);
            mem.memcpy(ref p, new Ptr<byte>(dataBuf, X00_ID_LEN), X00_NAME_LEN);
            pHeader.length = dataBuf[X00_ID_LEN + X00_NAME_LEN];

            if (Encoding.ASCII.GetString(pHeader.id) == P00_ID)
                return null;

            // File types current supported
            if (type != X00Format.X00_PRG)
                throw new loadError("Not a PRG inside X00");

            if (bufLen < 26+2)//sizeof(X00Header) + 2)
                throw new loadError(ERR_TRUNCATED);

            p00 tune = new p00();
            tune.load(Encoding.ASCII.GetBytes(format), pHeader);

            return tune;
        }

        public void load(byte[] format, X00Header pHeader)
        {
            info.m_formatString = Encoding.ASCII.GetString(format);

            {   // Decode file name
                Ptr<byte> spPet = new Ptr<byte>(pHeader.name, X00_NAME_LEN);
                info.m_infoString.Add(petsciiToAscii(ref spPet));
            }

            // Automatic settings
            fileOffset = X00_ID_LEN + X00_NAME_LEN + 1;
            info.m_songs = 1;
            info.m_startSong = 1;
            info.m_compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_BASIC;

            // Create the speed/clock setting table.
            convertOldStyleSpeedToTables(0xffffffff, (Int64)info.m_clockSpeed);// ~0, info.m_clockSpeed);
        }

    }

}