using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmFDS : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.FDS newParam = null;
        private MDChipParams.FDS oldParam = new MDChipParams.FDS();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmFDS(frmMain frm, int chipID, int zoom, MDChipParams.FDS newParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Resources.planeFDS, null, zoom);
            DrawBuff.screenInitFDS(frameBuffer);
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

        private void frmFDS_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosFDS[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosFDS[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmFDS_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeFDS.Width * zoom, frameSizeH + Resources.planeFDS.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeFDS.Width * zoom, frameSizeH + Resources.planeFDS.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeFDS.Width * zoom, frameSizeH + Resources.planeFDS.Height * zoom);
            frmFDS_Resize(null, null);

        }

        private void frmFDS_Resize(object sender, EventArgs e)
        {

        }


        public void screenChangeParams()
        {
            const double LOG2_440 = 8.7813597135246596040696824762152;
            const double LOG_2 = 0.69314718055994530941723212145818;
            const int NOTE_440HZ = 12 * 4 + 9;

            MDSound.np.np_nes_fds.NES_FDS reg = Audio.GetFDSRegister(chipID);
            int freq;
            int vol;
            int note;
            if (reg != null)
            {
                freq = (int)reg.last_freq;
                vol = (int)reg.last_vol;
                note = -15 + (int)((12 * (Math.Log(freq) / LOG_2 - LOG2_440) + NOTE_440HZ + 0.5));
                note = note < 0 ? -1 : (note > 120 ? -1 : note);
                note = vol == 0 ? -1 : note;
                vol = note == -1 ? 0 : vol;
                newParam.channel.note = note;
                newParam.channel.volume = Math.Min((int)((vol) * 0.5), 19);

                for (int i = 0; i < 32; i++)
                {
                    newParam.wave[i] = (reg.wave[1][i * 2 + 0] + reg.wave[1][i * 2 + 1]) >> 2;
                    newParam.mod[i] = (reg.wave[0][i * 2 + 0] + reg.wave[0][i * 2 + 1]) << 1;
                }

                newParam.VolDir = reg.env_mode[1];
                newParam.VolSpd = (int)reg.env_speed[1];
                newParam.VolGain = (int)reg.env_out[1];
                newParam.VolDi = reg.env_halt;
                newParam.VolFrq = (int)reg.freq[1];
                newParam.VolHlR = reg.wav_halt;

                newParam.ModDir = reg.env_mode[0];
                newParam.ModSpd = (int)reg.env_speed[0];
                newParam.ModGain = (int)reg.env_out[0];
                newParam.ModDi = reg.mod_halt;
                newParam.ModFrq = (int)reg.freq[0];
                newParam.ModCnt = (int)reg.mod_pos;

                newParam.EnvSpd = (int)reg.master_env_speed;
                newParam.EnvVolSw = !reg.env_disable[1];
                newParam.EnvModSw = !reg.env_disable[0];

                newParam.MasterVol = reg.master_vol;
                newParam.WE = reg.wav_write;
            }

        }

        public void screenDrawParams()
        {
            DrawBuff.KeyBoard(frameBuffer, 0, ref oldParam.channel.note, newParam.channel.note, 0);
            DrawBuff.Volume(frameBuffer, 256, 8 + 0 * 8, 0, ref oldParam.channel.volume, newParam.channel.volume, 0);

            DrawBuff.WaveFormToFDS(frameBuffer, 0, ref oldParam.wave, newParam.wave);
            DrawBuff.WaveFormToFDS(frameBuffer, 1, ref oldParam.mod, newParam.mod);

            DrawBuff.drawNESSw(frameBuffer, 20 * 4, 6 * 4, ref oldParam.VolDir, newParam.VolDir);
            DrawBuff.font4Int2(frameBuffer, 19 * 4, 8 * 4, 0, 2, ref oldParam.VolSpd, newParam.VolSpd);
            DrawBuff.font4Int2(frameBuffer, 19 * 4, 10 * 4, 0, 2, ref oldParam.VolGain, newParam.VolGain);
            DrawBuff.drawNESSw(frameBuffer, 20 * 4, 12 * 4, ref oldParam.VolDi, newParam.VolDi);
            DrawBuff.font4Hex12Bit(frameBuffer, 26 * 4, 6 * 4, 0, ref oldParam.VolFrq, newParam.VolFrq);
            DrawBuff.drawNESSw(frameBuffer, 28 * 4, 8 * 4, ref oldParam.VolHlR, newParam.VolHlR);

            DrawBuff.drawNESSw(frameBuffer, 48 * 4, 6 * 4, ref oldParam.ModDir, newParam.ModDir);
            DrawBuff.font4Int2(frameBuffer, 47 * 4, 8 * 4, 0, 2, ref oldParam.ModSpd, newParam.ModSpd);
            DrawBuff.font4Int2(frameBuffer, 47 * 4, 10 * 4, 0, 2, ref oldParam.ModGain, newParam.ModGain);
            DrawBuff.drawNESSw(frameBuffer, 48 * 4, 12 * 4, ref oldParam.ModDi, newParam.ModDi);
            DrawBuff.font4Hex12Bit(frameBuffer, 54 * 4, 6 * 4, 0, ref oldParam.ModFrq, newParam.ModFrq);
            DrawBuff.font4Int3(frameBuffer, 54 * 4, 8 * 4, 0, 3, ref oldParam.ModCnt, newParam.ModCnt);

            DrawBuff.font4Int3(frameBuffer, 65 * 4, 6 * 4, 0, 3, ref oldParam.EnvSpd, newParam.EnvSpd);
            DrawBuff.drawNESSw(frameBuffer, 67 * 4, 8 * 4, ref oldParam.EnvVolSw, newParam.EnvVolSw);
            DrawBuff.drawNESSw(frameBuffer, 67 * 4, 10 * 4, ref oldParam.EnvModSw, newParam.EnvModSw);

            DrawBuff.font4Int2(frameBuffer, 76 * 4, 6 * 4, 0, 2, ref oldParam.MasterVol, newParam.MasterVol);
            DrawBuff.drawNESSw(frameBuffer, 77 * 4, 8 * 4, ref oldParam.WE, newParam.WE);

            DrawBuff.ChFDS(frameBuffer, 0, ref oldParam.channel.mask, newParam.channel.mask, 0);
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
                    if (newParam.channel.mask == true)
                        parent.ResetChannelMask(EnmChip.FDS, chipID, 0);
                    else
                        parent.SetChannelMask(EnmChip.FDS, chipID, 0);
                }
                return;
            }

            //鍵盤
            if (py < 2 * 8)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //マスク解除
                    parent.ResetChannelMask(EnmChip.FDS, chipID, 0);
                    return;
                }

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(EnmChip.FDS, chipID, 0);
                    return;
                }

            }

        }

        public void screenInit()
        {
            newParam.channel.note = -1;
            newParam.channel.volume = -1;
            for (int i = 0; i < 32; i++)
            {
                newParam.wave[i] = 0;
                newParam.mod[i] = 0;
            }

            newParam.VolDir = false;
            newParam.VolSpd = 0;
            newParam.VolGain = 0;
            newParam.VolDi = false;
            newParam.VolFrq = 0;
            newParam.VolHlR = false;

            newParam.ModDir = false;
            newParam.ModSpd = 0;
            newParam.ModGain = 0;
            newParam.ModDi = false;
            newParam.ModFrq = 0;
            newParam.ModCnt = 0;

            newParam.EnvSpd = 0;
            newParam.EnvVolSw = false;
            newParam.EnvModSw = false;

            newParam.MasterVol = 0;
            newParam.WE = false;
        }
    }
}
