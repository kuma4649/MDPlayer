/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
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
    /**
     * The C64 MMU chip.
     */
    public sealed class MMU : sidmemory, Banks.IPLA
    {



        //# include <stdint.h>
        //# include "sidendian.h"
        //# include "sidmemory.h"
        //# include "EventScheduler.h"
        //# include "Banks/SystemRAMBank.h"
        //# include "Banks/SystemROMBanks.h"
        //# include "Banks/ZeroRAMBank.h"
        //# include "sidcxx11.h"
        //# include <string.h>

        private EventScheduler eventScheduler = null;

        /// CPU port signals
        private bool loram, hiram, charen;

        /// CPU read memory mapping in 4k chunks
        private Banks.IBank[] cpuReadMap = new Banks.IBank[16];

        /// CPU write memory mapping in 4k chunks
        private Banks.IBank[] cpuWriteMap = new Banks.IBank[16];

        /// IO region handler
        private Banks.IOBank ioBank;

        /// Kernal ROM
        private Banks.KernalRomBank kernalRomBank=new Banks.KernalRomBank();

        /// BASIC ROM
        private Banks.BasicRomBank basicRomBank=new Banks.BasicRomBank();

        /// Character ROM
        private Banks.CharacterRomBank characterRomBank=new Banks.CharacterRomBank();

        /// RAM
        private Banks.SystemRAMBank ramBank=new Banks.SystemRAMBank();

        /// RAM bank 0
        private Banks.ZeroRAMBank zeroRAMBank;

        //public  void setCpuPort(byte state) { }
        public byte getLastReadByte() { return 0; }
        public Int64 getPhi2Time() { return eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI2); }

        //private void updateMappingPHI2() { }

        //public MMU(EventScheduler eventScheduler, Banks.IOBank ioBank) { }
        ~MMU() { }

        //public void reset() { }

        public void setRoms(Ptr<byte> kernal, Ptr<byte> basic, Ptr<byte> character)
        {
            kernalRomBank.set(kernal);
            basicRomBank.set(basic);
            characterRomBank.set(character);
        }

        // RAM access methods
        public override byte readMemByte(UInt16 addr) { return ramBank.peek(addr); }
        public override UInt16 readMemWord(UInt16 addr) { return sidendian.endian_little16(new Ptr<byte>(ramBank.ram, addr)); }

        public override void writeMemByte(UInt16 addr, byte value) { ramBank.poke(addr, value); }
        public override void writeMemWord(UInt16 addr, UInt16 value) { sidendian.endian_little16(new Ptr<byte>(ramBank.ram, addr), value); }

        public override void fillRam(UInt16 start, byte value, UInt32 size)
        {
            Ptr<byte> buf = new Ptr<byte>(ramBank.ram, start);
            mem.memset(ref buf, value, (Int32)size);
        }
        public override void fillRam(UInt16 start, Ptr<byte> value, UInt32 size)
        {
            Array.Copy(value.buf, value.ptr, ramBank.ram, start, size);
        }

        public override void fillRam(UInt16 start, byte[] source, UInt32 size)
        {
            Ptr<byte> buf = new Ptr<byte>(ramBank.ram, start);
            mem.memcpy(ref buf, source, (Int32)size);
        }

        // SID specific hacks
        public override void installResetHook(UInt16 addr) { kernalRomBank.installResetHook(addr); }

        public override void installBasicTrap(UInt16 addr) { basicRomBank.installTrap(addr); }

        public override void setBasicSubtune(byte tune) { basicRomBank.setSubtune(tune); }

        /**
         * Access memory as seen by CPU.
         *
         * @param addr the address where to read from
         * @return value at address
         */
        public byte cpuRead(UInt16 addr) { return cpuReadMap[addr >> 12].peek(addr); }

        /**
         * Access memory as seen by CPU.
         *
         * @param addr the address where to write
         * @param data the value to write
         */
        public void cpuWrite(UInt16 addr, byte data) { cpuWriteMap[addr >> 12].poke(addr, data); }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
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

        //#include "mmu.h"
        //#include "Banks/Bank.h"
        //#include "Banks/IOBank.h"

        public MMU(EventScheduler scheduler, Banks.IOBank ioBank) : base()
        {
            this.eventScheduler = scheduler;
            this.loram = false;
            this.hiram = false;
            this.charen = false;
            this.ioBank = ioBank;
            this.zeroRAMBank = new Banks.ZeroRAMBank(this, ramBank);
            cpuReadMap[0] = zeroRAMBank;
            cpuWriteMap[0] = zeroRAMBank;

            for (int i = 1; i < 16; i++)
            {
                cpuReadMap[i] = ramBank;
                cpuWriteMap[i] = ramBank;
            }
        }

        public void setCpuPort(byte state)
        {
            loram = (state & 1) != 0;
            hiram = (state & 2) != 0;
            charen = (state & 4) != 0;

            updateMappingPHI2();
        }

        private void updateMappingPHI2()
        {
            cpuReadMap[0xe] = cpuReadMap[0xf] = (Banks.IBank)(hiram ? (Banks.IBank)kernalRomBank : (Banks.IBank)ramBank);
            cpuReadMap[0xa] = cpuReadMap[0xb] = (Banks.IBank)((loram && hiram) ? (Banks.IBank)basicRomBank : (Banks.IBank)ramBank);

            if (charen && (loram || hiram))
            {
                cpuReadMap[0xd] = cpuWriteMap[0xd] = ioBank;
            }
            else
            {
                cpuReadMap[0xd] = (Banks.IBank)((!charen && (loram || hiram)) ? (Banks.IBank)characterRomBank : ramBank);
                cpuWriteMap[0xd] = ramBank;
            }
        }

        public void reset()
        {
            ramBank.reset();
            zeroRAMBank.reset();

            // Reset the ROMs to undo the hacks applied
            kernalRomBank.reset();
            basicRomBank.reset();

            loram = false;
            hiram = false;
            charen = false;

            updateMappingPHI2();
        }






    }
}