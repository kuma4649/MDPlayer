using NAudio.Dsp;

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
        private int dispType = 1;
        private double dispHeight = 1.0;
        private bool fft = false;

        public frmVisWave(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
            bmp = new Bitmap(400, 400);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Audio.CopyWaveBuffer(buf);

            g.Clear(Color.Black);

            if (fft)
            {
                float[] a = ConvertTo(buf[0]);
                FFTProcess(a);
                buf[0] = ConvertTo(a);

                a = ConvertTo(buf[1]);
                FFTProcess(a);
                buf[1] = ConvertTo2(a);
            }

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
                        int y = (int)(buf[ch][i] * dispHeight) * bmp.Height / 65536 + hPos;
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


        public float[] ConvertTo(short[] src)
        {
            for (int i = 0; i < src.Length; i++)
            {
                destF[i] = src[i] / 32768.0f;
            }
            return destF;
        }

        public short[] ConvertTo(float[] src)
        {
            for (int i = 0; i < src.Length / 2; i++)
            {
                destS[i * 2] = (short)(Math.Min(Math.Max(-src[i] * 150.0f * 32768.0f * 0.6, short.MinValue), short.MaxValue));
                destS[i * 2 + 1] = (short)(Math.Min(Math.Max(-src[i] * 150.0f * 32768.0f * 0.6, short.MinValue), short.MaxValue));
            }
            return destS;
        }
        public short[] ConvertTo2(float[] src)
        {
            for (int i = 0; i < src.Length / 2; i++)
            {
                destS2[i * 2] = (short)(Math.Min(Math.Max(-src[i] * 150.0f * 32768.0f * 0.6, short.MinValue), short.MaxValue));
                destS2[i * 2 + 1] = (short)(Math.Min(Math.Max(-src[i] * 150.0f * 32768.0f * 0.6, short.MinValue), short.MaxValue));
            }
            return destS2;
        }

        private float[] destF = new float[2048];
        private short[] destS = new short[2048];
        private short[] destS2 = new short[2048];
        private Complex[] fftsample = new Complex[2048];

        public void FFTProcess(float[] sdata)
        {

            //var res = new float[fftsampleRange / 2];

            for (int i = 0; i < sdata.Length; i++)
            {
                fftsample[i].X = (float)(sdata[i] * FastFourierTransform.HammingWindow(i, sdata.Length));
                fftsample[i].Y = 0;
            }

            FastFourierTransform.FFT(true, (int)Math.Log(sdata.Length, 2), fftsample);

            for (int i = 0; i < sdata.Length; i++)
            {
                //sdata[i] = (float)Math.Log(Math.Sqrt(fftsample[i].X * fftsample[i].X + fftsample[i].Y * fftsample[i].Y))/20;//Dbに変換
                sdata[i] = (float)Math.Sqrt(fftsample[i].X * fftsample[i].X + fftsample[i].Y * fftsample[i].Y);//パワースペクトル
            }

            //return res;

        }

        private void tsbFFT_Click(object sender, EventArgs e)
        {
            fft = tsbFFT.Checked;
        }
    }
}
