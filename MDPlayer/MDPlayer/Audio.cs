using System;
using System.Collections.Generic;
using NScci;
using System.Threading;
using System.Diagnostics;

namespace MDPlayer
{
    public class Audio
    {

        public enum enmScciChipType : int
        {
            YM2608 = 1
            , YM2151 = 2
            , YM2612 = 5
            , SN76489 = 7
        }

        private static object lockObj = new object();
        private static bool _fatalError = false;
        public static bool fatalError
        {
            get
            {
                lock(lockObj)
                {
                    return _fatalError;
                }
            }

            set
            {
                lock(lockObj)
                {
                    _fatalError = value;
                }
            }
        }

        private static uint SamplingRate = 44100;

        private static uint samplingBuffer = 1024;
        private static MDSound.MDSound mds = null;/*new MDSound.MDSound(
            SamplingRate
            , samplingBuffer
            , vgm.defaultYM2612ClockValue
            , vgm.defaultSN76489ClockValue
            , vgm.defaultRF5C164ClockValue
            , vgm.defaultPWMClockValue
            , vgm.defaultC140ClockValue, vgm.defaultC140Type);
            */

        private static NAudioWrap naudioWrap;

        private static NScci.NScci nscci;
        private static NSoundChip scYM2612 = null;
        private static NSoundChip scSN76489 = null;
        private static NSoundChip scYM2151 = null;
        private static NSoundChip scYM2608 = null;

        private static ChipRegister chipRegister = null;


        private static Thread trdMain = null;
        private static bool trdClosed = false;
        private static bool trdStopped = true;
        private static Stopwatch sw = Stopwatch.StartNew();
        private static double swFreq = Stopwatch.Frequency;

        private static byte[] vgmBuf = null;

        private static double vgmSpeed;

        private static bool vgmFadeout;
        private static double vgmFadeoutCounter;
        private static double vgmFadeoutCounterV;
        private static int vgmRealFadeoutVol = 0;
        private static int vgmRealFadeoutVolWait = 4;

        private static bool Paused = false;
        public static bool Stopped = false;

        private static Setting setting = null;

        public static vgm vgmVirtual = null;
        public static vgm vgmReal = null;

        private static bool oneTimeReset = false;
        private static int hiyorimiEven = 0;
        private static bool hiyorimiNecessary = false;



        public static PlayList.music getMusic(string file,byte[] buf)
        {
            PlayList.music music = new PlayList.music();

            music.fileName = file;
            music.title = "unknown";
            music.game = "unknown";

            if (buf.Length < 0x40) return music;

            if (getLE32(buf, 0x00) != MDPlayer.vgm.FCC_VGM) return music;

            uint version = getLE32(buf, 0x08);
            string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

            uint vgmGd3 = getLE32(buf, 0x14);
            GD3 gd3 = new GD3();
            if (vgmGd3 != 0)
            {
                uint vgmGd3Id = getLE32(buf, vgmGd3 + 0x14);
                if (vgmGd3Id != MDPlayer.vgm.FCC_GD3) return music;
                gd3.getGD3Info(buf, vgmGd3);
            }

            uint TotalCounter = getLE32(buf, 0x18);
            uint vgmLoopOffset = getLE32(buf, 0x1c);
            uint LoopCounter = getLE32(buf, 0x20);

            music.title = gd3.TrackName;
            music.titleJ = gd3.TrackNameJ;
            music.game = gd3.GameName;
            music.gameJ = gd3.GameNameJ;
            music.composer = gd3.Composer;
            music.composerJ = gd3.ComposerJ;
            music.vgmby = gd3.VGMBy;

            music.converted = gd3.Converted;
            music.notes = gd3.Notes;

            double sec = (double)TotalCounter / (double)SamplingRate;
            int TCminutes = (int)(sec / 60);
            sec -= TCminutes * 60;
            int TCsecond = (int)sec;
            sec -= TCsecond;
            int TCmillisecond = (int)(sec * 100.0);
            music.duration = string.Format("{0:D2}:{1:D2}:{2:D2}", TCminutes, TCsecond, TCmillisecond);

            return music;
        }



