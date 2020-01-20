using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZGM.ZgmChip
{
    public abstract class ZgmChip : Chip
    {
        protected ChipRegister chipRegister;
        protected Setting setting;
        protected byte[] vgmBuf;

        public string name;
        public DefineInfo defineInfo;

        public ZgmChip(int ch) : base(ch)
        {
        }

        public virtual void Setup(int chipIndex, ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<byte, uint>> cmdTable)
        {
            this.Index = chipIndex;
            defineInfo = new DefineInfo();
            defineInfo.length = vgmBuf[dataPos + 0x03];
            defineInfo.chipIdentNo = Common.getLE32(vgmBuf, dataPos + 0x4);
            defineInfo.commandNo = (int)Common.getLE16(vgmBuf, dataPos + 0x8);
            defineInfo.clock = (int)Common.getLE32(vgmBuf, dataPos + 0xa);
            defineInfo.option = null;
            if (defineInfo.length > 14)
            {
                defineInfo.option = new byte[defineInfo.length - 14];
                for (int j = 0; j < defineInfo.length - 14; j++)
                {
                    defineInfo.option[j] = vgmBuf[dataPos + 0x0e + j];
                }
            }

            dataPos += defineInfo.length;
        }

    }
}
