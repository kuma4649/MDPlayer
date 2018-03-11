using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public int lyricsIndex=0;

        public frmInfo(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
            update();
        }

        public void update()
        {
            lblTitle.Text = "";
            lblTitleJ.Text = "";
            lblGame.Text = "";
            lblGameJ.Text = "";
            lblSystem.Text = "";
            lblSystemJ.Text = "";
            lblComposer.Text = "";
            lblComposerJ.Text = "";
            lblRelease.Text = "";
            lblVGMBy.Text = "";
            lblNotes.Text = "";
            lblVersion.Text = "";
            lblUsedChips.Text = "";
            lblLyrics.Text = "";
            lyricsIndex = 0;

            GD3 gd3 = Audio.GetGD3();
            if (gd3 == null) return;

            lblTitle.Text = gd3.TrackName;
            lblTitleJ.Text = gd3.TrackNameJ;
            lblGame.Text = gd3.GameName;
            lblGameJ.Text = gd3.GameNameJ;
            lblSystem.Text = gd3.SystemName;
            lblSystemJ.Text = gd3.SystemNameJ;
            lblComposer.Text = gd3.Composer;
            lblComposerJ.Text = gd3.ComposerJ;
            lblRelease.Text = gd3.Converted;
            lblVGMBy.Text = gd3.VGMBy;
            lblNotes.Text = gd3.Notes;
            lblVersion.Text = gd3.Version;
            lblUsedChips.Text = gd3.UsedChips;

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

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            isClosed = true;
            setting.location.PInfo = this.Location;
        }

        private void frmInfo_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }

        protected override void WndProc(ref Message m)
        {
            if (parent != null)
            {
                parent.windowsMessage(ref m);
            }

            base.WndProc(ref m);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (lyrics == null) return;

            long cnt=Audio.GetDriverCounter();

            if (cnt >= lyrics[lyricsIndex].Item1)
            {

                lblLyrics.Text = lyrics[lyricsIndex].Item3;
                lyricsIndex++;

                if (lyricsIndex == lyrics.Count)
                {
                    timer.Enabled = false;
                }
            }
        }
    }
}
