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
        public FrameBuffer[] ym2608Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2151Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2203Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2413Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2610Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] ym2612Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] OKIM6258Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] OKIM6295Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] SN76489Screen = new FrameBuffer[2] { null, null };
        public FrameBuffer[] SegaPCMScreen = new FrameBuffer[2] { null, null };
        public FrameBuffer ym2612MIDIScreen = null;

        private byte[][] rChipName;
        private byte[][] rFont1;
        private byte[][] rFont2;
        private byte[][] rFont3;
        private byte[][] rKBD;
        private byte[][] rMenuButtons;
        private byte[][] rPan;
        private byte[][] rPan2;
        private byte[] rPSGEnv;
        private byte[][] rPSGMode;
        private byte[][] rType;
        private byte[][] rVol;
        private byte[] rWavGraph;
        private byte[] rFader;
        private byte[][] rMIDILCD_Fader;
        private byte[] rMIDILCD_KBD;
        private byte[][] rMIDILCD_Vol;
        private byte[][] rMIDILCD;
        private byte[][] rMIDILCD_Font;
        private byte[][] rPlane_MIDI;
        private Bitmap[] bitmapMIDILyric = null;
        private Graphics[] gMIDILyric = null;
        private Font[] fntMIDILyric = null;


        private static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        private static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        private static string[] kbns = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        private static string[] kbnp = new string[] { "C ", "C+", "D ", "D+", "E ", "F ", "F+", "G ", "G+", "A ", "A+", "B " };
        private static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };

        public Setting setting = null;

        public DoubleBuffer(PictureBox pbMainScreen, Image initialImage, int zoom)
        {
            this.Dispose();

            mainScreen = new FrameBuffer();
            mainScreen.Add(pbMainScreen, initialImage, this.Paint, zoom);

            rChipName = new byte[3][];
            rChipName[0] = getByteArray(Properties.Resources.rChipName_01);
            rChipName[1] = getByteArray(Properties.Resources.rChipName_02);
            rChipName[2] = getByteArray(Properties.Resources.rChipName_03);

            rFont1 = new byte[2][];
            rFont1[0] = getByteArray(Properties.Resources.rFont_01);
            rFont1[1] = getByteArray(Properties.Resources.rFont_02);
            rFont2 = new byte[5][];
            rFont2[0] = getByteArray(Properties.Resources.rFont_03);
            rFont2[1] = getByteArray(Properties.Resources.rFont_04);
            rFont2[2] = getByteArray(Properties.Resources.rMIDILCD_Font_04);
            rFont2[3] = getByteArray(Properties.Resources.rMIDILCD_Font_05);
            rFont2[4] = getByteArray(Properties.Resources.rMIDILCD_Font_06);
            rFont3 = new byte[2][];
            rFont3[0] = getByteArray(Properties.Resources.rFont_05);
            rFont3[1] = getByteArray(Properties.Resources.rFont_06);

            rKBD = new byte[2][];
            rKBD[0] = getByteArray(Properties.Resources.rKBD_01);
            rKBD[1] = getByteArray(Properties.Resources.rKBD_02);

            rMenuButtons = new byte[2][];
            rMenuButtons[0] = getByteArray(Properties.Resources.rMenuButtons_01);
            rMenuButtons[1] = getByteArray(Properties.Resources.rMenuButtons_02);

            rPan = new byte[2][];
            rPan[0] = getByteArray(Properties.Resources.rPan_01);
            rPan[1] = getByteArray(Properties.Resources.rPan_02);

            rPan2 = new byte[2][];
            rPan2[0] = getByteArray(Properties.Resources.rPan2_01);
            rPan2[1] = getByteArray(Properties.Resources.rPan2_02);

            rPSGEnv = getByteArray(Properties.Resources.rPSGEnv);

            rPSGMode = new byte[4][];
            rPSGMode[0] = getByteArray(Properties.Resources.rPSGMode_01);
            rPSGMode[1] = getByteArray(Properties.Resources.rPSGMode_02);
            rPSGMode[2] = getByteArray(Properties.Resources.rPSGMode_03);
            rPSGMode[3] = getByteArray(Properties.Resources.rPSGMode_04);

            rType = new byte[4][];
            rType[0] = getByteArray(Properties.Resources.rType_01);
            rType[1] = getByteArray(Properties.Resources.rType_02);
            rType[2] = getByteArray(Properties.Resources.rType_03);
            rType[3] = getByteArray(Properties.Resources.rType_04);

            rVol = new byte[2][];
            rVol[0] = getByteArray(Properties.Resources.rVol_01);
            rVol[1] = getByteArray(Properties.Resources.rVol_02);

            rWavGraph = getByteArray(Properties.Resources.rWavGraph);
            rFader = getByteArray(Properties.Resources.rFader);

            rMIDILCD_Fader = new byte[3][];
            rMIDILCD_Fader[0] = getByteArray(Properties.Resources.rMIDILCD_Fader_01);
            rMIDILCD_Fader[1] = getByteArray(Properties.Resources.rMIDILCD_Fader_02);
            rMIDILCD_Fader[2] = getByteArray(Properties.Resources.rMIDILCD_Fader_03);

            rMIDILCD_KBD = getByteArray(Properties.Resources.rMIDILCD_KBD_01);

            rMIDILCD_Vol = new byte[3][];
            rMIDILCD_Vol[0] = getByteArray(Properties.Resources.rMIDILCD_Vol_01);
            rMIDILCD_Vol[1] = getByteArray(Properties.Resources.rMIDILCD_Vol_02);
            rMIDILCD_Vol[2] = getByteArray(Properties.Resources.rMIDILCD_Vol_03);

            rMIDILCD = new byte[3][];
            rMIDILCD[0] = getByteArray(Properties.Resources.rMIDILCD_01);
            rMIDILCD[1] = getByteArray(Properties.Resources.rMIDILCD_02);
            rMIDILCD[2] = getByteArray(Properties.Resources.rMIDILCD_03);

            rMIDILCD_Font = new byte[3][];
            rMIDILCD_Font[0] = getByteArray(Properties.Resources.rMIDILCD_Font_01);
            rMIDILCD_Font[1] = getByteArray(Properties.Resources.rMIDILCD_Font_02);
            rMIDILCD_Font[2] = getByteArray(Properties.Resources.rMIDILCD_Font_03);

            rPlane_MIDI = new byte[3][];
            rPlane_MIDI[0] = getByteArray(Properties.Resources.planeMIDI_GM);
            rPlane_MIDI[1] = getByteArray(Properties.Resources.planeMIDI_XG);
            rPlane_MIDI[2] = getByteArray(Properties.Resources.planeMIDI_GS);

            bitmapMIDILyric = new Bitmap[2];
            bitmapMIDILyric[0] = new Bitmap(232, 24);
            bitmapMIDILyric[1] = new Bitmap(232, 24);
            gMIDILyric = new Graphics[2];
            gMIDILyric[0] = Graphics.FromImage(bitmapMIDILyric[0]);
            gMIDILyric[1] = Graphics.FromImage(bitmapMIDILyric[1]);
            fntMIDILyric = new Font[2];
            fntMIDILyric[0] = new Font("MS UI Gothic", 8);//, FontStyle.Bold);
            fntMIDILyric[1] = new Font("MS UI Gothic", 8);//, FontStyle.Bold);
        }

        public void AddYM2608(int chipID, PictureBox pbYM2608Screen, Image initialYM2608Image)
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

        public void AddYM2413(int chipID, PictureBox pbYM2413Screen, Image initialYM2413Image)
        {
            ym2413Screen[chipID] = new FrameBuffer();
            ym2413Screen[chipID].Add(pbYM2413Screen, initialYM2413Image, this.Paint, setting.other.Zoom);
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

   
        public void AddYM2612MIDI(PictureBox pbYM2612MIDIScreen, Image initialYM2612MIDIImage)
        {
            ym2612MIDIScreen = new FrameBuffer();
            ym2612MIDIScreen.Add(pbYM2612MIDIScreen, initialYM2612MIDIImage, this.Paint, setting.other.Zoom);
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

        public void RemoveYM2413(int chipID)
        {
            if (ym2413Screen[chipID] == null) return;
            ym2413Screen[chipID].Remove(this.Paint);
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

        public void RemoveYM2612MIDI()
        {
            if (ym2612MIDIScreen == null) return;
            ym2612MIDIScreen.Remove(this.Paint);
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
                if (ym2151Screen[chipID] != null) ym2151Screen[chipID].Remove(this.Paint);
                if (ym2203Screen[chipID] != null) ym2203Screen[chipID].Remove(this.Paint);
                if (ym2413Screen[chipID] != null) ym2413Screen[chipID].Remove(this.Paint);
                if (ym2608Screen[chipID] != null) ym2608Screen[chipID].Remove(this.Paint);
                if (ym2610Screen[chipID] != null) ym2610Screen[chipID].Remove(this.Paint);
                if (ym2612Screen[chipID] != null) ym2612Screen[chipID].Remove(this.Paint);
                if (OKIM6258Screen[chipID] != null) OKIM6258Screen[chipID].Remove(this.Paint);
                if (OKIM6295Screen[chipID] != null) OKIM6295Screen[chipID].Remove(this.Paint);
                if (SN76489Screen[chipID] != null) SN76489Screen[chipID].Remove(this.Paint);
                if (SegaPCMScreen[chipID] != null) SegaPCMScreen[chipID].Remove(this.Paint);
                
                if (fntMIDILyric!=null && fntMIDILyric[chipID] != null) fntMIDILyric[chipID].Dispose();
                if (gMIDILyric!=null && gMIDILyric[chipID] != null) gMIDILyric[chipID].Dispose();
                if (bitmapMIDILyric != null && bitmapMIDILyric[chipID] != null) bitmapMIDILyric[chipID].Dispose();
            }
            if (ym2612MIDIScreen != null) ym2612MIDIScreen.Remove(this.Paint);

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
                            ym2151Screen[chipID] = null;
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
                            ym2203Screen[chipID] = null;
                        }
                    }

                    if (ym2413Screen[chipID] != null)
                    {
                        try
                        {
                            ym2413Screen[chipID].Refresh(this.Paint);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            RemoveYM2413(chipID);
                            ym2413Screen[chipID] = null;
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
                            ym2608Screen[chipID] = null;
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
                            ym2610Screen[chipID] = null;
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
                            ym2612Screen[chipID] = null;
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
                            OKIM6258Screen[chipID] = null;
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
                            OKIM6295Screen[chipID] = null;
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
                            SN76489Screen[chipID] = null;
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
                            SegaPCMScreen[chipID] = null;
                        }
                    }

                }

                if (ym2612MIDIScreen != null)
                {
                    try
                    {
                        ym2612MIDIScreen.Refresh(this.Paint);
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                        RemoveYM2612MIDI();
                        ym2612MIDIScreen = null;
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
                screen.drawByteArray(x, y, rFont1[t], 128, (cd % 16) * 8, (cd / 16) * 8, 8, 8);
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
                    screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 8;
                if (n != 0 || f)
                {
                    screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
                }

                n = num / 1;
                num -= n * 1;
                x += 8;
                screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
            }
            else
            {
                screen.drawByteArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
            }

            n = num / 1;
            num -= n * 1;
            x += 8;
            screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
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
                if (n == 0) screen.drawByteArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
                else screen.drawByteArray(x, y, rFont1[t], 128, 0, 8, 8, 8);

                n = num / 10;
                num -= n * 10;
                x += 8;
                screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);

                n = num / 1;
                x += 8;
                screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);

            n = num / 1;
            x += 8;
            screen.drawByteArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
        }

        public void drawFont4(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null) return;

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawByteArray(x, y, rFont2[t], 128, (cd % 32) * 4, (cd / 32) * 8, 4, 8);
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
                    screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, 0, 0, 4, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, 0, 0, 4, 8);
                }

                n = num / 1;
                x += 4;
                screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
            }
            else
            {
                screen.drawByteArray(x, y, rFont2[t], 128, 0, 0, 4, 8);
            }

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
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
                screen.drawByteArray(x, y, rFont2[t], 128, 0, 0, 4, 8);

                n = num / 10;
                num -= n * 10;
                x += 4;
                screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

                n = num / 1;
                x += 4;
                screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
        }



        public void drawFont4V(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null) return;

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawByteArray(x, y, rFont3[t], 128, (cd % 16) * 8, (cd / 16) * 4, 8, 4);
                y -= 4;
            }
        }



        private void drawVolumeP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rVol[tp], 32, 2 * t, 0, 2, 8 - (t / 4) * 4);
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
                    screen.drawByteArray(x, y, rKBD[tp], 32, 0, 0, 4, 8);
                    break;
                case 1:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 4, 0, 3, 8);
                    break;
                case 2:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 8, 0, 4, 8);
                    break;
                case 3:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 12, 0, 4, 8);
                    break;
                case 4:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 0 + 16, 0, 4, 8);
                    break;
                case 5:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 4 + 16, 0, 3, 8);
                    break;
                case 6:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 8 + 16, 0, 4, 8);
                    break;
                case 7:
                    screen.drawByteArray(x, y, rKBD[tp], 32, 12 + 16, 0, 4, 8);
                    break;
            }
        }

        private void drawPanP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rPan[tp], 32, 8 * t, 0, 8, 8);
        }

        private void drawTnP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rPSGMode[tp], 32, 8 * t, 0, 8, 8);
        }

        private void drawEtypeP(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rPSGEnv, 64, 8 * t, 0, 8, 8);
        }

        private void drawPanType2P(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null)
            {
                return;
            }

            int p = (t & 0x0f);
            p = p == 0 ? 0 : (1 + p / 4);
            screen.drawByteArray(x, y, rPan2[0], 32, p * 4, 0, 4, 8);
            p = ((t & 0xf0) >> 4);
            p = p == 0 ? 0 : (1 + p / 4);
            screen.drawByteArray(x + 4, y, rPan2[0], 32, p * 4, 0, 4, 8);

        }

        private void drawChipNameP(FrameBuffer screen, int x, int y, int t, int c)
        {
            if (screen == null)
            {
                return;
            }

            screen.drawByteArray(x, y, rChipName[c], 128
                , (t % 8) * 16
                , (t / 8) * 8
                , 8 * 2
                , 8);

        }

        public void drawButtonP(int x, int y, int t, int m)
        {
            if (mainScreen == null) return;

            int n = t % 18;
            t /= 18;
            switch (n)
            {
                case 0:
                    //setting
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 5 * 16, 1 * 16, 16, 16);
                    break;
                case 1:
                    //stop
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 0 * 16, 0 * 16, 16, 16);
                    break;
                case 2:
                    //pause
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 1 * 16, 0 * 16, 16, 16);
                    break;
                case 3:
                    //fadeout
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 4 * 16, 1 * 16, 16, 16);
                    break;
                case 4:
                    //PREV
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 6 * 16, 1 * 16, 16, 16);
                    break;
                case 5:
                    //slow
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 2 * 16, 0 * 16, 16, 16);
                    break;
                case 6:
                    //play
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 3 * 16, 0 * 16, 16, 16);
                    break;
                case 7:
                    //fast
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 4 * 16, 0 * 16, 16, 16);
                    break;
                case 8:
                    //NEXT
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 7 * 16, 1 * 16, 16, 16);
                    break;
                case 9:
                    //loopmode
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 1 * 16 + m * 16, 2 * 16, 16, 16);
                    break;
                case 10:
                    //folder
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 5 * 16, 0 * 16, 16, 16);
                    break;
                case 11:
                    //List
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 0 * 16, 2 * 16, 16, 16);
                    break;
                case 12:
                    //info
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 0 * 16, 1 * 16, 16, 16);
                    break;
                case 13:
                    //mixer
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 2 * 16, 1 * 16, 16, 16);
                    break;
                case 14:
                    //panel
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 5 * 16, 2 * 16, 16, 16);
                    break;
                case 15:
                    //VST List
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 7 * 16, 0 * 16, 16, 16);
                    break;
                case 16:
                    //MIDI Keyboard
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 3 * 16, 1 * 16, 16, 16);
                    break;
                case 17:
                    //zoom
                    mainScreen.drawByteArray(x, y, rMenuButtons[t], 128, 6 * 16, 2 * 16, 16, 16);
                    break;
            }
        }

        public void drawToneFormatP(FrameBuffer screen, int x, int y, int toneFormat)
        {
            screen.drawByteArray(x, y, rMenuButtons[1], 128, (toneFormat % 3) *5* 8, (6 + toneFormat / 3) * 8, 40, 8);
        }

        public void drawChPYM2612(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ch == 5)
            {
                return;
            }

            if (ch < 5)
            {
                ym2612Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
            }
            else if (ch < 10)
            {
                ym2612Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 0, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, (ch - 5).ToString());
            }
        }

        public void drawCh6PYM2612(int chipID, int x, int y, int m, bool mask, int tp)
        {
            if (m == 0)
            {
                ym2612Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                ym2612Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, 0, " ");
            }
        }

        public void drawCh6PYM2612XGM(int chipID, int x, int y, int m, bool mask, int tp)
        {
            if (m == 0)
            {
                //FM mode

                ym2612Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, mask ? 1 : 0, "6");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2612Screen[chipID], 32 + kx, y, kt, tp);
                }
            }
            else
            {
                //PCM mode

                ym2612Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                drawFont8(ym2612Screen[chipID], x + 16, y, 0, " ");
                drawFont4(ym2612Screen[chipID], x + 32, y, 0, " 1C00             2C00             3C00             4C00                ");
            }
        }

        public void drawChPYM2151(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2151Screen[chipID] == null) return;

            ym2151Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
            drawFont8(ym2151Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public void drawChPYM2608(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2608Screen[chipID] == null) return;

            if (ch < 6)
            {
                ym2608Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2608Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                ym2608Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
                drawFont8(ym2608Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                ym2608Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 0, 16, 8);
                drawFont8(ym2608Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 9).ToString());
            }
            else
            {
                ym2608Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 0, 24, 8);
            }
        }

        public void drawChPYM2608Rhythm(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2608Screen[chipID] == null) return;

            drawFont4(ym2608Screen[chipID], x + 1 * 4, y, mask ? 1 : 0, "B");
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
                ym2610Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2610Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                ym2610Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
                drawFont8(ym2610Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                ym2610Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 0, 16, 8);
                drawFont8(ym2610Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 9).ToString());
            }
            else
            {
                ym2610Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 32, 24, 8);
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
                ym2203Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2203Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 6)
            {
                ym2203Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
                drawFont8(ym2203Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 3).ToString());
            }
            else if (ch < 9)
            {
                ym2203Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 0, 16, 8);
                drawFont8(ym2203Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else
            {
                ym2203Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 24, 8);
            }
        }

        public void drawChPYM2413(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (ym2413Screen[chipID] == null) return;

            if (ch < 9)
            {
                ym2413Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(ym2413Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else
            {
                switch (ch)
                {
                    case 9:
                        drawFont4(ym2413Screen[chipID], (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 10:
                        drawFont4(ym2413Screen[chipID], (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 11:
                        drawFont4(ym2413Screen[chipID], (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 12:
                        drawFont4(ym2413Screen[chipID], (ch - 9) * 4 * 15 + 3 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 13:
                        drawFont4(ym2413Screen[chipID], (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
        }

        public void drawChPSN76489(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (SN76489Screen[chipID] == null) return;

            SN76489Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
            drawFont8(SN76489Screen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public void drawChPSegaPCM(int chipID, int x, int y, int ch, bool mask, int tp)
        {
            if (SegaPCMScreen[chipID] == null) return;

            SegaPCMScreen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            if (ch < 9) drawFont8(SegaPCMScreen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            else drawFont4(SegaPCMScreen[chipID], x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }


        public void drawVolume(FrameBuffer screen, int y, int c, ref int ov, int nv, int tp)
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

        public void drawVolumeXY(FrameBuffer screen, int x, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }

            y *= 4;
            x *= 4;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(screen, x + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(screen, x + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public void drawVolumeYM2608(int chipID, int y, int c, ref int ov, int nv, int tp)
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

        public void drawVolumeYM2608Rhythm(int chipID, int x, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            x = x * 4 * 13 + 8 * 2;

            for (int i = 0; i <= 19; i++)
            {
                drawVolumeP(ym2608Screen[chipID], x + i * 2, sy + 8 * 14, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                drawVolumeP(ym2608Screen[chipID], x + i * 2, sy + 8 * 14, i > 17 ? (2 + t) : (0 + t), tp);
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

        public void drawChYM2612(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2612(chipID, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public void drawCh6YM2612(int chipID, ref int ot, int nt, ref bool om, bool nm, ref int otp, int ntp)
        {

            if (ot == nt && om == nm && otp == ntp)
            {
                return;
            }

            drawCh6PYM2612(chipID, 0, 48, nt, nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public void drawCh6YM2612XGM(int chipID, ref int ot, int nt, ref bool om, bool nm, ref int otp, int ntp)
        {

            if (ot == nt && om == nm && otp == ntp)
            {
                return;
            }

            drawCh6PYM2612XGM(chipID, 0, 48, nt, nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public void drawChYM2151(int chipID, int ch, ref bool om, bool nm, int tp)
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

        public void drawChYM2413(int chipID, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPYM2413(chipID, 0, ch < 9 ? (8 + ch * 8) : (8 + 9 * 8), ch, nm, tp);
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


        public void drawTn(FrameBuffer screen, int x, int y, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawTnP(screen, x * 4, y * 4 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public void drawNfrq(FrameBuffer screen, int x, int y, ref int onfrq, int nnfrq)
        {
            if (onfrq == nnfrq)
            {
                return;
            }

            x *= 4;
            y *= 4;
            drawFont4Int(screen, x, y, 0, 2, nnfrq);

            onfrq = nnfrq;
        }

        public void drawEfrq(FrameBuffer screen, int x, int y, ref int oefrq, int nefrq)
        {
            if (oefrq == nefrq)
            {
                return;
            }

            x *= 4;
            y *= 4;
            drawFont4(screen, x, y, 0, string.Format("{0:D5}", nefrq));

            oefrq = nefrq;
        }

        public void drawEtype(FrameBuffer screen, int x, int y, ref int oetype, int netype)
        {
            if (oetype == netype)
            {
                return;
            }

            x *= 4;
            y *= 4;

            drawEtypeP(screen, x, y, netype);
            oetype = netype;
        }

        public void drawLfoSw(FrameBuffer screen, int x, int y, ref bool olfosw, bool nlfosw)
        {
            if (olfosw == nlfosw)
            {
                return;
            }

            x *= 4;
            y *= 4;
            drawFont4(screen, x, y, 0, nlfosw ? "ON " : "OFF");

            olfosw = nlfosw;
        }

        public void drawLfoFrq(FrameBuffer screen, int x, int y, ref int olfofrq, int nlfofrq)
        {
            if (olfofrq == nlfofrq)
            {
                return;
            }

            x *= 4;
            y *= 4;
            drawFont4Int(screen, x, y, 0, 1, nlfofrq);

            olfofrq = nlfofrq;
        }

        public void drawToneFormat(FrameBuffer screen, int x, int y, ref int oToneFormat, int nToneFormat)
        {
            if (oToneFormat == nToneFormat)
            {
                return;
            }

            x *= 4;
            y *= 4;

            drawToneFormatP(screen, x, y, nToneFormat);

            oToneFormat = nToneFormat;
        }

        public void drawPanOKIM6258(int chipID, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(OKIM6258Screen[chipID], 24, 8, nt, ntp);
            ot = nt;
            otp = ntp;
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

        public void drawPanYM2608Rhythm(int chipID, int c, ref int ot, int nt, ref int otp, int ntp)
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

        public void drawKfYM2151(int chipID, int ch, ref int ok, int nk)
        {
            if (ok == nk)
            {
                return;
            }

            int x = (ch % 4) * 4 * 3 + 4 * 67;
            int y = (ch / 4) * 8 + 8 * 22;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 2, nk);
            ok = nk;
        }

        public void drawNeYM2151(int chipID, ref int one, int nne)
        {
            if (one == nne)
            {
                return;
            }

            int x = 4 * 60;
            int y = 8 * 22;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 1, nne);

            one = nne;
        }

        public void drawNfrqYM2151(int chipID, ref int onfrq, int nnfrq)
        {
            if (onfrq == nnfrq)
            {
                return;
            }

            int x = 4 * 60;
            int y = 8 * 23;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 2, nnfrq);

            onfrq = nnfrq;
        }

        public void drawLfrqYM2151(int chipID, ref int olfrq, int nlfrq)
        {
            if (olfrq == nlfrq)
            {
                return;
            }

            int x = 4 * 59;
            int y = 8 * 24;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 3, nlfrq);

            olfrq = nlfrq;
        }

        public void drawAmdYM2151(int chipID, ref int oamd, int namd)
        {
            if (oamd == namd)
            {
                return;
            }

            int x = 4 * 59;
            int y = 8 * 26;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 3, namd);

            oamd = namd;
        }

        public void drawPmdYM2151(int chipID, ref int opmd, int npmd)
        {
            if (opmd == npmd)
            {
                return;
            }

            int x = 4 * 59;
            int y = 8 * 25;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 3, npmd);

            opmd = npmd;
        }

        public void drawWaveFormYM2151(int chipID, ref int owaveform, int nwaveform)
        {
            if (owaveform == nwaveform)
            {
                return;
            }

            int x = 4 * 68;
            int y = 8 * 24;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 1, nwaveform);

            owaveform = nwaveform;
        }

        public void drawLfoSyncYM2151(int chipID, ref int olfosync, int nlfosync)
        {
            if (olfosync == nlfosync)
            {
                return;
            }

            int x = 4 * 68;
            int y = 8 * 25;
            drawFont4Int(ym2151Screen[chipID], x, y, 0, 1, nlfosync);

            olfosync = nlfosync;
        }


        public void drawButton(int c, ref int ot, int nt, ref int om, int nm)
        {
            if (ot == nt && om == nm)
            {
                return;
            }

            drawFont8(mainScreen, 32 + c * 16, 24, 0, "  ");
            drawFont8(mainScreen, 32 + c * 16, 32, 0, "  ");
            drawButtonP(32 + c * 16, 24, nt * 18 + c, nm);

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

        public void drawKbYM2608(int chipID, int y, ref int ot, int nt, int tp)
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

        public void drawChipName(int x, int y, int t, ref byte oc, byte nc)
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

        public void drawInst(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni, int[] ot, int[] nt)
        {
            int sx = (c % 3) * 8 * 13 + x * 8;
            int sy = (c / 3) * 8 * 6 + 8 * y;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (oi[i + j * 11] != ni[i + j * 11] || ot[i + j * 11] != nt[i + j * 11])
                    {
                        drawFont4Int(screen, sx + i * 8 + (i > 5 ? 4 : 0), sy + j * 8, nt[i + j * 11], (i == 5) ? 3 : 2, ni[i + j * 11]);
                        oi[i + j * 11] = ni[i + j * 11];
                        ot[i + j * 11] = nt[i + j * 11];
                    }
                }
            }

            if (oi[44] != ni[44] || ot[44] != nt[44])
            {
                drawFont4Int(screen, sx + 8 * 4, sy - 16, nt[44], 2, ni[44]);
                oi[44] = ni[44];
                ot[44] = nt[44];
            }
            if (oi[45] != ni[45] || ot[45] != nt[45])
            {
                drawFont4Int(screen, sx + 8 * 6, sy - 16, nt[45], 2, ni[45]);
                oi[45] = ni[45];
                ot[45] = nt[45];
            }
            if (oi[46] != ni[46] || ot[46] != nt[46])
            {
                drawFont4Int(screen, sx + 8 * 8 + 4, sy - 16, nt[46], 2, ni[46]);
                oi[46] = ni[46];
                ot[46] = nt[46];
            }
            if (oi[47] != ni[47] || ot[47] != nt[47])
            {
                drawFont4Int(screen, sx + 8 * 11, sy - 16, nt[47], 2, ni[47]);
                oi[47] = ni[47];
                ot[47] = nt[47];
            }
        }

        public void drawInstNumber(FrameBuffer screen, int x, int y, ref int oi, int ni)
        {
            if (oi != ni)
            {
                drawFont4Int(screen, x * 4, y * 4, 0, 2, ni);
                oi = ni;
            }
        }

        public void drawSUSFlag(FrameBuffer screen, int x, int y, ref int oi, int ni)
        {
            if (oi != ni)
            {
                drawFont4(screen, x * 4, y * 4, 0, ni == 0 ? "-" : "*");
                oi = ni;
            }
        }


        public void drawTimer(int c, ref int ot1, ref int ot2, ref int ot3, int nt1, int nt2, int nt3)
        {
            if (ot1 != nt1)
            {
                //drawFont4Int2(mainScreen, 4 * 30 + c * 4 * 11, 0, 0, 3, nt1);
                drawFont8Int2(mainScreen, 8 * 3 + c * 8 * 11, 16, 0, 3, nt1);
                ot1 = nt1;
            }
            if (ot2 != nt2)
            {
                drawFont8Int2(mainScreen, 8 * 7 + c * 8 * 11, 16, 0, 2, nt2);
                //drawFont4Int2(mainScreen, 4 * 34 + c * 4 * 11, 0, 0, 2, nt2);
                ot2 = nt2;
            }
            if (ot3 != nt3)
            {
                drawFont8Int2(mainScreen, 8 * 10 + c * 8 * 11, 16, 0, 2, nt3);
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

                if (SegaPCMScreen[chipID] != null) drawParamsToSegaPCM(oldParam, newParam, chipID);

                if (ym2151Screen[chipID] != null) drawParamsToYM2151(oldParam, newParam, chipID);

                if (ym2203Screen[chipID] != null) drawParamsToYM2203(oldParam, newParam, chipID);

                if (ym2413Screen[chipID] != null) drawParamsToYM2413(oldParam, newParam, chipID);

                if (ym2608Screen[chipID] != null) drawParamsToYM2608(oldParam, newParam, chipID);

                if (ym2610Screen[chipID] != null) drawParamsToYM2610(oldParam, newParam, chipID);

                if (OKIM6258Screen[chipID] != null) drawParamsToOKIM6258(oldParam, newParam, chipID);
            }

            if (ym2612MIDIScreen != null) drawParamsToYM2612MIDI(oldParam, newParam);

        }

        private void drawParamsToOKIM6258(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            MDChipParams.OKIM6258 ost = oldParam.okim6258[chipID];
            MDChipParams.OKIM6258 nst = newParam.okim6258[chipID];

            drawPanOKIM6258(chipID, ref ost.pan, nst.pan, ref ost.pantp, 0);

            if (ost.masterFreq != nst.masterFreq)
            {
                drawFont4(OKIM6258Screen[chipID], 12 * 4, 8, 0, string.Format("{0:d5}", nst.masterFreq));
                ost.masterFreq = nst.masterFreq;
            }

            if (ost.divider != nst.divider)
            {
                drawFont4(OKIM6258Screen[chipID], 19 * 4, 8, 0, string.Format("{0:d5}", nst.divider));
                ost.divider = nst.divider;
            }

            if (ost.pbFreq != nst.pbFreq)
            {
                drawFont4(OKIM6258Screen[chipID], 26 * 4, 8, 0, string.Format("{0:d5}", nst.pbFreq));
                ost.pbFreq = nst.pbFreq;
            }

            drawVolume(OKIM6258Screen[chipID], 0, 1, ref ost.volumeL, nst.volumeL/2, 0);
            drawVolume(OKIM6258Screen[chipID], 0, 2, ref ost.volumeR, nst.volumeR/2, 0);

            drawChOKIM6258(chipID, ref ost.mask, nst.mask, 0);

        }

        public void drawChOKIM6258(int chipID, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            drawChPOKIM6258(chipID, 0, 8 + 0 * 8, nm, tp);
            om = nm;
        }

        public void drawChPOKIM6258(int chipID, int x, int y, bool mask, int tp)
        {
            if (OKIM6258Screen[chipID] == null) return;

            OKIM6258Screen[chipID].drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 8*8, 0, 24, 8);
        }


        private void drawParamsToYM2151(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 8; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2151[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2151[chipID].channels[c];

                int tp = ((chipID == 0) ? setting.YM2151Type.UseScci : setting.YM2151SType.UseScci) ? 1 : 0;

                drawInst(ym2151Screen[chipID], 1, 11, c, oyc.inst, nyc.inst);

                drawPanYM2151(chipID, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                drawKb(ym2151Screen[chipID], c, ref oyc.note, nyc.note, tp);

                drawVolume(ym2151Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                drawVolume(ym2151Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp);

                drawChYM2151(chipID, c, ref oyc.mask, nyc.mask, tp);

                drawKfYM2151(chipID, c, ref oyc.kf, nyc.kf);
            }

            drawNeYM2151(chipID, ref oldParam.ym2151[chipID].ne, newParam.ym2151[chipID].ne);
            drawNfrqYM2151(chipID, ref oldParam.ym2151[chipID].nfrq, newParam.ym2151[chipID].nfrq);
            drawLfrqYM2151(chipID, ref oldParam.ym2151[chipID].lfrq, newParam.ym2151[chipID].lfrq);
            drawAmdYM2151(chipID, ref oldParam.ym2151[chipID].amd, newParam.ym2151[chipID].amd);
            drawPmdYM2151(chipID, ref oldParam.ym2151[chipID].pmd, newParam.ym2151[chipID].pmd);
            drawWaveFormYM2151(chipID, ref oldParam.ym2151[chipID].waveform, newParam.ym2151[chipID].waveform);
            drawLfoSyncYM2151(chipID, ref oldParam.ym2151[chipID].lfosync, newParam.ym2151[chipID].lfosync);

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
                    drawVolume(ym2203Screen[chipID], c + 3, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(ym2203Screen[chipID], c + 3, ref oyc.note, nyc.note, tp);
                }

                drawChYM2203(chipID, c, ref oyc.mask, nyc.mask, tp);

            }

            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.ym2203[chipID].channels[c + 6];
                MDChipParams.Channel nyc = newParam.ym2203[chipID].channels[c + 6];

                drawVolume(ym2203Screen[chipID], c + 3, 0, ref oyc.volume, nyc.volume, tp);
                drawKb(ym2203Screen[chipID], c + 3, ref oyc.note, nyc.note, tp);
                drawTn(ym2203Screen[chipID], 6, 2, c + 3, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

                drawChYM2203(chipID, c + 6, ref oyc.mask, nyc.mask, tp);

            }

            drawNfrq(ym2203Screen[chipID], 5, 32, ref oldParam.ym2203[chipID].nfrq, newParam.ym2203[chipID].nfrq);
            drawEfrq(ym2203Screen[chipID], 18, 32, ref oldParam.ym2203[chipID].efrq, newParam.ym2203[chipID].efrq);
            drawEtype(ym2203Screen[chipID], 33, 32, ref oldParam.ym2203[chipID].etype, newParam.ym2203[chipID].etype);

        }

        private void drawParamsToYM2413(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            int tp = setting.YM2413Type.UseScci ? 1 : 0;

            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int c = 0; c < 9; c++)
            {

                oyc = oldParam.ym2413[chipID].channels[c];
                nyc = newParam.ym2413[chipID].channels[c];

                drawVolume(ym2413Screen[chipID], c, 0, ref oyc.volumeL, nyc.volumeL, tp);
                drawKb(ym2413Screen[chipID], c, ref oyc.note, nyc.note, tp);

                drawInstNumber(ym2413Screen[chipID], (c % 3) * 16 + 37, (c / 3) * 2 + 24, ref oyc.inst[0], nyc.inst[0]);
                drawSUSFlag(ym2413Screen[chipID], (c % 3) * 16 + 41, (c / 3) * 2 + 24, ref oyc.inst[1], nyc.inst[1]);
                drawSUSFlag(ym2413Screen[chipID], (c % 3) * 16 + 44, (c / 3) * 2 + 24, ref oyc.inst[2], nyc.inst[2]);
                drawInstNumber(ym2413Screen[chipID], (c % 3) * 16 + 46, (c / 3) * 2 + 24, ref oyc.inst[3], nyc.inst[3]);

                drawChYM2413(chipID, c , ref oyc.mask, nyc.mask, tp);

            }

            drawChYM2413(chipID, 9, ref oldParam.ym2413[chipID].channels[9].mask, newParam.ym2413[chipID].channels[9].mask, tp);
            drawChYM2413(chipID, 10, ref oldParam.ym2413[chipID].channels[10].mask, newParam.ym2413[chipID].channels[10].mask, tp);
            drawChYM2413(chipID, 11, ref oldParam.ym2413[chipID].channels[11].mask, newParam.ym2413[chipID].channels[11].mask, tp);
            drawChYM2413(chipID, 12, ref oldParam.ym2413[chipID].channels[12].mask, newParam.ym2413[chipID].channels[12].mask, tp);
            drawChYM2413(chipID, 13, ref oldParam.ym2413[chipID].channels[13].mask, newParam.ym2413[chipID].channels[13].mask, tp);
            drawVolumeXY(ym2413Screen[chipID], 6, 20, 0, ref oldParam.ym2413[chipID].channels[9].volume, newParam.ym2413[chipID].channels[9].volume, tp);
            drawVolumeXY(ym2413Screen[chipID], 21, 20, 0, ref oldParam.ym2413[chipID].channels[10].volume, newParam.ym2413[chipID].channels[10].volume, tp);
            drawVolumeXY(ym2413Screen[chipID], 36, 20, 0, ref oldParam.ym2413[chipID].channels[11].volume, newParam.ym2413[chipID].channels[11].volume, tp);
            drawVolumeXY(ym2413Screen[chipID], 51, 20, 0, ref oldParam.ym2413[chipID].channels[12].volume, newParam.ym2413[chipID].channels[12].volume, tp);
            drawVolumeXY(ym2413Screen[chipID], 66, 20, 0, ref oldParam.ym2413[chipID].channels[13].volume, newParam.ym2413[chipID].channels[13].volume, tp);

            oyc = oldParam.ym2413[chipID].channels[0];
            nyc = newParam.ym2413[chipID].channels[0];
            drawInstNumber(ym2413Screen[chipID], 9, 22, ref oyc.inst[4], nyc.inst[4]); //TL
            drawInstNumber(ym2413Screen[chipID], 14, 22, ref oyc.inst[5], nyc.inst[5]); //FB

            for (int c = 0; c < 11; c++)
            {
                drawInstNumber(ym2413Screen[chipID], c * 3, 26, ref oyc.inst[6 + c], nyc.inst[6 + c]);
                drawInstNumber(ym2413Screen[chipID], c * 3, 28, ref oyc.inst[17 + c], nyc.inst[17 + c]);
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
                drawTn(ym2608Screen[chipID], 6, 2, c + 6, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

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

            drawLfoSw(ym2608Screen[chipID], 4, 54, ref oldParam.ym2608[chipID].lfoSw, newParam.ym2608[chipID].lfoSw);
            drawLfoFrq(ym2608Screen[chipID], 16, 54, ref oldParam.ym2608[chipID].lfoFrq, newParam.ym2608[chipID].lfoFrq);

            drawNfrq(ym2608Screen[chipID], 25, 54, ref oldParam.ym2608[chipID].nfrq, newParam.ym2608[chipID].nfrq);
            drawEfrq(ym2608Screen[chipID], 38, 54, ref oldParam.ym2608[chipID].efrq, newParam.ym2608[chipID].efrq);
            drawEtype(ym2608Screen[chipID], 53, 54, ref oldParam.ym2608[chipID].etype, newParam.ym2608[chipID].etype);

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
                drawTn(ym2610Screen[chipID], 6, 2, c + 6, ref oyc.tn, nyc.tn, ref oyc.tntp, tp);

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

            drawLfoSw(ym2610Screen[chipID], 4, 54, ref oldParam.ym2610[chipID].lfoSw, newParam.ym2610[chipID].lfoSw);
            drawLfoFrq(ym2610Screen[chipID], 16, 54, ref oldParam.ym2610[chipID].lfoFrq, newParam.ym2610[chipID].lfoFrq);

            drawNfrq(ym2610Screen[chipID], 25, 54, ref oldParam.ym2610[chipID].nfrq, newParam.ym2610[chipID].nfrq);
            drawEfrq(ym2610Screen[chipID], 38, 54, ref oldParam.ym2610[chipID].efrq, newParam.ym2610[chipID].efrq);
            drawEtype(ym2610Screen[chipID], 53, 54, ref oldParam.ym2610[chipID].etype, newParam.ym2610[chipID].etype);
        }

        private void drawParamsToSegaPCM(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 16; c++)
            {

                MDChipParams.Channel orc = oldParam.segaPcm[chipID].channels[c];
                MDChipParams.Channel nrc = newParam.segaPcm[chipID].channels[c];

                drawVolume(SegaPCMScreen[chipID], c, 1, ref orc.volumeL, nrc.volumeL, 0);
                drawVolume(SegaPCMScreen[chipID], c, 2, ref orc.volumeR, nrc.volumeR, 0);
                drawKb(SegaPCMScreen[chipID], c, ref orc.note, nrc.note, 0);
                drawPanType2(SegaPCMScreen[chipID], c, ref orc.pan, nrc.pan);

                drawChSegaPCM(chipID, c, ref orc.mask, nrc.mask, 0);
            }
        }

        private void drawParamsToSN76489(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            bool SN76489Type = (chipID == 0) ? setting.SN76489Type.UseScci : setting.SN76489SType.UseScci;
            int tp = SN76489Type ? 1 : 0;
            MDChipParams.Channel osc;
            MDChipParams.Channel nsc;

            for (int c = 0; c < 3; c++)
            {
                osc = oldParam.sn76489[chipID].channels[c];
                nsc = newParam.sn76489[chipID].channels[c];

                drawVolume(SN76489Screen[chipID], c, 0, ref osc.volume, nsc.volume, tp);
                drawKb(SN76489Screen[chipID], c, ref osc.note, nsc.note, tp);
                drawChSN76489(chipID, c, ref osc.mask, nsc.mask, tp);
            }

            osc = oldParam.sn76489[chipID].channels[3];
            nsc = newParam.sn76489[chipID].channels[3];
            drawVolume(SN76489Screen[chipID], 3, 0, ref osc.volume, nsc.volume, tp);
            drawChSN76489(chipID, 3, ref osc.mask, nsc.mask, tp);
            drawChSN76489Noise(chipID, ref osc, nsc, tp);

        }

        private void drawChSN76489Noise(int chipID, ref MDChipParams.Channel osc, MDChipParams.Channel nsc, int tp)
        {
            if (osc.note == nsc.note) return;

            drawFont4(SN76489Screen[chipID], 56, 32, tp, (nsc.note & 0x4) != 0 ? "WHITE   " : "PERIODIC");
            drawFont4(SN76489Screen[chipID], 120, 32, tp, (nsc.note & 0x3) == 0 ? "0  " : ((nsc.note & 0x3) == 1 ? "1  " : ((nsc.note & 0x3) == 2 ? "2  " : "CH3")));

            osc.note = nsc.note;
        }


        private void drawParamsToYM2612(MDChipParams oldParam, MDChipParams newParam, int chipID)
        {
            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2612[chipID].channels[c];
                MDChipParams.Channel nyc = newParam.ym2612[chipID].channels[c];

                bool YM2612type = (chipID == 0) ? setting.YM2612Type.UseScci : setting.YM2612SType.UseScci;
                int tp = YM2612type ? 1 : 0;

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

                    drawPan(ym2612Screen[chipID], c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp6v);
                    drawInst(ym2612Screen[chipID], 1, 12, c, oyc.inst, nyc.inst);

                    if (newParam.fileFormat != enmFileFormat.XGM)
                    {
                        drawCh6YM2612(chipID, ref oyc.pcmMode, nyc.pcmMode, ref oyc.mask, nyc.mask, ref oyc.tp, tp6v);
                        drawVolume(ym2612Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp6v);
                        drawVolume(ym2612Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp6v);
                        drawKb(ym2612Screen[chipID], c, ref oyc.note, nyc.note, tp6v);
                    }
                    else
                    {
                        drawCh6YM2612XGM(chipID, ref oyc.pcmMode, nyc.pcmMode, ref oyc.mask, nyc.mask, ref oyc.tp, tp6v);
                        if (newParam.ym2612[chipID].channels[5].pcmMode == 0)
                        {
                            drawVolume(ym2612Screen[chipID], c, 1, ref oyc.volumeL, nyc.volumeL, tp6v);
                            drawVolume(ym2612Screen[chipID], c, 2, ref oyc.volumeR, nyc.volumeR, tp6v);
                            drawKb(ym2612Screen[chipID], c, ref oyc.note, nyc.note, tp6v);
                        }
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                drawVolumeXY(ym2612Screen[chipID], 13 + i * 17, 12, 1, ref oldParam.ym2612[chipID].xpcmVolL[i], newParam.ym2612[chipID].xpcmVolL[i], tp6v);
                                drawVolumeXY(ym2612Screen[chipID], 13 + i * 17, 12, 2, ref oldParam.ym2612[chipID].xpcmVolR[i], newParam.ym2612[chipID].xpcmVolR[i], tp6v);
                                if (oldParam.ym2612[chipID].xpcmInst[i] != newParam.ym2612[chipID].xpcmInst[i])
                                {
                                    drawFont4Int2(ym2612Screen[chipID], 44 + i * 17 * 4, 48, tp6v, 2, newParam.ym2612[chipID].xpcmInst[i]);
                                    oldParam.ym2612[chipID].xpcmInst[i] = newParam.ym2612[chipID].xpcmInst[i];
                                }
                            }
                        }
                    }
                }
                else
                {
                    drawVolume(ym2612Screen[chipID], c, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    drawKb(ym2612Screen[chipID], c, ref oyc.note, nyc.note, tp);
                    drawChYM2612(chipID, c, ref oyc.mask, nyc.mask, tp);
                }

            }

            drawLfoSw(ym2612Screen[chipID], 4, 44, ref oldParam.ym2612[chipID].lfoSw, newParam.ym2612[chipID].lfoSw);
            drawLfoFrq(ym2612Screen[chipID], 16, 44, ref oldParam.ym2612[chipID].lfoFrq, newParam.ym2612[chipID].lfoFrq);

        }





        private void drawParamsToYM2612MIDI(MDChipParams oldParam, MDChipParams newParam)
        {
            for (int c = 0; c < 6; c++)
            {

                MDChipParams.Channel oyc = oldParam.ym2612Midi.channels[c];
                MDChipParams.Channel nyc = newParam.ym2612Midi.channels[c];

                bool YM2612type = setting.YM2612Type.UseScci;
                int tp = YM2612type ? 1 : 0;

                drawInst(ym2612MIDIScreen, 1, 6 + (c > 2 ? 3 : 0), c, oyc.inst, nyc.inst, oyc.typ, nyc.typ);

                int[] onl = oldParam.ym2612Midi.noteLog[c];
                int[] nnl = newParam.ym2612Midi.noteLog[c];

                for (int n = 0; n < 10; n++)
                {
                    drawNoteLogYM2612MIDI((c % 3) * 13 * 8 + 2 * 8 + n * 8, (c / 3) * 18 * 4 + 24 * 4, ref onl[n], nnl[n]);
                }

                drawUseChannelYM2612MIDI((c % 3) * 13 * 8, (c / 3) * 9 * 8+4*8, ref oldParam.ym2612Midi.useChannel[c], newParam.ym2612Midi.useChannel[c]);
            }

            drawMONOPOLYYM2612MIDI(ref oldParam.ym2612Midi.IsMONO, newParam.ym2612Midi.IsMONO);

            drawLfoSw(ym2612MIDIScreen, 4, 44, ref oldParam.ym2612Midi.lfoSw, newParam.ym2612Midi.lfoSw);
            drawLfoFrq(ym2612MIDIScreen, 16, 44, ref oldParam.ym2612Midi.lfoFrq, newParam.ym2612Midi.lfoFrq);
            drawToneFormat(ym2612MIDIScreen,16,6,ref oldParam.ym2612Midi.useFormat,newParam.ym2612Midi.useFormat);
        }

        private void drawMONOPOLYYM2612MIDI(ref bool olm, bool nlm)
        {
            if (olm == nlm) return;

            drawFont8(ym2612MIDIScreen, 8,16, 1, nlm ? "^" : "-");
            drawFont8(ym2612MIDIScreen, 8,24, 1, nlm ? "-" : "^");

            olm = nlm;
        }

        private void drawUseChannelYM2612MIDI(int x, int y, ref bool olm, bool nlm)
        {
            //if (olm == nlm) return;

            drawFont8(ym2612MIDIScreen, x, y, 1, nlm ? "^" : "-");

            olm = nlm;
        }

        private void drawNoteLogYM2612MIDI(int x,int y,ref int oln,int nln)
        {
            if (oln == nln) return;
            if (nln == -1)
            {
                drawFont4V(ym2612MIDIScreen, x, y, 0, "   ");
            }
            else
            {
                drawFont4V(ym2612MIDIScreen, x, y, 0, kbnp[nln % 12]);
                drawFont4V(ym2612MIDIScreen, x, y - 2 * 4, 0, kbo[nln / 12]);
            }
            oln = nln;
        }


        public void drawButtons(int[] oldButton, int[] newButton, int[] oldButtonMode, int[] newButtonMode)
        {

            for (int i = 0; i < newButton.Length; i++)
            {
                drawButton(i, ref oldButton[i], newButton[i], ref oldButtonMode[i], newButtonMode[i]);
            }

        }


        public void screenInitAll()
        {


            for (int chipID = 0; chipID < 2; chipID++)
            {
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

            screenInitYM2612MIDI();

        }

        public void screenInitYM2151(int chipID)
        {
            //YM2151
            for (int ch = 0; ch < 8; ch++)
            {
                int tp = ((chipID == 0) ? setting.YM2151Type.UseScci : setting.YM2151SType.UseScci) ? 1 : 0;

                drawFont8(ym2151Screen[chipID], 296, ch * 8 + 8, 1, "   ");

                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(ym2151Screen[chipID], 32 + kx, ch * 8 + 8, kt, tp);
                }

                drawChPYM2151(chipID, 0, ch * 8 + 8, ch, false, tp);
                drawPanP(ym2151Screen[chipID], 24, ch * 8 + 8, 3, tp);
                int d = 99;
                drawVolume(ym2151Screen[chipID], ch, 1, ref d, 0, tp);
                d = 99;
                drawVolume(ym2151Screen[chipID], ch, 2, ref d, 0, tp);

            }
        }

        public void screenInitYM2203(int chipID)
        {
            //YM2203
            for (int y = 0; y < 3 + 3 + 3; y++)
            {
                int tp = chipID == 0 ? (setting.YM2203Type.UseScci ? 1 : 0) : (setting.YM2203SType.UseScci ? 1 : 0);

                drawFont8(ym2203Screen[chipID], 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2203Screen[chipID], 32 + kx, y * 8 + 8, kt, tp);
                }

                int d = 99;
                drawVolume(ym2203Screen[chipID], y, 0, ref d, 0, tp);
            }

        }

        public void screenInitYM2413(int chipID)
        {
            for (int y = 0; y < 9; y++)
            {
                int tp = chipID == 0 ? (setting.YM2413Type.UseScci ? 1 : 0) : (setting.YM2413SType.UseScci ? 1 : 0);

                //Note
                drawFont8(ym2413Screen[chipID], 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    drawKbn(ym2413Screen[chipID], 32 + kx, y * 8 + 8, kt, tp);
                }

                //Volume
                int d = 99;
                drawVolume(ym2413Screen[chipID], y, 0, ref d, 0, tp);
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

        public void screenInitYM2612(int chipID)
        {
            if (ym2612Screen[chipID] == null) return;

            for (int y = 0; y < 9; y++)
            {

                int d = 99;
                bool YM2612type = (chipID == 0) ? setting.YM2612Type.UseScci : setting.YM2612SType.UseScci;
                bool onlyPCM = setting.YM2612Type.OnlyPCMEmulation;
                int tp = YM2612type ? 1 : 0;
                int tp6 = tp;
                if (tp6 == 1 && onlyPCM)
                {
                    //tp6 = 0;
                }

                //note
                drawFont8(ym2612Screen[chipID], 296, y * 8 + 8, 1, "   ");

                //keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = kbl[(i % 12) * 2 + 1];
                    if (y != 5)
                    {
                        drawKbn(ym2612Screen[chipID], 32 + kx, y * 8 + 8, kt, tp);
                    }
                    else
                    {
                        drawKbn(ym2612Screen[chipID], 32 + kx, y * 8 + 8, kt, tp6);
                    }
                }

                if (y != 5)
                {
                    d = -1;
                    drawVolume(ym2612Screen[chipID], y, 0, ref d, 0, tp);
                }

                if (y < 6)
                {
                    d = 99;
                    drawPan(ym2612Screen[chipID], y, ref d, 3, ref d, tp);
                }

                if (y != 5)
                {
                    drawChPYM2612(chipID, 0, y * 8 + 8, y, false, tp);
                }
                else
                {
                    drawCh6PYM2612(chipID, 0, y * 8 + 8, 0, false, tp6);
                    d = -1;
                    drawVolume(ym2612Screen[chipID], y, 0, ref d, 0, tp6);
                    d = -1;
                    drawPan(ym2612Screen[chipID], y, ref d, 3, ref d, tp6);
                }

            }
        }

        public void screenInitOKIM6258(int chipID)
        {
            int o;
            int n;

            o = 0;n = 3;
            drawPanOKIM6258(chipID, ref o, n, ref o, 0);

            drawFont4(OKIM6258Screen[chipID], 12 * 4, 8, 0, string.Format("{0:d5}", 0));
            drawFont4(OKIM6258Screen[chipID], 19 * 4, 8, 0, string.Format("{0:d5}", 0));
            drawFont4(OKIM6258Screen[chipID], 26 * 4, 8, 0, string.Format("{0:d5}", 0));

            o = 0; n = 38;
            drawVolume(OKIM6258Screen[chipID], 0, 1, ref o, n / 2, 0);
            o = 0; n = 38;
            drawVolume(OKIM6258Screen[chipID], 0, 2, ref o, n / 2, 0);

        }

        public void screenInitOKIM6295(int chipID)
        {
        }

        public void screenInitSN76489(int chipID)
        {
            int tp = chipID == 0 ? (setting.SN76489Type.UseScci ? 1 : 0) : (setting.SN76489SType.UseScci ? 1 : 0);

            for (int ch = 0; ch < 4; ch++)
            {
                if (ch != 3)
                {
                    for (int ot = 0; ot < 12 * 8; ot++)
                    {
                        int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                        int kt = kbl[(ot % 12) * 2 + 1];
                        drawKbn(SN76489Screen[chipID], 32 + kx, ch * 8 + 8, kt, tp);
                    }
                }
                else
                {
                }

                drawFont8(SN76489Screen[chipID], 296, ch * 8 + 8, 1, "   ");
                drawChPSN76489(chipID, 0, ch * 8 + 8, ch, false, tp);

                int d = 99;
                drawVolume(SN76489Screen[chipID], ch, 0, ref d, 0, tp);
            }
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

        public void screenInitYM2612MIDI()
        {
            if (ym2612MIDIScreen == null) return;

            for (int c = 0; c < 6; c++)
            {
                for (int n = 0; n < 10; n++)
                {
                    drawFont4V(ym2612MIDIScreen, (c % 3) * 13 * 8 + 2 * 8+n*8, (c / 3) * 18 * 4 + 24 * 4, 0, "   ");
                }
            }
        }

    }
}

