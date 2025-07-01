namespace MDPlayer.form
{
    public partial class frmPianoRoll : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private const int WIDTH = 1024;
        private const int HEIGHT = 384;

        private readonly FrameBuffer frameBuffer = new FrameBuffer();
        private int zoom = 1;
        readonly Image img = new Bitmap(1024, 8 * 12 * 4);
        //Image img = new Bitmap(8 * 12 * 4,1024);
        private readonly PianoRollMng pianoRollMng = null;

        //private List<Note> lstNotes = new List<Note>();
        private const double FREQ = 44100;
        private long tick = 0;
        private int noteHeight = 4;
        private int noteThin = 3;
        private int playLine = (int)FREQ;//1秒(44100Hz)
        private double mul = 1 / FREQ * 200.0;
        private int[] kn = [1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1];
        private int[] line = new int[8 * 12 + 1];
        private bool[] lineK = new bool[8 * 12 + 1];

        public frmPianoRoll(frmMain frm, int zoom)
        {
            parent = frm;
            pianoRollMng = Audio.pianoRollMng;
            this.zoom = zoom;

            InitializeComponent();
            frameBuffer.Add(pbScreen, img, null, zoom);
            update();
        }

        private void frmPianoRoll_Shown(object sender, EventArgs e)
        {
        }

        private void frmPianoRoll_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosPianoRoll = Location;
            }
            else
            {
                parent.setting.location.PosPianoRoll = RestoreBounds.Location;
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

        private void frmPianoRoll_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new Size(frameSizeW + WIDTH * zoom, frameSizeH + HEIGHT * zoom);
            this.MinimumSize = new Size(frameSizeW + WIDTH * zoom, frameSizeH + HEIGHT * zoom);
            this.Size = new Size(frameSizeW + WIDTH * zoom, frameSizeH + HEIGHT * zoom);
            frmPianoRoll_Resize(null, null);
        }

        private void frmPianoRoll_Resize(object sender, EventArgs e)
        {

        }

        public void screenChangeParams()
        {
        }

        public void screenDrawParams()
        {
            //screenDrawParams_V();
            //return;

            int x;
            int len;

            tick = Audio.GetVgmFrameCounter();
            for (int i = 0; i < 8 * 12; i++)
            {
                int k = kn[i % kn.Length];
                frameBuffer.drawFillBox(
                    0,
                    i * noteHeight,
                    pbScreen.Width,
                    noteHeight,
                    (byte)(0x08 * k), (byte)(0x08 * k), (byte)(0x08 * k));
            }
            for (int i = 0; i < 8; i++)
            {
                frameBuffer.drawFillBox(
                    0,
                    i * 12 * noteHeight,
                    pbScreen.Width,
                    1,
                    0x18, 0x18, 0x18);
            }

            for (int i = 0; i < pianoRollMng.lstPrNote.Count; i++)
            {
                PrNote n = pianoRollMng.lstPrNote[i];
                x = (int)((n.startTick - tick + playLine) * mul);
                len = (int)(n.endTick == -1 ? pbScreen.Width : ((n.endTick - tick + playLine) * mul) - x);

                if (tick >= 0 && (x + len < 0 || len == 0) && n.endTick != -1)
                {
                    pianoRollMng.lstPrNote.RemoveAt(i);
                    i--;
                    continue;
                }
                if (n.endTick == -1 && x < 0)
                {
                    x = 0;
                }

                for (int j = 0; j < 6; j++)
                {
                    if (n.color[j] == n.trgColor[j]) continue;
                    int delta = Math.Sign(n.trgColor[j] - n.color[j])*2;
                    if (delta > 0)
                    {
                        n.color[j] = (byte)Math.Min((int)n.color[j] + delta, n.trgColor[j]);
                    }
                    else if (delta < 0)
                    {
                        n.color[j] = (byte)Math.Max((int)n.color[j] + delta, n.trgColor[j]);
                    }
                }
                if (n.startTick <= tick && (n.endTick >= tick || n.endTick == -1))
                {
                    for (int j = 0; j < 6; j++)
                    {
                        n.color[j] = n.noteColor2[j];
                        n.trgColor[j] = n.noteColor2[j];
                    }
                    if (n.key >= 0 && n.key < lineK.Length) lineK[n.key] = true;
                }
                else
                {
                    for (int j = 0; j < 6; j++)
                        n.trgColor[j] = n.noteColor1[j];
                }

                frameBuffer.drawFillBox(
                    x,
                    n.key * noteHeight + (noteHeight - noteThin) / 2,//noteがnote間の中心に描画されるように調整
                    len,
                    noteThin,
                    n.color[0], n.color[1], n.color[2],
                    n.color[3], n.color[4], n.color[5]);
            }

            x = (int)(playLine * mul);
            frameBuffer.drawFillBox(
                x,
                0,
                1,
                img.Height,
                0xd0, 0xd0, 0xd0);



            for (int j = 0; j < 8 * 12; j++)
            {
                if (!lineK[j]) continue;
                DrawBuff.drawFont4(frameBuffer, x - ((j & 1) != 0 ? (4 * 4 * 2-4) : (4 * 4 * 1)), j * noteHeight - 2, 1, Tables.kbn[(95 - j) % 12] + Tables.kbo[(95 - j) / 12]);
                lineK[j] = false;

            }


            //// ポイントしたところにノート表示
            int maxl = 0x03;
            for (int j = 0; j < 8 * 12; j++)
            {
                for (int i = 0; i < line[j]; i++)
                {
                    byte c = (byte)(Math.Max((line[j] == maxl ? 0xff : 0xff) - i * 2, 0));
                    frameBuffer.drawFillBox(
                        x + i+1,
                        j * noteHeight,
                        1,
                        noteHeight,
                        0, (byte)(c*0.9), c);
                }
                line[j] -= line[j] > 0 ? 1 : 0;
            }

            Point sp = Cursor.Position;
            Point cp = PointToClient(sp);
            int mx = cp.X/zoom;
            int my = cp.Y/zoom/noteHeight*noteHeight;

            if (my >= 0 && my < 8*12*noteHeight)
            {
                line[my / noteHeight] = maxl;
                    int n = 95 - my / noteHeight;
                    if (n >= 0) DrawBuff.drawFont8(frameBuffer, x - 8 * 4, my-2, 0, Tables.kbn[n % 12] + Tables.kbo[n / 12]);
            }
            ////
            

        }


        public void screenDrawParams_V()
        {
            this.Size = this.MaximumSize = this.MinimumSize = new Size(401, 1040);
            pbScreen.Size = new Size(384, 1024);

            int y;
            int len;

            tick = Audio.GetVgmFrameCounter();
            for (int i = 0; i < 8 * 12; i++)
            {
                int k = kn[kn.Length-1-(i % kn.Length)];
                frameBuffer.drawFillBox(
                    i * noteHeight,
                    0,
                    noteHeight,
                    pbScreen.Height,
                    (byte)(0x08 * k), (byte)(0x08 * k), (byte)(0x08 * k));
            }
            for (int i = 0; i < 8; i++)
            {
                frameBuffer.drawFillBox(
                    i * 12 * noteHeight,
                    0,
                    1,
                    pbScreen.Height,
                    0x18, 0x18, 0x18);
            }

            for (int i = 0; i < pianoRollMng.lstPrNote.Count; i++)
            {
                PrNote n = pianoRollMng.lstPrNote[i];
                y = (int)((n.startTick - tick + playLine) * mul);
                len = (int)(n.endTick == -1 ? pbScreen.Height : ((n.endTick - tick + playLine) * mul) - y);

                if (tick >= 0 && (y + len < 0 || len == 0))
                {
                    pianoRollMng.lstPrNote.RemoveAt(i);
                    i--;
                    continue;
                }

                for (int j = 0; j < 6; j++)
                {
                    if (n.color[j] == n.trgColor[j]) continue;
                    n.color[j] += (byte)(Math.Sign(n.trgColor[j] - n.color[j]) * 2);
                }
                if (n.startTick <= tick && (n.endTick >= tick || n.endTick == -1))
                {
                    for (int j = 0; j < 6; j++)
                    {
                        n.color[j] = n.noteColor2[j];
                        n.trgColor[j] = n.noteColor2[j];
                    }
                }
                else
                {
                    for (int j = 0; j < 6; j++)
                        n.trgColor[j] = n.noteColor1[j];
                }

                frameBuffer.drawFillBox(
                    (95-n.key) * noteHeight + (noteHeight - noteThin) / 2,//noteがnote間の中心に描画されるように調整
                    pbScreen.Height-1- y-len,
                    noteThin,
                    len,
                    n.color[0], n.color[1], n.color[2],
                    n.color[3], n.color[4], n.color[5]);
            }

            y = (int)(pbScreen.Height - 1 - playLine * mul);
            frameBuffer.drawFillBox(
                0,
                y,
                pbScreen.Width,
                1,
                0xd0, 0xd0, 0xd0);
        }

        public void update()
        {
            frameBuffer.Refresh(null);
        }

        public void Clear()
        {
            pianoRollMng.lstPrNote.Clear();

        }
    }
}
