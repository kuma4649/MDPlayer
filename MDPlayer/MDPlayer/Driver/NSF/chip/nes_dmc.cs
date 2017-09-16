using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_dmc : ISoundChip
    {
        public np_nes_dmc dmc = new np_nes_dmc();
        public np_nes_dmc.NES_DMC chip;

        public override bool Read(uint adr, ref uint val, uint id = 0)
        {
            return dmc.NES_DMC_np_Read(chip, adr, ref val);
        }

        public override uint Render(int[] b)
        {
            return dmc.NES_DMC_org_Render(chip, b);
        }

        public override void Reset()
        {
            dmc.NES_DMC_np_Reset(chip);
        }

        public override void SetClock(double clock)
        {
            dmc.NES_DMC_np_SetClock(chip,clock);
        }

        public override void SetMask(int mask)
        {
            dmc.NES_DMC_np_SetMask(chip, mask);
        }

        public override void SetOption(int id, int val)
        {
            dmc.NES_DMC_np_SetOption(chip, id, val);
        }

        public override void SetRate(double rate)
        {
            dmc.NES_DMC_np_SetRate(chip, rate);
        }

        public override void SetStereoMix(int trk, short mixl, short mixr)
        {
            dmc.NES_DMC_np_SetStereoMix(chip, trk, mixl, mixr);
        }

        public override void Tick(uint clocks)
        {
            dmc.org_Tick(chip, clocks);
        }

        public override bool Write(uint adr, uint val, uint id = 0)
        {
            return dmc.NES_DMC_np_Write(chip, adr, val);
        }

        public void SetMemory(IDevice r)
        {
            dmc.NES_DMC_org_SetMemory(chip, r);
        }
    }
}
