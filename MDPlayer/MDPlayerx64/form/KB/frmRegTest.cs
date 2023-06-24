using Driver.libsidplayfp.sidplayfp;
using MDPlayer.Driver.SID;

namespace MDPlayer.form
{
    public partial class frmRegTest : frmChipBase
    {
        class ChipData
        {

            public delegate object GetRegisterDelegate(int Select);

            public string ChipName;
            public int BaseIndex;
            public GetRegisterDelegate Register;
            public int MaxRegisterSize;
            public int regWind;
            public ChipData(string chipName, int baseIndex, int maxRegisterSize, int regWindow, GetRegisterDelegate register)
            {
                ChipName = chipName;
                BaseIndex = baseIndex;
                Register = register;
                MaxRegisterSize = maxRegisterSize;
                regWind = regWindow;
            }
        }

        class RegisterManager
        {
            int Select;
            public bool needRefresh = false;
            List<ChipData> ChipList = new List<ChipData>();

            public RegisterManager()
            {
                // コメントはページ番号 (0-index)
                AddChip("YMF278B", 3, 0x100, (Select) =>
                { // 0
                    return Audio.GetYMF278BRegister(0)[Select];
                });

                AddChip("YMF262", 2, 0x100, (Select) =>
                { // 3
                    return Audio.GetYMF262Register(0)[Select];
                });

                AddChip("YM2151", 1, 0x100, (Select) =>
                { // 5
                    return Audio.GetYM2151Register(0);
                });

                AddChip("YM2610", 1, 0x200, (Select) =>
                { // 6 
                    return Audio.GetYM2610Register(0);
                });

                AddChip("YM2608", 1, 0x200, (Select) =>
                { // 7
                    return Audio.GetYM2608Register(0);
                });

                AddChip("YM2612", 1, 0x200, (Select) =>
                { // 8
                    return Audio.GetFMRegister(0);
                });

                AddChip("C140", 1, 0x200, (Select) =>
                { // 9
                    return Audio.GetC140Register(0);
                });

                AddChip("QSOUND", 1, 0x200, (Select) =>
                { // 10
                    return Audio.GetQSoundRegister(0);
                });

                AddChip("SEGAPCM", 1, 0x200, (Select) =>
                { // 11
                    return Audio.GetSEGAPCMRegister(0);
                });

                AddChip("YMZ280B", 1, 0x100, (Select) =>
                { // 12
                    return Audio.GetYMZ280BRegister(0);
                });

                AddChip("SN76489", 1, 8, (Select) =>
                { // 13
                    return Audio.GetPSGRegister(0);
                });

                AddChip("AY", 1, 16, (Select) =>
                { // 14
                    return Audio.GetAY8910Register(0);
                });

                AddChip("C352", 1, 0x400, (Select) =>
                { // 15
                    return Audio.GetC352Register(0);
                });

                AddChip("YM2203", 1, 0x200, (Select) =>
                { // 16
                    return Audio.GetYM2203Register(0);
                });

                AddChip("YM2413", 1, 0x100, (Select) =>
                { // 17
                    return Audio.GetYM2413Register(0);
                });

                AddChip("YM3812", 1, 0x100, (Select) =>
                { // 18
                    return Audio.GetYM3812Register(0);
                });

                AddChip("NES", 1, 0x30, (Select) =>
                { // 19
                    return Audio.GetAPURegister(0);
                });

                AddChip("SID", 3, 0x19, (Select) =>
                { // 20, 21, 22
                    return Audio.GetSIDRegister(Select);
                });
            }

            private void AddChip(string ChipName, int Max, int regSize, ChipData.GetRegisterDelegate p)
            {
                var BaseIndex = ChipList.Count;
                for (var i = 0; i < Max; i++)
                {
                    ChipList.Add(new ChipData(ChipName, BaseIndex, regSize, Max, p));
                }
            }

            public void Prev()
            {
                Select--;
                if (Select < 0) Select = ChipList.Count - 1;
                needRefresh = true;
            }

            public void Next()
            {
                Select++;
                if (Select > ChipList.Count - 1) Select = 0;
                needRefresh = true;
                //if (Select < ChipList.Count-1) Select++;
            }

            public object GetData()
            {
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

