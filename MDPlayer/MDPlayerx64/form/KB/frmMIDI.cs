#if X64
using MDPlayerx64;
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class frmMIDI : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MIDIParam newParam = null;
        private MIDIParam oldParam = new MIDIParam();
        private FrameBuffer frameBuffer = new FrameBuffer();
        private string notes = "";


        public frmMIDI(frmMain frm, int chipID, int zoom, MIDIParam newParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, ResMng.imgDic["planeMIDI_GM"], null, zoom);
            DrawBuff.screenInitMIDI(frameBuffer);
            update();
        }

        public void update()
        {
            frameBuffer.Refresh(null);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmMIDI_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosMIDI[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosMIDI[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmMIDI_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeMIDI_GM"].Width * zoom, frameSizeH + ResMng.imgDic["planeMIDI_GM"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeMIDI_GM"].Width * zoom, frameSizeH + ResMng.imgDic["planeMIDI_GM"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeMIDI_GM"].Width * zoom, frameSizeH + ResMng.imgDic["planeMIDI_GM"].Height * zoom);
            frmMIDI_Resize(null, null);

        }

        private void frmMIDI_Resize(object sender, EventArgs e)
        {

        }



        public void screenChangeParams()
        {
            MIDIParam prm = Audio.GetMIDIInfos(chipID);

            for (int ch = 0; ch < 16; ch++)
            {
                for (int i = 0; i < 256; i++)
                {
                    newParam.cc[ch][i] = prm.cc[ch][i];
                }
                newParam.bend[ch] = prm.bend[ch];

                for (int i = 0; i < 128; i++)
                {
                    newParam.note[ch][i] = prm.note[ch][i];
                }

                newParam.level[ch][0] = prm.level[ch][0];
                newParam.level[ch][1] = prm.level[ch][1];
                newParam.level[ch][2] = prm.level[ch][2];
                newParam.level[ch][3] = prm.level[ch][3];
                newParam.level[ch][4] = prm.level[ch][4];
                if (prm.level[ch][0] > 0) { prm.level[ch][0] -= 3; if (prm.level[ch][0] < 0) prm.level[ch][0] = 0; }
                if (prm.level[ch][1] > 0) { prm.level[ch][1] -= 3; if (prm.level[ch][1] < 0) prm.level[ch][1] = 0; }
                if (prm.level[ch][2] > 0) { prm.level[ch][2] -= 3; if (prm.level[ch][2] < 0) prm.level[ch][2] = 0; }
                if (prm.level[ch][3] > 0)
                {
                    prm.level[ch][4] -= 3;
                    if (prm.level[ch][4] < 0)
                    {
                        prm.level[ch][4] = 0;
                        prm.level[ch][3] -= 3;
                        if (prm.level[ch][3] < 0) prm.level[ch][3] = 0;
                    }
                }

                newParam.pc[ch] = prm.pc[ch];

                newParam.nrpnVibRate[ch] = prm.nrpnVibRate[ch];
                newParam.nrpnVibDepth[ch] = prm.nrpnVibDepth[ch];
                newParam.nrpnVibDelay[ch] = prm.nrpnVibDelay[ch];

                newParam.nrpnLPF[ch] = prm.nrpnLPF[ch];
                newParam.nrpnLPFRsn[ch] = prm.nrpnLPFRsn[ch];
                newParam.nrpnHPF[ch] = prm.nrpnHPF[ch];

                newParam.nrpnEQBaseFrq[ch] = prm.nrpnEQBaseFrq[ch];
                newParam.nrpnEQBaseGain[ch] = prm.nrpnEQBaseGain[ch];
                newParam.nrpnEQTrebleFrq[ch] = prm.nrpnEQTrebleFrq[ch];
                newParam.nrpnEQTrebleGain[ch] = prm.nrpnEQTrebleGain[ch];

                newParam.nrpnEGAttack[ch] = prm.nrpnEGAttack[ch];
                newParam.nrpnEGDecay[ch] = prm.nrpnEGDecay[ch];
                newParam.nrpnEGRls[ch] = prm.nrpnEGRls[ch];
            }

            newParam.MIDIModule = prm.MIDIModule;

            //Display Data
            for (int i = 0; i < 64; i++)
            {
                newParam.LCDDisplay[i] = prm.LCDDisplay[i];
            }
            newParam.LCDDisplayTime = prm.LCDDisplayTime;
            prm.LCDDisplayTime -= 3;
            if (prm.LCDDisplayTime < 0) prm.LCDDisplayTime = 0;

            for (int i = 0; i < prm.LCD8850Display.Length; i++)
            {
                newParam.LCD8850Display[i] = prm.LCD8850Display[i];
            }
            newParam.LCD8850DisplayTime = prm.LCD8850DisplayTime;
            if(newParam.LCD8850DisplayTime!=400) prm.LCD8850DisplayTime -= 3;
            if (prm.LCD8850DisplayTime < 0) prm.LCD8850DisplayTime = 0;

            newParam.LCDDisplayTimeXG = prm.LCDDisplayTimeXG;
            prm.LCDDisplayTimeXG -= 3;
            if (prm.LCDDisplayTimeXG < 0) prm.LCDDisplayTimeXG = 0;

            //Display Letter Data
            for (int i = 0; i < 32; i++)
            {
                newParam.LCDDisplayLetter[i] = prm.LCDDisplayLetter[i];
            }
            newParam.LCDDisplayLetterLen = prm.LCDDisplayLetterLen;
            newParam.LCDDisplayLetterTime = prm.LCDDisplayLetterTime;
            prm.LCDDisplayLetterTime -= 3;
            if (prm.LCDDisplayLetterTime < 0)
            {
                if (prm.LCDDisplayLetterLen > 0)
                {
                    for (int i = 1; i < 32; i++)
                    {
                        prm.LCDDisplayLetter[i - 1] = (byte)(i < prm.LCDDisplayLetterLen ? prm.LCDDisplayLetter[i] : 0x20);
                    }
                    prm.LCDDisplayLetterTime = 40;
                    prm.LCDDisplayLetterLen--;
                }
                else
                {
                    prm.LCDDisplayLetterTime = 0;
                }
            }
            newParam.LCDDisplayLetterTimeXG = prm.LCDDisplayLetterTimeXG;
            prm.LCDDisplayLetterTimeXG -= 3;
            if (prm.LCDDisplayLetterTimeXG < 0) prm.LCDDisplayLetterTimeXG = 0;

            newParam.ReverbGS = prm.ReverbGS;
            newParam.ChorusGS = prm.ChorusGS;
            newParam.DelayGS = prm.DelayGS;
            newParam.EFXGS = prm.EFXGS;

            newParam.ReverbXG = prm.ReverbXG;
            newParam.ChorusXG = prm.ChorusXG;
            newParam.VariationXG = prm.VariationXG;
            newParam.Insertion1XG = prm.Insertion1XG;
            newParam.Insertion2XG = prm.Insertion2XG;
            newParam.Insertion3XG = prm.Insertion3XG;
            newParam.Insertion4XG = prm.Insertion4XG;

            newParam.MasterVolume = prm.MasterVolume;

            newParam.Lyric = prm.Lyric;

        }

        public void screenDrawParams()
        {
            int module = newParam.MIDIModule;

            if (oldParam.MIDIModule != newParam.MIDIModule)
            {
                frameBuffer.drawByteArray(0, 0, DrawBuff.rPlane_MIDI[newParam.MIDIModule], 440, 0, 0, 440, 352);
                oldParam.MIDIModule = newParam.MIDIModule;
            }

            if (module == 1)
            {
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 0, 4 * 42, 16 + 33 * 8, ref oldParam.ReverbXG, newParam.ReverbXG);
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 1, 4 * 42, 32 + 33 * 8, ref oldParam.ChorusXG, newParam.ChorusXG);
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 2, 4 * 42, 48 + 33 * 8, ref oldParam.VariationXG, newParam.VariationXG);
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 2, 4 * 42, 64 + 33 * 8, ref oldParam.Insertion1XG, newParam.Insertion1XG);
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 2, 4 * 42, 80 + 33 * 8, ref oldParam.Insertion2XG, newParam.Insertion2XG);
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 2, 4 * 60, 32 + 33 * 8, ref oldParam.Insertion3XG, newParam.Insertion3XG);
                DrawBuff.drawMIDI_MacroXG(frameBuffer, module, 2, 4 * 60, 48 + 33 * 8, ref oldParam.Insertion4XG, newParam.Insertion4XG);
            }
            else
            {
                DrawBuff.drawMIDI_MacroGS(frameBuffer, module, 0, 4 * 42, 16 + 33 * 8, ref oldParam.ReverbGS, newParam.ReverbGS);
                DrawBuff.drawMIDI_MacroGS(frameBuffer, module, 1, 4 * 42, 32 + 33 * 8, ref oldParam.ChorusGS, newParam.ChorusGS);
                DrawBuff.drawMIDI_MacroGS(frameBuffer, module, 2, 4 * 42, 48 + 33 * 8, ref oldParam.DelayGS, newParam.DelayGS);
                DrawBuff.drawMIDI_MacroGS(frameBuffer, module, 3, 4 * 42, 64 + 33 * 8, ref oldParam.EFXGS, newParam.EFXGS);
            }

            if (newParam.LCDDisplayLetterTime == 0 && newParam.LCDDisplayLetterTimeXG == 0)
            {
                DrawBuff.drawMIDILCD_Letter(frameBuffer, module, 4, 277, ref oldParam.LCDDisplayLetter, 16);
            }
            else
            {
                DrawBuff.drawMIDILCD_Letter(frameBuffer, module, 4, 277, ref oldParam.LCDDisplayLetter, newParam.LCDDisplayLetter, newParam.LCDDisplayLetterLen);
            }

            DrawBuff.drawFont4IntMIDI(frameBuffer, 60 * 4, 17 * 16 + 8, 2 + module, ref oldParam.MasterVolume, newParam.MasterVolume);

            DrawBuff.drawMIDI_Lyric(frameBuffer, chipID, 60 * 4, 41 * 8, ref oldParam.Lyric, newParam.Lyric);

            for (int ch = 0; ch < 16; ch++)
            {
                byte b = (byte)(128 - newParam.cc[ch][10]);
                b = (byte)(b > 127 ? 127 : b);
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 0, 64, ch * 16 + 16, ref oldParam.cc[ch][10], b);//Panpot
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 68, ch * 16 + 16, ref oldParam.cc[ch][7], newParam.cc[ch][7]);//Volume
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 72, ch * 16 + 16, ref oldParam.cc[ch][11], newParam.cc[ch][11]);//Expression
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 0, 76, ch * 16 + 16, ref oldParam.bend[ch], newParam.bend[ch]);//PitchBend
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 80, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][1]);//Modulation
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 84, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][91]);//Reverb
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 88, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][93]);//Chorus
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 92, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][94]);//Variation(Delay)
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 96, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][64]);//Hold(DumperPedal)
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 100, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][67]);//Soft
                DrawBuff.drawMIDILCD_Fader(frameBuffer, module, 1, 104, ch * 16 + 16, ref oldParam.cc[ch][1], newParam.cc[ch][66]);//Sostenuto

                notes = "";
                for (int n = 0; n < 120; n++)
                {
                    DrawBuff.drawMIDILCD_Kbd(frameBuffer, 108
                        , ch * 16 + 16, n, ref oldParam.note[ch][n], newParam.note[ch][n]);

                    if (newParam.note[ch][n] > 0)
                    {
                        notes = notes + string.Format("{0}{1} ", Tables.kbns[n % 12], n / 12);
                    }
                }
                notes += "                           ";
                notes = notes.Substring(0, 26);
                DrawBuff.drawFont4MIDINotes(frameBuffer, 71 * 4, ch * 16 + 24, module + 2, ref oldParam.notes[ch], notes);

                DrawBuff.VolumeToMIDILCD(frameBuffer, module, 388, ch * 16 + 16, ref oldParam.level[ch][0], newParam.level[ch][0]);
                DrawBuff.VolumeToMIDILCD(frameBuffer, module, 388, ch * 16 + 24, ref oldParam.level[ch][2], newParam.level[ch][2]);

                //L1:
                if (newParam.LCDDisplayTime == 0 && newParam.LCD8850DisplayTime == 0 && newParam.LCDDisplayTimeXG == 0)
                {
                    DrawBuff.VolumeLCDToMIDILCD(frameBuffer
                        , module
                        , 5 + ch * 10 + (ch > 3 ? 1 : 0) + (ch > 11 ? 1 : 0)
                        , 341
                        , ref oldParam.level[ch][1]
                        , newParam.level[ch][1]
                        , ref oldParam.level[ch][3]
                        , newParam.level[ch][3]);
                }
                else
                {
                    int s = 0;
                    for (int n = 0; n < 16; n++)
                    {
                        oldParam.level[ch][1] = 256;
                        oldParam.level[ch][3] = 256;
                        oldParam.level[ch][4] = 256;
                    }
                    if (newParam.LCDDisplayTime > 0)
                    {
                        //GS
                        for (int n = 0; n < 64; n++)
                        {
                            s = newParam.LCDDisplay[n];
                            int x = n / 16;
                            int y = n % 16;
                            frameBuffer.drawByteArray(
                                x * 50 + 5 + 0 + (x > 0 ? 1 : 0) + (x > 2 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x10) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            if (n > 47) continue;
                            frameBuffer.drawByteArray(
                                x * 50 + 5 + 10 + (x > 0 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x08) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 50 + 5 + 20 + (x > 0 ? 1 : 0) + (x > 1 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x04) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 50 + 5 + 30 + (x > 0 ? 1 : 0) + (x > 1 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x02) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 50 + 5 + 41 + (x > 1 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x01) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                        }
                    }
                    else if (newParam.LCD8850DisplayTime == 400)
                    {
                        for (int y = 0; y < 64; y++)
                        {
                            for (int n = 0; n < 27; n++)
                            {
                                s = newParam.LCD8850Display[n + y * 27];
                                //if (oldParam.LCD8850Display[n + y * 27] != s)
                                //{
                                oldParam.LCD8850Display[n + y * 27] = (byte)s;
                                for (int d = 0; d < 6; d++)
                                {
                                    frameBuffer.drawByteArray(n * 6 + d + 4 + 0, 293 + y - 6, DrawBuff.rMIDILCD[module], 136, 8 * 16 + 3, ((s & (0x20 >> d)) != 0) ? 0 : 8, 1, 1);
                                }
                                //}
                            }
                        }
                        newParam.LCD8850DisplayTime--;
                        if (newParam.LCD8850DisplayTime == 0)
                        {
                            frameBuffer.drawByteArray(0, 272, DrawBuff.rPlane_MIDI[module], 440, 0, 272, 168, 80);
                        }
                    }
                    else if (newParam.LCDDisplayTimeXG > 0)
                    {
                        //XG
                        for (int n = 0; n < 48; n++)
                        {
                            s = newParam.LCDDisplay[n];
                            int x = n / 16;
                            int y = n % 16;
                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 0 + (x > 0 ? 1 : 0) + (x > 1 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x40) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 10 + (x > 0 ? 1 : 0) + (x > 1 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x20) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);

                            if (n > 31) continue;

                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 20 + (x > 0 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x10) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 30 + (x > 0 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x08) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 41
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x04) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 51 + (x > 0 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x02) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                            frameBuffer.drawByteArray(
                                x * 70 + 5 + 61 + (x > 0 ? 1 : 0)
                                , 288 + y * 3 + (y + 1) / 2
                                , DrawBuff.rMIDILCD[module], 136, 8 * 16, ((s & 0x01) != 0) ? 0 : 8, 8, (y % 2 == 0 || y == 15) ? 3 : 2);
                        }
                    }
                }

                // Prg Bank Map
                DrawBuff.drawFont4IntMIDIInstrument(frameBuffer, 4 * 7, ch * 16 + 16, 2 + module, ref oldParam.pc[ch], newParam.pc[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 10, ch * 16 + 16, 2 + module, ref oldParam.cc[ch][0], newParam.cc[ch][0]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 13, ch * 16 + 16, 2 + module, ref oldParam.cc[ch][32], newParam.cc[ch][32]);

                //Vib Rate Depth Delay
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 28, ch * 16 + 24, 2 + module, ref oldParam.nrpnVibRate[ch], newParam.nrpnVibRate[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 31, ch * 16 + 24, 2 + module, ref oldParam.nrpnVibDepth[ch], newParam.nrpnVibDepth[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 34, ch * 16 + 24, 2 + module, ref oldParam.nrpnVibDelay[ch], newParam.nrpnVibDelay[ch]);

                //Filter LPF LPFRsn HPF
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 38, ch * 16 + 24, 2 + module, ref oldParam.nrpnLPF[ch], newParam.nrpnLPF[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 41, ch * 16 + 24, 2 + module, ref oldParam.nrpnLPFRsn[ch], newParam.nrpnLPFRsn[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 44, ch * 16 + 24, 2 + module, ref oldParam.nrpnHPF[ch], newParam.nrpnHPF[ch]);

                //EG Atk Dcy Rsn
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 48, ch * 16 + 24, 2 + module, ref oldParam.nrpnEGAttack[ch], newParam.nrpnEGAttack[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 51, ch * 16 + 24, 2 + module, ref oldParam.nrpnEGDecay[ch], newParam.nrpnEGDecay[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 54, ch * 16 + 24, 2 + module, ref oldParam.nrpnEGRls[ch], newParam.nrpnEGRls[ch]);

                //EQ Base Gain Frq Treble Gain Frq
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 58, ch * 16 + 24, 2 + module, ref oldParam.nrpnEQBaseGain[ch], newParam.nrpnEQBaseGain[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 61, ch * 16 + 24, 2 + module, ref oldParam.nrpnEQBaseFrq[ch], newParam.nrpnEQBaseFrq[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 64, ch * 16 + 24, 2 + module, ref oldParam.nrpnEQTrebleGain[ch], newParam.nrpnEQTrebleGain[ch]);
                DrawBuff.drawFont4IntMIDI(frameBuffer, 4 * 67, ch * 16 + 24, 2 + module, ref oldParam.nrpnEQTrebleFrq[ch], newParam.nrpnEQTrebleFrq[ch]);

            }

        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;

        }

        internal void screenInit()
        {
            int module = newParam.MIDIModule;
            newParam.LCD8850DisplayTime = 0;
            frameBuffer.drawByteArray(0, 272, DrawBuff.rPlane_MIDI[module], 440, 0, 272, 168, 80);
        }
    }
}