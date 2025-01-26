using MDPlayer.Driver.GBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.GBS
{
    public partial class CPU
    {
        public Registers reg = new Registers();
        public Memory mem = null;
        public int clock = 4194304;
        public bool isHalt = false;
        public bool isStop = false;
        public string nimo = "";
        public bool cbSwitch = false;
        public int ime = 1;
        public long vgmFrameCounter;

        public CPU(int clock, Memory memory)
        {
            this.clock = clock;
            this.mem = memory;
            initInsts();
        }

        public void Init()
        {
            reg.pc = 0x100;
            isHalt = false;
            isStop = false;
            cbSwitch = false;
            ime = 1;
        }

        public int ExecuteOneStep()
        {
            int pc;
            int cycle = 0;


            do
            {
                pc = reg.pc;

                //命令読み込み
                ushort c = mem.PeekB(reg.pc);
                reg.pc++;
                if (cbSwitch)
                {
                    c += 0x100;
                    cbSwitch = false;
                }
                //命令実行
                cycle = insts[c].meth();

#if DEBUG
                string smem = "";
                int len = Math.Min(insts[c].length, 4);
                for (int i = 0; i < len; i++)
                    smem += string.Format("{0:X02} ", mem.PeekB(pc + i));
                smem += "            ".Substring(len * 3);
                Console.WriteLine("${0:X04} : {1,-20} {2} {3} {4} cycle:{5}",
                    pc, nimo, smem, reg, mem.GetBank(), cycle);
                nimo = "";
#endif
                if (cycle == 0) throw new NotImplementedException();
            } while (cbSwitch);

            return cycle;
        }

    }
}
