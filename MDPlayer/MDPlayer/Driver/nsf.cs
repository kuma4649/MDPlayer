using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDPlayer.NSF;
using MDSound.np;

namespace MDPlayer
{
    public class nsf : baseDriver
    {

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (common.getLE32(buf, 0) != FCC_NSF)
            {
                //NSFeはとりあえず未サポート
                return null;
            }

            if (buf.Length < 0x80) // no header?
                return null;

            version = buf[0x05];
            songs = buf[0x06];
            start = buf[0x07];
            load_address = (UInt16)(buf[0x08] | (buf[0x09] << 8));
            init_address = (UInt16)(buf[0x0a] | (buf[0x0B] << 8));
            play_address = (UInt16)(buf[0x0c] | (buf[0x0D] << 8));

            //memcpy(title_nsf, image + 0x0e, 32);
            //title_nsf[31] = '\0';
            List<byte> strLst = new List<byte>();
            Int32 TAGAdr = 0x0e;
            for (int i = 0; i < 32; i++)
            {
                if (buf[TAGAdr] == 0) break;
                strLst.Add(buf[TAGAdr++]);
            }
            title_nsf = Encoding.GetEncoding(932).GetString(strLst.ToArray());
            title = title_nsf;

            //memcpy(artist_nsf, image + 0x2e, 32);
            //artist_nsf[31] = '\0';
            strLst.Clear();
            TAGAdr = 0x2e;
            for (int i = 0; i < 32; i++)
            {
                if (buf[TAGAdr] == 0) break;
                strLst.Add(buf[TAGAdr++]);
            }
            artist_nsf = Encoding.GetEncoding(932).GetString(strLst.ToArray());
            artist = artist_nsf;

            //memcpy(copyright_nsf, image + 0x4e, 32);
            //copyright_nsf[31] = '\0';
            strLst.Clear();
            TAGAdr = 0x4e;
            for (int i = 0; i < 32; i++)
            {
                if (buf[TAGAdr] == 0) break;
                strLst.Add(buf[TAGAdr++]);
            }
            copyright_nsf = Encoding.GetEncoding(932).GetString(strLst.ToArray());
            copyright = copyright_nsf;

            ripper = ""; // NSFe only
            text = ""; // NSFe only
            text_len = 0; // NSFe only
            speed_ntsc = (UInt16)(buf[0x6e] | (buf[0x6f] << 8));
            //memcpy(bankswitch, image + 0x70, 8);
            for (int i = 0; i < 8; i++) bankswitch[i] = buf[0x70 + i];
            speed_pal = (UInt16)(buf[0x78] | (buf[0x79] << 8));
            pal_ntsc = buf[0x7a];

            if (speed_pal == 0)
                speed_pal = 0x4e20;
            if (speed_ntsc == 0)
                speed_ntsc = 0x411A;

            soundchip = buf[0x7b];

            use_vrc6 = (soundchip & 1) != 0;
            use_vrc7 = (soundchip & 2) != 0;
            use_fds = (soundchip & 4) != 0;
            use_mmc5 = (soundchip & 8) != 0;
            use_n106 = (soundchip & 16) != 0;
            use_fme7 = (soundchip & 32) != 0;

            //memcpy(extra, image + 0x7c, 4);
            for (int i = 0; i < 4; i++) extra[i] = buf[0x7c + i];

            //delete[] body;
            //body = new UINT8[size - 0x80];
            //memcpy(body, image + 0x80, size - 0x80);
            body = new byte[buf.Length - 0x80];
            for (int i = 0; i < buf.Length - 0x80; i++) body[i] = buf[0x80 + i];

            bodysize = buf.Length - 0x80;

            //song = start - 1;

            GD3 gd3 = new GD3();
            gd3.GameName = title;
            gd3.GameNameJ = title;
            gd3.Composer = artist;
            gd3.ComposerJ = artist;
            gd3.TrackName = title;
            gd3.TrackNameJ = title;
            gd3.SystemName = copyright;
            gd3.SystemNameJ = copyright;

            return gd3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip useChip, uint latency)
        {

            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;

            if (model == enmModel.RealModel)
            {
                Stopped = true;
                vgmCurLoop = 9999;
                return true;
            }

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = 0;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            GD3 = getGD3Info(vgmBuf, 0);

            nsfInit();

            return true;
        }

        public override void oneFrameProc()
        {
            if (model == enmModel.RealModel) return;

            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0 && !Stopped)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        //oneFrameMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
                //Stopped = !IsPlaying();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        public const int FCC_NSF = 0x4d53454e;  // "NESM"

