using System;
using System.Diagnostics;
using System.Threading;
using MDPlayer;

namespace SoundManager
{
    public class DataSender : BaseSender
    {
        private static Stopwatch sw = Stopwatch.StartNew();
        private static readonly double swFreq = Stopwatch.Frequency;
        private readonly int Frq = DATA_SEQUENCE_FREQUENCE;
        private const long Def_SeqCounter = -1500;
        private long SeqCounter = Def_SeqCounter;
        private double SeqSpeed = 0.0;
        private double SeqSpeedDelta = 1.0;//0.2;

        private readonly Enq EmuEnq = null;
        private readonly Enq RealEnq = null;
        private PackData[] startData = null;
        private PackData[] stopData = null;
        private Deq ProcessingData;
        private Action<long> DataSeqFrqCallBack = null;
        private Action WaitSync = null;
        private long EmuDelay = 0;
        private long RealDelay = 0;

        public DataSender(
            Enq EmuEnq
            , Enq RealEnq
            , Deq ProcessingData
            , PackData[] startData
            , PackData[] stopData
            , Action<long> DataSeqFrqCallBack
            , Action WaitSync
            , long EmuDelay
            , long RealDelay
            , int BufferSize = DATA_SEQUENCE_FREQUENCE
            , int Frq = DATA_SEQUENCE_FREQUENCE
            )
        {
            action = Main;
            this.Frq = Frq;
            ringBuffer = new RingBuffer(BufferSize);
            ringBuffer.AutoExtend = false;
            this.ringBufferSize = BufferSize;
            SeqCounter = Def_SeqCounter;
            this.EmuEnq = EmuEnq;
            this.RealEnq = RealEnq;
            this.startData = startData;
            this.stopData = stopData;
            this.ProcessingData = ProcessingData;
            this.DataSeqFrqCallBack = DataSeqFrqCallBack;
            this.WaitSync = WaitSync;
            this.EmuDelay = EmuDelay;
            this.RealDelay = RealDelay;
        }

    public void ResetSeqCounter()
        {
            lock (lockObj)
            {
                SeqCounter = Def_SeqCounter;
            }
        }

        public long GetSeqCounter()
        {
            lock (lockObj)
            {
                return SeqCounter;
            }
        }

        public void SetSpeed(double sp)
        {
            lock (lockObj)
            {
                SeqSpeedDelta = sp;
            }
        }

        public double GetSpeed()
        {
            lock (lockObj)
            {
                return SeqSpeedDelta;
            }
        }

        public void Init()
        {
            SeqCounter = Def_SeqCounter;
            ringBuffer.Init(ringBufferSize);

            //開始時のデータの送信
            if (startData != null)
            {
                foreach (PackData dat in startData)
                {
                    //振り分けてEnqueue
                    if (dat.Chip.Model == EnmModel.VirtualModel)
                        while (!EmuEnq(0, dat.Chip, dat.Type, dat.Address, dat.Data, null)) Thread.Sleep(1);
                    else
                        while (!RealEnq(0, dat.Chip, dat.Type, dat.Address, dat.Data, null)) Thread.Sleep(1);
                }
            }

        }

        public new bool Enq(long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            switch (Chip.Model)
            {
                case EnmModel.None:
                    return ringBuffer.Enq(Counter, Chip, Type, Address, Data, ExData);

                case EnmModel.VirtualModel:
                    return ringBuffer.Enq(Counter + EmuDelay, Chip, Type, Address, Data, ExData);

                case EnmModel.RealModel:
                    return ringBuffer.Enq(Counter + RealDelay, Chip, Type, Address, Data, ExData);

            }

            return ringBuffer.Enq(Counter, Chip, Type, Address, Data, ExData);
        }

