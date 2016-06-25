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
    public partial class frmInfo : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;

        public frmInfo(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
            update();
        }

        public void update()
        {
            lblTitle.Text = Audio.GD3.TrackName;
            lblTitleJ.Text = Audio.GD3.TrackNameJ;
            lblGame.Text = Audio.GD3.GameName;
            lblGameJ.Text = Audio.GD3.GameNameJ;
            lblSystem.Text = Audio.GD3.SystemName;
            lblSystemJ.Text = Audio.GD3.SystemNameJ;
            lblComposer.Text = Audio.GD3.Composer;
            lblComposerJ.Text = Audio.GD3.ComposerJ;
            lblRelease.Text = Audio.GD3.Converted;
            lblVGMBy.Text = Audio.GD3.VGMBy;
            lblNotes.Text = Audio.GD3.Notes;
            lblVersion.Text = Audio.vgmVersion;
            lblUsedChips.Text = Audio.vgmUsedChips;
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

    }
}
