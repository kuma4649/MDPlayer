using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_fds : ISoundChip
    {
        public np_nes_fds fds = new np_nes_fds();
        public np_nes_fds.NES_FDS chip;

        public override bool Read(uint adr, ref uint val, uint id = 0)
        {
            return fds.NES_FDS_Read(chip, adr, ref val);
        }

        public override uint Render(int[] b)
        {
            return fds.NES_FDS_org_Render(chip, b);
        }

        public override void Reset()
        {
            fds.NES_FDS_Reset(chip);
        }

        public override void SetClock(double clock)
        {
            fds.NES_FDS_SetClock(chip, clock);
        }

        public override void SetMask(int mask)
        {
            fds.NES_FDS_SetMask(chip, mask);
        }

        public override void SetOption(int id, int val)
        {
            fds.NES_FDS_SetOption(chip, id, val);
        }

        public override void SetRate(double rate)
        {
            fds.NES_FDS_SetRate(chip, rate);
        }

        public override void SetStereoMix(int trk, short mixl, short mixr)
        {
            fds.NES_FDS_SetStereoMix(chip, trk, mixl, mixr);
        }

        public override void Tick(uint clocks)
        {
            fds.Tick(chip, clocks);
        }

        public override bool Write(uint adr, uint val, uint id = 0)
        {
            return fds.NES_FDS_Write(chip, adr, val);
        }
    }
}
