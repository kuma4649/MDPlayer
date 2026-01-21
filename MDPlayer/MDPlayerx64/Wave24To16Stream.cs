using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayerx64
{
    public class Wave24To16Stream : WaveStream
    {
        private WaveStream sourceStream;

        private readonly WaveFormat waveFormat;

        private readonly long length;

        private long position;

        private bool clip;

        private float volume;

        private readonly object lockObject = new object();

        //
        // 概要:
        //     The NAudio.Wave.Wave32To16Stream.Read(System.Byte[],System.Int32,System.Int32)
        //     method reuses the same buffer to prevent unnecessary allocations.
        private byte[] sourceBuffer;

        //
        // 概要:
        //     Sets the volume for this stream. 1.0f is full scale
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

        //
        // 概要:
        //     NAudio.Wave.WaveStream.BlockAlign
        public override int BlockAlign => sourceStream.BlockAlign * 2 / 3;

        //
        // 概要:
        //     Returns the stream length
        public override long Length => length;

        //
        // 概要:
        //     Gets or sets the current position in the stream
        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                lock (lockObject)
                {
                    value -= value % BlockAlign;
                    sourceStream.Position = value * 2;
                    position = value;
                }
            }
        }

        //
        // 概要:
        //     NAudio.Wave.WaveStream.WaveFormat
        public override WaveFormat WaveFormat => waveFormat;

        //
        // 概要:
        //     Clip indicator. Can be reset.
        public bool Clip
        {
            get
            {
                return clip;
            }
            set
            {
                clip = value;
            }
        }

        //
        // 概要:
        //     Creates a new Wave32To16Stream
        //
        // パラメーター:
        //   sourceStream:
        //     the source stream
        public Wave24To16Stream(WaveStream sourceStream)
        {
            //if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            //{
            //    throw new ArgumentException("Only WaveFormatEncoding.Pcm supported");
            //}

            if (sourceStream.WaveFormat.BitsPerSample != 24)
            {
                throw new ArgumentException("Only 24 bit supported");
            }

            waveFormat = new WaveFormat(sourceStream.WaveFormat.SampleRate, 16, sourceStream.WaveFormat.Channels);
            volume = 1f;
            this.sourceStream = sourceStream;
            length = sourceStream.Length * 2 / 3;
            position = sourceStream.Position * 2 / 3;
        }

        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            lock (lockObject)
            {
                int num = numBytes * 3 / 2;
                sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
                int num2 = sourceStream.Read(sourceBuffer, 0, num);
                Convert24To16(destBuffer, offset, sourceBuffer, num2);
                position += num2 * 2 / 3;
                return num2 * 2 / 3;
            }
        }

        //
        // 概要:
        //     Conversion to 16 bit and clipping
        private unsafe void Convert24To16(byte[] destBuffer, int offset, byte[] source, int bytesRead)
        {
            fixed (byte* ptr = &destBuffer[offset])
            {
                fixed (byte* ptr3 = &source[0])
                {
                    short* ptr2 = (short*)ptr;
                    byte* ptr4 = (byte*)ptr3;
                    int num = destBuffer.Length/2;// bytesRead / (3/2*2);
                    for (int i = 0; i < num; i++)
                    {
                        short a = (short)(ptr4[i * 3 + 1] | (ptr4[i * 3 + 2] << 8));
                        ptr2[i] = a;
                        //float num2 = ptr4[i] * volume;
                        //if (num2 > 1f)
                        //{
                        //    ptr2[i] = short.MaxValue;
                        //    clip = true;
                        //}
                        //else if (num2 < -1f)
                        //{
                        //    ptr2[i] = short.MinValue;
                        //    clip = true;
                        //}
                        //else
                        //{
                        //    ptr2[i] = (short)(num2 * 32767f);
                        //}
                    }
                }
            }
        }

        //
        // 概要:
        //     Disposes this WaveStream
        protected override void Dispose(bool disposing)
        {
            if (disposing && sourceStream != null)
            {
                sourceStream.Dispose();
                sourceStream = null;
            }

            base.Dispose(disposing);
        }
    }
}