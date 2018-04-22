/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2017 Leandro Nini <drfiemost@users.sourceforge.net>
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

namespace Driver.libsidplayfp.sidplayfp
{
    /**
     * SidConfig
     *
     * An instance of this class is used to transport emulator settings
     * to and from the interface class.
     */
    public class SidConfig
    {




        //# include <stdint.h>
        //# include "sidplayfp/siddefs.h"

        /// Playback mode
        public enum playback_t
        {
            MONO = 1,
            STEREO
        }

        /// SID chip model
        public enum sid_model_t
        {
            MOS6581,
            MOS8580
        }


        /// C64 model
        public enum c64_model_t
        {
            PAL,
            NTSC,
            OLD_NTSC,
            DREAN
        }


        /// Sampling method
        public enum sampling_method_t
        {
            INTERPOLATE,
            RESAMPLE_INTERPOLATE
        }

        public const UInt32 DEFAULT_SAMPLING_FREQ = 44100;

        /**
         * Intended c64 model when unknown or forced.
         * - PAL
         * - NTSC
         * - OLD_NTSC
         * - DREAN
         */
        public c64_model_t defaultC64Model;

        /**
         * Force the model to #defaultC64Model ignoring tune's clock setting.
         */
        public bool forceC64Model;

        /**
         * Intended sid model when unknown or forced.
         * - MOS6581
         * - MOS8580
         */
        public sid_model_t defaultSidModel;

        /**
         * Force the sid model to #defaultSidModel.
         */
        public bool forceSidModel;

        /**
         * Playbak mode.
         * - MONO
         * - STEREO
         */
        public playback_t playback;

        /**
         * Sampling frequency.
         */
        public UInt32 frequency;

        /**
         * Extra SID chips addresses.
         */
        //@{
        public UInt16 secondSidAddress;
        public UInt16 thirdSidAddress;
        //@}

        /**
         * Pointer to selected emulation,
         * reSIDfp, reSID or hardSID.
         */
        public sidbuilder sidEmulation;

        /**
         * Left channel volume.
         */
        public UInt32 leftVolume;

        /**
         * Right channel volume.
         */
        public UInt32 rightVolume;

        /**
         * Sampling method.
         * - INTERPOLATE
         * - RESAMPLE_INTERPOLATE
         */
        public sampling_method_t samplingMethod;

        /**
         * Faster low-quality emulation,
         * available only for reSID.
         */
        public bool fastSampling;

        /**
         * Compare two config objects.
         *
         * @return true if different
         */
        //public bool compare(ref SidConfig config) { return false; }

        //public SidConfig() { }




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

        //# include "SidConfig.h"
        //# include "mixer.h"
        //# include "sidcxx11.h"

        public SidConfig()
        {
            defaultC64Model = c64_model_t.PAL;
            forceC64Model = false;
            defaultSidModel = sid_model_t.MOS6581;
            forceSidModel = false;
            playback = playback_t.MONO;
            frequency = DEFAULT_SAMPLING_FREQ;
            secondSidAddress = 0;
            thirdSidAddress = 0;
            sidEmulation = null;
            leftVolume = libsidplayfp.Mixer.VOLUME_MAX;
            rightVolume = libsidplayfp.Mixer.VOLUME_MAX;
            samplingMethod = sampling_method_t.RESAMPLE_INTERPOLATE;
            fastSampling = false;
        }

        public bool compare(ref SidConfig config)
        {
            return defaultC64Model != config.defaultC64Model
                || forceC64Model != config.forceC64Model
                || defaultSidModel != config.defaultSidModel
                || forceSidModel != config.forceSidModel
                || playback != config.playback
                || frequency != config.frequency
                || secondSidAddress != config.secondSidAddress
                || thirdSidAddress != config.thirdSidAddress
                || sidEmulation != config.sidEmulation
                || leftVolume != config.leftVolume
                || rightVolume != config.rightVolume
                || samplingMethod != config.samplingMethod
                || fastSampling != config.fastSampling;
        }

    }
}