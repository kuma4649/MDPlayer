#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using NAudio.Midi;
using System.Text;
//using MDPlayer.Properties;
using MDSound.np.chip;
using MDPlayerx64.form.SYS;
using MDPlayerx64;
using System;
using MDPlayerx64.Driver;

namespace MDPlayer.form
{
    public partial class frmMain : Form
    {

        private PictureBox pbRf5c164Screen;
        private DoubleBuffer screen;
        private int pWidth = 0;
        private int pHeight = 0;

        private frmInfo frmInfo = null;
        private frmPlayList frmPlayList = null;
        private frmVSTeffectList frmVSTeffectList = null;

        private frmMegaCD[] frmMCD = new frmMegaCD[2] { null, null };
        private frmRf5c68[] frmRf5c68 = new frmRf5c68[2] { null, null };
        private frmC140[] frmC140 = new frmC140[2] { null, null };
        private frmPPZ8[] frmPPZ8 = new frmPPZ8[2] { null, null };
        private frmS5B[] frmS5B = new frmS5B[2] { null, null };
        private frmDMG[] frmDMG = new frmDMG[2] { null, null };
        private frmYMZ280B[] frmYMZ280B = new frmYMZ280B[2] { null, null };
        private frmC352[] frmC352 = new frmC352[2] { null, null };
        private frmMultiPCM[] frmMultiPCM = new frmMultiPCM[2] { null, null };
        private frmQSound[] frmQSound = new frmQSound[2] { null, null };
        private frmYM2608[] frmYM2608 = new frmYM2608[2] { null, null };
        private frmYM2151[] frmYM2151 = new frmYM2151[2] { null, null };
        private frmYM2203[] frmYM2203 = new frmYM2203[2] { null, null };
        private frmYM2609[] frmYM2609 = new frmYM2609[2] { null, null };
        private frmYM2610[] frmYM2610 = new frmYM2610[2] { null, null };
        private frmYM2612[] frmYM2612 = new frmYM2612[2] { null, null };
        private frmYM3526[] frmYM3526 = new frmYM3526[2] { null, null };
        private frmY8950[] frmY8950 = new frmY8950[2] { null, null };
        private frmYM3812[] frmYM3812 = new frmYM3812[2] { null, null };
        private frmOKIM6258[] frmOKIM6258 = new frmOKIM6258[2] { null, null };
        private frmOKIM6295[] frmOKIM6295 = new frmOKIM6295[2] { null, null };
        private frmPCM8[] frmPCM8 = new frmPCM8[2] { null, null };
        private frmSN76489[] frmSN76489 = new frmSN76489[2] { null, null };
        private frmSegaPCM[] frmSegaPCM = new frmSegaPCM[2] { null, null };
        private frmAY8910[] frmAY8910 = new frmAY8910[2] { null, null };
        private frmHuC6280[] frmHuC6280 = new frmHuC6280[2] { null, null };
        private frmK051649[] frmK051649 = new frmK051649[2] { null, null };
        private frmK053260[] frmK053260 = new frmK053260[2] { null, null };
        private frmYM2413[] frmYM2413 = new frmYM2413[2] { null, null };
        private frmYMF262[] frmYMF262 = new frmYMF262[2] { null, null };
        private frmYMF271[] frmYMF271 = new frmYMF271[2] { null, null };
        private frmYMF278B[] frmYMF278B = new frmYMF278B[2] { null, null };
        private frmMIDI[] frmMIDI = new frmMIDI[2] { null, null };
        private frmYM2612MIDI frmYM2612MIDI = null;
        private frmMixer2 frmMixer2 = null;
        private frmNESDMC[] frmNESDMC = new frmNESDMC[2] { null, null };
        private frmFDS[] frmFDS = new frmFDS[2] { null, null };
        private frmMMC5[] frmMMC5 = new frmMMC5[2] { null, null };
        private frmVRC6[] frmVRC6 = new frmVRC6[2] { null, null };
        private frmVRC7[] frmVRC7 = new frmVRC7[2] { null, null };
        private frmN106[] frmN106 = new frmN106[2] { null, null };
        private frmRegTest frmRegTest;
        private frmVisWave frmVisWave;
        private frmConsole frmConsole;

        private List<Form[]> lstForm = new();

        public MDChipParams oldParam = new();
        private MDChipParams newParam = new();

        private int[] oldButton = new int[18];
        private int[] newButton = new int[18];
        private int[] oldButtonMode = new int[18];
        private int[] newButtonMode = new int[18];

        private bool isRunning = false;
        private bool stopped = false;

        private bool IsInitialOpenFolder = true;

        private byte[] srcBuf;

        public Setting setting = Setting.Load();
        public TonePallet tonePallet = TonePallet.Load(null);

        private int frameSizeW = 0;
        private int frameSizeH = 0;


        private MidiIn midiin = null;
        private bool forcedExit = false;
        private YM2612MIDI YM2612MIDI = null;
        private bool flgReinit = false;
        public bool reqAllScreenInit = true;

        private string[] modeTip = new string[]{
            "Mode\r\nNow:Step\r\nNext:Random",
            "Mode\r\nNow:Random\r\nNext:Loop",
            "Mode\r\nNow:Loop\r\nNext:LoopOne",
            "Mode\r\nNow:LoopOne\r\nNext:Step",
        };

        private string[] zoomTip = new string[]{
            "Zoom\r\nNow:x1\r\nNext:x2",
            "Zoom\r\nNow:x2\r\nNext:x3",
            "Zoom\r\nNow:x3\r\nNext:x4",
            "Zoom\r\nNow:x4\r\nNext:x1",
        };

        //private FileSystemWatcher watcher = null;
        private KumaCom mmf = null;
        private long now = 0;
        private string opeFolder = "";
        private object remoteLockObj = new();
        private bool remoteBusy = false;
        private List<string[]> remoteReq = new();

        public frmMain()
        {
            log.debug = this.setting.debug.logDebug;
            log.logLevel = this.setting.debug.logLevel;

            frmConsole = new frmConsole(setting);
            if (setting.debug.ShowConsole) frmConsole.Show();

            log.ForcedWrite("起動処理開始");
            log.ForcedWrite("frmMain(コンストラクタ):STEP 00");

            string resPath = setting.other.ResourceFile;
            if (string.IsNullOrEmpty(resPath))
                resPath = Application.StartupPath;
            ResMng.Init(resPath);
            setupControlImages();

            InitializeComponent();
            DrawBuff.Init();

            lstForm.Add(frmMCD);
            lstForm.Add(frmC140);
            lstForm.Add(frmPPZ8);
            lstForm.Add(frmC352);
            lstForm.Add(frmY8950);
            lstForm.Add(frmYM2608);
            lstForm.Add(frmYM2151);
            lstForm.Add(frmYM2203);
            lstForm.Add(frmYM2413);
            lstForm.Add(frmYM2609);
            lstForm.Add(frmYM2610);
            lstForm.Add(frmYM2612);
            lstForm.Add(frmYM3526);
            lstForm.Add(frmYM3812);
            lstForm.Add(frmYMF262);
            lstForm.Add(frmYMF278B);
            lstForm.Add(frmOKIM6258);
            lstForm.Add(frmPCM8);
            lstForm.Add(frmSN76489);
            lstForm.Add(frmSegaPCM);
            lstForm.Add(frmAY8910);
            lstForm.Add(frmHuC6280);
            lstForm.Add(frmK051649);
            lstForm.Add(frmK053260);
            lstForm.Add(frmMIDI);
            lstForm.Add(frmNESDMC);
            lstForm.Add(frmFDS);
            lstForm.Add(frmMMC5);
            lstForm.Add(frmVRC6);
            lstForm.Add(frmVRC7);

            log.ForcedWrite("frmMain(コンストラクタ):STEP 01");

            //引数が指定されている場合のみプロセスチェックを行い、自分と同じアプリケーションが実行中ならばそちらに引数を渡し終了する
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                Process prc = GetPreviousProcess();
                if (prc != null)
                {
                    string[] args = Environment.GetCommandLineArgs();
                    string arg = "";
                    for (int i = 1; i < args.Length; i++) arg += string.Format("\"{0}\"", args[i]);
                    SendString(prc.MainWindowHandle, arg);
                    forcedExit = true;
                    try
                    {
                        this.Close();
                    }
                    catch { }
                    return;
                }
            }

            log.ForcedWrite("frmMain(コンストラクタ):STEP 02");

            pbScreen.AllowDrop = true;

