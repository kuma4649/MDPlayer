//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MDPlayer.Driver.MUCOM88.ver1_1
//{
//    public class expand
//    {
//        //N88BASIC EXPAND COMMAND PROGRAM

//        //    ORG	0AB00H

//        public const int COMWK      = 0x0F320;
//        public const int MDATA      = COMWK;//ｺﾝﾊﾟｲﾙｻﾚﾀ ﾃﾞｰﾀｶﾞｵｶﾚﾙ ｹﾞﾝｻﾞｲﾉ ｱﾄﾞﾚｽ
//        public const int DATTBL		= MDATA+4;	// ｹﾞﾝｻﾞｲ ｺﾝﾊﾟｲﾙﾁｭｳ ﾉ MUSIC DATA TABLE TOP
//        public const int OCTAVE		= DATTBL+2;
//        public const int SIFTDAT  	= OCTAVE+1;
//        public const int CLOCK		= SIFTDAT+1;
//        public const int SECCOM		= CLOCK+1;
//        public const int KOTAE		= SECCOM+1;
//        public const int LINE	    = KOTAE+2;
//        public const int ERRORLINE	= LINE+2;
//        public const int COMNOW	    = ERRORLINE+2;
//        public const int COUNT	    = COMNOW+1;
//        public const int MOJIBUF    = COUNT+1;
//        public const int SEC        = MOJIBUF+4;
//        public const int MIN        = SEC+1;
//        public const int HOUR       = MIN+1;
//        public const int ALLSEC	    = HOUR+1;
//        public const int T_FLAG	    = ALLSEC+2;
//        public const int SE_SET	    = T_FLAG+1;
//        public const int FD_FLG	    = SE_SET+2;
//        public const int FD_EFG	    = FD_FLG+1;
//        public const int ESCAPE	    = FD_EFG+1;
//        public const int MINUSF	    = ESCAPE+1;
//        public const int BEFRST	    = MINUSF+1;// ｾﾞﾝｶｲ ｶﾞ 'r' ﾃﾞｱﾙｺﾄｦｼﾒｽ ﾌﾗｸﾞ ｹﾝ ｶｳﾝﾀ
//        public const int BEFCO	    = BEFRST+1;// ｾﾞﾝｶｲ ﾉ ｶｳﾝﾀ
//        public const int BEFTONE    = BEFCO+2;// ｾﾞﾝｶｲ ﾉ ﾄｰﾝ ﾃﾞｰﾀ
//        public const int TIEFG	    = BEFTONE+9;// ｾﾞﾝｶｲ ｶﾞ ﾀｲﾃﾞｱﾙｺﾄｦ ｼﾒｽ
//        public const int COMNO	    = TIEFG+1;// ｾﾞﾝｶｲﾉ ｺﾏﾝﾄﾞ｡ ﾄｰﾝﾉﾄｷﾊ 0
//        public const int ASEMFG	    = COMNO+1;
//        public const int VDDAT	    = ASEMFG+1;
//        public const int OTONUM	    = VDDAT+1;// ﾂｶﾜﾚﾃｲﾙ ｵﾝｼｮｸ ﾉ ｶｽﾞ
//        public const int VOLUME	    = OTONUM+1;// NOW VOLUME


//        public const int MWRITE = 0x9000;
//        public const int MWRIT2 = MWRITE + 3;
//        public const int ERRT = MWRIT2 + 3;
//        public const int ERRORSN = ERRT + 3;
//        public const int ERRORIF = ERRORSN + 3;
//        public const int ERRORNF = ERRORIF + 3;
//        public const int ERRORFN = ERRORNF + 3;
//        public const int ERRORVF = ERRORFN + 3;
//        public const int ERROROO = ERRORVF + 3;
//        public const int ERRORND = ERROROO + 3;
//        public const int ERRORRJ = ERRORND + 3;
//        public const int STTONE = ERRORRJ + 3;
//        public const int STLIZM = STTONE + 3;
//        public const int REDATA = STLIZM + 3;
//        public const int MULT = REDATA + 3;
//        public const int DIV = MULT + 3;
//        public const int HEXDEC = DIV + 3;
//        public const int HEXPRT = HEXDEC + 3;
//        public const int ROM = HEXPRT + 3;
//        public const int RAM = ROM + 3;
//        public const int FMCOMC = RAM + 3;
//        public const int T_RST = FMCOMC + 3;
//        public const int ERRORNE = T_RST + 3;
//        public const int ERRORDC = ERRORNE + 3;
//        public const int ERRORML = ERRORDC + 3;
//        public const int MCMP = ERRORML + 3;
//        public const int ERRORVO = MCMP + 3;


