/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2014 Leandro Nini <drfiemost@users.sourceforge.net>
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
    public sealed class PSID : SidTuneBase
    {



        //# include <stdint.h>
        //# include "SidTuneBase.h"
        //# include "sidplayfp/SidTune.h"
        //# include "sidcxx11.h"
        //struct psidHeader;

        private byte[] m_md5 = new byte[SidTune.MD5_LENGTH + 1];

        /**
         * Load PSID file.
         *
         * @throw loadError
         */
        //private void tryLoad(ref psidHeader pHeader) { }

        /**
         * Read PSID file header.
         *
         * @throw loadError
         */
        //private static void readHeader(byte[] dataBuf, ref psidHeader hdr) { }

        PSID() { }
        ~PSID() { }

        /**
         * @return pointer to a SidTune or 0 if not a PSID file
         * @throw loadError if PSID file is corrupt
         */
        //private static SidTuneBase load(ref byte[] dataBuf) { return null; }

        //public override byte[] createMD5(byte[] md5) { return null; }

        // prevent copying
        PSID(ref PSID p) { }
        private PSID opeEquel(ref PSID p) { return null; }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2015 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //#include "PSID.h"
        //#include <cstring>
        //#include <string>
        //#include <memory>
        //#include "sidplayfp/SidTuneInfo.h"
        //#include "sidendian.h"
        //#include "sidmd5.h"

        public const int PSID_MAXSTRLEN = 32;


        // Header has been extended for 'RSID' format
        // The following changes are present:
        //     id = 'RSID'
        //     version = 2, 3 or 4
        //     play, load and speed reserved 0
        //     psidspecific flag is called C64BASIC flag
        //     init cannot be under ROMS/IO memory area
        //     load address cannot be less than $07E8
        //     info strings may be 32 characters long without trailing zero

        public class psidHeader           // all values are big-endian
        {
            public UInt32 id;                   // 'PSID' or 'RSID' (ASCII)
            public UInt16 version;              // 1, 2, 3 or 4
            public UInt16 data;                 // 16-bit offset to binary data in file
            public UInt16 load;                 // 16-bit C64 address to load file to
            public UInt16 init;                 // 16-bit C64 address of init subroutine
            public UInt16 play;                 // 16-bit C64 address of play subroutine
            public UInt16 songs;                // number of songs
            public UInt16 start;                // start song out of [1..256]
            public UInt32 speed;                // 32-bit speed info
                                                // bit: 0=50 Hz, 1=CIA 1 Timer A (default: 60 Hz)
            public byte[] name = new byte[PSID_MAXSTRLEN];     // ASCII strings, 31 characters long and
            public byte[] author = new byte[PSID_MAXSTRLEN];   // terminated by a trailing zero
            public byte[] released = new byte[PSID_MAXSTRLEN]; //

            public UInt16 flags;                // only version >= 2
            public byte relocStartPage;        // only version >= 2ng
            public byte relocPages;            // only version >= 2ng
            public byte sidChipBase2;          // only version >= 3
            public byte sidChipBase3;          // only version >= 4
        }

        public enum Psid
        {
            PSID_MUS = 1 << 0,
            PSID_SPECIFIC = 1 << 1, // These two are mutally exclusive
            PSID_BASIC = 1 << 1,
            PSID_CLOCK = 3 << 2,
            PSID_SIDMODEL = 3 << 4
        }

        public enum Psid_Clock
        {
            PSID_CLOCK_UNKNOWN = 0,
            PSID_CLOCK_PAL = 1 << 2,
            PSID_CLOCK_NTSC = 1 << 3,
            PSID_CLOCK_ANY = PSID_CLOCK_PAL | PSID_CLOCK_NTSC
        };

        public enum Psid_Sidmodel
        {
            PSID_SIDMODEL_UNKNOWN = 0,
            PSID_SIDMODEL_6581 = 1,
            PSID_SIDMODEL_8580 = 2,
            PSID_SIDMODEL_ANY = 3//PSID_SIDMODEL_6581 | PSID_SIDMODEL_8580
        };

        // Format strings
        public const string TXT_FORMAT_PSID = "PlaySID one-file format (PSID)";
        public const string TXT_FORMAT_RSID = "Real C64 one-file format (RSID)";
        public const string TXT_UNKNOWN_PSID = "Unsupported PSID version";
        public const string TXT_UNKNOWN_RSID = "Unsupported RSID version";

        public const int psid_headerSize = 118;
        public const int psidv2_headerSize = psid_headerSize + 6;

        // Magic fields
        public const UInt32 PSID_ID = 0x50534944;
        public const UInt32 RSID_ID = 0x52534944;

        /**
         * Decode SID model flags.
         */
        public SidTuneInfo.model_t getSidModel(UInt16 modelFlag)
        {
            if ((modelFlag & (UInt16)Psid_Sidmodel.PSID_SIDMODEL_ANY) == (UInt16)Psid_Sidmodel.PSID_SIDMODEL_ANY)
                return SidTuneInfo.model_t.SIDMODEL_ANY;

            if ((modelFlag & (UInt16)Psid_Sidmodel.PSID_SIDMODEL_6581) != 0)
                return SidTuneInfo.model_t.SIDMODEL_6581;

            if ((modelFlag & (UInt16)Psid_Sidmodel.PSID_SIDMODEL_8580) != 0)
                return SidTuneInfo.model_t.SIDMODEL_8580;

            return SidTuneInfo.model_t.SIDMODEL_UNKNOWN;
        }

        /**
         * Check if extra SID addres is valid for PSID specs.
         */
        private bool validateAddress(byte address)
        {
            // Only even values are valid.
            if ((address & 1) != 0)
                return false;

            // Ranges $00-$41 ($D000-$D410) and $80-$DF ($D800-$DDF0) are invalid.
            // Any invalid value means that no second SID is used, like $00.
            if (address <= 0x41
                    || (address >= 0x80 && address <= 0xdf))
                return false;

            return true;
        }

        public static SidTuneBase load(ref byte[] dataBuf)
        {
            // File format check
            if (dataBuf.Length < 4)
            {
                return null;
            }

            UInt32 magic = sidendian.endian_big32(dataBuf);
            if ((magic != PSID_ID)
                && (magic != RSID_ID))
            {
                return null;
            }

            psidHeader pHeader = new psidHeader();
            readHeader(dataBuf, ref pHeader);

            PSID tune = new PSID();
            tune.tryLoad(ref pHeader);

            return tune;
        }

        private static void readHeader(byte[] dataBuf, ref psidHeader hdr)
        {
            // Due to security concerns, input must be at least as long as version 1
            // header plus 16-bit C64 load address. That is the area which will be
            // accessed.
            if (dataBuf.Length < (psid_headerSize + 2))
            {
                throw new loadError(ERR_TRUNCATED);
            }

            // Read v1 fields
            hdr.id = sidendian.endian_big32(new Ptr<byte>(dataBuf, 0));
            hdr.version = sidendian.endian_big16(new Ptr<byte>(dataBuf, 4));
            hdr.data = sidendian.endian_big16(new Ptr<byte>(dataBuf, 6));
            hdr.load = sidendian.endian_big16(new Ptr<byte>(dataBuf, 8));
            hdr.init = sidendian.endian_big16(new Ptr<byte>(dataBuf, 10));
            hdr.play = sidendian.endian_big16(new Ptr<byte>(dataBuf, 12));
            hdr.songs = sidendian.endian_big16(new Ptr<byte>(dataBuf, 14));
            hdr.start = sidendian.endian_big16(new Ptr<byte>(dataBuf, 16));
            hdr.speed = sidendian.endian_big32(new Ptr<byte>(dataBuf, 18));
            mem.memcpy(ref hdr.name, new Ptr<byte>(dataBuf, 22), PSID_MAXSTRLEN);
            mem.memcpy(ref hdr.author, new Ptr<byte>(dataBuf, 54), PSID_MAXSTRLEN);
            mem.memcpy(ref hdr.released, new Ptr<byte>(dataBuf, 86), PSID_MAXSTRLEN);

            if (hdr.version >= 2)
            {
                if (dataBuf.Length < (psidv2_headerSize + 2))
                {
                    throw new loadError(ERR_TRUNCATED);
                }

                // Read v2/3/4 fields
                hdr.flags = sidendian.endian_big16(new Ptr<byte>(dataBuf, 118));
                hdr.relocStartPage = dataBuf[120];
                hdr.relocPages = dataBuf[121];
                hdr.sidChipBase2 = dataBuf[122];
                hdr.sidChipBase3 = dataBuf[123];
            }
        }

        private void tryLoad(ref psidHeader pHeader)
        {
            SidTuneInfo.compatibility_t compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_C64;

            // Require a valid ID and version number.
            if (pHeader.id == PSID_ID)
            {
                switch (pHeader.version)
                {
                    case 1:
                        compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_PSID;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        break;
                    default:
                        throw new loadError(TXT_UNKNOWN_PSID);
                }
                info.m_formatString = TXT_FORMAT_PSID;
            }
            else if (pHeader.id == RSID_ID)
            {
                switch (pHeader.version)
                {
                    case 2:
                    case 3:
                    case 4:
                        break;
                    default:
                        throw new loadError(TXT_UNKNOWN_RSID);
                }
                info.m_formatString = TXT_FORMAT_RSID;
                compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_R64;
            }

            fileOffset = pHeader.data;
            info.m_loadAddr = pHeader.load;
            info.m_initAddr = pHeader.init;
            info.m_playAddr = pHeader.play;
            info.m_songs = pHeader.songs;
            info.m_startSong = pHeader.start;
            info.m_compatibility = compatibility;
            info.m_relocPages = 0;
            info.m_relocStartPage = 0;

            UInt32 speed = pHeader.speed;
            SidTuneInfo.clock_t clock = SidTuneInfo.clock_t.CLOCK_UNKNOWN;

            bool musPlayer = false;

            if (pHeader.version >= 2)
            {
                UInt16 flags = pHeader.flags;

                // Check clock
                if ((flags & (UInt16)Psid.PSID_MUS) != 0)
                {   // MUS tunes run at any speed
                    clock = SidTuneInfo.clock_t.CLOCK_ANY;
                    musPlayer = true;
                }
                else
                {
                    switch ((flags & (UInt16)Psid.PSID_CLOCK))
                    {
                        case (UInt16)Psid_Clock.PSID_CLOCK_ANY:
                            clock = SidTuneInfo.clock_t.CLOCK_ANY;
                            break;
                        case (UInt16)Psid_Clock.PSID_CLOCK_PAL:
                            clock = SidTuneInfo.clock_t.CLOCK_PAL;
                            break;
                        case (UInt16)Psid_Clock.PSID_CLOCK_NTSC:
                            clock = SidTuneInfo.clock_t.CLOCK_NTSC;
                            break;
                        default:
                            break;
                    }
                }

                // These flags are only available for the appropriate
                // file formats
                switch (compatibility)
                {
                    case SidTuneInfo.compatibility_t.COMPATIBILITY_C64:
                        if ((flags & (UInt16)Psid.PSID_SPECIFIC) != 0)
                            info.m_compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_PSID;
                        break;
                    case SidTuneInfo.compatibility_t.COMPATIBILITY_R64:
                        if ((flags & (UInt16)Psid.PSID_BASIC) != 0)
                            info.m_compatibility = SidTuneInfo.compatibility_t.COMPATIBILITY_BASIC;
                        break;
                    default:
                        break;
                }

                info.m_clockSpeed = clock;

                info.m_sidModels[0] = getSidModel((UInt16)(flags >> 4));

                info.m_relocStartPage = pHeader.relocStartPage;
                info.m_relocPages = pHeader.relocPages;

                if (pHeader.version >= 3)
                {
                    if (validateAddress(pHeader.sidChipBase2))
                    {
                        info.m_sidChipAddresses.Add((UInt16)(0xd000 | (pHeader.sidChipBase2 << 4)));

                        info.m_sidModels.Add(getSidModel((UInt16)(flags >> 6)));
                    }

                    if (pHeader.version >= 4)
                    {
                        if (pHeader.sidChipBase3 != pHeader.sidChipBase2
                            && validateAddress(pHeader.sidChipBase3))
                        {
                            info.m_sidChipAddresses.Add((UInt16)(0xd000 | (pHeader.sidChipBase3 << 4)));

                            info.m_sidModels.Add((SidTuneInfo.model_t)(getSidModel((UInt16)(flags >> 8))));
                        }
                    }
                }
            }

            // Check reserved fields to force real c64 compliance
            // as required by the RSID specification
            if (compatibility == SidTuneInfo.compatibility_t.COMPATIBILITY_R64)
            {
                if ((info.m_loadAddr != 0)
                    || (info.m_playAddr != 0)
                    || (speed != 0))
                {
                    throw new loadError(ERR_INVALID);
                }

                // Real C64 tunes appear as CIA
                speed = 0xffffffff;// ~0;
            }

            // Create the speed/clock setting table.
            convertOldStyleSpeedToTables(speed, (Int64)clock);

            // Copy info strings.
            info.m_infoString.Add(Encoding.ASCII.GetString(pHeader.name, 0, PSID_MAXSTRLEN));
            info.m_infoString.Add(Encoding.ASCII.GetString(pHeader.author, 0, PSID_MAXSTRLEN));
            info.m_infoString.Add(Encoding.ASCII.GetString(pHeader.released, 0, PSID_MAXSTRLEN));

            if (musPlayer)
                throw new loadError("Compute!'s Sidplayer MUS data is not supported yet"); // TODO
        }

        public override byte[] createMD5(byte[] md5)
        {
            if (md5 == null)
                md5 = m_md5;

            md5[0] = (byte)'\0';

            try
            {
                // Include C64 data.
                sidmd5 myMD5=new sidmd5();
                byte[] bcache = new byte[(int)info.m_c64dataLen];
                for(int i = 0; i < info.m_c64dataLen; i++)
                {
                    bcache[i] = cache[(int)fileOffset + i];
                }
                myMD5.append(bcache, (int)info.m_c64dataLen);

                byte[] tmp = new byte[2];
                // Include INIT and PLAY address.
                sidendian.endian_little16(tmp, info.m_initAddr);
                myMD5.append(tmp, tmp.Length);
                sidendian.endian_little16(tmp, info.m_playAddr);
                myMD5.append(tmp, tmp.Length);

                // Include number of songs.
                sidendian.endian_little16(tmp, (UInt16)info.m_songs);
                myMD5.append(tmp, tmp.Length);

                {
                    // Include song speed for each song.
                    UInt32 currentSong = info.m_currentSong;
                    for (UInt32 s = 1; s <= info.m_songs; s++)
                    {
                        selectSong(s);
                        byte songSpeed = (byte)info.m_songSpeed;
                        bcache = new byte[1] { songSpeed };
                        myMD5.append(bcache, 1);
                    }
                    // Restore old song
                    selectSong(currentSong);
                }

                // Deal with PSID v2NG clock speed flags: Let only NTSC
                // clock speed change the MD5 fingerprint. That way the
                // fingerprint of a PAL-speed sidtune in PSID v1, v2, and
                // PSID v2NG format is the same.
                if (info.m_clockSpeed == SidTuneInfo.clock_t.CLOCK_NTSC)
                {
                    const byte ntsc_val = 2;
                    bcache = new byte[1] { ntsc_val };
                    myMD5.append(bcache, 1);
                }

                // NB! If the fingerprint is used as an index into a
                // song-lengths database or cache, modify above code to
                // allow for PSID v2NG files which have clock speed set to
                // SIDTUNE_CLOCK_ANY. If the SID player program fully
                // supports the SIDTUNE_CLOCK_ANY setting, a sidtune could
                // either create two different fingerprints depending on
                // the clock speed chosen by the player, or there could be
                // two different values stored in the database/cache.

                myMD5.finish();

                // Get fingerprint.
                md5 = Encoding.ASCII.GetBytes(myMD5.getDigest());//,0, SidTune.MD5_LENGTH);
                md5[SidTune.MD5_LENGTH] = (byte)'\0';
            }
            catch //(md5Error ex)
            {
                return null;
            }

            return md5;
        }

    }

}