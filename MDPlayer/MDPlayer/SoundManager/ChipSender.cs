using System.Threading;

namespace SoundManager
{
    public class ChipSender : BaseSender
    {
        protected readonly Snd ActionOfChip = null;
        protected bool busy = false;

        public ChipSender(Snd ActionOfChip, int BufferSize = DATA_SEQUENCE_FREQUENCE)
        {
            action = Main;
            ringBuffer = new RingBuffer(BufferSize)
            {
                AutoExtend = false
            };
            this.ringBufferSize = BufferSize;
            this.ActionOfChip = ActionOfChip;
        }

        public bool IsBusy()
        {
            lock (lockObj)
            {
                return busy;
            }
        }

        protected virtual void Main() { }
    }

}
