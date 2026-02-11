using MDPlayerx64;
using System.Drawing.Imaging;
#if X64
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer
{
    public static class DrawBuff
    {

        private static int[][] rChipName;
        private static int[][] rFont1;
        private static int[][] rFont2;
        private static int[][] rFont3;
        private static int[][] rKBD;
        private static int[][] rMenuButtons;
        private static int[][] rPan;
        private static int[][] rPan2;
        private static int[] rPSGEnv;
        private static int[][] rPSGMode;
        private static int[] rPSG2;
        private static int[][] rType;
        private static int[][] rVol;
        private static int[] rWavGraph;
        private static int[] rWavGraph2;
        private static int[] rFader;
        private static int[] rFaderH;
        private static int[][] rMIDILCD_Fader;
        private static int[] rMIDILCD_KBD;
        private static int[][] rMIDILCD_Vol;
        public static int[][] rMIDILCD;
        private static int[][] rMIDILCD_Font;
        public static int[][] rPlane_MIDI;
        private static int[] rNESDMC;
        private static int[] rKakko;
        private static int[] rType_YMF271;

        private static Bitmap[] bitmapMIDILyric = null;
        private static Graphics[] gMIDILyric = null;
        private static Font[] fntMIDILyric = null;



        public static void Init()
        {
            rChipName = new int[3][];
            rChipName[0] = getIntArray(ResMng.ImgDic["rChipName_01"]);
            rChipName[1] = getIntArray(ResMng.ImgDic["rChipName_02"]);
            rChipName[2] = getIntArray(ResMng.ImgDic["rChipName_03"]);

            rFont1 = new int[2][];
            rFont1[0] = getIntArray(ResMng.ImgDic["rFont_01"]);
            rFont1[1] = getIntArray(ResMng.ImgDic["rFont_02"]);
            rFont2 = new int[5][];
            rFont2[0] = getIntArray(ResMng.ImgDic["rFont_03"]);
            rFont2[1] = getIntArray(ResMng.ImgDic["rFont_04"]);
            rFont2[2] = getIntArray(ResMng.ImgDic["rMIDILCD_Font_04"]);
            rFont2[3] = getIntArray(ResMng.ImgDic["rMIDILCD_Font_05"]);
            rFont2[4] = getIntArray(ResMng.ImgDic["rMIDILCD_Font_06"]);
            rFont3 = new int[2][];
            rFont3[0] = getIntArray(ResMng.ImgDic["rFont_05"]);
            rFont3[1] = getIntArray(ResMng.ImgDic["rFont_06"]);

            rKBD = new int[3][];
            rKBD[0] = getIntArray(ResMng.ImgDic["rKBD_01"]);
            rKBD[1] = getIntArray(ResMng.ImgDic["rKBD_02"]);
            rKBD[2] = getIntArray(ResMng.ImgDic["rKBD_03"]);

            rMenuButtons = new int[2][];
            rMenuButtons[0] = getIntArray(ResMng.ImgDic["rMenuButtons_01"]);
            rMenuButtons[1] = getIntArray(ResMng.ImgDic["rMenuButtons_02"]);

            rPan = new int[3][];
            rPan[0] = getIntArray(ResMng.ImgDic["rPan_01"]);
            rPan[1] = getIntArray(ResMng.ImgDic["rPan_02"]);
            rPan[2] = getIntArray(ResMng.ImgDic["rPan_03"]);

            rPan2 = new int[2][];
            rPan2[0] = getIntArray(ResMng.ImgDic["rPan2_01"]);
            rPan2[1] = getIntArray(ResMng.ImgDic["rPan2_02"]);

            rPSGEnv = getIntArray(ResMng.ImgDic["rPSGEnv"]);

            rPSGMode = new int[6][];
            rPSGMode[0] = getIntArray(ResMng.ImgDic["rPSGMode_01"]);
            rPSGMode[1] = getIntArray(ResMng.ImgDic["rPSGMode_02"]);
            rPSGMode[2] = getIntArray(ResMng.ImgDic["rPSGMode_03"]);
            rPSGMode[3] = getIntArray(ResMng.ImgDic["rPSGMode_04"]);
            rPSGMode[4] = getIntArray(ResMng.ImgDic["rPSGMode_05"]);
            rPSGMode[5] = getIntArray(ResMng.ImgDic["rPSGMode_06"]);

            rPSG2 = getIntArray(ResMng.ImgDic["rPSG2"]);

            rType = new int[6][];
            rType[0] = getIntArray(ResMng.ImgDic["rType_01"]);
            rType[1] = getIntArray(ResMng.ImgDic["rType_02"]);
            rType[2] = getIntArray(ResMng.ImgDic["rType_03"]);
            rType[3] = getIntArray(ResMng.ImgDic["rType_04"]);
            rType[4] = getIntArray(ResMng.ImgDic["rType_05"]);
            rType[5] = getIntArray(ResMng.ImgDic["rType_06"]);

            rVol = new int[3][];
            rVol[0] = getIntArray(ResMng.ImgDic["rVol_01"]);
            rVol[1] = getIntArray(ResMng.ImgDic["rVol_02"]);
            rVol[2] = getIntArray(ResMng.ImgDic["rVol_03"]);

            rWavGraph = getIntArray(ResMng.ImgDic["rWavGraph"]);
            rWavGraph2 = getIntArray(ResMng.ImgDic["rWavGraph2"]);
            rFader = getIntArray(ResMng.ImgDic["rFader"]);
            rFaderH = getIntArray(ResMng.ImgDic["rFaderH"]);
            rNESDMC = getIntArray(ResMng.ImgDic["rNESDMC"]);

            rKakko = getIntArray(ResMng.ImgDic["rKakko_00"]);

            rMIDILCD_Fader = new int[3][];
            rMIDILCD_Fader[0] = getIntArray(ResMng.ImgDic["rMIDILCD_Fader_01"]);
            rMIDILCD_Fader[1] = getIntArray(ResMng.ImgDic["rMIDILCD_Fader_02"]);
            rMIDILCD_Fader[2] = getIntArray(ResMng.ImgDic["rMIDILCD_Fader_03"]);

            rMIDILCD_KBD = getIntArray(ResMng.ImgDic["rMIDILCD_KBD_01"]);

            rMIDILCD_Vol = new int[3][];
            rMIDILCD_Vol[0] = getIntArray(ResMng.ImgDic["rMIDILCD_Vol_01"]);
            rMIDILCD_Vol[1] = getIntArray(ResMng.ImgDic["rMIDILCD_Vol_02"]);
            rMIDILCD_Vol[2] = getIntArray(ResMng.ImgDic["rMIDILCD_Vol_03"]);

            rMIDILCD = new int[3][];
            rMIDILCD[0] = getIntArray(ResMng.ImgDic["rMIDILCD_01"]);
            rMIDILCD[1] = getIntArray(ResMng.ImgDic["rMIDILCD_02"]);
            rMIDILCD[2] = getIntArray(ResMng.ImgDic["rMIDILCD_03"]);

            rMIDILCD_Font = new int[3][];
            rMIDILCD_Font[0] = getIntArray(ResMng.ImgDic["rMIDILCD_Font_01"]);
            rMIDILCD_Font[1] = getIntArray(ResMng.ImgDic["rMIDILCD_Font_02"]);
            rMIDILCD_Font[2] = getIntArray(ResMng.ImgDic["rMIDILCD_Font_03"]);

            rPlane_MIDI = new int[3][];
            rPlane_MIDI[0] = getIntArray(ResMng.ImgDic["planeMIDI_GM"]);
            rPlane_MIDI[1] = getIntArray(ResMng.ImgDic["planeMIDI_XG"]);
            rPlane_MIDI[2] = getIntArray(ResMng.ImgDic["planeMIDI_GS"]);

            bitmapMIDILyric = new Bitmap[4];
            gMIDILyric = new Graphics[4];
            fntMIDILyric = new Font[4];
            for (int i = 0; i < 4; i++)
            {
                bitmapMIDILyric[i] = new Bitmap(200, 24);
                gMIDILyric[i] = Graphics.FromImage(bitmapMIDILyric[i]);
                fntMIDILyric[i] = new Font("MS UI Gothic", 8);
            }

            //rType_YMF271 = getByteArray(ResMng.ImgDic["rType_YMF271"]);
            rType_YMF271 = getIntArray(ResMng.ImgDic["rType_YMF271"]);
        }



        public static void screenInitHuC6280(FrameBuffer screen)
        {
            for (int ch = 0; ch < 6; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
            }
        }

        public static void screenInitK051649(FrameBuffer screen)
        {
            for (int ch = 0; ch < 5; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
            }
        }

        public static void screenInitRF5C164(FrameBuffer screen)
        {
            //RF5C164
            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(screen, 24, ch * 8 + 8, 0, 0);
            }
        }

        public static void screenInitRF5C68(FrameBuffer screen)
        {
            //RF5C164
            for (int ch = 0; ch < 8; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(screen, 24, ch * 8 + 8, 0, 0);
            }
        }

        public static void screenInitMIDI(FrameBuffer screen)
        {
        }

        public static void screenInitMixer(FrameBuffer screen)
        {
        }

        public static void screenInitOKIM6258(FrameBuffer screen)
        {
            int o;
            int n;

            o = 0; n = 3;
            PanToOKIM6258(screen, ref o, n, ref o, 0);

            drawFont4(screen, 12 * 4, 8, 0, string.Format("{0:d5}", 0));
            drawFont4(screen, 19 * 4, 8, 0, string.Format("{0:d5}", 0));
            drawFont4(screen, 26 * 4, 8, 0, string.Format("{0:d5}", 0));

            o = 0; n = 38;
            Volume(screen, 256, 8 + 0 * 8, 1, ref o, n / 2, 0);
            o = 0; n = 38;
            Volume(screen, 256, 8 + 0 * 8, 2, ref o, n / 2, 0);

        }

        public static void screenInitOKIM6295(FrameBuffer screen)
        {
        }

        public static void screenInitPCM8(FrameBuffer screen)
        {
        }

        public static void screenInitMPCMX68k(FrameBuffer screen)
        {
            for (int y = 0; y < 16; y++)
            {
                drawFont8(screen, 142 * 4, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 40 + kx, y * 8 + 8, kt, 0);
                }

                drawPanP(screen, 32, y * 8 + 8, 3, 0);

                int d = 99;
                Volume(screen, 132 * 4, 8 + y * 8, 1, ref d, 0, 0);
                d = 99;
                Volume(screen, 132 * 4, 8 + y * 8, 2, ref d, 0, 0);
            }
        }

        public static void screenInitSN76489(FrameBuffer screen, int tp)
        {

            for (int ch = 0; ch < 4; ch++)
            {
                if (ch != 3)
                {
                    for (int ot = 0; ot < 12 * 8; ot++)
                    {
                        int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                        int kt = Tables.kbl[(ot % 12) * 2 + 1];
                        drawKbn(screen, 32 + kx, ch * 8 + 8, kt, tp);
                    }
                }
                else
                {
                }

                DrawBuff.drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
                DrawBuff.ChSN76489_P(screen, 0, ch * 8 + 8, ch, false, tp);

                //int d = 99;
                //DrawBuff.Volume(screen, 256, 8 + ch * 8, 0, ref d, 0, tp);
            }
        }

        public static void screenInitYM2203(FrameBuffer screen, int tp)
        {
            if (screen == null) return;

            //YM2203
            for (int y = 0; y < 3 + 3 + 3; y++)
            {

                drawFont8(screen, 296 + 32, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                int d = 99;
                Volume(screen, 272, 8 + y * 8, 0, ref d, 0, tp);

                if (y < 9)
                {
                    ChYM2203_P(screen, 0, y * 8 + 8, y, false, tp);
                }
            }

        }

        public static void screenInitYM2608(FrameBuffer screen, int tp)
        {
            //YM2608
            for (int y = 0; y < 6 + 3 + 3 + 1; y++)
            {

                drawFont8(screen, 328 + 1, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 33 + kx, y * 8 + 8, kt, tp);
                }

                if (y < 13)
                {
                    ChYM2608_P(screen, 1, y * 8 + 8, y, false, tp);
                }

                if (y < 6 || y == 12)
                {
                    drawPanP(screen, 25, y * 8 + 8, 3, tp);
                }

                int d = 99;
                if (y > 5 && y < 9)
                {
                    Volume(screen, 272 + 1, 8 + y * 8, 0, ref d, 0, tp);
                }
                else
                {
                    Volume(screen, 272 + 1, 8 + y * 8, 1, ref d, 0, tp);
                    d = 99;
                    Volume(screen, 272 + 1, 8 + y * 8, 2, ref d, 0, tp);
                }
            }

            for (int y = 0; y < 6; y++)
            {
                int d = 99;
                PanYM2608Rhythm(screen, y, ref d, 3, ref d, tp);
                d = 99;
                VolumeYM2608Rhythm(screen, y, 1, ref d, 0, tp);
                d = 99;
                VolumeYM2608Rhythm(screen, y, 2, ref d, 0, tp);
            }
        }

        public static void screenInitYM2609(FrameBuffer screen, int tp)
        {
            //YM2609
            for (int y = 0; y < 12 + 6 + 12 + 4; y++)
            {
                //drawFont8(screen, 296, y * 8 + 8, 1, "   ");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 33 + kx, y * 8 + 8, kt, tp);
                }

                //if (y < 13)
                //{
                //    ChYM2608_P(screen, 1, y * 8 + 8, y, false, tp);
                //}

                //if (y < 6 || y == 12)
                //{
                //    drawPanP(screen, 25, y * 8 + 8, 3, tp);
                //}

                int d = 99;
                if (y > 5 && y < 9)
                {
                    Volume(screen, 289, 8 + y * 8, 0, ref d, 0, tp);
                }
                else
                {
                    Volume(screen, 289, 8 + y * 8, 1, ref d, 0, tp);
                    d = 99;
                    Volume(screen, 289, 8 + y * 8, 2, ref d, 0, tp);
                }
            }

            //for (int y = 0; y < 6; y++)
            //{
            //    int d = 99;
            //    PanYM2608Rhythm(screen, y, ref d, 3, ref d, tp);
            //    d = 99;
            //    VolumeYM2608Rhythm(screen, y, 1, ref d, 0, tp);
            //    d = 99;
            //    VolumeYM2608Rhythm(screen, y, 2, ref d, 0, tp);
            //}
        }

        public static void screenInitYM2612(FrameBuffer screen, int tp, bool onlyPCM, int isXGM)
        {
            if (screen == null) return;

            for (int y = 0; y < 9; y++)
            {

                int d = 99;
                //                bool YM2612type = chipID==0 ? parent.setting.YM2612Type.UseScci : setting.YM2612SType.UseScci;
                int tp6 = tp;
                if (tp6 == 1 && onlyPCM)
                {
                    //tp6 = 0;
                }

                //note
                drawFont8(screen, 297, y * 8 + 8, 1, "   ");

                //keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    if (y != 5)
                    {
                        drawKbn(screen, 33 + kx, y * 8 + 8, kt, tp);
                    }
                    else
                    {
                        if (isXGM == 0) drawKbn(screen, 33 + kx, y * 8 + 8, kt, tp6);
                    }
                }

                if (isXGM == 1)
                {
                    Ch6YM2612XGM_P(screen, 1, 48, 0, false, false, false, false, false, tp6);
                }
                else if (isXGM == 2)
                {
                    Ch6YM2612XGM2_P(screen, 1, 48, 0, false, tp6);

                }

                if (y != 5)
                {
                    d = -1;
                    Volume(screen, 289, 8 + y * 8, 0, ref d, 0, tp);
                }

                if (y < 6)
                {
                    d = 99;
                    DrawBuff.Pan(screen, 25, 8 + y * 8, ref d, 3, ref d, tp);
                    byte b = 255;
                    Slot(screen, 257, 8 + y * 8, ref b, 0);
                }
                d = 1;
                font4Hex16Bit(screen, 273, 8 + y * 8, 0, ref d, 0);

                if (y != 5)
                {
                    //ChYM2612_P(screen, 1, y * 8 + 8, y, false, tp);
                }
                else
                {
                    //Ch6YM2612_P(screen, 1, y * 8 + 8, 0, false, tp6);
                    d = -1;
                    Volume(screen, 289, 8 + y * 8, 0, ref d, 0, tp6);
                    d = -1;
                    DrawBuff.Pan(screen, 25, 8 + y * 8, ref d, 3, ref d, tp6);
                }

            }
        }

        public static void screenInitYM3526(FrameBuffer screen, int tp)
        {
            for (int y = 0; y < 9; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                bool? dm = true;
                DrawBuff.ChYM3526(screen, y, ref dm, false, tp);

                //Volume
                int d = 99;
                Volume(screen, 256, 8 + y * 8, 0, ref d, 19, tp);
            }

        }

        public static void screenInitY8950(FrameBuffer screen, int tp)
        {
            for (int y = 0; y < 10; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                //Volume
                int d = 99;
                Volume(screen, 256, 8 + y * 8, 0, ref d, 19, tp);
            }

        }

        public static void screenInitYM3812(FrameBuffer screen, int tp)
        {
            for (int y = 0; y < 9; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                bool? dm = true;
                DrawBuff.ChYM3812(screen, y, ref dm, false, tp);

                //Volume
                int d = 99;
                Volume(screen, 256, 8 + y * 8, 0, ref d, 19, tp);
            }

        }

        public static void screenInitYMF262(FrameBuffer screen, int tp)
        {
            if (screen == null) return;

            for (int y = 0; y < 18; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                //bool bd = false;
                //ChYMF262(screen, y, ref bd, true, tp);
                //ChYMF262(screen, y, ref bd, false, tp);
                drawPanP(screen, 24, y * 8 + 8, 3, tp);

                //Volume
                int d = 99;
                Volume(screen, 256, 8 + y * 8, 0, ref d, 19, tp);
                Volume(screen, 256, 8 + y * 8, 0, ref d, 0, tp);
            }

        }

        public static void screenInitYMF278B(FrameBuffer screen, int tp)
        {
            for (int y = 0; y < 18; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                //Volume
                int d = 99;
                Volume(screen, 256, 8 + y * 8, 0, ref d, 19, tp);
            }

            for (int y = 19; y < 19 + 24; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 15 * 12; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                //Volume
                int d = 99;
                VolumeSt(screen, 512 - 4 * 15, y, 1, ref d, 19);
                d = 99;
                VolumeSt(screen, 512 - 4 * 15, y + 4, 1, ref d, 19);
            }

        }

        public static void screenInitYM2612MIDI(FrameBuffer screen)
        {
            if (screen == null) return;

            for (int c = 0; c < 6; c++)
            {
                for (int n = 0; n < 10; n++)
                {
                    drawFont4V(screen, (c % 3) * 13 * 8 + 2 * 8 + n * 8, (c / 3) * 18 * 4 + 24 * 4, 0, "   ");
                }
            }
        }

        public static void screenInitNESDMC(FrameBuffer screen)
        {
            if (screen == null) return;

            for (int ch = 0; ch < 3; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 16 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 16 + 8, 1, "   ");
                bool? m = true;
                ChNESDMC(screen, ch, ref m, false, 0);
            }
        }

        public static void screenInitFDS(FrameBuffer screen)
        {
            if (screen == null) return;

            for (int ot = 0; ot < 12 * 8; ot++)
            {
                int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                int kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, 8, kt, 0);
            }
            drawFont8(screen, 296, 8, 1, "   ");
            bool? m = true;
            ChFDS(screen, 0, ref m, false, 0);
        }

        public static void screenInitVRC7(FrameBuffer screen, int tp)
        {
            for (int y = 0; y < 6; y++)
            {
                //Note
                drawFont8(screen, 296, y * 8 + 8, 1, "   ");

                //Keyboard
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, y * 8 + 8, kt, tp);
                }

                //Volume
                int d = 99;
                Volume(screen, 256, 8 + y * 8, 0, ref d, 0, tp);
            }
        }


        public static void InstOPN2(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni)
        {
            int sx = (c % 3) * 4 * 29 + x;
            int sy = (c / 3) * 8 * 6 + y;

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

        public static void Inst(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni)
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

        public static void InstOPM(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni)
        {
            int sx = (c % 3) * 27 * 4 + x;
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

        public static void InstOPNA(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni)
        {
            int sx = (c % 3) * 4 * 25 + x;
            int sy = (c / 3) * 8 * 6 + y;

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

        public static void InstOPNA2(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni)
        {
            int sx = x;
            int sy = y;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (oi[i + j * 16] != ni[i + j * 16])
                    {
                        drawFont4Int(screen, sx + i * 8 + (i > 5 ? 4 : 0), sy + j * 8, 0, (i == 5) ? 3 : 2, ni[i + j * 16]);
                        oi[i + j * 16] = ni[i + j * 16];
                    }
                }
            }

            if (oi[64] != ni[64])
            {
                drawFont4Int(screen, sx + 4 * 9, sy - 16, 0, 2, ni[64]);
                oi[64] = ni[64];
            }
            if (oi[65] != ni[65])
            {
                drawFont4Int(screen, sx + 4 * 14, sy - 16, 0, 2, ni[65]);
                oi[65] = ni[65];
            }
            if (oi[66] != ni[66])
            {
                drawFont4Int(screen, sx + 4 * 19, sy - 16, 0, 2, ni[66]);
                oi[66] = ni[66];
            }
        }

        public static void Inst(FrameBuffer screen, int x, int y, int c, int[] oi, int[] ni, int[] ot, int[] nt)
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

        public static void Slot(FrameBuffer screen, int x, int y, ref byte os, byte ns)
        {
            if (os == ns) return;

            screen.drawIntArray(x + 0, y, rNESDMC, 64, ((ns & 1) != 0 ? 1 : 0) * 4 + 32, 0, 4, 8);
            screen.drawIntArray(x + 4, y, rNESDMC, 64, ((ns & 2) != 0 ? 1 : 0) * 4 + 32, 0, 4, 8);
            screen.drawIntArray(x + 8, y, rNESDMC, 64, ((ns & 4) != 0 ? 1 : 0) * 4 + 32, 0, 4, 8);
            screen.drawIntArray(x + 12, y, rNESDMC, 64, ((ns & 8) != 0 ? 1 : 0) * 4 + 32, 0, 4, 8);

            os = ns;
        }

        public static void drawInstNumber(FrameBuffer screen, int x, int y, ref int oi, int ni)
        {
            if (oi != ni)
            {
                drawFont4Int(screen, x * 4, y * 4, 0, 2, ni);
                oi = ni;
            }
        }

        /// <summary>
        /// ボリュームメータ描画
        /// </summary>
        /// <param name="screen">描画対象バッファ</param>
        /// <param name="x">x座標(x1)</param>
        /// <param name="y">y座標(x1)</param>
        /// <param name="c">0:Mono 1:Stereo(L) 2:Stereo(R)</param>
        /// <param name="ov">前回の値(ref)</param>
        /// <param name="nv">今回の値</param>
        /// <param name="tp">0:EMU 1:Real </param>
        public static void Volume(FrameBuffer screen, int x, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            //y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeShort(FrameBuffer screen, int x, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            //int t = 0;
            //int sy = 0;
            //if (c == 1 || c == 2) { t = 4; }
            //if (c == 2) { sy = 4; }
            //y = (y + 1) * 8;

            //for (int i = 0; i <= 19; i++)
            //{
            //    VolumeP(screen, 256 + i * 2, y + sy, (1 + t), tp);
            //}

            //for (int i = 0; i <= nv; i++)
            //{
            //    VolumeP(screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            //}

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            //y = (y + 1) * 8;

            for (int i = 0; i <= 15; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, i > 13 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeToC140(FrameBuffer screen, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, 356 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, 356 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeSt(FrameBuffer screen, int x, int y, int c, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            //x=256
            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, (1 + t), 0);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), 0);
            }

            ov = nv;

        }

        public static void VolumeToHuC6280(FrameBuffer screen, int y, int c, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, 256 + i * 2, y + sy, (1 + t), 0);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), 0);
            }

            ov = nv;

        }

        public static void VolumeToOKIM6295(FrameBuffer screen, int y, ref int ov, int nv)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, 80 + i * 2, y + sy, (1 + t), 0);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, 80 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), 0);
            }

            ov = nv;

        }

        public static void VolumeLCDToMIDILCD(FrameBuffer screen, int MIDImodule, int x, int y, ref int oldValue1, int value1, ref int oldValue2, int value2)
        {
            if (oldValue1 == value1 && oldValue2 == value2) return;

            int s = 0;
            int vy = y;
            //for (int n = (Math.Min(oldValue1, value1) / 8); n < 16; n++)
            for (int n = 0; n < 16; n++)
            {
                s = (value1 / 8) < n ? 8 : 0;
                screen.drawIntArray(x, vy, rMIDILCD[MIDImodule], 136, 8 * 16, s, 8, 3);// (n % 2 == 0 ? 2 : 3));
                vy -= 4;// (n % 2 == 0 ? 4 : 3);
            }

            s = value2 / 8;
            //screen.drawIntArray(x, y - s * 3 - (s + 1) / 2, rMIDILCD[MIDImodule], 136, 8 * 16, 0, 8, (s % 2 == 0 ? 2 : 3));
            screen.drawIntArray(x, y - s * 4, rMIDILCD[MIDImodule], 136, 8 * 16, 0, 8, 3);

            oldValue1 = value1;
            oldValue2 = value2;
        }

        public static void VolumeToMIDILCD(FrameBuffer screen, int MIDImodule, int x, int y, ref int oldValue, int value)
        {
            if (oldValue == value) return;

            int s = 0;
            for (int n = (Math.Min(oldValue, value) / 5); n < (Math.Max(oldValue, value) / 5) + 1; n++)
            {
                s = (value / 5) < n ? 2 : 0;
                screen.drawIntArray(n * 2 + x, y, rMIDILCD_Vol[MIDImodule], 32, 0 + (n > 23 ? 4 : 0) + s, 0, 2, 8);
            }

            oldValue = value;
        }

        public static void VolumeXY(FrameBuffer screen, int x, int y, int c, ref int ov, int nv, int tp)
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
                VolumeP(screen, x + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeXY1(FrameBuffer screen, int x, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeXYOPN2(FrameBuffer screen, int x, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }

            //y *= 4;
            //x *= 4;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeYM2608Rhythm(FrameBuffer screen, int x, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            x = x * 4 * 15 + 20;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, sy + 8 * 14, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, sy + 8 * 14, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeYM2609Rhythm(FrameBuffer screen, int x, int y, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 4;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, y, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, y, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeYM2610Rhythm(FrameBuffer screen, int x, int c, ref int ov, int nv, int tp)
        {

            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            x = x * 4 * 15 + 6 * 4 + 1;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, x + i * 2, sy + 8 * 13, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, x + i * 2, sy + 8 * 13, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void KeyBoard(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            drawFont8(screen, 296, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 296, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 312, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardOPM(FrameBuffer screen, int ch, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            int y = (ch + 1) * 8;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 33 + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 33 + kx, y, kt, tp);
            }

            drawFont8(screen, 82 * 4 + 1, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 82 * 4 + 1, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 86 * 4 + 1, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardXY(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, x + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, x + kx, y, kt, tp);
            }

            drawFont8(screen, 264 + x, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 264 + x, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 280 + x, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardXYFX(FrameBuffer screen, int x, int fx, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, x + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, x + kx, y, kt, tp);
            }

            drawFont8(screen, fx, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, fx, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 16 + fx, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardDCSG(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx;
            int kt;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, x + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, x + kx, y, kt, tp);
            }

            drawFont8(screen, 288 + x, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 288 + x, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 304 + x, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardOPNA(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, x + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, x + kx, y, kt, tp);
            }

            drawFont8(screen, 296 + x, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 296 + x, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 312 + x, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardOPNM(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 33 + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 33 + kx, y, kt, tp);
            }

            drawFont8(screen, 329, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 329, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 345, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardDMG(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            drawFont8(screen, 312, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 312, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 328, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardToC140(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
                drawFont8(screen, 396, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(screen, 412, y, 1, Tables.kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(screen, 396, y, 1, "   ");
            }

            ot = nt;
        }

        public static void KeyBoardToC352(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
                drawFont8(screen, 500, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(screen, 516, y, 1, Tables.kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(screen, 500, y, 1, "   ");
            }

            ot = nt;
        }

        public static void KeyBoardToGA20(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            drawFont8(screen, 296 + 4 * 24, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 296 + 4 * 24, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, 312 + 4 * 24, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardToMultiPCM(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
                drawFont8(screen, 63 * 8 + 4, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(screen, 65 * 8 + 4, y, 1, Tables.kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(screen, 63 * 8 + 4, y, 1, "   ");
            }

            ot = nt;
        }

        public static void KeyBoardToYMF278BPCM(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0 && ot < 12 * 15)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 15)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            drawFont8(screen, 300 + 8 * 24, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, 300 + 8 * 24, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 15)
                {
                    drawFont8(screen, 300 + 8 * 26, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }

        public static void KeyBoardToQSound(FrameBuffer screen, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, tp);
            }

            int x = 52;
            if (nt >= 0)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, tp);
                drawFont8(screen, x * 8, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 8)
                {
                    drawFont8(screen, (x + 2) * 8, y, 1, Tables.kbo[nt / 12]);
                }
            }
            else
            {
                drawFont8(screen, x * 8, y, 1, "   ");
            }

            ot = nt;
        }

        public static void KeyBoardYMZ280B(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            if (ot >= 0 && ot < 12 * 8)
            {
                kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = Tables.kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, x + kx, y, kt, tp);
            }

            if (nt >= 0 && nt < 12 * 8)
            {
                kx = Tables.kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = Tables.kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, x + kx, y, kt, tp);
            }

            int i = 280;
            drawFont8(screen, i + x, y, 1, "   ");

            if (nt >= 0)
            {
                drawFont8(screen, i + x, y, 1, Tables.kbn[nt % 12]);
                if (nt / 12 < 10)
                {
                    drawFont8(screen, i + 16 + x, y, 1, Tables.kbo[nt / 12]);
                }
            }

            ot = nt;
        }


        public static void Pan(FrameBuffer screen, int x, int y, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(screen, x, y, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void PanType2(FrameBuffer screen, int c, ref int ot, int nt, int tp)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(screen, 24, 8 + c * 8, nt, tp);
            ot = nt;
        }

        public static void PanType3(FrameBuffer screen, int c, ref int ot, int nt, int tp)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType3P(screen, 24, 8 + c * 8, nt, tp);
            ot = nt;
        }

        public static void PanType4(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType4P(screen, x, y, nt, tp);
            ot = nt;
        }

        public static void PanType5(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType5P(screen, x, y, nt, tp);
            ot = nt;
        }

        public static void PanType6(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType6P(screen, x, y, nt, tp);
            ot = nt;
        }

        public static void PanType2(FrameBuffer screen, int x, int y, ref int ot, int nt, int tp)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(screen, x, y, nt, tp);
            ot = nt;
        }

        public static void PanToOKIM6258(FrameBuffer screen, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(screen, 24, 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void PanYM2608Rhythm(FrameBuffer screen, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(screen, c * 4 * 15 + 12, 8 * 14, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void PanYM2609Rhythm(FrameBuffer screen, int x, int y, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(screen, x, y, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void PanYM2610Rhythm(FrameBuffer screen, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawPanP(screen, c * 4 * 15 + 17, 8 * 13, nt, ntp);
            ot = nt;
            otp = ntp;
        }



        public static void ChAY8910(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChAY8910_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChS5B(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChS5B_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChC140(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChC140_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChQSound(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }
            if (ch < 16)
            {
                ChQSound_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            }
            else
            {
                ChQSoundAdpcm_P(screen, 224, 8 + ch * 8, ch - 16, nm == null ? false : (bool)nm, tp);
            }
            om = nm;
        }

        public static void ChC352(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChC352_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYMZ280B(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYMZ280B_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChK053260(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChC352_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChHuC6280(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChHuC6280_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChOKIM6295(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChOKIM6295_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChPCM8(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChPCM8_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChMPCMX68k(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChMPCMX68k_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChPPZ8(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChPPZ8_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChK051649(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChK051649_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChRF5C164(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChRF5C164_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChOKIM6258(FrameBuffer screen, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChOKIM6258_P(screen, 0, 8 + 0 * 8, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChSegaPCM(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChSegaPCM_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChSN76489(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChSN76489_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChSN76489Noise(FrameBuffer screen, ref MDChipParams.Channel osc, MDChipParams.Channel nsc, int tp)
        {
            if (osc.note == nsc.note) return;

            drawFont4(screen, 56, 32, tp, (nsc.note & 0x4) != 0 ? "WHITE   " : "PERIODIC");
            drawFont4(screen, 120, 32, tp, (nsc.note & 0x3) == 0 ? "0  " : ((nsc.note & 0x3) == 1 ? "1  " : ((nsc.note & 0x3) == 2 ? "2  " : "CH3")));

            osc.note = nsc.note;
        }

        public static void ChYM2151(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2151_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYM2203(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2203_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void Ch3YM2203(FrameBuffer screen, int ch, ref bool? om, bool? nm, ref bool oe, bool ne, int tp)
        {

            if (om == nm && oe == ne)
            {
                return;
            }

            Ch3YM2612_P(screen, 0, 8 + ch * 8, ch, nm == null ? false : (bool)nm, ne, tp);
            om = nm;
            oe = ne;
        }

        public static void ChYM2413(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2413_P(screen, 0, ch < 9 ? (8 + ch * 8) : (8 + 9 * 8), ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChY8950(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }
            ChY8950_P(screen, 0
                , ch < 9
                    ? (8 + ch * 8)
                    : (ch < 14 ? (8 + 10 * 8) : (8 + 9 * 8))
                , ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYM3526(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }
            ChYM3526_P(screen, 0
                , ch < 9
                    ? (8 + ch * 8)
                    : (8 + 9 * 8)
                , ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYM3812(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }
            ChYM3812_P(screen, 0
                , ch < 9
                    ? (8 + ch * 8)
                    : (8 + 9 * 8)
                , ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        private static byte[] YMF262Ch = new byte[]
        {
                0,3,1,4,2,5,6,7,8,9,12,10,13,11,14,15,16,17,
                18,19,20,21,22
        };
        public static void ChYMF262(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }
            ChYMF262_P(screen, 0
                , ch < 18
                    ? (8 + ch * 8)
                    : (8 + 18 * 8)
                , YMF262Ch[ch], nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        private static byte[] YMF278BCh = new byte[]
        {
                0,3,1,4,2,5,6,7,8,9,12,10,13,11,14,15,16,17,
                18,19,20,21,22,
                23,24,25,26,27,28, 29,30,31,32,33,34,
                35,36,37,38,39,40, 41,42,43,44,45,46
        };
        public static void ChYMF278B(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }
            ChYMF278B_P(screen, 0
                , ch < 18
                    ? (8 + ch * 8)
                    : (ch < 23
                        ? (8 + 18 * 8)
                        : (8 + (ch - 4) * 8)), YMF278BCh[ch], nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYM2608(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2608_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void Ch3YM2608(FrameBuffer screen, int ch, ref bool? om, bool? nm, ref bool oe, bool ne, int tp)
        {

            if (om == nm && oe == ne)
            {
                return;
            }

            Ch3YM2612_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, ne, tp);
            om = nm;
            oe = ne;
        }

        public static void ChYM2608Rhythm(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2608Rhythm_P(screen, 0, 8 * 14, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYM2610(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2610_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void Ch3YM2610(FrameBuffer screen, int ch, ref bool? om, bool? nm, ref bool oe, bool ne, int tp)
        {

            if (om == nm && oe == ne)
            {
                return;
            }

            Ch3YM2612_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, ne, tp);
            om = nm;
            oe = ne;
        }

        public static void ChYM2610Rhythm(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2610Rhythm_P(screen, 1, 8 * 13, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChYM2612(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChYM2612_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void Ch3YM2612(FrameBuffer screen, int ch, ref bool? om, bool? nm, ref bool oe, bool ne, int tp)
        {

            if (om == nm && oe == ne)
            {
                return;
            }

            Ch3YM2612_P(screen, 1, 8 + ch * 8, ch, nm == null ? false : (bool)nm, ne, tp);
            om = nm;
            oe = ne;
        }

        public static void Ch6YM2612(FrameBuffer screen, int buff, ref int ot, int nt, ref bool? om, bool? nm, ref int otp, int ntp)
        {
            if (buff == 0)
            {
                if (ot == nt && om == nm && otp == ntp)
                {
                    return;
                }
            }

            Ch6YM2612_P(screen, 1, 48, nt, nm == null ? false : (bool)nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public static void Ch6YM2612XGM(FrameBuffer screen, int buff, ref int ot, int nt, ref bool? om, bool? nm,
            ref bool? om1, bool? nm1,
            ref bool? om2, bool? nm2,
            ref bool? om3, bool? nm3,
            ref bool? om4, bool? nm4, 
            ref int otp, int ntp)
        {
            if (buff == 0)
            {
                if (ot == nt && om == nm && om1 == nm1 && om2 == nm2 && om3 == nm3 && om4 == nm4 && otp == ntp)
                {
                    return;
                }
            }

            Ch6YM2612XGM_P(screen, 1, 48, nt, nm == null ? false : (bool)nm
                , nm1 == null ? false : (bool)nm1
                , nm2 == null ? false : (bool)nm2
                , nm3 == null ? false : (bool)nm3
                , nm4 == null ? false : (bool)nm4
                , ntp);
            ot = nt;
            om = nm;
            om1 = nm1;
            om2 = nm2;
            om3 = nm3;
            om4 = nm4;
            otp = ntp;
        }

        public static void Ch6YM2612XGM2(FrameBuffer screen, int buff, ref int ot, int nt, ref bool? om, bool? nm, ref int otp, int ntp)
        {
            if (buff == 0)
            {
                if (ot == nt && om == nm && otp == ntp)
                {
                    return;
                }
            }

            Ch6YM2612XGM2_P(screen, 1, 48, nt, nm == null ? false : (bool)nm, ntp);
            ot = nt;
            om = nm;
            otp = ntp;
        }

        public static void ChNESDMC(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChNESDMC_P(screen, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChFDS(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChFDS_P(screen, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChMMC5(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChMMC5_P(screen, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChDMG(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {
            if (om == nm) return;

            ChDMG_P(screen, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChVRC6(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChVRC6_P(screen, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }

        public static void ChN163(FrameBuffer screen, int ch, ref bool? om, bool? nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChN163_P(screen, ch, nm == null ? false : (bool)nm, tp);
            om = nm;
        }




        public static void ToneNoise(FrameBuffer screen, int x, int y, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            ToneNoiseP(screen, x * 4, y * 4 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void Nfrq(FrameBuffer screen, int x, int y, ref int onfrq, int nnfrq)
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

        public static void Efrq(FrameBuffer screen, int x, int y, ref int oefrq, int nefrq)
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

        public static void Etype(FrameBuffer screen, int x, int y, ref int oetype, int netype)
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

        public static void WaveFormToHuC6280(FrameBuffer screen, int c, ref int[] oi, int[] ni)
        {
            for (int i = 0; i < 32; i++)
            {
                if (oi[i] == ni[i]) continue;

                int n = (17 - ni[i]);
                int x = i + (((c > 2) ? c - 3 : c) * 8 * 13) + 4 * 7;
                int y = (((c > 2) ? 1 : 0) * 8 * 5) + 4 * 22;

                int m = 0;
                m = (n > 7) ? 8 : n;
                screen.drawIntArray(x, y, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 15) ? 8 : ((n - 8) < 0 ? 0 : (n - 8));
                screen.drawIntArray(x, y - 8, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 23) ? 8 : ((n - 16) < 0 ? 0 : (n - 16));
                screen.drawIntArray(x, y - 16, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 31) ? 8 : ((n - 24) < 0 ? 0 : (n - 24));
                screen.drawIntArray(x, y - 23, rWavGraph, 64, m + 1, 0, 1, 7);

                oi[i] = ni[i];
            }
        }

        public static void WaveFormToK051649(FrameBuffer screen, int c, ref int[] oi, int[] ni)
        {
            for (int i = 0; i < 32; i++)
            {
                if (oi[i] == ni[i]) continue;

                int n = (ni[i] / 8) + 16;
                int x = c % 3;
                x = x * 104 + i + 4;
                int y = c / 3;
                y = y * 48 + 80;

                int m = 0;
                m = (n > 7) ? 8 : n;
                screen.drawIntArray(x, y, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 15) ? 8 : ((n - 8) < 0 ? 0 : (n - 8));
                screen.drawIntArray(x, y - 8, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 23) ? 8 : ((n - 16) < 0 ? 0 : (n - 16));
                screen.drawIntArray(x, y - 16, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 31) ? 8 : ((n - 24) < 0 ? 0 : (n - 24));
                screen.drawIntArray(x, y - 23, rWavGraph, 64, m + 1, 0, 1, 7);

                oi[i] = ni[i];
            }
        }

        public static void WaveFormToFDS(FrameBuffer screen, int c, ref int[] oi, int[] ni)
        {
            for (int i = 0; i < 32; i++)
            {
                if (oi[i] == ni[i]) continue;

                int n = ni[i];
                int x = i + c * 4 * 31 + 8;
                int y = 8 * 6;

                int m = 0;
                m = (n > 7) ? 8 : n;
                screen.drawIntArray(x, y, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 15) ? 8 : ((n - 8) < 0 ? 0 : (n - 8));
                screen.drawIntArray(x, y - 8, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 23) ? 8 : ((n - 16) < 0 ? 0 : (n - 16));
                screen.drawIntArray(x, y - 16, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 31) ? 8 : ((n - 24) < 0 ? 0 : (n - 24));
                screen.drawIntArray(x, y - 23, rWavGraph, 64, m + 1, 0, 1, 7);

                oi[i] = ni[i];
            }
        }

        public static void WaveFormToN106(FrameBuffer screen, int x, int y, ref short[] oi, short[] ni)
        {
            if (ni == null) return;

            for (int i = 0; i < ni.Length; i++)
            {
                if (oi[i] == ni[i]) continue;

                screen.drawIntArray(x + i, y, rWavGraph2, 33, ni[i] % 33, 0, 1, 16);

                oi[i] = ni[i];
            }
        }

        public static void WaveFormToDMG(FrameBuffer screen, int x, int y, ref byte[] oi, byte[] ni)
        {
            for (int i = 0; i < 32; i++)
            {
                if (oi[i] == ni[i]) continue;

                int n = ni[i];

                int m = 0;
                m = (n > 7) ? 8 : n;
                screen.drawIntArray(x + i, y, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 15) ? 8 : ((n - 8) < 0 ? 0 : (n - 8));
                screen.drawIntArray(x + i, y - 8, rWavGraph, 64, m, 0, 1, 8);
                //m = (n > 23) ? 8 : ((n - 16) < 0 ? 0 : (n - 16));
                //screen.drawByteArray(x + i, y - 16, rWavGraph, 64, m, 0, 1, 8);
                //m = (n > 31) ? 8 : ((n - 24) < 0 ? 0 : (n - 24));
                //screen.drawByteArray(x + i, y - 23, rWavGraph, 64, m + 1, 0, 1, 7);

                oi[i] = ni[i];
            }
        }

        public static void WaveFormYM2609User(FrameBuffer screen, int x, int y, ref byte[] oi, byte[] ni)
        {
            if (oi == null) oi = new byte[64];

            for (int i = 0; i < 32; i++)
            {
                byte l = (byte)((ni[i * 2] + ni[i * 2 + 1]) / 2);
                if (oi[i] == l) continue;
                oi[i] = l;

                int n = (l / 8);
                int m = 0;
                m = (n > 7) ? 8 : n;
                screen.drawIntArray(x + i, y, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 15) ? 8 : ((n - 8) < 0 ? 0 : (n - 8));
                screen.drawIntArray(x + i, y - 8, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 23) ? 8 : ((n - 16) < 0 ? 0 : (n - 16));
                screen.drawIntArray(x + i, y - 16, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 31) ? 8 : ((n - 24) < 0 ? 0 : (n - 24));
                screen.drawIntArray(x + i, y - 23, rWavGraph, 64, m + 1, 0, 1, 7);

            }
        }

        public static void WaveFormYM2609Preset(FrameBuffer screen, int x, int y, ref int oi, int ni)
        {
            if (oi == ni) return;
            oi = ni;

            screen.drawIntArray(x, y, rPSG2, 320, ni * 32, 0, 32, 32);
        }

        public static void DDAToHuC6280(FrameBuffer screen, int c, ref bool od, bool nd)
        {
            if (od == nd) return;

            int x = (((c > 2) ? c - 3 : c) * 8 * 13) + 4 * 22;
            int y = (((c > 2) ? 1 : 0) * 8 * 5) + 4 * 18;

            drawFont4(screen, x, y, 0, nd ? "ON " : "OFF");
            od = nd;
        }

        public static void NoiseToHuC6280(FrameBuffer screen, int c, ref bool od, bool nd)
        {
            if (od == nd) return;

            int x = (((c > 2) ? c - 3 : c) * 8 * 13) + 4 * 22;
            int y = (((c > 2) ? 1 : 0) * 8 * 5) + 4 * 20;

            drawFont4(screen, x, y, 0, nd ? "ON " : "OFF");
            od = nd;
        }

        public static void NoiseFrqToHuC6280(FrameBuffer screen, int c, ref int od, int nd)
        {
            if (od == nd) return;

            int x = (((c > 2) ? c - 3 : c) * 8 * 13) + 4 * 22;
            int y = (((c > 2) ? 1 : 0) * 8 * 5) + 4 * 22;

            drawFont4(screen, x, y, 0, string.Format("{0:d2}", nd));
            od = nd;
        }

        public static void MainVolumeToHuC6280(FrameBuffer screen, int c, ref int od, int nd)
        {
            if (od == nd) return;

            int x = 8 * 9;
            int y = c * 8 + 8 * 17;

            drawFont4(screen, x, y, 0, string.Format("{0:d2}", nd));
            od = nd;
        }

        public static void LfoCtrlToHuC6280(FrameBuffer screen, ref int od, int nd)
        {
            if (od == nd) return;

            int x = 8 * 17;
            int y = 8 * 17;

            drawFont4(screen, x, y, 0, string.Format("{0:d1}", nd));
            od = nd;
        }

        public static void LfoFrqToHuC6280(FrameBuffer screen, ref int od, int nd)
        {
            if (od == nd) return;

            int x = 8 * 16;
            int y = 8 * 18;

            drawFont4(screen, x, y, 0, string.Format("{0:d3}", nd));
            od = nd;
        }

        public static void drawMIDILCD_Fader(FrameBuffer screen, int MIDImodule, int faderType, int x, int y, ref byte oldValue, byte value)
        {
            if (oldValue == value) return;
            oldValue = value;

            int v;
            switch (faderType)
            {
                case 0:
                    v = Math.Max(value - 8, 0) / 8;
                    drawMIDILCD_FaderP(screen, MIDImodule, 0, x, y, v);
                    break;
                case 1:
                    v = value / 8;
                    drawMIDILCD_FaderP(screen, MIDImodule, 1, x, y, v);
                    break;
            }

        }

        public static void drawMIDILCD_Fader(FrameBuffer screen, int MIDImodule, int faderType, int x, int y, ref short oldValue, short value)
        {
            if (oldValue == value) return;
            oldValue = value;

            int v;
            switch (faderType)
            {
                case 0:
                    v = Math.Max(value - 0x1ff, 0) / 0x3ff;
                    drawMIDILCD_FaderP(screen, MIDImodule, 0, x, y, v);
                    break;
                case 1:
                    break;
            }
        }

        public static void drawMIDILCD_Kbd(FrameBuffer screen, int x, int y, int note, ref byte oldVel, byte vel)
        {
            if (oldVel == vel) return;
            oldVel = vel;

            drawMIDILCD_KbdP(screen, x, y, note, vel);
        }

        public static void drawFont4MIDINotes(FrameBuffer screen, int x, int y, int t, ref string oldnotes, string notes)
        {
            if (oldnotes == notes) return;
            oldnotes = notes;

            if (screen == null) return;

            drawFont4(screen, x, y, t, notes);

            return;
        }

        public static void drawMIDI_Lyric(FrameBuffer screen, int chipID, int x, int y, ref string oldValue1, string value1)
        {
            //if (oldValue1 == value1) return;

            gMIDILyric[chipID].Clear(Color.Black);
            System.Windows.Forms.TextRenderer.DrawText(gMIDILyric[chipID], value1, fntMIDILyric[chipID], new Point(0, 0), Color.White);
            //byte[] bit = getByteArray(bitmapMIDILyric[chipID]);
            int[] bit = getIntArray(bitmapMIDILyric[chipID]);
            screen.drawIntArray(x, y, bit, 200, 0, 0, 200, 24);

            oldValue1 = value1;
        }

        public static void drawMIDI_MacroXG(FrameBuffer screen, int MIDImodule, int macroType, int x, int y, ref int oldValue1, int value1)
        {
            //if (oldValue1 == value1) return;

            drawFont4(screen, x, y, 2 + MIDImodule, Tables.tblMIDIEffectXG[macroType][value1]);

            oldValue1 = value1;
        }

        public static void drawMIDI_MacroGS(FrameBuffer screen, int MIDImodule, int macroType, int x, int y, ref int oldValue1, int value1)
        {
            //if (oldValue1 == value1) return;

            drawFont4(screen, x, y, 2 + MIDImodule, Tables.tblMIDIEffectGS[macroType][value1]);

            oldValue1 = value1;
        }

        public static void drawMIDILCD_Letter(FrameBuffer screen, int MIDImodule, int x, int y, ref byte[] oldValue, int len)
        {
            for (int i = 0; i < 16; i++)
            {
                if (oldValue[i] == Tables.spc[i]) continue;
                oldValue[i] = Tables.spc[i];

                if (screen == null) return;

                int cd = 0;
                //if (i < len) 
                cd = Tables.spc[i] - ' ';

                screen.drawIntArray(x + i * 8, y, rMIDILCD_Font[MIDImodule], 128, (cd % 16) * 8, (cd / 16) * 8, 8, 8);
            }

        }

        public static void drawMIDILCD_Letter(FrameBuffer screen, int MIDImodule, int x, int y, ref byte[] oldValue, byte[] value, int len)
        {
            for (int i = 0; i < 20; i++)
            {
                if (oldValue[i] == value[i]) continue;
                oldValue[i] = value[i];

                if (screen == null) return;

                int cd = 0;
                //if (i < len) 
                cd = value[i] - ' ';

                screen.drawIntArray(x + i * 8, y, rMIDILCD_Font[MIDImodule], 128, (cd % 16) * 8, (cd / 16) * 8, 8, 8);
            }

        }

        public static void drawFont4IntMIDI(FrameBuffer screen, int x, int y, int t, ref byte oldnum, byte num)
        {
            if (oldnum == num) return;
            oldnum = num;

            if (screen == null) return;

            int n;

            n = num / 100;
            num -= (byte)(n * 100);
            //n = (n > 9) ? 0 : n;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 10;
            num -= (byte)(n * 10);
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            return;
        }

        public static void drawFont4IntMIDIInstrument(FrameBuffer screen, int x, int y, int t, ref byte oldnum, byte num)
        {
            if (oldnum == num) return;
            oldnum = num;

            if (screen == null) return;

            drawFont4(screen, x, y + 8, t, Tables.tblMIDIInstrumentGM[num]);

            int n;

            n = num / 100;
            num -= (byte)(n * 100);
            //n = (n > 9) ? 0 : n;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 10;
            num -= (byte)(n * 10);
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            return;
        }

        public static void drawFader(FrameBuffer screen, int x, int y, int t, ref int od, int nd)
        {
            if (od == nd) return;

            drawFaderSlitP(screen, x, y - 8);
            drawFont4IntM(screen, x, y + 48, 3, nd);

            int n = 0;

            if (nd >= 0)
            {
                n = -(int)(nd / 20.0 * 8.0);
            }
            else
            {
                n = -(int)(nd / 192.0 * 35.0);
            }

            y += n;

            drawFaderP(screen, x, y, t);

            od = nd;
        }

        public static void drawGFader(FrameBuffer screen, int x, int y, int t, ref int od, int nd)
        {
            if (od == nd) return;

            drawFaderSlitP(screen, x, y - 8);
            drawFont4IntM(screen, x, y + 48, 3, nd);

            int n = 35 - (int)(nd / 127.0 * 43.0);
            y += n;

            drawFaderP(screen, x, y, t);

            od = nd;
        }

        public static void drawFaderCursor(FrameBuffer screen, int n, ref int od, int nd)
        {
            int x = (n % 16) * 20 + 5;
            int y = (n / 16) * 72 + 8;
            screen.drawBoxArray(x, y, 0xff, 1, 8, 56);
        }

        public static void MixerVolume(FrameBuffer screen, int x, int y, ref int od, int nd, ref int ov, int nv)
        {
            if (od == nd && ov == nv) return;

            for (int i = 0; i < 44; i++)
            {
                int t = i < 8 ? 0 : 1;
                if (i % 2 != 0) t = 2;
                else if (44 - i > nd) t = 2;

                drawMixerVolumeP(screen, x, y + i, t);
            }

            drawMixerVolumeP(screen, x, y + (44 - nv), nv > 36 ? 0 : 1);

            od = nd;
            ov = nv;
        }

        public static void KcYM2151(FrameBuffer screen, int ch, ref int ok, int nk)
        {
            if (ok == nk)
            {
                return;
            }

            int x = 78 * 4 +1;
            int y = ch * 8 + 8;
            drawFont4HexByte(screen, x, y, 0, nk);
            ok = nk;
        }

        public static void KfYM2151(FrameBuffer screen, int ch, ref int ok, int nk)
        {
            if (ok == nk)
            {
                return;
            }

            int x = 80 * 4 + 1;
            int y = ch * 8 + 8;
            drawFont4HexByte(screen, x, y, 0, nk);
            ok = nk;
        }

        public static void NeYM2151(FrameBuffer screen, ref int one, int nne)
        {
            if (one == nne)
            {
                return;
            }

            int x = 4 * 67+1;
            int y = 8 * 22;
            drawFont4Int(screen, x, y, 0, 1, nne);

            one = nne;
        }

        public static void NfrqYM2151(FrameBuffer screen, ref int onfrq, int nnfrq)
        {
            if (onfrq == nnfrq)
            {
                return;
            }

            int x = 4 * 67 + 1;
            int y = 8 * 23;
            drawFont4Int(screen, x, y, 0, 2, nnfrq);

            onfrq = nnfrq;
        }

        public static void LfrqYM2151(FrameBuffer screen, ref int olfrq, int nlfrq)
        {
            if (olfrq == nlfrq)
            {
                return;
            }

            int x = 4 * 66 + 1;
            int y = 8 * 24;
            drawFont4Int(screen, x, y, 0, 3, nlfrq);

            olfrq = nlfrq;
        }

        public static void AmdYM2151(FrameBuffer screen, ref int oamd, int namd)
        {
            if (oamd == namd)
            {
                return;
            }

            int x = 4 * 66 + 1;
            int y = 8 * 26;
            drawFont4Int(screen, x, y, 0, 3, namd);

            oamd = namd;
        }

        public static void PmdYM2151(FrameBuffer screen, ref int opmd, int npmd)
        {
            if (opmd == npmd)
            {
                return;
            }

            int x = 4 * 66 + 1;
            int y = 8 * 25;
            drawFont4Int(screen, x, y, 0, 3, npmd);

            opmd = npmd;
        }

        public static void WaveFormYM2151(FrameBuffer screen, ref int owaveform, int nwaveform)
        {
            if (owaveform == nwaveform)
            {
                return;
            }

            int x = 4 * 83+1;
            int y = 8 * 24;
            drawFont4Int(screen, x, y, 0, 1, nwaveform);

            owaveform = nwaveform;
        }

        public static void LfoSyncYM2151(FrameBuffer screen, ref int olfosync, int nlfosync)
        {
            if (olfosync == nlfosync)
            {
                return;
            }

            int x = 4 * 83+1;
            int y = 8 * 25;
            drawFont4Int(screen, x, y, 0, 1, nlfosync);

            olfosync = nlfosync;
        }

        public static void Tn(FrameBuffer screen, int x, int y, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawTnP(screen, x * 4, y * 4 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void TnOPNA(FrameBuffer screen, int x, int y, int c, ref int ot, int nt, ref int otp, int ntp)
        {

            if (ot == nt && otp == ntp)
            {
                return;
            }

            drawTnP(screen, x * 4 + 1, y * 4 + c * 8, nt, ntp);
            ot = nt;
            otp = ntp;
        }

        public static void flag16Bit(FrameBuffer screen, int x, int y, int t, ref int oi, int ni)
        {
            if (oi != ni)
            {
                drawFont4(screen, x + 0, y, t, (ni & 0x8000) == 0 ? "-" : "*");
                drawFont4(screen, x + 4, y, t, (ni & 0x4000) == 0 ? "-" : "*");
                drawFont4(screen, x + 8, y, t, (ni & 0x2000) == 0 ? "-" : "*");
                drawFont4(screen, x + 12, y, t, (ni & 0x1000) == 0 ? "-" : "*");
                drawFont4(screen, x + 16, y, t, (ni & 0x0800) == 0 ? "-" : "*");
                drawFont4(screen, x + 20, y, t, (ni & 0x0400) == 0 ? "-" : "*");
                drawFont4(screen, x + 24, y, t, (ni & 0x0200) == 0 ? "-" : "*");
                drawFont4(screen, x + 28, y, t, (ni & 0x0100) == 0 ? "-" : "*");
                drawFont4(screen, x + 32, y, t, (ni & 0x0080) == 0 ? "-" : "*");
                drawFont4(screen, x + 36, y, t, (ni & 0x0040) == 0 ? "-" : "*");
                drawFont4(screen, x + 40, y, t, (ni & 0x0020) == 0 ? "-" : "*");
                drawFont4(screen, x + 44, y, t, (ni & 0x0010) == 0 ? "-" : "*");
                drawFont4(screen, x + 48, y, t, (ni & 0x0008) == 0 ? "-" : "*");
                drawFont4(screen, x + 52, y, t, (ni & 0x0004) == 0 ? "-" : "*");
                drawFont4(screen, x + 56, y, t, (ni & 0x0002) == 0 ? "-" : "*");
                drawFont4(screen, x + 60, y, t, (ni & 0x0001) == 0 ? "-" : "*");
                oi = ni;
            }
        }

        public static void SUSFlag(FrameBuffer screen, int x, int y, int t, ref int oi, int ni)
        {
            if (oi != ni)
            {
                drawFont4(screen, x * 4, y * 4, t, ni == 0 ? "-" : "*");
                oi = ni;
            }
        }

        public static void Kakko(FrameBuffer screen, int x, int y, int t, ref int ot, int nt)
        {
            if (ot != nt)
            {
                screen.drawIntArray(x, y, rKakko, 16, nt * 4, 0, 4, 8);
                for (int n = 0; n < t; n++)
                {
                    screen.drawIntArray(x, y + n * 8 + 8, rKakko, 16, nt * 4, 8, 4, 8);
                }
                screen.drawIntArray(x, y + t * 8 + 8, rKakko, 16, nt * 4, 16, 4, 8);

                ot = nt;
            }
        }

        public static void OpxOP(FrameBuffer screen, int x, int y, int t, ref int ot, int nt)
        {
            if (ot != nt)
            {
                screen.drawIntArray(x, y, rType_YMF271, 32, nt * 8, 0, 8, 32);

                ot = nt;
            }
        }


        public static void LfoSw(FrameBuffer screen, int x, int y, ref bool olfosw, bool nlfosw)
        {
            if (olfosw == nlfosw)
            {
                return;
            }

            drawFont4(screen, x, y, 0, nlfosw ? "ON " : "OFF");

            olfosw = nlfosw;
        }

        public static void LfoFrq(FrameBuffer screen, int x, int y, ref int olfofrq, int nlfofrq)
        {
            if (olfofrq == nlfofrq)
            {
                return;
            }

            drawFont4Int(screen, x, y, 0, 1, nlfofrq);

            olfofrq = nlfofrq;
        }

        public static void NoteLogYM2612MIDI(FrameBuffer screen, int x, int y, ref int oln, int nln)
        {
            if (oln == nln) return;
            if (nln == -1)
            {
                drawFont4V(screen, x, y, 0, "   ");
            }
            else
            {
                drawFont4V(screen, x, y, 0, Tables.kbnp[nln % 12]);
                drawFont4V(screen, x, y - 2 * 4, 0, Tables.kbo[nln / 12]);
            }
            oln = nln;
        }

        public static void UseChannelYM2612MIDI(FrameBuffer screen, int x, int y, ref bool olm, bool nlm)
        {
            //if (olm == nlm) return;

            drawFont8(screen, x, y, 1, nlm ? "^" : "-");

            olm = nlm;
        }

        public static void MONOPOLYYM2612MIDI(FrameBuffer screen, ref bool olm, bool nlm)
        {
            if (olm == nlm) return;

            drawFont8(screen, 8, 16, 1, nlm ? "^" : "-");
            drawFont8(screen, 8, 24, 1, nlm ? "-" : "^");

            olm = nlm;
        }

        public static void ToneFormat(FrameBuffer screen, int x, int y, ref int oToneFormat, int nToneFormat)
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

        public static void drawChipName(FrameBuffer screen, int x, int y, int t, ref byte oc, byte nc)
        {
            if (oc == nc) return;

            drawChipNameP(screen, x, y, t, nc);

            oc = nc;
        }

        public static void drawTimer(FrameBuffer screen, int c, ref int ot1, ref int ot2, ref int ot3, int nt1, int nt2, int nt3)
        {
            if (ot1 != nt1)
            {
                //drawFont4Int2(mainScreen, 4 * 30 + c * 4 * 11, 0, 0, 3, nt1);
                DrawBuff.drawFont8Int2(screen, 8 * 5 - 16 + c * 8 * 11 + 1, 1, 0, 3, nt1);
                ot1 = nt1;
            }
            if (ot2 != nt2)
            {
                DrawBuff.drawFont8Int2(screen, 8 * 9 - 16 + c * 8 * 11 + 1, 1, 0, 2, nt2);
                //drawFont4Int2(mainScreen, 4 * 34 + c * 4 * 11, 0, 0, 2, nt2);
                ot2 = nt2;
            }
            if (ot3 != nt3)
            {
                DrawBuff.drawFont8Int2(screen, 8 * 12 - 16 + c * 8 * 11 + 1, 1, 0, 2, nt3);
                //drawFont4Int2(mainScreen, 4 * 37 + c * 4 * 11, 0, 0, 2, nt3);
                ot3 = nt3;
            }
        }

        public static void drawButtonP(FrameBuffer mainScreen, int x, int y, int t, int m)
        {
            if (mainScreen == null) return;

            int n = t % 18;
            t /= 18;
            switch (n)
            {
                case 0:
                    //setting
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 5 * 16, 1 * 16, 16, 16);
                    break;
                case 1:
                    //stop
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 0 * 16, 0 * 16, 16, 16);
                    break;
                case 2:
                    //pause
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 1 * 16, 0 * 16, 16, 16);
                    break;
                case 3:
                    //fadeout
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 4 * 16, 1 * 16, 16, 16);
                    break;
                case 4:
                    //PREV
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 6 * 16, 1 * 16, 16, 16);
                    break;
                case 5:
                    //slow
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 2 * 16, 0 * 16, 16, 16);
                    break;
                case 6:
                    //play
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 3 * 16, 0 * 16, 16, 16);
                    break;
                case 7:
                    //fast
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 4 * 16, 0 * 16, 16, 16);
                    break;
                case 8:
                    //NEXT
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 7 * 16, 1 * 16, 16, 16);
                    break;
                case 9:
                    //loopmode
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 1 * 16 + m * 16, 2 * 16, 16, 16);
                    break;
                case 10:
                    //folder
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 5 * 16, 0 * 16, 16, 16);
                    break;
                case 11:
                    //List
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 0 * 16, 2 * 16, 16, 16);
                    break;
                case 12:
                    //info
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 0 * 16, 1 * 16, 16, 16);
                    break;
                case 13:
                    //mixer
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 2 * 16, 1 * 16, 16, 16);
                    break;
                case 14:
                    //panel
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 5 * 16, 2 * 16, 16, 16);
                    break;
                case 15:
                    //VST List
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 7 * 16, 0 * 16, 16, 16);
                    break;
                case 16:
                    //MIDI Keyboard
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 3 * 16, 1 * 16, 16, 16);
                    break;
                case 17:
                    //zoom
                    mainScreen.drawIntArray(x, y, rMenuButtons[t], 128, 6 * 16, 2 * 16, 16, 16);
                    break;
            }
        }

        public static void drawButton(FrameBuffer mainScreen, int c, ref int ot, int nt, ref int om, int nm)
        {
            if (ot == nt && om == nm)
            {
                return;
            }

            drawFont8(mainScreen, 17 + c * 16, 9, 0, "  ");
            drawFont8(mainScreen, 17 + c * 16, 17, 0, "  ");
            drawButtonP(mainScreen, 17 + c * 16, 9, nt * 18 + c, nm);

            ot = nt;
            om = nm;
        }

        public static void drawButtons(FrameBuffer mainScreen, int[] oldButton, int[] newButton, int[] oldButtonMode, int[] newButtonMode)
        {

            for (int i = 0; i < newButton.Length; i++)
            {
                drawButton(mainScreen, i, ref oldButton[i], newButton[i], ref oldButtonMode[i], newButtonMode[i]);
            }

        }

        public static void drawDuty(FrameBuffer screen, int x, int y, ref int op, int np)
        {
            if (op == np) return;

            screen.drawIntArray(x, y, rNESDMC, 64, np * 8, 0, 8, 8);

            op = np;
        }

        public static void drawNESSw(FrameBuffer screen, int x, int y, ref bool os, bool ns)
        {
            if (os == ns) return;

            screen.drawIntArray(x, y, rNESDMC, 64, (ns ? 1 : 0) * 4 + 32, 0, 4, 8);

            os = ns;
        }

        public static void font4Int1(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Int1(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4Int2(FrameBuffer screen, int x, int y, int t, int k, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Int2(screen, x, y, t, k, nn);
            on = nn;
        }

        public static void font4Int3(FrameBuffer screen, int x, int y, int t, int k, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Int3(screen, x, y, t, k, nn);
            on = nn;
        }

        public static void font4Hex4Bit(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Hex4Bit(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4HexByte(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4HexByte(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4Hex12Bit(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Hex12Bit(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4Hex16Bit(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Hex16Bit(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4YM2609Duty(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4YM2609Duty(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4Hex20Bit(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Hex20Bit(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4Hex24Bit(FrameBuffer screen, int x, int y, int t, ref int on, int nn)
        {
            if (on == nn) return;

            drawFont4Hex24Bit(screen, x, y, t, nn);
            on = nn;
        }

        public static void font4Hex32Bit(FrameBuffer screen, int x, int y, int t, ref uint on, uint nn)
        {
            if (on == nn) return;

            drawFont4Hex32Bit(screen, x, y, t, nn);
            on = nn;
        }




        private static int[] getIntArray(Image img)
        {
            Bitmap bitmap = new Bitmap(img);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int[] intArray = new int[bitmapData.Stride * bitmap.Height / 4];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, intArray, 0, intArray.Length);
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();

            return intArray;
        }

        private static void VolumeP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawIntArray(x, y, rVol[tp], 32, 2 * t, 0, 2, 8 - (t / 4) * 4);
        }

        public static void drawKbn(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            switch (t)
            {
                case 0:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 0, 0, 4, 8);
                    break;
                case 1:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 4, 0, 3, 8);
                    break;
                case 2:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 8, 0, 4, 8);
                    break;
                case 3:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 12, 0, 4, 8);
                    break;
                case 4:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 0 + 16, 0, 4, 8);
                    break;
                case 5:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 4 + 16, 0, 3, 8);
                    break;
                case 6:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 8 + 16, 0, 4, 8);
                    break;
                case 7:
                    screen.drawIntArray(x, y, rKBD[tp], 32, 12 + 16, 0, 4, 8);
                    break;
            }
        }

        private static void ToneNoiseP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawIntArray(x, y, rPSGMode[tp], 32, 8 * t, 0, 8, 8);
        }

        public static void drawFont8(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null)
            {
                return;
            }

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawIntArray(x, y, rFont1[t], 128, (cd % 16) * 8, (cd / 16) * 8, 8, 8);
                x += 8;
            }
        }

        public static void drawFont8Int(FrameBuffer screen, int x, int y, int t, int k, int num)
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
                    screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawIntArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 8;
                if (n != 0 || f)
                {
                    screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawIntArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
                }

                n = num / 1;
                num -= n * 1;
                x += 8;
                screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
            }
            else
            {
                screen.drawIntArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
            }

            n = num / 1;
            num -= n * 1;
            x += 8;
            screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
        }

        public static void drawFont8Int2(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;

                n = (n > 9) ? 0 : n;
                if (n == 0) screen.drawIntArray(x, y, rFont1[t], 128, 0, 0, 8, 8);
                else screen.drawIntArray(x, y, rFont1[t], 128, 0, 8, 8, 8);

                n = num / 10;
                num -= n * 10;
                x += 8;
                screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);

                n = num / 1;
                x += 8;
                screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);

            n = num / 1;
            x += 8;
            screen.drawIntArray(x, y, rFont1[t], 128, n * 8, 8, 8, 8);
        }

        public static void drawFont4(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null) return;

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawIntArray(x, y, rFont2[t], 128, (cd % 32) * 4, (cd / 32) * 8, 4, 8);
                x += 4;
            }
        }

        private static void drawFont4Int(FrameBuffer screen, int x, int y, int t, int k, int num)
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
                    screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, 0, 0, 4, 8);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, 0, 0, 4, 8);
                }

                n = num / 1;
                x += 4;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
            }
            else
            {
                screen.drawIntArray(x, y, rFont2[t], 128, 0, 0, 4, 8);
            }

            n = num / 1;
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
        }

        public static void drawFont4IntM(FrameBuffer screen, int x, int y, int k, int num)
        {
            if (screen == null) return;

            int t = 0;
            int n;

            if (num < 0)
            {
                num = -num;
                screen.drawIntArray(x - 4, y, rFont2[t], 128, 52, 1, 4, 7);
            }
            else
            {
                if (num != 0) t = 1;
                screen.drawIntArray(x - 4, y, rFont2[t], 128, 24, 1, 4, 7);
            }

            if (k == 3)
            {
                bool f = false;
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                if (n != 0)
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, 0, 1, 4, 7);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawIntArray(x, y, rFont2[t], 128, 0, 1, 4, 7);
                }

                n = num / 1;
                x += 4;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
            }
            else
            {
                screen.drawIntArray(x, y, rFont2[t], 128, 0, 1, 4, 7);
            }

            n = num / 1;
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
        }

        public static void drawFont4Int1(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            n = num % 10;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
        }

        public static void drawFont4Int2(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                screen.drawIntArray(x, y, rFont2[t], 128, (n * 4 + 64), 0, 4, 8);

                n = num / 10;
                num -= n * 10;
                x += 4;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

                n = num / 1;
                x += 4;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
        }

        public static void drawFont4Int3(FrameBuffer screen, int x, int y, int t, int k, int num)
        {
            if (screen == null) return;

            int n;
            if (k == 3)
            {
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

                n = num / 10;
                num -= n * 10;
                x += 4;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

                n = num / 1;
                x += 4;
                screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawIntArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);
        }

        public static void drawFont4Hex4Bit(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            num = Common.Range((byte)num, 0, 15);

            n = num;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4HexByte(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            num = Common.Range((byte)num, 0, 255);

            n = num / 0x10;
            num -= n * 0x10;
            n = (n > 0xf) ? 0 : n;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 1;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4Hex12Bit(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            num = Common.Range((ushort)num, 0, 0xfff);

            n = num / 0x100;
            num -= n * 0x100;
            n = (n > 0xf) ? 0 : n;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x10;
            num -= n * 0x10;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 1;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4Hex16Bit(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            num = Common.Range((ushort)num, 0, 0xffff);

            n = num / 0x1000;
            num -= n * 0x1000;
            n = (n > 0xf) ? 0 : n;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x100;
            num -= n * 0x100;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x10;
            num -= n * 0x10;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 1;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4YM2609Duty(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (num == 0)
            {
                drawFont4(screen, x, y, t, "SQ.W ");
            }
            else if (num < 8)
            {
                drawFont4(screen, x, y, t, String.Format("DT{0}/8", 8 - num));
            }
            else if (num == 8)
            {
                drawFont4(screen, x, y, t, "TRI. ");
            }
            else if (num == 9)
            {
                drawFont4(screen, x, y, t, "SAW  ");
            }
            else
            {
                drawFont4(screen, x, y, t, "USER ");
            }
        }

        public static void drawFont4Hex20Bit(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            num = Common.Range((ushort)num, 0, 0xf_ffff);

            n = num / 0x1_0000;
            num -= n * 0x1_0000;
            n = (n > 0xf) ? 0 : n;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x1000;
            num -= n * 0x1000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x100;
            num -= n * 0x100;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x10;
            num -= n * 0x10;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 1;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4Hex24Bit(FrameBuffer screen, int x, int y, int t, int num)
        {
            if (screen == null) return;

            int n;
            num = Common.Range((int)num, 0, 0xff_ffff);

            n = num / 0x10_0000;
            num -= n * 0x10_0000;
            n = (n > 0xf) ? 0 : n;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x1_0000;
            num -= n * 0x1_0000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x1000;
            num -= n * 0x1000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x100;
            num -= n * 0x100;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x10;
            num -= n * 0x10;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 1;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4Hex32Bit(FrameBuffer screen, int x, int y, int t, uint num)
        {
            if (screen == null) return;

            uint n;
            num = Common.Range(num, 0, 0xffff_ffff);

            n = num / 0x1000_0000;
            num -= n * 0x1000_0000;
            n = (n > 0xf) ? 0 : n;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x100_0000;
            num -= n * 0x100_0000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x10_0000;
            num -= n * 0x10_0000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x1_0000;
            num -= n * 0x1_0000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x1000;
            num -= n * 0x1000;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x100;
            num -= n * 0x100;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 0x10;
            num -= n * 0x10;
            n = (n > 0xf) ? 0 : n;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            n = num / 1;
            x += 4;
            drawFont4(screen, x, y, t, Tables.hexCh[n]);

            return;
        }

        public static void drawFont4V(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null) return;

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawIntArray(x, y, rFont3[t], 128, (cd % 16) * 8, (cd / 16) * 4, 8, 4);
                y -= 4;
            }
        }

        private static void drawEtypeP(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null) return;
            screen.drawIntArray(x, y, rPSGEnv, 128, 8 * t, 0, 8, 8);
            drawFont4Int2(screen, x + 12, y, 0, 2, t);
        }

        public static void drawPanP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawIntArray(x, y, rPan[tp], 32, 8 * t, 0, 8, 8);
        }

        public static void drawPanType2P(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            int p = (t & 0x0f);
            p = p == 0 ? 0 : (1 + p / 4);
            screen.drawIntArray(x, y, rPan2[tp], 32, p * 4, 0, 4, 8);
            p = ((t & 0xf0) >> 4);
            p = p == 0 ? 0 : (1 + p / 4);
            screen.drawIntArray(x + 4, y, rPan2[tp], 32, p * 4, 0, 4, 8);

        }

        public static void drawPanType3P(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            int p = (t & 0x0f);
            p = p == 0 ? 0 : ((p + 1) / 4);
            screen.drawIntArray(x, y, rPan2[tp], 32, p * 4, 0, 4, 8);
            p = ((t & 0xf0) >> 4);
            p = p == 0 ? 0 : ((p + 1) / 4);
            screen.drawIntArray(x + 4, y, rPan2[tp], 32, p * 4, 0, 4, 8);

        }

        public static void drawPanType4P(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            int p = t / 5;
            screen.drawIntArray(x, y, rPan2[tp], 32, p * 4, 0, 4, 8);
            p = t % 5;
            screen.drawIntArray(x + 4, y, rPan2[tp], 32, p * 4, 0, 4, 8);

        }

        public static void drawPanType5P(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            int p = t;
            screen.drawIntArray(x, y, rPan2[tp], 32, p * 4, 0, 4, 8);
        }

        public static void drawPanType6P(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null)
            {
                return;
            }

            int p = 0;
            if ((t & 0x20) != 0)
            {
                p = 4 - ((t & 0x18) >> 3);
            }
            screen.drawIntArray(x, y, rPan2[tp], 32, p * 4, 0, 4, 8);

            p = 0;
            if ((t & 0x04) != 0)
            {
                p = 4 - ((t & 0x03) >> 0);
            }
            screen.drawIntArray(x + 4, y, rPan2[tp], 32, p * 4, 0, 4, 8);

        }


        private static void drawMIDILCD_FaderP(FrameBuffer screen, int MIDImodule, int faderType, int x, int y, int value)
        {
            screen.drawIntArray(x, y, rMIDILCD_Fader[MIDImodule], 64, value * 4, faderType * 16, 4, 16);
        }

        private static void drawMIDILCD_KbdP(FrameBuffer screen, int x, int y, int note, int vel)
        {
            screen.drawByteArrayTransp(x + Tables.kbdl[note % 12] + note / 12 * 28, y, rMIDILCD_KBD, 16, Tables.kbl2[note % 12], vel / 16 * 8, 4, 8);
        }

        private static void ChAY8910_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChS5B_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public static void ChC140_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            //if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            //else 
            drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
        }

        public static void ChQSound_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            //if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            //else 
            drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
        }
        public static void ChQSoundAdpcm_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 88, 0, 20, 8);
            //if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            //else 
            drawFont4(screen, x + 20, y, mask ? 1 : 0, (1 + ch).ToString("d1"));
        }

        public static void ChC352_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            //if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            //else
            drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
        }

        public static void ChYMZ280B_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d1"));
        }

        private static void ChHuC6280_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 112, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChOKIM6295_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 0, 24, 8);
            drawFont8(screen, x + 24, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChPCM8_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 0, 24, 8);
            drawFont4Int2(screen, x + 24, y, mask ? 1 : 0, 2, 1 + ch);
        }

        private static void ChMPCMX68k_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 0, 24, 8);
            drawFont4Int2(screen, x + 24, y, mask ? 1 : 0, 2, 1 + ch);
        }

        private static void ChPPZ8_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            drawFont4Int2(screen, x + 16, y, mask ? 1 : 0, 2, 1 + ch);
        }

        private static void ChK051649_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 112, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChRF5C164_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
        }

        private static void ChOKIM6258_P(FrameBuffer screen, int x, int y, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 8 * 8, 0, 24, 8);
        }

        public static void ChSegaPCM_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            //if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            //else
            drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
        }

        private static void ChSN76489_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        public static void ChYM2151_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChYM2203_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 3)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 6)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch - 3).ToString());
            }
            else if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32 * (ch - 5), 24, 32, 8);
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 24, 8);
            }
        }

        private static void ChYM2413_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else
            {
                switch (ch)
                {
                    case 9:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 10:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 11:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 12:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 3 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 13:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
        }

        private static void ChYM3526_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 14)
            {
                switch (ch)
                {
                    case 9:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 10:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 11:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 12:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 0 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 13:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
        }

        private static void ChY8950_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 14)
            {
                switch (ch)
                {
                    case 9:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 10:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 11:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 12:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 0 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 13:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 0, 24, 8);
            }
        }

        private static void ChYM3812_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 14)
            {
                switch (ch)
                {
                    case 9:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 10:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 11:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 12:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 0 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 13:
                        drawFont4(screen, (ch - 9) * 4 * 15 + 1 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
        }

        private static void ChYMF262_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 18)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                //if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
                //else
                drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
            }
            else if (ch < 23)
            {
                switch (ch)
                {
                    case 18:
                        drawFont4(screen, (ch - 18) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 19:
                        drawFont4(screen, (ch - 18) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 20:
                        drawFont4(screen, (ch - 18) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 21:
                        drawFont4(screen, (ch - 18) * 4 * 15 + 3 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 22:
                        drawFont4(screen, (ch - 18) * 4 * 15 + 4 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
        }

        private static void ChYMF278B_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 18)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
            }
            else if (ch < 23)
            {
                switch (ch)
                {
                    case 18:
                        drawFont4(screen, (ch - 18) * 4 * 13 + 10 * 4, y, mask ? 1 : 0, "BD");
                        break;
                    case 19:
                        drawFont4(screen, (ch - 18) * 4 * 13 + 10 * 4, y, mask ? 1 : 0, "SD");
                        break;
                    case 20:
                        drawFont4(screen, (ch - 18) * 4 * 13 + 10 * 4, y, mask ? 1 : 0, "TM");
                        break;
                    case 21:
                        drawFont4(screen, (ch - 18) * 4 * 13 + 9 * 4, y, mask ? 1 : 0, "CYM");// 3 character
                        break;
                    case 22:
                        drawFont4(screen, (ch - 18) * 4 * 13 + 10 * 4, y, mask ? 1 : 0, "HH");
                        break;
                }
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                ch -= 23;
                drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString("d2"));
            }
        }

        private static void ChYM2608_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 6)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32 * (ch - 8), 24, 32, 8);
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 0, 24, 8);
            }
        }

        private static void ChYM2608Rhythm_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;
            switch (ch)
            {
                case 0:
                    drawFont4(screen, x + 0 * 4, y, mask ? 1 : 0, "B");
                    break;
                case 1:
                    drawFont4(screen, x + 15 * 4, y, mask ? 1 : 0, "S");
                    break;
                case 2:
                    drawFont4(screen, x + 30 * 4, y, mask ? 1 : 0, "C");
                    break;
                case 3:
                    drawFont4(screen, x + 45 * 4, y, mask ? 1 : 0, "H");
                    break;
                case 4:
                    drawFont4(screen, x + 60 * 4, y, mask ? 1 : 0, "T");
                    break;
                case 5:
                    drawFont4(screen, x + 75 * 4, y, mask ? 1 : 0, "R");
                    break;
            }
        }

        public static void ChYM2610_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            if (ch < 6)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            }
            else if (ch < 9)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch - 6).ToString());
            }
            else if (ch < 12)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32 * (ch - 8), 24, 32, 8);
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 88, 0, 24, 8);
            }
        }

        private static void ChYM2610Rhythm_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            switch (ch)
            {
                case 0:
                    drawFont4(screen, x + 15 * 0 * 4, y, mask ? 1 : 0, "A1");
                    break;
                case 1:
                    drawFont4(screen, x + 15 * 1 * 4 + 4, y, mask ? 1 : 0, "2");
                    break;
                case 2:
                    drawFont4(screen, x + 15 * 2 * 4 + 4, y, mask ? 1 : 0, "3");
                    break;
                case 3:
                    drawFont4(screen, x + 15 * 3 * 4 + 4, y, mask ? 1 : 0, "4");
                    break;
                case 4:
                    drawFont4(screen, x + 15 * 4 * 4 + 4, y, mask ? 1 : 0, "5");
                    break;
                case 5:
                    drawFont4(screen, x + 15 * 5 * 4 + 4, y, mask ? 1 : 0, "6");
                    break;
            }
        }

        private static void Ch6YM2612_P(FrameBuffer screen, int x, int y, int m, bool mask, int tp)
        {
            if (m == 0)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, "6");
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                drawFont8(screen, x + 16, y, 0, " ");
            }
        }

        private static void ChYM2612_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (ch == 5)
            {
                return;
            }

            if (ch < 5)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
            }
            else if (ch < 10)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32 * (ch - 5), 24, 32, 8);
            }
        }

        private static void Ch3YM2612_P(FrameBuffer screen, int x, int y, int ch, bool mask, bool ex, int tp)
        {
            if (!ex)
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
            }
            else
            {
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 24, 24, 8);
            }
        }

        private static void Ch6YM2612XGM_P(FrameBuffer screen, int x, int y, int m, bool mask, bool mask1, bool mask2, bool mask3, bool mask4, int tp)
        {
            if (m == 0)
            {
                //FM mode

                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, "6");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 33 + kx, y, kt, tp);
                }
            }
            else
            {
                //PCM mode

                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                drawFont8(screen, x + 16, y, 0, " ");
                drawFont4(screen, x + 32, y, 0, " 1C00             2C00             3C00             4C00                ");
                drawFont4(screen, x + 36 + 17 * 4 * 0, y, mask1 ? 1 : 0, "1C");
                drawFont4(screen, x + 36 + 17 * 4 * 1, y, mask2 ? 1 : 0, "2C");
                drawFont4(screen, x + 36 + 17 * 4 * 2, y, mask3 ? 1 : 0, "3C");
                drawFont4(screen, x + 36 + 17 * 4 * 3, y, mask4 ? 1 : 0, "4C");
            }
        }

        private static void Ch6YM2612XGM2_P(FrameBuffer screen, int x, int y, int m, bool mask, int tp)
        {
            if (m == 0)
            {
                //FM mode

                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 0, 16, 8);
                drawFont8(screen, x + 16, y, mask ? 1 : 0, "6");
                for (int i = 0; i < 96; i++)
                {
                    int kx = Tables.kbl[(i % 12) * 2] + i / 12 * 28;
                    int kt = Tables.kbl[(i % 12) * 2 + 1];
                    drawKbn(screen, 33 + kx, y, kt, tp);
                }
            }
            else
            {
                //PCM mode
                screen.drawIntArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                drawFont8(screen, x + 16, y, 0, " ");
                drawFont4(screen, x + 32, y, 0, "  1C00                   2C00                   3C00                        ");
            }
        }

        private static void ChNESDMC_P(FrameBuffer screen, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            switch (ch)
            {
                case 0:
                    screen.drawIntArray(0, 8, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 8, mask ? 1 : 0, "1");
                    break;
                case 1:
                    screen.drawIntArray(0, 24, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 24, mask ? 1 : 0, "2");
                    break;
                case 2:
                    screen.drawIntArray(0, 40, rType[tp * 2 + (mask ? 1 : 0)], 128, 64, 8, 32, 8);
                    break;
                case 3:
                    screen.drawIntArray(112, 32, rType[tp * 2 + (mask ? 1 : 0)], 128, 96, 8, 24, 8);
                    break;
                case 4:
                    screen.drawIntArray(112, 48, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 16, 16, 8);
                    break;
            }
        }

        private static void ChFDS_P(FrameBuffer screen, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(0, 8, rType[tp * 2 + (mask ? 1 : 0)], 128, 14 * 8, 0 * 8, 16, 8);
        }

        private static void ChMMC5_P(FrameBuffer screen, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            switch (ch)
            {
                case 0:
                    screen.drawIntArray(0, 8, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 8, mask ? 1 : 0, "1");
                    break;
                case 1:
                    screen.drawIntArray(0, 24, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 24, mask ? 1 : 0, "2");
                    break;
                case 2:
                    screen.drawIntArray(112, 32, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
                    break;
            }
        }

        private static void ChDMG_P(FrameBuffer screen, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            switch (ch)
            {
                case 0:
                    screen.drawIntArray(0, 8, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 8, mask ? 1 : 0, "1");
                    break;
                case 1:
                    screen.drawIntArray(0, 16, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 16, mask ? 1 : 0, "2");
                    break;
                case 2:
                    screen.drawIntArray(0, 24, rType[tp * 2 + (mask ? 1 : 0)], 128, 112, 0, 16, 8);
                    break;
                case 3:
                    screen.drawIntArray(0, 32, rType[tp * 2 + (mask ? 1 : 0)], 128, 96, 8, 24, 8);
                    break;
            }
        }

        private static void ChVRC6_P(FrameBuffer screen, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            switch (ch)
            {
                case 0:
                    screen.drawIntArray(0, 8, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 8, mask ? 1 : 0, "1");
                    break;
                case 1:
                    screen.drawIntArray(0, 24, rType[tp * 2 + (mask ? 1 : 0)], 128, 48, 8, 16, 8);
                    drawFont8(screen, 16, 24, mask ? 1 : 0, "2");
                    break;
                case 2:
                    screen.drawIntArray(0, 40, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 16, 16, 8);
                    break;
            }
        }

        private static void ChN163_P(FrameBuffer screen, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawIntArray(0, ch * 8 * 3 + 8, rType[tp * 2 + (mask ? 1 : 0)], 128, 112, 0, 16, 8);
            drawFont8(screen, 16, ch * 8 * 3 + 8, mask ? 1 : 0, (ch + 1).ToString());
        }


        private static void drawFaderSlitP(FrameBuffer screen, int x, int y)
        {
            screen.drawIntArray(x, y, rFader, 32, 16, 0, 8, 8);
            screen.drawIntArray(x, y + 8, rFader, 32, 16, 8, 8, 8);
            screen.drawIntArray(x, y + 16, rFader, 32, 16, 8, 8, 8);
            screen.drawIntArray(x, y + 24, rFader, 32, 16, 8, 8, 8);
            screen.drawIntArray(x, y + 32, rFader, 32, 16, 8, 8, 8);
            screen.drawIntArray(x, y + 40, rFader, 32, 16, 8, 8, 8);
            screen.drawIntArray(x, y + 48, rFader, 32, 24, 0, 8, 8);
        }

        public static void drawFaderH(FrameBuffer screen, int x, int y,int d, int v, int val1, int val2,ref int od,ref int ov, ref int oval1, ref int oval2)
        {
            if (d == od && v == ov && val1 == oval1 && val2 == oval2)
            {
                return;
            }

            od = d;
            ov = v;
            oval1 = val1;
            oval2 = val2;

            drawFaderHP(screen, x, y, 2, v);
            for (int i = 0; i < 7 * 8+1; i++)
            {
                drawFaderHP(screen, x + 1 + i, y, (i < val2 ? (v == 0 ? 4 : 5) : 3), v);
            }
            drawFaderHP(screen, x + 2 + 7 * 8, y, 2, v);

            drawFaderHP(screen, x + val1 + 1, y, d, v);
        }

        private static void drawFaderHP(FrameBuffer screen, int x, int y, int c, int v)
        {
            c += v * 6;
            switch (c)
            {
                case 0:
                    screen.drawIntArray(x - 1, y, rFaderH, 32, 0, 0, 3, 6);
                    break;
                case 1:
                    screen.drawIntArray(x - 1, y, rFaderH, 32, 3, 0, 3, 6);
                    break;
                case 2:
                    screen.drawIntArray(x, y, rFaderH, 32, 6, 0, 1, 6);
                    break;
                case 3:
                    screen.drawIntArray(x, y, rFaderH, 32, 7, 0, 1, 6);
                    break;
                case 4:
                    screen.drawIntArray(x, y, rFaderH, 32, 8, 0, 1, 6);
                    break;
                case 5:
                    screen.drawIntArray(x, y, rFaderH, 32, 9, 0, 1, 6);
                    break;
                case 6:
                    screen.drawIntArray(x - 1, y, rFaderH, 32, 0, 8, 3, 6);
                    break;
                case 7:
                    screen.drawIntArray(x - 1, y, rFaderH, 32, 3, 8, 3, 6);
                    break;
                case 8:
                    screen.drawIntArray(x, y, rFaderH, 32, 6, 8, 1, 6);
                    break;
                case 9:
                    screen.drawIntArray(x, y, rFaderH, 32, 7, 8, 1, 6);
                    break;
                case 10:
                    screen.drawIntArray(x, y, rFaderH, 32, 8, 8, 1, 6);
                    break;
                case 11:
                    screen.drawIntArray(x, y, rFaderH, 32, 9, 8, 1, 6);
                    break;
            }
        }

        private static void drawFaderP(FrameBuffer screen, int x, int y, int t)
        {
            screen.drawIntArray(x, y, rFader, 32, t == 0 ? 0 : 8, 0, 8, 13);
        }

        private static void drawMixerVolumeP(FrameBuffer screen, int x, int y, int t)
        {
            screen.drawIntArray(x, y, rFader, 32, 24, 8 + t, 2, 1);
        }

        private static void drawTnP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawIntArray(x, y, rPSGMode[tp], 32, 8 * t, 0, 8, 8);
        }

        private static void drawToneFormatP(FrameBuffer screen, int x, int y, int toneFormat)
        {
            screen.drawIntArray(x, y, rMenuButtons[1], 128, (toneFormat % 3) * 5 * 8, (6 + toneFormat / 3) * 8, 40, 8);
        }

        private static void drawChipNameP(FrameBuffer screen, int x, int y, int t, int c)
        {
            if (screen == null)
            {
                return;
            }

            screen.drawIntArray(x, y, rChipName[c], 128
                , (t % 8) * 16
                , (t / 8) * 8
                , 8 * 2
                , 8);

        }




    }
}
