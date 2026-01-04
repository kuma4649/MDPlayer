using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class niseIOCS
    {
        private Memory68 mem;
        private Register68 reg;
        private Action[] cmdTbl;
        public uint interruptOPM;

        public niseIOCS(Memory68 mem, Register68 reg)
        {
            this.mem = mem;
            this.reg = reg;
            cmdTbl = new Action[]
            {
                //00
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,_DEFCHR,
                //10
                null ,null,null,null, null,null,null,null,  null,_FNTGET,null,null, null,null,null,null,
                //20
                null ,null,null,null, null,null,null,null,  null,null,_B_CLR_ST,null, null,null,null,null,
                //30
                _SET232C ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //40
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //50
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //60
                _ADPCMOUT ,null,null,null, null,null,null,_ADPCMMOD,  _OPMSET,null,_OPMINTST,null, null,null,null,null,
                //70
                _MS_INIT ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //80
                _B_INTVCS ,_B_SUPER,null,null, null,null,null,null,  null,null,_DMAMOVE,null, null,null,null,null,
                //90
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //a0
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //b0
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //c0
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //d0
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //e0
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
                //f0
                null ,null,null,null, null,null,null,null,  null,null,null,null, null,null,null,null,
            };
        }

        public void Call()
        {
            byte n = (byte)reg.D[0];
            if (cmdTbl[n] != null)
            {
                cmdTbl[n]();
            }
            else
            {
                throw new NotImplementedException(string.Format("IOCS 未実装!! [{0:x04}]", n));
            }
        }

        private void _DEFCHR()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _DEFCHR");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            ushort fontSize = (ushort)(reg.GetDl(1) >> 16);
            uint jiscode = reg.GetDw(1);
            uint ptr = reg.GetAl(1);
        }

        private void _FNTGET()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _FNTGET");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            ushort fontSize = (ushort)(reg.GetDl(1) >> 16);
            uint jiscode = reg.GetDw(1);
            uint ptr = reg.GetAl(1);
        }
        
        private void _B_CLR_ST()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _B_CLR_ST");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            byte clrArea = reg.GetDb(1);
        }

        private void _SET232C()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _SET232C");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            ushort settingNumber = reg.GetDw(1);

            reg.SetDl(0, 0x0000_0000);//以前の設定値を返す
        }

        private void _ADPCMOUT()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _ADPCMOUT");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            ushort frq_pan = reg.GetDw(1);
            uint length = reg.GetDl(2);
            uint ptr = reg.GetAl(1);
        }

        private void _ADPCMMOD()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _ADPCMMOD");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            uint mode = reg.GetDl(1);//0 stop 1 pause 2 resume
        }

        private void _OPMSET()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _OPMSET");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            byte rAdr = reg.GetDb(1);
            byte rDat = reg.GetDb(2);
        }

        private void _OPMINTST()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _OPMINTST");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            interruptOPM = reg.GetAl(1);
            reg.SetDl(0,0);
        }

        private void _MS_INIT()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _MS_INIT");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            //マウスの初期化
        }

        private void _B_INTVCS()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _B_INTVCS");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            ushort vctnum = reg.GetDw(1);
            uint ptr = reg.GetAl(1);
            reg.SetDl(0, mem.PeekL((uint)(vctnum * 4)));
            mem.PokeL((uint)(vctnum * 4), ptr);
        }

        private void _B_SUPER()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _B_SUPER");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            if (reg.A[1] == 0)
            {
                reg.SR |= 0x2000;//super
                //reg.D[0] = -;
            }
            else
            {
                reg.SR &= 0xdfff;//user
                reg.D[0] = 0;
            }

        }

        private void _DMAMOVE()
        {
            Log.WriteLine(LogLevel.Trace, "IOCS _DMAMOVE");

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;

            int cm = reg.GetDb(1) & 0x80;
            int a1m = (reg.GetDb(1) & 0x0c) >> 2;
            int a2m = reg.GetDb(1) & 0x03;
            uint size = reg.GetDl(2);
            uint a1 = reg.GetAl(1);
            uint a2 = reg.GetAl(2);

            while (size > 0) {
                uint src = cm == 0 ? a1 : a2;
                uint dst = cm == 0 ? a2 : a1;
                byte b = mem.PeekB(src);
                mem.PokeB(dst, b);
                a1 = (uint)(a1 + (a1m == 0 ? 0 : (a1m == 1 ? 1 : -1)));
                a2 = (uint)(a2 + (a2m == 0 ? 0 : (a2m == 1 ? 1 : -1)));
                size--;
            }
        }
    }
}