            public int getRegisterSize()
            {
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

        //public bool isClosed = false;
        ////public int x = -1;
        ////public int y = -1;
        //public frmMain parent = null;
        //private int frameSizeW = 0;
        //private int frameSizeH = 0;
        //private int chipID = 0;
        //private int zoom = 1;

        private int FormWidth;
        private int FormHeight;

        //private FrameBuffer frameBuffer = new FrameBuffer();

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

        public frmRegTest(frmMain frm, int chipID, EnmChip enmPage, int zoom)
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

        public new void update()
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
                parent.WindowsMessage(ref m);
            }

            try { base.WndProc(ref m); }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public new void screenChangeParams()
        {
        }

        public new void screenDrawParams()
        {
            //if (RegMan.needRefresh) { frameBuffer.clearScreen(); RegMan.needRefresh = false; }
            var Name = RegMan.GetName();
            var Name2 = RegMan.GetName2();
            var p = RegMan.getCurrentPage();
            //var MaxPage = RegMan.getPageSize();
            //var regSize = RegMan.getRegisterSize(); // Max
            //var actualRegSize = RegMan.getRegisterSize();//Reg.Length >= regSize ? regSize : Reg.Length; //TODO: Change this
            DrawBuff.drawFont8(frameBuffer, 2, 1, 0, Name);
            DrawBuff.drawFont8(frameBuffer, 2, 9, 0, Name2);
            DrawBuff.drawFont8(frameBuffer, 210, 1, 0, $"<>");

            var y = 17;

            if (RegMan.GetName().Contains("SID"))
            {
                //y += 8;
                sid CurSID = Audio.GetCurrentSIDContext();
                //sid CurSID = ChipRegister.SID;
                var a = RegMan.GetData();
                if (a == null) return;
                uint[] r = (uint[])a;

                // Voice Registers

                // Regards of TEST bit... it seems to reset OSC to 0 until it clears up.

                ushort voice1f = (ushort)(r[1] << 8 | r[0]); // D401-D400 Voice 1 Freq
                ushort pwdc1 = (ushort)(((r[3] & 0x0F) << 8) | r[2]); // D402-D403 Pulse Wave Duty Cycle
                ushort mode1 = (ushort)r[4]; // MODE Reg D404  NOISE PULSE SAWTOOTH TRIANGLE TEST RING3 SYNC3 GATE(KON)
                uint attack1 = r[5] >> 4; // D405.4-7 Attack
                uint decay1 = r[5] & 0x0F; // D405.0-3 Decay
                uint sustain1 = r[6] >> 4; // D406.4-7 Sustain
                uint release1 = r[6] & 0x0F; // D406.0-3 Release

                ushort voice2f = (ushort)(r[8] << 8 | r[7]); // D408-D407 Voice 2 Freq
                ushort pwdc2 = (ushort)(((r[0xA] & 0x0F) << 8) | r[9]); // D40A-D409 Pulse Wave Duty Cycle
                ushort mode2 = (ushort)r[0xB]; // D40B Control2 Reg  NOISE PULSE SAWTOOTH TRIANGLE TEST RING1 SYNC1 GATE(KON)
                uint attack2 = r[0xC] >> 4; // D40C.4-7 Attack
                uint decay2 = r[0xC] & 0x0F; // D40C.0-3 Decay
                uint sustain2 = r[0xD] >> 4; // D40D.4-7 Sustain
                uint release2 = r[0xD] & 0x0F; // D40D.0-3 Release

                ushort voice3f = (ushort)(r[0xF] << 8 | r[0xE]); // D40F-D40E Voice 2 Freq
                ushort pwdc3 = (ushort)(((r[0x11] & 0x0F) << 8) | r[0x10]); // D411-D410 Pulse Wave Duty Cycle
                ushort mode3 = (ushort)r[0x12]; // D412 Control2 Reg  NOISE PULSE SAWTOOTH TRIANGLE TEST RING1 SYNC1 GATE(KON)
                uint attack3 = r[0x13] >> 4; // D413.4-7 Attack
                uint decay3 = r[0x13] & 0x0F; // D413.0-3 Decay
                uint sustain3 = r[0x14] >> 4; // D414.4-7 Sustain
                uint release3 = r[0x14] & 0x0F; // D414.0-3 Release

                // Filter Registers
                ushort filtercutoff = (ushort)(r[0x16] << 3 | r[0x15]); // D416-D415 Filter Cutoff 
                uint filterreso = r[0x17] >> 4; // D417.4-7 Filter Resonanse 
                uint filterroute = r[0x17] & 0x0F; // D417.0-3 Filter Route (Enable bit)
                uint filtermode = r[0x18] >> 4; // D418.4-7 Filter Type (Mode... Hi/Band/Lo)
                uint mainvolume = r[0x18] & 0x0F; // D418.0-3 Main Volume (SID Volume is 4 bits)

                // Paddle Reg
                /*
                uint paddleX = r[0x19]; // D419 RO, Paddle Reg X
                uint paddleY = r[0x1A]; // D41A RO, Paddle Reg Y
                uint oscv3 = r[0x1B]; // D41B RO, Oscillator3 Value
                uint envv3 = r[0x1C]; // D41C RO, Oscillator3 Envelope
                */



                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"VOICE1 FREQ {voice1f:X4}");
                DrawBuff.drawFont4(frameBuffer, 2, y + 8, 0, $"VOICE1 PWDC {pwdc1:X4}");

                DrawBuff.drawFont4(frameBuffer, 2, y + 16, 0, $"VOICE1 MODE ");
                DrawBuff.drawFont4(frameBuffer, 2, y + 24, 0, $"" +
                    $"{((mode1 & 0b10000000) == 0x80 ? "NOISE" : "-----")} " +
                    $"{((mode1 & 0b01000000) == 0x40 ? "PULSE" : "-----")} " +
                    $"{((mode1 & 0b00100000) == 0x20 ? "SAWTOOTH" : "--------")} " +
                    $"{((mode1 & 0b00010000) == 0x10 ? "TRIANGLE" : "--------")} " +
                    $"{((mode1 & 0b00001000) == 0x08 ? "TEST" : "----")} " +
                    $"{((mode1 & 0b00000100) == 0x04 ? "RING 3" : "------")} " +
                    $"{((mode1 & 0b00000010) == 0x02 ? "SYNC 3" : "------")} " +
                    $"{((mode1 & 0b00000001) == 0x01 ? "GATE" : "----")}"); // Parse this
                DrawBuff.drawFont4(frameBuffer, 2, y + 32, 0, $"VOICE1 ADSR {attack1:X1} {decay1:X1} {sustain1:X1} {release1:X1}");

                y += 48;

                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"VOICE2 FREQ {voice2f:X4}");
                DrawBuff.drawFont4(frameBuffer, 2, y + 8, 0, $"VOICE2 PWDC {pwdc2:X4}");

