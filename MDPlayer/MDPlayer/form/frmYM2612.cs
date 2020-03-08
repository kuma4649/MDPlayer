using System;
using System.Drawing;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class frmYM2612 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM2612 newParam = null;
        private MDChipParams.YM2612 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM2612(frmMain frm, int chipID, int zoom, MDChipParams.YM2612 newParam, MDChipParams.YM2612 oldParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeYM2612, null, zoom);
            screenInit();
            update();
        }

        public void screenInit()
        {
            bool YM2612Type = (chipID == 0) ? parent.setting.YM2612Type.UseScci : parent.setting.YM2612SType.UseScci;
            int tp = YM2612Type ? 1 : 0;
            DrawBuff.screenInitYM2612(frameBuffer, tp, (chipID == 0) ? parent.setting.YM2612Type.OnlyPCMEmulation : parent.setting.YM2612SType.OnlyPCMEmulation, newParam.fileFormat == EnmFileFormat.XGM);
            newParam.channels[5].pcmBuff = 100;
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

        private void frmYM2612_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm2612[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm2612[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYM2612_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612.Width * zoom, frameSizeH + Properties.Resources.planeYM2612.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612.Width * zoom, frameSizeH + Properties.Resources.planeYM2612.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeYM2612.Width * zoom, frameSizeH + Properties.Resources.planeYM2612.Height * zoom);
            frmYM2612_Resize(null, null);

        }

        private void frmYM2612_Resize(object sender, EventArgs e)
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

        public void screenChangeParams()
        {
            int[][] fmRegister = Audio.GetFMRegister(chipID);
            int[] fmVol = Audio.GetFMVolume(chipID);
            int[] fmCh3SlotVol = Audio.GetFMCh3SlotVolume(chipID);
            int[] fmKey = Audio.GetFMKeyOn(chipID);

            bool isFmEx = (fmRegister[0][0x27] & 0x40) != 0;
            newParam.channels[2].ex = isFmEx;

            newParam.lfoSw = (fmRegister[0][0x22] & 0x8) != 0;
            newParam.lfoFrq = (fmRegister[0][0x22] & 0x7);
            newParam.timerA = fmRegister[0][0x24] | ((fmRegister[0][0x25] & 0x3) << 8);
            newParam.timerB = fmRegister[0][0x26];

            //int masterClock = Audio.clockYM2612;
            //int defaultMasterClock = 8000000;
            //float mul = 1.0f;
            //if (masterClock != 0)
            //    mul = masterClock / (float)defaultMasterClock;

            int defaultMasterClock = 8000000;
            float ssgMul = 1.0f;
            int masterClock = defaultMasterClock;
            if (Audio.clockYM2612 != 0)
            {
                ssgMul = Audio.clockYM2612 / (float)defaultMasterClock;
                masterClock = Audio.clockYM2612;
            }

            float fmDiv = 6;
            float ssgDiv = 4;
            ssgMul = ssgMul * ssgDiv / 4;

            for (int ch = 0; ch < 6; ch++)
            {
                int p = (ch > 2) ? 1 : 0;
                int c = (ch > 2) ? ch - 3 : ch;
                newParam.channels[ch].slot = (byte)(fmKey[ch] >> 4);
                for (int i = 0; i < 4; i++)
                {
                    int ops = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                    newParam.channels[ch].inst[i * 11 + 0] = fmRegister[p][0x50 + ops + c] & 0x1f; //AR
                    newParam.channels[ch].inst[i * 11 + 1] = fmRegister[p][0x60 + ops + c] & 0x1f; //DR
                    newParam.channels[ch].inst[i * 11 + 2] = fmRegister[p][0x70 + ops + c] & 0x1f; //SR
                    newParam.channels[ch].inst[i * 11 + 3] = fmRegister[p][0x80 + ops + c] & 0x0f; //RR
                    newParam.channels[ch].inst[i * 11 + 4] = (fmRegister[p][0x80 + ops + c] & 0xf0) >> 4;//SL
                    newParam.channels[ch].inst[i * 11 + 5] = fmRegister[p][0x40 + ops + c] & 0x7f;//TL
                    newParam.channels[ch].inst[i * 11 + 6] = (fmRegister[p][0x50 + ops + c] & 0xc0) >> 6;//KS
                    newParam.channels[ch].inst[i * 11 + 7] = fmRegister[p][0x30 + ops + c] & 0x0f;//ML
                    newParam.channels[ch].inst[i * 11 + 8] = (fmRegister[p][0x30 + ops + c] & 0x70) >> 4;//DT
                    newParam.channels[ch].inst[i * 11 + 9] = (fmRegister[p][0x60 + ops + c] & 0x80) >> 7;//AM
                    newParam.channels[ch].inst[i * 11 + 10] = fmRegister[p][0x90 + ops + c] & 0x0f;//SG
                }
                newParam.channels[ch].inst[44] = fmRegister[p][0xb0 + c] & 0x07;//AL
                newParam.channels[ch].inst[45] = (fmRegister[p][0xb0 + c] & 0x38) >> 3;//FB
                newParam.channels[ch].inst[46] = (fmRegister[p][0xb4 + c] & 0x38) >> 4;//AMS
                newParam.channels[ch].inst[47] = fmRegister[p][0xb4 + c] & 0x07;//FMS

                newParam.channels[ch].pan = (fmRegister[p][0xb4 + c] & 0xc0) >> 6;

                int freq = 0;
                int octav = 0;
                int n = -1;
                if (ch != 2 || !isFmEx)
                {
                    freq = fmRegister[p][0xa0 + c] + (fmRegister[p][0xa4 + c] & 0x07) * 0x100;
                    octav = (fmRegister[p][0xa4 + c] & 0x38) >> 3;
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKey[ch] & 1) != 0)
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    byte con = (byte)(fmKey[ch]);
                    int v = 127;
                    int m = md[fmRegister[p][0xb0 + c] & 7];

                    //OP1
                    v = (((con & 0x10) != 0) && ((m & 0x10) != 0) && v > (fmRegister[p][0x40 + c] & 0x7f)) ? (fmRegister[p][0x40 + c] & 0x7f) : v;
                    //OP3
                    v = (((con & 0x20) != 0) && ((m & 0x20) != 0) && v > (fmRegister[p][0x44 + c] & 0x7f)) ? (fmRegister[p][0x44 + c] & 0x7f) : v;
                    //OP2
                    v = (((con & 0x40) != 0) && ((m & 0x40) != 0) && v > (fmRegister[p][0x48 + c] & 0x7f)) ? (fmRegister[p][0x48 + c] & 0x7f) : v;
                    //OP4
                    v = (((con & 0x80) != 0) && ((m & 0x80) != 0) && v > (fmRegister[p][0x4c + c] & 0x7f)) ? (fmRegister[p][0x4c + c] & 0x7f) : v;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((fmRegister[p][0xb4 + c] & 0x80) != 0 ? 1 : 0) * fmVol[ch] / 80.0), 0), 19);
                    newParam.channels[ch].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((fmRegister[p][0xb4 + c] & 0x40) != 0 ? 1 : 0) * fmVol[ch] / 80.0), 0), 19);
                }
                else
                {
                    int m = md[fmRegister[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    freq = fmRegister[0][0xa9] + (fmRegister[0][0xad] & 0x07) * 0x100;
                    octav = (fmRegister[0][0xad] & 0x38) >> 3;
                    newParam.channels[2].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                    ff /= 1038f;

                    if ((fmKey[2] & 0x10) != 0 && ((m & 0x10) != 0))
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);

                    int v = ((m & 0x10) != 0) ? fmRegister[p][0x40 + c] : 127;
                    newParam.channels[2].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((fmRegister[0][0xb4 + 2] & 0x80) != 0 ? 1 : 0) * fmCh3SlotVol[0] / 80.0), 0), 19);
                    newParam.channels[2].volumeR = Math.Min(Math.Max((int)((127 - v) / 127.0 * ((fmRegister[0][0xb4 + 2] & 0x40) != 0 ? 1 : 0) * fmCh3SlotVol[0] / 80.0), 0), 19);
                }
                newParam.channels[ch].note = n;


            }

            for (int ch = 6; ch < 9; ch++)
            {
                //Operator 1′s frequency is in A9 and ADH
                //Operator 2′s frequency is in AA and AEH
                //Operator 3′s frequency is in A8 and ACH
                //Operator 4′s frequency is in A2 and A6H

                int[] exReg = new int[3] { 2, 0, -6 };
                int c = exReg[ch - 6];

                newParam.channels[ch].pan = 0;

                if (isFmEx)
                {
                    int m = md[fmRegister[0][0xb0 + 2] & 7];
                    if (parent.setting.other.ExAll) m = 0xf0;
                    int op = ch - 5;
                    op = op == 1 ? 2 : (op == 2 ? 1 : op);

                    int freq = fmRegister[0][0xa8 + c] + (fmRegister[0][0xac + c] & 0x07) * 0x100;
                    int octav = (fmRegister[0][0xac + c] & 0x38) >> 3;
                    newParam.channels[ch].freq = (freq & 0x7ff) | ((octav & 7) << 11);
                    int n = -1;
                    if ((fmKey[2] & (0x10 << (ch-5))) != 0 && ((m & (0x10 << op)) != 0))
                    {
                        float ff = freq / ((2 << 20) / (masterClock / (24 * fmDiv))) * (2 << (octav + 2));
                        ff /= 1038f;
                        n = Math.Min(Math.Max(Common.searchYM2608Adpcm(ff) - 1, 0), 95);
                    }
                    newParam.channels[ch].note = n;

                    int v = ((m & (0x10 << op)) != 0) ? fmRegister[0][0x42 + op * 4] : 127;
                    newParam.channels[ch].volumeL = Math.Min(Math.Max((int)((127 - v) / 127.0 * fmCh3SlotVol[ch - 5] / 80.0), 0), 19);
                }
                else
                {
                    newParam.channels[ch].note = -1;
                    newParam.channels[ch].volumeL = 0;
                }
            }

            newParam.channels[5].pcmMode = (fmRegister[0][0x2b] & 0x80) >> 7;
            if (newParam.channels[5].pcmBuff > 0)
                newParam.channels[5].pcmBuff--;
            if (newParam.channels[5].pcmMode!=0)
            {
                newParam.channels[5].volumeL = Math.Min(Math.Max(fmVol[5] / 80, 0), 19);
                newParam.channels[5].volumeR = Math.Min(Math.Max(fmVol[5] / 80, 0), 19);
            }

            if (newParam.fileFormat == EnmFileFormat.XGM && Audio.driverVirtual is xgm)
            {
                if (Audio.driverVirtual != null && ((xgm)Audio.driverVirtual).xgmpcm != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (((xgm)Audio.driverVirtual).xgmpcm[i].isPlaying)
                        {
                            newParam.xpcmInst[i] = (int)(((xgm)Audio.driverVirtual).xgmpcm[i].inst);
                            int d = (((xgm)Audio.driverVirtual).xgmpcm[i].data / 6);
                            d = Math.Min(d, 19);
                            newParam.xpcmVolL[i] = d;
                            newParam.xpcmVolR[i] = d;
                        }
                        else
                        {
                            newParam.xpcmInst[i] = 0;
                            newParam.xpcmVolL[i] = 0;
                            newParam.xpcmVolR[i] = 0;
                        }
                    }
                }
            }
        }

        public void screenDrawParams()
        {
            for (int c = 0; c < 9; c++)
            {

                MDChipParams.Channel oyc = oldParam.channels[c];
                MDChipParams.Channel nyc = newParam.channels[c];

                bool YM2612type = (chipID == 0) ? parent.setting.YM2612Type.UseScci : parent.setting.YM2612SType.UseScci;
                int tp = YM2612type ? 1 : 0;

                if (c == 2)
                {
                    DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 25, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoardOPNM(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.InstOPN2(frameBuffer, 13, 96, c, oyc.inst, nyc.inst);
                    DrawBuff.Ch3YM2612(frameBuffer, c, ref oyc.mask, nyc.mask, ref oyc.ex, nyc.ex, tp);
                    DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }
                else if (c < 5)
                {
                    DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp);
                    DrawBuff.Pan(frameBuffer, 25, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp);
                    DrawBuff.KeyBoardOPNM(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.InstOPN2(frameBuffer, 13, 96, c, oyc.inst, nyc.inst);
                    DrawBuff.ChYM2612(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                    DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }
                else if (c == 5)
                {
                    int tp6 = tp;
                    int tp6v = tp;
                    if (tp6 == 1 && parent.setting.YM2612Type.OnlyPCMEmulation)
                    {
                        tp6v = newParam.channels[5].pcmMode == 0 ? 1 : 0;//volumeのみモードの判定を行う
                                                                         //tp6 = 0;
                    }

                    DrawBuff.Pan(frameBuffer, 25, 8 + c * 8, ref oyc.pan, nyc.pan, ref oyc.pantp, tp6v);
                    DrawBuff.InstOPN2(frameBuffer, 13, 96, c, oyc.inst, nyc.inst);

                    if (newParam.fileFormat != EnmFileFormat.XGM)
                    {
                        DrawBuff.Ch6YM2612(frameBuffer, nyc.pcmBuff, ref oyc.pcmMode, nyc.pcmMode, ref oyc.mask, nyc.mask, ref oyc.tp, tp6v);
                        DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp6v);
                        DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp6v);
                        DrawBuff.KeyBoardOPNM(frameBuffer, c, ref oyc.note, nyc.note, tp6v);
                        DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                        DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                    }
                    else
                    {
                        DrawBuff.Ch6YM2612XGM(frameBuffer,nyc.pcmBuff, ref oyc.pcmMode, nyc.pcmMode, ref oyc.mask, nyc.mask, ref oyc.tp, tp6v);
                        if (newParam.channels[5].pcmMode == 0)
                        {
                            DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 1, ref oyc.volumeL, nyc.volumeL, tp6v);
                            DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 2, ref oyc.volumeR, nyc.volumeR, tp6v);
                            DrawBuff.KeyBoardOPNM(frameBuffer, c, ref oyc.note, nyc.note, tp6v);
                            DrawBuff.Slot(frameBuffer, 1 + 4 * 64, 8 + c * 8, ref oyc.slot, nyc.slot);
                            DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                        }
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                DrawBuff.VolumeXYOPN2(frameBuffer, (13 + i * 17) * 4 + 1, 12 * 4, 1, ref oldParam.xpcmVolL[i], newParam.xpcmVolL[i], tp6v);
                                DrawBuff.VolumeXYOPN2(frameBuffer, (13 + i * 17) * 4 + 1, 12 * 4, 2, ref oldParam.xpcmVolR[i], newParam.xpcmVolR[i], tp6v);
                                if (oldParam.xpcmInst[i] != newParam.xpcmInst[i])
                                {
                                    DrawBuff.drawFont4Int2(frameBuffer, 45 + i * 17 * 4, 48, tp6v, 2, newParam.xpcmInst[i]);
                                    oldParam.xpcmInst[i] = newParam.xpcmInst[i];
                                }
                            }
                        }
                    }
                }
                else
                {
                    DrawBuff.Volume(frameBuffer, 289, 8 + c * 8, 0, ref oyc.volumeL, nyc.volumeL, tp);
                    DrawBuff.KeyBoardOPNM(frameBuffer, c, ref oyc.note, nyc.note, tp);
                    DrawBuff.ChYM2612(frameBuffer, c, ref oyc.mask, nyc.mask, tp);
                    oyc.freq = 0;
                    DrawBuff.font4Hex16Bit(frameBuffer, 1 + 4 * 68, 8 + c * 8, 0, ref oyc.freq, nyc.freq);
                }

            }

            DrawBuff.LfoSw(frameBuffer, 16 + 1, 176, ref oldParam.lfoSw, newParam.lfoSw);
            DrawBuff.LfoFrq(frameBuffer, 64+1, 176, ref oldParam.lfoFrq, newParam.lfoFrq);
            DrawBuff.font4Hex12Bit(frameBuffer, 1 + 29 * 4, 44 * 4, 0, ref oldParam.timerA, newParam.timerA);
            DrawBuff.font4HexByte(frameBuffer, 1 + 43 * 4, 44 * 4, 0, ref oldParam.timerB, newParam.timerB);
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 10 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    if (ch < 6) parent.SetChannelMask(EnmChip.YM2612, chipID, ch);
                    else parent.SetChannelMask(EnmChip.YM2612, chipID, 2);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 6; ch++) parent.ResetChannelMask(EnmChip.YM2612, chipID, ch);
                return;
            }

            //音色で右クリックした場合は何もしない
            if (e.Button == MouseButtons.Right) return;

            // 音色表示欄の判定
            int h = (py - 10 * 8) / (6 * 8);
            int w = Math.Min(px / (29 * 4), 2);
            int instCh = h * 3 + w;

            if (instCh < 6)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(EnmChip.YM2612, instCh, chipID);
            }

        }
    }
}