        private void Main()
        {
            try
            {

                while (true)
                {
                    while (!GetStart())
                    {
                        Thread.Sleep(100);
                    }

                    WaitSync?.Invoke();

                    lock (lockObj) isRunning = true;

                    double o = sw.ElapsedTicks / swFreq;
                    double step = 1 / (double)Frq;
                    SeqCounter = Def_SeqCounter;

                    while (true)
                    {
                        if (!GetStart()) break;
                        Thread.Sleep(0);

                        double el1 = sw.ElapsedTicks / swFreq;
                        if (el1 - o < step) continue;
                        if (el1 - o >= step * Frq / 100.0)//閾値10ms
                        {
                            do
                            {
                                o += step;
                            } while (el1 - o >= step);
                        }
                        else
                        {
                            o += step;
                        }

                        //lock (lockObj)
                        {
                            //待ち合わせ割り込み
                            if (parent.GetInterrupt())
                            {
                                //Thread.Sleep(0);
                                continue;
                            }

                            SeqSpeed += SeqSpeedDelta;
                            while (SeqSpeed >= 1.0)
                            {
                                SeqCounter++;
                                SeqSpeed -= 1.0;
                            }

                            if (SeqCounter < 0) continue;

                            //コールバック実施
                            DataSeqFrqCallBack?.Invoke(SeqCounter);

                            if (ringBuffer.GetDataSize() == 0)
                            {
                                if (!parent.IsRunningAtDataMaker())
                                {
                                    //RequestStop();
                                    break;
                                }
                                continue;
                            }
                            if (SeqCounter < ringBuffer.LookUpCounter()) continue;
                            //continue;
                        }

                        //dataが貯まってます！
                        while (SeqCounter >= ringBuffer.LookUpCounter())
                        {
                            if (!ringBuffer.Deq(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData))
                            {
                                break;
                            }

                            //データ加工
                            ProcessingData?.Invoke(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);

                            //振り分けてEnqueue
                            if (Chip.Model == EnmModel.VirtualModel)
                            {
                                while (!EmuEnq(Counter, Chip, Type, Address, Data, ExData))
                                {
                                    if (!Start)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(0);
                                }
                            }
                            else if (Chip.Model == EnmModel.RealModel)
                            {
                                while (!RealEnq(Counter, Chip, Type, Address, Data, ExData))
                                {
                                    if (!Start)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(0);
                                }
                            }
                            else
                            {
                                //演奏制御処理
                                switch (Type)
                                {
                                    case EnmDataType.FadeOut:
                                        parent.SetFadeOut();
                                        break;
                                    case EnmDataType.Loop:
                                        parent.CountUpLoopCounter();
                                        break;
                                }
                            }
                        }
                    }

                    //停止時のデータの送信
                    if (stopData != null)
                    {
                        foreach (PackData dat in stopData)
                        {

                            //データ加工
                            ProcessingData?.Invoke(ref SeqCounter, ref dat.Chip, ref dat.Type, ref dat.Address, ref dat.Data, ref dat.ExData);

                            //振り分けてEnqueue
                            if (dat.Chip.Model == EnmModel.VirtualModel)
                            {
                                if (parent.IsRunningAtEmuChipSender())
                                {
                                    int timeOut = 1000;
                                    while (!EmuEnq(SeqCounter, dat.Chip, dat.Type, dat.Address, dat.Data, dat.ExData))
                                    {
                                        Thread.Sleep(1);
                                        timeOut--;
                                        if (timeOut == 0) goto Timeout;
                                    }
                                }
                            }
                            else if (dat.Chip.Model == EnmModel.RealModel)
                            {
                                if (parent.IsRunningAtRealChipSender())
                                {
                                    int timeOut = 1000;
                                    while (!RealEnq(SeqCounter, dat.Chip, dat.Type, dat.Address, dat.Data, dat.ExData))
                                    {
                                        Thread.Sleep(1);
                                        timeOut--;
                                        if (timeOut == 0) goto Timeout;
                                    }
                                }
                            }
                        }
                    Timeout:;
                    }

                    lock (lockObj)
                    {
                        isRunning = false;
                        Counter = 0;
                        Start = false;
                    }
                    parent.RequestStopAtEmuChipSender();
                    parent.RequestStopAtRealChipSender();
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

        public void SetStopData(PackData[] stopData)
        {
            this.stopData = stopData;
        }
    }

}
