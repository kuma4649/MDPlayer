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
    public partial class frmNESDMC : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        //
        private MDChipParams.NESDMC newParam = null;
        private MDChipParams.NESDMC oldParam = new MDChipParams.NESDMC();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmNESDMC(frmMain frm, int chipID, int zoom, MDChipParams.NESDMC newParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Resources.planeNESDMC, null, zoom);
            DrawBuff.screenInitNESDMC(frameBuffer);
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

        private void frmNESDMC_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosNESDMC[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosNESDMC[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmNESDMC_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeNESDMC.Width * zoom, frameSizeH + Resources.planeNESDMC.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeNESDMC.Width * zoom, frameSizeH + Resources.planeNESDMC.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeNESDMC.Width * zoom, frameSizeH + Resources.planeNESDMC.Height * zoom);
            frmNESDMC_Resize(null, null);

        }

        private void frmNESDMC_Resize(object sender, EventArgs e)
        {

        }


        public void screenChangeParams()
        {
            const double LOG2_440 = 8.7813597135246596040696824762152;
            const double LOG_2 = 0.69314718055994530941723212145818;
            const int NOTE_440HZ = 12 * 4 + 9;

            byte[] reg = Audio.GetAPURegister(chipID);
            int freq;
            int vol;
            int note;
            if (reg != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    freq = (reg[3 + i * 4] & 0x07) * 0x100 + reg[2 + i * 4];
                    vol = reg[i * 4] & 0xf;
                    note = 104 - (int)((12 * (Math.Log(freq) / LOG_2 - LOG2_440) + NOTE_440HZ + 0.5));
                    note = vol == 0 ? -1 : note;
                    newParam.sqrChannels[i].note = note;
                    newParam.sqrChannels[i].volume = Math.Min((int)((vol) * 1.33), 19);
                    newParam.sqrChannels[i].nfrq = (reg[1 + i * 4] & 0x70) >> 4;//Period
                    newParam.sqrChannels[i].pan = (reg[1 + i * 4] & 0x07);//Shift
                    newParam.sqrChannels[i].pantp = (reg[3 + i * 4] & 0xf8) >> 3;//Length counter load
                    newParam.sqrChannels[i].kf = (reg[i * 4] & 0xc0) >> 6;//Duty
                    newParam.sqrChannels[i].dda = ((reg[i * 4] & 0x20) >> 5) != 0;//LengthCounter
                    newParam.sqrChannels[i].noise = ((reg[i * 4] & 0x10) >> 4) != 0;//constantVolume
                    newParam.sqrChannels[i].volumeL = ((reg[1 + i * 4] & 0x80) >> 7);//Sweep unit enabled
                    newParam.sqrChannels[i].volumeR = ((reg[1 + i * 4] & 0x08) >> 3);//negate 
                }


            }

            byte[] reg2 = Audio.GetDMCRegister(chipID);
            int step = 0;
            if (reg2 == null)
            {
                step = 8;
                reg2 = reg;
            }
            if (reg2 == null) return;

            freq = (reg2[step+3] & 0x07) * 0x100 + reg2[step + 2];
            note = 92 - (int)((12 * (Math.Log(freq) / LOG_2 - LOG2_440) + NOTE_440HZ + 0.5));
            newParam.triChannel.note = (reg2[step + 0] & 0x7f) == 0 ? -1 : note;
            if ((reg2[step + 0] & 0x80) == 0)
            {
                if ((reg2[step + 13] & 0x04) == 0)
                    //if ((reg2[step + 1] & 0x4) == 0)
                    newParam.triChannel.note = -1;
            }

            newParam.dmcChannel.volumeR = (reg2[step + 9] & 0x7f); //Load counter 

            newParam.triChannel.volume = newParam.triChannel.note <0 ? 0 : (10 + (128 - newParam.dmcChannel.volumeR) / 128 * 9);
            newParam.triChannel.dda = (reg2[step + 0] & 0x80) != 0;//LengthCounterHalt
            newParam.triChannel.nfrq = (reg2[step + 0] & 0x7f);// linear counter load (R) 
            newParam.triChannel.pantp = (reg2[step + 3] & 0xf8) >> 3;//Length counter load

            newParam.noiseChannel.volume = Math.Min((int)((reg2[step + 4] & 0xf) * 1.33), 19);
            newParam.noiseChannel.dda = (reg2[step + 4] & 0x20) != 0; //Envelope loop / length counter halt
            newParam.noiseChannel.noise = (reg2[step + 4] & 0x10) != 0; //constant volume 
            newParam.noiseChannel.volumeL = (reg2[step + 6] & 0x80) >> 7; //Loop noise 
            newParam.noiseChannel.volumeR = reg2[step + 6] & 0x0f; //noise period 
            newParam.noiseChannel.nfrq = (reg2[step + 7] & 0xf8) >> 3; //Length counter load 
                                                                //newParam.noiseChannel.volume = ((reg2[1] & 0x8) != 0) ? newParam.noiseChannel.volume : 0;
            newParam.noiseChannel.volume = 
                ((reg2[step + 13] & 0x8) != 0) 
                ? ((reg2[step + 4] & 0x10) != 0 
                    ? newParam.noiseChannel.volume 
                    : (10 + (128 - newParam.dmcChannel.volumeR) / 128 * 9)) 
                : 0;

            newParam.dmcChannel.dda = (reg2[step + 8] & 0x80) != 0; //IRQ enable
            newParam.dmcChannel.noise = (reg2[step + 8] & 0x40) != 0; //loop 
            newParam.dmcChannel.volumeL = (reg2[step + 8] & 0x0f); //frequency
            newParam.dmcChannel.nfrq = reg2[step + 10]; //Sample address
            newParam.dmcChannel.pantp = reg2[step + 11]; //Sample length
            newParam.dmcChannel.volume = 
                ((reg2[step + 13] & 0x10) == 0) 
                ? 0 
                : (10 + (128 - newParam.dmcChannel.volumeR) / 128 * 9);
        }

        public void screenDrawParams()
        {
            bool ob;
            for (int i = 0; i < 2; i++)
            {
                DrawBuff.KeyBoard(frameBuffer, i * 2, ref oldParam.sqrChannels[i].note, newParam.sqrChannels[i].note, 0);
                DrawBuff.Volume(frameBuffer, 256, 8 + i*2 * 8, 0, ref oldParam.sqrChannels[i].volume, newParam.sqrChannels[i].volume, 0);
                DrawBuff.font4Int2(frameBuffer, 16 * 4, (2 + i * 2) * 8, 0, 2, ref oldParam.sqrChannels[i].nfrq, newParam.sqrChannels[i].nfrq);
                DrawBuff.font4Int2(frameBuffer, 19 * 4, (2 + i * 2) * 8, 0, 2, ref oldParam.sqrChannels[i].pan, newParam.sqrChannels[i].pan);
                DrawBuff.font4Int2(frameBuffer, 22 * 4, (2 + i * 2) * 8, 0, 2, ref oldParam.sqrChannels[i].pantp, newParam.sqrChannels[i].pantp);
                DrawBuff.drawDuty(frameBuffer, 24, (1 + i * 2) * 8, ref oldParam.sqrChannels[i].kf, newParam.sqrChannels[i].kf);
                DrawBuff.drawNESSw(frameBuffer, 32, (2 + i * 2) * 8, ref oldParam.sqrChannels[i].dda, newParam.sqrChannels[i].dda);
                DrawBuff.drawNESSw(frameBuffer, 40, (2 + i * 2) * 8, ref oldParam.sqrChannels[i].noise, newParam.sqrChannels[i].noise);
                ob = oldParam.sqrChannels[i].volumeL != 0;
                DrawBuff.drawNESSw(frameBuffer, 48, (2 + i * 2) * 8, ref ob, newParam.sqrChannels[i].volumeL != 0);
                oldParam.sqrChannels[i].volumeL = ob ? 1 : 0;
                ob = oldParam.sqrChannels[i].volumeR != 0;
                DrawBuff.drawNESSw(frameBuffer, 56, (2 + i * 2) * 8, ref ob, newParam.sqrChannels[i].volumeR != 0);
                oldParam.sqrChannels[i].volumeR = ob ? 1 : 0;
                DrawBuff.ChNESDMC(frameBuffer, i, ref oldParam.sqrChannels[i].mask, newParam.sqrChannels[i].mask,0);

            }

            DrawBuff.KeyBoard(frameBuffer, 4, ref oldParam.triChannel.note, newParam.triChannel.note, 0);
            DrawBuff.Volume(frameBuffer, 256, 8 + 4 * 8, 0, ref oldParam.triChannel.volume, newParam.triChannel.volume, 0);
            DrawBuff.drawNESSw(frameBuffer, 36, 6 * 8, ref oldParam.triChannel.dda, newParam.triChannel.dda);
            DrawBuff.font4Int3(frameBuffer, 13 * 4, 6 * 8, 0, 3,ref oldParam.triChannel.nfrq, newParam.triChannel.nfrq);
            DrawBuff.font4Int2(frameBuffer, 19 * 4, 6 * 8, 0, 2,ref oldParam.triChannel.pantp, newParam.triChannel.pantp);
            DrawBuff.ChNESDMC(frameBuffer, 2, ref oldParam.triChannel.mask, newParam.triChannel.mask, 0);

            DrawBuff.Volume(frameBuffer, 256, 8 + 3 * 8, 0, ref oldParam.noiseChannel.volume, newParam.noiseChannel.volume, 0);
            DrawBuff.drawNESSw(frameBuffer, 228, 32, ref oldParam.noiseChannel.dda, newParam.noiseChannel.dda);
            DrawBuff.drawNESSw(frameBuffer, 144, 32, ref oldParam.noiseChannel.noise, newParam.noiseChannel.noise);
            ob = oldParam.noiseChannel.volumeL != 0;
            DrawBuff.drawNESSw(frameBuffer, 160, 32, ref ob, newParam.noiseChannel.volumeL != 0);
            oldParam.noiseChannel.volumeL = ob ? 1 : 0;
            DrawBuff.font4Int2(frameBuffer, 176, 32, 0, 2,ref oldParam.noiseChannel.volumeR, newParam.noiseChannel.volumeR);
            DrawBuff.font4Int2(frameBuffer, 196, 32, 0, 2,ref oldParam.noiseChannel.nfrq, newParam.noiseChannel.nfrq);
            DrawBuff.ChNESDMC(frameBuffer, 3, ref oldParam.noiseChannel.mask, newParam.noiseChannel.mask, 0);

            DrawBuff.Volume(frameBuffer, 256, 8 + 5 * 8, 0, ref oldParam.dmcChannel.volume, newParam.dmcChannel.volume, 0);
            DrawBuff.drawNESSw(frameBuffer, 144, 48, ref oldParam.dmcChannel.dda, newParam.dmcChannel.dda);
            DrawBuff.drawNESSw(frameBuffer, 152, 48, ref oldParam.dmcChannel.dda, newParam.dmcChannel.noise);
            DrawBuff.font4Int2(frameBuffer, 176, 48, 0, 2,ref oldParam.dmcChannel.volumeL, newParam.dmcChannel.volumeL);
            DrawBuff.font4Int3(frameBuffer, 192, 48, 0, 3,ref oldParam.dmcChannel.volumeR, newParam.dmcChannel.volumeR);
            DrawBuff.font4HexByte(frameBuffer, 220, 48, 0, ref oldParam.dmcChannel.nfrq, newParam.dmcChannel.nfrq);
            DrawBuff.font4HexByte(frameBuffer, 244, 48, 0, ref oldParam.dmcChannel.pantp, newParam.dmcChannel.pantp);
            DrawBuff.ChNESDMC(frameBuffer, 4, ref oldParam.dmcChannel.mask, newParam.dmcChannel.mask, 0);
        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.sqrChannels.Length; c++)
            {
                newParam.sqrChannels[c].note = -1;
                newParam.sqrChannels[c].volume = 0;
                newParam.sqrChannels[c].pan = 0;
                newParam.sqrChannels[c].pantp = 0;
                newParam.sqrChannels[c].kf = 0;
                newParam.sqrChannels[c].dda = false;
                newParam.sqrChannels[c].noise = false;
                newParam.sqrChannels[c].volumeL = 0;
                newParam.sqrChannels[c].volumeR = 0;
            }
            newParam.triChannel.dda = false;
            newParam.triChannel.note = -1;
            newParam.triChannel.volume = 0;
            newParam.triChannel.nfrq = 0;
            newParam.triChannel.pantp = 0;

            newParam.noiseChannel.volume = 0;
            newParam.noiseChannel.dda = false;
            newParam.noiseChannel.noise = false;
            newParam.noiseChannel.volumeL = 0;
            newParam.noiseChannel.volumeR = 0;
            newParam.noiseChannel.nfrq = 0;
            newParam.noiseChannel.volume = 0;

            newParam.dmcChannel.dda = false;
            newParam.dmcChannel.noise = false;
            newParam.dmcChannel.volumeL = 0;
            newParam.dmcChannel.volumeR = 0;
            newParam.dmcChannel.nfrq = 0;
            newParam.dmcChannel.pantp = 0;
            newParam.dmcChannel.volume = 0;

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (int ch = 0; ch < 2; ch++)
                    {
                        if (newParam.sqrChannels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.NES, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.NES, chipID, ch);
                    }

                    if (newParam.triChannel.mask == true)
                        parent.ResetChannelMask(EnmChip.DMC, chipID, 0);
                    else
                        parent.SetChannelMask(EnmChip.DMC, chipID, 0);

                    if (newParam.noiseChannel.mask == true)
                        parent.ResetChannelMask(EnmChip.DMC, chipID, 1);
                    else
                        parent.SetChannelMask(EnmChip.DMC, chipID, 1);

                    if (newParam.dmcChannel.mask == true)
                        parent.ResetChannelMask(EnmChip.DMC, chipID, 2);
                    else
                        parent.SetChannelMask(EnmChip.DMC, chipID, 2);

                }
                return;
            }

            //鍵盤
            if (py < 7 * 8)
            {
                if (e.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        //マスク解除
                        if (i < 2) parent.ResetChannelMask(EnmChip.NES, chipID, i);
                        else parent.ResetChannelMask(EnmChip.DMC, chipID, i - 2);
                    }

                    return;
                }

                int ch = (py / 8) - 1;
                if (ch == 1) return;
                ch = ch == 3 ? 3 : (ch == 5 ? 4 : ch / 2);
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    if (ch < 2) parent.SetChannelMask(EnmChip.NES, chipID, ch);
                    else parent.SetChannelMask(EnmChip.DMC, chipID, ch - 2);

                    return;
                }

            }

        }
    }
}
