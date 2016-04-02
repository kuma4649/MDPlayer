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

        public frmInfo()
        {
            InitializeComponent();
            update();
        }

        public void update()
        {
            lblTitle.Text = Audio.vgmTrackName;
            lblTitleJ.Text = Audio.vgmTrackNameJ;
            lblGame.Text = Audio.vgmGameName;
            lblGameJ.Text = Audio.vgmGameNameJ;
            lblSystem.Text = Audio.vgmSystemName;
            lblSystemJ.Text = Audio.vgmSystemNameJ;
            lblComposer.Text = Audio.vgmComposer;
            lblComposerJ.Text = Audio.vgmComposerJ;
            lblRelease.Text = Audio.vgmConverted;
            lblVGMBy.Text = Audio.vgmVGMBy;
            lblNotes.Text = Audio.vgmNotes;
            lblVersion.Text = Audio.vgmVersion;
            lblUsedChips.Text = Audio.vgmUsedChips;
        }

        private void frmInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            isClosed = true;
        }

        private void frmInfo_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }
    }
}
