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
    public partial class frmSegaPCM : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        public frmSegaPCM(frmMain frm, int chipID, int zoom)
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

        private void frmSegaPCM_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosSegaPCM[chipID] = Location;
            isClosed = true;
        }

        private void frmSegaPCM_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeSEGAPCM.Width * zoom, frameSizeH + Properties.Resources.planeSEGAPCM.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeSEGAPCM.Width * zoom, frameSizeH + Properties.Resources.planeSEGAPCM.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeSEGAPCM.Width * zoom, frameSizeH + Properties.Resources.planeSEGAPCM.Height * zoom);
            frmSegaPCM_Resize(null, null);

        }

        private void frmSegaPCM_Resize(object sender, EventArgs e)
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
    }
}
