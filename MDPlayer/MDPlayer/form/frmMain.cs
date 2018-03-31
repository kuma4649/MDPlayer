using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.Midi;
using System.Collections.Generic;
using System.Text;

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
        private frmYMF278B[] frmYMF278B = new frmYMF278B[2] { null, null };
        private frmMIDI[] frmMIDI = new frmMIDI[2] { null, null };
        private frmYM2612MIDI frmYM2612MIDI = null;
        private frmMixer2 frmMixer2 = null;
        private frmNESDMC[] frmNESDMC = new frmNESDMC[2] { null, null };
        private frmFDS[] frmFDS = new frmFDS[2] { null, null };
        private frmMMC5[] frmMMC5 = new frmMMC5[2] { null, null };

        public MDChipParams oldParam = new MDChipParams();
        private MDChipParams newParam = new MDChipParams();

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



        public frmMain()
        {
            log.ForcedWrite("起動処理開始");
            log.ForcedWrite("frmMain(コンストラクタ):STEP 00");

            InitializeComponent();
            DrawBuff.Init();

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
            allScreenInit();

            log.ForcedWrite("frmMain_Load:STEP 07");

            pWidth = pbScreen.Width;
            pHeight = pbScreen.Height;

            frmPlayList = new frmPlayList(this);
            frmPlayList.Show();
            frmPlayList.Visible = false;
            frmPlayList.Opacity = 1.0;
            frmPlayList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmPlayList.Refresh();

            frmVSTeffectList = new frmVSTeffectList(this, setting);
            frmVSTeffectList.Show();
            frmVSTeffectList.Visible = false;
            frmVSTeffectList.Opacity = 1.0;
            //frmVSTeffectList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmVSTeffectList.Refresh();

            if (setting.location.OPlayList) dispPlayList();
            if (setting.location.OInfo) openInfo();

            if (setting.location.OpenAY8910[0]) tsmiPAY8910_Click(null, null);
            if (setting.location.OpenC140[0]) tsmiPC140_Click(null, null);
            if (setting.location.OpenHuC6280[0]) tsmiPHuC6280_Click(null, null);
            if (setting.location.OpenMIDI[0]) tsmiPMIDI_Click(null, null);
            if (setting.location.OpenNESDMC[0]) tsmiPNESDMC_Click(null, null);
            if (setting.location.OpenFDS[0]) tsmiPFDS_Click(null, null);
            if (setting.location.OpenMMC5[0]) tsmiPMMC5_Click(null, null);
            if (setting.location.OpenOKIM6258[0]) tsmiPOKIM6258_Click(null, null);
            if (setting.location.OpenOKIM6295[0]) tsmiPOKIM6258_Click(null, null);
            if (setting.location.OpenRf5c164[0]) tsmiPRF5C164_Click(null, null);
            if (setting.location.OpenSegaPCM[0]) tsmiPSegaPCM_Click(null, null);
            if (setting.location.OpenSN76489[0]) tsmiPDCSG_Click(null, null);
            if (setting.location.OpenYm2151[0]) tsmiPOPM_Click(null, null);
            if (setting.location.OpenYm2203[0]) tsmiPOPN_Click(null, null);
            if (setting.location.OpenYm2413[0]) tsmiPOPLL_Click(null, null);
            if (setting.location.OpenYm2608[0]) tsmiPOPNA_Click(null, null);
            if (setting.location.OpenYm2610[0]) tsmiPOPNB_Click(null, null);
            if (setting.location.OpenYm2612[0]) tsmiPOPN2_Click(null, null);

            if (setting.location.OpenAY8910[1]) tsmiSAY8910_Click(null, null);
            if (setting.location.OpenC140[1]) tsmiSC140_Click(null, null);
            if (setting.location.OpenHuC6280[1]) tsmiSHuC6280_Click(null, null);
            if (setting.location.OpenMIDI[1]) tsmiSMIDI_Click(null, null);
            if (setting.location.OpenNESDMC[1]) tsmiSNESDMC_Click(null, null);
            if (setting.location.OpenFDS[1]) tsmiSFDS_Click(null, null);
            if (setting.location.OpenMMC5[1]) tsmiSMMC5_Click(null, null);
            if (setting.location.OpenOKIM6258[1]) tsmiSOKIM6258_Click(null, null);
            if (setting.location.OpenOKIM6295[1]) tsmiSOKIM6258_Click(null, null);
            if (setting.location.OpenRf5c164[1]) tsmiSRF5C164_Click(null, null);
            if (setting.location.OpenSN76489[1]) tsmiSDCSG_Click(null, null);
            if (setting.location.OpenSegaPCM[1]) tsmiSSegaPCM_Click(null, null);
            if (setting.location.OpenYm2151[1]) tsmiSOPM_Click(null, null);
            if (setting.location.OpenYm2203[1]) tsmiSOPN_Click(null, null);
            if (setting.location.OpenYm2413[1]) tsmiSOPLL_Click(null, null);
            if (setting.location.OpenYm2608[1]) tsmiSOPNA_Click(null, null);
            if (setting.location.OpenYm2610[1]) tsmiSOPNB_Click(null, null);
            if (setting.location.OpenYm2612[1]) tsmiSOPN2_Click(null, null);

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

            if (frmYMF278B[0] != null && !frmYMF278B[0].isClosed)
            {
                tsmiPOPL4_Click(null, null);
                tsmiPOPL4_Click(null, null);
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

            if (frmYMF278B[1] != null && !frmYMF278B[1].isClosed)
            {
                tsmiSOPL4_Click(null, null);
                tsmiSOPL4_Click(null, null);
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
                    pl.AddFile(args[1]);
                    //frmPlayList.AddList(args[1]);
                }

                if (!loadAndPlay(0, 0,args[1], ""))
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
            allScreenInit();
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
                setting.location.OpenMIDI[chipID] = false;
                setting.location.OpenNESDMC[chipID] = false;
                setting.location.OpenFDS[chipID] = false;
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
                if (frmNESDMC[chipID] != null && !frmNESDMC[chipID].isClosed)
                {
                    setting.location.PosNESDMC[chipID] = frmNESDMC[chipID].Location;
                    setting.location.OpenNESDMC[chipID] = true;
                }
                if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed)
                {
                    setting.location.PosYm2612MIDI = frmYM2612MIDI.Location;
                    setting.location.OpenYm2612MIDI = true;
                }
                if (frmFDS[chipID] != null && !frmFDS[chipID].isClosed)
                {
                    setting.location.PosFDS[chipID] = frmFDS[chipID].Location;
                    setting.location.OpenFDS[chipID] = true;
                }
                if (frmMMC5[chipID] != null && !frmMMC5[chipID].isClosed)
                {
                    setting.location.PosMMC5[chipID] = frmMMC5[chipID].Location;
                    setting.location.OpenMMC5[chipID] = true;
                }
            }

            log.ForcedWrite("frmMain_FormClosing:STEP 05");

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
                oldParam = new MDChipParams();
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
                oldParam = new MDChipParams();
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
                oldParam = new MDChipParams();
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

                        //frmPlayList.AddList(fn[0]);
                        frmPlayList.getPlayList().AddFile(fn[0]);

                        if (common.CheckExt(fn[0]) != enmFileFormat.M3U && common.CheckExt(fn[0]) != enmFileFormat.ZIP)
                        {
                            loadAndPlay(0, 0, fn[0], "");
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

        private void tsmiPOPL4_Click(object sender, EventArgs e)
        {
            OpenFormYMF278B(0);
        }

        private void tsmiPHuC6280_Click(object sender, EventArgs e)
        {
            OpenFormHuC6280(0);
        }

        private void tsmiPMMC5_Click(object sender, EventArgs e)
        {
            OpenFormMMC5(0);
        }

        private void tsmiSMMC5_Click(object sender, EventArgs e)
        {
            OpenFormMMC5(1);
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

        private void tsmiSOPL4_Click(object sender, EventArgs e)
        {
            OpenFormYMF278B(1);
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

        private void tsmiPNESDMC_Click(object sender, EventArgs e)
        {
            OpenFormNESDMC(0);
        }

        private void tsmiSNESDMC_Click(object sender, EventArgs e)
        {
            OpenFormNESDMC(1);
        }

        private void tsmiPFDS_Click(object sender, EventArgs e)
        {
            OpenFormFDS(0);
        }

        private void tsmiSFDS_Click(object sender, EventArgs e)
        {
            OpenFormFDS(1);
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

            frmMCD[chipID] = new frmMegaCD(this, chipID, setting.other.Zoom,newParam.rf5c164[chipID]);
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

            frmYM2608[chipID] = new frmYM2608(this, chipID, setting.other.Zoom,newParam.ym2608[chipID]);

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

            frmYM2151[chipID] = new frmYM2151(this, chipID, setting.other.Zoom, newParam.ym2151[chipID]);

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

            frmC140[chipID] = new frmC140(this, chipID, setting.other.Zoom, newParam.c140[chipID]);

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

            frmYM2203[chipID] = new frmYM2203(this, chipID, setting.other.Zoom, newParam.ym2203[chipID]);

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

            frmYM2610[chipID] = new frmYM2610(this, chipID, setting.other.Zoom,newParam.ym2610[chipID]);

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

            frmYM2612[chipID] = new frmYM2612(this, chipID, setting.other.Zoom, newParam.ym2612[chipID]);

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
            oldParam.ym2612[chipID] = new MDChipParams.YM2612();
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

            frmOKIM6295[chipID] = new frmOKIM6295(this, chipID, setting.other.Zoom, newParam.okim6295[chipID]);

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

            frmSN76489[chipID] = new frmSN76489(this, chipID, setting.other.Zoom,newParam.sn76489[chipID]);

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

            frmSegaPCM[chipID] = new frmSegaPCM(this, chipID, setting.other.Zoom,newParam.segaPcm[chipID]);

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

            frmAY8910[chipID] = new frmAY8910(this, chipID, setting.other.Zoom, newParam.ay8910[chipID]);

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

            frmHuC6280[chipID] = new frmHuC6280(this, chipID, setting.other.Zoom,newParam.huc6280[chipID]);

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

            frmYM2413[chipID] = new frmYM2413(this, chipID, setting.other.Zoom, newParam.ym2413[chipID]);

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
            frmYM2413[chipID].Text = string.Format("YM2413 ({0})", chipID == 0 ? "Primary" : "Secondary");
            oldParam.ym2413[chipID] = new MDChipParams.YM2413();
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
            //ディスプレイの高さと幅を取得
            int h = s.Bounds.Height;
            int w = s.Bounds.Width;
            if (frmYM2612MIDI.x > w - 100 || frmYM2612MIDI.y > h - 100)
            {
                frmYM2612MIDI.x = 0;
                frmYM2612MIDI.y = 0;
            }

            //frmYM2612MIDI.setting = setting;
            frmYM2612MIDI.Show();
            frmYM2612MIDI.update();
            oldParam.ym2612Midi = new MDChipParams.YM2612MIDI();
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
                allScreenInit();
                //screen.screenInitAll();

                log.ForcedWrite("設定が変更されたため、再度Audio初期化処理開始");

                Audio.Init(setting);

                log.ForcedWrite("Audio初期化処理完了");
                log.debug = setting.Debug_DispFrameCounter;

                frmVSTeffectList.dispPluginList();
                StartMIDIInMonitoring();

                IsInitialOpenFolder = true;

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
            //ディスプレイの高さと幅を取得
            int h = s.Bounds.Height;
            int w = s.Bounds.Width;
            if (frmMixer2.x > w - 100 || frmMixer2.y > h - 100)
            {
                frmMixer2.x = 0;
                frmMixer2.y = 0;
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

                    frmPlayList.Stop();

                    frmPlayList.getPlayList().AddFile(filename);
                    //frmPlayList.AddList(filename);

                    if (filename.ToLower().LastIndexOf(".zip") == -1)
                    {
                        loadAndPlay(0,0,filename);
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
            oldParam = new MDChipParams();
            DrawBuff.drawTimer(screen.mainScreen, 0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            for (int i = 0; i < 2; i++)
            {
                if (frmYM2612[i] != null) frmYM2612[i].screenInit();

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

                    if (frmYMF278B[chipID] != null && !frmYMF278B[chipID].isClosed) frmYMF278B[chipID].screenChangeParams();
                    else frmYMF278B[chipID] = null;

                    if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed) frmOKIM6258[chipID].screenChangeParams();
                    else frmOKIM6258[chipID] = null;

                    if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed) frmOKIM6295[chipID].screenChangeParams();
                    else frmOKIM6295[chipID] = null;

                    if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed) frmSN76489[chipID].screenChangeParams();
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

                    if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed) frmMIDI[chipID].screenChangeParams();
                    else frmMIDI[chipID] = null;

                    if (frmNESDMC[chipID] != null && !frmNESDMC[chipID].isClosed) frmNESDMC[chipID].screenChangeParams();
                    else frmNESDMC[chipID] = null;

                    if (frmFDS[chipID] != null && !frmFDS[chipID].isClosed) frmFDS[chipID].screenChangeParams();
                    else frmFDS[chipID] = null;

                    if (frmMMC5[chipID] != null && !frmMMC5[chipID].isClosed) frmMMC5[chipID].screenChangeParams();
                    else frmMMC5[chipID] = null;

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
                    if (frmMCD[chipID] != null && !frmMCD[chipID].isClosed) { frmMCD[chipID].screenDrawParams(); frmMCD[chipID].update(); }
                    else frmMCD[chipID] = null;

                    if (frmC140[chipID] != null && !frmC140[chipID].isClosed) { frmC140[chipID].screenDrawParams(); frmC140[chipID].update(); }
                    else frmC140[chipID] = null;

                    if (frmYM2608[chipID] != null && !frmYM2608[chipID].isClosed) { frmYM2608[chipID].screenDrawParams(); frmYM2608[chipID].update(); }
                    else frmYM2608[chipID] = null;

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

                    if (frmYMF278B[chipID] != null && !frmYMF278B[chipID].isClosed) { frmYMF278B[chipID].screenDrawParams(); frmYMF278B[chipID].update(); }
                    else frmYMF278B[chipID] = null;

                    if (frmOKIM6258[chipID] != null && !frmOKIM6258[chipID].isClosed) { frmOKIM6258[chipID].screenDrawParams(); frmOKIM6258[chipID].update(); }
                    else frmOKIM6258[chipID] = null;

                    if (frmOKIM6295[chipID] != null && !frmOKIM6295[chipID].isClosed) { frmOKIM6295[chipID].screenDrawParams(); frmOKIM6295[chipID].update(); }
                    else frmOKIM6295[chipID] = null;

                    if (frmSN76489[chipID] != null && !frmSN76489[chipID].isClosed) { frmSN76489[chipID].screenDrawParams(); frmSN76489[chipID].update(); }
                    else frmSN76489[chipID] = null;

                    if (frmSegaPCM[chipID] != null && !frmSegaPCM[chipID].isClosed) { frmSegaPCM[chipID].screenDrawParams(); frmSegaPCM[chipID].update(); }
                    else frmSegaPCM[chipID] = null;

                    if (frmAY8910[chipID] != null && !frmAY8910[chipID].isClosed) { frmAY8910[chipID].screenDrawParams(); frmAY8910[chipID].update(); }
                    else frmAY8910[chipID] = null;

                    if (frmHuC6280[chipID] != null && !frmHuC6280[chipID].isClosed) { frmHuC6280[chipID].screenDrawParams(); frmHuC6280[chipID].update(); }
                    else frmHuC6280[chipID] = null;

                    if (frmMIDI[chipID] != null && !frmMIDI[chipID].isClosed) { frmMIDI[chipID].screenDrawParams(); frmMIDI[chipID].update(); }
                    else frmMIDI[chipID] = null;

                    if (frmNESDMC[chipID] != null && !frmNESDMC[chipID].isClosed) { frmNESDMC[chipID].screenDrawParams(); frmNESDMC[chipID].update(); }
                    else frmNESDMC[chipID] = null;

                    if (frmFDS[chipID] != null && !frmFDS[chipID].isClosed) { frmFDS[chipID].screenDrawParams(); frmFDS[chipID].update(); }
                    else frmFDS[chipID] = null;

                    if (frmMMC5[chipID] != null && !frmMMC5[chipID].isClosed) { frmMMC5[chipID].screenDrawParams(); frmMMC5[chipID].update(); }
                    else frmMMC5[chipID] = null;

                }
                if (frmYM2612MIDI != null && !frmYM2612MIDI.isClosed) { frmYM2612MIDI.screenDrawParams(); frmYM2612MIDI.update(); }
                else frmYM2612MIDI = null;
                if (frmMixer2 != null && !frmMixer2.isClosed) { frmMixer2.screenDrawParams(); frmMixer2.update(); }
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

            long w = Audio.GetCounter();
            double sec = (double)w / (double)common.SampleRate;
            newParam.Cminutes = (int)(sec / 60);
            sec -= newParam.Cminutes * 60;
            newParam.Csecond = (int)sec;
            sec -= newParam.Csecond;
            newParam.Cmillisecond = (int)(sec * 100.0);

            w = Audio.GetTotalCounter();
            sec = (double)w / (double)common.SampleRate;
            newParam.TCminutes = (int)(sec / 60);
            sec -= newParam.TCminutes * 60;
            newParam.TCsecond = (int)sec;
            sec -= newParam.TCsecond;
            newParam.TCmillisecond = (int)(sec * 100.0);

            w = Audio.GetLoopCounter();
            sec = (double)w / (double)common.SampleRate;
            newParam.LCminutes = (int)(sec / 60);
            sec -= newParam.LCminutes * 60;
            newParam.LCsecond = (int)sec;
            sec -= newParam.LCsecond;
            newParam.LCmillisecond = (int)(sec * 100.0);

        }

        private void screenDrawParams()
        {

            // 描画

            DrawBuff.drawButtons(screen.mainScreen, oldButton, newButton, oldButtonMode, newButtonMode);

            DrawBuff.drawTimer(screen.mainScreen, 0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            DrawBuff.drawTimer(screen.mainScreen, 2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            byte[] chips = Audio.GetChipStatus();
            DrawBuff.drawChipName(screen.mainScreen,14 * 4, 0 * 8, 0, ref oldParam.chipLED.PriOPN, chips[0]);
            DrawBuff.drawChipName(screen.mainScreen,18 * 4, 0 * 8, 1, ref oldParam.chipLED.PriOPN2, chips[1]);
            DrawBuff.drawChipName(screen.mainScreen,23 * 4, 0 * 8, 2, ref oldParam.chipLED.PriOPNA, chips[2]);
            DrawBuff.drawChipName(screen.mainScreen,28 * 4, 0 * 8, 3, ref oldParam.chipLED.PriOPNB, chips[3]);
            DrawBuff.drawChipName(screen.mainScreen,33 * 4, 0 * 8, 4, ref oldParam.chipLED.PriOPM, chips[4]);
            DrawBuff.drawChipName(screen.mainScreen,37 * 4, 0 * 8, 5, ref oldParam.chipLED.PriDCSG, chips[5]);
            DrawBuff.drawChipName(screen.mainScreen,42 * 4, 0 * 8, 6, ref oldParam.chipLED.PriRF5C, chips[6]);
            DrawBuff.drawChipName(screen.mainScreen,47 * 4, 0 * 8, 7, ref oldParam.chipLED.PriPWM, chips[7]);
            DrawBuff.drawChipName(screen.mainScreen,51 * 4, 0 * 8, 8, ref oldParam.chipLED.PriOKI5, chips[8]);
            DrawBuff.drawChipName(screen.mainScreen,56 * 4, 0 * 8, 9, ref oldParam.chipLED.PriOKI9, chips[9]);
            DrawBuff.drawChipName(screen.mainScreen,61 * 4, 0 * 8, 10, ref oldParam.chipLED.PriC140, chips[10]);
            DrawBuff.drawChipName(screen.mainScreen,66 * 4, 0 * 8, 11, ref oldParam.chipLED.PriSPCM, chips[11]);
            DrawBuff.drawChipName(screen.mainScreen,4 * 4, 0 * 8, 12, ref oldParam.chipLED.PriAY10, chips[12]);
            DrawBuff.drawChipName(screen.mainScreen,9 * 4, 0 * 8, 13, ref oldParam.chipLED.PriOPLL, chips[13]);
            DrawBuff.drawChipName(screen.mainScreen, 71 * 4, 0 * 8, 14, ref oldParam.chipLED.PriHuC8, chips[14]);

            DrawBuff.drawChipName(screen.mainScreen,14 * 4, 1 * 8, 0, ref oldParam.chipLED.SecOPN, chips[128 + 0]);
            DrawBuff.drawChipName(screen.mainScreen,18 * 4, 1 * 8, 1, ref oldParam.chipLED.SecOPN2, chips[128 + 1]);
            DrawBuff.drawChipName(screen.mainScreen,23 * 4, 1 * 8, 2, ref oldParam.chipLED.SecOPNA, chips[128 + 2]);
            DrawBuff.drawChipName(screen.mainScreen,28 * 4, 1 * 8, 3, ref oldParam.chipLED.SecOPNB, chips[128 + 3]);
            DrawBuff.drawChipName(screen.mainScreen,33 * 4, 1 * 8, 4, ref oldParam.chipLED.SecOPM, chips[128 + 4]);
            DrawBuff.drawChipName(screen.mainScreen,37 * 4, 1 * 8, 5, ref oldParam.chipLED.SecDCSG, chips[128 + 5]);
            DrawBuff.drawChipName(screen.mainScreen,42 * 4, 1 * 8, 6, ref oldParam.chipLED.SecRF5C, chips[128 + 6]);
            DrawBuff.drawChipName(screen.mainScreen,47 * 4, 1 * 8, 7, ref oldParam.chipLED.SecPWM, chips[128 + 7]);
            DrawBuff.drawChipName(screen.mainScreen,51 * 4, 1 * 8, 8, ref oldParam.chipLED.SecOKI5, chips[128 + 8]);
            DrawBuff.drawChipName(screen.mainScreen,56 * 4, 1 * 8, 9, ref oldParam.chipLED.SecOKI9, chips[128 + 9]);
            DrawBuff.drawChipName(screen.mainScreen,61 * 4, 1 * 8, 10, ref oldParam.chipLED.SecC140, chips[128 + 10]);
            DrawBuff.drawChipName(screen.mainScreen,66 * 4, 1 * 8, 11, ref oldParam.chipLED.SecSPCM, chips[128 + 11]);
            DrawBuff.drawChipName(screen.mainScreen,4 * 4, 1 * 8, 12, ref oldParam.chipLED.SecAY10, chips[128 + 12]);
            DrawBuff.drawChipName(screen.mainScreen,9 * 4, 1 * 8, 13, ref oldParam.chipLED.SecOPLL, chips[128 + 13]);
            DrawBuff.drawChipName(screen.mainScreen, 71 * 4, 0 * 8, 14, ref oldParam.chipLED.SecHuC8, chips[128 + 14]);

            DrawBuff.drawFont4(screen.mainScreen, 0, 24, 1, Audio.GetIsDataBlock(enmModel.VirtualModel) ? "VD" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 12, 24, 1, Audio.GetIsPcmRAMWrite(enmModel.VirtualModel) ? "VP" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 0, 32, 1, Audio.GetIsDataBlock(enmModel.RealModel) ? "RD" : "  ");
            DrawBuff.drawFont4(screen.mainScreen, 12, 32, 1, Audio.GetIsPcmRAMWrite(enmModel.RealModel) ? "RP" : "  ");

            if (setting.Debug_DispFrameCounter)
            {
                long v = Audio.getVirtualFrameCounter();
                if (v != -1) DrawBuff.drawFont8(screen.mainScreen, 0, 0, 0, string.Format("EMU        : {0:D12} ", v));
                long r = Audio.getRealFrameCounter();
                if (r != -1) DrawBuff.drawFont8(screen.mainScreen, 0, 8, 0, string.Format("SCCI       : {0:D12} ", r));
                long d = r - v;
                if (r != -1 && v != -1) DrawBuff.drawFont8(screen.mainScreen, 0, 16, 0, string.Format("SCCI - EMU : {0:D12} ", d));
                DrawBuff.drawFont8(screen.mainScreen, 0, 24, 0, string.Format("PROC TIME  : {0:D12} ", Audio.ProcTimePer1Frame));
            }

            screen.Refresh();

            Audio.updateVol();


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
            Tuple<int, int, string, string> playFn = null;

            frmPlayList.Stop();

            //if (srcBuf == null && frmPlayList.getMusicCount() < 1)
            if (frmPlayList.getMusicCount() < 1)
            {
                fn = fileOpen(false);
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

            allScreenInit();

            loadAndPlay(playFn.Item1, playFn.Item2, playFn.Item3,playFn.Item4);
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
                for (int ch = 0; ch < 3; ch++) ResetChannelMask(enmUseChip.AY8910, chipID, ch);
                for (int ch = 0; ch < 8; ch++) ResetChannelMask(enmUseChip.YM2151, chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(enmUseChip.YM2203 , chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask( enmUseChip.YM2413 , chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask( enmUseChip.YM2608 , chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask( enmUseChip.YM2610 , chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(enmUseChip.YM2612 , chipID, ch);
                for (int ch = 0; ch < 4; ch++) ResetChannelMask(enmUseChip.SN76489 , chipID, ch);
                for (int ch = 0; ch < 8; ch++) ResetChannelMask(enmUseChip.RF5C164 , chipID, ch);
                for (int ch = 0; ch < 24; ch++) ResetChannelMask( enmUseChip.C140 , chipID, ch);
                for (int ch = 0; ch < 16; ch++) ResetChannelMask( enmUseChip.SEGAPCM , chipID, ch);
                for (int ch = 0; ch < 6; ch++) ResetChannelMask(enmUseChip.HuC6280, chipID, ch);
                for (int ch = 0; ch < 2; ch++) ResetChannelMask(enmUseChip.NES, chipID, ch);
                for (int ch = 0; ch < 3; ch++) ResetChannelMask(enmUseChip.DMC, chipID, ch);
                for (int ch = 0; ch < 3; ch++) ResetChannelMask(enmUseChip.MMC5, chipID, ch);
                ResetChannelMask(enmUseChip.FDS, chipID, 0);
            }

            oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            allScreenInit();

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

                if (Audio.chipLED.PriOPM != 0) OpenFormYM2151(0, true); else CloseFormYM2151(0);
                if (Audio.chipLED.SecOPM != 0) OpenFormYM2151(1, true); else CloseFormYM2151(1);

                if (Audio.chipLED.PriOPN != 0) OpenFormYM2203(0, true); else CloseFormYM2203(0);
                if (Audio.chipLED.SecOPN != 0) OpenFormYM2203(1, true); else CloseFormYM2203(1);

                if (Audio.chipLED.PriOPLL != 0) OpenFormYM2413(0, true); else CloseFormYM2413(0);
                if (Audio.chipLED.SecOPLL != 0) OpenFormYM2413(1, true); else CloseFormYM2413(1);

                if (Audio.chipLED.PriOPNA != 0) OpenFormYM2608(0, true); else CloseFormYM2608(0);
                if (Audio.chipLED.SecOPNA != 0) OpenFormYM2608(1, true); else CloseFormYM2608(1);

                if (Audio.chipLED.PriOPNB != 0) OpenFormYM2610(0, true); else CloseFormYM2610(0);
                if (Audio.chipLED.SecOPNB != 0) OpenFormYM2610(1, true); else CloseFormYM2610(1);

                if (Audio.chipLED.PriOPN2 != 0) OpenFormYM2612(0, true); else CloseFormYM2612(0);
                if (Audio.chipLED.SecOPN2 != 0) OpenFormYM2612(1, true); else CloseFormYM2612(1);

                if (Audio.chipLED.PriDCSG != 0) OpenFormSN76489(0, true); else CloseFormSN76489(0);
                if (Audio.chipLED.SecDCSG != 0) OpenFormSN76489(1, true); else CloseFormSN76489(1);

                if (Audio.chipLED.PriRF5C != 0) OpenFormMegaCD(0, true); else CloseFormMegaCD(0);
                if (Audio.chipLED.SecRF5C != 0) OpenFormMegaCD(1, true); else CloseFormMegaCD(1);

                if (Audio.chipLED.PriOKI5 != 0) OpenFormOKIM6258(0, true); else CloseFormOKIM6258(0);
                if (Audio.chipLED.SecOKI5 != 0) OpenFormOKIM6258(1, true); else CloseFormOKIM6258(1);

                if (Audio.chipLED.PriOKI9 != 0) OpenFormOKIM6295(0, true); else CloseFormOKIM6295(0);
                if (Audio.chipLED.SecOKI9 != 0) OpenFormOKIM6295(1, true); else CloseFormOKIM6295(1);

                if (Audio.chipLED.PriC140 != 0) OpenFormC140(0, true); else CloseFormC140(0);
                if (Audio.chipLED.SecC140 != 0) OpenFormC140(1, true); else CloseFormC140(1);

                if (Audio.chipLED.PriSPCM != 0) OpenFormSegaPCM(0, true); else CloseFormSegaPCM(0);
                if (Audio.chipLED.SecSPCM != 0) OpenFormSegaPCM(1, true); else CloseFormSegaPCM(1);

                if (Audio.chipLED.PriAY10 != 0) OpenFormAY8910(0, true); else CloseFormAY8910(0);
                if (Audio.chipLED.SecAY10 != 0) OpenFormAY8910(1, true); else CloseFormAY8910(1);

                if (Audio.chipLED.PriHuC != 0) OpenFormHuC6280(0, true); else CloseFormHuC6280(0);
                if (Audio.chipLED.SecHuC != 0) OpenFormHuC6280(1, true); else CloseFormHuC6280(1);

                if (Audio.chipLED.PriMID != 0) OpenFormMIDI(0, true); else CloseFormMIDI(0);
                if (Audio.chipLED.SecMID != 0) OpenFormMIDI(1, true); else CloseFormMIDI(1);

                if (Audio.chipLED.PriNES != 0 || Audio.chipLED.PriDMC != 0) OpenFormNESDMC(0, true); else CloseFormNESDMC(0);
                if (Audio.chipLED.SecNES != 0 || Audio.chipLED.SecDMC != 0) OpenFormNESDMC(1, true); else CloseFormNESDMC(1);

                if (Audio.chipLED.PriFDS != 0 ) OpenFormFDS(0, true); else CloseFormFDS(0);
                if (Audio.chipLED.SecFDS != 0 ) OpenFormFDS(1, true); else CloseFormFDS(1);

                if (Audio.chipLED.PriMMC5 != 0) OpenFormMMC5(0, true); else CloseFormMMC5(0);
                if (Audio.chipLED.SecMMC5 != 0) OpenFormMMC5(1, true); else CloseFormMMC5(1);

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
            ofd.Filter = "VGMファイル(*.vgm;*.vgz)|*.vgm;*.vgz|"
                + "NRDファイル(*.nrd)|*.nrd|"
                + "XGMファイル(*.xgm)|*.xgm|"
                + "S98ファイル(*.s98)|*.s98|"
                + "NSFファイル(*.nsf)|*.nsf|"
                + "HESファイル(*.hes)|*.hes|"
                + "SIDファイル(*.sid)|*.sid|"
                + "MDRファイル(*.mdr)|*.mdr|"
                + "StandardMIDIファイル(*.mid)|*.mid|"
                + "RCPファイル(*.rcp)|*.rcp|"
                + "M3Uファイル(*.m3u)|*.m3u|"
                + "アーカイブファイル(*.zip;*.lzh)|*.zip;*.lzh|"
                + "すべてのサポートファイル(*.vgm;*.vgz;*.zip;*.lzh;*.nrd;*.xgm;*.s98;*.nsf;*.hes;*.sid;*.mdr;*.mid;*.rcp;*.m3u)|"
                + "*.vgm;*.vgz;*.zip;*.lzh;*.nrd;*.xgm;*.s98;*.nsf;*.hes;*.sid;*.mdr;*.mid;*.rcp;*.m3u|"
                + "すべてのファイル(*.*)|*.*";
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
                format = enmFileFormat.NRT;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".mdr") != -1)
            {
                format = enmFileFormat.MDR;
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

            if (filename.ToLower().LastIndexOf(".nsf") != -1)
            {
                format = enmFileFormat.NSF;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".hes") != -1)
            {
                format = enmFileFormat.HES;
                return buf;
            }

            if (filename.ToLower().LastIndexOf(".sid") != -1)
            {
                format = enmFileFormat.SID;
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
            if (chip == enmUseChip.YM2413)
            {
                getInstChForMGSC(chip, ch, chipID);
                return;
            }

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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
        }

        private void getInstChForMGSC(enmUseChip chip, int ch, int chipID)
        {

            string n = "";

            if (chip == enmUseChip.YM2413)
            {
                int[] Register = Audio.GetYM2413Register(chipID);
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

            }

            if (!string.IsNullOrEmpty(n)) Clipboard.SetText(n);
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
                        frmPlayList.getPlayList().AddFile(sParam);
                        //frmPlayList.AddList(sParam);
                    }

                    if (!loadAndPlay(0,0, sParam))
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



        public bool loadAndPlay(int m, int songNo, string fn, string zfn = null)
        {
            try
            {
                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                string playingFileName = fn;
                enmFileFormat format = enmFileFormat.unknown;
                List<Tuple<string, byte[]>> extFile = null;

                if (zfn == null || zfn == "")
                {
                    srcBuf = getAllBytes(fn, out format);
                    extFile = getExtendFile(fn, srcBuf, format);
                }
                else
                {
                    if (Path.GetExtension(zfn).ToUpper() == ".ZIP")
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(zfn))
                        {
                            ZipArchiveEntry entry = archive.GetEntry(fn);
                            string arcFn = "";

                            format = common.CheckExt(fn);
                            if (format != enmFileFormat.unknown)
                            {
                                srcBuf = getBytesFromZipFile(entry, out arcFn);
                                if (arcFn != "") playingFileName = arcFn;
                                extFile = getExtendFile(fn, srcBuf, format, archive);
                            }
                        }
                    }
                    else
                    {
                        format = common.CheckExt(fn);
                        if (format != enmFileFormat.unknown)
                        {
                            UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                            srcBuf = cmd.GetFileByte(zfn, fn);
                            playingFileName = fn;
                            extFile = getExtendFile(fn, srcBuf, format, new Tuple<string, string>(zfn, fn));
                        }
                    }
                }

                Audio.SetVGMBuffer(format, srcBuf, playingFileName, m, songNo, extFile);
                newParam.ym2612[0].fileFormat = format;
                newParam.ym2612[1].fileFormat = format;

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

        private List<Tuple<string, byte[]>> getExtendFile(string fn,byte[] srcBuf,enmFileFormat format,object archive= null)
        {
            List<Tuple<string, byte[]>> ret = new List<Tuple<string, byte[]>>();
            byte[] buf;
            switch (format)
            {
                case enmFileFormat.RCP:
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
                case enmFileFormat.MDR:
                    buf = getExtendFileAllBytes(fn, System.IO.Path.GetFileNameWithoutExtension(fn) + ".PCM", archive);
                    if (buf != null) ret.Add(new Tuple<string, byte[]>(".PCM", buf));
                    break;
                default:
                    return null;
            }

            return ret;
        }

        private byte[] getExtendFileAllBytes(string srcFn,string extFn, object archive)
        {
            try
            {
                if (archive == null)
                {
                    string trgFn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(srcFn), extFn).Trim();
                    if (!System.IO.File.Exists(trgFn)) return null;
                    return System.IO.File.ReadAllBytes(trgFn);
                }
                else
                {
                    string trgFn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(srcFn), extFn);
                    trgFn = trgFn.Replace("\\", "/").Trim();
                    if (archive is ZipArchive) {
                        ZipArchiveEntry entry = ((ZipArchive)archive).GetEntry(trgFn);
                        if (entry == null) return null;
                        string arcFn = "";
                        return getBytesFromZipFile(entry, out arcFn);
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
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
            arcFn = entry.FullName;
            using (BinaryReader reader = new BinaryReader(entry.Open()))
            {
                buf = reader.ReadBytes((int)entry.Length);
            }

            if (common.CheckExt(entry.FullName) == enmFileFormat.VGM)
            {
                try
                {
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
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    buf = null;
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
                    if (ch >= 0 && ch < 14)
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
                case enmUseChip.OKIM6258:
                    if (!newParam.okim6258[chipID].mask)
                    {
                        Audio.setOKIM6258Mask(chipID);
                    }
                    else
                    {
                        Audio.resetOKIM6258Mask(chipID);
                    }
                    newParam.okim6258[chipID].mask = !newParam.okim6258[chipID].mask;
                    break;
                case enmUseChip.NES:
                    if (!newParam.nesdmc[chipID].sqrChannels[ch].mask)
                    {
                        Audio.setNESMask(chipID, ch);
                    }
                    else
                    {
                        Audio.resetNESMask(chipID, ch);
                    }
                    newParam.nesdmc[chipID].sqrChannels[ch].mask = !newParam.nesdmc[chipID].sqrChannels[ch].mask;
                    break;
                case enmUseChip.DMC:
                    switch (ch) {
                        case 0:
                            if (!newParam.nesdmc[chipID].triChannel.mask) Audio.setDMCMask(chipID, ch);
                            else Audio.resetDMCMask(chipID, ch);
                            newParam.nesdmc[chipID].triChannel.mask = !newParam.nesdmc[chipID].triChannel.mask;
                            break;
                        case 1:
                            if (!newParam.nesdmc[chipID].noiseChannel.mask) Audio.setDMCMask(chipID, ch);
                            else Audio.resetDMCMask(chipID, ch);
                            newParam.nesdmc[chipID].noiseChannel.mask = !newParam.nesdmc[chipID].noiseChannel.mask;
                            break;
                        case 2:
                            if (!newParam.nesdmc[chipID].dmcChannel.mask) Audio.setDMCMask(chipID, ch);
                            else Audio.resetDMCMask(chipID, ch);
                            newParam.nesdmc[chipID].dmcChannel.mask = !newParam.nesdmc[chipID].dmcChannel.mask;
                            break;
                    }
                    break;
                case enmUseChip.FDS:
                    if (!newParam.fds[chipID].channel.mask) Audio.setFDSMask(chipID);
                    else Audio.resetFDSMask(chipID);
                    newParam.fds[chipID].channel.mask = !newParam.fds[chipID].channel.mask;
                    break;
                case enmUseChip.MMC5:
                    switch (ch)
                    {
                        case 0:
                            if (!newParam.mmc5[chipID].sqrChannels[0].mask) Audio.setMMC5Mask(chipID, ch);
                            else Audio.resetMMC5Mask(chipID, ch);
                            newParam.mmc5[chipID].sqrChannels[0].mask = !newParam.mmc5[chipID].sqrChannels[0].mask;
                            break;
                        case 1:
                            if (!newParam.mmc5[chipID].sqrChannels[1].mask) Audio.setMMC5Mask(chipID, ch);
                            else Audio.resetMMC5Mask(chipID, ch);
                            newParam.mmc5[chipID].sqrChannels[1].mask = !newParam.mmc5[chipID].sqrChannels[1].mask;
                            break;
                        case 2:
                            if (!newParam.mmc5[chipID].pcmChannel.mask) Audio.setMMC5Mask(chipID, ch);
                            else Audio.resetMMC5Mask(chipID,ch);
                            newParam.mmc5[chipID].pcmChannel.mask = !newParam.mmc5[chipID].pcmChannel.mask;
                            break;
                    }
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
                case enmUseChip.OKIM6258:
                    newParam.okim6258[chipID].mask = false;
                    Audio.resetOKIM6258Mask(chipID);
                    break;
                case enmUseChip.NES:
                    switch(ch) {
                        case 0:
                        case 1:
                            newParam.nesdmc[chipID].sqrChannels[ch].mask = false;
                            Audio.resetNESMask(chipID, ch);
                            break;
                        case 2:
                            newParam.nesdmc[chipID].triChannel.mask = false;
                            Audio.resetDMCMask(chipID, 0);
                            break;
                        case 3:
                            newParam.nesdmc[chipID].noiseChannel.mask = false;
                            Audio.resetDMCMask(chipID, 1);
                            break;
                        case 4:
                            newParam.nesdmc[chipID].dmcChannel.mask = false;
                            Audio.resetDMCMask(chipID, 2);
                            break;
                    }
                    break;
                case enmUseChip.DMC:
                    switch (ch)
                    {
                        case 0:
                            newParam.nesdmc[chipID].triChannel.mask = false;
                            Audio.resetDMCMask(chipID, 0);
                            break;
                        case 1:
                            newParam.nesdmc[chipID].noiseChannel.mask = false;
                            Audio.resetDMCMask(chipID, 1);
                            break;
                        case 2:
                            newParam.nesdmc[chipID].dmcChannel.mask = false;
                            Audio.resetDMCMask(chipID, 2);
                            break;
                    }
                    break;
                case enmUseChip.FDS:
                    newParam.fds[chipID].channel.mask = false;
                    Audio.resetFDSMask(chipID);
                    break;
                case enmUseChip.MMC5:
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
                    Audio.resetMMC5Mask(chipID, ch);
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

    }
}

