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

namespace Driver.libsidplayfp.c64.CPU
{
    /**
     * Cycle-exact 6502/6510 emulation core.
     *
     * Code is based on work by Simon A. White <sidplay2@yahoo.com>.
     * Original Java port by Ken Händel. Later on, it has been hacked to
     * improve compatibility with Lorenz suite on VICE's test suite.
     *
     * @author alankila
     */
    public partial class MOS6510
    {



        //# include <stdint.h>
        //# include <cstdio>
        //# include "flags.h"
        //# include "EventCallback.h"
        //# include "EventScheduler.h"
        //# ifdef HAVE_CONFIG_H
        //# include "config.h"
        //#endif
#if DEBUG
        //public void DumpState(Int64 time, MOS6510 cpu) { }
#endif

        /*
         * Define this to get correct emulation of SHA/SHX/SHY/SHS instructions
         * (see VICE CPU tests).
         * This will slow down the emulation a bit with no real benefit
         * for SID playing so we keep it disabled.
         */
        //#define CORRECT_SH_INSTRUCTIONS


#if DEBUG
        //friend void MOS6510Debug::DumpState(event_clock_t time, MOS6510 &cpu);
#endif

        public class haltInstruction : Exception { }

        /**
         * IRQ/NMI magic limit values.
         * Need to be larger than about 0x103 << 3,
         * but can't be min/max for Integer type.
         */
        private const int MAX = 65536;

        /// Stack page location
        private const byte SP_PAGE = 0x01;

        /// Status register interrupt bit.
        public const int SR_INTERRUPT = 2;

        private class ProcessorCycle
        {
            public delegate void dlgFunc();
            public dlgFunc func;
            public bool nosteal;
            public ProcessorCycle()
            {
                func = null;
                nosteal = false;
            }
        }


        /// Event scheduler
        private EventScheduler eventScheduler;

        /// Current instruction and subcycle within instruction
        private int cycleCount;

        /// When IRQ was triggered. -MAX means "during some previous instruction", MAX means "no IRQ"
        private int interruptCycle;

        /// IRQ asserted on CPU
        private bool irqAssertedOnPin;

        /// NMI requested?
        private bool nmiFlag;

        /// RST requested?
        private bool rstFlag;

        /// RDY pin state (stop CPU on read)
        private bool rdy;

        /// Address Low summer carry
        private bool adl_carry;

#if CORRECT_SH_INSTRUCTIONS
        /// The RDY pin state during last throw away read.
        private bool rdyOnThrowAwayRead;
#endif

        /// Status register
        private Flags flags=new Flags();

        // Data regarding current instruction
        private UInt16 Register_ProgramCounter;
        private UInt16 Cycle_EffectiveAddress;
        private UInt16 Cycle_Pointer;

        private byte Cycle_Data;
        private byte Register_StackPointer;
        private byte Register_Accumulator;
        private byte Register_X;
        private byte Register_Y;

#if DEBUG
        // Debug info
        private UInt16 instrStartPC;
        private UInt16 instrOperand;

        //private System.IO.FileStream m_fdbg;

        private bool dodump;
#endif

        /// Table of CPU opcode implementations
        private ProcessorCycle[] instrTable = new ProcessorCycle[0x101 << 3];


        /// Represents an instruction subcycle that writes
        private EventCallback<MOS6510> m_nosteal;

        /// Represents an instruction subcycle that reads
        private EventCallback<MOS6510> m_steal;

        //private void eventWithoutSteals() { }
        //private void eventWithSteals() { }

        //private void Initialise() { }

        // Declare Interrupt Routines
        //private void IRQLoRequest() { }
        //private void IRQHiRequest() { }
        //private void interruptsAndNextOpcode() { }
        //private void calculateInterruptTriggerCycle() { }

        // Declare Instruction Routines
        //private void fetchNextOpcode() { }
        //private void throwAwayFetch() { }
        //private void throwAwayRead() { }
        //private void FetchDataByte() { }
        //private void FetchLowAddr() { }
        //private void FetchLowAddrX() { }
        //private void FetchLowAddrY() { }
        //private void FetchHighAddr() { }
        //private void FetchHighAddrX() { }
        //private void FetchHighAddrX2() { }
        //private void FetchHighAddrY() { }
        //private void FetchHighAddrY2() { }
        //private void FetchLowEffAddr() { }
        //private void FetchHighEffAddr() { }
        //private void FetchHighEffAddrY() { }
        //private void FetchHighEffAddrY2() { }
        //private void FetchLowPointer() { }
        //private void FetchLowPointerX() { }
        //private void FetchHighPointer() { }
        //private void FetchEffAddrDataByte() { }
        //private void PutEffAddrDataByte() { }
        //private void PushLowPC() { }
        //private void PushHighPC() { }
        //private void PushSR() { }
        //private void PopLowPC() { }
        //private void PopHighPC() { }
        //private void PopSR() { }
        //private void brkPushLowPC() { }
        //private void WasteCycle() { }

        // Delcare Instruction Operation Routines
        //private void adc_instr() { }
        //private void alr_instr() { }
        //private void anc_instr() { }
        //private void and_instr() { }
        //private void ane_instr() { }
        //private void arr_instr() { }
        //private void asl_instr() { }
        //private void asla_instr() { }
        //private void aso_instr() { }
        //private void axa_instr() { }
        //private void axs_instr() { }
        //private void bcc_instr() { }
        //private void bcs_instr() { }
        //private void beq_instr() { }
        //private void bit_instr() { }
        //private void bmi_instr() { }
        //private void bne_instr() { }
        //private void branch_instr(bool condition) { }
        //private void fix_branch() { }
        //private void bpl_instr() { }
        //private void brk_instr() { }
        //private void bvc_instr() { }
        //private void bvs_instr() { }
        //private void clc_instr() { }
        //private void cld_instr() { }
        //private void cli_instr() { }
        //private void clv_instr() { }
        //private void cmp_instr() { }
        //private void cpx_instr() { }
        //private void cpy_instr() { }
        //private void dcm_instr() { }
        //private void dec_instr() { }
        //private void dex_instr() { }
        //private void dey_instr() { }
        //private void eor_instr() { }
        //private void inc_instr() { }
        //private void ins_instr() { }
        //private void inx_instr() { }
        //private void iny_instr() { }
        //private void jmp_instr() { }
        //private void las_instr() { }
        //private void lax_instr() { }
        //private void lda_instr() { }
        //private void ldx_instr() { }
        //private void ldy_instr() { }
        //private void lse_instr() { }
        //private void lsr_instr() { }
        //private void lsra_instr() { }
        //private void oal_instr() { }
        //private void ora_instr() { }
        //private void pha_instr() { }
        //private void pla_instr() { }
        //private void plp_instr() { }
        //private void rla_instr() { }
        //private void rol_instr() { }
        //private void rola_instr() { }
        //private void ror_instr() { }
        //private void rora_instr() { }
        //private void rra_instr() { }
        //private void rti_instr() { }
        //private void rts_instr() { }
        //private void sbx_instr() { }
        //private void say_instr() { }
        //private void sbc_instr() { }
        //private void sec_instr() { }
        //private void sed_instr() { }
        //private void sei_instr() { }
        //private void shs_instr() { }
        //private void sta_instr() { }
        //private void stx_instr() { }
        //private void sty_instr() { }
        //private void tax_instr() { }
        //private void tay_instr() { }
        //private void tsx_instr() { }
        //private void txa_instr() { }
        //private void txs_instr() { }
        //private void tya_instr() { }
        //private void xas_instr() { }
        //private void sh_instr(byte offset) { }

        /**
         * @throws haltInstruction
         */
        //private void invalidOpcode() { }

        // Declare Arithmetic Operations
        //private void doADC() { }
        //private void doSBC() { }

        //private void doJSR() { }

        //private void buildInstructionTable() { }

        //protected MOS6510(EventScheduler scheduler) { }
        ~MOS6510() { }

        /**
         * Get data from system environment.
         *
         * @param address
         * @return data byte CPU requested
         */
        protected virtual byte cpuRead(UInt16 addr) { return 0; }

        /**
         * Write data to system environment.
         *
         * @param address
         * @param data
         */
        protected virtual void cpuWrite(UInt16 addr, byte data) { }

#if PC64_TESTSUITE
    public virtual void loadFile(string file) {};
#endif

        //public void reset() { }

        //public string credits() { return ""; }

        //public void debug(bool enable, System.IO.FileStream out_) { }
        //public void setRDY(bool newRDY) { }

        // Non-standard functions
        //public void triggerRST() { }
        //public void triggerNMI() { }
        //public void triggerIRQ() { }
        //public void clearIRQ() { }








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

        //#include "mos6510.h"
        //#include "Event.h"
        //#include "sidendian.h"
        //#include "opcodes.h"
        //#if DEBUG
        //#  include <cstdio>
        //#  include "mos6510debug.h"
        //#endif
        //#ifdef PC64_TESTSUITE
        //#  include <cstdlib>
        //#endif

#if PC64_TESTSUITE

