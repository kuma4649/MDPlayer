/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2013 Leandro Nini <drfiemost@users.sourceforge.net>
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
    public class opcodes
    {



        public const int OPCODE_MAX = 0x100;

        /* HLT
            case 0x02: case 0x12: case 0x22: case 0x32: case 0x42: case 0x52:
            case 0x62: case 0x72: case 0x92: case 0xb2: case 0xd2: case 0xf2:
            case 0x02: case 0x12: case 0x22: case 0x32: case 0x42: case 0x52:
            case 0x62: case 0x72: case 0x92: case 0xb2: case 0xd2: case 0xf2:
        */

        public const int BRKn = 0x00;
        public const int JSRw = 0x20;
        public const int RTIn = 0x40;
        public const int RTSn = 0x60;
        public const int NOPb = 0x80;
        //public const int  NOPb_ NOPb: case 0x82: case 0xC2: case 0xE2: case 0x89
        public const int LDYb = 0xA0;
        public const int CPYb = 0xC0;
        public const int CPXb = 0xE0;

        public const int ORAix = 0x01;
        public const int ANDix = 0x21;
        public const int EORix = 0x41;
        public const int ADCix = 0x61;
        public const int STAix = 0x81;
        public const int LDAix = 0xA1;
        public const int CMPix = 0xC1;
        public const int SBCix = 0xE1;

        public const int LDXb = 0xA2;

        public const int SLOix = 0x03;
        public const int RLAix = 0x23;
        public const int SREix = 0x43;
        public const int RRAix = 0x63;
        public const int SAXix = 0x83;
        public const int LAXix = 0xA3;
        public const int DCPix = 0xC3;
        public const int ISBix = 0xE3;

        public const int NOPz = 0x04;
        //public const int  NOPz_ NOPz: case 0x44: case 0x64
        public const int BITz = 0x24;
        public const int STYz = 0x84;
        public const int LDYz = 0xA4;
        public const int CPYz = 0xC4;
        public const int CPXz = 0xE4;

        public const int ORAz = 0x05;
        public const int ANDz = 0x25;
        public const int EORz = 0x45;
        public const int ADCz = 0x65;
        public const int STAz = 0x85;
        public const int LDAz = 0xA5;
        public const int CMPz = 0xC5;
        public const int SBCz = 0xE5;

        public const int ASLz = 0x06;
        public const int ROLz = 0x26;
        public const int LSRz = 0x46;
        public const int RORz = 0x66;
        public const int STXz = 0x86;
        public const int LDXz = 0xA6;
        public const int DECz = 0xC6;
        public const int INCz = 0xE6;

        public const int SLOz = 0x07;
        public const int RLAz = 0x27;
        public const int SREz = 0x47;
        public const int RRAz = 0x67;
        public const int SAXz = 0x87;
        public const int LAXz = 0xA7;
        public const int DCPz = 0xC7;
        public const int ISBz = 0xE7;

        public const int PHPn = 0x08;
        public const int PLPn = 0x28;
        public const int PHAn = 0x48;
        public const int PLAn = 0x68;
        public const int DEYn = 0x88;
        public const int TAYn = 0xA8;
        public const int INYn = 0xC8;
        public const int INXn = 0xE8;

        public const int ORAb = 0x09;
        public const int ANDb = 0x29;
        public const int EORb = 0x49;
        public const int ADCb = 0x69;
        public const int LDAb = 0xA9;
        public const int CMPb = 0xC9;
        public const int SBCb = 0xE9;
        //public const int  SBCb_ SBCb: case 0XEB

        public const int ASLn = 0x0A;
        public const int ROLn = 0x2A;
        public const int LSRn = 0x4A;
        public const int RORn = 0x6A;
        public const int TXAn = 0x8A;
        public const int TAXn = 0xAA;
        public const int DEXn = 0xCA;
        public const int NOPn = 0xEA;
        //public const int  NOPn_ NOPn: case 0x1A: case 0x3A: case 0x5A: case 0x7A: case 0xDA: case 0xFA

        public const int ANCb = 0x0B;
        //public const int  ANCb_ ANCb: case 0x2B
        public const int ASRb = 0x4B;
        public const int ARRb = 0x6B;
        public const int ANEb = 0x8B;
        public const int XAAb = 0x8B;
        public const int LXAb = 0xAB;
        public const int SBXb = 0xCB;

        public const int NOPa = 0x0C;
        public const int BITa = 0x2C;
        public const int JMPw = 0x4C;
        public const int JMPi = 0x6C;
        public const int STYa = 0x8C;
        public const int LDYa = 0xAC;
        public const int CPYa = 0xCC;
        public const int CPXa = 0xEC;

        public const int ORAa = 0x0D;
        public const int ANDa = 0x2D;
        public const int EORa = 0x4D;
        public const int ADCa = 0x6D;
        public const int STAa = 0x8D;
        public const int LDAa = 0xAD;
        public const int CMPa = 0xCD;
        public const int SBCa = 0xED;

        public const int ASLa = 0x0E;
        public const int ROLa = 0x2E;
        public const int LSRa = 0x4E;
        public const int RORa = 0x6E;
        public const int STXa = 0x8E;
        public const int LDXa = 0xAE;
        public const int DECa = 0xCE;
        public const int INCa = 0xEE;

        public const int SLOa = 0x0F;
        public const int RLAa = 0x2F;
        public const int SREa = 0x4F;
        public const int RRAa = 0x6F;
        public const int SAXa = 0x8F;
        public const int LAXa = 0xAF;
        public const int DCPa = 0xCF;
        public const int ISBa = 0xEF;

        public const int BPLr = 0x10;
        public const int BMIr = 0x30;
        public const int BVCr = 0x50;
        public const int BVSr = 0x70;
        public const int BCCr = 0x90;
        public const int BCSr = 0xB0;
        public const int BNEr = 0xD0;
        public const int BEQr = 0xF0;

        public const int ORAiy = 0x11;
        public const int ANDiy = 0x31;
        public const int EORiy = 0x51;
        public const int ADCiy = 0x71;
        public const int STAiy = 0x91;
        public const int LDAiy = 0xB1;
        public const int CMPiy = 0xD1;
        public const int SBCiy = 0xF1;

        public const int SLOiy = 0x13;
        public const int RLAiy = 0x33;
        public const int SREiy = 0x53;
        public const int RRAiy = 0x73;
        public const int SHAiy = 0x93;
        public const int LAXiy = 0xB3;
        public const int DCPiy = 0xD3;
        public const int ISBiy = 0xF3;

        public const int NOPzx = 0x14;
        //public const int  NOPzx_ NOPzx: case 0x34: case 0x54: case 0x74: case 0xD4: case 0xF4
        public const int STYzx = 0x94;
        public const int LDYzx = 0xB4;

        public const int ORAzx = 0x15;
        public const int ANDzx = 0x35;
        public const int EORzx = 0x55;
        public const int ADCzx = 0x75;
        public const int STAzx = 0x95;
        public const int LDAzx = 0xB5;
        public const int CMPzx = 0xD5;
        public const int SBCzx = 0xF5;

        public const int ASLzx = 0x16;
        public const int ROLzx = 0x36;
        public const int LSRzx = 0x56;
        public const int RORzx = 0x76;
        public const int STXzy = 0x96;
        public const int LDXzy = 0xB6;
        public const int DECzx = 0xD6;
        public const int INCzx = 0xF6;

        public const int SLOzx = 0x17;
        public const int RLAzx = 0x37;
        public const int SREzx = 0x57;
        public const int RRAzx = 0x77;
        public const int SAXzy = 0x97;
        public const int LAXzy = 0xB7;
        public const int DCPzx = 0xD7;
        public const int ISBzx = 0xF7;

        public const int CLCn = 0x18;
        public const int SECn = 0x38;
        public const int CLIn = 0x58;
        public const int SEIn = 0x78;
        public const int TYAn = 0x98;
        public const int CLVn = 0xB8;
        public const int CLDn = 0xD8;
        public const int SEDn = 0xF8;

        public const int ORAay = 0x19;
        public const int ANDay = 0x39;
        public const int EORay = 0x59;
        public const int ADCay = 0x79;
        public const int STAay = 0x99;
        public const int LDAay = 0xB9;
        public const int CMPay = 0xD9;
        public const int SBCay = 0xF9;

        public const int TXSn = 0x9A;
        public const int TSXn = 0xBA;

        public const int SLOay = 0x1B;
        public const int RLAay = 0x3B;
        public const int SREay = 0x5B;
        public const int RRAay = 0x7B;
        public const int SHSay = 0x9B;
        public const int TASay = 0x9B;
        public const int LASay = 0xBB;
        public const int DCPay = 0xDB;
        public const int ISBay = 0xFB;

        public const int NOPax = 0x1C;
        //public const int  NOPax_ NOPax: case 0x3C: case 0x5C: case 0x7C: case 0xDC: case 0xFC
        public const int SHYax = 0x9C;
        public const int LDYax = 0xBC;

        public const int ORAax = 0x1D;
        public const int ANDax = 0x3D;
        public const int EORax = 0x5D;
        public const int ADCax = 0x7D;
        public const int STAax = 0x9D;
        public const int LDAax = 0xBD;
        public const int CMPax = 0xDD;
        public const int SBCax = 0xFD;

        public const int ASLax = 0x1E;
        public const int ROLax = 0x3E;
        public const int LSRax = 0x5E;
        public const int RORax = 0x7E;
        public const int SHXay = 0x9E;
        public const int LDXay = 0xBE;
        public const int DECax = 0xDE;
        public const int INCax = 0xFE;

        public const int SLOax = 0x1F;
        public const int RLAax = 0x3F;
        public const int SREax = 0x5F;
        public const int RRAax = 0x7F;
        public const int SHAay = 0x9F;
        public const int LAXay = 0xBF;
        public const int DCPax = 0xDF;
        public const int ISBax = 0xFF;

        // Instruction Aliases
        public const int ASOix = SLOix;
        public const int LSEix = SREix;
        public const int AXSix = SAXix;
        public const int DCMix = DCPix;
        public const int INSix = ISBix;
        public const int ASOz = SLOz;
        public const int LSEz = SREz;
        public const int AXSz = SAXz;
        public const int DCMz = DCPz;
        public const int INSz = ISBz;
        public const int ALRb = ASRb;
        public const int OALb = LXAb;
        public const int ASOa = SLOa;
        public const int LSEa = SREa;
        public const int AXSa = SAXa;
        public const int DCMa = DCPa;
        public const int INSa = ISBa;
        public const int ASOiy = SLOiy;
        public const int LSEiy = SREiy;
        public const int AXAiy = SHAiy;
        public const int DCMiy = DCPiy;
        public const int INSiy = ISBiy;
        public const int ASOzx = SLOzx;
        public const int LSEzx = SREzx;
        public const int AXSzy = SAXzy;
        public const int DCMzx = DCPzx;
        public const int INSzx = ISBzx;
        public const int ASOay = SLOay;
        public const int LSEay = SREay;
        public const int DCMay = DCPay;
        public const int INSay = ISBay;
        public const int SAYax = SHYax;
        public const int XASay = SHXay;
        public const int ASOax = SLOax;
        public const int LSEax = SREax;
        public const int AXAay = SHAay;
        public const int DCMax = DCPax;
        public const int INSax = ISBax;
        public const int SKBn = NOPb;
        public const int SKWn = NOPa;






    }
}