//        public const int MUSIC = 0x0B000;
//        public const int DRIVE = MUSIC + 3 * 5;
//        public const int WKGET = MUSIC + 3 * 8;


//        public const int MUC88 = 0x09600;
//        public const int MUSICSTART = MUC88 + 3 * 3;


//        public const int CURSOR = 0x0EF86;

//        public const int CLS1 = 0x5F0E;
//        public const int SCEDIT = 0x5F92;	//ｽｸﾘｰﾝ ｴﾃﾞｨｯﾄ
//        public const int STOPKC	=0x35C2;	//ｽﾄｯﾌﾟｷｰ ﾁｪｯｸ
//        public const int BUFCLR = 0x35D9;	//ｷｰﾊﾞｯﾌｧｸﾘｱ
//        public const int EDBUF	=0x08C10;	// T_CLKﾅﾄﾞﾄ ｷｮｳﾂｳ ﾉ ﾜｰｸ
//        public const int TXTEND	=0x0EB18;	//ﾃｷｽﾄ ｴﾝﾄﾞ
//        public const int LNKSET	=0x05BD;	//ﾘﾝｸﾎﾟｲﾝﾀ ｾｯﾄ
//        public const int CHGWA = 0x044D5;	// ｳｨﾝﾄﾞｳ->ｼﾞﾂｱﾄﾞﾚｽ ﾍﾝｶﾝ

//        // -- CLEAR FROM COMPI1	-->

//        public const int T_CLK = 0x08C10;
//        public const int UDFLG = T_CLK + 4 * 11;
//        public const int BEFMD = UDFLG + 1;
//        public const int PTMFG = BEFMD + 2;
//        public const int PTMDLY = PTMFG + 1;
//        public const int SPACE = PTMDLY + 2;//2*8BYTE ｱｷ ｶﾞ ｱﾙ
//        public const int DEFVOICE = SPACE + 2 * 8;
//        public const int DEFVSSG = DEFVOICE + 32;
//        public const int JCLOCK = DEFVSSG + 32;
//        public const int JPLINE = JCLOCK + 2;

//        //-<

//        public const int SMON = 0x0DE00;
//        public const int CONVERT = SMON + 3 * 2;


//        public Action[] tblOpe = new Action[] {
//          DSPMSG
//        , FOUND
//        , PRNFAC
//        , FVTEXT
//        , COLOR
//        , KEYCHK
//        , REPLACE
//        , CULPTM
//        };

//        // **	ﾃｷｽﾄ ｶﾗ ﾄｸﾃｲﾉ ﾓｼﾞﾚﾂｦ ｻｶﾞｽ**

//        public static void FOUND()
//        {
//            Z80.HL = PROMPT;

//            DSPMSG();
//            SCEDIT();

//            if (Z80.Carry) return;

//            Z80.HL++;
//            Z80.DE = EDBUF;
//            Z80.BC = 32;
//            //EDBUF ﾆ ﾃﾝｿｳ
//            do
//            {
//                Mem.LD_8(Z80.DE, Mem.LD_8(Z80.HL));
//                Z80.DE++;
//                Z80.HL++;
//                Z80.BC--;
//            } while (Z80.BC != 0);

//            UPCLS();
//            PRSWD();
//            CLS1();
//            Z80.A = 0;
//            Mem.LD_8(REPFLG, Z80.A);

//            //FO0:

//            Z80.A = 0;
//            Mem.LD_8(RETK, Z80.A);

//            //  DI

//            RAM();

//            Z80.HL = 1;

//            FO1:

//            Z80.E = Mem.LD_8(Z80.HL);
//            Z80.HL++;
//            Z80.D = Mem.LD_8(Z80.HL);
//            Z80.HL++;

