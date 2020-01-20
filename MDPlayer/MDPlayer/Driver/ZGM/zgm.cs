using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZGM
{
    public class zgm : baseDriver
    {
        public const int FCC_ZGM = 0x204D475A;	// "ZGM "
        public const int FCC_GD3 = 0x20336447;  // "Gd3 "
        public const int FCC_DEF = 0x666544;  // "Def"
        public const int FCC_TRK = 0x6b7254;  // "Trk"

        private uint vgmEof;
        private long vgmLoopOffset = 0;
        private int chipCommandSize = 1;
        private long vgmDataOffset = 0;

        public delegate void RefAction<T1, T2>(T1 arg1, ref T2 arg2);
        private Dictionary<int, RefAction<byte, uint>> vgmCmdTbl = new Dictionary<int, RefAction<byte, uint>>();


        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            getZGMGD3Info(buf);
            return GD3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            if (!getZGMInfo(vgmBuf)) return false;

            return true;
        }

        public override void oneFrameProc()
        {
            throw new NotImplementedException();
        }

        private bool getZGMGD3Info(byte[] buf)
        {
            if (buf == null) return false;

            uint vgmGd3 = Common.getLE32(buf, 0x18);
            if (vgmGd3 == 0) return false;
            uint vgmGd3Id = Common.getLE32(buf, vgmGd3);
            if (vgmGd3Id != FCC_GD3) throw new ArgumentOutOfRangeException();

            vgmEof = Common.getLE32(vgmBuf, 0x04);

            uint version = Common.getLE32(vgmBuf, 0x08);
            //バージョンチェック
            if (version < 10) return false;
            Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

            TotalCounter = Common.getLE32(vgmBuf, 0x0c);
            if (TotalCounter < 0) return false;
            vgmLoopOffset = Common.getLE32(vgmBuf, 0x14);
            LoopCounter = Common.getLE32(vgmBuf, 0x10);

            uint defineAddress = Common.getLE32(vgmBuf, 0x1c);
            uint defineCount = Common.getLE16(vgmBuf, 0x24);
            //音源定義数チェック
            if (defineCount < 1) return false;

            chipCommandSize = (defineCount > 128) ? 2 : 1;

            uint trackAddress = Common.getLE32(vgmBuf, 0x20);
            uint trackCounter = Common.getLE16(vgmBuf, 0x26);
            vgmDataOffset = trackAddress + 11;
            //トラック数チェック
            if (trackCounter != 1) return false;
            uint fcc = Common.getLE24(vgmBuf, trackAddress);
            if (fcc != FCC_TRK) return false;
            uint trackLength = Common.getLE32(vgmBuf, trackAddress + 3);
            vgmLoopOffset = (int)Common.getLE32(vgmBuf, trackAddress + 7);
            if (vgmLoopOffset != 0) LoopCounter = 1;
            vgmEof = (uint)(trackAddress + trackLength);

            uint pos = defineAddress;

            Dictionary<string, int> chipCount = new Dictionary<string, int>();
            for (int i = 0; i < defineCount; i++)
            {
                fcc = Common.getLE24(vgmBuf, pos);
                if (fcc != FCC_DEF) return false;
                ZgmChip.ZgmChip chip = (new ZgmChip.ChipFactory()).Create(Common.getLE32(vgmBuf, pos + 0x4), chipRegister, setting, vgmBuf);
                if (chip == null) return false;//non support

                if (!chipCount.ContainsKey(chip.name)) chipCount.Add(chip.name, -1);
                chipCount[chip.name]++;

                chip.Setup(chipCount[chip.name], ref pos, ref vgmCmdTbl);
                //chips.Add(chip);
            }

            //UsedChips = GetUsedChipsString(chips);


            vgmGd3 += 12;// + 0x14;
            GD3 = Common.getGD3Info(buf, vgmGd3);
            GD3.UsedChips = UsedChips;

            return true;
        }

        private bool getZGMInfo(byte[] vgmBuf)
        {
            if (vgmBuf == null) return false;

            try
            {
                if (Common.getLE32(vgmBuf, 0) != FCC_ZGM) return false;

                if (!getZGMGD3Info(vgmBuf)) return false;
            }
            catch (Exception e)
            {
                log.Write(string.Format("XGMの情報取得中に例外発生 Message=[{0}] StackTrace=[{1}]", e.Message, e.StackTrace));
                return false;
            }

            return true;
        }

    }

    public class TrackInfo
    {
        public int offset = 0;
    }

    public class DefineInfo
    {
        public byte length = 14;
        public uint chipIdentNo = 0;
        public int commandNo = 0;
        public int clock = 0;
        public byte[] option = null;

        //public ClsChip chip = null;
        public int offset = 0;
    }

}
