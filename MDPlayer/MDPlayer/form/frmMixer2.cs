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

        public frmMixer2(frmMain frm, int zoom)
        {
            parent = frm;
            this.zoom = zoom;

            InitializeComponent();
            pbScreen.MouseWheel += new MouseEventHandler(this.pbScreen_MouseWheel);

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
        }

        public void screenDrawParams()
        {
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
                    n= (int)(n * 2.5);
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
                    Audio.SetMasterVolume(parent.setting.balance.MasterVolume = b ? v : (parent.setting.balance.MasterVolume + delta));
                    break;
                case 1:
                    Audio.SetYM2151Volume(parent.setting.balance.YM2151Volume = b ? v : (parent.setting.balance.YM2151Volume + delta));
                    break;
                case 2:
                    Audio.SetYM2203Volume(parent.setting.balance.YM2203Volume = b ? v : (parent.setting.balance.YM2203Volume + delta));
                    break;
                case 3:
                    Audio.SetYM2203FMVolume(parent.setting.balance.YM2203FMVolume = b ? v : (parent.setting.balance.YM2203FMVolume + delta));
                    break;
                case 4:
                    Audio.SetYM2203PSGVolume(parent.setting.balance.YM2203PSGVolume = b ? v : (parent.setting.balance.YM2203PSGVolume + delta));
                    break;
                case 5:
                    Audio.SetYM2413Volume(parent.setting.balance.YM2413Volume = b ? v : (parent.setting.balance.YM2413Volume + delta));
                    break;
                case 6:
                    Audio.SetYM2608Volume(parent.setting.balance.YM2608Volume = b ? v : (parent.setting.balance.YM2608Volume + delta));
                    break;
                case 7:
                    Audio.SetYM2608FMVolume(parent.setting.balance.YM2608FMVolume = b ? v : (parent.setting.balance.YM2608FMVolume + delta));
                    break;
                case 8:
                    Audio.SetYM2608PSGVolume(parent.setting.balance.YM2608PSGVolume = b ? v : (parent.setting.balance.YM2608PSGVolume + delta));
                    break;
                case 9:
                    Audio.SetYM2608RhythmVolume(parent.setting.balance.YM2608RhythmVolume = b ? v : (parent.setting.balance.YM2608RhythmVolume + delta));
                    break;
                case 10:
                    Audio.SetYM2608AdpcmVolume(parent.setting.balance.YM2608AdpcmVolume = b ? v : (parent.setting.balance.YM2608AdpcmVolume + delta));
                    break;
                case 11:
                    Audio.SetYM2610Volume(parent.setting.balance.YM2610Volume = b ? v : (parent.setting.balance.YM2610Volume + delta));
                    break;
                case 12:
                    Audio.SetYM2610FMVolume(parent.setting.balance.YM2610FMVolume = b ? v : (parent.setting.balance.YM2610FMVolume + delta));
                    break;
                case 13:
                    Audio.SetYM2610PSGVolume(parent.setting.balance.YM2610PSGVolume = b ? v : (parent.setting.balance.YM2610PSGVolume + delta));
                    break;
                case 14:
                    Audio.SetYM2610AdpcmAVolume(parent.setting.balance.YM2610AdpcmAVolume = b ? v : (parent.setting.balance.YM2610AdpcmAVolume + delta));
                    break;
                case 15:
                    Audio.SetYM2610AdpcmBVolume(parent.setting.balance.YM2610AdpcmBVolume = b ? v : (parent.setting.balance.YM2610AdpcmBVolume + delta));
                    break;

                case 16:
                    Audio.SetYM2612Volume(parent.setting.balance.YM2612Volume = b ? v : (parent.setting.balance.YM2612Volume + delta));
                    break;
                case 17:
                    Audio.SetAY8910Volume(parent.setting.balance.AY8910Volume = b ? v : (parent.setting.balance.AY8910Volume + delta));
                    break;
                case 18:
                    Audio.SetSN76489Volume(parent.setting.balance.SN76489Volume = b ? v : (parent.setting.balance.SN76489Volume + delta));
                    break;
                case 19:
                    Audio.SetHuC6280Volume(parent.setting.balance.HuC6280Volume = b ? v : (parent.setting.balance.HuC6280Volume + delta));
                    break;
                case 20:
                    Audio.SetRF5C164Volume(parent.setting.balance.RF5C164Volume = b ? v : (parent.setting.balance.RF5C164Volume + delta));
                    break;
                case 21:
                    Audio.SetPWMVolume(parent.setting.balance.PWMVolume = b ? v : (parent.setting.balance.PWMVolume + delta));
                    break;
                case 22:
                    Audio.SetOKIM6258Volume(parent.setting.balance.OKIM6258Volume = b ? v : (parent.setting.balance.OKIM6258Volume + delta));
                    break;
                case 23:
                    Audio.SetOKIM6295Volume(parent.setting.balance.OKIM6295Volume = b ? v : (parent.setting.balance.OKIM6295Volume + delta));
                    break;
                case 24:
                    Audio.SetC140Volume(parent.setting.balance.C140Volume = b ? v : (parent.setting.balance.C140Volume + delta));
                    break;
                case 25:
                    Audio.SetSegaPCMVolume(parent.setting.balance.SEGAPCMVolume = b ? v : (parent.setting.balance.SEGAPCMVolume + delta));
                    break;
                case 26:
                    Audio.SetC352Volume(parent.setting.balance.C352Volume = b ? v : (parent.setting.balance.C352Volume + delta));
                    break;
                case 30:
                    Audio.SetK054539Volume(parent.setting.balance.K054539Volume = b ? v : (parent.setting.balance.K054539Volume + delta));
                    break;
            }
        }


    }
}
