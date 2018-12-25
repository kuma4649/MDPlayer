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
    public partial class frmYMF262 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YMF262 newParam = null;
        private MDChipParams.YMF262 oldParam = new MDChipParams.YMF262();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYMF262(frmMain frm, int chipID, int zoom, MDChipParams.YMF262 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeYMF262, null, zoom);
            bool YMF262Type = false;// (chipID == 0) ? parent.setting.YMF262Type.UseScci : parent.setting.YMF262Type.UseScci;
            int tp = YMF262Type ? 1 : 0;
            DrawBuff.screenInitYMF262(frameBuffer, tp);
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

        private void frmYMF262_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmYMF262_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYmf262[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYmf262[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYMF262.Width * zoom, frameSizeH + Properties.Resources.planeYMF262.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYMF262.Width * zoom, frameSizeH + Properties.Resources.planeYMF262.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYMF262.Width * zoom, frameSizeH + Properties.Resources.planeYMF262.Height * zoom);
            frmYMF262_Resize(null, null);

        }

        private void frmYMF262_Resize(object sender, EventArgs e)
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

        private int[] slot1Tbl = new int[] { 0, 6, 1, 7, 2, 8, 12, 13, 14, 18, 24, 19, 25, 20, 26, 30, 31, 32 };
        private int[] slot2Tbl = new int[] { 3, 9, 4, 10, 5, 11, 15, 16, 17, 21, 27, 22, 28, 23, 29, 33, 34, 35 };
        private int[] chTbl = new int[] { 0, 3, 1, 4, 2, 5, 6, 7, 8 };

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
            }
        }

        public void screenChangeParams()
        {
            int[][] ymf262Register = Audio.GetYMF262Register(chipID);
            MDChipParams.Channel nyc;
            int slot = 0;
            int slotP = 0;

            //FM
            for (int c = 0; c < 18; c++)
            {
                nyc = newParam.channels[c];
                for (int i = 0; i < 2; i++)
                {

                    if (i == 0)
                    {
                        slot = slot1Tbl[c] % 18;
                        slotP = slot1Tbl[c] / 18;
                    }
                    else
                    {
                        slot = slot2Tbl[c] % 18;
                        slotP = slot2Tbl[c] / 18;
                    }
                    slot = (slot % 6) + 8 * (slot / 6);

                    //AR
                    nyc.inst[0 + i * 17] = ymf262Register[slotP][0x60 + slot] >> 4;
                    //DR
                    nyc.inst[1 + i * 17] = ymf262Register[slotP][0x60 + slot] & 0xf;
                    //SL
                    nyc.inst[2 + i * 17] = ymf262Register[slotP][0x80 + slot] >> 4;
                    //RR
                    nyc.inst[3 + i * 17] = ymf262Register[slotP][0x80 + slot] & 0xf;
                    //KL
                    nyc.inst[4 + i * 17] = ymf262Register[slotP][0x40 + slot] >> 6;
                    //TL
                    nyc.inst[5 + i * 17] = ymf262Register[slotP][0x40 + slot] & 0x3f;
                    //MT
                    nyc.inst[6 + i * 17] = ymf262Register[slotP][0x20 + slot] & 0xf;
                    //AM
                    nyc.inst[7 + i * 17] = ymf262Register[slotP][0x20 + slot] >> 7;
                    //VB
                    nyc.inst[8 + i * 17] = (ymf262Register[slotP][0x20 + slot] >> 6) & 1;
                    //EG
                    nyc.inst[9 + i * 17] = (ymf262Register[slotP][0x20 + slot] >> 5) & 1;
                    //KR
                    nyc.inst[10 + i * 17] = (ymf262Register[slotP][0x20 + slot] >> 4) & 1;
                    //WS
                    nyc.inst[13 + i * 17] = (ymf262Register[slotP][0xe0 + slot] & 7);
                }
            }

            newParam.channels[18].dda = ((ymf262Register[1][0xbd] >> 7) & 0x01) != 0;//DA
            newParam.channels[19].dda = ((ymf262Register[1][0xbd] >> 6) & 0x01) != 0;//DV

            //ConnectSelect
            for (int c = 0; c < 6; c++)
            {
                newParam.channels[c].dda = (ymf262Register[1][0x04] & (0x1 << c)) != 0;
                newParam.channels[c].inst[34] = newParam.channels[c].dda ? 1 : 0; // [
                newParam.channels[c].inst[35] = newParam.channels[c].dda ? 2 : 0; // ]
                if (newParam.channels[c].dda)
                {
                    //OP4 mode
                    int ch = (c < 3) ? c * 2 : ((c - 3) * 2 + 9);
                    //cnt=14
                    int a = newParam.channels[ch].inst[14] * 2 + newParam.channels[ch].inst[14 + 17];
                    //mod=16
                    switch (a)
                    {
                        case 0:
                            newParam.channels[ch].inst[16] = 0;
                            newParam.channels[ch].inst[16 + 17] = 0;
                            newParam.channels[ch + 1].inst[16] = 0;
                            newParam.channels[ch + 1].inst[16 + 17] = 1;
                            break;
                        case 1:
                            newParam.channels[ch].inst[16] = 0;
                            newParam.channels[ch].inst[16 + 17] = 1;
                            newParam.channels[ch + 1].inst[16] = 0;
                            newParam.channels[ch + 1].inst[16 + 17] = 1;
                            break;
                        case 2:
                            newParam.channels[ch].inst[16] = 1;
                            newParam.channels[ch].inst[16 + 17] = 0;
                            newParam.channels[ch + 1].inst[16] = 0;
                            newParam.channels[ch + 1].inst[16 + 17] = 1;
                            break;
                        case 3:
                            newParam.channels[ch].inst[16] = 1;
                            newParam.channels[ch].inst[16 + 17] = 0;
                            newParam.channels[ch + 1].inst[16] = 1;
                            newParam.channels[ch + 1].inst[16 + 17] = 1;
                            break;
                    }
                }
            }

            int ko = Audio.getYMF262FMKeyON(chipID);

            for (int c = 0; c < 18; c++)
            {
                nyc = newParam.channels[c];

                int p = c / 9;
                bool isOp4 = false;
                int adr = c % 9;
                if (adr < 6)
                {
                    if (newParam.channels[(adr / 2) + p * 3].dda) isOp4 = true;
                }
                int kadr = isOp4 ? (adr / 2) : adr;
                adr = chTbl[adr];

                //BL
                nyc.inst[11] = (ymf262Register[p][0xb0 + adr] >> 2) & 7;
                //FNUM
                nyc.inst[12] = ymf262Register[p][0xa0 + adr]
                    + ((ymf262Register[p][0xb0 + adr] & 3) << 8);

                //FB
                nyc.inst[15] = (ymf262Register[p][0xc0 + adr] >> 1) & 7;
                //CN
                nyc.inst[14] = (ymf262Register[p][0xc0 + adr] & 1);
                //PAN
                nyc.inst[36] = ymf262Register[p][0xc0 + adr] & 0x30;
                nyc.inst[36] = ((nyc.inst[36] >> 5) & 1) | ((nyc.inst[36] >> 3) & 2); //00RL0000 -> 000000LR
                //modFlg
                int n = ymf262Register[p][0xc0 + adr] & 1;
                nyc.inst[16] = n == 0 ? 0 : 1;
                nyc.inst[33] = 1;

                int nt = common.searchSegaPCMNote(nyc.inst[12] / 344.0) + (nyc.inst[11] - 4) * 12;
                if ((ko & (1 << (adr + p * 9))) != 0)
                {
                    if (nyc.note != nt)
                    {
                        nyc.note = nt;
                        int tl1 = nyc.inst[5 + 0 * 17];
                        int tl2 = nyc.inst[5 + 1 * 17];
                        int tl = tl2;
                        if (n != 0)
                        {
                            tl = Math.Min(tl1, tl2);
                        }
                        nyc.volumeL = (nyc.inst[36] & 2) != 0 ? (19 * (64 - tl) / 64) : 0;
                        nyc.volumeR = (nyc.inst[36] & 1) != 0 ? (19 * (64 - tl) / 64) : 0;
                    }
                    else
                    {
                        nyc.volumeL--; if (nyc.volumeL < 0) nyc.volumeL = 0;
                        nyc.volumeR--; if (nyc.volumeR < 0) nyc.volumeR = 0;
                    }
                }
                else
                {
                    nyc.note = -1;
                    nyc.volumeL--; if (nyc.volumeL < 0) { nyc.volumeL = 0; }
                    nyc.volumeR--; if (nyc.volumeR < 0) { nyc.volumeR = 0; }
                }


            }

            #region リズム情報の取得

            int r = Audio.getYMF262RyhthmKeyON(chipID);

            //slot14 TL 0x51 HH
            //slot15 TL 0x52 TOM
            //slot16 TL 0x53 BD
            //slot17 TL 0x54 SD
            //slot18 TL 0x55 CYM

            //BD
            if ((r & 0x10) != 0)
            {
                newParam.channels[18].volume = 19 - ((ymf262Register[0][0x53] & 0x3f) >> 2);
            }
            else
            {
                newParam.channels[18].volume--;
                if (newParam.channels[18].volume < 0) newParam.channels[18].volume = 0;
            }

            //SD
            if ((r & 0x08) != 0)
            {
                newParam.channels[19].volume = 19 - ((ymf262Register[0][0x54] & 0x3f) >> 2);
            }
            else
            {
                newParam.channels[19].volume--;
                if (newParam.channels[19].volume < 0) newParam.channels[19].volume = 0;
            }

            //TOM
            if ((r & 0x04) != 0)
            {
                newParam.channels[20].volume = 19 - ((ymf262Register[0][0x52] & 0x3f) >> 2);
            }
            else
            {
                newParam.channels[20].volume--;
                if (newParam.channels[20].volume < 0) newParam.channels[20].volume = 0;
            }

            //CYM
            if ((r & 0x02) != 0)
            {
                newParam.channels[21].volume = 19 - ((ymf262Register[0][0x55] & 0x3f) >> 2);
            }
            else
            {
                newParam.channels[21].volume--;
                if (newParam.channels[21].volume < 0) newParam.channels[21].volume = 0;
            }

            //HH
            if ((r & 0x01) != 0)
            {
                newParam.channels[22].volume = 19 - ((ymf262Register[0][0x51] & 0x3f) >> 2);
            }
            else
            {
                newParam.channels[22].volume--;
                if (newParam.channels[22].volume < 0) newParam.channels[22].volume = 0;
            }

            //Audio.resetYMF278BRyhthmKeyON(chipID);

            #endregion

        }

        public void screenDrawParams()
        {
            int tp = 0;// parent.setting.YMF262Type.UseScci ? 1 : 0;
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            //FM
            for (int c = 0; c < 18; c++)
            {

                oyc = oldParam.channels[c];
                nyc = newParam.channels[c];

                for (int i = 0; i < 2; i++)
                {
                    DrawBuff.SUSFlag(frameBuffer, 2 + i * 33, c * 2 + 42, 1, ref oyc.inst[16 + i * 17], nyc.inst[16 + i * 17]);
                    DrawBuff.font4Int2(frameBuffer, 16 + 4 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[0 + i * 17], nyc.inst[0 + i * 17]);//AR
                    DrawBuff.font4Int2(frameBuffer, 16 + 12 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[1 + i * 17], nyc.inst[1 + i * 17]);//DR
                    DrawBuff.font4Int2(frameBuffer, 16 + 20 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[2 + i * 17], nyc.inst[2 + i * 17]);//SL
                    DrawBuff.font4Int2(frameBuffer, 16 + 28 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[3 + i * 17], nyc.inst[3 + i * 17]);//RR

                    DrawBuff.font4Int2(frameBuffer, 16 + 40 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[4 + i * 17], nyc.inst[4 + i * 17]);//KL
                    DrawBuff.font4Int2(frameBuffer, 16 + 48 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[5 + i * 17], nyc.inst[5 + i * 17]);//TL

                    DrawBuff.font4Int2(frameBuffer, 16 + 60 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[6 + i * 17], nyc.inst[6 + i * 17]);//MT

                    DrawBuff.font4Int2(frameBuffer, 16 + 72 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[7 + i * 17], nyc.inst[7 + i * 17]);//AM
                    DrawBuff.font4Int2(frameBuffer, 16 + 80 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[8 + i * 17], nyc.inst[8 + i * 17]);//VB
                    DrawBuff.font4Int2(frameBuffer, 16 + 88 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[9 + i * 17], nyc.inst[9 + i * 17]);//EG
                    DrawBuff.font4Int2(frameBuffer, 16 + 96 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[10 + i * 17], nyc.inst[10 + i * 17]);//KR
                    DrawBuff.font4Int2(frameBuffer, 16 + 108 + i * 132, c * 8 + 168, 0, 0, ref oyc.inst[13 + i * 17], nyc.inst[13 + i * 17]);//WS
                }

                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 64, c * 8 + 168, 0, 0, ref oyc.inst[11], nyc.inst[11]);//BL
                DrawBuff.font4Hex12Bit(frameBuffer, 16 + 4 * 68, c * 8 + 168, 0, ref oyc.inst[12], nyc.inst[12]);//F-Num
                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 72, c * 8 + 168, 0, 0, ref oyc.inst[14], nyc.inst[14]);//CN
                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 75, c * 8 + 168, 0, 0, ref oyc.inst[15], nyc.inst[15]);//FB
                int dmy = 99;
                DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref oyc.inst[36], nyc.inst[36], ref dmy, 0);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.VolumeXY(frameBuffer, 64, c * 2 + 2, 1, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.VolumeXY(frameBuffer, 64, c * 2 + 3, 1, ref oyc.volumeR, nyc.volumeR, tp);
                DrawBuff.ChYMF262(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            for (int c = 0; c < 6; c++)
            {
                //CS
                DrawBuff.drawNESSw(frameBuffer, 4 * 4 + c * 4, 39 * 8, ref oldParam.channels[c].dda, newParam.channels[c].dda);
                int ch = (c < 3) ? c * 2 : ((c - 3) * 2 + 9);
                DrawBuff.Kakko(frameBuffer, 4 * 0, (c < 3 ? 0 : 24) + c * 16 + 168, 0, ref oldParam.channels[c].inst[34], newParam.channels[c].inst[34]);
                DrawBuff.Kakko(frameBuffer, 4 * 163, (c < 3 ? 0 : 24) + c * 16 + 168, 0, ref oldParam.channels[c].inst[35], newParam.channels[c].inst[35]);
            }
            DrawBuff.drawNESSw(frameBuffer, 13 * 4, 39 * 8, ref oldParam.channels[18].dda, newParam.channels[18].dda);//DA
            DrawBuff.drawNESSw(frameBuffer, 17 * 4, 39 * 8, ref oldParam.channels[19].dda, newParam.channels[19].dda);//DV

            for (int c = 18; c < 23; c++)
            {
                DrawBuff.ChYMF262(frameBuffer, c, ref oldParam.channels[c].mask, newParam.channels[c].mask, tp);
                DrawBuff.VolumeXY(frameBuffer, 6 + (c - 18) * 15, 19 * 2, 0, ref oldParam.channels[c].volume, newParam.channels[c].volume, tp);
            }

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤 FM & RHM
            int ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch == 18)
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
                parent.SetChannelMask(enmUseChip.YMF262, chipID, ch);
                return;
            }

            //マスク解除
            for (ch = 0; ch < 18 + 5; ch++) parent.ResetChannelMask(enmUseChip.YMF262, chipID, ch);
            return;
        }
    }
}
