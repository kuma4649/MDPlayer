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
    public partial class frmYM2612 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        public frmYM2612(frmMain frm, int chipID, int zoom)
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

        private void frmYM2612_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosYm2612[chipID] = Location;
            isClosed = true;
        }

        private void frmYM2612_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612.Width * zoom, frameSizeH + Properties.Resources.planeYM2612.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612.Width * zoom, frameSizeH + Properties.Resources.planeYM2612.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612.Width * zoom, frameSizeH + Properties.Resources.planeYM2612.Height * zoom);
            frmYM2612_Resize(null, null);

        }

        private void frmYM2612_Resize(object sender, EventArgs e)
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

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 10 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(vgm.enmUseChip.YM2612, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 9; ch++) parent.ResetChannelMask(vgm.enmUseChip.YM2612, chipID, ch);
                return;
            }

            //音色で右クリックした場合は何もしない
            if (e.Button == MouseButtons.Right) return;

            int px = e.Location.X / zoom;

            // 音色表示欄の判定
            int h = (py - 10 * 8) / (6 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 3)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(vgm.enmUseChip.YM2612, instCh, chipID);
            }

        }
    }
}
