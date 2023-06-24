#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmOKIM6295 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.OKIM6295 newParam = null;
        private MDChipParams.OKIM6295 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmOKIM6295(frmMain frm, int chipID, int zoom, MDChipParams.OKIM6295 newParam, MDChipParams.OKIM6295 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeMSM6295"], null, zoom);
            DrawBuff.screenInitOKIM6295(frameBuffer);
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

        private void frmOKIM6295_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosOKIM6295[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosOKIM6295[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmOKIM6295_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeMSM6295"].Width * zoom, frameSizeH + ResMng.ImgDic["planeMSM6295"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeMSM6295"].Width * zoom, frameSizeH + ResMng.ImgDic["planeMSM6295"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeMSM6295"].Width * zoom, frameSizeH + ResMng.ImgDic["planeMSM6295"].Height * zoom);
            frmOKIM6295_Resize(null, null);

        }

        private void frmOKIM6295_Resize(object sender, EventArgs e)
        {

        }

        public void screenChangeParams()
        {
            MDSound.okim6295.okim6295Info info = Audio.GetOKIM6295Info(chipID);
            if (info == null) return;

            for (int c = 0; c < 4; c++)
            {
                MDChipParams.Channel nyc = newParam.channels[c];

                if (info.keyon[c])
                {
                    nyc.volume = 19;
                }
                else
                {
                    nyc.volume -= (nyc.volume > 0) ? 1 : 0;
                }
                nyc.sadr = info.chInfo[c].stAdr;
                nyc.eadr = info.chInfo[c].edAdr;
            }

            newParam.masterClock = info.masterClock;
            newParam.pin7State = info.pin7State;
            newParam.nmkBank[0] = info.nmkBank[0];
            newParam.nmkBank[1] = info.nmkBank[1];
            newParam.nmkBank[2] = info.nmkBank[2];
            newParam.nmkBank[3] = info.nmkBank[3];

        }

        public void screenDrawParams()
        {
            int tp = parent.setting.HuC6280Type[0].UseReal[0] ? 1 : 0;

            for (int c = 0; c < 4; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.ChOKIM6295(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                DrawBuff.VolumeToOKIM6295(frameBuffer, c, ref oyc.volume, nyc.volume);
                DrawBuff.font4Hex20Bit(frameBuffer, 36, 8 + c * 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex20Bit(frameBuffer, 60, 8 + c * 8, 0, ref oyc.eadr, nyc.eadr);

                DrawBuff.font4HexByte(frameBuffer, 36 + c * 16, 48, 0, ref oldParam.nmkBank[c], newParam.nmkBank[c]);
            }

            DrawBuff.font4Hex32Bit(frameBuffer, 24, 40, 0, ref oldParam.masterClock, newParam.masterClock);
            DrawBuff.font4HexByte(frameBuffer, 80, 40, 0, ref oldParam.pin7State, newParam.pin7State);
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
                    for (int ch = 0; ch < 4; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.OKIM6295, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.OKIM6295, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤
            if (py < 7 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;
                if (ch > 3) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(EnmChip.OKIM6295, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 4; ch++) parent.ResetChannelMask(EnmChip.OKIM6295, chipID, ch);
                return;
            }

        }
    }
}
