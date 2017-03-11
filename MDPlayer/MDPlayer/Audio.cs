using System;
using System.Collections.Generic;
using NScci;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using MDSound;

namespace MDPlayer
{
    public class Audio
    {

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
        private static MDSound.MDSound mds = null;
        private static NAudioWrap naudioWrap;

        private static NScci.NScci nscci;
        private static NSoundChip[] scYM2612 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scSN76489 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2151 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2608 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2203 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2610 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scAY8910 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2413 = new NSoundChip[2] { null, null };

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
        private static int StepCounter = 0;

        private static Setting setting = null;

        public static baseDriver vgmVirtual = null;
        public static baseDriver vgmReal = null;
        public static baseDriver nrtVirtual = null;
        public static baseDriver nrtReal = null;

        private static bool oneTimeReset = false;
        private static int hiyorimiEven = 0;
        private static bool hiyorimiNecessary = false;

        public static int ChipPriOPN = 0;
        public static int ChipPriOPN2 = 0;
        public static int ChipPriOPNA = 0;
        public static int ChipPriOPNB = 0;
        public static int ChipPriOPM = 0;
        public static int ChipPriDCSG = 0;
        public static int ChipPriRF5C = 0;
        public static int ChipPriPWM = 0;
        public static int ChipPriOKI5 = 0;
        public static int ChipPriOKI9 = 0;
        public static int ChipPriC140 = 0;
        public static int ChipPriSPCM = 0;
        //public static int ChipPriPSG = 0;
        public static int ChipPriAY10 = 0;
        public static int ChipPriOPLL = 0;

        public static int ChipSecOPN = 0;
        public static int ChipSecOPN2 = 0;
        public static int ChipSecOPNA = 0;
        public static int ChipSecOPNB = 0;
        public static int ChipSecOPM = 0;
        public static int ChipSecDCSG = 0;
        public static int ChipSecRF5C = 0;
        public static int ChipSecPWM = 0;
        public static int ChipSecOKI5 = 0;
        public static int ChipSecOKI9 = 0;
        public static int ChipSecC140 = 0;
        public static int ChipSecSPCM = 0;
        //public static int ChipSecPSG = 0;
        public static int ChipSecAY10 = 0;
        public static int ChipSecOPLL = 0;

        private static int MasterVolume = 0;
        static int[] chips = new int[256];
        private static string PlayingFileName;
        private static enmFileFormat PlayingFileFormat;



        public static PlayList.music getMusic(string file,byte[] buf,string zipFile=null)
        {
            PlayList.music music = new PlayList.music();

            music.format = enmFileFormat.unknown;
            music.fileName = file;
            music.zipFileName = zipFile;
            music.title = "unknown";
            music.game = "unknown";

            if (file.ToLower().LastIndexOf(".nrd") != -1)
            {

                music.format = enmFileFormat.NRTDRV;
                uint index = 42;
                GD3 gd3 = nrtVirtual.getGD3Info(buf, index);
                music.title = gd3.TrackName;
                music.titleJ = gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else
            {
                if (buf.Length < 0x40) return music;
                if (getLE32(buf, 0x00) != vgm.FCC_VGM) return music;

                music.format = enmFileFormat.VGM;
                uint version = getLE32(buf, 0x08);
                string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint vgmGd3 = getLE32(buf, 0x14);
                GD3 gd3 = new GD3();
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = getLE32(buf, vgmGd3 + 0x14);
                    if (vgmGd3Id != vgm.FCC_GD3) return music;
                    gd3 = vgmVirtual.getGD3Info(buf, vgmGd3);
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
            }

            return music;
        }

        private static string getNRDString(byte[] buf, ref int index)
        {
            if (buf == null || buf.Length < 1 || index < 0 || index >= buf.Length) return "";

            try
            {
                List<byte> lst = new List<byte>();
                for (; buf[index] != 0; index++)
                {
                    lst.Add(buf[index]);
                }

                string n = System.Text.Encoding.GetEncoding(932).GetString(lst.ToArray());
                index++;

                return n;
            }
            catch(Exception e)
            {
                log.ForcedWrite(e);
            }
            return "";
        }

        public static void Init(Setting setting)
        {
           log.ForcedWrite("Audio:Init:Begin");

            vgmVirtual = new vgm();
            vgmReal = new vgm();
            nrtVirtual = new NRTDRV();
            nrtReal = new NRTDRV();

            log.ForcedWrite("Audio:Init:STEP 01");

            naudioWrap = new NAudioWrap((int)SamplingRate, trdVgmVirtualFunction);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;

            log.ForcedWrite("Audio:Init:STEP 02");

            Audio.setting = setting.Copy();
            vgmVirtual.setting = setting;
            vgmReal.setting = setting;
            nrtVirtual.setting = setting;
            nrtReal.setting = setting;

            log.ForcedWrite("Audio:Init:STEP 03");

            MDSound.MDSound.Chip[] chips = new MDSound.MDSound.Chip[1];

            chips[0] = new MDSound.MDSound.Chip();
            chips[0].type = MDSound.MDSound.enmInstrumentType.SN76489;
            chips[0].ID = 0;
            sn76489 sn76489 = new sn76489();
            chips[0].Instrument = sn76489;
            chips[0].Update = sn76489.Update;
            chips[0].Start = sn76489.Start;
            chips[0].Stop = sn76489.Stop;
            chips[0].Reset = sn76489.Reset;
            chips[0].SamplingRate = SamplingRate;
            chips[0].Volume = 100;
            chips[0].Clock = vgm.defaultSN76489ClockValue;
            chips[0].Option = null;

            log.ForcedWrite("Audio:Init:STEP 04");

            if (mds == null)
                mds = new MDSound.MDSound(SamplingRate, samplingBuffer, chips);
            else
                mds.Init(SamplingRate, samplingBuffer, chips);

            log.ForcedWrite("Audio:Init:STEP 05");

            nscci = new NScci.NScci();
            scYM2612[0] = getChip(Audio.setting.YM2612Type);
            if (scYM2612[0] != null) scYM2612[0].init();
            scSN76489[0] = getChip(Audio.setting.SN76489Type);
            if (scSN76489[0] != null) scSN76489[0].init();
            scYM2608[0] = getChip(Audio.setting.YM2608Type);
            if (scYM2608[0] != null) scYM2608[0].init();
            scYM2151[0] = getChip(Audio.setting.YM2151Type);
            if (scYM2151[0] != null) scYM2151[0].init();
            scYM2203[0] = getChip(Audio.setting.YM2203Type);
            if (scYM2203[0] != null) scYM2203[0].init();
            scYM2610[0] = getChip(Audio.setting.YM2610Type);
            if (scYM2610[0] != null) scYM2610[0].init();
            scAY8910[0] = getChip(Audio.setting.AY8910Type);
            if (scAY8910[0] != null) scAY8910[0].init();
            scYM2413[0] = getChip(Audio.setting.YM2413Type);
            if (scYM2413[0] != null) scYM2413[0].init();

            scYM2612[1] = getChip(Audio.setting.YM2612SType);
            if (scYM2612[1] != null) scYM2612[1].init();
            scSN76489[1] = getChip(Audio.setting.SN76489SType);
            if (scSN76489[1] != null) scSN76489[1].init();
            scYM2608[1] = getChip(Audio.setting.YM2608SType);
            if (scYM2608[1] != null) scYM2608[1].init();
            scYM2151[1] = getChip(Audio.setting.YM2151SType);
            if (scYM2151[1] != null) scYM2151[1].init();
            scYM2203[1] = getChip(Audio.setting.YM2203SType);
            if (scYM2203[1] != null) scYM2203[1].init();
            scYM2610[1] = getChip(Audio.setting.YM2610SType);
            if (scYM2610[1] != null) scYM2610[1].init();
            scAY8910[1] = getChip(Audio.setting.AY8910SType);
            if (scAY8910[1] != null) scAY8910[1].init();
            scYM2413[1] = getChip(Audio.setting.YM2413SType);
            if (scYM2413[1] != null) scYM2413[1].init();

            chipRegister = new ChipRegister(
                setting
                , mds
                , scYM2612, scSN76489, scYM2608, scYM2151, scYM2203, scYM2610, scAY8910, scYM2413
                , new Setting.ChipType[] { setting.YM2612Type, setting.YM2612SType }
                , new Setting.ChipType[] { setting.SN76489Type, setting.SN76489SType }
                , new Setting.ChipType[] { setting.YM2608Type, setting.YM2608SType }
                , new Setting.ChipType[] { setting.YM2151Type, setting.YM2151SType }
                , new Setting.ChipType[] { setting.YM2203Type, setting.YM2203SType }
                , new Setting.ChipType[] { setting.YM2610Type, setting.YM2610SType }
                , new Setting.ChipType[] { setting.AY8910Type, setting.AY8910SType }
                , new Setting.ChipType[] { setting.YM2413Type, setting.YM2413SType }
                );
            chipRegister.initChipRegister();

            log.ForcedWrite("Audio:Init:STEP 06");

            ((vgm)vgmVirtual).dacControl.chipRegister = chipRegister;
            ((vgm)vgmVirtual).dacControl.model = enmModel.VirtualModel;

            ((vgm)vgmReal).dacControl.chipRegister = chipRegister;
            ((vgm)vgmReal).dacControl.model = enmModel.RealModel;

            Paused = false;
            Stopped = true;
            fatalError = false;
            oneTimeReset = false;

            log.ForcedWrite("Audio:Init:STEP 07");

            naudioWrap.Start(Audio.setting);

            log.ForcedWrite("Audio:Init:Complete");

        }

        public static List<NScci.NSoundChip> getChipList(enmScciChipType scciChipType)
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
                    if (t == (int)scciChipType)
                    {
                        ret.Add(sc);
                    }
                }
            }

