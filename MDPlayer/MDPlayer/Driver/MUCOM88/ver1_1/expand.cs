using System;

namespace MDPlayer.Driver.MUCOM88.ver1_1
{
    public class expand
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;
        public msub msub = null;
        public ver1_0.smon smon = null;
        public Action[] tblOpe = null;
        public string DSPMSG_msg;
        public object DSPMSG_option;

        //プログラム書き換えコード対策
        private byte FV3_VAL = 0;
        private byte FV6_VAL = 6;
        private byte FV63_VAL = 4;
        private ushort CULLP2_VAL = 0x0A1BB;
        private ushort CULLP3_VAL = 0x08071;
        private bool CULP2_Ptn = false;
        //


        //N88BASIC EXPAND COMMAND PROGRAM
        //    ORG	0AB00H

        public const int COMWK = 0x0F320;
        public const int MDATA = COMWK;//ｺﾝﾊﾟｲﾙｻﾚﾀ ﾃﾞｰﾀｶﾞｵｶﾚﾙ ｹﾞﾝｻﾞｲﾉ ｱﾄﾞﾚｽ
        public const int DATTBL = MDATA + 4;	// ｹﾞﾝｻﾞｲ ｺﾝﾊﾟｲﾙﾁｭｳ ﾉ MUSIC DATA TABLE TOP
        public const int OCTAVE = DATTBL + 2;
        public const int SIFTDAT = OCTAVE + 1;
        public const int CLOCK = SIFTDAT + 1;
        public const int SECCOM = CLOCK + 1;
        public const int KOTAE = SECCOM + 1;
        public const int LINE = KOTAE + 2;
        public const int ERRORLINE = LINE + 2;
        public const int COMNOW = ERRORLINE + 2;
        public const int COUNT = COMNOW + 1;
        public const int MOJIBUF = COUNT + 1;
        public const int SEC = MOJIBUF + 4;
        public const int MIN = SEC + 1;
        public const int HOUR = MIN + 1;
        public const int ALLSEC = HOUR + 1;
        public const int T_FLAG = ALLSEC + 2;
        public const int SE_SET = T_FLAG + 1;
        public const int FD_FLG = SE_SET + 2;
        public const int FD_EFG = FD_FLG + 1;
        public const int ESCAPE = FD_EFG + 1;
        public const int MINUSF = ESCAPE + 1;
        public const int BEFRST = MINUSF + 1;// ｾﾞﾝｶｲ ｶﾞ 'r' ﾃﾞｱﾙｺﾄｦｼﾒｽ ﾌﾗｸﾞ ｹﾝ ｶｳﾝﾀ
        public const int BEFCO = BEFRST + 1;// ｾﾞﾝｶｲ ﾉ ｶｳﾝﾀ
        public const int BEFTONE = BEFCO + 2;// ｾﾞﾝｶｲ ﾉ ﾄｰﾝ ﾃﾞｰﾀ
        public const int TIEFG = BEFTONE + 9;// ｾﾞﾝｶｲ ｶﾞ ﾀｲﾃﾞｱﾙｺﾄｦ ｼﾒｽ
        public const int COMNO = TIEFG + 1;// ｾﾞﾝｶｲﾉ ｺﾏﾝﾄﾞ｡ ﾄｰﾝﾉﾄｷﾊ 0
        public const int ASEMFG = COMNO + 1;
        public const int VDDAT = ASEMFG + 1;
        public const int OTONUM = VDDAT + 1;// ﾂｶﾜﾚﾃｲﾙ ｵﾝｼｮｸ ﾉ ｶｽﾞ
        public const int VOLUME = OTONUM + 1;// NOW VOLUME

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

        public const int MUSIC = 0x0B000;
        public const int DRIVE = MUSIC + 3 * 5;
        public const int WKGET = MUSIC + 3 * 8;

        public const int MUC88 = 0x09600;
        public const int MUSICSTART = MUC88 + 3 * 3;

        public const int CURSOR = 0x0EF86;

        public const int CLS1 = 0x5F0E;
        public const int SCEDIT = 0x5F92;	//ｽｸﾘｰﾝ ｴﾃﾞｨｯﾄ
        public const int STOPKC = 0x35C2;	//ｽﾄｯﾌﾟｷｰ ﾁｪｯｸ
        public const int BUFCLR = 0x35D9;	//ｷｰﾊﾞｯﾌｧｸﾘｱ
        public const int EDBUF = 0x08C10;	// T_CLKﾅﾄﾞﾄ ｷｮｳﾂｳ ﾉ ﾜｰｸ
        public const int TXTEND = 0x0EB18;	//ﾃｷｽﾄ ｴﾝﾄﾞ
        public const int LNKSET = 0x05BD;	//ﾘﾝｸﾎﾟｲﾝﾀ ｾｯﾄ
        public const int CHGWA = 0x044D5;	// ｳｨﾝﾄﾞｳ->ｼﾞﾂｱﾄﾞﾚｽ ﾍﾝｶﾝ

