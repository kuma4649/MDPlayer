using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer
{
    public class FrameBuffer
    {
        public PictureBox pbScreen;
        public Bitmap bmpPlane;
        public int bmpPlaneW = 0;
        public int bmpPlaneH = 0;
        public byte[] baPlaneBuffer;
        public BufferedGraphics bgPlane;
        public int zoom = 1;
        public Size imageSize = new Size(0, 0);

        public void Add(PictureBox pbScreen, Image initialImage, Action<object, PaintEventArgs> p, int zoom)
        {
            this.zoom = zoom;
            this.pbScreen = pbScreen;
            System.Drawing.BufferedGraphicsContext currentContext;
            currentContext = BufferedGraphicsManager.Current;
            imageSize = new Size(initialImage.Size.Width, initialImage.Size.Height);

            pbScreen.Size = new Size(imageSize.Width * zoom, imageSize.Height * zoom);

            bgPlane = currentContext.Allocate(pbScreen.CreateGraphics(), pbScreen.DisplayRectangle);
            if (p != null) pbScreen.Paint += new System.Windows.Forms.PaintEventHandler(p);
            bmpPlane = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
            bmpPlaneW = imageSize.Width;
            bmpPlaneH = imageSize.Height;
            BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.ReadOnly, bmpPlane.PixelFormat);
            baPlaneBuffer = new byte[bdPlane.Stride * bmpPlane.Height];
            System.Runtime.InteropServices.Marshal.Copy(bdPlane.Scan0, baPlaneBuffer, 0, baPlaneBuffer.Length);
            bmpPlane.UnlockBits(bdPlane);
            bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            bgPlane.Graphics.DrawImage(initialImage, 0, 0, imageSize.Width * zoom, imageSize.Height * zoom);
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
            bgPlane.Graphics.DrawImage(bmpPlane, 0, 0, bmpPlane.Width * zoom, bmpPlane.Height * zoom);

            //IntPtr hBmp = bmpPlane.GetHbitmap();
            //IntPtr hFormDC = bgPlane.Graphics.GetHdc(), hDC = CreateCompatibleDC(hFormDC);
            //IntPtr hPrevBmp = SelectObject(hDC, hBmp);
            //BitBlt(hFormDC, 0, 0, bmpPlane.Width, bmpPlane.Height, hDC, 0, 0, SRCCOPY);
            //bgPlane.Graphics.ReleaseHdc(hFormDC);
            //SelectObject(hDC, hPrevBmp);
            //DeleteDC(hDC);
            //DeleteObject(hBmp);
        }

        public void clearScreen()
        {
            for (int i = 0; i < baPlaneBuffer.Length; i+=4)
            {
                baPlaneBuffer[i] = 0x00; // R
                baPlaneBuffer[i + 1] = 0x00; // G
                baPlaneBuffer[i + 2] = 0x00; // B
                baPlaneBuffer[i + 3] = 0xFF; // A
            }
            //Array.Clear(baPlaneBuffer, 0, baPlaneBuffer.Length);
        }


        public static uint SRCINVERT = 0x00660046;
        public static uint SRCCOPY = 0x00CC0020;
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
         IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
                int nXSrc, int nYSrc, uint dwRop);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

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
            catch (ObjectDisposedException)
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
                    if (adr1 >= 0 && adr2 >= 0)
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

        public void drawByteArrayTransp(int x, int y, byte[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
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
                    if (adr1 >= 0 && adr2 >= 0)
                    {
                        for (int j = 0; j < imgWidth * 4; j += 4)
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

                            if (src[adr2 + j + 0] == 0x00 && src[adr2 + j + 1] == 0xff && src[adr2 + j + 2] == 0x00) continue;

                            baPlaneBuffer[adr1 + j + 0] = src[adr2 + j + 0];
                            baPlaneBuffer[adr1 + j + 1] = src[adr2 + j + 1];
                            baPlaneBuffer[adr1 + j + 2] = src[adr2 + j + 2];
                            baPlaneBuffer[adr1 + j + 3] = src[adr2 + j + 3];
                        }
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

}
