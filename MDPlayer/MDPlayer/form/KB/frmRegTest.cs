using MDSound;
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
    public partial class frmRegTest : frmChipBase
    {
        class ChipData {

            public delegate object GetRegisterDelegate(int Select);

            public string ChipName;
            public int BaseIndex;
            public GetRegisterDelegate Register;
            public int MaxRegisterSize;
            public int regWind;
            public ChipData(string chipName, int baseIndex, int maxRegisterSize, int regWindow, GetRegisterDelegate register) {
                ChipName = chipName;
                BaseIndex = baseIndex;
                Register = register;
                MaxRegisterSize = maxRegisterSize;
                regWind = regWindow;
            }
        }

        class RegisterManager {
            int Select;
            public bool needRefresh = false;
             List<ChipData> ChipList = new List<ChipData>();

            public RegisterManager() {
                AddChip("YMF278B", 3, 0x100, (Select) => { // 0
                    return Audio.GetYMF278BRegister(0)[Select];
                });

                AddChip("YMF262", 2, 0x100, (Select) => { // 3
                    return Audio.GetYMF262Register(0)[Select];
                });

                AddChip("YM2151", 1, 0x100, (Select) => { // 5
                    return Audio.GetYM2151Register(0);
                });

                AddChip("YM2610", 1, 0x200, (Select) => { // 6 
                    return Audio.GetYM2610Register(0);
                });

                AddChip("YM2608", 1, 0x200, (Select) => { // 7
                    return Audio.GetYM2608Register(0);
                });

                AddChip("YM2612", 1, 0x200, (Select) => {
                    return Audio.GetFMRegister(0);
                });

                AddChip("C140", 1, 0x200, (Select) => {
                    return Audio.GetC140Register(0);
                });

                AddChip("QSOUND", 1, 0x200, (Select) => {
                    return Audio.GetQSoundRegister(0);
                });

                AddChip("SEGAPCM", 1, 0x200, (Select) => {
                    return Audio.GetSEGAPCMRegister(0);
                });

                AddChip("YMZ280B", 1, 0x100, (Select) => {
                    return Audio.GetYMZ280BRegister(0);
                });

                AddChip("SN76489", 1, 8, (Select) => {
                    return Audio.GetPSGRegister(0);
                });

                AddChip("AY", 1, 16, (Select) => {
                    return Audio.GetAY8910Register(0);
                });

                AddChip("C352", 1, 0x400, (Select) => {
                    return Audio.GetC352Register(0);
                });

                AddChip("YM2203", 1, 0x200, (Select) => {
                    return Audio.GetYM2203Register(0);
                });

                AddChip("YM2413", 1, 0x100, (Select) => {
                    return Audio.GetYM2413Register(0);
                });

                AddChip("YM3812", 1, 0x100, (Select) => {
                    return Audio.GetYM3812Register(0);
                });

                AddChip("NES", 1, 0x30, (Select) => {
                    return Audio.GetAPURegister(0);
                });

                AddChip("SID", 1, 0x19, (Select) => {
                    return Audio.GetSIDRegister(0);
                });
            }

            private void AddChip(string ChipName, int Max, int regSize, ChipData.GetRegisterDelegate p) {
                var BaseIndex = ChipList.Count; 
                for(var i=0; i < Max; i++) {
                    ChipList.Add(new ChipData(ChipName,BaseIndex, regSize, Max, p));
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

            public object GetData() {
                var x = ChipList[Select];
                return x.Register(Select - x.BaseIndex);
            }

            public string GetName()
            {
                var x = ChipList[Select];
                return $"{x.ChipName,-10}  ";
            }

            public string GetName2()
            {
                var x = ChipList[Select];
                return $"#{Select - x.BaseIndex} REGISTER ({Select + 1}/{ChipList.Count})  ";
            }

            public int getRegisterSize() {
                return ChipList[Select].MaxRegisterSize;
            }

            //public int getPageSize()
            //{
            //    return ChipList[Select].regWind;
            //}

            public int getCurrentPage()
            {
                var x = ChipList[Select];
                return Select - x.BaseIndex;
            }
            public int getSelect()
            {
                return Select;
            }

            public void setSelect(int val)
            {
                Select = val;
            }
        }

        public bool isClosed = false;
        //public int x = -1;
        //public int y = -1;
        public frmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private int FormWidth;
        private int FormHeight;

        private FrameBuffer frameBuffer = new FrameBuffer();

        RegisterManager RegMan = new RegisterManager();

        private readonly Dictionary<EnmChip, int> pageDict = new Dictionary<EnmChip, int>()
        {
            { EnmChip.YMF278B, 0 },
            { EnmChip.YMF262, 3 },
            { EnmChip.YM2151, 5 },
            { EnmChip.YM2610, 6 },
            { EnmChip.YM2608, 7 },
            { EnmChip.YM2612, 8 },
            { EnmChip.C140, 9 },
            { EnmChip.QSound, 10 },
            { EnmChip.SEGAPCM, 11 },
            { EnmChip.YMZ280B, 12 },
            { EnmChip.SN76489, 13 },
            { EnmChip.AY8910, 14 },
            { EnmChip.C352, 15 },
            { EnmChip.YM2203, 16 },
            { EnmChip.YM2413, 17 },
            { EnmChip.YM3812, 18 },
            { EnmChip.NES, 19 },
            { EnmChip.SID, 20 },
        };

        public frmRegTest(frmMain frm, int chipID,EnmChip enmPage, int zoom)
        {
            parent = frm;
            this.chipID = chipID;
            this.zoom = zoom;
            int pageSel = 0;
            pageDict.TryGetValue(enmPage, out pageSel);
            FormWidth = 260;
            FormHeight = 280;//140;

            InitializeComponent();
            frameBuffer.Add(pbScreen, new Bitmap(FormWidth, FormHeight), null, zoom);
            RegMan.setSelect(pageSel);
            update();
        }

        public void changeChip(EnmChip chip)
        {
            _ = pageDict.TryGetValue(chip, out int pageSel);
            RegMan.setSelect(pageSel);
            RegMan.needRefresh = true;
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
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosRegTest[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosRegTest[chipID] = RestoreBounds.Location;
            }
            parent.setting.location.ChipSelect = RegMan.getSelect();
            update();
            isClosed = true;
        }

        private void frmRegTest_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
            RegMan.setSelect(parent.setting.location.ChipSelect);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + FormWidth * zoom, frameSizeH + FormHeight * zoom);
            //this.MinimumSize = new System.Drawing.Size(frameSizeW + FormWidth * zoom, frameSizeH + FormHeight * zoom);
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
            var Name2 = RegMan.GetName2();
            //var MaxPage = RegMan.getPageSize();
            //var regSize = RegMan.getRegisterSize(); // Max
            //var actualRegSize = RegMan.getRegisterSize();//Reg.Length >= regSize ? regSize : Reg.Length; //TODO: Change this
            DrawBuff.drawFont8(frameBuffer, 2, 1, 0, Name);
            DrawBuff.drawFont8(frameBuffer, 2, 9, 0, Name2);
            DrawBuff.drawFont8(frameBuffer, 210, 1, 0, $"<>");

            var y = 17;
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
            var p = RegMan.getCurrentPage();
            var Reg = RegMan.GetData();

            if(Reg is byte[])
            {
                byte[] r = (byte[])Reg;
                for (var i = 0; i < r.Length; i++)
                {
                    if (i % 16 == 0)
                    {
                        y += 8;
                        DrawBuff.drawFont4(frameBuffer, 2, y - 8, 0, $"{i:X3}:");
                    }
                    byte v = r[i];
                    DrawBuff.drawFont4(frameBuffer, 34 + ((i % 16) * 12), y - 8, 0, $"{v:X2}");
                }
            }
            else if (Reg is int[])
            {
                int[] r = (int[])Reg;
                int ms = RegMan.getRegisterSize();
                for (var i = 0; i < Math.Min(r.Length,ms); i++)
                {
                    if (i % 16 == 0)
                    {
                        y += 8;
                        DrawBuff.drawFont4(frameBuffer, 2, y - 8, 0, $"{i:X3}:");
                    }
                    byte v = (byte)r[i];
                    DrawBuff.drawFont4(frameBuffer, 34 + ((i % 16) * 12), y - 8, 0, $"{v:X2}");
                }
            }
            else if (Reg is ushort[])
            {
                ushort[] r = (ushort[])Reg;
                for (var i = 0; i < r.Length; i++)
                {
                    if (i % 16 == 0)
                    {
                        y += 8;
                        DrawBuff.drawFont4(frameBuffer, 2, y - 8, 0, $"{i:X3}:");
                    }
                    ushort v = r[i];
                    DrawBuff.drawFont4(frameBuffer, 30 + ((i % 8) * 18), y - 8, 0, $"{v:X4}");
                }
            }
            else if (Reg is int[][])
            {
                int[][] r = (int[][])Reg;
                for (int j = 0; j < r.Length; j++)
                {
                    for (var i = 0; i < r[j].Length; i++)
                    {
                        if (i % 16 == 0)
                        {
                            y += 8;
                            int n = i + j * r[j].Length;
                            DrawBuff.drawFont4(frameBuffer, 2, y - 8, 0, $"{n:X3}:");
                        }
                        int m= r[j][i];
                        byte v = (byte)r[j][i];
                        int c = 0;
                        if (m < 0)
                        {
                            //不明値
                            v = 0;
                            c = 1;
                        }
                        DrawBuff.drawFont4(frameBuffer, 34 + ((i % 16) * 12), y - 8, c, $"{v:X2}");
                    }
                }
            }

        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            //Console.WriteLine("{0} {1}", px, py);
            if (py > 8) return;
            if (px < 210) return;
            int xc = (px-210) / 4;
            if (xc == 0) {
                RegMan.Prev();
            }

            if (xc == 2) {
                RegMan.Next();
            }*/
            if (e.Button == MouseButtons.Right) RegMan.Prev();
            else if(e.Button == MouseButtons.Left) RegMan.Next();
        }


        public void screenInit()
        {
        }
    }
}