//            Mem.LD_16(LINKPT, Z80.DE);
//            Mem.LD_16(LINEADR, Z80.HL);
//            Z80.A = Z80.E;
//            Z80.A |= Z80.D;//BASIC END?

//            if (Z80.A == 0)
//            {
//                NOTFOUND();
//                return;
//            }

//            Z80.E = Mem.LD_8(Z80.HL);
//            Z80.HL++;
//            Z80.D = Mem.LD_8(Z80.HL);
//            Z80.HL++;

//            Mem.LD_16(LINE2, Z80.DE);
//            Mem.LD_16(BEGIN, Z80.HL);
//            Z80.HL++;
//            Z80.HL++;
//            Z80.HL++;
//            ushort dmy = Z80.DE;
//            Z80.DE = Z80.HL;
//            Z80.HL = dmy;

//            FO12:

//            Z80.HL = EDBUF;
//            Z80.B = 33;

//            //FO2:
//            do {
//                ushort stHL = Z80.HL;

//                Z80.HL = Mem.LD_16(LINKPT);
//                Z80.HL--;

//                //Z80.A &= Z80.A;//carryを0にする
//                Z80.HL -= Z80.DE;//上の行でcarryは0になっているので減算に影響なし
//                Z80.Zero = (Z80.HL == 0);
//                Z80.HL = stHL;

//                if (Z80.Zero) goto FO6;

//                Z80.A = Mem.LD_8(Z80.DE);
//                Z80.DE++;

//                if (Z80.A - Mem.LD_8(Z80.HL) != 0) goto F012;

//                Z80.HL++;

//                Z80.A = Mem.LD_8(Z80.HL);

//                if (Z80.A == 0)//BUFFER END?
//                {
//                    goto F0_OK;
//                }

//                Z80.B--;
//            } while (Z80.B != 0);

//            goto FO_OK;

//        FO6:
//            Z80.HL = Mem.LD_16(LINKPT);

//            goto FO1;// ﾂｷﾞ ﾉ ｷﾞｮｳﾍ



//        // --	ﾓｼﾞﾚﾂ ﾊｯｹﾝ	--

//        FO_OK:
//            ;
//            //	EI

//            ROM();
//            ushort stDE = Z80.DE;
//            FOCUR();
//            ushort stHL = Z80.HL;

//            Z80.DE = Mem.LD_16(LINE2);
//            PRNFAC();

//            FOK2:
//            Z80.A = 0x20;
//            //   NOP

//            Z80.HL = FOK2 + 1;
//            RST(0x18);

//            Z80.HL = Mem.LD_16(LINEADR);
//            Z80.HL++;
//            Z80.HL++;
//            CALL(0x44A4);
//            CALL(0x194C);
//            Z80.HL = Mem.LD_16(0x0E9B9);
//            DSPMSG();
//            Z80.HL = LF;
//            DSPMSG();

//            Z80.HL = Mem.LD_16(CURSOR);
//            Mem.LD_16(CUR2, Z80.HL);
//            Z80.HL = stHL;
//            Mem.LD_16(CURSOR, Z80.HL);

//            FOK3:

//            CALL(0x4290);
//            Z80.A = PC88.IN(9);//STOPKEY
//            if ((Z80.A & 1) == 0)
//            {
//                goto NOTF2;
//            }

//            Z80.HL = RETK;
//            Z80.BC = 0x7F01;
//            KEYCHK();//ﾘﾀｰﾝｷｰ ﾁｪｯｸ

//            if (Z80.Zero)
//            {
//                goto FOK32;
//            }

//            if ((Z80.A & 0x80) == 0)
//            {
//                goto FOK4;
//            }

//        FOK32:
//            Z80.HL = SPACEK;
//            Z80.BC = 0x0BF09;
//            KEYCHK();
//            if (Z80.Zero)
//            {
//                goto FOK3;
//            }

//            if ((Z80.A & 0x40) != 0)
//            {
//                goto FOK3;
//            }

//            Z80.DE = stDE;
//            goto FOK5;// ｽﾍﾟｰｽ ｶﾞ ｵｻﾚﾀﾅﾗ ﾘﾌﾟﾚｰｽ ｽｷｯﾌﾟ

//        FOK4:
//            Z80.DE = stDE;

//            Z80.A = Mem.LD_8(REPFLG);
//            Z80.A |= Z80.A;

