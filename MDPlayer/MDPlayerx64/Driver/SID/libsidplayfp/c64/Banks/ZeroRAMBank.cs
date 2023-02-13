/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2012-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2009-2014 VICE Project
 * Copyright 2010 Antti Lankila
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

namespace Driver.libsidplayfp.c64.Banks
{




    //# include <stdint.h>
    //# include "Bank.h"
    //# include "SystemRAMBank.h"
    //# include "Event.h"
    //# include "sidcxx11.h"

    /**
     * Interface to PLA functions.
     */
    public class PLA : IPLA
    {
        public void setCpuPort(byte state) { }
        public byte getLastReadByte() { return 0; }
        public Int64 getPhi2Time() { return 0; }

        ~PLA() { }
    }

    /**
 * Unused data port bits emulation, as investigated by groepaz:
 *
 *  - There are 2 different unused bits, 1) the output bits, 2) the input bits
 *  - The output bits can be (re)set when the data-direction is set to output
 *    for those bits and the output bits will not drop-off to 0.
 *  - When the data-direction for the unused bits is set to output then the
 *    unused input bits can be (re)set by writing to them, when set to 1 the
 *    drop-off timer will start which will cause the unused input bits to drop
 *    down to 0 in a certain amount of time.
 *  - When an unused input bit already had the drop-off timer running, and is
 *    set to 1 again, the drop-off timer will restart.
 *  - when a an unused bit changes from output to input, and the current output
 *    bit is 1, the drop-off timer will restart again
 */
    public class dataBit
    {
        private int Bit = 0;

        public dataBit(int Bit)
        {
            this.Bit = Bit;
        }

        /**
         * $01 bits 6 and 7 fall-off cycles (1->0), average is about 350 msec for a 6510
         * and about 1500 msec for a 8500.
         *
         *  NOTE: fall-off cycles are heavily chip- and temperature dependent. as a
         *        consequence it is very hard to find suitable realistic values that
         *        always work and we can only tweak them based on testcases. (unless we
         *        want to make it configurable or emulate temperature over time =))
         *
         *        it probably makes sense to tweak the values for a warmed up CPU, since
         *        this is likely how (old) programs were coded and tested :)
         *
         *  NOTE: the unused bits of the 6510 seem to be much more temperature dependant
         *        and the fall-off time decreases quicker and more drastically than on a
         *        8500
         *
         * cpuports.prg from the lorenz testsuite will fail when the falloff takes more
         * than 1373 cycles. this suggests that he tested on a well warmed up c64 :)
         * he explicitly delays by ~1280 cycles and mentions capacitance, so he probably
         * even was aware of what happens.
         */
        //@{
        private const Int64 C64_CPU6510_DATA_PORT_FALL_OFF_CYCLES = 350000;
        private const Int64 C64_CPU8500_DATA_PORT_FALL_OFF_CYCLES = 1500000; // Curently unused
                                                                             //@}

        /// Cycle that should invalidate the bit.
        private Int64 dataSetClk;

        /// Indicates if the bit is in the process of falling off.
        private bool isFallingOff;

        /// Value of the bit.
        private byte dataSet;

        public void reset()
        {
            isFallingOff = false;
            dataSet = 0;
        }

        public byte readBit(Int64 phi2time)
        {
            if (isFallingOff && dataSetClk < phi2time)
            {
                // discharge the "capacitor"
                reset();
            }
            return dataSet;
        }

        public void writeBit(Int64 phi2time, byte value)
        {
            dataSetClk = phi2time + C64_CPU6510_DATA_PORT_FALL_OFF_CYCLES;
            dataSet = (byte)(value & (1 << Bit));
            isFallingOff = true;
        }
    }


