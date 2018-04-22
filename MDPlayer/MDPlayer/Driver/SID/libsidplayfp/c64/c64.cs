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

namespace Driver.libsidplayfp.c64
{




    //# include <stdint.h>
    //# include <cstdio>
    //# include <map>
    //# include "Banks/IOBank.h"
    //# include "Banks/ColorRAMBank.h"
    //# include "Banks/DisconnectedBusBank.h"
    //# include "Banks/SidBank.h"
    //# include "Banks/ExtraSidBank.h"
    //# include "EventScheduler.h"
    //# include "c64/c64env.h"
    //# include "c64/c64cpu.h"
    //# include "c64/c64cia.h"
    //# include "c64/c64vic.h"
    //# include "c64/mmu.h"
    //# include "sidcxx11.h"

//#if PC64_TESTSUITE
        public class testEnv
        {
            ~testEnv() { }
            public virtual void load(string a) { }
        };
//#endif

    /**
     * Commodore 64 emulation core.
     *
     * It consists of the following chips:
     * - CPU 6510
     * - VIC-II 6567/6569/6572
     * - CIA 6526
     * - SID 6581/8580
     * - PLA 7700/82S100
     * - Color RAM 2114
     * - System RAM 4164-20/50464-150
     * - Character ROM 2332
     * - Basic ROM 2364
     * - Kernal ROM 2364
     */
    public sealed class c64 : c64env
    {
        public enum model_t
        {
            PAL_B = 0     ///< PAL C64
        , NTSC_M       ///< NTSC C64
        , OLD_NTSC_M   ///< Old NTSC C64
        , PAL_N        ///< C64 Drean
        }

        //private Dictionary<int, Banks.ExtraSidBank> sidBankMap_t;
        //typedef std::map<int, ExtraSidBank*> sidBankMap_t;

        /// System clock frequency
        private double cpuFrequency;

        /// Number of sources asserting IRQ
        private int irqCount;

        /// BA state
        private bool oldBAState;

        /// System event context
        private new EventScheduler eventScheduler;

        /// CPU
        private c64cpu cpu;

        /// CIA1
        private c64cia1 cia1;

        /// CIA2
        private c64cia2 cia2;

        /// VIC II
        private c64vic vic;

        /// Color RAM
        private Banks.ColorRAMBank colorRAMBank=new Banks.ColorRAMBank();

        /// SID
        private Banks.SidBank sidBank=new Banks.SidBank();

        /// Extra SIDs
        private List<Tuple<int, Banks.ExtraSidBank>> extraSidBanks=new List<Tuple<int, Banks.ExtraSidBank>>();

        /// I/O Area #1 and #2
        private Banks.DisconnectedBusBank disconnectedBusBank=new Banks.DisconnectedBusBank();

        /// I/O Area
        private Banks.IOBank ioBank=new Banks.IOBank();

        /// MMU chip
        private MMU mmu;

        //private double getCpuFreq(model_t model) { return 0; }

        /**
         * Access memory as seen by CPU.
         *
         * @param addr the address where to read from
         * @return value at address
         */
        public override byte cpuRead(UInt16 addr) { return mmu.cpuRead(addr); }

        /**
         * Access memory as seen by CPU.
         *
         * @param addr the address where to write to
         * @param data the value to write
         */
        public override void cpuWrite(UInt16 addr, byte data) { mmu.cpuWrite(addr, data); }

        /**
         * IRQ trigger signal.
         *
         * Calls permitted any time, but normally originated by chips at PHI1.
         *
         * @param state
         */
        //public override void interruptIRQ(bool state) { }

        /**
         * NMI trigger signal.
         *
         * Calls permitted any time, but normally originated by chips at PHI1.
         */
        public override void interruptNMI() { cpu.triggerNMI(); }

        /**
         * Reset signal.
         */
        public override void interruptRST() { cpu.triggerRST(); }

        /**
         * BA signal.
         *
         * Calls permitted during PHI1.
         *
         * @param state
         */
        //public override void setBA(bool state) { }

        //public override void lightpen(bool state) { }

//#if PC64_TESTSUITE
        public testEnv m_env;

        public void loadFile(string file)
        {
            m_env.load(file);
        }
//#endif

        //public void resetIoBank() { }

        //public c64() : base(null) { }
        ~c64() { }

//#if PC64_TESTSUITE
        public void setTestEnv(testEnv env)
        {
            m_env = env;
        }
//#endif

