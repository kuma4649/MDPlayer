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
    public partial class frmYM2608 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;

        public frmYM2608(frmMain frm)
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

        private void frmYM2608_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PYm2608 = Location;
            isClosed = true;
        }

        private void frmYM2608_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }

        private void frmYM2608_Resize(object sender, EventArgs e)
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

            if (ch < 14)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(vgm.enmUseChip.YM2608, ch);
                    return;
                }

                for (ch = 0; ch < 14; ch++) parent.ResetChannelMask(vgm.enmUseChip.YM2608, ch);
                return;

            }

            // 音色表示欄の判定

            int h = (e.Location.Y - 15 * 8) / (6 * 8);
            int w = Math.Min(e.Location.X / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(vgm.enmUseChip.YM2608, instCh);
            }
        }
    }
}
