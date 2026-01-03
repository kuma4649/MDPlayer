#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
using System.Diagnostics;
using System.Security.Cryptography;
using static MDPlayer.PlayList;
//using static System.Net.Mime.MediaTypeNames;

namespace MDPlayer.form
{
    public partial class frmPlayList : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public Setting setting = null;

        public string playFilename = "";
        public string playArcFilename = "";
        public EnmFileFormat playFormat = EnmFileFormat.unknown;
        public EnmArcType playArcType = EnmArcType.unknown;
        public int playSongNum = -1;

        private List<string> plList = null;
        private PlayList playList = null;
        private frmMain frmMain = null;

        private bool playing = false;
        private int playIndex = -1;
        public int oldPlayIndex = -1;

        private Random rand = new System.Random();
        private bool IsInitialOpenFolder = true;

        private string[] sext = ".vgm;.vgz;.zip;.lzh;.zdf;.nrd;.ndp;.xgm;.xgz;.zgm;.s98;.nsf;.gbs;.hes;.sid;.ay;.mnd;.mgs;.bgm;.msd;.mdr;.mdx;.mub;.muc;.m;.m2;.mz;.mml;.mpi;.mvi;.mzi;.opi;.ovi;.ozi;.mid;.zms;.zmd;.mus;.o;.ox;.oy;.rcp;.rcs;.wav;.mp3;.aiff;.ogg;.m4a;.aac;.wma;.flac;.m3u".Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        public frmPlayList(frmMain frm)
        {
            frmMain = frm;
            setting = frm.setting;
            InitializeComponent();

            plList = setting.playList.plList == null ? null : setting.playList.plList.ToList();
            setting.playList.currentPlayList = (plList == null || plList.Count < 1) ? 0 : Math.Min(Math.Max(setting.playList.currentPlayList, 0), plList.Count);
            string currentPlayList = (plList == null || plList.Count < 1 || setting.playList.currentPlayList == 0)
                ? null
                : plList[Math.Max(setting.playList.currentPlayList - 1, 0)];

            playList = null;
            if (currentPlayList == null)
            {
                playList = PlayList.Load(null);
            }
            else if (Path.GetExtension(currentPlayList).ToLower() == ".m3u")
            {
                playList = PlayList.LoadM3U(currentPlayList);
            }
            else
            {
                playList = PlayList.Load(currentPlayList);
            }
            playList.SetDGV(dgvList);
            playIndex = -1;

            oldPlayIndex = -1;
        }

        private void MakePlList(int currentPlayList)
        {
            dgvPlayList.Rows.Clear();

            DataGridViewRow row = new();
            row.CreateCells(dgvPlayList);
            row.Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value = null;
            row.Cells[dgvPlayList.Columns["clmPL_Title"].Index].Value = "(default)";
            dgvPlayList.Rows.Add(row);

            if (plList != null && plList.Count > 0)
            {
                foreach (string s in plList)
                {
                    row = new();
                    row.CreateCells(dgvPlayList);
                    row.Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value = s;
                    row.Cells[dgvPlayList.Columns["clmPL_Title"].Index].Value = Path.GetFileName(s);
                    dgvPlayList.Rows.Add(row);
                }
            }

            for (int i = 0; i < dgvPlayList.SelectedRows.Count; i++)
            {
                dgvPlayList.Rows[dgvPlayList.SelectedRows[i].Index].Selected = false;
            }
            dgvPlayList.Rows[currentPlayList].Selected = true;
        }

        public bool isPlaying()
        {
            return playing;
        }

        public int getMusicCount()
        {
            return playList.LstMusic.Count;
        }

        public PlayList getPlayList()
        {
            return playList;
        }

        public Tuple<int, int, string, string, string[], string> setStart(int n)
        {
            updatePlayingIndex(n);

            string fn = playList.LstMusic[playIndex].fileName;
            string zfn = playList.LstMusic[playIndex].arcFileName;
            int m = 0;
            int songNo = playList.LstMusic[playIndex].songNo;

            if (playList.LstMusic[playIndex].type != null && playList.LstMusic[playIndex].type != "-")
            {
                m = playList.LstMusic[playIndex].type[0] - 'A';
                if (m < 0 || m > 9) m = 0;
            }
            string[] spFn = null;
            if (playList.LstMusic[playIndex].supportFileName != null)
            {
                spFn = playList.LstMusic[playIndex].supportFileName.Split(';');
            }
            string useCom = null;
            if (playList.LstMusic[playIndex].useCompiler != null)
            {
                useCom = playList.LstMusic[playIndex].useCompiler;
            }

            return new Tuple<int, int, string, string, string[], string>(m, songNo, fn, zfn, spFn, useCom);
        }

        public void Play()
        {
            playing = true;
        }

        public void Stop()
        {
            //updatePlayingIndex(-1);
            //playIndex = -1;

            playing = false;
        }

