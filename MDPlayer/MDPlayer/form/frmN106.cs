using MDPlayer.Properties;
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
    public partial class frmN106 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.N106 newParam = null;
        private MDChipParams.N106 oldParam = new MDChipParams.N106();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmN106(frmMain frm, int chipID, int zoom, MDChipParams.N106 newParam, MDChipParams.N106 oldParam)
        {
            InitializeComponent();

            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, Resources.planeN106, null, zoom);
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

        private void frmN106_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosN106[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosN106[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmN106_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeN106.Width * zoom, frameSizeH + Resources.planeN106.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeN106.Width * zoom, frameSizeH + Resources.planeN106.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeN106.Width * zoom, frameSizeH + Resources.planeN106.Height * zoom);
            frmN106_Resize(null, null);

        }

        private void frmN106_Resize(object sender, EventArgs e)
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
            bool N106Type = false;
            int tp = N106Type ? 1 : 0;
            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 24 + 8, kt, tp);
                }
            }
        }

        public void screenChangeParams()
        {
            TrackInfoN106[] info = (TrackInfoN106[])Audio.GetN106Register(0);
            if (info == null) return;

            MDChipParams.Channel nyc;

            for (int ch = 0; ch < 8; ch++)
            {
                nyc = newParam.channels[ch];

                nyc.bit[0] = info[ch].GetKeyStatus();
                nyc.bit[1] = info[ch].GetHalt();

                int v = info[ch].GetVolume() * 2;
                nyc.volume = Math.Min(v, 19);
                nyc.volumeR = info[ch].GetVolume();

                nyc.freq = (int)info[ch].GetFreq();
                v = info[ch].GetNote(info[ch].GetFreqHz()) - 4 * 12;
                nyc.note = (nyc.volumeL == 0 || !nyc.bit[0]) ? -1 : v;

                nyc.bank = info[ch].wavelen & 127;
                nyc.bank = nyc.bank < 0 ? 0 : nyc.bank;
                if (nyc.aryWave16bit == null) nyc.aryWave16bit = new short[280];
                for (int i = 0; i < 280; i++)
                {
                    if (i < nyc.bank)
                    {
                        nyc.aryWave16bit[i] = info[ch].wave[i];
                    }
                    else
                    {
                        if (i != 279) nyc.aryWave16bit[i] = nyc.aryWave16bit[i + 1];
                        else
                        {
                            ushort w = (ushort)(((sbyte)info[ch].GetOutput() >> 4) + 8);
                            nyc.aryWave16bit[i] = (short)(w + 16);
                        }
                    }
                }
            }
        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int ch = 0; ch < 8; ch++)
            {
                oyc = oldParam.channels[ch];
                nyc = newParam.channels[ch];

                //Enable
                DrawBuff.drawNESSw(frameBuffer, 6 * 4, ch * 24 + 8
                    , ref oldParam.channels[ch].bit[1], newParam.channels[ch].bit[1]);

                //Key
                DrawBuff.drawNESSw(frameBuffer, 7 * 4, ch * 24 + 8
                    , ref oldParam.channels[ch].bit[0], newParam.channels[ch].bit[0]);

                //vol
                DrawBuff.Volume(frameBuffer, 256, 8 + ch * 3 * 8, 0, ref oyc.volume, nyc.volume, 0);
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 4, ch * 24 + 16, 0, ref oyc.volumeR, nyc.volumeR);

                //freq
                DrawBuff.font4Hex20Bit(frameBuffer, 4 * 4, ch * 24 + 24, 0, ref oyc.freq, nyc.freq);

                //Note
                DrawBuff.KeyBoard(frameBuffer, ch * 3, ref oyc.note, nyc.note, 0);

                if (oyc.aryWave16bit == null && nyc.aryWave16bit!=null) oyc.aryWave16bit = new short[nyc.aryWave16bit.Length];
                DrawBuff.WaveFormToN106(frameBuffer, 10 * 4, ch * 24 + 16, ref oyc.aryWave16bit, nyc.aryWave16bit);

            }
        }

    }
}
