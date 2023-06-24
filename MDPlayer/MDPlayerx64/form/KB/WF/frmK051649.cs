#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmK051649 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.K051649 newParam = null;
        private MDChipParams.K051649 oldParam = new MDChipParams.K051649();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmK051649(frmMain frm, int chipID, int zoom, MDChipParams.K051649 newParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeK051649"], null, zoom);
            DrawBuff.screenInitK051649(frameBuffer);
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

        private void frmK051649_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosK051649[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosK051649[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmK051649_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeK051649"].Width * zoom, frameSizeH + ResMng.ImgDic["planeK051649"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeK051649"].Width * zoom, frameSizeH + ResMng.ImgDic["planeK051649"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeK051649"].Width * zoom, frameSizeH + ResMng.ImgDic["planeK051649"].Height * zoom);
            frmK051649_Resize(null, null);

        }

        private void frmK051649_Resize(object sender, EventArgs e)
        {

        }



        public void screenChangeParams()
        {
            MDSound.K051649.k051649_state chip = Audio.GetK051649Register(chipID);
            if (chip == null) return;

            for (int ch = 0; ch < 5; ch++)
            {
                MDSound.K051649.k051649_sound_channel psg = chip.channel_list[ch];
                if (psg == null) continue;

                MDChipParams.Channel channel = newParam.channels[ch];
                for (int i = 0; i < 32; i++) channel.inst[i] = (int)psg.waveram[i];
                float ftone = Audio.ClockK051649 / (8.0f * (float)psg.frequency);
                channel.freq = psg.frequency;
                channel.volume = psg.key != 0 ? (int)(psg.volume * 1.33) : 0;
                channel.volumeL = psg.volume;
                channel.note = (psg.key != 0 && channel.volume != 0) ? searchSSGNote(ftone) : -1;
                channel.dda = psg.key != 0;

            }
        }

        public void screenDrawParams()
        {
            int tp = parent.setting.K051649Type[0].UseReal[0] ? 1 : 0;

            for (int c = 0; c < 5; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];
                int x = c % 3;
                int y = c / 3;

                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.font4Hex12Bit(frameBuffer, x * 4 * 26 + 4 * 14, y * 8 * 6 + 8 * 11, tp, ref oyc.freq, nyc.freq);
                DrawBuff.font4Hex4Bit(frameBuffer, x * 4 * 26 + 4 * 22, y * 8 * 6 + 8 * 11, tp, ref oyc.volumeL, nyc.volumeL);
                DrawBuff.drawNESSw(frameBuffer, x * 4 * 26 + 4 * 25, y * 8 * 6 + 8 * 11, ref oyc.dda, nyc.dda);
                DrawBuff.WaveFormToK051649(frameBuffer, c, ref oyc.typ, nyc.inst);

                DrawBuff.ChK051649(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

                for (int i = 0; i < 32; i++)
                {
                    int fx = i % 8;
                    int fy = i / 8;
                    DrawBuff.font4HexByte(frameBuffer, x * 4 * 26 + 4 * 10 + fx * 8, y * 8 * 6 + 8 * 7 + fy * 8, 0, ref oyc.inst[i], nyc.inst[i]);
                }
            }
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
                    for (int ch = 0; ch < 5; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.K051649, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.K051649, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤
            if (py < 6 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    if (newParam.channels[ch].mask == true)
                        parent.ResetChannelMask(EnmChip.K051649, chipID, ch);
                    else
                        parent.SetChannelMask(EnmChip.K051649, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 5; ch++) parent.ResetChannelMask(EnmChip.K051649, chipID, ch);
                return;
            }

            //音色で右クリックした場合は何もしない
            if (e.Button == MouseButtons.Right) return;

            // 音色表示欄の判定
            int instCh = ((py < 12 * 8) ? 0 : 3) + px / (8 * 13);

            if (instCh < 5)
            {
                //クリップボードに音色をコピーする
                parent.GetInstCh(EnmChip.K051649, instCh, chipID);
            }
        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
                newParam.channels[c].volume = -1;
                newParam.channels[c].volumeL = -1;
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