        // -- CLEAR FROM COMPI1	-->

        public const int T_CLK = 0x08C10;
        public const int UDFLG = T_CLK + 4 * 11;
        public const int BEFMD = UDFLG + 1;
        public const int PTMFG = BEFMD + 2;
        public const int PTMDLY = PTMFG + 1;
        public const int SPACE = PTMDLY + 2;//2*8BYTE ｱｷ ｶﾞ ｱﾙ
        public const int DEFVOICE = SPACE + 2 * 8;
        public const int DEFVSSG = DEFVOICE + 32;
        public const int JCLOCK = DEFVSSG + 32;
        public const int JPLINE = JCLOCK + 2;

        //-<

        //public const int SMON = 0x0DE00;
        //public const int CONVERT = SMON + 3 * 2;


        public expand()
        {
            tblOpe = new Action[] {
                  DSPMSG
                , FOUND
                , PRNFAC
                , FVTEXT
                , COLOR
                , KEYCHK
                , REPLACE
                , CULPTM
            };
        }

        // **	ﾃｷｽﾄ ｶﾗ ﾄｸﾃｲﾉ ﾓｼﾞﾚﾂｦ ｻｶﾞｽ**

        public void FOUND()
        {
            //Z80.HL = PROMPT;
            DSPMSG_msg = PROMPT;
            DSPMSG();

            PC88.CALL(SCEDIT);

            if (Z80.Carry) return;

            Z80.HL++;
            Z80.DE = EDBUF;
            Z80.BC = 32;
            //EDBUF ﾆ ﾃﾝｿｳ
            do
            {
                Mem.LD_8(Z80.DE, Mem.LD_8(Z80.HL));
                Z80.DE++;
                Z80.HL++;
                Z80.BC--;
            } while (Z80.BC != 0);

            UPCLS();
            PRSWD();
            PC88.CALL(CLS1);
            Z80.A = 0;
            //Mem.LD_8(REPFLG, Z80.A);
            REPFLG = Z80.A;

            FO0();
        }

