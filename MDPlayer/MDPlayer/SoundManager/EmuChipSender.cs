using System.Threading;

namespace SoundManager
{
    public class EmuChipSender : ChipSender
    {
        public EmuChipSender(int BufferSize = DATA_SEQUENCE_FREQUENCE)
            : base(null, BufferSize)
        {
        }

#if DEBUG
        public RingBuffer recvBuffer = new RingBuffer(DATA_SEQUENCE_FREQUENCE, "");
        //public RingBuffer recvBuffer = new RingBuffer(DATA_SEQUENCE_FREQUENCE, "EmuChipSender");
#else
        public RingBuffer recvBuffer = new RingBuffer(DATA_SEQUENCE_FREQUENCE, "");
#endif

        protected override void Main()
        {
            try
            {
                while (true)
                {
                    while (!GetStart())
                    {
                        Thread.Sleep(100);
                    }

                    lock (lockObj) isRunning = true;

                    while (true)
                    {
                        Thread.Sleep(1);
                        if (ringBuffer.GetDataSize() == 0)
                        {
                            //送信データが無く、停止指示がある場合のみ停止する
                            if (!GetStart())
                            {
                                if (recvBuffer.GetDataSize() > 0)
                                {
                                    continue;
                                }
                                break;
                            }
                            continue;
                        }

                        //dataが貯まってます！
                        lock (lockObj)
                        {
                            busy = true;
                        }

                        try
                        {
                            while (ringBuffer.Deq(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData))
                            {
                                //ActionOfChip?.Invoke(Counter, Dev, Typ, Adr, Val, Ex);
                                if (!recvBuffer.Enq(Counter, Chip, Type, Address, Data, ExData))
                                {
                                    parent.SetInterrupt();
                                    while (!recvBuffer.Enq(Counter, Chip, Type, Address, Data, ExData)) { }
                                    parent.ResetInterrupt();
                                }
                            }
                        }
                        catch
                        {

                        }

                        lock (lockObj)
                        {
                            busy = false;
                        }
                    }

                    lock (lockObj)
                    {
                        isRunning = false;
                        ringBuffer.Init(ringBufferSize);
                        recvBuffer.Init(DATA_SEQUENCE_FREQUENCE);
                    }

                }
            }
            catch
            {
                lock (lockObj)
                {
                    isRunning = false;
                    Start = false;
                }
            }
        }
    }

}
