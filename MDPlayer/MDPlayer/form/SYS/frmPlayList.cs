using MDPlayer.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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

        private PlayList playList = null;
        private frmMain frmMain = null;

        private bool playing = false;
        private int playIndex = -1;
        private int oldPlayIndex = -1;

        private Random rand = new System.Random();
        private bool IsInitialOpenFolder = true;

        public frmPlayList(frmMain frm)
        {
            frmMain = frm;
            setting = frm.setting;
            InitializeComponent();

            playList = PlayList.Load(null);
            playList.SetDGV(dgvList);
            playIndex = -1;

            oldPlayIndex = -1;
        }

        public bool isPlaying()
        {
            return playing;
        }

        public int getMusicCount()
        {
            return playList.lstMusic.Count;
        }

        public PlayList getPlayList()
        {
            return playList;
        }

        public Tuple<int,int,string,string> setStart(int n)
        {
            updatePlayingIndex(n);

            string fn = playList.lstMusic[playIndex].fileName;
            string zfn = playList.lstMusic[playIndex].arcFileName;
            int m = 0;
            int songNo = playList.lstMusic[playIndex].songNo;

            if (playList.lstMusic[playIndex].type != null && playList.lstMusic[playIndex].type != "-")
            {
                m = playList.lstMusic[playIndex].type[0] - 'A';
                if (m < 0 || m > 9) m = 0;
            }

            return new Tuple<int, int, string, string>(m, songNo, fn, zfn);
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
                playList.lstMusic = new List<PlayList.music>();
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

        protected override void WndProc(ref Message m)
        {
            if (frmMain != null)
            {
                frmMain.windowsMessage(ref m);
            }

            base.WndProc(ref m);
        }

        private void frmPlayList_Shown(object sender, EventArgs e)
        {
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
            this.Visible = false;
            e.Cancel = true;
        }

        public new void Refresh()
        {
            dgvList.Rows.Clear();
            List<DataGridViewRow> rows =playList.makeRow(playList.lstMusic);
            foreach (DataGridViewRow row in rows)
            {
                dgvList.Rows.Add(row);
            }
        }

        public void updatePlayingIndex(int newPlayingIndex)
        {
            if (oldPlayIndex != -1)
            {
                dgvList.Rows[oldPlayIndex].Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
            }

            if (newPlayingIndex >= 0 && newPlayingIndex < dgvList.Rows.Count)
            {
                dgvList.Rows[newPlayingIndex].Cells[dgvList.Columns["clmPlayingNow"].Index].Value = ">";
            }
            else if (newPlayingIndex == -1)
            {
                newPlayingIndex = dgvList.Rows.Count - 1;
                dgvList.Rows[newPlayingIndex].Cells[dgvList.Columns["clmPlayingNow"].Index].Value = ">";
            }
            else if (newPlayingIndex == -2)
            {
                newPlayingIndex = 0;
                dgvList.Rows[newPlayingIndex].Cells[dgvList.Columns["clmPlayingNow"].Index].Value = ">";
            }
            playIndex = newPlayingIndex;
            oldPlayIndex = newPlayingIndex;
        }

        private void dgvList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgvList.Rows[e.RowIndex].Selected = true;

            if (e.Button == MouseButtons.Right)
            {
                if (dgvList.SelectedRows.Count > 1)
                {
                    tsmiDelThis.Text = "選択した曲を除去";
                }
                else
                {
                    tsmiDelThis.Text = "この曲を除去";
                }
                cmsPlayList.Show();
                Point p = Control.MousePosition;
                cmsPlayList.Top = p.Y;
                cmsPlayList.Left = p.X;
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
                playList.lstMusic.RemoveAt(dgvList.SelectedRows[i].Index);
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

            frmMain.loadAndPlay(m, songNo, fn, zfn);
            if (Audio.errMsg != "")
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
            if (!playing) return;

            int pi = playIndex;
            playing = false;

            switch (mode)
            {
                case 0:// 通常
                    if (dgvList.Rows.Count <= playIndex + 1) return;
                    pi++;
                    break;
                case 1:// ランダム
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

            string fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
            string zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
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

            if(!frmMain.loadAndPlay(m, songNo, fn, zfn))
            {
                playing = false;
                return;
            }
            updatePlayingIndex(pi);
            playing = true;
        }

        public void prevPlay()
        {
            if (!playing) return;
            if (playIndex < 1) return;

            int pi = playIndex;
            playing = false;
            pi--;

            string fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
            string zfn = (string)dgvList.Rows[pi].Cells["clmZipFileName"].Value;
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

            frmMain.loadAndPlay(m, songNo, fn, zfn);
            updatePlayingIndex(pi);
            playing = true;
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

                if (!frmMain.loadAndPlay(m, songNo, fn, zfn)) return;
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

            frmMain.loadAndPlay(m, songNo, fn, zfn);
            updatePlayingIndex(dgvList.SelectedRows[0].Index);

            playing = true;
        }

        private void tsmiDelAllMusic_Click(object sender, EventArgs e)
        {

            DialogResult res = MessageBox.Show("プレイリストの全ての曲が除去されます。よろしいですか。", "PlayList", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (res != DialogResult.OK) return;

            playing = false;
            dgvList.Rows.Clear();
            playList.lstMusic.Clear();
            playIndex = -1;
            oldPlayIndex = -1;

        }

        private void tsbOpenPlayList_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "サポートする全てのプレイリスト(*.xml;*.m3u)|*.xml;*.m3u|ファイル(*.xml)|*.xml|M3Uファイル(*.m3u)|*.m3u";
            ofd.Title = "プレイリストファイルを選択";
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
                    playList.lstMusic.Clear();
                    foreach (PlayList.music ms in pl.lstMusic)
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
                        || ext == ".MDR"
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
            PlayList.music mus = playList.lstMusic[ind-1];
            DataGridViewRow row = dgvList.Rows[ind - 1];

            if (ind == playIndex) playIndex--;
            else if (ind == playIndex + 1) playIndex++;

            if (ind == oldPlayIndex) oldPlayIndex--;
            else if (ind == oldPlayIndex + 1) oldPlayIndex++;

            playList.lstMusic.RemoveAt(ind-1);
            dgvList.Rows.RemoveAt(ind-1);

            playList.lstMusic.Insert(ind, mus);
            dgvList.Rows.Insert(ind, row);

        }

        private void tsbDown_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count != 1 || dgvList.SelectedRows[0].Index >= dgvList.Rows.Count - 1)
            {
                return;
            }

            int ind = dgvList.SelectedRows[0].Index;
            PlayList.music mus = playList.lstMusic[ind + 1];
            DataGridViewRow row = dgvList.Rows[ind + 1];

            if (ind == playIndex) playIndex++;
            else if (ind == playIndex - 1) playIndex--;

            if (ind == oldPlayIndex) oldPlayIndex++;
            else if (ind == oldPlayIndex - 1) oldPlayIndex--;

            playList.lstMusic.RemoveAt(ind + 1);
            dgvList.Rows.RemoveAt(ind + 1);

            playList.lstMusic.Insert(ind, mus);
            dgvList.Rows.Insert(ind, row);

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            dgvList.Columns["clmTitle"].Visible = !tsbJapanese.Checked;
            dgvList.Columns["clmTitleJ"].Visible = tsbJapanese.Checked;
            dgvList.Columns["clmGame"].Visible = !tsbJapanese.Checked;
            dgvList.Columns["clmGameJ"].Visible = tsbJapanese.Checked;
            dgvList.Columns["clmComposer"].Visible = !tsbJapanese.Checked;
            dgvList.Columns["clmComposerJ"].Visible = tsbJapanese.Checked;
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

                    frmMain.loadAndPlay(m, songNo, fn, zfn);
                    updatePlayingIndex(index);

                    playing = true;
                    break;
                case 46: //Delete
                    e.Handled = true;
                    tsmiDelThis_Click(null, null);
                    break;
            }

        }

        private void frmPlayList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void frmPlayList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filename = ((string[])e.Data.GetData(DataFormats.FileDrop));

                foreach (string fn in filename)
                {
                    try
                    {

                        Stop();

                        playList.AddFile(fn);
                        //AddList(fn);

                        if (
                            fn.ToLower().LastIndexOf(".lzh") == -1 
                            && fn.ToLower().LastIndexOf(".zip") == -1
                            && fn.ToLower().LastIndexOf(".m3u") == -1
                            && fn.ToLower().LastIndexOf(".sid") == -1
                            )
                        {
                            frmMain.loadAndPlay(0, 0, fn);
                            setStart(-1);
                            frmMain.oldParam = new MDChipParams();
                            Play();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                        MessageBox.Show("ファイルの読み込みに失敗しました。");
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
                playList.lstMusic[r.Index].type= ((ToolStripMenuItem)sender).Text;
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

            string fn = "";
            string arcFn = "";

            Audio.getPlayingFileName(out fn, out arcFn);

            if (fn == ofn && arcFn == oafn) return;
            ofn = fn;
            oafn = arcFn;

            exts[0] = setting.other.TextExt.Split(';');
            exts[1] = setting.other.MMLExt.Split(';');
            exts[2] = setting.other.ImageExt.Split(';');

            string bfn = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn));
            string bfnfld = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileName(Path.GetDirectoryName(fn)));

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

        private void tsbTextExt_Click(object sender, EventArgs e)
        {
            if (text == "") return;
            Process.Start(text);
        }

        private void tsbMMLExt_Click(object sender, EventArgs e)
        {
            if (mml == "") return;
            Process.Start(mml);
        }

        private void tsbImgExt_Click(object sender, EventArgs e)
        {
            if (img == "") return;
            Process.Start(img);
        }

        public PlayList.music getPlayingSongInfo()
        {
            if (playIndex < 0 || dgvList.Rows.Count <= playIndex) return null;
            return (PlayList.music)dgvList.Rows[playIndex].Tag;
        }

        private void tsmiOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string path = (string)dgvList.SelectedRows[0].Cells["clmFileName"].Value;
                path = Path.GetDirectoryName(path);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                ;
            }
        }
    }
}
