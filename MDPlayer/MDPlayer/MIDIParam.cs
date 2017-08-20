using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class MIDIParam
    {
        public int MIDIModule = 0;//0:GMIDI 1:XG 2:GS
        public byte[][] note = null;
        public string[] notes = null;
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
        public byte[] LCDDisplayLetter = null;
        public int LCDDisplayLetterTime = 0;
        public int LCDDisplayLetterTimeXG = 0;
        public int LCDDisplayLetterLen = 0;

        public byte MasterVolume = 0;

        public int ReverbXG = 1; //HALL1
        public int ChorusXG = 1; //CHORUS1
        public int VariationXG = 15; //DELAY LCR
        public int Insertion1XG = 56;// DISTORTION 
        public int Insertion2XG = 56;//DISTORTION 
        public int Insertion3XG = 56;// DISTORTION 
        public int Insertion4XG = 56;//DISTORTION 

        public int ReverbGS = 4; // Room1(default)
        public int ChorusGS = 2; // Chorus3(default)
        public int DelayGS = 0; // Delay1(default)
        public int EFXGS = 0; // Thru(default)

        public int RevType_MSB = 0;
        public int RevType_LSB = 0;
        public int ChoType_MSB = 0;
        public int ChoType_LSB = 0;
        public int VarType_MSB = 0;
        public int VarType_LSB = 0;
        public int Ins1Type_MSB = 0;
        public int Ins1Type_LSB = 0;
        public int Ins2Type_MSB = 0;
        public int Ins2Type_LSB = 0;
        public int Ins3Type_MSB = 0;
        public int Ins3Type_LSB = 0;
        public int Ins4Type_MSB = 0;
        public int Ins4Type_LSB = 0;
        public int EFXType_MSB = 0;
        public int EFXType_LSB = 0;

        private byte[] msg = null;
        private int msgInd = 0;
        private bool NowSystemMsg = false;

        private int[] tblRevTypeXG = new int[] {
            0x0000
            ,0x0100,0x0101,0x0106,0x0107
            ,0x0200,0x0201,0x0202,0x0205,0x0206,0x0207
            ,0x0300,0x0301
            ,0x0400,0x0407
            ,0x1000
            ,0x1100
            ,0x1200
            ,0x1300
        };

        private int[] tblChoTypeXG = new int[] {
            0x0000
            ,0x4100,0x4101,0x4102,0x4103,0x4104,0x4105,0x4106,0x4107,0x4108
            ,0x4200,0x4201,0x4202,0x4208
            ,0x4300,0x4301,0x4307,0x4308
            ,0x4400
            ,0x4800
            ,0x5700
        };

        private int[] tblVarInsTypeXG = new int[] {
0x0000
,0x0100,0x0101,0x0106,0x0107
,0x0200,0x0201,0x0202,0x0205,0x0206,0x0207
,0x0300,0x0301
,0x0400,0x0407
,0x0500
,0x0600
,0x0700
,0x0800
,0x0900,0x0901
,0x0a00
,0x0b00
,0x1000
,0x1100
,0x1200
,0x1300
,0x1400,0x1401,0x1402
,0x4100,0x4101,0x4102,0x4103,0x4104,0x4105,0x4106,0x4107,0x4108
,0x4200,0x4201,0x4202,0x4208
,0x4300,0x4301,0x4307,0x4308
,0x4400
,0x4500,0x4501,0x4502,0x4503
,0x4600
,0x4700
,0x4800,0x4808
,0x4900,0x4901,0x4908
,0x4a00,0x4a08
,0x4b00,0x4b08
,0x4c00
,0x4d00
,0x4e00,0x4e01,0x4e02
,0x5000,0x5001
,0x5100
,0x5200,0x5201,0x5202,0x5208
,0x5300
,0x5400
,0x5500
,0x5600,0x5601,0x5602,0x5603
,0x5700
,0x5800
,0x5d00
,0x5e00
,0x5f00,0x5f01
,0x6000,0x6001
,0x6100,0x6101
,0x6200,0x6201,0x6202,0x6203
,0x6300,0x6301
        };

        private int[] tblIns1TypeFromEFX = new int[] {
            0x0000,0x0100,0x0101,0x0102,0x0103,0x0110,0x0111,0x0120
            ,0x0121,0x0122,0x0123,0x0124,0x0125,0x0126,0x0130,0x0131
            ,0x0140,0x0141,0x0142,0x0143,0x0144,0x0150,0x0151,0x0152
            ,0x0153,0x0154,0x0155,0x0156,0x0157,0x0160,0x0161,0x0170
            ,0x0171,0x0172,0x0173,0x0200,0x0201,0x0202,0x0203,0x0204
            ,0x0205,0x0206,0x0207,0x0208,0x0209,0x020a,0x020b,0x0300
            ,0x0400,0x0401,0x0402,0x0403,0x0404,0x0405,0x0406,0x0500
            ,0x1100,0x1101,0x1102,0x1103,0x1104,0x1105,0x1106,0x1107,0x1108
        };


        public MIDIParam()
        {
            note = new byte[16][] { new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256], new byte[256] };
            notes = new string[16];
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

            LCDDisplayLetter = new byte[32];
            LCDDisplayLetterTime = 0;
            LCDDisplayLetterTimeXG = 0;

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
                notes[ch] = "          ";

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
            else if (manufactureID == 0x7f)//universal realtime message
            {
                if (msg[2] == 0x7f && msg[3] == 0x04 && msg[4] == 0x01 && msg[7] == 0xf7)
                {
                    MasterVolume = msg[5];
                }
                return;
            }

            while (ptr < msg.Length && msg[ptr] != 0xf7)
            {
                byte dat = msg[ptr];

                if (manufactureID == 0x43) //YAMAHA
                {
                    if (adr == 0x020100 || adr == 0x020101)
                    {
                        //REVERB TYPE MSB/LSB
                        if (adr == 0x020100) RevType_MSB = dat & 0xff;
                        else RevType_LSB = dat & 0xff;
                        ReverbXG = getRevTypeXG();
                    }
                    else if (adr == 0x020120 || adr == 0x020121)
                    {
                        //CHORUS TYPE MSB/LSB
                        if (adr == 0x020120) ChoType_MSB = dat & 0xff;
                        else ChoType_LSB = dat & 0xff;
                        ChorusXG = getChoTypeXG();
                    }
                    else if (adr == 0x020140 || adr == 0x020141)
                    {
                        //VARIATION TYPE MSB/LSB
                        if (adr == 0x020140) VarType_MSB = dat & 0xff;
                        else VarType_LSB = dat & 0xff;
                        VariationXG = getVarTypeXG();
                    }
                    else if (adr == 0x030000 || adr == 0x030001)
                    {
                        //INSERTION EFFECT1 TYPE MSB/LSB
                        if (adr == 0x030000) Ins1Type_MSB = dat & 0xff;
                        else Ins1Type_LSB = dat & 0xff;
                        Insertion1XG = getIns1TypeXG();
                    }
                    else if (adr == 0x030100 || adr == 0x030101)
                    {
                        //INSERTION EFFECT2 TYPE MSB/LSB
                        if (adr == 0x030100) Ins2Type_MSB = dat & 0xff;
                        else Ins2Type_LSB = dat & 0xff;
                        Insertion2XG = getIns2TypeXG();
                    }
                    else if (adr == 0x030200 || adr == 0x030201)
                    {
                        //INSERTION EFFECT3 TYPE MSB/LSB
                        if (adr == 0x030200) Ins3Type_MSB = dat & 0xff;
                        else Ins3Type_LSB = dat & 0xff;
                        Insertion3XG = getIns3TypeXG();
                    }
                    else if (adr == 0x030300 || adr == 0x030301)
                    {
                        //INSERTION EFFECT4 TYPE MSB/LSB
                        if (adr == 0x030300) Ins4Type_MSB = dat & 0xff;
                        else Ins4Type_LSB = dat & 0xff;
                        Insertion4XG = getIns4TypeXG();
                    }
                    else if (adr >= 0x060000 && adr <= 0x06001f)
                    {
                        if (adr == 0x060000) for (int i = 0; i < 32; i++) LCDDisplayLetter[i] = 0x20;

                        //DISPLAY LETTER Data
                        dat = (byte)((dat < 0x20 || dat > 0x7f) ? 0x20 : dat);
                        LCDDisplayLetter[adr & 0x1f] = dat;
                        LCDDisplayLetterLen = (adr & 0x1f) + 1;
                        LCDDisplayLetterTime = 400;
                        if (LCDDisplayLetterLen > 16) LCDDisplayLetterTime = 40;
                    }
                    else if (adr >= 0x070000 && adr <= 0x07002f)
                    {
                        //DISPLAY Dot Data
                        LCDDisplay[adr & 0x3f] = dat;
                        if (adr == 0x07002f)
                        {
                            LCDDisplayTimeXG = 400;
                        }
                    }
                }
                else if (manufactureID == 0x41) //GS
                {
                    if (adr >= 0x100000 && adr <= 0x10001f)
                    {
                        if (adr == 0x100000) for (int i = 0; i < 32; i++) LCDDisplayLetter[i] = 0x20;

                        //DISPLAY LETTER Data
                        dat = (byte)((dat < 0x20 || dat > 0x7f) ? 0x20 : dat);
                        LCDDisplayLetter[adr & 0x1f] = dat;
                        LCDDisplayLetterLen = (adr & 0x1f);// + 1;
                        LCDDisplayLetterTime = 400;
                        if (LCDDisplayLetterLen > 16) LCDDisplayLetterTime = 40;
                    }
                    else if (adr >= 0x100100 && adr <= 0x10013f)
                    {
                        //DISPLAY Dot Data
                        LCDDisplay[adr & 0x3f] = dat;
                        if (adr == 0x10013f)
                        {
                            LCDDisplayTime = 400;
                        }
                    }
                    else if (adr == 0x400130)
                    {
                        //REVERB MACRO
                        if (dat >= 0 && dat <= 7) ReverbGS = dat;
                    }
                    else if (adr == 0x400138)
                    {
                        //CHORUS MACRO
                        if (dat >= 0 && dat <= 7) ChorusGS = dat;
                    }
                    else if (adr == 0x400150)
                    {
                        //Delay MACRO
                        if (dat >= 0 && dat <= 9) DelayGS = dat;
                    }
                    else if (adr == 0x400300)
                    {
                        //EFX Type
                        EFXType_MSB = dat & 0xff;
                        EFXGS = getIns1TypeFromEFX();
                    }
                    else if (adr == 0x400301)
                    {
                        //EFX Type
                        EFXType_LSB = dat & 0xff;
                        EFXGS = getIns1TypeFromEFX();
                    }
                }


                ptr++;
                adr++;
            }

        }

        private int getRevTypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblRevTypeXG.Length; i++)
            {
                if (RevType_MSB * 0x100 + RevType_LSB == tblRevTypeXG[i]) return i;
            }

            for (int i = 0; i < tblRevTypeXG.Length; i++)
            {
                if (RevType_MSB * 0x100 == tblRevTypeXG[i]) return i;
            }

            return ret;
        }

        private int getChoTypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblChoTypeXG.Length; i++)
            {
                if (ChoType_MSB * 0x100 + ChoType_LSB == tblChoTypeXG[i]) return i;
            }

            for (int i = 0; i < tblChoTypeXG.Length; i++)
            {
                if (ChoType_MSB * 0x100 == tblChoTypeXG[i]) return i;
            }

            return ret;
        }

        private int getVarTypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (VarType_MSB * 0x100 + VarType_LSB == tblVarInsTypeXG[i]) return i;
            }

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (VarType_MSB * 0x100 == tblVarInsTypeXG[i]) return i;
            }

            return ret;
        }

        private int getIns1TypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins1Type_MSB * 0x100 + Ins1Type_LSB == tblVarInsTypeXG[i]) return i;
            }

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins1Type_MSB * 0x100 == tblVarInsTypeXG[i]) return i;
            }

            return ret;
        }

        private int getIns2TypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins2Type_MSB * 0x100 + Ins2Type_LSB == tblVarInsTypeXG[i]) return i;
            }

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins2Type_MSB * 0x100 == tblVarInsTypeXG[i]) return i;
            }

            return ret;
        }

        private int getIns3TypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins3Type_MSB * 0x100 + Ins3Type_LSB == tblVarInsTypeXG[i]) return i;
            }

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins3Type_MSB * 0x100 == tblVarInsTypeXG[i]) return i;
            }

            return ret;
        }

        private int getIns4TypeXG()
        {
            int ret = 0;

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins4Type_MSB * 0x100 + Ins4Type_LSB == tblVarInsTypeXG[i]) return i;
            }

            for (int i = 0; i < tblVarInsTypeXG.Length; i++)
            {
                if (Ins4Type_MSB * 0x100 == tblVarInsTypeXG[i]) return i;
            }

            return ret;
        }

        private int getIns1TypeFromEFX()
        {
            int ret = 0;

            for (int i = 0; i < tblIns1TypeFromEFX.Length; i++)
            {
                if (EFXType_MSB * 0x100 + EFXType_LSB == tblIns1TypeFromEFX[i]) return i;
            }

            return ret;
        }

    }
}
