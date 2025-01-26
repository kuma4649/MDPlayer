using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.GBS
{
    public partial class CPU
    {
        // 0x00
        private int NOP()
        {
            nimo = "NOP";
            return insts[0].cycle[0];
        }


        int LD_BC_d16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            reg.bc = d;
#if DEBUG
            nimo = string.Format("LD BC,${0:X04}", d);
#endif

            return insts[0x01].cycle[0];
        }


        int LD_pBCs_A()
        {
            mem.PokeB(reg.bc, reg.a,vgmFrameCounter);

#if DEBUG
            nimo = "LD (BC),A";
#endif

            return insts[0x02].cycle[0];
        }


        int INC_BC()
        {
            reg.bc++;

#if DEBUG
            nimo = "INC BC";
#endif

            return insts[0x03].cycle[0];
        }


        int INC_B()
        {
            byte d = reg.b;
            reg.b++;

            reg.Z = (reg.b == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.C =;

#if DEBUG
            nimo = "INC B";
#endif

            return insts[0x04].cycle[0];
        }


        int DEC_B()
        {
            byte d = reg.b;
            reg.b--;

            reg.Z = (reg.b == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC B";
#endif

            return insts[0x05].cycle[0];
        }


        int LD_B_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.b = d;

#if DEBUG
            nimo = string.Format("LD B,${0:X02}", d);
#endif

            return insts[0x06].cycle[0];
        }


        int RLCA()
        {
            byte d = reg.a;
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | (reg.C ? 0x01 : 0));
            reg.a = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLCA";
#endif

            return insts[0x07].cycle[0];
        }


        // 0x08
        int LD_pa16s_SP()
        {
            ushort a = mem.PeekW(reg.pc);
            reg.pc += 2;
            mem.PokeW(a, reg.sp, vgmFrameCounter);
#if DEBUG
            nimo = string.Format("LD ({0:X04),SP", a);
#endif

            return insts[0x08].cycle[0];
        }


        int ADD_HL_BC()
        {
            int a = reg.hl + reg.bc;
            int b = reg.l + reg.c;
            reg.hl = (ushort)a;

            //reg.Z =;
            reg.S = false;
            reg.H = (b & 0xf00) != 0;
            reg.C = (a & 0xf_0000) != 0;

#if DEBUG
            nimo = "ADD HL,BC";
#endif

            return insts[0x09].cycle[0];
        }


        int LD_A_pBCs()
        {
            reg.a = mem.PeekB(reg.bc);

#if DEBUG
            nimo = "LD A,(BC)";
#endif

            return insts[0x0a].cycle[0];
        }


        int DEC_BC()
        {
            reg.bc--;

#if DEBUG
            nimo = "DEC BC";
#endif

            return insts[0x0b].cycle[0];
        }


        int INC_C()
        {
            byte d = reg.c;
            reg.c++;

            reg.Z = (reg.c == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.C =;

#if DEBUG
            nimo = "INC C";
#endif

            return insts[0x0c].cycle[0];
        }


        int DEC_C()
        {
            byte d = reg.c;
            reg.c--;

            reg.Z = (reg.c == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC C";
#endif

            return insts[0x0d].cycle[0];
        }


        int LD_C_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.c = d;

#if DEBUG
            nimo = string.Format("LD C,${0:X02}", d);
#endif

            return insts[0x0e].cycle[0];
        }


        int RRCA()
        {
            byte d = reg.a;
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.a = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRCA";
#endif

            return insts[0x0f].cycle[0];
        }


        // 0x10
        int STOP_0()
        {
            isStop = true;
            return 1;
        }


        int LD_DE_d16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            reg.de = d;
#if DEBUG
            nimo = string.Format("LD DE,${0:X04}", d);
#endif

            return insts[0x11].cycle[0];
        }


        int LD_pDEs_A()
        {
            mem.PokeB(reg.de, reg.a, vgmFrameCounter);

#if DEBUG
            nimo = "LD (DE),A";
#endif

            return insts[0x12].cycle[0];
        }


        int INC_DE()
        {
            reg.de++;

#if DEBUG
            nimo = "INC DE";
#endif

            return insts[0x13].cycle[0];
        }


        int INC_D()
        {
            byte d = reg.d;
            reg.d++;

            reg.Z = (reg.d == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.C =;

#if DEBUG
            nimo = "INC D";
#endif

            return insts[0x14].cycle[0];
        }


        int DEC_D()
        {
            byte d = reg.d;
            reg.d--;

            reg.Z = (reg.d == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC D";
#endif

            return insts[0x15].cycle[0];
        }


        int LD_D_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.d = d;

#if DEBUG
            nimo = string.Format("LD D,${0:X02}", d);
#endif

            return insts[0x16].cycle[0];
        }


        int RLA()
        {
            byte d = reg.a;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.a = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLA";
#endif

            return insts[0x17].cycle[0];
        }


        // 0x18
        int JR_r8()
        {
            sbyte d = (sbyte)mem.PeekB(reg.pc);
            reg.pc++;
            int c = insts[0x18].cycle[0];
            reg.pc = (ushort)(reg.pc + d);
#if DEBUG
            nimo = string.Format("JR ${0:X02}", d);
#endif

            return c;
        }


        int ADD_HL_DE()
        {
            int a = reg.hl + reg.de;
            int b = reg.l + reg.e;
            reg.hl = (ushort)a;

            //reg.Z =;
            reg.S = false;
            reg.H = (b & 0xf00) != 0;
            reg.C = (a & 0xf_0000) != 0;

#if DEBUG
            nimo = "ADD HL,DE";
#endif

            return insts[0x19].cycle[0];
        }


        int LD_A_pDEs()
        {
            reg.a = mem.PeekB(reg.de);

#if DEBUG
            nimo = "LD A,(DE)";
#endif

            return insts[0x1a].cycle[0];
        }


        int DEC_DE()
        {
            reg.de--;

#if DEBUG
            nimo = "DEC DE";
#endif

            return insts[0x1b].cycle[0];
        }


        int INC_E()
        {
            byte d = reg.e;
            reg.e++;

            reg.Z = (reg.e == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.c =;

#if DEBUG
            nimo = "INC E";
#endif

            return insts[0x1c].cycle[0];
        }


        int DEC_E()
        {
            byte d = reg.e;
            reg.e--;

            reg.Z = (reg.e == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC E";
#endif

            return insts[0x1d].cycle[0];
        }


        int LD_E_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.e = d;

#if DEBUG
            nimo = string.Format("LD E,${0:X02}", d);
#endif

            return insts[0x1e].cycle[0];
        }


        int RRA()
        {
            byte d = reg.a;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.a = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRA";
#endif

            return insts[0x1f].cycle[0];
        }


        // 0x20
        int JR_NZ_r8()
        {
            sbyte d = (sbyte)mem.PeekB(reg.pc);
            reg.pc++;
            int c = insts[0x20].cycle[1];
            if (!reg.Z)
            {
                reg.pc = (ushort)(reg.pc + d);
                c = insts[0x20].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JR NZ,${0:X02}", d);
#endif

            return c;
        }


        int LD_HL_d16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            reg.hl = d;
#if DEBUG
            nimo = string.Format("LD HL,${0:X04}", d);
#endif

            return insts[0x21].cycle[0];
        }


        int LD_pHLplss_A()
        {
            mem.PokeB(reg.hl, reg.a, vgmFrameCounter);
            reg.hl++;
#if DEBUG
            nimo = "LD (HL+),A";
#endif

            return insts[0x22].cycle[0];
        }


        int INC_HL()
        {
            reg.hl++;

#if DEBUG
            nimo = "INC HL";
#endif

            return insts[0x23].cycle[0];
        }


        int INC_H()
        {
            byte d = reg.h;
            reg.h++;

            reg.Z = (reg.h == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.C =;

#if DEBUG
            nimo = "INC H";
#endif

            return insts[0x24].cycle[0];
        }


        int DEC_H()
        {
            byte d = reg.h;
            reg.h--;

            reg.Z = (reg.h == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC H";
#endif

            return insts[0x25].cycle[0];
        }


        int LD_H_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.h = d;

#if DEBUG
            nimo = string.Format("LD H,${0:X02}", d);
#endif

            return insts[0x26].cycle[0];
        }


        int DAA()
        {
            byte tmp = reg.a;
            if (reg.S)
            {
                if (reg.H | ((reg.a & 0xf) > 9)) tmp -= 6;
                if (reg.C | (reg.a > 0x99)) tmp -= 0x60;
            }
            else
            {
                if (reg.H | ((reg.a & 0xf) > 9)) tmp += 6;
                if (reg.C | (reg.a > 0x99)) tmp += 0x60;
            }
            reg.H = false;
            reg.C = (reg.a > 0x99);
            reg.Z = (tmp == 0);
            reg.a = tmp;

#if DEBUG
            nimo = "DAA";
#endif

            return insts[0x27].cycle[0];
        }


        // 0x28
        int JR_Z_r8()
        {
            sbyte d = (sbyte)mem.PeekB(reg.pc);
            reg.pc++;
            int c = insts[0x28].cycle[1];
            if (reg.Z)
            {
                reg.pc = (ushort)(reg.pc + d);
                c = insts[0x28].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JR Z,${0:X02}", d);
#endif

            return c;
        }


        int ADD_HL_HL()
        {
            int a = reg.hl + reg.hl;
            int b = reg.l + reg.e;
            reg.hl = (ushort)a;

            //reg.Z =;
            reg.S = false;
            reg.H = (b & 0xf00) != 0;
            reg.C = (a & 0xf_0000) != 0;

#if DEBUG
            nimo = "ADD HL,HL";
#endif

            return insts[0x29].cycle[0];
        }


        int LD_A_pHLplss()
        {
            byte d = mem.PeekB(reg.hl);
            reg.a = d;
            reg.hl++;
#if DEBUG
            nimo = "LD A,(HL+)";
#endif

            return insts[0x2a].cycle[0];
        }


        int DEC_HL()
        {
            reg.hl--;

#if DEBUG
            nimo = "DEC HL";
#endif

            return insts[0x2b].cycle[0];
        }


        int INC_L()
        {
            byte d = reg.l;
            reg.l++;

            reg.Z = (reg.l == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.c =;

#if DEBUG
            nimo = "INC L";
#endif

            return insts[0x2c].cycle[0];
        }


        int DEC_L()
        {
            byte d = reg.l;
            reg.l--;

            reg.Z = (reg.l == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.c =;

#if DEBUG
            nimo = "DEC L";
#endif

            return insts[0x2d].cycle[0];
        }


        int LD_L_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.l = d;

#if DEBUG
            nimo = string.Format("LD L,${0:X02}", d);
#endif

            return insts[0x2e].cycle[0];
        }


        int CPL()
        {
            reg.a = (byte)~reg.a;

#if DEBUG
            nimo = "CPL";
#endif

            return insts[0x2f].cycle[0];
        }


        // 0x30
        int JR_NC_r8()
        {
            sbyte d = (sbyte)mem.PeekB(reg.pc);
            reg.pc++;
            int c = insts[0x30].cycle[1];
            if (!reg.C)
            {
                reg.pc = (ushort)(reg.pc + d);
                c = insts[0x30].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JR NC,${0:X02}", d);
#endif

            return c;
        }


        int LD_SP_d16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            reg.sp = d;
#if DEBUG
            nimo = string.Format("LD SP,${0:X04}", d);
#endif

            return insts[0x31].cycle[0];
        }


        int LD_pHLmiss_A()
        {
            mem.PokeB(reg.hl, reg.a, vgmFrameCounter);
            reg.hl--;
#if DEBUG
            nimo = "LD (HL-),A";
#endif

            return insts[0x32].cycle[0];
        }


        int INC_SP()
        {
            reg.sp++;

#if DEBUG
            nimo = "INC SP";
#endif

            return insts[0x33].cycle[0];
        }


        int INC_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            byte e = (byte)(d + 1);
            mem.PokeB(reg.hl, e, vgmFrameCounter);

            reg.Z = (e == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.C =;

#if DEBUG
            nimo = "INC (HL)";
#endif

            return insts[0x34].cycle[0];
        }


        int DEC_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            byte e = (byte)(d - 1);
            mem.PokeB(reg.hl, e, vgmFrameCounter);

            reg.Z = (e == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC (HL)";
#endif

            return insts[0x35].cycle[0];
        }


        int LD_pHLs_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            mem.PokeB(reg.hl, d, vgmFrameCounter);

#if DEBUG
            nimo = string.Format("LD (HL),${0:X02}", d);
#endif

            return insts[0x36].cycle[0];
        }


        int SCF()
        {
            reg.S = false;
            reg.H = false;
            reg.C = true;

#if DEBUG
            nimo = "SCF";
#endif

            return insts[0x37].cycle[0];
        }


        // 0x38
        int JR_C_r8()
        {
            sbyte d = (sbyte)mem.PeekB(reg.pc);
            reg.pc++;
            int c = insts[0x38].cycle[1];
            if (reg.C)
            {
                reg.pc = (ushort)(reg.pc + d);
                c = insts[0x38].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JR C,${0:X02}", d);
#endif

            return c;
        }


        int ADD_HL_SP()
        {
            int a = reg.hl + reg.sp;
            int b = reg.l + (byte)reg.sp;
            reg.hl = (ushort)a;

            //reg.Z =;
            reg.S = false;
            reg.H = (b & 0xf00) != 0;
            reg.C = (a & 0xf_0000) != 0;

#if DEBUG
            nimo = "ADD HL,SP";
#endif

            return insts[0x39].cycle[0];
        }


        int LD_A_pHLmiss()
        {
            byte d = mem.PeekB(reg.hl);
            reg.a = d;
            reg.hl--;
#if DEBUG
            nimo = "LD A,(HL-)";
#endif

            return insts[0x3a].cycle[0];
        }


        int DEC_SP()
        {
            reg.sp--;

#if DEBUG
            nimo = "DEC SP";
#endif

            return insts[0x3b].cycle[0];
        }


        int INC_A()
        {
            byte d = reg.a;
            reg.a++;

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((d & 0xf) == 0xf);// ((byte)(d & 0xf) + 1) > 0xf;
            //reg.C =;

#if DEBUG
            nimo = "INC A";
#endif

            return insts[0x3c].cycle[0];
        }


        int DEC_A()
        {
            byte d = reg.a;
            reg.a--;

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((d & 0xf) == 0x0); //((byte)(d & 0xf) - 1) < 0;
            //reg.C =;

#if DEBUG
            nimo = "DEC A";
#endif

            return insts[0x3d].cycle[0];
        }


        int LD_A_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.a = d;
#if DEBUG
            nimo = string.Format("LD A,${0:X02}", d);
#endif

            return insts[0x3e].cycle[0];
        }


        int CCF()
        {
            reg.S = false;
            reg.H = false;
            reg.C = !reg.C;

#if DEBUG
            nimo = "CCF";
#endif

            return insts[0x3f].cycle[0];
        }


        // 0x40
        int LD_B_B()
        {
            reg.b = reg.b;
#if DEBUG
            nimo = "LD B,B";
#endif
            return insts[0x40].cycle[0];
        }


        int LD_B_C()
        {
            reg.b = reg.c;
#if DEBUG
            nimo = "LD B,C";
#endif
            return insts[0x41].cycle[0];
        }


        int LD_B_D()
        {
            reg.b = reg.d;
#if DEBUG
            nimo = "LD B,D";
#endif
            return insts[0x42].cycle[0];
        }


        int LD_B_E()
        {
            reg.b = reg.e;
#if DEBUG
            nimo = "LD B,E";
#endif
            return insts[0x43].cycle[0];
        }


        int LD_B_H()
        {
            reg.b = reg.h;
#if DEBUG
            nimo = "LD B,H";
#endif
            return insts[0x44].cycle[0];
        }


        int LD_B_L()
        {
            reg.b = reg.l;
#if DEBUG
            nimo = "LD B,L";
#endif
            return insts[0x45].cycle[0];
        }


        int LD_B_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.b = d;
#if DEBUG
            nimo = "LD B,(HL)";
#endif
            return insts[0x46].cycle[0];
        }


        int LD_B_A()
        {
            reg.b = reg.a;
#if DEBUG
            nimo = "LD B,A";
#endif
            return insts[0x47].cycle[0];
        }


        // 0x48
        int LD_C_B()
        {
            reg.c = reg.b;
#if DEBUG
            nimo = "LD C,B";
#endif
            return insts[0x48].cycle[0];
        }


        int LD_C_C()
        {
            reg.c = reg.c;
#if DEBUG
            nimo = "LD C,C";
#endif
            return insts[0x49].cycle[0];
        }


        int LD_C_D()
        {
            reg.c = reg.d;
#if DEBUG
            nimo = "LD C,D";
#endif
            return insts[0x4a].cycle[0];
        }


        int LD_C_E()
        {
            reg.c = reg.e;
#if DEBUG
            nimo = "LD C,E";
#endif
            return insts[0x4b].cycle[0];
        }


        int LD_C_H()
        {
            reg.c = reg.h;
#if DEBUG
            nimo = "LD C,H";
#endif
            return insts[0x4c].cycle[0];
        }


        int LD_C_L()
        {
            reg.c = reg.l;
#if DEBUG
            nimo = "LD C,L";
#endif
            return insts[0x4d].cycle[0];
        }


        int LD_C_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.c = d;
#if DEBUG
            nimo = "LD C,(HL)";
#endif
            return insts[0x4e].cycle[0];
        }


        int LD_C_A()
        {
            reg.c = reg.a;
#if DEBUG
            nimo = "LD C,A";
#endif
            return insts[0x4f].cycle[0];
        }


        // 0x50
        int LD_D_B()
        {
            reg.d = reg.b;
#if DEBUG
            nimo = "LD D,B";
#endif
            return insts[0x50].cycle[0];
        }


        int LD_D_C()
        {
            reg.d = reg.c;
#if DEBUG
            nimo = "LD D,C";
#endif
            return insts[0x51].cycle[0];
        }


        int LD_D_D()
        {
            reg.d = reg.d;
#if DEBUG
            nimo = "LD D,D";
#endif
            return insts[0x52].cycle[0];
        }


        int LD_D_E()
        {
            reg.d = reg.e;
#if DEBUG
            nimo = "LD D,E";
#endif
            return insts[0x53].cycle[0];
        }


        int LD_D_H()
        {
            reg.d = reg.h;
#if DEBUG
            nimo = "LD D,H";
#endif
            return insts[0x54].cycle[0];
        }


        int LD_D_L()
        {
            reg.d = reg.l;
#if DEBUG
            nimo = "LD D,L";
#endif
            return insts[0x55].cycle[0];
        }


        int LD_D_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.d = d;
#if DEBUG
            nimo = "LD D,(HL)";
#endif
            return insts[0x56].cycle[0];
        }


        int LD_D_A()
        {
            reg.d = reg.a;
#if DEBUG
            nimo = "LD D,A";
#endif
            return insts[0x57].cycle[0];
        }


        // 0x58
        int LD_E_B()
        {
            reg.e = reg.b;
#if DEBUG
            nimo = "LD E,B";
#endif
            return insts[0x58].cycle[0];
        }


        int LD_E_C()
        {
            reg.e = reg.c;
#if DEBUG
            nimo = "LD E,C";
#endif
            return insts[0x59].cycle[0];
        }


        int LD_E_D()
        {
            reg.e = reg.d;
#if DEBUG
            nimo = "LD E,D";
#endif
            return insts[0x5a].cycle[0];
        }


        int LD_E_E()
        {
            reg.e = reg.e;
#if DEBUG
            nimo = "LD E,E";
#endif
            return insts[0x5b].cycle[0];
        }


        int LD_E_H()
        {
            reg.e = reg.h;
#if DEBUG
            nimo = "LD E,H";
#endif
            return insts[0x5c].cycle[0];
        }


        int LD_E_L()
        {
            reg.e = reg.l;
#if DEBUG
            nimo = "LD E,L";
#endif
            return insts[0x5d].cycle[0];
        }


        int LD_E_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.e = d;
#if DEBUG
            nimo = "LD E,(HL)";
#endif
            return insts[0x5e].cycle[0];
        }


        int LD_E_A()
        {
            reg.e = reg.a;
#if DEBUG
            nimo = "LD E,A";
#endif
            return insts[0x5f].cycle[0];
        }


        // 0x60
        int LD_H_B()
        {
            reg.h = reg.b;
#if DEBUG
            nimo = "LD H,B";
#endif
            return insts[0x60].cycle[0];
        }


        int LD_H_C()
        {
            reg.h = reg.c;
#if DEBUG
            nimo = "LD H,C";
#endif
            return insts[0x61].cycle[0];
        }


        int LD_H_D()
        {
            reg.h = reg.d;
#if DEBUG
            nimo = "LD H,D";
#endif
            return insts[0x62].cycle[0];
        }


        int LD_H_E()
        {
            reg.h = reg.e;
#if DEBUG
            nimo = "LD H,E";
#endif
            return insts[0x63].cycle[0];
        }


        int LD_H_H()
        {
            reg.h = reg.h;
#if DEBUG
            nimo = "LD H,H";
#endif
            return insts[0x64].cycle[0];
        }


        int LD_H_L()
        {
            reg.h = reg.l;
#if DEBUG
            nimo = "LD H,L";
#endif
            return insts[0x65].cycle[0];
        }


        int LD_H_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.h = d;
#if DEBUG
            nimo = "LD H,(HL)";
#endif
            return insts[0x66].cycle[0];
        }


        int LD_H_A()
        {
            reg.h = reg.a;
#if DEBUG
            nimo = "LD H,A";
#endif
            return insts[0x67].cycle[0];
        }


        // 0x68
        int LD_L_B()
        {
            reg.l = reg.b;
#if DEBUG
            nimo = "LD L,B";
#endif
            return insts[0x68].cycle[0];
        }


        int LD_L_C()
        {
            reg.l = reg.c;
#if DEBUG
            nimo = "LD L,C";
#endif
            return insts[0x69].cycle[0];
        }


        int LD_L_D()
        {
            reg.l = reg.d;
#if DEBUG
            nimo = "LD L,D";
#endif
            return insts[0x6a].cycle[0];
        }


        int LD_L_E()
        {
            reg.l = reg.e;
#if DEBUG
            nimo = "LD L,E";
#endif
            return insts[0x6b].cycle[0];
        }


        int LD_L_H()
        {
            reg.l = reg.h;
#if DEBUG
            nimo = "LD L,H";
#endif
            return insts[0x6c].cycle[0];
        }


        int LD_L_L()
        {
            reg.l = reg.l;
#if DEBUG
            nimo = "LD L,L";
#endif
            return insts[0x6d].cycle[0];
        }


        int LD_L_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.l = d;
