using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZMS.nise68
{
    //参考:
    // Outside X68000

    public class midiBoard
    {
        private int num = 0;//インターフェイス番号
        private Func<int, byte, int> MIDI;
        private byte group = 0;
        private byte interrupt = 0;
        private byte vect = 0;
        private byte intMask = 0xff;

        private byte[] reg = new byte[0x100];
        private Action<byte>[] cmdw = null;
        private Func<byte>[] cmdr = null;
        private ushort generalTimerValue = 0;
        private ushort midiClockTimerValue = 0;
        private int renderingFreq;
        //private double clkM = 4_915_200.0 / 8.0;// 1_000_000.0;
        private double clkM = 1_000_000.0;
        private double stepM;
        private double generalTimerValueWrk = 0.0;
        private double clickcounter = 1.0;

        public midiBoard(int num, int renderingFreq, Func<int, byte, int> MIDI)
        {
            this.num = num;
            this.MIDI = MIDI;
            cmdw = new Action<byte>[0x100]
            {
            //0x00-
            null,null,null,null, setVectH,setIntModeControl,setIntEnable,null,
            null,null,null,null, null,null,null,null,
            //0x10-
            null,null,null,null, setMIDIRealtimeMessageControl,null,null,null,
            null,null,null,null, null,null,null,null,
            //0x20-
            null,null,null,null, setRxCommRate,setRxCommMode,null,null,
            null,null,null,null, null,null,null,null,
            //0x30-
            null,null,null,null, null,setFIFO_RXControl,null,null,
            null,null,null,null, null,null,null,null,
            //0x40-
            null,null,null,null, setTxCommRate,null,null,null,
            null,null,null,null, null,null,null,null,
            //0x50-
            null,null,null,null, null,setFIFO_TXControl,setFIFO_TxData,null,
            null,null,null,null, null,null,null,null,
            //0x60-
            null,null,null,null, null,setFSKControl,setClickCounterControl,setClickCounter,
            null,null,null,null, null,null,null,null,
            //0x70-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x80-
            null,null,null,null, 
            setGeneralTimerValueL , setGeneralTimerValueH ,
            setMIDIClockTimerValueL , setMIDIClockTimerValueH ,
            null,null,null,null, null,null,null,null,
            //0x90-
            null,null,null,null, setExternalIOdirection,null,null,null,
            null,null,null,null, null,null,null,null,
            //0xa0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xb0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xc0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xd0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xe0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xf0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null
            };
            cmdr = new Func<byte>[0x100]
            {
            //0x00-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x10-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x20-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x30-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x40-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x50-
            null,null,null,null, getFIFI_TxStatus,null,null,null,null,null,null,null, null,null,null,null,
            //0x60-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x70-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x80-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0x90-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xa0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xb0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xc0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xd0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xe0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null,
            //0xf0-
            null,null,null,null, null,null,null,null,null,null,null,null, null,null,null,null
            };

            setClock(renderingFreq);
        }

        public void setClock(int renderingFreq)
        {
            this.renderingFreq = renderingFreq;
            stepM = clkM / (double)renderingFreq;
        }

        public byte Read(byte ptr)
        {
            int n = (ptr & 0x10) != 0 ? 1 : 0;
            if (num != n)
            {
                throw new ArgumentOutOfRangeException();
            }

            int c = (int)(ptr & 0xf);
            byte dat = 0;
            if (c == 0x3)//rgr
            {
                return dat;
                throw new Exception();//R01 is write only.
            }
            else if (c == 0x7)//
            {
                ;
            }
            else if (c > 0x8)
            {
                //R04,14,24,34,44,54,64,74,84,94 grp4
                //R05,15,25,35,45,55,65,75,85,95
                //R06,16,26,36,56,66,76,86,96 grp6
                //R17,27,67,77,87
                int r = (group << 4) + ((c - 1) >> 1);
                if (cmdr[r] != null) dat = cmdr[r]();
                else throw new NotImplementedException();
            }
            else
            {
                return dat;
                throw new NotImplementedException();
            }
            Log.WriteLine(LogLevel.Trace2, "Read CZ-6BM1 {2} Adr:$00ea_fa{0:x02} Dat:${1:x02}", ptr, dat, num == 0 ? "Pri" : "Sec");
            return dat;
        }

        public bool Write(byte ptr, byte dat)
        {
            int n = (ptr & 0x10) != 0 ? 1 : 0;
            if (num != n)
            {
                throw new ArgumentOutOfRangeException();
            }

            int c = (int)(ptr & 0xf);
            //Log.WriteLine(LogLevel.Trace2, "Write CZ-6BM1 {2} Adr:${0:x08} Dat:${1:x02}", ptr, dat, n == 0 ? "Pri" : "Sec");

            if (c == 0x1)//R00
            {
                throw new Exception();//R00 is read only.
            }
            else if (c == 0x3)//R01 rgr
            {
                group = (byte)(dat & 0xf);
                if ((dat & 0x80) != 0)
                {
                    Reset();
                }
            }
            else if (c == 0x5)//R02
            {
                throw new Exception();//R02 is read only.
            }
            else if (c == 0x7)//R03
            {
                interrupt = dat;
            }
            else if (c > 0x8)
            {
                //R04,14,24,34,44,54,64,74,84,94 grp4
                //R05,15,25,35,45,55,65,75,85,95
                //R06,16,26,36,56,66,76,86,96 grp6
                //R17,27,67,77,87
                int r = (group << 4) + ((c - 1) >> 1);
                reg[r] = dat;
                if (cmdw[r] != null) cmdw[r]!(dat);
                else throw new NotImplementedException(string.Format("未実装 R{0:x02} : {1:x02}", r, dat));
            }
            else
            {
                throw new NotImplementedException();
            }
            return false;
        }

        public void Timer()
        {

        }

        public bool IntTimer()
        {
            generalTimerValueWrk -= stepM;// / clickcounter;
            bool ret = false;
            while(generalTimerValueWrk <= 0.0)
            {
                generalTimerValueWrk += (double)((generalTimerValue & 0x3fff)<<3);
                ret = true;
            }
            return ret;
        }

        private void setGeneralTimerValueL(byte obj)
        {
            generalTimerValue &= 0b1011_1111_0000_0000;
            generalTimerValue |= obj;
        }

        private void setGeneralTimerValueH(byte obj)
        {
            generalTimerValue &= 0b0000_0000_1111_1111;
            generalTimerValue |= (ushort)((obj << 8) & 0b1011_1111_0000_0000);
        }
        
        private void setMIDIClockTimerValueL(byte obj)
        {
            midiClockTimerValue &= 0b1011_1111_0000_0000;
            midiClockTimerValue |= obj;
        }

        private void setMIDIClockTimerValueH(byte obj)
        {
            midiClockTimerValue &= 0b0000_0000_1111_1111;
            midiClockTimerValue |= (ushort)((obj << 8) & 0b1011_1111_0000_0000);
        }

        private void Reset()
        {
            intMask = 0xff;
        }

        // R04
        private void setVectH(byte dat)
        {
            vect = (byte)((vect & 0x1f) | (dat & 0xe0));
        }

        // R05
        private void setIntModeControl(byte dat)
        {
            //今のところ無視
            ;
        }

        // R06
        private void setIntEnable(byte dat)
        {
            intMask = (byte)~dat;
        }

        // R14
        private void setMIDIRealtimeMessageControl(byte dat)
        {
            //今のところ無視
            ;
        }


        // R24
        private void setRxCommRate(byte dat)
        {
            //今のところ無視
            ;
        }

        // R25
        private void setRxCommMode(byte dat)
        {
            //今のところ無視
            ;
        }

        // R35
        private void setFIFO_RXControl(byte dat)
        {
            //今のところ無視
            ;
        }

        // R44
        private void setTxCommRate(byte dat)
        {
            //今のところ無視
            ;

        }

        // R55
        private void setFIFO_TXControl(byte dat)
        {
            //今のところ無視
            ;
        }

        // R56
        private void setFIFO_TxData(byte dat)
        {
            Log.WriteLine(LogLevel.Trace2, "CZ-6BM1 {0} Send MIDI Data:${1:x02}", num == 0 ? "Pri" : "Sec", dat);
            MIDI(num,dat);
        }

        // R65
        private void setFSKControl(byte dat)
        {
            //今のところ無視
            ;
        }

        // R66
        private void setClickCounterControl(byte dat)
        {
            if ((dat & 2) != 0) clkM = 1_000_000.0;
            else clkM = 1_000_000.0 / 2.0;
            stepM = clkM / (double)renderingFreq;
        }

        // R67
        private void setClickCounter(byte dat)
        {
            //今のところ無視
            ;
            clickcounter = dat & 0x7f;//関係なさそう...
        }

        // R94
        private void setExternalIOdirection(byte dat)
        {
            //今のところ無視
            ;
        }

        private byte getFIFI_TxStatus()
        {
            //bit 7 :1 送信FIFOは空
            //bit 6 :1 送信FIFOは空きあり
            return 0b1100_0000;
        }

    }
}
