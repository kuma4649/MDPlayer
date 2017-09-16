using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class km6502 : IDevice
    {
        //# include <assert.h>
        //# include "nes_cpu.h"

        //#include "km6502/km6502m.h"

        //-------------------------------- km6502m.h START

        //#define BUILD_M6502 1
        //#define BUILD_M65C02 0
        //#define BUILD_HUC6280 0

        //# include "km6502.h"

        //-------------------------------- km6502.h START


        //# ifndef KM6502_H_
        //#define KM6502_H_

        //# include "kmconfig.h"

        //-------------------------------- kmconfig.h START

        //# ifndef KMCONFIG_H_
        //#define KMCONFIG_H_

        /* general setting */
        //#if !defined(USE_USERPOINTER)
        //#define USE_USERPOINTER	1
        //#endif

        //#if !defined(USE_INLINEMMC)
        //#define USE_INLINEMMC	12	/* mmc page bits */
        //#endif

        //#if USE_INLINEMMC
        //#if defined(USE_CALLBACK)
        //#undef USE_CALLBACK
        //#endif
        //#define USE_CALLBACK	1
        //#else

        //#if !defined(USE_CALLBACK)
        //#define USE_CALLBACK	0
        //#endif

        //#endif

        //#if !defined(BUILD_FOR_SIZE) && !defined(BUILD_FOR_SPEED)
        //#define BUILD_FOR_SIZE	1
        //#define BUILD_FOR_SPEED	0
        //#endif

        //#if defined(_MSC_VER)
        //typedef unsigned int Uword;				/* (0-0xFFFF) */
        //typedef unsigned char Ubyte;			/* unsigned 8bit integer for table */
        //# ifndef Inline
        //#define Inline __inline
        //#endif
        //#define CCall __cdecl
        //#define FastCall __fastcall
        //#define RTO16(w) ((Uword)(w & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)(w & 0xFF))		/* Round to  8bit integer */
        //#elif defined(__BORLANDC__)
        //        typedef unsigned int Uword;             /* (0-0xFFFF) */
        //        typedef unsigned char Ubyte;            /* unsigned 8bit integer for table */
        //# ifndef Inline
        //#define Inline __inline
        //#endif
        //#define CCall __cdecl
        //#define FastCall
        //#define RTO16(w) ((Uword)(w & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)(w & 0xFF))		/* Round to  8bit integer */
        //#elif defined(__GNUC__)
        //        typedef unsigned int Uword;             /* (0-0xFFFF) */
        //        typedef unsigned char Ubyte;            /* unsigned 8bit integer for table */
        //# ifndef Inline
        //#define Inline __inline__
        //#endif
        //#define CCall
        //#define FastCall /* __attribute__((regparm(2))) */
        //#define RTO16(w) ((Uword)(w & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)(w & 0xFF))		/* Round to  8bit integer */
        //#else
        //        typedef unsigned int Uword;             /* (0-0xFFFF) */
        //        typedef unsigned char Ubyte;            /* unsigned 8bit integer for table */
        //# ifndef Inline
        //#define Inline
        //#endif
        //# ifndef CCall
        //#define CCall
        //#endif
        //# ifndef FastCall
        //#define FastCall
        //#endif
        //#define RTO16(w) ((Uword)(w & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)(w & 0xFF))		/* Round to  8bit integer */
        //#endif

        //#define Callback FastCall
        //# ifndef External
        //#define External extern
        //#endif
        //# ifndef USE_DIRECT_ZEROPAGE
        //#define USE_DIRECT_ZEROPAGE 0
        //#endif

        /* advanced setting */

        //#if !BUILD_FOR_SIZE && !BUILD_FOR_SPEED
        //#define USE_FL_TABLE	1				/* Use table(512bytes) for flag */
        //#define OpsubCall CCall					/* OP code sub */
        //#define MasubCall CCall					/* addressing sub */
        //#endif

        /* auto setting */

        //#if BUILD_FOR_SIZE
        //#define USE_FL_TABLE 1
        //#define OpsubCall FastCall
        //#define MasubCall FastCall
        //#define OpcodeCall Inline
        //#elif BUILD_FOR_SPEED
        //#define USE_FL_TABLE 1
        //#define OpsubCall Inline
        //#define MasubCall FastCall
        //#define OpcodeCall Inline
        //#else
        //#define OpcodeCall Inline
        //#endif

        //#endif	/* KMCONFIG_H_ */
        //-------------------------------- kmconfig.h END

        //# ifdef __cplusplus
        //        extern "C" {
        //#endif

        //#if USE_USERPOINTER
        public delegate UInt32 dlgReadHandler(km6502 user, UInt32 adr);
        //typedef UInt32(Callback* ReadHandler)(void* user, UInt32 adr);
        public delegate void dlgWriterHandler(km6502 user, UInt32 adr, UInt32 value);
        //typedef void (Callback* WriteHandler)(void* user, UInt32 adr, UInt32 value);
        //#else
        //typedef Uword(Callback* ReadHandler)(Uword adr);
        //        typedef void (Callback* WriteHandler)(Uword adr, Uword value);
        //#endif

        public class K6502_Context
        {
            public UInt32 A;            /* Accumulator */
            public UInt32 P;            /* Status register */
            public UInt32 X;            /* X register */
            public UInt32 Y;            /* Y register */
            public UInt32 S;            /* Stack pointer */
            public UInt32 PC;           /* Program Counter */

            public UInt32 iRequest;     /* interrupt request */
            public UInt32 iMask;        /* interrupt mask */
            public UInt32 clock;        /* (incremental)cycle counter */
            public UInt32 lastcode;
            public km6502 user;         /* pointer to user area */

            //#if ILLEGAL_OPCODES
            public UInt32 illegal;
            //#endif

            //#if USE_CALLBACK
            //	/* pointer to callback functions */
            //#if USE_INLINEMMC
            //	ReadHandler ReadByte[1 << (16 - USE_INLINEMMC)];
            //	WriteHandler WriteByte[1 << (16 - USE_INLINEMMC)];
            //#else
            public dlgReadHandler ReadByte;
            public dlgWriterHandler WriteByte;
            //#endif
            //#endif

            //#if USE_DIRECT_ZEROPAGE
            //	Ubyte *zeropage;	/* pointer to zero page */
            //#endif
        };

        public enum K6502_FLAGS
        {
            K6502_C_FLAG = 0x01,
            K6502_Z_FLAG = 0x02,
            K6502_I_FLAG = 0x04,
            K6502_D_FLAG = 0x08,
            K6502_B_FLAG = 0x10,
            K6502_R_FLAG = 0x20,
            K6502_V_FLAG = 0x40,
            K6502_N_FLAG = 0x80
        };

        public enum K6502_IRQ
        {
            K6502_INIT = 1,
            K6502_RESET = 2,
            K6502_NMI = 4,
            K6502_BRK = 8,
            K6502_INT = 16
        };

        //# ifdef STATIC_CONTEXT6502
        //        External void K6502_Exec(void);
        //#else
        //External void K6502_Exec(struct K6502_Context *pc);
        //#endif

        //#if !USE_CALLBACK
        //#if USE_USERPOINTER
        //External Uword K6502_ReadByte(void *user, Uword adr);
        //External void K6502_WriteByte(void *user, Uword adr, Uword value);
        //#else
        //External Uword K6502_ReadByte(Uword adr);
        //        External void K6502_WriteByte(Uword adr, Uword value);
        //#endif
        //#endif

        //# ifdef __cplusplus
        //    }
        //#endif
        //#endif

        //-------------------------------- km6502.h END

        public const Int32 C_FLAG = (Int32)K6502_FLAGS.K6502_C_FLAG;
        public const Int32 Z_FLAG = (Int32)K6502_FLAGS.K6502_Z_FLAG;
        public const Int32 I_FLAG = (Int32)K6502_FLAGS.K6502_I_FLAG;
        public const Int32 D_FLAG = (Int32)K6502_FLAGS.K6502_D_FLAG;
        public const Int32 B_FLAG = (Int32)K6502_FLAGS.K6502_B_FLAG;
        public const Int32 R_FLAG = (Int32)K6502_FLAGS.K6502_R_FLAG;
        public const Int32 V_FLAG = (Int32)K6502_FLAGS.K6502_V_FLAG;
        public const Int32 N_FLAG = (Int32)K6502_FLAGS.K6502_N_FLAG;
        public const Int32 T_FLAG = 0;

        private const Int32 BASE_OF_ZERO = 0x0000;

        private const Int32 VEC_RESET = 0xFFFC;
        private const Int32 VEC_NMI = 0xFFFA;
        private const Int32 VEC_INT = 0xFFFE;

        private const Int32 VEC_BRK = VEC_INT;

        private const Int32 IRQ_INIT = (Int32)K6502_IRQ.K6502_INIT;
        private const Int32 IRQ_RESET = (Int32)K6502_IRQ.K6502_RESET;
        private const Int32 IRQ_NMI = (Int32)K6502_IRQ.K6502_NMI;
        private const Int32 IRQ_BRK = (Int32)K6502_IRQ.K6502_BRK;
        private const Int32 IRQ_INT = (Int32)K6502_IRQ.K6502_INT;

        //# ifdef STATIC_CONTEXT6502
        //        extern struct K6502_Context STATIC_CONTEXT6502;
        //#define __THIS__	STATIC_CONTEXT6502
        //#define __CONTEXT	void
        //#define __CONTEXT_	/* none */
        //#define __THISP		/* none */
        //#define __THISP_	/* none */
        //#else
        //#define __THIS__	(*pc)
        //#define __CONTEXT	struct K6502_Context *pc
        //#define __CONTEXT_	struct K6502_Context *pc,
        //#define __THISP		pc
        //#define __THISP_	pc,
        //#endif

        //#define K_EXEC		K6502_Exec

        //#if USE_USERPOINTER
        //#define __THIS_USER_ __THIS__.user,
        //#else
        //#define __THIS_USER_
        //#endif

        //#if USE_CALLBACK
        //#if USE_INLINEMMC
        //static Uword Inline K_READ(__CONTEXT_ Uword adr)
        //{
        //	return __THIS__.ReadByte[adr >> USE_INLINEMMC](__THIS_USER_ adr);
        //}
        //static void Inline K_WRITE(__CONTEXT_ Uword adr, Uword value)
        //{
        //	__THIS__.WriteByte[adr >> USE_INLINEMMC](__THIS_USER_ adr, value);
        //}
        //#else
        static UInt32 K_READ(K6502_Context pc, UInt32 adr)
        {
            return pc.ReadByte(pc.user, adr);
        }
        static void K_WRITE(K6502_Context pc, UInt32 adr, UInt32 value)
        {
            pc.WriteByte(pc.user, adr, value);
        }
        //#endif
        //#else
        //static Uword Inline K_READ(__CONTEXT_ Uword adr)
        //        {
        //            return K6502_ReadByte(__THIS_USER_ adr);
        //        }
        //        static void Inline K_WRITE(__CONTEXT_ Uword adr, Uword value)
        //        {
        //            K6502_WriteByte(__THIS_USER_ adr, value);
        //        }
        //#endif
        //#ifndef K_READNP
        //#define K_READNP K_READ
        //#define K_WRITENP K_WRITE
        //#endif
        //#ifndef K_READZP
        //#if !USE_DIRECT_ZEROPAGE
        //#define K_READZP K_READ
        //#define K_WRITEZP K_WRITE
        //#else
        //static Uword Inline K_READZP(__CONTEXT_ Uword adr)
        //{
        //	return __THIS__.zeropage[adr];
        //}
        //static void Inline K_WRITEZP(__CONTEXT_ Uword adr, Uword value)
        //{
        //	__THIS__.zeropage[adr] = value;
        //}
        //#endif
        //#endif

        //# include "km6502ft.h"

        //-------------------------------- km6502ft.h START

        //#if USE_FL_TABLE

        public byte[] fl_table = new byte[0x200]{
0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,

0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,
0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,0x80,

0x03,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,

0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,0x81,
        };

        public Int32 FLAG_NZ(Int32 w)
        {

            return fl_table[w & 0xff];
        }

        public Int32 FLAG_NZC(Int32 w)
        {
            return (fl_table[w & 0x01ff]);
        }

        //#else
        //#define FLAG_NZ(w)	((w & N_FLAG) + (RTO8(w) ? 0 : Z_FLAG))
        //#define FLAG_NZC(w)	(FLAG_NZ(w) + ((w >> 8) & C_FLAG))
        //#endif

        //-------------------------------- km6502ft.h END

        //# include "km6502cd.h"

        //-------------------------------- km6502cd.h START

        public void KI_ADDCLOCK(K6502_Context pc, UInt32 cycle)
        {
            //#if BUILD_HUC6280
            //	if (__THIS__.lowClockMode)
            //	{
            //#if 0
            //		cycle += (cycle << 2);	/* x5 */
            //#else
            //		cycle += cycle + cycle;	/*    */
            //		cycle += cycle;			/* x6 */
            //#endif
            //	}
            //	__THIS__.clock += cycle;
            //#else
            pc.clock += cycle;
            //#endif
        }

        public UInt32 KI_READWORD(K6502_Context pc, UInt32 adr)
        {
            UInt32 ret = K_READ(pc, adr);
            return ret + (K_READ(pc, (UInt32)((UInt32)((adr + 1) & 0xffff))) << 8);
        }

        public UInt32 KI_READWORDZP(K6502_Context pc, UInt32 adr)
        {
            UInt32 ret = K_READ(pc, (UInt32)(BASE_OF_ZERO + adr));
            return ret + (K_READ(pc, (UInt32)(BASE_OF_ZERO + (UInt32)((adr + 1) & 0xff))) << 8);
        }

        public UInt32 KAI_IMM(K6502_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 1) & 0xffff);
            return ret;
        }

        public UInt32 KAI_IMM16(K6502_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 2) & 0xffff);
            return ret;
        }

        public UInt32 KAI_ABS(K6502_Context pc)
        {
            return KI_READWORD(pc, KAI_IMM16(pc));
        }

        public UInt32 KAI_ABSX(K6502_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.X) & 0xffff);
        }

        public UInt32 KAI_ABSY(K6502_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.Y) & 0xffff);
        }

        public UInt32 KAI_ZP(K6502_Context pc)
        {
            return K_READ(pc, KAI_IMM(pc));
        }

        public UInt32 KAI_ZPX(K6502_Context pc)
        {
            return (UInt32)((KAI_ZP(pc) + pc.X) & 0xff);
        }

        public UInt32 KAI_INDY(K6502_Context pc)
        {
            return (UInt32)((KI_READWORDZP(pc, KAI_ZP(pc)) + pc.Y) & 0xffff);
        }

        public UInt32 KA_IMM(K6502_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 1) & 0xffff);
            return ret;
        }

        public UInt32 KA_IMM16(K6502_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 2) & 0xffff);
            return ret;
        }

        public UInt32 KA_ABS(K6502_Context pc)
        {
            return KI_READWORD(pc, KAI_IMM16(pc));
        }

        public UInt32 KA_ABSX(K6502_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.X) & 0xffff);
        }

        public UInt32 KA_ABSY(K6502_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.Y) & 0xffff);
        }

        public UInt32 KA_ZP(K6502_Context pc)
        {
            return BASE_OF_ZERO + K_READ(pc, KAI_IMM(pc));
        }

        public UInt32 KA_ZPX(K6502_Context pc)
        {
            return BASE_OF_ZERO + (UInt32)((KAI_ZP(pc) + pc.X) & 0xff);
        }

        public UInt32 KA_ZPY(K6502_Context pc)
        {
            return BASE_OF_ZERO + (UInt32)((KAI_ZP(pc) + pc.Y) & 0xff);
        }

        public UInt32 KA_INDX(K6502_Context pc)
        {
            return KI_READWORDZP(pc, KAI_ZPX(pc));
        }

        public UInt32 KA_INDY(K6502_Context pc)
        {
            return (UInt32)((KI_READWORDZP(pc, KAI_ZP(pc)) + pc.Y) & 0xffff);
        }

        //#if BUILD_HUC6280 || BUILD_M65C02
        //static Uword MasubCall KA_IND(__CONTEXT)
        //{
        //	return KI_READWORDZP(__THISP_ KAI_ZP(__THISP));
        //}
        //#else
        public UInt32 KI_READWORDBUG(K6502_Context pc, UInt32 adr)
        {
            UInt32 ret = K_READ(pc, adr);
            return ret + (K_READ(pc, (UInt32)((adr & 0xFF00) + (UInt32)((adr + 1) & 0xff))) << 8);
        }
        //#endif

        //#if BUILD_HUC6280
        //#define KA_ABSX_ KA_ABSX
        //#define KA_ABSY_ KA_ABSY
        //#define KA_INDY_ KA_INDY
        //#else

        public UInt32 KA_ABSX_(K6502_Context pc)
        {
            if ((UInt32)((pc.PC) & 0xff) == 0xFF) KI_ADDCLOCK(pc, 1); /* page break */
            return KAI_ABSX(pc);
        }

        public UInt32 KA_ABSY_(K6502_Context pc)
        {
            if ((UInt32)((pc.PC) & 0xff) == 0xFF) KI_ADDCLOCK(pc, 1); /* page break */
            return KAI_ABSY(pc);
        }

        public UInt32 KA_INDY_(K6502_Context pc)
        {
            UInt32 adr = KAI_INDY(pc);
            if ((UInt32)((adr) & 0xff) == 0xFF) KI_ADDCLOCK(pc, 1); /* page break */
            return adr;
        }

        //#endif

        public void KM_ALUADDER(K6502_Context pc, UInt32 src)
        {
            UInt32 w = pc.A + src + (pc.P & C_FLAG);
            pc.P &= (UInt32)(~(UInt32)(N_FLAG | V_FLAG | Z_FLAG | C_FLAG));
            pc.P += (UInt32)(FLAG_NZC((Int32)w)
                + ((((~pc.A ^ src) & (pc.A ^ w)) >> 1) & V_FLAG));
            pc.A = (UInt32)((w) & 0xff);
        }

        //#if 0
        //static void OpsubCall KM_ALUADDER_S(__CONTEXT_ Uword src)
        //{
        //	Uword w = __THIS__.A + src + (__THIS__.P & C_FLAG);
        //	__THIS__.P &= ~(N_FLAG | V_FLAG | Z_FLAG | C_FLAG);
        //	__THIS__.P += FLAG_NZC(w)
        //		+ (((__THIS__.A ^ ~src) & (__THIS__.A ^ w)) & V_FLAG);
        //	__THIS__.A = RTO8(w);
        //}
        //#endif

        public void KM_ALUADDER_D(K6502_Context pc, UInt32 src)
        {
            UInt32 wl = (pc.A & 0x0F) + (src & 0x0F) + (pc.P & C_FLAG);
            UInt32 w = pc.A + src + (pc.P & C_FLAG);
            //#if BUILD_HUC6280 || BUILD_M65C02
            //	__THIS__.P &= ~(N_FLAG | Z_FLAG | C_FLAG);
            //#else
            pc.P &= (UInt32)(~(UInt32)C_FLAG);
            //#endif
            if (wl > 0x9) w += 0x6;
            if (w > 0x9F)
            {
                pc.P += C_FLAG;
                w += 0x60;
            }
            //#if BUILD_HUC6280 || BUILD_M65C02
            //	__THIS__.P += FLAG_NZ(w);
            //#endif
            pc.A = (UInt32)((w) & 0xff);
            KI_ADDCLOCK(pc, 1);
        }

        public void KMI_ADC(K6502_Context pc, UInt32 src)
        {
            KM_ALUADDER(pc, src);
        }

        public void KMI_ADC_D(K6502_Context pc, UInt32 src)
        {
            KM_ALUADDER_D(pc, src);
        }

        public void KMI_SBC(K6502_Context pc, UInt32 src)
        {
            KM_ALUADDER(pc, src ^ 0xFF);
        }

        public void KMI_SBC_D(K6502_Context pc, UInt32 src)
        {
            KM_ALUADDER_D(pc, (UInt32)(((src ^ 0xFF) + (0x100 - 0x66)) & 0xff));
        }

        public void KM_CMP(K6502_Context pc, UInt32 src)
        {
            UInt32 w = pc.A + (src ^ 0xFF) + 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)FLAG_NZC((Int32)w);
        }

        public void KM_CPX(K6502_Context pc, UInt32 src)
        {
            UInt32 w = pc.X + (src ^ 0xFF) + 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)FLAG_NZC((Int32)w);
        }

        public void KM_CPY(K6502_Context pc, UInt32 src)
        {
            UInt32 w = pc.Y + (src ^ 0xFF) + 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)FLAG_NZC((Int32)w);
        }

        public void KM_BIT(K6502_Context pc, UInt32 src)
        {
            UInt32 w = pc.A & src;
            pc.P &= ~(UInt32)(N_FLAG | V_FLAG | Z_FLAG);
            pc.P += (UInt32)((src & (N_FLAG | V_FLAG)) + (w != 0 ? 0 : Z_FLAG));
        }

        public void KM_AND(K6502_Context pc, UInt32 src)
        {
            pc.A &= src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)pc.A);
        }

        public void KM_ORA(K6502_Context pc, UInt32 src)
        {
            pc.A |= src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)pc.A);
        }

        public void KM_EOR(K6502_Context pc, UInt32 src)
        {
            pc.A ^= src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)pc.A);
        }

        public UInt32 KM_DEC(K6502_Context pc, UInt32 des)
        {
            Int32 w = (Int32)(des - 1);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ(w);
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_INC(K6502_Context pc, UInt32 des)
        {
            Int32 w = (Int32)(des + 1);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ(w);
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_ASL(K6502_Context pc, UInt32 des)
        {
            Int32 w = (Int32)(des << 1);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ(w) + ((des >> 7)/* & C_FLAG*/));
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_LSR(K6502_Context pc, UInt32 des)
        {
            UInt32 w = des >> 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ((Int32)w) + (des & C_FLAG));
            return w;
        }

        public UInt32 KM_LD(K6502_Context pc, UInt32 src)
        {
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)src);
            return src;
        }

        public UInt32 KM_ROL(K6502_Context pc, UInt32 des)
        {
            UInt32 w = (des << 1) + (pc.P & C_FLAG);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ((Int32)w) + ((des >> 7)/* & C_FLAG*/));
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_ROR(K6502_Context pc, UInt32 des)
        {
            UInt32 w = (des >> 1) + ((pc.P & C_FLAG) << 7);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ((Int32)w) + (des & C_FLAG));
            return (UInt32)((w) & 0xff);
        }

        public void KM_BRA(K6502_Context pc, UInt32 rel)
        {
            //#if BUILD_HUC6280
            //	__THIS__.PC = RTO16(__THIS__.PC + (rel ^ 0x80) - 0x80);
            //	KI_ADDCLOCK(__THISP_ 2);
            //#else
            UInt32 oldPage = pc.PC & 0xFF00;
            pc.PC = (UInt32)((pc.PC + (rel ^ 0x80) - 0x80) & 0xffff);
            KI_ADDCLOCK(pc, (UInt32)(1 + ((oldPage != (pc.PC & 0xFF00)) ? 1 : 0)));
            //#endif
        }

        public void KM_PUSH(K6502_Context pc, UInt32 src)
        {
            K_WRITE(pc, (UInt32)(BASE_OF_ZERO + 0x100 + pc.S), src);
            pc.S = (UInt32)((pc.S - 1) & 0xff);
        }

        public UInt32 KM_POP(K6502_Context pc)
        {
            pc.S = (UInt32)((pc.S + 1) & 0xff);
            return K_READ(pc, (UInt32)(BASE_OF_ZERO + 0x100 + pc.S));
        }

        //#if BUILD_HUC6280 || BUILD_M65C02
        //static Uword OpsubCall KM_TSB(__CONTEXT_ Uword mem)
        //{
        //	Uword w = __THIS__.A | mem;
        //	__THIS__.P &= ~(N_FLAG | V_FLAG | Z_FLAG);
        //	__THIS__.P += (mem & (N_FLAG | V_FLAG)) + (w ? 0 : Z_FLAG);
        //	return w;
        //}
        //static Uword OpsubCall KM_TRB(__CONTEXT_ Uword mem)
        //{
        //	Uword w = (__THIS__.A ^ 0xFF) & mem;
        //	__THIS__.P &= ~(N_FLAG | V_FLAG | Z_FLAG);
        //	__THIS__.P += (mem & (N_FLAG | V_FLAG)) + (w ? 0 : Z_FLAG);
        //	return w;
        //}
        //#endif
        //#if BUILD_HUC6280
        //static Uword Inline KMI_PRET(__CONTEXT)
        //{
        //	Uword saveA = __THIS__.A;
        //	__THIS__.A = K_READZP(__THISP_ (Uword)(BASE_OF_ZERO + __THIS__.X));
        //	return saveA;
        //}
        //static void Inline KMI_POSTT(__CONTEXT_ Uword saveA)
        //{
        //	K_WRITEZP(__THISP_ (Uword)(BASE_OF_ZERO + __THIS__.X), __THIS__.A);
        //	__THIS__.A = saveA;
        //	 KI_ADDCLOCK(__THISP_ 3);
        //}
        //static void OpsubCall KM_TST(__CONTEXT_ Uword imm, Uword mem)
        //{
        //	Uword w = imm & mem;
        //	__THIS__.P &= ~(N_FLAG | V_FLAG | Z_FLAG);
        //	__THIS__.P += (mem & (N_FLAG | V_FLAG)) + (w ? 0 : Z_FLAG);
        //}
        //#endif

        /* --- ADC ---  */
        //#if BUILD_HUC6280
        //#define DEF_ADC(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ KMI_ADC(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall D_Opco##i##(__CONTEXT) \
        //{ KMI_ADC_D(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i##(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KMI_ADC(__THISP_ K_READ##p##(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //} \
        //static void OpcodeCall TD_Opc##i##(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KMI_ADC_D(__THISP_ K_READ##p##(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        //#define DEF_ADC(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ KMI_ADC(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall D_Opco##i##(__CONTEXT) \
        //{ KMI_ADC_D(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }

        public void Opcode61(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_INDX(pc)));
        }
        public void D_Opco61(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_INDX(pc)));
        }

        public void Opcode65(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_ZP(pc)));
        }
        public void D_Opco65(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_ZP(pc)));
        }

        public void Opcode69(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_IMM(pc)));
        }
        public void D_Opco69(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_IMM(pc)));
        }

        public void Opcode6D(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_ABS(pc)));
        }
        public void D_Opco6D(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_ABS(pc)));
        }

        public void Opcode71(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_INDY(pc)));
        }
        public void D_Opco71(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_INDY(pc)));
        }

        public void Opcode75(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_ZPX(pc)));
        }
        public void D_Opco75(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_ZPX(pc)));
        }

        public void Opcode79(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_ABSY(pc)));
        }
        public void D_Opco79(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_ABSY(pc)));
        }

        public void Opcode7D(K6502_Context pc)
        {
            KMI_ADC(pc, K_READ(pc, KA_ABSX(pc)));
        }
        public void D_Opco7D(K6502_Context pc)
        {
            KMI_ADC_D(pc, K_READ(pc, KA_ABSX(pc)));
        }

        //#endif
        //DEF_ADC(61, NP, KA_INDX)	/* 61 - ADC - (Indirect,X) */
        //DEF_ADC(65, ZP, KA_ZP)	/* 65 - ADC - Zero Page */
        //DEF_ADC(69, NP, KA_IMM)	/* 69 - ADC - Immediate */
        //DEF_ADC(6D, NP, KA_ABS)	/* 6D - ADC - Absolute */
        //DEF_ADC(71, NP, KA_INDY_)	/* 71 - ADC - (Indirect),Y */
        //DEF_ADC(75, ZP, KA_ZPX)	/* 75 - ADC - Zero Page,X */
        //DEF_ADC(79, NP, KA_ABSY_)	/* 79 - ADC - Absolute,Y */
        //DEF_ADC(7D, NP, KA_ABSX_)   /* 7D - ADC - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_ADC(72,NP,KA_IND)	/* 72 - ADC - (Indirect) */
        //#endif

        /* --- AND ---  */
        //#if BUILD_HUC6280
        //#define DEF_AND(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ KM_AND(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i##(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KM_AND(__THISP_ K_READ##p##(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        //#define DEF_AND(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //        {
        //            KM_AND(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //#endif

        public void Opcode21(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_INDX(pc)));
        }
        public void Opcode25(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_ZP(pc)));
        }
        public void Opcode29(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_IMM(pc)));
        }
        public void Opcode2D(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_ABS(pc)));
        }
        public void Opcode31(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_INDY(pc)));
        }
        public void Opcode35(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_ZPX(pc)));
        }
        public void Opcode39(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_ABSY(pc)));
        }
        public void Opcode3D(K6502_Context pc)
        {
            KM_AND(pc, K_READ(pc, KA_ABSX(pc)));
        }

        //DEF_AND(21, NP, KA_INDX)    /* 21 - AND - (Indirect,X) */
        //DEF_AND(25, ZP, KA_ZP)  /* 25 - AND - Zero Page */
        //DEF_AND(29, NP, KA_IMM) /* 29 - AND - Immediate */
        //DEF_AND(2D, NP, KA_ABS) /* 2D - AND - Absolute */
        //DEF_AND(31, NP, KA_INDY_)   /* 31 - AND - (Indirect),Y */
        //DEF_AND(35, ZP, KA_ZPX) /* 35 - AND - Zero Page,X */
        //DEF_AND(39, NP, KA_ABSY_)   /* 39 - AND - Absolute,Y */
        //DEF_AND(3D, NP, KA_ABSX_)	/* 3D - AND - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_AND(32,NP,KA_IND)	/* 32 - AND - (Indirect) */
        //#endif

        /* --- ASL ---  */
        public void Opcode06(K6502_Context pc)
        {
            UInt32 adr = KA_ZP(pc);
            K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr)));
        }
        public void Opcode0E(K6502_Context pc)
        {
            UInt32 adr = KA_ABS(pc);
            K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr)));
        }
        public void Opcode16(K6502_Context pc)
        {
            UInt32 adr = KA_ZPX(pc);
            K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr)));
        }
        public void Opcode1E(K6502_Context pc)
        {
            UInt32 adr = KA_ABSX(pc);
            K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr)));
        }

        //DEF_ASL(06, ZP, KA_ZP)	/* 06 - ASL - Zero Page */
        //DEF_ASL(0E, NP, KA_ABS)	/* 0E - ASL - Absolute */
        //DEF_ASL(16, ZP, KA_ZPX)	/* 16 - ASL - Zero Page,X */
        //DEF_ASL(1E, NP, KA_ABSX)	/* 1E - ASL - Absolute,X */
        public void Opcode0A(K6502_Context pc)  /* 0A - ASL - Accumulator */
        {
            pc.A = KM_ASL(pc, pc.A);
        }

        //#if BUILD_HUC6280
        ///* --- BBRi --- */
        //#define DEF_BBR(i,y) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword adr = KA_ZP(__THISP); \
        //	Uword rel = K_READNP(__THISP_ KA_IMM(__THISP)); \
        //	if ((K_READZP(__THISP_ adr) & (1 << y)) == 0) KM_BRA(__THISP_ rel); \
        //}
        //DEF_BBR(0F,0)			/* 0F - BBR0 */
        //DEF_BBR(1F,1)			/* 1F - BBR1 */
        //DEF_BBR(2F,2)			/* 2F - BBR2 */
        //DEF_BBR(3F,3)			/* 3F - BBR3 */
        //DEF_BBR(4F,4)			/* 4F - BBR4 */
        //DEF_BBR(5F,5)			/* 5F - BBR5 */
        //DEF_BBR(6F,6)			/* 6F - BBR6 */
        //DEF_BBR(7F,7)			/* 7F - BBR7 */
        ///* --- BBSi --- */
        //#define DEF_BBS(i,y) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword adr = KA_ZP(__THISP); \
        //	Uword rel = K_READNP(__THISP_ KA_IMM(__THISP)); \
        //	if ((K_READZP(__THISP_ adr) & (1 << y)) != 0) KM_BRA(__THISP_ rel); \
        //}
        //DEF_BBS(8F,0)			/* 8F - BBS0 */
        //DEF_BBS(9F,1)			/* 9F - BBS1 */
        //DEF_BBS(AF,2)			/* AF - BBS2 */
        //DEF_BBS(BF,3)			/* BF - BBS3 */
        //DEF_BBS(CF,4)			/* CF - BBS4 */
        //DEF_BBS(DF,5)			/* DF - BBS5 */
        //DEF_BBS(EF,6)			/* EF - BBS6 */
        //DEF_BBS(FF,7)			/* FF - BBS7 */
        //#endif

        /* --- BIT ---  */
        public void Opcode24(K6502_Context pc)
        {
            KM_BIT(pc, K_READ(pc, KA_ZP(pc)));
        }
        public void Opcode2C(K6502_Context pc)
        {
            KM_BIT(pc, K_READ(pc, KA_ABS(pc)));
        }

        //#define DEF_BIT(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //            {
        //                KM_BIT(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_BIT(24, ZP, KA_ZP)  /* 24 - BIT - Zero Page */
        //DEF_BIT(2C, NP, KA_ABS) /* 2C - BIT - Absolute */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_BIT(34,ZP,KA_ZPX)	/* 34 - BIT - Zero Page,X */
        //DEF_BIT(3C,NP,KA_ABSX_)	/* 3C - BIT - Absolute,X */
        //DEF_BIT(89,NP,KA_IMM)	/* 89 - BIT - Immediate */
        //#endif

        /* --- Bcc ---  */
        public void Opcode10(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & N_FLAG) == 0)) KM_BRA(pc, rel);
        }
        public void Opcode30(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & N_FLAG) != 0)) KM_BRA(pc, rel);
        }
        public void Opcode50(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & V_FLAG) == 0)) KM_BRA(pc, rel);
        }
        public void Opcode70(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & V_FLAG) != 0)) KM_BRA(pc, rel);
        }
        public void Opcode90(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & C_FLAG) == 0)) KM_BRA(pc, rel);
        }
        public void OpcodeB0(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & C_FLAG) != 0)) KM_BRA(pc, rel);
        }
        public void OpcodeD0(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & Z_FLAG) == 0)) KM_BRA(pc, rel);
        }
        public void OpcodeF0(K6502_Context pc)
        {
            UInt32 rel = K_READ(pc, KA_IMM(pc));
            if (((pc.P & Z_FLAG) != 0)) KM_BRA(pc, rel);
        }
        ////#define DEF_BRA(i,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        ////                { \
        ////	Uword rel = K_READNP(__THISP_ KA_IMM(__THISP)); \
        ////	if (a) KM_BRA(__THISP_ rel); \
        ////}
        ////DEF_BRA(10, ((__THIS__.P & N_FLAG) == 0))   /* 10 - BPL */
        //DEF_BRA(30, ((__THIS__.P & N_FLAG) != 0))   /* 30 - BMI */
        //DEF_BRA(50, ((__THIS__.P & V_FLAG) == 0))   /* 50 - BVC */
        //DEF_BRA(70, ((__THIS__.P & V_FLAG) != 0))   /* 70 - BVS */
        //DEF_BRA(90, ((__THIS__.P & C_FLAG) == 0))   /* 90 - BCC */
        //DEF_BRA(B0, ((__THIS__.P & C_FLAG) != 0))   /* B0 - BCS */
        //DEF_BRA(D0, ((__THIS__.P & Z_FLAG) == 0))   /* D0 - BNE */
        //DEF_BRA(F0, ((__THIS__.P & Z_FLAG) != 0))   /* F0 - BEQ */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_BRA(80,1)								/* 80 - BRA */
        //#endif

        /* --- BRK --- */
        public void Opcode00(K6502_Context pc)
        {
            pc.PC = (UInt32)((pc.PC + 1) & 0xffff);
            pc.iRequest |= IRQ_BRK;
        }

        //static void OpcodeCall Opcode00(__CONTEXT)  /* 00 - BRK */
        //{
        //                    __THIS__.PC = RTO16(__THIS__.PC + 1);
        //                    __THIS__.iRequest |= IRQ_BRK;
        //                }
        //#if BUILD_HUC6280
        ///* --- BSR --- */
        //static void OpcodeCall Opcode44(__CONTEXT)	/* 44 - BSR */
        //{
        //	KM_PUSH(__THISP_ RTO8(__THIS__.PC >> 8));	/* !!! PC = NEXT - 1; !!! */
        //	KM_PUSH(__THISP_ RTO8(__THIS__.PC));
        //	KM_BRA(__THISP_ K_READNP(__THISP_ KA_IMM(__THISP)));
        //}
        ///* --- CLA --- */
        //static void OpcodeCall Opcode62(__CONTEXT)	/* 62 - CLA */
        //{ __THIS__.A = 0; }
        ///* --- CLX --- */
        //static void OpcodeCall Opcode82(__CONTEXT)	/* 82 - CLX */
        //{ __THIS__.X = 0; }
        ///* --- CLY --- */
        //static void OpcodeCall OpcodeC2(__CONTEXT)	/* C2 - CLY */
        //{ __THIS__.Y = 0; }
        //#endif
        /* --- CLC --- */
        public void Opcode18(K6502_Context pc)  /* 18 - CLC */
        { pc.P &= ~(UInt32)C_FLAG; }
        /* --- CLD --- */
        public void OpcodeD8(K6502_Context pc)  /* D8 - CLD */
        {
            pc.P &= ~(UInt32)D_FLAG;
        }
        /* --- CLI --- */
        public void Opcode58(K6502_Context pc)  /* 58 - CLI */
        { pc.P &= ~(UInt32)I_FLAG; }
        /* --- CLV --- */
        public void OpcodeB8(K6502_Context pc)  /* B8 - CLV */
        { pc.P &= ~(UInt32)V_FLAG; }

        /* --- CMP --- */
        public void OpcodeC1(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_INDX(pc)));
        }
        public void OpcodeC5(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_ZP(pc)));
        }
        public void OpcodeC9(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_IMM(pc)));
        }
        public void OpcodeCD(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_ABS(pc)));
        }
        public void OpcodeD1(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_INDY(pc)));
        }
        public void OpcodeD5(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_ZPX(pc)));
        }
        public void OpcodeD9(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_ABSY(pc)));
        }
        public void OpcodeDD(K6502_Context pc)
        {
            KM_CMP(pc, K_READ(pc, KA_ABSX(pc)));
        }
        //#define DEF_CMP(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                {
        //                    KM_CMP(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_CMP(C1, NP, KA_INDX)    /* C1 - CMP - (Indirect,X) */
        //DEF_CMP(C5, ZP, KA_ZP)  /* C5 - CMP - Zero Page */
        //DEF_CMP(C9, NP, KA_IMM) /* C9 - CMP - Immediate */
        //DEF_CMP(CD, NP, KA_ABS) /* CD - CMP - Absolute */
        //DEF_CMP(D1, NP, KA_INDY_)   /* D1 - CMP - (Indirect),Y */
        //DEF_CMP(D5, ZP, KA_ZPX) /* D5 - CMP - Zero Page,X */
        //DEF_CMP(D9, NP, KA_ABSY_)   /* D9 - CMP - Absolute,Y */
        //DEF_CMP(DD, NP, KA_ABSX_)   /* DD - CMP - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_CMP(D2,NP,KA_IND)	/* D2 - CMP - (Indirect) */
        //#endif

        /* --- CPX --- */
        public void OpcodeE0(K6502_Context pc)
        {
            KM_CPX(pc, K_READ(pc, KA_IMM(pc)));
        }
        public void OpcodeE4(K6502_Context pc)
        {
            KM_CPX(pc, K_READ(pc, KA_ZP(pc)));
        }
        public void OpcodeEC(K6502_Context pc)
        {
            KM_CPX(pc, K_READ(pc, KA_ABS(pc)));
        }
        //#define DEF_CPX(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                    {
        //                        KM_CPX(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_CPX(E0, NP, KA_IMM) /* E0 - CPX - Immediate */
        //DEF_CPX(E4, ZP, KA_ZP)  /* E4 - CPX - Zero Page */
        //DEF_CPX(EC, NP, KA_ABS)	/* EC - CPX - Absolute */

        /* --- CPY --- */
        public void OpcodeC0(K6502_Context pc)
        {
            KM_CPY(pc, K_READ(pc, KA_IMM(pc)));
        }
        public void OpcodeC4(K6502_Context pc)
        {
            KM_CPY(pc, K_READ(pc, KA_ZP(pc)));
        }
        public void OpcodeCC(K6502_Context pc)
        {
            KM_CPY(pc, K_READ(pc, KA_ABS(pc)));
        }
        //#define DEF_CPY(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                        {
        //                            KM_CPY(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_CPY(C0, NP, KA_IMM) /* C0 - CPY - Immediate */
        //DEF_CPY(C4, ZP, KA_ZP)  /* C4 - CPY - Zero Page */
        //DEF_CPY(CC, NP, KA_ABS)	/* CC - CPY - Absolute */

        /* --- DEC ---  */
        public void OpcodeC6(K6502_Context pc)
        {
            UInt32 adr = KA_ZP(pc);
            K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr)));
        }
        public void OpcodeCE(K6502_Context pc)
        {
            UInt32 adr = KA_ABS(pc);
            K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr)));
        }
        public void OpcodeD6(K6502_Context pc)
        {
            UInt32 adr = KA_ZPX(pc);
            K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr)));
        }
        public void OpcodeDE(K6502_Context pc)
        {
            UInt32 adr = KA_ABSX(pc);
            K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr)));
        }
        //#define DEF_DEC(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                            { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_DEC(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //                            DEF_DEC(C6, ZP, KA_ZP)	/* C6 - DEC - Zero Page */
        //DEF_DEC(CE, NP, KA_ABS)	/* CE - DEC - Absolute */
        //DEF_DEC(D6, ZP, KA_ZPX)	/* D6 - DEC - Zero Page,X */
        //DEF_DEC(DE, NP, KA_ABSX)	/* DE - DEC - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //static void OpcodeCall Opcode3A(__CONTEXT)	/* 3A - DEA */
        //{ __THIS__.A = KM_DEC(__THISP_ __THIS__.A); }
        //#endif
        public void OpcodeCA(K6502_Context pc)  /* CA - DEX */
        { pc.X = KM_DEC(pc, pc.X); }
        public void Opcode88(K6502_Context pc)  /* 88 - DEY */
        { pc.Y = KM_DEC(pc, pc.Y); }

        /* --- EOR ---  */
        //#if BUILD_HUC6280
        //#define DEF_EOR(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ KM_EOR(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i##(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KM_EOR(__THISP_ K_READ##p##(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        public void Opcode41(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_INDX(pc))); }
        public void Opcode45(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode49(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_IMM(pc))); }
        public void Opcode4D(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode51(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_INDY(pc))); }
        public void Opcode55(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode59(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_ABSY(pc))); }
        public void Opcode5D(K6502_Context pc) { KM_EOR(pc, K_READ(pc, KA_ABSX(pc))); }
        //#define DEF_EOR(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                            {
        //                                KM_EOR(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //#endif
        //DEF_EOR(41, NP, KA_INDX)    /* 41 - EOR - (Indirect,X) */
        //DEF_EOR(45, ZP, KA_ZP)  /* 45 - EOR - Zero Page */
        //DEF_EOR(49, NP, KA_IMM) /* 49 - EOR - Immediate */
        //DEF_EOR(4D, NP, KA_ABS) /* 4D - EOR - Absolute */
        //DEF_EOR(51, NP, KA_INDY_)   /* 51 - EOR - (Indirect),Y */
        //DEF_EOR(55, ZP, KA_ZPX) /* 55 - EOR - Zero Page,X */
        //DEF_EOR(59, NP, KA_ABSY_)   /* 59 - EOR - Absolute,Y */
        //DEF_EOR(5D, NP, KA_ABSX_)   /* 5D - EOR - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_EOR(52,NP,KA_IND)	/* 52 - EOR - (Indirect) */
        //#endif

        /* --- INC ---  */
        public void OpcodeE6(K6502_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        public void OpcodeEE(K6502_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        public void OpcodeF6(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        public void OpcodeFE(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        //#define DEF_INC(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_INC(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //                                DEF_INC(E6, ZP, KA_ZP)  /* E6 - INC - Zero Page */
        //DEF_INC(EE, NP, KA_ABS) /* EE - INC - Absolute */
        //DEF_INC(F6, ZP, KA_ZPX) /* F6 - INC - Zero Page,X */
        //DEF_INC(FE, NP, KA_ABSX)    /* FE - INC - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //static void OpcodeCall Opcode1A(__CONTEXT)	/* 1A - INA */
        //{ __THIS__.A = KM_INC(__THISP_ __THIS__.A); }
        //#endif
        public void OpcodeE8(K6502_Context pc)  /* E8 - INX */{ pc.X = KM_INC(pc, pc.X); }
        public void OpcodeC8(K6502_Context pc)  /* C8 - INY */{ pc.Y = KM_INC(pc, pc.Y); }

        /* --- JMP ---  */
        public void Opcode4C(K6502_Context pc) { pc.PC = KI_READWORD(pc, KA_IMM16(pc)); }
        public void Opcode6C(K6502_Context pc) { pc.PC = KI_READWORDBUG(pc, KA_ABS(pc)); }
        //#define DEF_JMP(i,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                { __THIS__.PC = KI_READWORD(__THISP_ a(__THISP)); }
        //#if BUILD_HUC6280 || BUILD_M65C02
        //#define DEF_JMPBUG(i,a) DEF_JMP(i,a)
        //#else
        //#define DEF_JMPBUG(i,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                { __THIS__.PC = KI_READWORDBUG(__THISP_ a(__THISP)); }
        //#endif
        //                                DEF_JMP(4C, KA_IMM16)   /* 4C - JMP - Immediate */
        //DEF_JMPBUG(6C, KA_ABS)  /* 6C - JMP - Absolute */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_JMP(7C,KA_ABSX)	/* 7C - JMP - Absolute,X */
        //#endif

        /* --- JSR --- */
        public void Opcode20(K6502_Context pc)  /* 20 - JSR */
        {
            UInt32 adr = KA_IMM(pc);
            KM_PUSH(pc, (UInt32)((pc.PC >> 8) & 0xff));   /* !!! PC = NEXT - 1; !!! */
            KM_PUSH(pc, (UInt32)((pc.PC) & 0xff));
            pc.PC = KI_READWORD(pc, adr);
        }

        /* --- LDA --- */
        public void OpcodeA1(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_INDX(pc))); }
        public void OpcodeA5(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeA9(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeAD(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeB1(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_INDY(pc))); }
        public void OpcodeB5(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeB9(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ABSY(pc))); }
        public void OpcodeBD(K6502_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ABSX(pc))); }
        //#define DEF_LDA(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                {
        //                                    __THIS__.A = KM_LD(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_LDA(A1, NP, KA_INDX)    /* A1 - LDA - (Indirect,X) */
        //DEF_LDA(A5, ZP, KA_ZP)  /* A5 - LDA - Zero Page */
        //DEF_LDA(A9, NP, KA_IMM) /* A9 - LDA - Immediate */
        //DEF_LDA(AD, NP, KA_ABS) /* AD - LDA - Absolute */
        //DEF_LDA(B1, NP, KA_INDY_)   /* B1 - LDA - (Indirect),Y */
        //DEF_LDA(B5, ZP, KA_ZPX) /* B5 - LDA - Zero Page,X */
        //DEF_LDA(B9, NP, KA_ABSY_)   /* B9 - LDA - Absolute,Y */
        //DEF_LDA(BD, NP, KA_ABSX_)   /* BD - LDA - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_LDA(B2,NP,KA_IND)	/* B2 - LDA - (Indirect) */
        //#endif

        /* --- LDX ---  */
        public void OpcodeA2(K6502_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeA6(K6502_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeAE(K6502_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeB6(K6502_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ZPY(pc))); }
        public void OpcodeBE(K6502_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ABSY(pc))); }
        //#define DEF_LDX(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                    {
        //                                        __THIS__.X = KM_LD(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_LDX(A2, NP, KA_IMM) /* A2 - LDX - Immediate */
        //DEF_LDX(A6, ZP, KA_ZP)  /* A6 - LDX - Zero Page */
        //DEF_LDX(AE, NP, KA_ABS) /* AE - LDX - Absolute */
        //DEF_LDX(B6, ZP, KA_ZPY) /* B6 - LDX - Zero Page,Y */
        //DEF_LDX(BE, NP, KA_ABSY_)	/* BE - LDX - Absolute,Y */

        /* --- LDY ---  */
        public void OpcodeA0(K6502_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeA4(K6502_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeAC(K6502_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeB4(K6502_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeBC(K6502_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ABSX(pc))); }
        //#define DEF_LDY(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                        {
        //                                            __THIS__.Y = KM_LD(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_LDY(A0, NP, KA_IMM) /* A0 - LDY - Immediate */
        //DEF_LDY(A4, ZP, KA_ZP)  /* A4 - LDY - Zero Page */
        //DEF_LDY(AC, NP, KA_ABS) /* AC - LDY - Absolute */
        //DEF_LDY(B4, ZP, KA_ZPX) /* B4 - LDY - Zero Page,X */
        //DEF_LDY(BC, NP, KA_ABSX_)	/* BC - LDY - Absolute,X */

        /* --- LSR ---  */
        public void Opcode46(K6502_Context pc)
        {
            UInt32 adr = KA_ZP(pc);
            K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr)));
        }
        public void Opcode4E(K6502_Context pc)
        {
            UInt32 adr = KA_ABS(pc);
            K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr)));
        }
        public void Opcode56(K6502_Context pc)
        {
            UInt32 adr = KA_ZPX(pc);
            K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr)));
        }
        public void Opcode5E(K6502_Context pc)
        {
            UInt32 adr = KA_ABSX(pc);
            K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr)));
        }

        //#define DEF_LSR(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                            { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_LSR(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //                                            DEF_LSR(46, ZP, KA_ZP)	/* 46 - LSR - Zero Page */
        //DEF_LSR(4E, NP, KA_ABS)	/* 4E - LSR - Absolute */
        //DEF_LSR(56, ZP, KA_ZPX)	/* 56 - LSR - Zero Page,X */
        //DEF_LSR(5E, NP, KA_ABSX)	/* 5E - LSR - Absolute,X */
        public void Opcode4A(K6502_Context pc)  /* 4A - LSR - Accumulator */
        { pc.A = KM_LSR(pc, pc.A); }

        /* --- NOP ---  */
        public void OpcodeEA(K6502_Context pc)  /* EA - NOP */
        {
        }

        /* --- ORA ---  */
        //#if BUILD_HUC6280
        //#define DEF_ORA(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ KM_ORA(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i##(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KM_ORA(__THISP_ K_READ##p##(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        public void Opcode01(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_INDX(pc))); }
        public void Opcode05(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode09(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_IMM(pc))); }
        public void Opcode0D(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode11(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_INDY(pc))); }
        public void Opcode15(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode19(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_ABSY(pc))); }
        public void Opcode1D(K6502_Context pc) { KM_ORA(pc, K_READ(pc, KA_ABSX(pc))); }
        //#define DEF_ORA(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                            {
        //                                                KM_ORA(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //#endif
        //DEF_ORA(01, NP, KA_INDX)    /* 01 - ORA - (Indirect,X) */
        //DEF_ORA(05, ZP, KA_ZP)  /* 05 - ORA - Zero Page */
        //DEF_ORA(09, NP, KA_IMM) /* 09 - ORA - Immediate */
        //DEF_ORA(0D, NP, KA_ABS) /* 0D - ORA - Absolute */
        //DEF_ORA(11, NP, KA_INDY_)   /* 11 - ORA - (Indirect),Y */
        //DEF_ORA(15, ZP, KA_ZPX) /* 15 - ORA - Zero Page,X */
        //DEF_ORA(19, NP, KA_ABSY_)   /* 19 - ORA - Absolute,Y */
        //DEF_ORA(1D, NP, KA_ABSX_)   /* 1D - ORA - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_ORA(12,NP,KA_IND)	/* 12 - ORA - (Indirect) */
        //#endif

        /* --- PHr PLr  --- */
        public void Opcode48(K6502_Context pc)  /* 48 - PHA */
        { KM_PUSH(pc, pc.A); }
        public void Opcode08(K6502_Context pc)  /* 08 - PHP */
        { KM_PUSH(pc, (UInt32)((pc.P | B_FLAG | R_FLAG) & ~T_FLAG)); }
        public void Opcode68(K6502_Context pc)  /* 68 - PLA */
        { pc.A = KM_LD(pc, KM_POP(pc)); }
        public void Opcode28(K6502_Context pc)  /* 28 - PLP */
        { pc.P = (UInt32)(KM_POP(pc) & ~T_FLAG); }
        //#if BUILD_HUC6280 || BUILD_M65C02
        //static void OpcodeCall OpcodeDA(__CONTEXT)	/* DA - PHX */
        //{ KM_PUSH(__THISP_ __THIS__.X); }
        //static void OpcodeCall Opcode5A(__CONTEXT)	/* 5A - PHY */
        //{ KM_PUSH(__THISP_ __THIS__.Y); }
        //static void OpcodeCall OpcodeFA(__CONTEXT)	/* FA - PLX */
        //{ __THIS__.X = KM_LD(__THISP_ KM_POP(__THISP)); }
        //static void OpcodeCall Opcode7A(__CONTEXT)	/* 7A - PLY */
        //{ __THIS__.Y = KM_LD(__THISP_ KM_POP(__THISP)); }
        //#endif

        //#if BUILD_HUC6280
        ///* --- RMBi --- */
        //#define DEF_RMB(i,y) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword adr = KA_ZP(__THISP); \
        //	K_WRITEZP(__THISP_ adr, (Uword)(K_READZP(__THISP_ adr) & (~(1 << y)))); \
        //}
        //DEF_RMB(07,0)	/* 07 - RMB0 */
        //DEF_RMB(17,1)	/* 17 - RMB1 */
        //DEF_RMB(27,2)	/* 27 - RMB2 */
        //DEF_RMB(37,3)	/* 37 - RMB3 */
        //DEF_RMB(47,4)	/* 47 - RMB4 */
        //DEF_RMB(57,5)	/* 57 - RMB5 */
        //DEF_RMB(67,6)	/* 67 - RMB6 */
        //DEF_RMB(77,7)	/* 77 - RMB7 */
        ///* --- SMBi --- */
        //#define DEF_SMB(i,y) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword adr = KA_ZP(__THISP); \
        //	K_WRITEZP(__THISP_ adr, (Uword)(K_READZP(__THISP_ adr) | (1 << y))); \
        //}
        //DEF_SMB(87,0)	/* 87 - SMB0 */
        //DEF_SMB(97,1)	/* 97 - SMB1 */
        //DEF_SMB(A7,2)	/* A7 - SMB2 */
        //DEF_SMB(B7,3)	/* B7 - SMB3 */
        //DEF_SMB(C7,4)	/* C7 - SMB4 */
        //DEF_SMB(D7,5)	/* D7 - SMB5 */
        //DEF_SMB(E7,6)	/* E7 - SMB6 */
        //DEF_SMB(F7,7)	/* F7 - SMB7 */
        //#endif

        /* --- ROL ---  */
        public void Opcode26(K6502_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode2E(K6502_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode36(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode3E(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        //#define DEF_ROL(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                                { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_ROL(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //                                                DEF_ROL(26, ZP, KA_ZP)  /* 26 - ROL - Zero Page */
        //DEF_ROL(2E, NP, KA_ABS) /* 2E - ROL - Absolute */
        //DEF_ROL(36, ZP, KA_ZPX) /* 36 - ROL - Zero Page,X */
        //DEF_ROL(3E, NP, KA_ABSX)    /* 3E - ROL - Absolute,X */
        public void Opcode2A(K6502_Context pc)  /* 2A - ROL - Accumulator */
        { pc.A = KM_ROL(pc, pc.A); }

        /* --- ROR ---  */
        public void Opcode66(K6502_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        public void Opcode6E(K6502_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        public void Opcode76(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        public void Opcode7E(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        //#define DEF_ROR(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                                { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_ROR(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //                                                DEF_ROR(66, ZP, KA_ZP)  /* 66 - ROR - Zero Page */
        //DEF_ROR(6E, NP, KA_ABS) /* 6E - ROR - Absolute */
        //DEF_ROR(76, ZP, KA_ZPX) /* 76 - ROR - Zero Page,X */
        //DEF_ROR(7E, NP, KA_ABSX)    /* 7E - ROR - Absolute,X */
        public void Opcode6A(K6502_Context pc)  /* 6A - ROR - Accumulator */
        { pc.A = KM_ROR(pc, pc.A); }

        public void Opcode40(K6502_Context pc)  /* 40 - RTI */
        {
            pc.P = KM_POP(pc);
            pc.PC = KM_POP(pc);
            pc.PC += KM_POP(pc) << 8;
        }

        public void Opcode60(K6502_Context pc)  /* 60 - RTS */
        {
            pc.PC = KM_POP(pc);
            pc.PC += KM_POP(pc) << 8;
            pc.PC = (UInt32)((pc.PC + 1) & 0xffff);
        }

        //#if BUILD_HUC6280
        //static void OpcodeCall Opcode22(__CONTEXT)	/* 22 - SAX */
        //{
        //	Uword temp = __THIS__.A;
        //	__THIS__.A = __THIS__.X;
        //	__THIS__.X = temp;
        //}
        //static void OpcodeCall Opcode42(__CONTEXT)	/* 42 - SAY */
        //{
        //	Uword temp = __THIS__.A;
        //	__THIS__.A = __THIS__.Y;
        //	__THIS__.Y = temp;
        //}
        //static void OpcodeCall Opcode02(__CONTEXT)	/* 02 - SXY */
        //{
        //	Uword temp = __THIS__.Y;
        //	__THIS__.Y = __THIS__.X;
        //	__THIS__.X = temp;
        //}
        //#endif

        /* --- SBC ---  */
        public void OpcodeE1(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_INDX(pc))); }
        public void D_OpcoE1(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_INDX(pc))); }
        public void OpcodeE5(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ZP(pc))); }
        public void D_OpcoE5(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeE9(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_IMM(pc))); }
        public void D_OpcoE9(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeED(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ABS(pc))); }
        public void D_OpcoED(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeF1(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_INDY(pc))); }
        public void D_OpcoF1(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_INDY(pc))); }
        public void OpcodeF5(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ZPX(pc))); }
        public void D_OpcoF5(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeF9(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ABSY(pc))); }
        public void D_OpcoF9(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ABSY(pc))); }
        public void OpcodeFD(K6502_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ABSX(pc))); }
        public void D_OpcoFD(K6502_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ABSX(pc))); }
        //#define DEF_SBC(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                                {
        //                                                    KMI_SBC(__THISP_ K_READ##p##(__THISP_ a(__THISP))); } \
        //static void OpcodeCall D_Opco##i##(__CONTEXT) \
        //{
        //                                                        KMI_SBC_D(__THISP_ K_READ##p##(__THISP_ a(__THISP))); }
        //DEF_SBC(E1, NP, KA_INDX)    /* E1 - SBC - (Indirect,X) */
        //DEF_SBC(E5, ZP, KA_ZP)  /* E5 - SBC - Zero Page */
        //DEF_SBC(E9, NP, KA_IMM) /* E9 - SBC - Immediate */
        //DEF_SBC(ED, NP, KA_ABS) /* ED - SBC - Absolute */
        //DEF_SBC(F1, NP, KA_INDY_)   /* F1 - SBC - (Indirect),Y */
        //DEF_SBC(F5, ZP, KA_ZPX) /* F5 - SBC - Zero Page,X */
        //DEF_SBC(F9, NP, KA_ABSY_)   /* F9 - SBC - Absolute,Y */
        //DEF_SBC(FD, NP, KA_ABSX_)	/* FD - SBC - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_SBC(F2,NP,KA_IND)	/* F2 - SBC - (Indirect) */
        //#endif

        /* --- SEC --- */
        public void Opcode38(K6502_Context pc)  /* 38 - SEC */
        { pc.P |= C_FLAG; }
        /* --- SED --- */
        public void OpcodeF8(K6502_Context pc)  /* F8 - SED */
        { pc.P |= D_FLAG; }
        /* --- SEI --- */
        public void Opcode78(K6502_Context pc)  /* 78 - SEI */
        { pc.P |= I_FLAG; }

        //#if BUILD_HUC6280
        ///* --- SET --- */
        //static void OpcodeCall OpcodeF4(__CONTEXT)	/* F4 - SET */
        //{ __THIS__.P |= T_FLAG; }
        //#endif

        //#if BUILD_HUC6280
        //static void OpcodeCall Opcode03(__CONTEXT)	/* 03 - ST0 */
        //{ K_WRITE6270(__THISP_ 0, K_READNP(__THISP_ KA_IMM(__THISP))); }
        //static void OpcodeCall Opcode13(__CONTEXT)	/* 13 - ST1 */
        //{ K_WRITE6270(__THISP_ 2, K_READNP(__THISP_ KA_IMM(__THISP))); }
        //static void OpcodeCall Opcode23(__CONTEXT)	/* 23 - ST2 */
        //{ K_WRITE6270(__THISP_ 3, K_READNP(__THISP_ KA_IMM(__THISP))); }
        //#endif

        /* --- STA --- */
        public void Opcode81(K6502_Context pc) { K_WRITE(pc, KA_INDX(pc), pc.A); }
        public void Opcode85(K6502_Context pc) { K_WRITE(pc, KA_ZP(pc), pc.A); }
        public void Opcode8D(K6502_Context pc) { K_WRITE(pc, KA_ABS(pc), pc.A); }
        public void Opcode91(K6502_Context pc) { K_WRITE(pc, KA_INDY(pc), pc.A); }
        public void Opcode95(K6502_Context pc) { K_WRITE(pc, KA_ZPX(pc), pc.A); }
        public void Opcode99(K6502_Context pc) { K_WRITE(pc, KA_ABSY(pc), pc.A); }
        public void Opcode9D(K6502_Context pc) { K_WRITE(pc, KA_ABSX(pc), pc.A); }
        //#define DEF_STA(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                                        {
        //                                                            K_WRITE##p##(__THISP_ a(__THISP), __THIS__.A); }
        //DEF_STA(81, NP, KA_INDX)	/* 81 - STA - (Indirect,X) */
        //DEF_STA(85, ZP, KA_ZP)  /* 85 - STA - Zero Page */
        //DEF_STA(8D, NP, KA_ABS) /* 8D - STA - Absolute */
        //DEF_STA(91, NP, KA_INDY)    /* 91 - STA - (Indirect),Y */
        //DEF_STA(95, ZP, KA_ZPX) /* 95 - STA - Zero Page,X */
        //DEF_STA(99, NP, KA_ABSY)    /* 99 - STA - Absolute,Y */
        //DEF_STA(9D, NP, KA_ABSX)    /* 9D - STA - Absolute,X */
        //#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_STA(92,NP,KA_IND)	/* 92 - STA - (Indirect) */
        //#endif

        /* --- STX ---  */
        public void Opcode86(K6502_Context pc) { K_WRITE(pc, KA_ZP(pc), pc.X); }
        public void Opcode8E(K6502_Context pc) { K_WRITE(pc, KA_ABS(pc), pc.X); }
        public void Opcode96(K6502_Context pc) { K_WRITE(pc, KA_ZPY(pc), pc.X); }
        //#define DEF_STX(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                                            {
        //                                                                K_WRITE##p##(__THISP_ a(__THISP), __THIS__.X); }
        //DEF_STX(86, ZP, KA_ZP)	/* 86 - STX - Zero Page */
        //DEF_STX(8E, NP, KA_ABS)	/* 8E - STX - Absolute */
        //DEF_STX(96, ZP, KA_ZPY)	/* 96 - STX - Zero Page,Y */

        /* --- STY ---  */
        public void Opcode84(K6502_Context pc) { K_WRITE(pc, KA_ZP(pc), pc.Y); }
        public void Opcode8C(K6502_Context pc) { K_WRITE(pc, KA_ABS(pc), pc.Y); }
        public void Opcode94(K6502_Context pc) { K_WRITE(pc, KA_ZPX(pc), pc.Y); }
        //#define DEF_STY(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //                                                                {
        //                                                                    K_WRITE##p##(__THISP_ a(__THISP), __THIS__.Y); }
        //DEF_STY(84, ZP, KA_ZP)	/* 84 - STY - Zero Page */
        //DEF_STY(8C, NP, KA_ABS)	/* 8C - STY - Absolute */
        //DEF_STY(94, ZP, KA_ZPX)	/* 94 - STY - Zero Page,X */

        //#if BUILD_HUC6280 || BUILD_M65C02
        ///* --- STZ ---  */
        //#define DEF_STZ(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ K_WRITE##p##(__THISP_ a(__THISP), 0); }
        //DEF_STZ(64,ZP,KA_ZP)	/* 64 - STZ - Zero Page */
        //DEF_STZ(9C,NP,KA_ABS)	/* 9C - STZ - Absolute */
        //DEF_STZ(74,ZP,KA_ZPX)	/* 74 - STZ - Zero Page,X */
        //DEF_STZ(9E,NP,KA_ABSX)	/* 9E - STZ - Absolute,X */
        //#endif

        //#if BUILD_HUC6280
        ///* --- TAMi ---  */
        //static void OpcodeCall Opcode53(__CONTEXT)	/* 53 - TAMi */
        //{ K_WRITEMPR(__THISP_ K_READNP(__THISP_ KA_IMM(__THISP)), __THIS__.A); }
        ///* --- TMAi ---  */
        //static void OpcodeCall Opcode43(__CONTEXT)	/* 43 - TMAi */
        //{ __THIS__.A = K_READMPR(__THISP_ K_READNP(__THISP_ KA_IMM(__THISP))); }
        //#endif

        //#if BUILD_HUC6280 || BUILD_M65C02
        ///* --- TRB --- */
        //#define DEF_TRB(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_TRB(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //DEF_TRB(14,ZP,KA_ZP)	/* 14 - TRB - Zero Page */
        //DEF_TRB(1C,NP,KA_ABS)	/* 1C - TRB - Absolute */
        ///* --- TSB --- */
        //#define DEF_TSB(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p##(__THISP_ adr, KM_TSB(__THISP_ K_READ##p##(__THISP_ adr))); \
        //}
        //DEF_TSB(04,ZP,KA_ZP)	/* 04 - TSB - Zero Page */
        //DEF_TSB(0C,NP,KA_ABS)	/* 0C - TSB - Absolute */
        //#endif

        //#if BUILD_HUC6280
        ///* --- TST --- */
        //#define DEF_TST(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //	Uword imm = K_READNP(__THISP_ KA_IMM(__THISP)); \
        //	KM_TST(__THISP_ imm, K_READ##p##(__THISP_ a(__THISP))); \
        //}
        //DEF_TST(83,ZP,KA_ZP)	/* 83 - TST - Zero Page */
        //DEF_TST(93,NP,KA_ABS)	/* 93 - TST - Absolute */
        //DEF_TST(A3,ZP,KA_ZPX)	/* A3 - TST - Zero Page,X */
        //DEF_TST(B3,NP,KA_ABSX)	/* B3 - TST - Absolute,X */
        //#endif

        /* --- TAX ---  */
        public void OpcodeAA(K6502_Context pc)  /* AA - TAX */
        { pc.X = KM_LD(pc, pc.A); }
        /* --- TAY ---  */
        public void OpcodeA8(K6502_Context pc)  /* A8 - TAY */
        { pc.Y = KM_LD(pc, pc.A); }
        /* --- TSX ---  */
        public void OpcodeBA(K6502_Context pc)  /* BA - TSX */
        { pc.X = KM_LD(pc, pc.S); }
        /* --- TXA ---  */
        public void Opcode8A(K6502_Context pc)  /* 8A - TXA */
        { pc.A = KM_LD(pc, pc.X); }
        /* --- TXS ---  */
        public void Opcode9A(K6502_Context pc)  /* 9A - TXS */
        { pc.S = pc.X; }
        /* --- TYA ---  */
        public void Opcode98(K6502_Context pc)  /* 98 - TYA */
        { pc.A = KM_LD(pc, pc.Y); }

        //#if BUILD_HUC6280
        //static void OpcodeCall Opcode73(__CONTEXT)	/* 73 - TII */
        //{
        //	Uword src,des,len;
        //	src = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	des = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	len = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	KI_ADDCLOCK(__THISP_ (Uword)(len ? len * 6 : (Uword)0x60000));
        //	do
        //	{
        //		K_WRITENP(__THISP_ des, K_READNP(__THISP_ src));
        //		src = RTO16(src + 1);
        //		des = RTO16(des + 1);
        //		len = RTO16(len - 1);
        //	} while (len != 0);
        //}
        //static void OpcodeCall OpcodeC3(__CONTEXT)	/* C3 - TDD */
        //{
        //	Uword src,des,len;
        //	src = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	des = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	len = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	KI_ADDCLOCK(__THISP_ (Uword)(len ? len * 6 : (Uword)0x60000));
        //	do
        //	{
        //		K_WRITENP(__THISP_ des, K_READNP(__THISP_ src));
        //		src = RTO16(src - 1);
        //		des = RTO16(des - 1);
        //		len = RTO16(len - 1);
        //	} while (len != 0);
        //}
        //static void OpcodeCall OpcodeD3(__CONTEXT)	/* D3 - TIN */
        //{
        //	Uword src,des,len;
        //	src = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	des = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	len = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	KI_ADDCLOCK(__THISP_ (Uword)(len ? len * 6 : (Uword)0x60000));
        //	do
        //	{
        //		K_WRITENP(__THISP_ des, K_READNP(__THISP_ src));
        //		src = RTO16(src + 1);
        //		len = RTO16(len - 1);
        //	} while (len != 0);
        //}
        //static void OpcodeCall OpcodeE3(__CONTEXT)	/* E3 - TIA */
        //{
        //	int add = +1;
        //	Uword src,des,len;
        //	src = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	des = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	len = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	KI_ADDCLOCK(__THISP_ (Uword)(len ? len * 6 : (Uword)0x60000));
        //	do
        //	{
        //		K_WRITENP(__THISP_ des, K_READNP(__THISP_ src));
        //		src = RTO16(src + 1);
        //		des = RTO16(des + add);
        //		add = -add;
        //		len = RTO16(len - 1);
        //	} while (len != 0);
        //}
        //static void OpcodeCall OpcodeF3(__CONTEXT)	/* F3 - TAI */
        //{
        //	int add = +1;
        //	Uword src,des,len;
        //	src = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	des = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	len = KI_READWORD(__THISP_ KA_IMM16(__THISP));
        //	KI_ADDCLOCK(__THISP_ (Uword)(len ? len * 6 : (Uword)0x60000));
        //	do
        //	{
        //		K_WRITENP(__THISP_ des, K_READNP(__THISP_ src));
        //		src = RTO16(src + add);
        //		des = RTO16(des + 1);
        //		add = -add;
        //		len = RTO16(len - 1);
        //	} while (len != 0);
        //}
        //static void OpcodeCall Opcode54(__CONTEXT)	/* 54 - CSL */
        //{ __THIS__.lowClockMode = 1; }
        //static void OpcodeCall OpcodeD4(__CONTEXT)	/* D4 - CSH */
        //{ __THIS__.lowClockMode = 0; }
        //#endif

        /* BS - implementing all illegal opcodes */
        //#if ILLEGAL_OPCODES

        /* --- KIL ---  */
        /* halts CPU */
        public void Opcode02(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode12(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode22(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode32(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode42(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode52(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode62(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode72(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void Opcode92(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void OpcodeB2(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void OpcodeD2(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        public void OpcodeF2(K6502_Context pc) { pc.PC = (UInt32)((pc.PC - 1) & 0xffff); pc.P |= I_FLAG; }
        //#define DEF_KIL(i) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    __THIS__.PC = RTO16(__THIS__.PC - 1); \
        //    __THIS__.P |= I_FLAG; /* disable interrupt */ \
        //}
        ///* opcodes */
        //DEF_KIL(02)
        //DEF_KIL(12)
        //DEF_KIL(22)
        //DEF_KIL(32)
        //DEF_KIL(42)
        //DEF_KIL(52)
        //DEF_KIL(62)
        //DEF_KIL(72)
        //DEF_KIL(92)
        //DEF_KIL(B2)
        //DEF_KIL(D2)
        //DEF_KIL(F2)

        /* --- NOP ---  */
        /* does nothing */
        public void Opcode80(K6502_Context pc) { KAI_IMM(pc); }
        public void Opcode82(K6502_Context pc) { KAI_IMM(pc); }
        public void OpcodeC2(K6502_Context pc) { KAI_IMM(pc); }
        public void OpcodeE2(K6502_Context pc) { KAI_IMM(pc); }
        public void Opcode04(K6502_Context pc) { KAI_ZP(pc); }
        public void Opcode14(K6502_Context pc) { KAI_ZPX(pc); }
        public void Opcode34(K6502_Context pc) { KAI_ZPX(pc); }
        public void Opcode44(K6502_Context pc) { KAI_ZP(pc); }
        public void Opcode54(K6502_Context pc) { KAI_ZPX(pc); }
        public void Opcode64(K6502_Context pc) { KAI_ZP(pc); }
        public void Opcode74(K6502_Context pc) { KAI_ZPX(pc); }
        public void OpcodeD4(K6502_Context pc) { KAI_ZPX(pc); }
        public void OpcodeF4(K6502_Context pc) { KAI_ZPX(pc); }
        public void Opcode89(K6502_Context pc) { KAI_IMM(pc); }
        public void Opcode1A(K6502_Context pc) { }
        public void Opcode3A(K6502_Context pc) { }
        public void Opcode5A(K6502_Context pc) { }
        public void Opcode7A(K6502_Context pc) { }
        public void OpcodeDA(K6502_Context pc) { }
        public void OpcodeFA(K6502_Context pc) { }
        public void Opcode0C(K6502_Context pc) { KAI_ABS(pc); }
        public void Opcode1C(K6502_Context pc) { KA_ABSX_(pc); }
        public void Opcode3C(K6502_Context pc) { KA_ABSX_(pc); }
        public void Opcode5C(K6502_Context pc) { KA_ABSX_(pc); }
        public void Opcode7C(K6502_Context pc) { KA_ABSX_(pc); }
        public void OpcodeDC(K6502_Context pc) { KA_ABSX_(pc); }
        public void OpcodeFC(K6502_Context pc) { KA_ABSX_(pc); }
        //#define DEF_NOP(i) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{}
        ///* fetches operands but does not use them, issues dummy reads (may have page boundary cycle penalty) */
        //#define DEF_NOP_A(i,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ a(__THISP); }
        ///* opcodes */
        //DEF_NOP_A(80,KAI_IMM);
        //DEF_NOP_A(82,KAI_IMM);
        //DEF_NOP_A(C2,KAI_IMM);
        //DEF_NOP_A(E2,KAI_IMM);
        //DEF_NOP_A(04,KAI_ZP);
        //DEF_NOP_A(14,KAI_ZPX);
        //DEF_NOP_A(34,KAI_ZPX);
        //DEF_NOP_A(44,KAI_ZP);
        //DEF_NOP_A(54,KAI_ZPX);
        //DEF_NOP_A(64,KAI_ZP);
        //DEF_NOP_A(74,KAI_ZPX);
        //DEF_NOP_A(D4,KAI_ZPX);
        //DEF_NOP_A(F4,KAI_ZPX);
        //DEF_NOP_A(89,KAI_IMM);
        //DEF_NOP(1A);
        //DEF_NOP(3A);
        //DEF_NOP(5A);
        //DEF_NOP(7A);
        //DEF_NOP(DA);
        //DEF_NOP(FA);
        //DEF_NOP_A(0C,KAI_ABS);
        //DEF_NOP_A(1C,KA_ABSX_);
        //DEF_NOP_A(3C,KA_ABSX_);
        //DEF_NOP_A(5C,KA_ABSX_);
        //DEF_NOP_A(7C,KA_ABSX_);
        //DEF_NOP_A(DC,KA_ABSX_);
        //DEF_NOP_A(FC,KA_ABSX_);

        /* --- SLO ---  */
        /* shift left, OR result */
        public UInt32 KM_SLO(K6502_Context pc, UInt32 src)
        {
            UInt32 w = (UInt32)((src << 1) & 0xff);
            pc.A |= w;
            pc.P &= (~(UInt32)(N_FLAG | Z_FLAG | C_FLAG));
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= (src >> 7) & C_FLAG;
            return w;
        }

        /* macro */
        public void Opcode03(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        public void Opcode13(K6502_Context pc) { UInt32 adr = KA_INDY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        public void Opcode07(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        public void Opcode17(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        public void Opcode1B(K6502_Context pc) { UInt32 adr = KA_ABSY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        public void Opcode0F(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        public void Opcode1F(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SLO(pc, src)); }
        //#define DEF_SLO(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    K_WRITE##p##(__THISP_ adr, KM_SLO(__THISP_ src)); \
        //}
        ///* opcodes */
        //DEF_SLO(03,NP,KA_INDX);
        //DEF_SLO(13,NP,KA_INDY);
        //DEF_SLO(07,ZP,KA_ZP);
        //DEF_SLO(17,ZP,KA_ZPX);
        //DEF_SLO(1B,NP,KA_ABSY);
        //DEF_SLO(0F,NP,KA_ABS);
        //DEF_SLO(1F,NP,KA_ABSX);

        /* --- RLA ---  */
        /* rotate left, AND result */
        public UInt32 KM_RLA(K6502_Context pc, UInt32 src)
        {
            UInt32 w = (UInt32)(((src << 1) | (pc.P & C_FLAG)) & 0xff);
            pc.A &= w;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= (src >> 7) & C_FLAG;
            return w;
        }

        /* macro */
        public void Opcode23(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }
        public void Opcode33(K6502_Context pc) { UInt32 adr = KA_INDY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }
        public void Opcode27(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }
        public void Opcode37(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }
        public void Opcode3B(K6502_Context pc) { UInt32 adr = KA_ABSY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }
        public void Opcode2F(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }
        public void Opcode3F(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RLA(pc, src)); }

        //#define DEF_RLA(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    K_WRITE##p##(__THISP_ adr, KM_RLA(__THISP_ src)); \
        //}
        ///* opcodes */
        //DEF_RLA(23,NP,KA_INDX);
        //DEF_RLA(33,NP,KA_INDY);
        //DEF_RLA(27,ZP,KA_ZP);
        //DEF_RLA(37,ZP,KA_ZPX);
        //DEF_RLA(3B,NP,KA_ABSY);
        //DEF_RLA(2F,NP,KA_ABS);
        //DEF_RLA(3F,NP,KA_ABSX);

        /* --- SRE ---  */
        /* shift right, EOR result */
        public UInt32 KM_SRE(K6502_Context pc, UInt32 src)
        {
            UInt32 w = (UInt32)((src >> 1) & 0xff);
            pc.A ^= w;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= src & C_FLAG;
            return w;
        }

        /* macro */
        public void Opcode43(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }
        public void Opcode53(K6502_Context pc) { UInt32 adr = KA_INDY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }
        public void Opcode47(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }
        public void Opcode57(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }
        public void Opcode5B(K6502_Context pc) { UInt32 adr = KA_ABSY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }
        public void Opcode4F(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }
        public void Opcode5F(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_SRE(pc, src)); }

        //#define DEF_SRE(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    K_WRITE##p##(__THISP_ adr, KM_SRE(__THISP_ src)); \
        //}
        ///* opcodes */
        //DEF_SRE(43,NP,KA_INDX);
        //DEF_SRE(53,NP,KA_INDY);
        //DEF_SRE(47,ZP,KA_ZP);
        //DEF_SRE(57,ZP,KA_ZPX);
        //DEF_SRE(5B,NP,KA_ABSY);
        //DEF_SRE(4F,NP,KA_ABS);
        //DEF_SRE(5F,NP,KA_ABSX);

        /* --- RRA ---  */
        /* rotate right, ADC result */
        public UInt32 KM_RRA(K6502_Context pc, UInt32 src)
        {
            UInt32 w = (UInt32)(((src >> 1) | ((pc.P & C_FLAG) << 7)) & 0xff);
            pc.P &= ~(UInt32)(C_FLAG);
            pc.P |= src & C_FLAG;
            KMI_ADC(pc, w);
            return w;
        }

        /* macro */
        public void Opcode63(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }
        public void Opcode73(K6502_Context pc) { UInt32 adr = KA_INDY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }
        public void Opcode67(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }
        public void Opcode77(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }
        public void Opcode7B(K6502_Context pc) { UInt32 adr = KA_ABSY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }
        public void Opcode6F(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }
        public void Opcode7F(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_RRA(pc, src)); }

        //#define DEF_RRA(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    K_WRITE##p##(__THISP_ adr, KM_RRA(__THISP_ src)); \
        //}
        ///* opcodes */
        //DEF_RRA(63,NP,KA_INDX);
        //DEF_RRA(73,NP,KA_INDY);
        //DEF_RRA(67,ZP,KA_ZP);
        //DEF_RRA(77,ZP,KA_ZPX);
        //DEF_RRA(7B,NP,KA_ABSY);
        //DEF_RRA(6F,NP,KA_ABS);
        //DEF_RRA(7F,NP,KA_ABSX);

        /* --- DCP ---  */
        /* decrement, CMP */
        public UInt32 KM_DCP(K6502_Context pc, UInt32 src)
        {
            UInt32 w = (UInt32)((src - 1) & 0xff);
            KM_CMP(pc, w);
            return w;
        }
        /* macro */
        public void OpcodeC3(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        public void OpcodeD3(K6502_Context pc) { UInt32 adr = KA_INDY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        public void OpcodeC7(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        public void OpcodeD7(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        public void OpcodeDB(K6502_Context pc) { UInt32 adr = KA_ABSY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        public void OpcodeCF(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        public void OpcodeDF(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_DCP(pc, src)); }
        //#define DEF_DCP(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    K_WRITE##p##(__THISP_ adr, KM_DCP(__THISP_ src)); \
        //}
        ///* opcodes */
        //DEF_DCP(C3,NP,KA_INDX);
        //DEF_DCP(D3,NP,KA_INDY);
        //DEF_DCP(C7,ZP,KA_ZP);
        //DEF_DCP(D7,ZP,KA_ZPX);
        //DEF_DCP(DB,NP,KA_ABSY);
        //DEF_DCP(CF,NP,KA_ABS);
        //DEF_DCP(DF,NP,KA_ABSX);

        /* --- ISC ---  */
        /* increment, SBC */
        public UInt32 KM_ISC(K6502_Context pc, UInt32 src)
        {
            UInt32 w = (UInt32)((src + 1) & 0xff);
            KMI_SBC(pc, w);
            return w;
        }
        /* macro */
        public void OpcodeE3(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        public void OpcodeF3(K6502_Context pc) { UInt32 adr = KA_INDY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        public void OpcodeE7(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        public void OpcodeF7(K6502_Context pc) { UInt32 adr = KA_ZPX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        public void OpcodeFB(K6502_Context pc) { UInt32 adr = KA_ABSY(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        public void OpcodeEF(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        public void OpcodeFF(K6502_Context pc) { UInt32 adr = KA_ABSX(pc); UInt32 src = K_READ(pc, adr); K_WRITE(pc, adr, KM_ISC(pc, src)); }
        //#define DEF_ISC(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    K_WRITE##p##(__THISP_ adr, KM_ISC(__THISP_ src)); \
        //}
        ///* opcodes */
        //DEF_ISC(E3,NP,KA_INDX);
        //DEF_ISC(F3,NP,KA_INDY);
        //DEF_ISC(E7,ZP,KA_ZP);
        //DEF_ISC(F7,ZP,KA_ZPX);
        //DEF_ISC(FB,NP,KA_ABSY);
        //DEF_ISC(EF,NP,KA_ABS);
        //DEF_ISC(FF,NP,KA_ABSX);

        /* --- LAX ---  */
        /* load A and X */
        public void KM_LAX(K6502_Context pc, UInt32 src)
        {
            pc.A = src;
            pc.X = src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)src);
        }

        /* macro */
        public void OpcodeA3(K6502_Context pc) { UInt32 adr = KA_INDX(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        public void OpcodeB3(K6502_Context pc) { UInt32 adr = KA_INDY_(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        public void OpcodeA7(K6502_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        public void OpcodeB7(K6502_Context pc) { UInt32 adr = KA_ZPY(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        public void OpcodeAB(K6502_Context pc) { UInt32 adr = KA_IMM(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        public void OpcodeAF(K6502_Context pc) { UInt32 adr = KA_ABS(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        public void OpcodeBF(K6502_Context pc) { UInt32 adr = KA_ABSY_(pc); UInt32 src = K_READ(pc, adr); KM_LAX(pc, src); }
        //#define DEF_LAX(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    Uword adr = a(__THISP); \
        //    Uword src = K_READ##p##(__THISP_ adr); \
        //    KM_LAX(__THISP_ src); \
        //}
        ///* opcodes */
        //DEF_LAX(A3,NP,KA_INDX);
        //DEF_LAX(B3,NP,KA_INDY_);
        //DEF_LAX(A7,ZP,KA_ZP);
        //DEF_LAX(B7,ZP,KA_ZPY);
        //DEF_LAX(AB,NP,KA_IMM); /* this one is unstable on hardware */
        //DEF_LAX(AF,NP,KA_ABS);
        //DEF_LAX(BF,NP,KA_ABSY_);

        /* --- SAX ---  */
        /* store A AND X */
        public void Opcode83(K6502_Context pc) { K_WRITE(pc, KA_INDX(pc), (pc.A & pc.X)); }
        public void Opcode87(K6502_Context pc) { K_WRITE(pc, KA_ZP(pc), (pc.A & pc.X)); }
        public void Opcode97(K6502_Context pc) { K_WRITE(pc, KA_ZPY(pc), (pc.A & pc.X)); }
        public void Opcode8F(K6502_Context pc) { K_WRITE(pc, KA_ABS(pc), (pc.A & pc.X)); }
        //#define DEF_SAX(i,p,a) static void OpcodeCall Opcode##i##(__CONTEXT) \
        //{ \
        //    K_WRITE##p##(__THISP_ a(__THISP), (__THIS__.A & __THIS__.X) ); \
        //}
        ///* opcodes */
        //DEF_SAX(83,NP,KA_INDX);
        //DEF_SAX(87,ZP,KA_ZP);
        //DEF_SAX(97,ZP,KA_ZPY);
        //DEF_SAX(8F,NP,KA_ABS);

        /* --- AHX ---  */
        /* store A AND X AND high address (somewhat unstable) */
        public void Opcode93(K6502_Context pc)
        {
            UInt32 adr = KA_ZPY(pc);
            K_WRITE(pc, adr, (UInt32)((pc.A & pc.X & ((adr >> 8) + 1)) & 0xff));
        }
        public void Opcode9F(K6502_Context pc)
        {
            UInt32 adr = KA_ABSY(pc);
            K_WRITE(pc, adr, (UInt32)((pc.A & pc.X & ((adr >> 8) + 1)) & 0xff));
        }

        /* --- TAS --- */
        /* transfer A AND X to S, store A AND X AND high address */
        public void Opcode9B(K6502_Context pc)
        {
            UInt32 adr = KA_ABSY(pc);
            pc.S = pc.A & pc.X;
            K_WRITE(pc, adr, (UInt32)((pc.S & ((adr >> 8) + 1)) & 0xff));
        }

        /* --- SHY --- */
        /* store Y AND high address (somewhat unstable) */
        public void Opcode9C(K6502_Context pc)
        {
            UInt32 adr = KA_ABSX(pc);
            K_WRITE(pc, adr, (UInt32)((pc.Y & ((adr >> 8) + 1)) & 0xff));
        }

        /* --- SHX --- */
        /* store X AND high address (somewhat unstable) */
        public void Opcode9E(K6502_Context pc)
        {
            UInt32 adr = KA_ABSY(pc);
            K_WRITE(pc, adr, (UInt32)((pc.X & ((adr >> 8) + 1)) & 0xff));
        }

        /* --- ANC --- */
        /* a = A AND immediate */
        public void Opcode0B(K6502_Context pc)
        {
            UInt32 adr = KA_IMM(pc);
            pc.A = (UInt32)((pc.A & K_READ(pc, adr)) & 0xff);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= (pc.A >> 7); /* C_FLAG */
        }

        public void Opcode2B(K6502_Context pc)
        {
            UInt32 adr = KA_IMM(pc);
            pc.A = (UInt32)((pc.A & K_READ(pc, adr)) & 0xff);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= (pc.A >> 7) & C_FLAG;
        }

        /* --- XAA --- */
        /* a = X AND immediate (unstable) */
        public void Opcode8B(K6502_Context pc)
        {
            UInt32 adr = KA_IMM(pc);
            pc.A = (UInt32)((pc.X & K_READ(pc, adr)) & 0xff);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
        }

        /* --- ALR --- */
        /* A AND immediate (unstable), shift right */
        public void Opcode4B(K6502_Context pc)
        {
            UInt32 adr = KA_IMM(pc);
            UInt32 res = (UInt32)((pc.A & K_READ(pc, adr)) & 0xff);
            pc.A = res >> 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= (res & C_FLAG);
        }

        /* --- ARR --- */
        /* A AND immediate (unstable), rotate right, weird carry */
        public void Opcode6B(K6502_Context pc)
        {
            UInt32 adr = KA_IMM(pc);
            UInt32 res = (UInt32)((pc.A & K_READ(pc, adr)) & 0xff);
            pc.A = (res >> 1) + ((pc.P & C_FLAG) << 7);
            pc.P &= ~(UInt32)(N_FLAG | V_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
            pc.P |= (res ^ (res >> 1)) & V_FLAG;
            pc.P |= (res >> 7) & C_FLAG;
        }

        /* --- LAS --- */
        /* stack AND immediate, copy to A and X */
        public void OpcodeBB(K6502_Context pc)
        {
            UInt32 adr = KA_ABSY_(pc);
            pc.S &= (UInt32)((K_READ(pc, adr)) & 0xff);
            pc.A = pc.S;
            pc.X = pc.S;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.A);
        }

        /* --- AXS --- */
        /* (A & X) - immediate, result in X */
        public void OpcodeCB(K6502_Context pc)
        {
            UInt32 adr = KA_IMM(pc);
            UInt32 res = (pc.A & pc.X) - (UInt32)(K_READ(pc, adr) & 0xff);
            pc.X = (UInt32)((res) & 0xff);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P |= (UInt32)FLAG_NZ((Int32)pc.X);
            pc.P |= (UInt32)((res <= 0xFF) ? C_FLAG : 0);
        }

        /* --- SBC --- */
        /* EB is alternate opcode for SBC E9 */
        public void OpcodeEB(K6502_Context pc)
        {
            OpcodeE9(pc);
        }

        //-------------------------------- km6502cd.h END

        //# include "km6502ct.h"
        //-------------------------------- km6502ct.h START
        //#if BUILD_HUC6280

        ///*

        // HuC6280 clock cycle table

        // -0         undefined OP-code
        // BRK(#$00)  +7 by interrupt

        //*/
        //const static Ubyte cl_table[256] = {
        ///* L 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F     H */
        //	 1, 7, 3, 4, 6, 4, 6, 7, 3, 2, 2,-0, 7, 5, 7, 6, /* 0 */
        //	 2, 7, 7, 4, 6, 4, 6, 7, 2, 5, 2,-0, 7, 5, 7, 6, /* 1 */
        //	 7, 7, 3, 4, 4, 4, 6, 7, 3, 2, 2,-0, 5, 5, 7, 6, /* 2 */
        //	 2, 7, 7,-0, 4, 4, 6, 7, 2, 5, 2,-0, 5, 5, 7, 6, /* 3 */
        //	 7, 7, 3, 4, 8, 4, 6, 7, 3, 2, 2,-0, 4, 5, 7, 6, /* 4 */
        //	 2, 7, 7, 5,2 , 4, 6, 7, 2, 5, 3,-0,-0, 5, 7, 6, /* 5 */
        //	 7, 7, 2,-0, 4, 4, 6, 7, 3, 2, 2,-0, 7, 5, 7, 6, /* 6 */
        //	 2, 7, 7,17, 4, 4, 6, 7, 2, 5, 3,-0, 7, 5, 7, 6, /* 7 */
        //	 2, 7, 2, 7, 4, 4, 4, 7, 2, 2, 2,-0, 5, 5, 5, 6, /* 8 */
        //	 2, 7, 7, 8, 4, 4, 4, 7, 2, 5, 2,-0, 5, 5, 5, 6, /* 9 */
        //	 2, 7, 2, 7, 4, 4, 4, 7, 2, 2, 2,-0, 5, 5, 5, 6, /* A */
        //	 2, 7, 7, 8, 4, 4, 4, 7, 2, 5, 2,-0, 5, 5, 5, 6, /* B */
        //	 2, 7, 2,17, 4, 4, 6, 7, 2, 2, 2,-0, 5, 5, 7, 6, /* C */
        //	 2, 7, 7,17,2 , 4, 6, 7, 2, 5, 3,-0,-0, 5, 7, 6, /* D */
        //	 2, 7,-0,17, 4, 4, 6, 7, 2, 2, 2,-0, 5, 5, 7, 6, /* E */
        //	 2, 7, 7,17, 2, 4, 6, 7, 2, 5, 3,-0,-0, 5, 7, 6, /* F */
        //};
        //#elif BUILD_M65C02

        ///*

        // m65c02 clock cycle table (incomplete)

        // -0         undefined OP-code
        // +n         +1 by page boundary
        // BRK(#$00)  +7 by interrupt

        //*/
        //const static Ubyte cl_table[256] = {
        ///* L 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F     H */
        //	 0, 6,-0,-0,-0, 3, 5,-0, 3, 2, 2,-0,-0, 4, 6,-0, /* 0 */
        //	 2,+5,-0,-0,-0, 4, 6,-0, 2,+4, 2,-0,-0,+4, 7,-0, /* 1 */
        //	 6, 6,-0,-0, 3, 3, 5,-0, 4, 2, 2,-0, 4, 4, 6,-0, /* 2 */
        //	 2,+5,-0,-0, 4, 4, 6,-0, 2,+4, 2,-0,+4,+4, 7,-0, /* 3 */
        //	 6, 6,-0,-0,-0, 3, 5,-0, 3, 2, 2,-0, 3, 4, 6,-0, /* 4 */
        //	 2,+5,-0,-0,-0, 4, 6,-0, 2,+4,-0,-0,-0,+4, 7,-0, /* 5 */
        //	 6, 6,-0,-0,-0, 3, 5,-0, 4, 2, 2,-0, 5, 4, 6,-0, /* 6 */
        //	 2,+5,-0,-0,-0, 4, 6,-0, 2,+4,-0,-0,-0,+4, 7,-0, /* 7 */
        //	 2, 6,-0,-0, 3, 3, 3,-0, 2,-0, 2,-0, 4, 4, 4,-0, /* 8 */
        //	 2, 6,-0,-0, 4, 4, 4,-0, 2, 5, 2,-0,-0, 5,-0,-0, /* 9 */
        //	 2, 6, 2,-0, 3, 3, 3,-0, 2, 2, 2,-0, 4, 4, 4,-0, /* A */
        //	 2,+5,-0,-0, 4, 4, 4,-0, 2,+4, 2,-0,+4,+4,+4,-0, /* B */
        //	 2, 6,-0,-0, 3, 3, 5,-0, 2, 2, 2,-0, 4, 4, 6,-0, /* C */
        //	 2,+5,-0,-0,-0, 4, 6,-0, 2,+4,-0,-0,-0,+4, 7,-0, /* D */
        //	 2, 6,-0,-0, 3, 3, 5,-0, 2, 2, 2,-0, 4, 4, 6,-0, /* E */
        //	 2,+5,-0,-0,-0, 4, 6,-0, 2,+4,-0,-0,-0,+4, 7,-0, /* F */
        //};

        //#else

        /*

         m6502 clock cycle table

         (n)        undefined OP-code
         +n         +1 by page boundary case
         BRK(#$00)  +7 by interrupt

         BS - corrected NOP timings for undefined opcodes

        */
        public Byte[] cl_table = new byte[256]{
/* L 0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F      H */
	 0 , 6 ,(2),(8),(3), 3 , 5 ,(5), 3 , 2 , 2 ,(2),(4), 4 , 6 ,(6), /* 0 */
	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(4),+4 , 7 ,(7), /* 1 */
	 6 , 6 ,(2),(8), 3 , 3 , 5 ,(5), 4 , 2 , 2 ,(2), 4 , 4 , 6 ,(6), /* 2 */
	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(4),+4 , 7 ,(7), /* 3 */
	 6 , 6 ,(2),(8),(3), 3 , 5 ,(5), 3 , 2 , 2 ,(2), 3 , 4 , 6 ,(6), /* 4 */
	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(4),+4 , 7 ,(7), /* 5 */
	 6 , 6 ,(2),(8),(3), 3 , 5 ,(5), 4 , 2 , 2 ,(2), 5 , 4 , 6 ,(6), /* 6 */
	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(4),+4 , 7 ,(7), /* 7 */
	(2), 6 ,(2),(6), 3 , 3 , 3 ,(3), 2 ,(2), 2 ,(2), 4 , 4 , 4 ,(4), /* 8 */
	 2 , 6 ,(2),(6), 4 , 4 , 4 ,(4), 2 , 5 , 2 ,(5),(5), 5 ,(5),(5), /* 9 */
	 2 , 6 , 2 ,(6), 3 , 3 , 3 ,(3), 2 , 2 , 2 ,(2), 4 , 4 , 4 ,(4), /* A */
	 2 ,+5 ,(2),(5), 4 , 4 , 4 ,(4), 2 ,+4 , 2 ,(4),+4 ,+4 ,+4 ,(4), /* B */
	 2 , 6 ,(2),(8), 3 , 3 , 5 ,(5), 2 , 2 , 2 ,(2), 4 , 4 , 6 ,(6), /* C */
	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(4),+4 , 7 ,(7), /* D */
	 2 , 6 ,(2),(8), 3 , 3 , 5 ,(5), 2 , 2 , 2 ,(2), 4 , 4 , 6 ,(6), /* E */
	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(4),+4 , 7 ,(7), /* F */
};
        //#endif

        //-------------------------------- km6502ct.h END

        //# include "km6502ot.h"
        //-------------------------------- km6502ot.h START
        //#define OP__(i) \
        //	case 0x##i##: \
        //		Opcode##i##(__THISP); \
        //		break;

        /* BS - option to disable decimal execution for NES */
        //#if DISABLE_DECIMAL
        //#define OP_d OP__
        //#else
        //#define OP_d(i) \
        //	case 0x##i##: \
        //		if (__THIS__.P & D_FLAG) \
        //			D_Opco##i##(__THISP); \
        //		else \
        //			Opcode##i##(__THISP); \
        //		break;
        //#endif

        //#if BUILD_HUC6280
        //#define OPtd(i) \
        //	case 0x##i##: \
        //		if (__THIS__.P & T_FLAG) \
        //			if (__THIS__.P & D_FLAG) \
        //				TD_Opc##i##(__THISP); \
        //			else \
        //				T_Opco##i##(__THISP); \
        //		else \
        //			if (__THIS__.P & D_FLAG) \
        //				D_Opco##i##(__THISP); \
        //			else \
        //				Opcode##i##(__THISP); \
        //		break;
        //#define OPt_(i) \
        //	case 0x##i##: \
        //		if (__THIS__.P & T_FLAG) \
        //			T_Opco##i##(__THISP); \
        //		else \
        //			Opcode##i##(__THISP); \
        //		break;
        //#else
        //#define OPtd OP_d
        //#define OPt_ OP__
        //#endif

        /* BS - allowing illegal opcode implementation */
        //#if ILLEGAL_OPCODES
        //#define OPxx(i) \
        //	case 0x##i##: \
        //		Opcode##i##(__THISP); \
        //		__THIS__.illegal = 1; \
        //		break;
        //#else
        //#define OPxx(i)
        //#endif

        public void K_OPEXEC(K6502_Context pc)
        {
            UInt32 opcode = pc.lastcode = K_READ(pc, KAI_IMM(pc));
            KI_ADDCLOCK(pc, cl_table[opcode]);
            switch (opcode)
            {
                //OP__(00)    OPt_(01)    OPxx(02)    OPxx(03)    OPxx(04)    OPt_(05)    OP__(06)    OPxx(07)
                //OP__(08)    OPt_(09)    OP__(0A)    OPxx(0B)    OPxx(0C)    OPt_(0D)    OP__(0E)    OPxx(0F)
                case 0x00: Opcode00(pc); break;
                case 0x01: Opcode01(pc); break;
                case 0x02: Opcode02(pc); pc.illegal = 1; break;
                case 0x03: Opcode03(pc); pc.illegal = 1; break;
                case 0x04: Opcode04(pc); pc.illegal = 1; break;
                case 0x05: Opcode05(pc); break;
                case 0x06: Opcode06(pc); break;
                case 0x07: Opcode07(pc); pc.illegal = 1; break;
                case 0x08: Opcode08(pc); break;
                case 0x09: Opcode09(pc); break;
                case 0x0A: Opcode0A(pc); break;
                case 0x0B: Opcode0B(pc); pc.illegal = 1; break;
                case 0x0C: Opcode0C(pc); pc.illegal = 1; break;
                case 0x0D: Opcode0D(pc); break;
                case 0x0E: Opcode0E(pc); break;
                case 0x0F: Opcode0F(pc); pc.illegal = 1; break;

                //OP__(10)    OPt_(11)    OPxx(12)    OPxx(13)    OPxx(14)    OPt_(15)    OP__(16)    OPxx(17)
                //OP__(18)    OPt_(19)    OPxx(1A)    OPxx(1B)    OPxx(1C)    OPt_(1D)    OP__(1E)    OPxx(1F)
                case 0x10: Opcode10(pc); break;
                case 0x11: Opcode11(pc); break;
                case 0x12: Opcode12(pc); pc.illegal = 1; break;
                case 0x13: Opcode13(pc); pc.illegal = 1; break;
                case 0x14: Opcode14(pc); pc.illegal = 1; break;
                case 0x15: Opcode15(pc); break;
                case 0x16: Opcode16(pc); break;
                case 0x17: Opcode17(pc); pc.illegal = 1; break;
                case 0x18: Opcode18(pc); break;
                case 0x19: Opcode19(pc); break;
                case 0x1A: Opcode1A(pc); pc.illegal = 1; break;
                case 0x1B: Opcode1B(pc); pc.illegal = 1; break;
                case 0x1C: Opcode1C(pc); pc.illegal = 1; break;
                case 0x1D: Opcode1D(pc); break;
                case 0x1E: Opcode1E(pc); break;
                case 0x1F: Opcode1F(pc); pc.illegal = 1; break;

                //OP__(20)    OP__(21)    OPxx(22)    OPxx(23)    OP__(24)    OP__(25)    OP__(26)    OPxx(27)
                //OP__(28)    OP__(29)    OP__(2A)    OPxx(2B)    OP__(2C)    OP__(2D)    OP__(2E)    OPxx(2F)
                case 0x20: Opcode20(pc); break;
                case 0x21: Opcode21(pc); break;
                case 0x22: Opcode22(pc); pc.illegal = 1; break;
                case 0x23: Opcode23(pc); pc.illegal = 1; break;
                case 0x24: Opcode24(pc); break;
                case 0x25: Opcode25(pc); break;
                case 0x26: Opcode26(pc); break;
                case 0x27: Opcode27(pc); pc.illegal = 1; break;
                case 0x28: Opcode28(pc); break;
                case 0x29: Opcode29(pc); break;
                case 0x2A: Opcode2A(pc); break;
                case 0x2B: Opcode2B(pc); pc.illegal = 1; break;
                case 0x2C: Opcode2C(pc); break;
                case 0x2D: Opcode2D(pc); break;
                case 0x2E: Opcode2E(pc); break;
                case 0x2F: Opcode2F(pc); pc.illegal = 1; break;

                //OP__(30)    OPt_(31)    OPxx(32)    OPxx(33)    OPxx(34)    OPt_(35)    OP__(36)    OPxx(37)
                //OP__(38)    OPt_(39)    OPxx(3A)    OPxx(3B)    OPxx(3C)    OPt_(3D)    OP__(3E)    OPxx(3F)
                case 0x30: Opcode30(pc); break;
                case 0x31: Opcode31(pc); break;
                case 0x32: Opcode32(pc); pc.illegal = 1; break;
                case 0x33: Opcode33(pc); pc.illegal = 1; break;
                case 0x34: Opcode34(pc); pc.illegal = 1; break;
                case 0x35: Opcode35(pc); break;
                case 0x36: Opcode36(pc); break;
                case 0x37: Opcode37(pc); pc.illegal = 1; break;
                case 0x38: Opcode38(pc); break;
                case 0x39: Opcode39(pc); break;
                case 0x3A: Opcode3A(pc); pc.illegal = 1; break;
                case 0x3B: Opcode3B(pc); pc.illegal = 1; break;
                case 0x3C: Opcode3C(pc); pc.illegal = 1; break;
                case 0x3D: Opcode3D(pc); break;
                case 0x3E: Opcode3E(pc); break;
                case 0x3F: Opcode3F(pc); pc.illegal = 1; break;

                //OP__(40)    OPt_(41)    OPxx(42)    OPxx(43)    OPxx(44)    OPt_(45)    OP__(46)    OPxx(47)
                //OP__(48)    OPt_(49)    OP__(4A)    OPxx(4B)    OP__(4C)    OPt_(4D)    OP__(4E)    OPxx(4F)
                case 0x40: Opcode40(pc); break;
                case 0x41: Opcode41(pc); break;
                case 0x42: Opcode42(pc); pc.illegal = 1; break;
                case 0x43: Opcode43(pc); pc.illegal = 1; break;
                case 0x44: Opcode44(pc); pc.illegal = 1; break;
                case 0x45: Opcode45(pc); break;
                case 0x46: Opcode46(pc); break;
                case 0x47: Opcode47(pc); pc.illegal = 1; break;
                case 0x48: Opcode48(pc); break;
                case 0x49: Opcode49(pc); break;
                case 0x4A: Opcode4A(pc); break;
                case 0x4B: Opcode4B(pc); pc.illegal = 1; break;
                case 0x4C: Opcode4C(pc); break;
                case 0x4D: Opcode4D(pc); break;
                case 0x4E: Opcode4E(pc); break;
                case 0x4F: Opcode4F(pc); pc.illegal = 1; break;

                //OP__(50)    OPt_(51)    OPxx(52)    OPxx(53)    OPxx(54)    OPt_(55)    OP__(56)    OPxx(57)
                //OP__(58)    OPt_(59)    OPxx(5A)    OPxx(5B)    OPxx(5C)    OPt_(5D)    OP__(5E)    OPxx(5F)
                case 0x50: Opcode50(pc); break;
                case 0x51: Opcode51(pc); break;
                case 0x52: Opcode52(pc); pc.illegal = 1; break;
                case 0x53: Opcode53(pc); pc.illegal = 1; break;
                case 0x54: Opcode54(pc); pc.illegal = 1; break;
                case 0x55: Opcode55(pc); break;
                case 0x56: Opcode56(pc); break;
                case 0x57: Opcode57(pc); pc.illegal = 1; break;
                case 0x58: Opcode58(pc); break;
                case 0x59: Opcode59(pc); break;
                case 0x5A: Opcode5A(pc); pc.illegal = 1; break;
                case 0x5B: Opcode5B(pc); pc.illegal = 1; break;
                case 0x5C: Opcode5C(pc); pc.illegal = 1; break;
                case 0x5D: Opcode5D(pc); break;
                case 0x5E: Opcode5E(pc); break;
                case 0x5F: Opcode5F(pc); pc.illegal = 1; break;

                //OP__(60)    OPtd(61)    OPxx(62)    OPxx(63)    OPxx(64)    OPtd(65)    OP__(66)    OPxx(67)
                //OP__(68)    OPtd(69)    OP__(6A)    OPxx(6B)    OP__(6C)    OPtd(6D)    OP__(6E)    OPxx(6F)
                case 0x60: Opcode60(pc); break;
                case 0x61: Opcode61(pc); break;
                case 0x62: Opcode62(pc); pc.illegal = 1; break;
                case 0x63: Opcode63(pc); pc.illegal = 1; break;
                case 0x64: Opcode64(pc); pc.illegal = 1; break;
                case 0x65: Opcode65(pc); break;
                case 0x66: Opcode66(pc); break;
                case 0x67: Opcode67(pc); pc.illegal = 1; break;
                case 0x68: Opcode68(pc); break;
                case 0x69: Opcode69(pc); break;
                case 0x6A: Opcode6A(pc); break;
                case 0x6B: Opcode6B(pc); pc.illegal = 1; break;
                case 0x6C: Opcode6C(pc); break;
                case 0x6D: Opcode6D(pc); break;
                case 0x6E: Opcode6E(pc); break;
                case 0x6F: Opcode6F(pc); pc.illegal = 1; break;

                //OP__(70)    OPtd(71)    OPxx(72)    OPxx(73)    OPxx(74)    OPtd(75)    OP__(76)    OPxx(77)
                //OP__(78)    OPtd(79)    OPxx(7A)    OPxx(7B)    OPxx(7C)    OPtd(7D)    OP__(7E)    OPxx(7F)
                case 0x70: Opcode70(pc); break;
                case 0x71: Opcode71(pc); break;
                case 0x72: Opcode72(pc); pc.illegal = 1; break;
                case 0x73: Opcode73(pc); pc.illegal = 1; break;
                case 0x74: Opcode74(pc); pc.illegal = 1; break;
                case 0x75: Opcode75(pc); break;
                case 0x76: Opcode76(pc); break;
                case 0x77: Opcode77(pc); pc.illegal = 1; break;
                case 0x78: Opcode78(pc); break;
                case 0x79: Opcode79(pc); break;
                case 0x7A: Opcode7A(pc); pc.illegal = 1; break;
                case 0x7B: Opcode7B(pc); pc.illegal = 1; break;
                case 0x7C: Opcode7C(pc); pc.illegal = 1; break;
                case 0x7D: Opcode7D(pc); break;
                case 0x7E: Opcode7E(pc); break;
                case 0x7F: Opcode7F(pc); pc.illegal = 1; break;

                //OPxx(80)    OP__(81)    OPxx(82)    OPxx(83)    OP__(84)    OP__(85)    OP__(86)    OPxx(87)
                //OP__(88)    OPxx(89)    OP__(8A)    OPxx(8B)    OP__(8C)    OP__(8D)    OP__(8E)    OPxx(8F)
                case 0x80: Opcode80(pc); pc.illegal = 1; break;
                case 0x81: Opcode81(pc); break;
                case 0x82: Opcode82(pc); pc.illegal = 1; break;
                case 0x83: Opcode83(pc); pc.illegal = 1; break;
                case 0x84: Opcode84(pc); break;
                case 0x85: Opcode85(pc); break;
                case 0x86: Opcode86(pc); break;
                case 0x87: Opcode87(pc); pc.illegal = 1; break;
                case 0x88: Opcode88(pc); break;
                case 0x89: Opcode89(pc); pc.illegal = 1; break;
                case 0x8A: Opcode8A(pc); break;
                case 0x8B: Opcode8B(pc); pc.illegal = 1; break;
                case 0x8C: Opcode8C(pc); break;
                case 0x8D: Opcode8D(pc); break;
                case 0x8E: Opcode8E(pc); break;
                case 0x8F: Opcode8F(pc); pc.illegal = 1; break;

                //OP__(90)    OP__(91)    OPxx(92)    OPxx(93)    OP__(94)    OP__(95)    OP__(96)    OPxx(97)
                //OP__(98)    OP__(99)    OP__(9A)    OPxx(9B)    OPxx(9C)    OP__(9D)    OPxx(9E)    OPxx(9F)
                case 0x90: Opcode90(pc); break;
                case 0x91: Opcode91(pc); break;
                case 0x92: Opcode92(pc); pc.illegal = 1; break;
                case 0x93: Opcode93(pc); pc.illegal = 1; break;
                case 0x94: Opcode94(pc); break;
                case 0x95: Opcode95(pc); break;
                case 0x96: Opcode96(pc); break;
                case 0x97: Opcode97(pc); pc.illegal = 1; break;
                case 0x98: Opcode98(pc); break;
                case 0x99: Opcode99(pc); break;
                case 0x9A: Opcode9A(pc); break;
                case 0x9B: Opcode9B(pc); pc.illegal = 1; break;
                case 0x9C: Opcode9C(pc); pc.illegal = 1; break;
                case 0x9D: Opcode9D(pc); break;
                case 0x9E: Opcode9E(pc); pc.illegal = 1; break;
                case 0x9F: Opcode9F(pc); pc.illegal = 1; break;

                //OP__(A0)    OP__(A1)    OP__(A2)    OPxx(A3)    OP__(A4)    OP__(A5)    OP__(A6)    OPxx(A7)
                //OP__(A8)    OP__(A9)    OP__(AA)    OPxx(AB)    OP__(AC)    OP__(AD)    OP__(AE)    OPxx(AF)
                case 0xA0: OpcodeA0(pc); break;
                case 0xA1: OpcodeA1(pc); break;
                case 0xA2: OpcodeA2(pc); break;
                case 0xA3: OpcodeA3(pc); pc.illegal = 1; break;
                case 0xA4: OpcodeA4(pc); break;
                case 0xA5: OpcodeA5(pc); break;
                case 0xA6: OpcodeA6(pc); break;
                case 0xA7: OpcodeA7(pc); pc.illegal = 1; break;
                case 0xA8: OpcodeA8(pc); break;
                case 0xA9: OpcodeA9(pc); break;
                case 0xAA: OpcodeAA(pc); break;
                case 0xAB: OpcodeAB(pc); pc.illegal = 1; break;
                case 0xAC: OpcodeAC(pc); break;
                case 0xAD: OpcodeAD(pc); break;
                case 0xAE: OpcodeAE(pc); break;
                case 0xAF: OpcodeAF(pc); pc.illegal = 1; break;

                //OP__(B0)    OP__(B1)    OPxx(B2)    OPxx(B3)    OP__(B4)    OP__(B5)    OP__(B6)    OPxx(B7)
                //OP__(B8)    OP__(B9)    OP__(BA)    OPxx(BB)    OP__(BC)    OP__(BD)    OP__(BE)    OPxx(BF)
                case 0xB0: OpcodeB0(pc); break;
                case 0xB1: OpcodeB1(pc); break;
                case 0xB2: OpcodeB2(pc); pc.illegal = 1; break;
                case 0xB3: OpcodeB3(pc); pc.illegal = 1; break;
                case 0xB4: OpcodeB4(pc); break;
                case 0xB5: OpcodeB5(pc); break;
                case 0xB6: OpcodeB6(pc); break;
                case 0xB7: OpcodeB7(pc); pc.illegal = 1; break;
                case 0xB8: OpcodeB8(pc); break;
                case 0xB9: OpcodeB9(pc); break;
                case 0xBA: OpcodeBA(pc); break;
                case 0xBB: OpcodeBB(pc); pc.illegal = 1; break;
                case 0xBC: OpcodeBC(pc); break;
                case 0xBD: OpcodeBD(pc); break;
                case 0xBE: OpcodeBE(pc); break;
                case 0xBF: OpcodeBF(pc); pc.illegal = 1; break;

                //OP__(C0)    OP__(C1)    OPxx(C2)    OPxx(C3)    OP__(C4)    OP__(C5)    OP__(C6)    OPxx(C7)
                //OP__(C8)    OP__(C9)    OP__(CA)    OPxx(CB)    OP__(CC)    OP__(CD)    OP__(CE)    OPxx(CF)
                case 0xC0: OpcodeC0(pc); break;
                case 0xC1: OpcodeC1(pc); break;
                case 0xC2: OpcodeC2(pc); pc.illegal = 1; break;
                case 0xC3: OpcodeC3(pc); pc.illegal = 1; break;
                case 0xC4: OpcodeC4(pc); break;
                case 0xC5: OpcodeC5(pc); break;
                case 0xC6: OpcodeC6(pc); break;
                case 0xC7: OpcodeC7(pc); pc.illegal = 1; break;
                case 0xC8: OpcodeC8(pc); break;
                case 0xC9: OpcodeC9(pc); break;
                case 0xCA: OpcodeCA(pc); break;
                case 0xCB: OpcodeCB(pc); pc.illegal = 1; break;
                case 0xCC: OpcodeCC(pc); break;
                case 0xCD: OpcodeCD(pc); break;
                case 0xCE: OpcodeCE(pc); break;
                case 0xCF: OpcodeCF(pc); pc.illegal = 1; break;

                //OP__(D0)    OP__(D1)    OPxx(D2)    OPxx(D3)    OPxx(D4)    OP__(D5)    OP__(D6)    OPxx(D7)
                //OP__(D8)    OP__(D9)    OPxx(DA)    OPxx(DB)    OPxx(DC)    OP__(DD)    OP__(DE)    OPxx(DF)
                case 0xD0: OpcodeD0(pc); break;
                case 0xD1: OpcodeD1(pc); break;
                case 0xD2: OpcodeD2(pc); pc.illegal = 1; break;
                case 0xD3: OpcodeD3(pc); pc.illegal = 1; break;
                case 0xD4: OpcodeD4(pc); pc.illegal = 1; break;
                case 0xD5: OpcodeD5(pc); break;
                case 0xD6: OpcodeD6(pc); break;
                case 0xD7: OpcodeD7(pc); pc.illegal = 1; break;
                case 0xD8: OpcodeD8(pc); break;
                case 0xD9: OpcodeD9(pc); break;
                case 0xDA: OpcodeDA(pc); pc.illegal = 1; break;
                case 0xDB: OpcodeDB(pc); pc.illegal = 1; break;
                case 0xDC: OpcodeDC(pc); pc.illegal = 1; break;
                case 0xDD: OpcodeDD(pc); break;
                case 0xDE: OpcodeDE(pc); break;
                case 0xDF: OpcodeDF(pc); pc.illegal = 1; break;

                //OP__(E0)    OP_d(E1)    OPxx(E2)    OPxx(E3)    OP__(E4)    OP_d(E5)    OP__(E6)    OPxx(E7)
                //OP__(E8)    OP_d(E9)    OP__(EA)    OPxx(EB)    OP__(EC)    OP_d(ED)    OP__(EE)    OPxx(EF)
                case 0xE0: OpcodeE0(pc); break;
                case 0xE1: OpcodeE1(pc); break;
                case 0xE2: OpcodeE2(pc); pc.illegal = 1; break;
                case 0xE3: OpcodeE3(pc); pc.illegal = 1; break;
                case 0xE4: OpcodeE4(pc); break;
                case 0xE5: OpcodeE5(pc); break;
                case 0xE6: OpcodeE6(pc); break;
                case 0xE7: OpcodeE7(pc); pc.illegal = 1; break;
                case 0xE8: OpcodeE8(pc); break;
                case 0xE9: OpcodeE9(pc); break;
                case 0xEA: OpcodeEA(pc); break;
                case 0xEB: OpcodeEB(pc); pc.illegal = 1; break;
                case 0xEC: OpcodeEC(pc); break;
                case 0xED: OpcodeED(pc); break;
                case 0xEE: OpcodeEE(pc); break;
                case 0xEF: OpcodeEF(pc); pc.illegal = 1; break;

                //OP__(F0)    OP_d(F1)    OPxx(F2)    OPxx(F3)    OPxx(F4)    OP_d(F5)    OP__(F6)    OPxx(F7)
                //OP__(F8)    OP_d(F9)    OPxx(FA)    OPxx(FB)    OPxx(FC)    OP_d(FD)    OP__(FE)    OPxx(FF)
                case 0xF0: OpcodeF0(pc); break;
                case 0xF1: OpcodeF1(pc); break;
                case 0xF2: OpcodeF2(pc); pc.illegal = 1; break;
                case 0xF3: OpcodeF3(pc); pc.illegal = 1; break;
                case 0xF4: OpcodeF4(pc); pc.illegal = 1; break;
                case 0xF5: OpcodeF5(pc); break;
                case 0xF6: OpcodeF6(pc); break;
                case 0xF7: OpcodeF7(pc); pc.illegal = 1; break;
                case 0xF8: OpcodeF8(pc); break;
                case 0xF9: OpcodeF9(pc); break;
                case 0xFA: OpcodeFA(pc); pc.illegal = 1; break;
                case 0xFB: OpcodeFB(pc); pc.illegal = 1; break;
                case 0xFC: OpcodeFC(pc); pc.illegal = 1; break;
                case 0xFD: OpcodeFD(pc); break;
                case 0xFE: OpcodeFE(pc); break;
                case 0xFF: OpcodeFF(pc); pc.illegal = 1; break;

                    //#if BUILD_HUC6280 || BUILD_M65C02
                    //		OP__(34)	/* 34 - BIT - Zero Page,X */
                    //		OP__(3C)	/* 3C - BIT - Absolute,X */
                    //		OP__(80)	/* 80 - BRA */
                    //		OP__(3A)	/* 3A - DEA */
                    //		OP__(1A)	/* 1A - INA */



                    //		OP__(89)	/* 89 - BIT - Immediate */

                    //		OP__(04)	OP__(0C)	/* TSB */
                    //		OP__(14)	OP__(1C)	/* TRB */

                    //		OPt_(12)	/* 12 - ORA - (Indirect) */
                    //		OPt_(32)	/* 32 - AND - (Indirect) */
                    //		OPt_(52)	/* 52 - EOR - (Indirect) */
                    //		OPtd(72)	/* 72 - ADC - (Indirect) */
                    //		OP__(92)	/* 92 - STA - (Indirect) */
                    //		OP__(B2)	/* B2 - LDA - (Indirect) */
                    //		OP__(D2)	/* D2 - CMP - (Indirect) */
                    //		OP_d(F2)	/* F2 - SBC - (Indirect) */

                    //		OP__(DA)	OP__(5A)	OP__(FA)	OP__(7A)	/* PHX PHY PLX PLY */
                    //		OP__(64)	OP__(9C)	OP__(74)	OP__(9E)	/* STZ */
                    //		OP__(7C)	/* 7C - JMP - Absolute,X */
                    //#endif
                    //#if 0 && BUILD_M65C02
                    //		OP__(CB)	/* WAI */
                    //		OP__(DB)	/* STP */
                    //#endif
                    //#if BUILD_HUC6280
                    //		OP__(0F)	OP__(1F)	OP__(2F)	OP__(3F)	/* BBRi */
                    //		OP__(4F)	OP__(5F)	OP__(6F)	OP__(7F)
                    //		OP__(8F)	OP__(9F)	OP__(AF)	OP__(BF)	/* BBSi*/
                    //		OP__(CF)	OP__(DF)	OP__(EF)	OP__(FF)
                    //		OP__(44)	/* 44 - BSR */
                    //		OP__(62)	OP__(82)	OP__(C2)	/* CLA CLX CLY */
                    //		OP__(07)	OP__(17)	OP__(27)	OP__(37)	/* RMBi */
                    //		OP__(47)	OP__(57)	OP__(67)	OP__(77)
                    //		OP__(87)	OP__(97)	OP__(A7)	OP__(B7)	/* SMBi */
                    //		OP__(C7)	OP__(D7)	OP__(E7)	OP__(F7)
                    //		OP__(02)	OP__(22)	OP__(42)	/* SXY SAX SAY */
                    //		OP__(F4)	/* F4 - SET */
                    //		OP__(03)	OP__(13)	OP__(23)	/* ST0 ST1 ST2 */
                    //		OP__(43)	OP__(53)	/* TMAi TAMi */
                    //		OP__(83)	OP__(93)	OP__(A3)	OP__(B3)	/* TST */
                    //		OP__(73)	OP__(C3)	OP__(D3)	OP__(E3)	OP__(F3)	/* block */
                    //		OP__(54)	OP__(D4)	/* CSL CSH */
                    //#endif
            }
        }
        //-------------------------------- km6502ot.h END
        //# include "km6502ex.h"
        //-------------------------------- km6502ex.h START
        public void K6502_Exec(K6502_Context pc)
        {
            if (pc.iRequest != 0)
            {
                if ((pc.iRequest & IRQ_INIT) != 0)
                {
                    //#if BUILD_HUC6280
                    //			__THIS__.lowClockMode = 1;
                    //#endif
                    pc.A = 0;
                    pc.X = 0;
                    pc.Y = 0;
                    pc.S = 0xFF;
                    pc.P = Z_FLAG | R_FLAG | I_FLAG;
                    pc.iRequest = 0;
                    pc.iMask = ~(UInt32)0;
                    KI_ADDCLOCK(pc, 7);
                    return;
                }
                else if ((pc.iRequest & IRQ_RESET) != 0)
                {
                    //#if BUILD_HUC6280
                    //			__THIS__.lowClockMode = 1;
                    //			K_WRITEMPR(__THISP_ 0x80, 0x00);	/* IPL(TOP OF ROM) */
                    //#endif
                    pc.A = 0;
                    pc.X = 0;
                    pc.Y = 0;
                    pc.S = 0xFF;
                    pc.P = Z_FLAG | R_FLAG | I_FLAG;
                    pc.PC = KI_READWORD(pc, VEC_RESET);
                    pc.iRequest = 0;
                    pc.iMask = ~(UInt32)0;
                }
                else if ((pc.iRequest & IRQ_NMI) != 0)
                {
                    KM_PUSH(pc, (UInt32)((pc.PC >> 8) & 0xff));
                    KM_PUSH(pc, (UInt32)((pc.PC) & 0xff));
                    KM_PUSH(pc, (UInt32)(pc.P | R_FLAG | B_FLAG));
                    //#if BUILD_M65C02 || BUILD_HUC6280
                    //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                    //			__THIS__.iRequest &= ~IRQ_NMI;
                    //#else
                    pc.P = (UInt32)((pc.P & ~T_FLAG) | I_FLAG);   /* 6502 bug */
                    pc.iRequest &= ~(UInt32)(IRQ_NMI | IRQ_BRK);
                    //#endif
                    pc.PC = KI_READWORD(pc, VEC_NMI);
                    KI_ADDCLOCK(pc, 7);
                }
                else if ((pc.iRequest & IRQ_BRK) != 0)
                {
                    KM_PUSH(pc, (UInt32)((pc.PC >> 8) & 0xff));
                    KM_PUSH(pc, (UInt32)((pc.PC) & 0xff));
                    KM_PUSH(pc, (UInt32)(pc.P | R_FLAG | B_FLAG));
                    //#if BUILD_M65C02 || BUILD_HUC6280
                    //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                    //#else
                    pc.P = (UInt32)((pc.P & ~T_FLAG) | I_FLAG);   /* 6502 bug */
                                                                  //#endif
                    pc.iRequest &= ~(UInt32)IRQ_BRK;
                    pc.PC = KI_READWORD(pc, VEC_BRK);
                    KI_ADDCLOCK(pc, 7);
                }
                else if ((pc.P & I_FLAG) != 0)
                {
                    /* interrupt disabled */
                }
                //#if BUILD_HUC6280
                //		else if (__THIS__.iMask & __THIS__.iRequest & IRQ_INT1)
                //		{
                //			KM_PUSH(__THISP_ RTO8(__THIS__.PC >> 8));
                //			KM_PUSH(__THISP_ RTO8(__THIS__.PC));
                //			KM_PUSH(__THISP_ (Uword)(__THIS__.P | R_FLAG | B_FLAG));
                //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                //			__THIS__.PC = KI_READWORD(__THISP_ VEC_INT1);
                //			KI_ADDCLOCK(__THISP_ 7);
                //		}
                //		else if (__THIS__.iMask & __THIS__.iRequest & IRQ_TIMER)
                //		{
                //			KM_PUSH(__THISP_ RTO8(__THIS__.PC >> 8));
                //			KM_PUSH(__THISP_ RTO8(__THIS__.PC));
                //			KM_PUSH(__THISP_ (Uword)(__THIS__.P | R_FLAG | B_FLAG));
                //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                //			__THIS__.PC = KI_READWORD(__THISP_ VEC_TIMER);
                //			KI_ADDCLOCK(__THISP_ 7);
                //		}
                //		else if (__THIS__.iMask & __THIS__.iRequest & IRQ_INT)
                //		{
                //			KM_PUSH(__THISP_ RTO8(__THIS__.PC >> 8));
                //			KM_PUSH(__THISP_ RTO8(__THIS__.PC));
                //			KM_PUSH(__THISP_ (Uword)((__THIS__.P | R_FLAG) & ~B_FLAG));
                //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                //			__THIS__.PC = KI_READWORD(__THISP_ VEC_INT);
                //			KI_ADDCLOCK(__THISP_ 7);
                //		}
                //#else
                else if ((pc.iMask & pc.iRequest & IRQ_INT) != 0)
                {
                    KM_PUSH(pc, (UInt32)((pc.PC >> 8) & 0xff));
                    KM_PUSH(pc, (UInt32)((pc.PC) & 0xff));
                    KM_PUSH(pc, (UInt32)((pc.P | R_FLAG) & ~B_FLAG));
                    //#if BUILD_M65C02
                    //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                    //#else
                    pc.P = (UInt32)((pc.P & ~T_FLAG) | I_FLAG);   /* 6502 bug */
                                                                  //#endif
                    pc.iRequest &= ~(UInt32)IRQ_INT;
                    pc.PC = KI_READWORD(pc, VEC_INT);
                    KI_ADDCLOCK(pc, 7);
                }
                //#endif
            }
            K_OPEXEC(pc);
        }
        //-------------------------------- km6502ex.h END

        //-------------------------------- km6502m.h END

        //#include "../misc/log_cpu.h"

        protected int int_address;
        protected K6502_Context context;
        protected bool breaked;
        protected UInt32 clock_per_frame;
        protected UInt32 clock_of_frame;
        protected UInt32 frame_quarter;
        protected UInt32 breakpoint;
        protected IDevice bus;// IDevice* bus;
        protected object log_cpu;// CPULogger* log_cpu;
        //protected void startup(UINT32 address);

        public double NES_BASECYCLES;
        private const double DEFAULT_CLOCK = 1789773.0;
        private const int DEFAULT_RATE = 44100;
        //  NES_CPU(double clock = DEFAULT_CLOCK);
        //~NES_CPU();
        //void Reset();
        //void Start(int s, int i, double f = 60, int a = 0, int x = 0, int y = 0);
        //UINT32 Exec(UINT32 clock); // returns number of clocks executed
        //void SetMemory(IDevice*);
        //bool Read(UINT32 adr, UINT32 & val, UINT32 id = 0);
        //bool Write(UINT32 adr, UINT32 val, UINT32 id = 0);
        //void SetLogger(CPULogger* logger);

        //#define DEBUG_RW 0
        //#define TRACE 0

#if TRACE
        string[] OP_NAME = new string[256]{
/*  0x-0   0x-1   0x-2   0x-3   0x-4   0x-5   0x-6   0x-7   0x-8   0x-9   0x-A   0x-B   0x-C   0x-D   0x-E   0x-F           */
    "BRK", "ORA", "kil", "slo", "nop", "ORA", "ASL", "slo", "PHP", "ORA", "ASL", "anc", "nop", "ORA", "ASL", "slo", /* 0x0- */
    "BPL", "ORA", "kil", "slo", "nop", "ORA", "ASL", "slo", "CLC", "ORA", "nop", "slo", "nop", "ORA", "ASL", "slo", /* 0x1- */
    "JSR", "AND", "kil", "rla", "BIT", "AND", "ROL", "rla", "PLP", "AND", "ROL", "anc", "BIT", "AND", "ROL", "rla", /* 0x2- */
    "BMI", "AND", "kil", "rla", "nop", "AND", "ROL", "rla", "SEC", "AND", "nop", "rla", "nop", "AND", "ROL", "rla", /* 0x3- */
    "RTI", "EOR", "kil", "sre", "nop", "EOR", "LSR", "sre", "PHA", "EOR", "LSR", "alr", "JMP", "EOR", "LSR", "sre", /* 0x4- */
    "BVC", "EOR", "kil", "sre", "nop", "EOR", "LSR", "sre", "CLI", "EOR", "nop", "sre", "nop", "EOR", "LSR", "sre", /* 0x5- */
    "RTS", "ADC", "kil", "rra", "nop", "ADC", "ROR", "rra", "PLA", "ADC", "ROR", "arr", "JMP", "ADC", "ROR", "rra", /* 0x6- */
    "BVS", "ADC", "kil", "rra", "nop", "ADC", "ROR", "rra", "SEI", "ADC", "nop", "rra", "nop", "ADC", "ROR", "rra", /* 0x7- */
    "nop", "STA", "nop", "sax", "STY", "STA", "STX", "sax", "DEY", "nop", "TXA", "xaa", "STY", "STA", "STX", "sax", /* 0x8- */
    "BCC", "STA", "kil", "ahx", "STY", "STA", "STX", "sax", "TYA", "STA", "TXS", "tas", "shy", "STA", "shx", "ahx", /* 0x9- */
    "LDY", "LDA", "LDX", "lax", "LDY", "LDA", "LDX", "lax", "TAY", "LDA", "TAX", "lax", "LDY", "LDA", "LDX", "lax", /* 0xA- */
    "BCS", "LDA", "kil", "lax", "LDY", "LDA", "LDX", "lax", "CLV", "LDA", "TSX", "las", "LDY", "LDA", "LDX", "lax", /* 0xB- */
    "CPY", "CMP", "nop", "dcp", "CPY", "CMP", "DEC", "dcp", "INY", "CMP", "DEX", "axs", "CPY", "CMP", "DEC", "dcp", /* 0xC- */
    "BNE", "CMP", "kil", "dcp", "nop", "CMP", "DEC", "dcp", "CLD", "CMP", "nop", "dcp", "nop", "CMP", "DEC", "dcp", /* 0xD- */
    "CPX", "SBC", "nop", "isc", "CPX", "SBC", "INC", "isc", "INX", "SBC", "NOP", "sbc", "CPX", "SBC", "INC", "isc", /* 0xE- */
    "BEQ", "SBC", "kil", "isc", "nop", "SBC", "INC", "isc", "SED", "SBC", "nop", "isc", "nop", "SBC", "INC", "isc"  /* 0xF- */
        };
#endif

        //    namespace xgm
        //    {

        public km6502(double clock=DEFAULT_CLOCK)
        {
            NES_BASECYCLES = clock;
            bus = null;
            log_cpu = null;
        }

        ~km6502() //    NES_CPU::~NES_CPU()
        {
        }

        private void writeByte(km6502 __THIS, UInt32 adr, UInt32 val)
        {
            (__THIS).Write(adr, val, 0);
        }

        private UInt32 readByte(km6502 __THIS, UInt32 adr)
        {
            UInt32 val = 0;
            (__THIS).Read(adr, ref val, 0);
            return val;
        }

        public void startup(UInt32 address)
        {
            breaked = false;
            context.PC = 0x4100;
            breakpoint = context.PC + 3;
            context.P = 0x26;                 // IRZ
                                              //assert(bus);
            bus.Write(context.PC + 0, 0x20, 0);  // JSR 
            bus.Write(context.PC + 1, address & 0xff, 0);
            bus.Write(context.PC + 2, address >> 8, 0);
            bus.Write(context.PC + 3, 0x4c, 0);  // JMP 04103H 
            bus.Write(context.PC + 4, breakpoint & 0xff, 0);
            bus.Write(context.PC + 5, breakpoint >> 8, 0);
        }

        public UInt32 Exec(UInt32 clock)
        {
            context.clock = 0;

            while (context.clock < clock)
            {
                if (!breaked)
                {
                    //DEBUG_OUT("PC: 0x%04X\n", context.PC);

                    #if TRACE
                        UInt32 TPC = context.PC;
                        UInt32[] tb = new UInt32[3];
                        bus.Read((TPC + 0) & 0xFFFF, ref tb[0]);
                        bus.Read((TPC + 1) & 0xFFFF, ref tb[1]);
                        bus.Read((TPC + 2) & 0xFFFF, ref tb[2]);
                        Console.Write("{0:X04}: A={1:X02} X={2:X02} Y={3:X02} P={4:X02} S={5:X02} {6} > ",
                            context.PC,
                            context.A, context.X, context.Y, context.P, context.S,
                            context.iRequest != 0 ? 'I' : 'i');
                    #endif

                    K6502_Exec(context);

                    #if TRACE
                        Console.Write("{0}", OP_NAME[context.lastcode]);
                                        int oplen = (Int32)(context.PC - TPC);
                                        for (int i = 0; i < 3; ++i)
                                        {
                                            if (i == 0 || i < oplen)
                                            {
                                Console.Write(" {0:X02}", tb[i]);
                                            }
                                        }
                        Console.WriteLine("");
                    #endif

                    if (context.PC == breakpoint)
                        breaked = true;
                }
                else
                {
                    if ((clock_of_frame >> 16) < clock)
                        context.clock = (clock_of_frame >> 16) + 1;
                    else
                        context.clock = clock;
                }

                // フレームクロックに到達
                if ((clock_of_frame >> 16) < context.clock)
                {
                    if (breaked)
                    {
                        //                    if (log_cpu!=null)
                        //                        log_cpu.Play();

                        startup((UInt32)int_address);
                    }
                    clock_of_frame += clock_per_frame;
                    //DEBUG_OUT("NMI\n");
                }
            }

            clock_of_frame -= (context.clock << 16);

            return context.clock; // return actual number of clocks executed
        }

        public void SetMemory(IDevice b)
        {
            bus = b;
        }

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id)
        {
            //#if DEBUG_RW
            //        DEBUG_OUT("Write: 0x%04X = 0x%02X\n", adr, val);
            //#endif

            // for blargg's CPU tests
            //#if 0
            //    if (adr == 0x6000)
            //    {
            //        DEBUG_OUT("Blargg result: %02X [");
            //        UINT32 msg = 0x6004;
            //        do
            //        {
            //            UINT32 ic;
            //            Read(msg, ic);
            //            if (ic == 0) break;
            //            ++msg;
            //            DEBUG_OUT("%c", char(ic));
            //        } while (1);
            //        DEBUG_OUT("]\n");
            //        return false;
            //    }
            //#endif

            if (bus != null)
                return bus.Write(adr, val, id);
            else
                return false;
        }

        public override bool Read(UInt32 adr, ref UInt32 val, UInt32 id)
        {
            if (bus != null)
            {
                bool result = bus.Read(adr, ref val, id);

                //#if DEBUG_RW
                //            DEBUG_OUT(" Read: 0x%04X = 0x%02X\n", adr, val);
                //#endif

                return result;
            }
            else
                return false;
        }

        public override void Reset()
        {
            // KM6502のリセット
            //memset(&context, 0, sizeof(K6502_Context));
            context = new K6502_Context();
            //public delegate UInt32 dlgReadHandler(object user, UInt32 adr);
            //public delegate void dlgWriterHandler(object user, UInt32 adr, UInt32 value);
            context.ReadByte = readByte;
            context.WriteByte = writeByte;
            context.iRequest = (UInt32)K6502_IRQ.K6502_INIT;
            context.clock = 0;
            context.user = this;
            context.A = 0;
            context.X = 0;
            context.Y = 0;
            context.S = 0xff;
            context.PC = breakpoint = 0xffff;
            context.illegal = 0;
            breaked = false;
            K6502_Exec(context);
        }

        public void Start(Int32 start_adr, Int32 int_adr, double int_freq=60.0, Int32 a=0, Int32 x=0, Int32 y=0)
        {
            // 割り込みアドレス設定
            int_address = int_adr;
            clock_per_frame = (UInt32)((double)((1 << 16) * NES_BASECYCLES) / int_freq);
            clock_of_frame = 0;

            // count clock quarters
            frame_quarter = 3;

            //        if (log_cpu!=null)
            //            log_cpu.Init(a, x);

            context.A = (UInt32)a;
            context.X = (UInt32)x;
            context.Y = (UInt32)y;
            startup((UInt32)start_adr);

            for (int i = 0; (i < (NES_BASECYCLES / int_freq)) && !breaked; i++, K6502_Exec(context))
            {
#if TRACE
                UInt32 TPC = context.PC;
                UInt32[] tb = new UInt32[3];
                bus.Read((TPC + 0) & 0xFFFF, ref tb[0]);
                bus.Read((TPC + 1) & 0xFFFF, ref tb[1]);
                bus.Read((TPC + 2) & 0xFFFF, ref tb[2]);
                Console.Write("{0:X04}: A={1:X02} X={2:X02} Y={3:X02} P={4:X02} S={5:X02} {6} > ",
                    context.PC,
                    context.A, context.X, context.Y, context.P, context.S,
                    context.iRequest != 0 ? 'I' : 'i');
#endif
                if (context.PC == breakpoint)
                {
                    breaked = true;
                }
            }

            clock_of_frame = 0;
        }

        public void SetLogger(object logger)//(CPULogger* logger)
        {
            log_cpu = logger;
        }

        public override void SetOption(int id, int val)
        {
            throw new NotImplementedException();
        }
    } // namespace xgm    }
}