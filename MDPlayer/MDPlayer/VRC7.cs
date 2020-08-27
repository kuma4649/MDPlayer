using MDSound;

namespace MDPlayer
{
    public class VRC7 : MDSound.ym2413
    {
        public override string Name { get => "VRC7";  set => throw new System.NotImplementedException(); }
        public override string ShortName { get => "VRC7"; set => throw new System.NotImplementedException(); }

        private MDSound.np.chip.nes_vrc7 nv;
        private double apu_clock_rest;
        private double rate;

        public VRC7()
        {
            nv = new MDSound.np.chip.nes_vrc7();
        }

        public override void Reset(byte ChipID)
        {
        }

        public override uint Start(byte ChipID, uint clock)
        {
            return Start(ChipID, clock, 0);
        }

        public override uint Start(byte ChipID, uint clock, uint ClockValue, params object[] option)
        {
            nv.SetClock(ClockValue/2);//masterclock(NES:1789773)
            nv.SetRate(clock);//samplerate
            nv.Reset();
            rate = (double)clock;
            return clock;
        }

        public override void Stop(byte ChipID)
        {
            ;
        }

        private int[] b = new int[2];

        public override void Update(byte ChipID, int[][] outputs, int samples)
        {
            double apu_clock_per_sample = 1789773 / rate;

            for (int i = 0; i < samples; i++)
            {
                // tick APU / expansions
                apu_clock_rest += apu_clock_per_sample;
                int apu_clocks = (int)(apu_clock_rest);
                if (apu_clocks > 0)
                {
                    apu_clock_rest -= (double)(apu_clocks);
                }

                nv.Tick((uint)apu_clocks);
                nv.Render(b);
                //if(b[0]!=0)System.Console.WriteLine("{0}",b[0]);
                outputs[0][i] += b[0]<<2;
                outputs[1][i] += b[1]<<2;
            }
        }

        public override int Write(byte ChipID, int port, int adr, int data)
        {
            nv.Write(0x9010, (uint)adr);
            nv.Write(0x9030, (uint)data);
            return 0;
        }
    }
}