//            if (Z80.A != 0)
//            {
//                REP2();// ﾘﾌﾟﾚｰｽ ﾅﾗ ﾘﾀｰﾝｷｰﾃﾞ ﾘﾌﾟﾚｰｽﾌﾟﾛｾｽ
//            }
//        FOK5:
//            stDE = Z80.DE;

//            CLS1();

//            Z80.DE = stDE;

//            //    DI

//            RAM();
//            goto FO12;

//        NOTFOUND:
//            ;
//            //    EI

//            ROM();
//            BUFCLR();
//            return;

//        NOTF2:
//            Z80.DE = stDE;// DUMMY
//            //   LD  DE,(CUR2)
//            //   LD(CURSOR),DE
//            // JR  NOTFOUND

//            //; --	ﾐﾂｹﾀﾄｺﾛﾉ ｶｰｿﾙｻﾞﾋｮｳ ｦ ｴﾙ	--

//            //	; IN:DE<=ﾃｷｽﾄ ｱﾄﾞﾚｽ
//            //     ; EXIT:	HL

//            //  FOCUR:

//            //    PUSH DE


//            //    LD DE,(LINE2)

//            //    CALL STRFAC

//            //    LD BC,0FFFFH
//            //   INC HL
//            //FOC2:

//            //    INC HL

//            //    LD A,(HL)

//            //    OR A

//            //    JR Z, FOC3

//            //    INC BC

//            //    JR FOC2
//            //FOC3:
//            //	LD HL, EDBUF+1
//            //FOC4:
//            //	LD A,(HL)

//            //    OR A

//            //    JR Z, FOC5

//            //    DEC BC

//            //    INC HL

//            //    JR FOC4
//            //FOC5:
//            //	POP DE


//            //    EX DE, HL

//            //    ADD HL, BC

//            //    EX DE, HL


//            //    LD HL,(BEGIN)

//            //    EX DE, HL

//            //    AND A

//            //    SBC HL, DE

//            //    LD D,0

//            //    LD E,80

//            //    CALL DIV

//            //    LD D, E; ｱﾏﾘ ﾊ Xｻﾞﾋｮｳ
//            //   LD  E,L	; ｼｮｳ ﾊ Y ｻﾞﾋｮｳ


//            //    LD HL,(CURSOR)

//            //    ADD HL, DE

//            //    RET
//        }


//        //; **	REPLACE TEXT	**



//        //REPLACE:
//        //	LD HL, REPWD

//        //    CALL DSPMSG

//        //    LD HL, PROMPT

//        //    CALL DSPMSG

//        //    CALL SCEDIT

//        //    RET C

//        //    INC HL

//        //    LD DE, EDBUF

//        //    LD BC,32

//        //    LDIR			;EDBUF ﾆ ﾃﾝｿｳ

//        //    CALL    UPCLS
//        //    CALL    PRSWD

//        //    LD  HL,PROM2
//        //    CALL    DSPMSG
//        //    CALL    SCEDIT
//        //    RET C
//        //    INC HL
//        //    LD  DE,EDBUF+32
//        //	LD BC,32

//        //    LDIR

//        //    CALL    PRRWD

//        //    CALL    CLS1
//        //    LD  A,0FFH
//        //    LD(REPFLG),A
//        //  JP  FO0

//        //; **	REPLACE PROCESS MAIN**

//        //	;ON:DE<= TEXT ADR


//        //REP2:
//        //	PUSH DE

//        //; --	ﾍﾝｶﾝｽﾙﾓﾉﾄ ﾍﾝｶﾝｻﾚﾙﾓﾉﾉ ﾓｼﾞｽｳﾉ ｻ	--


//        //    LD HL, EDBUF+1

//        //    LD C,1
//        //REP21:
//        //	DEC DE

//        //    LD A,(HL)

//        //    OR A

//        //    JR Z, REP22

//        //    INC HL

//        //    INC C

//        //    JR REP21
//        //REP22:
//        //	LD(HENTOP),DE
//        //  LD  HL,EDBUF+32
//        //	LD B,0
//        //REP23:
//        //	LD A,(HL)

//        //    OR A

//        //    JR Z, REP24

//        //    INC B

//        //    INC HL

