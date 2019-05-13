using System;
using System.Collections.Generic;
using MDPlayer;

namespace SoundManager
{
    public class RingBuffer
    {

        private List<CntPackData> buf;
        private CntPackData enqPos = null;
        private CntPackData deqPos = null;
        private CntPackData srcPos = null;
        private CntPackData tmpPos = null;
        private int bLength = 0;

        public bool AutoExtend = true;
        public string name = "";

        private readonly object lockObj = new object();

        public RingBuffer(int size,string name="")
        {
            if (size < 2) return;

            this.name = name;
            Init(size);
        }

        public void Init(int size)
        {
            buf = new List<CntPackData>
            {
                new CntPackData()
            };
            for (int i = 1; i < size + 1; i++)
            {
                buf.Add(new CntPackData());
                buf[i].prev = buf[i - 1];
                buf[i - 1].next = buf[i];
            }
            buf[0].prev = buf[buf.Count - 1];
            buf[buf.Count - 1].next = buf[0];

            enqPos = buf[0];
            deqPos = buf[0];
            bLength = 0;
        }

        public bool Enq(long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            if (name != "") log.Write(
                string.Format("[{0}]:Enqueue:Counter:{1}:Model:{2}:Device:{3}:Number:{4}:Type:{5}:Address:{6:x4}:Data:{7:x4}"
                , name, Counter, Chip.Model, Chip.Device, Chip.Number, Type, Address, Data));

            lock (lockObj)
            {
                if (enqPos.next == deqPos)
                {
                    if (!AutoExtend)
                    {
                        return false;
                    }
                    //自動拡張
                    try
                    {
                        CntPackData p = new CntPackData();
                        buf.Add(p);
                        p.prev = enqPos;
                        p.next = enqPos.next;
                        enqPos.next = p;
                        p.next.prev = p;
                    }
                    catch
                    {
                        return false;
                    }
                }

                bLength++;

                //データをセット
                enqPos.Counter = Counter;
                enqPos.pack.Copy(Chip, Type, Address, Data, ExData);

                if (Counter >= enqPos.prev.Counter)
                {
                    enqPos = enqPos.next;

                    //debugDispBuffer();

                    return true;
                }

                CntPackData lastPos = enqPos.prev;
                //サーチ
                srcPos = enqPos.prev;
                while (Counter < srcPos.Counter && srcPos != deqPos)
                {
                    srcPos = srcPos.prev;
                }

                if (Counter < srcPos.Counter && srcPos == deqPos)
                {
                    srcPos = srcPos.prev;
                    deqPos = enqPos;
                }

                //enqPosをリングから切り出す。
                CntPackData nextPack = enqPos;
                enqPos.prev.next = enqPos.next;
                enqPos.next.prev = enqPos.prev;

                //enqPosを挿入する
                tmpPos = srcPos.next;
                tmpPos.prev = enqPos;
                srcPos.next = enqPos;
                enqPos.prev = srcPos;
                enqPos.next = tmpPos;

                enqPos = lastPos.next;

                //debugDispBuffer();

                return true;
            }
        }

        public bool Deq(ref long Counter,ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            lock (lockObj)
            {
                Counter = deqPos.Counter;

                Chip.Move(deqPos.pack.Chip);
                Type = deqPos.pack.Type;
                Address = deqPos.pack.Address;
                Data = deqPos.pack.Data;
                ExData = deqPos.pack.ExData;

                if (enqPos == deqPos)
                {
                    bLength = 0;
                    return false;
                }

                bLength--;
                deqPos.Counter = 0;
                deqPos = deqPos.next;

                //debugDispBuffer();

                return true;
            }
        }

        public int GetDataSize()
        {
            lock (lockObj)
            {
                return bLength;
            }
        }

        public long LookUpCounter()
        {
            lock (lockObj)
            {
                return deqPos.Counter;
            }
        }

#if DEBUG

        public void debugDispBuffer()
        {
            lock (lockObj)
            {
                CntPackData edbg = deqPos;
                do
                {
                    Console.Write("[{0}:{1}]::", edbg.Counter, edbg.pack.Chip.Device);
                    edbg = edbg.next;
                } while (edbg != enqPos.next);
                Console.WriteLine("");
            }
        }

#endif 

    }
}
