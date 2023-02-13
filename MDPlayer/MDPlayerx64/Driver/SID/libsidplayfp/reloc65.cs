/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright (C) 2013-2014 Leandro Nini
 * Copyright (C) 2001 Dag Lem
 * Copyright (C) 1989-1997 Andr� Fachat (a.fachat@physik.tu-chemnitz.de)
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
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

/*
    Modified by Dag Lem <resid@nimrod.no>
    Relocate and extract text segment from memory buffer instead of file.
    For use with VICE VSID.

    Ported to c++ by Leandro Nini
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp
{
    /**
     * reloc65 -- A part of xa65 - 65xx/65816 cross-assembler and utility suite
     * o65 file relocator.
     */
    public class reloc65
    {



        public enum segment_t
        {
            WHOLE,
            TEXT,
            DATA,
            BSS,
            ZEROPAGE
        }

        private Int32 m_tbase, m_dbase, m_bbase, m_zbase;
        private Int32 m_tdiff, m_ddiff, m_bdiff, m_zdiff;
        private bool m_tflag, m_dflag, m_bflag, m_zflag;

        private segment_t m_extract;

        //private Int32 reldiff(byte s) { return 0; }

        /**
         * Relocate segment.
         *
         * @param buf segment
         * @param len segment size
         * @param rtab relocation table
         * @return a pointer to the next section
         */
        //private byte[] reloc_seg(byte[] buf, Int32 len, byte[] rtab) { return null; }

        /**
         * Relocate exported globals list.
         *
         * @param buf exported globals list
         * @return a pointer to the next section
         */
        //private byte[] reloc_globals(byte[] buf) { return null; }

        //public reloc65() { }

        /**
         * Select segment to relocate.
         * 
         * @param type the segment to relocate
         * @param addr new address
         */
        //public void setReloc(segment_t type, int addr) { }

        /**
         * Select segment to extract.
         * 
         * @param type the segment to extract
         */
        //public void setExtract(segment_t type) { }

        /**
         * Do the relocation.
         *
         * @param buf beffer containing o65 data
         * @param fsize size of the data
         */
        //public bool reloc(byte[][] buf, Int32[] fsize) { return false; }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright (C) 2013-2016 Leandro Nini
 * Copyright (C) 2001 Dag Lem
 * Copyright (C) 1989-1997 Andr� Fachat (a.fachat@physik.tu-chemnitz.de)
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
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

        //# include "reloc65.h"
        //# include <cstring>
        //# include "sidplayfp/siddefs.h"

        /// 16 bit header
        private const int HEADER_SIZE = (8 + 9 * 2);

        /// Magic number
        private byte[] o65hdr = new byte[] { 1, 0, (byte)'o', (byte)'6', (byte)'5' };

        /**
         * Read a 16 bit word from a buffer at specific location.
         *
         * @param buffer
         * @param idx
         */
        private Int32 getWord(byte[] buffer, Int32 idx)
        {
            return buffer[idx] | (buffer[idx + 1] << 8);
        }

        private Int32 getWord(Ptr<byte> buffer, Int32 idx)
        {
            return buffer[idx] | (buffer[idx + 1] << 8);
        }

        /**
         * Write a 16 bit word into a buffer at specific location.
         *
         * @param buffer
         * @param idx
         * @param value
         */
        private void setWord(byte[] buffer, Int32 idx, Int32 value)
        {
            buffer[idx] = (byte)(value & 0xff);
            buffer[idx + 1] = (byte)((value >> 8) & 0xff);
        }
        private void setWord(Ptr<byte> buffer, Int32 idx, Int32 value)
        {
            buffer[idx] = (byte)(value & 0xff);
            buffer[idx + 1] = (byte)((value >> 8) & 0xff);
        }

        /**
         * Get the size of header options section.
         *
         * @param buf
         */
        private Int32 read_options(byte[] buf, int ptr)
        {
            int l = 0;

            byte c = buf[0 + ptr];
            while (c != 0)
            {
                l += c;
                c = buf[l + ptr];
            }
            return ++l;
        }

        /**
         * Get the size of undefined references list.
         *
         * @param buf
         */
        private Int32 read_undef(Ptr<byte> buf)
        {
            Int32 l = 2;

            Int32 n = getWord(buf, 0);
            while (n != 0)
            {
                n--;
                while (buf[l++] == 0) { }
            }
            return l;
        }

        public reloc65()
        {
            m_tbase = (0);
            m_dbase = (0);
            m_bbase = (0);
            m_zbase = (0);
            m_tflag = (false);
            m_dflag = (false);
            m_bflag = (false);
            m_zflag = (false);
            m_extract = segment_t.WHOLE;
        }

        public void setReloc(segment_t type, Int32 addr)
        {
            switch (type)
            {
                case segment_t.TEXT:
                    m_tflag = true;
                    m_tbase = addr;
                    break;
                case segment_t.DATA:
                    m_dflag = true;
                    m_dbase = addr;
                    break;
                case segment_t.BSS:
                    m_bflag = true;
                    m_bbase = addr;
                    break;
                case segment_t.ZEROPAGE:
                    m_zflag = true;
                    m_zbase = addr;
                    break;
                default:
                    break;
            }
        }

        public void setExtract(segment_t type)
        {
            m_extract = type;
        }

        public bool reloc(ref byte[] aryBuf,ref Int32 iFsize)
        {
            Ptr<byte> buf = new Ptr<byte>(aryBuf, 0);
            Int32 fsize = iFsize;

            Ptr<byte> tmpBuf = buf;

            if (mem.memcmp(tmpBuf.buf, tmpBuf.ptr, o65hdr, 0, 5) != 0)
            {
                return false;
            }

            Int32 mode = getWord(tmpBuf, 6);
            if ((mode & 0x2000) != 0    // 32 bit size not supported
                || (mode & 0x4000) != 0) // pagewise relocation not supported
            {
                return false;
            }

            int hlen = HEADER_SIZE + read_options(tmpBuf.buf, tmpBuf.ptr + HEADER_SIZE);

            int tbase = getWord(tmpBuf, 8);
            int tlen = getWord(tmpBuf, 10);
            m_tdiff = m_tflag ? m_tbase - tbase : 0;

            int dbase = getWord(tmpBuf, 12);
            int dlen = getWord(tmpBuf, 14);
            m_ddiff = m_dflag ? m_dbase - dbase : 0;

            int bbase = getWord(tmpBuf, 16);
            int blen = getWord(tmpBuf, 18);
            m_bdiff = m_bflag ? m_bbase - bbase : 0;

            int zbase = getWord(tmpBuf, 20);
            int zlen = getWord(tmpBuf, 21);
            m_zdiff = m_zflag ? m_zbase - zbase : 0;

            Ptr<byte> segt = new Ptr<byte>(tmpBuf.buf, tmpBuf.ptr + hlen);                    // Text segment
            Ptr<byte> segd = new Ptr<byte>(segt.buf, segt.ptr + tlen);                      // Data segment
            Ptr<byte> utab = new Ptr<byte>(segd.buf, segd.ptr + dlen);                      // Undefined references list

            Ptr<byte> rttab = new Ptr<byte>(utab.buf,utab.ptr+ read_undef(utab));         // Text relocation table

            Ptr<byte> rdtab = reloc_seg(segt, tlen, rttab);    // Data relocation table
            Ptr<byte> extab = reloc_seg(segd, dlen, rdtab);    // Exported globals list

            reloc_globals(extab);

            if (m_tflag)
            {
                setWord(tmpBuf, 8, m_tbase);
            }
            if (m_dflag)
            {
                setWord(tmpBuf, 12, m_dbase);
            }
            if (m_bflag)
            {
                setWord(tmpBuf, 16, m_bbase);
            }
            if (m_zflag)
            {
                setWord(tmpBuf, 20, m_zbase);
            }

            bool ret = false;
            switch (m_extract)
            {
                case segment_t.WHOLE:
                    ret= true;
                    break;
                case segment_t.TEXT:
                    buf = segt;
                    fsize = tlen;
                    ret= true;
                    break;
                case segment_t.DATA:
                    buf = segd;
                    fsize = dlen;
                    ret= true;
                    break;
                default:
                    return false;
            }

            aryBuf = new byte[fsize];
            iFsize = fsize;
            for (int i = 0; i < fsize; i++) aryBuf[i] = buf[i];

            return ret;
        }

        private Int32 reldiff(byte s)
        {
            switch (s)
            {
                case 2: return m_tdiff;
                case 3: return m_ddiff;
                case 4: return m_bdiff;
                case 5: return m_zdiff;
                default: return 0;
            }
        }

        private Ptr<byte> reloc_seg(Ptr<byte> buf, int len, Ptr<byte> rtab)
        {
            Int32 adr = -1;
            while (rtab[0]!=0)//(rtab.ptr < rtab.Count)
            {
                if ((rtab[0] & 255) == 255)
                {
                    adr += 254;
                    rtab.AddPtr(1);
                }
                else
                {
                    adr += rtab[0] & 255;
                    rtab.AddPtr(1);
                    byte type = (byte)(rtab[0] & 0xe0);
                    byte seg = (byte)(rtab[0] & 0x07);
                    rtab.AddPtr(1);
                    switch (type)
                    {
                        case 0x80:
                            {
                                Int32 oldVal = getWord(buf, adr);
                                Int32 newVal = oldVal + reldiff(seg);
                                setWord(buf, adr, newVal);
                                break;
                            }
                        case 0x40:
                            {
                                Int32 oldVal = buf[adr] * 256 + rtab[0];
                                Int32 newVal = oldVal + reldiff(seg);
                                buf[adr] = (byte)((newVal >> 8) & 255);
                                rtab[0] = (byte)(newVal & 255);
                                rtab.AddPtr(1);
                                break;
                            }
                        case 0x20:
                            {
                                Int32 oldVal = buf[adr];
                                Int32 newVal = oldVal + reldiff(seg);
                                buf[adr] = (byte)(newVal & 255);
                                break;
                            }
                    }
                    if (seg == 0)
                    {
                        rtab.AddPtr(2);
                    }
                }
                //Console.WriteLine("buf[{0}]={1}",adr,buf[adr]);
                if (adr > len)
                {
                    // Warning: relocation table entries past segment end!
                }
            }

            rtab.AddPtr(1);
            return rtab;
        }

        private Ptr<byte> reloc_globals(Ptr<byte> buf)
        {
            int n = getWord(buf, 0);
            buf.AddPtr(2);

            while (n != 0)
            {
                while (buf[buf.ptr] != 0) { buf.AddPtr(1); }
                byte seg = buf[buf.ptr];
                Int32 oldVal = getWord(buf, 1);
                Int32 newVal = oldVal + reldiff(seg);
                setWord(buf, 1, newVal);
                buf.AddPtr(3);
                n--;
            }

            return buf;
        }

    }
}