#if DEBUG
            nimo = "LD L,(HL)";
#endif
            return insts[0x6e].cycle[0];
        }


        int LD_L_A()
        {
            reg.l = reg.a;
#if DEBUG
            nimo = "LD L,A";
#endif
            return insts[0x6f].cycle[0];
        }


        // 0x70
        int LD_pHLs_B()
        {
            mem.PokeB(reg.hl, reg.b, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),B";
#endif
            return insts[0x70].cycle[0];
        }


        int LD_pHLs_C()
        {
            mem.PokeB(reg.hl, reg.c, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),C";
#endif
            return insts[0x71].cycle[0];
        }


        int LD_pHLs_D()
        {
            mem.PokeB(reg.hl, reg.d, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),D";
#endif
            return insts[0x72].cycle[0];
        }


        int LD_pHLs_E()
        {
            mem.PokeB(reg.hl, reg.e, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),E";
#endif
            return insts[0x73].cycle[0];
        }


        int LD_pHLs_H()
        {
            mem.PokeB(reg.hl, reg.h, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),H";
#endif
            return insts[0x74].cycle[0];
        }


        int LD_pHLs_L()
        {
            mem.PokeB(reg.hl, reg.l, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),L";
#endif
            return insts[0x75].cycle[0];
        }


        int HALT()
        {
            isHalt = true;
#if DEBUG
            nimo = "HALT";
#endif
            return insts[0x76].cycle[0];
        }


        int LD_pHLs_A()
        {
            mem.PokeB(reg.hl, reg.a, vgmFrameCounter);
#if DEBUG
            nimo = "LD (HL),A";
#endif
            return insts[0x77].cycle[0];
        }


        // 0x78
        int LD_A_B()
        {
            reg.a = reg.b;
#if DEBUG
            nimo = "LD A,B";
#endif
            return insts[0x78].cycle[0];
        }


        int LD_A_C()
        {
            reg.a = reg.c;
#if DEBUG
            nimo = "LD A,C";
#endif
            return insts[0x79].cycle[0];
        }


        int LD_A_D()
        {
            reg.a = reg.d;
#if DEBUG
            nimo = "LD A,D";
#endif
            return insts[0x7a].cycle[0];
        }


        int LD_A_E()
        {
            reg.a = reg.e;
#if DEBUG
            nimo = "LD A,E";
#endif
            return insts[0x7b].cycle[0];
        }


        int LD_A_H()
        {
            reg.a = reg.h;
#if DEBUG
            nimo = "LD A,H";
#endif
            return insts[0x7c].cycle[0];
        }


        int LD_A_L()
        {
            reg.a = reg.l;
#if DEBUG
            nimo = "LD A,L";
#endif
            return insts[0x7d].cycle[0];
        }


        int LD_A_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.a = d;
