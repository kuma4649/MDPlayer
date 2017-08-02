using System;
using System.Collections.Generic;
using NScci;
using System.Threading;
using System.Diagnostics;
using MDSound;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;

namespace MDPlayer
{
    public class Audio
    {
        public class cNscci
        {
            public cSoundInterface[] arySoundInterface;
        }

        public class cSoundInterface
        {
            public NSoundInterface nSoundInterface;
            public cSoundChip[] arySoundChip;
        }

        public class cSoundChip
        {
            public NSoundChip nSoundChip;
            public NSCCI_SOUND_CHIP_INFO info;
        }


        private static object lockObj = new object();
        private static bool _fatalError = false;
        public static bool fatalError
        {
            get
            {
                lock (lockObj)
                {
                    return _fatalError;
                }
            }

            set
            {
                lock (lockObj)
                {
                    _fatalError = value;
                }
            }
        }

        public static List<vstInfo2> getVSTInfos()
        {
            return vstPlugins;
        }

        public static bool addVSTeffect(string fileName)
        {
            VstPluginContext ctx = OpenPlugin(fileName);
            if (ctx == null) return false;

            //Stop();

            vstInfo2 vi = new vstInfo2();
            vi.vstPlugins = ctx;
            vi.fileName = fileName;

            ctx.PluginCommandStub.SetBlockSize(512);
            ctx.PluginCommandStub.SetSampleRate(44100f);
            ctx.PluginCommandStub.MainsChanged(true);
            ctx.PluginCommandStub.StartProcess();
            vi.name = ctx.PluginCommandStub.GetEffectName();
            vi.power = true;
            ctx.PluginCommandStub.GetParameterProperties(0);


            frmVST dlg = new frmVST();
            dlg.PluginCommandStub = ctx.PluginCommandStub;
            dlg.Show(vi);
            vi.vstPluginsForm = dlg;
            vi.editor = true;

            vstPlugins.Add(vi);

            return true;
        }

        public static bool delVSTeffect(string fileName)
        {
            if (fileName == "")
            {
                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    try
                    {
                        if (vstPlugins[i].vstPlugins != null)
                        {
                            vstPlugins[i].vstPluginsForm.timer1.Enabled = false;
                            vstPlugins[i].location = vstPlugins[i].vstPluginsForm.Location;
                            vstPlugins[i].vstPluginsForm.Close();
                            vstPlugins[i].vstPlugins.PluginCommandStub.EditorClose();
                            vstPlugins[i].vstPlugins.PluginCommandStub.StopProcess();
                            vstPlugins[i].vstPlugins.PluginCommandStub.MainsChanged(false);
                            vstPlugins[i].vstPlugins.Dispose();
                        }
                    }
                    catch { }
                }
                vstPlugins.Clear();
            }
            else
            {
                int ind = -1;
                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    if (vstPlugins[i].fileName == fileName)
                    {
                        ind = i;
                        break;
                    }
                }
                if (ind != -1)
                {
                    try
                    {
                        if (vstPlugins[ind].vstPlugins != null)
                        {
                            vstPlugins[ind].vstPluginsForm.timer1.Enabled = false;
                            vstPlugins[ind].location = vstPlugins[ind].vstPluginsForm.Location;
                            vstPlugins[ind].vstPluginsForm.Close();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.EditorClose();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.StopProcess();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.MainsChanged(false);
                            vstPlugins[ind].vstPlugins.Dispose();
                        }
                    }
                    catch { }
                    vstPlugins.RemoveAt(ind);
                }
            }