    /**
     * Area backed by RAM, including cpu port addresses 0 and 1.
     *
     * This is bit of a fake. We know that the CPU port is an internal
     * detail of the CPU, and therefore CPU should simply pay the price
     * for reading/writing to $00/$01.
     *
     * However, that would slow down all accesses, which is suboptimal. Therefore
     * we install this little hook to the 4k 0 region to deal with this.
     *
     * Implementation based on VICE code.
     */
    public sealed class ZeroRAMBank : IBank
    {





        // not emulated
        private const bool tape_sense = false;

        public IPLA pla;

        /// C64 RAM area
        private SystemRAMBank ramBank;

        /// Unused bits of the data port.
        //@{
        private dataBit dataBit6 = new dataBit(6);
        private dataBit dataBit7 = new dataBit(7);
        //@}

        /// Value written to processor port.
        //@{
        private byte dir;
        private byte data;
        //@}

        /// Value read from processor port.
        private byte dataRead;

        /// State of processor port pins.
        private byte procPortPins;

        private void updateCpuPort()
        {
            // Update data pins for which direction is OUTPUT
            procPortPins = (byte)((procPortPins & ~dir) | (data & dir));

            dataRead = (byte)((data | ~dir) & (procPortPins | 0x17));

            pla.setCpuPort((byte)((data | ~dir) & 0x07));

            if ((dir & 0x20) == 0)
            {
                dataRead &= (byte)(0xdf);// ~0x20;
            }
            if (tape_sense && (dir & 0x10) == 0)
            {
                dataRead &= (byte)(0xef);// ~0x10;
            }
        }

        public ZeroRAMBank(IPLA pla, SystemRAMBank ramBank)
        {
            this.pla = pla;
            this.ramBank = ramBank;
        }

        public void reset()
        {
            dataBit6.reset();
            dataBit7.reset();

            dir = 0;
            data = 0x3f;
            dataRead = 0x3f;
            procPortPins = 0x3f;

            updateCpuPort();
        }

        public  byte peek(UInt16 address)
        {
            switch (address)
            {
                case 0:
                    return dir;
                case 1:
                    {
                        byte retval = dataRead;

                        // for unused bits in input mode, the value comes from the "capacitor"

                        // set real value of bit 6
                        if ((dir & 0x40) == 0)
                        {
                            retval &= (byte)(0xbf);// ~0x40;
                            retval |= dataBit6.readBit(pla.getPhi2Time());
                        }

                        // set real value of bit 7
                        if ((dir & 0x80) == 0)
                        {
                            retval &= (byte)(0x7f);// ~0x80;
                            retval |= dataBit7.readBit(pla.getPhi2Time());
                        }

                        return retval;
                    }
                default:
                    return ramBank.peek(address);
            }
        }

        public  void poke(UInt16 address, byte value)
        {
            switch (address)
            {
                case 0:
                    // when switching an unused bit from output (where it contained a
                    // stable value) to input mode (where the input is floating), some
                    // of the charge is transferred to the floating input

                    if (dir != value)
                    {
                        // check if bit 6 has flipped from 1 to 0
                        if ((dir & 0x40) != 0 && (value & 0x40) == 0)
                            dataBit6.writeBit(pla.getPhi2Time(), data);

                        // check if bit 7 has flipped from 1 to 0
                        if ((dir & 0x80) != 0 && (value & 0x80) == 0)
                            dataBit7.writeBit(pla.getPhi2Time(), data);

                        dir = value;
                        updateCpuPort();
                    }

                    value = pla.getLastReadByte();
                    break;
                case 1:
                    // when writing to an unused bit that is output, charge the "capacitor",
                    // otherwise don't touch it

                    if ((dir & 0x40) != 0)
                        dataBit6.writeBit(pla.getPhi2Time(), value);

                    if ((dir & 0x80) != 0)
                        dataBit7.writeBit(pla.getPhi2Time(), value);

                    if (data != value)
                    {
                        data = value;
                        updateCpuPort();
                    }

                    value = pla.getLastReadByte();
                    break;
                default:
                    break;
            }

            ramBank.poke(address, value);
        }
    }

}
