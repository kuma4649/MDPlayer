using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_vrc6 : ISoundChip
    {
        public const double DEFAULT_CLOCK = 1789772.0;
        public const int DEFAULT_RATE = 44100;

        protected UInt32[] counter=new UInt32[3]; // frequency divider
        protected UInt32[] phase = new UInt32[3];   // phase counter
        protected UInt32[] freq2 = new UInt32[3];   // adjusted frequency
        protected Int32 count14;       // saw 14-stage counter

        protected Int32 mask;
        protected Int32[][] sm = new Int32[2][] { new Int32[3],new Int32[3] }; // stereo mix
        protected Int32[] duty = new Int32[2];
        protected Int32[] volume = new Int32[3];
        protected Int32[] enable = new Int32[3];
        protected Int32[] gate = new Int32[3];
        protected UInt32[] freq = new UInt32[3];
        protected bool halt;
        protected Int32 freq_shift;
        protected double clock, rate;
        protected Int32[] _out = new Int32[3];
        protected TrackInfoBasic[] trkinfo = new TrackInfoBasic[3];


        public nes_vrc6()
        {
            SetClock(DEFAULT_CLOCK);
            SetRate(DEFAULT_RATE);

            halt = false;
            freq_shift = 0;

            for (int c = 0; c < 2; ++c)
                for (int t = 0; t < 3; ++t)
                    sm[c][t] = 128;
        }

        ~nes_vrc6()
        {
        }

        public override void SetStereoMix(Int32 trk, Int16 mixl, Int16 mixr)
        {
            if (trk < 0) return;
            if (trk > 2) return;
            sm[0][trk] = mixl;
            sm[1][trk] = mixr;
        }

        public ITrackInfo GetTrackInfo(Int32 trk)
        {
            if (trk < 2)
            {
                trkinfo[trk].max_volume = 15;
                trkinfo[trk].volume = volume[trk];
                trkinfo[trk]._freq = freq2[trk];
                trkinfo[trk].freq = freq2[trk] != 0 ? clock / 16 / (freq2[trk] + 1) : 0;
                trkinfo[trk].tone = duty[trk];
                trkinfo[trk].key = (volume[trk] > 0) && enable[trk] != 0 && gate[trk] == 0;
                return trkinfo[trk];
            }
            else if (trk == 2)
            {
                trkinfo[2].max_volume = 255;
                trkinfo[2].volume = volume[2];
                trkinfo[2]._freq = freq2[2];
                trkinfo[2].freq = freq2[2] != 0 ? clock / 14 / (freq2[2] + 1) : 0;
                trkinfo[2].tone = -1;
                trkinfo[2].key = (enable[2] > 0);
                return trkinfo[2];
            }
            else
                return null;
        }

        public override void SetClock(double c)
        {
            clock = c;
        }

        public override void SetRate(double r)
        {
            rate = r != 0 ? r : DEFAULT_RATE;
        }

        public override void Reset()
        {
            Write(0x9003, 0);
            for (int i = 0; i < 3; i++)
            {
                Write((UInt32)(0x9000 + i), 0);
                Write((UInt32)(0xa000 + i), 0);
                Write((UInt32)(0xb000 + i), 0);
            }
            count14 = 0;
            mask = 0;
            counter[0] = 0;
            counter[1] = 0;
            counter[2] = 0;
            phase[0] = 0;
            phase[0] = 1;
            phase[0] = 2;
        }

        Int16[][] sqrtbl = new Int16[8][] {
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1},
                new Int16[16]{0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1}
            };

        private Int16 calc_sqr(Int32 i, UInt32 clocks)
        {
            if (enable[i] == 0)
                return 0;

            if (!halt)
            {
                counter[i] += clocks;
                while (counter[i] > freq2[i])
                {
                    phase[i] = (phase[i] + 1) & 15;
                    counter[i] -= (freq2[i] + 1);
                }
            }

            return (Int16)((gate[i] != 0
              || sqrtbl[duty[i]][phase[i]] != 0) ? volume[i] : 0);
        }

        private Int16 calc_saw(UInt32 clocks)
        {
            if (enable[2] == 0)
                return 0;

            if (!halt)
            {
                counter[2] += clocks;
                while (counter[2] > freq2[2])
                {
                    counter[2] -= (freq2[2] + 1);

                    // accumulate saw
                    ++count14;
                    if (count14 >= 14)
                    {
                        count14 = 0;
                        phase[2] = 0;
                    }
                    else if (0 == (count14 & 1)) // only accumulate on even ticks
                    {
                        phase[2] = (UInt32)((phase[2] + volume[2]) & 0xFF); // note 8-bit wrapping behaviour
                    }
                }
            }

            // only top 5 bits of saw are output
            return (Int16)(phase[2] >> 3);
        }

        public override void Tick(UInt32 clocks)
        {
            _out[0] = calc_sqr(0, clocks);
            _out[1] = calc_sqr(1, clocks);
            _out[2] = calc_saw(clocks);
        }

        private Int32[] m = new Int32[3];

        public override UInt32 Render(Int32[] b)//b[2])
        {
            m[0] = _out[0];
            m[1] = _out[1];
            m[2] = _out[2];

            // note: signal is inverted compared to 2A03

            m[0] = (mask & 1) != 0 ? 0 : -m[0];
            m[1] = (mask & 2) != 0 ? 0 : -m[1];
            m[2] = (mask & 4) != 0 ? 0 : -m[2];

            b[0] = m[0] * sm[0][0];
            b[0] += m[1] * sm[0][1];
            b[0] += m[2] * sm[0][2];
            //b[0] >>= (7 - 7);

            b[1] = m[0] * sm[1][0];
            b[1] += m[1] * sm[1][1];
            b[1] += m[2] * sm[1][2];
            //b[1] >>= (7 - 7);

            // master volume adjustment
            const Int32 MASTER = (Int32)(256.0 * 1223.0 / 1920.0);
            b[0] = (b[0] * MASTER) >> 5;
            b[1] = (b[1] * MASTER) >> 5;

            return 2;
        }

        private Int32[] cmap = new Int32[4] { 0, 0, 1, 2 };

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id=0)
        {
            Int32 ch;

            switch (adr)
            {
                case 0x9000:
                case 0xa000:
                    ch = cmap[(adr >> 12) & 3];
                    volume[ch] = (Int32)(val & 15);
                    duty[ch] = (Int32)((val >> 4) & 7);
                    gate[ch] = (Int32)((val >> 7) & 1);
                    break;
                case 0xb000:
                    volume[2] = (Int32)(val & 63);
                    break;

                case 0x9001:
                case 0xa001:
                case 0xb001:
                    ch = cmap[(adr >> 12) & 3];
                    freq[ch] = (freq[ch] & 0xf00) | val;
                    freq2[ch] = (freq[ch] >> freq_shift);
                    if (counter[ch] > freq2[ch]) counter[ch] = freq2[ch];
                    break;

                case 0x9002:
                case 0xa002:
                case 0xb002:
                    ch = cmap[(adr >> 12) & 3];
                    freq[ch] = ((val & 0xf) << 8) + (freq[ch] & 0xff);
                    freq2[ch] = (freq[ch] >> freq_shift);
                    if (counter[ch] > freq2[ch]) counter[ch] = freq2[ch];
                    if (enable[ch] == 0) // if enable is being turned on, phase should be reset
                    {
                        if (ch == 2)
                        {
                            count14 = 0; // reset saw
                        }
                        phase[ch] = 0;
                    }
                    enable[ch] = (Int32)((val >> 7) & 1);
                    break;

                case 0x9003:
                    halt = (val & 1) != 0;
                    freq_shift =
                        (val & 4) != 0 ? 8 :
                        (val & 2) != 0 ? 4 :
                        0;
                    freq2[0] = (freq[0] >> freq_shift);
                    freq2[1] = (freq[1] >> freq_shift);
                    freq2[2] = (freq[2] >> freq_shift);
                    if (counter[0] > freq2[0]) counter[0] = freq2[0];
                    if (counter[1] > freq2[1]) counter[1] = freq2[1];
                    if (counter[2] > freq2[2]) counter[2] = freq2[2];
                    break;

                default:
                    return false;

            }

            return true;
        }


        public override bool Read(uint adr, ref uint val, uint id = 0)
        {
            return false;
        }

        public override void SetMask(int m)
        {
            mask = m;
        }

        public override void SetOption(int id, int val)
        {
            throw new NotImplementedException();
        }

    }
}
