using System;
using System.Collections.Generic;
using NScci;
using System.Threading;
using System.Diagnostics;
using MDSound;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;
using System.IO;
using System.IO.Compression;
using MDPlayer.form;

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
        //private static NSoundChip[] scYMF262 = new NSoundChip[2] { null, null };
        //private static NSoundChip[] scYMF271 = new NSoundChip[2] { null, null };
        //private static NSoundChip[] scYMF278B = new NSoundChip[2] { null, null };
        //private static NSoundChip[] scYMZ280B = new NSoundChip[2] { null, null };
        //private static NSoundChip[] scAY8910 = new NSoundChip[2] { null, null };
        //private static NSoundChip[] scYM2413 = new NSoundChip[2] { null, null };
        //private static NSoundChip[] scHuC6280 = new NSoundChip[2] { null, null };

        private static ChipRegister chipRegister = null;

        private static Thread trdMain = null;
        public static bool trdClosed = false;
        private static bool _trdStopped = true;
        public static bool trdStopped
        {
            get
            {
                lock (lockObj)
                {
                    return _trdStopped;
                }
            }
            set
            {
                lock (lockObj)
                {
                    _trdStopped = value;
                }
            }
        }
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

        public static ChipLEDs chipLED = new ChipLEDs();
        public static VisVolume visVolume = new VisVolume();

        private static int MasterVolume = 0;
        private static byte[] chips = new byte[256];
        private static string PlayingFileName;
        private static int MidiMode = 0;
        private static int SongNo = 0;
        private static List<Tuple<string, byte[]>> ExtendFile = null;
        private static enmFileFormat PlayingFileFormat;
        public static cNscci cnscci;
        private static System.Diagnostics.Stopwatch stwh = System.Diagnostics.Stopwatch.StartNew();
        public static int ProcTimePer1Frame = 0;

        private static List<vstInfo2> vstPlugins = new List<vstInfo2>();
        private static List<vstInfo2> vstPluginsInst = new List<vstInfo2>();

        private static List<NAudio.Midi.MidiOut> midiOuts = new List<NAudio.Midi.MidiOut>();
        private static List<int> midiOutsType = new List<int>();
        private static List<vstInfo2> vstMidiOuts = new List<vstInfo2>();
        private static List<int> vstMidiOutsType = new List<int>();



        public static List<vstInfo2> getVSTInfos()
        {
            return vstPlugins;
        }

        public static vstInfo getVSTInfo(string filename)
        {
            VstPluginContext ctx = OpenPlugin(filename);
            if (ctx == null) return null;

            vstInfo ret = new vstInfo();
            ret.effectName = ctx.PluginCommandStub.GetEffectName();
            ret.productName = ctx.PluginCommandStub.GetProductString();
            ret.vendorName = ctx.PluginCommandStub.GetVendorString();
            ret.programName = ctx.PluginCommandStub.GetProgramName();
            ret.fileName = filename;
            ret.midiInputChannels = ctx.PluginCommandStub.GetNumberOfMidiInputChannels();
            ret.midiOutputChannels = ctx.PluginCommandStub.GetNumberOfMidiOutputChannels();
            ctx.PluginCommandStub.Close();

            return ret;
        }

        public static bool addVSTeffect(string fileName)
        {
            VstPluginContext ctx = OpenPlugin(fileName);
            if (ctx == null) return false;

            //Stop();

            vstInfo2 vi = new vstInfo2();
            vi.vstPlugins = ctx;
            vi.fileName = fileName;
            vi.key = DateTime.Now.Ticks.ToString();
            Thread.Sleep(1);

            ctx.PluginCommandStub.SetBlockSize(512);
            ctx.PluginCommandStub.SetSampleRate(common.SampleRate);
            ctx.PluginCommandStub.MainsChanged(true);
            ctx.PluginCommandStub.StartProcess();
            vi.effectName = ctx.PluginCommandStub.GetEffectName();
            vi.power = true;
            ctx.PluginCommandStub.GetParameterProperties(0);


            frmVST dlg = new frmVST();
            dlg.PluginCommandStub = ctx.PluginCommandStub;
            dlg.Show(vi);
            vi.vstPluginsForm = dlg;
            vi.editor = true;

            vstPlugins.Add(vi);

            List<vstInfo> lvi = new List<vstInfo>();
            foreach (vstInfo2 vi2 in vstPlugins)
            {
                vstInfo v = new vstInfo();
                v.editor = vi.editor;
                v.effectName = vi.effectName;
                v.fileName = vi.fileName;
                v.key = vi.key;
                v.location = vi.location;
                v.midiInputChannels = vi.midiInputChannels;
                v.midiOutputChannels = vi.midiOutputChannels;
                v.param = vi.param;
                v.power = vi.power;
                v.productName = vi.productName;
                v.programName = vi.programName;
                v.vendorName = vi.vendorName;
                lvi.Add(v);
            }
            setting.vst.VSTInfo = lvi.ToArray();

            return true;
        }

        public static bool delVSTeffect(string key)
        {
            if (key == "")
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
                setting.vst.VSTInfo = new vstInfo[0];
            }
            else
            {
                int ind = -1;
                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    //if (vstPlugins[i].fileName == fileName)
                    if (vstPlugins[i].key == key)
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

                List<vstInfo> nvst = new List<vstInfo>();
                foreach (vstInfo vi in setting.vst.VSTInfo)
                {
                    if (vi.key == key) continue;
                    nvst.Add(vi);
                }
                setting.vst.VSTInfo = nvst.ToArray();
            }

            return true;
        }

        public static List<PlayList.music> getMusic(string file, byte[] buf, string zipFile = null,object entry=null)
        {
            List<PlayList.music> musics = new List<PlayList.music>();
            PlayList.music music = new PlayList.music();

            music.format = enmFileFormat.unknown;
            music.fileName = file;
            music.arcFileName = zipFile;
            music.title = "unknown";
            music.game = "unknown";
            music.type = "-";

            if (file.ToLower().LastIndexOf(".nrd") != -1)
            {

                music.format = enmFileFormat.NRT;
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
            else if (file.ToLower().LastIndexOf(".mdr") != -1)
            {

                music.format = enmFileFormat.MDR;
                uint index = 0;
                GD3 gd3 = (new Driver.MoonDriver.MoonDriver()).getGD3Info(buf, index);
                music.title = gd3.TrackName=="" ? Path.GetFileName( file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
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
            else if (file.ToLower().LastIndexOf(".nsf") != -1)
            {
                nsf nsf = new nsf();
                GD3 gd3 = nsf.getGD3Info(buf, 0);

                if (gd3 != null)
                {
                    for (int s = 0; s < nsf.songs; s++)
                    {
                        music = new PlayList.music();
                        music.format = enmFileFormat.NSF;
                        music.fileName = file;
                        music.arcFileName = zipFile;
                        music.title = string.Format("{0} - Trk {1}", gd3.GameName, s + 1);
                        music.titleJ = string.Format("{0} - Trk {1}", gd3.GameNameJ, s + 1);
                        music.game = gd3.GameName;
                        music.gameJ = gd3.GameNameJ;
                        music.composer = gd3.Composer;
                        music.composerJ = gd3.ComposerJ;
                        music.vgmby = gd3.VGMBy;
                        music.converted = gd3.Converted;
                        music.notes = gd3.Notes;
                        music.songNo = s;

                        musics.Add(music);
                    }

                    return musics;
                }
                else
                {
                    music.format = enmFileFormat.NSF;
                    music.fileName = file;
                    music.arcFileName = zipFile;
                    music.game = "unknown";
                    music.type = "-";
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }

            }
            else if (file.ToLower().LastIndexOf(".hes") != -1)
            {
                hes hes = new hes();
                GD3 gd3 = hes.getGD3Info(buf, 0);

                for (int s = 0; s < 256; s++)
                {
                    music = new PlayList.music();
                    music.format = enmFileFormat.HES;
                    music.fileName = file;
                    music.arcFileName = zipFile;
                    music.title = string.Format("{0} - Trk {1}", System.IO.Path.GetFileName(file), s + 1);
                    music.titleJ = string.Format("{0} - Trk {1}", System.IO.Path.GetFileName(file), s + 1);
                    music.game = "";
                    music.gameJ = "";
                    music.composer = "";
                    music.composerJ = "";
                    music.vgmby = "";
                    music.converted = "";
                    music.notes = "";
                    music.songNo = s;

                    musics.Add(music);
                }

                return musics;

            }
            else if (file.ToLower().LastIndexOf(".sid") != -1)
            {
                Driver.SID.sid sid = new Driver.SID.sid();
                GD3 gd3 = sid.getGD3Info(buf, 0);

                for (int s = 0; s < sid.songs; s++)
                {
                    music = new PlayList.music();
                    music.format = enmFileFormat.SID;
                    music.fileName = file;
                    music.arcFileName = zipFile;
                    music.title = string.Format("{0} - Trk {1}", gd3.TrackName, s + 1);
                    music.titleJ = string.Format("{0} - Trk {1}", gd3.TrackName, s + 1);
                    music.game = "";
                    music.gameJ = "";
                    music.composer = gd3.Composer;
                    music.composerJ = gd3.Composer;
                    music.vgmby = "";
                    music.converted = "";
                    music.notes = gd3.Notes;
                    music.songNo = s;

                    musics.Add(music);
                }

                return musics;

            }
            else if (file.ToLower().LastIndexOf(".mid") != -1)
            {
                music.format = enmFileFormat.MID;
                GD3 gd3 = new MID().getGD3Info(buf, 0);
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

                if (music.title == "" && music.titleJ == "")
                {
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }

            }
            else if (file.ToLower().LastIndexOf(".rcp") != -1)
            {
                music.format = enmFileFormat.RCP;
                GD3 gd3 = new RCP().getGD3Info(buf, 0);
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

                if (music.title == "" && music.titleJ == "")
                {
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }

            }
            else
            {
                if (buf.Length < 0x40)
                {
                    musics.Add(music);
                    return musics;
                }
                if (common.getLE32(buf, 0x00) != vgm.FCC_VGM)
                {
                    //musics.Add(music);
                    //return musics;
                    //VGZかもしれないので確認する
                    try
                    {
                        int num;
                        buf = new byte[1024]; // 1Kbytesずつ処理する

                        if (entry == null || entry is ZipArchiveEntry)
                        {
                            Stream inStream; // 入力ストリーム
                            if (entry == null)
                            {
                                inStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                            }
                            else
                            {
                                inStream = ((ZipArchiveEntry)entry).Open();
                            }
                            GZipStream decompStream // 解凍ストリーム
                              = new GZipStream(
                                inStream, // 入力元となるストリームを指定
                                CompressionMode.Decompress); // 解凍（圧縮解除）を指定

                            MemoryStream outStream // 出力ストリーム
                              = new MemoryStream();

                            using (inStream)
                            using (outStream)
                            using (decompStream)
                            {
                                while ((num = decompStream.Read(buf, 0, buf.Length)) > 0)
                                {
                                    outStream.Write(buf, 0, num);
                                }
                            }

                            buf = outStream.ToArray();
                        }
                        else
                        {
                            UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                            buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                        }
                    }
                    catch
                    {
                        //vgzではなかった
                    }
                }

                if (common.getLE32(buf, 0x00) != vgm.FCC_VGM)
                {
                    musics.Add(music);
                    return musics;
                }

                music.format = enmFileFormat.VGM;
                uint version = common.getLE32(buf, 0x08);
                string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint vgmGd3 = common.getLE32(buf, 0x14);
                GD3 gd3 = new GD3();
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = common.getLE32(buf, vgmGd3 + 0x14);
                    if (vgmGd3Id != vgm.FCC_GD3)
                    {
                        musics.Add(music);
                        return musics;
                    }
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

                double sec = (double)TotalCounter / (double)common.SampleRate;
                int TCminutes = (int)(sec / 60);
                sec -= TCminutes * 60;
                int TCsecond = (int)sec;
                sec -= TCsecond;
                int TCmillisecond = (int)(sec * 100.0);
                music.duration = string.Format("{0:D2}:{1:D2}:{2:D2}", TCminutes, TCsecond, TCmillisecond);
            }

            musics.Add(music);
            return musics;
        }

        public static List<PlayList.music> getMusic(PlayList.music ms, byte[] buf, string zipFile = null)
        {
            List<PlayList.music> musics = new List<PlayList.music>();
            PlayList.music music = new PlayList.music();

            music.format = enmFileFormat.unknown;
            music.fileName = ms.fileName;
            music.arcFileName = zipFile;
            music.title = "unknown";
            music.game = "unknown";
            music.type = "-";

            if (ms.fileName.ToLower().LastIndexOf(".nrd") != -1)
            {

                music.format = enmFileFormat.NRT;
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
            else if (ms.fileName.ToLower().LastIndexOf(".xgm") != -1)
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
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }
            }
            else if (ms.fileName.ToLower().LastIndexOf(".s98") != -1)
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
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

            }
            else if (ms.fileName.ToLower().LastIndexOf(".nsf") != -1)
            {
                nsf nsf = new nsf();
                GD3 gd3 = nsf.getGD3Info(buf, 0);

                if (gd3 != null)
                {
                    if (ms.songNo == -1)
                    {
                        for (int s = 0; s < nsf.songs; s++)
                        {
                            music = new PlayList.music();
                            music.format = enmFileFormat.NSF;
                            music.fileName = ms.fileName;
                            music.arcFileName = zipFile;
                            music.title = string.Format("{0} - Trk {1}", gd3.GameName, s);
                            music.titleJ = string.Format("{0} - Trk {1}", gd3.GameNameJ, s);
                            music.game = gd3.GameName;
                            music.gameJ = gd3.GameNameJ;
                            music.composer = gd3.Composer;
                            music.composerJ = gd3.ComposerJ;
                            music.vgmby = gd3.VGMBy;
                            music.converted = gd3.Converted;
                            music.notes = gd3.Notes;
                            music.songNo = s;

                            musics.Add(music);
                        }

                        return musics;

                    }
                    else
                    {
                        music.format = enmFileFormat.NSF;
                        music.fileName = ms.fileName;
                        music.arcFileName = zipFile;
                        music.title = ms.title;
                        music.titleJ = ms.titleJ;
                        music.game = gd3.GameName;
                        music.gameJ = gd3.GameNameJ;
                        music.composer = gd3.Composer;
                        music.composerJ = gd3.ComposerJ;
                        music.vgmby = gd3.VGMBy;
                        music.converted = gd3.Converted;
                        music.notes = gd3.Notes;
                        music.songNo = ms.songNo;
                    }
                }
                else
                {
                    music.format = enmFileFormat.NSF;
                    music.fileName = ms.fileName;
                    music.arcFileName = zipFile;
                    music.game = "unknown";
                    music.type = "-";
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

            }
            else if (ms.fileName.ToLower().LastIndexOf(".mid") != -1)
            {
                music.format = enmFileFormat.MID;
                GD3 gd3 = new MID().getGD3Info(buf, 0);
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
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

                if (music.title == "" && music.titleJ == "")
                {
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

            }
            else if (ms.fileName.ToLower().LastIndexOf(".rcp") != -1)
            {
                music.format = enmFileFormat.RCP;
                GD3 gd3 = new RCP().getGD3Info(buf, 0);
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
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

                if (music.title == "" && music.titleJ == "")
                {
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

            }
            else
            {
                if (buf.Length < 0x40)
                {
                    musics.Add(music);
                    return musics;
                }
                if (common.getLE32(buf, 0x00) != vgm.FCC_VGM)
                {
                    musics.Add(music);
                    return musics;
                }

                music.format = enmFileFormat.VGM;
                uint version = common.getLE32(buf, 0x08);
                string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint vgmGd3 = common.getLE32(buf, 0x14);
                GD3 gd3 = new GD3();
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = common.getLE32(buf, vgmGd3 + 0x14);
                    if (vgmGd3Id != vgm.FCC_GD3)
                    {
                        musics.Add(music);
                        return musics;
                    }
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

                double sec = (double)TotalCounter / (double)common.SampleRate;
                int TCminutes = (int)(sec / 60);
                sec -= TCminutes * 60;
                int TCsecond = (int)sec;
                sec -= TCsecond;
                int TCmillisecond = (int)(sec * 100.0);
                music.duration = string.Format("{0:D2}:{1:D2}:{2:D2}", TCminutes, TCsecond, TCmillisecond);
            }

            musics.Add(music);
            return musics;
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

            naudioWrap = new NAudioWrap((int)common.SampleRate, trdVgmVirtualFunction);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;

            log.ForcedWrite("Audio:Init:STEP 02");

            Audio.setting = setting;// Copy();

            waveWriter = new WaveWriter(setting);

            log.ForcedWrite("Audio:Init:STEP 03");

            if (mds == null)
                mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, null);
            else
                mds.Init((UInt32)common.SampleRate, samplingBuffer, null);

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
            chip.SamplingRate = (UInt32)common.SampleRate;
            chip.Volume = setting.balance.YM2612Volume;
            chip.Clock = 7670454;
            chip.Option = null;
            chipLED.PriOPN2 = 1;
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
            chip.SamplingRate = (UInt32)common.SampleRate;
            chip.Volume = setting.balance.SN76489Volume;
            chip.Clock = 3579545;
            chip.Option = null;
            chipLED.PriDCSG = 1;
            lstChips.Add(chip);

            if (mdsMIDI == null)
                mdsMIDI = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
            else
                mdsMIDI.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

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
            //scYMF262[0] = getChip(Audio.setting.YMF262Type);
            //if (scYMF262[0] != null) scYMF262[0].init();
            //scYMF271[0] = getChip(Audio.setting.YMF271Type);
            //if (scYMF271[0] != null) scYMF271[0].init();
            //scYMF278B[0] = getChip(Audio.setting.YMF278BType);
            //if (scYMF278B[0] != null) scYMF278B[0].init();
            //scYMZ280B[0] = getChip(Audio.setting.YMZ280BType);
            //if (scYMZ280B[0] != null) scYMZ280B[0].init();
            //scAY8910[0] = getChip(Audio.setting.AY8910Type);
            //if (scAY8910[0] != null) scAY8910[0].init();
            //scYM2413[0] = getChip(Audio.setting.YM2413Type);
            //if (scYM2413[0] != null) scYM2413[0].init();
            //scHuC6280[0] = getChip(Audio.setting.HuC6280Type);
            //if (scHuC6280[0] != null) scHuC6280[0].init();

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
            //scYMF262[1] = getChip(Audio.setting.YMF262SType);
            //if (scYMF262[1] != null) scYMF262[1].init();
            //scYMF271[1] = getChip(Audio.setting.YMF271SType);
            //if (scYMF271[1] != null) scYMF271[1].init();
            //scYMF278B[1] = getChip(Audio.setting.YMF278BSType);
            //if (scYMF278B[1] != null) scYMF278B[1].init();
            //scYMZ280B[1] = getChip(Audio.setting.YMZ280BSType);
            //if (scYMZ280B[1] != null) scYMZ280B[1].init();
            //scAY8910[1] = getChip(Audio.setting.AY8910SType);
            //if (scAY8910[1] != null) scAY8910[1].init();
            //scYM2413[1] = getChip(Audio.setting.YM2413SType);
            //if (scYM2413[1] != null) scYM2413[1].init();

            chipRegister = new ChipRegister(
                setting
                , mds
                , scYM2612
                , scSN76489
                , scYM2608
                , scYM2151
                , scYM2203
                , scYM2610
                //, scYMF262
                //, scYMF271
                //, scYMF278B
                //, scYMZ280B
                //, scAY8910
                //, scYM2413
                //, scHuC6280
                //, new Setting.ChipType[] { setting.YM2612Type, setting.YM2612SType }
                //, new Setting.ChipType[] { setting.SN76489Type, setting.SN76489SType }
                //, new Setting.ChipType[] { setting.YM2608Type, setting.YM2608SType }
                //, new Setting.ChipType[] { setting.YM2151Type, setting.YM2151SType }
                //, new Setting.ChipType[] { setting.YM2203Type, setting.YM2203SType }
                //, new Setting.ChipType[] { setting.YM2610Type, setting.YM2610SType }
                //, new Setting.ChipType[] { setting.YMF262Type, setting.YMF262SType }
                //, new Setting.ChipType[] { setting.YMF271Type, setting.YMF271SType }
                //, new Setting.ChipType[] { setting.YMF278BType, setting.YMF278BSType }
                //, new Setting.ChipType[] { setting.YMZ280BType, setting.YMZ280BSType }
                //, new Setting.ChipType[] { setting.AY8910Type, setting.AY8910SType }
                //, new Setting.ChipType[] { setting.YM2413Type, setting.YM2413SType }
                //, new Setting.ChipType[] { setting.HuC6280Type, setting.HuC6280SType }
                );
            chipRegister.initChipRegister();

            log.ForcedWrite("Audio:Init:STEP 05");

            Paused = false;
            Stopped = true;
            fatalError = false;
            oneTimeReset = false;

            log.ForcedWrite("Audio:Init:STEP 06");

            log.ForcedWrite("Audio:Init:VST:STEP 01");

            vstparse();

            log.ForcedWrite("Audio:Init:VST:STEP 02"); //Load VST instrument

            //複数のmidioutの設定から必要なVSTを絞り込む
            Dictionary<string, int> dicVst = new Dictionary<string, int>();
            if (setting.midiOut.lstMidiOutInfo != null)
            {
                foreach (midiOutInfo[] aryMoi in setting.midiOut.lstMidiOutInfo)
                {
                    if (aryMoi == null) continue;
                    Dictionary<string, int> dicVst2 = new Dictionary<string, int>();
                    foreach (midiOutInfo moi in aryMoi)
                    {
                        if (!moi.isVST) continue;
                        if (dicVst2.ContainsKey(moi.fileName))
                        {
                            dicVst2[moi.fileName]++;
                            continue;
                        }
                        dicVst2.Add(moi.fileName, 1);
                    }

                    foreach (var kv in dicVst2)
                    {
                        if (dicVst.ContainsKey(kv.Key))
                        {
                            if (dicVst[kv.Key] < kv.Value)
                            {
                                dicVst[kv.Key] = kv.Value;
                            }
                            continue;
                        }
                        dicVst.Add(kv.Key, kv.Value);
                    }
                }
            }

            foreach (var kv in dicVst)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    VstPluginContext ctx = OpenPlugin(kv.Key);
                    if (ctx == null) continue;

                    vstInfo2 vi = new vstInfo2();
                    vi.key = DateTime.Now.Ticks.ToString();
                    Thread.Sleep(1);
                    vi.vstPlugins = ctx;
                    vi.fileName = kv.Key;
                    vi.isInstrument = true;

                    ctx.PluginCommandStub.SetBlockSize(512);
                    ctx.PluginCommandStub.SetSampleRate(common.SampleRate);
                    ctx.PluginCommandStub.MainsChanged(true);
                    ctx.PluginCommandStub.StartProcess();
                    vi.effectName = ctx.PluginCommandStub.GetEffectName();
                    vi.editor = true;

                    if (vi.editor)
                    {
                        frmVST dlg = new frmVST();
                        dlg.PluginCommandStub = ctx.PluginCommandStub;
                        dlg.Show(vi);
                        vi.vstPluginsForm = dlg;
                    }

                    vstPluginsInst.Add(vi);
                }
            }


            if (setting.vst != null && setting.vst.VSTInfo != null)
            {

                log.ForcedWrite("Audio:Init:VST:STEP 03"); //Load VST Effect

                for (int i = 0; i < setting.vst.VSTInfo.Length; i++)
                {
                    if (setting.vst.VSTInfo[i] == null) continue;
                    VstPluginContext ctx = OpenPlugin(setting.vst.VSTInfo[i].fileName);
                    if (ctx == null) continue;

                    vstInfo2 vi = new vstInfo2();
                    vi.vstPlugins = ctx;
                    vi.fileName = setting.vst.VSTInfo[i].fileName;
                    vi.key = setting.vst.VSTInfo[i].key;

                    ctx.PluginCommandStub.SetBlockSize(512);
                    ctx.PluginCommandStub.SetSampleRate(common.SampleRate/1000.0f);
                    ctx.PluginCommandStub.MainsChanged(true);
                    ctx.PluginCommandStub.StartProcess();
                    vi.effectName = ctx.PluginCommandStub.GetEffectName();
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

            log.ForcedWrite("Audio:Init:STEP 07");

            //midi outをリリース
            ReleaseAllMIDIout();

            log.ForcedWrite("Audio:Init:STEP 08");

            //midi out のインスタンスを作成
            MakeMIDIout(setting, 1);
            chipRegister.resetAllMIDIout();

            log.ForcedWrite("Audio:Init:STEP 09");

            naudioWrap.Start(Audio.setting);

            log.ForcedWrite("Audio:Init:Complete");

        }

        private static void vstparse()
        {
            while (vstPluginsInst.Count > 0)
            {
                if (vstPluginsInst[0] != null)
                {
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.EditorClose();
                    vstPluginsInst[0].vstPluginsForm.timer1.Enabled = false;
                    vstPluginsInst[0].location = vstPluginsInst[0].vstPluginsForm.Location;
                    vstPluginsInst[0].vstPluginsForm.Close();
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.StopProcess();
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.MainsChanged(false);
                    vstPluginsInst[0].vstPlugins.Dispose();
                }

                vstPluginsInst.RemoveAt(0);
            }

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
        }

        private static void MakeMIDIout(Setting setting, int m)
        {
            if (setting.midiOut.lstMidiOutInfo == null || setting.midiOut.lstMidiOutInfo.Count < 1) return;
            if (setting.midiOut.lstMidiOutInfo[m] == null || setting.midiOut.lstMidiOutInfo[m].Length < 1) return;

            for (int i = 0; i < setting.midiOut.lstMidiOutInfo[m].Length; i++)
            {
                int n = -1;
                int t = 0;
                NAudio.Midi.MidiOut mo = null;
                int vn = -1;
                int vt = 0;
                vstInfo2 vmo = null;

                for (int j = 0; j < NAudio.Midi.MidiOut.NumberOfDevices; j++)
                {
                    if (setting.midiOut.lstMidiOutInfo[m][i].name != NAudio.Midi.MidiOut.DeviceInfo(j).ProductName) continue;

                    n = j;
                    t = setting.midiOut.lstMidiOutInfo[m][i].type;
                    break;
                }

                if (n != -1)
                {
                    try
                    {
                        mo = new NAudio.Midi.MidiOut(n);
                    }
                    catch
                    {
                        mo = null;
                    }
                }


                if (n == -1)
                {
                    for (int j = 0; j < vstPluginsInst.Count; j++)
                    {
                        if (!vstPluginsInst[j].isInstrument || setting.midiOut.lstMidiOutInfo[m][i].fileName != vstPluginsInst[j].fileName) continue;
                        bool k = false;
                        foreach (vstInfo2 v in vstMidiOuts) if (v == vstPluginsInst[j]) { k = true; break; }
                        if (k) continue;
                        vn = j;
                        vt = setting.midiOut.lstMidiOutInfo[m][i].type;
                        break;
                    }

                    if (vn != -1)
                    {
                        try
                        {
                            vmo = vstPluginsInst[vn];
                        }
                        catch
                        {
                            vmo = null;
                        }
                    }
                }

                if (mo != null || vmo != null)
                {
                    midiOuts.Add(mo);
                    midiOutsType.Add(t);

                    vstMidiOuts.Add(vmo);
                    vstMidiOutsType.Add(vt);
                }
            }
        }

        private static void ReleaseAllMIDIout()
        {
            if (midiOuts.Count > 0)
            {
                for (int i = 0; i < midiOuts.Count; i++)
                {
                    if (midiOuts[i] != null)
                    {
                        midiOuts[i].Reset();
                        midiOuts[i].Close();
                        midiOuts[i] = null;
                    }
                }
                midiOuts.Clear();
                midiOutsType.Clear();
            }

            if (vstMidiOuts.Count > 0)
            {
                vstMidiOuts.Clear();
                vstMidiOutsType.Clear();
            }
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
                return (int)common.SampleRate * setting.outputDevice.Latency / 1000;
            }
            return naudioWrap.getAsioLatency();
        }

        public static void SetVGMBuffer(enmFileFormat format, byte[] srcBuf, string playingFileName,int midiMode,int songNo,List<Tuple<string,byte[]>> extFile)
        {
            Stop();
            PlayingFileFormat = format;
            vgmBuf = srcBuf;
            PlayingFileName = playingFileName;//WaveWriter向け
            MidiMode = midiMode;
            SongNo = songNo;
            chipRegister.SetFileName(playingFileName);//ExportMIDI向け
            ExtendFile = extFile;//追加ファイル
        }

        public static bool Play(Setting setting)
        {
            waveWriter.Open(PlayingFileName);

            if (PlayingFileFormat == enmFileFormat.NRT)
            {
                driverVirtual = new NRTDRV();
                driverReal = new NRTDRV();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                return nrdPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.MDR)
            {
                driverVirtual = new Driver.MoonDriver.MoonDriver();
                driverReal = new Driver.MoonDriver.MoonDriver();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                ((Driver.MoonDriver.MoonDriver)driverVirtual).ExtendFile = (ExtendFile != null && ExtendFile.Count > 0) ? ExtendFile[0] : null;
                ((Driver.MoonDriver.MoonDriver)driverReal).ExtendFile = (ExtendFile != null && ExtendFile.Count > 0) ? ExtendFile[0] : null;
                return mdrPlay(setting);
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

            if (PlayingFileFormat == enmFileFormat.MID)
            {
                driverVirtual = new MID();
                driverReal = new MID();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                return midPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.RCP)
            {
                driverVirtual = new RCP();
                driverReal = new RCP();
                driverVirtual.setting = setting;
                driverReal.setting = setting;
                ((RCP)driverVirtual).ExtendFile=ExtendFile;
                ((RCP)driverReal).ExtendFile = ExtendFile;
                return rcpPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.NSF)
            {
                driverVirtual = new nsf();
                driverReal = new nsf();
                driverVirtual.setting = setting;
                driverReal.setting = setting;

                return nsfPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.HES)
            {
                driverVirtual = new hes();
                driverReal = new hes();
                driverVirtual.setting = setting;
                driverReal.setting = setting;

                return hesPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.SID)
            {
                driverVirtual = new Driver.SID.sid();
                driverReal = new Driver.SID.sid();
                driverVirtual.setting = setting;
                driverReal.setting = setting;

                return sidPlay(setting);
            }

            if (PlayingFileFormat == enmFileFormat.VGM)
            {
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

            return false;
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

                chipLED = new ChipLEDs();

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2151Volume;
                        chip.Clock = 4000000;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPM = 1;
                        else chipLED.SecOPM = 1;

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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.AY8910Volume;
                    chip.Clock = 2000000 / 2;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x1;
                    chipLED.PriAY10 = 1;

                    lstChips.Add(chip);
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2151Volume(setting.balance.YM2151Volume);
                SetAY8910Volume(setting.balance.AY8910Volume);

                driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.YM2151, enmUseChip.AY8910 }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000));
                driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.YM2151, enmUseChip.AY8910 }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000));
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

        public static bool mdrPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

                //int r = ((NRTDRV)driverVirtual).checkUseChip(vgmBuf);

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

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                MDSound.ymf278b ymf278b = new MDSound.ymf278b();

                chip = new MDSound.MDSound.Chip();
                chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
                chip.ID = 0;
                chip.Instrument = ymf278b;
                chip.Update = ymf278b.Update;
                chip.Start = ymf278b.Start;
                chip.Stop = ymf278b.Stop;
                chip.Reset = ymf278b.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Volume = setting.balance.YMF278BVolume;
                chip.Clock = 33868800;// 4000000;
                chip.Option = null;

                hiyorimiDeviceFlag |= 0x2;

                chipLED.PriOPL4 = 1;

                lstChips.Add(chip);

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYMF278BVolume(setting.balance.YMF278BVolume);

                driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000));
                driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000));

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

                chipLED = new ChipLEDs();

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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.YM2612Volume;
                    chip.Clock = 7670454;
                    chip.Option = null;
                    chipLED.PriOPN2 = 1;
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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.SN76489Volume;
                    chip.Clock = 3579545;
                    chip.Option = null;
                    chipLED.PriDCSG = 1;
                    lstChips.Add(chip);

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister();

                SetYM2612Volume(setting.balance.YM2612Volume);
                SetSN76489Volume(setting.balance.SN76489Volume);

                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.YM2612, enmUseChip.SN76489 }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.YM2612, enmUseChip.SN76489 }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;

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

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.YM2203 }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel,new enmUseChip[] { enmUseChip.YM2203 }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;

                List<S98.S98DevInfo> s98DInfo = ((S98)driverVirtual).s98Info.DeviceInfos;

                ay8910 ym2149 = null;
                ym2203 ym2203 = null;
                ym2612 ym2612 = null;
                ym2608 ym2608 = null;
                ym2151 ym2151 = null;
                ym2413 ym2413 = null;
                ay8910 ay8910 = null;
                foreach (S98.S98DevInfo dInfo in s98DInfo)
                {
                    switch (dInfo.DeviceType)
                    {
                        case 1:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2149 == null)
                            {
                                ym2149 = new ay8910();
                                chip.ID = 0;
                                chipLED.PriAY10 = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecAY10 = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                            chip.Instrument = ym2149;
                            chip.Update = ym2149.Update;
                            chip.Start = ym2149.Start;
                            chip.Stop = ym2149.Stop;
                            chip.Reset = ym2149.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
                            chip.Volume = setting.balance.AY8910Volume;
                            chip.Clock = dInfo.Clock/4;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);

                            break;
                        case 2:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2203 == null)
                            {
                                ym2203 = new ym2203();
                                chip.ID = 0;
                                chipLED.PriOPN = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPN = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2203;
                            chip.Instrument = ym2203;
                            chip.Update = ym2203.Update;
                            chip.Start = ym2203.Start;
                            chip.Stop = ym2203.Stop;
                            chip.Reset = ym2203.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
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
                                chipLED.PriOPN = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPN = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                            chip.Instrument = ym2612;
                            chip.Update = ym2612.Update;
                            chip.Start = ym2612.Start;
                            chip.Stop = ym2612.Stop;
                            chip.Reset = ym2612.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
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
                                chipLED.PriOPNA=1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPNA = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                            chip.Instrument = ym2608;
                            chip.Update = ym2608.Update;
                            chip.Start = ym2608.Start;
                            chip.Stop = ym2608.Stop;
                            chip.Reset = ym2608.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
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
                                chipLED.PriOPM = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPM = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                            chip.Instrument = ym2151;
                            chip.Update = ym2151.Update;
                            chip.Start = ym2151.Start;
                            chip.Stop = ym2151.Stop;
                            chip.Reset = ym2151.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
                            chip.Volume = setting.balance.YM2151Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);

                            break;
                        case 6:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2413 == null)
                            {
                                ym2413 = new ym2413();
                                chip.ID = 0;
                                chipLED.PriOPLL = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPLL = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2413;
                            chip.Instrument = ym2413;
                            chip.Update = ym2413.Update;
                            chip.Start = ym2413.Start;
                            chip.Stop = ym2413.Stop;
                            chip.Reset = ym2413.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
                            chip.Volume = setting.balance.YM2413Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);

                            break;
                        case 15:
                            chip = new MDSound.MDSound.Chip();
                            if (ay8910 == null)
                            {
                                ay8910 = new ay8910();
                                chip.ID = 0;
                                chipLED.PriAY10 = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecAY10 = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                            chip.Instrument = ay8910;
                            chip.Update = ay8910.Update;
                            chip.Start = ay8910.Start;
                            chip.Stop = ay8910.Stop;
                            chip.Reset = ay8910.Reset;
                            chip.SamplingRate = (UInt32)common.SampleRate;
                            chip.Volume = setting.balance.AY8910Volume;
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
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

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

                SetYM2413Volume(setting.balance.YM2413Volume);

                SetAY8910Volume(setting.balance.AY8910Volume);

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

        public static bool midPlay(Setting setting)
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

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriMID = 1;
                chipLED.SecMID = 1;

                MasterVolume = setting.balance.MasterVolume;

                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel,new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                chipRegister.initChipRegister();
                ReleaseAllMIDIout();
                MakeMIDIout(setting, MidiMode);

                chipRegister.setMIDIout(setting.midiOut.lstMidiOutInfo[MidiMode], midiOuts, midiOutsType, vstMidiOuts, vstMidiOutsType);

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

        public static bool rcpPlay(Setting setting)
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

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriMID = 1;
                chipLED.SecMID = 1;

                MasterVolume = setting.balance.MasterVolume;

                chipRegister.initChipRegister();
                ReleaseAllMIDIout();
                MakeMIDIout(setting, MidiMode);
                chipRegister.setMIDIout(setting.midiOut.lstMidiOutInfo[MidiMode], midiOuts, midiOutsType, vstMidiOuts, vstMidiOutsType);

                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

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

        public static bool nsfPlay(Setting setting)
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

                chipRegister.initChipRegister();


                trdClosed = false;
                trdMain = new Thread(new ThreadStart(trdVgmRealFunction));
                trdMain.Priority = ThreadPriority.Highest;
                trdMain.IsBackground = true;
                trdMain.Name = "trdVgmReal";
                trdMain.Start();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriNES = 1;
                chipLED.PriDMC = 1;

                MasterVolume = setting.balance.MasterVolume;

                ((nsf)driverVirtual).song = SongNo;
                ((nsf)driverReal).song = SongNo;
                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;

                if (((nsf)driverVirtual).use_fds) chipLED.PriFDS = 1;
                if (((nsf)driverVirtual).use_fme7) chipLED.PriFME7 = 1;
                if (((nsf)driverVirtual).use_mmc5) chipLED.PriMMC5 = 1;
                if (((nsf)driverVirtual).use_n106) chipLED.PriN160 = 1;
                if (((nsf)driverVirtual).use_vrc6) chipLED.PriVRC6 = 1;
                if (((nsf)driverVirtual).use_vrc7) chipLED.PriVRC7 = 1;

                //nes_intf nes = new nes_intf();
                MDSound.MDSound.Chip chip;
                nes_intf nes = new nes_intf();

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.Nes;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Volume = setting.balance.APUVolume;
                chip.Clock = 0;
                chip.Option = null;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cAPU = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.DMC;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.DMCVolume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cDMC = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.FDS;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.FDSVolume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cFDS = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.MMC5;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.MMC5Volume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cMMC5 = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.N160;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.N160Volume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cN160 = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.VRC6;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.VRC6Volume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cVRC6 = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.VRC7;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.VRC7Volume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cVRC7 = chip;

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.FME7;
                chip.Instrument = nes;
                chip.Update = nes.Update;
                chip.Start = nes.Start;
                chip.Stop = nes.Stop;
                chip.Reset = nes.Reset;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Clock = 0;
                chip.Option = null;
                chip.Volume = setting.balance.FME7Volume;
                lstChips.Add(chip);
                ((nsf)driverVirtual).cFME7 = chip;

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

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

        public static bool hesPlay(Setting setting)
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

                chipRegister.initChipRegister();


                trdClosed = false;
                trdMain = new Thread(new ThreadStart(trdVgmRealFunction));
                trdMain.Priority = ThreadPriority.Highest;
                trdMain.IsBackground = true;
                trdMain.Name = "trdVgmReal";
                trdMain.Start();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriHuC = 1;

                MasterVolume = setting.balance.MasterVolume;

                //((hes)driverVirtual).song = (byte)SongNo;
                //((hes)driverReal).song = (byte)SongNo;
                //if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }, 0)) return false;
                //if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }, 0)) return false;

                MDSound.MDSound.Chip chip;
                MDSound.Ootake_PSG huc = new Ootake_PSG();

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.HuC6280;
                chip.Instrument = huc;
                chip.Update = huc.Update;
                chip.Start = huc.Start;
                chip.Stop = huc.Stop;
                chip.Reset = huc.Reset;
                chip.AdditionalUpdate = ((hes)driverVirtual).AdditionalUpdate;
                chip.SamplingRate = (UInt32)common.SampleRate;
                chip.Volume = setting.balance.APUVolume;
                chip.Clock = 3579545;
                chip.Option = null;
                lstChips.Add(chip);
                ((hes)driverVirtual).c6280 = chip;

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

                ((hes)driverVirtual).song = (byte)SongNo;
                ((hes)driverReal).song = (byte)SongNo;
                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
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

        public static bool sidPlay(Setting setting)
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

                chipRegister.initChipRegister();


                trdClosed = false;
                trdMain = new Thread(new ThreadStart(trdVgmRealFunction));
                trdMain.Priority = ThreadPriority.Highest;
                trdMain.IsBackground = true;
                trdMain.Name = "trdVgmReal";
                trdMain.Start();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriSID = 1;

                MasterVolume = setting.balance.MasterVolume;

                ((Driver.SID.sid)driverVirtual).song = (byte)SongNo+1;
                ((Driver.SID.sid)driverReal).song = (byte)SongNo+1;
                if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;

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

                List<enmUseChip> usechip = new List<enmUseChip>();
                usechip.Add(enmUseChip.Unuse);
                if (setting.YM2612Type.UseScci)
                {
                    if (setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip.Add(enmUseChip.YM2612Ch6);
                    }
                }
                else
                {
                    usechip.Add(enmUseChip.YM2612);
                    usechip.Add(enmUseChip.YM2612Ch6);
                }
                if (!setting.SN76489Type.UseScci)
                {
                    usechip.Add(enmUseChip.SN76489);
                }
                usechip.Add(enmUseChip.RF5C164);
                usechip.Add(enmUseChip.PWM);

                if (!driverVirtual.init(vgmBuf
                    , chipRegister
                    , enmModel.VirtualModel
                    , usechip.ToArray()
                    , (uint)(common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000)))
                    return false;

                usechip.Clear();
                usechip.Add(enmUseChip.Unuse);
                if (setting.YM2612Type.UseScci)
                {
                    usechip.Add(enmUseChip.YM2612);
                    if (!setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip.Add(enmUseChip.YM2612Ch6);
                    }
                }

                if (!driverReal.init(vgmBuf
                    , chipRegister
                    , enmModel.RealModel
                    , usechip.ToArray()
                    , (uint)(common.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(common.SampleRate * setting.outputDevice.WaitTime / 1000)))
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

                chipLED = new ChipLEDs();

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.SN76489Volume;
                        chip.Clock = ((vgm)driverVirtual).SN76489ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriDCSG = 1;
                        else chipLED.SecDCSG = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2612Volume;
                        chip.Clock = ((vgm)driverVirtual).YM2612ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci) ? 0x1 : 0x2;
                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci && setting.YM2612Type.OnlyPCMEmulation) ? 0x2 : 0x0;

                        if (i == 0) chipLED.PriOPN2 = 1;
                        else chipLED.SecOPN2 = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.RF5C164Volume;
                        chip.Clock = ((vgm)driverVirtual).RF5C164ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriRF5C = 1;
                        else chipLED.SecRF5C = 1;

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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.PWMVolume;
                    chip.Clock = ((vgm)driverVirtual).PWMClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriPWM = 1;

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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.C140Volume;
                    chip.Clock = ((vgm)driverVirtual).C140ClockValue;
                    chip.Option = new object[1] { ((vgm)driverVirtual).C140Type };

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriC140 = 1;

                    lstChips.Add(chip);
                }

                if (((vgm)driverVirtual).MultiPCMClockValue != 0)
                {
                    MDSound.multipcm multipcm = new MDSound.multipcm();
                    for (int i = 0; i < (((vgm)driverVirtual).MultiPCMDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.MultiPCM;
                        chip.ID = (byte)i;
                        chip.Instrument = multipcm;
                        chip.Update = multipcm.Update;
                        chip.Start = multipcm.Start;
                        chip.Stop = multipcm.Stop;
                        chip.Reset = multipcm.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.MultiPCMVolume;
                        chip.Clock = ((vgm)driverVirtual).MultiPCMClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriMPCM = 1;
                        else chipLED.SecMPCM = 1;

                        lstChips.Add(chip);
                    }
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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.OKIM6258Volume;
                    chip.Clock = ((vgm)driverVirtual).OKIM6258ClockValue;
                    chip.Option = new object[1] { (int)((vgm)driverVirtual).OKIM6258Type };
                    //chip.Option = new object[1] { 6 };
                    okim6258.okim6258_set_srchg_cb(0, ChangeChipSampleRate, chip);

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriOKI5 = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.OKIM6295Volume;
                        chip.Clock = ((vgm)driverVirtual).OKIM6295ClockValue;
                        chip.Option = null;
                        okim6295.okim6295_set_srchg_cb(0, ChangeChipSampleRate, chip);

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOKI9 = 1;
                        else chipLED.SecOKI9 = 1;

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
                    chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.SEGAPCMVolume;
                    chip.Clock = ((vgm)driverVirtual).SEGAPCMClockValue;
                    chip.Option = new object[1] { ((vgm)driverVirtual).SEGAPCMInterface };

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriSPCM = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2608Volume;
                        chip.Clock = ((vgm)driverVirtual).YM2608ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPNA = 1;
                        else chipLED.SecOPNA = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2151Volume;
                        chip.Clock = ((vgm)driverVirtual).YM2151ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPM = 1;
                        else chipLED.SecOPM = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2203Volume;
                        chip.Clock = ((vgm)driverVirtual).YM2203ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPN = 1;
                        else chipLED.SecOPN = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2610Volume;
                        chip.Clock = ((vgm)driverVirtual).YM2610ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPNB = 1;
                        else chipLED.SecOPNB = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YMF262ClockValue != 0)
                {
                    MDSound.ymf262 ymf262 = new MDSound.ymf262();
                    for (int i = 0; i < (((vgm)driverVirtual).YMF262DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF262;
                        chip.ID = (byte)i;
                        chip.Instrument = ymf262;
                        chip.Update = ymf262.Update;
                        chip.Start = ymf262.Start;
                        chip.Stop = ymf262.Stop;
                        chip.Reset = ymf262.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YMF262Volume;
                        chip.Clock = ((vgm)driverVirtual).YMF262ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPL3 = 1;
                        else chipLED.SecOPL3 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YMF271ClockValue != 0)
                {
                    MDSound.ymf271 ymf271 = new MDSound.ymf271();
                    for (int i = 0; i < (((vgm)driverVirtual).YMF271DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF271;
                        chip.ID = (byte)i;
                        chip.Instrument = ymf271;
                        chip.Update = ymf271.Update;
                        chip.Start = ymf271.Start;
                        chip.Stop = ymf271.Stop;
                        chip.Reset = ymf271.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YMF271Volume;
                        chip.Clock = ((vgm)driverVirtual).YMF271ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPX = 1;
                        else chipLED.SecOPX = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YMF278BClockValue != 0)
                {
                    MDSound.ymf278b ymf278b = new MDSound.ymf278b();
                    for (int i = 0; i < (((vgm)driverVirtual).YMF278BDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
                        chip.ID = (byte)i;
                        chip.Instrument = ymf278b;
                        chip.Update = ymf278b.Update;
                        chip.Start = ymf278b.Start;
                        chip.Stop = ymf278b.Stop;
                        chip.Reset = ymf278b.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume =  setting.balance.YMF278BVolume;
                        chip.Clock = ((vgm)driverVirtual).YMF278BClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPL4 = 1;
                        else chipLED.SecOPL4 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).YMZ280BClockValue != 0)
                {
                    MDSound.ymz280b ymz280b = new MDSound.ymz280b();
                    for (int i = 0; i < (((vgm)driverVirtual).YMZ280BDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMZ280B;
                        chip.ID = (byte)i;
                        chip.Instrument = ymz280b;
                        chip.Update = ymz280b.Update;
                        chip.Start = ymz280b.Start;
                        chip.Stop = ymz280b.Stop;
                        chip.Reset = ymz280b.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YMZ280BVolume;
                        chip.Clock = ((vgm)driverVirtual).YMZ280BClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriYMZ = 1;
                        else chipLED.SecYMZ = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.AY8910Volume;
                        chip.Clock = (((vgm)driverVirtual).AY8910ClockValue & 0x7fffffff) / 2;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriAY10 = 1;
                        else chipLED.SecAY10 = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.YM2413Volume;
                        chip.Clock = (((vgm)driverVirtual).YM2413ClockValue & 0x7fffffff);
                        chip.Option = null;
                        
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPLL = 1;
                        else chipLED.SecOPLL = 1;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.HuC6280Volume;
                        chip.Clock = (((vgm)driverVirtual).HuC6280ClockValue & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriHuC = 1;
                        else chipLED.SecHuC = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).QSoundClockValue != 0)
                {
                    MDSound.qsound qsound = new MDSound.qsound();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.QSound;
                        chip.ID = (byte)0;
                        chip.Instrument = qsound;
                        chip.Update = qsound.Update;
                        chip.Start = qsound.Start;
                        chip.Stop = qsound.Stop;
                        chip.Reset = qsound.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                    chip.Volume = setting.balance.QSoundVolume;
                    chip.Clock = (((vgm)driverVirtual).QSoundClockValue);// & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        //if (i == 0) chipLED.PriHuC = 1;
                        //else chipLED.SecHuC = 1;
                    chipLED.PriQsnd = 1;

                    lstChips.Add(chip);
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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.C352Volume;
                        chip.Clock = (((vgm)driverVirtual).C352ClockValue & 0x7fffffff);
                        chip.Option = new object[1] { (((vgm)driverVirtual).C352ClockDivider) };
                        c352.c352_set_options((byte)(((vgm)driverVirtual).C352ClockValue >> 31));
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriC352 = 1;
                        else chipLED.SecC352 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).GA20ClockValue != 0)
                {
                    MDSound.iremga20 ga20 = new iremga20();
                    for (int i = 0; i < (((vgm)driverVirtual).GA20DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.GA20;
                        chip.ID = (byte)i;
                        chip.Instrument = ga20;
                        chip.Update = ga20.Update;
                        chip.Start = ga20.Start;
                        chip.Stop = ga20.Stop;
                        chip.Reset = ga20.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.GA20Volume;
                        chip.Clock = (((vgm)driverVirtual).GA20ClockValue & 0x7fffffff);
                        chip.Option = null;
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriGA20 = 1;
                        else chipLED.SecGA20 = 1;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).K053260ClockValue != 0)
                {
                    MDSound.K053260 k053260 = new MDSound.K053260();

                    for (int i = 0; i < (((vgm)driverVirtual).K053260DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K053260;
                        chip.ID = (byte)i;
                        chip.Instrument = k053260;
                        chip.Update = k053260.Update;
                        chip.Start = k053260.Start;
                        chip.Stop = k053260.Stop;
                        chip.Reset = k053260.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = 0;// setting.balance.K053260Volume;
                        chip.Clock = ((vgm)driverVirtual).K053260ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriK053260 = 1;
                        else chipLED.SecK053260 = 1;

                        hiyorimiDeviceFlag |= 0x2;

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
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.K054539Volume;
                        chip.Clock = ((vgm)driverVirtual).K054539ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriK054539 = 1;
                        else chipLED.SecK054539 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).K051649ClockValue != 0)
                {
                    MDSound.K051649 k051649 = new MDSound.K051649();

                    for (int i = 0; i < (((vgm)driverVirtual).K051649DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K051649;
                        chip.ID = (byte)i;
                        chip.Instrument = k051649;
                        chip.Update = k051649.Update;
                        chip.Start = k051649.Start;
                        chip.Stop = k051649.Stop;
                        chip.Reset = k051649.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.K051649Volume;
                        chip.Clock = ((vgm)driverVirtual).K051649ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriK051649 = 1;
                        else chipLED.SecK051649 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).Y8950ClockValue != 0)
                {
                    MDSound.y8950 y8950 = new MDSound.y8950();

                    for (int i = 0; i < (((vgm)driverVirtual).Y8950DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.Y8950;
                        chip.ID = (byte)i;
                        chip.Instrument = y8950;
                        chip.Update = y8950.Update;
                        chip.Start = y8950.Start;
                        chip.Stop = y8950.Stop;
                        chip.Reset = y8950.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = 0;// setting.balance.Y8950Volume;
                        chip.Clock = ((vgm)driverVirtual).Y8950ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriY895 = 1;
                        else chipLED.SecY895 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).DMGClockValue != 0)
                {
                    MDSound.gb dmg = new MDSound.gb();

                    for (int i = 0; i < (((vgm)driverVirtual).DMGDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.DMG;
                        chip.ID = (byte)i;
                        chip.Instrument = dmg;
                        chip.Update = dmg.Update;
                        chip.Start = dmg.Start;
                        chip.Stop = dmg.Stop;
                        chip.Reset = dmg.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.DMGVolume;
                        chip.Clock = ((vgm)driverVirtual).DMGClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriDMG = 1;
                        else chipLED.SecDMG = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (((vgm)driverVirtual).NESClockValue != 0)
                {
                    MDSound.nes_intf nes = new MDSound.nes_intf();

                    for (int i = 0; i < (((vgm)driverVirtual).NESDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.Nes;
                        chip.ID = (byte)i;
                        chip.Instrument = nes;
                        chip.Update = nes.Update;
                        chip.Start = nes.Start;
                        chip.Stop = nes.Stop;
                        chip.Reset = nes.Reset;
                        chip.SamplingRate = (UInt32)common.SampleRate;
                        chip.Volume = setting.balance.APUVolume;
                        chip.Clock = ((vgm)driverVirtual).NESClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriNES = 1;
                        else chipLED.SecNES = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                    }
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)common.SampleRate, samplingBuffer, lstChips.ToArray());

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
            if (CAA.SamplingRate < common.SampleRate)//SampleRate)
                CAA.Resampler = 0x01;
            else if (CAA.SamplingRate == common.SampleRate)//SampleRate)
                CAA.Resampler = 0x02;
            else if (CAA.SamplingRate > common.SampleRate)//SampleRate)
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

                //DEBUG
                //vstparse();
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

                //midi outをリリース
                if (midiOuts.Count > 0)
                {
                    for (int i = 0; i < midiOuts.Count; i++)
                    {
                        if (midiOuts[i] != null)
                        {
                            midiOuts[i].Reset();
                            midiOuts[i].Close();
                            midiOuts[i] = null;
                        }
                    }
                    midiOuts.Clear();
                    midiOutsType.Clear();
                }

                if (vstMidiOuts.Count > 0)
                {
                    vstMidiOuts.Clear();
                    vstMidiOutsType.Clear();
                }

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
                    vi.effectName = vstPlugins[i].effectName;
                    vi.power = vstPlugins[i].power;
                    vi.location = vstPlugins[i].location;
                    vi.param = vstPlugins[i].param;

                    if (!vstPlugins[i].isInstrument) vstlst.Add(vi);
                }
                setting.vst.VSTInfo = vstlst.ToArray();


                for (int i = 0; i < vstPluginsInst.Count; i++)
                {
                    try
                    {
                        vstPluginsInst[i].vstPluginsForm.timer1.Enabled = false;
                        vstPluginsInst[i].location = vstPluginsInst[i].vstPluginsForm.Location;
                        vstPluginsInst[i].vstPluginsForm.Close();
                    }
                    catch { }

                    try
                    {
                        if (vstPluginsInst[i].vstPlugins != null)
                        {
                            vstPluginsInst[i].vstPlugins.PluginCommandStub.EditorClose();
                            vstPluginsInst[i].vstPlugins.PluginCommandStub.StopProcess();
                            vstPluginsInst[i].vstPlugins.PluginCommandStub.MainsChanged(false);
                            int pc = vstPluginsInst[i].vstPlugins.PluginInfo.ParameterCount;
                            List<float> plst = new List<float>();
                            for (int p = 0; p < pc; p++)
                            {
                                float v = vstPluginsInst[i].vstPlugins.PluginCommandStub.GetParameter(p);
                                plst.Add(v);
                            }
                            vstPluginsInst[i].param = plst.ToArray();
                            vstPluginsInst[i].vstPlugins.Dispose();
                        }
                    }
                    catch { }

                    vstInfo vi = new vstInfo();
                    vi.editor = vstPluginsInst[i].editor;
                    vi.fileName = vstPluginsInst[i].fileName;
                    vi.key = vstPluginsInst[i].key;
                    vi.effectName = vstPluginsInst[i].effectName;
                    vi.power = vstPluginsInst[i].power;
                    vi.location = vstPluginsInst[i].location;
                    vi.param = vstPluginsInst[i].param;

                }

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
            if (driverVirtual == null && driverReal == null) return -1;

            if (driverVirtual == null) return driverReal.Counter;
            if (driverReal==null) return driverVirtual.Counter;

            return driverVirtual.Counter > driverReal.Counter ? driverVirtual.Counter : driverReal.Counter;
        }

        public static long GetTotalCounter()
        {
            if (driverVirtual == null) return -1;

            return driverVirtual.TotalCounter;
        }

        public static long GetDriverCounter()
        {
            if (driverVirtual == null && driverReal == null) return -1;


            if (driverVirtual == null)
            {
                if (driverReal is NRTDRV) return ((NRTDRV)driverReal).work.TOTALCOUNT;
                else return 0;
            }
            if (driverReal == null)
            {
                if (driverVirtual is NRTDRV) return ((NRTDRV)driverVirtual).work.TOTALCOUNT;
                else return 0;
            }

            if (driverVirtual is NRTDRV && driverReal is NRTDRV)
            {
                return ((NRTDRV)driverVirtual).work.TOTALCOUNT > ((NRTDRV)driverReal).work.TOTALCOUNT ? ((NRTDRV)driverVirtual).work.TOTALCOUNT : ((NRTDRV)driverReal).work.TOTALCOUNT;
            }
            else
            {
                return 0;
            }
        }

        public static long GetLoopCounter()
        {
            if (driverVirtual == null) return -1;

            return driverVirtual.LoopCounter;
        }

        public static byte[] GetChipStatus()
        {
            chips[0] = chipRegister.chipLED.PriOPN;
            chipRegister.chipLED.PriOPN = chipLED.PriOPN;
            chips[1] = chipRegister.chipLED.PriOPN2;
            chipRegister.chipLED.PriOPN2 = chipLED.PriOPN2;
            chips[2] = chipRegister.chipLED.PriOPNA;
            chipRegister.chipLED.PriOPNA = chipLED.PriOPNA;
            chips[3] = chipRegister.chipLED.PriOPNB;
            chipRegister.chipLED.PriOPNB = chipLED.PriOPNB;

            chips[4] = chipRegister.chipLED.PriOPM;
            chipRegister.chipLED.PriOPM = chipLED.PriOPM;
            chips[5] = chipRegister.chipLED.PriDCSG;
            chipRegister.chipLED.PriDCSG = chipLED.PriDCSG;
            chips[6] = chipRegister.chipLED.PriRF5C;
            chipRegister.chipLED.PriRF5C = chipLED.PriRF5C;
            chips[7] = chipRegister.chipLED.PriPWM;
            chipRegister.chipLED.PriPWM = chipLED.PriPWM;

            chips[8] = chipRegister.chipLED.PriOKI5;
            chipRegister.chipLED.PriOKI5 = chipLED.PriOKI5;
            chips[9] = chipRegister.chipLED.PriOKI9;
            chipRegister.chipLED.PriOKI9 = chipLED.PriOKI9;
            chips[10] = chipRegister.chipLED.PriC140;
            chipRegister.chipLED.PriC140 = chipLED.PriC140;
            chips[11] = chipRegister.chipLED.PriSPCM;
            chipRegister.chipLED.PriSPCM = chipLED.PriSPCM;

            chips[12] = chipRegister.chipLED.PriAY10;
            chipRegister.chipLED.PriAY10 = chipLED.PriAY10;
            chips[13] = chipRegister.chipLED.PriOPLL;
            chipRegister.chipLED.PriOPLL = chipLED.PriOPLL;
            chips[14] = chipRegister.chipLED.PriHuC;
            chipRegister.chipLED.PriHuC = chipLED.PriHuC;
            chips[15] = chipRegister.chipLED.PriC352;
            chipRegister.chipLED.PriC352 = chipLED.PriC352;
            chips[16] = chipRegister.chipLED.PriK054539;
            chipRegister.chipLED.PriK054539 = chipLED.PriK054539;


            chips[128 + 0] = chipRegister.chipLED.SecOPN;
            chipRegister.chipLED.SecOPN = chipLED.SecOPN;
            chips[128 + 1] = chipRegister.chipLED.SecOPN2;
            chipRegister.chipLED.SecOPN2 = chipLED.SecOPN2;
            chips[128 + 2] = chipRegister.chipLED.SecOPNA;
            chipRegister.chipLED.SecOPNA = chipLED.SecOPNA;
            chips[128 + 3] = chipRegister.chipLED.SecOPNB;
            chipRegister.chipLED.SecOPNB = chipLED.SecOPNB;

            chips[128 + 4] = chipRegister.chipLED.SecOPM;
            chipRegister.chipLED.SecOPM = chipLED.SecOPM;
            chips[128 + 5] = chipRegister.chipLED.SecDCSG;
            chipRegister.chipLED.SecDCSG = chipLED.SecDCSG;
            chips[128 + 6] = chipRegister.chipLED.SecRF5C;
            chipRegister.chipLED.SecRF5C = chipLED.SecRF5C;
            chips[128 + 7] = chipRegister.chipLED.SecPWM;
            chipRegister.chipLED.SecPWM = chipLED.SecPWM;

            chips[128 + 8] = chipRegister.chipLED.SecOKI5;
            chipRegister.chipLED.SecOKI5 = chipLED.SecOKI5;
            chips[128 + 9] = chipRegister.chipLED.SecOKI9;
            chipRegister.chipLED.SecOKI9 = chipLED.SecOKI9;
            chips[128 + 10] = chipRegister.chipLED.SecC140;
            chipRegister.chipLED.SecC140 = chipLED.SecC140;
            chips[128 + 11] = chipRegister.chipLED.SecSPCM;
            chipRegister.chipLED.SecSPCM = chipLED.SecSPCM;

            chips[128 + 12] = chipRegister.chipLED.SecAY10;
            chipRegister.chipLED.SecAY10 = chipLED.SecAY10;
            chips[128 + 13] = chipRegister.chipLED.SecOPLL;
            chipRegister.chipLED.SecOPLL = chipLED.SecOPLL;
            chips[128 + 14] = chipRegister.chipLED.SecHuC;
            chipRegister.chipLED.SecHuC = chipLED.SecHuC;
            chips[128+15] = chipRegister.chipLED.SecC352;
            chipRegister.chipLED.SecC352 = chipLED.SecC352;
            chips[128+16] = chipRegister.chipLED.SecK054539;
            chipRegister.chipLED.SecK054539 = chipLED.SecK054539;


            return chips;
        }

        public static void updateVol()
        {
            chipRegister.updateVol();
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

            v = driverVirtual == null ? true : driverVirtual.Stopped;
            r = driverReal == null ? true : driverReal.Stopped;
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
            double step = 1 / (double)common.SampleRate;

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
                            chipRegister.resetAllMIDIout();
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
                                chipRegister.resetAllMIDIout();
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
                        long d = common.SampleRate * (setting.LatencySCCI - common.SampleRate * setting.LatencyEmulation) / 1000;
                        long l = getLatency() / 4;

                        int m = 0;
                        if (d >= 0)
                        {
                            if (v >= d - l && v <= d + l) m = 0;
                            else m = (v + d > l) ? 1 : 2;
                        }
                        else
                        {
                            d = Math.Abs(common.SampleRate * ((uint)setting.LatencyEmulation - (uint)setting.LatencySCCI) / 1000);
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

            if (vstPlugins.Count > 0 || vstPluginsInst.Count > 0) VST_Update(buffer, offset, sampleCount);

            return cnt1;
        }

        private static void VST_Update(short[] buffer, int offset, int sampleCount)
        {
            if (buffer == null || buffer.Length < 1 || sampleCount == 0) return;

            try
            {
                //if (trdStopped) return;

                int blockSize = sampleCount / 2;

                for (int i = 0; i < vstPluginsInst.Count; i++)
                {
                    vstInfo2 info2 = vstPluginsInst[i];
                    VstPluginContext PluginContext = info2.vstPlugins;
                    if (PluginContext == null) continue;
                    if (PluginContext.PluginCommandStub == null) continue;


                    int inputCount = info2.vstPlugins.PluginInfo.AudioInputCount;
                    int outputCount = info2.vstPlugins.PluginInfo.AudioOutputCount;

                    using (VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize))
                    {
                        using (VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize))
                        {
                            VstAudioBuffer[] inputBuffers = inputMgr.ToArray();
                            VstAudioBuffer[] outputBuffers = outputMgr.ToArray();

                            if (inputCount != 0)
                            {
                                inputMgr.ClearBuffer(inputBuffers[0]);
                                inputMgr.ClearBuffer(inputBuffers[1]);

                                for (int j = 0; j < blockSize; j++)
                                {
                                    // generate a value between -1.0 and 1.0
                                    inputBuffers[0][j] = buffer[j * 2 + offset + 0] / (float)short.MaxValue;
                                    inputBuffers[1][j] = buffer[j * 2 + offset + 1] / (float)short.MaxValue;
                                }
                            }

                            outputMgr.ClearBuffer(outputBuffers[0]);
                            outputMgr.ClearBuffer(outputBuffers[1]);

                            PluginContext.PluginCommandStub.ProcessEvents(info2.lstEvent.ToArray());
                            info2.lstEvent.Clear();


                            PluginContext.PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);

                            for (int j = 0; j < blockSize; j++)
                            {
                                // generate a value between -1.0 and 1.0
                                if (inputCount == 0)
                                {
                                    buffer[j * 2 + offset + 0] += (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] += (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                                else
                                {
                                    buffer[j * 2 + offset + 0] = (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] = (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                            }

                        }
                    }
                }

                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    vstInfo2 info2 = vstPlugins[i];
                    VstPluginContext PluginContext = info2.vstPlugins;
                    if (PluginContext == null) continue;
                    if (PluginContext.PluginCommandStub == null) continue;


                    int inputCount = info2.vstPlugins.PluginInfo.AudioInputCount;
                    int outputCount = info2.vstPlugins.PluginInfo.AudioOutputCount;

                    using (VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize))
                    {
                        using (VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize))
                        {
                            VstAudioBuffer[] inputBuffers = inputMgr.ToArray();
                            VstAudioBuffer[] outputBuffers = outputMgr.ToArray();

                            if (inputCount != 0)
                            {
                                inputMgr.ClearBuffer(inputBuffers[0]);
                                inputMgr.ClearBuffer(inputBuffers[1]);

                                for (int j = 0; j < blockSize; j++)
                                {
                                    // generate a value between -1.0 and 1.0
                                    inputBuffers[0][j] = buffer[j * 2 + offset + 0] / (float)short.MaxValue;
                                    inputBuffers[1][j] = buffer[j * 2 + offset + 1] / (float)short.MaxValue;
                                }
                            }

                            outputMgr.ClearBuffer(outputBuffers[0]);
                            outputMgr.ClearBuffer(outputBuffers[1]);

                            PluginContext.PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);

                            for (int j = 0; j < blockSize; j++)
                            {
                                // generate a value between -1.0 and 1.0
                                if (inputCount == 0)
                                {
                                    buffer[j * 2 + offset + 0] += (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] += (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                                else
                                {
                                    buffer[j * 2 + offset + 0] = (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] = (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                            }

                        }
                    }
                }

            }
            catch { }
        }

        private static int trdVgmVirtualMainFunction(short[] buffer, int offset, int sampleCount)
        {
            if (buffer == null || buffer.Length < 1 || sampleCount == 0) return 0;

            try
            {
                stwh.Reset();stwh.Start();

                int i;
                int cnt = 0;

                if (driverVirtual is nsf)
                {
                    if (Stopped || Paused) return mds.Update(buffer, offset, sampleCount, null);

                    driverVirtual.vstDelta = 0;
                    cnt = (Int32)((nsf)driverVirtual).Render(buffer, (UInt32)sampleCount / 2, offset) * 2;
                }
                else if(driverVirtual is Driver.SID.sid)
                {
                    if (Stopped || Paused) return mds.Update(buffer, offset, sampleCount, null);

                    driverVirtual.vstDelta = 0;
                    cnt = (Int32)((Driver.SID.sid)driverVirtual).Render(buffer, (UInt32)sampleCount );
                }
                else
                {
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

                    driverVirtual.vstDelta = 0;
                    cnt = mds.Update(buffer, offset, sampleCount, driverVirtual.oneFrameProc);
                }

                //for (i = 0; i < sampleCount; i++)
                //{
                //    Console.WriteLine("{0}:{1}:{2}", offset,i,buffer[offset + i]);
                //}

                for (i = 0; i < sampleCount; i++)
                {
                    int mul = (int)(16384.0 * Math.Pow(10.0, MasterVolume / 40.0));
                    buffer[offset + i] = (short)((Limit(buffer[offset + i], 0x7fff, -0x8000) * mul) >> 14);
                }

                visVolume.master = buffer[offset];

                int[][][] vol = mds.getYM2151VisVolume();

                if (vol != null) visVolume.ym2151 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getYM2203VisVolume();
                if (vol != null) visVolume.ym2203 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
                if (vol != null) visVolume.ym2203FM = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
                if (vol != null) visVolume.ym2203SSG = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);

                vol = mds.getYM2413VisVolume();
                if (vol != null) visVolume.ym2413 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getYM2608VisVolume();
                if (vol != null) visVolume.ym2608 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
                if (vol != null) visVolume.ym2608FM = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
                if (vol != null) visVolume.ym2608SSG = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
                if (vol != null) visVolume.ym2608Rtm = (short)getMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
                if (vol != null) visVolume.ym2608APCM = (short)getMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);

                vol = mds.getYM2610VisVolume();
                if (vol != null) visVolume.ym2610 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
                if (vol != null) visVolume.ym2610FM = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
                if (vol != null) visVolume.ym2610SSG = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
                if (vol != null) visVolume.ym2610APCMA = (short)getMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
                if (vol != null) visVolume.ym2610APCMB = (short)getMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);

                vol = mds.getYM2612VisVolume();
                if (vol != null) visVolume.ym2612 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getAY8910VisVolume();
                if (vol != null) visVolume.ay8910 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getSN76489VisVolume();
                if (vol != null) visVolume.sn76489 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getHuC6280VisVolume();
                if (vol != null) visVolume.huc6280 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getRF5C164VisVolume();
                if (vol != null) visVolume.rf5c164 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getPWMVisVolume();
                if (vol != null) visVolume.pwm = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getOKIM6258VisVolume();
                if (vol != null) visVolume.okim6258 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getOKIM6295VisVolume();
                if (vol != null) visVolume.okim6295 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getC140VisVolume();
                if (vol != null) visVolume.c140 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getSegaPCMVisVolume();
                if (vol != null) visVolume.segaPCM = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getC352VisVolume();
                if (vol != null) visVolume.c352 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getK054539VisVolume();
                if (vol != null) visVolume.k054539 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getNESVisVolume();
                if (vol != null) visVolume.APU = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getDMCVisVolume();
                if (vol != null) visVolume.DMC = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getFDSVisVolume();
                if (vol != null) visVolume.FDS = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getMMC5VisVolume();
                if (vol != null) visVolume.MMC5 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getN160VisVolume();
                if (vol != null) visVolume.N160 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getVRC6VisVolume();
                if (vol != null) visVolume.VRC6 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getVRC7VisVolume();
                if (vol != null) visVolume.VRC7 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

                vol = mds.getFME7VisVolume();
                if (vol != null) visVolume.FME7 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

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
                        mds = new MDSound.MDSound((UInt32)common.SampleRate, samplingBuffer, null);
                    else
                        mds.Init((UInt32)common.SampleRate, samplingBuffer, null);

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

        public static int[][] GetYM2608Register(int chipID)
        {
            return chipRegister.fmRegisterYM2608[chipID];
        }

        public static int[][] GetYM2610Register(int chipID)
        {
            return chipRegister.fmRegisterYM2610[chipID];
        }

        public static int[][] GetYMF278BRegister(int chipID)
        {
            return chipRegister.fmRegisterYMF278B[chipID];
        }

        public static int[] GetMoonDriverPCMKeyOn()
        {
            if(driverVirtual is Driver.MoonDriver.MoonDriver)
            {
                if (driverVirtual != null) return ((Driver.MoonDriver.MoonDriver)driverVirtual).GetPCMKeyOn();
            }
            return null;
        }

        public static int[] GetPSGRegister(int chipID)
        {
            return chipRegister.sn76489Register[chipID];
        }

        public static int GetPSGRegisterGGPanning(int chipID)
        {
            return chipRegister.sn76489RegisterGGPan[chipID];
        }

        public static int[] GetAY8910Register(int chipID)
        {
            return chipRegister.psgRegisterAY8910[chipID];
        }

        public static Ootake_PSG.huc6280_state GetHuC6280Register(int chipID)
        {
            return mds.ReadHuC6280Status(chipID);
        }

        public static MIDIParam GetMIDIInfos(int chipID)
        {
            return chipRegister.midiParams[chipID];
        }

        public static scd_pcm.pcm_chip_ GetRf5c164Register(int chipID)
        {
            return mds.ReadRf5c164Register(chipID);
        }

        public static c140.c140_state GetC140Register(int chipID)
        {
            return mds.ReadC140Register(chipID);
        }

        public static okim6258.okim6258_state GetOKIM6258Register(int chipID)
        {
            return mds.ReadOKIM6258Status(chipID);
        }

        public static segapcm.segapcm_state GetSegaPCMRegister(int chipID)
        {
            return mds.ReadSegaPCMStatus(chipID);
        }

        public static byte[] GetAPURegister(int chipID)
        {
            byte[] reg = null;

            //nsf向け
            if (chipRegister == null) reg = null;
            else if (chipRegister.nes_apu == null) reg = null;
            else if (chipRegister.nes_apu.chip == null) reg = null;
            else if (chipID == 1) reg = null;
            else reg = chipRegister.nes_apu.chip.reg;

            //vgm向け
            if (reg == null) reg = chipRegister.getNESRegister(chipID, enmModel.VirtualModel);

            return reg;
        }

        public static byte[] GetDMCRegister(int chipID)
        {
            byte[] reg = null;

            //nsf向け
            if (chipRegister == null) reg = null;
            else if (chipRegister.nes_apu == null) reg = null;
            else if (chipRegister.nes_apu.chip == null) reg = null;
            else if (chipID == 1) reg = null;
            else reg = chipRegister.nes_dmc.chip.reg;

            //vgm向け
            //if (reg == null) reg = chipRegister.getNESRegister(chipID, enmModel.VirtualModel);

            return reg;
        }

        public static MDSound.np.np_nes_fds.NES_FDS GetFDSRegister(int chipID)
        {
            MDSound.np.np_nes_fds.NES_FDS reg = null;

            //nsf向け
            if (chipRegister == null) reg = null;
            else if (chipRegister.nes_apu == null) reg = null;
            else if (chipRegister.nes_apu.chip == null) reg = null;
            else if (chipID == 1) reg = null;
            else reg = chipRegister.nes_fds.chip;

            //vgm向け
            if (reg == null) reg = chipRegister.getFDSRegister(chipID, enmModel.VirtualModel);

            return reg;
        }

        private static byte[] mmc5regs = new byte[10];
        public static byte[] GetMMC5Register(int chipID)
        {
            //nsf向け
            if (chipRegister == null) return null;
            else if (chipRegister.nes_mmc5 == null) return null;
            else if (chipID == 1) return null;

            uint dat = 0;
            for (uint adr = 0x5000; adr < 0x5008; adr++)
            {
                dat=0;
                chipRegister.nes_mmc5.Read(adr, ref dat);
                mmc5regs[adr & 0x7] = (byte)dat;
            }

            chipRegister.nes_mmc5.Read(0x5010, ref dat);
            mmc5regs[8] = (byte)(chipRegister.nes_mmc5.pcm_mode ? 1 : 0);
            mmc5regs[9] = chipRegister.nes_mmc5.pcm;


            return mmc5regs;
        }

        public static int[] GetFMKeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2612[chipID];
        }

        public static int[] GetYM2151KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2151[chipID];
        }

        public static bool GetOKIM6258KeyOn(int chipID)
        {
            return chipRegister.okim6258Keyon[chipID];
        }

        public static void ResetOKIM6258KeyOn(int chipID)
        {
            chipRegister.okim6258Keyon[chipID] = false;
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

        public static int getYM2413RyhthmKeyON(int chipID)
        {
            return chipRegister.getYM2413RyhthmKeyON(chipID);
        }

        public static void resetYM2413RyhthmKeyON(int chipID)
        {
            chipRegister.resetYM2413RyhthmKeyON(chipID);
        }

        public static int getYMF278BFMKeyON(int chipID)
        {
            return chipRegister.getYMF278BFMKeyON(chipID);
        }

        public static void resetYMF278BFMKeyON(int chipID)
        {
            chipRegister.resetYMF278BFMKeyON(chipID);
        }

        public static int getYMF278BRyhthmKeyON(int chipID)
        {
            return chipRegister.getYMF278BRyhthmKeyON(chipID);
        }

        public static void resetYMF278BRyhthmKeyON(int chipID)
        {
            chipRegister.resetYMF278BRyhthmKeyON(chipID);
        }

        public static int[] getYMF278BPCMKeyON(int chipID)
        {
            return chipRegister.getYMF278BPCMKeyON(chipID);
        }

        public static void resetYMF278BPCMKeyON(int chipID)
        {
            chipRegister.resetYMF278BPCMKeyON(chipID);
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

        public static void SetK051649Volume(int volume)
        {
            try
            {
                mds.SetVolumeK051649(volume);
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

        public static void SetQSoundVolume(int volume)
        {
            try
            {
                mds.SetVolumeQSound(volume);
            }
            catch { }
        }

        public static void SetDMGVolume(int volume)
        {
            try
            {
                mds.SetVolumeDMG(volume);
            }
            catch { }
        }

        public static void SetGA20Volume(int volume)
        {
            try
            {
                mds.SetVolumeGA20(volume);
            }
            catch { }
        }

        public static void SetYMZ280BVolume(int volume)
        {
            try
            {
                mds.SetVolumeYMZ280B(volume);
            }
            catch { }
        }

        public static void SetYMF271Volume(int volume)
        {
            try
            {
                mds.SetVolumeYMF271(volume);
            }
            catch { }
        }

        public static void SetYMF262Volume(int volume)
        {
            try
            {
                mds.SetVolumeYMF262(volume);
            }
            catch { }
        }

        public static void SetYMF278BVolume(int volume)
        {
            try
            {
                mds.SetVolumeYMF278B(volume);
            }
            catch { }
        }

        public static void SetMultiPCMVolume(int volume)
        {
            try
            {
                mds.SetVolumeMultiPCM(volume);
            }
            catch { }
        }



        public static void SetAPUVolume(int volume)
        {
            try
            {
                mds.SetVolumeNES(volume);
            }
            catch { }
        }

        public static void SetDMCVolume(int volume)
        {
            try
            {
                mds.SetVolumeDMC(volume);
            }
            catch { }
        }

        public static void SetFDSVolume(int volume)
        {
            try
            {
                mds.SetVolumeFDS(volume);
            }
            catch { }
        }

        public static void SetMMC5Volume(int volume)
        {
            try
            {
                mds.SetVolumeMMC5(volume);
            }
            catch { }
        }

        public static void SetN160Volume(int volume)
        {
            try
            {
                mds.SetVolumeN160(volume);
            }
            catch { }
        }

        public static void SetVRC6Volume(int volume)
        {
            try
            {
                mds.SetVolumeVRC6(volume);
            }
            catch { }
        }

        public static void SetVRC7Volume(int volume)
        {
            try
            {
                mds.SetVolumeVRC7(volume);
            }
            catch { }
        }

        public static void SetFME7Volume(int volume)
        {
            try
            {
                mds.SetVolumeFME7(volume);
            }
            catch { }
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

        //public static int[][] GetRf5c164Volume(int chipID)
        //{
            //return mds.ReadRf5c164Volume(chipID);
        //}


        public static void setSN76489Mask(int chipID, int ch)
        {
            //mds.setSN76489Mask(chipID,1 << ch);
            chipRegister.setMaskSN76489(chipID, ch, true);
        }

        public static void setRF5C164Mask(int chipID, int ch)
        {
            mds.setRf5c164Mask(chipID, ch);
        }

        public static void setYM2151Mask(int chipID, int ch)
        {
            //mds.setYM2151Mask(ch);
            chipRegister.setMaskYM2151(chipID, ch, true);
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
            mds.setAY8910Mask(chipID, 1 << ch);
        }

        public static void setHuC6280Mask(int chipID, int ch)
        {
            mds.setHuC6280Mask(chipID, 1 << ch);
        }

        public static void setOKIM6258Mask(int chipID)
        {
            chipRegister.setMaskOKIM6258(chipID, true);
        }

        public static void setNESMask(int chipID, int ch)
        {
            chipRegister.setNESMask(chipID, ch);
        }

        public static void setDMCMask(int chipID, int ch)
        {
            chipRegister.setNESMask(chipID, ch+2);
        }

        public static void setFDSMask(int chipID)
        {
            chipRegister.setFDSMask(chipID);
        }

        public static void setMMC5Mask(int chipID, int ch)
        {
            chipRegister.setMMC5Mask(chipID, ch);
        }


        public static void resetOKIM6258Mask(int chipID)
        {
            chipRegister.setMaskOKIM6258(chipID, false);
        }

        public static void resetYM2612Mask(int chipID, int ch)
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

        public static void resetRF5C164Mask(int chipID, int ch)
        {
            try
            {
                mds.resetRf5c164Mask(chipID, ch);
            }
            catch { }
        }

        public static void resetYM2151Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2151Mask(ch);
                chipRegister.setMaskYM2151(chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM2608Mask(int chipID, int ch)
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
            mds.resetC140Mask(chipID, 1 << ch);
        }

        public static void resetSegaPCMMask(int chipID, int ch)
        {
            mds.resetSegaPcmMask(chipID, 1 << ch);
        }

        public static void resetAY8910Mask(int chipID, int ch)
        {
            mds.resetAY8910Mask(chipID, 1 << ch);
        }

        public static void resetHuC6280Mask(int chipID, int ch)
        {
            mds.resetHuC6280Mask(chipID, 1 << ch);
        }

        public static void resetNESMask(int chipID, int ch)
        {
            chipRegister.resetNESMask(chipID, ch);
        }

        public static void resetDMCMask(int chipID, int ch)
        {
            chipRegister.resetNESMask(chipID, ch+2);
        }

        public static void resetFDSMask(int chipID)
        {
            chipRegister.resetFDSMask(chipID);
        }

        public static void resetMMC5Mask(int chipID, int ch)
        {
            chipRegister.resetMMC5Mask(chipID, ch);
        }

    }


}
