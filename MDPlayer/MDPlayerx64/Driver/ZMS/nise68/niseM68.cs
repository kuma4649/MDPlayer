using System.Diagnostics;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class niseM68
    {
        private Memory68 mem;
        private Register68 reg;
        public niseHuman hmn;
        private Func<UInt16, int>[] cmdTbl = null;

        public niseM68(Memory68 mem, Register68 reg)
        {
            this.mem = mem;
            this.reg = reg;
            cmdTbl = new Func<UInt16, int>[]
            {
                //00
                Cori     ,Cbsetbtst,Candi    ,Cbsetbtst , Csubi    ,Cbsetbtst,Caddi     ,Cbsetbtst,
                Cbtst08  ,Cbsetbtst,Ceori    ,Cbsetbtst , Ccmpi    ,Cbsetbtst,null      ,Cbsetbtst,
                //10
                Cmove    ,Cmove    ,Cmove    ,Cmove     , Cmove    ,Cmove    ,Cmove     ,Cmove,
                Cmove    ,Cmove    ,Cmove    ,Cmove     , Cmove    ,Cmove    ,Cmove     ,Cmove,
                //20
                Cmove    ,Cmove    ,Cmove    ,Cmove     , Cmove    ,Cmove    ,Cmove     ,Cmove,
                Cmove    ,Cmove    ,Cmove    ,Cmove     , Cmove    ,Cmove    ,Cmove     ,Cmove,
                //30
                Cmove    ,Cmove    ,Cmove    ,Cmove     , Cmove    ,Cmove    ,Cmove     ,Cmove,
                Cmove    ,Cmove    ,Cmove    ,Cmove     , Cmove    ,Cmove    ,Cmove     ,Cmove,
                //40
                Cmove    ,Clea     ,Cclr     ,Clea      , Cmoveccr ,Clea     ,CmoveToSR ,Clea,
                Cpea     ,Clea     ,Ctst     ,Clea      , Cmovem   ,Clea     ,Crts      ,Clea,
                //50
                Caqsqdbs ,Caqsqdbs ,Caqsqdbs ,Caqsqdbs  , Caqsqdbs ,Caqsqdbs ,Caqsqdbs  ,Caqsqdbs,
                Caqsqdbs ,Caqsqdbs ,Caqsqdbs ,Caqsqdbs  , Caqsqdbs ,Caqsqdbs ,Caqsqdbs  ,Caqsqdbs,
                //60
                Cbra     ,Cbsr     ,Cbra     ,Cbra      , Cbra     ,Cbra     ,Cbra      ,Cbra,
                Cbra     ,Cbra     ,Cbra     ,Cbra      , Cbra     ,Cbra     ,Cbra      ,Cbra,
                //70
                Cmoveq   ,null     ,Cmoveq   ,null      , Cmoveq   ,null     ,Cmoveq    ,null,
                Cmoveq   ,null     ,Cmoveq   ,null      , Cmoveq   ,null     ,Cmoveq    ,null,
                //80
                Cor      ,Cor      ,Cor      ,Cor       , Cor      ,Cor      ,Cor       ,Cor,
                Cor      ,Cor      ,Cor      ,Cor       , Cor      ,Cor      ,Cor       ,Cor,
                //90
                Csub     ,Csub     ,Csub     ,Csub      , Csub     ,Csub     ,Csub      ,Csub,
                Csub     ,Csub     ,Csub     ,Csub      , Csub     ,Csub     ,Csub      ,Csub,
                //a0
                null     ,null     ,null     ,null      , null     ,null     ,null      ,null,
                null     ,null     ,null     ,null      , null     ,null     ,null      ,null,
                //b0
                Ccmp     ,Ceor     ,Ccmp     ,Ceor      , Ccmp     ,Ceor     ,Ccmp      ,Ceor,
                Ccmp     ,Ceor     ,Ccmp     ,Ceor      , Ccmp     ,Ceor     ,Ccmp      ,Ceor,
                //c0
                Cmulu    ,Cand     ,Cmulu    ,Cand      , Cmulu    ,Cand     ,Cmulu     ,Cand,
                Cmulu    ,Cand     ,Cmulu    ,Cand      , Cmulu    ,Cand     ,Cmulu     ,Cand,
                //d0
                Cadd     ,Cadd     ,Cadd     ,Cadd      , Cadd     ,Cadd     ,Cadd      ,Cadd,
                Cadd     ,Cadd     ,Cadd     ,Cadd      , Cadd     ,Cadd     ,Cadd      ,Cadd,
                //e0
                Cshift   ,Cshift   ,Cshift   ,Cshift    , Cshift   ,Cshift   ,Cshift    ,Cshift,
                Cshift   ,Cshift   ,Cshift   ,Cshift    , Cshift   ,Cshift   ,Cshift    ,Cshift,
                //f0
                null     ,null     ,null     ,null      , null     ,null     ,null      ,null,
                null     ,null     ,null     ,null      , null     ,null     ,CFEFunc   ,Cdos,
            };
        }

        public int StepExecute()
        {
            UInt16 n = FetchW();
            int cycle = 0;

            if (cmdTbl[n >> 8] != null)
            {
                cycle += cmdTbl[n >> 8](n);
            }
            else
            {
                throw new NotImplementedException(string.Format("未実装!! [{0:x04}]", n));
            }

            return cycle;
        }

        private byte FetchB()
        {
            byte n = mem.PeekB(reg.PC);
            reg.PC++;
            return n;
        }

        private UInt16 FetchW()
        {
            UInt16 n = mem.PeekW(reg.PC);
            reg.PC += 2;
            return n;
        }

        private UInt32 FetchL()
        {
            UInt32 n = mem.PeekL(reg.PC);
            reg.PC += 4;
            return n;
        }

        private int Cori(ushort n)
        {
            if (n == 0x007c)
            {
                return CoriToSr(n);
            }

            int size = (n & 0x00c0) >> 6;

            switch (size)
            {
                case 0://byte
                    return Corib(n);
                case 1://word
                    return Coriw(n);
                case 2://long
                    return Coril(n);
            }
            throw new NotImplementedException();
        }

        private int Corib(ushort n)
        {
#if DEBUG_M68
            string nimo = "ORI.b ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            byte val = (byte)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x02},", val);
#endif


            ushort after = 0;
            ushort before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = reg.GetDb(r);
                    after = (ushort)(val | before);
                    reg.SetDb(r, (byte)after);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Ori_b[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val | before);
                    mem.PokeB(reg.A[r], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Ori_b[1];
                    break;
                case 3://(An)+
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val | before);
                    mem.PokeB(reg.A[r], (byte)after);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Ori_b[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val | before);
                    mem.PokeB(reg.A[r], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Ori_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekB((UInt32)(reg.A[r] + d16));
                    after = (ushort)(val | before);
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Ori_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekB(ptr);
                    after = (ushort)(val | before);
                    mem.PokeB(ptr, (byte)after);
                    cycle = cy.Ori_b[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = (ushort)(val | before);
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Ori_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = (ushort)(val | before);
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Ori_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x80) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Coriw(ushort n)
        {
#if DEBUG_M68
            string nimo = "ORI.w ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            ushort val = FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x04},", val);
#endif


            ushort after = 0;
            ushort before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = reg.GetDw(r);
                    after = (ushort)(val | before);
                    reg.SetDw(r, after);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Ori_w[0];
                    break;
                case 2://(An)
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val | before);
                    mem.PokeW(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Ori_w[1];
                    break;
                case 3://(An)+
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val | before);
                    mem.PokeW(reg.A[r], after);
                    reg.A[r] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Ori_w[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 2;
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val | before);
                    mem.PokeW(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Ori_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekW((UInt32)(reg.A[r] + d16));
                    after = (ushort)(val | before);
                    mem.PokeW((UInt32)(reg.A[r] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Ori_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekW(ptr);
                    after = (ushort)(val | before);
                    mem.PokeW(ptr, after);
                    cycle = cy.Ori_w[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = (ushort)(val | before);
                            mem.PokeW(ptr, after);
                            cycle = cy.Ori_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = (ushort)(val | before);
                            mem.PokeW(ptr, after);
                            cycle = cy.Ori_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Coril(ushort n)
        {
#if DEBUG_M68
            string nimo = "ORI.l ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            uint val = FetchL();
#if DEBUG_M68
            nimo += string.Format("#${0:x08},", val);
#endif


            uint after = 0;
            uint before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = reg.D[r];
                    after = val | before;
                    reg.D[r] = after;
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Ori_l[0];
                    break;
                case 2://(An)
                    before = mem.PeekL(reg.A[r]);
                    after = val | before;
                    mem.PokeL(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Ori_l[1];
                    break;
                case 3://(An)+
                    before = mem.PeekL(reg.A[r]);
                    after = val | before;
                    mem.PokeL(reg.A[r], after);
                    reg.A[r] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Ori_l[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 4;
                    before = mem.PeekL(reg.A[r]);
                    after = val | before;
                    mem.PokeL(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Ori_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekL((UInt32)(reg.A[r] + d16));
                    after = val | before;
                    mem.PokeL((UInt32)(reg.A[r] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Ori_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekL(ptr);
                    after = val | before;
                    mem.PokeL(ptr, after);
                    cycle = cy.Ori_l[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val | before;
                            mem.PokeL(ptr, after);
                            cycle = cy.Ori_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val | before;
                            mem.PokeL(ptr, after);
                            cycle = cy.Ori_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000_0000) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CoriToSr(ushort n)
        {
#if DEBUG_M68
            string nimo = "ORI.w ";
#endif

            ushort val = FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x04},sr", val);
#endif


            reg.SR |= (ushort)(val & 0b1010_0111_0001_1111);

            //if ((val & 0b1000_0000_0000_0000) != 0) reg.T = true;
            //if ((val & 0b0010_0000_0000_0000) != 0) reg.S = true;
            //if ((val & 0b0000_0100_0000_0000) != 0) reg.SR|= 0b0000_0100_0000_0000;//I2フラグ
            //if ((val & 0b0000_0010_0000_0000) != 0) reg.SR|= 0b0000_0010_0000_0000;//I1フラグ
            //if ((val & 0b0000_0001_0000_0000) != 0) reg.SR|= 0b0000_0001_0000_0000;//I0フラグ
            //if ((val & 0b0000_0000_0001_0000) != 0) reg.X = true;
            //if ((val & 0b0000_0000_0000_1000) != 0) reg.N = true;
            //if ((val & 0b0000_0000_0000_0100) != 0) reg.Z = true;
            //if ((val & 0b0000_0000_0000_0010) != 0) reg.V = true;
            //if ((val & 0b0000_0000_0000_0001) != 0) reg.C = true;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return 20;
        }

        private int Candi(ushort n)
        {
            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0://byte
                    return Candib(n);
                case 1://word
                    return Candiw(n);
                case 2://long
                    return Candil(n);
            }
            throw new NotImplementedException();
        }

        private int Candib(ushort n)
        {
#if DEBUG_M68
            string nimo = "ANDI.b ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            byte val = (byte)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x02},", val);
#endif


            ushort after = 0;
            ushort before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = reg.GetDb(r);
                    after = (ushort)(val & before);
                    reg.SetDb(r, (byte)after);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Andi_b[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeB(reg.A[r], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Andi_b[1];
                    break;
                case 3://(An)+
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeB(reg.A[r], (byte)after);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Andi_b[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeB(reg.A[r], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Andi_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekB((UInt32)(reg.A[r] + d16));
                    after = (ushort)(val & before);
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Andi_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekB(ptr);
                    after = (ushort)(val & before);
                    mem.PokeB(ptr, (byte)after);
                    cycle = cy.Andi_b[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = (ushort)(val & before);
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Andi_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = (ushort)(val & before);
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Andi_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x80) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Candiw(ushort n)
        {
#if DEBUG_M68
            string nimo = "ANDI.w ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            ushort val = FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x04},", val);
#endif


            ushort after = 0;
            ushort before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = reg.GetDw(r);
                    after = (ushort)(val & before);
                    reg.SetDw(r, after);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Andi_w[0];
                    break;
                case 2://(An)
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeW(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Andi_w[1];
                    break;
                case 3://(An)+
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeW(reg.A[r], after);
                    reg.A[r] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Andi_w[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 2;
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeW(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Andi_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekW((UInt32)(reg.A[r] + d16));
                    after = (ushort)(val & before);
                    mem.PokeW((UInt32)(reg.A[r] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Andi_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekW(ptr);
                    after = (ushort)(val & before);
                    mem.PokeW(ptr, after);
                    cycle = cy.Andi_w[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = (ushort)(val & before);
                            mem.PokeW(ptr, after);
                            cycle = cy.Andi_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = (ushort)(val & before);
                            mem.PokeW(ptr, after);
                            cycle = cy.Andi_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Candil(ushort n)
        {
#if DEBUG_M68
            string nimo = "ANDI.l ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            uint val = FetchL();
#if DEBUG_M68
            nimo += string.Format("#${0:x08},", val);
#endif


            uint after = 0;
            uint before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = reg.D[r];
                    after = val & before;
                    reg.D[r] = after;
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Andi_l[0];
                    break;
                case 2://(An)
                    before = mem.PeekL(reg.A[r]);
                    after = val & before;
                    mem.PokeL(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Andi_l[1];
                    break;
                case 3://(An)+
                    before = mem.PeekL(reg.A[r]);
                    after = val & before;
                    mem.PokeL(reg.A[r], after);
                    reg.A[r] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Andi_l[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 4;
                    before = mem.PeekL(reg.A[r]);
                    after = val & before;
                    mem.PokeL(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Andi_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekL((UInt32)(reg.A[r] + d16));
                    after = val & before;
                    mem.PokeL((UInt32)(reg.A[r] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Andi_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekL(ptr);
                    after = val & before;
                    mem.PokeL(ptr, after);
                    cycle = cy.Andi_l[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val & before;
                            mem.PokeL(ptr, after);
                            cycle = cy.Andi_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val & before;
                            mem.PokeL(ptr, after);
                            cycle = cy.Andi_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000_0000) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cand(ushort n)
        {
            if ((n & 0xf1f8) == 0xc140 || (n & 0xf1f8) == 0xc148 || (n & 0xf1f8) == 0xc188)
            {
                return Cexg(n);
            }
            if ((n & 0xf1c0) == 0xc1c0)
            {
                return Cmuls(n);
            }

            int size = (n & 0x01c0) >> 6;
            switch (size)
            {
                case 0://byte
                    return CandbEADn(n);
                case 1://word
                    return CandwEADn(n);
                case 2://long
                    return CandlEADn(n);
                case 4://byte
                    return CandbDnEA(n);
                case 5://word
                    return CandwDnEA(n);
                case 6://long
                    return CandlDnEA(n);
            }
            throw new NotImplementedException();
        }

        private int CandbDnEA(ushort n)
        {
#if DEBUG_M68
            string nimo = "AND.b ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            byte val = reg.GetDb(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            ushort after = 0;
            ushort before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeB(reg.A[r], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.And_bDnEA[0];
                    break;
                case 3://(An)+
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeB(reg.A[r], (byte)after);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.And_bDnEA[1];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    before = mem.PeekB(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeB(reg.A[r], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.And_bDnEA[2];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekB((UInt32)(reg.A[r] + d16));
                    after = (ushort)(val & before);
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.And_bDnEA[3];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekB(ptr);
                    after = (ushort)(val & before);
                    mem.PokeB(ptr, (byte)after);
                    cycle = cy.And_bDnEA[4];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = (ushort)(val & before);
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.And_bDnEA[5];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = (ushort)(val & before);
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.And_bDnEA[6];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x80) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CandwDnEA(ushort n)
        {
#if DEBUG_M68
            string nimo = "AND.w ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            ushort val = reg.GetDw(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            ushort after = 0;
            ushort before;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeW(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.And_wDnEA[0];
                    break;
                case 3://(An)+
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeW(reg.A[r], after);
                    reg.A[r] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.And_wDnEA[1];
                    break;
                case 4://-(An)
                    reg.A[r] -= 2;
                    before = mem.PeekW(reg.A[r]);
                    after = (ushort)(val & before);
                    mem.PokeW(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.And_wDnEA[2];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekW((UInt32)(reg.A[r] + d16));
                    after = (ushort)(val & before);
                    mem.PokeW((UInt32)(reg.A[r] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.And_wDnEA[3];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekW(ptr);
                    after = (ushort)(val & before);
                    mem.PokeW(ptr, after);
                    cycle = cy.And_wDnEA[4];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = (ushort)(val & before);
                            mem.PokeW(ptr, after);
                            cycle = cy.And_wDnEA[5];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = (ushort)(val & before);
                            mem.PokeW(ptr, after);
                            cycle = cy.And_wDnEA[6];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CandlDnEA(ushort n)
        {
#if DEBUG_M68
            string nimo = "AND.l ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            uint val = reg.GetDl(sr);
#if DEBUG_M68
            nimo += string.Format("D{0:d},", sr);
#endif


            uint after = 0;
            uint before;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    before = mem.PeekL(reg.A[r]);
                    after = val & before;
                    mem.PokeL(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.And_lDnEA[0];
                    break;
                case 3://(An)+
                    before = mem.PeekL(reg.A[r]);
                    after = val & before;
                    mem.PokeL(reg.A[r], after);
                    reg.A[r] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.And_lDnEA[1];
                    break;
                case 4://-(An)
                    reg.A[r] -= 4;
                    before = mem.PeekL(reg.A[r]);
                    after = val & before;
                    mem.PokeL(reg.A[r], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.And_lDnEA[2];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekL((UInt32)(reg.A[r] + d16));
                    after = val & before;
                    mem.PokeL((UInt32)(reg.A[r] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.And_lDnEA[3];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekL(ptr);
                    after = val & before;
                    mem.PokeL(ptr, after);
                    cycle = cy.And_lDnEA[4];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val & before;
                            mem.PokeL(ptr, after);
                            cycle = cy.And_lDnEA[5];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val & before;
                            mem.PokeL(ptr, after);
                            cycle = cy.And_lDnEA[6];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000_0000) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CandbEADn(ushort n)
        {
#if DEBUG_M68
            string nimo = "AND.b ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            byte src = 0;
            byte dst = reg.GetDb(sr);
            byte after = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    src = reg.GetDb(r);
                    after = (byte)(src & dst);
                    reg.SetDb(sr, after);
#if DEBUG_M68
                    nimo += string.Format("D{0},D{1}", r, sr);
#endif

                    cycle = cy.And_bEADn[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    src = mem.PeekB(reg.A[r]);
                    after = (byte)(src & dst);
                    reg.SetDb(sr, after);
#if DEBUG_M68
                    nimo += string.Format("(A{0}),D{1}", r, sr);
#endif

                    cycle = cy.And_bEADn[1];
                    break;
                case 3://(An)+
                    src = mem.PeekB(reg.A[r]);
                    after = (byte)(src & dst);
                    reg.SetDb(sr, after);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+,D{1}", r, sr);
#endif

                    cycle = cy.And_bEADn[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    src = mem.PeekB(reg.A[r]);
                    after = (byte)(src & dst);
                    reg.SetDb(sr, after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0}),D{1}", r, sr);
#endif

                    cycle = cy.And_bEADn[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = mem.PeekB((UInt32)(reg.A[r] + d16));
                    after = (byte)(src & dst);
                    reg.SetDb(sr, after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1}),D{2}", d16, r, sr);
#endif

                    cycle = cy.And_bEADn[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4}),D{5}", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w", sr);
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    src = mem.PeekB(ptr);
                    after = (byte)(src & dst);
                    reg.SetDb(sr, after);
                    cycle = cy.And_bEADn[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04},D{1}", (Int16)ptr, sr);
#endif

                            src = mem.PeekB(ptr);
                            after = (byte)(src & dst);
                            reg.SetDb(sr, after);
                            cycle = cy.And_bEADn[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08},D{1}", (Int32)ptr, sr);
#endif

                            src = mem.PeekB(ptr);
                            after = (byte)(src & dst);
                            reg.SetDb(sr, after);
                            cycle = cy.And_bEADn[7];
                            break;
                        case 2://d16(PC)
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC),D{1}", (UInt16)ptr, sr);
#endif

                            src = mem.PeekB(ptr + reg.PC - 2);
                            after = (byte)(src & dst);
                            reg.SetDb(sr, after);
                            cycle = cy.And_bEADn[8];
                            break;
                        case 3://d8(PC,IX)
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3}),D{4}", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w", sr);
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            src = mem.PeekB(ptr);
                            after = (byte)(src & dst);
                            reg.SetDb(sr, after);
                            cycle = cy.And_bEADn[9];
                            break;
                        case 4://#Imm
                            src = (byte)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x02},D{1}", (Int32)src, sr);
#endif

                            after = (byte)(src & dst);
                            reg.SetDb(sr, after);
                            cycle = cy.And_bEADn[10];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x80) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CandwEADn(ushort n)
        {
#if DEBUG_M68
            string nimo = "AND.w ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            ushort src = 0;
            ushort dst = reg.GetDw(sr);
            ushort after = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    src = reg.GetDw(r);
                    after = (ushort)(src & dst);
                    reg.SetDw(sr, after);
#if DEBUG_M68
                    nimo += string.Format("D{0},D{1}", r, sr);
#endif

                    cycle = cy.And_wEADn[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    src = mem.PeekW(reg.A[r]);
                    after = (ushort)(src & dst);
                    reg.SetDw(sr, after);
#if DEBUG_M68
                    nimo += string.Format("(A{0}),D{1}", r, sr);
#endif

                    cycle = cy.And_wEADn[1];
                    break;
                case 3://(An)+
                    src = mem.PeekW(reg.A[r]);
                    after = (ushort)(src & dst);
                    reg.SetDw(sr, after);
                    reg.A[r] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+,D{1}", r, sr);
#endif

                    cycle = cy.And_wEADn[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 2;
                    src = mem.PeekW(reg.A[r]);
                    after = (ushort)(src & dst);
                    reg.SetDw(sr, after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0}),D{1}", r, sr);
#endif

                    cycle = cy.And_wEADn[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = mem.PeekW((UInt32)(reg.A[r] + d16));
                    after = (ushort)(src & dst);
                    reg.SetDw(sr, after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1}),D{2}", d16, r, sr);
#endif

                    cycle = cy.And_wEADn[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4}),D{5}", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w", sr);
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    src = mem.PeekW(ptr);
                    after = (ushort)(src & dst);
                    reg.SetDw(sr, after);
                    cycle = cy.And_wEADn[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04},D{1}", (Int16)ptr, sr);
#endif

                            src = mem.PeekW(ptr);
                            after = (ushort)(src & dst);
                            reg.SetDw(sr, after);
                            cycle = cy.And_wEADn[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08},D{1}", (Int32)ptr, sr);
#endif

                            src = mem.PeekW(ptr);
                            after = (ushort)(src & dst);
                            reg.SetDw(sr, after);
                            cycle = cy.And_wEADn[7];
                            break;
                        case 2://d16(PC)
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC),D{1}", (UInt16)ptr, sr);
#endif

                            src = mem.PeekW(ptr + reg.PC - 2);
                            after = (ushort)(src & dst);
                            reg.SetDw(sr, after);
                            cycle = cy.And_wEADn[8];
                            break;
                        case 3://d8(PC,IX)
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3}),D{4}", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w", sr);
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            src = mem.PeekW(ptr);
                            after = (ushort)(src & dst);
                            reg.SetDw(sr, after);
                            cycle = cy.And_wEADn[9];
                            break;
                        case 4://#Imm
                            src = FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x02},D{1}", (Int32)src, sr);
#endif

                            after = (ushort)(src & dst);
                            reg.SetDw(sr, after);
                            cycle = cy.And_wEADn[10];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            //reg.N = (after & 0x80) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CandlEADn(ushort n)
        {
#if DEBUG_M68
            string nimo = "AND.l ";
#endif

            int cycle = 0;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            UInt32 src = 0;
            UInt32 dst = reg.GetDl(sr);
            UInt32 after = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    src = reg.GetDl(r);
                    after = (src & dst);
                    reg.SetDl(sr, after);
#if DEBUG_M68
                    nimo += string.Format("D{0},D{1}", r, sr);
#endif

                    cycle = cy.And_lEADn[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    src = mem.PeekL(reg.A[r]);
                    after = (src & dst);
                    reg.SetDl(sr, after);
#if DEBUG_M68
                    nimo += string.Format("(A{0}),D{1}", r, sr);
#endif

                    cycle = cy.And_lEADn[1];
                    break;
                case 3://(An)+
                    src = mem.PeekL(reg.A[r]);
                    after = (src & dst);
                    reg.SetDl(sr, after);
                    reg.A[r] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+,D{1}", r, sr);
#endif

                    cycle = cy.And_lEADn[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 4;
                    src = mem.PeekL(reg.A[r]);
                    after = (src & dst);
                    reg.SetDl(sr, after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0}),D{1}", r, sr);
#endif

                    cycle = cy.And_lEADn[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = mem.PeekL((UInt32)(reg.A[r] + d16));
                    after = (src & dst);
                    reg.SetDl(sr, after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1}),D{2}", d16, r, sr);
#endif

                    cycle = cy.And_lEADn[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4}),D{5}", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w", sr);
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    src = mem.PeekL(ptr);
                    after = (src & dst);
                    reg.SetDl(sr, after);
                    cycle = cy.And_lEADn[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04},D{1}", (Int16)ptr, sr);
#endif

                            src = mem.PeekL(ptr);
                            after = (src & dst);
                            reg.SetDl(sr, after);
                            cycle = cy.And_lEADn[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08},D{1}", (Int32)ptr, sr);
#endif

                            src = mem.PeekL(ptr);
                            after = (src & dst);
                            reg.SetDl(sr, after);
                            cycle = cy.And_lEADn[7];
                            break;
                        case 2://d16(PC)
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC),D{1}", (UInt16)ptr, sr);
#endif

                            src = mem.PeekL(ptr + reg.PC - 2);
                            after = (src & dst);
                            reg.SetDl(sr, after);
                            cycle = cy.And_lEADn[8];
                            break;
                        case 3://d8(PC,IX)
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3}),D{4}", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w", sr);
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            src = mem.PeekL(ptr);
                            after = (src & dst);
                            reg.SetDl(sr, after);
                            cycle = cy.And_lEADn[9];
                            break;
                        case 4://#Imm
                            src = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x02},D{1}", (Int32)src, sr);
#endif

                            after = (src & dst);
                            reg.SetDl(sr, after);
                            cycle = cy.And_lEADn[10];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000_0000) != 0;
            //reg.N = (after & 0x80) != 0;
            reg.Z = (after == 0);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cexg(ushort n)
        {
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007) >> 0;
            int opm = (n & 0x00f8) >> 3;
            opm = (opm == 0b0_1000) ? 0 : (opm == 0b0_1001 ? 1 : 2);

            UInt32 src;
            UInt32 dst;

            switch (opm)
            {
                case 0://EXG Dn,Dn
                    src = reg.GetDl(sr);
                    dst = reg.GetDl(dr);
                    reg.SetDl(sr, dst);
                    reg.SetDl(dr, src);
                    break;
                case 1://EXG An,An
                    src = reg.GetAl(sr);
                    dst = reg.GetAl(dr);
                    reg.SetAl(sr, dst);
                    reg.SetAl(dr, src);
                    break;
                case 2://EXG Dn,An
                    src = reg.GetDl(sr);
                    dst = reg.GetAl(dr);
                    reg.SetDl(sr, dst);
                    reg.SetAl(dr, src);
                    break;
            }

            return 6;
        }

        private int Cor(ushort n)
        {
            //0x8ffc
            //if ((n & 0x0100) == 0) return Ccmp(n);
            //if ((n & 0xf138) == 0xb108)
            //return Ccmp(n);
            if ((n & 0xf1c0) == 0x81c0) return Cdivs(n);
            if ((n & 0xf1c0) == 0x80c0) return Cdivu(n);

            int size = (n & 0x00c0) >> 6;
            bool isA = (n & 0x0100) == 0;
            if (!isA)
            {
                switch (size)
                {
                    case 0://byte
                        return CorDnEab(n);
                    case 1://word
                        return CorDnEaw(n);
                    case 2://long
                        return CorDnEal(n);
                }
            }
            else
            {
                switch (size)
                {
                    case 0://byte
                        return CorEaDnb(n);
                    case 1://word
                        return CorEaDnw(n);
                    case 2://long
                        return CorEaDnl(n);
                }
            }

            throw new NotImplementedException();
        }

        private int CorDnEab(ushort n)
        {
#if DEBUG_M68
            string nimo = "OR.b ";
#endif

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            uint val = reg.GetDb(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            uint after = 0;
            uint before;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 2://(An)
                    before = mem.PeekB(reg.A[dr]);
                    after = val | before;
                    mem.PokeB(reg.A[dr], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Or_b[0];
                    break;
                case 3://(An)+
                    before = mem.PeekB(reg.A[dr]);
                    after = val | before;
                    mem.PokeB(reg.A[dr], (byte)after);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Or_b[1];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    before = mem.PeekB(reg.A[dr]);
                    after = val | before;
                    mem.PokeB(reg.A[dr], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Or_b[2];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekB((UInt32)(reg.A[dr] + d16));
                    after = val | before;
                    mem.PokeB((UInt32)(reg.A[dr] + d16), (byte)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Or_b[3];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekB(ptr);
                    after = val | before;
                    mem.PokeB(ptr, (byte)after);
                    cycle = cy.Or_b[4];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = val | before;
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Or_b[5];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = val | before;
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Or_b[6];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x80) != 0;
            reg.SetZ((byte)after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CorDnEaw(ushort n)
        {
#if DEBUG_M68
            string nimo = "OR.w ";
#endif

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            uint val = reg.GetDw(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            uint after = 0;
            uint before;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 2://(An)
                    before = mem.PeekW(reg.A[dr]);
                    after = val | before;
                    mem.PokeW(reg.A[dr], (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Or_w[0];
                    break;
                case 3://(An)+
                    before = mem.PeekW(reg.A[dr]);
                    after = val | before;
                    mem.PokeW(reg.A[dr], (ushort)after);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Or_w[1];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    before = mem.PeekW(reg.A[dr]);
                    after = val | before;
                    mem.PokeW(reg.A[dr], (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Or_w[2];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekW((UInt32)(reg.A[dr] + d16));
                    after = val | before;
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Or_w[3];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekW(ptr);
                    after = val | before;
                    mem.PokeW(ptr, (ushort)after);
                    cycle = cy.Or_w[4];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = val | before;
                            mem.PokeW(ptr, (ushort)after);
                            cycle = cy.Or_w[5];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = val | before;
                            mem.PokeW(ptr, (ushort)after);
                            cycle = cy.Or_w[6];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            reg.SetZ((ushort)after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CorDnEal(ushort n)
        {
            throw new NotImplementedException();
        }

        private int CorEaDnb(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "OR.b ";
#endif

            int cycle = 0;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);
            int dr = (n & 0x0e00) >> 9;

            byte val = reg.GetDb(dr);

            byte after = 0;
            byte before;

            //if (sm == 7 && sr == 4)
            //{
            //    throw new NotImplementedException("OR <ea>,Dnの#Immのパターンは未実装");
            //}
            before = (byte)srcAddressingByte(ref nimo, ref cycle, sm, sr, 0b1111_1111_1101);

            cycle = cy.OrEaDn_b[cycle];
            after = (byte)(val | before);
#if DEBUG_M68
            nimo += string.Format(",D{0}", dr);
#endif

            reg.SetDb(dr, after);

            //flag
            //reg.X
            reg.N = (after & 0x800) != 0;
            reg.SetZ((byte)after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CorEaDnw(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "OR.w ";
#endif

            int cycle = 0;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);
            int dr = (n & 0x0e00) >> 9;

            uint val = reg.GetDw(dr);

            uint after = 0;
            uint before;

            before = srcAddressingWord(ref nimo, ref cycle, sm, sr, 0b1111_1111_1101);

            cycle = cy.OrEaDn_w[cycle];
            after = val | before;
#if DEBUG_M68
            nimo += string.Format(",D{0}", dr);
#endif

            reg.SetDw(dr, (ushort)after);

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            reg.SetZ((ushort)after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CorEaDnl(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "OR.l ";
#endif

            int cycle = 0;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);
            int dr = (n & 0x0e00) >> 9;

            uint val = reg.GetDl(dr);

            uint after = 0;
            uint before;

            before = srcAddressingLong(ref nimo, ref cycle, sm, sr, 0b1111_1111_1101);
            cycle = cy.OrEaDn_l[cycle];
            after = val | before;
#if DEBUG_M68
            nimo += string.Format(",D{0}", dr);
#endif

            reg.SetDl(dr, (uint)after);

            //flag
            //reg.X
            reg.N = (after & 0x8000_0000) != 0;
            reg.SetZ(after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ceor(ushort n)
        {
            if ((n & 0x0100) == 0) return Ccmp(n);
            if ((n & 0xf138) == 0xb108)
                return Ccmp(n);

            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0://byte
                    return Ceorb(n);
                case 1://word
                    return Ceorw(n);
                case 2://long
                    return Ceorl(n);
                case 3://cmpa.l
                    return Ccmpa_l(n);
            }
            throw new NotImplementedException();
        }

        private int Ceorb(ushort n)
        {
#if DEBUG_M68
            string nimo = "EOR.b ";
#endif

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            uint val = reg.GetDb(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            uint after = 0;
            uint before;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    before = reg.GetDb(dr);
                    after = val ^ before;
                    reg.SetDb(dr, (byte)after);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Eor_b[0];
                    break;
                case 2://(An)
                    before = mem.PeekB(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeB(reg.A[dr], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Eor_b[1];
                    break;
                case 3://(An)+
                    before = mem.PeekB(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeB(reg.A[dr], (byte)after);
                    reg.A[dr] ++;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Eor_b[2];
                    break;
                case 4://-(An)
                    reg.A[dr] --;
                    if (dr == 7) reg.A[dr]--;
                    before = mem.PeekB(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeB(reg.A[dr], (byte)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Eor_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekB((UInt32)(reg.A[dr] + d16));
                    after = val ^ before;
                    mem.PokeB((UInt32)(reg.A[dr] + d16), (byte)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Eor_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekB(ptr);
                    after = val ^ before;
                    mem.PokeB(ptr, (byte)after);
                    cycle = cy.Eor_b[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = val ^ before;
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Eor_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekB(ptr);
                            after = val ^ before;
                            mem.PokeB(ptr, (byte)after);
                            cycle = cy.Eor_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x80) != 0;
            reg.SetZ((byte)after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ceorw(ushort n)
        {
#if DEBUG_M68
            string nimo = "EOR.w ";
#endif

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            uint val = reg.GetDw(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            uint after = 0;
            uint before;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    before = reg.GetDw(dr);
                    after = val ^ before;
                    reg.SetDw(dr, (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Eor_w[0];
                    break;
                case 2://(An)
                    before = mem.PeekW(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeW(reg.A[dr], (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Eor_w[1];
                    break;
                case 3://(An)+
                    before = mem.PeekW(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeW(reg.A[dr], (ushort)after);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Eor_w[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    before = mem.PeekW(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeW(reg.A[dr], (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Eor_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekW((UInt32)(reg.A[dr] + d16));
                    after = val ^ before;
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Eor_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekW(ptr);
                    after = val ^ before;
                    mem.PokeW(ptr, (ushort)after);
                    cycle = cy.Eor_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = val ^ before;
                            mem.PokeW(ptr, (ushort)after);
                            cycle = cy.Eor_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekW(ptr);
                            after = val ^ before;
                            mem.PokeW(ptr, (ushort)after);
                            cycle = cy.Eor_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000) != 0;
            reg.SetZ((ushort)after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ceorl(ushort n)
        {
#if DEBUG_M68
            string nimo = "EOR.l ";
#endif

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            uint val = reg.GetDl(sr);
#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            uint after = 0;
            uint before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    before = reg.GetDl(dr);
                    after = val ^ before;
                    reg.D[dr] = after;
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Eor_l[0];
                    break;
                case 2://(An)
                    before = mem.PeekL(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeL(reg.A[dr], after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Eor_l[1];
                    break;
                case 3://(An)+
                    before = mem.PeekL(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeL(reg.A[dr], after);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Eor_l[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    before = mem.PeekL(reg.A[dr]);
                    after = val ^ before;
                    mem.PokeL(reg.A[dr], after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Eor_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = mem.PeekL((UInt32)(reg.A[dr] + d16));
                    after = val ^ before;
                    mem.PokeL((UInt32)(reg.A[dr] + d16), after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Eor_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    before = mem.PeekL(ptr);
                    after = val ^ before;
                    mem.PokeL(ptr, after);
                    cycle = cy.Eor_l[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val ^ before;
                            mem.PokeL(ptr, after);
                            cycle = cy.Eor_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = mem.PeekL(ptr);
                            after = val ^ before;
                            mem.PokeL(ptr, after);
                            cycle = cy.Eor_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (after & 0x8000_0000) != 0;
            reg.SetZ(after);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ceori(ushort n)
        {
            if ((n & 0x00c0) == 0x00c0) throw new NotImplementedException();

            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0://byte
                    return Ceorib(n);
                case 1://word
                    return Ceoriw(n);
                case 2://long
                    return Ceoril(n);
            }
            throw new NotImplementedException();
        }

        private int Ceorib(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Ceoriw(ushort n)
        {
#if DEBUG_M68
            string nimo = "EORI.w ";
#endif

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            ushort src = FetchW();
#if DEBUG_M68
            nimo += string.Format("#{0:x04},", src);
#endif


            ushort ans = 0;
            ushort dst;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    dst = reg.GetDw(dr);
                    ans = (ushort)(src ^ dst);
                    reg.SetDw(dr, ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Eori_w[0];
                    break;
                case 2://(An)
                    dst = mem.PeekW(reg.A[dr]);
                    ans = (ushort)(src ^ dst);
                    mem.PokeW(reg.A[dr], (ushort)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Eori_w[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekW(reg.A[dr]);
                    ans = (ushort)(src ^ dst);
                    mem.PokeW(reg.A[dr], (ushort)ans);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Eori_w[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    dst = mem.PeekW(reg.A[dr]);
                    ans = (ushort)(src ^ dst);
                    mem.PokeW(reg.A[dr], (ushort)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Eori_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekW((UInt32)(reg.A[dr] + d16));
                    ans = (ushort)(src ^ dst);
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (ushort)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Eori_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekW(ptr);
                    ans = (ushort)(src ^ dst);
                    mem.PokeW(ptr, (ushort)ans);
                    cycle = cy.Eori_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekW(ptr);
                            ans = (ushort)(src ^ dst);
                            mem.PokeW(ptr, (ushort)ans);
                            cycle = cy.Eori_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekW(ptr);
                            ans = (ushort)(src ^ dst);
                            mem.PokeW(ptr, (ushort)ans);
                            cycle = cy.Eori_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = (ans & 0x8000) != 0;
            reg.SetZ((ushort)ans);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ceoril(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cbtst08(ushort n)
        {
            if ((n & 0xffc0) == 0x08c0)
            {
                return Cbset_imm(n);
            }

            if ((n & 0xffc0) == 0x0880)
            {
                return Cbclr_imm(n);
            }

            if ((n & 0xffc0) != 0x0800)
            {
                throw new NotImplementedException();
            }

            int cycle = 10;
            int data = FetchW() & 0xff;
#if DEBUG_M68
            string nimo = "BTST";
#endif


            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

#if DEBUG_M68
            if (m == 0) nimo += ".l";
#endif

#if DEBUG_M68
            else nimo += ".b";
#endif


#if DEBUG_M68
            nimo += string.Format(" #${0:x02},", data);
#endif


            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 d16;

            switch (m)
            {
                case 0://Dn
                    dst = reg.D[r];
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Btst08[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Btst08[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Btst08[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Btst08[3];
                    break;
                case 5://d16(An)
                    d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Btst08[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    cycle = cy.Btst08[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            cycle = cy.Btst08[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            cycle = cy.Btst08[7];
                            break;
                        case 2://d16(PC)
                            d16 = (Int16)FetchW();
                            dst = mem.PeekB((UInt32)(reg.PC + d16 - 2));
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", d16);
#endif

                            cycle = cy.Btst08[8];
                            break;
                        case 3://d8(An,IX)
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            dst = mem.PeekB(ptr);
                            cycle = cy.Btst08[9];
                            break;
                    }
                    break;
            }

            //compute
            bool ans;
            if (m == 0) data %= 32;
            else data %= 8;
            ans = (dst & (1 << data)) == 0;

            //flag
            //reg.X
            //reg.N
            reg.Z = ans;
            //reg.V
            //reg.C

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cbsetbtst(ushort n)
        {
            if ((n & 0xf1c0) == 0x0100)
            {
                return Cbtst(n);
            }
            if ((n & 0xf1c0) == 0x01c0)
            {
                return Cbset(n);
            }
            if ((n & 0xf1c0) == 0x0180)
            {
                return Cbclr_Dn(n);
            }
            throw new NotImplementedException("dummy");
        }

        private int Cbtst(ushort n)
        {
            int cycle;
#if DEBUG_M68
            string nimo = "BTST";
#endif


            int sr = (n & 0x0e00) >> 9;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

#if DEBUG_M68
            if (m == 0) nimo += ".l ";
#endif

#if DEBUG_M68
            else nimo += ".b ";
#endif


            int data = (int)reg.GetDl(sr);
#if DEBUG_M68
            nimo += string.Format("D{0:d},", sr);
#endif


            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 d16;

            //compute
            bool ans;
            if (m == 0) data %= 32;
            else data %= 8;

            switch (m)
            {
                case 0://Dn
                    dst = reg.D[r];
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    //reg.D[r] = dst | (uint)(1 << data);
                    cycle = cy.Btst[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    //mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Btst[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    //mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
                    cycle = cy.Btst[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    //mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Btst[3];
                    break;
                case 5://d16(An)
                    d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    //mem.PokeB((UInt32)(reg.A[r] + d16), (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Btst[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (dst & (1 << data)) == 0;
                    //mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Btst[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            //mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Btst[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            //mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Btst[7];
                            break;
                        case 2://d16(PC)
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", (UInt16)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr + reg.PC - 2);
                            ans = (dst & (1 << data)) == 0;
                            //mem.PokeB(ptr + reg.PC - 2, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Btst[8];
                            break;
                        case 3://d8(PC,IX)
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            //mem.PokeB(ptr + reg.PC - 2, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Btst[9];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


            //flag
            //reg.X
            //reg.N
            reg.Z = ans;
            //reg.V
            //reg.C

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cbset(ushort n)
        {
            int cycle;
#if DEBUG_M68
            string nimo = "BSET";
#endif


            int sr = (n & 0x0e00) >> 9;
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

#if DEBUG_M68
            if (m == 0) nimo += ".l ";
#endif

#if DEBUG_M68
            else nimo += ".b ";
#endif


            int data = (int)reg.GetDl(sr);
#if DEBUG_M68
            nimo += string.Format("D{0:d},", sr);
#endif


            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 d16;

            //compute
            bool ans;
            if (m == 0) data %= 32;
            else data %= 8;

            switch (m)
            {
                case 0://Dn
                    dst = reg.D[r];
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    reg.D[r] = dst | (uint)(1 << data);
                    cycle = cy.Bset[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
                    cycle = cy.Bset[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset[3];
                    break;
                case 5://d16(An)
                    d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Bset[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Bset[7];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


            //flag
            //reg.X
            //reg.N
            reg.Z = ans;
            //reg.V
            //reg.C

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cbset_imm(ushort n)
        {
            int cycle;
            int data = FetchW() & 0xff;
#if DEBUG_M68
            string nimo = "BSET";
#endif


            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

#if DEBUG_M68
            if (m == 0) nimo += ".l ";
#endif

#if DEBUG_M68
            else nimo += ".b ";
#endif


#if DEBUG_M68
            nimo += string.Format("#${0:x02},", data);
#endif


            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 d16;

            //compute
            bool ans;
            if (m == 0) data %= 32;
            else data %= 8;

            switch (m)
            {
                case 0://Dn
                    dst = reg.D[r];
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    reg.D[r] = dst | (uint)(1 << data);
                    cycle = cy.Bset_i[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset_i[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
                    cycle = cy.Bset_i[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset_i[3];
                    break;
                case 5://d16(An)
                    d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset_i[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                    cycle = cy.Bset_i[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Bset_i[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst | (byte)(1 << data)));
                            cycle = cy.Bset_i[7];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


            //flag
            //reg.X
            //reg.N
            reg.Z = ans;
            //reg.V
            //reg.C

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }
        
        private int Cbclr_Dn(ushort n)
        {
            int cycle;
#if DEBUG_M68
            string nimo = "BCLR";
#endif


            int sr = (n & 0x0e00) >> 9;
            int data = (int)reg.GetDl(sr);
            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

#if DEBUG_M68
            if (m == 0) nimo += ".l ";
#endif

#if DEBUG_M68
            else nimo += ".b ";
#endif


#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 d16;

            //compute
            bool ans;
            if (m == 0) data %= 32;
            else data %= 8;

            switch (m)
            {
                case 0://Dn
                    dst = reg.D[r];
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    reg.D[r] = dst & (uint)~(1 << data);
                    cycle = cy.Bclr_i[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst & (byte)~(1 << data)));
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
                    cycle = cy.Bclr_i[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[3];
                    break;
                case 5://d16(An)
                    d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(ptr, (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst & (byte)~(1 << data)));
                            cycle = cy.Bclr_i[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst & (byte)~(1 << data)));
                            cycle = cy.Bclr_i[7];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


            //flag
            //reg.X
            //reg.N
            reg.Z = ans;
            //reg.V
            //reg.C

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cbclr_imm(ushort n)
        {
            int cycle;
            int data = FetchW() & 0xff;
#if DEBUG_M68
            string nimo = "BCLR";
#endif


            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

#if DEBUG_M68
            if (m == 0) nimo += ".l ";
#endif

#if DEBUG_M68
            else nimo += ".b ";
#endif


#if DEBUG_M68
            nimo += string.Format("#${0:x02},", data);
#endif


            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 d16;

            //compute
            bool ans;
            if (m == 0) data %= 32;
            else data %= 8;

            switch (m)
            {
                case 0://Dn
                    dst = reg.D[r];
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    reg.D[r] = dst & (uint)~(1 << data);
                    cycle = cy.Bclr_i[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst & (byte)~(1 << data)));
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
                    cycle = cy.Bclr_i[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(reg.A[r], (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[3];
                    break;
                case 5://d16(An)
                    d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB((UInt32)(reg.A[r] + d16), (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (dst & (1 << data)) == 0;
                    mem.PokeB(ptr, (byte)(dst & (byte)~(1 << data)));
                    cycle = cy.Bclr_i[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst & (byte)~(1 << data)));
                            cycle = cy.Bclr_i[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            ans = (dst & (1 << data)) == 0;
                            mem.PokeB(ptr, (byte)(dst & (byte)~(1 << data)));
                            cycle = cy.Bclr_i[7];
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


            //flag
            //reg.X
            //reg.N
            reg.Z = ans;
            //reg.V
            //reg.C

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ccmpi(ushort n)
        {
            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0:
                    return Ccmpib(n);
                case 1:
                    return Ccmpiw(n);
                case 2:
                    return Ccmpil(n);
            }
            throw new NotImplementedException("dummy");
        }

        private int Ccmpib(ushort n)
        {

#if DEBUG_M68
            string nimo = "CMPI.b ";
#endif


            int cycle = 0;

            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            ushort val = (ushort)(short)(sbyte)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x02},",(byte)val);
#endif


            ushort after = 0;
            ushort before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = (ushort)(short)(sbyte)reg.D[r];
                    after = (ushort)((short)before - (short)val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Cmpi_b[0];
                    break;
                case 2://(An)
                    before = (ushort)(short)(sbyte)mem.PeekB(reg.A[r]);
                    after = (ushort)((short)before - (short)val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Cmpi_b[1];
                    break;
                case 3://(An)+
                    before = (ushort)(short)(sbyte)mem.PeekB(reg.A[r]);
                    after = (ushort)((short)before - (short)val);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Cmpi_b[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    before = (ushort)(short)(sbyte)mem.PeekB(reg.A[r]);
                    after = (ushort)((short)before - (short)val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Cmpi_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = (ushort)(short)(sbyte)mem.PeekB((UInt32)(reg.A[r] + d16));
                    after = (ushort)((short)before - (short)val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Cmpi_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = (ushort)(short)(sbyte)mem.PeekB(ptr);
                    after = (ushort)((short)before - (short)val);
                    cycle = cy.Cmpi_b[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            after = (ushort)((short)before - (short)val);
                            cycle = cy.Cmpi_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            after = (ushort)((short)before - (short)val);
                            cycle = cy.Cmpi_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.SetN((byte)after);
            reg.SetZ((byte)after);
            //reg.SetVcmp((byte)before, (byte)val, (byte)after);
            //reg.SetCcmp((byte)before, (byte)val, (byte)after);
            reg.SetVcmp((byte)val, (byte)before, (byte)after);
            reg.SetCcmp((byte)val, (byte)before, (byte)after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ccmpiw(ushort n)
        {
#if DEBUG_M68
            string nimo = "CMPI.w ";
#endif


            int cycle = 0;

            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            UInt32 val = (UInt32)(Int32)(short)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x04},", (ushort)val);
#endif


            UInt32 after = 0;
            UInt32 before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = (UInt32)(Int32)(Int16)reg.GetDw(r);
                    after = (UInt32)((Int32)before - (Int32)val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Cmpi_w[0];
                    break;
                case 2://(An)
                    before = (UInt32)(Int32)(Int16)mem.PeekW(reg.A[r]);
                    after = (UInt32)((Int32)before - (Int32)val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Cmpi_w[1];
                    break;
                case 3://(An)+
                    before = (UInt32)(Int32)(Int16)mem.PeekW(reg.A[r]);
                    after = (UInt32)((Int32)before - (Int32)val);
                    reg.A[r] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Cmpi_w[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 2;
                    before = (UInt32)(Int32)(Int16)mem.PeekW(reg.A[r]);
                    after = (UInt32)((Int32)before - (Int32)val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Cmpi_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = (UInt32)(Int32)(Int16)mem.PeekW((UInt32)(reg.A[r] + d16));
                    after = (UInt32)((Int32)before - (Int32)val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Cmpi_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = (UInt32)(Int32)(Int16)mem.PeekW(ptr);
                    after = (UInt32)((Int32)before - (Int32)val);
                    cycle = cy.Cmpi_w[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = (UInt32)(Int32)(Int16)mem.PeekW(ptr);
                            after = (UInt32)((Int32)before - (Int32)val);
                            cycle = cy.Cmpi_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = (UInt32)(Int32)(Int16)mem.PeekW(ptr);
                            after = (UInt32)((Int32)before - (Int32)val);
                            cycle = cy.Cmpi_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.SetN((UInt16)after);
            reg.SetZ((UInt16)after);
            //reg.SetVcmp((UInt16)before, (UInt16)val, (UInt16)after);
            //reg.SetCcmp((UInt16)before, (UInt16)val, (UInt16)after);
            reg.SetVcmp((UInt16)val, (UInt16)before, (UInt16)after);
            reg.SetCcmp((UInt16)val, (UInt16)before, (UInt16)after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ccmpil(ushort n)
        {
#if DEBUG_M68
            string nimo = "CMPI.l ";
#endif


            int cycle = 0;

            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            UInt64 val = (UInt64)(Int64)(int)FetchL();
#if DEBUG_M68
            nimo += string.Format("#${0:x08},", val);
#endif


            UInt64 after = 0;
            UInt64 before = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    before = (UInt64)(Int64)(Int32)reg.D[r];
                    after = (UInt64)((Int64)before - (Int64)val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    cycle = cy.Cmpi_l[0];
                    break;
                case 2://(An)
                    before = (UInt64)(Int64)(Int32)mem.PeekL(reg.A[r]);
                    after = (UInt64)((Int64)before - (Int64)val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    cycle = cy.Cmpi_l[1];
                    break;
                case 3://(An)+
                    before = (UInt64)(Int64)(Int32)mem.PeekL(reg.A[r]);
                    after = (UInt64)((Int64)before - (Int64)val);
                    reg.A[r] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Cmpi_l[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 4;
                    before = (UInt64)(Int64)(Int32)mem.PeekL(reg.A[r]);
                    after = (UInt64)((Int64)before - (Int64)val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Cmpi_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = (UInt64)(Int64)(Int32)mem.PeekL((UInt32)(reg.A[r] + d16));
                    after = (UInt64)((Int64)before - (Int64)val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Cmpi_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    before = (UInt64)(Int64)(Int32)mem.PeekL(ptr);
                    after = (UInt64)((Int64)before - (Int64)val);
                    cycle = cy.Cmpi_l[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = (UInt64)(Int64)(Int32)mem.PeekL(ptr);
                            after = (UInt64)((Int64)before - (Int64)val);
                            cycle = cy.Cmpi_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = (UInt64)(Int64)(Int32)mem.PeekL(ptr);
                            after = (UInt64)((Int64)before - (Int64)val);
                            cycle = cy.Cmpi_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.SetN((UInt32)after);
            reg.SetZ((UInt32)after);
            //reg.SetVcmp((UInt32)before, (UInt32)val, (UInt32)after);
            //reg.SetCcmp((UInt32)before, (UInt32)val, (UInt32)after);
            reg.SetVcmp((UInt32)val, (UInt32)before, (UInt32)after);
            reg.SetCcmp((UInt32)val, (UInt32)before, (UInt32)after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubi(ushort n)
        {
            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0:
                    return Csubib(n);
                case 1:
                    return Csubiw(n);
                case 2:
                    return Csubil(n);
            }
            throw new NotImplementedException("dummy");
        }

        private int Csubib(ushort n)
        {
#if DEBUG_M68
            string nimo = "SUBI.b ";
#endif


            int cycle = 0;

            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            byte src = (byte)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x02},", src);
#endif

            byte ans = 0;
            byte dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    dst = reg.GetDb(r);
                    ans = (byte)((sbyte)dst - (sbyte)src);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    reg.SetDb(r, ans);
                    cycle = cy.Subi_b[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[r]);
                    ans = (byte)((sbyte)dst - (sbyte)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    mem.PokeB(reg.A[r], ans);
                    cycle = cy.Subi_b[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[r]);
                    ans = (byte)((sbyte)dst - (sbyte)src);
                    mem.PokeB(reg.A[r], ans);
                    reg.A[r] += 1;
                    if (r == 7) reg.A[r]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Subi_b[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 1;
                    if (r == 7) reg.A[r]--;
                    dst = mem.PeekB(reg.A[r]);
                    ans = (byte)((sbyte)dst - (sbyte)src);
                    mem.PokeB(reg.A[r], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Subi_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[r] + d16));
                    ans = (byte)((sbyte)dst - (sbyte)src);
                    mem.PokeB((UInt32)(reg.A[r] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Subi_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (byte)((sbyte)dst - (sbyte)src);
                    mem.PokeB(ptr, ans);
                    cycle = cy.Subi_b[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (byte)((sbyte)dst - (sbyte)src);
                            mem.PokeB(ptr, ans);
                            cycle = cy.Subi_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (byte)((sbyte)dst - (sbyte)src);
                            mem.PokeB(ptr, ans);
                            cycle = cy.Subi_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.SetN((byte)ans);
            reg.SetZ((byte)ans);
            reg.SetVcmp((byte)src, (byte)dst, (byte)ans);
            reg.SetCcmp((byte)src, (byte)dst, (byte)ans);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubiw(ushort n)
        {
#if DEBUG_M68
            string nimo = "SUBI.w ";
#endif


            int cycle = 0;

            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            ushort src = (ushort)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x04},", src);
#endif

            ushort ans = 0;
            ushort dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    dst = reg.GetDw(r);
                    ans = (ushort)((short)dst - (short)src);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    reg.SetDw(r, ans);
                    cycle = cy.Subi_w[0];
                    break;
                case 2://(An)
                    dst = mem.PeekW(reg.A[r]);
                    ans = (ushort)((short)dst - (short)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    mem.PokeW(reg.A[r], ans);
                    cycle = cy.Subi_w[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekW(reg.A[r]);
                    ans = (ushort)((short)dst - (short)src);
                    mem.PokeW(reg.A[r], ans);
                    reg.A[r] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Subi_w[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 2;
                    dst = mem.PeekW(reg.A[r]);
                    ans = (ushort)((short)dst - (short)src);
                    mem.PokeW(reg.A[r], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Subi_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekW((UInt32)(reg.A[r] + d16));
                    ans = (ushort)((short)dst - (short)src);
                    mem.PokeW((UInt32)(reg.A[r] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Subi_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekW(ptr);
                    ans = (ushort)((short)dst - (short)src);
                    mem.PokeW(ptr, ans);
                    cycle = cy.Subi_w[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekW(ptr);
                            ans = (ushort)((short)dst - (short)src);
                            mem.PokeW(ptr, ans);
                            cycle = cy.Subi_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekW(ptr);
                            ans = (ushort)((short)dst - (short)src);
                            mem.PokeW(ptr, ans);
                            cycle = cy.Subi_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVcmp(src, dst, ans);
            reg.SetCcmp(src, dst, ans);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubil(ushort n)
        {
#if DEBUG_M68
            string nimo = "SUBI.l ";
#endif


            int cycle = 0;

            int m = (n & 0x0038) >> 3;
            int r = (n & 0x0007);

            UInt32 src = (UInt32)FetchL();
#if DEBUG_M68
            nimo += string.Format("#${0:x08},", src);
#endif

            UInt32 ans = 0;
            UInt32 dst = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (m)
            {
                case 0://Dn
                    dst = reg.GetDl(r);
                    ans = (UInt32)((Int32)dst - (Int32)src);
#if DEBUG_M68
                    nimo += string.Format("D{0}", r);
#endif

                    reg.SetDl(r, ans);
                    cycle = cy.Subi_l[0];
                    break;
                case 2://(An)
                    dst = mem.PeekL(reg.A[r]);
                    ans = (UInt32)((Int32)dst - (Int32)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", r);
#endif

                    mem.PokeL(reg.A[r], ans);
                    cycle = cy.Subi_l[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekL(reg.A[r]);
                    ans = (UInt32)((Int32)dst - (Int32)src);
                    mem.PokeL(reg.A[r], ans);
                    reg.A[r] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", r);
#endif

                    cycle = cy.Subi_l[2];
                    break;
                case 4://-(An)
                    reg.A[r] -= 4;
                    dst = mem.PeekL(reg.A[r]);
                    ans = (UInt32)((Int32)dst - (Int32)src);
                    mem.PokeL(reg.A[r], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", r);
#endif

                    cycle = cy.Subi_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekL((UInt32)(reg.A[r] + d16));
                    ans = (UInt32)((Int32)dst - (Int32)src);
                    mem.PokeL((UInt32)(reg.A[r] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, r);
#endif

                    cycle = cy.Subi_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, r, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekL(ptr);
                    ans = (UInt32)((Int32)dst - (Int32)src);
                    mem.PokeL(ptr, ans);
                    cycle = cy.Subi_l[5];
                    break;
                case 7://etc.
                    switch (r)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekL(ptr);
                            ans = (UInt32)((Int32)dst - (Int32)src);
                            mem.PokeL(ptr, ans);
                            cycle = cy.Subi_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekL(ptr);
                            ans = (UInt32)((Int32)dst - (Int32)src);
                            mem.PokeL(ptr, ans);
                            cycle = cy.Subi_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVcmp(src, dst, ans);
            reg.SetCcmp(src, dst, ans);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmovea(ushort n)
        {
            int size = (n & 0x3000) >> 12;
            switch (size)
            {
                case 2://long
                    return Cmoveal(n);
                case 3://word
                    return Cmoveaw(n);
            }

            throw new NotImplementedException("dummy");
        }

        private int Cmoveaw(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "MOVEA.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 val = srcAddressingWord(ref nimo, ref cycle, sm, sr);

            reg.SetAw(dr, (ushort)val);
#if DEBUG_M68
            nimo += string.Format(",A{0}", dr);
#endif

            cycle = cy.Movea_w[cycle];

            //flag
            //全て変化せず

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmoveal(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "MOVEA.l ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 val = srcAddressingLong(ref nimo, ref cycle, sm, sr);

            reg.A[dr] = val;
#if DEBUG_M68
            nimo += string.Format(",A{0}", dr);
#endif

            cycle = cy.Movea_l[cycle];

            //flag
            //全て変化せず

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CmoveFromSR(ushort n)
        {
#if DEBUG_M68
            string nimo = "MOVE.w sr,";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //src
            Int16 val = (Int16)reg.SR;

            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.SetDw(dr, (ushort)val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.MoveFromSr_w[0];
                    break;
                case 2://(An)
                    mem.PokeW(reg.A[dr], (UInt16)val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.MoveFromSr_w[1];
                    break;
                case 3://(An)+
                    mem.PokeW(reg.A[dr], (UInt16)val);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.MoveFromSr_w[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    mem.PokeW(reg.A[dr], (UInt16)val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.MoveFromSr_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (UInt16)val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.MoveFromSr_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeW(ptr, (UInt16)val);
                    cycle = cy.MoveFromSr_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            mem.PokeW(ptr, (UInt16)val);
                            cycle = cy.MoveFromSr_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            mem.PokeW(ptr, (UInt16)val);
                            cycle = cy.MoveFromSr_w[7];
                            break;
                    }
                    break;
            }

            //flag

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CmoveToSR(ushort n)
        {
            if ((n & 0xffc0) != 0x46c0)
            {
                return Cnot(n);
            }

            string nimo = "";
#if DEBUG_M68
            nimo = "MOVE.w ";
#endif


            int cycle = 0;

            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int16 val = (Int16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",sr";
#endif


            cycle = cy.MoveToSr_w[cycle];

            //dst
            //flag
            reg.SR = (ushort)((reg.SR & 0b1010_0111_0001_1111) | (ushort)val);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmove(ushort n)
        {
            if ((n & 0xffc0) == 0x40c0)
            {
                return CmoveFromSR(n);
            }

            int size = (n & 0x3000) >> 12;

            if ((size == 2 || size == 3) && (n & 0xc1c0) == 0x40)
            {
                return Cmovea(n);
            }

            if ((n & 0x3000) == 0x00)
            {
                //MOVE以外の命令っぽい
                throw new NotImplementedException(string.Format("未実装!! [{0:x04}]", n));
            }


            switch (size)
            {
                case 1://byte
                    return Cmoveb(n);
                case 2://long
                    return Cmovel(n);
                case 3://word
                    return Cmovew(n);
            }

            throw new NotImplementedException("dummy");
        }

        private int Cmoveb(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "MOVE.b ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //src
            sbyte val = (sbyte)srcAddressingByte(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.SetDb(dr, (byte)val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Move_b[cycle][0];
                    break;
                case 2://(An)
                    mem.PokeB(reg.A[dr], (byte)val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Move_b[cycle][1];
                    break;
                case 3://(An)+
                    mem.PokeB(reg.A[dr], (byte)val);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Move_b[cycle][2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    mem.PokeB(reg.A[dr], (byte)val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Move_b[cycle][3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeB((UInt32)(reg.A[dr] + d16), (byte)val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Move_b[cycle][4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeB(ptr, (byte)val);
                    cycle = cy.Move_b[cycle][5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            mem.PokeB(ptr, (byte)val);
                            cycle = cy.Move_b[cycle][6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            mem.PokeB(ptr, (byte)val);
                            cycle = cy.Move_b[cycle][7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN((byte)val);
            reg.SetZ((byte)val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmovew(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "MOVE.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //src
            Int16 val = (Int16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.SetDw(dr, (ushort)val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Move_w[cycle][0];
                    break;
                case 2://(An)
                    mem.PokeW(reg.A[dr], (UInt16)val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Move_w[cycle][1];
                    break;
                case 3://(An)+
                    mem.PokeW(reg.A[dr], (UInt16)val);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Move_w[cycle][2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    mem.PokeW(reg.A[dr], (UInt16)val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Move_w[cycle][3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (UInt16)val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Move_w[cycle][4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeW(ptr, (UInt16)val);
                    cycle = cy.Move_w[cycle][5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            mem.PokeW(ptr, (UInt16)val);
                            cycle = cy.Move_w[cycle][6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            mem.PokeW(ptr, (UInt16)val);
                            cycle = cy.Move_w[cycle][7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN((ushort)val);
            reg.SetZ((ushort)val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmovel(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "MOVE.l ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //src
            UInt32 val = srcAddressingLong(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.D[dr] = val;
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Move_l[cycle][0];
                    break;
                case 2://(An)
                    mem.PokeL(reg.A[dr], val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Move_l[cycle][1];
                    break;
                case 3://(An)+
                    mem.PokeL(reg.A[dr], val);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Move_l[cycle][2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    mem.PokeL(reg.A[dr], val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Move_l[cycle][3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeL((UInt32)(reg.A[dr] + d16), val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Move_l[cycle][4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeL(ptr, val);
                    cycle = cy.Move_l[cycle][5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            mem.PokeL(ptr, val);
                            cycle = cy.Move_l[cycle][6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            mem.PokeL(ptr, val);
                            cycle = cy.Move_l[cycle][7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN((UInt32)val);
            reg.SetZ((UInt32)val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clea(UInt16 n)
        {
            int cycle = 0;

            if ((n & 0xc0) != 0xc0)
            {
                //LEA以外の命令っぽい
                throw new NotImplementedException(string.Format("未実装!! [{0:x04}]", n));
            }

            int a = (n & 0x0e00) >> 9;
            int mode = (n & 0x0038) >> 3;
            int r = (n & 0x0007);
            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            uint ptr;

            switch (mode)
            {
                case 0:
                case 1:
                case 3:
                case 4:
                    throw new NotImplementedException(string.Format("LEA 不正なアドレッシングモード {0:x04}", n));
                case 2://(An)
#if DEBUG_M68
                    Log.WriteLine(LogLevel.Trace, "LEA (A{0}),A{1}", r, a);
#endif
                    reg.A[a] = reg.A[r];
                    cycle = 4;
                    break;
                case 5://d16(An)
                    vw = (UInt16)(Int16)FetchW();
#if DEBUG_M68
                    Log.WriteLine(LogLevel.Trace, "LEA ${0:x04}(A{1}),A{2} ; d16+A{1}=${3:x08}",
                        vw, r, a, (UInt32)(reg.A[r] + (Int16)vw));
#endif
                    reg.A[a] = (UInt32)(reg.A[r] + (Int16)vw);
                    cycle = 8;
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
                    if (!isL) ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[r] + ((sbyte)(byte)vw) + IX);
#if DEBUG_M68
                    Log.WriteLine(LogLevel.Trace, "LEA ${0:x02}(A{1},{2}),A{3} ; d8+A{1}+IX=${4:x08}",
                        vw, r, isA ? string.Format("A{0}", ni) : string.Format("D{0}", ni), a, ptr);
#endif
                    reg.A[a] = ptr;
                    cycle = 12;
                    break;
                case 7://etc
                    if (r == 0)//Abs.W
                    {
                        ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                        Log.WriteLine(LogLevel.Trace, "LEA ${0:x08},A{1}", ptr, a);
#endif
                        reg.A[a] = ptr;
                        cycle = 8;
                    }
                    else if (r == 1)//Abs.L
                    {
                        ptr = FetchL();
#if DEBUG_M68
                        Log.WriteLine(LogLevel.Trace, "LEA ${0:x08},A{1}", ptr, a);
#endif
                        reg.A[a] = ptr;
                        cycle = 12;
                    }
                    else if (r == 2)//d16(PC)
                    {
                        ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                        Log.WriteLine(LogLevel.Trace, "LEA ${0:x04}(PC),A{1} ; d16+PC=${2:x08}", ptr, a, ptr + reg.PC);
#endif
                        reg.A[a] = ptr + reg.PC - 2;
                        cycle = 8;
                    }
                    else if (r == 3)//d8(PC,IX)
                    {
                        vw = FetchW();
                        isA = (vw & 0x8000) != 0;
                        ni = (vw & 0x7000) >> 12;
                        isL = (vw & 0x0800) != 0;
                        IX = (isA ? reg.A[ni] : reg.D[ni]);
                        if (!isL) ptr = (UInt32)(reg.PC - 2 + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                        else ptr = (UInt32)(reg.PC - 2 + ((sbyte)(byte)vw) + IX);
#if DEBUG_M68
                        Log.WriteLine(LogLevel.Trace, "LEA ${0:x02}(PC,{1}.{4}),A{2} ; d8+PC+{1}.{4}=${3:x08}",
                            vw,
                            isA ? string.Format("A{0}", ni) : string.Format("D{0}", ni),
                            a,
                            ptr,
                            isL?"l":"w");
#endif
                        reg.A[a] = ptr;
                        cycle = 12;
                    }
                    else
                        throw new NotImplementedException(string.Format("LEA 知らないモード {0:x04}", n));
                    break;
            }

            return cycle;
        }

        private int Cnot(UInt16 n)
        {
            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0x0:
                    return Cnot_b(n);
                case 0x1:
                    return Cnot_w(n);
                case 0x2:
                    return Cnot_l(n);
            }

            throw new NotImplementedException();
        }

        private int Cnot_b(ushort n)
        {
#if DEBUG_M68
            string nimo = "NOT.b ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            byte ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    ans = reg.GetDb(dr);
                    ans = (byte)~ans;
                    reg.SetDb(dr, ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Not_b[0];
                    break;
                case 2://(An)
                    ans = mem.PeekB(reg.A[dr]);
                    ans = (byte)~ans;
                    mem.PokeB(reg.A[dr], ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Not_b[1];
                    break;
                case 3://(An)+
                    ans = mem.PeekB(reg.A[dr]);
                    ans = (byte)~ans;
                    mem.PokeB(reg.A[dr], ans);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Not_b[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    ans = mem.PeekB(reg.A[dr]);
                    ans = (byte)~ans;
                    mem.PokeB(reg.A[dr], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Not_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    ans = mem.PeekB((UInt32)(reg.A[dr] + d16));
                    ans = (byte)~ans;
                    mem.PokeB((UInt32)(reg.A[dr] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Not_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    ans = mem.PeekB(ptr);
                    ans = (byte)~ans;
                    mem.PokeB(ptr, ans);
                    cycle = cy.Not_b[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            ans = mem.PeekB(ptr);
                            ans = (byte)~ans;
                            mem.PokeB(ptr, ans);
                            cycle = cy.Not_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            ans = mem.PeekB(ptr);
                            ans = (byte)~ans;
                            mem.PokeB(ptr, ans);
                            cycle = cy.Not_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = false;
            reg.Z = true;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cnot_w(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cnot_l(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cclr(UInt16 n)
        {
            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0x0:
                    return Cclr_b(n);
                case 0x1:
                    return Cclr_w(n);
                case 0x2:
                    return Cclr_l(n);
            }

            throw new ArgumentOutOfRangeException("CLRが扱える範囲エラー");
        }

        private int Cclr_b(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "CLR.b ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.SetDb(dr, 0);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Clr_b[0];
                    break;
                case 2://(An)
                    mem.PokeB(reg.A[dr], 0);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Clr_b[1];
                    break;
                case 3://(An)+
                    mem.PokeB(reg.A[dr], 0);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Clr_b[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    mem.PokeB(reg.A[dr], 0);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Clr_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeB((UInt32)(reg.A[dr] + d16), 0);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Clr_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeB(ptr, 0);
                    cycle = cy.Clr_b[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            mem.PokeB(ptr, 0);
                            cycle = cy.Clr_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            mem.PokeB(ptr, 0);
                            cycle = cy.Clr_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = false;
            reg.Z = true;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cclr_w(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "CLR.w ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.SetDw(dr, 0);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Clr_w[0];
                    break;
                case 2://(An)
                    mem.PokeW(reg.A[dr], 0);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Clr_w[1];
                    break;
                case 3://(An)+
                    mem.PokeW(reg.A[dr], 0);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Clr_w[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    mem.PokeW(reg.A[dr], 0);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Clr_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeW((UInt32)(reg.A[dr] + d16), 0);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Clr_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeW(ptr, 0);
                    cycle = cy.Clr_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            mem.PokeW(ptr, 0);
                            cycle = cy.Clr_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            mem.PokeW(ptr, 0);
                            cycle = cy.Clr_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = false;
            reg.Z = true;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cclr_l(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "CLR.l ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.D[dr] = 0;
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Clr_l[0];
                    break;
                case 2://(An)
                    mem.PokeL(reg.A[dr], 0);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Clr_l[1];
                    break;
                case 3://(An)+
                    mem.PokeL(reg.A[dr], 0);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Clr_l[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    mem.PokeL(reg.A[dr], 0);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Clr_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeL((UInt32)(reg.A[dr] + d16), 0);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Clr_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeL(ptr, 0);
                    cycle = cy.Clr_l[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            mem.PokeL(ptr, 0);
                            cycle = cy.Clr_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            mem.PokeL(ptr, 0);
                            cycle = cy.Clr_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X
            reg.N = false;
            reg.Z = true;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmoveccr(UInt16 n)
        {
            if ((n & 0xc0) != 0xc0)
            {
                return Cneg(n);
            }

            string nimo = "";
#if DEBUG_M68
            nimo = "MOVE.w ";
#endif


            int cycle = 0;

            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int16 val = (Int16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",ccr";
#endif

            cycle = cy.MoveToCcr_w[cycle];

            reg.CCR = (byte)val;
#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            return cycle;
        }

        private int Cpea(UInt16 n)
        {
            if ((n & 0xfff8) == 0x4840)
            {
                return Cswap(n);
            }

            if ((n & 0xfe38) == 0x4800)
            {
                return Cext(n);
            }

            if ((n & 0xc0) != 0x40)
            {
                //PEA以外の命令っぽい

                if ((n & 0x0b80) == 0x0880)
                {
                    return Cmovem(n);
                }

                throw new NotImplementedException(string.Format("未実装!! [{0:x04}]", n));
            }

            string nimo = "";
#if DEBUG_M68
            nimo = "PEA.l ";
#endif

            int cycle = 0;

            //src
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);
            UInt32 val = srcAddressingLongLea(ref nimo, ref cycle, sm, sr);


            //compute
            Push(val);

            //flag
            //none

            //cycle
            cycle = cy.Pea_l[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cswap(UInt16 n)
        {
            int r = n & 0x0007;
#if DEBUG_M68
            string nimo = string.Format("SWAP D{0}", r);
#endif


            //compute
            UInt32 a = reg.GetDl(r);
            a = (UInt32)((a << 16) | (a >> 16));
            reg.SetDl(r, a);

            //flag
            //X変化なし
            reg.N = (a & 0x8000_0000) != 0;
            reg.Z = (a == 0);
            reg.V = false;
            reg.C = false;

            //cycle
            int cycle = 4;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cext(UInt16 n)
        {
            int r = n & 0x0007;
            int op = (n & 0x01c0) >> 6;
#if DEBUG_M68
            string nimo = string.Format("EXT.{0} D{1}",op==2?"w":"l", r);
#endif


            UInt32 a;
            //compute
            switch (op)
            {
                case 2:
                    a = reg.GetDb(r);
                    a = (uint)(Int32)(sbyte)a;
                    reg.SetDw(r, (ushort)a);
                    break;
                case 3:
                    a = reg.GetDw(r);
                    a = (uint)(Int32)(short)a;
                    reg.SetDl(r, a);
                    break;
                default:
                    throw new NotImplementedException();
            }

            //flag
            //X変化なし
            reg.N = (a & 0x8000_0000) != 0;
            reg.Z = (a == 0);
            reg.V = false;
            reg.C = false;

            //cycle
            int cycle = 4;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmovem(UInt16 n)
        {
            bool r2m = ((n & 0x0400) == 0);
            if (r2m) return CmovemFromReg(n);
            else return CmovemToReg(n);
        }

        private int CmovemFromReg(UInt16 n)
        {
            bool wordTrns = ((n & 0x0040) == 0);
            if (wordTrns) return CmovemFromReg_w(n);
            else return CmovemFromReg_l(n);
        }

        private int CmovemFromReg_w(UInt16 n)
        {
            int cycle = 0;
            int cyc = 0;

#if DEBUG_M68
            string nimo = "MOVEM.w ";
#endif


            //dst
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw = 0;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr = 0;

            UInt16 rl = FetchW();
#if DEBUG_M68
            string dnimo = "";
#endif

            bool ff = false;
            Int16 d16 = 0;
            int i = 15;
            int shift = 0;

            for (UInt16 rb = 0x0001; i >= 0; rb <<= 1, i--)
            {
                if ((rl & rb) == 0) continue;
                UInt32 val;
                if (rb > 0xff)
                {
                    if (dm == 4)
                    {
                        //プリデクリメントモード
                        val = reg.D[i % 8];
#if DEBUG_M68
                        nimo += string.Format("D{0}", i % 8);
#endif

                    }
                    else
                    {
                        val = reg.A[7 - (i % 8)];
#if DEBUG_M68
                        nimo += string.Format("A{0}", 7 - (i % 8));
#endif

                    }
                }
                else
                {
                    if (dm == 4)
                    {
                        //プリデクリメントモード
                        val = reg.A[i % 8];
#if DEBUG_M68
                        nimo += string.Format("A{0}", i % 8);
#endif

                    }
                    else
                    {
                        val = reg.D[7 - (i % 8)];
#if DEBUG_M68
                        nimo += string.Format("D{0}", 7 - (i % 8));
#endif

                    }
                }

                //dst
                switch (dm)
                {
                    case 2://(An)
                        mem.PokeW((UInt32)(reg.A[dr] + shift), (UInt16)val);
#if DEBUG_M68
                        dnimo = string.Format("(A{0})", dr);
#endif

                        cyc = 4;
                        cycle += cy.MovemFromReg_w[0];
                        break;
                    case 4://-(An)
                        reg.A[dr] -= 2;
                        mem.PokeW(reg.A[dr], (UInt16)val);
#if DEBUG_M68
                        dnimo = string.Format("-(A{0})", dr);
#endif

                        cyc = 4;
                        cycle += cy.MovemFromReg_w[1];
                        break;
                    case 5://d16(An)
                        if (!ff)
                        {
                            d16 = (Int16)FetchW();
                            ff = true;
                        }
                        mem.PokeW((UInt32)(reg.A[dr] + d16 + shift), (UInt16)val);
#if DEBUG_M68
                        dnimo = string.Format("${0:x04}(A{1})", d16, dr);
#endif

                        cyc = 6;
                        cycle += cy.MovemFromReg_w[2];
                        break;
                    case 6://d8(An,IX)
                        if (!ff)
                        {
                            vw = FetchW();
                            ff = true;
                        }
                        isA = (vw & 0x8000) != 0;
                        ni = (vw & 0x7000) >> 12;
                        isL = (vw & 0x0800) != 0;
                        IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                        dnimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                        if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                        else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                        mem.PokeW((UInt32)(ptr + shift), (UInt16)val);
                        cyc = 6;
                        cycle += cy.MovemFromReg_w[3];
                        break;
                    case 7://etc.
                        switch (dr)
                        {
                            case 0://Abs.W
                                if (!ff)
                                {
                                    ptr = (UInt32)(Int16)FetchW();
                                    ff = true;
                                }
#if DEBUG_M68
                                dnimo = string.Format("${0:x04}", (Int16)ptr);
#endif

                                mem.PokeW((UInt32)(ptr + shift), (UInt16)val);
                                cyc = 6;
                                cycle += cy.MovemFromReg_w[4];
                                break;
                            case 1://Abs.L
                                if (!ff)
                                {
                                    ptr = FetchL();
                                    ff = true;
                                }
#if DEBUG_M68
                                dnimo = string.Format("${0:x08}", (Int32)ptr);
#endif

                                mem.PokeW((UInt32)(ptr + shift), (UInt16)val);
                                cyc = 8;
                                cycle += cy.MovemFromReg_w[5];
                                break;
                        }
                        break;
                }

                shift += 2;

            }

#if DEBUG_M68
            nimo += " , " + dnimo;
#endif

            cycle += cyc;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CmovemFromReg_l(UInt16 n)
        {
            int cycle = 0;
            int cyc = 0;

#if DEBUG_M68
            string nimo = "MOVEM ";
#endif


            //dst
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw = 0;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr = 0;

            UInt16 rl = FetchW();
#if DEBUG_M68
            string dnimo = "";
#endif

            bool ff = false;
            Int16 d16 = 0;
            int i = 15;
            int shift = 0;

            for (UInt16 rb = 0x0001; i >= 0; rb <<= 1, i--)
            {
                if ((rl & rb) == 0) continue;
                UInt32 val;
                if (rb > 0xff)
                {
                    if (dm == 4)
                    {
                        //プリデクリメントモード
                        val = reg.D[i % 8];
#if DEBUG_M68
                        nimo += string.Format("D{0}", i % 8);
#endif

                    }
                    else
                    {
                        val = reg.A[7 - (i % 8)];
#if DEBUG_M68
                        nimo += string.Format("A{0}", 7 - (i % 8));
#endif

                    }
                }
                else
                {
                    if (dm == 4)
                    {
                        //プリデクリメントモード
                        val = reg.A[i % 8];
#if DEBUG_M68
                        nimo += string.Format("A{0}", i % 8);
#endif

                    }
                    else
                    {
                        val = reg.D[7 - (i % 8)];
#if DEBUG_M68
                        nimo += string.Format("D{0}", 7 - (i % 8));
#endif

                    }
                }

                //dst
                switch (dm)
                {
                    case 2://(An)
                        mem.PokeL((UInt32)(reg.A[dr] + shift), val);
#if DEBUG_M68
                        dnimo = string.Format("(A{0})", dr);
#endif

                        cyc = 4;
                        cycle += cy.MovemFromReg_l[0];
                        break;
                    case 4://-(An)
                        reg.A[dr] -= 4;
                        mem.PokeL(reg.A[dr], val);
#if DEBUG_M68
                        dnimo = string.Format("-(A{0})", dr);
#endif

                        cyc = 4;
                        cycle += cy.MovemFromReg_l[1];
                        break;
                    case 5://d16(An)
                        if (!ff)
                        {
                            d16 = (Int16)FetchW();
                            ff = true;
                        }
                        mem.PokeL((UInt32)(reg.A[dr] + d16 + shift), val);
#if DEBUG_M68
                        dnimo = string.Format("${0:x04}(A{1})", d16, dr);
#endif

                        cyc = 6;
                        cycle += cy.MovemFromReg_l[2];
                        break;
                    case 6://d8(An,IX)
                        if (!ff)
                        {
                            vw = FetchW();
                            ff = true;
                        }
                        isA = (vw & 0x8000) != 0;
                        ni = (vw & 0x7000) >> 12;
                        isL = (vw & 0x0800) != 0;
                        IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                        dnimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                        if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                        else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                        mem.PokeL((UInt32)(ptr + shift), val);
                        cyc = 6;
                        cycle += cy.MovemFromReg_l[3];
                        break;
                    case 7://etc.
                        switch (dr)
                        {
                            case 0://Abs.W
                                if (!ff)
                                {
                                    ptr = (UInt32)(Int16)FetchW();
                                    ff = true;
                                }
#if DEBUG_M68
                                dnimo = string.Format("${0:x04}", (Int16)ptr);
#endif

                                mem.PokeL((UInt32)(ptr + shift), val);
                                cyc = 6;
                                cycle += cy.MovemFromReg_l[4];
                                break;
                            case 1://Abs.L
                                if (!ff)
                                {
                                    ptr = FetchL();
                                    ff = true;
                                }
#if DEBUG_M68
                                dnimo = string.Format("${0:x08}", (Int32)ptr);
#endif

                                mem.PokeL((UInt32)(ptr + shift), val);
                                cyc = 8;
                                cycle += cy.MovemFromReg_l[5];
                                break;
                        }
                        break;
                }

                shift += 4;
            }

#if DEBUG_M68
            nimo += " , " + dnimo;
#endif

            cycle += cyc;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CmovemToReg(UInt16 n)
        {
            bool wordTrns = ((n & 0x0040) == 0);
            if (wordTrns) return CmovemToReg_w(n);
            else return CmovemToReg_l(n);
        }

        private int CmovemToReg_w(UInt16 n)
        {
            throw new NotImplementedException();
        }

        private int CmovemToReg_l(UInt16 n)
        {
            int cycle = 0;
            int cyc = 0;

            string nimo = "";
#if DEBUG_M68
            nimo = "MOVEM.l ";
#endif

            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            UInt16 rl = FetchW();
#if DEBUG_M68
            string dnimo = "";
#endif

            bool nimoSw = true;

            //int i = 0;

            //for (UInt16 rb = 0x8000; rb != 0; rb >>= 1, i++)
            int i = 15;
            int shift = 0;

            for (UInt16 rb = 0x0001; i >= 0; rb <<= 1, i--)
            {
                if ((rl & rb) == 0) continue;

                //src
                UInt32 val = srcAddressingLong(ref nimo, ref cycle, sm, sr, 0xfff, nimoSw, shift);

                shift += 4;
                if (sm > 4)
                {
                    reg.PC -= 2;
                    if (sm == 7 && (sr == 1 || sr == 4)) reg.PC -= 2;
                }
#if DEBUG_M68
                nimoSw = false;
#endif


                if (rb > 0xff)
                {
                    //dst
                    reg.A[7 - (i % 8)] = val;
#if DEBUG_M68
                    dnimo += string.Format("A{0}", 7 - (i % 8));
#endif

                    cyc += cy.MovemToReg_l1[cycle];
                }
                else
                {
                    //dst
                    reg.D[7 - (i % 8)] = val;
#if DEBUG_M68
                    dnimo += string.Format("D{0}", 7 - (i % 8));
#endif

                    cyc += cy.MovemToReg_l1[cycle];
                }

            }

            if (shift != 0)
            {
                if (sm > 4)
                {
                    reg.PC += 2;
                    if (sm == 7 && (sr == 1 || sr == 4)) reg.PC += 2;
                }
            }

#if DEBUG_M68
            nimo += "," + dnimo;
#endif

            cycle = cy.MovemToReg_l0[cycle] + cyc;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;

        }

        private int Cneg(UInt16 n)
        {
            int size = (n & 0x00c0) >> 6;

            switch (size)
            {
                case 0://byte
                    return Cnegb(n);
                case 1://word
                    return Cnegw(n);
                case 2://long
                    return Cnegl(n);
            }

            throw new NotImplementedException("dummy");

        }

        private int Cnegb(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "NEG.b ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            byte val = 0;
            byte ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.GetDb(dr);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    ans = (byte)-(sbyte)val;
                    reg.SetDb(dr, ans);
                    cycle = cy.Neg_b[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    val = mem.PeekB(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    ans = (byte)-(sbyte)val;
                    mem.PokeB(reg.A[dr], ans);
                    cycle = cy.Neg_b[1];
                    break;
                case 3://(An)+
                    val = mem.PeekB(reg.A[dr]);
                    ans = (byte)-(sbyte)val;
                    mem.PokeB(reg.A[dr], ans);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Neg_b[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    val = mem.PeekB(reg.A[dr]);
                    ans = (byte)-(sbyte)val;
                    mem.PokeB(reg.A[dr], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Neg_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekB((UInt32)(reg.A[dr] + d16));
                    ans = (byte)-(sbyte)val;
                    mem.PokeB((UInt32)(reg.A[dr] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Neg_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekB(ptr);
                    ans = (byte)-(sbyte)val;
                    mem.PokeB(ptr, ans);
                    cycle = cy.Neg_b[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekB(ptr);
                            ans = (byte)-(sbyte)val;
                            mem.PokeB(ptr, ans);
                            cycle = cy.Neg_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekB(ptr);
                            ans = (byte)-(sbyte)val;
                            mem.PokeB(ptr, ans);
                            cycle = cy.Neg_b[7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVneg(val, ans);
            reg.SetCneg(val, ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cnegw(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "NEG.w ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt16 val = 0;
            UInt16 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.GetDw(dr);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    ans = (ushort)-(short)val;
                    reg.SetDw(dr, ans);
                    cycle = cy.Neg_w[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    val = mem.PeekW(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    ans = (ushort)-(short)val;
                    mem.PokeW(reg.A[dr], ans);
                    cycle = cy.Neg_w[1];
                    break;
                case 3://(An)+
                    val = mem.PeekW(reg.A[dr]);
                    ans = (ushort)-(short)val;
                    mem.PokeW(reg.A[dr], ans);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Neg_w[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    val = mem.PeekW(reg.A[dr]);
                    ans = (ushort)-(short)val;
                    mem.PokeW(reg.A[dr], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Neg_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekW((UInt32)(reg.A[dr] + d16));
                    ans = (ushort)-(short)val;
                    mem.PokeW((UInt32)(reg.A[dr] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Neg_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekW(ptr);
                    ans = (ushort)-(short)val;
                    mem.PokeW(ptr, ans);
                    cycle = cy.Neg_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekW(ptr);
                            ans = (ushort)-(short)val;
                            mem.PokeW(ptr, ans);
                            cycle = cy.Neg_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekW(ptr);
                            ans = (ushort)-(short)val;
                            mem.PokeW(ptr, ans);
                            cycle = cy.Neg_w[7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVneg(val, ans);
            reg.SetCneg(val, ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cnegl(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "NEG.l ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.D[dr];
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    reg.SetDl(dr, (uint)-val);
                    cycle = cy.Neg_l[0];
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2://(An)
                    val = mem.PeekL(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    mem.PokeL(reg.A[dr], (uint)-val);
                    cycle = cy.Neg_l[1];
                    break;
                case 3://(An)+
                    val = mem.PeekL(reg.A[dr]);
                    mem.PokeL(reg.A[dr], (uint)-val);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Neg_l[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    val = mem.PeekL(reg.A[dr]);
                    mem.PokeL(reg.A[dr], (uint)-val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Neg_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekL((UInt32)(reg.A[dr] + d16));
                    mem.PokeL((UInt32)(reg.A[dr] + d16), (uint)-val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Neg_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekL(ptr);
                    mem.PokeL(ptr, (uint)-val);
                    cycle = cy.Neg_l[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekL(ptr);
                            mem.PokeL(ptr, (uint)-val);
                            cycle = cy.Neg_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekL(ptr);
                            mem.PokeL(ptr, (uint)-val);
                            cycle = cy.Neg_l[7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN((uint)-val);
            reg.SetZ((uint)-val);
            reg.SetVneg(val, (uint)-val);
            reg.SetCneg(val, (uint)-val);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ctst(UInt16 n)
        {
            int size = (n & 0x00c0) >> 6;

            switch (size)
            {
                case 0://byte
                    return Ctstb(n);
                case 1://word
                    return Ctstw(n);
                case 2://long
                    return Ctstl(n);
            }

            if ((n & 0x00c0) == 0x00c0)
            {
                return Ctas(n);
            }

            throw new NotImplementedException("dummy");

        }

        private int Ctstb(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "TST.b ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.D[dr];
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Tst_b[0];
                    break;
                case 2://(An)
                    val = mem.PeekB(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Tst_b[1];
                    break;
                case 3://(An)+
                    val = mem.PeekB(reg.A[dr]);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr]++;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Tst_b[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    val = mem.PeekB(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Tst_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekB((UInt32)(reg.A[dr] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Tst_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekB(ptr);
                    cycle = cy.Tst_b[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekB(ptr);
                            cycle = cy.Tst_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekB(ptr);
                            cycle = cy.Tst_b[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X -
            reg.SetN((byte)val);
            reg.SetZ((byte)val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ctstw(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "TST.w ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.D[dr];
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Tst_w[0];
                    break;
                case 2://(An)
                    val = mem.PeekW(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Tst_w[1];
                    break;
                case 3://(An)+
                    val = mem.PeekW(reg.A[dr]);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Tst_w[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    val = mem.PeekW(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Tst_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekW((UInt32)(reg.A[dr] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Tst_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekW(ptr);
                    cycle = cy.Tst_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekW(ptr);
                            cycle = cy.Tst_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekW(ptr);
                            cycle = cy.Tst_w[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X -
            reg.SetN((ushort)val);
            reg.SetZ((ushort)val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ctstl(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "TST.l ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.D[dr];
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Tst_l[0];
                    break;
                case 2://(An)
                    val = mem.PeekL(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Tst_l[1];
                    break;
                case 3://(An)+
                    val = mem.PeekL(reg.A[dr]);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Tst_l[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    val = mem.PeekL(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Tst_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekL((UInt32)(reg.A[dr] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Tst_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekL(ptr);
                    cycle = cy.Tst_l[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekL(ptr);
                            cycle = cy.Tst_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekL(ptr);
                            cycle = cy.Tst_l[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X -
            reg.SetN(val);
            reg.SetZ(val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ctas(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "TAS.b ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    val = reg.GetDb(dr);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Tas[0];
                    reg.SetDb(dr, (byte)(val | 0x80));
                    break;
                case 1:
                    throw new ArgumentOutOfRangeException();
                case 2://(An)
                    val = mem.PeekB(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Tas[1];
                    mem.PokeB(reg.A[dr], (byte)(val | 0x80));
                    break;
                case 3://(An)+
                    val = mem.PeekB(reg.A[dr]);
                    mem.PokeB(reg.A[dr], (byte)(val | 0x80));
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr] += 1;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Tas[2];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    val = mem.PeekB(reg.A[dr]);
                    mem.PokeB(reg.A[dr], (byte)(val | 0x80));
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Tas[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    val = mem.PeekB((UInt32)(reg.A[dr] + d16));
                    mem.PokeB((UInt32)(reg.A[dr] + d16), (byte)(val | 0x80));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Tas[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekB(ptr);
                    mem.PokeB(ptr, (byte)(val | 0x80));
                    cycle = cy.Tas[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = mem.PeekB(ptr);
                            mem.PokeB(ptr, (byte)(val | 0x80));
                            cycle = cy.Tas[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = mem.PeekB(ptr);
                            mem.PokeB(ptr, (byte)(val | 0x80));
                            cycle = cy.Tas[7];
                            break;
                    }
                    break;
            }

            //flag
            //reg.X -
            reg.SetN((byte)val);
            reg.SetZ((byte)val);
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Crts(UInt16 n)
        {
            if ((n & 0xfff0) == 0x4e40)
            {
                return Ctrap(n);
            }

            if ((n & 0xffc0) == 0x4e80)
            {
                return Cjsr(n);
            }

            if ((n & 0xffc0) == 0x4ec0)
            {
                return Cjmp(n);
            }

            if ((n & 0xfff8) == 0x4e50)
            {
                return Clink(n);
            }

            if ((n & 0xfff8) == 0x4e58)
            {
                return Cunlk(n);
            }

            if (n == 0x4e71)
            {
                return Cnop(n);
            }

            if (n == 0x4e73)
            {
                return Crte(n);
            }

            if (n != 0x4e75)
            {
                throw new NotImplementedException("未実装!");
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, "RTS");
#endif
            reg.PC = Pop();
            return 16;
        }

        private int Crte(UInt16 n)
        {
            Log.WriteLine(LogLevel.Trace, "RTE");
            reg.SR = Popw();
            reg.PC = Pop();
            return 20;
        }

        public int Ctrap(UInt16 n)
        {
            UInt32 t = (UInt32)(n & 0xf);
#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace2, "TRAP #{0:X02}", t);
#endif
            t += 32;//vector32～
            t *= 4; //4byte
            reg.SRbk = reg.SR;
            reg.S = true;
            reg.T = false;
            PushSSPw((UInt16)reg.PC);
            PushSSPw((UInt16)(reg.PC >> 16));
            PushSSPw(reg.SRbk);
            reg.PC = mem.PeekL(t);

            return 34;//cycle
        }

        public int Ctrap2(UInt16 n)
        {
            UInt32 t = (UInt32)(n & 0xff);
#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace2, "TRAP #{0:X02}", t);
#endif
            t *= 4; //4byte
            reg.SRbk = reg.SR;
            reg.S = true;
            reg.T = false;
            PushSSPw((UInt16)reg.PC);
            PushSSPw((UInt16)(reg.PC >> 16));
            PushSSPw(reg.SRbk);
            reg.PC = mem.PeekL(t);

            return 34;//cycle
        }

        public int CtrapPtr(UInt32 n)
        {
#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace2, "TRAP ${0:X08}", n);
#endif
            reg.SRbk = reg.SR;
            reg.S = true;
            reg.T = false;
            PushSSPw((UInt16)reg.PC);
            PushSSPw((UInt16)(reg.PC >> 16));
            PushSSPw(reg.SRbk);
            reg.PC = n;

            return 34;//cycle
        }

        private int Cnop(UInt16 n)
        {
#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, "NOP");
#endif
            return 4;//cycle
        }

        private int Caqsqdbs(UInt16 n)
        {
            // 0b0101_xxxx_1100_1xxx -> DBcc
            // 0b0101_xxxx_11xx_xxxx -> Scc
            // 0b0101_xxx1_xxxx_xxxx -> SUBQ
            // 0b0101_xxx0_xxxx_xxxx -> ADDQ
            if ((n & 0xf0f8) == 0x50c8) return CDBcc(n);
            if ((n & 0xf0c0) == 0x50c0) return CScc(n);
            if ((n & 0xf100) == 0x5100) return Csubq(n);
            if ((n & 0xf100) == 0x5000) return Caddq(n);

            throw new NotImplementedException();
        }

        private int CDBcc(UInt16 n)
        {
            int cnd = (n & 0x0f00) >> 8;
            int dr = (n & 0x7);
            int cycle = 10;
#if DEBUG_M68
            string nimo = "DB{0}.w D{1},#${2:x04}";
#endif

            bool v = getCond(cnd, out string cs);
            short ptr = (short)FetchW();
#if DEBUG_M68
            nimo = string.Format(nimo, cs == "f" ? "ra" : cs, dr, ptr);
#endif


            if (!v)
            {
                ushort d = reg.GetDw(dr);
                d--;
                reg.SetDw(dr, d);
                if (d != 0xffff)
                {
                    reg.PC = (uint)(reg.PC + (int)(ptr - 2));
                }
                else
                {
                    cycle = 14;
                }
            }
            else
            {
                cycle = 12;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CScc(UInt16 n)
        {
            int cycle;
#if DEBUG_M68
            string nimo = "S{0}.b ";
#endif

            int cond = (n & 0x0f00) >> 8;
            bool b = false;
            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            byte val = 0;

            b = getCond(cond, out string cs);
#if DEBUG_M68
            nimo = string.Format(nimo, cs);
#endif

            cycle = cy.Scc_b[dm];
            if (!b)
            {
                if (dm == 0) cycle = 4;
            }
            else
                val = 0xff;

            //dst
            switch (dm)
            {
                case 0://Dn
                    reg.SetDb(dr, val);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    break;
                case 2://(An)
                    mem.PokeB(reg.A[dr], val);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    break;
                case 3://(An)+
                    mem.PokeB(reg.A[dr], val);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr] += 1;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    mem.PokeB(reg.A[dr], val);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    mem.PokeB((UInt32)(reg.A[dr] + d16), val);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    mem.PokeB(ptr, val);
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            mem.PokeB(ptr, val);
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            mem.PokeB(ptr, val);
                            break;
                    }
                    break;
            }

            //flag
            //変化なし

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddq(UInt16 n)
        {
            int data = (n & 0x0e00) >> 9;
            int size = (n & 0x00c0) >> 6;

            if (data == 0) data = 8;

            switch (size)
            {
                case 0://byte
                    return Caddqb(n, data);
                case 1://word
                    return Caddqw(n, data);
                case 2://long
                    return Caddql(n, data);
            }

            throw new NotImplementedException("dummy");
        }

        private int Caddqb(UInt16 n, int data)
        {
#if DEBUG_M68
            string nimo = string.Format("ADDQ.b #{0:x},", data);
#endif


            int cycle = 0;

            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            Int32 src = 0;
            Int32 dst = (Int32)data;
            Int32 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (sbyte)reg.GetDb(dr);
                    ans = src + dst;
                    reg.SetDb(dr, (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Addq_b[0];
                    break;
                case 1://An
                    throw new ArgumentOutOfRangeException();
                case 2://(An)
                    src = (sbyte)mem.PeekB(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeB(reg.A[dr], (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Addq_b[2];
                    break;
                case 3://(An)+
                    src = (sbyte)mem.PeekB(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeB(reg.A[dr], (byte)ans);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr] += 1;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Addq_b[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    src = (sbyte)mem.PeekB(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeB(reg.A[dr], (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Addq_b[4];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = (sbyte)mem.PeekB((UInt32)(reg.A[dr] + d16));
                    ans = src + dst;
                    mem.PokeB((UInt32)(reg.A[dr] + d16), (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Addq_b[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (sbyte)mem.PeekB(ptr);
                    ans = src + dst;
                    mem.PokeB(ptr, (byte)ans);
                    cycle = cy.Addq_b[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            src = (sbyte)mem.PeekB(ptr);
                            ans = src + dst;
                            mem.PokeB(ptr, (byte)ans);
                            cycle = cy.Addq_b[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            src = (sbyte)mem.PeekB(ptr);
                            ans = src + dst;
                            mem.PokeB(ptr, (byte)ans);
                            cycle = cy.Addq_b[8];
                            break;
                    }
                    break;
            }

            //flag
            if (dm != 1) // Anの場合はCCRに影響を与えない!!
            {
                reg.SetN((byte)ans);
                reg.SetZ((byte)ans);
                reg.SetV((byte)src, (byte)ans);
                reg.SetC((ushort)src, (ushort)ans);
                reg.X = reg.C;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddqw(UInt16 n, int data)
        {
#if DEBUG_M68
            string nimo = string.Format("ADDQ.w #{0:x},", data);
#endif


            int cycle = 0;

            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            Int32 src = 0;
            Int32 dst = (Int32)data;
            Int32 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (short)reg.GetDw(dr);
                    ans = src + dst;
                    reg.SetDw(dr, (ushort)ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Addq_w[0];
                    break;
                case 1://An
                    src = (int)reg.GetAl(dr);//An の場合は32bit演算
                    ans = src + dst;
                    reg.SetAl(dr, (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("A{0}", dr);
#endif

                    cycle = cy.Addq_w[1];
                    break;
                case 2://(An)
                    src = (Int16)mem.PeekW(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeW(reg.A[dr], (UInt16)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Addq_w[2];
                    break;
                case 3://(An)+
                    src = (Int16)mem.PeekW(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeW(reg.A[dr], (UInt16)ans);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Addq_w[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    src = (Int16)mem.PeekW(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeW(reg.A[dr], (UInt16)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Addq_w[4];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = (Int16)mem.PeekW((UInt32)(reg.A[dr] + d16));
                    ans = src + dst;
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (UInt16)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Addq_w[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (Int16)mem.PeekW(ptr);
                    ans = src + dst;
                    mem.PokeW(ptr, (UInt16)ans);
                    cycle = cy.Addq_w[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            src = (Int16)mem.PeekW(ptr);
                            ans = src + dst;
                            mem.PokeW(ptr, (UInt16)ans);
                            cycle = cy.Addq_w[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            src = (Int16)mem.PeekW(ptr);
                            ans = src + dst;
                            mem.PokeW(ptr, (UInt16)ans);
                            cycle = cy.Addq_w[8];
                            break;
                    }
                    break;
            }

            //flag
            if (dm != 1) // Anの場合はCCRに影響を与えない!!
            {
                reg.SetN((ushort)ans);
                reg.SetZ((ushort)ans);
                reg.SetV((ushort)src, (ushort)ans);
                reg.SetC((ushort)src, (ushort)ans);
                reg.X = reg.C;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddql(UInt16 n, int data)
        {
#if DEBUG_M68
            string nimo = string.Format("ADDQ.l #{0:x},", data);
#endif


            int cycle = 0;

            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            Int32 src = 0;
            Int32 dst = (Int32)data;
            Int32 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (int)reg.GetDl(dr);
                    ans = src + dst;
                    reg.SetDl(dr, (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Addq_l[0];
                    break;
                case 1://An
                    src = (int)reg.GetAl(dr);//An の場合は32bit演算
                    ans = src + dst;
                    reg.SetAl(dr, (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("A{0}", dr);
#endif

                    cycle = cy.Addq_l[1];
                    break;
                case 2://(An)
                    src = (int)mem.PeekL(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeL(reg.A[dr], (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Addq_l[2];
                    break;
                case 3://(An)+
                    src = (int)mem.PeekL(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeL(reg.A[dr], (uint)ans);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Addq_l[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    src = (int)mem.PeekL(reg.A[dr]);
                    ans = src + dst;
                    mem.PokeL(reg.A[dr], (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Addq_l[4];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = (int)mem.PeekL((UInt32)(reg.A[dr] + d16));
                    ans = src + dst;
                    mem.PokeL((UInt32)(reg.A[dr] + d16), (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Addq_l[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (int)mem.PeekL(ptr);
                    ans = src + dst;
                    mem.PokeL(ptr, (uint)ans);
                    cycle = cy.Addq_l[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            src = (int)mem.PeekL(ptr);
                            ans = src + dst;
                            mem.PokeL(ptr, (uint)ans);
                            cycle = cy.Addq_l[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            src = (int)mem.PeekL(ptr);
                            ans = src + dst;
                            mem.PokeL(ptr, (uint)ans);
                            cycle = cy.Addq_l[8];
                            break;
                    }
                    break;
            }

            //flag
            if (dm != 1) // Anの場合はCCRに影響を与えない!!
            {
                reg.SetN((uint)ans);
                reg.SetZ((uint)ans);
                reg.SetV((uint)src, (uint)ans);
                reg.SetC((uint)src, (uint)ans);
                reg.X = reg.C;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubq(UInt16 n)
        {

            int size = (n & 0x00c0) >> 6;
            switch (size)
            {
                case 0:
                    return Csubqb(n);
                case 1:
                    return Csubqw(n);
                case 2:
                    return Csubql(n);
            }

            throw new NotImplementedException();
        }

        private int Csubqb(UInt16 n)
        {
            int imm = (n & 0x0e00) >> 9;
            if (imm == 0) imm = 8;
#if DEBUG_M68
            string nimo = string.Format("SUBQ.b #{0:x},", imm);
#endif

            int cycle = 0;
            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            Int32 src = 0;
            Int32 dst = (Int32)imm;
            Int32 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (sbyte)reg.GetDb(dr);
                    ans = src - dst;
                    reg.SetDb(dr, (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Subq_b[0];
                    break;
                case 1://An
                    throw new NotImplementedException();
                case 2://(An)
                    src = (sbyte)mem.PeekB(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeB(reg.A[dr], (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Subq_b[2];
                    break;
                case 3://(An)+
                    src = (sbyte)mem.PeekB(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeB(reg.A[dr], (byte)ans);
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr] += 1;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Subq_b[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr]--;
                    src = (sbyte)mem.PeekB(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeB(reg.A[dr], (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Subq_b[4];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = (sbyte)mem.PeekB((UInt32)(reg.A[dr] + d16));
                    ans = src - dst;
                    mem.PokeB((UInt32)(reg.A[dr] + d16), (byte)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Subq_b[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (sbyte)mem.PeekB(ptr);
                    ans = src - dst;
                    mem.PokeB(ptr, (byte)ans);
                    cycle = cy.Subq_b[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            src = (sbyte)mem.PeekB(ptr);
                            ans = src - dst;
                            mem.PokeB(ptr, (byte)ans);
                            cycle = cy.Subq_b[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            src = (sbyte)mem.PeekB(ptr);
                            ans = src - dst;
                            mem.PokeB(ptr, (byte)ans);
                            cycle = cy.Subq_b[8];
                            break;
                    }
                    break;
            }

            //flag
            if (dm != 1) // Anの場合はCCRに影響を与えない!!
            {
                reg.SetN((byte)ans);
                reg.SetZ((byte)ans);
                reg.SetV((byte)src, (byte)ans);
                reg.SetCcmp((byte)src, (byte)dst, (byte)ans);
                reg.X = reg.C;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubqw(UInt16 n)
        {
            int imm = (n & 0x0e00) >> 9;
            if (imm == 0) imm = 8;
#if DEBUG_M68
            string nimo = string.Format("SUBQ.w #{0:x},", imm);
#endif

            int cycle = 0;
            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            Int32 src = 0;
            Int32 dst = (Int32)imm;
            Int32 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (short)reg.GetDw(dr);
                    ans = src - dst;
                    reg.SetDw(dr, (ushort)ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Subq_w[0];
                    break;
                case 1://An
                    src = (int)reg.GetAl(dr); // Anの場合は32bit演算が行われる!!
                    ans = src - dst;
                    reg.SetAl(dr, (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("A{0}", dr);
#endif

                    cycle = cy.Subq_w[1];
                    break;
                case 2://(An)
                    src = (Int16)mem.PeekW(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeW(reg.A[dr], (UInt16)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Subq_w[2];
                    break;
                case 3://(An)+
                    src = (Int16)mem.PeekW(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeW(reg.A[dr], (UInt16)ans);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Subq_w[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    src = (Int16)mem.PeekW(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeW(reg.A[dr], (UInt16)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Subq_w[4];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = (Int16)mem.PeekW((UInt32)(reg.A[dr] + d16));
                    ans = src - dst;
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (UInt16)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Subq_w[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (Int16)mem.PeekW(ptr);
                    ans = src - dst;
                    mem.PokeW(ptr, (UInt16)ans);
                    cycle = cy.Subq_w[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            src = (Int16)mem.PeekW(ptr);
                            ans = src - dst;
                            mem.PokeW(ptr, (UInt16)ans);
                            cycle = cy.Subq_w[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            src = (Int16)mem.PeekW(ptr);
                            ans = src - dst;
                            mem.PokeW(ptr, (UInt16)ans);
                            cycle = cy.Subq_w[8];
                            break;
                    }
                    break;
            }

            //flag
            if (dm != 1) // Anの場合はCCRに影響を与えない!!
            {
                reg.SetN((ushort)ans);
                reg.SetZ((ushort)ans);
                reg.SetV((ushort)src, (ushort)ans);
                reg.SetCcmp((ushort)src, (ushort)dst, (ushort)ans);
                reg.X = reg.C;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubql(UInt16 n)
        {
            int imm = (n & 0x0e00) >> 9;
            if (imm == 0) imm = 8;
#if DEBUG_M68
            string nimo = string.Format("SUBQ.l #{0:x},", imm);
#endif

            int cycle = 0;
            int dr = (n & 0x0007);
            int dm = (n & 0x0038) >> 3;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            Int32 src = 0;
            Int32 dst = (Int32)imm;
            Int32 ans = 0;

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (int)reg.GetDl(dr);
                    ans = src - dst;
                    reg.SetDl(dr, (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Subq_l[0];
                    break;
                case 1://An
                    src = (int)reg.GetAl(dr);
                    ans = src - dst;
                    reg.SetAl(dr, (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("A{0}", dr);
#endif

                    cycle = cy.Subq_l[1];
                    break;
                case 2://(An)
                    src = (int)mem.PeekL(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeL(reg.A[dr], (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Subq_l[2];
                    break;
                case 3://(An)+
                    src = (int)mem.PeekL(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeL(reg.A[dr], (uint)ans);
                    reg.A[dr] += 4;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Subq_l[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    src = (int)mem.PeekL(reg.A[dr]);
                    ans = src - dst;
                    mem.PokeL(reg.A[dr], (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Subq_l[4];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    src = (int)mem.PeekL((UInt32)(reg.A[dr] + d16));
                    ans = src - dst;
                    mem.PokeL((uint)(reg.A[dr] + d16), (uint)ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Subq_l[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (int)mem.PeekL(ptr);
                    ans = src - dst;
                    mem.PokeL(ptr, (uint)ans);
                    cycle = cy.Subq_l[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            src = (int)mem.PeekL(ptr);
                            ans = src - dst;
                            mem.PokeL(ptr, (uint)ans);
                            cycle = cy.Subq_l[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            src = (int)mem.PeekL(ptr);
                            ans = src - dst;
                            mem.PokeL(ptr, (uint)ans);
                            cycle = cy.Subq_l[8];
                            break;
                    }
                    break;
            }

            //flag
            if (dm != 1) // Anの場合はCCRに影響を与えない!!
            {
                reg.SetN((uint)ans);
                reg.SetZ((uint)ans);
                reg.SetV((uint)src, (uint)ans);
                reg.SetCcmp((uint)src, (uint)dst, (uint)ans);
                reg.X = reg.C;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cbsr(UInt16 n)
        {
            int cycle = 20;
            Int16 ptr = (sbyte)n;
            int size = 0;
            if (ptr == 0)
            {
                ptr = (Int16)FetchW();
                size = 2;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, "BSR ${0:x04} ; ptr+PC=${1:x08}", ptr, reg.PC + ptr - size);
#endif

            Push(reg.PC);
            reg.PC += (UInt32)(ptr - size);

            return cycle;
        }

        private int Cjsr(UInt16 n)
        {
            int cycle = 0;
#if DEBUG_M68
            string nimo = "JSR ";
#endif


            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 2://(An)
                    val = reg.A[dr];
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Jsr_l[0];
                    break;
                case 5://d16(An)
                    Int32 d16 = (Int32)(Int16)FetchW();
                    val = (UInt32)(reg.A[dr] + d16);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", (Int16)d16, dr);
#endif

                    cycle = cy.Jsr_l[1];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = ptr;
                    cycle = cy.Jsr_l[2];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = ptr;
                            cycle = cy.Jsr_l[3];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = ptr;
                            cycle = cy.Jsr_l[4];
                            break;
                        case 2:
                            Int32 ed16 = (Int32)(Int16)FetchW();
                            val = (UInt32)(reg.PC + ed16 - 2);
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", (Int16)ed16);
#endif

                            cycle = cy.Jsr_l[5];
                            break;
                        case 3:
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            val = ptr;
                            cycle = cy.Jsr_l[6];
                            break;
                    }
                    break;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            Push(reg.PC);
            reg.PC = val;

            return cycle;
        }

        private int Cjmp(UInt16 n)
        {
            int cycle = 0;
#if DEBUG_M68
            string nimo = "JMP ";
#endif


            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            //dst
            switch (dm)
            {
                case 0:
                case 1:
                case 3:
                case 4:
                    throw new NotImplementedException();
                case 2://(An)
                    val = reg.A[dr];
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Jmp[0];
                    break;
                case 5://d16(An)
                    Int32 d16 = (Int32)(Int16)FetchW();
                    val = (UInt32)(reg.A[dr] + d16);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", (Int16)d16, dr);
#endif

                    cycle = cy.Jmp[1];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    val = ptr;
                    cycle = cy.Jmp[2];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            val = ptr;
                            cycle = cy.Jmp[3];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            val = ptr;
                            cycle = cy.Jmp[4];
                            break;
                        case 2:
                            Int32 ed16 = (Int32)(Int16)FetchW();
                            val = (UInt32)(reg.PC + ed16 - 2);
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", (Int16)ed16);
#endif

                            cycle = cy.Jmp[5];
                            break;
                        case 3:
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            val = ptr;
                            cycle = cy.Jmp[6];
                            break;
                    }
                    break;
            }

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            reg.PC = val;

            return cycle;
        }

        private int Cbra(UInt16 n)
        {
            int cycle = 10;
            int cnd = (n & 0x0f00) >> 8;
            Int16 ptr = (sbyte)n;
            int size = 0;
            if ((byte)n == 0)
            {
                ptr = (Int16)(((Int16)FetchW()));
                cycle = 8;
                size = 2;
            }

            bool v = getCond(cnd, out string cs);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, "B{0} ${1:x04} ; ptr+PC=${2:x08}",
                cs == "t" ? "ra" : cs, ptr, (UInt32)(reg.PC + ptr - size));
#endif
            if (v) reg.PC = (UInt32)(reg.PC +ptr - size);

            return cycle;
        }

        private int Cmoveq(UInt16 n)
        {
            int dr = (n & 0x0e00) >> 9;
            sbyte val = (sbyte)(byte)n;

            reg.D[dr] = (UInt32)(Int32)val;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, "MOVEQ.l #${0:x02},D{1}", val, dr);
#endif

            reg.N = (reg.D[dr] & 0x8000_000) != 0;
            reg.Z = (reg.D[dr] == 0);
            reg.V = false;
            reg.C = false;

            return 4;
        }

        private int Csub(UInt16 n)
        {
            int opMode = (n & 0x01c0) >> 6;

            switch (opMode)
            {
                case 0x00:
                    return Csubb(n);
                case 0x01:
                    return Csubw(n);
                case 0x02:
                    return Csubl(n);
                case 0x03:
                    return Csubaw(n);
                case 0x04:
                    return CsubbDn(n);
                case 0x05:
                    return CsubwDn(n);
                case 0x06:
                    return CsublDn(n);
                case 0x07:
                    return Csubal(n);
                default:
                    //SUBA以外の命令っぽい
                    throw new NotImplementedException(string.Format("未実装!! [{0:x04}]", n));
            }
        }

        private int CsubbDn(ushort n)
        {
#if DEBUG_M68
            string nimo = "SUB.b ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 src = reg.GetDb(dr);
#if DEBUG_M68
            nimo += string.Format("D{0},", dr);
#endif


            UInt32 dst = 0;
            UInt32 ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            cycle = sm;
            switch (sm)
            {
                case 2://(An)
#if DEBUG_M68
                    nimo += string.Format("(A{0})", sr);
#endif

                    dst = (UInt32)(Int16)mem.PeekB(reg.A[sr]);
                    ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                    mem.PokeB(reg.A[sr], (byte)ans);
                    break;
                case 3://(An)+
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", sr);
#endif

                    dst = (UInt32)(Int16)mem.PeekB(reg.A[sr]);
                    ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                    mem.PokeB(reg.A[sr], (byte)ans);
                    reg.A[sr] += 1;
                    if (sr == 7) reg.A[sr] += 1;
                    break;
                case 4://-(An)
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 1;
                    if (sr == 7) reg.A[sr]--;
                    dst = (UInt32)(Int16)mem.PeekB(reg.A[sr]);
                    ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                    mem.PokeB(reg.A[sr], (byte)ans);
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    dst = (UInt32)(Int16)mem.PeekB((UInt32)(reg.A[sr] + d16));
                    ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                    mem.PokeB((UInt32)(reg.A[sr] + d16), (byte)ans);
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    dst = (UInt32)(Int16)mem.PeekB(ptr);
                    ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                    mem.PokeB(ptr, (byte)ans);
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            dst = (UInt32)(Int16)mem.PeekB(ptr);
                            ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            dst = (UInt32)(Int16)mem.PeekB(ptr);
                            ans = (UInt32)((Int32)(sbyte)(byte)dst - (Int32)(sbyte)(byte)src);
                            mem.PokeB(ptr, (byte)ans);
                            cycle = 8;
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN((byte)ans);
            reg.SetZ((byte)ans);
            reg.SetVcmp((byte)src, (byte)dst, (byte)ans);
            reg.SetCcmp((byte)src, (byte)dst, (byte)ans);
            reg.X = reg.C;

            //cycle
            cycle = cy.Sub_bDn[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CsubwDn(ushort n)
        {
#if DEBUG_M68
            string nimo = "SUB.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 src = reg.GetDw(dr);
#if DEBUG_M68
            nimo += string.Format("D{0},", dr);
#endif


            UInt32 dst = 0;
            UInt32 ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            cycle = sm;
            switch (sm)
            {
                case 2://(An)
#if DEBUG_M68
                    nimo += string.Format("(A{0})", sr);
#endif

                    dst = (UInt32)(Int16)mem.PeekW(reg.A[sr]);
                    ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                    mem.PokeW(reg.A[sr], (UInt16)ans);
                    break;
                case 3://(An)+
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", sr);
#endif

                    dst = (UInt32)(Int16)mem.PeekW(reg.A[sr]);
                    ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                    mem.PokeW(reg.A[sr], (UInt16)ans);
                    reg.A[sr] += 2;
                    break;
                case 4://-(An)
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 2;
                    dst = (UInt32)(Int16)mem.PeekW(reg.A[sr]);
                    ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                    mem.PokeW(reg.A[sr], (UInt16)ans);
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    dst = (UInt32)(Int16)mem.PeekW((UInt32)(reg.A[sr] + d16));
                    ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                    mem.PokeW((UInt32)(reg.A[sr] + d16), (UInt16)ans);
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    dst = (UInt32)(Int16)mem.PeekW(ptr);
                    ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                    mem.PokeW(ptr, (UInt16)ans);
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            dst = (UInt32)(Int16)mem.PeekW(ptr);
                            ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                            mem.PokeW(ptr, (UInt16)ans);
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            dst = (UInt32)(Int16)mem.PeekW(ptr);
                            ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
                            mem.PokeW(ptr, (UInt16)ans);
                            cycle = 8;
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN((UInt16)ans);
            reg.SetZ((UInt16)ans);
            reg.SetVcmp((UInt16)src, (UInt16)dst, (UInt16)ans);
            reg.SetCcmp((UInt16)src, (UInt16)dst, (UInt16)ans);
            reg.X = reg.C;

            //cycle
            cycle = cy.Sub_wDn[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int CsublDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Csubb(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "SUB.b ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 src = srcAddressingByte(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            UInt16 dst = reg.GetDb(dr);
#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif


            //compute
            UInt16 ans = (UInt16)((Int16)(byte)dst - (Int16)(byte)src);
            reg.SetDb(dr, (byte)ans);

            //flag
            reg.SetN((byte)ans);
            reg.SetZ((byte)ans);
            reg.SetVcmp((byte)src, (byte)dst, (byte)ans);
            reg.SetCcmp((byte)src, (byte)dst, (byte)ans);
            reg.X = reg.C;

            //cycle
            cycle = cy.Sub_b[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubw(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "SUB.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 src = srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            UInt32 dst = reg.GetDw(dr);
#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif


            //compute
            UInt32 ans = (UInt32)((Int32)(UInt16)dst - (Int32)(UInt16)src);
            reg.SetDw(dr, (UInt16)ans);

            //flag
            reg.SetN((UInt16)ans);
            reg.SetZ((UInt16)ans);
            reg.SetVcmp((UInt16)src, (UInt16)dst, (UInt16)ans);
            reg.SetCcmp((UInt16)src, (UInt16)dst, (UInt16)ans);
            reg.X = reg.C;

            //cycle
            cycle = cy.Sub_w[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubl(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "SUB.l ";
#endif


            int cycle = 0;
            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 src = srcAddressingLong(ref nimo, ref cycle, sm, sr);
#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            UInt64 dst = reg.D[dr];
#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif


            //compute
            UInt64 ans = (UInt64)((Int64)(UInt32)dst - (Int64)(UInt32)src);
            reg.SetDl(dr, (UInt32)ans);

            //flag
            reg.SetN((UInt32)ans);
            reg.SetZ((UInt32)ans);
            reg.SetVcmp((UInt32)src, (UInt32)dst, (UInt32)ans);
            reg.SetCcmp((UInt32)src, (UInt32)dst, (UInt32)ans);
            reg.X = reg.C;

            //cycle
            cycle = cy.Sub_l[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubaw(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "SUBA.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int32 val = (Int32)(Int16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //compute
            reg.A[dr] = (UInt32)(reg.A[dr] - val);
#if DEBUG_M68
            nimo += string.Format("A{0}", dr);
#endif


            //flag
            //none

            //cycle
            cycle = cy.Suba_w[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Csubal(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "SUBA.l ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 val = srcAddressingLong(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //compute
            reg.A[dr] -= val;
#if DEBUG_M68
            nimo += string.Format("A{0}", dr);
#endif


            //flag
            //none

            //cycle
            cycle = cy.Suba_l[cycle];

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Ccmp(ushort n)
        {
            int opmode = (n & 0x1c0) >> 6;

            switch (opmode)
            {
                case 0:
                    return Ccmp_b(n);
                case 1:
                    return Ccmp_w(n);
                case 2:
                    return Ccmp_l(n);
                case 3:
                    return Ccmpa_w(n);
                case 4:
                    return Ccmpm_b(n);
                case 5:
                    return Ccmpm_w(n);
                case 6:
                    return Ccmpm_l(n);
                case 7:
                    return Ccmpa_l(n);
            }

            throw new NotImplementedException();
        }

        private int Ccmp_b(ushort n)
        {
            int cycle = 0;
#if DEBUG_M68
            string nimo = "CMP.b ";
#endif

            int rn = (n & 0x0e00) >> 9;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            ushort after;
            ushort src = 0;
            byte dst = reg.GetDb(rn);

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = (ushort)(short)(sbyte)reg.GetDb(dr);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Cmp_b[0];
                    break;
                case 1://An
                    throw new NotImplementedException();
                case 2://(An)
                    src = (ushort)(short)(sbyte)mem.PeekB(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Cmp_b[2];
                    break;
                case 3://(An)+
                    src = (ushort)(short)(sbyte)mem.PeekB(reg.A[dr]);
                    reg.A[dr]++;
                    if (dr == 7) reg.A[dr] += 1;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Cmp_b[3];
                    break;
                case 4://-(An)
                    reg.A[dr]--;
                    if (dr == 7) reg.A[dr]--;
                    src = (ushort)(short)(sbyte)mem.PeekB(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Cmp_b[4];
                    break;
                case 5://d16(An)
                    Int32 d16 = (Int32)(Int16)FetchW();
                    src = (ushort)(short)(sbyte)mem.PeekB((UInt32)(reg.A[dr] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", (Int16)d16, dr);
#endif

                    cycle = cy.Cmp_b[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = (ushort)(short)(sbyte)mem.PeekB(ptr);
                    cycle = cy.Cmp_b[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            src = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            cycle = cy.Cmp_b[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            src = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            cycle = cy.Cmp_b[8];
                            break;
                        case 2:
                            Int32 ed16 = (Int32)(Int16)FetchW();
                            src = (ushort)(short)(sbyte)mem.PeekB((UInt32)(reg.PC + ed16 - 2));
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", (Int16)ed16);
#endif

                            cycle = cy.Cmp_b[9];
                            break;
                        case 3:
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;

#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }

                            src = (ushort)(short)(sbyte)mem.PeekB(ptr);
                            cycle = cy.Cmp_b[10];
                            break;
                        case 4:
                            src = (byte)FetchW();
#if DEBUG_M68
                            nimo += string.Format("#${0:x02}", (byte)src);
#endif

                            cycle = cy.Cmp_b[11];
                            break;
                    }
                    break;
            }

            after = (ushort)((short)dst - (short)src);

#if DEBUG_M68
            nimo += string.Format(",D{0}", rn);
#endif


            //flag
            reg.SetN((byte)after);
            reg.SetZ((byte)after);
            reg.SetVcmp((byte)src, (byte)dst, (byte)after);
            reg.SetCcmp((byte)src, (byte)dst, (byte)after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            return cycle;
        }

        private int Ccmp_w(ushort n)
        {
            int cycle = 0;
#if DEBUG_M68
            string nimo = "CMP.w ";
#endif

            int rn = (n & 0x0e00) >> 9;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            ushort after;
            ushort src = 0;
            ushort dst = reg.GetDw(rn);

            //dst
            switch (dm)
            {
                case 0://Dn
                    src = reg.GetDw(dr);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    cycle = cy.Cmp_w[0];
                    break;
                case 1://An
                    src = reg.GetAw(dr);
#if DEBUG_M68
                    nimo += string.Format("A{0}", dr);
#endif

                    cycle = cy.Cmp_w[1];
                    break;
                case 2://(An)
                    src = mem.PeekW(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    cycle = cy.Cmp_w[2];
                    break;
                case 3://(An)+
                    src = mem.PeekW(reg.A[dr]);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    cycle = cy.Cmp_w[3];
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    src = mem.PeekW(reg.A[dr]);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Cmp_w[4];
                    break;
                case 5://d16(An)
                    Int32 d16 = (Int32)(Int16)FetchW();
                    src = mem.PeekW((UInt32)(reg.A[dr] + d16));
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", (Int16)d16, dr);
#endif

                    cycle = cy.Cmp_w[5];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    src = mem.PeekW(ptr);
                    cycle = cy.Cmp_w[6];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("(${0:x04})", (Int16)ptr);
#endif

                            src = mem.PeekW(ptr);
                            cycle = cy.Cmp_w[7];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("(${0:x08})", (Int32)ptr);
#endif

                            src = mem.PeekW(ptr);
                            cycle = cy.Cmp_w[8];
                            break;
                        case 2:
                            Int32 ed16 = (Int32)(Int16)FetchW();
                            src = mem.PeekW((UInt32)(reg.PC + ed16 - 2));
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", (Int16)ed16);
#endif

                            cycle = cy.Cmp_w[9];
                            break;
                        case 3:
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }

                            src = mem.PeekW(ptr);
                            cycle = cy.Cmp_w[10];
                            break;
                        case 4:
                            src = FetchW();
#if DEBUG_M68
                            nimo += string.Format("#${0:x02}", (byte)src);
#endif

                            cycle = cy.Cmp_w[11];
                            break;
                    }
                    break;
            }

            //after = (ushort)((short)before - (short)val);
            after = (ushort)((short)dst - (short)src);

#if DEBUG_M68
            nimo += string.Format(",D{0}", rn);
#endif


            //flag
            reg.SetN(after);
            reg.SetZ(after);
            reg.SetVcmp(src, dst, after);
            reg.SetCcmp(src, dst, after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            return cycle;
        }

        private int Ccmp_l(ushort n)
        {
            int cycle = 0;
            string nimo = "";
#if DEBUG_M68
            nimo = "CMP.l ";
#endif

            int rn = (n & 0x0e00) >> 9;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            uint after;

            uint src = srcAddressingLong(ref nimo, ref cycle, dm, dr);

            uint dst = reg.GetDl(rn);
            cycle = cy.Cmp_l[cycle];

            after = (uint)((int)dst - (int)src);

#if DEBUG_M68
            nimo += string.Format(",D{0}", rn);
#endif


            //flag
            reg.SetN(after);
            reg.SetZ(after);
            reg.SetVcmp(src, dst, after);
            reg.SetCcmp(src, dst, after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            return cycle;
        }

        private int Ccmpa_w(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Ccmpa_l(ushort n)
        {
            int cycle = 0;
            string nimo = "";
#if DEBUG_M68
            nimo = "CMPA.l ";
#endif

            int rn = (n & 0x0e00) >> 9;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            uint after;
            uint val = reg.GetAl(rn);

            //dst
            uint before = srcAddressingLong(ref nimo, ref cycle, dm, dr);

            cycle = cy.Cmpa_l[cycle];

            after = (uint)((int)val - (int)before);

#if DEBUG_M68
            nimo += string.Format(",A{0}", rn);
#endif


            //flag
            reg.SetN(after);
            reg.SetZ(after);
            reg.SetVcmp((uint)val, (uint)before, (uint)after);
            reg.SetCcmp((uint)val, (uint)before, (uint)after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            return cycle;
        }

        private int Ccmpm_b(ushort n)
        {
            int cycle = 12;
            int dr = (n & 0x0e00) >> 9;
            int sr = (n & 0x0007);

            ushort after;
            ushort dst;
            byte vald = mem.PeekB(reg.GetAl(dr));
            byte src = mem.PeekB(reg.GetAl(sr));

            dst = (ushort)(short)(sbyte)vald;
#if DEBUG_M68
            string nimo = string.Format("CMPM.b (A{0})+,(A{1})+", sr, dr);
#endif

            reg.A[dr] += 1;
            reg.A[sr] += 1;

            after = (ushort)((short)dst - (sbyte)src);

            //flag
            reg.SetN((byte)after);
            reg.SetZ((byte)after);
            //reg.SetVcmp((byte)before, (byte)vals, (byte)after);
            //reg.SetCcmp((byte)before, (byte)vals, (byte)after);
            reg.SetVcmp((byte)src, (byte)dst, (byte)after);
            reg.SetCcmp((byte)src, (byte)dst, (byte)after);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif


            return cycle;
        }

        private int Ccmpm_w(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Ccmpm_l(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cmuls(UInt16 n)
        {

            string nimo = "";
#if DEBUG_M68
            nimo = "MULS.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int32 sval = (Int32)(Int16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int32 dval = (Int16)reg.D[dr];

            //compute
            Int32 ans = dval * sval;
            reg.D[dr] = (UInt32)(ans & 0xffff_ffff);

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Muls_w[cycle];

            //flag
            //reg.X -
            reg.N = (ans & 0x8000_000) != 0;
            reg.Z = ans == 0;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cmulu(UInt16 n)
        {

            if ((n & 0xf1c0) != 0xc0c0)
            {
                return Cand(n);
            }

            string nimo = "";
#if DEBUG_M68
            nimo = "MULU.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 sval = (UInt32)(UInt16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            UInt32 dval = (UInt16)reg.D[dr];

            //compute
            UInt32 ans = dval * sval;
            reg.D[dr] = (UInt32)ans;

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Mulu_w[cycle];

            //flag
            //reg.X -
            reg.N = (ans & 0x8000_000) != 0;
            reg.Z = ans == 0;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cdivs(UInt16 n)
        {

            string nimo = "";
#if DEBUG_M68
            nimo = "DIVS.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int32 sval = (Int32)(Int16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int32 dval = (Int32)reg.D[dr];

            //check TBD
            if (sval == 0)
            {
                //TRAP
            }
            //compute
            Int32 ans = dval / sval;
            Int32 mod = dval % sval;
            if (ans > 32767 || ans < -32768)
            {
                reg.V = true;
                return cy.Divs_w[cycle];
            }

            reg.D[dr] = (UInt32)((ans & 0xffff) | ((mod & 0xffff) * 0x10000));

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Divs_w[cycle];

            //flag
            //reg.X -
            reg.N = (ans & 0x8000) != 0;
            reg.Z = ans == 0;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cdivu(UInt16 n)
        {

            string nimo = "";
#if DEBUG_M68
            nimo = "DIVU.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            UInt32 sval = (UInt32)(UInt16)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            UInt32 dval = (UInt32)reg.D[dr];

            //check TBD
            if (sval == 0)
            {
                //TRAP
            }
            //compute
            UInt32 ans = dval / sval;
            UInt32 mod = dval % sval;
            if (ans > 0xffff)
            {
                reg.V = true;
                return cy.Divs_w[cycle];
            }

            reg.D[dr] = (UInt32)((ans & 0xffff) | ((mod & 0xffff) * 0x10000));

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Divu_w[cycle];

            //flag
            //reg.X -
            reg.N = (ans & 0x8000) != 0;
            reg.Z = ans == 0;
            reg.V = false;
            reg.C = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddi(UInt16 n)
        {
            int m = (n & 0x00c0) >> 6;
            switch (m)
            {
                case 0://byte
                    return Caddib(n);
                case 1://word
                    return Caddiw(n);
                case 2://long
                    return Caddil(n);
            }

            throw new NotImplementedException("dummy");
        }

        private int Caddib(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "ADDi.b ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            byte src = (byte)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x02},", src);
#endif

            byte dst = 0;
            byte ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    dst = reg.GetDb(dr);
                    ans = (byte)((sbyte)dst + (sbyte)src);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    reg.SetDb(dr, ans);
                    cycle = cy.Addi_b[0];
                    break;
                case 2://(An)
                    dst = mem.PeekB(reg.A[dr]);
                    ans = (byte)((sbyte)dst + (sbyte)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    mem.PokeB(reg.A[dr], ans);
                    cycle = cy.Addi_b[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekB(reg.A[dr]);
                    ans = (byte)((sbyte)dst + (sbyte)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    mem.PokeB(reg.A[dr], ans);
                    cycle = cy.Addi_b[2];
                    reg.A[dr] += 1;
                    if (dr == 7) reg.A[dr] += 1;
                    break;
                case 4://-(An)
                    reg.A[dr] -= 1;
                    if (dr == 7) reg.A[dr] += 1;
                    dst = mem.PeekB(reg.A[dr]);
                    ans = (byte)((sbyte)dst + (sbyte)src);
                    mem.PokeB(reg.A[dr], ans);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    cycle = cy.Addi_b[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekB((UInt32)(reg.A[dr] + d16));
                    ans = (byte)((sbyte)dst + (sbyte)src);
                    mem.PokeB((UInt32)(reg.A[dr] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Addi_b[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekB(ptr);
                    ans = (byte)((sbyte)dst + (sbyte)src);
                    mem.PokeB(ptr, ans);
                    cycle = cy.Addi_b[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (byte)((sbyte)dst + (sbyte)src);
                            mem.PokeB(ptr, ans);
                            cycle = cy.Addi_b[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekB(ptr);
                            ans = (byte)((sbyte)dst + (sbyte)src);
                            mem.PokeB(ptr, ans);
                            cycle = cy.Addi_b[7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVadd(src, dst, ans);
            reg.SetCadd(src, dst, ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddiw(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "ADDi.w ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            ushort src = (ushort)FetchW();
#if DEBUG_M68
            nimo += string.Format("#${0:x04},", src);
#endif

            ushort dst = 0;
            ushort ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    dst = reg.GetDw(dr);
                    ans = (ushort)((short)dst + (short)src);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    reg.SetDw(dr, ans);
                    cycle = cy.Addi_w[0];
                    break;
                case 2://(An)
                    dst = mem.PeekW(reg.A[dr]);
                    ans = (ushort)((short)dst + (short)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    mem.PokeW(reg.A[dr], ans);
                    cycle = cy.Addi_w[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekW(reg.A[dr]);
                    ans = (ushort)((short)dst + (short)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    mem.PokeW(reg.A[dr], ans);
                    cycle = cy.Addi_w[2];
                    reg.A[dr] += 2;
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    dst = mem.PeekW(reg.A[dr]);
                    ans = (ushort)((short)dst + (short)src);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    mem.PokeW(reg.A[dr], ans);
                    cycle = cy.Addi_w[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekW((UInt32)(reg.A[dr] + d16));
                    ans = (ushort)((short)dst + (short)src);
                    mem.PokeW((UInt32)(reg.A[dr] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Addi_w[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekW(ptr);
                    ans = (ushort)((short)dst + (short)src);
                    mem.PokeW(ptr, ans);
                    cycle = cy.Addi_w[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekW(ptr);
                            ans = (ushort)((short)dst + (short)src);
                            mem.PokeW(ptr, ans);
                            cycle = cy.Addi_w[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekW(ptr);
                            ans = (ushort)((short)dst + (short)src);
                            mem.PokeW(ptr, ans);
                            cycle = cy.Addi_w[7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVadd(src, dst, ans);
            reg.SetCadd(src, dst, ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddil(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "ADDi.l ";
#endif


            int cycle = 0;

            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);

            UInt32 src = (UInt32)FetchL();
#if DEBUG_M68
            nimo += string.Format("#${0:x08},", src);
#endif

            UInt32 dst = 0;
            UInt32 ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 0://Dn
                    dst = reg.GetDl(dr);
                    ans = (UInt32)((Int32)dst + (Int32)src);
#if DEBUG_M68
                    nimo += string.Format("D{0}", dr);
#endif

                    reg.SetDl(dr, ans);
                    cycle = cy.Addi_l[0];
                    break;
                case 2://(An)
                    dst = mem.PeekL(reg.A[dr]);
                    ans = (UInt32)((Int32)dst + (Int32)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif

                    mem.PokeL(reg.A[dr], ans);
                    cycle = cy.Addi_l[1];
                    break;
                case 3://(An)+
                    dst = mem.PeekL(reg.A[dr]);
                    ans = (UInt32)((Int32)dst + (Int32)src);
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif

                    mem.PokeL(reg.A[dr], ans);
                    cycle = cy.Addi_l[2];
                    reg.A[dr] += 4;
                    break;
                case 4://-(An)
                    reg.A[dr] -= 4;
                    dst = mem.PeekL(reg.A[dr]);
                    ans = (UInt32)((Int32)dst + (Int32)src);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif

                    mem.PokeL(reg.A[dr], ans);
                    cycle = cy.Addi_l[3];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    dst = mem.PeekL((UInt32)(reg.A[dr] + d16));
                    ans = (UInt32)((Int32)dst + (Int32)src);
                    mem.PokeL((UInt32)(reg.A[dr] + d16), ans);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif

                    cycle = cy.Addi_l[4];
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    dst = mem.PeekL(ptr);
                    ans = (UInt32)((Int32)dst + (Int32)src);
                    mem.PokeL(ptr, ans);
                    cycle = cy.Addi_l[5];
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            dst = mem.PeekL(ptr);
                            ans = (UInt32)((Int32)dst + (Int32)src);
                            mem.PokeL(ptr, ans);
                            cycle = cy.Addi_l[6];
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            dst = mem.PeekL(ptr);
                            ans = (UInt32)((Int32)dst + (Int32)src);
                            mem.PokeL(ptr, ans);
                            cycle = cy.Addi_l[7];
                            break;
                    }
                    break;
            }

            //flag
            reg.SetN(ans);
            reg.SetZ(ans);
            reg.SetVadd(src, dst, ans);
            reg.SetCadd(src, dst, ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cadd(UInt16 n)
        {
            // 0000 0110 xxxx xxxx ADDI
            // 0101 xxx0 xxxx xxxx ADDQ
            // 1101 xxx1 xx00 0xxx ADDX dr to dr
            // 1101 xxx1 xx00 1xxx ADDX mem to mem
            // 1101 xxx1 11xx xxxx ADDA
            // 1101 xxxx xxxx xxxx ADDA
            if ((n & 0xf138) == 0xd100)
            {
                //ADDX dr to dr
                int mm = (n & 0x01c0) >> 6;
                if (mm != 3 && mm != 7)
                {
                    mm = (n & 0x00c0) >> 6;
                    switch (mm)
                    {
                        case 0://byte
                            return Caddxb_dd(n);
                        case 1://word
                            return Caddxw_dd(n);
                        case 2://long
                            return Caddxl_dd(n);
                    }
                }
            }
            else if ((n & 0xf138) == 0xd108)
            {
                //ADDX mem to mem
                int mm = (n & 0x01c0) >> 6;
                if (mm != 3 && mm != 7)
                {
                    mm = (n & 0x00c0) >> 6;
                    switch (mm)
                    {
                        case 0://byte
                            return Caddxb_mm(n);
                        case 1://word
                            return Caddxw_mm(n);
                        case 2://long
                            return Caddxl_mm(n);
                    }
                }
            }

            int m = (n & 0x01c0) >> 6;
            switch (m)
            {
                case 0://byte
                    return Cadd0b(n);
                case 1://word
                    return Cadd0w(n);
                case 2://long
                    return Cadd0l(n);
                case 3://word Cadda
                    return Caddaw(n);
                case 4://byte
                    return Cadd1b(n);
                case 5://word
                    return Cadd1w(n);
                case 6://long
                    return Cadd1l(n);
                case 7://long Cadda
                    return Caddal(n);
            }

            throw new NotImplementedException("dummy");
        }

        private int Caddxb_dd(ushort n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "ADDX.b ";
#endif


            int cycle=4;

            int dr = (n & 0x0e00) >> 9;
            int sr = (n & 0x0007);

            //src
            Int32 sval = reg.GetDb(sr);

#if DEBUG_M68
            nimo += string.Format("D{0},", sr);
#endif


            //dst
            Int32 dval = (Int32)reg.GetDb(dr);

            //compute
            Int32 ans = dval + sval + (reg.X ? 1 : 0);
            reg.SetDb(dr, (byte)ans);

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            //flag
            reg.SetN((byte)ans);
            reg.SetZ((byte)ans);
            reg.SetVadd((byte)sval, (byte)dval, (byte)ans);
            reg.SetCadd((byte)sval, (byte)dval, (byte)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddxw_dd(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Caddxl_dd(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Caddxb_mm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Caddxw_mm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Caddxl_mm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cadd0b(UInt16 n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "ADD.b ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int32 sval = (Int32)(byte)srcAddressingByte(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int32 dval = (Int32)reg.GetDb(dr);

            //compute
            Int32 ans = dval + sval;
            reg.SetDb(dr, (byte)ans);

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Add0_b[cycle];

            //flag
            reg.SetN((byte)ans);
            reg.SetZ((byte)ans);
            reg.SetVadd((byte)sval, (byte)dval, (byte)ans);
            reg.SetCadd((byte)sval, (byte)dval, (byte)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cadd0w(UInt16 n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "ADD.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int32 sval = (Int32)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int32 dval = (Int16)reg.D[dr];

            //compute
            Int32 ans = dval + sval;
            reg.SetDw(dr, (UInt16)ans);

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Add0_w[cycle];

            //flag
            reg.SetN((UInt16)ans);
            reg.SetZ((UInt16)ans);
            reg.SetVadd((UInt16)sval, (UInt16)dval, (UInt16)ans);
            reg.SetCadd((UInt16)sval, (UInt16)dval, (UInt16)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cadd0l(UInt16 n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "ADD.l ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int64 sval = (Int64)srcAddressingLong(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int64 dval = (Int64)reg.D[dr];

            //compute
            Int64 ans = dval + sval;
            reg.D[dr] = (UInt32)ans;

#if DEBUG_M68
            nimo += string.Format("D{0}", dr);
#endif

            cycle = cy.Add0_l[cycle];

            //flag
            reg.SetN((UInt32)ans);
            reg.SetZ((UInt32)ans);
            reg.SetVadd((UInt32)sval, (UInt32)dval, (UInt32)ans);
            reg.SetCadd((UInt32)sval, (UInt32)dval, (UInt32)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cadd1b(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "ADD.b ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

#if DEBUG_M68
            nimo += string.Format("D{0},", dr);
#endif


            //src
            sbyte sval = (sbyte)reg.GetDb(dr);
            //dst
            sbyte dval = 0;

            sbyte ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (sm)
            {
                case 2://(An)
#if DEBUG_M68
                    nimo += string.Format("(A{0})", sr);
#endif

                    dval = (sbyte)mem.PeekB(reg.A[sr]);
                    ans = (sbyte)(dval + sval);
                    mem.PokeB(reg.A[sr], (byte)ans);
                    cycle = 0;
                    break;
                case 3://(An)+
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", sr);
#endif

                    dval = (sbyte)mem.PeekB(reg.A[sr]);
                    ans = (sbyte)(dval + sval);
                    mem.PokeB(reg.A[sr], (byte)ans);
                    reg.A[sr] += 1;
                    if (sr == 7) reg.A[sr] += 1;
                    cycle = 1;
                    break;
                case 4://-(An)
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 1;
                    if (sr == 7) reg.A[sr]--;
                    dval = (sbyte)mem.PeekB(reg.A[sr]);
                    ans = (sbyte)(dval + sval);
                    mem.PokeB(reg.A[sr], (byte)ans);
                    cycle = 2;
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    dval = (sbyte)mem.PeekB((UInt32)(reg.A[sr] + d16));
                    ans = (sbyte)(dval + sval);
                    mem.PokeB((UInt32)(reg.A[sr] + d16), (byte)ans);
                    cycle = 3;
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    dval = (sbyte)mem.PeekB(ptr);
                    ans = (sbyte)(dval + sval);
                    mem.PokeB(ptr, (byte)ans);
                    cycle = 4;
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            dval = (sbyte)mem.PeekB(ptr);
                            ans = (sbyte)(dval + sval);
                            mem.PokeB(ptr, (byte)ans);
                            cycle = 5;
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            dval = (sbyte)mem.PeekB(ptr);
                            ans = (sbyte)(dval + sval);
                            mem.PokeB(ptr, (byte)ans);
                            cycle = 6;
                            break;
                    }
                    break;
            }

            cycle = cy.Add1_b[cycle];

            //flag
            reg.SetN((byte)ans);
            reg.SetZ((byte)ans);
            reg.SetVadd((byte)sval, (byte)dval, (byte)ans);
            reg.SetCadd((byte)sval, (byte)dval, (byte)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cadd1w(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "ADD.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

#if DEBUG_M68
            nimo += string.Format("D{0},", dr);
#endif


            //src
            Int32 sval = (Int16)reg.D[dr];

            //dst
            Int32 dval = 0;

            Int32 ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (sm)
            {
                case 2://(An)
#if DEBUG_M68
                    nimo += string.Format("(A{0})", sr);
#endif

                    dval = (Int16)mem.PeekW(reg.A[sr]);
                    ans = dval + sval;
                    mem.PokeW(reg.A[sr], (ushort)ans);
                    cycle = 0;
                    break;
                case 3://(An)+
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", sr);
#endif

                    dval = (Int16)mem.PeekW(reg.A[sr]);
                    ans = dval + sval;
                    mem.PokeW(reg.A[sr], (ushort)ans);
                    reg.A[sr] += 2;
                    cycle = 1;
                    break;
                case 4://-(An)
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 2;
                    dval = (Int16)mem.PeekW(reg.A[sr]);
                    ans = dval + sval;
                    mem.PokeW(reg.A[sr], (ushort)ans);
                    cycle = 2;
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    dval = (Int16)mem.PeekW((UInt32)(reg.A[sr] + d16));
                    ans = dval + sval;
                    mem.PokeW((UInt32)(reg.A[sr] + d16), (ushort)ans);
                    cycle = 3;
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    dval = (Int16)mem.PeekW(ptr);
                    ans = dval + sval;
                    mem.PokeW(ptr, (ushort)ans);
                    cycle = 4;
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            dval = (Int16)mem.PeekW(ptr);
                            ans = dval + sval;
                            mem.PokeW(ptr, (ushort)ans);
                            cycle = 5;
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            dval = (Int16)mem.PeekW(ptr);
                            ans = dval + sval;
                            mem.PokeW(ptr, (ushort)ans);
                            cycle = 6;
                            break;
                    }
                    break;
            }

            cycle = cy.Add1_w[cycle];

            //flag
            reg.SetN((UInt16)ans);
            reg.SetZ((UInt16)ans);
            reg.SetVadd((UInt16)sval, (UInt16)dval, (UInt16)ans);
            reg.SetCadd((UInt16)sval, (UInt16)dval, (UInt16)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cadd1l(UInt16 n)
        {
#if DEBUG_M68
            string nimo = "ADD.l ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

#if DEBUG_M68
            nimo += string.Format("D{0},", dr);
#endif


            //src
            Int32 sval = (Int32)reg.GetDl(dr);

            //dst
            Int32 dval = 0;

            Int32 ans = 0;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (sm)
            {
                case 2://(An)
#if DEBUG_M68
                    nimo += string.Format("(A{0})", sr);
#endif

                    dval = (Int32)mem.PeekL(reg.A[sr]);
                    ans = dval + sval;
                    mem.PokeL(reg.A[sr], (UInt32)ans);
                    cycle = 0;
                    break;
                case 3://(An)+
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", sr);
#endif

                    dval = (Int32)mem.PeekL(reg.A[sr]);
                    ans = dval + sval;
                    mem.PokeL(reg.A[sr], (UInt32)ans);
                    reg.A[sr] += 4;
                    cycle = 1;
                    break;
                case 4://-(An)
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 4;
                    dval = (Int32)mem.PeekL(reg.A[sr]);
                    ans = dval + sval;
                    mem.PokeL(reg.A[sr], (UInt32)ans);
                    cycle = 2;
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    dval = (Int32)mem.PeekL((UInt32)(reg.A[sr] + d16));
                    ans = dval + sval;
                    mem.PokeL((UInt32)(reg.A[sr] + d16), (UInt32)ans);
                    cycle = 3;
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    dval = (Int32)mem.PeekL(ptr);
                    ans = dval + sval;
                    mem.PokeL(ptr, (UInt32)ans);
                    cycle = 4;
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            dval = (Int32)mem.PeekL(ptr);
                            ans = dval + sval;
                            mem.PokeL(ptr, (UInt32)ans);
                            cycle = 5;
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            dval = (Int32)mem.PeekL(ptr);
                            ans = dval + sval;
                            mem.PokeL(ptr, (UInt32)ans);
                            cycle = 6;
                            break;
                    }
                    break;
            }

            cycle = cy.Add1_l[cycle];

            //flag
            reg.SetN((UInt32)ans);
            reg.SetZ((UInt32)ans);
            reg.SetVadd((UInt32)sval, (UInt32)dval, (UInt32)ans);
            reg.SetCadd((UInt32)sval, (UInt32)dval, (UInt32)ans);
            reg.X = reg.C;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddaw(UInt16 n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "ADDA.w ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int32 sval = (Int32)srcAddressingWord(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int32 dval = (Int32)reg.A[dr];

            //compute
            Int32 ans = dval + sval;
            reg.A[dr] = (UInt32)ans;

#if DEBUG_M68
            nimo += string.Format("A{0}", dr);
#endif

            cycle = cy.Adda_w[cycle];

            ////flag
            //reg.SetN((UInt16)ans);
            //reg.SetZ((UInt16)ans);
            //reg.SetVadd((UInt16)sval, (UInt16)dval, (UInt16)ans);
            //reg.SetCadd((UInt16)sval, (UInt16)dval, (UInt16)ans);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Caddal(UInt16 n)
        {
            string nimo = "";
#if DEBUG_M68
            nimo = "ADDA.l ";
#endif


            int cycle = 0;

            int dr = (n & 0x0e00) >> 9;
            //int dm = (n & 0x01c0) >> 6;
            int sm = (n & 0x0038) >> 3;
            int sr = (n & 0x0007);

            //src
            Int64 sval = (Int64)srcAddressingLong(ref nimo, ref cycle, sm, sr);

#if DEBUG_M68
            nimo += ",";
#endif


            //dst
            Int64 dval = (Int64)reg.A[dr];

            //compute
            Int64 ans = dval + sval;
            reg.A[dr] = (UInt32)ans;

#if DEBUG_M68
            nimo += string.Format("A{0}", dr);
#endif

            cycle = cy.Adda_l[cycle];

            ////flag
            //reg.SetN((UInt32)ans);
            //reg.SetZ((UInt32)ans);
            //reg.SetVadd((UInt32)sval, (UInt32)dval, (UInt32)ans);
            //reg.SetCadd((UInt32)sval, (UInt32)dval, (UInt32)ans);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cshift(UInt16 n)
        {
            // 1110 ***1 **10 0*** -> ASL.b.w.l Dn,Dn
            // 1110 ***1 **00 0*** -> ASL.b.w.l #imm,Dn
            // 1110 0001 11** **** -> ASL.w     ea

            // 1110 ***0 **10 0*** -> ASR.b.w.l Dn,Dn
            // 1110 ***0 **00 0*** -> ASR.b.w.l #imm,Dn
            // 1110 0000 11** **** -> ASR.w     ea

            // 1110 ***1 **10 1*** -> LSL.b.w.l Dn,Dn
            // 1110 ***1 **00 1*** -> LSL.b.w.l #imm,Dn
            // 1110 0011 11** **** -> LSL.w     ea

            // 1110 ***0 **10 1*** -> LSR.b.w.l Dn,Dn
            // 1110 ***0 **00 1*** -> LSR.b.w.l #imm,Dn
            // 1110 0010 11** **** -> LSR.w     ea

            // 1110 ***1 **11 1*** -> ROL.b.w.l Dn,Dn
            // 1110 ***1 **01 1*** -> ROL.b.w.l #imm,Dn
            // 1110 0111 11** **** -> ROL.w     ea

            // 1110 ***0 **11 1*** -> ROR.b.w.l Dn,Dn
            // 1110 ***0 **01 1*** -> ROR.b.w.l #imm,Dn
            // 1110 0110 11** **** -> ROR.w     ea

            // 1110 ***1 **11 0*** -> ROXL.b.w.l Dn,Dn
            // 1110 ***1 **01 0*** -> ROXL.b.w.l #imm,Dn
            // 1110 0101 11** **** -> ROXL.w     ea

            // 1110 ***0 **11 0*** -> ROXR.b.w.l Dn,Dn
            // 1110 ***0 **01 0*** -> ROXR.b.w.l #imm,Dn
            // 1110 0100 11** **** -> ROXR.w     ea

            int cnt = (n & 0x0e00) >> 9;
            bool dirL = ((n & 0x0100) >> 8) == 1;
            int siz = (n & 0x00c0) >> 6;
            bool imm = ((n & 0x0020) >> 5) == 0;
            int typ = (n & 0x0018) >> 3;

            if (siz != 0x3)
            {
                switch (siz)
                {
                    case 0:
                        switch (typ)
                        {
                            case 0:
                                return dirL ? (imm ? Casl_b_imm(n) : Casl_b_DnDn(n)) : (imm ? Casr_b_imm(n) : Casr_b_DnDn(n));
                            case 1:
                                return dirL ? (imm ? Clsl_b_imm(n) : Clsl_b_DnDn(n)) : (imm ? Clsr_b_imm(n) : Clsr_b_DnDn(n));
                            case 2:
                                return dirL ? (imm ? Croxl_b_imm(n) : Croxl_b_DnDn(n)) : (imm ? Croxr_b_imm(n) : Croxr_b_DnDn(n));
                            case 3:
                                return dirL ? (imm ? Crol_b_imm(n) : Crol_b_DnDn(n)) : (imm ? Cror_b_imm(n) : Cror_b_DnDn(n));
                        }
                        break;
                    case 1:
                        switch (typ)
                        {
                            case 0:
                                return dirL ? (imm ? Casl_w_imm(n) : Casl_w_DnDn(n)) : (imm ? Casr_w_imm(n) : Casr_w_DnDn(n));
                            case 1:
                                return dirL ? (imm ? Clsl_w_imm(n) : Clsl_w_DnDn(n)) : (imm ? Clsr_w_imm(n) : Clsr_w_DnDn(n));
                            case 2:
                                return dirL ? (imm ? Croxl_w_imm(n) : Croxl_w_DnDn(n)) : (imm ? Croxr_w_imm(n) : Croxr_w_DnDn(n));
                            case 3:
                                return dirL ? (imm ? Crol_w_imm(n) : Crol_w_DnDn(n)) : (imm ? Cror_w_imm(n) : Cror_w_DnDn(n));
                        }
                        break;
                    case 2:
                        switch (typ)
                        {
                            case 0:
                                return dirL ? (imm ? Casl_l_imm(n) : Casl_l_DnDn(n)) : (imm ? Casr_l_imm(n) : Casr_l_DnDn(n));
                            case 1:
                                return dirL ? (imm ? Clsl_l_imm(n) : Clsl_l_DnDn(n)) : (imm ? Clsr_l_imm(n) : Clsr_l_DnDn(n));
                            case 2:
                                return dirL ? (imm ? Croxl_l_imm(n) : Croxl_l_DnDn(n)) : (imm ? Croxr_l_imm(n) : Croxr_l_DnDn(n));
                            case 3:
                                return dirL ? (imm ? Crol_l_imm(n) : Crol_l_DnDn(n)) : (imm ? Cror_l_imm(n) : Cror_l_DnDn(n));
                        }
                        break;
                }
            }
            else
            {
                switch (cnt)
                {
                    case 0:
                        return dirL ? Casl_w_ea(n) : Casr_w_ea(n);
                    case 1:
                        return dirL ? Clsl_w_ea(n) : Clsr_w_ea(n);
                    case 2:
                        return dirL ? Croxl_w_ea(n) : Croxr_w_ea(n);
                    case 3:
                        return dirL ? Crol_w_ea(n) : Cror_w_ea(n);
                }
            }

            throw new NotImplementedException();
        }

        private int Casl_b_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("ASL.b D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            sbyte bv = (sbyte)reg.GetDb(dr);
            sbyte av = (sbyte)(bv << cnt);
            reg.SetDb(dr, (byte)av);

            reg.C = ((bv & (0x80 >> (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = (bv & (0xff << cnt)) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casl_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ASL.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            sbyte bv = (sbyte)reg.GetDb(d);//算術シフトは符号有りの型でシフトを行う
            sbyte av = (sbyte)(bv << cnt);

            reg.SetDb(d, (byte)av);
            reg.C = reg.X = ((bv & (0x80 >> (cnt - 1))) != 0);
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = (bv & (0xff << cnt)) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casl_w_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("ASL.w D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            short bv = (short)reg.GetDw(dr);
            short av = (short)(bv << cnt);
            reg.SetDw(dr, (ushort)av);

            reg.C = ((bv & (0x8000 >> (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = (bv & (0xffff << cnt)) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casl_w_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ASL.w #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            short bv = (short)reg.GetDw(d);//算術シフトは符号有りの型でシフトを行う
            short av = (short)(bv << cnt);

            reg.SetDw(d, (ushort)av);
            reg.C = reg.X = ((bv & (0x8000 >> (cnt - 1))) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = (bv & (0xffff << cnt)) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casl_l_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("ASL.l D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            int bv = (int)reg.GetDl(dr);
            int av = bv << cnt;
            reg.SetDl(dr, (uint)av);

            reg.C = ((bv & (0x8000_0000 >> (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN((uint)av);
            reg.SetZ((uint)av);
            reg.V = (bv & (0xffff_ffff << cnt)) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casl_l_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ASL.l #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            int bv = (int)reg.GetDl(d);//算術シフトは符号有りの型でシフトを行う
            int av = bv << cnt;
            reg.SetDl(d, (uint)av);

            reg.C = reg.X = ((bv & (0x8000_0000 >> (cnt - 1))) != 0);
            reg.SetN((uint)av);
            reg.SetZ((uint)av);
            reg.V = (bv & (0xffff_ffff << cnt)) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casl_w_ea(ushort n)
        {
#if DEBUG_M68
            string nimo;
            nimo = "ASL.w ";
#endif

            short before=0;
            short after=0;

            int cycle = 0;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
            int sr = (n & 0x0e00) >> 9;

            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;

            switch (dm)
            {
                case 2://(An)
                    before = (short)mem.PeekW(reg.A[dr]);
                    after = (short)(before << 1);
                    mem.PokeW(reg.A[dr], (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("(A{0})", dr);
#endif
                    cycle = 12;
                    break;
                case 3://(An)+
                    before = (short)mem.PeekW(reg.A[dr]);
                    after = (short)(before << 1);
                    mem.PokeW(reg.A[dr], (ushort)after);
                    reg.A[dr] += 2;
#if DEBUG_M68
                    nimo += string.Format("(A{0})+", dr);
#endif
                    cycle = 12;
                    break;
                case 4://-(An)
                    reg.A[dr] -= 2;
                    before = (short)mem.PeekW(reg.A[dr]);
                    after = (short)(before << 1);
                    mem.PokeW(reg.A[dr], (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("-(A{0})", dr);
#endif
                    cycle = 14;
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
                    before = (short)mem.PeekW((UInt32)(reg.A[dr]+d16));
                    after = (short)(before << 1);
                    mem.PokeW((UInt32)(reg.A[dr] + d16), (ushort)after);
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, dr);
#endif
                    cycle = 16;
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, dr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[dr] + ((sbyte)(byte)vw) + IX);
                    before = (short)mem.PeekW(ptr);
                    after = (short)(before << 1);
                    mem.PokeW(ptr, (ushort)after);
                    cycle = 18;
                    break;
                case 7://etc.
                    switch (dr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (Int16)ptr);
#endif

                            before = (short)mem.PeekW(ptr);
                            after = (short)(before << 1);
                            mem.PokeW(ptr, (ushort)after);
                            cycle = 16;
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (Int32)ptr);
#endif

                            before = (short)mem.PeekW(ptr);
                            after = (short)(before << 1);
                            mem.PokeW(ptr, (ushort)after);
                            cycle = 20;
                            break;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            reg.C = reg.X = ((before & 0x8000) != 0);
            reg.SetN((ushort)after);
            reg.SetZ((ushort)after);
            reg.V = (before & 0xffff) != 0;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_b_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);
#if DEBUG_M68
            nimo = string.Format("ASR.b D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;
            sbyte bv = (sbyte)reg.GetDb(dr);//算術シフトは符号有りの型でシフトを行う
            sbyte av = (sbyte)(bv >> cnt);
            reg.SetDb(dr, (byte)av);

            reg.C = ((bv & (0x01 << (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ASR.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            sbyte bv = (sbyte)reg.GetDb(d);//算術シフトは符号有りの型でシフトを行う
            sbyte av = (sbyte)(bv >> cnt);
            reg.SetDb(d, (byte)av);

            reg.C = reg.X = ((bv & (0x01 << (cnt - 1))) != 0);
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_w_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);
#if DEBUG_M68
            nimo = string.Format("ASR.w D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;
            short bv = (short)reg.GetDw(dr);//算術シフトは符号有りの型でシフトを行う
            short av = (short)(bv >> cnt);
            reg.SetDw(dr, (ushort)av);

            reg.C = ((bv & (0x0001 << (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_w_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ASR.w #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            short bv = (short)reg.GetDw(d);//算術シフトは符号有りの型でシフトを行う
            short av = (short)(bv >> cnt);
            reg.SetDw(d, (ushort)av);

            reg.C = reg.X = ((bv & (0x0001 << (cnt - 1))) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_l_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);
#if DEBUG_M68
            nimo = string.Format("ASR.l D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;
            Int32 bv = (Int32)reg.GetDl(dr);//算術シフトは符号有りの型でシフトを行う
            Int32 av = (Int32)(bv >> cnt);
            reg.SetDl(dr, (UInt32)av);

            reg.C = ((bv & (0x0000_0001 << (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN((UInt32)av);
            reg.SetZ((UInt32)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_l_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ASR.l #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            int bv = (int)reg.GetDl(d);//算術シフトは符号有りの型でシフトを行う
            int av = bv >> cnt;
            reg.SetDl(d, (uint)av);

            reg.C = reg.X = ((bv & (0x0000_0001 << (cnt - 1))) != 0);
            reg.SetN((uint)av);
            reg.SetZ((uint)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Casr_w_ea(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Clsl_b_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("LSL.b D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            uint bv = reg.GetDb(dr);
            uint av = bv << cnt;
            reg.SetDb(dr, (byte)av); // >>論理シフト >>=算術シフト

            reg.C = (cnt == 0) ? false : ((bv & (0x80 >> cnt)) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsl_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("LSL.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDb(d);
            uint av = bv << cnt;
            reg.SetDb(d, (byte)av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((bv & (0x80 >> cnt)) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsl_w_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("LSL.w D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            uint bv = reg.GetDw(dr);
            uint av = bv << cnt;
            reg.SetDw(dr, (ushort)av); // >>論理シフト >>=算術シフト

            reg.C = (cnt == 0) ? false : ((bv & (0x8000 >> cnt)) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsl_w_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("LSL.w #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDw(d);
            uint av = bv << cnt;
            reg.SetDw(d, (ushort)av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((bv & (0x8000 >> cnt)) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsl_l_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("LSL.l D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            uint bv = reg.GetDl(dr);
            uint av = bv << cnt;
            reg.SetDl(dr, av); // >>論理シフト >>=算術シフト

            reg.C = (cnt == 0) ? false : ((bv & (0x8000_0000 >> cnt)) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsl_l_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("LSL.l #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDl(d);
            uint av = bv << cnt;
            reg.SetDl(d, av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((bv & (0x8000_0000 >> cnt)) != 0);
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsl_w_ea(ushort n)
        {
            string nimo = "";

            int cycle = 0;

            int cnt = 1;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
#if DEBUG_M68
            nimo = "LSL.w ";
#endif


            //dst
            uint before = srcAddressingWord(ref nimo, ref cycle, dm, dr, 0b_0001_1111_1100);

            cycle = cy.Clsrlsl_wea[cycle];
            uint av = before << cnt;
            reg.SetDw(dr, (ushort)av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((before & (0x8000 >> cnt)) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_b_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("LSR.b D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            uint bv = reg.GetDb(dr);
            uint av = bv >> cnt;
            reg.SetDb(dr, (byte)av); // >>論理シフト >>=算術シフト

            reg.C = (cnt == 0) ? false : ((bv & (0x01 << (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("LSR.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDb(d);
            uint av = bv >> cnt;
            reg.SetDb(d, (byte)av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((bv & (0x01 << (cnt - 1))) != 0);
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_w_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("LSR.w D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            uint bv = reg.GetDw(dr);
            uint av = bv >> cnt;
            reg.SetDw(dr, (ushort)av); // >>論理シフト >>=算術シフト

            reg.C = (cnt == 0) ? false : ((bv & (0x0001 << (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_w_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo=string.Format("LSR.w #{0:d},D{1}",cnt,d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDw(d);
            uint av = bv >> cnt;
            reg.SetDw(d, (ushort)av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((bv & (0x0001 << (cnt - 1))) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_l_DnDn(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;
            int sr = (n & 0x0e00) >> 9;
            int dr = (n & 0x0007);
            int cnt = (int)(reg.GetDl(sr) % 64);

#if DEBUG_M68
            nimo = string.Format("LSR.l D{0},D{1}", sr, dr);
#endif

            cycle += 2 * cnt;

            uint bv = reg.GetDl(dr);
            uint av = bv >> cnt;
            reg.SetDl(dr, av); // >>論理シフト >>=算術シフト

            reg.C = (cnt == 0) ? false : ((bv & (0x0000_0001 << (cnt - 1))) != 0);
            if (cnt != 0) reg.X = reg.C;
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_l_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("LSR.l #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDl(d);
            uint av = bv >> cnt;
            reg.SetDl(d, av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((bv & (0x0000_0001 << (cnt - 1))) != 0);
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Clsr_w_ea(ushort n)
        {
            string nimo = "";

            int cycle = 0;

            int cnt = 1;
            int dm = (n & 0x0038) >> 3;
            int dr = (n & 0x0007);
#if DEBUG_M68
            nimo = "LSR.w ";
#endif


            //dst
            uint before = srcAddressingWord(ref nimo, ref cycle, dm, dr, 0b_0001_1111_1100);

            cycle = cy.Clsrlsl_wea[cycle];

            uint av = before >> cnt;
            reg.SetDw(dr, (ushort)av); // >>論理シフト >>=算術シフト

            reg.C = reg.X = ((before & (0x0001 << (cnt - 1))) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Crol_b_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Crol_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROL.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDb(d);
            //uint av = (bv >> cnt) | (bv << (32 - cnt));//RLR
            uint av = (bv << cnt) | (bv >> (8 - cnt));
            reg.SetDb(d, (byte)av);

            //reg.X 変化なし
            reg.C = ((av & 0x1) != 0);
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Crol_w_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Crol_w_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROL.w #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDw(d);
            uint av = (bv << cnt) | (bv >> (16 - cnt));
            reg.SetDw(d, (ushort)av); // >>論理シフト >>=算術シフト

            //reg.X 変化なし
            reg.C = ((av & 0x1) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Crol_l_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Crol_l_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROL.l #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDl(d);
            //uint av = (bv >> cnt) | (bv << (32 - cnt));//RLR
            uint av = (bv << cnt) | (bv >> (32 - cnt));
            reg.SetDl(d, (uint)av); // >>論理シフト >>=算術シフト

            //reg.X 変化なし
            reg.C = ((av & 0x1) != 0);
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Crol_w_ea(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cror_b_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cror_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROR.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            byte bv = reg.GetDb(d);
            byte av = (byte)((bv >> cnt) | (bv << (8 - cnt)));
            //uint av = (bv << cnt) | (bv >> (16 - cnt));
            reg.SetDb(d, (byte)av);

            //reg.X 変化なし
            reg.C = ((av & 0x80) != 0);
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cror_w_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cror_w_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROR.w #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            ushort bv = reg.GetDw(d);
            ushort av = (ushort)((bv >> cnt) | (bv << (16 - cnt)));
            //uint av = (bv << cnt) | (bv >> (16 - cnt));
            reg.SetDw(d, (ushort)av);

            //reg.X 変化なし
            reg.C = ((av & 0x8000) != 0);
            reg.SetN(av);
            reg.SetZ(av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cror_l_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Cror_l_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 8;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROR.l #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDl(d);
            uint av = (bv >> cnt) | (bv << (32 - cnt));//ROR
            reg.SetDw(d, (ushort)av); // >>論理シフト >>=算術シフト

            //reg.X 変化なし
            reg.C = ((av & 0x8000_0000) != 0);
            reg.SetN((ushort)av);
            reg.SetZ((ushort)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cror_w_ea(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxl_b_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxl_b_imm(ushort n)
        {
#if DEBUG_M68
            string nimo;
#endif

            int cycle = 6;

            int cnt = (n & 0x0e00) >> 9;
            cnt = (cnt == 0) ? 8 : cnt;
            int d = (n & 0x0007);
#if DEBUG_M68
            nimo = string.Format("ROXL.b #{0:d},D{1}", cnt, d);
#endif

            cycle += 2 * cnt;
            uint bv = reg.GetDb(d);
            //uint av = (bv >> cnt) | (bv << (32 - cnt));//RLR
            uint av = (uint)((bv << cnt) | (((uint)(reg.X ? 0x100 : 0x000) | bv) >> (9 - cnt)));
            reg.SetDb(d, (byte)av);

            reg.X = reg.C = (((bv << cnt) & 0x100) != 0);
            reg.SetN((byte)av);
            reg.SetZ((byte)av);
            reg.V = false;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Croxl_w_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxl_w_imm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxl_l_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxl_l_imm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxl_w_ea(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_b_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_b_imm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_w_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_w_imm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_l_DnDn(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_l_imm(ushort n)
        {
            throw new NotImplementedException();
        }

        private int Croxr_w_ea(ushort n)
        {
            throw new NotImplementedException();
        }

        private int CFEFunc(ushort n)
        {
            hmn.FEFunc(n);
            return 0;
        }

        private int Cdos(ushort n)
        {
            hmn.doscall(n);
            return 0;
        }

        private int Clink(UInt16 n)
        {
            int dr = (n & 0x7);
            int cycle = 16;
#if DEBUG_M68
            string nimo = "LINK A{0}, #${1:d}";
#endif

            short ptr = (short)FetchW();
#if DEBUG_M68
            nimo = string.Format(nimo, dr, ptr);
#endif

            reg.A[7] -= 4;
            mem.PokeL(reg.A[7], reg.A[dr]);
            reg.A[dr] = reg.A[7];
            reg.A[7] = (uint)(reg.A[7] + (int)ptr);

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private int Cunlk(UInt16 n)
        {
            int dr = (n & 0x7);
            int cycle = 16;
#if DEBUG_M68
            string nimo = "UNLK A{0}";
#endif

#if DEBUG_M68
            nimo = string.Format(nimo, dr);
#endif

            reg.A[7] = reg.A[dr];
            reg.A[dr] = mem.PeekL(reg.A[7]);
            reg.A[7] += 4;

#if DEBUG_M68
            Log.WriteLine(LogLevel.Trace, nimo);
#endif

            return cycle;
        }

        private uint srcAddressingByte(ref string nimo, ref int cycle, int sm, int sr, uint support = 0xfff, bool nimoSw = true)
        {
            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            sbyte val = 0;

            cycle = sm;
            switch (sm)
            {
                case 0://Dn
                    if ((support & (1 << 0)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("D{0}", sr);
#endif

                    val = (sbyte)reg.D[sr];
                    break;
                case 1://An
                    if ((support & (1 << 1)) == 1) throw new NotImplementedException();
                    throw new ArgumentOutOfRangeException("対応していないアドレッシング");
#if DEBUG_M68
                    //nimo += string.Format("A{0}", sr);
#endif

                //val = (sbyte)reg.A[sr];
                //break;
                case 2://(An)
                    if ((support & (1 << 2)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("(A{0})", sr);
#endif

                    val = (sbyte)mem.PeekB(reg.A[sr]);
                    break;
                case 3://(An)+
                    if ((support & (1 << 3)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("(A{0})+", sr);
#endif

                    val = (sbyte)mem.PeekB(reg.A[sr]);
                    reg.A[sr] ++;
                    if (sr == 7) reg.A[sr]++;
                    break;
                case 4://-(An)
                    if ((support & (1 << 4)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr]--;
                    if (sr == 7) reg.A[sr]--;
                    val = (sbyte)mem.PeekB(reg.A[sr]);
                    break;
                case 5://d16(An)
                    if ((support & (1 << 5)) == 0) throw new NotImplementedException();
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    val = (sbyte)mem.PeekB((UInt32)(reg.A[sr] + d16));
                    break;
                case 6://d8(An,IX)
                    if ((support & (1 << 6)) == 0) throw new NotImplementedException();
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    val = (sbyte)mem.PeekB(ptr);
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            if ((support & (1 << 7)) == 0) throw new NotImplementedException();
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            val = (sbyte)mem.PeekB(ptr);
                            break;
                        case 1://Abs.L
                            if ((support & (1 << 8)) == 0) throw new NotImplementedException();
                            ptr = FetchL();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            val = (sbyte)mem.PeekB(ptr);
                            cycle = 8;
                            break;
                        case 2://d16(PC)
                            if ((support & (1 << 9)) == 0) throw new NotImplementedException();
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x04}(PC)", (UInt16)ptr);
#endif

                            val = (sbyte)mem.PeekB(ptr + reg.PC - 2);
                            cycle = 9;
                            break;
                        case 3://d8(PC,IX)
                            if ((support & (1 << 10)) == 0) throw new NotImplementedException();
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }

                            val = (sbyte)mem.PeekB(ptr);
                            cycle = 10;
                            break;
                        case 4://#Imm
                            if ((support & (1 << 11)) == 0) throw new NotImplementedException();
                            val = (sbyte)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("#${0:x02}", val);
#endif

                            cycle = 11;
                            break;
                    }
                    break;
            }

            return (UInt32)val;
        }

        private uint srcAddressingWord(ref string nimo, ref int cycle, int sm, int sr, uint support = 0xfff, bool nimoSw = true)
        {
            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            Int16 val = 0;

            cycle = sm;
            switch (sm)
            {
                case 0://Dn
                    if ((support & (1 << 0)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("D{0}", sr);
#endif

                    val = (Int16)reg.D[sr];
                    break;
                case 1://An
                    if ((support & (1 << 1)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("A{0}", sr);
#endif

                    val = (Int16)reg.A[sr];
                    break;
                case 2://(An)
                    if ((support & (1 << 2)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("(A{0})", sr);
#endif

                    val = (Int16)mem.PeekW(reg.A[sr]);
                    break;
                case 3://(An)+
                    if ((support & (1 << 3)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("(A{0})+", sr);
#endif

                    val = (Int16)mem.PeekW(reg.A[sr]);
                    reg.A[sr] += 2;
                    break;
                case 4://-(An)
                    if ((support & (1 << 4)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 2;
                    val = (Int16)mem.PeekW(reg.A[sr]);
                    break;
                case 5://d16(An)
                    if ((support & (1 << 5)) == 0) throw new NotImplementedException();
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    val = (Int16)mem.PeekW((UInt32)(reg.A[sr] + d16));
                    break;
                case 6://d8(An,IX)
                    if ((support & (1 << 6)) == 0) throw new NotImplementedException();
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    val = (Int16)mem.PeekW(ptr);
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            if ((support & (1 << 7)) == 0) throw new NotImplementedException();
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            val = (Int16)mem.PeekW(ptr);
                            break;
                        case 1://Abs.L
                            if ((support & (1 << 8)) == 0) throw new NotImplementedException();
                            ptr = FetchL();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            val = (Int16)mem.PeekW(ptr);
                            cycle = 8;
                            break;
                        case 2://d16(PC)
                            if ((support & (1 << 9)) == 0) throw new NotImplementedException();
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x04}(PC)", (UInt16)ptr);
#endif

                            val = (Int16)mem.PeekW(ptr + reg.PC - 2);
                            cycle = 9;
                            break;
                        case 3://d8(PC,IX)
                            if ((support & (1 << 10)) == 0) throw new NotImplementedException();
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }

                            val = (Int16)mem.PeekW(ptr);
                            cycle = 10;
                            break;
                        case 4://#Imm
                            if ((support & (1 << 11)) == 0) throw new NotImplementedException();
                            val = (Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("#${0:x04}", val);
#endif

                            cycle = 11;
                            break;
                    }
                    break;
            }

            return (UInt32)val;
        }

        private uint srcAddressingLong(ref string nimo, ref int cycle, int sm, int sr, uint support = 0xfff, bool nimoSw = true, int shift = 0)
        {
            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            cycle = sm;
            switch (sm)
            {
                case 0://Dn
                    if ((support & (1 << 0)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("D{0}", sr);
#endif

                    val = reg.D[sr];
                    break;
                case 1://An
                    if ((support & (1 << 1)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("A{0}", sr);
#endif

                    val = reg.A[sr];
                    break;
                case 2://(An)
                    if ((support & (1 << 2)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("(A{0})", sr);
#endif

                    val = mem.PeekL((UInt32)(reg.A[sr] + shift));
                    break;
                case 3://(An)+
                    if ((support & (1 << 3)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("(A{0})+", sr);
#endif

                    val = mem.PeekL(reg.A[sr]);
                    reg.A[sr] += 4;
                    break;
                case 4://-(An)
                    if ((support & (1 << 4)) == 0) throw new NotImplementedException();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("-(A{0})", sr);
#endif

                    reg.A[sr] -= 4;
                    val = mem.PeekL(reg.A[sr]);
                    break;
                case 5://d16(An)
                    if ((support & (1 << 5)) == 0) throw new NotImplementedException();
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    if (nimoSw) nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    val = mem.PeekL((UInt32)(reg.A[sr] + d16 + shift));
                    break;
                case 6://d8(An,IX)
                    if ((support & (1 << 6)) == 0) throw new NotImplementedException();
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    if(nimoSw) nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    val = mem.PeekL((UInt32)(ptr + shift));
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            if ((support & (1 << 7)) == 0) throw new NotImplementedException();
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("(${0:x04})", (UInt16)ptr);
#endif

                            val = mem.PeekL((UInt32)(ptr + shift));
                            break;
                        case 1://Abs.L
                            if ((support & (1 << 8)) == 0) throw new NotImplementedException();
                            ptr = FetchL();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("(${0:x08})", (UInt32)ptr);
#endif

                            val = mem.PeekL((UInt32)(ptr + shift));
                            cycle = 8;
                            break;
                        case 2://d16(PC)
                            if ((support & (1 << 9)) == 0) throw new NotImplementedException();
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("${0:x04}(PC)", (UInt16)ptr);
#endif

                            val = mem.PeekL((UInt32)(ptr + reg.PC - 2 + shift));
                            cycle = 9;
                            break;
                        case 3://d8(PC,IX)
                            if ((support & (1 << 10)) == 0) throw new NotImplementedException();
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;
#if DEBUG_M68
                            if(nimoSw) nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }
                            val = mem.PeekL((UInt32)(ptr + shift));
                            cycle = 10;
                            break;
                        case 4://#Imm
                            if ((support & (1 << 11)) == 0) throw new NotImplementedException();
                            val = FetchL();
#if DEBUG_M68
                            if (nimoSw) nimo += string.Format("#${0:x08}", val);
#endif

                            cycle = 11;
                            break;
                    }
                    break;
            }

            return val;
        }

        private uint srcAddressingLongLea(ref string nimo, ref int cycle, int sm, int sr)
        {
            UInt16 vw;
            bool isA;
            int ni;
            bool isL;
            UInt32 IX;
            UInt32 ptr;
            UInt32 val = 0;

            cycle = sm;
            switch (sm)
            {
                case 0:
                case 1:
                case 3:
                case 4:
                    throw new NotImplementedException(string.Format("LEA 不正なアドレッシングモード {0:x04}", sm));
                case 2://(An)
#if DEBUG_M68
                    nimo += string.Format("(A{0})", sr);
#endif

                    val = reg.A[sr];
                    break;
                case 5://d16(An)
                    Int16 d16 = (Int16)FetchW();
#if DEBUG_M68
                    nimo += string.Format("${0:x04}(A{1})", d16, sr);
#endif

                    val = (UInt32)(reg.A[sr] + d16);
                    break;
                case 6://d8(An,IX)
                    vw = FetchW();
                    isA = (vw & 0x8000) != 0;
                    ni = (vw & 0x7000) >> 12;
                    isL = (vw & 0x0800) != 0;
                    IX = (isA ? reg.A[ni] : reg.D[ni]);
#if DEBUG_M68
                    nimo += string.Format("${0:x02}(A{1},{2}{3}.{4})", (byte)vw, sr, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                    if (!isL) ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + (Int16)(UInt16)IX);
                    else ptr = (UInt32)(reg.A[sr] + ((sbyte)(byte)vw) + IX);
                    val = ptr;
                    break;
                case 7://etc.
                    switch (sr)
                    {
                        case 0://Abs.W
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}", (UInt16)ptr);
#endif

                            val = ptr;
                            break;
                        case 1://Abs.L
                            ptr = FetchL();
#if DEBUG_M68
                            nimo += string.Format("${0:x08}", (UInt32)ptr);
#endif

                            val = ptr;
                            cycle = 8;
                            break;
                        case 2://d16(PC)
                            ptr = (UInt32)(Int16)FetchW();
#if DEBUG_M68
                            nimo += string.Format("${0:x04}(PC)", (UInt16)ptr);
#endif

                            val = ptr + reg.PC - 2;
                            cycle = 9;
                            break;
                        case 3://d8(PC,IX)
                            vw = FetchW();
                            isA = (vw & 0x8000) != 0;
                            ni = (vw & 0x7000) >> 12;
                            isL = (vw & 0x0800) != 0;

#if DEBUG_M68
                            nimo += string.Format("${0:x02}(PC,{1}{2}.{3})", (byte)vw, isA ? "A" : "D", ni, isL ? "l" : "w");
#endif

                            if (isL)
                            {
                                IX = (isA ? reg.GetAl(ni) : reg.GetDl(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int32)(UInt32)IX - 2);
                            }
                            else
                            {
                                IX = (isA ? reg.GetAw(ni) : reg.GetDw(ni));
                                ptr = (UInt32)(reg.PC + ((sbyte)(byte)vw) + (Int16)(UInt16)IX - 2);
                            }

                            val = ptr;
                            cycle = 10;
                            break;
                        default:
                            throw new NotImplementedException(string.Format("LEA 知らないモード {0:x04}", sm));
                    }
                    break;
            }

            return val;
        }

        private void PushSSP(uint val)
        {
            reg.SSP -= 4;
            mem.PokeL(reg.SSP, val);
        }

        private void PushSSPw(UInt16 val)
        {
            reg.SSP -= 2;
            mem.PokeW(reg.SSP, val);
        }

        private void PushUSP(uint val)
        {
            reg.USP -= 4;
            mem.PokeL(reg.USP, val);
        }

        private void Push(uint val)
        {
            reg.A[7] -= 4;
            mem.PokeL(reg.A[7], val);
        }

        private UInt32 Pop()
        {
            UInt32 val = mem.PeekL(reg.A[7]);
            reg.A[7] += 4;
            return val;
        }

        private UInt16 Popw()
        {
            UInt16 val = mem.PeekW(reg.A[7]);
            reg.A[7] += 2;
            return val;
        }

        private bool getCond(int cnd, out string cs)
        {
            switch (cnd)
            {
                case 0://true
                    cs = "t";
                    return true;
                case 1://false
                    cs = "f";
                    return false;
                case 2:// hi
                    cs = "hi";
                    return (!reg.C && !reg.Z);
                case 3:// ls
                    cs = "ls";
                    return (reg.C || reg.Z);
                case 4:// cc
                    cs = "cc";
                    return !reg.C;
                case 5:// cs
                    cs = "cs";
                    return reg.C;
                case 6:// ne
                    cs = "ne";
                    return !reg.Z;
                case 7:// eq
                    cs = "eq";
                    return reg.Z;
                case 8:// vc
                    cs = "vc";
                    return !reg.V;
                case 9:// vs
                    cs = "vs";
                    return reg.V;
                case 0xa:// pl
                    cs = "pl";
                    return !reg.N;
                case 0xb:// mi
                    cs = "mi";
                    return reg.N;
                case 0xc:// ge
                    cs = "ge";
                    return (reg.N && reg.V) || (!reg.N && !reg.V);
                case 0xd:// lt
                    cs = "lt";
                    return (reg.N && !reg.V) || (!reg.N && reg.V);
                case 0xe:// gt
                    cs = "gt";
                    return !reg.Z && ((reg.N && reg.V) || (!reg.N && !reg.V));
                case 0xf:// le
                    cs = "le";
                    return reg.Z || (reg.N && !reg.V) || (!reg.N && reg.V);
            }
            throw new NotImplementedException();
        }

    }
}