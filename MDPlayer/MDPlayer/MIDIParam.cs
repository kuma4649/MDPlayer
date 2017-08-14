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
        public byte[][] pc = null;
        public byte[] cPress = null;
        public short[] bend = null;
        public int[][] level = null;

        private byte[] msg = null;
        private int msgInd = 0;
        private bool NowSystemMsg = false;

        public MIDIParam()
        {
            note = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            keyPress = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            cc = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            pc = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            cPress = new byte[16];
            bend = new short[16];
            level = new int[16][] { new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3], new int[3] };

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
                    msgInd = 0;
                    NowSystemMsg = ((d & 0xf0) == 0xf0);
                    if (d == 0xf7 && NowSystemMsg)
                    {
                        analyzeSystemMsg();
                        NowSystemMsg = false;
                    }
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
            if (msgInd == 3)
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

                        int lv = cc[ch][7] * cc[ch][11] * msg[2] >> (7+7);
                        int lvl = (lv * (cc[ch][10] > 64 ? ((127 - cc[ch][10]) * 2) : 127)) >> (7); //65->124 127->0
                        int lvr = (lv * (cc[ch][10] < 64 ? (cc[ch][10] * 2) : 127)) >> (7); // 63->126 0->0
                        level[ch][0] = (byte)lvl;
                        level[ch][1] = (byte)lv;
                        level[ch][2] = (byte)lvr;

                        break;
                    case 0xa0://Key Press
                        keyPress[ch][msg[1]] = msg[2];
                        break;
                    case 0xb0://Control Change
                        cc[ch][msg[1]] = msg[2];
                        break;
                    case 0xc0://Program Change
                        pc[ch][msg[1]] = msg[2];
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

        private void analyzeSystemMsg()
        {
        }

    }
}