            log.ForcedWrite("frmMain(コンストラクタ):STEP 03");
            if (setting == null)
            {
                log.ForcedWrite("frmMain(コンストラクタ):setting is null");
            }
            else
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    DialogResult res = MessageBox.Show(
                        "ウィンドウの位置情報を初期化しますか？",
                        "MDPlayer",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        ClearWindowPos();
                    }
                }
            }

            log.ForcedWrite("コントロール初期化処理開始");

            tsmiOutputwavFile.Checked = setting.other.WavSwitch;
            CheckTsmiChangeZoomItem();

            log.ForcedWrite("起動時のAudio初期化処理開始");

            Audio.FrmMain = this;
            Audio.Init(setting);

            YM2612MIDI = new YM2612MIDI(this, Audio.MdsMIDI, newParam);

            log.ForcedWrite("起動時のAudio初期化処理完了");

            StartMIDIInMonitoring();

            log.ForcedWrite("frmMain(コンストラクタ):STEP 04");


        }

        private void ClearWindowPos()
        {
            setting.location = new Setting.Location();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Microsoft.Win32.SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            log.ForcedWrite("frmMain_Load:STEP 05");

            if (setting.location.Main.Point != System.Drawing.Point.Empty)
                this.Location = setting.location.Main.Point;
            //this.WindowState = setting.location.Main.State;
            //if(setting.location.Main.Size!= System.Drawing.Size.Empty)
            //    this.Size=new Size(setting.location.Main.Size.Width,setting.location.Main.Size.Height);

            // DoubleBufferオブジェクトの作成

            pbRf5c164Screen = new PictureBox
            {
                Width = 320,
                Height = 72
            };

            log.ForcedWrite("frmMain_Load:STEP 06");

            screen = new DoubleBuffer(pbScreen, ResMng.ImgDic["planeControl"], 1)
            {
                setting = setting
            };
            //oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            reqAllScreenInit = true;

            log.ForcedWrite("frmMain_Load:STEP 07");

            pWidth = pbScreen.Width;
            pHeight = pbScreen.Height;

            frmPlayList = new frmPlayList(this);
            frmPlayList.Show();
            frmPlayList.Visible = false;
            frmPlayList.Opacity = 1.0;
            //frmPlayList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmPlayList.Refresh();

            frmVSTeffectList = new frmVSTeffectList(this, setting);
            frmVSTeffectList.Show();
            frmVSTeffectList.Visible = false;
            frmVSTeffectList.Opacity = 1.0;
            //frmVSTeffectList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmVSTeffectList.Refresh();

            if (setting.location.OPlayList) DispPlayList();
            if (setting.location.OInfo) openInfo();
            if (setting.location.OMixer) openMixer();
            if (setting.location.OpenYm2612MIDI) openMIDIKeyboard();
            if (setting.location.OpenVisWave) OpenFormVisWave();

            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (setting.location.OpenAY8910[chipID]) OpenFormAY8910(chipID);
                if (setting.location.OpenC140[chipID]) OpenFormC140(chipID);
                if (setting.location.OpenPPZ8[chipID]) OpenFormPPZ8(chipID);
                if (setting.location.OpenS5B[chipID]) OpenFormS5B(chipID);
                if (setting.location.OpenDMG[chipID]) OpenFormDMG(chipID);
                if (setting.location.OpenYMZ280B[chipID]) OpenFormYMZ280B(chipID);
                if (setting.location.OpenK053260[chipID]) OpenFormK053260(chipID);
                if (setting.location.OpenC352[chipID]) OpenFormC352(chipID);
                if (setting.location.OpenMultiPCM[chipID]) OpenFormMultiPCM(chipID);
                if (setting.location.OpenQSound[chipID]) OpenFormQSound(chipID);
                if (setting.location.OpenHuC6280[chipID]) OpenFormHuC6280(chipID);
                if (setting.location.OpenK051649[chipID]) OpenFormK051649(chipID);
                if (setting.location.OpenMIDI[chipID]) OpenFormMIDI(chipID);
                if (setting.location.OpenNESDMC[chipID]) OpenFormNESDMC(chipID);
                if (setting.location.OpenFDS[chipID]) OpenFormFDS(chipID);
                if (setting.location.OpenMMC5[chipID]) OpenFormMMC5(chipID);
                if (setting.location.OpenOKIM6258[chipID]) OpenFormOKIM6258(chipID);
                if (setting.location.OpenOKIM6295[chipID]) OpenFormOKIM6295(chipID);
                if (setting.location.OpenPCM8[chipID]) OpenFormPCM8(chipID);
                if (setting.location.OpenRf5c164[chipID]) OpenFormMegaCD(chipID);
                if (setting.location.OpenRf5c68[chipID]) OpenFormRf5c68(chipID);
                if (setting.location.OpenSN76489[chipID]) OpenFormSN76489(chipID);
                if (setting.location.OpenSegaPCM[chipID]) OpenFormSegaPCM(chipID);
                if (setting.location.OpenYm2151[chipID]) OpenFormYM2151(chipID);
                if (setting.location.OpenYm2203[chipID]) OpenFormYM2203(chipID);
                if (setting.location.OpenYm2413[chipID]) OpenFormYM2413(chipID);
                if (setting.location.OpenYm2608[chipID]) OpenFormYM2608(chipID);
                if (setting.location.OpenYm2609[chipID]) OpenFormYM2609(chipID);
                if (setting.location.OpenYm2610[chipID]) OpenFormYM2610(chipID);
                if (setting.location.OpenYm2612[chipID]) OpenFormYM2612(chipID);
                if (setting.location.OpenYm3526[chipID]) OpenFormYM3526(chipID);
                if (setting.location.OpenY8950[chipID]) OpenFormY8950(chipID);
                if (setting.location.OpenYm3812[chipID]) OpenFormYM3812(chipID);
                if (setting.location.OpenYmf262[chipID]) OpenFormYMF262(chipID);
                if (setting.location.OpenYMF271[chipID]) OpenFormYMF271(chipID);
                if (setting.location.OpenYmf278b[chipID]) OpenFormYMF278B(chipID);
                if (setting.location.OpenVrc6[chipID]) OpenFormVRC6(chipID);
                if (setting.location.OpenVrc7[chipID]) OpenFormVRC7(chipID);
                if (setting.location.OpenRegTest[chipID]) OpenFormRegTest(chipID);
                if (setting.location.OpenN106[chipID]) OpenFormN106(chipID);
            }

            log.ForcedWrite("frmMain_Load:STEP 08");

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
            toolTip1.SetToolTip(opeButtonMode, modeTip[newButtonMode[9]]);
            lstOpeButtonControl = new Button[]
            {
            opeButtonSetting,
            opeButtonStop,
            opeButtonPause,
            opeButtonFadeout,
            opeButtonPrevious,
            opeButtonSlow,
            opeButtonPlay,
            opeButtonFast,
            opeButtonNext,
            opeButtonMode,
            opeButtonOpen,
            opeButtonPlayList,
            opeButtonInformation,
            opeButtonMixer,
            opeButtonKBD,
            opeButtonVST,
            opeButtonMIDIKBD,
            opeButtonZoom,
            opeButtonMode,
            opeButtonMode,
            opeButtonMode
            };

            log.ForcedWrite("frmMain_Load:STEP 09");

            //operationフォルダクリア
            log.ForcedWrite("frmMain_Load:Clear Operation folder.");
            mmf = new fileCom(false, "MDPlayer", "MDPlayer", 1024 * 4);

        }

        //private void startWatch(string opeFolder)
        //{
        //    if (watcher != null) return;

        //    watcher = new System.IO.FileSystemWatcher();
        //    watcher.Path = Path.GetDirectoryName(opeFolder);
        //    watcher.NotifyFilter =
        //        (
        //        System.IO.NotifyFilters.LastAccess
        //        | System.IO.NotifyFilters.LastWrite
        //        | System.IO.NotifyFilters.FileName
        //        | System.IO.NotifyFilters.DirectoryName
        //        | System.IO.NotifyFilters.CreationTime
        //        | System.IO.NotifyFilters.Attributes
        //        );
        //    watcher.Filter = "";// Path.GetFileName(opeFolder);
        //    watcher.SynchronizingObject = this;

        //    watcher.Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
        //    watcher.Created += new System.IO.FileSystemEventHandler(watcher_Changed);

        //    watcher.EnableRaisingEvents = true;
        //}

        //private void stopWatch()
        //{
        //    watcher.EnableRaisingEvents = false;
        //    watcher.Dispose();
        //    watcher = null;
        //}

        private void Watcher_Changed(System.Object source, System.IO.FileSystemEventArgs e)
        {
            string trgFile = Path.Combine(opeFolder, "ope.txt");

            lock (remoteLockObj)
            {
                if (remoteBusy)
                {
                    try
                    {
                        File.Delete(trgFile);
                    }
                    catch (Exception deleteEx)
                    {
                        log.ForcedWrite(deleteEx);
                    }
                    return;
                }
                remoteBusy = true;
            }

            try
            {
                switch (e.ChangeType)
                {
                    case System.IO.WatcherChangeTypes.Changed:
                    case System.IO.WatcherChangeTypes.Created:

                        long n = DateTime.Now.Ticks / 1_000_000L;
                        if (now == n)
                        {
                            try
                            {
                                File.Delete(trgFile);
                            }
                            catch (Exception deleteEx)
                            {
                                log.ForcedWrite(deleteEx);
                            }
                            return;
                        }
                        now = n;

                        if (!File.Exists(trgFile)) return;
                        string[] lins = null;
                        int retry = 30;
                        while (retry > 0)
                        {
                            try
                            {
                                lins = File.ReadAllLines(trgFile);
                                retry = 0;
                            }
                            catch (IOException)
                            {
                                System.Threading.Thread.Sleep(100);
                                retry--;
                            }
                        }

                        try
                        {
                            File.Delete(trgFile);
                        }
                        catch (Exception deleteEx)
                        {
                            log.ForcedWrite(deleteEx);
                        }

                        remoteReq.Add(lins);

                        break;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            finally
            {
                lock (remoteLockObj)
                {
                    remoteBusy = false;
                }
            }
        }


        private void Remote(string line)
        {
            try
            {
                int n = Math.Min(
                    !line.Contains(' ') ? int.MaxValue : line.IndexOf(' '),
                    !line.Contains('\t') ? int.MaxValue : line.IndexOf('\t')
                    );
                string command = line;
                string optionLine = "";
                if (n != int.MaxValue)
                {
                    command = line[..(n + 1)].ToUpper().Trim();
                    optionLine = line[n..].Trim();
                }

                switch (command)
                {
                    case "PLAY":
                        if (!string.IsNullOrEmpty(optionLine))
                        {
                            if (optionLine[0] == '\"' && optionLine[^1] == '\"')
                            {
                                optionLine = optionLine[1..^1];
                            }
                            AddFileAndPlay(new string[] { optionLine });
                        }
                        else
                            TsmiPlay_Click(null, null);
                        break;
                    case "STOP":
                        TsmiStop_Click(null, null);
                        break;
                    case "NEXT":
                        TsmiNext_Click(null, null);
                        break;
                    case "PREV":
                        opeButtonPrevious_Click(null, null);
                        break;
                    case "FADEOUT":
                        TsmiFadeOut_Click(null, null);
                        break;
                    case "FAST":
                        TsmiFf_Click(null, null);
                        break;
                    case "SLOW":
                        TsmiSlow_Click(null, null);
                        break;
                    case "PAUSE":
                        TsmiPause_Click(null, null);
                        break;
                    case "CLOSE":
                        Close();
                        break;
                    case "LOOP":
                        TsmiPlayMode_Click(null, null);
                        break;
                    case "MIXER":
                        TsmiOpenMixer_Click(null, null);
                        break;
                    case "INFO":
                        TsmiOpenInfo_Click(null, null);
                        break;
                    case "SPLAY":

                        string lin = optionLine.Trim();
                        string appName = lin[..lin.IndexOf(" ")];
                        lin = lin[lin.IndexOf(" ")..].Trim();
                        string mName = lin[..lin.IndexOf(" ")];
                        lin = lin[lin.IndexOf(" ")..].Trim();
                        int count = int.Parse(lin[..lin.IndexOf(" ")]);
                        lin = lin[lin.IndexOf(" ")..].Trim();
                        string path = lin.Trim();
                        KumaCom mml2vgmMmf = new fileCom(true, appName, mName, count);
                        byte[] buf = mml2vgmMmf.GetBytes();

                        bufferPlay(buf, path);

                        break;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            this.Close();
        }

        private void changeZoom()
        {
            toolTip1.SetToolTip(opeButtonZoom, zoomTip[setting.other.Zoom - 1]);

            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeControl"].Width * setting.other.Zoom, frameSizeH + ResMng.ImgDic["planeControl"].Height * setting.other.Zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeControl"].Width * setting.other.Zoom, frameSizeH + ResMng.ImgDic["planeControl"].Height * setting.other.Zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeControl"].Width * setting.other.Zoom, frameSizeH + ResMng.ImgDic["planeControl"].Height * setting.other.Zoom);
            frmMain_Resize(null, null);
            RelocateOpeButton(setting.other.Zoom);

            if (frmMCD[0] != null && !frmMCD[0].isClosed)
            {
                TsmiPRF5C164_Click(null, null);
                TsmiPRF5C164_Click(null, null);
            }

            if (frmRf5c68[0] != null && !frmRf5c68[0].isClosed)
            {
                TsmiPRF5C68_Click(null, null);
                TsmiPRF5C68_Click(null, null);
            }

            if (frmRegTest != null && !frmRegTest.isClosed)
            {
                CloseFormRegTest(0);
                OpenFormRegTest(0);
            }

            if (frmC140[0] != null && !frmC140[0].isClosed)
            {
                TsmiPC140_Click(null, null);
                TsmiPC140_Click(null, null);
            }

            if (frmS5B[0] != null && !frmS5B[0].isClosed)
            {
                TsmiPS5B_Click(null, null);
                TsmiPS5B_Click(null, null);
            }

            if (frmDMG[0] != null && !frmDMG[0].isClosed)
            {
                TsmiPDMG_Click(null, null);
                TsmiPDMG_Click(null, null);
            }

            if (frmPPZ8[0] != null && !frmPPZ8[0].isClosed)
            {
                TsmiPPPZ8_Click(null, null);
                TsmiPPPZ8_Click(null, null);
            }

            if (frmYMZ280B[0] != null && !frmYMZ280B[0].isClosed)
            {
                TsmiYMZ280B_Click(null, null);
                TsmiYMZ280B_Click(null, null);
            }

            if (frmC352[0] != null && !frmC352[0].isClosed)
            {
                TsmiPC352_Click(null, null);
                TsmiPC352_Click(null, null);
            }

            if (frmQSound[0] != null && !frmQSound[0].isClosed)
            {
                TsmiPQSound_Click(null, null);
                TsmiPQSound_Click(null, null);
            }

            if (frmYM2608[0] != null && !frmYM2608[0].isClosed)
            {
                TsmiPOPNA_Click(null, null);
                TsmiPOPNA_Click(null, null);
            }

            if (frmYM2151[0] != null && !frmYM2151[0].isClosed)
            {
                TsmiPOPM_Click(null, null);
                TsmiPOPM_Click(null, null);
            }

            if (frmYM2203[0] != null && !frmYM2203[0].isClosed)
            {
                tsmiPOPN_Click(null, null);
                tsmiPOPN_Click(null, null);
            }

            if (frmYM2413[0] != null && !frmYM2413[0].isClosed)
            {
                TsmiPOPLL_Click(null, null);
                TsmiPOPLL_Click(null, null);
            }

            if (frmYM2609[0] != null && !frmYM2609[0].isClosed)
            {
                TsmiPOPNA2_Click(null, null);
                TsmiPOPNA2_Click(null, null);
            }

            if (frmYM2610[0] != null && !frmYM2610[0].isClosed)
            {
                TsmiPOPNB_Click(null, null);
                TsmiPOPNB_Click(null, null);
            }

            if (frmYM2612[0] != null && !frmYM2612[0].isClosed)
            {
                tsmiPOPN2_Click(null, null);
                tsmiPOPN2_Click(null, null);
            }

            if (frmYM3526[0] != null && !frmYM3526[0].isClosed)
            {
                TsmiPOPL_Click(null, null);
                TsmiPOPL_Click(null, null);
            }

            if (frmY8950[0] != null && !frmY8950[0].isClosed)
            {
                TsmiPY8950_Click(null, null);
                TsmiPY8950_Click(null, null);
            }

            if (frmYM3812[0] != null && !frmYM3812[0].isClosed)
            {
                TsmiPOPL2_Click(null, null);
                TsmiPOPL2_Click(null, null);
            }

            if (frmYMF262[0] != null && !frmYMF262[0].isClosed)
            {
                TsmiPOPL3_Click(null, null);
                TsmiPOPL3_Click(null, null);
            }

            if (frmYMF271[0] != null && !frmYMF271[0].isClosed)
            {
                TsmiPOPX_Click(null, null);
                TsmiPOPX_Click(null, null);
            }

            if (frmYMF278B[0] != null && !frmYMF278B[0].isClosed)
            {
                TsmiPOPL4_Click(null, null);
                TsmiPOPL4_Click(null, null);
            }

            if (frmOKIM6258[0] != null && !frmOKIM6258[0].isClosed)
            {
                TsmiPOKIM6258_Click(null, null);
                TsmiPOKIM6258_Click(null, null);
            }

            if (frmOKIM6295[0] != null && !frmOKIM6295[0].isClosed)
            {
                TsmiPOKIM6295_Click(null, null);
                TsmiPOKIM6295_Click(null, null);
            }

            if (frmPCM8[0] != null && !frmPCM8[0].isClosed)
            {
                TsmiPPCM8_Click(null, null);
                TsmiPPCM8_Click(null, null);
            }

            if (frmSN76489[0] != null && !frmSN76489[0].isClosed)
            {
                TsmiPDCSG_Click(null, null);
                TsmiPDCSG_Click(null, null);
            }

            if (frmSegaPCM[0] != null && !frmSegaPCM[0].isClosed)
            {
                TsmiPSegaPCM_Click(null, null);
                TsmiPSegaPCM_Click(null, null);
            }

            if (frmAY8910[0] != null && !frmAY8910[0].isClosed)
            {
                TsmiPAY8910_Click(null, null);
                TsmiPAY8910_Click(null, null);
            }

            if (frmHuC6280[0] != null && !frmHuC6280[0].isClosed)
            {
                TsmiPHuC6280_Click(null, null);
                TsmiPHuC6280_Click(null, null);
            }

            if (frmK051649[0] != null && !frmK051649[0].isClosed)
            {
                TsmiPK051649_Click(null, null);
                TsmiPK051649_Click(null, null);
            }



            if (frmMCD[1] != null && !frmMCD[1].isClosed)
            {
                TsmiSRF5C164_Click(null, null);
                TsmiSRF5C164_Click(null, null);
            }

            if (frmRf5c68[1] != null && !frmRf5c68[1].isClosed)
            {
                TsmiSRF5C68_Click(null, null);
                TsmiSRF5C68_Click(null, null);
            }

            if (frmC140[1] != null && !frmC140[1].isClosed)
            {
                TsmiSC140_Click(null, null);
                TsmiSC140_Click(null, null);
            }

            if (frmS5B[1] != null && !frmS5B[1].isClosed)
            {
                tsmiSS5B_Click(null, null);
                tsmiSS5B_Click(null, null);
            }

            if (frmDMG[1] != null && !frmDMG[1].isClosed)
            {
                TsmiSDMG_Click(null, null);
                TsmiSDMG_Click(null, null);
            }

            if (frmPPZ8[1] != null && !frmPPZ8[1].isClosed)
            {
                TsmiSPPZ8_Click(null, null);
                TsmiSPPZ8_Click(null, null);
            }

            if (frmYMZ280B[1] != null && !frmYMZ280B[1].isClosed)
            {
                TsmiSYMZ280B_Click(null, null);
                TsmiSYMZ280B_Click(null, null);
            }

            if (frmC352[1] != null && !frmC352[1].isClosed)
            {
                TsmiSC352_Click(null, null);
                TsmiSC352_Click(null, null);
            }

            if (frmYM2608[1] != null && !frmYM2608[1].isClosed)
            {
                TsmiSOPNA_Click(null, null);
                TsmiSOPNA_Click(null, null);
            }

            if (frmYM2151[1] != null && !frmYM2151[1].isClosed)
            {
                TsmiSOPM_Click(null, null);
                TsmiSOPM_Click(null, null);
            }

            if (frmYM2203[1] != null && !frmYM2203[1].isClosed)
            {
                TsmiSOPN_Click(null, null);
                TsmiSOPN_Click(null, null);
            }

            if (frmYM3526[1] != null && !frmYM3526[1].isClosed)
            {
                TsmiSOPL_Click(null, null);
                TsmiSOPL_Click(null, null);
            }

            if (frmY8950[1] != null && !frmY8950[1].isClosed)
            {
                TsmiSY8950_Click(null, null);
                TsmiSY8950_Click(null, null);
            }

            if (frmYM3812[1] != null && !frmYM3812[1].isClosed)
            {
                TsmiSOPL2_Click(null, null);
                TsmiSOPL2_Click(null, null);
            }

            if (frmYM2413[1] != null && !frmYM2413[1].isClosed)
            {
                TsmiSOPLL_Click(null, null);
                TsmiSOPLL_Click(null, null);
            }

            if (frmYM2609[1] != null && !frmYM2609[1].isClosed)
            {
                TsmiSOPNA2_Click(null, null);
                TsmiSOPNA2_Click(null, null);
            }

            if (frmYM2610[1] != null && !frmYM2610[1].isClosed)
            {
                TsmiSOPNB_Click(null, null);
                TsmiSOPNB_Click(null, null);
            }

            if (frmYM2612[1] != null && !frmYM2612[1].isClosed)
            {
                TsmiSOPN2_Click(null, null);
                TsmiSOPN2_Click(null, null);
            }

            if (frmYMF262[1] != null && !frmYMF262[1].isClosed)
            {
                TsmiSOPL3_Click(null, null);
                TsmiSOPL3_Click(null, null);
            }

            if (frmYMF271[1] != null && !frmYMF271[1].isClosed)
            {
                TsmiSOPX_Click(null, null);
                TsmiSOPX_Click(null, null);
            }

            if (frmYMF278B[1] != null && !frmYMF278B[1].isClosed)
            {
                TsmiSOPL4_Click(null, null);
                TsmiSOPL4_Click(null, null);
            }

            if (frmOKIM6258[1] != null && !frmOKIM6258[1].isClosed)
            {
                TsmiSOKIM6258_Click(null, null);
                TsmiSOKIM6258_Click(null, null);
            }

            if (frmOKIM6295[1] != null && !frmOKIM6295[1].isClosed)
            {
                TsmiSOKIM6295_Click(null, null);
                TsmiSOKIM6295_Click(null, null);
            }

            if (frmPCM8[1] != null && !frmPCM8[1].isClosed)
            {
                TsmiSPCM8_Click(null, null);
                TsmiSPCM8_Click(null, null);
            }

            if (frmSN76489[1] != null && !frmSN76489[1].isClosed)
            {
                TsmiSDCSG_Click(null, null);
                TsmiSDCSG_Click(null, null);
            }

            if (frmSegaPCM[1] != null && !frmSegaPCM[1].isClosed)
            {
                TsmiSSegaPCM_Click(null, null);
                TsmiSSegaPCM_Click(null, null);
            }

            if (frmAY8910[1] != null && !frmAY8910[1].isClosed)
            {
                TsmiSAY8910_Click(null, null);
                TsmiSAY8910_Click(null, null);
            }

            if (frmHuC6280[1] != null && !frmHuC6280[1].isClosed)
            {
                TsmiSHuC6280_Click(null, null);
                TsmiSHuC6280_Click(null, null);
            }

            if (frmK051649[1] != null && !frmK051649[1].isClosed)
            {
                TsmiSK051649_Click(null, null);
                TsmiSK051649_Click(null, null);
            }

            if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed)
            {
                openMIDIKeyboard();
                openMIDIKeyboard();
            }

            if (frmMIDI[0] != null && !frmMIDI[0].isClosed)
            {
                OpenFormMIDI(0);
                OpenFormMIDI(0);
            }

            if (frmMIDI[1] != null && !frmMIDI[1].isClosed)
            {
                OpenFormMIDI(1);
                OpenFormMIDI(1);
            }

            if (frmVRC6[0] != null && !frmVRC6[0].isClosed)
            {
                OpenFormVRC6(0);
                OpenFormVRC6(0);
            }

            if (frmVRC6[1] != null && !frmVRC6[1].isClosed)
            {
                OpenFormVRC6(1);
                OpenFormVRC6(1);
            }

            if (frmVRC7[0] != null && !frmVRC7[0].isClosed)
            {
                OpenFormVRC7(0);
                OpenFormVRC7(0);
            }

            if (frmVRC7[1] != null && !frmVRC7[1].isClosed)
            {
                OpenFormVRC7(1);
                OpenFormVRC7(1);
            }

            if (frmNESDMC[0] != null && !frmNESDMC[0].isClosed)
            {
                OpenFormNESDMC(0);
                OpenFormNESDMC(0);
            }

            if (frmNESDMC[1] != null && !frmNESDMC[1].isClosed)
            {
                OpenFormNESDMC(1);
                OpenFormNESDMC(1);
            }

            if (frmN106[0] != null && !frmN106[0].isClosed)
            {
                OpenFormN106(0);
                OpenFormN106(0);
            }

            if (frmN106[1] != null && !frmN106[1].isClosed)
            {
                OpenFormN106(1);
                OpenFormN106(1);
            }

            if (frmK053260[0] != null && !frmK053260[0].isClosed)
            {
                OpenFormK053260(0);
                OpenFormK053260(0);
            }

            if (frmK053260[1] != null && !frmK053260[1].isClosed)
            {
                OpenFormK053260(1);
                OpenFormK053260(1);
            }

            if (frmMixer2 != null && !frmMixer2.isClosed)
            {
                openMixer();
                openMixer();
            }

        }

        private void frmMain_Shown(object sender, EventArgs e)
        {

            log.ForcedWrite("frmMain_Shown:STEP 09");

            Thread trd = new(screenMainLoop)
            {
                Priority = ThreadPriority.BelowNormal
            };
            trd.Start();
            string[] args = Environment.GetCommandLineArgs();
            string arg = "";
            for (int i = 1; i < args.Length; i++) arg += string.Format("\"{0}\"", args[i]);

            Application.DoEvents();
            Activate();

            log.ForcedWrite("起動処理完了");

            log.Write("-------------------------------------------");
            log.Write(" Welcome to the MDPlayer zone. Get Ready ? ");
            log.Write("-------------------------------------------");

            if (args.Length < 2) return;

            log.ForcedWrite("frmMain_Shown:起動時オプション解析");

            try
            {
                PlayArgs(arg);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }

        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            // リサイズ時は再確保

            screen?.Dispose();

            screen = new DoubleBuffer(pbScreen, ResMng.ImgDic["planeControl"], setting.other.Zoom)
            {
                setting = setting
            };
            reqAllScreenInit = true;
            //screen.screenInitAll();

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (forcedExit) return;

            log.ForcedWrite("終了処理開始");
            log.ForcedWrite("frmMain_FormClosing:STEP 00");

            frmPlayList.Stop();
            frmPlayList.Save();

            tonePallet.Save(null);

            log.ForcedWrite("frmMain_FormClosing:STEP 01");

            StopMIDIInMonitoring();
            Request req = new(EnmRequest.Die);
            OpeManager.RequestToAudio(req);
            int timeout = 100;
            while (!req.End && timeout-- > 0)//自殺リクエストはコールバック無し
            {
                System.Threading.Thread.Sleep(1);
            }

            req = new Request(EnmRequest.Stop);
            OpeManager.RequestToAudio(req);
            timeout = 100;
            while (!req.End && timeout-- > 0)
            {
                System.Threading.Thread.Sleep(1);
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 02");

            isRunning = false;
            while (!stopped)
            {
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 03");

            YM2612MIDI.Close();

            // 解放
            screen.Dispose();

            setting.location.OInfo = false;
            setting.location.OPlayList = false;
            setting.location.OMixer = false;
            setting.location.OpenYm2612MIDI = false;
            setting.location.OpenVisWave = false;
            for (int chipID = 0; chipID < 2; chipID++)
            {
                setting.location.OpenAY8910[chipID] = false;
                setting.location.OpenC140[chipID] = false;
                setting.location.OpenPPZ8[chipID] = false;
                setting.location.OpenS5B[chipID] = false;
                setting.location.OpenDMG[chipID] = false;
                setting.location.OpenK053260[chipID] = false;
                setting.location.OpenYMZ280B[chipID] = false;
                setting.location.OpenC352[chipID] = false;
                setting.location.OpenQSound[chipID] = false;
                setting.location.OpenHuC6280[chipID] = false;
                setting.location.OpenK051649[chipID] = false;
                setting.location.OpenMIDI[chipID] = false;
                setting.location.OpenNESDMC[chipID] = false;
                setting.location.OpenFDS[chipID] = false;
                setting.location.OpenMMC5[chipID] = false;
                setting.location.OpenVrc6[chipID] = false;
                setting.location.OpenVrc7[chipID] = false;
                setting.location.OpenN106[chipID] = false;
                setting.location.OpenOKIM6258[chipID] = false;
                setting.location.OpenOKIM6295[chipID] = false;
                setting.location.OpenPCM8[chipID] = false;
                setting.location.OpenRf5c164[chipID] = false;
                setting.location.OpenRf5c68[chipID] = false;
                setting.location.OpenSegaPCM[chipID] = false;
                setting.location.OpenSN76489[chipID] = false;
                setting.location.OpenYm2151[chipID] = false;
                setting.location.OpenYm2203[chipID] = false;
                setting.location.OpenYm2413[chipID] = false;
                setting.location.OpenYm2608[chipID] = false;
                setting.location.OpenYm2610[chipID] = false;
                setting.location.OpenYm2612[chipID] = false;
                setting.location.OpenYm3526[chipID] = false;
                setting.location.OpenY8950[chipID] = false;
                setting.location.OpenYm3812[chipID] = false;
                setting.location.OpenYmf262[chipID] = false;
                setting.location.OpenYMF271[chipID] = false;
                setting.location.OpenYmf278b[chipID] = false;
                setting.location.OpenRegTest[chipID] = false;
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 04");

            if (WindowState == FormWindowState.Normal)
            {
                setting.location.Main.Point = Location;
                setting.location.Main.Size = Size;
            }
            else
            {
                setting.location.Main.Point = RestoreBounds.Location;
                setting.location.Main.Size = RestoreBounds.Size;
            }
            setting.location.Main.State = WindowState;

            if (frmPlayList != null && !frmPlayList.isClosed)
            {
                frmPlayList.Close();
                setting.location.OPlayList = true;
            }
            if (frmInfo != null && !frmInfo.isClosed)
            {
                frmInfo.Close();
                setting.location.OInfo = true;
            }
            if (frmMixer2 != null && !frmMixer2.isClosed)
            {
                frmMixer2.Close();
                setting.location.OMixer = true;
            }
            if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed)
            {
                frmYM2612MIDI.Close();
                setting.location.OpenYm2612MIDI = true;
            }
            if (frmVSTeffectList != null && !frmVSTeffectList.isClosed)
            {
                frmVSTeffectList.Close();
                setting.location.OpenVSTeffectList = true;
            }

            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed)
                {
                    frmAY8910[chipID].Close();
                    setting.location.OpenAY8910[chipID] = true;
                }
                if (frmC140[chipID] != null && !frmC140[chipID].isClosed)
                {
                    frmC140[chipID].Close();
                    setting.location.OpenC140[chipID] = true;
                }
                if (frmPPZ8[chipID] != null && !frmPPZ8[chipID].isClosed)
                {
                    frmPPZ8[chipID].Close();
                    setting.location.OpenPPZ8[chipID] = true;
                }
                if (frmS5B[chipID] != null && !frmS5B[chipID].isClosed)
                {
                    frmS5B[chipID].Close();
                    setting.location.OpenS5B[chipID] = true;
                }
                if (frmDMG[chipID] != null && !frmDMG[chipID].isClosed)
                {
                    frmDMG[chipID].Close();
                    setting.location.OpenDMG[chipID] = true;
                }
                if (frmYMZ280B[chipID] != null && !frmYMZ280B[chipID].isClosed)
                {
                    frmYMZ280B[chipID].Close();
                    setting.location.OpenYMZ280B[chipID] = true;
                }
                if (frmK053260[chipID] != null && !frmK053260[chipID].isClosed)
                {
                    frmK053260[chipID].Close();
                    setting.location.OpenK053260[chipID] = true;
                }
                if (frmC352[chipID] != null && !frmC352[chipID].isClosed)
                {
                    frmC352[chipID].Close();
                    setting.location.OpenC352[chipID] = true;
                }
                if (frmQSound[chipID] != null && !frmQSound[chipID].isClosed)
                {
                    frmQSound[chipID].Close();
                    setting.location.OpenQSound[chipID] = true;
                }
                if (frmFDS[chipID] != null && !frmFDS[chipID].isClosed)
                {
                    frmFDS[chipID].Close();
                    setting.location.OpenFDS[chipID] = true;
                }
                if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed)
                {
                    frmHuC6280[chipID].Close();
                    setting.location.OpenHuC6280[chipID] = true;
                }
                if (frmK051649[chipID] != null && !frmK051649[chipID].isClosed)
                {
                    frmK051649[chipID].Close();
                    setting.location.OpenK051649[chipID] = true;
                }
                if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed)
                {
                    frmMCD[chipID].Close();
                    setting.location.OpenRf5c164[chipID] = true;
                }
                if (frmRf5c68[chipID] != null && !frmRf5c68[chipID].isClosed)
                {
                    frmRf5c68[chipID].Close();
                    setting.location.OpenRf5c68[chipID] = true;
                }
                if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed)
                {
                    frmMIDI[chipID].Close();
                    setting.location.OpenMIDI[chipID] = true;
                }
                if (frmMMC5[chipID] != null && !frmMMC5[chipID].isClosed)
                {
                    frmMMC5[chipID].Close();
                    setting.location.OpenMMC5[chipID] = true;
                }
                if (frmVRC6[chipID] != null && !frmVRC6[chipID].isClosed)
                {
                    frmVRC6[chipID].Close();
                    setting.location.OpenVrc6[chipID] = true;
                }
                if (frmVRC7[chipID] != null && !frmVRC7[chipID].isClosed)
                {
                    frmVRC7[chipID].Close();
                    setting.location.OpenVrc7[chipID] = true;
                }
                if (frmN106[chipID] != null && !frmN106[chipID].isClosed)
                {
                    frmN106[chipID].Close();
                    setting.location.OpenN106[chipID] = true;
                }
                if (frmNESDMC[chipID] != null && !frmNESDMC[chipID].isClosed)
                {
                    frmNESDMC[chipID].Close();
                    setting.location.OpenNESDMC[chipID] = true;
                }
                if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed)
                {
                    frmOKIM6258[chipID].Close();
                    setting.location.OpenOKIM6258[chipID] = true;
                }
                if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed)
                {
                    frmOKIM6295[chipID].Close();
                    setting.location.OpenOKIM6295[chipID] = true;
                }
                if (frmPCM8[chipID] != null && !frmPCM8[chipID].isClosed)
                {
                    frmPCM8[chipID].Close();
                    setting.location.OpenPCM8[chipID] = true;
                }
                if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed)
                {
                    frmSegaPCM[chipID].Close();
                    setting.location.OpenSegaPCM[chipID] = true;
                }
                if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed)
                {
                    frmSN76489[chipID].Close();
                    setting.location.OpenSN76489[chipID] = true;
                }
                if (frmYM2151[chipID] != null && !frmYM2151[chipID].isClosed)
                {
                    frmYM2151[chipID].Close();
                    setting.location.OpenYm2151[chipID] = true;
                }
                if (frmYM2203[chipID] != null && !frmYM2203[chipID].isClosed)
                {
                    frmYM2203[chipID].Close();
                    setting.location.OpenYm2203[chipID] = true;
                }
                if (frmYM2413[chipID] != null && !frmYM2413[chipID].isClosed)
                {
                    frmYM2413[chipID].Close();
                    setting.location.OpenYm2413[chipID] = true;
                }
                if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed)
                {
                    frmYM2608[chipID].Close();
                    setting.location.OpenYm2608[chipID] = true;
                }
                if (frmYM2610[chipID] != null && !frmYM2610[chipID].isClosed)
                {
                    frmYM2610[chipID].Close();
                    setting.location.OpenYm2610[chipID] = true;
                }
                if (frmYM2612[chipID] != null && !frmYM2612[chipID].isClosed)
                {
                    frmYM2612[chipID].Close();
                    setting.location.OpenYm2612[chipID] = true;
                }
                if (frmYM3526[chipID] != null && !frmYM3526[chipID].isClosed)
                {
                    frmYM3526[chipID].Close();
                    setting.location.OpenYm3526[chipID] = true;
                }
                if (frmY8950[chipID] != null && !frmY8950[chipID].isClosed)
                {
                    frmY8950[chipID].Close();
                    setting.location.OpenY8950[chipID] = true;
                }
                if (frmYM3812[chipID] != null && !frmYM3812[chipID].isClosed)
                {
                    frmYM3812[chipID].Close();
                    setting.location.OpenYm3812[chipID] = true;
                }
                if (frmYMF262[chipID] != null && !frmYMF262[chipID].isClosed)
                {
                    frmYMF262[chipID].Close();
                    setting.location.OpenYmf262[chipID] = true;
                }
                if (frmYMF271[chipID] != null && !frmYMF271[chipID].isClosed)
                {
                    frmYMF271[chipID].Close();
                    setting.location.OpenYMF271[chipID] = true;
                }
                if (frmYMF278B[chipID] != null && !frmYMF278B[chipID].isClosed)
                {
                    frmYMF278B[chipID].Close();
                    setting.location.OpenYmf278b[chipID] = true;
                }

                if (frmRegTest != null && !frmRegTest.isClosed)
                {
                    frmRegTest.Close();
                    setting.location.OpenRegTest[chipID] = true;
                }

                if (frmVisWave != null && !frmVisWave.isClosed)
                {
                    frmVisWave.Close();
                    setting.location.OpenVisWave = true;
                }
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 05");

            setting.Save();

            log.ForcedWrite("frmMain_FormClosing:STEP 06");

            mmf.Close();
            ResMng.Release();

            log.ForcedWrite("終了処理完了");

        }

        private void PbScreen_MouseMove(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / setting.other.Zoom;
            int py = e.Location.Y / setting.other.Zoom;

            if (py < 9)
            {
                for (int n = 0; n < newButton.Length; n++)
                {
                    newButton[n] = 0;
                }
                return;
            }

            for (int n = 0; n < newButton.Length; n++)
            {
                //if (px >= 320 - (16 - n) * 16 && px < 320 - (15 - n) * 16) newButton[n] = 1;
                if (px >= n * 16 + 17 && px < n * 16 + 33) newButton[n] = 1;
                else newButton[n] = 0;
            }

        }

        private void PbScreen_MouseLeave(object sender, EventArgs e)
        {
            for (int i = 0; i < newButton.Length; i++)
                newButton[i] = 0;
        }

        private void PbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cmsMenu.Show();
                System.Drawing.Point p = Control.MousePosition;
                cmsMenu.Top = p.Y;
                cmsMenu.Left = p.X;
                return;
            }

            //int px = e.Location.X / setting.other.Zoom;
            //int py = e.Location.Y / setting.other.Zoom;

            //if (py < 16)
            //{
            //    if (px < 8 * 2) return;
            //    if (px < 8 * 5 + 4)
            //    {
            //        if (py < 8) tsmiPAY8910_Click(null, null);
            //        else tsmiSAY8910_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 7)
            //    {
            //        if (py < 8) tsmiPOPLL_Click(null, null);
            //        else tsmiSOPLL_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 9)
            //    {
            //        if (py < 8) tsmiPOPN_Click(null, null);
            //        else tsmiSOPN_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 11)
            //    {
            //        if (py < 8) tsmiPOPN2_Click(null, null);
            //        else tsmiSOPN2_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 13 + 4)
            //    {
            //        if (py < 8) tsmiPOPNA_Click(null, null);
            //        else tsmiSOPNA_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 16)
            //    {
            //        if (py < 8) tsmiPOPNB_Click(null, null);
            //        else tsmiSOPNB_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 18 + 4)
            //    {
            //        if (py < 8) tsmiPOPM_Click(null, null);
            //        else tsmiSOPM_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 20 + 4)
            //    {
            //        if (py < 8) tsmiPDCSG_Click(null, null);
            //        else tsmiSDCSG_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 23)
            //    {
            //        if (py < 8) tsmiPRF5C164_Click(null, null);
            //        else tsmiSRF5C164_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 25 + 4)
            //    {
            //        return;
            //    }
            //    if (px < 8 * 27 + 4)
            //    {
            //        if (py < 8) tsmiPOKIM6258_Click(null, null);
            //        else tsmiSOKIM6258_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 30)
            //    {
            //        if (py < 8) tsmiPOKIM6295_Click(null, null);
            //        else tsmiSOKIM6295_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 32 + 4)
            //    {
            //        if (py < 8) tsmiPC140_Click(null, null);
            //        else tsmiSC140_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 35)
            //    {
            //        if (py < 8) tsmiPSegaPCM_Click(null, null);
            //        else tsmiSSegaPCM_Click(null, null);
            //        return;
            //    }
            //    if (px < 8 * 37 + 4)
            //    {
            //        if (py < 8) tsmiPHuC6280_Click(null, null);
            //        else tsmiSHuC6280_Click(null, null);
            //        return;
            //    }
            //    return;
            //}

        }



        private void tsmiPOPN_Click(object sender, EventArgs e)
        {
            OpenFormYM2203(0);
        }

        private void tsmiPOPN2_Click(object sender, EventArgs e)
        {
            OpenFormYM2612(0);
        }

        private void TsmiPOPNA_Click(object sender, EventArgs e)
        {
            OpenFormYM2608(0);
        }

        private void TsmiPOPNA2_Click(object sender, EventArgs e)
        {
            OpenFormYM2609(0);
        }

        private void TsmiPOPNB_Click(object sender, EventArgs e)
        {
            OpenFormYM2610(0);
        }

        private void TsmiPOPM_Click(object sender, EventArgs e)
        {
            OpenFormYM2151(0);
        }

        private void TsmiPDCSG_Click(object sender, EventArgs e)
        {
            OpenFormSN76489(0);
        }

        private void TsmiPRF5C164_Click(object sender, EventArgs e)
        {
            OpenFormMegaCD(0);
        }

        private void TsmiPRF5C68_Click(object sender, EventArgs e)
        {
            OpenFormRf5c68(0);
        }

        private void TsmiPPWM_Click(object sender, EventArgs e)
        {

        }

        private void TsmiPOKIM6258_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6258(0);
        }

        private void TsmiPOKIM6295_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6295(0);
        }

        private void TsmiPPCM8_Click(object sender, EventArgs e)
        {
            OpenFormPCM8(0);
        }

        private void TsmiPC140_Click(object sender, EventArgs e)
        {
            OpenFormC140(0);
        }

        private void TsmiPPPZ8_Click(object sender, EventArgs e)
        {
            OpenFormPPZ8(0);
        }

        private void TsmiSPPZ8_Click(object sender, EventArgs e)
        {
            OpenFormPPZ8(1);
        }

        private void TsmiPS5B_Click(object sender, EventArgs e)
        {
            OpenFormS5B(0);
        }

        private void tsmiSS5B_Click(object sender, EventArgs e)
        {
            OpenFormS5B(1);
        }

        private void TsmiPDMG_Click(object sender, EventArgs e)
        {
            OpenFormDMG(0);
        }

        private void TsmiSDMG_Click(object sender, EventArgs e)
        {
            OpenFormDMG(1);
        }

        private void TsmiPC352_Click(object sender, EventArgs e)
        {
            OpenFormC352(0);
        }

        private void TsmiPMultiPCM_Click(object sender, EventArgs e)
        {
            OpenFormMultiPCM(0);
        }

        private void TsmiPQSound_Click(object sender, EventArgs e)
        {
            OpenFormQSound(0);
        }

        private void TsmiPSegaPCM_Click(object sender, EventArgs e)
        {
            OpenFormSegaPCM(0);
        }

        private void TsmiPAY8910_Click(object sender, EventArgs e)
        {
            OpenFormAY8910(0);
        }

        private void TsmiPOPLL_Click(object sender, EventArgs e)
        {
            OpenFormYM2413(0);
        }

        private void TsmiPOPL_Click(object sender, EventArgs e)
        {
            OpenFormYM3526(0);
        }

        private void TsmiPY8950_Click(object sender, EventArgs e)
        {
            OpenFormY8950(0);
        }

        private void TsmiPOPL2_Click(object sender, EventArgs e)
        {
            OpenFormYM3812(0);
        }

        private void TsmiPOPL3_Click(object sender, EventArgs e)
        {
            OpenFormYMF262(0);
        }

        private void TsmiPOPL4_Click(object sender, EventArgs e)
        {
            OpenFormYMF278B(0);
        }

        private void TsmiPOPX_Click(object sender, EventArgs e)
        {
            OpenFormYMF271(0);
        }

        private void TsmiPHuC6280_Click(object sender, EventArgs e)
        {
            OpenFormHuC6280(0);
        }

        private void TsmiPK051649_Click(object sender, EventArgs e)
        {
            OpenFormK051649(0);
        }

        private void TsmiPMMC5_Click(object sender, EventArgs e)
        {
            OpenFormMMC5(0);
        }

        private void TsmiSMMC5_Click(object sender, EventArgs e)
        {
            OpenFormMMC5(1);
        }

        private void TsmiSOPN_Click(object sender, EventArgs e)
        {
            OpenFormYM2203(1);
        }

        private void TsmiSOPN2_Click(object sender, EventArgs e)
        {
            OpenFormYM2612(1);
        }

        private void TsmiSOPNA_Click(object sender, EventArgs e)
        {
            OpenFormYM2608(1);
        }

        private void TsmiSOPNA2_Click(object sender, EventArgs e)
        {
            OpenFormYM2609(1);
        }

        private void TsmiSOPNB_Click(object sender, EventArgs e)
        {
            OpenFormYM2610(1);
        }

        private void TsmiSOPM_Click(object sender, EventArgs e)
        {
            OpenFormYM2151(1);
        }

        private void TsmiSDCSG_Click(object sender, EventArgs e)
        {
            OpenFormSN76489(1);
        }

        private void TsmiSRF5C164_Click(object sender, EventArgs e)
        {
            OpenFormMegaCD(1);
        }

        private void TsmiSRF5C68_Click(object sender, EventArgs e)
        {
            OpenFormRf5c68(1);
        }

        private void TsmiSPWM_Click(object sender, EventArgs e)
        {

        }

        private void TsmiSOKIM6258_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6258(1);
        }

        private void TsmiSOKIM6295_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6295(1);
        }

        private void TsmiSPCM8_Click(object sender, EventArgs e)
        {
            OpenFormPCM8(1);
        }

        private void TsmiSC140_Click(object sender, EventArgs e)
        {
            OpenFormC140(1);
        }

        private void TsmiYMZ280B_Click(object sender, EventArgs e)
        {
            OpenFormYMZ280B(0);
        }

        private void TsmiSYMZ280B_Click(object sender, EventArgs e)
        {
            OpenFormYMZ280B(1);
        }

        private void TsmiSC352_Click(object sender, EventArgs e)
        {
            OpenFormC352(1);
        }

        private void TsmiSMultiPCM_Click(object sender, EventArgs e)
        {
            OpenFormMultiPCM(1);
        }

        private void TsmiSQSound_Click(object sender, EventArgs e)
        {
            OpenFormQSound(1);
        }

        private void TsmiSSegaPCM_Click(object sender, EventArgs e)
        {
            OpenFormSegaPCM(1);
        }

        private void TsmiSAY8910_Click(object sender, EventArgs e)
        {
            OpenFormAY8910(1);
        }

        private void TsmiSOPLL_Click(object sender, EventArgs e)
        {
            OpenFormYM2413(1);
        }

        private void TsmiSOPL_Click(object sender, EventArgs e)
        {
            OpenFormYM3526(1);
        }

        private void TsmiSY8950_Click(object sender, EventArgs e)
        {
            OpenFormY8950(1);
        }

        private void TsmiSOPL2_Click(object sender, EventArgs e)
        {
            OpenFormYM3812(1);
        }

        private void TsmiSOPL3_Click(object sender, EventArgs e)
        {
            OpenFormYMF262(1);
        }

        private void TsmiSOPL4_Click(object sender, EventArgs e)
        {
            OpenFormYMF278B(1);
        }

        private void TsmiSOPX_Click(object sender, EventArgs e)
        {
            OpenFormYMF271(1);
        }

        private void TsmiSHuC6280_Click(object sender, EventArgs e)
        {
            OpenFormHuC6280(1);
        }

        private void TsmiSK051649_Click(object sender, EventArgs e)
        {
            OpenFormK051649(1);
        }

        private void TsmiPMIDI_Click(object sender, EventArgs e)
        {
            OpenFormMIDI(0);
        }

        private void TsmiSMIDI_Click(object sender, EventArgs e)
        {
            OpenFormMIDI(1);
        }

        private void TsmiPNESDMC_Click(object sender, EventArgs e)
        {
            OpenFormNESDMC(0);
        }

        private void TsmiSNESDMC_Click(object sender, EventArgs e)
        {
            OpenFormNESDMC(1);
        }

        private void TsmiPFDS_Click(object sender, EventArgs e)
        {
            OpenFormFDS(0);
        }

        private void TsmiSFDS_Click(object sender, EventArgs e)
        {
            OpenFormFDS(1);
        }

        private void TsmiPVRC6_Click(object sender, EventArgs e)
        {
            OpenFormVRC6(0);
        }

        private void TsmiSVRC6_Click(object sender, EventArgs e)
        {
            OpenFormVRC6(1);
        }

        private void TsmiPVRC7_Click(object sender, EventArgs e)
        {
            OpenFormVRC7(0);
        }

        private void TsmiSVRC7_Click(object sender, EventArgs e)
        {
            OpenFormVRC7(1);
        }

        private void TsmiPN106_Click(object sender, EventArgs e)
        {
            OpenFormN106(0);
        }

        private void TsmiSN106_Click(object sender, EventArgs e)
        {
            OpenFormN106(1);
        }

        private void TsmiPK053260_Click(global::System.Object sender, global::System.EventArgs e)
        {
            OpenFormK053260(0);
        }

        private void TsmiSK053260_Click(global::System.Object sender, global::System.EventArgs e)
        {
            OpenFormK053260(1);
        }








        private void TsmiVisWave_Click(object sender, EventArgs e)
        {
            OpenFormVisWave();
        }


        private void OpenFormMegaCD(int chipID, bool force = false)
        {
            if (frmMCD[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormMegaCD(chipID);
                    return;
                }
                else
                    return;
            }

            frmMCD[chipID] = new frmMegaCD(this, chipID, setting.other.Zoom, newParam.rf5c164[chipID], oldParam.rf5c164[chipID]);
            if (setting.location.PosRf5c164[chipID] == System.Drawing.Point.Empty)
            {
                frmMCD[chipID].x = this.Location.X;
                frmMCD[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmMCD[chipID].x = setting.location.PosRf5c164[chipID].X;
                frmMCD[chipID].y = setting.location.PosRf5c164[chipID].Y;
            }

            frmMCD[chipID].Show();
            frmMCD[chipID].update();
            frmMCD[chipID].Text = string.Format("RF5C164 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.rf5c164[chipID] = new MDChipParams.RF5C164();

            CheckAndSetForm(frmMCD[chipID]);
        }

        private void CloseFormMegaCD(int chipID)
        {
            if (frmMCD[chipID] == null) return;

            try
            {
                frmMCD[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
            try
            {
                frmMCD[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmMCD[chipID] = null;
        }


        private void OpenFormRf5c68(int chipID, bool force = false)
        {
            if (frmRf5c68[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormRf5c68(chipID);
                    return;
                }
                else
                    return;
            }

            frmRf5c68[chipID] = new frmRf5c68(this, chipID, setting.other.Zoom, newParam.rf5c68[chipID], oldParam.rf5c68[chipID]);
            if (setting.location.PosRf5c68[chipID] == System.Drawing.Point.Empty)
            {
                frmRf5c68[chipID].x = this.Location.X;
                frmRf5c68[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmRf5c68[chipID].x = setting.location.PosRf5c68[chipID].X;
                frmRf5c68[chipID].y = setting.location.PosRf5c68[chipID].Y;
            }

            frmRf5c68[chipID].Show();
            frmRf5c68[chipID].update();
            frmRf5c68[chipID].Text = string.Format("RF5C68 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.rf5c68[chipID] = new MDChipParams.RF5C68();

            CheckAndSetForm(frmRf5c68[chipID]);
        }

        private void CloseFormRf5c68(int chipID)
        {
            if (frmRf5c68[chipID] == null) return;

            try
            {
                frmRf5c68[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
            try
            {
                frmRf5c68[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmRf5c68[chipID] = null;
        }


        private void OpenFormYMF271(int chipID, bool force = false)
        {
            if (frmYMF271[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYMF271(chipID);
                    return;
                }
                else
                    return;
            }

            frmYMF271[chipID] = new frmYMF271(this, chipID, setting.other.Zoom, newParam.ymf271[chipID], oldParam.ymf271[chipID]);
            if (setting.location.PosYMF271[chipID] == System.Drawing.Point.Empty)
            {
                frmYMF271[chipID].x = this.Location.X;
                frmYMF271[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYMF271[chipID].x = setting.location.PosYMF271[chipID].X;
                frmYMF271[chipID].y = setting.location.PosYMF271[chipID].Y;
            }

            frmYMF271[chipID].Show();
            frmYMF271[chipID].update();
            frmYMF271[chipID].Text = string.Format("YMF271 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ymf271[chipID] = new MDChipParams.YMF271();

            CheckAndSetForm(frmYMF271[chipID]);
        }

        private void CloseFormYMF271(int chipID)
        {
            if (frmYMF271[chipID] == null) return;

            try
            {
                frmYMF271[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
            try
            {
                frmYMF271[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYMF271[chipID] = null;
        }

        private void OpenFormK053260(int chipID, bool force = false)
        {
            if (frmK053260[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormK053260(chipID);
                    return;
                }
                else
                    return;
            }

            frmK053260[chipID] = new frmK053260(
                this, chipID,
                setting.other.Zoom,
                newParam.k053260[chipID], oldParam.k053260[chipID]);
            if (setting.location.PosK053260[chipID] == System.Drawing.Point.Empty)
            {
                frmK053260[chipID].x = this.Location.X;
                frmK053260[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmK053260[chipID].x = setting.location.PosK053260[chipID].X;
                frmK053260[chipID].y = setting.location.PosK053260[chipID].Y;
            }

            frmK053260[chipID].Show();
            frmK053260[chipID].update();
            frmK053260[chipID].Text = string.Format("K053260 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.k053260[chipID] = new MDChipParams.K053260();

            CheckAndSetForm(frmK053260[chipID]);
        }

        private void CloseFormK053260(int chipID)
        {
            if (frmK053260[chipID] == null) return;

            try
            {
                frmK053260[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
            try
            {
                frmK053260[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmK053260[chipID] = null;
        }

        private void OpenFormYM2608(int chipID, bool force = false)
        {
            if (frmYM2608[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2608(chipID);
                    return;
                }
                else
                    return;
            }

            frmYM2608[chipID] = new frmYM2608(this, chipID, setting.other.Zoom, newParam.ym2608[chipID], oldParam.ym2608[chipID]);

            if (setting.location.PosYm2608[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2608[chipID].x = this.Location.X;
                frmYM2608[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2608[chipID].x = setting.location.PosYm2608[chipID].X;
                frmYM2608[chipID].y = setting.location.PosYm2608[chipID].Y;
            }

            frmYM2608[chipID].Show();
            frmYM2608[chipID].update();
            frmYM2608[chipID].Text = string.Format("YM2608 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2608[chipID] = new MDChipParams.YM2608();

            CheckAndSetForm(frmYM2608[chipID]);
        }

        private void CloseFormYM2608(int chipID)
        {
            if (frmYM2608[chipID] == null) return;

            try
            {
                frmYM2608[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2608[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2608[chipID] = null;
            return;
        }

        private void OpenFormYM2609(int chipID, bool force = false)
        {
            if (frmYM2609[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2609(chipID);
                    return;
                }
                else
                    return;
            }

            frmYM2609[chipID] = new frmYM2609(this, chipID, setting.other.Zoom, newParam.ym2609[chipID], oldParam.ym2609[chipID]);

            if (setting.location.PosYm2609[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2609[chipID].x = this.Location.X;
                frmYM2609[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2609[chipID].x = setting.location.PosYm2609[chipID].X;
                frmYM2609[chipID].y = setting.location.PosYm2609[chipID].Y;
            }

            frmYM2609[chipID].Show();
            frmYM2609[chipID].update();
            frmYM2609[chipID].Text = string.Format("YM2609 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2609[chipID] = new MDChipParams.YM2609();

            CheckAndSetForm(frmYM2609[chipID]);
        }

        private void CloseFormYM2609(int chipID)
        {
            if (frmYM2609[chipID] == null) return;

            try
            {
                frmYM2609[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2609[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2609[chipID] = null;
            return;
        }

        private void OpenFormYM2151(int chipID, bool force = false)
        {
            if (frmYM2151[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2151(chipID);
                    return;
                }
                else return;
            }

            frmYM2151[chipID] = new frmYM2151(this, chipID, setting.other.Zoom, newParam.ym2151[chipID], oldParam.ym2151[chipID]);

            if (setting.location.PosYm2151[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2151[chipID].x = this.Location.X;
                frmYM2151[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2151[chipID].x = setting.location.PosYm2151[chipID].X;
                frmYM2151[chipID].y = setting.location.PosYm2151[chipID].Y;
            }

            frmYM2151[chipID].Show();
            frmYM2151[chipID].update();
            frmYM2151[chipID].Text = string.Format("YM2151 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2151[chipID] = new MDChipParams.YM2151();

            CheckAndSetForm(frmYM2151[chipID]);
        }

        private void CloseFormYM2151(int chipID)
        {
            if (frmYM2151[chipID] == null) return;

            try
            {
                frmYM2151[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2151[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2151[chipID] = null;
        }

        private void OpenFormC140(int chipID, bool force = false)
        {
            if (frmC140[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormC140(chipID);
                    return;
                }
                else return;
            }

            frmC140[chipID] = new frmC140(this, chipID, setting.other.Zoom, newParam.c140[chipID], oldParam.c140[chipID]);

            if (setting.location.PosC140[chipID] == System.Drawing.Point.Empty)
            {
                frmC140[chipID].x = this.Location.X;
                frmC140[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmC140[chipID].x = setting.location.PosC140[chipID].X;
                frmC140[chipID].y = setting.location.PosC140[chipID].Y;
            }

            frmC140[chipID].Show();
            frmC140[chipID].update();
            frmC140[chipID].Text = string.Format("C140 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.c140[chipID] = new MDChipParams.C140();

            CheckAndSetForm(frmC140[chipID]);
        }

        private void CloseFormC140(int chipID)
        {
            if (frmC140[chipID] == null) return;

            try
            {
                frmC140[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmC140[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmC140[chipID] = null;
        }

        private void OpenFormPPZ8(int chipID, bool force = false)
        {
            if (frmPPZ8[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormPPZ8(chipID);
                    return;
                }
                else return;
            }

            frmPPZ8[chipID] = new frmPPZ8(this, chipID, setting.other.Zoom, newParam.ppz8[chipID], oldParam.ppz8[chipID]);

            if (setting.location.PosPPZ8[chipID] == System.Drawing.Point.Empty)
            {
                frmPPZ8[chipID].x = this.Location.X;
                frmPPZ8[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmPPZ8[chipID].x = setting.location.PosPPZ8[chipID].X;
                frmPPZ8[chipID].y = setting.location.PosPPZ8[chipID].Y;
            }

            frmPPZ8[chipID].Show();
            frmPPZ8[chipID].update();
            frmPPZ8[chipID].Text = string.Format("PPZ8 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ppz8[chipID] = new MDChipParams.PPZ8();

            CheckAndSetForm(frmPPZ8[chipID]);
        }

        private void CloseFormPPZ8(int chipID)
        {
            if (frmPPZ8[chipID] == null) return;

            try
            {
                frmPPZ8[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmPPZ8[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmPPZ8[chipID] = null;
        }

        private void OpenFormS5B(int chipID, bool force = false)
        {
            if (frmS5B[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormS5B(chipID);
                    return;
                }
                else return;
            }

            frmS5B[chipID] = new frmS5B(this, chipID, setting.other.Zoom, newParam.s5b[chipID], oldParam.s5b[chipID]);

            if (setting.location.PosS5B[chipID] == System.Drawing.Point.Empty)
            {
                frmS5B[chipID].x = this.Location.X;
                frmS5B[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmS5B[chipID].x = setting.location.PosS5B[chipID].X;
                frmS5B[chipID].y = setting.location.PosS5B[chipID].Y;
            }

            frmS5B[chipID].Show();
            frmS5B[chipID].update();
            frmS5B[chipID].Text = string.Format("S5B ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.s5b[chipID] = new MDChipParams.S5B();

            CheckAndSetForm(frmS5B[chipID]);
        }

        private void CloseFormS5B(int chipID)
        {
            if (frmS5B[chipID] == null) return;

            try
            {
                frmS5B[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmS5B[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmS5B[chipID] = null;
        }

        private void OpenFormDMG(int chipID, bool force = false)
        {
            if (frmDMG[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormDMG(chipID);
                    return;
                }
                else return;
            }

            frmDMG[chipID] = new frmDMG(this, chipID, setting.other.Zoom, newParam.dmg[chipID], oldParam.dmg[chipID]);

            if (setting.location.PosDMG[chipID] == System.Drawing.Point.Empty)
            {
                frmDMG[chipID].x = this.Location.X;
                frmDMG[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmDMG[chipID].x = setting.location.PosDMG[chipID].X;
                frmDMG[chipID].y = setting.location.PosDMG[chipID].Y;
            }

            frmDMG[chipID].Show();
            frmDMG[chipID].update();
            frmDMG[chipID].Text = string.Format("DMG ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.dmg[chipID] = new MDChipParams.DMG();

            CheckAndSetForm(frmDMG[chipID]);
        }

        private void CloseFormDMG(int chipID)
        {
            if (frmDMG[chipID] == null) return;

            try
            {
                frmDMG[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmDMG[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmDMG[chipID] = null;
        }

        private void OpenFormYMZ280B(int chipID, bool force = false)
        {
            if (frmYMZ280B[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYMZ280B(chipID);
                    return;
                }
                else return;
            }

            frmYMZ280B[chipID] = new frmYMZ280B(this, chipID, setting.other.Zoom, newParam.ymz280b[chipID], oldParam.ymz280b[chipID]);

            if (setting.location.PosYMZ280B[chipID] == System.Drawing.Point.Empty)
            {
                frmYMZ280B[chipID].x = this.Location.X;
                frmYMZ280B[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYMZ280B[chipID].x = setting.location.PosYMZ280B[chipID].X;
                frmYMZ280B[chipID].y = setting.location.PosYMZ280B[chipID].Y;
            }

            frmYMZ280B[chipID].Show();
            frmYMZ280B[chipID].update();
            frmYMZ280B[chipID].Text = string.Format("YMZ280B ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ymz280b[chipID] = new MDChipParams.YMZ280B();

            CheckAndSetForm(frmYMZ280B[chipID]);
        }

        private void CloseFormYMZ280B(int chipID)
        {
            if (frmYMZ280B[chipID] == null) return;

            try
            {
                frmYMZ280B[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYMZ280B[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYMZ280B[chipID] = null;
        }

        private void OpenFormC352(int chipID, bool force = false)
        {
            if (frmC352[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormC352(chipID);
                    return;
                }
                else return;
            }

            frmC352[chipID] = new frmC352(this, chipID, setting.other.Zoom, newParam.c352[chipID], oldParam.c352[chipID]);

            if (setting.location.PosC352[chipID] == System.Drawing.Point.Empty)
            {
                frmC352[chipID].x = this.Location.X;
                frmC352[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmC352[chipID].x = setting.location.PosC352[chipID].X;
                frmC352[chipID].y = setting.location.PosC352[chipID].Y;
            }

            frmC352[chipID].Show();
            frmC352[chipID].update();
            frmC352[chipID].Text = string.Format("C352 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.c352[chipID] = new MDChipParams.C352();

            CheckAndSetForm(frmC352[chipID]);
        }

        private void CloseFormC352(int chipID)
        {
            if (frmC352[chipID] == null) return;

            try
            {
                frmC352[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmC352[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmC352[chipID] = null;
        }

        private void OpenFormMultiPCM(int chipID, bool force = false)
        {
            if (frmMultiPCM[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormMultiPCM(chipID);
                    return;
                }
                else return;
            }

            frmMultiPCM[chipID] = new frmMultiPCM(this, chipID, setting.other.Zoom, newParam.multiPCM[chipID], oldParam.multiPCM[chipID]);

            if (setting.location.PosMultiPCM[chipID] == System.Drawing.Point.Empty)
            {
                frmMultiPCM[chipID].x = this.Location.X;
                frmMultiPCM[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmMultiPCM[chipID].x = setting.location.PosMultiPCM[chipID].X;
                frmMultiPCM[chipID].y = setting.location.PosMultiPCM[chipID].Y;
            }

            frmMultiPCM[chipID].Show();
            frmMultiPCM[chipID].update();
            frmMultiPCM[chipID].Text = string.Format("MultiPCM ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.multiPCM[chipID] = new MDChipParams.MultiPCM();

            CheckAndSetForm(frmMultiPCM[chipID]);
        }

        private void CloseFormMultiPCM(int chipID)
        {
            if (frmMultiPCM[chipID] == null) return;

            try
            {
                frmMultiPCM[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmMultiPCM[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmMultiPCM[chipID] = null;
        }

        private void OpenFormQSound(int chipID, bool force = false)
        {
            if (frmQSound[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormQSound(chipID);
                    return;
                }
                else return;
            }

            frmQSound[chipID] = new frmQSound(this, chipID, setting.other.Zoom, newParam.qSound[chipID], oldParam.qSound[chipID]);

            if (setting.location.PosQSound[chipID] == System.Drawing.Point.Empty)
            {
                frmQSound[chipID].x = this.Location.X;
                frmQSound[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmQSound[chipID].x = setting.location.PosQSound[chipID].X;
                frmQSound[chipID].y = setting.location.PosQSound[chipID].Y;
            }

            frmQSound[chipID].Show();
            frmQSound[chipID].update();
            frmQSound[chipID].Text = string.Format("QSound ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.qSound[chipID] = new MDChipParams.QSound();

            CheckAndSetForm(frmQSound[chipID]);
        }

        private void CloseFormQSound(int chipID)
        {
            if (frmQSound[chipID] == null) return;

            try
            {
                frmQSound[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmQSound[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmQSound[chipID] = null;
        }

        private void OpenFormYM2203(int chipID, bool force = false)
        {
            if (frmYM2203[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2203(chipID);
                    return;
                }
                else return;
            }

            frmYM2203[chipID] = new frmYM2203(this, chipID, setting.other.Zoom, newParam.ym2203[chipID], oldParam.ym2203[chipID]);

            if (setting.location.PosYm2203[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2203[chipID].x = this.Location.X;
                frmYM2203[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2203[chipID].x = setting.location.PosYm2203[chipID].X;
                frmYM2203[chipID].y = setting.location.PosYm2203[chipID].Y;
            }

            frmYM2203[chipID].Show();
            frmYM2203[chipID].update();
            frmYM2203[chipID].Text = string.Format("YM2203 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2203[chipID] = new MDChipParams.YM2203();

            CheckAndSetForm(frmYM2203[chipID]);

        }

        private void CloseFormYM2203(int chipID)
        {
            if (frmYM2203[chipID] == null) return;

            try
            {
                frmYM2203[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2203[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2203[chipID] = null;
        }

        private void OpenFormYM2610(int chipID, bool force = false)
        {
            if (frmYM2610[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2610(chipID);
                    return;
                }
                else return;
            }

            frmYM2610[chipID] = new frmYM2610(this, chipID, setting.other.Zoom, newParam.ym2610[chipID], oldParam.ym2610[chipID]);

            if (setting.location.PosYm2610[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2610[chipID].x = this.Location.X;
                frmYM2610[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2610[chipID].x = setting.location.PosYm2610[chipID].X;
                frmYM2610[chipID].y = setting.location.PosYm2610[chipID].Y;
            }

            frmYM2610[chipID].Show();
            frmYM2610[chipID].update();
            frmYM2610[chipID].Text = string.Format("YM2610 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2610[chipID] = new MDChipParams.YM2610();

            CheckAndSetForm(frmYM2610[chipID]);
        }

        private void CloseFormYM2610(int chipID)
        {
            if (frmYM2610[chipID] == null) return;

            try
            {
                frmYM2610[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2610[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2610[chipID] = null;
        }

        private void OpenFormYM2612(int chipID, bool force = false)
        {
            if (frmYM2612[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2612(chipID);
                    return;
                }
                else return;
            }

            oldParam.ym2612[chipID] = new MDChipParams.YM2612();
            for (int i = 0; i < oldParam.ym2612[chipID].channels.Length; i++)
            {
                oldParam.ym2612[chipID].channels[i].mask = null;
            }
            frmYM2612[chipID] = new frmYM2612(this, chipID, setting.other.Zoom, newParam.ym2612[chipID], oldParam.ym2612[chipID]);

            if (setting.location.PosYm2612[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2612[chipID].x = this.Location.X;
                frmYM2612[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2612[chipID].x = setting.location.PosYm2612[chipID].X;
                frmYM2612[chipID].y = setting.location.PosYm2612[chipID].Y;
            }

            frmYM2612[chipID].Show();
            frmYM2612[chipID].update();
            frmYM2612[chipID].Text = string.Format("YM2612 ({0})", chipID == 0 ? "Primary" : "Secondary");

            CheckAndSetForm(frmYM2612[chipID]);
        }

        private void CloseFormYM2612(int chipID)
        {
            if (frmYM2612[chipID] == null) return;
            try
            {
                frmYM2612[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2612[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2612[chipID] = null;
        }

        private void OpenFormOKIM6258(int chipID, bool force = false)
        {
            if (frmOKIM6258[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormOKIM6258(chipID);
                    return;
                }
                else return;
            }

            frmOKIM6258[chipID] = new frmOKIM6258(this, chipID, setting.other.Zoom, newParam.okim6258[chipID]);

            if (setting.location.PosOKIM6258[chipID] == System.Drawing.Point.Empty)
            {
                frmOKIM6258[chipID].x = this.Location.X;
                frmOKIM6258[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmOKIM6258[chipID].x = setting.location.PosOKIM6258[chipID].X;
                frmOKIM6258[chipID].y = setting.location.PosOKIM6258[chipID].Y;
            }

            frmOKIM6258[chipID].Show();
            frmOKIM6258[chipID].update();
            frmOKIM6258[chipID].Text = string.Format("OKIM6258 ({0})", chipID == 0 ? "Primary" : "Secondary");

            CheckAndSetForm(frmOKIM6258[chipID]);
        }

        private void CloseFormOKIM6258(int chipID)
        {
            if (frmOKIM6258[chipID] == null) return;

            try
            {
                frmOKIM6258[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmOKIM6258[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmOKIM6258[chipID] = null;
        }

        private void OpenFormOKIM6295(int chipID, bool force = false)
        {
            if (frmOKIM6295[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormOKIM6295(chipID);
                    return;
                }
                else return;
            }

            frmOKIM6295[chipID] = new frmOKIM6295(this, chipID, setting.other.Zoom, newParam.okim6295[chipID], oldParam.okim6295[chipID]);

            if (setting.location.PosOKIM6295[chipID] == System.Drawing.Point.Empty)
            {
                frmOKIM6295[chipID].x = this.Location.X;
                frmOKIM6295[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmOKIM6295[chipID].x = setting.location.PosOKIM6295[chipID].X;
                frmOKIM6295[chipID].y = setting.location.PosOKIM6295[chipID].Y;
            }

            frmOKIM6295[chipID].Show();
            frmOKIM6295[chipID].update();
            frmOKIM6295[chipID].Text = string.Format("OKIM6295 ({0})", chipID == 0 ? "Primary" : "Secondary");

            CheckAndSetForm(frmOKIM6295[chipID]);
        }

        private void CloseFormOKIM6295(int chipID)
        {
            if (frmOKIM6295[chipID] == null) return;

            try
            {
                frmOKIM6295[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmOKIM6295[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmOKIM6295[chipID] = null;
        }

        private void OpenFormPCM8(int chipID, bool force = false)
        {
            if (frmPCM8[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormPCM8(chipID);
                    return;
                }
                else return;
            }

            frmPCM8[chipID] = new frmPCM8(this, chipID, setting.other.Zoom, newParam.pcm8[chipID]);

            if (setting.location.PosPCM8[chipID] == System.Drawing.Point.Empty)
            {
                frmPCM8[chipID].x = this.Location.X;
                frmPCM8[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmPCM8[chipID].x = setting.location.PosPCM8[chipID].X;
                frmPCM8[chipID].y = setting.location.PosPCM8[chipID].Y;
            }

            frmPCM8[chipID].Show();
            frmPCM8[chipID].update();
            frmPCM8[chipID].Text = string.Format("PCM8 ({0})", chipID == 0 ? "Primary" : "Secondary");

            CheckAndSetForm(frmPCM8[chipID]);
        }

        private void CloseFormPCM8(int chipID)
        {
            if (frmPCM8[chipID] == null) return;

            try
            {
                frmPCM8[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmPCM8[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmPCM8[chipID] = null;
        }

        private void OpenFormSN76489(int chipID, bool force = false)
        {
            if (frmSN76489[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormSN76489(chipID);
                    return;
                }
                else return;
            }

            frmSN76489[chipID] = new frmSN76489(this, chipID, setting.other.Zoom, newParam.sn76489[chipID], oldParam.sn76489[chipID]);

            if (setting.location.PosSN76489[chipID] == System.Drawing.Point.Empty)
            {
                frmSN76489[chipID].x = this.Location.X;
                frmSN76489[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmSN76489[chipID].x = setting.location.PosSN76489[chipID].X;
                frmSN76489[chipID].y = setting.location.PosSN76489[chipID].Y;
            }

            frmSN76489[chipID].Show();
            frmSN76489[chipID].update();
            frmSN76489[chipID].Text = string.Format("SN76489 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.sn76489[chipID] = new MDChipParams.SN76489();

            CheckAndSetForm(frmSN76489[chipID]);
        }

        private void CloseFormSN76489(int chipID)
        {
            if (frmSN76489[chipID] == null) return;

            try
            {
                frmSN76489[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmSN76489[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmSN76489[chipID] = null;
        }

        private void OpenFormSegaPCM(int chipID, bool force = false)
        {
            if (frmSegaPCM[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormSegaPCM(chipID);
                    return;
                }
                else return;
            }

            frmSegaPCM[chipID] = new frmSegaPCM(this, chipID, setting.other.Zoom, newParam.segaPcm[chipID], oldParam.segaPcm[chipID]);

            if (setting.location.PosSegaPCM[chipID] == System.Drawing.Point.Empty)
            {
                frmSegaPCM[chipID].x = this.Location.X;
                frmSegaPCM[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmSegaPCM[chipID].x = setting.location.PosSegaPCM[chipID].X;
                frmSegaPCM[chipID].y = setting.location.PosSegaPCM[chipID].Y;
            }

            frmSegaPCM[chipID].Show();
            frmSegaPCM[chipID].update();
            frmSegaPCM[chipID].Text = string.Format("SegaPCM ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.segaPcm[chipID] = new MDChipParams.SegaPcm();

            CheckAndSetForm(frmSegaPCM[chipID]);
        }

        private void CloseFormSegaPCM(int chipID)
        {
            if (frmSegaPCM[chipID] == null) return;

            try
            {
                frmSegaPCM[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmSegaPCM[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmSegaPCM[chipID] = null;
        }


        private void OpenFormAY8910(int chipID, bool force = false)
        {
            if (frmAY8910[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormAY8910(chipID);
                    return;
                }
                else return;
            }

            frmAY8910[chipID] = new frmAY8910(this, chipID, setting.other.Zoom, newParam.ay8910[chipID], oldParam.ay8910[chipID]);

            if (setting.location.PosAY8910[chipID] == System.Drawing.Point.Empty)
            {
                frmAY8910[chipID].x = this.Location.X;
                frmAY8910[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmAY8910[chipID].x = setting.location.PosAY8910[chipID].X;
                frmAY8910[chipID].y = setting.location.PosAY8910[chipID].Y;
            }

            frmAY8910[chipID].Show();
            frmAY8910[chipID].update();
            frmAY8910[chipID].Text = string.Format("AY8910 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ay8910[chipID] = new MDChipParams.AY8910();

            CheckAndSetForm(frmAY8910[chipID]);
        }

        private void CloseFormAY8910(int chipID)
        {
            if (frmAY8910[chipID] == null) return;

            try
            {
                frmAY8910[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmAY8910[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmAY8910[chipID] = null;
        }

        private void OpenFormHuC6280(int chipID, bool force = false)
        {
            if (frmHuC6280[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormHuC6280(chipID);
                    return;
                }
                else return;
            }

            frmHuC6280[chipID] = new frmHuC6280(this, chipID, setting.other.Zoom, newParam.huc6280[chipID], oldParam.huc6280[chipID]);

            if (setting.location.PosHuC6280[chipID] == System.Drawing.Point.Empty)
            {
                frmHuC6280[chipID].x = this.Location.X;
                frmHuC6280[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmHuC6280[chipID].x = setting.location.PosHuC6280[chipID].X;
                frmHuC6280[chipID].y = setting.location.PosHuC6280[chipID].Y;
            }

            frmHuC6280[chipID].Show();
            frmHuC6280[chipID].update();
            frmHuC6280[chipID].Text = string.Format("HuC6280 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.huc6280[chipID] = new MDChipParams.HuC6280();

            CheckAndSetForm(frmHuC6280[chipID]);
        }

        private void CloseFormHuC6280(int chipID)
        {
            if (frmHuC6280[chipID] == null) return;

            try
            {
                frmHuC6280[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmHuC6280[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmHuC6280[chipID] = null;
        }

        private void OpenFormK051649(int chipID, bool force = false)
        {
            if (frmK051649[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormK051649(chipID);
                    return;
                }
                else return;
            }

            frmK051649[chipID] = new frmK051649(this, chipID, setting.other.Zoom, newParam.k051649[chipID]);

            if (setting.location.PosK051649[chipID] == System.Drawing.Point.Empty)
            {
                frmK051649[chipID].x = this.Location.X;
                frmK051649[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmK051649[chipID].x = setting.location.PosK051649[chipID].X;
                frmK051649[chipID].y = setting.location.PosK051649[chipID].Y;
            }

            frmK051649[chipID].Show();
            frmK051649[chipID].update();
            frmK051649[chipID].Text = string.Format("K051649 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.k051649[chipID] = new MDChipParams.K051649();

            CheckAndSetForm(frmK051649[chipID]);
        }

        private void CloseFormK051649(int chipID)
        {
            if (frmK051649[chipID] == null) return;

            try
            {
                frmK051649[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmK051649[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmK051649[chipID] = null;
        }

        private void OpenFormYM2413(int chipID, bool force = false)
        {
            if (frmYM2413[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM2413(chipID);
                    return;
                }
                else return;
            }

            frmYM2413[chipID] = new frmYM2413(this, chipID, setting.other.Zoom, newParam.ym2413[chipID], oldParam.ym2413[chipID]);

            if (setting.location.PosYm2413[chipID] == System.Drawing.Point.Empty)
            {
                frmYM2413[chipID].x = this.Location.X;
                frmYM2413[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM2413[chipID].x = setting.location.PosYm2413[chipID].X;
                frmYM2413[chipID].y = setting.location.PosYm2413[chipID].Y;
            }

            frmYM2413[chipID].Show();
            frmYM2413[chipID].update();
            frmYM2413[chipID].Text = string.Format("YM2413/VRC7 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2413[chipID] = new MDChipParams.YM2413();

            CheckAndSetForm(frmYM2413[chipID]);
        }

        private void CloseFormYM2413(int chipID)
        {
            if (frmYM2413[chipID] == null) return;

            try
            {
                frmYM2413[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM2413[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM2413[chipID] = null;
        }

        private void OpenFormYM3526(int chipID, bool force = false)
        {
            if (frmYM3526[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM3526(chipID);
                    return;
                }
                else return;
            }

            frmYM3526[chipID] = new frmYM3526(this, chipID, setting.other.Zoom, newParam.ym3526[chipID], oldParam.ym3526[chipID]);

            if (setting.location.PosYm3526[chipID] == System.Drawing.Point.Empty)
            {
                frmYM3526[chipID].x = this.Location.X;
                frmYM3526[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM3526[chipID].x = setting.location.PosYm3526[chipID].X;
                frmYM3526[chipID].y = setting.location.PosYm3526[chipID].Y;
            }

            frmYM3526[chipID].Show();
            frmYM3526[chipID].update();
            frmYM3526[chipID].Text = string.Format("YM3526 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym3526[chipID] = new MDChipParams.YM3526();

            CheckAndSetForm(frmYM3526[chipID]);
        }

        private void CloseFormYM3526(int chipID)
        {
            if (frmYM3526[chipID] == null) return;

            try
            {
                frmYM3526[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM3526[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM3526[chipID] = null;
        }

        private void OpenFormY8950(int chipID, bool force = false)
        {
            if (frmY8950[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormY8950(chipID);
                    return;
                }
                else return;
            }

            frmY8950[chipID] = new frmY8950(this, chipID, setting.other.Zoom, newParam.y8950[chipID]);

            if (setting.location.PosY8950[chipID] == System.Drawing.Point.Empty)
            {
                frmY8950[chipID].x = this.Location.X;
                frmY8950[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmY8950[chipID].x = setting.location.PosY8950[chipID].X;
                frmY8950[chipID].y = setting.location.PosY8950[chipID].Y;
            }

            frmY8950[chipID].Show();
            frmY8950[chipID].update();
            frmY8950[chipID].Text = string.Format("Y8950 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.y8950[chipID] = new MDChipParams.Y8950();

            CheckAndSetForm(frmY8950[chipID]);
        }

        private void CloseFormY8950(int chipID)
        {
            if (frmY8950[chipID] == null) return;

            try
            {
                frmY8950[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmY8950[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmY8950[chipID] = null;
        }

        private void OpenFormYM3812(int chipID, bool force = false)
        {
            if (frmYM3812[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYM3812(chipID);
                    return;
                }
                else return;
            }

            frmYM3812[chipID] = new frmYM3812(this, chipID, setting.other.Zoom, newParam.ym3812[chipID], oldParam.ym3812[chipID]);

            if (setting.location.PosYm3812[chipID] == System.Drawing.Point.Empty)
            {
                frmYM3812[chipID].x = this.Location.X;
                frmYM3812[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYM3812[chipID].x = setting.location.PosYm3812[chipID].X;
                frmYM3812[chipID].y = setting.location.PosYm3812[chipID].Y;
            }

            frmYM3812[chipID].Show();
            frmYM3812[chipID].update();
            frmYM3812[chipID].Text = string.Format("YM3812 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym3812[chipID] = new MDChipParams.YM3812();

            CheckAndSetForm(frmYM3812[chipID]);
        }

        private void CloseFormYM3812(int chipID)
        {
            if (frmYM3812[chipID] == null) return;

            try
            {
                frmYM3812[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYM3812[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYM3812[chipID] = null;
        }

        private void OpenFormYMF262(int chipID, bool force = false)
        {
            if (frmYMF262[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYMF262(chipID);
                    return;
                }
                else return;
            }

            frmYMF262[chipID] = new frmYMF262(this, chipID, setting.other.Zoom, newParam.ymf262[chipID], oldParam.ymf262[chipID]);

            if (setting.location.PosYmf262[chipID] == System.Drawing.Point.Empty)
            {
                frmYMF262[chipID].x = this.Location.X;
                frmYMF262[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYMF262[chipID].x = setting.location.PosYmf262[chipID].X;
                frmYMF262[chipID].y = setting.location.PosYmf262[chipID].Y;
            }

            frmYMF262[chipID].Show();
            frmYMF262[chipID].update();
            frmYMF262[chipID].Text = string.Format("YMF262 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ymf262[chipID] = new MDChipParams.YMF262();

            CheckAndSetForm(frmYMF262[chipID]);
        }

        private void CloseFormYMF262(int chipID)
        {
            if (frmYMF262[chipID] == null) return;

            try
            {
                frmYMF262[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYMF262[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYMF262[chipID] = null;
        }

        private void OpenFormYMF278B(int chipID, bool force = false)
        {
            if (frmYMF278B[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormYMF278B(chipID);
                    return;
                }
                else return;
            }

            frmYMF278B[chipID] = new frmYMF278B(this, chipID, setting.other.Zoom, newParam.ymf278b[chipID]);

            if (setting.location.PosYmf278b[chipID] == System.Drawing.Point.Empty)
            {
                frmYMF278B[chipID].x = this.Location.X;
                frmYMF278B[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmYMF278B[chipID].x = setting.location.PosYmf278b[chipID].X;
                frmYMF278B[chipID].y = setting.location.PosYmf278b[chipID].Y;
            }

            frmYMF278B[chipID].Show();
            frmYMF278B[chipID].update();
            frmYMF278B[chipID].Text = string.Format("YMF278B ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ymf278b[chipID] = new MDChipParams.YMF278B();

            CheckAndSetForm(frmYMF278B[chipID]);
        }

        private void CloseFormYMF278B(int chipID)
        {
            if (frmYMF278B[chipID] == null) return;

            try
            {
                frmYMF278B[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmYMF278B[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmYMF278B[chipID] = null;
        }

        private void OpenFormMIDI(int chipID, bool force = false)
        {
            if (frmMIDI[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormMIDI(chipID);
                    return;
                }
                else return;
            }

            frmMIDI[chipID] = new frmMIDI(this, chipID, setting.other.Zoom, newParam.midi[chipID]);

            if (setting.location.PosMIDI[chipID] == System.Drawing.Point.Empty)
            {
                frmMIDI[chipID].x = this.Location.X;
                frmMIDI[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmMIDI[chipID].x = setting.location.PosMIDI[chipID].X;
                frmMIDI[chipID].y = setting.location.PosMIDI[chipID].Y;
            }

            frmMIDI[chipID].Show();
            frmMIDI[chipID].update();
            frmMIDI[chipID].Text = string.Format("MIDI ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.midi[chipID] = new MIDIParam();

            CheckAndSetForm(frmMIDI[chipID]);
        }

        private void CloseFormMIDI(int chipID)
        {
            if (frmMIDI[chipID] == null) return;

            try
            {
                frmMIDI[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmMIDI[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmMIDI[chipID] = null;
        }

        private void OpenFormNESDMC(int chipID, bool force = false)
        {
            if (frmNESDMC[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormNESDMC(chipID);
                    return;
                }
                else return;
            }

            frmNESDMC[chipID] = new frmNESDMC(this, chipID, setting.other.Zoom, newParam.nesdmc[chipID]);

            if (setting.location.PosNESDMC[chipID] == System.Drawing.Point.Empty)
            {
                frmNESDMC[chipID].x = this.Location.X;
                frmNESDMC[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmNESDMC[chipID].x = setting.location.PosNESDMC[chipID].X;
                frmNESDMC[chipID].y = setting.location.PosNESDMC[chipID].Y;
            }

            frmNESDMC[chipID].Show();
            frmNESDMC[chipID].update();
            frmNESDMC[chipID].Text = string.Format("NES&DMC ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.nesdmc[chipID] = new MDChipParams.NESDMC();

            CheckAndSetForm(frmNESDMC[chipID]);
        }

        private void CloseFormNESDMC(int chipID)
        {
            if (frmNESDMC[chipID] == null) return;

            try
            {
                frmNESDMC[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmNESDMC[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmNESDMC[chipID] = null;
        }

        private void OpenFormFDS(int chipID, bool force = false)
        {
            if (frmFDS[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormFDS(chipID);
                    return;
                }
                else return;
            }

            frmFDS[chipID] = new frmFDS(this, chipID, setting.other.Zoom, newParam.fds[chipID]);

            if (setting.location.PosFDS[chipID] == System.Drawing.Point.Empty)
            {
                frmFDS[chipID].x = this.Location.X;
                frmFDS[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmFDS[chipID].x = setting.location.PosFDS[chipID].X;
                frmFDS[chipID].y = setting.location.PosFDS[chipID].Y;
            }

            frmFDS[chipID].Show();
            frmFDS[chipID].update();
            frmFDS[chipID].Text = string.Format("FDS ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.fds[chipID] = new MDChipParams.FDS();

            CheckAndSetForm(frmFDS[chipID]);
        }

        private void CloseFormFDS(int chipID)
        {
            if (frmFDS[chipID] == null) return;

            try
            {
                frmFDS[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmFDS[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmFDS[chipID] = null;
        }

        private void OpenFormVRC6(int chipID, bool force = false)
        {
            if (frmVRC6[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormVRC6(chipID);
                    return;
                }
                else return;
            }

            oldParam.vrc6[chipID] = new MDChipParams.VRC6();
            for (int i = 0; i < oldParam.vrc6[chipID].channels.Length; i++)
            {
                oldParam.vrc6[chipID].channels[i].mask = null;
            }
            frmVRC6[chipID] = new frmVRC6(this, chipID, setting.other.Zoom, newParam.vrc6[chipID], oldParam.vrc6[chipID]);

            if (setting.location.PosVrc6[chipID] == System.Drawing.Point.Empty)
            {
                frmVRC6[chipID].x = this.Location.X;
                frmVRC6[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmVRC6[chipID].x = setting.location.PosVrc6[chipID].X;
                frmVRC6[chipID].y = setting.location.PosVrc6[chipID].Y;
            }

            frmVRC6[chipID].Show();
            frmVRC6[chipID].update();
            frmVRC6[chipID].Text = string.Format("VRC6 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.vrc6[chipID] = new MDChipParams.VRC6();

            CheckAndSetForm(frmVRC6[chipID]);
        }

        private void CloseFormVRC6(int chipID)
        {
            if (frmVRC6[chipID] == null) return;

            try
            {
                frmVRC6[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmVRC6[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmVRC6[chipID] = null;
        }

        private void OpenFormVRC7(int chipID, bool force = false)
        {
            if (frmVRC7[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormVRC7(chipID);
                    return;
                }
                else return;
            }

            frmVRC7[chipID] = new frmVRC7(this, chipID, setting.other.Zoom, newParam.vrc7[chipID]);

            if (setting.location.PosVrc7[chipID] == System.Drawing.Point.Empty)
            {
                frmVRC7[chipID].x = this.Location.X;
                frmVRC7[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmVRC7[chipID].x = setting.location.PosVrc7[chipID].X;
                frmVRC7[chipID].y = setting.location.PosVrc7[chipID].Y;
            }

            frmVRC7[chipID].Show();
            frmVRC7[chipID].update();
            frmVRC7[chipID].Text = string.Format("VRC7 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.vrc7[chipID] = new MDChipParams.VRC7();

            CheckAndSetForm(frmVRC7[chipID]);
        }

        private void CloseFormVRC7(int chipID)
        {
            if (frmVRC7[chipID] == null) return;

            try
            {
                frmVRC7[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmVRC7[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmVRC7[chipID] = null;
        }

        private void OpenFormMMC5(int chipID, bool force = false)
        {
            if (frmMMC5[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormMMC5(chipID);
                    return;
                }
                else return;
            }

            frmMMC5[chipID] = new frmMMC5(this, chipID, setting.other.Zoom, newParam.mmc5[chipID]);

            if (setting.location.PosMMC5[chipID] == System.Drawing.Point.Empty)
            {
                frmMMC5[chipID].x = this.Location.X;
                frmMMC5[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmMMC5[chipID].x = setting.location.PosMMC5[chipID].X;
                frmMMC5[chipID].y = setting.location.PosMMC5[chipID].Y;
            }

            frmMMC5[chipID].Show();
            frmMMC5[chipID].update();
            frmMMC5[chipID].Text = string.Format("MMC5 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.mmc5[chipID] = new MDChipParams.MMC5();

            CheckAndSetForm(frmMMC5[chipID]);
        }

        private void CloseFormMMC5(int chipID)
        {
            if (frmMMC5[chipID] == null) return;

            try
            {
                frmMMC5[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmMMC5[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmMMC5[chipID] = null;
        }

        private void OpenFormRegTest(int chipID, EnmChip selectedChip = EnmChip.Unuse, bool force = false)
        {
            if (frmRegTest != null)
            {
                frmRegTest.changeChip(selectedChip);
                return;
            }

            frmRegTest = new frmRegTest(this, chipID, selectedChip, setting.other.Zoom);

            if (setting.location.PosRegTest[chipID] == System.Drawing.Point.Empty)
            {
                frmRegTest.x = this.Location.X;
                frmRegTest.y = this.Location.Y + 264;
            }
            else
            {
                frmRegTest.x = setting.location.PosRegTest[chipID].X;
                frmRegTest.y = setting.location.PosRegTest[chipID].Y;
            }

            frmRegTest.Show();
            frmRegTest.update();
            frmRegTest.changeChip(selectedChip);
            frmRegTest.Text = string.Format("RegTest ({0})", chipID == 0 ? "Primary" : "Secondary");

            CheckAndSetForm(frmRegTest);
        }

        private void OpenFormVisWave()
        {
            if (frmVisWave != null && !frmVisWave.isClosed)
            {
                frmVisWave.Focus();
                return;
            }

            frmVisWave = new frmVisWave(this);

            if (setting.location.PosVisWave == System.Drawing.Point.Empty)
            {
                frmVisWave.x = this.Location.X;
                frmVisWave.y = this.Location.Y + 264;
            }
            else
            {
                frmVisWave.x = setting.location.PosVisWave.X;
                frmVisWave.y = setting.location.PosVisWave.Y;
            }

            frmVisWave.Show();

            CheckAndSetForm(frmVisWave);
        }

        private void OpenFormN106(int chipID, bool force = false)
        {
            if (frmN106[chipID] != null)// && frmInfo.isClosed)
            {
                if (!force)
                {
                    CloseFormN106(chipID);
                    return;
                }
                else return;
            }

            oldParam.n106[chipID] = new MDChipParams.N106();
            for (int i = 0; i < oldParam.n106[chipID].channels.Length; i++)
            {
                oldParam.n106[chipID].channels[i].mask = null;
            }
            frmN106[chipID] = new frmN106(this, chipID, setting.other.Zoom, newParam.n106[chipID], oldParam.n106[chipID]);

            if (setting.location.PosN106[chipID] == System.Drawing.Point.Empty)
            {
                frmN106[chipID].x = this.Location.X;
                frmN106[chipID].y = this.Location.Y + 264;
            }
            else
            {
                frmN106[chipID].x = setting.location.PosN106[chipID].X;
                frmN106[chipID].y = setting.location.PosN106[chipID].Y;
            }

            frmN106[chipID].Show();
            frmN106[chipID].update();
            frmN106[chipID].Text = string.Format("N163(N106) ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.n106[chipID] = new MDChipParams.N106();

            CheckAndSetForm(frmN106[chipID]);
        }

        private void CloseFormN106(int chipID)
        {
            if (frmN106[chipID] == null) return;

            try
            {
                frmN106[chipID].Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmN106[chipID].Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmN106[chipID] = null;
        }




        private void CloseFormRegTest(int chipID)
        {
            if (frmRegTest == null) return;

            try
            {
                frmRegTest.Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmRegTest.Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmRegTest = null;
        }

        private void CloseFormVisWave()
        {
            if (frmVisWave == null) return;

            try
            {
                frmVisWave.Close();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            try
            {
                frmVisWave.Dispose();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            frmVisWave = null;
        }


        private void openInfo()
        {
            if (frmInfo != null && !frmInfo.isClosed)
            {
                try
                {
                    frmInfo.Close();
                    frmInfo.Dispose();
                }
                catch { }
                finally
                {
                    frmInfo = null;
                }
                return;
            }

            if (frmInfo != null)
            {
                try
                {
                    frmInfo.Close();
                    frmInfo.Dispose();
                }
                catch { }
                finally
                {
                    frmInfo = null;
                }
            }

            frmInfo = new frmInfo(this);
            if (setting.location.PInfo == System.Drawing.Point.Empty)
            {
                frmInfo.x = this.Location.X + 328;
                frmInfo.y = this.Location.Y;
            }
            else
            {
                frmInfo.x = setting.location.PInfo.X;
                frmInfo.y = setting.location.PInfo.Y;
            }

            Screen s = Screen.FromControl(frmInfo);
            Rectangle rc = new(frmInfo.Location, frmInfo.Size);
            if (s.WorkingArea.Contains(rc))
            {
                frmInfo.Location = rc.Location;
                frmInfo.Size = rc.Size;
            }
            else
            {
                frmInfo.Location = new System.Drawing.Point(100, 100);
            }

            frmInfo.setting = setting;
            frmInfo.Show();
            frmInfo.UpdateInfo();
        }

        private void openMIDIKeyboard()
        {
            if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed)
            {
                try
                {
                    frmYM2612MIDI.Close();
                    frmYM2612MIDI.Dispose();
                }
                catch { }
                finally
                {
                    frmYM2612MIDI = null;
                }
                return;
            }

            if (frmYM2612MIDI != null)
            {
                try
                {
                    frmYM2612MIDI.Close();
                    frmYM2612MIDI.Dispose();
                }
                catch { }
                finally
                {
                    frmYM2612MIDI = null;
                }
            }

            frmYM2612MIDI = new frmYM2612MIDI(this, setting.other.Zoom, newParam.ym2612Midi);
            if (setting.location.PosYm2612MIDI == System.Drawing.Point.Empty)
            {
                frmYM2612MIDI.x = this.Location.X + 328;
                frmYM2612MIDI.y = this.Location.Y;
            }
            else
            {
                frmYM2612MIDI.x = setting.location.PosYm2612MIDI.X;
                frmYM2612MIDI.y = setting.location.PosYm2612MIDI.Y;
            }

            Screen s = Screen.FromControl(frmYM2612MIDI);
            Rectangle rc = new(frmYM2612MIDI.Location, frmYM2612MIDI.Size);
            if (s.WorkingArea.Contains(rc))
            {
                frmYM2612MIDI.Location = rc.Location;
                frmYM2612MIDI.Size = rc.Size;
            }
            else
            {
                frmYM2612MIDI.Location = new System.Drawing.Point(100, 100);
            }

            //frmYM2612MIDI.setting = setting;
            frmYM2612MIDI.Show();
            frmYM2612MIDI.update();
            oldParam.ym2612Midi = new MDChipParams.YM2612MIDI();
        }

        private void openSetting()
        {
            frmSetting frm = new(setting);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                flgReinit = true;
                Reinit(frm.setting);

            }
        }

        private void Reinit(Setting setting)
        {
            if (!flgReinit) return;

            StopMIDIInMonitoring();
            frmPlayList.Stop();

            Request req = new(EnmRequest.Stop);
            OpeManager.RequestToAudio(req);
            while (!req.End) System.Threading.Thread.Sleep(1);

            //req = new Request(enmRequest.Die);
            //OpeManager.RequestToAudio(req);
            //while (!req.end) System.Threading.Thread.Sleep(1);

            Audio.Stop();
            Audio.Close(false);

            this.setting = setting;
            this.setting.Save();

            screen.setting = this.setting;
            frmPlayList.setting = this.setting;
            //oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            reqAllScreenInit = true;
            //screen.screenInitAll();

            log.ForcedWrite("設定が変更されたため、再度Audio初期化処理開始");

            Audio.Init(this.setting);

            log.ForcedWrite("Audio初期化処理完了");
            log.debug = this.setting.debug.logDebug;
            log.logLevel = this.setting.debug.logLevel;

            frmVSTeffectList.dispPluginList();
            StartMIDIInMonitoring();

            IsInitialOpenFolder = true;
            flgReinit = false;

            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }

        }

        private void openMixer()
        {
            if (frmMixer2 != null && !frmMixer2.isClosed)
            {
                try
                {
                    frmMixer2.Close();
                    frmMixer2.Dispose();
                }
                catch
                {
                }
                finally
                {
                    frmMixer2 = null;
                }
                return;
            }

            if (frmMixer2 != null)
            {
                try
                {
                    frmMixer2.Close();
                    frmMixer2.Dispose();
                }
                catch
                {
                }
                finally
                {
                    frmMixer2 = null;
                }
            }

            frmMixer2 = new frmMixer2(this, setting.other.Zoom, newParam.mixer);
            if (setting.location.PosMixer == System.Drawing.Point.Empty)
            {
                frmMixer2.x = this.Location.X + 328;
                frmMixer2.y = this.Location.Y;
            }
            else
            {
                frmMixer2.x = setting.location.PosMixer.X;
                frmMixer2.y = setting.location.PosMixer.Y;
            }

            Screen s = Screen.FromControl(frmMixer2);
            Rectangle rc = new(frmMixer2.Location, frmMixer2.Size);
            if (s.WorkingArea.Contains(rc))
            {
                frmMixer2.Location = rc.Location;
                frmMixer2.Size = rc.Size;
            }
            else
            {
                frmMixer2.Location = new System.Drawing.Point(100, 100);
            }

            //frmMixer.setting = setting;
            //screen.AddMixer(frmMixer2.pbScreen, Properties.Resources.planeMixer);
            frmMixer2.Show();
            frmMixer2.update();
            //screen.screenInitMixer();
            oldParam.mixer = new MDChipParams.Mixer();
        }



        private void pbScreen_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;

        }

        private void pbScreen_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string filename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

                try
                {

                    //曲を停止
                    frmPlayList.Stop();
                    this.Stop();
                    while (!Audio.IsStopped)
                        Application.DoEvents();

                    frmPlayList.getPlayList().AddFile(filename);
                    //frmPlayList.AddList(filename);

                    if (filename.ToLower().LastIndexOf(".zip") == -1)
                    {
                        loadAndPlay(0, 0, filename);
                        frmPlayList.setStart(-1);
                        oldParam = new MDChipParams();

                        frmPlayList.Play();
                    }
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    MessageBox.Show("ファイルの読み込みに失敗しました。");
                }
            }
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }


        private void allScreenInit()
        {
            //oldParam = new MDChipParams();
            DrawBuff.drawTimer(screen.mainScreen, 0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);
            screenInit(null);

            for (int i = 0; i < 2; i++)
            {
                frmAY8910[i]?.screenInit();
                //frmC140[i]?.screenInit();
                //frmC352[i]?.screenInit();
                frmFDS[i]?.screenInit();
                //frmHuC6280[i]?.screenInit();
                frmK051649[i]?.screenInit();
                frmK053260[i]?.screenInit();
                frmMCD[i]?.screenInit();
                frmMIDI[i]?.screenInit();
                frmMMC5[i]?.screenInit();
                frmNESDMC[i]?.screenInit();
                frmOKIM6258[i]?.screenInit();
                frmOKIM6295[i]?.screenInit();
                frmPCM8[i]?.screenInit();
                //frmSegaPCM[i]?.screenInit();
                frmSN76489[i]?.ScreenInit();
                //frmYM2151[i]?.screenInit();
                //frmYM2203[i]?.screenInit();
                frmYM2413[i]?.screenInit();
                //frmYM2608[i]?.screenInit();
                //frmYM2610[i]?.screenInit();
                //frmYM2612[i]?.screenInit();
                frmYM3526[i]?.screenInit();
                frmY8950[i]?.screenInit();
                frmYM3812[i]?.screenInit();
                frmYMF262[i]?.screenInit();
                frmYMF278B[i]?.screenInit();
                frmVRC7[i]?.screenInit();
            }

            frmMixer2?.screenInit();
            frmInfo?.ScreenInit();
            //frmYM2612MIDI?.screenInit();
            frmRegTest?.screenInit();

            reqAllScreenInit = false;
        }


        /// <summary>
        /// ！！このメソッドはメインスレッドで動いていません！！
        /// </summary>
        private void screenMainLoop()
        {
            double nextFrame = (double)System.Environment.TickCount;
            isRunning = true;
            stopped = false;

            try
            {
                while (isRunning)
                {

                    if (reqAllScreenInit)
                    {
                        allScreenInit();
                    }

                    float period = 1000f / (float)setting.other.ScreenFrameRate;
                    double tickCount = (double)System.Environment.TickCount;

                    if (tickCount < nextFrame)
                    {
                        if (nextFrame - tickCount > 1)
                        {
                            System.Threading.Thread.Sleep((int)(nextFrame - tickCount));
                        }
                        continue;
                    }


                    screenChangeParams();
                    screenChangeParamsForms();


                    if ((double)System.Environment.TickCount >= nextFrame + period)
                    {
                        nextFrame += period;
                        continue;
                    }


                    this.Invoke((Action)(screenDrawParams));
                    this.Invoke((Action)(screenDrawParamsForms));


                    nextFrame += period;

                    if (frmPlayList.isPlaying())
                    {
                        if ((setting.other.UseLoopTimes && Audio.GetVgmCurLoopCounter() > setting.other.LoopTimes - 1)
                            || Audio.GetVGMStopped())
                        {
                            Fadeout();
                        }
                        if (Audio.Stopped && frmPlayList.isPlaying())
                        {
                            NextPlayMode();
                        }
                    }

                    //remote対応
                    string msg = mmf.GetMessage();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (msg.Trim().ToUpper() == "CLOSE")
                        {
                            this.BeginInvoke((Action)Close);
                        }
                        else
                        {
                            this.Invoke(//Asyncしないこと
                                (Action<string>)Remote
                                //, new object[] { lin }//配列が引数の場合はこの様に指定する必要あり
                                , msg
                                );
                        }
                    }

                    if (Audio.FatalError)
                    {
                        log.ForcedWrite("AudioでFatalErrorが発生。再度Audio初期化処理開始");

                        frmPlayList.Stop();
                        try
                        {
                            Request req = new(EnmRequest.Stop);
                            OpeManager.RequestToAudio(req);
                            while (!req.End) System.Threading.Thread.Sleep(1);
                            //Audio.Stop();
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                        }

                        try { Audio.Close(); }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                        }

                        Audio.FatalError = false;
                        Audio.Init(setting);

                        log.ForcedWrite("Audio初期化処理完了");
                    }
                }
            }
            catch { }

            stopped = true;

        }

        private void screenChangeParams()
        {

            long w = Audio.GetCounter();
            double sec = (double)w / (double)Common.VGMProcSampleRate;// setting.outputDevice.SampleRate;
            newParam.Cminutes = (int)(sec / 60);
            sec -= newParam.Cminutes * 60;
            newParam.Csecond = (int)sec;
            sec -= newParam.Csecond;
            newParam.Cmillisecond = (int)(sec * 100.0);

            w = Audio.GetTotalCounter();
            sec = (double)w / (double)Common.VGMProcSampleRate;//setting.outputDevice.SampleRate;
            newParam.TCminutes = (int)(sec / 60);
            sec -= newParam.TCminutes * 60;
            newParam.TCsecond = (int)sec;
            sec -= newParam.TCsecond;
            newParam.TCmillisecond = (int)(sec * 100.0);

            w = Audio.GetLoopCounter();
            sec = (double)w / (double)Common.VGMProcSampleRate;//setting.outputDevice.SampleRate;
            newParam.LCminutes = (int)(sec / 60);
            sec -= newParam.LCminutes * 60;
            newParam.LCsecond = (int)sec;
            sec -= newParam.LCsecond;
            newParam.LCmillisecond = (int)(sec * 100.0);

            UpdateOpeButtonActiveState();

        }

        private void screenChangeParamsForms()
        {
            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed) frmMCD[chipID].screenChangeParams();
                else frmMCD[chipID] = null;

                if (frmRf5c68[chipID] != null && !frmRf5c68[chipID].isClosed) frmRf5c68[chipID].screenChangeParams();
                else frmRf5c68[chipID] = null;

                if (frmC140[chipID] != null && !frmC140[chipID].isClosed) frmC140[chipID].screenChangeParams();
                else frmC140[chipID] = null;

                if (frmS5B[chipID] != null && !frmS5B[chipID].isClosed) frmS5B[chipID].screenChangeParams();
                else frmS5B[chipID] = null;

                if (frmDMG[chipID] != null && !frmDMG[chipID].isClosed) frmDMG[chipID].screenChangeParams();
                else frmDMG[chipID] = null;

                if (frmPPZ8[chipID] != null && !frmPPZ8[chipID].isClosed) frmPPZ8[chipID].screenChangeParams();
                else frmPPZ8[chipID] = null;

                if (frmYMZ280B[chipID] != null && !frmYMZ280B[chipID].isClosed) frmYMZ280B[chipID].screenChangeParams();
                else frmYMZ280B[chipID] = null;

                if (frmC352[chipID] != null && !frmC352[chipID].isClosed) frmC352[chipID].screenChangeParams();
                else frmC352[chipID] = null;

                if (frmMultiPCM[chipID] != null && !frmMultiPCM[chipID].isClosed) frmMultiPCM[chipID].screenChangeParams();
                else frmMultiPCM[chipID] = null;

                if (frmQSound[chipID] != null && !frmQSound[chipID].isClosed) frmQSound[chipID].screenChangeParams();
                else frmQSound[chipID] = null;

                if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed) frmYM2608[chipID].screenChangeParams();
                else frmYM2608[chipID] = null;

                if (frmYM2609[chipID] != null && !frmYM2609[chipID].isClosed) frmYM2609[chipID].screenChangeParams();
                else frmYM2609[chipID] = null;

                if (frmYM2151[chipID] != null && !frmYM2151[chipID].isClosed) frmYM2151[chipID].screenChangeParams();
                else frmYM2151[chipID] = null;

                if (frmYM2203[chipID] != null && !frmYM2203[chipID].isClosed) frmYM2203[chipID].screenChangeParams();
                else frmYM2203[chipID] = null;

                if (frmYM2413[chipID] != null && !frmYM2413[chipID].isClosed) frmYM2413[chipID].screenChangeParams();
                else frmYM2413[chipID] = null;

                if (frmYM2610[chipID] != null && !frmYM2610[chipID].isClosed) frmYM2610[chipID].screenChangeParams();
                else frmYM2610[chipID] = null;

                if (frmYM2612[chipID] != null && !frmYM2612[chipID].isClosed) frmYM2612[chipID].screenChangeParams();
                else frmYM2612[chipID] = null;

                if (frmYM3526[chipID] != null && !frmYM3526[chipID].isClosed) frmYM3526[chipID].screenChangeParams();
                else frmYM3526[chipID] = null;

                if (frmY8950[chipID] != null && !frmY8950[chipID].isClosed) frmY8950[chipID].screenChangeParams();
                else frmY8950[chipID] = null;

                if (frmYM3812[chipID] != null && !frmYM3812[chipID].isClosed) frmYM3812[chipID].screenChangeParams();
                else frmYM3812[chipID] = null;

                if (frmYMF262[chipID] != null && !frmYMF262[chipID].isClosed) frmYMF262[chipID].screenChangeParams();
                else frmYMF262[chipID] = null;

                if (frmYMF271[chipID] != null && !frmYMF271[chipID].isClosed) frmYMF271[chipID].screenChangeParams();
                else frmYMF271[chipID] = null;

                if (frmYMF278B[chipID] != null && !frmYMF278B[chipID].isClosed) frmYMF278B[chipID].screenChangeParams();
                else frmYMF278B[chipID] = null;

                if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed) frmOKIM6258[chipID].screenChangeParams();
                else frmOKIM6258[chipID] = null;

                if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed) frmOKIM6295[chipID].screenChangeParams();
                else frmOKIM6295[chipID] = null;

                if (frmPCM8[chipID] != null && !frmPCM8[chipID].isClosed) frmPCM8[chipID].screenChangeParams();
                else frmPCM8[chipID] = null;

                if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed) frmSN76489[chipID].ScreenChangeParams();
                else frmSN76489[chipID] = null;

                if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed) frmSegaPCM[chipID].screenChangeParams();
                else frmSegaPCM[chipID] = null;

                if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed)
                {
                    frmAY8910[chipID].screenChangeParams();
                }
                else frmAY8910[chipID] = null;

                if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed) frmHuC6280[chipID].screenChangeParams();
                else frmHuC6280[chipID] = null;

                if (frmK051649[chipID] != null && !frmK051649[chipID].isClosed) frmK051649[chipID].screenChangeParams();
                else frmK051649[chipID] = null;

                if (frmK053260[chipID] != null && !frmK053260[chipID].isClosed) frmK053260[chipID].screenChangeParams();
                else frmK053260[chipID] = null;

                if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed) frmMIDI[chipID].screenChangeParams();
                else frmMIDI[chipID] = null;

                if (frmNESDMC[chipID] != null && !frmNESDMC[chipID].isClosed) frmNESDMC[chipID].screenChangeParams();
                else frmNESDMC[chipID] = null;

                if (frmFDS[chipID] != null && !frmFDS[chipID].isClosed) frmFDS[chipID].screenChangeParams();
                else frmFDS[chipID] = null;

                if (frmMMC5[chipID] != null && !frmMMC5[chipID].isClosed) frmMMC5[chipID].screenChangeParams();
                else frmMMC5[chipID] = null;

                if (frmVRC6[chipID] != null && !frmVRC6[chipID].isClosed) frmVRC6[chipID].screenChangeParams();
                else frmVRC6[chipID] = null;

                if (frmVRC7[chipID] != null && !frmVRC7[chipID].isClosed) frmVRC7[chipID].screenChangeParams();
                else frmVRC7[chipID] = null;

                if (frmN106[chipID] != null && !frmN106[chipID].isClosed) frmN106[chipID].screenChangeParams();
                else frmN106[chipID] = null;

            }
            if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed) frmYM2612MIDI.screenChangeParams();
            else frmYM2612MIDI = null;
            if (frmMixer2 != null && !frmMixer2.isClosed) frmMixer2.screenChangeParams();
            else frmMixer2 = null;

            if (frmRegTest != null && !frmRegTest.isClosed) frmRegTest.screenChangeParams();
            else frmRegTest = null;

        }

        private void screenDrawParams()
        {
            // 描画

            for (int i = 0; i < lstOpeButtonActive.Length; i++)
            {
                if (lstOpeButtonActive[i] != lstOpeButtonActiveOld[i])
                {
                    lstOpeButtonActiveOld[i] = lstOpeButtonActive[i];
                    RedrawButton(lstOpeButtonControl[i]
                        , setting.other.Zoom
                        , lstOpeButtonActive[i] ? lstOpeButtonActiveImage[i] : lstOpeButtonLeaveImage[i]
                        );
                }
            }

            DrawBuff.drawTimer(screen.mainScreen, 0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            //byte[] chips = Audio.GetChipStatus();
            //DrawBuff.drawChipName(screen.mainScreen, 14 * 4, 0 * 8, 0, ref oldParam.chipLED.PriOPN, chips[0]);
            //DrawBuff.drawChipName(screen.mainScreen, 18 * 4, 0 * 8, 1, ref oldParam.chipLED.PriOPN2, chips[1]);
            //DrawBuff.drawChipName(screen.mainScreen, 23 * 4, 0 * 8, 2, ref oldParam.chipLED.PriOPNA, chips[2]);
            //DrawBuff.drawChipName(screen.mainScreen, 28 * 4, 0 * 8, 3, ref oldParam.chipLED.PriOPNB, chips[3]);
            //DrawBuff.drawChipName(screen.mainScreen, 33 * 4, 0 * 8, 4, ref oldParam.chipLED.PriOPM, chips[4]);
            //DrawBuff.drawChipName(screen.mainScreen, 37 * 4, 0 * 8, 5, ref oldParam.chipLED.PriDCSG, chips[5]);
            //DrawBuff.drawChipName(screen.mainScreen, 42 * 4, 0 * 8, 6, ref oldParam.chipLED.PriRF5C, chips[6]);
            //DrawBuff.drawChipName(screen.mainScreen, 47 * 4, 0 * 8, 7, ref oldParam.chipLED.PriPWM, chips[7]);
            //DrawBuff.drawChipName(screen.mainScreen, 51 * 4, 0 * 8, 8, ref oldParam.chipLED.PriOKI5, chips[8]);
            //DrawBuff.drawChipName(screen.mainScreen, 56 * 4, 0 * 8, 9, ref oldParam.chipLED.PriOKI9, chips[9]);
            //DrawBuff.drawChipName(screen.mainScreen, 61 * 4, 0 * 8, 10, ref oldParam.chipLED.PriC140, chips[10]);
            //DrawBuff.drawChipName(screen.mainScreen, 66 * 4, 0 * 8, 11, ref oldParam.chipLED.PriSPCM, chips[11]);
            //DrawBuff.drawChipName(screen.mainScreen, 4 * 4, 0 * 8, 12, ref oldParam.chipLED.PriAY10, chips[12]);
            //DrawBuff.drawChipName(screen.mainScreen, 9 * 4, 0 * 8, 13, ref oldParam.chipLED.PriOPLL, chips[13]);
            //DrawBuff.drawChipName(screen.mainScreen, 71 * 4, 0 * 8, 14, ref oldParam.chipLED.PriHuC8, chips[14]);

            //DrawBuff.drawChipName(screen.mainScreen, 14 * 4, 1 * 8, 0, ref oldParam.chipLED.SecOPN, chips[128 + 0]);
            //DrawBuff.drawChipName(screen.mainScreen, 18 * 4, 1 * 8, 1, ref oldParam.chipLED.SecOPN2, chips[128 + 1]);
            //DrawBuff.drawChipName(screen.mainScreen, 23 * 4, 1 * 8, 2, ref oldParam.chipLED.SecOPNA, chips[128 + 2]);
            //DrawBuff.drawChipName(screen.mainScreen, 28 * 4, 1 * 8, 3, ref oldParam.chipLED.SecOPNB, chips[128 + 3]);
            //DrawBuff.drawChipName(screen.mainScreen, 33 * 4, 1 * 8, 4, ref oldParam.chipLED.SecOPM, chips[128 + 4]);
            //DrawBuff.drawChipName(screen.mainScreen, 37 * 4, 1 * 8, 5, ref oldParam.chipLED.SecDCSG, chips[128 + 5]);
            //DrawBuff.drawChipName(screen.mainScreen, 42 * 4, 1 * 8, 6, ref oldParam.chipLED.SecRF5C, chips[128 + 6]);
            //DrawBuff.drawChipName(screen.mainScreen, 47 * 4, 1 * 8, 7, ref oldParam.chipLED.SecPWM, chips[128 + 7]);
            //DrawBuff.drawChipName(screen.mainScreen, 51 * 4, 1 * 8, 8, ref oldParam.chipLED.SecOKI5, chips[128 + 8]);
            //DrawBuff.drawChipName(screen.mainScreen, 56 * 4, 1 * 8, 9, ref oldParam.chipLED.SecOKI9, chips[128 + 9]);
            //DrawBuff.drawChipName(screen.mainScreen, 61 * 4, 1 * 8, 10, ref oldParam.chipLED.SecC140, chips[128 + 10]);
            //DrawBuff.drawChipName(screen.mainScreen, 66 * 4, 1 * 8, 11, ref oldParam.chipLED.SecSPCM, chips[128 + 11]);
            //DrawBuff.drawChipName(screen.mainScreen, 4 * 4, 1 * 8, 12, ref oldParam.chipLED.SecAY10, chips[128 + 12]);
            //DrawBuff.drawChipName(screen.mainScreen, 9 * 4, 1 * 8, 13, ref oldParam.chipLED.SecOPLL, chips[128 + 13]);
            //DrawBuff.drawChipName(screen.mainScreen, 71 * 4, 0 * 8, 14, ref oldParam.chipLED.SecHuC8, chips[128 + 14]);

            DrawBuff.drawFont4(screen.mainScreen, 1, 9, 1, Audio.GetIsDataBlock(EnmModel.VirtualModel) ? "VD" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 321 - 16, 9, 1, Audio.GetIsPcmRAMWrite(EnmModel.VirtualModel) ? "VP" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 1, 17, 1, Audio.GetIsDataBlock(EnmModel.RealModel) ? "RD" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 321 - 16, 17, 1, Audio.GetIsPcmRAMWrite(EnmModel.RealModel) ? "RP" : "  ");

            if (setting.debug.DispFrameCounter)
            {
                long v = Audio.GetVirtualFrameCounter();
                if (v != -1) DrawBuff.drawFont8(screen.mainScreen, 0, 0, 0, string.Format("EMU        : {0:D12} ", v));
                long r = Audio.GetRealFrameCounter();
                if (r != -1) DrawBuff.drawFont8(screen.mainScreen, 0, 8, 0, string.Format("REAL CHIP  : {0:D12} ", r));
                long d = r - v;
                if (r != -1 && v != -1) DrawBuff.drawFont8(screen.mainScreen, 0, 16, 0, string.Format("R.CHIP-EMU : {0:D12} ", d));
                DrawBuff.drawFont8(screen.mainScreen, 0, 24, 0, string.Format("PROC TIME  : {0:D12} ", Audio.ProcTimePer1Frame));
            }

            screen.Refresh();

            Audio.UpdateVol();

            string newInfo;
            GD3 gd3 = Audio.GetGD3();
            if (gd3 != null)
            {
                string title = Common.EscSeqFilter(gd3.TrackName);
                string usedChips = Common.EscSeqFilter(gd3.UsedChips);
                newInfo = string.Format("MDPlayer - [{0}] {1}", usedChips, title);
            }
            else
            {
                newInfo = "MDPlayer";
            }

            try
            {
                this.Invoke((Action<string>)SetTitle, newInfo);
            }
            catch { }
        }

        private void screenDrawParamsForms()
        {
            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed) { frmMCD[chipID].screenDrawParams(); frmMCD[chipID].update(); }
                else frmMCD[chipID] = null;

                if (frmRf5c68[chipID] != null && !frmRf5c68[chipID].isClosed) { frmRf5c68[chipID].screenDrawParams(); frmRf5c68[chipID].update(); }
                else frmRf5c68[chipID] = null;

                if (frmC140[chipID] != null && !frmC140[chipID].isClosed) { frmC140[chipID].screenDrawParams(); frmC140[chipID].update(); }
                else frmC140[chipID] = null;

                if (frmPPZ8[chipID] != null && !frmPPZ8[chipID].isClosed) { frmPPZ8[chipID].screenDrawParams(); frmPPZ8[chipID].update(); }
                else frmPPZ8[chipID] = null;

                if (frmS5B[chipID] != null && !frmS5B[chipID].isClosed) { frmS5B[chipID].screenDrawParams(); frmS5B[chipID].update(); }
                else frmS5B[chipID] = null;

                if (frmDMG[chipID] != null && !frmDMG[chipID].isClosed) { frmDMG[chipID].screenDrawParams(); frmDMG[chipID].update(); }
                else frmDMG[chipID] = null;

                if (frmYMZ280B[chipID] != null && !frmYMZ280B[chipID].isClosed) { frmYMZ280B[chipID].screenDrawParams(); frmYMZ280B[chipID].update(); }
                else frmYMZ280B[chipID] = null;

                if (frmC352[chipID] != null && !frmC352[chipID].isClosed) { frmC352[chipID].screenDrawParams(); frmC352[chipID].update(); }
                else frmC352[chipID] = null;

                if (frmMultiPCM[chipID] != null && !frmMultiPCM[chipID].isClosed) { frmMultiPCM[chipID].screenDrawParams(); frmMultiPCM[chipID].update(); }
                else frmMultiPCM[chipID] = null;

                if (frmQSound[chipID] != null && !frmQSound[chipID].isClosed) { frmQSound[chipID].screenDrawParams(); frmQSound[chipID].update(); }
                else frmQSound[chipID] = null;

                if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed) { frmYM2608[chipID].screenDrawParams(); frmYM2608[chipID].update(); }
                else frmYM2608[chipID] = null;

                if (frmYM2609[chipID] != null && !frmYM2609[chipID].isClosed) { frmYM2609[chipID].screenDrawParams(); frmYM2609[chipID].update(); }
                else frmYM2609[chipID] = null;

                if (frmYM2151[chipID] != null && !frmYM2151[chipID].isClosed) { frmYM2151[chipID].screenDrawParams(); frmYM2151[chipID].update(); }
                else frmYM2151[chipID] = null;

                if (frmYM2203[chipID] != null && !frmYM2203[chipID].isClosed) { frmYM2203[chipID].screenDrawParams(); frmYM2203[chipID].update(); }
                else frmYM2203[chipID] = null;

                if (frmYM2413[chipID] != null && !frmYM2413[chipID].isClosed) { frmYM2413[chipID].screenDrawParams(); frmYM2413[chipID].update(); }
                else frmYM2413[chipID] = null;

                if (frmYM2610[chipID] != null && !frmYM2610[chipID].isClosed) { frmYM2610[chipID].screenDrawParams(); frmYM2610[chipID].update(); }
                else frmYM2610[chipID] = null;

                if (frmYM2612[chipID] != null && !frmYM2612[chipID].isClosed) { frmYM2612[chipID].screenDrawParams(); frmYM2612[chipID].update(); }
                else frmYM2612[chipID] = null;

                if (frmYM3526[chipID] != null && !frmYM3526[chipID].isClosed) { frmYM3526[chipID].screenDrawParams(); frmYM3526[chipID].update(); }
                else frmYM3526[chipID] = null;

                if (frmY8950[chipID] != null && !frmY8950[chipID].isClosed) { frmY8950[chipID].screenDrawParams(); frmY8950[chipID].update(); }
                else frmY8950[chipID] = null;

                if (frmYM3812[chipID] != null && !frmYM3812[chipID].isClosed) { frmYM3812[chipID].screenDrawParams(); frmYM3812[chipID].update(); }
                else frmYM3812[chipID] = null;

                if (frmYMF262[chipID] != null && !frmYMF262[chipID].isClosed) { frmYMF262[chipID].screenDrawParams(); frmYMF262[chipID].update(); }
                else frmYMF262[chipID] = null;

                if (frmYMF271[chipID] != null && !frmYMF271[chipID].isClosed) { frmYMF271[chipID].screenDrawParams(); frmYMF271[chipID].update(); }
                else frmYMF271[chipID] = null;

                if (frmYMF278B[chipID] != null && !frmYMF278B[chipID].isClosed) { frmYMF278B[chipID].screenDrawParams(); frmYMF278B[chipID].update(); }
                else frmYMF278B[chipID] = null;

                if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed) { frmOKIM6258[chipID].screenDrawParams(); frmOKIM6258[chipID].update(); }
                else frmOKIM6258[chipID] = null;

                if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed) { frmOKIM6295[chipID].screenDrawParams(); frmOKIM6295[chipID].update(); }
                else frmOKIM6295[chipID] = null;

                if (frmPCM8[chipID] != null && !frmPCM8[chipID].isClosed) { frmPCM8[chipID].screenDrawParams(); frmPCM8[chipID].update(); }
                else frmPCM8[chipID] = null;

                if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed) { frmSN76489[chipID].ScreenDrawParams(); frmSN76489[chipID].update(); }
                else frmSN76489[chipID] = null;

                if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed) { frmSegaPCM[chipID].screenDrawParams(); frmSegaPCM[chipID].update(); }
                else frmSegaPCM[chipID] = null;

                if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed) { frmAY8910[chipID].screenDrawParams(); frmAY8910[chipID].update(); }
                else frmAY8910[chipID] = null;

                if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed) { frmHuC6280[chipID].screenDrawParams(); frmHuC6280[chipID].update(); }
                else frmHuC6280[chipID] = null;

                if (frmK051649[chipID] != null && !frmK051649[chipID].isClosed) { frmK051649[chipID].screenDrawParams(); frmK051649[chipID].update(); }
                else frmK051649[chipID] = null;

                if (frmK053260[chipID] != null && !frmK053260[chipID].isClosed) { frmK053260[chipID].screenDrawParams(); frmK053260[chipID].update(); }
                else frmK053260[chipID] = null;

                if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed) { frmMIDI[chipID].screenDrawParams(); frmMIDI[chipID].update(); }
                else frmMIDI[chipID] = null;

                if (frmNESDMC[chipID] != null && !frmNESDMC[chipID].isClosed) { frmNESDMC[chipID].screenDrawParams(); frmNESDMC[chipID].update(); }
                else frmNESDMC[chipID] = null;

                if (frmVRC6[chipID] != null && !frmVRC6[chipID].isClosed) { frmVRC6[chipID].screenDrawParams(); frmVRC6[chipID].update(); }
                else frmVRC6[chipID] = null;

                if (frmVRC7[chipID] != null && !frmVRC7[chipID].isClosed) { frmVRC7[chipID].screenDrawParams(); frmVRC7[chipID].update(); }
                else frmVRC7[chipID] = null;

                if (frmFDS[chipID] != null && !frmFDS[chipID].isClosed) { frmFDS[chipID].screenDrawParams(); frmFDS[chipID].update(); }
                else frmFDS[chipID] = null;

                if (frmMMC5[chipID] != null && !frmMMC5[chipID].isClosed) { frmMMC5[chipID].screenDrawParams(); frmMMC5[chipID].update(); }
                else frmMMC5[chipID] = null;

                if (frmN106[chipID] != null && !frmN106[chipID].isClosed) { frmN106[chipID].screenDrawParams(); frmN106[chipID].update(); }
                else frmN106[chipID] = null;

            }
            if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed) { frmYM2612MIDI.screenDrawParams(); frmYM2612MIDI.update(); }
            else frmYM2612MIDI = null;
            if (frmMixer2 != null && !frmMixer2.isClosed) { frmMixer2.screenDrawParams(); frmMixer2.update(); }
            else frmMixer2 = null;

            if (frmRegTest != null && !frmRegTest.isClosed) { frmRegTest.screenDrawParams(); frmRegTest.update(); } else frmRegTest = null;

        }

        private void SetTitle(string newInfo)
        {
            if (this.Text != newInfo)
            {
                this.Text = newInfo;
            }
        }

        private void screenInit(object dmy)
        {

            oldParam.chipLED.PriOPN = 255;
            oldParam.chipLED.PriOPN2 = 255;
            oldParam.chipLED.PriOPNA = 255;
            oldParam.chipLED.PriOPNA2 = 255;
            oldParam.chipLED.PriOPNB = 255;
            oldParam.chipLED.PriOPM = 255;
            oldParam.chipLED.PriDCSG = 255;
            oldParam.chipLED.PriRF5C = 255;
            oldParam.chipLED.PriRF5C68 = 255;
            oldParam.chipLED.PriPWM = 255;
            oldParam.chipLED.PriOKI5 = 255;
            oldParam.chipLED.PriOKI9 = 255;
            oldParam.chipLED.PriPCM8 = 255;
            oldParam.chipLED.PriC140 = 255;
            oldParam.chipLED.PriSPCM = 255;
            oldParam.chipLED.PriAY10 = 255;
            oldParam.chipLED.PriOPLL = 255;
            oldParam.chipLED.PriHuC8 = 255;
            oldParam.chipLED.SecOPN = 255;
            oldParam.chipLED.SecOPN2 = 255;
            oldParam.chipLED.SecOPNA = 255;
            oldParam.chipLED.SecOPNA2 = 255;
            oldParam.chipLED.SecOPNB = 255;
            oldParam.chipLED.SecOPM = 255;
            oldParam.chipLED.SecDCSG = 255;
            oldParam.chipLED.SecRF5C = 255;
            oldParam.chipLED.SecRF5C68 = 255;
            oldParam.chipLED.SecPWM = 255;
            oldParam.chipLED.SecOKI5 = 255;
            oldParam.chipLED.SecOKI9 = 255;
            oldParam.chipLED.SecPCM8 = 255;
            oldParam.chipLED.SecC140 = 255;
            oldParam.chipLED.SecSPCM = 255;
            oldParam.chipLED.SecAY10 = 255;
            oldParam.chipLED.SecOPLL = 255;
            oldParam.chipLED.SecHuC8 = 255;

            //byte[] chips = Audio.GetChipStatus();
            //DrawBuff.drawChipName(screen.mainScreen, 14 * 4, 0 * 8, 0, ref oldParam.chipLED.PriOPN, chips[0]);
            //DrawBuff.drawChipName(screen.mainScreen, 18 * 4, 0 * 8, 1, ref oldParam.chipLED.PriOPN2, chips[1]);
            //DrawBuff.drawChipName(screen.mainScreen, 23 * 4, 0 * 8, 2, ref oldParam.chipLED.PriOPNA, chips[2]);
            //DrawBuff.drawChipName(screen.mainScreen, 28 * 4, 0 * 8, 3, ref oldParam.chipLED.PriOPNB, chips[3]);
            //DrawBuff.drawChipName(screen.mainScreen, 33 * 4, 0 * 8, 4, ref oldParam.chipLED.PriOPM, chips[4]);
            //DrawBuff.drawChipName(screen.mainScreen, 37 * 4, 0 * 8, 5, ref oldParam.chipLED.PriDCSG, chips[5]);
            //DrawBuff.drawChipName(screen.mainScreen, 42 * 4, 0 * 8, 6, ref oldParam.chipLED.PriRF5C, chips[6]);
            //DrawBuff.drawChipName(screen.mainScreen, 47 * 4, 0 * 8, 7, ref oldParam.chipLED.PriPWM, chips[7]);
            //DrawBuff.drawChipName(screen.mainScreen, 51 * 4, 0 * 8, 8, ref oldParam.chipLED.PriOKI5, chips[8]);
            //DrawBuff.drawChipName(screen.mainScreen, 56 * 4, 0 * 8, 9, ref oldParam.chipLED.PriOKI9, chips[9]);
            //DrawBuff.drawChipName(screen.mainScreen, 61 * 4, 0 * 8, 10, ref oldParam.chipLED.PriC140, chips[10]);
            //DrawBuff.drawChipName(screen.mainScreen, 66 * 4, 0 * 8, 11, ref oldParam.chipLED.PriSPCM, chips[11]);
            //DrawBuff.drawChipName(screen.mainScreen, 4 * 4, 0 * 8, 12, ref oldParam.chipLED.PriAY10, chips[12]);
            //DrawBuff.drawChipName(screen.mainScreen, 9 * 4, 0 * 8, 13, ref oldParam.chipLED.PriOPLL, chips[13]);
            //DrawBuff.drawChipName(screen.mainScreen, 71 * 4, 0 * 8, 14, ref oldParam.chipLED.PriHuC8, chips[14]);

            //DrawBuff.drawChipName(screen.mainScreen, 14 * 4, 1 * 8, 0, ref oldParam.chipLED.SecOPN, chips[128 + 0]);
            //DrawBuff.drawChipName(screen.mainScreen, 18 * 4, 1 * 8, 1, ref oldParam.chipLED.SecOPN2, chips[128 + 1]);
            //DrawBuff.drawChipName(screen.mainScreen, 23 * 4, 1 * 8, 2, ref oldParam.chipLED.SecOPNA, chips[128 + 2]);
            //DrawBuff.drawChipName(screen.mainScreen, 28 * 4, 1 * 8, 3, ref oldParam.chipLED.SecOPNB, chips[128 + 3]);
            //DrawBuff.drawChipName(screen.mainScreen, 33 * 4, 1 * 8, 4, ref oldParam.chipLED.SecOPM, chips[128 + 4]);
            //DrawBuff.drawChipName(screen.mainScreen, 37 * 4, 1 * 8, 5, ref oldParam.chipLED.SecDCSG, chips[128 + 5]);
            //DrawBuff.drawChipName(screen.mainScreen, 42 * 4, 1 * 8, 6, ref oldParam.chipLED.SecRF5C, chips[128 + 6]);
            //DrawBuff.drawChipName(screen.mainScreen, 47 * 4, 1 * 8, 7, ref oldParam.chipLED.SecPWM, chips[128 + 7]);
            //DrawBuff.drawChipName(screen.mainScreen, 51 * 4, 1 * 8, 8, ref oldParam.chipLED.SecOKI5, chips[128 + 8]);
            //DrawBuff.drawChipName(screen.mainScreen, 56 * 4, 1 * 8, 9, ref oldParam.chipLED.SecOKI9, chips[128 + 9]);
            //DrawBuff.drawChipName(screen.mainScreen, 61 * 4, 1 * 8, 10, ref oldParam.chipLED.SecC140, chips[128 + 10]);
            //DrawBuff.drawChipName(screen.mainScreen, 66 * 4, 1 * 8, 11, ref oldParam.chipLED.SecSPCM, chips[128 + 11]);
            //DrawBuff.drawChipName(screen.mainScreen, 4 * 4, 1 * 8, 12, ref oldParam.chipLED.SecAY10, chips[128 + 12]);
            //DrawBuff.drawChipName(screen.mainScreen, 9 * 4, 1 * 8, 13, ref oldParam.chipLED.SecOPLL, chips[128 + 13]);
            //DrawBuff.drawChipName(screen.mainScreen, 71 * 4, 0 * 8, 14, ref oldParam.chipLED.SecHuC8, chips[128 + 14]);

            DrawBuff.drawFont4(screen.mainScreen, 1, 9, 1, Audio.GetIsDataBlock(EnmModel.VirtualModel) ? "VD" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 321 - 16, 9, 1, Audio.GetIsPcmRAMWrite(EnmModel.VirtualModel) ? "VP" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 1, 17, 1, Audio.GetIsDataBlock(EnmModel.RealModel) ? "RD" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 321 - 16, 17, 1, Audio.GetIsPcmRAMWrite(EnmModel.RealModel) ? "RP" : "  ");

            oldParam.Cminutes = -1; oldParam.Csecond = -1; oldParam.Cmillisecond = -1;
            oldParam.TCminutes = -1; oldParam.TCsecond = -1; oldParam.TCmillisecond = -1;
            oldParam.LCminutes = -1; oldParam.LCsecond = -1; oldParam.LCmillisecond = -1;

        }



        public void Stop()
        {
            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            if (Audio.TrdStopped && Audio.Stopped)
            {
                Audio.ResetTimeCounter();
            }

            frmPlayList.Stop();
            OpeManager.RequestToAudio(new Request(EnmRequest.Stop, null, screenInit));
            //Audio.Stop();
            //screenInit();
        }

        public void Pause()
        {
            Audio.Pause();
        }

        public void Fadeout()
        {
            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            Audio.Fadeout();
        }

        public void Prev()
        {
            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            frmPlayList.prevPlay(newButtonMode[9]);
        }

        public void Play()
        {

            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            string[] fn = null;
            Tuple<int, int, string, string> playFn = null;

            frmPlayList.Stop();

            //if (srcBuf == null && frmPlayList.getMusicCount() < 1)
            if (frmPlayList.getMusicCount() < 1)
            {
                fn = FileOpen(false);
                if (fn == null) return;
                frmPlayList.getPlayList().AddFile(fn[0]);
                //frmPlayList.AddList(fn[0]);
                playFn = frmPlayList.setStart(-1); //last
            }
            else
            {
                fn = new string[1] { "" };
                playFn = frmPlayList.setStart(-2);//first 
            }

            reqAllScreenInit = true;

            if (loadAndPlay(playFn.Item1, playFn.Item2, playFn.Item3, playFn.Item4))
            {
                frmPlayList.Play();
            }

        }

        private void Playdata()
        {
            try
            {

                if (srcBuf == null)
                {

                    Audio.ErrMsg = "cancel";
                    return;
                }

                if (Audio.IsPaused)
                {
                    Audio.Pause();
                }
                //stop();

                //for (int chipID = 0; chipID < 2; chipID++)
                //{
                //    for (int ch = 0; ch < 3; ch++) ForceChannelMask(EnmChip.AY8910, chipID, ch, newParam.ay8910[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.YM2151, chipID, ch, newParam.ym2151[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 9; ch++) ForceChannelMask(EnmChip.YM2203, chipID, ch, newParam.ym2203[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 14; ch++) ForceChannelMask(EnmChip.YM2413, chipID, ch, newParam.ym2413[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 14; ch++) ForceChannelMask(EnmChip.YM2608, chipID, ch, newParam.ym2608[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 14; ch++) ForceChannelMask(EnmChip.YM2610, chipID, ch, newParam.ym2610[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 6; ch++) ForceChannelMask(EnmChip.YM2612, chipID, ch, newParam.ym2612[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 4; ch++) ForceChannelMask(EnmChip.SN76489, chipID, ch, newParam.sn76489[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.RF5C164, chipID, ch, newParam.rf5c164[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 24; ch++) ForceChannelMask(EnmChip.C140, chipID, ch, newParam.c140[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 32; ch++) ForceChannelMask(EnmChip.C352, chipID, ch, newParam.c352[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 16; ch++) ForceChannelMask(EnmChip.SEGAPCM, chipID, ch, newParam.segaPcm[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 6; ch++) ForceChannelMask(EnmChip.HuC6280, chipID, ch, newParam.huc6280[chipID].channels[ch].mask); 
                //    for (int ch = 0; ch < 4; ch++) ForceChannelMask(EnmChip.OKIM6295, chipID, ch, newParam.okim6295[chipID].channels[ch].mask);
                //    for (int ch = 0; ch < 2; ch++) ResetChannelMask(EnmChip.NES, chipID, ch);
                //    for (int ch = 0; ch < 3; ch++) ResetChannelMask(EnmChip.DMC, chipID, ch);
                //    for (int ch = 0; ch < 3; ch++) ResetChannelMask(EnmChip.MMC5, chipID, ch);
                //    ResetChannelMask(EnmChip.FDS, chipID, 0);
                //}

                //oldParam = new MDChipParams();
                //newParam = new MDChipParams();
                reqAllScreenInit = true;

                if (setting.other.WavSwitch)
                {
                    if (!System.IO.Directory.Exists(setting.other.WavPath))
                    {
                        DialogResult res = MessageBox.Show(
                            "wavファイル出力先に設定されたパスが存在しません。作成し演奏を続けますか。"
                            , "パス作成確認"
                            , MessageBoxButtons.YesNo
                            , MessageBoxIcon.Information);
                        if (res == DialogResult.No)
                        {
                            Audio.ErrMsg = "cancel";
                            return;
                        }
                        try
                        {
                            Directory.CreateDirectory(setting.other.WavPath);
                        }
                        catch
                        {
                            MessageBox.Show(
                               "パスの作成に失敗しました。演奏を停止します。"
                               , "作成失敗"
                               , MessageBoxButtons.OK
                               , MessageBoxIcon.Error);
                            Audio.ErrMsg = "cancel";
                            return;
                        }
                    }
                }

                if (!Audio.Play(setting))
                {
                    try
                    {
                        frmPlayList.Stop();
                        Request req = new(EnmRequest.Stop);
                        OpeManager.RequestToAudio(req);
                        //while (!req.end) System.Threading.Thread.Sleep(1);
                        //Audio.Stop();
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                    }
                    if (Audio.ErrMsg == "") throw new Exception();
                    else
                    {
                        MessageBox.Show(Audio.ErrMsg, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                for (int chipID = 0; chipID < 2; chipID++)
                {
                    for (int ch = 0; ch < 3; ch++) ForceChannelMask(EnmChip.AY8910, chipID, ch, newParam.ay8910[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.YM2151, chipID, ch, newParam.ym2151[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 9; ch++) ForceChannelMask(EnmChip.YM2203, chipID, ch, newParam.ym2203[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 14; ch++) ForceChannelMask(EnmChip.YM2413, chipID, ch, newParam.ym2413[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 14; ch++) ForceChannelMask(EnmChip.YM2608, chipID, ch, newParam.ym2608[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 14; ch++)
                    {
                        int c = ch;
                        if (ch == 12) c = 13;
                        else if (ch == 13) c = 12;
                        ForceChannelMask(EnmChip.YM2610, chipID, ch, newParam.ym2610[chipID].channels[c].mask);
                    }
                    for (int ch = 0; ch < 6; ch++) ForceChannelMask(EnmChip.YM2612, chipID, ch, newParam.ym2612[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 4; ch++) ForceChannelMask(EnmChip.SN76489, chipID, ch, newParam.sn76489[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.RF5C164, chipID, ch, newParam.rf5c164[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.RF5C68, chipID, ch, newParam.rf5c68[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 24; ch++) ForceChannelMask(EnmChip.C140, chipID, ch, newParam.c140[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 32; ch++) ForceChannelMask(EnmChip.C352, chipID, ch, newParam.c352[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 16; ch++) ForceChannelMask(EnmChip.SEGAPCM, chipID, ch, newParam.segaPcm[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 19; ch++) ForceChannelMask(EnmChip.QSound, chipID, ch, newParam.qSound[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 6; ch++) ForceChannelMask(EnmChip.HuC6280, chipID, ch, newParam.huc6280[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 4; ch++) ForceChannelMask(EnmChip.OKIM6295, chipID, ch, newParam.okim6295[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 2; ch++) ForceChannelMaskNES(EnmChip.NES, chipID, ch, newParam.nesdmc);
                    for (int ch = 2; ch < 5; ch++) ForceChannelMaskNES(EnmChip.DMC, chipID, ch, newParam.nesdmc);
                    for (int ch = 0; ch < 3; ch++) ResetChannelMask(EnmChip.MMC5, chipID, ch);
                    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.PPZ8, chipID, ch, newParam.ppz8[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.PCM8, chipID, ch, newParam.pcm8[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 4; ch++) ForceChannelMask(EnmChip.DMG, chipID, ch, newParam.dmg[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 3; ch++) ForceChannelMask(EnmChip.VRC6, chipID, ch, newParam.vrc6[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 8; ch++) ForceChannelMask(EnmChip.N163, chipID, ch, newParam.n106[chipID].channels[ch].mask);
                    for (int ch = 0; ch < 4; ch++) ForceChannelMask(EnmChip.K053260, chipID, ch, newParam.k053260[chipID].channels[ch].mask);
                    ResetChannelMask(EnmChip.FDS, chipID, 0);
                }

                Audio.GO();

                frmInfo?.UpdateInfo();

                if (setting.other.AutoOpen)
                {

                    if (Audio.ChipLED.PriOPM != 0) OpenFormYM2151(0, true); else CloseFormYM2151(0);
                    if (Audio.ChipLED.SecOPM != 0) OpenFormYM2151(1, true); else CloseFormYM2151(1);

                    if (Audio.ChipLED.PriOPN != 0) OpenFormYM2203(0, true); else CloseFormYM2203(0);
                    if (Audio.ChipLED.SecOPN != 0) OpenFormYM2203(1, true); else CloseFormYM2203(1);

                    if (Audio.ChipLED.PriOPLL != 0) OpenFormYM2413(0, true); else CloseFormYM2413(0);
                    if (Audio.ChipLED.SecOPLL != 0) OpenFormYM2413(1, true); else CloseFormYM2413(1);

                    if (Audio.ChipLED.PriOPNA != 0) OpenFormYM2608(0, true); else CloseFormYM2608(0);
                    if (Audio.ChipLED.SecOPNA != 0) OpenFormYM2608(1, true); else CloseFormYM2608(1);

                    if (Audio.ChipLED.PriOPNA2 != 0) OpenFormYM2609(0, true); else CloseFormYM2609(0);
                    if (Audio.ChipLED.SecOPNA2 != 0) OpenFormYM2609(1, true); else CloseFormYM2609(1);

                    if (Audio.ChipLED.PriOPNB != 0) OpenFormYM2610(0, true); else CloseFormYM2610(0);
                    if (Audio.ChipLED.SecOPNB != 0) OpenFormYM2610(1, true); else CloseFormYM2610(1);

                    if (Audio.ChipLED.PriOPN2 != 0) OpenFormYM2612(0, true); else CloseFormYM2612(0);
                    if (Audio.ChipLED.SecOPN2 != 0) OpenFormYM2612(1, true); else CloseFormYM2612(1);

                    if (Audio.ChipLED.PriDCSG != 0) OpenFormSN76489(0, true); else CloseFormSN76489(0);
                    if (Audio.ChipLED.SecDCSG != 0)
                    {
                        if (!Audio.SN76489NGPFlag) OpenFormSN76489(1, true);
                    }
                    else CloseFormSN76489(1);

                    if (Audio.ChipLED.PriPPZ8 != 0) OpenFormPPZ8(0, true); else CloseFormPPZ8(0);
                    if (Audio.ChipLED.SecPPZ8 != 0) OpenFormPPZ8(1, true); else CloseFormPPZ8(1);

                    if (Audio.ChipLED.PriFME7 != 0) OpenFormS5B(0, true); else CloseFormS5B(0);
                    if (Audio.ChipLED.SecFME7 != 0) OpenFormS5B(1, true); else CloseFormS5B(1);

                    if (Audio.ChipLED.PriDMG != 0) OpenFormDMG(0, true); else CloseFormDMG(0);
                    if (Audio.ChipLED.SecDMG != 0) OpenFormDMG(1, true); else CloseFormDMG(1);

                    if (Audio.ChipLED.PriRF5C != 0) OpenFormMegaCD(0, true); else CloseFormMegaCD(0);
                    if (Audio.ChipLED.SecRF5C != 0) OpenFormMegaCD(1, true); else CloseFormMegaCD(1);

                    if (Audio.ChipLED.PriRF5C68 != 0) OpenFormRf5c68(0, true); else CloseFormRf5c68(0);
                    if (Audio.ChipLED.SecRF5C68 != 0) OpenFormRf5c68(1, true); else CloseFormRf5c68(1);

                    if (Audio.ChipLED.PriOKI5 != 0) OpenFormOKIM6258(0, true); else CloseFormOKIM6258(0);
                    if (Audio.ChipLED.SecOKI5 != 0) OpenFormOKIM6258(1, true); else CloseFormOKIM6258(1);

                    if (Audio.ChipLED.PriOKI9 != 0) OpenFormOKIM6295(0, true); else CloseFormOKIM6295(0);
                    if (Audio.ChipLED.SecOKI9 != 0) OpenFormOKIM6295(1, true); else CloseFormOKIM6295(1);

                    if (Audio.ChipLED.PriPCM8 != 0) OpenFormPCM8(0, true); else CloseFormPCM8(0);
                    if (Audio.ChipLED.SecPCM8 != 0) OpenFormPCM8(1, true); else CloseFormPCM8(1);

                    if (Audio.ChipLED.PriC140 != 0) OpenFormC140(0, true); else CloseFormC140(0);
                    if (Audio.ChipLED.SecC140 != 0) OpenFormC140(1, true); else CloseFormC140(1);

                    if (Audio.ChipLED.PriYMZ != 0) OpenFormYMZ280B(0, true); else CloseFormYMZ280B(0);
                    if (Audio.ChipLED.SecYMZ != 0) OpenFormYMZ280B(1, true); else CloseFormYMZ280B(1);

                    if (Audio.ChipLED.PriC352 != 0) OpenFormC352(0, true); else CloseFormC352(0);
                    if (Audio.ChipLED.SecC352 != 0) OpenFormC352(1, true); else CloseFormC352(1);

                    if (Audio.ChipLED.PriMPCM != 0) OpenFormMultiPCM(0, true); else CloseFormMultiPCM(0);
                    if (Audio.ChipLED.SecMPCM != 0) OpenFormMultiPCM(1, true); else CloseFormMultiPCM(1);

                    if (Audio.ChipLED.PriQsnd != 0) OpenFormQSound(0, true); else CloseFormQSound(0);
                    if (Audio.ChipLED.SecQsnd != 0) OpenFormQSound(1, true); else CloseFormQSound(1);

                    if (Audio.ChipLED.PriSPCM != 0) OpenFormSegaPCM(0, true); else CloseFormSegaPCM(0);
                    if (Audio.ChipLED.SecSPCM != 0) OpenFormSegaPCM(1, true); else CloseFormSegaPCM(1);

                    if (Audio.ChipLED.PriAY10 != 0) OpenFormAY8910(0, true); else CloseFormAY8910(0);
                    if (Audio.ChipLED.SecAY10 != 0) OpenFormAY8910(1, true); else CloseFormAY8910(1);

                    if (Audio.ChipLED.PriHuC != 0) OpenFormHuC6280(0, true); else CloseFormHuC6280(0);
                    if (Audio.ChipLED.SecHuC != 0) OpenFormHuC6280(1, true); else CloseFormHuC6280(1);

                    if (Audio.ChipLED.PriK051649 != 0) OpenFormK051649(0, true); else CloseFormK051649(0);
                    if (Audio.ChipLED.SecK051649 != 0) OpenFormK051649(1, true); else CloseFormK051649(1);

                    if (Audio.ChipLED.PriMID != 0) OpenFormMIDI(0, true); else CloseFormMIDI(0);
                    //if (Audio.chipLED.SecMID != 0) OpenFormMIDI(1, true); else CloseFormMIDI(1);

                    if (Audio.ChipLED.PriNES != 0 || Audio.ChipLED.PriDMC != 0) OpenFormNESDMC(0, true); else CloseFormNESDMC(0);
                    if (Audio.ChipLED.SecNES != 0 || Audio.ChipLED.SecDMC != 0) OpenFormNESDMC(1, true); else CloseFormNESDMC(1);

                    if (Audio.ChipLED.PriFDS != 0) OpenFormFDS(0, true); else CloseFormFDS(0);
                    if (Audio.ChipLED.SecFDS != 0) OpenFormFDS(1, true); else CloseFormFDS(1);

                    if (Audio.ChipLED.PriVRC6 != 0) OpenFormVRC6(0, true); else CloseFormVRC6(0);
                    if (Audio.ChipLED.SecVRC6 != 0) OpenFormVRC6(1, true); else CloseFormVRC6(1);

                    if (Audio.ChipLED.PriVRC7 != 0) OpenFormVRC7(0, true); else CloseFormVRC7(0);
                    if (Audio.ChipLED.SecVRC7 != 0) OpenFormVRC7(1, true); else CloseFormVRC7(1);

                    if (Audio.ChipLED.PriMMC5 != 0) OpenFormMMC5(0, true); else CloseFormMMC5(0);
                    if (Audio.ChipLED.SecMMC5 != 0) OpenFormMMC5(1, true); else CloseFormMMC5(1);

                    if (Audio.ChipLED.PriN106 != 0) OpenFormN106(0, true); else CloseFormN106(0);
                    if (Audio.ChipLED.SecN106 != 0) OpenFormN106(1, true); else CloseFormN106(1);

                    if (Audio.ChipLED.PriOPL != 0) OpenFormYM3526(0, true); else CloseFormYM3526(0);
                    if (Audio.ChipLED.SecOPL != 0) OpenFormYM3526(1, true); else CloseFormYM3526(1);

                    if (Audio.ChipLED.PriY8950 != 0) OpenFormY8950(0, true); else CloseFormY8950(0);
                    if (Audio.ChipLED.SecY8950 != 0) OpenFormY8950(1, true); else CloseFormY8950(1);

                    if (Audio.ChipLED.PriOPL2 != 0) OpenFormYM3812(0, true); else CloseFormYM3812(0);
                    if (Audio.ChipLED.SecOPL2 != 0) OpenFormYM3812(1, true); else CloseFormYM3812(1);

                    if (Audio.ChipLED.PriOPL3 != 0) OpenFormYMF262(0, true); else CloseFormYMF262(0);
                    if (Audio.ChipLED.SecOPL3 != 0) OpenFormYMF262(1, true); else CloseFormYMF262(1);

                    if (Audio.ChipLED.PriOPL4 != 0) OpenFormYMF278B(0, true); else CloseFormYMF278B(0);
                    if (Audio.ChipLED.SecOPL4 != 0) OpenFormYMF278B(1, true); else CloseFormYMF278B(1);

                    if (Audio.ChipLED.PriOPX != 0) OpenFormYMF271(0, true); else CloseFormYMF271(0);
                    if (Audio.ChipLED.SecOPX != 0) OpenFormYMF271(1, true); else CloseFormYMF271(1);

                    if (Audio.ChipLED.PriK053260 != 0) OpenFormK053260(0, true); else CloseFormK053260(0);
                    if (Audio.ChipLED.SecK053260 != 0) OpenFormK053260(1, true); else CloseFormK053260(1);
                }
            }
            catch (Exception e)
            {
                Audio.ErrMsg = e.Message;
            }
        }

        public void Up_master_volume()
        {
            Audio.SetMasterVolume(false, 1);
        }

        public void Down_master_volume()
        {
            Audio.SetMasterVolume(false, -1);
        }

        public void Reset_master_volume()
        {
            Audio.SetMasterVolume(true, 0);
        }

        public void Up_playlist_cursor()
        {
            frmPlayList.upCursor();
        }

        public void Down_playlist_cursor()
        {
            frmPlayList.downCursor();
        }

        public void Play_playlist_cursor()
        {
            frmPlayList.playCursor();
        }

        public void Ff()
        {
            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            Audio.FF();
        }

        public void Next()
        {
            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            Request req = new(EnmRequest.Stop);
            OpeManager.RequestToAudio(req);
            while (!req.End) System.Threading.Thread.Sleep(1);
            //Audio.Stop();

            screenInit(null);

            //frmPlayList.nextPlay();
            frmPlayList.nextPlayMode(newButtonMode[9]);
        }

        private void NextPlayMode()
        {
            frmPlayList.nextPlayMode(newButtonMode[9]);
        }

        public void Slow()
        {
            if (Audio.IsPaused)
            {
                Audio.StepPlay(4000);
                Audio.Pause();
                return;
            }

            if (Audio.IsStopped)
            {
                Play();
            }

            Audio.Slow();
        }

        private void PlayMode()
        {
            newButtonMode[9]++;
            if (newButtonMode[9] > 3) newButtonMode[9] = 0;
            toolTip1.SetToolTip(opeButtonMode, modeTip[newButtonMode[9]]);
        }

        private string[] FileOpen(bool flg)
        {
            OpenFileDialog ofd = new()
            {
                Filter = Resources.cntSupportFile.Replace("\r\n", ""),
                Title = "ファイルを選択してください",
                FilterIndex = setting.other.FilterIndex
            };

            if (setting.other.DefaultDataPath != "" && Directory.Exists(setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = setting.other.DefaultDataPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;
            ofd.Multiselect = flg;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            IsInitialOpenFolder = false;
            setting.other.FilterIndex = ofd.FilterIndex;

            return ofd.FileNames;

        }

        private void DispPlayList()
        {
            frmPlayList.setting = setting;
            //if (setting.location.PPlayList != System.Drawing.Point.Empty)
            //{
            //    frmPlayList.Location = setting.location.PPlayList;

            //}
            //if (setting.location.PPlayListWH != System.Drawing.Point.Empty)
            //{
            //    frmPlayList.Width = setting.location.PPlayListWH.X;
            //    frmPlayList.Height = setting.location.PPlayListWH.Y;
            //}
            frmPlayList.Visible = !frmPlayList.Visible;
            if (frmPlayList.Visible) CheckAndSetForm(frmPlayList);
            frmPlayList.TopMost = true;
            frmPlayList.TopMost = false;
        }

        private void DispVSTList()
        {
            frmVSTeffectList.Visible = !frmVSTeffectList.Visible;
            if (frmVSTeffectList.Visible) CheckAndSetForm(frmVSTeffectList);
            frmVSTeffectList.TopMost = true;
            frmVSTeffectList.TopMost = false;
        }

        private void ShowContextMenu()
        {
            cmsOpenOtherPanel.Show();
            System.Drawing.Point p = Control.MousePosition;
            cmsOpenOtherPanel.Top = p.Y;
            cmsOpenOtherPanel.Left = p.X;
        }



        public const int FCC_VGM = 0x206D6756;	// "Vgm "

        public byte[] GetAllBytes(string filename, out EnmFileFormat format)
        {
            format = EnmFileFormat.unknown;

            string ext = Path.GetExtension(filename).ToLower();

            //wav/mp3/aiffはnaudioに任せるのでここの処理はスキップ
            if (ext == ".wav")
            {
                format = EnmFileFormat.WAV;
                return new byte[] { (byte)'W', (byte)'A', (byte)'V' };
            }

            if (ext == ".mp3")
            {
                format = EnmFileFormat.MP3;
                return new byte[] { (byte)'M', (byte)'P', (byte)'3' };
            }

            if (ext == ".aiff")
            {
                format = EnmFileFormat.AIFF;
                return new byte[] { (byte)'A', (byte)'I', (byte)'F', (byte)'F' };
            }

            //先ずは丸ごと読み込む
            byte[] buf = System.IO.File.ReadAllBytes(filename);

            //.NRDファイルの場合は拡張子判定
            if (ext == ".nrd")
            {
                format = EnmFileFormat.NRT;
                return buf;
            }

            if (ext == ".mgs")
            {
                format = EnmFileFormat.MGS;
                return buf;
            }

            if (ext == ".bgm")
            {
                format = EnmFileFormat.MuSICA;
                return buf;
            }

            if (ext == ".msd")
            {
                format = EnmFileFormat.MuSICA_src;
                return buf;
            }

            if (ext == ".mdr")
            {
                format = EnmFileFormat.MDR;
                return buf;
            }

            if (ext == ".mdl")
            {
                format = EnmFileFormat.MDL;
                return buf;
            }

            if (ext == ".mdx")
            {
                format = EnmFileFormat.MDX;
                return buf;
            }

            if (ext == ".mnd")
            {
                format = EnmFileFormat.MND;
                return buf;
            }

            if (ext == ".mub")
            {
                format = EnmFileFormat.MUB;
                return buf;
            }

            if (ext == ".muc")
            {
                format = EnmFileFormat.MUC;
                return buf;
            }

            if (ext == ".m" || ext == ".m2" || ext == ".mz")
            {
                format = EnmFileFormat.M;
                return buf;
            }

            if (ext == ".mpi" || ext == ".mvi" || ext == ".mzi" || ext == ".opi" || ext == ".ovi" || ext == ".ozi")
            {
                format = EnmFileFormat.FMP;
                return buf;
            }

            if (ext == ".mml")
            {
                format = EnmFileFormat.MML;
                return buf;
            }

            if (ext == ".xgm")
            {
                format = xgm2.CheckXGM2(buf) ? EnmFileFormat.XGM2 : EnmFileFormat.XGM;
                return buf;
            }

            if (ext == ".xgz")
            {
                buf = Common.unzipFile(filename);
                format = EnmFileFormat.XGM;
                return buf;
            }

            if (ext == ".zgm")
            {
                format = EnmFileFormat.ZGM;
                return buf;
            }

            if (ext == ".s98")
            {
                format = EnmFileFormat.S98;
                return buf;
            }

            if (ext == ".nsf")
            {
                format = EnmFileFormat.NSF;
                return buf;
            }

            if (ext == ".hes")
            {
                format = EnmFileFormat.HES;
                return buf;
            }

            if (ext == ".sid")
            {
                format = EnmFileFormat.SID;
                return buf;
            }

            if (ext == ".mid")
            {
                format = EnmFileFormat.MID;
                return buf;
            }

            if (ext == ".rcp")
            {
                format = EnmFileFormat.RCP;
                return buf;
            }


            //.VGMの場合はヘッダの確認とGzipで解凍後のファイルのヘッダの確認
            uint vgm = (UInt32)buf[0] + (UInt32)buf[1] * 0x100 + (UInt32)buf[2] * 0x10000 + (UInt32)buf[3] * 0x1000000;
            if (vgm == FCC_VGM)
            {
                format = EnmFileFormat.VGM;
                return buf;
            }

            format = EnmFileFormat.VGM;
            return Common.unzipFile(filename);
        }

        public void GetInstCh(EnmChip chip, int ch, int chipID,int note=-1)
        {
            try
            {
                YM2612MIDI.SetVoiceFromChipRegister(chip, chipID, ch);

                if (!setting.other.UseGetInst) return;

                if (chip == EnmChip.YM2413)
                {
                    if (setting.other.InstFormat == EnmInstFormat.MML2VGM)
                    {

                    }
                    else if (setting.other.InstFormat == EnmInstFormat.SendMML2VGM)
                    {
                        GetInstChForSendMML2VGM(chip, ch, chipID);
                    }
                    else
                    {
                        GetInstChForMGSC(chip, ch, chipID);
                    }
                    return;
                }
                else if (chip == EnmChip.YM3812 || chip == EnmChip.YMF262 || chip == EnmChip.YMF278B)
                {
                    if (setting.other.InstFormat == EnmInstFormat.MML2VGM)
                    {

                    }
                    else if (setting.other.InstFormat == EnmInstFormat.SendMML2VGM)
                    {
                        GetInstChForSendMML2VGM(EnmChip.YM3812, ch, chipID);
                    }
                    else if (setting.other.InstFormat == EnmInstFormat.OPLI)
                    {
                        getInstChForOPLI(chip, ch, chipID);
                    }
                    else
                    {
                    }
                }
                else if (chip == EnmChip.VRC7)
                {
                    if (setting.other.InstFormat == EnmInstFormat.MML2VGM)
                    {

                    }
                    else if (setting.other.InstFormat == EnmInstFormat.SendMML2VGM)
                    {
                        GetInstChForSendMML2VGM(chip, ch, chipID);
                    }
                    else
                    {
                        GetInstChForMGSC(chip, ch, chipID);
                    }
                    return;
                }
                else if (chip == EnmChip.K051649)
                {
                    if (setting.other.InstFormat == EnmInstFormat.MML2VGM)
                    {

                    }
                    else if (setting.other.InstFormat == EnmInstFormat.SendMML2VGM)
                    {
                        GetInstChForSendMML2VGM(chip, ch, chipID);
                    }
                    else if (setting.other.InstFormat == EnmInstFormat.MGSCSCC_PLAIN)
                    {
                        getInstChForMGSCSCCPLAIN(ch, chipID);
                    }
                    else
                    {
                        GetInstChForMGSC(chip, ch, chipID);
                    }
                    return;
                }
                else if (chip == EnmChip.N163)
                {
                    if (setting.other.InstFormat == EnmInstFormat.MML2VGM)
                    {

                    }
                    else if (setting.other.InstFormat == EnmInstFormat.SendMML2VGM)
                    {
                        GetInstChForSendMML2VGM(chip, ch, chipID);
                    }
                    else
                    {
                        getInstChForMCK(chip, ch, chipID);
                    }
                    return;
                }
                else if (chip == EnmChip.HuC6280)
                {
                    if (setting.other.InstFormat == EnmInstFormat.MML2VGM)
                    {

                    }
                    else if (setting.other.InstFormat == EnmInstFormat.SendMML2VGM)
                    {
                        GetInstChForSendMML2VGM(chip, ch, chipID);
                    }
                    else if (setting.other.InstFormat == EnmInstFormat.HUSIC)
                    {
                        GetInstChForHuSIC(chip, ch, chipID);
                    }
                    return;
                }
                else
                {
                    switch (setting.other.InstFormat)
                    {
                        case EnmInstFormat.FMP7:
                            GetInstChForFMP7(chip, ch, chipID);
                            break;
                        case EnmInstFormat.MDX:
                            GetInstChForMDX(chip, ch, chipID);
                            break;
                        case EnmInstFormat.MML2VGM:
                            GetInstChForMML2VGM(chip, ch, chipID);
                            break;
                        case EnmInstFormat.MUCOM88:
                            GetInstChForMucom88(chip, ch, chipID, note);
                            break;
                        case EnmInstFormat.MUSICLALF:
                            GetInstChForMUSICLALF(chip, ch, chipID);
                            break;
                        case EnmInstFormat.MUSICLALF2:
                            GetInstChForMUSICLALF2(chip, ch, chipID);
                            break;
                        case EnmInstFormat.TFI:
                            getInstChForTFI(chip, ch, chipID);
                            break;
                        case EnmInstFormat.NRTDRV:
                            GetInstChForNRTDRV(chip, ch, chipID);
                            break;
                        case EnmInstFormat.VOPM:
                            getInstChForVOPM(chip, ch, chipID);
                            break;
                        case EnmInstFormat.PMD:
                            getInstChForPMD(chip, ch, chipID);
                            break;
                        case EnmInstFormat.DMP:
                            getInstChForDMP(chip, ch, chipID);
                            break;
                        case EnmInstFormat.OPNI:
                            getInstChForOPNI(chip, ch, chipID);
                            break;
                        case EnmInstFormat.RYM2612:
                            getInstChForRYM2612(chip, ch, chipID);
                            break;
                        case EnmInstFormat.SendMML2VGM:
                            GetInstChForSendMML2VGM(chip, ch, chipID);
                            break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("音色出力エラー", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetInstChForFMP7(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "'@ FA xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , alg//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);
                n = "'@ FC xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AM\r\n";

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , alg //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForMDX(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "'@xx = {\r\n/* AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME\r\n";

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("   {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , 0
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                    );
                }
                n += "/* ALG FB  OP\r\n";
                n += string.Format("   {0:D3},{1:D3},15\r\n}}\r\n"
                    , alg//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                n = "'@xx = {\r\n/* AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME\r\n";

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("   {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "/* ALG FB  OP\r\n";
                n += string.Format("   {0:D3},{1:D3},15\r\n}}\r\n"
                    , alg //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForMML2VGM(EnmChip chip, int ch, int chipID)
        {
            string n = GetInstChForMML2VGMFormat(chip, ch, chipID);
            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForSendMML2VGM(EnmChip chip, int ch, int chipID)
        {
            string n = GetInstChForMML2VGMFormat(chip, ch, chipID);
            if (string.IsNullOrEmpty(n)) return;

            KumaCom mmf = new fileCom(true, "mml2vgmIDE", "mml2vgmFMVoicePool", 1024 * 4);
            try
            {
                mmf.SendMessage(string.Join(":", "SendVoice", n));
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("メッセージが長すぎ");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("mml2vgmの共有メモリが見つかりませんでした");
            }
        }

        private int[] slot1Tbl = new int[] { 0, 1, 2, 6, 7, 8, 12, 13, 14 };
        private int[] slot2Tbl = new int[] { 3, 4, 5, 9, 10, 11, 15, 16, 17 };

        private string GetInstChForMML2VGMFormat(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "'@ N xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSG-EG\r\n";

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                        , fmRegister[p][0x90 + ops + c] & 0x0f//SG
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , alg//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);
                n = "'@ M xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME\r\n";

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT1
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , alg //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }
            else if (chip == EnmChip.HuC6280)
            {
                MDSound.Ootake_PSG.huc6280_state huc6280Register = Audio.GetHuC6280Register(chipID);
                if (huc6280Register == null) return null;
                MDSound.Ootake_PSG.PSG psg = huc6280Register.Psg[ch];
                if (psg == null) return null;
                if (psg.wave == null) return null;
                if (psg.wave.Length != 32) return null;

                n = "'@ H xx,\r\n   +0 +1 +2 +3 +4 +5 +6 +7\r\n";

                for (int i = 0; i < 32; i += 8)
                {
                    n += string.Format("'@ {0:D2},{1:D2},{2:D2},{3:D2},{4:D2},{5:D2},{6:D2},{7:D2}\r\n"
                        , (17 - psg.wave[i + 0])
                        , (17 - psg.wave[i + 1])
                        , (17 - psg.wave[i + 2])
                        , (17 - psg.wave[i + 3])
                        , (17 - psg.wave[i + 4])
                        , (17 - psg.wave[i + 5])
                        , (17 - psg.wave[i + 6])
                        , (17 - psg.wave[i + 7])
                        );
                }
            }
            else if (chip == EnmChip.YM2413)
            {
                //OPLL
                int[] regs = Audio.GetYM2413Register(chipID);
            }
            else if (chip == EnmChip.YM3812)
            {
                //OPL2
                //'@ L No "Name"
                //'@ AR DR SL RR KSL TL MT AM VIB EGT KSR WS
                //'@ AR DR SL RR KSL TL MT AM VIB EGT KSR WS
                //'@ CNT FB

                int[] regs = Audio.GetYM3812Register(chipID);
                int slot;
                if (ch < 0 || ch > 8) return null;

                n = "'@ L No \"MDP\"\r\n   AR DR SL RR KSL TL MT AM VIB EGT KSR WS\r\n";
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0) slot = slot1Tbl[ch];
                    else slot = slot2Tbl[ch];

                    slot = (slot % 6) + 8 * (slot / 6);
                    n += string.Format("'@ {0:D2},{1:D2},{2:D2},{3:D2}, {4:D2},{5:D2},{6:D2},{7:D2}, {8:D2}, {9:D2}, {10:D2},{11:D2}\r\n"
                        , regs[0x60 + slot] >> 4
                        , regs[0x60 + slot] & 0xf
                        , regs[0x80 + slot] >> 4
                        , regs[0x80 + slot] & 0xf
                        , regs[0x40 + slot] >> 6
                        , regs[0x40 + slot] & 0x3f
                        , regs[0x20 + slot] & 0xf
                        , regs[0x20 + slot] >> 7
                        , (regs[0x20 + slot] >> 6) & 1
                        , (regs[0x20 + slot] >> 5) & 1
                        , (regs[0x20 + slot] >> 4) & 1
                        , (regs[0xe0 + slot] & 3)
                        );
                }
                n += string.Format("   CNT FB\r\n'@  {0:D2},{1:D2}\r\n"
                    , (regs[0xc0 + ch] & 1)
                    , (regs[0xc0 + ch] >> 1) & 7
                    );
            }

            //VRC7/K051649/N163

            return n;
        }

        private void GetInstChForMUSICLALF(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = string.Format("  @xx:{{\r\n  {0:D3} {1:D3}\r\n"
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                    , alg//AL
                    );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("  {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3} {8:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                    );
                }
                n += "  }\r\n";
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = string.Format("  @xx:{{\r\n  {0:D3} {1:D3}\r\n"
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                    , alg //AL
                    );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("  {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3} {8:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                    );
                }
                n += "  }\r\n";
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForMucom88(EnmChip chip, int ch, int chipID, int note)
        {

            string n = "";
            int num = ch + 1;
            string notes = "";
            if (note != -1)
            {
                notes = Tables.kbn[note % 12];
                if (note / 12 < 10) notes += Tables.kbo[note / 12];
            }

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = string.Format("  @{0:D02}:{{\r\n  {1:D3}, {2:D3}\r\n"
                    , num
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                    , alg//AL
                    );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("  {0:D3}, {1:D3}, {2:D3}, {3:D3}, {4:D3}, {5:D3}, {6:D3}, {7:D3}, {8:D3}" + (i != 3 ? "\r\n" : "")
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                    );
                }
                n += string.Format(",\"{0}\"  }}\r\n", note == -1 ? "MDP" : notes);
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = string.Format("  @{0:D02}:{{\r\n  {1:D3}, {2:D3}\r\n"
                    , num
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                    , alg //AL
                    );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("  {0:D3}, {1:D3}, {2:D3}, {3:D3}, {4:D3}, {5:D3}, {6:D3}, {7:D3}, {8:D3}" + (i != 3 ? "\r\n" : "")
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                    );
                }
                n += string.Format(",\"{0}\"  }}\r\n", note == -1 ? "MDP" : notes);
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForMUSICLALF2(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "@%xxx\r\n";

                for (int i = 0; i < 6; i++)
                {
                    if (0x30 + i * 0x10 != 0x40)
                    {
                        n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                            , fmRegister[p][0x30 + 0 + c + i * 0x10] & 0xff
                            , fmRegister[p][0x30 + 8 + c + i * 0x10] & 0xff
                            , fmRegister[p][0x30 + 16 + c + i * 0x10] & 0xff
                            , fmRegister[p][0x30 + 24 + c + i * 0x10] & 0xff
                        );
                    }
                    else
                    {
                        n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                            , tl[0]
                            , tl[2]
                            , tl[1]
                            , tl[3]
                        );
                    }
                }
                n += string.Format("${0:X3}\r\n"
                    , fmRegister[p][0xb0 + c] //FB/AL
                    );
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "@%xxx\r\n";

                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0x40 + 0 + ch] & 0x7f) //DT/ML
                    , (ym2151Register[0x40 + 8 + ch] & 0x7f) //DT/ML
                    , (ym2151Register[0x40 + 16 + ch] & 0x7f)//DT/ML
                    , (ym2151Register[0x40 + 24 + ch] & 0x7f)//DT/ML
                );
                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , tl[0] //TL
                    , tl[2] //TL
                    , tl[1] //TL
                    , tl[3] //TL
                );
                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0x80 + 0 + ch] & 0xdf) //KS/AR
                    , (ym2151Register[0x80 + 8 + ch] & 0xdf) //KS/AR
                    , (ym2151Register[0x80 + 16 + ch] & 0xdf)//KS/AR
                    , (ym2151Register[0x80 + 24 + ch] & 0xdf)//KS/AR
                );
                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0xa0 + 0 + ch] & 0x9f) //AM/DR
                    , (ym2151Register[0xa0 + 8 + ch] & 0x9f) //AM/DR
                    , (ym2151Register[0xa0 + 16 + ch] & 0x9f)//AM/DR
                    , (ym2151Register[0xa0 + 24 + ch] & 0x9f)//AM/DR
                );
                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0xc0 + 0 + ch] & 0x1f) //SR
                    , (ym2151Register[0xc0 + 8 + ch] & 0x1f) //SR
                    , (ym2151Register[0xc0 + 16 + ch] & 0x1f)//SR
                    , (ym2151Register[0xc0 + 24 + ch] & 0x1f)//SR
                );
                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0xe0 + 0 + ch] & 0xff) //SL/RR
                    , (ym2151Register[0xe0 + 8 + ch] & 0xff) //SL/RR
                    , (ym2151Register[0xe0 + 16 + ch] & 0xff)//SL/RR
                    , (ym2151Register[0xe0 + 24 + ch] & 0xff)//SL/RR
                );

                n += string.Format("${0:X3}\r\n"
                    , ym2151Register[0x20 + ch] //FB/AL
                    );
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForNRTDRV(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "@ xxxx {\r\n";
                n += string.Format("000,{0:D3},{1:D3},015\r\n"
                    , alg//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format(" {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , 0
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                    );
                }
                n += "}\r\n";
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "@ xxxx {\r\n";
                n += string.Format("000,{0:D3},{1:D3},015\r\n"
                    , alg //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format(" {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "}\r\n";
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForHuSIC(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.HuC6280)
            {
                MDSound.Ootake_PSG.huc6280_state huc6280Register = Audio.GetHuC6280Register(chipID);
                if (huc6280Register == null) return;
                MDSound.Ootake_PSG.PSG psg = huc6280Register.Psg[ch];
                if (psg == null) return;
                if (psg.wave == null) return;
                if (psg.wave.Length != 32) return;

                n = "@WTx={\r\n";

                for (int i = 0; i < 32; i += 8)
                {
                    n += string.Format("${0:x2},${1:x2},${2:x2},${3:x2},${4:x2},${5:x2},${6:x2},${7:x2},\r\n"
                        , (17 - psg.wave[i + 0])
                        , (17 - psg.wave[i + 1])
                        , (17 - psg.wave[i + 2])
                        , (17 - psg.wave[i + 3])
                        , (17 - psg.wave[i + 4])
                        , (17 - psg.wave[i + 5])
                        , (17 - psg.wave[i + 6])
                        , (17 - psg.wave[i + 7])
                        );
                }

                n = string.Concat(n.AsSpan(0, n.Length - 3), "\r\n}\r\n");
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForMGSC(EnmChip chip, int ch, int chipID)
        {

            string n = "";
            int[] Register = null;

            if (chip == EnmChip.YM2413)
            {
                Register = Audio.GetYM2413Register(chipID);
            }
            else if (chip == EnmChip.VRC7)
            {
                byte[] r = Audio.GetVRC7Register(chipID);
                if (r == null) return;
                Register = new int[r.Length];
                for (int i = 0; i < r.Length; i++)
                {
                    Register[i] = r[i];
                }
            }
            else if (chip == EnmChip.K051649)
            {
                GetInstChForMGSCSCC(ch, chipID);
                return;
            }

            if (Register == null) return;
            n = "@vXX = { \r\n";
            n += "   ;       TL FB\r\n";
            n += string.Format("           {0:d2},{1:d2},\r\n"
                , Register[0x02] & 0x3f
                , Register[0x03] & 0x7
                );
            n += "   ;       AR DR SL RR KL MT AM VB EG KR DT\r\n";

            n += string.Format("           {0:d2},{1:d2},{2:d2},{3:d2},{4:d2},{5:d2},{6:d2},{7:d2},{8:d2},{9:d2},{10:d2},\r\n"
                , (Register[0x04] & 0xf0) >> 4
                , (Register[0x04] & 0x0f)
                , (Register[0x06] & 0xf0) >> 4
                , (Register[0x06] & 0x0f)
                , (Register[0x02] & 0xc0) >> 6
                , (Register[0x00] & 0x0f)
                , (Register[0x00] & 0x80) >> 7
                , (Register[0x00] & 0x40) >> 6
                , (Register[0x00] & 0x20) >> 5
                , (Register[0x00] & 0x10) >> 4
                , (Register[0x03] & 0x08) >> 3
                );

            n += string.Format("           {0:d2},{1:d2},{2:d2},{3:d2},{4:d2},{5:d2},{6:d2},{7:d2},{8:d2},{9:d2},{10:d2} }}\r\n"
                , (Register[0x05] & 0xf0) >> 4
                , (Register[0x05] & 0x0f)
                , (Register[0x07] & 0xf0) >> 4
                , (Register[0x07] & 0x0f)
                , (Register[0x03] & 0xc0) >> 6
                , (Register[0x01] & 0x0f)
                , (Register[0x01] & 0x80) >> 7
                , (Register[0x01] & 0x40) >> 6
                , (Register[0x01] & 0x20) >> 5
                , (Register[0x01] & 0x10) >> 4
                , (Register[0x03] & 0x10) >> 4
                );



            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetInstChForMGSCSCC(int ch, int chipID)
        {

            int[] Register = null;

            MDSound.K051649.k051649_state chip = Audio.GetK051649Register(chipID);
            if (chip == null) return;
            MDSound.K051649.k051649_sound_channel psg = chip.channel_list[ch];
            if (psg == null) return;
            Register = new int[32];
            for (int i = 0; i < 32; i++) Register[i] = (int)psg.waveram[i];
            if (Register == null) return;

            string n = "@sXX = {";
            for (int i = 0; i < 8; i++)
            {
                n += string.Format(" {0:x02}{1:x02}{2:x02}{3:x02}",
                    (byte)Register[i * 4 + 0], (byte)Register[i * 4 + 1],
                    (byte)Register[i * 4 + 2], (byte)Register[i * 4 + 3]
                    );
            }
            n += " }\r\n";

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void getInstChForMGSCSCCPLAIN(int ch, int chipID)
        {

            int[] Register = null;

            MDSound.K051649.k051649_state chip = Audio.GetK051649Register(chipID);
            if (chip == null) return;
            MDSound.K051649.k051649_sound_channel psg = chip.channel_list[ch];
            if (psg == null) return;
            Register = new int[32];
            for (int i = 0; i < 32; i++) Register[i] = (int)psg.waveram[i];
            if (Register == null) return;

            string n = "";
            for (int i = 0; i < 8; i++)
            {
                n += string.Format("{0:x02}{1:x02}{2:x02}{3:x02}",
                    (byte)Register[i * 4 + 0], (byte)Register[i * 4 + 1],
                    (byte)Register[i * 4 + 2], (byte)Register[i * 4 + 3]
                    );
            }
            n += "\r\n";

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void getInstChForMCK(EnmChip chip, int ch, int chipID)
        {
            string n = "";

            if (chip == EnmChip.N163)
            {
                TrackInfoN106[] info = (TrackInfoN106[])Audio.GetN106Register(0);
                if (info == null) return;

                n = "@Nxx = { ";
                n += string.Format("{0} ", info[ch].wavelen);
                for (int i = 0; i < info[ch].wavelen; i++)
                {
                    n += string.Format("{0} ", (byte)info[ch].wave[i]);
                }
                n += "}\r\n";

                if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
            }

        }

        private void getInstChForTFI(EnmChip chip, int ch, int chipID)
        {

            byte[] n = new byte[42];

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n[0] = (byte)(alg);//AL
                n[1] = (byte)((fmRegister[p][0xb0 + c] & 0x38) >> 3);//FB


                for (int i = 0; i < 4; i++)
                {
                    //int ops = (i == 0) ? 0 : ((i == 1) ? 4 : ((i == 2) ? 8 : 12));
                    int ops = i * 4;

                    n[i * 10 + 2] = (byte)(fmRegister[p][0x30 + ops + c] & 0x0f);//ML
                    int dt = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    // 0>3  1>4  2>5  3>6  4>3  5>2  6>1  7>0
                    dt = (dt < 4) ? (dt + 3) : (7 - dt);
                    n[i * 10 + 3] = (byte)dt;
                    n[i * 10 + 4] = (byte)(tl[i]);//TL
                    n[i * 10 + 5] = (byte)((fmRegister[p][0x50 + ops + c] & 0xc0) >> 6);//KS
                    n[i * 10 + 6] = (byte)(fmRegister[p][0x50 + ops + c] & 0x1f); //AR
                    n[i * 10 + 7] = (byte)(fmRegister[p][0x60 + ops + c] & 0x1f); //DR
                    n[i * 10 + 8] = (byte)(fmRegister[p][0x70 + ops + c] & 0x1f); //SR
                    n[i * 10 + 9] = (byte)(fmRegister[p][0x80 + ops + c] & 0x0f); //RR
                    n[i * 10 + 10] = (byte)((fmRegister[p][0x80 + ops + c] & 0xf0) >> 4);//SL
                    n[i * 10 + 11] = (byte)(fmRegister[p][0x90 + ops + c] & 0x0f);//SSG
                }

            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n[0] = (byte)(alg);//AL
                n[1] = (byte)((ym2151Register[0x20 + ch] & 0x38) >> 3);//FB

                for (int i = 0; i < 4; i++)
                {
                    //int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 16 : 24));
                    int ops = i * 8;

                    n[i * 10 + 2] = (byte)(ym2151Register[0x40 + ops + ch] & 0x0f);//ML
                    int dt = ((ym2151Register[0x40 + ops + ch] & 0x70) >> 4);//DT
                    // 0>3  1>4  2>5  3>6  4>3  5>2  6>1  7>0
                    dt = (dt < 4) ? (dt + 3) : (7 - dt);
                    n[i * 10 + 3] = (byte)dt;
                    n[i * 10 + 4] = (byte)(tl[i]);//TL
                    n[i * 10 + 5] = (byte)((ym2151Register[0x80 + ops + ch] & 0xc0) >> 6);//KS
                    n[i * 10 + 6] = (byte)(ym2151Register[0x80 + ops + ch] & 0x1f); //AR
                    n[i * 10 + 7] = (byte)(ym2151Register[0xa0 + ops + ch] & 0x1f); //DR
                    n[i * 10 + 8] = (byte)(ym2151Register[0xc0 + ops + ch] & 0x1f); //SR
                    n[i * 10 + 9] = (byte)(ym2151Register[0xe0 + ops + ch] & 0x0f); //RR
                    n[i * 10 + 10] = (byte)((ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4);//SL
                    n[i * 10 + 11] = 0;
                }

            }

            SaveFileDialog sfd = new()
            {
                FileName = "音色ファイル.tfi",
                Filter = "TFIファイル(*.tfi)|*.tfi|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "名前を付けて保存",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using FileStream fs = new(
                sfd.FileName,
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write);

            fs.Write(n, 0, n.Length);
        }

        private void getInstChForDMP(EnmChip chip, int ch, int chipID)
        {

            byte[] n = new byte[51];
            n[0] = 0x0b;//FILE_VERSION
            n[2] = 0x01;//Instrument Mode(1=FM)

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n[1] = 0x02;//SYSTEM_GENESIS

                n[3] = (byte)(fmRegister[p][0xb4 + c] & 0x03);//LFO (FMS on YM2612, PMS on YM2151)
                n[4] = (byte)((fmRegister[p][0xb0 + c] & 0x38) >> 3);//FB
                n[5] = (byte)(alg);//ALG
                n[6] = (byte)((fmRegister[p][0xb4 + c] & 0x30) >> 4);//LFO2(AMS on YM2612, AMS on YM2151)

                for (int i = 0; i < 4; i++)
                {
                    //int ops = (i == 0) ? 0 : ((i == 1) ? 4 : ((i == 2) ? 8 : 12));
                    int ops = i * 4;

                    n[i * 11 + 7] = (byte)(fmRegister[p][0x30 + ops + c] & 0x0f);//ML
                    n[i * 11 + 8] = (byte)(tl[i]);//TL
                    n[i * 11 + 9] = (byte)(fmRegister[p][0x50 + ops + c] & 0x1f); //AR
                    n[i * 11 + 10] = (byte)(fmRegister[p][0x60 + ops + c] & 0x1f); //DR
                    n[i * 11 + 11] = (byte)((fmRegister[p][0x80 + ops + c] & 0xf0) >> 4);//SL
                    n[i * 11 + 12] = (byte)(fmRegister[p][0x80 + ops + c] & 0x0f); //RR
                    n[i * 11 + 13] = (byte)((fmRegister[p][0x60 + ops + c] & 0x80) >> 7); //AM
                    n[i * 11 + 14] = (byte)((fmRegister[p][0x50 + ops + c] & 0xc0) >> 6);//KS
                    int dt = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    dt = (dt == 4) ? 0 : dt;
                    // 0>5(-3)  1>6(-2)  2>7(-1)  3>0/4  4>1  5>2  6>3  7>3
                    dt = (dt > 4) ? (dt - 5) : (dt + 3);
                    n[i * 11 + 15] = (byte)(dt & 7);
                    n[i * 11 + 16] = (byte)(fmRegister[p][0x70 + ops + c] & 0x1f); //SR
                    n[i * 11 + 17] = (byte)(fmRegister[p][0x90 + ops + c] & 0x0f);//SSG
                }

            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n[1] = 0x08;//SYSTEM_YM2151

                n[3] = (byte)((ym2151Register[0x38 + ch] & 0x70) >> 4);//LFO (FMS on YM2612, PMS on YM2151)
                n[4] = (byte)((ym2151Register[0x20 + ch] & 0x38) >> 3);//FB
                n[5] = (byte)(alg);//AL
                n[6] = (byte)(ym2151Register[0x38 + ch] & 0x03);//LFO2(AMS on YM2612, AMS on YM2151)

                for (int i = 0; i < 4; i++)
                {
                    //int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 16 : 24));
                    int ops = i * 8;

                    n[i * 11 + 7] = (byte)(ym2151Register[0x40 + ops + ch] & 0x0f);//ML
                    n[i * 11 + 8] = (byte)(tl[i]);//TL
                    n[i * 11 + 9] = (byte)(ym2151Register[0x80 + ops + ch] & 0x1f); //AR
                    n[i * 11 + 10] = (byte)(ym2151Register[0xa0 + ops + ch] & 0x1f); //DR
                    n[i * 11 + 11] = (byte)((ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4);//SL
                    n[i * 11 + 12] = (byte)(ym2151Register[0xe0 + ops + ch] & 0x0f); //RR
                    n[i * 11 + 13] = (byte)((ym2151Register[0xa0 + ops + ch] & 0x80) >> 7); //AM
                    n[i * 11 + 14] = (byte)((ym2151Register[0x80 + ops + ch] & 0xc0) >> 6);//KS
                    int dt = ((ym2151Register[0x40 + ops + ch] & 0x70) >> 4);//DT
                    dt = (dt == 4) ? 0 : dt;
                    // 0>5(-3)  1>6(-2)  2>7(-1)  3>0/4  4>1  5>2  6>3  7>3
                    dt = (dt > 4) ? (dt - 5) : (dt + 3);
                    int dt2 = (byte)((ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6); //DT2
                    n[i * 11 + 15] = (byte)((dt & 0x7) | (dt2 << 4));
                    n[i * 11 + 16] = (byte)(ym2151Register[0xc0 + ops + ch] & 0x1f); //SR
                    n[i * 11 + 17] = 0;
                }

            }

            SaveFileDialog sfd = new()
            {
                FileName = "音色ファイル.dmp",
                Filter = "DMPファイル(*.dmp)|*.dmp|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "名前を付けて保存",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using FileStream fs = new(
                sfd.FileName,
                FileMode.Create,
                FileAccess.Write);

            fs.Write(n, 0, n.Length);
        }

        private void getInstChForRYM2612(EnmChip chip, int ch, int chipID)
        {
            //  
            //  以下のコードを使用、参考にさせていただいております。ありがとうございます！
            //  
            //  Title:
            //      mucom88torym2612
            //  Author:
            //      千霧＠ぶっちぎりP(but80) 様
            //  URL:
            //      https://github.com/but80/mucom88torym2612/
            //      Github
            //        but80/mucom88torym2612
            //  License:
            //      MIT License
            //

            List<string>[] op = new List<string>[4] { new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            int[] muls = new int[] { 0, 1054, 1581, 2635, 3689, 4743, 5797, 6851, 7905, 8959, 10013, 10540, 11594, 12648, 14229, 15000 };
            string buf = "<?xml version = \"1.0\" encoding = \"UTF-8\"?>\r\n";
            buf += "\r\n";
            int alg = 0, fb = 0, ams = 0, pms = 0;

            GD3 gd3 = Audio.GetGD3();
            string patch_Name = "MDPlayer_{0}";
            if (gd3 != null)
            {
                string pn = Common.EscSeqFilter(gd3.TrackName);
                if (string.IsNullOrEmpty(pn)) pn = Common.EscSeqFilter(gd3.TrackNameJ);
                if (!string.IsNullOrEmpty(pn))
                {
                    patch_Name = pn + "_{0}";
                }
            }
            patch_Name = string.Format(patch_Name, DateTime.UtcNow.Ticks);
            buf += $"<RYM2612Params patchName = \"{patch_Name}\" category = \"Piano\" rating = \"3\" type = \"User\" >\r\n";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612)
                    ? Audio.GetFMRegister(chipID)
                    : (chip == EnmChip.YM2608
                        ? Audio.GetYM2608Register(chipID)
                        : (chip == EnmChip.YM2203
                            ? new int[][] { Audio.GetYM2203Register(chipID), null }
                            : Audio.GetYM2610Register(chipID)
                        )
                    );

                alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tls = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tls[0], ref tls[1], ref tls[2], ref tls[3]);

                //alg = (fmRegister[p][0xb0 + c] & 0x07) >> 0;
                fb = (fmRegister[p][0xb0 + c] & 0x38) >> 3;
                ams = (fmRegister[p][0xb4 + c] & 0x30) >> 4;
                pms = (fmRegister[p][0xb4 + c] & 0x07) >> 0;

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    int tl = 127 - tls[i];
                    int vel = 0;
                    int ssg = fmRegister[p][0x90 + ops + c] & 0x0f;
                    ssg = ((ssg & 0x8) == 0) ? 0 : ((ssg & 0x7) + 1);
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}Vel\" value=\"{1}.0\"/>", i + 1, vel));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}TL\" value=\"{1}.0\"/>", i + 1, tl));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}SSGEG\" value=\"{1}.0\"/>", i + 1, ssg));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}RS\" value=\"{1}.0\"/>", i + 1, (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}RR\" value=\"{1}.0\"/>", i + 1, (fmRegister[p][0x80 + ops + c] & 0x0f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}MW\" value=\"0.0\"/>", i + 1));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}MUL\" value=\"{1}.0\"/>", i + 1, muls[(fmRegister[p][0x30 + ops + c] & 0x0f) >> 0]));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}Fixed\" value=\"0.0\"/>", i + 1));
                    int dt = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;
                    dt = (dt >= 4) ? (4 - dt) : dt;
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}DT\" value=\"{1}.0\"/>", i + 1, dt));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}D2R\" value=\"{1}.0\"/>", i + 1, (fmRegister[p][0x70 + ops + c] & 0x1f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}D2L\" value=\"{1}.0\"/>", i + 1, 15 - ((fmRegister[p][0x80 + ops + c] & 0xf0) >> 4)));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}D1R\" value=\"{1}.0\"/>", i + 1, (fmRegister[p][0x60 + ops + c] & 0x1f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}AR\" value=\"{1}.0\"/>", i + 1, (fmRegister[p][0x50 + ops + c] & 0x1f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}AM\" value=\"{1}.0\"/>", i + 1, (fmRegister[p][0x60 + ops + c] & 0x80) >> 7));
                }

            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tls = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tls[0], ref tls[1], ref tls[2], ref tls[3]);

                //alg = (ym2151Register[0x20 + ch] & 0x07) >> 0;
                fb = (ym2151Register[0x20 + ch] & 0x38) >> 3;
                ams = (ym2151Register[0x38 + ch] & 0x03) >> 0;
                pms = (ym2151Register[0x38 + ch] & 0x70) >> 4;

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    int tl = 127 - tls[i];
                    int vel = 0;
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}Vel\" value=\"{1}.0\"/>", i + 1, vel));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}TL\" value=\"{1}.0\"/>", i + 1, tl));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}SSGEG\" value=\"0.0\"/>", i + 1));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}RS\" value=\"{1}.0\"/>", i + 1, (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}RR\" value=\"{1}.0\"/>", i + 1, (ym2151Register[0xe0 + ops + ch] & 0x0f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}MW\" value=\"0.0\"/>", i + 1));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}MUL\" value=\"{1}.0\"/>", i + 1, muls[(ym2151Register[0x40 + ops + ch] & 0x0f) >> 0]));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}Fixed\" value=\"0.0\"/>", i + 1));
                    int dt = (ym2151Register[0x40 + ops + ch] & 0x70) >> 4;
                    dt = (dt >= 4) ? (4 - dt) : dt;
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}DT\" value=\"{1}.0\"/>", i + 1, dt));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}D2R\" value=\"{1}.0\"/>", i + 1, (ym2151Register[0xc0 + ops + ch] & 0x1f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}D2L\" value=\"{1}.0\"/>", i + 1, 15 - ((ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4)));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}D1R\" value=\"{1}.0\"/>", i + 1, (ym2151Register[0xa0 + ops + ch] & 0x1f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}AR\" value=\"{1}.0\"/>", i + 1, (ym2151Register[0x80 + ops + ch] & 0x1f) >> 0));
                    op[i].Add(string.Format("  <PARAM id=\"OP{0}AM\" value=\"{1}.0\"/>", i + 1, (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7));

                }

            }

            for (int i = 0; i < op[0].Count; i++)
            {
                buf += op[3][i] + "\r\n";
                buf += op[2][i] + "\r\n";
                buf += op[1][i] + "\r\n";
                buf += op[0][i] + "\r\n";
            }

            buf += "  <PARAM id=\"volume\" value=\"0.699999988079071\"/>\r\n";//-0.00db
            buf += "  <PARAM id=\"Ladder_Effect\" value=\"0.0\"/>\r\n";
            buf += "  <PARAM id=\"Output_Filtering\" value=\"1.0\"/>\r\n";//Crystal clear
            buf += "  <PARAM id=\"Polyphony\" value=\"6.0\"/>\r\n";
            buf += "  <PARAM id=\"TimerA\" value=\"0.0\"/>\r\n";//RETRIG RATE 1200
            buf += "  <PARAM id=\"Spec_Mode\" value=\"2.0\"/>\r\n";//1.0:float mode  2.0:int mode
            buf += "  <PARAM id=\"Pitchbend_Range\" value=\"2.0\"/>\r\n";
            buf += "  <PARAM id=\"Legato_Retrig\" value=\"0.0\"/>\r\n";
            buf += "  <PARAM id=\"LFO_Speed\" value=\"0.0\"/>\r\n";
            buf += string.Format("  <PARAM id=\"LFO_Enable\" value=\"{0}.0\"/>\r\n", (pms != 0 || ams != 0) ? 1 : 0);
            buf += string.Format("  <PARAM id=\"Feedback\" value=\"{0}.0\"/>\r\n", fb);
            buf += "  <PARAM id=\"FMSMW\" value=\"0.0\"/>\r\n";
            buf += string.Format("  <PARAM id=\"FMS\" value=\"{0}.0\"/>\r\n", pms);
            buf += "  <PARAM id=\"DAC_Prescaler\" value=\"0.0\"/>\r\n";
            buf += string.Format("  <PARAM id=\"Algorithm\" value=\"{0}.0\"/>\r\n", alg + 1);
            buf += string.Format("  <PARAM id=\"AMS\" value=\"{0}.0\"/>\r\n", ams);
            buf += "  <PARAM id=\"masterTune\"/>\r\n";
            buf += "</RYM2612Params>\r\n";



            SaveFileDialog sfd = new()
            {
                FileName = $"{patch_Name}.rym2612",
                Filter = "RYM2612ファイル(*.rym2612)|*.rym2612|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "名前を付けて保存",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using FileStream fs = new(
                sfd.FileName,
                FileMode.Create,
                FileAccess.Write);
            using StreamWriter sw = new(fs);
            sw.Write(buf);

        }

        private void getInstChForOPNI(EnmChip chip, int ch, int chipID)
        {

            byte[] n = new byte[77];
            Array.Clear(n, 0, n.Length);
            byte[] data = Encoding.UTF8.GetBytes("WOPN2-INST");
            Array.Copy(data, 0, n, 0, 10);
            n[10] = 0x00;
            n[11] = 0x00;//0 - melodic, or 1 - percussion
            data = Encoding.UTF8.GetBytes("MDPlayer");
            Array.Copy(data, 0, n, 12, 8);
            n[12 + 32 + 0] = 0x00;//Big-Endian 16-bit signed integer, MIDI key offset value
            n[12 + 32 + 1] = 0x00;
            n[12 + 32 + 2] = 0x00;//8-bit unsigned integer, Percussion instrument key number

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n[12 + 32 + 3] = (byte)(fmRegister[p][0xb0 + c] & 0x3f);//FB & ALG
                n[12 + 32 + 4] = 0x10;//0x00:OPN2  0x10:OPNA

                for (int i = 0; i < 4; i++)
                {
                    //int ops = (i == 0) ? 0 : ((i == 1) ? 4 : ((i == 2) ? 8 : 12));
                    int ops = i * 4;
                    n[i * 7 + 12 + 32 + 5] = (byte)fmRegister[p][0x30 + ops + c];//DT & ML
                    n[i * 7 + 12 + 32 + 6] = (byte)tl[i];//TL
                    n[i * 7 + 12 + 32 + 7] = (byte)fmRegister[p][0x50 + ops + c];//KS & AR
                    n[i * 7 + 12 + 32 + 8] = (byte)fmRegister[p][0x60 + ops + c]; //AM & DR
                    n[i * 7 + 12 + 32 + 9] = (byte)fmRegister[p][0x70 + ops + c]; //SR
                    n[i * 7 + 12 + 32 + 10] = (byte)fmRegister[p][0x80 + ops + c];//SL&RR
                    n[i * 7 + 12 + 32 + 11] = (byte)fmRegister[p][0x90 + ops + c];//SSG
                }

            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n[12 + 32 + 3] = (byte)ym2151Register[0x20 + ch];//FB & ALG
                n[12 + 32 + 4] = 0x10;//0x00:OPN2  0x10:OPNA

                for (int i = 0; i < 4; i++)
                {
                    //int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 16 : 24));
                    int ops = i * 8;
                    n[i * 7 + 12 + 32 + 5] = (byte)ym2151Register[0x40 + ops + ch];//DT & ML
                    n[i * 7 + 12 + 32 + 6] = (byte)tl[i];//TL
                    n[i * 7 + 12 + 32 + 7] = (byte)ym2151Register[0x80 + ops + ch]; //KS & AR
                    n[i * 7 + 12 + 32 + 8] = (byte)ym2151Register[0xa0 + ops + ch]; //AME DR
                    n[i * 7 + 12 + 32 + 9] = (byte)ym2151Register[0xc0 + ops + ch]; //SR
                    n[i * 7 + 12 + 32 + 10] = (byte)ym2151Register[0xe0 + ops + ch];//SL&RR
                    n[i * 7 + 12 + 32 + 11] = 0;//SSG

                    //int dt2 = (byte)((ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6); //DT2
                }

            }

            SaveFileDialog sfd = new()
            {
                FileName = "音色ファイル.opni",
                Filter = "OPNIファイル(*.opni)|*.opni|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "名前を付けて保存",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using FileStream fs = new(
                sfd.FileName,
                FileMode.Create,
                FileAccess.Write);

            fs.Write(n, 0, n.Length);
        }

        private void getInstChForOPLI(EnmChip chip, int ch, int chipID)
        {
            if (chip != EnmChip.YM3812 && chip != EnmChip.YMF262 && chip != EnmChip.YMF278B) return;

            int[][] reg;
            if (chip == EnmChip.YMF262) reg = Audio.GetYMF262Register(chipID);
            else if (chip == EnmChip.YMF278B) reg = Audio.GetYMF278BRegister(chipID);
            else
            {
                int[] r = Audio.GetYM3812Register(chipID);
                reg = new int[1][];
                reg[0] = r;
            }

            byte[] n = new byte[76];
            Array.Clear(n, 0, n.Length);
            byte[] data = Encoding.UTF8.GetBytes("WOPL3-INST");
            Array.Copy(data, 0, n, 0, 10);
            n[10] = 0x00;
            n[11] = 0x02;//Version 16bit-Integer LE
            n[12] = 0x00;
            n[13] = 0x00;//0 - melodic, or 1 - percussion

            data = Encoding.UTF8.GetBytes("MDPlayer");
            Array.Copy(data, 0, n, 14, 8);
            n[14 + 32 + 0] = 0x00;//(mstr)Big-Endian 16-bit signed integer, MIDI key offset value
            n[14 + 32 + 1] = 0x00;
            n[14 + 32 + 2] = 0x00;//(sec)Big-Endian 16-bit signed integer, MIDI key offset value
            n[14 + 32 + 3] = 0x00;
            n[14 + 32 + 4] = 0x00;//8-bit signed integer, MIDI Velocity offset
            n[14 + 32 + 5] = 0x00;//8-bit signed integer, Second voice detune
            n[14 + 32 + 6] = 0x00;//8-bit unsigned integer, Percussion instrument key number

            int[] op = new int[4] { 0, 0, 0, 0 };
            bool isOP4 = false;
            int c = ch;
            if (ch < 6)
            {
                c -= ch % 2;
                op[0] = c / 2;
                op[1] = op[0] + 3;
                op[2] = op[0] + 6;
                op[3] = op[0] + 9;
                isOP4 = chip != EnmChip.YM3812 && ((reg[1][0x04] & (0x1 << c)) != 0);
                if (!isOP4 && ch % 2 != 0)
                {
                    c = ch;
                    op[0] = op[2];
                    op[1] = op[3];
                }
            }
            else if (ch < 9)
            {
                op[0] = ch + 6;
                op[1] = op[0] + 3;
                isOP4 = false;
            }
            else if (ch < 15)
            {
                c -= (ch - 9) % 2;
                op[0] = (c - 9) / 2 + 18;
                op[1] = op[0] + 3;
                op[2] = op[0] + 6;
                op[3] = op[0] + 9;
                isOP4 = (reg[1][0x04] & (0x1 << ((c - 9) + 3))) != 0;
                if (!isOP4 && (ch - 9) % 2 != 0)
                {
                    c = ch;
                    op[0] = op[2];
                    op[1] = op[3];
                }
            }
            else if (ch < 18)
            {
                op[0] = ch + 15;
                op[1] = op[0] + 3;
                isOP4 = false;
            }

            n[14 + 32 + 7] = (byte)(
                (isOP4 ? 1 : 0)
                );

            int p = c / 9;
            //int adr = c % 9;
            int[] chTbl = new int[] { 0, 3, 1, 4, 2, 5, 6, 7, 8 };
            //adr = chTbl[adr];

            for (int i = 0; i < 4; i++)
            {
                op[i] -= (op[i] / 18) * 18;
                op[i] = (op[i] % 6) + 8 * (op[i] / 6);
            }

            //OPLIはop1<->op2  op3<->op4がそれぞれ逆(?)
            for (int i = 0; i < 2; i++)
            {
                (op[i * 2 + 1], op[i * 2]) = (op[i * 2], op[i * 2 + 1]);
            }

            if (!isOP4)
            {
                n[14 + 32 + 8] = (byte)(reg[p][0xc0 + chTbl[c % 9]]);//8-bit unsigned integer, Feedback / Connection
                n[14 + 32 + 9] = 0x00;
            }
            else
            {
                n[14 + 32 + 8] = (byte)(reg[p][0xc0 + chTbl[c % 9]]);//8-bit unsigned integer, Feedback / Connection
                n[14 + 32 + 9] = (byte)(reg[p][0xc0 + chTbl[(c + 1) % 9]]);//8-bit unsigned integer, Feedback / Connection
            }

            for (int i = 0; i < 4; i++)
            {
                if (isOP4 || i < 2)
                {
                    n[14 + 32 + 10 + i * 5] = (byte)reg[p][0x20 + op[i]];//AM/Vib/Env/Ksr/FMult characteristics
                    n[14 + 32 + 11 + i * 5] = (byte)reg[p][0x40 + op[i]];//Key Scale Level / Total level register data
                    n[14 + 32 + 12 + i * 5] = (byte)reg[p][0x60 + op[i]];//Attack / Decay
                    n[14 + 32 + 13 + i * 5] = (byte)reg[p][0x80 + op[i]];//Sustain and Release register data
                    n[14 + 32 + 14 + i * 5] = (byte)reg[p][0xe0 + op[i]];//WS
                }
                else
                {
                    n[14 + 32 + 10 + i * 5] = 0x00;//AM/Vib/Env/Ksr/FMult characteristics
                    n[14 + 32 + 11 + i * 5] = 0x00;//Key Scale Level / Total level register data
                    n[14 + 32 + 12 + i * 5] = 0x00;//Attack / Decay
                    n[14 + 32 + 13 + i * 5] = 0x00;//Sustain and Release register data
                    n[14 + 32 + 14 + i * 5] = 0x00;//WS
                }
            }

            SaveFileDialog sfd = new()
            {
                FileName = "音色ファイル.opli",
                Filter = "OPLIファイル(*.opli)|*.opli|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "名前を付けて保存",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using FileStream fs = new(
                sfd.FileName,
                FileMode.Create,
                FileAccess.Write);

            fs.Write(n, 0, n.Length);
        }

        private void getInstChForVOPM(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "@: n MDPlayer\r\n";
                n += "LFO:  0   0   0   0   0\r\n";
                n += string.Format("CH: 64  {0,2}  {1,2}   0   0 120   0\r\n"
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                    , alg//AL
                );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format(
                        "{0}{1}"
                        , string.Format(
                            "{0}:{1,3} {2,3} {3,3} {4,3} {5,3} "
                            , "M1C1M2C2".Substring(i * 2, 2)
                            , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                            , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                            , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                            , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                            , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        )
                        , string.Format(
                            "{0,3} {1,3} {2,3} {3,3}   0 {4,3}\r\n"
                            , tl[i]//TL
                            , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                            , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                            , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                            , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                        )
                    );
                }
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "@: n MDPlayer\r\n";
                n += "LFO:  0   0   0   0   0\r\n";
                n += string.Format("CH: 64  {0,2}  {1,2}   0   0 120   0\r\n"
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                    , alg //AL
                );

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format(
                        "{0}{1}"
                        , string.Format(
                            "{0}:{1,3} {2,3} {3,3} {4,3} {5,3} "
                            , "M1C1M2C2".Substring(i * 2, 2)
                            , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                            , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                            , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                            , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                            , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        )
                        , string.Format(
                            "{0,3} {1,3} {2,3} {3,3} {4,3} {5,3}\r\n"
                            , tl[i] //TL
                            , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                            , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                            , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                            , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                            , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                        )
                    );
                }
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void getInstChForPMD(EnmChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2203 || chip == EnmChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == EnmChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == EnmChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == EnmChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                int alg = fmRegister[p][0xb0 + c] & 0x07;
                int[] tl = new int[]
                {
                        fmRegister[p][0x40 + 0x0 + c] & 0x7f//TL1
                       ,fmRegister[p][0x40 + 0x8 + c] & 0x7f//TL2
                       ,fmRegister[p][0x40 + 0x4 + c] & 0x7f//TL3
                       ,fmRegister[p][0x40 + 0xc + c] & 0x7f//TL4
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "; nm alg fbl\r\n";
                n += string.Format("@xxx {0:D3} {1:D3}                            =      MDPlayer\r\n"
                    , alg//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
                n += "; ar  dr  sr  rr  sl  tl  ks  ml  dt ams   seg\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format(" {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3} {8:D3} {9:D3} ; {10:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , tl[i]//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                        , fmRegister[p][0x90 + ops + c] & 0x0f//SG
                    );
                }
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);
                int alg = ym2151Register[0x20 + ch] & 0x07;
                int[] tl = new int[]
                {
                        ym2151Register[0x60 + 0x00 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x10 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x08 + ch] & 0x7f//TL1
                       ,ym2151Register[0x60 + 0x18 + ch] & 0x7f//TL1
                };
                GetAdjustTLParam(alg, ref tl[0], ref tl[1], ref tl[2], ref tl[3]);

                n = "; nm alg fbl\r\n";
                n += string.Format("@xxx {0:D3} {1:D3}                            =      MDPlayer\r\n"
                    , alg //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
                n += "; ar  dr  sr  rr  sl  tl  ks  ml  dt ams   seg\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format(" {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3} {8:D3} {9:D3} ; {10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , tl[i] //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                                                                        //, (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                        , 0
                    );
                }
            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void GetAdjustTLParam(int alg, ref int tl1, ref int tl2, ref int tl3, ref int tl4)
        {
            if (!setting.other.AdjustTLParam) return;

            int[] algs = new int[] { 0x8, 0x8, 0x8, 0x8, 0xa, 0xe, 0xe, 0xe };
            int min = int.MaxValue;

            //最小値を調べる
            for (int i = 0; i < 4; i++)
            {
                if ((algs[alg] & (1 << i)) == 0) continue;
                switch (i)
                {
                    case 0:
                        if (tl1 < min) min = tl1;
                        break;
                    case 1:
                        if (tl2 < min) min = tl2;
                        break;
                    case 2:
                        if (tl3 < min) min = tl3;
                        break;
                    case 3:
                        if (tl4 < min) min = tl4;
                        break;
                }
            }

            //補正実施
            for (int i = 0; i < 4; i++)
            {
                if ((algs[alg] & (1 << i)) == 0) continue;
                switch (i)
                {
                    case 0:
                        tl1 -= min;
                        break;
                    case 1:
                        tl2 -= min;
                        break;
                    case 2:
                        tl3 -= min;
                        break;
                    case 3:
                        tl4 -= min;
                        break;
                }
            }

        }


        //SendMessageで送る構造体（Unicode文字列送信に最適化したパターン）
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        //SendMessage（データ転送）
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public const int WM_COPYDATA = 0x004A;
        public const int WM_PASTE = 0x0302;

        //SendMessageを使ってプロセス間通信で文字列を渡す
        void SendString(IntPtr targetWindowHandle, string str)
        {
            COPYDATASTRUCT cds = new()
            {
                dwData = IntPtr.Zero,
                lpData = str,
                cbData = str.Length * sizeof(char)
            };
            //受信側ではlpDataの文字列を(cbData/2)の長さでstring.Substring()する

            IntPtr myWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            _ = SendMessage(targetWindowHandle, WM_COPYDATA, myWindowHandle, ref cds);
        }

        public void WindowsMessage(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                string sParam = ReceiveString(m);
                try
                {
                    PlayArgs(sParam);
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    //メッセージによる読み込み失敗の場合は何も表示しない
                    //                    MessageBox.Show("ファイルの読み込みに失敗しました。");
                }
            }

        }

        private void PlayArgs(string arg)
        {
            string fname = "";
            bool addPL = true;
            List<string> args = new();
            string a = "";
            for (int i = 0; i < arg.Length; i++)
            {
                char ch = arg[i];
                if (ch != '"') continue;
                i++;
                a = "";
                for (int j = i; j < arg.Length; j++)
                {
                    ch = arg[j];
                    if (ch != '"')
                    {
                        a += ch;
                        continue;
                    }
                    args.Add(a);
                    i = j;
                    break;
                }
            }
            if (string.IsNullOrEmpty(a)) args.Add(a);

            foreach (string b in args)
            {
                if (string.IsNullOrEmpty(b)) continue;
                string c = b.Trim();
                if (string.IsNullOrEmpty(c)) continue;
                if (c[0] == '-')
                {
                    string d = c[1..].ToUpper();
                    if (d.Contains("PL", StringComparison.CurrentCulture))
                    {
                        addPL = true;
                        if (d.Length > 2 && d[2] == '-') addPL = false;
                    }

                    continue;
                }
                fname = c.Trim();
            }

            frmPlayList.Stop();

            PlayList pl = frmPlayList.getPlayList();
            if (pl.LstMusic.Count < 1 || pl.LstMusic[^1].fileName != fname)
            {
                if (addPL) frmPlayList.getPlayList().AddFile(fname);
                //frmPlayList.AddList(sParam);
            }

            if (!loadAndPlay(0, 0, fname))
            {
                frmPlayList.Stop();
                Request req = new(EnmRequest.Stop);
                OpeManager.RequestToAudio(req);
                //Audio.Stop();
                return;
            }

            frmPlayList.setStart(-1);
            oldParam = new MDChipParams();

            frmPlayList.Play();
        }

        //メッセージ処理
        protected override void WndProc(ref Message m)
        {
            int WM_NCLBUTTONDBLCLK = 0xA3;
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                TopMost = !TopMost;
                if (TopMost)
                    this.Icon = Resources.FeliTop;
                else
                    this.Icon = Resources.Feli128;
            }

            WindowsMessage(ref m);
            base.WndProc(ref m);
        }

        //SendString()で送信された文字列を取り出す
        string ReceiveString(Message m)
        {
            string str = null;
            try
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                str = cds.lpData;
                str = str[..(cds.cbData / 2)];
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                str = null;
            }
            return str;
        }

        public static Process GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (Process checkProcess in allProcesses)
            {
                // 自分自身のプロセスIDは無視する
                if (checkProcess.Id != curProcess.Id)
                {
                    // プロセスのフルパス名を比較して同じアプリケーションか検証
                    if (String.Compare(
                        checkProcess.MainModule.FileName,
                        curProcess.MainModule.FileName, true) == 0)
                    {
                        // 同じフルパス名のプロセスを取得
                        return checkProcess;
                    }
                }
            }

            // 同じアプリケーションのプロセスが見つからない！
            return null;
        }



        public bool loadAndPlay(int m, int songNo, string fn, string zfn = null)
        {
            try
            {
                if (Audio.FlgReinit) flgReinit = true;
                if (setting.other.InitAlways) flgReinit = true;
                Reinit(setting);
                Audio.FlgReinit = false;

                if (Audio.IsPaused)
                {
                    Audio.Pause();
                }

                string playingFileName = fn;
                string playingArcFileName = "";
                EnmFileFormat format = EnmFileFormat.unknown;
                List<Tuple<string, byte[]>> extFile = null;

                if (zfn == null || zfn == "")
                {
                    try
                    {
                        srcBuf = GetAllBytes(fn, out format);
                    }
                    catch
                    {
                        srcBuf = null;
                    }
                    extFile = getExtendFile(fn, srcBuf, format);
                }
                else
                {

                    playingArcFileName = zfn;

                    if (Path.GetExtension(zfn).ToUpper() == ".ZIP")
                    {
                        using ZipArchive archive = ZipFile.OpenRead(zfn);
                        ZipArchiveEntry entry = archive.GetEntry(fn);
                        string arcFn = "";

                        format = Common.CheckExt(fn);
                        if (format != EnmFileFormat.unknown)
                        {
                            srcBuf = getBytesFromZipFile(entry, out arcFn);
                            if (arcFn != "") playingFileName = arcFn;
                            extFile = getExtendFile(fn, srcBuf, format, archive);
                        }
                    }
                    else
                    {
                        format = Common.CheckExt(fn);
                        if (format != EnmFileFormat.unknown)
                        {
                            UnlhaWrap.UnlhaCmd cmd = new();
                            srcBuf = cmd.GetFileByte(zfn, fn);
                            playingFileName = fn;
                            extFile = getExtendFile(fn, srcBuf, format, new Tuple<string, string>(zfn, fn));
                        }
                    }
                }

                if (Path.GetExtension(playingFileName).ToUpper() == ".MDX")
                {
                    if (setting.outputDevice.SampleRate != 44100)
                    {
                        MessageBox.Show("MDXファイルを再生する場合はサンプリングレートを44.1kHzに設定してください。", "MDPlayer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                //再生前に音量のバランスを設定する
                LoadPresetMixerBalance(playingFileName, playingArcFileName, format);

                Audio.SetVGMBuffer(format, srcBuf, playingFileName, playingArcFileName, m, songNo, extFile);
                newParam.ym2612[0].fileFormat = format;
                newParam.ym2612[1].fileFormat = format;

                if (srcBuf != null)
                {
                    this.Invoke((Action)Playdata);
                    if (Audio.ErrMsg != "") return false;
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                srcBuf = null;
                MessageBox.Show(string.Format("ファイルの読み込みに失敗しました。\r\nメッセージ={0}", ex.Message), "MDPlayer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public bool bufferPlay(byte[] buf, string fullPath)
        {
            try
            {
                if (Audio.FlgReinit) flgReinit = true;
                if (setting.other.InitAlways) flgReinit = true;
                Reinit(setting);
                Audio.FlgReinit = false;

                if (Audio.IsPaused)
                {
                    Audio.Pause();
                }

                string playingFileName = fullPath;
                string playingArcFileName = "";
                EnmFileFormat format = EnmFileFormat.unknown;
                List<Tuple<string, byte[]>> extFile = null;
                format = Common.CheckExt(fullPath,buf);
                srcBuf = buf;

                //再生前に音量のバランスを設定する
                LoadPresetMixerBalance(playingFileName, playingArcFileName, format);

                Audio.SetVGMBuffer(format, srcBuf, playingFileName, playingArcFileName, 0, 0, extFile);
                newParam.ym2612[0].fileFormat = format;
                newParam.ym2612[1].fileFormat = format;

                if (srcBuf != null)
                {
                    this.Invoke((Action)Playdata);
                    if (Audio.ErrMsg != "") return false;
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                srcBuf = null;
                MessageBox.Show(string.Format("ファイルの読み込みに失敗しました。\r\nメッセージ={0}", ex.Message), "MDPlayer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            frmPlayList.Play();
            return true;
        }

        private List<Tuple<string, byte[]>> getExtendFile(string fn, byte[] srcBuf, EnmFileFormat format, object archive = null)
        {
            List<Tuple<string, byte[]>> ret = new();
            byte[] buf;
            switch (format)
            {
                case EnmFileFormat.RCP:
                    string CM6, GSD, GSD2;
                    RCP.getControlFileName(srcBuf, out CM6, out GSD, out GSD2);
                    if (!string.IsNullOrEmpty(CM6))
                    {
                        buf = getExtendFileAllBytes(fn, CM6, archive);
                        if (buf != null) ret.Add(new Tuple<string, byte[]>(".CM6", buf));
                    }
                    if (!string.IsNullOrEmpty(GSD))
                    {
                        buf = getExtendFileAllBytes(fn, GSD, archive);
                        if (buf != null) ret.Add(new Tuple<string, byte[]>(".GSD", buf));
                    }
                    if (!string.IsNullOrEmpty(GSD2))
                    {
                        buf = getExtendFileAllBytes(fn, GSD2, archive);
                        if (buf != null) ret.Add(new Tuple<string, byte[]>(".GSD", buf));
                    }
                    break;
                case EnmFileFormat.MDR:
                    buf = getExtendFileAllBytes(fn, System.IO.Path.GetFileNameWithoutExtension(fn) + ".PCM", archive);
                    if (buf != null) ret.Add(new Tuple<string, byte[]>(".PCM", buf));
                    break;
                case EnmFileFormat.MDX:
                    string PDX;
                    Driver.MXDRV.MXDRV.GetPDXFileName(srcBuf, out PDX);
                    if (!string.IsNullOrEmpty(PDX))
                    {
                        buf = getExtendFileAllBytes(fn, PDX, archive);
                        buf ??= getExtendFileAllBytes(fn, PDX + ".PDX", archive);
                        if (buf != null) ret.Add(new Tuple<string, byte[]>(".PDX", buf));
                    }
                    break;
                case EnmFileFormat.MND:
                    uint hs = (uint)((srcBuf[0x06] << 8) + srcBuf[0x07]);
                    uint pcmptr = (uint)((srcBuf[0x14] << 24) + (srcBuf[0x15] << 16) + (srcBuf[0x16] << 8) + srcBuf[0x17]);
                    if (hs < 0x18) pcmptr = 0;
                    if (pcmptr != 0)
                    {
                        int pcmnum = (srcBuf[pcmptr] << 8) + srcBuf[pcmptr + 1];
                        pcmptr += 2;
                        for (int i = 0; i < pcmnum; i++)
                        {
                            string mndPcmFn = Common.getNRDString(srcBuf, ref pcmptr);
                            buf = getExtendFileAllBytes(fn, mndPcmFn, archive);
                            if (buf != null) ret.Add(new Tuple<string, byte[]>(".PND", buf));
                        }
                    }
                    break;
                default:
                    return null;
            }

            return ret;
        }

        private IEnumerable<string> GetFileSearcePathList(string srcFn)
        {
            yield return System.IO.Path.GetDirectoryName(srcFn);
            string fileSearchPathList = this.setting.FileSearchPathList ?? "";
            IEnumerable<string> fileSerachPaths =
                fileSearchPathList
                    .Split(';')
                    .Where(path => !String.IsNullOrEmpty(path));
            foreach (string path in fileSerachPaths) yield return path;
        }

        private byte[] getExtendFileAllBytes(string srcFn, string extFn, object archive)
        {
            try
            {
                if (archive == null)
                {
                    string trgFn =
                        this.GetFileSearcePathList(srcFn)
                            .Select(dirPath => System.IO.Path.Combine(dirPath, extFn).Trim())
                            .FirstOrDefault(path => System.IO.File.Exists(path));
                    if (trgFn == default) return null;
                    return System.IO.File.ReadAllBytes(trgFn);
                }
                else
                {
                    string trgFn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(srcFn), extFn);
                    trgFn = trgFn.Replace("\\", "/").Trim();
                    if (archive is ZipArchive archive1)
                    {
                        ZipArchiveEntry entry = archive1.GetEntry(trgFn);
                        if (entry == null) return null;
                        string arcFn = "";
                        return getBytesFromZipFile(entry, out arcFn);
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new();
                        return cmd.GetFileByte(((Tuple<string, string>)archive).Item1, trgFn);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public byte[] getBytesFromZipFile(ZipArchiveEntry entry, out string arcFn)
        {
            byte[] buf = null;
            arcFn = "";
            if (entry == null) return null;
            arcFn = entry.FullName;
            using (BinaryReader reader = new(entry.Open()))
            {
                buf = reader.ReadBytes((int)entry.Length);
            }

            if (Common.CheckExt(entry.FullName) == EnmFileFormat.VGM)
            {
                try
                {
                    uint vgm = (UInt32)buf[0] + (UInt32)buf[1] * 0x100 + (UInt32)buf[2] * 0x10000 + (UInt32)buf[3] * 0x1000000;
                    if (vgm != FCC_VGM)
                    {
                        buf = Common.unzipFile(null, entry);
                    }
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    buf = null;
                }
            }

            return buf;
        }

        public void SetChannelMask(EnmChip chip, int chipID, int ch)
        {
            switch (chip)
            {
                case EnmChip.YM2203:
                    if (ch >= 0 && ch < 9)
                    {
                        Audio.SetYM2203Mask(chipID, ch);
                        newParam.ym2203[chipID].channels[ch].mask = true;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2203[chipID].channels[2].mask = true;
                            newParam.ym2203[chipID].channels[6].mask = true;
                            newParam.ym2203[chipID].channels[7].mask = true;
                            newParam.ym2203[chipID].channels[8].mask = true;
                        }
                    }
                    break;
                case EnmChip.YM2413:
                    if (ch >= 0 && ch < 14)
                    {
                        if (newParam.ym2413[chipID].channels[ch].mask == false || newParam.ym2413[chipID].channels[ch].mask == null)
                            Audio.SetYM2413Mask(chipID, ch);
                        else
                            Audio.ResetYM2413Mask(chipID, ch);

                        newParam.ym2413[chipID].channels[ch].mask = !newParam.ym2413[chipID].channels[ch].mask;
                    }
                    break;
                case EnmChip.YM3526:
                    if (ch >= 0 && ch < 14)
                    {
                        if (newParam.ym3526[chipID].channels[ch].mask == false || newParam.ym3526[chipID].channels[ch].mask == null)
                            Audio.SetYM3526Mask(chipID, ch);
                        else
                            Audio.ResetYM3526Mask(chipID, ch);

                        newParam.ym3526[chipID].channels[ch].mask = !newParam.ym3526[chipID].channels[ch].mask;

                    }
                    break;
                case EnmChip.Y8950:
                    if (ch >= 0 && ch < 15)
                    {
                        if (newParam.y8950[chipID].channels[ch].mask == false || newParam.y8950[chipID].channels[ch].mask == null)
                            Audio.SetY8950Mask(chipID, ch);
                        else
                            Audio.ResetY8950Mask(chipID, ch);

                        newParam.y8950[chipID].channels[ch].mask = !newParam.y8950[chipID].channels[ch].mask;

                    }
                    break;
                case EnmChip.YM3812:
                    if (ch >= 0 && ch < 14)
                    {
                        if (newParam.ym3812[chipID].channels[ch].mask == false || newParam.ym3812[chipID].channels[ch].mask == null)
                            Audio.SetYM3812Mask(chipID, ch);
                        else
                            Audio.ResetYM3812Mask(chipID, ch);

                        newParam.ym3812[chipID].channels[ch].mask = !newParam.ym3812[chipID].channels[ch].mask;

                    }
                    break;
                case EnmChip.YMF262:
                    if (ch >= 0 && ch < 23)
                    {
                        if (newParam.ymf262[chipID].channels[ch].mask == false || newParam.ymf262[chipID].channels[ch].mask == null)
                            Audio.SetYMF262Mask(chipID, ch);
                        else
                            Audio.ResetYMF262Mask(chipID, ch);

                        newParam.ymf262[chipID].channels[ch].mask = !newParam.ymf262[chipID].channels[ch].mask;

                    }
                    break;
                case EnmChip.YMF278B:
                    if (ch >= 0 && ch < 47)
                    {
                        if (newParam.ymf278b[chipID].channels[ch].mask == false || newParam.ymf278b[chipID].channels[ch].mask == null)
                            Audio.SetYMF278BMask(chipID, ch);
                        else
                            Audio.ResetYMF278BMask(chipID, ch);

                        newParam.ymf278b[chipID].channels[ch].mask = !newParam.ymf278b[chipID].channels[ch].mask;

                    }
                    break;
                case EnmChip.YM2608:
                    if (ch >= 0 && ch < 14)
                    {
                        Audio.SetYM2608Mask(chipID, ch);
                        newParam.ym2608[chipID].channels[ch].mask = true;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2608[chipID].channels[2].mask = true;
                            newParam.ym2608[chipID].channels[9].mask = true;
                            newParam.ym2608[chipID].channels[10].mask = true;
                            newParam.ym2608[chipID].channels[11].mask = true;
                        }
                    }
                    break;
                case EnmChip.YM2610:
                    if (ch >= 0 && ch < 20)
                    {
                        int c = ch;
                        if (ch >= 14) c = ch - 1;
                        else if (ch == 13) c = 12;

                        Audio.SetYM2610Mask(chipID, ch);
                        newParam.ym2610[chipID].channels[c].mask = true;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2610[chipID].channels[2].mask = true;
                            newParam.ym2610[chipID].channels[9].mask = true;
                            newParam.ym2610[chipID].channels[10].mask = true;
                            newParam.ym2610[chipID].channels[11].mask = true;
                        }
                    }
                    break;
                case EnmChip.YM2612:
                    if (ch >= 0 && ch < 9)
                    {
                        Audio.SetYM2612Mask(chipID, ch);
                        newParam.ym2612[chipID].channels[ch].mask = true;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2612[chipID].channels[2].mask = true;
                            newParam.ym2612[chipID].channels[6].mask = true;
                            newParam.ym2612[chipID].channels[7].mask = true;
                            newParam.ym2612[chipID].channels[8].mask = true;
                        }
                    }
                    break;
                case EnmChip.SN76489:
                    if (newParam.sn76489[chipID].channels[ch].mask == false || newParam.sn76489[chipID].channels[ch].mask == null)
                    {
                        Audio.SetSN76489Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetSN76489Mask(chipID, ch);
                    }
                    newParam.sn76489[chipID].channels[ch].mask = !newParam.sn76489[chipID].channels[ch].mask;
                    break;
                case EnmChip.RF5C164:
                    if (newParam.rf5c164[chipID].channels[ch].mask == false || newParam.rf5c164[chipID].channels[ch].mask == null)
                    {
                        Audio.SetRF5C164Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetRF5C164Mask(chipID, ch);
                    }
                    newParam.rf5c164[chipID].channels[ch].mask = !newParam.rf5c164[chipID].channels[ch].mask;
                    break;
                case EnmChip.RF5C68:
                    if (newParam.rf5c68[chipID].channels[ch].mask == false || newParam.rf5c68[chipID].channels[ch].mask == null)
                    {
                        Audio.SetRF5C68Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetRF5C68Mask(chipID, ch);
                    }
                    newParam.rf5c68[chipID].channels[ch].mask = !newParam.rf5c68[chipID].channels[ch].mask;
                    break;
                case EnmChip.YM2151:
                    if (newParam.ym2151[chipID].channels[ch].mask == false || newParam.ym2151[chipID].channels[ch].mask == null)
                    {
                        Audio.SetYM2151Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetYM2151Mask(chipID, ch);
                    }
                    newParam.ym2151[chipID].channels[ch].mask = !newParam.ym2151[chipID].channels[ch].mask;
                    break;
                case EnmChip.C140:
                    if (newParam.c140[chipID].channels[ch].mask == false || newParam.c140[chipID].channels[ch].mask == null)
                    {
                        Audio.SetC140Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetC140Mask(chipID, ch);
                    }
                    newParam.c140[chipID].channels[ch].mask = !newParam.c140[chipID].channels[ch].mask;
                    break;
                case EnmChip.PPZ8:
                    if (newParam.ppz8[chipID].channels[ch].mask == false || newParam.ppz8[chipID].channels[ch].mask == null)
                    {
                        Audio.SetPPZ8Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetPPZ8Mask(chipID, ch);
                    }
                    newParam.ppz8[chipID].channels[ch].mask = !newParam.ppz8[chipID].channels[ch].mask;
                    break;
                case EnmChip.PCM8:
                    if (newParam.pcm8[chipID].channels[ch].mask == false || newParam.pcm8[chipID].channels[ch].mask == null)
                    {
                        Audio.SetPCM8Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetPCM8Mask(chipID, ch);
                    }
                    newParam.pcm8[chipID].channels[ch].mask = !newParam.pcm8[chipID].channels[ch].mask;
                    break;
                case EnmChip.C352:
                    if (newParam.c352[chipID].channels[ch].mask == false || newParam.c352[chipID].channels[ch].mask == null)
                    {
                        Audio.SetC352Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetC352Mask(chipID, ch);
                    }
                    newParam.c352[chipID].channels[ch].mask = !newParam.c352[chipID].channels[ch].mask;
                    break;
                case EnmChip.SEGAPCM:
                    if (newParam.segaPcm[chipID].channels[ch].mask == false || newParam.segaPcm[chipID].channels[ch].mask == null)
                    {
                        Audio.SetSegaPCMMask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetSegaPCMMask(chipID, ch);
                    }
                    newParam.segaPcm[chipID].channels[ch].mask = !newParam.segaPcm[chipID].channels[ch].mask;
                    break;
                case EnmChip.QSound:
                    if (newParam.qSound[chipID].channels[ch].mask == false || newParam.qSound[chipID].channels[ch].mask == null)
                    {
                        Audio.SetQSoundMask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetQSoundMask(chipID, ch);
                    }
                    newParam.qSound[chipID].channels[ch].mask = !newParam.qSound[chipID].channels[ch].mask;
                    break;
                case EnmChip.AY8910:
                    if (newParam.ay8910[chipID].channels[ch].mask == false || newParam.ay8910[chipID].channels[ch].mask == null)
                    {
                        Audio.SetAY8910Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetAY8910Mask(chipID, ch);
                    }
                    newParam.ay8910[chipID].channels[ch].mask = !newParam.ay8910[chipID].channels[ch].mask;
                    break;
                case EnmChip.HuC6280:
                    if (newParam.huc6280[chipID].channels[ch].mask == false || newParam.huc6280[chipID].channels[ch].mask == null)
                    {
                        Audio.SetHuC6280Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetHuC6280Mask(chipID, ch);
                    }
                    newParam.huc6280[chipID].channels[ch].mask = !newParam.huc6280[chipID].channels[ch].mask;
                    break;
                case EnmChip.OKIM6258:
                    if (newParam.okim6258[chipID].mask == false || newParam.okim6258[chipID].mask == null)
                    {
                        Audio.SetOKIM6258Mask(chipID);
                    }
                    else
                    {
                        Audio.ResetOKIM6258Mask(chipID);
                    }
                    newParam.okim6258[chipID].mask = !newParam.okim6258[chipID].mask;
                    break;
                case EnmChip.OKIM6295:
                    if (newParam.okim6295[chipID].channels[ch].mask == false || newParam.okim6295[chipID].channels[ch].mask == null)
                    {
                        Audio.SetOKIM6295Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetOKIM6295Mask(chipID, ch);
                    }
                    newParam.okim6295[chipID].channels[ch].mask = !newParam.okim6295[chipID].channels[ch].mask;
                    break;
                case EnmChip.NES:
                    if (newParam.nesdmc[chipID].sqrChannels[ch].mask == false || newParam.nesdmc[chipID].sqrChannels[ch].mask == null)
                    {
                        Audio.SetNESMask(chipID, ch);
                    }
                    else
                    {
                        Audio.ResetNESMask(chipID, ch);
                    }
                    newParam.nesdmc[chipID].sqrChannels[ch].mask = !newParam.nesdmc[chipID].sqrChannels[ch].mask;
                    break;
                case EnmChip.DMC:
                    switch (ch)
                    {
                        case 0:
                            if (newParam.nesdmc[chipID].triChannel.mask == false || newParam.nesdmc[chipID].triChannel.mask == null)
                                Audio.SetDMCMask(chipID, ch);
                            else Audio.ResetDMCMask(chipID, ch);
                            newParam.nesdmc[chipID].triChannel.mask = !newParam.nesdmc[chipID].triChannel.mask;
                            break;
                        case 1:
                            if (newParam.nesdmc[chipID].noiseChannel.mask == false || newParam.nesdmc[chipID].noiseChannel.mask == null)
                                Audio.SetDMCMask(chipID, ch);
                            else Audio.ResetDMCMask(chipID, ch);
                            newParam.nesdmc[chipID].noiseChannel.mask = !newParam.nesdmc[chipID].noiseChannel.mask;
                            break;
                        case 2:
                            if (newParam.nesdmc[chipID].dmcChannel.mask == false || newParam.nesdmc[chipID].dmcChannel.mask == null)
                                Audio.SetDMCMask(chipID, ch);
                            else Audio.ResetDMCMask(chipID, ch);
                            newParam.nesdmc[chipID].dmcChannel.mask = !newParam.nesdmc[chipID].dmcChannel.mask;
                            break;
                    }
                    break;
                case EnmChip.FDS:
                    if (newParam.fds[chipID].channel.mask == false || newParam.fds[chipID].channel.mask == null)
                        Audio.SetFDSMask(chipID);
                    else Audio.ResetFDSMask(chipID);
                    newParam.fds[chipID].channel.mask = !newParam.fds[chipID].channel.mask;
                    break;
                case EnmChip.MMC5:
                    switch (ch)
                    {
                        case 0:
                            if (newParam.mmc5[chipID].sqrChannels[0].mask == false || newParam.mmc5[chipID].sqrChannels[ch].mask == null)
                                Audio.SetMMC5Mask(chipID, ch);
                            else Audio.ResetMMC5Mask(chipID, ch);
                            newParam.mmc5[chipID].sqrChannels[0].mask = !newParam.mmc5[chipID].sqrChannels[0].mask;
                            break;
                        case 1:
                            if (newParam.mmc5[chipID].sqrChannels[1].mask == false || newParam.mmc5[chipID].sqrChannels[ch].mask == null)
                                Audio.SetMMC5Mask(chipID, ch);
                            else Audio.ResetMMC5Mask(chipID, ch);
                            newParam.mmc5[chipID].sqrChannels[1].mask = !newParam.mmc5[chipID].sqrChannels[1].mask;
                            break;
                        case 2:
                            if (newParam.mmc5[chipID].pcmChannel.mask == false || newParam.mmc5[chipID].pcmChannel.mask == null)
                                Audio.SetMMC5Mask(chipID, ch);
                            else Audio.ResetMMC5Mask(chipID, ch);
                            newParam.mmc5[chipID].pcmChannel.mask = !newParam.mmc5[chipID].pcmChannel.mask;
                            break;
                    }
                    break;
                case EnmChip.VRC7:
                    if (ch >= 0 && ch < 6)
                    {
                        if (newParam.vrc7[chipID].channels[ch].mask == false || newParam.vrc7[chipID].channels[ch].mask == null)
                            Audio.SetVRC7Mask(chipID, ch);
                        else
                            Audio.ResetVRC7Mask(chipID, ch);

                        newParam.vrc7[chipID].channels[ch].mask = !newParam.vrc7[chipID].channels[ch].mask;
                    }
                    break;
                case EnmChip.K051649:
                    if (ch >= 0 && ch < 5)
                    {
                        if (newParam.k051649[chipID].channels[ch].mask == false || newParam.k051649[chipID].channels[ch].mask == null)
                            Audio.SetK051649Mask(chipID, ch);
                        else
                            Audio.ResetK051649Mask(chipID, ch);

                        newParam.k051649[chipID].channels[ch].mask = !newParam.k051649[chipID].channels[ch].mask;
                    }
                    break;
                case EnmChip.K053260:
                    if (ch >= 0 && ch < 4)
                    {
                        if (newParam.k053260[chipID].channels[ch].mask == false || newParam.k053260[chipID].channels[ch].mask == null)
                            Audio.SetK053260Mask(chipID, ch);
                        else
                            Audio.ResetK053260Mask(chipID, ch);

                        newParam.k053260[chipID].channels[ch].mask = !newParam.k053260[chipID].channels[ch].mask;
                    }
                    break;
                case EnmChip.DMG:
                    if (ch >= 0 && ch < 4)
                    {
                        if (newParam.dmg[chipID].channels[ch].mask == false || newParam.dmg[chipID].channels[ch].mask == null)
                            Audio.SetDMGMask(chipID, ch);
                        else
                            Audio.ResetDMGMask(chipID, ch);

                        newParam.dmg[chipID].channels[ch].mask = !newParam.dmg[chipID].channels[ch].mask;
                    }
                    break;
                case EnmChip.VRC6:
                    if (ch >= 0 && ch < 3)
                    {
                        if (newParam.vrc6[chipID].channels[ch].mask == false || newParam.vrc6[chipID].channels[ch].mask == null)
                            Audio.SetVRC6Mask(chipID, ch);
                        else
                            Audio.ResetVRC6Mask(chipID, ch);

                        newParam.vrc6[chipID].channels[ch].mask = !newParam.vrc6[chipID].channels[ch].mask;
                    }
                    break;
                case EnmChip.N163:
                    if (ch >= 0 && ch < 8)
                    {
                        if (newParam.n106[chipID].channels[ch].mask == false || newParam.n106[chipID].channels[ch].mask == null)
                            Audio.SetN163Mask(chipID, ch);
                        else
                            Audio.ResetN163Mask(chipID, ch);

                        newParam.n106[chipID].channels[ch].mask = !newParam.n106[chipID].channels[ch].mask;
                    }
                    break;
            }
        }

        public void ResetChannelMask(EnmChip chip, int chipID, int ch)
        {
            switch (chip)
            {
                case EnmChip.SN76489:
                    newParam.sn76489[chipID].channels[ch].mask = false;
                    Audio.ResetSN76489Mask(chipID, ch);
                    break;
                case EnmChip.RF5C164:
                    newParam.rf5c164[chipID].channels[ch].mask = false;
                    Audio.ResetRF5C164Mask(chipID, ch);
                    break;
                case EnmChip.RF5C68:
                    newParam.rf5c68[chipID].channels[ch].mask = false;
                    Audio.ResetRF5C68Mask(chipID, ch);
                    break;
                case EnmChip.YM2151:
                    newParam.ym2151[chipID].channels[ch].mask = false;
                    Audio.ResetYM2151Mask(chipID, ch);
                    break;
                case EnmChip.YM2203:
                    if (ch >= 0 && ch < 9)
                    {
                        Audio.ResetYM2203Mask(chipID, ch);
                        newParam.ym2203[chipID].channels[ch].mask = false;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2203[chipID].channels[2].mask = false;
                            newParam.ym2203[chipID].channels[6].mask = false;
                            newParam.ym2203[chipID].channels[7].mask = false;
                            newParam.ym2203[chipID].channels[8].mask = false;
                        }
                    }
                    break;
                case EnmChip.YM2413:
                    newParam.ym2413[chipID].channels[ch].mask = false;
                    Audio.ResetYM2413Mask(chipID, ch);
                    break;
                case EnmChip.VRC7:
                    newParam.vrc7[chipID].channels[ch].mask = false;
                    Audio.ResetVRC7Mask(chipID, ch);
                    break;
                case EnmChip.K053260:
                    newParam.k053260[chipID].channels[ch].mask = false;
                    Audio.ResetK053260Mask(chipID, ch);
                    break;
                case EnmChip.YM2608:
                    if (ch >= 0 && ch < 14)
                    {
                        Audio.ResetYM2608Mask(chipID, ch);
                        newParam.ym2608[chipID].channels[ch].mask = false;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2608[chipID].channels[2].mask = false;
                            newParam.ym2608[chipID].channels[9].mask = false;
                            newParam.ym2608[chipID].channels[10].mask = false;
                            newParam.ym2608[chipID].channels[11].mask = false;
                        }
                    }
                    break;
                case EnmChip.YM2610:
                    if (ch >= 0 && ch <= 20)
                    {
                        int c = ch;
                        if (ch == 12) c = 13;
                        else if (ch == 13) c = 12;
                        else if (ch > 13) c = ch - 1;

                        Audio.ResetYM2610Mask(chipID, ch);
                        newParam.ym2610[chipID].channels[c].mask = false;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2610[chipID].channels[2].mask = false;
                            newParam.ym2610[chipID].channels[9].mask = false;
                            newParam.ym2610[chipID].channels[10].mask = false;
                            newParam.ym2610[chipID].channels[11].mask = false;
                        }
                    }
                    break;
                case EnmChip.YM2612:
                    if (ch >= 0 && ch < 9)
                    {
                        Audio.ResetYM2612Mask(chipID, ch);
                        newParam.ym2612[chipID].channels[ch].mask = false;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2612[chipID].channels[2].mask = false;
                            newParam.ym2612[chipID].channels[6].mask = false;
                            newParam.ym2612[chipID].channels[7].mask = false;
                            newParam.ym2612[chipID].channels[8].mask = false;
                        }
                    }
                    break;
                case EnmChip.YM3526:
                    newParam.ym3526[chipID].channels[ch].mask = false;
                    Audio.ResetYM3526Mask(chipID, ch);
                    break;
                case EnmChip.Y8950:
                    newParam.y8950[chipID].channels[ch].mask = false;
                    Audio.ResetY8950Mask(chipID, ch);
                    break;
                case EnmChip.YM3812:
                    newParam.ym3812[chipID].channels[ch].mask = false;
                    Audio.ResetYM3812Mask(chipID, ch);
                    break;
                case EnmChip.YMF262:
                    newParam.ymf262[chipID].channels[ch].mask = false;
                    Audio.ResetYMF262Mask(chipID, ch);
                    break;
                case EnmChip.YMF278B:
                    newParam.ymf278b[chipID].channels[ch].mask = false;
                    Audio.ResetYMF278BMask(chipID, ch);
                    break;
                case EnmChip.C140:
                    newParam.c140[chipID].channels[ch].mask = false;
                    if (ch < 24) Audio.ResetC140Mask(chipID, ch);
                    break;
                case EnmChip.PPZ8:
                    newParam.ppz8[chipID].channels[ch].mask = false;
                    if (ch < 8) Audio.ResetPPZ8Mask(chipID, ch);
                    break;
                case EnmChip.PCM8:
                    newParam.pcm8[chipID].channels[ch].mask = false;
                    if (ch < 8) Audio.ResetPCM8Mask(chipID, ch);
                    break;
                case EnmChip.C352:
                    newParam.c352[chipID].channels[ch].mask = false;
                    if (ch < 32) Audio.ResetC352Mask(chipID, ch);
                    break;
                case EnmChip.SEGAPCM:
                    newParam.segaPcm[chipID].channels[ch].mask = false;
                    if (ch < 16) Audio.ResetSegaPCMMask(chipID, ch);
                    break;
                case EnmChip.QSound:
                    newParam.qSound[chipID].channels[ch].mask = false;
                    if (ch < 19) Audio.ResetQSoundMask(chipID, ch);
                    break;
                case EnmChip.AY8910:
                    newParam.ay8910[chipID].channels[ch].mask = false;
                    Audio.ResetAY8910Mask(chipID, ch);
                    break;
                case EnmChip.HuC6280:
                    newParam.huc6280[chipID].channels[ch].mask = false;
                    Audio.ResetHuC6280Mask(chipID, ch);
                    break;
                case EnmChip.K051649:
                    newParam.k051649[chipID].channels[ch].mask = false;
                    Audio.ResetK051649Mask(chipID, ch);
                    break;
                case EnmChip.OKIM6258:
                    newParam.okim6258[chipID].mask = false;
                    Audio.ResetOKIM6258Mask(chipID);
                    break;
                case EnmChip.OKIM6295:
                    newParam.okim6295[chipID].channels[ch].mask = false;
                    Audio.ResetOKIM6295Mask(chipID, ch);
                    break;
                case EnmChip.NES:
                    switch (ch)
                    {
                        case 0:
                        case 1:
                            newParam.nesdmc[chipID].sqrChannels[ch].mask = false;
                            Audio.ResetNESMask(chipID, ch);
                            break;
                        case 2:
                            newParam.nesdmc[chipID].triChannel.mask = false;
                            Audio.ResetDMCMask(chipID, 0);
                            break;
                        case 3:
                            newParam.nesdmc[chipID].noiseChannel.mask = false;
                            Audio.ResetDMCMask(chipID, 1);
                            break;
                        case 4:
                            newParam.nesdmc[chipID].dmcChannel.mask = false;
                            Audio.ResetDMCMask(chipID, 2);
                            break;
                    }
                    break;
                case EnmChip.DMC:
                    switch (ch)
                    {
                        case 0:
                            newParam.nesdmc[chipID].triChannel.mask = false;
                            Audio.ResetDMCMask(chipID, 0);
                            break;
                        case 1:
                            newParam.nesdmc[chipID].noiseChannel.mask = false;
                            Audio.ResetDMCMask(chipID, 1);
                            break;
                        case 2:
                            newParam.nesdmc[chipID].dmcChannel.mask = false;
                            Audio.ResetDMCMask(chipID, 2);
                            break;
                    }
                    break;
                case EnmChip.FDS:
                    newParam.fds[chipID].channel.mask = false;
                    Audio.ResetFDSMask(chipID);
                    break;
                case EnmChip.MMC5:
                    switch (ch)
                    {
                        case 0:
                            newParam.mmc5[chipID].sqrChannels[0].mask = false;
                            break;
                        case 1:
                            newParam.mmc5[chipID].sqrChannels[1].mask = false;
                            break;
                        case 2:
                            newParam.mmc5[chipID].pcmChannel.mask = false;
                            break;
                    }
                    Audio.ResetMMC5Mask(chipID, ch);
                    break;
                case EnmChip.DMG:
                    newParam.dmg[chipID].channels[ch].mask = false;
                    Audio.ResetDMGMask(chipID, ch);
                    break;
                case EnmChip.VRC6:
                    newParam.vrc6[chipID].channels[ch].mask = false;
                    Audio.ResetVRC6Mask(chipID, ch);
                    break;
                case EnmChip.N163:
                    newParam.n106[chipID].channels[ch].mask = false;
                    Audio.ResetN163Mask(chipID, ch);
                    break;

            }
        }

        public void ForceChannelMask(EnmChip chip, int chipID, int ch, bool? mask)
        {
            switch (chip)
            {
                case EnmChip.AY8910:
                    if (mask == true)
                        Audio.SetAY8910Mask(chipID, ch);
                    else
                        Audio.ResetAY8910Mask(chipID, ch);
                    newParam.ay8910[chipID].channels[ch].mask = mask;
                    oldParam.ay8910[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.C140:
                    if (mask == true)
                        Audio.SetC140Mask(chipID, ch);
                    else
                        Audio.ResetC140Mask(chipID, ch);
                    newParam.c140[chipID].channels[ch].mask = mask;
                    oldParam.c140[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.C352:
                    if (mask == true)
                        Audio.SetC352Mask(chipID, ch);
                    else
                        Audio.ResetC352Mask(chipID, ch);
                    newParam.c352[chipID].channels[ch].mask = mask;
                    oldParam.c352[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.HuC6280:
                    if (mask == true)
                        Audio.SetHuC6280Mask(chipID, ch);
                    else
                        Audio.ResetHuC6280Mask(chipID, ch);
                    newParam.huc6280[chipID].channels[ch].mask = mask;
                    oldParam.huc6280[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.RF5C164:
                    if (mask == true)
                        Audio.SetRF5C164Mask(chipID, ch);
                    else
                        Audio.ResetRF5C164Mask(chipID, ch);
                    newParam.rf5c164[chipID].channels[ch].mask = mask;
                    oldParam.rf5c164[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.RF5C68:
                    if (mask == true)
                        Audio.SetRF5C68Mask(chipID, ch);
                    else
                        Audio.ResetRF5C68Mask(chipID, ch);
                    newParam.rf5c68[chipID].channels[ch].mask = mask;
                    oldParam.rf5c68[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.SEGAPCM:
                    if (mask == true)
                        Audio.SetSegaPCMMask(chipID, ch);
                    else
                        Audio.ResetSegaPCMMask(chipID, ch);
                    newParam.segaPcm[chipID].channels[ch].mask = mask;
                    oldParam.segaPcm[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.QSound:
                    if (mask == true)
                        Audio.SetQSoundMask(chipID, ch);
                    else
                        Audio.ResetQSoundMask(chipID, ch);
                    newParam.qSound[chipID].channels[ch].mask = mask;
                    oldParam.qSound[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.K053260:
                    if (mask == true)
                        Audio.SetK053260Mask(chipID, ch);
                    else
                        Audio.ResetK053260Mask(chipID, ch);
                    newParam.k053260[chipID].channels[ch].mask = mask;
                    oldParam.k053260[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.YM2151:
                    if (mask == true)
                        Audio.SetYM2151Mask(chipID, ch);
                    else
                        Audio.ResetYM2151Mask(chipID, ch);
                    newParam.ym2151[chipID].channels[ch].mask = mask;
                    oldParam.ym2151[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.YM2203:
                    if (ch >= 0 && ch < 9)
                    {
                        if (mask == true)
                            Audio.SetYM2203Mask(chipID, ch);
                        else
                            Audio.ResetYM2203Mask(chipID, ch);

                        newParam.ym2203[chipID].channels[ch].mask = mask;
                        oldParam.ym2203[chipID].channels[ch].mask = !mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2203[chipID].channels[2].mask = mask;
                            newParam.ym2203[chipID].channels[6].mask = mask;
                            newParam.ym2203[chipID].channels[7].mask = mask;
                            newParam.ym2203[chipID].channels[8].mask = mask;
                            oldParam.ym2203[chipID].channels[2].mask = !mask;
                            oldParam.ym2203[chipID].channels[6].mask = !mask;
                            oldParam.ym2203[chipID].channels[7].mask = !mask;
                            oldParam.ym2203[chipID].channels[8].mask = !mask;
                        }
                    }
                    break;
                case EnmChip.YM2413:
                    if (ch >= 0 && ch < 14)
                    {
                        if (mask == true)
                            Audio.SetYM2413Mask(chipID, ch);
                        else
                            Audio.ResetYM2413Mask(chipID, ch);

                        newParam.ym2413[chipID].channels[ch].mask = mask;
                        oldParam.ym2413[chipID].channels[ch].mask = !mask;
                    }
                    break;
                case EnmChip.YM2608:
                    if (ch >= 0 && ch < 14)
                    {
                        //if (mask)
                        //    Audio.setYM2608Mask(chipID, ch);
                        //else
                        //    Audio.resetYM2608Mask(chipID, ch);

                        newParam.ym2608[chipID].channels[ch].mask = mask;
                        oldParam.ym2608[chipID].channels[ch].mask = !mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2608[chipID].channels[2].mask = mask;
                            newParam.ym2608[chipID].channels[9].mask = mask;
                            newParam.ym2608[chipID].channels[10].mask = mask;
                            newParam.ym2608[chipID].channels[11].mask = mask;
                            oldParam.ym2608[chipID].channels[2].mask = !mask;
                            oldParam.ym2608[chipID].channels[9].mask = !mask;
                            oldParam.ym2608[chipID].channels[10].mask = !mask;
                            oldParam.ym2608[chipID].channels[11].mask = !mask;
                        }
                    }
                    break;
                case EnmChip.YM2610:
                    if (ch >= 0 && ch < 14)
                    {
                        int c = ch;
                        if (ch == 12) c = 13;
                        else if (ch == 13) c = 12;

                        if (mask == true)
                            Audio.SetYM2610Mask(chipID, ch);
                        else
                            Audio.ResetYM2610Mask(chipID, ch);
                        newParam.ym2610[chipID].channels[c].mask = mask;
                        oldParam.ym2610[chipID].channels[c].mask = !mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2610[chipID].channels[2].mask = mask;
                            newParam.ym2610[chipID].channels[9].mask = mask;
                            newParam.ym2610[chipID].channels[10].mask = mask;
                            newParam.ym2610[chipID].channels[11].mask = mask;
                            oldParam.ym2610[chipID].channels[2].mask = !mask;
                            oldParam.ym2610[chipID].channels[9].mask = !mask;
                            oldParam.ym2610[chipID].channels[10].mask = !mask;
                            oldParam.ym2610[chipID].channels[11].mask = !mask;
                        }
                    }
                    break;
                case EnmChip.YM2612:
                    if (ch >= 0 && ch < 9)
                    {
                        if (mask == true)
                            Audio.SetYM2612Mask(chipID, ch);
                        else
                            Audio.ResetYM2612Mask(chipID, ch);

                        newParam.ym2612[chipID].channels[ch].mask = mask;
                        oldParam.ym2612[chipID].channels[ch].mask = null;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2612[chipID].channels[2].mask = mask;
                            newParam.ym2612[chipID].channels[6].mask = mask;
                            newParam.ym2612[chipID].channels[7].mask = mask;
                            newParam.ym2612[chipID].channels[8].mask = mask;
                            oldParam.ym2612[chipID].channels[2].mask = null;
                            oldParam.ym2612[chipID].channels[6].mask = null;
                            oldParam.ym2612[chipID].channels[7].mask = null;
                            oldParam.ym2612[chipID].channels[8].mask = null;
                        }
                    }
                    break;
                case EnmChip.YM3526:
                    if (ch >= 0 && ch < 14)
                    {
                        if (mask == true)
                            Audio.SetYM3526Mask(chipID, ch);
                        else
                            Audio.ResetYM3526Mask(chipID, ch);

                        newParam.ym3526[chipID].channels[ch].mask = mask;
                        oldParam.ym3526[chipID].channels[ch].mask = !mask;
                    }
                    break;
                case EnmChip.Y8950:
                    if (ch >= 0 && ch < 15)
                    {
                        if (mask == true)
                            Audio.SetY8950Mask(chipID, ch);
                        else
                            Audio.ResetY8950Mask(chipID, ch);

                        newParam.y8950[chipID].channels[ch].mask = mask;
                        oldParam.y8950[chipID].channels[ch].mask = !mask;
                    }
                    break;
                case EnmChip.YM3812:
                    if (ch >= 0 && ch < 14)
                    {
                        if (mask == true)
                            Audio.SetYM3812Mask(chipID, ch);
                        else
                            Audio.ResetYM3812Mask(chipID, ch);

                        newParam.ym3812[chipID].channels[ch].mask = mask;
                        oldParam.ym3812[chipID].channels[ch].mask = !mask;
                    }
                    break;
                case EnmChip.YMF262:
                    if (ch >= 0 && ch < 24)
                    {
                        if (mask == true)
                            Audio.SetYMF262Mask(chipID, ch);
                        else
                            Audio.ResetYMF262Mask(chipID, ch);

                        newParam.ymf262[chipID].channels[ch].mask = mask;
                        oldParam.ymf262[chipID].channels[ch].mask = !mask;
                    }
                    break;
                case EnmChip.YMF278B:
                    if (ch >= 0 && ch < 47)
                    {
                        if (mask == true)
                            Audio.SetYMF278BMask(chipID, ch);
                        else
                            Audio.ResetYMF278BMask(chipID, ch);

                        newParam.ymf278b[chipID].channels[ch].mask = mask;
                        oldParam.ymf278b[chipID].channels[ch].mask = !mask;
                    }
                    break;
                case EnmChip.SN76489:
                    if (mask == true)
                        Audio.SetSN76489Mask(chipID, ch);
                    else
                        Audio.ResetSN76489Mask(chipID, ch);
                    newParam.sn76489[chipID].channels[ch].mask = mask;
                    oldParam.sn76489[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.OKIM6295:
                    if (mask == true)
                        Audio.SetOKIM6295Mask(chipID, ch);
                    else
                        Audio.ResetOKIM6295Mask(chipID, ch);
                    newParam.okim6295[chipID].channels[ch].mask = mask;
                    oldParam.okim6295[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.PCM8:
                    if (mask == true)
                        Audio.SetPCM8Mask(chipID, ch);
                    else
                        Audio.ResetPCM8Mask(chipID, ch);
                    newParam.pcm8[chipID].channels[ch].mask = mask;
                    oldParam.pcm8[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.PPZ8:
                    if (mask == true)
                        Audio.SetPPZ8Mask(chipID, ch);
                    else
                        Audio.ResetPPZ8Mask(chipID, ch);
                    newParam.ppz8[chipID].channels[ch].mask = mask;
                    oldParam.ppz8[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.DMG:
                    if (mask == true)
                        Audio.SetDMGMask(chipID, ch);
                    else
                        Audio.ResetDMGMask(chipID, ch);
                    newParam.dmg[chipID].channels[ch].mask = mask;
                    oldParam.dmg[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.VRC6:
                    if (mask == true)
                        Audio.SetVRC6Mask(chipID, ch);
                    else
                        Audio.ResetVRC6Mask(chipID, ch);
                    newParam.vrc6[chipID].channels[ch].mask = mask;
                    oldParam.vrc6[chipID].channels[ch].mask = !mask;
                    break;
                case EnmChip.N163:
                    if (mask == true)
                        Audio.SetN163Mask(chipID, ch);
                    else
                        Audio.ResetN163Mask(chipID, ch);
                    newParam.n106[chipID].channels[ch].mask = mask;
                    oldParam.n106[chipID].channels[ch].mask = !mask;
                    break;
            }
        }

        public void ForceChannelMaskNES(EnmChip chip, int chipID, int ch, MDChipParams.NESDMC[] param)
        {
            if (ch == 0 || ch == 1)
            {
                if (param[chipID].sqrChannels[ch].mask == true)
                {
                    newParam.nesdmc[chipID].sqrChannels[ch].mask = true;
                    Audio.SetNESMask(chipID, ch);
                }
                else
                {
                    newParam.nesdmc[chipID].sqrChannels[ch].mask = false;
                    Audio.ResetNESMask(chipID, ch);
                }
            }
            else if (ch == 2)
            {
                if (param[chipID].triChannel.mask == true)
                {
                    newParam.nesdmc[chipID].triChannel.mask = true;
                    Audio.SetDMCMask(chipID, 0);
                }
                else
                {
                    newParam.nesdmc[chipID].triChannel.mask = false;
                    Audio.ResetDMCMask(chipID, 0);
                }

            }
            else if (ch == 3)
            {
                if (param[chipID].noiseChannel.mask == true)
                {
                    newParam.nesdmc[chipID].noiseChannel.mask = true;
                    Audio.SetDMCMask(chipID, 1);
                }
                else
                {
                    newParam.nesdmc[chipID].noiseChannel.mask = false;
                    Audio.ResetDMCMask(chipID, 1);
                }

            }
            else if (ch == 4)
            {
                if (param[chipID].dmcChannel.mask == true)
                {
                    newParam.nesdmc[chipID].dmcChannel.mask = true;
                    Audio.SetDMCMask(chipID, 2);
                }
                else
                {
                    newParam.nesdmc[chipID].dmcChannel.mask = false;
                    Audio.ResetDMCMask(chipID, 2);
                }

            }

        }

        private void StartMIDIInMonitoring()
        {

            if (setting.midiKbd.MidiInDeviceName == "")
            {
                return;
            }

            if (midiin != null)
            {
                try
                {
                    midiin.Stop();
                    midiin.Dispose();
                    midiin.MessageReceived -= MidiIn_MessageReceived;
                    midiin.ErrorReceived -= midiIn_ErrorReceived;
                    midiin = null;
                }
                catch
                {
                    midiin = null;
                }
            }

            if (midiin == null)
            {
                for (int i = 0; i < MidiIn.NumberOfDevices; i++)
                {
                    if (setting.midiKbd.MidiInDeviceName == MidiIn.DeviceInfo(i).ProductName)
                    {
                        try
                        {
                            midiin = new MidiIn(i);
                            midiin.MessageReceived += MidiIn_MessageReceived;
                            midiin.ErrorReceived += midiIn_ErrorReceived;
                            midiin.Start();
                        }
                        catch
                        {
                            midiin = null;
                        }
                    }
                }
            }

        }

        void midiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            Console.WriteLine(String.Format("Error Time {0} Message 0x{1:X8} Event {2}",
                e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        private void StopMIDIInMonitoring()
        {
            if (midiin != null)
            {
                try
                {
                    midiin.Stop();
                    midiin.Dispose();
                    midiin.MessageReceived -= MidiIn_MessageReceived;
                    midiin.ErrorReceived -= midiIn_ErrorReceived;
                    midiin = null;
                }
                catch
                {
                    midiin = null;
                }
            }
        }

        void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (!setting.midiKbd.UseMIDIKeyboard) return;

            YM2612MIDI.MidiInMessageReceived(e);
        }

        public void Ym2612Midi_ClearNoteLog()
        {
            YM2612MIDI.ClearNoteLog();
        }

        public void Ym2612Midi_ClearNoteLog(int ch)
        {
            YM2612MIDI.ClearNoteLog(ch);
        }

        public void Ym2612Midi_Log2MML(int ch)
        {
            YM2612MIDI.Log2MML(ch);
        }

        public void Ym2612Midi_Log2MML66(int ch)
        {
            YM2612MIDI.Log2MML66(ch);
        }

        public void Ym2612Midi_AllNoteOff()
        {
            YM2612MIDI.AllNoteOff();
        }

        public void Ym2612Midi_SetMode(int m)
        {
            YM2612MIDI.SetMode(m);
        }

        public void Ym2612Midi_SelectChannel(int ch)
        {
            YM2612MIDI.SelectChannel(ch);
        }

        public void Ym2612Midi_SetTonesToSetting()
        {
            YM2612MIDI.SetTonesToSettng();
        }

        public void Ym2612Midi_SetTonesFromSetting()
        {
            YM2612MIDI.SetTonesFromSettng();
        }

        public void Ym2612Midi_SaveTonePallet(string fn, int tp)
        {
            YM2612MIDI.SaveTonePallet(fn, tp, tonePallet);
        }

        public void Ym2612Midi_LoadTonePallet(string fn, int tp)
        {
            YM2612MIDI.LoadTonePallet(fn, tp, tonePallet);
        }

        public void Ym2612Midi_CopyToneToClipboard()
        {
            if (setting.midiKbd.IsMONO)
            {
                YM2612MIDI.CopyToneToClipboard(new int[] { setting.midiKbd.UseMONOChannel });
            }
            else
            {
                List<int> uc = new();
                for (int i = 0; i < setting.midiKbd.UseChannel.Length; i++)
                {
                    if (setting.midiKbd.UseChannel[i]) uc.Add(i);
                }
                YM2612MIDI.CopyToneToClipboard(uc.ToArray());
            }
        }

        public void Ym2612Midi_PasteToneFromClipboard()
        {
            if (setting.midiKbd.IsMONO)
            {
                YM2612MIDI.PasteToneFromClipboard(new int[] { setting.midiKbd.UseMONOChannel });
            }
            else
            {
                List<int> uc = new();
                for (int i = 0; i < setting.midiKbd.UseChannel.Length; i++)
                {
                    if (setting.midiKbd.UseChannel[i]) uc.Add(i);
                }
                YM2612MIDI.PasteToneFromClipboard(uc.ToArray());
            }
        }

        public void Ym2612Midi_CopyToneToClipboard(int ch)
        {
            YM2612MIDI.CopyToneToClipboard(new int[] { ch });
        }

        public void Ym2612Midi_PasteToneFromClipboard(int ch)
        {
            YM2612MIDI.PasteToneFromClipboard(new int[] { ch });
        }

        public void Ym2612Midi_SetSelectInstParam(int ch, int n)
        {
            YM2612MIDI.newParam.ym2612Midi.selectCh = ch;
            YM2612MIDI.newParam.ym2612Midi.selectParam = n;
        }

        public void Ym2612Midi_AddSelectInstParam(int n)
        {
            int p = YM2612MIDI.newParam.ym2612Midi.selectParam;
            p += n;
            if (p > 47) p = 0;
            YM2612MIDI.newParam.ym2612Midi.selectParam = p;
        }

        public void Ym2612Midi_ChangeSelectedParamValue(int n)
        {
            YM2612MIDI.ChangeSelectedParamValue(n);
        }

        private void LoadPresetMixerBalance(string playingFileName, string playingArcFileName, EnmFileFormat format)
        {
            if (!setting.autoBalance.UseThis) return;

            try
            {
                Setting.Balance balance = null;
                string fullPath = Common.settingFilePath;
                fullPath = Path.Combine(fullPath, "MixerBalance");
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                string fn = "";
                string defMbc = "";

                //曲ごとのプリセットを読み込むモード
                if (setting.autoBalance.LoadSongBalance)
                {
                    if (setting.autoBalance.SamePositionAsSongData)
                    {
                        fullPath = Path.GetDirectoryName(playingFileName);
                        if (!string.IsNullOrEmpty(playingArcFileName))
                        {
                            fullPath = Path.GetDirectoryName(playingArcFileName);
                        }
                    }
                    fn = Path.GetFileName(playingFileName);
                    if (!string.IsNullOrEmpty(playingArcFileName))
                    {
                        fn = Path.GetFileName(playingArcFileName);
                    }
                    fn += ".mbc";
                    if (!File.Exists(Path.Combine(fullPath, fn)))
                    {
                        fn = "";
                        fullPath = Common.settingFilePath;
                        fullPath = Path.Combine(fullPath, "MixerBalance");
                    }
                    else
                    {
                        fullPath = Path.Combine(fullPath, fn);
                    }
                }

                //ドライバごとのプリセットを読み込むモード
                if (setting.autoBalance.LoadDriverBalance && fn == "")
                {
                    switch (format)
                    {
                        case EnmFileFormat.VGM:
                            fn = "DriverBalance_VGM.mbc";
                            defMbc = Resources.DefaultVolumeBalance_VGM;
                            break;
                        case EnmFileFormat.XGM:
                        case EnmFileFormat.XGM2:
                            fn = "DriverBalance_XGM.mbc";
                            defMbc = Resources.DefaultVolumeBalance_XGM;
                            break;
                        case EnmFileFormat.ZGM:
                            fn = "DriverBalance_ZGM.mbc";
                            defMbc = Resources.DefaultVolumeBalance_ZGM;
                            break;
                        case EnmFileFormat.HES:
                            fn = "DriverBalance_HES.mbc";
                            defMbc = Resources.DefaultVolumeBalance_HES;
                            break;
                        case EnmFileFormat.NSF:
                            fn = "DriverBalance_NSF.mbc";
                            defMbc = Resources.DefaultVolumeBalance_NSF;
                            break;
                        case EnmFileFormat.NRT:
                            fn = "DriverBalance_NRT.mbc";
                            defMbc = Resources.DefaultVolumeBalance_NRT;
                            break;
                        case EnmFileFormat.MDR:
                            fn = "DriverBalance_MDR.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MDR;
                            break;
                        case EnmFileFormat.MDX:
                            fn = "DriverBalance_MDX.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MDX;
                            break;
                        case EnmFileFormat.MND:
                            fn = "DriverBalance_MND.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MND;
                            break;
                        case EnmFileFormat.S98:
                            fn = "DriverBalance_S98.mbc";
                            defMbc = Resources.DefaultVolumeBalance_S98;
                            break;
                        case EnmFileFormat.SID:
                            fn = "DriverBalance_SID.mbc";
                            defMbc = Resources.DefaultVolumeBalance_SID;
                            break;
                        case EnmFileFormat.MUC:
                            fn = "DriverBalance_MUC.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MUC;
                            break;
                        case EnmFileFormat.MUB:
                            fn = "DriverBalance_MUB.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MUB;
                            break;
                        case EnmFileFormat.M:
                        case EnmFileFormat.MML:
                            fn = "DriverBalance_M.mbc";
                            defMbc = Resources.DefaultVolumeBalance_M;
                            break;
                        case EnmFileFormat.MGS:
                            fn = "DriverBalance_MGS.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MGS;
                            break;
                        case EnmFileFormat.MDL:
                            fn = "DriverBalance_MDL.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MDL;
                            break;
                        case EnmFileFormat.MuSICA:
                        case EnmFileFormat.MuSICA_src:
                            fn = "DriverBalance_MuSICA.mbc";
                            defMbc = Resources.DefaultVolumeBalance_MuSICA;
                            break;
                        case EnmFileFormat.FMP:
                            fn = "DriverBalance_FMP.mbc";
                            defMbc = Resources.DefaultVolumeBalance_FMP;
                            break;
                    }

                    fullPath = Path.Combine(fullPath, fn);

                }

                if (fn == "") return;


                //存在確認。無い場合は作成。
                if (!File.Exists(fullPath) && defMbc != "") File.WriteAllText(fullPath, defMbc);
                //データフォルダに存在するファイルを読み込む
                balance = Setting.Balance.Load(fullPath);

                if (balance == null) return;

                //ミキサーバランス変更処理
                setting.balance = balance;
                frmMixer2?.update();
                Application.DoEvents();

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void ManualSavePresetMixerBalance(bool isDriverBalance, string playingFileName, string playingArcFileName, EnmFileFormat format, Setting.Balance balance)
        {
            if (!setting.autoBalance.UseThis) return;

            try
            {
                string fullPath = Common.settingFilePath;
                fullPath = Path.Combine(fullPath, "MixerBalance");
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                string fn = "";

                if (isDriverBalance)
                {
                    switch (format)
                    {
                        case EnmFileFormat.VGM:
                            fn = "DriverBalance_VGM.mbc";
                            break;
                        case EnmFileFormat.XGM:
                            fn = "DriverBalance_XGM.mbc";
                            break;
                        case EnmFileFormat.HES:
                            fn = "DriverBalance_HES.mbc";
                            break;
                        case EnmFileFormat.NSF:
                            fn = "DriverBalance_NSF.mbc";
                            break;
                        case EnmFileFormat.NRT:
                            fn = "DriverBalance_NRT.mbc";
                            break;
                        case EnmFileFormat.MDR:
                            fn = "DriverBalance_MDR.mbc";
                            break;
                        case EnmFileFormat.MDX:
                            fn = "DriverBalance_MDX.mbc";
                            break;
                        case EnmFileFormat.MND:
                            fn = "DriverBalance_MND.mbc";
                            break;
                        case EnmFileFormat.S98:
                            fn = "DriverBalance_S98.mbc";
                            break;
                        case EnmFileFormat.SID:
                            fn = "DriverBalance_SID.mbc";
                            break;
                        case EnmFileFormat.MUC:
                            fn = "DriverBalance_MUC.mbc";
                            break;
                        case EnmFileFormat.MUB:
                            fn = "DriverBalance_MUB.mbc";
                            break;
                        case EnmFileFormat.MML:
                        case EnmFileFormat.M:
                            fn = "DriverBalance_M.mbc";
                            break;
                        case EnmFileFormat.MGS:
                            fn = "DriverBalance_MGS.mbc";
                            break;
                        case EnmFileFormat.MDL:
                            fn = "DriverBalance_MDL.mbc";
                            break;
                        case EnmFileFormat.MuSICA:
                        case EnmFileFormat.MuSICA_src:
                            fn = "DriverBalance_MuSICA.mbc";
                            break;
                        case EnmFileFormat.FMP:
                            fn = "DriverBalance_FMP.mbc";
                            break;
                    }

                    fullPath = Path.Combine(fullPath, fn);

                }
                else
                {

                }

                balance.Save(fullPath);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public string SaveDriverBalance(Setting.Balance balance)
        {
            PlayList.Music music = frmPlayList.getPlayingSongInfo()
                ?? throw new Exception("演奏情報が取得できませんでした。\r\n演奏中又は演奏完了直後に再度お試しください。");
            EnmFileFormat fmt = music.format;
            ManualSavePresetMixerBalance(true, "", "", fmt, balance);

            return fmt.ToString();
        }

        public PlayList.Music GetPlayingMusicInfo()
        {
            PlayList.Music music = frmPlayList.getPlayingSongInfo();
            return music;
        }

        public static Action<HongliangSoft.Utilities.Gui.KeyboardHookedEventArgs> keyHookMeth = null;

        private void keyboardHook1_KeyboardHooked(object sender, HongliangSoft.Utilities.Gui.KeyboardHookedEventArgs e)
        {
            if (e.UpDown != HongliangSoft.Utilities.Gui.KeyboardUpDown.Up) return;

            if (keyHookMeth != null)
            {
                keyHookMeth(e);
                return;
            }

            if (!setting.keyBoardHook.UseKeyBoardHook) return;

            string k = e.KeyCode.ToString();
            bool Shift = (Control.ModifierKeys & Keys.Shift) != 0;
            bool Ctrl = (Control.ModifierKeys & Keys.Control) != 0;
            bool Alt = (Control.ModifierKeys & Keys.Alt) != 0;
            Setting.KeyBoardHook.HookKeyInfo info;

            info = setting.keyBoardHook.Stop;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Stop();
                return;
            }

            info = setting.keyBoardHook.Pause;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Pause();
                return;
            }

            info = setting.keyBoardHook.Fadeout;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Fadeout();
                return;
            }

            info = setting.keyBoardHook.Prev;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Prev();
                return;
            }

            info = setting.keyBoardHook.Slow;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Slow();
                return;
            }

            info = setting.keyBoardHook.Play;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Play();
                return;
            }

            info = setting.keyBoardHook.Next;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Next();
                return;
            }

            info = setting.keyBoardHook.Fast;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Ff();
                return;
            }

            info = setting.keyBoardHook.Umv;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Up_master_volume();
                return;
            }

            info = setting.keyBoardHook.Dmv;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Down_master_volume();
                return;
            }

            info = setting.keyBoardHook.Rmv;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Reset_master_volume();
                return;
            }

            info = setting.keyBoardHook.Upc;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Up_playlist_cursor();
                return;
            }

            info = setting.keyBoardHook.Dpc;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Down_playlist_cursor();
                return;
            }

            info = setting.keyBoardHook.Ppc;
            if (info.Key == k && info.Shift == Shift && info.Ctrl == Ctrl && info.Alt == Alt)
            {
                Play_playlist_cursor();
                return;
            }

        }


        private void CheckAndSetForm(Form frm)
        {
            Screen s = Screen.FromControl(frm);
            Rectangle rc = new(frm.Location, frm.Size);
            if (s.WorkingArea.Contains(rc))
            {
                frm.Location = rc.Location;
                frm.Size = rc.Size;
                return;
            }
            else
            {
                frm.Location = new System.Drawing.Point(100, 100);
                return;
            }

        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Microsoft.Win32.SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            string[] fn = FileOpen(true);

            if (fn != null)
                AddFileAndPlay(fn);

        }

        private void AddFileAndPlay(string[] fn)
        {
            if (Audio.IsPaused)
            {
                Audio.Pause();
            }

            if (fn.Length == 1)
            {
                frmPlayList.Stop();

                //frmPlayList.AddList(fn[0]);
                frmPlayList.getPlayList().AddFile(fn[0]);

                if (Common.CheckExt(fn[0]) != EnmFileFormat.M3U && Common.CheckExt(fn[0]) != EnmFileFormat.ZIP)
                {
                    if (!loadAndPlay(0, 0, fn[0], "")) return;
                    frmPlayList.setStart(-1);
                }
                oldParam = new MDChipParams();

                frmPlayList.Play();
            }
            else
            {
                frmPlayList.Stop();

                try
                {
                    foreach (string f in fn)
                    {
                        frmPlayList.getPlayList().AddFile(f);
                        //frmPlayList.AddList(f);
                    }
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                }
            }
        }

        private void TsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TsmiPlay_Click(object sender, EventArgs e)
        {
            int n = frmPlayList.getMusicCount();

            if (newButtonMode[9] != 1 || n < 1)
            {
                Play();
                oldParam = new MDChipParams();
            }
            else
            {
                Next();
            }
        }

        private void TsmiStop_Click(object sender, EventArgs e)
        {
            frmPlayList.Stop();
            Stop();
        }

        private void TsmiPause_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void TsmiFadeOut_Click(object sender, EventArgs e)
        {
            Fadeout();
            frmPlayList.Stop();
        }

        private void TsmiSlow_Click(object sender, EventArgs e)
        {
            Slow();
        }

        private void TsmiFf_Click(object sender, EventArgs e)
        {
            Ff();
        }

        private void TsmiNext_Click(object sender, EventArgs e)
        {
            Next();
            oldParam = new MDChipParams();
        }

        private void TsmiPlayMode_Click(object sender, EventArgs e)
        {
            PlayMode();
        }

        private void TsmiOption_Click(object sender, EventArgs e)
        {
            openSetting();
        }

        private void TsmiPlayList_Click(object sender, EventArgs e)
        {
            DispPlayList();
        }

        private void TsmiOpenInfo_Click(object sender, EventArgs e)
        {
            openInfo();
        }

        private void TsmiOpenMixer_Click(object sender, EventArgs e)
        {
            openMixer();
        }

        private void TsmiChangeZoom_Click(object sender, EventArgs e)
        {
            if (sender == tsmiChangeZoomX1) setting.other.Zoom = 1;
            else if (sender == tsmiChangeZoomX2) setting.other.Zoom = 2;
            else if (sender == tsmiChangeZoomX3) setting.other.Zoom = 3;
            else if (sender == tsmiChangeZoomX4) setting.other.Zoom = 4;
            else
                setting.other.Zoom = (setting.other.Zoom == 4) ? 1 : (setting.other.Zoom + 1);
            CheckTsmiChangeZoomItem();
            changeZoom();
        }

        private void CheckTsmiChangeZoomItem()
        {
            tsmiChangeZoomX1.Checked = false;
            tsmiChangeZoomX2.Checked = false;
            tsmiChangeZoomX3.Checked = false;
            tsmiChangeZoomX4.Checked = false;
            if (setting.other.Zoom == 1) tsmiChangeZoomX1.Checked = true;
            else if (setting.other.Zoom == 2) tsmiChangeZoomX2.Checked = true;
            else if (setting.other.Zoom == 3) tsmiChangeZoomX3.Checked = true;
            else if (setting.other.Zoom == 4) tsmiChangeZoomX4.Checked = true;
        }

        private void TsmiVST_Click(object sender, EventArgs e)
        {
            DispVSTList();
        }

        private void TsmiMIDIkbd_Click(object sender, EventArgs e)
        {
            openMIDIKeyboard();
        }

        private void TsmiKBrd_Click(object sender, EventArgs e)
        {
            ShowContextMenu();
        }

        private void RegisterDumpMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == yM2612ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM2612);
            else if (sender == ym2151ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM2151);
            else if (sender == ym2203ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM2203);
            else if (sender == ym2413ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM2413);
            else if (sender == ym2608ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM2608);
            else if (sender == yMF278BToolStripMenuItem) OpenFormRegTest(0, EnmChip.YMF278B);
            else if (sender == yMF262ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YMF262);
            else if (sender == yM2610ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM2610);
            else if (sender == qSoundToolStripMenuItem) OpenFormRegTest(0, EnmChip.QSound);
            else if (sender == segaPCMToolStripMenuItem) OpenFormRegTest(0, EnmChip.SEGAPCM);
            else if (sender == yMZ280BToolStripMenuItem) OpenFormRegTest(0, EnmChip.YMZ280B);
            else if (sender == sN76489ToolStripMenuItem) OpenFormRegTest(0, EnmChip.SN76489);
            else if (sender == aY8910ToolStripMenuItem) OpenFormRegTest(0, EnmChip.AY8910);
            else if (sender == c140ToolStripMenuItem) OpenFormRegTest(0, EnmChip.C140);
            else if (sender == c352ToolStripMenuItem) OpenFormRegTest(0, EnmChip.C352);
            else if (sender == yM3812ToolStripMenuItem) OpenFormRegTest(0, EnmChip.YM3812);
            else if (sender == sIDToolStripMenuItem) OpenFormRegTest(0, EnmChip.SID);
            else OpenFormRegTest(0);
        }

        private void setupControlImages()
        {
            lstOpeButtonEnterImage = new Bitmap[]
{
            ResMng.ImgDic["chSetting"],
            ResMng.ImgDic["chStop"],
            ResMng.ImgDic["chPause"],
            ResMng.ImgDic["chFadeout"],
            ResMng.ImgDic["chPrevious"],
            ResMng.ImgDic["chSlow"],
            ResMng.ImgDic["chPlay"],
            ResMng.ImgDic["chFast"],
            ResMng.ImgDic["chNext"],
            ResMng.ImgDic["chStep"],
            ResMng.ImgDic["chOpenFolder"],
            ResMng.ImgDic["chPlayList"],
            ResMng.ImgDic["chInformation"],
            ResMng.ImgDic["chMixer"],
            ResMng.ImgDic["chKBD"],
            ResMng.ImgDic["chVST"],
            ResMng.ImgDic["chMIDIKBD"],
            ResMng.ImgDic["chZoom"],
            ResMng.ImgDic["chRandom"],
            ResMng.ImgDic["chLoop"],
            ResMng.ImgDic["chLoopOne"]
};
            lstOpeButtonLeaveImage = new Bitmap[]
            {
            ResMng.ImgDic["ccSetting"],
            ResMng.ImgDic["ccStop"],
            ResMng.ImgDic["ccPause"],
            ResMng.ImgDic["ccFadeout"],
            ResMng.ImgDic["ccPrevious"],
            ResMng.ImgDic["ccSlow"],
            ResMng.ImgDic["ccPlay"],
            ResMng.ImgDic["ccFast"],
            ResMng.ImgDic["ccNext"],
            ResMng.ImgDic["ccStep"],
            ResMng.ImgDic["ccOpenFolder"],
            ResMng.ImgDic["ccPlayList"],
            ResMng.ImgDic["ccInformation"],
            ResMng.ImgDic["ccMixer"],
            ResMng.ImgDic["ccKBD"],
            ResMng.ImgDic["ccVST"],
            ResMng.ImgDic["ccMIDIKBD"],
            ResMng.ImgDic["ccZoom"],
            ResMng.ImgDic["ccRandom"],
            ResMng.ImgDic["ccLoop"],
            ResMng.ImgDic["ccLoopOne"]
            };
            lstOpeButtonActiveImage = new Bitmap[]
            {
            ResMng.ImgDic["ciSetting"],
            ResMng.ImgDic["ciStop"],
            ResMng.ImgDic["ciPause"],
            ResMng.ImgDic["ciFadeout"],
            ResMng.ImgDic["ciPrevious"],
            ResMng.ImgDic["ciSlow"],
            ResMng.ImgDic["ciPlay"],
            ResMng.ImgDic["ciFast"],
            ResMng.ImgDic["ciNext"],
            ResMng.ImgDic["ciStep"],
            ResMng.ImgDic["ciOpenFolder"],
            ResMng.ImgDic["ciPlayList"],
            ResMng.ImgDic["ciInformation"],
            ResMng.ImgDic["ciMixer"],
            ResMng.ImgDic["ciKBD"],
            ResMng.ImgDic["ciVST"],
            ResMng.ImgDic["ciMIDIKBD"],
            ResMng.ImgDic["ciZoom"],
            ResMng.ImgDic["ciRandom"],
            ResMng.ImgDic["ciLoop"],
            ResMng.ImgDic["ciLoopOne"]
            };

        }

        private Bitmap[] lstOpeButtonEnterImage;
        private Bitmap[] lstOpeButtonLeaveImage;
        private Bitmap[] lstOpeButtonActiveImage;
        private bool[] lstOpeButtonActive = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };
        private bool[] lstOpeButtonActiveOld = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };
        private Button[] lstOpeButtonControl;

        private void RelocateOpeButton(int zoom)
        {
            opeButtonSetting.Location = new Point((17 + 0) * zoom, 9 * zoom);
            opeButtonStop.Location = new Point((17 + 16 * 1) * zoom, 9 * zoom);
            opeButtonPause.Location = new Point((17 + 16 * 2) * zoom, 9 * zoom);
            opeButtonFadeout.Location = new Point((17 + 16 * 3) * zoom, 9 * zoom);
            opeButtonPrevious.Location = new Point((17 + 16 * 4) * zoom, 9 * zoom);
            opeButtonSlow.Location = new Point((17 + 16 * 5) * zoom, 9 * zoom);
            opeButtonPlay.Location = new Point((17 + 16 * 6) * zoom, 9 * zoom);
            opeButtonFast.Location = new Point((17 + 16 * 7) * zoom, 9 * zoom);
            opeButtonNext.Location = new Point((17 + 16 * 8) * zoom, 9 * zoom);
            opeButtonMode.Location = new Point((17 + 16 * 9) * zoom, 9 * zoom);
            opeButtonOpen.Location = new Point((17 + 16 * 10) * zoom, 9 * zoom);
            opeButtonPlayList.Location = new Point((17 + 16 * 11) * zoom, 9 * zoom);
            opeButtonInformation.Location = new Point((17 + 16 * 12) * zoom, 9 * zoom);
            opeButtonMixer.Location = new Point((17 + 16 * 13) * zoom, 9 * zoom);
            opeButtonKBD.Location = new Point((17 + 16 * 14) * zoom, 9 * zoom);
            opeButtonVST.Location = new Point((17 + 16 * 15) * zoom, 9 * zoom);
            opeButtonMIDIKBD.Location = new Point((17 + 16 * 16) * zoom, 9 * zoom);
            opeButtonZoom.Location = new Point((17 + 16 * 17) * zoom, 9 * zoom);

            RedrawButton(opeButtonSetting, setting.other.Zoom, lstOpeButtonLeaveImage[0]);
            RedrawButton(opeButtonStop, setting.other.Zoom, lstOpeButtonLeaveImage[1]);
            RedrawButton(opeButtonPause, setting.other.Zoom, lstOpeButtonLeaveImage[2]);
            RedrawButton(opeButtonFadeout, setting.other.Zoom, lstOpeButtonLeaveImage[3]);
            RedrawButton(opeButtonPrevious, setting.other.Zoom, lstOpeButtonLeaveImage[4]);
            RedrawButton(opeButtonSlow, setting.other.Zoom, lstOpeButtonLeaveImage[5]);
            RedrawButton(opeButtonPlay, setting.other.Zoom, lstOpeButtonLeaveImage[6]);
            RedrawButton(opeButtonFast, setting.other.Zoom, lstOpeButtonLeaveImage[7]);
            RedrawButton(opeButtonNext, setting.other.Zoom, lstOpeButtonLeaveImage[8]);
            int m = newButtonMode[9] == 0 ? 9 : (newButtonMode[9] == 1 ? 18 : (newButtonMode[9] == 2 ? 19 : 20));
            RedrawButton(opeButtonMode, setting.other.Zoom, lstOpeButtonLeaveImage[m]);
            RedrawButton(opeButtonOpen, setting.other.Zoom, lstOpeButtonLeaveImage[10]);
            RedrawButton(opeButtonPlayList, setting.other.Zoom, lstOpeButtonLeaveImage[11]);
            RedrawButton(opeButtonInformation, setting.other.Zoom, lstOpeButtonLeaveImage[12]);
            RedrawButton(opeButtonMixer, setting.other.Zoom, lstOpeButtonLeaveImage[13]);
            RedrawButton(opeButtonKBD, setting.other.Zoom, lstOpeButtonLeaveImage[14]);
            RedrawButton(opeButtonVST, setting.other.Zoom, lstOpeButtonLeaveImage[15]);
            RedrawButton(opeButtonMIDIKBD, setting.other.Zoom, lstOpeButtonLeaveImage[16]);
            RedrawButton(opeButtonZoom, setting.other.Zoom, lstOpeButtonLeaveImage[17]);
        }

        private void opeButton_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index = int.Parse(btn.Tag.ToString());
            int m = index;
            if (m == 9)
            {
                m = newButtonMode[9] == 0 ? 9 : (newButtonMode[9] == 1 ? 18 : (newButtonMode[9] == 2 ? 19 : 20));
            }
            RedrawButton(btn, setting.other.Zoom, lstOpeButtonEnterImage[m]);
        }

        private void opeButton_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index = int.Parse(btn.Tag.ToString());
            int m = index;
            if (m == 9)
            {
                m = newButtonMode[9] == 0 ? 9 : (newButtonMode[9] == 1 ? 18 : (newButtonMode[9] == 2 ? 19 : 20));
            }

            RedrawButton(btn, setting.other.Zoom, lstOpeButtonActive[m] ? lstOpeButtonActiveImage[m] : lstOpeButtonLeaveImage[m]);
        }

        private void RedrawButton(Button button, int zoom, Bitmap image)
        {
            try
            {
                const int size = 16;
                if (button.Size.Width != size * zoom) button.Size = new Size(size * zoom, size * zoom);

                Bitmap canvas;
                if (button.BackgroundImage != null && button.BackgroundImage.Width == size * zoom)
                    canvas = new Bitmap(button.BackgroundImage);
                else
                    canvas = new Bitmap(button.Width, button.Height);
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.Clear(Color.Black);
                    g.DrawImage(image, 0, 0, size * zoom, size * zoom);
                }

                button.BackgroundImage = canvas;
            }
            catch
            {
                ;
            }
        }

        private void opeButtonSetting_Click(object sender, EventArgs e)
        {
            TsmiOption_Click(null, null);
        }

        private void opeButtonStop_Click(object sender, EventArgs e)
        {
            TsmiStop_Click(null, null);
        }

        private void opeButtonPause_Click(object sender, EventArgs e)
        {
            TsmiPause_Click(null, null);
        }

        private void opeButtonFadeout_Click(object sender, EventArgs e)
        {
            TsmiFadeOut_Click(null, null);
        }

        private void opeButtonPrevious_Click(object sender, EventArgs e)
        {
            Prev();
            oldParam = new MDChipParams();
        }

        private void opeButtonSlow_Click(object sender, EventArgs e)
        {
            TsmiSlow_Click(null, null);
        }

        private void opeButtonPlay_Click(object sender, EventArgs e)
        {
            TsmiPlay_Click(null, null);
        }

        private void opeButtonFast_Click(object sender, EventArgs e)
        {
            TsmiFf_Click(null, null);
        }

        private void opeButtonNext_Click(object sender, EventArgs e)
        {
            TsmiNext_Click(null, null);
        }

        private void opeButtonMode_Click(object sender, EventArgs e)
        {
            TsmiPlayMode_Click(null, null);
            opeButton_MouseEnter(opeButtonMode, null);
        }

        private void opeButtonOpen_Click(object sender, EventArgs e)
        {
            tsmiOpenFile_Click(null, null);
        }

        private void opeButtonPlayList_Click(object sender, EventArgs e)
        {
            TsmiPlayList_Click(null, null);
        }

        private void opeButtonInformation_Click(object sender, EventArgs e)
        {
            TsmiOpenInfo_Click(null, null);
        }

        private void opeButtonMixer_Click(object sender, EventArgs e)
        {
            TsmiOpenMixer_Click(null, null);
        }

        private void opeButtonKBD_Click(object sender, EventArgs e)
        {
            TsmiKBrd_Click(null, null);
        }

        private void opeButtonVST_Click(object sender, EventArgs e)
        {
            TsmiVST_Click(null, null);
        }

        private void opeButtonMIDIKBD_Click(object sender, EventArgs e)
        {
            TsmiMIDIkbd_Click(null, null);
        }

        private void opeButtonZoom_Click(object sender, EventArgs e)
        {
            TsmiChangeZoom_Click(null, null);
        }

        private void UpdateOpeButtonActiveState()
        {
            lstOpeButtonActive[1] = (Audio.IsStopped);//STOP button
            lstOpeButtonActive[2] = !Audio.IsStopped && Audio.IsPaused;//PAUSE button
            lstOpeButtonActive[3] = !Audio.IsStopped && Audio.IsFadeOut;//Fade button
            lstOpeButtonActive[5] = Audio.IsSlow;//Slowbutton
            lstOpeButtonActive[6] = !Audio.IsPaused && (!Audio.IsSlow && !Audio.IsFF && !Audio.IsFadeOut && !Audio.IsStopped);//PLAY button
            lstOpeButtonActive[7] = Audio.IsFF;//FFbutton
        }

        private void tsmiOutputwavFile_Click(object sender, EventArgs e)
        {
            setting.other.WavSwitch = tsmiOutputwavFile.Checked;
        }

    }
}