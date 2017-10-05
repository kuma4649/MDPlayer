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
    public partial class frmSN76489 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.SN76489 newParam = null;
        private MDChipParams.SN76489 oldParam = new MDChipParams.SN76489();
        private FrameBuffer frameBuffer = new FrameBuffer();

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

        private void frmSN76489_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosSN76489[chipID] = Location;
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

        public void screenChangeParams()
        {
            int[] psgRegister = Audio.GetPSGRegister(chipID);
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

                    newParam.channels[ch].volume = Math.Min(Math.Max((int)((psgVol[ch][0] + psgVol[ch][1]) / (30.0 / 19.0)), 0), 19);
                }

                //Noise Ch
                newParam.channels[3].note = psgRegister[6];
                newParam.channels[3].volume = Math.Min(Math.Max((int)((psgVol[3][0] + psgVol[3][1]) / (30.0 / 19.0)), 0), 19);
            }
        }


        public void screenDrawParams()
        {
            bool SN76489Type = (chipID == 0) ? parent.setting.SN76489Type.UseScci : parent.setting.SN76489SType.UseScci;
            int tp = SN76489Type ? 1 : 0;
            MDChipParams.Channel osc;
            MDChipParams.Channel nsc;

            for (int c = 0; c < 3; c++)
            {
                osc = oldParam.channels[c];
                nsc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, c, 0, ref osc.volume, nsc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref osc.note, nsc.note, tp);
                DrawBuff.ChSN76489(frameBuffer, c, ref osc.mask, nsc.mask, tp);
            }

            osc = oldParam.channels[3];
            nsc = newParam.channels[3];
            DrawBuff.Volume(frameBuffer, 3, 0, ref osc.volume, nsc.volume, tp);
            DrawBuff.ChSN76489(frameBuffer, 3, ref osc.mask, nsc.mask, tp);
            DrawBuff.ChSN76489Noise(frameBuffer, ref osc, nsc, tp);

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
                    parent.SetChannelMask(enmUseChip.SN76489, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 4; ch++) parent.ResetChannelMask(enmUseChip.SN76489, chipID, ch);
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
