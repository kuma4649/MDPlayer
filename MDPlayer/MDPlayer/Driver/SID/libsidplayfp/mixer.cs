/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright (C) 2000 Simon White
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp
{
    /**
     * This class implements the mixer.
     */
    public class Mixer
    {



        //# include "sidcxx11.h"
        //# include <stdint.h>
        //# include <cstdlib>
        //# include <vector>
        //class sidemu;

        /// Maximum number of supported SIDs
        public const UInt32 MAX_SIDS = 3;

        public const Int32 SCALE_FACTOR = 1 << 16;
        //# ifdef HAVE_CXX11
        public const double SQRT_0_5 = 0.70710678118654746;
        //#else
        //#define SQRT_0_5 0.70710678118654746
        //#endif
        public const Int32 C1 = (Int32)(1.0 / (1.0 + SQRT_0_5) * SCALE_FACTOR);
        public const Int32 C2 = (Int32)(SQRT_0_5 / (1.0 + SQRT_0_5) * SCALE_FACTOR);

        private delegate Int32 mixer_func_t(); //typedef Int32(Mixer::* mixer_func_t)() const;

        /// Maximum allowed volume, must be a power of 2.
        public const Int32 VOLUME_MAX = 1024;

        private List<sidemu> m_chips=new List<sidemu>(); //std::vector<sidemu*> m_chips;
        private List<Int16[]> m_buffers=new List<Int16[]>();//std::vector<short*> m_buffers;

        private List<Int32> m_iSamples=new List<int>();//std::vector<Int32> m_iSamples;
        private List<Int32> m_volume=new List<int>();//std::vector<Int32> m_volume;

        private mixer_func_t[] m_mix=new mixer_func_t[1];//std::vector<mixer_func_t> m_mix;

        System.Random r = new System.Random((int)DateTime.Now.Ticks);
        private Int32 oldRandomValue;
        private Int32 m_fastForwardFactor;

        // Mixer settings
        private Int16[] m_sampleBuffer;
        private UInt32 m_sampleCount;
        private UInt32 m_sampleIndex;

        private bool m_stereo;

        //private void updateParams() { }

        private int triangularDithering()
        {
            Int32 prevValue = oldRandomValue;
            oldRandomValue = r.Next(0,1024) & (VOLUME_MAX - 1);
            return oldRandomValue - prevValue;
        }

        /*
         * Channel matrix
         *
         *   C1
         * L 1.0
         * R 1.0
         *
         *   C1   C2
         * L 1.0  0.0
         * R 0.0  1.0
         *
         *   C1       C2           C3
         * L 1/1.707  0.707/1.707  0.0
         * R 0.0      0.707/1.707  1/1.707
         * 
         * FIXME
         * it seems that scaling down the summed signals is not the correct way of mixing, see:
         * http://dsp.stackexchange.com/questions/3581/algorithms-to-mix-audio-signals-without-clipping
         * maybe we should consider some form of soft/hard clipping instead to avoid possible overflows
         */

        // Mono mixing
        //template<int Chips>
        //Int32 mono() const
        private Int32 mono1()
        {
            //Int32 res = 0;
            //for (int i = 0; i < 1; i++)
            //    res += m_iSamples[i];
            //return res /= 1;
            return m_iSamples[0];
        }
        private Int32 mono2()
        {
            //Int32 res = 0;
            //for (int i = 0; i < 2; i++)
            //    res += m_iSamples[i];
            //return res /= 2;
            return (m_iSamples[0] + m_iSamples[1]) >> 1;
        }
        private Int32 mono3()
        {
            //Int32 res = 0;
            //for (int i = 0; i < 3; i++)
            //    res += m_iSamples[i];
            //return res /= 3;
            return (m_iSamples[0] + m_iSamples[1] + m_iSamples[2]) /3;
        }

        // Stereo mixing
        private Int32 stereo_OneChip() { return m_iSamples[0]; }

        private Int32 stereo_ch1_TwoChips() { return m_iSamples[0]; }
        private Int32 stereo_ch2_TwoChips() { return m_iSamples[1]; }

        private Int32 stereo_ch1_ThreeChips() { return (C1 * m_iSamples[0] + C2 * m_iSamples[1]) / SCALE_FACTOR; }
        private Int32 stereo_ch2_ThreeChips() { return (C2 * m_iSamples[1] + C1 * m_iSamples[2]) / SCALE_FACTOR; }


        /**
         * Create a new mixer.
         */
        public Mixer()
        {
            oldRandomValue = 0;
            m_fastForwardFactor = 1;
            m_sampleCount = 0;
            m_stereo = false;

            m_mix[0]=mono1;
        }

        /**
         * Do the mixing.
         */
        //public void doMix() { }

        /**
         * This clocks the SID chips to the present moment, if they aren't already.
         */
        //public void clockChips() { }

        /**
         * Reset sidemu buffer position discarding produced samples.
         */
        //public void resetBufs() { }

        /**
         * Prepare for mixing cycle.
         *
         * @param buffer output buffer
         * @param count size of the buffer in samples
         */
        //public void begin(Int16[] buffer, UInt32 count) { }

        /**
         * Remove all SIDs from the mixer.
         */
        //public void clearSids() { }

        /**
         * Add a SID to the mixer.
         *
         * @param chip the sid emu to add
         */
        //public void addSid(sidemu chip) { }

        /**
         * Get a SID from the mixer.
         *
         * @param i the number of the SID to get
         * @return a pointer to the requested sid emu or 0 if not found
         */
        public sidemu getSid(UInt32 i)
        {
            return (i < m_chips.Count) ? m_chips[(Int32)i] : null;
        }

        /**
         * Set the fast forward ratio.
         *
         * @param ff the fast forward ratio, from 1 to 32
         * @return true if parameter is valid, false otherwise
         */
        //public bool setFastForward(int ff) { return false; }

        /**
         * Set mixing volumes, from 0 to #VOLUME_MAX.
         *
         * @param left volume for left or mono channel
         * @param right volume for right channel in stereo mode
         */
        //public void setVolume(Int32 left, Int32 right) { }

        /**
         * Set mixing mode.
         *
         * @param stereo true for stereo mode, false for mono
         */
        //public void setStereo(bool stereo) { }

        /**
         * Check if the buffer have been filled.
         */
        public bool notFinished()
        {
            return m_sampleIndex < m_sampleCount;//!= m_sampleCount;
        }

        /**
         * Get the number of samples generated up to now.
         */
        public UInt32 samplesGenerated()
        {
            return m_sampleIndex;
        }




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

        //#include "mixer.h"
        //#include <cassert>
        //#include <algorithm>
        //#include "sidemu.h"


        public void clockChip(sidemu s)
        {
            s.clock();
        }

        public class bufferPos
        {
            public bufferPos(int i) { pos = i; }
            public void opeKakko(List<sidemu> s, Int32 ind)
            {
                s[ind].bufferpos(pos);
            }

            private Int32 pos;
        }

        public class bufferMove
        {
            public bufferMove(Int32 p, Int32 s)
            {
                pos = p;
                samples = s;
            }

            public void opeKakko(List<Int16[]> dest, Int32 i)
            {
                for (int j = 0; j < samples; j++)
                {
                    dest[i][j] = dest[i][j+pos];
                }
            }

            private Int32 pos;
            private Int32 samples;
        }

        public void clockChips()
        {
            //std::for_each(m_chips.begin(), m_chips.end(), clockChip);
            foreach (sidemu i in m_chips)
            {
                //clockChip(i);
                i.clock();
            }
        }

        public void resetBufs()
        {
            //std::for_each(m_chips.begin(), m_chips.end(), bufferPos(0));
            foreach (sidemu i in m_chips)
            {
                i.bufferpos(0);
            }
        }

        public void doMix()
        {
            //Ptr<Int16> buf = new Ptr<Int16>(m_sampleBuffer, (Int32)m_sampleIndex);

            // extract buffer info now that the SID is updated.
            // clock() may update bufferpos.
            // NB: if more than one chip exists, their bufferpos is identical to first chip's.
            Int32 sampleCount = m_chips[0].bufferpos();

            Int32 i = 0;
            Int32 channels = m_stereo ? 2 : 1;
            while (i < sampleCount)
            {
                // Handle whatever output the sid has generated so far
                if (m_sampleIndex >= m_sampleCount)
                {
                    break;
                }
                // Are there enough samples to generate the next one?
                if (i + m_fastForwardFactor >= sampleCount)
                {
                    break;
                }

                //MDPlayer.log.Write(".");

                // This is a crude boxcar low-pass filter to
                // reduce aliasing during fast forward.
                for (Int32 k = 0; k < m_buffers.Count; k++)
                {
                    Int32 sample = 0;
                    //Ptr<Int16> buffer = new Ptr<Int16>(m_buffers[k], i);
                    //for (int j = 0; j < m_fastForwardFactor; j++)
                    //{
                    //    sample += buffer[j];
                    //}
                    for (int j = 0; j < m_fastForwardFactor; j++)
                    {
                        sample += m_buffers[k][i + j];
                    }

                    m_iSamples[k] = sample / m_fastForwardFactor;
                }

                // increment i to mark we ate some samples, finish the boxcar thing.
                i += m_fastForwardFactor;

                Int32 dither = triangularDithering();
                //Int32 dither = 0;//ディザリングの付加なし。(付加するとノイズが乗るが、割り算後の値を平均した際に、原音により近い波形を保つことができる)

                //Int32 channels = m_stereo ? 2 : 1;
                for (Int32 ch = 0; ch < channels; ch++)
                {
                    Int32 tmp = (this.m_mix[ch]() * m_volume[ch] + dither) / VOLUME_MAX;
                    //assert(tmp >= -32768 && tmp <= 32767);
                    //buf.buf[buf.ptr] = (Int16)tmp;
                    //buf.AddPtr(1);
                    m_sampleBuffer[m_sampleIndex] = (Int16)tmp;
                    m_sampleIndex++;
                }
            }

            // move the unhandled data to start of buffer, if any.
            Int32 samplesLeft = sampleCount - i;
            //std::for_each(m_buffers.begin(), m_buffers.end(), bufferMove(i, samplesLeft));
            for (int ind = 0; ind < m_buffers.Count; ind++)
            {
                //bufferMove bm = new bufferMove(i, samplesLeft);
                //bm.opeKakko(m_buffers, ind);

                for (int j = 0; j < samplesLeft; j++)
                {
                    m_buffers[ind][j] = m_buffers[ind][j + i];
                }
            }
            //std::for_each(m_chips.begin(), m_chips.end(), bufferPos(samplesLeft));
            for (int ind = 0; ind < m_chips.Count; ind++)
            {
                //bufferPos bp = new bufferPos(samplesLeft);
                //bp.opeKakko(m_chips, ind);

                m_chips[ind].bufferpos(samplesLeft);
            }
        }

        public void begin(short[] buffer, UInt32 count)
        {
            m_sampleIndex = 0;
            m_sampleCount = count;
            m_sampleBuffer = buffer;
        }

        private void updateParams()
        {
            switch (m_buffers.Count)
            {
                case 1:
                    //m_mix[0] = m_stereo ? (mixer_func_t)stereo_OneChip : (mixer_func_t)mono1;
                    //if (m_stereo) m_mix[1] = stereo_OneChip;
                    if (m_stereo)
                    {
                        m_mix[0] = stereo_OneChip;
                        m_mix[1] = stereo_OneChip;
                    }
                    else
                    {
                        m_mix[0] = mono1;
                    }
                    break;
                case 2:
                    //m_mix[0] = m_stereo ? (mixer_func_t)stereo_ch1_TwoChips : (mixer_func_t)mono2;
                    //if (m_stereo) m_mix[1] = (mixer_func_t)stereo_ch2_TwoChips;
                    if (m_stereo)
                    {
                        m_mix[0] = stereo_ch1_TwoChips;
                        m_mix[1] = stereo_ch2_TwoChips;
                    }
                    else
                    {
                        m_mix[0] = mono2;
                    }
                    break;
                case 3:
                    //m_mix[0] = m_stereo ? (mixer_func_t)stereo_ch1_ThreeChips : (mixer_func_t)mono3;
                    //if (m_stereo) m_mix[1] = (mixer_func_t)stereo_ch2_ThreeChips;
                    if (m_stereo)
                    {
                        m_mix[0] = stereo_ch1_ThreeChips;
                        m_mix[1] = stereo_ch2_ThreeChips;
                    }
                    else
                    {
                        m_mix[0] = mono3;
                    }
                    break;
            }
        }

        public void clearSids()
        {
            m_chips.Clear();
            m_buffers.Clear();
        }

        public void addSid(sidemu chip)
        {
            if (chip != null)
            {
                m_chips.Add(chip);
                m_buffers.Add(chip.buffer());
                m_iSamples=new List<int>(m_buffers.Count);
                m_iSamples.Add(0);

                if (m_mix.Length > 0)
                    updateParams();
            }
        }

        public void setStereo(bool stereo)
        {
            if (m_stereo != stereo)
            {
                m_stereo = stereo;

                m_mix = new mixer_func_t[m_stereo ? 2 : 1];

                updateParams();
            }
        }

        public bool setFastForward(Int32 ff)
        {
            if (ff < 1 || ff > 32)
                return false;

            m_fastForwardFactor = ff;
            return true;
        }

        public void setVolume(Int32 left, Int32 right)
        {
            m_volume.Clear();
            m_volume.Add(left);
            m_volume.Add(right);
        }

    }

}