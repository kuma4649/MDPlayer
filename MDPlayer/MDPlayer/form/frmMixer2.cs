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
    public partial class frmMixer2 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int zoom = 1;

        public frmMixer2(frmMain frm, int zoom)
        {
            parent = frm;
            this.zoom = zoom;

            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.frmMixer2_MouseWheel);

            update();
        }

        private void frmMixer2_MouseWheel(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
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



        private void frmMixer2_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosMixer = Location;
            isClosed = true;
        }

        private void frmMixer2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMixer.Width * zoom, frameSizeH + Properties.Resources.planeMixer.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMixer.Width * zoom, frameSizeH + Properties.Resources.planeMixer.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMixer.Width * zoom, frameSizeH + Properties.Resources.planeMixer.Height * zoom);
            frmMixer2_Resize(null, null);

        }

        private void frmMixer2_Resize(object sender, EventArgs e)
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
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;


        }

        private void frmMixer2_KeyDown(object sender, KeyEventArgs e)
        {

        }

    }
}