        /**
         * CHR$ conversion table (0x01 = no output)
         */
        public byte[] CHRtab = new byte[256]
        {
  0x00,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x0d,0x01,0x01,
  0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
  0x20,0x21,0x01,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2a,0x2b,0x2c,0x2d,0x2e,0x2f,
  0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3a,0x3b,0x3c,0x3d,0x3e,0x3f,
  0x40,0x61,0x62,0x63,0x64,0x65,0x66,0x67,0x68,0x69,0x6a,0x6b,0x6c,0x6d,0x6e,0x6f,
  0x70,0x71,0x72,0x73,0x74,0x75,0x76,0x77,0x78,0x79,0x7a,0x5b,0x24,0x5d,0x20,0x20,
  // alternative: CHR$(92=0x5c) => ISO Latin-1(0xa3)
  0x2d,0x23,0x7c,0x2d,0x2d,0x2d,0x2d,0x7c,0x7c,0x5c,0x5c,0x2f,0x5c,0x5c,0x2f,0x2f,
  0x5c,0x23,0x5f,0x23,0x7c,0x2f,0x58,0x4f,0x23,0x7c,0x23,0x2b,0x7c,0x7c,0x26,0x5c,
  // 0x80-0xFF
  0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
  0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
  0x20,0x7c,0x23,0x2d,0x2d,0x7c,0x23,0x7c,0x23,0x2f,0x7c,0x7c,0x2f,0x5c,0x5c,0x2d,
  0x2f,0x2d,0x2d,0x7c,0x7c,0x7c,0x7c,0x2d,0x2d,0x2d,0x2f,0x5c,0x5c,0x2f,0x2f,0x23,
  0x20,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4a,0x4b,0x4c,0x4d,0x4e,0x4f,
  0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5a,0x2b,0x7c,0x7c,0x26,0x5c,
  0x20,0x7c,0x23,0x2d,0x2d,0x7c,0x23,0x7c,0x23,0x2f,0x7c,0x7c,0x2f,0x5c,0x5c,0x2d,
  0x2f,0x2d,0x2d,0x7c,0x7c,0x7c,0x7c,0x2d,0x2d,0x2d,0x2f,0x5c,0x5c,0x2f,0x2f,0x23
        };
        public byte[] filetmp=new byte[0x100];
        public int filepos = 0;
#endif // PC64_TESTSUITE


        /**
         * Magic value for lxa and ane undocumented instructions.
         * Magic may be EE, EF, FE or FF, but most emulators seem to use EE.
         * Based on tests on a couple of chips at
         * http://visual6502.org/wiki/index.php?title=6502_Opcode_8B_(XAA,_ANE)
         * the value of magic for the MOS 6510 is FF.
         * However the Lorentz test suite assumes this to be EE.
         */
        public byte magic =
#if PC64_TESTSUITE
    0xee;
#else
    0xff;
#endif

        //-------------------------------------------------------------------------//

        /**
         * When AEC signal is high, no stealing is possible.
         */
        private void eventWithoutSteals()
        {
            ProcessorCycle instr = instrTable[cycleCount++];
            instr.func();
            eventScheduler.schedule(m_nosteal, 1);
        }

        /**
         * When AEC signal is low, steals permitted.
         */
        private void eventWithSteals()
        {
            if (instrTable[cycleCount].nosteal)
            {
                ProcessorCycle instr = instrTable[cycleCount++];
                instr.func();
                eventScheduler.schedule(m_steal, 1);
            }
            else
            {
# if CORRECT_SH_INSTRUCTIONS
                // Save rdy state for SH* instructions
                if (instrTable[cycleCount].func == throwAwayRead)
                {
                    rdyOnThrowAwayRead = false;
                }
#endif
                // Even while stalled, the CPU can still process first clock of
                // interrupt delay, but only the first one.
                if (interruptCycle == cycleCount)
                {
                    interruptCycle--;
                }
            }
        }


        /**
         * Handle bus access signals. When RDY line is asserted, the CPU
         * will pause when executing the next read operation.
         *
         * @param newRDY new state for RDY signal
         */
        public void setRDY(bool newRDY)
        {
            rdy = newRDY;

            if (rdy)
            {
                eventScheduler.cancel(m_steal);
                eventScheduler.schedule(m_nosteal, 0, event_phase_t.EVENT_CLOCK_PHI2);
            }
            else
            {
                eventScheduler.cancel(m_nosteal);
                eventScheduler.schedule(m_steal, 0, event_phase_t.EVENT_CLOCK_PHI2);
            }
        }


        /**
         * Push P on stack, decrement S.
         */
        private void PushSR()
        {
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            cpuWrite(addr, flags.get());
            Register_StackPointer--;
        }

        /**
         * increment S, Pop P off stack.
         */
        private void PopSR()
        {
            // Get status register off stack
            Register_StackPointer++;
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            flags.set(cpuRead(addr));
            flags.setB(true);

            calculateInterruptTriggerCycle();
        }


        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Interrupt Routines                                                      //
        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//

        /**
         * This forces the CPU to abort whatever it is doing and immediately
         * enter the RST interrupt handling sequence. The implementation is
         * not compatible: instructions actually get aborted mid-execution.
         * However, there is no possible way to trigger this signal from
         * programs, so it's OK.
         */
        public void triggerRST()
        {
            Initialise();
            cycleCount = opcodes.BRKn << 3;
            rstFlag = true;
            calculateInterruptTriggerCycle();
        }

        /**
         * Trigger NMI interrupt on the CPU. Calling this method
         * flags that CPU must enter the NMI routine at earliest
         * opportunity. There is no way to cancel NMI request once
         * given.
         */
        public void triggerNMI()
        {
            nmiFlag = true;
            calculateInterruptTriggerCycle();

            /* maybe process 1 clock of interrupt delay. */
            if (!rdy)
            {
                eventScheduler.cancel(m_steal);
                eventScheduler.schedule(m_steal, 0, event_phase_t.EVENT_CLOCK_PHI2);
            }
        }

        /**
         * Pull IRQ line low on CPU.
         */
        public void triggerIRQ()
        {
            irqAssertedOnPin = true;
            calculateInterruptTriggerCycle();

            /* maybe process 1 clock of interrupt delay. */
            if (!rdy && interruptCycle == cycleCount)
            {
                eventScheduler.cancel(m_steal);
                eventScheduler.schedule(m_steal, 0, event_phase_t.EVENT_CLOCK_PHI2);
            }
        }

        /**
         * Inform CPU that IRQ is no longer pulled low.
         */
        public void clearIRQ()
        {
            irqAssertedOnPin = false;
            calculateInterruptTriggerCycle();
        }

        private void interruptsAndNextOpcode()
        {
            if (cycleCount > interruptCycle + 2)
            {
# if DEBUG
                //if (dodump)
                //{
                //    Int64 cycles = eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI2);
                //    Console.Write("****************************************************\n");
                //    Console.Write(" interrupt ({0})\n", cycles);
                //    Console.Write("****************************************************\n");
                //    DumpState((UInt64)cycles, this);
                //}
#endif
                cpuRead(Register_ProgramCounter);
                cycleCount = opcodes.BRKn << 3;
                flags.setB(false);
                interruptCycle = MAX;
            }
            else
            {
                fetchNextOpcode();
            }
        }

        private void fetchNextOpcode()
        {
# if DEBUG
            if (dodump)
            {
                DumpState((UInt64)eventScheduler.getTime(event_phase_t.EVENT_CLOCK_PHI2), this);
            }

            instrStartPC = Register_ProgramCounter;
#endif

# if CORRECT_SH_INSTRUCTIONS
            rdyOnThrowAwayRead = true;
#endif
            cycleCount = cpuRead(Register_ProgramCounter) << 3;
            Register_ProgramCounter++;

            if (!rstFlag && !nmiFlag && !(!flags.getI() && irqAssertedOnPin))
            {
                interruptCycle = MAX;
            }
            if (interruptCycle != MAX)
            {
                interruptCycle = -MAX;
            }
        }

        /**
         * Evaluate when to execute an interrupt. Calling this method can also
         * result in the decision that no interrupt at all needs to be scheduled.
         */
        private void calculateInterruptTriggerCycle()
        {
            /* Interrupt cycle not going to trigger? */
            if (interruptCycle == MAX)
            {
                if (rstFlag || nmiFlag || (!flags.getI() && irqAssertedOnPin))
                {
                    interruptCycle = cycleCount;
                }
            }
        }

        private void IRQLoRequest()
        {
            sidendian.endian_16lo8(ref Register_ProgramCounter, cpuRead(Cycle_EffectiveAddress));
        }

        private void IRQHiRequest()
        {
            sidendian.endian_16hi8(ref Register_ProgramCounter, cpuRead((UInt16)(Cycle_EffectiveAddress + 1)));
        }

        /**
         * Read the next opcode byte from memory (and throw it away)
         */
        private void throwAwayFetch()
        {
            cpuRead(Register_ProgramCounter);
        }

        /**
         * Issue throw-away read and fix address.
         * Some people use these to ACK CIA IRQs.
         */
        private void throwAwayRead()
        {
            cpuRead(Cycle_EffectiveAddress);
            if (adl_carry)
                Cycle_EffectiveAddress += 0x100;
        }

        /**
         * Fetch value, increment PC.
         *
         * Addressing Modes:
         * - Immediate
         * - Relative
         */
        private void FetchDataByte()
        {
            Cycle_Data = cpuRead(Register_ProgramCounter);
            if (flags.getB())
            {
                Register_ProgramCounter++;
            }

# if DEBUG
            instrOperand = Cycle_Data;
#endif
        }

        /**
         * Fetch low address byte, increment PC.
         *
         * Addressing Modes:
         * - Stack Manipulation
         * - Absolute
         * - Zero Page
         * - Zero Page Indexed
         * - Absolute Indexed
         * - Absolute Indirect
         */
        private void FetchLowAddr()
        {
            Cycle_EffectiveAddress = cpuRead(Register_ProgramCounter);
            Register_ProgramCounter++;

# if DEBUG
            instrOperand = Cycle_EffectiveAddress;
#endif
        }

        /**
         * Read from address, add index register X to it.
         *
         * Addressing Modes:
         * - Zero Page Indexed
         */
        private void FetchLowAddrX()
        {
            FetchLowAddr();
            Cycle_EffectiveAddress = (UInt16)((Cycle_EffectiveAddress + Register_X) & 0xFF);
        }