        public byte version;
        public byte songs;
        public byte start;
        public UInt16 load_address;
        public UInt16 init_address;
        public UInt16 play_address;
        public string filename;
        public string print_title;//[256 + 64]; // margin 64 chars.
        public string title_nsf;//[32];
        public string artist_nsf;//[32];
        public string copyright_nsf;//[32];
        public string title;
        public string artist;
        public string copyright;
        public string ripper; // NSFe only
        public string text; // NSFe only
        public UInt32 text_len; // NSFe only
        public UInt16 speed_ntsc;
        public byte[] bankswitch = new byte[8];
        public UInt16 speed_pal;
        public byte pal_ntsc;
        public byte soundchip;
        public bool use_vrc7;
        public bool use_vrc6;
        public bool use_fds;
        public bool use_fme7;
        public bool use_mmc5;
        public bool use_n106;
        public byte[] extra = new byte[4];
        public byte[] body;
        public Int32 bodysize;
        public byte[] nsfe_image;
        public byte[] nsfe_plst;
        public Int32 nsfe_plst_size;
        const UInt32 NSFE_ENTRIES = 256;
        public class NSFE_Entry
        {
            public byte[] tlbl;
            public Int32 time;
            public Int32 fade;
        };
        public NSFE_Entry[] nsfe_entry = new NSFE_Entry[NSFE_ENTRIES];

        /** 現在選択中の曲番号 */
        public Int32 song;

        private Bus apu_bus;
        private Bus stack;
        private Layer layer;

        private nes_bank nes_bank = null;
        private nes_mem nes_mem = null;
        private km6502 nes_cpu = null;
        private nes_apu nes_apu = null;
        private nes_dmc nes_dmc = null;
        private nes_fds nes_fds = null;
        private nes_n106 nes_n106 = null;
        private nes_vrc6 nes_vrc6 = null;
        private nes_mmc5 nes_mmc5 = null;
        private nes_fme7 nes_fme7 = null;
        private nes_vrc7 nes_vrc7 = null;

        private NESDetector ld = null;
//        private NESDetectorEx ld = null;

        private double rate = common.SampleRate;
        private double cpu_clock_rest;
        private double apu_clock_rest;
        private Int32 time_in_ms;
        private long silent_length = 0;
        private Int32 last_out = 0;

        private void nsfInit()
        {
            nes_bank = new nes_bank();
            nes_mem = new nes_mem();
            nes_cpu = new km6502();
            nes_apu = new nes_apu();
            nes_dmc = new nes_dmc();
            nes_fds = new nes_fds();
            nes_n106 = new nes_n106();
            nes_vrc6 = new nes_vrc6();
            nes_mmc5 = new nes_mmc5();
            nes_fme7 = new nes_fme7();
            nes_vrc7 = new nes_vrc7();

            nes_apu.chip = nes_apu.apu.NES_APU_np_Create(common.NsfClock, common.SampleRate);
            nes_apu.Reset();
            nes_dmc.chip = nes_dmc.dmc.NES_DMC_np_Create(common.NsfClock, common.SampleRate);
            nes_dmc.Reset();
            nes_fds.chip = nes_fds.fds.NES_FDS_Create(common.NsfClock, common.SampleRate);
            nes_fds.Reset();
            nes_n106.SetClock(common.NsfClock);
            nes_n106.SetRate(common.SampleRate);
            nes_n106.Reset();
            nes_vrc6.SetClock(common.NsfClock);
            nes_vrc6.SetRate(common.SampleRate);
            nes_vrc6.Reset();
            nes_mmc5.SetClock(common.NsfClock);
            nes_mmc5.SetRate(common.SampleRate);
            nes_mmc5.Reset();
            nes_mmc5.SetCPU(nes_cpu);
            nes_fme7.SetClock(common.NsfClock);
            nes_fme7.SetRate(common.SampleRate);
            nes_fme7.Reset();
            nes_vrc7.SetClock(common.NsfClock);
            nes_vrc7.SetRate(common.SampleRate);
            nes_vrc7.Reset();

            nes_dmc.dmc.nes_apu = nes_apu.apu;
            nes_dmc.dmc.NES_DMC_np_SetAPU(nes_dmc.chip, nes_apu.chip);

            stack = new Bus();
            layer = new Layer();
            apu_bus = new Bus();

            int i, bmax = 0;

            for (i = 0; i < 8; i++)
                if (bmax < bankswitch[i])
                    bmax = bankswitch[i];

            nes_mem.SetImage(body, load_address, (UInt32)bodysize);

            if (bmax != 0)
            {
                nes_bank.SetImage(body, load_address, (UInt32)bodysize);
                for (i = 0; i < 8; i++)
                    nes_bank.SetBankDefault((byte)(i + 8), bankswitch[i]);
            }

            stack.DetachAll();
            layer.DetachAll();
            apu_bus.DetachAll();

            ld = new NESDetector();
//            ld = new NESDetectorEx();
            ld.Reset();
            stack.Attach(ld);

            apu_bus.Attach(nes_apu);
            apu_bus.Attach(nes_dmc);

            if (use_fds)
            {
                bool write_enable = !setting.nsf.FDSWriteDisable8000;
                nes_mem.SetFDSMode(write_enable);
                nes_bank.SetFDSMode(write_enable);
                nes_bank.SetBankDefault(6, bankswitch[6]);
                nes_bank.SetBankDefault(7, bankswitch[7]);
                apu_bus.Attach(nes_fds);
            }
            else
            {
                nes_mem.SetFDSMode(false);
                nes_bank.SetFDSMode(false);
            }
            if (use_n106)
            {
                apu_bus.Attach(nes_n106);
            }
            if (use_vrc6)
            {
                apu_bus.Attach(nes_vrc6);
            }
            if (use_mmc5)
            {
                apu_bus.Attach(nes_mmc5);
            }
            if (use_fme7)
            {
                apu_bus.Attach(nes_fme7);
            }
            if (use_vrc7)
            {
                apu_bus.Attach(nes_vrc7);
            }

            if (bmax > 0) layer.Attach(nes_bank);
            layer.Attach(nes_mem);

            stack.Attach(apu_bus);
            stack.Attach(layer);

            nes_cpu.SetMemory(stack);
            nes_dmc.SetMemory(stack);

            nes_apu.chip.square_table[0] = 0;
            for (i = 1; i < 32; i++)
                nes_apu.chip.square_table[i] = (Int32)((8192.0 * 95.88) / (8128.0 / i + 100));

            for (int c = 0; c < 2; ++c)
                for (int t = 0; t < 2; ++t)
                    nes_apu.chip.sm[c][t] = 128;

            Reset();
        }

