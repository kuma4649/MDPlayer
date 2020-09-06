using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class xgm : baseDriver
    {
        public const int FCC_XGM = 0x204d4758;	// "XGM "
        public const int FCC_GD3 = 0x20336447;  // "Gd3 "

        private class XGMSampleID
        {
            public uint addr = 0;
            public uint size = 0;
        }

        private XGMSampleID[] sampleID = new XGMSampleID[63];
        private uint sampleDataBlockSize = 0;
        private uint sampleDataBlockAddr = 0;
        private uint musicDataBlockSize = 0;
        private uint musicDataBlockAddr = 0;
        private byte versionInformation = 0;
        private byte dataInformation = 0;
        private bool isNTSC = false;
        private bool existGD3 = false;
        private bool multiTrackFile = false;
        private uint gd3InfoStartAddr = 0;


        public override bool init(byte[] xgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {

            this.vgmBuf = xgmBuf;
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

            if (!getXGMInfo(vgmBuf)) return false;

            if (model == EnmModel.RealModel)
            {
                chipRegister.setYM2612SyncWait(0, 1);
                chipRegister.setYM2612SyncWait(1, 1);
            }

            //Driverの初期化
            musicPtr = musicDataBlockAddr;
            xgmpcm= new XGMPCM[] { new XGMPCM(), new XGMPCM(), new XGMPCM(), new XGMPCM() };
            DACEnable = 0;

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

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            getXGMInfo(buf);
            return GD3;
        }

        private GD3 getGD3Info(byte[] vgmBuf)
        {

            if (!existGD3) return new GD3();

            GD3 GD3 = Common.getGD3Info(vgmBuf, gd3InfoStartAddr + 12);
            GD3.UsedChips = UsedChips;

            return GD3;

        }

        private bool getXGMInfo(byte[] vgmBuf)
        {
            if (vgmBuf == null) return false;

            try
            {
                if (Common.getLE32(vgmBuf, 0) != FCC_XGM) return false;

                for (uint i = 0; i < 63; i++)
                {
                    sampleID[i] = new XGMSampleID();
                    sampleID[i].addr = (Common.getLE16(vgmBuf, i * 4 + 4) * 256);
                    sampleID[i].size = (Common.getLE16(vgmBuf, i * 4 + 6) * 256);
                }

                sampleDataBlockSize = Common.getLE16(vgmBuf, 0x100);

                versionInformation = vgmBuf[0x102];

                dataInformation = vgmBuf[0x103];

                isNTSC = (dataInformation & 0x1) == 0;

                existGD3 = (dataInformation & 0x2) != 0;

                multiTrackFile = (dataInformation & 0x4) != 0;

                sampleDataBlockAddr = 0x104;

                musicDataBlockSize = Common.getLE32(vgmBuf, sampleDataBlockAddr + sampleDataBlockSize * 256);

                musicDataBlockAddr = sampleDataBlockAddr + sampleDataBlockSize * 256 + 4;

                gd3InfoStartAddr = musicDataBlockAddr + musicDataBlockSize;

                GD3 = getGD3Info(vgmBuf);

                if (musicDataBlockSize == 0)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Write(string.Format("XGMの情報取得中に例外発生 Message=[{0}] StackTrace=[{1}]", e.Message, e.StackTrace));
                return false;
            }

            return true;
        }

        public bool IsPlaying()
        {
            return true;
        }


        private double musicStep = Common.SampleRate / 60.0;
        private double pcmStep = Common.SampleRate / 14000.0;
        private double musicDownCounter = 0.0;
        private double pcmDownCounter = 0.0;
        private uint musicPtr = 0;
        private byte DACEnable = 0;

        private void oneFrameMain()
        {
            try
            {
                //if (model == EnmModel.RealModel) return;

                Counter++;
                vgmFrameCounter++;

                musicStep = Common.SampleRate / (isNTSC ? 60.0 : 50.0);

                if (musicDownCounter <= 0.0)
                {
                    //xgm処理
                    oneFrameXGM();
                    musicDownCounter += musicStep;
                }
                musicDownCounter -= 1.0;

                if (pcmDownCounter <= 0.0)
                {
                    //pcm処理
                    oneFramePCM();
                    pcmDownCounter += pcmStep;
                }
                pcmDownCounter -= 1.0;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        private void oneFrameXGM()
        {
            while (true)
            {

                byte cmd = vgmBuf[musicPtr++];

                //wait
                if (cmd == 0) break;

                //loop command
                if (cmd == 0x7e)
                {
                    musicPtr = musicDataBlockAddr + Common.getLE24(vgmBuf, musicPtr);
                    vgmCurLoop++;
                    continue;
                }

                //end command
                if (cmd == 0x7f)
                {
                    Stopped = true;
                    break;
                }

                byte X = (byte)(cmd & 0xf);
                cmd &= 0xf0;

                if (cmd == 0x10)
                {
                    //PSG register write:
                    WritePSG(X);
                }
                else if (cmd == 0x20)
                {
                    //YM2612 port 0 register write:
                    WriteYM2612P0(X);
                }
                else if (cmd == 0x30)
                {
                    //YM2612 port 1 register write:
                    WriteYM2612P1(X);
                }
                else if (cmd == 0x40)
                {
                    //YM2612 key off/on ($28) command write:
                    WriteYM2612Key(X);
                }
                else if (cmd == 0x50)
                {
                    //PCM play command:
                    PlayPCM(X);
                }

            }
        }

        private void WritePSG(byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                byte data = vgmBuf[musicPtr++];
                chipRegister.setSN76489Register(0, data, model);
            }
        }

        private void WriteYM2612P0(byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                byte adr = vgmBuf[musicPtr++];
                byte val = vgmBuf[musicPtr++];
                if (adr == 0x2b) DACEnable = (byte)(val & 0x80);
                chipRegister.setYM2612Register(0, 0, adr, val, model, vgmFrameCounter);
            }
        }

        private void WriteYM2612P1(byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                byte adr = vgmBuf[musicPtr++];
                byte val = vgmBuf[musicPtr++];
                chipRegister.setYM2612Register(0, 1, adr, val, model, vgmFrameCounter);
            }
        }

        private void WriteYM2612Key(byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                byte val = vgmBuf[musicPtr++];
                chipRegister.setYM2612Register(0, 0, 0x28, val, model, vgmFrameCounter);
            }
        }

        public class XGMPCM
        {
            public uint Priority = 0;
            public uint startAddr = 0;
            public uint endAddr = 0;
            public uint addr = 0;
            public uint inst = 0;
            public bool isPlaying = false;
            public byte data = 0;
        }

        public XGMPCM[] xgmpcm = null;

        private void PlayPCM(byte X)
        {
            byte priority = (byte)(X & 0xc);
            byte channel = (byte)(X & 0x3);
            byte id = vgmBuf[musicPtr++];

            //優先度が高い場合または消音中の場合のみ発音できる
            if (xgmpcm[channel].Priority <= priority || !xgmpcm[channel].isPlaying)
            {
                if (id == 0 || sampleID[id - 1].size == 0)
                {
                    //IDが0の場合や、定義されていないIDが指定された場合は発音を停止する
                    xgmpcm[channel].Priority = 0;
                    //xgmpcm[channel].startAddr = 0;
                    //xgmpcm[channel].endAddr = 0;
                    //xgmpcm[channel].addr = 0;
                    //xgmpcm[channel].inst = id;
                    xgmpcm[channel].isPlaying = false;
                }
                else
                {
                    xgmpcm[channel].Priority = priority;
                    xgmpcm[channel].startAddr = (uint)(sampleDataBlockAddr + sampleID[id - 1].addr);
                    xgmpcm[channel].endAddr = (uint)(sampleDataBlockAddr + sampleID[id - 1].addr + sampleID[id - 1].size);
                    xgmpcm[channel].addr = sampleDataBlockAddr + sampleID[id - 1].addr;
                    xgmpcm[channel].inst = id;
                    xgmpcm[channel].isPlaying = true;
                }
            }
        }

        private void oneFramePCM()
        {
            if (DACEnable == 0) return;

            short o = 0;

            for (int i = 0; i < 4; i++)
            {
                if (!xgmpcm[i].isPlaying) continue;
                sbyte d = (sbyte)vgmBuf[xgmpcm[i].addr++];
                o += (short)d;
                xgmpcm[i].data = (byte)Math.Abs((int)d);
                if (xgmpcm[i].addr >= xgmpcm[i].endAddr)
                {
                    xgmpcm[i].isPlaying = false;
                    xgmpcm[i].data = 0;
                }
            }
            o = Math.Min(Math.Max(o, (short)(sbyte.MinValue + 1)), (short)(sbyte.MaxValue));
            o += 0x80;

            chipRegister.setYM2612Register(0, 0, 0x2a, o, model, vgmFrameCounter);
        }

    }

}
