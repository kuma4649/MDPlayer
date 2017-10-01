using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MDPlayer
{
    class DoubleBuffer : IDisposable
    {

        public FrameBuffer mainScreen = null;

        public Setting setting = null;

        public DoubleBuffer(PictureBox pbMainScreen, Image initialImage, int zoom)
        {
            this.Dispose();

            mainScreen = new FrameBuffer();
            mainScreen.Add(pbMainScreen, initialImage, this.Paint, zoom);
        }

        ~DoubleBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (mainScreen != null) mainScreen.Remove(this.Paint);
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



            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

    }
}

