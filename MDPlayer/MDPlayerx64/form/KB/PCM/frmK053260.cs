#if X64
using MDPlayerx64;
using MDPlayerx64.Properties;
using MDSound;
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
    public partial class frmK053260 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;

        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;
        private MDChipParams.K053260 newParam = null;
        private MDChipParams.K053260 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();


        public frmK053260(frmMain frm, int chipID, int zoom, MDChipParams.K053260 newParam, MDChipParams.K053260 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;

            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, ResMng.imgDic["planeK053260"], null, zoom);
            screenInit();
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

        private void frmK053260_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosK053260[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosK053260[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        private void frmK053260_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeK053260"].Width * zoom, frameSizeH + ResMng.imgDic["planeK053260"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeK053260"].Width * zoom, frameSizeH + ResMng.imgDic["planeK053260"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.imgDic["planeK053260"].Width * zoom, frameSizeH + ResMng.imgDic["planeK053260"].Height * zoom);
            frmK053260_Resize(null, null);

        }

        private void frmK053260_Resize(object sender, EventArgs e)
        {

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            int ch;
            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (ch = 0; ch < 4; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.K053260, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.K053260, chipID, ch);
                    }
                }
                return;
            }

            ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch < 4)
            {
                if (e.Button == MouseButtons.Left)
                {
                    parent.SetChannelMask(EnmChip.K053260, chipID, ch);
                    return;
                }

                for (ch = 0; ch < 4; ch++) parent.ResetChannelMask(EnmChip.K053260, chipID, ch);
                return;

            }
        }

        public void screenInit()
        {
            for (int ch = 0; ch < 4; ch++)
            {
                for (int ot = 0; ot < 12 * 8; ot++)
                {
                    int kx = Tables.kbl[(ot % 12) * 2] + ot / 12 * 28;
                    int kt = Tables.kbl[(ot % 12) * 2 + 1];
                    DrawBuff.drawKbn(frameBuffer, 33 + kx, ch * 8 + 8, kt, 0);
                }
                DrawBuff.PanType5(frameBuffer, 4 * 6 + 1, ch * 8 + 8, ref oldParam.channels[ch].panL, 0, 0);
                DrawBuff.PanType5(frameBuffer, 4 * 7 + 1, ch * 8 + 8, ref oldParam.channels[ch].panR, 0, 0);
            }
        }

        public void screenChangeParams()
        {
            K053260.k053260_state regs = Audio.GetK053260Register(chipID);
            if (regs == null) return;

            for (int ch = 0; ch < 4; ch++)
            {
                newParam.channels[ch].freq = (int)regs.channels[ch].rate;
                newParam.channels[ch].eadr = (int)regs.channels[ch].size;
                newParam.channels[ch].sadr = (int)regs.channels[ch].start;
                newParam.channels[ch].bank = (int)regs.channels[ch].bank;
                newParam.channels[ch].pan = (int)regs.channels[ch].pan;
                newParam.channels[ch].panL = (int)(8-regs.channels[ch].pan);
                newParam.channels[ch].panR = (int)regs.channels[ch].pan;
                newParam.channels[ch].volume = (int)regs.channels[ch].volume / 2;
                newParam.channels[ch].bit[0] = (regs.channels[ch].play != 0);
                newParam.channels[ch].bit[1] = (regs.channels[ch].dir != 0);
                newParam.channels[ch].bit[2] = (regs.channels[ch].loop != 0);
                newParam.channels[ch].bit[3] = (regs.channels[ch].ppcm != 0);

                if (newParam.channels[ch].bit[0])
                {
                    newParam.channels[ch].volumeL = (int)regs.channels[ch].volume * newParam.channels[ch].panL / 8 / 5 / 2;
                    newParam.channels[ch].volumeL = Math.Min(newParam.channels[ch].volumeL, 19);
                    newParam.channels[ch].volumeR = (int)regs.channels[ch].volume * newParam.channels[ch].panR / 8 / 5 / 2;
                    newParam.channels[ch].volumeR = Math.Min(newParam.channels[ch].volumeR, 19);
                }
                else
                {
                    if (newParam.channels[ch].volumeL > 0) newParam.channels[ch].volumeL--;
                    if (newParam.channels[ch].volumeR > 0) newParam.channels[ch].volumeR--;
                }
                newParam.channels[ch].panL /= 2;
                newParam.channels[ch].panR /= 2;

                uint delta = regs.delta_table[newParam.channels[ch].freq];
                //if (newParam.channels[ch].bit[3]) delta /= 2;
                newParam.channels[ch].note = !newParam.channels[ch].bit[0] ? -1 : searchNote(delta);
            }
        }

        private int searchNote(uint freq)
        {
            double m = double.MaxValue;

            int clock = Audio.clockK053260;

            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                int a = (int)(
                    0x10000 //1sample進むのに必要なカウント数
                    * 8000.0
                    * Tables.pcmMulTbl[i % 12 + 12]
                    * Math.Pow(2, (i / 12 - 3 + 2))
                    / clock
                    *6
                    *2
                    );

                if (freq > a)
                {
                    m = a;
                    n = i;
                }
            }
            return Math.Min(Math.Max(n - 2, 0), 95);
        }

        public void screenDrawParams()
        {
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int ch = 0; ch < 4; ch++)
            {
                oyc = oldParam.channels[ch];
                nyc = newParam.channels[ch];

                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 69 + 1, ch * 8 + 8, 0, ref oyc.freq, nyc.freq);
                DrawBuff.font4HexByte(frameBuffer, 4 * 74 + 1, ch * 8 + 8, 0, ref oyc.bank, nyc.bank);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 77 + 1, ch * 8 + 8, 0, ref oyc.sadr, nyc.sadr);
                DrawBuff.font4Hex16Bit(frameBuffer, 4 * 82 + 1, ch * 8 + 8, 0, ref oyc.eadr, nyc.eadr);
                DrawBuff.font4HexByte(frameBuffer, 4 * 87 + 1, ch * 8 + 8, 0, ref oyc.pan, nyc.pan);
                DrawBuff.font4HexByte(frameBuffer, 4 * 90 + 1, ch * 8 + 8, 0, ref oyc.volume, nyc.volume);

                for (int b = 0; b < 4; b++)
                {
                    DrawBuff.drawNESSw(frameBuffer, 64 * 4 + b * 4+1, ch * 8 + 8
                        , ref oldParam.channels[ch].bit[b], newParam.channels[ch].bit[b]);
                }

                DrawBuff.PanType5(frameBuffer, 4 * 6 + 1, ch * 8 + 8, ref oyc.panL, nyc.panL, 0);
                DrawBuff.PanType5(frameBuffer, 4 * 7 + 1, ch * 8 + 8, ref oyc.panR, nyc.panR, 0);
                DrawBuff.VolumeXY1(frameBuffer, 4*92+1, ch * 8 + 8, 1, ref oyc.volumeL, nyc.volumeL, 0);
                DrawBuff.VolumeXY1(frameBuffer, 4*92+1, ch * 8 + 12, 1, ref oyc.volumeR, nyc.volumeR, 0);

                DrawBuff.KeyBoardXYFX(frameBuffer, 4 * 8 + 1, 4 * 103 + 1, ch * 8 + 8, ref oyc.note, nyc.note, 0);
                DrawBuff.ChK053260(frameBuffer, ch, ref oyc.mask, nyc.mask, 0);
            }
        }

    }
}
