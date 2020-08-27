using MDSound.np.chip;
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
    public partial class frmVRC6 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.VRC6 newParam = null;
        private MDChipParams.VRC6 oldParam = new MDChipParams.VRC6();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmVRC6(frmMain frm, int chipID, int zoom, MDChipParams.VRC6 newParam, MDChipParams.VRC6 oldParam)
        {
            InitializeComponent();

            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, Properties.Resources.planeVRC6, null, zoom);
            screenInit();
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

        private void frmVRC6_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosVrc6[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosVrc6[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmVRC6_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeVRC6.Width * zoom, frameSizeH + Properties.Resources.planeVRC6.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeVRC6.Width * zoom, frameSizeH + Properties.Resources.planeVRC6.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeVRC6.Width * zoom, frameSizeH + Properties.Resources.planeVRC6.Height * zoom);
            frmVRC6_Resize(null, null);

        }

        private void frmVRC6_Resize(object sender, EventArgs e)
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

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            //int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;
            if (ch < 0) return;

        }

        public void screenInit()
        {
            bool VRC6Type = false;
            int tp = VRC6Type ? 1 : 0;
            for (int ch = 0; ch < 3; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 16 + 8, kt, tp);
                }
            }
        }

        public void screenChangeParams()
        {
            TrackInfoBasic[] info = (TrackInfoBasic[])Audio.GetVRC6Register(0);
            if (info == null) return;

            MDChipParams.Channel nyc;

            for (int ch = 0; ch < 3; ch++)
            {
                nyc = newParam.channels[ch];
                nyc.kf = info[ch].GetTone();
                nyc.volumeR = info[ch].GetTone()/4;
                nyc.volumeL = info[ch].GetVolume();
                int v = info[ch].GetVolume();
                v = ch < 2 ? v * 2 : v / 3;
                nyc.volume = Math.Min(v, 19);
                nyc.bit[0] = info[ch].GetKeyStatus();
                nyc.freq = info[ch].GetFreqp();
                nyc.bit[1] = info[ch].GetHalt();
                v = info[ch].GetNote(info[ch].GetFreqHz())-4*12;
                nyc.note = nyc.volumeL == 0 ? -1 : v;
                nyc.sadr = info[ch].GetFreqShift();
            }

        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int ch = 0; ch < 3; ch++)
            {
                oyc = oldParam.channels[ch];
                nyc = newParam.channels[ch];

                DrawBuff.KeyBoard(frameBuffer, ch * 2, ref oyc.note, nyc.note, 0);
                if (ch < 2)
                {
                    DrawBuff.drawDuty(frameBuffer, 24, (1 + ch * 2) * 8, ref oyc.volumeR, nyc.volumeR);
                    DrawBuff.font4Int2(frameBuffer, 6 * 4, ch * 16 + 16, 0, 2, ref oyc.kf, nyc.kf);
                    DrawBuff.font4Int2(frameBuffer, 10 * 4, ch * 16 + 16, 0, 2, ref oyc.volumeL, nyc.volumeL);
                    DrawBuff.Volume(frameBuffer, 256, 8 + ch * 2 * 8, 0, ref oyc.volume, nyc.volume, 0);
                }
                else
                {
                    DrawBuff.font4Int2(frameBuffer, 9 * 4, ch * 16 + 16, 0, 3, ref oyc.volumeL, nyc.volumeL);
                    DrawBuff.Volume(frameBuffer, 256, 8 + ch * 2 * 8, 0, ref oyc.volume, nyc.volume, 0);
                    DrawBuff.drawNESSw(frameBuffer, 55 * 4, ch * 16 + 16
                        , ref oldParam.channels[ch].bit[1], newParam.channels[ch].bit[1]);
                    DrawBuff.font4Int1(frameBuffer, 62 * 4, ch * 16 + 16, 0, ref oyc.sadr, nyc.sadr);
                }

                DrawBuff.drawNESSw(frameBuffer, 13 * 4 , ch * 16 + 16
                    , ref oldParam.channels[ch].bit[0], newParam.channels[ch].bit[0]);

                DrawBuff.font4Hex12Bit(frameBuffer, 16*4 , ch * 16 + 16, 0, ref oyc.freq, nyc.freq);

            }
        }

    }
}
