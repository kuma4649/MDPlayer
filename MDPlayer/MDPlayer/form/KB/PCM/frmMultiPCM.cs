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
    public partial class frmMultiPCM : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.MultiPCM newParam = null;
        private MDChipParams.MultiPCM oldParam = new MDChipParams.MultiPCM();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmMultiPCM(frmMain frm, int chipID, int zoom, MDChipParams.MultiPCM newParam, MDChipParams.MultiPCM oldParam)
        {
            InitializeComponent();

            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(pbScreen, Resources.planeMultiPCM, null, zoom);
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

        private void frmMultiPCM_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosMultiPCM[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosMultiPCM[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmMultiPCM_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeMultiPCM.Width * zoom, frameSizeH + Resources.planeMultiPCM.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeMultiPCM.Width * zoom, frameSizeH + Resources.planeMultiPCM.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeMultiPCM.Width * zoom, frameSizeH + Resources.planeMultiPCM.Height * zoom);
            frmMultiPCM_Resize(null, null);

        }

        private void frmMultiPCM_Resize(object sender, EventArgs e)
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
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            int ch;
            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (ch = 0; ch < 28; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.MultiPCM, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.MultiPCM, chipID, ch);
                    }
                }
                return;
            }

            ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 28)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.MultiPCM, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 28; ch++) parent.ResetChannelMask(EnmChip.MultiPCM, chipID, ch);
                return;

            }
        }

        public void screenInit()
        {
            bool MultiPCMType = false;// (chipID == 0) ? parent.setting.MultiPCMType.UseScci : parent.setting.MultiPCMSType.UseScci;
            int tp = MultiPCMType ? 1 : 0;
            for (int ch = 0; ch < 28; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, ch * 8 + 8, kt, tp);
                }
                //DrawBuff.drawFont8(frameBuffer, 296, ch * 8 + 8, 1, "   ");
                //DrawBuff.drawPanType2P(frameBuffer, 24, ch * 8 + 8, 0, tp);
                //DrawBuff.ChMultiPCM_P(frameBuffer, 0, 8 + ch * 8, ch, false, tp);
                //DrawBuff.Volume(frameBuffer, ch, 1, ref d, 0, tp);
                //DrawBuff.Volume(frameBuffer, ch, 2, ref d, 0, tp);
            }
        }

        private int searchMultiPCMNote(int freq)
        {
            //double m = double.MaxValue;

            //int clock = Audio.clockMultiPCM;

            int n = 0;
            //for (int i = 0; i < 12 * 8; i++)
            //{
            //    int a = (int)(
            //        0x10000 //1sample進むのに必要なカウント数
            //        * 8000.0
            //        * Tables.pcmMulTbl[i % 12 + 12]
            //        * Math.Pow(2, (i / 12 - 3 + 2))
            //        / clock
            //        );

            //    if (freq > a)
            //    {
            //        m = a;
            //        n = i;
            //    }
            //}
            return n;
        }

        public void screenChangeParams()
        {
            MDSound.multipcm._MultiPCM MultiPCMRegister = Audio.GetMultiPCMRegister(chipID);
            if (MultiPCMRegister == null) return;

            for (int ch = 0; ch < 28; ch++)
            {
                int oct = (int)(((MultiPCMRegister.Slots[ch].Regs[3] >> 4) - 1) & 0xf);
                oct = ((oct & 0x8) != 0) ? (oct - 16) : oct;
                oct = oct + 4;//基音を o5 にしてます
                int pitch = (int)(((MultiPCMRegister.Slots[ch].Regs[3] & 0xf) << 6) | (MultiPCMRegister.Slots[ch].Regs[2] >> 2));

                int nt = Math.Max(Math.Min(oct * 12 + pitch / 85, 7 * 12), 0);
                newParam.channels[ch].note = nt;

                int d = (int)(MultiPCMRegister.Slots[ch].Pan);
                d = (d == 0) ? 0xf:d;
                newParam.channels[ch].pan = (int)(((((d & 0xc) >> 2) * 4) << 4) | (((d & 0x3) * 4) << 0));

                newParam.channels[ch].bit[0] = (bool)((MultiPCMRegister.Slots[ch].Regs[4] & 0x80) != 0);
                newParam.channels[ch].freq = (int)(((MultiPCMRegister.Slots[ch].Regs[3] & 0xf) << 6) | (MultiPCMRegister.Slots[ch].Regs[2] >> 2));
                newParam.channels[ch].bit[1] = (bool)((MultiPCMRegister.Slots[ch].Regs[5] & 1) != 0);//TL Interpolation
                newParam.channels[ch].inst[1] = (int)((MultiPCMRegister.Slots[ch].Regs[5] >> 1) & 0x7f);//TL
                newParam.channels[ch].inst[2] = (int)((MultiPCMRegister.Slots[ch].Regs[6] >> 3) & 7);//LFO freq
                newParam.channels[ch].inst[3] = (int)((MultiPCMRegister.Slots[ch].Regs[6]) & 7);//PLFO
                newParam.channels[ch].inst[4] = (int)((MultiPCMRegister.Slots[ch].Regs[7]) & 7);//ALFO

                if (MultiPCMRegister.Slots[ch].Sample != null)
                {
                    newParam.channels[ch].inst[0] = (int)MultiPCMRegister.Slots[ch].Regs[1];
                    newParam.channels[ch].sadr = (int)MultiPCMRegister.Slots[ch].Sample.Start;
                    newParam.channels[ch].eadr = (int)MultiPCMRegister.Slots[ch].Sample.End;
                    newParam.channels[ch].ladr = (int)MultiPCMRegister.Slots[ch].Sample.Loop;
                    newParam.channels[ch].inst[5] = (int)MultiPCMRegister.Slots[ch].Sample.LFOVIB;
                    newParam.channels[ch].inst[6] = (int)MultiPCMRegister.Slots[ch].Sample.AR;
                    newParam.channels[ch].inst[7] = (int)MultiPCMRegister.Slots[ch].Sample.DR1;
                    newParam.channels[ch].inst[8] = (int)MultiPCMRegister.Slots[ch].Sample.DR2;
                    newParam.channels[ch].inst[9] = (int)MultiPCMRegister.Slots[ch].Sample.DL;
                    newParam.channels[ch].inst[10] = (int)MultiPCMRegister.Slots[ch].Sample.RR;
                    newParam.channels[ch].inst[11] = (int)MultiPCMRegister.Slots[ch].Sample.KRS;
                    newParam.channels[ch].inst[12] = (int)MultiPCMRegister.Slots[ch].Sample.AM;
                }

                if (newParam.channels[ch].bit[0])
                {
                    newParam.channels[ch].volumeL = 
                        Math.Min(
                            (int)(((0x7f - newParam.channels[ch].inst[1]) * ((newParam.channels[ch].pan >> 4) & 0xf) / 0xf) / 4.5)
                            , 19);
                    newParam.channels[ch].volumeR = 
                        Math.Min(
                            (int)(((0x7f - newParam.channels[ch].inst[1]) * ((newParam.channels[ch].pan) & 0xf) / 0xf) / 4.5)
                            , 19);
                }
                else
                {
                    newParam.channels[ch].note = -1;
                    if (newParam.channels[ch].volumeL > 0) newParam.channels[ch].volumeL--;
                    if (newParam.channels[ch].volumeR > 0) newParam.channels[ch].volumeR--;
                }
            }
        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int ch = 0; ch < 28; ch++)
            {
                oyc = oldParam.channels[ch];
                nyc = newParam.channels[ch];

                DrawBuff.PanType2(frameBuffer, ch, ref oyc.pan, nyc.pan, 0);

                DrawBuff.drawNESSw(frameBuffer, 64 * 4 , ch * 8 + 8, ref oldParam.channels[ch].bit[0], newParam.channels[ch].bit[0]);
                DrawBuff.font4HexByte(frameBuffer, 4 * 66, ch * 8 + 8, 0, ref oyc.inst[0], nyc.inst[0]);
                DrawBuff.font4Hex12Bit(frameBuffer, 4 * 69, ch * 8 + 8, 0, ref oyc.freq, nyc.freq);
                DrawBuff.drawNESSw(frameBuffer, 72 * 4, ch * 8 + 8, ref oldParam.channels[ch].bit[1], newParam.channels[ch].bit[1]);//TL Interpolation
                DrawBuff.font4HexByte(frameBuffer, 4 * 74, ch * 8 + 8, 0, ref oyc.inst[1], nyc.inst[1]);//TL
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 77, ch * 8 + 8, 0, ref oyc.inst[2], nyc.inst[2]);//LFO freq
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 79, ch * 8 + 8, 0, ref oyc.inst[3], nyc.inst[3]);//PLFO
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 81, ch * 8 + 8, 0, ref oyc.inst[4], nyc.inst[4]);//ALFO
                DrawBuff.font4Hex24Bit(frameBuffer, 4 * 83, ch * 8 + 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 90, ch * 8 + 8, 0, ref oyc.eadr, nyc.eadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 95, ch * 8 + 8, 0, ref oyc.ladr, nyc.ladr);
                DrawBuff.font4HexByte(frameBuffer, 4 * 100, ch * 8 + 8, 0, ref oyc.inst[5], nyc.inst[5]);//LFOVIB
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 103, ch * 8 + 8, 0, ref oyc.inst[6], nyc.inst[6]);//AR
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 105, ch * 8 + 8, 0, ref oyc.inst[7], nyc.inst[7]);//DR1
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 107, ch * 8 + 8, 0, ref oyc.inst[8], nyc.inst[8]);//DR2
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 109, ch * 8 + 8, 0, ref oyc.inst[9], nyc.inst[9]);//DL
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 111, ch * 8 + 8, 0, ref oyc.inst[10], nyc.inst[10]);//RR
                DrawBuff.font4Hex4Bit(frameBuffer, 4 * 113, ch * 8 + 8, 0, ref oyc.inst[11], nyc.inst[11]);//KRS
                DrawBuff.font4HexByte(frameBuffer, 4 * 115, ch * 8 + 8, 0, ref oyc.inst[12], nyc.inst[12]);//AM

                DrawBuff.VolumeXY(frameBuffer, 117, ch * 2 + 2, 1, ref oyc.volumeL, nyc.volumeL, 0);//Front
                DrawBuff.VolumeXY(frameBuffer, 117, ch * 2 + 3, 1, ref oyc.volumeR, nyc.volumeR, 0);//Front

                DrawBuff.KeyBoardToMultiPCM(frameBuffer, ch, ref oyc.note, nyc.note, 0);
                //DrawBuff.ChMultiPCM(frameBuffer, ch, ref oyc.mask, nyc.mask, 0);
            }
        }


    }
}
