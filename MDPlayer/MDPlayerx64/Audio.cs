using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using MDSound;
using System.IO;
using System.IO.Compression;
using MDPlayer.form;
using musicDriverInterface;
using MDSound.np.chip;
using NAudio.Wave;
using MDPlayer.Driver.SID;
using System.Reflection;
using static MDPlayer.PlayList;

namespace MDPlayer
{
    public class Audio
    {


        public static frmMain frmMain = null;
        public static vstMng vstMng = new();
        public static Setting setting = null;

        public static int clockAY8910 = 1789750;
        public static int clockS5B = 1789772;
        public static int clockK051649 = 1500000;
        public static int clockK053260 = 3579545;
        public static int clockC140 = 21390;
        public static int clockPPZ8 = 44100;// setting.outputDevice.SampleRate;
        public static int clockC352 = 24192000;
        public static int clockFDS = 0;
        public static int clockHuC6280 = 0;
        public static int clockRF5C164 = 0;
        public static int clockMMC5 = 0;
        public static int clockNESDMC = 0;
        public static int clockOKIM6258 = 0;
        public static int clockOKIM6295 = 0;
        public static int clockSegaPCM = 0;
        public static int clockSN76489 = 0;
        public static int clockYM2151 = 0;
        public static int clockYM2203 = 0;
        public static int clockYM2413 = 0;
        public static int clockYM2608 = 0;
        public static int clockYM2609 = 0;
        public static int clockYM2610 = 0;
        public static int clockYM2612 = 0;
        public static int clockYMF278B = 0;

        private static object lockObj = new();
        private static bool _fatalError = false;
        public static bool FatalError
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

        private static RSoundChip[] scYM2612 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scSN76489 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2151 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2151_4M = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2608 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2203 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scAY8910 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scK051649 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2413 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM3526 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM3812 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYMF262 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2610 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2610EA = new RSoundChip[2] { null, null };
        private static RSoundChip[] scYM2610EB = new RSoundChip[2] { null, null };
        private static RSoundChip[] scC140 = new RSoundChip[2] { null, null };
        private static RSoundChip[] scSEGAPCM = new RSoundChip[2] { null, null };
        private static RealChip realChip;
        private static ChipRegister chipRegister = null;
        public static HashSet<EnmChip> useChip = new();


        private static Thread trdMain = null;
        public static bool trdClosed = false;
        private static bool _trdStopped = true;
        public static bool TrdStopped
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

        internal static object GetSIDRegister(int chipID)
        {
            return chipRegister.getSIDRegister(chipID);
        }
        
        internal static sid GetCurrentSIDContext()
        {
            return chipRegister.SID;
        }

