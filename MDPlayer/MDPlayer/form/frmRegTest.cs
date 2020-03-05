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
    public partial class frmRegTest : Form {
        class ChipData {
            public delegate int[] GetRegisterDelegate(int Select);
            public string ChipName;
            public int BaseIndex;
            public GetRegisterDelegate Register;

            public ChipData(string chipName, int baseIndex, GetRegisterDelegate register) {
                ChipName = chipName;
                BaseIndex = baseIndex;
                Register = register;
            }
        }

        class RegisterManager {
            int Select;

            List<ChipData> ChipList = new List<ChipData>();

            public RegisterManager() {
                AddChip("YMF278B", 3, (Select) => {
                    return Audio.GetYMF278BRegister(0)[Select];
                });

                AddChip("YMF262", 2, (Select) => {
                    return Audio.GetYMF262Register(0)[Select];
                });

                AddChip("YM2151", 1, (Select) => {
                    return Audio.GetYM2151Register(0);
                });

                AddChip("YM2610", 2, (Select) => {
                    return Audio.GetYM2610Register(0)[Select];
                });

                AddChip("YM2608", 2, (Select) => {
                    return Audio.GetYM2608Register(0)[Select];
                });
            }

            private void AddChip(string ChipName, int Max, ChipData.GetRegisterDelegate p) {
                var BaseIndex = ChipList.Count; 
                for(var i=0; i < Max; i++) {
                    ChipList.Add(new ChipData(ChipName, BaseIndex, p));
                }
            }

            public void Prev() {
                if (Select > 0) Select--;
            }

            public void Next() {
                if (Select < ChipList.Count-1) Select++;
            }

            public int[] GetData() {
                var x = ChipList[Select];
                return x.Register(Select - x.BaseIndex);
            }

            public string GetName() {
                var x = ChipList[Select];
                return $"{x.ChipName, -10} #{Select - x.BaseIndex} REGISTER ({Select+1}/{ChipList.Count})  ";
            }

        }

        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private int FormWidth;
        private int FormHeight;

        private FrameBuffer frameBuffer = new FrameBuffer();

        RegisterManager RegMan = new RegisterManager();

        public frmRegTest(frmMain frm, int chipID, int zoom)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;

            FormWidth = 260;
            FormHeight = 140;

            InitializeComponent();
            frameBuffer.Add(pbScreen, new Bitmap(FormWidth, FormHeight), null, zoom);
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

        private void frmRegTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            isClosed = true;
        }

        private void frmRegTest_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + FormWidth * zoom, frameSizeH + FormHeight * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + FormWidth * zoom, frameSizeH + FormHeight * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + FormWidth * zoom, frameSizeH + FormHeight * zoom);
            fmrRegTest_Resize(null, null);
        }

        private void fmrRegTest_Resize(object sender, EventArgs e)
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
        }

        public void screenDrawParams()
        {
            var Name = RegMan.GetName();
            var Reg = RegMan.GetData();

            DrawBuff.drawFont4(frameBuffer, 2, 0, 0, Name);
            DrawBuff.drawFont4(frameBuffer, 210, 0, 0, $"< >");

            var y = 8;
            for(var idx = 0; idx < 0x100;) {
                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"{idx:X3}:");
                for(var i=0; i < 0x10; i++) {
                    DrawBuff.drawFont4(frameBuffer, 34 + (i * 12), y, 0, $"{Reg[idx]:X2}");
                    idx++;
                }
                y += 8;
            }
        }


        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            if (py > 8) return;
            if (px < 210) return;
            int xc = (px-210) / 4;
            if (xc == 0) {
                RegMan.Prev();
            }

            if (xc == 2) {
                RegMan.Next();
            }
        }


        public void screenInit()
        {
        }

    }
}
