using MDPlayer.Driver.FMP.Nise98;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    internal class NiseInt08Timer
    {
        private Nise286 cpu;
        private bool enable = false;
        private int counter = 0;
        private int wcounter = 300;
        private int INTnumber = 8;



        public NiseInt08Timer(Nise286 cpu, int INTnumber = 8)
        {
            this.cpu = cpu;
            this.INTnumber = INTnumber;
        }

        public void Start()
        {
            enable = true;
            counter = wcounter;
        }

        public void StepExecute()
        {
            if ((cpu.w_mmsk & 0x01) != 0) return;
            if (!enable) return;

            wcounter--;
            if (wcounter > 0) return;
            wcounter = counter;
            cpu.interruptTrigger[INTnumber] = true;
        }
    }
}
