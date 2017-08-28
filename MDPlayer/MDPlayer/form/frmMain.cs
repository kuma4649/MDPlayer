using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.Midi;
using System.Collections.Generic;
using System.Text;

namespace MDPlayer
{
    public partial class frmMain : Form
    {
        private PictureBox pbRf5c164Screen;
        private DoubleBuffer screen;
        private int pWidth = 0;
        private int pHeight = 0;

        private frmInfo frmInfo = null;
        private frmPlayList frmPlayList = null;
        private frmMixer frmMixer = null;
        private frmVSTeffectList frmVSTeffectList = null;

        private frmMegaCD[] frmMCD = new frmMegaCD[2] { null, null };
        private frmC140[] frmC140 = new frmC140[2] { null, null };
        private frmYM2608[] frmYM2608 = new frmYM2608[2] { null, null };
        private frmYM2151[] frmYM2151 = new frmYM2151[2] { null, null };
        private frmYM2203[] frmYM2203 = new frmYM2203[2] { null, null };
        private frmYM2610[] frmYM2610 = new frmYM2610[2] { null, null };
        private frmYM2612[] frmYM2612 = new frmYM2612[2] { null, null };
        private frmOKIM6258[] frmOKIM6258 = new frmOKIM6258[2] { null, null };
        private frmOKIM6295[] frmOKIM6295 = new frmOKIM6295[2] { null, null };
        private frmSN76489[] frmSN76489 = new frmSN76489[2] { null, null };
        private frmSegaPCM[] frmSegaPCM = new frmSegaPCM[2] { null, null };
        private frmAY8910[] frmAY8910 = new frmAY8910[2] { null, null };
        private frmHuC6280[] frmHuC6280 = new frmHuC6280[2] { null, null };
        private frmYM2413[] frmYM2413 = new frmYM2413[2] { null, null };
        private frmMIDI[] frmMIDI = new frmMIDI[2] { null, null };
        private frmYM2612MIDI frmYM2612MIDI = null;
        private frmMixer2 frmMixer2 = null;

        public MDChipParams oldParam = new MDChipParams();
        private MDChipParams newParam = new MDChipParams();

        private int[] oldButton = new int[18];
        private int[] newButton = new int[18];
        private int[] oldButtonMode = new int[18];
        private int[] newButtonMode = new int[18];

        private bool isRunning = false;
        private bool stopped = false;

        private bool IsInitialOpenFolder = true;

        private static int SamplingRate = 44100;
        private byte[] srcBuf;

        public Setting setting = Setting.Load();
        public TonePallet tonePallet = TonePallet.Load(null);

        private int frameSizeW = 0;
        private int frameSizeH = 0;


        private MidiIn midiin = null;
        private bool forcedExit = false;
        private YM2612MIDI YM2612MIDI = null;

        public frmMain()
        {
            log.ForcedWrite("起動処理開始");
            log.ForcedWrite("frmMain(コンストラクタ):STEP 00");

            InitializeComponent();

            log.ForcedWrite("frmMain(コンストラクタ):STEP 01");

            //引数が指定されている場合のみプロセスチェックを行い、自分と同じアプリケーションが実行中ならばそちらに引数を渡し終了する
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                Process prc = GetPreviousProcess();
                if (prc != null)
                {
                    SendString(prc.MainWindowHandle, Environment.GetCommandLineArgs()[1]);
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

            log.ForcedWrite("起動時のAudio初期化処理開始");

            Audio.Init(setting);

            YM2612MIDI = new YM2612MIDI(this, Audio.mdsMIDI, newParam);

            log.ForcedWrite("起動時のAudio初期化処理完了");

            StartMIDIInMonitoring();

            log.ForcedWrite("frmMain(コンストラクタ):STEP 04");

            log.debug = setting.Debug_DispFrameCounter;

        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            log.ForcedWrite("frmMain_Load:STEP 05");

            if (setting.location.PMain != System.Drawing.Point.Empty)
                this.Location = setting.location.PMain;

            // DoubleBufferオブジェクトの作成

            pbRf5c164Screen = new PictureBox();
            pbRf5c164Screen.Width = 320;
            pbRf5c164Screen.Height = 72;

            log.ForcedWrite("frmMain_Load:STEP 06");

            screen = new DoubleBuffer(pbScreen, Properties.Resources.planeControl, 1);
            screen.setting = setting;
            //oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            screen.screenInitAll();

            log.ForcedWrite("frmMain_Load:STEP 07");

            pWidth = pbScreen.Width;
            pHeight = pbScreen.Height;

            frmPlayList = new frmPlayList(this);
            frmPlayList.Show();
            frmPlayList.Visible = false;
            frmPlayList.Opacity = 1.0;
            frmPlayList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmPlayList.Refresh();

            frmMixer = new frmMixer(setting);
            frmMixer.frmMain = this;
            frmMixer.Show();
            frmMixer.Visible = false;
            frmMixer.Opacity = 1.0;
            frmMixer.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmMixer.Refresh();

            frmVSTeffectList = new frmVSTeffectList(this, setting);
            frmVSTeffectList.Show();
            frmVSTeffectList.Visible = false;
            frmVSTeffectList.Opacity = 1.0;
            //frmVSTeffectList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmVSTeffectList.Refresh();

            if (setting.location.OPlayList) dispPlayList();
            if (setting.location.OInfo) openInfo();
            if (setting.location.OpenRf5c164[0]) tsmiPRF5C164_Click(null, null);
            if (setting.location.OpenC140[0]) tsmiPC140_Click(null, null);
            if (setting.location.OpenYm2151[0]) tsmiPOPM_Click(null, null);
            if (setting.location.OpenYm2608[0]) tsmiPOPNA_Click(null, null);
            if (setting.location.OpenYm2203[0]) tsmiPOPN_Click(null, null);
            if (setting.location.OpenYm2413[0]) tsmiPOPLL_Click(null, null);
            if (setting.location.OpenYm2610[0]) tsmiPOPNB_Click(null, null);
            if (setting.location.OpenYm2612[0]) tsmiPOPN2_Click(null, null);
            if (setting.location.OpenOKIM6258[0]) tsmiPOKIM6258_Click(null, null);
            if (setting.location.OpenOKIM6295[0]) tsmiPOKIM6258_Click(null, null);
            if (setting.location.OpenSN76489[0]) tsmiPDCSG_Click(null, null);
            if (setting.location.OpenSegaPCM[0]) tsmiPSegaPCM_Click(null, null);
            if (setting.location.OpenAY8910[0]) tsmiPAY8910_Click(null, null);
            if (setting.location.OpenHuC6280[0]) tsmiPHuC6280_Click(null, null);
            if (setting.location.OpenRf5c164[1]) tsmiSRF5C164_Click(null, null);
            if (setting.location.OpenC140[1]) tsmiSC140_Click(null, null);
            if (setting.location.OpenYm2151[1]) tsmiSOPM_Click(null, null);
            if (setting.location.OpenYm2608[1]) tsmiSOPNA_Click(null, null);
            if (setting.location.OpenYm2203[1]) tsmiSOPN_Click(null, null);
            if (setting.location.OpenYm2413[1]) tsmiSOPLL_Click(null, null);
            if (setting.location.OpenYm2610[1]) tsmiSOPNB_Click(null, null);
            if (setting.location.OpenYm2612[1]) tsmiSOPN2_Click(null, null);
            if (setting.location.OpenOKIM6258[1]) tsmiSOKIM6258_Click(null, null);
            if (setting.location.OpenOKIM6295[1]) tsmiSOKIM6258_Click(null, null);
            if (setting.location.OpenSN76489[1]) tsmiSDCSG_Click(null, null);
            if (setting.location.OpenSegaPCM[1]) tsmiSSegaPCM_Click(null, null);
            if (setting.location.OpenAY8910[1]) tsmiSAY8910_Click(null, null);
            if (setting.location.OpenHuC6280[1]) tsmiSHuC6280_Click(null, null);

            log.ForcedWrite("frmMain_Load:STEP 08");

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();

        }

        private void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeControl.Width * setting.other.Zoom, frameSizeH + Properties.Resources.planeControl.Height * setting.other.Zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeControl.Width * setting.other.Zoom, frameSizeH + Properties.Resources.planeControl.Height * setting.other.Zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeControl.Width * setting.other.Zoom, frameSizeH + Properties.Resources.planeControl.Height * setting.other.Zoom);
            frmMain_Resize(null, null);

            if (frmMCD[0] != null && !frmMCD[0].isClosed)
            {
                tsmiPRF5C164_Click(null, null);
                tsmiPRF5C164_Click(null, null);
            }

            if (frmC140[0] != null && !frmC140[0].isClosed)
            {
                tsmiPC140_Click(null, null);
                tsmiPC140_Click(null, null);
            }

            if (frmYM2608[0] != null && !frmYM2608[0].isClosed)
            {
                tsmiPOPNA_Click(null, null);
                tsmiPOPNA_Click(null, null);
            }

            if (frmYM2151[0] != null && !frmYM2151[0].isClosed)
            {
                tsmiPOPM_Click(null, null);
                tsmiPOPM_Click(null, null);
            }

            if (frmYM2203[0] != null && !frmYM2203[0].isClosed)
            {
                tsmiPOPN_Click(null, null);
                tsmiPOPN_Click(null, null);
            }

            if (frmYM2413[0] != null && !frmYM2413[0].isClosed)
            {
                tsmiPOPLL_Click(null, null);
                tsmiPOPLL_Click(null, null);
            }

            if (frmYM2610[0] != null && !frmYM2610[0].isClosed)
            {
                tsmiPOPNB_Click(null, null);
                tsmiPOPNB_Click(null, null);
            }

            if (frmYM2612[0] != null && !frmYM2612[0].isClosed)
            {
                tsmiPOPN2_Click(null, null);
                tsmiPOPN2_Click(null, null);
            }

            if (frmOKIM6258[0] != null && !frmOKIM6258[0].isClosed)
            {
                tsmiPOKIM6258_Click(null, null);
                tsmiPOKIM6258_Click(null, null);
            }

            if (frmOKIM6295[0] != null && !frmOKIM6295[0].isClosed)
            {
                tsmiPOKIM6295_Click(null, null);
                tsmiPOKIM6295_Click(null, null);
            }

            if (frmSN76489[0] != null && !frmSN76489[0].isClosed)
            {
                tsmiPDCSG_Click(null, null);
                tsmiPDCSG_Click(null, null);
            }

            if (frmSegaPCM[0] != null && !frmSegaPCM[0].isClosed)
            {
                tsmiPSegaPCM_Click(null, null);
                tsmiPSegaPCM_Click(null, null);
            }

            if (frmAY8910[0] != null && !frmAY8910[0].isClosed)
            {
                tsmiPAY8910_Click(null, null);
                tsmiPAY8910_Click(null, null);
            }

            if (frmHuC6280[0] != null && !frmHuC6280[0].isClosed)
            {
                tsmiPHuC6280_Click(null, null);
                tsmiPHuC6280_Click(null, null);
            }



            if (frmMCD[1] != null && !frmMCD[1].isClosed)
            {
                tsmiSRF5C164_Click(null, null);
                tsmiSRF5C164_Click(null, null);
            }

            if (frmC140[1] != null && !frmC140[1].isClosed)
            {
                tsmiSC140_Click(null, null);
                tsmiSC140_Click(null, null);
            }

            if (frmYM2608[1] != null && !frmYM2608[1].isClosed)
            {
                tsmiSOPNA_Click(null, null);
                tsmiSOPNA_Click(null, null);
            }

            if (frmYM2151[1] != null && !frmYM2151[1].isClosed)
            {
                tsmiSOPM_Click(null, null);
                tsmiSOPM_Click(null, null);
            }

            if (frmYM2203[1] != null && !frmYM2203[1].isClosed)
            {
                tsmiSOPN_Click(null, null);
                tsmiSOPN_Click(null, null);
            }

            if (frmYM2413[1] != null && !frmYM2413[1].isClosed)
            {
                tsmiSOPLL_Click(null, null);
                tsmiSOPLL_Click(null, null);
            }

            if (frmYM2610[1] != null && !frmYM2610[1].isClosed)
            {
                tsmiSOPNB_Click(null, null);
                tsmiSOPNB_Click(null, null);
            }

            if (frmYM2612[1] != null && !frmYM2612[1].isClosed)
            {
                tsmiSOPN2_Click(null, null);
                tsmiSOPN2_Click(null, null);
            }

            if (frmOKIM6258[1] != null && !frmOKIM6258[1].isClosed)
            {
                tsmiSOKIM6258_Click(null, null);
                tsmiSOKIM6258_Click(null, null);
            }

            if (frmOKIM6295[1] != null && !frmOKIM6295[1].isClosed)
            {
                tsmiSOKIM6295_Click(null, null);
                tsmiSOKIM6295_Click(null, null);
            }

            if (frmSN76489[1] != null && !frmSN76489[1].isClosed)
            {
                tsmiSDCSG_Click(null, null);
                tsmiSDCSG_Click(null, null);
            }

            if (frmSegaPCM[1] != null && !frmSegaPCM[1].isClosed)
            {
                tsmiSSegaPCM_Click(null, null);
                tsmiSSegaPCM_Click(null, null);
            }

            if (frmAY8910[1] != null && !frmAY8910[1].isClosed)
            {
                tsmiSAY8910_Click(null, null);
                tsmiSAY8910_Click(null, null);
            }

            if (frmHuC6280[1] != null && !frmHuC6280[1].isClosed)
            {
                tsmiSHuC6280_Click(null, null);
                tsmiSHuC6280_Click(null, null);
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

            if (frmMixer2 != null && !frmMixer2.isClosed)
            {
                openMixer();
                openMixer();
            }


        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            log.ForcedWrite("frmMain_Shown:STEP 09");

            System.Threading.Thread trd = new System.Threading.Thread(screenMainLoop);
            trd.Start();
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                return;
            }

            log.ForcedWrite("frmMain_Shown:STEP 10");

            try
            {

                frmPlayList.Stop();

                PlayList pl = frmPlayList.getPlayList();
                if (pl.lstMusic.Count < 1 || pl.lstMusic[pl.lstMusic.Count - 1].fileName != args[1])
                {
                    frmPlayList.AddList(args[1]);
                }

                if (!loadAndPlay(0,args[1], ""))
                {
                    frmPlayList.Stop();
                    Audio.Stop();
                    return;
                }

                frmPlayList.setStart(-1);

                oldParam = new MDChipParams();
                frmPlayList.Play();

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }

            log.ForcedWrite("frmMain_Shown:STEP 11");
            log.ForcedWrite("起動処理完了");
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            // リサイズ時は再確保

            if (screen != null) screen.Dispose();

