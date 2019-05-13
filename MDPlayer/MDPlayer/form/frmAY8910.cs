using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class frmAY8910 : frmChipBase
    {
        private MDChipParams.AY8910 newParam = null;
        private MDChipParams.AY8910 oldParam = null;


        public frmAY8910(frmMain frm, int chipID, int zoom, MDChipParams.AY8910 newParam) : base(frm, chipID, zoom)
        {
            InitializeComponent();

            oldParam = new MDChipParams.AY8910();
            this.newParam = newParam;
            frameBuffer.Add(this.pbScreen, Properties.Resources.planeAY8910, null, zoom);
            DrawBuff.screenInitAY8910(frameBuffer);
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
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeAY8910.Width * zoom, frameSizeH + Properties.Resources.planeAY8910.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeAY8910.Width * zoom, frameSizeH + Properties.Resources.planeAY8910.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeAY8910.Width * zoom, frameSizeH + Properties.Resources.planeAY8910.Height * zoom);
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
                newParam.etype = (AY8910Register[0x0d] & 0x7) + 2;

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

        override public void screenDrawParams()
        {
            //int tp = setting.AY8910Type.UseScci ? 1 : 0;
            int tp = 0;

            for (int c = 0; c < 3; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.ToneNoise(frameBuffer, 6, 2, c, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

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
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

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
