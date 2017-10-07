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
    public partial class frmHuC6280 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.HuC6280 newParam = null;
        private MDChipParams.HuC6280 oldParam = new MDChipParams.HuC6280();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmHuC6280(frmMain frm, int chipID, int zoom, MDChipParams.HuC6280 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeHuC6280, null, zoom);
            DrawBuff.screenInitHuC6280(frameBuffer);
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

        private void frmHuc6280_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosHuC6280[chipID] = Location;
            isClosed = true;
        }

        private void frmHuc6280_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeHuC6280.Width * zoom, frameSizeH + Properties.Resources.planeHuC6280.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeHuC6280.Width * zoom, frameSizeH + Properties.Resources.planeHuC6280.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeHuC6280.Width * zoom, frameSizeH + Properties.Resources.planeHuC6280.Height * zoom);
            frmHuc6280_Resize(null, null);

        }

        private void frmHuc6280_Resize(object sender, EventArgs e)
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

            MDSound.Ootake_PSG.huc6280_state chip = Audio.GetHuC6280Register(chipID);
            if (chip == null) return;

            for (int ch = 0; ch < 6; ch++)
            {
                MDSound.Ootake_PSG.PSG psg = chip.Psg[ch];
                if (psg == null) continue;
                MDChipParams.Channel channel = newParam.channels[ch];
                channel.volumeL = (psg.outVolumeL >> 10);
                channel.volumeR = (psg.outVolumeR >> 10);
                channel.volumeL = Math.Min(channel.volumeL, 19);
                channel.volumeR = Math.Min(channel.volumeR, 19);

                channel.pan = (int)((psg.volumeL & 0xf) | ((psg.volumeR & 0xf) << 4));

                channel.inst = psg.wave;

                channel.dda = psg.bDDA;

                int tp = (int)psg.frq;
                if (tp == 0) tp = 1;

                float ftone = 3579545.0f / 32.0f / (float)tp;
                channel.note = searchSSGNote(ftone);
                if (channel.volumeL == 0 && channel.volumeR == 0) channel.note = -1;

                if (ch < 4) continue;

                channel.noise = psg.bNoiseOn;
                channel.nfrq = (int)psg.noiseFrq;
            }

            newParam.mvolL = (int)chip.MainVolumeL;
            newParam.mvolR = (int)chip.MainVolumeR;
            newParam.LfoCtrl = (int)chip.LfoCtrl;
            newParam.LfoFrq = (int)chip.LfoFrq;


        }

        public void screenDrawParams()
        {
            int tp = parent.setting.HuC6280Type.UseScci ? 1 : 0;

            for (int c = 0; c < 6; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);

                DrawBuff.VolumeToHuC6280(frameBuffer, c, 1, ref oyc.volumeL, nyc.volumeL);
                DrawBuff.VolumeToHuC6280(frameBuffer, c, 2, ref oyc.volumeR, nyc.volumeR);
                DrawBuff.PanType2(frameBuffer, c, ref oyc.pan, nyc.pan);

                DrawBuff.WaveFormToHuC6280(frameBuffer, c, ref oyc.inst, nyc.inst);
                DrawBuff.DDAToHuC6280(frameBuffer, c, ref oyc.dda, nyc.dda);

                DrawBuff.ChHuC6280(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

                if (c < 4) continue;

                DrawBuff.NoiseToHuC6280(frameBuffer, c, ref oyc.noise, nyc.noise);
                DrawBuff.NoiseFrqToHuC6280(frameBuffer, c, ref oyc.nfrq, nyc.nfrq);

            }

            DrawBuff.MainVolumeToHuC6280(frameBuffer, 0, ref oldParam.mvolL, newParam.mvolL);
            DrawBuff.MainVolumeToHuC6280(frameBuffer, 1, ref oldParam.mvolR, newParam.mvolR);

            DrawBuff.LfoCtrlToHuC6280(frameBuffer, ref oldParam.LfoCtrl, newParam.LfoCtrl);
            DrawBuff.LfoFrqToHuC6280(frameBuffer, ref oldParam.LfoFrq, newParam.LfoFrq);

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 7 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask( enmUseChip.HuC6280, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 6; ch++) parent.ResetChannelMask( enmUseChip.HuC6280, chipID, ch);
                return;
            }

            //音色で右クリックした場合は何もしない
            if (e.Button == MouseButtons.Right) return;

            // 音色表示欄の判定
            int h = (py - 7 * 8) / (5 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(enmUseChip.HuC6280, instCh, chipID);
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