#if DEBUG
            nimo = "LD A,(HL)";
#endif
            return insts[0x7e].cycle[0];
        }


        int LD_A_A()
        {
            reg.a = reg.a;
#if DEBUG
            nimo = "LD A,A";
#endif
            return insts[0x7f].cycle[0];
        }


        // 0x80
        int ADD_A_B()
        {
            int a = reg.a;
            int b = reg.b;
            reg.a = (byte)(reg.a + reg.b);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,B";
#endif
            return insts[0x80].cycle[0];
        }


        int ADD_A_C()
        {
            int a = reg.a;
            int b = reg.c;
            reg.a = (byte)(reg.a + reg.c);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,C";
#endif
            return insts[0x81].cycle[0];
        }


        int ADD_A_D()
        {
            int a = reg.a;
            int b = reg.d;
            reg.a = (byte)(reg.a + reg.d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,D";
#endif
            return insts[0x82].cycle[0];
        }


        int ADD_A_E()
        {
            int a = reg.a;
            int b = reg.e;
            reg.a = (byte)(reg.a + reg.e);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,E";
#endif
            return insts[0x83].cycle[0];
        }


        int ADD_A_H()
        {
            int a = reg.a;
            int b = reg.h;
            reg.a = (byte)(reg.a + reg.h);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,H";
#endif
            return insts[0x84].cycle[0];
        }


        int ADD_A_L()
        {
            int a = reg.a;
            int b = reg.l;
            reg.a = (byte)(reg.a + reg.l);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,L";
#endif
            return insts[0x85].cycle[0];
        }


        int ADD_A_pHLs()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.hl);
            reg.a = (byte)(reg.a + b);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,(HL)";
#endif
            return insts[0x86].cycle[0];
        }


        int ADD_A_A()
        {
            int a = reg.a;
            int b = reg.a;
            reg.a = (byte)(reg.a + reg.a);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = "ADD A,A";
#endif
            return insts[0x87].cycle[0];
        }


        // 0x88
        int ADC_A_B()
        {
            int a = reg.a;
            int b = reg.b;
            reg.a = (byte)(reg.a + reg.b + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,B";
#endif
            return insts[0x88].cycle[0];
        }


        int ADC_A_C()
        {
            int a = reg.a;
            int b = reg.c;
            reg.a = (byte)(reg.a + reg.c + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,C";
#endif
            return insts[0x89].cycle[0];
        }


        int ADC_A_D()
        {
            int a = reg.a;
            int b = reg.d;
            reg.a = (byte)(reg.a + reg.d + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,D";
#endif
            return insts[0x8a].cycle[0];
        }


        int ADC_A_E()
        {
            int a = reg.a;
            int b = reg.e;
            reg.a = (byte)(reg.a + reg.e + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,E";
#endif
            return insts[0x8b].cycle[0];
        }


        int ADC_A_H()
        {
            int a = reg.a;
            int b = reg.h;
            reg.a = (byte)(reg.a + reg.h + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,H";
#endif
            return insts[0x8c].cycle[0];
        }


        int ADC_A_L()
        {
            int a = reg.a;
            int b = reg.l;
            reg.a = (byte)(reg.a + reg.l + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,L";
#endif
            return insts[0x8d].cycle[0];
        }


        int ADC_A_pHLs()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.hl);
            reg.a = (byte)(reg.a + b + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,(HL)";
#endif
            return insts[0x8e].cycle[0];
        }


        int ADC_A_A()
        {
            int a = reg.a;
            int b = reg.a;
            reg.a = (byte)(reg.a + reg.a + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = "ADC A,A";
#endif
            return insts[0x8f].cycle[0];
        }


        // 0x90
        int SUB_B()
        {
            int a = reg.a;
            int b = reg.b;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB B", b);
#endif
            return insts[0x90].cycle[0];
        }


        int SUB_C()
        {
            int a = reg.a;
            int b = reg.c;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB C", b);
#endif
            return insts[0x91].cycle[0];
        }


        int SUB_D()
        {
            int a = reg.a;
            int b = reg.d;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB D", b);
#endif
            return insts[0x92].cycle[0];
        }


        int SUB_E()
        {
            int a = reg.a;
            int b = reg.e;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB E", b);
#endif
            return insts[0x93].cycle[0];
        }


        int SUB_H()
        {
            int a = reg.a;
            int b = reg.h;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB H", b);
#endif
            return insts[0x94].cycle[0];
        }


        int SUB_L()
        {
            int a = reg.a;
            int b = reg.l;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB L", b);
#endif
            return insts[0x95].cycle[0];
        }


        int SUB_pHLs()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.hl);
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB (HL)", b);
#endif
            return insts[0x96].cycle[0];
        }


        int SUB_A()
        {
            int a = reg.a;
            int b = reg.a;
            reg.a = (byte)(a - b);

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB A", b);
#endif
            return insts[0x97].cycle[0];
        }


        // 0x98
        int SBC_A_B()
        {
            int a = reg.a;
            int b = reg.b;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,B";
#endif
            return insts[0x98].cycle[0];
        }


        int SBC_A_C()
        {
            int a = reg.a;
            int b = reg.c;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,C";
#endif
            return insts[0x99].cycle[0];
        }


        int SBC_A_D()
        {
            int a = reg.a;
            int b = reg.d;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,D";
#endif
            return insts[0x9a].cycle[0];
        }


        int SBC_A_E()
        {
            int a = reg.a;
            int b = reg.e;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,E";
#endif
            return insts[0x9b].cycle[0];
        }


        int SBC_A_H()
        {
            int a = reg.a;
            int b = reg.h;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,H";
#endif
            return insts[0x9c].cycle[0];
        }


        int SBC_A_L()
        {
            int a = reg.a;
            int b = reg.l;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,L";
#endif
            return insts[0x9d].cycle[0];
        }


        int SBC_A_pHLs()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.hl);
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,(HL)";
#endif
            return insts[0x9e].cycle[0];
        }


        int SBC_A_A()
        {
            int a = reg.a;
            int b = reg.a;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = "SBC A,A";
#endif
            return insts[0x9f].cycle[0];
        }


        // 0xa0
        int AND_B()
        {
            reg.a = (byte)(reg.a & reg.b);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND B";
#endif
            return insts[0xa0].cycle[0];
        }


        int AND_C()
        {
            reg.a = (byte)(reg.a & reg.c);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND C";
#endif
            return insts[0xa1].cycle[0];
        }


        int AND_D()
        {
            reg.a = (byte)(reg.a & reg.d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND D";
#endif
            return insts[0xa2].cycle[0];
        }


        int AND_E()
        {
            reg.a = (byte)(reg.a & reg.e);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND E";
#endif
            return insts[0xa3].cycle[0];
        }


        int AND_H()
        {
            reg.a = (byte)(reg.a & reg.h);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND H";
#endif
            return insts[0xa4].cycle[0];
        }


        int AND_L()
        {
            reg.a = (byte)(reg.a & reg.l);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND L";
#endif
            return insts[0xa5].cycle[0];
        }


        int AND_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.a = (byte)(reg.a & d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND (HL)";
#endif
            return insts[0xa6].cycle[0];
        }


        int AND_A()
        {
            reg.a = (byte)(reg.a & reg.a);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = "AND A";
#endif
            return insts[0xa7].cycle[0];
        }


        // 0xa8
        int XOR_B()
        {
            reg.a = (byte)(reg.a ^ reg.b);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR B";
#endif
            return insts[0xa8].cycle[0];
        }


        int XOR_C()
        {
            reg.a = (byte)(reg.a ^ reg.c);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR C";
#endif
            return insts[0xa9].cycle[0];
        }


        int XOR_D()
        {
            reg.a = (byte)(reg.a ^ reg.d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR D";
#endif
            return insts[0xaa].cycle[0];
        }


        int XOR_E()
        {
            reg.a = (byte)(reg.a ^ reg.e);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR E";
#endif
            return insts[0xab].cycle[0];
        }


        int XOR_H()
        {
            reg.a = (byte)(reg.a ^ reg.h);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR H";
#endif
            return insts[0xac].cycle[0];
        }


        int XOR_L()
        {
            reg.a = (byte)(reg.a ^ reg.l);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR L";
#endif
            return insts[0xad].cycle[0];
        }


        int XOR_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.a = (byte)(reg.a ^ d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR (HL)";
#endif
            return insts[0xae].cycle[0];
        }


        int XOR_A()
        {
            reg.a = (byte)(reg.a ^ reg.a);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "XOR A";
#endif
            return insts[0xaf].cycle[0];
        }


        // 0xb0
        int OR_B()
        {
            reg.a = (byte)(reg.a | reg.b);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR B";
#endif
            return insts[0xb0].cycle[0];
        }


        int OR_C()
        {
            reg.a = (byte)(reg.a | reg.c);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR C";
#endif
            return insts[0xb1].cycle[0];
        }


        int OR_D()
        {
            reg.a = (byte)(reg.a | reg.d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR D";
#endif
            return insts[0xb2].cycle[0];
        }


        int OR_E()
        {
            reg.a = (byte)(reg.a | reg.e);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR E";
#endif
            return insts[0xb3].cycle[0];
        }


        int OR_H()
        {
            reg.a = (byte)(reg.a | reg.h);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR H";
#endif
            return insts[0xb4].cycle[0];
        }


        int OR_L()
        {
            reg.a = (byte)(reg.a | reg.l);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR L";
#endif
            return insts[0xb5].cycle[0];
        }


        int OR_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.a = (byte)(reg.a | d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR (HL)";
#endif
            return insts[0xb6].cycle[0];
        }


        int OR_A()
        {
            reg.a = (byte)(reg.a | reg.a);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = "OR A";
#endif
            return insts[0xb7].cycle[0];
        }


        // 0xb8
        int CP_B()
        {
            int a = reg.b;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP B";
#endif

            return insts[0xb8].cycle[0];
        }


        int CP_C()
        {
            int a = reg.c;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP C";
#endif

            return insts[0xb9].cycle[0];
        }


        int CP_D()
        {
            int a = reg.d;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP D";
#endif

            return insts[0xba].cycle[0];
        }


        int CP_E()
        {
            int a = reg.e;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP E";
#endif

            return insts[0xbb].cycle[0];
        }


        int CP_H()
        {
            int a = reg.h;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP H";
#endif

            return insts[0xbc].cycle[0];
        }


        int CP_L()
        {
            int a = reg.l;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP L";
#endif

            return insts[0xbd].cycle[0];
        }


        int CP_pHLs()
        {
            int a = mem.PeekB(reg.hl);
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP (HL)";
#endif

            return insts[0xbe].cycle[0];
        }


        int CP_A()
        {
            int a = reg.a;
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = "CP A";
#endif

            return insts[0xbf].cycle[0];
        }


        // 0xc0
        int RET_NZ()
        {
            int c = insts[0xc0].cycle[1];
            if (!reg.Z)
            {
                reg.pc = pop();
                c = insts[0xc0].cycle[0];
            }
#if DEBUG
            nimo = "RET NZ";
#endif
            return c;
        }


        int POP_BC()
        {
            reg.bc = pop();
#if DEBUG
            nimo = "POP BC";
#endif
            return insts[0xc1].cycle[0];
        }


        int JP_NZ_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            int c = insts[0xc2].cycle[1];
            if (!reg.Z)
            {
                reg.pc = d;
                c = insts[0xc2].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JP NZ,${0:X04}", d);
#endif

            return c;
        }


        int JP_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc = d;
#if DEBUG
            nimo = string.Format("JP ${0:X04}", d);
#endif

            return insts[0xc3].cycle[0];
        }


        int CALL_NZ_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            int cycle = 0;
            if (!reg.Z)
            {
                push((ushort)(reg.pc + 2));
                reg.pc = d;
            }
            else
            {
                reg.pc += 2;
                cycle = 1;
            }
#if DEBUG
            nimo = string.Format("CALL NZ,${0:X04}", d);
#endif
            return insts[0xc4].cycle[cycle];
        }


        int PUSH_BC()
        {
            push(reg.bc);
#if DEBUG
            nimo = "PUSH BC";
#endif
            return insts[0xc5].cycle[0];
        }


        int ADD_A_d8()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.pc);
            reg.a = (byte)(a + b);
            reg.pc++;

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf)) > 0xf;
            reg.C = (a + b) > 0xff;

