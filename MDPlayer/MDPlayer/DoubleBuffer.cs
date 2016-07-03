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

        private byte[] fontBuf;
        private static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        private static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        private static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };

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

        public void RemoveRf5c164()
        {
            rf5c164Screen.Remove(this.Paint);
        }

        ~DoubleBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {

            if (mainScreen != null) mainScreen.Remove(this.Paint);
            if (rf5c164Screen != null) rf5c164Screen.Remove(this.Paint);

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

        private void drawVolumeP(int x, int y, int t)
        {
            mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * t, 96, 2, 8 - (t / 4) * 4);
        }

        private void drawVolumePToRf5c164(int x, int y, int t)
        {
            if (rf5c164Screen != null)
            {
                rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 2 * t, 96, 2, 8 - (t / 4) * 4);
            }
        }

        private void drawKbn(int x, int y, int t)
        {
            switch (t)
            {
                case 0:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 32, 104, 4, 8);
                    break;
                case 1:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 36, 104, 3, 8);
                    break;
                case 2:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 40, 104, 4, 8);
                    break;
                case 3:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 44, 104, 4, 8);
                    break;
                case 4:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 32 + 16, 104, 4, 8);
                    break;
                case 5:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 36 + 16, 104, 3, 8);
                    break;
                case 6:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 40 + 16, 104, 4, 8);
                    break;
                case 7:
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 44 + 16, 104, 4, 8);
                    break;
            }
        }

        private void drawKbnToRf5c164(int x, int y, int t)
        {
            if (rf5c164Screen == null)
            {
                return;
            }

            switch (t)
            {
                case 0:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 32, 104, 4, 8);
                    break;
                case 1:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 36, 104, 3, 8);
                    break;
                case 2:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 40, 104, 4, 8);
                    break;
                case 3:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 44, 104, 4, 8);
                    break;
                case 4:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 32 + 16, 104, 4, 8);
                    break;
                case 5:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 36 + 16, 104, 3, 8);
                    break;
                case 6:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 40 + 16, 104, 4, 8);
                    break;
                case 7:
                    rf5c164Screen.drawByteArray(x, y, fontBuf, 128, 44 + 16, 104, 4, 8);
                    break;
            }
        }

        private void drawPanP(int x, int y, int t)
        {
            mainScreen.drawByteArray(x, y, fontBuf, 128, 8 * t + 16, 96, 8, 8);
        }

        private void drawPanPToRf5c164(int x, int y, int t)
        {
            if (rf5c164Screen == null)
            {
                return;
            }
            int p = t & 0x0f;
            rf5c164Screen.drawByteArray(x, y, fontBuf, 128, p == 0 ? 0 : (p / 4 * 4 + 4), 104, 4, 8);
            p = (t & 0xf0)>>4;
            rf5c164Screen.drawByteArray(x + 4, y, fontBuf, 128, p == 0 ? 0 : (p / 4 * 4 + 4), 104, 4, 8);
        }

        public void drawButtonP(int x, int y, int t)
        {
            switch (t)
            {
                case 0:
                case 10:
                    //setting
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (9 + (t / 10)), 16, 16);
                    break;
                case 1:
                case 11:
                    //stop
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (7 + (t / 10)), 16, 16);
                    break;
                case 2:
                case 12:
                    //pause
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 1 * 16, 16 * (7 + (t / 10)), 16, 16);
                    break;
                case 3:
                case 13:
                    //fadeout
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 4 * 16, 16 * (9 + (t / 10)), 16, 16);
                    break;
                case 4:
                case 14:
                    //slow
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * 16, 16 * (7 + (t / 10)), 16, 16);
                    break;
                case 5:
                case 15:
                    //play
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 3 * 16, 16 * (7 + (t / 10)), 16, 16);
                    break;
                case 6:
                case 16:
                    //fast
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 4 * 16, 16 * (7 + (t / 10)), 16, 16);
                    break;
                case 7:
                case 17:
                    //folder
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 5 * 16, 16 * (7 + (t / 10)), 16, 16);
                    break;
                case 8:
                case 18:
                    //info
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0 * 16, 16 * (9 + (t / 10)), 16, 16);
                    break;
                case 9:
                case 19:
                    //megacd
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 2 * 16, 16 * (9 + (t / 10)), 16, 16);
                    break;
            }
        }

        public void drawChP(int x, int y, int ch, bool mask)
        {
            if (ch == 5)
            {
                return;
            }
            if (ch < 5)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 10)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 96, 104 - (mask ? 8 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 13)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 112, 104 - (mask ? 8 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch - 10).ToString());
            }
        }

        public void drawCh6P(int x, int y, int m, bool mask)
        {
            if (m == 0)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 64, 104 - (mask ? 8 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 80, 104 - (mask ? 8 : 0), 16, 8);
                drawFont8(x + 16, y, 0, " ");
            }
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

        public void drawFont8ToRf5c164(int x, int y, int t, string msg)
        {
            if (rf5c164Screen == null)
            {
                return;
            }

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                rf5c164Screen.drawByteArray(x, y, fontBuf, 128, (cd % 16) * 8, (cd / 16) * 8 + t * 32, 8, 8);
                x += 8;
            }
        }

        public void drawFont4Int(int x, int y, int t, int k, int num)
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
                    mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    mainScreen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
                }

                n = num / 1;
                x += 4;
                mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
            }
            else
            {
                mainScreen.drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
            }

            n = num / 1;
            x += 4;
            mainScreen.drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
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

        public void drawVolume(int y, int c, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(256 + i * 2, y + sy, (1 + t));
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t));
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
                drawVolumePToRf5c164(256 + i * 2, y + sy, (1 + t));
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumePToRf5c164(256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t));
            }

            ov = nv;

        }

        public void drawCh(int ch, ref bool om, bool nm)
        {

            if (om == nm)
            {
                return;
            }

            drawChP(0, 8 + ch * 8, ch, nm);
            om = nm;
        }

        public void drawCh6(ref int ot, int nt, ref bool om, bool nm)
        {

            if (ot == nt && om == nm)
            {
                return;
            }

            drawCh6P(0, 48, nt, nm);
            ot = nt;
            om = nm;
        }

        public void drawPan(int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanP(24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawPanToRf5c164(int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanPToRf5c164(24, 8 + c * 8, nt);
            ot = nt;
        }

        public void drawButton(int c, ref int ot, int nt)
        {
            if (ot == nt)
            {
                return;
            }
            drawFont8(144 + c * 16, 208, 0, "  ");
            drawFont8(144 + c * 16, 216, 0, "  ");
            drawButtonP(144 + c * 16, 208, nt * 10 + c);
            ot = nt;
        }

        public void drawKb(int y, ref int ot, int nt)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(32 + kx, y, kt);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(32 + kx, y, kt);
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
                drawKbnToRf5c164(32 + kx, y, kt);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbnToRf5c164(32 + kx, y, kt);
                drawFont8ToRf5c164(296, y, 1, kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8ToRf5c164(312, y, 1, kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8ToRf5c164(296, y, 1, "   ");
            }

            ot = nt;
        }

        public void drawInst(int c, int[] oi, int[] ni)
        {
            int x = (c % 3) * 8 * 13 + 8;
            int y = (c / 3) * 8 * 6 + 8 * 16;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (oi[i + j * 11] != ni[i + j * 11])
                    {
                        drawFont4Int(x + i * 8 + (i > 5 ? 4 : 0), y + j * 8, 0, (i == 5) ? 3 : 2, ni[i + j * 11]);
                        oi[i + j * 11] = ni[i + j * 11];
                    }
                }
            }

            if (oi[44] != ni[44])
            {
                drawFont4Int(x + 8 * 4, y - 16, 0, 2, ni[44]);
                oi[44] = ni[44];
            }
            if (oi[45] != ni[45])
            {
                drawFont4Int(x + 8 * 6, y - 16, 0, 2, ni[45]);
                oi[45] = ni[45];
            }
            if (oi[46] != ni[46])
            {
                drawFont4Int(x + 8 * 8 + 4, y - 16, 0, 2, ni[46]);
                oi[46] = ni[46];
            }
            if (oi[47] != ni[47])
            {
                drawFont4Int(x + 8 * 11, y - 16, 0, 2, ni[47]);
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

                if (c < 6)
                {
                    drawVolume(c, 1, ref oyc.volumeL, nyc.volumeL);
                    drawVolume(c, 2, ref oyc.volumeR, nyc.volumeR);
                    drawPan(c, ref oyc.pan, nyc.pan);
                    drawKb(c, ref oyc.note, nyc.note);
                    drawInst(c, oyc.inst, nyc.inst);
                }
                else
                {
                    drawVolume(c + 4, 0, ref oyc.volumeL, nyc.volumeL);
                    drawKb(c + 4, ref oyc.note, nyc.note);
                }

            }

            for (int c = 0; c < 4; c++)
            {

                MDChipParams.SN76489.Channel osc = oldParam.sn76489.channels[c];
                MDChipParams.SN76489.Channel nsc = newParam.sn76489.channels[c];

                drawVolume(c + 6, 0, ref osc.volume, nsc.volume);
                drawKb(c + 6, ref osc.note, nsc.note);

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

        }

        public void drawButtons(int[] oldButton, int[] newButton)
        {

            for (int i = 0; i < 10; i++)
            {
                drawButton(i, ref oldButton[i], newButton[i]);
            }

        }

    }
}
