#if X64
using MDPlayer.Driver.MXDRV;
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmPCM8 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.PCM8 newParam = null;
        private MDChipParams.PCM8 oldParam = new MDChipParams.PCM8();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmPCM8(frmMain frm, int chipID, int zoom, MDChipParams.PCM8 newParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planePCM8"], null, zoom);
            DrawBuff.screenInitPCM8(frameBuffer);
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

        private void frmPCM8_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosPCM8[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosPCM8[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmPCM8_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planePCM8"].Width * zoom, frameSizeH + ResMng.ImgDic["planePCM8"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planePCM8"].Width * zoom, frameSizeH + ResMng.ImgDic["planePCM8"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planePCM8"].Width * zoom, frameSizeH + ResMng.ImgDic["planePCM8"].Height * zoom);
            frmPCM8_Resize(null, null);

        }

        private void frmPCM8_Resize(object sender, EventArgs e)
        {

        }

        public void screenChangeParams()
        {
            if (Audio.DriverVirtual is not MXDRV) return;
            MXDRV mdx=Audio.DriverVirtual as MXDRV;
            MXDRV.Pcm8St[] pcm8St = mdx.pcm8St;

            for (int ch = 0; ch < pcm8St.Length; ch++)
            {
                MDChipParams.Channel nyc = newParam.channels[ch];
                if (pcm8St[ch].Keyon)
                {
                    nyc.volume = Math.Min(Math.Max((int)(((pcm8St[ch].mode >> 16) & 0x0f) * 20.0 / 16.0), 0), 19);
                    nyc.volumeL = (int)((pcm8St[ch].mode >> 16) & 0x0f);
                    nyc.freq = (int)((pcm8St[ch].mode >> 8) & 0x07);
                    nyc.pcmMode = (int)((pcm8St[ch].mode >> 0) & 0x03);
                    nyc.utp = pcm8St[ch].tablePtr;
                    nyc.utl = pcm8St[ch].length;
                    pcm8St[ch].Keyon = false;
                }
                else
                {
                    if (nyc.volume > 0) nyc.volume--;
                }
            }
        }

        public void screenDrawParams()
        {
            MDChipParams.PCM8 ost = oldParam;
            MDChipParams.PCM8 nst = newParam;
            int tp = 0;

            for (int c = 0; c < 8; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.ChPCM8(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

                int x = 10;
                DrawBuff.font4Hex32Bit(frameBuffer, (x + 0) * 4, c * 8 + 8, 0, ref oyc.utp, nyc.utp);//ptr
                DrawBuff.font4Hex32Bit(frameBuffer, (x + 10) * 4, c * 8 + 8, 0, ref oyc.utl, nyc.utl);//length

                DrawBuff.font4Int2(frameBuffer, (x + 19) * 4, c * 8 + 8, 0, 2, ref oyc.pcmMode, nyc.pcmMode);//mode
                DrawBuff.font4Int2(frameBuffer, (x + 22) * 4, c * 8 + 8, 0, 2, ref oyc.freq, nyc.freq);//rate
                DrawBuff.font4Int2(frameBuffer, (x + 51) * 4, c * 8 + 8, 0, 2, ref oyc.volumeL, nyc.volumeL);//volume

                DrawBuff.Volume(frameBuffer, (x + 54) * 4, c * 8 + 8, 0, ref oyc.volume, nyc.volume, 0);
            }

        }

        public void screenInit()
        {
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
                    for (int ch = 0; ch < 8; ch++)
                    {

                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.PCM8, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.PCM8, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤
            if (py < 9 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;
                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(EnmChip.PCM8, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 8; ch++) parent.ResetChannelMask(EnmChip.PCM8, chipID, ch);
                return;
            }

        }
    }
}
