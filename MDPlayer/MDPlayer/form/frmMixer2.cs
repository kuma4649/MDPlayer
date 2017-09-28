using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer
{
    public partial class frmMixer2 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int zoom = 1;
        private int chipn = -1;

        private MDChipParams.Mixer newParam = null;
        private MDChipParams.Mixer oldParam = new MDChipParams.Mixer();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmMixer2(frmMain frm, int zoom, MDChipParams.Mixer newParam)
        {
            parent = frm;
            this.zoom = zoom;

            InitializeComponent();
            pbScreen.MouseWheel += new MouseEventHandler(this.pbScreen_MouseWheel);

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeMixer, null, zoom);
            DrawBuff.screenInitMixer(frameBuffer);
            update();
        }

        private void pbScreen_MouseWheel(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;
            chipn = px / 20 + (py / 72) * 16;
            int delta = Math.Sign(e.Delta);

            fader(chipn, false, delta, 0);
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

        private void frmMixer2_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosMixer = Location;
            isClosed = true;
        }

        private void frmMixer2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMixer.Width * zoom, frameSizeH + Properties.Resources.planeMixer.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMixer.Width * zoom, frameSizeH + Properties.Resources.planeMixer.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeMixer.Width * zoom, frameSizeH + Properties.Resources.planeMixer.Height * zoom);
            frmMixer2_Resize(null, null);

        }

        private void frmMixer2_Resize(object sender, EventArgs e)
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

            newParam.AY8910.Volume = parent.setting.balance.AY8910Volume;
            newParam.C140.Volume = parent.setting.balance.C140Volume;
            newParam.HuC6280.Volume = parent.setting.balance.HuC6280Volume;
            newParam.OKIM6258.Volume = parent.setting.balance.OKIM6258Volume;
            newParam.OKIM6295.Volume = parent.setting.balance.OKIM6295Volume;
            newParam.PWM.Volume = parent.setting.balance.PWMVolume;
            newParam.RF5C164.Volume = parent.setting.balance.RF5C164Volume;
            newParam.SEGAPCM.Volume = parent.setting.balance.SEGAPCMVolume;
            newParam.SN76489.Volume = parent.setting.balance.SN76489Volume;
            newParam.YM2151.Volume = parent.setting.balance.YM2151Volume;
            newParam.YM2203FM.Volume = parent.setting.balance.YM2203FMVolume;
            newParam.YM2203PSG.Volume = parent.setting.balance.YM2203PSGVolume;
            newParam.YM2203.Volume = parent.setting.balance.YM2203Volume;
            newParam.YM2413.Volume = parent.setting.balance.YM2413Volume;
            newParam.YM2608Adpcm.Volume = parent.setting.balance.YM2608AdpcmVolume;
            newParam.YM2608FM.Volume = parent.setting.balance.YM2608FMVolume;
            newParam.YM2608PSG.Volume = parent.setting.balance.YM2608PSGVolume;
            newParam.YM2608Rhythm.Volume = parent.setting.balance.YM2608RhythmVolume;
            newParam.YM2608.Volume = parent.setting.balance.YM2608Volume;
            newParam.YM2610AdpcmA.Volume = parent.setting.balance.YM2610AdpcmAVolume;
            newParam.YM2610AdpcmB.Volume = parent.setting.balance.YM2610AdpcmBVolume;
            newParam.YM2610FM.Volume = parent.setting.balance.YM2610FMVolume;
            newParam.YM2610PSG.Volume = parent.setting.balance.YM2610PSGVolume;
            newParam.YM2610.Volume = parent.setting.balance.YM2610Volume;
            newParam.YM2612.Volume = parent.setting.balance.YM2612Volume;
            newParam.C352.Volume = parent.setting.balance.C352Volume;
            newParam.K054539.Volume = parent.setting.balance.K054539Volume;
            newParam.APU.Volume = parent.setting.balance.APUVolume;
            newParam.DMC.Volume = parent.setting.balance.DMCVolume;
            newParam.FDS.Volume = parent.setting.balance.FDSVolume;
            newParam.MMC5.Volume = parent.setting.balance.MMC5Volume;
            newParam.N160.Volume = parent.setting.balance.N160Volume;
            newParam.VRC6.Volume = parent.setting.balance.VRC6Volume;
            newParam.VRC7.Volume = parent.setting.balance.VRC7Volume;
            newParam.FME7.Volume = parent.setting.balance.FME7Volume;

            newParam.Master.Volume = parent.setting.balance.MasterVolume;
            newParam.Master.VisVolume1 = common.Range(Audio.visVolume.master / 250, 0, 44);//(short.MaxValue / 44);
            if (newParam.Master.VisVolume2 <= newParam.Master.VisVolume1)
            {
                newParam.Master.VisVolume2 = newParam.Master.VisVolume1;
                newParam.Master.VisVol2Cnt = 30;
            }

            newParam.YM2151.VisVolume1 = common.Range(Audio.visVolume.ym2151 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2151.VisVolume2 <= newParam.YM2151.VisVolume1)
            {
                newParam.YM2151.VisVolume2 = newParam.YM2151.VisVolume1;
                newParam.YM2151.VisVol2Cnt = 30;
            }

            newParam.YM2203.VisVolume1 = common.Range(Audio.visVolume.ym2203 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2203.VisVolume2 <= newParam.YM2203.VisVolume1)
            {
                newParam.YM2203.VisVolume2 = newParam.YM2203.VisVolume1;
                newParam.YM2203.VisVol2Cnt = 30;
            }

            newParam.YM2203FM.VisVolume1 = common.Range(Audio.visVolume.ym2203FM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2203FM.VisVolume2 <= newParam.YM2203FM.VisVolume1)
            {
                newParam.YM2203FM.VisVolume2 = newParam.YM2203FM.VisVolume1;
                newParam.YM2203FM.VisVol2Cnt = 30;
            }

            newParam.YM2203PSG.VisVolume1 = common.Range(Audio.visVolume.ym2203SSG / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2203PSG.VisVolume2 <= newParam.YM2203PSG.VisVolume1)
            {
                newParam.YM2203PSG.VisVolume2 = newParam.YM2203PSG.VisVolume1;
                newParam.YM2203PSG.VisVol2Cnt = 30;
            }


            newParam.YM2413.VisVolume1 = common.Range(Audio.visVolume.ym2413 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2413.VisVolume2 <= newParam.YM2413.VisVolume1)
            {
                newParam.YM2413.VisVolume2 = newParam.YM2413.VisVolume1;
                newParam.YM2413.VisVol2Cnt = 30;
            }


            newParam.YM2608.VisVolume1 = common.Range(Audio.visVolume.ym2608 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2608.VisVolume2 <= newParam.YM2608.VisVolume1)
            {
                newParam.YM2608.VisVolume2 = newParam.YM2608.VisVolume1;
                newParam.YM2608.VisVol2Cnt = 30;
            }

            newParam.YM2608FM.VisVolume1 = common.Range(Audio.visVolume.ym2608FM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2608FM.VisVolume2 <= newParam.YM2608FM.VisVolume1)
            {
                newParam.YM2608FM.VisVolume2 = newParam.YM2608FM.VisVolume1;
                newParam.YM2608FM.VisVol2Cnt = 30;
            }

            newParam.YM2608PSG.VisVolume1 = common.Range(Audio.visVolume.ym2608SSG / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2608PSG.VisVolume2 <= newParam.YM2608PSG.VisVolume1)
            {
                newParam.YM2608PSG.VisVolume2 = newParam.YM2608PSG.VisVolume1;
                newParam.YM2608PSG.VisVol2Cnt = 30;
            }

            newParam.YM2608Rhythm.VisVolume1 = common.Range(Audio.visVolume.ym2608Rtm / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2608Rhythm.VisVolume2 <= newParam.YM2608Rhythm.VisVolume1)
            {
                newParam.YM2608Rhythm.VisVolume2 = newParam.YM2608Rhythm.VisVolume1;
                newParam.YM2608Rhythm.VisVol2Cnt = 30;
            }

            newParam.YM2608Adpcm.VisVolume1 = common.Range(Audio.visVolume.ym2608APCM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2608Adpcm.VisVolume2 <= newParam.YM2608Adpcm.VisVolume1)
            {
                newParam.YM2608Adpcm.VisVolume2 = newParam.YM2608Adpcm.VisVolume1;
                newParam.YM2608Adpcm.VisVol2Cnt = 30;
            }


            newParam.YM2610.VisVolume1 = common.Range(Audio.visVolume.ym2610 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2610.VisVolume2 <= newParam.YM2610.VisVolume1)
            {
                newParam.YM2610.VisVolume2 = newParam.YM2610.VisVolume1;
                newParam.YM2610.VisVol2Cnt = 30;
            }

            newParam.YM2610FM.VisVolume1 = common.Range(Audio.visVolume.ym2610FM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2610FM.VisVolume2 <= newParam.YM2610FM.VisVolume1)
            {
                newParam.YM2610FM.VisVolume2 = newParam.YM2610FM.VisVolume1;
                newParam.YM2610FM.VisVol2Cnt = 30;
            }

            newParam.YM2610PSG.VisVolume1 = common.Range(Audio.visVolume.ym2610SSG / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2610PSG.VisVolume2 <= newParam.YM2610PSG.VisVolume1)
            {
                newParam.YM2610PSG.VisVolume2 = newParam.YM2610PSG.VisVolume1;
                newParam.YM2610PSG.VisVol2Cnt = 30;
            }

            newParam.YM2610AdpcmA.VisVolume1 = common.Range(Audio.visVolume.ym2610APCMA / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2610AdpcmA.VisVolume2 <= newParam.YM2610AdpcmA.VisVolume1)
            {
                newParam.YM2610AdpcmA.VisVolume2 = newParam.YM2610AdpcmA.VisVolume1;
                newParam.YM2610AdpcmA.VisVol2Cnt = 30;
            }

            newParam.YM2610AdpcmB.VisVolume1 = common.Range(Audio.visVolume.ym2610APCMB / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2610AdpcmB.VisVolume2 <= newParam.YM2610AdpcmB.VisVolume1)
            {
                newParam.YM2610AdpcmB.VisVolume2 = newParam.YM2610AdpcmB.VisVolume1;
                newParam.YM2610AdpcmB.VisVol2Cnt = 30;
            }

            newParam.YM2612.VisVolume1 = common.Range(Audio.visVolume.ym2612 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2612.VisVolume2 <= newParam.YM2612.VisVolume1)
            {
                newParam.YM2612.VisVolume2 = newParam.YM2612.VisVolume1;
                newParam.YM2612.VisVol2Cnt = 30;
            }

            newParam.AY8910.VisVolume1 = common.Range(Audio.visVolume.ay8910 / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.AY8910.VisVolume2 <= newParam.AY8910.VisVolume1)
            {
                newParam.AY8910.VisVolume2 = newParam.AY8910.VisVolume1;
                newParam.AY8910.VisVol2Cnt = 30;
            }

            newParam.SN76489.VisVolume1 = common.Range(Audio.visVolume.sn76489 / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.SN76489.VisVolume2 <= newParam.SN76489.VisVolume1)
            {
                newParam.SN76489.VisVolume2 = newParam.SN76489.VisVolume1;
                newParam.SN76489.VisVol2Cnt = 30;
            }

            newParam.HuC6280.VisVolume1 = common.Range(Audio.visVolume.huc6280 / 120, 0, 44);//(short.MaxValue / 44);
            if (newParam.HuC6280.VisVolume2 <= newParam.HuC6280.VisVolume1)
            {
                newParam.HuC6280.VisVolume2 = newParam.HuC6280.VisVolume1;
                newParam.HuC6280.VisVol2Cnt = 30;
            }

            newParam.RF5C164.VisVolume1 = common.Range(Audio.visVolume.rf5c164 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.RF5C164.VisVolume2 <= newParam.RF5C164.VisVolume1)
            {
                newParam.RF5C164.VisVolume2 = newParam.RF5C164.VisVolume1;
                newParam.RF5C164.VisVol2Cnt = 30;
            }

            newParam.PWM.VisVolume1 = common.Range(Audio.visVolume.pwm / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.PWM.VisVolume2 <= newParam.PWM.VisVolume1)
            {
                newParam.PWM.VisVolume2 = newParam.PWM.VisVolume1;
                newParam.PWM.VisVol2Cnt = 30;
            }

            newParam.OKIM6258.VisVolume1 = common.Range(Audio.visVolume.okim6258 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.OKIM6258.VisVolume2 <= newParam.OKIM6258.VisVolume1)
            {
                newParam.OKIM6258.VisVolume2 = newParam.OKIM6258.VisVolume1;
                newParam.OKIM6258.VisVol2Cnt = 30;
            }

            newParam.OKIM6295.VisVolume1 = common.Range(Audio.visVolume.okim6295 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.OKIM6295.VisVolume2 <= newParam.OKIM6295.VisVolume1)
            {
                newParam.OKIM6295.VisVolume2 = newParam.OKIM6295.VisVolume1;
                newParam.OKIM6295.VisVol2Cnt = 30;
            }

            newParam.C140.VisVolume1 = common.Range(Audio.visVolume.c140 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.C140.VisVolume2 <= newParam.C140.VisVolume1)
            {
                newParam.C140.VisVolume2 = newParam.C140.VisVolume1;
                newParam.C140.VisVol2Cnt = 30;
            }

            newParam.SEGAPCM.VisVolume1 = common.Range(Audio.visVolume.segaPCM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.SEGAPCM.VisVolume2 <= newParam.SEGAPCM.VisVolume1)
            {
                newParam.SEGAPCM.VisVolume2 = newParam.SEGAPCM.VisVolume1;
                newParam.SEGAPCM.VisVol2Cnt = 30;
            }

            newParam.C352.VisVolume1 = common.Range(Audio.visVolume.c352 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.C352.VisVolume2 <= newParam.C352.VisVolume1)
            {
                newParam.C352.VisVolume2 = newParam.C352.VisVolume1;
                newParam.C352.VisVol2Cnt = 30;
            }

            newParam.K054539.VisVolume1 = common.Range(Audio.visVolume.k054539 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.K054539.VisVolume2 <= newParam.K054539.VisVolume1)
            {
                newParam.K054539.VisVolume2 = newParam.K054539.VisVolume1;
                newParam.K054539.VisVol2Cnt = 30;
            }

            newParam.APU.VisVolume1 = common.Range(Audio.visVolume.APU / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.APU.VisVolume2 <= newParam.APU.VisVolume1)
            {
                newParam.APU.VisVolume2 = newParam.APU.VisVolume1;
                newParam.APU.VisVol2Cnt = 30;
            }
            newParam.DMC.VisVolume1 = common.Range(Audio.visVolume.DMC / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.DMC.VisVolume2 <= newParam.DMC.VisVolume1)
            {
                newParam.DMC.VisVolume2 = newParam.DMC.VisVolume1;
                newParam.DMC.VisVol2Cnt = 30;
            }
            newParam.FDS.VisVolume1 = common.Range(Audio.visVolume.FDS / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.FDS.VisVolume2 <= newParam.FDS.VisVolume1)
            {
                newParam.FDS.VisVolume2 = newParam.FDS.VisVolume1;
                newParam.FDS.VisVol2Cnt = 30;
            }
            newParam.MMC5.VisVolume1 = common.Range(Audio.visVolume.MMC5 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.MMC5.VisVolume2 <= newParam.K054539.VisVolume1)
            {
                newParam.MMC5.VisVolume2 = newParam.MMC5.VisVolume1;
                newParam.MMC5.VisVol2Cnt = 30;
            }
            newParam.N160.VisVolume1 = common.Range(Audio.visVolume.N160 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.N160.VisVolume2 <= newParam.N160.VisVolume1)
            {
                newParam.N160.VisVolume2 = newParam.N160.VisVolume1;
                newParam.N160.VisVol2Cnt = 30;
            }
            newParam.VRC6.VisVolume1 = common.Range(Audio.visVolume.VRC6 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.VRC6.VisVolume2 <= newParam.VRC6.VisVolume1)
            {
                newParam.VRC6.VisVolume2 = newParam.VRC6.VisVolume1;
                newParam.VRC6.VisVol2Cnt = 30;
            }
            newParam.VRC7.VisVolume1 = common.Range(Audio.visVolume.VRC7 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.VRC7.VisVolume2 <= newParam.VRC7.VisVolume1)
            {
                newParam.VRC7.VisVolume2 = newParam.VRC7.VisVolume1;
                newParam.VRC7.VisVol2Cnt = 30;
            }
            newParam.FME7.VisVolume1 = common.Range(Audio.visVolume.FME7 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.FME7.VisVolume2 <= newParam.FME7.VisVolume1)
            {
                newParam.FME7.VisVolume2 = newParam.FME7.VisVolume1;
                newParam.FME7.VisVol2Cnt = 30;
            }
        }


        public void screenDrawParams()
        {
            DrawBuff.drawFader(frameBuffer, 5, 16, 0, ref oldParam.Master.Volume, newParam.Master.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 1 * 20, 16, 1, ref oldParam.YM2151.Volume, newParam.YM2151.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 2 * 20, 16, 1, ref oldParam.YM2203.Volume, newParam.YM2203.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 3 * 20, 16, 1, ref oldParam.YM2203FM.Volume, newParam.YM2203FM.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 4 * 20, 16, 1, ref oldParam.YM2203PSG.Volume, newParam.YM2203PSG.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 5 * 20, 16, 1, ref oldParam.YM2413.Volume, newParam.YM2413.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 6 * 20, 16, 1, ref oldParam.YM2608.Volume, newParam.YM2608.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 7 * 20, 16, 1, ref oldParam.YM2608FM.Volume, newParam.YM2608FM.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 8 * 20, 16, 1, ref oldParam.YM2608PSG.Volume, newParam.YM2608PSG.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 9 * 20, 16, 1, ref oldParam.YM2608Rhythm.Volume, newParam.YM2608Rhythm.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 10 * 20, 16, 1, ref oldParam.YM2608Adpcm.Volume, newParam.YM2608Adpcm.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 11 * 20, 16, 1, ref oldParam.YM2610.Volume, newParam.YM2610.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 12 * 20, 16, 1, ref oldParam.YM2610FM.Volume, newParam.YM2610FM.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 13 * 20, 16, 1, ref oldParam.YM2610PSG.Volume, newParam.YM2610PSG.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 14 * 20, 16, 1, ref oldParam.YM2610AdpcmA.Volume, newParam.YM2610AdpcmA.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 15 * 20, 16, 1, ref oldParam.YM2610AdpcmB.Volume, newParam.YM2610AdpcmB.Volume);

            DrawBuff.drawFader(frameBuffer, 5 + 0 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.YM2612.Volume, newParam.YM2612.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 1 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.AY8910.Volume, newParam.AY8910.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 2 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.SN76489.Volume, newParam.SN76489.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 3 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.HuC6280.Volume, newParam.HuC6280.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 4 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.RF5C164.Volume, newParam.RF5C164.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 5 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.PWM.Volume, newParam.PWM.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 6 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.OKIM6258.Volume, newParam.OKIM6258.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 7 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.OKIM6295.Volume, newParam.OKIM6295.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 8 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.C140.Volume, newParam.C140.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 9 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.SEGAPCM.Volume, newParam.SEGAPCM.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 10 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.C352.Volume, newParam.C352.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 14 * 20, 16 + 1 * 8 * 9, 1, ref oldParam.K054539.Volume, newParam.K054539.Volume);

            DrawBuff.drawFader(frameBuffer, 5 + 0 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.APU.Volume, newParam.APU.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 1 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.DMC.Volume, newParam.DMC.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 2 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.FDS.Volume, newParam.FDS.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 3 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.MMC5.Volume, newParam.MMC5.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 4 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.N160.Volume, newParam.N160.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 5 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.VRC6.Volume, newParam.VRC6.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 6 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.VRC7.Volume, newParam.VRC7.Volume);
            DrawBuff.drawFader(frameBuffer, 5 + 7 * 20, 16 + 2 * 8 * 9, 1, ref oldParam.FME7.Volume, newParam.FME7.Volume);

            newParam.Master.VisVol2Cnt--;
            if (newParam.Master.VisVol2Cnt == 0)
            {
                newParam.Master.VisVol2Cnt = 1;
                if (newParam.Master.VisVolume2 > 0) newParam.Master.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 0 * 20, 10 + 0 * 8 * 9, ref oldParam.Master.VisVolume1, newParam.Master.VisVolume1, ref oldParam.Master.VisVolume2, newParam.Master.VisVolume2);

            newParam.YM2151.VisVol2Cnt--;
            if (newParam.YM2151.VisVol2Cnt == 0)
            {
                newParam.YM2151.VisVol2Cnt = 1;
                if (newParam.YM2151.VisVolume2 > 0) newParam.YM2151.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 1 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2151.VisVolume1, newParam.YM2151.VisVolume1, ref oldParam.YM2151.VisVolume2, newParam.YM2151.VisVolume2);

            newParam.YM2203.VisVol2Cnt--;
            if (newParam.YM2203.VisVol2Cnt == 0)
            {
                newParam.YM2203.VisVol2Cnt = 1;
                if (newParam.YM2203.VisVolume2 > 0) newParam.YM2203.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 2 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2203.VisVolume1, newParam.YM2203.VisVolume1, ref oldParam.YM2203.VisVolume2, newParam.YM2203.VisVolume2);

            newParam.YM2203FM.VisVol2Cnt--;
            if (newParam.YM2203FM.VisVol2Cnt == 0)
            {
                newParam.YM2203FM.VisVol2Cnt = 1;
                if (newParam.YM2203FM.VisVolume2 > 0) newParam.YM2203FM.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 3 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2203FM.VisVolume1, newParam.YM2203FM.VisVolume1, ref oldParam.YM2203FM.VisVolume2, newParam.YM2203FM.VisVolume2);

            newParam.YM2203PSG.VisVol2Cnt--;
            if (newParam.YM2203PSG.VisVol2Cnt == 0)
            {
                newParam.YM2203PSG.VisVol2Cnt = 1;
                if (newParam.YM2203PSG.VisVolume2 > 0) newParam.YM2203PSG.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 4 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2203PSG.VisVolume1, newParam.YM2203PSG.VisVolume1, ref oldParam.YM2203PSG.VisVolume2, newParam.YM2203PSG.VisVolume2);

            newParam.YM2413.VisVol2Cnt--;
            if (newParam.YM2413.VisVol2Cnt == 0)
            {
                newParam.YM2413.VisVol2Cnt = 1;
                if (newParam.YM2413.VisVolume2 > 0) newParam.YM2413.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 5 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2413.VisVolume1, newParam.YM2413.VisVolume1, ref oldParam.YM2413.VisVolume2, newParam.YM2413.VisVolume2);

            newParam.YM2608.VisVol2Cnt--;
            if (newParam.YM2608.VisVol2Cnt == 0)
            {
                newParam.YM2608.VisVol2Cnt = 1;
                if (newParam.YM2608.VisVolume2 > 0) newParam.YM2608.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 6 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2608.VisVolume1, newParam.YM2608.VisVolume1, ref oldParam.YM2608.VisVolume2, newParam.YM2608.VisVolume2);

            newParam.YM2608FM.VisVol2Cnt--;
            if (newParam.YM2608FM.VisVol2Cnt == 0)
            {
                newParam.YM2608FM.VisVol2Cnt = 1;
                if (newParam.YM2608FM.VisVolume2 > 0) newParam.YM2608FM.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 7 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2608FM.VisVolume1, newParam.YM2608FM.VisVolume1, ref oldParam.YM2608FM.VisVolume2, newParam.YM2608FM.VisVolume2);

            newParam.YM2608PSG.VisVol2Cnt--;
            if (newParam.YM2608PSG.VisVol2Cnt == 0)
            {
                newParam.YM2608PSG.VisVol2Cnt = 1;
                if (newParam.YM2608PSG.VisVolume2 > 0) newParam.YM2608PSG.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 8 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2608PSG.VisVolume1, newParam.YM2608PSG.VisVolume1, ref oldParam.YM2608PSG.VisVolume2, newParam.YM2608PSG.VisVolume2);

            newParam.YM2608Rhythm.VisVol2Cnt--;
            if (newParam.YM2608Rhythm.VisVol2Cnt == 0)
            {
                newParam.YM2608Rhythm.VisVol2Cnt = 1;
                if (newParam.YM2608Rhythm.VisVolume2 > 0) newParam.YM2608Rhythm.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 9 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2608Rhythm.VisVolume1, newParam.YM2608Rhythm.VisVolume1, ref oldParam.YM2608Rhythm.VisVolume2, newParam.YM2608Rhythm.VisVolume2);

            newParam.YM2608Adpcm.VisVol2Cnt--;
            if (newParam.YM2608Adpcm.VisVol2Cnt == 0)
            {
                newParam.YM2608Adpcm.VisVol2Cnt = 1;
                if (newParam.YM2608Adpcm.VisVolume2 > 0) newParam.YM2608Adpcm.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 10 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2608Adpcm.VisVolume1, newParam.YM2608Adpcm.VisVolume1, ref oldParam.YM2608Adpcm.VisVolume2, newParam.YM2608Adpcm.VisVolume2);

            newParam.YM2610.VisVol2Cnt--;
            if (newParam.YM2610.VisVol2Cnt == 0)
            {
                newParam.YM2610.VisVol2Cnt = 1;
                if (newParam.YM2610.VisVolume2 > 0) newParam.YM2610.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 11 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2610.VisVolume1, newParam.YM2610.VisVolume1, ref oldParam.YM2610.VisVolume2, newParam.YM2610.VisVolume2);

            newParam.YM2610FM.VisVol2Cnt--;
            if (newParam.YM2610FM.VisVol2Cnt == 0)
            {
                newParam.YM2610FM.VisVol2Cnt = 1;
                if (newParam.YM2610FM.VisVolume2 > 0) newParam.YM2610FM.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 12 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2610FM.VisVolume1, newParam.YM2610FM.VisVolume1, ref oldParam.YM2610FM.VisVolume2, newParam.YM2610FM.VisVolume2);

            newParam.YM2610PSG.VisVol2Cnt--;
            if (newParam.YM2610PSG.VisVol2Cnt == 0)
            {
                newParam.YM2610PSG.VisVol2Cnt = 1;
                if (newParam.YM2610PSG.VisVolume2 > 0) newParam.YM2610PSG.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 13 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2610PSG.VisVolume1, newParam.YM2610PSG.VisVolume1, ref oldParam.YM2610PSG.VisVolume2, newParam.YM2610PSG.VisVolume2);

            newParam.YM2610AdpcmA.VisVol2Cnt--;
            if (newParam.YM2610AdpcmA.VisVol2Cnt == 0)
            {
                newParam.YM2610AdpcmA.VisVol2Cnt = 1;
                if (newParam.YM2610AdpcmA.VisVolume2 > 0) newParam.YM2610AdpcmA.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 14 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2610AdpcmA.VisVolume1, newParam.YM2610AdpcmA.VisVolume1, ref oldParam.YM2610AdpcmA.VisVolume2, newParam.YM2610AdpcmA.VisVolume2);

            newParam.YM2610AdpcmB.VisVol2Cnt--;
            if (newParam.YM2610AdpcmB.VisVol2Cnt == 0)
            {
                newParam.YM2610AdpcmB.VisVol2Cnt = 1;
                if (newParam.YM2610AdpcmB.VisVolume2 > 0) newParam.YM2610AdpcmB.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 15 * 20, 10 + 0 * 8 * 9, ref oldParam.YM2610AdpcmB.VisVolume1, newParam.YM2610AdpcmB.VisVolume1, ref oldParam.YM2610AdpcmB.VisVolume2, newParam.YM2610AdpcmB.VisVolume2);

            newParam.YM2612.VisVol2Cnt--;
            if (newParam.YM2612.VisVol2Cnt == 0)
            {
                newParam.YM2612.VisVol2Cnt = 1;
                if (newParam.YM2612.VisVolume2 > 0) newParam.YM2612.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 0 * 20, 10 + 1 * 8 * 9, ref oldParam.YM2612.VisVolume1, newParam.YM2612.VisVolume1, ref oldParam.YM2612.VisVolume2, newParam.YM2612.VisVolume2);

            newParam.AY8910.VisVol2Cnt--;
            if (newParam.AY8910.VisVol2Cnt == 0)
            {
                newParam.AY8910.VisVol2Cnt = 1;
                if (newParam.AY8910.VisVolume2 > 0) newParam.AY8910.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 1 * 20, 10 + 1 * 8 * 9, ref oldParam.AY8910.VisVolume1, newParam.AY8910.VisVolume1, ref oldParam.AY8910.VisVolume2, newParam.AY8910.VisVolume2);

            newParam.SN76489.VisVol2Cnt--;
            if (newParam.SN76489.VisVol2Cnt == 0)
            {
                newParam.SN76489.VisVol2Cnt = 1;
                if (newParam.SN76489.VisVolume2 > 0) newParam.SN76489.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 2 * 20, 10 + 1 * 8 * 9, ref oldParam.SN76489.VisVolume1, newParam.SN76489.VisVolume1, ref oldParam.SN76489.VisVolume2, newParam.SN76489.VisVolume2);

            newParam.HuC6280.VisVol2Cnt--;
            if (newParam.HuC6280.VisVol2Cnt == 0)
            {
                newParam.HuC6280.VisVol2Cnt = 1;
                if (newParam.HuC6280.VisVolume2 > 0) newParam.HuC6280.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 3 * 20, 10 + 1 * 8 * 9, ref oldParam.HuC6280.VisVolume1, newParam.HuC6280.VisVolume1, ref oldParam.HuC6280.VisVolume2, newParam.HuC6280.VisVolume2);

            newParam.RF5C164.VisVol2Cnt--;
            if (newParam.RF5C164.VisVol2Cnt == 0)
            {
                newParam.RF5C164.VisVol2Cnt = 1;
                if (newParam.RF5C164.VisVolume2 > 0) newParam.RF5C164.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 4 * 20, 10 + 1 * 8 * 9, ref oldParam.RF5C164.VisVolume1, newParam.RF5C164.VisVolume1, ref oldParam.RF5C164.VisVolume2, newParam.RF5C164.VisVolume2);

            newParam.PWM.VisVol2Cnt--;
            if (newParam.PWM.VisVol2Cnt == 0)
            {
                newParam.PWM.VisVol2Cnt = 1;
                if (newParam.PWM.VisVolume2 > 0) newParam.PWM.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 5 * 20, 10 + 1 * 8 * 9, ref oldParam.PWM.VisVolume1, newParam.PWM.VisVolume1, ref oldParam.PWM.VisVolume2, newParam.PWM.VisVolume2);

            newParam.OKIM6258.VisVol2Cnt--;
            if (newParam.OKIM6258.VisVol2Cnt == 0)
            {
                newParam.OKIM6258.VisVol2Cnt = 1;
                if (newParam.OKIM6258.VisVolume2 > 0) newParam.OKIM6258.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 6 * 20, 10 + 1 * 8 * 9, ref oldParam.OKIM6258.VisVolume1, newParam.OKIM6258.VisVolume1, ref oldParam.OKIM6258.VisVolume2, newParam.OKIM6258.VisVolume2);

            newParam.OKIM6295.VisVol2Cnt--;
            if (newParam.OKIM6295.VisVol2Cnt == 0)
            {
                newParam.OKIM6295.VisVol2Cnt = 1;
                if (newParam.OKIM6295.VisVolume2 > 0) newParam.OKIM6295.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 7 * 20, 10 + 1 * 8 * 9, ref oldParam.OKIM6295.VisVolume1, newParam.OKIM6295.VisVolume1, ref oldParam.OKIM6295.VisVolume2, newParam.OKIM6295.VisVolume2);

            newParam.C140.VisVol2Cnt--;
            if (newParam.C140.VisVol2Cnt == 0)
            {
                newParam.C140.VisVol2Cnt = 1;
                if (newParam.C140.VisVolume2 > 0) newParam.C140.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 8 * 20, 10 + 1 * 8 * 9, ref oldParam.C140.VisVolume1, newParam.C140.VisVolume1, ref oldParam.C140.VisVolume2, newParam.C140.VisVolume2);

            newParam.SEGAPCM.VisVol2Cnt--;
            if (newParam.SEGAPCM.VisVol2Cnt == 0)
            {
                newParam.SEGAPCM.VisVol2Cnt = 1;
                if (newParam.SEGAPCM.VisVolume2 > 0) newParam.SEGAPCM.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 9 * 20, 10 + 1 * 8 * 9, ref oldParam.SEGAPCM.VisVolume1, newParam.SEGAPCM.VisVolume1, ref oldParam.SEGAPCM.VisVolume2, newParam.SEGAPCM.VisVolume2);

            newParam.C352.VisVol2Cnt--;
            if (newParam.C352.VisVol2Cnt == 0)
            {
                newParam.C352.VisVol2Cnt = 1;
                if (newParam.C352.VisVolume2 > 0) newParam.C352.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 10 * 20, 10 + 1 * 8 * 9, ref oldParam.C352.VisVolume1, newParam.C352.VisVolume1, ref oldParam.C352.VisVolume2, newParam.C352.VisVolume2);

            newParam.K054539.VisVol2Cnt--;
            if (newParam.K054539.VisVol2Cnt == 0)
            {
                newParam.K054539.VisVol2Cnt = 1;
                if (newParam.K054539.VisVolume2 > 0) newParam.K054539.VisVolume2--;
            }
            DrawBuff.drawMixerVolume(frameBuffer, 2 + 14 * 20, 10 + 1 * 8 * 9, ref oldParam.K054539.VisVolume1, newParam.K054539.VisVolume1, ref oldParam.K054539.VisVolume2, newParam.K054539.VisVolume2);
        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;
            chipn = px / 20 + (py / 72) * 16;
            bool b = e.Button == MouseButtons.Middle;

            fader(chipn, b, 0, 0);
        }

        private void frmMixer2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void frmMixer2_MouseDown(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;

            chipn = px / 20 + (py / 72) * 16;

        }

        private void frmMixer2_MouseMove(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;
            py = py % 72;
            int n = 0;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (py < 18)
                {
                    n = (18 - py) > 8 ? 8 : (18 - py);
                    n = (int)(n * 2.5);
                }
                else if (py == 18)
                {
                    n = 0;
                }
                else
                {
                    n = (18 - py) < -35 ? -35 : (18 - py);
                    n = (int)(n * (192.0 / 35.0));
                }

                fader(chipn, true, 0, n);

            }
        }


        private void fader(int chipn, bool b, int delta, int v)
        {
            switch (chipn)
            {
                case 0:
                    Audio.SetMasterVolume(parent.setting.balance.MasterVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.MasterVolume + delta)), 20), -192)));
                    break;
                case 1:
                    Audio.SetYM2151Volume(parent.setting.balance.YM2151Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2151Volume + delta)), 20), -192)));
                    break;
                case 2:
                    Audio.SetYM2203Volume(parent.setting.balance.YM2203Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2203Volume + delta)), 20), -192)));
                    break;
                case 3:
                    Audio.SetYM2203FMVolume(parent.setting.balance.YM2203FMVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2203FMVolume + delta)), 20), -192)));
                    break;
                case 4:
                    Audio.SetYM2203PSGVolume(parent.setting.balance.YM2203PSGVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2203PSGVolume + delta)), 20), -192)));
                    break;
                case 5:
                    Audio.SetYM2413Volume(parent.setting.balance.YM2413Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2413Volume + delta)), 20), -192)));
                    break;
                case 6:
                    Audio.SetYM2608Volume(parent.setting.balance.YM2608Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2608Volume + delta)), 20), -192)));
                    break;
                case 7:
                    Audio.SetYM2608FMVolume(parent.setting.balance.YM2608FMVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2608FMVolume + delta)), 20), -192)));
                    break;
                case 8:
                    Audio.SetYM2608PSGVolume(parent.setting.balance.YM2608PSGVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2608PSGVolume + delta)), 20), -192)));
                    break;
                case 9:
                    Audio.SetYM2608RhythmVolume(parent.setting.balance.YM2608RhythmVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2608RhythmVolume + delta)), 20), -192)));
                    break;
                case 10:
                    Audio.SetYM2608AdpcmVolume(parent.setting.balance.YM2608AdpcmVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2608AdpcmVolume + delta)), 20), -192)));
                    break;
                case 11:
                    Audio.SetYM2610Volume(parent.setting.balance.YM2610Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2610Volume + delta)), 20), -192)));
                    break;
                case 12:
                    Audio.SetYM2610FMVolume(parent.setting.balance.YM2610FMVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2610FMVolume + delta)), 20), -192)));
                    break;
                case 13:
                    Audio.SetYM2610PSGVolume(parent.setting.balance.YM2610PSGVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2610PSGVolume + delta)), 20), -192)));
                    break;
                case 14:
                    Audio.SetYM2610AdpcmAVolume(parent.setting.balance.YM2610AdpcmAVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2610AdpcmAVolume + delta)), 20), -192)));
                    break;
                case 15:
                    Audio.SetYM2610AdpcmBVolume(parent.setting.balance.YM2610AdpcmBVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2610AdpcmBVolume + delta)), 20), -192)));
                    break;

                case 16:
                    Audio.SetYM2612Volume(parent.setting.balance.YM2612Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.YM2612Volume + delta)), 20), -192)));
                    break;
                case 17:
                    Audio.SetAY8910Volume(parent.setting.balance.AY8910Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.AY8910Volume + delta)), 20), -192)));
                    break;
                case 18:
                    Audio.SetSN76489Volume(parent.setting.balance.SN76489Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.SN76489Volume + delta)), 20), -192)));
                    break;
                case 19:
                    Audio.SetHuC6280Volume(parent.setting.balance.HuC6280Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.HuC6280Volume + delta)), 20), -192)));
                    break;
                case 20:
                    Audio.SetRF5C164Volume(parent.setting.balance.RF5C164Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.RF5C164Volume + delta)), 20), -192)));
                    break;
                case 21:
                    Audio.SetPWMVolume(parent.setting.balance.PWMVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.PWMVolume + delta)), 20), -192)));
                    break;
                case 22:
                    Audio.SetOKIM6258Volume(parent.setting.balance.OKIM6258Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.OKIM6258Volume + delta)), 20), -192)));
                    break;
                case 23:
                    Audio.SetOKIM6295Volume(parent.setting.balance.OKIM6295Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.OKIM6295Volume + delta)), 20), -192)));
                    break;
                case 24:
                    Audio.SetC140Volume(parent.setting.balance.C140Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.C140Volume + delta)), 20), -192)));
                    break;
                case 25:
                    Audio.SetSegaPCMVolume(parent.setting.balance.SEGAPCMVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.SEGAPCMVolume + delta)), 20), -192)));
                    break;
                case 26:
                    Audio.SetC352Volume(parent.setting.balance.C352Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.C352Volume + delta)), 20), -192)));
                    break;
                case 27://K051
                    break;
                case 28://K052
                    break;
                case 29://K053
                    break;
                case 30://K054
                    Audio.SetK054539Volume(parent.setting.balance.K054539Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.K054539Volume + delta)), 20), -192)));
                    break;
                case 31://QSND
                    break;

                case 32://NES(APU)
                    Audio.SetAPUVolume(parent.setting.balance.APUVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.APUVolume + delta)), 20), -192)));
                    break;
                case 33://DMC
                    Audio.SetDMCVolume(parent.setting.balance.DMCVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.DMCVolume + delta)), 20), -192)));
                    break;
                case 34://FDS
                    Audio.SetFDSVolume(parent.setting.balance.FDSVolume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.FDSVolume + delta)), 20), -192)));
                    break;
                case 35://MMC5
                    Audio.SetMMC5Volume(parent.setting.balance.MMC5Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.MMC5Volume + delta)), 20), -192)));
                    break;
                case 36://N160
                    Audio.SetN160Volume(parent.setting.balance.N160Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.N160Volume + delta)), 20), -192)));
                    break;
                case 37://VRC6
                    Audio.SetVRC6Volume(parent.setting.balance.VRC6Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.VRC6Volume + delta)), 20), -192)));
                    break;
                case 38://VRC7
                    Audio.SetVRC7Volume(parent.setting.balance.VRC7Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.VRC7Volume + delta)), 20), -192)));
                    break;
                case 39://FME7
                    Audio.SetFME7Volume(parent.setting.balance.FME7Volume = (Math.Max(Math.Min((b ? v : (parent.setting.balance.FME7Volume + delta)), 20), -192)));
                    break;
                case 40://DMG
                    break;

            }
        }


    }
}
