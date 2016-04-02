using System;
using System.Collections.Generic;
using SdlDotNet.Audio;
using System.Runtime.InteropServices;

namespace MDPlayer
{
    public class Audio
    {

        public static string vgmTrackName = "";
        public static string vgmTrackNameJ = "";
        public static string vgmGameName = "";
        public static string vgmGameNameJ = "";
        public static string vgmSystemName = "";
        public static string vgmSystemNameJ = "";
        public static string vgmComposer = "";
        public static string vgmComposerJ = "";
        public static string vgmConverted = "";
        public static string vgmVGMBy = "";
        public static string vgmNotes = "";
        public static string vgmVersion = "";
        public static string vgmUsedChips = "";

        private static List<string> chips = null;

        private static int SamplingRate = 44100;
        private static int PSGClockValue = 3579545;
        private static int FMClockValue = 7670454;

        private static int samplingBuffer = 1024;
        private static short[] frames = new short[samplingBuffer * 2];
        private static MDSound.MDSound mds = new MDSound.MDSound(SamplingRate, samplingBuffer, FMClockValue, PSGClockValue);

        private static AudioStream sdl;
        private static AudioCallback sdlCb = new AudioCallback(callback);
        private static IntPtr sdlCbPtr;
        private static GCHandle sdlCbHandle;

        private static byte[] vgmBuf = null;
        private static uint vgmPcmPtr;
        private static uint[] vgmPcmBaseAdr = new uint[0x100];
        private static int[] vgmPcmBaseLength = new int[0x100];
        private static uint vgmAdr;
        private static int vgmWait;
        private static double vgmSpeed;
        private static double vgmSpeedCounter;
        private static uint vgmEof;
        private static bool vgmAnalyze;
        private static vgmStream[] vgmStreams = new vgmStream[0x100];
        private static long vgmCounter = 0;
        private static long vgmTotalCounter = 0;
        private static long vgmLoopCounter = 0;
        private static long vgmDataOffset = 0;
        private static long vgmLoopOffset = 0;
        private static int vgmDataBlockCounter=0;


        public static void SetVGMBuffer(byte[] srcBuf)
        {
            Stop();
            vgmBuf = srcBuf;
        }