        public enum enmREGION
        {
            NTSC = 0,
            PAL,
            DENDY
        };

        private void Reset()
        {
            apu_clock_rest = 0.0;
            cpu_clock_rest = 0.0;
            silent_length = 0;

            enmREGION region = GetRegion(pal_ntsc);
            double speed;
            speed = 1000000.0 / ((region == enmREGION.NTSC) ? speed_ntsc : speed_pal);

            layer.Reset();
            nes_cpu.Reset();

            nes_cpu.Start(init_address, play_address, speed, song, (region == enmREGION.PAL) ? 1 : 0);
        }

        private enmREGION GetRegion(byte flags)
        {
            int pref = 0;// config->GetValue("REGION").GetInt();

            // user forced region
            if (pref == 3) return enmREGION.NTSC;
            if (pref == 4) return enmREGION.PAL;
            if (pref == 5) return enmREGION.DENDY;

            // single-mode NSF
            if (flags == 0) return enmREGION.NTSC;
            if (flags == 1) return enmREGION.PAL;

            if ((flags & 2) != 0) // dual mode
            {
                if (pref == 1) return enmREGION.NTSC;
                if (pref == 2) return enmREGION.PAL;
                // else pref == 0 or invalid, use auto setting based on flags bit
                return ((flags & 1) != 0) ? enmREGION.PAL : enmREGION.NTSC;
            }

            return enmREGION.NTSC; // fallback for invalid flags
        }

        public UInt32 Render(Int16[] b, UInt32 length)
        {
            return Render(b, length, 0);
        }