        public static void Init(Setting setting)
        {
            vgmVirtual = new vgm();
            vgmReal = new vgm();

            naudioWrap = new NAudioWrap((int)SamplingRate, trdVgmVirtualFunction);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;

            Audio.setting = setting.Copy();

            MDSound.MDSound.Chip[] chips = new MDSound.MDSound.Chip[8];

            chips[0] = new MDSound.MDSound.Chip();
            chips[0].type = MDSound.MDSound.enmInstrumentType.SN76489;
            chips[0].ID = 0;
            MDSound.sn76489 sn76489 = new MDSound.sn76489();
            chips[0].Instrument = sn76489;
            chips[0].Update = sn76489.Update;
            chips[0].Start = sn76489.Start;
            chips[0].Stop = sn76489.Stop;
            chips[0].Reset = sn76489.Reset;
            chips[0].SamplingRate = SamplingRate;
            chips[0].Volume = 100;
            chips[0].Clock = vgm.defaultSN76489ClockValue;
            chips[0].Option = null;

            chips[1] = new MDSound.MDSound.Chip();
            chips[1].type = MDSound.MDSound.enmInstrumentType.YM2612;
            chips[1].ID = 0;
            MDSound.ym2612 ym2612 = new MDSound.ym2612();
            chips[1].Instrument = ym2612;
            chips[1].Update = ym2612.Update;
            chips[1].Start = ym2612.Start;
            chips[1].Stop = ym2612.Stop;
            chips[1].Reset = ym2612.Reset;
            chips[1].SamplingRate = SamplingRate;
            chips[1].Volume = 100;
            chips[1].Clock = vgm.defaultYM2612ClockValue;
            chips[1].Option = null;

            chips[2] = new MDSound.MDSound.Chip();
            chips[2].type = MDSound.MDSound.enmInstrumentType.RF5C164;
            chips[2].ID = 0;
            MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();
            chips[2].Instrument = rf5c164;
            chips[2].Update = rf5c164.Update;
            chips[2].Start = rf5c164.Start;
            chips[2].Stop = rf5c164.Stop;
            chips[2].Reset = rf5c164.Reset;
            chips[2].SamplingRate = SamplingRate;
            chips[2].Volume = 100;
            chips[2].Clock = vgm.defaultRF5C164ClockValue;
            chips[2].Option = null;

            chips[3] = new MDSound.MDSound.Chip();
            chips[3].type = MDSound.MDSound.enmInstrumentType.PWM;
            chips[3].ID = 0;
            MDSound.pwm pwm = new MDSound.pwm();
            chips[3].Instrument = pwm;
            chips[3].Update = pwm.Update;
            chips[3].Start = pwm.Start;
            chips[3].Stop = pwm.Stop;
            chips[3].Reset = pwm.Reset;
            chips[3].SamplingRate = SamplingRate;
            chips[3].Volume = 100;
            chips[3].Clock = vgm.defaultPWMClockValue;
            chips[3].Option = null;

            chips[4] = new MDSound.MDSound.Chip();
            chips[4].type = MDSound.MDSound.enmInstrumentType.C140;
            chips[4].ID = 0;
            MDSound.c140 c140 = new MDSound.c140();
            chips[4].Instrument = c140;
            chips[4].Update = c140.Update;
            chips[4].Start = c140.Start;
            chips[4].Stop = c140.Stop;
            chips[4].Reset = c140.Reset;
            chips[4].SamplingRate = SamplingRate;
            chips[4].Volume = 100;
            chips[4].Clock = vgm.defaultC140ClockValue;
            chips[4].Option = new object[1] { vgm.defaultC140Type };

            chips[5] = new MDSound.MDSound.Chip();
            chips[5].type = MDSound.MDSound.enmInstrumentType.OKIM6258;
            chips[5].ID = 0;
            MDSound.okim6258 okim6258 = new MDSound.okim6258();
            chips[5].Instrument = okim6258;
            chips[5].Update = okim6258.Update;
            chips[5].Start = okim6258.Start;
            chips[5].Stop = okim6258.Stop;
            chips[5].Reset = okim6258.Reset;
            chips[5].SamplingRate = SamplingRate;
            chips[5].Volume = 100;
            chips[5].Clock = vgm.defaultOKIM6258ClockValue;
            chips[5].Option = new object[1] { (int)0 };

            chips[6] = new MDSound.MDSound.Chip();
            chips[6].type = MDSound.MDSound.enmInstrumentType.OKIM6295;
            chips[6].ID = 0;
            MDSound.okim6295 okim6295 = new MDSound.okim6295();
            chips[6].Instrument = okim6295;
            chips[6].Update = okim6295.Update;
            chips[6].Start = okim6295.Start;
            chips[6].Stop = okim6295.Stop;
            chips[6].Reset = okim6295.Reset;
            chips[6].SamplingRate = SamplingRate;
            chips[6].Volume = 100;
            chips[6].Clock = vgm.defaultOKIM6295ClockValue;
            chips[6].Option = null;

            chips[7] = new MDSound.MDSound.Chip();
            chips[7].type = MDSound.MDSound.enmInstrumentType.SEGAPCM;
            chips[7].ID = 0;
            MDSound.segapcm segapcm = new MDSound.segapcm();
            chips[7].Instrument = segapcm;
            chips[7].Update = segapcm.Update;
            chips[7].Start = segapcm.Start;
            chips[7].Stop = segapcm.Stop;
            chips[7].Reset = segapcm.Reset;
            chips[7].SamplingRate = SamplingRate;
            chips[7].Volume = 100;
            chips[7].Clock = vgm.defaultSEGAPCMClockValue;
            chips[7].Option = new object[1] { (int)0 };

            if (mds == null)
                mds = new MDSound.MDSound(SamplingRate, samplingBuffer, chips);
            else
                mds.Init(SamplingRate, samplingBuffer, chips);

            //, (vgm.defaultRF5C164ClockValue & 0x80000000) + (uint)((vgm.defaultRF5C164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
            //, (uint)(vgm.defaultPWMClockValue * (SamplingRate / (23011361.0 / 384)))
            //, (uint)(vgm.defaultC140ClockValue * (SamplingRate / (21390.0 / 384)))

            mds.setVolume(MDSound.MDSound.enmInstrumentType.SN76489, 0, setting.balance.SN76489Volume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.YM2612, 0, setting.balance.YM2612Volume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.RF5C164, 0, setting.balance.RF5C164Volume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.PWM, 0, setting.balance.PWMVolume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.C140, 0, setting.balance.C140Volume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.OKIM6258, 0, setting.balance.OKIM6258Volume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.OKIM6295, 0, setting.balance.OKIM6295Volume);
            mds.setVolume(MDSound.MDSound.enmInstrumentType.SEGAPCM, 0, setting.balance.SEGAPCMVolume);

            nscci = new NScci.NScci();

            scYM2612 = getChip(Audio.setting.YM2612Type.SoundLocation, Audio.setting.YM2612Type.BusID, Audio.setting.YM2612Type.SoundChip);
            if (scYM2612 != null)
            {
                scYM2612.init();
                vgmVirtual.dacControl.scYM2612 = scYM2612;
                vgmReal.dacControl.scYM2612 = scYM2612;
            }

            scSN76489 = getChip(Audio.setting.SN76489Type.SoundLocation, Audio.setting.SN76489Type.BusID, Audio.setting.SN76489Type.SoundChip);
            if (scSN76489 != null)
            {
                scSN76489.init();
                vgmVirtual.dacControl.scSN76489 = scSN76489;
                vgmReal.dacControl.scSN76489 = scSN76489;
            }

            scYM2608 = getChip(Audio.setting.YM2608Type.SoundLocation, Audio.setting.YM2608Type.BusID, Audio.setting.YM2608Type.SoundChip);
            if (scYM2608 != null)
            {
                scYM2608.init();
                vgmVirtual.dacControl.scYM2608 = scYM2608;
                vgmReal.dacControl.scYM2608 = scYM2608;
            }

            scYM2151 = getChip(Audio.setting.YM2151Type.SoundLocation, Audio.setting.YM2151Type.BusID, Audio.setting.YM2151Type.SoundChip);
            if (scYM2151 != null)
            {
                scYM2151.init();
                vgmVirtual.dacControl.scYM2151 = scYM2151;
                vgmReal.dacControl.scYM2151 = scYM2151;
            }

            chipRegister = new ChipRegister(mds, scYM2612, scSN76489, scYM2608, scYM2151, setting.YM2612Type, setting.SN76489Type, setting.YM2608Type, setting.YM2151Type);
            chipRegister.initChipRegister();

            vgmVirtual.dacControl.chipRegister = chipRegister;
            vgmVirtual.dacControl.model = vgm.enmModel.VirtualModel;

            vgmReal.dacControl.chipRegister = chipRegister;
            vgmReal.dacControl.model = vgm.enmModel.RealModel;

            Paused = false;
            Stopped = true;
            fatalError = false;
            oneTimeReset = false;

            naudioWrap.Start(Audio.setting);

        }

