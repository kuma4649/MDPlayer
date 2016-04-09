using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer
{
    public partial class frmMain : Form
    {
        private DoubleBuffer screen;
        private int pWidth = 0;
        private int pHeight = 0;

        private frmInfo frmInfo = null;

        private MDChipParams oldParam = new MDChipParams();
        private MDChipParams newParam = new MDChipParams();

        private int[] oldButton = new int[6];
        private int[] newButton = new int[6];

        private bool isStandup = false;
        private int[] oldButtonY = new int[6];
        private int[] newButtonY = new int[6];

        private bool isRunning = false;
        private bool stopped = false;

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


        private static int SamplingRate = 44100;
        private byte[] srcBuf;


        public frmMain()
        {
            InitializeComponent();
            pbScreen.AllowDrop = true;
            Audio.Init();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // DoubleBufferオブジェクトの作成
            screen = new DoubleBuffer(pbScreen, Properties.Resources.plane, Properties.Resources.font);
            pWidth = pbScreen.Width;
            pHeight = pbScreen.Height;


        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            // リサイズ時は再確保

            if (screen != null) screen.Dispose();

            screen = new DoubleBuffer(pbScreen, Properties.Resources.plane, Properties.Resources.font);
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
            Audio.Stop();
            isRunning = false;
            while (!stopped)
            {
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }
            // 解放
            screen.Dispose();
        }

        private void pbScreen_MouseMove(object sender, MouseEventArgs e)
        {
            isStandup = true;
            if (e.Location.Y < 208)
            {
                newButton[0] = 0;
                newButton[1] = 0;
                newButton[2] = 0;
                newButton[3] = 0;
                newButton[4] = 0;
                return;
            }

            if (e.Location.X >= 320 - 6 * 16 && e.Location.X < 320 - 5 * 16) newButton[0] = 1;
            else newButton[0] = 0;

            if (e.Location.X >= 320 - 5 * 16 && e.Location.X < 320 - 4 * 16) newButton[1] = 1;
            else newButton[1] = 0;

            if (e.Location.X >= 320 - 4 * 16 && e.Location.X < 320 - 3 * 16) newButton[2] = 1;
            else newButton[2] = 0;

            if (e.Location.X >= 320 - 3 * 16 && e.Location.X < 320 - 2 * 16) newButton[3] = 1;
            else newButton[3] = 0;

            if (e.Location.X >= 320 - 2 * 16 && e.Location.X < 320 - 1 * 16) newButton[4] = 1;
            else newButton[4] = 0;

        }

        private void pbScreen_MouseEnter(object sender, EventArgs e)
        {
            isStandup = true;
        }

        private void pbScreen_MouseLeave(object sender, EventArgs e)
        {
            newButton[0] = 0;
            newButton[1] = 0;
            newButton[2] = 0;
            newButton[3] = 0;
            newButton[4] = 0;
            isStandup = false;
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Location.Y < 208)
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

            if (e.Location.X >= 320 - 6 * 16 && e.Location.X < 320 - 5 * 16)
            {
                stop();
                return;
            }

            if (e.Location.X >= 320 - 5 * 16 && e.Location.X < 320 - 4 * 16)
            {
                pause();
                return;
            }

            if (e.Location.X >= 320 - 4 * 16 && e.Location.X < 320 - 3 * 16)
            {
                play();
                return;
            }

            if (e.Location.X >= 320 - 3 * 16 && e.Location.X < 320 - 2 * 16)
            {
                ff();
                return;
            }

            if (e.Location.X >= 320 - 2 * 16 && e.Location.X < 320 - 1 * 16)
            {
                open();
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

                if ((double)System.Environment.TickCount >= nextFrame + period)
                {
                    nextFrame += period;
                    continue;
                }

                screenDrawParams();

                nextFrame += period;
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
                    if ((fmKey[2] & (0x20 << (ch-6)) ) > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);
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

                newParam.sn76489.channels[ch].volume = Math.Min(Math.Max((psgVol[ch][0] + psgVol[ch][1]) / 2 / 100, 0), 19);
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

            if (isStandup)
            {
                for (int c = 0; c < 5; c++)
                {
                    newButtonY[c]--;
                    newButtonY[c] = Math.Max(newButtonY[c], 0);
                }
            }
            else
            {
                for (int c = 0; c < 5; c++)
                {
                    newButtonY[c]++;
                    newButtonY[c] = Math.Min(newButtonY[c], 16);
                }
            }
        }

        private void screenDrawParams()
        {
            // 描画
            screen.drawParams(oldParam, newParam);

            screen.drawButtons(oldButtonY,newButtonY,oldButton, newButton);

            screen.drawTimer(0, ref oldParam.Cminutes, ref oldParam.Csecond, ref oldParam.Cmillisecond, newParam.Cminutes, newParam.Csecond, newParam.Cmillisecond);
            screen.drawTimer(1, ref oldParam.TCminutes, ref oldParam.TCsecond, ref oldParam.TCmillisecond, newParam.TCminutes, newParam.TCsecond, newParam.TCmillisecond);
            screen.drawTimer(2, ref oldParam.LCminutes, ref oldParam.LCsecond, ref oldParam.LCmillisecond, newParam.LCminutes, newParam.LCsecond, newParam.LCmillisecond);

            screen.drawCh6(ref oldParam.ym2612.channels[5].pcmMode, newParam.ym2612.channels[5].pcmMode, ref oldParam.ym2612.channels[5].mask, newParam.ym2612.channels[5].mask);
            for (int ch = 0; ch < 5; ch++)
            {
                screen.drawCh(ch, ref oldParam.ym2612.channels[ch].mask, newParam.ym2612.channels[ch].mask);
            }
            for (int ch = 6; ch < 10; ch++)
            {
                screen.drawCh(ch, ref oldParam.sn76489.channels[ch - 6].mask, newParam.sn76489.channels[ch - 6].mask);
            }
            for (int ch = 0; ch < 3; ch++)
            {
                screen.drawCh(ch+10, ref oldParam.ym2612.channels[ch+6].mask, newParam.ym2612.channels[ch+6].mask);
            }

            screen.Refresh();

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


        private void stop()
        {

            Audio.Stop();

        }

        private void pause()
        {
            Audio.Pause();
        }

        private void play()
        {
            stop();

            for (int ch = 0; ch < 6; ch++)
            {
                newParam.ym2612.channels[ch].mask = false;
            }
            for (int ch = 0; ch < 4; ch++)
            {
                newParam.sn76489.channels[ch].mask = false;
            }
            if (!Audio.Play())
            {
                MessageBox.Show("再生に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Audio.GD3.TrackNameJ != "")
                lblTitle.Text = Audio.GD3.TrackNameJ;
            else
                lblTitle.Text = Audio.GD3.TrackName;

            if (frmInfo != null && frmInfo.isClosed)
            {
                frmInfo.Dispose();
                frmInfo = null;
            }
            if (frmInfo == null)
            {
                frmInfo = new frmInfo();
                frmInfo.x = this.Location.X+328;
                frmInfo.y = this.Location.Y;
                frmInfo.Show();
            }
            frmInfo.update();
        }

        private void ff()
        {
            Audio.FF();
        }

        private void open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "VGMファイル(*.vgm;*.vgz)|*.vgm;*.vgz";
            ofd.Title = "ファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {

                srcBuf = getAllBytes(ofd.FileName);
                Audio.SetVGMBuffer(srcBuf);
                play();

            }
            catch
            {
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }
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

                    srcBuf = getAllBytes(filename);
                    Audio.SetVGMBuffer(srcBuf);
                    play();

                }
                catch
                {
                    MessageBox.Show("ファイルの読み込みに失敗しました。");
                }
            }
        }

        private byte[] getAllBytes(string filename)
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

    }
}