        /**
         * Read from address, add index register Y to it.
         *
         * Addressing Modes:
         * - Zero Page Indexed
         */
        private void FetchLowAddrY()
        {
            FetchLowAddr();
            Cycle_EffectiveAddress = (UInt16)((Cycle_EffectiveAddress + Register_Y) & 0xFF);
        }

        /**
         * Fetch high address byte, increment PC (Absolute Addressing).
         * Low byte must have been obtained first!
         *
         * Addressing Modes:
         * - Absolute
         */
        private void FetchHighAddr()
        {   // Get the high byte of an address from memory
            sidendian.endian_16hi8(ref Cycle_EffectiveAddress, cpuRead(Register_ProgramCounter));
            Register_ProgramCounter++;

# if DEBUG
            sidendian.endian_16hi8(ref instrOperand, sidendian.endian_16hi8(Cycle_EffectiveAddress));
#endif
        }

        /**
         * Fetch high byte of address, add index register X to low address byte,
         * increment PC.
         *
         * Addressing Modes:
         * - Absolute Indexed
         */
        private void FetchHighAddrX()
        {
            Cycle_EffectiveAddress += Register_X;
            adl_carry = Cycle_EffectiveAddress > 0xff;
            FetchHighAddr();
        }

        /**
         * Same as #FetchHighAddrX except doesn't worry about page crossing.
         */
        private void FetchHighAddrX2()
        {
            FetchHighAddrX();
            if (!adl_carry)
                cycleCount++;
        }

        /**
         * Fetch high byte of address, add index register Y to low address byte,
         * increment PC.
         *
         * Addressing Modes:
         * - Absolute Indexed
         */
        private void FetchHighAddrY()
        {
            Cycle_EffectiveAddress += Register_Y;
            adl_carry = Cycle_EffectiveAddress > 0xff;
            FetchHighAddr();
        }

        /**
         * Same as #FetchHighAddrY except doesn't worry about page crossing.
         */
        private void FetchHighAddrY2()
        {
            FetchHighAddrY();
            if (!adl_carry)
                cycleCount++;
        }

        /**
         * Fetch pointer address low, increment PC.
         *
         * Addressing Modes:
         * - Absolute Indirect
         * - Indirect indexed (post Y)
         */
        private void FetchLowPointer()
        {
            Cycle_Pointer = cpuRead(Register_ProgramCounter);
            Register_ProgramCounter++;

# if DEBUG
            instrOperand = Cycle_Pointer;
#endif
        }

        /**
         * Add X to it.
         *
         * Addressing Modes:
         * - Indexed Indirect (pre X)
         */
        private void FetchLowPointerX()
        {
            sidendian.endian_16lo8(ref Cycle_Pointer, (byte)((Cycle_Pointer + Register_X) & 0xFF));
        }

        /**
         * Fetch pointer address high, increment PC.
         *
         * Addressing Modes:
         * - Absolute Indirect
         */
        private void FetchHighPointer()
        {
            sidendian.endian_16hi8(ref Cycle_Pointer, cpuRead(Register_ProgramCounter));
            Register_ProgramCounter++;

# if DEBUG
            sidendian.endian_16hi8(ref instrOperand, sidendian.endian_16hi8(Cycle_Pointer));
#endif
        }

        /**
         * Fetch effective address low.
         *
         * Addressing Modes:
         * - Indirect
         * - Indexed Indirect (pre X)
         * - Indirect indexed (post Y)
         */
        private void FetchLowEffAddr()
        {
            Cycle_EffectiveAddress = cpuRead(Cycle_Pointer);
        }

        /**
         * Fetch effective address high.
         *
         * Addressing Modes:
         * - Indirect
         * - Indexed Indirect (pre X)
         */
        private void FetchHighEffAddr()
        {
            sidendian.endian_16lo8(ref Cycle_Pointer, (byte)((Cycle_Pointer + 1) & 0xff));
            sidendian.endian_16hi8(ref Cycle_EffectiveAddress, cpuRead(Cycle_Pointer));
        }

        /**
         * Fetch effective address high, add Y to low byte of effective address.
         *
         * Addressing Modes:
         * - Indirect indexed (post Y)
         */
        private void FetchHighEffAddrY()
        {
            Cycle_EffectiveAddress += Register_Y;
            adl_carry = Cycle_EffectiveAddress > 0xff;
            FetchHighEffAddr();
        }


        /**
         * Same as #FetchHighEffAddrY except doesn't worry about page crossing.
         */
        private void FetchHighEffAddrY2()
        {
            FetchHighEffAddrY();
            if (!adl_carry)
                cycleCount++;
        }

        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Common Data Accessing Routines                                          //
        // Data Accessing operations as described in 64doc by John West and        //
        // Marko Makela                                                            //
        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//

        private void FetchEffAddrDataByte()
        {
            Cycle_Data = cpuRead(Cycle_EffectiveAddress);
        }

        /**
         * Write Cycle_Data to effective address.
         */
        private void PutEffAddrDataByte()
        {
            cpuWrite(Cycle_EffectiveAddress, Cycle_Data);
        }

        /**
         * Push Program Counter Low Byte on stack, decrement S.
         */
        private void PushLowPC()
        {
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            cpuWrite(addr, sidendian.endian_16lo8(Register_ProgramCounter));
            Register_StackPointer--;
        }

        /**
         * Push Program Counter High Byte on stack, decrement S.
         */
        private void PushHighPC()
        {
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            cpuWrite(addr, sidendian.endian_16hi8(Register_ProgramCounter));
            Register_StackPointer--;
        }

        /**
         * Increment stack and pull program counter low byte from stack.
         */
        private void PopLowPC()
        {
            Register_StackPointer++;
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            sidendian.endian_16lo8(ref Cycle_EffectiveAddress, cpuRead(addr));
        }

        /**
         * Increment stack and pull program counter high byte from stack.
         */
        private void PopHighPC()
        {
            Register_StackPointer++;
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            sidendian.endian_16hi8(ref Cycle_EffectiveAddress, cpuRead(addr));
        }

        private void WasteCycle() { }

        private void brkPushLowPC()
        {
            PushLowPC();
            if (rstFlag)
            {
                /* rst = %10x */
                Cycle_EffectiveAddress = 0xfffc;
            }
            else if (nmiFlag)
            {
                /* nmi = %01x */
                Cycle_EffectiveAddress = 0xfffa;
            }
            else
            {
                /* irq = %11x */
                Cycle_EffectiveAddress = 0xfffe;
            }

            rstFlag = false;
            nmiFlag = false;
            calculateInterruptTriggerCycle();
        }

        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Common Instruction Opcodes                                              //
        // See and 6510 Assembly Book for more information on these instructions   //
        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//

        private void brk_instr()
        {
            PushSR();
            flags.setB(true);
            flags.setI(true);
        }

        private void cld_instr()
        {
            flags.setD(false);
            interruptsAndNextOpcode();
        }

        private void cli_instr()
        {
            flags.setI(false);
            calculateInterruptTriggerCycle();
            interruptsAndNextOpcode();
        }

        private void jmp_instr()
        {
            doJSR();
            interruptsAndNextOpcode();
        }

        private void doJSR()
        {
            Register_ProgramCounter = Cycle_EffectiveAddress;

# if PC64_TESTSUITE
            // trap handlers
            if (Register_ProgramCounter == 0xffd2)
            {
                // Print character
                byte ch = CHRtab[Register_Accumulator];
                switch (ch)
                {
                    case 0:
                        break;
                    case 1:
                        Console.Write(" ");
                        break;
                    case 0xd:
                        Console.Write( "\n");
                        filepos = 0;
                        break;
                    default:
                        filetmp[filepos++] = ch;
                        Console.Write( "{0}", ch);
                }
            }
            else if (Register_ProgramCounter == 0xe16f)
            {
                // Load
                filetmp[filepos] = '\0';
                loadFile(filetmp);
            }
            else if (Register_ProgramCounter == 0x8000
                || Register_ProgramCounter == 0xa474)
            {
                // Stop
                exit(0);
            }
#endif // PC64_TESTSUITE
        }

        private void pha_instr()
        {
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            cpuWrite(addr, Register_Accumulator);
            Register_StackPointer--;
        }

        /**
         * RTI does not delay the IRQ I flag change as it is set 3 cycles before
         * the end of the opcode, and thus the 6510 has enough time to call the
         * interrupt routine as soon as the opcode ends, if necessary.
         */
        private void rti_instr()
        {
# if DEBUG
            //if (dodump)
            //    Console.Write("****************************************************\n\n");
#endif
            Register_ProgramCounter = Cycle_EffectiveAddress;
            interruptsAndNextOpcode();
        }

        private void rts_instr()
        {
            cpuRead(Cycle_EffectiveAddress);
            Register_ProgramCounter = Cycle_EffectiveAddress;
            Register_ProgramCounter++;
        }

        private void sed_instr()
        {
            flags.setD(true);
            interruptsAndNextOpcode();
        }

        private void sei_instr()
        {
            flags.setI(true);
            interruptsAndNextOpcode();
            if (!rstFlag && !nmiFlag && interruptCycle != MAX)
                interruptCycle = MAX;
        }

        private void sta_instr()
        {
            Cycle_Data = Register_Accumulator;
            PutEffAddrDataByte();
        }

        private void stx_instr()
        {
            Cycle_Data = Register_X;
            PutEffAddrDataByte();
        }

