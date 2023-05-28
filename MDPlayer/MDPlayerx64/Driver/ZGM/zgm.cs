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
        private uint vgmAdr;
        private int vgmWait;
        private bool vgmAnalyze;

        public delegate void RefAction<T1, T2>(T1 arg1, ref T2 arg2);
        private Dictionary<int, RefAction<byte, uint>> vgmCmdTbl = new Dictionary<int, RefAction<byte, uint>>();
        public List<ZgmChip.Chip> chips = null;

        public uint SN76489ClockValue = 3579545;
        public uint YM2612ClockValue = 7670454;
        public uint RF5C68ClockValue = 12500000;
        public uint RF5C164ClockValue = 12500000;
        public uint PWMClockValue = 23011361;
        public uint C140ClockValue = 21390;
        public MDSound.c140.C140_TYPE C140Type = MDSound.c140.C140_TYPE.ASIC219;
        public uint OKIM6258ClockValue = 4000000;
        public byte OKIM6258Type = 0;
        public uint OKIM6295ClockValue = 4000000;
        public uint SEGAPCMClockValue = 4000000;
        public int SEGAPCMInterface = 0;
        public uint YM2151ClockValue;
        public uint YM2608ClockValue;
        public uint YM2609ClockValue;
        public uint YM2203ClockValue;
        public uint YM2610ClockValue;
        public uint YM3812ClockValue;
        public uint YM3526ClockValue;
        public uint Y8950ClockValue;
        public uint YMF262ClockValue;
        public uint YMF271ClockValue;
        public uint YMF278BClockValue;
        public uint YMZ280BClockValue;
        public uint AY8910ClockValue;
        public uint YM2413ClockValue;
        public uint HuC6280ClockValue;
        public uint QSoundClockValue;
        public uint C352ClockValue;
        public byte C352ClockDivider;
        public uint GA20ClockValue;
        public uint K053260ClockValue;
        public uint K054539ClockValue;
        public byte K054539Flags;
        public uint K051649ClockValue;
        public uint DMGClockValue;
        public uint NESClockValue;
        public uint VRC6ClockValue;
        public uint MultiPCMClockValue;
        public uint GigatronClockValue;

        public List<ZgmChip.Chip> CONDUCTOR = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> DACControl = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> AY8910 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> C140 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> C352 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> MIDI = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> HuC6280 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> K051649 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> RF5C164 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> DMG = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> NES = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> VRC6 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> Gigatron = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> SEGAPCM = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> SN76489 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2151 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2203 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2413 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM3526 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> Y8950 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM3812 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YMF262 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2608 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2609 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2610 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YM2612 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> QSound = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> K053260 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> PPZ8 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> PPSDRV = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> P86 = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YMF278B = new List<ZgmChip.Chip>();
        public List<ZgmChip.Chip> YMF271 = new List<ZgmChip.Chip>();

        private byte[][] ym2609AdpcmA = new byte[2][] { null, null };

        public void ZgmSetup(List<ZgmChip.Chip> chips)
        {
            CONDUCTOR.Clear();
            C140.Clear();
            C352.Clear();
            HuC6280.Clear();
            K051649.Clear();
            K053260.Clear();
            QSound.Clear();
            RF5C164.Clear();
            AY8910.Clear();
            DMG.Clear();
            NES.Clear();
            VRC6.Clear();
            Gigatron.Clear();
            SEGAPCM.Clear();
            SN76489.Clear();
            YM2151.Clear();
            YM2203.Clear();
            YM2413.Clear();
            YM3526.Clear();
            Y8950.Clear();
            YM3812.Clear();
            YMF262.Clear();
            YMF278B.Clear();
            YMF271.Clear();
            YM2608.Clear();
            YM2609.Clear();
            YM2610.Clear();
            YM2612.Clear();
            MIDI.Clear();

            foreach (ZgmChip.Chip c in chips)
            {
                if (c is Driver.ZGM.ZgmChip.Conductor) CONDUCTOR.Add(c);
                //if (c is Driver.ZGM.ZgmChip.SN76489) SN76489.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM2413) YM2413.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM2612) YM2612.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM2151) YM2151.Add(c);
                //if (c is Driver.ZGM.ZgmChip.SEGAPCM) SEGAPCM.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM2203) YM2203.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM2608) YM2608.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM2610) YM2610.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM3812) YM3812.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YM3526) YM3526.Add(c);
                //if (c is Driver.ZGM.ZgmChip.Y8950) Y8950.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YMF262) YMF262.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YMF278B) YMF278B.Add(c);
                //if (c is Driver.ZGM.ZgmChip.YMF271) YMF271.Add(c);
                //if (c is Driver.ZGM.ZgmChip.RF5C164) RF5C164.Add(c);
                //if (c is Driver.ZGM.ZgmChip.AY8910) AY8910.Add(c);
                //if (c is Driver.ZGM.ZgmChip.DMG) DMG.Add(c);
                //if (c is Driver.ZGM.ZgmChip.NES) NES.Add(c);
                //if (c is Driver.ZGM.ZgmChip.VRC6) VRC6.Add(c);
                //if (c is Driver.ZGM.ZgmChip.Gigatron) Gigatron.Add(c);
                //if (c is Driver.ZGM.ZgmChip.K051649) K051649.Add(c);
                //if (c is Driver.ZGM.ZgmChip.HuC6280) HuC6280.Add(c);
                //if (c is Driver.ZGM.ZgmChip.C140) C140.Add(c);
                //if (c is Driver.ZGM.ZgmChip.K053260) K053260.Add(c);
                //if (c is Driver.ZGM.ZgmChip.QSound) QSound.Add(c);
                //if (c is Driver.ZGM.ZgmChip.C352) C352.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2609) YM2609.Add(c);
                //if (c is Driver.ZGM.ZgmChip.MidiGM) MIDI.Add(c);
            }
        }


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

            LoopCounter = 0;

            if (!getZGMInfo(vgmBuf)) return false;

            vgmAdr = (uint)vgmDataOffset;
            vgmWait = 0;
            vgmAnalyze = true;
            Counter = 0;
            TotalCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            try
            {
                setCommands();
            }
            catch (Exception e)
            {
                log.Write(e.StackTrace);
                return false;
            }

            return true;
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException("このdriverはこのメソッドを必要としない");
        }

        public override void oneFrameProc()
        {
            try
            {
                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameVGMMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        private void oneFrameVGMMain()
        {

            if (vgmWait > 0)
            {
                //if (model == enmModel.VirtualModel)
                //oneFrameVGMStream();
                vgmWait--;
                Counter++;
                vgmFrameCounter++;
                return;
            }

            if (!vgmAnalyze)
            {
                Stopped = true;
                return;
            }

            {
                if (vgmAdr >= vgmBuf.Length || vgmAdr >= vgmEof)
                {
                    if (LoopCounter != 0)
                    {
                        vgmAdr = (uint)(vgmLoopOffset);// + 0x1c);
                        vgmCurLoop++;
                        Counter = 0;
                    }
                    else
                    {
                        vgmAnalyze = false;
                        //vgmAdr = (uint)vgmBuf.Length;
                        return;
                    }
                }

                byte cmd = vgmBuf[vgmAdr];

                if (vgmCmdTbl.ContainsKey(cmd))
                {
                    //Console.WriteLine("{0:X05} : {1:X02} {2:X02} {3:X02}", vgmAdr, vgmBuf[vgmAdr].val, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
                    vgmCmdTbl[cmd](cmd, ref vgmAdr);
                }
                else
                {
                    //わからんコマンド
                    Console.WriteLine("unknown command: Adr:{0:X} Dat:{1:X}", vgmAdr, vgmBuf[vgmAdr]);
                    vgmAdr++;
                }

            }

            //for (long i = Counter; i < Audio.DriverSeqCounter; i++)
            //{
            //    oneFrameVGMStream(i);
            //}

            vgmWait--;
            Counter++;
            vgmFrameCounter++;

        }

        private bool getZGMGD3Info(byte[] buf)
        {
            if (buf == null) return false;

            chips = new List<ZgmChip.Chip>();
            UsedChips = "";

            SN76489ClockValue = 0;
            YM2612ClockValue = 0;
            YM2151ClockValue = 0;
            SEGAPCMClockValue = 0;
            YM2203ClockValue = 0;
            YM2608ClockValue = 0;
            YM2609ClockValue = 0;
            YM2610ClockValue = 0;
            YM3812ClockValue = 0;
            YMF262ClockValue = 0;
            RF5C68ClockValue = 0;
            RF5C164ClockValue = 0;
            PWMClockValue = 0;
            OKIM6258ClockValue = 0;
            C140ClockValue = 0;
            OKIM6295ClockValue = 0;
            AY8910ClockValue = 0;
            YM2413ClockValue = 0;
            HuC6280ClockValue = 0;
            K054539ClockValue = 0;
            NESClockValue = 0;
            VRC6ClockValue = 0;
            MultiPCMClockValue = 0;
            GigatronClockValue = 0;

            uint vgmGd3 = Common.getLE32(buf, 0x18);
            if (vgmGd3 == 0) return false;
            uint vgmGd3Id = Common.getLE32(buf, vgmGd3);
            if (vgmGd3Id != FCC_GD3) throw new ArgumentOutOfRangeException();

            vgmBuf = buf;
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
                chips.Add(chip);
            }

            UsedChips = GetUsedChipsString(chips);
            ZgmSetup(chips);
            SetupDicChipCmdNo();

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
                log.ForcedWrite("XGMの情報取得中に例外発生 Message=[{0}] StackTrace=[{1}]", e.Message, e.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 使用チップを列挙した文字列を得る。
        /// (同じチップはカウントされ"x9"のように個数でまとめる。)
        /// </summary>
        private string GetUsedChipsString(List<ZgmChip.Chip> chips)
        {
            //同じチップの数をそれぞれ集計する
            Dictionary<string, int> c = new Dictionary<string, int>();
            foreach (ZgmChip.ZgmChip chip in chips)
            {
                if (c.ContainsKey(chip.name)) c[chip.name]++;
                else c.Add(chip.name, 1);
            }
            List<string> cc = new List<string>();
            foreach (string k in c.Keys)
            {
                cc.Add(k + (c[k] < 2 ? "" : string.Format(" x{0}", c[k])));
            }

            return string.Join(" , ", cc);
        }


        public Dictionary<int, Driver.ZGM.ZgmChip.ZgmChip> dicChipCmdNo = new Dictionary<int, Driver.ZGM.ZgmChip.ZgmChip>();

        private void SetupDicChipCmdNo()
        {
            dicChipCmdNo.Clear();

            foreach (Driver.ZGM.ZgmChip.ZgmChip c in CONDUCTOR) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in SN76489) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2413) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2612) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2151) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in SEGAPCM) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2203) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2608) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2610) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM3812) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM3526) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in Y8950) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YMF262) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YMF278B) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in YMF271) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in RF5C164) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in AY8910) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in DMG) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in NES) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in VRC6) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in K051649) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in HuC6280) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in C140) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in K053260) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in QSound) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in C352) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2609) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in MIDI) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            //foreach (Driver.ZGM.ZgmChip.ZgmChip c in Gigatron) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
        }

        private void setCommands()
        {

            vgmCmdTbl.Add(0x01, vcWaitNSamples);
            vgmCmdTbl.Add(0x02, vcWait735Samples);
            vgmCmdTbl.Add(0x03, vcWait882Samples);
            //vgmCmdTbl.Add(0x04, vcOverrideLength);

            //vgmCmdTbl.Add(0x06, vcEndOfSoundData);
            vgmCmdTbl.Add(0x07, vcDataBlock);
            //vgmCmdTbl.Add(0x08, vcPCMRamWrite);
            vgmCmdTbl.Add(0x09, vcDummyChip);

            //vgmCmdTbl.Add(0x10, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x11, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x12, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x13, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x14, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x15, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x16, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x17, vcWaitN1Samples);

            //vgmCmdTbl.Add(0x18, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x19, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x1a, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x1b, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x1c, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x1d, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x1e, vcWaitN1Samples);
            //vgmCmdTbl.Add(0x1f, vcWaitN1Samples);

            //vgmCmdTbl.Add(0x20, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x21, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x22, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x23, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x24, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x25, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x26, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x27, vcWaitNSamplesAndSendYM26120x2a);

            //vgmCmdTbl.Add(0x28, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x29, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x2a, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x2b, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x2c, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x2d, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x2e, vcWaitNSamplesAndSendYM26120x2a);
            //vgmCmdTbl.Add(0x2f, vcWaitNSamplesAndSendYM26120x2a);

            //vgmCmdTbl.Add(0x30, vcSetupStreamControl);
            //vgmCmdTbl.Add(0x31, vcSetStreamData);
            //vgmCmdTbl.Add(0x32, vcSetStreamFrequency);
            //vgmCmdTbl.Add(0x33, vcStartStream);
            //vgmCmdTbl.Add(0x34, vcStopStream);
            //vgmCmdTbl.Add(0x35, vcStartStreamFastCall);

            //vgmCmdTbl.Add(0x40, vcSeekToOffsetInPCMDataBank);

        }

        private void vcWaitNSamples(byte od, ref uint vgmAdr)
        {
            vgmWait += (int)Common.getLE16(vgmBuf, vgmAdr + 1);
            vgmAdr += 3;
        }

        private void vcWait735Samples(byte od, ref uint vgmAdr)
        {
            vgmWait += 735;
            vgmAdr++;
        }

        private void vcWait882Samples(byte od, ref uint vgmAdr)
        {
            vgmWait += 882;
            vgmAdr++;
        }

        private void vcDataBlock(byte od, ref uint vgmAdr)
        {

            int chipCommandNumber = (chipCommandSize == 1) ? (int)vgmBuf[vgmAdr + 1] : (int)Common.getLE16(vgmBuf, vgmAdr + 2);
            byte bType = (chipCommandSize == 1) ? vgmBuf[vgmAdr + 2] : vgmBuf[vgmAdr + 4];
            vgmAdr += (uint)((chipCommandSize == 1) ? 0 : 2);
            uint bAdr = vgmAdr + 7;
            uint bLen = Common.getLE32(vgmBuf, vgmAdr + 3);
            //byte chipID = 0;
            //if ((bLen & 0x80000000) != 0)
            //{
            //    bLen &= 0x7fffffff;
            //    chipID = 1;
            //}
            if (!dicChipCmdNo.ContainsKey(chipCommandNumber))
            {
                //未定義のccnの場合(あってはならないが)
                vgmAdr += (uint)bLen + 7;
                return;
            }
            Driver.ZGM.ZgmChip.ZgmChip chip = dicChipCmdNo[chipCommandNumber];

            switch (chip)
            {
                //case Driver.ZGM.ZgmChip.HuC6280 _:
                //case Driver.ZGM.ZgmChip.YM2612 _:
                //    pcmDat.Clear();
                //    for (uint i = bAdr; i < bAdr + bLen; i++) pcmDat.Add(vgmBuf[i].val);
                //    chipRegister.DACControlAddPCMData(bType, bLen, 0, pcmDat.ToArray());
                //    //AddPCMData(bType, bLen, bAdr);
                //    vgmAdr += (uint)bLen + 7;
                //    break;
                case Driver.ZGM.ZgmChip.YM2609 _:
                    uint romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    //int adpcmAdr = 0x100;
                    if (bType == 1)
                    {
                        //adpcmAdr = 0x300;
                    }
                    else if (bType == 2)
                    {
                        //adpcmAdr = 0x311;
                    }
                    else if (bType == 3)
                    {
                        if (ym2609AdpcmA[chip.Index] == null || ym2609AdpcmA[chip.Index].Length != romSize)
                            ym2609AdpcmA[chip.Index] = new byte[romSize];
                        if (ym2609AdpcmA[chip.Index].Length > 0)
                        {
                            for (int cnt = 0; cnt < bLen - 8; cnt++)
                            {
                                ym2609AdpcmA[chip.Index][startAddress + cnt] = vgmBuf[vgmAdr + 15 + cnt];
                            }
                            chipRegister.writeYM2609AdpcmA(chip.Index, ym2609AdpcmA[chip.Index], model);
                        }
                        vgmAdr += (uint)bLen + 7;
                        break;
                    }
                    else if (bType == 4)
                    {
                        int blockSize = 1024 * 2;
                        for (int j = 0; j < bLen - 8; j += blockSize + 2)
                        {
                            int n = vgmBuf[vgmAdr + 15 + j];
                            byte[] wav = new byte[blockSize];
                            for (int i = 0; i < blockSize; i++)
                            {
                                wav[i] = vgmBuf[vgmAdr + 17 + i + j];
                            }
                            chipRegister.writeYM2609SetOperatorWaveDic( chip.Index, n, wav,model);
                        }
                        vgmAdr += (uint)bLen + 7;
                        break;
                    }

                    byte[] pcm012Buf = new byte[bLen];
                    for (int cnt = 0; cnt < bLen - 8; cnt++)
                    {
                        pcm012Buf[cnt] = vgmBuf[vgmAdr + 15 + cnt];
                    }
                    chipRegister.writeYM2609SetAdpcm012(chip.Index, bType, pcm012Buf, model);

                    //int adpcmAdrP = adpcmAdr >> 8;
                    //adpcmAdr = (byte)adpcmAdr;

                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x00, 0x20, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x00, 0x21, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x00, 0x00, model);
                                                         
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x10, 0x00, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x10, 0x80, model);
                                                         
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x00, 0x61, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x00, 0x68, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x01, 0x00, model);
                                                         
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x02, (byte)(startAddress >> 2), model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x03, (byte)(startAddress >> 10), model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x04, 0xff, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x05, 0xff, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x0c, 0xff, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP,adpcmAdr+ 0x0d, 0xff, model);
                    ////};
                    ////// データ転送
                    //for (int cnt = 0; cnt < bLen - 8; cnt++)
                    //{
                    //    chipRegister.writeYM2609(chip.Index,adpcmAdrP, adpcmAdr+ 0x08, vgmBuf[vgmAdr + 15 + cnt], model);
                    //}
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP, adpcmAdr + 0x00, 0x00, model);
                    //chipRegister.writeYM2609(chip.Index, adpcmAdrP, adpcmAdr + 0x10, 0x80, model);

                    ////dumpData(dummyChip, "YM2609_ADPCM", vgmAdr + 15, bLen - 8);
                    vgmAdr += (uint)bLen + 7;
                    break;
                //case Driver.ZGM.ZgmChip.SEGAPCM _:// 0x80:
                //    uint segapcm_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint segapcm_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.SEGAPCMWritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, segapcm_romSize, segapcm_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    break;
                //case Driver.ZGM.ZgmChip.YM2608 _:
                //    //uint opna_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint opna_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    List<PackData> opna_data = new List<PackData>
                //            {
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x20,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x21,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x00,null),

                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x10, 0x00,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x10, 0x80,null),

                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x61,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x68,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x01, 0x00,null),

                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x02, (byte)(opna_startAddress >> 2),null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x03, (byte)(opna_startAddress >> 10),null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x04, 0xff,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x05, 0xff,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x0c, 0xff,null),
                //                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x0d, 0xff,null)
                //            };

                //    // データ転送
                //    for (int cnt = 0; cnt < bLen - 8; cnt++)
                //    {
                //        opna_data.Add(new PackData(null, chipRegister.YM2608[chip.Index], 0, 0x100 + 0x08, vgmBuf[vgmAdr + 15 + cnt].val, null));
                //    }
                //    opna_data.Add(new PackData(null, chipRegister.YM2608[chip.Index], 0, 0x100 + 0x00, 0x00, null));
                //    opna_data.Add(new PackData(null, chipRegister.YM2608[chip.Index], 0, 0x100 + 0x10, 0x80, null));

                //    //chipRegister.setYM2608Register(0x1, 0x10, 0x13, model);
                //    //chipRegister.setYM2608Register(0x1, 0x10, 0x80, model);
                //    //chipRegister.setYM2608Register(0x1, 0x00, 0x60, model);
                //    //chipRegister.setYM2608Register(0x1, 0x01, 0x00, model);

                //    //chipRegister.setYM2608Register(0x1, 0x02, (int)((startAddress >> 2) & 0xff), model);
                //    //chipRegister.setYM2608Register(0x1, 0x03, (int)((startAddress >> 10) & 0xff), model);
                //    //chipRegister.setYM2608Register(0x1, 0x04, (int)(((startAddress + bLen - 8) >> 2) & 0xff), model);
                //    //chipRegister.setYM2608Register(0x1, 0x05, (int)(((startAddress + bLen - 8) >> 10) & 0xff), model);
                //    //chipRegister.setYM2608Register(0x1, 0x0c, 0xff, model);
                //    //chipRegister.setYM2608Register(0x1, 0x0d, 0xff, model);

                //    //for (int cnt = 0; cnt < bLen - 8; cnt++)
                //    //{
                //    //    chipRegister.setYM2608Register(0x1, 0x08, vgmBuf[vgmAdr + 15 + cnt].val, model);
                //    //    chipRegister.setYM2608Register(0x1, 0x10, 0x1b, model);
                //    //    chipRegister.setYM2608Register(0x1, 0x10, 0x13, model);
                //    //}

                //    //chipRegister.setYM2608Register(0x1, 0x00, 0x00, model);
                //    //chipRegister.setYM2608Register(0x1, 0x10, 0x80, model);

                //    SoundManager.Chip opna_dummyChip = new SoundManager.Chip(1);

                //    opna_dummyChip.Model = chipRegister.YM2608[chip.Index].Model;
                //    opna_dummyChip.Delay = chipRegister.YM2608[chip.Index].Delay;
                //    opna_dummyChip.Device = chipRegister.YM2608[chip.Index].Device;
                //    opna_dummyChip.Number = chipRegister.YM2608[chip.Index].Number;
                //    opna_dummyChip.Use = chipRegister.YM2608[chip.Index].Use;

                //    if (chipRegister.YM2608[chip.Index].Model == EnmVRModel.RealModel)
                //    {
                //        if (setting.YM2608Type.OnlyPCMEmulation)
                //        {
                //            opna_dummyChip.Model = EnmVRModel.VirtualModel;
                //        }
                //        else
                //        {
                //            Audio.DriverSeqCounter += (long)(bLen * 1.5);
                //        }
                //    }

                //    chipRegister.YM2608SetRegister(od, Audio.DriverSeqCounter, opna_dummyChip, opna_data.ToArray());

                //    //dumpData(dummyChip, "YM2608_ADPCM", vgmAdr + 15, bLen - 8);
                //    vgmAdr += (uint)bLen + 7;
                //    break;

                //case Driver.ZGM.ZgmChip.YM2610 _:
                //    uint opnb_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint opnb_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    if (bType == 0x82)
                //    {
                //        if (ym2610AdpcmA[chip.Index] == null || ym2610AdpcmA[chip.Index].Length != opnb_romSize) ym2610AdpcmA[chip.Index] = new byte[opnb_romSize];
                //        if (ym2610AdpcmA[chip.Index].Length > 0)
                //        {
                //            for (int cnt = 0; cnt < bLen - 8; cnt++)
                //            {
                //                ym2610AdpcmA[chip.Index][opnb_startAddress + cnt] = vgmBuf[vgmAdr + 15 + cnt].val;
                //            }
                //            //if (model == EnmModel.VirtualModel) 
                //            chipRegister.YM2610WriteSetAdpcmA(od, Audio.DriverSeqCounter, chip.Index, ym2610AdpcmA[chip.Index]);
                //            //else chipRegister.WriteYM2610_SetAdpcmA(chipID, model, (int)startAddress, (int)(bLen - 8), vgmBuf, (int)(vgmAdr + 15));
                //            //dumpData(chipRegister.YM2610[chipID], "YM2610_ADPCMA", vgmAdr + 15, bLen - 8);
                //        }
                //    }
                //    else if (bType == 0x83)
                //    {
                //        if (ym2610AdpcmB[chip.Index] == null || ym2610AdpcmB[chip.Index].Length != opnb_romSize) ym2610AdpcmB[chip.Index] = new byte[opnb_romSize];
                //        if (ym2610AdpcmB[chip.Index].Length > 0)
                //        {
                //            for (int cnt = 0; cnt < bLen - 8; cnt++)
                //            {
                //                ym2610AdpcmB[chip.Index][opnb_startAddress + cnt] = vgmBuf[vgmAdr + 15 + cnt].val;
                //            }
                //            //if (model == EnmModel.VirtualModel)
                //            chipRegister.YM2610WriteSetAdpcmB(od, Audio.DriverSeqCounter, chip.Index, ym2610AdpcmB[chip.Index]);
                //            //else chipRegister.WriteYM2610_SetAdpcmB(chipID, model, (int)startAddress, (int)(bLen - 8), vgmBuf, (int)(vgmAdr + 15));
                //            //dumpData(chipRegister.YM2610[chipID], "YM2610_ADPCMB", vgmAdr + 15, bLen - 8);
                //        }
                //    }
                //    vgmAdr += (uint)bLen + 7;
                //    break;
                //case Driver.ZGM.ZgmChip.YMF278B _:
                //    // YMF278B
                //    //chipRegister.writeYMF278BPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //    pcmDat.Clear();
                //    uint ymf278B_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint ymf278B_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //    chipRegister.writeYMF278BPCMData((byte)chip.Index, ymf278B_romSize, ymf278B_startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //    //dumpData(model, "YMF278B_PCMData", vgmAdr + 15, bLen - 8);
                //    break;

                ////        case 0x85:
                ////            // YMF271
                ////            //chipRegister.writeYMF271PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeYMF271PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "YMF271_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                ////        case 0x86:
                ////            // YMZ280B
                ////            //chipRegister.writeYMZ280BPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeYMZ280BPCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "YMZ280B_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                ////        case 0x88:
                ////            // Y8950
                ////            //chipRegister.writeY8950PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeY8950PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "Y8950_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                ////        case 0x89:
                ////            // MultiPCM
                ////            //chipRegister.writeMultiPCMPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeMultiPCMPCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "MultiPCM_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                ////        case 0x8b:
                ////            // OKIM6295
                ////            //chipRegister.writeOKIM6295PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeOKIM6295PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "OKIM6295_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                ////        case 0x8c:
                ////            // K054539
                ////            //chipRegister.writeK054539PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeK054539PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "K054539_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                //case Driver.ZGM.ZgmChip.C140 _:
                //    uint c140_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint c140_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.C140WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, c140_romSize, c140_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    break;

                //case Driver.ZGM.ZgmChip.C352 _:
                //    uint c352_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint c352_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.C352WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, c352_romSize, c352_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    break;

                //case Driver.ZGM.ZgmChip.K053260 _:
                //    uint K053260_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint K053260_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.K053260WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, K053260_romSize, K053260_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    break;

                //case Driver.ZGM.ZgmChip.QSound _:
                //    uint QSound_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint QSound_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.QSoundWritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, QSound_romSize, QSound_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    //            // QSound
                //    //            //chipRegister.writeQSoundPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //    //            pcmDat.Clear();
                //    //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //    //            chipRegister.QSoundWritePCMData(od, Audio.DriverSeqCounter, chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //    //            //dumpData(model, "QSound_PCMData", vgmAdr + 15, bLen - 8);
                //    break;

                //case Driver.ZGM.ZgmChip.RF5C164 _:
                //    //uint RF5C164_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint RF5C164_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.RF5C164WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, RF5C164_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    break;

                //case Driver.ZGM.ZgmChip.NES _:
                //    //uint RF5C164_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                //    uint NES_startAddress = Common.getLE32(vgmBuf, vgmAdr + 7);// 0x0B);
                //    pcmDat.Clear();
                //    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                //    chipRegister.NESWritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, NES_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                //    vgmAdr += (uint)bLen + 7;
                //    break;

                ////        case 0x92:
                ////            // C352
                ////            //chipRegister.writeC352PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeC352PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "C352_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;

                ////        case 0x93:
                ////            // GA20
                ////            //chipRegister.writeGA20PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                ////            pcmDat.Clear();
                ////            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                ////            chipRegister.writeGA20PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                ////            //dumpData(model, "GA20_PCMData", vgmAdr + 15, bLen - 8);
                ////            break;
                ////    }
                ////    vgmAdr += (uint)bLen + 7;
                ////    break;
                ////case 0xc0:
                ////    uint stAdr = Common.getLE16(vgmBuf, vgmAdr + 7);
                ////    uint dataSize = bLen - 2;
                ////    uint ROMData = vgmAdr + 9;
                ////    if ((bType & 0x20) != 0)
                ////    {
                ////        stAdr = Common.getLE32(vgmBuf, vgmAdr + 7);
                ////        dataSize = bLen - 4;
                ////        ROMData = vgmAdr + 11;
                ////    }

                ////    try
                ////    {
                ////        switch (bType)
                ////        {
                ////            case 0xc0:
                ////                //chipRegister.writeRF5C68PCMData(chipID, stAdr, dataSize, vgmBuf, vgmAdr + 9);
                ////                pcmDat.Clear();
                ////                for (uint i = vgmAdr + 9; i < vgmAdr + 9 + dataSize; i++) pcmDat.Add(vgmBuf[i].val);
                ////                chipRegister.writeRF5C68PCMData(chipID, stAdr, dataSize, pcmDat.ToArray(), 0);
                ////                //dumpData(model, "RF5C68_PCMData(8BitMonoSigned)", vgmAdr + 9, dataSize);
                ////                break;
                ////        }
                ////    }
                ////    catch (Exception e)
                ////    {
                ////        log.ForcedWrite(e);
                ////    }

                ////    vgmAdr += bLen + 7;
                ////    break;
                default:
                    vgmAdr += bLen + 7;
                    break;
            }

        }

        private void vcDummyChip(byte od, ref uint vgmAdr)
        {
            //chipRegister.writeDummyChipZGM(od, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            vgmAdr += 3;
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
