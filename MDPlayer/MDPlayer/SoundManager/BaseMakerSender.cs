using System;
using System.Threading;
using System.Threading.Tasks;
using MDPlayer;

namespace SoundManager
{
    public abstract class BaseMakerSender : IDisposable
    {
        public const int DATA_SEQUENCE_FREQUENCE = 44100;

        private CancellationTokenSource tokenSource;
        private CancellationToken cancellationToken;

        protected Task task = null;
        protected RingBuffer ringBuffer = null;
        protected Action action = null;
        protected volatile bool Start = false;
        protected bool isRunning = false;
        protected object lockObj = new object();

        public SoundManager parent = null;


        public long GetRingBufferCounter()
        {
            return ringBuffer.LookUpCounter();
        }

        public long GetRingBufferSize()
        {
            return ringBuffer.GetDataSize();
        }

        public bool Mount()
        {
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;

            task = new Task(action, cancellationToken);
            task.Start();

            return true;
        }

        public bool Unmount()
        {
            if (task.Status == TaskStatus.Running)
            {
                tokenSource.Cancel();
            }
            //task.Wait(1000, cancellationToken);
            return true;
        }

        public bool Enq(long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            return ringBuffer.Enq(Counter, Chip , Type, Address, Data, ExData);
        }

        public bool Deq(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            return ringBuffer.Deq(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
        }

        public void RequestStart()
        {
            lock (lockObj)
            {
                Start = true;
            }
        }

        public void RequestStop()
        {
            lock (lockObj)
            {
                Start = false;
            }
        }

        protected bool GetStart()
        {
            lock (lockObj)
            {
                return Start;
            }
        }

        public bool IsRunning()
        {
            lock (lockObj)
            {
                return isRunning;
            }
        }




        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (tokenSource != null) tokenSource.Dispose();
                    if (task != null) task.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~BaseMakerSender() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        public void Dispose()
        {
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
