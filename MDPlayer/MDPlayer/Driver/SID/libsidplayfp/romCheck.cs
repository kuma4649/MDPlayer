/*
 * This file is part of libsidplayfp, a SID player engine.
 *
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

namespace Driver.libsidplayfp
{
    /**
     * Utility class to identify known ROM images through their md5 checksum.
     */
    public class romCheck
    {



        //# include <stdint.h>
        //# include <map>
        //# include <string>
        //# include <utility>
        //# include "sidmd5.h"

        //private Dictionary<string, byte[]> md5map;

        /**
         * Maps checksums to respective ROM description.
         * Must be filled by derived class.
         */
        private Dictionary<string, string> m_checksums=new Dictionary<string, string>();

        /**
         * Pointer to the ROM buffer
         */
        private byte[] m_rom;

        /**
         * Size of the ROM buffer.
         */
        private UInt32 m_size;

        private romCheck()
        {
        }

        /**
         * Calculate the md5 digest.
         */
        private string checksum()
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] bs = md5.ComputeHash(m_rom);
                md5.Clear();
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                foreach (byte b in bs)
                {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString();


                //sidmd5 md5 = new sidmd5();
                //md5.append(m_rom, (Int32)m_size);
                //md5.finish();

                //return md5.getDigest();
            }
            catch //(md5Error e)
            {
                return "";//string ();
            }
        }

        /**
         * Construct the class.
         *
         * @param rom pointer to the ROM buffer
         * @param size size of the ROM buffer
         */
        protected romCheck(byte[] rom, int size)
        {
            m_rom = rom;
            m_size = (UInt32)size;
        }

        protected void add(string md5, string desc)
        {
            m_checksums.Add(md5, desc);
        }

        /**
         * Get ROM description.
         *
         * @return the ROM description or "Unknown Rom".
         */
        public string info()
        {
            //md5map::const_iterator res = m_checksums.find(checksum());
            //return (res != m_checksums.end()) ? res->second : "Unknown Rom";
            string key = checksum();
            if (m_checksums.ContainsKey(key))
            {
                return m_checksums[key];
            }
            return "Unknown Rom";
        }
    }

    /**
     * romCheck implementation specific for kernal ROM.
     */
    public class kernalCheck : romCheck
    {
        public kernalCheck(byte[] kernal) : base(kernal, 0x2000)
        {
            add("1ae0ea224f2b291dafa2c20b990bb7d4", "C64 KERNAL first revision");
            add("7360b296d64e18b88f6cf52289fd99a1", "C64 KERNAL second revision");
            add("479553fd53346ec84054f0b1c6237397", "C64 KERNAL second revision (Japanese)");
            add("39065497630802346bce17963f13c092", "C64 KERNAL third revision");
            add("27e26dbb267c8ebf1cd47105a6ca71e7", "C64 KERNAL third revision (Swedish)");
            add("187b8c713b51931e070872bd390b472a", "Commodore SX-64 KERNAL");
            add("b7b1a42e11ff8efab4e49afc4faedeee", "Commodore SX-64 KERNAL (Swedish)");
            add("3abc938cac3d622e1a7041c15b928707", "Cockroach Turbo-ROM");
            add("631ea2ca0dcda414a90aeefeaf77fe45", "Cockroach Turbo-ROM (SX-64)");
            add("a9de1832e9be1a8c60f4f979df585681", "Datel DOS-ROM 1.2");
            add("da43563f218b46ece925f221ef1f4bc2", "Datel Mercury 3 (NTSC)");
            add("b7dc8ed82170c81773d4f5dc8069a000", "Datel Turbo ROM II (PAL)");
            add("6b309c76473dcf555c52c598c6a51011", "Dolphin DOS v1.0");
            add("c3c93b9a46f116acbfe7ee147c338c60", "Dolphin DOS v2.0-1 AU");
            add("2a441f4abd272d50f94b43c7ff3cc629", "Dolphin DOS v2.0-1");
            add("c7a175217e67dcb425feca5fcf2a01cc", "Dolphin DOS v2.0-2");
            add("7a9b1040cfbe769525bb9cdc28427be6", "Dolphin DOS v2.0-3");
            add("fc8fb5ec89b34ae41c8dc20907447e06", "Dolphin DOS v3.0");
            add("9a6e1c4b99c6f65323aa96940c7eb7f7", "ExOS v3 fertig");
            add("3241a4fcf2ba28ba3fc79826bc023814", "ExOS v3");
            add("cffd2616312801da56bcc6728f0e39ca", "ExOS v4");
            add("e6e2bb24a0fa414182b0fd149bde689d", "TurboAccess");
            add("c5c5990f0826fcbd372901e761fab1b7", "TurboTrans v3.0-1");
            add("042ffc11383849bdf0e600474cefaaaf", "TurboTrans v3.0-2");
            add("9d62852013fc2c29c3111c765698664b", "Turbo-Process US");
            add("f9c9838e8d6752dc6066a8c9e6c2e880", "Turbo-Process");
        }
    }

    /**
     * romCheck implementation specific for basic ROM.
     */
    public class basicCheck : romCheck
    {
        public basicCheck(byte[] basic) : base(basic, 0x2000)
        {
            add("57af4ae21d4b705c2991d98ed5c1f7b8", "C64 BASIC V2");
        }
    }

    /**
     * romCheck implementation specific for character generator ROM.
     */
    public class chargenCheck : romCheck
    {
        public chargenCheck(byte[] chargen) : base(chargen, 0x1000)
        {
            add("12a4202f5331d45af846af6c58fba946", "C64 character generator");
            add("cf32a93c0a693ed359a4f483ef6db53d", "C64 character generator (Japanese)");
        }
    }


}