        private void sty_instr()
        {
            Cycle_Data = Register_Y;
            PutEffAddrDataByte();
        }



        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Common Instruction Undocumented Opcodes                                 //
        // See documented 6502-nmo.opc by Adam Vardy for more details              //
        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//

#if false
// Not required - Operation performed By another method
//
// Undocumented - HLT crashes the microprocessor.  When this opcode is executed, program
// execution ceases.  No hardware interrupts will execute either.  The author
// has characterized this instruction as a halt instruction since this is the
// most straightforward explanation for this opcode's behaviour.  Only a reset
// will restart execution.  This opcode leaves no trace of any operation
// performed!  No registers affected.
void MOS6510::hlt_instr() {}
#endif

        /**
         * Perform the SH* instructions.
         *
         * @param offset the index added to the address
         */
        private void sh_instr(byte offset)
        {
            byte tmp = (byte)(Cycle_Data & (sidendian.endian_16hi8((UInt16)(Cycle_EffectiveAddress - offset)) + 1));

# if CORRECT_SH_INSTRUCTIONS
            /*
             * When a DMA is going on (the CPU is halted by the VIC-II)
             * while the instruction sha/shx/shy executes then the last
             * term of the ANDing (ADH+1) drops off.
             *
             * http://sourceforge.net/p/vice-emu/bugs/578/
             */
            if (rdyOnThrowAwayRead)
            {
                Cycle_Data = tmp;
            }
#endif

            /*
             * When the addressing/indexing causes a page boundary crossing
             * the highbyte of the target address becomes equal to the value stored.
             */
            if (adl_carry)
                sidendian.endian_16hi8(ref Cycle_EffectiveAddress, tmp);
            PutEffAddrDataByte();
        }

        /**
         * Undocumented - This opcode stores the result of A AND X AND ADH+1 in memory.
         */
        private void axa_instr()
        {
            Cycle_Data = (byte)(Register_X & Register_Accumulator);
            sh_instr(Register_Y);
        }

        /**
         * Undocumented - This opcode ANDs the contents of the Y register with ADH+1 and stores the
         * result in memory.
         */
        private void say_instr()
        {
            Cycle_Data = Register_Y;
            sh_instr(Register_X);
        }

        /**
         * Undocumented - This opcode ANDs the contents of the X register with ADH+1 and stores the
         * result in memory.
         */
        private void xas_instr()
        {
            Cycle_Data = Register_X;
            sh_instr(Register_Y);
        }

        /**
         * Undocumented - AXS ANDs the contents of the A and X registers (without changing the
         * contents of either register) and stores the result in memory.
         * AXS does not affect any flags in the processor status register.
         */
        private void axs_instr()
        {
            Cycle_Data = (byte)(Register_Accumulator & Register_X);
            PutEffAddrDataByte();
        }


        /**
         * BCD adding.
         */
        private void doADC()
        {
            UInt32 C = (UInt32)(flags.getC() ? 1 : 0);
            UInt32 A = Register_Accumulator;
            UInt32 s = Cycle_Data;
            UInt32 regAC2 = A + s + C;

            if (flags.getD())
            {   // BCD mode
                UInt32 lo = (A & 0x0f) + (s & 0x0f) + C;
                UInt32 hi = (A & 0xf0) + (s & 0xf0);
                if (lo > 0x09)
                    lo += 0x06;
                if (lo > 0x0f)
                    hi += 0x10;

                flags.setZ((regAC2 & 0xff) == 0);
                flags.setN((hi & 0x80) != 0);
                flags.setV(((hi ^ A) & 0x80) != 0 && ((A ^ s) & 0x80) == 0);
                if (hi > 0x90)
                    hi += 0x60;

                flags.setC(hi > 0xff);
                Register_Accumulator = (byte)(hi | (lo & 0x0f));
            }
            else
            {   // Binary mode
                flags.setC(regAC2 > 0xff);
                flags.setV(((regAC2 ^ A) & 0x80) != 0 && ((A ^ s) & 0x80) == 0);
                flags.setNZ(Register_Accumulator = (byte)(regAC2 & 0xff));
            }
        }

        /**
         * BCD subtracting.
         */
        private void doSBC()
        {
            UInt32 C = (UInt32)(flags.getC() ? 0 : 1);
            UInt32 A = Register_Accumulator;
            UInt32 s = Cycle_Data;
            UInt32 regAC2 = A - s - C;

            flags.setC(regAC2 < 0x100);
            flags.setV(((regAC2 ^ A) & 0x80) != 0 && ((A ^ s) & 0x80) == 0);
            flags.setNZ((byte)regAC2);

            if (flags.getD())
            {   // BCD mode
                UInt32 lo = (A & 0x0f) - (s & 0x0f) - C;
                UInt32 hi = (A & 0xf0) - (s & 0xf0);
                if ((lo & 0x10) != 0)
                {
                    lo -= 0x06;
                    hi -= 0x10;
                }
                if ((hi & 0x100) != 0)
                    hi -= 0x60;
                Register_Accumulator = (byte)(hi | (lo & 0x0f));
            }
            else
            {   // Binary mode
                Register_Accumulator = (byte)(regAC2 & 0xff);
            }
        }



        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Generic Instruction Addressing Routines                                 //
        //-------------------------------------------------------------------------//


        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Generic Instruction Opcodes                                             //
        // See and 6510 Assembly Book for more information on these instructions   //
        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//

        private void adc_instr()
        {
            doADC();
            interruptsAndNextOpcode();
        }

