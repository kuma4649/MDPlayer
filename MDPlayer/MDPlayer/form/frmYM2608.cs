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
    public partial class frmYM2608 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2608 newParam = null;
        private MDChipParams.YM2608 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2608(frmMain frm, int chipID, int zoom, MDChipParams.YM2608 newParam, MDChipParams.YM2608 oldParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeD, null, zoom);
            bool YM2608Type = (chipID == 0) ? parent.setting.YM2608Type.UseScci : parent.setting.YM2608SType.UseScci;
            int YM2608SoundLocation = (chipID == 0) ? parent.setting.YM2608Type.SoundLocation : parent.setting.YM2608SType.SoundLocation;
            int tp = !YM2608Type ? 0 : (YM2608SoundLocation < 0 ? 2 : 1);
            DrawBuff.screenInitYM2608(frameBuffer, tp);
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

        private void frmYM2608_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmYM2608_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2608[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm2608[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeD.Width * zoom, frameSizeH + Properties.Resources.planeD.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeD.Width * zoom, frameSizeH + Properties.Resources.planeD.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeD.Width * zoom, frameSizeH + Properties.Resources.planeD.Height * zoom);
            frmYM2608_Resize(null, null);

        }

        private void frmYM2608_Resize(object sender, EventArgs e)
        {

        }

        protected override void WndProc(ref Message m)
        {
            if (parent != null)
            {
                parent.windowsMessage(ref m);
            }

            try { base.WndProc(ref m); }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
            }
            bool YM2608Type = (chipID == 0) ? parent.setting.YM2608Type.UseScci : parent.setting.YM2608SType.UseScci;
            int YM2608SoundLocation = (chipID == 0) ? parent.setting.YM2608Type.SoundLocation : parent.setting.YM2608SType.SoundLocation;
            int tp = !YM2608Type ? 0 : (YM2608SoundLocation < 0 ? 2 : 1);
            DrawBuff.screenInitYM2608(frameBuffer, tp);
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

        public void screenChangeParams()
        {
            bool isFmEx;
            int[][] ym2608Register = Audio.GetYM2608Register(chipID);
            int[] fmKeyYM2608 = Audio.GetYM2608KeyOn(chipID);
            int[] ym2608Vol = Audio.GetYM2608Volume(chipID);
            int[] ym2608Ch3SlotVol = Audio.GetYM2608Ch3SlotVolume(chipID);
            int[][] ym2608Rhythm = Audio.GetYM2608RhythmVolume(chipID);
            int[] ym2608AdpcmVol = Audio.GetYM2608AdpcmVolume(chipID);
            //int masterClock = Audio.clockYM2608;
            //int defaultMasterClock = 8000000;
            //float mul = 1.0f;
            //if (masterClock != 0)
            //    mul = masterClock / (float)defaultMasterClock;

            isFmEx = (ym2608Register[0][0x27] & 0x40) > 0;
            newParam.channels[2].ex = isFmEx;

            int defaultMasterClock = 7987200;
            float ssgMul = 1.0f;
            int masterClock = defaultMasterClock;
            if (Audio.clockYM2608 != 0)
            {
                ssgMul = Audio.clockYM2608 / (float)defaultMasterClock;
                masterClock = Audio.clockYM2608;
            }

            float fmDiv = fmDivTbl[ym2608Register[0][0x2d]];
            float ssgDiv = ssgDivTbl[ym2608Register[0][0x2d]];
            ssgMul = ssgMul * ssgDiv / 4;

            newParam.lfoSw = (ym2608Register[0][0x22] & 0x8) != 0;
            newParam.lfoFrq = (ym2608Register[0][0x22] & 0x7);

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.channels[ch].inst[i * 11 + 0] = ym2608Register[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.channels[ch].inst[i * 11 + 1] = ym2608Register[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.channels[ch].inst[i * 11 + 2] = ym2608Register[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.channels[ch].inst[i * 11 + 3] = ym2608Register[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.channels[ch].inst[i * 11 + 4] = (ym2608Register[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.channels[ch].inst[i * 11 + 5] = ym2608Register[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.channels[ch].inst[i * 11 + 6] = (ym2608Register[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.channels[ch].inst[i * 11 + 7] = ym2608Register[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.channels[ch].inst[i * 11 + 8] = (ym2608Register[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.channels[ch].inst[i * 11 + 9] = (ym2608Register[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.channels[ch].inst[i * 11 + 10] = ym2608Register[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.channels[ch].inst[44] = ym2608Register[p][0xb0 + c] & 0x07;//AL
                newParam.channels[ch].inst[45] = (ym2608Register[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.channels[ch].inst[46] = (ym2608Register[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.channels[ch].inst[47] = ym2608Register[p][0xb4 + c] & 0x07;//FMS

                newParam.channels[ch].pan = (ym2608Register[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    octav = (ym2608Register[p][0xa4 + c] & 0x38) >> 3;
                    freq = ym2608Register[p][0xa0 + c] + (ym2608Register[p][0xa4 + c] & 0x07) * 0x100;
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2608[ch]&1) != 0)
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    byte con = (byte)(fmKeyYM2608[ch]);
                    int v = 127;
                    int m = md[ym2608Register[p][0xb0 + c] & 7];
                    //OP1
                    v = (((con & 0x10) != 0) && ((m & 0x10) != 0) && v > (ym2608Register[p][0x40 + c] & 0x7f)) ? (ym2608Register[p][0x40 + c] & 0x7f) : v;
                    //OP3
                    v = (((con & 0x20) != 0) && ((m & 0x20) != 0) && v > (ym2608Register[p][0x44 + c] & 0x7f)) ? (ym2608Register[p][0x44 + c] & 0x7f) : v;
                    //OP2
                    v = (((con & 0x40) != 0) && ((m & 0x40) != 0) && v > (ym2608Register[p][0x48 + c] & 0x7f)) ? (ym2608Register[p][0x48 + c] & 0x7f) : v;
                    //OP4
                    v = (((con & 0x80) != 0) && ((m & 0x80) != 0) && v > (ym2608Register[p][0x4c + c] & 0x7f)) ? (ym2608Register[p][0x4c + c] & 0x7f) : v;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2608Register[p][0xb4 + c] & 0x80) != 0 ? 1 : 0) * ym2608Vol[ch] / 80.0), 0), 19);
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2608Register[p][0xb4 + c] & 0x80) != 0 ? 1 : 0) * ym2608Vol[ch] / 80.0), 0), 19);
                }
                else
                {
                    int m = md[ym2608Register[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    freq = ym2608Register[0][0xa9] + (ym2608Register[0][0xad] & 0x07) * 0x100;
                    octav = (ym2608Register[0][0xad] & 0x38) >> 3;
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKeyYM2608[2] & 0x10) > 0 && ((m & 0x10) != 0))
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    int v = ((m & 0x10) != 0) ? ym2608Register[p][0x40 + c] : 127;
                    newParam.channels[2].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2608Register[0][0xb4 + 2] & 0x80) != 0 ? 1 : 0) * ym2608Ch3SlotVol[0] / 80.0), 0), 19);
                    newParam.channels[2].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((ym2608Register[0][0xb4 + 2] & 0x80) != 0 ? 1 : 0) * ym2608Ch3SlotVol[0] / 80.0), 0), 19);
                }
                newParam.channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++) //FM EX
            {
                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.channels[ch].pan = 0;

                if (isFmEx)
                {
                    int m = md[ym2608Register[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    int op = ch - 5;
                    op = op == 1 ? 2 : (op == 2 ? 1 : op);

                    int freq = ym2608Register[0][0xa8 + c] + (ym2608Register[0][0xac + c] & 0x07) * 0x100;
                    int octav = (ym2608Register[0][0xac + c] & 0x38) >> 3;
                    int n = -1;
                    if ((fmKeyYM2608[2] & (0x10 << (ch - 5))) != 0 && ((m & (0x10 << op)) != 0))
                    {
                        float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                        ff /= 1038f;
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);
                    }
                    newParam.channels[ch].note = n;

                    int v = ((m & (0x10 << op)) != 0) ? ym2608Register[0][0x42 + op * 4] : 127;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ym2608Ch3SlotVol[ch - 5] / 80.0), 0), 19);
                }
                else
                {
                    newParam.channels[ch].note = -1;
                    newParam.channels[ch].volumeL = 0;
                }
            }

            for (int ch = 0; ch < 3; ch++) //SSG
            {
                MDChipParams.Channel channel = newParam.channels[ch + 9];

                bool t = (ym2608Register[0][0x07] & (0x1 << ch)) == 0;
                bool n = (ym2608Register[0][0x07] & (0x8 << ch)) == 0;
                channel.tn = (t ? 1 : 0) + (n ? 2 : 0);

                channel.volume = (int)(((t || n) ? 1 : 0) * (ym2608Register[0][0x08 + ch] & 0xf) * (20.0 / 16.0));
                if (!t && !n && channel.volume > 0)
                {
                    channel.volume--;
                }

                if (channel.volume == 0)
                {
                    channel.note = -1;
                }
                else
                {
                    int ft = ym2608Register[0][0x00 + ch * 2];
                    int ct = ym2608Register[0][0x01 + ch * 2];
                    int tp = (ct << 8) | ft;
                    if (tp == 0) tp = 1;
                    float ftone = 7987200.0f / (64.0f * (float)tp) * ssgMul;// 7987200 = MasterClock
                    channel.note = Common.searchSSGNote(ftone);
                }

            }

            newParam.nfrq = ym2608Register[0][0x06] & 0x1f;
            newParam.efrq = ym2608Register[0][0x0c] * 0x100 + ym2608Register[0][0x0b];
            newParam.etype = (ym2608Register[0][0x0d] & 0x7) + 2;

            //ADPCM
            newParam.channels[12].pan = (ym2608Register[1][0x01] & 0xc0) >> 6; // ((ym2608Register[1][0x01] & 0xc0) >> 6) != 0 ? ((ym2608Register[1][0x01] & 0xc0) >> 6) : newParam.channels[12].pan;
            newParam.channels[12].volumeL = Math.Min(Math.Max(ym2608AdpcmVol[0] / 80, 0), 19);
            newParam.channels[12].volumeR = Math.Min(Math.Max(ym2608AdpcmVol[1] / 80, 0), 19);
            int delta = (ym2608Register[1][0x0a] << 8) | ym2608Register[1][0x09];
            float frq = (float)(delta / 9447.0f);
            newParam.channels[12].note = (ym2608Register[1][0x00] & 0x80) != 0 ? (Common.searchYM2608Adpcm(frq)-1) : -1;
            if ((ym2608Register[1][0x01] & 0xc0) == 0)
            {
                newParam.channels[12].note = -1;
            }

            for (int ch = 13; ch < 19; ch++) //RHYTHM
            {
                newParam.channels[ch].pan = (ym2608Register[0][0x18 + ch - 13] & 0xc0) >> 6;
                newParam.channels[ch].volumeL = Math.Min(Math.Max(ym2608Rhythm[ch - 13][0] / 80, 0), 19);
                newParam.channels[ch].volumeR = Math.Min(Math.Max(ym2608Rhythm[ch - 13][1] / 80, 0), 19);
            }

        }


        public void screenDrawParams()
        {
            bool chipType = (chipID == 0) ? parent.setting.YM2608Type.UseScci : parent.setting.YM2608SType.UseScci;
            int chipSoundLocation = (chipID == 0) ? parent.setting.YM2608Type.SoundLocation : parent.setting.YM2608SType.SoundLocation;
            int tp = !chipType ? 0 : (chipSoundLocation < 0 ? 2 : 1);

            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                if (c ==2)
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.Inst(frameBuffer, 1, 17, c, oyc.inst, nyc.inst);
                    DrawBuff.Ch3YM2608(frameBuffer, c, ref oyc.mask, nyc.mask, ref oyc.ex, nyc.ex, tp);
                }
                else if (c < 6)
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 256, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 24, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.Inst(frameBuffer, 1, 17, c, oyc.inst, nyc.inst);
                    DrawBuff.ChYM2608(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                }
                else
                {
                    DrawBuff.Volume(frameBuffer, 256, 8 + (c+3) * 8, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    //if (c == 7 && oyc.note != nyc.note)
                    //{
                        //Console.WriteLine("note:{0}", nyc.note);
                        //int[][] ym2608Register = Audio.GetYM2608Register(chipID);
                        //int freq1 = ym2608Register[0][0xa9] + (ym2608Register[0][0xad] ) * 0x100;
                        //int freq2 = ym2608Register[0][0xa8] + (ym2608Register[0][0xac] ) * 0x100;
                        //int freq3 = ym2608Register[0][0xaa] + (ym2608Register[0][0xae] ) * 0x100;
                        //int freq4 = ym2608Register[0][0xa2] + (ym2608Register[0][0xa6] ) * 0x100;
                        //Console.WriteLine("frq:{0:x4} {1:x4} {2:x4} {3:x4}", freq1, freq2, freq3, freq4);
                    //}
                    DrawBuff.KeyBoard(frameBuffer, c + 3, ref oyc.note, nyc.note, tp);
                    DrawBuff.ChYM2608(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                }


            }
            //SSG
            for (int c = 0; c < 3; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 9];
                MDChipParams.Channel nyc = newParam.channels[c + 9];

                DrawBuff.Volume(frameBuffer, 256, 8 + (c+6) * 8, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.KeyBoard(frameBuffer, c + 6, ref oyc.note, nyc.note, tp);
                DrawBuff.Tn(frameBuffer, 6, 2, c + 6, ref oyc.tn, nyc.tn, ref oyc.tntp, tp*2);

                DrawBuff.ChYM2608(frameBuffer, c + 9, ref oyc.mask, nyc.mask, tp);
            }

            DrawBuff.Volume(frameBuffer, 256, 8 + 12 * 8, 1, ref oldParam.channels[12].volumeL, newParam.channels[12].volumeL, tp);
            DrawBuff.Volume(frameBuffer, 256, 8 + 12 * 8, 2, ref oldParam.channels[12].volumeR, newParam.channels[12].volumeR, tp);
            DrawBuff.Pan(frameBuffer, 24, 8 + 12 * 8, ref oldParam.channels[12].pan, newParam.channels[12].pan, ref oldParam.channels[12].pantp, tp);
            DrawBuff.KeyBoard(frameBuffer, 12, ref oldParam.channels[12].note, newParam.channels[12].note, tp);
            DrawBuff.ChYM2608(frameBuffer, 12, ref oldParam.channels[12].mask, newParam.channels[12].mask, tp);

            for (int c = 0; c < 6; c++)
            {
                MDChipParams.Channel oyc = oldParam.channels[c + 13];
                MDChipParams.Channel nyc = newParam.channels[c + 13];

                DrawBuff.VolumeYM2608Rhythm(frameBuffer, c, 1, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.VolumeYM2608Rhythm(frameBuffer, c, 2, ref oyc.volumeR, nyc.volumeR, tp);
                DrawBuff.PanYM2608Rhythm(frameBuffer, c, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
            }
            DrawBuff.ChYM2608Rhythm(frameBuffer, 0, ref oldParam.channels[13].mask, newParam.channels[13].mask, tp);

            DrawBuff.LfoSw(frameBuffer, 16, 216, ref oldParam.lfoSw, newParam.lfoSw);
            DrawBuff.LfoFrq(frameBuffer, 64, 216, ref oldParam.lfoFrq, newParam.lfoFrq);

            DrawBuff.Nfrq(frameBuffer, 25, 54, ref oldParam.nfrq, newParam.nfrq);
            DrawBuff.Efrq(frameBuffer, 38, 54, ref oldParam.efrq, newParam.efrq);
            DrawBuff.Etype(frameBuffer, 53, 54, ref oldParam.etype, newParam.etype);

        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;

            int ch = (py / 8) - 1;

            if (ch < 0) return;

            if (ch < 14)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.YM2608, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 14; ch++) parent.ResetChannelMask(EnmChip.YM2608, chipID, ch);
                return;
            }

            // 音色表示欄の判定

            int h = (py - 15 * 8) / (6 * 8);
            int w = Math.Min(px / (13 * 8), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(EnmChip.YM2608, instCh, chipID);
            }
        }

    }
}