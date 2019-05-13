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
    public partial class frmSN76489 : frmChipBase
    {

        private MDChipParams.SN76489 newParam = null;
        private MDChipParams.SN76489 oldParam = new MDChipParams.SN76489();

        public frmSN76489(frmMain frm, int chipID, int zoom, MDChipParams.SN76489 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeSN76489, null, zoom);
            bool SN76489Type = (chipID == 0) ? parent.setting.SN76489Type.UseScci : parent.setting.SN76489SType.UseScci;
            int tp = SN76489Type ? 1 : 0;
            DrawBuff.screenInitSN76489(frameBuffer, tp);
            update();
        }

        public override void update()
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

        private void frmSN76489_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosSN76489[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosSN76489[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmSN76489_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeSN76489.Width * zoom, frameSizeH + Properties.Resources.planeSN76489.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeSN76489.Width * zoom, frameSizeH + Properties.Resources.planeSN76489.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeSN76489.Width * zoom, frameSizeH + Properties.Resources.planeSN76489.Height * zoom);
            frmSN76489_Resize(null, null);

        }

        private void frmSN76489_Resize(object sender, EventArgs e)
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

        public override void screenChangeParams()
        {
            int[] psgRegister = Audio.GetPSGRegister(chipID);
            int psgRegisterPan = Audio.GetPSGRegisterGGPanning(chipID);
            int[][] psgVol = Audio.GetPSGVolume(chipID);
            if (psgRegister != null)
            {
                //Tone Ch
                for (int ch = 0; ch < 3; ch++)
                {
                    if (psgRegister[ch * 2 + 1] != 15)
                    {
                        newParam.channels[ch].note = searchPSGNote(psgRegister[ch * 2]);
                    }
                    else
                    {
                        newParam.channels[ch].note = -1;
                    }

                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((psgVol[ch][0]) / (15.0 / 19.0)), 0), 19);
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((psgVol[ch][1]) / (15.0 / 19.0)), 0), 19);
                    newParam.channels[ch].pan = (psgRegisterPan >> ch) & 0x11;
                    newParam.channels[ch].pan = ((newParam.channels[ch].pan) & 0x1) | (newParam.channels[ch].pan >> 3);
                }

                //Noise Ch
                newParam.channels[3].note = psgRegister[6];
                newParam.channels[3].freq = psgRegister[4];//ch3Freq
                newParam.channels[3].volumeL = Math.Min(Math.Max((int)((psgVol[3][0]) / (15.0 / 19.0)), 0), 19);
                newParam.channels[3].volumeR = Math.Min(Math.Max((int)((psgVol[3][1]) / (15.0 / 19.0)), 0), 19);
                newParam.channels[3].pan = (psgRegisterPan >> 3) & 0x11;
                newParam.channels[3].pan = ((newParam.channels[3].pan) & 0x1) | (newParam.channels[3].pan >> 3);
            }
        }

        public override void screenDrawParams()
        {
            bool SN76489Type = (chipID == 0) ? parent.setting.SN76489Type.UseScci : parent.setting.SN76489SType.UseScci;
            int tp = SN76489Type ? 1 : 0;
            MDChipParams.Channel osc;
            MDChipParams.Channel nsc;

            for (int c = 0; c < 3; c++)
            {
                osc = oldParam.channels[c];
                nsc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref osc.volumeL, nsc.volumeL, tp);
                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref osc.volumeR, nsc.volumeR, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref osc.note, nsc.note, tp);
                DrawBuff.ChSN76489(frameBuffer, c, ref osc.mask, nsc.mask, tp);
                DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref osc.pan, nsc.pan, ref osc.pantp, tp);
            }

            osc = oldParam.channels[3];
            nsc = newParam.channels[3];
            DrawBuff.Volume(frameBuffer, 256, 8 + 3 * 8, 1, ref osc.volumeL, nsc.volumeL, tp);
            DrawBuff.Volume(frameBuffer, 256, 8 + 3 * 8, 2, ref osc.volumeR, nsc.volumeR, tp);
            DrawBuff.ChSN76489(frameBuffer, 3, ref osc.mask, nsc.mask, tp);
            DrawBuff.ChSN76489Noise(frameBuffer, ref osc, nsc, tp);
            DrawBuff.Pan(frameBuffer, 24, 8 + 3 * 8, ref osc.pan, nsc.pan, ref osc.pantp, tp);
            if (osc.freq != nsc.freq)
            {
                DrawBuff.drawFont4(frameBuffer, 172, 32, 0, nsc.freq.ToString("0000"));
                osc.freq = nsc.freq;
            }

        }

        public override void screenInit()
        {
            for (int ch = 0; ch < 3; ch++)
            {
                    newParam.channels[ch].note = -1;

                newParam.channels[ch].volumeL = 0;
                newParam.channels[ch].volumeR = 0;
                newParam.channels[ch].pan = 0;
            }

            newParam.channels[3].note = 0;
            newParam.channels[3].freq = 0;
            newParam.channels[3].volumeL = 0;
            newParam.channels[3].volumeR = 0;
            newParam.channels[3].pan = 0;
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 5 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(EnmChip.SN76489, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 4; ch++) parent.ResetChannelMask(EnmChip.SN76489, chipID, ch);
                return;
            }

        }

        private int searchPSGNote(int freq)
        {
            int m = int.MaxValue;
            int n = 0;

            for (int i = 0; i < 12 * 8; i++)
            {
                int a = Math.Abs(freq - Tables.PsgFNum[i]);

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
