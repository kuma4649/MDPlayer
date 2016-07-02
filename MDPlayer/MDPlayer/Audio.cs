using System;
using System.Collections.Generic;
using NScci;
using System.Threading;
using System.Diagnostics;

namespace MDPlayer
{
    public class Audio
    {

        public enum enmScciChipType : int
        {
            YM2612 = 5
            , SN76489 = 7
        }

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

        private static uint SamplingRate = 44100;

        private static uint samplingBuffer = 1024;
        private static MDSound.MDSound mds = new MDSound.MDSound(
            SamplingRate
            , samplingBuffer
            , vgm.defaultYM2612ClockValue
            , vgm.defaultSN76489ClockValue
            , vgm.defaultRF5C164ClockValue
            , vgm.defaultPWMClockValue);

        private static NAudioWrap naudioWrap;

        private static NScci.NScci nscci;
        private static NSoundChip scYM2612 = null;
        private static NSoundChip scSN76489 = null;

        private static ChipRegister chipRegister = null;


        private static Thread trdMain = null;
        private static bool trdClosed = false;
        private static bool trdStopped = true;
        private static Stopwatch sw = Stopwatch.StartNew();
        private static double swFreq = Stopwatch.Frequency;

        private static byte[] vgmBuf = null;

        private static double vgmSpeed;

        private static bool vgmFadeout;
        private static double vgmFadeoutCounter;
        private static double vgmFadeoutCounterV;

        private static bool Paused = false;
        private static bool Stopped = false;

        private static Setting setting = null;

        public static vgm vgmVirtual = null;
        public static vgm vgmReal = null;



