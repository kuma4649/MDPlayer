using System;
using System.Collections.Generic;
using NScci;
using System.Threading;
using System.Diagnostics;

namespace MDPlayer
{
    public class Audio
    {
        public const int FCC_VGM = 0x206D6756;	// "Vgm "
        public const int FCC_GD3 = 0x20336447;  // "Gd3 "

        public static GD3 GD3 = new GD3();
        public static string vgmVersion = "";
        public static string vgmUsedChips = "";
        private static object lockObj = new object();
        private static bool _fatalError = false;
        public static bool fatalError
        {
            get
            {
                lock(lockObj)
                {
                    return _fatalError;
                }
            }

            set
            {
                lock(lockObj)
                {
                    _fatalError = value;
                }
            }
        }

        public enum enmChipType : int
        {
            YM2612 = 5
            , SN76489 = 7
        }

        private static List<string> chips = null;

        private static uint SamplingRate = 44100;
        //private static uint SamplingRate = 48000;
        private static uint PSGClockValue = 3579545;
        private static uint FMClockValue = 7670454;
        private static uint rf5c164ClockValue = 12500000;
        private static uint pwmClockValue = 23011361;

        private static uint samplingBuffer = 1024;
        private static MDSound.MDSound mds = new MDSound.MDSound(SamplingRate, samplingBuffer, FMClockValue, PSGClockValue, rf5c164ClockValue, pwmClockValue);

        private static NAudioWrap naudioWrap;

        private static NScci.NScci nscci;
        private static NSoundChip scYM2612 = null;
        private static NSoundChip scSN76489 = null;

        private static Thread trdMain = null;
        private static bool trdClosed = false;
        private static Stopwatch sw = Stopwatch.StartNew();
        private static double swFreq = Stopwatch.Frequency;

        private static byte[] vgmBuf = null;

        private static uint vgmAdr;
        private static int vgmWait;
        private static double vgmSpeed;
        private static bool vgmFadeout;
        private static double vgmFadeoutCounter;
        private static double vgmFadeoutCounterV;
        private static double vgmSpeedCounter;
        private static uint vgmEof;
        private static bool vgmAnalyze;

        private const int PCM_BANK_COUNT = 0x40;
        private static VGM_PCM_BANK[] PCMBank=new VGM_PCM_BANK[PCM_BANK_COUNT];
        private static dacControl dacControl = new dacControl();
        private static PCMBANK_TBL PCMTbl=new PCMBANK_TBL();
        private static byte DacCtrlUsed;
        private static byte[] DacCtrlUsg= new byte[0xFF];
        private static DACCTRL_DATA[] DacCtrl=new DACCTRL_DATA[0xFF];
        private static uint VGMCurLoop=0;

        private static long vgmCounter = 0;
        private static long vgmTotalCounter = 0;
        private static long vgmLoopCounter = 0;
        private static long vgmDataOffset = 0;
        private static long vgmLoopOffset = 0;

        private static bool Paused = false;
        private static bool Stopped = false;

        private static Setting setting = null;

        public static void Init(Setting setting)
        {

            dacControl.mds = mds;
            naudioWrap = new NAudioWrap((int)SamplingRate, naudioCb);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;
            Audio.setting = setting.Copy();

            mds.Init(SamplingRate, samplingBuffer / 2
                , FMClockValue
                , PSGClockValue
                , (rf5c164ClockValue & 0x80000000) + (uint)((rf5c164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                , (uint)(pwmClockValue * (SamplingRate / (23011361.0 / 384)))
            );

            nscci = new NScci.NScci();
            scYM2612 = getChip(Audio.setting.YM2612Type.SoundLocation, Audio.setting.YM2612Type.BusID, Audio.setting.YM2612Type.SoundChip);
            if (scYM2612 != null)
            {
                scYM2612.init();
                dacControl.scYM2612 = scYM2612;
            }
            scSN76489 = getChip(Audio.setting.SN76489Type.SoundLocation, Audio.setting.SN76489Type.BusID, Audio.setting.SN76489Type.SoundChip);
            if (scSN76489 != null)
            {
                scSN76489.init();
                dacControl.scSN76489 = scSN76489;
            }

            Paused = false;
            Stopped = true;
            fatalError = false;

            naudioWrap.Start(Audio.setting);

        }

        private static void ThreadFunction()
        {
            double o = sw.ElapsedTicks / swFreq;
            double step = 1 / (double)SamplingRate;
            while (!trdClosed)
            {
                Thread.Sleep(0);
                if (Stopped || Paused) continue;

                double el1 = sw.ElapsedTicks / swFreq;
                if (el1 - o < step) continue;

                o = el1 - ((el1 - o) - step);

                if (scYM2612 != null) oneFrameVGMWithSpeedControl();
            }
        }

        private static NScci.NSoundChip getChip(int SoundLocation,int BusID,int SoundChip)
        {
            int ifc = nscci.getInterfaceCount();
            for (int i = 0; i < ifc; i++)
            {
                NSoundInterface sif = nscci.getInterface(i);
                int scc = sif.getSoundChipCount();
                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    if (info.getdSoundLocation() == SoundLocation && info.getdBusID() == BusID && info.getiSoundChip() == SoundChip)
                    {
                        return sc;
                    }
                }
            }

            return null;
        }

        private static List<NScci.NSoundChip> getChipList(int chipType)
        {
            List<NScci.NSoundChip> ret = new List<NSoundChip>();

            int ifc = nscci.getInterfaceCount();
            for (int i = 0; i < ifc; i++)
            {
                NSoundInterface sif = nscci.getInterface(i);
                int scc = sif.getSoundChipCount();
                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    int t = sc.getSoundChipType();
                    if (t == chipType)
                    {
                        ret.Add(sc);
                    }
                }
            }

            return ret;
        }

