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

        public frmInfo(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
            rtbLyric.GotFocus += RichTextBox1_GotFocus;
            UpdateInfo();
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

            dgvInfo.Rows.Add("Title", gd3.TrackName);
            dgvInfo.Rows.Add("TitleJ", gd3.TrackNameJ);
            dgvInfo.Rows.Add("Game", gd3.GameName);
            dgvInfo.Rows.Add("GameJ", gd3.GameNameJ);
            dgvInfo.Rows.Add("System", gd3.SystemName);
            dgvInfo.Rows.Add("SystemJ", gd3.SystemNameJ);
            dgvInfo.Rows.Add("Composer", gd3.Composer);
            dgvInfo.Rows.Add("ComposerJ", gd3.ComposerJ);
            dgvInfo.Rows.Add("Release", gd3.Converted);
            dgvInfo.Rows.Add("VGMBy", gd3.VGMBy);
            dgvInfo.Rows.Add("Notes", gd3.Notes);
            dgvInfo.Rows.Add("Version", gd3.Version);
            dgvInfo.Rows.Add("UsedChips", gd3.UsedChips);
            dgvInfo.ClearSelection();

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
            }
            else
            {
                parent.setting.location.PInfo = RestoreBounds.Location;
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
    }
}