#if DEBUG
            nimo = string.Format("ADD A,${0:X02}", b);
#endif
            return insts[0xc6].cycle[0];
        }


        int RST_00H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x00;
#if DEBUG
            nimo = "RST $00";
#endif
            return insts[0xc7].cycle[0];
        }


        // 0xc8
        int RET_Z()
        {
            int c = insts[0xc8].cycle[1];
            if (reg.Z)
            {
                reg.pc = pop();
                c = insts[0xc8].cycle[0];
            }
#if DEBUG
            nimo = "RET Z";
#endif
            return c;
        }


        int RET()
        {
            reg.pc = pop();
#if DEBUG
            nimo = "RET";
#endif
            return insts[0xc9].cycle[0];
        }


        int JP_Z_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            int c = insts[0xca].cycle[1];
            if (reg.Z)
            {
                reg.pc = d;
                c = insts[0xca].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JP Z,${0:X04}", d);
#endif

            return c;
        }


        int PREFIX_CB()
        {
            cbSwitch = true;
#if DEBUG
            nimo = "PREFIX CB";
#endif
            return insts[0xcb].cycle[0];
        }


        int CALL_Z_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            int cycle = 0;
            if (reg.Z)
            {
                push((ushort)(reg.pc + 2));
                reg.pc = d;
            }
            else
            {
                reg.pc += 2;
                cycle = 1;
            }
#if DEBUG
            nimo = string.Format("CALL Z,${0:X04}", d);
#endif
            return insts[0xcc].cycle[cycle];
        }


        int CALL_a16()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = mem.PeekW(reg.pc);
#if DEBUG
            nimo = string.Format("CALL ${0:X04}", reg.pc);
#endif
            return insts[0xcd].cycle[0];
        }

        int ADC_A_d8()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.pc);
            reg.pc++;
            reg.a = (byte)(a + b + (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = ((byte)(a & 0xf) + (byte)(b & 0xf) + (reg.C ? 1 : 0)) > 0xf;
            reg.C = (a + b + (reg.C ? 1 : 0)) > 0xff;

#if DEBUG
            nimo = string.Format("ADC A,${0:X02}", b);
#endif
            return insts[0xce].cycle[0];
        }


        int RST_08H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x08;
#if DEBUG
            nimo = "RST $08";
#endif
            return insts[0xcf].cycle[0];
        }


        // 0xd0
        int RET_NC()
        {
            int c = insts[0xd0].cycle[1];
            if (!reg.C)
            {
                reg.pc = pop();
                c = insts[0xd0].cycle[0];
            }
#if DEBUG
            nimo = "RET NC";
#endif
            return c;
        }


        int POP_DE()
        {
            reg.de = pop();
#if DEBUG
            nimo = "POP DE";
#endif
            return insts[0xd1].cycle[0];
        }


        int JP_NC_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            int c = insts[0xd2].cycle[1];
            if (!reg.C)
            {
                reg.pc = d;
                c = insts[0xd2].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JP NC,${0:X04}", d);
#endif

            return c;
        }

        //0xd3なし

        int CALL_NC_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            int cycle = 0;
            if (!reg.C)
            {
                push((ushort)(reg.pc + 2));
                reg.pc = d;
            }
            else
            {
                reg.pc += 2;
                cycle = 1;
            }
#if DEBUG
            nimo = string.Format("CALL NC,${0:X04}", d);
#endif
            return insts[0xd4].cycle[cycle];
        }


        int PUSH_DE()
        {
            push(reg.de);
#if DEBUG
            nimo = "PUSH DE";
#endif
            return insts[0xd5].cycle[0];
        }


        int SUB_d8()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.pc);
            reg.a = (byte)(a - b);
            reg.pc++;

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((byte)(a & 0xf) - (byte)(b & 0xf)) < 0;
            reg.C = (a - b) < 0;

#if DEBUG
            nimo = string.Format("SUB ${0:X02}", b);
#endif
            return insts[0xd6].cycle[0];
        }


        int RST_10H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x10;
#if DEBUG
            nimo = "RST $10";
#endif
            return insts[0xd7].cycle[0];
        }


        // 0xd8
        int RET_C()
        {
            int c = insts[0xd8].cycle[1];
            if (reg.C)
            {
                reg.pc = pop();
                c = insts[0xd8].cycle[0];
            }
#if DEBUG
            nimo = "RET C";
#endif
            return c;
        }


        int RETI()
        {
            int c = insts[0xd9].cycle[0];
            reg.pc = pop();
            ime = 1;
#if DEBUG
            nimo = "RETI";
#endif
            return c;
        }


        int JP_C_a16()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            int c = insts[0xda].cycle[1];
            if (reg.C)
            {
                reg.pc = d;
                c = insts[0xda].cycle[0];
            }
#if DEBUG
            nimo = string.Format("JP C,${0:X04}", d);
#endif

            return c;
        }

        //0xdb nasi

        int CALL_C_a16()
        {
            int cycle = 0;
            ushort d = mem.PeekW(reg.pc);

            if (reg.C)
            {
                push((ushort)(reg.pc + 2));
                reg.pc = d;
            }
            else
            {
                reg.pc += 2; cycle = 1;
            }
#if DEBUG
            nimo = string.Format("CALL C,${0:X04}", d);
#endif
            return insts[0xdc].cycle[cycle];
        }


        int SBC_A_d8()
        {
            int a = reg.a;
            int b = mem.PeekB(reg.pc);
            reg.pc++;
            reg.a = (byte)(a - b - (reg.C ? 1 : 0));

            reg.Z = (reg.a == 0);
            reg.S = true;
            reg.H = ((a & 0xf) - (b & 0xf) - (reg.C ? 1 : 0)) < 0x0;
            reg.C = (a - b - (reg.C ? 1 : 0)) < 0x0;

#if DEBUG
            nimo = string.Format("SBC A,${0:X02}", b);
#endif
            return insts[0xde].cycle[0];
        }


        int RST_18H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x18;
#if DEBUG
            nimo = "RST $18";
#endif
            return insts[0xdf].cycle[0];
        }


        // 0xe0
        int LDH_pa8s_A()
        {
            byte p = mem.PeekB(reg.pc++);
            mem.PokeB(0xff00 + p, reg.a, vgmFrameCounter);
#if DEBUG
            nimo = string.Format("LD ($FF00+${0:X02}),A", p);
#endif
            return insts[0xe0].cycle[0];
        }


        int POP_HL()
        {
            reg.hl = pop();
#if DEBUG
            nimo = "POP HL";
#endif
            return insts[0xe1].cycle[0];
        }


        int LD_pCs_A()
        {
            mem.PokeB(0xff00 + reg.c, reg.a, vgmFrameCounter);
#if DEBUG
            nimo = "LD ($ff00+C),A";
#endif
            return insts[0xe2].cycle[0];
        }


        int PUSH_HL()
        {
            push(reg.hl);
#if DEBUG
            nimo = "PUSH HL";
#endif
            return insts[0xe5].cycle[0];
        }


        int AND_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.a = (byte)(reg.a & d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = true;
            reg.C = false;

#if DEBUG
            nimo = string.Format("AND ${0:X02}", d);
#endif
            return insts[0xe6].cycle[0];
        }


        int RST_20H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x20;
#if DEBUG
            nimo = "RST $20";
#endif
            return insts[0xe7].cycle[0];
        }


        // 0xe8
        int ADD_SP_r8()
        {
            int a = reg.sp;
            int b = (sbyte)mem.PeekB(reg.pc);
            int c = a + b;
            int d = (a & 0xff) + b;

            reg.sp = (ushort)(c);
            reg.pc++;

            reg.Z = false;
            reg.S = false;
            reg.H = (d & 0xf00) != 0;
            reg.C = (c & 0xf_0000) != 0;

#if DEBUG
            nimo = string.Format("ADD SP,${0:X02}", b);
#endif
            return insts[0xe8].cycle[0];
        }


        int JP_pHLs()
        {
            ushort d = reg.hl;
            reg.pc = d;
#if DEBUG
            nimo = "JP HL";
#endif

            return insts[0xe9].cycle[0];
        }


        int LD_pa16s_A()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            mem.PokeB(d, reg.a, vgmFrameCounter);
