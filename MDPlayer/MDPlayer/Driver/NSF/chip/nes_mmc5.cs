using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_mmc5 : ISoundChip
    {
        public const double DEFAULT_CLOCK = 1789772.0;
        public const int DEFAULT_RATE = 44100;

        public enum OPT
        {
            NONLINEAR_MIXER = 0
            , PHASE_REFRESH
            , END
        };

        protected Int32[] option=new Int32[(int)OPT.END];
        protected Int32 mask;
        protected Int32[][] sm=new Int32[2][] { new Int32[3], new Int32[3] }; // stereo panning
    protected byte[] ram=new byte[0x6000 - 0x5c00];
        protected byte[] reg=new byte[8];
        protected byte[] mreg=new byte[2];
        protected byte pcm; // PCM channel
        protected bool pcm_mode; // PCM channel
        protected MDPlayer.NSF.km6502 cpu; // PCM channel reads need CPU access

        protected UInt32[] scounter=new UInt32[2];            // frequency divider
        protected UInt32[] sphase=new UInt32[2];              // phase counter

        protected UInt32[] duty=new UInt32[2];
        protected UInt32[] volume = new UInt32[2];
        protected UInt32[] freq = new UInt32[2];
        protected Int32[] _out = new Int32[3];
    protected bool[] enable = new bool[2];

        protected bool[] envelope_disable = new bool[2];   // エンベロープ有効フラグ
        protected bool[] envelope_loop = new bool[2];      // エンベロープループ
        protected bool[] envelope_write = new bool[2];
        protected Int32[] envelope_div_period = new Int32[2];
        protected Int32[] envelope_div = new Int32[2];
        protected Int32[] envelope_counter = new Int32[2];

        protected Int32[] length_counter = new Int32[2];

        protected Int32 frame_sequence_count;

        protected double clock, rate;
        protected Int32[] square_table = new Int32[32];
        protected Int32[] pcm_table = new Int32[256];
        protected TrackInfoBasic[] trkinfo = new TrackInfoBasic[3];

        public nes_mmc5()
        {
            cpu = null;
            SetClock(DEFAULT_CLOCK);
            SetRate(DEFAULT_RATE);
            option[(int)OPT.NONLINEAR_MIXER] = 1;//true;
            option[(int)OPT.PHASE_REFRESH] = 1;//true;
            frame_sequence_count = 0;

            // square nonlinear mix, same as 2A03
            square_table[0] = 0;
            for (int i = 1; i < 32; i++)
                square_table[i] = (Int32)((8192.0 * 95.88) / (8128.0 / i + 100));

            // 2A03 style nonlinear pcm mix with double the bits
            //pcm_table[0] = 0;
            //INT32 wd = 22638;
            //for(int d=1;d<256; ++d)
            //    pcm_table[d] = (INT32)((8192.0*159.79)/(100.0+1.0/((double)d/wd)));

            // linear pcm mix (actual hardware seems closer to this)
            pcm_table[0] = 0;
            double pcm_scale = 32.0;
            for (int d = 1; d < 256; ++d)
                pcm_table[d] = (Int32)((double)(d) * pcm_scale);

            // stereo mix
            for (int c = 0; c < 2; ++c)
                for (int t = 0; t < 3; ++t)
                    sm[c][t] = 128;
        }

        ~nes_mmc5()
        {
        }

        public override void Reset()
        {
            int i;

            scounter[0] = 0;
            scounter[1] = 0;
            sphase[0] = 0;
            sphase[1] = 0;

            envelope_div[0] = 0;
            envelope_div[1] = 0;
            length_counter[0] = 0;
            length_counter[1] = 0;
            envelope_counter[0] = 0;
            envelope_counter[1] = 0;
            frame_sequence_count = 0;

            for (i = 0; i < 8; i++)
                Write((UInt32)(0x5000 + i), 0);

            Write(0x5015, 0);

            for (i = 0; i < 3; ++i) _out[i] = 0;

            mask = 0;
            pcm = 0; // PCM channel
            pcm_mode = false; // write mode

            SetRate(rate);
        }

        public override void SetOption(int id, int val)
        {
            if (id < (int)OPT.END) option[id] = val;
        }

        public override void SetClock(double c)
        {
            this.clock = c;
        }

        public override void SetRate(double r)
        {
            rate = r!=0 ? r : DEFAULT_RATE;
        }

        public void FrameSequence()
        {
            // 240hz clock
            for (int i = 0; i < 2; ++i)
            {
                bool divider = false;
                if (envelope_write[i])
                {
                    envelope_write[i] = false;
                    envelope_counter[i] = 15;
                    envelope_div[i] = 0;
                }
                else
                {
                    ++envelope_div[i];
                    if (envelope_div[i] > envelope_div_period[i])
                    {
                        divider = true;
                        envelope_div[i] = 0;
                    }
                }
                if (divider)
                {
                    if (envelope_loop[i] && envelope_counter[i] == 0)
                        envelope_counter[i] = 15;
                    else if (envelope_counter[i] > 0)
                        --envelope_counter[i];
                }
            }

            // MMC5 length counter is clocked at 240hz, unlike 2A03
            for (int i = 0; i < 2; ++i)
            {
                if (!envelope_loop[i] && (length_counter[i] > 0))
                    --length_counter[i];
            }
        }

        private Int16[][] sqrtbl = new Int16[4][] {
      new Int16[16]{0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
      new Int16[16]{0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
      new Int16[16]{0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0},
      new Int16[16]{1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        private Int32 calc_sqr(Int32 i, UInt32 clocks)
        {

            scounter[i] += clocks;
            while (scounter[i] > freq[i])
            {
                sphase[i] = (sphase[i] + 1) & 15;
                scounter[i] -= (freq[i] + 1);
            }

            Int32 ret = 0;
            if (length_counter[i] > 0)
            {
                // note MMC5 does not silence the highest 8 frequencies like APU,
                // because this is done by the sweep unit.

                int v = (Int32)(envelope_disable[i] ? (Int32)volume[i] : envelope_counter[i]);
                ret = sqrtbl[duty[i]][sphase[i]] != 0 ? v : 0;
            }

            return ret;
        }

        public void TickFrameSequence(UInt32 clocks)
        {
            frame_sequence_count += (Int32)clocks;
            while (frame_sequence_count > 7458)
            {
                FrameSequence();
                frame_sequence_count -= 7458;
            }
        }

        public override void Tick(UInt32 clocks)
        {
            _out[0] = calc_sqr(0, clocks);
            _out[1] = calc_sqr(1, clocks);
            _out[2] = pcm;
        }

        public override UInt32 Render(Int32[] b)//b[2])
        {
            _out[0] = (mask & 1) != 0 ? 0 : _out[0];
            _out[1] = (mask & 2) != 0 ? 0 : _out[1];
            _out[2] = (mask & 4) != 0 ? 0 : _out[2];

            Int32[] m = new Int32[3];

            if (option[(int)OPT.NONLINEAR_MIXER] != 0)
            {
                // squares nonlinear
                Int32 voltage = square_table[_out[0] + _out[1]];
                m[0] = _out[0] << 6;
                m[1] = _out[1] << 6;
                Int32 _ref = m[0] + m[1];
                if (_ref > 0)
                {
                    m[0] = (m[0] * voltage) / _ref;
                    m[1] = (m[1] * voltage) / _ref;
                }
                else
                {
                    m[0] = voltage;
                    m[1] = voltage;
                }

                // pcm nonlinear
                m[2] = pcm_table[_out[2]];
            }
            else
            {
                // squares
                m[0] = _out[0] << 6;
                m[1] = _out[1] << 6;

                // pcm channel
                m[2] = _out[2] << 5;
            }

            // note polarity is flipped on output

            b[0] = m[0] * -sm[0][0];
            b[0] += m[1] * -sm[0][1];
            b[0] += m[2] * -sm[0][2];
            b[0] >>= 4;

            b[1] = m[0] * -sm[1][0];
            b[1] += m[1] * -sm[1][1];
            b[1] += m[2] * -sm[1][2];
            b[1] >>= 4;

            return 2;
        }

        public byte[] length_table = new byte[32]{
        0x0A, 0xFE,
        0x14, 0x02,
        0x28, 0x04,
        0x50, 0x06,
        0xA0, 0x08,
        0x3C, 0x0A,
        0x0E, 0x0C,
        0x1A, 0x0E,
        0x0C, 0x10,
        0x18, 0x12,
        0x30, 0x14,
        0x60, 0x16,
        0xC0, 0x18,
        0x48, 0x1A,
        0x10, 0x1C,
        0x20, 0x1E
        };

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id = 0)
        {
            Int32 ch;

            if ((0x5c00 <= adr) && (adr < 0x5ff0))
            {
                ram[adr & 0x3ff] = (byte)val;
                return true;
            }
            else if ((0x5000 <= adr) && (adr < 0x5008))
            {
                reg[adr & 0x7] = (byte)val;
            }

            switch (adr)
            {
                case 0x5000:
                case 0x5004:
                    ch = (Int32)((adr >> 2) & 1);
                    volume[ch] = val & 15;
                    envelope_disable[ch] = ((val >> 4) & 1) != 0;
                    envelope_loop[ch] = ((val >> 5) & 1) != 0;
                    envelope_div_period[ch] = (Int32)((val & 15));
                    duty[ch] = (val >> 6) & 3;
                    break;

                case 0x5002:
                case 0x5006:
                    ch = (Int32)((adr >> 2) & 1);
                    freq[ch] = val + (freq[ch] & 0x700);
                    if (scounter[ch] > freq[ch]) scounter[ch] = freq[ch];
                    break;

                case 0x5003:
                case 0x5007:
                    ch = (Int32)((adr >> 2) & 1);
                    freq[ch] = (freq[ch] & 0xff) + ((val & 7) << 8);
                    if (scounter[ch] > freq[ch]) scounter[ch] = freq[ch];
                    // phase reset
                    if (option[(int)OPT.PHASE_REFRESH] != 0)
                        sphase[ch] = 0;
                    envelope_write[ch] = true;
                    if (enable[ch])
                    {
                        length_counter[ch] = length_table[(val >> 3) & 0x1f];
                    }
                    break;

                // PCM channel control
                case 0x5010:
                    pcm_mode = ((val & 1) != 0); // 0 = write, 1 = read
                    break;

                // PCM channel control
                case 0x5011:
                    if (!pcm_mode)
                    {
                        val &= 0xFF;
                        if (val != 0) pcm = (byte)val;
                    }
                    break;

                case 0x5015:
                    enable[0] = (val & 1) != 0 ? true : false;
                    enable[1] = (val & 2) != 0 ? true : false;
                    if (!enable[0])
                        length_counter[0] = 0;
                    if (!enable[1])
                        length_counter[1] = 0;
                    break;

                case 0x5205:
                    mreg[0] = (byte)val;
                    break;

                case 0x5206:
                    mreg[1] = (byte)val;
                    break;

                default:
                    return false;

            }
            return true;
        }

        public override bool Read(UInt32 adr, ref UInt32 val, UInt32 id = 0)
        {
            // in PCM read mode, reads from $8000-$C000 automatically load the PCM output
            if (pcm_mode && (0x8000 <= adr) && (adr < 0xC000) && cpu != null)
            {
                pcm_mode = false; // prevent recursive entry
                UInt32 pcm_read = 0;
                cpu.Read(adr, ref pcm_read, id);
                pcm_read &= 0xFF;
                if (pcm_read != 0)
                    pcm = (byte)pcm_read;
                pcm_mode = true;
            }

            if ((0x5000 <= adr) && (adr < 0x5008))
            {
                val = reg[adr & 0x7];
                return true;
            }
            else if (adr == 0x5015)
            {
                val = (UInt32)((enable[1] ? 2 : 0) | (enable[0] ? 1 : 0));
                return true;
            }

            if ((0x5c00 <= adr) && (adr < 0x5ff0))
            {
                val = ram[adr & 0x3ff];
                return true;
            }
            else if (adr == 0x5205)
            {
                val = (UInt32)((mreg[0] * mreg[1]) & 0xff);
                return true;
            }
            else if (adr == 0x5206)
            {
                val = (UInt32)((mreg[0] * mreg[1]) >> 8);
                return true;
            }

            return false;
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
            //assert(trk < 3);

            if (trk < 2) // square
            {
                trkinfo[trk]._freq = freq[trk];
                if (freq[trk] != 0)
                    trkinfo[trk].freq = clock / 16 / (freq[trk] + 1);
                else
                    trkinfo[trk].freq = 0;

                trkinfo[trk].output = _out[trk];
                trkinfo[trk].max_volume = 15;
                trkinfo[trk].volume = (Int32)(volume[trk] + (envelope_disable[trk] ? 0 : 0x10));
                trkinfo[trk].key = (envelope_disable[trk] ? (volume[trk] > 0) : (envelope_counter[trk] > 0));
                trkinfo[trk].tone = (Int32)duty[trk];
            }
            else // pcm
            {
                trkinfo[trk]._freq = 0;
                trkinfo[trk].freq = 0;
                trkinfo[trk].output = _out[2];
                trkinfo[trk].max_volume = 255;
                trkinfo[trk].volume = pcm;
                trkinfo[trk].key = false;
                trkinfo[trk].tone = pcm_mode ? 1 : 0;
            }

            return trkinfo[trk];
        }

        // pcm read mode requires CPU read access
        public void SetCPU(km6502 cpu_)
        {
            cpu = cpu_;
        }

        public override void SetMask(int mask)
        {
            throw new NotImplementedException();
        }

    }
}
