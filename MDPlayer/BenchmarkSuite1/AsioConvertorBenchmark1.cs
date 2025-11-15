using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using NAudio.Wave;
using NAudio.Wave.Asio;
using Microsoft.VSDiagnostics;

namespace MDPlayer.Benchmarks
{
    [CPUUsageDiagnoser]
    public class AsioConvertorBenchmark
    {
        private myAsioSampleConvertor.SampleConvertor convertor;
        private IntPtr inputBuffer = IntPtr.Zero;
        private IntPtr[] outputBuffers;
        private int nbSamples = 512;
        private int channels = 2;
        private int inputBytes;
        [GlobalSetup]
        public void Setup()
        {
            // Prepare a floating point interleaved source and request an INT32 ASIO convertor
            var wf = WaveFormat.CreateIeeeFloatWaveFormat(44100, channels);
            convertor = myAsioSampleConvertor.SelectSampleConvertor(wf, AsioSampleType.Int32LSB);
            Debug.Assert(convertor != null, "Sample convertor selection failed");
            // Generate sample data
            float[] data = new float[nbSamples * channels];
            var rnd = new Random(42);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (float)(rnd.NextDouble() * 2.0 - 1.0);
            }

            // Copy floats into unmanaged memory as bytes
            byte[] bytes = new byte[data.Length * sizeof(float)];
            Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);
            inputBytes = bytes.Length;
            inputBuffer = Marshal.AllocHGlobal(inputBytes);
            Marshal.Copy(bytes, 0, inputBuffer, inputBytes);
            // Allocate per-channel ASIO output buffers (Int32 per sample)
            outputBuffers = new IntPtr[channels];
            int outBytesPerChannel = nbSamples * sizeof(int); // 4 bytes per int
            for (int i = 0; i < channels; i++)
            {
                outputBuffers[i] = Marshal.AllocHGlobal(outBytesPerChannel);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (inputBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(inputBuffer);
                inputBuffer = IntPtr.Zero;
            }

            if (outputBuffers != null)
            {
                for (int i = 0; i < outputBuffers.Length; i++)
                {
                    if (outputBuffers[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(outputBuffers[i]);
                        outputBuffers[i] = IntPtr.Zero;
                    }
                }
            }
        }

        [Benchmark]
        public void Convert_FloatToInt()
        {
            // Call the convertor (this is the hot path we want to measure)
            convertor(inputBuffer, outputBuffers, channels, nbSamples);
            // Ensure the call and outputs are considered observable by the JIT
            GC.KeepAlive(outputBuffers);
        }
    }
}