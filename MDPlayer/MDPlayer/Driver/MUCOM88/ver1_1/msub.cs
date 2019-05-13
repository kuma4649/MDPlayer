using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88.ver1_1
{
    public class msub
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;

        // SYSTEM WORK AREA
        // ORG	09000H

        public const int MDATA = 0x0F320;
        public const int DATTBL = MDATA + 4;
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
        public const int MEMEND = 0x0E3F0;//0DDF0H
        public const int ERRORTBL = 0x08800;

        //JP MWRITE
        //JP MWRIT2
        //JP ERRT
        //JP ERRORSN
        //JP ERRORIF
        //JP ERRORNF
        //JP ERRORFN
        //JP ERRORVF
        //JP ERROROO
        //JP ERRORND
        //JP ERRORRJ	;+3*10
        //JP STTONE
        //JP STLIZM
        //JP REDATA
        //JP MULT
        //JP DIV
        //JP HEXDEC
        //JP HEXPRT
        //JP ROM
        //JP RAM
        //JP FMCOMC;+3*20
        //JP T_RST
        //JP ERRORNE
        //JP ERRORDC
        //JP ERRORML
        //JP MCMP
        //JP ERRORVO
        //JP ERRORMD
        //JP ERRORME
        //JP CULSEC
        //JP CULTIM
        //JP GETTIME
        //JP ERRNMC
        //JP ERREMC


        // ***	MDATA ﾆ COMMABD ﾄ DATA ｦ ｶｷｺﾑ***
        // IN: A<= COMMAND No.
        // E<= COMMAND DATA
        public void MWRITE()
        {
            Z80.BC = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.BC, Z80.A);
            Z80.BC++;
            Z80.A = Z80.E;
            Mem.LD_8(Z80.BC, Z80.A);
            Z80.BC++;
            Mem.LD_16(MDATA, Z80.BC);
            Mem.stack.Push(Z80.HL);
            Z80.HL = MEMEND;
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.BC < 0);
            Z80.HL -= Z80.BC;
            Z80.HL = Mem.stack.Pop();
            if (Z80.Carry)
            {
                MAD2();
                return;
            }
            MADR();
        }

        public void MADR()
        {
            Mem.stack.Push(Z80.IY);
            Mem.stack.Push(Z80.HL);
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.BC);
            Z80.L = Z80.C;
            Z80.H = Z80.B;
            HEXPRT();
            Z80.HL = MOJIBUF;
            Z80.DE = 0x0F3C8 + 36;
            Z80.BC = 4;
            Z80.LDIR();
            Z80.BC = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();
            Z80.IY = Mem.stack.Pop();
            //    RET
        }

        public void MAD2()
        {
            Z80.DE = Mem.stack.Pop();
            ERRORME();//JP
        }

        // ***	MDATA ﾆ COMMAND ｵﾖﾋﾞ DATA ｦ ｶｷｺﾑ***
        //	IN: A<= COMMAND No.or DATA
        public void MWRIT2()
        {
            Mem.stack.Push(Z80.BC);

            Z80.BC = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.BC, Z80.A);
            Z80.BC++;
            Mem.LD_16(MDATA, Z80.BC);

            Mem.stack.Push(Z80.HL);

            Z80.HL = MEMEND;
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.BC < 0);
            Z80.HL -= Z80.BC;

            Z80.HL = Mem.stack.Pop();

            if (Z80.Carry)
            {
                MAD2();
                return;
            }
            MADR();

            Z80.BC = Mem.stack.Pop();
            //    RET
        }

        // ***	ERROR TRAP	***
        public void ERRT()
        {
            Z80.HL++;
            REDATA();
            if (Z80.Carry) {
                ERRSN();
                return;
            }
            //Z80.A |= Z80.A;
            if (Z80.A != 0) {
                ERRIF();
                return;
            }

            return;//    RET
        }

        public void ERRSN()
        {
            Z80.DE = Mem.stack.Pop();
            ERRORSN();
        }

        public void ERRIF()
        {
            Z80.DE = Mem.stack.Pop();
            ERRORIF();
        }

        // ***   ERROR PROCESS   ***

        public void ERRORSN()
        {
            Z80.A ^= Z80.A;
            ERROR();
        }

        public void ERRORIF()
        {
            Z80.A = 1;
            ERROR();
        }

        public void ERRORNF()
        {
            Z80.A = 2;
            ERROR();
        }

        public void ERRORFN()
        {
            Z80.A = 3;
            ERROR();
        }

        public void ERRORVF()
        {
            Z80.A = 4;
            ERROR();
        }

        public void ERROROO()
        {
            Z80.A = 5;
            ERROR();
        }

        public void ERRORND()
        {
            Z80.A = 6;
            ERROR();
        }

        public void ERRORRJ()
        {
            Z80.A = 7;
            ERROR();
        }

        public void ERRORNE()
        {
            Z80.A = 8;
            ERROR();
        }

        public void ERRORDC()
        {
            Z80.A = 9;
            ERROR();
        }

        public void ERRORML()
        {
            Z80.A = 0xa;
            ERROR();
        }

        public void ERRORVO()
        {
            Z80.A = 0xb;
            ERROR();
        }

        public void ERROROF()
        {
            Z80.A = 0xc;
            ERROR();
        }

        public void ERRORMD()
        {
            Z80.A = 0xd;
            ERROR();
        }

        public void ERRORME()
        {
            Z80.A = 0xe;
            ERROR();
        }

        public void ERRNMC()
        {
            Z80.A = 0xf;
            ERROR();
        }

        public void ERREMC()
        {
            Z80.A = 0x10;
            ERROR();
        }

        //IN:A<=ERROR CODE

        public void ERROR()
        { 
            Mem.LD_16(0x0EFBA, Z80.HL);
            Z80.EX_AF_AF();
            Z80.A ^= Z80.A;

            Mem.LD_8(COMNOW, Z80.A);
            Z80.HL = Mem.LD_16(MDATA);
            Mem.LD_8(Z80.HL, Z80.A);

            Z80.HL = Mem.LD_16(MDATA + 2);
            Mem.LD_16(MDATA, Z80.HL);

            Z80.HL = Mem.LD_16(LINE);
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);// DE AS ERROR LINE

            Mem.LD_16(ERRORLINE, Z80.DE);

            ROM();

            Z80.HL = Mem.stack.Pop();

            Mem.stack.Push(Z80.HL);
            Z80.HL = Mem.LD_16(0x0EF86);
            Mem.stack.Push(Z80.HL);// STORE CURSOR

            Z80.HL = 0x0F3C8;
            Z80.DE = 0x0F3C9;
            Mem.LD_8(Z80.HL, 0);
            Z80.BC = 79;

            Z80.LDIR();

            Z80.EX_AF_AF();// A as ERROR CODE
            Z80.A += Z80.A;

            Z80.HL = ERRORTBL;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.DE = 16 * 256 + 1;
            Mem.LD_16(0x0EF86, Z80.DE);
            PC88.CALL(0x5550);

            Z80.DE = 1 * 256 + 1;// CURSOR X, Y

            Mem.LD_16(0x0EF86, Z80.DE);
            //TODO:
            //Z80.HL = ERRORMSG;
            PC88.CALL(0x5550);

            Z80.HL = Mem.LD_16(ERRORLINE);
            Mem.LD_16(0x0E656, Z80.HL);

            Z80.HL = Mem.stack.Pop();
            Mem.LD_16(0x0EF86, Z80.HL);
            Z80.HL = Mem.stack.Pop();
            Z80.E = 2;// ERROR CODE

            //TODO:
            //goto 0x03B3;// ERROR PROCESS
            //    RET
            PC88.CALL(0x3b3);
        }

        public static byte[] ERLI = new byte[] {
            0x20,0,0,0,0,0,0
        };

        // ***	TONE SET	***
        // IN  :A<= MML DATA
        // EXIT:A<= KEY CODE DATA
        public void STTONE()
        {
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.HL);

            Z80.A = Mem.LD_8(Z80.HL);
            Z80.C = Z80.A;
            //Z80.HL = TONES;
            Z80.HL = 0;
            Z80.B = 7;// TONE NUMBER(c-b)
        //TNLP0:
            do
            {
                //Z80.A = Mem.LD_8(Z80.HL);
                Z80.A = TONES[Z80.HL];
                if (Z80.A - Z80.C == 0)
                {
                    //log.Write(string.Format("Note:{0}",Encoding.GetEncoding("Shift_JIS").GetString(new byte[] { (byte)(Z80.A) })));
                    TONEXT();
                    return;
                }

                Z80.HL++;
                Z80.HL++;
                Z80.B--;
            } while (Z80.B != 0);

            Z80.HL = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();

            Z80.Carry = true;//ERROR CODE
            //    RET
        }

        public void TONEXT()
        {
            Z80.HL++;
            //Z80.C = Mem.LD_8(Z80.HL);// GET KEY CODE DATA
            Z80.C = TONES[Z80.HL];
            Z80.A = Mem.LD_8(OCTAVE);
            Z80.B = Z80.A;// STORE A
            Z80.HL = Mem.stack.Pop();
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);

            // ---   SHARP PROCESS   ---
            if (Z80.A - 0x2b != 0)//'+'
            {
                goto TONEX0;
            }
            Z80.A = Z80.C;
            if (Z80.A - 11 != 0)// KEY='b'?
            {
                goto TONEX3;
            }
            Z80.C = 0xff;
            Z80.B++;// OCTAVE=OCTAVE+1
            Z80.A = Z80.B;
            if (Z80.A - 8 != 0)// OCTAVE=9?
            {
                goto TONEX3;
            }
            Z80.B = 7;// OCTAVE=8
        TONEX3:
            Z80.C++;// KEY CODE +1
            goto TONEX2;

        // ---   FLAT PROCESS   ---
        TONEX0:
            if (Z80.A - 0x2d != 0)// '-'
            {
                goto TONEX1;
            }
            Z80.A = Z80.C;
            if (Z80.A != 0)//    Z80.A |= Z80.A; KEY='c'?
            {
                goto TONEX4;
            }
            Z80.C = 12;
            Z80.B--;// OCTAVE=OCTAVE-1
            if (Z80.B == 0xff)
            {
                goto TONEX4;
            }
            Z80.B = 0;// OCTAVE=0
        TONEX4:
            Z80.C--;// KEY CODE -1
            goto TONEX2;
        TONEX1:
            Z80.HL--;

        // ---   MAKE TONE DATA   ---
        TONEX2:
            Mem.stack.Push(Z80.HL);
            KEYSIFT();
            Z80.HL = Mem.stack.Pop();
            Z80.A = Z80.B;// RESTORE A
            Z80.A <<= 4;
            Z80.A |= Z80.C;// A=OCTAVE & KEY CODE
            Z80.Carry = false;
            Z80.DE = Mem.stack.Pop();
            //    RET
        }

        public void KEYSIFT()
        {
            Z80.A = Mem.LD_8(SIFTDAT);
            //    Z80.A |= Z80.A
            if (Z80.A == 0) return;
            Mem.stack.Push(Z80.BC);
            Z80.H = 0;
            Z80.L = Z80.B; // OCTAVE
            Z80.DE = 12;
            MULT();
            Z80.BC = Mem.stack.Pop();
            Z80.A = Z80.C;
            Z80.Carry = (Z80.A + Z80.L > 0xff);
            Z80.A += Z80.L;
            Z80.L = Z80.A;
            Z80.A += (byte)(Z80.H + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.L;
            Z80.H = Z80.A; // HL = OCTAVE * 12 + KEYCODE
            Z80.A = Mem.LD_8(SIFTDAT);
            if (Z80.A - 128 < 0)
            {
                goto KYS2;
            }
            Z80.D = 0xff;
            goto KYS3;
        KYS2:
            Z80.D = 0;
        KYS3:
            Z80.E = Z80.A;
            Z80.HL += Z80.DE;
            Z80.DE = 12;
            DIV();
            Z80.B = Z80.L; // B as OCTAVE
            Z80.C = Z80.E; // C as KEY CODE
                           //RET
        }

        // ***   ﾘｽﾞﾑ ｾｯﾃｲ   ***

        // IN  : HL<= TEXT ADR
        // EXIT: A <= LIZM COUNT DATA

        public void STLIZM()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x25 != 0)//0x25  = '%'
            {
                goto SL2;
            }
            Z80.HL++;
            REDATA();
            if (Z80.Carry)
            {
                goto SL3;
            }
            //Z80.A |= Z80.A
            if (Z80.A != 0)
            {
                goto STLIER;
            }
            Z80.A = Z80.E;
            return;
        SL2:
            REDATA();
            if (Z80.Carry)
            {
                goto STLIZ2;
            }
            //    Z80.A |= Z80.A
            if (Z80.A != 0)
            {
                goto STLIER;
            }
            Z80.A = Mem.LD_8(CLOCK);
            if (Z80.A - Z80.E < 0)
            {
                goto STLIER;//CLOCK<E ﾃﾞ ERROR
            }
            Mem.stack.Push(Z80.HL);
            Z80.H = 0;
            Z80.L = Z80.A; // HL=CLOCK : DE = LIZM
            DIV();
            Z80.A = Mem.LD_8(KOTAE);// GET COUNTER
            Z80.HL = Mem.stack.Pop();
            Z80.E = 0;
            Z80.C = Z80.A;
        STLIZ0:
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2e != 0)//0x2e = '.'
            {
                goto STLIZ1;
            }
            Z80.HL++;
            Z80.C >>= 1;// /2
            Z80.A = Z80.C;
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A = Z80.C;
            goto STLIZ0;
        STLIZ1:
            Z80.A = Mem.LD_8(KOTAE);
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            if (!Z80.Carry) {
                return;
            }
            Z80.DE = Mem.stack.Pop();// DUM
            ERROROF();
            return;
        STLIZ2:
            Z80.E = 0;
            Z80.A = Mem.LD_8(COUNT);
            Z80.C = Z80.A;
        STLIZ4:
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x2e != 0)//0x2e = '.'
            {
                goto STLIZ5;
            }
            Z80.HL++;
            Z80.C >>= 1;
            Z80.A = Z80.C;
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A = Z80.C;
            goto STLIZ4;
        STLIZ5:
            Z80.A = Mem.LD_8(COUNT);// NOW COUNT
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            if (!Z80.Carry)
            {
                return;
            }
            Z80.DE = Mem.stack.Pop();// DUM
            ERROROF();
            return;
        STLIER:
            Z80.DE = Mem.stack.Pop();// DUMMY POP
            ERRORIF();
            return;
        SL3:
            Z80.DE = Mem.stack.Pop();
            ERRORSN();
            return;
        }

        // ***	ﾃﾞｰﾀ ﾖﾐｺﾐ ﾙｰﾁﾝ(2 BYTE) ***
        //	IN	: HL<= TEXT ADR
        // EXIT	: DE<= DATA : HL<= NEXT TEXT ADR
        //		: NON DATA ﾅﾗ SET CARRYｼﾃ Aﾆ ﾂｷﾞ ﾉ ﾓｼﾞﾚﾂ
        //		: ERROR...A=1 : NON ERROR...A=0
        // exam	: c16 ﾅﾗ 16 ｦ ﾖﾝﾃﾞ DE ﾆ ｶｴｽ

        public void REDATA()
        {
            Mem.stack.Push(Z80.BC);
            Mem.stack.Push(Z80.HL);
            //Z80.HL = SCORE;
            //Z80.DE = SCORE + 1;
            //Mem.LD_8(Z80.HL, 0);
            //Z80.BC = 5;
            //Z80.LDIR();// INIT SCORE
            for (int i = 0; i < SCORE.Length; i++) SCORE[i] = 0;
            Z80.HL = Mem.stack.Pop();
            Z80.B = 5;   // 5ｹﾀ ﾏﾃﾞ

            Z80.A ^= Z80.A;
            Mem.LD_8(HEXFG, Z80.A);
            Mem.LD_8(MINUSF, Z80.A);
        READ0:			// FIRST CHECK
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0x20 == 0)
            {
                goto RE01;
            }
            if (Z80.A - 0x24 == 0)//0x24 = '$'
            {
                goto READH;
            }
            if (Z80.A - 0x2d == 0)//0x2d = '-'
            {
                goto READ9;
            }
            if (Z80.A - 0x30 < 0)//0x30 = '0'
            {
                goto READE;//0ｲｼﾞｮｳ ﾉ ｷｬﾗｸﾀﾅﾗ ﾂｷﾞ
            }
            if (Z80.A - 0x3A >= 0)//0x3A = ':' :は9の次に定義されている文字
            {
                goto READE;//9ｲｶﾅﾗ ﾂｷﾞ
            }
            goto READ7;

        RE01:
            Z80.HL++;
            goto READ0;
        READH:
            Z80.HL++;
            Z80.A = 1;
            Mem.LD_8(HEXFG, Z80.A);
            goto READ7;
        READ9:          // MINUS CHECK
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);

            if (Z80.A - 0x30 < 0)//0x30 = '0'
            {
                goto READE;//0ｲｼﾞｮｳ ﾉ ｷｬﾗｸﾀﾅﾗ ﾂｷﾞ
            }
            if (Z80.A - 0x3A >= 0)//0x3A = ':' :は9の次に定義されている文字
            {
                goto READE;//9ｲｶﾅﾗ ﾂｷﾞ
            }

            Z80.A = 1;
            Mem.LD_8(MINUSF, Z80.A);    // SET MINUS FLAG
        READ7:
            do
            {
                Z80.A = Mem.LD_8(Z80.HL);       // SECOND CHECK
                Z80.D = Z80.A;
                Z80.A = Mem.LD_8(HEXFG);
                Z80.A |= Z80.A;
                Z80.Zero = (Z80.A == 0);
                Z80.A = Z80.D;
                if (Z80.Zero)
                {
                    goto READC;
                }
                if (Z80.A - 0x61 < 0)//0x61 = 'a'
                {
                    goto READG;
                }
                if (Z80.A - 0x67 >= 0)//0x67 = 'g'
                {
                    goto READG;
                }
                Z80.A -= 32;
            READG:
                if (Z80.A - 0x41 < 0)//0x61 = 'A'
                {
                    goto READC;
                }
                if (Z80.A - 0x47 >= 0)//0x67 = 'G'
                {
                    goto READC;
                }
                Z80.A -= 7;
                goto READF;
            READC:
                if (Z80.A - 0x30 < 0)//0x30 = '0'
                {
                    goto READ1;//0ｲｼﾞｮｳ ﾉ ｷｬﾗｸﾀﾅﾗ ﾂｷﾞ
                }
                if (Z80.A - 0x3A >= 0)//0x3A = ':' :は9の次に定義されている文字
                {
                    goto READ1;//9ｲｶﾅﾗ ﾂｷﾞ
                }
            READF:
                Mem.stack.Push(Z80.HL);
                Mem.stack.Push(Z80.BC);
                //Z80.HL = SCORE + 1;
                //Z80.DE = SCORE;
                //Z80.BC = 5;
                //Z80.LDIR();
                SCORE[0] = SCORE[1];
                SCORE[1] = SCORE[2];
                SCORE[2] = SCORE[3];
                SCORE[3] = SCORE[4];
                SCORE[4] = SCORE[5];
                Z80.BC = Mem.stack.Pop();
                Z80.HL = Mem.stack.Pop();
                Z80.A -= 0x30;// A= 0 - 9
                //Mem.LD_8(SCORE + 4, Z80.A);
                SCORE[4] = Z80.A;
                Z80.HL++;   // NEXT TEXT
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = Mem.LD_8(Z80.HL);   // THIRD CHECK
            if (Z80.A - 0x30 < 0)//0x30 = '0'
            {
                goto READ1;//0ｲｼﾞｮｳ ﾉ ｷｬﾗｸﾀﾅﾗ ﾂｷﾞ
            }
            if (Z80.A - 0x3A >= 0)//0x3A = ':' :は9の次に定義されている文字
            {
                goto READ1;//9ｲｶﾅﾗ ﾂｷﾞ
            }
        //READ8:
            Z80.A &= Z80.A;// CY=0
            Z80.A = 1;// ERROR SIGN
            Z80.BC = Mem.stack.Pop();
            return;//RET	; 7ｹﾀｲｼﾞｮｳ ﾊ ｴﾗｰ
        READ1:
            Z80.A = Mem.LD_8(HEXFG);
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto READD;
            }
            //Z80.A = Mem.LD_8(SCORE + 1);
            Z80.A = SCORE[1];
            Z80.RLCA();
            Z80.RLCA();
            Z80.RLCA();
            Z80.RLCA();
            Z80.D = Z80.A;
            //Z80.A = Mem.LD_8(SCORE + 2);
            Z80.A = SCORE[2];
            Z80.A |= Z80.D;
            Z80.D = Z80.A;
            //Z80.A = Mem.LD_8(SCORE + 3);
            Z80.A = SCORE[3];
            Z80.RLCA();
            Z80.RLCA();
            Z80.RLCA();
            Z80.RLCA();
            Z80.E = Z80.A;
            //Z80.A = Mem.LD_8(SCORE + 4);
            Z80.A = SCORE[4];
            Z80.A |= Z80.E;
            Z80.E = Z80.A;
            goto READA;
        READD:
            Mem.stack.Push(Z80.HL);
            Z80.HL = 0;
            Z80.DE = 10000; // 10000 ﾉ ｹﾀ
            //Z80.A = Mem.LD_8(SCORE);
            Z80.A = SCORE[0];
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto READSEN;
            }
            Z80.B = Z80.A;
        //READMAN:
            do
            {
                Z80.HL += Z80.DE;
                Z80.B--;
            } while (Z80.B != 0);
        READSEN:
            Z80.DE = 1000;
            //Z80.A = Mem.LD_8(SCORE + 1);// 1000 ﾉ ｹﾀ
            Z80.A = SCORE[1];// 1000 ﾉ ｹﾀ
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto READHYAKU;
            }
            Z80.B = Z80.A;
        //READSEN2:
            do
            {
                Z80.HL += Z80.DE;
                Z80.B--;
            } while (Z80.B != 0);
        READHYAKU:
            Z80.DE = 100;
            //Z80.A = Mem.LD_8(SCORE + 2);// 100 ﾉ ｹﾀ
            Z80.A = SCORE[2];// 100 ﾉ ｹﾀ
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto READ4;
            }
            Z80.B = Z80.A;
        //READ2:
            do
            {
                Z80.HL += Z80.DE;
                Z80.B--;
            } while (Z80.B != 0);
        READ4:
            //Z80.A = Mem.LD_8(SCORE + 3);// 10 ﾉ ｹﾀ
            Z80.A = SCORE[3];// 10 ﾉ ｹﾀ
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto READ5;
            }
            Z80.B = Z80.A;
            Z80.A = 0;
            Z80.C = 10;
        //READ3:
            do
            {
                Z80.A += Z80.C;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.C = Z80.A;
            goto READ6;
        READ5:
            Z80.C = 0;
        READ6:
            //Z80.A = Mem.LD_8(SCORE + 4);// 1 ﾉ ｹﾀ
            Z80.A = SCORE[4];// 1 ﾉ ｹﾀ
            Z80.A += Z80.C;
            Z80.Carry = (Z80.A + Z80.L > 0xff);
            Z80.A += Z80.L;
            Z80.L = Z80.A;
            Z80.A += (byte)(Z80.H + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.L;
            Z80.H = Z80.A;
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.A = Mem.LD_8(MINUSF);// CHECK MINUS FLAG
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto READA;// NON MINUS
            }
            Mem.stack.Push(Z80.HL);
            Z80.A ^= Z80.A;// CY=0
            Z80.HL = 0;
            Z80.HL -= Z80.DE;// DE ﾉ ﾎｽｳ ｦ ﾄﾙ
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
        READA:
            Z80.A ^= Z80.A; //CY=0
            Z80.Carry = false;
            Z80.BC = Mem.stack.Pop();
            return;//    RET
        READE:
            Mem.LD_8(SECCOM, Z80.A);
            Z80.Carry = true; // NON DATA
            Z80.BC = Mem.stack.Pop();
            //    RET
        }

        public byte[] SCORE = {
            0,0,0,0,0,0
        };

        public byte HEXFG = 0; //HEX FLAG

        public byte[] TONES = new byte[] {
             0x63,0 //  'c' ,0
            ,0x64,2 //  'd' ,2
            ,0x65,4 //  'e' ,4
            ,0x66,5 //  'f' ,5
            ,0x67,7 //  'g' ,7
            ,0x61,9 //  'a' ,9
            ,0x62,11//   'b' ,11
        };

        // ***	ｶｹｻﾞﾝ & ﾜﾘｻﾞﾝ***

        //
        //	HL = HL* E, HL = HL / DE
        //
        public void MULT()
        {
            Z80.D = 0;
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                MULT4();
                return;
            }
            Z80.B = Z80.E;
            Z80.EX_DE_HL();
            Z80.HL = 0;
        //MULT2:
            do
            {
                Z80.HL += Z80.DE;
                Z80.B--;
            } while (Z80.B != 0);
        //MULT3:
            Mem.LD_16(KOTAE, Z80.HL);
            return;//  RET
        }

        public void MULT4()
        {
            Z80.HL = 0;
            //goto MULT3;
            Mem.LD_16(KOTAE, Z80.HL);
            return;//  RET
        }

        public void DIV()
        {
            Z80.A = Z80.L;
            Z80.A |= Z80.H;
            if (Z80.A != 0)
            {
                goto MULT0;
            }
            Z80.DE = 0;
            MULT4();
            return;
        MULT0:
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                MULT4();
                return;
            }
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.DE < 0);
            Z80.HL -= Z80.DE;
            if (!Z80.Carry)
            {
                goto DIV1;
            }
            Z80.HL += Z80.DE;
            Z80.EX_DE_HL();
            MULT4();
            return;
        DIV1:
            Z80.BC = 0;
        DIV2:
            Z80.BC++;
            Z80.A &= Z80.A;
            Z80.Carry = (Z80.HL - Z80.DE < 0);
            Z80.HL -= Z80.DE;
            if (!Z80.Carry)
            {
                goto DIV2;
            }
            Z80.HL += Z80.DE;
            Z80.EX_DE_HL();
            Z80.L = Z80.C;
            Z80.H = Z80.B;
            //goto MULT3;
            Mem.LD_16(KOTAE, Z80.HL);
        }

        // ***   10 ｼﾝ ﾍﾝｶﾝ ﾙｰﾁﾝ***

        // 	ENTRY: HL<= DATA
        //EXIT:HL<=NUMBUF ADR
        /// <summary>
        /// 注意：HLはNUMBEUFのアドレスを示していません
        /// </summary>
        public void HEXDEC()
        {
            Mem.stack.Push(Z80.BC);
            //Z80.IY = NUMBUF;
            Z80.IY = 0;
            Z80.BC = 10000;
            DEVS();
            Z80.BC = 1000;
            DEVS();
            Z80.BC = 100;
            DEVS();
            Z80.BC = 10;
            DEVS();
            Z80.A = Z80.L;
            Z80.A += 0x30;
            //Mem.LD_8(Z80.IY, Z80.A);
            //Mem.LD_8((ushort)(Z80.IY + 1), 0); // END OF DATA
            NUMBUF[Z80.IY] = Z80.A;
            NUMBUF[Z80.IY+1] = 0;
            //Z80.HL = NUMBUF;
            Z80.HL = 0;
            Z80.BC = Mem.stack.Pop();

            //RET
        }

        // ---	DIVISION SCORE	---

        public void DEVS()
        {
            Z80.A ^= Z80.A;
        DEVS1:
            Z80.Carry = (Z80.HL - Z80.BC < 0);
            Z80.HL -= Z80.BC;
            if (Z80.Carry)
            {
                goto DEVS2;
            }
            Z80.A++;
            goto DEVS1;
        DEVS2:
            Z80.HL += Z80.BC;
            Z80.A += 0x30;
            Mem.LD_8(Z80.IY, Z80.A);
            Z80.IY++;
            //  RET
        }

        public byte[] NUMBUF = new byte[] {
                0,0,0,0,0,0,0,0,0,0
        };

        // ***   16 ｼﾝ DATA ﾋｮｳｼﾞ***

        //	IN:<= HL DATA
        public void HEXPRT()
        {
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.BC);
            Mem.stack.Push(Z80.AF);
            Z80.IY = MOJIBUF;
            Z80.A = Z80.H;
            HEXPRT2();
            Z80.A = Z80.L;
            HEXPRT2();
            Z80.HL = MOJIBUF;
            Z80.AF = Mem.stack.Pop();
            Z80.BC = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();
            //    RET
        }

        public void HEXPRT2()
        {
            Z80.C = Z80.A;
            Z80.A &= 0b1111_0000;
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();

            MOJISAVE();
            Mem.LD_8(Z80.IY, Z80.A);
            Z80.IY++;
            Z80.A = Z80.C;
            Z80.A &= 0b0000_1111;
            MOJISAVE();
            Mem.LD_8(Z80.IY, Z80.A);
            Z80.IY++;

            //RET
        }


        public void MOJISAVE()
        {
            //Z80.DE = MOJIDATA;
            //Z80.Carry = (Z80.A + Z80.E > 0xff);
            //Z80.A += Z80.E;
            //Z80.E = Z80.A;
            //Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            //Z80.A -= Z80.E;
            //Z80.D = Z80.A;
            //Z80.A = Mem.LD_8(Z80.DE);
            Z80.A = MOJIDATA[Z80.A];
            Z80.Carry = false;
            //    RET
        }

        public byte[] MOJIDATA = new byte[] {
            0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,//'0','1','2','3','4','5','6','7',
            0x38,0x39,0x41,0x42,0x43,0x44,0x45,0x46 //'8','9','A','B','C','D','E','F'
        };

        // ***	TO ROM/RAM MODE	***
        public void ROM()
        {
            Z80.A = Mem.LD_8(0x0E6C2);
            PC88.OUT(0x31, Z80.A);
            //   RET
        }

        public void RAM()
        {
            Z80.A = Mem.LD_8(0x0E6C2);
            Z80.A |= 2;
            PC88.OUT(0x31, Z80.A);
            //   RET
        }

        // ***	COMMAND ﾁｪｯｸ	***

        // IN A<= DATA :  IX<= COMMAND TABLE ADR
        //	EXIT: C <= FCOMS ﾉ ﾅﾝﾊﾞﾝﾒ ﾉ ｺﾏﾝﾄﾞ(1 - n)
        // NOT COMMAND ﾅﾗ C <= 0

        public void FMCOMC()
        {
            //Z80.IX = FCOMS;
            Z80.IX = 0;
            Z80.C = 1;
            Z80.D = Z80.A;
        FCMLP:
            //Z80.A = Mem.LD_8(Z80.IX);
            Z80.A = FCOMS[Z80.IX];
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto FCMLP2;
            }
            if (Z80.A - Z80.D == 0)
            {
                return;// SEARCH COM.
            }
            Z80.IX++;
            Z80.C++;
            goto FCMLP;
        FCMLP2:
            //log.Write(string.Format("Cmd '{0}' is not found.", (char)Z80.D));
            Z80.C = Z80.A;
            //RET     ; NO COM.
        }

        // ***	TIME RESET	***

        public void T_RST()
        {
            Z80.A ^= Z80.A;
            Mem.LD_8(HOUR, Z80.A);
            Mem.LD_8(SEC, Z80.A);
            Mem.LD_8(MIN, Z80.A);
            //DI
            GETTIME();// TIME READ
            //    EI

            Z80.HL = 0xf00f;//TIME DATA WORK
            CULSEC();// CONVERT SEC ALL
            Mem.LD_16(ALLSEC, Z80.HL);
            Z80.A = 0xff;
            Mem.LD_8(T_FLAG, Z80.A);
            //RET
        }

        public void GETTIME()
        {
            Z80.HL = 0xf00d;
            Z80.A = 3;
            CLKCOM();

            Z80.A = 1;
            CLKCOM();

            Z80.D = 5;
        GETT1:
            Z80.B = 8;

            Z80.E = 0;
        //GETT2:
            do
            {
                Z80.A = PC88.IN(0x40);

                Z80.RRCA();
                Z80.RRCA();

                Z80.RRCA();
                Z80.RRCA();

                Z80.A &= 1;
                Z80.A |= Z80.E;

                Z80.RRCA();
                Z80.E = Z80.A;
                CLKSFT();
                Z80.B--;
            } while (Z80.B != 0);

            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Z80.D--;
            if (Z80.D != 0) {
                goto GETT1;
            }
            //  RET
        }

        public void CLKCOM()
        {
            PC88.OUT(0x10, Z80.A);
            Z80.C = 2;
            CLKSND();
        }

        public void CLKSFT()
        {
            Z80.C = 4;
            CLKSND();
        }

        public void CLKSND()
        {
            Z80.A = Mem.LD_8(0x0E6C1);// PORT 40H DATA
            Z80.A &= 0b1111_1001;
            Z80.A |= Z80.C;
            PC88.OUT(0x40, Z80.A);
            Z80.A &= 0b1111_1001;
            Mem.stack.Push(Z80.BC);
            Z80.BC = Mem.stack.Pop();
            //   NOP
            Mem.LD_8(0xe6c1, Z80.A);
            PC88.OUT(0x40, Z80.A);
            // RET
        }

        // --	CULCRATE ALLSEC->TIME	--

        //IN:HL<=SEC DATA
        //EXIT:TIME WORK<=DATA
        public void CULTIM()
        {
            Z80.DE = 3600;

            DIV();

            Z80.A = Z80.L;
            Mem.LD_8(HOUR, Z80.A);
            Z80.EX_DE_HL();
            Z80.DE = 60;

            DIV();

            Z80.A = Z80.L;

            Mem.LD_8(MIN, Z80.A);
            Z80.A = Z80.E;
            Mem.LD_8(SEC, Z80.A);
            //RET
        }

        // --	CULCRATE TIME->HOUR*3600+MIN*60+SEC(ALLSEC)    --

        //IN:HL<=HOUR ADR
        // EXIT:HL

        public void CULSEC()
        {
            Z80.A = Mem.LD_8(Z80.HL);   // HOUR
            Z80.HL--;
            Mem.stack.Push(Z80.HL);
            BCDHEX();

            if (Z80.A - 13 < 0)
            {
                goto CULS2;
            }
            Z80.A = (byte)(Z80.A - 12);

        CULS2:
            Z80.E = Z80.A;

            Z80.HL = 3600;

            MULT();

            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Mem.stack.Push(Z80.DE);
            Z80.A = Mem.LD_8(Z80.HL);   // MIN
            Z80.HL--;
            Mem.stack.Push(Z80.HL);
            BCDHEX();
            Z80.E = Z80.A;
            Z80.HL = 60;
            MULT();

            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.A = Mem.LD_8(Z80.HL);   // SEC
            BCDHEX();
            Z80.L = Z80.A;
            Z80.H = 0;
            Z80.HL += Z80.DE; //SEC+MIN*60
            Z80.DE = Mem.stack.Pop();

            Z80.HL += Z80.DE; //SEC+MIN*60+HOUR*3600
            //	RET
        }

        // --	CONVERT BCD CODE INTO HEX	--

        //IN:A / EXIT:A
        public void BCDHEX()
        {
            Mem.stack.Push(Z80.DE);
            Z80.E = Z80.A;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A += Z80.A;
            Z80.D = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.D;// *10
            Z80.D = Z80.A;
            Z80.A = Z80.E;
            Z80.A &= 0xf;
            Z80.A += Z80.D;
            Z80.DE = Mem.stack.Pop();
            //RET
        }

        // ***

        //IN:HL<=TEXT ADR:DE<= STRING DATA TOP ADR
        // (DE) ｶﾗ ﾊｼﾞﾏﾙ ﾓｼﾞﾃﾞｰﾀﾚﾂ ﾄ(HL)ｶﾗ ﾊｼﾞﾏﾙ ﾃｷｽﾄ ﾉ ﾃﾞｰﾀﾚﾂ ﾉ ﾋｶｸ
        //EXIT: CY=1ﾅﾗ FAULT.Z=1ﾅﾗ ﾓｼﾞﾚﾂﾊｯｹﾝ
        public void MCMP()
        {
        MCMP1:
            Z80.A = Mem.LD_8(Z80.DE);
            Z80.A |= Z80.A;
            Z80.Zero = (Z80.A == 0);
            Z80.Carry = false;
            if (Z80.Zero)
            {
                return;
            }
            Z80.C = Mem.LD_8(Z80.HL);
            Z80.Zero = (Z80.A - Z80.C == 0);
            if (!Z80.Zero)
            {
                goto MCMP4;
            }
            Z80.DE++;
            Z80.HL++;
            goto MCMP1;
        MCMP4:
            Z80.Carry = true;
            //    RET
        }

        public void MCMP_DE(string strDE)
        {
            Z80.Zero = false;
            Z80.Carry = true;
            try
            {
                string trgDE = strDE.Substring(0, strDE.IndexOf("\0"));
                if (trgDE.Length < 1) return;

                byte[] bHL = new byte[trgDE.Length];
                for (int i = 0; i < trgDE.Length; i++)
                {
                    bHL[i] = Mem.LD_8(Z80.HL++);
                }
                string trgHL = Encoding.UTF8.GetString(bHL);
                if (trgHL == trgDE)
                {
                    Z80.Zero = true;
                    Z80.Carry = false;
                }
            }
            catch { }
        }

        // COMMAND

        public byte[] FCOMS = new byte[]{// COMMANDs
         0x6c // 'l'	LIZM
        ,0x6f //,'o'	OCTAVE
        ,0x44 //,'D'	DETUNE
        ,0x76 //,'v'	VOLUME
        ,0x40 //,'@'	SOUND COLOR
        ,0x3e //,'>'	OCTAVE UP
        ,0x3c //,'<'	OCTAVE DOWN
        ,0x29 //,')'	VOLUME UP
        ,0x28 //,'('	VOLUME DOWN
        ,0x26 //,'&'	TIE
        ,0x79 //,'y'	REGISTER WRITE
        ,0x4d //,'M'	MODURATION(LFO)
        ,0x72 //,'r'	REST
        ,0x5b //,'['	LOOP START
        ,0x5d //,']'	LOOP END
        ,0x53 //,'S'	SE DETUNE
        ,0x4c //,'L'	JUMP RESTART ADR
        ,0x71 //,'q'	COMMAND OF 'q'
        ,0x45 //,'E'	SOFT ENV
        ,0x50 //,'P'	MIX PORT
        ,0x77 //,'w'	NOIZE WAVE
        ,0x74 //,'t'	TEMPO(DIRECT CLOCK)
        ,0x43 //,'C'	SET CLOCK
        ,0x21 //,'!'	COMPILE END
        ,0x4b //,'K'	KEY SHIFT
        ,0x2f //,'/'	REPEAT JUMP
        ,0x56 //,'V'	TOTAL VOLUME OFFSET
        ,0x5c //,'\'    BEFORE CODE
        ,0x73 //,'s'	HARD ENVE SET
        ,0x6d //,'m'	HARD ENVE PERIOD
        ,0x25 //,'%'	SET LIZM(DIRECT CLOCK)
        ,0x70 //,'p'	STEREO PAN
        ,0x48 //,'H'	HARD LFO
        ,0x54 //,'T'	TEMPO
        ,0x4a //,'J'	TAG SET & JUMP TO TAG
        ,0x3b //,';'	ﾁｭｳﾔｸ ﾖｳ
        ,0x52 //,'R'	ﾘﾊﾞｰﾌﾞ
        ,0x2a //,'*'	MACRO
        ,0x3a //,':'	RETURN
        ,0x5e //,'^'	&ﾄ ｵﾅｼﾞ
        ,0x7c //,'|'	ｼｮｳｾﾂ
        ,0x7d //,'}'	ﾏｸﾛｴﾝﾄﾞ
        ,0x7b //,'{'	ﾎﾟﾙﾀﾒﾝﾄｽﾀｰﾄ
        ,0x23 //,'#'	FLAG SET
        ,0
        };

        public string ERRORMSG = "ERROR MESSAGE :\0";

    }
}
