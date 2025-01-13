namespace MDPlayer.Driver.ZMS.nise68
{
    public class Register68
    {
        private UInt32[] _D = new uint[8];
        public UInt32[] D { get => _D; set => _D = value; }
        private A _A = new();
        public A A { get => _A; set => _A = value; }
        public UInt32 PC;
        public UInt32 USP { get => A.USP; set => A.USP = value; }
        public UInt32 SSP { get => A.SSP; set => A.SSP = value; }
        public UInt16 SR { get => A.SR; set => A.SR = value; } 
        public UInt16 SRbk { get => A.SRbk; set => A.SRbk = value; }
        public byte CCR { get => (byte)A.SR; set => A.SR = (UInt16)((A.SR & 0xffe0) | (byte)(value & 0x1f)); }
        public bool X { get => (A.SR & 0x0010) != 0; set { A.SR = (UInt16)((A.SR & 0xffef) | (value ? 0x0010 : 0x00)); } }
        public bool N { get => (A.SR & 0x0008) != 0; set { A.SR = (UInt16)((A.SR & 0xfff7) | (value ? 0x0008 : 0x00)); } }
        public bool Z { get => (A.SR & 0x0004) != 0; set { A.SR = (UInt16)((A.SR & 0xfffb) | (value ? 0x0004 : 0x00)); } }
        public bool V { get => (A.SR & 0x0002) != 0; set { A.SR = (UInt16)((A.SR & 0xfffd) | (value ? 0x0002 : 0x00)); } }
        public bool C { get => (A.SR & 0x0001) != 0; set { A.SR = (UInt16)((A.SR & 0xfffe) | (value ? 0x0001 : 0x00)); } }
        public bool S { get => (A.SR & 0x2000) != 0; set { A.SR = (UInt16)((A.SR & 0xdfff) | (value ? 0x2000 : 0x00)); } }
        public bool T { get => (A.SR & 0x8000) != 0; set { A.SR = (UInt16)((A.SR & 0x7fff) | (value ? 0x8000 : 0x00)); } }

        public override string ToString()
        {
            return string.Format(
                //"D0:{0:X08} D1:{1:X08} D2:{2:X08} D3:{3:X08} D4:{4:X08} D5:{5:X08} D6:{6:X08} D7:{7:X08}\r\n"
                //+ "A0:{8:X08} A1:{9:X08} A2:{10:X08} A3:{11:X08} A4:{12:X08} A5:{13:X08} A6:{14:X08} A7:{15:X08}\r\n"
                //+ "PC:{16:X08} USP:{17:X08} SSP:{18:X08} SR:{19:X04}  X:{22} N:{23} Z:{24} V:{25} C:{26} {21} {20}",
                "D0-D7={0:X08},{1:X08},{2:X08},{3:X08},{4:X08},{5:X08},{6:X08},{7:X08}\r\n"
                + "A0-A7={8:X08},{9:X08},{10:X08},{11:X08},{12:X08},{13:X08},{14:X08},{15:X08}\r\n"
                + "  PC={16:X08}    SR={19:X04}",
                D[0], D[1], D[2], D[3], D[4], D[5], D[6], D[7],
                A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7],
                PC, USP, SSP, SR
                , ((SR & 0x8000) != 0 ? "[Trace]" : "")
                , ((SR & 0x2000) != 0 ? "[Super]" : "[User]")
                , X ? "*" : "."
                , N ? "*" : "."
                , Z ? "*" : "."
                , V ? "*" : "."
                , C ? "*" : "."
                );
        }

        public void SetC(ushort before ,ushort after)
        {
            C = ((before & 0xff00) != (after & 0xff00));
        }

        public void SetC(UInt32 before, UInt32 after)
        {
            C = ((before & 0xffff0000) != (after & 0xffff0000));
        }

        public void SetC(UInt64 before, UInt64 after)
        {
            C = ((before & 0xffff_ffff_0000_0000) != (after & 0xffff_ffff_0000_0000));
        }

        public void SetV(byte before, byte after)
        {
            V = ((before & 0x80) != (after & 0x80));
        }
        public void SetV(ushort before, ushort after)
        {
            V = ((before & 0x8000) != (after & 0x8000));
        }
        public void SetV(UInt32 before, UInt32 after)
        {
            V = ((before & 0x8000_0000) != (after & 0x8000_0000));
        }

        public void SetZ(byte after)
        {
            Z = (after == 0);
        }

        public void SetZ(ushort after)
        {
            Z = (after == 0);
        }

        public void SetZ(UInt32 after)
        {
            Z = (after == 0);
        }

        public void SetN(byte after)
        {
            N = ((after & 0x80) != 0);
        }
        public void SetN(ushort after)
        {
            N = ((after & 0x8000) != 0);
        }
        public void SetN(UInt32 after)
        {
            N = ((after & 0x8000_0000) != 0);
        }

        public void SetDb(int n,byte val)
        {
            D[n] = (D[n] & 0xffff_ff00) | val;
        }
        public void SetDw(int n, ushort val)
        {
            D[n] = (D[n] & 0xffff_0000) | val;
        }
        public void SetDl(int n, UInt32 val)
        {
            D[n] = val;
        }

        public byte GetDb(int n)
        {
            return (byte)D[n];
        }

        public ushort GetDw(int n)
        {
            return (ushort)D[n];
        }

        public UInt32 GetDl(int n)
        {
            return D[n];
        }

        public void SetAb(int n, byte val)
        {
            A[n] = (uint)(int)(sbyte)val;
        }
        public void SetAw(int n, ushort val)
        {
            A[n] = (uint)(int)(short)val;
        }
        public void SetAl(int n, UInt32 val)
        {
            A[n] = val;
        }

        public byte GetAb(int n)
        {
            return (byte)A[n];
        }
        public ushort GetAw(int n)
        {
            return (ushort)A[n];
        }
        public uint GetAl(int n)
        {
            return A[n];
        }


        //
        //run68を参考
        //

        public void SetVadd(byte src, byte dst, byte after)
        {
            bool s = ((src & 0x80) != 0);
            bool d = ((dst & 0x80) != 0);
            bool a = ((after & 0x80) != 0);
            V = (s && d && !a) || (!s && !d && a);
        }

        public void SetVadd(ushort src, ushort dst, ushort after)
        {
            bool s = ((src & 0x8000) != 0);
            bool d = ((dst & 0x8000) != 0);
            bool a = ((after & 0x8000) != 0);
            V = (s && d && !a) || (!s && !d && a);
        }

        public void SetVadd(uint src, uint dst, uint after)
        {
            bool s = ((src & 0x8000_0000) != 0);
            bool d = ((dst & 0x8000_0000) != 0);
            bool a = ((after & 0x8000_0000) != 0);
            V = (s && d && !a) || (!s && !d && a);
        }

        public void SetVcmp(byte src, byte dst, byte after)
        {
            bool s = ((src & 0x80) != 0);
            bool d = ((dst & 0x80) != 0);
            bool a = ((after & 0x80) != 0);
            V = (!s && d && !a) || (s && !d && a);
        }

        public void SetVcmp(ushort src, ushort dst, ushort after)
        {
            bool s = ((src & 0x8000) != 0);
            bool d = ((dst & 0x8000) != 0);
            bool a = ((after & 0x8000) != 0);
            V = (!s && d && !a) || (s && !d && a);
        }

        public void SetVcmp(uint src, uint dst, uint after)
        {
            bool s = ((src & 0x8000_0000) != 0);
            bool d = ((dst & 0x8000_0000) != 0);
            bool a = ((after & 0x8000_0000) != 0);
            V = (!s && d && !a) || (s && !d && a);
        }

        public void SetVneg(byte dst, byte after)
        {
            bool d = ((dst & 0x80) != 0);
            bool a = ((after & 0x80) != 0);
            V = (d && a);
        }

        public void SetVneg(ushort dst, ushort after)
        {
            bool d = ((dst & 0x8000) != 0);
            bool a = ((after & 0x8000) != 0);
            V = (d && a);
        }

        public void SetVneg(uint dst, uint after)
        {
            bool d = ((dst & 0x8000_0000) != 0);
            bool a = ((after & 0x8000_0000) != 0);
            V = (d && a);
        }




        public void SetCadd(byte src, byte dst, byte after)
        {
            bool s = ((src & 0x80) != 0);
            bool d = ((dst & 0x80) != 0);
            bool a = ((after & 0x80) != 0);
            C = (s && d) || (d && !a) || (s && !a);
        }

        public void SetCadd(ushort src, ushort dst, ushort after)
        {
            bool s = ((src & 0x8000) != 0);
            bool d = ((dst & 0x8000) != 0);
            bool a = ((after & 0x8000) != 0);
            C = (s && d) || (d && !a) || (s && !a);
        }

        public void SetCadd(uint src, uint dst, uint after)
        {
            bool s = ((src & 0x8000_0000) != 0);
            bool d = ((dst & 0x8000_0000) != 0);
            bool a = ((after & 0x8000_0000) != 0);
            C = (s && d) || (d && !a) || (s && !a);
        }

        public void SetCcmp(byte src, byte dst, byte after)
        {
            bool s = ((src & 0x80) != 0);
            bool d = ((dst & 0x80) != 0);
            bool a = ((after & 0x80) != 0);
            C = (s && !d) || (!d && a) || (s && a);
        }

        public void SetCcmp(ushort src, ushort dst, ushort after)
        {
            bool s = ((src & 0x8000) != 0);
            bool d = ((dst & 0x8000) != 0);
            bool a = ((after & 0x8000) != 0);
            C = (s && !d) || (!d && a) || (s && a);
        }

        public void SetCcmp(uint src, uint dst, uint after)
        {
            bool s = ((src & 0x8000_0000) != 0);
            bool d = ((dst & 0x8000_0000) != 0);
            bool a = ((after & 0x8000_0000) != 0);
            C = (s && !d) || (!d && a) || (s && a);
        }

        public void SetCneg(byte dst, byte after)
        {
            bool d = ((dst & 0x80) != 0);
            bool a = ((after & 0x80) != 0);
            V = (d || a);
        }

        public void SetCneg(ushort dst, ushort after)
        {
            bool d = ((dst & 0x8000) != 0);
            bool a = ((after & 0x8000) != 0);
            V = (d || a);
        }

        public void SetCneg(uint dst, uint after)
        {
            bool d = ((dst & 0x8000_0000) != 0);
            bool a = ((after & 0x8000_0000) != 0);
            V = (d || a);
        }
    }

    public class A
    {
        private UInt32[] items = new uint[8];    // Itemsプロパティのフィールド
        public UInt32 USP;
        public UInt32 SSP;
        public UInt16 SR;
        public UInt16 SRbk;

        public UInt32 this[int index]  // インデクサ
        {
            get
            {
                if (index == 7)
                {
                    if ((SR & 0x2000) != 0) return SSP;
                    else return USP;
                }
                return items[index];
            }
            set
            {
                if (index == 7)
                {
                    if ((SR & 0x2000) != 0) SSP = value;
                    else USP = value;
                }
                else
                    items[index] = value;
            }
        }
    }
}