        public static List<NScci.NSoundChip> getYM2612ChipList()
        {
            return getChipList((int)enmChipType.YM2612);
        }

        public static List<NScci.NSoundChip> getSN76489ChipList()
        {
            return getChipList((int)enmChipType.SN76489);
        }

        private static void NaudioWrap_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                System.Windows.Forms.MessageBox.Show(
                    string.Format("デバイスが何らかの原因で停止しました。\r\nメッセージ:\r\n{0}", e.Exception.Message)
                    , "エラー"
                    , System.Windows.Forms.MessageBoxButtons.OK
                    , System.Windows.Forms.MessageBoxIcon.Error);
                try
                {
                    naudioWrap.Stop();
                }
                catch
                {
                }
            }
            else
            {
                Stop();
            }
        }

        public static void SetVGMBuffer(byte[] srcBuf)
        {
            Stop();
            vgmBuf = srcBuf;
        }

        public static bool Play(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                Stop();

                chips = new List<string>();

                //ヘッダーを読み込めるサイズをもっているかチェック
                if (vgmBuf.Length < 0x40) return false;

                //ヘッダーから情報取得

                uint vgm = getLE32(0x00);
                if (vgm != FCC_VGM) return false;

                vgmEof = getLE32(0x04);

                uint version = getLE32(0x08);
                //バージョンチェック
                if (version < 0x0110) return false;
                vgmVersion = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

                uint SN76489clock = getLE32(0x0c);
                if (SN76489clock != 0) chips.Add("SN76489");

                uint YM2413clock = getLE32(0x10);
                if (YM2413clock != 0) chips.Add("YM2413");

                uint vgmGd3 = getLE32(0x14);
                if (vgmGd3 != 0)
                {
                    uint vgmGd3Id = getLE32(vgmGd3 + 0x14);
                    if (vgmGd3Id != FCC_GD3) return false;
                    GD3.getGD3Info(vgmBuf, vgmGd3);
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
                    rf5c164ClockValue = RF5C164clock;

                    uint PWMclock = getLE32(0x70);
                    if (PWMclock != 0) chips.Add("PWM");
                    pwmClockValue = PWMclock;

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
                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;
                vgmSpeedCounter = 0;
                for (int i = 0; i < PCM_BANK_COUNT; i++) PCMBank[i] = new VGM_PCM_BANK();
                dacControl.refresh();
                DacCtrlUsed = 0x00;
                for (byte CurChip = 0x00; CurChip < 0xFF; CurChip++)
                {
                    DacCtrl[CurChip] = new DACCTRL_DATA();
                    DacCtrl[CurChip].Enable = false;
                }


                trdClosed = false;
                trdMain = new Thread(new ThreadStart(ThreadFunction));
                trdMain.Priority = ThreadPriority.Highest;
                trdMain.IsBackground = true;
                trdMain.Start();

                mds.Init(SamplingRate, samplingBuffer / 2
                    , FMClockValue
                    , PSGClockValue
                    , (rf5c164ClockValue & 0x80000000) + (uint)((rf5c164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                    , (uint)(pwmClockValue * (SamplingRate / (23011361.0 / 384)))
                    );

                Paused = false;
                Stopped = false;

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

        public static void Slow()
        {
                vgmSpeed = (vgmSpeed == 1) ? 0.25 : 1;
        }

        public static void Pause()
        {

            try
            {
                    Paused = !Paused;
            }
            catch
            {
            }

        }

        public static void Fadeout()
        {
                vgmFadeout = true;
        }

        public static void Stop()
        {

            try
            {

                if (Stopped)
                {
                    trdClosed = true;
                    return;
                }

                if (!Paused)
                {
                    NAudio.Wave.PlaybackState? ps = naudioWrap.GetPlaybackState();
                    if (ps != null && ps != NAudio.Wave.PlaybackState.Stopped)
                    {
                        vgmFadeoutCounterV = 0.1;
                        vgmFadeout = true;
                        int cnt = 0;
                        while (!Stopped && cnt < 100)
                        {
                            System.Threading.Thread.Sleep(1);
                            System.Windows.Forms.Application.DoEvents();
                            cnt++;
                        }
                    }
                }
                trdClosed = true;
                scYM2612.init();
                //naudioWrap.Stop();
            }
            catch
            {
            }

        }

        public static void Close()
        {
            try
            {
                Stop();
                naudioWrap.Stop();
                nscci.Dispose();
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

        public static MDSound.scd_pcm.pcm_chip_ GetRf5c164Register()
        {
            return mds.ReadRf5c164Register();
        }

        public static int[][] GetRf5c164Volume()
        {
            return mds.ReadRf5c164Volume();
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

        internal static int naudioCb(short[] buffer, int offset, int sampleCount)
        {
            try
            {
                int i;

                if (Stopped || Paused)
                {

                    return mds.Update2(buffer, offset, sampleCount, null);

                }

                int cnt;
                if(scYM2612== null) cnt = mds.Update2(buffer, offset, sampleCount, oneFrameVGMWithSpeedControl);
                else cnt = mds.Update2(buffer, offset, sampleCount, null);

                if (vgmFadeout)
                {

                    for (i = 0; i < sampleCount; i++)
                    {
                        buffer[offset + i] = (short)(buffer[offset + i] * vgmFadeoutCounter);


                        vgmFadeoutCounter -= vgmFadeoutCounterV;
                        //vgmFadeoutCounterV += 0.00001;
                        if (vgmFadeoutCounterV >= 0.004 && vgmFadeoutCounterV != 0.1)
                        {
                            vgmFadeoutCounterV = 0.004;
                        }

                        if (vgmFadeoutCounter < 0.0)
                        {
                            vgmFadeoutCounter = 0.0;
                        }
                    }
                }

                if (vgmFadeoutCounter == 0.0)
                {
                    mds.Init(SamplingRate, samplingBuffer / 2
                        , FMClockValue
                        , PSGClockValue
                        , (rf5c164ClockValue & 0x80000000) + (uint)((rf5c164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                        , (uint)(pwmClockValue * (SamplingRate / (23011361.0 / 384)))
                    );
                    Stopped = true;
                }

                return cnt;

            }
            catch
            {
                fatalError = true;
                Stopped = true;
            }
            return -1;
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
            if (vgmWait > 0)
            {
                oneFrameVGMStream();
                vgmWait--;
                vgmCounter++;
                return;
            }

            if (!vgmAnalyze)
            {
                Stopped=true;
                return;
            }

            while (vgmWait <= 0)
            {
                if (vgmAdr == vgmBuf.Length || vgmAdr == vgmEof)
                {
                    if (vgmLoopCounter != 0)
                    {
                        vgmAdr = (uint)(vgmLoopOffset + 0x1c);
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
            vcDummy2Ope, //0xB0
            vcRf5c164, //0xB1
            vcPWM, //0xB2
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0xB3
            vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope,vcDummy2Ope, //0xB8
            vcDummy3Ope,//0xc0
            vcDummy3Ope,//0xc1
            vcRf5c164MemoryWrite,//0xc2
            vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope,vcDummy3Ope, //0xC3
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
            mds.WriteSN76489(vgmBuf[vgmAdr + 1]);
            vgmAdr += 2;
        }

        private static void vcDummy1Ope()
        {
            Console.Write("({0}:{1})", vgmBuf[vgmAdr], vgmBuf[vgmAdr+1]);
            vgmAdr += 2;
        }

        private static void vcDummy2Ope()
        {
            Console.Write("({0}:{1}:{2})", vgmBuf[vgmAdr], vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            vgmAdr += 3;
        }

        private static void vcDummy3Ope()
        {
            Console.Write("({0}:{1}:{2}:{3})", vgmBuf[vgmAdr], vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2], vgmBuf[vgmAdr + 3]);
            vgmAdr += 4;
        }

        private static void vcDummy4Ope()
        {
            Console.Write("({0}:{1}:{2}:{3}:{4})", vgmBuf[vgmAdr], vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2], vgmBuf[vgmAdr + 3], vgmBuf[vgmAdr + 4]);
            vgmAdr += 5;
        }

        private static void vcYM2612Port0()
        {
            if (scYM2612 == null) mds.WriteYM2612(0, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            else scYM2612.setRegister(vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            vgmAdr += 3;
        }

        private static void vcYM2612Port1()
        {
            if (scYM2612 == null) mds.WriteYM2612(1, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            else scYM2612.setRegister(vgmBuf[vgmAdr + 1] + 0x100, vgmBuf[vgmAdr + 2]);
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
            byte bType = vgmBuf[vgmAdr + 2];
            uint bLen = getLE32(vgmAdr + 3);

            switch (bType & 0xc0)
            {
                case 0x00:
                case 0x40:
                    AddPCMData(bType, bLen, bAdr);
                    vgmAdr += (uint)bLen + 7;
                    break;
                case 0xc0:
                    uint stAdr = getLE16(vgmAdr + 7);
                    uint dataSize = bLen - 2;
                    uint ROMData = vgmAdr + 9;
                    if ((bType & 0x20) != 0)
                    {
                        stAdr = getLE32(vgmAdr + 7);
                        dataSize = bLen - 4;
                        ROMData = vgmAdr + 11;
                    }

                    switch (bType)
                    {
                        case 0xc1:
                            mds.WriteRF5C164PCMData(0, stAdr, dataSize, vgmBuf, vgmAdr + 9);
                            break;
                    }

                    vgmAdr += bLen + 7;
                    break;
                default:
                    vgmAdr += bLen + 7;
                    break;
            }

        }

        private static void vcPCMRamWrite()
        {
            byte bType = vgmBuf[vgmAdr + 2];
            uint bReadOffset = getLE24(vgmAdr + 3);
            uint bWriteOffset = getLE24(vgmAdr + 6);
            uint bSize = getLE24(vgmAdr + 9);
            if (bSize == 0) bSize = 0x1000000;
            uint? pcmAdr = GetPCMAddressFromPCMBank(bType, bReadOffset);
            if (pcmAdr!=null && bType == 0x02)
            {
                mds.WriteRF5C164PCMData(0, bWriteOffset , bSize , PCMBank[bType].Data, (uint)pcmAdr);
            }

            vgmAdr += 12;
        }

        private static void vcWaitN1Samples()
        {
            vgmWait += (int)(vgmBuf[vgmAdr] - 0x6f);
            vgmAdr++;
        }

        private static void vcWaitNSamplesAndSendYM26120x2a()
        {
            byte dat = GetDACFromPCMBank();

            if (scYM2612 == null) mds.WriteYM2612(0, 0x2a, dat);
            else scYM2612.setRegister(0x2a, dat);

            vgmWait += (int)(vgmBuf[vgmAdr] - 0x80);
            vgmAdr++;
        }

        private static void vcSetupStreamControl()
        {
            byte si = vgmBuf[vgmAdr + 1];
            if (si == 0xff)
            {
                vgmAdr += 5;
                return;
            }
            if (!DacCtrl[si].Enable)
            {
                dacControl.device_start_daccontrol(si);
                dacControl.device_reset_daccontrol(si);
                DacCtrl[si].Enable = true;
                DacCtrlUsg[DacCtrlUsed] = si;
                DacCtrlUsed++;
            }
            byte chipId = vgmBuf[vgmAdr + 2];
            byte port = vgmBuf[vgmAdr + 3];
            byte cmd = vgmBuf[vgmAdr + 4];
            dacControl.setup_chip(si, (byte)(chipId & 0x7F), (byte)((chipId & 0x80) >> 7),(uint)(port * 0x100 + cmd));
            vgmAdr += 5;
        }

        private static void vcSetStreamData()
        {
            byte si = vgmBuf[vgmAdr + 1];
            if (si == 0xff)
            {
                vgmAdr += 5;
                return;
            }
            DacCtrl[si].Bank = vgmBuf[vgmAdr+2];
            if (DacCtrl[si].Bank >= PCM_BANK_COUNT)
                DacCtrl[si].Bank = 0x00;

            VGM_PCM_BANK TempPCM = PCMBank[DacCtrl[si].Bank];
            //Last95Max = TempPCM->BankCount;
            dacControl.set_data(si, TempPCM.Data, TempPCM.DataSize,
                                vgmBuf[vgmAdr + 3], vgmBuf[vgmAdr + 4]);

            vgmAdr += 5;
        }

        private static void vcSetStreamFrequency() {
            byte si = vgmBuf[vgmAdr + 1];
            if (si == 0xFF || !DacCtrl[si].Enable)
            {
                vgmAdr += 0x06;
                return;
            }
            uint TempLng = getLE32(vgmAdr + 2);
            //Last95Freq = TempLng;
            dacControl.set_frequency(si, TempLng);
            vgmAdr += 6;
        }

        private static void vcStartStream() {
            byte si = vgmBuf[vgmAdr + 1];
            if (si == 0xFF || !DacCtrl[si].Enable || PCMBank[DacCtrl[si].Bank].BankCount==0)
            {
                vgmAdr += 0x08;
                return;
            }
            uint DataStart = getLE32(vgmAdr + 2);
            //Last95Drum = 0xFFFF;
            byte TempByt = vgmBuf[vgmAdr + 6];
            uint DataLen = getLE32(vgmAdr + 7);
            dacControl.start(si, DataStart, TempByt, DataLen);
            vgmAdr += 0x0B;

        }

        private static void vcStopStream() {
            byte si = vgmBuf[vgmAdr + 1];
            if (!DacCtrl[si].Enable)
            {
                vgmAdr += 0x02;
                return;
            }
            //Last95Drum = 0xFFFF;
            if (si < 0xFF)
            {
                dacControl.stop(si);
            }
            else
            {
                for (si = 0x00; si < 0xFF; si++)
                    dacControl.stop(si);
            }
            vgmAdr += 0x02;
        }

        private static void vcStartStreamFastCall()
        {
            byte CurChip = vgmBuf[vgmAdr + 1];
            if (CurChip == 0xFF || !DacCtrl[CurChip].Enable ||
                PCMBank[DacCtrl[CurChip].Bank].BankCount==0)
            {
                vgmAdr += 0x05;
                return;
            }
            VGM_PCM_BANK TempPCM = PCMBank[DacCtrl[CurChip].Bank];
            uint TempSht = getLE16(vgmAdr + 2);
            //Last95Drum = TempSht;
            //Last95Max = TempPCM->BankCount;
            if (TempSht >= TempPCM.BankCount)
                TempSht = 0x00;
            VGM_PCM_DATA TempBnk = TempPCM.Bank[(int)TempSht];

            byte TempByt = (byte)(dacControl.DCTRL_LMODE_BYTES |
                        (vgmBuf[vgmAdr+4] & 0x10) |         // Reverse Mode
                        ((vgmBuf[vgmAdr+4] & 0x01) << 7));   // Looping
            dacControl.start(CurChip, TempBnk.DataStart, TempByt, TempBnk.DataSize);
            vgmAdr += 0x05;

        }

        private static void vcSeekToOffsetInPCMDataBank()
        {
            PCMBank[0x00].DataPos = getLE32(vgmAdr + 1);
            vgmAdr += 5;
        }

        private static void vcRf5c164()
        {
             mds.WriteRF5C164(0, vgmBuf[vgmAdr + 1], vgmBuf[vgmAdr + 2]);
            vgmAdr += 3;
        }

        private static void vcRf5c164MemoryWrite() {
            uint offset = getLE16(vgmAdr + 1);
            mds.WriteRF5C164MemW(0, offset, vgmBuf[vgmAdr + 3]);
            vgmAdr += 4;
        }

        private static void vcPWM()
        {
            byte cmd = (byte)((vgmBuf[vgmAdr + 1] & 0xf0) >> 4);
            uint data = (uint)((vgmBuf[vgmAdr + 1] & 0xf) * 0x100 + vgmBuf[vgmAdr + 2]);
            mds.WritePWM(0, cmd, data);
            vgmAdr += 3;
        }

        private static void oneFrameVGMStream()
        {
            for (int CurChip = 0x00; CurChip < DacCtrlUsed; CurChip++)
            {
                dacControl.update(DacCtrlUsg[CurChip], 1);
            }
        }

        private static void AddPCMData(byte Type, uint DataSize, uint Adr)
        {
            uint CurBnk;
            VGM_PCM_BANK TempPCM;
            VGM_PCM_DATA TempBnk;
            uint BankSize;
            //bool RetVal;
            byte BnkType;
            byte CurDAC;

            BnkType = (byte)(Type & 0x3F);
            if (BnkType >= PCM_BANK_COUNT || VGMCurLoop>0)
                return;

            if (Type == 0x7F)
            {
                //ReadPCMTable(DataSize, Data);
                ReadPCMTable(DataSize, Adr);
                return;
            }

            TempPCM = PCMBank[BnkType];// &PCMBank[BnkType];
            TempPCM.BnkPos++;
            if (TempPCM.BnkPos <= TempPCM.BankCount)
                return; // Speed hack for restarting playback (skip already loaded blocks)
            CurBnk = TempPCM.BankCount;
            TempPCM.BankCount++;
            //if (Last95Max != 0xFFFF) Last95Max = TempPCM.BankCount;
            TempPCM.Bank.Add(new VGM_PCM_DATA());// = (VGM_PCM_DATA*)realloc(TempPCM->Bank,
                                                 // sizeof(VGM_PCM_DATA) * TempPCM->BankCount);

            if ((Type & 0x40) == 0)
                BankSize = DataSize;
            else
                BankSize = getLE32(Adr + 1);// ReadLE32(&Data[0x01]);

            TempPCM.Data = new byte[TempPCM.DataSize + BankSize];// realloc(TempPCM->Data, TempPCM->DataSize + BankSize);
            TempBnk = TempPCM.Bank[(int)CurBnk];
            TempBnk.DataStart = TempPCM.DataSize;
            TempBnk.Data = new byte[BankSize];
            if ((Type & 0x40) == 0)
            {
                TempBnk.DataSize = DataSize;
                for (int i = 0; i < DataSize; i++)
                {
                    TempPCM.Data[i + TempBnk.DataStart] = vgmBuf[Adr + i];
                    TempBnk.Data[i] = vgmBuf[Adr + i];
                }
                //TempBnk.Data = TempPCM.Data + TempBnk.DataStart;
                //memcpy(TempBnk->Data, Data, DataSize);
            }
            else
            {
                //TempBnk.Data = TempPCM.Data + TempBnk.DataStart;
                bool RetVal = DecompressDataBlk(TempBnk, DataSize, Adr);
                if (RetVal==false)
                {
                    TempBnk.Data = null;
                    TempBnk.DataSize = 0x00;
                    //return;
                    goto RefreshDACStrm;    // sorry for the goto, but I don't want to copy-paste the code
                }
                for (int i = 0; i < BankSize;i++)// DataSize; i++)
                {
                    TempPCM.Data[i + TempBnk.DataStart] = TempBnk.Data[i];
                }
            }
            if (BankSize != TempBnk.DataSize)
                Console.Write("Error reading Data Block! Data Size conflict!\n");
            TempPCM.DataSize += BankSize;

            // realloc may've moved the Bank block, so refresh all DAC Streams
            RefreshDACStrm:
            for (CurDAC = 0x00; CurDAC < DacCtrlUsed; CurDAC++)
            {
                if (DacCtrl[DacCtrlUsg[CurDAC]].Bank == BnkType)
                    dacControl.refresh_data(DacCtrlUsg[CurDAC], TempPCM.Data, TempPCM.DataSize);
            }

            return;
        }

        private static bool DecompressDataBlk(VGM_PCM_DATA Bank, uint DataSize, uint Adr)
        {
            byte ComprType;
            byte BitDec;
            byte BitCmp;
            byte CmpSubType;
            uint AddVal;
            uint InPos;
            uint InDataEnd;
            uint OutPos;
            uint OutDataEnd;
            uint InVal;
            uint OutVal = 0;// FUINT16 OutVal;
            byte ValSize;
            byte InShift;
            byte OutShift;
            uint Ent1B = 0;// UINT8* Ent1B;
            uint Ent2B = 0;// UINT16* Ent2B;
            //#if defined(_DEBUG) && defined(WIN32)
            //	UINT32 Time;
            //#endif

            // ReadBits Variables
            byte BitsToRead;
            byte BitReadVal;
            byte InValB;
            byte BitMask;
            byte OutBit;

            // Variables for DPCM
            uint OutMask;

            //#if defined(_DEBUG) && defined(WIN32)
            //	Time = GetTickCount();
            //#endif
            ComprType = vgmBuf[Adr + 0];
            Bank.DataSize = getLE32(Adr + 1);

            switch (ComprType)
            {
                case 0x00:  // n-Bit compression
                    BitDec = vgmBuf[Adr + 5];
                    BitCmp = vgmBuf[Adr + 6];
                    CmpSubType = vgmBuf[Adr + 7];
                    AddVal = getLE16(Adr + 8);

                    if (CmpSubType == 0x02)
                    {
                        Ent1B = 0;// (UINT8*)PCMTbl.Entries; // Big Endian note: Those are stored in LE and converted when reading.
                        Ent2B = 0;// (UINT16*)PCMTbl.Entries;
                        if (PCMTbl.EntryCount == 0)
                        {
                            Bank.DataSize = 0x00;
                            //printf("Error loading table-compressed data block! No table loaded!\n");
                            return false;
                        }
                        else if (BitDec != PCMTbl.BitDec || BitCmp != PCMTbl.BitCmp)
                        {
                            Bank.DataSize = 0x00;
                            //printf("Warning! Data block and loaded value table incompatible!\n");
                            return false;
                        }
                    }

                    ValSize = (byte)((BitDec + 7) / 8);
                    InPos = Adr + 0x0A;
                    InDataEnd = Adr + DataSize;
                    InShift = 0;
                    OutShift = (byte)(BitDec - BitCmp);
                    //                    OutDataEnd = Bank.Data + Bank.DataSize;
                    OutDataEnd = Bank.DataSize;

                    //for (OutPos = Bank->Data; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    for (OutPos = 0; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    {
                        //InVal = ReadBits(Data, InPos, &InShift, BitCmp);
                        // inlined - is 30% faster
                        OutBit = 0x00;
                        InVal = 0x0000;
                        BitsToRead = BitCmp;
                        while (BitsToRead != 0)
                        {
                            BitReadVal = (byte)((BitsToRead >= 8) ? 8 : BitsToRead);
                            BitsToRead -= BitReadVal;
                            BitMask = (byte)((1 << BitReadVal) - 1);

                            InShift += BitReadVal;
                            InValB = (byte)((vgmBuf[InPos] << InShift >> 8) & BitMask);
                            if (InShift >= 8)
                            {
                                InShift -= 8;
                                InPos++;
                                if (InShift != 0)
                                    InValB |= (byte)((vgmBuf[InPos] << InShift >> 8) & BitMask);
                            }

                            InVal |= (uint)(InValB << OutBit);
                            OutBit += BitReadVal;
                        }

                        switch (CmpSubType)
                        {
                            case 0x00:  // Copy
                                OutVal = InVal + AddVal;
                                break;
                            case 0x01:  // Shift Left
                                OutVal = (InVal << OutShift) + AddVal;
                                break;
                            case 0x02:  // Table
                                switch (ValSize)
                                {
                                    case 0x01:
                                        OutVal = PCMTbl.Entries[Ent1B + InVal];
                                        break;
                                    case 0x02:
                                        //#ifndef BIG_ENDIAN
                                        //					OutVal = Ent2B[InVal];
                                        //#else
                                        OutVal = (uint)(PCMTbl.Entries[Ent2B + InVal] + PCMTbl.Entries[Ent2B + InVal + 1] * 0x100);// ReadLE16((UINT8*)&Ent2B[InVal]);
                                                                                                                                   //#endif
                                        break;
                                }
                                break;
                        }

                        //#ifndef BIG_ENDIAN
                        //			//memcpy(OutPos, &OutVal, ValSize);
                        //			if (ValSize == 0x01)
                        //               *((UINT8*)OutPos) = (UINT8)OutVal;
                        //			else //if (ValSize == 0x02)
                        //                *((UINT16*)OutPos) = (UINT16)OutVal;
                        //#else
                        if (ValSize == 0x01)
                        {
                            Bank.Data[OutPos] = (byte)OutVal;
                        }
                        else //if (ValSize == 0x02)
                        {
                            Bank.Data[OutPos + 0x00] = (byte)((OutVal & 0x00FF) >> 0);
                            Bank.Data[OutPos + 0x01] = (byte)((OutVal & 0xFF00) >> 8);
                        }
                        //#endif
                    }
                    break;
                case 0x01:  // Delta-PCM
                    BitDec = vgmBuf[Adr + 5];// Data[0x05];
                    BitCmp = vgmBuf[Adr + 6];// Data[0x06];
                    OutVal = getLE16(Adr + 8);// ReadLE16(&Data[0x08]);

                    Ent1B = 0;// (UINT8*)PCMTbl.Entries;
                    Ent2B = 0;// (UINT16*)PCMTbl.Entries;
                    if (PCMTbl.EntryCount == 0)
                    {
                        Bank.DataSize = 0x00;
                        //printf("Error loading table-compressed data block! No table loaded!\n");
                        return false;
                    }
                    else if (BitDec != PCMTbl.BitDec || BitCmp != PCMTbl.BitCmp)
                    {
                        Bank.DataSize = 0x00;
                        //printf("Warning! Data block and loaded value table incompatible!\n");
                        return false;
                    }

                    ValSize = (byte)((BitDec + 7) / 8);
                    OutMask = (uint)((1 << BitDec) - 1);
                    InPos = vgmBuf[Adr + 0xa];// Data + 0x0A;
                    InDataEnd = vgmBuf[Adr + DataSize];// Data + DataSize;
                    InShift = 0;
                    OutShift = (byte)(BitDec - BitCmp);
                    OutDataEnd = Bank.DataSize;// Bank.Data + Bank.DataSize;
                    AddVal = 0x0000;

                    //                    for (OutPos = Bank.Data; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    for (OutPos = 0; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    {
                        //InVal = ReadBits(Data, InPos, &InShift, BitCmp);
                        // inlined - is 30% faster
                        OutBit = 0x00;
                        InVal = 0x0000;
                        BitsToRead = BitCmp;
                        while (BitsToRead != 0)
                        {
                            BitReadVal = (byte)((BitsToRead >= 8) ? 8 : BitsToRead);
                            BitsToRead -= BitReadVal;
                            BitMask = (byte)((1 << BitReadVal) - 1);

                            InShift += BitReadVal;
                            InValB = (byte)((vgmBuf[InPos] << InShift >> 8) & BitMask);
                            if (InShift >= 8)
                            {
                                InShift -= 8;
                                InPos++;
                                if (InShift != 0)
                                    InValB |= (byte)((vgmBuf[InPos] << InShift >> 8) & BitMask);
                            }

                            InVal |= (byte)(InValB << OutBit);
                            OutBit += BitReadVal;
                        }

                        switch (ValSize)
                        {
                            case 0x01:
                                AddVal = PCMTbl.Entries[Ent1B + InVal];
                                OutVal += AddVal;
                                OutVal &= OutMask;
                                Bank.Data[OutPos] = (byte)OutVal;// *((UINT8*)OutPos) = (UINT8)OutVal;
                                break;
                            case 0x02:
                                //#ifndef BIG_ENDIAN
                                //				AddVal = Ent2B[InVal];
                                //#else
                                AddVal = (uint)(PCMTbl.Entries[Ent2B + InVal] + PCMTbl.Entries[Ent2B + InVal + 1] * 0x100);
                                //AddVal = ReadLE16((UINT8*)&Ent2B[InVal]);
                                //#endif
                                OutVal += AddVal;
                                OutVal &= OutMask;
                                //#ifndef BIG_ENDIAN
                                //				*((UINT16*)OutPos) = (UINT16)OutVal;
                                //#else
                                Bank.Data[OutPos + 0x00] = (byte)((OutVal & 0x00FF) >> 0);
                                Bank.Data[OutPos + 0x01] = (byte)((OutVal & 0xFF00) >> 8);
                                //#endif
                                break;
                        }
                    }
                    break;
                default:
                    //printf("Error: Unknown data block compression!\n");
                    return false;
            }

            //#if defined(_DEBUG) && defined(WIN32)
            //	Time = GetTickCount() - Time;
            //	printf("Decompression Time: %lu\n", Time);
            //#endif

            return true;
        }

        private static void ReadPCMTable(uint DataSize, uint Adr)
        {
            byte ValSize;
            uint TblSize;

            PCMTbl.ComprType = vgmBuf[Adr + 0];// Data[0x00];
            PCMTbl.CmpSubType = vgmBuf[Adr + 1];// Data[0x01];
            PCMTbl.BitDec = vgmBuf[Adr + 2];// Data[0x02];
            PCMTbl.BitCmp = vgmBuf[Adr + 3];// Data[0x03];
            PCMTbl.EntryCount = getLE16(Adr + 4);// ReadLE16(&Data[0x04]);

            ValSize = (byte)((PCMTbl.BitDec + 7) / 8);
            TblSize = PCMTbl.EntryCount * ValSize;

            PCMTbl.Entries = new byte[TblSize];// realloc(PCMTbl.Entries, TblSize);
            for (int i = 0; i < TblSize; i++) PCMTbl.Entries[i] = vgmBuf[Adr + 6 + i];
            //memcpy(PCMTbl.Entries, &Data[0x06], TblSize);

            if (DataSize < 0x06 + TblSize)
            {
                Console.Write("Warning! Bad PCM Table Length!\n");
                //printf("Warning! Bad PCM Table Length!\n");
            }

            return;
        }

        private static byte GetDACFromPCMBank()
        {
            // for YM2612 DAC data only
            /*VGM_PCM_BANK* TempPCM;
            UINT32 CurBnk;*/
            uint DataPos;

            /*TempPCM = &PCMBank[0x00];
            DataPos = TempPCM->DataPos;
            for (CurBnk = 0x00; CurBnk < TempPCM->BankCount; CurBnk ++)
            {
                if (DataPos < TempPCM->Bank[CurBnk].DataSize)
                {
                    if (TempPCM->DataPos < TempPCM->DataSize)
                        TempPCM->DataPos ++;
                    return TempPCM->Bank[CurBnk].Data[DataPos];
                }
                DataPos -= TempPCM->Bank[CurBnk].DataSize;
            }
            return 0x80;*/

            DataPos = PCMBank[0x00].DataPos;
            if (DataPos >= PCMBank[0x00].DataSize)
                return 0x80;

            PCMBank[0x00].DataPos++;
            return PCMBank[0x00].Bank[0].Data[DataPos];
        }

        private static uint? GetPCMAddressFromPCMBank(byte Type, uint DataPos)
        {
            if (Type >= PCM_BANK_COUNT)
                return null;

            if (DataPos >= PCMBank[Type].DataSize)
                return null;

            return DataPos;
        }

        private static UInt32 getLE16(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100;

            return dat;
        }

        private static UInt32 getLE24(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100 + (UInt32)vgmBuf[adr + 2] * 0x10000;

            return dat;
        }

        private static UInt32 getLE32(UInt32 adr)
        {
            UInt32 dat;
            dat = (UInt32)vgmBuf[adr] + (UInt32)vgmBuf[adr + 1] * 0x100 + (UInt32)vgmBuf[adr + 2] * 0x10000 + (UInt32)vgmBuf[adr + 3] * 0x1000000;

            return dat;
        }

    }

    public class GD3
    {
        public string TrackName = "";
        public string TrackNameJ = "";
        public string GameName = "";
        public string GameNameJ = "";
        public string SystemName = "";
        public string SystemNameJ = "";
        public string Composer = "";
        public string ComposerJ = "";
        public string Converted = "";
        public string Notes = "";
        public string VGMBy = "";

        public void getGD3Info(byte[] buf,uint vgmGd3)
        {
            uint adr = vgmGd3 + 12 + 0x14;

            TrackName = "";
            TrackNameJ = "";
            GameName = "";
            GameNameJ = "";
            SystemName = "";
            SystemNameJ = "";
            Composer = "";
            ComposerJ = "";
            Converted = "";
            Notes = "";
            VGMBy = "";

            try
            {
                //trackName
                TrackName = System.Text.Encoding.Unicode.GetString(getByteArray(buf,ref adr));
                //trackNameJ
                TrackNameJ = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //gameName
                GameName = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //gameNameJ
                GameNameJ = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //systemName
                SystemName = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //systemNameJ
                SystemNameJ = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //Composer
                Composer = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //ComposerJ
                ComposerJ = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //Converted
                Converted = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //VGMBy
                VGMBy = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
                //Notes
                Notes = System.Text.Encoding.Unicode.GetString(getByteArray(buf, ref adr));
            }
            catch { }
        }

        private static byte[] getByteArray(byte[] buf,ref uint adr)
        {
            List<byte> ary = new List<byte>();
            while (buf[adr] != 0 || buf[adr + 1] != 0)
            {
                ary.Add(buf[adr]);
                adr++;
                ary.Add(buf[adr]);
                adr++;
            }
            adr += 2;

            return ary.ToArray();
        }

    }

    public class VGM_PCM_DATA
    {
        public uint DataSize;
        public byte[] Data;
        public uint DataStart;
    }

    public class VGM_PCM_BANK
    {
        public uint BankCount;
        public List<VGM_PCM_DATA> Bank=new List<VGM_PCM_DATA>();
        public uint DataSize;
        public byte[] Data;
        public uint DataPos;
        public uint BnkPos;
    }

    public class DACCTRL_DATA
    {
        public bool Enable;
        public byte Bank;
    }

    public class PCMBANK_TBL
    {
        public byte ComprType;
        public byte CmpSubType;
        public byte BitDec;
        public byte BitCmp;
        public uint EntryCount;
        public byte[] Entries;// void* Entries;
    }


}
