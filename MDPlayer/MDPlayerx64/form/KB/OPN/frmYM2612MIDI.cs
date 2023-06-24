#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmYM2612MIDI : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int zoom = 1;

        private MDChipParams.YM2612MIDI newParam = null;
        private MDChipParams.YM2612MIDI oldParam = new MDChipParams.YM2612MIDI();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2612MIDI(frmMain frm, int zoom, MDChipParams.YM2612MIDI newParam) : base(frm)
        {
            this.zoom = zoom;

            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.frmYM2612MIDI_MouseWheel);

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeYM2612MIDI"], null, zoom);
            DrawBuff.screenInitYM2612MIDI(frameBuffer);
            update();
        }

        public void update()
        {
            frameBuffer.Refresh(null);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmYM2612MIDI_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2612MIDI = Location;
            }
            else
            {
                parent.setting.location.PosYm2612MIDI = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYM2612MIDI_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM2612MIDI"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM2612MIDI"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM2612MIDI"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM2612MIDI"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM2612MIDI"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM2612MIDI"].Height * zoom);
            frmYM2612MIDI_Resize(null, null);

        }

        private void frmYM2612MIDI_Resize(object sender, EventArgs e)
        {

        }

        public void screenChangeParams()
        {
            int[][] fmRegister = Audio.GetYM2612MIDIRegister();
            //int[] fmKey = Audio.GetFMKeyOn();

            newParam.IsMONO = parent.setting.midiKbd.IsMONO;
            if (parent.setting.midiKbd.IsMONO)
            {
                for (int i = 0; i < 6; i++)
                {
                    newParam.useChannel[i] = (parent.setting.midiKbd.UseMONOChannel == i);
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    newParam.useChannel[i] = parent.setting.midiKbd.UseChannel[i];
                }
            }

            newParam.useFormat = parent.setting.midiKbd.UseFormat;

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.channels[ch].inst[i * 11 + 0] = fmRegister[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.channels[ch].inst[i * 11 + 1] = fmRegister[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.channels[ch].inst[i * 11 + 2] = fmRegister[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.channels[ch].inst[i * 11 + 3] = fmRegister[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.channels[ch].inst[i * 11 + 4] = (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.channels[ch].inst[i * 11 + 5] = fmRegister[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.channels[ch].inst[i * 11 + 6] = (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.channels[ch].inst[i * 11 + 7] = fmRegister[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.channels[ch].inst[i * 11 + 8] = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.channels[ch].inst[i * 11 + 9] = (fmRegister[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.channels[ch].inst[i * 11 + 10] = fmRegister[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.channels[ch].inst[44] = fmRegister[p][0xb0 + c] & 0x07;//AL
                newParam.channels[ch].inst[45] = (fmRegister[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.channels[ch].inst[46] = (fmRegister[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.channels[ch].inst[47] = fmRegister[p][0xb4 + c] & 0x07;//FMS

                newParam.channels[ch].pan = (fmRegister[p][0xb4 + c] & 0xc0) >> 6;

                if (newParam.selectCh != -1 && newParam.selectParam != -1)
                {
                    if (oldParam.selectCh != -1 && oldParam.selectParam != -1)
                    {
                        newParam.channels[oldParam.selectCh].typ[oldParam.selectParam] = 0;
                    }
                    newParam.channels[newParam.selectCh].typ[newParam.selectParam] = 1;
                    oldParam.selectCh = newParam.selectCh;
                    oldParam.selectParam = newParam.selectParam;
                }

                //int freq = 0;
                //int octav = 0;
                //int n = -1;
                //freq = fmRegister[p][0xa0 + c] + (fmRegister[p][0xa4 + c] & 0x07) * 0x100;
                //octav = (fmRegister[p][0xa4 + c] & 0x38) >> 3;

                //if (fmKey[ch] > 0) n = Math.Min(Math.Max(octav * 12 + searchFMNote(freq), 0), 95);

                //newParam.channels[ch].volumeL = Math.Min(Math.Max(fmVol[ch][0] / 80, 0), 19);
                //newParam.channels[ch].volumeR = Math.Min(Math.Max(fmVol[ch][1] / 80, 0), 19);
                //newParam.channels[ch].note = n;

            }

        }


        public void screenDrawParams()
        {
            for (int c = 0; c < 6; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                bool YM2612type = parent.setting.YM2612Type[0].UseReal[0];
                int tp = YM2612type ? 1 : 0;

                DrawBuff.Inst(frameBuffer, 1, 6 + (c > 2 ? 3 : 0), c, oyc.inst, nyc.inst, oyc.typ, nyc.typ);

                int[] onl = oldParam.noteLog[c];
                int[] nnl = newParam.noteLog[c];

                for (int n = 0; n < 10; n++)
                {
                    DrawBuff.NoteLogYM2612MIDI(frameBuffer, (c % 3) * 13 * 8 + 2 * 8 + n * 8, (c / 3) * 18 * 4 + 24 * 4, ref onl[n], nnl[n]);
                }

                DrawBuff.UseChannelYM2612MIDI(frameBuffer, (c % 3) * 13 * 8, (c / 3) * 9 * 8 + 4 * 8, ref oldParam.useChannel[c], newParam.useChannel[c]);
            }

            DrawBuff.MONOPOLYYM2612MIDI(frameBuffer, ref oldParam.IsMONO, newParam.IsMONO);

            DrawBuff.LfoSw(frameBuffer, 16, 176, ref oldParam.lfoSw, newParam.lfoSw);
            DrawBuff.LfoFrq(frameBuffer, 64, 176, ref oldParam.lfoFrq, newParam.lfoFrq);
            DrawBuff.ToneFormat(frameBuffer, 16, 6, ref oldParam.useFormat, newParam.useFormat);
        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;

            //上部ラベル
            if (py < 8) return;

            if (py < 16)
            {
                //Console.WriteLine("鍵盤");
                return;
            }
            else if (py < 32)
            {
                //Console.WriteLine("各機能メニュー");
                int u = (py - 16) / 8;
                int p = -1;
                if (px >= 1 * 8 && px < 6 * 8) p = 0;
                else if (px >= 8 * 8 && px < 14 * 8) p = 1;
                else if (px >= 15 * 8 && px < 20 * 8) p = 2;
                else if (px >= 23 * 8 && px < 29 * 8) p = 3;
                else if (px >= 32 * 8 && px < 38 * 8) p = 4;

                if (p == -1) return;

                switch (u * 5 + p)
                {
                    case 0:
                        //Console.WriteLine("MONO");
                        cmdSetMode(0);
                        break;
                    case 1:
                        break;
                    case 2:
                        //Console.WriteLine("PANIC");
                        cmdAllNoteOff();
                        break;
                    case 3:
                        //Console.WriteLine("TP.PUT");
                        cmdTPPut();
                        break;
                    case 4:
                        //Console.WriteLine("T.LOAD");
                        cmdTLoad();
                        break;
                    case 5:
                        //Console.WriteLine("POLY");
                        cmdSetMode(1);
                        break;
                    case 6:
                        parent.setting.midiKbd.UseFormat++;
                        if (parent.setting.midiKbd.UseFormat > 4) parent.setting.midiKbd.UseFormat = 0;
                        break;
                    case 7:
                        //Console.WriteLine("L.CLS");
                        cmdLogClear();
                        break;
                    case 8:
                        //Console.WriteLine("TP.GET");
                        cmdTPGet();
                        break;
                    case 9:
                        //Console.WriteLine("T.SAVE");
                        cmdTSave();
                        break;
                }
            }
            else if (py < 40)
            {
                if ((px / 8) % 13 == 0)
                {
                    //Console.WriteLine("チャンネル選択");
                    cmdSelectChannel(px / 8 / 13);
                }
                else
                {
                    //Console.WriteLine("音色選択(1-3Ch)");
                    cmdSelectTone(px, py, e);// / 8 / 13, e);
                }
            }
            else if (py < 80)
            {
                //Console.WriteLine("音色選択(1-3Ch)");
                cmdSelectTone(px, py, e);
            }
            else if (py < 104)
            {
                if (py < 88 && (px / 8) % 13 == 3)
                {
                    //Console.WriteLine("ログクリア");
                    cmdLogClear(px / 8 / 13);
                }
                else
                {
                    //Console.WriteLine("ログ->MML変換(1-3Ch)");
                    cmdLog2MML(px / 8 / 13);
                }
            }
            else if (py < 112)
            {
                if ((px / 8) % 13 == 0)
                {
                    //Console.WriteLine("チャンネル選択");
                    cmdSelectChannel((px / 8 / 13) + 3);
                }
                else
                {
                    //Console.WriteLine("音色選択(4-6Ch)");
                    cmdSelectTone(px, py, e);
                }
            }
            else if (py < 152)
            {
                //Console.WriteLine("音色選択(4-6Ch)");
                cmdSelectTone(px, py, e);
            }
            else if (py < 176)
            {
                if (py < 160 && (px / 8) % 13 == 3)
                {
                    //Console.WriteLine("ログクリア");
                    cmdLogClear((px / 8 / 13) + 3);
                }
                else
                {
                    //Console.WriteLine("ログ->MML変換(4-6Ch)");
                    cmdLog2MML((px / 8 / 13) + 3);
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// MONO/POLY
        /// </summary>
        private void cmdSetMode(int m)
        {
            parent.Ym2612Midi_SetMode(m);
        }

        /// <summary>
        /// PANIC
        /// </summary>
        private void cmdAllNoteOff()
        {
            parent.Ym2612Midi_AllNoteOff();
        }

        /// <summary>
        /// L.CLS
        /// </summary>
        private void cmdLogClear()
        {
            parent.Ym2612Midi_ClearNoteLog();
        }

        /// <summary>
        /// LogClear
        /// </summary>
        private void cmdLogClear(int ch)
        {
            parent.Ym2612Midi_ClearNoteLog(ch);
        }

        /// <summary>
        /// MML変換
        /// </summary>
        private void cmdLog2MML(int ch)
        {
            parent.Ym2612Midi_Log2MML(ch);
        }

        /// <summary>
        /// 
        /// </summary>
        private void cmdSelectChannel(int ch)
        {
            parent.Ym2612Midi_SelectChannel(ch);
        }

        private void cmdTPPut()
        {
            parent.Ym2612Midi_SetTonesToSetting();
            frmTPPut frmTPPut = new frmTPPut();
            frmTPPut.ShowDialog(parent.setting, parent.tonePallet);
        }

        private void cmdTPGet()
        {
            parent.Ym2612Midi_SetTonesToSetting();
            frmTPGet frmTPGet = new frmTPGet();
            frmTPGet.ShowDialog(parent.setting, parent.tonePallet);
            parent.Ym2612Midi_SetTonesFromSetting();
        }

        private bool IsInitialOpenFolder = true;

        private void cmdTSave()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XMLファイル(*.xml)|*.xml|MML2VGMファイル(*.gwi)|*.gwi|FMP7ファイル(*.mwi)|*.mwi|NRTDRVファイル(*.mml)|*.mml|MXDRVファイル(*.mml)|*.mml|MusicLALFファイル(*.mml)|*.mml";
            sfd.Title = "TonePalletファイルを保存";
            if (parent.setting.other.DefaultDataPath != "" && Directory.Exists(parent.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                sfd.InitialDirectory = parent.setting.other.DefaultDataPath;
            }
            else
            {
                sfd.RestoreDirectory = true;
            }
            sfd.CheckPathExists = true;

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            IsInitialOpenFolder = false;

            try
            {
                parent.Ym2612Midi_SaveTonePallet(sfd.FileName, sfd.FilterIndex);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの保存に失敗しました。");
            }
        }

        private void cmdTLoad()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XMLファイル(*.xml)|*.xml|MML2VGMファイル(*.gwi)|*.gwi|FMP7ファイル(*.mwi)|*.mwi|NRTDRVファイル(*.mml)|*.mml|MXDRVファイル(*.mml)|*.mml|MusicLALFファイル(*.mml)|*.mml";
            ofd.Title = "TonePalletファイルの読込";
            if (parent.setting.other.DefaultDataPath != "" && Directory.Exists(parent.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = parent.setting.other.DefaultDataPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            IsInitialOpenFolder = false;

            try
            {
                parent.Ym2612Midi_LoadTonePallet(ofd.FileName, ofd.FilterIndex);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの読込に失敗しました。");
            }
        }

        private void cmdSelectTone(int px, int py, MouseEventArgs e)
        {
            int ch = px / 8 / 13 + (py < 104 ? 0 : 3);
            int row = -1;
            int col = 0;
            int n = -1;

            if (e.Button != MouseButtons.Right)
            {
                px %= 8 * 13;
                py = py >= 104 ? (py - 104) : (py - 32);
                col = px / 4;

                if (py < 8)
                {
                    row = 0;
                    switch (col)
                    {
                        case 10:
                        case 11:
                            n = 44;
                            break;
                        case 14:
                        case 15:
                            n = 45;
                            break;
                        case 19:
                        case 20:
                            n = 46;
                            break;
                        case 24:
                        case 25:
                            n = 47;
                            break;
                    }
                }
                else if (py < 16)
                {
                    return;
                }
                else if (py < 48)
                {
                    row = (py - 16) / 8 + 1;
                    if (col < 12)
                    {
                        n = col / 2;
                        if (n < 1) return;
                        n--;
                    }
                    else if (col < 15)
                    {
                        n = 5;
                    }
                    else if (col < 25)
                    {
                        n = (col + 1) / 2;
                        n -= 2;
                    }
                    else
                    {
                        return;
                    }
                    n += (row - 1) * 11;
                }

                //Console.WriteLine("row={0} col={1} ch={2} n={3}", row, col, ch, n);
                parent.Ym2612Midi_SetSelectInstParam(ch, n);
                return;
            }

            cmsMIDIKBD.Tag = ch;
            cmsMIDIKBD.Show(this, e.X, e.Y);
        }

        private void frmYM2612MIDI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    parent.Ym2612Midi_CopyToneToClipboard();
                }
                else if (e.KeyCode == Keys.V)
                {
                    parent.Ym2612Midi_PasteToneFromClipboard();
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                parent.Ym2612Midi_AddSelectInstParam(1);
            }
            else if (e.KeyCode == Keys.Space)
            {
                parent.Ym2612Midi_AddSelectInstParam(11);
            }
        }

        private void ctsmiCopy_Click(object sender, EventArgs e)
        {
            parent.Ym2612Midi_CopyToneToClipboard((int)cmsMIDIKBD.Tag);
        }

        private void ctsmiPaste_Click(object sender, EventArgs e)
        {
            parent.Ym2612Midi_PasteToneFromClipboard((int)cmsMIDIKBD.Tag);
        }

        private void frmYM2612MIDI_MouseWheel(object sender, MouseEventArgs e)
        {
            parent.Ym2612Midi_ChangeSelectedParamValue(Math.Sign(e.Delta));
        }

    }
}