//        //    JR REP23
//        //REP24:
//        //	LD A, B

//        //    OR A

//        //    JR NZ, REP25

//        //    LD(EDBUF+33),A
//        //  LD  A,20H
//        //  LD(EDBUF+32),A
//        //LD  A,1
//        //REP25:
//        //	LD(HENCO),A
//        //  SUB C
//        //  JR  NC,REPB1	;ﾍﾝｶﾝｻﾚﾙｶﾞﾜｶﾞ ｵｵｷｲﾄｷﾊ REPB1ﾍ
//        //REPS1:

//        //    POP DE

//        //    PUSH DE

//        //    EX DE, HL

//        //    LD E, A

//        //    LD D,0FFH
//        //   ADD HL,DE
//        //   PUSH    HL
//        //   LD  HL,(LINKPT)
//        //   ADD HL,DE
//        //   LD(LINKPT),HL
//        // POP HL
//        // EX  DE,HL
//        // POP HL
//        // PUSH    HL
//        // EXX


//        //    LD HL,(TXTEND)

//        //    POP DE

//        //    AND A

//        //    SBC HL, DE

//        //    LD C, L

//        //    LD B, H

//        //    PUSH BC

//        //    EXX
//        //    POP BC

//        //    PUSH    DE
//        //    DI

//        //    CALL RAM

//        //    LDIR

//        //    LD  DE,(HENTOP)
//        //    LD  HL,EDBUF+32
//        //	LD A,(HENCO)

//        //    LD C, A

//        //    LD B,0

//        //    LDIR
//        //REPS2:

//        //    CALL ROM

//        //    EI
//        //    CALL    LNKSET
//        //    INC HL
//        //    CALL    CHGWA
//        //    LD(TXTEND),HL
//        //  POP DE
//        //  RET


//        //REPB1:
//        //	OR A

//        //    JR Z, REPB3


//        //    LD HL,(LINKPT)

//        //    LD E, A

//        //    LD D,0

//        //    ADD HL, DE

//        //    LD(LINKPT),HL
//        //  LD  HL,(TXTEND)
//        //  POP DE
//        //  PUSH    DE
//        //  PUSH    HL
//        //  AND A
//        //  SBC HL,DE
//        //  INC HL
//        //  LD  C,L
//        //  LD  B,H
//        //  POP HL
//        //  PUSH    HL
//        //  LD  E,A
//        //  LD  D,0
//        //	ADD HL, DE

//        //    EX DE, HL

//        //    POP HL


//        //    DI
//        //    CALL    RAM
//        //    LDDR
//        //REPB3:
//        //	DI
//        //    CALL    RAM
//        //    POP DE
//        //    LD  DE,(HENTOP)
//        //    LD  HL,EDBUF+32
//        //	LD A,(HENCO)

//        //    LD C, A

//        //    LD B,0

//        //    LDIR
//        //    PUSH    DE
//        //    JR  REPS2


//        //; --	PRINT"SEARCH...."	--

//        //PRSWD:
//        //	LD HL, PROMPT

//        //    LD DE,0F3C8H
//        //   LD  BC,6
//        //	LDIR
//        //    LD  HL,EDBUF
//        //    CALL    PRS2
//        //    RET
//        //PRRWD:
//        //	LD HL, PROM2

//        //    LD DE,0F3C8H+23
//        //	LD BC,15

//        //    LDIR
//        //    LD  HL,EDBUF+32
//        //	CALL PRS2

//        //    RET
//        //PRS2:

//        //    LD A,(HL)

//        //    OR A

//        //    RET Z

//        //    LD(DE),A
//        //  INC HL
//        //  INC DE
//        //  JR  PRS2

//        //; **	PRINT TO DISPLAY**

//        //DSPMSG:

//        //    LD A,(HL)

//        //    AND A

//        //    RET Z

//        //    RST	18H
//        //    INC HL
//        //    JR  DSPMSG

//        //; **	PRINT FACC DATA**

//        //	;IN:DE

//        //PRNFAC:

//        //    CALL STRFAC

//        //    INC HL

//        //    CALL DSPMSG

//        //    RET
//        //STRFAC:

//        //    LD(0EC41H),DE
//        //  LD  A,2
//        //	LD(0EABDH),A
//        //  CALL	28D0H
//        //  RET

