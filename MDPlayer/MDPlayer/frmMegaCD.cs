using System;
using System.Drawing;
using System.Windows.Forms;

namespace MDPlayer
{
    public partial class frmMegaCD : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;

        public frmMegaCD(frmMain frm)
        {
            parent = frm;
            InitializeComponent();

            update();
        }

        public void update()
        {
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmMegaCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PRf5c164 = Location;
            isClosed = true;
        }

        private void frmMegaCD_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

        }

        private void frmMegaCD_Resize(object sender, EventArgs e)
        {
        }

        protected override void WndProc(ref Message m)
        {
            if (parent != null)
            {
                parent.windowsMessage(ref m);
            }

            try { base.WndProc(ref m); }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public void screenChangeParams()
        {
        }

        public void screenDrawParams()
        {
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int ch = (e.Location.Y / 8) - 1;
            if (ch < 0) return;

            if (e.Button == MouseButtons.Left)
            {
                parent.SetChannelMask(vgm.enmUseChip.RF5C164, ch);
                return;
            }

            for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(vgm.enmUseChip.RF5C164, ch);
        }

        private void pbScreen_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void pbScreen_DragEnter(object sender, DragEventArgs e)
        {
        }
    }
}
