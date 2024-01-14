using MDPlayerx64;
#if X64
#else
using MDPlayer.Properties;
#endif
using MDSound;

namespace MDPlayer.form
{
    public partial class frmPPZ8 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.PPZ8 newParam = null;
        private MDChipParams.PPZ8 oldParam = new MDChipParams.PPZ8();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmPPZ8(frmMain frm, int chipID, int zoom, MDChipParams.PPZ8 newParam, MDChipParams.PPZ8 oldParam) : base(frm)
        {
            InitializeComponent();

            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, ResMng.ImgDic["planePPZ8"], null, zoom);
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

        private void frmPPZ8_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosPPZ8[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosPPZ8[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmPPZ8_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planePPZ8"].Width * zoom, frameSizeH + ResMng.ImgDic["planePPZ8"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planePPZ8"].Width * zoom, frameSizeH + ResMng.ImgDic["planePPZ8"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planePPZ8"].Width * zoom, frameSizeH + ResMng.ImgDic["planePPZ8"].Height * zoom);
            frmPPZ8_Resize(null, null);

        }

        private void frmPPZ8_Resize(object sender, EventArgs e)
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
                            parent.ResetChannelMask(EnmChip.PPZ8, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.PPZ8, chipID, ch);
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
                    parent.SetChannelMask(EnmChip.PPZ8, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(EnmChip.PPZ8, chipID, ch);
                return;

            }

        }

