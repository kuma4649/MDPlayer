using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public frmMain frmMain = null;

        private PlayList playList = null;

        private bool playing = false;
        private int playIndex = -1;
        private int oldPlayIndex = -1;


        public frmPlayList()
        {
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

        public string setStart(int n)
        {
            updatePlayingIndex(n);

            string fn = playList.lstMusic[playIndex].fileName;

            return fn;
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

        private void frmPlayList_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }

        private void frmPlayList_Shown(object sender, EventArgs e)
        {
        }

        public new void Refresh()
        {
            dgvList.Rows.Clear();
            foreach (PlayList.music music in playList.lstMusic)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvList);
                row.Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
                row.Cells[dgvList.Columns["clmKey"].Index].Value = 0;
                row.Cells[dgvList.Columns["clmFileName"].Index].Value = music.fileName;
                row.Cells[dgvList.Columns["clmTitle"].Index].Value = music.title;
                row.Cells[dgvList.Columns["clmGame"].Index].Value = music.game;
                row.Cells[dgvList.Columns["clmRemark"].Index].Value = music.remark;

                dgvList.Rows.Add(row);
            }
        }

        public void AddList(string file)
        {
            PlayList.music music = Audio.getMusic(file, frmMain.getAllBytes(file));

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgvList);
            row.Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
            row.Cells[dgvList.Columns["clmKey"].Index].Value = 0;
            row.Cells[dgvList.Columns["clmFileName"].Index].Value = music.fileName;
            row.Cells[dgvList.Columns["clmTitle"].Index].Value = music.title;
            row.Cells[dgvList.Columns["clmGame"].Index].Value = music.game;
            row.Cells[dgvList.Columns["clmRemark"].Index].Value = music.remark;

            dgvList.Rows.Add(row);
            playList.lstMusic.Add(music);
            //updatePlayingIndex(dgvList.Rows.Count - 1);
        }

        private void frmPlayList_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosed = true;
            this.Visible = false;
            e.Cancel = true;
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
                cmsPlayList.Show();
                Point p = Control.MousePosition;
                cmsPlayList.Top = p.Y;
                cmsPlayList.Left = p.X;
            }
        }

        private void tsmiDelThis_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count < 1) return;

            if (oldPlayIndex >= dgvList.SelectedRows[0].Index)
            {
                oldPlayIndex--;
            }
            if (playIndex >= dgvList.SelectedRows[0].Index)
            {
                playIndex--;
            }
            playList.lstMusic.RemoveAt(dgvList.SelectedRows[0].Index);
            dgvList.Rows.RemoveAt(dgvList.SelectedRows[0].Index);
        }

        public void nextPlay()
        {
            if (!playing) return;
            if (dgvList.Rows.Count == playIndex + 1) return;

            int pi = playIndex;
            playing = false;

            pi++;

            string fn = (string)dgvList.Rows[pi].Cells["clmFileName"].Value;
            frmMain.loadAndPlay(fn);
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
            frmMain.loadAndPlay(fn);
            updatePlayingIndex(pi);
            playing = true;
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            playing = false;

            string fn = (string)dgvList.Rows[e.RowIndex].Cells["clmFileName"].Value;
            frmMain.loadAndPlay(fn);
            updatePlayingIndex(e.RowIndex);

            playing = true;
        }

        private void tsmiPlayThis_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count < 0) return;

            playing = false;

            string fn = (string)dgvList.Rows[dgvList.SelectedRows[0].Index].Cells["clmFileName"].Value;
            frmMain.loadAndPlay(fn);
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
    }
}
