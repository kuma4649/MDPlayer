using MDPlayer.Properties;
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
    public partial class frmYM2609 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2609 newParam = null;
        private MDChipParams.YM2609 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2609(frmMain frm, int chipID, int zoom, MDChipParams.YM2609 newParam, MDChipParams.YM2609 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, Resources.planeYM2609, null, zoom);
            DrawBuff.screenInitYM2609(frameBuffer, 0);
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

        private void frmYM2609_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2609[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm2609[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYM2609_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Resources.planeYM2609.Width * zoom, frameSizeH + Resources.planeYM2609.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Resources.planeYM2609.Width * zoom, frameSizeH + Resources.planeYM2609.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Resources.planeYM2609.Width * zoom, frameSizeH + Resources.planeYM2609.Height * zoom);
            frmYM2609_Resize(null, null);

        }

        private void frmYM2609_Resize(object sender, EventArgs e)
        {

        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {

        }

        public void screenInit()
        {
            DrawBuff.screenInitYM2609(frameBuffer, 0);
        }

        private static byte[] md = new byte[]
        {
            0x08<<4,
            0x08<<4,
            0x08<<4,
            0x08<<4,
            0x0c<<4,
            0x0e<<4,
            0x0e<<4,
            0x0f<<4
        };

        private static float[] fmDivTbl = new float[] { 6, 3, 2 };
        private static float[] ssgDivTbl = new float[] { 4, 2, 1 };
        private bool[] isFmEx = new bool[] { false, false };
        private int[] exReg = new int[3] { 2, 0, -6 };

        public void screenChangeParams()
        {
            int[][] ym2609Register = Audio.GetYM2609Register(chipID);
            int[] fmKeyYM2609 = Audio.GetYM2609KeyOn(chipID);
            int[] ym2609Vol = Audio.GetYM2609Volume(chipID);
            int[] ym2609Ch3SlotVol = Audio.GetYM2609Ch3SlotVolume(chipID);
            int[][] ym2609Rhythm = Audio.GetYM2609RhythmVolume(chipID);
            int[][] ym2609AdpcmVol = Audio.GetYM2609AdpcmVolume(chipID);
            int[][] ym2609AdpcmPan = Audio.GetYM2609AdpcmPan(chipID);

            isFmEx[0] = (ym2609Register[0][0x27] & 0x40) > 0;
            isFmEx[1] = (ym2609Register[2][0x27] & 0x40) > 0;
            newParam.channels[2].ex = isFmEx[0];
            newParam.channels[8].ex = isFmEx[1];

            int defaultMasterClock = 7987200;
            float ssgMul = 1.0f;
            int masterClock = defaultMasterClock;
            if (Audio.clockYM2609 != 0)
            {
                ssgMul = Audio.clockYM2609 / (float)defaultMasterClock;
                masterClock = Audio.clockYM2609;
            }

            int divInd = ym2609Register[0][0x2d];
            if (divInd < 0 || divInd > 2) divInd = 0;
            float fmDiv = fmDivTbl[divInd];
            float ssgDiv = ssgDivTbl[divInd];
            ssgMul = ssgMul / ssgDiv * 4;

            //FM
            for (int ch = 0; ch < 12; ch++)
            {
                int p = ch / 3;
                int c = ch % 3;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.channels[ch].inst[i * 16 + 0] = ym2609Register[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.channels[ch].inst[i * 16 + 1] = ym2609Register[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.channels[ch].inst[i * 16 + 2] = ym2609Register[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.channels[ch].inst[i * 16 + 3] = ym2609Register[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.channels[ch].inst[i * 16 + 4] = (ym2609Register[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.channels[ch].inst[i * 16 + 5] = ym2609Register[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.channels[ch].inst[i * 16 + 6] = (ym2609Register[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.channels[ch].inst[i * 16 + 7] = ym2609Register[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.channels[ch].inst[i * 16 + 8] = (ym2609Register[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.channels[ch].inst[i * 16 + 9] = (ym2609Register[p][0x60 + ops + c] & 0x60) >> 5;//D2
                    newParam.channels[ch].inst[i * 16 + 10] = (ym2609Register[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.channels[ch].inst[i * 16 + 11] = ym2609Register[p][0x90 + ops + c] & 0x0f;//SG
                    newParam.channels[ch].inst[i * 16 + 12] = (i == 0)
                        ? ((ym2609Register[p][0xb0 + c] & 0x38) >> 3)
                        : ((ym2609Register[p][0x70 + ops + c] & 0xe0) >> 5);//FB
                    newParam.channels[ch].inst[i * 16 + 13] =
                        ((ym2609Register[p][0x30 + ops + c] & 0x80) >> 7)
                        + ((ym2609Register[p][0x40 + ops + c] & 0x80) >> 6);//WT
                    newParam.channels[ch].inst[i * 16 + 14] = (ym2609Register[p][0x90 + ops + c] & 0xf0) >> 4;//ALL
                    newParam.channels[ch].inst[i * 16 + 15] = (ym2609Register[p][0x50 + ops + c] & 0x20) >> 5;//PR
                }
                newParam.channels[ch].inst[64] = ym2609Register[p][0xb0 + c] & 0x07;//AL
                newParam.channels[ch].inst[65] = (ym2609Register[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.channels[ch].inst[66] = ym2609Register[p][0xb4 + c] & 0x07;//FMS

                int pan = (ym2609Register[p][0xb4 + c] & 0xc0) >> 6;
                newParam.channels[ch].pan =
                    (((pan & 0x02) != 0) ? ((3 - ((ym2609Register[p][0xa4 + c] & 0xc0) >> 6)) + 1) : 0) * 5
                    + (((pan & 0x01) != 0) ? ((3 - ((ym2609Register[p][0xb0 + c] & 0xc0) >> 6)) + 1) : 0);
                newParam.channels[ch].slot = (byte)(fmKeyYM2609[ch] >> 4);

                int freq;
                int octav;
                int n = -1;

                if ((ch != 2 || !isFmEx[0]) && (ch != 8 || !isFmEx[1]))
                {
                    octav = (ym2609Register[p][0xa4 + c] & 0x38) >> 3;
                    freq = ym2609Register[p][0xa0 + c] + (ym2609Register[p][0xa4 + c] & 0x07) * 0x100;
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2609[ch] & 1) != 0)
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    byte con = (byte)(fmKeyYM2609[ch]);
                    int v = 127;
                    int m = md[ym2609Register[p][0xb0 + c] & 7];
                    //OP1
                    v = (((con & 0x10) != 0) && ((m & 0x10) != 0) && v > (ym2609Register[p][0x40 + c] & 0x7f)) ? (ym2609Register[p][0x40 + c] & 0x7f) : v;
                    //OP3
                    v = (((con & 0x20) != 0) && ((m & 0x20) != 0) && v > (ym2609Register[p][0x44 + c] & 0x7f)) ? (ym2609Register[p][0x44 + c] & 0x7f) : v;
                    //OP2
                    v = (((con & 0x40) != 0) && ((m & 0x40) != 0) && v > (ym2609Register[p][0x48 + c] & 0x7f)) ? (ym2609Register[p][0x48 + c] & 0x7f) : v;
                    //OP4
                    v = (((con & 0x80) != 0) && ((m & 0x80) != 0) && v > (ym2609Register[p][0x4c + c] & 0x7f)) ? (ym2609Register[p][0x4c + c] & 0x7f) : v;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2609Register[p][0xb4 + c] & 0x80) != 0 ? 1 : 0) * ym2609Vol[ch] / 80.0), 0), 19);
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2609Register[p][0xb4 + c] & 0x40) != 0 ? 1 : 0) * ym2609Vol[ch] / 80.0), 0), 19);

                }
                else
                {
                    int m = md[ym2609Register[p][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    freq = ym2609Register[p][0xa9] + (ym2609Register[p][0xad] & 0x07) * 0x100;
                    octav = (ym2609Register[p][0xad] & 0x38) >> 3;
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2609[ch] & 0x10) > 0 && ((m & 0x10) != 0))
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    int v = ((m & 0x10) != 0) ? (ym2609Register[p][0x40 + c] & 0x7f) : 127;
                    newParam.channels[ch].volumeL = 
                        Math.Min(Math.Max(
                            (int)((127 - v) / 127.0 * ((ym2609Register[p][0xb4 + 2] & 0x80) != 0 ? 1 : 0) 
                            * ym2609Ch3SlotVol[0] / 80.0)
                            , 0), 19);
                    newParam.channels[ch].volumeR = 
                        Math.Min(Math.Max(
                            (int)((127 - v) / 127.0 * ((ym2609Register[p][0xb4 + 2] & 0x40) != 0 ? 1 : 0) 
                            * ym2609Ch3SlotVol[0] / 80.0)
                            , 0), 19);
                }
                newParam.channels[ch].note = n;

            }
            //FMex
            for (int ch = 12; ch < 18; ch++) //FM EX
            {
                int c = exReg[ch % 3];
                int p = (ch - 12) / 3 * 2;

                newParam.channels[ch].pan = 0;

                int f = ch < 15 ? 0 : 1;
                if (isFmEx[f])
                {
                    int m = md[ym2609Register[p][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    int op = (ch - (11 + f * 3));
                    op = op == 1 ? 2 : (op == 2 ? 1 : op);

                    int freq = ym2609Register[p][0xa8 + c] + (ym2609Register[p][0xac + c] & 0x07) * 0x100;
                    int octav = (ym2609Register[p][0xac + c] & 0x38) >> 3;
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    int n = -1;
                    if ((fmKeyYM2609[2] & (0x10 << (ch - (11 + f * 3)))) != 0 && ((m & (0x10 << op)) != 0))
                    {
                        float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                        ff /= 1038f;
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);
                    }
                    newParam.channels[ch].note = n;

                    int v = ((m & (0x10 << op)) != 0) ? (ym2609Register[p][0x42 + op * 4] & 0x7f) : 127;
                    newParam.channels[ch].volumeL =
                        Math.Min(Math.Max((int)((127 - v) / 127.0 * ym2609Ch3SlotVol[ch - (11 - f)] / 80.0), 0), 19);
                }
                else
                {
                    newParam.channels[ch].note = -1;
                    newParam.channels[ch].volumeL = 0;
                }
            }
            //PSG
            for (int ch = 18; ch < 30; ch++) //PSG 1-12
            {
                int c = (ch - 18) % 3;
                int p = (ch - 18) / 3;

                MDChipParams.Channel channel = newParam.channels[ch];

                bool t = (ym2609Register[psgPort[p]][psgAdr[p] + 0x07] & (0x1 << c)) == 0;
                bool n = (ym2609Register[psgPort[p]][psgAdr[p] + 0x07] & (0x8 << c)) == 0;
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);
                channel.volume = (int)(((t || n) ? 1 : 0) 
                    * (ym2609Register[psgPort[p]][psgAdr[p] + 0x08 + c] & 0xf) * (20.0 / 16.0));
                channel.pan = (ym2609Register[psgPort[p]][psgAdr[p] + 0x08 + c] & 0xc0) >> 6;
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                channel.note = -1;
                if (channel.volume != 0)
                {
                    int ft = ym2609Register[psgPort[p]][psgAdr[p] + 0x00 + c * 2];
                    int ct = ym2609Register[psgPort[p]][psgAdr[p] + 0x01 + c * 2] & 0xf;
                    int tp = (ct << 8) | ft;
                    channel.freq = tp;
                    if (tp == 0)
                    {
                        channel.note = -1;
                    }
                    else
                    {
                        float ftone = masterClock / (64.0f * (float)tp) * ssgMul;// 7987200 = MasterClock
                        channel.note = Common.searchSSGNote(ftone);
                    }
                }

                channel.bank = (ym2609Register[psgPort[p]][psgAdr[p] + 0x01 + c * 2] & 0xf0) >> 4;
                if (channel.bank > 9)
                {
                    channel.PSGWave = Audio.GetYM2609UserWave(chipID, p, channel.bank - 10);
                }
                else
                {
                    channel.PSGWave = null;
                }

            }
            //RHYTHM
            for (int ch = 30; ch < 42; ch++) 
            {
                newParam.channels[ch].pan = (ym2609Register[0][0x18 + ch - 13] & 0xc0) >> 6;
                newParam.channels[ch].volumeL = Math.Min(Math.Max(ym2609Rhythm[ch -30][0] / 80, 0), 19);
                newParam.channels[ch].volumeR = Math.Min(Math.Max(ym2609Rhythm[ch - 30][1] / 80, 0), 19);
               // newParam.channels[ch].volumeRL = ym2609Register[0][ch - 13 + 0x18] & 0x1f;
            }


            //ADPCM012
            for (int ch = 42; ch < 45; ch++)
            {
                int p = ch == 42 ? 1 : 3;
                int sft = ch != 44 ? 0 : 0x11;

                newParam.channels[ch].panL = ym2609AdpcmPan[ch - 42][0];
                newParam.channels[ch].panR = ym2609AdpcmPan[ch - 42][1];
                newParam.channels[ch].volumeL = Math.Min(Math.Max(ym2609AdpcmVol[ch - 42][0] / 80, 0), 19);
                newParam.channels[ch].volumeR = Math.Min(Math.Max(ym2609AdpcmVol[ch - 42][1] / 80, 0), 19);
                int delta = (ym2609Register[p][sft + 0x0a] << 8) | ym2609Register[p][sft + 0x09];
                newParam.channels[ch].freq = delta;
                float frq = (float)(delta / 9447.0f);
                newParam.channels[ch].note = (ym2609Register[p][sft + 0x00] & 0x80) != 0 ? (Common.searchYM2608Adpcm(frq) - 1) : -1;
                if ((ym2609Register[p][sft + 0x01] & 0xc0) == 0)
                {
                    newParam.channels[ch].note = -1;
                }
            }

        }

        private int[] psgPort = new int[4] { 0, 1, 2, 2 };
        private int[] psgAdr = new int[4] { 0x00, 0x20, 0x00, 0x10 };

        public void screenDrawParams()
        {
            //bool ChipType2 = (chipID == 0)
            //    ? parent.setting.YM2609Type[0].UseReal[0]
            //    : parent.setting.YM2609Type[1].UseReal[0];
            //int chipSoundLocation = (chipID == 0)
            //    ? parent.setting.YM2609Type[0].realChipInfo[0].SoundLocation
            //    : parent.setting.YM2609Type[1].realChipInfo[0].SoundLocation;
            //int tp = !ChipType2 ? 0 : (chipSoundLocation < 0 ? 2 : 1);
            int tp = 0;

            for (int c = 0; c < 18; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                if (c == 2 || c == 8)
                {
                    DrawBuff.Volume(frameBuffer, 288 + 1, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 288 + 1, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.PanType4(frameBuffer, 6 * 4 + 1, 8 + c * 8, ref oyc.pan, nyc.pan, tp);
                    DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + c * 8, ref oyc.note, nyc.note, tp);
                    DrawBuff.InstOPNA2(frameBuffer,
                        (c % 4) * 4 * 35 + 368 + 1,
                        (c / 4) * 8 * 6 + 16,
                        c, oyc.inst, nyc.inst);
                    DrawBuff.Ch3YM2608(frameBuffer, c, ref oyc.mask, nyc.mask, ref oyc.ex, nyc.ex, tp);
                    DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }
                else if (c < 12)
                {
                    DrawBuff.Volume(frameBuffer, 288 + 1, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 288 + 1, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.PanType4(frameBuffer, 6 * 4 + 1, 8 + c * 8, ref oyc.pan, nyc.pan, tp);
                    DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + c * 8, ref oyc.note, nyc.note, tp);
                    DrawBuff.InstOPNA2(frameBuffer,
                        (c % 4) * 4 * 35 + 368 + 1,
                        (c / 4) * 8 * 6 + 16,
                        c, oyc.inst, nyc.inst);
                    DrawBuff.ChYM2608(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                    DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }
                else
                {
                    DrawBuff.Volume(frameBuffer, 288 + 1, 8 + c * 8, 0, ref oyc.volumeL, nyc.volumeL, tp);

                    DrawBuff.KeyBoardOPNA(frameBuffer, 33, 8 + c * 8, ref oyc.note, nyc.note, tp);
                    DrawBuff.ChYM2608(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }

            }

            //PSG
            for (int c = 0; c < 12; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 18];
                MDChipParams.Channel nyc = newParam.channels[c + 18];

                DrawBuff.Volume(frameBuffer, 289, 8 + (c + 18) * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoardOPNA(frameBuffer, 33, (c + 18) * 8 + 8, ref oyc.note, nyc.note, tp);
                DrawBuff.Pan(frameBuffer, 25, 8 + (c + 18) * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                DrawBuff.TnOPNA(frameBuffer, 64, 2, c + 18, ref oyc.tn, nyc.tn, ref oyc.tntp, tp * 2);

                DrawBuff.ChYM2608(frameBuffer, c + 18, ref oyc.mask, nyc.mask, tp);
                DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + (c + 18) * 8, 0, ref oyc.freq, nyc.freq);

                DrawBuff.font4YM2609Duty(frameBuffer,
                    1 + 4 * 92 + (c % 6) * 4 * 14 + (((c / 3) & 1) == 0 ? 0 : (4 * 16)),
                    8 * 21 + (c / 6) * 8 * 6,
                    0, ref oyc.bank, nyc.bank);

                if (nyc.bank < 0) continue;
                if (nyc.bank <10)
                {
                    DrawBuff.WaveFormYM2609Preset(frameBuffer,
                        1 + 4 * 97 + (c % 6) * 4 * 14 + (((c / 3) & 1) == 0 ? 0 : (4 * 16)),
                        20 * 8 + (c / 6) * 8 * 6,
                        ref oyc.volumeRR, nyc.bank);
                    continue;
                }

                DrawBuff.WaveFormYM2609User(frameBuffer,
                    1 + 4 * 97 + (c % 6) * 4 * 14 + (((c / 3) & 1) == 0 ? 0 : (4 * 16)),
                    23 * 8 + (c / 6) * 8 * 6,
                    ref oyc.PSGWave, nyc.PSGWave);
            }

            for (int c = 0; c < 6; c++)
            {
                //Rhythm
                MDChipParams.Channel oyc = oldParam.channels[c + 30];
                MDChipParams.Channel nyc = newParam.channels[c + 30];
                DrawBuff.VolumeYM2609Rhythm(frameBuffer, 4 * 97 + 1 + c * 64, 8 * 32, ref oyc.volumeL, nyc.volumeL, 0);
                DrawBuff.VolumeYM2609Rhythm(frameBuffer, 4 * 97 + 1 + c * 64, 8 * 32+4, ref oyc.volumeR, nyc.volumeR, 0);
                //DrawBuff.PanYM2608Rhythm(frameBuffer, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                //DrawBuff.font4Int2(frameBuffer, c * 4 * 15 + 4, 28 * 4, 0, 0, ref oyc.volumeRL, nyc.volumeRL);

                //ADPCMA
                oyc = oldParam.channels[c + 36];
                nyc = newParam.channels[c + 36];
                DrawBuff.VolumeYM2609Rhythm(frameBuffer, 4 * 97 + 1 + c * 64, 8 * 33, ref oyc.volumeL, nyc.volumeL, 0);
                DrawBuff.VolumeYM2609Rhythm(frameBuffer, 4 * 97 + 1 + c * 64, 8 * 33 + 4, ref oyc.volumeR, nyc.volumeR, 0);
                //DrawBuff.PanYM2608Rhythm(frameBuffer, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                //DrawBuff.font4Int2(frameBuffer, c * 4 * 15 + 4, 28 * 4, 0, 0, ref oyc.volumeRL, nyc.volumeRL);
            }
            //DrawBuff.ChYM2608Rhythm(frameBuffer, 0, ref oldParam.channels[30].mask, newParam.channels[30].mask, tp);

            for (int c = 0; c < 3; c++)
            {
                //ADPCM
                MDChipParams.Channel oyc = oldParam.channels[c + 42];
                MDChipParams.Channel nyc = newParam.channels[c + 42];

                DrawBuff.Volume(frameBuffer, 289, (31 + c) * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.Volume(frameBuffer, 289, (31 + c) * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                DrawBuff.PanType5(frameBuffer, 25, (31 + c) * 8, ref oyc.panL, nyc.panL, tp);
                DrawBuff.PanType5(frameBuffer, 29, (31 + c) * 8, ref oyc.panR, nyc.panR, tp);
                DrawBuff.KeyBoardOPNA(frameBuffer, 33, (31 + c) * 8, ref oyc.note, nyc.note, tp);
                //DrawBuff.ChYM2608(frameBuffer, 12, ref oyc.mask, nyc.mask, tp);
                DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, (31+c) * 8, 0, ref oyc.freq, nyc.freq);
            }
        }
    }
}