        private int searchPPZ8Note(int freq)
        {
            double m = double.MaxValue;

            int clock = Audio.ClockPPZ8;
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
            bool PPZ8Type = false;//  (chipID == 0) ? parent.setting.PPZ8Type.UseScci : parent.setting.PPZ8SType.UseScci;
            int tp = PPZ8Type ? 1 : 0;
            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 8 + 8, kt, tp);
                }
                DrawBuff.drawFont8(frameBuffer, 4 * 754, ch * 8 + 8, 1, "   ");
                DrawBuff.drawPanType2P(frameBuffer, 24, ch * 8 + 8, 0, tp);
                //DrawBuff.ChPPZ8_P(frameBuffer, 0, 8 + ch * 8, ch, false, tp);
                //int d = 99;
                //DrawBuff.VolumeToPPZ8(frameBuffer, ch, 1, ref d, 0, tp);
                //d = 99;
                //DrawBuff.VolumeToPPZ8(frameBuffer, ch, 2, ref d, 0, tp);
            }
        }

        public void screenChangeParams()
        {
            PPZ8.PPZChannelWork[] ppz8State = Audio.GetPPZ8Register(chipID);
            if (ppz8State == null) return;

            for (int ch = 0; ch < 8; ch++)
            {
                if (ppz8State.Length < ch + 1) continue;
                if (ppz8State[ch] == null) continue;

                newParam.channels[ch].pan =
                    ((ppz8State[ch].pan < 6) ? 0xf : (4 * (9 - ppz8State[ch].pan)))
                | (((ppz8State[ch].pan > 4) ? 0xf : (4 * ppz8State[ch].pan)) * 0x10);

                ushort p = ppz8State[ch].pan;
                newParam.channels[ch].pan = ((p > 0 && p < 6) ? 0xf : (4 * (9 - ppz8State[ch].pan)))
                | (((p > 4 && p < 10) ? 0xf : (4 * (p - 1))) * 0x10);

                if (ppz8State[ch].KeyOn)
                {
                    newParam.channels[ch].volumeL = Math.Min((ppz8State[ch].volume * (newParam.channels[ch].pan & 0xf)) / 8, 19);
                    newParam.channels[ch].volumeR = Math.Min((ppz8State[ch].volume * ((newParam.channels[ch].pan & 0xf0) >> 4)) / 8, 19);
                }
                else
                {
                    newParam.channels[ch].volumeL -= newParam.channels[ch].volumeL > 0 ? 1 : 0;
                    newParam.channels[ch].volumeR -= newParam.channels[ch].volumeR > 0 ? 1 : 0;
                }

                newParam.channels[ch].srcFreq = (int)ppz8State[ch].srcFrequency;
                newParam.channels[ch].freq = (int)ppz8State[ch].frequency;

                newParam.channels[ch].note = Common.searchSegaPCMNote(ppz8State[ch].frequency / (double)0x8000);
                if (!ppz8State[ch].playing) newParam.channels[ch].note = -1;

                newParam.channels[ch].dda = ppz8State[ch].bank != 0;
                newParam.channels[ch].flg16 = ppz8State[ch].num;

                newParam.channels[ch].sadr = ppz8State[ch].ptr;
                newParam.channels[ch].eadr = ppz8State[ch].end;
                newParam.channels[ch].ladr = ppz8State[ch].loopStartOffset;
                newParam.channels[ch].leadr = ppz8State[ch].loopEndOffset;
                newParam.channels[ch].volumeRL = ppz8State[ch].volume;
                newParam.channels[ch].volumeRR = ppz8State[ch].pan;
                newParam.channels[ch].mask = ppz8State[ch].mask;
            }
        }

        public void screenDrawParams()
        {
            int tp = 0;// ((chipID == 0) ? parent.setting.PPZ8Type.UseScci : parent.setting.PPZ8SType.UseScci) ? 1 : 0;

            for (int c = 0; c < 8; c++)
            {

                MDChipParams.Channel orc = oldParam.channels[c];
                MDChipParams.Channel nrc = newParam.channels[c];

                DrawBuff.ChPPZ8(frameBuffer, c, ref orc.mask, nrc.mask, tp);

                DrawBuff.VolumeXY(frameBuffer, 64, c * 2 + 2, 1, ref orc.volumeL, nrc.volumeL, tp);
                DrawBuff.VolumeXY(frameBuffer, 64, c * 2 + 3, 1, ref orc.volumeR, nrc.volumeR, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref orc.note, nrc.note, tp);
                DrawBuff.PanType3(frameBuffer, c, ref orc.pan, nrc.pan, tp);

                //DrawBuff.ChC140(frameBuffer, c, ref orc.mask, nrc.mask, tp);

                DrawBuff.drawNESSw(frameBuffer, 4 * 4, c * 8 + 8 * 10, ref oldParam.channels[c].dda, newParam.channels[c].dda);

                uint dmy;

                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 9, c * 8 + 8 * 10, 0, ref orc.flg16, nrc.flg16);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 15, c * 8 + 8 * 10, 0, ref orc.srcFreq, nrc.srcFreq);

                dmy = (uint)orc.freq;
                DrawBuff.font4Hex32Bit(frameBuffer, 4 * 21, c * 8 + 8 * 10, 0, ref dmy, (uint)nrc.freq);
                orc.freq = (int)dmy;

                dmy = (uint)orc.sadr;
                DrawBuff.font4Hex32Bit(frameBuffer, 4 * 31, c * 8 + 8 * 10, 0, ref dmy, (uint)nrc.sadr);
                orc.sadr = (int)dmy;

                dmy = (uint)orc.eadr;
                DrawBuff.font4Hex32Bit(frameBuffer, 4 * 41, c * 8 + 8 * 10, 0, ref dmy, (uint)nrc.eadr);
                orc.eadr = (int)dmy;

                dmy = (uint)orc.ladr;
                DrawBuff.font4Hex32Bit(frameBuffer, 4 * 51, c * 8 + 8 * 10, 0, ref dmy, (uint)nrc.ladr);
                orc.ladr = (int)dmy;

                dmy = (uint)orc.leadr;
                DrawBuff.font4Hex32Bit(frameBuffer, 4 * 61, c * 8 + 8 * 10, 0, ref dmy, (uint)nrc.leadr);
                orc.leadr = (int)dmy;

                DrawBuff.font4HexByte(frameBuffer, 4 * 71, c * 8 + 8 * 10, 0, ref orc.volumeRL, nrc.volumeRL);
                DrawBuff.font4HexByte(frameBuffer, 4 * 75, c * 8 + 8 * 10, 0, ref orc.volumeRR, nrc.volumeRR);
            }
        }

    }
}
