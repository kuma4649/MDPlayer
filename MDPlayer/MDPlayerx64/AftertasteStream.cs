using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayerx64
{
    //参考:
    //https://puarts.com/?pid=1689


    public class AftertasteStream : WaveStream
    {
        WaveStream sourceStream;

        public AftertasteStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        private int finish = -1;
        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            if (finish > -1)
            {
                Array.Clear(buffer, offset, count);
                finish++;
                return finish > 1 ? 0 : count;
            }

            while (totalBytesRead < count)
            {
                int bytesRead;
                bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (bytesRead <= 0)
                {
                    finish = 0;
                    Array.Clear(buffer, offset + totalBytesRead, count - totalBytesRead);
                    break;
                }

                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
