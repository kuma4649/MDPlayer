using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_apu :  ISoundChip
    {
        public np_nes_apu apu = new np_nes_apu();
        public np_nes_apu.NES_APU chip;

        public override bool Read(uint adr, ref uint val, uint id = 0)
        {
            return apu.NES_APU_np_Read(chip, adr, ref val);
        }

        public override uint Render(int[] b)
        {
            return apu.NES_APU_org_Render(chip, b);
        }

        public override void Reset()
        {
            apu.NES_APU_np_Reset(chip);
        }

        public override void SetClock(double clock)
        {
            apu.NES_APU_np_SetClock(chip, clock);
        }

        public override void SetMask(int mask)
        {
            apu.NES_APU_np_SetMask(chip, mask);
        }

        public override void SetOption(int id, int val)
        {
            apu.NES_APU_np_SetOption(chip, id, val);
        }

        public override void SetRate(double rate)
        {
            apu.NES_APU_np_SetRate(chip, rate);
        }

        public override void SetStereoMix(int trk, short mixl, short mixr)
        {
            apu.NES_APU_np_SetStereoMix(chip, trk, mixl, mixr);
        }

        public override void Tick(uint clocks)
        {
            apu.Tick(chip, clocks);
        }

        public override bool Write(uint adr, uint val, uint id = 0)
        {
            return apu.NES_APU_np_Write(chip, adr, val);
        }
    }
}