        /**
         * Get C64's event scheduler
         *
         * @return the scheduler
         */
        public EventScheduler getEventScheduler() { return eventScheduler; }

        public UInt32 getTime() { return (UInt32)(eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI1) / cpuFrequency); }

        /**
         * Clock the emulation.
         *
         * @throws haltInstruction
         */
        public void clock() { eventScheduler.clock(); }

        public void debug(bool enable, System.IO.FileStream out_) { cpu.debug(enable, out_); }

        //public void reset() { }
        public void resetCpu() { cpu.reset(); }

        /**
         * Set the c64 model.
         */
        //public void setModel(model_t model) { }

        public void setRoms(byte[] kernal, byte[] basic, byte[] character)
        {
            Ptr<byte> k=null, b=null, c=null;
            if (kernal != null) k = new Ptr<byte>(kernal, 0);
            if (basic != null) b = new Ptr<byte>(basic, 0);
            if (character != null) c = new Ptr<byte>(character, 0);

            mmu.setRoms(k,b,c);
        }

        /**
         * Get the CPU clock speed.
         *
         * @return the speed in Hertz
         */
        public double getMainCpuSpeed() { return cpuFrequency; }

        /**
         * Set the base SID.
         *
         * @param s the sid emu to set
         */
        //public void setBaseSid(c64sid s) { }

        /**
         * Add an extra SID.
         *
         * @param s the sid emu to set
         * @param sidAddress
         *            base address (e.g. 0xd420)
         *
         * @return false if address is unsupported
         */
        //public bool addExtraSid(c64sid s, int address) { return false; }

        /**
         * Remove all the SIDs.
         */
        //public void clearSids() { }

        /**
         * Get the components credits
         */
        //@{
        public string cpuCredits() { return cpu.credits(); }
        public string ciaCredits() { return cia1.credits(); }
        public string vicCredits() { return vic.credits(); }
        //@}

        public sidmemory getMemInterface() { return mmu; }

        public UInt16 getCia1TimerA() { return cia1.getTimerA(); }

        public override void interruptIRQ(bool state)
        {
            if (state)
            {
                if (irqCount == 0)
                    cpu.triggerIRQ();

                irqCount++;
            }
            else
            {
                irqCount--;
                if (irqCount == 0)
                    cpu.clearIRQ();
            }
        }

        public override void setBA(bool state)
        {
            // only react to changes in state
            if (state == oldBAState)
                return;

            oldBAState = state;

            // Signal changes in BA to interested parties
            cpu.setRDY(state);
        }

        public override void lightpen(bool state)
        {
            if (state)
                vic.triggerLightpen();
            else
                vic.clearLightpen();
        }





        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2014 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //#include "c64.h"
        //#include <algorithm>
        //#include "c64/VIC_II/mos656x.h"

        public class model_data_t
        {
            public double colorBurst;         ///< Colorburst frequency in Herz
            public double divider;            ///< Clock frequency divider
            public double powerFreq;          ///< Power line frequency in Herz
            public VIC_II.MOS656X.model_t vicModel; ///< Video chip model

            public model_data_t(double colorBurst, double divider, double powerFreq, VIC_II.MOS656X.model_t vicModel)
            {
                this.colorBurst = colorBurst;
                this.divider = divider;
                this.powerFreq = powerFreq;
                this.vicModel = vicModel;
            }
        }

        /*
         * Color burst frequencies:
         *
         * NTSC  - 3.579545455 MHz = 315/88 MHz
         * PAL-B - 4.43361875 MHz = 283.75 * 15625 Hz + 25 Hz.
         * PAL-M - 3.57561149 MHz
         * PAL-N - 3.58205625 MHz
         */

        public model_data_t[] modelData ;

        public double getCpuFreq(model_t model)
        {
            // The crystal clock that drives the VIC II chip is four times
            // the color burst frequency
            double crystalFreq = modelData[(int)model].colorBurst * 4.0;

            // The VIC II produces the two-phase system clock
            // by running the input clock through a divider
            return crystalFreq / modelData[(int)model].divider;
        }

