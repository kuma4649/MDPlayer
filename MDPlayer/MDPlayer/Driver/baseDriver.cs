using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public abstract class baseDriver
    {
        public Setting setting = null;
        public double vgmSpeed = 1;
        protected double vgmSpeedCounter;
        public long Counter = 0;
        public long TotalCounter = 0;
        public long LoopCounter = 0;
        public uint vgmCurLoop = 0;
        public bool Stopped = false;
        public long vgmFrameCounter;
        public GD3 GD3 = new GD3();
        public string Version = "";
        public string UsedChips = "";
        public int vstDelta = 0;

        public int[] YM2151Hosei = new int[2] { 0, 0 };

        protected byte[] vgmBuf = null;
        protected ChipRegister chipRegister = null;
        protected EnmChip[] useChip = new EnmChip[] { EnmChip.Unuse };
        protected uint latency = 1000;
        protected uint waitTime = 0;

        public string errMsg { get; internal set; }

        public abstract bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime);
        public abstract void oneFrameProc();
        public abstract GD3 getGD3Info(byte[] buf, uint vgmGd3);

        public void SetYM2151Hosei(SoundManager.Chip chip, float YM2151ClockValue)
        {
            chip.Hosei = 0;
            int clock = chipRegister.YM2151GetClock((byte)chip.Number);
            if (clock != -1)
            {
                chip.Hosei = Common.GetYM2151Hosei(YM2151ClockValue, clock);
            }
        }

    }
}
