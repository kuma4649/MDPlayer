﻿using System;
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
        private frmMegaCD frmMCD = null;
        private frmC140 frmC140 = null;
        private frmYM2608 frmYM2608 = null;
        private frmYM2151 frmYM2151 = null;
        private frmPlayList frmPlayList = null;

        private MDChipParams oldParam = new MDChipParams();
        private MDChipParams newParam = new MDChipParams();

        private int[] oldButton = new int[14];
        private int[] newButton = new int[14];
        private int[] oldButtonMode = new int[14];
        private int[] newButtonMode = new int[14];

        private bool isRunning = false;
        private bool stopped = false;

        private bool IsInitialOpenFolder = true;

        private int[] FmFNum = new int[] {
            0x289/8, 0x2af/8, 0x2d8/8, 0x303/8, 0x331/8, 0x362/8, 0x395/8, 0x3cc/8, 0x405/8, 0x443/8, 0x484/8,0x4c8/8,
            0x289/4, 0x2af/4, 0x2d8/4, 0x303/4, 0x331/4, 0x362/4, 0x395/4, 0x3cc/4, 0x405/4, 0x443/4, 0x484/4,0x4c8/4,
            0x289/2, 0x2af/2, 0x2d8/2, 0x303/2, 0x331/2, 0x362/2, 0x395/2, 0x3cc/2, 0x405/2, 0x443/2, 0x484/2,0x4c8/2,
            0x289, 0x2af, 0x2d8, 0x303, 0x331, 0x362, 0x395, 0x3cc, 0x405, 0x443, 0x484, 0x4c8,
            0x289*2, 0x2af*2, 0x2d8*2, 0x303*2, 0x331*2, 0x362*2, 0x395*2, 0x3cc*2, 0x405*2, 0x443*2, 0x484*2,0x4c8*2
        };

        private int[] PsgFNum = new int[] {
            0x6ae,0x64e,0x5f4,0x59e,0x54e,0x502,0x4ba,0x476,0x436,0x3f8,0x3c0,0x38a, // 0
            0x357,0x327,0x2fa,0x2cf,0x2a7,0x281,0x25d,0x23b,0x21b,0x1fc,0x1e0,0x1c5, // 1
            0x1ac,0x194,0x17d,0x168,0x153,0x140,0x12e,0x11d,0x10d,0x0fe,0x0f0,0x0e3, // 2
            0x0d6,0x0ca,0x0be,0x0b4,0x0aa,0x0a0,0x097,0x08f,0x087,0x07f,0x078,0x071, // 3
            0x06b,0x065,0x05f,0x05a,0x055,0x050,0x04c,0x047,0x043,0x040,0x03c,0x039, // 4
            0x035,0x032,0x030,0x02d,0x02a,0x028,0x026,0x024,0x022,0x020,0x01e,0x01c, // 5
            0x01b,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010,0x00f,0x00e, // 6
            0x00d,0x00d,0x00c,0x00b,0x00b,0x00a,0x009,0x008,0x007,0x006,0x005,0x004  // 7
        };

        private float[] pcmMTbl = new float[]
        {
            1.0f
            ,1.05947557526183f
            ,1.122467701246082f
            ,1.189205718217262f
            ,1.259918966439875f
            ,1.334836786178427f
            ,1.414226741074841f
            ,1.498318171393624f
            ,1.587416864154117f
            ,1.681828606375659f
            ,1.781820961700176f
            ,1.887776163901842f
        };

        private static int SamplingRate = 44100;
        private byte[] srcBuf;

        public Setting setting = Setting.Load();

        //private MidiIn midiin = null;


        public frmMain()
        {
            InitializeComponent();

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

            pbScreen.AllowDrop = true;
            Audio.Init(setting);
            StartMIDIInMonitoring();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (setting.location.PMain != System.Drawing.Point.Empty)
                this.Location = setting.location.PMain;

            // DoubleBufferオブジェクトの作成

            pbRf5c164Screen = new PictureBox();
            pbRf5c164Screen.Width = 320;
            pbRf5c164Screen.Height = 72;

            screen = new DoubleBuffer(pbScreen, Properties.Resources.plane, Properties.Resources.font);
            screen.setting = setting;
            screen.screenInit();
            oldParam = new MDChipParams();
            newParam = new MDChipParams();

            pWidth = pbScreen.Width;
            pHeight = pbScreen.Height;

            frmPlayList = new frmPlayList();
            frmPlayList.frmMain = this;
            frmPlayList.Show();
            frmPlayList.Visible =false;
            frmPlayList.Opacity = 1.0;
            frmPlayList.Location = new System.Drawing.Point(this.Location.X + 328, this.Location.Y + 264);
            frmPlayList.Refresh();

            if (setting.location.OPlayList) dispPlayList();
            if (setting.location.ORf5c164) openMegaCD();
            if (setting.location.OInfo) openInfo();
            if (setting.location.OC140) tsmiC140_Click(null, null);
            if (setting.location.OYm2151) tsmiOPM_Click(null, null);
            if (setting.location.OYm2608) tsmiOPNA_Click(null, null);

        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            // リサイズ時は再確保

            if (screen != null) screen.Dispose();

            screen = new DoubleBuffer(pbScreen, Properties.Resources.plane, Properties.Resources.font);
            screen.setting = setting;
            screen.screenInit();
            //oldParam = new MDChipParams();
            newParam = new MDChipParams();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            System.Threading.Thread trd = new System.Threading.Thread(screenMainLoop);
            trd.Start();
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                return;
            }

            try
            {

                srcBuf = getAllBytes(args[1]);
                Audio.SetVGMBuffer(srcBuf);
                play();

            }
            catch
            {
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmPlayList.Stop();
            frmPlayList.Save();
            StopMIDIInMonitoring();
            Audio.Close();
            isRunning = false;
            while (!stopped)
            {
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }
            // 解放
            screen.Dispose();

            setting.location.OInfo = false;
            setting.location.OPlayList = false;
            setting.location.ORf5c164 = false;
            setting.location.OC140 = false;
            setting.location.OYm2151 = false;
            setting.location.OYm2608 = false;

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
            if (frmMCD != null && !frmMCD.isClosed)
            {
                setting.location.PRf5c164 = frmMCD.Location;
                setting.location.ORf5c164 = true;
            }
            if (frmC140 != null && !frmC140.isClosed)
            {
                setting.location.PC140 = frmC140.Location;
                setting.location.OC140 = true;
            }
            if (frmYM2151 != null && !frmYM2151.isClosed)
            {
                setting.location.PYm2151 = frmYM2151.Location;
                setting.location.OYm2151 = true;
            }
            if (frmYM2608 != null && !frmYM2608.isClosed)
            {
                setting.location.PYm2608 = frmYM2608.Location;
                setting.location.OYm2608 = true;
            }

            setting.Save();
        }

        private void pbScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Location.Y < 208)
            {
                for (int n = 0; n < newButton.Length; n++)
                {
                    newButton[n] = 0;
                }
                return;
            }

            for (int n = 0; n < newButton.Length; n++)
            {
                if (e.Location.X >= 320 - (15 - n) * 16 && e.Location.X < 320 - (14 - n) * 16) newButton[n] = 1;
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
            if (e.Location.Y < 14 * 8)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int ch = (e.Location.Y / 8) - 1;
                    if (ch < 0) return;
                    if (ch >= 0 && ch < 6)
                    {
                        if (!newParam.ym2612.channels[ch].mask)
                        {
                            Audio.setFMMask(ch);

                        }
                        else
                        {
                            Audio.resetFMMask(ch);
                        }
                        newParam.ym2612.channels[ch].mask = !newParam.ym2612.channels[ch].mask;
                        if (ch == 2)
                        {
                            newParam.ym2612.channels[6].mask = newParam.ym2612.channels[2].mask;
                            newParam.ym2612.channels[7].mask = newParam.ym2612.channels[2].mask;
                            newParam.ym2612.channels[8].mask = newParam.ym2612.channels[2].mask;
                        }
                    }
                    else if (ch < 10)
                    {
                        if (!newParam.sn76489.channels[ch - 6].mask)
                        {
                            Audio.setPSGMask(ch - 6);

                        }
                        else
                        {
                            Audio.resetPSGMask(ch - 6);
                        }
                        newParam.sn76489.channels[ch - 6].mask = !newParam.sn76489.channels[ch - 6].mask;
                    }
                    else if (ch < 13)
                    {
                        if (!newParam.ym2612.channels[2].mask)
                        {
                            Audio.setFMMask(2);

                        }
                        else
                        {
                            Audio.resetFMMask(2);
                        }
                        newParam.ym2612.channels[2].mask = !newParam.ym2612.channels[2].mask;
                        newParam.ym2612.channels[6].mask = newParam.ym2612.channels[2].mask;
                        newParam.ym2612.channels[7].mask = newParam.ym2612.channels[2].mask;
                        newParam.ym2612.channels[8].mask = newParam.ym2612.channels[2].mask;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    for (int ch = 0; ch < 9; ch++)
                    {
                        newParam.ym2612.channels[ch].mask = false;
                        if (ch < 6) Audio.resetFMMask(ch);
                    }
                    for (int ch = 0; ch < 4; ch++)
                    {
                        newParam.sn76489.channels[ch].mask = false;
                        Audio.resetPSGMask(ch);
                    }
                }

                return;
            }


            // 音色表示欄の判定

            if (e.Location.Y < 26 * 8)
            {
                int h = (e.Location.Y - 14 * 8) / (6 * 8);
                int w = Math.Min(e.Location.X / (13 * 8), 2);
                int instCh = h * 3 + w;

                //クリップボードに音色をコピーする
                getInstCh(instCh);

                return;
            }


            // ボタンの判定

            if (e.Location.X >= 320 - 15 * 16 && e.Location.X < 320 - 14 * 16)
            {
                openSetting();
                return;
            }

            if (e.Location.X >= 320 - 14 * 16 && e.Location.X < 320 - 13 * 16)
            {
                frmPlayList.Stop();
                stop();
                return;
            }

            if (e.Location.X >= 320 - 13 * 16 && e.Location.X < 320 - 12 * 16)
            {
                pause();
                return;
            }

            if (e.Location.X >= 320 - 12 * 16 && e.Location.X < 320 - 11 * 16)
            {
                fadeout();
                frmPlayList.Stop();
                return;
            }

            if (e.Location.X >= 320 - 11 * 16 && e.Location.X < 320 - 10 * 16)
            {
                prev();
                return;
            }

            if (e.Location.X >= 320 - 10 * 16 && e.Location.X < 320 - 9 * 16)
            {
                slow();
                return;
            }

            if (e.Location.X >= 320 - 9 * 16 && e.Location.X < 320 - 8 * 16)
            {
                play();
                return;
            }

            if (e.Location.X >= 320 - 8 * 16 && e.Location.X < 320 - 7 * 16)
            {
                ff();
                return;
            }

            if (e.Location.X >= 320 - 7 * 16 && e.Location.X < 320 - 6 * 16)
            {
                next();
                return;
            }

            if (e.Location.X >= 320 - 6 * 16 && e.Location.X < 320 - 5 * 16)
            {
                playMode();
                return;
            }

            if (e.Location.X >= 320 - 5 * 16 && e.Location.X < 320 - 4 * 16)
            {
                string[] fn = fileOpen(true);

                if (fn != null)
                {
                    if (fn.Length == 1)
                    {
                        frmPlayList.Stop();

                        frmPlayList.AddList(fn[0]);

                        loadAndPlay(fn[0]);
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
                        catch { }
                    }
                }

                return;
            }

            if (e.Location.X >= 320 - 4 * 16 && e.Location.X < 320 - 3 * 16)
            {
                dispPlayList();
                return;
            }

            if (e.Location.X >= 320 - 3 * 16 && e.Location.X < 320 - 2 * 16)
            {
                openInfo();
                return;
            }

            if (e.Location.X >= 320 - 2 * 16 && e.Location.X < 320 - 1 * 16)
            {
                openMegaCD();
                return;
            }

            if (e.Location.X >= 320 - 1 * 16 && e.Location.X < 320 - 0 * 16)
            {
                showContextMenu();
                return;
            }

        }

        private void screenMainLoop()
        {
            double nextFrame = (double)System.Environment.TickCount;
            float period = 1000f / 60f;
            isRunning = true;
            stopped = false;

            while (isRunning)
            {
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

                if (frmMCD != null && !frmMCD.isClosed) frmMCD.screenChangeParams();
                else frmMCD = null;

                if (frmC140 != null && !frmC140.isClosed) frmC140.screenChangeParams();
                else frmC140 = null;

                if ((double)System.Environment.TickCount >= nextFrame + period)
                {
                    nextFrame += period;
                    continue;
                }

                screenDrawParams();

                if (frmMCD != null && !frmMCD.isClosed) frmMCD.screenDrawParams();
                else frmMCD = null;

                if (frmC140 != null && !frmC140.isClosed) frmC140.screenDrawParams();
                else frmC140 = null;

                if (frmYM2608 != null && !frmYM2608.isClosed) frmYM2608.screenDrawParams();
                else frmYM2608 = null;

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
                    frmPlayList.Stop();
                    try { Audio.Stop(); } catch { }
                    try { Audio.Close(); } catch { }
                    Audio.fatalError = false;
                    Audio.Init(setting);
                }
            }

            stopped = true;
        }

        private void screenChangeParams()
        {
            int[][] fmRegister = Audio.GetFMRegister();
            int[][] fmVol = Audio.GetFMVolume();
            int[] fmCh3SlotVol = Audio.GetFMCh3SlotVolume();
            int[] fmKey = Audio.GetFMKeyOn();
            int[][] psgVol = Audio.GetPSGVolume();
            int[] ym2151Register = Audio.GetYM2151Register();
            int[] fmKeyYM2151 = Audio.GetYM2151KeyOn();
            int[][] fmYM2151Vol = Audio.GetYM2151Volume();

            bool isFmEx = (fmRegister[0][0x27] & 0x40) > 0;

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.ym2612.channels[ch].inst[i * 11 + 0] = fmRegister[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.ym2612.channels[ch].inst[i * 11 + 1] = fmRegister[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.ym2612.channels[ch].inst[i * 11 + 2] = fmRegister[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.ym2612.channels[ch].inst[i * 11 + 3] = fmRegister[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.ym2612.channels[ch].inst[i * 11 + 4] = (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.ym2612.channels[ch].inst[i * 11 + 5] = fmRegister[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.ym2612.channels[ch].inst[i * 11 + 6] = (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.ym2612.channels[ch].inst[i * 11 + 7] = fmRegister[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.ym2612.channels[ch].inst[i * 11 + 8] = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.ym2612.channels[ch].inst[i * 11 + 9] = (fmRegister[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.ym2612.channels[ch].inst[i * 11 + 10] = fmRegister[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.ym2612.channels[ch].inst[44] = fmRegister[p][0xb0 + c] & 0x07;//AL
                newParam.ym2612.channels[ch].inst[45] = (fmRegister[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.ym2612.channels[ch].inst[46] = (fmRegister[p][0xb4 + c] & 0x38) >> 3;//AMS
                newParam.ym2612.channels[ch].inst[47] = fmRegister[p][0xb4 + c] & 0x03;//FMS

                newParam.ym2612.channels[ch].pan = (fmRegister[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = fmRegister[p][0xa0 + c] + (fmRegister[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (fmRegister[p][0xa4 + c] & 0x38) >> 3;

                    if (fmKey[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2612.channels[ch].volumeL = Math.Min(Math.Max(fmVol[ch][0] / 80, 0), 19);
                    newParam.ym2612.channels[ch].volumeR = Math.Min(Math.Max(fmVol[ch][1] / 80, 0), 19);
                }
                else
                {
                    freq = fmRegister[0][0xa9] + (fmRegister[0][0xad] & 0x07) * 0x100;
                    octav = (fmRegister[0][0xad] & 0x38) >> 3;

                    if ((fmKey[2] & 0x10) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2612.channels[2].volumeL = Math.Min(Math.Max(fmCh3SlotVol[0] / 80, 0), 19);
                    newParam.ym2612.channels[2].volumeR = Math.Min(Math.Max(fmCh3SlotVol[0] / 80, 0), 19);
                }
                newParam.ym2612.channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++)
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.ym2612.channels[ch].pan = 0;

                if (isFmEx)
                {
                    int freq = fmRegister[0][0xa8 + c] + (fmRegister[0][0xac + c] & 0x07) * 0x100;
                    int octav = (fmRegister[0][0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKey[2] & (0x20 << (ch - 6))) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
                    newParam.ym2612.channels[ch].note = n;
                    newParam.ym2612.channels[ch].volumeL = Math.Min(Math.Max(fmCh3SlotVol[ch - 5] / 80, 0), 19);
                }
                else
                {
                    newParam.ym2612.channels[ch].note = -1;
                    newParam.ym2612.channels[ch].volumeL = 0;
                }
            }

            newParam.ym2612.channels[5].pcmMode = (fmRegister[0][0x2b] & 0x80) >> 7;

            int[] psgRegister = Audio.GetPSGRegister();
            for (int ch = 0; ch < 4; ch++)
            {
                if (psgRegister[ch * 2 + 1] != 15)
                {
                    newParam.sn76489.channels[ch].note = searchPSGNote(psgRegister[ch * 2]);
                }
                else
                {
                    newParam.sn76489.channels[ch].note = -1;
                }

                newParam.sn76489.channels[ch].volume = Math.Min(Math.Max((int)((psgVol[ch][0] + psgVol[ch][1]) / (30.0 / 19.0)), 0), 19);
            }

            MDSound.scd_pcm.pcm_chip_ rf5c164Register = Audio.GetRf5c164Register();
            int[][] rf5c164Vol = Audio.GetRf5c164Volume();
            for (int ch = 0; ch < 8; ch++)
            {
                if (rf5c164Register.Channel[ch].Enable != 0)
                {
                    newParam.rf5c164.channels[ch].note = searchRf5c164Note(rf5c164Register.Channel[ch].Step_B);
                    newParam.rf5c164.channels[ch].volumeL = Math.Min(Math.Max(rf5c164Vol[ch][0] / 400, 0), 19);
                    newParam.rf5c164.channels[ch].volumeR = Math.Min(Math.Max(rf5c164Vol[ch][1] / 400, 0), 19);
                }
                else
                {
                    newParam.rf5c164.channels[ch].note = -1;
                    newParam.rf5c164.channels[ch].volumeL = 0;
                    newParam.rf5c164.channels[ch].volumeR = 0;
                }
                newParam.rf5c164.channels[ch].pan = (int)rf5c164Register.Channel[ch].PAN;
            }

            MDSound.c140.c140_state c140State = Audio.GetC140Register();
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

                newParam.c140.channels[ch].note = frequency == 0 ? -1 : searchC140Note(frequency);
                newParam.c140.channels[ch].pan = ((l>>2) & 0xf) | (((r>>2) & 0xf) << 4);
                newParam.c140.channels[ch].volumeL = Math.Min(Math.Max((l * vdt) >> 7, 0), 19);
                newParam.c140.channels[ch].volumeR = Math.Min(Math.Max((r * vdt) >> 7, 0), 19);
            }

            for (int ch = 0; ch < 8; ch++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    newParam.ym2151.channels[ch].inst[i * 11 + 0] = ym2151Register[0x80 + ops + ch] & 0x1f; //AR
                    newParam.ym2151.channels[ch].inst[i * 11 + 1] = ym2151Register[0xa0 + ops + ch] & 0x1f; //DR
                    newParam.ym2151.channels[ch].inst[i * 11 + 2] = ym2151Register[0xc0 + ops + ch] & 0x1f; //SR
                    newParam.ym2151.channels[ch].inst[i * 11 + 3] = ym2151Register[0xe0 + ops + ch] & 0x0f; //RR
                    newParam.ym2151.channels[ch].inst[i * 11 + 4] = (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4;//SL
                    newParam.ym2151.channels[ch].inst[i * 11 + 5] = ym2151Register[0x60 + ops + ch] & 0x7f;//TL
                    newParam.ym2151.channels[ch].inst[i * 11 + 6] = (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6;//KS
                    newParam.ym2151.channels[ch].inst[i * 11 + 7] = ym2151Register[0x40 + ops + ch] & 0x0f;//ML
                    newParam.ym2151.channels[ch].inst[i * 11 + 8] = (ym2151Register[0x40 + ops + ch] & 0x70) >> 4;//DT
                    newParam.ym2151.channels[ch].inst[i * 11 + 9] = (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6;//DT2
                    newParam.ym2151.channels[ch].inst[i * 11 + 10] = (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7;//AM
                }
                newParam.ym2151.channels[ch].inst[44] = ym2151Register[0x20 + ch] & 0x07;//AL
                newParam.ym2151.channels[ch].inst[45] = (ym2151Register[0x20 + ch] & 0x38) >> 3;//FB
                newParam.ym2151.channels[ch].inst[46] = (ym2151Register[0x38 + ch] & 0x3);//AMS
                newParam.ym2151.channels[ch].inst[47] = (ym2151Register[0x38 + ch] & 0x70)>>4;//PMS

                newParam.ym2151.channels[ch].pan = (ym2151Register[0x20 + ch] & 0xc0) >> 6;
                int note= (ym2151Register[0x28 + ch] & 0x0f);
                note = (note < 3) ? note : (note < 7 ? note - 1 : (note < 11 ? note - 2 : note - 3));
                int oct = (ym2151Register[0x28 + ch] & 0x70) >> 4;
                newParam.ym2151.channels[ch].note = (fmKeyYM2151[ch] > 0) ? (oct * 12 + note) : -1;

                newParam.ym2151.channels[ch].volumeL = Math.Min(Math.Max(fmYM2151Vol[ch][0] / 80, 0), 19);
                newParam.ym2151.channels[ch].volumeR = Math.Min(Math.Max(fmYM2151Vol[ch][1] / 80, 0), 19);
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

        private void screenDrawParams()
        {
            // 描画
            screen.drawParams(oldParam, newParam);

            screen.drawButtons(oldButton, newButton, oldButtonMode, newButtonMode);

            screen.drawTimer(0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            screen.drawTimer(1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            screen.drawTimer(2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            int tp = setting.YM2612Type.UseScci ? 1 : 0;
            int tp6 = tp;
            if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation)
            {
                tp6 = newParam.ym2612.channels[5].pcmMode == 0 ? 1 : 0;
            }
            screen.drawCh6(ref oldParam.ym2612.channels[5].pcmMode, newParam.ym2612.channels[5].pcmMode
                , ref oldParam.ym2612.channels[5].mask, newParam.ym2612.channels[5].mask
                , ref oldParam.ym2612.channels[5].tp, tp6);
            for (int ch = 0; ch < 5; ch++)
            {
                screen.drawCh(ch, ref oldParam.ym2612.channels[ch].mask, newParam.ym2612.channels[ch].mask,tp);
            }
            for (int ch = 6; ch < 10; ch++)
            {
                screen.drawCh(ch, ref oldParam.sn76489.channels[ch - 6].mask, newParam.sn76489.channels[ch - 6].mask, setting.SN76489Type.UseScci ? 1 : 0);
            }
            for (int ch = 0; ch < 3; ch++)
            {
                screen.drawCh(ch + 10, ref oldParam.ym2612.channels[ch + 6].mask, newParam.ym2612.channels[ch + 6].mask,tp);
            }

            if (setting.Debug_DispFrameCounter)
            {
                long v = Audio.getVirtualFrameCounter();
                if (v != -1) screen.drawFont8(0, 0, 0, string.Format("EMU        : {0:D12} ", v));
                long r = Audio.getRealFrameCounter();
                if (r != -1) screen.drawFont8(0, 8, 0, string.Format("SCCI       : {0:D12} ", r));
                long d = r - v;
                if (r != -1 && v != -1) screen.drawFont8(0, 16, 0, string.Format("SCCI - EMU : {0:D12} ", d));

            }

            screen.drawFont4(0, 208, 1, Audio.GetIsDataBlock(vgm.enmModel.VirtualModel) ? "VD" : "  ");
            screen.drawFont4(12, 208, 1, Audio.GetIsPcmRAMWrite(vgm.enmModel.VirtualModel) ? "VP" : "  ");
            screen.drawFont4(0, 216, 1, Audio.GetIsDataBlock(vgm.enmModel.RealModel) ? "RD" : "  ");
            screen.drawFont4(12, 216, 1, Audio.GetIsPcmRAMWrite(vgm.enmModel.RealModel) ? "RP" : "  ");

            screen.Refresh();

            Audio.updateVol();


        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private int searchFMNote(int freq)
        {
            int m = int.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 5; i++)
            {
                int a = Math.Abs(freq - FmFNum[i]);
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
                int a = Math.Abs(freq - PsgFNum[i]);
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
                double a = Math.Abs(freq - (0x0800 * pcmMTbl[i % 12] * Math.Pow(2, ((int)(i / 12) - 4))));
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
                double a = Math.Abs(freq - ((0x0800<<2) * pcmMTbl[i % 12] * Math.Pow(2, ((int)(i / 12) - 4))));
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
                screen.screenInit();
                //oldParam = new MDChipParams();
                newParam = new MDChipParams();

                Audio.Init(setting);
                StartMIDIInMonitoring();

                IsInitialOpenFolder = true;

            }
        }

        private void stop()
        {
            frmPlayList.Stop();
            Audio.Stop();

        }

        private void pause()
        {
            Audio.Pause();
        }

        private void fadeout()
        {
            Audio.Fadeout();
        }

        private void prev()
        {
            frmPlayList.prevPlay();
        }

        private void play()
        {
            string[] fn = null;

            frmPlayList.Stop();

            //if (srcBuf == null && frmPlayList.getMusicCount() < 1)
            if (frmPlayList.getMusicCount() < 1)
            {
                fn = fileOpen(false);
                if (fn == null) return;
                frmPlayList.AddList(fn[0]);
                fn[0] = frmPlayList.setStart(-1); //last
            }
            else
            {
                fn = new string[1] { "" };
                fn[0] = frmPlayList.setStart(-2);//first 
            }

            loadAndPlay(fn[0]);
            frmPlayList.Play();

        }

        private void playdata()
        {

            if (srcBuf == null)
            {
                return;
            }

            stop();

            for (int ch = 0; ch < 6; ch++)
            {
                newParam.ym2612.channels[ch].mask = false;
            }
            for (int ch = 0; ch < 4; ch++)
            {
                newParam.sn76489.channels[ch].mask = false;
            }
            screen.screenInit();
            //oldParam = new MDChipParams();
            newParam = new MDChipParams();

            if (!Audio.Play(setting))
            {
                MessageBox.Show("再生に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    frmPlayList.Stop();
                    Audio.Stop();
                }
                catch { }
                return;
            }

            if (frmInfo != null)
            {
                frmInfo.update();
            }
        }

        private void ff()
        {
            Audio.FF();
        }

        private void next()
        {
            frmPlayList.nextPlay();
        }

        private void nextPlayMode()
        {
            frmPlayList.nextPlayMode(newButtonMode[9]);
        }

        private void slow()
        {
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

        private void openMegaCD()
        {
            if (frmMCD != null)// && frmInfo.isClosed)
            {
                try
                {
                    screen.RemoveRf5c164();
                }
                catch { }
                try
                {
                    frmMCD.Close();
                }
                catch { }
                try
                {
                    frmMCD.Dispose();
                }
                catch { }
                frmMCD = null;
                return;
            }

            frmMCD = new frmMegaCD(this);
            if (setting.location.PRf5c164 == System.Drawing.Point.Empty)
            {
                frmMCD.x = this.Location.X;
                frmMCD.y = this.Location.Y + 264;
            }
            else
            {
                frmMCD.x = setting.location.PRf5c164.X;
                frmMCD.y = setting.location.PRf5c164.Y;
            }

            screen.AddRf5c164(frmMCD.pbScreen, Properties.Resources.planeC);
            frmMCD.Show();
            frmMCD.update();
        }

        private void showContextMenu()
        {
            cmsOpenOtherPanel.Show();
            System.Drawing.Point p = Control.MousePosition;
            cmsOpenOtherPanel.Top = p.Y;
            cmsOpenOtherPanel.Left = p.X;
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

                    loadAndPlay(filename);
                    frmPlayList.setStart(-1);

                    frmPlayList.Play();

                }
                catch
                {
                    MessageBox.Show("ファイルの読み込みに失敗しました。");
                }
            }
        }

        public byte[] getAllBytes(string filename)
        {

            if (!filename.ToLower().EndsWith(".vgz"))
            {
                return System.IO.File.ReadAllBytes(filename);
            }

            int num;
            byte[] buf = new byte[1024]; // 1Kbytesずつ処理する

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
                    stop();
                    srcBuf = getAllBytes(sParam);
                    Audio.SetVGMBuffer(srcBuf);
                    play();

                }
                catch
                {
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
            catch { str = null; }
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

        public void getInstCh(int ch)
        {
            if (!setting.other.UseGetInst) return;

            int p = (ch > 2) ? 1 : 0;
            int c = (ch > 2) ? ch - 3 : ch;
            int[][] fmRegister = Audio.GetFMRegister();

            string n = "AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSG-EG\r\n";

            for (int i = 0; i < 4; i++)
            {
                int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                n += string.Format("{0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}\r\n"
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
            n += "ALG FB\r\n";
            n += string.Format("{0:D3},{1:D3}\r\n"
                , fmRegister[p][0xb0 + c] & 0x07//AL
                , (fmRegister[p][0xb0 + c] & 0x38) >> 3//FB
            );

            Clipboard.SetText(n);
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

        public void loadAndPlay(string fn)
        {
            try
            {

                srcBuf = getAllBytes(fn);
                Audio.SetVGMBuffer(srcBuf);

                if (srcBuf != null)
                {
                    this.Invoke((Action)playdata);
                }

            }
            catch
            {
                srcBuf = null;
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }
        }

        private void tsmiC140_Click(object sender, EventArgs e)
        {
            if (frmC140 != null)// && frmInfo.isClosed)
            {
                try
                {
                    screen.RemoveC140();
                }
                catch { }
                try
                {
                    frmC140.Close();
                }
                catch { }
                try
                {
                    frmC140.Dispose();
                }
                catch { }
                frmC140 = null;
                return;
            }

            frmC140 = new frmC140(this);
            screen.AddC140(frmC140.pbScreen, Properties.Resources.planeF);

            if (setting.location.PC140 == System.Drawing.Point.Empty)
            {
                frmC140.x = this.Location.X;
                frmC140.y = this.Location.Y + 264;
            }
            else
            {
                frmC140.x = setting.location.PC140.X;
                frmC140.y = setting.location.PC140.Y;
            }

            frmC140.Show();
            frmC140.update();
        }

        private void tsmiOPNA_Click(object sender, EventArgs e)
        {
            if (frmYM2608 != null)// && frmInfo.isClosed)
            {
                try
                {
                    screen.RemoveYM2608();
                }
                catch { }
                try
                {
                    frmYM2608.Close();
                }
                catch { }
                try
                {
                    frmYM2608.Dispose();
                }
                catch { }
                frmYM2608 = null;
                return;
            }

            frmYM2608 = new frmYM2608(this);

            if (setting.location.PYm2608 == System.Drawing.Point.Empty)
            {
                frmYM2608.x = this.Location.X;
                frmYM2608.y = this.Location.Y + 264;
            }
            else
            {
                frmYM2608.x = setting.location.PYm2608.X;
                frmYM2608.y = setting.location.PYm2608.Y;
            }

            screen.AddYM2608(frmYM2608.pbScreen, Properties.Resources.planeD);
            frmYM2608.Show();
            frmYM2608.update();
        }

        private void tsmiOPM_Click(object sender, EventArgs e)
        {
            if (frmYM2151 != null)// && frmInfo.isClosed)
            {
                try
                {
                    screen.RemoveYM2151();
                }
                catch { }
                try
                {
                    frmYM2151.Close();
                }
                catch { }
                try
                {
                    frmYM2151.Dispose();
                }
                catch { }
                frmYM2151 = null;
                return;
            }

            frmYM2151 = new frmYM2151(this);

            if (setting.location.PYm2151 == System.Drawing.Point.Empty)
            {
                frmYM2151.x = this.Location.X;
                frmYM2151.y = this.Location.Y + 264;
            }
            else
            {
                frmYM2151.x = setting.location.PYm2151.X;
                frmYM2151.y = setting.location.PYm2151.Y;
            }

            screen.AddYM2151(frmYM2151.pbScreen, Properties.Resources.planeE);
            frmYM2151.Show();
            frmYM2151.update();
        }
    }
}
