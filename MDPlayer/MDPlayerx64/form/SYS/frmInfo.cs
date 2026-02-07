using MDPlayerx64;
using System.Reflection.Metadata.Ecma335;

namespace MDPlayer.form
{
    public partial class frmInfo : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        public Setting setting = null;
        public List<Tuple<int, int, string>> lyrics = null;
        public int lyricsIndex = 0;
        private Color culColor = Color.FromArgb(192, 192, 255);
        private int oldComlength = -1;

        public frmInfo(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
            rtbLyric.GotFocus += RichTextBox1_GotFocus;
        }

        private void RichTextBox1_GotFocus(object sender, EventArgs e)
        {
            dgvInfo.Focus();
        }

        public void UpdateInfo()
        {

            dgvInfo.Rows.Clear();
            rtbLyric.Clear();

            GD3 gd3 = Audio.GetGD3();
            if (gd3 == null) return;

            if (dgvInfo.Columns.Count < 2)
            {
                dgvInfo.Columns.Add("項目", "項目");
                dgvInfo.Columns.Add("内容", "内容");
            }
            dgvInfo.Rows.Add("Title", Common.EscSeqFilter(gd3.TrackName));
            dgvInfo.Rows.Add("TitleJ", Common.EscSeqFilter(gd3.TrackNameJ));
            dgvInfo.Rows.Add("Game", Common.EscSeqFilter(gd3.GameName));
            dgvInfo.Rows.Add("GameJ", Common.EscSeqFilter(gd3.GameNameJ));
            dgvInfo.Rows.Add("System", Common.EscSeqFilter(gd3.SystemName));
            dgvInfo.Rows.Add("SystemJ", Common.EscSeqFilter(gd3.SystemNameJ));
            dgvInfo.Rows.Add("Composer", Common.EscSeqFilter(gd3.Composer));
            dgvInfo.Rows.Add("ComposerJ", Common.EscSeqFilter(gd3.ComposerJ));
            dgvInfo.Rows.Add("Release", Common.EscSeqFilter(gd3.Converted));
            dgvInfo.Rows.Add("VGMBy", Common.EscSeqFilter(gd3.VGMBy));
            dgvInfo.Rows.Add("Notes", Common.EscSeqFilter(gd3.Notes));
            dgvInfo.Rows.Add("Version", Common.EscSeqFilter(gd3.Version));
            dgvInfo.Rows.Add("UsedChips", Common.EscSeqFilter(gd3.UsedChips));
            dgvInfo.ClearSelection();

            if (setting.other.ToastMode)
            {
                this.BeginInvoke(new Action(() =>
                {
                    frmToast toast = new frmToast(Common.EscSeqFilter(gd3.Composer), Common.EscSeqFilter(gd3.TrackName));
                    toast.Show();
                }));
            }

            parent.OpenPicWindow(gd3.pic);

            if (Audio.PlayingFileFormat == EnmFileFormat.MUAP || Audio.PlayingFileFormat == EnmFileFormat.MUAP_src)
            {
                timer.Enabled = true;
                return;
            }
            else if (Audio.PlayingFileFormat == EnmFileFormat.shoutcast)
            {
                timer.Enabled = true;
                return;
            }
            else
            {
                if (gd3.Lyrics == null)
                {
                    timer.Enabled = false;
                }
                else
                {
                    lyrics = gd3.Lyrics;
                    timer.Enabled = true;
                }
            }
        }

        public void ScreenInit()
        {
            lyricsIndex = 0;
            culColor = Color.FromArgb(192, 192, 255);
        }

