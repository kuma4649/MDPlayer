using System;
using System.Drawing;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class frmYM2610 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2610 newParam = null;
        private MDChipParams.YM2610 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2610(frmMain frm, int chipID, int zoom, MDChipParams.YM2610 newParam, MDChipParams.YM2610 oldParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeYM2610, null, zoom);
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

        private void frmYM2610_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2610[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm2610[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYM2610_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2610.Width * zoom, frameSizeH + Properties.Resources.planeYM2610.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2610.Width * zoom, frameSizeH + Properties.Resources.planeYM2610.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2610.Width * zoom, frameSizeH + Properties.Resources.planeYM2610.Height * zoom);
            frmYM2610_Resize(null, null);

        }

        private void frmYM2610_Resize(object sender, EventArgs e)
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
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;

            if (ch < 0) return;

            if (ch < 14)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.YM2610, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 14; ch++) parent.ResetChannelMask(EnmChip.YM2610, chipID, ch);
                return;
            }

            // 音色表示欄の判定

            int h = (py - 15 * 8) / (6 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(EnmChip.YM2610, instCh, chipID);
            }
        }


        public void screenInit()
        {
            int tp = ((chipID == 0) ? (parent.setting.YM2610Type.UseScci || parent.setting.YM2610Type.UseScci2) : parent.setting.YM2610SType.UseScci) ? 1 : 0;

            for (int y = 0; y < 14; y++)
            {
                DrawBuff.drawFont8(frameBuffer, 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, y * 8 + 8, kt, tp);
                }

                if (y < 13)
                {
                    DrawBuff.ChYM2610_P(frameBuffer, 0, y * 8 + 8, y, false, tp);
                }

                if (y < 6 || y == 13)
                {
                    DrawBuff.drawPanP(frameBuffer, 24, y * 8 + 8, 3, tp);
                }

                int d = 99;
                if (y > 5 && y < 9)
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + y * 8, 0, ref d, 0, tp);
                }
                else
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + y * 8, 1, ref d, 0, tp);
                    d = 99;
                    DrawBuff.Volume(frameBuffer, 256, 8 + y * 8, 2, ref d, 0, tp);
                }
            }

            for (int y = 0; y < 6; y++)
            {
                int d = 99;
                DrawBuff.PanYM2610Rhythm(frameBuffer, y, ref d, 3, ref d, tp);
                d = 99;
                DrawBuff.VolumeYM2610Rhythm(frameBuffer, y, 1, ref d, 0, tp);
                d = 99;
                DrawBuff.VolumeYM2610Rhythm(frameBuffer, y, 2, ref d, 0, tp);
            }
            bool f = true;
            DrawBuff.ChYM2610Rhythm(frameBuffer, 0, ref f, false, tp);
        }

        private static byte[] md = new byte[]
        {
            0x08<<4,
            0x08<<4,
            0x08<<4,
            0x08<<4,
            0x0c<<4,
            0x0e<<4,
            0x0e<<4,
            0x0f<<4
        };

        private static float[] fmDivTbl = new float[] { 6, 3, 2 };
        private static float[] ssgDivTbl = new float[] { 4, 2, 1 };

        public void screenChangeParams()
        {
            int delta;
            float frq;

            int[][] YM2610Register = Audio.GetYM2610Register(chipID);
            int[] fmKeyYM2610 = Audio.GetYM2610KeyOn(chipID);
            int[] YM2610Vol = Audio.GetYM2610Volume(chipID);
            int[] YM2610Ch3SlotVol = Audio.GetYM2610Ch3SlotVolume(chipID);
            int[][] YM2610Rhythm = Audio.GetYM2610RhythmVolume(chipID);
            int[] YM2610AdpcmVol = Audio.GetYM2610AdpcmVolume(chipID);

            bool isFmEx = (YM2610Register[chipID][0x27] & 0x40) > 0;
            newParam.channels[2].ex = isFmEx;

            int defaultMasterClock = 8000000;
            float ssgMul = 1.0f;
            int masterClock = defaultMasterClock;
            if (Audio.clockYM2610 != 0)
            {
                ssgMul = Audio.clockYM2610 / (float)defaultMasterClock;
                masterClock = Audio.clockYM2610;
            }

            float fmDiv = fmDivTbl[YM2610Register[0][0x2d]];
            float ssgDiv = ssgDivTbl[YM2610Register[0][0x2d]];
            ssgMul = ssgMul * ssgDiv / 4;

            //int masterClock = Audio.clockYM2610;
            //int defaultMasterClock = 8000000;
            //float mul = 1.0f;
            //if (masterClock != 0)
            //    mul = masterClock / (float)defaultMasterClock;

            newParam.lfoSw = (YM2610Register[0][0x22] & 0x8) != 0;
            newParam.lfoFrq = (YM2610Register[0][0x22] & 0x7);

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.channels[ch].inst[i * 11 + 0] = YM2610Register[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.channels[ch].inst[i * 11 + 1] = YM2610Register[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.channels[ch].inst[i * 11 + 2] = YM2610Register[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.channels[ch].inst[i * 11 + 3] = YM2610Register[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.channels[ch].inst[i * 11 + 4] = (YM2610Register[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.channels[ch].inst[i * 11 + 5] = YM2610Register[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.channels[ch].inst[i * 11 + 6] = (YM2610Register[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.channels[ch].inst[i * 11 + 7] = YM2610Register[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.channels[ch].inst[i * 11 + 8] = (YM2610Register[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.channels[ch].inst[i * 11 + 9] = (YM2610Register[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.channels[ch].inst[i * 11 + 10] = YM2610Register[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.channels[ch].inst[44] = YM2610Register[p][0xb0 + c] & 0x07;//AL
                newParam.channels[ch].inst[45] = (YM2610Register[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.channels[ch].inst[46] = (YM2610Register[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.channels[ch].inst[47] = YM2610Register[p][0xb4 + c] & 0x07;//FMS

                newParam.channels[ch].pan = (YM2610Register[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = YM2610Register[p][0xa0 + c] + (YM2610Register[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (YM2610Register[p][0xa4 + c] & 0x38) >> 3;
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2610[ch]&1) != 0)
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    byte con = (byte)(fmKeyYM2610[ch]);
                    int v = 127;
                    int m = md[YM2610Register[p][0xb0 + c] & 7];
                    //OP1
                    v = (((con & 0x10) != 0) && ((m & 0x10) != 0) && v > (YM2610Register[p][0x40 + c] & 0x7f)) ? (YM2610Register[p][0x40 + c] & 0x7f) : v;
                    //OP3
                    v = (((con & 0x20) != 0) && ((m & 0x20) != 0) && v > (YM2610Register[p][0x44 + c] & 0x7f)) ? (YM2610Register[p][0x44 + c] & 0x7f) : v;
                    //OP2
                    v = (((con & 0x40) != 0) && ((m & 0x40) != 0) && v > (YM2610Register[p][0x48 + c] & 0x7f)) ? (YM2610Register[p][0x48 + c] & 0x7f) : v;
                    //OP4
                    v = (((con & 0x80) != 0) && ((m & 0x80) != 0) && v > (YM2610Register[p][0x4c + c] & 0x7f)) ? (YM2610Register[p][0x4c + c] & 0x7f) : v;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[p][0xb4 + c] & 0x80) != 0 ? 1 : 0) * YM2610Vol[ch] / 80.0), 0), 19);
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[p][0xb4 + c] & 0x80) != 0 ? 1 : 0) * YM2610Vol[ch] / 80.0), 0), 19);
                }
                else
                {
                    int m = md[YM2610Register[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    freq = YM2610Register[0][0xa9] + (YM2610Register[0][0xad] & 0x07) * 0x100;
                    octav = (YM2610Register[0][0xad] & 0x38) >> 3;
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2610[2] & 0x10) != 0 && ((m & 0x10) != 0))
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    int v = ((m & 0x10) != 0) ? YM2610Register[p][0x40 + c] : 127;
                    newParam.channels[2].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[0][0xb4 + 2] & 0x80) != 0 ? 1 : 0) * YM2610Ch3SlotVol[0] / 80.0), 0), 19);
                    newParam.channels[2].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[0][0xb4 + 2] & 0x80) != 0 ? 1 : 0) * YM2610Ch3SlotVol[0] / 80.0), 0), 19);
                }
                newParam.channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++) //FM EX
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.channels[ch].pan = 0;

                if (isFmEx)
                {
                    int m = md[YM2610Register[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    int op = ch - 5;
                    op = op == 1 ? 2 : (op == 2 ? 1 : op);

                    int freq = YM2610Register[0][0xa8 + c] + (YM2610Register[0][0xac + c] & 0x07) * 0x100;
                    int octav = (YM2610Register[0][0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKeyYM2610[2] & (0x10 << (ch - 5))) != 0 && ((m & (0x10 << op)) != 0))
                    {
                        float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                        ff /= 1038f;
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);
                    }
                    newParam.channels[ch].note = n;

                    int v = ((m & (0x10 << op)) != 0) ? YM2610Register[0][0x42 + op * 4] : 127;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * YM2610Ch3SlotVol[ch - 5] / 80.0), 0), 19);
                }
                else
                {
                    newParam.channels[ch].note = -1;
                    newParam.channels[ch].volumeL = 0;
                }
            }

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.channels[ch + 9];

                bool t = (YM2610Register[0][0x07] & (0x1 << ch)) == 0;
                bool n = (YM2610Register[0][0x07] & (0x8 << ch)) == 0;
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);

                channel.volume = (int)(((t || n) ? 1 : 0) * (YM2610Register[0][0x08 + ch] & 0xf) * (20.0 / 16.0));
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                if (channel.volume == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    int ft = YM2610Register[0][0x00 + ch * 2];
                    int ct = YM2610Register[0][0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = 7987200.0f / (64.0f * (float)tp) * ssgMul;// 7987200 = MasterClock
                    channel.note = Common.searchSSGNote(ftone);
                }

            }

            newParam.nfrq = YM2610Register[0][0x06] & 0x1f;
            newParam.efrq = YM2610Register[0][0x0c] * 0x100 + YM2610Register[0][0x0b];
            newParam.etype = (YM2610Register[0][0x0d] & 0x7) + 2;

            //ADPCM B
            newParam.channels[12].pan = (YM2610Register[0][0x11] & 0xc0) >> 6;
            newParam.channels[12].volumeL = Math.Min(Math.Max(YM2610AdpcmVol[0] / 80, 0), 19);
            newParam.channels[12].volumeR = Math.Min(Math.Max(YM2610AdpcmVol[1] / 80, 0), 19);
            delta = (YM2610Register[0][0x1a] << 8) | YM2610Register[0][0x19];
            frq = (float)(delta / 9447.0f);//Delta=9447 at freq=8kHz
            newParam.channels[12].note = (YM2610Register[0][0x10] & 0x80) != 0 ? Common.searchYM2608Adpcm(frq) : -1;
            if ((YM2610Register[0][0x11] & 0xc0) == 0)
            {
                newParam.channels[12].note = -1;
            }


            for (int ch = 13; ch < 19; ch++) //ADPCM A
            {
                newParam.channels[ch].pan = (YM2610Register[1][0x08 + ch - 13] & 0xc0) >> 6;
                newParam.channels[ch].volumeL = Math.Min(Math.Max(YM2610Rhythm[ch - 13][0] / 80, 0), 19);
                newParam.channels[ch].volumeR = Math.Min(Math.Max(YM2610Rhythm[ch - 13][1] / 80, 0), 19);
            }
        }

        public void screenDrawParams()
        {
            int tp = ((chipID == 0) ? (parent.setting.YM2610Type.UseScci || parent.setting.YM2610Type.UseScci2) : parent.setting.YM2610SType.UseScci) ? 1 : 0;

            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                if (c != 2)
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.Inst(frameBuffer, 1, 17, c, oyc.inst, nyc.inst);
                    DrawBuff.Ch3YM2610(frameBuffer, c, ref oyc.mask, nyc.mask, ref oyc.ex, nyc.ex, tp);
                }
                else if (c < 6)
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.Inst(frameBuffer, 1, 17, c, oyc.inst, nyc.inst);
                    DrawBuff.ChYM2610(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                }
                else
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + (c+3) * 8, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.KeyBoard(frameBuffer, c + 3, ref oyc.note, nyc.note, tp);
                    DrawBuff.ChYM2610(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                }


            }

            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 9];
                MDChipParams.Channel nyc = newParam.channels[c + 9];

                DrawBuff.Volume(frameBuffer, 256, 8 + (c+6) * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c + 6, ref oyc.note, nyc.note, tp);
                DrawBuff.Tn(frameBuffer, 6, 2, c + 6, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

                DrawBuff.ChYM2610(frameBuffer, c + 9, ref oyc.mask, nyc.mask, tp);
            }

            DrawBuff.Volume(frameBuffer, 256, 8 + 13 * 8, 1, ref oldParam.channels[12].volumeL, newParam.channels[12].volumeL, tp);
            DrawBuff.Volume(frameBuffer, 256, 8 + 13 * 8, 2, ref oldParam.channels[12].volumeR, newParam.channels[12].volumeR, tp);
            DrawBuff.Pan(frameBuffer, 24, 8 + 13 * 8, ref oldParam.channels[12].pan, newParam.channels[12].pan, ref oldParam.channels[12].pantp, tp);
            DrawBuff.KeyBoard(frameBuffer, 13, ref oldParam.channels[12].note, newParam.channels[12].note, tp);
            DrawBuff.ChYM2610(frameBuffer, 13, ref oldParam.channels[12].mask, newParam.channels[12].mask, tp);

            for (int c = 0; c < 6; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 13];
                MDChipParams.Channel nyc = newParam.channels[c + 13];

                DrawBuff.VolumeYM2610Rhythm(frameBuffer, c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.VolumeYM2610Rhythm(frameBuffer, c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                DrawBuff.PanYM2610Rhythm(frameBuffer, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);

            }
            DrawBuff.ChYM2610Rhythm(frameBuffer, 0, ref oldParam.channels[13].mask, newParam.channels[13].mask, tp);

            DrawBuff.LfoSw(frameBuffer, 16, 216, ref oldParam.lfoSw, newParam.lfoSw);
            DrawBuff.LfoFrq(frameBuffer, 64, 216, ref oldParam.lfoFrq, newParam.lfoFrq);

            DrawBuff.Nfrq(frameBuffer, 25, 54, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.Efrq(frameBuffer, 38, 54, ref oldParam.efrq, newParam.efrq);
            DrawBuff.Etype(frameBuffer, 53, 54, ref oldParam.etype, newParam.etype);
        }

    }
}
