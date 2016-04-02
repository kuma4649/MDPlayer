using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MDPlayer
{
    class DoubleBuffer : IDisposable
    {
        BufferedGraphics myBuffer;

        Control _Control;
        private Bitmap bpPlane;
        private byte[] planeBuf;
        private byte[] fontBuf;
        private static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        private static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        private static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };


        public DoubleBuffer(Control control, Image initialImage, Image font)
        {
            _Control = control;

            this.Dispose();

            // This example assumes the existence of a form called control.
            System.Drawing.BufferedGraphicsContext currentContext;

            // Gets a reference to the current BufferedGraphicsContext
            currentContext = BufferedGraphicsManager.Current;
            // Creates a BufferedGraphics instance associated with control, and with 
            // dimensions the same size as the drawing surface of control.
            myBuffer = currentContext.Allocate(control.CreateGraphics(),
               control.DisplayRectangle);

            _Control.Paint += new System.Windows.Forms.PaintEventHandler(this.Paint);

            bpPlane = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppArgb);
            BitmapData bdPlane = bpPlane.LockBits(new Rectangle(0, 0, bpPlane.Width, bpPlane.Height), ImageLockMode.ReadOnly, bpPlane.PixelFormat);
            planeBuf = new byte[bdPlane.Stride * bpPlane.Height];
            System.Runtime.InteropServices.Marshal.Copy(bdPlane.Scan0, planeBuf, 0, planeBuf.Length);
            bpPlane.UnlockBits(bdPlane);

            myBuffer.Graphics.DrawImage(initialImage, 0, 0, new Rectangle(0, 0, initialImage.Width, initialImage.Height), GraphicsUnit.Pixel);

            fontBuf = getByteArray(font);
        }

        ~DoubleBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (bpPlane != null)
            {
                bpPlane.Dispose();
                bpPlane = null;
            }
            if (myBuffer != null)
            {
                myBuffer.Dispose();
                myBuffer = null;
            }
            _Control.Paint -= new System.Windows.Forms.PaintEventHandler(this.Paint);
        }

        private void Paint(object sender, PaintEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            try
            {
                Action act;
                _Control.Invoke(act = () => drawScreen());

                if (myBuffer != null)
                    _Control.Invoke(act = () => myBuffer.Render());
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

        private void drawByteArray(int x, int y, byte[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
        {
            int adr1;
            int adr2;
            //int wid = bpPlane.Width * 4;
            int wid = 320 * 4;
            adr1 = wid * y + x * 4;
            adr2 = srcWidth * 4 * imgY + imgX * 4;
            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth * 4; j++)
                {
                    if (adr1 + j >= planeBuf.Length)
                    {
                        continue;
                    }
                    if (adr2 + j >= src.Length)
                    {
                        continue;
                    }
                    planeBuf[adr1 + j] = src[adr2 + j];
                }

                adr1 += wid;
                adr2 += srcWidth * 4;

            }
        }

        private void drawScreen()
        {
            
            if (bpPlane == null)
            {
                return;
            }

            BitmapData bdPlane = bpPlane.LockBits(new Rectangle(0, 0, bpPlane.Width, bpPlane.Height), ImageLockMode.WriteOnly, bpPlane.PixelFormat);
            unsafe
            {
                byte* bdP = (byte*)bdPlane.Scan0;
                int adr;
                for (int y = 0; y < bdPlane.Height; y++)
                {
                    adr = bdPlane.Stride * y;
                    for (int x = 0; x < bdPlane.Stride; x++)
                    {
                        bdP[adr + x] = planeBuf[bdPlane.Stride * y + x];
                    }
                }
            }
            bpPlane.UnlockBits(bdPlane);

            myBuffer.Graphics.DrawImage(bpPlane, 0, 0);
        }

        private void drawVolumeP(int x, int y, int t)
        {
            drawByteArray(x, y, fontBuf, 128, 2 * t, 96, 2, 8 - (t / 4) * 4);
        }

        private void drawKbn(int x, int y, int t)
        {
            switch (t)
            {
                case 0:
                    drawByteArray(x, y, fontBuf, 128, 32, 120, 4, 8);
                    break;
                case 1:
                    drawByteArray(x, y, fontBuf, 128, 36, 120, 3, 8);
                    break;
                case 2:
                    drawByteArray(x, y, fontBuf, 128, 40, 120, 4, 8);
                    break;
                case 3:
                    drawByteArray(x, y, fontBuf, 128, 44, 120, 4, 8);
                    break;
                case 4:
                    drawByteArray(x, y, fontBuf, 128, 32 + 16, 120, 4, 8);
                    break;
                case 5:
                    drawByteArray(x, y, fontBuf, 128, 36 + 16, 120, 3, 8);
                    break;
                case 6:
                    drawByteArray(x, y, fontBuf, 128, 40 + 16, 120, 4, 8);
                    break;
                case 7:
                    drawByteArray(x, y, fontBuf, 128, 44 + 16, 120, 4, 8);
                    break;
            }
        }

        private void drawPanP(int x, int y, int t)
        {
            drawByteArray(x, y, fontBuf, 128, 8 * t + 16, 96, 8, 8);
        }

        public void drawButtonP(int x, int y, int t)
        {
            switch (t)
            {
                case 0:
                    drawByteArray(x, y, fontBuf, 128, 0, 104, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 16, 104, 16, 8);
                    break;
                case 1:
                    drawByteArray(x, y, fontBuf, 128, 32, 104, 8, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 40, 104, 8, 8);
                    drawByteArray(x + 8, y, fontBuf, 128, 32, 104, 8, 8);
                    drawByteArray(x + 8, y + 8, fontBuf, 128, 40, 104, 8, 8);
                    break;
                case 2:
                    drawByteArray(x, y, fontBuf, 128, 48, 104, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 64, 104, 16, 8);
                    break;
                case 3:
                    drawByteArray(x, y, fontBuf, 128, 80, 104, 8, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 88, 104, 8, 8);
                    drawByteArray(x + 8, y, fontBuf, 128, 80, 104, 8, 8);
                    drawByteArray(x + 8, y + 8, fontBuf, 128, 88, 104, 8, 8);
                    break;
                case 4:
                    drawByteArray(x, y, fontBuf, 128, 96, 104, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 16, 104, 16, 8);
                    break;
                case 5:
                    drawByteArray(x, y, fontBuf, 128, 112, 104, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 0, 112, 16, 8);
                    break;
                case 6:
                    drawByteArray(x, y, fontBuf, 128, 16, 112, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 32, 112, 16, 8);
                    break;
                case 7:
                    drawByteArray(x, y, fontBuf, 128, 48, 112, 8, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 56, 112, 8, 8);
                    drawByteArray(x + 8, y, fontBuf, 128, 48, 112, 8, 8);
                    drawByteArray(x + 8, y + 8, fontBuf, 128, 56, 112, 8, 8);
                    break;
                case 8:
                    drawByteArray(x, y, fontBuf, 128, 64, 112, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 80, 112, 16, 8);
                    break;
                case 9:
                    drawByteArray(x, y, fontBuf, 128, 96, 112, 8, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 104, 112, 8, 8);
                    drawByteArray(x + 8, y, fontBuf, 128, 96, 112, 8, 8);
                    drawByteArray(x + 8, y + 8, fontBuf, 128, 104, 112, 8, 8);
                    break;
                case 10:
                    drawByteArray(x, y, fontBuf, 128, 112, 112, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 32, 112, 16, 8);
                    break;
                case 11:
                    drawByteArray(x, y, fontBuf, 128, 0, 120, 16, 8);
                    drawByteArray(x, y + 8, fontBuf, 128, 16, 120, 16, 8);
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
                drawByteArray(x, y, fontBuf, 128, 64, 120 - (mask ? 24 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 10)
            {
                drawByteArray(x, y, fontBuf, 128, 96, 120 - (mask ? 24 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 13)
            {
                drawByteArray(x, y, fontBuf, 128,112, 120 - (mask ? 24 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, (1 + ch - 10).ToString());
            }
        }

        public void drawCh6P(int x, int y, int m, bool mask)
        {
            if (m == 0)
            {
                drawByteArray(x, y, fontBuf, 128, 64, 120 - (mask ? 24 : 0), 16, 8);
                drawFont8(x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                drawByteArray(x, y, fontBuf, 128, 80, 120 - (mask ? 24 : 0), 16, 8);
                drawFont8(x + 16, y, 0, " ");
            }
        }


        public void drawFont4(int x, int y, int t, string msg)
        {
            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                drawByteArray(x, y, fontBuf, 128, (cd % 32) * 4, (cd / 32) * 8 + 64 + t * 16, 4, 8);
                x += 4;
            }
        }

        public void drawFont8(int x, int y, int t, string msg)
        {
            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                drawByteArray(x, y, fontBuf, 128, (cd % 16) * 8, (cd / 16) * 8 + t * 32, 8, 8);
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
                    drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
                }

                n = num / 1;
                x += 4;
                drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
            }
            else
            {
                drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);
            }

            n = num / 1;
            x += 4;
            drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
        }

        public void drawFont4Int2(int x, int y, int t, int k, int num)
        {
            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                drawByteArray(x, y, fontBuf, 128, 0, 64 + t * 16, 4, 8);

                n = num / 10;
                num -= n * 10;
                x += 4;
                drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);

                n = num / 1;
                x += 4;
                drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);

            n = num / 1;
            x += 4;
            drawByteArray(x, y, fontBuf, 128, n * 4 + 64, 64 + t * 16, 4, 8);
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
                    drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 8;
                if (n != 0 || f)
                {
                    drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
                }

                n = num / 1;
                num -= n * 1;
                x += 8;
                drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
            }
            else
            {
                drawByteArray(x, y, fontBuf, 128, 0, t * 32, 8, 8);
            }

            n = num / 1;
            num -= n * 1;
            x += 8;
            drawByteArray(x, y, fontBuf, 128, n * 8, 8 + t * 32, 8, 8);
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

        public void drawButton(ref int oy, int ny,int c, ref int ot, int nt)
        {
            if (ot == nt && oy==ny)
            {
                return;
            }
            drawFont8(224 + c * 16, 208, 0, "  ");
            drawFont8(224 + c * 16, 216, 0, "  ");
            drawButtonP(224 + c * 16, 208+ny, nt * 6+c);
            ot = nt;
            oy = ny;
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

        }

        public void drawButtons(int[] oldButtonY,int[] newButtonY,int[] oldButton, int[] newButton)
        { 

            for(int i = 0; i < 5; i++)
            {
                drawButton(ref oldButtonY[i], newButtonY[i],i, ref oldButton[i], newButton[i]);
            }

        }

    }
}