using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class TrackInfoN106 : TrackInfoBasic
    {
        public Int32 wavelen;
        public Int16[] wave = new Int16[256];
        public new IDeviceInfo Clone()
        {
            TrackInfoN106 ti = new TrackInfoN106();
            ti.output = output;
            ti.volume = volume;
            ti.max_volume = max_volume;
            ti._freq = _freq;
            ti.freq = freq;
            ti.key = key;
            ti.tone = tone;
            ti.wavelen = wavelen;
            ti.wave = new Int16[256];
            for (int i = 0; i < ti.wave.Length; i++) ti.wave[i] = wave[i];
            return ti;
        }
    };

    public class nes_n106 : ISoundChip
    {
        public const double DEFAULT_CLOCK = 1789772.0;
        public const int DEFAULT_RATE = 44100;

        public enum OPT
        {
            SERIAL = 0,
            END
        };

        protected double rate, clock;
        protected int mask;
        protected Int32[][] sm = new Int32[2][] { new Int32[8], new Int32[8] }; // stereo mix
        protected Int32[] fout = new Int32[8]; // current output
        protected TrackInfoN106[] trkinfo = new TrackInfoN106[8];
        protected Int32[] option = new Int32[(int)OPT.END];

        protected bool master_disable;
        protected UInt32[] reg = new UInt32[0x80]; // all state is contained here
        protected UInt32 reg_select;
        protected bool reg_advance;
        protected Int32 tick_channel;
        protected Int32 tick_clock;
        protected Int32 render_channel;
        protected Int32 render_clock;
        protected Int32 render_subclock;


        public nes_n106()
        {
            option[(int)OPT.SERIAL] = 0;
            SetClock(DEFAULT_CLOCK);
            SetRate(DEFAULT_RATE);
            for (int i = 0; i < 8; ++i)
            {
                sm[0][i] = 128;
                sm[1][i] = 128;
            }
            Reset();
        }

        ~nes_n106()
        {
        }

        public override void SetStereoMix(Int32 trk, Int16 mixl, Int16 mixr)
        {
            if (trk < 0 || trk >= 8) return;
            trk = 7 - trk; // displayed channels are inverted
            sm[0][trk] = mixl;
            sm[1][trk] = mixr;
        }

        public ITrackInfo GetTrackInfo(Int32 trk)
        {
            Int32 channels = get_channels();
            Int32 channel = 7 - trk; // invert the track display

            TrackInfoN106 t = trkinfo[channel];

            if (trk >= channels)
            {
                t.max_volume = 15;
                t.volume = 0;
                t._freq = 0;
                t.wavelen = 0;
                t.tone = -1;
                t.output = 0;
                t.key = false;
                t.freq = 0;
            }
            else
            {
                t.max_volume = 15;
                t.volume = get_vol(channel);
                t._freq = get_freq(channel);
                t.wavelen = (Int32)get_len(channel);
                t.tone = (Int32)get_off(channel);
                t.output = fout[channel];

                t.key = (t.volume > 0) && (t._freq > 0);
                t.freq = ((double)(t._freq) * clock) / (double)(15 * 65536 * channels * t.wavelen);

                for (int i = 0; i < t.wavelen; ++i)
                    t.wave[i] = (Int16)get_sample((UInt32)((i + t.tone) & 0xFF));
            }

            return t;
        }

        public override void SetClock(double c)
        {
            clock = c;
        }

        public override void SetRate(double r)
        {
            rate = r;
        }

        public override void SetMask(int m)
        {
            // bit reverse the mask,
            // N163 waves are displayed in reverse order
            mask = 0
                | ((m & (1 << 0)) != 0 ? (1 << 7) : 0)
                | ((m & (1 << 1)) != 0 ? (1 << 6) : 0)
                | ((m & (1 << 2)) != 0 ? (1 << 5) : 0)
                | ((m & (1 << 3)) != 0 ? (1 << 4) : 0)
                | ((m & (1 << 4)) != 0 ? (1 << 3) : 0)
                | ((m & (1 << 5)) != 0 ? (1 << 2) : 0)
                | ((m & (1 << 6)) != 0 ? (1 << 1) : 0)
                | ((m & (1 << 7)) != 0 ? (1 << 0) : 0);
        }

        public override void SetOption(int id, int val)
        {
            if (id < (int)OPT.END) option[id] = val;
        }

        public override void Reset()
        {
            master_disable = false;
            for (int i = 0; i < reg.Length; i++) reg[i] = 0;
            reg_select = 0;
            reg_advance = false;
            tick_channel = 0;
            tick_clock = 0;
            render_channel = 0;
            render_clock = 0;
            render_subclock = 0;

            for (int i = 0; i < 8; ++i) fout[i] = 0;

            Write(0xE000, 0x00); // master disable off
            Write(0xF800, 0x80); // select $00 with auto-increment
            for (UInt32 i = 0; i < 0x80; ++i) // set all regs to 0
            {
                Write(0x4800, 0x00);
            }
            Write(0xF800, 0x00); // select $00 without auto-increment
        }

        public override void Tick(UInt32 clocks)
        {
            if (master_disable) return;

            int channels = get_channels();

            tick_clock +=(Int32) clocks;
            render_clock += (Int32)clocks; // keep render in sync
            while (tick_clock > 0)
            {
                int channel = 7 - tick_channel;

                UInt32 phase = get_phase(channel);
                UInt32 freq = get_freq(channel);
                UInt32 len = get_len(channel);
                UInt32 off = get_off(channel);
                UInt32 vol = (UInt32)get_vol(channel);

                // accumulate 24-bit phase
                phase = (phase + freq) & 0x00FFFFFF;

                // wrap phase if wavelength exceeded
                UInt32 hilen = len << 16;
                while (phase >= hilen) phase -= hilen;

                // write back phase
                set_phase(phase, channel);

                // fetch sample (note: N163 output is centred at 8, and inverted w.r.t 2A03)
                Int32 sample = 8 - get_sample(((phase >> 16) + off) & 0xFF);
                fout[channel] = (Int32)(sample * vol);

                // cycle to next channel every 15 clocks
                tick_clock -= 15;
                ++tick_channel;
                if (tick_channel >= channels)
                    tick_channel = 0;
            }
        }

        public override UInt32 Render(Int32[] b)//b[2])
        {
            b[0] = 0;
            b[1] = 0;
            if (master_disable) return 2;

            int channels = get_channels();

            if (option[(int)OPT.SERIAL] != 0) // hardware accurate serial multiplexing
            {
                // this could be made more efficient than going clock-by-clock
                // but this way is simpler
                int clocks = render_clock;
                while (clocks > 0)
                {
                    int c = 7 - render_channel;
                    if (0 == ((mask >> c) & 1))
                    {
                        b[0] += fout[c] * sm[0][c];
                        b[1] += fout[c] * sm[1][c];
                    }

                    ++render_subclock;
                    if (render_subclock >= 15) // each channel gets a 15-cycle slice
                    {
                        render_subclock = 0;
                        ++render_channel;
                        if (render_channel >= channels)
                            render_channel = 0;
                    }
                    --clocks;
                }

                // increase output level by 1 bits (7 bits already added from sm)
                b[0] <<= 1;
                b[1] <<= 1;

                // average the output
                if (render_clock > 0)
                {
                    b[0] /= render_clock;
                    b[1] /= render_clock;
                }
                render_clock = 0;
            }
            else // just mix all channels
            {
                for (int i = (8 - channels); i < 8; ++i)
                {
                    if (0 == ((mask >> i) & 1))
                    {
                        b[0] += fout[i] * sm[0][i];
                        b[1] += fout[i] * sm[1][i];
                    }
                }

                // mix together, increase output level by 8 bits, roll off 7 bits from sm
                Int32[] MIX = new Int32[9] { 256 / 1, 256 / 1, 256 / 2, 256 / 3, 256 / 4, 256 / 5, 256 / 6, 256 / 6, 256 / 6 };
                b[0] = (b[0] * MIX[channels]) >> 7;
                b[1] = (b[1] * MIX[channels]) >> 7;
                // when approximating the serial multiplex as a straight mix, once the
                // multiplex frequency gets below the nyquist frequency an average mix
                // begins to sound too quiet. To approximate this effect, I don't attenuate
                // any further after 6 channels are active.
            }

            // 8 bit approximation of master volume
            // max N163 vol vs max APU square
            // unfortunately, games have been measured as low as 3.4x and as high as 8.5x
            // with higher volumes on Erika, King of Kings, and Rolling Thunder
            // and lower volumes on others. Using 6.0x as a rough "one size fits all".
            const double MASTER_VOL = 6.0 * 1223.0;
            const double MAX_OUT = 15.0 * 15.0 * 256.0; // max digital value
            const Int32 GAIN = (Int32)((MASTER_VOL / MAX_OUT) * 256.0f);
            b[0] = (b[0] * GAIN) >> 5;
            b[1] = (b[1] * GAIN) >> 5;

            return 2;
        }

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id=0)
        {
            if (adr == 0xE000) // master disable
            {
                master_disable = ((val & 0x40) != 0);
                return true;
            }
            else if (adr == 0xF800) // register select
            {
                reg_select = (val & 0x7F);
                reg_advance = (val & 0x80) != 0;
                return true;
            }
            else if (adr == 0x4800) // register write
            {
                reg[reg_select] = val;
                if (reg_advance)
                    reg_select = (reg_select + 1) & 0x7F;
                return true;
            }
            return false;
        }

        public override bool Read(UInt32 adr, ref UInt32 val, UInt32 id)
        {
            if (adr == 0x4800) // register read
            {
                val = reg[reg_select];
                if (reg_advance)
                    reg_select = (reg_select + 1) & 0x7F;
                return true;
            }
            return false;
        }

        //
        // register decoding/encoding functions
        // 

        private UInt32 get_phase(Int32 channel)
        {
            // 24-bit phase stored in channel regs 1/3/5
            channel = channel << 3;
            return (reg[0x41 + channel])
                + (reg[0x43 + channel] << 8)
                + (reg[0x45 + channel] << 16);
        }

        private UInt32 get_freq(Int32 channel)
        {
            // 19-bit frequency stored in channel regs 0/2/4
            channel = channel << 3;
            return (reg[0x40 + channel])
                + (reg[0x42 + channel] << 8)
                + ((reg[0x44 + channel] & 0x03) << 16);
        }

        private UInt32 get_off(Int32 channel)
        {
            // 8-bit offset stored in channel reg 6
            channel = channel << 3;
            return reg[0x46 + channel];
        }

        private UInt32 get_len(Int32 channel)
        {
            // 6-bit<<3 length stored obscurely in channel reg 4
            channel = channel << 3;
            return 256 - (reg[0x44 + channel] & 0xFC);
        }

        private Int32 get_vol(Int32 channel)
        {
            // 4-bit volume stored in channel reg 7
            channel = channel << 3;
            return (Int32)(reg[0x47 + channel] & 0x0F);
        }

        private Int32 get_sample(UInt32 index)
        {
            // every sample becomes 2 samples in regs
            return (Int32)((index & 1) != 0 ?
                ((reg[index >> 1] >> 4) & 0x0F) :
                (reg[index >> 1] & 0x0F));
        }

        private Int32 get_channels()
        {
            // 3-bit channel count stored in reg 0x7F
            return (Int32)(((reg[0x7F] >> 4) & 0x07) + 1);
        }

        private void set_phase(UInt32 phase, Int32 channel)
        {
            // 24-bit phase stored in channel regs 1/3/5
            channel = channel << 3;
            reg[0x41 + channel] = phase & 0xFF;
            reg[0x43 + channel] = (phase >> 8) & 0xFF;
            reg[0x45 + channel] = (phase >> 16) & 0xFF;
        }

    }
}