        public static List<NScci.NSoundChip> getYM2612ChipList()
        {
            return getChipList((int)enmScciChipType.YM2612);
        }

        public static List<NScci.NSoundChip> getYM2608ChipList()
        {
            return getChipList((int)enmScciChipType.YM2608);
        }

        public static List<NScci.NSoundChip> getYM2151ChipList()
        {
            return getChipList((int)enmScciChipType.YM2151);
        }

        public static List<NScci.NSoundChip> getSN76489ChipList()
        {
            return getChipList((int)enmScciChipType.SN76489);
        }

        public static int getLatency()
        {
            if (setting.outputDevice.DeviceType != 3)
            {
                return (int)SamplingRate * setting.outputDevice.Latency / 1000;
            }
            return naudioWrap.getAsioLatency();
        }

        public static void SetVGMBuffer(byte[] srcBuf)
        {
            Stop();
            vgmBuf = srcBuf;
        }

        public static bool Play(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

                vgm.enmUseChip usechip = vgm.enmUseChip.Unuse;
                if (setting.YM2612Type.UseScci)
                {
                    if (setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip |= vgm.enmUseChip.YM2612Ch6;
                    }
                }
                else
                {
                    usechip |= vgm.enmUseChip.YM2612;
                    usechip |= vgm.enmUseChip.YM2612Ch6;
                }
                if (!setting.SN76489Type.UseScci)
                {
                    usechip |= vgm.enmUseChip.SN76489;
                }
                usechip |= vgm.enmUseChip.RF5C164;
                usechip |= vgm.enmUseChip.PWM;

                if (!vgmVirtual.init(vgmBuf
                    , chipRegister
                    , vgm.enmModel.VirtualModel
                    , usechip
                    , SamplingRate * (uint)setting.LatencyEmulation/1000
                    ))
                    return false;

                usechip = vgm.enmUseChip.Unuse;
                if (setting.YM2612Type.UseScci)
                {
                    usechip |= vgm.enmUseChip.YM2612;
                    if (!setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip |= vgm.enmUseChip.YM2612Ch6;
                    }
                }

                if (!vgmReal.init(vgmBuf
                    , chipRegister
                    , vgm.enmModel.RealModel
                    , usechip
                    , SamplingRate * (uint)setting.LatencySCCI / 1000
                    ))
                    return false;

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;
                chipRegister.setFadeoutVolYM2608(0);
                chipRegister.setFadeoutVolYM2151(0);
                chipRegister.setFadeoutVolYM2612(0);
                chipRegister.setFadeoutVolSN76489(0);
                chipRegister.resetChips();

                trdClosed = false;
                trdMain = new Thread(new ThreadStart(trdVgmRealFunction));
                trdMain.Priority = ThreadPriority.Highest;
                trdMain.IsBackground = true;
                trdMain.Name = "trdVgmReal";
                trdMain.Start();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;

                if (vgmVirtual.SN76489ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                    chip.ID = 0;
                    MDSound.sn76489 sn76489 = new MDSound.sn76489();
                    chip.Instrument = sn76489;
                    chip.Update = sn76489.Update;
                    chip.Start = sn76489.Start;
                    chip.Stop = sn76489.Stop;
                    chip.Reset = sn76489.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.SN76489Volume;
                    chip.Clock = vgmVirtual.SN76489ClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= (setting.SN76489Type.UseScci) ? 0x1 : 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.YM2612ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                    chip.ID = 0;
                    MDSound.ym2612 ym2612 = new MDSound.ym2612();
                    chip.Instrument = ym2612;
                    chip.Update = ym2612.Update;
                    chip.Start = ym2612.Start;
                    chip.Stop = ym2612.Stop;
                    chip.Reset = ym2612.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.YM2612Volume;
                    chip.Clock = vgmVirtual.YM2612ClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci) ? 0x1 : 0x2;
                    hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci && setting.YM2612Type.OnlyPCMEmulation) ? 0x2 : 0x0;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.RF5C164ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.RF5C164;
                    chip.ID = 0;
                    MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();
                    chip.Instrument = rf5c164;
                    chip.Update = rf5c164.Update;
                    chip.Start = rf5c164.Start;
                    chip.Stop = rf5c164.Stop;
                    chip.Reset = rf5c164.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.RF5C164Volume;
                    chip.Clock = vgmVirtual.RF5C164ClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.PWMClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.PWM;
                    chip.ID = 0;
                    MDSound.pwm pwm = new MDSound.pwm();
                    chip.Instrument = pwm;
                    chip.Update = pwm.Update;
                    chip.Start = pwm.Start;
                    chip.Stop = pwm.Stop;
                    chip.Reset = pwm.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.PWMVolume;
                    chip.Clock = vgmVirtual.PWMClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.C140ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.C140;
                    chip.ID = 0;
                    MDSound.c140 c140 = new MDSound.c140();
                    chip.Instrument = c140;
                    chip.Update = c140.Update;
                    chip.Start = c140.Start;
                    chip.Stop = c140.Stop;
                    chip.Reset = c140.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.C140Volume;
                    chip.Clock = vgmVirtual.C140ClockValue;
                    chip.Option = new object[1] { vgmVirtual.C140Type };