        private void and_instr()
        {
            flags.setNZ(Register_Accumulator &= Cycle_Data);
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - For a detailed explanation of this opcode look at:
         * http://visual6502.org/wiki/index.php?title=6502_Opcode_8B_(XAA,_ANE)
         */
        private void ane_instr()
        {
            flags.setNZ(Register_Accumulator = (byte)((Register_Accumulator | magic) & Register_X & Cycle_Data));
            interruptsAndNextOpcode();
        }

        private void asl_instr()
        {
            PutEffAddrDataByte();
            flags.setC((Cycle_Data & 0x80) != 0);
            flags.setNZ(Cycle_Data <<= 1);
        }

        private void asla_instr()
        {
            flags.setC((Register_Accumulator & 0x80) != 0);
            flags.setNZ(Register_Accumulator <<= 1);
            interruptsAndNextOpcode();
        }

        private void fix_branch()
        {
            // Throw away read
            cpuRead(Cycle_EffectiveAddress);

            // Fix address
            Register_ProgramCounter += (UInt16)(Cycle_Data < 0x80 ? 0x0100 : 0xff00);
        }

        private void branch_instr(bool condition)
        {
            // 2 cycles spent before arriving here. spend 0 - 2 cycles here;
            // - condition false: Continue immediately to FetchNextInstr.
            //
            // Otherwise read the byte following the opcode (which is already scheduled to occur on this cycle).
            // This effort is wasted. Then calculate address of the branch target. If branch is on same page,
            // then continue at that insn on next cycle (this delays IRQs by 1 clock for some reason, allegedly).
            //
            // If the branch is on different memory page, issue a spurious read with wrong high byte before
            // continuing at the correct address.
            if (condition)
            {
                // issue the spurious read for next insn here.
                cpuRead(Register_ProgramCounter);

                Cycle_EffectiveAddress = sidendian.endian_16lo8(Register_ProgramCounter);
                Cycle_EffectiveAddress += Cycle_Data;
                adl_carry = (Cycle_EffectiveAddress > 0xff) != (Cycle_Data > 0x7f);
                sidendian.endian_16hi8(ref Cycle_EffectiveAddress, sidendian.endian_16hi8(Register_ProgramCounter));

                Register_ProgramCounter = Cycle_EffectiveAddress;

                // Check for page boundary crossing
                if (!adl_carry)
                {
                    // Skip next throw away read
                    cycleCount++;

                    // Hack: delay the interrupt past this instruction.
                    if (interruptCycle >> 3 == cycleCount >> 3)
                        interruptCycle += 2;
                }
            }
            else
            {
                // branch not taken: skip the following spurious read insn and go to FetchNextInstr immediately.
                interruptsAndNextOpcode();
            }
        }

        private void bcc_instr()
        {
            branch_instr(!flags.getC());
        }

        private void bcs_instr()
        {
            branch_instr(flags.getC());
        }

        private void beq_instr()
        {
            branch_instr(flags.getZ());
        }

        private void bit_instr()
        {
            flags.setZ((Register_Accumulator & Cycle_Data) == 0);
            flags.setN((Cycle_Data & 0x80) != 0);
            flags.setV((Cycle_Data & 0x40) != 0);
            interruptsAndNextOpcode();
        }

        private void bmi_instr()
        {
            branch_instr(flags.getN());
        }

        private void bne_instr()
        {
            branch_instr(!flags.getZ());
        }

        private void bpl_instr()
        {
            branch_instr(!flags.getN());
        }

        private void bvc_instr()
        {
            branch_instr(!flags.getV());
        }

        private void bvs_instr()
        {
            branch_instr(flags.getV());
        }

        private void clc_instr()
        {
            flags.setC(false);
            interruptsAndNextOpcode();
        }

        private void clv_instr()
        {
            flags.setV(false);
            interruptsAndNextOpcode();
        }

        private void cmp_instr()
        {
            UInt16 tmp = (UInt16)(Register_Accumulator - Cycle_Data);
            flags.setNZ((byte)tmp);
            flags.setC(tmp < 0x100);
            interruptsAndNextOpcode();
        }

        private void cpx_instr()
        {
            UInt16 tmp = (UInt16)(Register_X - Cycle_Data);
            flags.setNZ((byte)tmp);
            flags.setC(tmp < 0x100);
            interruptsAndNextOpcode();
        }

        private void cpy_instr()
        {
            UInt16 tmp = (UInt16)(Register_Y - Cycle_Data);
            flags.setNZ((byte)tmp);
            flags.setC(tmp < 0x1009);
            interruptsAndNextOpcode();
        }

        private void dec_instr()
        {
            PutEffAddrDataByte();
            flags.setNZ(--Cycle_Data);
        }

        private void dex_instr()
        {
            flags.setNZ(--Register_X);
            interruptsAndNextOpcode();
        }

        private void dey_instr()
        {
            flags.setNZ(--Register_Y);
            interruptsAndNextOpcode();
        }

        private void eor_instr()
        {
            flags.setNZ(Register_Accumulator ^= Cycle_Data);
            interruptsAndNextOpcode();
        }

        private void inc_instr()
        {
            PutEffAddrDataByte();
            flags.setNZ(++Cycle_Data);
        }

        private void inx_instr()
        {
            flags.setNZ(++Register_X);
            interruptsAndNextOpcode();
        }

        private void iny_instr()
        {
            flags.setNZ(++Register_Y);
            interruptsAndNextOpcode();
        }

        private void lda_instr()
        {
            flags.setNZ(Register_Accumulator = Cycle_Data);
            interruptsAndNextOpcode();
        }

        private void ldx_instr()
        {
            flags.setNZ(Register_X = Cycle_Data);
            interruptsAndNextOpcode();
        }

        private void ldy_instr()
        {
            flags.setNZ(Register_Y = Cycle_Data);
            interruptsAndNextOpcode();
        }

        private void lsr_instr()
        {
            PutEffAddrDataByte();
            flags.setC((Cycle_Data & 0x01) != 0);
            flags.setNZ(Cycle_Data >>= 1);
        }

        private void lsra_instr()
        {
            flags.setC((Register_Accumulator & 0x01) != 0);
            flags.setNZ(Register_Accumulator >>= 1);
            interruptsAndNextOpcode();
        }

        private void ora_instr()
        {
            flags.setNZ(Register_Accumulator |= Cycle_Data);
            interruptsAndNextOpcode();
        }

        private void pla_instr()
        {
            Register_StackPointer++;
            UInt16 addr = sidendian.endian_16(SP_PAGE, Register_StackPointer);
            flags.setNZ(Register_Accumulator = cpuRead(addr));
        }

        private void plp_instr()
        {
            interruptsAndNextOpcode();
        }

        private void rol_instr()
        {
            byte newC = (byte)(Cycle_Data & 0x80);
            PutEffAddrDataByte();
            Cycle_Data <<= 1;
            if (flags.getC())
                Cycle_Data |= 0x01;
            flags.setNZ(Cycle_Data);
            flags.setC(newC != 0);
        }

        private void rola_instr()
        {
            byte newC = (byte)(Register_Accumulator & 0x80);
            Register_Accumulator <<= 1;
            if (flags.getC())
                Register_Accumulator |= 0x01;
            flags.setNZ(Register_Accumulator);
            flags.setC(newC != 0);
            interruptsAndNextOpcode();
        }

        private void ror_instr()
        {
            byte newC = (byte)(Cycle_Data & 0x01);
            PutEffAddrDataByte();
            Cycle_Data >>= 1;
            if (flags.getC())
                Cycle_Data |= 0x80;
            flags.setNZ(Cycle_Data);
            flags.setC(newC != 0);
        }

        private void rora_instr()
        {
            byte newC = (byte)(Register_Accumulator & 0x01);
            Register_Accumulator >>= 1;
            if (flags.getC())
                Register_Accumulator |= 0x80;
            flags.setNZ(Register_Accumulator);
            flags.setC(newC != 0);
            interruptsAndNextOpcode();
        }

        private void sbx_instr()
        {
            UInt32 tmp = (UInt32)((Register_X & Register_Accumulator) - Cycle_Data);
            flags.setNZ(Register_X = (byte)(tmp & 0xff));
            flags.setC(tmp < 0x100);
            interruptsAndNextOpcode();
        }

        private void sbc_instr()
        {
            doSBC();
            interruptsAndNextOpcode();
        }

        private void sec_instr()
        {
            flags.setC(true);
            interruptsAndNextOpcode();
        }

        private void shs_instr()
        {
            Register_StackPointer = (byte)(Register_Accumulator & Register_X);
            Cycle_Data = Register_StackPointer;
            sh_instr(Register_Y);
        }

        private void tax_instr()
        {
            flags.setNZ(Register_X = Register_Accumulator);
            interruptsAndNextOpcode();
        }

        private void tay_instr()
        {
            flags.setNZ(Register_Y = Register_Accumulator);
            interruptsAndNextOpcode();
        }

        private void tsx_instr()
        {
            flags.setNZ(Register_X = Register_StackPointer);
            interruptsAndNextOpcode();
        }

        private void txa_instr()
        {
            flags.setNZ(Register_Accumulator = Register_X);
            interruptsAndNextOpcode();
        }

        private void txs_instr()
        {
            Register_StackPointer = Register_X;
            interruptsAndNextOpcode();
        }

        private void tya_instr()
        {
            flags.setNZ(Register_Accumulator = Register_Y);
            interruptsAndNextOpcode();
        }

        private void invalidOpcode()
        {
            throw new haltInstruction();
        }


        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//
        // Generic Instruction Undocumented Opcodes                                //
        // See documented 6502-nmo.opc by Adam Vardy for more details              //
        //-------------------------------------------------------------------------//
        //-------------------------------------------------------------------------//

        /**
         * Undocumented - This opcode ANDs the contents of the A register with an immediate value and
         * then LSRs the result.
         */
        private void alr_instr()
        {
            Register_Accumulator &= Cycle_Data;
            flags.setC((Register_Accumulator & 0x01) != 0);
            flags.setNZ(Register_Accumulator >>= 1);
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - ANC ANDs the contents of the A register with an immediate value and then
         * moves bit 7 of A into the Carry flag.  This opcode works basically
         * identically to AND #immed. except that the Carry flag is set to the same
         * state that the Negative flag is set to.
         */
        private void anc_instr()
        {
            flags.setNZ(Register_Accumulator &= Cycle_Data);
            flags.setC(flags.getN());
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - This opcode ANDs the contents of the A register with an immediate value and
         * then RORs the result. (Implementation based on that of Frodo C64 Emulator)
         */
        private void arr_instr()
        {
            byte data = (byte)(Cycle_Data & Register_Accumulator);
            Register_Accumulator = (byte)(data >> 1);
            if (flags.getC())
                Register_Accumulator |= 0x80;

            if (flags.getD())
            {
                flags.setN(flags.getC());
                flags.setZ(Register_Accumulator == 0);
                flags.setV(((data ^ Register_Accumulator) & 0x40) != 0);

                if ((data & 0x0f) + (data & 0x01) > 5)
                    Register_Accumulator = (byte)((Register_Accumulator & 0xf0) | ((Register_Accumulator + 6) & 0x0f));
                flags.setC(((data + (data & 0x10)) & 0x1f0) > 0x50);
                if (flags.getC())
                    Register_Accumulator += 0x60;
            }
            else
            {
                flags.setNZ(Register_Accumulator);
                flags.setC((Register_Accumulator & 0x40) != 0);
                flags.setV(((Register_Accumulator & 0x40) ^ ((Register_Accumulator & 0x20) << 1)) != 0);
            }
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - This opcode ASLs the contents of a memory location and then ORs the result
         * with the accumulator.
         */
        private void aso_instr()
        {
            PutEffAddrDataByte();
            flags.setC((Cycle_Data & 0x80) != 0);
            Cycle_Data <<= 1;
            flags.setNZ(Register_Accumulator |= Cycle_Data);
        }

        /**
         * Undocumented - This opcode DECs the contents of a memory location and then CMPs the result
         * with the A register.
         */
        private void dcm_instr()
        {
            PutEffAddrDataByte();
            Cycle_Data--;
            UInt16 tmp = (UInt16)(Register_Accumulator - Cycle_Data);
            flags.setNZ((byte)tmp);
            flags.setC(tmp < 0x100);
        }

        /**
         * Undocumented - This opcode INCs the contents of a memory location and then SBCs the result
         * from the A register.
         */
        private void ins_instr()
        {
            PutEffAddrDataByte();
            Cycle_Data++;
            doSBC();
        }

        /**
         * Undocumented - This opcode ANDs the contents of a memory location with the contents of the
         * stack pointer register and stores the result in the accumulator, the X
         * register, and the stack pointer. Affected flags: N Z.
         */
        private void las_instr()
        {
            flags.setNZ(Cycle_Data &= Register_StackPointer);
            Register_Accumulator = Cycle_Data;
            Register_X = Cycle_Data;
            Register_StackPointer = Cycle_Data;
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - This opcode loads both the accumulator and the X register with the contents
         * of a memory location.
         */
        private void lax_instr()
        {
            flags.setNZ(Register_Accumulator = Register_X = Cycle_Data);
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - LSE LSRs the contents of a memory location and then EORs the result with
         * the accumulator.
         */
        private void lse_instr()
        {
            PutEffAddrDataByte();
            flags.setC((Cycle_Data & 0x01) != 0);
            Cycle_Data >>= 1;
            flags.setNZ(Register_Accumulator ^= Cycle_Data);
        }

        /**
         * Undocumented - This opcode ORs the A register with #xx (the "magic" value),
         * ANDs the result with an immediate value, and then stores the result in both A and X.
         */
        private void oal_instr()
        {
            flags.setNZ(Register_X = (Register_Accumulator = (byte)(Cycle_Data & (Register_Accumulator | magic))));
            interruptsAndNextOpcode();
        }

        /**
         * Undocumented - RLA ROLs the contents of a memory location and then ANDs the result with
         * the accumulator.
         */
        private void rla_instr()
        {
            byte newC = (byte)(Cycle_Data & 0x80);
            PutEffAddrDataByte();
            Cycle_Data = (byte)(Cycle_Data << 1);
            if (flags.getC())
                Cycle_Data |= 0x01;
            flags.setC(newC != 0);
            flags.setNZ(Register_Accumulator &= Cycle_Data);
        }

        /**
         * Undocumented - RRA RORs the contents of a memory location and then ADCs the result with
         * the accumulator.
         */
        private void rra_instr()
        {
            byte newC = (byte)(Cycle_Data & 0x01);
            PutEffAddrDataByte();
            Cycle_Data >>= 1;
            if (flags.getC())
                Cycle_Data |= 0x80;
            flags.setC(newC != 0);
            doADC();
        }

        //-------------------------------------------------------------------------//

        /**
         * Create new CPU emu.
         *
         * @param context
         *            The Event Context
         */
        protected MOS6510(EventScheduler scheduler)
        {
            eventScheduler = scheduler;
#if DEBUG
            //m_fdbg = stdout;
#endif
            m_nosteal = new EventCallback<MOS6510>("CPU-nosteal", this, eventWithoutSteals);
            m_steal = new EventCallback<MOS6510>("CPU-steal", this, eventWithSteals);
            buildInstructionTable();

            // Intialise Processor Registers
            Register_Accumulator = 0;
            Register_X = 0;
            Register_Y = 0;

            Cycle_EffectiveAddress = 0;
            Cycle_Data = 0;
# if DEBUG
            dodump = false;
#endif
            Initialise();
        }

        public enum AccessMode { WRITE, READ }

        /**
         * Build up the processor instruction table.
         */
        private void buildInstructionTable()
        {
            for (int i = 0; i < instrTable.Length; i++) instrTable[i] = new ProcessorCycle();

            for (UInt32 i = 0; i < 0x100; i++)
            {
#if DEBUG 
                //Console.Write("Building Command {0}[{1:x02}]... ", i, i);
#endif

                /*
                 * So: what cycles are marked as stealable? Rules are:
                 *
                 * - CPU performs either read or write at every cycle. Reads are
                 *   always stealable. Writes are rare.
                 *
                 * - Every instruction begins with a sequence of reads. Writes,
                 *   if any, are at the end for most instructions.
                 */

                UInt32 buildCycle = i << 3;

                AccessMode access = AccessMode.WRITE;
                bool legalMode = true;
                bool legalInstr = true;

                switch (i)
                {
                    // Accumulator or Implied addressing
                    case opcodes.ASLn: case opcodes.CLCn: case opcodes.CLDn: case opcodes.CLIn: case opcodes.CLVn: case opcodes.DEXn:
                    case opcodes.DEYn: case opcodes.INXn: case opcodes.INYn: case opcodes.LSRn: case opcodes.NOPn:
                    case 0x1A:
                    case 0x3A:
                    case 0x5A:
                    case 0x7A:
                    case 0xDA:
                    case 0xFA: case opcodes.PHAn:
                    case opcodes.PHPn: case opcodes.PLAn: case opcodes.PLPn: case opcodes.ROLn: case opcodes.RORn:
                    case opcodes.SECn: case opcodes.SEDn: case opcodes.SEIn: case opcodes.TAXn: case opcodes.TAYn:
                    case opcodes.TSXn: case opcodes.TXAn: case opcodes.TXSn: case opcodes.TYAn:
                        instrTable[buildCycle++].func = throwAwayFetch;
                        break;

                    // Immediate and Relative Addressing Mode Handler
                    case opcodes.ADCb: case opcodes.ANDb: case opcodes.ANCb:
                    case 0x2B: case opcodes.ANEb: case opcodes.ASRb: case opcodes.ARRb:
                    case opcodes.BCCr: case opcodes.BCSr: case opcodes.BEQr: case opcodes.BMIr: case opcodes.BNEr: case opcodes.BPLr:
                    case opcodes.BRKn: case opcodes.BVCr: case opcodes.BVSr: case opcodes.CMPb: case opcodes.CPXb: case opcodes.CPYb:
                    case opcodes.EORb: case opcodes.LDAb: case opcodes.LDXb: case opcodes.LDYb: case opcodes.LXAb: case opcodes.NOPb:
                    case 0x82:
                    case 0xC2:
                    case 0xE2:
                    case 0x89:
                    case opcodes.ORAb: case opcodes.SBCb:
                    case 0XEB: case opcodes.SBXb: case opcodes.RTIn: case opcodes.RTSn:
                        instrTable[buildCycle++].func = FetchDataByte;
                        break;

                    // Zero Page Addressing Mode Handler - Read & RMW
                    case opcodes.ADCz: case opcodes.ANDz: case opcodes.BITz: case opcodes.CMPz: case opcodes.CPXz: case opcodes.CPYz:
                    case opcodes.EORz: case opcodes.LAXz: case opcodes.LDAz: case opcodes.LDXz: case opcodes.LDYz: case opcodes.ORAz:
                    case opcodes.NOPz:
                    case 0x44:
                    case 0x64: case opcodes.SBCz:
                    case opcodes.ASLz: case opcodes.DCPz: case opcodes.DECz: case opcodes.INCz: case opcodes.ISBz: case opcodes.LSRz:
                    case opcodes.ROLz: case opcodes.RORz: case opcodes.SREz: case opcodes.SLOz: case opcodes.RLAz: case opcodes.RRAz:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddr;
                        break;
                    case opcodes.SAXz: case opcodes.STAz: case opcodes.STXz: case opcodes.STYz:
                        instrTable[buildCycle++].func = FetchLowAddr;
                        break;

                    // Zero Page with X Offset Addressing Mode Handler
                    // these issue extra reads on the 0 page, but we don't care about it
                    // because there are no detectable effects from them. These reads
                    // occur during the "wasted" cycle.
                    case opcodes.ADCzx: case opcodes.ANDzx: case opcodes.CMPzx: case opcodes.EORzx: case opcodes.LDAzx: case opcodes.LDYzx:
                    case opcodes.NOPzx:
                    case 0x34:
                    case 0x54:
                    case 0x74:
                    case 0xD4:
                    case 0xF4: case opcodes.ORAzx: case opcodes.SBCzx:
                    case opcodes.ASLzx: case opcodes.DCPzx: case opcodes.DECzx: case opcodes.INCzx: case opcodes.ISBzx: case opcodes.LSRzx:
                    case opcodes.RLAzx: case opcodes.ROLzx: case opcodes.RORzx: case opcodes.RRAzx: case opcodes.SLOzx: case opcodes.SREzx:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddrX;
                        // operates on 0 page in read mode. Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        break;
                    case opcodes.STAzx: case opcodes.STYzx:
                        instrTable[buildCycle++].func = FetchLowAddrX;
                        // operates on 0 page in read mode. Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        break;

                    // Zero Page with Y Offset Addressing Mode Handler
                    case opcodes.LDXzy: case opcodes.LAXzy:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddrY;
                        // operates on 0 page in read mode. Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        break;
                    case opcodes.STXzy: case opcodes.SAXzy:
                        instrTable[buildCycle++].func = FetchLowAddrY;
                        // operates on 0 page in read mode. Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        break;

                    // Absolute Addressing Mode Handler
                    case opcodes.ADCa: case opcodes.ANDa: case opcodes.BITa: case opcodes.CMPa: case opcodes.CPXa: case opcodes.CPYa:
                    case opcodes.EORa: case opcodes.LAXa: case opcodes.LDAa: case opcodes.LDXa: case opcodes.LDYa: case opcodes.NOPa:
                    case opcodes.ORAa: case opcodes.SBCa:
                    case opcodes.ASLa: case opcodes.DCPa: case opcodes.DECa: case opcodes.INCa: case opcodes.ISBa: case opcodes.LSRa:
                    case opcodes.ROLa: case opcodes.RORa: case opcodes.SLOa: case opcodes.SREa: case opcodes.RLAa: case opcodes.RRAa:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddr;
                        break;
                    case opcodes.JMPw: case opcodes.SAXa: case opcodes.STAa: case opcodes.STXa: case opcodes.STYa:
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddr;
                        break;

                    case opcodes.JSRw:
                        instrTable[buildCycle++].func = FetchLowAddr;
                        break;

                    // Absolute With X Offset Addressing Mode Handler (Read)
                    case opcodes.ADCax: case opcodes.ANDax: case opcodes.CMPax: case opcodes.EORax: case opcodes.LDAax:
                    case opcodes.LDYax: case opcodes.NOPax:
                    case 0x3C:
                    case 0x5C:
                    case 0x7C:
                    case 0xDC:
                    case 0xFC: case opcodes.ORAax: case opcodes.SBCax:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddrX2;
                        // this cycle is skipped if the address is already correct.
                        // otherwise, it will be read and ignored.
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;

                    // Absolute X (RMW; no page crossing handled, always reads before writing)
                    case opcodes.ASLax: case opcodes.DCPax: case opcodes.DECax: case opcodes.INCax: case opcodes.ISBax:
                    case opcodes.LSRax: case opcodes.RLAax: case opcodes.ROLax: case opcodes.RORax: case opcodes.RRAax:
                    case opcodes.SLOax: case opcodes.SREax:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddrX;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;
                    case opcodes.SHYax: case opcodes.STAax:
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddrX;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;

                    // Absolute With Y Offset Addresing Mode Handler (Read)
                    case opcodes.ADCay: case opcodes.ANDay: case opcodes.CMPay: case opcodes.EORay: case opcodes.LASay:
                    case opcodes.LAXay: case opcodes.LDAay: case opcodes.LDXay: case opcodes.ORAay: case opcodes.SBCay:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddrY2;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;

                    // Absolute Y (No page crossing handled)
                    case opcodes.DCPay: case opcodes.ISBay: case opcodes.RLAay: case opcodes.RRAay: case opcodes.SLOay:
                    case opcodes.SREay:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddrY;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;
                    case opcodes.SHAay: case opcodes.SHSay: case opcodes.SHXay: case opcodes.STAay:
                        instrTable[buildCycle++].func = FetchLowAddr;
                        instrTable[buildCycle++].func = FetchHighAddrY;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;

                    // Absolute Indirect Addressing Mode Handler
                    case opcodes.JMPi:
                        instrTable[buildCycle++].func = FetchLowPointer;
                        instrTable[buildCycle++].func = FetchHighPointer;
                        instrTable[buildCycle++].func = FetchLowEffAddr;
                        instrTable[buildCycle++].func = FetchHighEffAddr;
                        break;

                    // Indexed with X Preinc Addressing Mode Handler
                    case opcodes.ADCix: case opcodes.ANDix: case opcodes.CMPix: case opcodes.EORix: case opcodes.LAXix: case opcodes.LDAix:
                    case opcodes.ORAix: case opcodes.SBCix:
                    case opcodes.DCPix: case opcodes.ISBix: case opcodes.SLOix: case opcodes.SREix: case opcodes.RLAix: case opcodes.RRAix:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowPointer;
                        instrTable[buildCycle++].func = FetchLowPointerX;
                        instrTable[buildCycle++].func = FetchLowEffAddr;
                        instrTable[buildCycle++].func = FetchHighEffAddr;
                        break;
                    case opcodes.SAXix: case opcodes.STAix:
                        instrTable[buildCycle++].func = FetchLowPointer;
                        instrTable[buildCycle++].func = FetchLowPointerX;
                        instrTable[buildCycle++].func = FetchLowEffAddr;
                        instrTable[buildCycle++].func = FetchHighEffAddr;
                        break;

                    // Indexed with Y Postinc Addressing Mode Handler (Read)
                    case opcodes.ADCiy: case opcodes.ANDiy: case opcodes.CMPiy: case opcodes.EORiy: case opcodes.LAXiy:
                    case opcodes.LDAiy: case opcodes.ORAiy: case opcodes.SBCiy:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowPointer;
                        instrTable[buildCycle++].func = FetchLowEffAddr;
                        instrTable[buildCycle++].func = FetchHighEffAddrY2;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;

                    // Indexed Y (No page crossing handled)
                    case opcodes.DCPiy: case opcodes.ISBiy: case opcodes.RLAiy: case opcodes.RRAiy: case opcodes.SLOiy:
                    case opcodes.SREiy:
                        access = AccessMode.READ;
                        instrTable[buildCycle++].func = FetchLowPointer;
                        instrTable[buildCycle++].func = FetchLowEffAddr;
                        instrTable[buildCycle++].func = FetchHighEffAddrY;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;
                    case opcodes.SHAiy: case opcodes.STAiy:
                        instrTable[buildCycle++].func = FetchLowPointer;
                        instrTable[buildCycle++].func = FetchLowEffAddr;
                        instrTable[buildCycle++].func = FetchHighEffAddrY;
                        instrTable[buildCycle++].func = throwAwayRead;
                        break;

                    default:
                        legalMode = false;
                        break;
                }

                if (access == AccessMode.READ)
                {
                    instrTable[buildCycle++].func = FetchEffAddrDataByte;
                }

                //---------------------------------------------------------------------------------------
                // Addressing Modes Finished, other cycles are instruction dependent
                //---------------------------------------------------------------------------------------
                switch (i)
                {
                    case opcodes.ADCz: case opcodes.ADCzx: case opcodes.ADCa: case opcodes.ADCax: case opcodes.ADCay: case opcodes.ADCix:
                    case opcodes.ADCiy: case opcodes.ADCb:
                        instrTable[buildCycle++].func = adc_instr;
                        break;

                    case opcodes.ANCb:
                    case 0x2B:
                        instrTable[buildCycle++].func = anc_instr;
                        break;

                    case opcodes.ANDz: case opcodes.ANDzx: case opcodes.ANDa: case opcodes.ANDax: case opcodes.ANDay: case opcodes.ANDix:
                    case opcodes.ANDiy: case opcodes.ANDb:
                        instrTable[buildCycle++].func = and_instr;
                        break;

                    case opcodes.ANEb: // Also known as XAA
                        instrTable[buildCycle++].func = ane_instr;
                        break;

                    case opcodes.ARRb:
                        instrTable[buildCycle++].func = arr_instr;
                        break;

                    case opcodes.ASLn:
                        instrTable[buildCycle++].func = asla_instr;
                        break;

                    case opcodes.ASLz: case opcodes.ASLzx: case opcodes.ASLa: case opcodes.ASLax:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = asl_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.ASRb: // Also known as ALR
                        instrTable[buildCycle++].func = alr_instr;
                        break;

                    case opcodes.BCCr:
                        instrTable[buildCycle++].func = bcc_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BCSr:
                        instrTable[buildCycle++].func = bcs_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BEQr:
                        instrTable[buildCycle++].func = beq_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BITz: case opcodes.BITa:
                        instrTable[buildCycle++].func = bit_instr;
                        break;

                    case opcodes.BMIr:
                        instrTable[buildCycle++].func = bmi_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BNEr:
                        instrTable[buildCycle++].func = bne_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BPLr:
                        instrTable[buildCycle++].func = bpl_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BRKn:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PushHighPC;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = brkPushLowPC;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = brk_instr;
                        instrTable[buildCycle++].func = IRQLoRequest;
                        instrTable[buildCycle++].func = IRQHiRequest;
                        instrTable[buildCycle++].func = fetchNextOpcode;
                        break;

                    case opcodes.BVCr:
                        instrTable[buildCycle++].func = bvc_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.BVSr:
                        instrTable[buildCycle++].func = bvs_instr;
                        instrTable[buildCycle++].func = fix_branch;
                        break;

                    case opcodes.CLCn:
                        instrTable[buildCycle++].func = clc_instr;
                        break;

                    case opcodes.CLDn:
                        instrTable[buildCycle++].func = cld_instr;
                        break;

                    case opcodes.CLIn:
                        instrTable[buildCycle++].func = cli_instr;
                        break;

                    case opcodes.CLVn:
                        instrTable[buildCycle++].func = clv_instr;
                        break;

                    case opcodes.CMPz: case opcodes.CMPzx: case opcodes.CMPa: case opcodes.CMPax: case opcodes.CMPay: case opcodes.CMPix:
                    case opcodes.CMPiy: case opcodes.CMPb:
                        instrTable[buildCycle++].func = cmp_instr;
                        break;

                    case opcodes.CPXz: case opcodes.CPXa: case opcodes.CPXb:
                        instrTable[buildCycle++].func = cpx_instr;
                        break;

                    case opcodes.CPYz: case opcodes.CPYa: case opcodes.CPYb:
                        instrTable[buildCycle++].func = cpy_instr;
                        break;

                    case opcodes.DCPz: case opcodes.DCPzx: case opcodes.DCPa: case opcodes.DCPax: case opcodes.DCPay: case opcodes.DCPix:
                    case opcodes.DCPiy: // Also known as DCM
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = dcm_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.DECz: case opcodes.DECzx: case opcodes.DECa: case opcodes.DECax:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = dec_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.DEXn:
                        instrTable[buildCycle++].func = dex_instr;
                        break;

                    case opcodes.DEYn:
                        instrTable[buildCycle++].func = dey_instr;
                        break;

                    case opcodes.EORz: case opcodes.EORzx: case opcodes.EORa: case opcodes.EORax: case opcodes.EORay: case opcodes.EORix:
                    case opcodes.EORiy: case opcodes.EORb:
                        instrTable[buildCycle++].func = eor_instr;
                        break;
#if false
        // HLT, also known as JAM
        case opcodes.0x02: case opcodes.0x12: case opcodes.0x22: case opcodes.0x32: case opcodes.0x42: case opcodes.0x52:
        case opcodes.0x62: case opcodes.0x72: case opcodes.0x92: case opcodes.0xb2: case opcodes.0xd2: case opcodes.0xf2:
        case opcodes.0x02: case opcodes.0x12: case opcodes.0x22: case opcodes.0x32: case opcodes.0x42: case opcodes.0x52:
        case opcodes.0x62: case opcodes.0x72: case opcodes.0x92: case opcodes.0xb2: case opcodes.0xd2: case opcodes.0xf2:
            instrTable[buildCycle++].func = hlt_instr;
            break;
#endif
                    case opcodes.INCz: case opcodes.INCzx: case opcodes.INCa: case opcodes.INCax:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = inc_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.INXn:
                        instrTable[buildCycle++].func = inx_instr;
                        break;

                    case opcodes.INYn:
                        instrTable[buildCycle++].func = iny_instr;
                        break;

                    case opcodes.ISBz: case opcodes.ISBzx: case opcodes.ISBa: case opcodes.ISBax: case opcodes.ISBay: case opcodes.ISBix:
                    case opcodes.ISBiy: // Also known as INS
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = ins_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.JSRw:
                        instrTable[buildCycle++].func = WasteCycle;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PushHighPC;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PushLowPC;
                        instrTable[buildCycle++].func = FetchHighAddr;
                        instrTable[buildCycle++].func = jmp_instr;
                        break;
                    case opcodes.JMPw: case opcodes.JMPi:
                        instrTable[buildCycle++].func = jmp_instr;
                        break;

                    case opcodes.LASay:
                        instrTable[buildCycle++].func = las_instr;
                        break;

                    case opcodes.LAXz: case opcodes.LAXzy: case opcodes.LAXa: case opcodes.LAXay: case opcodes.LAXix: case opcodes.LAXiy:
                        instrTable[buildCycle++].func = lax_instr;
                        break;

                    case opcodes.LDAz: case opcodes.LDAzx: case opcodes.LDAa: case opcodes.LDAax: case opcodes.LDAay: case opcodes.LDAix:
                    case opcodes.LDAiy: case opcodes.LDAb:
                        instrTable[buildCycle++].func = lda_instr;
                        break;

                    case opcodes.LDXz: case opcodes.LDXzy: case opcodes.LDXa: case opcodes.LDXay: case opcodes.LDXb:
                        instrTable[buildCycle++].func = ldx_instr;
                        break;

                    case opcodes.LDYz: case opcodes.LDYzx: case opcodes.LDYa: case opcodes.LDYax: case opcodes.LDYb:
                        instrTable[buildCycle++].func = ldy_instr;
                        break;

                    case opcodes.LSRn:
                        instrTable[buildCycle++].func = lsra_instr;
                        break;

                    case opcodes.LSRz: case opcodes.LSRzx: case opcodes.LSRa: case opcodes.LSRax:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = lsr_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.NOPn:
                    case 0x1A:
                    case 0x3A:
                    case 0x5A:
                    case 0x7A:
                    case 0xDA:
                    case 0xFA: case opcodes.NOPb:
                    case 0x82:
                    case 0xC2:
                    case 0xE2:
                    case 0x89:
                    case opcodes.NOPz:
                    case 0x44:
                    case 0x64: case opcodes.NOPzx:
                    case 0x34:
                    case 0x54:
                    case 0x74:
                    case 0xD4:
                    case 0xF4: case opcodes.NOPa: case opcodes.NOPax:
                    case 0x3C:
                    case 0x5C:
                    case 0x7C:
                    case 0xDC:
                    case 0xFC:
                        // NOPb NOPz NOPzx - Also known as SKBn
                        // NOPa NOPax      - Also known as SKWn
                        break;

                    case opcodes.LXAb: // Also known as OAL
                        instrTable[buildCycle++].func = oal_instr;
                        break;

                    case opcodes.ORAz: case opcodes.ORAzx: case opcodes.ORAa: case opcodes.ORAax: case opcodes.ORAay: case opcodes.ORAix:
                    case opcodes.ORAiy: case opcodes.ORAb:
                        instrTable[buildCycle++].func = ora_instr;
                        break;

                    case opcodes.PHAn:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = pha_instr;
                        break;

                    case opcodes.PHPn:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PushSR;
                        break;

                    case opcodes.PLAn:
                        // should read the value at current stack register.
                        // Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        instrTable[buildCycle++].func = pla_instr;
                        break;

                    case opcodes.PLPn:
                        // should read the value at current stack register.
                        // Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        instrTable[buildCycle++].func = PopSR;
                        instrTable[buildCycle++].func = plp_instr;
                        break;

                    case opcodes.RLAz: case opcodes.RLAzx: case opcodes.RLAix: case opcodes.RLAa: case opcodes.RLAax: case opcodes.RLAay:
                    case opcodes.RLAiy:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = rla_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.ROLn:
                        instrTable[buildCycle++].func = rola_instr;
                        break;

                    case opcodes.ROLz: case opcodes.ROLzx: case opcodes.ROLa: case opcodes.ROLax:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = rol_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.RORn:
                        instrTable[buildCycle++].func = rora_instr;
                        break;

                    case opcodes.RORz: case opcodes.RORzx: case opcodes.RORa: case opcodes.RORax:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = ror_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.RRAa: case opcodes.RRAax: case opcodes.RRAay: case opcodes.RRAz: case opcodes.RRAzx: case opcodes.RRAix:
                    case opcodes.RRAiy:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = rra_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.RTIn:
                        // should read the value at current stack register.
                        // Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        instrTable[buildCycle++].func = PopSR;
                        instrTable[buildCycle++].func = PopLowPC;
                        instrTable[buildCycle++].func = PopHighPC;
                        instrTable[buildCycle++].func = rti_instr;
                        break;

                    case opcodes.RTSn:
                        // should read the value at current stack register.
                        // Truly side-effect free.
                        instrTable[buildCycle++].func = WasteCycle;
                        instrTable[buildCycle++].func = PopLowPC;
                        instrTable[buildCycle++].func = PopHighPC;
                        instrTable[buildCycle++].func = rts_instr;
                        break;

                    case opcodes.SAXz: case opcodes.SAXzy: case opcodes.SAXa: case opcodes.SAXix: // Also known as AXS
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = axs_instr;
                        break;

                    case opcodes.SBCz: case opcodes.SBCzx: case opcodes.SBCa: case opcodes.SBCax: case opcodes.SBCay: case opcodes.SBCix:
                    case opcodes.SBCiy: case opcodes.SBCb:
                    case 0XEB:
                        instrTable[buildCycle++].func = sbc_instr;
                        break;

                    case opcodes.SBXb:
                        instrTable[buildCycle++].func = sbx_instr;
                        break;

                    case opcodes.SECn:
                        instrTable[buildCycle++].func = sec_instr;
                        break;

                    case opcodes.SEDn:
                        instrTable[buildCycle++].func = sed_instr;
                        break;

                    case opcodes.SEIn:
                        instrTable[buildCycle++].func = sei_instr;
                        break;

                    case opcodes.SHAay: case opcodes.SHAiy: // Also known as AXA
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = axa_instr;
                        break;

                    case opcodes.SHSay: // Also known as TAS
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = shs_instr;
                        break;

                    case opcodes.SHXay: // Also known as XAS
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = xas_instr;
                        break;

                    case opcodes.SHYax: // Also known as SAY
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = say_instr;
                        break;

                    case opcodes.SLOz: case opcodes.SLOzx: case opcodes.SLOa: case opcodes.SLOax: case opcodes.SLOay: case opcodes.SLOix:
                    case opcodes.SLOiy: // Also known as ASO
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = aso_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.SREz: case opcodes.SREzx: case opcodes.SREa: case opcodes.SREax: case opcodes.SREay: case opcodes.SREix:
                    case opcodes.SREiy: // Also known as LSE
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = lse_instr;
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = PutEffAddrDataByte;
                        break;

                    case opcodes.STAz: case opcodes.STAzx: case opcodes.STAa: case opcodes.STAax: case opcodes.STAay: case opcodes.STAix:
                    case opcodes.STAiy:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = sta_instr;
                        break;

                    case opcodes.STXz: case opcodes.STXzy: case opcodes.STXa:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = stx_instr;
                        break;

                    case opcodes.STYz: case opcodes.STYzx: case opcodes.STYa:
                        instrTable[buildCycle].nosteal = true;
                        instrTable[buildCycle++].func = sty_instr;
                        break;

                    case opcodes.TAXn:
                        instrTable[buildCycle++].func = tax_instr;
                        break;

                    case opcodes.TAYn:
                        instrTable[buildCycle++].func = tay_instr;
                        break;

                    case opcodes.TSXn:
                        instrTable[buildCycle++].func = tsx_instr;
                        break;

                    case opcodes.TXAn:
                        instrTable[buildCycle++].func = txa_instr;
                        break;

                    case opcodes.TXSn:
                        instrTable[buildCycle++].func = txs_instr;
                        break;

                    case opcodes.TYAn:
                        instrTable[buildCycle++].func = tya_instr;
                        break;

                    default:
                        legalInstr = false;
                        break;
                }

                // Missing an addressing mode or implementation makes opcode invalid.
                // These are normally called HLT instructions. In the hardware, the
                // CPU state machine locks up and will never recover.
                if (!(legalMode && legalInstr))
                {
                    instrTable[buildCycle++].func = invalidOpcode;
                }

                // check for IRQ triggers or fetch next opcode...
                instrTable[buildCycle].func = interruptsAndNextOpcode;

#if DEBUG
                //Console.Write("Done [{0} Cycles]\n", buildCycle - (i << 3));
#endif
            }
        }

        /**
         * Initialise CPU Emulation (Registers).
         */
        private void Initialise()
        {
            // Reset stack
            Register_StackPointer = 0xFF;

            // Reset Cycle Count
            cycleCount = (opcodes.BRKn << 3) + 6; // fetchNextOpcode

            // Reset Status Register
            flags.reset();

            // Set PC to some value
            Register_ProgramCounter = 0;

            // IRQs pending check
            irqAssertedOnPin = false;
            nmiFlag = false;
            rstFlag = false;
            interruptCycle = MAX;

            // Signals
            rdy = true;

            eventScheduler.schedule(m_nosteal, 0, event_phase_t.EVENT_CLOCK_PHI2);
        }

        /**
         * Reset CPU Emulation.
         */
        public void reset()
        {
            // Internal Stuff
            Initialise();

            // Set processor port to the default values
            cpuWrite(0, 0x2F);
            cpuWrite(1, 0x37);

            // Requires External Bits
            // Read from reset vector for program entry point
            sidendian.endian_16lo8(ref Cycle_EffectiveAddress, cpuRead(0xFFFC));
            sidendian.endian_16hi8(ref Cycle_EffectiveAddress, cpuRead(0xFFFD));
            Register_ProgramCounter = Cycle_EffectiveAddress;
        }

        /**
         * Module Credits.
         */
        public string credits()
        {
            return
                "MOS6510 Cycle Exact Emulation\n"
                + "\t(C) 2000 Simon A. White\n"
                + "\t(C) 2008-2010 Antti S. Lankila\n"
                + "\t(C) 2011-2017 Leandro Nini\n";
        }

        public void debug(bool enable, System.IO.FileStream out_)
        {
#if DEBUG
            dodump = enable;
            //if (!(out_ != null && enable))
                //m_fdbg = stdout;
            //else
                //m_fdbg = out_;
#endif
        }

    }

}
