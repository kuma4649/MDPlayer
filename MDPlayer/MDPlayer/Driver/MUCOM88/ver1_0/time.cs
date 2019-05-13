using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MUCOM88.ver1_0
{
    public class time
    {
        public Mem Mem = null;
        public Z80 Z80 = null;
        public PC88 PC88 = null;

        //プログラム書き換えコード対策
        private ushort CHJ_VAL = 0x0E808;
        //

        //TXTEND CHECK & TIME PRINT
        //    ORG	0E400H

        public ushort NOTSB2 = 0x0FFF8;
        public ushort TXTEND = 0x0EB18;
        public ushort S_ILVL = 0x0E6C3;
        public ushort TIME = 0x0B000 + 3 * 17;

        // --	BASIC ﾉ TEXTENDﾁｪｯｸ ｦ vrtcﾜﾘｺﾐ ﾆ ｾｯﾃｲ	--

        public void STTE()
        {
            Z80.HL = Mem.LD_16(0x0F302);
            //Mem.LD_16(CHJ + 1, Z80.HL);
            CHJ_VAL = Z80.HL;

            //  DI

            //TODO:
            //Z80.HL = CHECK;
            Mem.LD_16(0x0F302, Z80.HL);

            //  EI

            //  RET
        }

        // --	CHECK ﾙｰﾁﾝ(8D00Hﾍ ﾃﾝｿｳｻﾚﾙ)  --

        public void CHECK()
        {
            //	DI
            Mem.stack.Push(Z80.AF);
            Mem.stack.Push(Z80.HL);
            Mem.stack.Push(Z80.DE);
            Mem.stack.Push(Z80.BC);
            Z80.HL = 0; //VRTCC;
            //Mem.LD_8(Z80.HL, (byte)(Mem.LD_8(Z80.HL) - 1));
            VRTCC--;
            //if (Mem.LD_8(Z80.HL) != 0)
            if (VRTCC != 0)
            {
                goto CH3;
            }
            //Mem.LD_8(Z80.HL, 6);
            VRTCC = 6;

        //CH2:
            PC88.CALL(TIME);
        CH3:
            Z80.A = Mem.LD_8(S_ILVL);
            PC88.OUT(0xe4, Z80.A);

            Z80.BC = Mem.stack.Pop();
            Z80.DE = Mem.stack.Pop();
            Z80.HL = Mem.stack.Pop();
            Z80.AF = Mem.stack.Pop();
            //   EI

            //TODO:
            //CHJ:
            //goto CHJ_VAL;// 0x0E808;
        }

        public byte VRTCC = 30;

    }
}
