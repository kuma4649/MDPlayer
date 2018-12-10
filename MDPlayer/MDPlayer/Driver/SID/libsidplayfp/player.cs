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
using Driver.libsidplayfp.c64;

namespace Driver.libsidplayfp
{





    //# include <stdint.h>
    //# include <cstdio>
    //# include "sidplayfp/siddefs.h"
    //# include "sidplayfp/SidConfig.h"
    //# include "sidplayfp/SidTuneInfo.h"
    //# include "SidInfoImpl.h"
    //# include "mixer.h"
    //# include "c64/c64.h"

    //# ifdef HAVE_CONFIG_H
    //# include "config.h"
    //#endif

    //# ifdef PC64_TESTSUITE
    //# include <string>
    //#endif

    //# include <vector>

    //class SidTune;
    //class SidInfo;
    //class sidbuilder;


    public class Player
//# ifdef PC64_TESTSUITE
: testEnv
    //#endif
    {
        private enum state_t
        {
            STOPPED,
            PLAYING,
            STOPPING
        }


        /// Commodore 64 emulator
        private c64.c64 m_c64=new c64.c64();

        /// Mixer
        private Mixer m_mixer=new Mixer();

        /// Emulator info
        private sidplayfp.SidTune m_tune;

        /// User Configuration Settings
        private SidInfoImpl m_info=new SidInfoImpl();

        /// User Configuration Settings
        private sidplayfp.SidConfig m_cfg=new sidplayfp.SidConfig();

        /// Error message
        private string m_errorString;

        private volatile state_t m_isPlaying;

        /// PAL/NTSC switch value
        private byte videoSwitch;


        /**
         * Get the C64 model for the current loaded tune.
         *
         * @param defaultModel the default model
         * @param forced true if the default model shold be forced in spite of tune model
         */
        //private c64.c64.model_t c64model(sidplayfp.SidConfig.c64_model_t defaultModel, bool forced) { return c64.c64.model_t.NTSC_M; }

        /**
         * Initialize the emulation.
         *
         * @throw configError
         */
        //private void initialise() { }

        /**
         * Release the SID builders.
         */
        //private void sidRelease() { }

        /**
         * Create the SID emulation(s).
         *
         * @throw configError
         */
        //private void sidCreate(sidplayfp.sidbuilder[] builder, sidplayfp.SidConfig.sid_model_t defaultModel,
        //bool forced, ref List<UInt32> extraSidAddresses)
        //{ }

        /**
         * Set the SID emulation parameters.
         *
         * @param cpuFreq the CPU clock frequency
         * @param frequency the output sampling frequency
         * @param sampling the sampling method to use
         * @param fastSampling true to enable fast low quality resampling (only for reSID)
         */
        //private void sidParams(double cpuFreq, int frequency,
        //sidplayfp.SidConfig.sampling_method_t sampling, bool fastSampling)
        //{ }

        //# ifdef PC64_TESTSUITE
        //private void load(string file) { }
        //#endif

        //private void run(UInt32 events) { }

        //public Player() { }
        ~Player() { }

        public sidplayfp.SidConfig config() { return m_cfg; }

        public sidplayfp.SidInfo info() { return m_info; }

        //public bool config(ref sidplayfp.SidConfig cfg, bool force = false) { return false; }

        //public bool fastForward(UInt32 percent) { return false; }

        //public bool load(sidplayfp.SidTune tune) { return false; }

        //public UInt32 play(Int16[] buffer, UInt32 samples) { return 0; }

        public bool isPlaying() { return m_isPlaying != state_t.STOPPED; }

        //public void stop() { }

        public UInt32 time() { return m_c64.getTime(); }

        public void debug(bool enable, System.IO.FileStream out_) { m_c64.debug(enable, out_); }

        //public void mute(UInt32 sidNum, UInt32 voice, bool enable) { }

        public string error() { return m_errorString; }

        //public void setRoms(byte[] kernal, byte[] basic, byte[] character) { }

        public UInt16 getCia1TimerA() { return m_c64.getCia1TimerA(); }




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


        //#include "player.h"
        //#include "sidplayfp/SidTune.h"
        //#include "sidplayfp/sidbuilder.h"
        //#include "sidemu.h"
        //#include "psiddrv.h"
        //#include "romCheck.h"
        //#include "sidcxx11.h"


        // Speed strings
        string TXT_PAL_VBI = "50 Hz VBI (PAL)";
        string TXT_PAL_VBI_FIXED = "60 Hz VBI (PAL FIXED)";
        string TXT_PAL_CIA = "CIA (PAL)";
        //string TXT_PAL_UNKNOWN = "UNKNOWN (PAL)";
        string TXT_NTSC_VBI = "60 Hz VBI (NTSC)";
        string TXT_NTSC_VBI_FIXED = "50 Hz VBI (NTSC FIXED)";
        string TXT_NTSC_CIA = "CIA (NTSC)";
        //string TXT_NTSC_UNKNOWN = "UNKNOWN (NTSC)";

        // Error Strings
        string ERR_NA = "NA";
        string ERR_UNSUPPORTED_FREQ = "SIDPLAYER ERROR: Unsupported sampling frequency.";
        string ERR_UNSUPPORTED_SID_ADDR = "SIDPLAYER ERROR: Unsupported SID address.";
        string ERR_UNSUPPORTED_SIZE = "SIDPLAYER ERROR: Size of music data exceeds C64 memory.";
        string ERR_INVALID_PERCENTAGE = "SIDPLAYER ERROR: Percentage value out of range.";

        /**
         * Configuration error exception.
         */
        public class configError : Exception
        {
            private string m_msg;

            public configError(string msg) { m_msg = msg; }
            public string message() { return m_msg; }
        }

        public Player()
        {
            // Set default settings for system
            m_tune = null;
            m_errorString = ERR_NA;
            m_isPlaying = state_t.STOPPED;
            //# ifdef PC64_TESTSUITE
            m_c64.setTestEnv(this);
            //#endif

            m_c64.setRoms(null, null, null);
            config(ref m_cfg);

            // Get component credits
            m_info.m_credits.Add(m_c64.cpuCredits());
            m_info.m_credits.Add(m_c64.ciaCredits());
            m_info.m_credits.Add(m_c64.vicCredits());
        }

        //template<class T>
        public void checkRomkernalCheck(byte[] rom, ref string desc)
        {

            if (rom != null)
            {
                kernalCheck romCheck = new kernalCheck(rom);
                desc = romCheck.info();
            }
            else
                desc = "";
        }
        public void checkRombasicCheck(byte[] rom, ref string desc)
        {
            if (rom != null)
            {
                basicCheck romCheck = new basicCheck(rom);
                desc = romCheck.info();
            }
            else
                desc = "";
        }
        public void checkRomchargenCheck(byte[] rom, ref string desc)
        {
            if (rom != null)
            {
                chargenCheck romCheck = new chargenCheck(rom);
                desc = romCheck.info();
            }
            else
                desc = "";
        }

        public void setRoms(byte[] kernal, byte[] basic, byte[] character)
        {
            checkRomkernalCheck(kernal, ref m_info.m_kernalDesc);
            checkRombasicCheck(basic, ref m_info.m_basicDesc);
            checkRomchargenCheck(character, ref m_info.m_chargenDesc);

            m_c64.setRoms(kernal, basic, character);
        }

        public bool fastForward(UInt32 percent)
        {
            if (!m_mixer.setFastForward((Int32)(percent / 100)))
            {
                m_errorString = ERR_INVALID_PERCENTAGE;
                return false;
            }

            return true;
        }

        private void initialise()
        {
            m_isPlaying = state_t.STOPPED;

            m_c64.reset();

            sidplayfp.SidTuneInfo tuneInfo = m_tune.getInfo();

            UInt32 size = (UInt32)(tuneInfo.loadAddr()) + tuneInfo.c64dataLen() - 1;
            if (size > 0xffff)
            {
                throw new configError(ERR_UNSUPPORTED_SIZE);
            }

            psiddrv driver = new psiddrv(m_tune.getInfo());
            if (!driver.drvReloc())
            {
                throw new configError(driver.errorString());
            }

            m_info.m_driverAddr = driver.driverAddr();
            m_info.m_driverLength = driver.driverLength();

            sidmemory sm = m_c64.getMemInterface();
            driver.install(ref sm, videoSwitch);

            sm = m_c64.getMemInterface();
            if (!m_tune.placeSidTuneInC64mem(ref sm))
            {
                throw new configError(m_tune.statusString());
            }

            m_c64.resetCpu();
            //Console.WriteLine("{0:x}", sm.readMemByte(0x17e3));
        }

        public bool load(sidplayfp.SidTune tune)
        {
            m_tune = tune;

            if (tune != null)
            {
                // Must re-configure on fly for stereo support!
                if (!config(ref m_cfg, true))
                {
                    // Failed configuration with new tune, reject it
                    m_tune = null;
                    return false;
                }
            }
            return true;
        }

        public void mute(UInt32 sidNum, UInt32 voice, bool enable)
        {
            sidemu s = m_mixer.getSid(sidNum);
            if (s != null)
                s.voice(voice, enable);
        }

        /**
         * @throws MOS6510::haltInstruction
         */
        private void run(UInt32 events)
        {
            for (UInt32 i = 0; m_isPlaying != state_t.STOPPED && i < events; i++)
            {
                //Console.WriteLine("run counter i : {0}",i);
                m_c64.clock();
            }
        }

        public UInt32 play(Int16[] buffer, UInt32 count)
        {
            // Make sure a tune is loaded
            if (m_tune == null)
                return 0;

            // Start the player loop
            if (m_isPlaying == state_t.STOPPED)
                m_isPlaying = state_t.PLAYING;

            if (m_isPlaying == state_t.PLAYING)
            {
                m_mixer.begin(buffer, count);
                //MDPlayer.log.Write(string.Format("{0}", count));
                try
                {
                    if (m_mixer.getSid(0) != null)
                    {
                        if (count != 0 && buffer != null)
                        {
                            // Clock chips and mix into output buffer
                            while (m_isPlaying != state_t.STOPPED && m_mixer.notFinished())
                            {
                                run((UInt32)sidemu.output.OUTPUTBUFFERSIZE);

                                m_mixer.clockChips();
                                m_mixer.doMix();
                            }
                            count = m_mixer.samplesGenerated();
                        }
                        else
                        {
                            // Clock chips and discard buffers
                            int size = (Int32)(m_c64.getMainCpuSpeed() / m_cfg.frequency);
                            while (m_isPlaying != state_t.STOPPED && (--size) != 0)
                            {
                                run((UInt32)sidemu.output.OUTPUTBUFFERSIZE);

                                m_mixer.clockChips();
                                m_mixer.resetBufs();
                            }
                        }
                    }
                    else
                    {
                        // Clock the machine
                        int size = (Int32)(m_c64.getMainCpuSpeed() / m_cfg.frequency);
                        while (m_isPlaying != state_t.STOPPED && (--size) != 0)
                        {
                            run((UInt32)sidemu.output.OUTPUTBUFFERSIZE);
                        }
                    }
                }
                catch //(MOS6510.haltInstruction const &)
                {
                    m_errorString = "Illegal instruction executed";
                    m_isPlaying = state_t.STOPPING;
                }
            }

            if (m_isPlaying == state_t.STOPPING)
            {
                try
                {
                    initialise();
                }
                catch
                { //(configError const &) { }
                    m_isPlaying = state_t.STOPPED;
                }
            }

            return count;
        }

        public void stop()
        {
            if (m_tune != null && m_isPlaying == state_t.PLAYING)
            {
                m_isPlaying = state_t.STOPPING;
            }
        }

        public bool config(ref sidplayfp.SidConfig cfg, bool force = false)
        {
            // Check if configuration have been changed or forced
            if (!force && !m_cfg.compare(ref cfg))
            {
                return true;
            }

            // Check for base sampling frequency
            if (cfg.frequency < 8000)
            {
                m_errorString = ERR_UNSUPPORTED_FREQ;
                return false;
            }

            // Only do these if we have a loaded tune
            if (m_tune != null)
            {
                sidplayfp.SidTuneInfo tuneInfo = m_tune.getInfo();

                try
                {
                    sidRelease();

                    List<UInt32> addresses = new List<uint>();
                    UInt16 secondSidAddress = tuneInfo.sidChipBase(1) != 0 ?
                        tuneInfo.sidChipBase(1) :
                        cfg.secondSidAddress;
                    if (secondSidAddress != 0)
                        addresses.Add(secondSidAddress);

                    UInt16 thirdSidAddress = tuneInfo.sidChipBase(2) != 0 ?
                            tuneInfo.sidChipBase(2) :
                            cfg.thirdSidAddress;
                    if (thirdSidAddress != 0)
                        addresses.Add(thirdSidAddress);

                    // SID emulation setup (must be performed before the
                    // environment setup call)
                    sidCreate(cfg.sidEmulation, cfg.defaultSidModel, cfg.forceSidModel, ref addresses);

                    // Determine clock speed
                    c64.c64.model_t model = c64model(cfg.defaultC64Model, cfg.forceC64Model);

                    m_c64.setModel(model);

                    sidParams(m_c64.getMainCpuSpeed(), (Int32)cfg.frequency, cfg.samplingMethod, cfg.fastSampling);

                    // Configure, setup and install C64 environment/events
                    initialise();
                }
                catch (configError e)
                {
                    m_errorString = e.message();
                    m_cfg.sidEmulation = null;
                    if (m_cfg != cfg)
                    {
                        config(ref m_cfg);
                    }
                    return false;
                }
            }

            bool isStereo = cfg.playback == sidplayfp.SidConfig.playback_t.STEREO;
            m_info.m_channels = (UInt32)(isStereo ? 2 : 1);

            m_mixer.setStereo(isStereo);
            m_mixer.setVolume((Int32)cfg.leftVolume, (Int32)cfg.rightVolume);

            // Update Configuration
            m_cfg = cfg;

            return true;
        }

        // Clock speed changes due to loading a new song
        private c64.c64.model_t c64model(sidplayfp.SidConfig.c64_model_t defaultModel, bool forced)
        {
            sidplayfp.SidTuneInfo tuneInfo = m_tune.getInfo();

            sidplayfp.SidTuneInfo.clock_t clockSpeed = tuneInfo.clockSpeed();

            c64.c64.model_t model = c64.c64.model_t.PAL_B;

            // Use preferred speed if forced or if song speed is unknown
            if (forced || clockSpeed == sidplayfp.SidTuneInfo.clock_t.CLOCK_UNKNOWN || clockSpeed == sidplayfp.SidTuneInfo.clock_t.CLOCK_ANY)
            {
                switch (defaultModel)
                {
                    case sidplayfp.SidConfig.c64_model_t.PAL:
                        clockSpeed = sidplayfp.SidTuneInfo.clock_t.CLOCK_PAL;
                        model = c64.c64.model_t.PAL_B;
                        videoSwitch = 1;
                        break;
                    case sidplayfp.SidConfig.c64_model_t.DREAN:
                        clockSpeed = sidplayfp.SidTuneInfo.clock_t.CLOCK_PAL;
                        model = c64.c64.model_t.PAL_N;
                        videoSwitch = 1; // TODO verify
                        break;
                    case sidplayfp.SidConfig.c64_model_t.NTSC:
                        clockSpeed = sidplayfp.SidTuneInfo.clock_t.CLOCK_NTSC;
                        model = c64.c64.model_t.NTSC_M;
                        videoSwitch = 0;
                        break;
                    case sidplayfp.SidConfig.c64_model_t.OLD_NTSC:
                        clockSpeed = sidplayfp.SidTuneInfo.clock_t.CLOCK_NTSC;
                        model = c64.c64.model_t.OLD_NTSC_M;
                        videoSwitch = 0;
                        break;
                }
            }
            else
            {
                switch (clockSpeed)
                {
                    default:
                    case sidplayfp.SidTuneInfo.clock_t.CLOCK_PAL:
                        model = c64.c64.model_t.PAL_B;
                        videoSwitch = 1;
                        break;
                    case sidplayfp.SidTuneInfo.clock_t.CLOCK_NTSC:
                        model = c64.c64.model_t.NTSC_M;
                        videoSwitch = 0;
                        break;
                }
            }

            switch (clockSpeed)
            {
                case sidplayfp.SidTuneInfo.clock_t.CLOCK_PAL:
                    if (tuneInfo.songSpeed() == sidplayfp.SidTuneInfo.SPEED_CIA_1A)
                        m_info.m_speedString = TXT_PAL_CIA;
                    else if (tuneInfo.clockSpeed() == sidplayfp.SidTuneInfo.clock_t.CLOCK_NTSC)
                        m_info.m_speedString = TXT_PAL_VBI_FIXED;
                    else
                        m_info.m_speedString = TXT_PAL_VBI;
                    break;
                case sidplayfp.SidTuneInfo.clock_t.CLOCK_NTSC:
                    if (tuneInfo.songSpeed() == sidplayfp.SidTuneInfo.SPEED_CIA_1A)
                        m_info.m_speedString = TXT_NTSC_CIA;
                    else if (tuneInfo.clockSpeed() == sidplayfp.SidTuneInfo.clock_t.CLOCK_PAL)
                        m_info.m_speedString = TXT_NTSC_VBI_FIXED;
                    else
                        m_info.m_speedString = TXT_NTSC_VBI;
                    break;
                default:
                    break;
            }

            return model;
        }

        /**
         * Get the SID model.
         *
         * @param sidModel the tune requested model
         * @param defaultModel the default model
         * @param forced true if the default model shold be forced in spite of tune model
         */
        public sidplayfp.SidConfig.sid_model_t getSidModel(sidplayfp.SidTuneInfo.model_t sidModel, sidplayfp.SidConfig.sid_model_t defaultModel, bool forced)
        {
            sidplayfp.SidTuneInfo.model_t tuneModel = sidModel;

            // Use preferred speed if forced or if song speed is unknown
            if (forced || tuneModel == sidplayfp.SidTuneInfo.model_t.SIDMODEL_UNKNOWN || tuneModel == sidplayfp.SidTuneInfo.model_t.SIDMODEL_ANY)
            {
                switch (defaultModel)
                {
                    case sidplayfp.SidConfig.sid_model_t.MOS6581:
                        tuneModel = sidplayfp.SidTuneInfo.model_t.SIDMODEL_6581;
                        break;
                    case sidplayfp.SidConfig.sid_model_t.MOS8580:
                        tuneModel = sidplayfp.SidTuneInfo.model_t.SIDMODEL_8580;
                        break;
                    default:
                        break;
                }
            }

            sidplayfp.SidConfig.sid_model_t newModel;

            switch (tuneModel)
            {
                default:
                case sidplayfp.SidTuneInfo.model_t.SIDMODEL_6581:
                    newModel = sidplayfp.SidConfig.sid_model_t.MOS6581;
                    break;
                case sidplayfp.SidTuneInfo.model_t.SIDMODEL_8580:
                    newModel = sidplayfp.SidConfig.sid_model_t.MOS8580;
                    break;
            }

            return newModel;
        }

        private void sidRelease()
        {
            m_c64.clearSids();

            for (UInt32 i = 0; ; i++)
            {
                sidemu s = m_mixer.getSid(i);
                if (s == null)
                    break;
                sidplayfp.sidbuilder b = s.builder();
                if (b != null)
                {
                    b.unlock(s);
                }
            }

            m_mixer.clearSids();
        }

        private void sidCreate(sidplayfp.sidbuilder builder, sidplayfp.SidConfig.sid_model_t defaultModel,
                        bool forced, ref List<UInt32> extraSidAddresses)
        {
            if (builder != null)
            {
                sidplayfp.SidTuneInfo tuneInfo = m_tune.getInfo();

                // Setup base SID
                sidplayfp.SidConfig.sid_model_t userModel = getSidModel(tuneInfo.sidModel(0), defaultModel, forced);
                sidemu s = builder.lock_(m_c64.getEventScheduler(), userModel);
                if (!builder.getStatus())
                {
                    throw new configError(builder.error());
                }

                m_c64.setBaseSid(s);
                m_mixer.addSid(s);

                // Setup extra SIDs if needed
                if (extraSidAddresses.Count != 0)
                {
                    // If bits 6-7 are set to Unknown then the second SID will be set to the same SID
                    // model as the first SID.
                    defaultModel = userModel;

                    UInt32 extraSidChips = (UInt32)extraSidAddresses.Count;

                    for (UInt32 i = 0; i < extraSidChips; i++)
                    {
                        sidplayfp.SidConfig.sid_model_t userModel_1 = getSidModel(tuneInfo.sidModel(i + 1), defaultModel, forced);

                        sidemu s1 = builder.lock_(m_c64.getEventScheduler(), userModel_1);
                        if (!builder.getStatus())
                        {
                            throw new configError(builder.error());
                        }

                        if (!m_c64.addExtraSid(s1, (Int32)extraSidAddresses[(Int32)i]))
                            throw new configError(ERR_UNSUPPORTED_SID_ADDR);

                        m_mixer.addSid(s1);
                    }
                }
            }
        }

        private void sidParams(double cpuFreq, int frequency,
                        sidplayfp.SidConfig.sampling_method_t sampling, bool fastSampling)
        {
            for (UInt32 i = 0; ; i++)
            {
                sidemu s = m_mixer.getSid(i);
                if (s == null)
                    break;

                s.sampling((float)cpuFreq, frequency, sampling, fastSampling);
            }
        }

        //# ifdef PC64_TESTSUITE
        public override void load(string file)
        {
            string name = "$enable_testsuite";// PC64_TESTSUITE;
            name += file;
            name += ".prg";

            m_tune.load(name);
            m_tune.selectSong(0);
            initialise();
        }
        //#endif

    }

}