//        //; **	FIND VOICE FROM TEXT	**

//        //	; IN:A<= VOICE NUMBER
//        //     ; STORE:6001Hｶﾗ 25BYTE
//        //	;NOTFOUND:SCF

//        //FVTEXT:

//        //    EXX
//        //    LD(FV3+1),A
//        //  XOR A
//        //  LD(FVFG),A
//        //LD  HL,6001H
//        //EXX

//        //    LD HL,1
//        //FV1:
//        //	LD E,(HL)

//        //    INC HL

//        //    LD D,(HL)

//        //    INC HL

//        //    LD(LINKPT),DE
//        //  LD(LINEADR),HL
//        //LD  A,E
//        //OR  D	;BASIC END?

//        //    JP Z, FVF2

//        //    LD E,(HL)

//        //    INC HL

//        //    LD D,(HL)

//        //    INC HL

//        //    LD(LINE2),DE
//        //  INC HL
//        //  INC HL
//        //  INC HL
//        //FV2:

//        //    LD A,(HL)

//        //    INC HL

//        //    CP	20H
//        //    JR  NZ,FV4
//        //    INC HL
//        //    LD  A,(HL)
//        //    CP	'@'

//        //    JR NZ, FV4

//        //    INC HL

//        //    LD A,(HL)

//        //    CP	'%'
//        //	JR NZ, FV22

//        //    INC HL

//        //    LD(FVFG),A
//        //FV22:

//        //    CALL REDATA
//        //FV3:
//        //	LD A,0

//        //    CP E

//        //    JR Z, FV5
//        //FV4:
//        //	LD HL,(LINKPT)

//        //    JR FV1

//        //; ---	%(25BYTEｼｷ) ﾉ ﾄｷ ﾉ ﾖﾐｺﾐ	---


//        //FV5:
//        //	LD A,(FVFG)

//        //    OR A

//        //    JR Z, FV52

//        //    LD A,6	; ﾀﾃ
//        //    LD(FV6+1),A
//        //  LD  A,4	;ﾖｺ
//        //  LD(FV63+1),A
//        //CALL    FV6
//        //LD  HL,(LINKPT)
//        //LD  DE,9
//        //	ADD HL, DE

//        //    CALL REDATA

//        //    LD A, E

//        //    EXX
//        //    LD(HL),A
//        //  EXX

//        //    AND A

//        //    RET

//        //; --	38ﾊﾞｲﾄﾍﾞｰｼｯｸﾎｳｼｷﾉﾄｷﾉ ﾖﾐｺﾐ	--
//        //FV52:
//        //	LD HL,(LINKPT)

//        //    LD E,(HL)

//        //    INC HL

//        //    LD D,(HL)

//        //    LD(LINKPT),DE
//        //  LD  DE,8
//        //	ADD HL, DE

//        //    CALL REDATA

//        //    PUSH DE

//        //    INC HL

//        //    CALL REDATA

//        //    LD A, E

//        //    POP DE

//        //    LD D, A

//        //    PUSH DE


//        //    LD A,4	; ﾀﾃ
//        //    LD(FV6+1),A
//        //  LD  A,9	;ﾖｺ
//        //  LD(FV63+1),A
//        //CALL    FV6

//        //EXX

//        //    POP DE

//        //    LD(HL),E	;FB
//        //  INC HL
//        //  LD(HL),D	;ALGO
//        //EXX


//        //    LD HL,6001H
//        //   CALL    CONVERT	;38BYTE->25BYTE
//        //   AND A
//        //   RET

//        //; --	ﾖﾐｺﾐ ｻﾌﾞﾙｰﾁﾝ	--


//        //FV6:
//        //	LD B,6
//        //FV62:
//        //	PUSH BC

//        //    LD HL,(LINKPT)

//        //    LD E,(HL)

//        //    INC HL

//        //    LD D,(HL)

//        //    LD(LINKPT),DE
//        //  LD  DE,8
//        //	ADD HL, DE
//        //FV63:
//        //	LD B,4
//        //FV7:
//        //	PUSH BC

//        //    CALL REDATA

//        //    INC HL; SKIP','
//        //	LD A, E

//        //    EXX
//        //    LD(HL),A
//        //  INC HL
//        //  EXX

