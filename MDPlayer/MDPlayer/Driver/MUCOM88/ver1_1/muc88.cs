using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88.ver1_1
{
    public class muc88
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;
        public msub msub = null;
        public expand expand = null;
        public ver1_0.smon smon = null;

        public Action[] COMTBL;
        public int COMTBL_RetPtn = -1;

        //プログラム書き換えコード対策
        private ushort EMAC2_VAL = 0;
        private byte FCP18_VAL = 0;
        private ushort SETLPE3_VAL = 0x0000;
        private ushort SETLPE4_VAL = 0x0000;
        private byte SETR9_VAL = 0;
        private byte STCL6_VAL = 0;
        private ushort TST2_VAL = 0xc000;
        private byte VCON0_VAL = 0;
        private byte STM2_VAL = 0;
        private byte SETVI1_VAL = 4;

        //       "MUCOM88A"
        //       Programed by YK-2

        //	ORG	09600H

        public const int COMWK = 0xF320;//ｺﾝﾊﾟｲﾗ ﾜｰｸﾄｯﾌﾟ
        public const int SUBWK = 0xC400;//GOSUBﾖｳ ｽﾀｯｸ(VRAM0ｦﾂｶｳ)
        public const int LOOPSP = 0xF260;//ﾙｰﾌﾟｽﾀｯｸ
        public const int CURSOR = 0xEF86;//ｶｰｿﾙﾜｰｸ

        public const int MDATA = COMWK;// ｺﾝﾊﾟｲﾙｻﾚﾀ ﾃﾞｰﾀｶﾞｵｶﾚﾙ ｹﾞﾝｻﾞｲﾉ ｱﾄﾞﾚｽ//KUMA:0xf320
        public const int DATTBL = MDATA + 4;// ｹﾞﾝｻﾞｲ ｺﾝﾊﾟｲﾙﾁｭｳ ﾉ MUSIC DATA TABLE TOP//KUMA:0xf324
        public const int OCTAVE = DATTBL + 2;//KUMA:0xf326
        public const int SIFTDAT = OCTAVE + 1;//KUMA:0xf327
        public const int CLOCK = SIFTDAT + 1;//KUMA:0xf328
        public const int SECCOM = CLOCK + 1;//KUMA:0xf329
        public const int KOTAE = SECCOM + 1;//KUMA:0xf32A
        public const int LINE = KOTAE + 2;//KUMA:0xf32c
        public const int ERRLINE = LINE + 2;//KUMA:0xf32e
        public const int COMNOW = ERRLINE + 2;//
        public const int COUNT = COMNOW + 1;//
        public const int MOJIBUF = COUNT + 1;//
        public const int SEC = MOJIBUF + 4;//
        public const int MIN = SEC + 1;//
        public const int HOUR = MIN + 1;//
        public const int ALLSEC = HOUR + 1;//
        public const int T_FLAG = ALLSEC + 2;//
        public const int SE_SET = T_FLAG + 1;//
        public const int VOLINT = SE_SET + 2;//
        public const int FLGADR = VOLINT + 1;//
        public const int ESCAPE = FLGADR + 1;//
        public const int MINUSF = ESCAPE + 1;//
        public const int BEFRST = MINUSF + 1;// ｾﾞﾝｶｲ ｶﾞ 'r' ﾃﾞｱﾙｺﾄｦｼﾒｽ ﾌﾗｸﾞ ｹﾝ ｶｳﾝﾀ
        public const int BEFCO = BEFRST + 1;// ｾﾞﾝｶｲ ﾉ ｶｳﾝﾀ
        public const int BEFTONE = BEFCO + 2;// ｾﾞﾝｶｲ ﾉ ﾄｰﾝ ﾃﾞｰﾀ
        public const int TIEFG = BEFTONE + 9;// ｾﾞﾝｶｲ ｶﾞ ﾀｲﾃﾞｱﾙｺﾄｦ ｼﾒｽ
        public const int COMNO = TIEFG + 1;// ｾﾞﾝｶｲﾉ ｺﾏﾝﾄﾞ｡ ﾄｰﾝﾉﾄｷﾊ 0
        public const int ASEMFG = COMNO + 1;//
        public const int VDDAT = ASEMFG + 1;//
        public const int OTONUM = VDDAT + 1;// ﾂｶﾜﾚﾃｲﾙ ｵﾝｼｮｸ ﾉ ｶｽﾞ
        public const int VOLUME = OTONUM + 1;// NOW VOLUME
        public const int LINKPT = VOLUME + 1;// LINK POINTER
        public const int ENDADR = LINKPT + 2;//
        public const int OCTINT = ENDADR + 2;//


        //MWRITE	EQU	9000H
        //MWRIT2	EQU MWRITE+3
        //ERRT	    EQU MWRIT2+3
        //ERRSN     EQU ERRT+3
        //ERRIF     EQU  ERRSN+3
        //ERRNF     EQU  ERRIF+3
        //ERRFN     EQU  ERRNF+3
        //ERRVF     EQU  ERRFN+3
        //ERROO     EQU  ERRVF+3
        //ERRND     EQU  ERROO+3
        //ERRRJ     EQU  ERRND+3
        //STTONE	EQU ERRRJ+3
        //STLIZM	EQU STTONE+3
        //REDATA	EQU STLIZM+3
        //MULT	    EQU REDATA+3
        //DIV	    EQU MULT+3
        //HEXDEC	EQU DIV+3
        //HEXPRT	EQU HEXDEC+3
        //ROM	    EQU HEXPRT+3
        //RAM	    EQU ROM+3
        //FMCOMC	EQU RAM+3
        //T_RST	    EQU FMCOMC+3
        //ERRNE     EQU T_RST+3
        //ERRDC     EQU ERRNE+3
        //ERRML     EQU ERRDC+3
        //MCMP	    EQU ERRML+3
        //ERRVO     EQU MCMP+3
        //ERRMD     EQU ERRVO+3
        //ERRNMC	EQU ERRMD+3*5
        //ERREMC	EQU ERRNMC+3


        public const int MSTART = 0x0B000;
        public const int MSTOP = MSTART + 3;
        public const int START = MSTART + 3 * 6;
        public const int WORKINIT = START + 3;
        public const int AKYOFF = START + 3 * 2;
        public const int SSGOFF = START + 3 * 3;
        public const int MONO = START + 3 * 4;
        public const int DRIVE = START + 3 * 5;
        public const int TO_NML = START + 3 * 6;
        public const int WKGET = START + 3 * 8;
        public const int STVOL = START + 3 * 9;
        public const int ENBL = START + 3 * 10;
        public const int INFADR = START + 3 * 12;


        public const int MU_NUM = 0xC200;//ｺﾝﾊﾟｲﾙﾁｭｳ ﾉ MUSICﾅﾝﾊﾞｰ
        public const int OTODAT = MU_NUM + 1;// FMｵﾝｼｮｸ ｶﾞ ｶｸﾉｳｻﾚﾙ ｱﾄﾞﾚｽﾄｯﾌﾟ ｶﾞ ﾊｲｯﾃｲﾙ
        public const int SSGDAT = MU_NUM + 3;// SSG...
        public const int MU_TOP = MU_NUM + 5;// ﾐｭｰｼﾞｯｸ ﾃﾞｰﾀ(ｱﾄﾞﾚｽﾃｰﾌﾞﾙ ﾌｸﾑ) ｽﾀｰﾄ ｱﾄﾞﾚｽ
        public const int MAXCHN = 11;// ﾂｶﾜﾚﾙ ｵﾝｹﾞﾝｽｳ ﾉ MAX
        public const int FMLIB = 0x6000;// ｵﾝｼｮｸﾗｲﾌﾞﾗﾘ ｱﾄﾞﾚｽ
        public const int SSGLIB = 0x5E00;

        public const int CLS1 = 0x5F0E;
        public const int KBD = 0x09A00;
        //public const int DSPMSG	EQU	0AB00H
        //public const int FOUND	    EQU DSPMSG+3
        //public const int PRNFAC	EQU FOUND+3
        //public const int FVTEXT	EQU PRNFAC+3
        //public const int COLOR	    EQU FVTEXT+3
        //public const int REPLACE   EQU COLOR+6
        //public const int CULPTM	EQU REPLACE+3


        //PRNWK	    EQU	0DE00H
        //PRNWKI	EQU PRNWK+3
        //CONVERT   EQU PRNWK+3*2
        //CONVERT2  EQU PRNWK+3*3
        //GETPARA   EQU PRNWK+3*4
        //OPEX	    EQU PRNWK+3*5
        //OTOWK	    EQU	8B00H+60H+60H	;25BYTEﾃﾞｰﾀｶﾞ ﾊｲﾙﾄｺﾛ
        //PARAM	    EQU OTOWK+32	;38BYTEﾃﾞｰﾀｶﾞﾊｲﾙﾄｺﾛ
        public const int TEXT = 0x0F3C8;

        // -- CLEAR FROM COMPI1	-->

        public const int T_CLK = 0x8C10;
        public const int BEFMD = T_CLK + 4 * 11 + 1;//+1ｱﾏﾘ
        public const int PTMFG = BEFMD + 2;
        public const int PTMDLY = PTMFG + 1;
        public const int TONEADR = PTMDLY + 2;
        public const int SPACE = TONEADR + 2;//2*6BYTE ｱｷ ｶﾞ ｱﾙ
        public const int DEFVOICE = SPACE + 2 * 6;
        public const int DEFVSSG = DEFVOICE + 32;
        public const int JCLOCK = DEFVSSG + 32;
        public const int JPLINE = JCLOCK + 2;

        //;-<
        //JP CINT
        //JP GETADR

        //上位が直接COMPILを呼ぶのでこのメソッドに意味は特にない。
        public void CINT()
        {
            Z80.HL = 0;// COMPIL;
            Z80.A = 0xc3;
        //CI2:
            Mem.LD_8(0xeea7, Z80.A);
            Mem.LD_16(0xeea8, Z80.HL);
            //RET
        }

        public int COMPIL()
        {
            Mem.stack.Push(Z80.HL);
            INIT0();
            Z80.HL = Mem.stack.Pop();
            Z80.A = 1;
            //Mem.LD_8(BFDAT, Z80.A);
            BFDAT = Z80.A;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
        //COM1:
            Z80.HL = ASEMFG;
            if (Z80.A - 0xca == 0)
            {
                // MON
                CHECK();
                return 0;
            }
            if (Z80.A - 0x46 == 0)//'F'
            {
                WFOUND();
                return 0;
            }
            if (Z80.A - 0x52 == 0)//'R'
            {
                REPST();
                return 0;
            }
            if (Z80.A - 0xce == 0)
            {
                goto FDO;
            }

            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.HL);
            PC88.CALL(INFADR);
            Mem.LD_8((ushort)(Z80.IX + 2), 0);
            PC88.CALL(TO_NML);
            PC88.CALL(MONO);
            PC88.CALL(MSTOP);
            Z80.HL = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();

            if (Z80.A - 0x45 == 0)//'E'
            {
                CHGPMD();
                return 0;
            }
            if (Z80.A - 0x49 == 0)//'I'
            {
                INITFG();
                return 0;
            }
            if (Z80.A - 0x44 == 0)//'D'
            {
                DEBFCH();
                return 0;
            }
            if (Z80.A - 0x56 == 0)//'V'
            {
                VICMKE();
                return 0;
            }
            if (Z80.A - 0x4c == 0)//'L'
            {
                LINC();
                return 0;
            }
            if (Z80.A - 0x53 == 0)//'S'
            {
                M_ST();
                return 0;
            }
            if (Z80.A - 0x50 == 0)//'P'
            {
                COMPI1();
                return 0;
            }
            if (Z80.A - 0x5a == 0)//'Z'
            {
                CMP1();
                return 0;
            }
            Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) + 1));
            if (Z80.A - 0x41 == 0)//'A' ｺﾝﾊﾟｲﾙｵﾝﾘｰ
            {
                COMPI1();
                if(FCOMP_nextRtn== enmFCOMPNextRtn.occuredERROR)
                {
                    return -1;
                }
                return 0;
            }

            Z80.HL = Mem.stack.Pop();
            Z80.E = 2;

            FCOMP_nextRtn = enmFCOMPNextRtn.occuredERROR;
            PC88.CALL(0x03B3);
            return 0;

        FDO:
            PC88.CALL(INFADR);
            Mem.LD_8((ushort)(Z80.IX + 8), 16);
            Z80.HL = Mem.stack.Pop();
            //    RET
            return 0;
        }

        public void CHGPMD()
        {
            //Z80.A = Mem.LD_8(PSGMD);
            Z80.A = PSGMD;
            Z80.A = (byte)~Z80.A;
            //Mem.LD_8(PSGMD, Z80.A);
            PSGMD = Z80.A;
            PRMODE();
            Z80.HL = Mem.stack.Pop();
            //  RET
        }

        public void CHECK()
        {
            Z80.HL = Mem.LD_16(CURSOR);
            Mem.stack.Push(Z80.HL);
            PC88.CALL(CLS1);
            Z80.HL = 0x0A8 * 256 + 40;
            Mem.LD_16(TEXT + 120 * 6 + 80, Z80.HL);
            Mem.LD_16(TEXT + 120 * 7 + 80, Z80.HL);
            Mem.LD_16(TEXT + 120 * 8 + 80, Z80.HL);
            Z80.HL = 0x88 * 256 + 40;
            Mem.LD_16(TEXT + 120 * 9 + 80, Z80.HL);
            Z80.HL = 0x0C8 * 256 + 40;
            Mem.LD_16(TEXT + 120 * 13 + 80, Z80.HL);
            smon.PRNWKI();
        CC2:
            smon.PRNWK();
            Z80.A = PC88.IN(9);
            Z80.Zero = (Z80.A & 0x01) == 0;
            if (!Z80.Zero)
            {
                goto CC2;
            }
            PC88.CALL(CLS1);
            Z80.HL = Mem.stack.Pop();
            Mem.LD_16(CURSOR, Z80.HL);
            Z80.HL = Mem.stack.Pop();
            //  RET
        }

        public void WFOUND()
        {
            expand.FOUND();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void INITFG()
        {
            //Z80.A = Mem.LD_8(UDFLG);
            Z80.A = UDFLG;
            Z80.A = (byte)~Z80.A;
            //Mem.LD_8(UDFLG, Z80.A);
            UDFLG = Z80.A;
            //Z80.HL = IFGM;
            expand.DSPMSG_msg = IFGM;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto IFG2;
            }
            //Z80.HL = IFGM2;
            expand.DSPMSG_msg = IFGM2;
        IFG2:
            expand.DSPMSG();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void REPST()
        {
            expand.REPLACE();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        // --	SYSTEM KBD	--

        public void KBDSUB()
        {
            //DI
            msub.RAM();
            Z80.HL = 0x4800;
            Z80.DE = 0x09A00;
            Z80.BC = 0x1600;
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.HL);
            Mem.stack.Push(Z80.HL);
            EXCG();
            msub.ROM();
            //EI

            PC88.CALL(KBD);

            //DI
            msub.RAM();
            Z80.BC = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();
            EXCG();
            msub.ROM();
            //EI

            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void EXCG()
        {
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.EX_AF_AF();
                Z80.A = Mem.LD_8(Z80.DE);
                Z80.EX_AF_AF();
                Mem.LD_8(Z80.DE, Z80.A);
                Z80.EX_AF_AF();
                Mem.LD_8(Z80.HL, Z80.A);
                Z80.HL++;
                Z80.DE++;
                Z80.BC--;
                Z80.A = Z80.C;
                Z80.A |= Z80.B;
            } while (Z80.A != 0);
            //  RET
        }

        // *	ﾒｲﾝ*

        public void DEBFCH()
        {
            //Z80.A = Mem.LD_8(DEBFLG);
            Z80.A = DEBFLG;
            Z80.A = (byte)~Z80.A;
            //Mem.LD_8(DEBFLG, Z80.A);
            DEBFLG = Z80.A;
            Z80.HL = Mem.stack.Pop();
            //  RET
        }

        public void CMP1()
        {
            //DI
            Z80.HL = VPCO;
            Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) + 1));
            CMP2();
        }

        public void COMPI1()
        {
            //DI
            Z80.A ^= Z80.A;
            //Mem.LD_8(VPCO, Z80.A);
            VPCO = Z80.A;
            CMP2();
        }

        public void CMP2()
        {
            //Z80.A = Mem.LD_8(LINCFG);
            Z80.A = LINCFG;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                COMPI4();
                return;
            }
            Z80.BC = 239;
            Z80.HL = T_CLK;
            MEMCLR();
            COMPI4();
        }

        public void COMPI4()
        {
            Z80.HL = Mem.stack.Pop();// TEXT POINTER
            msub.REDATA();
            if (Z80.Carry)
            {
                COMPI2();
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.A = Z80.E;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                COMP42();
                return;
            }
            //Z80.A = Mem.LD_8(LINCFG);
            Z80.A = LINCFG;
            Z80.A |= Z80.A;
            Z80.Zero = (Z80.A == 0);
            Z80.A = Z80.E;
            if (!Z80.Zero)
            {
                COMPI3();
                return;
            }
            Z80.HL = Mem.stack.Pop();

            msub.ERRORMD();
        }

        public void COMP42()
        {
            Z80.HL = Mem.stack.Pop();
            COMPI2();
        }
        public void COMPI2()
        {
            Mem.stack.Push(Z80.HL);
            Z80.A = 1;
            COMPI3();
        }
        public void COMPI3()
        {
            WORKGET(); // EXIT: (MU_NUM)<= MUSIC NUMBER
                       // DATTBL INIT
            Z80.A ^= Z80.A;
            //Mem.LD_8(REPCOUNT, Z80.A);
            REPCOUNT = Z80.A;
            PC88.CALL(AKYOFF);
            PC88.CALL(SSGOFF);
            TEXTINIT();
            msub.RAM();
            Z80.HL = Mem.LD_16(MDATA);
            TBLSET();

            Z80.HL = LOOPSP - 10;// LOOP ﾖｳ ｽﾀｯｸ
            //Mem.LD_16(POINTC, Z80.HL);
            POINTC = Z80.HL;
            Z80.HL = 1;// TEXT START ADR

            CSTART();
        }

        // *   ｺﾝﾊﾟｲﾙ ｽﾀｰﾄ   *

        public void CSTART()
        {
            Z80.A = 0xff;
            //Mem.LD_8(MACFG, Z80.A);
            MACFG = Z80.A;
            COMPST();//KUMA:先ずマクロの解析
            Z80.HL = Mem.LD_16(MDATA);
            TBLSET();
            Z80.HL = 1;
            CSTART2();
        }

        public void CSTART2()
        {
            do
            {
                Z80.A ^= Z80.A;
                //Mem.LD_8(MACFG, Z80.A);
                MACFG = Z80.A;
                COMPST();
                if (FCOMP_nextRtn == enmFCOMPNextRtn.occuredERROR)
                {
                    return;
                }

                CMPEND();// ﾘﾝｸ ﾎﾟｲﾝﾀ = 0->BASIC END
                if (CMPEND_nextRtn == enmCMPENDNextRtn.occuredERROR)
                {
                    FCOMP_nextRtn = enmFCOMPNextRtn.occuredERROR;
                    return;
                }

            } while (CMPEND_nextRtn == enmCMPENDNextRtn.CSTART2);
        }

        public void COMPST()
        {
            do
            {
                Z80.E = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.D = Mem.LD_8(Z80.HL);       // DE=LINK POINTER
                Z80.HL++;
                Mem.LD_16(LINKPT, Z80.DE);  // STORE LINK POINTER
                Z80.A = Z80.E;
                Z80.A |= Z80.D;
                if (Z80.A == 0)//最終行まで走査完了したか
                {
                    return;
                }

                Mem.LD_16(LINE, Z80.HL);
                //log.Write(string.Format("{0}行目の解析", Mem.LD_16(Z80.HL)));
                Mem.LD_16(ERRLINE, Mem.LD_16(Z80.HL));

                Z80.HL++;       //
                Z80.HL++;// SKIP LINE NUMBER
                Z80.A = Mem.LD_8(Z80.HL);
                if (Z80.A - 0x03A != 0)
                {
                    // BASIC COM.
                    goto RECOM;
                }
                Z80.HL++;
                Z80.A = Mem.LD_8(Z80.HL);
                if (Z80.A - 0x8F != 0)
                {
                    // BASIC COM. (REM)
                    goto RECOM;
                }
                Z80.HL++;
                Z80.A = Mem.LD_8(Z80.HL);
                if (Z80.A - 0x0E9 != 0)
                {
                    // BASIC COM. (')
                    goto RECOM;
                }
                Z80.HL++;
                //Z80.A = Mem.LD_8(MACFG);
                Z80.A = MACFG;
                Z80.A++;
                if (Z80.A == 0)
                {
                    MACPRC();//マクロの解析

                    //KUMA:RECOMの処理をここに追加
                    Z80.HL = Mem.LD_16(LINKPT); // ﾘﾝｸﾎﾟｲﾝﾀ ｻｲｾｯﾄ
                    continue;
                }
                Z80.A = Mem.LD_8(Z80.HL);      // A=Ch.NUMBER(A-F)
                Z80.HL++;
                Z80.C = Z80.A;
                Mem.stack.Push(Z80.HL);
                Z80.HL = 0x0C800;
                //Z80.DE = Mem.LD_16(ADRSTC);
                Z80.DE = ADRSTC;
                Z80.A &= Z80.A;
                Z80.HL -= Z80.DE;
                Z80.EX_DE_HL();
                Z80.HL = Mem.stack.Pop();
                Z80.A = Z80.E;
                Z80.A |= Z80.D;
                if (Z80.A != 0)
                {
                    goto CST3;//ﾏｸﾛﾁｭｳﾅﾗ ﾍｯﾀﾞﾁｪｯｸﾊﾟｽ
                }
                Z80.A = Z80.C;
                Z80.A |= Z80.A;
                if (Z80.A == 0)
                {
                    goto RECOM;
                }
                Mem.stack.Push(Z80.AF);
                //Z80.A = Mem.LD_8(MAXCH);
                Z80.A = MAXCH[0];
                Z80.A += 0x41;// 'A';
                Z80.C = Z80.A;
                Z80.AF = Mem.stack.Pop();
                if (Z80.A - Z80.C >= 0)//KUMA:MUCOM88が扱える最大のチャンネルよりも大きいパートを指定しているか
                {
                    goto RECOM;
                }
                if (Z80.A - 0x41 < 0)//	'A'
                {
                    goto RECOM;
                }
                Z80.C = Z80.A;
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.A |= Z80.A;
                if (Z80.A == 0)
                {
                    goto RECOM;
                }
                Z80.A = Z80.C;
                Z80.A &= 0b0000_1111;
                Z80.A--;        // A=Ch.NUMBER(0-5)
                Z80.C = Z80.A;
                Z80.A = Mem.LD_8(COMNOW);
                if (Z80.A - Z80.C != 0)
                {
                    // ｹﾞﾝｻﾞｲ ｺﾝﾊﾟｲﾙﾁｭｳ ﾉ ﾁｬﾝﾈﾙ
                    // ﾃﾞﾅｹﾚ ﾊﾞ ﾂｷﾞﾉｷﾞｮｳ
                    goto RECOM;
                }
            CST3:
                //log.Write(string.Format("Ch.{0}のFMCOMP開始", Encoding.GetEncoding("Shift_JIS").GetString(new byte[] { (byte)(0x41 + Z80.C) })));
                FMCOMP();// TO FM COMPILE
                if(FCOMP_nextRtn== enmFCOMPNextRtn.occuredERROR)
                {
                    break;
                }
                continue;// goto COMPST;
            RECOM:
                Z80.HL = Mem.LD_16(LINKPT); // ﾘﾝｸﾎﾟｲﾝﾀ ｻｲｾｯﾄ
                continue;// goto COMPST;// ﾂｷﾞ ﾉ ｷﾞｮｳﾍ
            } while (true);
        }

        // *	ﾏｸﾛ ｼｮﾘ	*


        public void MACPRC()
        {
            Z80.C = 0x2a;// '*'

            Z80.DE = 0x0C000;//VRAMSTAC
            //Mem.LD_16(TST2 + 1, Z80.DE);
            TST2_VAL = Z80.DE;
            MPC1();
            //goto RECOM; //KUMA:呼び出しもとでRECOMを実施する
        }

        // --	ﾃｷｽﾄｶﾗ Cregｺﾏﾝﾄﾞ ｦｻｶﾞｽ	--

        public void MPC1()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            if (Z80.A - 0x23 != 0)//'#'
            {
                goto MPNON;
            }

        MPC2:
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            if (Z80.A - 0x20 == 0)
            {
                goto MPC2;
            }
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto MPNON;
            }
            if (Z80.A - 0x3b == 0)//';'
            {
                goto MPNON;
            }
            if (Z80.A - 0x7b == 0)//'{'
            {
                goto MPNON;
            }
            if (Z80.A - Z80.C != 0)
            {
                goto MPNON;
            }

            msub.REDATA();// MAC NO.
            if (Z80.Carry)
            {
                msub.ERRORNE();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            if (Z80.A - 0x7b != 0)//'{'
            {
                msub.ERRSN();
                return;
            }
            //Z80.A = Mem.LD_8(MACFG);
            Z80.A = MACFG;
            Z80.A++;
            if (Z80.A == 0)
            {
                goto MPC3;
            }
            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;        //RET
        MPC3:
            TOSTAC();
            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;//RET
        MPNON:
            Z80.Carry = true; // ﾅｲﾖ
            // RET
        }

        // *	MACRO STAC	*

        public void TOSTAC()
        {
            PC88.OUT(0x5C, Z80.A);
            Mem.stack.Push(Z80.HL);
            Z80.L = Z80.E;
            Z80.H = 0;
            Z80.D = Z80.H;
            Z80.HL += Z80.DE;
            Z80.HL += Z80.HL;//*4
        //TST2:
            Z80.DE = TST2_VAL;// 0xC000;
            Z80.HL += Z80.DE;
            Z80.DE = Mem.stack.Pop();
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL++;
            Z80.DE = Mem.LD_16(LINKPT);
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            PC88.OUT(0x5F, Z80.A);
            //RET
        }

        // *   LINC OBJECT FLAG SET  *

        public void LINC()
        {
            //Z80.A = Mem.LD_8(LINCFG);
            Z80.A = LINCFG;
            Z80.A = (byte)~Z80.A;
            //Mem.LD_8(LINCFG, Z80.A);
            LINCFG = Z80.A;
            PRMODE();
            Z80.HL = Mem.LD_16(MDATA + 2);
            Mem.LD_16(MDATA, Z80.HL);
            Z80.HL = DEFVOICE;
            Z80.BC = 63;
            MEMCLR();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void PRMODE()
        {
            //Z80.HL = MESNML;
            Z80.HL = 0;
            //Z80.A = Mem.LD_8(PSGMD);
            Z80.A = PSGMD;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                //Z80.HL = MESNML + 16;
                Z80.HL = 16;
            }
        //PRM0:
            //Z80.A = Mem.LD_8(LINCFG);
            Z80.A = LINCFG;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                //Z80.HL = MESNML + 8;
                Z80.HL = 8;
            }
        //PRM1:
            Z80.DE = TEXT + 70;
            Z80.BC = 8;
            Z80.LDIR_HL(Encoding.UTF8.GetBytes(MESNML));
            //    RET
        }

        // *	BASIC END	*


        public void CMPEND()
        {
            CMPEND_nextRtn = enmCMPENDNextRtn.Unknown;

            TCLKADR();
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.A |= Z80.E;
            if (Z80.A != 0)
            {
                goto CMPE2;
            }
            Z80.A = Mem.LD_8(COMNOW);
            CHTBL();
            Z80.HL++;
            Z80.HL++;
            Mem.LD_8(Z80.HL, 0);
            Z80.HL++;
            Mem.LD_8(Z80.HL, 0);
        CMPE2:
            Z80.HL = Mem.LD_16(MDATA);
            Z80.A ^= Z80.A;
            Mem.LD_8(Z80.HL, Z80.A);        // SET END MARK = 0
            Z80.HL++;
            Mem.LD_16(MDATA, Z80.HL);
            Z80.A = Mem.LD_8(COMNOW);
            Z80.A++;
            Mem.LD_8(COMNOW, Z80.A);		// Ch.=Ch.+ 1
            TBLSET();
            Z80.A = Mem.LD_8(COMNOW);
            Z80.L = Z80.A;
            //Z80.A = Mem.LD_8(MAXCH);
            Z80.A = MAXCH[0];
            if (Z80.A - Z80.L == 0)
            {
                CMPEN1();
                CMPEND_nextRtn = enmCMPENDNextRtn.Success;
                return;
            }
            Z80.HL = 1;// TEXT START ADR
            INIT();
            //Z80.A = Mem.LD_8(REPCOUNT);
            Z80.A = REPCOUNT;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRORFN();// [] ﾉ ｸﾐｱﾜｾ ｶﾞ ｱﾜﾅｲ
                CMPEND_nextRtn = enmCMPENDNextRtn.occuredERROR;
                return;
            }

            Z80.A ^= Z80.A;
            //Mem.LD_8(REPCOUNT, Z80.A);
            REPCOUNT = Z80.A;
            //Mem.LD_8(TV_OFS, Z80.A);
            TV_OFS = Z80.A;
            Mem.LD_8(SIFTDAT, Z80.A);
            Mem.LD_8(BEFRST, Z80.A);
            Mem.LD_8(TIEFG, Z80.A);

            CMPEND_nextRtn = enmCMPENDNextRtn.CSTART2;
        }

        public void TBLSET()
        {
            TBOFS();
            Z80.EX_DE_HL();
            Z80.A = Mem.LD_8(COMNOW);
            CHTBL();
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            //RET
        }

        public void TBOFS()
        {
            Z80.DE = MU_TOP;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            //    RET
        }

        // --	Areg CH ﾉ ﾃｰﾌﾞﾙｱﾄﾞﾚｽｦﾓﾄﾒﾙ	--

        // OUT:HL
        public void CHTBL()
        {
            Z80.HL = Mem.LD_16(DATTBL);
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.Carry = (Z80.A + Z80.L > 0xff);
            Z80.A += Z80.L;
            Z80.L = Z80.A;
            Z80.A += (byte)(Z80.H + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.L;
            Z80.H = Z80.A;
            //    RET
        }

        // *	ｽﾍﾞﾃ ﾉ ch ﾉ ｼｮﾘ ｼｭｳﾘｮｳ	*

        public void CMPEN1()
        {
            //Z80.A = Mem.LD_8(REPCOUNT);
            Z80.A = REPCOUNT;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRORFN();
                return;
            }
            Z80.HL = Mem.LD_16(MDATA);
            Mem.LD_16(ENDADR, Z80.HL);
            Mem.stack.Push(Z80.HL);
            Z80.DE = MU_NUM;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Mem.LD_16(OTODAT, Z80.HL);
            Z80.HL = Mem.stack.Pop(); // HL = ｵﾄDATAﾉﾐEND + 1
            Z80.HL = MU_NUM;
            msub.HEXPRT();// PRINT START ADR
            Z80.HL = MOJIBUF;
            Z80.DE = TEXT + 31;
            Z80.BC = 4;
            Z80.LDIR();
            // --   25 BYTE VOICE DATA ﾉ ｾｲｾｲ   --
            Z80.HL = Mem.LD_16(ENDADR);
            Mem.stack.Push(Z80.HL);
            Z80.HL++;
            Mem.LD_16(ENDADR, Z80.HL);
            //Mem.LD_16(VICADR, Z80.HL);
            VICADR = Z80.HL;

            CMPEN3();
        }

        public void CMPEN3()
        {
            Z80.DE = DEFVOICE;
            Z80.B = 32;
            Z80.A ^= Z80.A;
            VOICECONV1();
        }

        public void VOICECONV1()
        {
            do
            {
                Mem.stack.Push(Z80.AF);
                Mem.stack.Push(Z80.BC);
                Z80.A = Mem.LD_8(Z80.DE);   // GET VOICE NUMBER
                Z80.A |= Z80.A;
                if (Z80.A == 0)
                {
                    goto VCON2;
                }
                Z80.C = Z80.A;
                //Z80.A = Mem.LD_8(VPCO);
                Z80.A = VPCO;
                Z80.A += Z80.C;
                Z80.DE++;
                Z80.A--;
                //Mem.LD_8(VCON0 + 1, Z80.A);
                VCON0_VAL = Z80.A;
                Mem.stack.Push(Z80.DE);
                Mem.stack.Push(Z80.HL);
                expand.FVTEXT(); //KUMA:MML中で音色定義されているナンバーかどうか探す
                Z80.HL = Mem.stack.Pop();
                Z80.DE = Mem.stack.Pop();
                if (Z80.Carry)
                {
                    goto VCON0;//KUMA:見つからなかった
                }
                Z80.HL = FMLIB + 1;
                goto VCON01;
            VCON0:
                Z80.A = VCON0_VAL;// 0;
                GETADR();
                Z80.HL++;// HL= VOICE INDEX
            VCON01:
                Mem.stack.Push(Z80.DE);
                Z80.BC = 12;
                Z80.DE = Mem.LD_16(ENDADR);
                Z80.LDIR();//KUMA:最初の12byte分の音色データをコピー
                Z80.B = 4;
                //VCON1:
                do//KUMA:次の4byte分の音色データをbit7(AMON)を立ててコピー
                {
                    Z80.A = Mem.LD_8(Z80.HL);
                    Z80.A |= 0b1000_0000;// SET AMON FLAG
                    Mem.LD_8(Z80.DE, Z80.A);
                    Z80.HL++;
                    Z80.DE++;
                    Z80.B--;
                } while (Z80.B != 0);
                Z80.BC = 9;
                Z80.LDIR();
                Mem.LD_16(ENDADR, Z80.DE);
                Z80.DE = Mem.stack.Pop();
                Z80.BC = Mem.stack.Pop();
                Z80.AF = Mem.stack.Pop();
                Z80.A++;
                Z80.B--;
            } while (Z80.B != 0);
            goto VCON3;
        VCON2:
            Z80.BC = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
        VCON3:
            Mem.LD_8(OTONUM, Z80.A);// ﾂｶﾜﾚﾃﾙ ｵﾝｼｮｸ ﾉ ｶｽﾞ
            Z80.HL = Mem.stack.Pop();
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.HL = Mem.LD_16(ENDADR);
            Mem.stack.Push(Z80.HL);
            Z80.DE = MU_NUM;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Mem.LD_16(SSGDAT, Z80.HL);
            Z80.HL = Mem.stack.Pop();
        //SCON4:
            // Z80.HL = Mem.LD_16(ENDADR);
            Mem.stack.Push(Z80.HL);
            msub.HEXPRT(); //PR END ADR
            Z80.HL = MOJIBUF;
            Z80.DE = TEXT + 36;
            Z80.BC = 4;
            Z80.LDIR();
            Z80.DE = MU_NUM;
            Z80.HL = Mem.stack.Pop();
            Z80.HL -= (ushort)(Z80.DE + (Z80.Carry ? 1 : 0));
            Z80.HL++;
            msub.HEXPRT();//PR LENGTH
            Z80.HL = MOJIBUF;
            Z80.DE = TEXT + 41;
            Z80.BC = 4;
            Z80.LDIR();
            Z80.A = Mem.LD_8(ASEMFG);
            Z80.A--;
            if (Z80.A == 0)
            {
                CMPEN2();
                return;
            }
            msub.ROM();
            //Z80.A = Mem.LD_8(DEBFLG);
            Z80.A = DEBFLG;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto PGMEND;
            }

            //Z80.HL = VNMESS;
            expand.DSPMSG_msg = VNMESS;
            expand.DSPMSG();
            Z80.A = 5;
            expand.COLOR();
            Z80.A = Mem.LD_8(OTONUM);
            Z80.E = Z80.A;
            Z80.D = 0;
            expand.PRNFAC();
            Z80.A = 7;
            expand.COLOR();
            Z80.A = 4;
            expand.COLOR();
            //Z80.HL = TCMESS;
            expand.DSPMSG_msg = TCMESS;
            expand.DSPMSG();
            Z80.A = 7;
            expand.COLOR();
            Z80.HL = T_CLK;
            VCONX();
            // --	ﾙｰﾌﾟ ｶｳﾝﾄ ｹｲｻﾝ	--
            Z80.HL = T_CLK;
            //Z80.A = Mem.LD_8(MAXCH);
            Z80.A = MAXCH[0];
            Z80.B = Z80.A;
        //VCON4:
            do
            {
                Z80.E = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.D = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Mem.stack.Push(Z80.HL);
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.H = Mem.LD_8(Z80.HL);
                Z80.L = Z80.A;
                Z80.A |= Z80.H;
                if (Z80.A != 0)
                {
                    goto VCON5;
                }
                Z80.DE = 0;
                goto VCON6;
            VCON5:
                Z80.HL--;
                Z80.EX_DE_HL();
                Z80.A &= Z80.A;
                Z80.HL -= Z80.DE;
                Z80.EX_DE_HL();
            VCON6:
                Z80.HL = Mem.stack.Pop();
                Mem.LD_8(Z80.HL, Z80.E);
                Z80.HL++;
                Mem.LD_8(Z80.HL, Z80.D);
                Z80.HL++;
                Z80.B--;
            } while (Z80.B != 0);

            Z80.A = 6;
            expand.COLOR();
            //Z80.HL = LCMESS;
            expand.DSPMSG_msg = LCMESS;
            expand.DSPMSG();
            Z80.A = 7;
            expand.COLOR();
            Z80.HL = T_CLK + 2;
            VCONX();
            //Z80.A = Mem.LD_8(VPCO);
            Z80.A = VPCO;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto PGMEND;
            }
            Mem.stack.Push(Z80.AF);
            //Z80.HL = ADMES;
            expand.DSPMSG_msg = ADMES;
            expand.DSPMSG();
            Z80.AF = Mem.stack.Pop();
            Z80.E = Z80.A;
            Z80.D = 0;
            expand.PRNFAC();
        PGMEND:
            //DI
            INIT();
            PC88.CALL(WORKINIT);
            Z80.HL = Mem.LD_16(JCLOCK);
            Z80.A = Z80.H;
            Z80.A |= Z80.L;
            if (Z80.A == 0)
            {
                PGME2();
                return;
            }
            Mem.stack.Push(Z80.HL);
            //Z80.HL = JPLMES;
            Z80.HL = 0;
            Z80.DE = TEXT + 23;
            Z80.BC = 25;
            Z80.LDIR_HL(Encoding.UTF8.GetBytes(JPLMES));
            Z80.HL = Mem.LD_16(JPLINE);
            msub.HEXDEC();
            Z80.DE = TEXT + 48;
            Z80.BC = 5;
            Z80.LDIR();
            PC88.CALL(START);
            PC88.CALL(MSTOP);
            Z80.HL = Mem.stack.Pop();
        //DI
        PGME1:
            Mem.stack.Push(Z80.HL);
            PC88.CALL(DRIVE);
            Z80.HL = Mem.stack.Pop();
            Z80.HL--;
            Z80.A = Z80.H;
            Z80.A |= Z80.L;
            if (Z80.A != 0)
            {
                goto PGME1;
            }
            //EI
            PC88.CALL(ENBL);
            REDOF();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void PGME2()
        {
            REDOF();
            PC88.CALL(START);
            msub.T_RST();// RESET TIME
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        // --	ﾄｰﾀﾙ/ﾙｰﾌﾟｶｳﾝﾄ ﾋｮｳｼﾞ	--
        // IN:HL
        public void VCONX()
        {
            //Z80.A = Mem.LD_8(MAXCH);
            Z80.A = MAXCH[0];
            Z80.B = Z80.A;
            Z80.C = 0x41;// 'A'
        //VCONX2:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.A = Z80.C;
                PC88.CALL(0x03E0D);
                Z80.A = 0x3a;// ':'
                PC88.CALL(0x03E0D);
                Z80.E = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.D = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.HL++;
                Z80.HL++;
                Mem.stack.Push(Z80.HL);
                expand.PRNFAC();
                Z80.A = 0x20;// ' '
                PC88.CALL(0x03E0D);
                Z80.HL = Mem.stack.Pop();
                Z80.BC = Mem.stack.Pop();
                Z80.C++;
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        // --	Aregﾉ ｵﾝｼｮｸｱﾄﾞﾚｽｦﾓﾄﾒﾙ	--
        // EXIT:HL

        public void GETADR()
        {
            Mem.stack.Push(Z80.DE);
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.E = Z80.A;
            Z80.A &= 0b0001_1111;
            Z80.D = Z80.A;
            Z80.A = Z80.E;
            Z80.A &= 0b1110_0000;
            Z80.E = Z80.A; // BC = A * 32
            Z80.HL = FMLIB;
            Z80.HL += Z80.DE;
            Z80.DE = Mem.stack.Pop();
            //    RET
        }

        // --	COMPILE ONLY	--

        public void CMPEN2()
        {
            msub.ROM();
            Z80.HL = Mem.stack.Pop();
            Z80.A = 1;
            Mem.LD_8(ESCAPE, Z80.A);
            //EI
            //    RET
        }

        // *	FM COMPILED	*

        public void FMCOMP()
        {
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                if (Z80.A - 0x20 == 0)//ONE SPACE?
                {
                    goto FCOMP0;
                }
                Z80.HL++;
            } while (true);
        //goto FMCOMP;
        FCOMP0:
            Z80.HL++;// SKIP SPACE
            do
            {
                if (FCOMP_nextRtn == enmFCOMPNextRtn.comprc)
                {
                    COMPRC();
                    switch (COMTBL_RetPtn)
                    {
                        case 1:
                            FCOMP_nextRtn = enmFCOMPNextRtn.fcomp1;
                            break;
                        case 12:
                            FCOMP12();
                            break;
                        case 13:
                            FCOMP13();
                            break;
                        case 14:
                            FCOMP_nextRtn = enmFCOMPNextRtn.NextLine;
                            break;
                        default:
                            FCOMP_nextRtn = enmFCOMPNextRtn.occuredERROR;
                            break;
                    }
                }
                else
                {
                    FCOMP1();
                }
                if(FCOMP_nextRtn== enmFCOMPNextRtn.occuredERROR)
                {
                    break;
                }
            } while (FCOMP_nextRtn != enmFCOMPNextRtn.NextLine);
        }

        public enum enmFCOMPNextRtn
        {
            Unknown,
            NextLine,
            comprc,
            occuredERROR,
            fcomp1
        }

        public enmFCOMPNextRtn FCOMP_nextRtn = enmFCOMPNextRtn.Unknown; 

        public enum enmCMPENDNextRtn
        {
            Unknown,
            Success,
            occuredERROR,
            CSTART2
        }
        public enmCMPENDNextRtn CMPEND_nextRtn = enmCMPENDNextRtn.Unknown;

        public void FCOMP1()
        {
            FCOMP_nextRtn = enmFCOMPNextRtn.Unknown;
            Z80.A ^= Z80.A;
            Mem.LD_8(BEFRST, Z80.A);
            Mem.LD_8(TIEFG, Z80.A);
            FCOMP12();
        }

        public void FCOMP12()
        {
            FCOMP_nextRtn = enmFCOMPNextRtn.Unknown;
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.A |= Z80.A;// DATA END?
                if (Z80.A == 0)
                {
                    FCOMP_nextRtn = enmFCOMPNextRtn.NextLine;
                    return;// ﾂｷﾞ ﾉ ｷﾞｮｳﾍ
                }
            } while (Z80.A - 0x20 == 0);//CHECK SPACE
            Z80.HL--;
            msub.FMCOMC();// COM CHECK
            Z80.A = Z80.C;
            Mem.LD_8(COMNO, Z80.A);
            Z80.A |= Z80.A;     	// COM?
            if (Z80.A != 0)
            {
                //COMPRC();// ｺﾏﾝﾄﾞ ﾅﾗ COMPRC
                FCOMP_nextRtn = enmFCOMPNextRtn.comprc;
                return;
            }
            Z80.A = Mem.LD_8(Z80.HL);
            msub.STTONE();// TONE SET
            if (Z80.Carry)
            {
                //msub.ERRORSN();
                FCOMP_nextRtn = enmFCOMPNextRtn.occuredERROR;
                return;
            }
            Z80.C = Z80.A;
            Z80.A = Mem.LD_8(BEFTONE);
            Z80.Zero = (Z80.A - Z80.C == 0);
            Z80.A = Z80.C;
            if (Z80.Zero)
            {
                goto FC11;
            }
            Z80.DE = Mem.LD_16(MDATA);
            Mem.LD_16(TONEADR, Z80.DE);
            goto FCOMP16;
        FC11:
            Z80.A = Mem.LD_8(TIEFG);//KUMA:タイかどうか
            Z80.A |= Z80.A;
            Z80.Zero = (Z80.A == 0);
            Z80.A = Z80.C;
            if (Z80.Zero)
            {
                goto FCOMP16;//タイではない
            }
            Z80.HL++;
            FCOMP13();
            return;
        FCOMP16:
            Z80.HL++;
            FC162();
            //goto FCOMP1;
            FCOMP_nextRtn = enmFCOMPNextRtn.fcomp1;
            return;
        }

        public void FCOMP13()
        {
            FCOMP_nextRtn = enmFCOMPNextRtn.Unknown;
            Z80.DE = Mem.LD_16(MDATA);
            Z80.DE--;
            Z80.DE--;
            Z80.DE--;
            Mem.LD_16(MDATA, Z80.DE);
            //Mem.LD_8(FCP18 + 1, Z80.A);	// STORE TONE
            FCP18_VAL = Z80.A;
            msub.STLIZM();
            TCLKSUB();
            Z80.C = Z80.A;
            Z80.A = Mem.LD_8(BEFCO);
            Z80.Carry = (Z80.A + Z80.C > 0xff);
            Z80.A += Z80.C;
            if (!Z80.Carry)
            {
                goto FC14;
            }
            Z80.A = Mem.LD_8(BEFCO);
            Z80.B = Z80.A;
            Z80.A = 127;
            Z80.A -= Z80.B;
            Z80.B = Z80.A;
            Z80.A = Z80.C;
            Z80.A -= Z80.B;
            Z80.C = Z80.A;
            Z80.A = 127;
            msub.MWRIT2();
            //Z80.A = Mem.LD_8(FCP18 + 1);
            Z80.A = FCP18_VAL;
            msub.MWRIT2();
            Z80.A = 0xfd;
            msub.MWRIT2();
            Z80.A = Z80.C;
        FC14:
            FCOMP17();
            //goto FCOMP1;
            FCOMP_nextRtn = enmFCOMPNextRtn.fcomp1;
            return;

            //KUMA:↓FCOMP12へ移動
            //FCOMP16:
            //    Z80.HL++;
            //    FC162();
            //    goto FCOMP1;
            // --
        }

        public void FC162()
        {
            //Mem.LD_8(FCP18 + 1, Z80.A);	// STORE TONE
            FCP18_VAL = Z80.A;
            msub.STLIZM();// LIZM SET
            TCLKSUB();// ﾄｰﾀﾙｸﾛｯｸ ｶｻﾝ
            FCOMP17();
            //    RET
        }

        public void FCOMP17()
        {
            Mem.LD_8(BEFCO + 1, Z80.A);
            if (Z80.A - 128 < 0)
            {
                FCOMP2();
                return;
            }
            // --	ｶｳﾝﾄ ｵｰﾊﾞｰ ｼｮﾘ	--
            Z80.A -= 127;
            Z80.C = Z80.A;// STORE
            Z80.A = 127;// FIRST COUNT
            msub.MWRIT2();
            //Z80.A = Mem.LD_8(FCP18 + 1);	// TONE DATA
            Z80.A = FCP18_VAL;
            msub.MWRIT2();
            Z80.A = 0xfd;	// COM OF COUNT OVER(SOUND)
            msub.MWRIT2();
            Z80.A = Z80.C;// RESTORE SECOND COUNT
            Mem.LD_8(BEFCO, Z80.A);
            msub.MWRIT2();
            Z80.EXX();
            Z80.HL = BEFTONE + 7;
            Z80.DE = BEFTONE + 8;
            Z80.BC = 8;
            Z80.LDDR();
            Z80.EXX();
        //FCP18:
            Z80.A = FCP18_VAL;
            Mem.LD_8(BEFTONE, Z80.A);
            msub.MWRIT2();
            //  RET
        }

        // --	ﾉｰﾏﾙ ｼｮﾘ	--
        public void FCOMP2()
        {
            Z80.DE = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.DE, Z80.A);// SAVE LIZM
            Mem.LD_8(BEFCO, Z80.A);
            Z80.DE++;
            //Z80.A = Mem.LD_8(FCP18 + 1);
            Z80.A = FCP18_VAL;
            Mem.LD_8(Z80.DE, Z80.A);// SAVE TONE
            Z80.EXX();
            Z80.HL = BEFTONE + 7;
            Z80.DE = BEFTONE + 8;
            Z80.BC = 8;
            Z80.LDDR();
            Z80.EXX();
            Mem.LD_8(BEFTONE, Z80.A);
            Z80.DE++;
            Mem.LD_16(MDATA, Z80.DE);	// SET NEW SAVING ADR
            //    RET
        }

        // *	COM PROCESS(FM)    *

        public void COMPRC()
        {
            Z80.C--;
            Z80.A = Z80.C;
            Z80.A += Z80.A;
            Z80.A += Z80.C;// *3
            Z80.DE = 0;// COMTBL;
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;

            //KUMA:オリジナルは以下の2行でCOMTBLのルーチンがコールされる。
            //更に一部を除き、戻り先がない状態なのでRETせずにJR/JPなどで任意の処理に飛ぶ
            //Mem.stack.Push(Z80.DE);
            //RET
            //KUMA:代わりにデリゲートし、飛び先はFMCOMPで判断する
            COMTBL_RetPtn = -1;
            //log.Write(string.Format("Command:{0}",COMTBL[Z80.DE/3].Method.ToString()));
            COMTBL[Z80.DE / 3]();
        }

        public muc88()
        {
            COMTBL = new Action[]
            {
             SETLIZ
            , SETOCT
            , SETDT
            , SETVOL
            , SETCOL
            , SETOUP
            , SETODW
            , SETVUP
            , SETVDW
            , SETTIE
            , SETREG
            , SETMOD
            , SETRST
            , SETLPS
            , SETLPE
            , SETSE
            , SETJMP
            , SETQLG
            , SETSEV
            , SETMIX
            , SETWAV
            , TIMERB
            , SETCLK
            , COMOVR
            , SETKST
            , SETRJP
            , TOTALV
            , SETBEF
            , SETHE
            , SETHEP
            , SETDCO
            , SETLR
            , SETHLF
            , SETTMP
            , SETTAG
            , SETMEM
            , SETRV
            , SETMAC
            , STRET //RETで戻る!
            , SETTI2
            , SETSYO
            , ENDMAC
            , SETPTM
            , SETFLG
            };
        }

        //KUMA:どこからも参照されない？
        public void NTMN3()
        {
            //Z80.HL++;
            //goto FCOMP1;
        }

        // *	!	*

        public void COMOVR()
        {
            Z80.HL = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();
            CMPEND();
        }

        // --	ﾁｬﾝﾈﾙ CHECK	--

        // EXIT:SCF as SSG
        //	A=0-5 as FM
        //	6 as RHYTHM
        //	7 as pcm

        public void CHCHK()
        {
            Z80.A = Mem.LD_8(COMNOW);
            Z80.Carry = (Z80.A - 3 < 0);
            if (Z80.A - 3 >= 0)
            {
                goto CHE2;
            }
            Z80.Carry = false;
            Z80.A &= Z80.A;
            return;
        CHE2:
            Z80.A -= 3;
            Z80.Carry = (Z80.A - 3 < 0);
            if (Z80.A - 3 < 0)
            {
                return;//ssg
            }
            if (Z80.A - 3 != 0)
            {
                goto CHE3;
            }
            Z80.A = 6;
            Z80.Carry = false;
            Z80.A &= Z80.A;
            return;
        CHE3:
            Z80.Carry = (Z80.A - 7 < 0);
            if (Z80.A - 7 >= 0)
            {
                return;
            }
            Z80.A--;
            Z80.Carry = false;
            Z80.A &= Z80.A;
            //    RET
        }

        // --	ｶｸ Chﾉ ﾄｰﾀﾙｸﾛｯｸ(ﾜｰｸ) ｦ ｶｻﾝｽﾙ	--

        //IN:A<=CLOCK
        //AF,HL,DEﾊ ﾎｿﾞﾝ

        public void TCLKSUB()
        {
            Z80.EXX();
            Mem.stack.Push(Z80.AF);
            TCLKADR();
            Z80.E = Z80.L;
            Z80.D = Z80.H;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.AF = Mem.stack.Pop();
            Mem.stack.Push(Z80.AF);
            Z80.Carry = (Z80.A + Z80.L > 0xff);
            Z80.A += Z80.L;
            Z80.L = Z80.A;
            Z80.A += (byte)(Z80.H + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.L;
            Z80.H = Z80.A;
            Z80.EX_DE_HL();
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL--;
            Z80.EX_DE_HL();
            Z80.AF = Mem.stack.Pop();
            Z80.EXX();

            //    RET
        }

        //	OUT:HL<=ADR

        public void TCLKADR()
        {
            Z80.A = Mem.LD_8(COMNOW);
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.HL = T_CLK;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            //    RET
        }

        public void TCLKCLS()
        {
            TCLKADR();
            Z80.A ^= Z80.A;
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.A);
            //RET
        }

        // *	FLAGDATA SET	*

        public void SETFLG()
        {
            Z80.HL++;
            msub.REDATA();
            if (!Z80.Carry)
            {
                goto STF2;
            }
            Z80.E = 0xFF;
        STF2:
            Z80.A = 0xf9;
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	ﾏｸﾛｾｯﾄ*

        public void SETMAC()
        {
            Z80.HL++;
            msub.REDATA();
            Z80.A = Z80.E;
            //Mem.LD_8(STM2 + 1, Z80.A);
            STM2_VAL = Z80.A;
            //Z80.DE = Mem.LD_16(ADRSTC);
            Z80.DE = ADRSTC;
            Z80.EX_DE_HL();
            PC88.OUT(0x5C, Z80.A);
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);//TEXTPOINTER
            Z80.HL++;
            Z80.DE = Mem.LD_16(LINKPT);
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);	//LINKPOINTER
            Z80.HL++;
            //Mem.LD_16(ADRSTC, Z80.HL);
            ADRSTC = Z80.HL;
            Z80.HL = Mem.LD_16(LINE);
            //Mem.LD_16(EMAC2 + 1, Z80.HL);
            EMAC2_VAL = Z80.HL;
        //STM2:
            Z80.E = STM2_VAL;// 0;
            Z80.H = 0;
            Z80.D = Z80.H;
            Z80.L = Z80.E;
            Z80.HL += Z80.DE;
            Z80.HL += Z80.HL;
            Z80.DE = 0xC000;
            Z80.HL += Z80.DE;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);   //DE=MACROSTART.ADR
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                STMERR();
                return;
            }
            Z80.HL++;
            Mem.stack.Push(Z80.DE);
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Mem.LD_16(LINKPT, Z80.DE);
            Z80.HL = Mem.stack.Pop();	//ﾏｸﾛﾃｷｽﾄﾍHLｦｳﾂｽ
            PC88.OUT(0x5F, Z80.A);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void STMERR()
        {
            PC88.OUT(0x5F, Z80.A);
            Z80.HL = Mem.stack.Pop();
            msub.ERRNMC();
            return;
        }

        public void STMER2()
        {
            Z80.HL = Mem.stack.Pop();
            msub.ERREMC();
            return;
        }

        public void ENDMAC()
        {
            //Z80.A = Mem.LD_8(MACFG);
            Z80.A = MACFG;
            if (Z80.A - 0xfd != 0)
            {
                //GOSUB?
                goto EMAC2;
            }
            Z80.A++;
            //Mem.LD_8(MACFG, Z80.A);
            MACFG = Z80.A;
            Z80.HL++;
            return;// FMCOMPｼｮﾘｶﾗﾇｹﾙ

        EMAC2:
            Z80.HL = EMAC2_VAL;// 0;
            Mem.LD_16(LINE, Z80.HL);
            //Z80.HL = Mem.LD_16(ADRSTC);
            Z80.HL = ADRSTC;
            PC88.OUT(0x5C, Z80.A);
            Z80.HL--;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL--;
            Z80.E = Mem.LD_8(Z80.HL);
            Mem.LD_16(LINKPT, Z80.DE);
            Z80.HL--;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL--;
            Z80.E = Mem.LD_8(Z80.HL);
            //Mem.LD_16(ADRSTC, Z80.HL);
            ADRSTC = Z80.HL;
            Z80.EX_DE_HL();
            PC88.OUT(0x5F, Z80.A);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void STRET()
        { 
            //KUMA:注意！戻り先をスタックに積んでいると思われる。
            //    RET
        }

        // *	ﾎﾟﾙﾀst*

        public void SETPTM()
        {
            Mem.stack.Push(Z80.HL);
            Z80.DE = Mem.LD_16(MDATA);
            Z80.HL = 0;// PTMDAT;
            Z80.BC = 4;
            Z80.LDIR_HL(PTMDAT);//KUMA:Mコマンドのテンプレを4byte書き込む(0xf4,0,1,1)
            Mem.LD_16(BEFMD, Z80.DE);//KUMA:DEPTHの書き込み位置を退避
            Z80.DE++;
            Z80.DE++;
            Z80.A = 255;
            Mem.LD_8(Z80.DE, Z80.A);//KUMA:回数(255回)を書き込む
            Z80.DE++;
            Mem.LD_16(MDATA, Z80.DE);//KUMA:Mコマンドの次の位置をMDATAに退避)
            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);//KUMA:音符(文字)を読み込み
            msub.STTONE();//KUMA:オクターブ情報などを含めた音符情報に変換
            if (Z80.Carry)
            {
                msub.ERRORSN();//KUMA:エラーが発生したらエラー処理へ
                return;
            }
            Z80.HL++;
            FC162();//SET TONE&LIZ
            Mem.stack.Push(Z80.HL);
            Z80.DE = Mem.LD_16(MDATA);
            Z80.A = 0xF4;
            Mem.LD_8(Z80.DE, Z80.A); //KUMA:2個目のMコマンド作成開始
            Z80.DE++;
            //Z80.A = Mem.LD_8(LFODAT);
            Z80.A = LFODAT[0]; //KUMA:現在のLFOのスイッチを取得
            Z80.A--;
            if (Z80.A == 0)//KUMA:OFF(1)の場合はSTP1で2個めのMコマンドへOFF(1)を書き込む
            {
                goto STP1;
            }
            Z80.A ^= Z80.A;
            Mem.LD_8(Z80.DE, Z80.A);//KUMA:ON(0)の場合は2個めのMコマンドへON(0)を書き込む
            Z80.DE++;
            //Z80.HL = LFODAT + 1;
            Z80.HL = 1;
            Z80.BC = 5;
            Z80.LDIR_HL(LFODAT);//KUMA:残りの現在のLFOの設定5byteをそのまま２個目のMコマンドへコピー
            goto STP2;
        STP1:
            Z80.A = 1;
            Mem.LD_8(Z80.DE, Z80.A);
            Z80.DE++;
        STP2:
            Mem.LD_16(MDATA, Z80.DE);//KUMA:MDATAの位置を退避
            Z80.HL = Mem.stack.Pop();
        //STP22:
            Z80.A = Mem.LD_8(Z80.HL);//KUMA:次のコマンドを取得
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                msub.ERRORSN();//KUMA:データがない場合はエラー処理へ
                return;
            }
            if (Z80.A - 0x7d == 0)//'}'
            {
                msub.ERRORNE();
                return;
            }
            if (Z80.A - 0x3e == 0)//'>'
            {
                SOU1();
            }
            if (Z80.A - 0x3c == 0)//'<'
            {
                SOD1();
            }
            expand.CULPTM();//KUMA:DEPTHを計算
            if (Z80.Carry)
            {
                msub.ERRORSN();
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.HL = Mem.LD_16(BEFMD);
            Mem.LD_8(Z80.HL, Z80.E);//KUMA:DE(DEPTH)を書き込む
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL++;
            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x7d != 0)//'}'
            {
                msub.ERRORSN();
                return;
            }
            Z80.HL++;
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public byte[] PTMDAT = new byte[] {
            0xF4,0,1,1
        };

        // *	ｼｮｳｾﾂﾏｰｸ*

        public void SETSYO()
        {
            Z80.HL++;
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	ﾘﾊﾞｰﾌﾞ*

        public void SETRV()
        {
            CHCHK();
            if (Z80.A - 6 >= 0)
            {
                msub.ERRORDC();
                return;
            }
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x6d == 0)//'m'
            {
                goto SRV3;
            }
            if (Z80.A - 0x46 == 0)//'F'
            {
                goto SRV4;
            }
            Z80.HL--;
            Z80.A = 0xff;
            Z80.E = 0xf3;
        SRV2:
            msub.MWRITE();
            msub.ERRT();
            Z80.A = Z80.E;
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        SRV3:
            Z80.A = 0xff;
            Z80.E = 0xf4;
            goto SRV2;
        SRV4:
            Z80.A = 0xff;
            Z80.E = 0xf5;
            goto SRV2;
        }

        // *	ﾁｭｳﾔｸ*

        public void SETMEM()
        {
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.A |= Z80.A;
            } while (Z80.A != 0);
            //    RET
            COMTBL_RetPtn = 14;//KUMA:NextLine
        }

        // *	SET TAG & JUMP TO TAG*

        public void SETTAG()
        {
            Mem.stack.Push(Z80.HL);
            TCLKADR();
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Mem.LD_16(JCLOCK, Z80.DE);
            Z80.HL = Mem.LD_16(LINE);
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Mem.LD_16(JPLINE, Z80.HL);
            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	HARD LFO	*

        public void SETHLF()
        {
            CHCHK();
            if (Z80.Carry)
            {
                msub.ERRORDC();
                return;
            }

            if (Z80.A - 6 >= 0)
            {
                msub.ERRORDC();
                return;
            }

            Z80.A = 0xfc;
            msub.MWRIT2();
            msub.ERRT();
            Z80.A = Z80.E;
            msub.MWRIT2();
            msub.ERRT();
            Z80.A = Z80.E;
            msub.MWRIT2();
            msub.ERRT();
            Z80.A = Z80.E;
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	STEREO PAN	*

        public void SETLR()
        {
            msub.ERRT();
            Z80.A = 0xf8;	// COM OF 'p'
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	DIRECT COUNT	*

        public void SETDCO()
        {
            msub.ERRT();
            Z80.A = Z80.E;
            Mem.LD_8(COUNT, Z80.A);
            //goto FCOMP12;
            COMTBL_RetPtn = 12;
            return;
        }

        // *	SET HARD ENVE TYPE/FLAG*

        public void SETHE()
        {
            Z80.HL++;
            Z80.A = 0xff;
            Z80.E = 0xf1; // 2nd COM
            msub.MWRITE();
            msub.REDATA();
            Z80.A = Z80.E;
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void SETHEP()
        {
            Z80.HL++;
            Z80.A = 0xff;
            Z80.E = 0xf2;
            msub.MWRITE();
            msub.REDATA();
            Z80.A = Z80.E;
            Z80.E = Z80.D;
            msub.MWRITE();// 2ﾊﾞｲﾄﾃﾞｰﾀ ｶｸ
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	BEFORE CODE	*

        public void SETBEF()
        {
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x3d != 0)//'='
            {
                goto STBF3;
            }
            msub.ERRT();
            Z80.A = Z80.E;
            if (Z80.A - 10 >= 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto STBF2;
            }
            Z80.A++;
        STBF2:
            Z80.A--;
            //Mem.LD_8(BFDAT, Z80.A);
            BFDAT = Z80.A;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c == 0)//','
            {
                goto STBF21;
            }
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        STBF21:
            msub.ERRT();
            Z80.A = Z80.E;
            Mem.LD_8(VDDAT, Z80.A);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        STBF3:
            Z80.A = Mem.LD_8(VDDAT);
            Z80.A = (byte)-Z80.A;
            Z80.E = Z80.A;
            Z80.A = 0xFB;
            msub.MWRITE();
            Z80.A = Mem.LD_8(BEFCO);
            msub.MWRIT2();
            Z80.A = Mem.LD_8(BEFCO);
            TCLKSUB();
            //Z80.A = Mem.LD_8(BFDAT);
            Z80.A = BFDAT;
            Mem.stack.Push(Z80.HL);
            Z80.HL = BEFTONE;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            msub.MWRIT2();
            Z80.HL = Mem.stack.Pop();
            Z80.A = Mem.LD_8(VDDAT);
            Z80.E = Z80.A;
            Z80.A = 0xfb;
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public byte BFDAT = 1;

        // *	TOTAL VOLUME	*

        public void TOTALV()
        {
            msub.ERRT();
            Z80.A = Z80.E;
            //Mem.LD_8(TV_OFS, Z80.A);
            TV_OFS = Z80.A;
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public byte TV_OFS = 0; // TOTAL V.OFFSET

        // *	KEY SHIFT	*

        public void SETKST()
        {
            msub.ERRT();
            Z80.A = Z80.E;
            Mem.LD_8(SIFTDAT, Z80.A);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // *	CLOCK SET	*

        public void SETCLK()
        {
            msub.ERRT();
            Z80.A = Z80.E;
            Mem.LD_8(CLOCK, Z80.A);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	Q**

        public void SETQLG()
        {
            msub.ERRT();
            Z80.A = 0xf3;	// COM OF 'q'
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	JUMP ADDRESS SET**

        public void SETJMP()
        {
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            Z80.HL = Mem.LD_16(MDATA);
            TBOFS();
            Z80.EX_DE_HL();
            Z80.A = Mem.LD_8(COMNOW);
            CHTBL();
            Z80.HL++;
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            TCLKADR();
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.DE++;   // +1('L'ﾌﾗｸﾞﾉ ｶﾜﾘ)
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	TEMPO(TIMER_B) SET**

        public void TIMERB()
        {
            msub.ERRT();// E<=DATA
            TIMEB2();
        }

        public void TIMEB2()
        { 
            Mem.stack.Push(Z80.HL);
            Z80.HL = Mem.LD_16(DATTBL);
            Z80.HL--;// TIMER_B ﾆ ｱﾜｾﾙ
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL = Mem.stack.Pop();
            Z80.A = Mem.LD_8(COMNOW);
            Z80.A -= 3;
            if (Z80.A - 3 < 0)
            {
                //goto FCOMP1;
                COMTBL_RetPtn = 1;
                return;
            }
            Mem.stack.Push(Z80.DE);
            Z80.A = 0xfa;
            Z80.E = 0x26;
            msub.MWRITE();
            Z80.DE = Mem.stack.Pop();
            Z80.A = Z80.E;
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void SETTMP()
        {
            msub.ERRT();// T
            Z80.A = Z80.E;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                msub.ERRSN();
                return;
            }
            Mem.stack.Push(Z80.HL);
            Mem.stack.Push(Z80.DE);
            Z80.A = Mem.LD_8(CLOCK);
            Z80.L = Z80.A;
            Z80.H = 0;
            Z80.DE = 4;
            msub.DIV();// L
            Z80.DE = Mem.stack.Pop();
            msub.MULT();// T* L
            Z80.EX_DE_HL();
            Z80.HL = 60000;
            Z80.BC = 0;
        STTMP2:
            Z80.BC++;
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.DE < 0);
            Z80.HL -= Z80.DE;
            if (!Z80.Carry)
            {
                goto STTMP2;
            }
            Z80.E = Z80.C;
            Z80.HL = 346;
            msub.MULT();
            Z80.DE = 25600;
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.DE < 0);
            Z80.HL -= Z80.DE;
            if (Z80.Carry)
            {
                goto STTMP3;
            }
            Z80.HL = 0xff9c;//-100;
        STTMP3:
            Z80.EX_DE_HL();
            Z80.HL = 0;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE; // HL=-(346*SEC-256)
            Z80.DE = 100;
            msub.DIV();
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            TIMEB2();
        }

        // **	NOIZE WAVE	**

        public void SETWAV()
        {
            CHCHK();
            if (!Z80.Carry)
            {
                msub.ERRORDC();
                return;
            }
            msub.ERRT();
            Z80.A = 0xf8;	// COM OF 'w'
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	MIX PORT	**

        public void SETMIX()
        {
            CHCHK();
            if (!Z80.Carry)
            {
                msub.ERRORDC();
                return;
            }
            msub.ERRT();
            Z80.A = Z80.E;
            Z80.Zero = (Z80.A - 1) == 0;
            Z80.E = 8;
            if (Z80.Zero)
            {
                goto SETMI2;
            }
            Z80.Zero = (Z80.A - 2) == 0;
            Z80.E = 1;
            if (Z80.Zero)
            {
                goto SETMI2;
            }
            Z80.Zero = (Z80.A - 3) == 0;
            Z80.E = 0;
            if (Z80.Zero)
            {
                goto SETMI2;
            }
            Z80.A |= Z80.A;
            Z80.E = 9;
            if (Z80.A == 0)
            {
                goto SETMI2;
            }
            msub.ERRORSN();
            return;
        SETMI2:
            Z80.A = 0xf7;	// COM OF 'P'
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	SOFT ENVELOPE	**

        public void SETSEV()
        {
            CHCHK();
            if (!Z80.Carry)
            {
                msub.ERRORDC();
                return;
            }
            msub.ERRT();
                Z80.A = 0xfa;    // COM OF 'E'
            msub.MWRITE();
            Z80.B = 5;//ﾉｺﾘ 5 PARAMETER
            SETSE1();
        }

        // **	REST**

        public void SETRST()
        {
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x25 != 0)//'%'
            {
                goto SRS2;
            }
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                goto SETRS2;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            goto SETRS3;
        SRS2:
            msub.REDATA();
            if (Z80.Carry)
            {
                goto SETRS2;// NON DATA
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.A = Mem.LD_8(CLOCK);
            Z80.L = Z80.A;
            Z80.H = 0;
            msub.DIV();
            Z80.HL = Mem.stack.Pop();
            Z80.A = Mem.LD_8(KOTAE);
        SETRS0:
            Z80.C = Z80.A;// STORE
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.Zero = (Z80.A - 0x2e == 0);//'.'
            Z80.A = Z80.C;// RESTORE
            if (!Z80.Zero)
            {
                goto SETRS3;// ﾌﾃﾝ ｼﾞｬﾅｹﾚﾊﾞ SETRS3
            }
            Z80.HL++;
            Z80.B = Z80.A;
            Z80.A >>= 1;// /2
            Z80.C = Z80.A;
            Z80.A = Z80.B;
            goto SETRS4;
        SETRS3:
            Z80.C = 0;
        SETRS4:
            Z80.A += Z80.C;
            Z80.C = Z80.A;
            TCLKSUB();
            Z80.A = Mem.LD_8(BEFRST);   // ｾﾞﾝｶｲｶｳﾝﾀ ﾜｰｸ(ﾌﾗｸﾞ)
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto SETRS5;// ｾﾞﾝｶｲ ｶﾞ 'r'ﾅﾗ ﾂｷﾞﾍ
            }
            Z80.A = Mem.LD_8(BEFRST);
            Z80.A += Z80.C;
            Z80.C = Z80.A;
            Z80.DE = Mem.LD_16(MDATA);
            Z80.DE--;
            Mem.LD_16(MDATA, Z80.DE);
        SETRS5:
            Z80.A = Z80.C;
            if (Z80.A - 0x70 >= 0)
            {
                goto SETRS1;
            }
            Mem.LD_8(BEFRST, Z80.A);
            Z80.A |= 0b1000_0000;// SET REST FLAG
            msub.MWRIT2();
            //goto FCOMP12;
            COMTBL_RetPtn = 12;
            return;
        SETRS1:
            Z80.A -= 0x6f;
            Z80.C = Z80.A;
            Z80.A = 0b1110_1111;
            msub.MWRIT2();
            Z80.A = Z80.C;
            if (Z80.A - 0x70 >= 0)
            {
                goto SETRS1;
            }
            Mem.LD_8(BEFRST, Z80.A);
            Z80.A |= 0b1000_0000;
            msub.MWRIT2();
            //goto FCOMP12;
            COMTBL_RetPtn = 12;
            return;
        SETRS2:
            Z80.A = Mem.LD_8(COUNT);
            goto SETRS0;
        }

        // **	REPEAT JUMP	**

        public void SETRJP()
        {
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            Z80.A = 0xfe;
            msub.MWRIT2();
            //Z80.HL = Mem.LD_16(POINTC);
            Z80.HL = POINTC;
            Z80.DE = 4;
            Z80.HL += Z80.DE;
            Mem.stack.Push(Z80.HL);
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.A |= Z80.D;
            Z80.HL = Mem.stack.Pop();
            if (Z80.A != 0)
            {
                goto RJPER;//[] ﾆ / ﾊ ﾋﾄﾂ ﾀﾞｹ
            }
            Z80.DE = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL++;
            Z80.HL++;
            Z80.HL++;
            Z80.DE++;
            Z80.DE++;
            Mem.LD_16(MDATA, Z80.DE);
            Mem.stack.Push(Z80.HL);
            TCLKADR();
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL = Mem.stack.Pop();
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        RJPER:
            Z80.HL = Mem.stack.Pop();
            msub.ERRORRJ();
            return;
        }

        // **	LOOP START	**

        public void SETLPS()
        {
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            Z80.A = 0xf5;	// COM OF LOOPSTART
            msub.MWRIT2();
            //Z80.HL = Mem.LD_16(POINTC);
            Z80.HL = POINTC;
            Z80.DE = 10;
            Z80.HL += Z80.DE;
            //Mem.LD_16(POINTC, Z80.HL);
            POINTC = Z80.HL;
            Z80.DE = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.HL, Z80.E);		// SAVE REWRITE ADR
            Z80.HL++;       //
            Mem.LD_8(Z80.HL, Z80.D);        //
            Z80.HL++;
            Z80.DE++;
            Z80.DE++;
            Mem.LD_16(MDATA, Z80.DE);
            Mem.LD_8(Z80.HL, Z80.E);    // SAVE LOOP START ADR
            Z80.HL++;//
            Mem.LD_8(Z80.HL, Z80.D);        //
            Z80.HL++;
            Z80.A ^= Z80.A;
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            TCLKADR();
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL = Mem.stack.Pop();
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.A ^= Z80.A;
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.A);
            TCLKCLS();//ﾄｰﾀﾙ ｸﾛｯｸ ｸﾘｱ
            //Z80.HL = REPCOUNT;
            //Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) + 1));
            //Z80.A = Mem.LD_8(Z80.HL);
            REPCOUNT++;
            Z80.A = REPCOUNT;
            if (Z80.A - 17 >= 0)
            {
                goto SETLPS2;
            }
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        SETLPS2:
            Z80.HL = Mem.stack.Pop();
            msub.ERRORML();
            return;
        }

        public ushort POINTC = 0;
        // LOOPSTART ADR ｶﾞ
        // ｾｯﾃｲｻﾚﾃｲﾙ ADR

        public byte REPCOUNT = 0;

        // **	LOOP END	**

        public void SETLPE()
        {
            msub.ERRT();
            Mem.stack.Push(Z80.HL);
            Z80.A = 0xf6;		// WRITE COM OF LOOP
            msub.MWRIT2();
            Z80.A = Z80.E;
            msub.MWRIT2(); // WRITE LOOP Co.
            Z80.DE = Mem.LD_16(MDATA);
            msub.MWRIT2(); // WRITE LOOP Co. (SPEAR)
            Mem.stack.Push(Z80.AF);
            //Z80.HL = Mem.LD_16(POINTC);
            Z80.HL = POINTC;
            Z80.BC = LOOPSP;// STAC TOP
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.BC < 0);
            Z80.HL -= Z80.BC;
            if (Z80.Carry)
            {
                goto SETLPEE;// ']' ﾉ ｶｽﾞ ｶﾞ ｵｵｲ
            }
            //Z80.HL = Mem.LD_16(POINTC);
            Z80.HL = POINTC;
            Mem.stack.Push(Z80.HL);
            Z80.BC = 10;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.BC;
            //Mem.LD_16(POINTC, Z80.HL);
            POINTC = Z80.HL;
            Z80.HL = Mem.stack.Pop();

            Mem.stack.Push(Z80.HL);
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;

            Mem.stack.Push(Z80.HL);
            Z80.A &= Z80.A;
            Z80.EX_DE_HL();
            Z80.HL -= Z80.DE;
            Z80.EX_DE_HL(); // DE as OFFSET
            Z80.HL = Mem.stack.Pop();

            Mem.LD_8(Z80.HL, Z80.E);    //
            Z80.HL++;// RSKIP JP ADR
            Mem.LD_8(Z80.HL, Z80.D);  //

            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            Z80.HL++;
            Mem.stack.Push(Z80.HL);

            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;// HL ﾊ LOOP ｦ ｶｲｼｼﾀ ｱﾄﾞﾚｽ

            Z80.DE = Mem.LD_16(MDATA);
            Z80.EX_DE_HL();
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE; // HL as LOOP RET ADR OFFSET
            Z80.EX_DE_HL();

            Z80.HL = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.HL, Z80.E);        //
            Z80.HL++;// WRITE RET ADR OFFSET
            Mem.LD_8(Z80.HL, Z80.D);        //
            Z80.HL++;
            Mem.LD_16(MDATA, Z80.HL);

            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.A |= Z80.H;
            if (Z80.A == 0)
            {
                goto SETLPE1;
            }

            Z80.DE = Mem.LD_16(MDATA);
            Z80.DE--;
            Z80.DE--;
            Z80.DE--;
            Z80.DE--;
            Mem.stack.Push(Z80.HL);
            Z80.EX_DE_HL();
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.EX_DE_HL(); // DE as OFFSET
            Z80.HL = Mem.stack.Pop();

            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);

        SETLPE1:
            //Z80.HL = REPCOUNT;
            //Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) - 1));
            REPCOUNT--;
            TCLKADR();
            //Mem.LD_16(SETLPE4 + 1, Z80.HL);
            SETLPE4_VAL = Z80.HL;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.AF = Mem.stack.Pop();
            //Mem.LD_16(SETLPE3 + 1, Z80.HL);
            SETLPE3_VAL = Z80.HL;
            Z80.A--;
            Z80.E = Z80.A;
            msub.MULT();
            Z80.EX_DE_HL();
            //Z80.HL = Mem.LD_16(POINTC);
            Z80.HL = POINTC;
            Z80.BC = 16;
            Z80.HL += Z80.BC;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.HL += Z80.DE;
            Mem.stack.Push(Z80.HL);
            //Z80.HL = Mem.LD_16(POINTC);
            Z80.HL = POINTC;
            Z80.DE = 18;
            Z80.HL += Z80.DE;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL = Mem.stack.Pop();
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                goto SETLPE3;// '/'ﾊ ｶｯｺﾅｲﾆ ﾂｶﾜﾚﾃﾅｲ
            }
            Z80.HL += Z80.DE;
            goto SETLPE4;
        SETLPE3:
            Z80.DE = SETLPE3_VAL;//0000;
            Z80.HL += Z80.DE;
        SETLPE4:
            //Mem.LD_16(0x0000, Z80.HL);
            Mem.LD_16(SETLPE4_VAL, Z80.HL);
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        SETLPEE:
            Z80.AF = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();// DUMMY
            msub.ERRORNF();
        }

        // **	SE DETUNE ﾉ ｾｯﾃｲ	**

        public void SETSE()
        {
            Z80.A = Mem.LD_8(COMNOW);
            if (Z80.A - 2 != 0)
            {
                // 3 Ch ｲｶﾞｲﾅﾗ ERROR
                msub.ERRORDC();
                return;
            }
            msub.ERRT();
            Z80.A = 0xF7;// COM OF 'S'
            msub.MWRIT2();
            Z80.A = Z80.E;
            msub.MWRIT2();
            Z80.B = 3;      // ﾉｺﾘ 3 PARAMETER
            SETSE1();
        }

        public void SETSE1()
        { 
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                if (Z80.A - 0x2c != 0)//','
                {
                    msub.ERRORNE();
                    return;
                }
                Z80.HL++;
                msub.REDATA();
                if (Z80.Carry)
                {
                    // NONDATA ﾅﾗERROR
                    msub.ERRORNE();
                    return;
                }
                Z80.A |= Z80.A;
                if (Z80.A != 0)
                {
                    msub.ERRIF();
                    return;
                }
                Z80.A = Z80.E;
                msub.MWRIT2();// SET DATA ONLY
                Z80.B--;
            } while (Z80.B != 0);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	LFO SET	**

        public void SETMOD()
        {
            //Z80.IX = LFODAT;
            Z80.IX = 0;
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                SETMO2();// NONDATA ﾅﾗ 2nd COM PRC
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            //Mem.LD_8(Z80.IX, 0);
            LFODAT[Z80.IX] = 0;
            Z80.A = 0xf4;	// COM OF 'M'
            msub.MWRIT2();
            Z80.A = 0;	// 2nd COM
            msub.MWRIT2();
            Z80.A = Z80.E;
            //Mem.LD_8((ushort)(Z80.IX + 1), Z80.A);
            LFODAT[Z80.IX + 1] = Z80.A;
            msub.MWRIT2();// SET DELAY
                          // --	ｶｳﾝﾀ ｾｯﾄ	--
        //SETMO1:
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c != 0)//','
            {
                msub.ERRORNE();
                return;
            }
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                msub.ERRORNE();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            //Mem.LD_8((ushort)(Z80.IX + 2), Z80.A);
            LFODAT[Z80.IX + 2] = Z80.A;
            msub.MWRIT2();// SET DATA ONLY
                          // --	SET VECTOR	--
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c != 0)
            {
                msub.ERRORNE();
                return;
            }
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                msub.ERRORNE();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.HL = Mem.LD_16(MDATA);
            CHCHK();
            if (!Z80.Carry)
            {
                goto SETVEC2;
            }
            NEG16();//ssgの場合はdeの符号を反転
        SETVEC2:
            Mem.LD_8(Z80.HL, Z80.E);
            //Mem.LD_8((ushort)(Z80.IX + 3), Z80.E);
            LFODAT[Z80.IX + 3] = Z80.E;
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            //Mem.LD_8((ushort)(Z80.IX + 4), Z80.D);
            LFODAT[Z80.IX + 4] = Z80.D;
            Z80.HL++;
            Mem.LD_16(MDATA, Z80.HL);
            Z80.HL = Mem.stack.Pop();
        // --	SET DEPTH	--
        //SETPLAMU:
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c != 0)
            {
                msub.ERRORNE();
                return;
            }
            msub.ERRT();
            Z80.A = Z80.E;
            //Mem.LD_8((ushort)(Z80.IX + 5), Z80.A);
            LFODAT[Z80.IX + 5] = Z80.A;
            msub.MWRIT2();// SET DATA ONLY
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c != 0)
            {
                //goto FCOMP1;
                COMTBL_RetPtn = 1;
                return;
            }
            msub.ERRT();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void NEG16()//DE<=NEG.DE
        {
            Mem.stack.Push(Z80.HL);
            Z80.HL = 0;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        // --	LFO 2nd COM	--

        public void SETMO2()
        {
            Z80.HL++;
            Z80.A = 0xf4;   // COM OF 'M'
            msub.MWRIT2();// COM ONLY
            Z80.A = Mem.LD_8(SECCOM);
            if (Z80.A - 0x46 != 0)//'F'
            {
                goto MO4;
            }
            msub.REDATA();
            if (Z80.Carry) {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0) {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            Z80.A |= Z80.A;
            if (Z80.A != 0) {
                goto MO3;
            }
            Z80.E++;// SECOND COM
            Mem.LD_8(Z80.IX, Z80.E);
            Z80.A = Z80.E;
            msub.MWRIT2();// 'MF0'
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        MO3:
            Z80.A--;
            if (Z80.A != 0) {
                msub.ERRORSN();
                return;
            }
            Z80.E++;
            Mem.LD_8(Z80.IX, Z80.E);
            Z80.A = Z80.E;
            msub.MWRIT2();// 'MF1'
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        MO4:
            //Z80.IY = LFODAT + 1;
            Z80.IY = 1;
            if (Z80.A - 0x57 != 0)//'W'
            {
                goto M05;
            }
            Z80.A = 3;  // COM OF 'MW'
            MODP2();
            return;
        M05:
            Z80.IY++;
            if (Z80.A - 0x43 != 0)//'C'
            {
                goto M06;
            }
            Z80.A = 4;  // 'MC'
            MODP2();
            return;
        M06:
            Z80.IY++;
            if (Z80.A - 0x00 != 0)//'L'
            {
                goto M07;
            }
            Z80.A = 5;  // 'ML'
            Mem.LD_8(Z80.IX, Z80.A);
            msub.MWRIT2();
            msub.REDATA();
            if (Z80.Carry) {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0) {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            //Mem.LD_8(Z80.IY, Z80.A);
            LFODAT[Z80.IY] = Z80.A;
            Z80.E = Z80.D;
            //Mem.LD_8((ushort)(Z80.IY + 1), Z80.E);
            //Mem.LD_8((ushort)(Z80.IY + 2), 0);
            LFODAT[Z80.IY + 1] = Z80.E;
            LFODAT[Z80.IY + 2] = 0;
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        M07:
            Z80.IY++;
            Z80.IY++;
            Z80.IY++;
            if (Z80.A - 0x44 != 0)//'D'
            {
                goto M08;
            }
            Z80.A = 6;
            MODP2();
            return;
        M08:
            if (Z80.A - 0x54 != 0)//'T'
            {
                msub.ERRORSN();
                return;
            }
            CHCHK();
            if (Z80.Carry) {
                msub.ERRORDC();
                return;
            }
            msub.REDATA();
            if (Z80.Carry) {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0) {
                msub.ERRIF();
                return;
            }
            Z80.A = 7;
            msub.MWRITE();
            Z80.E = Z80.A;
            Z80.A |= Z80.A;
            if (Z80.A == 0) {
                //goto FCOMP1;
                COMTBL_RetPtn = 1;
                return;
            }
        //M083:
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry) {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0) {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	PARAMETER SET	**

        // IN: A<= COM No.

        public void MODP2()
        {
            Mem.LD_8(Z80.IX, Z80.A);
            msub.MWRIT2();
            msub.REDATA();
            if (Z80.Carry)
            {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            //Mem.LD_8(Z80.IY, Z80.E);
            LFODAT[Z80.IY] = Z80.E;
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	REGISTER SET	**

        public void SETREG()
        {
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                goto SETR2;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = 0xb2;
            if (Z80.A - Z80.E < 0)
            {
                msub.ERRIF();
                return;
            }
        SETR1:
            Z80.A = 0xfa;   // COM OF 'y'
            msub.MWRITE();
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c != 0)//','
            {
                msub.ERRORSN();
                return;
            }
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            msub.MWRIT2();// SET DATA ONLY
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        // --	yXX(ﾓｼﾞﾚﾂ),OpNo.,DATA	--
        SETR2:
            //Z80.DE = DMDAT;
            Z80.DE = 0;
            Z80.BC = 7 * 256;
        //SETR3:
            do
            {
                Mem.stack.Push(Z80.HL);

                Mem.stack.Push(Z80.BC);
                Mem.stack.Push(Z80.DE);
                //msub.MCMP();
                msub.MCMP_DE(DMDAT[Z80.DE / 3]);
                Z80.DE = Mem.stack.Pop();
                Z80.BC = Mem.stack.Pop();
                if (!Z80.Carry)
                {
                    goto SETR4;
                }
                Z80.DE++;
                Z80.DE++;
                Z80.DE++;
                Z80.C++;
                Z80.HL = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            msub.ERRORSN();
            return;
        SETR4:
            Z80.DE = Mem.stack.Pop();// HLﾊ POPｼﾅｲ!
            Z80.A = Z80.C;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.A;// *16
            //Mem.LD_8((ushort)(SETR9 + 1), Z80.A);
            SETR9_VAL = Z80.A;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2c != 0)//','
            {
                msub.ERRORSN();
                return;
            }
            msub.ERRT();// ｵﾍﾟﾚｰﾀｰ No.
            Z80.A = Z80.E;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                msub.ERRIF();
                return;
            }
            if (Z80.A - 5 >= 0)
            {
                msub.ERRIF();
                return;
            }
            if (Z80.A - 3 != 0)
            {
                goto SETR5;
            }
            Z80.A = 2;
            goto SETR6;
        SETR5:
            if (Z80.A - 2 != 0)
            {
                goto SETR6;
            }
            Z80.A = 3;
        SETR6:
            Z80.A--;
            Z80.A += Z80.A;
            Z80.A += Z80.A; // Op*4
            Z80.C = Z80.A;
            Z80.A = Mem.LD_8(COMNOW);
            if (Z80.A - 3 < 0)
            {
                goto SETR8;
            }
            Z80.A -= 7;
            if (Z80.A - 3 >= 0)
            {
                // CH A-C,H-Jﾅﾗ ｼﾖｳﾃﾞｷﾙ
                msub.ERRORDC();
                return;
            }
        SETR8:
            Z80.A += Z80.C;// Op*4+CH No.
            Z80.C = Z80.A;
        //SETR9:
            Z80.A = SETR9_VAL;
            Z80.A += 0x30;
            Z80.A += Z80.C;
            Z80.E = Z80.A;
            goto SETR1;
        }

        public string[] DMDAT = new string[] 
        {
            "DM\0"
            ,"TL\0"
            ,"KA\0"
            ,"DR\0"
            ,"SR\0"
            ,"SL\0"
            ,"SE\0"
        };

        // **	TIE**

        public void SETTIE()
        {
            SETTI1();
            //goto FCOMP12;
            COMTBL_RetPtn = 12;
            return;
        }

        public void SETTI1()
        {
            Z80.HL++;
            Z80.A = 0xfd;
            Mem.LD_8(TIEFG, Z80.A);
            msub.MWRIT2();
            // RET
        }

        public void SETTI2()
        {
            SETTI1();
            Z80.A = Mem.LD_8(BEFTONE);
            //goto FCOMP13;
            COMTBL_RetPtn = 13;
            return;
        }

        // **	OCTAVE UP & DOWN**

        public void SETOUP()
        {
            SOU1();
            if (Z80.Carry)
            {
                msub.ERROROO();
                return;
            }
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void SOU1()
        {
            Z80.HL++;
            //Z80.A = Mem.LD_8(UDFLG);
            Z80.A = UDFLG;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                SOD2();
                return;
            }
            SOU2();
        }

        public void SOU2()
        { 
            Z80.A = Mem.LD_8(OCTAVE);
            if (Z80.A - 7 == 0)
            {
                OO();
                return;
            }
            Z80.A++;
            Mem.LD_8(OCTAVE, Z80.A);
            Z80.A &= Z80.A;
            Z80.Carry = false;
            //  RET
        }

        public void SETODW()
        {
            SOD1();
            if (Z80.Carry)
            {
                msub.ERROROO();
                return;
            }
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void SOD1()
        {
            Z80.HL++;
            //Z80.A = Mem.LD_8(UDFLG);
            Z80.A = UDFLG;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                SOU2();
                return;
            }
            SOD2();
        }

        public void SOD2()
        { 
            Z80.A = Mem.LD_8(OCTAVE);
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                OO();
                return;
            }
            Z80.A--;
            Mem.LD_8(OCTAVE, Z80.A);
            Z80.A &= Z80.A;
            Z80.Carry = false;
            //  RET
        }

        public void OO()
        {
            Z80.Carry = true;
            //    RET
        }

        // **	VOL UP& DOWN SET	**

        public void SETVUP()
        {
            //Z80.A = Mem.LD_8(UDFLG);
            Z80.A = UDFLG;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                SVD2();
                return;
            }
            SVU2();
        }

        public void SVU2()
        {
            Z80.HL++;
            msub.REDATA();
            if (!Z80.Carry)
            {
                goto SETVU2;
            }
            Z80.E = 1;// ﾍﾝｶ 1
        SETVU2:
            Z80.A = Mem.LD_8(VOLUME);
            Z80.A += Z80.E;
            Mem.LD_8(VOLUME, Z80.A);
            Z80.A = 0xFB;// COM OF ')'
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void SETVDW()
        {
            //Z80.A = Mem.LD_8(UDFLG);
            Z80.A = UDFLG;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                SVU2();
                return;
            }
            SVD2();
        }

        public void SVD2()
        {
            Z80.HL++;
            msub.REDATA();
            if (!Z80.Carry)
            {
                goto SETVD2;
            }
            Z80.E = 1;
        SETVD2:
            Z80.A = Mem.LD_8(VOLUME);
            Z80.A -= Z80.E;
            Mem.LD_8(VOLUME, Z80.A);
            Z80.A = Z80.E;
            Z80.A = (byte)-Z80.A;// ﾌｺﾞｳ ﾊﾝﾃﾝ
            Z80.E = Z80.A;
            Z80.A = 0xfb;   // ')' ﾉ ﾊﾝﾀｲ ﾊ '('
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **	ｵﾝｼｮｸｾｯﾄ**

        public void SETCOL()
        {
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x22 == 0)//'"'
            {
                SETVN();//文字列による指定
                return;
            }
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x3d == 0)//'='
            {
                SETEV();
                return;
            }
            Z80.HL--;
            Z80.HL--;
            msub.ERRT();
            Z80.A = Z80.E;
            //Mem.LD_8((ushort)(STCL6 + 1), Z80.A);
            STCL6_VAL = Z80.A;
            CHCHK();
            if (Z80.Carry)
            {
                STCL5();//STCL3
                return;
            }
            if (Z80.A - 6 < 0)
            {
                STCL2();
                return;
            }
        //SETCO1:			//RHY&PCM
            Z80.C = Z80.E;
            Mem.stack.Push(Z80.HL);
            STCL72();
        }

        public void STCL2()      // FM
        {
            Z80.E++;
            Mem.stack.Push(Z80.HL);
            Z80.HL = DEFVOICE;
            CCVC();
            if (Z80.Carry)
            {
                STCL7();
                return;
            }
            Z80.HL = DEFVOICE;
            CWVC();
            if (!Z80.Carry)
            {
                STCL7();
                return;
            }
            Z80.HL = Mem.stack.Pop();
            msub.ERRORVF();
            return;
        }

        public void STCL3()      // SSG
        {
            Z80.E++;
            Mem.stack.Push(Z80.HL);
            Z80.HL = DEFVSSG;
            CCVC();
            if (Z80.Carry)
            {
                STCL5();
                return;
            }
            Z80.HL = DEFVSSG;
            CWVC();
            if (!Z80.Carry)
            {
                STCL5();
                return;
            }
            Z80.HL = Mem.stack.Pop();
            msub.ERRORVF();
            return;
        }

        public void STCL5()
        {
            Z80.C = Z80.E;
            STCL8();
            //Z80.A = Mem.LD_8(PSGMD);
            Z80.A = PSGMD;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                //goto FCOMP1;
                COMTBL_RetPtn = 1;
                return;
            }
        //STCL6:
            Z80.A = STCL6_VAL;
            Mem.stack.Push(Z80.HL);
            Z80.L = Z80.A;
            Z80.H = 0;
            Z80.DE = 16;
            msub.MULT();
            Z80.DE = SSGLIB;
            Z80.HL += Z80.DE;
            Z80.A = 0xf7;
            msub.MWRIT2();
            Z80.A = Mem.LD_8(Z80.HL);
            msub.MWRIT2();
            Z80.DE = 8;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.A--;
            if (Z80.A == 0)
            {
                STCL73();
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.DE = Mem.LD_16(MDATA);
            Z80.A = 0xf4;
            Mem.LD_8(Z80.DE, Z80.A);
            Z80.DE++;
            Z80.BC = 6;
            Z80.LDIR();
            Mem.LD_16(MDATA, Z80.DE);
            Z80.HL = Mem.stack.Pop();
            //Z80.DE = LFODAT;
            Z80.DE = 0;
            Z80.BC = 6;
            Z80.LDIR_DE(LFODAT);
            STCL73();
        }

        public void STCL7()
        {
            Z80.C--;
            STCL8();
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void STCL72()
        {
            STCL8();
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void STCL73()
        {
            Z80.HL = Mem.stack.Pop();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        public void STCL8()
        { 
            Z80.E = Z80.C;
            Z80.A = 0xf0;		// COM OF '@'
            msub.MWRITE();// E as VOICE NUMBER
                          //    RET
        }

        public void SETEV()
        {
            CHCHK();
            if (!Z80.Carry)
            {
                msub.ERRORDC();
                return;
            }
            Z80.HL--;
            Z80.HL--;
            msub.ERRT();
            Z80.A = Z80.E;
            if (Z80.A - 16 >= 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.E++;
            Mem.stack.Push(Z80.HL);
            Z80.HL = DEFVSSG;
            CCVC();
            if (Z80.Carry)
            {
                goto STEV2;
            }
            Z80.HL = DEFVSSG;
            CWVC();
            if (!Z80.Carry)
            {
                goto STEV2;
            }
            Z80.HL = Mem.stack.Pop();
            msub.ERRORVF();
            return;

        STEV2:
            Z80.E = Z80.C;
            Z80.E--;
            Z80.A = 0xfc;
            msub.MWRITE();
            Z80.HL = Mem.stack.Pop();
            msub.ERRT();
            Z80.A = Z80.E;
            msub.MWRIT2();
            Z80.B = 5;
            SETSE1();
        }

        public void SETVN()
        {
            CHCHK();
            if (Z80.Carry)
            {
                msub.ERRORDC();
                return;
            }
            if (Z80.A - 6 >= 0)
            {
                msub.ERRORDC();
                return;
            }
            Z80.HL++;
            Z80.DE = 0x6020 + 26;
            Z80.BC = 0xFF01;
        //SETVN1:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.B = 6;
                Mem.stack.Push(Z80.HL);
                Mem.stack.Push(Z80.DE);
            //SETVN2:
                do
                {
                    Z80.A = Mem.LD_8(Z80.DE);
                    if (Z80.A - Mem.LD_8(Z80.HL) != 0)
                    {
                        goto SETVN4;
                    }
                    Z80.HL++;
                    Z80.DE++;
                    Z80.A = Mem.LD_8(Z80.HL);
                    if (Z80.A - 0x22 == 0)//'"'
                    {
                        goto SETVN6;
                    }
                    Z80.B--;
                } while (Z80.B != 0);
                Z80.DE = Mem.stack.Pop();
                Z80.HL = Mem.stack.Pop();
                Z80.BC = Mem.stack.Pop();
                msub.ERRORVO();
                return;
            SETVN4:
                Z80.DE = Mem.stack.Pop();
                Z80.HL = 32;
                Z80.HL += Z80.DE;
                Z80.EX_DE_HL();
                Z80.HL = Mem.stack.Pop();
                Z80.BC = Mem.stack.Pop();
                Z80.C++;
                Z80.B--;
            } while (Z80.B != 0);
            msub.ERRORVO();
            return;
        SETVN6:
            Z80.HL++;
            Z80.AF = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            Z80.B--;
            if (Z80.B == 0)
            {
                goto SETVN8;
            }
            Z80.A = Mem.LD_8(Z80.DE);
            if (Z80.A - 0x20 != 0)
            {
                msub.ERRORVO();
                return;
            }
        SETVN8:
            Z80.E = Z80.C;
            STCL2();
        }

        // --	VOICE ｶﾞ ﾄｳﾛｸｽﾞﾐｶ?	--

        //IN:HL<=BUF
        public void CCVC()
        {
            Z80.B = 32;
            Z80.A = Z80.E;// GET VOICE NUMBER
            Z80.C = 1;
        //CCVC2:
            do
            {
                Z80.D = Mem.LD_8(Z80.HL);
                if (Z80.A - Z80.D == 0)
                {
                    goto CCVC3;// VOICE ﾊ ｽﾃﾞﾆ ﾄｳﾛｸｽﾞﾐ
                }
                Z80.HL++;
                Z80.C++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;//    RET
        CCVC3:
            Z80.Carry = true;
            //    RET
        }

        // --	WORK ﾆ ｱｷ ｶﾞ ｱﾙｶ?	--

        //IN:HL<=BUF
        public void CWVC()
        {
            Z80.B = 32;
            Z80.C = 1;
        //CWVC2:
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.A |= Z80.A;
                if (Z80.A == 0)
                {
                    goto CWVC3;// WORK ﾆ ｱｷ ｱﾘ
                }
                Z80.HL++;
                Z80.C++;
                Z80.B--;
            } while (Z80.B != 0);

            Z80.Carry = true;
            return;//    RET
        CWVC3:
            Z80.A = Z80.E;// GET VOICE NUMBER
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.A &= Z80.A;
            //RET
        }

        // **	VOLUME SET	**

        public void SETVOL()
        {
            Z80.A = Mem.LD_8(COMNOW);
            if (Z80.A - 10 == 0)
            {
                goto STV4;
            }
            Z80.HL++;
            msub.REDATA();
            if (!Z80.Carry)
            {
                goto STV1;
            }
            Z80.A = Mem.LD_8(VOLINT);
            Z80.E = Z80.A;
        STV1:
            Z80.A = Z80.E;
            Mem.LD_8(VOLUME, Z80.A);
            Mem.LD_8(VOLINT, Z80.A);

            Z80.A = Mem.LD_8(COMNOW);
            if (Z80.A - 6 == 0) {
                goto STV2;
            }
            Z80.A -= 3;
            Z80.Carry = (Z80.A - 3 < 0);
            //Z80.A = Mem.LD_8(TV_OFS);
            Z80.A = TV_OFS;
            if (Z80.Carry)
            {
                goto STV12;
            }
            Z80.A += Z80.E;
            Z80.A += 4;
            Z80.E = Z80.A;
            goto SETVOL2;
        STV12:
            Z80.A += Z80.E;
            Z80.E = Z80.A;
        SETVOL2:
            Z80.A = 0xf1;		// COM OF 'v'
            msub.MWRITE();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;

        // -	DRAM V. -

        STV2:
            //Z80.A = Mem.LD_8(TV_OFS);
            Z80.A = TV_OFS;
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A = 0xF1;
            msub.MWRITE();
            Z80.B = 6;
        //STV3:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.A = Mem.LD_8(Z80.HL);
                if (Z80.A - 0x2c != 0)//','
                {
                    msub.ERRORNE();
                    return;
                }
                msub.ERRT();
                Z80.A = Z80.E;
                msub.MWRIT2();
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;

        // -	PCMVOL	-

        STV4:
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                goto STV5;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            goto STV1;
        STV5:
            Z80.A = 0xff;
            Z80.E = 0xf0;
            msub.MWRITE();
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x6d != 0)
            {
                msub.ERRORSN();
                return;
            }
            Z80.HL++;
            msub.REDATA();
            if (Z80.Carry)
            {
                msub.ERRORSN();
                return;
            }
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                msub.ERRIF();
                return;
            }
            Z80.A = Z80.E;
            msub.MWRIT2();// SET DATA ONLY
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **   DETUNE SET   **

        public void SETDT()
        {
            msub.ERRT();
            CHCHK();
            if (!Z80.Carry)
            {
                goto STD2;
            }
            NEG16();
        STD2:
            Z80.A = 0xf2;   // COM OF 'D'
            msub.MWRITE();
            Z80.A = Z80.D;
            msub.MWRIT2();
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            if (Z80.A - 0x2b == 0)//'+'
            {
                goto STD5;
            }
            Z80.A ^= Z80.A;
            Z80.HL--;
        STD5:
            msub.MWRIT2();
            //goto FCOMP1;
            COMTBL_RetPtn = 1;
            return;
        }

        // **   ﾘｽﾞﾑ ｾｯﾄ   **

        public void SETLIZ()
        {
            COMTBL_RetPtn = -1;

            msub.ERRT();
            Z80.A = Mem.LD_8(CLOCK);
            if (Z80.A - Z80.E < 0)
            {
                msub.ERRORND();// CLOCK<E ﾃﾞ ERR
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.H = 0;
            Z80.L = Z80.A;     // HL=CLOCK : DE = LIZM
            msub.DIV();
            Z80.A = Mem.LD_8(KOTAE);// GET COUNTER
            Z80.HL = Mem.stack.Pop();
            Mem.LD_8(COUNT, Z80.A);
            Z80.C = Z80.A;
            Z80.E = 0;
        SETLI1:
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2e != 0)//'.'
            {
                goto SETLI2;
            }
            Z80.HL++;
            Z80.C >>= 1;
            Z80.A = Z80.C;
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A = Z80.C;
            goto SETLI1;
        SETLI2:
            Z80.A = Mem.LD_8(KOTAE);
            Z80.A += Z80.E;
            Mem.LD_8(COUNT, Z80.A);
            //goto FCOMP12;// EXIT SETCOM
            COMTBL_RetPtn = 12;
            return;
        }

        // **   ｵｸﾀｰﾌﾞ ｾｯﾄ  **

        public void SETOCT()
        {
            COMTBL_RetPtn = -1;

            Z80.HL++;
            msub.REDATA();
            if (!Z80.Carry)
            {
                goto STO2;
            }
            Z80.A = Mem.LD_8(OCTINT);
            Z80.E = Z80.A;
            Z80.E++;
        STO2:
            Z80.A = Z80.E;
            Z80.A--;
            if (Z80.A - 8 >= 0)
            {
                // OCTAVE > 8?
                msub.ERROROO();
                return;
            }
            Mem.LD_8(OCTAVE, Z80.A);
            Mem.LD_8(OCTINT, Z80.A);
            //goto FCOMP1;// EXIT SETCOM
            COMTBL_RetPtn = 1;
            return;
        }

        // **   ｺﾝﾊﾟｲﾗ ﾜｰｸ ｴﾘｱ ｲﾆｼｬﾗｲｽﾞ   **

        public void INIT0()
        {
            //DI
            Z80.A = Mem.LD_8(0xE6C2);
            Z80.A &= 0xf7;
            Mem.LD_8(0xe6c2, Z80.A);
            PC88.OUT(0x31, Z80.A);//ｸﾞﾗﾌｨｯｸ OFF
            PC88.OUT(0x5c, Z80.A);
            Z80.HL = 0xC000;
            Z80.BC = 255 * 2;
            MEMCLR();
            PC88.OUT(0x5F, Z80.A);
            //EI

            Z80.A ^= Z80.A;
            Mem.LD_8(COMNOW, Z80.A);
            Z80.HL = COMWK;
            Z80.BC = 127;
            MEMCLR();
            Z80.A = 11;
            //Mem.LD_8(MAXCH, Z80.A);
            MAXCH[0] = Z80.A;
            Z80.A += Z80.A; //*4 //KUMA:*2のような。。。
            //Mem.LD_8(MAXCH_MULT4, Z80.A);
            MAXCH_MULT4[0] = Z80.A;
            Z80.HL = 0xC800;
            //Mem.LD_16(ADRSTC, Z80.HL);
            ADRSTC = Z80.HL;
            INIT();
            //RET
        }

        public void INIT()
        {
            Z80.A = 1;
            //Mem.LD_8(LFODAT, Z80.A);
            LFODAT[0] = Z80.A;
            Z80.A = 5;
            Mem.LD_8(OCTAVE, Z80.A);
            Z80.A = 24;
            Mem.LD_8(COUNT, Z80.A);
            Z80.A = 128;
            Mem.LD_8(CLOCK, Z80.A);
            Z80.A ^= Z80.A;
            Mem.LD_8(VOLUME, Z80.A);
            //RET
        }

        public void TEXTINIT()
        {
            Z80.HL = TEXT + 80;   // ｱﾄﾘﾋﾞｭｰﾄ
            Mem.LD_8(Z80.HL, 22);
            Z80.HL++;
            Mem.LD_8(Z80.HL, 0b0100_1000);
            Z80.HL++;
            Mem.LD_8(Z80.HL, 80 - 22);
            Z80.HL++;
            Mem.LD_8(Z80.HL, 0b1110_1000);
            //Z80.HL = MESS;
            Z80.HL = 0;
            Z80.DE = TEXT;
            Z80.BC = 70;
            Z80.LDIR_HL(Encoding.UTF8.GetBytes(MESS));
            PRMODE();
            //    RET
        }

        // --	ﾒﾓﾘｸﾘｱ	--

        //IN:	HL<=ADR ,BC=LENGTH
        public void MEMCLR()
        {
            Mem.LD_8(Z80.HL, 0);
            Z80.E = Z80.L;
            Z80.D = Z80.H;
            Z80.DE++;
            Z80.LDIR();
            //    RET
        }

        // **   ﾜｰｸｴﾘｱ ｾﾝﾄｳﾊﾞﾝﾁ ｾｯﾃｲ**

        // 	IN: A<= ASC CODE(A-Z)

        public void WORKGET()
        {
            Z80.A--;
            Mem.LD_8(MU_NUM, Z80.A);
            TBCLR4();// ﾃｰﾌﾞﾙｸﾘｱ
            Z80.A = Mem.LD_8(MU_NUM);
            GETTBL();
            Mem.LD_16(DATTBL, Z80.HL);
            Mem.LD_16(MDATA, Z80.DE);
            //RET
        }

        // --	Aｷｮｸﾒ ﾉ ﾃｰﾌﾞﾙｾﾝﾄｳｱﾄﾞﾚｽ ﾄ ｵﾝｶﾞｸﾃﾞｰﾀｾﾝﾄｳｱﾄﾞﾚｽ ｦ ﾓﾄﾒﾙ	--

        //IN:	A<=MUSICNUM(0-N)
        // EXIT:	HL<=TABLE ADR TOP
        //	DE<=MUSIC DATA ADR TOP

        public void GETTBL()
        {
            Z80.DE = MU_TOP + 1;
        GETTB2:
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.DE);
            //Z80.HL = Mem.LD_16(MAXCH);
            Z80.HL = (ushort)(MAXCH[0] | (MAXCH[1] << 8));
            Z80.E = 4;
            msub.MULT();
            Z80.DE = Mem.stack.Pop();
            Z80.HL += Z80.DE;
            Z80.HL++;
            Z80.HL++;
            Z80.EX_DE_HL();// DE=MDATA
            Z80.AF = Mem.stack.Pop();
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                return;
            }
            Z80.A--;
            Z80.EX_DE_HL();
            Z80.HL--;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL--;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL = MU_TOP;
            Z80.HL += Z80.DE;
            Z80.EX_DE_HL();
            Z80.DE++;	// DE=ﾂｷﾞ ﾉ ｷｮｸ ﾉ MU_TOP+1ﾉ ｱﾄﾞﾚｽ
            goto GETTB2;
        }

        // --	ﾃｰﾌﾞﾙ ｦ ｸﾘｱｽﾙ	--

        public void TBCLR4()
        {
            //Z80.HL = Mem.LD_16(MAXCH);
            Z80.HL = (ushort)(MAXCH[0] | (MAXCH[1] << 8));
            Z80.E = 4;
            msub.MULT();
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            Z80.A = Mem.LD_8(MU_NUM);
            GETTBL();
            Z80.D = Z80.H;
            Z80.E = Z80.L;
            Z80.DE++;
            Mem.LD_8(Z80.HL, 0);
            Z80.BC = Mem.stack.Pop();
            Z80.LDIR();
            //    RET
        }

        // **	MUSIC START	**

        // IN: (MU_NUM)<= MUSIC NUMBER(0-3)

        public void M_ST()
        {
            //	DI
            Z80.HL = Mem.stack.Pop();// TEXT POINTER
            msub.REDATA();
            if (Z80.Carry)
            {
                goto MSTART2;
            }
            Mem.stack.Push(Z80.HL);
            Z80.A = Z80.E;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto MAKEWORK;
            }
            Z80.HL = Mem.stack.Pop();
        MSTART2:
            Mem.stack.Push(Z80.HL);
            Z80.A = 1;
        MAKEWORK:
            Z80.A--;
            PC88.CALL(MSTART);
            REDOF();
            msub.ROM();
            msub.T_RST();// RESET TIME
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void REDOF()
        {
            Z80.A ^= Z80.A;
            Mem.LD_8(ESCAPE, Z80.A);	// ESCｷｰ ｶｲｼﾞｮ
            PC88.CALL(INFADR);
            Mem.LD_8((ushort)(Z80.IX + 2), 1);
            //	RET
        }

        // **	ｵﾝｼｮｸ ﾂｸﾙ	**

        public void VICMKE()
        {
            //	DI
            msub.RAM();

            //Z80.HL = NEXTPT;
            //Z80.BC = 6;
            //MEMCLR();
            NEXTPT = 0;
            NEXTVL = 0;
            LINENM = 0;

            Z80.HL = DEFVOICE;
            //Mem.LD_16(VICAD2, Z80.HL);
            VICAD2 = Z80.HL;
            Z80.HL = 1;
        VICMK1:
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);       // DE = LINK POINTER
            Z80.HL++;
            Z80.A = Z80.D;
            Z80.A |= Z80.E;// BASICEND?
            if (Z80.A == 0)
            {
                goto VICMK2;
            }

            Z80.C = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.B = Mem.LD_8(Z80.HL);       // BC = LINE NUMBER
            //Mem.LD_16(LINENM, Z80.BC);
            LINENM = Z80.BC;
            Z80.EX_DE_HL();
            goto VICMK1;

        VICMK2:
            SCHVIC();// ｾｯﾃｲｽﾙ ｵﾄﾉｶｽﾞｦ ｷﾒﾙ
            //Z80.A = Mem.LD_8(VICNUM);
            Z80.A = VICNUM;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto VICEXT;
            }
            Z80.B = Z80.A;
        //VICMK4:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.B = 8;
                VICPRT();
                //Mem.LD_16(NEXTVL, Z80.HL);
                NEXTVL = Z80.HL;
            //VICMK3:
                do
                {
                    Mem.stack.Push(Z80.BC);
                    //Mem.LD_16(NEXTPT, Z80.HL);
                    NEXTPT = Z80.HL;
                    //Z80.DE = Mem.LD_16(LINENM);
                    Z80.DE = LINENM;
                    Z80.A = 10;
                    Z80.Carry = (Z80.A + Z80.E > 0xff);
                    Z80.A += Z80.E;
                    Z80.E = Z80.A;
                    Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
                    Z80.A -= Z80.E;
                    Z80.D = Z80.A;

                    //Mem.LD_16(LINENM, Z80.DE);
                    LINENM = Z80.DE;
                    Mem.LD_8(Z80.HL, Z80.E);        //
                    Z80.HL++;// DE = LINE NUMBER
                    Mem.LD_8(Z80.HL, Z80.D);        //
                    Z80.HL++;
                    Z80.EX_DE_HL();
                    //Z80.HL = LNKTXT;
                    Z80.HL = 0;
                    Z80.BC = 5;
                    Z80.LDIR_HL(LNKTXT);
                    Z80.EX_DE_HL();
                    Z80.BC = 19;
                    Mem.stack.Push(Z80.DE);
                    Z80.E = Z80.L;
                    Z80.D = Z80.H;
                    Z80.DE++;
                    Mem.LD_8(Z80.HL, 0x20);
                    Z80.LDIR();// SKIP&CLEAR
                    Z80.DE = Mem.stack.Pop();
                    Mem.LD_8(Z80.HL, 0);// LINE END MARK
                    Z80.HL++;
                    Z80.EX_DE_HL();
                    //Z80.HL = Mem.LD_16(NEXTPT);
                    Z80.HL = NEXTPT;
                    Z80.HL--;
                    Mem.LD_8(Z80.HL, Z80.D);//
                    Z80.HL--;// SAVE NEW LINK PT
                    Mem.LD_8(Z80.HL, Z80.E);        //
                    Z80.EX_DE_HL();
                    Z80.HL++;
                    Z80.HL++;
                    Z80.BC = Mem.stack.Pop();
                    Z80.B--;
                } while (Z80.B != 0);
                Z80.EX_DE_HL();
                Mem.stack.Push(Z80.HL);
                SETVIC();
                Z80.HL = Mem.stack.Pop();
                Z80.HL++;
                Z80.HL++;
                Z80.BC = Mem.stack.Pop();
            } while (Z80.B != 0);

            Z80.HL--;
            Mem.LD_8(Z80.HL, 0);        //
            Z80.HL--;// BASIC END MARK
            Mem.LD_8(Z80.HL, 0);        //
            Z80.HL++;
            Z80.HL++;
            Z80.HL++;
            Mem.LD_16(0xeb18, Z80.HL);
        VICEXT:
            msub.ROM();
            Z80.HL = Mem.stack.Pop();
            return;//    RET
        }

        public void SCHVIC()
        {
            Mem.stack.Push(Z80.HL);
            Z80.HL = DEFVOICE;
            Z80.BC = 32 * 256;
        //SCHVI2:
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.A |= Z80.A;
                if (Z80.A == 0)
                {
                    goto SCHVI3;
                }
                Z80.C++;
                Z80.A = Z80.C;
                //Mem.LD_8(VICNUM, Z80.A);
                VICNUM = Z80.A;
                Z80.HL++;
                Z80.B--;
            } while (Z80.B != 0);
        SCHVI3:
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        // **	ﾃｷｽﾄ ﾆ ｵﾝｼｮｸｾｯﾄ**

        public void SETVIC()
        {
            //Z80.HL = Mem.LD_16(NEXTVL);
            Z80.HL = NEXTVL;
            Z80.HL--;
            Z80.HL--;// HL=LINK PT TOP ADR
            Mem.stack.Push(Z80.BC);
            Z80.B = 6;// ﾀﾃ 7 ｶｳﾝﾄ
        //SETVI0:
            do
            {
                Mem.stack.Push(Z80.BC);
                Mem.stack.Push(Z80.HL);
                Z80.DE = 9;
                Z80.HL += Z80.DE;// SKIP
                Z80.EX_DE_HL();
                SETVI1();
                Z80.HL = Mem.stack.Pop();
                Z80.BC = 29;
                Z80.HL += Z80.BC;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);

            Z80.DE = 9;
            Z80.HL += Z80.DE;
            Z80.EX_DE_HL();
            Z80.A = 1;
            //Mem.LD_8(SETVI1 + 1, Z80.A);
            SETVI1_VAL = Z80.A;
            SETVI1();
            Z80.A = 4;
            //Mem.LD_8(SETVI1 + 1, Z80.A);
            SETVI1_VAL = Z80.A;
            Z80.BC = Mem.stack.Pop();
            //  RET
        }

        public void SETVI1()
        {
            Z80.B = SETVI1_VAL;// 4;// ﾖｺ 4
            //Z80.HL = Mem.LD_16(VICADR);
            Z80.HL = VICADR;
            //SETVI2:
            do
            {
                Mem.stack.Push(Z80.BC);
                Mem.stack.Push(Z80.HL);
                Mem.stack.Push(Z80.DE);
                Z80.L = Mem.LD_8(Z80.HL);
                Z80.H = 0;
                msub.HEXPRT();
                Z80.DE = Mem.stack.Pop();
                Z80.A = 0x24;// '$'
                Mem.LD_8(Z80.DE, Z80.A);
                Z80.DE++;
                Z80.HL = MOJIBUF + 1;
                Z80.BC = 3;
                Z80.LDIR();
                Z80.HL = Mem.stack.Pop();
                Z80.HL++;
                Z80.BC = Mem.stack.Pop();
                Z80.A = 1;
                if (Z80.A - Z80.B == 0)
                {
                    goto SETVI3;
                }
                Z80.A = 0x2c;// ','
                Mem.LD_8(Z80.DE, Z80.A);
                Z80.DE++;
            SETVI3:
                Z80.B--;
            } while (Z80.B != 0);

            //Mem.LD_16(VICADR, Z80.HL);
            VICADR = Z80.HL;
            //RET
        }

        public void VICPRT()
        {
            Mem.stack.Push(Z80.BC);
            Mem.stack.Push(Z80.HL);
            //Z80.DE = Mem.LD_16(LINENM);
            Z80.DE = LINENM;
            Z80.A = 10;
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;
            //Mem.LD_16(LINENM, Z80.DE);
            LINENM = Z80.DE;
            Mem.LD_8(Z80.HL, Z80.E);        //
            Z80.HL++;// DE = LINE NUMBER
            Mem.LD_8(Z80.HL, Z80.D);        //
            Z80.HL++;
            Z80.EX_DE_HL();
            //Z80.HL = LNKTXT;
            Z80.HL = 0;
            Z80.BC = 5;
            Z80.LDIR_HL(LNKTXT);
            Z80.A = 0x40;// '@'
            Mem.LD_8(Z80.DE, Z80.A);
            Z80.DE++;
            Z80.A = 0x25;// '%'
            Mem.LD_8(Z80.DE, Z80.A);
            Z80.DE++;
            //Z80.HL = Mem.LD_16(VICAD2);
            Z80.HL = VICAD2;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            //Mem.LD_16(VICAD2, Z80.HL);
            VICAD2 = Z80.HL;
            Z80.A--;
            Mem.stack.Push(Z80.HL);
            Z80.L = Z80.A;
            Z80.H = 0;
            Mem.stack.Push(Z80.DE);
            msub.HEXDEC();
            Z80.HL++;
            Z80.HL++;
            Z80.DE = Mem.stack.Pop();
            Z80.BC = 3;
            Z80.LDIR();
            Z80.A = 0x3b;// ';'
            Mem.LD_8(Z80.DE, Z80.A);
            Z80.DE++;
            Z80.AF = Mem.stack.Pop();
            GETADR();
            Z80.BC = 32 - 6;
            Z80.HL += Z80.BC;
            Z80.BC = 6;
            Z80.LDIR();
            Z80.A ^= Z80.A;
            Mem.LD_8(Z80.DE, Z80.A);        // SET LINE END MARK
            Z80.DE++;
            Z80.HL = Mem.stack.Pop();
            Z80.HL--;
            Mem.LD_8(Z80.HL, Z80.D);
            Z80.HL--;
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.DE = 24;
            Z80.HL += Z80.DE;
            Z80.BC = Mem.stack.Pop();
            //    RET
        }

        public ushort NEXTPT = 0;
        public ushort NEXTVL = 0;
        public ushort LINENM = 0;
        public byte VICNUM = 0;
        public ushort VICADR = 0xE300;
        public ushort VICAD2 = DEFVOICE;
        public byte[] LNKTXT = new byte[] { 0x3A, 0x8F, 0xE9, 0x20, 0x20 };	// 5 ｺ

        // **	SYSTEM WORK AREA**

        public byte MACFG = 0;//0>< AS MACRO PRC
        public string MESS = "[  MUCOM88 Ver:1.7  ]  Address:    -    (    )         [ 00:00 ] MODE:";
        public string MESNML = "NORMAL  LINC    EXPERT  ";
        public string MESNOT = "----";
        public string VNMESS = "\x0AUsed FM voice:\0";
        public string TCMESS = "\x0A[ Total count ]\x0A\0";
        public string LCMESS = "\x0A[ Loop count  ]\x0A\0";
        public string ADMES = "\x0AVoice added: +\0";
        public string JPLMES = "Jumping to linenumber   >";
        public byte DEBFLG = 0xFF;
        public byte LINCFG = 0;
        public byte[] MAXCH = new byte[] { 11, 0 };
        public byte[] MAXCH_MULT4 = new byte[] { 11 * 4, 0 };        // MAXCH*4
        public string IFGM = ">) as UP,<( as DOWN\0";
        public string IFGM2 = ">) as DOWN,<( as UP\0";
        public byte[] LFODAT = new byte[] { 1, 0, 0, 0, 0, 0, 0 };
        //+0 LFO SW(0:ON 1:OFF)
        //+1 DELAY
        //+2 Clock Unit(SPEED?)
        //+3 DEPTH L
        //+4 DEPTH H
        //+5 変化の回数
        //+6 ?

        public ushort ADRSTC = 0;
        public byte VPCO = 0;
        public byte UDFLG = 0;
        public byte PSGMD = 0;

        //
    }
}
