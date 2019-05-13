using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88.ver1_0
{
    public class music2
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;


        public byte PLSET1_VAL = 0x38;
        public byte PLSET2_VAL = 0x3a;
        public byte LFOP6_VAL = 1;
        public byte FPORT_VAL = 0xa4;
        public ushort FMSUB8_VAL = 0;


        //   MUSIC DRIVE Ver 1.0
        //   FOR PC-8801MK2SR/FR/MR/TR
        //   1987/3~FH/MH/FA/MA & VA
        //   PROGRAMED BY YUZO KOSHIRO

        public const int VRTC = 0x0F302;
        public const int R_TIME = 0x0F304;
        public const int INT3 = 0x0F308;
        public const int S_ILVL = 0x0E6C3;
        public const int MAXCH = 11;

        public const int MUSNUM = 0xC200;// ﾃﾞｰﾀ ﾉ ｽﾀｰﾄ ｱﾄﾞﾚｽ
        public const int PCMADR = 0xE300;// ADPCMﾃﾞｰﾀﾃｰﾌﾞﾙ ﾉ ｽﾀｰﾄ ｱﾄﾞﾚｽ
        public const int EFCTBL = 0xAA00;// ｺｳｶｵﾝ ﾉ ｽﾀｰﾄ ｱﾄﾞﾚｽ

        public const int OTODAT = MUSNUM + 1;
        public const int MU_TOP = MUSNUM + 5;

        //    ORG 0B000H

        public Action[] COMTBL = null;

        public void initMusic2()
        {
            COMTBL = new Action[] {
                MSTART
                ,MSTOP
                ,FDO
                ,EFC
                ,RETW
            };

            SetFMCOMTable();
            SetLFOTBL();
            SetPSGCOM();
            SetSoundWork();

            loopCounter = new long[MAXCH];
            for (int i = 0; i < loopCounter.Length; i++) loopCounter[i] = -1;// ulong.MaxValue;
        }

        public byte FLGADR = 0;//#n

        // **	RETURN WORK ADR**

        //IN:	A<-ﾁｬﾝﾈﾙﾅﾝﾊﾞ(0-10)
        //EXIT:	IX
        public void RETW()
        {
            Z80.IX = CH1DAT;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                return;
            }
            Z80.DE = CH2DAT - CH1DAT;
        RETW2:
            Z80.IX += Z80.DE;
            Z80.A--;
            if (Z80.A == 0)
            {
                return;
            }
            goto RETW2;
        }

        // **	EFC FROM BASIC**

        //USR(n) :    n=0<->max255
        public void EFC()
        {
            if (Z80.A - 2 != 0)
            {
                return;
            }
            PC88.CALL(0x21A0);//FACC->BIN
            Z80.H = 1;//PRI
            EFECT();
            //    RET
        }

        // **	EFECT***

        //IN:	H<=ﾌﾟﾗｲｵﾘﾃｨ
        //	L<=EFC ﾅﾝﾊﾞ

        public void EFECT()
        {
            Z80.A = Mem.LD_8(PRISSG);
            Z80.Zero = (Z80.A - Z80.H == 0);
            Z80.Carry = (Z80.A - Z80.H < 0);
            if (Z80.Zero)
            {
                goto EFECT2;
            }
            if (!Z80.Carry)
            {
                return;
            }
        EFECT2:
            //	DI
            Z80.A = Z80.H;
            Mem.LD_8(PRISSG, Z80.A);
            Z80.A = Z80.L;
            Z80.A += Z80.A;
            Z80.L = Z80.A;
            Z80.H = 0;
            Z80.DE = EFCTBL;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.DE = 0;
            Z80.A = 1;
            Mem.LD_16(CH6DAT + 2, Z80.HL);
            Mem.LD_16(CH6DAT + 4, Z80.DE);
            Mem.LD_8(CH6DAT, Z80.A);
            Z80.IX = CH6DAT;
            Z80.HL = DMY;
            NOISE();
            //EI
            //    RET
        }


        // **	MUSIC START/STOP**

        //	IN: A<= MUSIC NUMBER(0->)

        public void MSTART()
        {
            Z80.A = 0;//!
            //	DI
            Mem.LD_8(MUSNUM, Z80.A);
            CHK();
            AKYOFF();
            SSGOFF();
            WORKINIT();

        //START:
            Mem.stack.Push(Z80.HL);
            INT57();
            ENBL();
            TO_NML();
            Z80.HL = Mem.stack.Pop();
            //    EI
            //    RET
        }

        public void MSTOP()
        {
            //	DI
            AKYOFF();
            SSGOFF();
            //Z80.A = Mem.LD_8(M_VECTR);
            Z80.A = M_VECTR;
            Z80.C = Z80.A;
            Z80.A = PC88.IN(Z80.C);
            Z80.A |= 0b1000_0000;
            PC88.OUT(Z80.C, Z80.A);
            //   EI
            //    RET
        }

        // **	FADEOUT**

        public void FDO()
        {
            Z80.A = 16;
            //Mem.LD_8(FDCO, Z80.A);
            FDCO[0] = Z80.A;
            //  RET
        }

        public void FDOUT()
        {
            //Z80.HL = FDCO + 1;
            Z80.HL = 1;
            //Z80.Zero = (Mem.LD_8(Z80.HL) - 1) == 0;
            Z80.Zero = (FDCO[Z80.HL] - 1) == 0;
            //Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) - 1));
            FDCO[Z80.HL]--;
            if (!Z80.Zero)
            {
                return;
            }
            //Mem.LD_8(Z80.HL, 16);
            FDCO[Z80.HL] = 16;
            //Z80.A = Mem.LD_8(FDCO);
            Z80.A = FDCO[0];
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                return;
            }
            Z80.A--;
            //Mem.LD_8(FDCO, Z80.A);
            FDCO[0] = Z80.A;
        //FDO2:
            Z80.A += 0x0F0;
            //Mem.LD_8(TOTALV, Z80.A);
            TOTALV = Z80.A;
            Z80.A ^= Z80.A;
            //Mem.LD_8(FMPORT, Z80.A);
            FMPORT = Z80.A;
            Z80.IX = CH1DAT;
            FDOFM();
            Z80.B = 3;
        //FDOSSG:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
                Z80.C = Z80.A;
                Z80.A &= 0b1111_0000;
                Z80.E = Z80.A;
                Z80.A = Z80.C;
                Z80.A &= 0b0000_1111;
                Z80.C = Z80.A;
                PV1();
                Z80.DE = WKLENG;
                Z80.IX += Z80.DE;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            DVOLSET();
            Z80.A = 4;
            //Mem.LD_8(FMPORT, Z80.A);
            FMPORT = Z80.A;
            Z80.DE = WKLENG;
            Z80.IX += Z80.DE;
            FDOFM();
            //Z80.A = Mem.LD_8(FDCO);
            Z80.A = FDCO[0];
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                FDO3();
                return;
            }
            //  RET
        }

        public void FDOFM()
        {
            Z80.B = 3;
        //FDL2:
            do
            {
                Mem.stack.Push(Z80.BC);
                STVOL();
                Z80.DE = WKLENG;
                Z80.IX += Z80.DE;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.B = 3;
            return;
        }

        public void FDO3()
        {
            MSTOP();
            Z80.A ^= Z80.A;
            //Mem.LD_8(TOTALV, Z80.A);
            TOTALV = Z80.A;
            //  RET
        }

        // **	ﾜﾘｺﾐ ﾉ ﾚﾍﾞﾙ ｿﾉﾀ ｼｮｷｾｯﾃｲ ｦ ｵｺﾅｳ**

        public void INT57()
        {
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.HL);
            Z80.A = 5;
            //	; LD(S.ILVL),A
            PC88.OUT(0xe4, Z80.A);//  CUT INT 5-7
            Z80.A = 3;
            PC88.OUT(0xe6, Z80.A);//  VRTC=ON;RTCLOCK=ON;USART=OFF
            Z80.A = Z80.I;
            Z80.H = Z80.A;
            Z80.L = 8;
            Z80.DE = 0;// PL_SND;
            Mem.LD_8(Z80.HL, Z80.E);
            Z80.HL++;
            Mem.LD_8(Z80.HL, Z80.D);
            TO_NML();
        //INT573:
            MONO();
            AKYOFF();// ALL KEY OFF
            SSGOFF();
            Z80.DE = 0x2983;// CH 4-6 ENABLE
            PSGOUT();
            Z80.DE = 0;
            Z80.B = 6;
        //INITF2:
            do
            {
                PSGOUT();
                Z80.D++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.D = 7;
            Z80.E = 0b0011_1000;
            PSGOUT();
            Z80.HL = INITPM;
            Z80.DE = PREGBF;
            Z80.BC = 9;
            Z80.LDIR();// PSGﾊﾞｯﾌｧ ｲﾆｼｬﾗｲｽﾞ
            Z80.HL = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            //    RET
        }

        // **	ﾐｭｰｼﾞｯｸ ﾜﾘｺﾐ ENABLE**

        public void ENBL()
        {
            Z80.A = Mem.LD_8(TIMER_B);
            Z80.E = Z80.A;
            STTMB();// SET Timer-B
            //Z80.A = Mem.LD_8(M_VECTR);
            Z80.A = M_VECTR;
            Z80.C = Z80.A;
            Z80.A = PC88.IN(Z80.C);
            Z80.A &= 0x7F;
            PC88.OUT(Z80.C, Z80.A);
            //RET
        }

        // **	ALL MONORAL / H.LFO OFF	***

        public void MONO()
        {
            Z80.D = 0x0B4;
            Z80.E = 0x0C0;
            Z80.A ^= Z80.A;
            FMPORT= Z80.A;
            Z80.B = 3;
        //MONO2:
            do
            {
                PSGOUT();
                Z80.D++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.D = 0x018;
            Z80.B = 6;
        //MONO3:
            do
            {
                PSGOUT();
                Z80.D++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.B = 3;
            Z80.D = 0x0B4;
            Z80.A = 4;
            //Mem.LD_8(FMPORT, Z80.A);
            FMPORT = Z80.A;
        //MONO4:
            do
            {
                PSGOUT();
                Z80.D++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A ^= Z80.A;
            //Mem.LD_8(FMPORT, Z80.A);
            FMPORT = Z80.A;
            Z80.DE = 0x2200;
            PSGOUT();
            Z80.DE = 0x1200;
            PSGOUT();
            Z80.HL = 0;// PALDAT;
            Z80.B = 7;
        //MONO5:
            do
            {
                //Mem.LD_8(Z80.HL, 0x0C0);
                PALDAT[Z80.HL] = 0xc0;
                Z80.HL++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = 3;
            //Mem.LD_8(PCMLR, Z80.A);
            PCMLR = Z80.A;
            //  RET
        }

        // **	MUSIC MAIN	**

        public void PL_SND()
        {
            //	DI
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.HL);
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.BC);
            Mem.stack.Push(Z80.IX);
            Mem.stack.Push(Z80.IY);
        //PLSET1:
            Z80.E = PLSET1_VAL;// 0x38;//  TIMER-OFF DATA
            Z80.D = 0x27;
            PSGOUT();//  TIMER-OFF
        //PLSET2:
            Z80.E = PLSET2_VAL;// 0x3a;
            PSGOUT();//  TIMER-ON
            DRIVE();
            FDOUT();
        //PLSND3:
            //	; LD A,(S.ILVL)
            Z80.A = 5;
            PC88.OUT(0x0E4, Z80.A);//CUT INT 5-7
            Z80.IY = Mem.stack.Pop();
            Z80.IX = Mem.stack.Pop();
            Z80.BC = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            //    EI
            //    RET
        }

        public long[] loopCounter = null;
        public int currentCh = 0;

        // **	CALL FM		**

        public void DRIVE()
        {
            Z80.A ^= Z80.A;
            //Mem.LD_8(FMPORT, Z80.A);
            FMPORT = Z80.A;
            Z80.IX = CH1DAT;
            currentCh = 0;
            FMENT();
            Z80.IX = CH2DAT;
            currentCh = 1;
            FMENT();
            Z80.IX = CH3DAT;
            currentCh = 2;
            FMENT();
            // **	CALL SSG	**
            Z80.A = 0xff;
            //Mem.LD_8(SSGF1, Z80.A);
            SSGF1 = Z80.A;
            Z80.IX = CH4DAT;
            currentCh = 3;
            SSGENT();
            Z80.IX = CH5DAT;
            currentCh = 4;
            SSGENT();
            Z80.IX = CH6DAT;
            currentCh = 5;
            SSGENT();
            Z80.A ^= Z80.A;
            //Mem.LD_8(SSGF1, Z80.A);
            SSGF1 = Z80.A;

            //Z80.A = Mem.LD_8(NOTSB2);//KUMA:SoundBoard2(OPNA)か
            Z80.A = NOTSB2;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                return;
            }

            //KUMA:Rhythm
            Z80.A++;
            //Mem.LD_8(DRMF1, Z80.A);
            DRMF1 = Z80.A;
            Z80.IX = DRAMDAT;
            currentCh = 9;
            FMENT();
            Z80.A ^= Z80.A;
            //Mem.LD_8(DRMF1, Z80.A);
            DRMF1 = Z80.A;

            //KUMA:FM Ch4-6
            Z80.A = 4;
            //Mem.LD_8(FMPORT, Z80.A);
            FMPORT = Z80.A;
            Z80.IX = CHADAT;
            currentCh = 6;
            FMENT();
            Z80.IX = CHBDAT;
            currentCh = 7;
            FMENT();
            Z80.IX = CHCDAT;
            currentCh = 8;
            FMENT();

            //KUMA:Adpcm
            Z80.A = 0xff;
            Mem.LD_8(PCMFLG, Z80.A);
            Z80.IX = PCMDAT;
            currentCh = 10;
            FMENT();
            Z80.A ^= Z80.A;
            Z80.Zero = false;
            Z80.Carry = false;
            Mem.LD_8(PCMFLG, Z80.A);
            //RET
        }

        public void SSGENT()
        {
            SSGSUB();
            PLLFO();
            //    RET
        }

        public void FMENT()
        {
            FMSUB();
            PLLFO();
            //    RET
        }

        //**	FM ｵﾝｹﾞﾝ ﾆ ﾀｲｽﾙ ｴﾝｿｳ ﾙｰﾁﾝ	**

        public void FMSUB()
        {
            Z80.A = Mem.LD_8(Z80.IX);
            Z80.A--;
            Mem.LD_8(Z80.IX, Z80.A);
            if (Z80.A == 0)
            {
                FMSUB1();
                return;
            }
            Z80.B = Mem.LD_8((ushort)(Z80.IX + 18));//  'q'
            Z80.Zero = (Z80.A - Z80.B == 0);
            Z80.Carry = (Z80.A - Z80.B < 0);
            if (Z80.Zero)
            {
                goto FMSUB0;
            }
            if (!Z80.Carry)
            {
                return;
            }
        FMSUB0:
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 3));
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 2));// HL=SOUND DATA ADD
            Z80.A = Mem.LD_8(Z80.HL);//  A=DATA
            if (Z80.A - 0x0FD == 0)// COUNT OVER ?
            {
                return;
            }
            //    BIT	5,(IX+33)
            if ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x20) != 0)//KUMA: 0x20(bit5)=REVERVE FLAG
            {
                FS2();
                return;
            }
            KEYOFF();
            return;
        }

        public void FS2()
        {
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));  //KUMA:IX+6 VOLUME DATA
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 17));  //KUMA:IX+17 SOFT ENVE DUMMY? なぜ？
            Z80.C = Z80.A;
            Z80.C >>= 1;
            STV2();
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x40));//  SET KEYOFF FLAG
            return;
        }
        // **	SET NEW SOUND**

        public void FMSUB1()
        { 
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x40));
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 3));
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 2));// HL=SOUND DATA ADD
            Z80.A = Mem.LD_8(Z80.HL);//  A=DATA
            if (Z80.A - 0x0FD != 0)// COUNT OVER?
            {
                FMSUBC();
                return;
            }
        //FMSUBE:
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xbf));// RES KEYOFF FLAG
            Z80.HL++;
            FMSUBC();
        }

        public void FMSUBC()
        { 
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.A |= Z80.A;// ﾃﾞｰﾀ ｼｭｳﾘｮｳ ｦ ｼﾗﾍﾞﾙ
            if (Z80.A != 0)
            {
                goto FMSUB2;//* 00H as end
            }
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x01));
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 5));
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 4));// HL=DATA TOP ADDRES
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                FMEND();//* DATA TOP ADRESS ｶﾞ 0000H ﾃﾞ BGM
                return; // ﾉ ｼｭｳﾘｮｳ ｦ ｹｯﾃｲ ｿﾚ ｲｶﾞｲﾊ ｸﾘｶｴｼ
            }
            loopCounter[currentCh]++;
            Z80.EX_DE_HL();
        //FMSUBB:
            Z80.A = Mem.LD_8(Z80.HL);// GET FLAG & LENGTH

        // **	SET LENGTH	**

        FMSUB2:
            Z80.HL++;
            if (Z80.A - 0xf0 >= 0)
            {
                FMSUBA();// DATA ｶﾞ ｺﾏﾝﾄﾞ ﾅﾗ FMSUBA ﾍ
                return;
            }
            Z80.RLCA();
            Z80.Carry = ((Z80.A & 0x01) != 0);
            Z80.A >>= 1; // GET CY = 7TH BIT(ｷｭｳﾌ ﾌﾗｸﾞ) : A=LENGTH
            Mem.LD_8(Z80.IX, Z80.A);// SET WAIT COUNTER
            if (!Z80.Carry)
            {
                goto FMSUB5;// ｵﾝﾌﾟ ﾅﾗ FMSUB5 ﾍ
            }

        // **	SET F-NUMBER**

        //FMSUB3:
            Mem.LD_8((ushort)(Z80.IX + 3), Z80.H);
            Mem.LD_8((ushort)(Z80.IX + 2), Z80.L);// SET NEXT SOUND DATA ADD
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x10) == 0);
            if (!Z80.Zero)
            {
                goto FS3;
            }
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x20) == 0);
            if (!Z80.Zero)
            {
                FS2();
                return;
            }
        FS3:
            KEYOFF();
            return;

        FMSUB5:
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                KEYOFF();
            }
            //Z80.A = Mem.LD_8((ushort)(PLSET1 + 1));
            Z80.A = PLSET1_VAL;
            if (Z80.A - 0x78 != 0)
            {
                FMSUB4();
                return;
            }
            //Z80.A = Mem.LD_8(FMPORT);
            Z80.A = FMPORT;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                FMSUB4();
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 8));
            if (Z80.A - 2 == 0)// CH=3?
            {
                EXMODE();
                return;
            }
            FMSUB4();
        }

        public void FMSUB4()
        {
            Z80.A = Mem.LD_8(Z80.HL);// A=BLOCK(OCTAVE-1 ) & KEY CODE DATA
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 3), Z80.H);
            Mem.LD_8((ushort)(Z80.IX + 2), Z80.L);// SET NEXT SOUND DATA ADD
            Z80.B = Z80.A;// STORE
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);// CHECK KEYOFF FLAG
            if (!Z80.Zero)
            {
                goto FMSUB9;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 32));// GET BEFORE CODE DATA
            Z80.A -= Z80.B;
            if (Z80.A != 0)
            {
                goto FMSUB9;
            }
            Z80.Carry = true;
            return;
        FMSUB9:
            Z80.A = Z80.B;
            Mem.LD_8((ushort)(Z80.IX + 32), Z80.A);
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto PCMGFQ;
            }
            //Z80.A = Mem.LD_8(DRMF1);
            Z80.A = DRMF1;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto FMGFQ;
            }
        //DRMFQ:
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (Z80.Zero)
            {
                return;
            }
            DKEYON();
            return;
        PCMGFQ:
            Z80.A = Z80.B;
            Z80.A &= 0b0000_1111;
            Z80.HL = PCMNMB;
            Z80.A += Z80.A;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);
            Z80.L = Z80.A;
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 9));
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 10));
            Z80.HL += Z80.DE;
            Z80.A = Z80.B;
            Z80.A &= 0b1111_0000;
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.B = Z80.A;
            Z80.B--;
            Z80.B++;
            if (Z80.B == 0)
            {
                goto ASUB72;
            }
        //ASUB7:
            do
            {
                //KUMA:やりたいこと Z80.HL >>= 1;
                Z80.Carry = (Z80.H & 0x01) != 0;
                Z80.H >>= 1;
                Z80.Carry = ((Z80.L & 0x01) != 0);
                Z80.L = (byte)((Z80.Carry ? 0x80 : 0x00) | (Z80.L >> 1));

                Z80.B--;
            } while (Z80.B != 0);
        ASUB72:
            Mem.LD_16(DELT_N, Z80.HL);
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                goto AS72;
            }
            LFORST();
        AS72:
            LFORST2();
            PLAY();
            return;
        FMGFQ:
            Z80.A = Z80.B;
            Z80.C = Z80.A;// STORE
            Z80.A &= 0x70;// GET BLOCK DATA
            Z80.A >>= 1;// A4-A6 ﾎﾟｰﾄ ｼｭﾂﾘｮｸﾖｳ ﾆ ｱﾜｾﾙ
            Z80.B = Z80.A;
            Z80.A = Z80.C;// RESTORE A
            Z80.A &= 0xf;// GET KEY CODE(C, C+, D...B)
            Z80.A += Z80.A;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL = FNUMB;
            Z80.HL += Z80.DE;
            Z80.C = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);// GET FNUM2
            Z80.A |= Z80.B;// A= KEY CODE & FNUM HI
            Z80.L = Z80.C;
            Z80.H = Z80.A;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 10));
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 9));// GET DETUNE DATA
            Z80.HL += Z80.DE;// DETUNE PLUS
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                goto FMS92;
            }
            Mem.LD_8((ushort)(Z80.IX + 29), Z80.L);// FOR LFO
            Mem.LD_8((ushort)(Z80.IX + 30), Z80.H);// FOR LFO
            //Mem.LD_16(FNUM, Z80.HL);
            FNUM = Z80.HL;
        FMS92:
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                LFORST();
            }
            LFORST2();
        //FMSUB8:
            Z80.BC = FMSUB8_VAL;// 0;
            FMSUB6();
        }

        public void FMSUB6()
        { 
            Z80.HL += Z80.BC;// BLOCK/FNUM1&2 DETUNE PLUS(for SE MODE)
            Z80.E = Z80.H;// BLOCK/F-NUMBER2 DATA
        //FPORT:
            Z80.A = FPORT_VAL;// 0x0A4;// PORT A4H
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.D = Z80.A;
            PSGOUT();
            Z80.A -= 4;
            Z80.D = Z80.A;
            Z80.E = Z80.L;// F-NUMBER1 DATA
        //FMSUB7:
            PSGOUT();
            KEYON();
            Z80.A &= Z80.A;
            Z80.Carry = false;
            //    RET
        }

        // **	SE MODE ﾉ DETUNE ｾｯﾃｲ**

        public void EXMODE()
        {
            Z80.BC = Mem.LD_16(DETDAT);
            Z80.B = 0;
            //Mem.LD_16((ushort)(FMSUB8 + 1), Z80.BC);
            FMSUB8_VAL = Z80.BC;
            FMSUB4();// SET OP1
            if (Z80.Carry)
            {
                return;
            }
            Z80.HL = DETDAT + 1;
            Z80.A = 0x0AA;//  A = CH3 F-NUM2 OP1 PORT - 2
        EXMLP:
            //Mem.LD_8((ushort)(FPORT + 1), Z80.A);
            FPORT_VAL = Z80.A;
            Z80.A++;
            Mem.stack.Push(Z80.AF);
            Z80.C = Mem.LD_8(Z80.HL);
            Z80.B = 0;
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
        //HLSTC0:
            //Z80.HL = Mem.LD_16(FNUM);
            Z80.HL = FNUM;
            FMSUB6();// SET OP2-OP4
            Z80.HL = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            if (Z80.A - 0xad != 0)//END PORT+1
            {
                goto EXMLP;
            }
            Z80.A = 0x0A4;
            //Mem.LD_8((ushort)(FPORT + 1), Z80.A);
            FPORT_VAL = Z80.A;
        //BRESET:
            Z80.BC = 0;
            //Mem.LD_16((ushort)(FMSUB8 + 1), Z80.BC);
            FMSUB8_VAL = Z80.BC;
            //  RET; RET TO MAIN
        }

        // **	KEY-OFF ROUTINE		**

        public void KEYOFF()
        {
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                PCMEND();
                return;
            }
            //Z80.A = Mem.LD_8(DRMF1);
            Z80.A = DRMF1;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto DKEYOF;
            }
            //Z80.A = Mem.LD_8(FMPORT);
            Z80.A = FMPORT;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.E = Z80.A;
            Z80.D = 0x28;//  PORT 28H
            PSGOUT();//  KEY-OFF
            return;

        // --	ﾘｽﾞﾑ ｵﾝｹﾞﾝ ﾉ ｷｰｵﾌ	--

        DKEYOF:
            Z80.D = 0x10;
            Z80.A = Mem.LD_8(RHYTHM);// GET RETHM PARAMETER
            Z80.A &= 0b0011_1111;
            Z80.A |= 0x80;
            Z80.E = Z80.A;
            PSGOUT();
            //   RET
        }

        // **	KEY-ON ROUTINE   **

        public void KEYON()
        {
            //Z80.A = Mem.LD_8(FMPORT);
            Z80.A = FMPORT;
            Z80.A |= Z80.A;
            Z80.Zero = (Z80.A == 0);
            Z80.A = 0xf0;
            if (Z80.Zero)
            {
                goto KEYON2;
            }
            Z80.A = 0xf4;
        KEYON2:
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.E = Z80.A;
            Z80.D = 0x28;
            PSGOUT();//KEY-ON
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x20) == 0);
            if (!Z80.Zero)
            {
                STVOL();
            }
            //    RET
        }

        // **   ﾘｽﾞﾑ ｵﾝｹﾞﾝ ﾉ ｷｰｵﾝ   **

        public void DKEYON()
        {
            Z80.D = 0x10;
            Z80.A = Mem.LD_8(RHYTHM);// GET RETHM PARAMETER
            Z80.A &= 0b0011_1111;
            Z80.E = Z80.A;// KEY ON
            PSGOUT();
            //    RET
        }

        // **	ALL KEY-OFF ROUTINE   **

        public void AKYOFF()
        {
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.BC);
            Mem.stack.Push(Z80.DE);
            Z80.E = 0;
            Z80.D = 0x28;
            Z80.B = 7;
        //AKYOF2:
            do
            {
                PSGOUT();
                Z80.E++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.DE = Mem.stack.Pop();
            Z80.BC = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            //    RET
        }

        // **	FM DATA OUT ROUTINE	**
        //
        // ENTRY: D<= REGISTER No.
        // E<= DATA

        public void PSGOUT()
        {
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.BC);
            Mem.stack.Push(Z80.HL);

            Z80.A = PORT13[0];
            Z80.C = Z80.A;
            Z80.A = Z80.D;
            if (Z80.A - 0x30 < 0)
            {
                goto PSGO4;
            }
            Z80.A = FMPORT;
            Z80.A &= Z80.A;
            if (Z80.A == 0)
            {
                goto PSGO4;
            }
            Z80.A = PORT13[1];
            Z80.C = Z80.A;
        PSGO4:
            PC88.OUT(Z80.C, Z80.D);
            Mem.stack.Push(Z80.BC); //KUMA: Wait?
            Z80.BC = Mem.stack.Pop();
            Z80.BC++;
            PC88.OUT(Z80.C, Z80.E);
        //PSGOE:
            Z80.HL = Mem.stack.Pop();
            Z80.BC = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            //    RET
        }

        // **	ｻﾌﾞ･ｺﾏﾝﾄﾞ ﾉ ｹｯﾃｲ**

        public void FMSUBA()
        {
            Z80.A &= 0xf;// A=COMMAND No.(0-F)
            Z80.DE = 0;// FMSUBC;
            Mem.stack.Push(Z80.DE);// STORE RETURN ADDRES
            Z80.DE = 0;// FMCOM;
            Z80.B = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.B;// A*3
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;// DE+A*3

            //KUMA:FMCOMテーブルのコマンドをコール
            //PUSH DE
            //RET
            //log.Write(string.Format("Z80.DE:{0}",Z80.DE));
            FMCOM[Z80.DE / 3]();
            FMSUBC();
        }

        // **	FM CONTROL COMMAND(s)   **
        public Action[] FMCOM = null;
        public Action[] FMCOM2 = null;

        public void SetFMCOMTable()
        {
            FMCOM = new Action[] {
            OTOPST // 0xF0 - ｵﾝｼｮｸ ｾｯﾄ    '@'
            ,VOLPST// 0xF1 - VOLUME SET   'v'
            ,FRQ_DF// 0xF2 - DETUNE(ｼｭｳﾊｽｳ ｽﾞﾗｼ) 'D'
            ,SETQ  // 0xF3 - SET COMMAND 'q'
            ,LFOON // 0xF4 - LFO SET
            ,REPSTF// 0xF5 - REPEAT START SET  '['
            ,REPENF// 0xF6 - REPEAT END SET    ']'
            ,MDSET // 0xF7 - FMｵﾝｹﾞﾝ ﾓｰﾄﾞｾｯﾄ
            ,STEREO// 0xF8 - STEREO MODE
            ,FLGSET// 0xF9 - FLAG SET
            ,W_REG // 0xFA - COMMAND OF   'y'
            ,VOLUPF// 0xFB - VOLUME UP    ')'
            ,HLFOON// 0xFC - HARD LFO
            ,TIE   // (CANT USE)
            ,RSKIP // 0xFE - REPEAT JUMP'/'
            ,SECPRC// 0xFF - to second com
            };

            FMCOM2 = new Action[] {
             PVMCHG // 0xFF 0xF0 - PCM VOLUME MODE
            ,HRDENV	// 0xFF 0xF1 - HARD ENVE SET 's'
            ,ENVPOD // 0xFF 0xF2 - HARD ENVE PERIOD
            ,REVERVE// 0xFF 0xF3 - ﾘﾊﾞｰﾌﾞ
            ,REVMOD	// 0xFF 0xF4 - ﾘﾊﾞｰﾌﾞﾓｰﾄﾞ
            ,REVSW	// 0xFF 0xF5 - ﾘﾊﾞｰﾌﾞ ｽｲｯﾁ
            ,NTMEAN
            ,NTMEAN
            };
        }

        public void SECPRC()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A &= 0xf;// A=COMMAND No.(0-F)
            Z80.DE = 0;// FMCOM2;
            Z80.B = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.B;
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;

            //KUMA:FMCOM2テーブルのコマンドをコール
            //PUSH DE
            //NTMEAN:
            //	RET
            FMCOM2[Z80.DE / 3]();
        }

        public void NTMEAN() { }

        public void TIE()
        {
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xbf));
            //	RET
        }

        // **	ﾌﾗｸﾞｾｯﾄ**

        public void FLGSET()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            //Mem.LD_8(FLGADR, Z80.A);
            FLGADR = Z80.A;
            //  RET
        }

        // **	ﾘﾊﾞｰﾌﾞ**

        public void REVERVE()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 17), Z80.A);
        //RV1:
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) | 0x20));
            //        RET
        }

        public void REVSW()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                //goto RV1;
                Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) | 0x20));
                return;
            }
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) & 0xdf));
            STVOL();
            //    RET
        }

        public void REVMOD()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto RM2;
            }
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) | 0x10));
            return;
        RM2:
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) & 0xef));
            //	RET
        }

        // **	PCM VMODE CHANGE**

        public void PVMCHG()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            //Mem.LD_8(PVMODE, Z80.A);
            PVMODE= Z80.A;
            //  RET
        }

        // **	STEREO**

        public void STEREO()
        {
            //Z80.A = Mem.LD_8(DRMF1);
            Z80.A = DRMF1;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto STE2;
            }
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto STER2;
            }
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            //Mem.LD_8(PCMLR, Z80.A);
            PCMLR = Z80.A;
            return;
        STER2:
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.RRCA();
            Z80.RRCA();
            Z80.C = Z80.A;
            Z80.DE = 0;// PALDAT;
            //Z80.A = Mem.LD_8(FMPORT);
            Z80.A = FMPORT;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;
            //Z80.A = Mem.LD_8(Z80.DE);
            Z80.A = PALDAT[Z80.DE];
            Z80.A &= 0b0011_1111;
            Z80.A |= Z80.C;
            //Mem.LD_8(Z80.DE, Z80.A);
            PALDAT[Z80.DE] = Z80.A;
            Z80.E = Z80.A;
            Z80.A = 0x0B4;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.D = Z80.A;
            PSGOUT();
            return;
        STE2:
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.C = Z80.A;
            Z80.A &= 0b0000_1111;
            Z80.B = Z80.A;
            Z80.DE = DRMVOL;
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;
            Z80.A = Mem.LD_8(Z80.DE);
            Mem.stack.Push(Z80.DE);
            Z80.A &= 0b0001_1111;
            Z80.E = Z80.A;
            Z80.A = Z80.C;
            Z80.RLCA();
            Z80.RLCA();
            Z80.A &= 0b1100_0000;
            Z80.A |= Z80.E;
            Z80.DE = Mem.stack.Pop();
            Mem.LD_8(Z80.DE, Z80.A);
            Z80.E = Z80.A;
            Z80.A = Z80.B;
            Z80.A += 0x18;
            Z80.D = Z80.A;
            PSGOUT();
            //  RET
        }

        // **	VOLUME UP & DOWN**

        public void VOLUPF()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 6));
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                return;
            }
            //Z80.A = Mem.LD_8(DRMF1);
            Z80.A = DRMF1;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                DVOLSET();
                return;
            }
            STVOL();
            //  RET
        }

        // **	SE DETUNE SET SUB ROUTINE**

        public void MDSET()
        {
            TO_EFC();
            Z80.DE = DETDAT;
            Z80.BC = 4;
            Z80.LDIR();
            //    RET
        }

        // **	HARD LFO SET**

        public void HLFOON()
        {
            Z80.A = Mem.LD_8(Z80.HL);// FREQ CONT
            Z80.HL++;
            Z80.A |= 0b0000_1000;
            Z80.E = Z80.A;
            Z80.D = 0x22;
            PSGOUT();
            Z80.C = Mem.LD_8(Z80.HL);// PMS
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);// AMS
            Z80.HL++;
            Z80.RLCA();
            Z80.RLCA();
            Z80.RLCA();
            Z80.RLCA();
            Z80.A |= Z80.C;
            Z80.C = Z80.A;// AMS+PMS
            //Z80.A = Mem.LD_8(FMPORT);
            Z80.A = FMPORT;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.DE = 0;// PALDAT;
            Z80.Carry = (Z80.A + Z80.E > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;
            //Z80.A = Mem.LD_8(Z80.DE);
            Z80.A = PALDAT[Z80.DE];
            Z80.A &= 0b1100_0000;
            Z80.A |= Z80.C;
            //Mem.LD_8(Z80.DE, Z80.A);
            PALDAT[Z80.DE] = Z80.A;
            Z80.E = Z80.A;
            Z80.A = 0xb4;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.D = Z80.A;
            PSGOUT();
            //    RET
        }

        // **	SOFT LFO SET(RESET) **

        public void LFOON()
        {
            Z80.A = Mem.LD_8(Z80.HL);// GET SUB COMMAND
            Z80.HL++;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                LFOON3();
                return;
            }
            SETDEL();
            SETCO();
            SETVCT();
            SETPEK();
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x80));// SET LFO FLAG
            //    RET
        }

        public void LFOON3()
        {
            Z80.A--;
            Z80.C = Z80.A;
            //Z80.A += Z80.A;
            //Z80.A += Z80.C;
            Z80.A *= 3;
            Z80.DE = 0;//LFOTBL;
            //Z80.Carry = (Z80.A + Z80.E > 0xff);
            //Z80.A += Z80.E;
            //Z80.E = Z80.A;
            //Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            //Z80.A -= Z80.E;
            //Z80.D = Z80.A;
            Z80.DE += Z80.A;
            //PUSH DE
            //RET
            LFOTBL[Z80.DE / 3]();
        }

        public Action[] LFOTBL = null;
        public void SetLFOTBL()
        {
            LFOTBL = new Action[]{
             LFOOFF
            , LFOON2
            , SETDEL
            , SETCO
            , SETVC2
            , SETPEK
            , TLLFO
            };
        }

        public void SETDEL()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 19), Z80.A);// SET DELAY
            Mem.LD_8((ushort)(Z80.IX + 20), Z80.A);
            //  RET
        }

        public void SETCO()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 21), Z80.A);// SET COUNTER
            Mem.LD_8((ushort)(Z80.IX + 22), Z80.A);
            //  RET
        }

        public void SETVCT()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 23), Z80.E);// SET ﾍﾝｶﾘｮｳ
            Mem.LD_8((ushort)(Z80.IX + 25), Z80.E);
            Mem.LD_8((ushort)(Z80.IX + 24), Z80.D);
            Mem.LD_8((ushort)(Z80.IX + 26), Z80.D);
            //RET
        }

        public void SETVC2()
        {
            SETVCT();
            LFORST();
        }

        public void SETPEK()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 27), Z80.A);//SET PEAK LEVEL
            Z80.A >>= 1;
            Mem.LD_8((ushort)(Z80.IX + 28), Z80.A);
            //RET
        }

        public void LFOON2()
        {
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x80));// LFOON
            //        RET
        }

        public void LFOOFF()
        {
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0x7f));// RESET LFO
            //    RET
        }

        public void TLLFO()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto TLL2;
            }
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) & 0xbf));
            return;

        TLL2:
            //Mem.LD_8((ushort)(LFOP6 + 1), Z80.A);
            LFOP6_VAL = Z80.A;
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)(Mem.LD_8((ushort)(Z80.IX + 33)) | 0x40));
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 29), Z80.A);
            Mem.LD_8((ushort)(Z80.IX + 30), 0);
            Mem.LD_8((ushort)(Z80.IX + 11), Z80.A);
            //  RET
        }

        // **	SET Q COMMAND**

        public void SETQ()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 18), Z80.A);
            //  RET
        }

        // **	ｵﾝｼｮｸ ｾｯﾄ ﾒｲﾝ**

        public void OTOPST()
        {
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                OTOPCM();
                return;
            }
            //Z80.A = Mem.LD_8(DRMF1);
            Z80.A = DRMF1;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                OTODRM();
                return;
            }
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 1), Z80.A);
            STENV();
            STVOL();
            //  RET
        }

        public void OTODRM()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8(RHYTHM, Z80.A);// SET RETHM PARA
            //  RET
        }

        public void OTOPCM()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Mem.LD_8(PCMNUM, Z80.A);
            Z80.A--;
            Mem.LD_8((ushort)(Z80.IX + 1), Z80.A);
            Z80.HL++;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Mem.stack.Push(Z80.HL);
            Z80.HL = PCMADR;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_16(STTADR, Z80.DE);
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_16(ENDADR, Z80.DE);
            Z80.HL++;
            Z80.HL++;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL = Mem.stack.Pop();
            //Z80.A = Mem.LD_8(PVMODE);
            Z80.A = PVMODE;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                return;
            }
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.E);
            //RET
        }

        // **	ﾎﾞﾘｭｰﾑ ｾｯﾄ	**

        public void VOLPST()
        {
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                PCMVOL();
                return;
            }
            //Z80.A = Mem.LD_8(DRMF1);
            Z80.A = DRMF1;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                VOLDRM();
                return;
            }
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            STVOL();
            //  RET
        }

        public void VOLDRM()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            DVOLSET();
        //VOLDR1:
            Z80.B = 6;
            Z80.DE = DRMVOL;
        //VOLDR2:
            do
            {
                Z80.C = Mem.LD_8(Z80.HL);
                Z80.A = Mem.LD_8(Z80.DE);
                Z80.A &= 0b1100_0000;
                Z80.A |= Z80.C;
                Mem.LD_8(Z80.DE, Z80.A);
                Mem.stack.Push(Z80.DE);
                Z80.E = Z80.A;
                Z80.A = Z80.B;
                Z80.A -= 6;
                Z80.A = (byte)-Z80.A;
                Z80.D = 0x18;
                Z80.A += Z80.D;
                Z80.D = Z80.A;
                PSGOUT();
                Z80.DE = Mem.stack.Pop();
                Z80.DE++;
                Z80.HL++;
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        public void PCMVOL()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            //Z80.A = Mem.LD_8(PVMODE);
            Z80.A = PVMODE;
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                PCMV2();
                return;
            }
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.E);
            //  RET
        }

        public void PCMV2()
        {
            Mem.LD_8((ushort)(Z80.IX + 7), Z80.E);
            //  RET
        }

        // --   SET TOTAL RHYTHM VOL	--

        public void DVOLSET()
        {
            Z80.D = 0x11;
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.A &= 0b0011_1111;
            Z80.E = Z80.A;
            //Z80.A = Mem.LD_8(TOTALV);
            Z80.A = TOTALV;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.E;
            if (Z80.A - 64 < 0)
            {
                goto DV2;
            }
            Z80.A ^= Z80.A;
        DV2:
            Z80.E = Z80.A;
            PSGOUT();
            //    RET
        }

        // **	ﾃﾞﾁｭｰﾝ ｾｯﾄ	**

        public void FRQ_DF()
        {
            Z80.A ^= Z80.A;
            Mem.LD_8((ushort)(Z80.IX + 32), Z80.A);// DETUNE ﾉ ﾊﾞｱｲﾊ BEFORE CODE ｦ CLEAR
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto FD2;
            }
            Mem.stack.Push(Z80.HL);
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 9));
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 10));
            Z80.HL += Z80.DE;
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
        FD2:
            Mem.LD_8((ushort)(Z80.IX + 9), Z80.E);
            Mem.LD_8((ushort)(Z80.IX + 10), Z80.D);
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                return;
            }
            Mem.stack.Push(Z80.HL);
            Z80.HL = Mem.LD_16(DELT_N);
            Z80.HL += Z80.DE;
            Z80.EX_DE_HL();
            Z80.C = Z80.D;
            Z80.D = 0x09;
            PCMOUT();
            Z80.D++;
            Z80.E = Z80.C;
            PCMOUT();
            Z80.HL = Mem.stack.Pop();
            //RET
        }

        // **	ﾘﾋﾟｰﾄ ｽｷｯﾌﾟ	**

        public void RSKIP()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;

            Mem.stack.Push(Z80.HL);
            Z80.HL--;
            Z80.HL--;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.A--;// LOOP ｶｳﾝﾀ = 1 ?
            if (Z80.A == 0)
            {
                RSKIP2();
                return;
            }
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void RSKIP2()
        {
            Z80.DE = 4;
            Z80.HL += Z80.DE;// HL = JUMP ADR
            Z80.EX_DE_HL();
            Z80.HL = Mem.stack.Pop();
            Z80.EX_DE_HL();
            //    RET
        }

        // **	ﾘﾋﾟｰﾄ ｽﾀｰﾄ ｾｯﾄ**

        public void REPSTF()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);//DE as REWRITE ADR OFFSET +1
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            Z80.HL--;
            Z80.HL--;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL--;
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.HL = Mem.stack.Pop();
            //  RET
        }

        // **	ﾘﾋﾟｰﾄ ｴﾝﾄﾞ ｾｯﾄ(FM) **

        public void REPENF()
        {
            Z80.Zero = ((Mem.LD_8(Z80.HL) - 1) == 0);// DEC REPEAT Co.
            Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) - 1));
            if (Z80.Zero)
            {
                REPENF2();
                return;
            }
            Z80.HL++;
            Z80.HL++;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL--;
            Z80.A &= Z80.A;
            Z80.HL -= Z80.DE;
            //    RET
        }

        public void REPENF2()
        {
            Z80.HL++;
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL--;
            Mem.LD_8(Z80.HL, Z80.A);
            Z80.DE = 4;
            Z80.HL += Z80.DE;
            //    RET
        }

        // **	ｵﾝｼｮｸ ｾｯﾄ ｻﾌﾞﾙｰﾁﾝ(FM)  **

        public void STENV()
        {
            Mem.stack.Push(Z80.HL);
            KEYOFF();
            Z80.A = 0x80;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.E = 0xf;
            Z80.B = 4;
        //ENVLP:
            do
            {
                Z80.D = Z80.A;
                PSGOUT();// ﾘﾘｰｽ(RR) ｶｯﾄ ﾉ ｼｮﾘ
                Z80.A += 4;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 1));// ﾜｰｸ ｶﾗ ｵﾝｼｮｸ ﾅﾝﾊﾞｰ ｦ ｴﾙ
        //STENV0:
            Z80.C = Z80.A;
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();
            Z80.RRCA();// *16
            Z80.H = Z80.A;
            Z80.A &= 0b1111_0000;
            Z80.L = Z80.A;
            Z80.A = Z80.H;
            Z80.A &= 0b0000_1111;
            Z80.H = Z80.A;// HL=*16
            Z80.A = Z80.C;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.C;// *9
            Z80.Carry = ((Z80.A + Z80.L) > 0xff);
            Z80.A += Z80.L;
            Z80.L = Z80.A;
            Z80.A += (byte)(Z80.H + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.L;
            Z80.H = Z80.A;// HL=*25
            Z80.EX_DE_HL();
            Z80.HL = Mem.LD_16(OTODAT);
            Z80.HL++;
            Z80.HL += Z80.DE;// HL ﾊ ｵﾝｼｮｸﾃﾞｰﾀ ｶｸﾉｳ ｱﾄﾞﾚｽ
            Z80.DE = MUSNUM;
            Z80.HL += Z80.DE;
        //STENV1:
            Z80.BC = 0x0406;// 4 OPERATER
            // 6 PARAMATER(Det/Mul, Total, KS/AR, DR, SR, SL/RR)
            Z80.A = 0x30;// START=PORT 30H
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));// PLUS CHANNEL No.
            Z80.D = Z80.A;
        STENV2:
            Mem.stack.Push(Z80.BC);
        //STENV3:
            do
            {
                Z80.E = Mem.LD_8(Z80.HL);// GET DATA
                PSGOUT();
                Z80.D++;
                Z80.D++;// SKIP BLANK PORT
                Z80.D++;//
                Z80.D++;//
                Z80.HL++;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.BC = Mem.stack.Pop();
            Z80.C--;
            if (Z80.C != 0)
            {
                goto STENV2;
            }
            Z80.A = Mem.LD_8(Z80.HL);// GET FEEDBACK/ALGORIZM
            Z80.E = Z80.A;
            Z80.A &= 0x07;// GET ALGORIZM
            Mem.LD_8((ushort)(Z80.IX + 7), Z80.A);// STORE ALGORIZM
            Z80.A = 0x0B0;// GET ALGO SET ADDRES
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));// CH PLUS
            Z80.D = Z80.A;
            PSGOUT();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        // **	ﾎﾞﾘｭｰﾑ**

        public void STVOL()
        {
            Mem.stack.Push(Z80.HL);
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.BC);
            STV1();
            Z80.BC = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void STV1()
        {
            Z80.C = Mem.LD_8((ushort)(Z80.IX + 6));// INPUT VOLUME
            //Z80.A = Mem.LD_8(TOTALV);
            Z80.A = TOTALV;
            Z80.A += Z80.C;
            if (Z80.A - 20 < 0)
            {
                goto STV12;
            }
            Z80.A ^= Z80.A;
        STV12:
            Z80.C = Z80.A;
            STV2();
        }

        public void STV2()
        { 
            Z80.B = 0;
            Z80.HL = 0;// FMVDAT;
            Z80.HL += Z80.BC;
            //Z80.E = Mem.LD_8(Z80.HL);// GET VOLUME DATA
            Z80.E = FMVDAT[Z80.HL];// GET VOLUME DATA
            Z80.A = 0x40;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));// GET PORT No.
            Z80.HL = 0;// CRYDAT;
            Z80.B = 0;
            Z80.C = Mem.LD_8((ushort)(Z80.IX + 7));// INPUT ALGOLIZM
            Z80.Carry = (Z80.HL + Z80.BC > 0xffff);
            Z80.HL += Z80.BC;
            //Z80.C = Mem.LD_8(Z80.HL);// C=ｷｬﾘｱ
            Z80.C = CRYDAT[Z80.HL];// C=ｷｬﾘｱ
            Z80.B = 4;// 4 OPERATER
        //STVOL2:
            do
            {
                bool bcarry = Z80.Carry;
                Z80.Carry = ((Z80.C & 0x1) != 0);
                Z80.C = (byte)((bcarry ? 0x80 : 0x00) | (Z80.C >> 1));
                Z80.D = Z80.A;
                if (Z80.Carry)
                {
                    PSGOUT();// ｷｬﾘｱ ﾅﾗ PSGOUT ﾍ
                }
                Z80.A += 4;
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        // **	Timer-B ｶｳﾝﾀ･ｾｯﾄ ﾙｰﾁﾝ   **
        // IN: E<= TIMER_B COUNTER

        public void STTMB()
        {
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.DE);
        //STTMB2:
            Z80.D = 0x26;
            PSGOUT();
            Z80.D = 0x27;
            Z80.E = 0x78;
            PSGOUT();//  Timer-B OFF
            Z80.E = 0x7a;
            PSGOUT();//  Timer-B ON
            Z80.A = 5;
            PC88.OUT(0xe4, Z80.A);
            Z80.DE = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            //   RET
        }

        // **	LFO ﾙｰﾁﾝ	**

        public void PLLFO()
        {
            // ---	FOR FM & SSG LFO	---
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x80) == 0);//  CHECK bit 7 ... LFO FLAG
            if (Z80.Zero)
            {
                return;
            }
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 2));
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 3));
            Z80.HL--;
            Z80.A = Mem.LD_8(Z80.HL);
            if (Z80.A - 0xf0 == 0)
            {
                return;//  ｲｾﾞﾝ ﾉ ﾃﾞｰﾀ ｶﾞ '&' ﾅﾗ RET
            }
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x20) == 0);// LFO CONTINE FLAG
            if (!Z80.Zero)
            {
                goto CTLFO;// bit 5 = 1 ﾅﾗ LFO ｹｲｿﾞｸ
            }
            // **	LFO INITIARIZE   **
            LFORST();
            LFORST2();
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 21));
            Mem.LD_8((ushort)(Z80.IX + 22), Z80.A);
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x20));// SET CONTINUE FLAG
        CTLFO:
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 20));//Delayのworkを取得
            Z80.A |= Z80.A;
            if (Z80.A == 0)//delayが完了していたら次の処理へ
            {
                CTLFO1();
                return;
            }
            Z80.A--;//delayのカウントダウン
            Mem.LD_8((ushort)(Z80.IX + 20), Z80.A);
            //  RET
        }

        public void CTLFO1()
        {
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 22)) - 1) == 0);
            Mem.LD_8((ushort)(Z80.IX + 22), (byte)(Mem.LD_8((ushort)(Z80.IX + 22)) - 1));// ｶｳﾝﾀ
            if (!Z80.Zero)
            {
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 21));
            Mem.LD_8((ushort)(Z80.IX + 22), Z80.A);//ｶｳﾝﾀ ｻｲ ｾｯﾃｲ
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 28));//  GET PEAK LEVEL COUNTER(P.L.C)
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                goto PLLFO1;// P.L.C > 0 ﾅﾗ PLLFO1
            }
            Z80.A &= Z80.A;
            Z80.HL = 0;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 26));
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 25));
            Z80.HL -= Z80.DE;
            Mem.LD_8((ushort)(Z80.IX + 26), Z80.H);
            Mem.LD_8((ushort)(Z80.IX + 25), Z80.L);// WAVE ﾊﾝﾃﾝ
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 27));
            Mem.LD_8((ushort)(Z80.IX + 28), Z80.A);//  P.L.C ｻｲ ｾｯﾃｲ
        PLLFO1:
            Mem.LD_8((ushort)(Z80.IX + 28), (byte)(Mem.LD_8((ushort)(Z80.IX + 28)) - 1));// P.L.C.-1
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 25));
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 26));
            PLS2();
            //    RET
        }

        public void PLS2()
        {
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                PLSKI2();
                return;
            }
            Z80.DE = Mem.LD_16(DELT_N);
            Z80.HL += Z80.DE;
            Mem.LD_16(DELT_N, Z80.HL);
            Z80.D = 0x09;
            Z80.E = Z80.L;
            PCMOUT();
            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();
            //  RET
        }

        public void PLSKI2()
        {
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 29));// GET FNUM1
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 30));// GET B/FNUM2
            Z80.HL += Z80.DE;//  HL= NEW F-NUMBER
            Mem.LD_8((ushort)(Z80.IX + 29), Z80.L);// SET NEW F-NUM1
            Mem.LD_8((ushort)(Z80.IX + 30), Z80.H);// SET NEW F-NUM2
            //Z80.A = Mem.LD_8(SSGF1);
            Z80.A = SSGF1;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                LFOP5();
                return;
            }
            // ---	FOR SSG LFO	---
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 32));// GET KEY CODE&OCTAVE
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A >>= 1;
            Z80.A |= Z80.A;//  OCTAVE=1?
            if (Z80.A == 0)
            {
                goto SSLFO2;
            }
            Z80.B = Z80.A;
        //SNUMGETL:
            do
            {
                Z80.HL >>= 1;
                Z80.B--;
            } while (Z80.B != 0);
        SSLFO2:
            Z80.E = Z80.L;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 8));
            PSGOUT();
            Z80.D++;
            Z80.E = Z80.H;
            PSGOUT();
            //    RET
        }

        // ---	FOR FM LFO	---

        public void LFOP5()
        {
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                LFOP6();
                return;
            }
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 8)) & 0x02) == 0);//  CH=3?
            if (Z80.Zero)
            {
                PLLFO2();// NOT CH3 THEN PLLFO2
                return;
            }
            //Z80.A = Mem.LD_8((ushort)(PLSET1 + 1));
            Z80.A = PLSET1_VAL;
            if (Z80.A - 0x78 != 0)
            {
                PLLFO2();// NOT SE MODE
                return;
            }
            Mem.LD_16(NEWFNM, Z80.HL);
        //LFOP4:
            Z80.HL = DETDAT;
            Z80.IY = OP_SEL;
            Z80.B = 4;
        //LFOP3:
            do
            {
                Mem.stack.Push(Z80.BC);
                Z80.DE = Mem.LD_16(NEWFNM);
                Z80.C = Mem.LD_8(Z80.HL);
                Z80.B = 0;
                Z80.HL++;
                Z80.EX_DE_HL();
                Z80.HL += Z80.BC;
                Mem.stack.Push(Z80.DE);
                Z80.E = Z80.H;
                Z80.D = Mem.LD_8(Z80.IY);
                Z80.IY++;
                PSGOUT();
                Z80.D--;
                Z80.D--;
                Z80.D--;
                Z80.D--;
                Z80.E = Z80.L;
                PSGOUT();
                Z80.DE = Mem.stack.Pop();
                Z80.EX_DE_HL();
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        public void PLLFO2()
        {
            Z80.E = Z80.H;
            Z80.A = 0xa4;//  PORT A4H
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.D = Z80.A;
            PSGOUT();
            Z80.A -= 4;
            Z80.E = Z80.L;// F-NUMBER1 DATA
            Z80.D = Z80.A;
            PSGOUT();
            //    RET
        }

        public void LFOP6()
        {
            Z80.C = LFOP6_VAL;
            Z80.A = 0x40;
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 8));
            Z80.E = Z80.L;
            Z80.Zero = ((Z80.C & 0x01) == 0);
            if (!Z80.Zero)
            {
                LFP62();
            }
            Z80.Zero = ((Z80.C & 0x04) == 0);
            if (!Z80.Zero)
            {
                LFP62();
            }
            Z80.Zero = ((Z80.C & 0x02) == 0);
            if (!Z80.Zero)
            {
                LFP62();
            }
            Z80.Zero = ((Z80.C & 0x08) == 0);
            if (Z80.Zero)
            {
                return;
            }
            LFP62();
        }

        public void LFP62()
        {
            Z80.D = Z80.A;
            PSGOUT();
            Z80.A += 4;
            //    RET
        }

        // ---	RESET PEAK L.&DELAY	---

        public void LFORST()
        {
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 19));
            Mem.LD_8((ushort)(Z80.IX + 20), Z80.A);// LFO DELAY ﾉ ｻｲｾｯﾃｲ
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xdf));// RESET LFO CONTINE FLAG
            //    RET
        }

        public void LFORST2()
        {
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 27));
            Z80.A >>= 1;
            Mem.LD_8((ushort)(Z80.IX + 28), Z80.A);// LFO PEAK LEVEL ｻｲ ｾｯﾃｲ
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 23));//
            Mem.LD_8((ushort)(Z80.IX + 25), Z80.A);// ﾍﾝｶﾘｮｳ ｻｲｾｯﾃｲ
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 24));
            Mem.LD_8((ushort)(Z80.IX + 26), Z80.A);
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x40) == 0);
            if (Z80.Zero)
            {
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 11));
            Mem.LD_8((ushort)(Z80.IX + 29), Z80.A);
            Mem.LD_8((ushort)(Z80.IX + 30), 0);
            //	RET
        }

        //SSG:
        // **	SSG ｵﾝｹﾞﾝｴﾝｿｳ ﾙｰﾁﾝ**

        public void SSGSUB()
        {
            Z80.A = Mem.LD_8(Z80.IX);
            Z80.A--;
            Mem.LD_8(Z80.IX, Z80.A);
            if (Z80.A == 0)
            {
                 SSSUB7();
                return;
            }
            Z80.B = Mem.LD_8((ushort)(Z80.IX + 18));//  'q'
            if (Z80.A - Z80.B != 0)
            {
                SSSUB0();
                return;
            }
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 3));
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 2));// HL=SOUND DATA ADD
            Z80.A = Mem.LD_8(Z80.HL);//  A=DATA
            if (Z80.A - 0xfd == 0)//COUNT OVER?
            {
                goto SSUB0;
            }
            SSSUBA();// TO REREASE
            return;//    RET
        SSUB0:
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xbf));//  SET TIE FLAG
            SSSUB0();
        }

        public void SSSUB0()
        {
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 6)) & 0x80) == 0);// ENVELOPE CHECK
            if (Z80.Zero)
            {
                return;
            }
            SOFENV();
            Z80.E = Z80.A;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 7));
            PSGOUT();
            return;
        }

        public void SSSUB7()
        {
            Z80.H = Mem.LD_8((ushort)(Z80.IX + 3));
            Z80.L = Mem.LD_8((ushort)(Z80.IX + 2));// HL AS SOUND DATA ADD
            Z80.A = Mem.LD_8(Z80.HL);//  A=DATA
            if (Z80.A - 0xfd != 0)//COUNT OVER?
            {
                goto SSSUBE;
            }
        //SSUB1:
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xbf));//  SET TIE FLAG
            Z80.HL++;
            SSSUBB();
            return;
        SSSUBE:
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x40));
            SSSUBB();
        }

        public void SSSUBB()
        { 
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.A |= Z80.A;// CHECK END MARK
            if (Z80.A != 0)
            {
                goto SSSUB2;
            }
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) | 0x01));
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 5));
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 4));// HL=DATA TOP ADD
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                SSGEND();
                return;
            }
            Z80.EX_DE_HL();
        //SSSUB1:
            Z80.A = Mem.LD_8(Z80.HL);// INPUT FLAG &LENGTH
        SSSUB2:
            Z80.HL++;
            if (Z80.A - 0xf0 >= 0)//COMMAND OF PSG?
            {
                SSSUB8();
                return;
            }
            Z80.RLCA();
            Z80.Carry = ((Z80.A & 1) != 0);
            Z80.A >>= 1;// CY=REST FLAG
            Mem.LD_8((ushort)(Z80.IX + 0), Z80.A);//  SET WAIT COUNTER
            if (!Z80.Carry)
            {
                goto SSSUB6;//  ｷｭｳﾌ ﾅﾗ SSSUBA
            }
            SSSUBA();
            SETPT();
            return;

        // **	SET FINE TUNE & COARSE TUNE	**

        SSSUB6:
            Z80.A = Mem.LD_8(Z80.HL);// LOAD OCT & KEY CODE
            Z80.HL++;
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                goto SSSKIP0;// NON TIE
            }
            Z80.C = Z80.A;
            Z80.B = Mem.LD_8((ushort)(Z80.IX + 32));
            Z80.A -= Z80.B;
            if (Z80.A == 0)
            {
                SETPT();// IF NOW CODE=BEFORE CODE THEN SETPT
                return;
            }
            Z80.A = Z80.C;
        SSSKIP0:
            Mem.LD_8((ushort)(Z80.IX + 32), Z80.A);// STORE KEY CODE & OCTAVE
            Mem.stack.Push(Z80.HL);
            Z80.B = Z80.A;
            Z80.A &= 0b0000_1111;//  GET KEY CODE
            Z80.A += Z80.A;
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL = SNUMB;
            Z80.HL += Z80.DE;
            Z80.A = Mem.LD_8(Z80.HL);// GET FNUM2
            Z80.HL++;
            Z80.H = Mem.LD_8(Z80.HL);// GET FNUM1
            Z80.L = Z80.A;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 10));
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 9));// GET DETUNE DATA
            Z80.HL += Z80.DE;//  DETUNE PLUS
            Mem.LD_8((ushort)(Z80.IX + 30), Z80.H);// SAVE FOR LFO
            Mem.LD_8((ushort)(Z80.IX + 29), Z80.L);
            Z80.B >>= 1;
            Z80.B >>= 1;
            Z80.B >>= 1;
            Z80.B >>= 1;
            Z80.B--;
            Z80.B++;//  OCTAVE=1?
            if (Z80.B == 0)
            {
                goto SSSUB4;//  1 ﾅﾗ SSSUB4 ﾍ
            }
        //SSSUB5:
            do
            {
                Z80.HL >>= 1;
                Z80.B--;
            } while (Z80.B != 0);// OCTAVE DATA ﾉ ｹｯﾃｲ
        SSSUB4:
            Z80.E = Z80.L;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 8));
            PSGOUT();
            Z80.E = Z80.H;
            Z80.D++;
            PSGOUT();
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                goto SSSUBF;
            }
            SOFENV();
            goto SSSUB9;
        SSSUBF:			// KEYON ｻﾚﾀﾄｷ ﾉ ｼｮﾘ
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x80) == 0);
            if (Z80.Zero)
            {
                goto SSSUBG;// NOT HARD ENV.
            }

            // ---	HARD ENV.KEY ON    ---

            Z80.E = 16;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 7));
            PSGOUT();// HARD ENV.KEYON
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 33));
            Z80.A &= 0b0000_1111;
            Z80.E = Z80.A;
            Z80.D = 0x0d;
            PSGOUT();
            goto SSSUBH;

        // ---	SOFT ENV.KEYON     ---

        SSSUBG:
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.A &= 0b0000_1111;
            Z80.A |= 0b1001_0000;//  TO STATE 1 (ATTACK)
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 12));//  ENVE INIT
            Mem.LD_8((ushort)(Z80.IX + 11), Z80.A);//KUMA:ALがcounterの初期値として使用される
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)(Mem.LD_8((ushort)(Z80.IX + 31)) & 0xdf));// RESET LFO CONTINE FLAG
            SOFEV7();
        SSSUBH:
            Z80.C = Mem.LD_8((ushort)(Z80.IX + 27));
            Z80.C >>= 1;
            Mem.LD_8((ushort)(Z80.IX + 28), Z80.C);//  LFO PEAK LEVEL ｻｲ ｾｯﾃｲ
            Z80.C = Mem.LD_8((ushort)(Z80.IX + 19));
            Mem.LD_8((ushort)(Z80.IX + 20), Z80.C);//  LFO DELAY ﾉ ｻｲｾｯﾃｲ
        SSSUB9:
            Z80.HL = Mem.stack.Pop();

            // **   VOLUME OUT PROCESS**

            //
            //  ENTRY A: VOLUME DATA
            //
            SSSUB3();
        }

        public void SSSUB3()
        {
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x80) == 0);
            if (!Z80.Zero)
            {
                SETPT();// IF HARD ENVE THEN SETPT
                return;
            }
            Z80.E = Z80.A;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 7));
            PSGOUT();
            SETPT();
        }
        // **   SET POINTER   **

        public void SETPT()
        { 
            Mem.LD_8((ushort)(Z80.IX + 3), Z80.H);
            Mem.LD_8((ushort)(Z80.IX + 2), Z80.L);//  SET NEXT SOUND DATA ADDRES
            return;
        }
        // **	KEY OFF ｼﾞ ﾉ RR ｼｮﾘ	**

        public void SSSUBA()
        {

            // --	HARD ENV.KEY OFF   --

            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x80) == 0);
            if (Z80.Zero)
            {
                goto SSUBAB;// NOT HARD ENV.
            }
            Z80.E = 0;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 7));
            PSGOUT();// HARD ENV.KEYOFF

        // --	SOFT ENV.KEY OFF   --

        SSUBAB:
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x20) == 0);
            if (Z80.Zero)
            {
                goto SSUBAC;
            }
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)((Mem.LD_8((ushort)(Z80.IX + 31)) & 0xbf)));
            SSSUB0();
            return;
        SSUBAC:
            Z80.A ^= Z80.A;
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 6)) & 0x80) == 0);
            if (Z80.Zero)
            {
                SSSUB3();// ﾘﾘｰｽ ｼﾞｬﾅｹﾚﾊﾞ SSSUB3
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.A &= 0b1000_1111;// STATE 4 (ﾘﾘｰｽ)
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            SOFEV9();
            SSSUB3();
        }

        // **   ｻﾌﾞ ｺﾏﾝﾄﾞ ﾉ ｹｯﾃｲ   **

        public void SSSUB8()
        {
            Z80.A &= 0xf;// A=COMMAND No.(0-F)
            Z80.DE = 0;// SSSUBB;
            Mem.stack.Push(Z80.DE);// STORE RETURN ADDRES
            Z80.DE = 0;// PSGCOM;
            Z80.B = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.B;// A*3
            Z80.Carry = ((Z80.A + Z80.E) > 0xff);
            Z80.A += Z80.E;
            Z80.E = Z80.A;
            Z80.A += (byte)(Z80.D + (Z80.Carry ? 1 : 0));
            Z80.A -= Z80.E;
            Z80.D = Z80.A;// DE+A*3

            //PUSH DE
            //RET
            PSGCOM[Z80.DE / 3]();
            SSSUBB();
        }

        // **   PSG COMMAND TABLE**
        public Action[] PSGCOM = null;
        public void SetPSGCOM()
        {
            PSGCOM = new Action[] {
                 OTOSSG// 0xF0 - ｵﾝｼｮｸ ｾｯﾄ         '@'
                ,PSGVOL// 0xF1 - VOLUME SET
                ,FRQ_DF// 0xF2 - DETUNE
                ,SETQ  // 0xF3 - COMMAND OF        'q'
                ,LFOON // 0xF4 - LFO
                ,REPSTF// 0xF5 - REPEAT START SET  '['
                ,REPENF// 0xF6 - REPEAT END SET    ']'
                ,NOISE // 0xF7 - MIX PORT          'P'
                ,NOISEW// 0xF8 - NOIZE PARAMATER   'w'
                ,FLGSET// 0xF9 - FLAG SET
                ,ENVPST// 0xFA - SOFT ENVELOPE     'E'
                ,VOLUPS// 0xFB - VOLUME UP    ')'
                ,NTMEAN// 0xFC -
                ,TIE   // 0x
                ,RSKIP // 0x
                ,SECPRC// 0xFF - to sec com
            };
        }

        // **	HARD ENVE SET**

        public void HRDENV()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = 0xd;
            PSGOUT();
            Z80.A = Z80.E;
            Z80.A |= 0b1000_0000;// SET H.E FLAG
            Mem.LD_8((ushort)(Z80.IX + 33), Z80.A);// H.E MODE
            Mem.LD_8((ushort)(Z80.IX + 6), 16);
            //	RET
        }

        // **	HARD ENVE PERIOD**

        public void ENVPOD()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = 0x0b;
            PSGOUT();
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D++;
            PSGOUT();
            //   RET
        }

        // **   WRITE REG   **

        public void W_REG()
        {
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            PSGOUT();
            //    RET
        }

        // **   MIX PORT CONTROL**

        public void NOISE()
        {
            Z80.C = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.B = Mem.LD_8((ushort)(Z80.IX + 8));// CH NO.
            Z80.A = Mem.LD_8((ushort)(PREGBF + 5));
            Z80.E = Z80.A;
            Z80.B >>= 1;
            Z80.B++;
            Z80.D = Z80.B;
            Z80.A = 0b0111_1011;
        //NOISE1:
            do
            {
                Z80.RLCA();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A &= Z80.E;
            Z80.E = Z80.A;
            Z80.A = Z80.C;
            Z80.B = Z80.D;
            Z80.RRCA();
        //NOISE2:
            do
            {
                Z80.RLCA();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A |= Z80.E;
            Z80.D = 7;
            Z80.E = Z80.A;
            PSGOUT();
            Z80.A = Z80.E;
            Mem.LD_8((ushort)(PREGBF + 5), Z80.A);
            //  RET
        }

        // **   ﾉｲｽﾞ ｼｭｳﾊｽｳ   **

        public void NOISEW()
        {
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = 6;
            PSGOUT();
            Z80.A = Z80.E;
            Mem.LD_8((ushort)(PREGBF + 4), Z80.A);
            //  RET
        }

        // **   ｴﾝﾍﾞﾛｰﾌﾟ ﾊﾟﾗﾒｰﾀ ｾｯﾄ**

        public void ENVPST()
        {
            Z80.EX_DE_HL();
            Mem.stack.Push(Z80.IX);
            Z80.HL = Mem.stack.Pop();
            Z80.BC = 12;
            Z80.HL += Z80.BC;
            Z80.EX_DE_HL();
            Z80.BC = 6;
            Z80.LDIR();
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.A |= 0b1001_0000;// ｴﾝﾍﾞﾌﾗｸﾞ ｱﾀｯｸﾌﾗｸﾞ ｾｯﾄ
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            //  RET
        }

        // **   PSG ｵﾝｼｮｸｾｯﾄ   **

        public void OTOSSG()
        {
            Z80.A = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            OTOCAL();
            ENVPST();
            Z80.HL = Mem.stack.Pop();
            //    RET
        }

        public void OTOCAL()
        {
            Z80.HL = SSGDAT;
            Z80.A += Z80.A;
            Z80.C = Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.C;//*6
            Z80.E = Z80.A;
            Z80.D = 0;
            Z80.HL += Z80.DE;
            //    RET
        }

        // **	SSG VOLUME UP & DOWN**

        public void VOLUPS()
        {
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x80) == 0);
            if (!Z80.Zero)
            {
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.E = Z80.A;
            Z80.A &= 0b0000_1111;
            Z80.A += Z80.D;
            if (Z80.A - 16 >= 0)
            {
                return;
            }
            Z80.D = Z80.A;
            Z80.A = Z80.E;
            Z80.A &= 0b1111_0000;
            Z80.A |= Z80.D;
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            //  RET
        }

        // **	PSG VOLUME	**

        public void PSGVOL()
        {
            Mem.LD_8((ushort)(Z80.IX + 33), (byte)((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x7f)));// RES HARD ENV FLAG
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));
            Z80.A &= 0b1111_0000;
            Z80.E = Z80.A;
            Z80.C = Mem.LD_8(Z80.HL);
            PV1();
        }

        public void PV1()
        { 
            //Z80.A = Mem.LD_8(TOTALV);
            Z80.A = TOTALV;
            Z80.A += Z80.C;
            if (Z80.A - 16 < 0)
            {
                goto PV2;
            }
            Z80.A ^= Z80.A;
        PV2:
            Z80.A |= Z80.E;
            Z80.HL++;
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);
            //  RET
        }

        // **	SSG ALL SOUND OFF	**

        public void SSGOFF()
        {
            Z80.B = 3;
            Z80.D = 8;
            Z80.E = 0;
        //SSGOF1:
            do
            {
                PSGOUT();
                Z80.D++;
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        // **   SSG KEY OFF**

        public void SKYOFF()
        {
            Z80.A ^= Z80.A;
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);// ENVE FLAG RESET
            Z80.E = Z80.A;
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 7));
            PSGOUT();
            //    RET
        }

        // **	SOFT ENVEROPE PROCESS**

        public void SOFENV()
        {
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 6)) & 0x10) == 0);// CHECK ATTACK FLAG
            if (Z80.Zero)
            {
                goto SOFEV2; //KUMA:decay flagのチェックへゴー
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 11));  //KUMA:get counter
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 13));  //KUMA:get AR
            Z80.Carry = ((Z80.A + Z80.D) > 0xff); //KUMA:counter + AR が255を超えたか？
            Z80.A += Z80.D;
            if (!Z80.Carry)
            {
                goto SOFEV1;
            }
            Z80.A = 0xff; //KUMA:counterが上限を突破したので,counterを255に修正
        SOFEV1: //KUMA:counterとflagの更新
            Z80.Zero = ((Z80.A - 0xff) == 0);
            Mem.LD_8((ushort)(Z80.IX + 11), Z80.A); //KUMA: counter = counter + AR(毎クロック,AR分だけcounterが増える)
            if (!Z80.Zero)
            {
                SOFEV7(); //KUMA:counterが255に達していないならSOFEV7へ
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));//KUMA:current volume & flagsを取得
            Z80.A ^= 0b0011_0000;//KUMA:attack flag:off  decay flag:on をxorで実現(上手い)
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);// TO STATE 2 (DECAY) //KUMA:current volume & flagsを更新
            SOFEV7();
            return;
        SOFEV2:
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 6)) & 0x20) == 0);//KUMA: Check decay flag
            if (Z80.Zero)
            {
                goto SOFEV4;//KUMA:sustain flagのチェックへ
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 11));// KUMA:get counter
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 14));// GET DECAY //KUMA:get DR
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 15));// GET SUSTAIN //KUMA:get SR
            Z80.Carry = ((Z80.A - Z80.D) < 0); //KUMA:counter = counter - DR 結果、counterが0未満の場合はSOFEV8へ
            Z80.A -= Z80.D;
            if (Z80.Carry)
            {
                goto SOFEV8;
            }
            if (Z80.A - Z80.E >= 0)//KUMA:counter-SR は0以上の場合はSOFEV3へ
            {
                goto SOFEV3;
            }
        SOFEV8:
            Z80.A = Z80.E;//KUMA: counter = SR
        SOFEV3:
            Z80.Zero = ((Z80.A - Z80.E) == 0);
            Mem.LD_8((ushort)(Z80.IX + 11), Z80.A);//KUMA:counter=counter-DR(毎クロック,DR分だけcounterが減る)
            if (!Z80.Zero)
            {
                SOFEV7();//KUMA: counterがSRに到達していないならSOFEV7へ
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));//KUMA:current volume & flagsを取得
            Z80.A ^= 0b0110_0000;//KUMA:dcay flag:off  sustain flag:on
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);// TO STATE 3 (SUSTAIN) //KUMA:current volume & flagsを更新
            SOFEV7();
            return;
        SOFEV4:
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 6)) & 0x40) == 0);//KUMA: Check sustain flag
            if (Z80.Zero)
            {
                SOFEV9();//KUMA:release 処理へ
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 11));// KUMA:get counter
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 16));// GET SUSTAIN LEVEL// KUMA:get SL
            Z80.Carry = ((Z80.A - Z80.D) < 0);//KUMA:counter = counter - SL 結果、counterが0以上の場合はSOFEV5へ
            Z80.A -= Z80.D;
            if (!Z80.Carry)
            {
                goto SOFEV5;
            }
            Z80.A ^= Z80.A;//KUMA: counter=0
        SOFEV5:
            Z80.A |= Z80.A;
            Mem.LD_8((ushort)(Z80.IX + 11), Z80.A);//KUMA:counter=counter-SL(毎クロック,SL分だけcounterが減る)
            if (Z80.A != 0)
            {
                SOFEV7();
                return;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));//KUMA:current volume & flagsを取得
            Z80.A &= 0b1000_1111;//KUMA:エンベロープで使用した進捗に関わるフラグをリセット
            Mem.LD_8((ushort)(Z80.IX + 6), Z80.A);// END OF ENVE //KUMA:KEYON中にSLにきて更にcounterが0になったらエンベロープ処理は終了する
            SOFEV7();
        }

        public void SOFEV9()
        {
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 11));//KUMA:get counter
            Z80.D = Mem.LD_8((ushort)(Z80.IX + 17));// GET REREASE//KUMA:get RR
            Z80.Carry = ((Z80.A - Z80.D) < 0);//KUMA:RRでcounterを減算
            Z80.A -= Z80.D;
            if (!Z80.Carry)
            {
                goto SOFEVA;
            }
            Z80.A ^= Z80.A;
        SOFEVA:
            Mem.LD_8((ushort)(Z80.IX + 11), Z80.A);//KUMA:counterを更新
            SOFEV7();
        }
        // **	VOLUME CALCURATE	**

        public void SOFEV7()
        { 
            Mem.stack.Push(Z80.HL);
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 11));//KUMA:get counter
            Z80.D = 0;
            Z80.HL = 0;
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 6));// GET VOLUME
            Z80.A &= 0b0000_1111;
            Z80.A++;
            Z80.B = Z80.A;//繰り返す回数 VOLUME+1回
        //SOFEV6:
            do
            {
                Z80.HL += Z80.DE;
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A = Z80.H;//AにはVOLUME+1を最大値としたcounter/256の割合分の値が入る
            Z80.HL = Mem.stack.Pop();
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x40) == 0);
            if (!Z80.Zero)
            {
                return;
            }
            Z80.Zero = ((Mem.LD_8((ushort)(Z80.IX + 33)) & 0x20) == 0);
            if (Z80.Zero)
            {
                return;
            }
            Z80.A += Mem.LD_8((ushort)(Z80.IX + 17));
            Z80.Carry = ((Z80.A & 0x01) != 0);
            Z80.A >>= 1;
            //    RET
        }

        // **	ｴﾝｿｳ ｵﾜﾘ	**

        public void FMEND()
        {
            Mem.LD_8((ushort)(Z80.IX + 2), Z80.L);
            Mem.LD_8((ushort)(Z80.IX + 3), Z80.H);
            Z80.A = Mem.LD_8(PCMFLG);
            Z80.A |= Z80.A;
            if (Z80.A != 0)
            {
                PCMEND();
                return;
            }
            KEYOFF();
            //RET
        }

        public void PCMEND()
        {
            Z80.DE = 0x0B00;
            PCMOUT();
            Z80.DE = 0x0100;
            PCMOUT();
            Z80.DE = 0x0021;
            PCMOUT();
            //   RET
        }

        public void SSGEND()
        {
            Mem.LD_8((ushort)(Z80.IX + 2), Z80.L);
            Mem.LD_8((ushort)(Z80.IX + 3), Z80.H);
            SKYOFF();
            Mem.LD_8((ushort)(Z80.IX + 31), (byte)((Mem.LD_8((ushort)(Z80.IX + 31)) & 0x7f)));// RESET LFO FLAG
            //  RET
        }

        // **   VOLUME OR FADEOUT etc RESET**

        public void WORKINIT()
        {
            Z80.A ^= Z80.A;
            Mem.LD_8(C2NUM, Z80.A);
            Mem.LD_8(CHNUM, Z80.A);
            PVMODE= Z80.A;
            Z80.A = Mem.LD_8(MUSNUM);
            Z80.HL = MU_TOP;
        WI1:
            Z80.DE = MAXCH * 4;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto WI2;
            }
            Z80.HL++;
            Z80.HL += Z80.DE;
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL = MU_TOP;
            Z80.HL += Z80.DE;
            Z80.A--;
            goto WI1;
        WI2:
            Z80.A = Mem.LD_8(Z80.HL);
            Mem.LD_8(TIMER_B, Z80.A);
            Z80.HL++;
            Mem.LD_16(TB_TOP, Z80.HL);
            Z80.B = 6;
            Z80.IX = CH1DAT;
        //WI4:
            do
            {
                Mem.stack.Push(Z80.BC);
                FMINIT();
                Z80.DE = WKLENG;
                Z80.IX += Z80.DE;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            Z80.A ^= Z80.A;
            Mem.LD_8(CHNUM, Z80.A);
            Z80.IX = DRAMDAT;
            FMINIT();
            Z80.A ^= Z80.A;
            Mem.LD_8(CHNUM, Z80.A);
            Z80.B = 4;
            Z80.IX = CHADAT;
        //WI6:
            do
            {
                Mem.stack.Push(Z80.BC);
                FMINIT();
                Z80.DE = WKLENG;
                Z80.IX += Z80.DE;
                Z80.BC = Mem.stack.Pop();
                Z80.B--;
            } while (Z80.B != 0);
            //    RET
        }

        public void FMINIT()
        {
            Mem.stack.Push(Z80.IX);
            Z80.HL = Mem.stack.Pop();
            Z80.E = Z80.L;
            Z80.D = Z80.H;
            Z80.DE++;
            Mem.LD_8(Z80.HL, 0);
            Z80.BC = WKLENG - 1;
            Z80.LDIR();
            Mem.LD_8(Z80.IX, 1);
            Mem.LD_8((ushort)(Z80.IX + 6), 0);

            // ---	POINTER ﾉ ｻｲｾｯﾃｲ	---

            Z80.HL = Mem.LD_16(TB_TOP);// HL=TABLE TOP ADR(Ch)
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Mem.stack.Push(Z80.HL);
            Z80.HL = MU_TOP;
            Z80.HL += Z80.DE;
            Mem.LD_8((ushort)(Z80.IX + 2), Z80.L);
            Mem.LD_8((ushort)(Z80.IX + 3), Z80.H);
            Z80.HL = Mem.stack.Pop();
            Z80.E = Mem.LD_8(Z80.HL);
            Z80.HL++;
            Z80.D = Mem.LD_8(Z80.HL);
            Z80.A = Z80.E;
            Z80.A |= Z80.D;
            if (Z80.A == 0)
            {
                goto FMI2;
            }
            Z80.HL = MU_TOP;
            Z80.HL += Z80.DE;
            Mem.LD_8((ushort)(Z80.IX + 4), Z80.L);
            Mem.LD_8((ushort)(Z80.IX + 5), Z80.H);
        FMI2:
            Z80.HL = C2NUM;
            Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) + 1));
            Z80.HL = Mem.LD_16(TB_TOP);
            Z80.DE = 4;
            Z80.HL += Z80.DE;
            Mem.LD_16(TB_TOP, Z80.HL);
            Z80.A = Mem.LD_8(CHNUM);
            if (Z80.A - 3 >= 0)
            {
                SSINIT();
                return;
            }
            Mem.LD_8((ushort)(Z80.IX + 8), Z80.A);
            Z80.A++;
            Mem.LD_8(CHNUM, Z80.A);
            //RET
        }

        // ---   FOR SSG   ---

        public void SSINIT()
        {
            Z80.A += 5;
            Mem.LD_8((ushort)(Z80.IX + 7), Z80.A);
            Z80.A = Mem.LD_8(CHNUM);
            Z80.A -= 3;
            Z80.A += Z80.A;
            Mem.LD_8((ushort)(Z80.IX + 8), Z80.A);
            Z80.A = Mem.LD_8(CHNUM);
            Z80.A++;
            Mem.LD_8(CHNUM, Z80.A);
            return;
        }
        // **	CHANGE SE MODE**

        public void TO_NML()
        {
            Z80.A = 0x38;
            //Mem.LD_8((ushort)(PLSET1 + 1), Z80.A);
            PLSET1_VAL = Z80.A;
            Z80.A = 0x3a;
            TNML2();
        }

        public void TNML2()
        {
            //Mem.LD_8((ushort)(PLSET2 + 1), Z80.A);
            PLSET2_VAL = Z80.A;
            Z80.D = 0x27;
            Z80.E = Z80.A;
            PSGOUT();
            //  RET
        }

        public void TO_EFC()
        {
            Z80.A = 0x78;
            //Mem.LD_8((ushort)(PLSET1 + 1), Z80.A);
            PLSET1_VAL = Z80.A;
            Z80.A = 0x7a;
            TNML2();
        }

        // ***	ADPCM PLAY	***

        // IN:(STTADR)<=ｻｲｾｲ ｽﾀｰﾄ ｱﾄﾞﾚｽ
        //	   (ENDADR)  <=ｻｲｾｲ ｴﾝﾄﾞ ｱﾄﾞﾚｽ
        //	   (DELT_N)<=ｻｲｾｲ ﾚｰﾄ

        public void PLAY()
        {
            Mem.stack.Push(Z80.HL);
            Z80.DE = 0x0B00;
            PCMOUT();
            Z80.DE = 0x0100;
            PCMOUT();
            Z80.DE = 0x0021;
            PCMOUT();
            Z80.DE = 0x1008;
            PCMOUT();
            Z80.DE = 0x1080;
            PCMOUT();// INIT
            Z80.HL = Mem.LD_16(STTADR);
            Z80.D = 2;
            Z80.E = Z80.L;
            PCMOUT();// START ADR
            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();
            Z80.HL = Mem.LD_16(ENDADR);
            Z80.D = 4;
            Z80.E = Z80.L;
            PCMOUT();// END ADR
            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();
            Z80.D = 0x09;
            Z80.A = Mem.LD_8(DELT_N);// ｻｲｾｲ ﾚｰﾄ ｶｲ
            Z80.E = Z80.A;
            PCMOUT();
            Z80.D = 0x0A;
            Z80.A = Mem.LD_8((ushort)(DELT_N + 1));// ｻｲｾｲ ﾚｰﾄ ｼﾞｮｳｲ
            Z80.E = Z80.A;
            PCMOUT();
            Z80.DE = 0x00A0;
            PCMOUT();
            Z80.D = 0x0B;
            Z80.E = Mem.LD_8((ushort)(Z80.IX + 6));
            //Z80.A = Mem.LD_8(TOTALV);
            Z80.A = TOTALV;
            Z80.A += Z80.A;
            Z80.A += Z80.A;
            Z80.A += Z80.E;
            if (Z80.A - 250 < 0)
            {
                goto PL1;
            }
            Z80.A ^= Z80.A;
        PL1:
            Z80.E = Z80.A;
            //Z80.A = Mem.LD_8(PVMODE);
            Z80.A = PVMODE;
            Z80.A |= Z80.A;
            if (Z80.A == 0)
            {
                goto PL2;
            }
            Z80.A = Mem.LD_8((ushort)(Z80.IX + 7));
            Z80.A += Z80.E;
            Z80.E = Z80.A;
        PL2:
            PCMOUT();// VOLUME
            Z80.D = 0x01;
            //Z80.A = Mem.LD_8(PCMLR);
            Z80.A = PCMLR;
            Z80.RRCA();
            Z80.RRCA();
            Z80.A &= 0b1100_0000;
            Z80.E = Z80.A;
            PCMOUT();// 1 bit TYPE, L&R OUT
            Z80.A = Mem.LD_8(PCMNUM);
            //Mem.LD_8(P_OUT, Z80.A);// ｼﾝｺﾞｳﾀﾞｽ
            P_OUT = Z80.A;
            Z80.HL = Mem.stack.Pop();
            //  RET
        }

        // ***	ADPCM OUT	***

        public void PCMOUT()
        {
            Mem.stack.Push(Z80.BC);
            Z80.A = PORT13[1];
            Z80.C = Z80.A;
        PCMO2:
            Z80.A = PC88.IN(Z80.C);
            if ((Z80.A & 0x80) != 0)
            {
                goto PCMO2;
            }
            PC88.OUT(Z80.C, Z80.D);
        PCMO3:
            Z80.A = PC88.IN(Z80.C);
            if ((Z80.A & 0x80) != 0)
            {
                goto PCMO3;
            }
            Z80.C++;
            PC88.OUT(Z80.C, Z80.E);
            Z80.BC = Mem.stack.Pop();
            //   RET
        }

        // **	ﾜﾘｺﾐ ｾｯﾃｲ/ﾎﾞｰﾄﾞ ﾁｪｯｸ ｿﾉﾀ**

        public void CHK()
        {
            Z80.A ^= Z80.A;
            //Mem.LD_8(NOTSB2, Z80.A);
            NOTSB2 = Z80.A;
            Z80.HL = TYPE1;
            Z80.DE = M_VECTR;
            //Z80.LDI();
            //Z80.LDI();
            //Z80.LDI();
            M_VECTR = Mem.LD_8(TYPE1);
            PORT13[0] = Mem.LD_8(TYPE1 + 1);
            PORT13[1] = Mem.LD_8(TYPE1 + 2);
            Z80.C = 0x44;
            STT1();
            Z80.A--;
            if (Z80.A == 0) {
                goto STTE;
            }
            Z80.C = 0x0A8;
            STT1();
            Z80.A--;
            if (Z80.A == 0)
            {
                goto STT2;
            }
            //Mem.LD_8(NOTSB2, Z80.A);
            NOTSB2 = Z80.A;
            goto STTE;
        STT2:
            Z80.HL = TYPE2;
            Z80.DE = M_VECTR;
            //Z80.LDI();
            //Z80.LDI();
            //Z80.LDI();
            M_VECTR = Mem.LD_8(TYPE2);
            PORT13[0] = Mem.LD_8(TYPE2 + 1);
            PORT13[1] = Mem.LD_8(TYPE2 + 2);
            goto STTE;
        STTE:
            return;
        }
        // --	CHECK BORD TYPE	--

        public void STT1()
        {
            Z80.A = 0x0FF;
            PC88.OUT(Z80.C, Z80.A);
            Mem.stack.Push(Z80.BC);
            Z80.BC = Mem.stack.Pop();
            Z80.BC++;
            Z80.A = PC88.IN(Z80.C);
            //  RET
        }

        // **	MUSIC WORK	**

        public byte NOTSB2 = 0;//(INFADR)
        public byte PVMODE = 0;//PCMvolMODE
        public byte P_OUT = 0;
        public byte M_VECTR = 0x32;//32H OR AAH
        public byte[] PORT13 = { 0x44, 0x46 };//44H OR A8H
        public byte TOTALV = 0;
        public byte[] FDCO = new byte[] { 0, 0 };
        public byte SSGF1 = 0;// SSG 4-6CH PLAY FLAG
        public byte DRMF1 = 0;
        public byte KEYBUF = 0;
        public byte FMPORT = 0;
        public ushort FNUM = 0;

        // **	ﾎﾞﾘｭｰﾑ ﾃﾞｰﾀ   **

        public byte[] FMVDAT = new byte[]{// ﾎﾞﾘｭｰﾑ ﾃﾞｰﾀ(FM)
        0x36,0x33,0x30,0x2D,
        0x2A,0x28,0x25,0x22,//  0,  1,  2,  3
        0x20,0x1D,0x1A,0x18,//  4,  5,  6,  7
        0x15,0x12,0x10,0x0D,//  8,  9, 10, 11
        0x0a,0x08,0x05,0x02 // 12, 13, 14, 15
        };

        public byte[] CRYDAT = new byte[]{// ｷｬﾘｱ / ﾓｼﾞｭﾚｰﾀ ﾉ ﾃﾞｰﾀ
        0x08,
        0x08,// ｶｸ ﾋﾞｯﾄ ｶﾞ ｷｬﾘｱ/ﾓｼﾞｭﾚｰﾀ ｦ ｱﾗﾜｽ
        0x08,//
        0x08,// Bit=1 ｶﾞ ｷｬﾘｱ
        0x0C,//      0 ｶﾞ ﾓｼﾞｭﾚｰﾀ
        0x0E,//
        0x0E,// Bit0=OP 1 , Bit1=OP 2 ... etc
        0x0F
        };

        // **	PMS/AMS/LR DATA	**

        public byte[] PALDAT = new byte[] {
            0x0C0,
            0x0C0,
            0x0C0,
            0x0,// DUMMY
            0x0C0,
            0x0C0,
            0x0C0
        };

        public byte PCMLR = 0;

        // **	SOUND WORK(FM) **
        public const ushort SOUNDWORK = 0xb000;
        public const ushort CH1DAT = SOUNDWORK;
        //DB	1	        ; LENGTH ｶｳﾝﾀｰ      IX+ 0
        //DB	24	        ; ｵﾝｼｮｸ ﾅﾝﾊﾞｰ		1
        //DW	0	        ; DATA ADDRES WORK	2,3
        //DW	0	        ; DATA TOP ADDRES	4,5
        //DB	10	        ; VOLUME DATA		6
        //DB	0	        ; ｱﾙｺﾞﾘｽﾞﾑ No.      7
        //DB    0	        ; ﾁｬﾝﾈﾙ ﾅﾝﾊﾞｰ          	8
        //DW	0	        ; ﾃﾞﾁｭｰﾝ DATA		9,10
        //DB	0	        ; for TLLFO		11
        //DB	0	        ; for ﾘﾊﾞｰﾌﾞ		12
        //DS	5	        ; SOFT ENVE DUMMY	13-17
        //DB	0	        ; qｵﾝﾀｲｽﾞ		18
        //DB	0	        ; LFO DELAY		19
        //DB	0	        ; WORK			20
        //DB	0	        ; LFO COUNTER		21
        //DB	0	        ; WORK			22
        //DW	0	        ; LFO ﾍﾝｶﾘｮｳ 2BYTE	23,24
        //DW	0	        ; WORK			25,26
        //DB	0	        ; LFO PEAK LEVEL	27
        //DB	0	        ; WORK			28
        //DB	0	        ; FNUM1 DATA		29
        //DB	0	        ; B/FNUM2 DATA		30
        //DB	00000001B	; bit 7=LFO FLAG	31
        //			        ; bit	6=KEYOFF FLAG
        //                  ; 5=LFO CONTINUE FLAG
        //			        ; 4=TIE FLAG
        //                  ; 3=MUTE FLAG
        //                  ; 2=LFO 1SHOT FLAG
        //                  ;
        //			        ; 0=1LOOPEND FLAG
        //DB 	0           ; BEFORE CODE		32
        //DB	0	        ; bit	6=TL LFO FLAG     33
        //			        ; 5=REVERVE FLAG
        //                  ; 4=REVERVE MODE
        //DW	0	        ; ﾘﾀｰﾝｱﾄﾞﾚｽ	34,35
        //DB	0,0         ; 36,37 (ｱｷ)

        public const ushort CH2DAT = CH1DAT + 38;
        //DB	1
        //DB	24
        //DW	0000H
        //DW	0000H
        //DB	10
        //DB	0
        //DB	1
        //DW	0000H
        //DS	7
        //DB	0	;18
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DW	0
        //DW	0
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DB	00000010B
        //DW	0,0,0

        public const ushort CH3DAT = CH2DAT + 38;
        //DB	1
        //DB	24
        //DW	0000H
        //DW	0000H
        //DB	10
        //DB	0
        //DB	2
        //DW	0000H
        //DS	7
        //DB	0	;18
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DW	0
        //DW	0
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DB	00000011B
        //DW	0,0,0

        // **	WORK(SSG)  **

        public const ushort CH4DAT = CH3DAT + 38;
        //DB	1	        ; COUNTER WORK		0
        //DB	0	        ; ｵﾝｼｮｸ No.     1
        //DW    0000H       ; DATA ADRS WORK	2,3
        //DW	0000H	    ; DATA TOP ADRS		4,5
        //DB	8	        ; CURENT VOLUME(bit0-3) 6
        //			        ; bit 4 = attack flag
        //                  ; bit 5 = decay flag
        //                  ; bit 6 = sustain flag
        //                  ; bit 7 = soft envelope flag
        //DB	8   	    ; VOL.REG.No.       7
        //DB	0           ; CHANNEL No.           8
        //DW    0   	    ; FOR DETUNE		9,10
        //DB	0   	    ; SOFT ENVE COUNTER	11
        //DS	6	        ; SOFT ENVE		12-17    //KUMA:  12:AL 13:AR 14:DR 15:SR 16:SL 17:RR
        //DB	0   	    ; COUNTER OF 'q'	18
        //DB	0   	    ; LFO DELAY		19
        //DB	0   	    ; WORK			20
        //DB	0   	    ; LFO COUNTER		21
        //DB	0   	    ; WORK			22
        //DW	0   	    ; LFO ﾍﾝｶﾘｮｳ 2BYTE	23,24
        //DW	0   	    ; WORK			25,26
        //DB	0   	    ; LFO PEAK LEVEL	27
        //DB	0   	    ; WORK			28
        //DB	0   	    ; FNUM1 DATA		29
        //DB	0   	    ; B/FNUM2 DATA		30
        //DB	00000100B	; bit 7=LFO FLAG	31
        //		            ; bit	6=KEYOFF FLAG
        //                  ; bit	5=LFO CONTINUE FLAG
        //		            ; bit	4=TIE FLAG
        //                  ;	3=MUTE FLAG
        //                  ;	0=1LOOPEND FG
        //DB	0	        ; BEFORE CODE		32
        //DB    0           ; bit 7 = HARD ENVE FLAG 33
        //		            ; bit 0-3 = HARD ENVE TYPE
        //DW	0           ; 34
        //DB	0,0         ; 36,37

        public const ushort CH5DAT = CH4DAT + 38;
        //DB	1
        //DB	0
        //DW	0000H
        //DW	0000H
        //DB	8
        //DB	9
        //DB	2
        //DW	0
        //DB	0
        //DS	6
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DW	0
        //DW	0
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DB	00000101B
        //DW	0,0,0

        public const ushort CH6DAT = CH5DAT + 38;
        //DB	1
        //DB	0
        //DW	0000H
        //DW	0000H
        //DB	8
        //DB	10
        //DB	4
        //DW	0
        //DB	0
        //DS	6
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DW	0
        //DW	0
        //DB	0
        //DB	0
        //DB	0
        //DB	0
        //DB	00000110B
        //DW	0,0,0

        public const ushort DRAMDAT = CH6DAT + 38;
        //	DB	1,0     ;0,1
        //	DW	0,0     ;2,3,4,5
        //	DB	10,0,2  ;6,7,8
        //	DW	0       ;9,10
        //	DS	7       ;11
        //	DB	0	    ;18
        //	DS	19,0    ;19-38

        public const ushort CHADAT = DRAMDAT + 38;
        //	DB	1,0
        //	DW	0,0
        //	DB	10,0,2
        //	DW	0
        //	DS	7
        //	DB	0	;18
        //	DS	19,0

        public const ushort CHBDAT = CHADAT + 38;
        //	DB	1,0
        //	DW	0,0
        //	DB	10,0,2
        //	DW	0
        //	DS	7
        //	DB	0	;18
        //	DS	19,0

        public const ushort CHCDAT = CHBDAT + 38;
        //	DB	1,0
        //	DW	0,0
        //	DB	10,0,2
        //	DW	0
        //	DS	7
        //	DB	0	;18
        //	DS	19,0

        public const ushort PCMDAT = CHCDAT + 38;
        //	DB	1,0
        //	DW	0,0
        //	DB	10,0,2
        //	DW	0
        //	DS	7
        //	DB	0	;18
        //	DS	19,0

        public const ushort RHYTHM = PCMDAT + 38;
        //	DB	0

        public const ushort WKLENG = CH2DAT - CH1DAT;

        // **	PSG REGISTOR WORK**

        public const ushort PREGBF = RHYTHM + 1;
        //DB	0,0,0,0, 0,0,0,0, 0 ;0-8
        public const ushort INITPM = PREGBF + 9;
        //DB	0,0,0,0, 0,56,0,0, 0 ;0-8

        // **	SE MODE(MODE2) ﾉ ﾃﾞﾁｭｰﾝ ﾜｰｸ**

        public const ushort DETDAT = INITPM + 9;
        //DB	0	; OP1
        //DB	0	; OP2
        //DB	0	; 3
        //DB	0	; 4

        // **	DRAM VOLUME DATA**

        public const ushort DRMVOL = DETDAT + 4;
        //DB	0C0H	; BASS
        //DB	0C0H	; SNEA
        //DB	0C0H	; SYMB
        //DB	0C0H	; HI-HAT
        //DB	0C0H	; TAM
        //DB	0C0H	; RIM

        public const ushort NEWFNM = DRMVOL + 6;
        //DW	0

        public const ushort OP_SEL = NEWFNM + 2;
        //DB	0A6H,0ACH,0ADH,0AEH	; OP 4,3,1,2

        public const ushort CHNUM = OP_SEL + 4;//DB	0
        public const ushort C2NUM = CHNUM + 1;//DB	0
        public const ushort TB_TOP = C2NUM + 1;//DW	0
        public const ushort TIMER_B = TB_TOP + 2;//DB	0
        public const ushort PRISSG = TIMER_B + 1;//DB	0

        // ***	ADPCM WORK	***

        public const ushort DMY = PRISSG + 1;       //DB    8
        public const ushort STTADR = DMY + 1;       //DW    0
        public const ushort ENDADR = STTADR + 2;    //DW    0
        public const ushort DELT_N = ENDADR + 2;    //DW	0	; ｻｲｾｲ ﾚｰﾄ
        public const ushort PCMNUM = DELT_N + 2;    //DB	0
        public const ushort PCMFLG = PCMNUM + 1;    //DB	0
        public const ushort PRI = PCMFLG + 1;	    //DB	0
        public const ushort TYPE1 = PRI + 1; 	    //DB	032H,044H,046H
        public const ushort TYPE2 = TYPE1 + 3;	    //DB	0AAH,0A8H,0ACH
        public const ushort FNUMB = TYPE2 + 3;      //DW	026AH,028FH,02B6H,02DFH,030BH,0339H
        public const ushort SNUMB = FNUMB + 24;     //DW	0EE8H,0E12H,0D48H,0C89H,0BD5H,0B2BH
        public const ushort PCMNMB = SNUMB + 24;    //	    ; C-B ﾏﾃﾞ ﾉ ｻｲｾｲ ｻﾝﾌﾟﾘﾝｸﾞ ﾚｰﾄ
        public const ushort SSGDAT = PCMNMB + 24;

        public void SetSoundWork()
        {
            for (ushort i = SOUNDWORK; i < SSGDAT + 6 * 16; i++)
            {
                Mem.LD_8(i, 0);
            }
            Mem.LD_8(CH1DAT + 0, 1);
            Mem.LD_8(CH1DAT + 1, 24);
            Mem.LD_8(CH1DAT + 6, 10);
            Mem.LD_8(CH1DAT + 31, 0b0000_0001);

            Mem.LD_8(CH2DAT + 0, 1);
            Mem.LD_8(CH2DAT + 1, 24);
            Mem.LD_8(CH2DAT + 6, 10);
            Mem.LD_8(CH2DAT + 8, 1);
            Mem.LD_8(CH2DAT + 31, 0b0000_0010);

            Mem.LD_8(CH3DAT + 0, 1);
            Mem.LD_8(CH3DAT + 1, 24);
            Mem.LD_8(CH3DAT + 6, 10);
            Mem.LD_8(CH3DAT + 8, 2);
            Mem.LD_8(CH3DAT + 31, 0b0000_0011);

            Mem.LD_8(CH4DAT + 0, 1);
            Mem.LD_8(CH4DAT + 1, 0);
            Mem.LD_8(CH4DAT + 6, 8);
            Mem.LD_8(CH4DAT + 7, 8);
            Mem.LD_8(CH4DAT + 31, 0b0000_0100);

            Mem.LD_8(CH5DAT + 0, 1);
            Mem.LD_8(CH5DAT + 1, 0);
            Mem.LD_8(CH5DAT + 6, 8);
            Mem.LD_8(CH5DAT + 7, 9);
            Mem.LD_8(CH5DAT + 8, 2);
            Mem.LD_8(CH5DAT + 31, 0b0000_0101);

            Mem.LD_8(CH6DAT + 0, 1);
            Mem.LD_8(CH6DAT + 1, 0);
            Mem.LD_8(CH6DAT + 6, 8);
            Mem.LD_8(CH6DAT + 7, 10);
            Mem.LD_8(CH6DAT + 8, 4);
            Mem.LD_8(CH6DAT + 31, 0b0000_0110);

            Mem.LD_8(DRAMDAT + 0, 1);
            Mem.LD_8(DRAMDAT + 6, 10);
            Mem.LD_8(DRAMDAT + 8, 2);

            Mem.LD_8(CHADAT + 0, 1);
            Mem.LD_8(CHADAT + 6, 10);
            Mem.LD_8(CHADAT + 8, 2);

            Mem.LD_8(CHBDAT + 0, 1);
            Mem.LD_8(CHBDAT + 6, 10);
            Mem.LD_8(CHBDAT + 8, 2);

            Mem.LD_8(CHCDAT + 0, 1);
            Mem.LD_8(CHCDAT + 6, 10);
            Mem.LD_8(CHCDAT + 8, 2);

            Mem.LD_8(PCMDAT + 0, 1);
            Mem.LD_8(PCMDAT + 6, 10);
            Mem.LD_8(PCMDAT + 8, 2);

            Mem.LD_8(INITPM + 5, 56);

            Mem.LD_8(DRMVOL + 0, 0xc0);
            Mem.LD_8(DRMVOL + 1, 0xc0);
            Mem.LD_8(DRMVOL + 2, 0xc0);
            Mem.LD_8(DRMVOL + 3, 0xc0);
            Mem.LD_8(DRMVOL + 4, 0xc0);
            Mem.LD_8(DRMVOL + 5, 0xc0);

            Mem.LD_8(OP_SEL + 0, 0xa6);
            Mem.LD_8(OP_SEL + 1, 0xac);
            Mem.LD_8(OP_SEL + 2, 0xad);
            Mem.LD_8(OP_SEL + 3, 0xae);

            Mem.LD_8(DMY, 8);

            byte[] type1_2 = new byte[] {
                0x032,0x044,0x046,
                0x0AA,0x0A8,0x0AC
            };
            for (int i = 0; i < type1_2.Length; i++) Mem.LD_8((ushort)(TYPE1 + i), type1_2[i]);

            ushort[] f_s_pcm_numb = new ushort[] {
                0x026A    ,0x028F    ,0x02B6    ,0x02DF    ,0x030B,0x0339
                ,0x036A    ,0x039E    ,0x03D5    ,0x0410    ,0x044E,0x048F
                ,0x0EE8    ,0x0E12    ,0x0D48    ,0x0C89    ,0x0BD5,0x0B2B
                ,0x0A8A    ,0x09F3    ,0x0964    ,0x08DD    ,0x085E,0x07E6
                ,0x49BA+200,0x4E1C+200,0x52C1+200,0x57AD+200
                ,0x5CE4+200,0x626A+200,0x6844+200,0x6E77+200
                ,0x7509+200,0x7BFE+200,0x835E+200,0x8B2D+200
            };
            for (int i = 0; i < f_s_pcm_numb.Length; i++) Mem.LD_16((ushort)(FNUMB + i * 2), f_s_pcm_numb[i]);

            byte[] sd = new byte[]{
                255,255,255,255,0,255 // E
                ,255,255,255,200,0,10
                ,255,255,255,200,1,10
                ,255,255,255,190,0,10
                ,255,255,255,190,1,10
                ,255,255,255,170,0,10
                ,40,70,14,190,0,15
                ,120,030,255,255,0,10
                ,255,255,255,225,8,15
                ,255,255,255,1,255,255
                ,255,255,255,200,8,255
                ,255,255,255,220,20,8
                ,255,255,255,255,0,10
                ,255,255,255,255,0,10
                ,120,80,255,255,0,255
                ,255,255,255,220,0,255 // 6*16
            };
            for (int i = 0; i < sd.Length; i++) Mem.LD_8((ushort)(SSGDAT + i), sd[i]);

        }
    }
}