            return true;
        }

        private static uint SamplingRate = 44100;

        private static uint samplingBuffer = 1024;
        private static MDSound.MDSound mds = null;
        public static MDSound.MDSound mdsMIDI = null;
        private static NAudioWrap naudioWrap;
        private static WaveWriter waveWriter = null;

        private static NScci.NScci nscci;
        private static NSoundChip[] scYM2612 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scSN76489 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2151 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2608 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2203 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2610 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scAY8910 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scYM2413 = new NSoundChip[2] { null, null };
        private static NSoundChip[] scHuC6280 = new NSoundChip[2] { null, null };

        private static ChipRegister chipRegister = null;

        private static Thread trdMain = null;
        private static bool trdClosed = false;
        public static bool trdStopped = true;
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

        public static baseDriver driverVirtual = null;
        public static baseDriver driverReal = null;

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
        public static int ChipPriAY10 = 0;
        public static int ChipPriOPLL = 0;
        public static int ChipPriHuC = 0;
        public static int ChipPriC352 = 0;
        public static int ChipPriK054539 = 0;

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
        public static int ChipSecAY10 = 0;
        public static int ChipSecOPLL = 0;
        public static int ChipSecHuC = 0;
        public static int ChipSecC352 = 0;
        public static int ChipSecK054539 = 0;

        private static int MasterVolume = 0;
        private static int[] chips = new int[256];
        private static string PlayingFileName;
        private static enmFileFormat PlayingFileFormat;
        public static cNscci cnscci;
        private static System.Diagnostics.Stopwatch stwh = System.Diagnostics.Stopwatch.StartNew();
        public static int ProcTimePer1Frame = 0;

        public static short masterVisVolume = 0;
        public static short ym2151VisVolume = 0;
        public static short ym2203VisVolume = 0;
        public static short ym2203FMVisVolume = 0;
        public static short ym2203SSGVisVolume = 0;
        public static short ym2413VisVolume = 0;
        public static short ym2608VisVolume = 0;
        public static short ym2608FMVisVolume = 0;
        public static short ym2608SSGVisVolume = 0;
        public static short ym2608RtmVisVolume = 0;
        public static short ym2608APCMVisVolume = 0;
        public static short ym2610VisVolume = 0;
        public static short ym2610FMVisVolume = 0;
        public static short ym2610SSGVisVolume = 0;
        public static short ym2610APCMAVisVolume = 0;
        public static short ym2610APCMBVisVolume = 0;
        public static short ym2612VisVolume = 0;
        public static short ay8910VisVolume = 0;
        public static short sn76489VisVolume = 0;
        public static short huc6280VisVolume = 0;
        public static short rf5c164VisVolume = 0;
        public static short pwmVisVolume = 0;
        public static short okim6258VisVolume = 0;
        public static short okim6295VisVolume = 0;
        public static short c140VisVolume = 0;
        public static short segaPCMVisVolume = 0;
        public static short c352VisVolume = 0;
        public static short k054539VisVolume = 0;

        private static List<vstInfo2> vstPlugins = new List<vstInfo2>();

        public static PlayList.music getMusic(string file, byte[] buf, string zipFile = null)
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
                GD3 gd3 = (new NRTDRV()).getGD3Info(buf, index);
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
            else if (file.ToLower().LastIndexOf(".xgm") != -1)
            {
                music.format = enmFileFormat.XGM;
                GD3 gd3 = new xgm().getGD3Info(buf, 0);
                music.title = gd3.TrackName;
                music.titleJ = gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

                if (music.title == "" && music.titleJ == "" && music.game == "" && music.gameJ == "" && music.composer == "" && music.composerJ == "")
                {
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }
            }
            else if (file.ToLower().LastIndexOf(".s98") != -1)
            {
                music.format = enmFileFormat.S98;
                GD3 gd3 = new S98().getGD3Info(buf, 0);
                if (gd3 != null)
                {
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
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }

            }
            else
            {
                if (buf.Length < 0x40) return music;
                if (common.getLE32(buf, 0x00) != vgm.FCC_VGM) return music;

                music.format = enmFileFormat.VGM;
                uint version = common.getLE32(buf, 0x08);
                string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint vgmGd3 = common.getLE32(buf, 0x14);
                GD3 gd3 = new GD3();
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = common.getLE32(buf, vgmGd3 + 0x14);
                    if (vgmGd3Id != vgm.FCC_GD3) return music;
                    gd3 = (new vgm()).getGD3Info(buf, vgmGd3);
                }

                uint TotalCounter = common.getLE32(buf, 0x18);
                uint vgmLoopOffset = common.getLE32(buf, 0x1c);
                uint LoopCounter = common.getLE32(buf, 0x20);

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
            catch (Exception e)
            {
                log.ForcedWrite(e);
            }
            return "";
        }

        public static void Init(Setting setting)
        {
            log.ForcedWrite("Audio:Init:Begin");

            log.ForcedWrite("Audio:Init:STEP 01");

            naudioWrap = new NAudioWrap((int)SamplingRate, trdVgmVirtualFunction);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;

            log.ForcedWrite("Audio:Init:STEP 02");

            Audio.setting = setting;// Copy();

            waveWriter = new WaveWriter(setting);

            log.ForcedWrite("Audio:Init:STEP 03");

            if (mds == null)
                mds = new MDSound.MDSound(SamplingRate, samplingBuffer, null);
            else
                mds.Init(SamplingRate, samplingBuffer, null);

            List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();
            MDSound.MDSound.Chip chip;

            ym2612 ym2612 = new ym2612();
            chip = new MDSound.MDSound.Chip();
            chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
            chip.ID = (byte)0;
            chip.Instrument = ym2612;
            chip.Update = ym2612.Update;
            chip.Start = ym2612.Start;
            chip.Stop = ym2612.Stop;
            chip.Reset = ym2612.Reset;
            chip.SamplingRate = SamplingRate;
            chip.Volume = setting.balance.YM2612Volume;
            chip.Clock = 7670454;
            chip.Option = null;
            ChipPriOPN2 = 1;
            lstChips.Add(chip);

            sn76489 sn76489 = new sn76489();
            chip = new MDSound.MDSound.Chip();
            chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
            chip.ID = (byte)0;
            chip.Instrument = sn76489;
            chip.Update = sn76489.Update;
            chip.Start = sn76489.Start;
            chip.Stop = sn76489.Stop;
            chip.Reset = sn76489.Reset;
            chip.SamplingRate = SamplingRate;
            chip.Volume = setting.balance.SN76489Volume;
            chip.Clock = 3579545;
            chip.Option = null;
            ChipPriDCSG = 1;
            lstChips.Add(chip);

            if (mdsMIDI == null)
                mdsMIDI = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
            else
                mdsMIDI.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

            log.ForcedWrite("Audio:Init:STEP 04");

            if (cnscci == null)
            {
                nscci = new NScci.NScci();
                getScciInstances();
            }

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
            scHuC6280[0] = getChip(Audio.setting.HuC6280Type);
            if (scHuC6280[0] != null) scHuC6280[0].init();

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
                , scYM2612, scSN76489, scYM2608, scYM2151, scYM2203, scYM2610, scAY8910, scYM2413, scHuC6280
                , new Setting.ChipType[] { setting.YM2612Type, setting.YM2612SType }
                , new Setting.ChipType[] { setting.SN76489Type, setting.SN76489SType }
                , new Setting.ChipType[] { setting.YM2608Type, setting.YM2608SType }
                , new Setting.ChipType[] { setting.YM2151Type, setting.YM2151SType }
                , new Setting.ChipType[] { setting.YM2203Type, setting.YM2203SType }
                , new Setting.ChipType[] { setting.YM2610Type, setting.YM2610SType }
                , new Setting.ChipType[] { setting.AY8910Type, setting.AY8910SType }
                , new Setting.ChipType[] { setting.YM2413Type, setting.YM2413SType }
                , new Setting.ChipType[] { setting.HuC6280Type, setting.HuC6280SType }
                );
            chipRegister.initChipRegister();

            log.ForcedWrite("Audio:Init:STEP 05");

            Paused = false;
            Stopped = true;
            fatalError = false;
            oneTimeReset = false;

            while (vstPlugins.Count > 0)
            {
                if (vstPlugins[0] != null)
                {
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.EditorClose();
                    vstPlugins[0].vstPluginsForm.timer1.Enabled = false;
                    vstPlugins[0].location = vstPlugins[0].vstPluginsForm.Location;
                    vstPlugins[0].vstPluginsForm.Close();
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.StopProcess();
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.MainsChanged(false);
                    vstPlugins[0].vstPlugins.Dispose();
                }

                vstPlugins.RemoveAt(0);

            }

            if (setting.vst != null && setting.vst.VSTInfo != null)
            {
                for (int i = 0; i < setting.vst.VSTInfo.Length; i++)
                {
                    if (setting.vst.VSTInfo[i] == null) continue;
                    VstPluginContext ctx = OpenPlugin(setting.vst.VSTInfo[i].fileName);
                    if (ctx == null) continue;

                    vstInfo2 vi = new vstInfo2();
                    vi.vstPlugins = ctx;
                    vi.fileName = setting.vst.VSTInfo[i].fileName;

                    ctx.PluginCommandStub.SetBlockSize(512);
                    ctx.PluginCommandStub.SetSampleRate(44100f);
                    ctx.PluginCommandStub.MainsChanged(true);
                    ctx.PluginCommandStub.StartProcess();
                    vi.name = ctx.PluginCommandStub.GetEffectName();
                    vi.power = setting.vst.VSTInfo[i].power; 
                    vi.editor = setting.vst.VSTInfo[i].editor;
                    vi.location = setting.vst.VSTInfo[i].location;
                    vi.param = setting.vst.VSTInfo[i].param;

                    if (vi.editor)
                    {
                        frmVST dlg = new frmVST();
                        dlg.PluginCommandStub = ctx.PluginCommandStub;
                        dlg.Show(vi);
                        vi.vstPluginsForm = dlg;
                    }

                    if (vi.param != null)
                    {
                        for (int p = 0; p < vi.param.Length; p++)
                        {
                            ctx.PluginCommandStub.SetParameter(p, vi.param[p]);
                        }
                    }

                    vstPlugins.Add(vi);
                }
            }

            log.ForcedWrite("Audio:Init:STEP 06");

            naudioWrap.Start(Audio.setting);

            log.ForcedWrite("Audio:Init:Complete");

        }

        public static void getScciInstances()
        {
            cnscci = new cNscci();
            int ifc = nscci.getInterfaceCount();
            cnscci.arySoundInterface = new cSoundInterface[ifc];

            for (int i = 0; i < ifc; i++)
            {
                cnscci.arySoundInterface[i] = new cSoundInterface();
                NSoundInterface sif = nscci.getInterface(i);
                cnscci.arySoundInterface[i].nSoundInterface = sif;

                int scc = sif.getSoundChipCount();
                cnscci.arySoundInterface[i].arySoundChip = new cSoundChip[scc];

                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    cnscci.arySoundInterface[i].arySoundChip[j] = new cSoundChip();
                    cnscci.arySoundInterface[i].arySoundChip[j].nSoundChip = sc;

                    NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();

                    cnscci.arySoundInterface[i].arySoundChip[j].info = info;

                }
            }

        }

        public static List<Setting.ChipType> getChipList(enmScciChipType scciChipType)
        {
            List<Setting.ChipType> ret = new List<Setting.ChipType>();

            for (int i = 0; i < cnscci.arySoundInterface.Length; i++)
            {
                for (int j = 0; j < cnscci.arySoundInterface[i].arySoundChip.Length; j++)
                {
                    NSoundChip sc = cnscci.arySoundInterface[i].arySoundChip[j].nSoundChip;
                    int t = sc.getSoundChipType();
                    if (t == (int)scciChipType)
                    {
                        Setting.ChipType ct = new Setting.ChipType();
                        ct.SoundLocation = 0;
                        ct.BusID = i;
                        ct.SoundChip = j;
                        ct.ChipName = sc.getSoundChipInfo().getcSoundChipName();
                        ret.Add(ct);
                    }
                }
            }

            return ret;
        }

        private static NSoundChip getChip(Setting.ChipType ct)
        {
            for (int i = 0; i < cnscci.arySoundInterface.Length; i++)
            {
                for (int j = 0; j < cnscci.arySoundInterface[i].arySoundChip.Length; j++)
                {
                    NSoundChip sc = cnscci.arySoundInterface[i].arySoundChip[j].nSoundChip;
                    NSCCI_SOUND_CHIP_INFO info = cnscci.arySoundInterface[i].arySoundChip[j].info;
                    if (0 == ct.SoundLocation
                        && i == ct.BusID
                        && j == ct.SoundChip)
                    {
                        return sc;
                    }
                }
            }

            return null;
        }

        public static int getLatency()
        {
            if (setting.outputDevice.DeviceType != 3)
            {
                return (int)SamplingRate * setting.outputDevice.Latency / 1000;
            }
            return naudioWrap.getAsioLatency();
        }

        public static void SetVGMBuffer(enmFileFormat format, byte[] srcBuf, string playingFileName)
        {
            Stop();
            PlayingFileFormat = format;
            vgmBuf = srcBuf;
            PlayingFileName = playingFileName;
            chipRegister.SetFileName(playingFileName);
        }


        public static bool Play(Setting setting)
        {

            waveWriter.Open(PlayingFileName);

            if (PlayingFileFormat == enmFileFormat.NRTDRV)
            {
                driverVirtual = new NRTDRV();
                driverReal = new NRTDRV();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                return nrdPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.XGM)
            {
                driverVirtual = new xgm();
                driverReal = new xgm();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                return xgmPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.S98)
            {
                driverVirtual = new S98();
                driverReal = new S98();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                return s98Play(setting);
            }

            driverVirtual = new vgm();
            driverReal = new vgm();
            driverVirtual.setting = setting;
            driverReal.setting = setting;
            ((vgm)driverVirtual).dacControl.chipRegister = chipRegister;
            ((vgm)driverVirtual).dacControl.model = enmModel.VirtualModel;
            ((vgm)driverReal).dacControl.chipRegister = chipRegister;
            ((vgm)driverReal).dacControl.model = enmModel.RealModel;

            return vgmPlay(setting);
        }

        public static bool nrdPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

                int r = ((NRTDRV)driverVirtual).checkUseChip(vgmBuf);

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
                ChipPriHuC = 0;
                ChipPriC352 = 0;
                ChipPriK054539 = 0;

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
                ChipSecHuC = 0;
                ChipSecC352 = 0;
                ChipSecK054539 = 0;

                MasterVolume = setting.balance.MasterVolume;

                MDSound.ym2151 ym2151 = new MDSound.ym2151();
                for (int i = 0; i < 2; i++)
                {
                    if ((i == 0 && (r & 0x3) != 0) || (i == 1 && (r & 0x2) != 0))
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
                }

                if ((r & 0x4) != 0)
                {
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
                    chip.Clock = 2000000 / 2;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x1;
                    ChipPriAY10 = 1;

                    lstChips.Add(chip);
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2151Volume(setting.balance.YM2151Volume);
                SetAY8910Volume(setting.balance.AY8910Volume);

                driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, enmUseChip.YM2151 | enmUseChip.AY8910, 0);
                driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, enmUseChip.YM2151 | enmUseChip.AY8910, 0);
                ((NRTDRV)driverVirtual).Call(0);//
                ((NRTDRV)driverVirtual).Call(1);//MPLAY
                ((NRTDRV)driverReal).Call(0);//
                ((NRTDRV)driverReal).Call(1);//MPLAY

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

        public static bool xgmPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

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
                ChipPriHuC = 0;
                ChipPriC352 = 0;
                ChipPriK054539 = 0;

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
                ChipSecHuC = 0;
                ChipSecC352 = 0;
                ChipSecK054539 = 0;

                MasterVolume = setting.balance.MasterVolume;

                ym2612 ym2612 = new ym2612();
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                    chip.ID = (byte)0;
                    chip.Instrument = ym2612;
                    chip.Update = ym2612.Update;
                    chip.Start = ym2612.Start;
                    chip.Stop = ym2612.Stop;
                    chip.Reset = ym2612.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = setting.balance.YM2612Volume;
                    chip.Clock = 7670454;
                    chip.Option = null;
                    ChipPriOPN2 = 1;
                    lstChips.Add(chip);

                    sn76489 sn76489 = new sn76489();
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                    chip.ID = (byte)0;
                    chip.Instrument = sn76489;
                    chip.Update = sn76489.Update;
                    chip.Start = sn76489.Start;
                    chip.Stop = sn76489.Stop;
                    chip.Reset = sn76489.Reset;
                    chip.SamplingRate = SamplingRate;
                    chip.Volume = setting.balance.SN76489Volume;
                    chip.Clock = 3579545;
                    chip.Option = null;
                    ChipPriDCSG = 1;
                    lstChips.Add(chip);

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2612Volume(setting.balance.YM2612Volume);
                SetSN76489Volume(setting.balance.SN76489Volume);

                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, enmUseChip.YM2612 | enmUseChip.SN76489, 0)) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, enmUseChip.YM2612 | enmUseChip.SN76489, 0)) return false;

                //Play

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

        public static bool s98Play(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

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
                ChipPriHuC = 0;
                ChipPriC352 = 0;
                ChipPriK054539 = 0;

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
                ChipSecHuC = 0;
                ChipSecC352 = 0;
                ChipSecK054539 = 0;

                MasterVolume = setting.balance.MasterVolume;

                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, enmUseChip.YM2203, 0)) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, enmUseChip.YM2203, 0)) return false;

                List<S98.S98DevInfo> s98DInfo = ((S98)driverVirtual).s98Info.DeviceInfos;

                ym2203 ym2203 = null;
                ym2612 ym2612 = null;
                ym2608 ym2608 = null;
                ym2151 ym2151 = null;
                foreach (S98.S98DevInfo dInfo in s98DInfo)
                {
                    switch (dInfo.DeviceType)
                    {
                        case 2:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2203 == null)
                            {
                                ym2203 = new ym2203();
                                chip.ID = 0;
                                ChipPriOPN = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                ChipSecOPN = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2203;
                            chip.Instrument = ym2203;
                            chip.Update = ym2203.Update;
                            chip.Start = ym2203.Start;
                            chip.Stop = ym2203.Stop;
                            chip.Reset = ym2203.Reset;
                            chip.SamplingRate = SamplingRate;
                            chip.Volume = setting.balance.YM2203Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            lstChips.Add(chip);

                            break;
                        case 3:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2612 == null)
                            {
                                ym2612 = new ym2612();
                                chip.ID = 0;
                                ChipPriOPN = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                ChipSecOPN = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                            chip.Instrument = ym2612;
                            chip.Update = ym2612.Update;
                            chip.Start = ym2612.Start;
                            chip.Stop = ym2612.Stop;
                            chip.Reset = ym2612.Reset;
                            chip.SamplingRate = SamplingRate;
                            chip.Volume = setting.balance.YM2612Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            lstChips.Add(chip);

                            break;
                        case 4:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2608 == null)
                            {
                                ym2608 = new ym2608();
                                chip.ID = 0;
                                ChipPriOPNA=1;
                            }
                            else
                            {
                                chip.ID = 1;
                                ChipSecOPNA = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                            chip.Instrument = ym2608;
                            chip.Update = ym2608.Update;
                            chip.Start = ym2608.Start;
                            chip.Stop = ym2608.Stop;
                            chip.Reset = ym2608.Reset;
                            chip.SamplingRate = SamplingRate;
                            chip.Volume = setting.balance.YM2608Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);

                            break;
                        case 5:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2151 == null)
                            {
                                ym2151 = new ym2151();
                                chip.ID = 0;
                                ChipPriOPM = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                ChipSecOPM = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                            chip.Instrument = ym2151;
                            chip.Update = ym2151.Update;
                            chip.Start = ym2151.Start;
                            chip.Stop = ym2151.Stop;
                            chip.Reset = ym2151.Reset;
                            chip.SamplingRate = SamplingRate;
                            chip.Volume = setting.balance.YM2151Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);

                            break;
                    }
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound(SamplingRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init(SamplingRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2203Volume(setting.balance.YM2203Volume);
                SetYM2203FMVolume(setting.balance.YM2203FMVolume);
                SetYM2203PSGVolume(setting.balance.YM2203PSGVolume);

                SetYM2612Volume(setting.balance.YM2612Volume);

                SetYM2608Volume(setting.balance.YM2608Volume);
                SetYM2608FMVolume(setting.balance.YM2608FMVolume);
                SetYM2608PSGVolume(setting.balance.YM2608PSGVolume);
                SetYM2608RhythmVolume(setting.balance.YM2608RhythmVolume);
                SetYM2608AdpcmVolume(setting.balance.YM2608AdpcmVolume);
                chipRegister.setYM2608Register(0, 0, 0x29, 0x82, enmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x29, 0x82, enmModel.RealModel);
                chipRegister.setYM2608Register(1, 0, 0x29, 0x82, enmModel.VirtualModel);
                chipRegister.setYM2608Register(1, 0, 0x29, 0x82, enmModel.RealModel);

                SetYM2151Volume(setting.balance.YM2151Volume);

                //Play

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

        public static bool vgmPlay(Setting setting)
        {

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

                if (!driverVirtual.init(vgmBuf
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

                if (!driverReal.init(vgmBuf
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
                ChipPriHuC = 0;
                ChipPriC352 = 0;
                ChipPriK054539 = 0;

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
                ChipSecHuC = 0;
                ChipSecC352 = 0;
                ChipSecK054539 = 0;

                MasterVolume = setting.balance.MasterVolume;

                if (((vgm)driverVirtual).SN76489ClockValue != 0)
                {
                    MDSound.sn76489 sn76489 = new MDSound.sn76489();

                    for (int i = 0; i < (((vgm)driverVirtual).SN76489DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).SN76489ClockValue;
                        chip.Option = null;
                        if (i == 0) ChipPriDCSG = 1;
                        else ChipSecDCSG = 1;

                        hiyorimiDeviceFlag |= (setting.SN76489Type.UseScci) ? 0x1 : 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YM2612ClockValue != 0)
                {
                    MDSound.ym2612 ym2612 = new MDSound.ym2612();

                    for (int i = 0; i < (((vgm)driverVirtual).YM2612DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).YM2612ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci) ? 0x1 : 0x2;
                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci && setting.YM2612Type.OnlyPCMEmulation) ? 0x2 : 0x0;

                        if (i == 0) ChipPriOPN2 = 1;
                        else ChipSecOPN2 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).RF5C164ClockValue != 0)
                {
                    MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();

                    for (int i = 0; i < (((vgm)driverVirtual).RF5C164DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).RF5C164ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriRF5C = 1;
                        else ChipSecRF5C = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).PWMClockValue != 0)
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
                    chip.Clock = ((vgm)driverVirtual).PWMClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriPWM = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)driverVirtual).C140ClockValue != 0)
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
                    chip.Clock = ((vgm)driverVirtual).C140ClockValue;
                    chip.Option = new object[1] { ((vgm)driverVirtual).C140Type };

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriC140 = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)driverVirtual).OKIM6258ClockValue != 0)
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
                    chip.Clock = ((vgm)driverVirtual).OKIM6258ClockValue;
                    chip.Option = new object[1] { (int)((vgm)driverVirtual).OKIM6258Type };
                    //chip.Option = new object[1] { 6 };
                    okim6258.okim6258_set_srchg_cb(0, ChangeChipSampleRate, chip);

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriOKI5 = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)driverVirtual).OKIM6295ClockValue != 0)
                {
                    MDSound.okim6295 okim6295 = new MDSound.okim6295();
                    for (int i = 0; i < (((vgm)driverVirtual).OKIM6295DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).OKIM6295ClockValue;
                        chip.Option = null;
                        okim6295.okim6295_set_srchg_cb(0, ChangeChipSampleRate, chip);

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOKI9 = 1;
                        else ChipSecOKI9 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).SEGAPCMClockValue != 0)
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
                    chip.Clock = ((vgm)driverVirtual).SEGAPCMClockValue;
                    chip.Option = new object[1] { ((vgm)driverVirtual).SEGAPCMInterface };

                    hiyorimiDeviceFlag |= 0x2;

                    ChipPriSPCM = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)driverVirtual).YM2608ClockValue != 0)
                {
                    MDSound.ym2608 ym2608 = new MDSound.ym2608();
                    for (int i = 0; i < (((vgm)driverVirtual).YM2608DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).YM2608ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPNA = 1;
                        else ChipSecOPNA = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YM2151ClockValue != 0)
                {
                    MDSound.ym2151 ym2151 = new MDSound.ym2151();
                    for (int i = 0; i < (((vgm)driverVirtual).YM2151DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).YM2151ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPM = 1;
                        else ChipSecOPM = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YM2203ClockValue != 0)
                {
                    MDSound.ym2203 ym2203 = new MDSound.ym2203();
                    for (int i = 0; i < (((vgm)driverVirtual).YM2203DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).YM2203ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPN = 1;
                        else ChipSecOPN = 1;

                        lstChips.Add(chip);

                    }
                }

                if (((vgm)driverVirtual).YM2610ClockValue != 0)
                {
                    MDSound.ym2610 ym2610 = new MDSound.ym2610();
                    for (int i = 0; i < (((vgm)driverVirtual).YM2610DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = ((vgm)driverVirtual).YM2610ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPNB = 1;
                        else ChipSecOPNB = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).AY8910ClockValue != 0)
                {
                    MDSound.ay8910 ay8910 = new MDSound.ay8910();
                    for (int i = 0; i < (((vgm)driverVirtual).AY8910DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = (((vgm)driverVirtual).AY8910ClockValue & 0x7fffffff) / 2;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriAY10 = 1;
                        else ChipSecAY10 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YM2413ClockValue != 0)
                {
                    MDSound.ym2413 ym2413 = new MDSound.ym2413();
                    for (int i = 0; i < (((vgm)driverVirtual).YM2413DualChipFlag ? 2 : 1); i++)
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
                        chip.Clock = (((vgm)driverVirtual).YM2413ClockValue & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriOPLL = 1;
                        else ChipSecOPLL = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).HuC6280ClockValue != 0)
                {
                    MDSound.Ootake_PSG huc6280 = new MDSound.Ootake_PSG();
                    for (int i = 0; i < (((vgm)driverVirtual).HuC6280DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.HuC6280;
                        chip.ID = (byte)i;
                        chip.Instrument = huc6280;
                        chip.Update = huc6280.Update;
                        chip.Start = huc6280.Start;
                        chip.Stop = huc6280.Stop;
                        chip.Reset = huc6280.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.HuC6280Volume;
                        chip.Clock = (((vgm)driverVirtual).HuC6280ClockValue & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriHuC = 1;
                        else ChipSecHuC = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).C352ClockValue != 0)
                {
                    MDSound.c352 c352 = new c352();
                    for (int i = 0; i < (((vgm)driverVirtual).C352DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.C352;
                        chip.ID = (byte)i;
                        chip.Instrument = c352;
                        chip.Update = c352.Update;
                        chip.Start = c352.Start;
                        chip.Stop = c352.Stop;
                        chip.Reset = c352.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.C352Volume;
                        chip.Clock = (((vgm)driverVirtual).C352ClockValue & 0x7fffffff);
                        chip.Option = new object[1] { (((vgm)driverVirtual).C352ClockDivider) };
                        c352.c352_set_options((byte)(((vgm)driverVirtual).C352ClockValue >> 31));
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) ChipPriC352 = 1;
                        else ChipSecC352 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).K054539ClockValue != 0)
                {
                    MDSound.K054539 k054539 = new MDSound.K054539();

                    for (int i = 0; i < (((vgm)driverVirtual).K054539DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K054539;
                        chip.ID = (byte)i;
                        chip.Instrument = k054539;
                        chip.Update = k054539.Update;
                        chip.Start = k054539.Start;
                        chip.Stop = k054539.Stop;
                        chip.Reset = k054539.Reset;
                        chip.SamplingRate = SamplingRate;
                        chip.Volume = setting.balance.K054539Volume;
                        chip.Clock = ((vgm)driverVirtual).K054539ClockValue;
                        chip.Option = null;
                        if (i == 0) ChipPriK054539 = 1;
                        else ChipSecK054539 = 1;

                        hiyorimiDeviceFlag |= 0x2;

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

        public static void ChangeChipSampleRate(MDSound.MDSound.Chip chip, int NewSmplRate)
        {
            MDSound.MDSound.Chip CAA = chip; 

            if (CAA.SamplingRate == NewSmplRate)
                return;

            // quick and dirty hack to make sample rate changes work
            CAA.SamplingRate = (uint)NewSmplRate;
            if (CAA.SamplingRate < 44100)//SampleRate)
                CAA.Resampler = 0x01;
            else if (CAA.SamplingRate == 44100)//SampleRate)
                CAA.Resampler = 0x02;
            else if (CAA.SamplingRate > 44100)//SampleRate)
                CAA.Resampler = 0x03;
            CAA.SmpP = 1;
            CAA.SmpNext -= CAA.SmpLast;
            CAA.SmpLast = 0x00;

            return;
        }

        public static void FF()
        {
            vgmSpeed = (vgmSpeed == 1) ? 4 : 1;
            driverVirtual.vgmSpeed = vgmSpeed;
            driverReal.vgmSpeed = vgmSpeed;
        }

        public static void Slow()
        {
            vgmSpeed = (vgmSpeed == 1) ? 0.25 : 1;
            driverVirtual.vgmSpeed = vgmSpeed;
            driverReal.vgmSpeed = vgmSpeed;
        }

        public static void ResetSlow()
        {
            vgmSpeed = 1;
            driverVirtual.vgmSpeed = vgmSpeed;
            driverReal.vgmSpeed = vgmSpeed;
        }

        public static void Pause()
        {

            try
            {
                Paused = !Paused;
            }
            catch (Exception ex)
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

                waveWriter.Close();
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

                setting.vst.VSTInfo = null;
                List<vstInfo> vstlst = new List<vstInfo>();

                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    try
                    {
                        vstPlugins[i].vstPluginsForm.timer1.Enabled = false;
                        vstPlugins[i].location = vstPlugins[i].vstPluginsForm.Location;
                        vstPlugins[i].vstPluginsForm.Close();
                    }
                    catch { }

                    try
                    {
                        if (vstPlugins[i].vstPlugins != null)
                        {
                            vstPlugins[i].vstPlugins.PluginCommandStub.EditorClose();
                            vstPlugins[i].vstPlugins.PluginCommandStub.StopProcess();
                            vstPlugins[i].vstPlugins.PluginCommandStub.MainsChanged(false);
                            int pc=vstPlugins[i].vstPlugins.PluginInfo.ParameterCount;
                            List<float> plst = new List<float>();
                            for (int p = 0; p < pc; p++)
                            {
                                float v = vstPlugins[i].vstPlugins.PluginCommandStub.GetParameter(p);
                                plst.Add(v);
                            }
                            vstPlugins[i].param = plst.ToArray();
                            vstPlugins[i].vstPlugins.Dispose();
                        }
                    }
                    catch { }

                    vstInfo vi = new vstInfo();
                    vi.editor = vstPlugins[i].editor;
                    vi.fileName = vstPlugins[i].fileName;
                    vi.key = vstPlugins[i].key;
                    vi.name = vstPlugins[i].name;
                    vi.power = vstPlugins[i].power;
                    vi.location = vstPlugins[i].location;
                    vi.param = vstPlugins[i].param;

                    vstlst.Add(vi);
                }
                setting.vst.VSTInfo = vstlst.ToArray();

                //nscci.Dispose();
                //nscci = null;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public static long GetCounter()
        {
            if (driverVirtual == null) return -1;

            return driverVirtual.Counter;
        }

        public static long GetTotalCounter()
        {
            if (driverVirtual == null) return -1;

            return driverVirtual.TotalCounter;
        }

        public static long GetLoopCounter()
        {
            if (driverVirtual == null) return -1;

            return driverVirtual.LoopCounter;
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
            chips[14] = chipRegister.ChipPriHuC;
            chipRegister.ChipPriHuC = ChipPriHuC;
            chips[15] = chipRegister.ChipPriC352;
            chipRegister.ChipPriC352 = ChipPriC352;
            chips[16] = chipRegister.ChipPriK054539;
            chipRegister.ChipPriK054539 = ChipPriK054539;


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
            chips[128 + 14] = chipRegister.ChipSecHuC;
            chipRegister.ChipSecHuC = ChipSecHuC;
            chips[128+15] = chipRegister.ChipSecC352;
            chipRegister.ChipSecC352 = ChipSecC352;
            chips[128+16] = chipRegister.ChipSecK054539;
            chipRegister.ChipSecK054539 = ChipSecK054539;


            return chips;
        }

        public static int[][] GetFMRegister(int chipID)
        {
            return chipRegister.fmRegisterYM2612[chipID];
        }

        public static int[][] GetYM2612MIDIRegister()
        {
            return mdsMIDI.ReadYM2612Register(0);
        }

        public static int[] GetYM2151Register(int chipID)
        {
            return chipRegister.fmRegisterYM2151[chipID];
        }

        public static int[] GetYM2203Register(int chipID)
        {
            return chipRegister.fmRegisterYM2203[chipID];
        }

        public static int[] GetYM2413Register(int chipID)
        {
            return chipRegister.fmRegisterYM2413[chipID];
        }

        public static int getYM2413RyhthmKeyON(int chipID)
        {
            return chipRegister.getYM2413RyhthmKeyON(chipID);
        }

        public static void resetYM2413RyhthmKeyON(int chipID)
        {
            chipRegister.resetYM2413RyhthmKeyON(chipID);
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

        public static int[] GetAY8910Register(int chipID)
        {
            return chipRegister.psgRegisterAY8910[chipID];
        }

        public static Ootake_PSG.huc6280_state GetHuC6280Register(int chipID)
        {
            return mds.ReadHuC6280Status(chipID);
        }

        public static scd_pcm.pcm_chip_ GetRf5c164Register(int chipID)
        {
            return mds.ReadRf5c164Register(chipID);
        }

        public static c140.c140_state GetC140Register(int chipID)
        {
            return mds.ReadC140Register(chipID);
        }

        public static segapcm.segapcm_state GetSegaPCMRegister(int chipID)
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

        public static void setYM2203Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM2203(chipID, ch, true);
        }

        public static void setYM2413Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM2413(chipID, ch, true);
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

        public static void setYM2612Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM2612(chipID, ch, true);
        }

        public static void setC140Mask(int chipID, int ch)
        {
            mds.setC140Mask(chipID, 1 << ch);
        }

        public static void setSegaPCMMask(int chipID, int ch)
        {
            mds.setSegaPcmMask(chipID, 1 << ch);
        }

        public static void setAY8910Mask(int chipID, int ch)
        {
            mds.setAY8910Mask(chipID, 1<<ch);
        }

        public static void setHuC6280Mask(int chipID, int ch)
        {
            mds.setHuC6280Mask(chipID, 1 << ch);
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
                chipRegister.setMaskYM2203(chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM2413Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM2413(chipID, ch, false);
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

        public static void resetAY8910Mask(int chipID, int ch)
        {
            mds.resetAY8910Mask(chipID, 1<<ch);
        }

        public static void resetHuC6280Mask(int chipID, int ch)
        {
            mds.resetHuC6280Mask(chipID, 1 << ch);
        }

        public static uint GetVgmCurLoopCounter()
        {
            uint cnt = 0;

            if (driverVirtual != null)
            {
                cnt = driverVirtual.vgmCurLoop;
            }
            if (driverReal != null)
            {
                cnt = Math.Min(driverReal.vgmCurLoop, cnt);
            }

            return cnt;
        }

        public static bool GetVGMStopped()
        {
            bool v;
            bool r;

            v = driverVirtual.Stopped;
            r = driverReal.Stopped;
            return v && r;
        }

        public static bool GetIsDataBlock(enmModel model)
        {
            if (driverVirtual == null) return false;
            if (!(driverVirtual is vgm)) return false;

            return (model == enmModel.VirtualModel) ? ((vgm)driverVirtual).isDataBlock : ((vgm)driverReal).isDataBlock;
        }

        public static bool GetIsPcmRAMWrite(enmModel model)
        {
            if (driverVirtual == null) return false;
            if (!(driverVirtual is vgm)) return false;

            return (model == enmModel.VirtualModel) ? ((vgm)driverVirtual).isPcmRAMWrite : ((vgm)driverReal).isPcmRAMWrite;
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
                    if ((driverVirtual is vgm) && ((vgm)driverVirtual).isDataBlock) { continue; }

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
                        v = driverReal.vgmFrameCounter - driverVirtual.vgmFrameCounter;
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
                                driverReal.oneFrameProc();
                                break;
                            case 1: //x1/2
                                hiyorimiEven++;
                                if (hiyorimiEven > 1)
                                {
                                    driverReal.oneFrameProc();
                                    hiyorimiEven = 0;
                                }
                                break;
                            case 2: //x2
                                driverReal.oneFrameProc();
                                driverReal.oneFrameProc();
                                break;
                        }
                    }
                    else
                    {
                        driverReal.oneFrameProc();
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
            int cnt1 = trdVgmVirtualMainFunction(buffer, offset, sampleCount);

            if (setting.midiKbd.UseMIDIKeyboard)
            {
                short[] buf = new short[sampleCount];
                int cnt2 = mdsMIDI.Update(buf, 0, sampleCount, null);
                for (int i = 0; i < sampleCount; i++)
                {
                    buffer[i + offset] += buf[i];
                }
            }

            if (vstPlugins.Count > 0) VST_Update(buffer, offset, sampleCount);

            return cnt1;
        }

        private static void VST_Update(short[] buffer, int offset, int sampleCount)
        {
            try
            {
                int blockSize = sampleCount / 2;
                int inputCount = 2;
                int outputCount = 2;

                using (VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize))
                {
                    using (VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize))
                    {
                        VstAudioBuffer[] inputBuffers = inputMgr.ToArray();
                        VstAudioBuffer[] outputBuffers = outputMgr.ToArray();

                        inputMgr.ClearBuffer(inputBuffers[0]);
                        inputMgr.ClearBuffer(inputBuffers[1]);
                        outputMgr.ClearBuffer(outputBuffers[0]);
                        outputMgr.ClearBuffer(outputBuffers[1]);

                        for (int j = 0; j < blockSize; j++)
                        {
                            // generate a value between -1.0 and 1.0
                            inputBuffers[0][j] = buffer[j * 2 + offset + 0] / (float)short.MaxValue;
                            inputBuffers[1][j] = buffer[j * 2 + offset + 1] / (float)short.MaxValue;
                        }


                        for (int i = 0; i < vstPlugins.Count; i++)
                        {
                            VstPluginContext PluginContext = vstPlugins[i].vstPlugins;
                            PluginContext.PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);
                            inputBuffers = outputBuffers;
                        }

                        for (int j = 0; j < blockSize; j++)
                        {
                            // generate a value between -1.0 and 1.0
                            buffer[j * 2 + offset + 0] = (short)(outputBuffers[0][j] * short.MaxValue);
                            buffer[j * 2 + offset + 1] = (short)(outputBuffers[1][j] * short.MaxValue);
                        }

                    }
                }
            }
            catch { }
        }

        private static int trdVgmVirtualMainFunction(short[] buffer, int offset, int sampleCount)
        {
            try
            {
                stwh.Reset();stwh.Start();

                int i;

                if (Stopped || Paused) return mds.Update(buffer, offset, sampleCount, null);

                if ((driverReal is vgm) && ((vgm)driverReal).isDataBlock) return mds.Update(buffer, offset, sampleCount, null);

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

                int cnt = 0;
                cnt = mds.Update(buffer, offset, sampleCount, driverVirtual.oneFrameProc);

                for (i = 0; i < sampleCount; i++)
                {
                    int mul = (int)(16384.0 * Math.Pow(10.0, MasterVolume / 40.0));
                    buffer[offset + i] = (short)((Limit(buffer[offset + i], 0x7fff, -0x8000) * mul) >> 14);
                }

                masterVisVolume = buffer[offset];

                int[][][] vol = mds.getYM2151VisVolume();

                if (vol != null) ym2151VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getYM2203VisVolume();
                if (vol != null) ym2203VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
                if (vol != null) ym2203FMVisVolume = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
                if (vol != null) ym2203SSGVisVolume = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);

                vol = mds.getYM2413VisVolume();
                if (vol != null) ym2413VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getYM2608VisVolume();
                if (vol != null) ym2608VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
                if (vol != null) ym2608FMVisVolume = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
                if (vol != null) ym2608SSGVisVolume = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
                if (vol != null) ym2608RtmVisVolume = (short)getMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
                if (vol != null) ym2608APCMVisVolume = (short)getMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);

                vol = mds.getYM2610VisVolume();
                if (vol != null) ym2610VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
                if (vol != null) ym2610FMVisVolume = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
                if (vol != null) ym2610SSGVisVolume = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
                if (vol != null) ym2610APCMAVisVolume = (short)getMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
                if (vol != null) ym2610APCMBVisVolume = (short)getMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);

                vol = mds.getYM2612VisVolume();
                if (vol != null) ym2612VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getAY8910VisVolume();
                if (vol != null) ay8910VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getSN76489VisVolume();
                if (vol != null) sn76489VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getHuC6280VisVolume();
                if (vol != null) huc6280VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getRF5C164VisVolume();
                if (vol != null) rf5c164VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getPWMVisVolume();
                if (vol != null) pwmVisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getOKIM6258VisVolume();
                if (vol != null) okim6258VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getOKIM6295VisVolume();
                if (vol != null) okim6295VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getC140VisVolume();
                if (vol != null) c140VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getSegaPCMVisVolume();
                if (vol != null) segaPCMVisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getC352VisVolume();
                if (vol != null) c352VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getK054539VisVolume();
                if (vol != null) k054539VisVolume = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

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

                waveWriter.Write(buffer, offset, sampleCount);

                if (vgmFadeoutCounter == 0.0)
                {
                    waveWriter.Close();

                    if (mds == null)
                        mds = new MDSound.MDSound(SamplingRate, samplingBuffer, null);
                    else
                        mds.Init(SamplingRate, samplingBuffer, null);

                    Stopped = true;

                    chipRegister.Close();
                }

                //1frame当たりの処理時間
                ProcTimePer1Frame = (int)((double)stwh.ElapsedMilliseconds / sampleCount * 1000000.0);

                return cnt;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                fatalError = true;
                Stopped = true;
            }

            return -1;
        }

        public static int getMonoVolume(int pl, int pr, int sl, int sr)
        {
            int v = pl + pr + sl + sr;
            v >>= 1;
            if (sl + sr != 0) v >>= 1;

            return v;
        }

        public static int Limit(int v, int max, int min)
        {
            return v > max ? max : (v < min ? min : v);
        }

        public static long getVirtualFrameCounter()
        {
            if (driverVirtual == null) return -1;
            return driverVirtual.vgmFrameCounter;
        }

        public static long getRealFrameCounter()
        {
            if (driverReal == null) return -1;
            return driverReal.vgmFrameCounter;
        }

        public static void SetMasterVolume(int volume)
        {
            MasterVolume = volume;
        }

        public static void SetAY8910Volume(int volume)
        {
            try
            {
                mds.setVolumeAY8910(volume);
            }
            catch { }
        }

        public static void SetYM2151Volume(int volume)
        {
            try
            {
                mds.SetVolumeYM2151(volume);
            }
            catch { }
        }

        public static void SetYM2203Volume(int volume)
        {
            try
            {
                mds.SetVolumeYM2203(volume);
            }
            catch { }
        }

        public static void SetYM2203FMVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2203FM(volume);
            }
            catch { }
        }

        public static void SetYM2203PSGVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2203PSG(volume);
            }
            catch { }
        }

        public static void SetYM2413Volume(int volume)
        {
            try
            {
                mds.SetVolumeYM2413(volume);
            }
            catch { }
        }

        public static void SetYM2608Volume(int volume)
        {
            try
            {
                mds.SetVolumeYM2608(volume);
            }
            catch { }
        }

        public static void SetYM2608FMVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2608FM(volume);
            }
            catch { }
        }

        public static void SetYM2608PSGVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2608PSG(volume);
            }
            catch { }
        }

        public static void SetYM2608RhythmVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2608Rhythm(volume);
            }
            catch { }
        }

        public static void SetYM2608AdpcmVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2608Adpcm(volume);
            }
            catch { }
        }

        public static void SetYM2610Volume(int volume)
        {
            try
            {
                mds.SetVolumeYM2610(volume);
            }
            catch { }
        }

        public static void SetYM2610FMVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2610FM(volume);
            }
            catch { }
        }

        public static void SetYM2610PSGVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2610PSG(volume);
            }
            catch { }
        }

        public static void SetYM2610AdpcmAVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2610AdpcmA(volume);
            }
            catch { }
        }

        public static void SetYM2610AdpcmBVolume(int volume)
        {
            try
            {
                mds.SetVolumeYM2610AdpcmB(volume);
            }
            catch { }
        }

        public static void SetYM2612Volume(int volume)
        {
            try
            {
                mds.SetVolumeYM2612(volume);
            }
            catch { }
        }

        public static void SetSN76489Volume(int volume)
        {
            try
            {
                mds.SetVolumeSN76489(volume);
            }
            catch { }
        }

        public static void SetHuC6280Volume(int volume)
        {
            try
            {
                mds.setVolumeHuC6280(volume);
            }
            catch { }
        }

        public static void SetRF5C164Volume(int volume)
        {
            try
            {
                mds.SetVolumeRF5C164(volume);
            }
            catch { }
        }

        public static void SetPWMVolume(int volume)
        {
            try
            {
                mds.SetVolumePWM(volume);
            }
            catch { }
        }

        public static void SetOKIM6258Volume(int volume)
        {
            try
            {
                mds.SetVolumeOKIM6258(volume);
            }
            catch { }
        }

        public static void SetOKIM6295Volume(int volume)
        {
            try
            {
                mds.SetVolumeOKIM6295(volume);
            }
            catch { }
        }

        public static void SetC140Volume(int volume)
        {
            try
            {
                mds.SetVolumeC140(volume);
            }
            catch { }
        }

        public static void SetSegaPCMVolume(int volume)
        {
            try
            {
                mds.SetVolumeSegaPCM(volume);
            }
            catch { }
        }

        public static void SetC352Volume(int volume)
        {
            try
            {
                mds.SetVolumeC352(volume);
            }
            catch { }
        }

        public static void SetK054539Volume(int volume)
        {
            try
            {
                mds.SetVolumeK054539(volume);
            }
            catch { }
        }

        public static GD3 GetGD3()
        {
            if (driverVirtual != null) return driverVirtual.GD3;
            return null;
        }

        private static VstPluginContext OpenPlugin(string pluginPath)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub();
                hostCmdStub.PluginCalled += new EventHandler<PluginCalledEventArgs>(HostCmdStub_PluginCalled);

                VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);

                // add custom data to the context
                ctx.Set("PluginPath", pluginPath);
                ctx.Set("HostCmdStub", hostCmdStub);

                // actually open the plugin itself
                ctx.PluginCommandStub.Open();

                return ctx;
            }
            catch (Exception e)
            {
                log.ForcedWrite(e);
                //MessageBox.Show(this, e.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        private static void ReleaseAllPlugins()
        {
            foreach (vstInfo2 ctx in vstPlugins)
            {
                // dispose of all (unmanaged) resources
                ctx.vstPlugins.Dispose();
            }

            vstPlugins.Clear();
        }

        private static void HostCmdStub_PluginCalled(object sender, PluginCalledEventArgs e)
        {
            HostCommandStub hostCmdStub = (HostCommandStub)sender;

            // can be null when called from inside the plugin main entry point.
            if (hostCmdStub.PluginContext.PluginInfo != null)
            {
                Debug.WriteLine("Plugin " + hostCmdStub.PluginContext.PluginInfo.PluginID + " called:" + e.Message);
            }
            else
            {
                Debug.WriteLine("The loading Plugin called:" + e.Message);
            }
        }

    }


}