                    hiyorimiDeviceFlag |= 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.OKIM6258ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.OKIM6258;
                    chip.ID = 0;
                    MDSound.okim6258 okim6258 = new MDSound.okim6258();
                    chip.Instrument = okim6258;
                    chip.Update = okim6258.Update;
                    chip.Start = okim6258.Start;
                    chip.Stop = okim6258.Stop;
                    chip.Reset = okim6258.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.OKIM6258Volume;
                    chip.Clock = vgmVirtual.OKIM6258ClockValue;
                    chip.Option = new object[1] { (int)vgmVirtual.OKIM6258Type };

                    hiyorimiDeviceFlag |= 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.OKIM6295ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.OKIM6295;
                    chip.ID = 0;
                    MDSound.okim6295 okim6295 = new MDSound.okim6295();
                    chip.Instrument = okim6295;
                    chip.Update = okim6295.Update;
                    chip.Start = okim6295.Start;
                    chip.Stop = okim6295.Stop;
                    chip.Reset = okim6295.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.OKIM6295Volume;
                    chip.Clock = vgmVirtual.OKIM6295ClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.SEGAPCMClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.SEGAPCM;
                    chip.ID = 0;
                    MDSound.segapcm segapcm = new MDSound.segapcm();
                    chip.Instrument = segapcm;
                    chip.Update = segapcm.Update;
                    chip.Start = segapcm.Start;
                    chip.Stop = segapcm.Stop;
                    chip.Reset = segapcm.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = (uint)setting.balance.SEGAPCMVolume;
                    chip.Clock = vgmVirtual.SEGAPCMClockValue;
                    chip.Option = new object[1] { vgmVirtual.SEGAPCMInterface };

                    hiyorimiDeviceFlag |= 0x2;

                    lstChips.Add(chip);
                }

                if (vgmVirtual.YM2608ClockValue != 0)
                {
                    hiyorimiDeviceFlag |= 0x1;
                }