        public void Save()
        {
            if (setting.other.EmptyPlayList)
            {
                playList.LstMusic = new List<PlayList.Music>();
            }
            playList.Save(null);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        public List<Tuple<string, string>> randomStack = new List<Tuple<string, string>>();

        protected override void WndProc(ref Message m)
        {
            if (frmMain != null)
            {
                frmMain.WindowsMessage(ref m);
            }

            base.WndProc(ref m);
        }

        private void frmPlayList_Shown(object sender, EventArgs e)
        {
            tsbJapanese.Checked = setting.playList.isJP;
            ChangeLang(setting.playList.isJP);

            SetDisplayIndex(setting.playList.clmInfo);
            if (setting.playList.cwExt != -1) dgvList.Columns["clmExt"].Width = setting.playList.cwExt;
            if (setting.playList.cwType != -1) dgvList.Columns["clmType"].Width = setting.playList.cwType;
            if (setting.playList.cwTitle != -1) dgvList.Columns["clmTitle"].Width = setting.playList.cwTitle;
            if (setting.playList.cwTitleJ != -1) dgvList.Columns["clmTitleJ"].Width = setting.playList.cwTitleJ;
            if (setting.playList.cwFilename != -1) dgvList.Columns["clmDispFileName"].Width = setting.playList.cwFilename;
            if (setting.playList.cwSupportFilename != -1) dgvList.Columns["clmDispSupportFileName"].Width = setting.playList.cwSupportFilename;
            if (setting.playList.cwGame != -1) dgvList.Columns["clmGame"].Width = setting.playList.cwGame;
            if (setting.playList.cwGameJ != -1) dgvList.Columns["clmGameJ"].Width = setting.playList.cwGameJ;
            if (setting.playList.cwComposer != -1) dgvList.Columns["clmComposer"].Width = setting.playList.cwComposer;
            if (setting.playList.cwComposerJ != -1) dgvList.Columns["clmComposerJ"].Width = setting.playList.cwComposerJ;
            if (setting.playList.cwRelease != -1) dgvList.Columns["clmConverted"].Width = setting.playList.cwRelease;
            if (setting.playList.cwNotes != -1) dgvList.Columns["clmNotes"].Width = setting.playList.cwNotes;
            if (setting.playList.cwDuration != -1) dgvList.Columns["clmDuration"].Width = setting.playList.cwDuration;
            if (setting.playList.cwVersion != -1) dgvList.Columns["ClmVersion"].Width = setting.playList.cwVersion;
            if (setting.playList.cwUseChips != -1) dgvList.Columns["ClmUseChips"].Width = setting.playList.cwUseChips;

            if (setting.playList.cwPL_FileName != -1) dgvPlayList.Columns["clmPL_FileName"].Width = setting.playList.cwPL_FileName;
            if (setting.playList.cwPL_Title != -1) dgvPlayList.Columns["clmPL_Title"].Width = setting.playList.cwPL_Title;

            splitContainer1.SplitterDistance = setting.playList.splitterDistance;

            MakePlList(setting.playList.currentPlayList);

        }

        private void frmPlayList_Load(object sender, EventArgs e)
        {
            if (setting.location.PPlayList != System.Drawing.Point.Empty)
                this.Location = setting.location.PPlayList;
            if (setting.location.PPlayListWH != System.Drawing.Point.Empty)
                this.Size = new Size(setting.location.PPlayListWH);
        }

        private void frmPlayList_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosed = true;
            if (WindowState == FormWindowState.Normal)
            {
                setting.location.PPlayList = Location;
                setting.location.PPlayListWH = new Point(Size.Width, Size.Height);
            }
            else
            {
                setting.location.PPlayList = RestoreBounds.Location;
                setting.location.PPlayListWH = new Point(RestoreBounds.Size.Width, RestoreBounds.Size.Height);
            }

            setting.playList.clmInfo = getDisplayIndex();
            setting.playList.isJP = tsbJapanese.Checked;
            setting.playList.cwExt = dgvList.Columns["clmExt"].Width;
            setting.playList.cwType = dgvList.Columns["clmType"].Width;
            setting.playList.cwTitle = dgvList.Columns["clmTitle"].Width;
            setting.playList.cwTitleJ = dgvList.Columns["clmTitleJ"].Width;
            setting.playList.cwFilename = dgvList.Columns["clmDispFileName"].Width;
            setting.playList.cwSupportFilename = dgvList.Columns["clmDispSupportFileName"].Width;
            setting.playList.cwGame = dgvList.Columns["clmGame"].Width;
            setting.playList.cwGameJ = dgvList.Columns["clmGameJ"].Width;
            setting.playList.cwComposer = dgvList.Columns["clmComposer"].Width;
            setting.playList.cwComposerJ = dgvList.Columns["clmComposerJ"].Width;
            setting.playList.cwRelease = dgvList.Columns["clmConverted"].Width;
            setting.playList.cwNotes = dgvList.Columns["clmNotes"].Width;
            setting.playList.cwDuration = dgvList.Columns["clmDuration"].Width;
            setting.playList.cwVersion = dgvList.Columns["ClmVersion"].Width;
            setting.playList.cwUseChips = dgvList.Columns["ClmUseChips"].Width;

            setting.playList.cwPL_FileName = dgvPlayList.Columns["clmPL_FileName"].Width;
            setting.playList.cwPL_Title = dgvPlayList.Columns["clmPL_Title"].Width;

            setting.playList.splitterDistance = splitContainer1.SplitterDistance;

            if (plList == null) plList = new List<string>();

            plList.Clear();
            foreach (DataGridViewRow row in dgvPlayList.Rows)
            {
                if (row.Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value == null) continue;
                plList.Add(row.Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value.ToString());
            }

            setting.playList.plList = (plList == null || plList.Count < 1) ? null : plList.ToArray();
            setting.playList.currentPlayList = 0;
            if (dgvPlayList.SelectedRows != null && dgvPlayList.SelectedRows.Count > 0)
            {
                setting.playList.currentPlayList = dgvPlayList.SelectedRows[0].Index;
            }

            this.Visible = false;
            e.Cancel = true;
        }

        private dgvColumnInfo[] getDisplayIndex()
        {
            List<dgvColumnInfo> ret = new List<dgvColumnInfo>();

            DataGridView dgv = dgvList;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                dgvColumnInfo info = (dgvColumnInfo)dgv.Columns[i].Tag;
                if (info == null)
                {
                    info = new dgvColumnInfo();
                }

                info.columnName = dgv.Columns[i].Name;
                info.displayIndex = dgv.Columns[i].DisplayIndex;
                info.size = dgv.Columns[i].Width;
                info.visible = dgv.Columns[i].Visible;

                ret.Add(info);
            }

