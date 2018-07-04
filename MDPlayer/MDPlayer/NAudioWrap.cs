using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Threading;

namespace MDPlayer
{
    public class NAudioWrap
    {

        public delegate int naudioCallBack(short[] buffer, int offset, int sampleCount);
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        private WaveOut waveOut;
        private WasapiOut wasapiOut;
        private DirectSoundOut dsOut;
        private AsioOut asioOut;

        private SineWaveProvider16 waveProvider;

        private static naudioCallBack callBack = null;
        private Setting setting = null;
        private SynchronizationContext syncContext = SynchronizationContext.Current;

        public NAudioWrap(int sampleRate, naudioCallBack nCallBack)
        {
            Init(sampleRate, nCallBack);
        }

        public void Init(int sampleRate, naudioCallBack nCallBack)
        {

            Stop();

            waveProvider = new SineWaveProvider16();
            waveProvider.SetWaveFormat(sampleRate, 2);

            callBack = nCallBack;

        }

        public void Start(Setting setting)
        {
            this.setting = setting;
            if (waveOut != null) waveOut.Dispose();
            waveOut = null;
            if (wasapiOut != null) wasapiOut.Dispose();
            wasapiOut = null;
            if (dsOut != null) dsOut.Dispose();
            dsOut = null;
            if (asioOut != null) asioOut.Dispose();
            asioOut = null;

            try
            {
                switch (setting.outputDevice.DeviceType)
                {
                    case 0:
                        waveOut = new WaveOut();
                        waveOut.DeviceNumber = 0;
                        waveOut.DesiredLatency = setting.outputDevice.Latency;
                        for (int i = 0; i < WaveOut.DeviceCount; i++)
                        {
                            if (setting.outputDevice.WaveOutDeviceName == WaveOut.GetCapabilities(i).ProductName)
                            {
                                waveOut.DeviceNumber = i;
                                break;
                            }
                        }
                        waveOut.PlaybackStopped += DeviceOut_PlaybackStopped;
                        waveOut.Init(waveProvider);
                        waveOut.Play();
                        break;
                    case 1:
                        System.Guid g = System.Guid.Empty;
                        foreach (DirectSoundDeviceInfo d in DirectSoundOut.Devices)
                        {
                            if (setting.outputDevice.DirectSoundDeviceName == d.Description)
                            {
                                g = d.Guid;
                                break;
                            }
                        }
                        if (g == System.Guid.Empty)
                        {
                            dsOut = new DirectSoundOut(setting.outputDevice.Latency);
                        }
                        else
                        {
                            dsOut = new DirectSoundOut(g,setting.outputDevice.Latency);
                        }
                        dsOut.PlaybackStopped += DeviceOut_PlaybackStopped;
                        dsOut.Init(waveProvider);
                        dsOut.Play();
                        break;
                    case 2:
                        MMDevice dev = null;
                        var enumerator = new MMDeviceEnumerator();
                        var endPoints = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                        foreach (var endPoint in endPoints)
                        {
                            if (setting.outputDevice.WasapiDeviceName == string.Format("{0} ({1})", endPoint.FriendlyName, endPoint.DeviceFriendlyName))
                            {
                                dev = endPoint;
                                break;
                            }
                        }
                        if (dev == null)
                        {
                            wasapiOut = new WasapiOut(setting.outputDevice.WasapiShareMode ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive, setting.outputDevice.Latency);
                        }
                        else
                        {
                            wasapiOut = new WasapiOut(dev, setting.outputDevice.WasapiShareMode ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive, false, setting.outputDevice.Latency);
                        }
                        wasapiOut.PlaybackStopped += DeviceOut_PlaybackStopped;
                        wasapiOut.Init(waveProvider);
                        wasapiOut.Play();
                        break;
                    case 3:
                        if (AsioOut.isSupported())
                        {
                            int i = 0;
                            foreach (string s in AsioOut.GetDriverNames())
                            {
                                if (setting.outputDevice.AsioDeviceName == s)
                                {
                                    break;
                                }
                                i++;
                            }
                            asioOut = new AsioOut(i);
                            asioOut.PlaybackStopped += DeviceOut_PlaybackStopped;
                            asioOut.Init(waveProvider);
                            asioOut.Play();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                waveOut = new WaveOut();
                waveOut.PlaybackStopped += DeviceOut_PlaybackStopped;
                waveOut.Init(waveProvider);
                waveOut.DeviceNumber = 0;
                waveOut.Play();
            }

        }

        private void DeviceOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (this.syncContext == null)
                {
                    handler(this, e);
                }
                else
                {
                    syncContext.Post(state => handler(this,e), null);
                }
            }
        }

        /// <summary>
        /// コールバックの中から呼び出さないこと(ハングします)
        /// </summary>
        public void Stop()
        {
            if (waveOut != null)
            {
                try
                {
                    //waveOut.Pause();
                    waveOut.Stop();
                    while (waveOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                    waveOut.Dispose();
                }
                catch { }
                waveOut = null;
            }

            if (wasapiOut != null)
            {
                try
                {
                    //wasapiOut.Pause();
                    wasapiOut.Stop();
                    while (wasapiOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                    wasapiOut.Dispose();
                }
                catch { }
                wasapiOut = null;
            }

            if (dsOut != null)
            {
                try
                {
                    //dsOut.Pause();
                    dsOut.Stop();
                    while (dsOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                    dsOut.Dispose();
                }
                catch { }
                dsOut = null;
            }

            if (asioOut != null)
            {
                try
                {
                    //asioOut.Pause();
                    asioOut.Stop();
                    while (asioOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                    asioOut.Dispose();
                }
                catch { }
                asioOut = null;
            }

            //一休み
            //for (int i = 0; i < 10; i++)
            //{
            //    System.Threading.Thread.Sleep(1);
            //    System.Windows.Forms.Application.DoEvents();
            //}
        }

        public class SineWaveProvider16 : WaveProvider16
        {

            public SineWaveProvider16()
            {
            }

            public override int Read(short[] buffer, int offset, int sampleCount)
            {

                return callBack(buffer,offset, sampleCount);

            }

        }

        public NAudio.Wave.PlaybackState? GetPlaybackState()
        {
            bool notNull = false;

            if (waveOut != null)
            {
                if (waveOut.PlaybackState != PlaybackState.Stopped) return waveOut.PlaybackState;
            }
            if (dsOut != null)
            {
                if (dsOut.PlaybackState != PlaybackState.Stopped) return dsOut.PlaybackState;
            }
            if (wasapiOut != null)
            {
                if (wasapiOut.PlaybackState != PlaybackState.Stopped) return wasapiOut.PlaybackState;
            }
            if (asioOut != null)
            {
                if (asioOut.PlaybackState != PlaybackState.Stopped) return asioOut.PlaybackState;
            }

            return notNull ? (PlaybackState?)PlaybackState.Stopped : null;
        }

        public int getAsioLatency()
        {
            if (asioOut == null) return 0;

            return asioOut.PlaybackLatency;
        }
    }
}
