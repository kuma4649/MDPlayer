using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public static class Tables
    {
        public static int[] FmFNum = new int[] {
            0x289/8, 0x2af/8, 0x2d8/8, 0x303/8, 0x331/8, 0x362/8, 0x395/8, 0x3cc/8, 0x405/8, 0x443/8, 0x484/8,0x4c8/8,
            0x289/4, 0x2af/4, 0x2d8/4, 0x303/4, 0x331/4, 0x362/4, 0x395/4, 0x3cc/4, 0x405/4, 0x443/4, 0x484/4,0x4c8/4,
            0x289/2, 0x2af/2, 0x2d8/2, 0x303/2, 0x331/2, 0x362/2, 0x395/2, 0x3cc/2, 0x405/2, 0x443/2, 0x484/2,0x4c8/2,
            0x289, 0x2af, 0x2d8, 0x303, 0x331, 0x362, 0x395, 0x3cc, 0x405, 0x443, 0x484, 0x4c8,
            0x289*2, 0x2af*2, 0x2d8*2, 0x303*2, 0x331*2, 0x362*2, 0x395*2, 0x3cc*2, 0x405*2, 0x443*2, 0x484*2,0x4c8*2
        };

        public static int[] PsgFNum = new int[] {
            0x6ae,0x64e,0x5f4,0x59e,0x54e,0x502,0x4ba,0x476,0x436,0x3f8,0x3c0,0x38a, // 0
            0x357,0x327,0x2fa,0x2cf,0x2a7,0x281,0x25d,0x23b,0x21b,0x1fc,0x1e0,0x1c5, // 1
            0x1ac,0x194,0x17d,0x168,0x153,0x140,0x12e,0x11d,0x10d,0x0fe,0x0f0,0x0e3, // 2
            0x0d6,0x0ca,0x0be,0x0b4,0x0aa,0x0a0,0x097,0x08f,0x087,0x07f,0x078,0x071, // 3
            0x06b,0x065,0x05f,0x05a,0x055,0x050,0x04c,0x047,0x043,0x040,0x03c,0x039, // 4
            0x035,0x032,0x030,0x02d,0x02a,0x028,0x026,0x024,0x022,0x020,0x01e,0x01c, // 5
            0x01b,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010,0x00f,0x00e, // 6
            0x00d,0x00d,0x00c,0x00b,0x00b,0x00a,0x009,0x008,0x007,0x006,0x005,0x004  // 7
        };

        public static float[] freqTbl = new float[] {
            261.6255653005986f/8.0f , 277.1826309768721f/8.0f , 293.6647679174076f/8.0f , 311.12698372208087f/8.0f , 329.6275569128699f/8.0f , 349.2282314330039f/8.0f , 369.9944227116344f/8.0f , 391.99543598174927f/8.0f , 415.3046975799451f/8.0f , 440f/8.0f , 466.1637615180899f/8.0f,493.8833012561241f/8.0f,
            261.6255653005986f/4.0f , 277.1826309768721f/4.0f , 293.6647679174076f/4.0f , 311.12698372208087f/4.0f , 329.6275569128699f/4.0f , 349.2282314330039f/4.0f , 369.9944227116344f/4.0f , 391.99543598174927f/4.0f , 415.3046975799451f/4.0f , 440f/4.0f , 466.1637615180899f/4.0f,493.8833012561241f/4.0f,
            261.6255653005986f/2.0f , 277.1826309768721f/2.0f , 293.6647679174076f/2.0f , 311.12698372208087f/2.0f , 329.6275569128699f/2.0f , 349.2282314330039f/2.0f , 369.9944227116344f/2.0f , 391.99543598174927f/2.0f , 415.3046975799451f/2.0f , 440f/2.0f , 466.1637615180899f/2.0f,493.8833012561241f/2.0f,
            261.6255653005986f , 277.1826309768721f , 293.6647679174076f , 311.12698372208087f , 329.6275569128699f , 349.2282314330039f , 369.9944227116344f , 391.99543598174927f , 415.3046975799451f , 440f , 466.1637615180899f,493.8833012561241f,
            261.6255653005986f*2.0f , 277.1826309768721f*2.0f , 293.6647679174076f*2.0f , 311.12698372208087f*2.0f , 329.6275569128699f*2.0f , 349.2282314330039f*2.0f , 369.9944227116344f*2.0f , 391.99543598174927f*2.0f , 415.3046975799451f*2.0f , 440f*2.0f , 466.1637615180899f*2.0f,493.8833012561241f*2.0f,
            261.6255653005986f*4.0f , 277.1826309768721f*4.0f , 293.6647679174076f*4.0f , 311.12698372208087f*4.0f , 329.6275569128699f*4.0f , 349.2282314330039f*4.0f , 369.9944227116344f*4.0f , 391.99543598174927f*4.0f , 415.3046975799451f*4.0f , 440f*4.0f , 466.1637615180899f*4.0f,493.8833012561241f*4.0f,
            261.6255653005986f*8.0f , 277.1826309768721f*8.0f , 293.6647679174076f*8.0f , 311.12698372208087f*8.0f , 329.6275569128699f*8.0f , 349.2282314330039f*8.0f , 369.9944227116344f*8.0f , 391.99543598174927f*8.0f , 415.3046975799451f*8.0f , 440f*8.0f , 466.1637615180899f*8.0f,493.8833012561241f*8.0f,
            261.6255653005986f*16.0f , 277.1826309768721f*16.0f , 293.6647679174076f*16.0f , 311.12698372208087f*16.0f , 329.6275569128699f*16.0f , 349.2282314330039f*16.0f , 369.9944227116344f*16.0f , 391.99543598174927f*16.0f , 415.3046975799451f*16.0f , 440f*16.0f , 466.1637615180899f*16.0f,493.8833012561241f*16.0f
        };

        public static float[] pcmMulTbl = new float[]
        {
            1.0f/2.0f
            ,1.05947557526183f/2.0f
            ,1.122467701246082f/2.0f
            ,1.189205718217262f/2.0f
            ,1.259918966439875f/2.0f
            ,1.334836786178427f/2.0f
            ,1.414226741074841f/2.0f
            ,1.498318171393624f/2.0f
            ,1.587416864154117f/2.0f
            ,1.681828606375659f/2.0f
            ,1.781820961700176f/2.0f
            ,1.887776163901842f/2.0f
            ,1.0f
            ,1.05947557526183f
            ,1.122467701246082f
            ,1.189205718217262f
            ,1.259918966439875f
            ,1.334836786178427f
            ,1.414226741074841f
            ,1.498318171393624f
            ,1.587416864154117f
            ,1.681828606375659f
            ,1.781820961700176f
            ,1.887776163901842f
        };

        public static string[][] tblMIDIEffectGS = new string[4][] {
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

        public static string[][] tblMIDIEffectXG = new string[3][] {
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

        public static string[] tblMIDIInstrumentGM = new string[] {
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

        public static byte[] spc = new byte[] {
              0x20, 0x3c, 0x3c, 0x20, 0x4d, 0x44, 0x50, 0x6c
            , 0x61, 0x79, 0x65, 0x72, 0x20, 0x3e, 0x3e, 0x20
            , 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20
            , 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

        public static int[] kbl = new int[] { 0, 0, 2, 1, 4, 2, 6, 1, 8, 3, 12, 0, 14, 1, 16, 2, 18, 1, 20, 2, 22, 1, 24, 3 };
        public static string[] kbn = new string[] { "C ", "C#", "D ", "D#", "E ", "F ", "F#", "G ", "G#", "A ", "A#", "B " };
        public static string[] kbns = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public static string[] kbnp = new string[] { "C ", "C+", "D ", "D+", "E ", "F ", "F+", "G ", "G+", "A ", "A+", "B " };
        public static string[] kbo = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
        public static int[] kbl2 = new int[] { 0, 12, 4, 12, 8, 0, 12, 4, 12, 4, 12, 8 };
        public static int[] kbdl = new int[] { 0, 2, 4, 6, 8, 12, 14, 16, 18, 20, 22, 24 };

        public static string[] hexCh = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
    }
}
