using NAudio.Wave;

#pragma warning disable CS0067

namespace MDPlayer
{
    public class NullOut : IWavePlayer, IDisposable
    {
        //private bool isNoWaitMode;

        public NullOut(bool _)
        {
            //this.isNoWaitMode = isNoWaitMode;
        }

        public PlaybackState PlaybackState
        {
            get
            {
                return pbState;
            }
        }

        public float Volume
        {
            get
            {
                return 0f;
            }
            set
            {
                ;
            }
        }

        public WaveFormat OutputWaveFormat => throw new NotImplementedException();

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Init(IWaveProvider waveProvider)
        {
            //初期化
            wP = waveProvider;
        }

        public void Pause()
        {
        }

        public void Play()
        {
            //レンダリング開始
            RequestPlay();
        }

        public void Stop()
        {
            //レンダリング停止
            reqStop = true;
        }



        private Thread trdMain;
        private IWaveProvider wP;
        private readonly byte[] buf = new byte[4000];
        private PlaybackState pbState = PlaybackState.Stopped;
        private bool reqStop = false;

        private void RequestPlay()
        {
            if (trdMain != null)
            {
                return;
            }

            reqStop = false;
            trdMain = new Thread(new ThreadStart(TrdFunction))
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true,
                Name = "trdNullOutFunction"
            };
            trdMain.Start();
        }

        private void TrdFunction()
        {
            pbState = PlaybackState.Playing;

            while (!reqStop)
            {
                wP.Read(buf, 0, 4000);
            }

            pbState = PlaybackState.Stopped;
        }
    }
}
