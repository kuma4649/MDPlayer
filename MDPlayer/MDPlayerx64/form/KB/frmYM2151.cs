#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmYM2151 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2151 newParam = null;
        private MDChipParams.YM2151 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();


        public frmYM2151(frmMain frm, int chipID, int zoom, MDChipParams.YM2151 newParam, MDChipParams.YM2151 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeE"], null, zoom);
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

        private void frmYM2151_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2151[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm2151[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYM2151_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeE"].Width * zoom, frameSizeH + ResMng.ImgDic["planeE"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeE"].Width * zoom, frameSizeH + ResMng.ImgDic["planeE"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeE"].Width * zoom, frameSizeH + ResMng.ImgDic["planeE"].Height * zoom);
            frmYM2151_Resize(null, null);
        }

        private void frmYM2151_Resize(object sender, EventArgs e)
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
                            parent.ResetChannelMask(EnmChip.YM2151, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.YM2151, chipID, ch);
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
                    parent.SetChannelMask(EnmChip.YM2151, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(EnmChip.YM2151, chipID, ch);
                return;

            }

            // 音色表示欄の判定

            int h = (py - 9 * 8) / (6 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 8)
            {
                //クリップボードに音色をコピーする
                parent.GetInstCh(EnmChip.YM2151, instCh, chipID);
            }
        }

        public void screenInit()
        {
            bool YM2151Type = (chipID == 0)
                ? parent.setting.YM2151Type[0].UseReal[0]
                : parent.setting.YM2151Type[1].UseReal[0];
            int YM2151SoundLocation = (chipID == 0)
                ? parent.setting.YM2151Type[0].realChipInfo[0].SoundLocation
                : parent.setting.YM2151Type[1].realChipInfo[0].SoundLocation;
            int tp = !YM2151Type ? 0 : (YM2151SoundLocation < 0 ? 2 : 1);

            for (int ch = 0; ch < 8; ch++)
            {

                DrawBuff.drawFont8(frameBuffer, 296, ch * 8 + 8, 1, "   ");

                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 8 + 8, kt, tp);
                }

                DrawBuff.ChYM2151_P(frameBuffer, 0, ch * 8 + 8, ch, false, tp);
                DrawBuff.drawPanP(frameBuffer, 24, ch * 8 + 8, 3, tp);
                int d = 99;
                DrawBuff.Volume(frameBuffer, 256, 8 + ch * 8, 1, ref d, 0, tp);
                d = 99;
                DrawBuff.Volume(frameBuffer, 256, 8 + ch * 8, 2, ref d, 0, tp);

            }
        }

        // CONの接続をOPマスクの並びに変換するテーブル
        // 7  6   5   4   3   2  1  0
        // x, C2, M2, C1, M1, x, x, x

        private static byte[] md = new byte[]
        {
            0x40,
            0x40,
            0x40,
            0x40,
            0x50,
            0x70,
            0x70,
            0x78,
        };

        public void screenChangeParams()
        {
            int[] ym2151Register = Audio.GetYM2151Register(chipID);
            int[] fmKeyYM2151 = Audio.GetYM2151KeyOn(chipID);
            int[] fmYM2151Vol = Audio.GetYM2151Volume(chipID);

            for (int ch = 0; ch < 8; ch++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));
                    newParam.channels[ch].inst[i * 11 + 0] = ym2151Register[0x80 + ops + ch] & 0x1f; //AR
                    newParam.channels[ch].inst[i * 11 + 1] = ym2151Register[0xa0 + ops + ch] & 0x1f; //DR
                    newParam.channels[ch].inst[i * 11 + 2] = ym2151Register[0xc0 + ops + ch] & 0x1f; //SR
                    newParam.channels[ch].inst[i * 11 + 3] = ym2151Register[0xe0 + ops + ch] & 0x0f; //RR
                    newParam.channels[ch].inst[i * 11 + 4] = (ym2151Register[0xe0 + ops + ch] & 0xf0) >> 4;//SL
                    newParam.channels[ch].inst[i * 11 + 5] = ym2151Register[0x60 + ops + ch] & 0x7f;//TL
                    newParam.channels[ch].inst[i * 11 + 6] = (ym2151Register[0x80 + ops + ch] & 0xc0) >> 6;//KS
                    newParam.channels[ch].inst[i * 11 + 7] = ym2151Register[0x40 + ops + ch] & 0x0f;//ML
                    newParam.channels[ch].inst[i * 11 + 8] = (ym2151Register[0x40 + ops + ch] & 0x70) >> 4;//DT
                    newParam.channels[ch].inst[i * 11 + 9] = (ym2151Register[0xc0 + ops + ch] & 0xc0) >> 6;//DT2
                    newParam.channels[ch].inst[i * 11 + 10] = (ym2151Register[0xa0 + ops + ch] & 0x80) >> 7;//AM
                }
                newParam.channels[ch].inst[44] = ym2151Register[0x20 + ch] & 0x07;//AL
                newParam.channels[ch].inst[45] = (ym2151Register[0x20 + ch] & 0x38) >> 3;//FB
                newParam.channels[ch].inst[46] = (ym2151Register[0x38 + ch] & 0x3);//AMS
                newParam.channels[ch].inst[47] = (ym2151Register[0x38 + ch] & 0x70) >> 4;//PMS

                int p = (ym2151Register[0x20 + ch] & 0xc0) >> 6;
                newParam.channels[ch].pan = p == 1 ? 2 : (p == 2 ? 1 : p);
                int note = (ym2151Register[0x28 + ch] & 0x0f);
                note = (note < 3) ? note : (note < 7 ? note - 1 : (note < 11 ? note - 2 : note - 3));
                int oct = ((ym2151Register[0x28 + ch] & 0x70) >> 4);
                //newParam.ym2151[chipID].channels[ch].note = (fmKeyYM2151[ch] > 0) ? (oct * 12 + note + Audio.vgmReal.YM2151Hosei + 1 + 9) : -1;
                int hosei = 0;
                if (Audio.DriverVirtual != null)//is vgm)
                {
                    hosei = (Audio.DriverVirtual).YM2151Hosei[chipID];
                }
                newParam.channels[ch].note = ((fmKeyYM2151[ch] & 1) != 0) ? (oct * 12 + note + hosei) : -1;

                byte con = (byte)(fmKeyYM2151[ch]);
                int v = 127;
                byte m = md[ym2151Register[0x20 + ch] & 7];

                byte carrierOp = (byte)(con & m);

                //OP1 M1
                v = (((carrierOp & 0x08) != 0) && v > (ym2151Register[0x60 + ch] & 0x7f)) ? (ym2151Register[0x60 + ch] & 0x7f) : v;
                //OP3 C1
                v = (((carrierOp & 0x10) != 0) && v > (ym2151Register[0x68 + ch] & 0x7f)) ? (ym2151Register[0x68 + ch] & 0x7f) : v;
                //OP2 M2
                v = (((carrierOp & 0x20) != 0) && v > (ym2151Register[0x70 + ch] & 0x7f)) ? (ym2151Register[0x70 + ch] & 0x7f) : v;
                //OP4 C2
                v = (((carrierOp & 0x40) != 0) && v > (ym2151Register[0x78 + ch] & 0x7f)) ? (ym2151Register[0x78 + ch] & 0x7f) : v;

                newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2151Register[0x20 + ch] & 0x80) != 0 ? 1 : 0) * fmYM2151Vol[ch] / 80.0), 0), 19);
                newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2151Register[0x20 + ch] & 0x40) != 0 ? 1 : 0) * fmYM2151Vol[ch] / 80.0), 0), 19);

                newParam.channels[ch].kf = ((ym2151Register[0x30 + ch] & 0xfc) >> 2);

            }
            newParam.ne = ((ym2151Register[0x0f] & 0x80) >> 7);
            newParam.nfrq = ((ym2151Register[0x0f] & 0x1f) >> 0);
            newParam.lfrq = ((ym2151Register[0x18] & 0xff) >> 0);
            newParam.pmd = Audio.GetYM2151PMD(chipID);
            newParam.amd = Audio.GetYM2151AMD(chipID);
            newParam.waveform = ((ym2151Register[0x1b] & 0x3) >> 0);
            newParam.lfosync = ((ym2151Register[0x01] & 0x02) >> 1);

        }

        public void screenDrawParams()
        {
            for (int c = 0; c < 8; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                bool YM2151Type = (chipID == 0)
                    ? parent.setting.YM2151Type[0].UseReal[0]
                    : parent.setting.YM2151Type[1].UseReal[0];
                int YM2151SoundLocation = (chipID == 0)
                    ? parent.setting.YM2151Type[0].realChipInfo[0].SoundLocation
                    : parent.setting.YM2151Type[1].realChipInfo[0].SoundLocation;
                int tp = !YM2151Type ? 0 : (YM2151SoundLocation < 0 ? 2 : 1);

                DrawBuff.Inst(frameBuffer, 1, 11, c, oyc.inst, nyc.inst);

                DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);

                DrawBuff.ChYM2151(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

                DrawBuff.KfYM2151(frameBuffer, c, ref oyc.kf, nyc.kf);
            }

            DrawBuff.NeYM2151(frameBuffer, ref oldParam.ne, newParam.ne);
            DrawBuff.NfrqYM2151(frameBuffer, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.LfrqYM2151(frameBuffer, ref oldParam.lfrq, newParam.lfrq);
            DrawBuff.AmdYM2151(frameBuffer, ref oldParam.amd, newParam.amd);
            DrawBuff.PmdYM2151(frameBuffer, ref oldParam.pmd, newParam.pmd);
            DrawBuff.WaveFormYM2151(frameBuffer, ref oldParam.waveform, newParam.waveform);
            DrawBuff.LfoSyncYM2151(frameBuffer, ref oldParam.lfosync, newParam.lfosync);

        }

    }
}
