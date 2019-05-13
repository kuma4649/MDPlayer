using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SoundManager
{
    /// <summary>
    /// データ生成器
    /// ミュージックドライバーを駆動させ、生成するデータをDataSenderに送る
    /// </summary>
    public class DataMaker : BaseMakerSender
    {
        private readonly DriverAction ActionOfDriver;
        private bool pause = false;

        public DataMaker(DriverAction ActionOfDriver)
        {
            action = Main;
            this.ActionOfDriver = ActionOfDriver;
        }

        private void Main()
        {
            try
            {
                while (true)
                {

                    pause = false;

                    while (true)
                    {
                        Thread.Sleep(100);
                        if (GetStart())
                        {
                            break;
                        }
                    }

                    lock (lockObj) isRunning = true;

                    ActionOfDriver?.Init?.Invoke();

                    while (true)
                    {
                        if (!GetStart()) break;

                        if (pause)
                        {
                            if (parent.GetDataSenderBufferSize() >= DATA_SEQUENCE_FREQUENCE / 2)
                            {
                                Thread.Sleep(10);
                                continue;
                            }

                            pause = false;
                        }

                        Thread.Sleep(0);
                        ActionOfDriver?.Main?.Invoke();

                        if (parent.GetDataSenderBufferSize() >= DATA_SEQUENCE_FREQUENCE)
                        {
                            pause = true;
                        }
                    }

                    ActionOfDriver?.Final?.Invoke();

                    lock (lockObj) isRunning = false;

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
