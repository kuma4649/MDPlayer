/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2017 Leandro Nini <drfiemost@users.sourceforge.net>
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.sidplayfp
{

    /**
     * sidplayfp
     */
    public class sidplayfp
    {


        //# include <stdint.h>
        //# include <stdio.h>
        //# include "sidplayfp/siddefs.h"
        //# include "sidplayfp/sidversion.h"

        //class SidConfig;
        //class SidTune;
        //class SidInfo;
        //class EventContext;

        // Private Sidplayer
        //class Player;
        private libsidplayfp.Player sidplayer;

        //public sidplayfp() { }
        //~sidplayfp() { }

        /**
         * Get the current engine configuration.
         *
         * @return a const reference to the current configuration.
         */
        //public SidConfig config() { return null; }

        /**
         * Get the current player informations.
         *
         * @return a const reference to the current info.
         */
        //public SidInfo info() { return null; }

        /**
         * Configure the engine.
         * Check #error for detailed message if something goes wrong.
         *
         * @param cfg the new configuration
         * @return true on success, false otherwise.
         */
        //public bool config(ref SidConfig cfg) { return false; }

        /**
         * Error message.
         *
         * @return string error message.
         */
        //public string error() { return ""; }

        /**
         * Set the fast-forward factor.
         *
         * @param percent
         */
        //public bool fastForward(UInt32 percent) { return false; }

        /**
         * Load a tune.
         * Check #error for detailed message if something goes wrong.
         *
         * @param tune the SidTune to load, 0 unloads current tune.
         * @return true on sucess, false otherwise.
         */
        //public bool load(SidTune tune) { return false; }

        /**
         * Run the emulation and produce samples to play if a buffer is given.
         *
         * @param buffer pointer to the buffer to fill with samples.
         * @param count the size of the buffer measured in 16 bit samples
         *              or 0 if no output is needed (e.g. Hardsid)
         * @return the number of produced samples. If less than requested
         * and #isPlaying() is true an error occurred, use #error() to get
         * a detailed message.
         */
        //public UInt32 play(Int16[] buffer, UInt32 count) { return 0; }

        /**
         * Check if the engine is playing or stopped.
         *
         * @return true if playing, false otherwise.
         */
        //public bool isPlaying() { return false; }

        /**
         * Stop the engine.
         */
        //public void stop() { }

        /**
         * Control debugging.
         * Only has effect if library have been compiled
         * with the --enable-debug option.
         *
         * @param enable enable/disable debugging.
         * @param out the file where to redirect the debug info.
         */
        //public void debug(bool enable, FILE out_) { }

        /**
         * Mute/unmute a SID channel.
         *
         * @param sidNum the SID chip, 0 for the first one, 1 for the second.
         * @param voice the channel to mute/unmute.
         * @param enable true unmutes the channel, false mutes it.
         */
        //public void mute(UInt32 sidNum, UInt32 voice, bool enable) { }

        /**
         * Get the current playing time.
         *
         * @return the current playing time measured in seconds.
         */
        //public UInt32 time() { return 0; }

        /**
         * Set ROM images.
         *
         * @param kernal pointer to Kernal ROM.
         * @param basic pointer to Basic ROM, generally needed only for BASIC tunes.
         * @param character pointer to character generator ROM.
         */
        //public void setRoms(byte[] kernal, byte[] basic = null, byte[] character = null) { }

        /**
         * Get the CIA 1 Timer A programmed value.
         */
        //public UInt16 getCia1TimerA() { return 0; }




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


        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        // Redirection to private version of sidplayer (This method is called Cheshire Cat)
        // [ms: which is J. Carolan's name for a degenerate 'bridge']
        // This interface can be directly replaced with a libsidplay1 or C interface wrapper.
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------

        //# include "sidplayfp.h"
        //# include "player.h"

        public sidplayfp()
        {
            sidplayer = new libsidplayfp.Player();
        }

        ~sidplayfp()
        {
            sidplayer = null;
        }

        public bool config(ref SidConfig cfg)
        {
            return sidplayer.config(ref cfg, true);
        }

        public SidConfig config()
        {
            return sidplayer.config();
        }

        public void stop()
        {
            sidplayer.stop();
        }

        public UInt32 play(Int16[] buffer, UInt32 count)
        {
            return sidplayer.play(buffer, count);
        }

        public bool load(SidTune tune)
        {
            return sidplayer.load(tune);
        }

        public SidInfo info()
        {
            return sidplayer.info();
        }

        public UInt32 time()
        {
            return sidplayer.time();
        }

        public string error()
        {
            return sidplayer.error();
        }

        public bool fastForward(UInt32 percent)
        {
            return sidplayer.fastForward(percent);
        }

        public void mute(UInt32 sidNum, UInt32 voice, bool enable)
        {
            sidplayer.mute(sidNum, voice, enable);
        }

        public void debug(bool enable, System.IO.FileStream out_)
        {
            sidplayer.debug(enable, out_);
        }

        public bool isPlaying()
        {
            return sidplayer.isPlaying();
        }

        public void setRoms(byte[] kernal, byte[] basic = null, byte[] character = null)
        {
            sidplayer.setRoms(kernal, basic, character);
        }

        public UInt16 getCia1TimerA()
        {
            return sidplayer.getCia1TimerA();
        }

    }
}