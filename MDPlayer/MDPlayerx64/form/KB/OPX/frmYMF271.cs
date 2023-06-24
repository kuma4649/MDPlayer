using MDPlayerx64;
#if X64
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmYMF271 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YMF271 newParam = null;
        private MDChipParams.YMF271 oldParam = new MDChipParams.YMF271();
        private FrameBuffer frameBuffer = new FrameBuffer();

        private static int[] slotTbl = new int[]
        {
            0,24,12,36,
            1,25,13,37,
            2,26,14,38,
            3,27,15,39,

            4,28,16,40,
            5,29,17,41,
            6,30,18,42,
            7,31,19,43,

            8,32,20,44,
            9,33,21,45,
            10,34,22,46,
            11,35,23,47,
        };

        public frmYMF271(frmMain frm, int chipID, int zoom, MDChipParams.YMF271 newParam, MDChipParams.YMF271 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeYMF271"], null, zoom);
            screenInitYMF271(frameBuffer);
            update();
        }

        private void frmYMF271_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYMF271[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYMF271[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmYMF271_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
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

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYMF271"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYMF271"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYMF271"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYMF271"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYMF271"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYMF271"].Height * zoom);
            frmYMF271_Resize(null, null);

        }

        private void frmYMF271_Resize(object sender, EventArgs e)
        {
        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {

        }

        public void screenInitYMF271(FrameBuffer screen)
        {
            for (int ch = 0; ch < 48; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(screen, 49 + kx, ch * 8 + 8, kt, 0);
                }
                DrawBuff.drawFont8(screen, 313, ch * 8 + 8, 1, "   ");
                DrawBuff.drawPanType2P(screen, 24, ch * 8 + 8, 0, 0);

                oldParam.channels[ch].tn = -1;
            }
        }

        public void screenChangeParams()
        {
            MDSound.ymf271.YMF271Chip reg = Audio.GetYMF271Register(chipID);
            if (reg != null)
            {
                for (int i = 0; i < 48; i++)
                {
                    int slot = slotTbl[i];

                    MDChipParams.Channel nrc = newParam.channels[slot];
                    MDSound.ymf271.YMF271Slot slt = reg.slots[slot];
                    nrc.volumeL = Math.Min(Math.Max((slt.volume * slt.ch0_level) >> 23, 0), 19);
                    nrc.volumeR = Math.Min(Math.Max((slt.volume * slt.ch1_level) >> 23, 0), 19);
                    nrc.pan = (slt.ch1_level << 4) | (slt.ch0_level & 0xf);
                    nrc.pantp = (slt.ch3_level & 0xf0) | ((slt.ch2_level >> 4) & 0xf);
                    nrc.inst[0] = slt.ar;//AR (&0x1f)
                    nrc.inst[1] = slt.decay1rate;//DR (&0x1f)
                    nrc.inst[2] = slt.decay2rate;//SR (&0x1f)
                    nrc.inst[3] = slt.relrate;//RR(&0xf)
                    nrc.inst[4] = slt.decay1lvl;//SL(&0xf)
                    nrc.inst[5] = slt.tl;//TL(&0x7f)
                    nrc.inst[6] = slt.keyscale;//KS(&0x7)
                    nrc.inst[7] = slt.multiple;//ML(&0xf)
                    nrc.inst[8] = slt.detune;//DT(&0x7)
                    nrc.inst[9] = slt.waveform;//waveform(&0x7)
                    nrc.inst[10] = slt.feedback;//feedback(&0x7)
                    nrc.inst[11] = slt.accon;//accon(&0x80)
                    nrc.inst[12] = slt.algorithm;//algorithm(&0x0f)

                    nrc.inst[13] = slt.block;//block(&0x0f)
                    nrc.inst[14] = (int)slt.fns;//fns(&0x0fff)

                    nrc.inst[15] = (int)slt.startaddr;//(&0xffffff)
                    nrc.inst[16] = (int)slt.endaddr;//(&0xffffff)
                    nrc.inst[17] = (int)slt.loopaddr;//(&0xffffff)

                    nrc.inst[18] = slt.fs;//(&0x3)
                    nrc.inst[19] = slt.bits == 12 ? 1 : 0;//(&0x4)
                    nrc.inst[20] = slt.srcnote;//(&0x3)
                    nrc.inst[21] = slt.srcb;//(&0x7)

                    nrc.inst[22] = slt.lfoFreq;//(&0xff)
                    nrc.inst[23] = slt.lfowave;//(&0x3)
                    nrc.inst[24] = slt.pms;//(&0x7)
                    nrc.inst[25] = slt.ams;//(&0x3)

                    //note
                    if (slt.active != 0)
                    {// nrc.volumeL != 0 || nrc.volumeR != 0)
                     //nrc.note = (int)(1200 * (((nrc.inst[13]+8)&0xf)-5) + 1200 * Math.Log((4096 + (nrc.inst[14] & 0xfff)) / 4096.0)) / 100;
                        nrc.volumeL = Math.Min(Math.Max((slt.volume * slt.ch0_level) >> 23, 0), 19);
                        nrc.volumeR = Math.Min(Math.Max((slt.volume * slt.ch1_level) >> 23, 0), 19);
                        nrc.note = Common.searchSSGNote(nrc.inst[14]) + (((nrc.inst[13] + 8) & 0xf) - 11) * 12 - 7;
                    }
                    else
                    {
                        nrc.volumeL += nrc.volumeL > 0 ? -1 : 0;
                        nrc.volumeR += nrc.volumeR > 0 ? -1 : 0;
                        nrc.note = -1;
                    }

                    if (i % 4 == 0)
                    {
                        nrc.tn = reg.groups[i / 4].sync;
                    }
                }
            }
        }

        public void screenDrawParams()
        {
            for (int i = 0; i < 48; i++)
            {
                int slot = slotTbl[i];

                MDChipParams.Channel orc = oldParam.channels[slot];
                MDChipParams.Channel nrc = newParam.channels[slot];

                DrawBuff.Volume(frameBuffer, 273, 8 + i * 8, 1, ref orc.volumeL, nrc.volumeL, 0);
                DrawBuff.Volume(frameBuffer, 273, 12 + i * 8, 1, ref orc.volumeR, nrc.volumeR, 0);
                DrawBuff.font4Int2(frameBuffer, 25, 8 + i * 8, 0, 2, ref orc.echo, slot + 1);//slotnum
                DrawBuff.PanType2(frameBuffer, 33, 8 + i * 8, ref orc.pan, nrc.pan, 0);
                DrawBuff.PanType2(frameBuffer, 41, 8 + i * 8, ref orc.pantp, nrc.pantp, 0);

                DrawBuff.KeyBoardXY(frameBuffer, 49, 8 + i * 8, ref orc.note, nrc.note, 0);

                DrawBuff.font4Int2(frameBuffer, 357, 8 + i * 8, 0, 2, ref orc.inst[0], nrc.inst[0]);//AR
                DrawBuff.font4Int2(frameBuffer, 365, 8 + i * 8, 0, 2, ref orc.inst[1], nrc.inst[1]);//DR
                DrawBuff.font4Int2(frameBuffer, 373, 8 + i * 8, 0, 2, ref orc.inst[2], nrc.inst[2]);//SR
                DrawBuff.font4Int2(frameBuffer, 381, 8 + i * 8, 0, 2, ref orc.inst[3], nrc.inst[3]);//RR
                DrawBuff.font4Int2(frameBuffer, 389, 8 + i * 8, 0, 2, ref orc.inst[4], nrc.inst[4]);//SL
                DrawBuff.font4Int3(frameBuffer, 397, 8 + i * 8, 0, 3, ref orc.inst[5], nrc.inst[5]);//TL
                DrawBuff.font4Int1(frameBuffer, 413, 8 + i * 8, 0, ref orc.inst[6], nrc.inst[6]);//KS
                DrawBuff.font4Int2(frameBuffer, 417, 8 + i * 8, 0, 2, ref orc.inst[7], nrc.inst[7]);//ML
                DrawBuff.font4Int1(frameBuffer, 429, 8 + i * 8, 0, ref orc.inst[8], nrc.inst[8]);//DT
                DrawBuff.font4Int1(frameBuffer, 437, 8 + i * 8, 0, ref orc.inst[9], nrc.inst[9]);//WF
                DrawBuff.font4Int1(frameBuffer, 445, 8 + i * 8, 0, ref orc.inst[10], nrc.inst[10]);//FB
                DrawBuff.font4Int1(frameBuffer, 449, 8 + i * 8, 0, ref orc.inst[11], nrc.inst[11]);//accon
                DrawBuff.font4Int2(frameBuffer, 453, 8 + i * 8, 0, 2, ref orc.inst[12], nrc.inst[12]);//algorithm
                DrawBuff.font4Int2(frameBuffer, 465, 8 + i * 8, 0, 2, ref orc.inst[13], nrc.inst[13]);//algorithm
                DrawBuff.font4Hex12Bit(frameBuffer, 477, 8 + i * 8, 0, ref orc.inst[14], nrc.inst[14]);//fns
                DrawBuff.font4Hex24Bit(frameBuffer, 497, 8 + i * 8, 0, ref orc.inst[15], nrc.inst[15]);//startaddr
                DrawBuff.font4Hex24Bit(frameBuffer, 525, 8 + i * 8, 0, ref orc.inst[16], nrc.inst[16]);//endaddr
                DrawBuff.font4Hex24Bit(frameBuffer, 553, 8 + i * 8, 0, ref orc.inst[17], nrc.inst[17]);//loopaddr
                DrawBuff.font4Int1(frameBuffer, 581, 8 + i * 8, 0, ref orc.inst[18], nrc.inst[18]);//fs
                DrawBuff.font4Int1(frameBuffer, 585, 8 + i * 8, 0, ref orc.inst[19], nrc.inst[19]);//bits
                DrawBuff.font4Int1(frameBuffer, 589, 8 + i * 8, 0, ref orc.inst[20], nrc.inst[20]);//srcnote
                DrawBuff.font4Int1(frameBuffer, 593, 8 + i * 8, 0, ref orc.inst[21], nrc.inst[21]);//srcb

                DrawBuff.font4Int3(frameBuffer, 601, 8 + i * 8, 0, 3, ref orc.inst[22], nrc.inst[22]);//lfofreq
                DrawBuff.font4Int1(frameBuffer, 617, 8 + i * 8, 0, ref orc.inst[23], nrc.inst[23]);//lfowave
                DrawBuff.font4Int1(frameBuffer, 621, 8 + i * 8, 0, ref orc.inst[24], nrc.inst[24]);//pms
                DrawBuff.font4Int1(frameBuffer, 625, 8 + i * 8, 0, ref orc.inst[25], nrc.inst[25]);//ams

                if (i % 4 == 0)
                {
                    DrawBuff.OpxOP(frameBuffer, 17, 8 + i * 8, 0, ref orc.tn, nrc.tn & 3);//sync
                }
            }
        }

    }
}
