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
    public partial class frmOKIM6258 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.OKIM6258 newParam = null;
        private MDChipParams.OKIM6258 oldParam = new MDChipParams.OKIM6258();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmOKIM6258(frmMain frm, int chipID, int zoom, MDChipParams.OKIM6258 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeMSM6258, null, zoom);
            DrawBuff.screenInitOKIM6258(frameBuffer);
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

        private void frmOKIM6258_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosOKIM6258[chipID] = Location;
            isClosed = true;
        }

        private void frmOKIM6258_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMSM6258.Width * zoom, frameSizeH + Properties.Resources.planeMSM6258.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMSM6258.Width * zoom, frameSizeH + Properties.Resources.planeMSM6258.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMSM6258.Width * zoom, frameSizeH + Properties.Resources.planeMSM6258.Height * zoom);
            frmOKIM6258_Resize(null, null);

        }

        private void frmOKIM6258_Resize(object sender, EventArgs e)
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
            MDSound.okim6258.okim6258_state okim6258State = Audio.GetOKIM6258Register(chipID);
            if (okim6258State == null) return;

            switch (okim6258State.pan & 0x3)
            {
                case 0:
                case 3:
                    newParam.pan = 3;
                    break;
                case 1:
                    newParam.pan = 2;
                    break;
                case 2:
                    newParam.pan = 1;
                    break;
            }

            newParam.masterFreq = (int)okim6258State.master_clock / 1000;
            newParam.divider = (int)okim6258State.divider;
            if (okim6258State.divider == 0) newParam.pbFreq = 0;
            else newParam.pbFreq = (int)(okim6258State.master_clock / okim6258State.divider / 1000);

            int v = (int)(((Math.Abs(okim6258State.data_in - 128) * 2) >> 3) * 1.2);
            if ((okim6258State.status & 0x2) == 0) v = 0;
            v = Math.Min(v, 38);
            if (newParam.volumeL < v && ((newParam.pan & 0x2) != 0))
            {
                newParam.volumeL = v;
            }
            else
            {
                newParam.volumeL--;
            }
            if (newParam.volumeR < v && ((newParam.pan & 0x1) != 0))
            {
                newParam.volumeR = v;
            }
            else
            {
                newParam.volumeR--;
            }
        }


        public void screenDrawParams()
        {
            MDChipParams.OKIM6258 ost = oldParam;
            MDChipParams.OKIM6258 nst = newParam;

            DrawBuff.PanToOKIM6258(frameBuffer, ref ost.pan, nst.pan, ref ost.pantp, 0);

            if (ost.masterFreq != nst.masterFreq)
            {
                DrawBuff.drawFont4(frameBuffer, 12 * 4, 8, 0, string.Format("{0:d5}", nst.masterFreq));
                ost.masterFreq = nst.masterFreq;
            }

            if (ost.divider != nst.divider)
            {
                DrawBuff.drawFont4(frameBuffer, 19 * 4, 8, 0, string.Format("{0:d5}", nst.divider));
                ost.divider = nst.divider;
            }

            if (ost.pbFreq != nst.pbFreq)
            {
                DrawBuff.drawFont4(frameBuffer, 26 * 4, 8, 0, string.Format("{0:d5}", nst.pbFreq));
                ost.pbFreq = nst.pbFreq;
            }

            DrawBuff.Volume(frameBuffer, 0, 1, ref ost.volumeL, nst.volumeL / 2, 0);
            DrawBuff.Volume(frameBuffer, 0, 2, ref ost.volumeR, nst.volumeR / 2, 0);

            DrawBuff.ChOKIM6258(frameBuffer, ref ost.mask, nst.mask, 0);

        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 2 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(enmUseChip.OKIM6258, chipID, 0);
                    return;
                }

                //マスク解除
                parent.ResetChannelMask(enmUseChip.OKIM6258, chipID, 0);
                return;
            }

        }
    }
}