#if DEBUG
            nimo = string.Format("LD (${0:X04}),A", d);
#endif
            return insts[0xea].cycle[0];
        }


        int XOR_d8()
        {
            byte a = mem.PeekB(reg.pc);
            reg.pc++;
            reg.a = (byte)(reg.a ^ a);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = string.Format("XOR ${0:X02}", a);
#endif
            return insts[0xee].cycle[0];
        }


        int RST_28H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x28;
#if DEBUG
            nimo = "RST $28";
#endif
            return insts[0xef].cycle[0];
        }


        // 0xf0
        int LDH_A_pa8s()
        {
            byte p = mem.PeekB(reg.pc++);
            reg.a = mem.PeekB(0xff00 + p);
#if DEBUG
            nimo = string.Format("LD A,($FF00+${0:X02})", p);
#endif
            return insts[0xf0].cycle[0];
        }


        int POP_AF()
        {
            reg.af = pop();
            reg.af &= 0xfff0;
#if DEBUG
            nimo = "POP AF";
#endif
            return insts[0xf1].cycle[0];
        }


        int LD_A_pCs()
        {
            reg.a = mem.PeekB(0xff00 + reg.c);
#if DEBUG
            nimo = "LD A,($ff00+C)";
#endif
            return insts[0xf2].cycle[0];
        }


        int DI()
        {
            ime = 0;
#if DEBUG
            nimo = "DI";
#endif
            return insts[0xf3].cycle[0];
        }


        int PUSH_AF()
        {
            push(reg.af);
#if DEBUG
            nimo = "PUSH AF";
#endif
            return insts[0xf5].cycle[0];
        }


        int OR_d8()
        {
            byte d = mem.PeekB(reg.pc++);
            reg.a = (byte)(reg.a | d);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = false;

#if DEBUG
            nimo = string.Format("OR ${0:X02}", d);
#endif
            return insts[0xf6].cycle[0];
        }


        int RST_30H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x30;
#if DEBUG
            nimo = "RST $30";
