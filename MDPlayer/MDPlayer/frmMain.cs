using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.Midi;
using System.Collections.Generic;

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

        private MDChipParams oldParam = new MDChipParams();
        private MDChipParams newParam = new MDChipParams();

        private int[] oldButton = new int[16];
        private int[] newButton = new int[16];
        private int[] oldButtonMode = new int[16];
        private int[] newButtonMode = new int[16];

        private bool isRunning = false;
        private bool stopped = false;

        private bool IsInitialOpenFolder = true;

        private static int SamplingRate = 44100;
        private byte[] srcBuf;

        public Setting setting = Setting.Load();

        private int frameSizeW = 0;
        private int frameSizeH = 0;


        //private MidiIn midiin = null;


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
                    this.Close();
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

            screen = new DoubleBuffer(pbScreen, Properties.Resources.planeControl, Properties.Resources.font, 1);
            screen.setting = setting;
            //oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            screen.screenInitAll();

            log.ForcedWrite("frmMain_Load:STEP 07");

            pWidth = pbScreen.Width;
            pHeight = pbScreen.Height;

            frmPlayList = new frmPlayList();
            frmPlayList.frmMain = this;
            frmPlayList.Show();
            frmPlayList.Visible = false;
            frmPlayList.Opacity = 1.0;
            frmPlayList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmPlayList.Refresh();

            if (setting.location.OPlayList) dispPlayList();
            if (setting.location.OInfo) openInfo();
            if (setting.location.OpenRf5c164[0]) tsmiPRF5C164_Click(null, null);
            if (setting.location.OpenC140[0]) tsmiPC140_Click(null, null);
            if (setting.location.OpenYm2151[0]) tsmiPOPM_Click(null, null);
            if (setting.location.OpenYm2608[0]) tsmiPOPNA_Click(null, null);
            if (setting.location.OpenYm2203[0]) tsmiPOPN_Click(null, null);
            if (setting.location.OpenYm2610[0]) tsmiPOPNB_Click(null, null);
            if (setting.location.OpenYm2612[0]) tsmiPOPN2_Click(null, null);
            if (setting.location.OpenOKIM6258[0]) tsmiPOKIM6258_Click(null, null);
            if (setting.location.OpenOKIM6295[0]) tsmiPOKIM6258_Click(null, null);
            if (setting.location.OpenSN76489[0]) tsmiPDCSG_Click(null, null);
            if (setting.location.OpenSegaPCM[0]) tsmiPSegaPCM_Click(null, null);
            if (setting.location.OpenRf5c164[1]) tsmiSRF5C164_Click(null, null);
            if (setting.location.OpenC140[1]) tsmiSC140_Click(null, null);
            if (setting.location.OpenYm2151[1]) tsmiSOPM_Click(null, null);
            if (setting.location.OpenYm2608[1]) tsmiSOPNA_Click(null, null);
            if (setting.location.OpenYm2203[1]) tsmiSOPN_Click(null, null);
            if (setting.location.OpenYm2610[1]) tsmiSOPNB_Click(null, null);
            if (setting.location.OpenYm2612[1]) tsmiSOPN2_Click(null, null);
            if (setting.location.OpenOKIM6258[1]) tsmiSOKIM6258_Click(null, null);
            if (setting.location.OpenOKIM6295[1]) tsmiSOKIM6258_Click(null, null);
            if (setting.location.OpenSN76489[1]) tsmiSDCSG_Click(null, null);
            if (setting.location.OpenSegaPCM[1]) tsmiSSegaPCM_Click(null, null);

            log.ForcedWrite("frmMain_Load:STEP 08");

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW+Properties.Resources.planeControl.Width* setting.other.Zoom, frameSizeH+Properties.Resources.planeControl.Height* setting.other.Zoom);
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

                if (!loadAndPlay(args[1],""))
                {
                    frmPlayList.Stop();
                    Audio.Stop();
                    return;
                }

                frmPlayList.setStart(-1);

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

            screen = new DoubleBuffer(pbScreen, Properties.Resources.planeControl, Properties.Resources.font, setting.other.Zoom);
            screen.setting = setting;
            //oldParam = new MDChipParams();
            //newParam = new MDChipParams();
            screen.screenInitAll();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            log.ForcedWrite("終了処理開始");
            log.ForcedWrite("frmMain_FormClosing:STEP 00");

            frmPlayList.Stop();
            frmPlayList.Save();

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

            // 解放
            screen.Dispose();

            setting.location.OInfo = false;
            setting.location.OPlayList = false;
            for (int chipID = 0; chipID < 2; chipID++)
            {
                setting.location.OpenRf5c164[chipID] = false;
                setting.location.OpenC140[chipID] = false;
                setting.location.OpenYm2151[chipID] = false;
                setting.location.OpenYm2608[chipID] = false;
                setting.location.OpenYm2203[chipID] = false;
                setting.location.OpenYm2610[chipID] = false;
                setting.location.OpenYm2612[chipID] = false;
                setting.location.OpenOKIM6258[chipID] = false;
                setting.location.OpenOKIM6295[chipID] = false;
                setting.location.OpenSN76489[chipID] = false;
                setting.location.OpenSegaPCM[chipID] = false;
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

            if (py < 24)//208)
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
                if (px >= n * 16 + 24 && px < n * 16 + 16 + 24) newButton[n] = 1;
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

            //if (py < 14 * 8)
            //{
            //    if (e.Button == MouseButtons.Left)
            //    {
            //        int ch = (py / 8) - 1;
            //        if (ch < 0) return;

            //        if (ch >= 0 && ch < 6)
            //        {
            //            SetChannelMask(vgm.enmUseChip.YM2612, ch);
            //        }
            //        else if (ch < 10)
            //        {
            //            SetChannelMask(vgm.enmUseChip.SN76489, ch - 6);
            //        }
            //        else if (ch < 13)
            //        {
            //            SetChannelMask(vgm.enmUseChip.YM2612, ch - 4);
            //        }

            //    }
            //    else if (e.Button == MouseButtons.Right)
            //    {
            //        for (int ch = 0; ch < 9; ch++) ResetChannelMask(vgm.enmUseChip.YM2612, ch);
            //        for (int ch = 0; ch < 4; ch++) ResetChannelMask(vgm.enmUseChip.SN76489, ch);
            //    }

            //    return;
            //}


            //// 音色表示欄の判定

            //if (py < 26 * 8)
            //{
            //    int h = (py - 14 * 8) / (6 * 8);
            //    int w = Math.Min(px / (13 * 8), 2);
            //    int instCh = h * 3 + w;

            //    //クリップボードに音色をコピーする
            //    getInstCh(vgm.enmUseChip.YM2612, instCh);

            //    return;
            //}



            if (py < 16)
            {
                if (px < 8 * 2) return;
                if (px < 8 * 4)
                {
                    if (py < 8) tsmiPOPN_Click(null, null);
                    else tsmiSOPN_Click(null, null);
                    return;
                }
                if (px < 8 * 6)
                {
                    if (py < 8) tsmiPOPN2_Click(null, null);
                    else tsmiSOPN2_Click(null, null);
                    return;
                }
                if (px < 8 * 8+4)
                {
                    if (py < 8) tsmiPOPNA_Click(null, null);
                    else tsmiSOPNA_Click(null, null);
                    return;
                }
                if (px < 8 * 11)
                {
                    if (py < 8) tsmiPOPNB_Click(null, null);
                    else tsmiSOPNB_Click(null, null);
                    return;
                }
                if (px < 8 * 13+4)
                {
                    if (py < 8) tsmiPOPM_Click(null, null);
                    else tsmiSOPM_Click(null, null);
                    return;
                }
                if (px < 8 * 15+4)
                {
                    if (py < 8) tsmiPDCSG_Click(null, null);
                    else tsmiSDCSG_Click(null, null);
                    return;
                }
                if (px < 8 * 18)
                {
                    if (py < 8) tsmiPRF5C164_Click(null, null);
                    else tsmiSRF5C164_Click(null, null);
                    return;
                }
                if (px < 8 * 20+4)
                {
                    return;
                }
                if (px < 8 * 22 + 4)
                {
                    if (py < 8) tsmiPOKIM6258_Click(null, null);
                    else tsmiSOKIM6258_Click(null, null);
                    return;
                }
                if (px < 8 * 25)
                {
                    if (py < 8) tsmiPOKIM6295_Click(null, null);
                    else tsmiSOKIM6295_Click(null, null);
                    return;
                }
                if (px < 8 * 27 + 4)
                {
                    if (py < 8) tsmiPC140_Click(null, null);
                    else tsmiSC140_Click(null, null);
                    return;
                }
                if (px < 8 * 30)
                {
                    if (py < 8) tsmiPSegaPCM_Click(null, null);
                    else tsmiSSegaPCM_Click(null, null);
                    return;
                }
                return;
            }

            if (py < 24) return;

            // ボタンの判定

            //if (px >= 320 - 16 * 16 && px < 320 - 15 * 16)
            if (px >= 0 * 16+24 && px < 1 * 16 + 24)
            {
                openSetting();
                return;
            }

            //if (px >= 320 - 15 * 16 && px < 320 - 14 * 16)
            if (px >= 1 * 16 + 24 && px < 2 * 16 + 24)
            {
                    frmPlayList.Stop();
                stop();
                return;
            }

            //if (px >= 320 - 14 * 16 && px < 320 - 13 * 16)
            if (px >= 2 * 16 + 24 && px < 3 * 16 + 24)
            {
                pause();
                return;
            }

            //if (px >= 320 - 13 * 16 && px < 320 - 12 * 16)
            if (px >= 3 * 16 + 24 && px < 4 * 16 + 24)
            {
                fadeout();
                frmPlayList.Stop();
                return;
            }

            //if (px >= 320 - 12 * 16 && px < 320 - 11 * 16)
            if (px >= 4 * 16 + 24 && px < 5 * 16 + 24)
            {
                prev();
                return;
            }

            //if (px >= 320 - 11 * 16 && px < 320 - 10 * 16)
            if (px >= 5 * 16 + 24 && px < 6 * 16 + 24)
            {
                slow();
                return;
            }

            //if (px >= 320 - 10 * 16 && px < 320 - 9 * 16)
            if (px >= 6 * 16 + 24 && px < 7 * 16 + 24)
            {
                play();
                return;
            }

            //if (px >= 320 - 9 * 16 && px < 320 - 8 * 16)
            if (px >= 7 * 16 + 24 && px < 8 * 16 + 24)
            {
                ff();
                return;
            }

            //if (px >= 320 - 8 * 16 && px < 320 - 7 * 16)
            if (px >= 8 * 16 + 24 && px < 9 * 16 + 24)
            {
                next();
                return;
            }

            //if (px >= 320 - 7 * 16 && px < 320 - 6 * 16)
            if (px >= 9 * 16 + 24 && px < 10 * 16 + 24)
            {
                playMode();
                return;
            }

            //if (px >= 320 - 6 * 16 && px < 320 - 5 * 16)
            if (px >= 10 * 16 + 24 && px < 11 * 16 + 24)
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

                        loadAndPlay(fn[0],"");
                        frmPlayList.setStart(-1);

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

            //if (px >= 320 - 5 * 16 && px < 320 - 4 * 16)
            if (px >= 11 * 16 + 24 && px < 12 * 16 + 24)
            {
                dispPlayList();
                return;
            }

            //if (px >= 320 - 4 * 16 && px < 320 - 3 * 16)
            if (px >= 12 * 16 + 24 && px < 13 * 16 + 24)
            {
                openInfo();
                return;
            }

            //if (px >= 320 - 3 * 16 && px < 320 - 2 * 16)
            if (px >= 13 * 16 + 24 && px < 14 * 16 + 24)
            {
                OpenMegaCD(0);
                return;
            }

            //if (px >= 320 - 2 * 16 && px < 320 - 1 * 16)
            if (px >= 14 * 16 + 24 && px < 15 * 16 + 24)
            {
                showContextMenu();
                return;
            }

            //if (px >= 320 - 1 * 16 && px < 320 - 0 * 16)
            if (px >= 15 * 16 + 24 && px < 16 * 16 + 24)
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
            OpenMegaCD(0);
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
            OpenFromC140(0);
        }

        private void tsmiPSegaPCM_Click(object sender, EventArgs e)
        {
            OpenFormSegaPCM(0);
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
            OpenMegaCD(1);
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
            OpenFromC140(1);
        }

        private void tsmiSSegaPCM_Click(object sender, EventArgs e)
        {
            OpenFormSegaPCM(1);
        }


        private void OpenMegaCD(int chipID)
        {
            if (frmMCD[chipID] != null)// && frmInfo.isClosed)
            {
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
        }

        private void OpenFormYM2608(int chipID)
        {
            if (frmYM2608[chipID] != null)// && frmInfo.isClosed)
            {
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
        }

        private void OpenFormYM2151(int chipID)
        {
            if (frmYM2151[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
        }

        private void OpenFromC140(int chipID)
        {
            if (frmC140[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
        }

        private void OpenFormYM2203(int chipID)
        {
            if (frmYM2203[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
        }

        private void OpenFormYM2610(int chipID)
        {
            if (frmYM2610[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
        }

        private void OpenFormYM2612(int chipID)
        {
            if (frmYM2612[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
        }

        private void OpenFormOKIM6258(int chipID)
        {
            if (frmOKIM6258[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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

        private void OpenFormOKIM6295(int chipID)
        {
            if (frmOKIM6295[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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

        private void OpenFormSN76489(int chipID)
        {
            if (frmSN76489[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
        }

        private void OpenFormSegaPCM(int chipID)
        {
            if (frmSegaPCM[chipID] != null)// && frmInfo.isClosed)
            {
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
                return;
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
                        loadAndPlay(filename);
                        frmPlayList.setStart(-1);

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

                }

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

                }

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
                screenChangeParamsFromYM2608(chipID);
                screenChangeParamsFromYM2610(chipID);
                screenChangeParamsFromYM2203(chipID);
            }

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
                newParam.ym2203[chipID].channels[ch].inst[46] = (ym2203Register[0xb4 + c] & 0x38) >> 3;//AMS
                newParam.ym2203[chipID].channels[ch].inst[47] = ym2203Register[0xb4 + c] & 0x03;//FMS

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

            for (int ch = 6; ch < 9; ch++) //FM EX
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.ym2203[chipID].channels[ch].pan = 0;

                if (isFmEx)
                {
                    int freq = ym2203Register[0xa8 + c] + (ym2203Register[0xac + c] & 0x07) * 0x100;
                    int octav = (ym2203Register[0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKeyYM2203[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2203[chipID].channels[ch].note = n;
                    newParam.ym2203[chipID].channels[ch].volumeL = Math.Min(Math.Max(ym2203Ch3SlotVol[ch - 5] / 80, 0), 19);
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
                newParam.ym2610[chipID].channels[ch].inst[46] = (YM2610Register[p][0xb4 + c] & 0x38) >> 3;//AMS
                newParam.ym2610[chipID].channels[ch].inst[47] = YM2610Register[p][0xb4 + c] & 0x03;//FMS

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
                    if ((fmKeyYM2610[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
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

            //ADPCM B
            newParam.ym2610[chipID].channels[12].pan = (YM2610Register[0][0x11] & 0xc0) >> 6;
            newParam.ym2610[chipID].channels[12].volumeL = Math.Min(Math.Max(YM2610AdpcmVol[0] / 80, 0), 19);
            newParam.ym2610[chipID].channels[12].volumeR = Math.Min(Math.Max(YM2610AdpcmVol[1] / 80, 0), 19);
            delta = (YM2610Register[0][0x1a] << 8) | YM2610Register[0][0x19];
            frq = (float)(delta / 9447.0f);//Delta=9447 at freq=8kHz
            newParam.ym2610[chipID].channels[12].note = searchYM2608Adpcm(frq);

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
                newParam.ym2608[chipID].channels[ch].inst[46] = (ym2608Register[p][0xb4 + c] & 0x38) >> 3;//AMS
                newParam.ym2608[chipID].channels[ch].inst[47] = ym2608Register[p][0xb4 + c] & 0x03;//FMS

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
                    if ((fmKeyYM2608[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
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

            //ADPCM
            newParam.ym2608[chipID].channels[12].pan = (ym2608Register[1][0x01] & 0xc0) >> 6;
            newParam.ym2608[chipID].channels[12].volumeL = Math.Min(Math.Max(ym2608AdpcmVol[0] / 80, 0), 19);
            newParam.ym2608[chipID].channels[12].volumeR = Math.Min(Math.Max(ym2608AdpcmVol[1] / 80, 0), 19);
            int delta = (ym2608Register[1][0x0a] << 8) | ym2608Register[1][0x09];
            float frq = (float)(delta / 9447.0f);
            newParam.ym2608[chipID].channels[12].note = searchYM2608Adpcm(frq) + 1;

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
                int oct = (ym2151Register[0x28 + ch] & 0x70) >> 4;
                newParam.ym2151[chipID].channels[ch].note = (fmKeyYM2151[ch] > 0) ? (oct * 12 + note + Audio.vgmReal.YM2151Hosei + 1) : -1;

                newParam.ym2151[chipID].channels[ch].volumeL = Math.Min(Math.Max(fmYM2151Vol[ch][0] / 80, 0), 19);
                newParam.ym2151[chipID].channels[ch].volumeR = Math.Min(Math.Max(fmYM2151Vol[ch][1] / 80, 0), 19);
            }
        }

        private void screenChangeParamsFromSegaPCM(int chipID)
        {
            MDSound.segapcm.segapcm_state segapcmState = Audio.GetSegaPCMRegister(chipID);
            if (segapcmState != null && segapcmState.ram!=null && segapcmState.rom!=null)
            {
                for (int ch = 0; ch < 16; ch++)
                {
                    int l = segapcmState.ram[ch * 8 + 2] & 0x7f;
                    int r = segapcmState.ram[ch * 8 + 3] & 0x7f;
                    int dt = segapcmState.ram[ch * 8 + 7];
                    double ml = dt / 256.0;

                    int ptrRom = segapcmState.ptrRom + ((segapcmState.ram[ch * 8 + 0x86] & segapcmState.bankmask) << segapcmState.bankshift);
                    uint addr = (uint)((segapcmState.ram[ch * 8 + 0x85] << 16) | (segapcmState.ram[ch * 8 + 0x84] << 8) | segapcmState.low[ch]);
                    int vdt=0;
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
                    newParam.segaPcm[chipID].channels[ch].note = (ml == 0 || vdt == 0) ? -1 : (searchSegaPCMNote(ml) + 1);
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
                newParam.ym2612[chipID].channels[ch].inst[46] = (fmRegister[p][0xb4 + c] & 0x38) >> 3;//AMS
                newParam.ym2612[chipID].channels[ch].inst[47] = fmRegister[p][0xb4 + c] & 0x03;//FMS

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
            screen.drawChipName( 4 * 4, 0 * 8, 0, ref oldParam.ChipPriOPN,chips[0]);
            screen.drawChipName( 8 * 4, 0 * 8, 1, ref oldParam.ChipPriOPN2, chips[1]);
            screen.drawChipName(13 * 4, 0 * 8, 2, ref oldParam.ChipPriOPNA, chips[2]);
            screen.drawChipName(18 * 4, 0 * 8, 3, ref oldParam.ChipPriOPNB, chips[3]);
            screen.drawChipName(23 * 4, 0 * 8, 4, ref oldParam.ChipPriOPM, chips[4]);
            screen.drawChipName(27 * 4, 0 * 8, 5, ref oldParam.ChipPriDCSG, chips[5]);
            screen.drawChipName(32 * 4, 0 * 8, 6, ref oldParam.ChipPriRF5C, chips[6]);
            screen.drawChipName(37 * 4, 0 * 8, 7, ref oldParam.ChipPriPWM, chips[7]);
            screen.drawChipName(41 * 4, 0 * 8, 8, ref oldParam.ChipPriOKI5, chips[8]);
            screen.drawChipName(46 * 4, 0 * 8, 9, ref oldParam.ChipPriOKI9, chips[9]);
            screen.drawChipName(51 * 4, 0 * 8, 10, ref oldParam.ChipPriC140, chips[10]);
            screen.drawChipName(56 * 4, 0 * 8, 11, ref oldParam.ChipPriSPCM, chips[11]);

            screen.drawChipName(4 * 4, 1 * 8, 0, ref oldParam.ChipSecOPN, chips[128 + 0]);
            screen.drawChipName(8 * 4, 1 * 8, 1, ref oldParam.ChipSecOPN2, chips[128 + 1]);
            screen.drawChipName(13 * 4, 1 * 8, 2, ref oldParam.ChipSecOPNA, chips[128 + 2]);
            screen.drawChipName(18 * 4, 1 * 8, 3, ref oldParam.ChipSecOPNB, chips[128 + 3]);
            screen.drawChipName(23 * 4, 1 * 8, 4, ref oldParam.ChipSecOPM, chips[128 + 4]);
            screen.drawChipName(27 * 4, 1 * 8, 5, ref oldParam.ChipSecDCSG, chips[128 + 5]);
            screen.drawChipName(32 * 4, 1 * 8, 6, ref oldParam.ChipSecRF5C, chips[128 + 6]);
            screen.drawChipName(37 * 4, 1 * 8, 7, ref oldParam.ChipSecPWM, chips[128 + 7]);
            screen.drawChipName(41 * 4, 1 * 8, 8, ref oldParam.ChipSecOKI5, chips[128 + 8]);
            screen.drawChipName(46 * 4, 1 * 8, 9, ref oldParam.ChipSecOKI9, chips[128 + 9]);
            screen.drawChipName(51 * 4, 1 * 8, 10, ref oldParam.ChipSecC140, chips[128 + 10]);
            screen.drawChipName(56 * 4, 1 * 8, 11, ref oldParam.ChipSecSPCM, chips[128 + 11]);

            if (setting.Debug_DispFrameCounter)
            {
                long v = Audio.getVirtualFrameCounter();
                if (v != -1) screen.drawFont8(screen.mainScreen, 0, 0, 0, string.Format("EMU        : {0:D12} ", v));
                long r = Audio.getRealFrameCounter();
                if (r != -1) screen.drawFont8(screen.mainScreen, 0, 8, 0, string.Format("SCCI       : {0:D12} ", r));
                long d = r - v;
                if (r != -1 && v != -1) screen.drawFont8(screen.mainScreen, 0, 16, 0, string.Format("SCCI - EMU : {0:D12} ", d));

            }

            screen.drawFont4(screen.mainScreen, 0, 24, 1, Audio.GetIsDataBlock(vgm.enmModel.VirtualModel) ? "VD" : "  ");
            screen.drawFont4(screen.mainScreen, 12, 24, 1, Audio.GetIsPcmRAMWrite(vgm.enmModel.VirtualModel) ? "VP" : "  ");
            screen.drawFont4(screen.mainScreen, 0, 32, 1, Audio.GetIsDataBlock(vgm.enmModel.RealModel) ? "RD" : "  ");
            screen.drawFont4(screen.mainScreen, 12, 32, 1, Audio.GetIsPcmRAMWrite(vgm.enmModel.RealModel) ? "RP" : "  ");

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
                if (freq < Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))) break;
                n = i;
                float a = Math.Abs(freq - (float)(Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }

            return n;
        }



        private void openSetting()
        {
            frmSetting frm = new frmSetting(setting);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                StopMIDIInMonitoring();
                frmPlayList.Stop();
                Audio.Close();

                setting = frm.setting;
                setting.Save();

                screen.setting = setting;
                //oldParam = new MDChipParams();
                //newParam = new MDChipParams();
                screen.screenInitAll();

                log.ForcedWrite("設定が変更されたため、再度Audio初期化処理開始");

                Audio.Init(setting);

                log.ForcedWrite("Audio初期化処理完了");
                log.debug = setting.Debug_DispFrameCounter;

                StartMIDIInMonitoring();

                IsInitialOpenFolder = true;

            }
        }

        private void stop()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            frmPlayList.Stop();
            Audio.Stop();

        }

        private void pause()
        {
            Audio.Pause();
        }

        private void fadeout()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            Audio.Fadeout();
        }

        private void prev()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            frmPlayList.prevPlay();
        }

        private void play()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            string[] fn = null;
            Tuple<string, string> playFn = null;

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

            loadAndPlay(playFn.Item1, playFn.Item2);
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
                for (int ch = 0; ch < 8; ch++) ResetChannelMask(vgm.enmUseChip.YM2151, chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(vgm.enmUseChip.YM2203, chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask(vgm.enmUseChip.YM2608, chipID, ch);
                for (int ch = 0; ch < 14; ch++) ResetChannelMask(vgm.enmUseChip.YM2610, chipID, ch);
                for (int ch = 0; ch < 9; ch++) ResetChannelMask(vgm.enmUseChip.YM2612,chipID, ch);
                for (int ch = 0; ch < 4; ch++) ResetChannelMask(vgm.enmUseChip.SN76489, chipID, ch);
                for (int ch = 0; ch < 8; ch++) ResetChannelMask(vgm.enmUseChip.RF5C164, chipID, ch);
                for (int ch = 0; ch < 24; ch++) ResetChannelMask(vgm.enmUseChip.C140, chipID, ch);
                for (int ch = 0; ch < 16; ch++) ResetChannelMask(vgm.enmUseChip.SEGAPCM, chipID, ch);
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
        }

        private void ff()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            Audio.FF();
        }

        private void next()
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

        private void slow()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
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
            ofd.Filter = "VGMファイル(*.vgm;*.vgz)|*.vgm;*.vgz";
            ofd.Title = "VGM/VGZファイルを選択してください";
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

        private void openInfo()
        {
            if (frmInfo != null && !frmInfo.isClosed)
            {
                frmInfo.Close();
                frmInfo.Dispose();
                frmInfo = null;
                return;
            }

            if (frmInfo != null)
            {
                frmInfo.Close();
                frmInfo.Dispose();
                frmInfo = null;
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
            frmInfo.setting = setting;
            frmInfo.Show();
            frmInfo.update();
        }

        private void showContextMenu()
        {
            cmsOpenOtherPanel.Show();
            System.Drawing.Point p = Control.MousePosition;
            cmsOpenOtherPanel.Top = p.Y;
            cmsOpenOtherPanel.Left = p.X;
        }


        public const int FCC_VGM = 0x206D6756;	// "Vgm "

        public byte[] getAllBytes(string filename)
        {
            byte[] buf = System.IO.File.ReadAllBytes(filename);
            uint vgm = (UInt32)buf[0] + (UInt32)buf[1] * 0x100 + (UInt32)buf[2] * 0x10000 + (UInt32)buf[3] * 0x1000000;
            if (vgm == FCC_VGM) return buf;


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

            return outStream.ToArray();
        }

        public void getInstCh(vgm.enmUseChip chip, int ch, int chipID)
        {
            if (!setting.other.UseGetInst) return;

            switch (setting.other.InstFormat) {
                case Setting.Other.enmInstFormat.FMP7:
                    getInstChForFMP7(chip, ch, chipID);
                    break;
                case Setting.Other.enmInstFormat.MDX:
                    getInstChForMDX(chip, ch, chipID);
                    break;
                case Setting.Other.enmInstFormat.MML2VGM:
                    getInstChForMML2VGM(chip, ch, chipID);
                    break;
                case Setting.Other.enmInstFormat.MUSICLALF:
                    getInstChForMUSICLALF(chip, ch, chipID);
                    break;
                case Setting.Other.enmInstFormat.MUSICLALF2:
                    getInstChForMUSICLALF2(chip, ch, chipID);
                    break;
                case Setting.Other.enmInstFormat.TFI:
                    getInstChForTFI(chip, ch, chipID);
                    break;
                case Setting.Other.enmInstFormat.NRTDRV:
                    getInstChForNRTDRV(chip, ch, chipID);
                    break;
            }
        }

        private void getInstChForFMP7(vgm.enmUseChip chip, int ch,int chipID)
        {

            string n = "";

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip==vgm.enmUseChip.YM2203 || chip==vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
            else if (chip == vgm.enmUseChip.YM2151)
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

        private void getInstChForMDX(vgm.enmUseChip chip, int ch,int chipID)
        {

            string n = "";

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip == vgm.enmUseChip.YM2203 || chip == vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
            else if (chip == vgm.enmUseChip.YM2151)
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

        private void getInstChForMML2VGM(vgm.enmUseChip chip, int ch,int chipID)
        {

            string n = "";

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip == vgm.enmUseChip.YM2203 || chip == vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

                n = "'@ M xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSG-EG\r\n";

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
            else if (chip == vgm.enmUseChip.YM2151)
            {
                int[] ym2151Register = Audio.GetYM2151Register(chipID);
                n = "'@ M xx\r\n   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSG-EG\r\n";

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
                        , (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7 //AM
                        , 0
                    );
                }
                n += "   ALG FB\r\n";
                n += string.Format("'@ {0:D3},{1:D3}\r\n"
                    , ym2151Register[0x20 + ch] & 0x07 //AL
                    , (ym2151Register[0x20 + ch] & 0x38) >> 3//FB
                );
            }

            Clipboard.SetText(n);
        }

        private void getInstChForMUSICLALF(vgm.enmUseChip chip, int ch,int chipID)
        {

            string n = "";

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip == vgm.enmUseChip.YM2203 || chip == vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
            else if (chip == vgm.enmUseChip.YM2151)
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

        private void getInstChForMUSICLALF2(vgm.enmUseChip chip, int ch,int chipID)
        {

            string n = "";

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip == vgm.enmUseChip.YM2203 || chip == vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
            else if (chip == vgm.enmUseChip.YM2151)
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

        private void getInstChForNRTDRV(vgm.enmUseChip chip, int ch,int chipID)
        {

            string n = "";

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip == vgm.enmUseChip.YM2203 || chip == vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
            else if (chip == vgm.enmUseChip.YM2151)
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

        private void getInstChForTFI(vgm.enmUseChip chip, int ch,int chipID)
        {

            byte[] n = new byte[42];

            if (chip == vgm.enmUseChip.YM2612 || chip == vgm.enmUseChip.YM2608 || chip == vgm.enmUseChip.YM2203 || chip == vgm.enmUseChip.YM2610)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                int[][] fmRegister = (chip == vgm.enmUseChip.YM2612) ? Audio.GetFMRegister(chipID) : (chip == vgm.enmUseChip.YM2608 ? Audio.GetYM2608Register(chipID) : (chip == vgm.enmUseChip.YM2203 ? new int[][] { Audio.GetYM2203Register(chipID), null } : Audio.GetYM2610Register(chipID)));

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
            else if (chip == vgm.enmUseChip.YM2151)
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
                    dt = (dt < 4) ? (dt + 3) : (7-dt);
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

                    if (!loadAndPlay(sParam))
                    {
                        frmPlayList.Stop();
                        Audio.Stop();
                        return;
                    }

                    frmPlayList.setStart(-1);

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



        private void StartMIDIInMonitoring()
        {

            //if (setting.other.MidiInDeviceName == "")
            //{
            //    return;
            //}

            //if (midiin != null)
            //{
            //    try
            //    {
            //        midiin.Stop();
            //        midiin.Dispose();
            //        midiin.MessageReceived -= midiIn_MessageReceived;
            //        midiin.ErrorReceived -= midiIn_ErrorReceived;
            //        midiin = null;
            //    }
            //    catch
            //    {
            //        midiin = null;
            //    }
            //}

            //if (midiin == null)
            //{
            //    for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            //    {
            //        if (setting.other.MidiInDeviceName == MidiIn.DeviceInfo(i).ProductName)
            //        {
            //            try
            //            {
            //                midiin = new MidiIn(i);
            //                midiin.MessageReceived += midiIn_MessageReceived;
            //                midiin.ErrorReceived += midiIn_ErrorReceived;
            //                midiin.Start();
            //            }
            //            catch
            //            {
            //                midiin = null;
            //            }
            //        }
            //    }
            //}

        }

        void midiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            //            Console.WriteLine(String.Format("Error Time {0} Message 0x{1:X8} Event {2}",
            //                e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        private void StopMIDIInMonitoring()
        {
            //if (midiin != null)
            //{
            //    try
            //    {
            //        midiin.Stop();
            //        midiin.Dispose();
            //        midiin.MessageReceived -= midiIn_MessageReceived;
            //        midiin.ErrorReceived -= midiIn_ErrorReceived;
            //        midiin = null;
            //    }
            //    catch
            //    {
            //        midiin = null;
            //    }
            //}
        }

        void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn || e.MidiEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                //Console.WriteLine(String.Format("Time {0} Message 0x{1:X8} Event {2}",
                //    e.Timestamp, e.RawMessage, e.MidiEvent));
            }
        }

        public bool loadAndPlay(string fn,string zfn=null)
        {
            try
            {
                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                if (zfn == null || zfn == "")
                {
                    srcBuf = getAllBytes(fn);
                }
                else
                {
                    using (ZipArchive archive = ZipFile.OpenRead(zfn))
                    {
                        ZipArchiveEntry entry = archive.GetEntry(fn);
                        srcBuf = getBytesFromZipFile(entry);
                    }
                }

                Audio.SetVGMBuffer(srcBuf);

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

        public byte[] getBytesFromZipFile(ZipArchiveEntry entry)
        {
            byte[] buf=null;

            if (entry.FullName.EndsWith(".vgm", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(".vgz", StringComparison.OrdinalIgnoreCase))
            {
                //Console.WriteLine(entry.FullName);
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

        public void SetChannelMask(vgm.enmUseChip chip, int chipID, int ch)
        {
            switch (chip)
            {
                case vgm.enmUseChip.YM2203:
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
                case vgm.enmUseChip.YM2608:
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
                case vgm.enmUseChip.YM2610:
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
                case vgm.enmUseChip.YM2612:
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
                case vgm.enmUseChip.SN76489:
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
                case vgm.enmUseChip.RF5C164:
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
                case vgm.enmUseChip.YM2151:
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
                case vgm.enmUseChip.C140:
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
                case vgm.enmUseChip.SEGAPCM:
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
            }
        }

        public void ResetChannelMask(vgm.enmUseChip chip,int chipID, int ch)
        {
            switch (chip)
            {
                case vgm.enmUseChip.SN76489:
                    newParam.sn76489[chipID].channels[ch].mask = false;
                    Audio.resetSN76489Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.RF5C164:
                    newParam.rf5c164[chipID].channels[ch].mask = false;
                    Audio.resetRF5C164Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.YM2151:
                    newParam.ym2151[chipID].channels[ch].mask = false;
                    Audio.resetYM2151Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.YM2203:
                    newParam.ym2203[chipID].channels[ch].mask = false;
                    Audio.resetYM2203Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.YM2608:
                    newParam.ym2608[chipID].channels[ch].mask = false;
                    Audio.resetYM2608Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.YM2610:
                    if(ch<12)
                    newParam.ym2610[chipID].channels[ch].mask = false;
                    else if(ch==12)
                    newParam.ym2610[chipID].channels[13].mask = false;
                    else if(ch==13)
                    newParam.ym2610[chipID].channels[12].mask = false;

                    Audio.resetYM2610Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.YM2612:
                    newParam.ym2612[chipID].channels[ch].mask = false;
                    if (ch == 2)
                    {
                        newParam.ym2612[chipID].channels[6].mask = false;
                        newParam.ym2612[chipID].channels[7].mask = false;
                        newParam.ym2612[chipID].channels[8].mask = false;
                    }
                    if (ch < 6) Audio.resetYM2612Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.C140:
                    newParam.c140[chipID].channels[ch].mask = false;
                    if (ch < 24) Audio.resetC140Mask(chipID, ch);
                    break;
                case vgm.enmUseChip.SEGAPCM:
                    newParam.segaPcm[chipID].channels[ch].mask = false;
                    if (ch < 16) Audio.resetSegaPCMMask(chipID, ch);
                    break;

            }
        }

    }
}
