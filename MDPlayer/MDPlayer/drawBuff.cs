using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public static class DrawBuff
    {


        private static byte[][] rChipName;
        private static byte[][] rFont1;
        private static byte[][] rFont2;
        private static byte[][] rFont3;
        private static byte[][] rKBD;
        private static byte[][] rMenuButtons;
        private static byte[][] rPan;
        private static byte[][] rPan2;
        private static byte[] rPSGEnv;
        private static byte[][] rPSGMode;
        private static byte[][] rType;
        private static byte[][] rVol;
        private static byte[] rWavGraph;
        private static byte[] rFader;
        private static byte[][] rMIDILCD_Fader;
        private static byte[] rMIDILCD_KBD;
        private static byte[][] rMIDILCD_Vol;
        public static byte[][] rMIDILCD;
        private static byte[][] rMIDILCD_Font;
        public static byte[][] rPlane_MIDI;
        private static Bitmap[] bitmapMIDILyric = null;
        private static Graphics[] gMIDILyric = null;
        private static Font[] fntMIDILyric = null;

        private static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        private static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        public static string[] kbns = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        private static string[] kbnp = new string[] { "C ", "C+", "D ", "D+", "E ", "F ", "F+", "G ", "G+", "A ", "A+", "B " };
        private static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
        private static int[] kbl2 = new int[] { 0, 12, 4, 12, 8, 0, 12, 4, 12, 4, 12, 8 };
        private static int[] kbdl = new int[] { 0, 2, 4, 6, 8, 12, 14, 16, 18, 20, 22, 24 };

        private static string[][] tblMIDIEffectGS = new string[4][] {
            new string[] { "Room 1         " , "Room 2         " , "Room 3         " , "Hall 1         "
                         , "Hall 2         " , "Plate          " , "Delay          " , "Panning Delay  " },
            new string[] { "Chorus 1       " , "Chorus 2       " , "Chorus 3       " , "Chorus 4       "
                         , "Feedback Chorus" , "Flanger        " , "Short Delay    " , "ShortDelay(FB) " },
            new string[] { "Delay 1        " , "Delay 2        " , "Delay 3        " , "Delay 4        "
                         , "Pan Delay 1    " , "Pan Delay 2    " , "Pan Delay 3    " , "Pan Delay 4    "
                         , "Delay to Reverb" , "Pan Repeat     " },
            new string[] { "Thru           " , "Stereo-EQ      " , "Spectrum       " , "Enhancer       "
                         , "Humanizer      " , "Overdrive      " , "Distortion     " , "Phaser         "
                         , "Auto Wah       " , "Rotary         " , "Stereo Flanger " , "Step Flanger   "
                         , "Tremolo        " , "Auto Pan       " , "Compressor     " , "Limiter        "
                         , "Hexa Chorus    " , "Tremolo Chorus " , "Stereo Chorus  " , "Space D        "
                         , "3D Chorus      " , "Stereo Delay   " , "Mod Delay      " , "3 Tap Delay    "
                         , "4 Tap Delay    " , "Tm Ctrl Delay  " , "Reverb         " , "Gate Reverb    "
                         , "3D Delay       " , "2 Pitch Shifter" , "Fb P.Shifter   " , "3D Auto        "
                         , "3D Manual      " , "Lo-Fi 1        " , "Lo-Fi 2        " , "OD>Chorus      "
                         , "OD>Flanger     " , "OD>Delay       " , "DS>Chorus      " , "DS>Flanger     "
                         , "DS>Delay       " , "EH>Chorus      " , "EH>Flanger     " , "EH>Delay       "
                         , "Cho>Delay      " , "FL>Delay       " , "Cho>Flanger    " , "Rotary Multi   "
                         , "GTR Multi 1    " , "GTR Multi 2    " , "GTR Multi 3    " , "Clean GtMulti 1"
                         , "Clean GtMulti 2" , "Bass Multi     " , "Rhodes Multi   " , "Keyboard Multi "
                         , "Cho/Delay      " , "FL/Dealy       " , "Cho/Flanger    " , "OD1/OD2        "
                         , "OD/Rotary      " , "OD/Phaser      " , "OD/AutoWah     " , "PH/Rotary      "
                         , "PH/AutoWah     "
            }
        };

        private static string[][] tblMIDIEffectXG = new string[3][] {
            new string[] {
                           "NO EFFECT                 " , "HALL 1                    " , "HALL 2                    " , "HALL M                    "
                         , "HALL L                    " , "ROOM 1                    " , "ROOM 2                    " , "ROOM 3                    "
                         , "ROOM S                    " , "ROOM M                    " , "ROOM L                    " , "STAGE1                    "
                         , "STAGE2                    " , "PLATE                     " , "GMPLATE                   " , "WHITEROOM                 "
                         , "TUNNEL                    " , "CANYON                    " , "BASEMENT                  "
            } ,
            new string[] {
                           "NO EFFECT                 " , "CHORUS1                   " , "CHORUS2                   " , "CHORUS3                   "
                         , "CHORUS4                   " , "GMCHORUS 1                " , "GMCHORUS 2                " , "GMCHORUS 3                "
                         , "GMCHORUS 4                " , "FB CHORUS                 " , "CELESTE1                  " , "CELESTE2                  "
                         , "CELESTE3                  " , "CELESTE4                  " , "FLANGER 1                 " , "FLANGER 2                 "
                         , "FLANGER 3                 " , "GMFLANGER                 " , "SYMPHONIC                 " , "PHASER 1                  "
                         , "ENSEMBLEDETUNE            "
            } ,
            new string[] {
                           "NO EFFECT                 " , "HALL1                     " , "HALL2                     " , "HALL M                    "
                         , "HALL L                    " , "ROOM1                     " , "ROOM2                     " , "ROOM3                     "
                         , "ROOM S                    " , "ROOM M                    " , "ROOM L                    " , "STAGE1                    "
                         , "STAGE2                    " , "PLATE                     " , "GM PLATE                  " , "DELAY L,C,R               "
                         , "DELAY L,R                 " , "ECHO                      " , "CROSSDELAY                " , "ER1                       "
                         , "ER2                       " , "GATE REVERB               " , "REVERSE GATE              " , "WHITE ROOM                "
                         , "TUNNEL                    " , "CANYON                    " , "BASEMENT                  " , "KARAOKE1                  "
                         , "KARAOKE2                  " , "KARAOKE3                  " , "CHORUS1                   " , "CHORUS2                   "
                         , "CHORUS3                   " , "CHORUS4                   " , "GMCHORUS 1                " , "GMCHORUS 2                "
                         , "GMCHORUS 3                " , "GMCHORUS 4                " , "FB CHORUS                 " , "CELESTE1                  "
                         , "CELESTE2                  " , "CELESTE3                  " , "CELESTE4                  " , "FLANGER 1                 "
                         , "FLANGER 2                 " , "FLANGER 3                 " , "GMFLANGER                 " , "SYMPHONIC                 "
                         , "ROTARYSP.                 " , "DIST+ROTARYSP.            " , "OVERDRIVE+ROTARYSP.       " , "AMPSIM.+ROTARY            "
                         , "TREMOLO                   " , "AUTO PAN                  " , "PHASER1                   " , "PHASER2                   "
                         , "DISTORTION                " , "COMP+DISTORTION           " , "STEREODISTORTION          " , "OVERDRIVE                 "
                         , "STEREOOVERDRIVE           " , "AMPSIM.                   " , "STEREOAMPSIM.             " , "3BANDEQ                   "
                         , "2BANDEQ                   " , "AUTO WAH                  " , "AUTO WAH+DIST             " , "AUTO WAH+OVERDRIVE        "
                         , "PITCH CHANGE              " , "PITCH CHANGE2             " , "HARMONIC ENHANCER         " , "TOUCHWAH 1                "
                         , "TOUCHWAH 2                " , "TOUCHWAH+DIST             " , "TOUCHWAH+OVERDRIVE        " , "COMPRESSOR                "
                         , "NOISEGATE                 " , "VOICECANCEL               " , "2WAY ROTARY SP            " , "DIST. + 2WAYROTARY SP.    "
                         , "OVERDRIVE + 2WAY ROTARYSP." , "AMPSIM. + 2WAY ROTARYSP.  " , "ENSEMBLE DETUNE           " , "AMBIENCE                  "
                         , "TALKING MODULATION        " , "LO-FI                     " , "DIST+DELAY                " , "OVERDRIVE+DELAY           "
                         , "COMP+DIST+DELAY           " , "COMP+OVERDRIVE+DELAY      " , "WAH+DIST+DELAY            " , "WAH+OVERDRIVE+DELAY       "
                         , "V DISTORTION HARD         " , "V DISTORTION HARD+DELAY   " , "V DISTORTION SOFT         " , "V DISTORTION SOFT+DELAY   "
                         , "DUAL ROTOR SPEAKER1       " , "DUAL ROTOR SPEAKER2       " , "THRU                      "
            }
        };

        private static string[] tblMIDIInstrumentGM = new string[] {
                         "G.Piano  ","B.Piano  ","E.Piano  ","Honkytonk"
                        ,"E.Piano1 ","E.Piano2 ","Harpschrd","Clavi    "
                        ,"Celesta  ","Glocken  ","Music Box","Vibraphon"
                        ,"Marimba  ","Xylophone","Tblarbell","Dulcimer "
                        ,"D.Organ  ","P.Organ  ","R.Organ  ","ChrchOrgn"
                        ,"Reed Orgn","Accordion","Harmonica","T.Accrdon"
                        ,"NylonGt. ","SteelGt. ","JazzGt.  ","CleanGt. "
                        ,"MutedGt. ","Overd.Gt.","Dist.Gt. ","Harmo.Gt."
                        ,"A.Bass   ","FingrBass","PickBass ","FrtlBass "
                        ,"SlapBass1","SlapBass2","Syn.Bass1","Syn.Bass2"
                        ,"Violin   ","Viola    ","Cello    ","Cntrabass"
                        ,"TremlStr.","PizzStr. ","Harp     ","Timpani  "
                        ,"Strings1 ","Strings2 ","Syn.Str1 ","Syn.Str2 "
                        ,"ChoirAahs","VoiceOohs","SynVoice ","OrchHit  "
                        ,"Trumpet  ","Trombone ","Tuba     ","MtTrumpet"
                        ,"Fr. Horn ","BrassSec.","Syn.Brs1 ","Syn.Brs2 "
                        ,"SoprnoSax","AltoSax  ","TenorSax ","BartnSax "
                        ,"Oboe     ","Eng.Horn ","Bassoon  ","Clarinet "
                        ,"Piccolo  ","Flute    ","Recorder ","PanFlute "
                        ,"BlowBttle","Shakuhach","Whistle  ","Ocarina  "
                        ,"Square   ","Saw      ","Calliope ","Chiff    "
                        ,"Charang  ","Voice    ","5thSaw   ","Bassoon  "
                        ,"NewAge   ","Warm     ","Polysynth","Choir    "
                        ,"BowedGlss","MetalPad ","Halo     ","Sweep    "
                        ,"IceRain  ","Soundtrk ","Crystal  ","Atmsphere"
                        ,"Brightnes","Goblins  ","Echoes   ","Sci-fi   "
                        ,"Sitar    ","Banjo    ","Shamisen ","Koto     "
                        ,"Kalimba  ","BagPipe  ","Fiddle   ","Shanai   "
                        ,"TinkleBll","Agogo    ","SteelDrum","Woodblock"
                        ,"Taiko    ","Melo.Tom ","Syn.Drum ","Rev.Cym  "
                        ,"Gt.FretNz","BrthNoise","Seashore ","BirdTweet"
                        ,"Telephone","Helicoptr","Applause ","Gunshot  "
        };

        private static byte[] spc = new byte[] {
              0x20, 0x3c, 0x3c, 0x20, 0x4d, 0x44, 0x50, 0x6c
            , 0x61, 0x79, 0x65, 0x72, 0x20, 0x3e, 0x3e, 0x20
            , 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20
            , 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };





        public static void Init()
        {
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


        public static void screenInitAY8910(FrameBuffer screen)
        {
            for (int ch = 0; ch < 3; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
            }
        }

        public static void screenInitC140(FrameBuffer screen)
        {
            //C140
            for (int ch = 0; ch < 24; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(screen, 24, ch * 8 + 8, 0);
                ChC140_P(screen, 0, 8 + ch * 8, ch, false, 0);
            }
        }

        public static void screenInitHuC6280(FrameBuffer screen)
        {
            for (int ch = 0; ch < 6; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
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
                    int kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = kbl[(ot % 12) * 2 + 1];
                    drawKbn(screen, 32 + kx, ch * 8 + 8, kt, 0);
                }
                drawFont8(screen, 296, ch * 8 + 8, 1, "   ");
                drawPanType2P(screen, 24, ch * 8 + 8, 0);
            }
        }

        public static void screenInitMIDI(FrameBuffer screen)
        {
        }

        public static void screenInitMixer(FrameBuffer screen)
        {
        }


        public static void Volume(FrameBuffer screen, int y, int c, ref int ov, int nv, int tp)
        {
            if (ov == nv) return;

            int t = 0;
            int sy = 0;
            if (c == 1 || c == 2) { t = 4; }
            if (c == 2) { sy = 4; }
            y = (y + 1) * 8;

            for (int i = 0; i <= 19; i++)
            {
                VolumeP(screen, 256 + i * 2, y + sy, (1 + t), tp);
            }

            for (int i = 0; i <= nv; i++)
            {
                VolumeP(screen, 256 + i * 2, y + sy, i > 17 ? (2 + t) : (0 + t), tp);
            }

            ov = nv;

        }

        public static void VolumeToC140(FrameBuffer screen, int y, int c, ref int ov, int nv)
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

        public static void VolumeLCDToMIDILCD(FrameBuffer screen, int MIDImodule, int x, int y, ref int oldValue1, int value1, ref int oldValue2, int value2)
        {
            if (oldValue1 == value1 && oldValue2 == value2) return;

            int s = 0;
            //for (int n = (Math.Min(oldValue1, value1) / 8); n < 16; n++)
            for (int n = 0; n < 16; n++)
            {
                s = (value1 / 8) < n ? 8 : 0;
                screen.drawByteArray(x, y - (n * 3), rMIDILCD[MIDImodule], 136, 8 * 16, s, 8, 2);
            }

            screen.drawByteArray(x, y - (value2 / 8 * 3), rMIDILCD[MIDImodule], 136, 8 * 16, 0, 8, 2);

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
                screen.drawByteArray(n * 2 + x, y, rMIDILCD_Vol[MIDImodule], 32, 0 + (n > 23 ? 4 : 0) + s, 0, 2, 8);
            }

            oldValue = value;
        }


        public static void KeyBoard(FrameBuffer screen, int y, ref int ot, int nt, int tp)
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

        public static void KeyBoardToC140(FrameBuffer screen, int y, ref int ot, int nt)
        {
            if (ot == nt) return;

            int kx = 0;
            int kt = 0;

            y = (y + 1) * 8;

            if (ot >= 0)
            {
                kx = kbl[(ot % 12) * 2] + ot / 12 * 28;
                kt = kbl[(ot % 12) * 2 + 1];
                drawKbn(screen, 32 + kx, y, kt, 0);
            }

            if (nt >= 0)
            {
                kx = kbl[(nt % 12) * 2] + nt / 12 * 28;
                kt = kbl[(nt % 12) * 2 + 1] + 4;
                drawKbn(screen, 32 + kx, y, kt, 0);
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


        public static void PanToC140(FrameBuffer screen, int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(screen, 24, 8 + c * 8, nt);
            ot = nt;
        }

        public static void PanToHuC6280(FrameBuffer screen, int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(screen, 24, 8 + c * 8, nt);
            ot = nt;
        }

        public static void PanType2(FrameBuffer screen, int c, ref int ot, int nt)
        {

            if (ot == nt)
            {
                return;
            }

            drawPanType2P(screen, 24, 8 + c * 8, nt);
            ot = nt;
        }


        public static void ChAY8910(FrameBuffer screen, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChAY8910_P(screen, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public static void ChC140(FrameBuffer screen, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChC140_P(screen, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public static void ChHuC6280(FrameBuffer screen, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChHuC6280_P(screen, 0, 8 + ch * 8, ch, nm, tp);
            om = nm;
        }

        public static void ChRF5C164(FrameBuffer screen, int ch, ref bool om, bool nm, int tp)
        {

            if (om == nm)
            {
                return;
            }

            ChRF5C164_P(screen, 0, 8 + ch * 8, ch, nm, tp);
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
                screen.drawByteArray(x, y, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 15) ? 8 : ((n - 8) < 0 ? 0 : (n - 8));
                screen.drawByteArray(x, y - 8, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 23) ? 8 : ((n - 16) < 0 ? 0 : (n - 16));
                screen.drawByteArray(x, y - 16, rWavGraph, 64, m, 0, 1, 8);
                m = (n > 31) ? 8 : ((n - 24) < 0 ? 0 : (n - 24));
                screen.drawByteArray(x, y - 23, rWavGraph, 64, m + 1, 0, 1, 7);

                oi[i] = ni[i];
            }
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

        public static void drawMIDI_Lyric(FrameBuffer screen,int chipID, int x, int y, ref string oldValue1, string value1)
        {
            //if (oldValue1 == value1) return;

            gMIDILyric[chipID].Clear(Color.Black);
            System.Windows.Forms.TextRenderer.DrawText(gMIDILyric[chipID], value1, fntMIDILyric[chipID], new Point(0, 0), Color.White);
            byte[] bit = getByteArray(bitmapMIDILyric[chipID]);
            screen.drawByteArray(x, y, bit, 232, 0, 0, 232, 24);

            oldValue1 = value1;
        }

        public static void drawMIDI_MacroXG(FrameBuffer screen, int MIDImodule, int macroType, int x, int y, ref int oldValue1, int value1)
        {
            //if (oldValue1 == value1) return;

            drawFont4(screen, x, y, 2 + MIDImodule, tblMIDIEffectXG[macroType][value1]);

            oldValue1 = value1;
        }

        public static void drawMIDI_MacroGS(FrameBuffer screen, int MIDImodule, int macroType, int x, int y, ref int oldValue1, int value1)
        {
            //if (oldValue1 == value1) return;

            drawFont4(screen, x, y, 2 + MIDImodule, tblMIDIEffectGS[macroType][value1]);

            oldValue1 = value1;
        }

        public static void drawMIDILCD_Letter(FrameBuffer screen, int MIDImodule, int x, int y, ref byte[] oldValue, int len)
        {
            for (int i = 0; i < 16; i++)
            {
                if (oldValue[i] == spc[i]) continue;
                oldValue[i] = spc[i];

                if (screen == null) return;

                int cd = 0;
                //if (i < len) 
                cd = spc[i] - ' ';

                screen.drawByteArray(x + i * 8, y, rMIDILCD_Font[MIDImodule], 128, (cd % 16) * 8, (cd / 16) * 8, 8, 8);
            }

        }

        public static void drawMIDILCD_Letter(FrameBuffer screen, int MIDImodule, int x, int y, ref byte[] oldValue, byte[] value, int len)
        {
            for (int i = 0; i < 16; i++)
            {
                if (oldValue[i] == value[i]) continue;
                oldValue[i] = value[i];

                if (screen == null) return;

                int cd = 0;
                //if (i < len) 
                cd = value[i] - ' ';

                screen.drawByteArray(x + i * 8, y, rMIDILCD_Font[MIDImodule], 128, (cd % 16) * 8, (cd / 16) * 8, 8, 8);
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
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 10;
            num -= (byte)(n * 10);
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            return;
        }

        public static void drawFont4IntMIDIInstrument(FrameBuffer screen, int x, int y, int t, ref byte oldnum, byte num)
        {
            if (oldnum == num) return;
            oldnum = num;

            if (screen == null) return;

            drawFont4(screen, x, y + 8, t, tblMIDIInstrumentGM[num]);

            int n;

            n = num / 100;
            num -= (byte)(n * 100);
            //n = (n > 9) ? 0 : n;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 10;
            num -= (byte)(n * 10);
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 0, 4, 8);

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

        public static void drawMixerVolume(FrameBuffer screen, int x, int y, ref int od, int nd, ref int ov, int nv)
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








        private static byte[] getByteArray(Image img)
        {
            Bitmap bitmap = new Bitmap(img);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            byte[] byteArray = new byte[bitmapData.Stride * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, byteArray, 0, byteArray.Length);
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();

            return byteArray;
        }

        private static void VolumeP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rVol[tp], 32, 2 * t, 0, 2, 8 - (t / 4) * 4);
        }

        private static void drawKbn(FrameBuffer screen, int x, int y, int t, int tp)
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

        private static void ToneNoiseP(FrameBuffer screen, int x, int y, int t, int tp)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rPSGMode[tp], 32, 8 * t, 0, 8, 8);
        }

        private static void drawFont8(FrameBuffer screen, int x, int y, int t, string msg)
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

        private static void drawFont4(FrameBuffer screen, int x, int y, int t, string msg)
        {
            if (screen == null) return;

            foreach (char c in msg)
            {
                int cd = c - 'A' + 0x20 + 1;
                screen.drawByteArray(x, y, rFont2[t], 128, (cd % 32) * 4, (cd / 32) * 8, 4, 8);
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

        private static void drawFont4IntM(FrameBuffer screen, int x, int y, int k, int num)
        {
            if (screen == null) return;

            int t = 0;
            int n;

            if (num < 0)
            {
                num = -num;
                screen.drawByteArray(x - 4, y, rFont2[t], 128, 52, 1, 4, 7);
            }
            else
            {
                if (num != 0) t = 1;
                screen.drawByteArray(x - 4, y, rFont2[t], 128, 24, 1, 4, 7);
            }

            if (k == 3)
            {
                bool f = false;
                n = num / 100;
                num -= n * 100;
                n = (n > 9) ? 0 : n;
                if (n != 0)
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, 0, 1, 4, 7);
                }

                n = num / 10;
                num -= n * 10;
                x += 4;
                if (n != 0 || f)
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
                    if (n != 0) { f = true; }
                }
                else
                {
                    screen.drawByteArray(x, y, rFont2[t], 128, 0, 1, 4, 7);
                }

                n = num / 1;
                x += 4;
                screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
                return;
            }

            n = num / 10;
            num -= n * 10;
            n = (n > 9) ? 0 : n;
            if (n != 0)
            {
                screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
            }
            else
            {
                screen.drawByteArray(x, y, rFont2[t], 128, 0, 1, 4, 7);
            }

            n = num / 1;
            x += 4;
            screen.drawByteArray(x, y, rFont2[t], 128, n * 4 + 64, 1, 4, 7);
        }

        private static void drawEtypeP(FrameBuffer screen, int x, int y, int t)
        {
            if (screen == null) return;
            screen.drawByteArray(x, y, rPSGEnv, 64, 8 * t, 0, 8, 8);
        }

        private static void drawPanType2P(FrameBuffer screen, int x, int y, int t)
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

        private static void drawMIDILCD_FaderP(FrameBuffer screen, int MIDImodule, int faderType, int x, int y, int value)
        {
            screen.drawByteArray(x, y, rMIDILCD_Fader[MIDImodule], 64, value * 4, faderType * 16, 4, 16);
        }

        private static void drawMIDILCD_KbdP(FrameBuffer screen, int x, int y, int note, int vel)
        {
            screen.drawByteArrayTransp(x + kbdl[note % 12] + note / 12 * 28, y, rMIDILCD_KBD, 16, kbl2[note % 12], vel / 16 * 8, 4, 8);
        }

        private static void ChAY8910_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 32, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChC140_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            if (ch < 9) drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
            else drawFont4(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChHuC6280_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 0, 14 * 8, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (1 + ch).ToString());
        }

        private static void ChRF5C164_P(FrameBuffer screen, int x, int y, int ch, bool mask, int tp)
        {
            if (screen == null) return;

            screen.drawByteArray(x, y, rType[tp * 2 + (mask ? 1 : 0)], 128, 16, 0, 16, 8);
            drawFont8(screen, x + 16, y, mask ? 1 : 0, (ch + 1).ToString());
        }

        private static void drawFaderSlitP(FrameBuffer screen, int x, int y)
        {
            screen.drawByteArray(x, y, rFader, 32, 16, 0, 8, 8);
            screen.drawByteArray(x, y + 8, rFader, 32, 16, 8, 8, 8);
            screen.drawByteArray(x, y + 16, rFader, 32, 16, 8, 8, 8);
            screen.drawByteArray(x, y + 24, rFader, 32, 16, 8, 8, 8);
            screen.drawByteArray(x, y + 32, rFader, 32, 16, 8, 8, 8);
            screen.drawByteArray(x, y + 40, rFader, 32, 16, 8, 8, 8);
            screen.drawByteArray(x, y + 48, rFader, 32, 24, 0, 8, 8);
        }

        private static void drawFaderP(FrameBuffer screen, int x, int y, int t)
        {
            screen.drawByteArray(x, y, rFader, 32, t == 0 ? 0 : 8, 0, 8, 13);
        }

        private static void drawMixerVolumeP(FrameBuffer screen, int x, int y, int t)
        {
            screen.drawByteArray(x, y, rFader, 32, 24, 8 + t, 2, 1);
        }



    }
}
