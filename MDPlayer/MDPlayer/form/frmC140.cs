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
    public partial class frmC140 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        public frmC140(frmMain frm,int chipID,int zoom)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
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

        private void frmC140_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosC140[chipID] = Location;
            isClosed = true;
        }

        private void frmC140_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeF.Width * zoom, frameSizeH + Properties.Resources.planeF.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeF.Width * zoom, frameSizeH + Properties.Resources.planeF.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeF.Width * zoom, frameSizeH + Properties.Resources.planeF.Height * zoom);
            frmC140_Resize(null, null);

        }

        private void frmC140_Resize(object sender, EventArgs e)
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
            //int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 24)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(enmUseChip.C140, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 24; ch++) parent.ResetChannelMask(enmUseChip.C140, chipID, ch);
                return;

            }

        }
    }
}
