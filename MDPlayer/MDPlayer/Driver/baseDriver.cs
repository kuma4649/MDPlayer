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

        public int[] YM2151Hosei = new int[2] { 0, 0 };

        protected byte[] vgmBuf = null;
        protected ChipRegister chipRegister = null;
        protected enmModel model = enmModel.VirtualModel;
        protected enmUseChip useChip = enmUseChip.Unuse;
        protected uint latency = 1000;

        public abstract bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip useChip, uint latency);
        public abstract void oneFrameProc();
        public abstract GD3 getGD3Info(byte[] buf, uint vgmGd3);

    }
}
