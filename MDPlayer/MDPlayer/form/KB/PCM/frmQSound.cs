using MDPlayer.Properties;
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
    public partial class frmQSound : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.QSound newParam = null;
        private MDChipParams.QSound oldParam = new MDChipParams.QSound();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmQSound(frmMain frm, int chipID, int zoom, MDChipParams.QSound newParam, MDChipParams.QSound oldParam)
        {
            InitializeComponent();

            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, Resources.planeQSound, null, zoom);
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

        private void frmQSound_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosQSound[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosQSound[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmQSound_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeQSound.Width * zoom, frameSizeH + Resources.planeQSound.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeQSound.Width * zoom, frameSizeH + Resources.planeQSound.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeQSound.Width * zoom, frameSizeH + Resources.planeQSound.Height * zoom);
            frmQSound_Resize(null, null);

        }

        private void frmQSound_Resize(object sender, EventArgs e)
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

        }

        private void screenInit()
        {
        }

        public void screenChangeParams()
        {
            ushort[] QSoundRegister = Audio.GetQSoundRegister(chipID);

            //PCM 16ch
            for (int ch = 0; ch < 16; ch++)
            {
                newParam.channels[ch].freq = QSoundRegister[(ch << 3) + 2];
                newParam.channels[ch].bank = QSoundRegister[(((ch + 15) % 16) << 3) + 0];
                newParam.channels[ch].sadr = QSoundRegister[(ch << 3) + 1];
                newParam.channels[ch].eadr = QSoundRegister[(ch << 3) + 5];
                newParam.channels[ch].ladr = QSoundRegister[(ch << 3) + 4];
            }
            //ADPCM 3ch
            for (int ch = 0; ch < 3; ch++)
            {
                newParam.channels[ch + 16].bank = QSoundRegister[(ch << 2) + 0xcc];
                newParam.channels[ch + 16].sadr = QSoundRegister[(ch << 2) + 0xca];
                newParam.channels[ch + 16].eadr = QSoundRegister[(ch << 2) + 0xcb];
            }
        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            //PCM 16ch
            for (int ch = 0; ch < 16; ch++)
            {
                oyc = oldParam.channels[ch];
                nyc = newParam.channels[ch];

                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 70, ch * 8 + 8, 0, ref oyc.freq, nyc.freq);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 75, ch * 8 + 8, 0, ref oyc.bank, nyc.bank);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 80, ch * 8 + 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 85, ch * 8 + 8, 0, ref oyc.eadr, nyc.eadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 90, ch * 8 + 8, 0, ref oyc.ladr, nyc.ladr);
            }
            //ADPCM 3ch
            for (int ch = 0; ch < 3; ch++)
            {
                oyc = oldParam.channels[ch + 16];
                nyc = newParam.channels[ch + 16];
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 75, (ch + 16) * 8 + 8, 0, ref oyc.bank, nyc.bank);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 80, (ch + 16) * 8 + 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 85, (ch + 16) * 8 + 8, 0, ref oyc.eadr, nyc.eadr);
            }
        }

    }
}
