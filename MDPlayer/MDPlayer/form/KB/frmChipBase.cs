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

        protected MDChipParams.AY8910 newParam = null;
        protected MDChipParams.AY8910 oldParam = null;

        protected FrameBuffer frameBuffer = new FrameBuffer();

        public frmChipBase()
        {
            InitializeComponent();
        }

        public frmChipBase(frmMain frm, int chipID, int zoom, MDChipParams.AY8910 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;

            InitializeComponent();

        }

        private void frmChipBase_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void frmChipBase_Load(object sender, EventArgs e)
        {

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {

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
    }
}
