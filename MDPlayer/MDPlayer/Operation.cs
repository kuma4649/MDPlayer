using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MDPlayer
{
    public enum Ope
    {
        END,
        PLAY,
        STOP,
        PAUSE,
        RELEASE
    }

    public class Operation
    {
        private object lockObj = new object();
        private List<Tuple<Ope, object[]>> cmdBuf = new List<Tuple<Ope, object[]>>();
        private form.frmMain parent;

        public void SendCommand(Ope cmd,params object[] option)
        {
            lock (lockObj)
            {
                cmdBuf.Add(new Tuple<Ope, object[]>(cmd, option));
            }
        }

        public Operation(form.frmMain parent)
        {
            this.parent = parent;

            Thread trd = new Thread(start);
            trd.Start();
        }

        private void start()
        {
            while (true)
            {
                Thread.Sleep(10);
                if (cmdBuf.Count < 1) continue;

                Tuple<Ope, object[]> cmd;
                lock (lockObj)
                {
                    cmd = cmdBuf[0];
                    if (cmd == null) continue;
                    if (cmd.Item1 == Ope.END) return;
                    cmdBuf.Clear();

                    switch (cmd.Item1)
                    {
                        case Ope.PLAY:
                            parent.opePlay();
                            break;
                        case Ope.STOP:
                            parent.opeStop();
                            break;
                        case Ope.PAUSE:
                            parent.pause();
                            break;
                    }
                }

                //RELEASEを受け取るまで待ち状態
                while (true)
                {
                    Thread.Sleep(10);
                    if (cmdBuf.Count < 1) continue;

                    lock (lockObj)
                    {
                        cmd = cmdBuf[0];
                        if (cmd == null) continue;
                        if (cmd.Item1 == Ope.RELEASE)
                        {
                            cmdBuf.Clear();
                            break;
                        }
                        cmdBuf.RemoveAt(0);
                    }
                }

            }
        }


    }
}
