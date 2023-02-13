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

namespace Driver.libsidplayfp.c64.CPU
{
    public partial class MOS6510 //mos6510debug
    {



        //# ifdef HAVE_CONFIG_H
        //# include "config.h"
        //#endif





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

        //# include "mos6510debug.h"

#if DEBUG

        //# include <cstdio>
        //# include <cstdlib>
        //# include "mos6510.h"
        //# include "sidendian.h"
        //# include "opcodes.h"

        public void DumpState(UInt64 time, MOS6510 cpu)
        {
#if false
            Console.Write(" PC  I  A  X  Y  SP  DR PR NV-BDIZC  Instruction ({0})\n", time);
            Console.Write("{0:x04} ", cpu.instrStartPC);
            Console.Write(cpu.irqAssertedOnPin ? "t " : "f ");
            Console.Write("{0:x02} ", cpu.Register_Accumulator);
            Console.Write("{0:x02} ", cpu.Register_X);
            Console.Write("{0:x02} ", cpu.Register_Y);
            Console.Write("01{0:x02} ", sidendian.endian_16lo8(cpu.Register_StackPointer));
            Console.Write("{0:x02} ", cpu.cpuRead(0));
            Console.Write("{0:x02} ", cpu.cpuRead(1));

            Console.Write(cpu.flags.getN() ? "1" : "0");
            Console.Write(cpu.flags.getV() ? "1" : "0");
            Console.Write("1");
            Console.Write(cpu.flags.getB() ? "1" : "0");
            Console.Write(cpu.flags.getD() ? "1" : "0");
            Console.Write(cpu.flags.getI() ? "1" : "0");
            Console.Write(cpu.flags.getZ() ? "1" : "0");
            Console.Write(cpu.flags.getC() ? "1" : "0");

            int opcode = cpu.cpuRead(cpu.instrStartPC);

            Console.Write("  {0:x02} ", opcode);

            switch (opcode)
            {
                // Accumulator or Implied cpu.Cycle_EffectiveAddressing
                case opcodes.ASLn:
                case opcodes.LSRn:
                case opcodes.ROLn:
                case opcodes.RORn:
                    Console.Write("      ");
                    break;
                // Zero Page Addressing Mode Handler
                case opcodes.ADCz:
                case opcodes.ANDz:
                case opcodes.ASLz:
                case opcodes.BITz:
                case opcodes.CMPz:
                case opcodes.CPXz:
                case opcodes.CPYz:
                case opcodes.DCPz:
                case opcodes.DECz:
                case opcodes.EORz:
                case opcodes.INCz:
                case opcodes.ISBz:
                case opcodes.LAXz:
                case opcodes.LDAz:
                case opcodes.LDXz:
                case opcodes.LDYz:
                case opcodes.LSRz:
                case opcodes.NOPz:
                case 0x44:
                case 0x64:
                case opcodes.ORAz:
                case opcodes.ROLz:
                case opcodes.RORz:
                case opcodes.SAXz:
                case opcodes.SBCz:
                case opcodes.SREz:
                case opcodes.STAz:
                case opcodes.STXz:
                case opcodes.STYz:
                case opcodes.SLOz:
                case opcodes.RLAz:
                case opcodes.RRAz:
                    // ASOz AXSz DCMz INSz LSEz - Optional Opcode Names
                    Console.Write("{0:x02}    ", sidendian.endian_16lo8(cpu.instrOperand));
                    break;
                // Zero Page with X Offset Addressing Mode Handler
                case opcodes.ADCzx:
                case opcodes.ANDzx:
                case opcodes.ASLzx:
                case opcodes.CMPzx:
                case opcodes.DCPzx:
                case opcodes.DECzx:
                case opcodes.EORzx:
                case opcodes.INCzx:
                case opcodes.ISBzx:
                case opcodes.LDAzx:
                case opcodes.LDYzx:
                case opcodes.LSRzx:
                case opcodes.NOPzx:
                case 0x34:
                case 0x54:
                case 0x74:
                case 0xD4:
                case 0xF4:
                case opcodes.ORAzx:
                case opcodes.RLAzx:
                case opcodes.ROLzx:
                case opcodes.RORzx:
                case opcodes.RRAzx:
                case opcodes.SBCzx:
                case opcodes.SLOzx:
                case opcodes.SREzx:
                case opcodes.STAzx:
                case opcodes.STYzx:
                    // ASOzx DCMzx INSzx LSEzx - Optional Opcode Names
                    Console.Write("{0:x02}    ", sidendian.endian_16lo8(cpu.instrOperand));
                    break;
                // Zero Page with Y Offset Addressing Mode Handler
                case opcodes.LDXzy:
                case opcodes.STXzy:
                case opcodes.SAXzy:
                case opcodes.LAXzy:
                    // AXSzx - Optional Opcode Names
                    Console.Write("{0:x02}    ", sidendian.endian_16lo8(cpu.instrOperand));
                    break;
                // Absolute Addressing Mode Handler
                case opcodes.ADCa:
                case opcodes.ANDa:
                case opcodes.ASLa:
                case opcodes.BITa:
                case opcodes.CMPa:
                case opcodes.CPXa:
                case opcodes.CPYa:
                case opcodes.DCPa:
                case opcodes.DECa:
                case opcodes.EORa:
                case opcodes.INCa:
                case opcodes.ISBa:
                case opcodes.JMPw:
                case opcodes.JSRw:
                case opcodes.LAXa:
                case opcodes.LDAa:
                case opcodes.LDXa:
                case opcodes.LDYa:
                case opcodes.LSRa:
                case opcodes.NOPa:
                case opcodes.ORAa:
                case opcodes.ROLa:
                case opcodes.RORa:
                case opcodes.SAXa:
                case opcodes.SBCa:
                case opcodes.SLOa:
                case opcodes.SREa:
                case opcodes.STAa:
                case opcodes.STXa:
                case opcodes.STYa:
                case opcodes.RLAa:
                case opcodes.RRAa:
                    // ASOa AXSa DCMa INSa LSEa - Optional Opcode Names
                    Console.Write("{0:x02} {1:x02} ", sidendian.endian_16lo8(cpu.instrOperand), sidendian.endian_16hi8(cpu.instrOperand));
                    break;
                // Absolute With X Offset Addresing Mode Handler
                case opcodes.ADCax:
                case opcodes.ANDax:
                case opcodes.ASLax:
                case opcodes.CMPax:
                case opcodes.DCPax:
                case opcodes.DECax:
                case opcodes.EORax:
                case opcodes.INCax:
                case opcodes.ISBax:
                case opcodes.LDAax:
                case opcodes.LDYax:
                case opcodes.LSRax:
                case opcodes.NOPax:
                case 0x3C:
                case 0x5C:
                case 0x7C:
                case 0xDC:
                case 0xFC:
                case opcodes.ORAax:
                case opcodes.RLAax:
                case opcodes.ROLax:
                case opcodes.RORax:
                case opcodes.RRAax:
                case opcodes.SBCax:
                case opcodes.SHYax:
                case opcodes.SLOax:
                case opcodes.SREax:
                case opcodes.STAax:
                    // ASOax DCMax INSax LSEax SAYax - Optional Opcode Names
                    Console.Write("{0:x02} {1:x02} ", sidendian.endian_16lo8(cpu.instrOperand), sidendian.endian_16hi8(cpu.instrOperand));
                    break;
                // Absolute With Y Offset Addresing Mode Handler
                case opcodes.ADCay:
                case opcodes.ANDay:
                case opcodes.CMPay:
                case opcodes.DCPay:
                case opcodes.EORay:
                case opcodes.ISBay:
                case opcodes.LASay:
                case opcodes.LAXay:
                case opcodes.LDAay:
                case opcodes.LDXay:
                case opcodes.ORAay:
                case opcodes.RLAay:
                case opcodes.RRAay:
                case opcodes.SBCay:
                case opcodes.SHAay:
                case opcodes.SHSay:
                case opcodes.SHXay:
                case opcodes.SLOay:
                case opcodes.SREay:
                case opcodes.STAay:
                    // ASOay AXAay DCMay INSax LSEay TASay XASay - Optional Opcode Names
                    Console.Write("{0:x02} {1:x02} ", sidendian.endian_16lo8(cpu.instrOperand), sidendian.endian_16hi8(cpu.instrOperand));
                    break;
                // Immediate and Relative Addressing Mode Handler
                case opcodes.ADCb:
                case opcodes.ANDb:
                case opcodes.ANCb:
                case 0x2B:
                case opcodes.ANEb:
                case opcodes.ASRb:
                case opcodes.ARRb:
                case opcodes.BCCr:
                case opcodes.BCSr:
                case opcodes.BEQr:
                case opcodes.BMIr:
                case opcodes.BNEr:
                case opcodes.BPLr:
                case opcodes.BVCr:
                case opcodes.BVSr:
                case opcodes.CMPb:
                case opcodes.CPXb:
                case opcodes.CPYb:
                case opcodes.EORb:
                case opcodes.LDAb:
                case opcodes.LDXb:
                case opcodes.LDYb:
                case opcodes.LXAb:
                case opcodes.NOPb:
                case 0x82:
                case 0xC2:
                case 0xE2:
                case 0x89:
                case opcodes.ORAb:
                case opcodes.SBCb:
                case 0XEB:
                case opcodes.SBXb:
                    // OALb ALRb XAAb - Optional Opcode Names
                    Console.Write("{0:x02}    ", sidendian.endian_16lo8(cpu.Cycle_Data));
                    break;
                // Indirect Addressing Mode Handler
                case opcodes.JMPi:
                    Console.Write("{0:x02} {1:x02} ", sidendian.endian_16lo8(cpu.instrOperand), sidendian.endian_16hi8(cpu.instrOperand));
                    break;
                // Indexed with X Preinc Addressing Mode Handler
                case opcodes.ADCix:
                case opcodes.ANDix:
                case opcodes.CMPix:
                case opcodes.DCPix:
                case opcodes.EORix:
                case opcodes.ISBix:
                case opcodes.LAXix:
                case opcodes.LDAix:
                case opcodes.ORAix:
                case opcodes.SAXix:
                case opcodes.SBCix:
                case opcodes.SLOix:
                case opcodes.SREix:
                case opcodes.STAix:
                case opcodes.RLAix:
                case opcodes.RRAix:
                    // ASOix AXSix DCMix INSix LSEix - Optional Opcode Names
                    Console.Write("{0:x02}    ", sidendian.endian_16lo8(cpu.instrOperand));
                    break;
                // Indexed with Y Postinc Addressing Mode Handler
                case opcodes.ADCiy:
                case opcodes.ANDiy:
                case opcodes.CMPiy:
                case opcodes.DCPiy:
                case opcodes.EORiy:
                case opcodes.ISBiy:
                case opcodes.LAXiy:
                case opcodes.LDAiy:
                case opcodes.ORAiy:
                case opcodes.RLAiy:
                case opcodes.RRAiy:
                case opcodes.SBCiy:
                case opcodes.SHAiy:
                case opcodes.SLOiy:
                case opcodes.SREiy:
                case opcodes.STAiy:
                    // AXAiy ASOiy LSEiy DCMiy INSiy - Optional Opcode Names
                    Console.Write("{0:x02}    ", sidendian.endian_16lo8(cpu.instrOperand));
                    break;
                default:
                    Console.Write("      ");
                    break;
            }

            switch (opcode)
            {
                case opcodes.ADCb:
                case opcodes.ADCz:
                case opcodes.ADCzx:
                case opcodes.ADCa:
                case opcodes.ADCax:
                case opcodes.ADCay:
                case opcodes.ADCix:
                case opcodes.ADCiy:
                    Console.Write(" ADC"); break;
                case opcodes.ANCb:
                case 0x2B:
                    Console.Write("*ANC"); break;
                case opcodes.ANDb:
                case opcodes.ANDz:
                case opcodes.ANDzx:
                case opcodes.ANDa:
                case opcodes.ANDax:
                case opcodes.ANDay:
                case opcodes.ANDix:
                case opcodes.ANDiy:
                    Console.Write(" AND"); break;
                case opcodes.ANEb: // Also known as XAA
                    Console.Write("*ANE"); break;
                case opcodes.ARRb:
                    Console.Write("*ARR"); break;
                case opcodes.ASLn:
                case opcodes.ASLz:
                case opcodes.ASLzx:
                case opcodes.ASLa:
                case opcodes.ASLax:
                    Console.Write(" ASL"); break;
                case opcodes.ASRb: // Also known as ALR
                    Console.Write("*ASR"); break;
                case opcodes.BCCr:
                    Console.Write(" BCC"); break;
                case opcodes.BCSr:
                    Console.Write(" BCS"); break;
                case opcodes.BEQr:
                    Console.Write(" BEQ"); break;
                case opcodes.BITz:
                case opcodes.BITa:
                    Console.Write(" BIT"); break;
                case opcodes.BMIr:
                    Console.Write(" BMI"); break;
                case opcodes.BNEr:
                    Console.Write(" BNE"); break;
                case opcodes.BPLr:
                    Console.Write(" BPL"); break;
                case opcodes.BRKn:
                    Console.Write(" BRK"); break;
                case opcodes.BVCr:
                    Console.Write(" BVC"); break;
                case opcodes.BVSr:
                    Console.Write(" BVS"); break;
                case opcodes.CLCn:
                    Console.Write(" CLC"); break;
                case opcodes.CLDn:
                    Console.Write(" CLD"); break;
                case opcodes.CLIn:
                    Console.Write(" CLI"); break;
                case opcodes.CLVn:
                    Console.Write(" CLV"); break;
                case opcodes.CMPb:
                case opcodes.CMPz:
                case opcodes.CMPzx:
                case opcodes.CMPa:
                case opcodes.CMPax:
                case opcodes.CMPay:
                case opcodes.CMPix:
                case opcodes.CMPiy:
                    Console.Write(" CMP"); break;
                case opcodes.CPXb:
                case opcodes.CPXz:
                case opcodes.CPXa:
                    Console.Write(" CPX"); break;
                case opcodes.CPYb:
                case opcodes.CPYz:
                case opcodes.CPYa:
                    Console.Write(" CPY"); break;
                case opcodes.DCPz:
                case opcodes.DCPzx:
                case opcodes.DCPa:
                case opcodes.DCPax:
                case opcodes.DCPay:
                case opcodes.DCPix:
                case opcodes.DCPiy: // Also known as DCM
                    Console.Write("*DCP"); break;
                case opcodes.DECz:
                case opcodes.DECzx:
                case opcodes.DECa:
                case opcodes.DECax:
                    Console.Write(" DEC"); break;
                case opcodes.DEXn:
                    Console.Write(" DEX"); break;
                case opcodes.DEYn:
                    Console.Write(" DEY"); break;
                case opcodes.EORb:
                case opcodes.EORz:
                case opcodes.EORzx:
                case opcodes.EORa:
                case opcodes.EORax:
                case opcodes.EORay:
                case opcodes.EORix:
                case opcodes.EORiy:
                    Console.Write(" EOR"); break;
                case opcodes.INCz:
                case opcodes.INCzx:
                case opcodes.INCa:
                case opcodes.INCax:
                    Console.Write(" INC"); break;
                case opcodes.INXn:
                    Console.Write(" INX"); break;
                case opcodes.INYn:
                    Console.Write(" INY"); break;
                case opcodes.ISBz:
                case opcodes.ISBzx:
                case opcodes.ISBa:
                case opcodes.ISBax:
                case opcodes.ISBay:
                case opcodes.ISBix:
                case opcodes.ISBiy: // Also known as INS
                    Console.Write("*ISB"); break;
                case opcodes.JMPw:
                case opcodes.JMPi:
                    Console.Write(" JMP"); break;
                case opcodes.JSRw:
                    Console.Write(" JSR"); break;
                case opcodes.LASay:
                    Console.Write("*LAS"); break;
                case opcodes.LAXz:
                case opcodes.LAXzy:
                case opcodes.LAXa:
                case opcodes.LAXay:
                case opcodes.LAXix:
                case opcodes.LAXiy:
                    Console.Write("*LAX"); break;
                case opcodes.LDAb:
                case opcodes.LDAz:
                case opcodes.LDAzx:
                case opcodes.LDAa:
                case opcodes.LDAax:
                case opcodes.LDAay:
                case opcodes.LDAix:
                case opcodes.LDAiy:
                    Console.Write(" LDA"); break;
                case opcodes.LDXb:
                case opcodes.LDXz:
                case opcodes.LDXzy:
                case opcodes.LDXa:
                case opcodes.LDXay:
                    Console.Write(" LDX"); break;
                case opcodes.LDYb:
                case opcodes.LDYz:
                case opcodes.LDYzx:
                case opcodes.LDYa:
                case opcodes.LDYax:
                    Console.Write(" LDY"); break;
                case opcodes.LSRz:
                case opcodes.LSRzx:
                case opcodes.LSRa:
                case opcodes.LSRax:
                case opcodes.LSRn:
                    Console.Write(" LSR"); break;
                case opcodes.NOPn:
                case 0x1A:
                case 0x3A:
                case 0x5A:
                case 0x7A:
                case 0xDA:
                case 0xFA:
                case opcodes.NOPb:
                case 0x82:
                case 0xC2:
                case 0xE2:
                case 0x89:
                case opcodes.NOPz:
                case 0x44:
                case 0x64:
                case opcodes.NOPzx:
                case 0x34:
                case 0x54:
                case 0x74:
                case 0xD4:
                case 0xF4:
                case opcodes.NOPa:
                case opcodes.NOPax:
                case 0x3C:
                case 0x5C:
                case 0x7C:
                case 0xDC:
                case 0xFC:
                    if (opcode != opcodes.NOPn) Console.Write("*");
                    else Console.Write(" ");
                    Console.Write("NOP"); break;
                case opcodes.LXAb: // Also known as OAL
                    Console.Write("*LXA"); break;
                case opcodes.ORAb:
                case opcodes.ORAz:
                case opcodes.ORAzx:
                case opcodes.ORAa:
                case opcodes.ORAax:
                case opcodes.ORAay:
                case opcodes.ORAix:
                case opcodes.ORAiy:
                    Console.Write(" ORA"); break;
                case opcodes.PHAn:
                    Console.Write(" PHA"); break;
                case opcodes.PHPn:
                    Console.Write(" PHP"); break;
                case opcodes.PLAn:
                    Console.Write(" PLA"); break;
                case opcodes.PLPn:
                    Console.Write(" PLP"); break;
                case opcodes.RLAz:
                case opcodes.RLAzx:
                case opcodes.RLAix:
                case opcodes.RLAa:
                case opcodes.RLAax:
                case opcodes.RLAay:
                case opcodes.RLAiy:
                    Console.Write("*RLA"); break;
                case opcodes.ROLz:
                case opcodes.ROLzx:
                case opcodes.ROLa:
                case opcodes.ROLax:
                case opcodes.ROLn:
                    Console.Write(" ROL"); break;
                case opcodes.RORz:
                case opcodes.RORzx:
                case opcodes.RORa:
                case opcodes.RORax:
                case opcodes.RORn:
                    Console.Write(" ROR"); break;
                case opcodes.RRAa:
                case opcodes.RRAax:
                case opcodes.RRAay:
                case opcodes.RRAz:
                case opcodes.RRAzx:
                case opcodes.RRAix:
                case opcodes.RRAiy:
                    Console.Write("*RRA"); break;
                case opcodes.RTIn:
                    Console.Write(" RTI"); break;
                case opcodes.RTSn:
                    Console.Write(" RTS"); break;
                case opcodes.SAXz:
                case opcodes.SAXzy:
                case opcodes.SAXa:
                case opcodes.SAXix: // Also known as AXS
                    Console.Write("*SAX"); break;
                case opcodes.SBCb:
                case 0XEB:
                    if (opcode !=opcodes.SBCb) Console.Write("*");
                    else Console.Write(" ");
                    Console.Write("SBC"); break;
                case opcodes.SBCz:
                case opcodes.SBCzx:
                case opcodes.SBCa:
                case opcodes.SBCax:
                case opcodes.SBCay:
                case opcodes.SBCix:
                case opcodes.SBCiy:
                    Console.Write(" SBC"); break;
                case opcodes.SBXb:
                    Console.Write("*SBX"); break;
                case opcodes.SECn:
                    Console.Write(" SEC"); break;
                case opcodes.SEDn:
                    Console.Write(" SED"); break;
                case opcodes.SEIn:
                    Console.Write(" SEI"); break;
                case opcodes.SHAay:
                case opcodes.SHAiy: // Also known as AXA
                    Console.Write("*SHA"); break;
                case opcodes.SHSay: // Also known as TAS
                    Console.Write("*SHS"); break;
                case opcodes.SHXay: // Also known as XAS
                    Console.Write("*SHX"); break;
                case opcodes.SHYax: // Also known as SAY
                    Console.Write("*SHY"); break;
                case opcodes.SLOz:
                case opcodes.SLOzx:
                case opcodes.SLOa:
                case opcodes.SLOax:
                case opcodes.SLOay:
                case opcodes.SLOix:
                case opcodes.SLOiy: // Also known as ASO
                    Console.Write("*SLO"); break;
                case opcodes.SREz:
                case opcodes.SREzx:
                case opcodes.SREa:
                case opcodes.SREax:
                case opcodes.SREay:
                case opcodes.SREix:
                case opcodes.SREiy: // Also known as LSE
                    Console.Write("*SRE"); break;
                case opcodes.STAz:
                case opcodes.STAzx:
                case opcodes.STAa:
                case opcodes.STAax:
                case opcodes.STAay:
                case opcodes.STAix:
                case opcodes.STAiy:
                    Console.Write(" STA"); break;
                case opcodes.STXz:
                case opcodes.STXzy:
                case opcodes.STXa:
                    Console.Write(" STX"); break;
                case opcodes.STYz:
                case opcodes.STYzx:
                case opcodes.STYa:
                    Console.Write(" STY"); break;
                case opcodes.TAXn:
                    Console.Write(" TAX"); break;
                case opcodes.TAYn:
                    Console.Write(" TAY"); break;
                case opcodes.TSXn:
                    Console.Write(" TSX"); break;
                case opcodes.TXAn:
                    Console.Write(" TXA"); break;
                case opcodes.TXSn:
                    Console.Write(" TXS"); break;
                case opcodes.TYAn:
                    Console.Write(" TYA"); break;
                default:
                    Console.Write("*HLT"); break;
            }

            switch (opcode)
            {
                // Accumulator or Implied cpu.Cycle_EffectiveAddressing
                case opcodes.ASLn:
                case opcodes.LSRn:
                case opcodes.ROLn:
                case opcodes.RORn:
                    Console.Write("n  A");
                    break;

                // Zero Page Addressing Mode Handler
                case opcodes.ADCz:
                case opcodes.ANDz:
                case opcodes.ASLz:
                case opcodes.BITz:
                case opcodes.CMPz:
                case opcodes.CPXz:
                case opcodes.CPYz:
                case opcodes.DCPz:
                case opcodes.DECz:
                case opcodes.EORz:
                case opcodes.INCz:
                case opcodes.ISBz:
                case opcodes.LAXz:
                case opcodes.LDAz:
                case opcodes.LDXz:
                case opcodes.LDYz:
                case opcodes.LSRz:
                case opcodes.ORAz:

                case opcodes.ROLz:
                case opcodes.RORz:
                case opcodes.SBCz:
                case opcodes.SREz:
                case opcodes.SLOz:
                case opcodes.RLAz:
                case opcodes.RRAz:
                    // ASOz AXSz DCMz INSz LSEz - Optional Opcode Names
                    Console.Write("z  {0:x02} {2}{1:x02}{3}", sidendian.endian_16lo8(cpu.instrOperand), cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.SAXz:
                case opcodes.STAz:
                case opcodes.STXz:
                case opcodes.STYz:
                case opcodes.NOPz:
                case 0x44:
                case 0x64:
                    Console.Write("z  {0:x02}", sidendian.endian_16lo8(cpu.instrOperand));
                    break;

                // Zero Page with X Offset Addressing Mode Handler
                case opcodes.ADCzx:
                case opcodes.ANDzx:
                case opcodes.ASLzx:
                case opcodes.CMPzx:
                case opcodes.DCPzx:
                case opcodes.DECzx:
                case opcodes.EORzx:
                case opcodes.INCzx:
                case opcodes.ISBzx:
                case opcodes.LDAzx:
                case opcodes.LDYzx:
                case opcodes.LSRzx:
                case opcodes.ORAzx:
                case opcodes.RLAzx:
                case opcodes.ROLzx:
                case opcodes.RORzx:
                case opcodes.RRAzx:
                case opcodes.SBCzx:
                case opcodes.SLOzx:
                case opcodes.SREzx:
                    // ASOzx DCMzx INSzx LSEzx - Optional Opcode Names
                    Console.Write("zx {0:x02},X", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]{2}{1:x02}{3}", cpu.Cycle_EffectiveAddress, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.STAzx:
                case opcodes.STYzx:
                case opcodes.NOPzx:
                case 0x34:
                case 0x54:
                case 0x74:
                case 0xD4:
                case 0xF4:
                    Console.Write("zx {0:x02},X", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Zero Page with Y Offset Addressing Mode Handler
                case opcodes.LAXzy:
                case opcodes.LDXzy:
                    // AXSzx - Optional Opcode Names
                    Console.Write("zy {0:x02},Y", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]{2}{1:x02}{3}", cpu.Cycle_EffectiveAddress, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.STXzy:
                case opcodes.SAXzy:
                    Console.Write("zy {0:x02},Y", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Absolute Addressing Mode Handler
                case opcodes.ADCa:
                case opcodes.ANDa:
                case opcodes.ASLa:
                case opcodes.BITa:
                case opcodes.CMPa:
                case opcodes.CPXa:
                case opcodes.CPYa:
                case opcodes.DCPa:
                case opcodes.DECa:
                case opcodes.EORa:
                case opcodes.INCa:
                case opcodes.ISBa:
                case opcodes.LAXa:
                case opcodes.LDAa:
                case opcodes.LDXa:
                case opcodes.LDYa:
                case opcodes.LSRa:
                case opcodes.ORAa:
                case opcodes.ROLa:
                case opcodes.RORa:
                case opcodes.SBCa:
                case opcodes.SLOa:
                case opcodes.SREa:
                case opcodes.RLAa:
                case opcodes.RRAa:
                    // ASOa AXSa DCMa INSa LSEa - Optional Opcode Names
                    Console.Write("a  {0:x04} {2}{1:x02}{3}", cpu.instrOperand, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.SAXa:
                case opcodes.STAa:
                case opcodes.STXa:
                case opcodes.STYa:
                case opcodes.NOPa:
                    Console.Write("a  {0:x04}", cpu.instrOperand);
                    break;
                case opcodes.JMPw:
                case opcodes.JSRw:
                    Console.Write("w  {0:x04}", cpu.instrOperand);
                    break;

                // Absolute With X Offset Addresing Mode Handler
                case opcodes.ADCax:
                case opcodes.ANDax:
                case opcodes.ASLax:
                case opcodes.CMPax:
                case opcodes.DCPax:
                case opcodes.DECax:
                case opcodes.EORax:
                case opcodes.INCax:
                case opcodes.ISBax:
                case opcodes.LDAax:
                case opcodes.LDYax:
                case opcodes.LSRax:
                case opcodes.ORAax:
                case opcodes.RLAax:
                case opcodes.ROLax:
                case opcodes.RORax:
                case opcodes.RRAax:
                case opcodes.SBCax:
                case opcodes.SLOax:
                case opcodes.SREax:
                    // ASOax DCMax INSax LSEax SAYax - Optional Opcode Names
                    Console.Write("ax {0:x04},X", cpu.instrOperand);
                    Console.Write(" [{0:x04}]{2}{1:x02}{3}", cpu.Cycle_EffectiveAddress, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.SHYax:
                case opcodes.STAax:
                case opcodes.NOPax:
                case 0x3C:
                case 0x5C:
                case 0x7C:
                case 0xDC:
                case 0xFC:
                    Console.Write("ax {0:x04},X", cpu.instrOperand);
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Absolute With Y Offset Addresing Mode Handler
                case opcodes.ADCay:
                case opcodes.ANDay:
                case opcodes.CMPay:
                case opcodes.DCPay:
                case opcodes.EORay:
                case opcodes.ISBay:
                case opcodes.LASay:
                case opcodes.LAXay:
                case opcodes.LDAay:
                case opcodes.LDXay:
                case opcodes.ORAay:
                case opcodes.RLAay:
                case opcodes.RRAay:
                case opcodes.SBCay:
                case opcodes.SHSay:
                case opcodes.SLOay:
                case opcodes.SREay:
                    // ASOay AXAay DCMay INSax LSEay TASay XASay - Optional Opcode Names
                    Console.Write("ay {0:x04},Y", cpu.instrOperand);
                    Console.Write(" [{0:x04}]{2}{1:x02}{3}", cpu.Cycle_EffectiveAddress, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.SHAay:
                case opcodes.SHXay:
                case opcodes.STAay:
                    Console.Write("ay {0:x04},Y", cpu.instrOperand);
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Immediate Addressing Mode Handler
                case opcodes.ADCb:
                case opcodes.ANDb:
                case opcodes.ANCb:
                case 0x2B:
                case opcodes.ANEb:
                case opcodes.ASRb:
                case opcodes.ARRb:
                case opcodes.CMPb:
                case opcodes.CPXb:
                case opcodes.CPYb:
                case opcodes.EORb:
                case opcodes.LDAb:
                case opcodes.LDXb:
                case opcodes.LDYb:
                case opcodes.LXAb:
                case opcodes.ORAb:
                case opcodes.SBCb:
                case 0XEB:
                case opcodes.SBXb:
                // OALb ALRb XAAb - Optional Opcode Names
                case opcodes.NOPb:
                case 0x82:
                case 0xC2:
                case 0xE2:
                case 0x89:
                    Console.Write("b  #{0:x02}", sidendian.endian_16lo8(cpu.instrOperand));
                    break;

                // Relative Addressing Mode Handler
                case opcodes.BCCr:
                case opcodes.BCSr:
                case opcodes.BEQr:
                case opcodes.BMIr:
                case opcodes.BNEr:
                case opcodes.BPLr:
                case opcodes.BVCr:
                case opcodes.BVSr:
                    Console.Write("r  #{0:x02}", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Indirect Addressing Mode Handler
                case opcodes.JMPi:
                    Console.Write("i  ({0:x04})", cpu.instrOperand);
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Indexed with X Preinc Addressing Mode Handler
                case opcodes.ADCix:
                case opcodes.ANDix:
                case opcodes.CMPix:
                case opcodes.DCPix:
                case opcodes.EORix:
                case opcodes.ISBix:
                case opcodes.LAXix:
                case opcodes.LDAix:
                case opcodes.ORAix:
                case opcodes.SBCix:
                case opcodes.SLOix:
                case opcodes.SREix:
                case opcodes.RLAix:
                case opcodes.RRAix:
                    // ASOix AXSix DCMix INSix LSEix - Optional Opcode Names
                    Console.Write("ix ({0:x02},X)", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]{2}{1:x02}{3}", cpu.Cycle_EffectiveAddress, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.SAXix:
                case opcodes.STAix:
                    Console.Write("ix ({0:x02},X)", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                // Indexed with Y Postinc Addressing Mode Handler
                case opcodes.ADCiy:
                case opcodes.ANDiy:
                case opcodes.CMPiy:
                case opcodes.DCPiy:
                case opcodes.EORiy:
                case opcodes.ISBiy:
                case opcodes.LAXiy:
                case opcodes.LDAiy:
                case opcodes.ORAiy:
                case opcodes.RLAiy:
                case opcodes.RRAiy:
                case opcodes.SBCiy:
                case opcodes.SLOiy:
                case opcodes.SREiy:
                    // AXAiy ASOiy LSEiy DCMiy INSiy - Optional Opcode Names
                    Console.Write("iy ({0:x02}),Y", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]{2}{1:x02}{3}", cpu.Cycle_EffectiveAddress, cpu.Cycle_Data, "{", "}");
                    break;
                case opcodes.SHAiy:
                case opcodes.STAiy:
                    Console.Write("iy ({0:x02}),Y", sidendian.endian_16lo8(cpu.instrOperand));
                    Console.Write(" [{0:x04}]", cpu.Cycle_EffectiveAddress);
                    break;

                default:
                    break;
            }

            Console.Write("\n\n");
            //fflush(cpu.m_fdbg);
#endif
        }

#endif

    }


}



