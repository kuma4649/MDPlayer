using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MDPlayer
{
    public class NAudioWrap
    {
        private WaveOut waveOut;
        private WasapiOut wasapiOut;
        private DirectSoundOut dsOut;
        private AsioOut asioOut;

        private SineWaveProvider16 waveProvider;

        public delegate int naudioCallBack(short[] buffer, int offset, int sampleCount);
        private static naudioCallBack callBack = null;
        private Setting setting = null;

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

            try
            {
                switch (setting.outputDevice.DeviceType)
                {
                    case 0:
                        waveOut = new WaveOut();
                        waveOut.DeviceNumber = 0;
                        for (int i = 0; i < WaveOut.DeviceCount; i++)
                        {
                            if (setting.outputDevice.WaveOutDeviceName == WaveOut.GetCapabilities(i).ProductName)
                            {
                                waveOut.DeviceNumber = i;
                                break;
                            }
                        }
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
                            dsOut = new DirectSoundOut();
                        }
                        else
                        {
                            dsOut = new DirectSoundOut(g);
                        }
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
                            wasapiOut = new WasapiOut(setting.outputDevice.WasapiShareMode ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive, 10);
                        }
                        else
                        {
                            wasapiOut = new WasapiOut(dev, setting.outputDevice.WasapiShareMode ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive, false, 10);
                        }
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
                            asioOut.Init(waveProvider);
                            asioOut.Play();
                        }
                        break;
                }
            }
            catch
            {
                waveOut = new WaveOut();
                waveOut.Init(waveProvider);
                waveOut.DeviceNumber = 0;
                waveOut.Play();
            }

        }

        /// <summary>
        /// コールバックの中から呼び出さないこと(ハングします)
        /// </summary>
        public void Stop()
        {
            if (waveOut != null)
            {
                //waveOut.Pause();
                waveOut.Stop();
                while (waveOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                waveOut.Dispose();
                waveOut = null;
            }

            if (wasapiOut != null)
            {
                //wasapiOut.Pause();
                wasapiOut.Stop();
                while (wasapiOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                wasapiOut.Dispose();
                wasapiOut = null;
            }

            if (dsOut != null)
            {
                //dsOut.Pause();
                dsOut.Stop();
                while (dsOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                dsOut.Dispose();
                dsOut = null;
            }

            if (asioOut != null)
            {
                //asioOut.Pause();
                asioOut.Stop();
                while (asioOut.PlaybackState != PlaybackState.Stopped) { System.Threading.Thread.Sleep(1); }
                asioOut.Dispose();
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


    }
}
