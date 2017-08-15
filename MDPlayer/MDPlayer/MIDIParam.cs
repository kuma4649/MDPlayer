using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class MIDIParam
    {
        public byte[][] note = null;
        public byte[][] keyPress = null;
        public byte[][] cc = null;
        public byte[] pc = null;
        public byte[] cPress = null;
        public short[] bend = null;
        public int[][] level = null;
        public byte[] nrpnVibRate = null;
        public byte[] nrpnVibDepth = null;
        public byte[] nrpnVibDelay = null;
        public byte[] nrpnLPF = null;
        public byte[] nrpnLPFRsn = null;
        public byte[] nrpnHPF = null;
        public byte[] nrpnEQBaseGain = null;
        public byte[] nrpnEQTrebleGain = null;
        public byte[] nrpnEQBaseFrq = null;
        public byte[] nrpnEQTrebleFrq = null;
        public byte[] nrpnEGAttack = null;
        public byte[] nrpnEGDecay = null;
        public byte[] nrpnEGRls = null;
        public byte[] LCDDisplay = null;
        public int LCDDisplayTime = 0;
        public int LCDDisplayTimeXG = 0;

        private byte[] msg = null;
        private int msgInd = 0;
        private bool NowSystemMsg = false;

        public MIDIParam()
        {
            note = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            keyPress = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            cc = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            pc = new byte[16];
            cPress = new byte[16];
            bend = new short[16];
            level = new int[16][] { new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5], new int[5] };
            nrpnVibRate = new byte[16];
            nrpnVibDepth = new byte[16];
            nrpnVibDelay = new byte[16];
            nrpnLPF = new byte[16];
            nrpnLPFRsn = new byte[16];
            nrpnHPF = new byte[16];
            nrpnEQBaseGain = new byte[16];
            nrpnEQTrebleGain = new byte[16];
            nrpnEQBaseFrq = new byte[16];
            nrpnEQTrebleFrq = new byte[16];
            nrpnEGAttack = new byte[16];
            nrpnEGDecay = new byte[16];
            nrpnEGRls = new byte[16];
            LCDDisplay = new byte[64];
            LCDDisplayTime = 0;
            LCDDisplayTimeXG = 0;

            msg = new byte[256];
            msgInd = 0;
            NowSystemMsg = false;

            for (int ch = 0; ch < 16; ch++)
            {
                for (int n = 0; n < 256; n++)
                {
                    note[ch][n] = 0;
                }
                cc[ch][7] = 110;
                cc[ch][10] = 64;
                cc[ch][11] = 110;
                bend[ch] = 8192;

                //nrpnVibRate[ch] = 64;
                //nrpnVibDepth[ch] = 64;
                //nrpnVibDelay[ch] = 64;
                //nrpnLPF[ch] = 64;
                //nrpnLPFRsn[ch] = 64;
                //nrpnHPF[ch] = 64;
                //nrpnEQBaseGain[ch] = 64;
                //nrpnEQTrebleGain[ch] = 64;
                //nrpnEQBaseFrq[ch] = 64;
                //nrpnEQTrebleFrq[ch] = 64;
                //nrpnEGAttack[ch] = 64;
                //nrpnEGDecay[ch] = 64;
                //nrpnEGRls[ch] = 64;

            }
        }

        public void SendBuffer(byte[] dat)
        {
            foreach (byte d in dat)
            {
                bool IsStatusByte = (d & 0x80) != 0;

                if (IsStatusByte)
                {
                    //Console.WriteLine("");
                    NowSystemMsg = ((d & 0xf0) == 0xf0);
                    if (d == 0xf7 && NowSystemMsg)
                    {
                        if (msgInd < msg.Length) msg[msgInd] = 0xf7;
                        analyzeSystemMsg();
                        NowSystemMsg = false;
                    }
                    msgInd = 0;
                }

                if (msgInd < msg.Length)
                {
                    msg[msgInd] = d;
                    //Console.Write("{0:X2}:", msg[msgInd]);
                    msgInd++;
                }

                analyze();

            }
        }

        private void analyze()
        {
            if (msgInd == 2)
            {
                byte cmd = (byte)(msg[0] & 0xf0);
                byte ch = (byte)(msg[0] & 0xf);
                switch (cmd)
                {
                    case 0xc0://Program Change
                        pc[ch] = msg[1];
                        break;
                }
            }
            else if (msgInd == 3)
            {
                byte cmd = (byte)(msg[0] & 0xf0);
                byte ch = (byte)(msg[0] & 0xf);
                switch (cmd)
                {
                    case 0x80://Note OFF
                        note[ch][msg[1]] = 0;
                        break;
                    case 0x90://Note ON
                        note[ch][msg[1]] = msg[2];

                        if (msg[2] != 0)//NOTE OFF の代用の場合は液晶パラメータの更新を行わない
                        {
                            int lv = cc[ch][7] * cc[ch][11] * msg[2] >> (7 + 7);
                            int lvl = (lv * (cc[ch][10] > 64 ? ((127 - cc[ch][10]) * 2) : 127)) >> (7); //65->124 127->0
                            int lvr = (lv * (cc[ch][10] < 64 ? (cc[ch][10] * 2) : 127)) >> (7); // 63->126 0->0
                            level[ch][0] = (byte)lvl;
                            level[ch][1] = (byte)lv;
                            level[ch][2] = (byte)lvr;
                            if (level[ch][3] < lv)
                            {
                                level[ch][3] = (byte)lv;
                                level[ch][4] = 100;
                            }
                        }
                        break;
                    case 0xa0://Key Press
                        keyPress[ch][msg[1]] = msg[2];
                        break;
                    case 0xb0://Control Change
                        cc[ch][msg[1]] = msg[2];
                        analyzeControlChange(ch);
                        break;
                    case 0xd0://Ch Press
                        cPress[ch] = msg[1];
                        break;
                    case 0xe0://Pitch Bend
                        bend[ch] = (short)(msg[2] * 0x80 + msg[1]);
                        break;
                }
            }
        }

        private void analyzeControlChange(byte ch)
        {
            switch (msg[1])
            {
                case 0x06://Data Entry MSB
                    analyzeDataEntryMSB(ch);
                    break;
                case 0x26://Data Entry LSB
                    break;
            }
        }

        private void analyzeDataEntryMSB(byte ch)
        {
            switch (cc[ch][0x63])//NRPN MSB
            {
                case 0x01:
                    switch (cc[ch][0x62]) //NRPN LSB
                    {
                        case 0x08:
                            nrpnVibRate[ch] = cc[ch][0x06];
                            break;
                        case 0x09:
                            nrpnVibDepth[ch] = cc[ch][0x06];
                            break;
                        case 0x0a:
                            nrpnVibDelay[ch] = cc[ch][0x06];
                            break;
                        case 0x20:
                            nrpnLPF[ch] = cc[ch][0x06];
                            break;
                        case 0x21:
                            nrpnLPFRsn[ch] = cc[ch][0x06];
                            break;
                        case 0x24:
                            nrpnHPF[ch] = cc[ch][0x06];
                            break;
                        case 0x30:
                            nrpnEQBaseGain[ch] = cc[ch][0x06];
                            break;
                        case 0x31:
                            nrpnEQTrebleGain[ch] = cc[ch][0x06];
                            break;
                        case 0x34:
                            nrpnEQBaseFrq[ch] = cc[ch][0x06];
                            break;
                        case 0x35:
                            nrpnEQTrebleFrq[ch] = cc[ch][0x06];
                            break;
                        case 0x63:
                            nrpnEGAttack[ch] = cc[ch][0x06];
                            break;
                        case 0x64:
                            nrpnEGDecay[ch] = cc[ch][0x06];
                            break;
                        case 0x66:
                            nrpnEGRls[ch] = cc[ch][0x06];
                            break;
                    }
                    break;
            }
        }

        private void analyzeSystemMsg()
        {

            if (msg[0] != 0xf0) return;
            if (msgInd < 8) return;

            byte manufactureID = msg[1];
            int adr = 0;
            int ptr = 0;

            if (manufactureID == 0x43)//YAMAHA ID?
            {
                if ((msg[2] & 0xf0) != 0x10) return;
                if (msg[3] != 0x4c) return;
                adr = msg[4] * 0x10000 + msg[5] * 0x100 + msg[6];
                ptr = 7;
            }
            else if (manufactureID == 0x41)//Roland ID
            {
                //if (msg[3] != 0x42) return;
                //if (msg[4] != 0x12) return;
                adr = msg[5] * 0x10000 + msg[6] * 0x100 + msg[7];
                ptr = 8;
            }

            while (ptr < msg.Length && msg[ptr] != 0xf7)
            {
                byte dat = msg[ptr];

                if (manufactureID == 0x43)
                {
                    if (adr >= 0x070000 && adr <= 0x07002f)
                    {
                        //DISPLAY Dot Data
                        LCDDisplay[adr & 0x3f] = dat;
                        if (adr == 0x07002f)
                        {
                            LCDDisplayTimeXG = 300;
                        }
                    }
                }
                else if (manufactureID == 0x41)
                {
                    if (adr >= 0x100100 && adr <= 0x10013f)
                    {
                        //DISPLAY Dot Data
                        LCDDisplay[adr & 0x3f] = dat;
                        if (adr == 0x10013f)
                        {
                            LCDDisplayTime = 300;
                        }
                    }
                }


                ptr++;
                adr++;
            }

        }

    }
}
