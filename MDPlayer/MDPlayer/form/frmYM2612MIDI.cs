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
    public partial class frmYM2612MIDI : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int zoom = 1;

        public frmYM2612MIDI(frmMain frm, int zoom)
        {
            parent = frm;
            this.zoom = zoom;
            InitializeComponent();

            update();
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

        private void frmYM2612MIDI_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosYm2612MIDI = Location;
            isClosed = true;
        }

        private void frmYM2612MIDI_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612MIDI.Width * zoom, frameSizeH + Properties.Resources.planeYM2612MIDI.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612MIDI.Width * zoom, frameSizeH + Properties.Resources.planeYM2612MIDI.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612MIDI.Width * zoom, frameSizeH + Properties.Resources.planeYM2612MIDI.Height * zoom);
            frmYM2612MIDI_Resize(null, null);

        }

        private void frmYM2612MIDI_Resize(object sender, EventArgs e)
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

            //上部ラベル
            if (py < 8) return;

            if (py < 16)
            {
                Console.WriteLine("鍵盤");
                return;
            }
            else if (py < 32)
            {
                //Console.WriteLine("各機能メニュー");
                int u = (py - 16) / 8;
                int p = -1;
                if (px >= 1 * 8 && px < 6 * 8) p = 0;
                else if (px >= 15 * 8 && px < 20 * 8) p = 1;
                else if (px >= 23 * 8 && px < 29 * 8) p = 2;
                else if (px >= 32 * 8 && px < 38 * 8) p = 3;

                if (p == -1) return;

                switch (u * 4 + p)
                {
                    case 0:
                        Console.WriteLine("MONO");
                        break;
                    case 1:
                        Console.WriteLine("PANIC");
                        break;
                    case 2:
                        Console.WriteLine("TP.PUT");
                        break;
                    case 3:
                        Console.WriteLine("T.LOAD");
                        break;
                    case 4:
                        Console.WriteLine("POLY");
                        break;
                    case 5:
                        Console.WriteLine("L.CLS");
                        cmdLogClear();
                        break;
                    case 6:
                        Console.WriteLine("TP.GET");
                        break;
                    case 7:
                        Console.WriteLine("T.SAVE");
                        break;
                }
            }
            else if (py < 40)
            {
                Console.WriteLine("チャンネル選択 || 音色選択(1-3Ch)");
            }
            else if (py < 80)
            {
                Console.WriteLine("音色選択(1-3Ch)");
            }
            else if (py < 104)
            {
                Console.WriteLine("ログ表示部(1-3Ch)");
            }
            else if (py < 112)
            {
                Console.WriteLine("チャンネル選択 || 音色選択(4-6Ch)");
            }
            else if (py < 152)
            {
                Console.WriteLine("音色選択(4-6Ch)");
            }
            else if (py < 176)
            {
                Console.WriteLine("ログ表示部(4-6Ch)");
            }
            else
            {
                return;
            }
        }

        private void cmdLogClear()
        {
            parent.ym2612Midi_ClearNoteLog();
        }
    }
}
