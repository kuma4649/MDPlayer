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
            public int MaxRegisterSize;
            public ChipData(string chipName, int baseIndex, int maxRegisterSize, GetRegisterDelegate register) {
                ChipName = chipName;
                BaseIndex = baseIndex;
                Register = register;
                MaxRegisterSize = maxRegisterSize;
            }
        }

        class RegisterManager {
            int Select;
            public bool needRefresh = false;
            List<ChipData> ChipList = new List<ChipData>();

            public RegisterManager() {
                AddChip("YMF278B", 3, 0x100, (Select) => {
                    return Audio.GetYMF278BRegister(0)[Select];
                });

                AddChip("YMF262", 2, 0x100, (Select) => {
                    return Audio.GetYMF262Register(0)[Select];
                });

                AddChip("YM2151", 1, 0x100, (Select) => {
                    return Audio.GetYM2151Register(0);
                });

                AddChip("YM2610", 2, 0x100, (Select) => {
                    return Audio.GetYM2610Register(0)[Select];
                });

                AddChip("YM2608", 2, 0x100, (Select) => {
                    return Audio.GetYM2608Register(0)[Select];
                });

                AddChip("YM2612 P", 2, 0x100, (Select) => {
                    return Audio.GetFMRegister(0)[Select];
                });

                AddChip("SN76489", 1, 8, (Select) => {
                    return Audio.GetPSGRegister(0);
                });
            }

            private void AddChip(string ChipName, int Max, int regSize, ChipData.GetRegisterDelegate p) {
                var BaseIndex = ChipList.Count; 
                for(var i=0; i < Max; i++) {
                    ChipList.Add(new ChipData(ChipName, BaseIndex, regSize, p));
                }
            }

            public void Prev() {
                Select--;
                if (Select < 0) Select = ChipList.Count - 1;
                needRefresh = true;
            }

            public void Next() {
                Select++;
                if (Select > ChipList.Count - 1) Select = 0;
                needRefresh = true;
                //if (Select < ChipList.Count-1) Select++;
            }

            public int[] GetData() {
                var x = ChipList[Select];
                return x.Register(Select - x.BaseIndex);
            }

            public string GetName() {
                var x = ChipList[Select];
                return $"{x.ChipName, -10} #{Select - x.BaseIndex} REGISTER ({Select+1}/{ChipList.Count})  ";
            }

            public int getRegisterSize() {
                return ChipList[Select].MaxRegisterSize;
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
            if (RegMan.needRefresh) { frameBuffer.clearScreen(); RegMan.needRefresh = false; }
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
            //if (RegMan.needRefresh) { frameBuffer.clearScreen(); RegMan.needRefresh = false; }
            var Name = RegMan.GetName();
            var Reg = RegMan.GetData();
            var regSize = RegMan.getRegisterSize();

            DrawBuff.drawFont4(frameBuffer, 2, 0, 0, Name);
            DrawBuff.drawFont4(frameBuffer, 210, 0, 0, $"< >");

            var y = 8;
            /*
            var y = 8; // 行 0x10毎に変わる・・・
            for(var idx = 0; idx < regSize; idx+=0x10) {
                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"{idx:X3}:");
                var remainingRegNum = regSize >= 0x10 ? 0x10 : regSize;
                for (var i = 0; i < remainingRegNum; i++)
                {
                    DrawBuff.drawFont4(frameBuffer, 34 + (i * 12), y, 0, $"{Reg[idx+i]:X2}");
                }
                y += 8;
            }*/
            for(var i = 0; i < regSize; i++)
            {
                if (i % 16 == 0) {
                    y += 8;
                    DrawBuff.drawFont4(frameBuffer, 2, y-8, 0, $"{i:X3}:"); 
                }
                byte v = (byte)Reg[i];
                DrawBuff.drawFont4(frameBuffer, 34 + ((i%16) * 12), y-8, 0, $"{v:X2}");
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
