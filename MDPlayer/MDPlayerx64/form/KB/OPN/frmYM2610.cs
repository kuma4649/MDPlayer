#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmYM2610 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2610 newParam = null;
        private MDChipParams.YM2610 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2610(frmMain frm, int chipID, int zoom, MDChipParams.YM2610 newParam, MDChipParams.YM2610 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeYM2610"], null, zoom);
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
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM2610"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM2610"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM2610"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM2610"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM2610"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM2610"].Height * zoom);
            frmYM2610_Resize(null, null);

        }

        private void frmYM2610_Resize(object sender, EventArgs e)
        {

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            int ch;
            int c;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (ch = 0; ch < 20; ch++)
                    {
                        if (ch >= 9 && ch <= 11) continue;
                        if (ch == 12) continue;

                        c = ch;
                        if (ch == 13) c = 12;
                        if (ch > 13) c = ch - 1;

                        if (newParam.channels[c].mask == true)
                            parent.ResetChannelMask(EnmChip.YM2610, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.YM2610, chipID, ch);
                    }
                }
                return;
            }

            ch = (py / 8) - 1;
            c = ch;
            if (ch == 12) c = 13;
            if (ch == 13) c = 12;

            if (ch < 0) return;

            if (ch < 14)
            {
                if (e.Button == MouseButtons.Left)
                {

                    if (ch == 12)
                    {
                        ch = 14 + (Math.Max(px - 4, 0)) / 60;
                        c = ch - 1;
                    }

                    //マスク
                    if (newParam.channels[c].mask == true)
                        parent.ResetChannelMask(EnmChip.YM2610, chipID, ch);
                    else
                        parent.SetChannelMask(EnmChip.YM2610, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 20; ch++) parent.ResetChannelMask(EnmChip.YM2610, chipID, ch);
                return;
            }

            // 音色表示欄の判定

            int h = (py - 15 * 8) / (6 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.GetInstCh(EnmChip.YM2610, instCh, chipID);
            }
        }


        public void screenInit()
        {
            int tp = (
                        (chipID == 0)
                        ? (parent.setting.YM2610Type[0].UseReal[0] || (parent.setting.YM2610Type[0].UseReal.Length > 1 && parent.setting.YM2610Type[0].UseReal[1]))
                        : (parent.setting.YM2610Type[1].UseReal[0] || (parent.setting.YM2610Type[1].UseReal.Length > 1 && parent.setting.YM2610Type[1].UseReal[1]))
                    )
                    ? 1
                    : 0;

            for (int y = 0; y < 14; y++)
            {
                DrawBuff.drawFont8(frameBuffer, 328 + 1, y * 8 + 8, 1, "   ");

                if (y != 12)
                {
                    for (int i = 0; i < 96; i++)
                    {
                        int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                        int kt = Tables.kbl[(i % 12) * 2 + 1];
                        DrawBuff.drawKbn(frameBuffer, 33 + kx, y * 8 + 8, kt, tp);
                    }
                }

                if (y < 13)
                {
                    DrawBuff.ChYM2610_P(frameBuffer, 1, y * 8 + 8, y, false, tp);
                }

                if (y < 6 || y == 13)
                {
                    DrawBuff.drawPanP(frameBuffer, 25, y * 8 + 8, 3, tp);
                }

                int d = 99;
                if (y > 5 && y < 9)
                {
                    DrawBuff.VolumeShort(frameBuffer, 280 + 1, 8 + y * 8, 0, ref d, 0, tp);
                }
                else
                {
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + y * 8, 1, ref d, 0, tp);
                    d = 99;
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + y * 8, 2, ref d, 0, tp);
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
            for (int y = 0; y < 6; y++)
            {
                bool? f = true;
                DrawBuff.ChYM2610Rhythm(frameBuffer, y, ref f, false, tp);
            }
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
            if (Audio.ClockYM2610 != 0)
            {
                ssgMul = Audio.ClockYM2610 / (float)defaultMasterClock;
                masterClock = Audio.ClockYM2610;
            }

            int divInd = YM2610Register[0][0x2d];
            if (divInd < 0 || divInd > 2) divInd = 0;
            float fmDiv = fmDivTbl[divInd];
            float ssgDiv = ssgDivTbl[divInd];
            ssgMul = ssgMul / ssgDiv * 4;

            //int masterClock = Audio.clockYM2610;
            //int defaultMasterClock = 8000000;
            //float mul = 1.0f;
            //if (masterClock != 0)
            //    mul = masterClock / (float)defaultMasterClock;

            newParam.timerA = YM2610Register[0][0x24] | ((YM2610Register[0][0x25] & 0x3) << 8);
            newParam.timerB = YM2610Register[0][0x26];
            newParam.rhythmTotalLevel = YM2610Register[1][0x01];
            newParam.adpcmLevel = YM2610Register[0][0x1b];

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
                newParam.channels[ch].slot = (byte)(fmKeyYM2610[ch] >> 4);

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = YM2610Register[p][0xa0 + c] + (YM2610Register[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (YM2610Register[p][0xa4 + c] & 0x38) >> 3;
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2610[ch] & 1) != 0)
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
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[p][0xb4 + c] & 0x40) != 0 ? 1 : 0) * YM2610Vol[ch] / 80.0), 0), 19);
                }
                else
                {
                    int m = md[YM2610Register[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    freq = YM2610Register[0][0xa9] + (YM2610Register[0][0xad] & 0x07) * 0x100;
                    octav = (YM2610Register[0][0xad] & 0x38) >> 3;
                    newParam.channels[2].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2610[2] & 0x10) != 0 && ((m & 0x10) != 0))
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) , 0), 95);

                    int v = ((m & 0x10) != 0) ? YM2610Register[p][0x40 + c] : 127;
                    newParam.channels[2].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[0][0xb4 + 2] & 0x80) != 0 ? 1 : 0) * YM2610Ch3SlotVol[0] / 80.0), 0), 19);
                    newParam.channels[2].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((YM2610Register[0][0xb4 + 2] & 0x40) != 0 ? 1 : 0) * YM2610Ch3SlotVol[0] / 80.0), 0), 19);
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
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
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

                channel.volumeL = YM2610Register[0][0x08 + ch] & 0xf;
                channel.volume = (int)(((t || n) ? 1 : 0) * (YM2610Register[0][0x08 + ch] & 0xf));

                int ft = YM2610Register[0][0x00 + ch * 2];
                int ct = YM2610Register[0][0x01 + ch * 2]&0xf;
                int tp = (ct << 8) | ft;
                channel.freq = tp;

                if (channel.volumeL == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    if (tp == 0)
                    {
                        channel.note = -1;
                        //channel.volume = 0;
                    }
                    else
                    {
                        float ftone = masterClock / (64.0f * (float)tp) * ssgMul;// 7987200 = MasterClock
                        channel.note = Common.searchSSGNote(ftone);
                    }
                }
                channel.ex = (YM2610Register[0][0x08 + ch] & 0xf0) != 0;
            }

            newParam.nfrq = YM2610Register[0][0x06] & 0x1f;
            newParam.efrq = YM2610Register[0][0x0c] * 0x100 + YM2610Register[0][0x0b];
            newParam.etype = (YM2610Register[0][0x0d] & 0xf);

            //ADPCM B
            newParam.channels[12].pan = (YM2610Register[0][0x11] & 0xc0) >> 6;
            if (YM2610AdpcmVol[0] != 0)
            {
                newParam.channels[12].volumeL = Math.Min(Math.Max(YM2610AdpcmVol[0] * YM2610Register[0][0x1b], 0), 19);
            }
            else
            {
                if (newParam.channels[12].volumeL > 0) newParam.channels[12].volumeL--;
            }
            if (YM2610AdpcmVol[1] != 0)
            {
                newParam.channels[12].volumeR = Math.Min(Math.Max(YM2610AdpcmVol[1] * YM2610Register[0][0x1b], 0), 19);
            }
            else
            {
                if (newParam.channels[12].volumeR > 0) newParam.channels[12].volumeR--;
            }
            delta = (YM2610Register[0][0x1a] << 8) | YM2610Register[0][0x19];
            newParam.channels[12].freq = delta;
            frq = (float)(delta / 9447.0f);//Delta=9447 at freq=8kHz
            newParam.channels[12].note = (YM2610Register[0][0x10] & 0x80) != 0 ? (Common.searchYM2608Adpcm(frq) - 0) : -1;
            if ((YM2610Register[0][0x11] & 0xc0) == 0)
            {
                newParam.channels[12].note = -1;
            }


            int tl = YM2610Register[1][0x01] & 0x3f;
            for (int ch = 13; ch < 19; ch++) //ADPCM A
            {
                newParam.channels[ch].pan = (YM2610Register[1][0x08 + ch - 13] & 0xc0) >> 6;
                newParam.channels[ch].volumeRL = YM2610Register[1][ch - 13 + 0x08] & 0x1f;
                //newParam.channels[ch].volumeL = Math.Min(Math.Max(YM2610Rhythm[ch - 13][0] / 80, 0), 19);
                //newParam.channels[ch].volumeR = Math.Min(Math.Max(YM2610Rhythm[ch - 13][1] / 80, 0), 19);
                int il = YM2610Register[1][0x08 + ch - 13] & 0x1f;

                if (YM2610Rhythm[ch - 13][0] != 0)
                {
                    newParam.channels[ch].volumeL = Math.Min(Math.Max(YM2610Rhythm[ch - 13][0] * tl * il / 128, 0), 19);
                    //newParam.channels[12].volumeR = Math.Min(Math.Max(YM2610AdpcmVol[1] * YM2610Register[0][0x1b], 0), 19);
                }
                else
                {
                    if (newParam.channels[ch].volumeL > 0) newParam.channels[ch].volumeL--;
                }
                if (YM2610Rhythm[ch - 13][1] != 0)
                {
                    newParam.channels[ch].volumeR = Math.Min(Math.Max(YM2610Rhythm[ch - 13][1] * tl * il / 128, 0), 19);
                    //newParam.channels[12].volumeR = Math.Min(Math.Max(YM2610AdpcmVol[1] * YM2610Register[0][0x1b], 0), 19);
                }
                else
                {
                    if (newParam.channels[ch].volumeR > 0) newParam.channels[ch].volumeR--;
                }
            }
        }

        public void screenDrawParams()
        {
            int tp = (
                        (chipID == 0)
                        ? (parent.setting.YM2610Type[0].UseReal[0] || (parent.setting.YM2610Type[0].UseReal.Length > 1 && parent.setting.YM2610Type[0].UseReal[1]))
                        : (parent.setting.YM2610Type[1].UseReal[0] || (parent.setting.YM2610Type[1].UseReal.Length > 1 && parent.setting.YM2610Type[1].UseReal[1]))
                    )
                    ? 1
                    : 0;

            //FM - SSG
            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                if (c == 2)
                {
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 25, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + c * 8, ref oyc.note, nyc.note, tp);
                    DrawBuff.InstOPNA(frameBuffer, 5, 17 * 8, c, oyc.inst, nyc.inst);
                    DrawBuff.Ch3YM2610(frameBuffer, c, ref oyc.mask, nyc.mask, ref oyc.ex, nyc.ex, tp);
                    DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 78, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }
                else if (c < 6)
                {
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 25, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + c * 8, ref oyc.note, nyc.note, tp);
                    DrawBuff.InstOPNA(frameBuffer, 5, 17 * 8, c, oyc.inst, nyc.inst);
                    DrawBuff.ChYM2610(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                    DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 78, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }
                else
                {
                    //FMex
                    DrawBuff.Volume(frameBuffer, 272 + 1, 8 + (c + 3) * 8, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + (c + 3) * 8, ref oyc.note, nyc.note, tp);
                    DrawBuff.ChYM2610(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 78, 8 + (c + 3) * 8, 0, ref oyc.freq, nyc.freq);
                }


            }

            //SSG
            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 9];
                MDChipParams.Channel nyc = newParam.channels[c + 9];

                DrawBuff.VolumeShort(frameBuffer, 280 + 1, 8 + (c + 6) * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoardOPNA(frameBuffer, 33, (c + 6) * 8 + 8, ref oyc.note, nyc.note, tp);
                DrawBuff.TnOPNA(frameBuffer, 6, 2, c + 6, ref oyc.tn, nyc.tn, ref oyc.tntp, tp * 2);

                DrawBuff.ChYM2610(frameBuffer, c + 9, ref oyc.mask, nyc.mask, tp);
                DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 78, 8 + (c + 6) * 8, 0, ref oyc.freq, nyc.freq);
                DrawBuff.font4HexByte(frameBuffer, 272 + 1, 8 + (c + 6) * 8, 0, ref oyc.volumeL, nyc.volumeL);
                DrawBuff.drawNESSw(frameBuffer, 268 + 1, 8 + c * 8, ref oyc.ex, nyc.ex);
            }

            //ADPCM B
            DrawBuff.Volume(frameBuffer, 256, 8 + 13 * 8, 1, ref oldParam.channels[12].volumeL, newParam.channels[12].volumeL, tp);
            DrawBuff.Volume(frameBuffer, 256, 8 + 13 * 8, 2, ref oldParam.channels[12].volumeR, newParam.channels[12].volumeR, tp);
            DrawBuff.Pan(frameBuffer, 24, 8 + 13 * 8, ref oldParam.channels[12].pan, newParam.channels[12].pan, ref oldParam.channels[12].pantp, tp);
            DrawBuff.ChYM2610(frameBuffer, 13, ref oldParam.channels[12].mask, newParam.channels[12].mask, tp);
            DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + 13 * 8, ref oldParam.channels[12].note, newParam.channels[12].note, tp);
            DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 78, 8 + 13 * 8, 0, ref oldParam.channels[12].freq, newParam.channels[12].freq);
            DrawBuff.font4HexByte(frameBuffer, 272 + 1, 8 + 13 * 8, 0, ref oldParam.channels[12].volume, newParam.channels[12].volume);

            //ADPCM A(Rhythm)
            for (int c = 0; c < 6; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 13];
                MDChipParams.Channel nyc = newParam.channels[c + 13];

                DrawBuff.VolumeYM2610Rhythm(frameBuffer, c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.VolumeYM2610Rhythm(frameBuffer, c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                DrawBuff.PanYM2610Rhythm(frameBuffer, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                DrawBuff.font4Int2(frameBuffer, c * 4 * 15 + 9, 26 * 4, 0, 0, ref oyc.volumeRL, nyc.volumeRL);
            }
            for (int c = 0; c < 6; c++)
                DrawBuff.ChYM2610Rhythm(frameBuffer, c, ref oldParam.channels[13 + c].mask, newParam.channels[13 + c].mask, tp);

            DrawBuff.font4Hex12Bit(frameBuffer, 85 * 4, 30 * 4, 0, ref oldParam.timerA, newParam.timerA);
            DrawBuff.font4HexByte(frameBuffer, 85 * 4, 32 * 4, 0, ref oldParam.timerB, newParam.timerB);

            //HardLFO NOISE ENV
            DrawBuff.LfoSw(frameBuffer, 84 * 4, 18 * 8, ref oldParam.lfoSw, newParam.lfoSw);
            DrawBuff.LfoFrq(frameBuffer, 84 * 4, 19 * 8, ref oldParam.lfoFrq, newParam.lfoFrq);

            DrawBuff.Nfrq(frameBuffer, 84, 42, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.Efrq(frameBuffer, 84, 44, ref oldParam.efrq, newParam.efrq);
            DrawBuff.Etype(frameBuffer, 84, 46, ref oldParam.etype, newParam.etype);

            DrawBuff.font4Int2(frameBuffer, 84 * 4, 50 * 4, 0, 0, ref oldParam.rhythmTotalLevel, newParam.rhythmTotalLevel);
            DrawBuff.font4Int3(frameBuffer, 84 * 4, 52 * 4, 0, 3, ref oldParam.adpcmLevel, newParam.adpcmLevel);
        }

    }
}
