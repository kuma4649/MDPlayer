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
    public partial class frmYM2413 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2413 newParam = null;
        private MDChipParams.YM2413 oldParam = new MDChipParams.YM2413();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2413(frmMain frm, int chipID, int zoom, MDChipParams.YM2413 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeYM2413, null, zoom);
            bool YM2413Type = (chipID == 0) ? parent.setting.YM2413Type.UseScci : parent.setting.YM2413SType.UseScci;
            int tp = YM2413Type ? 1 : 0;
            DrawBuff.screenInitYM2413(frameBuffer, tp);
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

        private void frmYM2413_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmYM2413_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2413[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm2413[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2413.Width * zoom, frameSizeH + Properties.Resources.planeYM2413.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2413.Width * zoom, frameSizeH + Properties.Resources.planeYM2413.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2413.Width * zoom, frameSizeH + Properties.Resources.planeYM2413.Height * zoom);
            frmYM2413_Resize(null, null);

        }

        private void frmYM2413_Resize(object sender, EventArgs e)
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
            int[] ym2413Register = Audio.GetYM2413Register(chipID);
            MDChipParams.Channel nyc;
            ChipKeyInfo ki = Audio.getYM2413KeyInfo(chipID);

            for (int ch = 0; ch < 9; ch++)
            {
                nyc = newParam.channels[ch];

                nyc.inst[0] = (ym2413Register[0x30 + ch] & 0xf0) >> 4;
                nyc.inst[1] = (ym2413Register[0x20 + ch] & 0x20) >> 5;
                nyc.inst[2] = (ym2413Register[0x20 + ch] & 0x10) >> 4;
                nyc.inst[3] = (ym2413Register[0x30 + ch] & 0x0f);

                int freq = ym2413Register[0x10 + ch] + ((ym2413Register[0x20 + ch] & 0x1) << 8);
                int oct = ((ym2413Register[0x20 + ch] & 0xe) >> 1);

                nyc.note = common.searchSegaPCMNote(freq / 172.0) + (oct - 4) * 12;

                if (ki.On[ch])
                {
                    nyc.volumeL = (19 - nyc.inst[3]);
                }
                else
                {
                    if (nyc.inst[2] == 0) nyc.note = -1;
                    nyc.volumeL--; if (nyc.volumeL < 0) nyc.volumeL = 0;
                }

            }

            //int r = Audio.getYM2413RyhthmKeyON(chipID);

            //BD
            if (ki.On[9])
            {
                newParam.channels[9].volume = (19 - (ym2413Register[0x36] & 0x0f));
            }
            else
            {
                newParam.channels[9].volume--;
                if (newParam.channels[9].volume < 0) newParam.channels[9].volume = 0;
            }

            //SD
            if (ki.On[10])
            {
                newParam.channels[10].volume = (19 - (ym2413Register[0x37] & 0x0f));
            }
            else
            {
                newParam.channels[10].volume--;
                if (newParam.channels[10].volume < 0) newParam.channels[10].volume = 0;
            }

            //TOM
            if (ki.On[11])
            {
                newParam.channels[11].volume = 19 - ((ym2413Register[0x38] & 0xf0) >> 4);
            }
            else
            {
                newParam.channels[11].volume--;
                if (newParam.channels[11].volume < 0) newParam.channels[11].volume = 0;
            }

            //CYM
            if (ki.On[12])
            {
                newParam.channels[12].volume = 19 - (ym2413Register[0x38] & 0x0f);
            }
            else
            {
                newParam.channels[12].volume--;
                if (newParam.channels[12].volume < 0) newParam.channels[12].volume = 0;
            }

            //HH
            if (ki.On[13])
            {
                newParam.channels[13].volume = 19 - ((ym2413Register[0x37] & 0xf0) >> 4);
            }
            else
            {
                newParam.channels[13].volume--;
                if (newParam.channels[13].volume < 0) newParam.channels[13].volume = 0;
            }


            newParam.channels[0].inst[4] = (ym2413Register[0x02] & 0x3f);//TL
            newParam.channels[0].inst[5] = (ym2413Register[0x03] & 0x07);//FB

            newParam.channels[0].inst[6] = (ym2413Register[0x04] & 0xf0) >> 4;//AR
            newParam.channels[0].inst[7] = (ym2413Register[0x04] & 0x0f);//DR
            newParam.channels[0].inst[8] = (ym2413Register[0x06] & 0xf0) >> 4;//SL
            newParam.channels[0].inst[9] = (ym2413Register[0x06] & 0x0f);//RR
            newParam.channels[0].inst[10] = (ym2413Register[0x02] & 0x80) >> 7;//KL
            newParam.channels[0].inst[11] = (ym2413Register[0x00] & 0x0f);//MT
            newParam.channels[0].inst[12] = (ym2413Register[0x00] & 0x80) >> 7;//AM
            newParam.channels[0].inst[13] = (ym2413Register[0x00] & 0x40) >> 6;//VB
            newParam.channels[0].inst[14] = (ym2413Register[0x00] & 0x20) >> 5;//EG
            newParam.channels[0].inst[15] = (ym2413Register[0x00] & 0x10) >> 4;//KR
            newParam.channels[0].inst[16] = (ym2413Register[0x03] & 0x08) >> 3;//DM
            newParam.channels[0].inst[17] = (ym2413Register[0x05] & 0xf0) >> 4;//AR
            newParam.channels[0].inst[18] = (ym2413Register[0x05] & 0x0f);//DR
            newParam.channels[0].inst[19] = (ym2413Register[0x07] & 0xf0) >> 4;//SL
            newParam.channels[0].inst[20] = (ym2413Register[0x07] & 0x0f);//RR
            newParam.channels[0].inst[21] = (ym2413Register[0x03] & 0x80) >> 7;//KL
            newParam.channels[0].inst[22] = (ym2413Register[0x01] & 0x0f);//MT
            newParam.channels[0].inst[23] = (ym2413Register[0x01] & 0x80) >> 7;//AM
            newParam.channels[0].inst[24] = (ym2413Register[0x01] & 0x40) >> 6;//VB
            newParam.channels[0].inst[25] = (ym2413Register[0x01] & 0x20) >> 5;//EG
            newParam.channels[0].inst[26] = (ym2413Register[0x01] & 0x10) >> 4;//KR
            newParam.channels[0].inst[27] = (ym2413Register[0x03] & 0x10) >> 4;//DC

        }


        public void screenDrawParams()
        {
            int tp = parent.setting.YM2413Type.UseScci ? 1 : 0;

            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int c = 0; c < 9; c++)
            {

                oyc = oldParam.channels[c];
                nyc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 0, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);

                DrawBuff.drawInstNumber(frameBuffer, (c % 3) * 16 + 37, (c / 3) * 2 + 24, ref oyc.inst[0], nyc.inst[0]);
                DrawBuff.SUSFlag(frameBuffer, (c % 3) * 16 + 41, (c / 3) * 2 + 24,0, ref oyc.inst[1], nyc.inst[1]);
                DrawBuff.SUSFlag(frameBuffer, (c % 3) * 16 + 44, (c / 3) * 2 + 24,0, ref oyc.inst[2], nyc.inst[2]);
                DrawBuff.drawInstNumber(frameBuffer, (c % 3) * 16 + 46, (c / 3) * 2 + 24, ref oyc.inst[3], nyc.inst[3]);

                DrawBuff.ChYM2413(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            DrawBuff.ChYM2413(frameBuffer, 9, ref oldParam.channels[9].mask, newParam.channels[9].mask, tp);
            DrawBuff.ChYM2413(frameBuffer, 10, ref oldParam.channels[10].mask, newParam.channels[10].mask, tp);
            DrawBuff.ChYM2413(frameBuffer, 11, ref oldParam.channels[11].mask, newParam.channels[11].mask, tp);
            DrawBuff.ChYM2413(frameBuffer, 12, ref oldParam.channels[12].mask, newParam.channels[12].mask, tp);
            DrawBuff.ChYM2413(frameBuffer, 13, ref oldParam.channels[13].mask, newParam.channels[13].mask, tp);
            DrawBuff.VolumeXY(frameBuffer, 6, 20, 0, ref oldParam.channels[9].volume, newParam.channels[9].volume, tp);
            DrawBuff.VolumeXY(frameBuffer, 21, 20, 0, ref oldParam.channels[10].volume, newParam.channels[10].volume, tp);
            DrawBuff.VolumeXY(frameBuffer, 36, 20, 0, ref oldParam.channels[11].volume, newParam.channels[11].volume, tp);
            DrawBuff.VolumeXY(frameBuffer, 51, 20, 0, ref oldParam.channels[12].volume, newParam.channels[12].volume, tp);
            DrawBuff.VolumeXY(frameBuffer, 66, 20, 0, ref oldParam.channels[13].volume, newParam.channels[13].volume, tp);

            oyc = oldParam.channels[0];
            nyc = newParam.channels[0];
            DrawBuff.drawInstNumber(frameBuffer, 9, 22, ref oyc.inst[4], nyc.inst[4]); //TL
            DrawBuff.drawInstNumber(frameBuffer, 14, 22, ref oyc.inst[5], nyc.inst[5]); //FB

            for (int c = 0; c < 11; c++)
            {
                DrawBuff.drawInstNumber(frameBuffer, c * 3, 26, ref oyc.inst[6 + c], nyc.inst[6 + c]);
                DrawBuff.drawInstNumber(frameBuffer, c * 3, 28, ref oyc.inst[17 + c], nyc.inst[17 + c]);
            }
        }

        public void screenInit()
        {
            for (int ch = 0; ch < 9; ch++)
            {
                newParam.channels[ch].inst[0] = 0;
                newParam.channels[ch].inst[1] = 0;
                newParam.channels[ch].inst[2] = 0;
                newParam.channels[ch].inst[3] = 0;
                newParam.channels[ch].note = -1;
                newParam.channels[ch].volumeL = 0;
            }

            newParam.channels[9].volume = 0;
            newParam.channels[10].volume = 0;
            newParam.channels[11].volume = 0;
            newParam.channels[12].volume = 0;
            newParam.channels[13].volume = 0;

            newParam.channels[0].inst[4] = 0;
            newParam.channels[0].inst[5] = 0;
            newParam.channels[0].inst[6] = 0;
            newParam.channels[0].inst[7] = 0;
            newParam.channels[0].inst[8] = 0;
            newParam.channels[0].inst[9] = 0;
            newParam.channels[0].inst[10] = 0;
            newParam.channels[0].inst[11] = 0;
            newParam.channels[0].inst[12] = 0;
            newParam.channels[0].inst[13] = 0;
            newParam.channels[0].inst[14] = 0;
            newParam.channels[0].inst[15] = 0;
            newParam.channels[0].inst[16] = 0;
            newParam.channels[0].inst[17] = 0;
            newParam.channels[0].inst[18] = 0;
            newParam.channels[0].inst[19] = 0;
            newParam.channels[0].inst[20] = 0;
            newParam.channels[0].inst[21] = 0;
            newParam.channels[0].inst[22] = 0;
            newParam.channels[0].inst[23] = 0;
            newParam.channels[0].inst[24] = 0;
            newParam.channels[0].inst[25] = 0;
            newParam.channels[0].inst[26] = 0;
            newParam.channels[0].inst[27] = 0;

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 11 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (ch == 9)
                {
                    int x = (px / 4 - 4);
                    if (x < 0) return;
                    x /= 15;
                    if (x > 4) return;
                    ch += x;
                }

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(enmUseChip.YM2413, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 14; ch++) parent.ResetChannelMask(enmUseChip.YM2413, chipID, ch);
                return;
            }

            //音色欄
            if (py < 15 * 8 && px < 16 * 8)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(enmUseChip.YM2413, 0, chipID);
            }
        }
    }
}
