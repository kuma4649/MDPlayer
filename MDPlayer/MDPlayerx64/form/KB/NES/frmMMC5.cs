#if X64
using MDPlayerx64;
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
    public partial class frmMMC5 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        //
        private MDChipParams.MMC5 newParam = null;
        private MDChipParams.MMC5 oldParam = new MDChipParams.MMC5();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmMMC5(frmMain frm, int chipID, int zoom, MDChipParams.MMC5 newParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, ResMng.imgDic["planeMMC5"], null, zoom);
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

        private void frmMMC5_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosMMC5[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosMMC5[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmMMC5_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeMMC5"].Width * zoom, frameSizeH + ResMng.imgDic["planeMMC5"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeMMC5"].Width * zoom, frameSizeH + ResMng.imgDic["planeMMC5"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeMMC5"].Width * zoom, frameSizeH + ResMng.imgDic["planeMMC5"].Height * zoom);
            frmMMC5_Resize(null, null);

        }

        private void frmMMC5_Resize(object sender, EventArgs e)
        {

        }

     
        public void screenChangeParams()
        {
            const double LOG2_440 = 8.7813597135246596040696824762152;
            const double LOG_2 = 0.69314718055994530941723212145818;
            const int NOTE_440HZ = 12 * 4 + 9;

            byte[] reg = Audio.GetMMC5Register(chipID);
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
                    newParam.sqrChannels[i].pantp = (reg[3 + i * 4] & 0xf8) >> 3;//Length counter load
                    newParam.sqrChannels[i].kf = (reg[i * 4] & 0xc0) >> 6;//Duty
                    newParam.sqrChannels[i].dda = ((reg[i * 4] & 0x20) >> 5) != 0;//LengthCounter
                    newParam.sqrChannels[i].noise = ((reg[i * 4] & 0x10) >> 4) != 0;//constantVolume
                }

                newParam.pcmChannel.dda = (reg[8] & 0x80) != 0;
                newParam.pcmChannel.noise = (reg[8] & 0x01) != 0;
                newParam.pcmChannel.note = (reg[9] & 0xff);
                newParam.pcmChannel.volume = (reg[9] & 0xff) >> 3;
                newParam.pcmChannel.volume = Math.Min(newParam.pcmChannel.volume, 19);
            }

        }

        public void screenDrawParams()
        {
            for (int i = 0; i < 2; i++)
            {
                DrawBuff.KeyBoard(frameBuffer, i * 2, ref oldParam.sqrChannels[i].note, newParam.sqrChannels[i].note, 0);
                DrawBuff.Volume(frameBuffer, 256, 8 + i*2 * 8, 0, ref oldParam.sqrChannels[i].volume, newParam.sqrChannels[i].volume, 0);
                DrawBuff.font4Int2(frameBuffer, 22 * 4, (2 + i * 2) * 8, 0, 2, ref oldParam.sqrChannels[i].pantp, newParam.sqrChannels[i].pantp);
                DrawBuff.drawDuty(frameBuffer, 24, (1 + i * 2) * 8, ref oldParam.sqrChannels[i].kf, newParam.sqrChannels[i].kf);
                DrawBuff.drawNESSw(frameBuffer, 32, (2 + i * 2) * 8, ref oldParam.sqrChannels[i].dda, newParam.sqrChannels[i].dda);
                DrawBuff.drawNESSw(frameBuffer, 40, (2 + i * 2) * 8, ref oldParam.sqrChannels[i].noise, newParam.sqrChannels[i].noise);
                DrawBuff.ChMMC5(frameBuffer, i, ref oldParam.sqrChannels[i].mask, newParam.sqrChannels[i].mask, 0);
            }

            DrawBuff.Volume(frameBuffer, 256, 8 + 3 * 8, 0, ref oldParam.pcmChannel.volume, newParam.pcmChannel.volume, 0);
            DrawBuff.drawNESSw(frameBuffer, 148, 32, ref oldParam.pcmChannel.dda, newParam.pcmChannel.dda);
            DrawBuff.drawNESSw(frameBuffer, 160, 32, ref oldParam.pcmChannel.noise, newParam.pcmChannel.noise);
            DrawBuff.font4HexByte(frameBuffer, 196, 32, 0, ref oldParam.pcmChannel.note, newParam.pcmChannel.note);
            DrawBuff.ChMMC5(frameBuffer, 2, ref oldParam.pcmChannel.mask, newParam.pcmChannel.mask, 0);

        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.sqrChannels.Length; c++)
            {
                newParam.sqrChannels[c].note = -1;
                newParam.sqrChannels[c].volume = 0;
                newParam.sqrChannels[c].pantp = 0;
                newParam.sqrChannels[c].kf = 0;
                newParam.sqrChannels[c].dda = false;
                newParam.sqrChannels[c].noise = false;
            }
            newParam.pcmChannel.dda = false;
            newParam.pcmChannel.noise = false;
            newParam.pcmChannel.note = -1;
            newParam.pcmChannel.volume = 0;

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
                            parent.ResetChannelMask(EnmChip.MMC5, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.MMC5, chipID, ch);
                    }
                    
                    if (newParam.pcmChannel.mask == true)
                        parent.ResetChannelMask(EnmChip.MMC5, chipID, 0);
                    else
                        parent.SetChannelMask(EnmChip.MMC5, chipID, 0);

                }
                return;
            }

            //鍵盤
            if (py < 5 * 8)
            {
                if (e.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //マスク解除
                        if (i < 3) parent.ResetChannelMask(EnmChip.MMC5, chipID, i);
                    }

                    return;
                }

                int ch = (py / 8) - 1;

                if (e.Button == MouseButtons.Left)
                {
                    switch (ch)
                    {
                        case 0:
                            ch = 0;
                            break;
                        case 2:
                            ch = 1;
                            break;
                        case 3:
                            ch = 2;
                            break;
                        default:
                            return;
                    }
                    //マスク
                    parent.SetChannelMask(EnmChip.MMC5, chipID, ch);

                    return;
                }

            }


        }

    }
}
