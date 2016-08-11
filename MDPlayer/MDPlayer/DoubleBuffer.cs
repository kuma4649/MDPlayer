using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MDPlayer
{
    class DoubleBuffer : IDisposable
    {

        private FrameBuffer mainScreen = null;
        private FrameBuffer rf5c164Screen = null;
        private FrameBuffer c140Screen = null;
        private FrameBuffer ym2608Screen = null;
        private FrameBuffer ym2151Screen = null;

        private byte[] fontBuf;
        private static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        private static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        private static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };

        public Setting setting = null;

        public class FrameBuffer
        {
            public PictureBox pbScreen;
            public Bitmap bmpPlane;
            public byte[] baPlaneBuffer;
            public BufferedGraphics bgPlane;

            public void Add(PictureBox pbScreen, Image initialImage, Action<object, PaintEventArgs> p)
            {
                this.pbScreen = pbScreen;
                System.Drawing.BufferedGraphicsContext currentContext;
                currentContext = BufferedGraphicsManager.Current;

                bgPlane = currentContext.Allocate(pbScreen.CreateGraphics(), pbScreen.DisplayRectangle);
                pbScreen.Paint += new System.Windows.Forms.PaintEventHandler(p);
                bmpPlane = new Bitmap(pbScreen.Width, pbScreen.Height, PixelFormat.Format32bppArgb);
                BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.ReadOnly, bmpPlane.PixelFormat);
                baPlaneBuffer = new byte[bdPlane.Stride * bmpPlane.Height];
                System.Runtime.InteropServices.Marshal.Copy(bdPlane.Scan0, baPlaneBuffer, 0, baPlaneBuffer.Length);
                bmpPlane.UnlockBits(bdPlane);
                bgPlane.Graphics.DrawImage(initialImage, 0, 0, new Rectangle(0, 0, initialImage.Width, initialImage.Height), GraphicsUnit.Pixel);
            }

            public void Remove(Action<object, PaintEventArgs> p)
            {
                if (bmpPlane != null)
                {
                    bmpPlane.Dispose();
                    bmpPlane = null;
                }
                if (bgPlane != null)
                {
                    bgPlane.Dispose();
                    bgPlane = null;
                }
                try
                {
                    if(pbScreen!= null) pbScreen.Paint -= new System.Windows.Forms.PaintEventHandler(p);
                }
                catch { }
                pbScreen = null;

                baPlaneBuffer = null;
            }

            private void drawScreen()
            {
                if (bmpPlane == null) return;

                BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.WriteOnly, bmpPlane.PixelFormat);
                unsafe
                {
                    byte* bdP = (byte*)bdPlane.Scan0;
                    int adr;
                    for (int y = 0; y < bdPlane.Height; y++)
                    {
                        adr = bdPlane.Stride * y;
                        for (int x = 0; x < bdPlane.Stride; x++)
                        {
                            bdP[adr + x] = baPlaneBuffer[bdPlane.Stride * y + x];
                        }
                    }
                }
                bmpPlane.UnlockBits(bdPlane);

                bgPlane.Graphics.DrawImage(bmpPlane, 0, 0);
            }

            public void Refresh(Action<object, PaintEventArgs> p)
            {
                Action act;
                if (pbScreen == null) return;
                pbScreen.Invoke(act = () =>
                {
                    try
                    {
                        drawScreen();
                    }
                    catch
                    {
                        Remove(p);
                    }
                    if(bgPlane!= null) bgPlane.Render();
                });
            }

            public void drawByteArray(int x, int y, byte[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
            {
                if (bmpPlane == null)
                {
                    return;
                }

                try
                {
                    int adr1;
                    int adr2;
                    int wid = bmpPlane.Width * 4;
                    adr1 = wid * y + x * 4;
                    adr2 = srcWidth * 4 * imgY + imgX * 4;
                    for (int i = 0; i < imgHeight; i++)
                    {
                        for (int j = 0; j < imgWidth * 4; j++)
                        {
                            if (baPlaneBuffer == null)
                            {
                                continue;
                            }

                            if (adr1 + j >= baPlaneBuffer.Length)
                            {
                                continue;
                            }
                            if (adr2 + j >= src.Length)
                            {
                                continue;
                            }
                            baPlaneBuffer[adr1 + j] = src[adr2 + j];
                        }

                        adr1 += wid;
                        adr2 += srcWidth * 4;

                    }
                }
                catch { }
            }

        }

        public DoubleBuffer(PictureBox pbMainScreen, Image initialImage, Image font)
        {
            this.Dispose();

            mainScreen = new FrameBuffer();
            mainScreen.Add(pbMainScreen, initialImage, this.Paint);

            fontBuf = getByteArray(font);
        }

        public void AddRf5c164(PictureBox pbRf5c164Screen, Image initialRf5c164Image)
        {
            rf5c164Screen = new FrameBuffer();
            rf5c164Screen.Add(pbRf5c164Screen, initialRf5c164Image, this.Paint);

        }

        public void AddC140(PictureBox pbC140Screen, Image initialC140Image)
        {
            c140Screen = new FrameBuffer();
            c140Screen.Add(pbC140Screen, initialC140Image, this.Paint);

        }

        public void AddYM2608(PictureBox pbYM2608Screen, Image initialYM2608Image)
        {
            ym2608Screen = new FrameBuffer();
            ym2608Screen.Add(pbYM2608Screen, initialYM2608Image, this.Paint);

        }

        public void AddYM2151(PictureBox pbYM2151Screen, Image initialYM2151Image)
        {
            ym2151Screen = new FrameBuffer();
            ym2151Screen.Add(pbYM2151Screen, initialYM2151Image, this.Paint);

        }

        public void RemoveRf5c164()
        {
            rf5c164Screen.Remove(this.Paint);
        }

        public void RemoveC140()
        {
            c140Screen.Remove(this.Paint);
        }

        public void RemoveYM2608()
        {
            ym2608Screen.Remove(this.Paint);
        }

        public void RemoveYM2151()
        {
            ym2151Screen.Remove(this.Paint);
        }

        ~DoubleBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {

            if (mainScreen != null) mainScreen.Remove(this.Paint);
            if (rf5c164Screen != null) rf5c164Screen.Remove(this.Paint);
            if (c140Screen != null) c140Screen.Remove(this.Paint);
            if (ym2151Screen != null) ym2151Screen.Remove(this.Paint);
            if (ym2608Screen != null) ym2608Screen.Remove(this.Paint);

        }

        private void Paint(object sender, PaintEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            try
            {
                if (mainScreen != null)
                {
                    try
                    {
                        mainScreen.Refresh(this.Paint);
                    }
                    catch
                    {
                        mainScreen.Remove(this.Paint);
                        mainScreen = null;
                    }
                }

                if (rf5c164Screen != null)
                {
                    try
                    {
                        rf5c164Screen.Refresh(this.Paint);
                    }
                    catch
                    {
                        RemoveRf5c164();
                        rf5c164Screen = null;
                    }
                }

                if (c140Screen != null)
                {
                    try
                    {
                        c140Screen.Refresh(this.Paint);
                    }
                    catch
                    {
                        RemoveC140();
                        c140Screen = null;
                    }
                }

                if (ym2151Screen != null)
                {
                    try
                    {
                        ym2151Screen.Refresh(this.Paint);
                    }
                    catch
                    {
                        RemoveYM2151();
                        ym2151Screen = null;
                    }
                }

                if (ym2608Screen != null)
                {
                    try
                    {
                        ym2608Screen.Refresh(this.Paint);
                    }
                    catch
                    {
                        RemoveYM2608();
                        ym2608Screen = null;
                    }
                }
            }
            catch { }
        }

        private byte[] getByteArray(Image img)
        {
            Bitmap bitmap = new Bitmap(img);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            byte[] byteArray = new byte[bitmapData.Stride * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, byteArray, 0, byteArray.Length);
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();

            return byteArray;
        }

        private void drawVolumeP(int x, int y, int t,int tp)
        {
            mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * t, 96 + 16 * tp, 2, 8 - (t / 4) * 4);
        }

        private void drawVolumePYM2151(int x, int y, int t, int tp)
        {
            ym2151Screen.drawByteArray(x, y, fontBuf, 128, 2 * t, 96 + 16 * tp, 2, 8 - (t / 4) * 4);
        }

        private void drawVolumePToOtherScreen(FrameBuffer screen, int x, int y, int t)
        {
            if (screen != null)
            {
                screen.drawByteArray(x, y, fontBuf, 128, 2 * t, 96, 2, 8 - (t / 4) * 4);
            }
        }

        private void drawKbn(int x, int y, int t, int tp)
        {
            switch (t)
            {
                case 0:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 32, 104 + 16 * tp, 4, 8);
                    break;
                case 1:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 36, 104 + 16 * tp, 3, 8);
                    break;
                case 2:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 40, 104 + 16 * tp, 4, 8);
                    break;
                case 3:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 44, 104 + 16 * tp, 4, 8);
                    break;
                case 4:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 32 + 16, 104 + 16 * tp, 4, 8);
                    break;
                case 5:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 36 + 16, 104 + 16 * tp, 3, 8);
                    break;
                case 6:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 40 + 16, 104 + 16 * tp, 4, 8);
                    break;
                case 7:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 44 + 16, 104 + 16 * tp, 4, 8);
                    break;
            }
        }

        private void drawKbnYM2151(int x, int y, int t, int tp)
        {
            if (ym2151Screen == null)
            {
                return;
            }

            switch (t)
            {
                case 0:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 32, 104 + 16 * tp, 4, 8);
                    break;
                case 1:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 36, 104 + 16 * tp, 3, 8);
                    break;
                case 2:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 40, 104 + 16 * tp, 4, 8);
                    break;
                case 3:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 44, 104 + 16 * tp, 4, 8);
                    break;
                case 4:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 32 + 16, 104 + 16 * tp, 4, 8);
                    break;
                case 5:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 36 + 16, 104 + 16 * tp, 3, 8);
                    break;
                case 6:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 40 + 16, 104 + 16 * tp, 4, 8);
                    break;
                case 7:
                    ym2151Screen.drawByteArray(x, y, fontBuf, 128, 44 + 16, 104 + 16 * tp, 4, 8);
                    break;
            }
        }

        private void drawKbnToOtherScreen(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null)
            {
                return;
            }

            switch (t)
            {
                case 0:
                    screen.drawByteArray(x, y, fontBuf, 128, 32, 104, 4, 8);
                    break;
                case 1:
                    screen.drawByteArray(x, y, fontBuf, 128, 36, 104, 3, 8);
                    break;
                case 2:
                    screen.drawByteArray(x, y, fontBuf, 128, 40, 104, 4, 8);
                    break;
                case 3:
                    screen.drawByteArray(x, y, fontBuf, 128, 44, 104, 4, 8);
                    break;
                case 4:
                    screen.drawByteArray(x, y, fontBuf, 128, 32 + 16, 104, 4, 8);
                    break;
                case 5:
                    screen.drawByteArray(x, y, fontBuf, 128, 36 + 16, 104, 3, 8);
                    break;
                case 6:
                    screen.drawByteArray(x, y, fontBuf, 128, 40 + 16, 104, 4, 8);
                    break;
                case 7:
                    screen.drawByteArray(x, y, fontBuf, 128, 44 + 16, 104, 4, 8);
                    break;
            }
        }

        private void drawPanP(int x, int y, int t, int tp)
        {
            mainScreen.drawByteArray(x, y, fontBuf, 128, 8 * t + 16, 96 + 16 * tp, 8, 8);
        }

        private void drawPanPYM2151(int x, int y, int t, int tp)
        {
            ym2151Screen.drawByteArray(x, y, fontBuf, 128, 8 * t + 16, 96 + 16 * tp, 8, 8);
        }

        private void drawPanPToOtherScreen(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null)
            {
                return;
            }
            int p = t & 0x0f;
            screen.drawByteArray(x, y, fontBuf, 128, p == 0 ? 0 : (p / 4 * 4 + 4), 104, 4, 8);
            p = (t & 0xf0)>>4;
            screen.drawByteArray(x + 4, y, fontBuf, 128, p == 0 ? 0 : (p / 4 * 4 + 4), 104, 4, 8);
        }

        public void drawButtonP(int x, int y, int t,int m)
        {
            switch (t)
            {
                case 0:
                case 0+14:
                    //setting
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (10 + (t / 14)), 16, 16);
                    break;
                case 1:
                case 1+14:
                    //stop
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (8 + (t / 14)), 16, 16);
                    break;
                case 2:
                case 2+14:
                    //pause
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 1 * 16, 16 * (8 + (t / 14)), 16, 16);
                    break;
                case 3:
                case 3+14:
                    //fadeout
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 4 * 16, 16 * (10 + (t / 14)), 16, 16);
                    break;
                case 4:
                case 4 + 14:
                    //PREV
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 6 * 16, 16 * (10 + (t / 14)), 16, 16);
                    break;
                case 5:
                case 5 + 14:
                    //slow
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * 16, 16 * (8 + (t / 14)), 16, 16);
                    break;
                case 6:
                case 6+14:
                    //play
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 3 * 16, 16 * (8 + (t / 14)), 16, 16);
                    break;
                case 7:
                case 7 + 14:
                    //fast
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 4 * 16, 16 * (8 + (t / 14)), 16, 16);
                    break;
                case 8:
                case 8 + 14:
                    //NEXT
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 7 * 16, 16 * (10 + (t / 14)), 16, 16);
                    break;
                case 9:
                case 9 + 14:
                    //loopmode
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 1 * 16 + m * 16, 16 * (12 + (t / 14)), 16, 16);
                    break;
                case 10:
                case 10 + 14:
                    //folder
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (8 + (t / 14)), 16, 16);
                    break;
                case 11:
                case 11 + 14:
                    //List
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (12 + (t / 14)), 16, 16);
                    break;
                case 12:
                case 12+14:
                    //info
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (10 + (t / 14)), 16, 16);
                    break;
                case 13:
                case 13+14:
                    //megacd
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * 16, 16 * (10 + (t / 14)), 16, 16);
                    break;
            }
        }

        public void drawChP(int x, int y, int ch, bool mask,int tp)
        {
            if (ch == 5)
            {
                return;
            }
            if (ch < 5)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0)+16*tp, 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 10)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 13)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0)+16 * tp, 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch - 10).ToString());
            }
        }

        public void drawCh6P(int x, int y, int m, bool mask,int tp)
        {
            if (m == 0)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(x + 16, y, 0, " ");
            }
        }

        public void drawChPYM2151(int x, int y, int ch, bool mask, int tp)
        {
            if (ym2151Screen == null) return;

            ym2151Screen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            drawFont8ToOtherScreen(ym2151Screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }


        public void drawFont4(int x, int y, int t, string msg)
        {
            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                mainScreen.drawByteArray(x, y, fontBuf, 128, (cd % 32) * 4, (cd / 32) * 8 + 64 + t * 16, 4, 8);
                x += 4;
            }
        }

        public void drawFont8(int x, int y, int t, string msg)
        {
            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                mainScreen.drawByteArray(x, y, fontBuf, 128, (cd % 16) * 8, (cd / 16) * 8 + t * 32, 8, 8);
                x += 8;
            }
        }

        public void drawFont8ToOtherScreen(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null)
            {
                return;
            }

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawByteArray(x, y, fontBuf, 128, (cd % 16) * 8, (cd / 16) * 8 + t * 32, 8, 8);
                x += 8;
            }
        }

        public void drawFont4Int(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            int n;
            if (k == 3)
            {
                bool f = false;
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                if (n != 0)
                {
                    screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
                }

                n = num / 1;
                x += 4;
                screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
            }
            else
            {
                screen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
            }

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
        }

        public void drawFont4Int2(int x, int y, int t, int k, int num)
        {
            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                mainScreen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);

                n = num / 10;
                num -= n * 10;
                x += 4;
                mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);

                n = num / 1;
                x += 4;
                mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);

            n = num / 1;
            x += 4;
            mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
        }

        public void drawFont8Int(int x, int y, int t, int k, int num)
        {
            int n;
            if (k == 3)
            {
                bool f = false;
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                if (n != 0)
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 8;
                if (n != 0 || f)
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
                }

                n = num / 1;
                num -= n * 1;
                x += 8;
                mainScreen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
            }
            else
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
            }

            n = num / 1;
            num -= n * 1;
            x += 8;
            mainScreen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
        }

        public void drawVolume(int y, int c, ref int ov, int nv,int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(256 + i * 2, y + sy, (1 + t),tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2151(int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumePYM2151(256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumePYM2151(256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeToRf5c164(int y, int c, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumePToOtherScreen(rf5c164Screen, 256 + i * 2, y + sy, (1 + t));
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumePToOtherScreen(rf5c164Screen,256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t));
            }

            ov = nv;

        }

        public void drawVolumeToC140(int y, int c, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumePToOtherScreen(c140Screen,256 + i * 2, y + sy, (1 + t));
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumePToOtherScreen(c140Screen,256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t));
            }

            ov = nv;

        }

        public void drawCh(int ch, ref bool om, bool nm,int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChP(0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawCh6(ref int ot, int nt, ref bool om, bool nm,ref int otp,int ntp)
        {

            if (ot == nt && om == nm && otp==ntp)
            {
                return;
            }

            drawCh6P(0, 48, nt, nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public void drawPan(int c, ref int ot, int nt,ref int otp,int ntp)
        {

            if (ot == nt && otp==ntp)
            {
                return;
            }

            drawPanP(24, 8 + c * 8, nt,ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanToRf5c164(int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanPToOtherScreen(rf5c164Screen, 24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanToC140(int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanPToOtherScreen(c140Screen,24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanYM2151(int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanPYM2151(24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawButton(int c, ref int ot, int nt,ref int om,int nm)
        {
            if (ot == nt && om==nm)
            {
                return;
            }
            drawFont8(80 + c * 16, 208, 0, "  ");
            drawFont8(80 + c * 16, 216, 0, "  ");
            drawButtonP(80 + c * 16, 208, nt * 14 + c, nm);
            ot = nt;
            om = nm;
        }

        public void drawKb(int y, ref int ot, int nt,int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(32 + kx, y, kt, tp);
                drawFont8(296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawKbYM2151(int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbnYM2151(32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbnYM2151(32 + kx, y, kt, tp);
                drawFont8ToOtherScreen(ym2151Screen,296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8ToOtherScreen(ym2151Screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8ToOtherScreen(ym2151Screen, 296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawKbToRf5c164(int y, ref int ot, int nt)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbnToOtherScreen(rf5c164Screen, 32 + kx, y, kt);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbnToOtherScreen(rf5c164Screen,32 + kx, y, kt);
                drawFont8ToOtherScreen(rf5c164Screen,296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8ToOtherScreen(rf5c164Screen,312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8ToOtherScreen(rf5c164Screen,296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawKbToC140(int y, ref int ot, int nt)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbnToOtherScreen(c140Screen, 32 + kx, y, kt);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbnToOtherScreen(c140Screen,32 + kx, y, kt);
                drawFont8ToOtherScreen(c140Screen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8ToOtherScreen(c140Screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8ToOtherScreen(c140Screen, 296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawInst(FrameBuffer screen, int c, int[] oi, int[] ni)
        {
            int x = (c % 3) * 8 * 13 + 8;
            int y = (c / 3) * 8 * 6 + 8 * 16;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (oi[i + j * 11] != ni[i + j * 11])
                    {
                        drawFont4Int(screen, x + i * 8 + (i > 5 ? 4 : 0), y + j * 8, 0, (i == 5) ? 3 : 2, ni[i + j * 11]);
                        oi[i + j * 11] = ni[i + j * 11];
                    }
                }
            }

            if (oi[44] != ni[44])
            {
                drawFont4Int(screen, x + 8 * 4, y - 16, 0, 2, ni[44]);
                oi[44] = ni[44];
            }
            if (oi[45] != ni[45])
            {
                drawFont4Int(screen, x + 8 * 6, y - 16, 0, 2, ni[45]);
                oi[45] = ni[45];
            }
            if (oi[46] != ni[46])
            {
                drawFont4Int(screen, x + 8 * 8 + 4, y - 16, 0, 2, ni[46]);
                oi[46] = ni[46];
            }
            if (oi[47] != ni[47])
            {
                drawFont4Int(screen, x + 8 * 11, y - 16, 0, 2, ni[47]);
                oi[47] = ni[47];
            }
        }

        public void drawInstYM2151(FrameBuffer screen, int c, int[] oi, int[] ni)
        {
            int x = (c % 3) * 8 * 13 + 8;
            int y = (c / 3) * 8 * 6 + 8 * 11;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (oi[i + j * 11] != ni[i + j * 11])
                    {
                        drawFont4Int(screen, x + i * 8 + (i > 5 ? 4 : 0), y + j * 8, 0, (i == 5) ? 3 : 2, ni[i + j * 11]);
                        oi[i + j * 11] = ni[i + j * 11];
                    }
                }
            }

            if (oi[44] != ni[44])
            {
                drawFont4Int(screen, x + 8 * 4, y - 16, 0, 2, ni[44]);
                oi[44] = ni[44];
            }
            if (oi[45] != ni[45])
            {
                drawFont4Int(screen, x + 8 * 6, y - 16, 0, 2, ni[45]);
                oi[45] = ni[45];
            }
            if (oi[46] != ni[46])
            {
                drawFont4Int(screen, x + 8 * 8 + 4, y - 16, 0, 2, ni[46]);
                oi[46] = ni[46];
            }
            if (oi[47] != ni[47])
            {
                drawFont4Int(screen, x + 8 * 11, y - 16, 0, 2, ni[47]);
                oi[47] = ni[47];
            }
        }

        public void drawTimer(int c, ref int ot1, ref int ot2, ref int ot3, int nt1, int nt2, int nt3)
        {
            if (ot1 != nt1)
            {
                drawFont4Int2(4 * 30 + c * 4 * 11, 0, 0, 3, nt1);
                ot1 = nt1;
            }
            if (ot2 != nt2)
            {
                drawFont4Int2(4 * 34 + c * 4 * 11, 0, 0, 2, nt2);
                ot2 = nt2;
            }
            if (ot3 != nt3)
            {
                drawFont4Int2(4 * 37 + c * 4 * 11, 0, 0, 2, nt3);
                ot3 = nt3;
            }
        }

        public void drawParams(MDChipParams oldParam, MDChipParams newParam)
        {

            for (int c = 0; c < 9; c++)
            {

                MDChipParams.YM2612.Channel oyc = oldParam.ym2612.channels[c];
                MDChipParams.YM2612.Channel nyc = newParam.ym2612.channels[c];

                int tp = setting.YM2612Type.UseScci ? 1 : 0;

                if (c < 5)
                {
                    drawVolume(c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolume(c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    drawPan(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    drawKb(c, ref oyc.note, nyc.note, tp);
                    drawInst(mainScreen, c, oyc.inst, nyc.inst);
                }
                else if (c == 5)
                {
                    int tp6 = tp;
                    if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) {
                        tp6 = newParam.ym2612.channels[5].pcmMode == 0 ? 1 : 0;
                    }
                    drawVolume(c, 1, ref oyc.volumeL, nyc.volumeL, tp6);
                    drawVolume(c, 2, ref oyc.volumeR, nyc.volumeR, tp6);
                    drawPan(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp6);
                    drawKb(c, ref oyc.note, nyc.note, tp);
                    drawInst(mainScreen, c, oyc.inst, nyc.inst);
                }
                else
                {
                    drawVolume(c + 4, 0, ref oyc.volumeL, nyc.volumeL,tp);
                    drawKb(c + 4, ref oyc.note, nyc.note, tp);
                }

            }

            for (int c = 0; c < 4; c++)
            {

                int tp = setting.SN76489Type.UseScci ? 1 : 0;

                MDChipParams.SN76489.Channel osc = oldParam.sn76489.channels[c];
                MDChipParams.SN76489.Channel nsc = newParam.sn76489.channels[c];

                drawVolume(c + 6, 0, ref osc.volume, nsc.volume, tp);
                drawKb(c + 6, ref osc.note, nsc.note, tp);

            }

            for (int c = 0; c < 8; c++)
            {

                MDChipParams.RF5C164.Channel orc = oldParam.rf5c164.channels[c];
                MDChipParams.RF5C164.Channel nrc = newParam.rf5c164.channels[c];

                drawVolumeToRf5c164(c, 1, ref orc.volumeL, nrc.volumeL);
                drawVolumeToRf5c164(c, 2, ref orc.volumeR, nrc.volumeR);
                drawKbToRf5c164(c, ref orc.note, nrc.note);
                drawPanToRf5c164(c, ref orc.pan, nrc.pan);

            }

            for (int c = 0; c < 24; c++)
            {

                MDChipParams.C140.Channel orc = oldParam.c140.channels[c];
                MDChipParams.C140.Channel nrc = newParam.c140.channels[c];

                drawVolumeToC140(c, 1, ref orc.volumeL, nrc.volumeL);
                drawVolumeToC140(c, 2, ref orc.volumeR, nrc.volumeR);
                drawKbToC140(c, ref orc.note, nrc.note);
                drawPanToC140(c, ref orc.pan, nrc.pan);

            }

            for (int c = 0; c < 8; c++)
            {
                MDChipParams.YM2151.Channel oyc = oldParam.ym2151.channels[c];
                MDChipParams.YM2151.Channel nyc = newParam.ym2151.channels[c];

                int tp = setting.YM2151Type.UseScci ? 1 : 0;

                drawInstYM2151(ym2151Screen, c, oyc.inst, nyc.inst);

                drawPanYM2151(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                drawKbYM2151(c, ref oyc.note, nyc.note, tp);

                drawVolumeYM2151(c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                drawVolumeYM2151(c, 2, ref oyc.volumeR, nyc.volumeR, tp);
            }

        }

        public void drawButtons(int[] oldButton, int[] newButton,int[] oldButtonMode,int[] newButtonMode)
        {

            for (int i = 0; i < 14; i++)
            {
                drawButton(i, ref oldButton[i], newButton[i], ref oldButtonMode[i], newButtonMode[i]);
            }

        }

        public void screenInit()
        {
            for (int x = 0; x < 3; x++)
            {
                drawFont4Int2(4 * 30 + x * 4 * 11, 0, 0, 3, 0);
                drawFont4Int2(4 * 34 + x * 4 * 11, 0, 0, 2, 0);
                drawFont4Int2(4 * 37 + x * 4 * 11, 0, 0, 2, 0);
            }

            for (int y = 0; y < 13; y++)
            {
                drawFont8(296, y*8+8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    if (y < 5 || y > 9)
                    {
                        drawKbn(32 + kx, y * 8 + 8, kt, setting.YM2612Type.UseScci ? 1 : 0);
                    }
                    else if (y == 5)
                    {
                        int tp6 = setting.YM2612Type.UseScci ? 1 : 0;
                        //if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) tp6 = 0;
                        drawKbn(32 + kx, y * 8 + 8, kt, tp6);
                    }
                    else
                    {
                        drawKbn(32 + kx, y * 8 + 8, kt, setting.SN76489Type.UseScci ? 1 : 0);
                    }
                }

                if (y < 5 || y > 9)
                {
                    drawChP(0, y * 8 + 8, y, false, setting.YM2612Type.UseScci ? 1 : 0);
                }
                else if (y == 5)
                {
                    int tp6 = setting.YM2612Type.UseScci ? 1 : 0;
                    //if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) tp6 = 0;
                    drawCh6P(0, y * 8 + 8, 0, false, tp6);
                }
                else
                {
                    drawChP(0, y * 8 + 8, y, false, setting.SN76489Type.UseScci ? 1 : 0);
                }

            }

            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbnToOtherScreen(rf5c164Screen, 32 + kx, ch * 8 + 8, kt);
                }
                drawFont8ToOtherScreen(rf5c164Screen, 296, ch * 8 + 8, 1, "   ");
                drawPanPToOtherScreen(rf5c164Screen, 24, ch * 8 + 8, 0);
            }

            for (int ch = 0; ch < 24; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbnToOtherScreen(c140Screen, 32 + kx, ch * 8 + 8, kt);
                }
                drawFont8ToOtherScreen(c140Screen, 296, ch * 8 + 8, 1, "   ");
                drawPanPToOtherScreen(c140Screen, 24, ch * 8 + 8, 0);
            }

            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbnYM2151(32 + kx, ch * 8 + 8, kt, setting.YM2151Type.UseScci ? 1 : 0);
                }

                drawChPYM2151(0, ch * 8 + 8, ch, false, setting.YM2151Type.UseScci ? 1 : 0);
            }
        }

    }
}