                DrawBuff.drawFont4(frameBuffer, 2, y + 16, 0, $"VOICE2 MODE ");
                DrawBuff.drawFont4(frameBuffer, 2, y + 24, 0, $"" +
                    $"{((mode2 & 0b10000000) == 0x80 ? "NOISE" : "-----")} " +
                    $"{((mode2 & 0b01000000) == 0x40 ? "PULSE" : "-----")} " +
                    $"{((mode2 & 0b00100000) == 0x20 ? "SAWTOOTH" : "--------")} " +
                    $"{((mode2 & 0b00010000) == 0x10 ? "TRIANGLE" : "--------")} " +
                    $"{((mode2 & 0b00001000) == 0x08 ? "TEST" : "----")} " +
                    $"{((mode2 & 0b00000100) == 0x04 ? "RING 1" : "------")} " +
                    $"{((mode2 & 0b00000010) == 0x02 ? "SYNC 1" : "------")} " +
                    $"{((mode2 & 0b00000001) == 0x01 ? "GATE" : "----")}"); // Parse this
                DrawBuff.drawFont4(frameBuffer, 2, y + 32, 0, $"VOICE2 ADSR {attack2:X1} {decay2:X1} {sustain2:X1} {release2:X1}");

                y += 48;

                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"VOICE3 FREQ {voice3f:X4}");
                DrawBuff.drawFont4(frameBuffer, 2, y + 8, 0, $"VOICE3 PWDC {pwdc3:X4}");

