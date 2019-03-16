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
        public bool isDataBlock = false;

        public int[] YM2151Hosei = new int[2] { 0, 0 };

        protected byte[] vgmBuf = null;
        protected ChipRegister chipRegister = null;
        protected EnmModel model = EnmModel.VirtualModel;
        protected EnmChip[] useChip = new EnmChip[] { EnmChip.Unuse };
        protected uint latency = 1000;
        protected uint waitTime = 0;

        public string errMsg { get; internal set; }

        public abstract bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime);
        public abstract void oneFrameProc();
        public abstract GD3 getGD3Info(byte[] buf, uint vgmGd3);

        public void SetYM2151Hosei(float YM2151ClockValue)
        {
            for (int chipID = 0; chipID < 2; chipID++)
            {
                YM2151Hosei[chipID] = Common.GetYM2151Hosei(YM2151ClockValue, 3579545);
                if (model == EnmModel.RealModel)
                {
                    YM2151Hosei[chipID] = 0;
                    int clock = chipRegister.getYM2151Clock((byte)chipID);
                    if (clock != -1)
                    {
                        YM2151Hosei[chipID] = Common.GetYM2151Hosei(YM2151ClockValue, clock);
                    }
                }
            }
        }

    }
}
