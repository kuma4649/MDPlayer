using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer
{
    public partial class frmPlayList : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private frmMain frmMain = null;
        public Setting setting = null;

        private PlayList playList = null;

        private bool playing = false;
        private int playIndex = -1;
        private int oldPlayIndex = -1;

        private Random rand = new System.Random();
        private bool IsInitialOpenFolder = true;


        public frmPlayList(frmMain frm)
        {
            frmMain = frm;
            InitializeComponent();

            playList = PlayList.Load(null);
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
            string zfn = playList.lstMusic[playIndex].zipFileName;
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
            this.Location = new Point(x, y);
        }

        private void frmPlayList_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosed = true;
            setting.location.PPlayList = this.Location;
            setting.location.PPlayListWH = new Point(this.Width, this.Height);
            this.Visible = false;
            e.Cancel = true;
        }

        public new void Refresh()
        {
            dgvList.Rows.Clear();
            List<DataGridViewRow> rows = makeRow(playList.lstMusic);
            foreach (DataGridViewRow row in rows)
            {
                dgvList.Rows.Add(row);
            }
        }

        public const int FCC_VGM = 0x206D6756;	// "Vgm "

        public void AddList(string file)
        {
            if (file.ToLower().LastIndexOf(".zip") != -1)
            {
                using (ZipArchive archive = ZipFile.OpenRead(file))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string dummy;
                        byte[] buf = frmMain.getBytesFromZipFile(entry,out dummy);

                        if (buf != null)
                        {
                            List<PlayList.music> zipmusics = Audio.getMusic(entry.FullName, buf, file);
                            List<DataGridViewRow> ziprows = makeRow(zipmusics);

                            foreach (DataGridViewRow ziprow in ziprows)
                            {
                                dgvList.Rows.Add(ziprow);
                            }
                            foreach (PlayList.music zipmusic in zipmusics)
                            {
                                playList.lstMusic.Add(zipmusic);
                            }
                        }
                    }
                }
                return;
            }

            if (file.ToLower().LastIndexOf(".m3u") != -1)
            {
                PlayList pl= PlayList.LoadM3U(file);
                foreach (PlayList.music ms in pl.lstMusic)
                {
                    AddList(ms.fileName);
                }
                return;
            }


            enmFileFormat dmyFileFormat;
            List<PlayList.music> musics = Audio.getMusic(file, frmMain.getAllBytes(file, out dmyFileFormat));

            List<DataGridViewRow> rows = makeRow(musics);

            foreach(DataGridViewRow row in rows) dgvList.Rows.Add(row);
            foreach(PlayList.music music in musics) playList.lstMusic.Add(music);
            //updatePlayingIndex(dgvList.Rows.Count - 1);
        }

        private List<DataGridViewRow> makeRow(List<PlayList.music> musics)
        {
            List<DataGridViewRow> ret = new List<DataGridViewRow>();

            foreach (PlayList.music music in musics)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvList);
                row.Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
                row.Cells[dgvList.Columns["clmKey"].Index].Value = 0;
                row.Cells[dgvList.Columns["clmFileName"].Index].Value = music.fileName;
                row.Cells[dgvList.Columns["clmZipFileName"].Index].Value = music.zipFileName;
                row.Cells[dgvList.Columns["clmEXT"].Index].Value = Path.GetExtension(music.fileName).ToUpper();
                row.Cells[dgvList.Columns["clmType"].Index].Value = music.type;
                row.Cells[dgvList.Columns["clmTitle"].Index].Value = music.title;
                row.Cells[dgvList.Columns["clmTitleJ"].Index].Value = music.titleJ;
                row.Cells[dgvList.Columns["clmGame"].Index].Value = music.game;
                row.Cells[dgvList.Columns["clmGameJ"].Index].Value = music.gameJ;
                //row.Cells[dgvList.Columns["clmRemark"].Index].Value = music.remark;
                row.Cells[dgvList.Columns["clmComposer"].Index].Value = music.composer;
                row.Cells[dgvList.Columns["clmComposerJ"].Index].Value = music.composerJ;
                row.Cells[dgvList.Columns["clmConverted"].Index].Value = music.converted;
                row.Cells[dgvList.Columns["clmNotes"].Index].Value = music.notes;
                row.Cells[dgvList.Columns["clmDuration"].Index].Value = music.duration;
                row.Cells[dgvList.Columns["clmVGMby"].Index].Value = music.vgmby;
                row.Cells[dgvList.Columns["clmSongNo"].Index].Value = music.songNo;

                ret.Add(row);
            }
            return ret;
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
            updatePlayingIndex(pi);
            playing = true;
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

            frmMain.loadAndPlay(m, songNo, fn, zfn);
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

            playing = false;

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

            frmMain.loadAndPlay(m, songNo, fn, zfn);
            updatePlayingIndex(e.RowIndex);

            playing = true;
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
                }
                else
                {
                    pl = PlayList.LoadM3U(ofd.FileName);
                    playing = false;
                    playList.lstMusic.Clear();
                    foreach (PlayList.music ms in pl.lstMusic)
                    {
                        AddList(ms.fileName);
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
            ofd.Filter = "VGMファイル(*.vgm;*.vgz;*.zip)|*.vgm;*.vgz;*.zip|"
                + "NRDファイル(*.nrd)|*.nrd|"
                + "XGMファイル(*.xgm)|*.xgm|"
                + "S98ファイル(*.s98)|*.s98|"
                + "NSFファイル(*.nsf)|*.nsf|"
                + "StandardMIDIファイル(*.mid)|*.mid|"
                + "RCPファイル(*.rcp)|*.rcp|"
                + "M3Uファイル(*.m3u)|*.m3u|"
                + "すべてのサポートファイル(*.vgm;*.vgz;*.zip;*.nrd;*.xgm;*.s98;*.nsf;*.mid;*.rcp;*.m3u)|"
                + "*.vgm;*.vgz;*.zip;*.nrd;*.xgm;*.s98;*.nsf;*.mid;*.rcp;*.m3u|"
                + "すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            ofd.FilterIndex=setting.other.FilterIndex;

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
                foreach (string fn in ofd.FileNames) AddList(fn);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

            //Play();
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
                    string ext1 = System.IO.Path.GetExtension(fn).ToUpper();
                    if (ext1 == ".VGM" || ext1 == ".VGZ" || ext1 == ".NRD" || ext1 == ".XGM")
                    {
                        AddList(fn);
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

                        AddList(fn);

                        if (fn.ToLower().LastIndexOf(".zip") == -1 && fn.ToLower().LastIndexOf(".m3u") == -1)
                        {
                            frmMain.loadAndPlay(0,0, fn);
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
    }
}
