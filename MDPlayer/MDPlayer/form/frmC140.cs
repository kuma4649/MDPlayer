using System;
using System.Drawing;
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

        private void frmC140_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosC140[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosC140[chipID] = RestoreBounds.Location;
            }
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
                    parent.SetChannelMask( EnmChip.C140, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 24; ch++) parent.ResetChannelMask(EnmChip.C140, chipID, ch);
                return;

            }

        }

        private int searchC140Note(int freq)
        {
            double m = double.MaxValue;

            int clock = Audio.clockC140;
            if (clock >= 1000000)
                clock = (int)clock / 384;

            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                //double a = Math.Abs(freq - ((0x0800 << 2) * Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                int a = (int)(
                    65536.0 
                    / 2.0 
                    / clock 
                    * 8000.0
                    * Tables.pcmMulTbl[i % 12 + 12]
                    * Math.Pow(2, (i / 12 - 3))
                    );
                if (freq > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }


        public void screenInit()
        {
            bool C140Type = (chipID == 0) ? parent.setting.C140Type.UseScci : parent.setting.C140SType.UseScci;
            int tp = C140Type ? 1 : 0;
            for (int ch = 0; ch < 24; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 8 + 8, kt, tp);
                }
                DrawBuff.drawFont8(frameBuffer, 396, ch * 8 + 8, 1, "   ");
                DrawBuff.drawPanType2P(frameBuffer, 24, ch * 8 + 8, 0, tp);
                DrawBuff.ChC140_P(frameBuffer, 0, 8 + ch * 8, ch, false, tp);
                int d = 99;
                DrawBuff.VolumeToC140(frameBuffer, ch, 1, ref d,0, tp);
                d = 99;
                DrawBuff.VolumeToC140(frameBuffer, ch, 2, ref d,0, tp);
            }
        }

        public void screenChangeParams()
        {
            byte[] c140State = Audio.GetC140Register(chipID);
            bool[] c140KeyOn = Audio.GetC140KeyOn(chipID);
            if (c140State != null)
            {
                for (int ch = 0; ch < 24; ch++)
                {
                    int frequency = c140State[ch * 16 + 2] * 256 + c140State[ch * 16 + 3];
                    int l = c140State[ch * 16 + 1];
                    int r = c140State[ch * 16 + 0];

                    newParam.channels[ch].note = searchC140Note(frequency) + 1;
                    if (c140KeyOn[ch])
                    {
                        newParam.channels[ch].volumeL = Math.Min(Math.Max((int)(l / 13.4) * 3, 0), 19);
                        newParam.channels[ch].volumeR = Math.Min(Math.Max((int)(r / 13.4) * 3, 0), 19);
                    }
                    else
                    {
                        newParam.channels[ch].volumeL -= newParam.channels[ch].volumeL > 0 ? 1 : 0;
                        newParam.channels[ch].volumeR -= newParam.channels[ch].volumeR > 0 ? 1 : 0;
                        if (newParam.channels[ch].volumeL == 0 && newParam.channels[ch].volumeR == 0)
                        {
                            if (c140State[ch * 16 + 5] == 0)
                            {
                                newParam.channels[ch].note = -1;
                            }
                            newParam.channels[ch].volumeL = 0;
                            newParam.channels[ch].volumeR = 0;
                        }
                    }
                    newParam.channels[ch].pan = ((l >> 4) & 0xf) | (((r >> 4) & 0xf) << 4);

                    c140KeyOn[ch] = false;

                    newParam.channels[ch].freq = (c140State[ch * 16 + 2] << 8) | c140State[ch * 16 + 3];
                    newParam.channels[ch].bank = c140State[ch * 16 + 4];
                    byte d = c140State[ch * 16 + 5];
                    newParam.channels[ch].bit[0] = (d & 0x10) != 0;
                    newParam.channels[ch].bit[1] = (d & 0x08) != 0;
                    newParam.channels[ch].sadr = (c140State[ch * 16 + 6] << 8) | c140State[ch * 16 + 7];
                    newParam.channels[ch].eadr = (c140State[ch * 16 + 8] << 8) | c140State[ch * 16 + 9];
                    newParam.channels[ch].ladr = (c140State[ch * 16 + 10] << 8) | c140State[ch * 16 + 11];

                }
            }
        }

        public void screenDrawParams()
        {
            int tp = ((chipID == 0) ? parent.setting.C140Type.UseScci : parent.setting.C140SType.UseScci) ? 1 : 0;

            for (int c = 0; c < 24; c++)
            {

                MDChipParams.Channel orc = oldParam.channels[c];
                MDChipParams.Channel nrc = newParam.channels[c];

                DrawBuff.VolumeToC140(frameBuffer, c, 1, ref orc.volumeL, nrc.volumeL, tp);
                DrawBuff.VolumeToC140(frameBuffer, c, 2, ref orc.volumeR, nrc.volumeR, tp);
                DrawBuff.KeyBoardToC140(frameBuffer, c, ref orc.note, nrc.note, tp);
                DrawBuff.PanType2(frameBuffer, c, ref orc.pan, nrc.pan, tp);

                DrawBuff.ChC140(frameBuffer, c, ref orc.mask, nrc.mask, tp);

                DrawBuff.drawNESSw(frameBuffer, 64 * 4, c * 8 + 8, ref oldParam.channels[c].bit[0], newParam.channels[c].bit[0]);
                DrawBuff.drawNESSw(frameBuffer, 65 * 4, c * 8 + 8, ref oldParam.channels[c].bit[1], newParam.channels[c].bit[1]);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 67, c * 8 + 8, 0, ref orc.freq, nrc.freq);
                DrawBuff.font4HexByte(frameBuffer, 4 * 72, c * 8 + 8, 0, ref orc.bank, nrc.bank);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 75, c * 8 + 8, 0, ref orc.sadr, nrc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 80, c * 8 + 8, 0, ref orc.eadr, nrc.eadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 85, c * 8 + 8, 0, ref orc.ladr, nrc.ladr);
            }
        }

    }
}
