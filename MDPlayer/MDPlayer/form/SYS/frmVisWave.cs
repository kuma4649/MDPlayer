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

        private short[][] buf = new short[2][] { new short[2048], new short[2048] };
        private Graphics g;
        private Bitmap bmp;

        public frmVisWave()
        {
            InitializeComponent();
            bmp = new Bitmap(400,400);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Audio.CopyWaveBuffer(buf);

            g.Clear(Color.Black);

            for (int ch = 0; ch < 2; ch++)
            {
                int hPos = bmp.Height / 4 + ch * bmp.Height / 2;
                int ox = 0;
                int oy = hPos;

                for (int i = 0; i < 2048; i++)
                {
                    int x = (i * bmp.Width) / 2048;
                    int y = buf[ch][i] * bmp.Height / 65536 + hPos;
                    g.DrawLine(Pens.Green, ox, oy, x, y);
                    ox = x;
                    oy = y;
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
    }
}