        //protected override bool ShowWithoutActivation
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        private void FrmInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PInfo = Location;
                parent.setting.location.SInfo = Size;
            }
            else
            {
                parent.setting.location.PInfo = RestoreBounds.Location;
                parent.setting.location.SInfo = RestoreBounds.Size;
            }

            isClosed = true;
        }

        private void FrmInfo_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }

        protected override void WndProc(ref Message m)
        {
            if (parent != null)
            {
                parent.WindowsMessage(ref m);
            }

            base.WndProc(ref m);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Audio.PlayingFileFormat == EnmFileFormat.MUAP || Audio.PlayingFileFormat == EnmFileFormat.MUAP_src)
            {
                muapLyrics();
                return;
            }
            else if (Audio.PlayingFileFormat == EnmFileFormat.shoutcast)
            {
                UpdateShoutcastTitle();
                return;
            }

            if (lyrics == null || lyrics.Count < 1) return;

            long cnt = Audio.GetDriverCounter();

            try
            {
                if (cnt >= lyrics[lyricsIndex].Item1)
                {

                    //dgvInfo.Rows[13].Cells[1].Value = "";
                    rtbLyric.Clear();

                    int ind = 0;
                    //dgvInfo.Rows[13].Cells[1].Style.ForeColor = culColor;
                    rtbLyric.SelectionColor = culColor;
                    while (ind < lyrics[lyricsIndex].Item3.Length)
                    {
                        char c = lyrics[lyricsIndex].Item3[ind];
                        if (c == '\\')
                        {
                            ind++;
                            c = lyrics[lyricsIndex].Item3[ind];
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                    break;
                                case 'c':
                                    ind++;
                                    string n = lyrics[lyricsIndex].Item3[ind++].ToString();
                                    int r, g, b;
                                    if (n == "s")
                                    {
                                        r = 192;
                                        g = 192;
                                        b = 255; //192,192,255 system color
                                    }
                                    else
                                    {
                                        n += lyrics[lyricsIndex].Item3[ind++].ToString();
                                        r = Int32.Parse(n, System.Globalization.NumberStyles.HexNumber);
                                        n = lyrics[lyricsIndex].Item3[ind++].ToString();
                                        n += lyrics[lyricsIndex].Item3[ind++].ToString();
                                        g = Int32.Parse(n, System.Globalization.NumberStyles.HexNumber);
                                        n = lyrics[lyricsIndex].Item3[ind++].ToString();
                                        n += lyrics[lyricsIndex].Item3[ind++].ToString();
                                        b = Int32.Parse(n, System.Globalization.NumberStyles.HexNumber);
                                    }
                                    culColor = Color.FromArgb(r, g, b);
                                    rtbLyric.SelectionColor = culColor;
                                    continue;
                            }
                        }
                        rtbLyric.SelectedText = c.ToString();
                        ind++;
                    }

                    lyricsIndex++;

                    if (lyricsIndex == lyrics.Count)
                    {
                        timer.Enabled = false;
                    }
                }
            }
            catch
            {
                try
                {
                    rtbLyric.Clear();
                    rtbLyric.SelectedText = "LYLIC PARSE ERROR";
                }
                catch { }
            }
        }

        private void UpdateShoutcastTitle()
        {
            List<Tuple<string, string>> ret = Audio.GetTagsDriver();
            if (ret == null || ret.Count < 1 || ret[0] == null) return;
            dgvInfo.Rows[0].Cells[1].Value = ret[0].Item2;

            if (setting.other.ToastMode)
            {
                this.BeginInvoke(new Action(() =>
                {
                    frmToast toast = new frmToast("", ret[0].Item2);
                    toast.Show();
                }));
            }
        }

        private void muapLyrics()
        {
            List<Tuple<string, string>> ret = Audio.GetTagsDriver();
            if (ret == null || ret.Count < 2 || ret[0] == null) return;

            string ly = ret[0].Item2;
            int comlength = int.Parse(ret[1].Item2);
            ly = ly.Replace("\0", "");
            if (comlength == 255)
            {
                rtbLyric.ForeColor = Color.White;
                rtbLyric.Text = ly;
                oldComlength = -1;
                return;
            }

            if (oldComlength == comlength) return;

            oldComlength = comlength;
            rtbLyric.SuspendLayout();
            rtbLyric.Clear();
            if (comlength != 0)
            {
                rtbLyric.SelectionColor = Color.White;
                rtbLyric.SelectedText = ly.Substring(0, Math.Min(ly.Length, comlength));
            }
            if (ly.Length > comlength)
            {
                rtbLyric.SelectionColor = Color.Blue;
                rtbLyric.SelectedText = ly.Substring(comlength);
            }
            rtbLyric.ResumeLayout();
        }

        private void frmInfo_Shown(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void frmInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }
    }
}
