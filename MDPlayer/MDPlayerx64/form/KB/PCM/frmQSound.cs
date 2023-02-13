#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
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
    public partial class frmQSound : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.QSound newParam = null;
        private MDChipParams.QSound oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmQSound(frmMain frm, int chipID, int zoom, MDChipParams.QSound newParam, MDChipParams.QSound oldParam) : base(frm)
        {
            InitializeComponent();

            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, Resources.planeQSound, null, zoom);
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

        private void frmQSound_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosQSound[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosQSound[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmQSound_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeQSound.Width * zoom, frameSizeH + Resources.planeQSound.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeQSound.Width * zoom, frameSizeH + Resources.planeQSound.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeQSound.Width * zoom, frameSizeH + Resources.planeQSound.Height * zoom);
            frmQSound_Resize(null, null);

        }

        private void frmQSound_Resize(object sender, EventArgs e)
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
                    for (ch = 0; ch < 19; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.QSound, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.QSound, chipID, ch);
                    }
                }
                return;
            }

            ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 19)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.QSound, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 19; ch++) parent.ResetChannelMask(EnmChip.QSound, chipID, ch);
                return;
            }
        }

        private void screenInit()
        {
            for (int ch = 0; ch < 16; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 8 + 8, kt, 0);
                }
            }
        }

        public void screenChangeParams()
        {
            ushort[] QSoundRegister = Audio.GetQSoundRegister(chipID);

            //PCM 16ch
            for (int ch = 0; ch < 16; ch++)
            {
                newParam.channels[ch].echo = QSoundRegister[ch + 0xba];
                newParam.channels[ch].freq = QSoundRegister[(ch << 3) + 2];
                newParam.channels[ch].bank = QSoundRegister[(((ch + 15) % 16) << 3) + 0];
                newParam.channels[ch].sadr = QSoundRegister[(ch << 3) + 1];
                newParam.channels[ch].eadr = QSoundRegister[(ch << 3) + 5];
                newParam.channels[ch].ladr = QSoundRegister[(ch << 3) + 4];
                //newParam.channels[ch].ladr = QSoundRegister[(ch << 3) + 3];
                int vol = QSoundRegister[(ch << 3) + 6];
                int pan = QSoundRegister[ch + 0x80] - 0x110;
                if (pan >= 97) pan = 16;//center?
                int panL = (int)(15.0 / 16.0 * (pan > 16 ? (16 - (33 - pan)) : 16));
                int panR = (int)(15.0 / 16.0 * (pan < 16 ? (16 - pan) : 16));
                newParam.channels[ch].pan = (panR << 4) | panL;
                newParam.channels[ch].volumeL = Math.Min(Math.Max(vol * panL / 256 / 16, 0), 19);
                newParam.channels[ch].volumeR = Math.Min(Math.Max(vol * panR / 256 / 16, 0), 19);

                newParam.channels[ch].note = Math.Max(Math.Min(Common.searchSegaPCMNote(newParam.channels[ch].freq /16.0/ 166.0), 7 * 12), 0);
                if (vol == 0) newParam.channels[ch].note = -1;
            }
            //ADPCM 3ch
            for (int ch = 0; ch < 3; ch++)
            {
                newParam.channels[ch + 16].bank = QSoundRegister[(ch << 2) + 0xcc];
                newParam.channels[ch + 16].sadr = QSoundRegister[(ch << 2) + 0xca];
                newParam.channels[ch + 16].eadr = QSoundRegister[(ch << 2) + 0xcb];
                int vol = (QSoundRegister[(ch << 2) + 0xcd] >> 16);
                int pan = QSoundRegister[ch + 16 + 0x80] - 0x110;
                if (pan >= 97) pan = 16;//center?
                int panL = (int)(15.0 / 16.0 * (pan > 16 ? (16 - (33 - pan)) : 16));
                int panR = (int)(15.0 / 16.0 * (pan < 16 ? (16 - pan) : 16));
                newParam.channels[ch + 16].pan = (panR << 4) | panL;
                newParam.channels[ch + 16].volumeL = Math.Min(Math.Max(vol * panL / 10, 0), 19);
                newParam.channels[ch + 16].volumeR = Math.Min(Math.Max(vol * panR / 10, 0), 19);
            }

            //echo
            newParam.channels[0].inst[0] = QSoundRegister[0x93];//feedback
            newParam.channels[0].inst[1] = QSoundRegister[0xd9];//end_pos
            newParam.channels[0].inst[2] = QSoundRegister[0xe2];//delay_update
            newParam.channels[0].inst[3] = QSoundRegister[0xe3];//next_state
            //Wet
            newParam.channels[0].inst[4] = QSoundRegister[0xde];//delay left
            newParam.channels[0].inst[5] = QSoundRegister[0xe0];//delay right
            newParam.channels[0].inst[6] = QSoundRegister[0xe4];//volume_left
            newParam.channels[0].inst[7] = QSoundRegister[0xe6];//volume right
            //Dry
            newParam.channels[0].inst[8] = QSoundRegister[0xdf];//delay left
            newParam.channels[0].inst[9] = QSoundRegister[0xe1];//delay right
            newParam.channels[0].inst[10] = QSoundRegister[0xe5];//volume_left
            newParam.channels[0].inst[11] = QSoundRegister[0xe7];//volume right
        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            //PCM 16ch
            for (int ch = 0; ch < 16; ch++)
            {
                oyc = oldParam.channels[ch];
                nyc = newParam.channels[ch];

                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 65, ch * 8 + 8, 0, ref oyc.echo, nyc.echo);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 70, ch * 8 + 8, 0, ref oyc.freq, nyc.freq);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 75, ch * 8 + 8, 0, ref oyc.bank, nyc.bank);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 80, ch * 8 + 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 85, ch * 8 + 8, 0, ref oyc.eadr, nyc.eadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 90, ch * 8 + 8, 0, ref oyc.ladr, nyc.ladr);
                DrawBuff.PanType2(frameBuffer, ch, ref oyc.pan, nyc.pan, 0);
                DrawBuff.VolumeXY(frameBuffer, 94, ch * 2 + 2, 1, ref oyc.volumeL, nyc.volumeL, 0);
                DrawBuff.VolumeXY(frameBuffer, 94, ch * 2 + 3, 1, ref oyc.volumeR, nyc.volumeR, 0);
                DrawBuff.KeyBoardToQSound(frameBuffer, ch , ref oyc.note, nyc.note, 0);

                DrawBuff.ChQSound(frameBuffer, ch, ref oyc.mask, nyc.mask, 0);
            }
            //ADPCM 3ch
            for (int ch = 0; ch < 3; ch++)
            {
                oyc = oldParam.channels[ch + 16];
                nyc = newParam.channels[ch + 16];
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 75, (ch + 16) * 8 + 8, 0, ref oyc.bank, nyc.bank);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 80, (ch + 16) * 8 + 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 85, (ch + 16) * 8 + 8, 0, ref oyc.eadr, nyc.eadr);
                //DrawBuff.PanType2(frameBuffer, (ch + 16), ref oyc.pan, nyc.pan, 0);
                DrawBuff.VolumeXY(frameBuffer, 94, (ch + 16) * 2 + 2, 1, ref oyc.volumeL, nyc.volumeL, 0);
                DrawBuff.VolumeXY(frameBuffer, 94, (ch + 16) * 2 + 3, 1, ref oyc.volumeR, nyc.volumeR, 0);

                DrawBuff.ChQSound(frameBuffer, ch + 16, ref oyc.mask, nyc.mask, 0);
            }

            //echo
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 36, 17 * 8 + 8, 0, ref oldParam.channels[0].inst[0], newParam.channels[0].inst[0]);//feedback
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 36, 18 * 8 + 8, 0, ref oldParam.channels[0].inst[1], newParam.channels[0].inst[1]);//end_pos
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 51, 17 * 8 + 8, 0, ref oldParam.channels[0].inst[2], newParam.channels[0].inst[2]);//delay_update
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 51, 18 * 8 + 8, 0, ref oldParam.channels[0].inst[3], newParam.channels[0].inst[3]);//next_state

            //Wet
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 07, 17 * 8 + 8, 0, ref oldParam.channels[0].inst[4], newParam.channels[0].inst[4]);//delay l
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 12, 17 * 8 + 8, 0, ref oldParam.channels[0].inst[5], newParam.channels[0].inst[5]);//delay r
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 07, 18 * 8 + 8, 0, ref oldParam.channels[0].inst[6], newParam.channels[0].inst[6]);//vol l
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 12, 18 * 8 + 8, 0, ref oldParam.channels[0].inst[7], newParam.channels[0].inst[7]);//vol r

            //Dry
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 18, 17 * 8 + 8, 0, ref oldParam.channels[0].inst[8], newParam.channels[0].inst[8]);//delay l
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 23, 17 * 8 + 8, 0, ref oldParam.channels[0].inst[9], newParam.channels[0].inst[9]);//delay r
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 18, 18 * 8 + 8, 0, ref oldParam.channels[0].inst[10], newParam.channels[0].inst[10]);//vol l
            DrawBuff.font4Hex16Bit(frameBuffer, 4 * 23, 18 * 8 + 8, 0, ref oldParam.channels[0].inst[11], newParam.channels[0].inst[11]);//vol r
        }

    }
}