        public static void Init(Setting setting)
        {
            vgmVirtual = new vgm();
            vgmVirtual.dacControl.mds = mds;
            vgmReal = new vgm();
            vgmReal.dacControl.mds = mds;

            naudioWrap = new NAudioWrap((int)SamplingRate, naudioCb);
            naudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;

            Audio.setting = setting.Copy();

            mds.Init(SamplingRate, samplingBuffer / 2
                , vgm.defaultYM2612ClockValue
                , vgm.defaultSN76489ClockValue
                , (vgm.defaultRF5C164ClockValue & 0x80000000) + (uint)((vgm.defaultRF5C164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                , (uint)(vgm.defaultPWMClockValue * (SamplingRate / (23011361.0 / 384)))
            );

            nscci = new NScci.NScci();

            scYM2612 = getChip(Audio.setting.YM2612Type.SoundLocation, Audio.setting.YM2612Type.BusID, Audio.setting.YM2612Type.SoundChip);
            if (scYM2612 != null)
            {
                scYM2612.init();
                vgmVirtual.dacControl.scYM2612 = scYM2612;
                vgmReal.dacControl.scYM2612 = scYM2612;
            }
            scSN76489 = getChip(Audio.setting.SN76489Type.SoundLocation, Audio.setting.SN76489Type.BusID, Audio.setting.SN76489Type.SoundChip);
            if (scSN76489 != null)
            {
                scSN76489.init();
                vgmVirtual.dacControl.scSN76489 = scSN76489;
                vgmReal.dacControl.scSN76489 = scSN76489;
            }

            chipRegister = new ChipRegister(mds, scYM2612, scSN76489, setting.YM2612Type, setting.SN76489Type);

            Paused = false;
            Stopped = true;
            fatalError = false;

            naudioWrap.Start(Audio.setting);

        }

        public static List<NScci.NSoundChip> getYM2612ChipList()
        {
            return getChipList((int)enmScciChipType.YM2612);
        }

        public static List<NScci.NSoundChip> getSN76489ChipList()
        {
            return getChipList((int)enmScciChipType.SN76489);
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

                vgm.enmUseChip usechip = vgm.enmUseChip.Unuse;
                if (setting.YM2612Type.UseScci)
                {
                    if (setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip |= vgm.enmUseChip.YM2612Ch6;
                    }
                }
                else
                {
                    usechip |= vgm.enmUseChip.YM2612;
                    usechip |= vgm.enmUseChip.YM2612Ch6;
                }
                if (!setting.SN76489Type.UseScci)
                {
                    usechip |= vgm.enmUseChip.SN76489;
                }
                usechip |= vgm.enmUseChip.RF5C164;
                usechip |= vgm.enmUseChip.PWM;

                if (!vgmVirtual.init(vgmBuf
                    , chipRegister
                    , vgm.enmModel.VirtualModel
                    , usechip
                    , SamplingRate * (uint)setting.LatencyEmulation/1000
                    ))
                    return false;

                usechip = vgm.enmUseChip.Unuse;
                if (setting.YM2612Type.UseScci)
                {
                    usechip |= vgm.enmUseChip.YM2612;
                    if (!setting.YM2612Type.OnlyPCMEmulation)
                    {
                        usechip |= vgm.enmUseChip.YM2612Ch6;
                    }
                }

                if (!vgmReal.init(vgmBuf
                    , chipRegister
                    , vgm.enmModel.RealModel
                    , usechip
                    , SamplingRate * (uint)setting.LatencySCCI / 1000
                    ))
                    return false;

                vgmFadeout = false;
                vgmFadeoutCounter = 1.0;
                vgmFadeoutCounterV = 0.00001;

                trdClosed = false;
                trdMain = new Thread(new ThreadStart(ThreadFunction));
                trdMain.Priority = ThreadPriority.Highest;
                trdMain.IsBackground = true;
                trdMain.Start();

                mds.Init(SamplingRate, samplingBuffer / 2
                    , vgmVirtual.YM2612ClockValue
                    , vgmVirtual.SN76489ClockValue
                    , (vgmVirtual.RF5C164ClockValue & 0x80000000) + (uint)((vgmVirtual.RF5C164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                    , (uint)(vgmVirtual.PWMClockValue * (SamplingRate / (23011361.0 / 384)))
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
                    while (!trdStopped) { Thread.Sleep(1); };
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
                while (!trdStopped) { Thread.Sleep(1); };
                if(scYM2612!=null) scYM2612.init();
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
                nscci = null;
            }
            catch
            {
            }
        }


        public static long GetCounter()
        {
            return vgmVirtual.Counter;
        }

        public static long GetTotalCounter()
        {
            return vgmVirtual.TotalCounter;
        }

        public static long GetLoopCounter()
        {
            return vgmVirtual.LoopCounter;
        }

        public static int[][] GetFMRegister()
        {
            return chipRegister.fmRegister;
        }

        public static int[] GetPSGRegister()
        {
            return chipRegister.psgRegister;
        }

        public static MDSound.scd_pcm.pcm_chip_ GetRf5c164Register()
        {
            return mds.ReadRf5c164Register();
        }

        public static int[] GetFMKeyOn()
        {
            return chipRegister.fmKeyOn;
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

        public static int[][] GetRf5c164Volume()
        {
            return mds.ReadRf5c164Volume();
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


        private static NScci.NSoundChip getChip(int SoundLocation, int BusID, int SoundChip)
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

        private static void ThreadFunction()
        {
            double o = sw.ElapsedTicks / swFreq;
            double step = 1 / (double)SamplingRate;

            trdStopped = false;
            while (!trdClosed)
            {
                Thread.Sleep(0);
                if (Stopped || Paused) continue;

                double el1 = sw.ElapsedTicks / swFreq;
                if (el1 - o < step) continue;

                o = el1 - ((el1 - o) - step);

                if (scYM2612 != null)
                {
                    vgmReal.oneFrameVGM();
                }
            }
            trdStopped = true;
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
                cnt = mds.Update2(buffer, offset, sampleCount, vgmVirtual.oneFrameVGM);

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
                        , vgm.defaultYM2612ClockValue
                        , vgm.defaultSN76489ClockValue
                        , (vgm.defaultRF5C164ClockValue & 0x80000000) + (uint)((vgm.defaultRF5C164ClockValue & 0x7fffffff) * (SamplingRate / (12500000.0 / 384)))
                        , (uint)(vgm.defaultPWMClockValue * (SamplingRate / (23011361.0 / 384)))
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


}
