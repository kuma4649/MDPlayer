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
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        public frmMegaCD(frmMain frm,int chipID,int zoom)
        {
            this.chipID = chipID;
            this.zoom = zoom;
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
            parent.setting.location.PosRf5c164[chipID] = Location;
            isClosed = true;
        }

        private void frmMegaCD_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeC.Width * zoom, frameSizeH + Properties.Resources.planeC.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeC.Width * zoom, frameSizeH + Properties.Resources.planeC.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeC.Width * zoom, frameSizeH + Properties.Resources.planeC.Height * zoom);
            frmMegaCD_Resize(null, null);

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
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;
            if (ch < 0) return;

            if (e.Button == MouseButtons.Left)
            {
                parent.SetChannelMask(enmUseChip.RF5C164, chipID, ch);
                return;
            }

            for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(enmUseChip.RF5C164, chipID, ch);
        }

        private void pbScreen_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void pbScreen_DragEnter(object sender, DragEventArgs e)
        {
        }
    }
}
