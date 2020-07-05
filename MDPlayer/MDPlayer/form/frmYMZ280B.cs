using System;
using System.Drawing;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class frmYMZ280B : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.YMZ280B newParam = null;
        private MDChipParams.YMZ280B oldParam = new MDChipParams.YMZ280B();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYMZ280B(frmMain frm, int chipID, int zoom, MDChipParams.YMZ280B newParam, MDChipParams.YMZ280B oldParam)
        {
            InitializeComponent();

            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, Properties.Resources.planeYMZ280B, null, zoom);
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

        private void frmYMZ280B_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYMZ280B[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYMZ280B[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYMZ280B_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYMZ280B.Width * zoom, frameSizeH + Properties.Resources.planeYMZ280B.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYMZ280B.Width * zoom, frameSizeH + Properties.Resources.planeYMZ280B.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYMZ280B.Width * zoom, frameSizeH + Properties.Resources.planeYMZ280B.Height * zoom);
            frmYMZ280B_Resize(null, null);

        }

        private void frmYMZ280B_Resize(object sender, EventArgs e)
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

            if (ch < 8)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.YMZ280B, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(EnmChip.YMZ280B, chipID, ch);
                return;

            }

        }


        public void screenInit()
        {
        }

        public void screenChangeParams()
        {
            int[] reg = Audio.GetYMZ280BRegister(chipID);
            if (reg == null) return;

            for (int ch = 0; ch < 8; ch++)
            {
                newParam.channels[ch].freq = (byte)reg[0x0 + ch * 4]
                    + ((reg[0x1 + ch * 4]&1) << 8);
                newParam.channels[ch].nfrq = (byte)reg[0x2 + ch * 4];
                newParam.channels[ch].pan = (byte)(reg[0x3 + ch * 4] & 0xf);
                newParam.channels[ch].sadr = ((byte)reg[0x20 + ch * 4] << 16)
                    + ((byte)reg[0x40 + ch * 4] << 8)
                    + (byte)reg[0x60 + ch * 4];
                newParam.channels[ch].ladr = ((byte)reg[0x21 + ch * 4] << 16)
                    + ((byte)reg[0x41 + ch * 4] << 8)
                    + (byte)reg[0x61 + ch * 4];
                newParam.channels[ch].leadr = ((byte)reg[0x22 + ch * 4] << 16)
                    + ((byte)reg[0x42 + ch * 4] << 8)
                    + (byte)reg[0x62 + ch * 4];
                newParam.channels[ch].eadr = ((byte)reg[0x23 + ch * 4] << 16)
                    + ((byte)reg[0x43 + ch * 4] << 8)
                    + (byte)reg[0x63 + ch * 4];

                newParam.channels[ch].dda = (reg[0x1 + ch * 4] & 0x80) != 0;
                newParam.channels[ch].ex = (reg[0x1 + ch * 4] & 0x40) != 0;
                newParam.channels[ch].noise = (reg[0x1 + ch * 4] & 0x20) != 0;
                newParam.channels[ch].loopFlg = (reg[0x1 + ch * 4] & 0x10) != 0;
            }
        }

        public void screenDrawParams()
        {
            for (int ch = 0; ch < 8; ch++)
            {
                MDChipParams.Channel orc = oldParam.channels[ch];
                MDChipParams.Channel nrc = newParam.channels[ch];

                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 7, ch * 8 + 8, 0, ref orc.pan, nrc.pan);
                DrawBuff.drawNESSw(frameBuffer, 4 * 08, ch * 8 + 8, ref orc.dda, nrc.dda);
                DrawBuff.drawNESSw(frameBuffer, 4 * 09, ch * 8 + 8, ref orc.ex, nrc.ex);
                DrawBuff.drawNESSw(frameBuffer, 4 * 10, ch * 8 + 8, ref orc.noise, nrc.noise);
                DrawBuff.drawNESSw(frameBuffer, 4 * 11, ch * 8 + 8, ref orc.loopFlg, nrc.loopFlg);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 13, ch * 8 + 8, 0, ref orc.sadr, nrc.sadr);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 20, ch * 8 + 8, 0, ref orc.ladr, nrc.ladr);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 27, ch * 8 + 8, 0, ref orc.leadr, nrc.leadr);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 34, ch * 8 + 8, 0, ref orc.eadr, nrc.eadr);
                DrawBuff.font4Hex12Bit(frameBuffer, 4 * 41, ch * 8 + 8, 0, ref orc.freq, nrc.freq);//PITCH
                DrawBuff.font4HexByte(frameBuffer, 4 * 45, ch * 8 + 8, 0, ref orc.nfrq, nrc.nfrq);//TL
            }
        }


    }
}