//        //    POP BC

//        //    DJNZ FV7
//        //FV8:
//        //	POP BC

//        //    DJNZ FV62

//        //    RET
//        //FVF2:

//        //    SCF	;ERROR
//        //    RET
//        //FVFG:
//        //	DB	0

//        //; **	COLOR SET	**

//        //	; IN:A<=COLOR CODE
//        //COLOR:
//        //	PUSH HL

//        //    PUSH DE

//        //    PUSH BC

//        //    LD C, A

//        //    ADD A, A

//        //    ADD A, C

//        //    LD HL, CCODE

//        //    LD E, A

//        //    LD D,0

//        //    ADD HL, DE

//        //    CALL	6EC6H
//        //    POP BC
//        //    POP DE
//        //    POP HL
//        //    RET

//        //; **	ｷｰｴﾝﾄﾘｰ ﾁｪｯｸ	**


//        //KEYCHK:	;IN:HL<=WORK:C<=PORT:B<=MASK

//        //    IN  A,(C)
//        //    OR  B
//        //    LD  B,(HL)
//        //    LD(HL),A
//        //  CP	0FFH
//        //  RET Z
//        //  CP  B
//        //  RET

//        //; **	ｼﾞｮｳﾌﾞ 1ｷﾞｮｳ ｦ ｹｽ**

//        //UPCLS:

//        //    LD HL,0F3C8H
//        //   LD  DE,0F3C9H
//        //   LD  BC,119
//        //	LD(HL),0
//        //	LDIR
//        //    RET



//        //RETK:	DB	0
//        //SPACEK:	DB	0

//        //CCODE:
//        //	DB	' 0',0,' 1',0,' 2',0,' 3',0,' 4',0,' 5',0,' 6',0,' 7',0
//        //PROMPT:
//        //	DB	'Find:',0
//        //PROM2:
//        //	DB	'Replace which:',0
//        //REPWD:
//        //	DB	'RET(GO)/SPACE(SKIP)',0AH,0

//        //REPFLG:	DB	0
//        //LF:	DB	0DH,0AH,0
//        //LINKPT:	DW	0
//        //LINE2:	DW	0
//        //LINEADR:	DW	0
//        //HENTOP:	DW	0
//        //HENCO:	DB	0
//        //BEGIN:	DW	0
//        //CUR2:	DW	0


//        //; **	ﾎﾟﾙﾀﾒﾝﾄ ｹｲｻﾝ	**

//        //	; IN:	HL<={CG
//        //    }
//        //    ﾀﾞｯﾀﾗ GﾉﾃｷｽﾄADR
//        //; EXIT:	DE<=Mｺﾏﾝﾄﾞﾉ 3ﾊﾞﾝﾒ ﾉ ﾍﾝｶﾘｮｳ
//        //	;	Zﾌﾗｸﾞ=1 ﾅﾗ ﾍﾝｶｼﾅｲ


//        //CULPTM:
//        //	LD DE,(MDATA)

//        //    PUSH DE

//        //    CALL STTONE

//        //    POP DE

//        //    LD(MDATA),DE
//        //  JR  NC,CPT2
//        //  SCF

//        //    RET
//        //CPT2:
//        //	PUSH HL

//        //    LD C, A

//        //    CALL CULP2

//        //    PUSH AF


//        //    LD A,(BEFCO + 1)

//        //    LD E, A

//        //    LD D,0
//        //	CALL DIV


//        //    LD C, E

//        //    LD B, D

//        //    EX DE, HL

//        //    POP AF

//        //    POP HL

//        //    RET NC

//        //    PUSH HL

//        //    LD HL,0
//        //	AND A

//        //    SBC HL, DE

//        //    EX DE, HL

//        //    POP HL

//        //    AND A

//        //    RET

//        //CULP2:
//        //	;EXIT:	HL<=ﾍﾝｶﾊﾝｲ
//        //	;	CY ﾅﾗ ｻｶﾞﾘﾊｹｲ
//        //	;	Z ﾅﾗ ﾍﾝｶｾｽﾞ

//        //    LD  HL,SETPM4
//        //    LD  DE,SETPM3
//        //    EXX

//        //    LD HL, FNUMB

