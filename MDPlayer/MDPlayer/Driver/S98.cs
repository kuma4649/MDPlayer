using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class S98 : baseDriver
    {
        public const int FCC_S98 = 0x00383953;	// "S98 "
        public const int FCC_BOM = 0x00BFBBEF;	// BOM

        public S98Info s98Info;
        private List<string> chips = null;
        private uint musicPtr = 0;
        private double oneSyncTime;
        private double musicStep = 44100.0 / 60.0;
        private double musicDownCounter = 0.0;
        private int s98WaitCounter;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (buf == null) return null;

            GD3 gd3 = new GD3();
            s98Info = new S98Info();
            chips = new List<string>();

            try
            {
                if (common.getLE24(buf, 0) != FCC_S98) return null;
                uint TAGAdr = common.getLE32(buf, 0x10);
                if (buf[TAGAdr++] != 0x5b) return null;
                if (buf[TAGAdr++] != 0x53) return null;
                if (buf[TAGAdr++] != 0x39) return null;
                if (buf[TAGAdr++] != 0x38) return null;
                if (buf[TAGAdr++] != 0x5d) return null;
                bool IsUTF8 = false;
                if (common.getLE24(buf, TAGAdr) == FCC_BOM)
                {
                    IsUTF8 = true;
                    TAGAdr += 3;
                }

                while (buf[TAGAdr] != 0x00)
                {
                    List<byte> strLst = new List<byte>();
                    string str;
                    while (buf[TAGAdr] != 0x0a && buf[TAGAdr] != 0x00)
                    {
                        strLst.Add(buf[TAGAdr++]);
                    }
                    if (IsUTF8)
                    {
                        str = Encoding.UTF8.GetString(strLst.ToArray());
                    }
                    else
                    {
                        str = Encoding.ASCII.GetString(strLst.ToArray());
                    }
                    TAGAdr++;

                    if (str.ToLower().IndexOf("artist=") != -1)
                    {
                        try
                        {
                            gd3.Composer = str.Substring(str.IndexOf("=") + 1);
                            gd3.ComposerJ = str.Substring(str.IndexOf("=") + 1);
                        }
                        catch
                        { }
                    }
                    if (str.ToLower().IndexOf("s98by=") != -1)
                    {
                        try
                        {
                            gd3.VGMBy = str.Substring(str.IndexOf("=") + 1);
                        }
                        catch
                        { }
                    }
                    if (str.ToLower().IndexOf("game=") != -1)
                    {
                        try
                        {
                            gd3.GameName = str.Substring(str.IndexOf("=") + 1);
                            gd3.GameNameJ = str.Substring(str.IndexOf("=") + 1);
                        }
                        catch
                        { }
                    }
                    if (str.ToLower().IndexOf("system=") != -1)
                    {
                        try
                        {
                            gd3.SystemName = str.Substring(str.IndexOf("=") + 1);
                            gd3.SystemNameJ = str.Substring(str.IndexOf("=") + 1);
                        }
                        catch
                        { }
                    }
                    if (str.ToLower().IndexOf("title=") != -1)
                    {
                        try
                        {
                            gd3.TrackName = str.Substring(str.IndexOf("=") + 1);
                            gd3.TrackNameJ = str.Substring(str.IndexOf("=") + 1);
                        }
                        catch
                        { }
                    }
                    if (str.ToLower().IndexOf("year=") != -1)
                    {
                        try
                        {
                            gd3.Converted = str.Substring(str.IndexOf("=") + 1);
                        }
                        catch
                        { }
                    }
                }

            }
            catch (Exception e)
            {
                log.Write(string.Format("S98のTAG情報取得中に例外発生 Message=[{0}] StackTrace=[{1}]", e.Message, e.StackTrace));
                return null;
            }

            return gd3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip useChip, uint latency)
        {
            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = 0;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            GD3 = getGD3Info(vgmBuf, 0);
            //if (GD3 == null) return false;

            if (!getInformationHeader()) return false;

            if (model == enmModel.RealModel)
            {
                chipRegister.setYM2612SyncWait(0, 1);
                chipRegister.setYM2612SyncWait(1, 1);
            }

            return true;
        }

        public override void oneFrameProc()
        {
            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0 && !Stopped)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
                //Stopped = !IsPlaying();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        private bool getInformationHeader()
        {
           
            s98Info.FormatVersion = (uint)(vgmBuf[3] - '0');
            s98Info.SyncNumerator = common.getLE32(vgmBuf, 4);
            if (s98Info.SyncNumerator == 0) s98Info.SyncNumerator = 10;
            s98Info.SyncDnumerator = common.getLE32(vgmBuf, 8);
            if (s98Info.SyncDnumerator == 0) s98Info.SyncDnumerator = 1000;
            s98Info.Compressing = common.getLE32(vgmBuf, 0xc);
            s98Info.TAGAddress = common.getLE32(vgmBuf, 0x10);
            s98Info.DumpAddress = common.getLE32(vgmBuf, 0x14);
            s98Info.LoopAddress = common.getLE32(vgmBuf, 0x18);
            s98Info.DeviceCount = common.getLE32(vgmBuf, 0x1c);

            byte[] devIDs = new byte[256];

            s98Info.DeviceInfos = new List<S98DevInfo>();
            if (s98Info.DeviceCount == 0)
            {
                S98DevInfo info = new S98DevInfo();
                info.ChipID = 0;
                info.DeviceType = 4;
                info.Clock = 7987200;
                info.Pan = 3;
                s98Info.DeviceInfos.Add(info);
                chips.Add("YM2608");
            }
            else
            {
                for (int i = 0; i < s98Info.DeviceCount; i++)
                {
                    S98DevInfo info = new S98DevInfo();
                    info.DeviceType = common.getLE32(vgmBuf, (uint)(0x20 + i * 0x10));
                    if (devIDs[info.DeviceType] > 1) continue;//同じchipは2こまで

                    info.Clock = common.getLE32(vgmBuf, (uint)(0x24 + i * 0x10));
                    info.Pan = common.getLE32(vgmBuf, (uint)(0x28 + i * 0x10));
                    switch (info.DeviceType)
                    {
                        case 1:
                            chips.Add("YM2149");
                            break;
                        case 2:
                            chips.Add("YM2203");
                            break;
                        case 3:
                            chips.Add("YM2612");
                            break;
                        case 4:
                            chips.Add("YM2608");
                            break;
                        case 5:
                            chips.Add("YM2151");
                            break;
                        case 6:
                            chips.Add("YM2413");
                            break;
                        case 7:
                            chips.Add("YM3526");
                            break;
                        case 8:
                            chips.Add("YM3812");
                            break;
                        case 9:
                            chips.Add("YMF262");
                            break;
                        case 15:
                            chips.Add("AY8910");
                            break;
                        case 16:
                            chips.Add("SN76489");
                            break;
                    }

                    info.ChipID = devIDs[info.DeviceType]++;
                    s98Info.DeviceInfos.Add(info);
                }
            }

            musicPtr = s98Info.DumpAddress;
            oneSyncTime = s98Info.SyncNumerator / (double)s98Info.SyncDnumerator;

            return true;
        }

        public class S98Info
        {
            public uint FormatVersion = 0; 
            public uint SyncNumerator = 0;
            public uint SyncDnumerator = 0;
            public uint Compressing = 0;
            public uint TAGAddress = 0;
            public uint DumpAddress = 0;
            public uint LoopAddress = 0;
            public uint DeviceCount = 0;
            public List<S98DevInfo> DeviceInfos = null;
        }

        public class S98DevInfo
        {
            public byte ChipID = 0;
            public uint DeviceType = 0;
            public uint Clock = 0;
            public uint Pan = 0;
        }

        private void oneFrameMain()
        {
            try
            {

                Counter++;
                vgmFrameCounter++;

                musicStep = 44100.0 * oneSyncTime;

                if (musicDownCounter <= 0.0)
                {
                    s98WaitCounter--;
                    if (s98WaitCounter <= 0) oneFrameS98();
                    musicDownCounter += musicStep;
                }
                musicDownCounter -= 1.0;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void oneFrameS98()
        {
            while (true)
            {

                byte cmd = vgmBuf[musicPtr++];

                //wait 1Sync
                if (cmd == 0xff)
                {
                    s98WaitCounter = 1;
                    break;
                }

                //wait nSync
                if (cmd == 0xfe)
                {
                    s98WaitCounter = getvv();
                    break;
                }

                //end/loop command
                if (cmd == 0xfd)
                {
                    if (s98Info.LoopAddress != 0)
                    {
                        musicPtr = s98Info.LoopAddress;
                        vgmCurLoop++;
                        continue;
                    }
                    else
                    {
                        Stopped = true;
                        break;
                    }
                }

                int devNo = cmd / 2;
                if (devNo >= s98Info.DeviceCount)
                {
                    musicPtr += 2;
                    continue;
                }

                byte devPort = (byte)(cmd % 2);

                switch (s98Info.DeviceInfos[devNo].DeviceType)
                {
                    case 2:
                        WriteYM2203(s98Info.DeviceInfos[devNo].ChipID, vgmBuf[musicPtr], vgmBuf[musicPtr + 1]);
                        break;
                    case 3:
                        WriteYM2612(s98Info.DeviceInfos[devNo].ChipID, devPort, vgmBuf[musicPtr], vgmBuf[musicPtr + 1]);
                        break;
                    case 4:
                        WriteYM2608(s98Info.DeviceInfos[devNo].ChipID, devPort, vgmBuf[musicPtr], vgmBuf[musicPtr + 1]);
                        break;
                    case 5:
                        WriteYM2151(s98Info.DeviceInfos[devNo].ChipID, devPort, vgmBuf[musicPtr], vgmBuf[musicPtr + 1]);
                        break;
                    case 6:
                        WriteYM2413(s98Info.DeviceInfos[devNo].ChipID, vgmBuf[musicPtr], vgmBuf[musicPtr + 1]);
                        break;
                    case 15:
                        WriteAY8910(s98Info.DeviceInfos[devNo].ChipID, vgmBuf[musicPtr], vgmBuf[musicPtr + 1]);
                        break;
                    case 16:
                        WriteSN76489(s98Info.DeviceInfos[devNo].ChipID, vgmBuf[musicPtr + 1]);
                        break;
                }
                musicPtr += 2;

            }
        }

        private int getvv()
        {
            int s = 0, n = 0;

            do
            {
                n |= (vgmBuf[musicPtr] & 0x7f) << s;
                s += 7;
            }
            while ((vgmBuf[musicPtr++] & 0x80) > 0);

            return n + 2;
        }

        private void WriteYM2203(int chipID, byte adr, byte data)
        {
            chipRegister.setYM2203Register(chipID, adr, data, model);
        }

        private void WriteYM2612(int chipID, byte port, byte adr, byte data)
        {
            chipRegister.setYM2612Register(chipID, port, adr, data, model, 0);
        }

        private void WriteYM2608(int chipID, byte port, byte adr, byte data)
        {
            chipRegister.setYM2608Register(chipID, port, adr, data, model);
        }

        private void WriteYM2151(int chipID, byte port, byte adr, byte data)
        {
            chipRegister.setYM2151Register(chipID, port, adr, data, model, YM2151Hosei[chipID], 0);
        }

        private void WriteYM2413(int chipID, byte adr, byte data)
        {
            chipRegister.setYM2413Register(chipID, adr, data, model);
        }

        private void WriteAY8910(int chipID, byte adr, byte data)
        {
            chipRegister.setAY8910Register(chipID, adr, data, model);
        }

        private void WriteSN76489(int chipID, byte data)
        {
            chipRegister.setSN76489Register(chipID, data, model);
        }

    }
}
