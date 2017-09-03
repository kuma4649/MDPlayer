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
    public partial class frmYM2413 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        public frmYM2413(frmMain frm, int chipID, int zoom)
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

        private void frmYM2413_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmYM2413_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosYm2413[chipID] = Location;
            isClosed = true;
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2413.Width * zoom, frameSizeH + Properties.Resources.planeYM2413.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2413.Width * zoom, frameSizeH + Properties.Resources.planeYM2413.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2413.Width * zoom, frameSizeH + Properties.Resources.planeYM2413.Height * zoom);
            frmYM2413_Resize(null, null);

        }

        private void frmYM2413_Resize(object sender, EventArgs e)
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
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 11 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (ch == 9)
                {
                    int x = (px / 4 - 4);
                    if (x < 0) return;
                    x /= 15;
                    if (x > 4) return;
                    ch += x;
                }

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(enmUseChip.YM2413, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 14; ch++) parent.ResetChannelMask(enmUseChip.YM2413, chipID, ch);
                return;
            }

            //音色欄
            if (py < 15 * 8 && px < 16 * 8)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(enmUseChip.YM2413, 0, chipID);
            }
        }
    }
}