        internal static okim6295.okim6295Info GetOKIM6295Info(int chipID)
        {
            return chipRegister.GetOKIM6295Info(chipID);
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

        public static baseDriver driverVirtual = null;
        public static baseDriver driverReal = null;

        private static bool oneTimeReset = false;
        private static int hiyorimiEven = 0;
        private static bool hiyorimiNecessary = false;

        public static ChipLEDs chipLED = new();
        public static VisVolume visVolume = new();

        private static int MasterVolume = 0;
        private static byte[] chips = new byte[256];
        private static string PlayingFileName;
        private static string PlayingArcFileName;
        private static int MidiMode = 0;
        private static int SongNo = 0;
        private static List<Tuple<string, byte[]>> ExtendFile = null;
        private static EnmFileFormat PlayingFileFormat;

        private static System.Diagnostics.Stopwatch stwh = System.Diagnostics.Stopwatch.StartNew();
        public static int ProcTimePer1Frame = 0;

        private static List<NAudio.Midi.MidiOut> midiOuts = new();
        private static List<int> midiOutsType = new();
        public static string errMsg = "";
        public static bool flgReinit = false;

        public static bool EmuOnly { get; set; }
        public static InstanceMarker MucomDotNETim { get; private set; }
        public static InstanceMarker PMDDotNETim { get; private set; }

        public static InstanceMarker MoonDriverDotNETim { get; private set; }


        public static List<vstMng.vstInfo2> GetVSTInfos()
        {
            return vstMng.getVSTInfos();
        }

        public static vstInfo GetVSTInfo(string filename)
        {
            return vstMng.getVSTInfo(filename);
        }

        public static bool AddVSTeffect(string fileName)
        {
            return vstMng.addVSTeffect(fileName);
        }

        public static bool DelVSTeffect(string key)
        {
            return vstMng.delVSTeffect(key);
        }

        public static void CopyWaveBuffer(short[][] dest)
        {
            if (driverVirtual is nsf)
            {
                ((nsf)driverVirtual).visWaveBufferCopy(dest);
                return;
            }
            else if (driverVirtual is sid)
            {
                ((sid)driverVirtual).visWaveBufferCopy(dest);
                return;
            }

            if (mds == null) return;
            mds.visWaveBuffer.Copy(dest);
        }

        public static List<PlayList.music> GetMusic(string file, byte[] buf, string zipFile = null, object entry = null)
        {
            List<PlayList.music> musics = new();
            PlayList.music music = new();

            music.format = EnmFileFormat.unknown;
            music.fileName = file;
            music.arcFileName = zipFile;
            music.arcType = EnmArcType.unknown;
            if (!string.IsNullOrEmpty(zipFile)) music.arcType = zipFile.ToLower().LastIndexOf(".zip") != -1 ? EnmArcType.ZIP : EnmArcType.LZH;
            music.title = "unknown";
            music.game = "unknown";
            music.type = "-";

            if (file.ToLower().LastIndexOf(".nrd") != -1)
            {

                music.format = EnmFileFormat.NRT;
                uint index = 42;
                GD3 gd3 = (new NRTDRV(setting)).getGD3Info(buf, index);
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
            else if (file.ToLower().LastIndexOf(".mgs") != -1)
            {

                music.format = EnmFileFormat.MGS;
                uint index = 8;
                GD3 gd3 = (new Driver.MGSDRV.MGSDRV()).getGD3Info(buf, index);
                music.title = gd3.TrackName;
                music.titleJ = gd3.TrackNameJ;
                music.game = "";
                music.gameJ = "";
                music.composer = "";
                music.composerJ = "";
                music.vgmby = "";

                music.converted = "";
                music.notes = "";

            }
            else if (file.ToLower().LastIndexOf(".bgm") != -1)
            {

                music.format = EnmFileFormat.MuSICA;
                GD3 gd3 = (new Driver.MuSICA.MuSICA()).getGD3Info(buf, 0);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = "";
                music.gameJ = "";
                music.composer = "";
                music.composerJ = "";
                music.vgmby = "";
                music.converted = "";
                music.notes = gd3.Notes == "" ? "" : gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".msd") != -1)
            {

                music.format = EnmFileFormat.MuSICA_src;
                string vcd = Path.ChangeExtension(music.fileName,".vcd");
                byte[] vcdBuf = null;
                if (File.Exists(vcd))
                {
                    vcdBuf=File.ReadAllBytes(vcd);
                }
                GD3 gd3 = (new Driver.MuSICA.MuSICA_K4()).getGD3Info(buf, vcdBuf);
                if (gd3 == null)
                {
                    //MessageBox.Show(".MSDのコンパイルに失敗しました", "PlayList", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    music.title = Path.GetFileName(file);
                    music.titleJ = Path.GetFileName(file);
                    music.notes = "";
                }
                else
                {
                    music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                    music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                    music.notes = gd3.Notes == "" ? "" : gd3.Notes;
                }
                music.game = "";
                music.gameJ = "";
                music.composer = "";
                music.composerJ = "";
                music.vgmby = "";
                music.converted = "";
            }
            else if (file.ToLower().LastIndexOf(".mdr") != -1)
            {

                music.format = EnmFileFormat.MDR;
                uint index = 0;
                GD3 gd3 = (new Driver.MoonDriver.MoonDriver()).getGD3Info(buf, index);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".mdx") != -1)
            {

                music.format = EnmFileFormat.MDX;
                uint index = 0;
                GD3 gd3 = (new Driver.MXDRV.MXDRV()).getGD3Info(buf, index);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".mnd") != -1)
            {

                music.format = EnmFileFormat.MND;
                uint index = 0;
                GD3 gd3 = (new Driver.MNDRV.mndrv()).getGD3Info(buf, index);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".mub") != -1)
            {

                music.format = EnmFileFormat.MUB;
                uint index = 0;
                //GD3 gd3 = (new Driver.MUCOM88.MUCOM88()).getGD3InfoMUB(buf, index);
                GD3 gd3 = new Driver.MucomDotNET(MucomDotNETim).getGD3Info(buf, index);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".muc") != -1)
            {

                music.format = EnmFileFormat.MUC;
                uint index = 0;
                //GD3 gd3 = (new Driver.MUCOM88.MUCOM88()).getGD3Info(buf, index);
                GD3 gd3 = new Driver.MucomDotNET(MucomDotNETim).getGD3Info(buf, index);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".mml") != -1)
            {

                music.format = EnmFileFormat.MML;
                uint index = 0;
                Driver.PMDDotNET pmd= new(PMDDotNETim);
                pmd.PlayingFileName = file;
                GD3 gd3 = pmd.getGD3Info(buf, index, Driver.PMDDotNET.enmPMDFileType.MML);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
                music.titleJ = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

            }
            else if (file.ToLower().LastIndexOf(".m") != -1 || file.ToLower().LastIndexOf(".m2") != -1 || file.ToLower().LastIndexOf(".mz") != -1)
            {

                music.format = EnmFileFormat.M;
                uint index = 0;
                GD3 gd3 = new Driver.PMDDotNET(PMDDotNETim).getGD3Info(buf, index, Driver.PMDDotNET.enmPMDFileType.M);
                music.title = gd3.TrackName == "" ? Path.GetFileName(file) : gd3.TrackName;
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
                music.format = EnmFileFormat.XGM;
                GD3 gd3 = new xgm(setting).getGD3Info(buf, 0);
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
            else if (file.ToLower().LastIndexOf(".xgz") != -1)
            {

                buf = null;
                try
                {
                    if (entry == null || entry is ZipArchiveEntry)
                    {
                        if (entry == null)
                            buf = Common.unzipFile(file, null);
                        else
                            buf = Common.unzipFile(null, (ZipArchiveEntry)entry);
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }
                catch
                {
                    //vgzではなかった
                }

                music.format = EnmFileFormat.XGM;
                GD3 gd3 = new xgm(setting).getGD3Info(buf, 0);
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
            else if (file.ToLower().LastIndexOf(".zgm") != -1)
            {
                music.format = EnmFileFormat.ZGM;
                GD3 gd3 = new Driver.ZGM.zgm().getGD3Info(buf, 0);
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
                music.format = EnmFileFormat.S98;
                GD3 gd3 = new S98(setting).getGD3Info(buf, 0);
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

                    music.version = gd3.Version;
                    music.useChips = gd3.UsedChips;
                }
                else
                {
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }

            }
            else if (file.ToLower().LastIndexOf(".nsf") != -1)
            {
                nsf nsf = new(setting);
                GD3 gd3 = nsf.getGD3Info(buf, 0);

                if (gd3 != null)
                {
                    for (int s = 0; s < nsf.songs; s++)
                    {
                        music = new PlayList.music();
                        music.format = EnmFileFormat.NSF;
                        music.fileName = file;
                        music.arcFileName = zipFile;
                        music.arcType = EnmArcType.unknown;
                        if (!string.IsNullOrEmpty(zipFile)) music.arcType = zipFile.ToLower().LastIndexOf(".zip") != -1 ? EnmArcType.ZIP : EnmArcType.LZH;
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
                    music.format = EnmFileFormat.NSF;
                    music.fileName = file;
                    music.arcFileName = zipFile;
                    music.game = "unknown";
                    music.type = "-";
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
                }

            }
            else if (file.ToLower().LastIndexOf(".hes") != -1)
            {
                hes hes = new();
                GD3 gd3 = hes.getGD3Info(buf, 0);

                for (int s = 0; s < 256; s++)
                {
                    music = new PlayList.music();
                    music.format = EnmFileFormat.HES;
                    music.fileName = file;
                    music.arcFileName = zipFile;
                    music.arcType = EnmArcType.unknown;
                    if (!string.IsNullOrEmpty(zipFile)) music.arcType = zipFile.ToLower().LastIndexOf(".zip") != -1 ? EnmArcType.ZIP : EnmArcType.LZH;
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
                Driver.SID.sid sid = new();
                GD3 gd3 = sid.getGD3Info(buf, 0);

                for (int s = 0; s < sid.songs; s++)
                {
                    music = new PlayList.music();
                    music.format = EnmFileFormat.SID;
                    music.fileName = file;
                    music.arcFileName = zipFile;
                    music.arcType = EnmArcType.unknown;
                    if (!string.IsNullOrEmpty(zipFile)) music.arcType = zipFile.ToLower().LastIndexOf(".zip") != -1 ? EnmArcType.ZIP : EnmArcType.LZH;
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
                music.format = EnmFileFormat.MID;
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
                music.format = EnmFileFormat.RCP;
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
            else if (file.ToLower().LastIndexOf(".wav") != -1)
            {
                music.format = EnmFileFormat.WAV;
                music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
            }
            else if (file.ToLower().LastIndexOf(".mp3") != -1)
            {
                music.format = EnmFileFormat.MP3;
                music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
            }
            else if (file.ToLower().LastIndexOf(".aiff") != -1)
            {
                music.format = EnmFileFormat.AIFF;
                music.title = string.Format("({0})", System.IO.Path.GetFileName(file));
            }
            else
            {
                if (buf.Length < 0x40)
                {
                    musics.Add(music);
                    return musics;
                }
                if (Common.getLE32(buf, 0x00) != Vgm.FCC_VGM)
                {
                    //musics.Add(music);
                    //return musics;
                    //VGZかもしれないので確認する
                    try
                    {
                        if (entry == null || entry is ZipArchiveEntry)
                        {
                            buf = null;
                            if (entry == null)
                                buf = Common.unzipFile(file, null);
                            else
                                buf = Common.unzipFile(null, (ZipArchiveEntry)entry);
                        }
                        else
                        {
                            UnlhaWrap.UnlhaCmd cmd = new();
                            buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                        }
                    }
                    catch
                    {
                        //vgzではなかった
                    }
                }

                if (Common.getLE32(buf, 0x00) != Vgm.FCC_VGM)
                {
                    musics.Add(music);
                    return musics;
                }

                music.format = EnmFileFormat.VGM;
                uint version = Common.getLE32(buf, 0x08);
                string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint vgmGd3 = Common.getLE32(buf, 0x14);
                GD3 gd3 = new();
                if (vgmGd3 != 0)
                {
                    try
                    {
                        uint vgmGd3Id = Common.getLE32(buf, vgmGd3 + 0x14);
                        if (vgmGd3Id != Vgm.FCC_GD3)
                        {
                            musics.Add(music);
                            return musics;
                        }
                        gd3 = (new Vgm(setting)).getGD3Info(buf, vgmGd3);
                    }
                    catch { }
                }

                uint TotalCounter = Common.getLE32(buf, 0x18);
                uint vgmLoopOffset = Common.getLE32(buf, 0x1c);
                uint LoopCounter = Common.getLE32(buf, 0x20);

                music.title = gd3.TrackName;
                music.titleJ = gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;
                music.version = Version;
                music.useChips = gd3.UsedChips;
                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

                double sec = (double)TotalCounter / (double)setting.outputDevice.SampleRate;
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

        public static void RealChipClose()
        {
            if (realChip != null)
            {
                realChip.Close();
                realChip = null;
            }
        }

        public static List<PlayList.music> GetMusic(PlayList.music ms, byte[] buf, string zipFile = null)
        {
            List<PlayList.music> musics = new();
            PlayList.music music = new()
            {
                format = EnmFileFormat.unknown,
                fileName = ms.fileName,
                arcFileName = zipFile,
                title = "unknown",
                game = "unknown",
                type = "-"
            };

            if (ms.fileName.ToLower().LastIndexOf(".nrd") != -1)
            {

                music.format = EnmFileFormat.NRT;
                uint index = 42;
                GD3 gd3 = (new NRTDRV(setting)).getGD3Info(buf, index);
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
            else if (ms.fileName.ToLower().LastIndexOf(".mgs") != -1)
            {

                music.format = EnmFileFormat.MGS;
                uint index = 8;
                GD3 gd3 = (new Driver.MGSDRV.MGSDRV()).getGD3Info(buf, index);
                music.title = gd3.TrackName;
                music.titleJ = gd3.TrackNameJ;
                music.game = "";
                music.gameJ = "";
                music.composer = "";
                music.composerJ = "";
                music.vgmby = "";

                music.converted = "";
                music.notes = "";

            }
            else if (ms.fileName.ToLower().LastIndexOf(".xgm") != -1)
            {
                music.format = EnmFileFormat.XGM;
                GD3 gd3 = new xgm(setting).getGD3Info(buf, 0);
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
                music.format = EnmFileFormat.S98;
                GD3 gd3 = new S98(setting).getGD3Info(buf, 0);
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
                nsf nsf = new(setting);
                GD3 gd3 = nsf.getGD3Info(buf, 0);

                if (gd3 != null)
                {
                    if (ms.songNo == -1)
                    {
                        for (int s = 0; s < nsf.songs; s++)
                        {
                            music = new PlayList.music
                            {
                                format = EnmFileFormat.NSF,
                                fileName = ms.fileName,
                                arcFileName = zipFile,
                                title = string.Format("{0} - Trk {1}", gd3.GameName, s),
                                titleJ = string.Format("{0} - Trk {1}", gd3.GameNameJ, s),
                                game = gd3.GameName,
                                gameJ = gd3.GameNameJ,
                                composer = gd3.Composer,
                                composerJ = gd3.ComposerJ,
                                vgmby = gd3.VGMBy,
                                converted = gd3.Converted,
                                notes = gd3.Notes,
                                songNo = s
                            };

                            musics.Add(music);
                        }

                        return musics;

                    }
                    else
                    {
                        music.format = EnmFileFormat.NSF;
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
                    music.format = EnmFileFormat.NSF;
                    music.fileName = ms.fileName;
                    music.arcFileName = zipFile;
                    music.game = "unknown";
                    music.type = "-";
                    music.title = string.Format("({0})", System.IO.Path.GetFileName(ms.fileName));
                }

            }
            else if (ms.fileName.ToLower().LastIndexOf(".mid") != -1)
            {
                music.format = EnmFileFormat.MID;
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
                music.format = EnmFileFormat.RCP;
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
                if (Common.getLE32(buf, 0x00) != Vgm.FCC_VGM)
                {
                    musics.Add(music);
                    return musics;
                }

                music.format = EnmFileFormat.VGM;
                uint version = Common.getLE32(buf, 0x08);
                string Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint vgmGd3 = Common.getLE32(buf, 0x14);
                GD3 gd3 = new();
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = Common.getLE32(buf, vgmGd3 + 0x14);
                    if (vgmGd3Id != Vgm.FCC_GD3)
                    {
                        musics.Add(music);
                        return musics;
                    }
                    gd3 = (new Vgm(setting)).getGD3Info(buf, vgmGd3);
                }

                uint TotalCounter = Common.getLE32(buf, 0x18);
                uint vgmLoopOffset = Common.getLE32(buf, 0x1c);
                uint LoopCounter = Common.getLE32(buf, 0x20);

                music.title = gd3.TrackName;
                music.titleJ = gd3.TrackNameJ;
                music.game = gd3.GameName;
                music.gameJ = gd3.GameNameJ;
                music.composer = gd3.Composer;
                music.composerJ = gd3.ComposerJ;
                music.vgmby = gd3.VGMBy;

                music.converted = gd3.Converted;
                music.notes = gd3.Notes;

                double sec = (double)TotalCounter / (double)setting.outputDevice.SampleRate;
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

        public static List<Setting.ChipType2> GetRealChipList(EnmRealChipType scciType)
        {
            if (realChip == null) return null;
            return realChip.GetRealChipList(scciType);
        }

        private static string GetNRDString(byte[] buf, ref int index)
        {
            if (buf == null || buf.Length < 1 || index < 0 || index >= buf.Length) return "";

            try
            {
                List<byte> lst = new();
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

        private static System.Threading.Thread trd = null;
        public static void Init(Setting setting)
        {
            log.ForcedWrite("Audio:Init:Begin");

            if (trd == null)
            {
                trd = new Thread(TrdIF)
                {
                    Priority = System.Threading.ThreadPriority.BelowNormal
                };
                trd.Start();
            }

            log.ForcedWrite("Audio:Init:STEP 01");

            naudioWrap = new NAudioWrap((int)setting.outputDevice.SampleRate, TrdVgmVirtualFunction);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;

            log.ForcedWrite("Audio:Init:STEP 02");

            Audio.setting = setting;// Copy();
            vstMng.setting = setting;

            waveWriter = new WaveWriter(setting);

            log.ForcedWrite("Audio:Init:STEP 03");

            if (Audio.setting.AY8910Type == null || Audio.setting.AY8910Type.Length < 2)
            {
                Audio.setting.AY8910Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.AY8910Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.AY8910Type[i].UseEmu = new bool[1];
                    Audio.setting.AY8910Type[i].UseEmu[0] = true;
                    Audio.setting.AY8910Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.K051649Type == null || Audio.setting.K051649Type.Length < 2)
            {
                Audio.setting.K051649Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.K051649Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.K051649Type[i].UseEmu = new bool[1];
                    Audio.setting.K051649Type[i].UseEmu[0] = true;
                    Audio.setting.K051649Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.C140Type == null || Audio.setting.C140Type.Length < 2)
            {
                Audio.setting.C140Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.C140Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.C140Type[i].UseEmu = new bool[1];
                    Audio.setting.C140Type[i].UseEmu[0] = true;
                    Audio.setting.C140Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.ES5503Type == null || Audio.setting.ES5503Type.Length < 2)
            {
                Audio.setting.ES5503Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.ES5503Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.ES5503Type[i].UseEmu = new bool[1];
                    Audio.setting.ES5503Type[i].UseEmu[0] = true;
                    Audio.setting.ES5503Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.HuC6280Type == null || Audio.setting.HuC6280Type.Length < 2)
            {
                Audio.setting.HuC6280Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.HuC6280Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.HuC6280Type[i].UseEmu = new bool[1];
                    Audio.setting.HuC6280Type[i].UseEmu[0] = true;
                    Audio.setting.HuC6280Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.SEGAPCMType == null || Audio.setting.SEGAPCMType.Length < 2)
            {
                Audio.setting.SEGAPCMType = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.SEGAPCMType[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.SEGAPCMType[i].UseEmu = new bool[1];
                    Audio.setting.SEGAPCMType[i].UseEmu[0] = true;
                    Audio.setting.SEGAPCMType[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.SN76489Type == null || Audio.setting.SN76489Type.Length < 2)
            {
                Audio.setting.SN76489Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.SN76489Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.SN76489Type[i].UseEmu = new bool[2];
                    Audio.setting.SN76489Type[i].UseEmu[0] = true;
                    Audio.setting.SN76489Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.Y8950Type == null || Audio.setting.Y8950Type.Length < 2)
            {
                Audio.setting.Y8950Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.Y8950Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.Y8950Type[i].UseEmu = new bool[1];
                    Audio.setting.Y8950Type[i].UseEmu[0] = true;
                    Audio.setting.Y8950Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YM2151Type == null || Audio.setting.YM2151Type.Length < 2 || (Audio.setting.YM2151Type[0].realChipInfo!=null && Audio.setting.YM2151Type[0].realChipInfo.Length<2))
            {
                Audio.setting.YM2151Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM2151Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo(), new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM2151Type[i].UseEmu = new bool[3];
                    Audio.setting.YM2151Type[i].UseEmu[0] = true;
                    Audio.setting.YM2151Type[i].UseReal = new bool[2];
                }
            }
            if (Audio.setting.YM2203Type == null || Audio.setting.YM2203Type.Length < 2)
            {
                Audio.setting.YM2203Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM2203Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM2203Type[i].UseEmu = new bool[1];
                    Audio.setting.YM2203Type[i].UseEmu[0] = true;
                    Audio.setting.YM2203Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YM2413Type == null || Audio.setting.YM2413Type.Length < 2)
            {
                Audio.setting.YM2413Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM2413Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM2413Type[i].UseEmu = new bool[1];
                    Audio.setting.YM2413Type[i].UseEmu[0] = true;
                    Audio.setting.YM2413Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YM2608Type == null || Audio.setting.YM2608Type.Length < 2)
            {
                Audio.setting.YM2608Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM2608Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM2608Type[i].UseEmu = new bool[1];
                    Audio.setting.YM2608Type[i].UseEmu[0] = true;
                    Audio.setting.YM2608Type[i].UseReal = new bool[1];
                }
            }

            if (Audio.setting.YM2610Type == null 
                || Audio.setting.YM2610Type.Length < 2
                || Audio.setting.YM2610Type[0].UseReal == null
                || Audio.setting.YM2610Type[0].UseReal.Length < 3
                || Audio.setting.YM2610Type[1].UseReal == null
                || Audio.setting.YM2610Type[1].UseReal.Length < 3
                )
            {
                Audio.setting.YM2610Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM2610Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo(), new Setting.ChipType2.RealChipInfo(), new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM2610Type[i].UseEmu = new bool[1];
                    Audio.setting.YM2610Type[i].UseEmu[0] = true;
                    Audio.setting.YM2610Type[i].UseReal = new bool[3];
                }
            }

            if (Audio.setting.YM2612Type == null || Audio.setting.YM2612Type.Length < 2)
            {
                Audio.setting.YM2612Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM2612Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM2612Type[i].UseEmu = new bool[3];
                    Audio.setting.YM2612Type[i].UseEmu[0] = true;
                    Audio.setting.YM2612Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YM3526Type == null || Audio.setting.YM3526Type.Length < 2)
            {
                Audio.setting.YM3526Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM3526Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM3526Type[i].UseEmu = new bool[1];
                    Audio.setting.YM3526Type[i].UseEmu[0] = true;
                    Audio.setting.YM3526Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YM3812Type == null || Audio.setting.YM3812Type.Length < 2)
            {
                Audio.setting.YM3812Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YM3812Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YM3812Type[i].UseEmu = new bool[1];
                    Audio.setting.YM3812Type[i].UseEmu[0] = true;
                    Audio.setting.YM3812Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YMF262Type == null || Audio.setting.YMF262Type.Length < 2)
            {
                Audio.setting.YMF262Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YMF262Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YMF262Type[i].UseEmu = new bool[1];
                    Audio.setting.YMF262Type[i].UseEmu[0] = true;
                    Audio.setting.YMF262Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YMF271Type == null || Audio.setting.YMF271Type.Length < 2)
            {
                Audio.setting.YMF271Type = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YMF271Type[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YMF271Type[i].UseEmu = new bool[1];
                    Audio.setting.YMF271Type[i].UseEmu[0] = true;
                    Audio.setting.YMF271Type[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YMF278BType == null || Audio.setting.YMF278BType.Length < 2)
            {
                Audio.setting.YMF278BType = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YMF278BType[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YMF278BType[i].UseEmu = new bool[1];
                    Audio.setting.YMF278BType[i].UseEmu[0] = true;
                    Audio.setting.YMF278BType[i].UseReal = new bool[1];
                }
            }
            if (Audio.setting.YMZ280BType == null || Audio.setting.YMZ280BType.Length < 2)
            {
                Audio.setting.YMZ280BType = new Setting.ChipType2[] { new Setting.ChipType2(), new Setting.ChipType2() };
                for (int i = 0; i < 2; i++)
                {
                    Audio.setting.YMZ280BType[i].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                    Audio.setting.YMZ280BType[i].UseEmu = new bool[1];
                    Audio.setting.YMZ280BType[i].UseEmu[0] = true;
                    Audio.setting.YMZ280BType[i].UseReal = new bool[1];
                }
            }

            if (mds == null)
                mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, null);
            else
                mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, null);

            List<MDSound.MDSound.Chip> lstChips = new();
            MDSound.MDSound.Chip chip;

            ym2612 ym2612 = new();
            chip = new MDSound.MDSound.Chip
            {
                type = MDSound.MDSound.enmInstrumentType.YM2612,
                ID = (byte)0,
                Instrument = ym2612,
                Update = ym2612.Update,
                Start = ym2612.Start,
                Stop = ym2612.Stop,
                Reset = ym2612.Reset,
                SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                Volume = setting.balance.YM2612Volume,
                Clock = 7670454,
                Option = null
            };
            chipLED.PriOPN2 = 1;
            lstChips.Add(chip);

            sn76489 sn76489 = new();
            chip = new MDSound.MDSound.Chip
            {
                type = MDSound.MDSound.enmInstrumentType.SN76489,
                ID = (byte)0,
                Instrument = sn76489,
                Update = sn76489.Update,
                Start = sn76489.Start,
                Stop = sn76489.Stop,
                Reset = sn76489.Reset,
                SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                Volume = setting.balance.SN76489Volume,
                Clock = 3579545,
                Option = null
            };
            chipLED.PriDCSG = 1;
            lstChips.Add(chip);

            if (mdsMIDI == null)
                mdsMIDI = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
            else
                mdsMIDI.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

            if (realChip == null && !EmuOnly )
            {
                log.ForcedWrite("Audio:Init:STEP 04");
                realChip = new RealChip(!setting.unuseRealChip);
            }

            if (realChip != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    scYM2612[i] = realChip.GetRealChip(Audio.setting.YM2612Type[i]);
                    scYM2612[i]?.init();
                    scSN76489[i] = realChip.GetRealChip(Audio.setting.SN76489Type[i]);
                    scSN76489[i]?.init();
                    scYM2608[i] = realChip.GetRealChip(Audio.setting.YM2608Type[i]);
                    scYM2608[i]?.init();
                    scYM2151[i] = realChip.GetRealChip(Audio.setting.YM2151Type[i], 3);
                    scYM2151[i]?.init();
                    scYM2151_4M[i] = realChip.GetRealChip(Audio.setting.YM2151Type[i], 4);
                    scYM2151_4M[i]?.init();
                    scYM2203[i] = realChip.GetRealChip(Audio.setting.YM2203Type[i]);
                    scYM2203[i]?.init();
                    scAY8910[i] = realChip.GetRealChip(Audio.setting.AY8910Type[i]);
                    scAY8910[i]?.init();
                    scK051649[i] = realChip.GetRealChip(Audio.setting.K051649Type[i]);
                    scK051649[i]?.init();
                    scYM2413[i] = realChip.GetRealChip(Audio.setting.YM2413Type[i]);
                    scYM2413[i]?.init();
                    scYM3526[i] = realChip.GetRealChip(Audio.setting.YM3526Type[i]);
                    scYM3526[i]?.init();
                    scYM3812[i] = realChip.GetRealChip(Audio.setting.YM3812Type[i]);
                    scYM3812[i]?.init();
                    scYMF262[i] = realChip.GetRealChip(Audio.setting.YMF262Type[i]);
                    scYMF262[i]?.init();
                    scYM2610[i] = realChip.GetRealChip(Audio.setting.YM2610Type[i]);
                    scYM2610[i]?.init();
                    scYM2610EA[i] = realChip.GetRealChip(Audio.setting.YM2610Type[i], 1);
                    scYM2610EA[i]?.init();
                    scYM2610EB[i] = realChip.GetRealChip(Audio.setting.YM2610Type[i], 2);
                    scYM2610EB[i]?.init();
                    scSEGAPCM[i] = realChip.GetRealChip(Audio.setting.SEGAPCMType[i]);
                    scSEGAPCM[i]?.init();
                    scC140[i] = realChip.GetRealChip(Audio.setting.C140Type[i]);
                    scC140[i]?.init();
                }

            }

            chipRegister = new ChipRegister(
                setting
                , mds
                , realChip
                , vstMng
                , scYM2612
                , scSN76489
                , scYM2608
                , scYM2151
                , scYM2151_4M
                , scYM2203
                , scYM2413
                , scYM2610
                , scYM2610EA
                , scYM2610EB
                , scYM3526
                , scYM3812
                , scYMF262
                , scC140
                , scSEGAPCM
                , scAY8910
                , scK051649
                );
            chipRegister.initChipRegister(null);

            log.ForcedWrite("Audio:Init:STEP 05");

            Paused = false;
            Stopped = true;
            FatalError = false;
            oneTimeReset = false;

            log.ForcedWrite("Audio:Init:STEP 06");

            log.ForcedWrite("Audio:Init:VST:STEP 01");

            vstMng.vstparse();

            log.ForcedWrite("Audio:Init:VST:STEP 02"); //Load VST instrument

            //複数のmidioutの設定から必要なVSTを絞り込む
            Dictionary<string, int> dicVst = new();
            if (setting.midiOut.lstMidiOutInfo != null)
            {
                foreach (midiOutInfo[] aryMoi in setting.midiOut.lstMidiOutInfo)
                {
                    if (aryMoi == null) continue;
                    Dictionary<string, int> dicVst2 = new();
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
                    vstMng.SetUpVstInstrument(kv);
            }


            if (setting.vst != null && setting.vst.VSTInfo != null)
            {
                log.ForcedWrite("Audio:Init:VST:STEP 03"); //Load VST Effect
                vstMng.SetUpVstEffect();
            }

            log.ForcedWrite("Audio:Init:STEP 07");

            //midi outをリリース
            ReleaseAllMIDIout();

            log.ForcedWrite("Audio:Init:STEP 08");

            //midi out のインスタンスを作成
            MakeMIDIout(setting, 1);
            chipRegister.resetAllMIDIout();

            log.ForcedWrite("Audio:Init:STEP 09");

            //各外部dllの動的読み込み
            MucomDotNETim = new InstanceMarker();
            MucomDotNETim.LoadCompilerDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\mucomDotNETCompiler.dll"));
            MucomDotNETim.LoadDriverDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\mucomDotNETDriver.dll"));
            PMDDotNETim = new InstanceMarker();
            PMDDotNETim.LoadCompilerDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\PMDDotNETCompiler.dll"));
            PMDDotNETim.LoadDriverDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\PMDDotNETDriver.dll"));
            MoonDriverDotNETim = new InstanceMarker();
            MoonDriverDotNETim.LoadCompilerDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\moonDriverDotNETCompiler.dll"));
#if X64
            MoonDriverDotNETim.LoadDriverDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\MoonDriverDotNETDriver.dll"));
#else
            moonDriverDotNETim.LoadDriverDll(Path.Combine(System.Windows.Forms.Application.StartupPath, "plugin\\driver\\moonDriverDotNETDriver.dll"));
#endif

            log.ForcedWrite("Audio:Init:STEP 10");

            naudioWrap.Start(Audio.setting);

            log.ForcedWrite("Audio:Init:Complete");

        }

        private static void TrdIF()
        {
            while (true)
            {
                Request req = OpeManager.GetRequestToAudio();
                if (req == null)
                {
                    Thread.Sleep(1);
                    continue;
                }

                switch (req.request)
                {
                    case enmRequest.Die://自殺してください
                        SeqDie();
                        req.end = true;
                        return;
                    case enmRequest.Stop:
                        Stop();
                        req.end = true;
                        OpeManager.CompleteRequestToAudio(req);
                        break;
                }
            }
        }

        private static void SeqDie()
        {
            Close(false);
            RealChipClose();
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
                    vstMng.SetupVstMidiOut(setting.midiOut.lstMidiOutInfo[m][i]);
                }

                if (mo != null)
                {
                    midiOuts.Add(mo);
                    midiOutsType.Add(t);
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

            vstMng.ReleaseAllMIDIout();
        }

        public static MDSound.MDSound.Chip GetMDSChipInfo(MDSound.MDSound.enmInstrumentType typ)
        {
            return chipRegister.GetChipInfo(typ);
        }

        public static int GetLatency()
        {
            if (setting.outputDevice.DeviceType != Common.DEV_AsioOut)
            {
                return (int)setting.outputDevice.SampleRate * setting.outputDevice.Latency / 1000;
            }
            return naudioWrap.getAsioLatency();
        }

        public static void SetVGMBuffer(EnmFileFormat format, byte[] srcBuf, string playingFileName, string playingArcFileName, int midiMode, int songNo, List<Tuple<string, byte[]>> extFile)
        {
            //Stop();
            PlayingFileFormat = format;
            vgmBuf = srcBuf;
            PlayingFileName = playingFileName;//WaveWriter向け
            PlayingArcFileName = playingArcFileName;
            MidiMode = midiMode;
            SongNo = songNo;
            chipRegister.SetFileName(playingFileName);//ExportMIDI向け
            ExtendFile = extFile;//追加ファイル
            Common.playingFilePath = Path.GetDirectoryName(playingFileName);

            if (naudioFileReader != null)
            {
                NAudioStop();
            }

            if (format == EnmFileFormat.WAV || format == EnmFileFormat.MP3 || format == EnmFileFormat.AIFF)
            {
                naudioFileName = playingFileName;
            }
            else
            {
                naudioFileName = null;
            }
        }

        public static void GetPlayingFileName(out string playingFileName, out string playingArcFileName)
        {
            playingFileName = PlayingFileName;
            playingArcFileName = PlayingArcFileName;
        }


        public static bool Play(Setting setting)
        {
            errMsg = "";

            Stop();

            try
            {
                waveWriter.Open(PlayingFileName);
            }
            catch
            {
                errMsg = "wave file open error.";
                return false;
            }

            MDSound.MDSound.np_nes_apu_volume = 0;
            MDSound.MDSound.np_nes_dmc_volume = 0;
            MDSound.MDSound.np_nes_fds_volume = 0;
            MDSound.MDSound.np_nes_fme7_volume = 0;
            MDSound.MDSound.np_nes_mmc5_volume = 0;
            MDSound.MDSound.np_nes_n106_volume = 0;
            MDSound.MDSound.np_nes_vrc6_volume = 0;
            MDSound.MDSound.np_nes_vrc7_volume = 0;


            if (PlayingFileFormat == EnmFileFormat.MGS)
            {
                driverVirtual = new Driver.MGSDRV.MGSDRV
                {
                    setting = setting
                };
                ((Driver.MGSDRV.MGSDRV)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MGSDRV.MGSDRV
                    {
                        setting = setting
                    };
                    ((Driver.MGSDRV.MGSDRV)driverReal).PlayingFileName = PlayingFileName;
                }
                return MgsPlay_mgsdrv(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.MuSICA)
            {
                driverVirtual = new Driver.MuSICA.MuSICA
                {
                    setting = setting
                };
                ((Driver.MuSICA.MuSICA)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MuSICA.MuSICA
                    {
                        setting = setting
                    };
                    ((Driver.MuSICA.MuSICA)driverReal).PlayingFileName = PlayingFileName;
                }
                return MscPlay_mscdrv(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.MuSICA_src)
            {
                string vcd = Path.ChangeExtension(PlayingFileName, ".vcd");
                byte[] vcdBuf = null;
                if (File.Exists(vcd))
                {
                    vcdBuf = File.ReadAllBytes(vcd);
                }

                driverVirtual = new Driver.MuSICA.MuSICA_K4();
                bool ret = ((Driver.MuSICA.MuSICA_K4)driverVirtual).compile(vgmBuf, vcdBuf);
                if (!ret) return false;

                vgmBuf = ((Driver.MuSICA.MuSICA_K4)driverVirtual).getBgmBin();
                driverVirtual = new Driver.MuSICA.MuSICA
                {
                    setting = setting
                };
                ((Driver.MuSICA.MuSICA)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MuSICA.MuSICA();
                    driverReal.setting = setting;
                    ((Driver.MuSICA.MuSICA)driverReal).PlayingFileName = PlayingFileName;
                }
                return MscPlay_mscdrv(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.MUB)
            {
                driverVirtual = new Driver.MucomDotNET(MucomDotNETim);
                driverVirtual.setting = setting;
                ((Driver.MucomDotNET)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null && !setting.YM2608Type[0].UseEmu[0])
                {
                    driverReal = new Driver.MucomDotNET(MucomDotNETim);
                    driverReal.setting = setting;
                    ((Driver.MucomDotNET)driverReal).PlayingFileName = PlayingFileName;
                }
                return MucPlay_mucomDotNET(setting, Driver.MucomDotNET.enmMUCOMFileType.MUB);
            }

            if (PlayingFileFormat == EnmFileFormat.MUC)
            {
                driverVirtual = new Driver.MucomDotNET(MucomDotNETim);
                driverVirtual.setting = setting;
                ((Driver.MucomDotNET)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null && !setting.YM2608Type[0].UseEmu[0])
                {
                    driverReal = new Driver.MucomDotNET(MucomDotNETim);
                    driverReal.setting = setting;
                    ((Driver.MucomDotNET)driverReal).PlayingFileName = PlayingFileName;
                }
                
                return MucPlay_mucomDotNET(setting, Driver.MucomDotNET.enmMUCOMFileType.MUC);
            }

            if (PlayingFileFormat == EnmFileFormat.MML || PlayingFileFormat == EnmFileFormat.M)
            {
                driverVirtual = new Driver.PMDDotNET(PMDDotNETim);
                driverVirtual.setting = setting;
                ((Driver.PMDDotNET)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null && !setting.YM2608Type[0].UseEmu[0])
                {
                    driverReal = new Driver.PMDDotNET(PMDDotNETim);
                    driverReal.setting = setting;
                    ((Driver.PMDDotNET)driverReal).PlayingFileName = PlayingFileName;
                }
                return MmlPlay_PMDDotNET(setting, PlayingFileFormat == EnmFileFormat.MML ? 0 : 1);
            }

            if (PlayingFileFormat == EnmFileFormat.NRT)
            {
                driverVirtual = new NRTDRV(setting);
                driverVirtual.setting = setting;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new NRTDRV(setting);
                    driverReal.setting = setting;
                }
                return NrdPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.MDR)
            {
                //driverVirtual = new Driver.MoonDriver.MoonDriver();
                //driverVirtual.setting = setting;
                //((Driver.MoonDriver.MoonDriver)driverVirtual).ExtendFile = (ExtendFile != null && ExtendFile.Count > 0) ? ExtendFile[0] : null;
                //driverReal = null;
                //if (setting.outputDevice.DeviceType != Common.DEV_Null)
                //{
                //    driverReal = new Driver.MoonDriver.MoonDriver();
                //    driverReal.setting = setting;
                //    ((Driver.MoonDriver.MoonDriver)driverReal).ExtendFile = (ExtendFile != null && ExtendFile.Count > 0) ? ExtendFile[0] : null;
                //}
                //return mdrPlay(setting);

                driverVirtual = new Driver.MoonDriverDotNET(MoonDriverDotNETim);
                driverVirtual.setting = setting;
                ((Driver.MoonDriverDotNET)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MoonDriverDotNET(MoonDriverDotNETim);
                    driverReal.setting = setting;
                    ((Driver.MoonDriverDotNET)driverReal).PlayingFileName = PlayingFileName;
                }

                return MdlPlay_moonDriverDotNET(setting, Driver.MoonDriverDotNET.enmMoonDriverFileType.MDR);
            }

            if (PlayingFileFormat == EnmFileFormat.MDL)
            {
                driverVirtual = new Driver.MoonDriverDotNET(MoonDriverDotNETim);
                driverVirtual.setting = setting;
                ((Driver.MoonDriverDotNET)driverVirtual).PlayingFileName = PlayingFileName;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MoonDriverDotNET(MoonDriverDotNETim);
                    driverReal.setting = setting;
                    ((Driver.MoonDriverDotNET)driverReal).PlayingFileName = PlayingFileName;
                }

                return MdlPlay_moonDriverDotNET(setting, Driver.MoonDriverDotNET.enmMoonDriverFileType.MDL);
            }

            if (PlayingFileFormat == EnmFileFormat.MDX)
            {
                driverVirtual = new Driver.MXDRV.MXDRV();
                driverVirtual.setting = setting;
                ((Driver.MXDRV.MXDRV)driverVirtual).ExtendFile = (ExtendFile != null && ExtendFile.Count > 0) ? ExtendFile[0] : null;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MXDRV.MXDRV();
                    driverReal.setting = setting;
                    ((Driver.MXDRV.MXDRV)driverReal).ExtendFile = (ExtendFile != null && ExtendFile.Count > 0) ? ExtendFile[0] : null;
                }
                return MdxPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.MND)
            {
                driverVirtual = new Driver.MNDRV.mndrv();
                driverVirtual.setting = setting;

                ((Driver.MNDRV.mndrv)driverVirtual).ExtendFile = ExtendFile;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.MNDRV.mndrv();
                    driverReal.setting = setting;
                    ((Driver.MNDRV.mndrv)driverReal).ExtendFile = ExtendFile;
                }
                return MndPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.XGM)
            {
                driverVirtual = new xgm(setting);
                driverVirtual.setting = setting;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new xgm(setting);
                    driverReal.setting = setting;
                }

                return XgmPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.ZGM)
            {
                driverVirtual = new Driver.ZGM.zgm();
                driverVirtual.setting = setting;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Driver.ZGM.zgm();
                    driverReal.setting = setting;
                }

                return ZgmPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.S98)
            {
                driverVirtual = new S98(setting);
                driverVirtual.setting = setting;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new S98(setting);
                    driverReal.setting = setting;
                }

                return S98Play(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.MID)
            {
                driverVirtual = new MID();
                driverVirtual.setting = setting;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new MID();
                    driverReal.setting = setting;
                }
                return MidPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.RCP)
            {
                driverVirtual = new RCP();
                driverVirtual.setting = setting;
                ((RCP)driverVirtual).ExtendFile = ExtendFile;
                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new RCP();
                    driverReal.setting = setting;
                    ((RCP)driverReal).ExtendFile = ExtendFile;
                }
                return RcpPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.NSF)
            {
                driverVirtual = new nsf(setting)
                {
                    setting = setting
                };
                driverReal = null;
                //if (setting.outputDevice.DeviceType != Common.DEV_Null)
                //{
                //    driverReal = new nsf();
                //    driverReal.setting = setting;
                //}
                return NsfPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.HES)
            {
                driverVirtual = new hes
                {
                    setting = setting
                };

                driverReal = null;
                //if (setting.outputDevice.DeviceType != Common.DEV_Null)
                //{
                //    driverReal = new hes();
                //    driverReal.setting = setting;
                //}
                return HesPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.SID)
            {
                driverVirtual = new Driver.SID.sid
                {
                    setting = setting
                };

                driverReal = null;
                //if (setting.outputDevice.DeviceType != Common.DEV_Null)
                //{
                //    driverReal = new Driver.SID.sid();
                //    driverReal.setting = setting;
                //}
                return SidPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.VGM)
            {
                driverVirtual = new Vgm(setting);
                driverVirtual.setting = setting;
                ((Vgm)driverVirtual).dacControl.chipRegister = chipRegister;
                ((Vgm)driverVirtual).dacControl.model = EnmModel.VirtualModel;


                driverReal = null;
                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    driverReal = new Vgm(setting);
                    driverReal.setting = setting;
                    ((Vgm)driverReal).dacControl.chipRegister = chipRegister;
                    ((Vgm)driverReal).dacControl.model = EnmModel.RealModel;
                }
                return VgmPlay(setting);
            }

            if (PlayingFileFormat == EnmFileFormat.WAV
                || PlayingFileFormat == EnmFileFormat.MP3
                || PlayingFileFormat == EnmFileFormat.AIFF)
            {
                naudioFileReader = new AudioFileReader(naudioFileName);
                naudioWs = new NAudio.Wave.SampleProviders.SampleToWaveProvider16(naudioFileReader);
                return true;
            }

            return false;
        }

        public static bool MgsPlay_mgsdrv(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                int i = 0;
                while (vgmBuf.Length>1 && i < vgmBuf.Length - 1 && (vgmBuf[i] != 0x1a || vgmBuf[i + 1] != 0x00))
                {
                    i++;
                }
                i += 7;
                int[] trkOffsets=new int[18];
                for (int t = 0; t < trkOffsets.Length; t++)
                {
                    trkOffsets[t] = vgmBuf[i + t * 2] + vgmBuf[i + t * 2 + 1] * 0x100;
                }
                bool useAY = (trkOffsets[0] + trkOffsets[1] + trkOffsets[2] != 0);
                bool useSCC = (trkOffsets[3] + trkOffsets[4] + trkOffsets[5] + trkOffsets[6] + trkOffsets[7] != 0);
                bool useOPLL = (trkOffsets[8] + trkOffsets[9] + trkOffsets[10]
                    + trkOffsets[11] + trkOffsets[12] + trkOffsets[13]
                    + trkOffsets[14] + trkOffsets[15] + trkOffsets[16]
                    + trkOffsets[17]
                    != 0);

                chipRegister.resetChips();
                ResetFadeOutParam();
                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                MasterVolume = setting.balance.MasterVolume;

                if (useAY)
                {
                    //ay8910 ay8910 = null;
                    //chip = new MDSound.MDSound.Chip();
                    //ay8910 = new ay8910();
                    //chip.ID = 0;
                    //chipLED.PriAY10 = 1;
                    //chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                    //chip.Instrument = ay8910;
                    //chip.Update = ay8910.Update;
                    //chip.Start = ay8910.Start;
                    //chip.Stop = ay8910.Stop;
                    //chip.Reset = ay8910.Reset;
                    //chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    //chip.Volume = setting.balance.AY8910Volume;
                    //chip.Clock = Driver.MGSDRV.MGSDRV.baseclockAY8910 / 2;
                    //chip.Option = null;
                    //lstChips.Add(chip);
                    //useChip.Add(EnmChip.AY8910);
                    //clockAY8910 = (int)Driver.MGSDRV.MGSDRV.baseclockAY8910;

                    ay8910 ay8910 = null;
                    ay8910_mame ay8910mame = null;
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.AY8910,
                        ID = (byte)0
                    };

                    if ((setting.AY8910Type[0].UseEmu[0] || setting.AY8910Type[0].UseReal[0]))
                    {
                        ay8910 ??= new ay8910();
                        chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                        chip.Instrument = ay8910;
                        chip.Update = ay8910.Update;
                        chip.Start = ay8910.Start;
                        chip.Stop = ay8910.Stop;
                        chip.Reset = ay8910.Reset;
                    }
                    else if ((setting.AY8910Type[0].UseEmu[1]))
                    {
                        ay8910mame??= new ay8910_mame();
                        chip.type = MDSound.MDSound.enmInstrumentType.AY8910mame;
                        chip.Instrument = ay8910mame;
                        chip.Update = ay8910mame.Update;
                        chip.Start = ay8910mame.Start;
                        chip.Stop = ay8910mame.Stop;
                        chip.Reset = ay8910mame.Reset;
                    }

                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.AY8910Volume;
                    chip.Clock = Driver.MGSDRV.MGSDRV.baseclockAY8910 / 2;
                    clockAY8910 = (int)Driver.MGSDRV.MGSDRV.baseclockAY8910;
                    chip.Option = null;

                    chipLED.PriAY10 = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.AY8910);

                }

                if (useOPLL)
                {
                    //ym2413 ym2413 = null;
                    //chip = new MDSound.MDSound.Chip();
                    //ym2413 = new ym2413();
                    //chip.ID = 0;
                    //chipLED.PriOPLL = 1;
                    //chip.type = MDSound.MDSound.enmInstrumentType.YM2413;
                    //chip.Instrument = ym2413;
                    //chip.Update = ym2413.Update;
                    //chip.Start = ym2413.Start;
                    //chip.Stop = ym2413.Stop;
                    //chip.Reset = ym2413.Reset;
                    //chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    //chip.Volume = setting.balance.YM2413Volume;
                    //chip.Clock = Driver.MGSDRV.MGSDRV.baseclockYM2413;
                    //chip.Option = null;
                    //lstChips.Add(chip);
                    //useChip.Add(EnmChip.YM2413);
                    //clockYM2413 = (int)Driver.MGSDRV.MGSDRV.baseclockYM2413;

                    MDSound.emu2413 ym2413 = null;
                    chip = new MDSound.MDSound.Chip();
                    ym2413 = new MDSound.emu2413();
                    chip.ID = 0;
                    chipLED.PriOPLL = 1;
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2413emu;
                    chip.Instrument = ym2413;
                    chip.Update = ym2413.Update;
                    chip.Start = ym2413.Start;
                    chip.Stop = ym2413.Stop;
                    chip.Reset = ym2413.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.YM2413Volume;
                    chip.Clock = Driver.MGSDRV.MGSDRV.baseclockYM2413;
                    chip.Option = null;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YM2413);
                    clockYM2413 = (int)Driver.MGSDRV.MGSDRV.baseclockYM2413;
                }

                if (useSCC)
                {
                    K051649 K051649 = null;
                    chip = new MDSound.MDSound.Chip();
                    K051649 = new K051649();
                    chip.ID = 0;
                    chipLED.PriK051649 = 1;
                    chip.type = MDSound.MDSound.enmInstrumentType.K051649;
                    chip.Instrument = K051649;
                    chip.Update = K051649.Update;
                    chip.Start = K051649.Start;
                    chip.Stop = K051649.Stop;
                    chip.Reset = K051649.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.K051649Volume;
                    chip.Clock = Driver.MGSDRV.MGSDRV.baseclockK051649;
                    chip.Option = null;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.K051649);
                    clockK051649 = (int)Driver.MGSDRV.MGSDRV.baseclockK051649;
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                if (useOPLL)
                {
                    chipRegister.setYM2413Register(0, 14, 32, EnmModel.VirtualModel);
                    //chipRegister.setYM2610Register(0, 0, 0x11, 0xc0, EnmModel.VirtualModel);
                }

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.AY8910, EnmChip.YM2413, EnmChip.K051649 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.AY8910 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                //Play

                Paused = false;
                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MscPlay_mscdrv(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                int[] trkOffsets = new int[17];
                for (int t = 0; t < trkOffsets.Length; t++)
                {
                    trkOffsets[t] = vgmBuf[8 + t * 2] + vgmBuf[9 + t * 2] * 0x100;
                }
                bool useAY = ((trkOffsets[9] + trkOffsets[10] + trkOffsets[11]) != 0);
                bool useSCC = ((trkOffsets[12] + trkOffsets[13] + trkOffsets[14] + trkOffsets[15] + trkOffsets[16]) != 0);
                bool useOPLL = ((trkOffsets[0] + trkOffsets[1] + trkOffsets[2]
                    + trkOffsets[3] + trkOffsets[4] + trkOffsets[5]
                    + trkOffsets[6] + trkOffsets[7] + trkOffsets[8]
                    ) != 0);

                chipRegister.resetChips();
                ResetFadeOutParam();
                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                MasterVolume = setting.balance.MasterVolume;

                if (useAY)
                {
                    ay8910 ay8910 = null;
                    ay8910_mame ay8910mame = null;
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.AY8910,
                        ID = (byte)0
                    };

                    if ((setting.AY8910Type[0].UseEmu[0] || setting.AY8910Type[0].UseReal[0]))
                    {
                        ay8910 ??= new ay8910();
                        chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                        chip.Instrument = ay8910;
                        chip.Update = ay8910.Update;
                        chip.Start = ay8910.Start;
                        chip.Stop = ay8910.Stop;
                        chip.Reset = ay8910.Reset;
                    }
                    else if ((setting.AY8910Type[0].UseEmu[1]))
                    {
                        ay8910mame ??= new ay8910_mame();
                        chip.type = MDSound.MDSound.enmInstrumentType.AY8910mame;
                        chip.Instrument = ay8910mame;
                        chip.Update = ay8910mame.Update;
                        chip.Start = ay8910mame.Start;
                        chip.Stop = ay8910mame.Stop;
                        chip.Reset = ay8910mame.Reset;
                    }

                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.AY8910Volume;
                    chip.Clock = Driver.MuSICA.MuSICA.baseclockAY8910 / 2;
                    clockAY8910 = (int)Driver.MuSICA.MuSICA.baseclockAY8910;
                    chip.Option = null;

                    chipLED.PriAY10 = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.AY8910);

                }

                if (useOPLL)
                {
                    MDSound.emu2413 ym2413 = null;
                    chip = new MDSound.MDSound.Chip();
                    ym2413 = new MDSound.emu2413();
                    chip.ID = 0;
                    chipLED.PriOPLL = 1;
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2413emu;
                    chip.Instrument = ym2413;
                    chip.Update = ym2413.Update;
                    chip.Start = ym2413.Start;
                    chip.Stop = ym2413.Stop;
                    chip.Reset = ym2413.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.YM2413Volume;
                    chip.Clock = Driver.MuSICA.MuSICA.baseclockYM2413;
                    chip.Option = null;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YM2413);
                    clockYM2413 = (int)Driver.MuSICA.MuSICA.baseclockYM2413;
                }

                if (useSCC)
                {
                    K051649 K051649 = null;
                    chip = new MDSound.MDSound.Chip();
                    K051649 = new K051649();
                    chip.ID = 0;
                    chipLED.PriK051649 = 1;
                    chip.type = MDSound.MDSound.enmInstrumentType.K051649;
                    chip.Instrument = K051649;
                    chip.Update = K051649.Update;
                    chip.Start = K051649.Start;
                    chip.Stop = K051649.Stop;
                    chip.Reset = K051649.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.K051649Volume;
                    chip.Clock = Driver.MuSICA.MuSICA.baseclockK051649;
                    chip.Option = null;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.K051649);
                    clockK051649 = (int)Driver.MuSICA.MuSICA.baseclockK051649;
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                if (useOPLL)
                {
                    chipRegister.setYM2413Register(0, 14, 32, EnmModel.VirtualModel);
                }

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.AY8910, EnmChip.YM2413, EnmChip.K051649 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.AY8910 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                //Play

                Paused = false;
                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MucPlay_mucomDotNET(Setting setting,Driver.MucomDotNET.enmMUCOMFileType fileType)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                if (fileType == Driver.MucomDotNET.enmMUCOMFileType.MUC)
                {
                    vgmBuf = ((Driver.MucomDotNET)driverVirtual).Compile(vgmBuf);
                }
                if (vgmBuf == null) return false;
                EnmChip[] useChipFromMub = ((Driver.MucomDotNET)driverVirtual).useChipsFromMub(vgmBuf);
                ((Driver.MucomDotNET)driverVirtual).GetMUBTAGOption(vgmBuf);

                //Stop();
                chipRegister.resetChips();
                ResetFadeOutParam();
                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                MasterVolume = setting.balance.MasterVolume;

                ym2608 ym2608 = null;
                ym2608 = new ym2608();
                ym2610 ym2610 = null;
                ym2610 = new ym2610();
                ym2151 ym2151 = null;
                ym2151 = new ym2151();
                Func<string, Stream> fn = Common.GetOPNARyhthmStream;

                if (useChipFromMub[0] != EnmChip.Unuse)
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        ID = 0,
                        type = MDSound.MDSound.enmInstrumentType.YM2608,
                        Instrument = ym2608,
                        Update = ym2608.Update,
                        Start = ym2608.Start,
                        Stop = ym2608.Stop,
                        Reset = ym2608.Reset,
                        SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                        Volume = setting.balance.YM2608Volume,
                        Clock = Driver.MucomDotNET.OPNAbaseclock,
                        Option = new object[] { fn }
                    };
                    chipLED.PriOPNA = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YM2608);
                    clockYM2608 = Driver.MucomDotNET.OPNAbaseclock;
                }

                if (useChipFromMub[1] != EnmChip.Unuse)
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        ID = 1,
                        type = MDSound.MDSound.enmInstrumentType.YM2608,
                        Instrument = ym2608,
                        Update = ym2608.Update,
                        Start = ym2608.Start,
                        Stop = ym2608.Stop,
                        Reset = ym2608.Reset,
                        SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                        Volume = setting.balance.YM2608Volume,
                        Clock = Driver.MucomDotNET.OPNAbaseclock,
                        Option = new object[] { fn }
                    };
                    chipLED.SecOPNA = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.S_YM2608);
                }

                if (useChipFromMub[2] != EnmChip.Unuse)
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        ID = 0,
                        type = MDSound.MDSound.enmInstrumentType.YM2610,
                        Instrument = ym2610,
                        Update = ym2610.Update,
                        Start = ym2610.Start,
                        Stop = ym2610.Stop,
                        Reset = ym2610.Reset,
                        SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                        Volume = setting.balance.YM2610Volume,
                        Clock = Driver.MucomDotNET.OPNBbaseclock,
                        Option = null
                    };
                    chipLED.PriOPNB = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YM2610);
                    clockYM2610 = Driver.MucomDotNET.OPNBbaseclock;
                }

                if (useChipFromMub[3] != EnmChip.Unuse)
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        ID = 1,
                        type = MDSound.MDSound.enmInstrumentType.YM2610,
                        Instrument = ym2610,
                        Update = ym2610.Update,
                        Start = ym2610.Start,
                        Stop = ym2610.Stop,
                        Reset = ym2610.Reset,
                        SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                        Volume = setting.balance.YM2610Volume,
                        Clock = Driver.MucomDotNET.OPNBbaseclock,
                        Option = null
                    };
                    chipLED.SecOPNB = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.S_YM2610);
                }

                if (useChipFromMub[4] != EnmChip.Unuse)
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        ID = 0,
                        type = MDSound.MDSound.enmInstrumentType.YM2151,
                        Instrument = ym2151,
                        Update = ym2151.Update,
                        Start = ym2151.Start,
                        Stop = ym2151.Stop,
                        Reset = ym2151.Reset,
                        Volume = setting.balance.YM2151Volume
                    };
                    uint clock = (uint)((Driver.MucomDotNET)driverVirtual).OPMClock;
                    if (clock == 0) clock = Driver.MucomDotNET.OPMbaseclock;
                    chip.Clock = clock;
                    chip.SamplingRate = (UInt32)chip.Clock / 64;// (UInt32)setting.outputDevice.SampleRate;
                    chip.Option = null;
                    chipLED.PriOPM = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YM2151);
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                SetYM2608Volume(true, setting.balance.YM2608Volume);
                SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);

                chipRegister.setYM2608Register(0, 0, 0x2d, 0x00, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x2d, 0x00, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.RealModel);
                chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x07, 0x38, EnmModel.VirtualModel); //PSG TONE でリセット
                chipRegister.setYM2608Register(0, 0, 0x07, 0x38, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x08, 0x00, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x08, 0x00, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x09, 0x00, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x09, 0x00, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x0a, 0x00, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x0a, 0x00, EnmModel.RealModel);

                chipRegister.writeYM2608Clock(0, Driver.MucomDotNET.OPNAbaseclock, EnmModel.RealModel);
                chipRegister.writeYM2608Clock(1, Driver.MucomDotNET.OPNAbaseclock, EnmModel.RealModel);
                chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, EnmModel.RealModel);


                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.YM2608 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.YM2608 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                if (((Driver.MucomDotNET)driverVirtual).SSGExtend)
                {
                    try { mds.ChangeYM2608_PSGMode(0, 1); } catch { }
                    try { mds.ChangeYM2608_PSGMode(1, 1); } catch { }
                    try { mds.ChangeYM2610_PSGMode(0, 1); } catch { }
                    try { mds.ChangeYM2610_PSGMode(1, 1); } catch { }
                }

                //Play

                Paused = false;

                if (driverReal != null && setting.YM2608Type[0].UseReal[0])
                {
                    realChip.WaitOPNADPCMData(setting.YM2608Type[0].realChipInfo[0].SoundLocation == -1);
                }

                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MmlPlay_PMDDotNET(Setting setting,int fileType)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();
                ResetFadeOutParam();
                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                MasterVolume = setting.balance.MasterVolume;

                ym2608 ym2608 = null;
                ym2608 = new ym2608();
                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.YM2608,
                    Instrument = ym2608,
                    Update = ym2608.Update,
                    Start = ym2608.Start,
                    Stop = ym2608.Stop,
                    Reset = ym2608.Reset,
                    SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                    Volume = setting.balance.YM2608Volume,
                    Clock = Driver.PMDDotNET.baseclock
                };
                Func<string, Stream> fn = Common.GetOPNARyhthmStream;
                chip.Option = new object[] { fn };
                chipLED.PriOPNA = 1;
                lstChips.Add(chip);
                useChip.Add(EnmChip.YM2608);
                clockYM2608 = Driver.PMDDotNET.baseclock;

                MDSound.PPZ8 ppz8 = null;
                ppz8 = new MDSound.PPZ8();
                chip = new MDSound.MDSound.Chip
                {
                    ID = (byte)0,
                    type = MDSound.MDSound.enmInstrumentType.PPZ8,
                    Instrument = ppz8,
                    Update = ppz8.Update,
                    Start = ppz8.Start,
                    Stop = ppz8.Stop,
                    Reset = ppz8.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Volume = setting.balance.PPZ8Volume,
                    Clock = Driver.PMDDotNET.baseclock,
                    Option = null
                };
                chipLED.PriPPZ8 = 1;
                lstChips.Add(chip);
                useChip.Add(EnmChip.PPZ8);


                MDSound.PPSDRV ppsdrv = null;
                ppsdrv = new PPSDRV();
                chip = new MDSound.MDSound.Chip
                {
                    ID = (byte)0,
                    type = MDSound.MDSound.enmInstrumentType.PPSDRV,
                    Instrument = ppsdrv,
                    Update = ppsdrv.Update,
                    Start = ppsdrv.Start,
                    Stop = ppsdrv.Stop,
                    Reset = ppsdrv.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Volume = 0,// setting.balance.PPZ8Volume;
                    Clock = Driver.PMDDotNET.baseclock,
                    Option = null
                };
                chipLED.PriPPSDRV = 1;
                lstChips.Add(chip);
                useChip.Add(EnmChip.PPSDRV);


                MDSound.P86 P86 = null;
                P86 = new MDSound.P86();
                chip = new MDSound.MDSound.Chip
                {
                    ID = (byte)0,
                    type = MDSound.MDSound.enmInstrumentType.P86,
                    Instrument = P86,
                    Update = P86.Update,
                    Start = P86.Start,
                    Stop = P86.Stop,
                    Reset = P86.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Volume = 0,// setting.balance.P86Volume;
                    Clock = Driver.PMDDotNET.baseclock,
                    Option = null
                };
                chipLED.PriP86 = 1;
                lstChips.Add(chip);
                useChip.Add(EnmChip.P86);





                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                SetYM2608Volume(true, setting.balance.YM2608Volume);
                SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);

                chipRegister.setYM2608Register(0, 0, 0x2d, 0x00, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x2d, 0x00, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.RealModel);
                chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.VirtualModel);
                chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.RealModel);
                chipRegister.setYM2608Register(0, 0, 0x07, 0x38, EnmModel.VirtualModel); //PSG TONE でリセット
                chipRegister.setYM2608Register(0, 0, 0x07, 0x38, EnmModel.RealModel);

                chipRegister.writeYM2608Clock(0, Driver.PMDDotNET.baseclock, EnmModel.RealModel);
                chipRegister.writeYM2608Clock(1, Driver.PMDDotNET.baseclock, EnmModel.RealModel);
                chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, EnmModel.RealModel);


                if (!driverVirtual.init(vgmBuf, fileType, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.YM2608 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, fileType, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.YM2608 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                //Play

                Paused = false;

                if (driverReal != null && setting.YM2608Type[0].UseReal[0])
                {
                    realChip.WaitOPNADPCMData(setting.YM2608Type[0].realChipInfo[0].SoundLocation == -1);
                }

                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MdlPlay_moonDriverDotNET(Setting setting, Driver.MoonDriverDotNET.enmMoonDriverFileType fileType)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                if (fileType == Driver.MoonDriverDotNET.enmMoonDriverFileType.MDL)
                {
                    vgmBuf = ((Driver.MoonDriverDotNET)driverVirtual).Compile(vgmBuf);
                }

                //Stop();
                chipRegister.resetChips();
                ResetFadeOutParam();
                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;

                chipLED = new ChipLEDs();
                MasterVolume = setting.balance.MasterVolume;
                byte sndgen = vgmBuf[7];
                bool EX_OPL3 = ((sndgen & 2) != 0);
                bool OPL4_NOUSE = ((sndgen & 1) == 0);
                EnmChip[] useChipFromMdr = new EnmChip[] { EnmChip.YMF278B };

                if (OPL4_NOUSE && !EX_OPL3)
                {
                    errMsg = "OPL4_NOUSEとEX_OPL3の組み合わせが不正です。";
                    return false;
                }

                if (EX_OPL3 && OPL4_NOUSE)
                {
                    MDSound.ymf262 ymf262 = new();

                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YMF262,
                        ID = 0,
                        Instrument = ymf262,
                        Update = ymf262.Update,
                        Start = ymf262.Start,
                        Stop = ymf262.Stop,
                        Reset = ymf262.Reset,
                        SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                        Volume = setting.balance.YMF262Volume,
                        Clock = 14318180,
                        Option = new object[] { Common.GetApplicationFolder() }
                    };

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriOPL3 = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YMF262);
                    useChipFromMdr[0] = EnmChip.YMF262;
                }
                else
                {
                    MDSound.ymf278b ymf278b = new();

                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YMF278B,
                        ID = 0,
                        Instrument = ymf278b,
                        Update = ymf278b.Update,
                        Start = ymf278b.Start,
                        Stop = ymf278b.Stop,
                        Reset = ymf278b.Reset,
                        SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                        Volume = setting.balance.YMF278BVolume,
                        Clock = 33868800,
                        Option = new object[] { Common.GetApplicationFolder() }
                    };

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriOPL4 = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YMF278B);
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                if (EX_OPL3 && OPL4_NOUSE) SetYMF262Volume(true, setting.balance.YMF262Volume);
                else SetYMF278BVolume(true, setting.balance.YMF278BVolume);

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, useChipFromMdr
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, useChipFromMdr
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                //Play

                Paused = false;
                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        //public static bool mdrPlay(Setting setting)
        //{

        //    try
        //    {

        //        if (vgmBuf == null || setting == null) return false;

        //        //Stop();

        //        //int r = ((NRTDRV)driverVirtual).checkUseChip(vgmBuf);

        //        chipRegister.setFadeoutVolYM2151(0, 0);
        //        chipRegister.setFadeoutVolYM2151(1, 0);

        //        chipRegister.resetChips();
        //        ResetFadeOutParam();
        //        useChip.Clear();

        //        vgmFadeout = false;
        //        vgmFadeoutCounter = 1.0;
        //        vgmFadeoutCounterV = 0.00001;
        //        vgmSpeed = 1;
        //        vgmRealFadeoutVol = 0;
        //        vgmRealFadeoutVolWait = 4;

        //        ClearFadeoutVolume();

        //        chipRegister.resetChips();

        //        startTrdVgmReal();

        //        List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();
        //        MDSound.MDSound.Chip chip;

        //        hiyorimiNecessary = setting.HiyorimiMode;
        //        int hiyorimiDeviceFlag = 0;

        //        chipLED = new ChipLEDs();
        //        MasterVolume = setting.balance.MasterVolume;
        //        byte sndgen = vgmBuf[7];
        //        bool EX_OPL3 = ((sndgen & 2) != 0);
        //        bool OPL4_NOUSE = ((sndgen & 1) == 0);

        //        if (EX_OPL3 && OPL4_NOUSE)
        //        {
        //            MDSound.ymf262 ymf262 = new ymf262();

        //            chip = new MDSound.MDSound.Chip();
        //            chip.type = MDSound.MDSound.enmInstrumentType.YMF262;
        //            chip.ID = 0;
        //            chip.Instrument = ymf262;
        //            chip.Update = ymf262.Update;
        //            chip.Start = ymf262.Start;
        //            chip.Stop = ymf262.Stop;
        //            chip.Reset = ymf262.Reset;
        //            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
        //            chip.Volume = setting.balance.YMF262Volume;
        //            chip.Clock = 14318180;
        //            chip.Option = new object[] { Common.GetApplicationFolder() };

        //            hiyorimiDeviceFlag |= 0x2;

        //            chipLED.PriOPL3 = 1;

        //            lstChips.Add(chip);
        //            useChip.Add(EnmChip.YMF262);
        //        }
        //        else
        //        {
        //            MDSound.ymf278b ymf278b = new MDSound.ymf278b();

        //            chip = new MDSound.MDSound.Chip();
        //            chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
        //            chip.ID = 0;
        //            chip.Instrument = ymf278b;
        //            chip.Update = ymf278b.Update;
        //            chip.Start = ymf278b.Start;
        //            chip.Stop = ymf278b.Stop;
        //            chip.Reset = ymf278b.Reset;
        //            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
        //            chip.Volume = setting.balance.YMF278BVolume;
        //            chip.Clock = 33868800;
        //            chip.Option = new object[] { Common.GetApplicationFolder() };

        //            hiyorimiDeviceFlag |= 0x2;

        //            chipLED.PriOPL4 = 1;

        //            lstChips.Add(chip);
        //            useChip.Add(EnmChip.YMF278B);
        //        }

        //        if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
        //        else hiyorimiNecessary = false;

        //        if (mds == null)
        //            mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
        //        else
        //            mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

        //        chipRegister.initChipRegister(lstChips.ToArray());

        //        if (EX_OPL3 && OPL4_NOUSE) SetYMF262Volume(true, setting.balance.YMF262Volume);
        //        else SetYMF278BVolume(true, setting.balance.YMF278BVolume);

        //        ((MDPlayer.Driver.MoonDriver.MoonDriver)driverVirtual).isOPL3 = EX_OPL3;
        //        ((MDPlayer.Driver.MoonDriver.MoonDriver)driverReal).isOPL3 = EX_OPL3;

        //        driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
        //            , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
        //            , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000));
        //        if (driverReal != null)
        //        {
        //            driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
        //                , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
        //                , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000));
        //        }

        //        Paused = false;
        //        oneTimeReset = false;

        //        //Thread.Sleep(500);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.ForcedWrite(ex);
        //        return false;
        //    }

        //}

        public static bool NrdPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                int r = ((NRTDRV)driverVirtual).checkUseChip(vgmBuf);

                if (!setting.debug.debugOPZ)
                {
                    chipRegister.setFadeoutVolYM2151(0, 0);
                    chipRegister.setFadeoutVolYM2151(1, 0);

                    chipRegister.resetChips();
                }

                //Realも含めた使用するChips
                useChip.Clear();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                if (!setting.debug.debugOPZ)
                {
                    ClearFadeoutVolume();
                    chipRegister.resetChips();
                }
                StartTrdVgmReal();

                //emuするChips
                List<MDSound.MDSound.Chip> lstChips = new();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                MDSound.ym2151 ym2151 = null;
                MDSound.ym2151_mame ym2151_mame = null;
                MDSound.ym2151_x68sound ym2151_x68sound = null;
                for (int i = 0; i < 2; i++)
                {
                    if ((i == 0 && (r & 0x3) != 0) || (i == 1 && (r & 0x2) != 0))
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            ID = (byte)i
                        };

                        if ((i == 0 && setting.YM2151Type[0].UseEmu[0]) || (i == 1 && setting.YM2151Type[1].UseEmu[0]))
                        {
                            ym2151 ??= new MDSound.ym2151();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                            chip.Instrument = ym2151;
                            chip.Update = ym2151.Update;
                            chip.Start = ym2151.Start;
                            chip.Stop = ym2151.Stop;
                            chip.Reset = ym2151.Reset;
                        }
                        else if ((i == 0 && setting.YM2151Type[0].UseEmu[1]) || (i == 1 && setting.YM2151Type[1].UseEmu[1]))
                        {
                            ym2151_mame ??= new MDSound.ym2151_mame();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151mame;
                            chip.Instrument = ym2151_mame;
                            chip.Update = ym2151_mame.Update;
                            chip.Start = ym2151_mame.Start;
                            chip.Stop = ym2151_mame.Stop;
                            chip.Reset = ym2151_mame.Reset;
                        }
                        else if ((i == 0 && setting.YM2151Type[0].UseEmu[2]) || (i == 1 && setting.YM2151Type[1].UseEmu[2]))
                        {
                            ym2151_x68sound ??= new MDSound.ym2151_x68sound();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151x68sound;
                            chip.Instrument = ym2151_x68sound;
                            chip.Update = ym2151_x68sound.Update;
                            chip.Start = ym2151_x68sound.Start;
                            chip.Stop = ym2151_x68sound.Stop;
                            chip.Reset = ym2151_x68sound.Reset;
                        }

                        chip.Volume = setting.balance.YM2151Volume;
                        chip.Clock = 4000000;
                        chip.SamplingRate = (UInt32)chip.Clock / 64;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPM = 1;
                        else chipLED.SecOPM = 1;

                        if (chip.Start != null) lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM2151 : EnmChip.S_YM2151);
                    }
                }

                if ((r & 0x4) != 0)
                {
                    MDSound.ay8910 ay8910 = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.AY8910,
                        ID = (byte)0,
                        Instrument = ay8910,
                        Update = ay8910.Update,
                        Start = ay8910.Start,
                        Stop = ay8910.Stop,
                        Reset = ay8910.Reset,
                        SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                        Volume = setting.balance.AY8910Volume,
                        Clock = 2000000 / 2
                    };
                    clockAY8910 = (int)chip.Clock;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x1;
                    chipLED.PriAY10 = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.AY8910);
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                if (useChip.Contains(EnmChip.YM2151) || useChip.Contains(EnmChip.S_YM2151))
                    SetYM2151Volume(true, setting.balance.YM2151Volume);
                if (useChip.Contains(EnmChip.AY8910))
                    SetAY8910Volume(true, setting.balance.AY8910Volume);

                int chipClock = 4000000;
                if (setting.debug.debugOPZ)
                {
                    chipClock = 3579545;
                }

                if (useChip.Contains(EnmChip.YM2151))
                    chipRegister.writeYM2151Clock(0, chipClock, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2151))
                    chipRegister.writeYM2151Clock(1, chipClock, EnmModel.RealModel);

                driverVirtual?.SetYM2151Hosei(chipClock);
                driverReal?.SetYM2151Hosei(chipClock);
                //chipRegister.setYM2203SSGVolume(0, setting.balance.GimicOPNVolume, enmModel.RealModel);
                //chipRegister.setYM2203SSGVolume(1, setting.balance.GimicOPNVolume, enmModel.RealModel);
                //chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, enmModel.RealModel);
                //chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, enmModel.RealModel);


                if (driverVirtual != null)
                {
                    driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.YM2151, EnmChip.AY8910 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000));
                    ((NRTDRV)driverVirtual).Call(0);//
                }

                if (driverReal != null)
                {
                    driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.YM2151, EnmChip.AY8910 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000));
                    ((NRTDRV)driverReal).Call(0);//
                }


                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                ((NRTDRV)driverVirtual).Call(1);//MPLAY

                if (driverReal != null)
                {
                    ((NRTDRV)driverReal).Call(1);//MPLAY
                }



                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MdxPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;
                if (setting.outputDevice.SampleRate != 44100)
                {
                    return false;
                }
                //Stop();

                chipRegister.resetChips();
                useChip.Clear();
                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                StartTrdVgmReal();

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 3;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip = null;

                if (setting.YM2151Type[0].UseEmu[0])
                {
                    MDSound.ym2151 ym2151 = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2151,
                        ID = (byte)0,
                        Instrument = ym2151,
                        Update = ym2151.Update,
                        Start = ym2151.Start,
                        Stop = ym2151.Stop,
                        Reset = ym2151.Reset,
                        Volume = setting.balance.YM2151Volume,
                        Clock = 4000000
                    };
                    chip.SamplingRate = (UInt32)chip.Clock / 64;
                    chip.Option = null;
                }
                else if (setting.YM2151Type[0].UseEmu[1])
                {
                    MDSound.ym2151_mame ym2151mame = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2151mame,
                        ID = (byte)0,
                        Instrument = ym2151mame,
                        Update = ym2151mame.Update,
                        Start = ym2151mame.Start,
                        Stop = ym2151mame.Stop,
                        Reset = ym2151mame.Reset,
                        Volume = setting.balance.YM2151Volume
                    };
                    chip.SamplingRate = (UInt32)chip.Clock / 64;
                    chip.Clock = 4000000;
                    chip.Option = null;
                }
                else if (setting.YM2151Type[0].UseEmu[2])
                {
                    MDSound.ym2151_x68sound mdxOPM = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2151x68sound,
                        ID = (byte)0,
                        Instrument = mdxOPM,
                        Update = mdxOPM.Update,
                        Start = mdxOPM.Start,
                        Stop = mdxOPM.Stop,
                        Reset = mdxOPM.Reset,
                        Volume = setting.balance.YM2151Volume,
                        Clock = 4000000
                    };
                    chip.SamplingRate = (UInt32)chip.Clock / 64;
                    chip.Option = new object[3] { 1, 0, 0 };
                }
                if (chip != null)
                {
                    lstChips.Add(chip);
                }
                useChip.Add(EnmChip.YM2151);

                MDSound.ym2151_x68sound mdxPCM_V = new();
                mdxPCM_V.x68sound[0] = new MDSound.NX68Sound.X68Sound();
                mdxPCM_V.sound_Iocs[0] = new MDSound.NX68Sound.sound_iocs(mdxPCM_V.x68sound[0]);
                MDSound.ym2151_x68sound mdxPCM_R = new();
                mdxPCM_R.x68sound[0] = new MDSound.NX68Sound.X68Sound();
                mdxPCM_R.sound_Iocs[0] = new MDSound.NX68Sound.sound_iocs(mdxPCM_R.x68sound[0]);
                useChip.Add(EnmChip.OKIM6258);

                chipLED.PriOPM = 1;
                chipLED.PriOKI5 = 1;


                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                SetYM2151Volume(true, setting.balance.YM2151Volume);


                if (useChip.Contains(EnmChip.YM2151))
                    chipRegister.writeYM2151Clock(0, 4000000, EnmModel.RealModel);
                chipRegister.use4MYM2151scci[0] = false;
                if (setting.YM2151Type[0].UseRealChipFreqDiff != null
                    && setting.YM2151Type[0].UseRealChipFreqDiff.Length > 0
                    && setting.YM2151Type[0].UseRealChipFreqDiff[0])
                {
                    chipRegister.use4MYM2151scci[0] = true;
                }
                driverVirtual.SetYM2151Hosei(4000000);
                driverReal?.SetYM2151Hosei(4000000);

                //chipRegister.setYM2203SSGVolume(0, setting.balance.GimicOPNVolume, enmModel.RealModel);
                //chipRegister.setYM2203SSGVolume(1, setting.balance.GimicOPNVolume, enmModel.RealModel);
                //chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, enmModel.RealModel);
                //chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, enmModel.RealModel);

                bool retV = ((MDPlayer.Driver.MXDRV.MXDRV)driverVirtual).init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , mdxPCM_V);
                bool retR = true;
                if (driverReal != null)
                {
                    retR = ((MDPlayer.Driver.MXDRV.MXDRV)driverReal).init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)
                        , mdxPCM_R);
                }

                if (!retV || !retR)
                {
                    errMsg = driverVirtual.errMsg != "" ? driverVirtual.errMsg : (driverReal != null ? driverReal.errMsg : "");
                    return false;
                }

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MndPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                StartTrdVgmReal();

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 3;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                List<MDSound.MDSound.Chip> lstChips = new();
                MDSound.MDSound.Chip chip = null;

                if (setting.YM2151Type[0].UseEmu[0])
                {
                    MDSound.ym2151 ym2151 = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2151,
                        ID = (byte)0,
                        Instrument = ym2151,
                        Update = ym2151.Update,
                        Start = ym2151.Start,
                        Stop = ym2151.Stop,
                        Reset = ym2151.Reset,
                        Volume = setting.balance.YM2151Volume,
                        Clock = 4000000
                    };
                    chip.SamplingRate = (UInt32)chip.Clock / 64;
                    chip.Option = null;
                }
                else if (setting.YM2151Type[0].UseEmu[1])
                {
                    MDSound.ym2151_mame ym2151mame = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2151mame,
                        ID = (byte)0,
                        Instrument = ym2151mame,
                        Update = ym2151mame.Update,
                        Start = ym2151mame.Start,
                        Stop = ym2151mame.Stop,
                        Reset = ym2151mame.Reset,
                        Volume = setting.balance.YM2151Volume,
                        Clock = 4000000
                    };
                    chip.SamplingRate = (UInt32)chip.Clock / 64;
                    chip.Option = null;
                }
                else if (setting.YM2151Type[0].UseEmu[2])
                {
                    MDSound.ym2151_x68sound mdxOPM = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2151x68sound,
                        ID = (byte)0,
                        Instrument = mdxOPM,
                        Update = mdxOPM.Update,
                        Start = mdxOPM.Start,
                        Stop = mdxOPM.Stop,
                        Reset = mdxOPM.Reset,
                        Volume = setting.balance.YM2151Volume,
                        Clock = 4000000
                    };
                    chip.SamplingRate = (UInt32)chip.Clock / 64;
                    chip.Option = new object[3] { 1, 0, 0 };
                }
                if (chip != null)
                {
                    lstChips.Add(chip);
                }
                useChip.Add(EnmChip.YM2151);

                MDSound.ym2608 opna = new();
                if (setting.YM2608Type[0].UseEmu[0])
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2608,
                        ID = (byte)0,
                        Instrument = opna,
                        Update = opna.Update,
                        Start = opna.Start,
                        Stop = opna.Stop,
                        Reset = opna.Reset,
                        SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                        Volume = setting.balance.YM2608Volume,
                        Clock = 8000000// 7987200;
                    };
                    Func<string, Stream> fn = Common.GetOPNARyhthmStream;
                    chip.Option = new object[] { fn };
                    lstChips.Add(chip);
                    clockYM2608 = 8000000;
                }
                useChip.Add(EnmChip.YM2608);

                if (setting.YM2608Type[1].UseEmu[0])
                {
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.YM2608,
                        ID = (byte)1,
                        Instrument = opna,
                        Update = opna.Update,
                        Start = opna.Start,
                        Stop = opna.Stop,
                        Reset = opna.Reset,
                        SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                        Volume = setting.balance.YM2608Volume,
                        Clock = 8000000,// 7987200;
                        Option = new object[] { Common.GetApplicationFolder() }
                    };
                    lstChips.Add(chip);
                    clockYM2608 = 8000000;
                }
                useChip.Add(EnmChip.S_YM2608);

                MDSound.mpcmX68k mpcm = new();
                chip = new MDSound.MDSound.Chip
                {
                    type = MDSound.MDSound.enmInstrumentType.mpcmX68k,
                    ID = (byte)0,
                    Instrument = mpcm,
                    Update = mpcm.Update,
                    Start = mpcm.Start,
                    Stop = mpcm.Stop,
                    Reset = mpcm.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Volume = setting.balance.OKIM6258Volume,
                    Clock = 15600,
                    Option = new object[] { Common.GetApplicationFolder() }
                };
                lstChips.Add(chip);
                useChip.Add(EnmChip.OKIM6258);

                chipLED.PriOPM = 1;
                chipLED.PriOPNA = 1;
                chipLED.SecOPNA = 1;
                chipLED.PriOKI5 = 1;

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                if (useChip.Contains(EnmChip.YM2151) || useChip.Contains(EnmChip.S_YM2151))
                    SetYM2151Volume(true, setting.balance.YM2151Volume);

                if (useChip.Contains(EnmChip.YM2608) || useChip.Contains(EnmChip.S_YM2608))
                {
                    SetYM2608Volume(true, setting.balance.YM2608Volume);
                    SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                    SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                    SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                    SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);
                }

                Thread.Sleep(500);

                if (useChip.Contains(EnmChip.YM2608))
                {
                    chipRegister.setYM2608Register(0, 0, 0x2d, 0x00, EnmModel.VirtualModel);
                    chipRegister.setYM2608Register(0, 0, 0x2d, 0x00, EnmModel.RealModel);
                    chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.VirtualModel);
                    chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.RealModel);
                    chipRegister.setYM2608Register(0, 0, 0x07, 0x38, EnmModel.VirtualModel); //PSG TONE でリセット
                    chipRegister.setYM2608Register(0, 0, 0x07, 0x38, EnmModel.RealModel);
                    chipRegister.writeYM2608Clock(0, 8000000, EnmModel.RealModel);
                    chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                }

                if (useChip.Contains(EnmChip.S_YM2608))
                {
                    chipRegister.setYM2608Register(1, 0, 0x2d, 0x00, EnmModel.VirtualModel);
                    chipRegister.setYM2608Register(1, 0, 0x2d, 0x00, EnmModel.RealModel);
                    chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.VirtualModel);
                    chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.RealModel);
                    chipRegister.setYM2608Register(1, 0, 0x07, 0x38, EnmModel.VirtualModel); //PSG TONE でリセット
                    chipRegister.setYM2608Register(1, 0, 0x07, 0x38, EnmModel.RealModel);
                    chipRegister.writeYM2608Clock(1, 8000000, EnmModel.RealModel);
                    chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                }

                if (useChip.Contains(EnmChip.YM2151))
                    chipRegister.writeYM2151Clock(0, 4000000, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2151))
                    chipRegister.writeYM2151Clock(1, 4000000, EnmModel.RealModel);

                driverVirtual.SetYM2151Hosei(4000000);
                driverReal?.SetYM2151Hosei(4000000);

                if (useChip.Contains(EnmChip.YM2203))
                    chipRegister.setYM2203SSGVolume(0, setting.balance.GimicOPNVolume, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2203))
                    chipRegister.setYM2203SSGVolume(1, setting.balance.GimicOPNVolume, EnmModel.RealModel);

                bool retV = ((MDPlayer.Driver.MNDRV.mndrv)driverVirtual).init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.YM2151, EnmChip.YM2608 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)
                    );
                bool retR = true;
                if (driverReal != null)
                {
                    retR = ((MDPlayer.Driver.MNDRV.mndrv)driverReal).init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.YM2151, EnmChip.YM2608 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)
                        );
                }

                if (!retV || !retR)
                {
                    errMsg = driverVirtual.errMsg != "" ? driverVirtual.errMsg : (driverReal != null ? driverReal.errMsg : "");
                    return false;
                }

                ((MDPlayer.Driver.MNDRV.mndrv)driverVirtual).m_MPCM = mpcm;

                Paused = false;
                oneTimeReset = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool XgmPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                chip = new MDSound.MDSound.Chip
                {
                    ID = (byte)0,
                    Option = null
                };
                MDSound.ym2612 ym2612 = null;
                MDSound.ym3438 ym3438 = null;
                MDSound.ym2612mame ym2612mame = null;

                if (setting.YM2612Type[0].UseEmu[0])
                {
                    ym2612 ??= new ym2612();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                    chip.Instrument = ym2612;
                    chip.Update = ym2612.Update;
                    chip.Start = ym2612.Start;
                    chip.Stop = ym2612.Stop;
                    chip.Reset = ym2612.Reset;
                    chip.Option = new object[]
                    {
                        (int)(
                            (setting.nukedOPN2.GensDACHPF ? 0x01: 0x00)
                            |(setting.nukedOPN2.GensSSGEG ? 0x02: 0x00)
                        )
                    };
                }
                else if (setting.YM2612Type[0].UseEmu[1])
                {
                    ym3438 ??= new ym3438();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM3438;
                    chip.Instrument = ym3438;
                    chip.Update = ym3438.Update;
                    chip.Start = ym3438.Start;
                    chip.Stop = ym3438.Stop;
                    chip.Reset = ym3438.Reset;
                    switch (setting.nukedOPN2.EmuType)
                    {
                        case 0:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.discrete);
                            break;
                        case 1:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic);
                            break;
                        case 2:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612);
                            break;
                        case 3:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612_u);
                            break;
                        case 4:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic_lp);
                            break;
                    }
                }
                else if (setting.YM2612Type[0].UseEmu[2])
                {
                    ym2612mame ??= new ym2612mame();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2612mame;
                    chip.Instrument = ym2612mame;
                    chip.Update = ym2612mame.Update;
                    chip.Start = ym2612mame.Start;
                    chip.Stop = ym2612mame.Stop;
                    chip.Reset = ym2612mame.Reset;
                }

                chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                chip.Volume = setting.balance.YM2612Volume;
                chip.Clock = 7670454;
                clockYM2612 = 7670454;
                chipLED.PriOPN2 = 1;
                lstChips.Add(chip);
                useChip.Add(EnmChip.YM2612);

                sn76489 sn76489 = new();
                chip = new MDSound.MDSound.Chip
                {
                    type = MDSound.MDSound.enmInstrumentType.SN76489,
                    ID = (byte)0,
                    Instrument = sn76489,
                    Update = sn76489.Update,
                    Start = sn76489.Start,
                    Stop = sn76489.Stop,
                    Reset = sn76489.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Volume = setting.balance.SN76489Volume,
                    Clock = 3579545,
                    Option = null
                };
                chipLED.PriDCSG = 1;
                lstChips.Add(chip);
                useChip.Add(EnmChip.SN76489);

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                SetYM2612Volume(true, setting.balance.YM2612Volume);
                SetSN76489Volume(true, setting.balance.SN76489Volume);
                //chipRegister.setYM2203SSGVolume(0, setting.balance.GimicOPNVolume, enmModel.RealModel);
                //chipRegister.setYM2203SSGVolume(1, setting.balance.GimicOPNVolume, enmModel.RealModel);
                //chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, enmModel.RealModel);
                //chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, enmModel.RealModel);

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.YM2612, EnmChip.SN76489 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.YM2612, EnmChip.SN76489 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }
                //Play

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool ZgmPlay(Setting setting)
        {
            if (vgmBuf == null || setting == null) return false;

            try
            {
                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                //MIDIに対応するまで封印
                //startTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                if (!driverVirtual.init(vgmBuf
                    , chipRegister
                    , EnmModel.VirtualModel
                    , new EnmChip[] { EnmChip.YM2203 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)))
                    return false;

                //MIDIに対応するまで封印
                //if (driverReal != null && !driverReal.init(vgmBuf
                //    , chipRegister
                //    , EnmModel.RealModel
                //    , new EnmChip[] { EnmChip.YM2203 }
                //    , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                //    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)))
                //    return false;
                driverReal.Stopped = true;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                //
                //chips initialization
                //
                
                MDSound.ym2609 ym2609 = null;
                Func<string, Stream> fn = Common.GetOPNARyhthmStream;

                foreach (Driver.ZGM.ZgmChip.Chip ch in ((Driver.ZGM.zgm)driverVirtual).chips)
                {
                    if (ch.Device == Driver.ZGM.EnmZGMDevice.YM2609)
                    {
                        ym2609 ??= new ym2609();

                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.YM2609,
                            ID = (byte)0,
                            Instrument = ym2609,
                            Update = ym2609.Update,
                            Start = ym2609.Start,
                            Stop = ym2609.Stop,
                            Reset = ym2609.Reset,
                            SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                            Volume = 0,
                            Clock = (uint)ch.defineInfo.clock,
                            Option = new object[] { fn }
                        };

                        //hiyorimiDeviceFlag |= 0x2;

                        //if (i == 0) 
                        chipLED.PriOPNA2 = 1;
                        //else chipLED.SecOPNA2 = 1;

                        lstChips.Add(chip);
                        useChip.Add(EnmChip.YM2609);

                    }
                }

                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                //log.Write("参照用使用音源の登録(ZGM)");
                //((Driver.ZGM.zgm)driverVirtual).SetupDicChipCmdNo();

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }
        }

        public static bool S98Play(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.YM2203 }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.YM2203 }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                List<S98.S98DevInfo> s98DInfo = ((S98)driverVirtual).s98Info.DeviceInfos;

                ay8910 ym2149 = null;
                ym2203 ym2203 = null;
                ym2612 ym2612 = null;
                ym3438 ym3438 = null;
                ym2612mame ym2612mame = null;
                ym2608 ym2608 = null;
                ym2151 ym2151 = null;
                ym2151_mame ym2151mame = null;
                ym2151_x68sound ym2151_x68sound = null;
                MDSound.emu2413 ym2413 = null;
                ym3526 ym3526 = null;
                ym3812 ym3812 = null;
                ymf262 ymf262 = null;
                ay8910 ay8910 = null;

                int YM2151ClockValue = 4000000;
                int YM2203ClockValue = 4000000;
                int YM2608ClockValue = 8000000;
                int YMF262ClockValue = 14318180;
                useChip.Clear();

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
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.AY8910Volume;
                            chip.Clock = dInfo.Clock / 4;
                            clockAY8910 = (int)chip.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.AY8910 : EnmChip.S_AY8910);
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
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YM2203Volume;
                            chip.Clock = dInfo.Clock;
                            YM2203ClockValue = (int)chip.Clock;
                            chip.Option = null;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM2203 : EnmChip.S_YM2203);

                            break;
                        case 3:
                            chip = new MDSound.MDSound.Chip
                            {
                                Option = null
                            };
                            if (ym2612 == null)
                            {
                                ym2612 = new ym2612();
                                ym3438 = new ym3438();
                                ym2612mame = new ym2612mame();
                                chip.ID = 0;
                                chipLED.PriOPN2 = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPN2 = 1;
                            }

                            if ((chip.ID == 0 && setting.YM2612Type[0].UseEmu[0]) || (chip.ID == 1 && setting.YM2612Type[1].UseEmu[0]))
                            {
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                                chip.Instrument = ym2612;
                                chip.Update = ym2612.Update;
                                chip.Start = ym2612.Start;
                                chip.Stop = ym2612.Stop;
                                chip.Reset = ym2612.Reset;
                                chip.Option = new object[]
                                {
                                    (int)(
                                        (setting.nukedOPN2.GensDACHPF ? 0x01: 0x00)
                                        |(setting.nukedOPN2.GensSSGEG ? 0x02: 0x00)
                                    )
                                };
                            }
                            else if ((chip.ID == 0 && setting.YM2612Type[0].UseEmu[1]) || (chip.ID == 1 && setting.YM2612Type[1].UseEmu[1]))
                            {
                                chip.type = MDSound.MDSound.enmInstrumentType.YM3438;
                                chip.Instrument = ym3438;
                                chip.Update = ym3438.Update;
                                chip.Start = ym3438.Start;
                                chip.Stop = ym3438.Stop;
                                chip.Reset = ym3438.Reset;
                                switch (setting.nukedOPN2.EmuType)
                                {
                                    case 0:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.discrete);
                                        break;
                                    case 1:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic);
                                        break;
                                    case 2:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612);
                                        break;
                                    case 3:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612_u);
                                        break;
                                    case 4:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic_lp);
                                        break;
                                }
                            }
                            else if ((chip.ID == 0 && setting.YM2612Type[0].UseEmu[2]) || (chip.ID == 1 && setting.YM2612Type[1].UseEmu[2]))
                            {
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2612mame;
                                chip.Instrument = ym2612mame;
                                chip.Update = ym2612mame.Update;
                                chip.Start = ym2612mame.Start;
                                chip.Stop = ym2612mame.Stop;
                                chip.Reset = ym2612mame.Reset;
                            }

                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YM2612Volume;
                            chip.Clock = dInfo.Clock;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM2612 : EnmChip.S_YM2612);

                            break;
                        case 4:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2608 == null)
                            {
                                ym2608 = new ym2608();
                                chip.ID = 0;
                                chipLED.PriOPNA = 1;
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
                            chip.SamplingRate = 55467;// (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YM2608Volume;
                            chip.Clock = dInfo.Clock;
                            YM2608ClockValue = (int)chip.Clock;
                            Func<string, Stream> fn = Common.GetOPNARyhthmStream;
                            chip.Option = new object[] { fn };
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM2608 : EnmChip.S_YM2608);

                            break;
                        case 5:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2151 == null && ym2151mame == null)
                            {
                                chip.ID = 0;
                                chipLED.PriOPM = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPM = 1;
                            }

                            if ((chip.ID == 0 && setting.YM2151Type[0].UseEmu[0]) || (chip.ID == 1 && setting.YM2151Type[1].UseEmu[0]))
                            {
                                ym2151 ??= new MDSound.ym2151();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                                chip.Instrument = ym2151;
                                chip.Update = ym2151.Update;
                                chip.Start = ym2151.Start;
                                chip.Stop = ym2151.Stop;
                                chip.Reset = ym2151.Reset;
                            }
                            else if ((chip.ID == 0 && setting.YM2151Type[0].UseEmu[1]) || (chip.ID == 1 && setting.YM2151Type[1].UseEmu[1]))
                            {
                                ym2151mame ??= new MDSound.ym2151_mame();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2151mame;
                                chip.Instrument = ym2151mame;
                                chip.Update = ym2151mame.Update;
                                chip.Start = ym2151mame.Start;
                                chip.Stop = ym2151mame.Stop;
                                chip.Reset = ym2151mame.Reset;
                            }
                            else if ((chip.ID == 0 && setting.YM2151Type[0].UseEmu[2]) || (chip.ID == 1 && setting.YM2151Type[1].UseEmu[2]))
                            {
                                ym2151_x68sound ??= new MDSound.ym2151_x68sound();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2151x68sound;
                                chip.Instrument = ym2151_x68sound;
                                chip.Update = ym2151_x68sound.Update;
                                chip.Start = ym2151_x68sound.Start;
                                chip.Stop = ym2151_x68sound.Stop;
                                chip.Reset = ym2151_x68sound.Reset;
                            }

                            chip.Volume = setting.balance.YM2151Volume;
                            chip.Clock = dInfo.Clock;
                            chip.SamplingRate = (UInt32)chip.Clock / 64;
                            YM2151ClockValue = (int)chip.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            if (chip.Start != null)
                                lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM2151 : EnmChip.S_YM2151);

                            break;
                        case 6:
                            chip = new MDSound.MDSound.Chip();
                            if (ym2413 == null)
                            {
                                ym2413 = new MDSound.emu2413();
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
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YM2413Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM2413 : EnmChip.S_YM2413);

                            break;
                        case 7:
                            chip = new MDSound.MDSound.Chip();
                            if (ym3526 == null)
                            {
                                ym3526 = new ym3526();
                                chip.ID = 0;
                                chipLED.PriOPL = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPL = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3526;
                            chip.Instrument = ym3526;
                            chip.Update = ym3526.Update;
                            chip.Start = ym3526.Start;
                            chip.Stop = ym3526.Stop;
                            chip.Reset = ym3526.Reset;
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YM3526Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM3526 : EnmChip.S_YM3526);

                            break;
                        case 8:
                            chip = new MDSound.MDSound.Chip();
                            if (ym3812 == null)
                            {
                                ym3812 = new ym3812();
                                chip.ID = 0;
                                chipLED.PriOPL2 = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPL2 = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3812;
                            chip.Instrument = ym3812;
                            chip.Update = ym3812.Update;
                            chip.Start = ym3812.Start;
                            chip.Stop = ym3812.Stop;
                            chip.Reset = ym3812.Reset;
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YM3812Volume;
                            chip.Clock = dInfo.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YM3812 : EnmChip.S_YM3812);

                            break;
                        case 9:
                            chip = new MDSound.MDSound.Chip();
                            if (ymf262 == null)
                            {
                                ymf262 = new ymf262();
                                chip.ID = 0;
                                chipLED.PriOPL3 = 1;
                            }
                            else
                            {
                                chip.ID = 1;
                                chipLED.SecOPL3 = 1;
                            }
                            chip.type = MDSound.MDSound.enmInstrumentType.YMF262;
                            chip.Instrument = ymf262;
                            chip.Update = ymf262.Update;
                            chip.Start = ymf262.Start;
                            chip.Stop = ymf262.Stop;
                            chip.Reset = ymf262.Reset;
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.YMF262Volume;
                            chip.Clock = dInfo.Clock;
                            YMF262ClockValue = (int)chip.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.YMF262 : EnmChip.S_YMF262);

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
                            chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                            chip.Volume = setting.balance.AY8910Volume;
                            chip.Clock = dInfo.Clock;
                            clockAY8910 = (int)chip.Clock;
                            chip.Option = null;
                            //hiyorimiDeviceFlag |= 0x2;
                            lstChips.Add(chip);
                            useChip.Add(chip.ID == 0 ? EnmChip.AY8910 : EnmChip.S_AY8910);

                            break;
                    }
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                if (useChip.Contains(EnmChip.YM2203) || useChip.Contains(EnmChip.S_YM2203))
                {
                    SetYM2203Volume(true, setting.balance.YM2203Volume);
                    SetYM2203FMVolume(true, setting.balance.YM2203FMVolume);
                    SetYM2203PSGVolume(true, setting.balance.YM2203PSGVolume);
                }

                if (useChip.Contains(EnmChip.YM2612) || useChip.Contains( EnmChip.S_YM2612))
                    SetYM2612Volume(true, setting.balance.YM2612Volume);

                if (useChip.Contains(EnmChip.YM2608) || useChip.Contains(EnmChip.S_YM2608))
                {
                    SetYM2608Volume(true, setting.balance.YM2608Volume);
                    SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                    SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                    SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                    SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);
                }

                if (useChip.Contains(EnmChip.YM2608))
                {
                    chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.VirtualModel);
                    chipRegister.setYM2608Register(0, 0, 0x29, 0x82, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.S_YM2608))
                {
                    chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.VirtualModel);
                    chipRegister.setYM2608Register(1, 0, 0x29, 0x82, EnmModel.RealModel);
                }
                if (useChip.Contains( EnmChip.YM2151) || useChip.Contains( EnmChip.S_YM2151))
                    SetYM2151Volume(true, setting.balance.YM2151Volume);
                if (useChip.Contains(EnmChip.YM2413) || useChip.Contains(EnmChip.S_YM2413))
                    SetYM2413Volume(true, setting.balance.YM2413Volume);
                if (useChip.Contains(EnmChip.YM3526) || useChip.Contains(EnmChip.S_YM3526))
                    SetYM3526Volume(true, setting.balance.YM3526Volume);
                if (useChip.Contains(EnmChip.AY8910) || useChip.Contains(EnmChip.S_AY8910))
                    SetAY8910Volume(true, setting.balance.AY8910Volume);

                if (useChip.Contains(EnmChip.AY8910))
                    chipRegister.writeAY8910Clock(0, clockAY8910, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_AY8910))
                    chipRegister.writeAY8910Clock(1, clockAY8910, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.YM2151))
                    chipRegister.writeYM2151Clock(0, YM2151ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2151))
                    chipRegister.writeYM2151Clock(1, YM2151ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.YM2203))
                    chipRegister.writeYM2203Clock(0, YM2203ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2203))
                    chipRegister.writeYM2203Clock(1, YM2203ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.YM2608))
                    chipRegister.writeYM2608Clock(0, YM2608ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2608))
                    chipRegister.writeYM2608Clock(1, YM2608ClockValue, EnmModel.RealModel);

                if (useChip.Contains(EnmChip.YMF262))
                {
                    chipRegister.setYMF262Register(0, 1, 5, 1, EnmModel.RealModel);//opl3mode
                    chipRegister.writeYMF262Clock(0, YMF262ClockValue, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.S_YMF262))
                {
                    chipRegister.setYMF262Register(1, 1, 5, 1, EnmModel.RealModel);//opl3mode
                    chipRegister.writeYMF262Clock(1, YMF262ClockValue, EnmModel.RealModel);
                }

                driverVirtual.SetYM2151Hosei(YM2151ClockValue);
                driverReal?.SetYM2151Hosei(YM2151ClockValue);

                if (driverReal == null || ((S98)driverReal).SSGVolumeFromTAG == -1)
                {
                    if (useChip.Contains(EnmChip.YM2203))
                        chipRegister.setYM2203SSGVolume(0, setting.balance.GimicOPNVolume, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2203))
                        chipRegister.setYM2203SSGVolume(1, setting.balance.GimicOPNVolume, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.YM2608))
                        chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2608))
                        chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                }
                else
                {
                    if (useChip.Contains(EnmChip.YM2203))
                        chipRegister.setYM2203SSGVolume(0, ((S98)driverReal).SSGVolumeFromTAG, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2203))
                        chipRegister.setYM2203SSGVolume(1, ((S98)driverReal).SSGVolumeFromTAG, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.YM2608))
                        chipRegister.setYM2608SSGVolume(0, ((S98)driverReal).SSGVolumeFromTAG, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2608))
                        chipRegister.setYM2608SSGVolume(1, ((S98)driverReal).SSGVolumeFromTAG, EnmModel.RealModel);
                }
                //Play

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool MidPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs
                {
                    PriMID = 1,
                    SecMID = 1
                };

                MasterVolume = setting.balance.MasterVolume;

                chipRegister.initChipRegister(null);
                ReleaseAllMIDIout();
                MakeMIDIout(setting, MidiMode);
                chipRegister.setMIDIout(setting.midiOut.lstMidiOutInfo[MidiMode], midiOuts, midiOutsType);

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;


                //Play

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                Stopped = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool RcpPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs
                {
                    PriMID = 1,
                    SecMID = 1
                };

                MasterVolume = setting.balance.MasterVolume;

                chipRegister.initChipRegister(null);
                ReleaseAllMIDIout();
                MakeMIDIout(setting, MidiMode);
                chipRegister.setMIDIout(setting.midiOut.lstMidiOutInfo[MidiMode], midiOuts, midiOutsType);

                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                //Play

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool NsfPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();


                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs
                {
                    PriNES = 1,
                    PriDMC = 1
                };

                MasterVolume = setting.balance.MasterVolume;

                ((nsf)driverVirtual).song = SongNo;
                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    ((nsf)driverReal).song = SongNo;
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                if (((nsf)driverVirtual).use_fds) chipLED.PriFDS = 1;
                if (((nsf)driverVirtual).use_fme7) chipLED.PriFME7 = 1;
                if (((nsf)driverVirtual).use_mmc5) chipLED.PriMMC5 = 1;
                if (((nsf)driverVirtual).use_n106) chipLED.PriN106 = 1;
                if (((nsf)driverVirtual).use_vrc6) chipLED.PriVRC6 = 1;
                if (((nsf)driverVirtual).use_vrc7) chipLED.PriVRC7 = 1;

                //nes_intf nes = new nes_intf();
                MDSound.MDSound.Chip chip;
                nes_intf nes = new();

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.Nes,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Volume = setting.balance.APUVolume,
                    Clock = 0,
                    Option = null
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cAPU = chip;
                useChip.Add(EnmChip.NES);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.DMC,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.DMCVolume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cDMC = chip;
                useChip.Add(EnmChip.DMC);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.FDS,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.FDSVolume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cFDS = chip;
                useChip.Add(EnmChip.FDS);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.MMC5,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.MMC5Volume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cMMC5 = chip;
                useChip.Add(EnmChip.MMC5);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.N160,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.N160Volume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cN160 = chip;
                useChip.Add(EnmChip.N163);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.VRC6,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.VRC6Volume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cVRC6 = chip;
                useChip.Add(EnmChip.VRC6);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.VRC7,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.VRC7Volume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cVRC7 = chip;
                useChip.Add(EnmChip.VRC7);

                chip = new MDSound.MDSound.Chip
                {
                    ID = 0,
                    type = MDSound.MDSound.enmInstrumentType.FME7,
                    Instrument = nes,
                    Update = nes.Update,
                    Start = nes.Start,
                    Stop = nes.Stop,
                    Reset = nes.Reset,
                    SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                    Clock = 0,
                    Option = null,
                    Volume = setting.balance.FME7Volume
                };
                lstChips.Add(chip);
                ((nsf)driverVirtual).cFME7 = chip;
                useChip.Add(EnmChip.FME7);

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegisterNSF(lstChips.ToArray());

                //Play

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool HesPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriHuC = 1;

                MasterVolume = setting.balance.MasterVolume;

                //((hes)driverVirtual).song = (byte)SongNo;
                //((hes)driverReal).song = (byte)SongNo;
                //if (!driverVirtual.init(vgmBuf, chipRegister, enmModel.VirtualModel, new enmUseChip[] { enmUseChip.Unuse }, 0)) return false;
                //if (!driverReal.init(vgmBuf, chipRegister, enmModel.RealModel, new enmUseChip[] { enmUseChip.Unuse }, 0)) return false;

                MDSound.MDSound.Chip chip;
                MDSound.Ootake_PSG huc = new();

                chip = new MDSound.MDSound.Chip();
                chip.ID = 0;
                chip.type = MDSound.MDSound.enmInstrumentType.HuC6280;
                chip.Instrument = huc;
                chip.Update = huc.Update;
                chip.Start = huc.Start;
                chip.Stop = huc.Stop;
                chip.Reset = huc.Reset;
                chip.AdditionalUpdate = ((hes)driverVirtual).AdditionalUpdate;
                chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                chip.Volume = setting.balance.HuC6280Volume;
                chip.Clock = 3579545;
                chip.Option = null;
                lstChips.Add(chip);
                ((hes)driverVirtual).c6280 = chip;
                useChip.Add(EnmChip.HuC6280);

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());

                ((hes)driverVirtual).song = (byte)SongNo;
                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    ((hes)driverReal).song = (byte)SongNo;
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }
                //Play

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool SidPlay(Setting setting)
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

                ClearFadeoutVolume();

                chipRegister.resetChips();

                chipRegister.initChipRegister(null);

                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();
                chipLED.PriSID = 1;

                MasterVolume = setting.balance.MasterVolume;

                ((Driver.SID.sid)driverVirtual).song = (byte)SongNo + 1;
                if (!driverVirtual.init(vgmBuf, chipRegister, EnmModel.VirtualModel, new EnmChip[] { EnmChip.Unuse }
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                if (driverReal != null)
                {
                    ((Driver.SID.sid)driverReal).song = (byte)SongNo + 1;
                    if (!driverReal.init(vgmBuf, chipRegister, EnmModel.RealModel, new EnmChip[] { EnmChip.Unuse }
                        , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                        , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000))) return false;
                }

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool VgmPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                //Stop();

                chipRegister.resetChips();

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeed = 1;
                vgmRealFadeoutVol = 0;
                vgmRealFadeoutVolWait = 4;

                ClearFadeoutVolume();

                chipRegister.resetChips();

                useChip.Clear();

                StartTrdVgmReal();

                List<MDSound.MDSound.Chip> lstChips = new();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                if (!driverVirtual.init(vgmBuf
                    , chipRegister
                    , EnmModel.VirtualModel
                    , new EnmChip[] { EnmChip.YM2203 }// usechip.ToArray()
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)))
                    return false;

                if (driverReal != null && !driverReal.init(vgmBuf
                    , chipRegister
                    , EnmModel.RealModel
                    , new EnmChip[]{ EnmChip.YM2203 }// usechip.ToArray()
                    , (uint)(setting.outputDevice.SampleRate * setting.LatencySCCI / 1000)
                    , (uint)(setting.outputDevice.SampleRate * setting.outputDevice.WaitTime / 1000)))
                    return false;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                if (((Vgm)driverVirtual).YM2413ClockValue != 0)
                {
                    Instrument opll = null;
                    if (!((Vgm)driverVirtual).YM2413VRC7Flag)
                    {
                        opll = new MDSound.emu2413();// MDSound.ym2413();
                    }
                    else
                    {
                        opll = new VRC7();
                    }

                    for (int i = 0; i < (((Vgm)driverVirtual).YM2413DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.YM2413emu,
                            ID = (byte)i,
                            Instrument = opll,
                            Update = opll.Update,
                            Start = opll.Start,
                            Stop = opll.Stop,
                            Reset = opll.Reset,
                            SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                            Volume = setting.balance.YM2413Volume,
                            Clock = (((Vgm)driverVirtual).YM2413ClockValue & 0x7fffffff),
                            Option = null
                        };

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPLL = 1;
                        else chipLED.SecOPLL = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM2413 : EnmChip.S_YM2413);
                    }
                }

                if (((Vgm)driverVirtual).SN76489ClockValue != 0)
                {
                    MDSound.sn76489 sn76489 = null;
                    MDSound.SN76496 sn76496 = null;

                    for (int i = 0; i < (((Vgm)driverVirtual).SN76489DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            ID = (byte)i,
                            Option = null
                        };

                        if ((i == 0 && (setting.SN76489Type[0].UseEmu[0] || setting.SN76489Type[0].UseReal[0]))
                            || (i == 1 && (setting.SN76489Type[1].UseEmu[0] || setting.SN76489Type[1].UseReal[0])))
                        {
                            sn76489 ??= new sn76489();
                            chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                            chip.Instrument = sn76489;
                            chip.Update = sn76489.Update;
                            chip.Start = sn76489.Start;
                            chip.Stop = sn76489.Stop;
                            chip.Reset = sn76489.Reset;
                        }
                        else if ((i == 0 && setting.SN76489Type[0].UseEmu[1])
                            || (i == 1 && setting.SN76489Type[1].UseEmu[1]))
                        {
                            sn76496 ??= new SN76496();
                            chip.type = MDSound.MDSound.enmInstrumentType.SN76496;
                            chip.Instrument = sn76496;
                            chip.Update = sn76496.Update;
                            chip.Start = sn76496.Start;
                            chip.Stop = sn76496.Stop;
                            chip.Reset = sn76496.Reset;
                            chip.Option = ((Vgm)driverVirtual).SN76489Option;
                        }

                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.SN76489Volume;
                        chip.Clock = ((Vgm)driverVirtual).SN76489ClockValue
                            | (((Vgm)driverVirtual).SN76489NGPFlag ? 0x80000000 : 0);
                        clockSN76489 = (int)(chip.Clock & 0x7fff_ffff);
                        if (i == 0) chipLED.PriDCSG = 1;
                        else chipLED.SecDCSG = 1;

                        hiyorimiDeviceFlag |= (setting.SN76489Type[0].UseReal[0]) ? 0x1 : 0x2;
                        SN76489NGPFlag = ((Vgm)driverVirtual).SN76489NGPFlag;

                        if (chip.Start != null) lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.SN76489 : EnmChip.S_SN76489);
                    }
                }

                if (((Vgm)driverVirtual).YM2612ClockValue != 0)
                {
                    MDSound.ym2612 ym2612 = null;
                    MDSound.ym3438 ym3438 = null;
                    MDSound.ym2612mame ym2612mame = null;

                    for (int i = 0; i < (((Vgm)driverVirtual).YM2612DualChipFlag ? 2 : 1); i++)
                    {
                        //MDSound.ym2612 ym2612 = new MDSound.ym2612();
                        chip = new MDSound.MDSound.Chip
                        {
                            ID = (byte)i,
                            Option = null
                        };

                        if (
                            (i == 0 && (
                                (setting.YM2612Type[0].UseEmu[0] || setting.YM2612Type[0].realChipInfo[0].OnlyPCMEmulation )
                                || setting.YM2612Type[0].UseReal[0])
                            )
                            || (i == 1 && 
                                (setting.YM2612Type[1].UseEmu[0] || setting.YM2612Type[1].realChipInfo[0].OnlyPCMEmulation)
                                || setting.YM2612Type[1].UseReal[0]
                            )
                        )
                        {
                            ym2612 ??= new ym2612();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                            chip.Instrument = ym2612;
                            chip.Update = ym2612.Update;
                            chip.Start = ym2612.Start;
                            chip.Stop = ym2612.Stop;
                            chip.Reset = ym2612.Reset;
                            chip.Option = new object[]
                            {
                                (int)(
                                    (setting.nukedOPN2.GensDACHPF ? 0x01: 0x00)
                                    |(setting.nukedOPN2.GensSSGEG ? 0x02: 0x00)
                                )
                            };
                        }
                        else if ((i == 0 && setting.YM2612Type[0].UseEmu[1]) || (i == 1 && setting.YM2612Type[1].UseEmu[1]))
                        {
                            ym3438 ??= new ym3438();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3438;
                            chip.Instrument = ym3438;
                            chip.Update = ym3438.Update;
                            chip.Start = ym3438.Start;
                            chip.Stop = ym3438.Stop;
                            chip.Reset = ym3438.Reset;
                            switch (setting.nukedOPN2.EmuType)
                            {
                                case 0:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.discrete);
                                    break;
                                case 1:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic);
                                    break;
                                case 2:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612);
                                    break;
                                case 3:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612_u);
                                    break;
                                case 4:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic_lp);
                                    break;
                            }
                        }
                        else if ((i == 0 && setting.YM2612Type[0].UseEmu[2]) || (i == 1 && setting.YM2612Type[0].UseEmu[2]))
                        {
                            ym2612mame ??= new ym2612mame();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2612mame;
                            chip.Instrument = ym2612mame;
                            chip.Update = ym2612mame.Update;
                            chip.Start = ym2612mame.Start;
                            chip.Stop = ym2612mame.Stop;
                            chip.Reset = ym2612mame.Reset;
                        }

                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.YM2612Volume;
                        chip.Clock = ((Vgm)driverVirtual).YM2612ClockValue;
                        clockYM2612 = (int)((Vgm)driverVirtual).YM2612ClockValue;

                        hiyorimiDeviceFlag |= (setting.YM2612Type[0].UseReal[0]) ? 0x1 : 0x2;
                        hiyorimiDeviceFlag |= (setting.YM2612Type[0].UseReal[0] 
                            && setting.YM2612Type[0].realChipInfo[0].OnlyPCMEmulation) ? 0x2 : 0x0;

                        if (i == 0) chipLED.PriOPN2 = 1;
                        else chipLED.SecOPN2 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM2612 : EnmChip.S_YM2612);
                    }
                }

                if (((Vgm)driverVirtual).YM2151ClockValue != 0)
                {
                    MDSound.ym2151 ym2151 = null;
                    MDSound.ym2151_mame ym2151_mame = null;
                    MDSound.ym2151_x68sound ym2151_x68sound = null;
                    for (int i = 0; i < (((Vgm)driverVirtual).YM2151DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)i;

                        if ((i == 0 && (setting.YM2151Type[0].UseEmu[0] || setting.YM2151Type[0].UseReal[0]))
                         || (i == 1 && (setting.YM2151Type[1].UseEmu[0] || setting.YM2151Type[1].UseReal[0])))
                        {
                            ym2151 ??= new MDSound.ym2151();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                            chip.Instrument = ym2151;
                            chip.Update = ym2151.Update;
                            chip.Start = ym2151.Start;
                            chip.Stop = ym2151.Stop;
                            chip.Reset = ym2151.Reset;
                        }
                        else if ((i == 0 && setting.YM2151Type[0].UseEmu[1]) || (i == 1 && setting.YM2151Type[1].UseEmu[1]))
                        {
                            ym2151_mame ??= new MDSound.ym2151_mame();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151mame;
                            chip.Instrument = ym2151_mame;
                            chip.Update = ym2151_mame.Update;
                            chip.Start = ym2151_mame.Start;
                            chip.Stop = ym2151_mame.Stop;
                            chip.Reset = ym2151_mame.Reset;
                        }
                        else if ((i == 0 && setting.YM2151Type[0].UseEmu[2]) || (i == 1 && setting.YM2151Type[1].UseEmu[2]))
                        {
                            ym2151_x68sound ??= new MDSound.ym2151_x68sound();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2151x68sound;
                            chip.Instrument = ym2151_x68sound;
                            chip.Update = ym2151_x68sound.Update;
                            chip.Start = ym2151_x68sound.Start;
                            chip.Stop = ym2151_x68sound.Stop;
                            chip.Reset = ym2151_x68sound.Reset;
                        }

                        chip.Volume = setting.balance.YM2151Volume;
                        chip.Clock = ((Vgm)driverVirtual).YM2151ClockValue;
                        chip.SamplingRate = (UInt32)chip.Clock / 64;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPM = 1;
                        else chipLED.SecOPM = 1;

                        if (chip.Start != null)
                            lstChips.Add(chip);

                        useChip.Add(i == 0 ? EnmChip.YM2151 : EnmChip.S_YM2151);
                    }
                }

                if (((Vgm)driverVirtual).SEGAPCMClockValue != 0)
                {
                    MDSound.segapcm segapcm = new();
                    chip = new MDSound.MDSound.Chip
                    {
                        type = MDSound.MDSound.enmInstrumentType.SEGAPCM,
                        ID = 0,
                        Instrument = segapcm,
                        Update = segapcm.Update,
                        Start = segapcm.Start,
                        Stop = segapcm.Stop,
                        Reset = segapcm.Reset,
                        SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                        Volume = setting.balance.SEGAPCMVolume,
                        Clock = ((Vgm)driverVirtual).SEGAPCMClockValue,
                        Option = new object[1] { ((Vgm)driverVirtual).SEGAPCMInterface }
                    };

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriSPCM = 1;

                    lstChips.Add(chip);
                    useChip.Add( EnmChip.SEGAPCM);
                }

                if (((Vgm)driverVirtual).RF5C68ClockValue != 0)
                {
                    MDSound.rf5c68 rf5c68 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).RF5C68DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.RF5C68,
                            ID = (byte)i,
                            Instrument = rf5c68,
                            Update = rf5c68.Update,
                            Start = rf5c68.Start,
                            Stop = rf5c68.Stop,
                            Reset = rf5c68.Reset,
                            SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                            Volume = setting.balance.RF5C68Volume,
                            Clock = ((Vgm)driverVirtual).RF5C68ClockValue,
                            Option = null
                        };

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriRF5C68 = 1;
                        else chipLED.SecRF5C68 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.RF5C68 : EnmChip.S_RF5C68);
                    }
                }

                if (((Vgm)driverVirtual).YM2203ClockValue != 0)
                {
                    MDSound.ym2203 ym2203 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YM2203DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.YM2203,
                            ID = (byte)i,
                            Instrument = ym2203,
                            Update = ym2203.Update,
                            Start = ym2203.Start,
                            Stop = ym2203.Stop,
                            Reset = ym2203.Reset,
                            SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                            Volume = setting.balance.YM2203Volume,
                            Clock = ((Vgm)driverVirtual).YM2203ClockValue,
                            Option = null
                        };

                        clockYM2203 = (int)((Vgm)driverVirtual).YM2203ClockValue;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPN = 1;
                        else chipLED.SecOPN = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM2203 : EnmChip.S_YM2203);
                    }
                }

                if (((Vgm)driverVirtual).YM2608ClockValue != 0)
                {
                    MDSound.ym2608 ym2608 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YM2608DualChipFlag ? 2 : 1); i++)
                    {
                        Func<string, Stream> fn = Common.GetOPNARyhthmStream;
                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.YM2608,
                            ID = (byte)i,
                            Instrument = ym2608,
                            Update = ym2608.Update,
                            Start = ym2608.Start,
                            Stop = ym2608.Stop,
                            Reset = ym2608.Reset,
                            SamplingRate = 55467,// (UInt32)setting.outputDevice.SampleRate;
                            Volume = setting.balance.YM2608Volume,
                            Clock = ((Vgm)driverVirtual).YM2608ClockValue,
                            Option = new object[] { fn }
                        };
                        hiyorimiDeviceFlag |= 0x2;
                        clockYM2608 = (int)((Vgm)driverVirtual).YM2608ClockValue;

                        if (i == 0) chipLED.PriOPNA = 1;
                        else chipLED.SecOPNA = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM2608 : EnmChip.S_YM2608);
                    }
                }

                if (((Vgm)driverVirtual).YM2610ClockValue != 0)
                {
                    MDSound.ym2610 ym2610 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YM2610DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.YM2610,
                            ID = (byte)i,
                            Instrument = ym2610,
                            Update = ym2610.Update,
                            Start = ym2610.Start,
                            Stop = ym2610.Stop,
                            Reset = ym2610.Reset,
                            SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                            Volume = setting.balance.YM2610Volume,
                            Clock = ((Vgm)driverVirtual).YM2610ClockValue & 0x7fffffff,
                            Option = null
                        };

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPNB = 1;
                        else chipLED.SecOPNB = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM2610 : EnmChip.S_YM2610);
                    }
                }

                if (((Vgm)driverVirtual).YM3812ClockValue != 0)
                {
                    MDSound.ym3812 ym3812 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YM3812DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip
                        {
                            type = MDSound.MDSound.enmInstrumentType.YM3812,
                            ID = (byte)i,
                            Instrument = ym3812,
                            Update = ym3812.Update,
                            Start = ym3812.Start,
                            Stop = ym3812.Stop,
                            Reset = ym3812.Reset,
                            SamplingRate = (UInt32)setting.outputDevice.SampleRate,
                            Volume = setting.balance.YM3812Volume,
                            Clock = ((Vgm)driverVirtual).YM3812ClockValue & 0x7fffffff,
                            Option = null
                        };

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPL2 = 1;
                        else chipLED.SecOPL2 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM3812 : EnmChip.S_YM3812);
                    }
                }

                if (((Vgm)driverVirtual).YM3526ClockValue != 0)
                {
                    MDSound.ym3526 ym3526 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).YM3526DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM3526;
                        chip.ID = (byte)i;
                        chip.Instrument = ym3526;
                        chip.Update = ym3526.Update;
                        chip.Start = ym3526.Start;
                        chip.Stop = ym3526.Stop;
                        chip.Reset = ym3526.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.YM3526Volume;
                        chip.Clock = ((Vgm)driverVirtual).YM3526ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriOPL = 1;
                        else chipLED.SecOPL = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YM3526 : EnmChip.S_YM3526);
                    }
                }

                if (((Vgm)driverVirtual).Y8950ClockValue != 0)
                {
                    MDSound.y8950 y8950 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).Y8950DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.Y8950;
                        chip.ID = (byte)i;
                        chip.Instrument = y8950;
                        chip.Update = y8950.Update;
                        chip.Start = y8950.Start;
                        chip.Stop = y8950.Stop;
                        chip.Reset = y8950.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.Y8950Volume;
                        chip.Clock = ((Vgm)driverVirtual).Y8950ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriY8950 = 1;
                        else chipLED.SecY8950 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.Y8950 : EnmChip.S_Y8950);
                    }
                }

                if (((Vgm)driverVirtual).YMF262ClockValue != 0)
                {
                    MDSound.ymf262 ymf262 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YMF262DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF262;
                        chip.ID = (byte)i;
                        chip.Instrument = ymf262;
                        chip.Update = ymf262.Update;
                        chip.Start = ymf262.Start;
                        chip.Stop = ymf262.Stop;
                        chip.Reset = ymf262.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.YMF262Volume;
                        chip.Clock = ((Vgm)driverVirtual).YMF262ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPL3 = 1;
                        else chipLED.SecOPL3 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YMF262 : EnmChip.S_YMF262);
                    }
                }

                if (((Vgm)driverVirtual).YMF278BClockValue != 0)
                {
                    MDSound.ymf278b ymf278b = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YMF278BDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
                        chip.ID = (byte)i;
                        chip.Instrument = ymf278b;
                        chip.Update = ymf278b.Update;
                        chip.Start = ymf278b.Start;
                        chip.Stop = ymf278b.Stop;
                        chip.Reset = ymf278b.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.YMF278BVolume;
                        chip.Clock = ((Vgm)driverVirtual).YMF278BClockValue & 0x7fffffff;
                        chip.Option = new object[] { Common.GetApplicationFolder() };

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPL4 = 1;
                        else chipLED.SecOPL4 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YMF278B : EnmChip.S_YMF278B);
                    }
                }

                if (((Vgm)driverVirtual).YMF271ClockValue != 0)
                {
                    MDSound.ymf271 ymf271 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YMF271DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF271;
                        chip.ID = (byte)i;
                        chip.Instrument = ymf271;
                        chip.Update = ymf271.Update;
                        chip.Start = ymf271.Start;
                        chip.Stop = ymf271.Stop;
                        chip.Reset = ymf271.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.YMF271Volume;
                        chip.Clock = ((Vgm)driverVirtual).YMF271ClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOPX = 1;
                        else chipLED.SecOPX = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YMF271 : EnmChip.S_YMF271);
                    }
                }

                if (((Vgm)driverVirtual).YMZ280BClockValue != 0)
                {
                    MDSound.ymz280b ymz280b = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).YMZ280BDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMZ280B;
                        chip.ID = (byte)i;
                        chip.Instrument = ymz280b;
                        chip.Update = ymz280b.Update;
                        chip.Start = ymz280b.Start;
                        chip.Stop = ymz280b.Stop;
                        chip.Reset = ymz280b.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.YMZ280BVolume;
                        chip.Clock = ((Vgm)driverVirtual).YMZ280BClockValue & 0x7fffffff;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriYMZ = 1;
                        else chipLED.SecYMZ = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.YMZ280B : EnmChip.S_YMZ280B);
                    }
                }

                if (((Vgm)driverVirtual).RF5C164ClockValue != 0)
                {
                    MDSound.scd_pcm rf5c164 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).RF5C164DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.RF5C164;
                        chip.ID = (byte)i;
                        chip.Instrument = rf5c164;
                        chip.Update = rf5c164.Update;
                        chip.Start = rf5c164.Start;
                        chip.Stop = rf5c164.Stop;
                        chip.Reset = rf5c164.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.RF5C164Volume;
                        chip.Clock = ((Vgm)driverVirtual).RF5C164ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriRF5C = 1;
                        else chipLED.SecRF5C = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.RF5C164 : EnmChip.S_RF5C164);
                    }
                }

                if (((Vgm)driverVirtual).PWMClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.PWM;
                    chip.ID = 0;
                    MDSound.pwm pwm = new();
                    chip.Instrument = pwm;
                    chip.Update = pwm.Update;
                    chip.Start = pwm.Start;
                    chip.Stop = pwm.Stop;
                    chip.Reset = pwm.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.PWMVolume;
                    chip.Clock = ((Vgm)driverVirtual).PWMClockValue;
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriPWM = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.PWM);
                }

                if (((Vgm)driverVirtual).AY8910ClockValue != 0)
                {
                    MDSound.ay8910 ay8910 = null;
                    MDSound.ay8910_mame ay8910mame = null;

                    for (int i = 0; i < (((Vgm)driverVirtual).AY8910DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                        chip.ID = (byte)i;

                        if ((i == 0 && (setting.AY8910Type[0].UseEmu[0] || setting.AY8910Type[0].UseReal[0]))
                            || (i == 1 && (setting.AY8910Type[1].UseEmu[0] || setting.AY8910Type[1].UseReal[0])))
                        {
                            ay8910 ??= new ay8910();
                            chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                            chip.Instrument = ay8910;
                            chip.Update = ay8910.Update;
                            chip.Start = ay8910.Start;
                            chip.Stop = ay8910.Stop;
                            chip.Reset = ay8910.Reset;
                        }
                        else if ((i == 0 && setting.AY8910Type[0].UseEmu[1])
                            || (i == 1 && setting.AY8910Type[1].UseEmu[1]))
                        {
                            ay8910mame ??= new ay8910_mame();
                            chip.type = MDSound.MDSound.enmInstrumentType.AY8910mame;
                            chip.Instrument = ay8910mame;
                            chip.Update = ay8910mame.Update;
                            chip.Start = ay8910mame.Start;
                            chip.Stop = ay8910mame.Stop;
                            chip.Reset = ay8910mame.Reset;
                        }

                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.AY8910Volume;
                        chip.Clock = (((Vgm)driverVirtual).AY8910ClockValue & 0x7fffffff) / 2;
                        clockAY8910 = (int)chip.Clock;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriAY10 = 1;
                        else chipLED.SecAY10 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.AY8910 : EnmChip.S_AY8910);
                    }
                }

                if (((Vgm)driverVirtual).DMGClockValue != 0)
                {
                    MDSound.gb dmg = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).DMGDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.DMG;
                        chip.ID = (byte)i;
                        chip.Instrument = dmg;
                        chip.Update = dmg.Update;
                        chip.Start = dmg.Start;
                        chip.Stop = dmg.Stop;
                        chip.Reset = dmg.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.DMGVolume;
                        chip.Clock = ((Vgm)driverVirtual).DMGClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriDMG = 1;
                        else chipLED.SecDMG = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.DMG : EnmChip.S_DMG);
                    }
                }

                if (((Vgm)driverVirtual).NESClockValue != 0)
                {

                    for (int i = 0; i < (((Vgm)driverVirtual).NESDualChipFlag ? 2 : 1); i++)
                    {
                        MDSound.nes_intf nes = new();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.Nes;
                        chip.ID = (byte)i;
                        chip.Instrument = nes;
                        chip.Update = nes.Update;
                        chip.Start = nes.Start;
                        chip.Stop = nes.Stop;
                        chip.Reset = nes.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.APUVolume;
                        chip.Clock = ((Vgm)driverVirtual).NESClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriNES = 1;
                        else chipLED.SecNES = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.NES : EnmChip.S_NES);

                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.DMC;
                        chip.ID = (byte)i;
                        chip.Instrument = nes;
                        //chip.Update = nes.Update;
                        chip.Start = nes.Start;
                        chip.Stop = nes.Stop;
                        chip.Reset = nes.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.DMCVolume;
                        chip.Clock = ((Vgm)driverVirtual).NESClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriDMC = 1;
                        else chipLED.SecDMC = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.DMC : EnmChip.S_DMC);


                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.FDS;
                        chip.ID = (byte)i;
                        chip.Instrument = nes;
                        //chip.Update = nes.Update;
                        chip.Start = nes.Start;
                        chip.Stop = nes.Stop;
                        chip.Reset = nes.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.FDSVolume;
                        chip.Clock = ((Vgm)driverVirtual).NESClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriFDS = 1;
                        else chipLED.SecFDS = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.FDS : EnmChip.S_FDS);


                        hiyorimiDeviceFlag |= 0x2;

                    }
                }

                if (((Vgm)driverVirtual).MultiPCMClockValue != 0)
                {
                    MDSound.multipcm multipcm = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).MultiPCMDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.MultiPCM;
                        chip.ID = (byte)i;
                        chip.Instrument = multipcm;
                        chip.Update = multipcm.Update;
                        chip.Start = multipcm.Start;
                        chip.Stop = multipcm.Stop;
                        chip.Reset = multipcm.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.MultiPCMVolume;
                        chip.Clock = ((Vgm)driverVirtual).MultiPCMClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriMPCM = 1;
                        else chipLED.SecMPCM = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.MultiPCM : EnmChip.S_MultiPCM);
                    }
                }

                if (((Vgm)driverVirtual).uPD7759ClockValue != 0)
                {
                    MDSound.upd7759 upd7759 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).uPD7759DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.uPD7759;
                        chip.ID = (byte)i;
                        chip.Instrument = upd7759;
                        chip.Update = upd7759.Update;
                        chip.Start = upd7759.Start;
                        chip.Stop = upd7759.Stop;
                        chip.Reset = upd7759.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.uPD7759Volume;
                        chip.Clock = ((Vgm)driverVirtual).uPD7759ClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriuPD7759 = 1;
                        else chipLED.SecuPD7759 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.uPD7759 : EnmChip.S_uPD7759);
                    }
                }

                if (((Vgm)driverVirtual).OKIM6258ClockValue != 0)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.OKIM6258;
                    chip.ID = 0;
                    MDSound.okim6258 okim6258 = new();
                    chip.Instrument = okim6258;
                    chip.Update = okim6258.Update;
                    chip.Start = okim6258.Start;
                    chip.Stop = okim6258.Stop;
                    chip.Reset = okim6258.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.OKIM6258Volume;
                    chip.Clock = ((Vgm)driverVirtual).OKIM6258ClockValue;
                    chip.Option = new object[1] { (int)((Vgm)driverVirtual).OKIM6258Type };
                    //chip.Option = new object[1] { 6 };
                    okim6258.okim6258_set_srchg_cb(0, ChangeChipSampleRate, chip);

                    hiyorimiDeviceFlag |= 0x2;

                    chipLED.PriOKI5 = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.OKIM6258);
                }

                if (((Vgm)driverVirtual).OKIM6295ClockValue != 0)
                {
                    MDSound.okim6295 okim6295 = new();
                    for (byte i = 0; i < (((Vgm)driverVirtual).OKIM6295DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.OKIM6295;
                        chip.ID = (byte)i;
                        chip.Instrument = okim6295;
                        chip.Update = okim6295.Update;
                        chip.Start = okim6295.Start;
                        chip.Stop = okim6295.Stop;
                        chip.Reset = okim6295.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.OKIM6295Volume;
                        chip.Clock = ((Vgm)driverVirtual).OKIM6295ClockValue;
                        chip.Option = null;
                        okim6295.okim6295_set_srchg_cb(i, ChangeChipSampleRate, chip);

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriOKI9 = 1;
                        else chipLED.SecOKI9 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.OKIM6295 : EnmChip.S_OKIM6295);
                    }
                }

                if (((Vgm)driverVirtual).K051649ClockValue != 0)
                {
                    MDSound.K051649 k051649 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).K051649DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K051649;
                        chip.ID = (byte)i;
                        chip.Instrument = k051649;
                        chip.Update = k051649.Update;
                        chip.Start = k051649.Start;
                        chip.Stop = k051649.Stop;
                        chip.Reset = k051649.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.K051649Volume;
                        chip.Clock = ((Vgm)driverVirtual).K051649ClockValue;
                        clockK051649 = (int)chip.Clock;
                        chip.Option = null;
                        if (i == 0) chipLED.PriK051649 = 1;
                        else chipLED.SecK051649 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.K051649 : EnmChip.S_K051649);
                    }
                }

                if (((Vgm)driverVirtual).K054539ClockValue != 0)
                {
                    MDSound.K054539 k054539 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).K054539DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K054539;
                        chip.ID = (byte)i;
                        chip.Instrument = k054539;
                        chip.Update = k054539.Update;
                        chip.Start = k054539.Start;
                        chip.Stop = k054539.Stop;
                        chip.Reset = k054539.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.K054539Volume;
                        chip.Clock = ((Vgm)driverVirtual).K054539ClockValue;
                        chip.Option = null;
                        if (i == 0) chipLED.PriK054539 = 1;
                        else chipLED.SecK054539 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.K054539 : EnmChip.S_K054539);
                    }
                }

                if (((Vgm)driverVirtual).HuC6280ClockValue != 0)
                {
                    MDSound.Ootake_PSG huc6280 = new ();
                    for (int i = 0; i < (((Vgm)driverVirtual).HuC6280DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.HuC6280;
                        chip.ID = (byte)i;
                        chip.Instrument = huc6280;
                        chip.Update = huc6280.Update;
                        chip.Start = huc6280.Start;
                        chip.Stop = huc6280.Stop;
                        chip.Reset = huc6280.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.HuC6280Volume;
                        chip.Clock = (((Vgm)driverVirtual).HuC6280ClockValue & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriHuC = 1;
                        else chipLED.SecHuC = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.HuC6280 : EnmChip.S_HuC6280);
                    }
                }

                if (((Vgm)driverVirtual).C140ClockValue != 0)
                {
                    MDSound.c140 c140 = new ();
                    for (int i = 0; i < (((Vgm)driverVirtual).C140DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.C140;
                        chip.ID = (byte)i;
                        chip.Instrument = c140;
                        chip.Update = c140.Update;
                        chip.Start = c140.Start;
                        chip.Stop = c140.Stop;
                        chip.Reset = c140.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.C140Volume;
                        chip.Clock = ((Vgm)driverVirtual).C140ClockValue;
                        chip.Option = new object[1] { ((Vgm)driverVirtual).C140Type };

                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriC140 = 1;
                        else chipLED.SecC140 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.C140 : EnmChip.S_C140);
                    }
                }

                if (((Vgm)driverVirtual).K053260ClockValue != 0)
                {
                    MDSound.K053260 k053260 = new();

                    for (int i = 0; i < (((Vgm)driverVirtual).K053260DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K053260;
                        chip.ID = (byte)i;
                        chip.Instrument = k053260;
                        chip.Update = k053260.Update;
                        chip.Start = k053260.Start;
                        chip.Stop = k053260.Stop;
                        chip.Reset = k053260.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.K053260Volume;
                        chip.Clock = ((Vgm)driverVirtual).K053260ClockValue;
                        chip.Option = null;
                        clockK053260 = (int)chip.Clock;

                        if (i == 0) chipLED.PriK053260 = 1;
                        else chipLED.SecK053260 = 1;

                        hiyorimiDeviceFlag |= 0x2;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.K053260 : EnmChip.S_K053260);
                    }
                }

                if (((Vgm)driverVirtual).POKEYClockValue != 0)
                {
                    MDSound.pokey pokey = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).POKEYDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.POKEY;
                        chip.ID = (byte)i;
                        chip.Instrument = pokey;
                        chip.Update = pokey.Update;
                        chip.Start = pokey.Start;
                        chip.Stop = pokey.Stop;
                        chip.Reset = pokey.Reset;
                        chip.SamplingRate = (((Vgm)driverVirtual).POKEYClockValue & 0x3fffffff);// (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.POKEYVolume;
                        chip.Clock = (((Vgm)driverVirtual).POKEYClockValue & 0x3fffffff);
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriPOK = 1;
                        else chipLED.SecPOK = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.POKEY : EnmChip.S_POKEY);
                    }
                }

                if (((Vgm)driverVirtual).QSoundClockValue != 0)
                {
                    MDSound.Qsound_ctr qsound = new();
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.QSoundCtr;
                    chip.ID = (byte)0;
                    chip.Instrument = qsound;
                    chip.Update = qsound.Update;
                    chip.Start = qsound.Start;
                    chip.Stop = qsound.Stop;
                    chip.Reset = qsound.Reset;
                    chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                    chip.Volume = setting.balance.QSoundVolume;
                    chip.Clock = (((Vgm)driverVirtual).QSoundClockValue);// & 0x7fffffff);
                    chip.Option = null;

                    hiyorimiDeviceFlag |= 0x2;

                    //if (i == 0) chipLED.PriHuC = 1;
                    //else chipLED.SecHuC = 1;
                    chipLED.PriQsnd = 1;

                    lstChips.Add(chip);
                    useChip.Add(EnmChip.QSound);
                }

                if (((Vgm)driverVirtual).WSwanClockValue != 0)
                {
                    MDSound.ws_audio WSwan = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).WSwanDualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.WSwan;
                        chip.ID = (byte)i;
                        chip.Instrument = WSwan;
                        chip.Update = WSwan.Update;
                        chip.Start = WSwan.Start;
                        chip.Stop = WSwan.Stop;
                        chip.Reset = WSwan.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.WSwanVolume;
                        chip.Clock = (((Vgm)driverVirtual).WSwanClockValue & 0x3fffffff);
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriWSW = 1;
                        else chipLED.SecWSW = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.WSwan : EnmChip.S_WSwan);
                    }
                }

                if (((Vgm)driverVirtual).SAA1099ClockValue != 0)
                {
                    MDSound.saa1099 saa1099 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).SAA1099DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.SAA1099;
                        chip.ID = (byte)i;
                        chip.Instrument = saa1099;
                        chip.Update = saa1099.Update;
                        chip.Start = saa1099.Start;
                        chip.Stop = saa1099.Stop;
                        chip.Reset = saa1099.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.SAA1099Volume;
                        chip.Clock = (((Vgm)driverVirtual).SAA1099ClockValue & 0x3fffffff);
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriSAA = 1;
                        else chipLED.SecSAA = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.SAA1099 : EnmChip.S_SAA1099);
                    }
                }

                if (((Vgm)driverVirtual).ES5503ClockValue != 0)
                {
                    MDSound.Es5503 es5503 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).ES5503DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.ES5503;
                        chip.ID = (byte)i;
                        chip.Instrument = es5503;
                        chip.Update = es5503.Update;
                        chip.Start = es5503.Start;
                        chip.Stop = es5503.Stop;
                        chip.Reset = es5503.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.ES5503Volume;
                        chip.Clock = (((Vgm)driverVirtual).ES5503ClockValue & 0x3fffffff);
                        chip.Option = new object[] { (byte)((Vgm)driverVirtual).ES5503Ch };
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriES53 = 1;
                        else chipLED.SecES53 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.ES5503 : EnmChip.S_ES5503);
                    }
                }

                if (((Vgm)driverVirtual).X1_010ClockValue != 0)
                {
                    MDSound.x1_010 X1_010 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).X1_010DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.X1_010;
                        chip.ID = (byte)i;
                        chip.Instrument = X1_010;
                        chip.Update = X1_010.Update;
                        chip.Start = X1_010.Start;
                        chip.Stop = X1_010.Stop;
                        chip.Reset = X1_010.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.X1_010Volume;
                        chip.Clock = (((Vgm)driverVirtual).X1_010ClockValue & 0x3fffffff);
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriX1010 = 1;
                        else chipLED.SecX1010 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.X1_010 : EnmChip.S_X1_010);
                    }
                }

                if (((Vgm)driverVirtual).C352ClockValue != 0)
                {
                    MDSound.c352 c352 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).C352DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.C352;
                        chip.ID = (byte)i;
                        chip.Instrument = c352;
                        chip.Update = c352.Update;
                        chip.Start = c352.Start;
                        chip.Stop = c352.Stop;
                        chip.Reset = c352.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.C352Volume;
                        chip.Clock = (((Vgm)driverVirtual).C352ClockValue & 0x7fffffff);
                        chip.Option = new object[1] { (((Vgm)driverVirtual).C352ClockDivider) };
                        int divider = (ushort)((((Vgm)driverVirtual).C352ClockDivider) != 0 ? (((Vgm)driverVirtual).C352ClockDivider) : 288);
                        clockC352 = (int)(chip.Clock / divider);
                        c352.c352_set_options((byte)(((Vgm)driverVirtual).C352ClockValue >> 31));
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriC352 = 1;
                        else chipLED.SecC352 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.C352 : EnmChip.S_C352);
                    }
                }

                if (((Vgm)driverVirtual).GA20ClockValue != 0)
                {
                    MDSound.iremga20 ga20 = new();
                    for (int i = 0; i < (((Vgm)driverVirtual).GA20DualChipFlag ? 2 : 1); i++)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.GA20;
                        chip.ID = (byte)i;
                        chip.Instrument = ga20;
                        chip.Update = ga20.Update;
                        chip.Start = ga20.Start;
                        chip.Stop = ga20.Stop;
                        chip.Reset = ga20.Reset;
                        chip.SamplingRate = (UInt32)setting.outputDevice.SampleRate;
                        chip.Volume = setting.balance.GA20Volume;
                        chip.Clock = (((Vgm)driverVirtual).GA20ClockValue & 0x7fffffff);
                        chip.Option = null;
                        hiyorimiDeviceFlag |= 0x2;

                        if (i == 0) chipLED.PriGA20 = 1;
                        else chipLED.SecGA20 = 1;

                        lstChips.Add(chip);
                        useChip.Add(i == 0 ? EnmChip.GA20 : EnmChip.S_GA20);
                    }
                }



                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;



                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, lstChips.ToArray());

                chipRegister.initChipRegister(lstChips.ToArray());



                if (useChip.Contains(EnmChip.YM2203) || useChip.Contains(EnmChip.S_YM2203))
                {
                    chipRegister.setYM2203Register(0, 0x7, 0x3f, EnmModel.RealModel);//出力オフ
                    chipRegister.setYM2203Register(1, 0x7, 0x3f, EnmModel.RealModel);
                    chipRegister.setYM2203Register(0, 0x8, 0x0, EnmModel.RealModel);
                    chipRegister.setYM2203Register(1, 0x8, 0x0, EnmModel.RealModel);
                    chipRegister.setYM2203Register(0, 0x9, 0x0, EnmModel.RealModel);
                    chipRegister.setYM2203Register(1, 0x9, 0x0, EnmModel.RealModel);
                    chipRegister.setYM2203Register(0, 0xa, 0x0, EnmModel.RealModel);
                    chipRegister.setYM2203Register(1, 0xa, 0x0, EnmModel.RealModel);
                    SetYM2203FMVolume(true, setting.balance.YM2203FMVolume);
                    SetYM2203PSGVolume(true, setting.balance.YM2203PSGVolume);
                }

                if (useChip.Contains(EnmChip.YM2608) || useChip.Contains(EnmChip.S_YM2608))
                {
                    SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                    SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                    SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                    SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);
                }

                if (useChip.Contains(EnmChip.YM2610) || useChip.Contains(EnmChip.S_YM2610))
                {

                    SetYM2610FMVolume(true, setting.balance.YM2610FMVolume);
                    SetYM2610PSGVolume(true, setting.balance.YM2610PSGVolume);
                    SetYM2610AdpcmAVolume(true, setting.balance.YM2610AdpcmAVolume);
                    SetYM2610AdpcmBVolume(true, setting.balance.YM2610AdpcmBVolume);
                }

                if (useChip.Contains(EnmChip.AY8910))
                    chipRegister.writeAY8910Clock(0, (int)((Vgm)driverVirtual).AY8910ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_AY8910))
                    chipRegister.writeAY8910Clock(1, (int)((Vgm)driverVirtual).AY8910ClockValue, EnmModel.RealModel);

                if (useChip.Contains(EnmChip.YM2151))
                    chipRegister.writeYM2151Clock(0, (int)((Vgm)driverVirtual).YM2151ClockValue, EnmModel.RealModel);
                chipRegister.use4MYM2151scci[0] = false;
                if (setting.YM2151Type[0].UseRealChipFreqDiff != null
                    && setting.YM2151Type[0].UseRealChipFreqDiff.Length >0
                    && setting.YM2151Type[0].UseRealChipFreqDiff[0] 
                    && ((Vgm)driverVirtual).YM2151ClockValue == 4000_000)
                {
                    chipRegister.use4MYM2151scci[0] = true;
                }
                if (useChip.Contains(EnmChip.S_YM2151))
                    chipRegister.writeYM2151Clock(1, (int)((Vgm)driverVirtual).YM2151ClockValue, EnmModel.RealModel);
                chipRegister.use4MYM2151scci[1] = false;
                if (setting.YM2151Type[1].UseRealChipFreqDiff != null
                    && setting.YM2151Type[1].UseRealChipFreqDiff.Length > 0
                    && setting.YM2151Type[1].UseRealChipFreqDiff[0] && ((Vgm)driverVirtual).YM2151ClockValue == 4000_000)
                {
                    chipRegister.use4MYM2151scci[1] = true;
                }
                if (useChip.Contains(EnmChip.YM2203))
                    chipRegister.writeYM2203Clock(0, (int)((Vgm)driverVirtual).YM2203ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2203))
                    chipRegister.writeYM2203Clock(1, (int)((Vgm)driverVirtual).YM2203ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.YM2608))
                    chipRegister.writeYM2608Clock(0, (int)((Vgm)driverVirtual).YM2608ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.S_YM2608))
                    chipRegister.writeYM2608Clock(1, (int)((Vgm)driverVirtual).YM2608ClockValue, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.YM3526))
                {
                    chipRegister.setYM3526Register(0, 0xbd, 0, EnmModel.RealModel);//リズムモードオフ
                    chipRegister.writeYM3526Clock(0, (int)((Vgm)driverVirtual).YM3526ClockValue, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.S_YM3526))
                {
                    chipRegister.setYM3526Register(1, 0xbd, 0, EnmModel.RealModel);//リズムモードオフ
                    chipRegister.writeYM3526Clock(1, (int)((Vgm)driverVirtual).YM3526ClockValue, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.YM3812))
                {
                    chipRegister.setYM3812Register(0, 0xbd, 0, EnmModel.RealModel);//リズムモードオフ
                    chipRegister.writeYM3812Clock(0, (int)((Vgm)driverVirtual).YM3812ClockValue, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.S_YM3812))
                {
                    chipRegister.setYM3812Register(1, 0xbd, 0, EnmModel.RealModel);//リズムモードオフ
                    chipRegister.writeYM3812Clock(1, (int)((Vgm)driverVirtual).YM3812ClockValue, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.YMF262))
                {
                    chipRegister.setYMF262Register(0, 0, 0xbd, 0, EnmModel.RealModel);//リズムモードオフ
                    chipRegister.setYMF262Register(0, 1,    5, 1, EnmModel.RealModel);//opl3mode
                    chipRegister.writeYMF262Clock(0, (int)((Vgm)driverVirtual).YMF262ClockValue, EnmModel.RealModel);
                }
                if (useChip.Contains(EnmChip.S_YMF262))
                {
                    chipRegister.setYMF262Register(1, 0, 0xbd, 0, EnmModel.RealModel);//リズムモードオフ
                    chipRegister.setYMF262Register(1, 1,    5, 1, EnmModel.RealModel);//opl3mode
                    chipRegister.writeYMF262Clock(1, (int)((Vgm)driverVirtual).YMF262ClockValue, EnmModel.RealModel);
                }
                if (SN76489NGPFlag)
                {
                    chipRegister.setSN76489Register(0, 0xe5, EnmModel.RealModel);//white noise mode 
                    chipRegister.setSN76489Register(1, 0xe5, EnmModel.RealModel);//white noise mode 
                    chipRegister.setSN76489Register(0, 0xe5, EnmModel.VirtualModel);//white noise mode 
                    chipRegister.setSN76489Register(1, 0xe5, EnmModel.VirtualModel);//white noise mode 
                }
                if (useChip.Contains(EnmChip.YM2610))
                {
                    //control2 レジスタのパンをセンターに予め設定
                    chipRegister.setYM2610Register(0, 0, 0x11, 0xc0, EnmModel.RealModel);
                    chipRegister.setYM2610Register(0, 0, 0x11, 0xc0, EnmModel.VirtualModel);
                }
                if (useChip.Contains(EnmChip.S_YM2610))
                {
                    //control2 レジスタのパンをセンターに予め設定
                    chipRegister.setYM2610Register(1, 0, 0x11, 0xc0, EnmModel.RealModel);
                    chipRegister.setYM2610Register(1, 0, 0x11, 0xc0, EnmModel.VirtualModel);
                }
                if (useChip.Contains(EnmChip.C140))
                    chipRegister.writeC140Type(0, ((Vgm)driverVirtual).C140Type, EnmModel.RealModel);
                if (useChip.Contains(EnmChip.SEGAPCM))
                    chipRegister.writeSEGAPCMClock(0, (int)((Vgm)driverVirtual).SEGAPCMClockValue, EnmModel.RealModel);

                int SSGVolumeFromTAG = -1;
                if (driverReal != null)
                {
                    if (((Vgm)driverReal).GD3.SystemNameJ.IndexOf("9801") > 0) SSGVolumeFromTAG = 31;
                    if (((Vgm)driverReal).GD3.SystemNameJ.IndexOf("8801") > 0) SSGVolumeFromTAG = 63;
                    if (((Vgm)driverReal).GD3.SystemNameJ.IndexOf("PC-88") > 0) SSGVolumeFromTAG = 63;
                    if (((Vgm)driverReal).GD3.SystemNameJ.IndexOf("PC88") > 0) SSGVolumeFromTAG = 63;
                    if (((Vgm)driverReal).GD3.SystemNameJ.IndexOf("PC-98") > 0) SSGVolumeFromTAG = 31;
                    if (((Vgm)driverReal).GD3.SystemNameJ.IndexOf("PC98") > 0) SSGVolumeFromTAG = 31;
                    if (((Vgm)driverReal).GD3.SystemName.IndexOf("9801") > 0) SSGVolumeFromTAG = 31;
                    if (((Vgm)driverReal).GD3.SystemName.IndexOf("8801") > 0) SSGVolumeFromTAG = 63;
                    if (((Vgm)driverReal).GD3.SystemName.IndexOf("PC-88") > 0) SSGVolumeFromTAG = 63;
                    if (((Vgm)driverReal).GD3.SystemName.IndexOf("PC88") > 0) SSGVolumeFromTAG = 63;
                    if (((Vgm)driverReal).GD3.SystemName.IndexOf("PC-98") > 0) SSGVolumeFromTAG = 31;
                    if (((Vgm)driverReal).GD3.SystemName.IndexOf("PC98") > 0) SSGVolumeFromTAG = 31;
                }

                if (SSGVolumeFromTAG == -1)
                {
                    if (useChip.Contains(EnmChip.YM2203))
                        chipRegister.setYM2203SSGVolume(0, setting.balance.GimicOPNVolume, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2203))
                        chipRegister.setYM2203SSGVolume(1, setting.balance.GimicOPNVolume, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.YM2608))
                        chipRegister.setYM2608SSGVolume(0, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2608))
                        chipRegister.setYM2608SSGVolume(1, setting.balance.GimicOPNAVolume, EnmModel.RealModel);
                }
                else
                {
                    if (useChip.Contains(EnmChip.YM2203))
                        chipRegister.setYM2203SSGVolume(0, SSGVolumeFromTAG, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2203))
                        chipRegister.setYM2203SSGVolume(1, SSGVolumeFromTAG, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.YM2608))
                        chipRegister.setYM2608SSGVolume(0, SSGVolumeFromTAG, EnmModel.RealModel);
                    if (useChip.Contains(EnmChip.S_YM2608))
                        chipRegister.setYM2608SSGVolume(1, SSGVolumeFromTAG, EnmModel.RealModel);
                }

                driverVirtual.SetYM2151Hosei(((Vgm)driverVirtual).YM2151ClockValue);
                driverReal?.SetYM2151Hosei(((Vgm)driverReal).YM2151ClockValue);


                //frmMain.ForceChannelMask(EnmChip.YM2612, 0, 0, true);

                Paused = false;
                oneTimeReset = false;

                Thread.Sleep(500);

                //Stopped = false;

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }



        public static void GO()
        {
            Stopped = false;
        }

        private static void ClearFadeoutVolume()
        {
            chipRegister.setFadeoutVolYM2203(0, 0);
            chipRegister.setFadeoutVolYM2203(1, 0);
            chipRegister.setFadeoutVolAY8910(0, 0);
            chipRegister.setFadeoutVolAY8910(1, 0);
            chipRegister.setFadeoutVolYM2413(0, 0);
            chipRegister.setFadeoutVolYM2413(1, 0);
            chipRegister.setFadeoutVolYM2608(0, 0);
            chipRegister.setFadeoutVolYM2608(1, 0);
            chipRegister.setFadeoutVolYM2151(0, 0);
            chipRegister.setFadeoutVolYM2151(1, 0);
            chipRegister.setFadeoutVolYM2612(0, 0);
            chipRegister.setFadeoutVolYM2612(1, 0);
            chipRegister.setFadeoutVolSN76489(0, 0);
            chipRegister.setFadeoutVolSN76489(1, 0);
            chipRegister.setFadeoutVolYM3526(0, 0);
            chipRegister.setFadeoutVolYM3526(1, 0);
            chipRegister.setFadeoutVolYM3812(0, 0);
            chipRegister.setFadeoutVolYM3812(1, 0);
            chipRegister.setFadeoutVolYMF262(0, 0);
            chipRegister.setFadeoutVolYMF262(1, 0);
        }

        private static void ResetFadeOutParam()
        {
            vgmFadeout = false;
            vgmFadeoutCounter = 1.0;
            vgmFadeoutCounterV = 0.00001;
            vgmSpeed = 1;
            vgmRealFadeoutVol = 0;
            vgmRealFadeoutVolWait = 4;

            ClearFadeoutVolume();

            chipRegister.resetChips();
        }

        public static void ChangeChipSampleRate(MDSound.MDSound.Chip chip, int NewSmplRate)
        {
            MDSound.MDSound.Chip CAA = chip;

            if (CAA.SamplingRate == NewSmplRate)
                return;

            // quick and dirty hack to make sample rate changes work
            CAA.SamplingRate = (uint)NewSmplRate;
            if (CAA.SamplingRate < setting.outputDevice.SampleRate)//SampleRate)
                CAA.Resampler = 0x01;
            else if (CAA.SamplingRate == setting.outputDevice.SampleRate)//SampleRate)
                CAA.Resampler = 0x02;
            else if (CAA.SamplingRate > setting.outputDevice.SampleRate)//SampleRate)
                CAA.Resampler = 0x03;
            CAA.SmpP = 1;
            CAA.SmpNext -= CAA.SmpLast;
            CAA.SmpLast = 0x00;

            return;
        }

        public static void FF()
        {
            if (driverVirtual == null) return;
            vgmSpeed = (vgmSpeed == 1) ? 4 : 1;
            driverVirtual.vgmSpeed = vgmSpeed;
            if (driverReal != null) driverReal.vgmSpeed = vgmSpeed;
        }

        public static void Slow()
        {
            vgmSpeed = (vgmSpeed == 1) ? 0.25 : 1;
            driverVirtual.vgmSpeed = vgmSpeed;
            if (driverReal != null) driverReal.vgmSpeed = vgmSpeed;
        }

        public static void ResetSlow()
        {
            vgmSpeed = 1;
            driverVirtual.vgmSpeed = vgmSpeed;
            if (driverReal != null) driverReal.vgmSpeed = vgmSpeed;
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

        public static bool IsPaused
        {
            get
            {
                return Paused;
            }
        }

        public static bool IsStopped
        {
            get
            {
                return Stopped;
            }
        }

        public static bool IsFadeOut
        {
            get
            {
                return vgmFadeout;
            }
        }
        public static bool IsSlow {
            get
            {
                return !IsStopped && (vgmSpeed < 1.0);
            }
        }
        public static bool IsFF
        {
            get
            {
                return !IsStopped && (vgmSpeed > 1.0);
            }
        }

        public static bool SN76489NGPFlag { get; private set; } = false;

        public static void StepPlay(int Step)
        {
            StepCounter = Step;
        }

        public static void Fadeout()
        {
            vgmFadeout = true;
        }

        public static void CloseWaveWriter()
        {
            waveWriter.Close();
        }

        public static void Stop()
        {

            try
            {
                if(Paused) Pause();

                if (Stopped)
                {
                    trdClosed = true;
                    while (!TrdStopped) { Thread.Sleep(1); }

                    if ((PlayingFileFormat != EnmFileFormat.WAV 
                        || PlayingFileFormat != EnmFileFormat.MP3 
                        || PlayingFileFormat != EnmFileFormat.AIFF )
                        && naudioFileReader != null)
                    {
                        NAudioStop();
                    }

                    return;
                }

                if (!Paused)
                {
                    PlaybackState? ps = naudioWrap.GetPlaybackState();
                    if (ps != null && ps != PlaybackState.Stopped)
                    {
                        vgmFadeoutCounterV = 0.1;
                        vgmFadeout = true;
                        int cnt = 0;
                        while (!Stopped && cnt < 100)
                        {
                            Thread.Sleep(1);
                            System.Windows.Forms.Application.DoEvents();
                            cnt++;
                        }
                    }
                }
                trdClosed = true;

                if (naudioFileReader != null)
                {
                    NAudioStop();
                    return;
                }

                SoftReset(EnmModel.VirtualModel);
                SoftReset(EnmModel.RealModel);

                int timeout = 5000;
                while (!TrdStopped)
                {
                    Thread.Sleep(1);
                    timeout--;
                    if (timeout < 1) break;
                };
                while (!Stopped)
                {
                    Thread.Sleep(1);
                    timeout--;
                    if (timeout < 1) break;
                };
                Stopped = true;

                SoftReset(EnmModel.VirtualModel);
                SoftReset(EnmModel.RealModel);

                //chipRegister.outMIDIData_Close();
                if (setting.other.WavSwitch)
                {
                    Thread.Sleep(500);
                    waveWriter.Close();
                }

                //DEBUG
                //vstparse();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

        }

        private static void NAudioStop()
        {
            try
            {
                AudioFileReader dmy = naudioFileReader;
                NAudio.Wave.SampleProviders.SampleToWaveProvider16 dmy2 = naudioWs;
                naudioFileReader = null;
                naudioWs = null;
                dmy.Dispose();
            }
            catch { }
        }

        public static void Close(bool isRealChipClose=true)
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

                vstMng.ReleaseAllMIDIout();
                vstMng.Close();

                if (isRealChipClose) realChip = null;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public static void ResetTimeCounter()
        {
            if (driverVirtual == null && driverReal == null) return;
            if (driverVirtual != null)
            {
                driverVirtual.Counter = 0;
                driverVirtual.TotalCounter = 0;
                driverVirtual.LoopCounter = 0;
            }

            if (driverReal != null)
            {
                driverReal.Counter = 0;
                driverReal.TotalCounter = 0;
                driverReal.LoopCounter = 0;
            }

        }

        public static long GetCounter()
        {
            if (driverVirtual == null && driverReal == null) return -1;

            if (driverVirtual == null) return driverReal.Counter;
            if (driverReal == null) return driverVirtual.Counter;

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
                return driverReal is NRTDRV nrtdrv 
                    ? nrtdrv.work.TOTALCOUNT 
                    : driverReal is Vgm vgm 
                    ? vgm.vgmFrameCounter 
                    : 0;
            if (driverReal == null)
                return driverVirtual is NRTDRV nrtdrv 
                    ? nrtdrv.work.TOTALCOUNT 
                    : driverVirtual is Vgm vgm 
                    ? vgm.vgmFrameCounter 
                    : 0;

            if (driverVirtual is NRTDRV nrtV && driverReal is NRTDRV nrtR)
            {
                ushort nVTC = nrtV.work.TOTALCOUNT;
                ushort nRTC = nrtR.work.TOTALCOUNT;
                return nVTC > nRTC ? nVTC : nRTC;
            }
            else if (driverVirtual is Vgm vgmV && driverReal is Vgm vgmR)
            {
                long vVFC = vgmV.vgmFrameCounter;
                long vRFC = vgmR.vgmFrameCounter;
                return vVFC > vRFC ? vVFC : vRFC;
            }

            return 0;
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
            chips[17] = chipRegister.chipLED.PriRF5C68;
            chipRegister.chipLED.PriRF5C68 = chipLED.PriRF5C68;


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
            chips[128 + 15] = chipRegister.chipLED.SecC352;
            chipRegister.chipLED.SecC352 = chipLED.SecC352;
            chips[128 + 16] = chipRegister.chipLED.SecK054539;
            chipRegister.chipLED.SecK054539 = chipLED.SecK054539;
            chips[128 + 17] = chipRegister.chipLED.SecRF5C68;
            chipRegister.chipLED.SecRF5C68 = chipLED.SecRF5C68;


            return chips;
        }

        public static void UpdateVol()
        {
            chipRegister.updateVol();
        }

        public static uint GetVgmCurLoopCounter()
        {
            uint cnt = 0;

            if (driverVirtual != null && !driverVirtual.Stopped)
            {
                cnt = driverVirtual.vgmCurLoop;
            }
            if (driverReal != null && !driverReal.Stopped)
            {
                cnt = Math.Min(driverReal.vgmCurLoop, cnt);
            }

            return cnt;
        }

        public static bool GetVGMStopped()
        {
            bool v;
            bool r;

            v = driverVirtual == null || driverVirtual.Stopped;
            r = driverReal == null || driverReal.Stopped;
            return v && r;
        }

        public static bool GetIsDataBlock(EnmModel model)
        {

            if (model == EnmModel.VirtualModel)
            {
                if (driverVirtual == null) return false;
                return driverVirtual.isDataBlock;
            }
            else
            {
                if (driverReal == null) return false;
                return driverReal.isDataBlock;
            }
        }

        public static bool GetIsPcmRAMWrite(EnmModel model)
        {
            if (model == EnmModel.VirtualModel)
            {
                if (driverVirtual == null) return false;
                if (!(driverVirtual is Vgm)) return false;
                return ((Vgm)driverVirtual).isPcmRAMWrite;
            }
            else
            {
                if (driverReal == null) return false;
                if (!(driverReal is Vgm)) return false;
                return ((Vgm)driverReal).isPcmRAMWrite;
            }
        }

        private static void NaudioWrap_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                if (e.Exception.Message.Contains("NoDriver calling waveOutWrite", StringComparison.CurrentCulture)
                    || e.Exception.Message.Contains("HRESULT", StringComparison.CurrentCulture)
                    || e.Exception.Message.Contains("DirectSound buffer", StringComparison.CurrentCulture))
                {
                    log.ForcedWrite(e.Exception);
                }
                else
                {
                    log.ForcedWrite(e.Exception);
                    System.Windows.Forms.MessageBox.Show(
                        string.Format("デバイスが何らかの原因で停止しました。\r\nメッセージ:\r\n{0}\r\nスタックトレース:\r\n{1}"
                        , e.Exception.Message
                        , e.Exception.StackTrace)
                        , "エラー"
                        , System.Windows.Forms.MessageBoxButtons.OK
                        , System.Windows.Forms.MessageBoxIcon.Error);
                }

                flgReinit = true;

                try
                {
                    Stopped = true;
                    Stop();
                }
                catch { }
                try
                {
                    naudioWrap.Stop();
                }
                catch (Exception ex)
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

        private static void StartTrdVgmReal()
        {
            if (setting.outputDevice.DeviceType == Common.DEV_Null)
            {
                return;
            }

            trdClosed = false;
            trdMain = new Thread(new ThreadStart(TrdVgmRealFunction));
            trdMain.Priority = ThreadPriority.Highest;
            trdMain.IsBackground = true;
            trdMain.Name = "trdVgmReal";
            trdMain.Start();
        }

        private static void TrdVgmRealFunction()
        {

            if (driverReal == null)
            {
                trdClosed = true;
                TrdStopped = true;
                return;
            }

            double o = sw.ElapsedTicks / swFreq;
            double step = 1 / (double)setting.outputDevice.SampleRate;
            TrdStopped = false;
            try
            {
                while (!trdClosed)
                {
                    Thread.Sleep(0);

                    double el1 = sw.ElapsedTicks / swFreq;
                    if (el1 - o < step)
                        continue;
                    if (el1 - o >= step * setting.outputDevice.SampleRate / 1.0)//閾値1000ms  //100.0)//閾値10ms
                    {
                        do
                        {
                            o += step;
                        } while (el1 - o >= step);
                    }
                    else
                    {
                        o += step;
                    }

                    if (Stopped || Paused)
                    {
                        if (realChip != null && !oneTimeReset)
                        {
                            SoftReset(EnmModel.RealModel);
                            oneTimeReset = true;
                            chipRegister.resetAllMIDIout();
                        }
                        continue;
                    }
                    if (hiyorimiNecessary && driverVirtual.isDataBlock) { continue; }

                    if (vgmFadeout)
                    {
                        if (vgmRealFadeoutVol != 1000) vgmRealFadeoutVolWait--;
                        if (vgmRealFadeoutVolWait == 0)
                        {
                            if (useChip.Contains(EnmChip.YM2151)) chipRegister.setFadeoutVolYM2151(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM2203)) chipRegister.setFadeoutVolYM2203(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.AY8910)) chipRegister.setFadeoutVolAY8910(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM2413)) chipRegister.setFadeoutVolYM2413(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM2608)) chipRegister.setFadeoutVolYM2608(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM2610)) chipRegister.setFadeoutVolYM2610(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM2612)) chipRegister.setFadeoutVolYM2612(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM3526)) chipRegister.setFadeoutVolYM3526(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YM3812)) chipRegister.setFadeoutVolYM3812(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.SN76489)) chipRegister.setFadeoutVolSN76489(0, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.YMF262)) chipRegister.setFadeoutVolYMF262(0, vgmRealFadeoutVol);

                            if (useChip.Contains(EnmChip.S_YM2151)) chipRegister.setFadeoutVolYM2151(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM2203)) chipRegister.setFadeoutVolYM2203(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_AY8910)) chipRegister.setFadeoutVolAY8910(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM2413)) chipRegister.setFadeoutVolYM2413(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM2608)) chipRegister.setFadeoutVolYM2608(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM2610)) chipRegister.setFadeoutVolYM2610(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM2612)) chipRegister.setFadeoutVolYM2612(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM3526)) chipRegister.setFadeoutVolYM3526(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YM3812)) chipRegister.setFadeoutVolYM3812(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_SN76489)) chipRegister.setFadeoutVolSN76489(1, vgmRealFadeoutVol);
                            if (useChip.Contains(EnmChip.S_YMF262)) chipRegister.setFadeoutVolYMF262(1, vgmRealFadeoutVol);

                            vgmRealFadeoutVol++;

                            vgmRealFadeoutVol = Math.Min(127, vgmRealFadeoutVol);
                            if (vgmRealFadeoutVol == 127)
                            {
                                if (realChip != null)
                                {
                                    SoftReset(EnmModel.RealModel);
                                }
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
                        //long v;
                        //v = driverReal.vgmFrameCounter - driverVirtual.vgmFrameCounter;
                        //long d = setting.outputDevice.SampleRate * (setting.LatencySCCI - setting.outputDevice.SampleRate * setting.LatencyEmulation) / 1000;
                        //long l = getLatency() / 4;

                        //int m = 0;
                        //if (d >= 0)
                        //{
                        //    if (v >= d - l && v <= d + l) m = 0;
                        //    else m = (v + d > l) ? 1 : 2;
                        //}
                        //else
                        //{
                        //    d = Math.Abs(setting.outputDevice.SampleRate * ((uint)setting.LatencyEmulation - (uint)setting.LatencySCCI) / 1000);
                        //    if (v >= d - l && v <= d + l) m = 0;
                        //    else m = (v - d > l) ? 1 : 2;
                        //}

                        double dEMU = setting.outputDevice.SampleRate * setting.LatencyEmulation / 1000.0;
                        double dSCCI = setting.outputDevice.SampleRate * setting.LatencySCCI / 1000.0;
                        double abs = Math.Abs((driverReal.vgmFrameCounter - dSCCI) - (driverVirtual.vgmFrameCounter - dEMU));
                        int m = 0;
                        long l = GetLatency() / 10;
                        if (abs >= l)
                        {
                            m = ((driverReal.vgmFrameCounter - dSCCI) > (driverVirtual.vgmFrameCounter - dEMU)) ? 1 : 2;
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
            TrdStopped = true;
        }

        private static void SoftReset(EnmModel model)
        {
            chipRegister.softResetYM2203(0, model);
            chipRegister.softResetYM2203(1, model);
            chipRegister.softResetAY8910(0, model);
            chipRegister.softResetAY8910(1, model);
            chipRegister.softResetYM2413(0, model);
            chipRegister.softResetYM2413(1, model);
            chipRegister.softResetYM2608(0, model);
            chipRegister.softResetYM2608(1, model);
            chipRegister.softResetYM2151(0, model);
            chipRegister.softResetYM2151(1, model);
            chipRegister.softResetYM3526(0, model);
            chipRegister.softResetYM3526(1, model);
            chipRegister.softResetYM3812(0, model);
            chipRegister.softResetYM3812(1, model);
            chipRegister.softResetYMF262(0, model);
            chipRegister.softResetYMF262(1, model);
            chipRegister.softResetK051649(0, model);
            chipRegister.softResetK051649(1, model);
            chipRegister.softResetC140(0, model);
            chipRegister.softResetC140(1, model);
            chipRegister.softResetSEGAPCM(0, model);
            chipRegister.softResetSEGAPCM(1, model);
            chipRegister.softResetMIDI(0, model);
            chipRegister.softResetMIDI(1, model);

            if (model == EnmModel.RealModel && realChip != null)
            {
                realChip.SendData();
            }
        }

        private static short[] bufVirtualFunction_MIDIKeyboard = null;

        internal static int TrdVgmVirtualFunction(short[] buffer, int offset, int sampleCount)
        {
            //return NaudioRead(buffer, offset, sampleCount);

            if (naudioFileReader != null)
            {
                if (trdClosed)
                {
                    TrdStopped = true;
                    //vgmFadeout = false;
                    //Stopped = true;
                }
                return NaudioRead(buffer, offset, sampleCount);
            }

            int cnt = TrdVgmVirtualMainFunction(buffer, offset, sampleCount);

            if (setting.midiKbd.UseMIDIKeyboard)
            {
                if (bufVirtualFunction_MIDIKeyboard == null || bufVirtualFunction_MIDIKeyboard.Length < sampleCount)
                {
                    bufVirtualFunction_MIDIKeyboard = new short[sampleCount];
                }
                mdsMIDI.Update(bufVirtualFunction_MIDIKeyboard, 0, sampleCount, null);
                for (int i = 0; i < sampleCount; i++)
                {
                    buffer[i + offset] += bufVirtualFunction_MIDIKeyboard[i];
                }
            }

            return cnt;
        }

        private static int TrdVgmVirtualMainFunction(short[] buffer, int offset, int sampleCount)
        {
            if (buffer == null || buffer.Length < 1 || sampleCount == 0) return 0;
            if (driverVirtual == null) return sampleCount;

            try
            {
                //stwh.Reset(); stwh.Start();

                int i;
                int cnt = 0;

                if (Stopped || Paused)
                {
                    if (setting.other.NonRenderingForPause
                        || driverVirtual is nsf
                        )
                    {
                        for (int d = offset; d < offset + sampleCount; d++) buffer[d] = 0;
                        return sampleCount;
                    }

                    int ret = mds.Update(buffer, offset, sampleCount, null);
                    return ret;
                }

                if (driverVirtual is nsf)
                {
                    driverVirtual.vstDelta = 0;
                    cnt = (Int32)((nsf)driverVirtual).Render(buffer, (UInt32)sampleCount / 2, offset) * 2;
                }
                else if (driverVirtual is Driver.SID.sid)
                {
                    driverVirtual.vstDelta = 0;
                    cnt = (Int32)((Driver.SID.sid)driverVirtual).Render(buffer, (UInt32)sampleCount);
                }
                else if (driverVirtual is Driver.MXDRV.MXDRV)
                {
                    mds.setIncFlag();
                    driverVirtual.vstDelta = 0;
                    for (i = 0; i < sampleCount; i += 2)
                    {
                        cnt = (Int32)((Driver.MXDRV.MXDRV)driverVirtual).Render(buffer, offset + i, 2);
                        mds.Update(buffer, offset + i, 2, null);
                    }
                    //cnt = (Int32)((Driver.MXDRV.MXDRV)driverVirtual).Render(buffer, offset , sampleCount);
                    //mds.Update(buffer, offset , sampleCount, null);
                    cnt = sampleCount;
                }
                else
                {
                    if (hiyorimiNecessary && driverReal!=null && driverReal.isDataBlock)
                        return mds.Update(buffer, offset, sampleCount, null);

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
                    stwh.Reset(); stwh.Start();
                    cnt = mds.Update(buffer, offset, sampleCount, driverVirtual.oneFrameProc);
                    ProcTimePer1Frame = (int)((double)stwh.ElapsedMilliseconds / (sampleCount + 1) * 1000000.0);
                }

                //waveWriter.Write(buffer, offset, sampleCount);
                //VST
                vstMng.VST_Update(buffer, offset, sampleCount);

                for (i = 0; i < sampleCount; i++)
                {
                    int mul = (int)(16384.0 * Math.Pow(10.0, MasterVolume / 40.0));
                    buffer[offset + i] = (short)Limit((buffer[offset + i] * mul) >> 13, 0x7fff, -0x8000);

                    if (!vgmFadeout) continue;

                    //フェードアウト処理
                    buffer[offset + i] = (short)(buffer[offset + i] * vgmFadeoutCounter);

                    vgmFadeoutCounter -= vgmFadeoutCounterV;
                    if (vgmFadeoutCounterV >= 0.004 && vgmFadeoutCounterV != 0.1)
                    {
                        vgmFadeoutCounterV = 0.004;
                    }

                    if (vgmFadeoutCounter < 0.0)
                    {
                        vgmFadeoutCounter = 0.0;
                    }

                    //フェードアウト完了後、演奏を完全停止する
                    if (vgmFadeoutCounter == 0.0)
                    {
                        SoftReset(EnmModel.VirtualModel);
                        SoftReset(EnmModel.RealModel);

                        waveWriter.Write(buffer, offset, i + 1);

                        waveWriter.Close();

                        if (mds == null)
                            mds = new MDSound.MDSound((UInt32)setting.outputDevice.SampleRate, samplingBuffer, null);
                        else
                            mds.Init((UInt32)setting.outputDevice.SampleRate, samplingBuffer, null);


                        chipRegister.Close();
                        
                        //Thread.Sleep(500);//noise対策

                        Stopped = true;

                        //1frame当たりの処理時間
                        //ProcTimePer1Frame = (int)((double)stwh.ElapsedMilliseconds / (i + 1) * 1000000.0);
                        return i + 1;
                    }

                }

                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    UpdateVisualVolume(buffer, offset);
                }

                waveWriter.Write(buffer, offset, sampleCount);

                ////1frame当たりの処理時間
                //ProcTimePer1Frame = (int)((double)stwh.ElapsedMilliseconds / sampleCount * 1000000.0);
                return cnt;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                FatalError = true;
                Stopped = true;
            }

            return -1;
        }

        private static string naudioFileName = null;
        private static AudioFileReader naudioFileReader = null;
        private static NAudio.Wave.SampleProviders.SampleToWaveProvider16 naudioWs = null;
        private static byte[] naudioSrcbuffer = null;

        public static int NaudioRead(short[] buffer, int offset, int count)
        {
            try
            {
                naudioSrcbuffer = Ensure(naudioSrcbuffer, count * 2);
                naudioWs.Read(naudioSrcbuffer, 0, count * 2);
                Convert2byteToShort(buffer, offset, naudioSrcbuffer, count);
            }
            catch
            {

            }

            return count;
        }

        public static byte[] Ensure(byte[] buffer, int bytesRequired)
        {
            if (buffer == null || buffer.Length < bytesRequired)
            {
                buffer = new byte[bytesRequired];
            }
            return buffer;
        }

        private static unsafe void Convert2byteToShort(short[] destBuffer, int offset, byte[] source, int shortCount)
        {
            fixed (short* pDestBuffer = &destBuffer[offset])
            fixed (byte* pSourceBuffer = &source[0])
            {
                short* psDestBuffer = pDestBuffer;
                short* pfSourceBuffer = (short*)pSourceBuffer;

                int samplesRead = shortCount;
                for (int n = 0; n < samplesRead; n++)
                {
                    psDestBuffer[n] = pfSourceBuffer[n];// volume;
                }
            }
        }


        private static void UpdateVisualVolume(short[] buffer, int offset)
        {
            visVolume.master = buffer[offset];

            int[][][] vol = mds.getYM2151VisVolume();
            if (vol != null) visVolume.ym2151 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM2203VisVolume();
            if (vol != null) visVolume.ym2203 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (vol != null) visVolume.ym2203FM = (short)GetMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
            if (vol != null) visVolume.ym2203SSG = (short)GetMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);

            vol = mds.getYM2612VisVolume();
            if (vol != null) visVolume.ym2612 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM2608VisVolume();
            if (vol != null) visVolume.ym2608 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (vol != null) visVolume.ym2608FM = (short)GetMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
            if (vol != null) visVolume.ym2608SSG = (short)GetMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
            if (vol != null) visVolume.ym2608Rtm = (short)GetMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
            if (vol != null) visVolume.ym2608APCM = (short)GetMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);

            vol = mds.getYM2610VisVolume();
            if (vol != null) visVolume.ym2610 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (vol != null) visVolume.ym2610FM = (short)GetMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
            if (vol != null) visVolume.ym2610SSG = (short)GetMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
            if (vol != null) visVolume.ym2610APCMA = (short)GetMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
            if (vol != null) visVolume.ym2610APCMB = (short)GetMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);


            vol = mds.getYM2413VisVolume();
            if (vol != null) visVolume.ym2413 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM3526VisVolume();
            if (vol != null) visVolume.ym3526 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getY8950VisVolume();
            if (vol != null) visVolume.y8950 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM3812VisVolume();
            if (vol != null) visVolume.ym3812 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMF262VisVolume();
            if (vol != null) visVolume.ymf262 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMF278BVisVolume();
            if (vol != null) visVolume.ymf278b = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMF271VisVolume();
            if (vol != null) visVolume.ymf271 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMZ280BVisVolume();
            if (vol != null) visVolume.ymz280b = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            visVolume.ay8910 = 0;
            vol = mds.getAY8910VisVolume();
            if (vol != null) visVolume.ay8910 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            vol = mds.getAY8910mameVisVolume();
            if (vol != null) visVolume.ay8910 += (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getSN76489VisVolume();
            if (vol != null) visVolume.sn76489 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getHuC6280VisVolume();
            if (vol != null) visVolume.huc6280 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);


            vol = mds.getRF5C164VisVolume();
            if (vol != null) visVolume.rf5c164 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getRF5C68VisVolume();
            if (vol != null) visVolume.rf5c68 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getPWMVisVolume();
            if (vol != null) visVolume.pwm = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getOKIM6258VisVolume();
            if (vol != null) visVolume.okim6258 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getOKIM6295VisVolume();
            if (vol != null) visVolume.okim6295 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getC140VisVolume();
            if (vol != null) visVolume.c140 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getC352VisVolume();
            if (vol != null) visVolume.c352 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getSAA1099VisVolume();
            if (vol != null) visVolume.saa1099 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getPPZ8VisVolume();
            if (vol != null) visVolume.ppz8 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getSegaPCMVisVolume();
            if (vol != null) visVolume.segaPCM = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getMultiPCMVisVolume();
            if (vol != null) visVolume.multiPCM = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getK051649VisVolume();
            if (vol != null) visVolume.k051649 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getK053260VisVolume();
            if (vol != null) visVolume.k053260 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getK054539VisVolume();
            if (vol != null) visVolume.k054539 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getQSoundCtrVisVolume();
            if (vol != null) visVolume.qSound = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getGA20VisVolume();
            if (vol != null) visVolume.ga20 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);


            vol = mds.getNESVisVolume();
            if (vol != null) visVolume.APU = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            else visVolume.APU = (short)MDSound.MDSound.np_nes_apu_volume;

            vol = mds.getDMCVisVolume();
            if (vol != null) visVolume.DMC = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            else visVolume.DMC = (short)MDSound.MDSound.np_nes_dmc_volume;

            vol = mds.getFDSVisVolume();
            if (vol != null) visVolume.FDS = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            else visVolume.FDS = (short)MDSound.MDSound.np_nes_fds_volume;

            vol = mds.getMMC5VisVolume();
            if (vol != null) visVolume.MMC5 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.MMC5 == 0) visVolume.MMC5 = (short)MDSound.MDSound.np_nes_mmc5_volume;

            vol = mds.getN160VisVolume();
            if (vol != null) visVolume.N160 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.N160 == 0) visVolume.N160 = (short)MDSound.MDSound.np_nes_n106_volume;

            vol = mds.getVRC6VisVolume();
            if (vol != null) visVolume.VRC6 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.VRC6 == 0) visVolume.VRC6 = (short)MDSound.MDSound.np_nes_vrc6_volume;

            vol = mds.getVRC7VisVolume();
            if (vol != null) visVolume.VRC7 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.VRC7 == 0) visVolume.VRC7 = (short)MDSound.MDSound.np_nes_vrc7_volume;

            vol = mds.getFME7VisVolume();
            if (vol != null) visVolume.FME7 = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.FME7 == 0) visVolume.FME7 = (short)MDSound.MDSound.np_nes_fme7_volume;

            vol = mds.getDMGVisVolume();
            if (vol != null) visVolume.DMG = (short)GetMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
        }

        public static int GetMonoVolume(int pl, int pr, int sl, int sr)
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

        public static long GetVirtualFrameCounter()
        {
            if (driverVirtual == null) return -1;
            return driverVirtual.vgmFrameCounter;
        }

        public static long GetRealFrameCounter()
        {
            if (driverReal == null) return -1;
            return driverReal.vgmFrameCounter;
        }

        public static GD3 GetGD3()
        {
            if (driverVirtual != null) return driverVirtual.GD3;
            return null;
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

        public static ITrackInfo[] GetVRC6Register(int chipID)
        {
            return chipRegister.getVRC6Register(chipID);
        }

        public static byte[] GetVRC7Register(int chipID)
        {
            return chipRegister.getVRC7Register(chipID);
        }

        public static ITrackInfo[] GetN106Register(int chipID)
        {
            return chipRegister.getN106Register(chipID);
        }

        public static int[][] GetYM2608Register(int chipID)
        {
            return chipRegister.fmRegisterYM2608[chipID];
        }

        public static int[][] GetYM2609Register(int chipID)
        {
            return chipRegister.fmRegisterYM2609[chipID];
        }

        public static int[][] GetYM2610Register(int chipID)
        {
            return chipRegister.fmRegisterYM2610[chipID];
        }

        public static int[] GetYM3526Register(int chipID)
        {
            return chipRegister.fmRegisterYM3526[chipID];
        }

        public static int[] GetY8950Register(int chipID)
        {
            return chipRegister.fmRegisterY8950[chipID];
        }

        public static int[] GetYM3812Register(int chipID)
        {
            return chipRegister.fmRegisterYM3812[chipID];
        }

        public static int[][] GetYMF262Register(int chipID)
        {
            return chipRegister.fmRegisterYMF262[chipID];
        }

        public static int[][] GetYMF278BRegister(int chipID)
        {
            return chipRegister.fmRegisterYMF278B[chipID];
        }

        public static int[] GetMoonDriverPCMKeyOn()
        {
            if (driverVirtual is Driver.MoonDriver.MoonDriver)
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

        public static K051649.k051649_state GetK051649Register(int chipID)
        {
            return chipRegister.scc_k051649.GetK051649_State((byte)chipID);//  mds.ReadK051649Status(chipID);
        }

        public static K053260.k053260_state GetK053260Register(int chipID)
        {
            return mds.getK053260State(chipID);
        }

        public static MIDIParam GetMIDIInfos(int chipID)
        {
            return chipRegister.midiParams[chipID];
        }

        public static scd_pcm.pcm_chip_ GetRf5c164Register(int chipID)
        {
            return mds.ReadRf5c164Register(chipID);
        }

        public static MDSound.rf5c68.rf5c68_state GetRf5c68Register(int chipID)
        {
            return mds.ReadRf5c68Register(chipID);
        }

        public static ymf271.YMF271Chip GetYMF271Register(int chipID)
        {
            return mds.ReadYMF271Register(chipID);
        }


        public static byte[] GetC140Register(int chipID)
        {
            return chipRegister.pcmRegisterC140[chipID];
        }

        public static PPZ8.PPZChannelWork[] GetPPZ8Register(int chipID)
        {
            return chipRegister.GetPPZ8Register(chipID);
        }

        public static bool[] GetC140KeyOn(int chipID)
        {
            return chipRegister.pcmKeyOnC140[chipID];
        }

        public static int[] GetYMZ280BRegister(int chipID)
        {
            return chipRegister.YMZ280BRegister[chipID];
        }

        public static ushort[] GetC352Register(int chipID)
        {
            return chipRegister.pcmRegisterC352[chipID];
        }

        public static multipcm._MultiPCM GetMultiPCMRegister(int chipID)
        {
            return chipRegister.getMultiPCMRegister(chipID);
        }

        public static ushort[] GetC352KeyOn(int chipID)
        {
            return chipRegister.readC352((byte)chipID);
        }

        public static ushort[] GetQSoundRegister(int chipID)
        {
            return chipRegister.getQSoundRegister(chipID);
        }

        public static byte[] GetSEGAPCMRegister(int chipID)
        {
            return chipRegister.pcmRegisterSEGAPCM[chipID];
        }

        public static bool[] GetSEGAPCMKeyOn(int chipID)
        {
            return chipRegister.pcmKeyOnSEGAPCM[chipID];
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
            reg ??= chipRegister.getNESRegisterAPU(chipID, EnmModel.VirtualModel);

            return reg;
        }

        public static byte[] GetDMCRegister(int chipID)
        {
            byte[] reg = null;
            try
            {
                //nsf向け
                if (chipRegister == null) reg = null;
                else if (chipRegister.nes_apu == null) reg = null;
                else if (chipRegister.nes_apu.chip == null) reg = null;
                else if (chipID == 1) reg = null;
                else reg = chipRegister.nes_dmc.chip.reg;

                //vgm向け
                reg ??= chipRegister.getNESRegisterDMC(chipID, EnmModel.VirtualModel);

                return reg;
            }
            catch
            {
                return null;
            }
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
            reg ??= chipRegister.getFDSRegister(chipID, EnmModel.VirtualModel);

            return reg;
        }

        private static byte[] s5bregs = new byte[0x20];
        public static byte[] GetS5BRegister(int chipID)
        {
            //nsf向け
            if (chipRegister == null) return null;
            else if (chipRegister.nes_fme7 == null) return null;
            else if (chipID == 1) return null;

            uint dat = 0;
            for (uint adr = 0x00; adr < 0x20; adr++)
            {
                dat = 0;
                chipRegister.nes_fme7.Read(adr, ref dat);
                s5bregs[adr] = (byte)dat;
            }

            return s5bregs;
        }

        public static gb.gb_sound_t GetDMGRegister(int chipID)
        {
            if (mds == null) return null;
            else if (chipID == 1) return null;

            return mds.ReadDMG((byte)chipID);
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
                dat = 0;
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

        public static int[] GetYM2609KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2609[chipID];
        }

        public static int[] GetYM2610KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2610[chipID];
        }

        public static int[] GetYM2203KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2203[chipID];
        }

        public static ChipKeyInfo GetYM2413KeyInfo(int chipID)
        {
            return chipRegister.getYM2413KeyInfo(chipID);
        }

        public static ChipKeyInfo GetYM3526KeyInfo(int chipID)
        {
            return chipRegister.getYM3526KeyInfo(chipID);
        }

        public static ChipKeyInfo GetY8950KeyInfo(int chipID)
        {
            return chipRegister.getY8950KeyInfo(chipID);
        }

        public static ChipKeyInfo GetYM3812KeyInfo(int chipID)
        {
            return chipRegister.getYM3812KeyInfo(chipID);
        }

        public static ChipKeyInfo GetVRC7KeyInfo(int chipID)
        {
            return chipRegister.getVRC7KeyInfo(chipID);
        }

        public static int GetYMF262FMKeyON(int chipID)
        {
            return chipRegister.getYMF262FMKeyON(chipID);
        }

        public static int GetYMF262RyhthmKeyON(int chipID)
        {
            return chipRegister.getYMF262RyhthmKeyON(chipID);
        }

        public static int GetYMF278BFMKeyON(int chipID)
        {
            return chipRegister.getYMF278BFMKeyON(chipID);
        }

        public static void ResetYMF278BFMKeyON(int chipID)
        {
            chipRegister.resetYMF278BFMKeyON(chipID);
        }

        public static int GetYMF278BRyhthmKeyON(int chipID)
        {
            return chipRegister.getYMF278BRyhthmKeyON(chipID);
        }

        public static void ResetYMF278BRyhthmKeyON(int chipID)
        {
            chipRegister.resetYMF278BRyhthmKeyON(chipID);
        }

        public static int[] GetYMF278BPCMKeyON(int chipID)
        {
            return chipRegister.getYMF278BPCMKeyON(chipID);
        }

        public static void ResetYMF278BPCMKeyON(int chipID)
        {
            chipRegister.resetYMF278BPCMKeyON(chipID);
        }


        public static void SetMasterVolume(bool isAbs, int volume)
        {
            MasterVolume
                = setting.balance.MasterVolume
                = Common.Range((isAbs ? 0 : setting.balance.MasterVolume) + volume, -192, 20);
        }

        public static void SetAY8910Volume(bool isAbs, int volume)
        {
            try
            {
                mds.setVolumeAY8910(setting.balance.AY8910Volume
                    = Common.Range((isAbs ? 0 : setting.balance.AY8910Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2151Volume(bool isAbs, int volume)
        {
            try
            {
                int vol
                    = setting.balance.YM2151Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2151Volume) + volume, -192, 20);

                mds.SetVolumeYM2151(vol);
                mds.SetVolumeYM2151mame(vol);
                mds.SetVolumeYM2151x68sound(vol);
            }
            catch { }
        }

        public static void SetYM2203Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2203(setting.balance.YM2203Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2203Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2203FMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2203FM(setting.balance.YM2203FMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2203FMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2203PSGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2203PSG(setting.balance.YM2203PSGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2203PSGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2413Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2413(setting.balance.YM2413Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2413Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetK053260Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeK053260(setting.balance.K053260Volume
                    = Common.Range((isAbs ? 0 : setting.balance.K053260Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetRF5C68Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeRF5C68(setting.balance.RF5C68Volume
                    = Common.Range((isAbs ? 0 : setting.balance.RF5C68Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM3812Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM3812(setting.balance.YM3812Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM3812Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetY8950Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeY8950(setting.balance.Y8950Volume
                    = Common.Range((isAbs ? 0 : setting.balance.Y8950Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM3526Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM3526(setting.balance.YM3526Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM3526Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608(setting.balance.YM2608Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608FMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608FM(setting.balance.YM2608FMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608FMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608PSGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608PSG(setting.balance.YM2608PSGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608PSGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608RhythmVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608Rhythm(setting.balance.YM2608RhythmVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608RhythmVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608AdpcmVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608Adpcm(setting.balance.YM2608AdpcmVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608AdpcmVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610(setting.balance.YM2610Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610FMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610FM(setting.balance.YM2610FMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610FMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610PSGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610PSG(setting.balance.YM2610PSGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610PSGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610AdpcmAVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610AdpcmA(setting.balance.YM2610AdpcmAVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610AdpcmAVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610AdpcmBVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610AdpcmB(setting.balance.YM2610AdpcmBVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610AdpcmBVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2612Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2612(setting.balance.YM2612Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2612Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetSN76489Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeSN76489(setting.balance.SN76489Volume
                    = Common.Range((isAbs ? 0 : setting.balance.SN76489Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetHuC6280Volume(bool isAbs, int volume)
        {
            try
            {
                mds.setVolumeHuC6280(setting.balance.HuC6280Volume
                    = Common.Range((isAbs ? 0 : setting.balance.HuC6280Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetRF5C164Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeRF5C164(setting.balance.RF5C164Volume
                    = Common.Range((isAbs ? 0 : setting.balance.RF5C164Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetPWMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumePWM(setting.balance.PWMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.PWMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetOKIM6258Volume(bool isAbs, int volume)
        {
            try
            {
                int vol = setting.balance.OKIM6258Volume
                    = Common.Range((isAbs ? 0 : setting.balance.OKIM6258Volume) + volume, -192, 20);

                mds.SetVolumeOKIM6258(vol);
                mds.SetVolumeMpcmX68k(vol);
            }
            catch { }
        }

        public static void SetOKIM6295Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeOKIM6295(setting.balance.OKIM6295Volume
                    = Common.Range((isAbs ? 0 : setting.balance.OKIM6295Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetC140Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeC140(setting.balance.C140Volume
                    = Common.Range((isAbs ? 0 : setting.balance.C140Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetSegaPCMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeSegaPCM(setting.balance.SEGAPCMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.SEGAPCMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetC352Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeC352(setting.balance.C352Volume
                    = Common.Range((isAbs ? 0 : setting.balance.C352Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetSA1099Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeSAA1099(setting.balance.SAA1099Volume
                    = Common.Range((isAbs ? 0 : setting.balance.SAA1099Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetPPZ8Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumePPZ8(setting.balance.PPZ8Volume
                    = Common.Range((isAbs ? 0 : setting.balance.PPZ8Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetK051649Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeK051649(setting.balance.K051649Volume
                    = Common.Range((isAbs ? 0 : setting.balance.K051649Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetK054539Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeK054539(setting.balance.K054539Volume
                    = Common.Range((isAbs ? 0 : setting.balance.K054539Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetQSoundVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeQSoundCtr(setting.balance.QSoundVolume
                    = Common.Range((isAbs ? 0 : setting.balance.QSoundVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetDMGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeDMG(setting.balance.DMGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.DMGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetGA20Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeGA20(setting.balance.GA20Volume
                    = Common.Range((isAbs ? 0 : setting.balance.GA20Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMZ280BVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMZ280B(setting.balance.YMZ280BVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YMZ280BVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMF271Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMF271(setting.balance.YMF271Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YMF271Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMF262Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMF262(setting.balance.YMF262Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YMF262Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMF278BVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMF278B(setting.balance.YMF278BVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YMF278BVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetMultiPCMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeMultiPCM(setting.balance.MultiPCMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.MultiPCMVolume) + volume, -192, 20));
            }
            catch { }
        }



        public static void SetAPUVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeNES(
                    setting.balance.APUVolume
                    = Common.Range((isAbs ? 0 : setting.balance.APUVolume) + volume, -192, 20)
                    );
            }
            catch { }
        }

        public static void SetDMCVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeDMC(setting.balance.DMCVolume
                    = Common.Range((isAbs ? 0 : setting.balance.DMCVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetFDSVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeFDS(setting.balance.FDSVolume
                    = Common.Range((isAbs ? 0 : setting.balance.FDSVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetMMC5Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeMMC5(setting.balance.MMC5Volume
                    = Common.Range((isAbs ? 0 : setting.balance.MMC5Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetN160Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeN160(setting.balance.N160Volume
                    = Common.Range((isAbs ? 0 : setting.balance.N160Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetVRC6Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeVRC6(setting.balance.VRC6Volume
                    = Common.Range((isAbs ? 0 : setting.balance.VRC6Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetVRC7Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeVRC7(setting.balance.VRC7Volume
                    = Common.Range((isAbs ? 0 : setting.balance.VRC7Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetFME7Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeFME7(setting.balance.FME7Volume
                    = Common.Range((isAbs ? 0 : setting.balance.FME7Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetGimicOPNVolume(bool isAbs, int volume)
        {
            setting.balance.GimicOPNVolume = Common.Range((isAbs ? 0 : setting.balance.GimicOPNVolume) + volume, 0, 127);
        }

        public static void SetGimicOPNAVolume(bool isAbs, int volume)
        {
            setting.balance.GimicOPNAVolume = Common.Range((isAbs ? 0 : setting.balance.GimicOPNAVolume) + volume, 0, 127);
        }


        public static int[] GetFMVolume(int chipID)
        {
            return chipRegister.GetYM2612Volume(chipID);
        }

        public static int[] GetYM2151Volume(int chipID)
        {
            return chipRegister.GetYM2151Volume(chipID);
        }

        public static int[] GetYM2608Volume(int chipID)
        {
            return chipRegister.GetYM2608Volume(chipID);
        }

        public static int[][] GetYM2608RhythmVolume(int chipID)
        {
            return chipRegister.GetYM2608RhythmVolume(chipID);
        }

        public static int[] GetYM2609Volume(int chipID)
        {
            return chipRegister.GetYM2609Volume(chipID);
        }

        public static int[][] GetYM2609RhythmVolume(int chipID)
        {
            return chipRegister.GetYM2609RhythmVolume(chipID);
        }
        public static int[] GetYM2609AdpcmAPan(int chipID)
        {
            return chipRegister.GetYM2609AdpcmAPan(chipID);
        }
        public static int[] GetYM2609AdpcmAVol(int chipID)
        {
            return chipRegister.GetYM2609AdpcmAVol(chipID);
        }

        public static int[] GetYM2608AdpcmVolume(int chipID)
        {
            return chipRegister.GetYM2608AdpcmVolume(chipID);
        }

        public static int[][] GetYM2609AdpcmVolume(int chipID)
        {
            return chipRegister.GetYM2609AdpcmVolume(chipID);
        }
        public static int[][] GetYM2609AdpcmPan(int chipID)
        {
            return chipRegister.GetYM2609AdpcmPan(chipID);
        }

        public static int[] GetYM2610Volume(int chipID)
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



        public static int[] GetYM2609Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2609Ch3SlotVolume(chipID);
        }

        public static byte[] GetYM2609UserWave(int chipID, int p, int n)
        {
            return chipRegister.readYM2609GetUserWave(chipID, p, n, EnmModel.VirtualModel);
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

        public static void SetRF5C164Mask(int chipID, int ch)
        {
            //mds.setRf5c164Mask(chipID, ch);
            chipRegister.setMaskRF5C164(chipID, ch, true);
        }

        public static void SetRF5C68Mask(int chipID, int ch)
        {
            //mds.setRf5c68Mask(chipID, ch);
            chipRegister.setMaskRF5C68(chipID, ch, true);
        }

        public static void SetSN76489Mask(int chipID, int ch)
        {
            //mds.setSN76489Mask(chipID,1 << ch);
            chipRegister.setMaskSN76489(chipID, ch, true);
            SN76489ForcedSendVolume(chipID, ch);
        }

        public static void ResetSN76489Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetSN76489Mask(chipID, 1 << ch);
                chipRegister.setMaskSN76489(chipID, ch, false);
                SN76489ForcedSendVolume(chipID, ch);
            }
            catch { }
        }

        private static void SN76489ForcedSendVolume(int chipID, int ch)
        {
            Setting.ChipType2 ct = setting.SN76489Type[chipID];
            chipRegister.setSN76489Register(chipID
                , (byte)(0x90 
                    | ((ch & 3) << 5) 
                    | (15 - (Math.Max(chipRegister.sn76489Vol[chipID][ch][0], chipRegister.sn76489Vol[chipID][ch][1]) & 0xf)))
                , ct.UseEmu[0] ? EnmModel.VirtualModel : EnmModel.RealModel);
        }

        public static void SetYM2151Mask(int chipID, int ch)
        {
            //mds.setYM2151Mask(ch);
            chipRegister.setMaskYM2151(chipID, ch, true);
        }

        public static void SetYM2203Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM2203(chipID, ch, true);
        }

        public static void SetYM2413Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM2413(chipID, ch, true);
        }

        public static void SetYM2608Mask(int chipID, int ch)
        {
            //mds.setYM2608Mask(ch);
            chipRegister.setMaskYM2608(chipID, ch, true);
        }

        public static void SetYM2610Mask(int chipID, int ch)
        {
            //mds.setYM2610Mask(ch);
            chipRegister.setMaskYM2610(chipID, ch, true);
        }

        public static void SetYM2612Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM2612(chipID, ch, true);
        }

        public static void SetYM3526Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM3526(chipID, ch, true);
        }

        public static void SetY8950Mask(int chipID, int ch)
        {
            chipRegister.setMaskY8950(chipID, ch, true);
        }

        public static void SetYM3812Mask(int chipID, int ch)
        {
            chipRegister.setMaskYM3812(chipID, ch, true);
        }

        public static void SetYMF262Mask(int chipID, int ch)
        {
            chipRegister.setMaskYMF262(chipID, ch, true);
        }

        public static void SetYMF278BMask(int chipID, int ch)
        {
            chipRegister.setMaskYMF278B(chipID, ch, true);
        }

        public static void SetC140Mask(int chipID, int ch)
        {
            //mds.setC140Mask(chipID, 1 << ch);
            chipRegister.setMaskC140(chipID, ch, true);
        }

        public static void SetPPZ8Mask(int chipID, int ch)
        {
            //mds.setPPZ8Mask(chipID, 1 << ch);
            chipRegister.setMaskPPZ8(chipID, ch, true);
        }

        public static void SetC352Mask(int chipID, int ch)
        {
            chipRegister.setMaskC352(chipID, ch, true);
        }

        public static void SetSegaPCMMask(int chipID, int ch)
        {
            //mds.setSegaPcmMask(chipID, 1 << ch);
            chipRegister.setMaskSegaPCM(chipID,ch, true);
        }

        public static void SetQSoundMask(int chipID, int ch)
        {
            chipRegister.setMaskQSound(chipID, ch, true);
        }

        public static void SetAY8910Mask(int chipID, int ch)
        {
            //mds.setAY8910Mask(chipID, 1 << ch);
            chipRegister.setMaskAY8910(chipID, ch, true);
        }

        public static void SetHuC6280Mask(int chipID, int ch)
        {
            //mds.setHuC6280Mask(chipID, 1 << ch);
            chipRegister.setMaskHuC6280(chipID,ch, true);
        }

        public static void SetOKIM6258Mask(int chipID)
        {
            chipRegister.setMaskOKIM6258(chipID, true);
        }

        public static void SetOKIM6295Mask(int chipID, int ch)
        {
            chipRegister.setMaskOKIM6295(chipID, ch, true);
        }

        public static void ResetOKIM6295Mask(int chipID, int ch)
        {
            chipRegister.setMaskOKIM6295(chipID, ch, false);
        }

        public static void SetNESMask(int chipID, int ch)
        {
            chipRegister.setNESMask(chipID, ch);
        }

        public static void SetDMCMask(int chipID, int ch)
        {
            chipRegister.setNESMask(chipID, ch + 2);
        }

        public static void SetFDSMask(int chipID)
        {
            chipRegister.setFDSMask(chipID);
        }

        public static void SetMMC5Mask(int chipID, int ch)
        {
            chipRegister.setMMC5Mask(chipID, ch);
        }

        public static void SetVRC7Mask(int chipID, int ch)
        {
            chipRegister.setVRC7Mask(chipID, ch);
        }

        public static void SetK051649Mask(int chipID, int ch)
        {
            chipRegister.setK051649Mask(chipID, ch);
        }

        public static void SetK053260Mask(int chipID, int ch)
        {
            chipRegister.setK053260Mask(chipID, ch,true);
        }

        public static void SetDMGMask(int chipID, int ch)
        {
            chipRegister.setDMGMask(chipID, ch);
        }

        public static void SetVRC6Mask(int chipID, int ch)
        {
            chipRegister.setVRC6Mask(chipID, ch);
        }

        public static void SetN163Mask(int chipID, int ch)
        {
            chipRegister.setN163Mask(chipID, ch);
        }



        public static void ResetOKIM6258Mask(int chipID)
        {
            chipRegister.setMaskOKIM6258(chipID, false);
        }

        public static void ResetYM2612Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2612Mask(chipID, 1 << ch);
                chipRegister.setMaskYM2612(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetYM2203Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM2203(chipID, ch, false, Stopped);
            }
            catch { }
        }

        public static void ResetYM2413Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM2413(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetRF5C164Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetRf5c164Mask(chipID, ch);
                chipRegister.setMaskRF5C164(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetRF5C68Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetRf5c68Mask(chipID, ch);
                chipRegister.setMaskRF5C68(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetYM2151Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2151Mask(ch);
                chipRegister.setMaskYM2151(chipID, ch, false, Stopped);
            }
            catch { }
        }

        public static void ResetYM2608Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2608Mask(ch);
                chipRegister.setMaskYM2608(chipID, ch, false, Stopped);
            }
            catch { }
        }

        public static void ResetYM2610Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM2610(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetYM3526Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM3526(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetY8950Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskY8950(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetYM3812Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYM3812(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetYMF262Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYMF262(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetYMF278BMask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskYMF278B(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetC140Mask(int chipID, int ch)
        {
            //mds.resetC140Mask(chipID, 1 << ch);
            try
            {
                chipRegister.setMaskC140(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetPPZ8Mask(int chipID, int ch)
        {
            //mds.resetPPZ8Mask(chipID, 1 << ch);
            try
            {
                chipRegister.setMaskPPZ8(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetC352Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskC352(chipID, ch, false);
            }
            catch { }
        }

        public static void ResetSegaPCMMask(int chipID, int ch)
        {
            //mds.resetSegaPcmMask(chipID, 1 << ch);
            chipRegister.setMaskSegaPCM(chipID,ch, false);
        }

        public static void ResetQSoundMask(int chipID, int ch)
        {
            chipRegister.setMaskQSound(chipID, ch, false);
        }

        public static void ResetAY8910Mask(int chipID, int ch)
        {
            //mds.resetAY8910Mask(chipID, 1 << ch);
            chipRegister.setMaskAY8910(chipID, ch, false);
        }

        public static void ResetHuC6280Mask(int chipID, int ch)
        {
            //mds.resetHuC6280Mask(chipID, 1 << ch);
            chipRegister.setMaskHuC6280(chipID,ch, false);
        }

        public static void ResetNESMask(int chipID, int ch)
        {
            chipRegister.resetNESMask(chipID, ch);
        }

        public static void ResetDMCMask(int chipID, int ch)
        {
            chipRegister.resetNESMask(chipID, ch + 2);
        }

        public static void ResetFDSMask(int chipID)
        {
            chipRegister.resetFDSMask(chipID);
        }

        public static void ResetMMC5Mask(int chipID, int ch)
        {
            chipRegister.resetMMC5Mask(chipID, ch);
        }

        public static void ResetVRC7Mask(int chipID, int ch)
        {
            chipRegister.resetVRC7Mask(chipID, ch);
        }

        public static void ResetK051649Mask(int chipID, int ch)
        {
            chipRegister.resetK051649Mask(chipID, ch);
        }

        public static void ResetK053260Mask(int chipID, int ch)
        {
            chipRegister.setK053260Mask(chipID, ch, false);
        }

        public static void ResetDMGMask(int chipID, int ch)
        {
            chipRegister.resetDMGMask(chipID, ch);
        }

        public static void ResetVRC6Mask(int chipID, int ch)
        {
            chipRegister.resetVRC6Mask(chipID, ch);
        }

        public static void ResetN163Mask(int chipID, int ch)
        {
            chipRegister.resetN163Mask(chipID, ch);
        }
    }


}