        public UInt32 Render(Int16[] b, UInt32 length,Int32 offset)
        {
            if (model == enmModel.RealModel) return length;
 
            Int32[] buf = new Int32[2];
            Int32[] _out = new Int32[2];
            Int32 outm;
            UInt32 i;
            UInt32 master_volume;

            master_volume = 0x80;// (*config)["MASTER_VOLUME"];

            double apu_clock_per_sample = nes_cpu.NES_BASECYCLES / rate;
            double cpu_clock_per_sample = apu_clock_per_sample * vgmSpeed;// ((double)((*config)["MULT_SPEED"].GetInt()) / 256.0);


            for (i = 0; i < length; i++)
            {
                //total_render++;
                vgmSpeedCounter += vgmSpeed;
                Counter=(Int32)vgmSpeedCounter;
                vgmFrameCounter++;

                // tick CPU
                cpu_clock_rest += cpu_clock_per_sample;
                int cpu_clocks = (int)(cpu_clock_rest);
                if (cpu_clocks > 0)
                {
                    UInt32 real_cpu_clocks = nes_cpu.Exec((UInt32)cpu_clocks);
                    cpu_clock_rest -= (double)(real_cpu_clocks);

                    // tick APU frame sequencer
                    nes_dmc.dmc.TickFrameSequence(nes_dmc.chip, real_cpu_clocks);
                    if (use_mmc5)
                        nes_mmc5.TickFrameSequence(real_cpu_clocks);
                }

                //UpdateInfo();

                // tick APU / expansions
                apu_clock_rest += apu_clock_per_sample;
                int apu_clocks = (int)(apu_clock_rest);
                if (apu_clocks > 0)
                {
                    //mixer.Tick(apu_clocks);
                    apu_clock_rest -= (double)(apu_clocks);
                }

                // render output
                //mixer.Render(buf);
                nes_apu.Tick((UInt32)apu_clocks);
                nes_apu.Render(buf);

                // echo.FastRender(buf);
                //dcf.FastRender(buf);
                //lpf.FastRender(buf);
                //cmp.FastRender(buf);

                //mfilter->Put(buf[0]);
                //out = mfilter->Get();

                _out[0] = buf[0];
                _out[1] = buf[1];

                nes_dmc.Tick((UInt32)apu_clocks);
                nes_dmc.Render(buf);
                _out[0] += buf[0];
                _out[1] += buf[1];

                if (use_fds)
                {
                    nes_fds.Tick((UInt32)apu_clocks);
                    nes_fds.Render(buf);
                    _out[0] += buf[0];
                    _out[1] += buf[1];
                }

                if (use_n106)
                {
                    nes_n106.Tick((UInt32)apu_clocks);
                    nes_n106.Render(buf);
                    _out[0] += buf[0];
                    _out[1] += buf[1];
                }

                if (use_vrc6)
                {
                    nes_vrc6.Tick((UInt32)apu_clocks);
                    nes_vrc6.Render(buf);
                    _out[0] += buf[0];
                    _out[1] += buf[1];
                }

                if (use_mmc5)
                {
                    nes_mmc5.Tick((UInt32)apu_clocks);
                    nes_mmc5.Render(buf);
                    _out[0] += buf[0];
                    _out[1] += buf[1];
                }

                if (use_fme7)
                {
                    nes_fme7.Tick((UInt32)apu_clocks);
                    nes_fme7.Render(buf);
                    _out[0] += buf[0];
                    _out[1] += buf[1];
                }

                if (use_vrc7)
                {
                    nes_vrc7.Tick((UInt32)apu_clocks);
                    nes_vrc7.Render(buf);
                    _out[0] += buf[0];
                    _out[1] += buf[1];
                }

                outm = (_out[0] + _out[1]);// >> 1; // mono mix
                if (outm == last_out) silent_length++;
                else silent_length = 0;
                last_out = outm;

                _out[0] = (Int32)((_out[0] * master_volume) >> 8);
                _out[1] = (Int32)((_out[1] * master_volume) >> 8);

                if (_out[0] < -32767) _out[0] = -32767;
                else if (32767 < _out[0]) _out[0] = 32767;

                if (_out[1] < -32767) _out[1] = -32767;
                else if (32767 < _out[1]) _out[1] = 32767;

                //if (nch == 2)
                //{
                b[offset + i * 2] = (Int16)_out[0];
                b[offset + i * 2 + 1] = (Int16)_out[1];
                //}
                //else // if not 2 channels, presume mono
                //{
                //    outm = (_out[0] + _out[1]) >> 1;
                //    for (int i = 0; i<nch; ++i)
                //        b[0] = outm;
                //}
                //b += nch;
            }

            time_in_ms += (int)(1000 * length / rate * vgmSpeed);// ((* config)["MULT_SPEED"].GetInt()) / 256);

            //CheckTerminal();
            DetectLoop();
            DetectSilent();
            if (!playtime_detected) vgmCurLoop = 0;
            else
            {
                if (TotalCounter != 0) vgmCurLoop = (uint)(Counter / TotalCounter);
                else Stopped = true;
            }

            return length;
        }

        public bool playtime_detected = false;

        public void DetectLoop()
        {
            if (ld.IsLooped(time_in_ms, 30000, 5000) && !playtime_detected)
            {
                playtime_detected = true;
                TotalCounter = (long)(ld.GetLoopEnd() * 44100L/1000L);
                if (TotalCounter == 0) TotalCounter = Counter;
                LoopCounter = (long)((ld.GetLoopEnd()- ld.GetLoopStart()) * 44100L/1000L);
            }
        }

        public void DetectSilent()
        {
            if (silent_length>44100*3 && !playtime_detected)
            {
                playtime_detected = true;
                TotalCounter = (long)(ld.GetLoopEnd() * 44100L / 1000L);
                if (TotalCounter == 0) TotalCounter = Counter;
                LoopCounter = 0;
                Stopped = true;
            }
        }

    }
}
