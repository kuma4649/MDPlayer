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

        private void frmMegaCD_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            catch
            { }
        }

        public void screenChangeParams()
        {
        }

        public void screenDrawParams()
        {
        }


    }
}
