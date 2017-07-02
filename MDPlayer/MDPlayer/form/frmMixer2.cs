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
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.frmMixer2_MouseWheel);

            update();
        }

        private void frmMixer2_MouseWheel(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
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

                switch (chipn)
                {
                    case 0:
                        parent.setting.balance.MasterVolume = n;
                        Audio.SetMasterVolume(n);
                        break;
                    case 1:
                        parent.setting.balance.YM2151Volume = n;
                        Audio.SetYM2151Volume(n);
                        break;
                    case 2:
                        parent.setting.balance.YM2203Volume = n;
                        Audio.SetYM2203Volume(n);
                        break;
                    case 3:
                        parent.setting.balance.YM2203FMVolume = n;
                        Audio.SetYM2203FMVolume(n);
                        break;
                    case 4:
                        parent.setting.balance.YM2203PSGVolume = n;
                        Audio.SetYM2203PSGVolume(n);
                        break;
                    case 5:
                        parent.setting.balance.YM2413Volume = n;
                        Audio.SetYM2413Volume(n);
                        break;
                    case 6:
                        parent.setting.balance.YM2608Volume = n;
                        Audio.SetYM2608Volume(n);
                        break;
                    case 7:
                        parent.setting.balance.YM2608FMVolume = n;
                        Audio.SetYM2608FMVolume(n);
                        break;
                    case 8:
                        parent.setting.balance.YM2608PSGVolume = n;
                        Audio.SetYM2608PSGVolume(n);
                        break;
                    case 9:
                        parent.setting.balance.YM2608RhythmVolume = n;
                        Audio.SetYM2608RhythmVolume(n);
                        break;
                    case 10:
                        parent.setting.balance.YM2608AdpcmVolume = n;
                        Audio.SetYM2608AdpcmVolume(n);
                        break;
                    case 11:
                        parent.setting.balance.YM2610Volume = n;
                        Audio.SetYM2610Volume(n);
                        break;
                    case 12:
                        parent.setting.balance.YM2610FMVolume = n;
                        Audio.SetYM2610FMVolume(n);
                        break;
                    case 13:
                        parent.setting.balance.YM2610PSGVolume = n;
                        Audio.SetYM2610PSGVolume(n);
                        break;
                    case 14:
                        parent.setting.balance.YM2610AdpcmAVolume = n;
                        Audio.SetYM2610AdpcmAVolume(n);
                        break;
                    case 15:
                        parent.setting.balance.YM2610AdpcmBVolume = n;
                        Audio.SetYM2610AdpcmBVolume(n);
                        break;

                    case 16:
                        parent.setting.balance.YM2612Volume = n;
                        Audio.SetYM2612Volume(n);
                        break;
                    case 17:
                        parent.setting.balance.AY8910Volume = n;
                        Audio.SetAY8910Volume(n);
                        break;
                    case 18:
                        parent.setting.balance.SN76489Volume = n;
                        Audio.SetSN76489Volume(n);
                        break;
                    case 19:
                        parent.setting.balance.HuC6280Volume = n;
                        Audio.SetHuC6280Volume(n);
                        break;
                    case 20:
                        parent.setting.balance.RF5C164Volume = n;
                        Audio.SetRF5C164Volume(n);
                        break;
                    case 21:
                        parent.setting.balance.PWMVolume = n;
                        Audio.SetPWMVolume(n);
                        break;
                    case 22:
                        parent.setting.balance.OKIM6258Volume = n;
                        Audio.SetOKIM6258Volume(n);
                        break;
                    case 23:
                        parent.setting.balance.OKIM6295Volume = n;
                        Audio.SetOKIM6295Volume(n);
                        break;
                    case 24:
                        parent.setting.balance.C140Volume = n;
                        Audio.SetC140Volume(n);
                        break;
                    case 25:
                        parent.setting.balance.SEGAPCMVolume = n;
                        Audio.SetSegaPCMVolume(n);
                        break;
                }

            }
        }
    }
}
