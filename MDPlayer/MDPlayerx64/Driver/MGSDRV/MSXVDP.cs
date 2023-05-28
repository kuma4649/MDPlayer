namespace MDPlayer.Driver.MGSDRV
{
    public class MSXVDP
    {
        private int port1Flip = 0;
        private byte port1Data = 0;
        private byte port1RegNo = 0;
        private byte[] R = new byte[47];
        private bool adrCounterSet = false;
        private int adrCounter = 0;
        private bool adrCounterIsWrite = false;
        private byte[] memory = new byte[0x2_00_00];

        public byte m = 0;
        public byte n = 0;

        public MSXVDP()
        {
        }

        public byte Read(int address)
        {

            byte value = 0;

            switch (address)
            {
                case 0:
                    if (!adrCounterIsWrite)
                    {
                        //log.Write(LogLevel.Trace, "VDP.Read:memory[{0:X05}]=Val:{1:X02}", adrCounter, memory[adrCounter]);
                        value = memory[adrCounter++];
                    }
                    break;
                case 1:
                    throw new NotImplementedException();
                case 2:
                    throw new NotImplementedException();
            }

            //log.Write(LogLevel.Trace, "VDP.Read:Port:{0:X} Val:{1:X02}", address, value);
            return value;
        }

        public void Write(int address, byte value)
        {
            //log.Write(LogLevel.Trace, "VDP.Write:Port:{0:X} Val:{1:X02}", address, value);

            switch (address)
            {
                case 0:
                    if (adrCounterIsWrite)
                    {
                        //log.Write(LogLevel.Trace, "VDP.Write:memory[{0:X05}]=Val:{1:X02}", adrCounter, value);
                        memory[adrCounter++] = value;
                    }
                    break;
                case 1:
                    if (port1Flip == 0)
                    {
                        port1Data = value;
                    }
                    else
                    {
                        if ((value & 0xc0) == 0x80)
                        {
                            int regNo = value & 0x3f;
                            R[regNo] = port1Data;
                            if (regNo == 14)
                            {
                                adrCounterSet = true;
                                adrCounter &= 0x0_3f_ff;
                                adrCounter |= (port1Data & 0x7) << 14;
                            }
                        }
                        else if ((value & 0x80) == 0x00)
                        {
                            if (adrCounterSet)
                            {
                                adrCounter &= 0x1_ff_00;
                                adrCounter |= port1Data;

                                adrCounterIsWrite = (value & 0x40) != 0;
                                adrCounter &= 0x1_c0_ff;
                                adrCounter |= (value & 0x3f) << 8;
                                adrCounterSet = false;
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    port1Flip++;
                    port1Flip &= 1;
                    break;
                case 3:
                    throw new NotImplementedException();
            }

        }
    }
}