        public static bool Play()
        {

            try
            {

                if (vgmBuf == null) return false;

                Stop();

                chips = new List<string>();

                //ヘッダーを読み込めるサイズをもっているかチェック
                if (vgmBuf.Length < 0x40) return false;

                //ヘッダーから情報取得

                uint vgm = getLE32(0x00);
                if (vgm != 0x206d6756) return false;

                vgmEof = getLE32(0x04);

                uint version = getLE32(0x08);
                if (version < 0x0150) return false;
                vgmVersion = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint SN76489clock = getLE32(0x0c);
                if (SN76489clock != 0) chips.Add("SN76489");

                uint YM2413clock = getLE32(0x10);
                if (YM2413clock != 0) chips.Add("YM2413");

                uint vgmGd3 = getLE32(0x14);
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = getLE32(vgmGd3 + 0x14);
                    if (vgmGd3Id != 0x20336447) return false;
                    getGD3Info(vgmGd3);
                }

                vgmTotalCounter = getLE32(0x18);
                if (vgmTotalCounter <= 0) return false;

                vgmLoopOffset = getLE32(0x1c);

                vgmLoopCounter = getLE32(0x20);

                uint YM2612clock = getLE32(0x2c);
                if (YM2612clock != 0) chips.Add("YM2612");

                uint YM2151clock = getLE32(0x30);
                if (YM2151clock != 0) chips.Add("YM2151");

                vgmDataOffset = getLE32(0x34);
                if (vgmDataOffset == 0)
                {
                    vgmDataOffset = 0x40;
                }
                else
                {
                    vgmDataOffset += 0x34;
                }

                if (version >= 0x0151)
                {
                    uint SegaPCMclock = getLE32(0x38);
                    if (SegaPCMclock != 0) chips.Add("Sega PCM");

                    uint RF5C68clock = getLE32(0x40);
                    if (RF5C68clock != 0) chips.Add("RF5C68");

                    uint YM2203clock = getLE32(0x44);
                    if (YM2203clock != 0) chips.Add("YM2203");

                    uint YM2608clock = getLE32(0x48);
                    if (YM2608clock != 0) chips.Add("YM2608");

                    uint YM2610Bclock = getLE32(0x4c);
                    if (YM2610Bclock != 0) chips.Add("YM2610/B");

                    uint YM3812clock = getLE32(0x50);
                    if (YM3812clock != 0) chips.Add("YM3812");

                    uint YM3526clock = getLE32(0x54);
                    if (YM3526clock != 0) chips.Add("YM3526");

                    uint Y8950clock = getLE32(0x58);
                    if (Y8950clock != 0) chips.Add("Y8950");

                    uint YMF262clock = getLE32(0x5c);
                    if (YMF262clock != 0) chips.Add("YMF262");

                    uint YMF278Bclock = getLE32(0x60);
                    if (YMF278Bclock != 0) chips.Add("YMF278B");

                    uint YMF271clock = getLE32(0x64);
                    if (YMF271clock != 0) chips.Add("YMF271");

                    uint YMZ280Bclock = getLE32(0x68);
                    if (YMZ280Bclock != 0) chips.Add("YMZ280B");

                    uint RF5C164clock = getLE32(0x6c);
                    if (RF5C164clock != 0) chips.Add("RF5C164");

                    uint PWMclock = getLE32(0x70);
                    if (PWMclock != 0) chips.Add("PWM");

                    uint AY8910clock = getLE32(0x74);
                    if (AY8910clock != 0) chips.Add("AY8910");

                }

                vgmUsedChips = "";
                foreach (string chip in chips)
                {
                    vgmUsedChips += chip + " , ";
                }
                if (vgmUsedChips.Length > 2)
                {
                    vgmUsedChips = vgmUsedChips.Substring(0, vgmUsedChips.Length - 3);
                }

                vgmAdr = (uint)vgmDataOffset;
                vgmWait = 0;
                vgmAnalyze = true;
                vgmCounter = 0;
                vgmSpeed = 1;
                vgmSpeedCounter = 0;
                vgmDataBlockCounter = 0;
                for (int i = 0; i < 0x100; i++)
                {
                    vgmStreams[i].blockId = 0;
                    vgmStreams[i].chipId = 0;
                    vgmStreams[i].cmd = 0;
                    vgmStreams[i].databankId = 0;
                    vgmStreams[i].dataLength = 0;
                    vgmStreams[i].dataStartOffset = 0;
                    vgmStreams[i].frequency = 0;
                    vgmStreams[i].lengthMode = 0;
                    vgmStreams[i].port = 0;
                    vgmStreams[i].stepbase = 0;
                    vgmStreams[i].stepsize = 0;
                    vgmStreams[i].sw = false;
                    vgmStreams[i].wkDataAdr = 0;
                    vgmStreams[i].wkDataLen = 0;
                    vgmStreams[i].wkDataStep = 0;
                }

                mds.Init(SamplingRate, samplingBuffer, FMClockValue, PSGClockValue);

                sdlCbHandle = GCHandle.Alloc(sdlCb);
                sdlCbPtr = Marshal.GetFunctionPointerForDelegate(sdlCb);
                sdl = new AudioStream(SamplingRate, AudioFormat.Signed16Little, SoundChannel.Stereo, (short)samplingBuffer, sdlCb, null);
                sdl.Paused = false;

                return true;
            }
            catch
            {
                return false;
            }

        }

        public static void FF()
        {
            vgmSpeed = (vgmSpeed == 1) ? 4 : 1;
        }

        public static void Pause()
        {
            if (sdl == null) return;

            try
            {
                    sdl.Paused = !sdl.Paused;
            }
            catch
            {
            }

        }

        public static void Stop()
        {

            try
            {
                if (sdl != null)
                {
                        sdl.Paused = true;
                        sdl.Close();
                        sdl.Dispose();
                        sdl = null;
                }
            }
            catch
            {
            }

            try
            {
                if (sdlCbHandle.IsAllocated) sdlCbHandle.Free();
            }
            catch
            {
            }

        }

        public static int[][] GetFMRegister()
        {
            return mds.ReadFMRegister();
        }

        public static int[] GetPSGRegister()
        {
            return mds.ReadPSGRegister();
        }

        public static long GetCounter()
        {
            return vgmCounter;
        }

        public static long GetTotalCounter()
        {
            return vgmTotalCounter;
        }

        public static long GetLoopCounter()
        {
            return vgmLoopCounter;
        }

        public static int[][] GetFMVolume()
        {
            return mds.ReadFMVolume();
        }

