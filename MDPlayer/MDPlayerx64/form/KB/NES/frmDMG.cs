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
    public partial class frmDMG : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.DMG newParam = null;
        private MDChipParams.DMG oldParam = new MDChipParams.DMG();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmDMG(frmMain frm, int chipID, int zoom, MDChipParams.DMG newParam, MDChipParams.DMG oldParam) : base(frm)
        {
            InitializeComponent();

            this.chipID = chipID;
            this.zoom = zoom;
            this.newParam = newParam;
            this.oldParam = oldParam;

            frameBuffer.Add(this.pbScreen, Resources.planeDMG, null, zoom);
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

        private void frmDMG_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosDMG[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosDMG[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmDMG_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new Size(frameSizeW + Resources.planeDMG.Width * zoom, frameSizeH + Resources.planeDMG.Height * zoom);
            this.MinimumSize = new Size(frameSizeW + Resources.planeDMG.Width * zoom, frameSizeH + Resources.planeDMG.Height * zoom);
            this.Size = new Size(frameSizeW + Resources.planeDMG.Width * zoom, frameSizeH + Resources.planeDMG.Height * zoom);
            frmDMG_Resize(null, null);

        }

        private void frmDMG_Resize(object sender, EventArgs e)
        {

        }

        public void screenChangeParams()
        {
            MDSound.gb.gb_sound_t dat = Audio.GetDMGRegister(chipID);
            if (dat == null) return;

            //pan
            newParam.channels[0].pan = (dat.snd_control.mode1_left * 2) + dat.snd_control.mode1_right;
            newParam.channels[1].pan = (dat.snd_control.mode2_left * 2) + dat.snd_control.mode2_right;
            newParam.channels[2].pan = (dat.snd_control.mode3_left * 2) + dat.snd_control.mode3_right;
            newParam.channels[3].pan = (dat.snd_control.mode4_left * 2) + dat.snd_control.mode4_right;

            //freq
            newParam.channels[0].freq = dat.snd_1.frequency;
            newParam.channels[1].freq = dat.snd_2.frequency;
            newParam.channels[2].freq = dat.snd_3.frequency;
            newParam.channels[3].freq = dat.snd_4.reg[3] & 0x7;//pfq
            newParam.channels[3].bit[47] = (dat.snd_4.reg[3] & 0x8) != 0;//poly
            newParam.channels[3].srcFreq = (dat.snd_4.reg[3] & 0xf0) >> 4;//pc

            //CC
            newParam.channels[0].bit[0] = dat.snd_1.length_enabled;
            newParam.channels[1].bit[0] = dat.snd_2.length_enabled;
            newParam.channels[2].bit[0] = dat.snd_3.length_enabled;
            newParam.channels[3].bit[0] = dat.snd_4.length_enabled;

            //Ini
            newParam.channels[0].bit[1] = (dat.snd_1.reg[4] & 0x80) != 0;
            newParam.channels[1].bit[1] = (dat.snd_2.reg[4] & 0x80) != 0;
            newParam.channels[2].bit[1] = (dat.snd_3.reg[4] & 0x80) != 0;
            newParam.channels[3].bit[1] = (dat.snd_4.reg[4] & 0x80) != 0;

            //Env.Dir
            newParam.channels[0].bit[2] = dat.snd_1.envelope_direction == 1;
            newParam.channels[1].bit[2] = dat.snd_2.envelope_direction == 1;
            //newParam.channels[2].bit[2] = ないよ
            newParam.channels[3].bit[2] = dat.snd_4.envelope_direction == 1;

            //Sweep Dec
            newParam.channels[0].bit[3] = dat.snd_1.sweep_direction == -1;

            //Env.Spd
            newParam.channels[0].inst[0] = dat.snd_1.envelope_time;
            newParam.channels[1].inst[0] = dat.snd_2.envelope_time;
            //newParam.channels[2].inst[0] = ないよ
            newParam.channels[3].inst[0] = dat.snd_4.envelope_time;

            //Env.Vol
            newParam.channels[0].inst[1] = dat.snd_1.envelope_value;
            newParam.channels[1].inst[1] = dat.snd_2.envelope_value;
            //newParam.channels[2].inst[1] = ないよ
            newParam.channels[3].inst[1] = dat.snd_4.envelope_value;

            //Len
            newParam.channels[0].inst[2] = dat.snd_1.length;
            newParam.channels[1].inst[2] = dat.snd_2.length;
            //newParam.channels[2].inst[2] = ないよ
            newParam.channels[3].inst[2] = dat.snd_4.length;

            //Duty
            newParam.channels[0].inst[3] = dat.snd_1.duty;
            newParam.channels[1].inst[3] = dat.snd_2.duty;
            //newParam.channels[2].inst[3] = ないよ
            //newParam.channels[3].inst[3] = ないよ

            //Sweep time
            newParam.channels[0].inst[4] = dat.snd_1.sweep_time;
            //Sweep shift
            newParam.channels[0].inst[5] = dat.snd_1.sweep_shift;

            //Len
            newParam.channels[2].inst[4] = dat.snd_3.length;
            //Vol
            newParam.channels[2].inst[5] = dat.snd_3.level;

            //wf
            for (int i = 0; i < 16; i++)
            {
                newParam.wf[i * 2] = (byte)((dat.snd_regs[0x20 + i] >> 4) & 0xf);
                newParam.wf[i * 2 + 1] = (byte)(dat.snd_regs[0x20 + i] & 0xf);
            }

            int r = 10;
            newParam.channels[0].volumeL = Math.Min((dat.snd_1.envelope_value * dat.snd_control.mode1_left) * 16 / r, 19);
            newParam.channels[0].volumeR = Math.Min((dat.snd_1.envelope_value * dat.snd_control.mode1_right) * 16 / r, 19);
            newParam.channels[1].volumeL = Math.Min((dat.snd_2.envelope_value * dat.snd_control.mode2_left) * 16 / r, 19);
            newParam.channels[1].volumeR = Math.Min((dat.snd_2.envelope_value * dat.snd_control.mode2_right) * 16 / r, 19);
            int lvl = dat.snd_3.level == 0 ? 0 : (19 >> (dat.snd_3.level - 1));
            newParam.channels[2].volumeL = Math.Min(lvl * dat.snd_control.mode3_left * 19 / r, 19);
            newParam.channels[2].volumeR = Math.Min(lvl * dat.snd_control.mode3_right * 19 / r, 19);
            newParam.channels[3].volumeL = Math.Min((dat.snd_4.envelope_value * dat.snd_control.mode4_left) * 16 / r, 19);
            newParam.channels[3].volumeR = Math.Min((dat.snd_4.envelope_value * dat.snd_control.mode4_right) * 16 / r, 19);

            float ftone;

            for (int i = 0; i < 3; i++)
            {
                newParam.channels[i].note = -1;
                if (newParam.channels[i].volumeL != 0 || newParam.channels[i].volumeR != 0)
                {
                    ftone = 4194304.0f / (4 * 2 * (2048.0f - (float)newParam.channels[i].freq));
                    newParam.channels[i].note = Math.Max(Math.Min(searchSSGNote(ftone) , 8 * 12), 0);
                }
            }
        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc = oldParam.channels[0];
            MDChipParams.Channel nyc = newParam.channels[0];
            DrawBuff.Pan(frameBuffer, 24, 8, ref oyc.pan, nyc.pan, ref oyc.pantp, 0);
            DrawBuff.font4Hex12Bit(frameBuffer, 260, 8, 0, ref oyc.freq, nyc.freq);
            DrawBuff.VolumeXY(frameBuffer, 68, 2, 1, ref oyc.volumeL, nyc.volumeL, 0);
            DrawBuff.VolumeXY(frameBuffer, 68, 3, 1, ref oyc.volumeR, nyc.volumeR, 0);
            DrawBuff.drawNESSw(frameBuffer, 60, 40, ref oyc.bit[0], nyc.bit[0]);//CC
            DrawBuff.drawNESSw(frameBuffer, 60, 48, ref oyc.bit[1], nyc.bit[1]);//Ini
            DrawBuff.drawNESSw(frameBuffer, 28, 64, ref oyc.bit[2], nyc.bit[2]);//Env.Dir
            DrawBuff.drawNESSw(frameBuffer, 88, 56, ref oyc.bit[3], nyc.bit[3]);//Sweep Dec
            DrawBuff.font4Int1(frameBuffer, 28, 48, 0, ref oyc.inst[0], nyc.inst[0]);//Env. Spd
            DrawBuff.font4Int2(frameBuffer, 24, 56, 0, 1, ref oyc.inst[1], nyc.inst[1]);//Env. Vol
            DrawBuff.font4Int2(frameBuffer, 56, 64, 0, 1, ref oyc.inst[2], nyc.inst[2]);//Len
            DrawBuff.font4Int1(frameBuffer, 60, 56, 0, ref oyc.inst[3], nyc.inst[3]);//Duty
            DrawBuff.font4Int1(frameBuffer, 88, 48, 0, ref oyc.inst[4], nyc.inst[4]);//Sweep time
            DrawBuff.font4Int1(frameBuffer, 88, 64, 0, ref oyc.inst[5], nyc.inst[5]);//Sweep shift
            DrawBuff.KeyBoardDMG(frameBuffer, 0, ref oyc.note, nyc.note, 0);
            DrawBuff.ChDMG(frameBuffer,  0, ref oyc.mask, nyc.mask,0);

            oyc = oldParam.channels[1];
            nyc = newParam.channels[1];
            DrawBuff.Pan(frameBuffer, 24, 16, ref oyc.pan, nyc.pan, ref oyc.pantp, 0);
            DrawBuff.font4Hex12Bit(frameBuffer, 260, 16, 0, ref oyc.freq, nyc.freq);
            DrawBuff.VolumeXY(frameBuffer, 68, 4, 1, ref oyc.volumeL, nyc.volumeL, 0);
            DrawBuff.VolumeXY(frameBuffer, 68, 5, 1, ref oyc.volumeR, nyc.volumeR, 0);
            DrawBuff.drawNESSw(frameBuffer, 152, 40, ref oyc.bit[0], nyc.bit[0]);//CC
            DrawBuff.drawNESSw(frameBuffer, 152, 48, ref oyc.bit[1], nyc.bit[1]);//Ini
            DrawBuff.drawNESSw(frameBuffer, 120, 64, ref oyc.bit[2], nyc.bit[2]);//Env.Dir
            DrawBuff.font4Int1(frameBuffer, 120, 48, 0, ref oyc.inst[0], nyc.inst[0]);//Env. Spd
            DrawBuff.font4Int2(frameBuffer, 116, 56, 0, 1, ref oyc.inst[1], nyc.inst[1]);//Env. Vol
            DrawBuff.font4Int2(frameBuffer, 148, 64, 0, 1, ref oyc.inst[2], nyc.inst[2]);//Len
            DrawBuff.font4Int1(frameBuffer, 152, 56, 0, ref oyc.inst[3], nyc.inst[3]);//Duty
            DrawBuff.KeyBoardDMG(frameBuffer, 1, ref oyc.note, nyc.note, 0);
            DrawBuff.ChDMG(frameBuffer,  1, ref oyc.mask, nyc.mask, 0);

            oyc = oldParam.channels[2];
            nyc = newParam.channels[2];
            DrawBuff.Pan(frameBuffer, 24, 24, ref oyc.pan, nyc.pan, ref oyc.pantp, 0);
            DrawBuff.font4Hex12Bit(frameBuffer, 260, 24, 0, ref oyc.freq, nyc.freq);
            DrawBuff.VolumeXY(frameBuffer, 68, 6, 1, ref oyc.volumeL, nyc.volumeL, 0);
            DrawBuff.VolumeXY(frameBuffer, 68, 7, 1, ref oyc.volumeR, nyc.volumeR, 0);
            DrawBuff.drawNESSw(frameBuffer, 228, 40, ref oyc.bit[0], nyc.bit[0]);//CC
            DrawBuff.drawNESSw(frameBuffer, 228, 48, ref oyc.bit[1], nyc.bit[1]);//Ini
            //Env.Dirなし
            DrawBuff.font4Int2(frameBuffer, 220, 56, 0, 3, ref oyc.inst[4], nyc.inst[4]);//Len
            DrawBuff.font4Int1(frameBuffer, 228, 64, 0, ref oyc.inst[5], nyc.inst[5]);//Vol
            DrawBuff.KeyBoardDMG(frameBuffer, 2, ref oyc.note, nyc.note, 0);
            DrawBuff.ChDMG(frameBuffer,  2, ref oyc.mask, nyc.mask, 0);

            oyc = oldParam.channels[3];
            nyc = newParam.channels[3];
            DrawBuff.Pan(frameBuffer, 24, 32, ref oyc.pan, nyc.pan, ref oyc.pantp, 0);
            DrawBuff.VolumeXY(frameBuffer, 68, 8, 1, ref oyc.volumeL, nyc.volumeL, 0);
            DrawBuff.VolumeXY(frameBuffer, 68, 9, 1, ref oyc.volumeR, nyc.volumeR, 0);
            DrawBuff.font4Int1(frameBuffer, 316, 40, 0, ref oyc.freq, nyc.freq);
            DrawBuff.drawNESSw(frameBuffer, 316, 48, ref oyc.bit[47], nyc.bit[47]);
            DrawBuff.font4Int2(frameBuffer, 312, 56, 0, 1, ref oyc.srcFreq, nyc.srcFreq);
            DrawBuff.drawNESSw(frameBuffer, 288, 40, ref oyc.bit[0], nyc.bit[0]);//CC
            DrawBuff.drawNESSw(frameBuffer, 288, 48, ref oyc.bit[1], nyc.bit[1]);//Ini
            DrawBuff.drawNESSw(frameBuffer, 260, 64, ref oyc.bit[2], nyc.bit[2]);//Env.Dir
            DrawBuff.font4Int1(frameBuffer, 260, 48, 0, ref oyc.inst[0], nyc.inst[0]);//Env. Spd
            DrawBuff.font4Int2(frameBuffer, 256, 56, 0, 1, ref oyc.inst[1], nyc.inst[1]);//Env. Vol
            DrawBuff.font4Int2(frameBuffer, 284, 64, 0, 1, ref oyc.inst[2], nyc.inst[2]);//Len
            DrawBuff.ChDMG(frameBuffer,  3, ref oyc.mask, nyc.mask, 0);

            DrawBuff.WaveFormToDMG(frameBuffer, 168, 58, ref oldParam.wf, newParam.wf);//wave form
        }

        public void screenInit()
        {
            for (int c = 0; c < 3; c++)
            {
                newParam.channels[c].note = -1;
                newParam.channels[c].volume = -1;
                newParam.channels[c].tn = -1;
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 32 + kx, c * 8 + 8, kt, 0);
                }
            }
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
                    for (int ch = 0; ch < 4; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.DMG, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.DMG, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤
            if (py < 5 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(EnmChip.DMG, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 4; ch++) parent.ResetChannelMask(EnmChip.DMG, chipID, ch);
                return;
            }

        }

        private int searchSSGNote(float freq)
        {
            float m = float.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 9; i++)
            {
                float a = Math.Abs((freq / (1 << (6 - 4))) - Tables.freqTbl[i]);// 6:正規の範囲   4:補正
                if (m > a)
                {
                    m = a;
                    n = i;
                }
                else break;
            }
            return n;
        }

    }
}
