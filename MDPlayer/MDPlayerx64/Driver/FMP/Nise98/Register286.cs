using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class Register286
    {
        // eRegs     sRegs
        // 0 ... AX  0 ... ES
        // 1 ... CX  1 ... CS
        // 2 ... DX  2 ... SS
        // 3 ... BX  3 ... DS
        // 4 ... SP
        // 5 ... BP
        // 6 ... SI
        // 7 ... DI
        public Int16[] eRegs = new Int16[8];
        public Int16[] sRegs = new Int16[4];
        public Int16 IP = 0;
        public Int16 FLAG = unchecked((short)0x8000);

        public Int16 ES
        {
            get
            {
                return sRegs[0];
            }
            set
            {
                sRegs[0] = value;
            }
        }

        public Int16 CS
        {
            get
            {
                return sRegs[1];
            }
            set
            {
                sRegs[1] = value;
            }
        }

        public Int16 SS
        {
            get
            {
                return sRegs[2];
            }
            set
            {
                sRegs[2] = value;
            }
        }

        public Int16 DS
        {
            get
            {
                return sRegs[3];
            }
            set
            {
                sRegs[3] = value;
            }
        }

        public Int16 AX
        {
            get
            {
                return eRegs[0];
            }
            set
            {
                eRegs[0] = value;
            }
        }

        public Int16 CX
        {
            get
            {
                return eRegs[1];
            }
            set
            {
                eRegs[1] = value;
            }
        }

        public Int16 DX
        {
            get
            {
                return eRegs[2];
            }
            set
            {
                eRegs[2] = value;
            }
        }

        public Int16 BX
        {
            get
            {
                return eRegs[3];
            }
            set
            {
                eRegs[3] = value;
            }
        }

        public Int16 SP
        {
            get
            {
                return eRegs[4];
            }
            set
            {
                eRegs[4] = value;
            }
        }

        public Int16 BP
        {
            get
            {
                return eRegs[5];
            }
            set
            {
                eRegs[5] = value;
            }
        }

        public Int16 SI
        {
            get
            {
                return eRegs[6];
            }
            set
            {
                eRegs[6] = value;
            }
        }

        public Int16 DI
        {
            get
            {
                return eRegs[7];
            }
            set
            {
                eRegs[7] = value;
            }
        }

        public Byte AL
        {
            get
            {
                return (Byte)eRegs[0];
            }
            set
            {
                eRegs[0] &= unchecked((Int16)0xff00);
                eRegs[0] |= (Int16)value;
            }
        }

        public Byte AH
        {
            get
            {
                return (Byte)(eRegs[0] >> 8);
            }
            set
            {
                eRegs[0] &= unchecked((Int16)0x00ff);
                eRegs[0] |= (Int16)(value << 8);
            }
        }

        public Byte CL
        {
            get
            {
                return (Byte)eRegs[1];
            }
            set
            {
                eRegs[1] &= unchecked((Int16)0xff00);
                eRegs[1] |= (Int16)value;
            }
        }

        public Byte CH
        {
            get
            {
                return (Byte)(eRegs[1] >> 8);
            }
            set
            {
                eRegs[1] &= unchecked((Int16)0x00ff);
                eRegs[1] |= (Int16)(value << 8);
            }
        }

        public Byte DL
        {
            get
            {
                return (Byte)eRegs[2];
            }
            set
            {
                eRegs[2] &= unchecked((Int16)0xff00);
                eRegs[2] |= (Int16)value;
            }
        }

        public Byte DH
        {
            get
            {
                return (Byte)(eRegs[2] >> 8);
            }
            set
            {
                eRegs[2] &= unchecked((Int16)0x00ff);
                eRegs[2] |= (Int16)(value << 8);
            }
        }

        public Byte BL
        {
            get
            {
                return (Byte)eRegs[3];
            }
            set
            {
                eRegs[3] &= unchecked((Int16)0xff00);
                eRegs[3] |= (Int16)value;
            }
        }

        public Byte BH
        {
            get
            {
                return (Byte)(eRegs[3] >> 8);
            }
            set
            {
                eRegs[3] &= unchecked((Int16)0x00ff);
                eRegs[3] |= (Int16)(value << 8);
            }
        }

        public int CS_IP
        {
            get { return ((UInt16)CS << 4) + (UInt16)IP; }
        }

        public int DS_DX
        {
            get { return ((UInt16)DS << 4) + (UInt16)DX; }
        }

        public int DS_SI
        {
            get { return ((UInt16)DS << 4) + (UInt16)SI; }
        }

        public int DS_DI
        {
            get { return ((UInt16)DS << 4) + (UInt16)DI; }
        }

        public int ES_DI
        {
            get { return ((UInt16)ES << 4) + (UInt16)DI; }
        }

        public int SS_SP
        {
            get { return ((UInt16)SS << 4) + (UInt16)SP; }
        }

        //CF bit0
        public bool CF
        {
            get { return ((FLAG & (1 << 0)) != 0); }
            set
            {
                FLAG &= ~(1 << 0);
                FLAG |= (Int16)(value ? (1 << 0) : 0);
            }
        }

        //PF bit2
        public bool PF
        {
            get { return ((FLAG & (1 << 2)) != 0); }
            set
            {
                FLAG &= ~(1 << 2);
                FLAG |= (Int16)(value ? (1 << 2) : 0);
            }
        }

        //AF bit4
        public bool AF
        {
            get { return ((FLAG & (1 << 4)) != 0); }
            set
            {
                FLAG &= ~(1 << 4);
                FLAG |= (Int16)(value ? (1 << 4) : 0);
            }
        }

        //ZF bit6
        public bool ZF
        {
            get { return ((FLAG & (1 << 6)) != 0); }
            set
            {
                FLAG &= ~(1 << 6);
                FLAG |= (Int16)(value ? (1 << 6) : 0);
            }
        }

        //SF bit7
        public bool SF
        {
            get { return ((FLAG & (1 << 7)) != 0); }
            set
            {
                FLAG &= ~(1 << 7);
                FLAG |= (Int16)(value ? (1 << 7) : 0);
            }
        }

        //TF bit8
        public bool TF
        {
            get { return ((FLAG & (1 << 8)) != 0); }
            set
            {
                FLAG &= ~(1 << 8);
                FLAG |= (Int16)(value ? (1 << 8) : 0);
            }
        }

        //IF bit9
        public bool IF
        {
            get { return ((FLAG & (1 << 9)) != 0); }
            set
            {
                FLAG &= ~(1 << 9);
                FLAG |= (Int16)(value ? (1 << 9) : 0);
            }
        }

        //DF bit10
        public bool DF
        {
            get { return ((FLAG & (1 << 10)) != 0); }
            set
            {
                FLAG &= ~(1 << 10);
                FLAG |= (Int16)(value ? (1 << 10) : 0);
            }
        }

        //OF bit11
        public bool OF
        {
            get { return ((FLAG & (1 << 11)) != 0); }
            set
            {
                FLAG &= ~(1 << 11);
                FLAG |= (Int16)(value ? (1 << 11) : 0);
            }
        }


        public int AuxVal, OverVal, SignVal, ZeroVal, CarryVal, DirVal;      /* 0 or non-0 valued flags */
        public byte ParityVal;


        public void SetSZPFb(byte ans)
        {
            SignVal = (sbyte)ans;
            SF = (SignVal < 0);
            ZeroVal = ans;
            ZF = ZeroVal == 0;
            ParityVal = ans;
            PF = parity_table[ParityVal & 0xff];
        }

        public void SetSZPFw(ushort ans)
        {
            SignVal = (short)ans;
            SF = (SignVal < 0);
            ZeroVal = ans;
            ZF = ZeroVal == 0;
            ParityVal = (byte)ans;
            PF = parity_table[ParityVal & 0xff];
        }

        public void SetCFb(UInt16 a)
        {
            CarryVal = (a) & 0x100;
            CF = CarryVal != 0;
        }

        public void SetCFw(UInt32 a)
        {
            CarryVal = (int)((a) & 0x10000);
            CF = CarryVal != 0;
        }

        public void SetAF(byte a, byte b, byte ans)
        {
            AuxVal = ((ans) ^ ((a) ^ (b))) & 0x10;
            AF = AuxVal != 0;
        }

        // ans = a - b
        // の時のOF判定
        // 事前にSFの判定を行っておくこと
        public void SetOFwSub(ushort a, ushort b, ushort ans)
        {
            //OF = SF
            //    ? ((b > 0 && ans > a) || (b < 0 && ans < a))
            //    : ans > a;

            OverVal = ((b ^ a) & (b ^ ans) & 0x8000);
            OF = OverVal != 0;
        }
        public void SetOFbSub(byte a, byte b, byte ans)
        {
            //OF = SF
            //    ? ((b > 0 && ans > a) || (b < 0 && ans < a))
            //    : ans > a;
            OverVal = ((b ^ a) & (b ^ ans) & 0x80);
            OF = OverVal != 0;
        }
        public void SetOFwAdd(ushort a, ushort b, ushort ans)
        {
            //OF = SF
            //    ? ((a >= 0 && ans < b) || (a < 0 && ans > b))
            //    : (ans < a || ans < b);
            OverVal = (((ans) ^ (a)) & ((ans) ^ (b)) & 0x8000);
        }
        public void SetOFbAdd(byte a, byte b, byte ans)
        {
            //OF = SF
            //    ? ((a >= 0 && ans < b) || (a < 0 && ans > b))
            //    : (ans < a || ans < b);
            OverVal = (((ans) ^ (a)) & ((ans) ^ (b)) & 0x80);
        }

        public override string ToString()
        {
            return string.Format(
                "AX:{0:X04} CX:{1:X04} DX:{2:X04} BX:{3:X04} SP:{4:X04} BP:{5:X04} SI:{6:X04} DI:{7:X04} \r\n"
                + "ES:{8:X04} CS:{9:X04} SS:{10:X04} DS:{11:X04} IP:{12:X04} FLAG:{13}",
                AX, CX, DX, BX, SP, BP, SI, DI,
                ES, CS, SS, DS, IP, string.Format(
                    "{16:X04}[{15}{14}{13}{12}{11}{10}{9}{8}{7}{6}{5}{4}{3}{2}{1}{0}]",
                    CF ? "C" : "-",
                    ".",
                    PF ? "P" : "-",
                    ".",
                    AF ? "A" : "-",
                    ".",
                    ZF ? "Z" : "-",
                    SF ? "S" : "-",
                    TF ? "T" : "-",
                    IF ? "I" : "-",
                    DF ? "D" : "-",
                    OF ? "O" : "-",
                    ".",
                    ".",
                    ".",
                    ".",
                    FLAG
                )
                );
        }

        public Register286()
        {
            parity_table = new bool[256];
            for (int i = 0; i < 256; i++)
            {
                int c = 0;
                for (int j = 0; j < 8; j++)
                {
                    if ((i & (1 << j)) != 0)
                        c++;
                }

                parity_table[i] = ((c & 1) == 0);
            }
        }

        private Stack<Int16> regStack = new Stack<short>();
        private bool[] parity_table;

    }
}
