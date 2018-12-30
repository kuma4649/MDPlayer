using System;

namespace MDPlayer.Driver.MUCOM88.ver1_1
{
    public class adpcm
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;

        //	ADPCM TEST ROUTINE
        //
        //       YK-2 1988 / 1 / 30
        //

        //	ORG	0B000H

        public const ushort PORT13 = 0x0FFFD;
        public Action[] tblOpe;

        public adpcm()
        {
            tblOpe = new Action[] {
                PLYPCM,
                REC,
                RAM_W,
                RAM_R
            };

        }

        public void PLYPCM()
        {
            Z80.A = Mem.LD_8(0xc000); //ｵﾝﾃｲ
            ushort adr = PCMDAT[Z80.A << 1];
            Z80.L = Mem.LD_8(adr++);
            Z80.H = Mem.LD_8(adr);

            Z80.A = Mem.LD_8(0xc001); //ｵｸﾀｰﾌﾞ
            Z80.B = Z80.A;
            if (Z80.B == 1) goto PCMSB4;

            //PCMSB5:
            Z80.HL >>= Z80.B;

        PCMSB4:
            Mem.LD_16(DELT_N, Z80.HL);
            PLAY();
        }

        public ushort[] PCMDAT = new ushort[]{// C-B ﾏﾃﾞ ﾉ ｻｲｾｲ ｻﾝﾌﾟﾘﾝｸﾞ ﾚｰﾄ
            0x49BA,0x4E1C,0x52C1,0x57AD,
            0x5CE4,0x626A,0x6844,0x6E77,
            0x7509,0x7BFE,0x835E,0x8B2D
        };


        // ***	ADPCM REC	***
        //	; IN:	(STTADR)<=SAMPLING START ADR
        //	; 	(TIME)	<=SAMPLING TIME(by 0.1 SEC)

        public const ushort STTADR = 0x0E000;
        public const ushort ENDADR = 0x0E002;
        public const ushort TIME = 0x0E004;

        public void REC()
        {
            Z80.DE = 0x1008;// MASK BRDY
            PCMOUT();

            Z80.E = 0x80;
            PCMOUT(); // RESET ALL FLAG

            Z80.DE = 0x0068;
            PCMOUT(); // PCM ENABLE


            Z80.DE = 0x0100;
            PCMOUT(); // MEMOLY TYPE RAM

            Z80.DE = 0x0CFF;
            PCMOUT(); // LIMIT ADR = 0FFFFH
            Z80.D++;
            PCMOUT();

            Z80.HL = Mem.LD_16(STTADR);

            Z80.D = 2;
            Z80.E = Z80.L;
            PCMOUT();//START ADR

            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();


            Z80.A = Mem.LD_8(TIME);
            Z80.B = Z80.A;

            Z80.DE = 0x0DA;// 0.1 SEC ｱﾀﾘ ﾉ SAMPLING ADR
            Z80.HL = Mem.LD_16(STTADR);

            //REC2:
            do
            {
                Z80.HL += Z80.DE;
            } while (Z80.B-- != 0);

            Mem.LD_16(ENDADR, Z80.HL);

            Z80.D = 4;
            Z80.E = Z80.L;
            PCMOUT();//END ADR

            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();

            Z80.DE = 0x06FA;
            PCMOUT();
            Z80.DE = 0x0700;// 16khz SAMPLING
            PCMOUT();

            Z80.DE = 0x00E8;// PCM START
            PCMOUT();

            Z80.A = Mem.LD_8(PORT13 + 1);
            Z80.C = Z80.A;
            //PCMLP3:
            do
            {
                Z80.A = PC88.IN(Z80.C);
            } while ((Z80.A & 0x4) == 0);// ﾌﾞﾝｾｷ ｼｭｳﾘｮｳ ﾏﾃﾞ ﾀｲｷ

            Z80.D = 0;
            Z80.E = 0x68;
            PCMOUT();// ﾌﾞﾝｾｷ ｼｭｳﾘｮｳ

            Z80.E = 0;
            PCMOUT();
        }


        //***	ADPCM PLAY	***
        // IN:(STTADR)<=ｻｲｾｲ ｽﾀｰﾄ ｱﾄﾞﾚｽ
        //	   (ENDADR)  <=ｻｲｾｲ ｴﾝﾄﾞ ｱﾄﾞﾚｽ
        //	   (DELT_N)<=ｻｲｾｲ ﾚｰﾄ
        public void PLAY()
        {
            Z80.DE = 0x1008;
            PCMOUT();
            Z80.DE = 0x1080;
            PCMOUT();// INIT
            Z80.DE = 0x0021;
            PCMOUT();
            Z80.DE = 0x0020;// RESET
            PCMOUT();

            Z80.DE = 0x01C0;
            PCMOUT();// 1 bit TYPE, L&R OUT

            Z80.DE = 0x0CFF;
            PCMOUT();// LIMIT ADR = 0FFFFH
            Z80.D++;
            PCMOUT();

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

            Z80.DE = 0x0BF0;
            PCMOUT();// VOLUME

            Z80.DE = 0x00A0;
            PCMOUT();// START

        }


