using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDPlayer;

namespace SoundManager
{
    public class Chip
    {
        public bool Use;
        public long Delay;
        public EnmModel Model;
        public EnmDevice Device;
        public int Number;
        public int Hosei;

        public void Move(Chip chip)
        {
            if (chip == null) return;

            this.Use = chip.Use;
            this.Delay = chip.Delay;
            this.Model = chip.Model;
            this.Device = chip.Device;
            this.Number = chip.Number;
            this.Hosei = chip.Hosei;
        }
    }
}
