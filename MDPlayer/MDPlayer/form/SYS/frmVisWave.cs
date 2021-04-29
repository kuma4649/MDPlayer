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
    public partial class frmVisWave : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;

        private short[][] buf = new short[2][] { new short[2048], new short[2048] };
        private Graphics g;
        private Bitmap bmp;
        private int dispType=1;
        private double dispHeight=1.0;

        public frmVisWave(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
            bmp = new Bitmap(400,400);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Audio.CopyWaveBuffer(buf);

            g.Clear(Color.Black);

            for (int ch = 0; ch < 2; ch++)
            {
                if (dispType <= 1)
                {
                    int hPos = bmp.Height / 4 + ch * bmp.Height / 2;
                    int ox = 0;
                    int oy = hPos;

                    for (int i = 0; i < 2048; i++)
                    {
                        int x = (i * bmp.Width) / 2048;
                        int y = (int)(buf[ch][i]*dispHeight) * bmp.Height / 65536 + hPos;
                        g.DrawLine(Pens.SteelBlue, ox, oy, x, y);
                        ox = x;
                        oy = y;
                    }
                }
                else
                {
                    int hPos = bmp.Height / 4 + ch * bmp.Height / 2;
                    for (int i = 0; i < 2048; i++)
                    {
                        int x = (i * bmp.Width) / 2048;
                        int y = (int)(buf[ch][i] * dispHeight) * bmp.Height / 65536 + hPos;
                        g.DrawLine(Pens.Khaki, x, hPos, x, y);
                    }
                }
            }

            pictureBox1.Image = bmp;
        }

        private void frmVisWave_Shown(object sender, EventArgs e)
        {
            g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            
        }

        private void frmVisWave_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosVisWave = Location;
            }
            else
            {
                parent.setting.location.PosVisWave = RestoreBounds.Location;
            }
            isClosed = true;
        }
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }
        private void frmVisWave_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }

        private void tsbDispType2_Click(object sender, EventArgs e)
        {
            dispType = 2;
        }

        private void tsbDispType1_Click(object sender, EventArgs e)
        {
            dispType = 1;

        }

        private void tsbHeight3_Click(object sender, EventArgs e)
        {
            dispHeight = 3;

        }

        private void tsbHeight2_Click(object sender, EventArgs e)
        {
            dispHeight = 1.0;

        }

        private void tsbHeight1_Click(object sender, EventArgs e)
        {
            dispHeight = 0.3;

        }
    }
}
