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
    public partial class frmVRC7 : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.VRC7 newParam = null;
        private MDChipParams.VRC7 oldParam = new MDChipParams.VRC7();
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmVRC7(frmMain frm, int chipID, int zoom, MDChipParams.VRC7 newParam)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeVRC7, null, zoom);
            bool VRC7Type = false;
            int tp = VRC7Type ? 1 : 0;
            DrawBuff.screenInitVRC7(frameBuffer, tp);
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

        private void frmVRC7_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmVRC7_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.setting.location.PosVrc7[chipID] = Location;
            isClosed = true;
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeVRC7.Width * zoom, frameSizeH + Properties.Resources.planeVRC7.Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + Properties.Resources.planeVRC7.Width * zoom, frameSizeH + Properties.Resources.planeVRC7.Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + Properties.Resources.planeVRC7.Width * zoom, frameSizeH + Properties.Resources.planeVRC7.Height * zoom);
            frmVRC7_Resize(null, null);

        }

        private void frmVRC7_Resize(object sender, EventArgs e)
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

        public void screenChangeParams()
        {
            byte[] vrc7Register = Audio.GetVRC7Register(chipID);
            if (vrc7Register == null) return;

            //キーオン(ワンショット)があったかを取得する
            ChipKeyInfo ki = Audio.getVRC7KeyInfo(chipID);

            for (int ch = 0; ch < 6; ch++)
            {
                MDChipParams.Channel nyc = newParam.channels[ch];

                //音色番号
                nyc.inst[0] = (vrc7Register[0x30 + ch] & 0xf0) >> 4;
                //サスティンの取得
                nyc.inst[1] = (vrc7Register[0x20 + ch] & 0x20) >> 5;
                //現在のキーオン状態
                nyc.inst[2] = (vrc7Register[0x20 + ch] & 0x10) >> 4;
                //ボリューム
                nyc.inst[3] = (vrc7Register[0x30 + ch] & 0x0f);

                //再生周波数
                int freq = vrc7Register[0x10 + ch] + ((vrc7Register[0x20 + ch] & 0x1) << 8);
                //オクターブ
                int oct = ((vrc7Register[0x20 + ch] & 0xe) >> 1);
                //周波数とオクターブ情報から近似する音程を取得する
                nyc.note = common.searchSegaPCMNote(freq / 172.0) + (oct - 4) * 12;


                //ワンショット(前回の処理から比較してキーオンが1度以上発生している状態)の場合
                if (ki.On[ch])
                {
                    //ボリュームメーターを振る
                    nyc.volumeL = (19 - nyc.inst[3]);
                }
                else
                {
                    //ワンショットが無く、現在もキーオンしていない場合は音程を無しにする。
                    //ワンショットが無くても、キーオン状態ならば音程をリセットしない。
                    //(持続している場合やベンドやスラーをしていることが考えられる為。)
                    //また、この処理はワンショットが発生しているときは実施しない。
                    //ワンショットが有り、現在はキーオンしていない場合に対応するため。
                    //上記ケースは、ボリュームメータを振り、音程表示は一瞬だけ表示する動きになる
                    if (nyc.inst[2] == 0) nyc.note = -1;

                    //ボリュームメータの減衰処理(音色設定を無視し常に一定)
                    nyc.volumeL--; if (nyc.volumeL < 0) nyc.volumeL = 0;
                }
            }


            newParam.channels[0].inst[4] = (vrc7Register[0x02] & 0x3f);//TL
            newParam.channels[0].inst[5] = (vrc7Register[0x03] & 0x07);//FB

            newParam.channels[0].inst[6] = (vrc7Register[0x04] & 0xf0) >> 4;//AR
            newParam.channels[0].inst[7] = (vrc7Register[0x04] & 0x0f);//DR
            newParam.channels[0].inst[8] = (vrc7Register[0x06] & 0xf0) >> 4;//SL
            newParam.channels[0].inst[9] = (vrc7Register[0x06] & 0x0f);//RR
            newParam.channels[0].inst[10] = (vrc7Register[0x02] & 0x80) >> 7;//KL
            newParam.channels[0].inst[11] = (vrc7Register[0x00] & 0x0f);//MT
            newParam.channels[0].inst[12] = (vrc7Register[0x00] & 0x80) >> 7;//AM
            newParam.channels[0].inst[13] = (vrc7Register[0x00] & 0x40) >> 6;//VB
            newParam.channels[0].inst[14] = (vrc7Register[0x00] & 0x20) >> 5;//EG
            newParam.channels[0].inst[15] = (vrc7Register[0x00] & 0x10) >> 4;//KR
            newParam.channels[0].inst[16] = (vrc7Register[0x03] & 0x08) >> 3;//DM
            newParam.channels[0].inst[17] = (vrc7Register[0x05] & 0xf0) >> 4;//AR
            newParam.channels[0].inst[18] = (vrc7Register[0x05] & 0x0f);//DR
            newParam.channels[0].inst[19] = (vrc7Register[0x07] & 0xf0) >> 4;//SL
            newParam.channels[0].inst[20] = (vrc7Register[0x07] & 0x0f);//RR
            newParam.channels[0].inst[21] = (vrc7Register[0x03] & 0x80) >> 7;//KL
            newParam.channels[0].inst[22] = (vrc7Register[0x01] & 0x0f);//MT
            newParam.channels[0].inst[23] = (vrc7Register[0x01] & 0x80) >> 7;//AM
            newParam.channels[0].inst[24] = (vrc7Register[0x01] & 0x40) >> 6;//VB
            newParam.channels[0].inst[25] = (vrc7Register[0x01] & 0x20) >> 5;//EG
            newParam.channels[0].inst[26] = (vrc7Register[0x01] & 0x10) >> 4;//KR
            newParam.channels[0].inst[27] = (vrc7Register[0x03] & 0x10) >> 4;//DC

        }


        public void screenDrawParams()
        {
            int tp = 0;

            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            for (int c = 0; c < 6; c++)
            {

                oyc = oldParam.channels[c];
                nyc = newParam.channels[c];

                DrawBuff.Volume(frameBuffer, c, 0, ref oyc.volumeL, nyc.volumeL, tp);
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);

                DrawBuff.drawInstNumber(frameBuffer, (c % 3) * 16 + 37, (c / 3) * 2 + 16, ref oyc.inst[0], nyc.inst[0]);
                DrawBuff.SUSFlag(frameBuffer, (c % 3) * 16 + 41, (c / 3) * 2 + 16, 0, ref oyc.inst[1], nyc.inst[1]);
                DrawBuff.SUSFlag(frameBuffer, (c % 3) * 16 + 44, (c / 3) * 2 + 16, 0, ref oyc.inst[2], nyc.inst[2]);
                DrawBuff.drawInstNumber(frameBuffer, (c % 3) * 16 + 46, (c / 3) * 2 + 16, ref oyc.inst[3], nyc.inst[3]);

                DrawBuff.ChYM2413(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            oyc = oldParam.channels[0];
            nyc = newParam.channels[0];
            DrawBuff.drawInstNumber(frameBuffer, 9, 14, ref oyc.inst[4], nyc.inst[4]); //TL
            DrawBuff.drawInstNumber(frameBuffer, 14, 14, ref oyc.inst[5], nyc.inst[5]); //FB

            for (int c = 0; c < 11; c++)
            {
                DrawBuff.drawInstNumber(frameBuffer, c * 3, 18, ref oyc.inst[6 + c], nyc.inst[6 + c]);
                DrawBuff.drawInstNumber(frameBuffer, c * 3, 20, ref oyc.inst[17 + c], nyc.inst[17 + c]);
            }
        }

        public void screenInit()
        {
            for (int ch = 0; ch < 6; ch++)
            {
                newParam.channels[ch].inst[0] = 0;
                newParam.channels[ch].inst[1] = 0;
                newParam.channels[ch].inst[2] = 0;
                newParam.channels[ch].inst[3] = 0;
                newParam.channels[ch].note = -1;
                newParam.channels[ch].volumeL = 0;
                newParam.channels[ch].mask = false;
            }

            newParam.channels[0].inst[4] = 0;
            newParam.channels[0].inst[5] = 0;
            newParam.channels[0].inst[6] = 0;
            newParam.channels[0].inst[7] = 0;
            newParam.channels[0].inst[8] = 0;
            newParam.channels[0].inst[9] = 0;
            newParam.channels[0].inst[10] = 0;
            newParam.channels[0].inst[11] = 0;
            newParam.channels[0].inst[12] = 0;
            newParam.channels[0].inst[13] = 0;
            newParam.channels[0].inst[14] = 0;
            newParam.channels[0].inst[15] = 0;
            newParam.channels[0].inst[16] = 0;
            newParam.channels[0].inst[17] = 0;
            newParam.channels[0].inst[18] = 0;
            newParam.channels[0].inst[19] = 0;
            newParam.channels[0].inst[20] = 0;
            newParam.channels[0].inst[21] = 0;
            newParam.channels[0].inst[22] = 0;
            newParam.channels[0].inst[23] = 0;
            newParam.channels[0].inst[24] = 0;
            newParam.channels[0].inst[25] = 0;
            newParam.channels[0].inst[26] = 0;
            newParam.channels[0].inst[27] = 0;

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int py = e.Location.Y / zoom;
            int px = e.Location.X / zoom;

            //上部のラベル行の場合は何もしない
            if (py < 1 * 8) return;

            //鍵盤
            if (py < 7 * 8)
            {
                int ch = (py / 8) - 1;
                if (ch < 0) return;

                if (ch == 9)
                {
                    int x = (px / 4 - 4);
                    if (x < 0) return;
                    x /= 15;
                    if (x > 4) return;
                    ch += x;
                }

                if (e.Button == MouseButtons.Left)
                {
                    //マスク
                    parent.SetChannelMask(enmUseChip.VRC7, chipID, ch);
                    return;
                }

                //マスク解除
                for (ch = 0; ch < 6; ch++) parent.ResetChannelMask(enmUseChip.VRC7, chipID, ch);
                return;
            }

            //音色欄
            if (py < 15 * 8 && px < 16 * 8)
            {
                //クリップボードに音色をコピーする
                parent.getInstCh(enmUseChip.VRC7, 0, chipID);
            }
        }
    }
}
