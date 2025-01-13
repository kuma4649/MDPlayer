using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZMS.nise68
{
    //参考:
    // Outside X68000

    public class scc_A
    {
        private Func<int, byte, int> SCC;
        private byte group = 0;
        private byte interrupt = 0;
        private byte vect = 0;
        private byte intMask = 0xff;

        private byte[] reg = new byte[0x100];
        private Action<byte>[] cmdw = null;
        private Func<byte>[] cmdr = null;
        private ushort generalTimerValue = 0;
        private int renderingFreq;
        //private double clkM = 4_915_200.0 / 8.0;// 1_000_000.0;
        private double clkM = 1_000_000.0;
        private double stepM;
        private double generalTimerValueWrk = 0.0;
        private double clickcounter = 1.0;

        private byte currentReg = 0;

        public scc_A(int renderingFreq, Func<int, byte, int> SCC)
        {
            this.SCC = SCC;

            setClock(renderingFreq);
        }

        public void setClock(int renderingFreq)
        {
            this.renderingFreq = renderingFreq;
            stepM = clkM / (double)renderingFreq;
        }

        public byte Read(uint ptr)
        {
            int c = (int)(ptr & 0xf);
            byte dat = 0;
            if (c == 0x5)
            {
                switch (currentReg)
                {
                    case 0:
                        dat = 4;//4 TxBufferEmpty
                        break;
                    case 1:
                        dat = 1;//1 AllSent完了
                        break;
                    case 2:
                        dat = vect;
                        break;
                    case 3:
                        dat = 0;
                        break;
                    case 8:
                        throw new NotImplementedException();
                    case 10:
                        throw new NotImplementedException();
                    case 12:
                        throw new NotImplementedException();
                    case 13:
                        throw new NotImplementedException();
                    case 15:
                        throw new NotImplementedException();
                }
                ;
                currentReg = 0;//何かするとレジスタは0にリセットされる
            }
            else
            {
                return dat;
                throw new NotImplementedException();
            }
            Log.WriteLine(LogLevel.Trace2, "Read SCC_A Adr:$00e9_80{0:x02} Dat:${1:x02}", c, dat);
            return dat;
        }

        public bool Write(uint ptr, byte dat)
        {
            int c = (int)(ptr & 0xf);

            if (c == 0x5)
            {
                currentReg = dat;
            }
            else if (c == 0x7)
            {
                switch (currentReg)
                {
                    case 0:
                        byte crcResetCommand = (byte)(dat & 0xe0);
                        byte CommandCode = (byte)(dat & 0x1c);
                        byte RegisterSelect = (byte)(dat & 0x03);
                        Log.WriteLine(LogLevel.Trace, "Write SCC_A WR00:{0:x02}", dat);
                        DivSendSCC(dat);
                        return false;
                    case 1:
                        throw new NotImplementedException();
                    case 2:
                        throw new NotImplementedException();
                    case 3:
                        throw new NotImplementedException();
                    case 8:
                        throw new NotImplementedException();
                    case 10:
                        throw new NotImplementedException();
                    case 12:
                        throw new NotImplementedException();
                    case 13:
                        throw new NotImplementedException();
                    case 15:
                        throw new NotImplementedException();
                }
                ;
                currentReg = 0;
            }
            else
            {
                throw new NotImplementedException();
            }
            return false;
        }

        int currentMIDI = 0;
        bool changeMIDI = false;
        private void DivSendSCC(byte dat)
        {
            switch (dat)
            {
                case 0xf5:
                    changeMIDI = true;
                    return;
                default:
                    if (changeMIDI)
                    {
                        currentMIDI = dat == 1 ? 0 : 1;
                        changeMIDI = false;
                        return;
                    }
                    break;
            }

            SCC(currentMIDI, dat);
        }
    }
}
