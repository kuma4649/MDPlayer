using System;
using System.Drawing;
using System.Windows.Forms;
using MDPlayer.Properties;

namespace MDPlayer.form
{
    public partial class frmAY8910 : frmChipBase
    {

        public frmAY8910(frmMain frm, int chipID, int zoom, MDChipParams.AY8910 newParam, MDChipParams.AY8910 oldParam) : base(frm, chipID, zoom, newParam)
        {
            InitializeComponent();

            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(this.pbScreen, Resources.planeAY8910, null, zoom);

            bool AY8910Type = (chipID == 0) 
                ? parent.setting.AY8910Type[0].UseReal[0] 
                : parent.setting.AY8910Type[1].UseReal[0];
            int AY8910SoundLocation = (chipID == 0) 
                ? parent.setting.AY8910Type[0].realChipInfo[0].SoundLocation 
                : parent.setting.AY8910Type[1].realChipInfo[0].SoundLocation;
            int tp = !AY8910Type ? 0 : (AY8910SoundLocation < 0 ? 2 : 1);

            screenInitAY8910(frameBuffer, tp);
            update();
        }

        private void frmAY8910_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosAY8910[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosAY8910[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmAY8910_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeAY8910.Width * zoom, frameSizeH + Resources.planeAY8910.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeAY8910.Width * zoom, frameSizeH + Resources.planeAY8910.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeAY8910.Width * zoom, frameSizeH + Resources.planeAY8910.Height * zoom);
            frmAY8910_Resize(null, null);

        }

        private void frmAY8910_Resize(object sender, EventArgs e)
        {

        }

        override public void screenChangeParams()
        {
            int[] AY8910Register = Audio.GetAY8910Register(chipID);

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.channels[ch];

                bool t = (AY8910Register[0x07] & (0x1 << ch)) == 0;
                bool n = (AY8910Register[0x07] & (0x8 << ch)) == 0;
                //Console.WriteLine("r[8]={0:x} r[9]={1:x} r[10]={2:x}", AY8910Register[0x8], AY8910Register[0x9], AY8910Register[0xa]);
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);
                newParam.nfrq = AY8910Register[0x06] & 0x1f;
                newParam.efrq = AY8910Register[0x0c] * 0x100 + AY8910Register[0x0b];
                newParam.etype = (AY8910Register[0x0d] & 0xf);

                int v = (AY8910Register[0x08 + ch] & 0x1f);
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
                    int ft = AY8910Register[0x00 + ch * 2];
                    int ct = AY8910Register[0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = Audio.clockAY8910 / (8.0f * (float)tp); 
                    channel.note = searchSSGNote(ftone);
                }

            }
        }

        public static void screenInitAY8910(FrameBuffer screen, int tp)
        {
            for (int ch = 0; ch < 3; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(screen, 32 + kx, ch * 8 + 8, kt, tp);
                }
                DrawBuff.drawFont8(screen, 296, ch * 8 + 8, 1, "   ");

                //Volume
                int d = 99;
                DrawBuff.Volume(screen, 256, 8 + ch * 8, 0, ref d, 0, tp);

                bool? db = null;
                DrawBuff.ChAY8910(screen, ch, ref db, false, tp);
            }
        }

        override public void screenDrawParams()
        {
            bool AY8910Type = (chipID == 0)
                ? parent.setting.AY8910Type[0].UseReal[0]
                : parent.setting.AY8910Type[1].UseReal[0];
            int AY8910SoundLocation = (chipID == 0)
                ? parent.setting.AY8910Type[0].realChipInfo[0].SoundLocation
                : parent.setting.AY8910Type[1].realChipInfo[0].SoundLocation;
            int tp = !AY8910Type ? 0 : (AY8910SoundLocation < 0 ? 2 : 1);

            for (int c = 0; c < 3; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.ToneNoise(frameBuffer, 6, 2, c, ref oyc.tn, nyc.tn, ref oyc.tntp, tp * 2 + (nyc.mask == true ? 1 : 0));

                DrawBuff.ChAY8910(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            DrawBuff.Nfrq(frameBuffer, 5, 8, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.Efrq(frameBuffer, 18, 8, ref oldParam.efrq, newParam.efrq);
            DrawBuff.Etype(frameBuffer, 33, 8, ref oldParam.etype, newParam.etype);

        }

        override public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
                newParam.channels[c].volume = -1;
                newParam.channels[c].tn = -1;
            }
            newParam.nfrq = 0;
            newParam.efrq = 0;
            newParam.etype = 0;

            bool AY8910Type = (chipID == 0)
                ? parent.setting.AY8910Type[0].UseReal[0]
                : parent.setting.AY8910Type[1].UseReal[0];
            int AY8910SoundLocation = (chipID == 0)
                ? parent.setting.AY8910Type[0].realChipInfo[0].SoundLocation
                : parent.setting.AY8910Type[1].realChipInfo[0].SoundLocation;
            int tp = !AY8910Type ? 0 : (AY8910SoundLocation < 0 ? 2 : 1);

            screenInitAY8910(frameBuffer, tp);
            update();
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
                            parent.ResetChannelMask(EnmChip.AY8910, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.AY8910, chipID, ch);
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
                    parent.SetChannelMask(EnmChip.AY8910, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 3; ch++) parent.ResetChannelMask(EnmChip.AY8910, chipID, ch);
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
