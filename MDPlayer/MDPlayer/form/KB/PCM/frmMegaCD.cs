using System;
using System.Drawing;
using System.Windows.Forms;
using MDPlayer.Properties;

namespace MDPlayer.form
{
    public partial class frmMegaCD : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.RF5C164 newParam = null;
        private MDChipParams.RF5C164 oldParam = new MDChipParams.RF5C164();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmMegaCD(frmMain frm, int chipID, int zoom, MDChipParams.RF5C164 newParam, MDChipParams.RF5C164 oldParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, Resources.planeC, null, zoom);
            DrawBuff.screenInitRF5C164(frameBuffer);
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

        private void frmMegaCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosRf5c164[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosRf5c164[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmMegaCD_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeC.Width * zoom, frameSizeH + Resources.planeC.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeC.Width * zoom, frameSizeH + Resources.planeC.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeC.Width * zoom, frameSizeH + Resources.planeC.Height * zoom);
            frmMegaCD_Resize(null, null);

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
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public void screenChangeParams()
        {
            MDSound.scd_pcm.pcm_chip_ rf5c164Register = Audio.GetRf5c164Register(chipID);
            if (rf5c164Register != null)
            {
                //int[][] rf5c164Vol = Audio.GetRf5c164Volume(chipID);
                for (int ch = 0; ch < 8; ch++)
                {
                    if (rf5c164Register.Channel[ch].Enable != 0)
                    {
                        newParam.channels[ch].note = searchRf5c164Note(rf5c164Register.Channel[ch].Step_B);
                        //newParam.channels[ch].volumeL = Math.Min(Math.Max(rf5c164Vol[ch][0] / 400, 0), 19);
                        //newParam.channels[ch].volumeR = Math.Min(Math.Max(rf5c164Vol[ch][1] / 400, 0), 19);
                        newParam.channels[ch].volumeL = Math.Min(Math.Max((int)rf5c164Register.Channel[ch].MUL_L / 3, 0), 19);
                        newParam.channels[ch].volumeR = Math.Min(Math.Max((int)rf5c164Register.Channel[ch].MUL_R / 3, 0), 19);
                    }
                    else
                    {
                        newParam.channels[ch].note = -1;
                        newParam.channels[ch].volumeL = 0;
                        newParam.channels[ch].volumeR = 0;
                    }
                    newParam.channels[ch].pan = (int)rf5c164Register.Channel[ch].PAN;
                }
            }

        }

        public void screenDrawParams()
        {
            for (int c = 0; c < 8; c++)
            {

                MDChipParams.Channel orc = oldParam.channels[c];
                MDChipParams.Channel nrc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref orc.volumeL, nrc.volumeL, 0);
                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref orc.volumeR, nrc.volumeR, 0);
                DrawBuff.KeyBoard(frameBuffer, c, ref orc.note, nrc.note, 0);
                DrawBuff.PanType2(frameBuffer, c, ref orc.pan, nrc.pan,0);
                DrawBuff.ChRF5C164(frameBuffer, c, ref orc.mask, nrc.mask, 0);

            }
        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            int ch;
            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (ch = 0; ch < 8; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.RF5C164, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.RF5C164, chipID, ch);
                    }
                }
                return;
            }

            ch = (py / 8) - 1;
            if (ch < 0) return;

            if (e.Button == MouseButtons.Left)
            {
                parent.SetChannelMask(EnmChip.RF5C164, chipID, ch);
                return;
            }

            for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(EnmChip.RF5C164, chipID, ch);
        }

        private int searchRf5c164Note(uint freq)
        {
            double m = double.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                double a = Math.Abs(freq - (0x0800 * Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
                newParam.channels[c].volumeL = -1;
                newParam.channels[c].volumeR = -1;
                newParam.channels[c].pan = -1;
            }
        }
    }
}
