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
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        public frmYM2608(frmMain frm,int chipID, int zoom)
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

        private void frmYM2608_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosYm2608[chipID] = Location;
            isClosed = true;
        }

        private void frmYM2608_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeD.Width * zoom, frameSizeH + Properties.Resources.planeD.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeD.Width * zoom, frameSizeH + Properties.Resources.planeD.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeD.Width * zoom, frameSizeH + Properties.Resources.planeD.Height * zoom);
            frmYM2608_Resize(null, null);

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
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 14)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(vgm.enmUseChip.YM2608,chipID, ch);
                    return;
                }

                for (ch = 0; ch < 14; ch++) parent.ResetChannelMask(vgm.enmUseChip.YM2608, chipID, ch);
                return;

            }

            // 音色表示欄の判定

            int h = (py - 15 * 8) / (6 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(vgm.enmUseChip.YM2608, instCh, chipID);
            }
        }
    }
}
