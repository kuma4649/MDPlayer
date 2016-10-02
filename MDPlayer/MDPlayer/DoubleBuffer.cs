using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MDPlayer
{
    class DoubleBuffer : IDisposable
    {

        public FrameBuffer mainScreen = null;
        public FrameBuffer rf5c164Screen = null;
        public FrameBuffer c140Screen = null;
        public FrameBuffer ym2608Screen = null;
        public FrameBuffer ym2151Screen = null;

        private byte[] fontBuf;
        private static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        private static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        private static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };

        public Setting setting = null;

        public class FrameBuffer
        {
            public PictureBox pbScreen;
            public Bitmap bmpPlane;
            public int bmpPlaneW = 0;
            public int bmpPlaneH = 0;
            public byte[] baPlaneBuffer;
            public BufferedGraphics bgPlane;
            public int zoom = 1;
            public Size imageSize = new Size(0,0);

            public void Add(PictureBox pbScreen, Image initialImage, Action<object, PaintEventArgs> p,int zoom)
            {
                this.zoom = zoom;
                this.pbScreen = pbScreen;
                System.Drawing.BufferedGraphicsContext currentContext;
                currentContext = BufferedGraphicsManager.Current;
                imageSize = new Size(initialImage.Size.Width, initialImage.Size.Height);

                pbScreen.Size = new Size(imageSize.Width * zoom, imageSize.Height * zoom);

                bgPlane = currentContext.Allocate(pbScreen.CreateGraphics(), pbScreen.DisplayRectangle);
                pbScreen.Paint += new System.Windows.Forms.PaintEventHandler(p);
                bmpPlane = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
                bmpPlaneW = imageSize.Width;
                bmpPlaneH = imageSize.Height;
                BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.ReadOnly, bmpPlane.PixelFormat);
                baPlaneBuffer = new byte[bdPlane.Stride * bmpPlane.Height];
                System.Runtime.InteropServices.Marshal.Copy(bdPlane.Scan0, baPlaneBuffer, 0, baPlaneBuffer.Length);
                bmpPlane.UnlockBits(bdPlane);
                bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                bgPlane.Graphics.DrawImage(initialImage, 0, 0, imageSize.Width*zoom, imageSize.Height*zoom);
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
                    if (pbScreen != null) pbScreen.Paint -= new System.Windows.Forms.PaintEventHandler(p);
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                }
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

                bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                bgPlane.Graphics.DrawImage(bmpPlane, 0, 0, bmpPlane.Width*zoom , bmpPlane.Height*zoom);
            }

            public void Refresh(Action<object, PaintEventArgs> p)
            {
                Action act;

                if (pbScreen == null) return;
                if (pbScreen.IsDisposed) return;

                try
                {
                    pbScreen.Invoke(act = () =>
                    {
                        try
                        {
                            drawScreen();
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            Remove(p);
                        }
                        if (bgPlane != null) bgPlane.Render();
                    });
                }
                catch (ObjectDisposedException )
                {
                    ;//握りつぶす
                }
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
                    int wid = bmpPlaneW * 4;
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
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                }
            }

        }

        public DoubleBuffer(PictureBox pbMainScreen, Image initialImage, Image font,int zoom)
        {
            this.Dispose();

            mainScreen = new FrameBuffer();
            mainScreen.Add(pbMainScreen, initialImage, this.Paint,zoom);

            fontBuf = getByteArray(font);
        }

        public void AddRf5c164(PictureBox pbRf5c164Screen, Image initialRf5c164Image)
        {
            rf5c164Screen = new FrameBuffer();
            rf5c164Screen.Add(pbRf5c164Screen, initialRf5c164Image, this.Paint,setting.other.Zoom);

        }

        public void AddC140(PictureBox pbC140Screen, Image initialC140Image)
        {
            c140Screen = new FrameBuffer();
            c140Screen.Add(pbC140Screen, initialC140Image, this.Paint, setting.other.Zoom);

        }

        public void AddYM2608(PictureBox pbYM2608Screen, Image initialYM2608Image)
        {
            ym2608Screen = new FrameBuffer();
            ym2608Screen.Add(pbYM2608Screen, initialYM2608Image, this.Paint, setting.other.Zoom);

        }

        public void AddYM2151(PictureBox pbYM2151Screen, Image initialYM2151Image)
        {
            ym2151Screen = new FrameBuffer();
            ym2151Screen.Add(pbYM2151Screen, initialYM2151Image, this.Paint, setting.other.Zoom);

        }

        public void RemoveRf5c164()
        {
            if (rf5c164Screen == null) return;
            rf5c164Screen.Remove(this.Paint);
        }

        public void RemoveC140()
        {
            if (c140Screen == null) return;
            c140Screen.Remove(this.Paint);
        }

        public void RemoveYM2608()
        {
            if (ym2608Screen == null) return;
            ym2608Screen.Remove(this.Paint);
        }

        public void RemoveYM2151()
        {
            if (ym2151Screen == null) return;
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
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
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
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
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
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
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
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
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
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                        RemoveYM2608();
                        ym2608Screen = null;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
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


        public void drawFont8(FrameBuffer screen, int x, int y, int t, string msg)
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

        public void drawFont8Int(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

            int n;
            if (k == 3)
            {
                bool f = false;
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                if (n != 0)
                {
                    screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 8;
                if (n != 0 || f)
                {
                    screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
                }

                n = num / 1;
                num -= n * 1;
                x += 8;
                screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
            }
            else
            {
                screen.drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
            }

            n = num / 1;
            num -= n * 1;
            x += 8;
            screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
        }

        public void drawFont4(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null) return;

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawByteArray(x, y, fontBuf, 128, (cd % 32) * 4, (cd / 32) * 8 + 64 + t * 16, 4, 8);
                x += 4;
            }
        }

        public void drawFont4Int(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

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

        public void drawFont4Int2(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                screen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);

                n = num / 10;
                num -= n * 10;
                x += 4;
                screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);

                n = num / 1;
                x += 4;
                screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
        }


        private void drawVolumeP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, fontBuf, 128, 2 * t, 96 + 16 * tp, 2, 8 - (t / 4) * 4);
        }


        private void drawKbn(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            switch (t)
            {
                case 0:
                    screen.drawByteArray(x, y, fontBuf, 128, 32, 104 + 16 * tp, 4, 8);
                    break;
                case 1:
                    screen.drawByteArray(x, y, fontBuf, 128, 36, 104 + 16 * tp, 3, 8);
                    break;
                case 2:
                    screen.drawByteArray(x, y, fontBuf, 128, 40, 104 + 16 * tp, 4, 8);
                    break;
                case 3:
                    screen.drawByteArray(x, y, fontBuf, 128, 44, 104 + 16 * tp, 4, 8);
                    break;
                case 4:
                    screen.drawByteArray(x, y, fontBuf, 128, 32 + 16, 104 + 16 * tp, 4, 8);
                    break;
                case 5:
                    screen.drawByteArray(x, y, fontBuf, 128, 36 + 16, 104 + 16 * tp, 3, 8);
                    break;
                case 6:
                    screen.drawByteArray(x, y, fontBuf, 128, 40 + 16, 104 + 16 * tp, 4, 8);
                    break;
                case 7:
                    screen.drawByteArray(x, y, fontBuf, 128, 44 + 16, 104 + 16 * tp, 4, 8);
                    break;
            }
        }


        private void drawPanP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, fontBuf, 128, 8 * t + 16, 96 + 16 * tp, 8, 8);
        }

        private void drawPanType2P(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null)
            {
                return;
            }
            int p = t & 0x0f;
            screen.drawByteArray(x, y, fontBuf, 128, p == 0 ? 0 : (p / 4 * 4 + 4), 104, 4, 8);
            p = (t & 0xf0) >> 4;
            screen.drawByteArray(x + 4, y, fontBuf, 128, p == 0 ? 0 : (p / 4 * 4 + 4), 104, 4, 8);
        }


        public void drawButtonP(int x, int y, int t, int m)
        {
            switch (t)
            {
                case 0:
                case 0 + 16:
                    //setting
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (12 + (t / 16)), 16, 16);
                    break;
                case 1:
                case 1 + 16:
                    //stop
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (10 + (t / 16)), 16, 16);
                    break;
                case 2:
                case 2 + 16:
                    //pause
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 1 * 16, 16 * (10 + (t / 16)), 16, 16);
                    break;
                case 3:
                case 3 + 16:
                    //fadeout
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 4 * 16, 16 * (12 + (t / 16)), 16, 16);
                    break;
                case 4:
                case 4 + 16:
                    //PREV
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 6 * 16, 16 * (12 + (t / 16)), 16, 16);
                    break;
                case 5:
                case 5 + 16:
                    //slow
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * 16, 16 * (10 + (t / 16)), 16, 16);
                    break;
                case 6:
                case 6 + 16:
                    //play
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 3 * 16, 16 * (10 + (t / 16)), 16, 16);
                    break;
                case 7:
                case 7 + 16:
                    //fast
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 4 * 16, 16 * (10 + (t / 16)), 16, 16);
                    break;
                case 8:
                case 8 + 16:
                    //NEXT
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 7 * 16, 16 * (12 + (t / 16)), 16, 16);
                    break;
                case 9:
                case 9 + 16:
                    //loopmode
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 1 * 16 + m * 16, 16 * (14 + (t / 16)), 16, 16);
                    break;
                case 10:
                case 10 + 16:
                    //folder
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (10 + (t / 16)), 16, 16);
                    break;
                case 11:
                case 11 + 16:
                    //List
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (14 + (t / 16)), 16, 16);
                    break;
                case 12:
                case 12 + 16:
                    //info
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (12 + (t / 16)), 16, 16);
                    break;
                case 13:
                case 13 + 16:
                    //megacd
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * 16, 16 * (12 + (t / 16)), 16, 16);
                    break;
                case 14:
                case 14 + 16:
                    //panel
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (14 + (t / 16)), 16, 16);
                    break;
                case 15:
                case 15 + 16:
                    //zoom
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 6 * 16, 16 * (14 + (t / 16)), 16, 16);
                    break;
            }
        }


        public void drawChP(int x, int y, int ch, bool mask, int tp)
        {
            if (ch == 5)
            {
                return;
            }

            if (ch < 5)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(mainScreen, x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
            }
            else if (ch < 10)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(mainScreen, x + 16, y, mask ? 1 : 0, (ch - 5).ToString());
            }
            else
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(mainScreen, x + 16, y, mask ? 1 : 0, (ch - 9).ToString());
            }
        }

        public void drawCh6P(int x, int y, int m, bool mask, int tp)
        {
            if (m == 0)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(mainScreen, x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(mainScreen, x + 16, y, 0, " ");
            }
        }

        public void drawChPYM2151(int x, int y, int ch, bool mask, int tp)
        {
            if (ym2151Screen == null) return;

            ym2151Screen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            drawFont8(ym2151Screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public void drawChPYM2608(int x, int y, int ch, bool mask, int tp)
        {
            if (ym2608Screen == null) return;

            if (ch < 6)
            {
                ym2608Screen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2608Screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                ym2608Screen.drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2608Screen, x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                ym2608Screen.drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2608Screen, x + 16, y, mask ? 1 : 0, (1 + ch - 9).ToString());
            }
            else
            {
                ym2608Screen.drawByteArray(x, y, fontBuf, 128, 0, 136 - (mask ? 8 : 0) + 16 * tp, 24, 8);
            }
        }

        public void drawChPRF5C164(int x, int y, int ch, bool mask, int tp)
        {
            if (rf5c164Screen == null) return;

            rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            drawFont8(rf5c164Screen, x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
        }


        public void drawVolume(int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(mainScreen, 256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(mainScreen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
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
                drawVolumeP(ym2151Screen, 256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2151Screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2608(int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(ym2608Screen, 256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2608Screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2608Rhythm(int x, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            x = x * 4 * 13 + 8 * 2;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(ym2608Screen, x + i * 2, sy+8*14, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2608Screen, x+ i * 2, sy+8*14, i > 17 ? (2 + t) : (0 + t), tp);
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
                drawVolumeP(rf5c164Screen, 256 + i * 2, y + sy, (1 + t), 0);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(rf5c164Screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), 0);
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
                drawVolumeP(c140Screen, 256 + i * 2, y + sy, (1 + t), 0);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(c140Screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), 0);
            }

            ov = nv;

        }


        public void drawCh(int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChP(0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawCh6(ref int ot, int nt, ref bool om, bool nm, ref int otp, int ntp)
        {

            if (ot == nt && om == nm && otp == ntp)
            {
                return;
            }

            drawCh6P(0, 48, nt, nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public void drawChRF5C164(int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPRF5C164(0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2151(int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2151(0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2608(int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2608(0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }


        public void drawPan(int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(mainScreen, 24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanToRf5c164(int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(rf5c164Screen, 24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanToC140(int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(c140Screen, 24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanYM2151(int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2151Screen, 24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanYM2608(int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2608Screen, 24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanYM2608Rhythm(int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2608Screen, c * 4 * 13 + 8, 8 * 14, nt, ntp);
            ot = nt;
            otp = ntp;
        }


        public void drawButton(int c, ref int ot, int nt, ref int om, int nm)
        {
            if (ot == nt && om == nm)
            {
                return;
            }
            drawFont8(mainScreen, 64 + c * 16, 208, 0, "  ");
            drawFont8(mainScreen, 64 + c * 16, 216, 0, "  ");
            drawButtonP(64 + c * 16, 208, nt * 16 + c, nm);
            ot = nt;
            om = nm;
        }


        public void drawKb(int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(mainScreen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(mainScreen, 32 + kx, y, kt, tp);
                drawFont8(mainScreen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(mainScreen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(mainScreen, 296, y, 1, "   ");
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
                drawKbn(ym2151Screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(ym2151Screen, 32 + kx, y, kt, tp);
                drawFont8(ym2151Screen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(ym2151Screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(ym2151Screen, 296, y, 1, "   ");
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
                drawKbn(rf5c164Screen, 32 + kx, y, kt, 0);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(rf5c164Screen, 32 + kx, y, kt, 0);
                drawFont8(rf5c164Screen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(rf5c164Screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(rf5c164Screen, 296, y, 1, "   ");
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
                drawKbn(c140Screen, 32 + kx, y, kt, 0);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(c140Screen, 32 + kx, y, kt, 0);
                drawFont8(c140Screen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(c140Screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(c140Screen, 296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawKbYM2608(int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(ym2608Screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(ym2608Screen, 32 + kx, y, kt, tp);
                drawFont8(ym2608Screen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(ym2608Screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(ym2608Screen, 296, y, 1, "   ");
            }

            ot = nt;
        }


        public void drawInst(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni)
        {
            int sx = (c % 3) * 8 * 13 + x * 8;
            int sy = (c / 3) * 8 * 6 + 8 * y;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (oi[i + j * 11] != ni[i + j * 11])
                    {
                        drawFont4Int(screen, sx + i * 8 + (i > 5 ? 4 : 0), sy + j * 8, 0, (i == 5) ? 3 : 2, ni[i + j * 11]);
                        oi[i + j * 11] = ni[i + j * 11];
                    }
                }
            }

            if (oi[44] != ni[44])
            {
                drawFont4Int(screen, sx + 8 * 4, sy - 16, 0, 2, ni[44]);
                oi[44] = ni[44];
            }
            if (oi[45] != ni[45])
            {
                drawFont4Int(screen, sx + 8 * 6, sy - 16, 0, 2, ni[45]);
                oi[45] = ni[45];
            }
            if (oi[46] != ni[46])
            {
                drawFont4Int(screen, sx + 8 * 8 + 4, sy - 16, 0, 2, ni[46]);
                oi[46] = ni[46];
            }
            if (oi[47] != ni[47])
            {
                drawFont4Int(screen, sx + 8 * 11, sy - 16, 0, 2, ni[47]);
                oi[47] = ni[47];
            }
        }


        public void drawTimer(int c, ref int ot1, ref int ot2, ref int ot3, int nt1, int nt2, int nt3)
        {
            if (ot1 != nt1)
            {
                drawFont4Int2(mainScreen, 4 * 30 + c * 4 * 11, 0, 0, 3, nt1);
                ot1 = nt1;
            }
            if (ot2 != nt2)
            {
                drawFont4Int2(mainScreen, 4 * 34 + c * 4 * 11, 0, 0, 2, nt2);
                ot2 = nt2;
            }
            if (ot3 != nt3)
            {
                drawFont4Int2(mainScreen, 4 * 37 + c * 4 * 11, 0, 0, 2, nt3);
                ot3 = nt3;
            }
        }


        public void drawParams(MDChipParams oldParam, MDChipParams newParam)
        {

            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2612.channels[c];
                MDChipParams.Channel nyc = newParam.ym2612.channels[c];

                int tp = setting.YM2612Type.UseScci ? 1 : 0;

                if (c < 5)
                {
                    drawVolume(c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolume(c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    drawPan(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    drawKb(c, ref oyc.note, nyc.note, tp);
                    drawInst(mainScreen, 1, 16, c, oyc.inst, nyc.inst);
                }
                else if (c == 5)
                {
                    int tp6 = tp;
                    int tp6v = tp;
                    if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation)
                    {
                        tp6v = newParam.ym2612.channels[5].pcmMode == 0 ? 1 : 0;//volumeのみモードの判定を行う
                        //tp6 = 0;
                    }
                    drawVolume(c, 1, ref oyc.volumeL, nyc.volumeL, tp6v);
                    drawVolume(c, 2, ref oyc.volumeR, nyc.volumeR, tp6v);
                    drawPan(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp6);
                    drawKb(c, ref oyc.note, nyc.note, tp6);
                    drawInst(mainScreen, 1, 16, c, oyc.inst, nyc.inst);
                }
                else
                {
                    drawVolume(c + 4, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(c + 4, ref oyc.note, nyc.note, tp);
                }

            }

            for (int c = 0; c < 4; c++)
            {

                int tp = setting.SN76489Type.UseScci ? 1 : 0;

                MDChipParams.Channel osc = oldParam.sn76489.channels[c];
                MDChipParams.Channel nsc = newParam.sn76489.channels[c];

                drawVolume(c + 6, 0, ref osc.volume, nsc.volume, tp);
                drawKb(c + 6, ref osc.note, nsc.note, tp);

            }

            if (rf5c164Screen != null)
            {
                for (int c = 0; c < 8; c++)
                {

                    MDChipParams.Channel orc = oldParam.rf5c164.channels[c];
                    MDChipParams.Channel nrc = newParam.rf5c164.channels[c];

                    drawVolumeToRf5c164(c, 1, ref orc.volumeL, nrc.volumeL);
                    drawVolumeToRf5c164(c, 2, ref orc.volumeR, nrc.volumeR);
                    drawKbToRf5c164(c, ref orc.note, nrc.note);
                    drawPanToRf5c164(c, ref orc.pan, nrc.pan);
                    drawChRF5C164(c, ref orc.mask, nrc.mask, 0);

                }
            }

            if (c140Screen != null)
            {
                for (int c = 0; c < 24; c++)
                {

                    MDChipParams.Channel orc = oldParam.c140.channels[c];
                    MDChipParams.Channel nrc = newParam.c140.channels[c];

                    drawVolumeToC140(c, 1, ref orc.volumeL, nrc.volumeL);
                    drawVolumeToC140(c, 2, ref orc.volumeR, nrc.volumeR);
                    drawKbToC140(c, ref orc.note, nrc.note);
                    drawPanToC140(c, ref orc.pan, nrc.pan);

                }
            }

            if (ym2151Screen != null)
            {
                for (int c = 0; c < 8; c++)
                {
                    MDChipParams.Channel oyc = oldParam.ym2151.channels[c];
                    MDChipParams.Channel nyc = newParam.ym2151.channels[c];

                    int tp = setting.YM2151Type.UseScci ? 1 : 0;

                    drawInst(ym2151Screen, 1, 11, c, oyc.inst, nyc.inst);

                    drawPanYM2151(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    drawKbYM2151(c, ref oyc.note, nyc.note, tp);

                    drawVolumeYM2151(c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolumeYM2151(c, 2, ref oyc.volumeR, nyc.volumeR, tp);

                    drawChYM2151(c, ref oyc.mask, nyc.mask, tp);
                }
            }

            if (ym2608Screen != null)
            {
                int tp = setting.YM2608Type.UseScci ? 1 : 0;

                for (int c = 0; c < 9; c++)
                {

                    MDChipParams.Channel oyc = oldParam.ym2608.channels[c];
                    MDChipParams.Channel nyc = newParam.ym2608.channels[c];

                    if (c < 6)
                    {
                        drawVolumeYM2608(c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                        drawVolumeYM2608(c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                        drawPanYM2608(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                        drawKbYM2608(c, ref oyc.note, nyc.note, tp);
                        drawInst(ym2608Screen, 1, 17, c, oyc.inst, nyc.inst);
                    }
                    else
                    {
                        drawVolumeYM2608(c + 3, 0, ref oyc.volumeL, nyc.volumeL, tp);
                        drawKbYM2608(c + 3, ref oyc.note, nyc.note, tp);
                    }

                    drawChYM2608(c, ref oyc.mask, nyc.mask, tp);

                }

                for (int c = 0; c < 3; c++)
                {
                    MDChipParams.Channel oyc = oldParam.ym2608.channels[c + 9];
                    MDChipParams.Channel nyc = newParam.ym2608.channels[c + 9];

                    drawVolumeYM2608(c + 6, 0, ref oyc.volume, nyc.volume, tp);
                    drawKbYM2608(c + 6, ref oyc.note, nyc.note, tp);

                    drawChYM2608(c+6, ref oyc.mask, nyc.mask, 0);
                }

                drawVolumeYM2608(12, 1, ref oldParam.ym2608.channels[12].volumeL, newParam.ym2608.channels[12].volumeL, tp);
                drawVolumeYM2608(12, 2, ref oldParam.ym2608.channels[12].volumeR, newParam.ym2608.channels[12].volumeR, tp);
                drawPanYM2608(12, ref oldParam.ym2608.channels[12].pan, newParam.ym2608.channels[12].pan, ref oldParam.ym2608.channels[12].pantp, tp);
                drawKbYM2608(12, ref oldParam.ym2608.channels[12].note, newParam.ym2608.channels[12].note, tp);

                for (int c = 0; c < 6; c++)
                {
                    MDChipParams.Channel oyc = oldParam.ym2608.channels[c + 13];
                    MDChipParams.Channel nyc = newParam.ym2608.channels[c + 13];

                    drawVolumeYM2608Rhythm(c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolumeYM2608Rhythm(c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    drawPanYM2608Rhythm(c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);

                }
            }

        }

        public void drawButtons(int[] oldButton, int[] newButton, int[] oldButtonMode, int[] newButtonMode)
        {

            for (int i = 0; i < 16; i++)
            {
                drawButton(i, ref oldButton[i], newButton[i], ref oldButtonMode[i], newButtonMode[i]);
            }

        }


        public void screenInitAll()
        {

            screenInitYM2612();

            screenInitRF5C164();

            screenInitC140();

            screenInitYM2151();

            screenInitYM2608();

        }

        public void screenInitYM2608()
        {
            //YM2608
            for (int y = 0; y < 6 + 3 + 3 + 1; y++)
            {
                int tp = setting.YM2608Type.UseScci ? 1 : 0;

                drawFont8(ym2608Screen, 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2608Screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                if (y < 13)
                {
                    drawChPYM2608(0, y * 8 + 8, y, false, tp);
                }

                if (y < 6 || y == 12)
                {
                    drawPanP(ym2608Screen, 24, y * 8 + 8, 3, tp);
                }

                int d = 99;
                if (y > 5 && y < 9)
                {
                    drawVolumeYM2608(y, 0, ref d, 0, tp);
                }
                else
                {
                    drawVolumeYM2608(y, 1, ref d, 0, tp);
                    d = 99;
                    drawVolumeYM2608(y, 2, ref d, 0, tp);
                }
            }

            for (int y = 0; y < 6; y++)
            {
                int tp = setting.YM2608Type.UseScci ? 1 : 0;
                int d = 99;
                drawPanYM2608Rhythm(y, ref d, 3, ref d, tp);
                d = 99;
                drawVolumeYM2608Rhythm(y, 1, ref d, 0, tp);
                d = 99;
                drawVolumeYM2608Rhythm(y, 2, ref d, 0, tp);
            }
        }

        public void screenInitYM2151()
        {
            //YM2151
            for (int ch = 0; ch < 8; ch++)
            {
                int tp = setting.YM2151Type.UseScci ? 1 : 0;

                drawFont8(ym2151Screen, 296, ch * 8 + 8, 1, "   ");

                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(ym2151Screen, 32 + kx, ch * 8 + 8, kt, tp);
                }

                drawChPYM2151(0, ch * 8 + 8, ch, false, tp);
                drawPanP(ym2151Screen, 24, ch * 8 + 8, 3, tp);
                int d = 99;
                drawVolumeYM2151(ch, 0, ref d, 0, tp);
            }
        }

        public void screenInitC140()
        {
            //C140
            for (int ch = 0; ch < 24; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(c140Screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(c140Screen, 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(c140Screen, 24, ch * 8 + 8, 0);
            }
        }

        public void screenInitRF5C164()
        {
            //RF5C164
            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(rf5c164Screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(rf5c164Screen, 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(rf5c164Screen, 24, ch * 8 + 8, 0);
            }
        }

        public void screenInitYM2612()
        {
            for (int x = 0; x < 3; x++)
            {
                drawFont4Int2(mainScreen, 4 * 30 + x * 4 * 11, 0, 0, 3, 0);
                drawFont4Int2(mainScreen, 4 * 34 + x * 4 * 11, 0, 0, 2, 0);
                drawFont4Int2(mainScreen, 4 * 37 + x * 4 * 11, 0, 0, 2, 0);
            }

            for (int y = 0; y < 13; y++)
            {

                //note
                drawFont8(mainScreen, 296, y * 8 + 8, 1, "   ");

                //keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    if (y < 5 || y > 9)
                    {
                        drawKbn(mainScreen, 32 + kx, y * 8 + 8, kt, setting.YM2612Type.UseScci ? 1 : 0);
                    }
                    else if (y == 5)
                    {
                        int tp6 = setting.YM2612Type.UseScci ? 1 : 0;
                        //if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) tp6 = 0;
                        drawKbn(mainScreen, 32 + kx, y * 8 + 8, kt, tp6);
                    }
                    else
                    {
                        drawKbn(mainScreen, 32 + kx, y * 8 + 8, kt, setting.SN76489Type.UseScci ? 1 : 0);
                    }
                }

                int d = 99;
                if (y < 6 || y > 9)
                {
                    drawVolume(y, 0, ref d, 0, setting.YM2612Type.UseScci ? 1 : 0);
                    if (y < 6)
                    {
                        d = 99;
                        drawPan(y, ref d, 0, ref d, setting.YM2612Type.UseScci ? 1 : 0);
                    }
                }
                else
                {
                    drawVolume(y, 0, ref d, 0, setting.SN76489Type.UseScci ? 1 : 0);
                }

                if (y < 5)
                {
                    drawChP(0, y * 8 + 8, y, false, setting.YM2612Type.UseScci ? 1 : 0);
                }
                else if (y == 5)
                {
                    int tp6 = setting.YM2612Type.UseScci ? 1 : 0;
                    //if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) tp6 = 0;
                    drawCh6P(0, y * 8 + 8, 0, false, tp6);
                }
                else if (y < 10)
                {
                    drawChP(0, y * 8 + 8, y, false, setting.SN76489Type.UseScci ? 1 : 0);
                }
                else
                {
                    drawChP(0, y * 8 + 8, y, false, setting.YM2612Type.UseScci ? 1 : 0);
                }

            }
        }

    }
}

