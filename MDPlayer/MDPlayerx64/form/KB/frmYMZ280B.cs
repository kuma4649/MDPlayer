#if X64
using MDPlayerx64;
using System.Numerics;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmYMZ280B : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.YMZ280B newParam = null;
        private MDChipParams.YMZ280B oldParam = new MDChipParams.YMZ280B();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYMZ280B(frmMain frm, int chipID, int zoom, MDChipParams.YMZ280B newParam, MDChipParams.YMZ280B oldParam) : base(frm)
        {
            InitializeComponent();

            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeYMZ280B"], null, zoom);
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
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYMZ280B"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYMZ280B"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYMZ280B"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYMZ280B"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYMZ280B"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYMZ280B"].Height * zoom);
            frmYMZ280B_Resize(null, null);

        }

        private void frmYMZ280B_Resize(object sender, EventArgs e)
        {

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
                            parent.ResetChannelMask(EnmChip.YMZ280B, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.YMZ280B, chipID, ch);
                    }
                }
                return;
            }

            ch = (py / 8) - 1;
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
            for (int ch = 0; ch < 8; ch++)
            {
                MDChipParams.Channel nrc = newParam.channels[ch];
                MDChipParams.Channel orc = oldParam.channels[ch];
                nrc.volumeL = 0;
                nrc.volumeR = 0;
                nrc.panL = 4;
                nrc.panR = 4;
                orc.panL = -1;
                orc.panR = -1;

                DrawBuff.drawFont8(frameBuffer, 4 * 78 + 1, ch * 8 + 8, 1, "   ");

                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 8 * 4 + kx + 1, ch * 8 + 8, kt, 0);
                }

            }
        }

        public void screenChangeParams()
        {
            int[] reg = Audio.GetYMZ280BRegister(chipID);
            if (reg == null) return;

            for (int ch = 0; ch < 8; ch++)
            {
                MDChipParams.Channel nrc = newParam.channels[ch];

                nrc.freq = (byte)reg[0x0 + ch * 4]
                    + ((reg[0x1 + ch * 4] & 1) << 8);
                nrc.nfrq = (byte)reg[0x2 + ch * 4];//tl

                nrc.pan = (byte)(reg[0x3 + ch * 4] & 0xf);
                nrc.panL = nrc.pan == 8 ? 4 : (nrc.pan < 8 ? 4 : (4 * (15 - nrc.pan) / 7));
                nrc.panR = nrc.pan == 8 ? 4 : (nrc.pan < 8 ? ((nrc.pan == 0) ? 0 : 4 * (nrc.pan - 1) / 7) : 4);

                nrc.sadr = ((byte)reg[0x20 + ch * 4] << 16)
                    + ((byte)reg[0x40 + ch * 4] << 8)
                    + (byte)reg[0x60 + ch * 4];
                nrc.ladr = ((byte)reg[0x21 + ch * 4] << 16)
                    + ((byte)reg[0x41 + ch * 4] << 8)
                    + (byte)reg[0x61 + ch * 4];
                nrc.leadr = ((byte)reg[0x22 + ch * 4] << 16)
                    + ((byte)reg[0x42 + ch * 4] << 8)
                    + (byte)reg[0x62 + ch * 4];
                nrc.eadr = ((byte)reg[0x23 + ch * 4] << 16)
                    + ((byte)reg[0x43 + ch * 4] << 8)
                    + (byte)reg[0x63 + ch * 4];

                nrc.dda = (reg[0x1 + ch * 4] & 0x80) != 0;//key on
                nrc.ex = (reg[0x1 + ch * 4] & 0x40) != 0;
                nrc.noise = (reg[0x1 + ch * 4] & 0x20) != 0;
                nrc.loopFlg = (reg[0x1 + ch * 4] & 0x10) != 0;

                int vol = Math.Min(19, nrc.nfrq / 12);
                nrc.note = -1;
                if (nrc.dda)
                {
                    if (vol > 0) {
                        nrc.note = SearchNote(nrc.freq);
                    }
                    //if (!oldParam.channels[ch].dda)
                    {
                        nrc.volumeL = nrc.pan == 8 ? vol : (nrc.pan < 8 ? vol : (vol * (15 - nrc.pan) / 7));
                        nrc.volumeR = nrc.pan == 8 ? vol : (nrc.pan < 8 ? ((nrc.pan == 0) ? 0 : vol * (nrc.pan - 1) / 7) : 4);
                    }
                }
                else
                {
                    nrc.volumeL += nrc.volumeL > 0 ? -1 : 0;
                    nrc.volumeR += nrc.volumeR > 0 ? -1 : 0;
                }
            }
        }

        public void screenDrawParams()
        {
            for (int ch = 0; ch < 8; ch++)
            {
                MDChipParams.Channel orc = oldParam.channels[ch];
                MDChipParams.Channel nrc = newParam.channels[ch];

                DrawBuff.ChYMZ280B(frameBuffer, ch, ref orc.mask, nrc.mask, 0);
                DrawBuff.PanType5(frameBuffer, 6 * 4 + 1, ch * 8 + 8, ref orc.panL, nrc.panL, 0);
                DrawBuff.PanType5(frameBuffer, 7 * 4 + 1, ch * 8 + 8, ref orc.panR, nrc.panR, 0);
                DrawBuff.KeyBoardYMZ280B(frameBuffer, 4 * 8 + 1, ch * 8 + 8, ref orc.note, nrc.note, 0);
                DrawBuff.Volume(frameBuffer, 68 * 4 + 1, 8 + ch * 8, 1, ref orc.volumeL, nrc.volumeL, 0);
                DrawBuff.Volume(frameBuffer, 68 * 4 + 1, 8 + ch * 8, 2, ref orc.volumeR, nrc.volumeR, 0);

                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 8 + 1, ch * 8 + 10 * 8, 0, ref orc.pan, nrc.pan);
                DrawBuff.drawNESSw(frameBuffer, 4 * 64 + 1, ch * 8 + 8, ref orc.dda, nrc.dda);//KEY ON
                DrawBuff.drawNESSw(frameBuffer, 4 * 65 + 1, ch * 8 + 8, ref orc.ex, nrc.ex);
                DrawBuff.drawNESSw(frameBuffer, 4 * 66 + 1, ch * 8 + 8, ref orc.noise, nrc.noise);
                DrawBuff.drawNESSw(frameBuffer, 4 * 67 + 1, ch * 8 + 8, ref orc.loopFlg, nrc.loopFlg);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 13 + 1, ch * 8 + 10 * 8, 0, ref orc.sadr, nrc.sadr);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 22 + 1, ch * 8 + 10 * 8, 0, ref orc.ladr, nrc.ladr);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 31 + 1, ch * 8 + 10 * 8, 0, ref orc.leadr, nrc.leadr);
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 40 + 1, ch * 8 + 10 * 8, 0, ref orc.eadr, nrc.eadr);
                DrawBuff.font4Hex12Bit(frameBuffer, 4 * 49 + 1, ch * 8 + 10 * 8, 0, ref orc.freq, nrc.freq);//PITCH
                DrawBuff.font4HexByte(frameBuffer, 4 * 55 + 1, ch * 8 + 10 * 8, 0, ref orc.nfrq, nrc.nfrq);//TL
            }
        }

        //Furnace
        private static readonly int[] NoteTableOct = new int[]
        {
            //Oct0
            0x002, 0x002, 0x002, 0x002, 0x002, 0x003, 0x003, 0x003, 0x004, 0x004, 0x004, 0x004,
            //Oct1
            0x005, 0x005, 0x006, 0x006, 0x006, 0x007, 0x007, 0x008, 0x008, 0x009, 0x009, 0x00a,
            //Oct2
            0x00b, 0x00b, 0x00c, 0x00d, 0x00e, 0x00f, 0x00f, 0x010, 0x011, 0x013, 0x014, 0x015, 
            //Oct3
            0x016, 0x018, 0x019, 0x01b, 0x01c, 0x01e, 0x020, 0x022, 0x024, 0x026, 0x028, 0x02b,
            //Oct4
            0x02d, 0x030, 0x033, 0x036, 0x03a, 0x03d, 0x041, 0x045, 0x049, 0x04d, 0x052, 0x057,
            //Oct5 
            0x05c, 0x062, 0x067, 0x06e, 0x074, 0x07b, 0x082, 0x08a, 0x093, 0x09b, 0x0a5, 0x0af,
            //Oct6 
            0x0b9, 0x0c4, 0x0d0, 0x0dc, 0x0e9, 0x0f7, 0x106, 0x116, 0x126, 0x138, 0x14A, 0x15E, 
            //Oct7
            0x173, 0x189, 0x1A0, 0x1B9, 0x1D4, 0x1EF, 0x1ff, 0x1ff, 0x1ff, 0x1ff, 0x1ff, 0x1ff
        };

        private int SearchNote(int freq)
        {
            // freq 以下で最大の境界値を探す
            for (int i = 0; i < NoteTableOct.Length; i++)
            {
                if (freq < NoteTableOct[i])
                    return i - 1;
            }

            return NoteTableOct.Length - 1;

        }

    }
}
