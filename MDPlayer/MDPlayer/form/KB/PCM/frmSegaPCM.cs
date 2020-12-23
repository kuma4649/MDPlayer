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
    public partial class frmSegaPCM : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.SegaPcm newParam = null;
        private MDChipParams.SegaPcm oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmSegaPCM(frmMain frm, int chipID, int zoom, MDChipParams.SegaPcm newParam, MDChipParams.SegaPcm oldParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, Resources.planeSEGAPCM, null, zoom);
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

        private void frmSegaPCM_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosSegaPCM[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosSegaPCM[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmSegaPCM_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeSEGAPCM.Width * zoom, frameSizeH + Resources.planeSEGAPCM.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeSEGAPCM.Width * zoom, frameSizeH + Resources.planeSEGAPCM.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeSEGAPCM.Width * zoom, frameSizeH + Resources.planeSEGAPCM.Height * zoom);
            frmSegaPCM_Resize(null, null);

        }

        private void frmSegaPCM_Resize(object sender, EventArgs e)
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

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            //int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 16)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.SEGAPCM, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 16; ch++) parent.ResetChannelMask(EnmChip.SEGAPCM, chipID, ch);
                return;

            }

        }


        public void screenInit()
        {
            bool SEGAPCMType = (chipID == 0) ? parent.setting.SEGAPCMType.UseScci : parent.setting.SEGAPCMSType.UseScci;
            int tp = SEGAPCMType ? 1 : 0;
            for (int ch = 0; ch < 16; ch++)
            {
                int o = -1;
                DrawBuff.Volume(frameBuffer, 256, 8 + ch * 8, 1, ref o, 0, tp);
                o = -1;
                DrawBuff.Volume(frameBuffer, 256, 8 + ch * 8, 2, ref o, 0, tp);
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 8 + 8, kt, tp);
                }
                DrawBuff.drawFont8(frameBuffer, 296, ch * 8 + 8, 1, "   ");
                DrawBuff.drawPanType2P(frameBuffer, 24, ch * 8 + 8, 0, tp);
                DrawBuff.ChSegaPCM_P(frameBuffer, 0, 8 + ch * 8, ch, false, tp);
            }
        }

        public void screenChangeParams()
        {
            //MDSound.segapcm.segapcm_state segapcmState = Audio.GetSegaPCMRegister(chipID);
            //if (segapcmState != null && segapcmState.ram != null && segapcmState.rom != null)
            //{
            //    for (int ch = 0; ch < 16; ch++)
            //    {
            //        int l = segapcmState.ram[ch * 8 + 2] & 0x7f;
            //        int r = segapcmState.ram[ch * 8 + 3] & 0x7f;
            //        int dt = segapcmState.ram[ch * 8 + 7];
            //        double ml = dt / 256.0;

            //        int ptrRom = segapcmState.ptrRom + ((segapcmState.ram[ch * 8 + 0x86] & segapcmState.bankmask) << segapcmState.bankshift);
            //        uint addr = (uint)((segapcmState.ram[ch * 8 + 0x85] << 16) | (segapcmState.ram[ch * 8 + 0x84] << 8) | segapcmState.low[ch]);
            //        int vdt = 0;
            //        if (ptrRom + ((addr >> 8) & segapcmState.rgnmask) < segapcmState.rom.Length)
            //        {
            //            vdt = Math.Abs((sbyte)(segapcmState.rom[ptrRom + ((addr >> 8) & segapcmState.rgnmask)]) - 0x80);
            //        }
            //        byte end = (byte)(segapcmState.ram[ch * 8 + 6] + 1);
            //        if ((segapcmState.ram[ch * 8 + 0x86] & 1) != 0) vdt = 0;
            //        if ((addr >> 16) == end)
            //        {
            //            if ((segapcmState.ram[ch * 8 + 0x86] & 2) == 0)
            //                ml = 0;
            //        }

            //        newParam.channels[ch].volumeL = Math.Min(Math.Max((l * vdt) >> 8, 0), 19);
            //        newParam.channels[ch].volumeR = Math.Min(Math.Max((r * vdt) >> 8, 0), 19);
            //        if (newParam.channels[ch].volumeL == 0 && newParam.channels[ch].volumeR == 0)
            //        {
            //            ml = 0;
            //        }
            //        newParam.channels[ch].note = (ml == 0 || vdt == 0) ? -1 : (common.searchSegaPCMNote(ml));
            //        newParam.channels[ch].pan = (r >> 3) * 0x10 + (l >> 3);
            //    }
            //}

            byte[] segapcmReg = Audio.GetSEGAPCMRegister(chipID);
            bool[] segapcmKeyOn = Audio.GetSEGAPCMKeyOn(chipID);
            if (segapcmReg != null)
            {
                for (int ch = 0; ch < 16; ch++)
                {
                    int l = segapcmReg[ch * 8 + 2] & 0x7f;
                    int r = segapcmReg[ch * 8 + 3] & 0x7f;
                    int dt = segapcmReg[ch * 8 + 7];
                    double ml = dt / 256.0;

                    if (segapcmKeyOn[ch])
                    {
                        newParam.channels[ch].note = Common.searchSegaPCMNote(ml);
                        newParam.channels[ch].volumeL = Math.Min(Math.Max((l * 1) >> 1, 0), 19);
                        newParam.channels[ch].volumeR = Math.Min(Math.Max((r * 1) >> 1, 0), 19);
                    }
                    else
                    {
                        newParam.channels[ch].volumeL -= newParam.channels[ch].volumeL > 0 ? 1 : 0;
                        newParam.channels[ch].volumeR -= newParam.channels[ch].volumeR > 0 ? 1 : 0;

                        if(newParam.channels[ch].volumeL==0 && newParam.channels[ch].volumeR == 0)
                        {
                            newParam.channels[ch].note = -1;
                        }
                    }

                    newParam.channels[ch].pan = ((l >> 3) & 0xf) | (((r >> 3) & 0xf) << 4);

                    segapcmKeyOn[ch] = false;
                }
            }

        }

        public void screenDrawParams()
        {
            int tp = ((chipID == 0) ? parent.setting.SEGAPCMType.UseScci : parent.setting.SEGAPCMSType.UseScci) ? 1 : 0;

            for (int c = 0; c < 16; c++)
            {

                MDChipParams.Channel orc = oldParam.channels[c];
                MDChipParams.Channel nrc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref orc.volumeL, nrc.volumeL, tp);
                DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref orc.volumeR, nrc.volumeR, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref orc.note, nrc.note, tp);
                DrawBuff.PanType2(frameBuffer, c, ref orc.pan, nrc.pan, tp);

                DrawBuff.ChSegaPCM(frameBuffer, c, ref orc.mask, nrc.mask, tp);
            }
        }



    }
}