#endif
            return insts[0xf7].cycle[0];
        }


        // 0xf8
        int LD_HL_SPplsr8()
        {
            int a = reg.sp;
            int b = (sbyte)mem.PeekB(reg.pc);
            reg.pc++;
            int c = a + b;
            int d = (a & 0xff) + b;

            reg.hl = (ushort)(c);

            reg.Z = false;
            reg.S = false;
            reg.H = (d & 0xf00) != 0;
            reg.C = (c & 0xf_0000) != 0;

#if DEBUG
            nimo = string.Format("LD HL,SP+${0:X02}", b);
#endif
            return insts[0xf8].cycle[0];
        }


        int LD_SP_HL()
        {
            reg.sp = reg.hl;
#if DEBUG
            nimo = "LD SP,HL";
#endif
            return insts[0xf9].cycle[0];
        }


        int LD_A_pa16s()
        {
            ushort d = mem.PeekW(reg.pc);
            reg.pc += 2;
            reg.a = mem.PeekB(d);
#if DEBUG
            nimo = string.Format("LD A,(${0:X04})", d);
#endif
            return insts[0xfa].cycle[0];
        }


        int EI()
        {
            ime = 1;
#if DEBUG
            nimo = "EI";
#endif
            return insts[0xfb].cycle[0];
        }


        int CP_d8()
        {
            int a = mem.PeekB(reg.pc);
            int b = reg.a - a;
            int h = (reg.a & 0xf) - (a & 0xf);

            //reg.a = (byte)b;
            reg.pc++;

            reg.Z = (b == 0);
            reg.S = true;
            reg.H = (h & 0xf0) != 0;
            reg.C = (b & 0xf00) != 0;

#if DEBUG
            nimo = string.Format("CP ${0:X02}", a);
#endif

            return insts[0xfe].cycle[0];
        }


        int RST_38H()
        {
            push((ushort)(reg.pc + 2));
            reg.pc = 0x38;
#if DEBUG
            nimo = "RST $38";
#endif
            return insts[0xff].cycle[0];
        }


        // 0x00 (CB)
        int RLC_B()
        {
            byte d = reg.b;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.b = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC B";
#endif

            return insts[0x100].cycle[0];
        }


        int RLC_C()
        {
            byte d = reg.c;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.c = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC C";
#endif

            return insts[0x101].cycle[0];
        }


        int RLC_D()
        {
            byte d = reg.d;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.d = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC D";
#endif

            return insts[0x102].cycle[0];
        }


        int RLC_E()
        {
            byte d = reg.e;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.e = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC E";
#endif

            return insts[0x103].cycle[0];
        }


        int RLC_H()
        {
            byte d = reg.h;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.h = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC H";
#endif

            return insts[0x104].cycle[0];
        }


        int RLC_L()
        {
            byte d = reg.l;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.l = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC L";
#endif

            return insts[0x105].cycle[0];
        }


        int RLC_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            mem.PokeB(reg.hl, d, vgmFrameCounter);

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC (HL)";
#endif

            return insts[0x106].cycle[0];
        }


        int RLC_A()
        {
            byte d = reg.a;
            reg.C = (d & 0x80) != 0;
            d = (byte)((d << 1) | (reg.C ? 1 : 0));
            reg.a = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RLC A";
#endif

            return insts[0x107].cycle[0];
        }


        // 0x08 (CB)
        int RRC_B()
        {
            byte d = reg.b;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.b = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC B";
#endif

            return insts[0x108].cycle[0];
        }


        int RRC_C()
        {
            byte d = reg.c;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.c = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC C";
#endif

            return insts[0x109].cycle[0];
        }


        int RRC_D()
        {
            byte d = reg.d;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.d = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC D";
#endif

            return insts[0x10a].cycle[0];
        }


        int RRC_E()
        {
            byte d = reg.e;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.e = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC E";
#endif

            return insts[0x10b].cycle[0];
        }


        int RRC_H()
        {
            byte d = reg.h;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.h = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC H";
#endif

            return insts[0x10c].cycle[0];
        }


        int RRC_L()
        {
            byte d = reg.l;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.l = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC L";
#endif

            return insts[0x10d].cycle[0];
        }


        int RRC_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            mem.PokeB(reg.hl, d,vgmFrameCounter);

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC (HL)";
#endif

            return insts[0x10e].cycle[0];
        }


        int RRC_A()
        {
            byte d = reg.a;
            reg.C = (d & 0x01) != 0;
            d = (byte)((d >> 1) | (reg.C ? 0x80 : 0));
            reg.a = d;

            reg.Z = (d == 0);
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RRC A";
#endif

            return insts[0x10f].cycle[0];
        }


        // 0x10 (CB)
        int RL_B()
        {
            byte d = reg.b;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.b = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL B";
#endif

            return insts[0x110].cycle[0];
        }


        int RL_C()
        {
            byte d = reg.c;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.c = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL C";
#endif

            return insts[0x111].cycle[0];
        }


        int RL_D()
        {
            byte d = reg.d;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.d = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL D";
#endif

            return insts[0x112].cycle[0];
        }


        int RL_E()
        {
            byte d = reg.e;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.e = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL E";
#endif

            return insts[0x113].cycle[0];
        }


        int RL_H()
        {
            byte d = reg.h;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.h = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL H";
#endif

            return insts[0x114].cycle[0];
        }


        int RL_L()
        {
            byte d = reg.l;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.l = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL L";
#endif

            return insts[0x115].cycle[0];
        }


        int RL_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            mem.PokeB(reg.hl, d, vgmFrameCounter);

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL (HL)";
#endif

            return insts[0x116].cycle[0];
        }


        int RL_A()
        {
            byte d = reg.a;
            byte e = (byte)(reg.C ? 0x01 : 0);
            reg.C = (d & 0x80) != 0;

            d = (byte)((d << 1) | e);
            reg.a = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RL A";
#endif

            return insts[0x117].cycle[0];
        }


        // 0x18 (CB)
        int RR_B()
        {
            byte d = reg.b;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.b = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR B";
#endif

            return insts[0x118].cycle[0];
        }


        int RR_C()
        {
            byte d = reg.c;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.c = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR C";
#endif

            return insts[0x119].cycle[0];
        }


        int RR_D()
        {
            byte d = reg.d;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.d = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR D";
#endif

            return insts[0x11a].cycle[0];
        }


        int RR_E()
        {
            byte d = reg.e;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.e = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR E";
#endif

            return insts[0x11b].cycle[0];
        }


        int RR_H()
        {
            byte d = reg.h;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.h = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR H";
#endif

            return insts[0x11c].cycle[0];
        }


        int RR_L()
        {
            byte d = reg.l;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.l = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR L";
#endif

            return insts[0x11d].cycle[0];
        }


        int RR_pHLs()
        {
            byte d = mem.PeekB(reg.hl);
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            mem.PokeB(reg.hl, d , vgmFrameCounter);

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR (HL)";
#endif

            return insts[0x11e].cycle[0];
        }


        int RR_A()
        {
            byte d = reg.a;
            byte e = (byte)(reg.C ? 0x80 : 0);
            reg.C = (d & 0x01) != 0;

            d = (byte)((d >> 1) | e);
            reg.a = d;

            reg.Z = false;
            reg.S = false;
            reg.H = false;

#if DEBUG
            nimo = "RR A";
#endif

            return insts[0x11f].cycle[0];
        }


        // 0x20 (CB)
        int SLA_B()
        {
            byte a = reg.b;
            reg.b = (byte)(a << 1);

            reg.Z = (reg.b == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA B";
#endif

            return insts[0x120].cycle[0];
        }


        int SLA_C()
        {
            byte a = reg.c;
            reg.c = (byte)(a << 1);

            reg.Z = (reg.c == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA C";
#endif

            return insts[0x121].cycle[0];
        }


        int SLA_D()
        {
            byte a = reg.d;
            reg.d = (byte)(a << 1);

            reg.Z = (reg.d == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA D";
#endif

            return insts[0x122].cycle[0];
        }


        int SLA_E()
        {
            byte a = reg.e;
            reg.e = (byte)(a << 1);

            reg.Z = (reg.e == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA E";
#endif

            return insts[0x123].cycle[0];
        }


        int SLA_H()
        {
            byte a = reg.h;
            reg.h = (byte)(a << 1);

            reg.Z = (reg.h == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA H";
#endif

            return insts[0x124].cycle[0];
        }


        int SLA_L()
        {
            byte a = reg.l;
            reg.l = (byte)(a << 1);

            reg.Z = (reg.l == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA L";
#endif

            return insts[0x125].cycle[0];
        }


        int SLA_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            byte b = (byte)(a << 1);
            mem.PokeB(reg.hl, b, vgmFrameCounter);

            reg.Z = (b == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA (HL)";
#endif

            return insts[0x126].cycle[0];
        }


        int SLA_A()
        {
            byte a = reg.a;
            reg.a = (byte)(a << 1);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x80) != 0;

#if DEBUG
            nimo = "SLA A";
#endif

            return insts[0x127].cycle[0];
        }


        // 0x28 (CB)
        int SRA_B()
        {
            byte a = reg.b;
            reg.b = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.b == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA B";
#endif

            return insts[0x128].cycle[0];
        }


        int SRA_C()
        {
            byte a = reg.c;
            reg.c = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.c == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA C";
#endif

            return insts[0x129].cycle[0];
        }


        int SRA_D()
        {
            byte a = reg.d;
            reg.d = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.d == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA D";
#endif

            return insts[0x12a].cycle[0];
        }


        int SRA_E()
        {
            byte a = reg.e;
            reg.e = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.e == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA E";
#endif

            return insts[0x12b].cycle[0];
        }


        int SRA_H()
        {
            byte a = reg.h;
            reg.h = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.h == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA H";
#endif

            return insts[0x12c].cycle[0];
        }


        int SRA_L()
        {
            byte a = reg.l;
            reg.l = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.l == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA L";
#endif

            return insts[0x12d].cycle[0];
        }


        int SRA_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            byte b = (byte)((a >> 1) | (a & 0x80));
            mem.PokeB(reg.hl, b, vgmFrameCounter);

            reg.Z = (b == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA (HL)";
#endif

            return insts[0x12e].cycle[0];
        }


        int SRA_A()
        {
            byte a = reg.a;
            reg.a = (byte)((a >> 1) | (a & 0x80));

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRA A";
#endif

            return insts[0x12f].cycle[0];
        }


        // 0x30 (CB)
        int SWAP_B()
        {
            byte a = reg.b;
            reg.b = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.b == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP B";
#endif
            return insts[0x130].cycle[0];
        }


        int SWAP_C()
        {
            byte a = reg.c;
            reg.c = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.c == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP C";
#endif
            return insts[0x131].cycle[0];
        }


        int SWAP_D()
        {
            byte a = reg.d;
            reg.d = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.d == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP D";
#endif
            return insts[0x132].cycle[0];
        }


        int SWAP_E()
        {
            byte a = reg.e;
            reg.e = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.e == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP E";
#endif
            return insts[0x133].cycle[0];
        }


        int SWAP_H()
        {
            byte a = reg.h;
            reg.h = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.h == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP H";
#endif
            return insts[0x134].cycle[0];
        }


        int SWAP_L()
        {
            byte a = reg.l;
            reg.l = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.l == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP L";
#endif
            return insts[0x135].cycle[0];
        }


        int SWAP_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            mem.PokeB(reg.hl, a, vgmFrameCounter);
            reg.Z = a == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP (HL)";
#endif
            return insts[0x136].cycle[0];
        }


        int SWAP_A()
        {
            byte a = reg.a;
            reg.a = (byte)(((a >> 4) & 0xf) | ((a << 4) & 0xf0));
            reg.Z = reg.a == 0;
            reg.S = false;
            reg.H = false;
            reg.C = false;
#if DEBUG
            nimo = "SWAP A";
#endif
            return insts[0x137].cycle[0];
        }


        // 0x38 (CB)
        int SRL_B()
        {
            byte a = reg.b;
            reg.b = (byte)(a >> 1);

            reg.Z = (reg.b == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL B";
#endif

            return insts[0x138].cycle[0];
        }


        int SRL_C()
        {
            byte a = reg.c;
            reg.c = (byte)(a >> 1);

            reg.Z = (reg.c == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL C";
#endif

            return insts[0x139].cycle[0];
        }


        int SRL_D()
        {
            byte a = reg.d;
            reg.d = (byte)(a >> 1);

            reg.Z = (reg.d == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL D";
#endif

            return insts[0x13a].cycle[0];
        }


        int SRL_E()
        {
            byte a = reg.e;
            reg.e = (byte)(a >> 1);

            reg.Z = (reg.e == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL E";
#endif

            return insts[0x13b].cycle[0];
        }


        int SRL_H()
        {
            byte a = reg.h;
            reg.h = (byte)(a >> 1);

            reg.Z = (reg.h == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL H";
#endif

            return insts[0x13c].cycle[0];
        }


        int SRL_L()
        {
            byte a = reg.l;
            reg.l = (byte)(a >> 1);

            reg.Z = (reg.l == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL L";
#endif

            return insts[0x13d].cycle[0];
        }


        int SRL_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            byte b = (byte)(a >> 1);
            mem.PokeB(reg.hl, b, vgmFrameCounter);

            reg.Z = (b == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL (HL)";
#endif

            return insts[0x13e].cycle[0];
        }


        int SRL_A()
        {
            byte a = reg.a;
            reg.a = (byte)(a >> 1);

            reg.Z = (reg.a == 0);
            reg.S = false;
            reg.H = false;
            reg.C = (a & 0x01) != 0;

#if DEBUG
            nimo = "SRL A";
#endif

            return insts[0x13f].cycle[0];
        }


        // 0x40 (CB)
        int BIT_0_B()
        {
            bool a = (reg.b & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,B";
#endif

            return insts[0x140].cycle[0];
        }


        int BIT_0_C()
        {
            bool a = (reg.c & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,C";
#endif

            return insts[0x141].cycle[0];
        }


        int BIT_0_D()
        {
            bool a = (reg.d & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,D";
#endif

            return insts[0x142].cycle[0];
        }


        int BIT_0_E()
        {
            bool a = (reg.e & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,E";
#endif

            return insts[0x143].cycle[0];
        }


        int BIT_0_H()
        {
            bool a = (reg.h & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,H";
#endif

            return insts[0x144].cycle[0];
        }


        int BIT_0_L()
        {
            bool a = (reg.l & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,L";
#endif

            return insts[0x145].cycle[0];
        }


        int BIT_0_pHLs()
        {
            bool a = (mem.PeekB(reg.hl) & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,(HL)";
#endif

            return insts[0x146].cycle[0];
        }


        int BIT_0_A()
        {
            bool a = (reg.a & 0x1) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 0,A";
#endif

            return insts[0x147].cycle[0];
        }


        // 0x48 (CB)
        int BIT_1_B()
        {
            bool a = (reg.b & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,B";
#endif

            return insts[0x148].cycle[0];
        }


        int BIT_1_C()
        {
            bool a = (reg.c & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,C";
#endif

            return insts[0x149].cycle[0];
        }


        int BIT_1_D()
        {
            bool a = (reg.d & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,D";
#endif

            return insts[0x14a].cycle[0];
        }


        int BIT_1_E()
        {
            bool a = (reg.e & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,E";
#endif

            return insts[0x14b].cycle[0];
        }


        int BIT_1_H()
        {
            bool a = (reg.h & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,H";
#endif

            return insts[0x14c].cycle[0];
        }


        int BIT_1_L()
        {
            bool a = (reg.l & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,L";
#endif

            return insts[0x14d].cycle[0];
        }


        int BIT_1_pHLs()
        {
            bool a = (mem.PeekB(reg.hl) & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,(HL)";
#endif

            return insts[0x14e].cycle[0];
        }


        int BIT_1_A()
        {
            bool a = (reg.a & 0x2) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 1,A";
#endif

            return insts[0x14f].cycle[0];
        }


        // 0x50 (CB)
        int BIT_2_B()
        {
            bool a = (reg.b & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,B";
#endif

            return insts[0x150].cycle[0];
        }


        int BIT_2_C()
        {
            bool a = (reg.c & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,C";
#endif

            return insts[0x151].cycle[0];
        }


        int BIT_2_D()
        {
            bool a = (reg.d & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,D";
#endif

            return insts[0x152].cycle[0];
        }


        int BIT_2_E()
        {
            bool a = (reg.e & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,E";
#endif

            return insts[0x153].cycle[0];
        }


        int BIT_2_H()
        {
            bool a = (reg.h & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,H";
#endif

            return insts[0x154].cycle[0];
        }


        int BIT_2_L()
        {
            bool a = (reg.l & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,L";
#endif

            return insts[0x155].cycle[0];
        }


        int BIT_2_pHLs()
        {
            bool a = (mem.PeekB(reg.hl) & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,(HL)";
#endif

            return insts[0x156].cycle[0];
        }


        int BIT_2_A()
        {
            bool a = (reg.a & 0x4) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 2,A";
#endif

            return insts[0x157].cycle[0];
        }


        // 0x58 (CB)
        int BIT_3_B()
        {
            bool a = (reg.b & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,B";
#endif

            return insts[0x158].cycle[0];
        }


        int BIT_3_C()
        {
            bool a = (reg.c & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,C";
#endif

            return insts[0x159].cycle[0];
        }


        int BIT_3_D()
        {
            bool a = (reg.d & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,D";
#endif

            return insts[0x15a].cycle[0];
        }


        int BIT_3_E()
        {
            bool a = (reg.e & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,E";
#endif

            return insts[0x15b].cycle[0];
        }


        int BIT_3_H()
        {
            bool a = (reg.h & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,H";
#endif

            return insts[0x15c].cycle[0];
        }


        int BIT_3_L()
        {
            bool a = (reg.l & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,L";
#endif

            return insts[0x15d].cycle[0];
        }


        int BIT_3_pHLs()
        {
            bool a = (mem.PeekB(reg.hl) & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,(HL)";
#endif

            return insts[0x15e].cycle[0];
        }


        int BIT_3_A()
        {
            bool a = (reg.a & 0x8) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 3,A";
#endif

            return insts[0x15f].cycle[0];
        }


        // 0x60 (CB)
        int BIT_4_B()
        {
            bool a = (reg.b & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,B";
#endif

            return insts[0x160].cycle[0];
        }


        int BIT_4_C()
        {
            bool a = (reg.c & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,C";
#endif

            return insts[0x161].cycle[0];
        }


        int BIT_4_D()
        {
            bool a = (reg.d & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,D";
#endif

            return insts[0x162].cycle[0];
        }


        int BIT_4_E()
        {
            bool a = (reg.e & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,E";
#endif

            return insts[0x163].cycle[0];
        }


        int BIT_4_H()
        {
            bool a = (reg.h & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,H";
#endif

            return insts[0x164].cycle[0];
        }


        int BIT_4_L()
        {
            bool a = (reg.l & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,L";
#endif

            return insts[0x165].cycle[0];
        }


        int BIT_4_pHLs()
        {
            bool a = (mem.PeekB(reg.hl) & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,(HL)";
#endif

            return insts[0x166].cycle[0];
        }


        int BIT_4_A()
        {
            bool a = (reg.a & 0x10) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 4,A";
#endif

            return insts[0x167].cycle[0];
        }


        // 0x68 (CB)
        int BIT_5_B()
        {
            bool a = (reg.b & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,B";
#endif

            return insts[0x168].cycle[0];
        }


        int BIT_5_C()
        {
            bool a = (reg.c & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,C";
#endif

            return insts[0x169].cycle[0];
        }


        int BIT_5_D()
        {
            bool a = (reg.d & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,D";
#endif

            return insts[0x16a].cycle[0];
        }


        int BIT_5_E()
        {
            bool a = (reg.e & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,E";
#endif

            return insts[0x16b].cycle[0];
        }


        int BIT_5_H()
        {
            bool a = (reg.h & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,H";
#endif

            return insts[0x16c].cycle[0];
        }


        int BIT_5_L()
        {
            bool a = (reg.l & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,L";
#endif

            return insts[0x16d].cycle[0];
        }


        int BIT_5_pHLs()
        {
            bool a = (mem.PeekB(reg.hl) & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,(HL)";
#endif

            return insts[0x16e].cycle[0];
        }


        int BIT_5_A()
        {
            bool a = (reg.a & 0x20) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 5,A";
#endif

            return insts[0x16f].cycle[0];
        }


        // 0x70 (CB)
        int BIT_6_B()
        {
            bool a = (reg.b & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,B";
#endif

            return insts[0x170].cycle[0];
        }


        int BIT_6_C()
        {
            bool a = (reg.c & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,C";
#endif

            return insts[0x171].cycle[0];
        }


        int BIT_6_D()
        {
            bool a = (reg.d & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,D";
#endif

            return insts[0x172].cycle[0];
        }


        int BIT_6_E()
        {
            bool a = (reg.e & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,E";
#endif

            return insts[0x173].cycle[0];
        }


        int BIT_6_H()
        {
            bool a = (reg.h & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,H";
#endif

            return insts[0x174].cycle[0];
        }


        int BIT_6_L()
        {
            bool a = (reg.l & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,L";
#endif

            return insts[0x175].cycle[0];
        }


        int BIT_6_pHLs()
        {
            byte b = mem.PeekB(reg.hl);
            bool a = (b & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,(HL)";
#endif

            return insts[0x176].cycle[0];
        }


        int BIT_6_A()
        {
            bool a = (reg.a & 0x40) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 6,A";
#endif

            return insts[0x177].cycle[0];
        }


        // 0x78 (CB)
        int BIT_7_B()
        {
            bool a = (reg.b & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,B";
#endif

            return insts[0x178].cycle[0];
        }


        int BIT_7_C()
        {
            bool a = (reg.c & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,C";
#endif

            return insts[0x179].cycle[0];
        }


        int BIT_7_D()
        {
            bool a = (reg.d & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,D";
#endif

            return insts[0x17a].cycle[0];
        }


        int BIT_7_E()
        {
            bool a = (reg.e & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,E";
#endif

            return insts[0x17b].cycle[0];
        }


        int BIT_7_H()
        {
            bool a = (reg.h & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,H";
#endif

            return insts[0x17c].cycle[0];
        }


        int BIT_7_L()
        {
            bool a = (reg.l & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,L";
#endif

            return insts[0x17d].cycle[0];
        }


        int BIT_7_pHLs()
        {
            byte b = mem.PeekB(reg.hl);
            bool a = (b & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,(HL)";
#endif

            return insts[0x17e].cycle[0];
        }


        int BIT_7_A()
        {
            bool a = (reg.a & 0x80) == 0;

            reg.Z = a;
            reg.S = false;
            reg.H = true;

#if DEBUG
            nimo = "BIT 7,A";
#endif

            return insts[0x17f].cycle[0];
        }


        // 0x80 (CB)
        int RES_0_B()
        {
            reg.b &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,B";
#endif
            return insts[0x180].cycle[0];
        }


        int RES_0_C()
        {
            reg.c &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,C";
#endif
            return insts[0x181].cycle[0];
        }


        int RES_0_D()
        {
            reg.d &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,D";
#endif
            return insts[0x182].cycle[0];
        }


        int RES_0_E()
        {
            reg.e &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,E";
#endif
            return insts[0x183].cycle[0];
        }


        int RES_0_H()
        {
            reg.h &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,H";
#endif
            return insts[0x184].cycle[0];
        }


        int RES_0_L()
        {
            reg.l &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,L";
#endif
            return insts[0x185].cycle[0];
        }


        int RES_0_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 0));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 0,(HL)";
#endif
            return insts[0x186].cycle[0];
        }


        int RES_0_A()
        {
            reg.a &= unchecked((byte)~(1 << 0));

#if DEBUG
            nimo = "RES 0,A";
#endif
            return insts[0x187].cycle[0];
        }


        // 0x88 (CB)
        int RES_1_B()
        {
            reg.b &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,B";
#endif
            return insts[0x188].cycle[0];
        }


        int RES_1_C()
        {
            reg.c &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,C";
#endif
            return insts[0x189].cycle[0];
        }


        int RES_1_D()
        {
            reg.d &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,D";
#endif
            return insts[0x18a].cycle[0];
        }


        int RES_1_E()
        {
            reg.e &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,E";
#endif
            return insts[0x18b].cycle[0];
        }


        int RES_1_H()
        {
            reg.h &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,H";
#endif
            return insts[0x18c].cycle[0];
        }


        int RES_1_L()
        {
            reg.l &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,L";
#endif
            return insts[0x18d].cycle[0];
        }


        int RES_1_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 1));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 1,(HL)";
#endif
            return insts[0x18e].cycle[0];
        }


        int RES_1_A()
        {
            reg.a &= unchecked((byte)~(1 << 1));

#if DEBUG
            nimo = "RES 1,A";
#endif
            return insts[0x18f].cycle[0];
        }


        // 0x90 (CB)
        int RES_2_B()
        {
            reg.b &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,B";
#endif
            return insts[0x190].cycle[0];
        }


        int RES_2_C()
        {
            reg.c &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,C";
#endif
            return insts[0x191].cycle[0];
        }


        int RES_2_D()
        {
            reg.d &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,D";
#endif
            return insts[0x192].cycle[0];
        }


        int RES_2_E()
        {
            reg.e &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,E";
#endif
            return insts[0x193].cycle[0];
        }


        int RES_2_H()
        {
            reg.h &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,H";
#endif
            return insts[0x194].cycle[0];
        }


        int RES_2_L()
        {
            reg.l &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,L";
#endif
            return insts[0x195].cycle[0];
        }


        int RES_2_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 2));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 2,(HL)";
#endif
            return insts[0x196].cycle[0];
        }


        int RES_2_A()
        {
            reg.a &= unchecked((byte)~(1 << 2));

#if DEBUG
            nimo = "RES 2,A";
#endif
            return insts[0x197].cycle[0];
        }


        // 0x98 (CB)
        int RES_3_B()
        {
            reg.b &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,B";
#endif
            return insts[0x198].cycle[0];
        }


        int RES_3_C()
        {
            reg.c &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,C";
#endif
            return insts[0x199].cycle[0];
        }


        int RES_3_D()
        {
            reg.d &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,D";
#endif
            return insts[0x19a].cycle[0];
        }


        int RES_3_E()
        {
            reg.e &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,E";
#endif
            return insts[0x19b].cycle[0];
        }


        int RES_3_H()
        {
            reg.h &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,H";
#endif
            return insts[0x19c].cycle[0];
        }


        int RES_3_L()
        {
            reg.l &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,L";
#endif
            return insts[0x19d].cycle[0];
        }


        int RES_3_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 3));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 3,(HL)";
#endif
            return insts[0x19e].cycle[0];
        }


        int RES_3_A()
        {
            reg.a &= unchecked((byte)~(1 << 3));

#if DEBUG
            nimo = "RES 3,A";
#endif
            return insts[0x19f].cycle[0];
        }


        // 0xa0 (CB)
        int RES_4_B()
        {
            reg.b &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,B";
#endif
            return insts[0x1a0].cycle[0];
        }


        int RES_4_C()
        {
            reg.c &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,C";
#endif
            return insts[0x1a1].cycle[0];
        }


        int RES_4_D()
        {
            reg.d &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,D";
#endif
            return insts[0x1a2].cycle[0];
        }


        int RES_4_E()
        {
            reg.e &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,E";
#endif
            return insts[0x1a3].cycle[0];
        }


        int RES_4_H()
        {
            reg.h &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,H";
#endif
            return insts[0x1a4].cycle[0];
        }


        int RES_4_L()
        {
            reg.l &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,L";
#endif
            return insts[0x1a5].cycle[0];
        }


        int RES_4_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 4));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 4,(HL)";
#endif
            return insts[0x1a6].cycle[0];
        }


        int RES_4_A()
        {
            reg.a &= unchecked((byte)~(1 << 4));

#if DEBUG
            nimo = "RES 4,A";
#endif
            return insts[0x1a7].cycle[0];
        }


        // 0xa8 (CB)
        int RES_5_B()
        {
            reg.b &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,B";
#endif
            return insts[0x1a8].cycle[0];
        }


        int RES_5_C()
        {
            reg.c &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,C";
#endif
            return insts[0x1a9].cycle[0];
        }


        int RES_5_D()
        {
            reg.d &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,D";
#endif
            return insts[0x1aa].cycle[0];
        }


        int RES_5_E()
        {
            reg.e &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,E";
#endif
            return insts[0x1ab].cycle[0];
        }


        int RES_5_H()
        {
            reg.h &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,H";
#endif
            return insts[0x1ac].cycle[0];
        }


        int RES_5_L()
        {
            reg.l &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,L";
#endif
            return insts[0x1ad].cycle[0];
        }


        int RES_5_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 5));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 5,(HL)";
#endif
            return insts[0x1ae].cycle[0];
        }


        int RES_5_A()
        {
            reg.a &= unchecked((byte)~(1 << 5));

#if DEBUG
            nimo = "RES 5,A";
#endif
            return insts[0x1af].cycle[0];
        }


        // 0xb0 (CB)
        int RES_6_B()
        {
            reg.b &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,B";
#endif
            return insts[0x1b0].cycle[0];
        }


        int RES_6_C()
        {
            reg.c &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,C";
#endif
            return insts[0x1b1].cycle[0];
        }


        int RES_6_D()
        {
            reg.d &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,D";
#endif
            return insts[0x1b2].cycle[0];
        }


        int RES_6_E()
        {
            reg.e &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,E";
#endif
            return insts[0x1b3].cycle[0];
        }


        int RES_6_H()
        {
            reg.h &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,H";
#endif
            return insts[0x1b4].cycle[0];
        }


        int RES_6_L()
        {
            reg.l &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,L";
#endif
            return insts[0x1b5].cycle[0];
        }


        int RES_6_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 6));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 6,(HL)";
#endif
            return insts[0x1b6].cycle[0];
        }


        int RES_6_A()
        {
            reg.a &= unchecked((byte)~(1 << 6));

#if DEBUG
            nimo = "RES 6,A";
#endif
            return insts[0x1b7].cycle[0];
        }


        // 0xb8 (CB)
        int RES_7_B()
        {
            reg.b &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,B";
#endif
            return insts[0x1b8].cycle[0];
        }


        int RES_7_C()
        {
            reg.c &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,C";
#endif
            return insts[0x1b9].cycle[0];
        }


        int RES_7_D()
        {
            reg.d &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,D";
#endif
            return insts[0x1ba].cycle[0];
        }


        int RES_7_E()
        {
            reg.e &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,E";
#endif
            return insts[0x1bb].cycle[0];
        }


        int RES_7_H()
        {
            reg.h &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,H";
#endif
            return insts[0x1bc].cycle[0];
        }


        int RES_7_L()
        {
            reg.l &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,L";
#endif
            return insts[0x1bd].cycle[0];
        }


        int RES_7_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a &= unchecked((byte)~(1 << 7));
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "RES 7,(HL)";
#endif
            return insts[0x1be].cycle[0];
        }


        int RES_7_A()
        {
            reg.a &= unchecked((byte)~(1 << 7));

#if DEBUG
            nimo = "RES 7,A";
#endif
            return insts[0x1bf].cycle[0];
        }


        // 0xc0 (CB)
        int SET_0_B()
        {
            reg.b |= (1 << 0);

#if DEBUG
            nimo = "SET 0,B";
#endif
            return insts[0x1c0].cycle[0];
        }


        int SET_0_C()
        {
            reg.c |= (1 << 0);

#if DEBUG
            nimo = "SET 0,C";
#endif
            return insts[0x1c1].cycle[0];
        }


        int SET_0_D()
        {
            reg.d |= (1 << 0);

#if DEBUG
            nimo = "SET 0,D";
#endif
            return insts[0x1c2].cycle[0];
        }


        int SET_0_E()
        {
            reg.e |= (1 << 0);

#if DEBUG
            nimo = "SET 0,E";
#endif
            return insts[0x1c3].cycle[0];
        }


        int SET_0_H()
        {
            reg.h |= (1 << 0);

#if DEBUG
            nimo = "SET 0,H";
#endif
            return insts[0x1c4].cycle[0];
        }


        int SET_0_L()
        {
            reg.l |= (1 << 0);

#if DEBUG
            nimo = "SET 0,L";
#endif
            return insts[0x1c5].cycle[0];
        }


        int SET_0_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 0);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 0,(HL)";
#endif
            return insts[0x1c6].cycle[0];
        }


        int SET_0_A()
        {
            reg.a |= (1 << 0);

#if DEBUG
            nimo = "SET 0,A";
#endif
            return insts[0x1c7].cycle[0];
        }


        // 0xc8 (CB)
        int SET_1_B()
        {
            reg.b |= (1 << 1);

#if DEBUG
            nimo = "SET 1,B";
#endif
            return insts[0x1c8].cycle[0];
        }


        int SET_1_C()
        {
            reg.c |= (1 << 1);

#if DEBUG
            nimo = "SET 1,C";
#endif
            return insts[0x1c9].cycle[0];
        }


        int SET_1_D()
        {
            reg.d |= (1 << 1);

#if DEBUG
            nimo = "SET 1,D";
#endif
            return insts[0x1ca].cycle[0];
        }


        int SET_1_E()
        {
            reg.e |= (1 << 1);

#if DEBUG
            nimo = "SET 1,E";
#endif
            return insts[0x1cb].cycle[0];
        }


        int SET_1_H()
        {
            reg.h |= (1 << 1);

#if DEBUG
            nimo = "SET 1,H";
#endif
            return insts[0x1cc].cycle[0];
        }


        int SET_1_L()
        {
            reg.l |= (1 << 1);

#if DEBUG
            nimo = "SET 1,L";
#endif
            return insts[0x1cd].cycle[0];
        }


        int SET_1_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 1);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 1,(HL)";
#endif
            return insts[0x1ce].cycle[0];
        }


        int SET_1_A()
        {
            reg.a |= (1 << 1);

#if DEBUG
            nimo = "SET 1,A";
#endif
            return insts[0x1cf].cycle[0];
        }


        // 0xd0 (CB)
        int SET_2_B()
        {
            reg.b |= (1 << 2);

#if DEBUG
            nimo = "SET 2,B";
#endif
            return insts[0x1d0].cycle[0];
        }


        int SET_2_C()
        {
            reg.c |= (1 << 2);

#if DEBUG
            nimo = "SET 2,C";
#endif
            return insts[0x1d1].cycle[0];
        }


        int SET_2_D()
        {
            reg.d |= (1 << 2);

#if DEBUG
            nimo = "SET 2,D";
#endif
            return insts[0x1d2].cycle[0];
        }


        int SET_2_E()
        {
            reg.e |= (1 << 2);

#if DEBUG
            nimo = "SET 2,E";
#endif
            return insts[0x1d3].cycle[0];
        }


        int SET_2_H()
        {
            reg.h |= (1 << 2);

#if DEBUG
            nimo = "SET 2,H";
#endif
            return insts[0x1d4].cycle[0];
        }


        int SET_2_L()
        {
            reg.l |= (1 << 2);

#if DEBUG
            nimo = "SET 2,L";
#endif
            return insts[0x1d5].cycle[0];
        }


        int SET_2_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 2);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 2,(HL)";
#endif
            return insts[0x1d6].cycle[0];
        }


        int SET_2_A()
        {
            reg.a |= (1 << 2);

#if DEBUG
            nimo = "SET 2,A";
#endif
            return insts[0x1d7].cycle[0];
        }


        // 0xd8 (CB)
        int SET_3_B()
        {
            reg.b |= (1 << 3);

#if DEBUG
            nimo = "SET 3,B";
#endif
            return insts[0x1d8].cycle[0];
        }


        int SET_3_C()
        {
            reg.c |= (1 << 3);

#if DEBUG
            nimo = "SET 3,C";
#endif
            return insts[0x1d9].cycle[0];
        }


        int SET_3_D()
        {
            reg.d |= (1 << 3);

#if DEBUG
            nimo = "SET 3,D";
#endif
            return insts[0x1da].cycle[0];
        }


        int SET_3_E()
        {
            reg.e |= (1 << 3);

#if DEBUG
            nimo = "SET 3,E";
#endif
            return insts[0x1db].cycle[0];
        }


        int SET_3_H()
        {
            reg.h |= (1 << 3);

#if DEBUG
            nimo = "SET 3,H";
#endif
            return insts[0x1dc].cycle[0];
        }


        int SET_3_L()
        {
            reg.l |= (1 << 3);

#if DEBUG
            nimo = "SET 3,L";
#endif
            return insts[0x1dd].cycle[0];
        }


        int SET_3_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 3);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 3,(HL)";
#endif
            return insts[0x1de].cycle[0];
        }


        int SET_3_A()
        {
            reg.a |= (1 << 3);

#if DEBUG
            nimo = "SET 3,A";
#endif
            return insts[0x1df].cycle[0];
        }


        // 0xe0 (CB)
        int SET_4_B()
        {
            reg.b |= (1 << 4);

#if DEBUG
            nimo = "SET 4,B";
#endif
            return insts[0x1e0].cycle[0];
        }


        int SET_4_C()
        {
            reg.c |= (1 << 4);

#if DEBUG
            nimo = "SET 4,C";
#endif
            return insts[0x1e1].cycle[0];
        }


        int SET_4_D()
        {
            reg.d |= (1 << 4);

#if DEBUG
            nimo = "SET 4,D";
#endif
            return insts[0x1e2].cycle[0];
        }


        int SET_4_E()
        {
            reg.e |= (1 << 4);

#if DEBUG
            nimo = "SET 4,E";
#endif
            return insts[0x1e3].cycle[0];
        }


        int SET_4_H()
        {
            reg.h |= (1 << 4);

#if DEBUG
            nimo = "SET 4,H";
#endif
            return insts[0x1e4].cycle[0];
        }


        int SET_4_L()
        {
            reg.l |= (1 << 4);

#if DEBUG
            nimo = "SET 4,L";
#endif
            return insts[0x1e5].cycle[0];
        }


        int SET_4_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 4);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 4,(HL)";
#endif
            return insts[0x1e6].cycle[0];
        }


        int SET_4_A()
        {
            reg.a |= (1 << 4);

#if DEBUG
            nimo = "SET 4,A";
#endif
            return insts[0x1e7].cycle[0];
        }


        // 0xe8 (CB)
        int SET_5_B()
        {
            reg.b |= (1 << 5);

#if DEBUG
            nimo = "SET 5,B";
#endif
            return insts[0x1e8].cycle[0];
        }


        int SET_5_C()
        {
            reg.c |= (1 << 5);

#if DEBUG
            nimo = "SET 5,C";
#endif
            return insts[0x1e9].cycle[0];
        }


        int SET_5_D()
        {
            reg.d |= (1 << 5);

#if DEBUG
            nimo = "SET 5,D";
#endif
            return insts[0x1ea].cycle[0];
        }


        int SET_5_E()
        {
            reg.e |= (1 << 5);

#if DEBUG
            nimo = "SET 5,E";
#endif
            return insts[0x1eb].cycle[0];
        }


        int SET_5_H()
        {
            reg.h |= (1 << 5);

#if DEBUG
            nimo = "SET 5,H";
#endif
            return insts[0x1ec].cycle[0];
        }


        int SET_5_L()
        {
            reg.l |= (1 << 5);

#if DEBUG
            nimo = "SET 5,L";
#endif
            return insts[0x1ed].cycle[0];
        }


        int SET_5_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 5);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 5,(HL)";
#endif
            return insts[0x1ee].cycle[0];
        }


        int SET_5_A()
        {
            reg.a |= (1 << 5);

#if DEBUG
            nimo = "SET 5,A";
#endif
            return insts[0x1ef].cycle[0];
        }


        // 0xf0 (CB)
        int SET_6_B()
        {
            reg.b |= (1 << 6);

#if DEBUG
            nimo = "SET 6,B";
#endif
            return insts[0x1f0].cycle[0];
        }


        int SET_6_C()
        {
            reg.c |= (1 << 6);

#if DEBUG
            nimo = "SET 6,C";
#endif
            return insts[0x1f1].cycle[0];
        }


        int SET_6_D()
        {
            reg.d |= (1 << 6);

#if DEBUG
            nimo = "SET 6,D";
#endif
            return insts[0x1f2].cycle[0];
        }


        int SET_6_E()
        {
            reg.e |= (1 << 6);

#if DEBUG
            nimo = "SET 6,E";
#endif
            return insts[0x1f3].cycle[0];
        }


        int SET_6_H()
        {
            reg.h |= (1 << 6);

#if DEBUG
            nimo = "SET 6,H";
#endif
            return insts[0x1f4].cycle[0];
        }


        int SET_6_L()
        {
            reg.l |= (1 << 6);

#if DEBUG
            nimo = "SET 6,L";
#endif
            return insts[0x1f5].cycle[0];
        }


        int SET_6_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 6);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 6,(HL)";
#endif
            return insts[0x1f6].cycle[0];
        }


        int SET_6_A()
        {
            reg.a |= (1 << 6);

#if DEBUG
            nimo = "SET 6,A";
#endif
            return insts[0x1f7].cycle[0];
        }


        // 0xf8 (CB)
        int SET_7_B()
        {
            reg.b |= (1 << 7);

#if DEBUG
            nimo = "SET 7,B";
#endif
            return insts[0x1f8].cycle[0];
        }


        int SET_7_C()
        {
            reg.c |= (1 << 7);

#if DEBUG
            nimo = "SET 7,C";
#endif
            return insts[0x1f9].cycle[0];
        }


        int SET_7_D()
        {
            reg.d |= (1 << 7);

#if DEBUG
            nimo = "SET 7,D";
#endif
            return insts[0x1fa].cycle[0];
        }


        int SET_7_E()
        {
            reg.e |= (1 << 7);

#if DEBUG
            nimo = "SET 7,E";
#endif
            return insts[0x1fb].cycle[0];
        }


        int SET_7_H()
        {
            reg.h |= (1 << 7);

#if DEBUG
            nimo = "SET 7,H";
#endif
            return insts[0x1fc].cycle[0];
        }


        int SET_7_L()
        {
            reg.l |= (1 << 7);

#if DEBUG
            nimo = "SET 7,L";
#endif
            return insts[0x1fd].cycle[0];
        }


        int SET_7_pHLs()
        {
            byte a = mem.PeekB(reg.hl);
            a |= (1 << 7);
            mem.PokeB(reg.hl, a, vgmFrameCounter);

#if DEBUG
            nimo = "SET 7,(HL)";
#endif
            return insts[0x1fe].cycle[0];
        }


        int SET_7_A()
        {
            reg.a |= (1 << 7);

#if DEBUG
            nimo = "SET 7,A";
#endif
            return insts[0x1ff].cycle[0];
        }














        private void push(ushort dat)
        {
            reg.sp -= 2;
            mem.PokeW(reg.sp, dat, vgmFrameCounter);
        }

        private ushort pop()
        {
            ushort dat = mem.PeekW(reg.sp);
            reg.sp += 2;
            return dat;
        }

    }
}