//        //    EXX
//        //    LD  A,(COMNOW)
//        //    SUB 3
//        //	CP	3
//        //	JR NC, CULP4

//        //    EX DE, HL

//        //    EXX
//        //    LD  HL,SNUMB
//        //    EXX
//        //CULP4:
//        //	LD(W1+1),HL
//        //  LD(W2+1),DE
//        //LD  A,C
//        //EXX

//        //    LD C, A

//        //    LD A,(BEFTONE)

//        //    AND	0FH	;KEY
//        //    ADD A,A
//        //    LD  E,A
//        //    LD  D,0
//        //	ADD HL, DE

//        //    LD E,(HL)

//        //    INC HL

//        //    LD D,(HL)

//        //    LD(FRQBEF),DE

//        //  LD  A,C
//        //  CALL    CTONE
//        //  PUSH    AF
//        //  LD  A,(BEFTONE)
//        //  CALL    CTONE
//        //  LD  C,A
//        //  POP AF
//        //  SUB C
//        //  RET Z
//        //W1:
//        //	JP NC, SETPM4

//        //    NEG
//        //W2:
//        //	JP SETPM3

//        //; BEFTONE>NOWTONE(ｻｶﾞﾘ)


//        //SETPM3:
//        //	LD HL,0A1BBH	;FACC=BBA17180(0.943874)

//        //    LD(CULLP2+1),HL
//        //  LD  HL,08071H
//        //  LD(CULLP3+1),HL
//        //CALL    CULC
//        //EX  DE,HL
//        //LD  HL,(FRQBEF)
//        //AND A
//        //SBC HL,DE
//        //SCF

//        //    RET

//        //;	BEFTONE<NOWTONE (ｱｶﾞﾘ)

//        //SETPM4:
//        //	LD HL,9C62H	;FACC=629C0781(1.05946)

//        //    LD(CULLP2+1),HL
//        //  LD  HL,8107H
//        //  LD(CULLP3+1),HL
//        //CALL    CULC
//        //LD  DE,(FRQBEF)
//        //AND A
//        //SBC HL,DE
//        //AND A
//        //RET
//        //CULC:
//        //	PUSH AF

//        //    CALL ROM

//        //    LD HL,(FRQBEF)

//        //    CALL	21FDH	;STORE HL INTO FACC

//        //    CALL	222FH	;ﾀﾝｾｲﾄﾞ ﾆ ﾍﾝｶﾝ
//        //    CALL    20E8H	;BCDE<=FACC
//        //    POP AF
//        //CULLP:
//        //	PUSH AF
//        //CULLP2:
//        //	LD HL,0A1BBH	;FACC=BBA17180(0.943874)

//        //    LD(0EC41H),HL
//        //CULLP3:
//        //	LD HL,08071H
//        //   LD(0EC43H),HL	;FACC=LHED
//        // LD  A,04
//        //	LD(0EABDH),A
//        //  CALL    1F53H	;FACC=BCDE* FACC..F_NUM*1/(2^(1/12))
//        //	CALL	20E8H	;BCDE<=FACC(NEW F_NUM)

//        //    POP AF

//        //    DEC A

//        //    JR NZ, CULLP


//        //    CALL	21A0H	;ｾｲｽｳ ﾆ ﾍﾝｶﾝ=>HL

//        //    CALL    RAM

//        //    RET


//        //CTONE:
//        //	LD D, A

//        //    AND	11110000B	;OCTAVE
//        //    SRL A
//        //    SRL A
//        //    SRL A
//        //    SRL A
//        //    ADD A,A
//        //    ADD A,A
//        //    LD  E,A
//        //    ADD A,A
//        //    ADD A,E	;*12
//        //	LD E, A

//        //    LD A, D

//        //    AND	0FH
//        //    ADD A,E
//        //    RET


//        //FNUMB:
//        //	DW	26AH,28FH,2B6H,2DFH,30BH,339H,36AH,39EH
//        //    DW  3D5H,410H,44EH,48FH
//        //SNUMB:
//        //	DW	0EE8H,0E12H,0D48H,0C89H,0BD5H,0B2BH,0A8AH,09F3H
//        //    DW  0964H,08DDH,085EH,07E6H
//        //FRQBEF:
//        //	DW	0

//        //
//    }
//}
