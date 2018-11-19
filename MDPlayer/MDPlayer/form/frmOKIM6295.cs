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
    public partial class frmOKIM6295 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.OKIM6295 newParam = null;
        private MDChipParams.OKIM6295 oldParam = new MDChipParams.OKIM6295();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmOKIM6295(frmMain frm, int chipID, int zoom,MDChipParams.OKIM6295 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeMSM6295, null, zoom);
            DrawBuff.screenInitOKIM6295(frameBuffer);
            update();
        }

        public void update()
        {
            frameBuffer.Refresh(null);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmOKIM6295_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosOKIM6295[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosOKIM6295[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmOKIM6295_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMSM6295.Width * zoom, frameSizeH + Properties.Resources.planeMSM6295.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMSM6295.Width * zoom, frameSizeH + Properties.Resources.planeMSM6295.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMSM6295.Width * zoom, frameSizeH + Properties.Resources.planeMSM6295.Height * zoom);
            frmOKIM6295_Resize(null, null);

        }

        private void frmOKIM6295_Resize(object sender, EventArgs e)
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

        public void screenInit()
        {
        }

    }
}
