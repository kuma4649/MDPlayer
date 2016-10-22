using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MDPlayer
{
    class DoubleBuffer : IDisposable
    {

        public FrameBuffer mainScreen = null;
        public FrameBuffer[] rf5c164Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] c140Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2608Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2151Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2203Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2610Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2612Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] OKIM6258Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] OKIM6295Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] SN76489Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] SegaPCMScreen = new FrameBuffer[2] { null, null };

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

        public void AddRf5c164(int chipID, PictureBox pbRf5c164Screen, Image initialRf5c164Image)
        {
            rf5c164Screen[chipID] = new FrameBuffer();
            rf5c164Screen[chipID].Add(pbRf5c164Screen, initialRf5c164Image, this.Paint,setting.other.Zoom);

        }

        public void AddC140(int chipID, PictureBox pbC140Screen, Image initialC140Image)
        {
            c140Screen[chipID] = new FrameBuffer();
            c140Screen[chipID].Add(pbC140Screen, initialC140Image, this.Paint, setting.other.Zoom);

        }

        public void AddYM2608(int chipID,PictureBox pbYM2608Screen, Image initialYM2608Image)
        {
            ym2608Screen[chipID] = new FrameBuffer();
            ym2608Screen[chipID].Add(pbYM2608Screen, initialYM2608Image, this.Paint, setting.other.Zoom);
        }

        public void AddYM2151(int chipID, PictureBox pbYM2151Screen, Image initialYM2151Image)
        {
            ym2151Screen[chipID] = new FrameBuffer();
            ym2151Screen[chipID].Add(pbYM2151Screen, initialYM2151Image, this.Paint, setting.other.Zoom);

        }

        public void AddYM2203(int chipID, PictureBox pbYM2203Screen, Image initialYM2203Image)
        {
            ym2203Screen[chipID] = new FrameBuffer();
            ym2203Screen[chipID].Add(pbYM2203Screen, initialYM2203Image, this.Paint, setting.other.Zoom);
        }

        public void AddYM2610(int chipID, PictureBox pbYM2610Screen, Image initialYM2610Image)
        {
            ym2610Screen[chipID] = new FrameBuffer();
            ym2610Screen[chipID].Add(pbYM2610Screen, initialYM2610Image, this.Paint, setting.other.Zoom);
        }

        public void AddYM2612(int chipID, PictureBox pbYM2612Screen, Image initialYM2612Image)
        {
            ym2612Screen[chipID] = new FrameBuffer();
            ym2612Screen[chipID].Add(pbYM2612Screen, initialYM2612Image, this.Paint, setting.other.Zoom);
        }

        public void AddOKIM6258(int chipID, PictureBox pbOKIM6258Screen, Image initialOKIM6258Image)
        {
            OKIM6258Screen[chipID] = new FrameBuffer();
            OKIM6258Screen[chipID].Add(pbOKIM6258Screen, initialOKIM6258Image, this.Paint, setting.other.Zoom);
        }

        public void AddOKIM6295(int chipID, PictureBox pbOKIM6295Screen, Image initialOKIM6295Image)
        {
            OKIM6295Screen[chipID] = new FrameBuffer();
            OKIM6295Screen[chipID].Add(pbOKIM6295Screen, initialOKIM6295Image, this.Paint, setting.other.Zoom);
        }

        public void AddSN76489(int chipID, PictureBox pbSN76489Screen, Image initialSN76489Image)
        {
            SN76489Screen[chipID] = new FrameBuffer();
            SN76489Screen[chipID].Add(pbSN76489Screen, initialSN76489Image, this.Paint, setting.other.Zoom);
        }

        public void AddSegaPCM(int chipID, PictureBox pbSegaPCMScreen, Image initialSegaPCMImage)
        {
            SegaPCMScreen[chipID] = new FrameBuffer();
            SegaPCMScreen[chipID].Add(pbSegaPCMScreen, initialSegaPCMImage, this.Paint, setting.other.Zoom);
        }


        public void RemoveRf5c164(int chipID)
        {
            if (rf5c164Screen[chipID] == null) return;
            rf5c164Screen[chipID].Remove(this.Paint);
        }

        public void RemoveC140(int chipID)
        {
            if (c140Screen[chipID] == null) return;
            c140Screen[chipID].Remove(this.Paint);
        }

        public void RemoveYM2608(int chipID)
        {
            if (ym2608Screen[chipID] == null) return;
            ym2608Screen[chipID].Remove(this.Paint);
        }

        public void RemoveYM2151(int chipID)
        {
            if (ym2151Screen[chipID] == null) return;
            ym2151Screen[chipID].Remove(this.Paint);
        }

        public void RemoveYM2203(int chipID)
        {
            if (ym2203Screen[chipID] == null) return;
            ym2203Screen[chipID].Remove(this.Paint);
        }

        public void RemoveYM2610(int chipID)
        {
            if (ym2610Screen[chipID] == null) return;
            ym2610Screen[chipID].Remove(this.Paint);
        }

        public void RemoveYM2612(int chipID)
        {
            if (ym2612Screen[chipID] == null) return;
            ym2612Screen[chipID].Remove(this.Paint);
        }

        public void RemoveOKIM6258(int chipID)
        {
            if (OKIM6258Screen[chipID] == null) return;
            OKIM6258Screen[chipID].Remove(this.Paint);
        }

        public void RemoveOKIM6295(int chipID)
        {
            if (OKIM6295Screen[chipID] == null) return;
            OKIM6295Screen[chipID].Remove(this.Paint);
        }

        public void RemoveSN76489(int chipID)
        {
            if (SN76489Screen[chipID] == null) return;
            SN76489Screen[chipID].Remove(this.Paint);
        }

        public void RemoveSegaPCM(int chipID)
        {
            if (SegaPCMScreen[chipID] == null) return;
            SegaPCMScreen[chipID].Remove(this.Paint);
        }

        ~DoubleBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {

            if (mainScreen != null) mainScreen.Remove(this.Paint);
            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (rf5c164Screen[chipID] != null) rf5c164Screen[chipID].Remove(this.Paint);
                if (c140Screen[chipID] != null) c140Screen[chipID].Remove(this.Paint);
                if (ym2151Screen[chipID] != null) ym2151Screen[chipID].Remove(this.Paint);
                if (ym2608Screen[chipID] != null) ym2608Screen[chipID].Remove(this.Paint);
                if (ym2203Screen[chipID] != null) ym2203Screen[chipID].Remove(this.Paint);
                if (ym2610Screen[chipID] != null) ym2610Screen[chipID].Remove(this.Paint);
                if (ym2612Screen[chipID] != null) ym2612Screen[chipID].Remove(this.Paint);
                if (OKIM6258Screen[chipID] != null) OKIM6258Screen[chipID].Remove(this.Paint);
                if (OKIM6295Screen[chipID] != null) OKIM6295Screen[chipID].Remove(this.Paint);
                if (SN76489Screen[chipID] != null) SN76489Screen[chipID].Remove(this.Paint);
                if (SegaPCMScreen[chipID] != null) SegaPCMScreen[chipID].Remove(this.Paint);
            }
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

                for (int chipID = 0; chipID < 2; chipID++)
                {
                    if (rf5c164Screen[chipID] != null)
                    {
                        try
                        {
                            rf5c164Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveRf5c164(chipID);
                            rf5c164Screen = null;
                        }
                    }

                    if (c140Screen[chipID] != null)
                    {
                        try
                        {
                            c140Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveC140(chipID);
                            c140Screen = null;
                        }
                    }

                    if (ym2151Screen[chipID] != null)
                    {
                        try
                        {
                            ym2151Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveYM2151(chipID);
                            ym2151Screen = null;
                        }
                    }

                    if (ym2608Screen[chipID] != null)
                    {
                        try
                        {
                            ym2608Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveYM2608(chipID);
                            ym2608Screen = null;
                        }
                    }

                    if (ym2203Screen[chipID] != null)
                    {
                        try
                        {
                            ym2203Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveYM2203(chipID);
                            ym2203Screen = null;
                        }
                    }

                    if (ym2610Screen[chipID] != null)
                    {
                        try
                        {
                            ym2610Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveYM2610(chipID);
                            ym2610Screen = null;
                        }
                    }

                    if (ym2612Screen[chipID] != null)
                    {
                        try
                        {
                            ym2612Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveYM2612(chipID);
                            ym2612Screen = null;
                        }
                    }

                    if (OKIM6258Screen[chipID] != null)
                    {
                        try
                        {
                            OKIM6258Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveOKIM6258(chipID);
                            OKIM6258Screen = null;
                        }
                    }

                    if (OKIM6295Screen[chipID] != null)
                    {
                        try
                        {
                            OKIM6295Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveOKIM6295(chipID);
                            OKIM6295Screen = null;
                        }
                    }

                    if (SN76489Screen[chipID] != null)
                    {
                        try
                        {
                            SN76489Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveSN76489(chipID);
                            SN76489Screen = null;
                        }
                    }

                    if (SegaPCMScreen[chipID] != null)
                    {
                        try
                        {
                            SegaPCMScreen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveSegaPCM(chipID);
                            SegaPCMScreen = null;
                        }
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

        public void drawFont8Int2(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;

                n = (n > 9) ? 0 : n;
                if (n == 0) screen.drawByteArray(x, y, fontBuf, 128, 0, 0 + t * 32, 8, 8);
                else screen.drawByteArray(x, y, fontBuf, 128, 0, 8 + t * 32, 8, 8);

                n = num / 10;
                num -= n * 10;
                x += 8;
                screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);

                n = num / 1;
                x += 8;
                screen.drawByteArray(x, y, fontBuf, 128, n * 8 , 8 + t * 32, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);

            n = num / 1;
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

        private void drawChipNameP(FrameBuffer screen, int x, int y, int t, int c)
        {
            if (screen == null)
            {
                return;
            }

            screen.drawByteArray(x, y, fontBuf, 128
                , 0 + (t % 4) * 8 * 2 + (c % 2) * 8 * 8
                , 32 * 8 + (t / 4) * 8 * 1 + (c / 2) * 8 * 4
                , 8 * 2
                , 8);

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

        public void drawChPYM2612(int chipID,int x, int y, int ch, bool mask, int tp)
        {
            if (ch == 5)
            {
                return;
            }

            if (ch < 5)
            {
                ym2612Screen[chipID].drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
            }
            else if (ch < 10)
            {
                ym2612Screen[chipID].drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, (ch - 5).ToString());
            }
        }

        public void drawCh6PYM2612(int chipID, int x, int y, int m, bool mask, int tp)
        {
            if (m == 0)
            {
                ym2612Screen[chipID].drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                ym2612Screen[chipID].drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, 0, " ");
            }
        }

        public void drawChPYM2151(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2151Screen[chipID] == null) return;

            ym2151Screen[chipID].drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            drawFont8(ym2151Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public void drawChPYM2608(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2608Screen[chipID] == null) return;

            if (ch < 6)
            {
                ym2608Screen[chipID].drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2608Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                ym2608Screen[chipID].drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2608Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                ym2608Screen[chipID].drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2608Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 9).ToString());
            }
            else
            {
                ym2608Screen[chipID].drawByteArray(x, y, fontBuf, 128, 0, 136 - (mask ? 8 : 0) + 16 * tp, 24, 8);
            }
        }

        public void drawChPYM2608Rhythm(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2608Screen[chipID] == null) return;

            drawFont4(ym2608Screen[chipID], x + 1*4, y, mask ? 1 : 0, "B");
            drawFont4(ym2608Screen[chipID], x + 14 * 4, y, mask ? 1 : 0, "S");
            drawFont4(ym2608Screen[chipID], x + 27 * 4, y, mask ? 1 : 0, "C");
            drawFont4(ym2608Screen[chipID], x + 40 * 4, y, mask ? 1 : 0, "H");
            drawFont4(ym2608Screen[chipID], x + 53 * 4, y, mask ? 1 : 0, "T");
            drawFont4(ym2608Screen[chipID], x + 66 * 4, y, mask ? 1 : 0, "R");
        }

        public void drawChPYM2610(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2610Screen[chipID] == null) return;

            if (ch < 6)
            {
                ym2610Screen[chipID].drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2610Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                ym2610Screen[chipID].drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2610Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                ym2610Screen[chipID].drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2610Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 9).ToString());
            }
            else
            {
                ym2610Screen[chipID].drawByteArray(x, y, fontBuf, 128, 7*8, 136 - (mask ? 8 : 0) + 16 * tp, 24, 8);
            }
        }

        public void drawChPYM2610Rhythm(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2610Screen[chipID] == null) return;

            drawFont4(ym2610Screen[chipID], x + 0 * 4, y, mask ? 1 : 0, "A1");
            drawFont4(ym2610Screen[chipID], x + 14 * 4, y, mask ? 1 : 0, "2");
            drawFont4(ym2610Screen[chipID], x + 27 * 4, y, mask ? 1 : 0, "3");
            drawFont4(ym2610Screen[chipID], x + 40 * 4, y, mask ? 1 : 0, "4");
            drawFont4(ym2610Screen[chipID], x + 53 * 4, y, mask ? 1 : 0, "5");
            drawFont4(ym2610Screen[chipID], x + 66 * 4, y, mask ? 1 : 0, "6");
        }

        public void drawChPYM2203(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2203Screen[chipID] == null) return;

            if (ch < 3)
            {
                ym2203Screen[chipID].drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2203Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 6)
            {
                ym2203Screen[chipID].drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2203Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 3).ToString());
            }
            else if (ch < 9)
            {
                ym2203Screen[chipID].drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
                drawFont8(ym2203Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else
            {
                ym2203Screen[chipID].drawByteArray(x, y, fontBuf, 128, 0, 136 - (mask ? 8 : 0) + 16 * tp, 24, 8);
            }
        }

        public void drawChPSN76489(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (SN76489Screen[chipID] == null) return;

            SN76489Screen[chipID].drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            drawFont8(SN76489Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }
        
        public void drawChPRF5C164(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (rf5c164Screen == null) return;

            rf5c164Screen[chipID].drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            drawFont8(rf5c164Screen[chipID], x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
        }

        public void drawChPC140(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (c140Screen[chipID] == null) return;

            c140Screen[chipID].drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            if (ch < 9) drawFont8(c140Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            else drawFont4(c140Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public void drawChPSegaPCM(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (SegaPCMScreen[chipID] == null) return;

            SegaPCMScreen[chipID].drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0) + 16 * tp, 16, 8);
            if (ch < 9) drawFont8(SegaPCMScreen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            else drawFont4(SegaPCMScreen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }


        public void drawVolume(FrameBuffer screen,int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(screen, 256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2608(int chipID,int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(ym2608Screen[chipID], 256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2608Screen[chipID], 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2608Rhythm(int chipID,int x, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            x = x * 4 * 13 + 8 * 2;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(ym2608Screen[chipID], x + i * 2, sy+8*14, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2608Screen[chipID], x+ i * 2, sy+8*14, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2610Rhythm(int chipID, int x, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            x = x * 4 * 13 + 8 * 2;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(ym2610Screen[chipID], x + i * 2, sy + 8 * 13, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2610Screen[chipID], x + i * 2, sy + 8 * 13, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeToC140(int chipID,int y, int c, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(c140Screen[chipID], 256 + i * 2, y + sy, (1 + t), 0);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(c140Screen[chipID], 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), 0);
            }

            ov = nv;

        }

        public void drawChYM2612(int chipID,int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2612(chipID,0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawCh6YM2612(int chipID, ref int ot, int nt, ref bool om, bool nm, ref int otp, int ntp)
        {

            if (ot == nt && om == nm && otp == ntp)
            {
                return;
            }

            drawCh6PYM2612(chipID,0, 48, nt, nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public void drawChRF5C164(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPRF5C164(chipID,0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2151(int chipID,int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2151(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2608(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2608(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2608Rhythm(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2608Rhythm(chipID, 0, 8 * 14, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2610(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2610(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2610Rhythm(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2610Rhythm(chipID, 0, 8 * 13, ch, nm, tp);
            om = nm;
        }

        public void drawChYM2203(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2203(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChSN76489(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPSN76489(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChC140(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPC140(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawChSegaPCM(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPSegaPCM(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawPan(FrameBuffer screen, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(screen, 24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanType2(FrameBuffer screen, int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(screen, 24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanToC140(int chipID, int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(c140Screen[chipID], 24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanYM2151(int chipID, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2151Screen[chipID], 24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanYM2608(int chipID, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2608Screen[chipID], 24, 8 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanYM2608Rhythm(int chipID,int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2608Screen[chipID], c * 4 * 13 + 8, 8 * 14, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawPanYM2610Rhythm(int chipID, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(ym2610Screen[chipID], c * 4 * 13 + 8, 8 * 13, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawButton(int c, ref int ot, int nt, ref int om, int nm)
        {
            if (ot == nt && om == nm)
            {
                return;
            }
            //drawFont8(mainScreen, 64 + c * 16, 208, 0, "  ");
            //drawFont8(mainScreen, 64 + c * 16, 216, 0, "  ");
            //drawButtonP(64 + c * 16, 208, nt * 16 + c, nm);
            drawFont8(mainScreen, 24 + c * 16, 24, 0, "  ");
            drawFont8(mainScreen, 24 + c * 16, 32, 0, "  ");
            drawButtonP(24 + c * 16, 24, nt * 16 + c, nm);
            ot = nt;
            om = nm;
        }

        public void drawKb(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
                drawFont8(screen, 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(screen, 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(screen, 296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawKbToC140(int chipID, int y, ref int ot, int nt)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(c140Screen[chipID], 32 + kx, y, kt, 0);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(c140Screen[chipID], 32 + kx, y, kt, 0);
                drawFont8(c140Screen[chipID], 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(c140Screen[chipID], 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(c140Screen[chipID], 296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawKbYM2608(int chipID,int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(ym2608Screen[chipID], 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(ym2608Screen[chipID], 32 + kx, y, kt, tp);
                drawFont8(ym2608Screen[chipID], 296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(ym2608Screen[chipID], 312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(ym2608Screen[chipID], 296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawChipName(int x, int y, int t, ref int oc, int nc)
        {
            if (oc == nc) return;

            drawChipNameP(mainScreen, x, y, t, nc);

            oc = nc;
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
                //drawFont4Int2(mainScreen, 4 * 30 + c * 4 * 11, 0, 0, 3, nt1);
                drawFont8Int2(mainScreen, 8 * 1 + c * 8 * 11, 16, 0, 3, nt1);
                ot1 = nt1;
            }
            if (ot2 != nt2)
            {
                drawFont8Int2(mainScreen, 8 * 5 + c * 8 * 11,16, 0, 2, nt2);
                //drawFont4Int2(mainScreen, 4 * 34 + c * 4 * 11, 0, 0, 2, nt2);
                ot2 = nt2;
            }
            if (ot3 != nt3)
            {
                drawFont8Int2(mainScreen, 8 * 8 + c * 8 * 11, 16, 0, 2, nt3);
                //drawFont4Int2(mainScreen, 4 * 37 + c * 4 * 11, 0, 0, 2, nt3);
                ot3 = nt3;
            }
        }


        public void drawParams(MDChipParams oldParam, MDChipParams newParam)
        {

            for (int chipID = 0; chipID < 2; chipID++)
            {
                if (ym2612Screen[chipID] != null) drawParamsToYM2612(oldParam, newParam, chipID);

                if (SN76489Screen[chipID] != null) drawParamsToSN76489(oldParam, newParam, chipID);

                if (rf5c164Screen[chipID] != null) drawParamsToRF5C164(oldParam, newParam, chipID);

                if (c140Screen[chipID] != null) drawParamsToC140(oldParam, newParam, chipID);

                if (SegaPCMScreen[chipID] != null) drawParamsToSegaPCM(oldParam, newParam, chipID);

                if (ym2151Screen[chipID] != null) drawParamsToYM2151(oldParam, newParam, chipID);

                if (ym2608Screen[chipID] != null) drawParamsToYM2608(oldParam, newParam, chipID);

                if (ym2610Screen[chipID] != null) drawParamsToYM2610(oldParam, newParam, chipID);

                if (ym2203Screen[chipID] != null) drawParamsToYM2203(oldParam, newParam, chipID);
            }

        }

        private void drawParamsToYM2203(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            int tp = setting.YM2203Type.UseScci ? 1 : 0;

            for (int c = 0; c < 6; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2203[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2203[chipID].channels[c];

                if (c < 3)
                {
                    drawVolume(ym2203Screen[chipID], c, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(ym2203Screen[chipID], c, ref oyc.note, nyc.note, tp);
                    drawInst(ym2203Screen[chipID], 1, 12, c, oyc.inst, nyc.inst);
                }
                else
                {
                    drawVolume(ym2203Screen[chipID], c+3, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(ym2203Screen[chipID], c+3, ref oyc.note, nyc.note, tp);
                }

                drawChYM2203(chipID, c, ref oyc.mask, nyc.mask, tp);

            }

            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2203[chipID].channels[c + 6];
                MDChipParams.Channel nyc = newParam.ym2203[chipID].channels[c + 6];

                drawVolume(ym2203Screen[chipID], c + 3, 0, ref oyc.volume, nyc.volume, tp);
                drawKb(ym2203Screen[chipID], c + 3, ref oyc.note, nyc.note, tp);

                drawChYM2203(chipID, c + 6, ref oyc.mask, nyc.mask, tp);
            }

        }

        private void drawParamsToYM2608(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            int tp = setting.YM2608Type.UseScci ? 1 : 0;

            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2608[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2608[chipID].channels[c];

                if (c < 6)
                {
                    drawVolumeYM2608(chipID, c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolumeYM2608(chipID, c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    drawPanYM2608(chipID, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    drawKbYM2608(chipID, c, ref oyc.note, nyc.note, tp);
                    drawInst(ym2608Screen[chipID], 1, 17, c, oyc.inst, nyc.inst);
                }
                else
                {
                    drawVolumeYM2608(chipID, c + 3, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKbYM2608(chipID, c + 3, ref oyc.note, nyc.note, tp);
                }

                drawChYM2608(chipID, c, ref oyc.mask, nyc.mask, tp);

            }
            //SSG
            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2608[chipID].channels[c + 9];
                MDChipParams.Channel nyc = newParam.ym2608[chipID].channels[c + 9];

                drawVolumeYM2608(chipID, c + 6, 0, ref oyc.volume, nyc.volume, tp);
                drawKbYM2608(chipID, c + 6, ref oyc.note, nyc.note, tp);

                drawChYM2608(chipID, c + 9, ref oyc.mask, nyc.mask, tp);
            }

            drawVolumeYM2608(chipID, 12, 1, ref oldParam.ym2608[chipID].channels[12].volumeL, newParam.ym2608[chipID].channels[12].volumeL, tp);
            drawVolumeYM2608(chipID, 12, 2, ref oldParam.ym2608[chipID].channels[12].volumeR, newParam.ym2608[chipID].channels[12].volumeR, tp);
            drawPanYM2608(chipID, 12, ref oldParam.ym2608[chipID].channels[12].pan, newParam.ym2608[chipID].channels[12].pan, ref oldParam.ym2608[chipID].channels[12].pantp, tp);
            drawKbYM2608(chipID, 12, ref oldParam.ym2608[chipID].channels[12].note, newParam.ym2608[chipID].channels[12].note, tp);
            drawChYM2608(chipID, 12, ref oldParam.ym2608[chipID].channels[12].mask, newParam.ym2608[chipID].channels[12].mask, tp);

            for (int c = 0; c < 6; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2608[chipID].channels[c + 13];
                MDChipParams.Channel nyc = newParam.ym2608[chipID].channels[c + 13];

                drawVolumeYM2608Rhythm(chipID, c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                drawVolumeYM2608Rhythm(chipID, c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                drawPanYM2608Rhythm(chipID, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
            }
            drawChYM2608Rhythm(chipID, 0, ref oldParam.ym2608[chipID].channels[13].mask, newParam.ym2608[chipID].channels[13].mask, tp);

        }

        private void drawParamsToYM2610(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            int tp = setting.YM2610Type.UseScci ? 1 : 0;

            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2610[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2610[chipID].channels[c];

                if (c < 6)
                {
                    drawVolume(ym2610Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolume(ym2610Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    drawPan(ym2610Screen[chipID], c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    drawKb(ym2610Screen[chipID], c, ref oyc.note, nyc.note, tp);
                    drawInst(ym2610Screen[chipID], 1, 17, c, oyc.inst, nyc.inst);
                }
                else
                {
                    drawVolume(ym2610Screen[chipID], c + 3, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(ym2610Screen[chipID], c + 3, ref oyc.note, nyc.note, tp);
                }

                drawChYM2610(chipID, c, ref oyc.mask, nyc.mask, tp);

            }

            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2610[chipID].channels[c + 9];
                MDChipParams.Channel nyc = newParam.ym2610[chipID].channels[c + 9];

                drawVolume(ym2610Screen[chipID], c + 6, 0, ref oyc.volume, nyc.volume, tp);
                drawKb(ym2610Screen[chipID], c + 6, ref oyc.note, nyc.note, tp);

                drawChYM2610(chipID, c + 9, ref oyc.mask, nyc.mask, tp);
            }

            drawVolume(ym2610Screen[chipID], 13, 1, ref oldParam.ym2610[chipID].channels[12].volumeL, newParam.ym2610[chipID].channels[12].volumeL, tp);
            drawVolume(ym2610Screen[chipID], 13, 2, ref oldParam.ym2610[chipID].channels[12].volumeR, newParam.ym2610[chipID].channels[12].volumeR, tp);
            drawPan(ym2610Screen[chipID], 13, ref oldParam.ym2610[chipID].channels[12].pan, newParam.ym2610[chipID].channels[12].pan, ref oldParam.ym2610[chipID].channels[12].pantp, tp);
            drawKb(ym2610Screen[chipID], 13, ref oldParam.ym2610[chipID].channels[12].note, newParam.ym2610[chipID].channels[12].note, tp);
            drawChYM2610(chipID, 13, ref oldParam.ym2610[chipID].channels[12].mask, newParam.ym2610[chipID].channels[12].mask, tp);

            for (int c = 0; c < 6; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2610[chipID].channels[c + 13];
                MDChipParams.Channel nyc = newParam.ym2610[chipID].channels[c + 13];

                drawVolumeYM2610Rhythm(chipID, c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                drawVolumeYM2610Rhythm(chipID, c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                drawPanYM2610Rhythm(chipID, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);

            }
            drawChYM2610Rhythm(chipID, 0, ref oldParam.ym2610[chipID].channels[13].mask, newParam.ym2610[chipID].channels[13].mask, tp);
        }

        private void drawParamsToYM2151(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 8; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2151[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2151[chipID].channels[c];

                int tp = setting.YM2151Type.UseScci ? 1 : 0;

                drawInst(ym2151Screen[chipID], 1, 11, c, oyc.inst, nyc.inst);

                drawPanYM2151(chipID, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                drawKb(ym2151Screen[chipID], c, ref oyc.note, nyc.note, tp);

                drawVolume(ym2151Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                drawVolume(ym2151Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp);

                drawChYM2151(chipID, c, ref oyc.mask, nyc.mask, tp);
            }
        }

        private void drawParamsToC140(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 24; c++)
            {

                MDChipParams.Channel orc = oldParam.c140[chipID].channels[c];
                MDChipParams.Channel nrc = newParam.c140[chipID].channels[c];

                drawVolumeToC140(chipID, c, 1, ref orc.volumeL, nrc.volumeL);
                drawVolumeToC140(chipID, c, 2, ref orc.volumeR, nrc.volumeR);
                drawKbToC140(chipID, c, ref orc.note, nrc.note);
                drawPanToC140(chipID, c, ref orc.pan, nrc.pan);

                drawChC140(chipID, c, ref orc.mask, nrc.mask, 0);
            }
        }

        private void drawParamsToSegaPCM(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 16; c++)
            {

                MDChipParams.Channel orc = oldParam.segaPcm[chipID].channels[c];
                MDChipParams.Channel nrc = newParam.segaPcm[chipID].channels[c];

                drawVolume(SegaPCMScreen[chipID], c, 1, ref orc.volumeL, nrc.volumeL,0);
                drawVolume(SegaPCMScreen[chipID], c, 2, ref orc.volumeR, nrc.volumeR,0);
                drawKb(SegaPCMScreen[chipID], c, ref orc.note, nrc.note, 0);
                drawPanType2(SegaPCMScreen[chipID], c, ref orc.pan, nrc.pan);

                drawChSegaPCM(chipID, c, ref orc.mask, nrc.mask, 0);
            }
        }

        private void drawParamsToRF5C164(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 8; c++)
            {

                MDChipParams.Channel orc = oldParam.rf5c164[chipID].channels[c];
                MDChipParams.Channel nrc = newParam.rf5c164[chipID].channels[c];

                drawVolume(rf5c164Screen[chipID], c, 1, ref orc.volumeL, nrc.volumeL, 0);
                drawVolume(rf5c164Screen[chipID], c, 2, ref orc.volumeR, nrc.volumeR, 0);
                drawKb(rf5c164Screen[chipID], c, ref orc.note, nrc.note, 0);
                drawPanType2(rf5c164Screen[chipID], c, ref orc.pan, nrc.pan);
                drawChRF5C164(chipID, c, ref orc.mask, nrc.mask, 0);

            }
        }

        private void drawParamsToSN76489(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 4; c++)
            {

                int tp = setting.SN76489Type.UseScci ? 1 : 0;

                MDChipParams.Channel osc = oldParam.sn76489[chipID].channels[c];
                MDChipParams.Channel nsc = newParam.sn76489[chipID].channels[c];

                drawVolume(SN76489Screen[chipID], c, 0, ref osc.volume, nsc.volume, tp);
                drawKb(SN76489Screen[chipID], c, ref osc.note, nsc.note, tp);
                drawChSN76489(chipID, c , ref osc.mask, nsc.mask, tp);
            }
        }

        private void drawParamsToYM2612(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2612[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2612[chipID].channels[c];

                int tp = setting.YM2612Type.UseScci ? 1 : 0;

                if (c < 5)
                {
                    drawVolume(ym2612Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    drawVolume(ym2612Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    drawPan(ym2612Screen[chipID], c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    drawKb(ym2612Screen[chipID], c, ref oyc.note, nyc.note, tp);
                    drawInst(ym2612Screen[chipID], 1, 12, c, oyc.inst, nyc.inst);
                    drawChYM2612(chipID, c, ref oyc.mask, nyc.mask, tp);
                }
                else if (c == 5)
                {
                    int tp6 = tp;
                    int tp6v = tp;
                    if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation)
                    {
                        tp6v = newParam.ym2612[chipID].channels[5].pcmMode == 0 ? 1 : 0;//volumeのみモードの判定を行う
                                                                                //tp6 = 0;
                    }
                    drawVolume(ym2612Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp6v);
                    drawVolume(ym2612Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp6v);
                    drawPan(ym2612Screen[chipID], c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp6);
                    drawKb(ym2612Screen[chipID], c, ref oyc.note, nyc.note, tp6);
                    drawInst(ym2612Screen[chipID], 1, 12, c, oyc.inst, nyc.inst);
                    drawCh6YM2612(chipID, ref oyc.pcmMode, nyc.pcmMode, ref oyc.mask, nyc.mask, ref oyc.tp, tp6);
                }
                else
                {
                    drawVolume(ym2612Screen[chipID], c, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(ym2612Screen[chipID], c, ref oyc.note, nyc.note, tp);
                    drawChYM2612(chipID, c, ref oyc.mask, nyc.mask, tp);
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


            for (int chipID = 0; chipID < 2; chipID++)
            {
                screenInitRF5C164(chipID);

                screenInitC140(chipID);

                screenInitYM2151(chipID);

                screenInitYM2203(chipID);

                screenInitYM2608(chipID);

                screenInitYM2610(chipID);

                screenInitYM2612(chipID);

                screenInitOKIM6258(chipID);

                screenInitOKIM6295(chipID);

                screenInitSN76489(chipID);

                screenInitSegaPCM(chipID);

            }

        }

        public void screenInitYM2608(int chipID)
        {
            //YM2608
            for (int y = 0; y < 6 + 3 + 3 + 1; y++)
            {
                int tp = setting.YM2608Type.UseScci ? 1 : 0;

                drawFont8(ym2608Screen[chipID], 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2608Screen[chipID], 32 + kx, y * 8 + 8, kt, tp);
                }

                if (y < 13)
                {
                    drawChPYM2608(chipID, 0, y * 8 + 8, y, false, tp);
                }

                if (y < 6 || y == 12)
                {
                    drawPanP(ym2608Screen[chipID], 24, y * 8 + 8, 3, tp);
                }

                int d = 99;
                if (y > 5 && y < 9)
                {
                    drawVolumeYM2608(chipID, y, 0, ref d, 0, tp);
                }
                else
                {
                    drawVolumeYM2608(chipID, y, 1, ref d, 0, tp);
                    d = 99;
                    drawVolumeYM2608(chipID, y, 2, ref d, 0, tp);
                }
            }

            for (int y = 0; y < 6; y++)
            {
                int tp = setting.YM2608Type.UseScci ? 1 : 0;
                int d = 99;
                drawPanYM2608Rhythm(chipID, y, ref d, 3, ref d, tp);
                d = 99;
                drawVolumeYM2608Rhythm(chipID, y, 1, ref d, 0, tp);
                d = 99;
                drawVolumeYM2608Rhythm(chipID, y, 2, ref d, 0, tp);
            }
        }

        public void screenInitYM2151(int chipID)
        {
            //YM2151
            for (int ch = 0; ch < 8; ch++)
            {
                int tp = setting.YM2151Type.UseScci ? 1 : 0;

                drawFont8(ym2151Screen[chipID], 296, ch * 8 + 8, 1, "   ");

                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(ym2151Screen[chipID], 32 + kx, ch * 8 + 8, kt, tp);
                }

                drawChPYM2151(chipID,0, ch * 8 + 8, ch, false, tp);
                drawPanP(ym2151Screen[chipID], 24, ch * 8 + 8, 3, tp);
                int d = 99;
                drawVolume(ym2151Screen[chipID], ch, 1, ref d, 0, tp);
                d = 99;
                drawVolume(ym2151Screen[chipID], ch, 2, ref d, 0, tp);
            }
        }

        public void screenInitC140(int chipID)
        {
            //C140
            for (int ch = 0; ch < 24; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(c140Screen[chipID], 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(c140Screen[chipID], 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(c140Screen[chipID], 24, ch * 8 + 8, 0);
                drawChPC140(chipID, 0, 8 + ch * 8, ch, false, 0);
            }
        }

        public void screenInitRF5C164(int chipID)
        {
            //RF5C164
            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(rf5c164Screen[chipID], 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(rf5c164Screen[chipID], 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(rf5c164Screen[chipID], 24, ch * 8 + 8, 0);
            }
        }

        public void screenInitYM2612(int chipID)
        {
            //for (int x = 0; x < 3; x++)
            //{
            //    drawFont4Int2(mainScreen, 4 * 30 + x * 4 * 11, 0, 0, 3, 0);
            //    drawFont4Int2(mainScreen, 4 * 34 + x * 4 * 11, 0, 0, 2, 0);
            //    drawFont4Int2(mainScreen, 4 * 37 + x * 4 * 11, 0, 0, 2, 0);
            //}

            //for (int y = 0; y < 13; y++)
            //{

            //    //note
            //    drawFont8(mainScreen, 296, y * 8 + 8, 1, "   ");

            //    //keyboard
            //    for (int i = 0; i < 96; i++)
            //    {
            //        int kx = kbl[(i % 12) * 2] + i / 12 * 28;
            //        int kt = kbl[(i % 12) * 2 + 1];
            //        if (y < 5 || y > 9)
            //        {
            //            drawKbn(mainScreen, 32 + kx, y * 8 + 8, kt, setting.YM2612Type.UseScci ? 1 : 0);
            //        }
            //        else if (y == 5)
            //        {
            //            int tp6 = setting.YM2612Type.UseScci ? 1 : 0;
            //            //if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) tp6 = 0;
            //            drawKbn(mainScreen, 32 + kx, y * 8 + 8, kt, tp6);
            //        }
            //        else
            //        {
            //            drawKbn(mainScreen, 32 + kx, y * 8 + 8, kt, setting.SN76489Type.UseScci ? 1 : 0);
            //        }
            //    }

            //    int d = 99;
            //    if (y < 6 || y > 9)
            //    {
            //        drawVolume(y, 0, ref d, 0, setting.YM2612Type.UseScci ? 1 : 0);
            //        if (y < 6)
            //        {
            //            d = 99;
            //            drawPan(y, ref d, 0, ref d, setting.YM2612Type.UseScci ? 1 : 0);
            //        }
            //    }
            //    else
            //    {
            //        drawVolume(y, 0, ref d, 0, setting.SN76489Type.UseScci ? 1 : 0);
            //    }

            //    if (y < 5)
            //    {
            //        drawChP(0, y * 8 + 8, y, false, setting.YM2612Type.UseScci ? 1 : 0);
            //    }
            //    else if (y == 5)
            //    {
            //        int tp6 = setting.YM2612Type.UseScci ? 1 : 0;
            //        //if (tp6 == 1 && setting.YM2612Type.OnlyPCMEmulation) tp6 = 0;
            //        drawCh6P(0, y * 8 + 8, 0, false, tp6);
            //    }
            //    else if (y < 10)
            //    {
            //        drawChP(0, y * 8 + 8, y, false, setting.SN76489Type.UseScci ? 1 : 0);
            //    }
            //    else
            //    {
            //        drawChP(0, y * 8 + 8, y, false, setting.YM2612Type.UseScci ? 1 : 0);
            //    }

            //}
        }

        public void screenInitYM2610(int chipID)
        {
            int tp = setting.YM2610Type.UseScci ? 1 : 0;
            //YM2610
            for (int y = 0; y < 14; y++)
            {
                drawFont8(ym2610Screen[chipID], 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2610Screen[chipID], 32 + kx, y * 8 + 8, kt, tp);
                }

                if (y < 13)
                {
                    drawChPYM2610(chipID, 0, y * 8 + 8, y, false, tp);
                }

                if (y < 6 || y == 13)
                {
                    drawPanP(ym2610Screen[chipID], 24, y * 8 + 8, 3, tp);
                }

                int d = 99;
                if (y > 5 && y < 9)
                {
                    drawVolume(ym2610Screen[chipID], y, 0, ref d, 0, tp);
                }
                else
                {
                    drawVolume(ym2610Screen[chipID], y, 1, ref d, 0, tp);
                    d = 99;
                    drawVolume(ym2610Screen[chipID], y, 2, ref d, 0, tp);
                }
            }

            for (int y = 0; y < 6; y++)
            {
                int d = 99;
                drawPanYM2610Rhythm(chipID, y, ref d, 3, ref d, tp);
                d = 99;
                drawVolumeYM2610Rhythm(chipID, y, 1, ref d, 0, tp);
                d = 99;
                drawVolumeYM2610Rhythm(chipID, y, 2, ref d, 0, tp);
            }
            bool f = true;
            drawChYM2610Rhythm(chipID, 0, ref f, false, tp);
        }

        public void screenInitYM2203(int chipID)
        {
            //YM2203
            for (int y = 0; y < 3 + 3 + 3; y++)
            {
                int tp = setting.YM2203Type.UseScci ? 1 : 0;

                drawFont8(ym2203Screen[chipID], 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2203Screen[chipID], 32 + kx, y * 8 + 8, kt, tp);
                }

                //if (y < 13)
                //{
                //drawChPYM2608(chipID, 0, y * 8 + 8, y, false, tp);
                //}

                int d = 99;
                drawVolume(ym2203Screen[chipID], y, 0, ref d, 0, tp);
            }

        }

        public void screenInitOKIM6258(int chipID)
        {
        }

        public void screenInitOKIM6295(int chipID)
        {
        }

        public void screenInitSN76489(int chipID)
        {
        }

        public void screenInitSegaPCM(int chipID)
        {
            for (int ch = 0; ch < 16; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(SegaPCMScreen[chipID], 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(SegaPCMScreen[chipID], 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(SegaPCMScreen[chipID], 24, ch * 8 + 8, 0);
                drawChPSegaPCM(chipID, 0, 8 + ch * 8, ch, false, 0);
            }
        }

    }
}