                if (vgmVirtual.YM2151ClockValue != 0)
                {
                    hiyorimiDeviceFlag |= 0x1;
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

                //, (vgmVirtual.RF5C164ClockValue & 0x80000000) + (uint)((vgmVirtual.RF5C164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                //, (uint)(vgmVirtual.PWMClockValue * (SamplingRate / (23011361.0 / 384)))
                //, (uint)(vgmVirtual.C140ClockValue * (SamplingRate / (21390.0 / 384)) / 4.0)

                mds.setVolume(MDSound.MDSound.enmInstrumentType.SN76489, 0, setting.balance.SN76489Volume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.YM2612, 0, setting.balance.YM2612Volume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.RF5C164, 0, setting.balance.RF5C164Volume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.PWM, 0, setting.balance.PWMVolume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.C140, 0, setting.balance.C140Volume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.OKIM6258, 0, setting.balance.OKIM6258Volume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.OKIM6295, 0, setting.balance.OKIM6295Volume);
                mds.setVolume(MDSound.MDSound.enmInstrumentType.SEGAPCM, 0, setting.balance.SEGAPCMVolume);

                chipRegister.initChipRegister();



                Paused = false;
                Stopped = false;
                oneTimeReset = false;

                return true;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("ex.Message:\r\n{0}\r\nex.StackTrace:\r\n{1}",ex.Message, ex.StackTrace);
                return false;
            }

        }

        public static void FF()
        {
            vgmSpeed = (vgmSpeed == 1) ? 4 : 1;
            vgmVirtual.vgmSpeed = vgmSpeed;
            vgmReal.vgmSpeed = vgmSpeed;
        }

        public static void Slow()
        {
            vgmSpeed = (vgmSpeed == 1) ? 0.25 : 1;
            vgmVirtual.vgmSpeed = vgmSpeed;
            vgmReal.vgmSpeed = vgmSpeed;
        }

        public static void Pause()
        {

            try
            {
                    Paused = !Paused;
            }
            catch
            {
            }

        }

        public static void Fadeout()
        {
                vgmFadeout = true;
        }

        public static void Stop()
        {

            try
            {

                if (Stopped)
                {
                    trdClosed = true;
                    while (!trdStopped) { Thread.Sleep(1); };
                    return;
                }

                if (!Paused)
                {
                    NAudio.Wave.PlaybackState? ps = naudioWrap.GetPlaybackState();
                    if (ps != null && ps != NAudio.Wave.PlaybackState.Stopped)
                    {
                        vgmFadeoutCounterV = 0.1;
                        vgmFadeout = true;
                        int cnt = 0;
                        while (!Stopped && cnt < 100)
                        {
                            System.Threading.Thread.Sleep(1);
                            System.Windows.Forms.Application.DoEvents();
                            cnt++;
                        }
                    }
                }
                trdClosed = true;

                int timeout = 5000;
                while (!trdStopped)
                {
                    Thread.Sleep(1);
                    timeout--;
                    if (timeout < 1) break;
                };
                while (!Stopped) {
                    Thread.Sleep(1);
                    timeout--;
                    if (timeout < 1) break;
                };
                //if (scYM2612 != null) scYM2612.init();
                if (nscci != null) nscci.reset();
            }
            catch
            {
            }

        }

        public static void Close()
        {
            try
            {
                Stop();
                naudioWrap.Stop();
                nscci.Dispose();
                nscci = null;
            }
            catch
            {
            }
        }


        public static long GetCounter()
        {
            return vgmVirtual.Counter;
        }

        public static long GetTotalCounter()
        {
            return vgmVirtual.TotalCounter;
        }

        public static long GetLoopCounter()
        {
            return vgmVirtual.LoopCounter;
        }

        public static int[][] GetFMRegister()
        {
            return chipRegister.fmRegister;
        }

        public static int[] GetYM2151Register()
        {
            return chipRegister.fmRegisterYM2151;
        }

        public static int[][] GetYM2608Register()
        {
            return chipRegister.fmRegisterYM2608;
        }

        public static int[] GetPSGRegister()
        {
            return chipRegister.psgRegister;
        }

        public static MDSound.scd_pcm.pcm_chip_ GetRf5c164Register()
        {
            return mds.ReadRf5c164Register();
        }

        public static MDSound.c140.c140_state GetC140Register()
        {
            return mds.ReadC140Register(0);
        }

        public static int[] GetFMKeyOn()
        {
            return chipRegister.fmKeyOn;
        }

        public static int[] GetYM2151KeyOn()
        {
            return chipRegister.fmKeyOnYM2151;
        }

        public static int[] GetYM2608KeyOn()
        {
            return chipRegister.fmKeyOnYM2608;
        }

        public static int[][] GetFMVolume()
        {
            return chipRegister.GetFMVolume();
        }

        public static int[][] GetYM2151Volume()
        {
            return chipRegister.GetYM2151Volume();
        }

        public static int[][] GetYM2608Volume()
        {
            return chipRegister.GetYM2608Volume();
        }

        public static int[][] GetYM2608RhythmVolume()
        {
            return chipRegister.GetYM2608RhythmVolume();
        }

        public static int[] GetYM2608AdpcmVolume()
        {
            return chipRegister.GetYM2608AdpcmVolume();
        }

        public static void updateVol()
        {
            chipRegister.updateVol();
        }

        public static int[] GetFMCh3SlotVolume()
        {
            return chipRegister.GetFMCh3SlotVolume();
        }

        public static int[] GetYM2608Ch3SlotVolume()
        {
            return chipRegister.GetYM2608Ch3SlotVolume();
        }

        public static int[][] GetPSGVolume()
        {
            return chipRegister.GetPSGVolume();
        }

        public static int[][] GetRf5c164Volume()
        {
            return mds.ReadRf5c164Volume();
        }

        public static void setFMMask(int ch)
        {
            mds.setFMMask(1 << ch);
            chipRegister.setMaskYM2612(ch, true);
        }

        public static void setPSGMask(int ch)
        {
            mds.setPSGMask(1 << ch);
        }

        public static void resetFMMask(int ch)
        {
            mds.resetFMMask(1 << ch);
            chipRegister.setMaskYM2612(ch, false);
        }

        public static void resetPSGMask(int ch)
        {
            mds.resetPSGMask(1 << ch);
        }

        public static uint GetVgmCurLoopCounter()
        {
            uint cnt = 0;

            if (vgmVirtual != null)
            {
                cnt = vgmVirtual.vgmCurLoop;
            }
            if (vgmReal != null)
            {
                cnt = Math.Min(vgmReal.vgmCurLoop, cnt);
            }

            return cnt;
        }

        public static bool GetVGMStopped()
        {
            bool v = vgmVirtual.Stopped;
            bool r = vgmReal.Stopped;

            return v && r;
        }

        public static bool GetIsDataBlock(vgm.enmModel model)
        {
            return (model == vgm.enmModel.VirtualModel) ? vgmVirtual.isDataBlock : vgmReal.isDataBlock;
        }

        public static bool GetIsPcmRAMWrite(vgm.enmModel model)
        {
            return (model == vgm.enmModel.VirtualModel) ? vgmVirtual.isPcmRAMWrite : vgmReal.isPcmRAMWrite;
        }


        private static NScci.NSoundChip getChip(int SoundLocation, int BusID, int SoundChip)
        {
            int ifc = nscci.getInterfaceCount();
            for (int i = 0; i < ifc; i++)
            {
                NSoundInterface sif = nscci.getInterface(i);
                int scc = sif.getSoundChipCount();
                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    if (info.getdSoundLocation() == SoundLocation && info.getdBusID() == BusID && info.getiSoundChip() == SoundChip)
                    {
                        return sc;
                    }
                }
            }

            return null;
        }

        private static List<NScci.NSoundChip> getChipList(int chipType)
        {
            List<NScci.NSoundChip> ret = new List<NSoundChip>();

            int ifc = nscci.getInterfaceCount();
            for (int i = 0; i < ifc; i++)
            {
                NSoundInterface sif = nscci.getInterface(i);
                int scc = sif.getSoundChipCount();
                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    int t = sc.getSoundChipType();
                    if (t == chipType)
                    {
                        ret.Add(sc);
                    }
                }
            }

            return ret;
        }

        private static void NaudioWrap_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                System.Windows.Forms.MessageBox.Show(
                    string.Format("デバイスが何らかの原因で停止しました。\r\nメッセージ:\r\n{0}", e.Exception.Message)
                    , "エラー"
                    , System.Windows.Forms.MessageBoxButtons.OK
                    , System.Windows.Forms.MessageBoxIcon.Error);
                try
                {
                    naudioWrap.Stop();
                }
                catch
                {
                }
            }
            else
            {
                Stop();
            }
        }

        private static void trdVgmRealFunction()
        {
            double o = sw.ElapsedTicks / swFreq;
            double step = 1 / (double)SamplingRate;

            trdStopped = false;
            while (!trdClosed)
            {
                Thread.Sleep(0);

                double el1 = sw.ElapsedTicks / swFreq;
                if (el1 - o < step) continue;

                o += step;
                //while (el1 - o >= step) o += step;

                if (Stopped || Paused)
                {
                    if (nscci != null && !oneTimeReset)
                    {
                        nscci.reset();
                        oneTimeReset = true;
                    }
                    continue;
                }
                if (vgmVirtual.isDataBlock) { continue;}

                if (vgmFadeout)
                {
                    if (vgmRealFadeoutVol != 1000) vgmRealFadeoutVolWait--;
                    if (vgmRealFadeoutVolWait == 0)
                    {
                        chipRegister.setFadeoutVolYM2151(vgmRealFadeoutVol);
                        chipRegister.setFadeoutVolYM2608(vgmRealFadeoutVol);
                        chipRegister.setFadeoutVolYM2612(vgmRealFadeoutVol);
                        chipRegister.setFadeoutVolSN76489(vgmRealFadeoutVol++);

                        vgmRealFadeoutVol = Math.Min(127, vgmRealFadeoutVol);
                        if (vgmRealFadeoutVol == 127)
                        {
                            if (nscci != null) nscci.reset();
                            vgmRealFadeoutVolWait = 1000;
                        }
                        else
                        {
                            vgmRealFadeoutVolWait = 700 - vgmRealFadeoutVol * 2;
                        }
                    }
                }

                if (hiyorimiNecessary)
                {
                    long v = vgmReal.vgmFrameCounter - vgmVirtual.vgmFrameCounter;
                    long d = SamplingRate * (setting.LatencySCCI - SamplingRate * setting.LatencyEmulation) / 1000;
                    long l = getLatency() / 4;

                    int m = 0;
                    if (d >= 0)
                    {
                        if (v >= d - l && v <= d + l) m = 0;
                        else m = (v + d > l) ? 1 : 2;
                    }
                    else
                    {
                        d = Math.Abs(SamplingRate * ((uint)setting.LatencyEmulation - (uint)setting.LatencySCCI) / 1000);
                        if (v >= d - l && v <= d + l) m = 0; 
                        else m = (v - d > l) ? 1 : 2;
                    }

                    switch (m)
                    {
                        case 0: //x1
                            vgmReal.oneFrameVGM();
                            break;
                        case 1: //x1/2
                            hiyorimiEven++;
                            if (hiyorimiEven > 1)
                            {
                                vgmReal.oneFrameVGM();
                                hiyorimiEven = 0;
                            }
                            break;
                        case 2: //x2
                            vgmReal.oneFrameVGM();
                            vgmReal.oneFrameVGM();
                            break;
                    }
                }
                else
                {
                    vgmReal.oneFrameVGM();
                }
            }
            trdStopped = true;
        }

        internal static int trdVgmVirtualFunction(short[] buffer, int offset, int sampleCount)
        {
            try
            {
                int i;

                if (Stopped || Paused)
                {

                    return mds.Update(buffer, offset, sampleCount, null);

                }
                if (vgmReal.isDataBlock) { return mds.Update(buffer, offset, sampleCount, null); }

                int cnt;
                cnt = mds.Update(buffer, offset, sampleCount, vgmVirtual.oneFrameVGM);

                if (vgmFadeout)
                {

                    for (i = 0; i < sampleCount; i++)
                    {
                        buffer[offset + i] = (short)(buffer[offset + i] * vgmFadeoutCounter);


                        vgmFadeoutCounter -= vgmFadeoutCounterV;
                        //vgmFadeoutCounterV += 0.00001;
                        if (vgmFadeoutCounterV >= 0.004 && vgmFadeoutCounterV != 0.1)
                        {
                            vgmFadeoutCounterV = 0.004;
                        }

                        if (vgmFadeoutCounter < 0.0)
                        {
                            vgmFadeoutCounter = 0.0;
                        }
                    }
                }

                if (vgmFadeoutCounter == 0.0)
                {
                    MDSound.MDSound.Chip[] chips = new MDSound.MDSound.Chip[8];

                    chips[0] = new MDSound.MDSound.Chip();
                    chips[0].type = MDSound.MDSound.enmInstrumentType.SN76489;
                    chips[0].ID = 0;
                    MDSound.sn76489 sn76489 = new MDSound.sn76489();
                    chips[0].Instrument = sn76489;
                    chips[0].Update = sn76489.Update;
                    chips[0].Start = sn76489.Start;
                    chips[0].Stop = sn76489.Stop;
                    chips[0].Reset = sn76489.Reset;
                    chips[0].SamplingRate = SamplingRate;
                    chips[0].Volume = 100;
                    chips[0].Clock = vgm.defaultSN76489ClockValue;
                    chips[0].Option = null;

                    chips[1] = new MDSound.MDSound.Chip();
                    chips[1].type = MDSound.MDSound.enmInstrumentType.YM2612;
                    chips[1].ID = 0;
                    MDSound.ym2612 ym2612 = new MDSound.ym2612();
                    chips[1].Instrument = ym2612;
                    chips[1].Update = ym2612.Update;
                    chips[1].Start = ym2612.Start;
                    chips[1].Stop = ym2612.Stop;
                    chips[1].Reset = ym2612.Reset;
                    chips[1].SamplingRate = SamplingRate;
                    chips[1].Volume = 100;
                    chips[1].Clock = vgm.defaultYM2612ClockValue;
                    chips[1].Option = null;

                    chips[2] = new MDSound.MDSound.Chip();
                    chips[2].type = MDSound.MDSound.enmInstrumentType.RF5C164;
                    chips[2].ID = 0;
                    MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();
                    chips[2].Instrument = rf5c164;
                    chips[2].Update = rf5c164.Update;
                    chips[2].Start = rf5c164.Start;
                    chips[2].Stop = rf5c164.Stop;
                    chips[2].Reset = rf5c164.Reset;
                    chips[2].SamplingRate = SamplingRate;
                    chips[2].Volume = 100;
                    chips[2].Clock = vgm.defaultRF5C164ClockValue;
                    chips[2].Option = null;

                    chips[3] = new MDSound.MDSound.Chip();
                    chips[3].type = MDSound.MDSound.enmInstrumentType.PWM;
                    chips[3].ID = 0;
                    MDSound.pwm pwm = new MDSound.pwm();
                    chips[3].Instrument = pwm;
                    chips[3].Update = pwm.Update;
                    chips[3].Start = pwm.Start;
                    chips[3].Stop = pwm.Stop;
                    chips[3].Reset = pwm.Reset;
                    chips[3].SamplingRate = SamplingRate;
                    chips[3].Volume = 100;
                    chips[3].Clock = vgm.defaultPWMClockValue;
                    chips[3].Option = null;

                    chips[4] = new MDSound.MDSound.Chip();
                    chips[4].type = MDSound.MDSound.enmInstrumentType.C140;
                    chips[4].ID = 0;
                    MDSound.c140 c140 = new MDSound.c140();
                    chips[4].Instrument = c140;
                    chips[4].Update = c140.Update;
                    chips[4].Start = c140.Start;
                    chips[4].Stop = c140.Stop;
                    chips[4].Reset = c140.Reset;
                    chips[4].SamplingRate = SamplingRate;
                    chips[4].Volume = 100;
                    chips[4].Clock = vgm.defaultC140ClockValue;
                    chips[4].Option = new object[1] { vgm.defaultC140Type };

                    chips[5] = new MDSound.MDSound.Chip();
                    chips[5].type = MDSound.MDSound.enmInstrumentType.OKIM6258;
                    chips[5].ID = 0;
                    MDSound.okim6258 okim6258 = new MDSound.okim6258();
                    chips[5].Instrument = okim6258;
                    chips[5].Update = okim6258.Update;
                    chips[5].Start = okim6258.Start;
                    chips[5].Stop = okim6258.Stop;
                    chips[5].Reset = okim6258.Reset;
                    chips[5].SamplingRate = SamplingRate;
                    chips[5].Volume = 100;
                    chips[5].Clock = vgm.defaultOKIM6258ClockValue;
                    chips[5].Option = new object[1] { (int)0 };

                    chips[6] = new MDSound.MDSound.Chip();
                    chips[6].type = MDSound.MDSound.enmInstrumentType.OKIM6295;
                    chips[6].ID = 0;
                    MDSound.okim6295 okim6295 = new MDSound.okim6295();
                    chips[6].Instrument = okim6295;
                    chips[6].Update = okim6295.Update;
                    chips[6].Start = okim6295.Start;
                    chips[6].Stop = okim6295.Stop;
                    chips[6].Reset = okim6295.Reset;
                    chips[6].SamplingRate = SamplingRate;
                    chips[6].Volume = 100;
                    chips[6].Clock = vgm.defaultOKIM6295ClockValue;
                    chips[6].Option = null;

                    chips[7] = new MDSound.MDSound.Chip();
                    chips[7].type = MDSound.MDSound.enmInstrumentType.SEGAPCM;
                    chips[7].ID = 0;
                    MDSound.segapcm segapcm = new MDSound.segapcm();
                    chips[7].Instrument = segapcm;
                    chips[7].Update = segapcm.Update;
                    chips[7].Start = segapcm.Start;
                    chips[7].Stop = segapcm.Stop;
                    chips[7].Reset = segapcm.Reset;
                    chips[7].SamplingRate = SamplingRate;
                    chips[7].Volume = 100;
                    chips[7].Clock = vgm.defaultSEGAPCMClockValue;
                    chips[7].Option = new object[1] { (int)0 };

                    if (mds == null)
                        mds = new MDSound.MDSound(SamplingRate, samplingBuffer, chips);
                    else
                        mds.Init(SamplingRate, samplingBuffer, chips);

                    //, (vgm.defaultRF5C164ClockValue & 0x80000000) + (uint)((vgm.defaultRF5C164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                    //, (uint)(vgm.defaultPWMClockValue * (SamplingRate / (23011361.0 / 384)))
                    //, (uint)(vgm.defaultC140ClockValue * (SamplingRate / (21390.0 / 384)))

                    mds.setVolume(MDSound.MDSound.enmInstrumentType.SN76489, 0, setting.balance.SN76489Volume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.YM2612, 0, setting.balance.YM2612Volume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.RF5C164, 0, setting.balance.RF5C164Volume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.PWM, 0, setting.balance.PWMVolume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.C140, 0, setting.balance.C140Volume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.OKIM6258, 0, setting.balance.OKIM6258Volume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.OKIM6295, 0, setting.balance.OKIM6295Volume);
                    mds.setVolume(MDSound.MDSound.enmInstrumentType.SEGAPCM, 0, setting.balance.SEGAPCMVolume);

                    Stopped = true;
                }

                return cnt;

            }
            catch(Exception ex)
            {
                System.Console.WriteLine("ex.Message:\r\n{0}\r\nex.StackTrace:\r\n{1}", ex.Message, ex.StackTrace);
                fatalError = true;
                Stopped = true;
            }
            return -1;
        }

        public static long getVirtualFrameCounter()
        {
            if (vgmVirtual == null) return -1;
            return vgmVirtual.vgmFrameCounter;
        }

        public static long getRealFrameCounter()
        {
            if (vgmReal == null) return -1;
            return vgmReal.vgmFrameCounter;
        }


        private static UInt32 getLE16(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100;

            return dat;
        }

        private static UInt32 getLE24(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100 + (UInt32)vgmBuf[adr + 2] * 0x10000;

            return dat;
        }

        private static UInt32 getLE32(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100 + (UInt32)vgmBuf[adr + 2] * 0x10000 + (UInt32)vgmBuf[adr + 3] * 0x1000000;

            return dat;
        }

        private static UInt32 getLE32(byte[] buf,UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100 + (UInt32)buf[adr + 2] * 0x10000 + (UInt32)buf[adr + 3] * 0x1000000;

            return dat;
        }

    }


}
