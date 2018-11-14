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

            if (chipn < 0 || chipn >= SetVolume.Length) return;
            SetVolume[chipn]?.Invoke(false, delta);
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

            newParam.Master.Volume = parent.setting.balance.MasterVolume;
            newParam.YM2151.Volume = parent.setting.balance.YM2151Volume;
            newParam.YM2203.Volume = parent.setting.balance.YM2203Volume;
            newParam.YM2203FM.Volume = parent.setting.balance.YM2203FMVolume;
            newParam.YM2203PSG.Volume = parent.setting.balance.YM2203PSGVolume;
            newParam.YM2612.Volume = parent.setting.balance.YM2612Volume;
            newParam.YM2608.Volume = parent.setting.balance.YM2608Volume;
            newParam.YM2608FM.Volume = parent.setting.balance.YM2608FMVolume;
            newParam.YM2608PSG.Volume = parent.setting.balance.YM2608PSGVolume;
            newParam.YM2608Rhythm.Volume = parent.setting.balance.YM2608RhythmVolume;
            newParam.YM2608Adpcm.Volume = parent.setting.balance.YM2608AdpcmVolume;
            newParam.YM2610.Volume = parent.setting.balance.YM2610Volume;
            newParam.YM2610FM.Volume = parent.setting.balance.YM2610FMVolume;
            newParam.YM2610PSG.Volume = parent.setting.balance.YM2610PSGVolume;
            newParam.YM2610AdpcmA.Volume = parent.setting.balance.YM2610AdpcmAVolume;
            newParam.YM2610AdpcmB.Volume = parent.setting.balance.YM2610AdpcmBVolume;

            newParam.YM2413.Volume = parent.setting.balance.YM2413Volume;
            newParam.YM3526.Volume = parent.setting.balance.YM3526Volume;
            newParam.Y8950.Volume = parent.setting.balance.Y8950Volume;
            newParam.YM3812.Volume = parent.setting.balance.YM3812Volume;
            newParam.YMF262.Volume = parent.setting.balance.YMF262Volume;
            newParam.YMF278B.Volume = parent.setting.balance.YMF278BVolume;
            newParam.YMZ280B.Volume = parent.setting.balance.YMZ280BVolume;
            newParam.YMF271.Volume = parent.setting.balance.YMF271Volume;
            newParam.AY8910.Volume = parent.setting.balance.AY8910Volume;
            newParam.SN76489.Volume = parent.setting.balance.SN76489Volume;
            newParam.HuC6280.Volume = parent.setting.balance.HuC6280Volume;

            newParam.RF5C164.Volume = parent.setting.balance.RF5C164Volume;
            newParam.RF5C68.Volume = parent.setting.balance.RF5C68Volume;
            newParam.PWM.Volume = parent.setting.balance.PWMVolume;
            newParam.OKIM6258.Volume = parent.setting.balance.OKIM6258Volume;
            newParam.OKIM6295.Volume = parent.setting.balance.OKIM6295Volume;
            newParam.C140.Volume = parent.setting.balance.C140Volume;
            newParam.C352.Volume = parent.setting.balance.C352Volume;
            newParam.SEGAPCM.Volume = parent.setting.balance.SEGAPCMVolume;
            newParam.MultiPCM.Volume = parent.setting.balance.MultiPCMVolume;
            newParam.K051649.Volume = parent.setting.balance.K051649Volume;
            newParam.K053260.Volume = parent.setting.balance.K053260Volume;
            newParam.K054539.Volume = parent.setting.balance.K054539Volume;
            newParam.QSound.Volume = parent.setting.balance.QSoundVolume;
            newParam.GA20.Volume = parent.setting.balance.GA20Volume;

            newParam.APU.Volume = parent.setting.balance.APUVolume;
            newParam.DMC.Volume = parent.setting.balance.DMCVolume;
            newParam.FDS.Volume = parent.setting.balance.FDSVolume;
            newParam.MMC5.Volume = parent.setting.balance.MMC5Volume;
            newParam.N160.Volume = parent.setting.balance.N160Volume;
            newParam.VRC6.Volume = parent.setting.balance.VRC6Volume;
            newParam.VRC7.Volume = parent.setting.balance.VRC7Volume;
            newParam.FME7.Volume = parent.setting.balance.FME7Volume;
            newParam.DMG.Volume = parent.setting.balance.DMGVolume;


            newParam.Master.VisVolume1 = common.Range(Audio.visVolume.master / 250, 0, 44);
            if (newParam.Master.VisVolume2 <= newParam.Master.VisVolume1)
            {
                newParam.Master.VisVolume2 = newParam.Master.VisVolume1;
                newParam.Master.VisVol2Cnt = 30;
            }

            newParam.YM2151.VisVolume1 = common.Range(Audio.visVolume.ym2151 / 200, 0, 44);
            if (newParam.YM2151.VisVolume2 <= newParam.YM2151.VisVolume1)
            {
                newParam.YM2151.VisVolume2 = newParam.YM2151.VisVolume1;
                newParam.YM2151.VisVol2Cnt = 30;
            }

            newParam.YM2203.VisVolume1 = common.Range(Audio.visVolume.ym2203 / 200, 0, 44);
            if (newParam.YM2203.VisVolume2 <= newParam.YM2203.VisVolume1)
            {
                newParam.YM2203.VisVolume2 = newParam.YM2203.VisVolume1;
                newParam.YM2203.VisVol2Cnt = 30;
            }

            newParam.YM2203FM.VisVolume1 = common.Range(Audio.visVolume.ym2203FM / 200, 0, 44);
            if (newParam.YM2203FM.VisVolume2 <= newParam.YM2203FM.VisVolume1)
            {
                newParam.YM2203FM.VisVolume2 = newParam.YM2203FM.VisVolume1;
                newParam.YM2203FM.VisVol2Cnt = 30;
            }

            newParam.YM2203PSG.VisVolume1 = common.Range(Audio.visVolume.ym2203SSG / 120, 0, 44);
            if (newParam.YM2203PSG.VisVolume2 <= newParam.YM2203PSG.VisVolume1)
            {
                newParam.YM2203PSG.VisVolume2 = newParam.YM2203PSG.VisVolume1;
                newParam.YM2203PSG.VisVol2Cnt = 30;
            }

            newParam.YM2612.VisVolume1 = common.Range(Audio.visVolume.ym2612 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.YM2612.VisVolume2 <= newParam.YM2612.VisVolume1)
            {
                newParam.YM2612.VisVolume2 = newParam.YM2612.VisVolume1;
                newParam.YM2612.VisVol2Cnt = 30;
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




            newParam.YM2413.VisVolume1 = common.Range(Audio.visVolume.ym2413 / 200, 0, 44);
            if (newParam.YM2413.VisVolume2 <= newParam.YM2413.VisVolume1)
            {
                newParam.YM2413.VisVolume2 = newParam.YM2413.VisVolume1;
                newParam.YM2413.VisVol2Cnt = 30;
            }

            newParam.YM3526.VisVolume1 = common.Range(Audio.visVolume.ym3526 / 200, 0, 44);
            if (newParam.YM3526.VisVolume2 <= newParam.YM3526.VisVolume1)
            {
                newParam.YM3526.VisVolume2 = newParam.YM3526.VisVolume1;
                newParam.YM3526.VisVol2Cnt = 30;
            }

            newParam.Y8950.VisVolume1 = common.Range(Audio.visVolume.y8950 / 200, 0, 44);
            if (newParam.Y8950.VisVolume2 <= newParam.Y8950.VisVolume1)
            {
                newParam.Y8950.VisVolume2 = newParam.Y8950.VisVolume1;
                newParam.Y8950.VisVol2Cnt = 30;
            }

            newParam.YM3812.VisVolume1 = common.Range(Audio.visVolume.ym3812 / 200, 0, 44);
            if (newParam.YM3812.VisVolume2 <= newParam.YM3812.VisVolume1)
            {
                newParam.YM3812.VisVolume2 = newParam.YM3812.VisVolume1;
                newParam.YM3812.VisVol2Cnt = 30;
            }

            newParam.YMF262.VisVolume1 = common.Range(Audio.visVolume.ymf262 / 200, 0, 44);
            if (newParam.YMF262.VisVolume2 <= newParam.YMF262.VisVolume1)
            {
                newParam.YMF262.VisVolume2 = newParam.YMF262.VisVolume1;
                newParam.YMF262.VisVol2Cnt = 30;
            }

            newParam.YMF278B.VisVolume1 = common.Range(Audio.visVolume.ymf278b / 200, 0, 44);
            if (newParam.YMF278B.VisVolume2 <= newParam.YMF278B.VisVolume1)
            {
                newParam.YMF278B.VisVolume2 = newParam.YMF278B.VisVolume1;
                newParam.YMF278B.VisVol2Cnt = 30;
            }

            newParam.YMZ280B.VisVolume1 = common.Range(Audio.visVolume.ymz280b / 200, 0, 44);
            if (newParam.YMZ280B.VisVolume2 <= newParam.YMZ280B.VisVolume1)
            {
                newParam.YMZ280B.VisVolume2 = newParam.YMZ280B.VisVolume1;
                newParam.YMZ280B.VisVol2Cnt = 30;
            }

            newParam.YMF271.VisVolume1 = common.Range(Audio.visVolume.ymf271 / 200, 0, 44);
            if (newParam.YMF271.VisVolume2 <= newParam.YMF271.VisVolume1)
            {
                newParam.YMF271.VisVolume2 = newParam.YMF271.VisVolume1;
                newParam.YMF271.VisVol2Cnt = 30;
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

            newParam.RF5C68.VisVolume1 = common.Range(Audio.visVolume.rf5c68 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.RF5C68.VisVolume2 <= newParam.RF5C68.VisVolume1)
            {
                newParam.RF5C68.VisVolume2 = newParam.RF5C68.VisVolume1;
                newParam.RF5C68.VisVol2Cnt = 30;
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

            newParam.C352.VisVolume1 = common.Range(Audio.visVolume.c352 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.C352.VisVolume2 <= newParam.C352.VisVolume1)
            {
                newParam.C352.VisVolume2 = newParam.C352.VisVolume1;
                newParam.C352.VisVol2Cnt = 30;
            }

            newParam.SEGAPCM.VisVolume1 = common.Range(Audio.visVolume.segaPCM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.SEGAPCM.VisVolume2 <= newParam.SEGAPCM.VisVolume1)
            {
                newParam.SEGAPCM.VisVolume2 = newParam.SEGAPCM.VisVolume1;
                newParam.SEGAPCM.VisVol2Cnt = 30;
            }

            newParam.MultiPCM.VisVolume1 = common.Range(Audio.visVolume.multiPCM / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.MultiPCM.VisVolume2 <= newParam.MultiPCM.VisVolume1)
            {
                newParam.MultiPCM.VisVolume2 = newParam.MultiPCM.VisVolume1;
                newParam.MultiPCM.VisVol2Cnt = 30;
            }

            newParam.K051649.VisVolume1 = common.Range(Audio.visVolume.k051649 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.K051649.VisVolume2 <= newParam.K051649.VisVolume1)
            {
                newParam.K051649.VisVolume2 = newParam.K051649.VisVolume1;
                newParam.K051649.VisVol2Cnt = 30;
            }

            newParam.K053260.VisVolume1 = common.Range(Audio.visVolume.k053260 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.K053260.VisVolume2 <= newParam.K053260.VisVolume1)
            {
                newParam.K053260.VisVolume2 = newParam.K053260.VisVolume1;
                newParam.K053260.VisVol2Cnt = 30;
            }

            newParam.K054539.VisVolume1 = common.Range(Audio.visVolume.k054539 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.K054539.VisVolume2 <= newParam.K054539.VisVolume1)
            {
                newParam.K054539.VisVolume2 = newParam.K054539.VisVolume1;
                newParam.K054539.VisVol2Cnt = 30;
            }

            newParam.QSound.VisVolume1 = common.Range(Audio.visVolume.qSound / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.QSound.VisVolume2 <= newParam.QSound.VisVolume1)
            {
                newParam.QSound.VisVolume2 = newParam.QSound.VisVolume1;
                newParam.QSound.VisVol2Cnt = 30;
            }

            newParam.GA20.VisVolume1 = common.Range(Audio.visVolume.ga20 / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.GA20.VisVolume2 <= newParam.GA20.VisVolume1)
            {
                newParam.GA20.VisVolume2 = newParam.GA20.VisVolume1;
                newParam.GA20.VisVol2Cnt = 30;
            }

            newParam.APU.VisVolume1 = common.Range(Audio.visVolume.APU / 200, 0, 44);//(short.MaxValue / 44);
            if (newParam.APU.VisVolume2 <= newParam.APU.VisVolume1)
            {
                newParam.APU.VisVolume2 = newParam.APU.VisVolume1;
                newParam.APU.VisVol2Cnt = 30;
            }

            newParam.DMC.VisVolume1 = common.Range(Audio.visVolume.DMC / 350, 0, 44);//(short.MaxValue / 44);
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

            newParam.MMC5.VisVolume1 = common.Range(Audio.visVolume.MMC5 / 50, 0, 44);//(short.MaxValue / 44);
            if (newParam.MMC5.VisVolume2 <= newParam.K054539.VisVolume1)
            {
                newParam.MMC5.VisVolume2 = newParam.MMC5.VisVolume1;
                newParam.MMC5.VisVol2Cnt = 30;
            }

            newParam.N160.VisVolume1 = common.Range(Audio.visVolume.N160 / 50, 0, 44);//(short.MaxValue / 44);
            if (newParam.N160.VisVolume2 <= newParam.N160.VisVolume1)
            {
                newParam.N160.VisVolume2 = newParam.N160.VisVolume1;
                newParam.N160.VisVol2Cnt = 30;
            }
            newParam.VRC6.VisVolume1 = common.Range(Audio.visVolume.VRC6 / 50, 0, 44);//(short.MaxValue / 44);
            if (newParam.VRC6.VisVolume2 <= newParam.VRC6.VisVolume1)
            {
                newParam.VRC6.VisVolume2 = newParam.VRC6.VisVolume1;
                newParam.VRC6.VisVol2Cnt = 30;
            }

            newParam.VRC7.VisVolume1 = common.Range(Audio.visVolume.VRC7 / 50, 0, 44);//(short.MaxValue / 44);
            if (newParam.VRC7.VisVolume2 <= newParam.VRC7.VisVolume1)
            {
                newParam.VRC7.VisVolume2 = newParam.VRC7.VisVolume1;
                newParam.VRC7.VisVol2Cnt = 30;
            }

            newParam.FME7.VisVolume1 = common.Range(Audio.visVolume.FME7 / 50, 0, 44);//(short.MaxValue / 44);
            if (newParam.FME7.VisVolume2 <= newParam.FME7.VisVolume1)
            {
                newParam.FME7.VisVolume2 = newParam.FME7.VisVolume1;
                newParam.FME7.VisVol2Cnt = 30;
            }

            newParam.DMG.VisVolume1 = common.Range(Audio.visVolume.DMG / 50, 0, 44);//(short.MaxValue / 44);
            if (newParam.DMG.VisVolume2 <= newParam.DMG.VisVolume1)
            {
                newParam.DMG.VisVolume2 = newParam.DMG.VisVolume1;
                newParam.DMG.VisVol2Cnt = 30;
            }
        }

        public void screenDrawParams()
        {
            int num;
            MDChipParams.Mixer.VolumeInfo oVI, nVI;

            num = 0; oVI = oldParam.Master; nVI = newParam.Master; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2151; nVI = newParam.YM2151; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2203; nVI = newParam.YM2203; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2203FM; nVI = newParam.YM2203FM; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2203PSG; nVI = newParam.YM2203PSG; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2612; nVI = newParam.YM2612; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2608; nVI = newParam.YM2608; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2608FM; nVI = newParam.YM2608FM; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2608PSG; nVI = newParam.YM2608PSG; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2608Rhythm; nVI = newParam.YM2608Rhythm; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2608Adpcm; nVI = newParam.YM2608Adpcm; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2610; nVI = newParam.YM2610; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2610FM; nVI = newParam.YM2610FM; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2610PSG; nVI = newParam.YM2610PSG; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2610AdpcmA; nVI = newParam.YM2610AdpcmA; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM2610AdpcmB; nVI = newParam.YM2610AdpcmB; drawVolAndFader(num, oVI, nVI);

            num++; oVI = oldParam.YM2413; nVI = newParam.YM2413; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM3526; nVI = newParam.YM3526; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.Y8950; nVI = newParam.Y8950; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YM3812; nVI = newParam.YM3812; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YMF262; nVI = newParam.YMF262; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YMF278B; nVI = newParam.YMF278B; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YMZ280B; nVI = newParam.YMZ280B; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.YMF271; nVI = newParam.YMF271; drawVolAndFader(num, oVI, nVI);
            num++; 
            num++; oVI = oldParam.AY8910; nVI = newParam.AY8910; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.SN76489; nVI = newParam.SN76489; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.HuC6280; nVI = newParam.HuC6280; drawVolAndFader(num, oVI, nVI);
            num++; 
            num++; 
            num++; 
            num++;

            num++; 
            num++; 
            num++; oVI = oldParam.RF5C164; nVI = newParam.RF5C164; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.RF5C68; nVI = newParam.RF5C68; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.PWM; nVI = newParam.PWM; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.OKIM6258; nVI = newParam.OKIM6258; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.OKIM6295; nVI = newParam.OKIM6295; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.C140; nVI = newParam.C140; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.C352; nVI = newParam.C352; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.SEGAPCM; nVI = newParam.SEGAPCM; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.MultiPCM; nVI = newParam.MultiPCM; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.K051649; nVI = newParam.K051649; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.K053260; nVI = newParam.K053260; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.K054539; nVI = newParam.K054539; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.QSound; nVI = newParam.QSound; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.GA20; nVI = newParam.GA20; drawVolAndFader(num, oVI, nVI);

            num++; oVI = oldParam.APU; nVI = newParam.APU; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.DMC; nVI = newParam.DMC; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.FDS; nVI = newParam.FDS; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.MMC5; nVI = newParam.MMC5; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.N160; nVI = newParam.N160; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.VRC6; nVI = newParam.VRC6; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.VRC7; nVI = newParam.VRC7; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.FME7; nVI = newParam.FME7; drawVolAndFader(num, oVI, nVI);
            num++; oVI = oldParam.DMG; nVI = newParam.DMG; drawVolAndFader(num, oVI, nVI);
            num++;
            num++;
            num++;
            num++;
            num++;
            num++;
            num++;

        }

        private void drawVolAndFader(int num, MDChipParams.Mixer.VolumeInfo oVI, MDChipParams.Mixer.VolumeInfo nVI)
        {
            DrawBuff.drawFader(
                frameBuffer
                , 5 + (num % 16) * 20
                , 16 + (num / 16) * 8 * 9
                , num == 0 ? 0 : 1
                , ref oVI.Volume
                , nVI.Volume);
            nVI.VisVol2Cnt--;
            if (nVI.VisVol2Cnt == 0)
            {
                nVI.VisVol2Cnt = 1;
                if (nVI.VisVolume2 > 0) nVI.VisVolume2--;
            }
            DrawBuff.MixerVolume(
                frameBuffer
                , 2 + (num % 16) * 20
                , 10 + (num / 16) * 8 * 9
                , ref oVI.VisVolume1
                , nVI.VisVolume1
                , ref oVI.VisVolume2
                , nVI.VisVolume2);
        }

        public void screenInit()
        {
            Audio.visVolume.master = -1;
            Audio.visVolume.ym2151 = -1;
            Audio.visVolume.ym2203 = -1;
            Audio.visVolume.ym2203FM = -1;
            Audio.visVolume.ym2203SSG = -1;
            Audio.visVolume.ym2612 = -1;
            Audio.visVolume.ym2608 = -1;
            Audio.visVolume.ym2608APCM = -1;
            Audio.visVolume.ym2608FM = -1;
            Audio.visVolume.ym2608Rtm = -1;
            Audio.visVolume.ym2608SSG = -1;
            Audio.visVolume.ym2610 = -1;
            Audio.visVolume.ym2610APCMA = -1;
            Audio.visVolume.ym2610APCMB = -1;
            Audio.visVolume.ym2610FM = -1;
            Audio.visVolume.ym2610SSG = -1;

            Audio.visVolume.ym2413 = -1;
            Audio.visVolume.ym3526 = -1;
            Audio.visVolume.y8950 = -1;
            Audio.visVolume.ym3812 = -1;
            Audio.visVolume.ymf262 = -1;
            Audio.visVolume.ymf278b = -1;
            Audio.visVolume.ymz280b = -1;
            Audio.visVolume.ymf271 = -1;
            Audio.visVolume.ay8910 = -1;
            Audio.visVolume.sn76489 = -1;
            Audio.visVolume.huc6280 = -1;

            Audio.visVolume.rf5c164 = -1;
            Audio.visVolume.rf5c68 = -1;
            Audio.visVolume.pwm = -1;
            Audio.visVolume.okim6258 = -1;
            Audio.visVolume.okim6295 = -1;
            Audio.visVolume.c140 = -1;
            Audio.visVolume.c352 = -1;
            Audio.visVolume.segaPCM = -1;
            Audio.visVolume.multiPCM = -1;
            Audio.visVolume.k051649 = -1;
            Audio.visVolume.k053260 = -1;
            Audio.visVolume.k054539 = -1;
            Audio.visVolume.qSound = -1;
            Audio.visVolume.ga20 = -1;

            Audio.visVolume.APU = 0;
            Audio.visVolume.DMC = 0;
            Audio.visVolume.FDS = 0;
            Audio.visVolume.MMC5 = 0;
            Audio.visVolume.N160 = 0;
            Audio.visVolume.VRC6 = 0;
            Audio.visVolume.VRC7 = 0;
            Audio.visVolume.FME7 = 0;
            Audio.visVolume.DMG = -1;

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / parent.setting.other.Zoom;
            int py = e.Location.Y / parent.setting.other.Zoom;
            chipn = px / 20 + (py / 72) * 16;
            bool b = e.Button == MouseButtons.Middle;
            if (b) SetVolume[chipn]?.Invoke(true, 0);
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

                if (chipn < 0 || chipn >= SetVolume.Length) return;
                SetVolume[chipn]?.Invoke(true, n);

            }
        }


        Action<bool, int>[] SetVolume = new Action<bool, int>[]
        {
                  Audio.SetMasterVolume    , Audio.SetYM2151Volume       , Audio.SetYM2203Volume       , Audio.SetYM2203FMVolume
                , Audio.SetYM2203PSGVolume , Audio.SetYM2612Volume       , Audio.SetYM2608Volume       , Audio.SetYM2608FMVolume
                , Audio.SetYM2608PSGVolume , Audio.SetYM2608RhythmVolume , Audio.SetYM2608AdpcmVolume  , Audio.SetYM2610Volume
                , Audio.SetYM2610FMVolume  , Audio.SetYM2610PSGVolume    , Audio.SetYM2610AdpcmAVolume , Audio.SetYM2610AdpcmBVolume

                , Audio.SetYM2413Volume    , Audio.SetYM3526Volume       , Audio.SetY8950Volume        , Audio.SetYM3812Volume
                , Audio.SetYMF262Volume    , Audio.SetYMF278BVolume      , Audio.SetYMZ280BVolume      , Audio.SetYMF271Volume
                , null                     , Audio.SetAY8910Volume       , Audio.SetSN76489Volume      , Audio.SetHuC6280Volume
                , null                     , null                        , null                        , null

                , null                     , null                        , Audio.SetRF5C164Volume      , Audio.SetRF5C68Volume
                , Audio.SetPWMVolume       , Audio.SetOKIM6258Volume     , Audio.SetOKIM6295Volume     , Audio.SetC140Volume
                , Audio.SetC352Volume      , Audio.SetSegaPCMVolume      , Audio.SetMultiPCMVolume     , Audio.SetK051649Volume
                , Audio.SetK053260Volume   , Audio.SetK054539Volume      , Audio.SetQSoundVolume       , Audio.SetGA20Volume

                , Audio.SetAPUVolume       , Audio.SetDMCVolume          , Audio.SetFDSVolume          , Audio.SetMMC5Volume
                , Audio.SetN160Volume      , Audio.SetVRC6Volume         , Audio.SetVRC7Volume         , Audio.SetFME7Volume
                , Audio.SetDMGVolume       , null                        , null                        , null
                , null                     , null                        , null                        , null
        };


        private void pbScreen_MouseEnter(object sender, EventArgs e)
        {
            pbScreen.Focus();
        }

        private void tsmiLoadDriverBalance_Click(object sender, EventArgs e)
        {

        }

        private void tsmiLoadSongBalance_Click(object sender, EventArgs e)
        {

        }

        private void tsmiSaveDriverBalance_Click(object sender, EventArgs e)
        {
            try
            {
                string retMsg = parent.SaveDriverBalance(parent.setting.balance.Copy());
                if (retMsg != "")
                {
                    MessageBox.Show(string.Format("ドライバーのミキサーバランス[{0}]を設定フォルダーに保存しました。", retMsg), "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show(string.Format("{0}",ex.Message), "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void tsmiSaveSongBalance_Click(object sender, EventArgs e)
        {
            try
            {
                Setting.Balance bln = parent.setting.balance.Copy();
                PlayList.music ms = parent.GetPlayingMusicInfo();
                if (ms == null)
                {
                    MessageBox.Show("演奏情報が取得できませんでした。\r\n演奏中又は演奏完了直後に再度お試しください。", "情報取得失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "ミキサーバランス(*.mbc)|*.mbc";
                sfd.Title = "ミキサーバランスを保存";
                sfd.InitialDirectory = System.IO.Path.GetDirectoryName(string.IsNullOrEmpty(ms.arcFileName) ? ms.fileName : ms.arcFileName);
                if (!parent.setting.autoBalance.SamePositionAsSongData) sfd.InitialDirectory = System.IO.Path.Combine(common.GetApplicationDataFolder(true), "MixerBalance");

                sfd.RestoreDirectory = false;
                sfd.FileName = System.IO.Path.GetFileName(string.IsNullOrEmpty(ms.arcFileName) ? ms.fileName : ms.arcFileName) + ".mbc";
                sfd.CheckPathExists = true;

                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                bln.Save(sfd.FileName);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show(string.Format("{0}", ex.Message), "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
