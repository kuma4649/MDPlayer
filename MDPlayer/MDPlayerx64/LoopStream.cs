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


    public class LoopStream : WaveStream
    {
        WaveStream sourceStream;
        private long loopstart;
        private long loopLength;
        public int loopCount;

        public LoopStream(WaveStream sourceStream, long loopStart, long loopLength)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
            this.loopstart = loopStart * 4 * 2;//4:32bit=4byte 2:ステレオ(2ch)
            this.loopLength = loopLength * 4 * 2;
            loopCount = 0;
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead;

                if (sourceStream.Position + count < loopstart + loopLength)
                {
                    bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                }
                else
                {
                    bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, (int)(loopstart + loopLength - sourceStream.Position));
                    sourceStream.Position = loopstart;
                    loopCount++;
                }

                if (bytesRead == 0)
                {
                    Array.Clear(buffer, offset + totalBytesRead, count - totalBytesRead);
                    break;
                }

                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