            return ret;
        }

        public static int getLatency()
        {
            if (setting.outputDevice.DeviceType != 3)
            {
                return (int)SamplingRate * setting.outputDevice.Latency / 1000;
            }
            return naudioWrap.getAsioLatency();
        }

        public static void SetVGMBuffer(enmFileFormat format, byte[] srcBuf,string playingFileName)
        {
            Stop();
            PlayingFileFormat = format;
            vgmBuf = srcBuf;
            PlayingFileName = playingFileName;
            chipRegister.SetFileName(playingFileName);
        }

        public static bool nPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

                chipRegister.setFadeoutVolYM2151(0, 0);
                chipRegister.setFadeoutVolYM2151(1, 0);
                
                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;
                chipRegister.setFadeoutVolYM2203(0, 0);
                chipRegister.setFadeoutVolYM2203(1, 0);
                chipRegister.setFadeoutVolYM2608(0, 0);
                chipRegister.setFadeoutVolYM2608(1, 0);
                chipRegister.setFadeoutVolYM2151(0, 0);
                chipRegister.setFadeoutVolYM2151(1, 0);
                chipRegister.setFadeoutVolYM2612(0, 0);
                chipRegister.setFadeoutVolYM2612(1, 0);
                chipRegister.setFadeoutVolSN76489(0, 0);
                chipRegister.setFadeoutVolSN76489(1, 0);
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

                ChipPriOPN = 0;
                ChipPriOPN2 = 0;
                ChipPriOPNA = 0;
                ChipPriOPNB = 0;
                ChipPriOPM = 0;
                ChipPriDCSG = 0;
                ChipPriRF5C = 0;
                ChipPriPWM = 0;
                ChipPriOKI5 = 0;
                ChipPriOKI9 = 0;
                ChipPriC140 = 0;
                ChipPriSPCM = 0;
                ChipPriAY10 = 0;
                ChipPriOPLL = 0;
                //ChipPriPSG = 0;
                ChipSecOPN = 0;
                ChipSecOPN2 = 0;
                ChipSecOPNA = 0;
                ChipSecOPNB = 0;
                ChipSecOPM = 0;
                ChipSecDCSG = 0;
                ChipSecRF5C = 0;
                ChipSecPWM = 0;
                ChipSecOKI5 = 0;
                ChipSecOKI9 = 0;
                ChipSecC140 = 0;
                ChipSecSPCM = 0;
                ChipSecAY10 = 0;
                ChipSecOPLL = 0;
                //ChipSecPSG = 0;

                MasterVolume = setting.balance.MasterVolume;

                MDSound.ym2151 ym2151 = new MDSound.ym2151();
                for (int i = 0; i < 2; i++)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                    chip.ID = (byte)i;
                    chip.Instrument = ym2151;
                    chip.Update = ym2151.Update;
                    chip.Start = ym2151.Start;
                    chip.Stop = ym2151.Stop;
                    chip.Reset = ym2151.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = setting.balance.YM2151Volume;
                    chip.Clock = 4000000;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    if (i == 0) ChipPriOPM = 1;
                    else ChipSecOPM = 1;

                    lstChips.Add(chip);
                }

                MDSound.ay8910 ay8910 = new MDSound.ay8910();
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                    chip.ID = (byte)0;
                    chip.Instrument = ay8910;
                    chip.Update = ay8910.Update;
                    chip.Start = ay8910.Start;
                    chip.Stop = ay8910.Stop;
                    chip.Reset = ay8910.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = setting.balance.AY8910Volume;
                    chip.Clock = 2000000/2;
                    chip.Option = null;

                hiyorimiDeviceFlag |= 0x1;
                ChipPriAY10 = 1;

                lstChips.Add(chip);

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2151Volume(setting.balance.YM2151Volume);
                SetAY8910Volume(setting.balance.AY8910Volume);

                nrtVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, enmUseChip.YM2151 | enmUseChip.AY8910, 0);
                nrtReal.init(vgmBuf, chipRegister, enmModel.RealModel, enmUseChip.YM2151 | enmUseChip.AY8910, 0);
                ((NRTDRV)nrtVirtual).Call(0);//
                ((NRTDRV)nrtVirtual).Call(1);//MPLAY
                ((NRTDRV)nrtReal).Call(0);//
                ((NRTDRV)nrtReal).Call(1);//MPLAY

                Paused = false;
                Stopped = false;
                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool Play(Setting setting)
        {

            if (PlayingFileFormat == enmFileFormat.NRTDRV)
            {
                return nPlay(setting);
            }

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

                enmUseChip usechip = enmUseChip.Unuse;
                if (setting.YM2612Type.UseScci)
                {
                    if (setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip |= enmUseChip.YM2612Ch6;
                    }
                }
                else
                {
                    usechip |= enmUseChip.YM2612;
                    usechip |= enmUseChip.YM2612Ch6;
                }
                if (!setting.SN76489Type.UseScci)
                {
                    usechip |= enmUseChip.SN76489;
                }
                usechip |= enmUseChip.RF5C164;
                usechip |= enmUseChip.PWM;

                if (!vgmVirtual.init(vgmBuf
                    , chipRegister
                    , enmModel.VirtualModel
                    , usechip
                    , SamplingRate * (uint)setting.LatencyEmulation / 1000
                    ))
                    return false;

                usechip = enmUseChip.Unuse;
                if (setting.YM2612Type.UseScci)
                {
                    usechip |= enmUseChip.YM2612;
                    if (!setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip |= enmUseChip.YM2612Ch6;
                    }
                }

                if (!vgmReal.init(vgmBuf
                    , chipRegister
                    , enmModel.RealModel
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
                chipRegister.setFadeoutVolYM2203(0, 0);
                chipRegister.setFadeoutVolYM2203(1, 0);
                chipRegister.setFadeoutVolYM2608(0, 0);
                chipRegister.setFadeoutVolYM2608(1, 0);
                chipRegister.setFadeoutVolYM2151(0, 0);
                chipRegister.setFadeoutVolYM2151(1, 0);
                chipRegister.setFadeoutVolYM2612(0, 0);
                chipRegister.setFadeoutVolYM2612(1, 0);
                chipRegister.setFadeoutVolSN76489(0, 0);
                chipRegister.setFadeoutVolSN76489(1, 0);
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

                ChipPriOPN = 0;
                ChipPriOPN2 = 0;
                ChipPriOPNA = 0;
                ChipPriOPNB = 0;
                ChipPriOPM = 0;
                ChipPriDCSG = 0;
                ChipPriRF5C = 0;
                ChipPriPWM = 0;
                ChipPriOKI5 = 0;
                ChipPriOKI9 = 0;
                ChipPriC140 = 0;
                ChipPriSPCM = 0;
                ChipPriAY10 = 0;
                ChipPriOPLL = 0;
                //ChipPriPSG = 0;
                ChipSecOPN = 0;
                ChipSecOPN2 = 0;
                ChipSecOPNA = 0;
                ChipSecOPNB = 0;
                ChipSecOPM = 0;
                ChipSecDCSG = 0;
                ChipSecRF5C = 0;
                ChipSecPWM = 0;
                ChipSecOKI5 = 0;
                ChipSecOKI9 = 0;
                ChipSecC140 = 0;
                ChipSecSPCM = 0;
                ChipSecAY10 = 0;
                ChipSecOPLL = 0;

                MasterVolume = setting.balance.MasterVolume;

                if (((vgm)vgmVirtual).SN76489ClockValue != 0)
                {
                    MDSound.sn76489 sn76489 = new MDSound.sn76489();

                    for (int i = 0; i < (((vgm)vgmVirtual).SN76489DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                        chip.ID = (byte)i;
                        chip.Instrument = sn76489;
                        chip.Update = sn76489.Update;
                        chip.Start = sn76489.Start;
                        chip.Stop = sn76489.Stop;
                        chip.Reset = sn76489.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.SN76489Volume;
                        chip.Clock = ((vgm)vgmVirtual).SN76489ClockValue;
                        chip.Option = null;
                        if (i == 0) ChipPriDCSG = 1;
                        else ChipSecDCSG = 1;

                        hiyorimiDeviceFlag |= (setting.SN76489Type.UseScci) ? 0x1 : 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).YM2612ClockValue != 0)
                {
                    MDSound.ym2612 ym2612 = new MDSound.ym2612();

                    for (int i = 0; i < (((vgm)vgmVirtual).YM2612DualChipFlag ? 2 : 1); i++)
                    {
                        //MDSound.ym2612 ym2612 = new MDSound.ym2612();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                        chip.ID = (byte)i;
                        chip.Instrument = ym2612;
                        chip.Update = ym2612.Update;
                        chip.Start = ym2612.Start;
                        chip.Stop = ym2612.Stop;
                        chip.Reset = ym2612.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.YM2612Volume;
                        chip.Clock = ((vgm)vgmVirtual).YM2612ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci) ? 0x1 : 0x2;
                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci && setting.YM2612Type.OnlyPCMEmulation) ? 0x2 : 0x0;

                        if (i == 0) ChipPriOPN2 = 1;
                        else ChipSecOPN2 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).RF5C164ClockValue != 0)
                {
                    MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();

                    for (int i = 0; i < (((vgm)vgmVirtual).RF5C164DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.RF5C164;
                        chip.ID = (byte)i;
                        chip.Instrument = rf5c164;
                        chip.Update = rf5c164.Update;
                        chip.Start = rf5c164.Start;
                        chip.Stop = rf5c164.Stop;
                        chip.Reset = rf5c164.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.RF5C164Volume;
                        chip.Clock = ((vgm)vgmVirtual).RF5C164ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriRF5C = 1;
                        else ChipSecRF5C = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).PWMClockValue != 0)
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
                    chip.Volume = setting.balance.PWMVolume;
                    chip.Clock = ((vgm)vgmVirtual).PWMClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriPWM = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)vgmVirtual).C140ClockValue != 0)
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
                    chip.Volume = setting.balance.C140Volume;
                    chip.Clock = ((vgm)vgmVirtual).C140ClockValue;
                    chip.Option = new object[1] { ((vgm)vgmVirtual).C140Type };

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriC140 = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)vgmVirtual).OKIM6258ClockValue != 0)
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
                    chip.Volume = setting.balance.OKIM6258Volume;
                    chip.Clock = ((vgm)vgmVirtual).OKIM6258ClockValue;
                    //chip.Option = new object[1] { (int)vgmVirtual.OKIM6258Type };
                    chip.Option = new object[1] { 6 };

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriOKI5 = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)vgmVirtual).OKIM6295ClockValue != 0)
                {
                    MDSound.okim6295 okim6295 = new MDSound.okim6295();
                    for (int i = 0; i < (((vgm)vgmVirtual).OKIM6295DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.OKIM6295;
                        chip.ID = (byte)i;
                        chip.Instrument = okim6295;
                        chip.Update = okim6295.Update;
                        chip.Start = okim6295.Start;
                        chip.Stop = okim6295.Stop;
                        chip.Reset = okim6295.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.OKIM6295Volume;
                        chip.Clock = ((vgm)vgmVirtual).OKIM6295ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOKI9 = 1;
                        else ChipSecOKI9 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).SEGAPCMClockValue != 0)
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
                    chip.Volume = setting.balance.SEGAPCMVolume;
                    chip.Clock = ((vgm)vgmVirtual).SEGAPCMClockValue;
                    chip.Option = new object[1] { ((vgm)vgmVirtual).SEGAPCMInterface };

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriSPCM = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)vgmVirtual).YM2608ClockValue != 0)
                {
                    MDSound.ym2608 ym2608 = new MDSound.ym2608();
                    for (int i = 0; i < (((vgm)vgmVirtual).YM2608DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                        chip.ID = (byte)i;
                        chip.Instrument = ym2608;
                        chip.Update = ym2608.Update;
                        chip.Start = ym2608.Start;
                        chip.Stop = ym2608.Stop;
                        chip.Reset = ym2608.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.YM2608Volume;
                        chip.Clock = ((vgm)vgmVirtual).YM2608ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPNA = 1;
                        else ChipSecOPNA = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).YM2151ClockValue != 0)
                {
                    MDSound.ym2151 ym2151 = new MDSound.ym2151();
                    for (int i = 0; i < (((vgm)vgmVirtual).YM2151DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                        chip.ID = (byte)i;
                        chip.Instrument = ym2151;
                        chip.Update = ym2151.Update;
                        chip.Start = ym2151.Start;
                        chip.Stop = ym2151.Stop;
                        chip.Reset = ym2151.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.YM2151Volume;
                        chip.Clock = ((vgm)vgmVirtual).YM2151ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPM = 1;
                        else ChipSecOPM = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).YM2203ClockValue != 0)
                {
                    MDSound.ym2203 ym2203 = new MDSound.ym2203();
                    for (int i = 0; i < (((vgm)vgmVirtual).YM2203DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2203;
                        chip.ID = (byte)i;
                        chip.Instrument = ym2203;
                        chip.Update = ym2203.Update;
                        chip.Start = ym2203.Start;
                        chip.Stop = ym2203.Stop;
                        chip.Reset = ym2203.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.YM2203Volume;
                        chip.Clock = ((vgm)vgmVirtual).YM2203ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPN = 1;
                        else ChipSecOPN = 1;

                        lstChips.Add(chip);

                    }
                }

                if (((vgm)vgmVirtual).YM2610ClockValue != 0)
                {
                    MDSound.ym2610 ym2610 = new MDSound.ym2610();
                    for (int i = 0; i < (((vgm)vgmVirtual).YM2610DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2610;
                        chip.ID = (byte)i;
                        chip.Instrument = ym2610;
                        chip.Update = ym2610.Update;
                        chip.Start = ym2610.Start;
                        chip.Stop = ym2610.Stop;
                        chip.Reset = ym2610.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.YM2610Volume;
                        chip.Clock = ((vgm)vgmVirtual).YM2610ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPNB = 1;
                        else ChipSecOPNB = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).AY8910ClockValue != 0)
                {
                    MDSound.ay8910 ay8910 = new MDSound.ay8910();
                    for (int i = 0; i < (((vgm)vgmVirtual).AY8910DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                        chip.ID = (byte)i;
                        chip.Instrument = ay8910;
                        chip.Update = ay8910.Update;
                        chip.Start = ay8910.Start;
                        chip.Stop = ay8910.Stop;
                        chip.Reset = ay8910.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.AY8910Volume;
                        chip.Clock = (((vgm)vgmVirtual).AY8910ClockValue & 0x7fffffff) / 2;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriAY10 = 1;
                        else ChipSecAY10 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)vgmVirtual).YM2413ClockValue != 0)
                {
                    MDSound.ym2413 ym2413 = new MDSound.ym2413();
                    for (int i = 0; i < (((vgm)vgmVirtual).YM2413DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2413;
                        chip.ID = (byte)i;
                        chip.Instrument = ym2413;
                        chip.Update = ym2413.Update;
                        chip.Start = ym2413.Start;
                        chip.Stop = ym2413.Stop;
                        chip.Reset = ym2413.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.YM2413Volume;
                        chip.Clock = (((vgm)vgmVirtual).YM2413ClockValue & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPLL = 1;
                        else ChipSecOPLL = 1;

                        lstChips.Add(chip);
                    }
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2203FMVolume(setting.balance.YM2203FMVolume);
                SetYM2203PSGVolume(setting.balance.YM2203PSGVolume);

                SetYM2608FMVolume(setting.balance.YM2608FMVolume);
                SetYM2608PSGVolume(setting.balance.YM2608PSGVolume);
                SetYM2608RhythmVolume(setting.balance.YM2608RhythmVolume);
                SetYM2608AdpcmVolume(setting.balance.YM2608AdpcmVolume);

                SetYM2610FMVolume(setting.balance.YM2610FMVolume);
                SetYM2610PSGVolume(setting.balance.YM2610PSGVolume);
                SetYM2610AdpcmAVolume(setting.balance.YM2610AdpcmAVolume);
                SetYM2610AdpcmBVolume(setting.balance.YM2610AdpcmBVolume);


                Paused = false;
                Stopped = false;
                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static void FF()
        {
            vgmSpeed = (vgmSpeed == 1) ? 4 : 1;
            vgmVirtual.vgmSpeed = vgmSpeed;
            vgmReal.vgmSpeed = vgmSpeed;
            nrtVirtual.vgmSpeed = vgmSpeed;
            nrtReal.vgmSpeed = vgmSpeed;
        }

        public static void Slow()
        {
            vgmSpeed = (vgmSpeed == 1) ? 0.25 : 1;
            vgmVirtual.vgmSpeed = vgmSpeed;
            vgmReal.vgmSpeed = vgmSpeed;
            nrtVirtual.vgmSpeed = vgmSpeed;
            nrtReal.vgmSpeed = vgmSpeed;
        }

        public static void ResetSlow()
        {
            vgmSpeed = 1;
            vgmVirtual.vgmSpeed = vgmSpeed;
            vgmReal.vgmSpeed = vgmSpeed;
            nrtVirtual.vgmSpeed = vgmSpeed;
            nrtReal.vgmSpeed = vgmSpeed;
        }

        public static void Pause()
        {

            try
            {
                    Paused = !Paused;
            }
            catch(Exception ex)
            {
                log.ForcedWrite(ex);
            }

        }

        public static bool isPaused
        {
            get
            {
                return Paused;
            }
        }

        public static bool isStopped
        {
            get
            {
                return Stopped;
            }
        }

        public static void StepPlay(int Step)
        {
            StepCounter = Step;
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
                //chipRegister.outMIDIData_Close();

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
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
            catch(Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }


        public static long GetCounter()
        {
            if (PlayingFileFormat == enmFileFormat.VGM)
            {
                return vgmVirtual.Counter;
            }
            else
            {
                return nrtVirtual.Counter;
            }
        }

        public static long GetTotalCounter()
        {
            if (PlayingFileFormat == enmFileFormat.VGM)
            {
                return vgmVirtual.TotalCounter;
            }
            else
            {
                return nrtVirtual.TotalCounter;
            }
        }

        public static long GetLoopCounter()
        {
            if (PlayingFileFormat == enmFileFormat.VGM)
            {
                return vgmVirtual.LoopCounter;
            }
            else
            {
                return nrtVirtual.LoopCounter;
            }
        }

        public static int[] GetChipStatus()
        {
            chips[0] = chipRegister.ChipPriOPN;
            chipRegister.ChipPriOPN = ChipPriOPN;
            chips[1] = chipRegister.ChipPriOPN2;
            chipRegister.ChipPriOPN2 = ChipPriOPN2;
            chips[2] = chipRegister.ChipPriOPNA;
            chipRegister.ChipPriOPNA = ChipPriOPNA;
            chips[3] = chipRegister.ChipPriOPNB;
            chipRegister.ChipPriOPNB = ChipPriOPNB;

            chips[4] = chipRegister.ChipPriOPM;
            chipRegister.ChipPriOPM = ChipPriOPM;
            chips[5] = chipRegister.ChipPriDCSG;
            chipRegister.ChipPriDCSG = ChipPriDCSG;
            chips[6] = chipRegister.ChipPriRF5C;
            chipRegister.ChipPriRF5C = ChipPriRF5C;
            chips[7] = chipRegister.ChipPriPWM;
            chipRegister.ChipPriPWM = ChipPriPWM;

            chips[8] = chipRegister.ChipPriOKI5;
            chipRegister.ChipPriOKI5 = ChipPriOKI5;
            chips[9] = chipRegister.ChipPriOKI9;
            chipRegister.ChipPriOKI9 = ChipPriOKI9;
            chips[10] = chipRegister.ChipPriC140;
            chipRegister.ChipPriC140 = ChipPriC140;
            chips[11] = chipRegister.ChipPriSPCM;
            chipRegister.ChipPriSPCM = ChipPriSPCM;

            chips[12] = chipRegister.ChipPriAY10;
            chipRegister.ChipPriAY10 = ChipPriAY10;
            chips[13] = chipRegister.ChipPriOPLL;
            chipRegister.ChipPriOPLL = ChipPriOPLL;


            chips[128 + 0] = chipRegister.ChipSecOPN;
            chipRegister.ChipSecOPN = ChipSecOPN;
            chips[128 + 1] = chipRegister.ChipSecOPN2;
            chipRegister.ChipSecOPN2 = ChipSecOPN2;
            chips[128 + 2] = chipRegister.ChipSecOPNA;
            chipRegister.ChipSecOPNA = ChipSecOPNA;
            chips[128 + 3] = chipRegister.ChipSecOPNB;
            chipRegister.ChipSecOPNB = ChipSecOPNB;

            chips[128 + 4] = chipRegister.ChipSecOPM;
            chipRegister.ChipSecOPM = ChipSecOPM;
            chips[128 + 5] = chipRegister.ChipSecDCSG;
            chipRegister.ChipSecDCSG = ChipSecDCSG;
            chips[128 + 6] = chipRegister.ChipSecRF5C;
            chipRegister.ChipSecRF5C = ChipSecRF5C;
            chips[128 + 7] = chipRegister.ChipSecPWM;
            chipRegister.ChipSecPWM = ChipSecPWM;

            chips[128 + 8] = chipRegister.ChipSecOKI5;
            chipRegister.ChipSecOKI5 = ChipSecOKI5;
            chips[128 + 9] = chipRegister.ChipSecOKI9;
            chipRegister.ChipSecOKI9 = ChipSecOKI9;
            chips[128 + 10] = chipRegister.ChipSecC140;
            chipRegister.ChipSecC140 = ChipSecC140;
            chips[128 + 11] = chipRegister.ChipSecSPCM;
            chipRegister.ChipSecSPCM = ChipSecSPCM;

            chips[128 + 12] = chipRegister.ChipSecAY10;
            chipRegister.ChipSecAY10 = ChipSecAY10;
            chips[128 + 13] = chipRegister.ChipSecOPLL;
            chipRegister.ChipSecOPLL = ChipSecOPLL;


            return chips;
        }

        public static int[][] GetFMRegister(int chipID)
        {
            return chipRegister.fmRegisterYM2612[chipID];
        }

        public static int[] GetYM2151Register(int chipID)
        {
            return chipRegister.fmRegisterYM2151[chipID];
        }

        public static int[] GetYM2203Register(int chipID)
        {
            return chipRegister.fmRegisterYM2203[chipID];
        }

        public static int[][] GetYM2608Register(int chipID)
        {
            return chipRegister.fmRegisterYM2608[chipID];
        }

        public static int[][] GetYM2610Register(int chipID)
        {
            return chipRegister.fmRegisterYM2610[chipID];
        }

        public static int[] GetPSGRegister(int chipID)
        {
            return chipRegister.sn76489Register[chipID];
        }

        public static MDSound.scd_pcm.pcm_chip_ GetRf5c164Register(int chipID)
        {
            return mds.ReadRf5c164Register(chipID);
        }

        public static MDSound.c140.c140_state GetC140Register(int chipID)
        {
            return mds.ReadC140Register(chipID);
        }

        public static MDSound.segapcm.segapcm_state GetSegaPCMRegister(int chipID)
        {
            return mds.ReadSegaPCMStatus(chipID);
        }

        public static int[] GetFMKeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2612[chipID];
        }

        public static int[] GetYM2151KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2151[chipID];
        }

        public static int GetYM2151PMD(int chipID)
        {
            return chipRegister.fmPMDYM2151[chipID];
        }

        public static int GetYM2151AMD(int chipID)
        {
            return chipRegister.fmAMDYM2151[chipID];
        }

        public static int[] GetYM2608KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2608[chipID];
        }

        public static int[] GetYM2610KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2610[chipID];
        }

        public static int[] GetYM2203KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2203[chipID];
        }

        public static int[][] GetFMVolume(int chipID)
        {
            return chipRegister.GetYM2612Volume(chipID);
        }

        public static int[][] GetYM2151Volume(int chipID)
        {
            return chipRegister.GetYM2151Volume(chipID);
        }

        public static int[][] GetYM2608Volume(int chipID)
        {
            return chipRegister.GetYM2608Volume(chipID);
        }

        public static int[][] GetYM2608RhythmVolume(int chipID)
        {
            return chipRegister.GetYM2608RhythmVolume(chipID);
        }

        public static int[] GetYM2608AdpcmVolume(int chipID)
        {
            return chipRegister.GetYM2608AdpcmVolume(chipID);
        }

        public static int[][] GetYM2610Volume(int chipID)
        {
            return chipRegister.GetYM2610Volume(chipID);
        }

        public static int[][] GetYM2610RhythmVolume(int chipID)
        {
            return chipRegister.GetYM2610RhythmVolume(chipID);
        }

        public static int[] GetYM2610AdpcmVolume(int chipID)
        {
            return chipRegister.GetYM2610AdpcmVolume(chipID);
        }

        public static int[] GetYM2203Volume(int chipID)
        {
            return chipRegister.GetYM2203Volume(chipID);
        }

        public static void updateVol()
        {
            chipRegister.updateVol();
        }

        public static int[] GetFMCh3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2612Ch3SlotVolume(chipID);
        }

        public static int[] GetYM2608Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2608Ch3SlotVolume(chipID);
        }

        public static int[] GetYM2610Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2610Ch3SlotVolume(chipID);
        }

        public static int[] GetYM2203Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2203Ch3SlotVolume(chipID);
        }

        public static int[][] GetPSGVolume(int chipID)
        {
            return chipRegister.GetPSGVolume(chipID);
        }

        public static int[][] GetRf5c164Volume(int chipID)
        {
            return mds.ReadRf5c164Volume(chipID);
        }

        public static void setYM2612Mask(int chipID,int ch)
        {
            //mds.setYM2612Mask(chipID,1 << ch);
            chipRegister.setMaskYM2612(chipID, ch, true);
        }

        public static void setYM2203Mask(int chipID, int ch)
        {
            //mds.setYM2203Mask(chipID, 1 << ch);
            chipRegister.setMaskYM2203(chipID, ch, true);
        }

        public static void setSN76489Mask(int chipID, int ch)
        {
            //mds.setSN76489Mask(chipID,1 << ch);
            chipRegister.setMaskSN76489(chipID, ch, true);
        }

        public static void setRF5C164Mask(int chipID, int ch)
        {
            mds.setRf5c164Mask(chipID,ch);
        }

        public static void setYM2151Mask(int chipID,int ch)
        {
            //mds.setYM2151Mask(ch);
            chipRegister.setMaskYM2151(chipID,ch, true);
        }

        public static void setYM2608Mask(int chipID, int ch)
        {
            //mds.setYM2608Mask(ch);
            chipRegister.setMaskYM2608(chipID, ch, true);
        }

        public static void setYM2610Mask(int chipID, int ch)
        {
            //mds.setYM2610Mask(ch);
            chipRegister.setMaskYM2610(chipID, ch, true);
        }

        public static void setC140Mask(int chipID, int ch)
        {
            mds.setC140Mask(chipID, 1 << ch);
        }

        public static void setSegaPCMMask(int chipID, int ch)
        {
            mds.setSegaPcmMask(chipID, 1 << ch);
        }

        public static void resetYM2612Mask(int chipID,int ch)
        {
            try
            {
                //mds.resetYM2612Mask(chipID, 1 << ch);
                chipRegister.setMaskYM2612(chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM2203Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2203Mask(chipID, 1 << ch);
                chipRegister.setMaskYM2203(chipID, ch, false);
            }
            catch { }
        }

        public static void resetSN76489Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetSN76489Mask(chipID, 1 << ch);
                chipRegister.setMaskSN76489(chipID, ch, false);
            }
            catch { }
        }

        public static void resetRF5C164Mask(int chipID,int ch)
        {
            try
            {
                mds.resetRf5c164Mask(chipID, ch);
            }
            catch { }
        }

        public static void resetYM2151Mask(int chipID,int ch)
        {
            try
            {
                //mds.resetYM2151Mask(ch);
                chipRegister.setMaskYM2151(chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM2608Mask(int chipID,int ch)
        {
            try
            {
                //mds.resetYM2608Mask(ch);
                chipRegister.setMaskYM2608(chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM2610Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM2610(chipID, ch, false);
            }
            catch { }
        }

        public static void resetC140Mask(int chipID, int ch)
        {
            mds.resetC140Mask(chipID,1<<ch);
        }

        public static void resetSegaPCMMask(int chipID, int ch)
        {
            mds.resetSegaPcmMask(chipID, 1 << ch);
        }


        public static uint GetVgmCurLoopCounter()
        {
            uint cnt = 0;

            if (PlayingFileFormat == enmFileFormat.VGM)
            {
                if (vgmVirtual != null)
                {
                    cnt = vgmVirtual.vgmCurLoop;
                }
                if (vgmReal != null)
                {
                    cnt = Math.Min(vgmReal.vgmCurLoop, cnt);
                }
            }
            else
            {
                if (nrtVirtual != null)
                {
                    cnt = nrtVirtual.vgmCurLoop;
                }
                if (nrtReal != null)
                {
                    cnt = Math.Min(nrtReal.vgmCurLoop, cnt);
                }
            }
            return cnt;
        }

        public static bool GetVGMStopped()
        {
            bool v;
            bool r;
            switch (PlayingFileFormat)
            {
                case enmFileFormat.NRTDRV:
                    v = nrtVirtual.Stopped;
                    r = nrtReal.Stopped;
                    return v && r;
                case enmFileFormat.VGM:
                    v = vgmVirtual.Stopped;
                    r = vgmReal.Stopped;
                    return v && r;
            }
            return true;
        }

        public static bool GetIsDataBlock(enmModel model)
        {
            return (model == enmModel.VirtualModel) ? ((vgm)vgmVirtual).isDataBlock : ((vgm)vgmReal).isDataBlock;
        }

        public static bool GetIsPcmRAMWrite(enmModel model)
        {
            return (model == enmModel.VirtualModel) ? ((vgm)vgmVirtual).isPcmRAMWrite : ((vgm)vgmReal).isPcmRAMWrite;
        }


        private static NScci.NSoundChip getChip(Setting.ChipType ct)// int SoundLocation, int BusID, int SoundChip)
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
                    if (info.getdSoundLocation() == ct.SoundLocation && info.getdBusID() == ct.BusID && info.getiSoundChip() == ct.SoundChip)
                    {
                        return sc;
                    }
                }
            }

            return null;
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
                catch(Exception ex)
                {
                    log.ForcedWrite(ex);
                }
            }
            else
            {
                try
                {
                    Stop();
                }
                catch { }
            }
        }

        private static void trdVgmRealFunction()
        {
            double o = sw.ElapsedTicks / swFreq;
            double step = 1 / (double)SamplingRate;

            trdStopped = false;
            try
            {
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
                    if (((vgm)vgmVirtual).isDataBlock) { continue; }

                    if (vgmFadeout)
                    {
                        if (vgmRealFadeoutVol != 1000) vgmRealFadeoutVolWait--;
                        if (vgmRealFadeoutVolWait == 0)
                        {
                            chipRegister.setFadeoutVolYM2151(0, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolYM2203(0, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolYM2608(0, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolYM2612(0, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolSN76489(0, vgmRealFadeoutVol);

                            chipRegister.setFadeoutVolYM2151(1, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolYM2203(1, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolYM2608(1, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolYM2612(1, vgmRealFadeoutVol);
                            chipRegister.setFadeoutVolSN76489(1, vgmRealFadeoutVol);

                            vgmRealFadeoutVol++;

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
                        long v;
                        if (PlayingFileFormat == enmFileFormat.VGM)
                        {
                            v = vgmReal.vgmFrameCounter - vgmVirtual.vgmFrameCounter;
                        }
                        else
                        {
                            v = nrtReal.vgmFrameCounter - nrtVirtual.vgmFrameCounter;
                        }
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

                        if (PlayingFileFormat == enmFileFormat.VGM)
                        {
                            switch (m)
                            {
                                case 0: //x1
                                    vgmReal.oneFrameProc();
                                    break;
                                case 1: //x1/2
                                    hiyorimiEven++;
                                    if (hiyorimiEven > 1)
                                    {
                                        vgmReal.oneFrameProc();
                                        hiyorimiEven = 0;
                                    }
                                    break;
                                case 2: //x2
                                    vgmReal.oneFrameProc();
                                    vgmReal.oneFrameProc();
                                    break;
                            }
                        }
                        else
                        {
                            switch (m)
                            {
                                case 0: //x1
                                    nrtReal.oneFrameProc();
                                    break;
                                case 1: //x1/2
                                    hiyorimiEven++;
                                    if (hiyorimiEven > 1)
                                    {
                                        nrtReal.oneFrameProc();
                                        hiyorimiEven = 0;
                                    }
                                    break;
                                case 2: //x2
                                    nrtReal.oneFrameProc();
                                    nrtReal.oneFrameProc();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (PlayingFileFormat == enmFileFormat.VGM)
                            vgmReal.oneFrameProc();
                        else
                            nrtReal.oneFrameProc();
                    }
                }
            }
            catch
            {
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
                if (((vgm)vgmReal).isDataBlock) { return mds.Update(buffer, offset, sampleCount, null); }

                if (StepCounter > 0)
                {
                    StepCounter -= sampleCount;
                    if (StepCounter <= 0)
                    {
                        Paused = true;
                        StepCounter = 0;
                        return mds.Update(buffer, offset, sampleCount, null);
                    }
                }


                int cnt=0;
                switch (PlayingFileFormat)
                {
                    case enmFileFormat.VGM:
                        cnt = mds.Update(buffer, offset, sampleCount, vgmVirtual.oneFrameProc);
                        break;
                    case enmFileFormat.NRTDRV:
                        cnt = mds.Update(buffer, offset, sampleCount, nrtVirtual.oneFrameProc);
                        break;
                }


                for (i = 0; i < sampleCount; i++)
                {
                    int mul = (int)(16384.0 * Math.Pow(10.0, MasterVolume / 40.0));
                    buffer[offset + i] = (short)((Limit(buffer[offset + i], 0x7fff, -0x8000) * mul) >> 14);
                }

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
                    chips[0].Volume = 0;
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
                    chips[1].Volume = 0;
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
                    chips[2].Volume = 0;
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
                    chips[3].Volume = 0;
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
                    chips[5].Volume = 0;
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
                    chips[6].Volume = 0;
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
                    chips[7].Volume = 0;
                    chips[7].Clock = vgm.defaultSEGAPCMClockValue;
                    chips[7].Option = new object[1] { (int)0 };

                    if (mds == null)
                        mds = new MDSound.MDSound(SamplingRate, samplingBuffer, chips);
                    else
                        mds.Init(SamplingRate, samplingBuffer, chips);

                    Stopped = true;

                    chipRegister.Close();

                }

                return cnt;

            }
            catch(Exception ex)
            {
                log.ForcedWrite(ex);
                fatalError = true;
                Stopped = true;
            }
            return -1;
        }

        public static int Limit(int v, int max, int min)
        {
            return v > max ? max : (v < min ? min : v);
        }

        public static long getVirtualFrameCounter()
        {
            if (PlayingFileFormat == enmFileFormat.VGM)
            {
                if (vgmVirtual == null) return -1;
                return vgmVirtual.vgmFrameCounter;
            }
            else
            {
                if (nrtVirtual == null) return -1;
                return nrtVirtual.vgmFrameCounter;
            }
        }

        public static long getRealFrameCounter()
        {
            if (PlayingFileFormat == enmFileFormat.VGM)
            {
                if (nrtReal == null) return -1;
                return nrtReal.vgmFrameCounter;
            }
            else
            {
                if (nrtReal == null) return -1;
                return nrtReal.vgmFrameCounter;
            }
        }


        public static void SetMasterVolume(int volume)
        {
            MasterVolume = volume;
        }

        public static void SetAY8910Volume(int volume)
        {
            //mds.setVolumeAY8910(volume);
        }

        public static void SetYM2151Volume(int volume)
        {
            mds.SetVolumeYM2151(volume);
        }

        public static void SetYM2203Volume(int volume)
        {
            mds.SetVolumeYM2203(volume);
        }

        public static void SetYM2203FMVolume(int volume)
        {
            mds.SetVolumeYM2203FM(volume);
        }

        public static void SetYM2203PSGVolume(int volume)
        {
            mds.SetVolumeYM2203PSG(volume);
        }

        public static void SetYM2608Volume(int volume)
        {
            mds.SetVolumeYM2608(volume);
        }

        public static void SetYM2608FMVolume(int volume)
        {
            mds.SetVolumeYM2608FM(volume);
        }

        public static void SetYM2608PSGVolume(int volume)
        {
            mds.SetVolumeYM2608PSG(volume);
        }

        public static void SetYM2608RhythmVolume(int volume)
        {
            mds.SetVolumeYM2608Rhythm(volume);
        }

        public static void SetYM2608AdpcmVolume(int volume)
        {
            mds.SetVolumeYM2608Adpcm(volume);
        }

        public static void SetYM2610Volume(int volume)
        {
            mds.SetVolumeYM2610(volume);
        }

        public static void SetYM2610FMVolume(int volume)
        {
            mds.SetVolumeYM2610FM(volume);
        }

        public static void SetYM2610PSGVolume(int volume)
        {
            mds.SetVolumeYM2610PSG(volume);
        }

        public static void SetYM2610AdpcmAVolume(int volume)
        {
            mds.SetVolumeYM2610AdpcmA(volume);
        }

        public static void SetYM2610AdpcmBVolume(int volume)
        {
            mds.SetVolumeYM2610AdpcmB(volume);
        }

        public static void SetYM2612Volume(int volume)
        {
            mds.SetVolumeYM2612(volume);
        }

        public static void SetSN76489Volume(int volume)
        {
            mds.SetVolumeSN76489(volume);
        }

        public static void SetRF5C164Volume(int volume)
        {
            mds.SetVolumeRF5C164(volume);
        }

        public static void SetPWMVolume(int volume)
        {
            mds.SetVolumePWM(volume);
        }

        public static void SetOKIM6258Volume(int volume)
        {
            mds.SetVolumeOKIM6258(volume);
        }

        public static void SetOKIM6295Volume(int volume)
        {
            mds.SetVolumeOKIM6295(volume);
        }

        public static void SetC140Volume(int volume)
        {
            mds.SetVolumeC140(volume);
        }

        public static void SetSegaPCMVolume(int volume)
        {
            mds.SetVolumeSegaPCM(volume);
        }

        public static GD3 GetGD3()
        {
            switch (PlayingFileFormat)
            {
                case enmFileFormat.NRTDRV:
                    return nrtVirtual.GD3;
                case enmFileFormat.VGM:
                    return vgmVirtual.GD3;
            }

            return null;
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
