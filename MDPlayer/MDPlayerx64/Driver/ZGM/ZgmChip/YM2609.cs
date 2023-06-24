namespace MDPlayer.Driver.ZGM.ZgmChip
{
    public class YM2609 : ZgmChip
    {

        public YM2609(ChipRegister chipRegister, Setting setting, byte[] vgmBuf) : base(12 + 6 + 12 + 6 + 3)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.YM2609;
            name = "YM2609";
            Model = EnmModel.VirtualModel;
            Number = 0;
            Hosei = 0;
        }

        public override void Setup(int chipIndex, ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<byte, uint>> cmdTable)
        {
            base.Setup(chipIndex, ref dataPos, ref cmdTable);

            if (cmdTable.ContainsKey(defineInfo.commandNo)) cmdTable.Remove(defineInfo.commandNo);
            cmdTable.Add(defineInfo.commandNo, SendPort0);

            if (cmdTable.ContainsKey(defineInfo.commandNo + 1)) cmdTable.Remove(defineInfo.commandNo + 1);
            cmdTable.Add(defineInfo.commandNo + 1, SendPort1);

            if (cmdTable.ContainsKey(defineInfo.commandNo + 2)) cmdTable.Remove(defineInfo.commandNo + 2);
            cmdTable.Add(defineInfo.commandNo + 2, SendPort2);

            if (cmdTable.ContainsKey(defineInfo.commandNo + 3)) cmdTable.Remove(defineInfo.commandNo + 3);
            cmdTable.Add(defineInfo.commandNo + 3, SendPort3);
        }

        private void SendPort0(byte od, ref uint vgmAdr)
        {
            chipRegister.writeYM2609(Index, 0, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2], Model);
            vgmAdr += 3;
        }

        private void SendPort1(byte od, ref uint vgmAdr)
        {
            chipRegister.writeYM2609(Index, 1, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2], Model);
            vgmAdr += 3;
        }

        private void SendPort2(byte od, ref uint vgmAdr)
        {
            chipRegister.writeYM2609(Index, 2, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2], Model);
            vgmAdr += 3;
        }

        private void SendPort3(byte od, ref uint vgmAdr)
        {
            chipRegister.writeYM2609(Index, 3, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2], Model);
            vgmAdr += 3;
        }

    }
}