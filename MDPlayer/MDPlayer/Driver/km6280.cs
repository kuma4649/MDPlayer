using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver
{
    public class km6280
    {
        //-------------------------------- km6280m.h START

        //#define BUILD_M6502 0
        //#define BUILD_M65C02 0
        //#define BUILD_HUC6280 1
        //#define BUILD_2A03 0

        //# include "km6280.h"
        //-------------------------------- km6280.h START

        //# ifndef KM6280_H_
        //#define KM6280_H_

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
        //#define RTO16(w) ((Uword)((w) & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)((w) & 0xFF))		/* Round to  8bit integer */
        //#elif defined(__BORLANDC__)
        //        typedef unsigned int Uword;             /* (0-0xFFFF) */
        //        typedef unsigned char Ubyte;            /* unsigned 8bit integer for table */
        //# ifndef Inline
        //#define Inline __inline
        //#endif
        //#define CCall __cdecl
        //#define FastCall
        //#define RTO16(w) ((Uword)((w) & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)((w) & 0xFF))		/* Round to  8bit integer */
        //#elif defined(__GNUC__)
        //        typedef unsigned int Uword;             /* (0-0xFFFF) */
        //        typedef unsigned char Ubyte;            /* unsigned 8bit integer for table */
        //# ifndef Inline
        //#define Inline __inline__
        //#endif
        //#define CCall
        //#define FastCall /* __attribute__((regparm(2))) */
        //#define RTO16(w) ((Uword)((w) & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)((w) & 0xFF))		/* Round to  8bit integer */
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
        //#define RTO16(w) ((Uword)((w) & 0xFFFF))	/* Round to 16bit integer */
        //#define RTO8(w) ((Uword)((w) & 0xFF))		/* Round to  8bit integer */
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
        public delegate UInt32 dlgReadHandler(m_hes.HESHES user, UInt32 adr);
        //typedef UInt32(Callback* ReadHandler)(void* user, UInt32 adr);
        public delegate void dlgWriterHandler(m_hes.HESHES user, UInt32 adr, UInt32 value);
        //typedef void (Callback *WriteHandler)(void *user, Uword adr, Uword value);
        //#else
        //typedef Uword(Callback* ReadHandler)(Uword adr);
        //typedef void (Callback* WriteHandler) (Uword adr, Uword value);
        //#endif

        public class K6280_Context
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
            //public km6280 user;         /* pointer to user area */
            public m_hes.HESHES user;         /* pointer to user area */

            public UInt32 lowClockMode;

            //#if USE_CALLBACK
            //	/* pointer to callback functions */
            //#if USE_INLINEMMC
            //	ReadHandler ReadByte[1 << (16 - USE_INLINEMMC)];
            //	WriteHandler WriteByte[1 << (16 - USE_INLINEMMC)];
            //#else
            public dlgReadHandler ReadByte;
            public dlgWriterHandler WriteByte;
            //	ReadHandler ReadByte;
            //	WriteHandler WriteByte;
            //#endif
            public dlgReadHandler ReadMPR;
            public dlgWriterHandler WriteMPR;
            public dlgWriterHandler Write6270;
            //ReadHandler ReadMPR;
            //WriteHandler WriteMPR;
            //WriteHandler Write6270;
            //#endif

            //#if USE_DIRECT_ZEROPAGE
            //Ubyte *zeropage;	/* pointer to zero page */
            //#endif
        };

        public enum K6280_FLAGS
        {
            K6280_C_FLAG = 0x01,
            K6280_Z_FLAG = 0x02,
            K6280_I_FLAG = 0x04,
            K6280_D_FLAG = 0x08,
            K6280_B_FLAG = 0x10,
            K6280_T_FLAG = 0x20,
            K6280_V_FLAG = 0x40,
            K6280_N_FLAG = 0x80
        };

        public enum K6280_IRQ
        {
            K6280_INIT = 1,
            K6280_RESET = 2,
            K6280_NMI = 4,
            K6280_BRK = 8,
            K6280_TIMER = 16,
            K6280_INT1 = 32,
            K6280_INT2 = 64
        };

        //# ifdef STATIC_CONTEXT6280
        //        External void K6280_Exec(void);
        //#else
        //        External void K6280_Exec(struct K6280_Context *pc);
        //#endif

        //#if !USE_CALLBACK
        //#if USE_USERPOINTER
        //External Uword CCall K6280_ReadByte(void *user, Uword adr);
        //External void CCall K6280_WriteByte(void *user, Uword adr, Uword value);
        //External Uword CCall K6280_ReadMPR(void *user, Uword adr);
        //External void CCall K6280_WriteMPR(void *user, Uword adr, Uword value);
        //External void CCall K6280_Write6270(void *user, Uword adr, Uword value);
        //#else
        //External Uword CCall K6280_ReadByte(Uword adr);
        //        External void CCall K6280_WriteByte(Uword adr, Uword value);
        //        External Uword CCall K6280_ReadMPR(Uword adr);
        //        External void CCall K6280_WriteMPR(Uword adr, Uword value);
        //        External void CCall K6280_Write6270(Uword adr, Uword value);
        //#endif
        //#endif

        //# ifdef __cplusplus
        //    }
        //#endif
        //#endif

        //-------------------------------- km6280.h END

        public const Int32 C_FLAG = (Int32)K6280_FLAGS.K6280_C_FLAG;
        public const Int32 Z_FLAG = (Int32)K6280_FLAGS.K6280_Z_FLAG;
        public const Int32 I_FLAG = (Int32)K6280_FLAGS.K6280_I_FLAG;
        public const Int32 D_FLAG = (Int32)K6280_FLAGS.K6280_D_FLAG;
        public const Int32 B_FLAG = (Int32)K6280_FLAGS.K6280_B_FLAG;
        public const Int32 T_FLAG = (Int32)K6280_FLAGS.K6280_T_FLAG;
        public const Int32 V_FLAG = (Int32)K6280_FLAGS.K6280_V_FLAG;
        public const Int32 N_FLAG = (Int32)K6280_FLAGS.K6280_N_FLAG;
        public const Int32 R_FLAG = 0;

        public const Int32 BASE_OF_ZERO = 0x2000;

        public const Int32 VEC_RESET = 0xFFFE;
        public const Int32 VEC_NMI = 0xFFFC;
        public const Int32 VEC_TIMER = 0xFFFA;
        public const Int32 VEC_INT1 = 0xFFF8;
        public const Int32 VEC_INT = 0xFFF6;

        public const Int32 VEC_BRK = VEC_INT;

        public const Int32 IRQ_INIT = (Int32)K6280_IRQ.K6280_INIT;
        public const Int32 IRQ_RESET = (Int32)K6280_IRQ.K6280_RESET;
        public const Int32 IRQ_NMI = (Int32)K6280_IRQ.K6280_NMI;
        public const Int32 IRQ_BRK = (Int32)K6280_IRQ.K6280_BRK;
        public const Int32 IRQ_TIMER = (Int32)K6280_IRQ.K6280_TIMER;
        public const Int32 IRQ_INT1 = (Int32)K6280_IRQ.K6280_INT1;
        public const Int32 IRQ_INT = (Int32)K6280_IRQ.K6280_INT2;

        //# ifdef STATIC_CONTEXT6280
        //        extern struct K6280_Context STATIC_CONTEXT6280;
        //#define __THIS__	STATIC_CONTEXT6280
        //#define __CONTEXT	void
        //#define __CONTEXT_	/* none */
        //#define __THISP		/* none */
        //#define __THISP_	/* none */
        //#else
        //#define __THIS__	(*pc)
        //#define __CONTEXT	struct K6280_Context *pc
        //#define __CONTEXT_	struct K6280_Context *pc,
        //#define __THISP		pc
        //#define __THISP_	pc,
        //#endif

        //#define K_EXEC		K6280_Exec

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
        //static Uword Inline K_READ(__CONTEXT_ Uword adr)
        //{
        //	return __THIS__.ReadByte(__THIS_USER_ adr);
        //}
        //static void Inline K_WRITE(__CONTEXT_ Uword adr, Uword value)
        //{
        //	__THIS__.WriteByte(__THIS_USER_ adr, value);
        //}
        //#endif
        //static Uword Inline K_READMPR(__CONTEXT_ Uword adr)
        //{
        //	return __THIS__.ReadMPR(__THIS_USER_ adr);
        //}
        //static void Inline K_WRITEMPR(__CONTEXT_ Uword adr, Uword value)
        //{
        //	__THIS__.WriteMPR(__THIS_USER_ adr, value);
        //}
        //static void Inline K_WRITE6270(__CONTEXT_ Uword adr, Uword value)
        //{
        //	__THIS__.Write6270(__THIS_USER_ adr, value);
        //}
        //#else
        static UInt32 K_READ(K6280_Context pc, UInt32 adr)
        {
            return pc.ReadByte(pc.user, adr);
        }
        static void K_WRITE(K6280_Context pc, UInt32 adr, UInt32 value)
        {
            pc.WriteByte(pc.user, adr, value);
        }
        static UInt32 K_READMPR(K6280_Context pc, UInt32 adr)
        {
            return pc.ReadMPR(pc.user, adr);
        }
        static void K_WRITEMPR(K6280_Context pc, UInt32 adr, UInt32 value)
        {
            pc.WriteMPR(pc.user, adr, value);
        }
        static void K_WRITE6270(K6280_Context pc, UInt32 adr, UInt32 value)
        {
            pc.Write6270(pc.user, adr, value);
        }

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
        //#define FLAG_NZ(w)	(fl_table[RTO8(w)])
        //#define FLAG_NZC(w)	(fl_table[w & 0x01ff])
        //#else
        //#define FLAG_NZ(w)	((w & N_FLAG) + (RTO8(w) ? 0 : Z_FLAG))
        //#define FLAG_NZC(w)	(FLAG_NZ(w) + ((w >> 8) & C_FLAG))
        //#endif

        //-------------------------------- km6502ft.h END

        //# include "km6502cd.h"
        //-------------------------------- km6502cd.h START
        public void KI_ADDCLOCK(K6280_Context pc, UInt32 cycle)
        {
            //#if BUILD_HUC6280
            if (pc.lowClockMode != 0)
            {
                //#if 0
                //cycle += (cycle << 2);	/* x5 */
                //#else
                cycle += cycle + cycle; /*    */
                cycle += cycle;         /* x6 */
                                        //#endif
            }
            pc.clock += cycle;
            //#else
            //__THIS__.clock += cycle;
            //#endif
        }

        public UInt32 KI_READWORD(K6280_Context pc, UInt32 adr)
        {
            UInt32 ret = K_READ(pc, adr);
            return ret + (K_READ(pc, (UInt32)((UInt32)((adr + 1) & 0xffff))) << 8);
        }

        public UInt32 KI_READWORDZP(K6280_Context pc, UInt32 adr)
        {
            UInt32 ret = K_READ(pc, (UInt32)(BASE_OF_ZERO + adr));
            return ret + (K_READ(pc, (UInt32)(BASE_OF_ZERO + (UInt32)((adr + 1) & 0xff))) << 8);
        }

        public UInt32 KAI_IMM(K6280_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 1) & 0xffff);
            return ret;
        }

        public UInt32 KAI_IMM16(K6280_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 2) & 0xffff);
            return ret;
        }

        public UInt32 KAI_ABS(K6280_Context pc)
        {
            return KI_READWORD(pc, KAI_IMM16(pc));
        }

        public UInt32 KAI_ABSX(K6280_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.X) & 0xffff);
        }

        public UInt32 KAI_ABSY(K6280_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.Y) & 0xffff);
        }

        public UInt32 KAI_ZP(K6280_Context pc)
        {
            return K_READ(pc, KAI_IMM(pc));
        }

        public UInt32 KAI_ZPX(K6280_Context pc)
        {
            return (UInt32)((KAI_ZP(pc) + pc.X) & 0xff);
        }

        public UInt32 KAI_INDY(K6280_Context pc)
        {
            return (UInt32)((KI_READWORDZP(pc, KAI_ZP(pc)) + pc.Y) & 0xffff);
        }

        public UInt32 KA_IMM(K6280_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 1) & 0xffff);
            return ret;
        }

        public UInt32 KA_IMM16(K6280_Context pc)
        {
            UInt32 ret = pc.PC;
            pc.PC = (UInt32)((pc.PC + 2) & 0xffff);
            return ret;
        }

        public UInt32 KA_ABS(K6280_Context pc)
        {
            return KI_READWORD(pc, KAI_IMM16(pc));
        }

        public UInt32 KA_ABSX(K6280_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.X) & 0xffff);
        }

        public UInt32 KA_ABSY(K6280_Context pc)
        {
            return (UInt32)((KAI_ABS(pc) + pc.Y) & 0xffff);
        }

        public UInt32 KA_ZP(K6280_Context pc)
        {
            return BASE_OF_ZERO + K_READ(pc, KAI_IMM(pc));
        }

        public UInt32 KA_ZPX(K6280_Context pc)
        {
            return BASE_OF_ZERO + (UInt32)((KAI_ZP(pc) + pc.X) & 0xff);
        }

        public UInt32 KA_ZPY(K6280_Context pc)
        {
            return BASE_OF_ZERO + (UInt32)((KAI_ZP(pc) + pc.Y) & 0xff);
        }

        public UInt32 KA_INDX(K6280_Context pc)
        {
            return KI_READWORDZP(pc, KAI_ZPX(pc));
        }

        public UInt32 KA_INDY(K6280_Context pc)
        {
            return (UInt32)((KI_READWORDZP(pc, KAI_ZP(pc)) + pc.Y) & 0xffff);
        }

        //#if BUILD_HUC6280 || BUILD_M65C02
        public UInt32 KA_IND(K6280_Context pc)
        {
            return KI_READWORDZP(pc, KAI_ZP(pc));
        }
        //#else
        //static Uword Inline KI_READWORDBUG(__CONTEXT_ Uword adr)
        //{
        //Uword ret = K_READNP(__THISP_ adr);
        //return ret + (K_READNP(__THISP_(Uword)((adr & 0xFF00) + RTO8(adr + 1))) << 8);
        //}
        //#endif

        //#if BUILD_HUC6280
        //#define KA_ABSX_ KA_ABSX
        //#define KA_ABSY_ KA_ABSY
        //#define KA_INDY_ KA_INDY
        //#else
        //        static Uword MasubCall KA_ABSX_(__CONTEXT)
        //        {
        //            if (RTO8(__THIS__.PC) == 0xFF) KI_ADDCLOCK(__THISP_ 1); /* page break */
        //            return KAI_ABSX(__THISP);
        //        }
        //        static Uword MasubCall KA_ABSY_(__CONTEXT)
        //        {
        //            if (RTO8(__THIS__.PC) == 0xFF) KI_ADDCLOCK(__THISP_ 1); /* page break */
        //            return KAI_ABSY(__THISP);
        //        }
        //        static Uword MasubCall KA_INDY_(__CONTEXT)
        //        {
        //            Uword adr = KAI_INDY(__THISP);
        //            if (RTO8(adr) == 0xFF) KI_ADDCLOCK(__THISP_ 1); /* page break */
        //            return adr;
        //        }
        //#endif

        public void KM_ALUADDER(K6280_Context pc, UInt32 src)
        {
            UInt32 w = pc.A + src + (pc.P & C_FLAG);
            pc.P &= (UInt32)(~(UInt32)(N_FLAG | V_FLAG | Z_FLAG | C_FLAG | T_FLAG));
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

        public void KM_ALUADDER_D(K6280_Context pc, UInt32 src)
        {
            UInt32 wl = (pc.A & 0x0F) + (src & 0x0F) + (pc.P & C_FLAG);
            UInt32 w = pc.A + src + (pc.P & C_FLAG);
            //#if BUILD_HUC6280 || BUILD_M65C02
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            //#else
            //pc.P &= (UInt32)(~(UInt32)C_FLAG);
            //#endif
            if (wl > 0x9) w += 0x6;
            if (w > 0x9F)
            {
                pc.P += C_FLAG;
                w += 0x60;
            }
            //#if BUILD_HUC6280 || BUILD_M65C02
            pc.P += (UInt32)FLAG_NZ((Int32)w);
            //#endif
            pc.A = (UInt32)((w) & 0xff);
            KI_ADDCLOCK(pc, 1);
        }

        public void KMI_ADC(K6280_Context pc, UInt32 src)
        {
            KM_ALUADDER(pc, src);
        }

        public void KMI_ADC_D(K6280_Context pc, UInt32 src)
        {
            KM_ALUADDER_D(pc, src);
        }

        public void KMI_SBC(K6280_Context pc, UInt32 src)
        {
            KM_ALUADDER(pc, src ^ 0xFF);
        }

        public void KMI_SBC_D(K6280_Context pc, UInt32 src)
        {
            KM_ALUADDER_D(pc, (UInt32)(((src ^ 0xFF) + (0x100 - 0x66)) & 0xff));
        }

        public void KM_CMP(K6280_Context pc, UInt32 src)
        {
            UInt32 w = pc.A + (src ^ 0xFF) + 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)FLAG_NZC((Int32)w);
        }

        public void KM_CPX(K6280_Context pc, UInt32 src)
        {
            UInt32 w = pc.X + (src ^ 0xFF) + 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)FLAG_NZC((Int32)w);
        }

        public void KM_CPY(K6280_Context pc, UInt32 src)
        {
            UInt32 w = pc.Y + (src ^ 0xFF) + 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)FLAG_NZC((Int32)w);
        }

        public void KM_BIT(K6280_Context pc, UInt32 src)
        {
            UInt32 w = pc.A & src;
            pc.P &= ~(UInt32)(N_FLAG | V_FLAG | Z_FLAG);
            pc.P += (UInt32)((src & (N_FLAG | V_FLAG)) + (w != 0 ? 0 : Z_FLAG));
        }

        public void KM_AND(K6280_Context pc, UInt32 src)
        {
            pc.A &= src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | T_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)pc.A);
        }

        public void KM_ORA(K6280_Context pc, UInt32 src)
        {
            pc.A |= src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | T_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)pc.A);
        }

        public void KM_EOR(K6280_Context pc, UInt32 src)
        {
            pc.A ^= src;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | T_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)pc.A);
        }

        public UInt32 KM_DEC(K6280_Context pc, UInt32 des)
        {
            Int32 w = (Int32)(des - 1);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ(w);
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_INC(K6280_Context pc, UInt32 des)
        {
            Int32 w = (Int32)(des + 1);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ(w);
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_ASL(K6280_Context pc, UInt32 des)
        {
            Int32 w = (Int32)(des << 1);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ(w) + ((des >> 7)/* & C_FLAG*/));
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_LSR(K6280_Context pc, UInt32 des)
        {
            UInt32 w = des >> 1;
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ((Int32)w) + (des & C_FLAG));
            return w;
        }

        public UInt32 KM_LD(K6280_Context pc, UInt32 src)
        {
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG);
            pc.P += (UInt32)FLAG_NZ((Int32)src);
            return src;
        }

        public UInt32 KM_ROL(K6280_Context pc, UInt32 des)
        {
            UInt32 w = (des << 1) + (pc.P & C_FLAG);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ((Int32)w) + ((des >> 7)/* & C_FLAG*/));
            return (UInt32)((w) & 0xff);
        }

        public UInt32 KM_ROR(K6280_Context pc, UInt32 des)
        {
            UInt32 w = (des >> 1) + ((pc.P & C_FLAG) << 7);
            pc.P &= ~(UInt32)(N_FLAG | Z_FLAG | C_FLAG);
            pc.P += (UInt32)(FLAG_NZ((Int32)w) + (des & C_FLAG));
            return (UInt32)((w) & 0xff);
        }

        public void KM_BRA(K6280_Context pc, UInt32 rel)
        {
            //#if BUILD_HUC6280
            pc.PC = (UInt32)((pc.PC + (rel ^ 0x80) - 0x80) & 0xffff);
            KI_ADDCLOCK(pc, 2);
            //#else
            //UInt32 oldPage = pc.PC & 0xFF00;
            //pc.PC = (UInt32)((pc.PC + (rel ^ 0x80) - 0x80) & 0xffff);
            //KI_ADDCLOCK(pc, (UInt32)(1 + ((oldPage != (pc.PC & 0xFF00)) ? 1 : 0)));
            //#endif
        }

        public void KM_PUSH(K6280_Context pc, UInt32 src)
        {
            K_WRITE(pc, (UInt32)(BASE_OF_ZERO + 0x100 + pc.S), src);
            pc.S = (UInt32)((pc.S - 1) & 0xff);
        }

        public UInt32 KM_POP(K6280_Context pc)
        {
            pc.S = (UInt32)((pc.S + 1) & 0xff);
            return K_READ(pc, (UInt32)(BASE_OF_ZERO + 0x100 + pc.S));
        }

        //#if BUILD_HUC6280 || BUILD_M65C02
        public UInt32 KM_TSB(K6280_Context pc, UInt32 mem)
        {
            UInt32 w = pc.A | mem;
            pc.P &= ~(UInt32)(N_FLAG | V_FLAG | Z_FLAG);
            pc.P += (UInt32)((mem & (N_FLAG | V_FLAG)) + (w != 0 ? 0 : Z_FLAG));
            return w;
        }

        public UInt32 KM_TRB(K6280_Context pc, UInt32 mem)
        {
            UInt32 w = (pc.A ^ 0xFF) & mem;
            pc.P &= ~(UInt32)(N_FLAG | V_FLAG | Z_FLAG);
            pc.P += (UInt32)((mem & (N_FLAG | V_FLAG)) + (w != 0 ? 0 : Z_FLAG));
            return w;
        }
        //#endif
        //#if BUILD_HUC6280
        public UInt32 KMI_PRET(K6280_Context pc)
        {
            UInt32 saveA = pc.A;
            pc.A = K_READ(pc, (UInt32)(BASE_OF_ZERO + pc.X));
            return saveA;
        }

        public void KMI_POSTT(K6280_Context pc, UInt32 saveA)
        {
            K_WRITE(pc, (UInt32)(BASE_OF_ZERO + pc.X), pc.A);
            pc.A = saveA;
            KI_ADDCLOCK(pc, 3);
        }

        public void KM_TST(K6280_Context pc, UInt32 imm, UInt32 mem)
        {
            UInt32 w = imm & mem;
            pc.P &= ~(UInt32)(N_FLAG | V_FLAG | Z_FLAG);
            pc.P += (UInt32)((mem & (N_FLAG | V_FLAG)) + (w != 0 ? 0 : Z_FLAG));
        }
        //#endif

        /* --- ADC ---  */
        //#if BUILD_HUC6280
        //#define DEF_ADC(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ KMI_ADC(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall D_Opco##i(__CONTEXT) \
        //{ KMI_ADC_D(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KMI_ADC(__THISP_ K_READ##p(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //} \
        //static void OpcodeCall TD_Opc##i(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KMI_ADC_D(__THISP_ K_READ##p(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        //#define DEF_ADC(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ KMI_ADC(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall D_Opco##i(__CONTEXT) \
        //{ KMI_ADC_D(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //#endif

        //DEF_ADC(61, NP, KA_INDX)	/* 61 - ADC - (Indirect,X) */
        //DEF_ADC(65, ZP, KA_ZP)	/* 65 - ADC - Zero Page */
        //DEF_ADC(69, NP, KA_IMM)	/* 69 - ADC - Immediate */
        //DEF_ADC(6D, NP, KA_ABS)	/* 6D - ADC - Absolute */
        //DEF_ADC(71, NP, KA_INDY_)	/* 71 - ADC - (Indirect),Y */
        //DEF_ADC(75, ZP, KA_ZPX)	/* 75 - ADC - Zero Page,X */
        //DEF_ADC(79, NP, KA_ABSY_)	/* 79 - ADC - Absolute,Y */
        //DEF_ADC(7D, NP, KA_ABSX_)   /* 7D - ADC - Absolute,X */
        //DEF_ADC(72, NP, KA_IND)	/* 72 - ADC - (Indirect) */

        public void Opcode61(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_INDX(pc))); }
        public void Opcode65(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode69(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_IMM(pc))); }
        public void Opcode6D(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode71(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_INDY(pc))); }
        public void Opcode75(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode79(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_ABSY(pc))); }
        public void Opcode7D(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_ABSX(pc))); }
        public void Opcode72(K6280_Context pc) { KMI_ADC(pc, K_READ(pc, KA_IND(pc))); }

        public void D_Opco61(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_INDX(pc))); }
        public void D_Opco65(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_ZP(pc))); }
        public void D_Opco69(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_IMM(pc))); }
        public void D_Opco6D(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_ABS(pc))); }
        public void D_Opco71(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_INDY(pc))); }
        public void D_Opco75(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_ZPX(pc))); }
        public void D_Opco79(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_ABSY(pc))); }
        public void D_Opco7D(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_ABSX(pc))); }
        public void D_Opco72(K6280_Context pc) { KMI_ADC_D(pc, K_READ(pc, KA_IND(pc))); }

        public void T_Opco61(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_INDX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco65(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_ZP(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco69(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_IMM(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco6D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_ABS(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco71(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_INDY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco75(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_ZPX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco79(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_ABSY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco7D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_ABSX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco72(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC(pc, K_READ(pc, KA_IND(pc))); KMI_POSTT(pc, saveA); }

        public void TD_Opc61(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_INDX(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc65(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_ZP(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc69(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_IMM(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc6D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_ABS(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc71(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_INDY(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc75(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_ZPX(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc79(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_ABSY(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc7D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_ABSX(pc))); KMI_POSTT(pc, saveA); }
        public void TD_Opc72(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KMI_ADC_D(pc, K_READ(pc, KA_IND(pc))); KMI_POSTT(pc, saveA); }


        /* --- AND ---  */
        //#if BUILD_HUC6280
        //#define DEF_AND(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ KM_AND(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KM_AND(__THISP_ K_READ##p(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        //#define DEF_AND(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{
        //KM_AND(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //#endif
        //DEF_AND(21, NP, KA_INDX)    /* 21 - AND - (Indirect,X) */
        //DEF_AND(25, ZP, KA_ZP)  /* 25 - AND - Zero Page */
        //DEF_AND(29, NP, KA_IMM) /* 29 - AND - Immediate */
        //DEF_AND(2D, NP, KA_ABS) /* 2D - AND - Absolute */
        //DEF_AND(31, NP, KA_INDY_)   /* 31 - AND - (Indirect),Y */
        //DEF_AND(35, ZP, KA_ZPX) /* 35 - AND - Zero Page,X */
        //DEF_AND(39, NP, KA_ABSY_)   /* 39 - AND - Absolute,Y */
        //DEF_AND(3D, NP, KA_ABSX_)	/* 3D - AND - Absolute,X */
        //DEF_AND(32,NP,KA_IND)   /* 32 - AND - (Indirect) */

        public void Opcode21(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_INDX(pc))); }
        public void Opcode25(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode29(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_IMM(pc))); }
        public void Opcode2D(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode31(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_INDY(pc))); }
        public void Opcode35(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode39(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_ABSY(pc))); }
        public void Opcode3D(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_ABSX(pc))); }
        public void Opcode32(K6280_Context pc) { KM_AND(pc, K_READ(pc, KA_IND(pc))); }

        public void T_Opco21(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_INDX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco25(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_ZP(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco29(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_IMM(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco2D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_ABS(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco31(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_INDY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco35(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_ZPX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco39(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_ABSY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco3D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_ABSX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco32(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_AND(pc, K_READ(pc, KA_IND(pc))); KMI_POSTT(pc, saveA); }


        /* --- ASL ---  */
        //#define DEF_ASL(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //        { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_ASL(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //            DEF_ASL(06, ZP, KA_ZP)	/* 06 - ASL - Zero Page */
        //DEF_ASL(0E, NP, KA_ABS)	/* 0E - ASL - Absolute */
        //DEF_ASL(16, ZP, KA_ZPX)	/* 16 - ASL - Zero Page,X */
        //DEF_ASL(1E, NP, KA_ABSX)	/* 1E - ASL - Absolute,X */
        //static void OpcodeCall Opcode0A(__CONTEXT)	/* 0A - ASL - Accumulator */
        //{ __THIS__.A = KM_ASL(__THISP_ __THIS__.A); }

        public void Opcode06(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr))); }
        public void Opcode0E(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr))); }
        public void Opcode16(K6280_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr))); }
        public void Opcode1E(K6280_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_ASL(pc, K_READ(pc, adr))); }
        public void Opcode0A(K6280_Context pc) { pc.A = KM_ASL(pc, pc.A); }


        //#if BUILD_HUC6280
        /* --- BBRi --- */
        //#define DEF_BBR(i,y) static void OpcodeCall Opcode##i(__CONTEXT) \
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
        //DEF_BBR(7F,7)         /* 7F - BBR7 */
        public void Opcode0F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 0)) == 0) KM_BRA(pc, rel); }
        public void Opcode1F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 1)) == 0) KM_BRA(pc, rel); }
        public void Opcode2F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 2)) == 0) KM_BRA(pc, rel); }
        public void Opcode3F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 3)) == 0) KM_BRA(pc, rel); }
        public void Opcode4F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 4)) == 0) KM_BRA(pc, rel); }
        public void Opcode5F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 5)) == 0) KM_BRA(pc, rel); }
        public void Opcode6F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 6)) == 0) KM_BRA(pc, rel); }
        public void Opcode7F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 7)) == 0) KM_BRA(pc, rel); }

        /* --- BBSi --- */
        //#define DEF_BBS(i,y) static void OpcodeCall Opcode##i(__CONTEXT) \
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
        public void Opcode8F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 0)) != 0) KM_BRA(pc, rel); }
        public void Opcode9F(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 1)) != 0) KM_BRA(pc, rel); }
        public void OpcodeAF(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 2)) != 0) KM_BRA(pc, rel); }
        public void OpcodeBF(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 3)) != 0) KM_BRA(pc, rel); }
        public void OpcodeCF(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 4)) != 0) KM_BRA(pc, rel); }
        public void OpcodeDF(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 5)) != 0) KM_BRA(pc, rel); }
        public void OpcodeEF(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 6)) != 0) KM_BRA(pc, rel); }
        public void OpcodeFF(K6280_Context pc) { UInt32 adr = KA_ZP(pc); UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((K_READ(pc, adr) & (1 << 7)) != 0) KM_BRA(pc, rel); }

        //#endif

        /* --- BIT ---  */
        //#define DEF_BIT(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //            {
        //                KM_BIT(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_BIT(24, ZP, KA_ZP)  /* 24 - BIT - Zero Page */
        //DEF_BIT(2C, NP, KA_ABS) /* 2C - BIT - Absolute */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_BIT(34,ZP,KA_ZPX)	/* 34 - BIT - Zero Page,X */
        //DEF_BIT(3C,NP,KA_ABSX_)	/* 3C - BIT - Absolute,X */
        //DEF_BIT(89,NP,KA_IMM)	/* 89 - BIT - Immediate */
        ////#endif
        public void Opcode24(K6280_Context pc) { KM_BIT(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode2C(K6280_Context pc) { KM_BIT(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode34(K6280_Context pc) { KM_BIT(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode3C(K6280_Context pc) { KM_BIT(pc, K_READ(pc, KA_ABSX(pc))); }
        public void Opcode89(K6280_Context pc) { KM_BIT(pc, K_READ(pc, KA_IMM(pc))); }

        /* --- Bcc ---  */
        //#define DEF_BRA(i,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                { \
        //	Uword rel = K_READNP(__THISP_ KA_IMM(__THISP)); \
        //	if (a) KM_BRA(__THISP_ rel); \
        //}
        //DEF_BRA(10, ((__THIS__.P & N_FLAG) == 0))   /* 10 - BPL */
        //DEF_BRA(30, ((__THIS__.P & N_FLAG) != 0))   /* 30 - BMI */
        //DEF_BRA(50, ((__THIS__.P & V_FLAG) == 0))   /* 50 - BVC */
        //DEF_BRA(70, ((__THIS__.P & V_FLAG) != 0))   /* 70 - BVS */
        //DEF_BRA(90, ((__THIS__.P & C_FLAG) == 0))   /* 90 - BCC */
        //DEF_BRA(B0, ((__THIS__.P & C_FLAG) != 0))   /* B0 - BCS */
        //DEF_BRA(D0, ((__THIS__.P & Z_FLAG) == 0))   /* D0 - BNE */
        //DEF_BRA(F0, ((__THIS__.P & Z_FLAG) != 0))   /* F0 - BEQ */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_BRA(80,1)                               /* 80 - BRA */
        ////#endif
        public void Opcode10(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & N_FLAG) == 0) KM_BRA(pc, rel); }
        public void Opcode30(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & N_FLAG) != 0) KM_BRA(pc, rel); }
        public void Opcode50(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & V_FLAG) == 0) KM_BRA(pc, rel); }
        public void Opcode70(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & V_FLAG) != 0) KM_BRA(pc, rel); }
        public void Opcode90(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & C_FLAG) == 0) KM_BRA(pc, rel); }
        public void OpcodeB0(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & C_FLAG) != 0) KM_BRA(pc, rel); }
        public void OpcodeD0(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & Z_FLAG) == 0) KM_BRA(pc, rel); }
        public void OpcodeF0(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if ((pc.P & Z_FLAG) != 0) KM_BRA(pc, rel); }
        public void Opcode80(K6280_Context pc) { UInt32 rel = K_READ(pc, KA_IMM(pc)); if (true) KM_BRA(pc, rel); }

        /* --- BRK --- */
        public void Opcode00(K6280_Context pc)
        {
            pc.PC = (pc.PC + 1) & 0xffff; pc.iRequest |= IRQ_BRK;
        } /* 00 - BRK */

        //#if BUILD_HUC6280
        /* --- BSR --- */
        public void Opcode44(K6280_Context pc)  /* 44 - BSR */
        {
            KM_PUSH(pc, (pc.PC >> 8) & 0xff);  /* !!! PC = NEXT - 1; !!! */
            KM_PUSH(pc, (pc.PC) & 0xff);
            KM_BRA(pc, K_READ(pc, KA_IMM(pc)));
        }

        /* --- CLA --- */
        public void Opcode62(K6280_Context pc)  /* 62 - CLA */
        { pc.A = 0; }
        /* --- CLX --- */
        public void Opcode82(K6280_Context pc)  /* 82 - CLX */
        { pc.X = 0; }
        /* --- CLY --- */
        public void OpcodeC2(K6280_Context pc)  /* C2 - CLY */
        { pc.Y = 0; }
        //#endif
        /* --- CLC --- */
        public void Opcode18(K6280_Context pc)  /* 18 - CLC */
        { pc.P &= ~(UInt32)C_FLAG; }
        /* --- CLD --- */
        public void OpcodeD8(K6280_Context pc)  /* D8 - CLD */
        { pc.P &= ~(UInt32)D_FLAG; }
        /* --- CLI --- */
        public void Opcode58(K6280_Context pc)  /* 58 - CLI */
        { pc.P &= ~(UInt32)I_FLAG; }
        /* --- CLV --- */
        public void OpcodeB8(K6280_Context pc)  /* B8 - CLV */
        { pc.P &= ~(UInt32)V_FLAG; }

        /* --- CMP --- */
        //#define DEF_CMP(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{
        //KM_CMP(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_CMP(C1, NP, KA_INDX)    /* C1 - CMP - (Indirect,X) */
        //DEF_CMP(C5, ZP, KA_ZP)  /* C5 - CMP - Zero Page */
        //DEF_CMP(C9, NP, KA_IMM) /* C9 - CMP - Immediate */
        //DEF_CMP(CD, NP, KA_ABS) /* CD - CMP - Absolute */
        //DEF_CMP(D1, NP, KA_INDY_)   /* D1 - CMP - (Indirect),Y */
        //DEF_CMP(D5, ZP, KA_ZPX) /* D5 - CMP - Zero Page,X */
        //DEF_CMP(D9, NP, KA_ABSY_)   /* D9 - CMP - Absolute,Y */
        //DEF_CMP(DD, NP, KA_ABSX_)   /* DD - CMP - Absolute,X */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_CMP(D2,NP,KA_IND)	/* D2 - CMP - (Indirect) */
        ////#endif
        public void OpcodeC1(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_INDX(pc))); }
        public void OpcodeC5(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeC9(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeCD(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeD1(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_INDY(pc))); }
        public void OpcodeD5(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeD9(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_ABSY(pc))); }
        public void OpcodeDD(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_ABSX(pc))); }
        public void OpcodeD2(K6280_Context pc) { KM_CMP(pc, K_READ(pc, KA_IND(pc))); }

        /* --- CPX --- */
        //#define DEF_CPX(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{
        //KM_CPX(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_CPX(E0, NP, KA_IMM) /* E0 - CPX - Immediate */
        //DEF_CPX(E4, ZP, KA_ZP)  /* E4 - CPX - Zero Page */
        //DEF_CPX(EC, NP, KA_ABS)	/* EC - CPX - Absolute */
        public void OpcodeE0(K6280_Context pc) { KM_CPX(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeE4(K6280_Context pc) { KM_CPX(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeEC(K6280_Context pc) { KM_CPX(pc, K_READ(pc, KA_ABS(pc))); }

        /* --- CPY --- */
        //#define DEF_CPY(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{
        //KM_CPY(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_CPY(C0, NP, KA_IMM) /* C0 - CPY - Immediate */
        //DEF_CPY(C4, ZP, KA_ZP)  /* C4 - CPY - Zero Page */
        //DEF_CPY(CC, NP, KA_ABS)	/* CC - CPY - Absolute */
        public void OpcodeC0(K6280_Context pc) { KM_CPY(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeC4(K6280_Context pc) { KM_CPY(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeCC(K6280_Context pc) { KM_CPY(pc, K_READ(pc, KA_ABS(pc))); }

        /* --- DEC ---  */
        //#define DEF_DEC(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ \
        //Uword adr = a(__THISP); \
        //K_WRITE##p(__THISP_ adr, KM_DEC(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //DEF_DEC(C6, ZP, KA_ZP)	/* C6 - DEC - Zero Page */
        //DEF_DEC(CE, NP, KA_ABS)	/* CE - DEC - Absolute */
        //DEF_DEC(D6, ZP, KA_ZPX)	/* D6 - DEC - Zero Page,X */
        //DEF_DEC(DE, NP, KA_ABSX)	/* DE - DEC - Absolute,X */
        public void OpcodeC6(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr))); }
        public void OpcodeCE(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr))); }
        public void OpcodeD6(K6280_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr))); }
        public void OpcodeDE(K6280_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_DEC(pc, K_READ(pc, adr))); }

        //#if BUILD_HUC6280 || BUILD_M65C02
        public void Opcode3A(K6280_Context pc)	/* 3A - DEA */
        { pc.A = KM_DEC(pc, pc.A); }
        //#endif
        public void OpcodeCA(K6280_Context pc)  /* CA - DEX */
        { pc.X = KM_DEC(pc, pc.X); }
        public void Opcode88(K6280_Context pc)	/* 88 - DEY */
        { pc.Y = KM_DEC(pc, pc.Y); }

        /* --- EOR ---  */
        //#if BUILD_HUC6280
        //#define DEF_EOR(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ KM_EOR(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KM_EOR(__THISP_ K_READ##p(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        //#else
        //#define DEF_EOR(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{
        //KM_EOR(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //#endif
        //DEF_EOR(41, NP, KA_INDX)    /* 41 - EOR - (Indirect,X) */
        //DEF_EOR(45, ZP, KA_ZP)  /* 45 - EOR - Zero Page */
        //DEF_EOR(49, NP, KA_IMM) /* 49 - EOR - Immediate */
        //DEF_EOR(4D, NP, KA_ABS) /* 4D - EOR - Absolute */
        //DEF_EOR(51, NP, KA_INDY_)   /* 51 - EOR - (Indirect),Y */
        //DEF_EOR(55, ZP, KA_ZPX) /* 55 - EOR - Zero Page,X */
        //DEF_EOR(59, NP, KA_ABSY_)   /* 59 - EOR - Absolute,Y */
        //DEF_EOR(5D, NP, KA_ABSX_)   /* 5D - EOR - Absolute,X */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_EOR(52,NP,KA_IND)   /* 52 - EOR - (Indirect) */
        //                        //#endif
        public void Opcode41(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_INDX(pc))); }
        public void Opcode45(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode49(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_IMM(pc))); }
        public void Opcode4D(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode51(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_INDY(pc))); }
        public void Opcode55(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode59(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_ABSY(pc))); }
        public void Opcode5D(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_ABSX(pc))); }
        public void Opcode52(K6280_Context pc) { KM_EOR(pc, K_READ(pc, KA_IND(pc))); }
        public void T_Opco41(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_INDX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco45(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_ZP(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco49(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_IMM(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco4D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_ABS(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco51(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_INDY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco55(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_ZPX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco59(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_ABSY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco5D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_ABSX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco52(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_EOR(pc, K_READ(pc, KA_IND(pc))); KMI_POSTT(pc, saveA); }

        /* --- INC ---  */
        //#define DEF_INC(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //        { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_INC(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //                                DEF_INC(E6, ZP, KA_ZP)  /* E6 - INC - Zero Page */
        //DEF_INC(EE, NP, KA_ABS) /* EE - INC - Absolute */
        //DEF_INC(F6, ZP, KA_ZPX) /* F6 - INC - Zero Page,X */
        //DEF_INC(FE, NP, KA_ABSX)    /* FE - INC - Absolute,X */
        public void OpcodeE6(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        public void OpcodeEE(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        public void OpcodeF6(K6280_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        public void OpcodeFE(K6280_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_INC(pc, K_READ(pc, adr))); }
        //#if BUILD_HUC6280 || BUILD_M65C02
        public void Opcode1A(K6280_Context pc)  /* 1A - INA */
        { pc.A = KM_INC(pc, pc.A); }
        //#endif
        public void OpcodeE8(K6280_Context pc)  /* E8 - INX */
        { pc.X = KM_INC(pc, pc.X); }
        public void OpcodeC8(K6280_Context pc)  /* C8 - INY */
        { pc.Y = KM_INC(pc, pc.Y); }

        /* --- JMP ---  */
        //#define DEF_JMP(i,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ __THIS__.PC = KI_READWORD(__THISP_ a(__THISP)); }
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //#define DEF_JMPBUG(i,a) DEF_JMP(i,a)
        ////#else
        ////#define DEF_JMPBUG(i,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        ////{ __THIS__.PC = KI_READWORDBUG(__THISP_ a(__THISP)); }
        ////#endif
        //DEF_JMP(4C, KA_IMM16)   /* 4C - JMP - Immediate */
        //DEF_JMPBUG(6C, KA_ABS)  /* 6C - JMP - Absolute */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_JMP(7C,KA_ABSX) /* 7C - JMP - Absolute,X */
        ////#endif
        public void Opcode4C(K6280_Context pc) { pc.PC = KI_READWORD(pc, KA_IMM16(pc)); }
        public void Opcode6C(K6280_Context pc) { pc.PC = KI_READWORD(pc, KA_ABS(pc)); }
        public void Opcode7C(K6280_Context pc) { pc.PC = KI_READWORD(pc, KA_ABSX(pc)); }

        /* --- JSR --- */
        public void Opcode20(K6280_Context pc)  /* 20 - JSR */
        {
            UInt32 adr = KA_IMM(pc);
            KM_PUSH(pc, (pc.PC >> 8) & 0xff);   /* !!! PC = NEXT - 1; !!! */
            KM_PUSH(pc, (pc.PC) & 0xff);
            pc.PC = KI_READWORD(pc, adr);
        }

        /* --- LDA --- */
        //#define DEF_LDA(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                {
        //                                    __THIS__.A = KM_LD(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_LDA(A1, NP, KA_INDX)    /* A1 - LDA - (Indirect,X) */
        //DEF_LDA(A5, ZP, KA_ZP)  /* A5 - LDA - Zero Page */
        //DEF_LDA(A9, NP, KA_IMM) /* A9 - LDA - Immediate */
        //DEF_LDA(AD, NP, KA_ABS) /* AD - LDA - Absolute */
        //DEF_LDA(B1, NP, KA_INDY_)   /* B1 - LDA - (Indirect),Y */
        //DEF_LDA(B5, ZP, KA_ZPX) /* B5 - LDA - Zero Page,X */
        //DEF_LDA(B9, NP, KA_ABSY_)   /* B9 - LDA - Absolute,Y */
        //DEF_LDA(BD, NP, KA_ABSX_)   /* BD - LDA - Absolute,X */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_LDA(B2,NP,KA_IND)	/* B2 - LDA - (Indirect) */
        ////#endif
        public void OpcodeA1(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_INDX(pc))); }
        public void OpcodeA5(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeA9(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeAD(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeB1(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_INDY(pc))); }
        public void OpcodeB5(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeB9(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ABSY(pc))); }
        public void OpcodeBD(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_ABSX(pc))); }
        public void OpcodeB2(K6280_Context pc) { pc.A = KM_LD(pc, K_READ(pc, KA_IND(pc))); }

        /* --- LDX ---  */
        //#define DEF_LDX(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                    {
        //                                        __THIS__.X = KM_LD(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_LDX(A2, NP, KA_IMM) /* A2 - LDX - Immediate */
        //DEF_LDX(A6, ZP, KA_ZP)  /* A6 - LDX - Zero Page */
        //DEF_LDX(AE, NP, KA_ABS) /* AE - LDX - Absolute */
        //DEF_LDX(B6, ZP, KA_ZPY) /* B6 - LDX - Zero Page,Y */
        //DEF_LDX(BE, NP, KA_ABSY_)	/* BE - LDX - Absolute,Y */
        public void OpcodeA2(K6280_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeA6(K6280_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeAE(K6280_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeB6(K6280_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ZPY(pc))); }
        public void OpcodeBE(K6280_Context pc) { pc.X = KM_LD(pc, K_READ(pc, KA_ABSY(pc))); }

        /* --- LDY ---  */
        //#define DEF_LDY(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                        {
        //                                            __THIS__.Y = KM_LD(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_LDY(A0, NP, KA_IMM) /* A0 - LDY - Immediate */
        //DEF_LDY(A4, ZP, KA_ZP)  /* A4 - LDY - Zero Page */
        //DEF_LDY(AC, NP, KA_ABS) /* AC - LDY - Absolute */
        //DEF_LDY(B4, ZP, KA_ZPX) /* B4 - LDY - Zero Page,X */
        //DEF_LDY(BC, NP, KA_ABSX_)	/* BC - LDY - Absolute,X */
        public void OpcodeA0(K6280_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeA4(K6280_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeAC(K6280_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeB4(K6280_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeBC(K6280_Context pc) { pc.Y = KM_LD(pc, K_READ(pc, KA_ABSX(pc))); }

        /* --- LSR ---  */
        //#define DEF_LSR(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                            { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_LSR(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //                                            DEF_LSR(46, ZP, KA_ZP)	/* 46 - LSR - Zero Page */
        //DEF_LSR(4E, NP, KA_ABS)	/* 4E - LSR - Absolute */
        //DEF_LSR(56, ZP, KA_ZPX)	/* 56 - LSR - Zero Page,X */
        //DEF_LSR(5E, NP, KA_ABSX)    /* 5E - LSR - Absolute,X */
        public void Opcode46(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr))); }
        public void Opcode4E(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr))); }
        public void Opcode56(K6280_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr))); }
        public void Opcode5E(K6280_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_LSR(pc, K_READ(pc, adr))); }

        public void Opcode4A(K6280_Context pc)  /* 4A - LSR - Accumulator */
        { pc.A = KM_LSR(pc, pc.A); }

        /* --- NOP ---  */
        public void OpcodeEA(K6280_Context pc)  /* EA - NOP */
        {
        }

        /* --- ORA ---  */
        ////#if BUILD_HUC6280
        //#define DEF_ORA(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ KM_ORA(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall T_Opco##i(__CONTEXT) \
        //{ \
        //	Uword saveA = KMI_PRET(__THISP); \
        //	KM_ORA(__THISP_ K_READ##p(__THISP_ a(__THISP))); \
        //	KMI_POSTT(__THISP_ saveA); \
        //}
        ////#else
        ////#define DEF_ORA(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                            //{
        //                                                //KM_ORA(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        ////#endif
        //DEF_ORA(01, NP, KA_INDX)    /* 01 - ORA - (Indirect,X) */
        //DEF_ORA(05, ZP, KA_ZP)  /* 05 - ORA - Zero Page */
        //DEF_ORA(09, NP, KA_IMM) /* 09 - ORA - Immediate */
        //DEF_ORA(0D, NP, KA_ABS) /* 0D - ORA - Absolute */
        //DEF_ORA(11, NP, KA_INDY_)   /* 11 - ORA - (Indirect),Y */
        //DEF_ORA(15, ZP, KA_ZPX) /* 15 - ORA - Zero Page,X */
        //DEF_ORA(19, NP, KA_ABSY_)   /* 19 - ORA - Absolute,Y */
        //DEF_ORA(1D, NP, KA_ABSX_)   /* 1D - ORA - Absolute,X */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_ORA(12,NP,KA_IND)   /* 12 - ORA - (Indirect) */
        //                        //#endif
        public void Opcode01(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_INDX(pc))); }
        public void Opcode05(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_ZP(pc))); }
        public void Opcode09(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_IMM(pc))); }
        public void Opcode0D(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_ABS(pc))); }
        public void Opcode11(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_INDY(pc))); }
        public void Opcode15(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_ZPX(pc))); }
        public void Opcode19(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_ABSY(pc))); }
        public void Opcode1D(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_ABSX(pc))); }
        public void Opcode12(K6280_Context pc) { KM_ORA(pc, K_READ(pc, KA_IND(pc))); }
        public void T_Opco01(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_INDX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco05(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_ZP(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco09(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_IMM(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco0D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_ABS(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco11(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_INDY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco15(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_ZPX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco19(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_ABSY(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco1D(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_ABSX(pc))); KMI_POSTT(pc, saveA); }
        public void T_Opco12(K6280_Context pc) { UInt32 saveA = KMI_PRET(pc); KM_ORA(pc, K_READ(pc, KA_IND(pc))); KMI_POSTT(pc, saveA); }

        /* --- PHr PLr  --- */
        public void Opcode48(K6280_Context pc)  /* 48 - PHA */
        { KM_PUSH(pc, pc.A); }
        public void Opcode08(K6280_Context pc)  /* 08 - PHP */
        { KM_PUSH(pc, (UInt32)((pc.P | B_FLAG | R_FLAG) & ~T_FLAG)); }
        public void Opcode68(K6280_Context pc)  /* 68 - PLA */
        { pc.A = KM_LD(pc, KM_POP(pc)); }
        public void Opcode28(K6280_Context pc)  /* 28 - PLP */
        { pc.P = (UInt32)(KM_POP(pc) & ~T_FLAG); }
        //#if BUILD_HUC6280 || BUILD_M65C02
        public void OpcodeDA(K6280_Context pc)  /* DA - PHX */
        { KM_PUSH(pc, pc.X); }
        public void Opcode5A(K6280_Context pc)  /* 5A - PHY */
        { KM_PUSH(pc, pc.Y); }
        public void OpcodeFA(K6280_Context pc)  /* FA - PLX */
        { pc.X = KM_LD(pc, KM_POP(pc)); }
        public void Opcode7A(K6280_Context pc)  /* 7A - PLY */
        { pc.Y = KM_LD(pc, KM_POP(pc)); }
        //#endif

        //#if BUILD_HUC6280
        /* --- RMBi --- */
        //#define DEF_RMB(i,y) static void OpcodeCall Opcode##i(__CONTEXT) \
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
        //DEF_RMB(77,7)   /* 77 - RMB7 */
        public void Opcode07(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 0)))); }
        public void Opcode17(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 1)))); }
        public void Opcode27(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 2)))); }
        public void Opcode37(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 3)))); }
        public void Opcode47(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 4)))); }
        public void Opcode57(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 5)))); }
        public void Opcode67(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 6)))); }
        public void Opcode77(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) & (~(1 << 7)))); }

        /* --- SMBi --- */
        //#define DEF_SMB(i,y) static void OpcodeCall Opcode##i(__CONTEXT) \
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
        //DEF_SMB(F7,7)   /* F7 - SMB7 */
        public void Opcode87(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 0))); }
        public void Opcode97(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 1))); }
        public void OpcodeA7(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 2))); }
        public void OpcodeB7(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 3))); }
        public void OpcodeC7(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 4))); }
        public void OpcodeD7(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 5))); }
        public void OpcodeE7(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 6))); }
        public void OpcodeF7(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, (UInt32)(K_READ(pc, adr) | (1 << 7))); }
        //#endif

        /* --- ROL ---  */
        //#define DEF_ROL(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                                { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_ROL(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //                                                DEF_ROL(26, ZP, KA_ZP)  /* 26 - ROL - Zero Page */
        //DEF_ROL(2E, NP, KA_ABS) /* 2E - ROL - Absolute */
        //DEF_ROL(36, ZP, KA_ZPX) /* 36 - ROL - Zero Page,X */
        //DEF_ROL(3E, NP, KA_ABSX)    /* 3E - ROL - Absolute,X */
        public void Opcode26(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode2E(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode36(K6280_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode3E(K6280_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_ROL(pc, K_READ(pc, adr))); }
        public void Opcode2A(K6280_Context pc)  /* 2A - ROL - Accumulator */
        { pc.A = KM_ROL(pc, pc.A); }

        /* --- ROR ---  */
        //#define DEF_ROR(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                                { \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_ROR(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //DEF_ROR(66, ZP, KA_ZP)  /* 66 - ROR - Zero Page */
        //DEF_ROR(6E, NP, KA_ABS) /* 6E - ROR - Absolute */
        //DEF_ROR(76, ZP, KA_ZPX) /* 76 - ROR - Zero Page,X */
        //DEF_ROR(7E, NP, KA_ABSX)    /* 7E - ROR - Absolute,X */
        public void Opcode66(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        public void Opcode6E(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        public void Opcode76(K6280_Context pc) { UInt32 adr = KA_ZPX(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }
        public void Opcode7E(K6280_Context pc) { UInt32 adr = KA_ABSX(pc); K_WRITE(pc, adr, KM_ROR(pc, K_READ(pc, adr))); }

        public void Opcode6A(K6280_Context pc)  /* 6A - ROR - Accumulator */
        { pc.A = KM_ROR(pc, pc.A); }

        public void Opcode40(K6280_Context pc)  /* 40 - RTI */
        {

            pc.P = KM_POP(pc);
            pc.PC = KM_POP(pc);
            pc.PC += KM_POP(pc) << 8;
        }

        public void Opcode60(K6280_Context pc)  /* 60 - RTS */
        {
            pc.PC = KM_POP(pc);
            pc.PC += KM_POP(pc) << 8;
            pc.PC = (pc.PC + 1) & 0xffff;
        }

        //#if BUILD_HUC6280
        public void Opcode22(K6280_Context pc)  /* 22 - SAX */
        {
            UInt32 temp = pc.A;
            pc.A = pc.X;
            pc.X = temp;
        }
        public void Opcode42(K6280_Context pc)  /* 42 - SAY */
        {
            UInt32 temp = pc.A;
            pc.A = pc.Y;
            pc.Y = temp;
        }
        public void Opcode02(K6280_Context pc)  /* 02 - SXY */
        {
            UInt32 temp = pc.Y;
            pc.Y = pc.X;
            pc.X = temp;
        }
        //#endif

        /* --- SBC ---  */
        //#define DEF_SBC(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                                {
        //                                                    KMI_SBC(__THISP_ K_READ##p(__THISP_ a(__THISP))); } \
        //static void OpcodeCall D_Opco##i(__CONTEXT) \
        //{
        //                                                        KMI_SBC_D(__THISP_ K_READ##p(__THISP_ a(__THISP))); }
        //DEF_SBC(E1, NP, KA_INDX)    /* E1 - SBC - (Indirect,X) */
        //DEF_SBC(E5, ZP, KA_ZP)  /* E5 - SBC - Zero Page */
        //DEF_SBC(E9, NP, KA_IMM) /* E9 - SBC - Immediate */
        //DEF_SBC(ED, NP, KA_ABS) /* ED - SBC - Absolute */
        //DEF_SBC(F1, NP, KA_INDY_)   /* F1 - SBC - (Indirect),Y */
        //DEF_SBC(F5, ZP, KA_ZPX) /* F5 - SBC - Zero Page,X */
        //DEF_SBC(F9, NP, KA_ABSY_)   /* F9 - SBC - Absolute,Y */
        //DEF_SBC(FD, NP, KA_ABSX_)	/* FD - SBC - Absolute,X */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_SBC(F2,NP,KA_IND)	/* F2 - SBC - (Indirect) */
        ////#endif
        public void OpcodeE1(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_INDX(pc))); }
        public void OpcodeE5(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ZP(pc))); }
        public void OpcodeE9(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_IMM(pc))); }
        public void OpcodeED(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeF1(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_INDY(pc))); }
        public void OpcodeF5(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeF9(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ABSY(pc))); }
        public void OpcodeFD(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_ABSX(pc))); }
        public void OpcodeF2(K6280_Context pc) { KMI_SBC(pc, K_READ(pc, KA_IND(pc))); }
        public void D_OpcoE1(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_INDX(pc))); }
        public void D_OpcoE5(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ZP(pc))); }
        public void D_OpcoE9(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_IMM(pc))); }
        public void D_OpcoED(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ABS(pc))); }
        public void D_OpcoF1(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_INDY(pc))); }
        public void D_OpcoF5(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ZPX(pc))); }
        public void D_OpcoF9(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ABSY(pc))); }
        public void D_OpcoFD(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_ABSX(pc))); }
        public void D_OpcoF2(K6280_Context pc) { KMI_SBC_D(pc, K_READ(pc, KA_IND(pc))); }

        /* --- SEC --- */
        public void Opcode38(K6280_Context pc)  /* 38 - SEC */
        { pc.P |= C_FLAG; }
        /* --- SED --- */
        public void OpcodeF8(K6280_Context pc)  /* F8 - SED */
        { pc.P |= D_FLAG; }
        /* --- SEI --- */
        public void Opcode78(K6280_Context pc)  /* 78 - SEI */
        { pc.P |= I_FLAG; }

        //#if BUILD_HUC6280
        /* --- SET --- */
        public void OpcodeF4(K6280_Context pc)  /* F4 - SET */
        { pc.P |= T_FLAG; }
        //#endif

        //#if BUILD_HUC6280
        public void Opcode03(K6280_Context pc)  /* 03 - ST0 */
        { K_WRITE6270(pc, 0, K_READ(pc, KA_IMM(pc))); }
        public void Opcode13(K6280_Context pc)  /* 13 - ST1 */
        { K_WRITE6270(pc, 2, K_READ(pc, KA_IMM(pc))); }
        public void Opcode23(K6280_Context pc)  /* 23 - ST2 */
        { K_WRITE6270(pc, 3, K_READ(pc, KA_IMM(pc))); }
        //#endif

        /* --- STA --- */
        //#define DEF_STA(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                                        {
        //                                                            K_WRITE##p(__THISP_ a(__THISP), __THIS__.A); }
        //DEF_STA(81,NP,KA_INDX)	/* 81 - STA - (Indirect,X) */
        //DEF_STA(85, ZP, KA_ZP)  /* 85 - STA - Zero Page */
        //DEF_STA(8D, NP, KA_ABS) /* 8D - STA - Absolute */
        //DEF_STA(91, NP, KA_INDY)    /* 91 - STA - (Indirect),Y */
        //DEF_STA(95, ZP, KA_ZPX) /* 95 - STA - Zero Page,X */
        //DEF_STA(99, NP, KA_ABSY)    /* 99 - STA - Absolute,Y */
        //DEF_STA(9D, NP, KA_ABSX)    /* 9D - STA - Absolute,X */
        ////#if BUILD_HUC6280 || BUILD_M65C02
        //DEF_STA(92,NP,KA_IND)   /* 92 - STA - (Indirect) */
        //                        //#endif
        public void Opcode81(K6280_Context pc) { K_WRITE(pc, KA_INDX(pc), pc.A); }
        public void Opcode85(K6280_Context pc) { K_WRITE(pc, KA_ZP(pc), pc.A); }
        public void Opcode8D(K6280_Context pc) { K_WRITE(pc, KA_ABS(pc), pc.A); }
        public void Opcode91(K6280_Context pc) { K_WRITE(pc, KA_INDY(pc), pc.A); }
        public void Opcode95(K6280_Context pc) { K_WRITE(pc, KA_ZPX(pc), pc.A); }
        public void Opcode99(K6280_Context pc) { K_WRITE(pc, KA_ABSY(pc), pc.A); }
        public void Opcode9D(K6280_Context pc) { K_WRITE(pc, KA_ABSX(pc), pc.A); }
        public void Opcode92(K6280_Context pc) { K_WRITE(pc, KA_IND(pc), pc.A); }

        /* --- STX ---  */
        //#define DEF_STX(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{
        //K_WRITE##p(__THISP_ a(__THISP), __THIS__.X); }
        //DEF_STX(86,ZP,KA_ZP)	/* 86 - STX - Zero Page */
        //DEF_STX(8E, NP, KA_ABS)	/* 8E - STX - Absolute */
        //DEF_STX(96, ZP, KA_ZPY) /* 96 - STX - Zero Page,Y */
        public void Opcode86(K6280_Context pc) { K_WRITE(pc, KA_ZP(pc), pc.X); }
        public void Opcode8E(K6280_Context pc) { K_WRITE(pc, KA_ABS(pc), pc.X); }
        public void Opcode96(K6280_Context pc) { K_WRITE(pc, KA_ZPY(pc), pc.X); }

        /* --- STY ---  */
        //#define DEF_STY(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //                                                {
        //                                                    K_WRITE##p(__THISP_ a(__THISP), __THIS__.Y); }
        //DEF_STY(84,ZP,KA_ZP)	/* 84 - STY - Zero Page */
        //DEF_STY(8C, NP, KA_ABS)	/* 8C - STY - Absolute */
        //DEF_STY(94, ZP, KA_ZPX) /* 94 - STY - Zero Page,X */
        public void Opcode84(K6280_Context pc) { K_WRITE(pc, KA_ZP(pc), pc.Y); }
        public void Opcode8C(K6280_Context pc) { K_WRITE(pc, KA_ABS(pc), pc.Y); }
        public void Opcode94(K6280_Context pc) { K_WRITE(pc, KA_ZPX(pc), pc.Y); }

        //#if BUILD_HUC6280 || BUILD_M65C02
        /* --- STZ ---  */
        //#define DEF_STZ(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ K_WRITE##p(__THISP_ a(__THISP), 0); }
        //DEF_STZ(64,ZP,KA_ZP)	/* 64 - STZ - Zero Page */
        //DEF_STZ(9C,NP,KA_ABS)	/* 9C - STZ - Absolute */
        //DEF_STZ(74,ZP,KA_ZPX)	/* 74 - STZ - Zero Page,X */
        //DEF_STZ(9E,NP,KA_ABSX)  /* 9E - STZ - Absolute,X */
        public void Opcode64(K6280_Context pc) { K_WRITE(pc, KA_ZP(pc), 0); }
        public void Opcode9C(K6280_Context pc) { K_WRITE(pc, KA_ABS(pc), 0); }
        public void Opcode74(K6280_Context pc) { K_WRITE(pc, KA_ZPX(pc), 0); }
        public void Opcode9E(K6280_Context pc) { K_WRITE(pc, KA_ABSX(pc), 0); }
        //#endif

        //#if BUILD_HUC6280
        /* --- TAMi ---  */
        public void Opcode53(K6280_Context pc)  /* 53 - TAMi */
        { K_WRITEMPR(pc, K_READ(pc, KA_IMM(pc)), pc.A); }
        /* --- TMAi ---  */
        public void Opcode43(K6280_Context pc)  /* 43 - TMAi */
        { pc.A = K_READMPR(pc, K_READ(pc, KA_IMM(pc))); }
        //#endif

        //#if BUILD_HUC6280 || BUILD_M65C02
        /* --- TRB --- */
        //#define DEF_TRB(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_TRB(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //DEF_TRB(14,ZP,KA_ZP)	/* 14 - TRB - Zero Page */
        //DEF_TRB(1C,NP,KA_ABS)   /* 1C - TRB - Absolute */
        public void Opcode14(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_TRB(pc, K_READ(pc, adr))); }
        public void Opcode1C(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_TRB(pc, K_READ(pc, adr))); }

        /* --- TSB --- */
        //#define DEF_TSB(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ \
        //	Uword adr = a(__THISP); \
        //	K_WRITE##p(__THISP_ adr, KM_TSB(__THISP_ K_READ##p(__THISP_ adr))); \
        //}
        //DEF_TSB(04,ZP,KA_ZP)	/* 04 - TSB - Zero Page */
        //DEF_TSB(0C,NP,KA_ABS)	/* 0C - TSB - Absolute */
        public void Opcode04(K6280_Context pc) { UInt32 adr = KA_ZP(pc); K_WRITE(pc, adr, KM_TSB(pc, K_READ(pc, adr))); }
        public void Opcode0C(K6280_Context pc) { UInt32 adr = KA_ABS(pc); K_WRITE(pc, adr, KM_TSB(pc, K_READ(pc, adr))); }
        //#endif

        //#if BUILD_HUC6280
        /* --- TST --- */
        //#define DEF_TST(i,p,a) static void OpcodeCall Opcode##i(__CONTEXT) \
        //{ \
        //	Uword imm = K_READNP(__THISP_ KA_IMM(__THISP)); \
        //	KM_TST(__THISP_ imm, K_READ##p(__THISP_ a(__THISP))); \
        //}
        //DEF_TST(83,ZP,KA_ZP)	/* 83 - TST - Zero Page */
        //DEF_TST(93,NP,KA_ABS)	/* 93 - TST - Absolute */
        //DEF_TST(A3,ZP,KA_ZPX)	/* A3 - TST - Zero Page,X */
        //DEF_TST(B3,NP,KA_ABSX)	/* B3 - TST - Absolute,X */
        public void Opcode83(K6280_Context pc) { UInt32 imm = K_READ(pc, KA_IMM(pc)); KM_TST(pc, imm, K_READ(pc, KA_ZP(pc))); }
        public void Opcode93(K6280_Context pc) { UInt32 imm = K_READ(pc, KA_IMM(pc)); KM_TST(pc, imm, K_READ(pc, KA_ABS(pc))); }
        public void OpcodeA3(K6280_Context pc) { UInt32 imm = K_READ(pc, KA_IMM(pc)); KM_TST(pc, imm, K_READ(pc, KA_ZPX(pc))); }
        public void OpcodeB3(K6280_Context pc) { UInt32 imm = K_READ(pc, KA_IMM(pc)); KM_TST(pc, imm, K_READ(pc, KA_ABSX(pc))); }
        //#endif

        /* --- TAX ---  */
        public void OpcodeAA(K6280_Context pc)  /* AA - TAX */
        { pc.X = KM_LD(pc, pc.A); }
        /* --- TAY ---  */
        public void OpcodeA8(K6280_Context pc)  /* A8 - TAY */
        { pc.Y = KM_LD(pc, pc.A); }
        /* --- TSX ---  */
        public void OpcodeBA(K6280_Context pc)  /* BA - TSX */
        { pc.X = KM_LD(pc, pc.S); }
        /* --- TXA ---  */
        public void Opcode8A(K6280_Context pc)  /* 8A - TXA */
        { pc.A = KM_LD(pc, pc.X); }
        /* --- TXS ---  */
        public void Opcode9A(K6280_Context pc)  /* 9A - TXS */
        { pc.S = pc.X; }
        /* --- TYA ---  */
        public void Opcode98(K6280_Context pc)  /* 98 - TYA */
        { pc.A = KM_LD(pc, pc.Y); }

        //#if BUILD_HUC6280
        public void Opcode73(K6280_Context pc)  /* 73 - TII */
        {
            UInt32 src, des, len;
            src = KI_READWORD(pc, KA_IMM16(pc));
            des = KI_READWORD(pc, KA_IMM16(pc));
            len = KI_READWORD(pc, KA_IMM16(pc));
            KI_ADDCLOCK(pc, (UInt32)(len != 0 ? len * 6 : (UInt32)0x60000));
            do
            {
                K_WRITE(pc, des, K_READ(pc, src));
                src = (src + 1) & 0xffff;
                des = (des + 1) & 0xffff;
                len = (len - 1) & 0xffff;
            } while (len != 0);
        }

        public void OpcodeC3(K6280_Context pc)  /* C3 - TDD */
        {
            UInt32 src, des, len;
            src = KI_READWORD(pc, KA_IMM16(pc));
            des = KI_READWORD(pc, KA_IMM16(pc));
            len = KI_READWORD(pc, KA_IMM16(pc));
            KI_ADDCLOCK(pc, (UInt32)(len != 0 ? len * 6 : (UInt32)0x60000));
            do
            {
                K_WRITE(pc, des, K_READ(pc, src));
                src = (src - 1) & 0xffff;
                des = (des - 1) & 0xffff;
                len = (len - 1) & 0xffff;
            } while (len != 0);
        }

        public void OpcodeD3(K6280_Context pc)  /* D3 - TIN */
        {
            UInt32 src, des, len;
            src = KI_READWORD(pc, KA_IMM16(pc));
            des = KI_READWORD(pc, KA_IMM16(pc));
            len = KI_READWORD(pc, KA_IMM16(pc));
            KI_ADDCLOCK(pc, (UInt32)(len != 0 ? len * 6 : (UInt32)0x60000));
            do
            {
                K_WRITE(pc, des, K_READ(pc, src));
                src = (src + 1) & 0xffff;
                len = (len - 1) & 0xffff;
            } while (len != 0);
        }

        public void OpcodeE3(K6280_Context pc)  /* E3 - TIA */
        {
            int add = +1;
            UInt32 src, des, len;
            src = KI_READWORD(pc, KA_IMM16(pc));
            des = KI_READWORD(pc, KA_IMM16(pc));
            len = KI_READWORD(pc, KA_IMM16(pc));
            KI_ADDCLOCK(pc, (UInt32)(len != 0 ? len * 6 : (UInt32)0x60000));
            do
            {
                K_WRITE(pc, des, K_READ(pc, src));
                src = (src + 1) & 0xffff;
                des = (UInt32)((des + add) & 0xffff);
                add = -add;
                len = (len - 1) & 0xffff;
            } while (len != 0);
        }

        public void OpcodeF3(K6280_Context pc)  /* F3 - TAI */
        {
            int add = +1;
            UInt32 src, des, len;
            src = KI_READWORD(pc, KA_IMM16(pc));
            des = KI_READWORD(pc, KA_IMM16(pc));
            len = KI_READWORD(pc, KA_IMM16(pc));
            KI_ADDCLOCK(pc, (UInt32)(len != 0 ? len * 6 : (UInt32)0x60000));
            do
            {
                K_WRITE(pc, des, K_READ(pc, src));
                src = (UInt32)((src + add) & 0xffff);
                des = (des + 1) & 0xffff;
                add = -add;
                len = (len - 1) & 0xffff;
            } while (len != 0);
        }

        public void Opcode54(K6280_Context pc)  /* 54 - CSL */
        { pc.lowClockMode = 1; }

        public void OpcodeD4(K6280_Context pc)  /* D4 - CSH */
        { pc.lowClockMode = 0; }
        //#endif
        //-------------------------------- km6502cd.h END

        //# include "km6502ct.h"
        //-------------------------------- km6502ct.h START
        //#if BUILD_HUC6280

        /*

         HuC6280 clock cycle table

         -0         undefined OP-code
         BRK(#$00)  +7 by interrupt

        */
        public byte[] cl_table = new byte[256]{
/* L 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F     H */
	 1, 7, 3, 4, 6, 4, 6, 7, 3, 2, 2,-0, 7, 5, 7, 6, /* 0 */
	 2, 7, 7, 4, 6, 4, 6, 7, 2, 5, 2,-0, 7, 5, 7, 6, /* 1 */
	 7, 7, 3, 4, 4, 4, 6, 7, 3, 2, 2,-0, 5, 5, 7, 6, /* 2 */
	 2, 7, 7,-0, 4, 4, 6, 7, 2, 5, 2,-0, 5, 5, 7, 6, /* 3 */
	 7, 7, 3, 4, 8, 4, 6, 7, 3, 2, 2,-0, 4, 5, 7, 6, /* 4 */
	 2, 7, 7, 5,2 , 4, 6, 7, 2, 5, 3,-0,-0, 5, 7, 6, /* 5 */
	 7, 7, 2,-0, 4, 4, 6, 7, 3, 2, 2,-0, 7, 5, 7, 6, /* 6 */
	 2, 7, 7,17, 4, 4, 6, 7, 2, 5, 3,-0, 7, 5, 7, 6, /* 7 */
	 2, 7, 2, 7, 4, 4, 4, 7, 2, 2, 2,-0, 5, 5, 5, 6, /* 8 */
	 2, 7, 7, 8, 4, 4, 4, 7, 2, 5, 2,-0, 5, 5, 5, 6, /* 9 */
	 2, 7, 2, 7, 4, 4, 4, 7, 2, 2, 2,-0, 5, 5, 5, 6, /* A */
	 2, 7, 7, 8, 4, 4, 4, 7, 2, 5, 2,-0, 5, 5, 5, 6, /* B */
	 2, 7, 2,17, 4, 4, 6, 7, 2, 2, 2,-0, 5, 5, 7, 6, /* C */
	 2, 7, 7,17,2 , 4, 6, 7, 2, 5, 3,-0,-0, 5, 7, 6, /* D */
	 2, 7,-0,17, 4, 4, 6, 7, 2, 2, 2,-0, 5, 5, 7, 6, /* E */
	 2, 7, 7,17, 2, 4, 6, 7, 2, 5, 3,-0,-0, 5, 7, 6, /* F */
};
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

        //        /*

        //         m6502 clock cycle table

        //         (n)        undefined OP-code
        //         +n         +1 by page boundary case
        //         BRK(#$00)  +7 by interrupt

        //        */
        //        const static Ubyte cl_table[256] = {
        ///* L 0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F      H */
        //	 0 , 6 ,(2),(8),(3), 3 , 5 ,(5), 3 , 2 , 2 ,(2),(4), 4 , 6 ,(6), /* 0 */
        //	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(5),+4 , 7 ,(7), /* 1 */
        //	 6 , 6 ,(2),(8), 3 , 3 , 5 ,(5), 4 , 2 , 2 ,(2), 4 , 4 , 6 ,(6), /* 2 */
        //	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(5),+4 , 7 ,(7), /* 3 */
        //	 6 , 6 ,(2),(8),(3), 3 , 5 ,(5), 3 , 2 , 2 ,(2), 3 , 4 , 6 ,(6), /* 4 */
        //	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(5),+4 , 7 ,(7), /* 5 */
        //	 6 , 6 ,(2),(8),(3), 3 , 5 ,(5), 4 , 2 , 2 ,(2), 5 , 4 , 6 ,(6), /* 6 */
        //	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(5),+4 , 7 ,(7), /* 7 */
        //	(2), 6 ,(2),(6), 3 , 3 , 3 ,(3), 2 ,(2), 2 ,(2), 4 , 4 , 4 ,(4), /* 8 */
        //	 2 , 6 ,(2),(6), 4 , 4 , 4 ,(4), 2 , 5 , 2 ,(5),(5), 5 ,(5),(5), /* 9 */
        //	 2 , 6 , 2 ,(6), 3 , 3 , 3 ,(3), 2 , 2 , 2 ,(2), 4 , 4 , 4 ,(4), /* A */
        //	 2 ,+5 ,(2),(5), 4 , 4 , 4 ,(4), 2 ,+4 , 2 ,(5),+4 ,+4 ,+4 ,(4), /* B */
        //	 2 , 6 ,(2),(8), 3 , 3 , 5 ,(5), 2 , 2 , 2 ,(2), 4 , 4 , 6 ,(6), /* C */
        //	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(5),+4 , 7 ,(7), /* D */
        //	 2 , 6 ,(2),(8), 3 , 3 , 5 ,(5), 2 , 2 , 2 ,(2), 4 , 4 , 6 ,(6), /* E */
        //	 2 ,+5 ,(2),(8),(4), 4 , 6 ,(6), 2 ,+4 ,(2),(7),(5),+4 , 7 ,(7), /* F */
        //};
        //#endif

        //-------------------------------- km6502ct.h END

        //# include "km6502ot.h"
        //-------------------------------- km6502ot.h START
        //#define OPxx(i)
        //#define OP__(i) 
        //	case 0x##i: 
        //		Opcode##i(pc); 
        //		break;
        ////#if BUILD_2A03 //2A03にDecimalModeなんてねーよｗ
        ////#define OP_d(i) \
        ////	case 0x##i: \
        ////		Opcode##i(__THISP); \
        ////		break;
        ////#else
        //#define OP_d(i) 
        //	case 0x##i: 
        //		if (pc.P & D_FLAG) D_Opco##i(pc);
        //		else Opcode##i(pc); 
        //		break;
        ////#endif
        ////#if BUILD_HUC6280
        //#define OPtd(i) 
        //	case 0x##i: 
        //		if (pc.P & T_FLAG) 
        //			if (pc.P & D_FLAG) TD_Opc##i(pc); 
        //			else T_Opco##i(pc);
        //		else 
        //			if (pc.P & D_FLAG) D_Opco##i(pc);
        //			else Opcode##i(pc);
        //		break;
        //#define OPt_(i) 
        //	case 0x##i: 
        //		if (pc.P & T_FLAG) T_Opco##i(pc);
        //		else Opcode##i(pc);
        //		break;
        ////#else
        ////#define OPtd OP_d
        ////#define OPt_ OP__
        ////#endif

        public void K_OPEXEC(K6280_Context pc)
        {
            UInt32 opcode = pc.lastcode = K_READ(pc, KAI_IMM(pc));
            KI_ADDCLOCK(pc, cl_table[opcode]);
            switch (opcode)
            {

                //OP__(00)    OPt_(01)    OPxx(02)    OPxx(04)    OPt_(05)    OP__(06)
                case 0x00: Opcode00(pc); break;
                case 0x01: if ((pc.P & T_FLAG) != 0) T_Opco01(pc); else Opcode01(pc); break;
                case 0x05: if ((pc.P & T_FLAG) != 0) T_Opco05(pc); else Opcode05(pc); break;
                case 0x06: Opcode06(pc); break;

                //OP__(08)    OPt_(09)    OP__(0A)    OPxx(0C)    OPt_(0D)    OP__(0E)
                case 0x08: Opcode08(pc); break;
                case 0x09: if ((pc.P & T_FLAG) != 0) T_Opco09(pc); else Opcode09(pc); break;
                case 0x0A: Opcode0A(pc); break;
                case 0x0D: if ((pc.P & T_FLAG) != 0) T_Opco0D(pc); else Opcode0D(pc); break;
                case 0x0E: Opcode0E(pc); break;

                //OP__(10)    OPt_(11)    OPxx(12)    OPxx(14)    OPt_(15)    OP__(16)
                case 0x10: Opcode10(pc); break;
                case 0x11: if ((pc.P & T_FLAG) != 0) T_Opco11(pc); else Opcode11(pc); break;
                case 0x15: if ((pc.P & T_FLAG) != 0) T_Opco15(pc); else Opcode15(pc); break;
                case 0x16: Opcode16(pc); break;

                //OP__(18)    OPt_(19)    OPxx(1A)    OPxx(1C)    OPt_(1D)    OP__(1E)
                case 0x18: Opcode18(pc); break;
                case 0x19: if ((pc.P & T_FLAG) != 0) T_Opco19(pc); else Opcode19(pc); break;
                case 0x1D: if ((pc.P & T_FLAG) != 0) T_Opco1D(pc); else Opcode1D(pc); break;
                case 0x1E: Opcode1E(pc); break;

                //OP__(20)    OP__(21)    OPxx(22)    OP__(24)    OP__(25)    OP__(26)
                case 0x20: Opcode20(pc); break;
                case 0x21: Opcode21(pc); break;
                case 0x24: Opcode24(pc); break;
                case 0x25: Opcode25(pc); break;
                case 0x26: Opcode26(pc); break;

                //OP__(28)    OP__(29)    OP__(2A)    OP__(2C)    OP__(2D)    OP__(2E)
                case 0x28: Opcode28(pc); break;
                case 0x29: Opcode29(pc); break;
                case 0x2A: Opcode2A(pc); break;
                case 0x2C: Opcode2C(pc); break;
                case 0x2D: Opcode2D(pc); break;
                case 0x2E: Opcode2E(pc); break;

                //OP__(30)    OPt_(31)    OPxx(32)    OPxx(34)    OPt_(35)    OP__(36)
                case 0x30: Opcode30(pc); break;
                case 0x31: if ((pc.P & T_FLAG) != 0) T_Opco31(pc); else Opcode31(pc); break;
                case 0x35: if ((pc.P & T_FLAG) != 0) T_Opco35(pc); else Opcode35(pc); break;
                case 0x36: Opcode36(pc); break;

                //OP__(38)    OPt_(39)    OPxx(3A)    OPxx(3C)    OPt_(3D)    OP__(3E)
                case 0x38: Opcode38(pc); break;
                case 0x39: if ((pc.P & T_FLAG) != 0) T_Opco39(pc); else Opcode39(pc); break;
                case 0x3D: if ((pc.P & T_FLAG) != 0) T_Opco3D(pc); else Opcode3D(pc); break;
                case 0x3E: Opcode3E(pc); break;

                //OP__(40)    OPt_(41)    OPxx(42)    OPxx(44)    OPt_(45)    OP__(46)
                case 0x40: Opcode40(pc); break;
                case 0x41: if ((pc.P & T_FLAG) != 0) T_Opco41(pc); else Opcode41(pc); break;
                case 0x45: if ((pc.P & T_FLAG) != 0) T_Opco45(pc); else Opcode45(pc); break;
                case 0x46: Opcode46(pc); break;

                //OP__(48)    OPt_(49)    OP__(4A)    OP__(4C)    OPt_(4D)    OP__(4E)
                case 0x48: Opcode48(pc); break;
                case 0x49: if ((pc.P & T_FLAG) != 0) T_Opco49(pc); else Opcode49(pc); break;
                case 0x4A: Opcode4A(pc); break;
                case 0x4C: Opcode4C(pc); break;
                case 0x4D: if ((pc.P & T_FLAG) != 0) T_Opco4D(pc); else Opcode4D(pc); break;
                case 0x4E: Opcode4E(pc); break;

                //OP__(50)    OPt_(51)    OPxx(52)    OPxx(54)    OPt_(55)    OP__(56)
                case 0x50: Opcode50(pc); break;
                case 0x51: if ((pc.P & T_FLAG) != 0) T_Opco51(pc); else Opcode51(pc); break;
                case 0x55: if ((pc.P & T_FLAG) != 0) T_Opco55(pc); else Opcode55(pc); break;
                case 0x56: Opcode56(pc); break;

                //OP__(58)    OPt_(59)    OPxx(5A)    OPxx(5C)    OPt_(5D)    OP__(5E)
                case 0x58: Opcode58(pc); break;
                case 0x59: if ((pc.P & T_FLAG) != 0) T_Opco59(pc); else Opcode59(pc); break;
                case 0x5D: if ((pc.P & T_FLAG) != 0) T_Opco5D(pc); else Opcode5D(pc); break;
                case 0x5E: Opcode5E(pc); break;

                //OP__(60)    OPtd(61)    OPxx(62)    OPxx(64)    OPtd(65)    OP__(66)
                case 0x60: Opcode60(pc); break;
                case 0x61: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc61(pc); else T_Opco61(pc); else if ((pc.P & D_FLAG) != 0) D_Opco61(pc); else Opcode61(pc); break;
                case 0x65: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc65(pc); else T_Opco65(pc); else if ((pc.P & D_FLAG) != 0) D_Opco65(pc); else Opcode65(pc); break;
                case 0x66: Opcode66(pc); break;

                //OP__(68)    OPtd(69)    OP__(6A)    OP__(6C)    OPtd(6D)    OP__(6E)
                case 0x68: Opcode68(pc); break;
                case 0x69: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc69(pc); else T_Opco69(pc); else if ((pc.P & D_FLAG) != 0) D_Opco69(pc); else Opcode69(pc); break;
                case 0x6A: Opcode6A(pc); break;
                case 0x6C: Opcode6C(pc); break;
                case 0x6D: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc6D(pc); else T_Opco6D(pc); else if ((pc.P & D_FLAG) != 0) D_Opco6D(pc); else Opcode6D(pc); break;
                case 0x6E: Opcode6E(pc); break;

                //OP__(70)    OPtd(71)    OPxx(72)    OPxx(74)    OPtd(75)    OP__(76)
                case 0x70: Opcode70(pc); break;
                case 0x71: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc71(pc); else T_Opco71(pc); else if ((pc.P & D_FLAG) != 0) D_Opco71(pc); else Opcode71(pc); break;
                case 0x75: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc75(pc); else T_Opco75(pc); else if ((pc.P & D_FLAG) != 0) D_Opco75(pc); else Opcode75(pc); break;
                case 0x76: Opcode76(pc); break;

                //OP__(78)    OPtd(79)    OPxx(7A)    OPxx(7C)    OPtd(7D)    OP__(7E)
                case 0x78: Opcode78(pc); break;
                case 0x79: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc79(pc); else T_Opco79(pc); else if ((pc.P & D_FLAG) != 0) D_Opco79(pc); else Opcode79(pc); break;
                case 0x7D: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc7D(pc); else T_Opco7D(pc); else if ((pc.P & D_FLAG) != 0) D_Opco7D(pc); else Opcode7D(pc); break;
                case 0x7E: Opcode7E(pc); break;

                //OPxx(80)    OP__(81)    OPxx(82)    OP__(84)    OP__(85)    OP__(86)
                case 0x81: Opcode81(pc); break;
                case 0x84: Opcode84(pc); break;
                case 0x85: Opcode85(pc); break;
                case 0x86: Opcode86(pc); break;

                //OP__(88)    OPxx(89)    OP__(8A)    OP__(8C)    OP__(8D)    OP__(8E)
                case 0x88: Opcode88(pc); break;
                case 0x8A: Opcode8A(pc); break;
                case 0x8C: Opcode8C(pc); break;
                case 0x8D: Opcode8D(pc); break;
                case 0x8E: Opcode8E(pc); break;

                //OP__(90)    OP__(91)    OPxx(92)    OP__(94)    OP__(95)    OP__(96)
                case 0x90: Opcode90(pc); break;
                case 0x91: Opcode91(pc); break;
                case 0x94: Opcode94(pc); break;
                case 0x95: Opcode95(pc); break;
                case 0x96: Opcode96(pc); break;

                //OP__(98)    OP__(99)    OP__(9A)    OPxx(9C)    OP__(9D)    OPxx(9E)
                case 0x98: Opcode98(pc); break;
                case 0x99: Opcode99(pc); break;
                case 0x9A: Opcode9A(pc); break;
                case 0x9D: Opcode9D(pc); break;

                //OP__(A0)    OP__(A1)    OP__(A2)    OP__(A4)    OP__(A5)    OP__(A6)
                case 0xA0: OpcodeA0(pc); break;
                case 0xA1: OpcodeA1(pc); break;
                case 0xA2: OpcodeA2(pc); break;
                case 0xA4: OpcodeA4(pc); break;
                case 0xA5: OpcodeA5(pc); break;
                case 0xA6: OpcodeA6(pc); break;

                //OP__(A8)    OP__(A9)    OP__(AA)    OP__(AC)    OP__(AD)    OP__(AE)
                case 0xA8: OpcodeA8(pc); break;
                case 0xA9: OpcodeA9(pc); break;
                case 0xAA: OpcodeAA(pc); break;
                case 0xAC: OpcodeAC(pc); break;
                case 0xAD: OpcodeAD(pc); break;
                case 0xAE: OpcodeAE(pc); break;

                //OP__(B0)    OP__(B1)    OPxx(B2)    OP__(B4)    OP__(B5)    OP__(B6)
                case 0xB0: OpcodeB0(pc); break;
                case 0xB1: OpcodeB1(pc); break;
                case 0xB4: OpcodeB4(pc); break;
                case 0xB5: OpcodeB5(pc); break;
                case 0xB6: OpcodeB6(pc); break;

                //OP__(B8)    OP__(B9)    OP__(BA)    OP__(BC)    OP__(BD)    OP__(BE)
                case 0xB8: OpcodeB8(pc); break;
                case 0xB9: OpcodeB9(pc); break;
                case 0xBA: OpcodeBA(pc); break;
                case 0xBC: OpcodeBC(pc); break;
                case 0xBD: OpcodeBD(pc); break;
                case 0xBE: OpcodeBE(pc); break;

                //OP__(C0)    OP__(C1)    OPxx(C2)    OP__(C4)    OP__(C5)    OP__(C6)
                case 0xC0: OpcodeC0(pc); break;
                case 0xC1: OpcodeC1(pc); break;
                case 0xC4: OpcodeC4(pc); break;
                case 0xC5: OpcodeC5(pc); break;
                case 0xC6: OpcodeC6(pc); break;

                //OP__(C8)    OP__(C9)    OP__(CA)    OP__(CC)    OP__(CD)    OP__(CE)
                case 0xC8: OpcodeC8(pc); break;
                case 0xC9: OpcodeC9(pc); break;
                case 0xCA: OpcodeCA(pc); break;
                case 0xCC: OpcodeCC(pc); break;
                case 0xCD: OpcodeCD(pc); break;
                case 0xCE: OpcodeCE(pc); break;

                //OP__(D0)    OP__(D1)    OPxx(D2)    OPxx(D4)    OP__(D5)    OP__(D6)
                case 0xD0: OpcodeD0(pc); break;
                case 0xD1: OpcodeD1(pc); break;
                case 0xD5: OpcodeD5(pc); break;
                case 0xD6: OpcodeD6(pc); break;

                //OP__(D8)    OP__(D9)    OPxx(DA)    OPxx(DC)    OP__(DD)    OP__(DE)
                case 0xD8: OpcodeD8(pc); break;
                case 0xD9: OpcodeD9(pc); break;
                case 0xDD: OpcodeDD(pc); break;
                case 0xDE: OpcodeDE(pc); break;

                //OP__(E0)    OP_d(E1)    OPxx(E2)    OP__(E4)    OP_d(E5)    OP__(E6)
                case 0xE0: OpcodeE0(pc); break;
                case 0xE1: if ((pc.P & D_FLAG) != 0) D_OpcoE1(pc); else OpcodeE1(pc); break;
                case 0xE4: OpcodeE4(pc); break;
                case 0xE5: if ((pc.P & D_FLAG) != 0) D_OpcoE5(pc); else OpcodeE5(pc); break;
                case 0xE6: OpcodeE6(pc); break;

                //OP__(E8)    OP_d(E9)    OP__(EA)    OP__(EC)    OP_d(ED)    OP__(EE)
                case 0xE8: OpcodeE8(pc); break;
                case 0xE9: if ((pc.P & D_FLAG) != 0) D_OpcoE9(pc); else OpcodeE9(pc); break;
                case 0xEA: OpcodeEA(pc); break;
                case 0xEC: OpcodeEC(pc); break;
                case 0xED: if ((pc.P & D_FLAG) != 0) D_OpcoED(pc); else OpcodeED(pc); break;
                case 0xEE: OpcodeEE(pc); break;

                //OP__(F0)    OP_d(F1)    OPxx(F2)    OPxx(F4)    OP_d(F5)    OP__(F6)
                case 0xF0: OpcodeF0(pc); break;
                case 0xF1: if ((pc.P & D_FLAG) != 0) D_OpcoF1(pc); else OpcodeF1(pc); break;
                case 0xF5: if ((pc.P & D_FLAG) != 0) D_OpcoF5(pc); else OpcodeF5(pc); break;
                case 0xF6: OpcodeF6(pc); break;

                //OP__(F8)    OP_d(F9)    OPxx(FA)    OPxx(FC)    OP_d(FD)    OP__(FE)
                case 0xF8: OpcodeF8(pc); break;
                case 0xF9: if ((pc.P & D_FLAG) != 0) D_OpcoF9(pc); else OpcodeF9(pc); break;
                case 0xFD: if ((pc.P & D_FLAG) != 0) D_OpcoFD(pc); else OpcodeFD(pc); break;
                case 0xFE: OpcodeFE(pc); break;

                //#if BUILD_HUC6280 || BUILD_M65C02
                //OP__(34)	/* 34 - BIT - Zero Page,X */
                //OP__(3C)	/* 3C - BIT - Absolute,X */
                //OP__(80)	/* 80 - BRA */
                //OP__(3A)	/* 3A - DEA */
                //OP__(1A)	/* 1A - INA */
                case 0x34: Opcode34(pc); break;
                case 0x3C: Opcode3C(pc); break;
                case 0x80: Opcode80(pc); break;
                case 0x3A: Opcode3A(pc); break;
                case 0x1A: Opcode1A(pc); break;

                //OP__(89)	/* 89 - BIT - Immediate */
                case 0x89: Opcode89(pc); break;

                //OP__(04)	OP__(0C)	/* TSB */
                case 0x04: Opcode04(pc); break;
                case 0x0C: Opcode0C(pc); break;

                //OP__(14)	OP__(1C)	/* TRB */
                case 0x14: Opcode14(pc); break;
                case 0x1C: Opcode1C(pc); break;

                //OPt_(12)	/* 12 - ORA - (Indirect) */
                //OPt_(32)	/* 32 - AND - (Indirect) */
                //OPt_(52)	/* 52 - EOR - (Indirect) */
                //OPtd(72)	/* 72 - ADC - (Indirect) */
                //OP__(92)	/* 92 - STA - (Indirect) */
                //OP__(B2)	/* B2 - LDA - (Indirect) */
                //OP__(D2)	/* D2 - CMP - (Indirect) */
                //OP_d(F2)	/* F2 - SBC - (Indirect) */
                case 0x12: if ((pc.P & T_FLAG) != 0) T_Opco12(pc); else Opcode12(pc); break;
                case 0x32: if ((pc.P & T_FLAG) != 0) T_Opco32(pc); else Opcode32(pc); break;
                case 0x52: if ((pc.P & T_FLAG) != 0) T_Opco52(pc); else Opcode52(pc); break;
                case 0x72: if ((pc.P & T_FLAG) != 0) if ((pc.P & D_FLAG) != 0) TD_Opc72(pc); else T_Opco72(pc); else if ((pc.P & D_FLAG) != 0) D_Opco72(pc); else Opcode72(pc); break;
                case 0x92: Opcode92(pc); break;
                case 0xB2: OpcodeB2(pc); break;
                case 0xD2: OpcodeD2(pc); break;
                case 0xF2: if ((pc.P & D_FLAG) != 0) D_OpcoF2(pc); else OpcodeF2(pc); break;

                //OP__(DA)	OP__(5A)	OP__(FA)	OP__(7A)	/* PHX PHY PLX PLY */
                case 0xDA: OpcodeDA(pc); break;
                case 0x5A: Opcode5A(pc); break;
                case 0xFA: OpcodeFA(pc); break;
                case 0x7A: Opcode7A(pc); break;

                //OP__(64)	OP__(9C)	OP__(74)	OP__(9E)	/* STZ */
                case 0x64: Opcode64(pc); break;
                case 0x9C: Opcode9C(pc); break;
                case 0x74: Opcode74(pc); break;
                case 0x9E: Opcode9E(pc); break;

                //OP__(7C)	/* 7C - JMP - Absolute,X */
                case 0x7C: Opcode7C(pc); break;

                //#endif
                //#if 0 && BUILD_M65C02
                //OP__(CB)	/* WAI */
                //OP__(DB)	/* STP */
                //#endif
                //#if BUILD_HUC6280
                //OP__(0F)	OP__(1F)	OP__(2F)	OP__(3F)	/* BBRi */
                case 0x0F: Opcode0F(pc); break;
                case 0x1F: Opcode1F(pc); break;
                case 0x2F: Opcode2F(pc); break;
                case 0x3F: Opcode3F(pc); break;

                //OP__(4F)	OP__(5F)	OP__(6F)	OP__(7F)
                case 0x4F: Opcode4F(pc); break;
                case 0x5F: Opcode5F(pc); break;
                case 0x6F: Opcode6F(pc); break;
                case 0x7F: Opcode7F(pc); break;

                //OP__(8F)	OP__(9F)	OP__(AF)	OP__(BF)	/* BBSi*/
                case 0x8F: Opcode8F(pc); break;
                case 0x9F: Opcode9F(pc); break;
                case 0xAF: OpcodeAF(pc); break;
                case 0xBF: OpcodeBF(pc); break;

                //OP__(CF)	OP__(DF)	OP__(EF)	OP__(FF)
                case 0xCF: OpcodeCF(pc); break;
                case 0xDF: OpcodeDF(pc); break;
                case 0xEF: OpcodeEF(pc); break;
                case 0xFF: OpcodeFF(pc); break;

                //OP__(44)	/* 44 - BSR */
                case 0x44: Opcode44(pc); break;

                //OP__(62)	OP__(82)	OP__(C2)	/* CLA CLX CLY */
                case 0x62: Opcode62(pc); break;
                case 0x82: Opcode82(pc); break;
                case 0xC2: OpcodeC2(pc); break;

                //OP__(07)	OP__(17)	OP__(27)	OP__(37)	/* RMBi */
                case 0x07: Opcode07(pc); break;
                case 0x17: Opcode17(pc); break;
                case 0x27: Opcode27(pc); break;
                case 0x37: Opcode37(pc); break;

                //OP__(47)	OP__(57)	OP__(67)	OP__(77)
                case 0x47: Opcode47(pc); break;
                case 0x57: Opcode57(pc); break;
                case 0x67: Opcode67(pc); break;
                case 0x77: Opcode77(pc); break;

                //OP__(87)	OP__(97)	OP__(A7)	OP__(B7)	/* SMBi */
                case 0x87: Opcode87(pc); break;
                case 0x97: Opcode97(pc); break;
                case 0xA7: OpcodeA7(pc); break;
                case 0xB7: OpcodeB7(pc); break;

                //OP__(C7)	OP__(D7)	OP__(E7)	OP__(F7)
                case 0xC7: OpcodeC7(pc); break;
                case 0xD7: OpcodeD7(pc); break;
                case 0xE7: OpcodeE7(pc); break;
                case 0xF7: OpcodeF7(pc); break;

                //OP__(02)	OP__(22)	OP__(42)	/* SXY SAX SAY */
                case 0x02: Opcode02(pc); break;
                case 0x22: Opcode22(pc); break;
                case 0x42: Opcode42(pc); break;

                //OP__(F4)	/* F4 - SET */
                case 0xF4: OpcodeF4(pc); break;

                //OP__(03)	OP__(13)	OP__(23)	/* ST0 ST1 ST2 */
                case 0x03: Opcode03(pc); break;
                case 0x13: Opcode13(pc); break;
                case 0x23: Opcode23(pc); break;

                //OP__(43)	OP__(53)	/* TMAi TAMi */
                case 0x43: Opcode43(pc); break;
                case 0x53: Opcode53(pc); break;

                //OP__(83)	OP__(93)	OP__(A3)	OP__(B3)	/* TST */
                case 0x83: Opcode83(pc); break;
                case 0x93: Opcode93(pc); break;
                case 0xA3: OpcodeA3(pc); break;
                case 0xB3: OpcodeB3(pc); break;

                //OP__(73)	OP__(C3)	OP__(D3)	OP__(E3)	OP__(F3)	/* block */
                case 0x73: Opcode73(pc); break;
                case 0xC3: OpcodeC3(pc); break;
                case 0xD3: OpcodeD3(pc); break;
                case 0xE3: OpcodeE3(pc); break;
                case 0xF3: OpcodeF3(pc); break;

                //OP__(54)	OP__(D4)	/* CSL CSH */
                case 0x54: Opcode54(pc); break;
                case 0xD4: OpcodeD4(pc); break;
                    //#endif
            }
        }
        //-------------------------------- km6502ot.h END

        //# include "km6502ex.h"
        //-------------------------------- km6502ex.h START
        public void K_EXEC(K6280_Context pc)
        {
            if (pc.iRequest != 0)
            {
                if ((pc.iRequest & IRQ_INIT) != 0)
                {
                    //#if BUILD_HUC6280
                    pc.lowClockMode = 1;
                    //#endif
                    pc.A = 0;
                    pc.X = 0;
                    pc.Y = 0;
                    pc.S = 0xFF;
                    pc.P = Z_FLAG | R_FLAG | I_FLAG;
                    pc.iRequest = 0;
                    pc.iMask = 0xffffffff;// ~0;
                    KI_ADDCLOCK(pc, 7);
                    return;
                }
                else if ((pc.iRequest & IRQ_RESET) != 0)
                {
                    //#if BUILD_HUC6280
                    pc.lowClockMode = 1;
                    K_WRITEMPR(pc, 0x80, 0x00); /* IPL(TOP OF ROM) */
                                                //#endif
                    pc.A = 0;
                    pc.X = 0;
                    pc.Y = 0;
                    pc.S = 0xFF;
                    pc.P = Z_FLAG | R_FLAG | I_FLAG;
                    pc.PC = KI_READWORD(pc, VEC_RESET);
                    pc.iRequest = 0;
                    pc.iMask = 0xffffffff;// ~0;
                }
                else if ((pc.iRequest & IRQ_NMI) != 0)
                {
                    KM_PUSH(pc, (pc.PC >> 8) & 0xff);
                    KM_PUSH(pc, (pc.PC) & 0xff);
                    KM_PUSH(pc, (UInt32)(pc.P | R_FLAG | B_FLAG));
                    //#if BUILD_M65C02 || BUILD_HUC6280
                    pc.P = (UInt32)((pc.P & ~(D_FLAG | T_FLAG)) | I_FLAG);
                    pc.iRequest &= ~(UInt32)IRQ_NMI;
                    //#else
                    //__THIS__.P = (__THIS__.P & ~T_FLAG) | I_FLAG;   /* 6502 bug */
                    //__THIS__.iRequest &= ~(IRQ_NMI | IRQ_BRK);
                    //#endif
                    pc.PC = KI_READWORD(pc, VEC_NMI);
                    KI_ADDCLOCK(pc, 7);
                }
                else if ((pc.iRequest & IRQ_BRK) != 0)
                {
                    KM_PUSH(pc, (pc.PC >> 8) & 0xff);
                    KM_PUSH(pc, (pc.PC) & 0xff);
                    KM_PUSH(pc, (UInt32)(pc.P | R_FLAG | B_FLAG));
                    //#if BUILD_M65C02 || BUILD_HUC6280
                    pc.P = (UInt32)((pc.P & ~(D_FLAG | T_FLAG)) | I_FLAG);
                    //#else
                    //__THIS__.P = (__THIS__.P & ~T_FLAG) | I_FLAG;   /* 6502 bug */
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
                else if ((pc.iMask & pc.iRequest & IRQ_INT1) != 0)
                {
                    KM_PUSH(pc, (pc.PC >> 8) & 0xff);
                    KM_PUSH(pc, (pc.PC) & 0xff);
                    KM_PUSH(pc, (UInt32)(pc.P | R_FLAG | B_FLAG));
                    pc.P = (UInt32)((pc.P & ~(D_FLAG | T_FLAG)) | I_FLAG);
                    pc.PC = KI_READWORD(pc, VEC_INT1);
                    KI_ADDCLOCK(pc, 7);
                }
                else if ((pc.iMask & pc.iRequest & IRQ_TIMER) != 0)
                {
                    KM_PUSH(pc, (pc.PC >> 8) & 0xff);
                    KM_PUSH(pc, (pc.PC) & 0xff);
                    KM_PUSH(pc, (UInt32)(pc.P | R_FLAG | B_FLAG));
                    pc.P = (UInt32)((pc.P & ~(D_FLAG | T_FLAG)) | I_FLAG);
                    pc.PC = KI_READWORD(pc, VEC_TIMER);
                    KI_ADDCLOCK(pc, 7);
                }
                else if ((pc.iMask & pc.iRequest & IRQ_INT) != 0)
                {
                    KM_PUSH(pc, (pc.PC >> 8) & 0xff);
                    KM_PUSH(pc, (pc.PC) & 0xff);
                    KM_PUSH(pc, (UInt32)((pc.P | R_FLAG) & ~B_FLAG));
                    pc.P = (UInt32)((pc.P & ~(D_FLAG | T_FLAG)) | I_FLAG);
                    pc.PC = KI_READWORD(pc, VEC_INT);
                    KI_ADDCLOCK(pc, 7);
                }
                //#else
                //                else if (__THIS__.iMask & __THIS__.iRequest & IRQ_INT)
                //                {
                //                    KM_PUSH(__THISP_ RTO8(__THIS__.PC >> 8));
                //                    KM_PUSH(__THISP_ RTO8(__THIS__.PC));
                //                    KM_PUSH(__THISP_(Uword)((__THIS__.P | R_FLAG) & ~B_FLAG));
                //#if BUILD_M65C02
                //			__THIS__.P = (__THIS__.P & ~(D_FLAG | T_FLAG)) | I_FLAG;
                //#else
                //                    __THIS__.P = (__THIS__.P & ~T_FLAG) | I_FLAG;   /* 6502 bug */
                //#endif
                //                    __THIS__.iRequest &= ~IRQ_INT;
                //                    __THIS__.PC = KI_READWORD(__THISP_ VEC_INT);
                //                    KI_ADDCLOCK(__THISP_ 7);
                //                }
                //#endif
            }
            K_OPEXEC(pc);
        }
        //-------------------------------- km6502ex.h END

    }
}