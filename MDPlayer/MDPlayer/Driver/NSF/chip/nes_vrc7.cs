using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_vrc7 : ISoundChip
    {
        protected Int32 mask;
        protected UInt32 patch_set;
        protected Int32[][] sm = new Int32[2][] { new Int32[6], new Int32[6] }; // stereo mix
        protected Int16[] buf = new Int16[2];
        protected emu2413.OPLL opll;
        protected emu2413 emu2413 = new emu2413();
        protected UInt32 divider; // clock divider
        protected double clock, rate;
        protected TrackInfoBasic[] trkinfo = new TrackInfoBasic[6];

        public nes_vrc7()
        {
            patch_set = (UInt32)emu2413.OPLL_TONE_ENUM.OPLL_VRC7_RW_TONE;

            opll = emu2413.OPLL_new(3579545, (UInt32)common.SampleRate);
            emu2413.OPLL_reset_patch(opll, (Int32)patch_set);
            SetClock(common.NsfClock);// DEFAULT_CLOCK);

            for (int c = 0; c < 2; ++c)
                for (int t = 0; t < 6; ++t)
                    sm[c][t] = 128;
        }

        ~nes_vrc7()
        {
            opll = null;
            //OPLL_delete(opll);
        }

        public void SetPatchSet(UInt32 p)
        {
            patch_set = p;
        }

        public override void SetClock(double c)
        {
            clock = c / 36;
        }

        public override void SetRate(double r)
        {
            //rate = r ? r : DEFAULT_RATE;
            //(void)r; // rate is ignored
            rate = 49716;
            emu2413.OPLL_set_quality(opll, 1); // quality always on (not really a CPU hog)
            emu2413.OPLL_set_rate(opll, (UInt32)rate);
        }

        public override void Reset()
        {
            for (UInt32 i = 0; i < 0x40; ++i)
            {
                Write(0x9010, i);
                Write(0x9030, 0);
            }

            divider = 0;
            emu2413.OPLL_reset_patch(opll, (Int32)patch_set);
            emu2413.OPLL_reset(opll);
        }

        public override void SetStereoMix(Int32 trk, Int16 mixl, Int16 mixr)
        {
            if (trk < 0) return;
            if (trk > 5) return;
            sm[0][trk] = mixl;
            sm[1][trk] = mixr;
        }

        public ITrackInfo GetTrackInfo(int trk)
        {
            if (opll != null && trk < 6)
            {
                trkinfo[trk].max_volume = 15;
                trkinfo[trk].volume = 15 - ((opll.reg[0x30 + trk]) & 15);
                trkinfo[trk]._freq = (UInt32)(opll.reg[0x10 + trk] + ((opll.reg[0x20 + trk] & 1) << 8));
                int blk = (opll.reg[0x20 + trk] >> 1) & 7;
                trkinfo[trk].freq = clock * trkinfo[trk]._freq / (double)(0x80000 >> blk);
                trkinfo[trk].tone = (opll.reg[0x30 + trk] >> 4) & 15;
                trkinfo[trk].key = (opll.reg[0x20 + trk] & 0x10) != 0 ? true : false;
                return trkinfo[trk];
            }
            else
                return null;
        }

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id = 0)
        {
            if (adr == 0x9010)
            {
                emu2413.OPLL_writeIO(opll, 0, val);
                return true;
            }
            if (adr == 0x9030)
            {
                emu2413.OPLL_writeIO(opll, 1, val);
                return true;
            }
            else
                return false;
        }

        public override bool Read(UInt32 adr, ref UInt32 val, UInt32 id = 0)
        {
            return false;
        }

        public override void Tick(UInt32 clocks)
        {
            divider += clocks;
            while (divider >= 36)
            {
                divider -= 36;
                emu2413.OPLL_calc(opll);
            }
        }

        public override UInt32 Render(Int32[] b)//b[2])
        {
            b[0] = b[1] = 0;
            for (int i = 0; i < 6; ++i)
            {
                Int32 val = (mask & (1 << i)) != 0 ? 0 : opll.slot[(i << 1) | 1].output[1];
                b[0] += val * sm[0][i];
                b[1] += val * sm[1][i];
            }
            b[0] >>= (7 - 4);
            b[1] >>= (7 - 4);

            // master volume adjustment
            const Int32 MASTER = (Int32)(0.8 * 256.0);
            b[0] = (b[0] * MASTER) >> 5;// 8;
            b[1] = (b[1] * MASTER) >> 5;// 8;

            return 2;
        }


        public override void SetMask(int m)
        {
            mask = m;
            if (opll != null) emu2413.OPLL_setMask(opll, (UInt32)m);
        }

        public override void SetOption(int id, int val)
        {
            throw new NotImplementedException();
        }
    }
}
