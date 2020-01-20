using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZGM.ZgmChip
{
    public class Conductor : ZgmChip
    {

        public Conductor(ChipRegister chipRegister, Setting setting, byte[] vgmBuf) : base(2)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.Conductor;
            name = "CONDUCTOR";
            Model = EnmModel.VirtualModel;
            Number = 0;
            Hosei = 0;
        }

        public override void Setup(int chipIndex, ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<byte, uint>> cmdTable)
        {
            base.Setup(chipIndex, ref dataPos, ref cmdTable);

            if (cmdTable.ContainsKey(defineInfo.commandNo)) cmdTable.Remove(defineInfo.commandNo);
            cmdTable.Add(defineInfo.commandNo, SendPort0);

        }

        private void SendPort0(byte od, ref uint vgmAdr)
        {
            vgmAdr += 3;
        }

    }
}
