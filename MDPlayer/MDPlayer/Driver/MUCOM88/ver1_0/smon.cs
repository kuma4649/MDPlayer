using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88.ver1_0
{
    public class smon
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;
        public ver1_1.msub msub = null;
        public ver1_1.expand expand = null;

        //N88BASIC EXPAND COMMAND PROGRAM

        //ORG	0DE00H

        //public const int MWRITE = 0x9000;
        //public const int MWRIT2 = MWRITE + 3;
        //public const int ERRT = MWRIT2 + 3;
        //public const int ERRORSN = ERRT + 3;
        //public const int ERRORIF = ERRORSN + 3;
        //public const int ERRORNF = ERRORIF + 3;
        //public const int ERRORFN = ERRORNF + 3;
        //public const int ERRORVF = ERRORFN + 3;
        //public const int ERROROO = ERRORVF + 3;
        //public const int ERRORND = ERROROO + 3;
        //public const int ERRORRJ = ERRORND + 3;
        //public const int STTONE = ERRORRJ + 3;
        //public const int STLIZM = STTONE + 3;
        //public const int REDATA = STLIZM + 3;
        //public const int MULT = REDATA + 3;
        //public const int DIV = MULT + 3;
        //public const int HEXDEC = DIV + 3;
        //public const int HEXPRT = HEXDEC + 3;
        //public const int ROM = HEXPRT + 3;
        //public const int RAM = ROM + 3;
        //public const int FMCOMC = RAM + 3;
        //public const int T_RST = FMCOMC + 3;
        //public const int ERRORNE = T_RST + 3;
        //public const int ERRORDC = ERRORNE + 3;
        //public const int ERRORML = ERRORDC + 3;
        //public const int MCMP = ERRORML + 3;
        //public const int ERRORVO = MCMP + 3;
        public const int FMDAT = 0x08B00;
        public const int OTOWK = 0x8B00 + 0x60 + 0x60;
        public const int PARAM = OTOWK + 32;

        // -- CLEAR FROM COMPI1	-->

        public const int T_CLK = 0x08C10;
        public const int UDFLG = T_CLK + 4 * 11;
        public const int BEFMD = UDFLG + 1;
        public const int PTMFG = BEFMD + 2;
        public const int PTMDLY = PTMFG + 1;
        public const int TONEADR = PTMDLY + 2;
        public const int SPACE = TONEADR + 2;	//2*6BYTE ｱｷ ｶﾞ ｱﾙ
        public const int DEFVOICE = SPACE + 2 * 6;
        public const int DEFVSSG = DEFVOICE + 32;
        public const int JCLOCK = DEFVSSG + 32;
        public const int JPLINE = JCLOCK + 2;

        //-<

        public const int MSTART = 0x0B000;
        public const int MSTOP = MSTART + 3;
        public const int MUSIC = MSTART + 3 * 6;
        public const int DRIVE = MUSIC + 3 * 5;
        public const int WKGET = MUSIC + 3 * 8;

        public const int MUC88 = 0x09600;

        public const int CLS1 = 0x5F0E;
        public const int SCEDIT = 0x5F92;	//ｽｸﾘｰﾝ ｴﾃﾞｨｯﾄ
        public const int STOPKC = 0x35C2;	//ｽﾄｯﾌﾟｷｰ ﾁｪｯｸ
        public const int BUFCLR = 0x35D9;	//ｷｰﾊﾞｯﾌｧｸﾘｱ
        public const int DSPMSG = 0x0AB00;
        public const int KEYCHK = DSPMSG + 3 * 5;

        //    JP PRNWK
        //    JP PRNWKI
        //    JP CONVERT
        //    JP CONVERT2
        //    JP GETPARA
        //    JP OPEX

        public void PRNWK()
        {
            byte[] buf = Encoding.UTF8.GetBytes(CHCHR);

            Z80.B = 11;
            Z80.A = 0x41;// 'A';
            //Mem.LD_8(CHCHR, Z80.A);
            CHCHR = "A";
            Z80.A = 1;
            //Mem.LD_8(CHNUM, Z80.A);
            CHNUM = 1;
            Z80.HL = 0x104;
            Mem.LD_16(XY, Z80.HL);
        PRNWK2:
            Mem.stack.Push(Z80.BC);
            Z80.HL = Mem.LD_16(XY);
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = 0;// CHCHR;
            Z80.LDI(buf);
            Z80.HL--;
            //Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) + 1));
            buf[0]++;
            Z80.A = Mem.LD_8(CHNUM);
            Z80.B = Z80.A;
            PC88.CALL(WKGET);
            Z80.A = 3;
            LOC();
            Z80.Zero = (Mem.LD_8((ushort)(Z80.IX + 31)) & 0x08) == 0;
            ONOFF();
            Z80.C = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.A = Mem.LD_8(CHNUM);
            if (Z80.A - 7 == 0)
            {
                goto PRNW4;
            }
            if (Z80.A - 11 == 0)
            {
                goto PRNW4;
            }
            Z80.A -= 4;
            if (Z80.A - 3 < 0)
            {
                goto PRNW3;
            }
            Z80.A = Z80.C;
            Z80.A -= 4;
            Z80.C = Z80.A;
            goto PRNW4;
        PRNW3:
            Z80.A = Z80.C;
            Z80.A &= 0b0000_1111;
            Z80.C = Z80.A;
        PRNW4:
            Z80.A = 7;
            LOC();
            Z80.EX_DE_HL();
            Z80.L = Z80.C;
            PRNSUB();
            Z80.A = 36;
            LOC();
            Z80.EX_DE_HL();
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 18));
            PRNSUB();
            Z80.A = 11;
            LOC();
            Z80.EX_DE_HL();
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 9));
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 10));
            PRNS1();
            Z80.A = 17;
            LOC();
            Z80.EX_DE_HL();
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 2));
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 3));
            PRNS2();
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 32));
            Mem.stack.Push(Z80.AF);
            Z80.A &= 0x0F0;
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.A++;
            Z80.A += 0x30;
            Mem.stack.Push(Z80.AF);
            Z80.A = 22;
            LOC();
            PC88.CALL(0x429D);
            Z80.AF = Mem.stack.Pop();
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.AF = Mem.stack.Pop();
            Z80.A &= 0x0F;
            Z80.HL = 0;// KEYD;
            Z80.A += Z80.A;
            Z80.D = 0;
            Z80.E = Z80.A;
            Z80.HL += Z80.DE;
            Mem.stack.Push(Z80.HL);
            Z80.A = 23;
            LOC();
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            byte[] buf1 = Encoding.UTF8.GetBytes(KEYD[Z80.HL / 2]);
            Z80.HL = 0;
            Z80.LDI();
            Z80.LDI();
            Z80.A = 26;
            LOC();
            Z80.Zero = (Mem.LD_8((ushort)(Z80.IX + 31)) & 0x80) == 0;
            ONOFF();
            Z80.A = 30;
            LOC();
            Z80.Zero = (Mem.LD_8((ushort)(Z80.IX + 33)) & 0x20) == 0;
            ONOFF();
            Z80.B = 1;
            PC88.CALL(WKGET);
            Mem.stack.Push(Z80.IX);
            Z80.HL = Mem.stack.Pop();
            Z80.DE = 8;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;// L/R DATA AREA
            Z80.A = Mem.LD_8(CHNUM);
            Z80.A--;
            if (Z80.A - 3 < 0)
            {
                goto PRNW5;
            }
            Z80.A -= 3;
            if (Z80.A - 4 < 0)
            {
                goto PRNW6;
            }
            Z80.A -= 4;
            Z80.A += 4;
        PRNW5:
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(CHNUM);
            if (Z80.A - 11 != 0)
            {
                goto PRNW50;
            }
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.RRCA();
            Z80.RRCA();
            goto PRNW51;
        PRNW50:
            Z80.A = Mem.LD_8(Z80.HL);
        PRNW51:
            Z80.A &= 0b1100_0000;
            if (Z80.A == 0)
            {
                goto PRNW6;
            }
            if (Z80.A - 0xc0 == 0)
            {
                goto PRNW6;
            }
            Z80.Zero = (Z80.A & 0x80) == 0;
            if (!Z80.Zero)
            {
                goto PRNW52;
            }
            Z80.A = 0x52;// 'R';
            goto PRNW7;
        PRNW52:
            Z80.A = 0x4c;// 'L';
            goto PRNW7;
        PRNW6:
            Z80.A = 0x43;// 'C';
        PRNW7:
            Mem.stack.Push(Z80.AF);
            Z80.A = 34;
            LOC();
            PC88.CALL(0x429D);
            Z80.AF = Mem.stack.Pop();
            Mem.LD_8(Z80.HL, Z80.A);

            // --	MUTE CHECK	--

            Z80.BC = 0;
            //Z80.HL = BEFK;
            expand.KEYCHK(ref BEFK[0]);// PC88.CALL(KEYCHK);
            if (!Z80.Zero)
            {
                goto PRNW71;
            }
            Z80.BC = 0xf801;
            //Z80.HL = BEFK + 1;
            expand.KEYCHK(ref BEFK[1]); //PC88.CALL(KEYCHK);
            if (Z80.Zero)
            {
                goto PRNW9;
            }
            Z80.C = 9;
            goto PRNW72;
        PRNW71:
            Z80.C = 1;
        PRNW72:
            Z80.B = 8;
        //PRNW73:
            do
            {
                Z80.RRCA();
                if (!Z80.Carry)
                {
                    goto PRNW74;
                }
                Z80.C++;
                Z80.B--;
            } while (Z80.B != 0);
            goto PRNW9;
        PRNW74:
            Z80.B = Z80.C;
            PC88.CALL(WKGET);
            Z80.Zero = (Mem.LD_8((ushort)(Z80.IX + 31)) & 0x08) == 0;
            if (!Z80.Zero)
            {
                goto PRNW8;
            }
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x08));
            goto PRNW9;
        PRNW8:
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xf7));
        PRNW9:
            Z80.DE = Mem.LD_16(XY);
            Z80.E++;
            Mem.LD_16(XY, Z80.DE);
            Z80.A = Mem.LD_8(CHNUM);
            Z80.A++;
            Mem.LD_8(CHNUM, Z80.A);
            Z80.BC = Mem.stack.Pop();
            Z80.B--;
            if (Z80.B != 0)
            {
                goto PRNWK2;
            }

            // --	RETKEY CHCEK	--

            Z80.HL = RETK;
            Z80.BC = 0x7f01;
            PC88.CALL(KEYCHK);
            if (Z80.Zero)
            {
                goto PRNWA;
            }
            Z80.Zero = (Z80.A & 0x08) == 0;
            if (!Z80.Zero)
            {
                goto PRNWA;
            }
            //    DI
            PC88.CALL(DRIVE);
        //    EI

        // --	F5 CHECK	--

        PRNWA:
            Z80.HL = F5K;
            Z80.BC = 0xdf09;
            PC88.CALL(KEYCHK);
            if (Z80.Zero)
            {
                goto PRNWB;
            }
            Z80.Zero = (Z80.A & 0x20) == 0;
            if (!Z80.Zero)
            {
                goto PRNWB;
            }
            Z80.A ^= Z80.A;
            PC88.CALL(MSTART);
        PRNWB:
            FMPUT();

            // --	ｵﾝｼｮｸﾅﾝﾊﾞｰ ﾋｮｳｼﾞ	--

            Z80.A = 1;
            Z80.B = 6;
            Z80.DE = 6 * 256 + 17;
        //OPUT0:
            do
            {
                Mem.stack.Push(Z80.BC);
                Mem.stack.Push(Z80.AF);
                if (Z80.A - 4 != 0)
                {
                    goto OPUT1;
                }
                Z80.AF = Mem.stack.Pop();
                Z80.A += 4;
                Mem.stack.Push(Z80.AF);
            OPUT1:
                Z80.B = Z80.A;
                PC88.CALL(WKGET);
                Z80.C = Mem.LD_8((ushort)(Z80.IX + 1));
                // Z80.C--;
                Z80.B = 0;
                Z80.HL = DEFVOICE;
                Z80.HL += Z80.BC;
                Z80.L = Mem.LD_8(Z80.HL);
                Z80.L--;
                Mem.stack.Push(Z80.DE);
                PRNSUB();
                Z80.DE = Mem.stack.Pop();
                Z80.A = Z80.D;
                Z80.A += 13;
                Z80.D = Z80.A;
                Z80.AF = Mem.stack.Pop();
                Z80.A++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        public void PRNSUB()    // IN:DE<=X,Y:HL<=DATA(10ｼﾝ)
        {
            Z80.H = 0;
            msub.HEXDEC();
            Z80.HL++;
            Z80.HL++;
            Z80.EX_DE_HL();
            Mem.stack.Push(Z80.DE);
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.LDI();
            Z80.LDI();
            Z80.LDI();
            //    RET
        }

        public void PRNS1()
        {
            msub.HEXDEC();
            Z80.EX_DE_HL();
            Mem.stack.Push(Z80.DE);
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.LDI();
            Z80.LDI();
            Z80.LDI();
            Z80.LDI();
            Z80.LDI();
            //    RET
        }

        public void PRNS2()         // (16ｼﾝ)
        {
            msub.HEXPRT();
            Z80.EX_DE_HL();
            Mem.stack.Push(Z80.DE);
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.LDI();
            Z80.LDI();
            Z80.LDI();
            Z80.LDI();
            //    RET
        }

        public void PRNS3()
        {
            Z80.H = 0;
            msub.HEXPRT();
            Z80.EX_DE_HL();
            Mem.stack.Push(Z80.DE);
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            Z80.HL++;
            Z80.LDI();
            Z80.LDI();
            //    RET
        }

        public void ONOFF()
        {
            if (!Z80.Zero)
            {
                goto ONO2;
            }
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = 0;// OFFD;
            byte[] buf = Encoding.UTF8.GetBytes(OFFD);
            Z80.LDI(buf);
            Z80.LDI(buf);
            Z80.LDI(buf);
            return;
        ONO2:
            PC88.CALL(0x429D);
            Z80.EX_DE_HL();
            Z80.HL = 0;// OND;
            buf = Encoding.UTF8.GetBytes(OND);
            Z80.LDI(buf);
            Z80.LDI(buf);
            Z80.LDI(buf);
            //    RET
        }

        public void PRNWKI()
        {
            byte[] buf;

            //Z80.HL = NAME;
            //PC88.CALL(DSPMSG);
            expand.DSPMSG_msg = NAME;
            expand.DSPMSG();
            Z80.HL = 0;// NAME2;
            Z80.DE = 0xF3C8 + 120 * 17;
            Z80.B = 8;
        //FPL1:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.HL &= 1;
                buf = Encoding.UTF8.GetBytes(NAME2[8 - Z80.B]);
                Z80.LDI(buf);
                Z80.LDI(buf);
                Z80.A = 120 - 2;
                Z80.Carry = (Z80.A + Z80.E > 0xff);
                Z80.A += Z80.E;
                Z80.E = Z80.A;
                Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
                Z80.A -= Z80.E;
                Z80.D = Z80.A;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.DE = 0xF3C8 + 120 * 17 + 4;
            Z80.B = 6;
        //FPL2:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.HL = 0;// NAME3;
                buf = Encoding.UTF8.GetBytes(NAME3);
                Z80.BC = 10;
                Z80.LDIR_HL(buf);
                Z80.DE++;
                Z80.DE++;
                Z80.DE++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.HL = 0;// NAME4;
            buf = Encoding.UTF8.GetBytes(NAME4);
            Z80.DE = 0xF3C8 + 120 * 16 + 3;
            Z80.BC = 6 * 256 + 0xff;
        //FPL3:
            do
            {
                Z80.LDI(buf);
                Z80.A = 0x3a;// ':';
                Mem.LD_8(Z80.DE, Z80.A);
                Z80.A = 12;
                Z80.Carry = (Z80.A + Z80.E > 0xff);
                Z80.A += Z80.E;
                Z80.E = Z80.A;
                Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
                Z80.A -= Z80.E;
                Z80.D = Z80.A;
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        public void LOC()   // IN:A<=X ADR/OUT:HL
        {
            Z80.HL = Mem.LD_16(XY);
            Z80.A += Z80.H;
            Z80.H = Z80.A;
            //     RET
        }

        // --	FMｵﾝｹﾞﾝﾉ ﾃﾞｰﾀ ｦ ﾋｮｳｼﾞ	--
        public void FMPUT()
        {
            Z80.HL = FMDAT;
            Z80.DE = 4 * 256 + 19;  // XY
            FMP();
            Z80.HL = FMDAT + 1;
            Z80.DE = 17 * 256 + 19;
            FMP();
            Z80.HL = FMDAT + 2;
            Z80.DE = 30 * 256 + 19;
            FMP();
            Z80.HL = FMDAT + 0x60;
            Z80.DE = 43 * 256 + 19;
            FMP();
            Z80.HL = FMDAT + 0x61;
            Z80.DE = 56 * 256 + 19;
            FMP();
            Z80.HL = FMDAT + 0x62;
            Z80.DE = 69 * 256 + 19;
            FMP();
            //    RET
        }

        public void FMP()
        {
            Z80.B = 6;
        //FMP2:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.B = 4;
            //FMP3:
                do
                {
                    Mem.stack.Push(Z80.BC);
                    Mem.stack.Push(Z80.HL);
                    Mem.stack.Push(Z80.DE);
                    Z80.A = Mem.LD_8(Z80.HL);
                    Z80.L = Z80.A;
                    PRNS3();
                    Z80.DE = Mem.stack.Pop();
                    Z80.HL = Mem.stack.Pop();
                    Z80.HL++;
                    Z80.HL++;
                    Z80.HL++;
                    Z80.HL++;
                    Z80.D++;
                    Z80.D++;
                    Z80.D++;
                    Z80.BC = Mem.stack.Pop();
                    Z80.B--;
                } while (Z80.B != 0);
                Z80.A = Z80.D;
                Z80.A -= 12;
                Z80.D = Z80.A;
                Z80.E++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = 32;
            Z80.Carry = (Z80.A + Z80.L > 0xff);
            Z80.A += Z80.L;
            Z80.L = Z80.A;
            Z80.A += (byte)(Z80.H + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.L;
            Z80.H = Z80.A;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            PRNS3();
            //    RET
        }

        // ***   40 BYTE => 25 BYTE***

        public void CONVERT()
        {
            // IN:HL<=VOICEADR(40BYTE)
            // OUT:6000Hﾖﾘ 26BYTE
            Z80.DE = PARAM;
            Z80.B = 4;
        //CV1:
            do
            {
                Mem.stack.Push(Z80.BC);
                Mem.stack.Push(Z80.DE);
                Z80.B = 9;
            //CV2:
                do
                {
                    Z80.A = Mem.LD_8(Z80.HL);
                    Z80.HL++;
                    Mem.LD_8(Z80.DE, Z80.A);
                    Z80.A = 4;
                    Z80.Carry = (Z80.A + Z80.E > 0xff);
                    Z80.A += Z80.E;
                    Z80.E = Z80.A;
                    Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
                    Z80.A -= Z80.E;
                    Z80.D = Z80.A;
                    Z80.B--;
                } while (Z80.B != 0);
                Z80.DE = Mem.stack.Pop();
                Z80.DE++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = Mem.LD_8(Z80.HL);
            Mem.LD_8((PARAM + 36), Z80.A);
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            Mem.LD_8((PARAM + 38), Z80.A);
            GETPARA();
            Z80.HL = OTOWK;
            Z80.DE = 0x6000;
            Z80.BC = 32;
            Z80.LDIR();
            Z80.HL = 0x6001;
            //    RET
        }

        public void GETPARA()
        {
            OPEX();
            Z80.IX = PARAM;
            Z80.IY = OTOWK + 1;
            Z80.B = 4;
        //GETP2:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.L = Mem.LD_8((ushort)(Z80.IX + 32));
                Z80.H = 0;
                Z80.E = 16;
                Mem.stack.Push(Z80.IX);
                msub.MULT();
                Z80.IX = Mem.stack.Pop();
                Z80.A = Z80.L;
                Z80.L = Mem.LD_8((ushort)(Z80.IX + 28));
                Z80.A += Z80.L; //MUL/DT
                Mem.LD_8(Z80.IY, Z80.A);
                Z80.IX++;
                Z80.IY++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.HL = PARAM + 20;
            Z80.DE = OTOWK + 5;
            Z80.BC = 4;
            Z80.LDIR();                    // TL
            Z80.IX = PARAM;
            Z80.IY = OTOWK + 9;
            Z80.B = 4;
        //GETP3:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.L = Mem.LD_8((ushort)(Z80.IX + 24));
                Z80.H = 0;
                Z80.E = 64;
                Mem.stack.Push(Z80.IX);
                msub.MULT();
                Z80.IX = Mem.stack.Pop();
                Z80.A = Z80.L;
                Z80.L = Mem.LD_8(Z80.IX);
                Z80.A += Z80.L;// KS/AR
                Mem.LD_8(Z80.IY, Z80.A);
                Z80.IX++;
                Z80.IY++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.HL = PARAM + 4;
            Z80.DE = OTOWK + 13;
            Z80.BC = 8;
            Z80.LDIR();                    // DR&SR
            Z80.IX = PARAM;
            Z80.IY = OTOWK + 21;
            Z80.B = 4;
        //GETP4:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.L = Mem.LD_8((ushort)(Z80.IX + 16));
                Z80.H = 0;
                Z80.E = 16;
                Mem.stack.Push(Z80.IX);
                msub.MULT();
                Z80.IX = Mem.stack.Pop();
                Z80.A = Z80.L;
                Z80.L = Mem.LD_8((ushort)(Z80.IX + 12));
                Z80.A += Z80.L;// SL/RR
                Mem.LD_8(Z80.IY, Z80.A);
                Z80.IX++;
                Z80.IY++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = Mem.LD_8(PARAM + 36);
            Z80.L = Z80.A;
            Z80.H = 0;
            Z80.E = 8;
            Mem.stack.Push(Z80.IX);
            msub.MULT();
            Z80.IX = Mem.stack.Pop();
            Z80.A = Mem.LD_8(PARAM + 37);
            Z80.A += Z80.L;
            Mem.LD_8(OTOWK + 25, Z80.A);
            OPEX();
            //  RET
        }

        public void OPEX()
        {
            Z80.B = 10;
            Z80.HL = PARAM + 1;     // EX OP2, OP3
        //OPI4:
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.C = Mem.LD_8(Z80.HL);
                Mem.LD_8(Z80.HL, Z80.A);
                Z80.HL--;
                Mem.LD_8(Z80.HL, Z80.C);
                Z80.DE = 4;
                Z80.HL += Z80.DE;
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        // ***   ｵﾝｼｮｸﾃﾞｰﾀ ﾍﾝｶﾝ( 25 BYTE => 40 BYTE )    ***
        //IN:HL<=ﾍｯﾀﾞｦﾌｸﾑ ｵﾝｼｮｸﾃﾞｰﾀｱﾄﾞﾚｽ

        public void CONVERT2()
        {
            Z80.IX = PARAM + 28;
            Z80.B = 4;

        // ---   SET DT&ML   ---

        //OPI0:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.HL++;// ｻｲｼｮ ﾀﾞｹ SKIP
                Z80.A = Mem.LD_8(Z80.HL);
                Mem.stack.Push(Z80.HL);
                Z80.L = Z80.A;
                Z80.H = 0;
                Z80.DE = 16;
                Mem.stack.Push(Z80.IX);
                msub.DIV();
                Z80.IX = Mem.stack.Pop();
                Mem.LD_8(Z80.IX, Z80.E);// E=MULTIPLE
                Mem.LD_8((ushort)(Z80.IX + 4), Z80.L);// L=DETUNE
                Z80.HL = Mem.stack.Pop();
                Z80.IX++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);

            // ---   SET TL   ---

            Z80.HL++;
            Z80.DE = PARAM + 20;
            Z80.BC = 4;
            Z80.LDIR();

            // ---   SET KS&AR   ---

            Z80.IX = PARAM;
            Z80.B = 4;
        //OPI1:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Mem.stack.Push(Z80.HL);
                Z80.L = Z80.A;
                Z80.H = 0;
                Z80.DE = 64;
                Mem.stack.Push(Z80.IX);
                msub.DIV();
                Z80.IX = Mem.stack.Pop();
                Mem.LD_8(Z80.IX, Z80.E);// E=ATTACK
                Mem.LD_8((ushort)(Z80.IX + 24), Z80.L);// L=KEY SCALE
                Z80.HL = Mem.stack.Pop();
                Z80.IX++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);

            // ---   SET DR&SR   ---

            Z80.DE = PARAM + 4;
            Z80.BC = 8;
            Z80.LDIR();

            // ---   SET SL&RR   ---

            Z80.IX = PARAM + 12;
            Z80.B = 4;
        //OPI3:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Mem.stack.Push(Z80.HL);
                Z80.L = Z80.A;
                Z80.H = 0;
                Z80.DE = 16;
                Mem.stack.Push(Z80.IX);
                msub.DIV();
                Z80.IX = Mem.stack.Pop();
                Mem.LD_8(Z80.IX, Z80.E);// E=RR
                Mem.LD_8((ushort)(Z80.IX + 4), Z80.L);// L=SL
                Z80.HL = Mem.stack.Pop();
                Z80.IX++;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);

            // ---   SET AR & FB   ---

            Z80.L = Mem.LD_8(Z80.HL);
            Z80.H = 0;
            Z80.DE = 8;
            Mem.stack.Push(Z80.IX);
            msub.DIV();
            Z80.IX = Mem.stack.Pop();
            Z80.A = Z80.L;// L=AL
            Mem.LD_8((ushort)(PARAM + 36), Z80.A);
            Z80.A = Z80.E;		// E=FB
            Mem.LD_8((ushort)(PARAM + 37), Z80.A);
            Z80.HL = 0;
            Mem.LD_16((ushort)(PARAM + 38), Z80.HL);
            OPEX();
            //  RET
        }

        public string NAME = "   Mut Vol Det   Adr  Key LFO Rev p qtz\0";
        public string[] NAME2 = new string[] {
            "op"
            ,"DM"
            ,"TL"
            ,"KA"
            ,"DR"
            ,"SR"
            ,"SL"
            ,"FC"
        };
        public string NAME3 = "1  3  2  4";
        public string NAME4 = "ABCHIJ";
        public byte CHNUM = 0;
        public ushort XY = 0;
        public string CHCHR = "A";
        public string[] KEYD = new string[] { "C ", "C+", "D ", "D+", "E ", "F ", "F+", "G ", "G+", "A ", "A+", "B " };
        public string OND = "ON ";
        public string OFFD = "OFF";
        public byte[] BEFK = new byte[] { 0, 0 };
        public byte RETK = 0;
        public byte F5K = 0;

    }
}