        // ***	RAM-WRITE***
        //IN:HL<=START ADR
        public void RAM_W()
        {
            //	DI
            Z80.A = Mem.LD_8(0xE6C2);
            Z80.A |= 2;
            PC88.OUT(0x31, Z80.A);

            Z80.DE = 0x1000;
            PCMOUT();
            Z80.DE = 0x1080;
            PCMOUT();

            Z80.DE = 0x0060;// MEMOLY WRITE MODE
            PCMOUT();
            Z80.DE = 0x0100;
            PCMOUT();// RAM TYPE 1 bit

            // ---	SET START & END ADR	---

            Z80.DE = 0x0CFF;
            PCMOUT();// LIMIT ADR

            Z80.D++;
            PCMOUT();

            Z80.HL = Mem.LD_16(STTADR);
            Z80.D = 2;
            Z80.E = Z80.L;
            PCMOUT();

            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();// START ADR

            Z80.HL = Mem.LD_16(ENDADR);
            Z80.D = 4;
            Z80.E = Z80.L;
            PCMOUT();

            Z80.D++;
            Z80.E = Z80.H;
            PCMOUT();// END ADR

            // ---	ﾃﾝｿｳ ｶｲｼ	---

            Z80.A = Mem.LD_8(PORT13 + 1);
            Z80.C = Z80.A;
            //RAMW4:
            do
            {
                Z80.A = PC88.IN(Z80.C);
            } while ((Z80.A & 0x8) == 0);

            Z80.HL = 0x1000;

            //RAMW2:
            do
            {
                Z80.E = Mem.LD_8(Z80.HL);
                Z80.HL++;
                Z80.D = 0x8;
                PCMOUT();

                Z80.A = Mem.LD_8(PORT13 + 1);
                Z80.C = Z80.A;
                //RAMW3:
                do
                {
                    Z80.A = PC88.IN(Z80.C);
                } while ((Z80.A & 0x8) == 0);// BRDY CHECK
            } while ((Z80.A & 0x4) == 0);// EOS

            Z80.DE = 0x1;// RESET
            PCMOUT();

            Z80.E = 0x0;
            PCMOUT();

            Z80.A = Mem.LD_8(0x0E6C2);
            PC88.OUT(0x31, Z80.A);
            //   EI
        }


        // ***	RAM READ	***
        // IN:HL<=START ADR
        public void RAM_R()
        {
            //	DI
            Z80.A = Mem.LD_8(0x0E6C2);
            Z80.A |= 2;
            PC88.OUT(0x31, Z80.A);

            Z80.DE = 0x1000;
            PCMOUT();

            Z80.DE = 0x1080;
            PCMOUT();

            Z80.DE = 0x0020;// MEMOLY READ MODE
            PCMOUT();
            Z80.DE = 0x0100;
            PCMOUT();// RAM TYPE 1 bit

            // ---	SET START & END ADR	---
            Z80.DE = Mem.LD_16(0x0CFF);
            PCMOUT();// LIMIT ADR = 0FFFFH
            Z80.D++;
            PCMOUT();
            Z80.HL = Mem.LD_16(STTADR);
            Z80.D = 2;
            Z80.E = Z80.L;
            PCMOUT();

            Z80.D = 3;
            Z80.E = Z80.H;
            PCMOUT();// START ADR

            Z80.HL = Mem.LD_16(ENDADR);
            Z80.D = 4;
            Z80.E = Z80.L;
            PCMOUT();

            Z80.D = 5;
            Z80.E = Z80.H;
            PCMOUT();// END ADR

            RESUB();
            RESUB();// DUMMY READ(2ｶｲ)

            Z80.HL = Mem.LD_16(01000);
            Z80.BC = 0x6FFF;

            // ---	READ ｶｲｼ	---

            //RAMR2:
            do
            {
                RESUB();

                Mem.LD_8(Z80.HL, Z80.A);
                Z80.HL++;
                Z80.BC--;
                Z80.A = Z80.C;
                Z80.A |= Z80.B;
            } while (Z80.A != 0);

            Z80.DE = 1;// RESET
            PCMOUT();

            Z80.E = 0;
            PCMOUT();

            Z80.A = Mem.LD_8(0x0E6C2);

            PC88.OUT(0x31, Z80.A);
            //   EI
        }


        // ---	READ SUB	---
        public void RESUB()
        {
            PCMIN();

            ushort stBC = Z80.BC;
            ushort stAF = Z80.AF;

            Z80.A = Mem.LD_8(PORT13 + 1);
            Z80.C = Z80.A;

            //RAMR1:
            do
            {
                Z80.A = PC88.IN(Z80.C);
            } while ((Z80.A & 0x8) == 0);// BRDY CHECK

            Z80.AF = stAF;
            Z80.BC = stBC;
        }


        // ***	ADPCM IN	***
        // EXIT:	A<= $8(DAC) ﾉ ﾃﾞｰﾀ
        public void PCMIN()
        {
            ushort stBC = Z80.BC;

            Z80.A = Mem.LD_8(PORT13 + 1);
            Z80.C = Z80.A;

            //PCMI2:
            Z80.A = PC88.IN(Z80.C);
            //    JP M, PCMI2

            Z80.A = 8;
            PC88.OUT(Z80.C, Z80.A);

            //PCMI3:
            Z80.A = PC88.IN(Z80.C);
            //    JP M, PCMI3

            Z80.C++;
            Z80.A = PC88.IN(Z80.C);

            Z80.BC = stBC;
        }


        // ***	ADPCM OUT	***
        public void PCMOUT()
        {
            ushort stBC = Z80.BC;

            Z80.A = Mem.LD_8(PORT13 + 1);
            Z80.C = Z80.A;

            //PCMO2:
            Z80.A = PC88.IN(Z80.C);
            //    JP M, PCMO2

            PC88.OUT(Z80.C, Z80.D);
            //PCMO3:
            Z80.A = PC88.IN(Z80.C);
            //    JP M, PCMO3

            Z80.C++;
            PC88.OUT(Z80.C, Z80.E);

            Z80.BC = stBC;
        }


        // ***	ADPCM WORK	***
        public ushort DELT_N = 0;

    }
}
