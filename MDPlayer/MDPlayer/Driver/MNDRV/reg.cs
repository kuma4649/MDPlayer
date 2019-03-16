using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MNDRV
{
    public class reg
    {
        public UInt32 D0_L;
        public UInt32 D1_L;
        public UInt32 D2_L;
        public UInt32 D3_L;
        public UInt32 D4_L;
        public UInt32 D5_L;
        public UInt32 D6_L;
        public UInt32 D7_L;
        public UInt32 a0;
        public UInt32 a1;
        public UInt32 a2;
        public UInt32 a3;
        public UInt32 a4;
        public UInt32 a5;
        public UInt32 a6;
        public UInt32 a7;

        public UInt32 SR;

        public List<UInt32> Arg = new List<uint>();


        public UInt32 D0_B { get { return (byte)D0_L; } set { D0_L = (D0_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D1_B { get { return (byte)D1_L; } set { D1_L = (D1_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D2_B { get { return (byte)D2_L; } set { D2_L = (D2_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D3_B { get { return (byte)D3_L; } set { D3_L = (D3_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D4_B { get { return (byte)D4_L; } set { D4_L = (D4_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D5_B { get { return (byte)D5_L; } set { D5_L = (D5_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D6_B { get { return (byte)D6_L; } set { D6_L = (D6_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D7_B { get { return (byte)D7_L; } set { D7_L = (D7_L & 0xffffff00) | (value & 0xff); } }
        public UInt32 D0_W { get { return (ushort)D0_L; } set { D0_L = (D0_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D1_W { get { return (ushort)D1_L; } set { D1_L = (D1_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D2_W { get { return (ushort)D2_L; } set { D2_L = (D2_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D3_W { get { return (ushort)D3_L; } set { D3_L = (D3_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D4_W { get { return (ushort)D4_L; } set { D4_L = (D4_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D5_W { get { return (ushort)D5_L; } set { D5_L = (D5_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D6_W { get { return (ushort)D6_L; } set { D6_L = (D6_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 D7_W { get { return (ushort)D7_L; } set { D7_L = (D7_L & 0xffff0000) | (value & 0xffff); } }
        public UInt32 SR_W { get { return (ushort)SR; } set { SR = (SR & 0xffff0000) | (value & 0xffff); } }

        public void SetD0_L(uint v) { D0_L = v; }
        public void SetD1_L(uint v) { D1_L = v; }
        public void SetD2_L(uint v) { D2_L = v; }
        public void SetD3_L(uint v) { D3_L = v; }
        public void SetD4_L(uint v) { D4_L = v; }
        public void SetD5_L(uint v) { D5_L = v; }
        public void SetD6_L(uint v) { D6_L = v; }
        public void SetD7_L(uint v) { D7_L = v; }
        public void SetD0_L(int v) { D0_L = (uint)v; }
        public void SetD1_L(int v) { D1_L = (uint)v; }
        public void SetD2_L(int v) { D2_L = (uint)v; }
        public void SetD3_L(int v) { D3_L = (uint)v; }
        public void SetD4_L(int v) { D4_L = (uint)v; }
        public void SetD5_L(int v) { D5_L = (uint)v; }
        public void SetD6_L(int v) { D6_L = (uint)v; }
        public void SetD7_L(int v) { D7_L = (uint)v; }
        public void SetSR(int v) { SR = (uint)v; }

        public bool cryADD(byte a, byte b)
        {
            return a + b > 0xff;
        }

        public bool cryADD(UInt16 a, UInt16 b)
        {
            return a + b > 0xffff;
        }

        public bool cryADD(UInt32 a, UInt32 b)
        {
            return (Int64)a + (Int64)b > (Int64)0xffffffff;
        }

    }

    public class ab
    {
        public const UInt32 dummyAddress = 0xffffffff;

        public Dictionary<UInt32, Action> hlTRKANA_RESTADR = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_qtjob = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_mmljob_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_lfojob_adrs = new Dictionary<uint, Action>();
        //public Dictionary<UInt32, Action> hlw_psgenv_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_softenv_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_rrcut_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_echo_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_keyoff_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_keyoff_adrs2 = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_subcmd_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_setnote_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_inithlfo_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_we_exec_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_we_ycom_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_we_tone_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlw_we_pan_adrs = new Dictionary<uint, Action>();
        public Dictionary<UInt32, Action> hlINTEXECBUF = new Dictionary<uint, Action>();
    }

    public class FMTimer
    {
        private int TimerAregH;     // タイマーAの上位8ビット
        private int TimerAregL;     // タイマーAの下位2ビット
        private int TimerA;         // タイマーAのオーバーフロー設定値
        private double TimerAcounter;  // タイマーAのカウンター値
        private int TimerB;         // タイマーBのオーバーフロー設定値
        private double TimerBcounter;  // タイマーBのカウンター値
        private int TimerReg;       // タイマー制御レジスタ (下位4ビット+7ビット)
        private int StatReg;        // ステータスレジスタ (下位2ビット)
        private bool isOPM = false;
        private Action CsmKeyOn;
        private double step = 0.0;
        private double MasterClock = 3579545.0;

        public FMTimer(bool isOPM,Action CsmKeyOn,double MasterClock)
        {
            this.isOPM = isOPM;
            this.CsmKeyOn = CsmKeyOn;
            this.MasterClock = MasterClock;
            if (isOPM)
            {
                step = MasterClock / 64.0 / 1.0 / (double)Common.SampleRate;
            }
            else
            {
                step = MasterClock / 72.0 / 2.0 / (double)Common.SampleRate;
            }
        }

        public void timer()
        {
            int flag_set = 0;

            if ((TimerReg & 0x01) != 0)
            {   // TimerA 動作中
                TimerAcounter+=step;
                if (TimerAcounter >= TimerA)
                {
                    flag_set |= ((TimerReg >> 2) & 0x01);
                    TimerAcounter -= TimerA;
                    if ((TimerReg & 0x80) != 0) CsmKeyOn?.Invoke();
                }
            }

            if ((TimerReg & 0x02) != 0)
            {   // TimerB 動作中
                TimerBcounter+=step;
                if (TimerBcounter >= TimerB)
                {
                    flag_set |= ((TimerReg >> 2) & 0x02);
                    TimerBcounter -= TimerB;
                }
            }

            StatReg |= flag_set;
        }

        public void WriteReg(byte adr, byte data)
        {
            if (isOPM) WriteRegOPM(adr, data);
            else WriteRegOPN(adr, data);
        }

        private void WriteRegOPM(byte adr, byte data)
        {
            switch (adr)
            {
                case 0x10:
                case 0x11:
                    // TimerA
                    if (adr == 0x10) TimerAregH = data;
                    else TimerAregL = data & 3;
                    TimerA = 1024 - ((TimerAregH << 2) + TimerAregL);
                    break;

                case 0x12:
                    // TimerB
                    TimerB = (256 - (int)data) << (10 - 6);
                    break;

                case 0x14:
                    // タイマー制御レジスタ
                    TimerReg = data & 0x8F;
                    StatReg &= 0xFF - ((data >> 4) & 3);
                    break;
            }
        }

        private void WriteRegOPN(byte adr, byte data)
        {
            switch (adr)
            {
                case 0x24:
                case 0x25:
                    // TimerA
                    if (adr == 0x24) TimerAregH = data;
                    else TimerAregL = data & 3;
                    TimerA = 1024 - ((TimerAregH << 2) + TimerAregL);
                    break;

                case 0x26:
                    // TimerB
                    TimerB = (256 - (int)data) << (10 - 6);
                    break;

                case 0x27:
                    // タイマー制御レジスタ
                    TimerReg = data & 0x8F;
                    StatReg &= 0xFF - ((data >> 4) & 3);
                    break;
            }
        }

        public int ReadStatus()
        {
            return StatReg;
        }

    }
}
