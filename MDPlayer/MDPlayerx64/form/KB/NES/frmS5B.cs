#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmS5B : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.S5B newParam = null;
        private MDChipParams.S5B oldParam = new MDChipParams.S5B();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmS5B(frmMain frm, int chipID, int zoom, MDChipParams.S5B newParam, MDChipParams.S5B oldParam) : base(frm)
        {
            InitializeComponent();

            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(this.pbScreen, ResMng.ImgDic["planeS5B"], null, zoom);
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

        private void frmS5B_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosS5B[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosS5B[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmS5B_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeS5B"].Width * zoom, frameSizeH + ResMng.ImgDic["planeS5B"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeS5B"].Width * zoom, frameSizeH + ResMng.ImgDic["planeS5B"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeS5B"].Width * zoom, frameSizeH + ResMng.ImgDic["planeS5B"].Height * zoom);
            frmS5B_Resize(null, null);

        }

        private void frmS5B_Resize(object sender, EventArgs e)
        {

        }

        public void screenChangeParams()
        {
            byte[] S5BRegister = Audio.GetS5BRegister(chipID);
            if (S5BRegister == null) return;

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.channels[ch];

                bool t = (S5BRegister[0x07] & (0x1 << ch)) == 0;
                bool n = (S5BRegister[0x07] & (0x8 << ch)) == 0;
                //Console.WriteLine("r[8]={0:x} r[9]={1:x} r[10]={2:x}", S5BRegister[0x8], S5BRegister[0x9], S5BRegister[0xa]);
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);
                newParam.nfrq = S5BRegister[0x06] & 0x1f;
                newParam.efrq = S5BRegister[0x0c] * 0x100 + S5BRegister[0x0b];
                newParam.etype = (S5BRegister[0x0d] & 0xf);

                int v = (S5BRegister[0x08 + ch] & 0x1f);
                v = v > 15 ? 15 : v;
                channel.volume = (int)(((t || n) ? 1 : 0) * v * (20.0 / 16.0));
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
                    int ft = S5BRegister[0x00 + ch * 2];
                    int ct = S5BRegister[0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = Audio.ClockS5B / (8.0f * (float)tp);
                    channel.note = searchSSGNote(ftone);
                }

            }
        }

        public void screenDrawParams()
        {
            //int tp = setting.S5BType.UseScci ? 1 : 0;
            int tp = 0;

            for (int c = 0; c < 3; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.ToneNoise(frameBuffer, 6, 2, c, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

                DrawBuff.ChS5B(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            DrawBuff.Nfrq(frameBuffer, 5, 8, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.Efrq(frameBuffer, 18, 8, ref oldParam.efrq, newParam.efrq);
            DrawBuff.Etype(frameBuffer, 33, 8, ref oldParam.etype, newParam.etype);

        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
                newParam.channels[c].volume = -1;
                newParam.channels[c].tn = -1;
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, c * 8 + 8, kt, 0);
                }
            }
            newParam.nfrq = 0;
            newParam.efrq = 0;
            newParam.etype = 0;
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (int ch = 0; ch < 3; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.FME7, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.FME7, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤
            if (py < 4 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(EnmChip.FME7, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 3; ch++) parent.ResetChannelMask(EnmChip.FME7, chipID, ch);
                return;
            }

        }

        private int searchSSGNote(float freq)
        {
            float m = float.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                //if (freq < Tables.freqTbl[i]) break;
                //n = i;
                float a = Math.Abs(freq - Tables.freqTbl[i]);
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