        public void FO0()
        {

            Z80.A = 0;
            //Mem.LD_8(RETK, Z80.A);
            RETK = Z80.A;

            //  DI

            msub.RAM();

            Z80.HL = 1;

        FO1:

            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;

            //Mem.LD_16(LINKPT, Z80.DE);
            LINKPT = Z80.DE;
            //Mem.LD_16(LINEADR, Z80.HL);
            LINEADR = Z80.HL;
            Z80.A = Z80.E;
            Z80.A |= Z80.D;//BASIC END?

            if (Z80.A == 0)
            {
                goto NOTFOUND;
            }

            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;

            //Mem.LD_16(LINE2, Z80.DE);
            LINE2 = Z80.DE;
            //Mem.LD_16(BEGIN, Z80.HL);
            BEGIN = Z80.HL;
            Z80.HL++;
            Z80.HL++;
            Z80.HL++;
            ushort dmy = Z80.DE;
            Z80.DE = Z80.HL;
            Z80.HL = dmy;

        FO12:

            Z80.HL = EDBUF;
            Z80.B = 33;
            ushort stHL;
            //FO2:
            do
            {
                stHL = Z80.HL;

                //Z80.HL = Mem.LD_16(LINKPT);
                Z80.HL = LINKPT;
                Z80.HL--;

                //Z80.A &= Z80.A;//carryを0にする
                Z80.HL -= Z80.DE;//上の行でcarryは0になっているので減算に影響なし
                Z80.Zero = (Z80.HL == 0);
                Z80.HL = stHL;

                if (Z80.Zero) goto FO6;

                Z80.A = Mem.LD_8(Z80.DE);
                Z80.DE++;

                if (Z80.A - Mem.LD_8(Z80.HL) != 0) goto FO12;

                Z80.HL++;

                Z80.A = Mem.LD_8(Z80.HL);

                if (Z80.A == 0)//BUFFER END?
                {
                    goto FO_OK;
                }

                Z80.B--;
            } while (Z80.B != 0);

            goto FO_OK;

        FO6:
            //Z80.HL = Mem.LD_16(LINKPT);
            Z80.HL = LINKPT;

            goto FO1;// ﾂｷﾞ ﾉ ｷﾞｮｳﾍ



        // --	ﾓｼﾞﾚﾂ ﾊｯｹﾝ	--

        FO_OK:
            ;
            //	EI

            msub.ROM();
            ushort stDE = Z80.DE;
            FOCUR();
            stHL = Z80.HL;

            //Z80.DE = Mem.LD_16(LINE2);
            Z80.DE = LINE2;
            PRNFAC();

            //FOK2:
            Z80.A = 0x20;
            //   NOP

            Z80.HL = 0;// FOK2 + 1;
            PC88.RST(0x18, new byte[] { 0x20, 0x00 });

            //Z80.HL = Mem.LD_16(LINEADR);
            Z80.HL = LINEADR;
            Z80.HL++;
            Z80.HL++;
            PC88.CALL(0x44A4);
            PC88.CALL(0x194C);
            Z80.HL = Mem.LD_16(0x0E9B9);
            DSPMSG();
            Z80.HL = 0;// LF;
            DSPMSG_option = LF;
            DSPMSG();

            Z80.HL = Mem.LD_16(CURSOR);
            //Mem.LD_16(CUR2, Z80.HL);
            CUR2 = Z80.HL;
            Z80.HL = stHL;
            Mem.LD_16(CURSOR, Z80.HL);

        FOK3:

            PC88.CALL(0x4290);
            Z80.A = PC88.IN(9);//STOPKEY
            if ((Z80.A & 1) == 0)
            {
                goto NOTF2;
            }

            Z80.HL = RETK;
            Z80.BC = 0x7F01;
            KEYCHK();//ﾘﾀｰﾝｷｰ ﾁｪｯｸ

            if (Z80.Zero)
            {
                goto FOK32;
            }

            if ((Z80.A & 0x80) == 0)
            {
                goto FOK4;
            }

        FOK32:
            Z80.HL = SPACEK;
            Z80.BC = 0x0BF09;
            KEYCHK();
            if (Z80.Zero)
            {
                goto FOK3;
            }

            if ((Z80.A & 0x40) != 0)
            {
                goto FOK3;
            }

            Z80.DE = stDE;
            goto FOK5;// ｽﾍﾟｰｽ ｶﾞ ｵｻﾚﾀﾅﾗ ﾘﾌﾟﾚｰｽ ｽｷｯﾌﾟ

        FOK4:
            Z80.DE = stDE;

            //Z80.A = Mem.LD_8(REPFLG);
            Z80.A = REPFLG;
            Z80.A |= Z80.A;

            if (Z80.A != 0)
            {
                REP2();// ﾘﾌﾟﾚｰｽ ﾅﾗ ﾘﾀｰﾝｷｰﾃﾞ ﾘﾌﾟﾚｰｽﾌﾟﾛｾｽ
            }
        FOK5:
            stDE = Z80.DE;

            PC88.CALL(CLS1);

            Z80.DE = stDE;

            //    DI

            msub.RAM();
            goto FO12;

        NOTFOUND:
            ;
            //    EI

            msub.ROM();
            PC88.CALL(BUFCLR);
            return;

        NOTF2:
            Z80.DE = stDE;// DUMMY
            //Z80.DE = Mem.LD_16(CUR2);
            Z80.DE = CUR2;
            Mem.LD_16(CURSOR, Z80.DE);
            goto NOTFOUND;
        }

        // --	ﾐﾂｹﾀﾄｺﾛﾉ ｶｰｿﾙｻﾞﾋｮｳ ｦ ｴﾙ	--
        // IN:DE<=ﾃｷｽﾄ ｱﾄﾞﾚｽ
        // EXIT:	HL
        public void FOCUR()
        {
            ushort stDE = Z80.DE;

            //Z80.DE = Mem.LD_16(LINE2);
            Z80.DE = LINE2;

            STRFAC();

            Z80.BC = 0x0FFFF;
            Z80.HL++;

        FOC2:
            Z80.HL++;

            Z80.A = Mem.LD_8(Z80.HL);
            //Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto FOC3;
            }

            Z80.BC++;
            goto FOC2;

