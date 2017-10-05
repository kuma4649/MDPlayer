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
    public partial class frmC140 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.C140 newParam = null;
        private MDChipParams.C140 oldParam = new MDChipParams.C140();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmC140(frmMain frm,int chipID,int zoom, MDChipParams.C140 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeF, null, zoom);
            DrawBuff.screenInitC140(frameBuffer);
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

        private void frmC140_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosC140[chipID] = Location;
            isClosed = true;
        }

        private void frmC140_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeF.Width * zoom, frameSizeH + Properties.Resources.planeF.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeF.Width * zoom, frameSizeH + Properties.Resources.planeF.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeF.Width * zoom, frameSizeH + Properties.Resources.planeF.Height * zoom);
            frmC140_Resize(null, null);

        }

        private void frmC140_Resize(object sender, EventArgs e)
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
            MDSound.c140.c140_state c140State = Audio.GetC140Register(chipID);
            if (c140State != null)
            {
                for (int ch = 0; ch < 24; ch++)
                {
                    int frequency = c140State.REG[ch * 16 + 2] * 256 + c140State.REG[ch * 16 + 3];
                    int l = c140State.REG[ch * 16 + 1];
                    int r = c140State.REG[ch * 16 + 0];
                    int vdt = Math.Abs((int)c140State.voi[ch].prevdt);

                    if (c140State.voi[ch].key == 0) frequency = 0;
                    if (frequency == 0)
                    {
                        l = 0;
                        r = 0;
                    }

                    newParam.channels[ch].note = frequency == 0 ? -1 : (searchC140Note(frequency) + 1);
                    newParam.channels[ch].pan = ((l >> 2) & 0xf) | (((r >> 2) & 0xf) << 4);
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((l * vdt) >> 7, 0), 19);
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((r * vdt) >> 7, 0), 19);
                }
            }
        }

        public void screenDrawParams()
        {
            for (int c = 0; c < 24; c++)
            {

                MDChipParams.Channel orc = oldParam.channels[c];
                MDChipParams.Channel nrc = newParam.channels[c];

                DrawBuff.VolumeToC140(frameBuffer, c, 1, ref orc.volumeL, nrc.volumeL);
                DrawBuff.VolumeToC140(frameBuffer, c, 2, ref orc.volumeR, nrc.volumeR);
                DrawBuff.KeyBoardToC140(frameBuffer, c, ref orc.note, nrc.note);
                DrawBuff.PanType2(frameBuffer, c, ref orc.pan, nrc.pan);

                DrawBuff.ChC140(frameBuffer, c, ref orc.mask, nrc.mask, 0);
            }
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            //int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 24)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(enmUseChip.C140, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 24; ch++) parent.ResetChannelMask(enmUseChip.C140, chipID, ch);
                return;

            }

        }

        private int searchC140Note(int freq)
        {
            double m = double.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                double a = Math.Abs(freq - ((0x0800 << 2) * Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

    }
}