                DrawBuff.drawFont4(frameBuffer, 2, y + 16, 0, $"VOICE3 MODE ");
                DrawBuff.drawFont4(frameBuffer, 2, y + 24, 0, $"" +
                    $"{((mode3 & 0b10000000) == 0x80 ? "NOISE" : "-----")} " +
                    $"{((mode3 & 0b01000000) == 0x40 ? "PULSE" : "-----")} " +
                    $"{((mode3 & 0b00100000) == 0x20 ? "SAWTOOTH" : "--------")} " +
                    $"{((mode3 & 0b00010000) == 0x10 ? "TRIANGLE" : "--------")} " +
                    $"{((mode3 & 0b00001000) == 0x08 ? "TEST" : "----")} " +
                    $"{((mode3 & 0b00000100) == 0x04 ? "RING 2" : "------")} " +
                    $"{((mode3 & 0b00000010) == 0x02 ? "SYNC 2" : "------")} " +
                    $"{((mode3 & 0b00000001) == 0x01 ? "GATE" : "----")}"); // Parse this
                DrawBuff.drawFont4(frameBuffer, 2, y + 32, 0, $"VOICE3 ADSR {attack3:X1} {decay3:X1} {sustain3:X1} {release3:X1}");

                y += 48;

                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"FILTER CUTOFF FREQ {filtercutoff:X4}");
                DrawBuff.drawFont4(frameBuffer, 2, y + 8, 0, $"FILTER RESONANCE {filterreso:X2}");

                DrawBuff.drawFont4(frameBuffer, 2, y + 16, 0, $"FILTER ROUTE");
                DrawBuff.drawFont4(frameBuffer, 2, y + 24, 0, $"" +
                    $"{((filterroute & 0b00001000) == 0x08 ? "EXT.IN" : "------")} " +
                    $"{((filterroute & 0b00000100) == 0x04 ? "VOICE3" : "------")} " +
                    $"{((filterroute & 0b00000010) == 0x02 ? "VOICE2" : "------")} " +
                    $"{((filterroute & 0b00000001) == 0x01 ? "VOICE1" : "------")}");

                DrawBuff.drawFont4(frameBuffer, 2, y + 32, 0, $"FILTER MODE");
                DrawBuff.drawFont4(frameBuffer, 2, y + 40, 0, $"" +
                    $"{((filtermode & 0b00001000) == 0x08 ? "MUTE V3" : "-------")} " +
                    $"{((filtermode & 0b00000100) == 0x04 ? "HIGHPASS" : "--------")} " +
                    $"{((filtermode & 0b00000010) == 0x02 ? "BANDPASS" : "--------")} " +
                    $"{((filtermode & 0b00000001) == 0x01 ? "LOWPASS" : "-------")}");
                DrawBuff.drawFont4(frameBuffer, 2, y + 48, 0, $"MAIN VOLUME {mainvolume:X2}");

                y += 56;

                SidTuneInfo sti = CurSID.tuneInfo;
                SidConfig cfg = CurSID.cfg;
                sidplayfp curengine = CurSID.GetCurrentEngineContext();
                SidInfo si = curengine.info();

                DrawBuff.drawFont4(frameBuffer, 2, y, 0, $"LOAD ADDR {sti.getLoadAddr():X4}h");
                DrawBuff.drawFont4(frameBuffer, 2, y + 8, 0, $"INIT ADDR {sti.getInitAddr():X4}h");
                DrawBuff.drawFont4(frameBuffer, 2, y + 16, 0, $"PLAY ADDR {sti.getPlayAddr():X4}h");
                DrawBuff.drawFont4(frameBuffer, 2, y + 24, 0, $"{sti.sidModel((uint)p)} {sti.getClockSpeed()}; CUR:{cfg.defaultSidModel} SPD:{si.getSpeedString()}");


                y += 32;
                //return;
            }



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

            var Reg = RegMan.GetData();

            if (Reg is byte[])
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
                for (var i = 0; i < Math.Min(r.Length, ms); i++)
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
                    if (i % 8 == 0)
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
                        int m = r[j][i];
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
            else if (e.Button == MouseButtons.Left) RegMan.Next();
        }


    }
}