            screen = new DoubleBuffer(pbScreen, Properties.Resources.planeControl, setting.other.Zoom);
            screen.setting = setting;
            screen.screenInitAll();
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
            Audio.Close();

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
            for (int chipID = 0; chipID < 2; chipID++)
            {
                setting.location.OpenRf5c164[chipID] = false;
                setting.location.OpenC140[chipID] = false;
                setting.location.OpenYm2151[chipID] = false;
                setting.location.OpenYm2608[chipID] = false;
                setting.location.OpenYm2203[chipID] = false;
                setting.location.OpenYm2413[chipID] = false;
                setting.location.OpenYm2610[chipID] = false;
                setting.location.OpenYm2612[chipID] = false;
                setting.location.OpenOKIM6258[chipID] = false;
                setting.location.OpenOKIM6295[chipID] = false;
                setting.location.OpenSN76489[chipID] = false;
                setting.location.OpenSegaPCM[chipID] = false;
                setting.location.OpenAY8910[chipID] = false;
                setting.location.OpenHuC6280[chipID] = false;
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 04");

            setting.location.PMain = this.Location;
            if (frmInfo != null && !frmInfo.isClosed)
            {
                setting.location.PInfo = frmInfo.Location;
                setting.location.OInfo = true;
            }
            if (frmPlayList != null && !frmPlayList.isClosed)
            {
                setting.location.PPlayList = frmPlayList.Location;
                setting.location.PPlayListWH = new System.Drawing.Point(frmPlayList.Width, frmPlayList.Height);
                setting.location.OPlayList = true;
            }
            if (frmMixer != null && !frmMixer.isClosed)
            {
                setting.location.PMixer = frmMixer.Location;
                setting.location.PMixerWH = new System.Drawing.Point(frmMixer.Width, frmMixer.Height);
                setting.location.OMixer = true;
            }
            if (frmVSTeffectList != null && !frmVSTeffectList.isClosed)
            {
                setting.location.PosVSTeffectList = frmVSTeffectList.Location;
                setting.location.OpenVSTeffectList = true;
            }
            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed)
                {
                    setting.location.PosRf5c164[chipID] = frmMCD[chipID].Location;
                    setting.location.OpenRf5c164[chipID] = true;
                }
                if (frmC140[chipID] != null && !frmC140[chipID].isClosed)
                {
                    setting.location.PosC140[chipID] = frmC140[chipID].Location;
                    setting.location.OpenC140[chipID] = true;
                }
                if (frmYM2151[chipID] != null && !frmYM2151[chipID].isClosed)
                {
                    setting.location.PosYm2151[chipID] = frmYM2151[chipID].Location;
                    setting.location.OpenYm2151[chipID] = true;
                }
                if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed)
                {
                    setting.location.PosYm2608[chipID] = frmYM2608[chipID].Location;
                    setting.location.OpenYm2608[chipID] = true;
                }
                if (frmYM2203[chipID] != null && !frmYM2203[chipID].isClosed)
                {
                    setting.location.PosYm2203[chipID] = frmYM2203[chipID].Location;
                    setting.location.OpenYm2203[chipID] = true;
                }
                if (frmYM2413[chipID] != null && !frmYM2413[chipID].isClosed)
                {
                    setting.location.PosYm2413[chipID] = frmYM2413[chipID].Location;
                    setting.location.OpenYm2413[chipID] = true;
                }
                if (frmYM2610[chipID] != null && !frmYM2610[chipID].isClosed)
                {
                    setting.location.PosYm2610[chipID] = frmYM2610[chipID].Location;
                    setting.location.OpenYm2610[chipID] = true;
                }
                if (frmYM2612[chipID] != null && !frmYM2612[chipID].isClosed)
                {
                    setting.location.PosYm2612[chipID] = frmYM2612[chipID].Location;
                    setting.location.OpenYm2612[chipID] = true;
                }
                if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed)
                {
                    setting.location.PosOKIM6258[chipID] = frmOKIM6258[chipID].Location;
                    setting.location.OpenOKIM6258[chipID] = true;
                }
                if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed)
                {
                    setting.location.PosOKIM6295[chipID] = frmOKIM6295[chipID].Location;
                    setting.location.OpenOKIM6295[chipID] = true;
                }
                if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed)
                {
                    setting.location.PosSN76489[chipID] = frmSN76489[chipID].Location;
                    setting.location.OpenSN76489[chipID] = true;
                }
                if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed)
                {
                    setting.location.PosSegaPCM[chipID] = frmSegaPCM[chipID].Location;
                    setting.location.OpenSegaPCM[chipID] = true;
                }
                if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed)
                {
                    setting.location.PosAY8910[chipID] = frmAY8910[chipID].Location;
                    setting.location.OpenAY8910[chipID] = true;
                }
                if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed)
                {
                    setting.location.PosHuC6280[chipID] = frmHuC6280[chipID].Location;
                    setting.location.OpenHuC6280[chipID] = true;
                }
                if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed)
                {
                    setting.location.PosMIDI[chipID] = frmMIDI[chipID].Location;
                    setting.location.OpenMIDI[chipID] = true;
                }
                if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed)
                {
                    setting.location.PosYm2612MIDI = frmYM2612MIDI.Location;
                    setting.location.OpenYm2612MIDI = true;
                }
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 05");

            try
            {
                frmMixer.Close();
            }
            catch { }

            setting.Save();

            log.ForcedWrite("frmMain_FormClosing:STEP 06");
            log.ForcedWrite("終了処理完了");

        }

        private void pbScreen_MouseMove(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / setting.other.Zoom;
            int py = e.Location.Y / setting.other.Zoom;

            if (py < 24)
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
                if (px >= n * 16 + 32 && px < n * 16 + 48) newButton[n] = 1;
                else newButton[n] = 0;
            }

        }

        private void pbScreen_MouseLeave(object sender, EventArgs e)
        {
            for (int i = 0; i < newButton.Length; i++)
                newButton[i] = 0;
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / setting.other.Zoom;
            int py = e.Location.Y / setting.other.Zoom;

            if (py < 16)
            {
                if (px < 8 * 2) return;
                if (px < 8 * 5 + 4)
                {
                    if (py < 8) tsmiPAY8910_Click(null, null);
                    else tsmiSAY8910_Click(null, null);
                    return;
                }
                if (px < 8 * 7)
                {
                    if (py < 8) tsmiPOPLL_Click(null, null);
                    else tsmiSOPLL_Click(null, null);
                    return;
                }
                if (px < 8 * 9)
                {
                    if (py < 8) tsmiPOPN_Click(null, null);
                    else tsmiSOPN_Click(null, null);
                    return;
                }
                if (px < 8 * 11)
                {
                    if (py < 8) tsmiPOPN2_Click(null, null);
                    else tsmiSOPN2_Click(null, null);
                    return;
                }
                if (px < 8 * 13 + 4)
                {
                    if (py < 8) tsmiPOPNA_Click(null, null);
                    else tsmiSOPNA_Click(null, null);
                    return;
                }
                if (px < 8 * 16)
                {
                    if (py < 8) tsmiPOPNB_Click(null, null);
                    else tsmiSOPNB_Click(null, null);
                    return;
                }
                if (px < 8 * 18 + 4)
                {
                    if (py < 8) tsmiPOPM_Click(null, null);
                    else tsmiSOPM_Click(null, null);
                    return;
                }
                if (px < 8 * 20 + 4)
                {
                    if (py < 8) tsmiPDCSG_Click(null, null);
                    else tsmiSDCSG_Click(null, null);
                    return;
                }
                if (px < 8 * 23)
                {
                    if (py < 8) tsmiPRF5C164_Click(null, null);
                    else tsmiSRF5C164_Click(null, null);
                    return;
                }
                if (px < 8 * 25 + 4)
                {
                    return;
                }
                if (px < 8 * 27 + 4)
                {
                    if (py < 8) tsmiPOKIM6258_Click(null, null);
                    else tsmiSOKIM6258_Click(null, null);
                    return;
                }
                if (px < 8 * 30)
                {
                    if (py < 8) tsmiPOKIM6295_Click(null, null);
                    else tsmiSOKIM6295_Click(null, null);
                    return;
                }
                if (px < 8 * 32 + 4)
                {
                    if (py < 8) tsmiPC140_Click(null, null);
                    else tsmiSC140_Click(null, null);
                    return;
                }
                if (px < 8 * 35)
                {
                    if (py < 8) tsmiPSegaPCM_Click(null, null);
                    else tsmiSSegaPCM_Click(null, null);
                    return;
                }
                if (px < 8 * 37 + 4)
                {
                    if (py < 8) tsmiPHuC6280_Click(null, null);
                    else tsmiSHuC6280_Click(null, null);
                    return;
                }
                return;
            }

            if (py < 24) return;

            // ボタンの判定

            if (px >= 0 * 16 + 32 && px < 1 * 16 + 32)
            {
                openSetting();
                return;
            }

            if (px >= 1 * 16 + 32 && px < 2 * 16 + 32)
            {
                frmPlayList.Stop();
                stop();
                return;
            }

            if (px >= 2 * 16 + 32 && px < 3 * 16 + 32)
            {
                pause();
                return;
            }

            if (px >= 3 * 16 + 32 && px < 4 * 16 + 32)
            {
                fadeout();
                frmPlayList.Stop();
                return;
            }

            if (px >= 4 * 16 + 32 && px < 5 * 16 + 32)
            {
                prev();
                return;
            }

            if (px >= 5 * 16 + 32 && px < 6 * 16 + 32)
            {
                slow();
                return;
            }

            if (px >= 6 * 16 + 32 && px < 7 * 16 + 32)
            {
                play();
                return;
            }

            if (px >= 7 * 16 + 32 && px < 8 * 16 + 32)
            {
                ff();
                return;
            }

            if (px >= 8 * 16 + 32 && px < 9 * 16 + 32)
            {
                next();
                return;
            }

            if (px >= 9 * 16 + 32 && px < 10 * 16 + 32)
            {
                playMode();
                return;
            }

            if (px >= 10 * 16 + 32 && px < 11 * 16 + 32)
            {
                string[] fn = fileOpen(true);

                if (fn != null)
                {
                    if (Audio.isPaused)
                    {
                        Audio.Pause();
                    }

                    if (fn.Length == 1)
                    {
                        frmPlayList.Stop();

                        frmPlayList.AddList(fn[0]);

                        loadAndPlay(0, fn[0], "");
                        frmPlayList.setStart(-1);
                        oldParam = new MDChipParams();

                        frmPlayList.Play();
                    }
                    else
                    {
                        frmPlayList.Stop();

                        try
                        {
                            foreach (string f in fn) frmPlayList.AddList(f);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                        }
                    }
                }

                return;
            }

            if (px >= 11 * 16 + 32 && px < 12 * 16 + 32)
            {
                dispPlayList();
                return;
            }

            if (px >= 12 * 16 + 32 && px < 13 * 16 + 32)
            {
                openInfo();
                return;
            }

            if (px >= 13 * 16 + 32 && px < 14 * 16 + 32)
            {
                //dispMixer();
                openMixer();
                return;
            }

            if (px >= 14 * 16 + 32 && px < 15 * 16 + 32)
            {
                showContextMenu();
                return;
            }

            if (px >= 15 * 16 + 32 && px < 16 * 16 + 32)
            {
                dispVSTList();
                return;
            }

            if (px >= 16 * 16 + 32 && px < 17 * 16 + 32)
            {
                openMIDIKeyboard();
                return;
            }

            if (px >= 17 * 16 + 32 && px < 18 * 16 + 32)
            {
                setting.other.Zoom = (setting.other.Zoom == 3) ? 1 : (setting.other.Zoom + 1);
                changeZoom();
                return;
            }
        }



        private void tsmiPOPN_Click(object sender, EventArgs e)
        {
            OpenFormYM2203(0);
        }

        private void tsmiPOPN2_Click(object sender, EventArgs e)
        {
            OpenFormYM2612(0);
        }

        private void tsmiPOPNA_Click(object sender, EventArgs e)
        {
            OpenFormYM2608(0);
        }

        private void tsmiPOPNB_Click(object sender, EventArgs e)
        {
            OpenFormYM2610(0);
        }

        private void tsmiPOPM_Click(object sender, EventArgs e)
        {
            OpenFormYM2151(0);
        }

        private void tsmiPDCSG_Click(object sender, EventArgs e)
        {
            OpenFormSN76489(0);
        }

        private void tsmiPRF5C164_Click(object sender, EventArgs e)
        {
            OpenFormMegaCD(0);
        }

        private void tsmiPPWM_Click(object sender, EventArgs e)
        {

        }

        private void tsmiPOKIM6258_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6258(0);
        }

        private void tsmiPOKIM6295_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6295(0);
        }

        private void tsmiPC140_Click(object sender, EventArgs e)
        {
            OpenFormC140(0);
        }

        private void tsmiPSegaPCM_Click(object sender, EventArgs e)
        {
            OpenFormSegaPCM(0);
        }

        private void tsmiPAY8910_Click(object sender, EventArgs e)
        {
            OpenFormAY8910(0);
        }

        private void tsmiPOPLL_Click(object sender, EventArgs e)
        {
            OpenFormYM2413(0);
        }

        private void tsmiPHuC6280_Click(object sender, EventArgs e)
        {
            OpenFormHuC6280(0);
        }



        private void tsmiSOPN_Click(object sender, EventArgs e)
        {
            OpenFormYM2203(1);
        }

        private void tsmiSOPN2_Click(object sender, EventArgs e)
        {
            OpenFormYM2612(1);
        }

        private void tsmiSOPNA_Click(object sender, EventArgs e)
        {
            OpenFormYM2608(1);
        }

        private void tsmiSOPNB_Click(object sender, EventArgs e)
        {
            OpenFormYM2610(1);
        }

        private void tsmiSOPM_Click(object sender, EventArgs e)
        {
            OpenFormYM2151(1);
        }

        private void tsmiSDCSG_Click(object sender, EventArgs e)
        {
            OpenFormSN76489(1);
        }

        private void tsmiSRF5C164_Click(object sender, EventArgs e)
        {
            OpenFormMegaCD(1);
        }

        private void tsmiSPWM_Click(object sender, EventArgs e)
        {

        }

        private void tsmiSOKIM6258_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6258(1);
        }

        private void tsmiSOKIM6295_Click(object sender, EventArgs e)
        {
            OpenFormOKIM6295(1);
        }

        private void tsmiSC140_Click(object sender, EventArgs e)
        {
            OpenFormC140(1);
        }

        private void tsmiSSegaPCM_Click(object sender, EventArgs e)
        {
            OpenFormSegaPCM(1);
        }

        private void tsmiSAY8910_Click(object sender, EventArgs e)
        {
            OpenFormAY8910(1);
        }

        private void tsmiSOPLL_Click(object sender, EventArgs e)
        {
            OpenFormYM2413(1);
        }

        private void tsmiSHuC6280_Click(object sender, EventArgs e)
        {
            OpenFormHuC6280(1);
        }

        private void tsmiPMIDI_Click(object sender, EventArgs e)
        {
            OpenFormMIDI(0);
        }

        private void tsmiSMIDI_Click(object sender, EventArgs e)
        {
            OpenFormMIDI(1);
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

            frmMCD[chipID] = new frmMegaCD(this, chipID, setting.other.Zoom);
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

            screen.AddRf5c164(chipID, frmMCD[chipID].pbScreen, Properties.Resources.planeC);
            frmMCD[chipID].Show();
            frmMCD[chipID].update();
            frmMCD[chipID].Text = string.Format("RF5C164 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitRF5C164(chipID);
            oldParam.rf5c164[chipID] = new MDChipParams.RF5C164();
        }

        private void CloseFormMegaCD(int chipID)
        {
            if (frmMCD[chipID] == null) return;

            try
            {
                screen.RemoveRf5c164(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

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

            frmYM2608[chipID] = new frmYM2608(this, chipID, setting.other.Zoom);

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

            screen.AddYM2608(chipID, frmYM2608[chipID].pbScreen, Properties.Resources.planeD);
            frmYM2608[chipID].Show();
            frmYM2608[chipID].update();
            frmYM2608[chipID].Text = string.Format("YM2608 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitYM2608(chipID);
            oldParam.ym2608[chipID] = new MDChipParams.YM2608();
        }

        private void CloseFormYM2608(int chipID)
        {
            if (frmYM2608[chipID] == null) return;

            try
            {
                screen.RemoveYM2608(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmYM2151[chipID] = new frmYM2151(this, chipID, setting.other.Zoom);

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

            screen.AddYM2151(chipID, frmYM2151[chipID].pbScreen, Properties.Resources.planeE);
            frmYM2151[chipID].Show();
            frmYM2151[chipID].update();
            frmYM2151[chipID].Text = string.Format("YM2151 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitYM2151(chipID);
            oldParam.ym2151[chipID] = new MDChipParams.YM2151();
        }

        private void CloseFormYM2151(int chipID)
        {
            if (frmYM2151[chipID] == null) return;

            try
            {
                screen.RemoveYM2151(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmC140[chipID] = new frmC140(this, chipID, setting.other.Zoom);
            screen.AddC140(chipID, frmC140[chipID].pbScreen, Properties.Resources.planeF);

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
            screen.screenInitC140(chipID);
            oldParam.c140[chipID] = new MDChipParams.C140();
        }

        private void CloseFormC140(int chipID)
        {
            if (frmC140[chipID] == null) return;

            try
            {
                screen.RemoveC140(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

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

            frmYM2203[chipID] = new frmYM2203(this, chipID, setting.other.Zoom);

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

            screen.AddYM2203(chipID, frmYM2203[chipID].pbScreen, Properties.Resources.planeYM2203);
            frmYM2203[chipID].Show();
            frmYM2203[chipID].update();
            frmYM2203[chipID].Text = string.Format("YM2203 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitYM2203(chipID);
            oldParam.ym2203[chipID] = new MDChipParams.YM2203();
        }

        private void CloseFormYM2203(int chipID)
        {
            if (frmYM2203[chipID] == null) return;

            try
            {
                screen.RemoveYM2203(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmYM2610[chipID] = new frmYM2610(this, chipID, setting.other.Zoom);

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

            screen.AddYM2610(chipID, frmYM2610[chipID].pbScreen, Properties.Resources.planeYM2610);
            frmYM2610[chipID].Show();
            frmYM2610[chipID].update();
            frmYM2610[chipID].Text = string.Format("YM2610 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitYM2610(chipID);
            oldParam.ym2610[chipID] = new MDChipParams.YM2610();
        }

        private void CloseFormYM2610(int chipID)
        {
            if (frmYM2610[chipID] == null) return;

            try
            {
                screen.RemoveYM2610(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmYM2612[chipID] = new frmYM2612(this, chipID, setting.other.Zoom);

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

            screen.AddYM2612(chipID, frmYM2612[chipID].pbScreen, Properties.Resources.planeYM2612);
            frmYM2612[chipID].Show();
            frmYM2612[chipID].update();
            frmYM2612[chipID].Text = string.Format("YM2612 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitYM2612(chipID);
            oldParam.ym2612[chipID] = new MDChipParams.YM2612();
        }

        private void CloseFormYM2612(int chipID)
        {
            if (frmYM2612[chipID] == null) return;
            try
            {
                screen.RemoveYM2612(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmOKIM6258[chipID] = new frmOKIM6258(this, chipID, setting.other.Zoom);

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

            screen.AddOKIM6258(chipID, frmOKIM6258[chipID].pbScreen, Properties.Resources.planeMSM6258);
            frmOKIM6258[chipID].Show();
            frmOKIM6258[chipID].update();
            frmOKIM6258[chipID].Text = string.Format("OKIM6258 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitOKIM6258(chipID);
        }

        private void CloseFormOKIM6258(int chipID)
        {
            if (frmOKIM6258[chipID] == null) return;

            try
            {
                screen.RemoveOKIM6258(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmOKIM6295[chipID] = new frmOKIM6295(this, chipID, setting.other.Zoom);

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

            screen.AddOKIM6295(chipID, frmOKIM6295[chipID].pbScreen, Properties.Resources.planeMSM6295);
            frmOKIM6295[chipID].Show();
            frmOKIM6295[chipID].update();
            frmOKIM6295[chipID].Text = string.Format("OKIM6295 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitOKIM6295(chipID);
        }

        private void CloseFormOKIM6295(int chipID)
        {
            if (frmOKIM6295[chipID] == null) return;

            try
            {
                screen.RemoveOKIM6295(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmSN76489[chipID] = new frmSN76489(this, chipID, setting.other.Zoom);

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

            screen.AddSN76489(chipID, frmSN76489[chipID].pbScreen, Properties.Resources.planeSN76489);
            frmSN76489[chipID].Show();
            frmSN76489[chipID].update();
            frmSN76489[chipID].Text = string.Format("SN76489 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitSN76489(chipID);
            oldParam.sn76489[chipID] = new MDChipParams.SN76489();
        }

        private void CloseFormSN76489(int chipID)
        {
            if (frmSN76489[chipID] == null) return;

            try
            {
                screen.RemoveSN76489(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmSegaPCM[chipID] = new frmSegaPCM(this, chipID, setting.other.Zoom);

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

            screen.AddSegaPCM(chipID, frmSegaPCM[chipID].pbScreen, Properties.Resources.planeSEGAPCM);
            frmSegaPCM[chipID].Show();
            frmSegaPCM[chipID].update();
            frmSegaPCM[chipID].Text = string.Format("SegaPCM ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitSegaPCM(chipID);
            oldParam.segaPcm[chipID] = new MDChipParams.SegaPcm();
        }

        private void CloseFormSegaPCM(int chipID)
        {
            if (frmSegaPCM[chipID] == null) return;

            try
            {
                screen.RemoveSegaPCM(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmAY8910[chipID] = new frmAY8910(this, chipID, setting.other.Zoom);

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

            screen.AddAY8910(chipID, frmAY8910[chipID].pbScreen, Properties.Resources.planeAY8910);
            frmAY8910[chipID].Show();
            frmAY8910[chipID].update();
            frmAY8910[chipID].Text = string.Format("AY8910 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitAY8910(chipID);
            oldParam.ay8910[chipID] = new MDChipParams.AY8910();
        }

        private void CloseFormAY8910(int chipID)
        {
            if (frmAY8910[chipID] == null) return;

            try
            {
                screen.RemoveAY8910(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmHuC6280[chipID] = new frmHuC6280(this, chipID, setting.other.Zoom);

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

            screen.AddHuC6280(chipID, frmHuC6280[chipID].pbScreen, Properties.Resources.planeHuC6280);
            frmHuC6280[chipID].Show();
            frmHuC6280[chipID].update();
            frmHuC6280[chipID].Text = string.Format("HuC6280 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitHuC6280(chipID);
            oldParam.huc6280[chipID] = new MDChipParams.HuC6280();
        }

        private void CloseFormHuC6280(int chipID)
        {
            if (frmHuC6280[chipID] == null) return;

            try
            {
                screen.RemoveHuC6280(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmYM2413[chipID] = new frmYM2413(this, chipID, setting.other.Zoom);

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

            screen.AddYM2413(chipID, frmYM2413[chipID].pbScreen, Properties.Resources.planeYM2413);
            frmYM2413[chipID].Show();
            frmYM2413[chipID].update();
            frmYM2413[chipID].Text = string.Format("YM2413 ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitYM2413(chipID);
            oldParam.ym2413[chipID] = new MDChipParams.YM2413();
        }

        private void CloseFormYM2413(int chipID)
        {
            if (frmYM2413[chipID] == null) return;

            try
            {
                screen.RemoveYM2413(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

            frmMIDI[chipID] = new frmMIDI(this, chipID, setting.other.Zoom);

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

            screen.AddMIDI(chipID, frmMIDI[chipID].pbScreen, Properties.Resources.planeMIDI_GM);
            frmMIDI[chipID].Show();
            frmMIDI[chipID].update();
            frmMIDI[chipID].Text = string.Format("MIDI ({0})", chipID == 0 ? "Primary" : "Secondary");
            screen.screenInitMIDI(chipID);
            oldParam.midi[chipID] = new MIDIParam();
        }

        private void CloseFormMIDI(int chipID)
        {
            if (frmMIDI[chipID] == null) return;

            try
            {
                screen.RemoveMIDI(chipID);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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

                    frmPlayList.Stop();

                    frmPlayList.AddList(filename);

                    if (filename.ToLower().LastIndexOf(".zip") == -1)
                    {
                        loadAndPlay(0,filename);
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



        private void screenMainLoop()
        {
            double nextFrame = (double)System.Environment.TickCount;
            isRunning = true;
            stopped = false;

            while (isRunning)
            {
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

                for (int chipID = 0; chipID < 2; chipID++)
                {
                    if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed) frmMCD[chipID].screenChangeParams();
                    else frmMCD[chipID] = null;

                    if (frmC140[chipID] != null && !frmC140[chipID].isClosed) frmC140[chipID].screenChangeParams();
                    else frmC140[chipID] = null;

                    if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed) frmYM2608[chipID].screenChangeParams();
                    else frmYM2608[chipID] = null;

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

                    if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed) frmOKIM6258[chipID].screenChangeParams();
                    else frmOKIM6258[chipID] = null;

                    if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed) frmOKIM6295[chipID].screenChangeParams();
                    else frmOKIM6295[chipID] = null;

                    if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed) frmSN76489[chipID].screenChangeParams();
                    else frmSN76489[chipID] = null;

                    if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed) frmSegaPCM[chipID].screenChangeParams();
                    else frmSegaPCM[chipID] = null;

                    if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed) frmAY8910[chipID].screenChangeParams();
                    else frmAY8910[chipID] = null;

                    if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed) frmHuC6280[chipID].screenChangeParams();
                    else frmHuC6280[chipID] = null;

                    if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed) frmMIDI[chipID].screenChangeParams();
                    else frmMIDI[chipID] = null;

                }
                if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed) frmYM2612MIDI.screenChangeParams();
                else frmYM2612MIDI = null;
                if (frmMixer2 != null && !frmMixer2.isClosed) frmMixer2.screenChangeParams();
                else frmMixer2 = null;

                if ((double)System.Environment.TickCount >= nextFrame + period)
                {
                    nextFrame += period;
                    continue;
                }

                screenDrawParams();

                for (int chipID = 0; chipID < 2; chipID++)
                {
                    if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed) frmMCD[chipID].screenDrawParams();
                    else frmMCD[chipID] = null;

                    if (frmC140[chipID] != null && !frmC140[chipID].isClosed) frmC140[chipID].screenDrawParams();
                    else frmC140[chipID] = null;

                    if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed) frmYM2608[chipID].screenDrawParams();
                    else frmYM2608[chipID] = null;

                    if (frmYM2151[chipID] != null && !frmYM2151[chipID].isClosed) frmYM2151[chipID].screenDrawParams();
                    else frmYM2151[chipID] = null;

                    if (frmYM2203[chipID] != null && !frmYM2203[chipID].isClosed) frmYM2203[chipID].screenDrawParams();
                    else frmYM2203[chipID] = null;

                    if (frmYM2413[chipID] != null && !frmYM2413[chipID].isClosed) frmYM2413[chipID].screenDrawParams();
                    else frmYM2413[chipID] = null;

                    if (frmYM2610[chipID] != null && !frmYM2610[chipID].isClosed) frmYM2610[chipID].screenDrawParams();
                    else frmYM2610[chipID] = null;

                    if (frmYM2612[chipID] != null && !frmYM2612[chipID].isClosed) frmYM2612[chipID].screenDrawParams();
                    else frmYM2612[chipID] = null;

                    if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed) frmOKIM6258[chipID].screenDrawParams();
                    else frmOKIM6258[chipID] = null;

                    if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed) frmOKIM6295[chipID].screenDrawParams();
                    else frmOKIM6295[chipID] = null;

                    if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed) frmSN76489[chipID].screenDrawParams();
                    else frmSN76489[chipID] = null;

                    if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed) frmSegaPCM[chipID].screenDrawParams();
                    else frmSegaPCM[chipID] = null;

                    if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed) frmAY8910[chipID].screenDrawParams();
                    else frmAY8910[chipID] = null;

                    if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed) frmHuC6280[chipID].screenDrawParams();
                    else frmHuC6280[chipID] = null;

                    if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed) frmMIDI[chipID].screenDrawParams();
                    else frmMIDI[chipID] = null;

                }
                if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed) frmYM2612MIDI.screenDrawParams();
                else frmYM2612MIDI = null;
                if (frmMixer2 != null && !frmMixer2.isClosed) frmMixer2.screenDrawParams();
                else frmMixer2 = null;

                nextFrame += period;

                if (frmPlayList.isPlaying())
                {
                    if ((setting.other.UseLoopTimes && Audio.GetVgmCurLoopCounter() > setting.other.LoopTimes - 1) || Audio.GetVGMStopped())
                    {
                        fadeout();
                    }
                    if (Audio.Stopped && frmPlayList.isPlaying())
                    {
                        nextPlayMode();
                    }
                }

                if (Audio.fatalError)
                {
                    log.ForcedWrite("AudioでFatalErrorが発生。再度Audio初期化処理開始");

                    frmPlayList.Stop();
                    try { Audio.Stop(); }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                    }

                    try { Audio.Close(); }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                    }

                    Audio.fatalError = false;
                    Audio.Init(setting);

                    log.ForcedWrite("Audio初期化処理完了");
                }
            }

            stopped = true;
        }

        private void screenChangeParams()
        {

            for (int chipID = 0; chipID < 2; chipID++)
            {
                screenChangeParamsFromYM2612(chipID);
                screenChangeParamsFromSN76489(chipID);
                screenChangeParamsFromRF5C164(chipID);
                screenChangeParamsFromC140(chipID);
                screenChangeParamsFromSegaPCM(chipID);
                screenChangeParamsFromYM2151(chipID);
                screenChangeParamsFromYM2203(chipID);
                screenChangeParamsFromYM2413(chipID);
                screenChangeParamsFromYM2608(chipID);
                screenChangeParamsFromYM2610(chipID);
                screenChangeParamsFromAY8910(chipID);
                screenChangeParamsFromHuC6280(chipID);
                screenChangeParamsFromMIDI(chipID);
            }

            screenChangeParamsFromYM2612MIDI();
            screenChangeParamsFromMixer();

            long w = Audio.GetCounter();
            double sec = (double)w / (double)SamplingRate;
            newParam.Cminutes = (int)(sec / 60);
            sec -= newParam.Cminutes * 60;
            newParam.Csecond = (int)sec;
            sec -= newParam.Csecond;
            newParam.Cmillisecond = (int)(sec * 100.0);

            w = Audio.GetTotalCounter();
            sec = (double)w / (double)SamplingRate;
            newParam.TCminutes = (int)(sec / 60);
            sec -= newParam.TCminutes * 60;
            newParam.TCsecond = (int)sec;
            sec -= newParam.TCsecond;
            newParam.TCmillisecond = (int)(sec * 100.0);

            w = Audio.GetLoopCounter();
            sec = (double)w / (double)SamplingRate;
            newParam.LCminutes = (int)(sec / 60);
            sec -= newParam.LCminutes * 60;
            newParam.LCsecond = (int)sec;
            sec -= newParam.LCsecond;
            newParam.LCmillisecond = (int)(sec * 100.0);

        }

        private void screenChangeParamsFromYM2203(int chipID)
        {
            bool isFmEx;
            int[] ym2203Register = Audio.GetYM2203Register(chipID);
            int[] fmKeyYM2203 = Audio.GetYM2203KeyOn(chipID);
            int[] ym2203Vol = Audio.GetYM2203Volume(chipID);
            int[] ym2203Ch3SlotVol = Audio.GetYM2203Ch3SlotVolume(chipID);

            isFmEx = (ym2203Register[0x27] & 0x40) > 0;
            for (int ch = 0; ch < 3; ch++)
            {
                int c = ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 0] = ym2203Register[0x50 + ops + c] & 0x1f; //AR
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 1] = ym2203Register[0x60 + ops + c] & 0x1f; //DR
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 2] = ym2203Register[0x70 + ops + c] & 0x1f; //SR
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 3] = ym2203Register[0x80 + ops + c] & 0x0f; //RR
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 4] = (ym2203Register[0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 5] = ym2203Register[0x40 + ops + c] & 0x7f;//TL
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 6] = (ym2203Register[0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 7] = ym2203Register[0x30 + ops + c] & 0x0f;//ML
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 8] = (ym2203Register[0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 9] = (ym2203Register[0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.ym2203[chipID].channels[ch].inst[i * 11 + 10] = ym2203Register[0x90 + ops + c] & 0x0f;//SG
                }
                newParam.ym2203[chipID].channels[ch].inst[44] = ym2203Register[0xb0 + c] & 0x07;//AL
                newParam.ym2203[chipID].channels[ch].inst[45] = (ym2203Register[0xb0 + c] & 0x38) >> 3;//FB
                newParam.ym2203[chipID].channels[ch].inst[46] = (ym2203Register[0xb4 + c] & 0x38) >> 4;//AMS
                newParam.ym2203[chipID].channels[ch].inst[47] = ym2203Register[0xb4 + c] & 0x07;//FMS

                newParam.ym2203[chipID].channels[ch].pan = 3;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = ym2203Register[0xa0 + c] + (ym2203Register[0xa4 + c] & 0x07) * 0x100;
                    octav = (ym2203Register[0xa4 + c] & 0x38) >> 3;

                    if (fmKeyYM2203[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq) + 1, 0), 95);
                    newParam.ym2203[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2203Vol[ch] / 80, 0), 19);
                    //newParam.ym2203[0].channels[ch].volumeR = Math.Min(Math.Max(ym2203Vol[ch] / 80, 0), 19);
                }
                else
                {
                    freq = ym2203Register[0xa9] + (ym2203Register[0xad] & 0x07) * 0x100;
                    octav = (ym2203Register[0xad] & 0x38) >> 3;

                    if ((fmKeyYM2203[2] & 0x10) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq) + 1, 0), 95);
                    newParam.ym2203[chipID].channels[2].volumeL = Math.Min(Math.Max(ym2203Ch3SlotVol[0] / 80, 0), 19);
                    //newParam.ym2203[0].channels[2].volumeR = Math.Min(Math.Max(ym2203Ch3SlotVol[0] / 80, 0), 19);
                }
                newParam.ym2203[chipID].channels[ch].note = n;


            }

            //for (int ch = 6; ch < 9; ch++) //FM EX
            //{
            //    int[] exReg = new int[3] { 2, 0, -6 };
            //    int c = exReg[ch - 6];

            //    newParam.ym2203[chipID].channels[ch].pan = 0;

            //    if (isFmEx)
            //    {
            //        int freq = ym2203Register[0xa8 + c] + (ym2203Register[0xac + c] & 0x07) * 0x100;
            //        int octav = (ym2203Register[0xac + c] & 0x38) >> 3;
            //        int n = -1;
            //        if ((fmKeyYM2203[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
            //        newParam.ym2203[chipID].channels[ch].note = n;
            //        newParam.ym2203[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2203Ch3SlotVol[ch - 5] / 80, 0), 19);
            //    }
            //    else
            //    {
            //        newParam.ym2203[chipID].channels[ch].note = -1;
            //        newParam.ym2203[chipID].channels[ch].volumeL = 0;
            //    }
            //}
            for (int ch = 3; ch < 6; ch++) //FM EX
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 3];

                newParam.ym2203[chipID].channels[ch].pan = 0;

                if (isFmEx)
                {
                    int freq = ym2203Register[0xa8 + c] + (ym2203Register[0xac + c] & 0x07) * 0x100;
                    int octav = (ym2203Register[0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKeyYM2203[2] & (0x20 << (ch - 3))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq)+1, 0), 95);
                    newParam.ym2203[chipID].channels[ch].note = n;
                    newParam.ym2203[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2203Ch3SlotVol[ch - 2] / 80, 0), 19);
                }
                else
                {
                    newParam.ym2203[chipID].channels[ch].note = -1;
                    newParam.ym2203[chipID].channels[ch].volumeL = 0;
                }
            }

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.ym2203[chipID].channels[ch + 6];

                bool t = (ym2203Register[0x07] & (0x1 << ch)) == 0;
                bool n = (ym2203Register[0x07] & (0x8 << ch)) == 0;
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);
                channel.volume = (int)(((t || n) ? 1 : 0) * (ym2203Register[0x08 + ch] & 0xf) * (20.0 / 16.0));
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                if (channel.volume == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    int ft = ym2203Register[0x00 + ch * 2];
                    int ct = ym2203Register[0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = 7987200.0f / (64.0f * (float)tp);// 7987200 = MasterClock
                    channel.note = searchSSGNote(ftone);
                }

            }

            newParam.ym2203[chipID].nfrq = ym2203Register[0x06] & 0x1f;
            newParam.ym2203[chipID].efrq = ym2203Register[0x0c] * 0x100 + ym2203Register[0x0b];
            newParam.ym2203[chipID].etype = (ym2203Register[0x0d] & 0x7) + 2;

        }

        private void screenChangeParamsFromYM2413(int chipID)
        {
            int[] ym2413Register = Audio.GetYM2413Register(chipID);

            for (int ch = 0; ch < 9; ch++)
            {
                newParam.ym2413[chipID].channels[ch].inst[0] = (ym2413Register[0x30 + ch] & 0xf0) >> 4;
                newParam.ym2413[chipID].channels[ch].inst[1] = (ym2413Register[0x20 + ch] & 0x20) >> 5;
                newParam.ym2413[chipID].channels[ch].inst[2] = (ym2413Register[0x20 + ch] & 0x10) >> 4;
                newParam.ym2413[chipID].channels[ch].inst[3] = (ym2413Register[0x30 + ch] & 0x0f);

                int freq = ym2413Register[0x10 + ch] + ((ym2413Register[0x20 + ch] & 0x1) << 8);
                int oct = ((ym2413Register[0x20 + ch] & 0xe) >> 1);

                if (newParam.ym2413[chipID].channels[ch].inst[2] == 0)
                {
                    newParam.ym2413[chipID].channels[ch].note = -1;
                    newParam.ym2413[chipID].channels[ch].volumeL--;
                    if (newParam.ym2413[chipID].channels[ch].volumeL < 0) newParam.ym2413[chipID].channels[ch].volumeL = 0;
                }
                else
                {
                    int n = searchSegaPCMNote(freq / 172.0) + (oct - 4) * 12;
                    if (newParam.ym2413[chipID].channels[ch].note != n)
                    {
                        newParam.ym2413[chipID].channels[ch].note = n;
                        newParam.ym2413[chipID].channels[ch].volumeL = (19 - newParam.ym2413[chipID].channels[ch].inst[3]);
                    }
                    else
                    {
                        newParam.ym2413[chipID].channels[ch].volumeL--;
                        if (newParam.ym2413[chipID].channels[ch].volumeL < 0) newParam.ym2413[chipID].channels[ch].volumeL = 0;
                    }
                }

            }

            int r = Audio.getYM2413RyhthmKeyON(chipID);

            //BD
            if ((r & 0x10) != 0)
            {
                newParam.ym2413[chipID].channels[9].volume = (19 - (ym2413Register[0x36] & 0x0f));
            }
            else
            {
                newParam.ym2413[chipID].channels[9].volume--;
                if (newParam.ym2413[chipID].channels[9].volume < 0) newParam.ym2413[chipID].channels[9].volume = 0;
            }

            //SD
            if ((r & 0x08) != 0)
            {
                newParam.ym2413[chipID].channels[10].volume = (19 - (ym2413Register[0x37] & 0x0f));
            }
            else
            {
                newParam.ym2413[chipID].channels[10].volume--;
                if (newParam.ym2413[chipID].channels[10].volume < 0) newParam.ym2413[chipID].channels[10].volume = 0;
            }

            //TOM
            if ((r & 0x04) != 0)
            {
                newParam.ym2413[chipID].channels[11].volume = 19 - ((ym2413Register[0x38] & 0xf0) >> 4);
            }
            else
            {
                newParam.ym2413[chipID].channels[11].volume--;
                if (newParam.ym2413[chipID].channels[11].volume < 0) newParam.ym2413[chipID].channels[11].volume = 0;
            }

            //CYM
            if ((r & 0x02) != 0)
            {
                newParam.ym2413[chipID].channels[12].volume = 19 - ((ym2413Register[0x38] & 0x0f) >> 0);
            }
            else
            {
                newParam.ym2413[chipID].channels[12].volume--;
                if (newParam.ym2413[chipID].channels[12].volume < 0) newParam.ym2413[chipID].channels[12].volume = 0;
            }

            //HH
            if ((r & 0x01) != 0)
            {
                newParam.ym2413[chipID].channels[13].volume = 19 - ((ym2413Register[0x37] & 0xf0) >> 4);
            }
            else
            {
                newParam.ym2413[chipID].channels[13].volume--;
                if (newParam.ym2413[chipID].channels[13].volume < 0) newParam.ym2413[chipID].channels[13].volume = 0;
            }

            Audio.resetYM2413RyhthmKeyON(chipID);


            newParam.ym2413[chipID].channels[0].inst[4] = (ym2413Register[0x02] & 0x3f);//TL
            newParam.ym2413[chipID].channels[0].inst[5] = (ym2413Register[0x03] & 0x07);//FB

            newParam.ym2413[chipID].channels[0].inst[6] = (ym2413Register[0x04] & 0xf0) >> 4;//AR
            newParam.ym2413[chipID].channels[0].inst[7] = (ym2413Register[0x04] & 0x0f);//DR
            newParam.ym2413[chipID].channels[0].inst[8] = (ym2413Register[0x06] & 0xf0) >> 4;//SL
            newParam.ym2413[chipID].channels[0].inst[9] = (ym2413Register[0x06] & 0x0f);//RR
            newParam.ym2413[chipID].channels[0].inst[10] = (ym2413Register[0x02] & 0x80) >> 7;//KL
            newParam.ym2413[chipID].channels[0].inst[11] = (ym2413Register[0x00] & 0x0f);//MT
            newParam.ym2413[chipID].channels[0].inst[12] = (ym2413Register[0x00] & 0x80) >> 7;//AM
            newParam.ym2413[chipID].channels[0].inst[13] = (ym2413Register[0x00] & 0x40) >> 6;//VB
            newParam.ym2413[chipID].channels[0].inst[14] = (ym2413Register[0x00] & 0x20) >> 5;//EG
            newParam.ym2413[chipID].channels[0].inst[15] = (ym2413Register[0x00] & 0x10) >> 4;//KR
            newParam.ym2413[chipID].channels[0].inst[16] = (ym2413Register[0x03] & 0x08) >> 3;//DM
            newParam.ym2413[chipID].channels[0].inst[17] = (ym2413Register[0x05] & 0xf0) >> 4;//AR
            newParam.ym2413[chipID].channels[0].inst[18] = (ym2413Register[0x05] & 0x0f);//DR
            newParam.ym2413[chipID].channels[0].inst[19] = (ym2413Register[0x07] & 0xf0) >> 4;//SL
            newParam.ym2413[chipID].channels[0].inst[20] = (ym2413Register[0x07] & 0x0f);//RR
            newParam.ym2413[chipID].channels[0].inst[21] = (ym2413Register[0x03] & 0x80) >> 7;//KL
            newParam.ym2413[chipID].channels[0].inst[22] = (ym2413Register[0x01] & 0x0f);//MT
            newParam.ym2413[chipID].channels[0].inst[23] = (ym2413Register[0x01] & 0x80) >> 7;//AM
            newParam.ym2413[chipID].channels[0].inst[24] = (ym2413Register[0x01] & 0x40) >> 6;//VB
            newParam.ym2413[chipID].channels[0].inst[25] = (ym2413Register[0x01] & 0x20) >> 5;//EG
            newParam.ym2413[chipID].channels[0].inst[26] = (ym2413Register[0x01] & 0x10) >> 4;//KR
            newParam.ym2413[chipID].channels[0].inst[27] = (ym2413Register[0x03] & 0x10) >> 4;//DC

        }

        private void screenChangeParamsFromYM2610(int chipID)
        {
            int delta;
            float frq;

            int[][] YM2610Register = Audio.GetYM2610Register(chipID);
            int[] fmKeyYM2610 = Audio.GetYM2610KeyOn(chipID);
            int[][] YM2610Vol = Audio.GetYM2610Volume(chipID);
            int[] YM2610Ch3SlotVol = Audio.GetYM2610Ch3SlotVolume(chipID);
            int[][] YM2610Rhythm = Audio.GetYM2610RhythmVolume(chipID);
            int[] YM2610AdpcmVol = Audio.GetYM2610AdpcmVolume(chipID);
            bool isFmEx = (YM2610Register[chipID][0x27] & 0x40) > 0;

            newParam.ym2610[chipID].lfoSw = (YM2610Register[0][0x22] & 0x8) != 0;
            newParam.ym2610[chipID].lfoFrq = (YM2610Register[0][0x22] & 0x7);

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 0] = YM2610Register[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 1] = YM2610Register[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 2] = YM2610Register[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 3] = YM2610Register[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 4] = (YM2610Register[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 5] = YM2610Register[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 6] = (YM2610Register[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 7] = YM2610Register[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 8] = (YM2610Register[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 9] = (YM2610Register[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.ym2610[chipID].channels[ch].inst[i * 11 + 10] = YM2610Register[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.ym2610[chipID].channels[ch].inst[44] = YM2610Register[p][0xb0 + c] & 0x07;//AL
                newParam.ym2610[chipID].channels[ch].inst[45] = (YM2610Register[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.ym2610[chipID].channels[ch].inst[46] = (YM2610Register[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.ym2610[chipID].channels[ch].inst[47] = YM2610Register[p][0xb4 + c] & 0x07;//FMS

                newParam.ym2610[chipID].channels[ch].pan = (YM2610Register[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = YM2610Register[p][0xa0 + c] + (YM2610Register[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (YM2610Register[p][0xa4 + c] & 0x38) >> 3;

                    if (fmKeyYM2610[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq) + 1, 0), 95);
                    newParam.ym2610[chipID].channels[ch].volumeL = Math.Min(Math.Max(YM2610Vol[ch][0] / 80, 0), 19);
                    newParam.ym2610[chipID].channels[ch].volumeR = Math.Min(Math.Max(YM2610Vol[ch][1] / 80, 0), 19);
                }
                else
                {
                    freq = YM2610Register[0][0xa9] + (YM2610Register[0][0xad] & 0x07) * 0x100;
                    octav = (YM2610Register[0][0xad] & 0x38) >> 3;

                    if ((fmKeyYM2610[2] & 0x10) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq) + 1, 0), 95);
                    newParam.ym2610[chipID].channels[2].volumeL = Math.Min(Math.Max(YM2610Ch3SlotVol[0] / 80, 0), 19);
                    newParam.ym2610[chipID].channels[2].volumeR = Math.Min(Math.Max(YM2610Ch3SlotVol[0] / 80, 0), 19);
                }
                newParam.ym2610[chipID].channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++) //FM EX
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.ym2610[chipID].channels[ch].pan = 0;

                if (isFmEx)
                {
                    int freq = YM2610Register[0][0xa8 + c] + (YM2610Register[0][0xac + c] & 0x07) * 0x100;
                    int octav = (YM2610Register[0][0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKeyYM2610[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq)+1, 0), 95);
                    newParam.ym2610[chipID].channels[ch].note = n;
                    newParam.ym2610[chipID].channels[ch].volumeL = Math.Min(Math.Max(YM2610Ch3SlotVol[ch - 5] / 80, 0), 19);
                }
                else
                {
                    newParam.ym2610[chipID].channels[ch].note = -1;
                    newParam.ym2610[chipID].channels[ch].volumeL = 0;
                }
            }

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.ym2610[chipID].channels[ch + 9];

                bool t = (YM2610Register[0][0x07] & (0x1 << ch)) == 0;
                bool n = (YM2610Register[0][0x07] & (0x8 << ch)) == 0;
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);

                channel.volume = (int)(((t || n) ? 1 : 0) * (YM2610Register[0][0x08 + ch] & 0xf) * (20.0 / 16.0));
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                if (channel.volume == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    int ft = YM2610Register[0][0x00 + ch * 2];
                    int ct = YM2610Register[0][0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = 7987200.0f / (64.0f * (float)tp);// 7987200 = MasterClock
                    channel.note = searchSSGNote(ftone);
                }

            }

            newParam.ym2610[chipID].nfrq = YM2610Register[0][0x06] & 0x1f;
            newParam.ym2610[chipID].efrq = YM2610Register[0][0x0c] * 0x100 + YM2610Register[0][0x0b];
            newParam.ym2610[chipID].etype = (YM2610Register[0][0x0d] & 0x7) + 2;

            //ADPCM B
            newParam.ym2610[chipID].channels[12].pan = (YM2610Register[0][0x11] & 0xc0) >> 6;
            newParam.ym2610[chipID].channels[12].volumeL = Math.Min(Math.Max(YM2610AdpcmVol[0] / 80, 0), 19);
            newParam.ym2610[chipID].channels[12].volumeR = Math.Min(Math.Max(YM2610AdpcmVol[1] / 80, 0), 19);
            delta = (YM2610Register[0][0x1a] << 8) | YM2610Register[0][0x19];
            frq = (float)(delta / 9447.0f);//Delta=9447 at freq=8kHz
            newParam.ym2610[chipID].channels[12].note = (YM2610Register[0][0x10] & 0x80) != 0 ? searchYM2608Adpcm(frq) : -1;
            if ((YM2610Register[0][0x11] & 0xc0) == 0)
            {
                newParam.ym2610[chipID].channels[12].note = -1;
            }


            for (int ch = 13; ch < 19; ch++) //ADPCM A
            {
                newParam.ym2610[chipID].channels[ch].pan = (YM2610Register[1][0x08 + ch - 13] & 0xc0) >> 6;
                newParam.ym2610[chipID].channels[ch].volumeL = Math.Min(Math.Max(YM2610Rhythm[ch - 13][0] / 80, 0), 19);
                newParam.ym2610[chipID].channels[ch].volumeR = Math.Min(Math.Max(YM2610Rhythm[ch - 13][1] / 80, 0), 19);
            }
        }

        private void screenChangeParamsFromYM2608(int chipID)
        {
            bool isFmEx;
            int[][] ym2608Register = Audio.GetYM2608Register(chipID);
            int[] fmKeyYM2608 = Audio.GetYM2608KeyOn(chipID);
            int[][] ym2608Vol = Audio.GetYM2608Volume(chipID);
            int[] ym2608Ch3SlotVol = Audio.GetYM2608Ch3SlotVolume(chipID);
            int[][] ym2608Rhythm = Audio.GetYM2608RhythmVolume(chipID);
            int[] ym2608AdpcmVol = Audio.GetYM2608AdpcmVolume(chipID);

            isFmEx = (ym2608Register[0][0x27] & 0x40) > 0;

            newParam.ym2608[chipID].lfoSw = (ym2608Register[0][0x22] & 0x8) != 0;
            newParam.ym2608[chipID].lfoFrq = (ym2608Register[0][0x22] & 0x7);

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 0] = ym2608Register[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 1] = ym2608Register[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 2] = ym2608Register[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 3] = ym2608Register[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 4] = (ym2608Register[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 5] = ym2608Register[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 6] = (ym2608Register[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 7] = ym2608Register[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 8] = (ym2608Register[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 9] = (ym2608Register[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.ym2608[chipID].channels[ch].inst[i * 11 + 10] = ym2608Register[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.ym2608[chipID].channels[ch].inst[44] = ym2608Register[p][0xb0 + c] & 0x07;//AL
                newParam.ym2608[chipID].channels[ch].inst[45] = (ym2608Register[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.ym2608[chipID].channels[ch].inst[46] = (ym2608Register[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.ym2608[chipID].channels[ch].inst[47] = ym2608Register[p][0xb4 + c] & 0x07;//FMS

                newParam.ym2608[chipID].channels[ch].pan = (ym2608Register[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = ym2608Register[p][0xa0 + c] + (ym2608Register[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (ym2608Register[p][0xa4 + c] & 0x38) >> 3;

                    if (fmKeyYM2608[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq) + 1, 0), 95);
                    newParam.ym2608[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2608Vol[ch][0] / 80, 0), 19);
                    newParam.ym2608[chipID].channels[ch].volumeR = Math.Min(Math.Max(ym2608Vol[ch][1] / 80, 0), 19);
                }
                else
                {
                    freq = ym2608Register[0][0xa9] + (ym2608Register[0][0xad] & 0x07) * 0x100;
                    octav = (ym2608Register[0][0xad] & 0x38) >> 3;

                    if ((fmKeyYM2608[2] & 0x10) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq) + 1, 0), 95);
                    newParam.ym2608[chipID].channels[2].volumeL = Math.Min(Math.Max(ym2608Ch3SlotVol[0] / 80, 0), 19);
                    newParam.ym2608[chipID].channels[2].volumeR = Math.Min(Math.Max(ym2608Ch3SlotVol[0] / 80, 0), 19);
                }
                newParam.ym2608[chipID].channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++) //FM EX
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.ym2608[chipID].channels[ch].pan = 0;

                if (isFmEx)
                {
                    int freq = ym2608Register[0][0xa8 + c] + (ym2608Register[0][0xac + c] & 0x07) * 0x100;
                    int octav = (ym2608Register[0][0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKeyYM2608[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq)+1, 0), 95);
                    newParam.ym2608[chipID].channels[ch].note = n;
                    newParam.ym2608[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2608Ch3SlotVol[ch - 5] / 80, 0), 19);
                }
                else
                {
                    newParam.ym2608[chipID].channels[ch].note = -1;
                    newParam.ym2608[chipID].channels[ch].volumeL = 0;
                }
            }

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.ym2608[chipID].channels[ch + 9];

                bool t = (ym2608Register[0][0x07] & (0x1 << ch)) == 0;
                bool n = (ym2608Register[0][0x07] & (0x8 << ch)) == 0;
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);

                channel.volume = (int)(((t || n) ? 1 : 0) * (ym2608Register[0][0x08 + ch] & 0xf) * (20.0 / 16.0));
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                if (channel.volume == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    int ft = ym2608Register[0][0x00 + ch * 2];
                    int ct = ym2608Register[0][0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = 7987200.0f / (64.0f * (float)tp);// 7987200 = MasterClock
                    channel.note = searchSSGNote(ftone);
                }

            }

            newParam.ym2608[chipID].nfrq = ym2608Register[0][0x06] & 0x1f;
            newParam.ym2608[chipID].efrq = ym2608Register[0][0x0c] * 0x100 + ym2608Register[0][0x0b];
            newParam.ym2608[chipID].etype = (ym2608Register[0][0x0d] & 0x7) + 2;

            //ADPCM
            newParam.ym2608[chipID].channels[12].pan = (ym2608Register[1][0x01] & 0xc0) >> 6; // ((ym2608Register[1][0x01] & 0xc0) >> 6) != 0 ? ((ym2608Register[1][0x01] & 0xc0) >> 6) : newParam.ym2608[chipID].channels[12].pan;
            newParam.ym2608[chipID].channels[12].volumeL = Math.Min(Math.Max(ym2608AdpcmVol[0] / 80, 0), 19);
            newParam.ym2608[chipID].channels[12].volumeR = Math.Min(Math.Max(ym2608AdpcmVol[1] / 80, 0), 19);
            int delta = (ym2608Register[1][0x0a] << 8) | ym2608Register[1][0x09];
            float frq = (float)(delta / 9447.0f);
            newParam.ym2608[chipID].channels[12].note = (ym2608Register[1][0x00] & 0x80) != 0 ? searchYM2608Adpcm(frq) : -1;
            if ((ym2608Register[1][0x01] & 0xc0) == 0)
            {
                newParam.ym2608[chipID].channels[12].note = -1;
            }

            for (int ch = 13; ch < 19; ch++) //RHYTHM
            {
                newParam.ym2608[chipID].channels[ch].pan = (ym2608Register[0][0x18 + ch - 13] & 0xc0) >> 6;
                newParam.ym2608[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2608Rhythm[ch - 13][0] / 80, 0), 19);
                newParam.ym2608[chipID].channels[ch].volumeR = Math.Min(Math.Max(ym2608Rhythm[ch - 13][1] / 80, 0), 19);
            }

        }

        private void screenChangeParamsFromYM2151(int chipID)
        {
            int[] ym2151Register = Audio.GetYM2151Register(chipID);
            int[] fmKeyYM2151 = Audio.GetYM2151KeyOn(chipID);
            int[][] fmYM2151Vol = Audio.GetYM2151Volume(chipID);

            for (int ch = 0; ch < 8; ch++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 0] = ym2151Register[0x80 + ops + ch] & 0x1f; //AR
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 1] = ym2151Register[0xa0 + ops + ch] & 0x1f; //DR
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 2] = ym2151Register[0xc0 + ops + ch] & 0x1f; //SR
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 3] = ym2151Register[0xe0 + ops + ch] & 0x0f; //RR
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 4] = (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4;//SL
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 5] = ym2151Register[0x60 + ops + ch] & 0x7f;//TL
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 6] = (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6;//KS
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 7] = ym2151Register[0x40 + ops + ch] & 0x0f;//ML
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 8] = (ym2151Register[0x40 + ops + ch] & 0x70) >> 4;//DT
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 9] = (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6;//DT2
                    newParam.ym2151[chipID].channels[ch].inst[i * 11 + 10] = (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7;//AM
                }
                newParam.ym2151[chipID].channels[ch].inst[44] = ym2151Register[0x20 + ch] & 0x07;//AL
                newParam.ym2151[chipID].channels[ch].inst[45] = (ym2151Register[0x20 + ch] & 0x38) >> 3;//FB
                newParam.ym2151[chipID].channels[ch].inst[46] = (ym2151Register[0x38 + ch] & 0x3);//AMS
                newParam.ym2151[chipID].channels[ch].inst[47] = (ym2151Register[0x38 + ch] & 0x70) >> 4;//PMS

                int p = (ym2151Register[0x20 + ch] & 0xc0) >> 6;
                newParam.ym2151[chipID].channels[ch].pan = p == 1 ? 2 : (p == 2 ? 1 : p);
                int note = (ym2151Register[0x28 + ch] & 0x0f);
                note = (note < 3) ? note : (note < 7 ? note - 1 : (note < 11 ? note - 2 : note - 3));
                int oct = ((ym2151Register[0x28 + ch] & 0x70) >> 4);
                //newParam.ym2151[chipID].channels[ch].note = (fmKeyYM2151[ch] > 0) ? (oct * 12 + note + Audio.vgmReal.YM2151Hosei + 1 + 9) : -1;
                int hosei = 0;
                if (Audio.driverVirtual is vgm)
                {
                    hosei= ((vgm)Audio.driverVirtual).YM2151Hosei[chipID];
                }
                newParam.ym2151[chipID].channels[ch].note = (fmKeyYM2151[ch] > 0) ? (oct * 12 + note + hosei) : -1;//4

                newParam.ym2151[chipID].channels[ch].volumeL = Math.Min(Math.Max(fmYM2151Vol[ch][0] / 80, 0), 19);
                newParam.ym2151[chipID].channels[ch].volumeR = Math.Min(Math.Max(fmYM2151Vol[ch][1] / 80, 0), 19);

                newParam.ym2151[chipID].channels[ch].kf = ((ym2151Register[0x30 + ch] & 0xfc) >> 2);

            }
            newParam.ym2151[chipID].ne = ((ym2151Register[0x0f] & 0x80) >> 7);
            newParam.ym2151[chipID].nfrq = ((ym2151Register[0x0f] & 0x1f) >> 0);
            newParam.ym2151[chipID].lfrq = ((ym2151Register[0x18] & 0xff) >> 0);
            newParam.ym2151[chipID].pmd = Audio.GetYM2151PMD(chipID);
            newParam.ym2151[chipID].amd = Audio.GetYM2151AMD(chipID);
            newParam.ym2151[chipID].waveform = ((ym2151Register[0x1b] & 0x3) >> 0);
            newParam.ym2151[chipID].lfosync = ((ym2151Register[0x01] & 0x02) >> 1);

        }

        private void screenChangeParamsFromSegaPCM(int chipID)
        {
            MDSound.segapcm.segapcm_state segapcmState = Audio.GetSegaPCMRegister(chipID);
            if (segapcmState != null && segapcmState.ram != null && segapcmState.rom != null)
            {
                for (int ch = 0; ch < 16; ch++)
                {
                    int l = segapcmState.ram[ch * 8 + 2] & 0x7f;
                    int r = segapcmState.ram[ch * 8 + 3] & 0x7f;
                    int dt = segapcmState.ram[ch * 8 + 7];
                    double ml = dt / 256.0;

                    int ptrRom = segapcmState.ptrRom + ((segapcmState.ram[ch * 8 + 0x86] & segapcmState.bankmask) << segapcmState.bankshift);
                    uint addr = (uint)((segapcmState.ram[ch * 8 + 0x85] << 16) | (segapcmState.ram[ch * 8 + 0x84] << 8) | segapcmState.low[ch]);
                    int vdt = 0;
                    if (ptrRom + ((addr >> 8) & segapcmState.rgnmask) < segapcmState.rom.Length)
                    {
                        vdt = Math.Abs((sbyte)(segapcmState.rom[ptrRom + ((addr >> 8) & segapcmState.rgnmask)]) - 0x80);
                    }
                    byte end = (byte)(segapcmState.ram[ch * 8 + 6] + 1);
                    if ((segapcmState.ram[ch * 8 + 0x86] & 1) != 0) vdt = 0;
                    if ((addr >> 16) == end)
                    {
                        if ((segapcmState.ram[ch * 8 + 0x86] & 2) == 0)
                            ml = 0;
                    }

                    newParam.segaPcm[chipID].channels[ch].volumeL = Math.Min(Math.Max((l * vdt) >> 8, 0), 19);
                    newParam.segaPcm[chipID].channels[ch].volumeR = Math.Min(Math.Max((r * vdt) >> 8, 0), 19);
                    if (newParam.segaPcm[chipID].channels[ch].volumeL == 0 && newParam.segaPcm[chipID].channels[ch].volumeR == 0)
                    {
                        ml = 0;
                    }
                    newParam.segaPcm[chipID].channels[ch].note = (ml == 0 || vdt == 0) ? -1 : (searchSegaPCMNote(ml));
                    newParam.segaPcm[chipID].channels[ch].pan = (r >> 3) * 0x10 + (l >> 3);
                }
            }
        }

        private void screenChangeParamsFromC140(int chipID)
        {
            MDSound.c140.c140_state c140State = Audio.GetC140Register(chipID);
            if (c140State != null)
            {
                for (int ch = 0; ch < 24; ch++)
                {
                    int frequency = c140State.REG[ch * 16 + 2] * 256 + c140State.REG[ch * 16 + 3];
                    int l = c140State.REG[ch * 16 + 1];
                    int r = c140State.REG[ch * 16 + 0];
                    int vdt = Math.Abs((int)c140State.voi[ch].prevdt);

                    if (c140State.voi[ch].key == 0) frequency = 0;
                    if (frequency == 0)
                    {
                        l = 0;
                        r = 0;
                    }

                    newParam.c140[chipID].channels[ch].note = frequency == 0 ? -1 : (searchC140Note(frequency) + 1);
                    newParam.c140[chipID].channels[ch].pan = ((l >> 2) & 0xf) | (((r >> 2) & 0xf) << 4);
                    newParam.c140[chipID].channels[ch].volumeL = Math.Min(Math.Max((l * vdt) >> 7, 0), 19);
                    newParam.c140[chipID].channels[ch].volumeR = Math.Min(Math.Max((r * vdt) >> 7, 0), 19);
                }
            }
        }

        private void screenChangeParamsFromRF5C164(int chipID)
        {
            MDSound.scd_pcm.pcm_chip_ rf5c164Register = Audio.GetRf5c164Register(chipID);
            if (rf5c164Register != null)
            {
                int[][] rf5c164Vol = Audio.GetRf5c164Volume(chipID);
                for (int ch = 0; ch < 8; ch++)
                {
                    if (rf5c164Register.Channel[ch].Enable != 0)
                    {
                        newParam.rf5c164[chipID].channels[ch].note = searchRf5c164Note(rf5c164Register.Channel[ch].Step_B);
                        newParam.rf5c164[chipID].channels[ch].volumeL = Math.Min(Math.Max(rf5c164Vol[ch][0] / 400, 0), 19);
                        newParam.rf5c164[chipID].channels[ch].volumeR = Math.Min(Math.Max(rf5c164Vol[ch][1] / 400, 0), 19);
                    }
                    else
                    {
                        newParam.rf5c164[chipID].channels[ch].note = -1;
                        newParam.rf5c164[chipID].channels[ch].volumeL = 0;
                        newParam.rf5c164[chipID].channels[ch].volumeR = 0;
                    }
                    newParam.rf5c164[chipID].channels[ch].pan = (int)rf5c164Register.Channel[ch].PAN;
                }
            }
        }

        private void screenChangeParamsFromSN76489(int chipID)
        {
            int[] psgRegister = Audio.GetPSGRegister(chipID);
            int[][] psgVol = Audio.GetPSGVolume(chipID);
            if (psgRegister != null)
            {
                //Console.WriteLine("Val{0:X}", psgRegister[0 * 2]);
                for (int ch = 0; ch < 4; ch++)
                {
                    if (psgRegister[ch * 2 + 1] != 15)
                    {
                        newParam.sn76489[chipID].channels[ch].note = searchPSGNote(psgRegister[ch * 2]);
                    }
                    else
                    {
                        newParam.sn76489[chipID].channels[ch].note = -1;
                    }

                    newParam.sn76489[chipID].channels[ch].volume = Math.Min(Math.Max((int)((psgVol[ch][0] + psgVol[ch][1]) / (30.0 / 19.0)), 0), 19);
                }
            }
        }

        private void screenChangeParamsFromYM2612(int chipID)
        {
            int[][] fmRegister = Audio.GetFMRegister(chipID);
            int[][] fmVol = Audio.GetFMVolume(chipID);
            int[] fmCh3SlotVol = Audio.GetFMCh3SlotVolume(chipID);
            int[] fmKey = Audio.GetFMKeyOn(chipID);

            bool isFmEx = (fmRegister[0][0x27] & 0x40) > 0;

            newParam.ym2612[chipID].lfoSw = (fmRegister[0][0x22] & 0x8) != 0;
            newParam.ym2612[chipID].lfoFrq = (fmRegister[0][0x22] & 0x7);

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 0] = fmRegister[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 1] = fmRegister[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 2] = fmRegister[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 3] = fmRegister[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 4] = (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 5] = fmRegister[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 6] = (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 7] = fmRegister[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 8] = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 9] = (fmRegister[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.ym2612[chipID].channels[ch].inst[i * 11 + 10] = fmRegister[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.ym2612[chipID].channels[ch].inst[44] = fmRegister[p][0xb0 + c] & 0x07;//AL
                newParam.ym2612[chipID].channels[ch].inst[45] = (fmRegister[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.ym2612[chipID].channels[ch].inst[46] = (fmRegister[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.ym2612[chipID].channels[ch].inst[47] = fmRegister[p][0xb4 + c] & 0x07;//FMS

                newParam.ym2612[chipID].channels[ch].pan = (fmRegister[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = fmRegister[p][0xa0 + c] + (fmRegister[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (fmRegister[p][0xa4 + c] & 0x38) >> 3;

                    if (fmKey[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2612[chipID].channels[ch].volumeL = Math.Min(Math.Max(fmVol[ch][0] / 80, 0), 19);
                    newParam.ym2612[chipID].channels[ch].volumeR = Math.Min(Math.Max(fmVol[ch][1] / 80, 0), 19);
                }
                else
                {
                    freq = fmRegister[0][0xa9] + (fmRegister[0][0xad] & 0x07) * 0x100;
                    octav = (fmRegister[0][0xad] & 0x38) >> 3;

                    if ((fmKey[2] & 0x10) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2612[chipID].channels[2].volumeL = Math.Min(Math.Max(fmCh3SlotVol[0] / 80, 0), 19);
                    newParam.ym2612[chipID].channels[2].volumeR = Math.Min(Math.Max(fmCh3SlotVol[0] / 80, 0), 19);
                }
                newParam.ym2612[chipID].channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++)
            {
                //Operator 1′s frequency is in A9 and ADH
                //Operator 2′s frequency is in AA and AEH
                //Operator 3′s frequency is in A8 and ACH
                //Operator 4′s frequency is in A2 and A6H

                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.ym2612[chipID].channels[ch].pan = 0;

                if (isFmEx)
                {
                    int freq = fmRegister[0][0xa8 + c] + (fmRegister[0][0xac + c] & 0x07) * 0x100;
                    int octav = (fmRegister[0][0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKey[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2612[chipID].channels[ch].note = n;
                    newParam.ym2612[chipID].channels[ch].volumeL = Math.Min(Math.Max(fmCh3SlotVol[ch - 5] / 80, 0), 19);
                }
                else
                {
                    newParam.ym2612[chipID].channels[ch].note = -1;
                    newParam.ym2612[chipID].channels[ch].volumeL = 0;
                }
            }

            newParam.ym2612[chipID].channels[5].pcmMode = (fmRegister[0][0x2b] & 0x80) >> 7;

            if (newParam.fileFormat == enmFileFormat.XGM)
            {
                if (Audio.driverVirtual != null && ((xgm)Audio.driverVirtual).xgmpcm != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (((xgm)Audio.driverVirtual).xgmpcm[i].isPlaying)
                        {
                            newParam.ym2612[chipID].xpcmInst[i] = (int)(((xgm)Audio.driverVirtual).xgmpcm[i].inst);
                            int d = (((xgm)Audio.driverVirtual).xgmpcm[i].data / 6);
                            d = Math.Min(d, 19);
                            newParam.ym2612[chipID].xpcmVolL[i] = d;
                            newParam.ym2612[chipID].xpcmVolR[i] = d;
                        }
                        else
                        {
                            newParam.ym2612[chipID].xpcmInst[i] = 0;
                            newParam.ym2612[chipID].xpcmVolL[i] = 0;
                            newParam.ym2612[chipID].xpcmVolR[i] = 0;
                        }
                    }
                }
            }
        }

        private void screenChangeParamsFromAY8910(int chipID)
        {
            int[] AY8910Register = Audio.GetAY8910Register(chipID);

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.ay8910[chipID].channels[ch];

                bool t = (AY8910Register[0x07] & (0x1 << ch)) == 0;
                bool n = (AY8910Register[0x07] & (0x8 << ch)) == 0;

                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);
                newParam.ay8910[chipID].nfrq = AY8910Register[0x06] & 0x1f;
                newParam.ay8910[chipID].efrq = AY8910Register[0x0c] * 0x100 + AY8910Register[0x0b];
                newParam.ay8910[chipID].etype = (AY8910Register[0x0d] & 0x7) + 2;

                int v = (AY8910Register[0x08 + ch] & 0x1f);
                v = v > 15 ? 15 : v;
                channel.volume = (int)(((t || n) ? 1 : 0) * v * (20.0 / 16.0));
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                if (channel.volume == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    int ft = AY8910Register[0x00 + ch * 2];
                    int ct = AY8910Register[0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = 7987200.0f / (64.0f * (float)tp);// 7987200 = MasterClock
                    channel.note = searchSSGNote(ftone);
                }

            }

        }

        private void screenChangeParamsFromHuC6280(int chipID)
        {

            MDSound.Ootake_PSG.huc6280_state chip = Audio.GetHuC6280Register(chipID);
            if (chip == null) return;

            for (int ch = 0; ch < 6; ch++)
            {
                MDSound.Ootake_PSG.PSG psg = chip.Psg[ch];
                if (psg == null) continue;
                MDChipParams.Channel channel = newParam.huc6280[chipID].channels[ch];
                channel.volumeL = (psg.outVolumeL >> 10);
                channel.volumeR = (psg.outVolumeR >> 10);
                channel.volumeL = Math.Min(channel.volumeL, 19);
                channel.volumeR = Math.Min(channel.volumeR, 19);

                channel.pan = (int)((psg.volumeL & 0xf) | ((psg.volumeR & 0xf) << 4));

                channel.inst = psg.wave;

                channel.dda = psg.bDDA;

                int tp = (int)psg.frq;
                if (tp == 0) tp = 1;

                float ftone = 3579545.0f / 32.0f / (float)tp;
                channel.note = searchSSGNote(ftone);
                if (channel.volumeL == 0 && channel.volumeR == 0) channel.note = -1;

                if (ch < 4) continue;

                channel.noise = psg.bNoiseOn;
                channel.nfrq = (int)psg.noiseFrq;
            }

            newParam.huc6280[chipID].mvolL = (int)chip.MainVolumeL;
            newParam.huc6280[chipID].mvolR = (int)chip.MainVolumeR;
            newParam.huc6280[chipID].LfoCtrl = (int)chip.LfoCtrl;
            newParam.huc6280[chipID].LfoFrq = (int)chip.LfoFrq;

        }

        private void screenChangeParamsFromMIDI(int chipID)
        {
            MIDIParam prm = Audio.GetMIDIInfos(chipID);

            for (int ch = 0; ch < 16; ch++)
            {
                for (int i = 0; i < 256; i++)
                {
                    newParam.midi[chipID].cc[ch][i] = prm.cc[ch][i];
                }
                newParam.midi[chipID].bend[ch] = prm.bend[ch];

                for (int i = 0; i < 128; i++)
                {
                    newParam.midi[chipID].note[ch][i] = prm.note[ch][i];
                }

                newParam.midi[chipID].level[ch][0] = prm.level[ch][0];
                newParam.midi[chipID].level[ch][1] = prm.level[ch][1];
                newParam.midi[chipID].level[ch][2] = prm.level[ch][2];
                newParam.midi[chipID].level[ch][3] = prm.level[ch][3];
                newParam.midi[chipID].level[ch][4] = prm.level[ch][4];
                if (prm.level[ch][0] > 0) { prm.level[ch][0] -= 3; if (prm.level[ch][0] < 0) prm.level[ch][0] = 0; }
                if (prm.level[ch][1] > 0) { prm.level[ch][1] -= 3; if (prm.level[ch][1] < 0) prm.level[ch][1] = 0; }
                if (prm.level[ch][2] > 0) { prm.level[ch][2] -= 3; if (prm.level[ch][2] < 0) prm.level[ch][2] = 0; }
                if (prm.level[ch][3] > 0) {
                    prm.level[ch][4] -= 3;
                    if (prm.level[ch][4] < 0)
                    {
                        prm.level[ch][4] = 0;
                        prm.level[ch][3] -= 3;
                        if (prm.level[ch][3] < 0) prm.level[ch][3] = 0;
                    }
                }

                newParam.midi[chipID].pc[ch] = prm.pc[ch];

                newParam.midi[chipID].nrpnVibRate[ch] = prm.nrpnVibRate[ch];
                newParam.midi[chipID].nrpnVibDepth[ch] = prm.nrpnVibDepth[ch];
                newParam.midi[chipID].nrpnVibDelay[ch] = prm.nrpnVibDelay[ch];

                newParam.midi[chipID].nrpnLPF[ch] = prm.nrpnLPF[ch];
                newParam.midi[chipID].nrpnLPFRsn[ch] = prm.nrpnLPFRsn[ch];
                newParam.midi[chipID].nrpnHPF[ch] = prm.nrpnHPF[ch];

                newParam.midi[chipID].nrpnEQBaseFrq[ch] = prm.nrpnEQBaseFrq[ch];
                newParam.midi[chipID].nrpnEQBaseGain[ch] = prm.nrpnEQBaseGain[ch];
                newParam.midi[chipID].nrpnEQTrebleFrq[ch] = prm.nrpnEQTrebleFrq[ch];
                newParam.midi[chipID].nrpnEQTrebleGain[ch] = prm.nrpnEQTrebleGain[ch];

                newParam.midi[chipID].nrpnEGAttack[ch] = prm.nrpnEGAttack[ch];
                newParam.midi[chipID].nrpnEGDecay[ch] = prm.nrpnEGDecay[ch];
                newParam.midi[chipID].nrpnEGRls[ch] = prm.nrpnEGRls[ch];
            }

            newParam.midi[chipID].MIDIModule = prm.MIDIModule;

            //Display Data
            for (int i = 0; i < 64; i++)
            {
                newParam.midi[chipID].LCDDisplay[i] = prm.LCDDisplay[i];
            }
            newParam.midi[chipID].LCDDisplayTime = prm.LCDDisplayTime;
            prm.LCDDisplayTime -= 3;
            if (prm.LCDDisplayTime < 0) prm.LCDDisplayTime = 0;
            newParam.midi[chipID].LCDDisplayTimeXG = prm.LCDDisplayTimeXG;
            prm.LCDDisplayTimeXG -= 3;
            if (prm.LCDDisplayTimeXG < 0) prm.LCDDisplayTimeXG = 0;

            //Display Letter Data
            for (int i = 0; i < 32; i++)
            {
                newParam.midi[chipID].LCDDisplayLetter[i] = prm.LCDDisplayLetter[i];
            }
            newParam.midi[chipID].LCDDisplayLetterLen = prm.LCDDisplayLetterLen;
            newParam.midi[chipID].LCDDisplayLetterTime = prm.LCDDisplayLetterTime;
            prm.LCDDisplayLetterTime -= 3;
            if (prm.LCDDisplayLetterTime < 0)
            {
                if (prm.LCDDisplayLetterLen > 0)
                {
                    for (int i = 1; i < 32; i++)
                    {
                        prm.LCDDisplayLetter[i - 1] = (byte)(i < prm.LCDDisplayLetterLen ? prm.LCDDisplayLetter[i] : 0x20);
                    }
                    prm.LCDDisplayLetterTime = 40;
                    prm.LCDDisplayLetterLen--;
                }
                else
                {
                    prm.LCDDisplayLetterTime = 0;
                }
            }
            newParam.midi[chipID].LCDDisplayLetterTimeXG = prm.LCDDisplayLetterTimeXG;
            prm.LCDDisplayLetterTimeXG -= 3;
            if (prm.LCDDisplayLetterTimeXG < 0) prm.LCDDisplayLetterTimeXG = 0;

            newParam.midi[chipID].ReverbGS = prm.ReverbGS;
            newParam.midi[chipID].ChorusGS = prm.ChorusGS;
            newParam.midi[chipID].DelayGS = prm.DelayGS;
            newParam.midi[chipID].EFXGS = prm.EFXGS;

            newParam.midi[chipID].ReverbXG = prm.ReverbXG;
            newParam.midi[chipID].ChorusXG = prm.ChorusXG;
            newParam.midi[chipID].VariationXG = prm.VariationXG;
            newParam.midi[chipID].Insertion1XG = prm.Insertion1XG;
            newParam.midi[chipID].Insertion2XG = prm.Insertion2XG;
            newParam.midi[chipID].Insertion3XG = prm.Insertion3XG;
            newParam.midi[chipID].Insertion4XG = prm.Insertion4XG;

            newParam.midi[chipID].MasterVolume = prm.MasterVolume;

            newParam.midi[chipID].Lyric = prm.Lyric;

        }

        private void screenChangeParamsFromYM2612MIDI()
        {
            int[][] fmRegister = Audio.GetYM2612MIDIRegister();
            //int[] fmKey = Audio.GetFMKeyOn();

            newParam.ym2612Midi.IsMONO = setting.midiKbd.IsMONO;
            if (setting.midiKbd.IsMONO)
            {
                for (int i = 0; i < 6; i++)
                {
                    newParam.ym2612Midi.useChannel[i] = (setting.midiKbd.UseMONOChannel == i);
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    newParam.ym2612Midi.useChannel[i] = setting.midiKbd.UseChannel[i];
                }
            }

            newParam.ym2612Midi.useFormat = setting.midiKbd.UseFormat;

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 0] = fmRegister[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 1] = fmRegister[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 2] = fmRegister[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 3] = fmRegister[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 4] = (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 5] = fmRegister[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 6] = (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 7] = fmRegister[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 8] = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 9] = (fmRegister[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.ym2612Midi.channels[ch].inst[i * 11 + 10] = fmRegister[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.ym2612Midi.channels[ch].inst[44] = fmRegister[p][0xb0 + c] & 0x07;//AL
                newParam.ym2612Midi.channels[ch].inst[45] = (fmRegister[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.ym2612Midi.channels[ch].inst[46] = (fmRegister[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.ym2612Midi.channels[ch].inst[47] = fmRegister[p][0xb4 + c] & 0x07;//FMS

                newParam.ym2612Midi.channels[ch].pan = (fmRegister[p][0xb4 + c] & 0xc0) >> 6;

                if (newParam.ym2612Midi.selectCh != -1 && newParam.ym2612Midi.selectParam != -1)
                {
                    if (oldParam.ym2612Midi.selectCh != -1 && oldParam.ym2612Midi.selectParam != -1)
                    {
                        newParam.ym2612Midi.channels[oldParam.ym2612Midi.selectCh].typ[oldParam.ym2612Midi.selectParam] = 0;
                    }
                    newParam.ym2612Midi.channels[newParam.ym2612Midi.selectCh].typ[newParam.ym2612Midi.selectParam] = 1;
                    oldParam.ym2612Midi.selectCh = newParam.ym2612Midi.selectCh;
                    oldParam.ym2612Midi.selectParam = newParam.ym2612Midi.selectParam;
                }

                //int freq = 0;
                //int octav = 0;
                //int n = -1;
                //freq = fmRegister[p][0xa0 + c] + (fmRegister[p][0xa4 + c] & 0x07) * 0x100;
                //octav = (fmRegister[p][0xa4 + c] & 0x38) >> 3;

                //if (fmKey[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);

                //newParam.ym2612Midi.channels[ch].volumeL = Math.Min(Math.Max(fmVol[ch][0] / 80, 0), 19);
                //newParam.ym2612Midi.channels[ch].volumeR = Math.Min(Math.Max(fmVol[ch][1] / 80, 0), 19);
                //newParam.ym2612Midi.channels[ch].note = n;

            }

        }

        private void screenChangeParamsFromMixer()
        {

            newParam.mixer.AY8910.Volume = setting.balance.AY8910Volume;
            newParam.mixer.C140.Volume = setting.balance.C140Volume;
            newParam.mixer.HuC6280.Volume = setting.balance.HuC6280Volume;
            newParam.mixer.OKIM6258.Volume = setting.balance.OKIM6258Volume;
            newParam.mixer.OKIM6295.Volume = setting.balance.OKIM6295Volume;
            newParam.mixer.PWM.Volume = setting.balance.PWMVolume;
            newParam.mixer.RF5C164.Volume = setting.balance.RF5C164Volume;
            newParam.mixer.SEGAPCM.Volume = setting.balance.SEGAPCMVolume;
            newParam.mixer.SN76489.Volume = setting.balance.SN76489Volume;
            newParam.mixer.YM2151.Volume = setting.balance.YM2151Volume;
            newParam.mixer.YM2203FM.Volume = setting.balance.YM2203FMVolume;
            newParam.mixer.YM2203PSG.Volume = setting.balance.YM2203PSGVolume;
            newParam.mixer.YM2203.Volume = setting.balance.YM2203Volume;
            newParam.mixer.YM2413.Volume = setting.balance.YM2413Volume;
            newParam.mixer.YM2608Adpcm.Volume = setting.balance.YM2608AdpcmVolume;
            newParam.mixer.YM2608FM.Volume = setting.balance.YM2608FMVolume;
            newParam.mixer.YM2608PSG.Volume = setting.balance.YM2608PSGVolume;
            newParam.mixer.YM2608Rhythm.Volume = setting.balance.YM2608RhythmVolume;
            newParam.mixer.YM2608.Volume = setting.balance.YM2608Volume;
            newParam.mixer.YM2610AdpcmA.Volume = setting.balance.YM2610AdpcmAVolume;
            newParam.mixer.YM2610AdpcmB.Volume = setting.balance.YM2610AdpcmBVolume;
            newParam.mixer.YM2610FM.Volume = setting.balance.YM2610FMVolume;
            newParam.mixer.YM2610PSG.Volume = setting.balance.YM2610PSGVolume;
            newParam.mixer.YM2610.Volume = setting.balance.YM2610Volume;
            newParam.mixer.YM2612.Volume = setting.balance.YM2612Volume;
            newParam.mixer.C352.Volume = setting.balance.C352Volume;
            newParam.mixer.K054539.Volume = setting.balance.K054539Volume;

            newParam.mixer.Master.Volume = setting.balance.MasterVolume;
            newParam.mixer.Master.VisVolume1 = common.Range(Audio.masterVisVolume / 250, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.Master.VisVolume2 <= newParam.mixer.Master.VisVolume1)
            {
                newParam.mixer.Master.VisVolume2 = newParam.mixer.Master.VisVolume1;
                newParam.mixer.Master.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2151.VisVolume1 = common.Range(Audio.ym2151VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2151.VisVolume2 <= newParam.mixer.YM2151.VisVolume1)
            {
                newParam.mixer.YM2151.VisVolume2 = newParam.mixer.YM2151.VisVolume1;
                newParam.mixer.YM2151.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2203.VisVolume1 = common.Range(Audio.ym2203VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2203.VisVolume2 <= newParam.mixer.YM2203.VisVolume1)
            {
                newParam.mixer.YM2203.VisVolume2 = newParam.mixer.YM2203.VisVolume1;
                newParam.mixer.YM2203.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2203FM.VisVolume1 = common.Range(Audio.ym2203FMVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2203FM.VisVolume2 <= newParam.mixer.YM2203FM.VisVolume1)
            {
                newParam.mixer.YM2203FM.VisVolume2 = newParam.mixer.YM2203FM.VisVolume1;
                newParam.mixer.YM2203FM.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2203PSG.VisVolume1 = common.Range(Audio.ym2203SSGVisVolume / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2203PSG.VisVolume2 <= newParam.mixer.YM2203PSG.VisVolume1)
            {
                newParam.mixer.YM2203PSG.VisVolume2 = newParam.mixer.YM2203PSG.VisVolume1;
                newParam.mixer.YM2203PSG.VisVol2Cnt = 30;
            }


            newParam.mixer.YM2413.VisVolume1 = common.Range(Audio.ym2413VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2413.VisVolume2 <= newParam.mixer.YM2413.VisVolume1)
            {
                newParam.mixer.YM2413.VisVolume2 = newParam.mixer.YM2413.VisVolume1;
                newParam.mixer.YM2413.VisVol2Cnt = 30;
            }


            newParam.mixer.YM2608.VisVolume1 = common.Range(Audio.ym2608VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2608.VisVolume2 <= newParam.mixer.YM2608.VisVolume1)
            {
                newParam.mixer.YM2608.VisVolume2 = newParam.mixer.YM2608.VisVolume1;
                newParam.mixer.YM2608.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2608FM.VisVolume1 = common.Range(Audio.ym2608FMVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2608FM.VisVolume2 <= newParam.mixer.YM2608FM.VisVolume1)
            {
                newParam.mixer.YM2608FM.VisVolume2 = newParam.mixer.YM2608FM.VisVolume1;
                newParam.mixer.YM2608FM.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2608PSG.VisVolume1 = common.Range(Audio.ym2608SSGVisVolume / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2608PSG.VisVolume2 <= newParam.mixer.YM2608PSG.VisVolume1)
            {
                newParam.mixer.YM2608PSG.VisVolume2 = newParam.mixer.YM2608PSG.VisVolume1;
                newParam.mixer.YM2608PSG.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2608Rhythm.VisVolume1 = common.Range(Audio.ym2608RtmVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2608Rhythm.VisVolume2 <= newParam.mixer.YM2608Rhythm.VisVolume1)
            {
                newParam.mixer.YM2608Rhythm.VisVolume2 = newParam.mixer.YM2608Rhythm.VisVolume1;
                newParam.mixer.YM2608Rhythm.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2608Adpcm.VisVolume1 = common.Range(Audio.ym2608APCMVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2608Adpcm.VisVolume2 <= newParam.mixer.YM2608Adpcm.VisVolume1)
            {
                newParam.mixer.YM2608Adpcm.VisVolume2 = newParam.mixer.YM2608Adpcm.VisVolume1;
                newParam.mixer.YM2608Adpcm.VisVol2Cnt = 30;
            }


            newParam.mixer.YM2610.VisVolume1 = common.Range(Audio.ym2610VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2610.VisVolume2 <= newParam.mixer.YM2610.VisVolume1)
            {
                newParam.mixer.YM2610.VisVolume2 = newParam.mixer.YM2610.VisVolume1;
                newParam.mixer.YM2610.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2610FM.VisVolume1 = common.Range(Audio.ym2610FMVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2610FM.VisVolume2 <= newParam.mixer.YM2610FM.VisVolume1)
            {
                newParam.mixer.YM2610FM.VisVolume2 = newParam.mixer.YM2610FM.VisVolume1;
                newParam.mixer.YM2610FM.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2610PSG.VisVolume1 = common.Range(Audio.ym2610SSGVisVolume / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2610PSG.VisVolume2 <= newParam.mixer.YM2610PSG.VisVolume1)
            {
                newParam.mixer.YM2610PSG.VisVolume2 = newParam.mixer.YM2610PSG.VisVolume1;
                newParam.mixer.YM2610PSG.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2610AdpcmA.VisVolume1 = common.Range(Audio.ym2610APCMAVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2610AdpcmA.VisVolume2 <= newParam.mixer.YM2610AdpcmA.VisVolume1)
            {
                newParam.mixer.YM2610AdpcmA.VisVolume2 = newParam.mixer.YM2610AdpcmA.VisVolume1;
                newParam.mixer.YM2610AdpcmA.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2610AdpcmB.VisVolume1 = common.Range(Audio.ym2610APCMBVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2610AdpcmB.VisVolume2 <= newParam.mixer.YM2610AdpcmB.VisVolume1)
            {
                newParam.mixer.YM2610AdpcmB.VisVolume2 = newParam.mixer.YM2610AdpcmB.VisVolume1;
                newParam.mixer.YM2610AdpcmB.VisVol2Cnt = 30;
            }

            newParam.mixer.YM2612.VisVolume1 = common.Range(Audio.ym2612VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.YM2612.VisVolume2 <= newParam.mixer.YM2612.VisVolume1)
            {
                newParam.mixer.YM2612.VisVolume2 = newParam.mixer.YM2612.VisVolume1;
                newParam.mixer.YM2612.VisVol2Cnt = 30;
            }

            newParam.mixer.AY8910.VisVolume1 = common.Range(Audio.ay8910VisVolume / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.AY8910.VisVolume2 <= newParam.mixer.AY8910.VisVolume1)
            {
                newParam.mixer.AY8910.VisVolume2 = newParam.mixer.AY8910.VisVolume1;
                newParam.mixer.AY8910.VisVol2Cnt = 30;
            }

            newParam.mixer.SN76489.VisVolume1 = common.Range(Audio.sn76489VisVolume / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.SN76489.VisVolume2 <= newParam.mixer.SN76489.VisVolume1)
            {
                newParam.mixer.SN76489.VisVolume2 = newParam.mixer.SN76489.VisVolume1;
                newParam.mixer.SN76489.VisVol2Cnt = 30;
            }

            newParam.mixer.HuC6280.VisVolume1 = common.Range(Audio.huc6280VisVolume / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.HuC6280.VisVolume2 <= newParam.mixer.HuC6280.VisVolume1)
            {
                newParam.mixer.HuC6280.VisVolume2 = newParam.mixer.HuC6280.VisVolume1;
                newParam.mixer.HuC6280.VisVol2Cnt = 30;
            }

            newParam.mixer.RF5C164.VisVolume1 = common.Range(Audio.rf5c164VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.RF5C164.VisVolume2 <= newParam.mixer.RF5C164.VisVolume1)
            {
                newParam.mixer.RF5C164.VisVolume2 = newParam.mixer.RF5C164.VisVolume1;
                newParam.mixer.RF5C164.VisVol2Cnt = 30;
            }

            newParam.mixer.PWM.VisVolume1 = common.Range(Audio.pwmVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.PWM.VisVolume2 <= newParam.mixer.PWM.VisVolume1)
            {
                newParam.mixer.PWM.VisVolume2 = newParam.mixer.PWM.VisVolume1;
                newParam.mixer.PWM.VisVol2Cnt = 30;
            }

            newParam.mixer.OKIM6258.VisVolume1 = common.Range(Audio.okim6258VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.OKIM6258.VisVolume2 <= newParam.mixer.OKIM6258.VisVolume1)
            {
                newParam.mixer.OKIM6258.VisVolume2 = newParam.mixer.OKIM6258.VisVolume1;
                newParam.mixer.OKIM6258.VisVol2Cnt = 30;
            }

            newParam.mixer.OKIM6295.VisVolume1 = common.Range(Audio.okim6295VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.OKIM6295.VisVolume2 <= newParam.mixer.OKIM6295.VisVolume1)
            {
                newParam.mixer.OKIM6295.VisVolume2 = newParam.mixer.OKIM6295.VisVolume1;
                newParam.mixer.OKIM6295.VisVol2Cnt = 30;
            }

            newParam.mixer.C140.VisVolume1 = common.Range(Audio.c140VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.C140.VisVolume2 <= newParam.mixer.C140.VisVolume1)
            {
                newParam.mixer.C140.VisVolume2 = newParam.mixer.C140.VisVolume1;
                newParam.mixer.C140.VisVol2Cnt = 30;
            }

            newParam.mixer.SEGAPCM.VisVolume1 = common.Range(Audio.segaPCMVisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.SEGAPCM.VisVolume2 <= newParam.mixer.SEGAPCM.VisVolume1)
            {
                newParam.mixer.SEGAPCM.VisVolume2 = newParam.mixer.SEGAPCM.VisVolume1;
                newParam.mixer.SEGAPCM.VisVol2Cnt = 30;
            }

            newParam.mixer.C352.VisVolume1 = common.Range(Audio.c352VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.C352.VisVolume2 <= newParam.mixer.C352.VisVolume1)
            {
                newParam.mixer.C352.VisVolume2 = newParam.mixer.C352.VisVolume1;
                newParam.mixer.C352.VisVol2Cnt = 30;
            }

            newParam.mixer.K054539.VisVolume1 = common.Range(Audio.k054539VisVolume / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.mixer.K054539.VisVolume2 <= newParam.mixer.K054539.VisVolume1)
            {
                newParam.mixer.K054539.VisVolume2 = newParam.mixer.K054539.VisVolume1;
                newParam.mixer.K054539.VisVol2Cnt = 30;
            }
        }


        private void screenDrawParams()
        {
            // 描画
            screen.drawParams(oldParam, newParam);

            screen.drawButtons(oldButton, newButton, oldButtonMode, newButtonMode);

            screen.drawTimer(0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            screen.drawTimer(1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            screen.drawTimer(2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            int[] chips = Audio.GetChipStatus();
            screen.drawChipName(14 * 4, 0 * 8, 0, ref oldParam.ChipPriOPN, chips[0]);
            screen.drawChipName(18 * 4, 0 * 8, 1, ref oldParam.ChipPriOPN2, chips[1]);
            screen.drawChipName(23 * 4, 0 * 8, 2, ref oldParam.ChipPriOPNA, chips[2]);
            screen.drawChipName(28 * 4, 0 * 8, 3, ref oldParam.ChipPriOPNB, chips[3]);
            screen.drawChipName(33 * 4, 0 * 8, 4, ref oldParam.ChipPriOPM, chips[4]);
            screen.drawChipName(37 * 4, 0 * 8, 5, ref oldParam.ChipPriDCSG, chips[5]);
            screen.drawChipName(42 * 4, 0 * 8, 6, ref oldParam.ChipPriRF5C, chips[6]);
            screen.drawChipName(47 * 4, 0 * 8, 7, ref oldParam.ChipPriPWM, chips[7]);
            screen.drawChipName(51 * 4, 0 * 8, 8, ref oldParam.ChipPriOKI5, chips[8]);
            screen.drawChipName(56 * 4, 0 * 8, 9, ref oldParam.ChipPriOKI9, chips[9]);
            screen.drawChipName(61 * 4, 0 * 8, 10, ref oldParam.ChipPriC140, chips[10]);
            screen.drawChipName(66 * 4, 0 * 8, 11, ref oldParam.ChipPriSPCM, chips[11]);
            screen.drawChipName(4 * 4, 0 * 8, 12, ref oldParam.ChipPriAY10, chips[12]);
            screen.drawChipName(9 * 4, 0 * 8, 13, ref oldParam.ChipPriOPLL, chips[13]);
            screen.drawChipName(71 * 4, 0 * 8, 14, ref oldParam.ChipPriHuC8, chips[14]);

            screen.drawChipName(14 * 4, 1 * 8, 0, ref oldParam.ChipSecOPN, chips[128 + 0]);
            screen.drawChipName(18 * 4, 1 * 8, 1, ref oldParam.ChipSecOPN2, chips[128 + 1]);
            screen.drawChipName(23 * 4, 1 * 8, 2, ref oldParam.ChipSecOPNA, chips[128 + 2]);
            screen.drawChipName(28 * 4, 1 * 8, 3, ref oldParam.ChipSecOPNB, chips[128 + 3]);
            screen.drawChipName(33 * 4, 1 * 8, 4, ref oldParam.ChipSecOPM, chips[128 + 4]);
            screen.drawChipName(37 * 4, 1 * 8, 5, ref oldParam.ChipSecDCSG, chips[128 + 5]);
            screen.drawChipName(42 * 4, 1 * 8, 6, ref oldParam.ChipSecRF5C, chips[128 + 6]);
            screen.drawChipName(47 * 4, 1 * 8, 7, ref oldParam.ChipSecPWM, chips[128 + 7]);
            screen.drawChipName(51 * 4, 1 * 8, 8, ref oldParam.ChipSecOKI5, chips[128 + 8]);
            screen.drawChipName(56 * 4, 1 * 8, 9, ref oldParam.ChipSecOKI9, chips[128 + 9]);
            screen.drawChipName(61 * 4, 1 * 8, 10, ref oldParam.ChipSecC140, chips[128 + 10]);
            screen.drawChipName(66 * 4, 1 * 8, 11, ref oldParam.ChipSecSPCM, chips[128 + 11]);
            screen.drawChipName(4 * 4, 1 * 8, 12, ref oldParam.ChipSecAY10, chips[128 + 12]);
            screen.drawChipName(9 * 4, 1 * 8, 13, ref oldParam.ChipSecOPLL, chips[128 + 13]);
            screen.drawChipName(71 * 4, 0 * 8, 14, ref oldParam.ChipSecHuC8, chips[128 + 14]);

            screen.drawFont4(screen.mainScreen, 0, 24, 1, Audio.GetIsDataBlock(enmModel.VirtualModel) ? "VD" : "  ");
            screen.drawFont4(screen.mainScreen, 12, 24, 1, Audio.GetIsPcmRAMWrite(enmModel.VirtualModel) ? "VP" : "  ");
            screen.drawFont4(screen.mainScreen, 0, 32, 1, Audio.GetIsDataBlock(enmModel.RealModel) ? "RD" : "  ");
            screen.drawFont4(screen.mainScreen, 12, 32, 1, Audio.GetIsPcmRAMWrite(enmModel.RealModel) ? "RP" : "  ");

            if (setting.Debug_DispFrameCounter)
            {
                long v = Audio.getVirtualFrameCounter();
                if (v != -1) screen.drawFont8(screen.mainScreen, 0, 0, 0, string.Format("EMU        : {0:D12} ", v));
                long r = Audio.getRealFrameCounter();
                if (r != -1) screen.drawFont8(screen.mainScreen, 0, 8, 0, string.Format("SCCI       : {0:D12} ", r));
                long d = r - v;
                if (r != -1 && v != -1) screen.drawFont8(screen.mainScreen, 0, 16, 0, string.Format("SCCI - EMU : {0:D12} ", d));
                screen.drawFont8(screen.mainScreen, 0, 24, 0, string.Format("PROC TIME  : {0:D12} ", Audio.ProcTimePer1Frame));
            }

            screen.Refresh();

            Audio.updateVol();


        }

        private int searchFMNote(int freq)
        {
            int m = int.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 5; i++)
            {
                //if (freq < Tables.FmFNum[i]) break;
                //n = i;
                int a = Math.Abs(freq - Tables.FmFNum[i]);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n - 12 * 3;
        }

        private int searchPSGNote(int freq)
        {
            int m = int.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                int a = Math.Abs(freq - Tables.PsgFNum[i]);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        private int searchSSGNote(float freq)
        {
            float m = float.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                //if (freq < Tables.freqTbl[i]) break;
                //n = i;
                float a = Math.Abs(freq - Tables.freqTbl[i]);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        private int searchRf5c164Note(uint freq)
        {
            double m = double.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                double a = Math.Abs(freq - (0x0800 * Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        private int searchC140Note(int freq)
        {
            double m = double.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                double a = Math.Abs(freq - ((0x0800 << 2) * Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        private int searchSegaPCMNote(double ml)
        {
            double m = double.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                double a = Math.Abs(ml - (Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        private int searchYM2608Adpcm(float freq)
        {
            float m = float.MaxValue;
            int n = 0;

            for (int i = 0; i < 12 * 8; i++)
            {
                if (freq < Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 3))) break;
                n = i;
                float a = Math.Abs(freq - (float)(Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 3))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }

            return n + 1;
        }



        private void openSetting()
        {
            frmSetting frm = new frmSetting(setting);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                StopMIDIInMonitoring();
                frmPlayList.Stop();
                Audio.Stop();
                Audio.Close();

                setting = frm.setting;
                setting.Save();

                screen.setting = setting;
                //oldParam = new MDChipParams();
                //newParam = new MDChipParams();
                screen.screenInitAll();

                log.ForcedWrite("設定が変更されたため、再度Audio初期化処理開始");

                Audio.Init(setting);

                frmMixer.balance = setting.balance;
                frmMixer.setting = setting;

                log.ForcedWrite("Audio初期化処理完了");
                log.debug = setting.Debug_DispFrameCounter;

                frmVSTeffectList.dispPluginList();
                StartMIDIInMonitoring();

                IsInitialOpenFolder = true;

            }
        }

        public void stop()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            frmPlayList.Stop();
            Audio.Stop();

        }

        public void pause()
        {
            Audio.Pause();
        }

        public void fadeout()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            Audio.Fadeout();
        }

        public void prev()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            frmPlayList.prevPlay();
        }

        public void play()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            string[] fn = null;
            Tuple<int, string, string> playFn = null;

            frmPlayList.Stop();

            //if (srcBuf == null && frmPlayList.getMusicCount() < 1)
            if (frmPlayList.getMusicCount() < 1)
            {
                fn = fileOpen(false);
                if (fn == null) return;
                frmPlayList.AddList(fn[0]);
                playFn = frmPlayList.setStart(-1); //last
            }
            else
            {
                fn = new string[1] { "" };
                playFn = frmPlayList.setStart(-2);//first 
            }

            oldParam = new MDChipParams();
            screen.drawTimer(0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            screen.drawTimer(1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            screen.drawTimer(2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            loadAndPlay(playFn.Item1, playFn.Item2, playFn.Item3);
            frmPlayList.Play();

        }

        private void playdata()
        {

            if (srcBuf == null)
            {
                return;
            }

            if (Audio.isPaused)
            {
                Audio.Pause();
            }
            stop();

            for (int chipID = 0; chipID < 2; chipID++)
            {
                for (int ch = 0; ch < 8; ch++) ResetChannelMask(enmUseChip.YM2151, chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(enmUseChip.YM2203, chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(enmUseChip.YM2413, chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask(enmUseChip.YM2608, chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask(enmUseChip.YM2610, chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(enmUseChip.YM2612, chipID, ch);
                for (int ch = 0; ch < 4; ch++) ResetChannelMask(enmUseChip.SN76489, chipID, ch);
                for (int ch = 0; ch < 8; ch++) ResetChannelMask(enmUseChip.RF5C164, chipID, ch);
                for (int ch = 0; ch < 24; ch++) ResetChannelMask(enmUseChip.C140, chipID, ch);
                for (int ch = 0; ch < 16; ch++) ResetChannelMask(enmUseChip.SEGAPCM, chipID, ch);
                for (int ch = 0; ch < 6; ch++) ResetChannelMask(enmUseChip.HuC6280, chipID, ch);
            }

            //oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            screen.screenInitAll();

            if (!Audio.Play(setting))
            {
                //MessageBox.Show("再生に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    frmPlayList.Stop();
                    Audio.Stop();
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                }
                throw new Exception();
                //return;
            }

            if (frmInfo != null)
            {
                frmInfo.update();
            }

            if (setting.other.AutoOpen)
            {

                if (Audio.ChipPriOPM != 0) OpenFormYM2151(0, true); else CloseFormYM2151(0);
                if (Audio.ChipSecOPM != 0) OpenFormYM2151(1, true); else CloseFormYM2151(1);

                if (Audio.ChipPriOPN != 0) OpenFormYM2203(0, true); else CloseFormYM2203(0);
                if (Audio.ChipSecOPN != 0) OpenFormYM2203(1, true); else CloseFormYM2203(1);

                if (Audio.ChipPriOPLL != 0) OpenFormYM2413(0, true); else CloseFormYM2413(0);
                if (Audio.ChipSecOPLL != 0) OpenFormYM2413(1, true); else CloseFormYM2413(1);

                if (Audio.ChipPriOPNA != 0) OpenFormYM2608(0, true); else CloseFormYM2608(0);
                if (Audio.ChipSecOPNA != 0) OpenFormYM2608(1, true); else CloseFormYM2608(1);

                if (Audio.ChipPriOPNB != 0) OpenFormYM2610(0, true); else CloseFormYM2610(0);
                if (Audio.ChipSecOPNB != 0) OpenFormYM2610(1, true); else CloseFormYM2610(1);

                if (Audio.ChipPriOPN2 != 0) OpenFormYM2612(0, true); else CloseFormYM2612(0);
                if (Audio.ChipSecOPN2 != 0) OpenFormYM2612(1, true); else CloseFormYM2612(1);

                if (Audio.ChipPriDCSG != 0) OpenFormSN76489(0, true); else CloseFormSN76489(0);
                if (Audio.ChipSecDCSG != 0) OpenFormSN76489(1, true); else CloseFormSN76489(1);

                if (Audio.ChipPriRF5C != 0) OpenFormMegaCD(0, true); else CloseFormMegaCD(0);
                if (Audio.ChipSecRF5C != 0) OpenFormMegaCD(1, true); else CloseFormMegaCD(1);

                if (Audio.ChipPriOKI5 != 0) OpenFormOKIM6258(0, true); else CloseFormOKIM6258(0);
                if (Audio.ChipSecOKI5 != 0) OpenFormOKIM6258(1, true); else CloseFormOKIM6258(1);

                if (Audio.ChipPriOKI9 != 0) OpenFormOKIM6295(0, true); else CloseFormOKIM6295(0);
                if (Audio.ChipSecOKI9 != 0) OpenFormOKIM6295(1, true); else CloseFormOKIM6295(1);

                if (Audio.ChipPriC140 != 0) OpenFormC140(0, true); else CloseFormC140(0);
                if (Audio.ChipSecC140 != 0) OpenFormC140(1, true); else CloseFormC140(1);

                if (Audio.ChipPriSPCM != 0) OpenFormSegaPCM(0, true); else CloseFormSegaPCM(0);
                if (Audio.ChipSecSPCM != 0) OpenFormSegaPCM(1, true); else CloseFormSegaPCM(1);

                if (Audio.ChipPriAY10 != 0) OpenFormAY8910(0, true); else CloseFormAY8910(0);
                if (Audio.ChipSecAY10 != 0) OpenFormAY8910(1, true); else CloseFormAY8910(1);

                if (Audio.ChipPriHuC != 0) OpenFormHuC6280(0, true); else CloseFormHuC6280(0);
                if (Audio.ChipSecHuC != 0) OpenFormHuC6280(1, true); else CloseFormHuC6280(1);

            }
        }

        public void ff()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            Audio.FF();
        }

        public void next()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            frmPlayList.nextPlay();
        }

        private void nextPlayMode()
        {
            frmPlayList.nextPlayMode(newButtonMode[9]);
        }

        public void slow()
        {
            if (Audio.isPaused)
            {
                Audio.StepPlay(4000);
                Audio.Pause();
                return;
            }

            if (Audio.isStopped)
            {
                play();
            }

            Audio.Slow();
        }

        private void playMode()
        {
            newButtonMode[9]++;
            if (newButtonMode[9] > 3) newButtonMode[9] = 0;

        }

        private string[] fileOpen(bool flg)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "VGMファイル(*.vgm;*.vgz;*.zip)|*.vgm;*.vgz;*.zip|NRDファイル(*.nrd)|*.nrd|XGMファイル(*.xgm)|*.xgm|すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            ofd.FilterIndex = setting.other.FilterIndex;
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

        private void dispPlayList()
        {
            frmPlayList.setting = setting;
            if (setting.location.PPlayList != System.Drawing.Point.Empty)
            {
                frmPlayList.Location = setting.location.PPlayList;

            }
            if (setting.location.PPlayListWH != System.Drawing.Point.Empty)
            {
                frmPlayList.Width = setting.location.PPlayListWH.X;
                frmPlayList.Height = setting.location.PPlayListWH.Y;
            }
            frmPlayList.Visible = !frmPlayList.Visible;
            frmPlayList.TopMost = true;
            frmPlayList.TopMost = false;
        }

        private void dispVSTList()
        {
            frmVSTeffectList.Visible = !frmVSTeffectList.Visible;
            frmVSTeffectList.TopMost = true;
            frmVSTeffectList.TopMost = false;
        }

        private void dispMixer()
        {
            frmMixer.setting = setting;
            if (!frmMixer.Visible)
            {
                if (setting.location.PMixer != System.Drawing.Point.Empty)
                {
                    frmMixer.Location = setting.location.PMixer;
                }
                if (setting.location.PMixerWH != System.Drawing.Point.Empty)
                {
                    frmMixer.Width = setting.location.PMixerWH.X;
                    frmMixer.Height = setting.location.PMixerWH.Y;
                }
            }
            else
            {
                if (setting.location.PMixer != System.Drawing.Point.Empty)
                {
                    setting.location.PMixer = frmMixer.Location;
                }
                if (setting.location.PMixerWH != System.Drawing.Point.Empty)
                {
                    setting.location.PMixerWH = new System.Drawing.Point(frmMixer.Width, frmMixer.Height);
                }
            }

            frmMixer.Visible = !frmMixer.Visible;

            Screen s = Screen.FromControl(frmMixer);
            //ディスプレイの高さと幅を取得
            int h = s.Bounds.Height;
            int w = s.Bounds.Width;
            if (frmMixer.Location.X > w - 100 || frmMixer.Location.Y > h - 100)
            {
                frmMixer.Location = new System.Drawing.Point(0, 0);
            }

            frmMixer.TopMost = true;
            frmMixer.TopMost = false;
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
            //ディスプレイの高さと幅を取得
            int h = s.Bounds.Height;
            int w = s.Bounds.Width;
            if (frmInfo.x > w - 100 || frmInfo.y > h - 100)
            {
                frmInfo.x = 0;
                frmInfo.y = 0;
            }

            frmInfo.setting = setting;
            frmInfo.Show();
            frmInfo.update();
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

            frmYM2612MIDI = new frmYM2612MIDI(this, setting.other.Zoom);
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
            //ディスプレイの高さと幅を取得
            int h = s.Bounds.Height;
            int w = s.Bounds.Width;
            if (frmYM2612MIDI.x > w - 100 || frmYM2612MIDI.y > h - 100)
            {
                frmYM2612MIDI.x = 0;
                frmYM2612MIDI.y = 0;
            }

            //frmYM2612MIDI.setting = setting;
            screen.AddYM2612MIDI(frmYM2612MIDI.pbScreen, Properties.Resources.planeYM2612MIDI);
            frmYM2612MIDI.Show();
            frmYM2612MIDI.update();
            screen.screenInitYM2612MIDI();
            oldParam.ym2612Midi = new MDChipParams.YM2612MIDI();
        }

        private void showContextMenu()
        {
            cmsOpenOtherPanel.Show();
            System.Drawing.Point p = Control.MousePosition;
            cmsOpenOtherPanel.Top = p.Y;
            cmsOpenOtherPanel.Left = p.X;
        }


        public const int FCC_VGM = 0x206D6756;	// "Vgm "

        public byte[] getAllBytes(string filename, out enmFileFormat format)
        {
            format = enmFileFormat.unknown;

            //先ずは丸ごと読み込む
            byte[] buf = System.IO.File.ReadAllBytes(filename);


            //.NRDファイルの場合は拡張子判定
            if (filename.ToLower().LastIndexOf(".nrd") != -1)
            {
                format = enmFileFormat.NRTDRV;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".xgm") != -1)
            {
                format = enmFileFormat.XGM;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".s98") != -1)
            {
                format = enmFileFormat.S98;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".mid") != -1)
            {
                format = enmFileFormat.MID;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".rcp") != -1)
            {
                format = enmFileFormat.RCP;
                return buf;
            }


            //.VGMの場合はヘッダの確認とGzipで解凍後のファイルのヘッダの確認
            uint vgm = (UInt32)buf[0] + (UInt32)buf[1] * 0x100 + (UInt32)buf[2] * 0x10000 + (UInt32)buf[3] * 0x1000000;
            if (vgm == FCC_VGM)
            {
                format = enmFileFormat.VGM;
                return buf;
            }

            int num;
            buf = new byte[1024]; // 1Kbytesずつ処理する

            FileStream inStream // 入力ストリーム
              = new FileStream(filename, FileMode.Open, FileAccess.Read);

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

            format = enmFileFormat.VGM;
            return outStream.ToArray();
        }

        public void getInstCh(enmUseChip chip, int ch, int chipID)
        {

            YM2612MIDI.SetVoiceFromChipRegister(chip, chipID, ch);

            if (!setting.other.UseGetInst) return;

            switch (setting.other.InstFormat)
            {
                case enmInstFormat.FMP7:
                    getInstChForFMP7(chip, ch, chipID);
                    break;
                case enmInstFormat.MDX:
                    getInstChForMDX(chip, ch, chipID);
                    break;
                case enmInstFormat.MML2VGM:
                    getInstChForMML2VGM(chip, ch, chipID);
                    break;
                case enmInstFormat.MUSICLALF:
                    getInstChForMUSICLALF(chip, ch, chipID);
                    break;
                case enmInstFormat.MUSICLALF2:
                    getInstChForMUSICLALF2(chip, ch, chipID);
                    break;
                case enmInstFormat.TFI:
                    getInstChForTFI(chip, ch, chipID);
                    break;
                case enmInstFormat.NRTDRV:
                    getInstChForNRTDRV(chip, ch, chipID);
                    break;
                case enmInstFormat.HUSIC:
                    getInstChForHuSIC(chip, ch, chipID);
                    break;
            }
        }

        private void getInstChForFMP7(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
                        , fmRegister[p][0x40 + ops + c] & 0x7f//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , fmRegister[p][0xb0 + c] & 0x07//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);
                n = "'@ FC xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AM\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , ym2151Register[0x60 + ops + ch] & 0x7f //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , ym2151Register[0x20 + ch] & 0x07 //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }

            if (string.IsNullOrEmpty(n)) return;
            Clipboard.SetText(n);
        }

        private void getInstChForMDX(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "'@xx = {\r\n/* AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("   {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , fmRegister[p][0x40 + ops + c] & 0x7f//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , 0
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                    );
                }
                n += "/* ALG FB  OP\r\n";
                n += string.Format("   {0:D3},{1:D3},15\r\n}}\r\n"
                    , fmRegister[p][0xb0 + c] & 0x07//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                n = "'@xx = {\r\n/* AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("   {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , ym2151Register[0x60 + ops + ch] & 0x7f //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "/* ALG FB  OP\r\n";
                n += string.Format("   {0:D3},{1:D3},15\r\n}}\r\n"
                    , ym2151Register[0x20 + ch] & 0x07 //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }

            Clipboard.SetText(n);
        }

        private void getInstChForMML2VGM(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "'@ N xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSG-EG\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , fmRegister[p][0x50 + ops + c] & 0x1f //AR
                        , fmRegister[p][0x60 + ops + c] & 0x1f //DR
                        , fmRegister[p][0x70 + ops + c] & 0x1f //SR
                        , fmRegister[p][0x80 + ops + c] & 0x0f //RR
                        , (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4//SL
                        , fmRegister[p][0x40 + ops + c] & 0x7f//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                        , fmRegister[p][0x90 + ops + c] & 0x0f//SG
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , fmRegister[p][0xb0 + c] & 0x07//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
                );
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);
                n = "'@ M xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME\r\n";

                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    n += string.Format("'@ {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
                        , ym2151Register[0x80 + ops + ch] & 0x1f //AR
                        , ym2151Register[0xa0 + ops + ch] & 0x1f //DR
                        , ym2151Register[0xc0 + ops + ch] & 0x1f //SR
                        , ym2151Register[0xe0 + ops + ch] & 0x0f //RR
                        , (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4 //SL
                        , ym2151Register[0x60 + ops + ch] & 0x7f //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT1
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , ym2151Register[0x20 + ch] & 0x07 //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }
            else if (chip == enmUseChip.HuC6280)
            {
                MDSound.Ootake_PSG.huc6280_state huc6280Register = Audio.GetHuC6280Register(chipID);
                if (huc6280Register == null) return;
                MDSound.Ootake_PSG.PSG psg = huc6280Register.Psg[ch];
                if (psg == null) return;
                if (psg.wave == null) return;
                if (psg.wave.Length != 32) return;

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

            if (n != null) Clipboard.SetText(n);
        }

        private void getInstChForMUSICLALF(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = string.Format("@xx:{{\r\n  {0:D3} {1:D3}\r\n"
                    , fmRegister[p][0xb0 + c] & 0x07//AL
                    , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
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
                        , fmRegister[p][0x40 + ops + c] & 0x7f//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                    );
                }
                n += "}\r\n";
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                n = string.Format("@xx:{{\r\n  {0:D3} {1:D3}\r\n"
                    , ym2151Register[0x20 + ch] & 0x07 //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
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
                        , ym2151Register[0x60 + ops + ch] & 0x7f //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                    );
                }
                n += "}\r\n";
            }

            Clipboard.SetText(n);
        }

        private void getInstChForMUSICLALF2(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "@%xxx\r\n";

                for (int i = 0; i < 6; i++)
                {
                    n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                        , fmRegister[p][0x30 + 0 + c + i * 0x10] & 0xff
                        , fmRegister[p][0x30 + 8 + c + i * 0x10] & 0xff
                        , fmRegister[p][0x30 + 16 + c + i * 0x10] & 0xff
                        , fmRegister[p][0x30 + 24 + c + i * 0x10] & 0xff
                    );
                }
                n += string.Format("${0:X3}\r\n"
                    , fmRegister[p][0xb0 + c] //FB/AL
                    );
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                n = "@%xxx\r\n";

                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0x40 + 0 + ch] & 0x7f) //DT/ML
                    , (ym2151Register[0x40 + 8 + ch] & 0x7f) //DT/ML
                    , (ym2151Register[0x40 + 16 + ch] & 0x7f)//DT/ML
                    , (ym2151Register[0x40 + 24 + ch] & 0x7f)//DT/ML
                );
                n += string.Format("${0:X3},${1:X3},${2:X3},${3:X3}\r\n"
                    , (ym2151Register[0x60 + 0 + ch] & 0x7f) //TL
                    , (ym2151Register[0x60 + 8 + ch] & 0x7f) //TL
                    , (ym2151Register[0x60 + 16 + ch] & 0x7f)//TL
                    , (ym2151Register[0x60 + 24 + ch] & 0x7f)//TL
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

            Clipboard.SetText(n);
        }

        private void getInstChForNRTDRV(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "@ xxxx {\r\n";
                n += string.Format("000,{0:D3},{1:D3},015\r\n"
                    , fmRegister[p][0xb0 + c] & 0x07//AL
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
                        , fmRegister[p][0x40 + ops + c] & 0x7f//TL
                        , (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6//KS
                        , fmRegister[p][0x30 + ops + c] & 0x0f//ML
                        , (fmRegister[p][0x30 + ops + c] & 0x70) >> 4//DT
                        , 0
                        , (fmRegister[p][0x60 + ops + c] & 0x80) >> 7//AM
                    );
                }
                n += "}\r\n";
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                n = "@ xxxx {\r\n";
                n += string.Format("000,{0:D3},{1:D3},015\r\n"
                    , ym2151Register[0x20 + ch] & 0x07 //AL
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
                        , ym2151Register[0x60 + ops + ch] & 0x7f //TL
                        , (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6 //KS
                        , ym2151Register[0x40 + ops + ch] & 0x0f //ML
                        , (ym2151Register[0x40 + ops + ch] & 0x70) >> 4 //DT
                        , (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6 //DT2
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                    );
                }
                n += "}\r\n";
            }

            Clipboard.SetText(n);
        }

        private void getInstChForHuSIC(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.HuC6280)
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

                n = n.Substring(0, n.Length - 3) + "\r\n}\r\n";
            }

            Clipboard.SetText(n);
        }

        private void getInstChForTFI(enmUseChip chip, int ch, int chipID)
        {

            byte[] n = new byte[42];

            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2203 || chip == enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n[0] = (byte)(fmRegister[p][0xb0 + c] & 0x07);//AL
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
                    n[i * 10 + 4] = (byte)(fmRegister[p][0x40 + ops + c] & 0x7f);//TL
                    n[i * 10 + 5] = (byte)((fmRegister[p][0x50 + ops + c] & 0xc0) >> 6);//KS
                    n[i * 10 + 6] = (byte)(fmRegister[p][0x50 + ops + c] & 0x1f); //AR
                    n[i * 10 + 7] = (byte)(fmRegister[p][0x60 + ops + c] & 0x1f); //DR
                    n[i * 10 + 8] = (byte)(fmRegister[p][0x70 + ops + c] & 0x1f); //SR
                    n[i * 10 + 9] = (byte)(fmRegister[p][0x80 + ops + c] & 0x0f); //RR
                    n[i * 10 + 10] = (byte)((fmRegister[p][0x80 + ops + c] & 0xf0) >> 4);//SL
                    n[i * 10 + 11] = (byte)(fmRegister[p][0x90 + ops + c] & 0x0f);//SSG
                }

            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);

                n[0] = (byte)(ym2151Register[0x20 + ch] & 0x07);//AL
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
                    n[i * 10 + 4] = (byte)(ym2151Register[0x60 + ops + ch] & 0x7f);//TL
                    n[i * 10 + 5] = (byte)((ym2151Register[0x80 + ops + ch] & 0xc0) >> 6);//KS
                    n[i * 10 + 6] = (byte)(ym2151Register[0x80 + ops + ch] & 0x1f); //AR
                    n[i * 10 + 7] = (byte)(ym2151Register[0xa0 + ops + ch] & 0x1f); //DR
                    n[i * 10 + 8] = (byte)(ym2151Register[0xc0 + ops + ch] & 0x1f); //SR
                    n[i * 10 + 9] = (byte)(ym2151Register[0xe0 + ops + ch] & 0x0f); //RR
                    n[i * 10 + 10] = (byte)((ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4);//SL
                    n[i * 10 + 11] = 0;
                }

            }

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "音色ファイル.tfi";
            sfd.Filter = "TFIファイル(*.tfi)|*.tfi|すべてのファイル(*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (System.IO.FileStream fs = new System.IO.FileStream(
                sfd.FileName,
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write))
            {

                fs.Write(n, 0, n.Length);

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
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = IntPtr.Zero;
            cds.lpData = str;
            cds.cbData = str.Length * sizeof(char);
            //受信側ではlpDataの文字列を(cbData/2)の長さでstring.Substring()する

            IntPtr myWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            SendMessage(targetWindowHandle, WM_COPYDATA, myWindowHandle, ref cds);
        }

        public void windowsMessage(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                string sParam = ReceiveString(m);
                try
                {

                    frmPlayList.Stop();

                    PlayList pl = frmPlayList.getPlayList();
                    if (pl.lstMusic.Count < 1 || pl.lstMusic[pl.lstMusic.Count - 1].fileName != sParam)
                    {
                        frmPlayList.AddList(sParam);
                    }

                    if (!loadAndPlay(0, sParam))
                    {
                        frmPlayList.Stop();
                        Audio.Stop();
                        return;
                    }

                    frmPlayList.setStart(-1);
                    oldParam = new MDChipParams();

                    frmPlayList.Play();

                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    //メッセージによる読み込み失敗の場合は何も表示しない
                    //                    MessageBox.Show("ファイルの読み込みに失敗しました。");
                }
            }

        }

        //メッセージ処理
        protected override void WndProc(ref Message m)
        {
            windowsMessage(ref m);
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
                str = str.Substring(0, cds.cbData / 2);
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

        public bool loadAndPlay(int m , string fn, string zfn = null)
        {
            try
            {
                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                string outMIDIFn = fn;
                enmFileFormat format = enmFileFormat.unknown;

                if (zfn == null || zfn == "")
                {
                    srcBuf = getAllBytes(fn, out format);
                }
                else
                {
                    using (ZipArchive archive = ZipFile.OpenRead(zfn))
                    {
                        ZipArchiveEntry entry = archive.GetEntry(fn);
                        string arcFn = "";
                        srcBuf = getBytesFromZipFile(entry, out arcFn);
                        if (arcFn != "") outMIDIFn = arcFn;
                        format = enmFileFormat.VGM;
                    }
                }

                Audio.SetVGMBuffer(format, srcBuf, outMIDIFn, m);
                newParam.fileFormat = format;

                if (srcBuf != null)
                {
                    this.Invoke((Action)playdata);
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                srcBuf = null;
                MessageBox.Show("ファイルの読み込みに失敗しました。", "MDPlayer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public byte[] getBytesFromZipFile(ZipArchiveEntry entry, out string arcFn)
        {
            byte[] buf = null;
            arcFn = "";
            if (entry.FullName.EndsWith(".vgm", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(".vgz", StringComparison.OrdinalIgnoreCase))
            {
                //Console.WriteLine(entry.FullName);
                arcFn = entry.FullName;
                using (BinaryReader reader = new BinaryReader(entry.Open()))
                {
                    buf = reader.ReadBytes((int)entry.Length);
                }

                uint vgm = (UInt32)buf[0] + (UInt32)buf[1] * 0x100 + (UInt32)buf[2] * 0x10000 + (UInt32)buf[3] * 0x1000000;
                if (vgm != FCC_VGM)
                {
                    int num;
                    buf = new byte[1024]; // 1Kbytesずつ処理する

                    Stream inStream // 入力ストリーム
                      = entry.Open();

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

            }

            return buf;
        }

        public void SetChannelMask(enmUseChip chip, int chipID, int ch)
        {
            switch (chip)
            {
                case enmUseChip.YM2203:
                    if (ch >= 0 && ch < 9)
                    {
                        if (!newParam.ym2203[chipID].channels[ch].mask)
                            Audio.setYM2203Mask(chipID, ch);
                        else
                            Audio.resetYM2203Mask(chipID, ch);

                        newParam.ym2203[chipID].channels[ch].mask = !newParam.ym2203[chipID].channels[ch].mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2203[chipID].channels[2].mask = newParam.ym2203[chipID].channels[ch].mask;
                            newParam.ym2203[chipID].channels[6].mask = newParam.ym2203[chipID].channels[ch].mask;
                            newParam.ym2203[chipID].channels[7].mask = newParam.ym2203[chipID].channels[ch].mask;
                            newParam.ym2203[chipID].channels[8].mask = newParam.ym2203[chipID].channels[ch].mask;
                        }
                    }
                    break;
                case enmUseChip.YM2413:
                    if (ch >= 0 && ch < 9)
                    {
                        if (!newParam.ym2413[chipID].channels[ch].mask)
                            Audio.setYM2413Mask(chipID, ch);
                        else
                            Audio.resetYM2413Mask(chipID, ch);

                        newParam.ym2413[chipID].channels[ch].mask = !newParam.ym2413[chipID].channels[ch].mask;
                    }
                    break;
                case enmUseChip.YM2608:
                    if (ch >= 0 && ch < 14)
                    {
                        if (!newParam.ym2608[chipID].channels[ch].mask)
                            Audio.setYM2608Mask(chipID, ch);
                        else
                            Audio.resetYM2608Mask(chipID, ch);

                        newParam.ym2608[chipID].channels[ch].mask = !newParam.ym2608[chipID].channels[ch].mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2608[chipID].channels[2].mask = newParam.ym2608[chipID].channels[ch].mask;
                            newParam.ym2608[chipID].channels[9].mask = newParam.ym2608[chipID].channels[ch].mask;
                            newParam.ym2608[chipID].channels[10].mask = newParam.ym2608[chipID].channels[ch].mask;
                            newParam.ym2608[chipID].channels[11].mask = newParam.ym2608[chipID].channels[ch].mask;
                        }
                    }
                    break;
                case enmUseChip.YM2610:
                    if (ch >= 0 && ch < 14)
                    {
                        int c = ch;
                        if (ch == 12) c = 13;
                        if (ch == 13) c = 12;

                        if (!newParam.ym2610[chipID].channels[c].mask)
                            Audio.setYM2610Mask(chipID, ch);
                        else
                            Audio.resetYM2610Mask(chipID, ch);
                        newParam.ym2610[chipID].channels[c].mask = !newParam.ym2610[chipID].channels[c].mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 9 && ch < 12))
                        {
                            newParam.ym2610[chipID].channels[2].mask = newParam.ym2610[chipID].channels[ch].mask;
                            newParam.ym2610[chipID].channels[9].mask = newParam.ym2610[chipID].channels[ch].mask;
                            newParam.ym2610[chipID].channels[10].mask = newParam.ym2610[chipID].channels[ch].mask;
                            newParam.ym2610[chipID].channels[11].mask = newParam.ym2610[chipID].channels[ch].mask;
                        }
                    }
                    break;
                case enmUseChip.YM2612:
                    if (ch >= 0 && ch < 9)
                    {
                        if (!newParam.ym2612[chipID].channels[ch].mask)
                            Audio.setYM2612Mask(chipID, ch);
                        else
                            Audio.resetYM2612Mask(chipID, ch);

                        newParam.ym2612[chipID].channels[ch].mask = !newParam.ym2612[chipID].channels[ch].mask;

                        //FM(2ch) FMex
                        if ((ch == 2) || (ch >= 6 && ch < 9))
                        {
                            newParam.ym2612[chipID].channels[2].mask = newParam.ym2612[chipID].channels[ch].mask;
                            newParam.ym2612[chipID].channels[6].mask = newParam.ym2612[chipID].channels[ch].mask;
                            newParam.ym2612[chipID].channels[7].mask = newParam.ym2612[chipID].channels[ch].mask;
                            newParam.ym2612[chipID].channels[8].mask = newParam.ym2612[chipID].channels[ch].mask;
                        }
                    }
                    break;
                case enmUseChip.SN76489:
                    if (!newParam.sn76489[chipID].channels[ch].mask)
                    {
                        Audio.setSN76489Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetSN76489Mask(chipID, ch);
                    }
                    newParam.sn76489[chipID].channels[ch].mask = !newParam.sn76489[chipID].channels[ch].mask;
                    break;
                case enmUseChip.RF5C164:
                    if (!newParam.rf5c164[chipID].channels[ch].mask)
                    {
                        Audio.setRF5C164Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetRF5C164Mask(chipID, ch);
                    }
                    newParam.rf5c164[chipID].channels[ch].mask = !newParam.rf5c164[chipID].channels[ch].mask;
                    break;
                case enmUseChip.YM2151:
                    if (!newParam.ym2151[chipID].channels[ch].mask)
                    {
                        Audio.setYM2151Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetYM2151Mask(chipID, ch);
                    }
                    newParam.ym2151[chipID].channels[ch].mask = !newParam.ym2151[chipID].channels[ch].mask;
                    break;
                case enmUseChip.C140:
                    if (!newParam.c140[chipID].channels[ch].mask)
                    {
                        Audio.setC140Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetC140Mask(chipID, ch);
                    }
                    newParam.c140[chipID].channels[ch].mask = !newParam.c140[chipID].channels[ch].mask;
                    break;
                case enmUseChip.SEGAPCM:
                    if (!newParam.segaPcm[chipID].channels[ch].mask)
                    {
                        Audio.setSegaPCMMask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetSegaPCMMask(chipID, ch);
                    }
                    newParam.segaPcm[chipID].channels[ch].mask = !newParam.segaPcm[chipID].channels[ch].mask;
                    break;
                case enmUseChip.AY8910:
                    if (!newParam.ay8910[chipID].channels[ch].mask)
                    {
                        Audio.setAY8910Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetAY8910Mask(chipID, ch);
                    }
                    newParam.ay8910[chipID].channels[ch].mask = !newParam.ay8910[chipID].channels[ch].mask;
                    break;
                case enmUseChip.HuC6280:
                    if (!newParam.huc6280[chipID].channels[ch].mask)
                    {
                        Audio.setHuC6280Mask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetHuC6280Mask(chipID, ch);
                    }
                    newParam.huc6280[chipID].channels[ch].mask = !newParam.huc6280[chipID].channels[ch].mask;
                    break;
            }
        }

        public void ResetChannelMask(enmUseChip chip, int chipID, int ch)
        {
            switch (chip)
            {
                case enmUseChip.SN76489:
                    newParam.sn76489[chipID].channels[ch].mask = false;
                    Audio.resetSN76489Mask(chipID, ch);
                    break;
                case enmUseChip.RF5C164:
                    newParam.rf5c164[chipID].channels[ch].mask = false;
                    Audio.resetRF5C164Mask(chipID, ch);
                    break;
                case enmUseChip.YM2151:
                    newParam.ym2151[chipID].channels[ch].mask = false;
                    Audio.resetYM2151Mask(chipID, ch);
                    break;
                case enmUseChip.YM2203:
                    newParam.ym2203[chipID].channels[ch].mask = false;
                    Audio.resetYM2203Mask(chipID, ch);
                    break;
                case enmUseChip.YM2413:
                    newParam.ym2413[chipID].channels[ch].mask = false;
                    Audio.resetYM2413Mask(chipID, ch);
                    break;
                case enmUseChip.YM2608:
                    newParam.ym2608[chipID].channels[ch].mask = false;
                    Audio.resetYM2608Mask(chipID, ch);
                    break;
                case enmUseChip.YM2610:
                    if (ch < 12)
                        newParam.ym2610[chipID].channels[ch].mask = false;
                    else if (ch == 12)
                        newParam.ym2610[chipID].channels[13].mask = false;
                    else if (ch == 13)
                        newParam.ym2610[chipID].channels[12].mask = false;

                    Audio.resetYM2610Mask(chipID, ch);
                    break;
                case enmUseChip.YM2612:
                    newParam.ym2612[chipID].channels[ch].mask = false;
                    if (ch == 2)
                    {
                        newParam.ym2612[chipID].channels[6].mask = false;
                        newParam.ym2612[chipID].channels[7].mask = false;
                        newParam.ym2612[chipID].channels[8].mask = false;
                    }
                    if (ch < 6) Audio.resetYM2612Mask(chipID, ch);
                    break;
                case enmUseChip.C140:
                    newParam.c140[chipID].channels[ch].mask = false;
                    if (ch < 24) Audio.resetC140Mask(chipID, ch);
                    break;
                case enmUseChip.SEGAPCM:
                    newParam.segaPcm[chipID].channels[ch].mask = false;
                    if (ch < 16) Audio.resetSegaPCMMask(chipID, ch);
                    break;
                case enmUseChip.AY8910:
                    newParam.ay8910[chipID].channels[ch].mask = false;
                    Audio.resetAY8910Mask(chipID, ch);
                    break;
                case enmUseChip.HuC6280:
                    newParam.huc6280[chipID].channels[ch].mask = false;
                    Audio.resetHuC6280Mask(chipID, ch);
                    break;

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
                    midiin.MessageReceived -= midiIn_MessageReceived;
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
                            midiin.MessageReceived += midiIn_MessageReceived;
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
                    midiin.MessageReceived -= midiIn_MessageReceived;
                    midiin.ErrorReceived -= midiIn_ErrorReceived;
                    midiin = null;
                }
                catch
                {
                    midiin = null;
                }
            }
        }

        void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (!setting.midiKbd.UseMIDIKeyboard) return;

            YM2612MIDI.midiIn_MessageReceived(e);
        }

        public void ym2612Midi_ClearNoteLog()
        {
            YM2612MIDI.ClearNoteLog();
        }

        public void ym2612Midi_ClearNoteLog(int ch)
        {
            YM2612MIDI.ClearNoteLog(ch);
        }

        public void ym2612Midi_Log2MML(int ch)
        {
            YM2612MIDI.Log2MML(ch);
        }

        public void ym2612Midi_Log2MML66(int ch)
        {
            YM2612MIDI.Log2MML66(ch);
        }

        public void ym2612Midi_AllNoteOff()
        {
            YM2612MIDI.AllNoteOff();
        }

        public void ym2612Midi_SetMode(int m)
        {
            YM2612MIDI.SetMode(m);
        }

        public void ym2612Midi_SelectChannel(int ch)
        {
            YM2612MIDI.SelectChannel(ch);
        }

        public void ym2612Midi_SetTonesToSetting()
        {
            YM2612MIDI.SetTonesToSettng();
        }

        public void ym2612Midi_SetTonesFromSetting()
        {
            YM2612MIDI.SetTonesFromSettng();
        }

        public void ym2612Midi_SaveTonePallet(string fn, int tp)
        {
            YM2612MIDI.SaveTonePallet(fn, tp, tonePallet);
        }

        public void ym2612Midi_LoadTonePallet(string fn, int tp)
        {
            YM2612MIDI.LoadTonePallet(fn, tp, tonePallet);
        }

        public void ym2612Midi_CopyToneToClipboard()
        {
            if (setting.midiKbd.IsMONO)
            {
                YM2612MIDI.CopyToneToClipboard(new int[] { setting.midiKbd.UseMONOChannel });
            }
            else
            {
                List<int> uc = new List<int>();
                for(int i=0;i<setting.midiKbd.UseChannel.Length;i++)
                {
                    if (setting.midiKbd.UseChannel[i]) uc.Add(i);
                }
                YM2612MIDI.CopyToneToClipboard(uc.ToArray());
            }
        }

        public void ym2612Midi_PasteToneFromClipboard()
        {
            if (setting.midiKbd.IsMONO)
            {
                YM2612MIDI.PasteToneFromClipboard(new int[] { setting.midiKbd.UseMONOChannel });
            }
            else
            {
                List<int> uc = new List<int>();
                for (int i = 0; i < setting.midiKbd.UseChannel.Length; i++)
                {
                    if (setting.midiKbd.UseChannel[i]) uc.Add(i);
                }
                YM2612MIDI.PasteToneFromClipboard(uc.ToArray());
            }
        }

        public void ym2612Midi_CopyToneToClipboard(int ch)
        {
            YM2612MIDI.CopyToneToClipboard(new int[] { ch });
        }

        public void ym2612Midi_PasteToneFromClipboard(int ch)
        {
            YM2612MIDI.PasteToneFromClipboard(new int[] { ch });
        }

        public void ym2612Midi_SetSelectInstParam(int ch, int n)
        {
            YM2612MIDI.newParam.ym2612Midi.selectCh = ch;
            YM2612MIDI.newParam.ym2612Midi.selectParam = n;
        }

        public void ym2612Midi_AddSelectInstParam(int n)
        {
            int p = YM2612MIDI.newParam.ym2612Midi.selectParam;
            p += n;
            if (p > 47) p = 0;
            YM2612MIDI.newParam.ym2612Midi.selectParam = p;
        }

        public void ym2612Midi_ChangeSelectedParamValue(int n)
        {
            YM2612MIDI.ChangeSelectedParamValue(n);
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

            frmMixer2 = new frmMixer2(this, setting.other.Zoom);
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
            //ディスプレイの高さと幅を取得
            int h = s.Bounds.Height;
            int w = s.Bounds.Width;
            if (frmMixer2.x > w - 100 || frmMixer2.y > h - 100)
            {
                frmMixer2.x = 0;
                frmMixer2.y = 0;
            }

            //frmMixer.setting = setting;
            screen.AddMixer(frmMixer2.pbScreen, Properties.Resources.planeMixer);
            frmMixer2.Show();
            frmMixer2.update();
            screen.screenInitMixer();
            oldParam.mixer = new MDChipParams.Mixer();
        }

    }
}