        public c64() : base(new EventScheduler())
        {
            eventScheduler=base.eventScheduler;
            modelData = new model_data_t[]
            {
                new model_data_t(4433618.75,  18.0, 50.0, VIC_II.MOS656X.model_t.MOS6569),      // PAL-B
                new model_data_t(3579545.455, 14.0, 60.0, VIC_II.MOS656X.model_t.MOS6567R8),    // NTSC-M
                new model_data_t(3579545.455, 14.0, 60.0, VIC_II.MOS656X.model_t.MOS6567R56A),  // Old NTSC-M
                new model_data_t(3582056.25,  14.0, 50.0, VIC_II.MOS656X.model_t.MOS6572)       // PAL-N
            };



            cpuFrequency = getCpuFreq(model_t.PAL_B);
            cpu = new c64cpu(this);
            cia1 = new c64cia1(this);
            cia2 = new c64cia2(this);
            vic = new c64vic(this);
            mmu = new MMU(eventScheduler, ioBank);
            resetIoBank();
        }


        public void resetIoBank()
        {
            ioBank.setBank(0x0, vic);
            ioBank.setBank(0x1, vic);
            ioBank.setBank(0x2, vic);
            ioBank.setBank(0x3, vic);
            ioBank.setBank(0x4, sidBank);
            ioBank.setBank(0x5, sidBank);
            ioBank.setBank(0x6, sidBank);
            ioBank.setBank(0x7, sidBank);
            ioBank.setBank(0x8, colorRAMBank);
            ioBank.setBank(0x9, colorRAMBank);
            ioBank.setBank(0xa, colorRAMBank);
            ioBank.setBank(0xb, colorRAMBank);
            ioBank.setBank(0xc, cia1);
            ioBank.setBank(0xd, cia2);
            ioBank.setBank(0xe, disconnectedBusBank);
            ioBank.setBank(0xf, disconnectedBusBank);
        }

        //template<typename T>
        public void resetSID(Banks.ExtraSidBank e)
        {
            //e.second.reset();
            e.reset();
        }

        public void reset()
        {
            eventScheduler.reset();

            //cpu.reset();
            cia1.reset();
            cia2.reset();
            vic.reset();
            sidBank.reset();
            colorRAMBank.reset();
            mmu.reset();

            foreach (Tuple<int, Banks.ExtraSidBank> b in extraSidBanks) {
                resetSID(b.Item2);
            }

            irqCount = 0;
            oldBAState = true;
        }

        public void setModel(model_t model)
        {
            cpuFrequency = getCpuFreq(model);
            vic.chip(modelData[(int)model].vicModel);

            UInt32 rate = (UInt32)(cpuFrequency / modelData[(int)model].powerFreq);
            cia1.setDayOfTimeRate(rate);
            cia2.setDayOfTimeRate(rate);
        }

        public void setBaseSid(c64sid s)
        {
            sidBank.setSID(s);
        }

        public bool addExtraSid(c64sid s, int address)
        {
            // Check for valid address in the IO area range ($dxxx)
            if ((address & 0xf000) != 0xd000)
                return false;

            int idx = (address >> 8) & 0xf;

            // Only allow second SID chip in SID area ($d400-$d7ff)
            // or IO Area ($de00-$dfff)
            if (idx < 0x4 || (idx > 0x7 && idx < 0xe))
                return false;

            // Add new SID bank
            Tuple<int, Banks.ExtraSidBank> it = extraSidBanks[idx];
            if (idx != extraSidBanks.Count - 1)
            {
                Banks.ExtraSidBank extraSidBank = it.Item2;
                extraSidBank.addSID(s, address);
            }
            else
            {
                extraSidBanks.Add(new Tuple<int, Banks.ExtraSidBank>(idx, new Banks.ExtraSidBank()));
                Banks.ExtraSidBank extraSidBank = extraSidBanks[extraSidBanks.Count - 1].Item2;
                extraSidBank.resetSIDMapper(ioBank.getBank(idx));
                ioBank.setBank(idx, extraSidBank);
                extraSidBank.addSID(s, address);
            }

            return true;
        }

        //template<class T>
        //public void Delete(T s)
        //{
        //delete s.second;
        //}

        public void clearSids()
        {
            sidBank.setSID(null);

            resetIoBank();

            //std::for_each(extraSidBanks.begin(), extraSidBanks.end(), Delete<sidBankMap_t::value_type>);
            for (int i = 0; i < extraSidBanks.Count; i++) {
                {
                    extraSidBanks[i] = new Tuple<int, Banks.ExtraSidBank>(extraSidBanks[i].Item1, null);
                }

                extraSidBanks.Clear();
            }

        }





    }
}