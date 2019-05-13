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
    public partial class frmChipBase : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        protected int frameSizeW = 0;
        protected int frameSizeH = 0;
        protected int chipID = 0;
        protected int zoom = 1;

        protected FrameBuffer frameBuffer = new FrameBuffer();

        public frmChipBase()
        {
            InitializeComponent();
            Opened = false;
        }

        public frmChipBase(frmMain frm, int chipID, int zoom)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

        }

        private void frmChipBase_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void frmChipBase_Load(object sender, EventArgs e)
        {
            this.Opacity = parent.setting.other.Opacity / 100.0;
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {

        }


        public virtual void update()
        {
            //while (!Opened) { Application.DoEvents(); }
            frameBuffer.Refresh(null);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        public bool Opened { get; internal set; }

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

        virtual public void screenChangeParams() { }
        virtual public void screenDrawParams() { }
        virtual public void screenInit() { }

        private void FrmChipBase_Shown(object sender, EventArgs e)
        {
            Opened = true;
        }
    }
}
