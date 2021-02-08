using MDPlayer.Properties;
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
    public partial class frmY8950 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.Y8950 newParam = null;
        private MDChipParams.Y8950 oldParam = new MDChipParams.Y8950();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmY8950(frmMain frm, int chipID, int zoom, MDChipParams.Y8950 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Resources.planeY8950, null, zoom);
            bool Y8950Type = false;// (chipID == 0) ? parent.setting.Y8950Type.UseScci : parent.setting.Y8950Type.UseScci;
            int tp = Y8950Type ? 1 : 0;
            DrawBuff.screenInitY8950(frameBuffer, tp);
            update();
        }

        private void frmY8950_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosY8950[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosY8950[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmY8950_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmY8950_Resize(object sender, EventArgs e)
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
                    for (ch = 0; ch < 9 + 5 + 1; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.Y8950, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.Y8950, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤 FM & RHM
             ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch == 9)
            {
                ch = 14;
            }
            else if (ch == 10)
            {
                int x = (px / 4 - 1);
                if (x < 0) return;
                x /= 15;
                if (x > 4) return;
                ch += x - 1;
            }

            if (e.Button == MouseButtons.Left)
            {
                //マスク
                parent.SetChannelMask(EnmChip.Y8950, chipID, ch);
                return;
            }

            //マスク解除
            for (ch = 0; ch < 9 + 5 + 1; ch++) parent.ResetChannelMask(EnmChip.Y8950, chipID, ch);
            return;
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

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeY8950.Width * zoom, frameSizeH + Resources.planeY8950.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeY8950.Width * zoom, frameSizeH + Resources.planeY8950.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeY8950.Width * zoom, frameSizeH + Resources.planeY8950.Height * zoom);
            frmY8950_Resize(null, null);
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

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
            }
        }

        private int[] slot1Tbl = new int[] { 0, 1, 2, 6, 7, 8, 12, 13, 14 };
        private int[] slot2Tbl = new int[] { 3, 4, 5, 9, 10, 11, 15, 16, 17 };
        private static byte[] rhythmAdr = new byte[] { 0x53, 0x54, 0x52, 0x55, 0x51 };

        public void screenChangeParams()
        {
            int[] Y8950Register = Audio.GetY8950Register(chipID);
            MDChipParams.Channel nyc;
            int slot = 0;
            ChipKeyInfo ki = Audio.getY8950KeyInfo(chipID);
            MDSound.MDSound.Chip chipInfo = Audio.GetMDSChipInfo(MDSound.MDSound.enmInstrumentType.Y8950);
            uint masterClock = chipInfo == null ? 3579545 : chipInfo.Clock; //3579545 -> Default master clock

            //FM
            for (int c = 0; c < 9; c++)
            {
                nyc = newParam.channels[c];
                for (int i = 0; i < 2; i++)
                {

                    if (i == 0)
                    {
                        slot = slot1Tbl[c];
                    }
                    else
                    {
                        slot = slot2Tbl[c];
                    }
                    slot = (slot % 6) + 8 * (slot / 6);

                    //AR
                    nyc.inst[0 + i * 17] = Y8950Register[0x60 + slot] >> 4;
                    //DR
                    nyc.inst[1 + i * 17] = Y8950Register[0x60 + slot] & 0xf;
                    //SL
                    nyc.inst[2 + i * 17] = Y8950Register[0x80 + slot] >> 4;
                    //RR
                    nyc.inst[3 + i * 17] = Y8950Register[0x80 + slot] & 0xf;
                    //KL
                    nyc.inst[4 + i * 17] = Y8950Register[0x40 + slot] >> 6;
                    //TL
                    nyc.inst[5 + i * 17] = Y8950Register[0x40 + slot] & 0x3f;
                    //MT
                    nyc.inst[6 + i * 17] = Y8950Register[0x20 + slot] & 0xf;
                    //AM
                    nyc.inst[7 + i * 17] = Y8950Register[0x20 + slot] >> 7;
                    //VB
                    nyc.inst[8 + i * 17] = (Y8950Register[0x20 + slot] >> 6) & 1;
                    //EG
                    nyc.inst[9 + i * 17] = (Y8950Register[0x20 + slot] >> 5) & 1;
                    //KR
                    nyc.inst[10 + i * 17] = (Y8950Register[0x20 + slot] >> 4) & 1;
                }

                //BL
                nyc.inst[11] = (Y8950Register[0xb0 + c] >> 2) & 7;
                //FNUM
                nyc.inst[12] = Y8950Register[0xa0 + c]
                    + ((Y8950Register[0xb0 + c] & 3) << 8);

                //FB
                nyc.inst[15] = (Y8950Register[0xc0 + c] >> 1) & 7;
                //CN
                nyc.inst[14] = (Y8950Register[0xc0 + c] & 1);

                // FNUM / (2^19) * (mClock/72) * (2 ^ (block - 1)) 
                double fmus = (double)nyc.inst[12] / (1 << 19) * (masterClock / 72.0) * (1 << nyc.inst[11]);
                nyc.note = Common.searchSegaPCMNote(fmus/523.3);//523.3 -> c4

                if (ki.On[c])
                {
                    int tl1 = nyc.inst[5 + 0 * 17];
                    int tl2 = nyc.inst[5 + 1 * 17];
                    int tl = tl2;
                    if (nyc.inst[14] != 0)
                    {
                        tl = Math.Min(tl1, tl2);
                    }
                    nyc.volume = (19 * (64 - tl) / 64);
                }
                else
                {
                    if ((Y8950Register[0xb0 + c] & 0x20) == 0) nyc.note = -1;
                    nyc.volume--; if (nyc.volume < 0) nyc.volume = 0;
                }


            }
            newParam.channels[9].dda = ((Y8950Register[0xbd] >> 7) & 0x01) != 0;//DA
            newParam.channels[10].dda = ((Y8950Register[0xbd] >> 6) & 0x01) != 0;//DV

            #region リズム情報の取得

            //slot14 TL 0x51 HH
            //slot15 TL 0x52 TOM
            //slot16 TL 0x53 BD
            //slot17 TL 0x54 SD
            //slot18 TL 0x55 CYM

            for (int i = 0; i < 5; i++)
            {
                if (ki.On[i + 9])
                {
                    newParam.channels[i + 9].volume = 19 - ((Y8950Register[rhythmAdr[i]] & 0x3f) >> 2);
                }
                else
                {
                    newParam.channels[i + 9].volume--;
                    if (newParam.channels[i + 9].volume < 0) newParam.channels[i + 9].volume = 0;
                }
            }

            #endregion

            #region ADPCM

            //Delta
            newParam.channels[14].inst[12] = Y8950Register[0x10]
                + (Y8950Register[0x11] << 8);

            if (ki.On[14] || ki.Off[14])
            {
                //fSample = deltaN * 50KHz / (2^16)
                double fSample = newParam.channels[14].inst[12] * 50000.0 / (double)(1 << 16);
                int pnt = ki.Off[14] ? -1 : Common.searchSegaPCMNote(fSample / 8000.0);

                if (newParam.channels[14].note != pnt || ki.Off[14])
                {
                    newParam.channels[14].note = pnt;
                    int tl = Y8950Register[0x12];
                    newParam.channels[14].volume = pnt == -1 ? 0 : Common.Range(tl >> 3, 0, 19);
                }
                else
                {
                    newParam.channels[14].volume--; if (newParam.channels[14].volume < 0) newParam.channels[14].volume = 0;
                }
            }
            else
            {
                newParam.channels[14].note = -1;
                newParam.channels[14].volume--; if (newParam.channels[14].volume < 0) newParam.channels[14].volume = 0;
            }

            #endregion
        }

        public void screenDrawParams()
        {
            int tp = 0;// parent.setting.YMF262Type.UseScci ? 1 : 0;
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            //FM
            for (int c = 0; c < 9; c++)
            {

                oyc = oldParam.channels[c];
                nyc = newParam.channels[c];

                for (int i = 0; i < 2; i++)
                {
                    DrawBuff.font4Int2(frameBuffer, 16 + 4 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[0 + i * 17], nyc.inst[0 + i * 17]);//AR
                    DrawBuff.font4Int2(frameBuffer, 16 + 16 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[1 + i * 17], nyc.inst[1 + i * 17]);//DR
                    DrawBuff.font4Int2(frameBuffer, 16 + 28 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[2 + i * 17], nyc.inst[2 + i * 17]);//SL
                    DrawBuff.font4Int2(frameBuffer, 16 + 40 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[3 + i * 17], nyc.inst[3 + i * 17]);//RR
                    DrawBuff.font4Int2(frameBuffer, 16 + 52 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[4 + i * 17], nyc.inst[4 + i * 17]);//KL
                    DrawBuff.font4Int2(frameBuffer, 16 + 64 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[5 + i * 17], nyc.inst[5 + i * 17]);//TL
                    DrawBuff.font4Int2(frameBuffer, 16 + 76 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[6 + i * 17], nyc.inst[6 + i * 17]);//MT

                    DrawBuff.font4Int2(frameBuffer, 16 + 88 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[7 + i * 17], nyc.inst[7 + i * 17]);//AM
                    DrawBuff.font4Int2(frameBuffer, 16 + 96 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[8 + i * 17], nyc.inst[8 + i * 17]);//VB
                    DrawBuff.font4Int2(frameBuffer, 16 + 104 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[9 + i * 17], nyc.inst[9 + i * 17]);//EG
                    DrawBuff.font4Int2(frameBuffer, 16 + 112 + i * 136, c * 8 + 104, 0, 0, ref oyc.inst[10 + i * 17], nyc.inst[10 + i * 17]);//KR
                }

                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 65, c * 8 + 104, 0, 0, ref oyc.inst[11], nyc.inst[11]);//BL
                DrawBuff.font4Hex12Bit(frameBuffer, 16 + 4 * 69, c * 8 + 104, 0, ref oyc.inst[12], nyc.inst[12]);//F-Num
                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 73, c * 8 + 104, 0, 0, ref oyc.inst[14], nyc.inst[14]);//CN
                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 76, c * 8 + 104, 0, 0, ref oyc.inst[15], nyc.inst[15]);//FB
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.VolumeXY(frameBuffer, 64, c * 2 + 2, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.ChY8950(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            DrawBuff.drawNESSw(frameBuffer, 76 * 4, 11 * 8, ref oldParam.channels[9].dda, newParam.channels[9].dda);//DA
            DrawBuff.drawNESSw(frameBuffer, 80 * 4, 11 * 8, ref oldParam.channels[10].dda, newParam.channels[10].dda);//DV

            //Rhythm
            for (int c = 9; c < 14; c++)
            {
                DrawBuff.ChY8950(frameBuffer, c, ref oldParam.channels[c].mask, newParam.channels[c].mask, tp);
                DrawBuff.VolumeXY(frameBuffer, 3 + (c - 9) * 15, 11 * 2, 0, ref oldParam.channels[c].volume, newParam.channels[c].volume, tp);
            }

            //ADPCM
            DrawBuff.KeyBoard(frameBuffer, 9, ref oldParam.channels[14].note, newParam.channels[14].note, tp);
            DrawBuff.VolumeXY(frameBuffer, 64, 9 * 2 + 2, 0, ref oldParam.channels[14].volume, newParam.channels[14].volume, tp);
            DrawBuff.ChY8950(frameBuffer, 14, ref oldParam.channels[14].mask, newParam.channels[14].mask, tp);

        }

    }
}