            return ret.ToArray();
        }

        private void SetDisplayIndex(dgvColumnInfo[] aryIndex)
        {
            if (aryIndex == null) return;

            DataGridView dgv = dgvList;

            foreach (dgvColumnInfo info in aryIndex)
            {
                if (info == null) continue;
                if (!dgv.Columns.Contains(info.columnName)) continue;
                dgv.Columns[info.columnName].DisplayIndex = info.displayIndex;
                dgv.Columns[info.columnName].Visible = info.visible;
            }

        }

        public new void Refresh()
        {
            dgvList.Rows.Clear();
            List<DataGridViewRow> rows = playList.MakeRow(playList.LstMusic);
            foreach (DataGridViewRow row in rows)
            {
                dgvList.Rows.Add(row);
            }
        }

        public void updatePlayingIndex(int newPlayingIndex)
        {
            if (oldPlayIndex != -1)
            {
                ResetColor(oldPlayIndex);
            }

            if (newPlayingIndex >= 0 && newPlayingIndex < dgvList.Rows.Count)
            {
                SetColor(newPlayingIndex);
            }
            else if (newPlayingIndex == -1)
            {
                newPlayingIndex = dgvList.Rows.Count - 1;
                SetColor(newPlayingIndex);
            }
            else if (newPlayingIndex == -2)
            {
                newPlayingIndex = 0;
                SetColor(newPlayingIndex);
            }
            playIndex = newPlayingIndex;
            oldPlayIndex = newPlayingIndex;
        }

        private void SetColor(int rowIndex)
        {
            dgvList.Rows[rowIndex].Cells[dgvList.Columns["clmPlayingNow"].Index].Value = ">";
            for (int i = 0; i < dgvList.Rows[rowIndex].Cells.Count; i++)
            {
                dgvList.Rows[rowIndex].Cells[i].Style.ForeColor = Color.LightGreen;
                dgvList.Rows[rowIndex].Cells[i].Style.SelectionForeColor = Color.LightGreen;
            }
        }

        private static Color clrLightBlue = Color.FromArgb(255, 192, 192, 255);

        private void ResetColor(int rowIndex)
        {
            dgvList.Rows[rowIndex].Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
            for (int i = 0; i < dgvList.Rows[rowIndex].Cells.Count; i++)
            {
                dgvList.Rows[rowIndex].Cells[i].Style.ForeColor = clrLightBlue;
                dgvList.Rows[rowIndex].Cells[i].Style.SelectionForeColor = Color.White;
            }
        }

        private void tsmiDelThis_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count < 1) return;

            List<int> sel = new List<int>();
            foreach (DataGridViewRow r in dgvList.SelectedRows)
            {
                sel.Add(r.Index);
            }
            sel.Sort();

            for (int i = sel.Count - 1; i >= 0; i--)
            {
                if (oldPlayIndex >= dgvList.SelectedRows[i].Index)
                {
                    oldPlayIndex--;
                }
                if (playIndex >= dgvList.SelectedRows[i].Index)
                {
                    playIndex--;
                }
                playList.LstMusic.RemoveAt(dgvList.SelectedRows[i].Index);
                dgvList.Rows.RemoveAt(dgvList.SelectedRows[i].Index);
            }
        }

        public void nextPlay()
        {
            if (!playing) return;
            if (dgvList.Rows.Count == playIndex + 1) return;

            int pi = playIndex;
            playing = false;

            pi++;

            string fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
            string zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
            int m = 0;
            int songNo = 0;
            try
            {
                songNo = (Int32)dgvList.Rows[pi].Cells["clmSongNo"].Value;
            }
            catch
            {
                songNo = 0;
            }
            if (dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value != null && dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value.ToString() != "-")
            {
                m = dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value.ToString()[0] - 'A';
                if (m < 0 || m > 9) m = 0;
            }
            string[] spFn = null;
            if (dgvList.Rows[pi].Cells["clmSupportFile"].Value != null)
            {
                spFn = dgvList.Rows[pi].Cells["clmSupportFile"].Value.ToString().Split(';');
            }
            string useCom = null;
            if (dgvList.Rows[pi].Cells["clmUseCompiler"].Value != null)
            {
                useCom = dgvList.Rows[pi].Cells["clmUseCompiler"].Value.ToString();
            }

            GD3 gd3 = getGD3FromPl(pi);
            frmMain.loadAndPlay(m, songNo, fn, zfn, spFn, useCom,gd3);
            if (Audio.ErrMsg != "")
            {
                playing = false;
                return;
            }
            updatePlayingIndex(pi);
            playing = true;

            playFilename = fn;
            playArcFilename = zfn;
            playSongNum = songNo;
            //playFormat = dgvList.Rows[pi].Cells["clmSongNo"].Value;
            //playArcType = dgvList.Rows[pi].Cells["clmSongNo"].Value;
        }

        public void nextPlayMode(int mode)
        {
            if (!playing)
            {
                playIndex = -1;
            }

            int pi = playIndex;
            playing = false;
            string fn, zfn;

            switch (mode)
            {
                case 0:// 通常
                    if (dgvList.Rows.Count <= playIndex + 1) return;
                    pi++;
                    break;
                case 1:// ランダム

                    if (pi != -1)
                    {
                        //再生履歴の更新
                        fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
                        zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;

                        randomStack.Add(new Tuple<string, string>(fn, zfn));
                        while (randomStack.Count > 1000)
                            randomStack.RemoveAt(0);
                    }

                    pi = rand.Next(dgvList.Rows.Count);
                    break;
                case 2:// 全曲ループ
                    pi++;
                    if (pi >= dgvList.Rows.Count)
                    {
                        pi = 0;
                    }
                    break;
                case 3:// １曲ループ
                    break;
            }

            if (pi + 1 > dgvList.Rows.Count)
            {
                playing = false;
                return;
            }

            fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
            zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
            int m = 0;
            if (dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value != null && dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value.ToString() != "-")
            {
                m = dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value.ToString()[0] - 'A';
                if (m < 0 || m > 9) m = 0;
            }
            int songNo = 0;
            try
            {
                songNo = (Int32)dgvList.Rows[pi].Cells["clmSongNo"].Value;
            }
            catch
            {
                songNo = 0;
            }
            string[] spFn = null;
            if (dgvList.Rows[pi].Cells["clmSupportFile"].Value != null)
            {
                spFn = dgvList.Rows[pi].Cells["clmSupportFile"].Value.ToString().Split(';');
            }
            string useCom = null;
            if (dgvList.Rows[pi].Cells["clmUseCompiler"].Value != null)
            {
                useCom = dgvList.Rows[pi].Cells["clmUseCompiler"].Value.ToString();
            }
            GD3 gd3 = getGD3FromPl(pi);
            if (!frmMain.loadAndPlay(m, songNo, fn, zfn, spFn, useCom,gd3))
            {
                playing = false;
                return;
            }

            updatePlayingIndex(pi);
            playing = true;
        }

        public void prevPlay(int mode)
        {
            if (!playing) return;
            if (mode != 1 && playIndex < 1) return;

            int pi = playIndex;
            playing = false;
            string fn, zfn;

            if (mode != 1)
            {
                pi--;
                fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
                zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
            }
            else
            {
                pi = 0;
                if (randomStack.Count > 0)
                {
                    while (true)
                    {
                        string hfn = randomStack[randomStack.Count - 1].Item1;
                        string hzfn = randomStack[randomStack.Count - 1].Item2;
                        randomStack.RemoveAt(randomStack.Count - 1);

                        for (; pi < dgvList.Rows.Count; pi++)
                        {
                            fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
                            zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
                            if (hfn == fn && hzfn == zfn)
                            {
                                goto loopEx;
                            }
                        }

                        if (randomStack.Count == 0) break;
                    }

                    if (playIndex < 1) return;
                    pi = playIndex - 1;
                    fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
                    zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
                }
                else
                {
                    pi = playIndex;
                    pi--;
                    if (pi < 0) pi = 0;
                    fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
                    zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
                }
            loopEx:;
            }

            int m = 0;
            if (dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value != null && dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value.ToString() != "-")
            {
                m = dgvList.Rows[pi].Cells[dgvList.Columns["clmType"].Index].Value.ToString()[0] - 'A';
                if (m < 0 || m > 9) m = 0;
            }
            int songNo = 0;
            try
            {
                songNo = (Int32)dgvList.Rows[pi].Cells["clmSongNo"].Value;
            }
            catch
            {
                songNo = 0;
            }
            string[] spFn = null;
            if (dgvList.Rows[pi].Cells["clmSupportFile"].Value != null)
            {
                spFn = dgvList.Rows[pi].Cells["clmSupportFile"].Value.ToString().Split(';');
            }
            string useCom = null;
            if (dgvList.Rows[pi].Cells["clmUseCompiler"].Value != null)
            {
                useCom = dgvList.Rows[pi].Cells["clmUseCompiler"].Value.ToString();
            }
            GD3 gd3 = getGD3FromPl(pi);
            frmMain.loadAndPlay(m, songNo, fn, zfn, spFn, useCom,gd3);
            updatePlayingIndex(pi);
            playing = true;
        }

        private void dgvList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                SetupHeaderSwitch(e.ColumnIndex);
                cmsHeaderSwitch.Show();
                Point p = Control.MousePosition;
                cmsHeaderSwitch.Top = p.Y;
                cmsHeaderSwitch.Left = p.X;
                return;
            }

            dgvList.Rows[e.RowIndex].Selected = true;

            if (e.Button == MouseButtons.Right)
            {
                tsmiSelectSupportFile.Enabled = false;
                tsmiRemoveSupportFile.Enabled = false;
                tsmiSelectCompiler.Enabled = false;
                if (dgvList.SelectedRows.Count > 1)
                {
                    tsmiDelThis.Text = "選択した曲を除去";
                }
                else
                {
                    tsmiDelThis.Text = "この曲を除去";
                }

                bool fnd = false;
                bool fndUseCom = false;
                foreach (DataGridViewRow row in dgvList.SelectedRows)
                {
                    string ext = row.Cells["clmExt"].Value.ToString().ToUpper();
                    if (ext != ".ZMS" && ext != ".ZMD" && ext != ".RCS")
                    {
                        fnd = true;
                    }
                    if (ext != ".ZMS")
                    {
                        fndUseCom = true;
                    }
                }
                if (!fnd)
                {
                    tsmiSelectSupportFile.Enabled = true;
                    tsmiRemoveSupportFile.Enabled = true;
                }
                if (!fndUseCom)
                {
                    tsmiSelectCompiler.Enabled = true;
                }

                cmsPlayList.Show();
                Point p = Control.MousePosition;
                cmsPlayList.Top = p.Y;
                cmsPlayList.Left = p.X;
            }
        }

        private void SetupHeaderSwitch(int col)
        {
            cmsHeaderSwitch.Items.Clear();
            ToolStripMenuItem tsi;

            if (dgvList.Columns[col].Name != "clmSpacer")
            {
                tsi = new ToolStripMenuItem("Hide this item");
                tsi.Click += header_Click;
                tsi.Tag = dgvList.Columns[col];
                cmsHeaderSwitch.Items.Add(tsi);
            }

            tsi = new ToolStripMenuItem("Show all");
            tsi.Click += showAll_Click;
            cmsHeaderSwitch.Items.Add(tsi);
        }

        private void header_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)sender;
            DataGridViewColumn dgvc = (DataGridViewColumn)tsi.Tag;
            dgvc.Visible = false;
        }

        private void showAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn col in dgvList.Columns)
            {
                if (col.Name == "clmSpacer") { col.Visible = true; continue; }
                if (col.Name == "clmKey"
                    || col.Name == "clmSongNo"
                    || col.Name == "clmZipFileName"
                    || col.Name == "clmFileName"
                    || col.Name == "clmSupportFile"
                    || col.Name == "clmUseCompiler"
                    ) continue;
                col.Visible = true;
            }
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            dgvList.Enabled = false;

            playing = false;

            try
            {
                string fn = (string)dgvList.Rows[e.RowIndex].Cells["clmFileName"].Value;
                string zfn = (string)dgvList.Rows[e.RowIndex].Cells["clmZipFileName"].Value;
                int m = 0;
                int songNo = 0;
                try
                {
                    songNo = (Int32)dgvList.Rows[e.RowIndex].Cells["clmSongNo"].Value;
                }
                catch
                {
                    songNo = 0;
                }
                if (dgvList.Rows[e.RowIndex].Cells[dgvList.Columns["clmType"].Index].Value != null && dgvList.Rows[e.RowIndex].Cells[dgvList.Columns["clmType"].Index].Value.ToString() != "-")
                {
                    m = dgvList.Rows[e.RowIndex].Cells[dgvList.Columns["clmType"].Index].Value.ToString()[0] - 'A';
                    if (m < 0 || m > 9) m = 0;
                }
                string[] spFn = null;
                if (dgvList.Rows[e.RowIndex].Cells["clmSupportFile"].Value != null)
                {
                    spFn = dgvList.Rows[e.RowIndex].Cells["clmSupportFile"].Value.ToString().Split(';');
                }
                string useCom = null;
                if (dgvList.Rows[e.RowIndex].Cells["clmUseCompiler"].Value != null)
                {
                    useCom = dgvList.Rows[e.RowIndex].Cells["clmUseCompiler"].Value.ToString();
                }

                GD3 gd3= getGD3FromPl(e.RowIndex);
                if (!frmMain.loadAndPlay(m, songNo, fn, zfn, spFn, useCom, gd3)) return;
                updatePlayingIndex(e.RowIndex);

                playing = true;
            }
            finally
            {
                //dgvList.MultiSelect = true;
                dgvList.Enabled = true;
                dgvList.Rows[e.RowIndex].Selected = true;
            }
        }

        private GD3 getGD3FromPl(int row)
        {
            if (dgvList == null || dgvList.Rows.Count <= 0) return null;
            if (dgvList.Rows.Count <= row) return null;

            DataGridViewRow r = dgvList.Rows[row];
            GD3 gd3 = new GD3();
            gd3.Composer = r.Cells["clmComposer"].Value == null ? "" : r.Cells["clmComposer"].Value.ToString();
            gd3.ComposerJ = r.Cells["clmComposerJ"].Value == null ? "" : r.Cells["clmComposerJ"].Value.ToString();
            gd3.Converted = r.Cells["clmConverted"].Value == null ? "" : r.Cells["clmConverted"].Value.ToString();
            gd3.GameName = r.Cells["clmGame"].Value == null ? "" : r.Cells["clmGame"].Value.ToString();
            gd3.GameNameJ = r.Cells["clmGameJ"].Value == null ? "" : r.Cells["clmGameJ"].Value.ToString();
            gd3.Lyrics = null;
            gd3.Notes = r.Cells["clmNotes"].Value == null ? "" : r.Cells["clmNotes"].Value.ToString();
            gd3.pic = null;
            gd3.SystemName = r.Cells["clmTitle"].Value == null ? "" : r.Cells["clmTitle"].Value.ToString();
            gd3.SystemNameJ = r.Cells["clmTitleJ"].Value == null ? "" : r.Cells["clmTitleJ"].Value.ToString();
            gd3.TrackName = r.Cells["clmTitle"].Value == null ? "" : r.Cells["clmTitle"].Value.ToString();
            gd3.TrackNameJ = r.Cells["clmTitleJ"].Value == null ? "" : r.Cells["clmTitleJ"].Value.ToString();
            gd3.UsedChips = "";
            gd3.Version = "";
            gd3.VGMBy = "";

            return gd3;
        }

        private void dgvPlayList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgvPlayList.Rows[e.RowIndex].Selected = true;

            //if (e.Button == MouseButtons.Right)
            //{
            //    cmsPLList.Show();
            //    Point p = Control.MousePosition;
            //    cmsPLList.Top = p.Y;
            //    cmsPLList.Left = p.X;
            //}
        }
        private void dgvPlayList_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo inf = dgvPlayList.HitTest(e.Location.X, e.Location.Y);
            if (inf.Type == DataGridViewHitTestType.Cell)
            {
                dgvPlayList.Rows[inf.RowIndex].Selected = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                cmsPLList.Show();
                Point p = Control.MousePosition;
                cmsPLList.Top = p.Y;
                cmsPLList.Left = p.X;
            }
        }

        private void dgvPlayList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvPlayList.Rows[e.RowIndex].Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value == null)
            {
                playList = PlayList.Load(null);

            }
            else
            {
                string currentPlayList = dgvPlayList.Rows[e.RowIndex].Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value.ToString();
                playList = null;
                if (Path.GetExtension(currentPlayList).ToLower() == ".m3u")
                {
                    playList = PlayList.LoadM3U(currentPlayList);
                }
                else
                {
                    playList = PlayList.Load(currentPlayList);
                }
            }
            playList.SetDGV(dgvList);
            playIndex = -1;

            oldPlayIndex = -1;
            Refresh();
        }

        private void tsmiPlayThis_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count < 0) return;

            playing = false;

            string fn = (string)dgvList.Rows[dgvList.SelectedRows[0].Index].Cells["clmFileName"].Value;
            string zfn = (string)dgvList.Rows[dgvList.SelectedRows[0].Index].Cells["clmZipFileName"].Value;
            int m = 0;
            if (dgvList.Rows[dgvList.SelectedRows[0].Index].Cells[dgvList.Columns["clmType"].Index].Value != null && dgvList.Rows[dgvList.SelectedRows[0].Index].Cells[dgvList.Columns["clmType"].Index].Value.ToString() != "-")
            {
                m = dgvList.Rows[dgvList.SelectedRows[0].Index].Cells[dgvList.Columns["clmType"].Index].Value.ToString()[0] - 'A';
                if (m < 0 || m > 9) m = 0;
            }
            int songNo = 0;
            try
            {
                songNo = (Int32)dgvList.Rows[dgvList.SelectedRows[0].Index].Cells["clmSongNo"].Value;
            }
            catch
            {
                songNo = 0;
            }
            string[] spFn = null;
            if (dgvList.SelectedRows[0].Cells["clmSupportFile"].Value != null)
            {
                spFn = dgvList.SelectedRows[0].Cells["clmSupportFile"].Value.ToString().Split(';');
            }
            string useCom = null;
            if (dgvList.Rows[0].Cells["clmUseCompiler"].Value != null)
            {
                useCom = dgvList.Rows[0].Cells["clmUseCompiler"].Value.ToString();
            }
            GD3 gd3 = getGD3FromPl(0);
            frmMain.loadAndPlay(m, songNo, fn, zfn, spFn, useCom, gd3);
            updatePlayingIndex(dgvList.SelectedRows[0].Index);

            playing = true;
        }

        private void tsmiDelAllMusic_Click(object sender, EventArgs e)
        {

            DialogResult res = MessageBox.Show("プレイリストの全ての曲が除去されます。よろしいですか。", "PlayList", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (res != DialogResult.OK) return;

            playing = false;
            dgvList.Rows.Clear();
            playList.LstMusic.Clear();
            playIndex = -1;
            oldPlayIndex = -1;

        }

        private void tsbOpenPlayList_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "サポートする全てのプレイリスト(*.xml;*.m3u)|*.xml;*.m3u|ファイル(*.xml)|*.xml|M3Uファイル(*.m3u)|*.m3u";
            ofd.Title = "プレイリストファイルを選択";
            string[] fl=ofd.Filter.Split('|');
            int index=-1;
            for (int i = 0; i < fl.Length; i += 2)
            {
                if (fl[i + 1] != setting.other.PlayListFilterIndex) continue;
                index = i / 2 + 1;
                break;
            }
            if (index == -1) index = 0;

            ofd.FilterIndex = index;
            if (frmMain.setting.other.DefaultDataPath != "" && Directory.Exists(frmMain.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = frmMain.setting.other.DefaultDataPath;
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
            setting.other.PlayListFilterIndex = fl[(ofd.FilterIndex - 1) * 2 + 1];

            try
            {

                PlayList pl = null;

                if (ofd.FileName.ToLower().LastIndexOf(".m3u") == -1)
                {
                    pl = PlayList.Load(ofd.FileName);
                    playing = false;
                    playList = pl;
                    playList.SetDGV(dgvList);
                }
                else
                {
                    pl = PlayList.LoadM3U(ofd.FileName);
                    playing = false;
                    playList.LstMusic.Clear();
                    foreach (PlayList.Music ms in pl.LstMusic)
                    {
                        playList.AddFile(ms.fileName);
                        //AddList(ms.fileName);
                    }
                }

                playIndex = -1;
                oldPlayIndex = -1;

                Refresh();

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }
        }

        private void tsbSavePlayList_Click(object sender, EventArgs e)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "M3Uファイル(*.m3u)|*.m3u|XMLファイル(*.xml)|*.xml";
            sfd.Title = "プレイリストファイルを保存";
            string[] fl = sfd.Filter.Split('|');
            int index = -1;
            for (int i = 0; i < fl.Length; i += 2)
            {
                if (fl[i + 1]!=setting.other.PlayListFilterIndex) continue;
                index = i / 2 + 1;
                break;
            }
            if (index == -1) index = 0;
            sfd.FilterIndex = index;
            if (frmMain.setting.other.DefaultDataPath != "" && Directory.Exists(frmMain.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                sfd.InitialDirectory = frmMain.setting.other.DefaultDataPath;
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
            string filename = sfd.FileName;
            setting.other.PlayListFilterIndex = fl[(sfd.FilterIndex - 1) * 2 + 1];

            switch (sfd.FilterIndex)
            {
                case 0:
                    if (Path.GetExtension(filename) == "")
                    {
                        filename = Path.Combine(filename, ".m3u");
                    }
                    break;
                case 1:
                    if (Path.GetExtension(filename) == "")
                    {
                        filename = Path.Combine(filename, ".xml");
                    }
                    break;
            }

            try
            {
                if (sfd.FileName.ToLower().LastIndexOf(".m3u") == -1)
                    playList.Save(sfd.FileName);
                else
                    playList.SaveM3U(sfd.FileName);

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの保存に失敗しました。");
            }
        }

        private void tsbAddMusic_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Resources.cntSupportFile.Replace("\r\n", "");
            ofd.Title = "ファイルを選択してください";
            ofd.FilterIndex = setting.other.FilterIndex;

            if (frmMain.setting.other.DefaultDataPath != "" && Directory.Exists(frmMain.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = frmMain.setting.other.DefaultDataPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            IsInitialOpenFolder = false;
            setting.other.FilterIndex = ofd.FilterIndex;

            Stop();

            try
            {
                foreach (string fn in ofd.FileNames)
                {
                    playList.AddFile(fn);
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

            //Play();
        }

        private void tsbAddFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "フォルダーを指定してください。";
            if (frmMain.setting.other.DefaultDataPath != "" && Directory.Exists(frmMain.setting.other.DefaultDataPath))
            {
                fbd.SelectedPath = frmMain.setting.other.DefaultDataPath;
            }

            if (fbd.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Stop();

            try
            {
                string[] files = System.IO.Directory.GetFiles(fbd.SelectedPath, "*", System.IO.SearchOption.TopDirectoryOnly);
                foreach (string fn in files)
                {
                    string ext = System.IO.Path.GetExtension(fn).ToUpper();
                    if (
                           ext == ".VGM" || ext == ".VGZ" || ext == ".ZIP" || ext == ".NRD"
                        || ext == ".XGM" || ext == ".S98" || ext == ".NSF" || ext == ".HES"
                        || ext == ".SID" || ext == ".MID" || ext == ".RCP" || ext == ".M3U"
                        || ext == ".MDR" || ext == ".XGZ"
                        )
                    {
                        playList.AddFile(fn);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

            frmMain.oldParam = new MDChipParams();

            Play();

        }

        private void tsbUp_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count < 1 || dgvList.SelectedRows[0].Index < 1)
            {
                return;
            }

            int ind = dgvList.SelectedRows[0].Index;
            PlayList.Music mus = playList.LstMusic[ind - 1];
            DataGridViewRow row = dgvList.Rows[ind - 1];

            if (ind == playIndex) playIndex--;
            else if (ind == playIndex + 1) playIndex++;

            if (ind == oldPlayIndex) oldPlayIndex--;
            else if (ind == oldPlayIndex + 1) oldPlayIndex++;

            playList.LstMusic.RemoveAt(ind - 1);
            dgvList.Rows.RemoveAt(ind - 1);

            playList.LstMusic.Insert(ind, mus);
            dgvList.Rows.Insert(ind, row);

        }

        private void tsbDown_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count != 1 || dgvList.SelectedRows[0].Index >= dgvList.Rows.Count - 1)
            {
                return;
            }

            int ind = dgvList.SelectedRows[0].Index;
            PlayList.Music mus = playList.LstMusic[ind + 1];
            DataGridViewRow row = dgvList.Rows[ind + 1];

            if (ind == playIndex) playIndex++;
            else if (ind == playIndex - 1) playIndex--;

            if (ind == oldPlayIndex) oldPlayIndex++;
            else if (ind == oldPlayIndex - 1) oldPlayIndex--;

            playList.LstMusic.RemoveAt(ind + 1);
            dgvList.Rows.RemoveAt(ind + 1);

            playList.LstMusic.Insert(ind, mus);
            dgvList.Rows.Insert(ind, row);

        }

        private void tsbAll_Click(object sender, EventArgs e)
        {
            showAll_Click(null, null);
        }

        private void tsbEnglish_Click(object sender, EventArgs e)
        {
            ChangeLang(false);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ChangeLang(true);
        }

        private void ChangeLang(bool isJP)
        {
            dgvList.Columns["clmTitle"].Visible = !isJP;
            dgvList.Columns["clmTitleJ"].Visible = isJP;
            dgvList.Columns["clmGame"].Visible = !isJP;
            dgvList.Columns["clmGameJ"].Visible = isJP;
            dgvList.Columns["clmComposer"].Visible = !isJP;
            dgvList.Columns["clmComposerJ"].Visible = isJP;
        }

        private void frmPlayList_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine("keycode{0} {1} {2}", e.KeyCode, e.KeyData, e.KeyValue);

            switch (e.KeyValue)
            {
                case 32: //Space
                case 13: //Enter
                    if (dgvList.SelectedRows.Count == 0)
                    {
                        return;
                    }

                    int index = dgvList.SelectedRows[0].Index;

                    e.Handled = true;

                    playing = false;

                    string fn = (string)dgvList.SelectedRows[0].Cells["clmFileName"].Value;
                    string zfn = (string)dgvList.SelectedRows[0].Cells["clmZipFileName"].Value;
                    int m = 0;
                    if (dgvList.SelectedRows[0].Cells[dgvList.Columns["clmType"].Index].Value != null && dgvList.SelectedRows[0].Cells[dgvList.Columns["clmType"].Index].Value.ToString() != "-")
                    {
                        m = dgvList.SelectedRows[0].Cells[dgvList.Columns["clmType"].Index].Value.ToString()[0] - 'A';
                        if (m < 0 || m > 9) m = 0;
                    }
                    int songNo = 0;
                    try
                    {
                        songNo = (Int32)dgvList.SelectedRows[0].Cells["clmSongNo"].Value;
                    }
                    catch
                    {
                        songNo = 0;
                    }
                    string[] spFn = null;
                    if (dgvList.SelectedRows[0].Cells["clmSupportFile"].Value != null)
                    {
                        spFn = dgvList.SelectedRows[0].Cells["clmSupportFile"].Value.ToString().Split(';');
                    }
                    string useCom = null;
                    if (dgvList.Rows[0].Cells["clmUseCompiler"].Value != null)
                    {
                        useCom = dgvList.Rows[0].Cells["clmUseCompiler"].Value.ToString();
                    }
                    GD3 gd3 = getGD3FromPl(0);
                    frmMain.loadAndPlay(m, songNo, fn, zfn, spFn, useCom, gd3);
                    updatePlayingIndex(index);

                    playing = true;
                    break;
                case 46: //Delete
                    e.Handled = true;
                    tsmiDelThis_Click(null, null);
                    break;
            }

        }

        private void dgvList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            Point cp = dgvList.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hti = dgvList.HitTest(cp.X, cp.Y);
            if (hti.Type != DataGridViewHitTestType.Cell || hti.RowIndex < 0 || hti.RowIndex >= dgvList.Rows.Count) return;
            dgvList.MultiSelect = false;
            dgvList.MultiSelect = true;
            dgvList.Rows[hti.RowIndex].Selected = true;
        }

        private void dgvList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            Point cp = dgvList.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hti = dgvList.HitTest(cp.X, cp.Y);
            if (hti.Type != DataGridViewHitTestType.Cell || hti.RowIndex < 0 || hti.RowIndex >= dgvList.Rows.Count) return;
            dgvList.MultiSelect = false;
            dgvList.MultiSelect = true;
            dgvList.Rows[hti.RowIndex].Selected = true;
        }

        private object relock = new object();
        private bool reent = false;

        public void dgvList_DragDrop(object sender, DragEventArgs e)
        {
            lock (relock)
            {
                if (reent) return;
                reent = true;
            }

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            try
            {
                this.Enabled = false;
                this.timer1.Enabled = false;

                string[] filename = ((string[])e.Data.GetData(DataFormats.FileDrop));

                //ドロップされたアイテムがフォルダーの場合は下位フォルダー内も含めた
                //実際のファイルのリストを取得する
                List<string> result = new List<string>();
                GetTrueFileNameList(result, filename);
                //重複を取り除く
                filename = result.Distinct().ToArray();

                int i = playList.LstMusic.Count;
                Point cp = dgvList.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo hti = dgvList.HitTest(cp.X, cp.Y);
                if (hti.Type == DataGridViewHitTestType.Cell && hti.RowIndex >= 0 && hti.RowIndex < dgvList.Rows.Count)
                {
                    if (hti.RowIndex < playList.LstMusic.Count) i = hti.RowIndex;
                }

                //曲を停止
                Stop();
                frmMain.Stop();
                while (!Audio.IsStopped)
                    Application.DoEvents();

                int buIndex = i;

                playList.InsertFile(ref i, filename);

                if (buIndex <= oldPlayIndex)
                {
                    oldPlayIndex += i - buIndex;
                }
                i = buIndex;

                if (i >= playList.LstMusic.Count) return;

                //選択位置の曲を再生する
                string fn = playList.LstMusic[i].fileName;
                if (playList.LstMusic[i].arcType != EnmArcType.LZH
                    && playList.LstMusic[i].arcType != EnmArcType.ZIP
                    && playList.LstMusic[i].arcType != EnmArcType.ZDF
                    && (playList.LstMusic[i].arcFileName == null || playList.LstMusic[i].arcFileName.ToLower().LastIndexOf(".m3u") == -1)
                    && fn.ToLower().LastIndexOf(".lzh") == -1
                    && fn.ToLower().LastIndexOf(".zip") == -1
                    && fn.ToLower().LastIndexOf(".zdf") == -1
                    && fn.ToLower().LastIndexOf(".m3u") == -1
                    //&& fn.ToLower().LastIndexOf(".sid") == -1
                    )
                {
                    frmMain.loadAndPlay(0, 0, fn, null, null, null,null);
                    setStart(i);// -1);
                    frmMain.oldParam = new MDChipParams();
                    Play();
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show("ファイルの読み込みに失敗しました。");
            }
            finally
            {
                this.Enabled = true;
                this.timer1.Enabled = true;
                lock (relock)
                {
                    reent = false;
                }
            }

        }

        private void GetTrueFileNameList(List<string> res, IEnumerable<string> files)
        {
            foreach (string f in files)
            {
                if (File.Exists(f))
                {
                    if (!res.Contains(f))
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        if (sext.Contains(ext)) res.Add(f);
                    }
                }
                else
                {
                    if (Directory.Exists(f))
                    {
                        IEnumerable<string> fs = Directory.EnumerateFiles(f, "*", SearchOption.AllDirectories);
                        GetTrueFileNameList(res, fs);
                    }
                }
            }
        }

        private void tsmiA_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count < 1) return;

            List<int> sel = new List<int>();
            foreach (DataGridViewRow r in dgvList.SelectedRows)
            {
                playList.LstMusic[r.Index].type = ((ToolStripMenuItem)sender).Text;
                r.Cells["clmType"].Value = ((ToolStripMenuItem)sender).Text;
            }
        }

        string ofn = "";
        string oafn = "";
        string[][] exts = new string[3][];
        string text = "";
        string mml = "";
        string img = "";

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!playing) return;
            if (setting == null) return;

            try
            {
                string fn = "";
                string arcFn = "";

                Audio.GetPlayingFileName(out fn, out arcFn);

                if (fn == ofn && arcFn == oafn) return;
                ofn = fn;
                oafn = arcFn;

                exts[0] = setting.other.TextExt.Split(';');
                exts[1] = setting.other.MMLExt.Split(';');
                exts[2] = setting.other.ImageExt.Split(';');

                string dir = Path.GetDirectoryName(fn);
                string bfn = Path.Combine(dir, Path.GetFileNameWithoutExtension(fn));
                string bfnfld = Path.Combine(dir, Path.GetFileName(dir));

                text = "";
                foreach (string ext in exts[0])
                {
                    if (File.Exists(bfn + "." + ext))
                    {
                        text = bfn + "." + ext;
                        break;
                    }
                    if (File.Exists(bfnfld + "." + ext))
                    {
                        text = bfnfld + "." + ext;
                        break;
                    }
                }
                mml = "";
                foreach (string ext in exts[1])
                {
                    if (File.Exists(bfn + "." + ext))
                    {
                        mml = bfn + "." + ext;
                        break;
                    }
                    if (File.Exists(bfnfld + "." + ext))
                    {
                        mml = bfnfld + "." + ext;
                        break;
                    }
                }
                img = "";
                foreach (string ext in exts[2])
                {
                    if (File.Exists(bfn + "." + ext))
                    {
                        img = bfn + "." + ext;
                        break;
                    }
                    if (File.Exists(bfnfld + "." + ext))
                    {
                        img = bfnfld + "." + ext;
                        break;
                    }
                }

                tsbTextExt.Enabled = (text != "");
                tsbMMLExt.Enabled = (mml != "");
                tsbImgExt.Enabled = (img != "");

                if (setting.other.AutoOpenText && text != "") tsbTextExt_Click(null, null);
                if (setting.other.AutoOpenMML && mml != "") tsbMMLExt_Click(null, null);
                if (setting.other.AutoOpenImg && img != "") tsbImgExt_Click(null, null);
            }
            catch
            {
                //解析できないファイル名の場合は無視
                ;
            }
        }

        private void tsbTextExt_Click(object sender, EventArgs e)
        {
            if (text == "") return;
            Process.Start("explorer.exe", text);
        }

        private void tsbMMLExt_Click(object sender, EventArgs e)
        {
            if (mml == "") return;
            Process.Start("explorer.exe", mml);
        }

        private void tsbImgExt_Click(object sender, EventArgs e)
        {
            if (img == "") return;
            Process.Start("explorer.exe", img);
        }

        public PlayList.Music getPlayingSongInfo()
        {
            if (playIndex < 0 || dgvList.Rows.Count <= playIndex) return null;
            return (PlayList.Music)dgvList.Rows[playIndex].Tag;
        }

        private void tsmiOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string path = (string)dgvList.SelectedRows[0].Cells["clmFileName"].Value;
                path = Path.GetDirectoryName(path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            catch
            {
                ;
            }
        }

        public void upCursor()
        {
            if (dgvList.SelectedRows.Count < 1)
            {
                if (dgvList.Rows.Count < 1) return;
                dgvList.Rows[0].Selected = true;
                return;
            }

            int c = dgvList.SelectedRows[0].Index;
            dgvList.Rows[c].Selected = false;
            c--;
            if (c < 0) c = 0;
            dgvList.Rows[c].Selected = true;
        }

        public void downCursor()
        {
            if (dgvList.SelectedRows.Count < 1)
            {
                if (dgvList.Rows.Count < 1) return;
                dgvList.Rows[dgvList.Rows.Count - 1].Selected = true;
                return;
            }

            int c = dgvList.SelectedRows[0].Index;
            dgvList.Rows[c].Selected = false;
            c++;
            if (c >= dgvList.Rows.Count) c = dgvList.Rows.Count - 1;
            dgvList.Rows[c].Selected = true;
        }

        public void playCursor()
        {
            if (dgvList.SelectedRows.Count < 1)
            {
                if (dgvList.Rows.Count < 1) return;
                dgvList.Rows[0].Selected = true;
            }

            KeyEventArgs kea = new KeyEventArgs(Keys.Enter);
            frmPlayList_KeyDown(null, kea);
        }

        private void tsmiSelectSupportFile_Click(object sender, EventArgs e)
        {
            int chk = 0;
            foreach (DataGridViewRow row in dgvList.SelectedRows)
            {
                if (row.Cells["clmEXT"].Value.ToString() == ".RCS") { chk |= 1; }
                else chk |= 2;
            }
            if (chk == 3)
            {
                MessageBox.Show("Select files of the same type.", "Information", MessageBoxButtons.OK);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            if (chk == 1) ofd.Filter = "support file(*.rcp;*.g36)|*.rcp;*.g36";
            else if (chk == 2) ofd.Filter = "support file(*.zmd;*.zms;*.zpd)|*.zmd;*.zms;*.zpd";
            ofd.Title = "サポートファイルを選択";
            if (frmMain.setting.other.DefaultDataPath != "" && Directory.Exists(frmMain.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = frmMain.setting.other.DefaultDataPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            foreach (DataGridViewRow row in dgvList.SelectedRows)
            {
                string fs = "";
                string ffs = "";
                foreach (string f in ofd.FileNames)
                {
                    fs += f + ";";
                    ffs += Path.GetFileName(f) + ";";
                }
                if (fs.Length > 0) fs = fs.Substring(0, fs.Length - 1);
                if (ffs.Length > 0) ffs = ffs.Substring(0, ffs.Length - 1);
                row.Cells["clmSupportFile"].Value = fs;
                row.Cells["clmDispSupportFileName"].Value = string.IsNullOrEmpty(ffs) ? "-" : ffs;
                row.Cells["clmDispSupportFileName"].ToolTipText = fs;
                playList.LstMusic[row.Index].supportFileName = fs;
            }

            Refresh();

        }

        private void tsmiRemoveSupportFile_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvList.SelectedRows)
            {
                row.Cells["clmSupportFile"].Value = null;
                row.Cells["clmDispSupportFileName"].Value = "-";
                row.Cells["clmDispSupportFileName"].ToolTipText = null;
                playList.LstMusic[row.Index].supportFileName = null;
            }

            Refresh();

        }

        private void tsmiSelectCompiler_default_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvList.SelectedRows)
            {
                row.Cells["clmUseCompiler"].Value = null;
                row.Cells["clmDispUseCompiler"].Value = "-";
                playList.LstMusic[row.Index].useCompiler = null;
            }

            Refresh();

        }

        private void tsmiSelectCompiler_zmusicV2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvList.SelectedRows)
            {
                row.Cells["clmUseCompiler"].Value = "zmusic v2";
                row.Cells["clmDispUseCompiler"].Value = "zmusic v2";
                playList.LstMusic[row.Index].useCompiler = "zmusic v2";
            }

            Refresh();

        }

        private void tsmiSelectCompiler_zmusicV3_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvList.SelectedRows)
            {
                row.Cells["clmUseCompiler"].Value = "zmusic v3";
                row.Cells["clmDispUseCompiler"].Value = "zmusic v3";
                playList.LstMusic[row.Index].useCompiler = "zmusic v3";
            }

            Refresh();
        }

        private void tsmiAddPlayList_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "M3Uファイル(*.m3u)|*.m3u|XMLファイル(*.xml)|*.xml";
            ofd.Title = "PlayListファイルを選択してください";
            ofd.FilterIndex = setting.other.FilterIndex;

            if (frmMain.setting.other.DefaultDataPath != "" && Directory.Exists(frmMain.setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = frmMain.setting.other.DefaultDataPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            IsInitialOpenFolder = false;
            setting.other.FilterIndex = ofd.FilterIndex;

            try
            {
                foreach (string fn in ofd.FileNames)
                {
                    DataGridViewRow row = new();
                    row.CreateCells(dgvPlayList);
                    row.Cells[dgvPlayList.Columns["clmPL_FileName"].Index].Value = fn;
                    row.Cells[dgvPlayList.Columns["clmPL_Title"].Index].Value = Path.GetFileName(fn);
                    dgvPlayList.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }


        }

        private void tsmiRemovePlayList_Click(object sender, EventArgs e)
        {
            int row = 1;
            for (; row < dgvPlayList.Rows.Count; row++)
            {
                if (!dgvPlayList.Rows[row].Selected) continue;
                dgvPlayList.Rows.RemoveAt(row);
                row--;
            }
        }

        public void UpdateDuration(string pfn, string paf, int minutes, int second, int millisecond)
        {
            pfn = string.IsNullOrEmpty(pfn) ? "" : pfn;
            paf = string.IsNullOrEmpty(paf) ? "" : paf;

            for (int row = 0; row < dgvList.Rows.Count; row++)
            {
                string fn = (string)dgvList.Rows[row].Cells["clmFileName"].Value;
                fn = string.IsNullOrEmpty(fn) ? "" : fn;
                if (pfn != fn) continue;
                string zfn = (string)dgvList.Rows[row].Cells["clmZipFileName"].Value;
                zfn = string.IsNullOrEmpty(zfn) ? "" : zfn;
                if (paf != zfn) continue;

                dgvList.Rows[row].Cells["clmDuration"].Value = string.Format("{0:d02}:{1:d02}.{2:d02}", minutes, second, millisecond);
            }

            for (int row = 0; row < playList.LstMusic.Count; row++)
            {
                string fn = (string)playList.LstMusic[row].fileName;
                fn = string.IsNullOrEmpty(fn) ? "" : fn;
                if (pfn != fn) continue;
                string zfn = (string)playList.LstMusic[row].arcFileName;
                zfn = string.IsNullOrEmpty(zfn) ? "" : zfn;
                if (paf != zfn) continue;

                playList.LstMusic[row].duration = string.Format("{0:d02}:{1:d02}.{2:d02}", minutes, second, millisecond);
            }

        }

    }
}