        FOC3:
            Z80.HL = EDBUF + 1;
        FOC4:
            Z80.A = Mem.LD_8(Z80.HL);
            //Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto FOC5;
            }

            Z80.BC--;
            Z80.HL++;
            goto FOC4;

        FOC5:
            Z80.DE = stDE;
            Z80.DE += Z80.BC;

            //Z80.HL = Mem.LD_16(BEGIN);
            Z80.HL = BEGIN;

            stDE = Z80.DE;
            Z80.DE = Z80.HL;
            Z80.HL = stDE;
            //Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.D = 0;
            Z80.E = 80;

            msub.DIV();

            Z80.D = Z80.E;// ｱﾏﾘ ﾊ Xｻﾞﾋｮｳ
            Z80.E = Z80.L;// ｼｮｳ ﾊ Y ｻﾞﾋｮｳ

            Z80.HL = Mem.LD_16(CURSOR);
            Z80.HL += Z80.DE;
        }




        // **	REPLACE TEXT	**

        public void REPLACE()
        {
            //Z80.HL = REPWD;
            DSPMSG_msg = REPWD;
            DSPMSG();

            //Z80.HL = PROMPT;
            DSPMSG_msg = PROMPT;
            DSPMSG();

            PC88.CALL(SCEDIT);

            if (Z80.Carry)
            {
                return;
            }

            Z80.HL++;
            Z80.DE = EDBUF;
            Z80.BC = 32;
            //EDBUF ﾆ ﾃﾝｿｳ
            do
            {
                Mem.LD_8(Z80.DE, Mem.LD_8(Z80.HL));
                Z80.DE++;
                Z80.HL++;
                Z80.BC--;
            } while (Z80.BC != 0);

            UPCLS();
            PRSWD();

            //Z80.HL = PROM2;
            DSPMSG_msg = PROM2;
            DSPMSG();

            PC88.CALL(SCEDIT);

            if (Z80.Carry)
            {
                return;
            }

            Z80.HL++;
            Z80.DE = EDBUF + 32;
            Z80.BC = 32;
            do
            {
                Mem.LD_8(Z80.DE, Mem.LD_8(Z80.HL));
                Z80.DE++;
                Z80.HL++;
                Z80.BC--;
            } while (Z80.BC != 0);

            PRRWD();

            PC88.CALL(CLS1);
            Z80.A = 0xff;
            //Mem.LD_8(REPFLG, Z80.A);
            REPFLG = Z80.A;
            FO0();
        }

        // **	REPLACE PROCESS MAIN**
        //ON:DE<= TEXT ADR
        public void REP2()
        {
            ushort stDE = Z80.DE;

            // --	ﾍﾝｶﾝｽﾙﾓﾉﾄ ﾍﾝｶﾝｻﾚﾙﾓﾉﾉ ﾓｼﾞｽｳﾉ ｻ	--

            Z80.HL = EDBUF + 1;
            Z80.C = 1;
            //REP21:
            do
            {
                Z80.DE--;
                Z80.A = Mem.LD_8(Z80.HL);
                //Z80.A |= Z80.A;
                if (Z80.A == 0) break;

                Z80.HL++;
                Z80.C++;
            } while (true);

            //REP22:
            //Mem.LD_16(HENTOP, Z80.DE);
            HENTOP = Z80.DE;
            Z80.HL = EDBUF + 32;
            Z80.B = 0;

            //REP23:
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                //Z80.A |= Z80.A;
                if (Z80.A == 0) break;

                Z80.B++;
                Z80.HL++;
            } while (true);

            //REP24:
            Z80.A = Z80.B;
            //Z80.A |= Z80.A;
            if (Z80.A != 0) goto REP25;

            Mem.LD_8(EDBUF + 33, Z80.A);
            Z80.A = 0x20;
            Mem.LD_8(EDBUF + 32, Z80.A);
            Z80.A = 1;

        REP25:
            //Mem.LD_8(HENCO, Z80.A);
            HENCO = Z80.A;
            Z80.Carry = (Z80.A < Z80.C);
            Z80.A -= Z80.C;
            if (!Z80.Carry) goto REPB1;//ﾍﾝｶﾝｻﾚﾙｶﾞﾜｶﾞ ｵｵｷｲﾄｷﾊ REPB1ﾍ
            //REPS1:

            Z80.DE = stDE;
            stDE = Z80.DE;

            ushort dmy = Z80.DE;
            Z80.DE = Z80.HL;
            Z80.HL = dmy;

            Z80.E = Z80.A;
            Z80.D = 0x0FF;
            Z80.HL += Z80.DE;

            ushort stHL = Z80.HL;
            //Z80.HL = Mem.LD_16(LINKPT);
            Z80.HL = LINKPT;
            Z80.HL += Z80.DE;
            //Mem.LD_16(LINKPT, Z80.HL);
            LINKPT = Z80.HL;
            Z80.HL = stHL;
            dmy = Z80.DE;
            Z80.DE = Z80.HL;
            Z80.HL = dmy;
            Z80.HL = stDE;

            stHL = Z80.HL;
            Z80.EXX();
            Z80.HL = Mem.LD_16(TXTEND);
            Z80.DE = stHL;

            //Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.C = Z80.L;
            Z80.B = Z80.H;
            ushort stBC = Z80.BC;
            Z80.EXX();
            Z80.BC = stBC;
            stDE = Z80.DE;
            //    DI
            msub.RAM();
            Z80.LDIR();

            //Z80.DE = Mem.LD_16(HENTOP);
            Z80.DE = HENTOP;
            Z80.HL = EDBUF + 32;
            //Z80.A = Mem.LD_8(HENCO);
            Z80.A = HENCO;
            Z80.C = Z80.A;
            Z80.B = 0;
            Z80.LDIR();

        REPS2:
            msub.ROM();
            //    EI
            PC88.CALL(LNKSET);

            Z80.HL++;
            PC88.CALL(CHGWA);
            Mem.LD_16(TXTEND, Z80.HL);
            Z80.DE = stDE;
            return;

        REPB1:
            //Z80.A |= Z80.A;
            if (Z80.A == 0) goto REPB3;
            //Z80.HL = Mem.LD_16(LINKPT);
            Z80.HL = LINKPT;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            //Mem.LD_16(LINKPT, Z80.HL);
            LINKPT = Z80.HL;
            Z80.HL = Mem.LD_16(TXTEND);

            Z80.DE = stDE;
            stDE = Z80.DE;
            stHL = Z80.HL;
            //Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.HL++;
            Z80.C = Z80.L;
            Z80.B = Z80.H;
            Z80.HL = stHL;
            stHL = Z80.HL;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            dmy = Z80.DE;
            Z80.DE = Z80.HL;
            Z80.HL = dmy;
            Z80.HL = stHL;
            //    DI
            msub.RAM();
            do//	LDDR
            {
                Mem.LD_8(Z80.DE, Mem.LD_8(Z80.HL));
                Z80.DE--;
                Z80.HL--;
                Z80.BC--;
            } while (Z80.BC != 0);

        REPB3:
            //	DI
            msub.RAM();
            Z80.DE = stDE;
            //Z80.DE = Mem.LD_16(HENTOP);
            Z80.DE = HENTOP;
            Z80.HL = EDBUF + 32;
            //Z80.A = Mem.LD_8(HENCO);
            Z80.A = HENCO;
            Z80.C = Z80.A;
            Z80.B = 0;
            Z80.LDIR();
            stDE = Z80.DE;
            goto REPS2;

        }

        //--	PRINT"SEARCH...."	--

        public void PRSWD()
        {
            Z80.HL = 0;// PROMPT;
            Z80.DE = 0x0F3C8;//TextRAMの先頭アドレス
            Z80.BC = 6;

            Z80.LDIR();

            Z80.HL = EDBUF;//使いまわしワーク
            PRS2();
        }

        public void PRRWD()
        {
            Z80.HL = 0;// PROM2;
            Z80.DE = 0x0F3C8 + 23;
            Z80.BC = 15;

            Z80.LDIR();

            Z80.HL = EDBUF + 32;
            PRS2();
        }

        public void PRS2()
        {
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);
                Z80.A |= Z80.A;
                if (Z80.A == 0) return;

                Mem.LD_8(Z80.DE, Z80.A);
                Z80.HL++;
                Z80.DE++;
            } while (true);
        }


        // **	PRINT TO DISPLAY**
        public void DSPMSG()
        {
            System.Console.Write(DSPMSG_msg);
            //do
            //{
            //Z80.A = Mem.LD_8(Z80.HL);
            //Z80.A &= Z80.A;
            //if (Z80.A == 0) return;
            //RST(0x18);
            //Z80.HL++;
            //} while (true);
        }

        // **	PRINT FACC DATA**
        //IN:DE
        public void PRNFAC()
        {
            STRFAC();
            Z80.HL++;
            DSPMSG();
        }

        public void STRFAC()
        {
            Mem.LD_16(0x0EC41, Z80.DE);
            Z80.A = 2;
            Mem.LD_8(0x0EABD, Z80.A);
            PC88.CALL(0x28D0);
        }

        // **	FIND VOICE FROM TEXT	**

        // IN:A<= VOICE NUMBER
        // STORE:6001Hｶﾗ 25BYTE
        //NOTFOUND:SCF
        public void FVTEXT()
        {
            Z80.EXX();
            //Mem.LD_8(FV3 + 1, Z80.A);
            FV3_VAL = Z80.A;
            Z80.A = 0;
            //Mem.LD_8(FVFG, Z80.A);
            FVFG = Z80.A;
            Z80.HL = 0x6001;
            Z80.EXX();

            Z80.HL = 1;

        FV1:
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;

            //Mem.LD_16(LINKPT, Z80.DE);
            LINKPT = Z80.DE;
            //Mem.LD_16(LINEADR, Z80.HL);
            LINEADR = Z80.HL;
            Z80.A = Z80.E;

            //Z80.A |= Z80.D;//BASIC END?
            if (Z80.A == 0)
            {
                FVF2();
                return;
            }

            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;

            //Mem.LD_16(LINE2, Z80.DE);
            LINE2 = Z80.DE;
            Z80.HL++;
            Z80.HL++;
            Z80.HL++;

            //FV2:
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;

            if (Z80.A - 0x20 != 0) goto FV4;

            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);

            if (Z80.A - 0x40 != 0) goto FV4;//'@'0x40

            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);

            if (Z80.A - 0x25 != 0) goto FV22;//'%'0x25

            Z80.HL++;
            //Mem.LD_8(FVFG, Z80.A);
            FVFG = Z80.A;

        FV22:
            msub.REDATA();

            //FV3:
            Z80.A = FV3_VAL;// 0;
            if (Z80.A - Z80.E == 0) goto FV5;
            FV4:
            //Z80.HL = Mem.LD_16(LINKPT);
            Z80.HL = LINKPT;

            goto FV1;

        // ---	%(25BYTEｼｷ) ﾉ ﾄｷ ﾉ ﾖﾐｺﾐ	---


        FV5:
            //Z80.A = Mem.LD_8(FVFG);
            Z80.A = FVFG;
            //Z80.A |= Z80.A;
            if (Z80.A == 0) goto FV52;

            Z80.A = 6;// ﾀﾃ
            //Mem.LD_8(FV6 + 1, Z80.A);
            FV6_VAL = Z80.A;
            Z80.A = 4;//ﾖｺ
            //Mem.LD_8(FV63 + 1, Z80.A);
            FV63_VAL = Z80.A;

            FV6();

            //Z80.HL = Mem.LD_16(LINKPT);
            Z80.HL = LINKPT;
            Z80.DE = 9;
            Z80.HL += Z80.DE;

            msub.REDATA();

            Z80.A = Z80.E;

            Z80.EXX();
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.EXX();

            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;//    RET

        // --	38ﾊﾞｲﾄﾍﾞｰｼｯｸﾎｳｼｷﾉﾄｷﾉ ﾖﾐｺﾐ	--
        FV52:
            //Z80.HL = Mem.LD_16(LINKPT);
            Z80.HL = LINKPT;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;

            Z80.D = Mem.LD_8(Z80.HL);

            //Mem.LD_16(LINKPT, Z80.DE);
            LINKPT = Z80.DE;
            Z80.DE = 8;
            Z80.HL += Z80.DE;

            msub.REDATA();

            ushort stDE = Z80.DE;
            Z80.HL++;
            msub.REDATA();
            Z80.A = Z80.E;
            Z80.DE = stDE;

            Z80.D = Z80.A;

            stDE = Z80.DE;
            Z80.A = 4;// ﾀﾃ
            //Mem.LD_8(FV6 + 1, Z80.A);
            FV6_VAL = Z80.A;
            Z80.A = 9;//ﾖｺ
            //Mem.LD_8(FV63 + 1, Z80.A);
            FV63_VAL = Z80.A;

            FV6();

            Z80.EXX();
            Z80.DE = stDE;
            Mem.LD_8(Z80.HL, Z80.E);//FB
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);//ALGO
            Z80.EXX();

            Z80.HL = 0x6001;
            smon.CONVERT();//38BYTE->25BYTE
            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;//   RET
        }

        // --	ﾖﾐｺﾐ ｻﾌﾞﾙｰﾁﾝ	--
        public void FV6()
        {
            Z80.B = FV6_VAL;// 6;

            //FV62:
            do
            {
                ushort stBC1 = Z80.BC;
                //Z80.HL = Mem.LD_16(LINKPT);
                Z80.HL = LINKPT;
                Z80.E = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.D = Mem.LD_8(Z80.HL);
                //Mem.LD_16(LINKPT, Z80.DE);
                LINKPT = Z80.DE;
                Z80.DE = 8;
                Z80.HL += Z80.DE;

                //FV63:
                Z80.B = FV63_VAL;// 4;

                //FV7:
                do
                {
                    ushort stBC2 = Z80.BC;
                    msub.REDATA();
                    Z80.HL++;// SKIP','
                    Z80.A = Z80.E;

                    Z80.EXX();
                    Mem.LD_8(Z80.HL, Z80.A);
                    Z80.HL++;
                    Z80.EXX();
                    Z80.BC = stBC2;

                    Z80.B--;
                } while (Z80.B != 0);

                //FV8:
                Z80.BC = stBC1;
                Z80.B--;
            } while (Z80.B != 0);

            return;//    RET
        }

        public void FVF2()
        {
            Z80.Carry = true; //ERROR
            return;//    RET
        }

        public byte FVFG = 0;

        // **	COLOR SET	**
        // IN:A<=COLOR CODE
        public void COLOR()
        {
            ushort stHL = Z80.HL;
            ushort stDE = Z80.DE;
            ushort stBC = Z80.BC;

            Z80.C = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.C;

            //TODO: CCODE
            //Z80.HL = CCODE;

            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;

            PC88.CALL(0x6EC6);
            Z80.BC = stBC;
            Z80.DE = stDE;
            Z80.HL = stHL;
            return;//    RET
        }

        // **	ｷｰｴﾝﾄﾘｰ ﾁｪｯｸ	**
        //IN:HL<=WORK:C<=PORT:B<=MASK
        public void KEYCHK()
        {
            Z80.A = PC88.IN(Z80.C);
            Z80.A |= Z80.B;
            Z80.B = Mem.LD_8(Z80.HL);
            Mem.LD_8(Z80.HL, Z80.A);
            if (Z80.A - 0x0FF == 0)
            {
                Z80.Zero = true;
                return;
            }
            if (Z80.A - Z80.B == 0)
            {
                Z80.Zero = true;
            }

            return;//  RET
        }

        public void KEYCHK(ref byte buf)
        {
            Z80.A = PC88.IN(Z80.C);
            Z80.A |= Z80.B;
            Z80.B = buf;
            buf = Z80.A;
            if (Z80.A - 0x0FF == 0)
            {
                Z80.Zero = true;
                return;
            }
            if (Z80.A - Z80.B == 0)
            {
                Z80.Zero = true;
            }
            return;//  RET
        }

        // **	ｼﾞｮｳﾌﾞ 1ｷﾞｮｳ ｦ ｹｽ**
        public void UPCLS()
        {
            Z80.HL = 0x0F3C8;
            Z80.DE = 0x0F3C9;
            Z80.BC = 119;
            Mem.LD_8(Z80.HL, 0);

            do
            {
                Mem.LD_8(Z80.DE, Mem.LD_8(Z80.HL));
                Z80.DE++;
                Z80.HL++;
                Z80.BC--;
            } while (Z80.BC != 0);

        }


        public byte RETK = 0;
        public byte SPACEK = 0;

        public byte[] CCODE = new byte[] {
            //' 0',0,' 1',0,' 2',0,' 3',0,' 4',0,' 5',0,' 6',0,' 7',0
            0x20,0x30,0x00 ,0x20,0x31,0x00 ,0x20,0x32,0x00 ,0x20,0x33,0x00 ,
            0x20,0x34,0x00 ,0x20,0x35,0x00 ,0x20,0x36,0x00 ,0x20,0x37,0x00
        };

        public string PROMPT = "Find:";
        public string PROM2 = "Replace which:";
        public string REPWD = "RET(GO)/SPACE(SKIP)\0xa";

        public byte REPFLG = 0;
        public byte[] LF = new byte[] { 0x0D, 0x0A, 0 };
        public ushort LINKPT = 0;
        public ushort LINE2 = 0;
        public ushort LINEADR = 0;
        public ushort HENTOP = 0;
        public byte HENCO = 0;
        public ushort BEGIN = 0;
        public ushort CUR2 = 0;


        // **	ﾎﾟﾙﾀﾒﾝﾄ ｹｲｻﾝ	**
        // IN:	HL<={CG}ﾀﾞｯﾀﾗ GﾉﾃｷｽﾄADR
        // EXIT:	DE<=Mｺﾏﾝﾄﾞﾉ 3ﾊﾞﾝﾒ ﾉ ﾍﾝｶﾘｮｳ
        //	Zﾌﾗｸﾞ=1 ﾅﾗ ﾍﾝｶｼﾅｲ
        public void CULPTM()
        {
            Z80.DE = Mem.LD_16(MDATA);

            Mem.stack.Push(Z80.DE);
            msub.STTONE();
            Z80.DE = Mem.stack.Pop();
            Mem.LD_16(MDATA, Z80.DE);
            if (!Z80.Carry) goto CPT2;

            Z80.Carry = true;//  SCF
            return;//    RET

        CPT2:
            Mem.stack.Push(Z80.HL);
            Z80.C = Z80.A;

            CULP2();

            Mem.stack.Push(Z80.AF);
            Z80.A = Mem.LD_8(BEFCO + 1);
            Z80.E = Z80.A;
            Z80.D = 0;
            msub.DIV();

            Z80.C = Z80.E;
            Z80.B = Z80.D;

            Z80.EX_DE_HL();

            Z80.AF = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();

            if (!Z80.Carry) return;

            Mem.stack.Push(Z80.HL);

            Z80.HL = 0;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;

            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();

            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;//    RET
        }

        public void CULP2()
        {
            //EXIT:	HL<=ﾍﾝｶﾊﾝｲ
            //	CY ﾅﾗ ｻｶﾞﾘﾊｹｲ
            //	Z ﾅﾗ ﾍﾝｶｾｽﾞ

            //Z80.HL = SETPM4;
            //Z80.DE = SETPM3;
            //Z80.EXX();
            //Z80.HL = FNUMB;
            //Z80.EXX();
            CULP2_Ptn = false;

            Z80.A = Mem.LD_8(COMNOW);
            Z80.A -= 3;
            if (Z80.A - 3 >= 0) goto CULP4;

            //ushort dmy = Z80.DE;
            //Z80.DE = Z80.HL;
            //Z80.HL = dmy;
            //Z80.EXX();
            //Z80.HL = SNUMB;
            //Z80.EXX();
            CULP2_Ptn = true;

        CULP4:
            //Mem.LD_16(W1 + 1, Z80.HL);
            //Mem.LD_16(W2 + 1, Z80.DE);
            //Z80.A = Z80.C;
            //Z80.EXX();

            Z80.C = (byte)(Mem.LD_8(BEFTONE) & 0x0F);//KEY

            if (!CULP2_Ptn)
            {
                Z80.DE = FNUMB[Z80.C];
            }
            else
            {
                Z80.DE = SNUMB[Z80.C];
            }

            //Mem.LD_16(FRQBEF, Z80.DE);
            FRQBEF = Z80.DE;

            CTONE();
            ushort stAF = Z80.AF;
            Z80.A = Mem.LD_8(BEFTONE);
            CTONE();
            Z80.C = Z80.A;
            Z80.AF = stAF;
            Z80.A -= Z80.C;
            if (Z80.A == 0) return;
            //W1:
            if (!Z80.Carry)
            {
                if (!CULP2_Ptn) goto SETPM4;
                else goto SETPM3;
            }

            Z80.A = (byte)-Z80.A;

            //W2:
            if (!CULP2_Ptn) goto SETPM3;
            else goto SETPM4;

            // BEFTONE>NOWTONE(ｻｶﾞﾘ)

            SETPM3:
            Z80.HL = 0x0A1BB;//FACC=BBA17180(0.943874)
            //Mem.LD_16(CULLP2 + 1, Z80.HL);
            CULLP2_VAL = Z80.HL;
            Z80.HL = 0x08071;
            //Mem.LD_16(CULLP3 + 1, Z80.HL);
            CULLP3_VAL = Z80.HL;
            CULC();
            Z80.EX_DE_HL();
            //Z80.HL = Mem.LD_16(FRQBEF);
            Z80.HL = FRQBEF;
            //Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.Carry = true;

            return;//    RET

        //	BEFTONE<NOWTONE (ｱｶﾞﾘ)

        SETPM4:
            Z80.HL = 0x9C62;//FACC=629C0781(1.05946)
            //Mem.LD_16(CULLP2 + 1, Z80.HL);
            CULLP2_VAL = Z80.HL;
            Z80.HL = 0x08107;
            //Mem.LD_16(CULLP3 + 1, Z80.HL);
            CULLP3_VAL = Z80.HL;
            CULC();
            //Z80.DE = Mem.LD_16(FRQBEF);
            Z80.DE = FRQBEF;
            //Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            Z80.A &= Z80.A;
            Z80.Carry = false;
            return;//RET
        }

        public void CULC()
        {
            //ushort stAF = Z80.AF;
            //msub.ROM();
            ////Z80.HL = Mem.LD_16(FRQBEF);
            //Z80.HL = FRQBEF;
            //PC88.CALL(0x21FD);//STORE HL INTO FACC
            //PC88.CALL(0x222F);//ﾀﾝｾｲﾄﾞ ﾆ ﾍﾝｶﾝ
            //PC88.CALL(0x20E8);//BCDE<=FACC
            //Z80.AF = stAF;
            ////CULLP:
            //do
            //{
            //    stAF = Z80.AF;
            //    //CULLP2:
            //    Z80.HL = CULLP2_VAL;// 0x0A1BB;//FACC=BBA17180(0.943874)
            //    Mem.LD_16(0x0EC41, Z80.HL);
            //    //CULLP3:
            //    Z80.HL = CULLP3_VAL;// 0x08071;
            //    Mem.LD_16(0x0EC43, Z80.HL);//FACC=LHED
            //    Z80.A = 04;
            //    Mem.LD_8(0x0EABD, Z80.A);
            //    PC88.CALL(0x1F53);//FACC=BCDE* FACC..F_NUM*1/(2^(1/12))
            //    PC88.CALL(0x20E8);//BCDE<=FACC(NEW F_NUM)
            //    Z80.AF = stAF;
            //    Z80.A--;
            //} while (Z80.A != 0);

            //PC88.CALL(0x21A0);//ｾｲｽｳ ﾆ ﾍﾝｶﾝ=>HL
            //msub.RAM();
            //return;//    RET


            int amul = Z80.A;
            int val = CULLP2_VAL;
            int frq = CULLP3_VAL;
            int ans, count;
            float facc;
            float frqbef = (float)frq;
            if (val == 0x0A1BB)
            {
                facc = 0.943874f;
            }
            else
            {
                facc = 1.059463f;
            }
            for (count = 0; count < amul; count++)
            {
                frqbef = frqbef * facc;
            }
            ans = (int)frqbef;
            Z80.HL=(ushort)ans;
        }

        public void CTONE()
        {
            Z80.D = Z80.A;

            Z80.A &= 0b1111_0000;//OCTAVE
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.E = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.E; //*12
            Z80.E = Z80.A;

            Z80.A = Z80.D;

            Z80.A &= 0x0F;
            Z80.A += Z80.E;
            return;//    RET
        }

        public ushort[] FNUMB = new ushort[] {
            0x26A,0x28F,0x2B6,0x2DF,0x30B,0x339,0x36A,0x39E,
            0x3D5,0x410,0x44E,0x48F
        };

        public ushort[] SNUMB = new ushort[] {
            0x0EE8,0x0E12,0x0D48,0x0C89,0x0BD5,0x0B2B,0x0A8A,0x09F3,
            0x0964,0x08DD,0x085E,0x07E6
        };

        public ushort FRQBEF = 0;

    }
}
