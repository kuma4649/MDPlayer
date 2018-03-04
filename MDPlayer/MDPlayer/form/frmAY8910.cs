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
    public partial class frmAY8910 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.AY8910 newParam = null;
        private MDChipParams.AY8910 oldParam = new MDChipParams.AY8910();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmAY8910(frmMain frm, int chipID, int zoom, MDChipParams.AY8910 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeAY8910, null, zoom);
            DrawBuff.screenInitAY8910(frameBuffer);
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

        private void frmAY8910_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosAY8910[chipID] = Location;
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

        public void screenChangeParams()
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
                    float ftone = 7987200.0f / (64.0f * (float)tp);// 7987200 = MasterClock
                    channel.note = searchSSGNote(ftone);
                }

            }
        }

        public void screenDrawParams()
        {
            //int tp = setting.AY8910Type.UseScci ? 1 : 0;
            int tp = 0;

            for (int c = 0; c < 3; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, c, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.ToneNoise(frameBuffer, 6, 2, c, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

                DrawBuff.ChAY8910(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            DrawBuff.Nfrq(frameBuffer, 5, 8, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.Efrq(frameBuffer, 18, 8, ref oldParam.efrq, newParam.efrq);
            DrawBuff.Etype(frameBuffer, 33, 8, ref oldParam.etype, newParam.etype);

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
                    parent.SetChannelMask(enmUseChip.AY8910, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 3; ch++) parent.ResetChannelMask(enmUseChip.AY8910, chipID, ch);
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