        public static int[] GetFMCh3SlotVolume()
        {
            return mds.ReadFMCh3SlotVolume();
        }

        public static int[][] GetPSGVolume()
        {
            return mds.ReadPSGVolume();
        }

        public static int[] GetFMKeyOn()
        {
            return mds.ReadFMKeyOn();
        }

        public static void setFMMask(int ch)
        {
            mds.setFMMask(1 << ch);
        }

        public static void setPSGMask(int ch)
        {
            mds.setPSGMask(1 << ch);
        }

        public static void resetFMMask(int ch)
        {
            mds.resetFMMask(1 << ch);
        }

        public static void resetPSGMask(int ch)
        {
            mds.resetPSGMask(1 << ch);
        }



        internal static void callback(IntPtr userData, IntPtr stream, int len)
        {
            int i;

            int[][] buf = mds.Update2(oneFrameVGMWithSpeedControl);

            for (i = 0; i < len / 4; i++)
            {
                frames[i * 2 + 0] = (short)buf[0][i];
                frames[i * 2 + 1] = (short)buf[1][i];
            }

            Marshal.Copy(frames, 0, stream, len / 2);
        }

        private static void oneFrameVGMWithSpeedControl()
        {
            try {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;
                    oneFrameVGM();
                }
            }
            catch
            {

            }
        }

        private static void oneFrameVGM()
        {
            if (sdl==null || sdl.Paused)
            {
                return;
            }

            if (vgmWait > 0)
            {
                oneFrameVGMStream();
                vgmWait--;
                vgmCounter++;
                return;
            }

            if (!vgmAnalyze)
            {
                Stop();
                return;
            }

            while (vgmWait <= 0)
            {
                if (vgmAdr == vgmBuf.Length || vgmAdr == vgmEof)
                {
                    if (vgmLoopCounter != 0)
                    {
                        //vgmAdr = (uint)(vgmDataOffset + vgmLoopOffset);
                        vgmAdr = (uint)(vgmLoopOffset + 0x1c);
                        vgmPcmPtr = 0;
                        vgmCounter = 0;
                    }
                    else
                    {
                        vgmAnalyze = false;
                        return;
                    }
                }

                byte cmd = vgmBuf[vgmAdr];
                if (vgmCmdTbl[cmd] != null)
                {
                    vgmCmdTbl[cmd]();
                }
                else
                {
                    //わからんコマンド
                    Console.WriteLine("{0:X}", vgmBuf[vgmAdr++]);
                }
            }

            oneFrameVGMStream();
            vgmWait--;
            vgmCounter++;

        }

        private static Action[] vgmCmdTbl = new Action[0x100] {
            null,null,null,null,null,null,null,null, //0x00
            null,null,null,null,null,null,null,null, //0x08
            null,null,null,null,null,null,null,null, //0x10
            null,null,null,null,null,null,null,null, //0x18
            null,null,null,null,null,null,null,null, //0x20
            null,null,null,null,null,null,null,null, //0x28
            vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope, //0x30
            vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope,vcDummy1Ope, //0x38
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0x40
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0x48
            vcGGPSGPort06, //0x4F
            vcPSG, //0x50
            vcDummy2Ope, //0x51
            vcYM2612Port0, //0x52
            vcYM2612Port1, //0x53
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0x54
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0x58
            null,//0x60
            vcWaitNSamples,//0x61
            vcWait735Samples,//0x62
            vcWait882Samples,//0x63
            vcOverrideLength,//0x64
            null,//0x65
            vcEndOfSoundData,//0x66
            vcDataBlock, //0x67
            vcPCMRamWrite,//0x68

            null,null,null,null,null,null,null, //0x69

            vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples, //0x70
            vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples, //0x74
            vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples, //0x78
            vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples,vcWaitN1Samples, //0x7C

            vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a, //0x80
            vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a, //0x84
            vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a, //0x88
            vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a,vcWaitNSamplesAndSendYM26120x2a, //0x8C

            vcSetupStreamControl,//0x90
            vcSetStreamData,//0x91
            vcSetStreamFrequency,//0x92
            vcStartStream,//0x93
            vcStopStream,//0x94
            vcStartStreamFastCall,//0x95
            null,null, //0x96
            null,null,null,null,null,null,null,null, //0x98
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0xA0
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0xA8
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0xB0
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0xB8
            vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope, //0xC0
            vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope, //0xC8
            vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope, //0xD0
            vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope, //0xD8
            vcSeekToOffsetInPCMDataBank,//0xE0
            vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope, //0xE1
            vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope, //0xE8
            vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope, //0xF0
            vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope,vcDummy4Ope //0xF8

        };

        private static void vcGGPSGPort06()
        {
            vgmAdr += 2;
        }

        private static void vcPSG()
        {
            mds.WritePSG(vgmBuf[vgmAdr + 1]);
            vgmAdr += 2;
        }

        private static void vcDummy1Ope()
        {
            vgmAdr += 2;
        }

        private static void vcDummy2Ope()
        {
            vgmAdr += 3;
        }

        private static void vcDummy3Ope()
        {
            vgmAdr += 4;
        }

        private static void vcDummy4Ope()
        {
            vgmAdr += 5;
        }

        private static void vcYM2612Port0()
        {
            mds.WriteFM(0, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            vgmAdr += 3;
        }

        private static void vcYM2612Port1()
        {
            mds.WriteFM(1, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            vgmAdr += 3;
        }

        private static void vcWaitNSamples()
        {
            vgmWait += (int)getLE16(vgmAdr + 1);
            vgmAdr += 3;
        }

        private static void vcWait735Samples()
        {
            vgmWait += 735;
            vgmAdr++;
        }

        private static void vcWait882Samples()
        {
            vgmWait += 882;
            vgmAdr++;
        }

        private static void vcOverrideLength()
        {
            vgmAdr += 4;
        }

        private static void vcEndOfSoundData()
        {
            vgmAdr = (uint)vgmBuf.Length;
        }

        private static void vcDataBlock()
        {
            uint bAdr = vgmAdr + 7;
            int bLen = (int)getLE32(vgmAdr + 3);
            vgmAdr += (uint)bLen + 7;

            if (vgmDataBlockCounter < vgmPcmBaseAdr.Length)
            {
                vgmPcmBaseAdr[vgmDataBlockCounter] = bAdr;
                vgmPcmBaseLength[vgmDataBlockCounter] = (int)bLen;
                vgmDataBlockCounter++;
            }
        }

        private static void vcPCMRamWrite()
        {
            vgmAdr += 12;
        }

        private static void vcWaitN1Samples()
        {
            vgmWait += (int)(vgmBuf[vgmAdr] - 0x6f);
            vgmAdr++;
        }

        private static void vcWaitNSamplesAndSendYM26120x2a()
        {
            mds.WriteFM(0, 0x2a, vgmBuf[vgmPcmPtr++]);
            vgmWait += (int)(vgmBuf[vgmAdr] - 0x80);
            vgmAdr++;
        }

        private static void vcSetupStreamControl() {
            vgmAdr++;
            byte si = vgmBuf[vgmAdr++];
            vgmStreams[si].chipId = vgmBuf[vgmAdr++];
            vgmStreams[si].port = vgmBuf[vgmAdr++];
            vgmStreams[si].cmd = vgmBuf[vgmAdr++];
        }

        private static void vcSetStreamData() {
            vgmAdr++;
            byte si = vgmBuf[vgmAdr++];
            vgmStreams[si].databankId = vgmBuf[vgmAdr++];
            vgmStreams[si].stepsize = vgmBuf[vgmAdr++];
            vgmStreams[si].stepbase = vgmBuf[vgmAdr++];
        }

        private static void vcSetStreamFrequency() {
            vgmAdr++;
            byte si = vgmBuf[vgmAdr++];
            vgmStreams[si].frequency = getLE32(vgmAdr);
            vgmAdr += 4;
        }

        private static void vcStartStream() {
            vgmAdr++;
            byte si = vgmBuf[vgmAdr++];
            vgmStreams[si].dataStartOffset = getLE32(vgmAdr);
            vgmAdr += 4;
            vgmStreams[si].lengthMode = vgmBuf[vgmAdr++];//用途がいまいちわかってません
            vgmStreams[si].dataLength = getLE32(vgmAdr);
            vgmAdr += 4;

            vgmStreams[si].sw = true;
            vgmStreams[si].wkDataAdr = vgmStreams[si].dataStartOffset;
            vgmStreams[si].wkDataLen = (int)vgmStreams[si].dataLength;
            vgmStreams[si].wkDataStep = 1.0;

        }

        private static void vcStopStream() {
            vgmAdr++;
            byte si = vgmBuf[vgmAdr++];
            vgmStreams[si].sw = false;
        }

        private static void vcStartStreamFastCall()
        {
            //使い方がいまいちわかってません
            vgmAdr++;
            byte si = vgmBuf[vgmAdr++];
            vgmStreams[si].blockId = getLE16(vgmAdr);
            vgmAdr += 2;
            byte p = vgmBuf[vgmAdr++];
            if ((p & 1) > 0)
            {
                vgmStreams[si].lengthMode |= 0x80;
            }
            if ((p & 16) > 0)
            {
                vgmStreams[si].lengthMode |= 0x10;
            }

            vgmStreams[si].sw = true;
            vgmStreams[si].wkDataAdr = 0;// vgmStreams[si].dataStartOffset;
            vgmStreams[si].wkDataLen = vgmPcmBaseLength[vgmStreams[si].blockId];// (int)vgmStreams[si].dataLength;
            vgmStreams[si].wkDataStep = 1.0;

        }

        private static void vcSeekToOffsetInPCMDataBank()
        {
            vgmPcmPtr = getLE32(vgmAdr + 1) + vgmPcmBaseAdr[0];
            vgmAdr += 5;
        }

        private static void oneFrameVGMStream()
        {
            for (int i = 0; i < 0x100; i++)
            {

                if (!vgmStreams[i].sw) continue;
                if (vgmStreams[i].chipId != 0x02) continue;//とりあえずYM2612のみ

                while (vgmStreams[i].wkDataStep >= 1.0)
                {
                    mds.WriteFM(vgmStreams[i].port, vgmStreams[i].cmd, vgmBuf[vgmPcmBaseAdr[vgmStreams[i].blockId] + vgmStreams[i].wkDataAdr]);
                    vgmStreams[i].wkDataAdr++;
                    vgmStreams[i].wkDataLen--;
                    vgmStreams[i].wkDataStep -= 1.0;
                }
                vgmStreams[i].wkDataStep += (double)vgmStreams[i].frequency / (double)SamplingRate;

                if (vgmStreams[i].wkDataLen <= 0)
                {
                    vgmStreams[i].sw = false;
                }

            }
        }


        private struct vgmStream
        {

            public byte chipId;
            public byte port;
            public byte cmd;

            public byte databankId;
            public byte stepsize;
            public byte stepbase;

            public uint frequency;

            public uint dataStartOffset;
            public byte lengthMode;
            public uint dataLength;

            public bool sw;

            public uint blockId;

            public uint wkDataAdr;
            public int wkDataLen;
            public double wkDataStep;
        }

        private static UInt32 getLE16(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100;

            return dat;
        }

        private static UInt32 getLE32(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100 + (UInt32)vgmBuf[adr + 2] * 0x10000 + (UInt32)vgmBuf[adr + 3] * 0x1000000;

            return dat;
        }

        private static void getGD3Info(uint vgmGd3)
        {
            uint adr = vgmGd3 + 12 + 0x14;

            vgmTrackName = "";
            vgmTrackNameJ = "";
            vgmGameName = "";
            vgmGameNameJ = "";
            vgmSystemName = "";
            vgmSystemNameJ = "";
            vgmComposer = "";
            vgmComposerJ = "";
            vgmConverted = "";
            vgmVGMBy = "";
            vgmNotes = "";

            try
            {
                //trackName
                vgmTrackName = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //trackNameJ
                vgmTrackNameJ = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //gameName
                vgmGameName = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //gameNameJ
                vgmGameNameJ = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //systemName
                vgmSystemName = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //systemNameJ
                vgmSystemNameJ = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //Composer
                vgmComposer = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //ComposerJ
                vgmComposerJ = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //Converted
                vgmConverted = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //VGMBy
                vgmVGMBy = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
                //Notes
                vgmNotes = System.Text.Encoding.Unicode.GetString(getByteArray(ref adr));
            }
            catch { }
        }

        private static byte[] getByteArray(ref uint adr)
        {
            List<byte> ary = new List<byte>();
            while (vgmBuf[adr] != 0 || vgmBuf[adr + 1] != 0)
            {
                ary.Add(vgmBuf[adr]);
                adr++;
                ary.Add(vgmBuf[adr]);
                adr++;
            }
            adr += 2;

            return ary.ToArray();
        }

    }
}
