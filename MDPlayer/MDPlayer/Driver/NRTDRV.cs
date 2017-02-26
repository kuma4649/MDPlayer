using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MDPlayer
{
    public class NRTDRV : baseDriver
    {

        private byte[] ram;
        private Work work = new Work();

        private byte[] KTABLE = new byte[] {
            // C    C+   D    D+   E    F    F+   G    G+   A    A+   B
             0x00,0x00,0x00,0x00,0x01,0x02,0x04,0x05,0x06,0x08,0x09,0x0A	//o0
            ,0x0C,0x0D,0x0E,0x10,0x11,0x12,0x14,0x15,0x16,0x18,0x19,0x1A	//o1
            ,0x1C,0x1D,0x1E,0x20,0x21,0x22,0x24,0x25,0x26,0x28,0x29,0x2A	//o2
            ,0x2C,0x2D,0x2E,0x30,0x31,0x32,0x34,0x35,0x36,0x38,0x39,0x3A	//o3
            ,0x3C,0x3D,0x3E,0x40,0x41,0x42,0x44,0x45,0x46,0x48,0x49,0x4A	//o4
            ,0x4C,0x4D,0x4E,0x50,0x51,0x52,0x54,0x55,0x56,0x58,0x59,0x5A	//o5
            ,0x5C,0x5D,0x5E,0x60,0x61,0x62,0x64,0x65,0x66,0x68,0x69,0x6A	//o6
            ,0x6C,0x6D,0x6E,0x70,0x71,0x72,0x74,0x75,0x76,0x78,0x79,0x7A	//o7
            ,0x7C,0x7D,0x7E,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F	//o8
            ,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F,0x7F    //o9
        };

        private ushort[] PTABLE = new ushort[] {
		    // C    C+   D    D+   E    F    F+   G    G+   A    A+   B
        	 4095,4036,3980,3978,3924,3894,3868,3812,3756,3700,3644,3588	//o0
            ,3532,3476,3420,3228,3047,2876,2715,2562,2419,2283,2155,2034	//o1
            ,1920,1812,1711,1614,1524,1438,1358,1281,1210,1142,1078,1017	//o2
            , 960, 906, 855, 807, 762, 719, 679, 641, 605, 571, 539, 509	//o3
            , 480, 453, 428, 404, 381, 360, 339, 320, 302, 285, 269, 254	//o4
            , 240, 227, 214, 202, 190, 180, 170, 160, 151, 143, 135, 127	//o5
            , 120, 113, 107, 101,  95,  90,  85,  80,  76,  71,  67,  64	//o6
            ,  60,  57,  53,  50,  48,  45,  42,  40,  38,  36,  34,  32	//o7
            ,  30,  28,  27,  25,  24,  22,  21,  20,  19,  18,  17,  16	//o8
            ,  15,  14,  13,  12,  11,  10,   9,   8,   7,   6,   5,   4	//o9
        };

        private byte[] PMTBL = new byte[] {
             0x3f,0x09 //Ch.1/P0
            ,0x3e,0x08 //Ch.1/P1
            ,0x37,0x01 //Ch.1/P2
            ,0x36,0x00 //Ch.1/P3

            ,0x3f,0x12 //Ch.2/P0
            ,0x3d,0x10 //Ch.2/P1
            ,0x2f,0x02 //Ch.2/P2
            ,0x2d,0x00 //Ch.2/P3

            ,0x3f,0x24 //Ch.3/P0
            ,0x3b,0x20 //Ch.3/P1
            ,0x1f,0x04 //Ch.3/P2
            ,0x1b,0x00 //Ch.3/P3
        };

        private byte[] TTONE = new byte[] {
            162,1,0                 //+3
            , 75,0 ,74,0 ,74,0 ,74,0    //+8
            , 74,0 ,74,0 ,74,0 ,74,0    //+8
            ,126,0 ,74,0 ,74,0 ,74,0    //+8
            , 74,0 ,74,0 ,74,0 ,74,0    //+8
            ,175,0 ,74,0 ,74,0          //+6
            ,15,255,2               //+3 PVX
            ,0x3c                   //+1 FVX
            ,0x18                   //+1
            ,0x02,0x00,0x01,0x00    //+4
            ,0x1c,0x7f,0x02,0x81    //+4
            ,0x1e,0x00,0x1f,0x00    //+4
            ,0x00,0x00,0x00,0x00    //+4
            ,0x00,0x00,0x00,0x00    //+4
            ,0x0f,0xff,0x0f,0xff    //+4
            ,0x1c,0x7f,0x00,0x7f    //+4
            ,126                    //+1 TR0
            ,3,125,14               //+3 TR1
            ,44,0                   //+2
            ,176,24,178,24,180,24,181,24,183,24,185,24,187,24,188,24,0,255,129,0,48 //+21
            ,188,24,187,24,0,96,178,24,176,24,0,48,176,192,0,48,25,6    //+18
            ,188,24,0,72,127        //+5 TR1L
            ,119,0                  //+2
            ,3,125,14               //+3 TR2
            ,44,0                   //+2
            ,0,255,129,176,24,178,24,180,24,181,24,183,24,185,24,187,24,188,24,0,48 //+21
            ,0,96,181,24,180,24,0,192,183,96,0,48,25,6 //+14
            ,0,48,188,24,0,24,127   //+7 TR2L
            ,166,0                  //+2 
            ,14                     //+1 TR3
            ,41,0                   //+2
            ,19,112,0,192,176,24,178,24,180,24,181,24,183,24,185,24,187,24,188,24,0,192 //+22
            ,0,96,185,24,183,24,0,192,180,144,0,48,25,6 //+14
            ,0,24,188,24,0,48,127   //+7 TR3L
            ,214,0                  //+2
        };

        public override bool init(byte[] nrdFileData, ChipRegister chipRegister, enmModel model, enmUseChip useChip, uint latency)
        {

            this.vgmBuf = nrdFileData;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;

            GD3 = getGD3Info(nrdFileData, 42);
            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = 0;

            try
            {
                ram = new byte[65536];
                Array.Clear(ram, 0, ram.Length);

                Array.Copy(vgmBuf, 0, ram, 0x4000, Math.Min(vgmBuf.Length, 0xfeff - 0x4000));
            }
            catch (Exception ex)
            {
                throw new Exception("Driverの初期化に失敗しました。", ex);
            }

            //Driverの初期化
            Call(0);

            return true;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 gd3 = new GD3();
            gd3.TrackName = getNRDString(buf, ref vgmGd3);
            gd3.TrackNameJ = getNRDString(buf, ref vgmGd3);
            gd3.Composer = getNRDString(buf, ref vgmGd3);
            gd3.ComposerJ = gd3.Composer;
            gd3.VGMBy = getNRDString(buf, ref vgmGd3);
            gd3.Notes = getNRDString(buf, ref vgmGd3);

            return gd3;
        }

        public void Call(int cmdNo)
        {
            switch (cmdNo)
            {
                case 0:
                case 4:
                    drvini();
                    break;
                case 1:
                    mplay();
                    break;
                case 2:
                    mstop();
                    break;
                case 3:
                    mfade();
                    break;
                case 5:
                    mtest();
                    break;
                case 6:
                    mplayd();
                    break;
            }
        }

        private float CTC0DownCounter = 0.0f;
        private float CTC0DownCounterMAX = 0.0f;
        private bool CTC0Paluse = false;
        private float CTC1DownCounter = 0.0f;
        private float CTC1DownCounterMAX = 0.0f;
        //private bool CTC1Paluse = false;
        private float CTC3DownCounter = 0.0f;
        private float CTC3DownCounterMAX = 0.0f;
        //private bool CTC3Paluse = false;
        private float CTCStep = 4000000.0f / 44100.0f;
        private float CTC1Step = 4000000.0f / 44100.0f;

        public override void oneFrameProc()
        {
            try
            {

                CTC0DownCounterMAX = (work.ctc0timeconstant == 0 ? 0x100 : work.ctc0timeconstant) * ((work.ctc0 & 0x40) == 0 ? ((work.ctc0 & 0x20) != 0 ? 256.0f : 16.0f) : 1);
                CTC1DownCounterMAX = (work.ctc1timeconstant == 0 ? 0x100 : work.ctc1timeconstant) * ((work.ctc1 & 0x40) == 0 ? ((work.ctc1 & 0x20) != 0 ? 256.0f : 16.0f) : 1);
                CTC3DownCounterMAX = (work.ctc3timeconstant == 0 ? 0x100 : work.ctc3timeconstant) * ((work.ctc3 & 0x40) == 0 ? ((work.ctc3 & 0x20) != 0 ? 256.0f : 16.0f) : 1);
                CTC0Paluse = false;
                //CTC3Paluse = false;

                //ctc0
                if ((work.ctc0 & 0x40) == 0)
                {
                    //Timer Mode
                    CTC0DownCounter -= CTCStep;
                }
                else
                {
                    //CounterMode 無し
                    ;
                }

                if (CTC0DownCounter <= 0.0f)
                {
                    CTC0Paluse = true;
                    if ((work.ctc0 & 0x80) != 0)
                    {
                        //Parse();
                    }
                    CTC0DownCounter += CTC0DownCounterMAX;
                }


                //ctc1
                if ((work.ctc1 & 0x40) == 0)
                {
                    //Timer Mode
                    CTC1DownCounter -= CTC1Step;
                }
                else
                {
                    //CounterMode 無し
                    ;
                }

                if (CTC1DownCounter <= 0.0f)
                {
                    //CTC1Paluse = true;
                    if ((work.ctc1 & 0x80) != 0)
                    {
                        int2();
                    }
                    CTC1DownCounter += CTC1DownCounterMAX;
                }


                //ctc3
                if ((work.ctc3 & 0x40) == 0)
                {
                    //Timer Mode
                    CTC3DownCounter -= 1.0f;
                }
                else if (CTC0Paluse)
                {
                    //Counter Mode(ctc0のパルスをカウント)
                    CTC3DownCounter -= 1.0f;
                }

                if (CTC3DownCounter <= 0.0f)
                {
                    //CTC3Paluse = true;
                    if ((work.ctc3 & 0x80) != 0)
                    {
                        imain();
                    }
                    CTC3DownCounter = CTC3DownCounterMAX;
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        private static string getNRDString(byte[] buf, ref uint index)
        {
            if (buf == null || buf.Length < 1 || index < 0 || index >= buf.Length) return "";

            try
            {
                List<byte> lst = new List<byte>();
                for (; buf[index] != 0; index++)
                {
                    lst.Add(buf[index]);
                }

                string n = System.Text.Encoding.GetEncoding(932).GetString(lst.ToArray());
                index++;

                return n;
            }
            catch (Exception e)
            {
                log.ForcedWrite(e);
            }
            return "";
        }

        private void drvini()
        {

            work.ctcflg = 0;
            work.ctcflg |= 2; //OPM2を使用する場合は2　しない場合は0
            work.ctcflg |= 1;//OPM1を使用する場合は1　しない場合は0
            work.ctcflg |= 4;//turbo仕様の場合は4　しない場合は0

            work.ctc3io = 0x1fa3;//turboのctc3のIOポート番号保存　しない場合は0x0707

        }

        private void mplay()
        {
            mstop();

            // DI

            work.ctc0 = 0x27; //TimerMode PreScal=256
            work.ctc0timeconstant = ram[work.bgmadr];
            work.ctc1 = 0xb7;
            work.ctc1timeconstant = 0;
            work.ctc3 = 0xc7; //CounterMode PreScal = none
            work.ctc3timeconstant = ram[work.bgmadr + 1];
            work.MVOL = 0;
            work.FFFLG = 0;
            work.FSPEED = 5;
            work.OPMT02_LightMode = true;
            work.OPMT14_MVMode = false;
            work.PLYFLG &= 0xc0;
            work.PLYFLG |= 0x01;
            work.OPMKeyONEnable = false;
            work.OPMRestEnable = false;
            work.OPMT19Enable = false;
            work.PSGKeyONEnable = false;
            work.PSGRestEnable = false;

            // 割り込みルーチン最後のEI 無効

            imain();

            work.OPMKeyONEnable = true;
            work.PSGKeyONEnable = true;
            work.OPMRestEnable = true;
            work.PSGRestEnable = true;
            work.OPMT19Enable = true;

            // 割り込みルーチン最後のEI 有効

            //EI

        }

        private void mstop()
        {

            //DI

            ushort hl = work.bgmadr;
            hl += 2;
            byte c = ram[hl];

            work.opmflg = (byte)(ram[hl] & 1);//OPMウェイトフラグ保存
            if ((ram[hl] & 2) != 0)
            {
                work.KEYONF_OPMMASK = 0;
                work.PENVF_VOL0 = true;
            }

            work.KEYON_LightMode = false;
            work.PKEYON_LightMode = false; 
            if ((ram[hl] & 4) != 0)
            {
                work.KEYON_LightMode = true;
                work.PKEYON_LightMode = true;
            }

            hl++;
            if ((work.ctcflg & 1) != 0)
            {
                work.OPMIO = 0x701;
                minit(hl);
            }
            hl += 2 * 8;
            if ((work.ctcflg & 2) != 0)
            {
                work.OPMIO = 0x709;
                minit(hl);
            }

            hl = work.bgmadr;
            hl += 35;
            pinit(hl);
            ctcrst();

            work.PLYFLG &= 0xc0;
            work.COUNT = 0;

            //EI
        }

        private void mfade()
        {
            if ((work.PLYFLG & 0x3) == 0) return;
            MFADE1(5);
        }

        private void mtest()
        {
            for (int i = 0; i < TTONE.Length; i++)
            {
                ram[0x4000 + i] = TTONE[i];
            }
            work.bgmadr = 0x4000;
            mplay();
        }

        private void mplayd()
        {
            work.bgmadr = 0x4000;
            mplay();
        }

        private void ctcrst()
        {
            work.ctc3 = 3;
            work.ctc2 = 3;
            work.ctc1 = 3;
            work.ctc0 = 3;
        }

        private void pinit(ushort hl)
        {
            Ch[] wChs = work.PSGChs;
            for (byte b = 0; b < 3; b++)
            {
                ushort de = (ushort)(ram[hl] + ram[hl + 1] * 0x100);
                hl += 2;
                wChs[b].ptrData = (ushort)(de + work.bgmadr);
                wreset(wChs[b]);
                wChs[b].PSGToneStartAdr = 0;
                wChs[b].PSGToneAdr = 0;
                wChs[b].PSGTone = 480;
                wChs[b].PSGHardEnvelopeType = 16;

                byte a = (byte)(work.PLYFLG & 0x80);
                if (a != 0)
                {
                    a = (byte)(wChs[b].LFOFlags & 0x80);
                }
                wChs[b].LFOFlags = a;
            }

            work.PFLG = 0x38;
            byte[] psrtbl = new byte[] { 0xe0, 1, 0xe0, 1, 0xe0, 1, 0, 0x38, 0, 0, 0, 0, 0x10, 0 };
            for(byte d=0;d<psrtbl.Length;d++)
            {
                wpsg(d, psrtbl[d]);
            }
        }

        private void minit(ushort hl)
        {
            lreset();
            wopm(0xf, 0);//OPMレジスタ0FH=0（ノイズ初期化）

            Ch[] wChs = work.OPM1Chs;
            if (work.OPMIO != 0x701)
            {
                wChs = work.OPM2Chs;
            }
            for (byte b = 0; b < 8; b++)
            {
                opmrst(b);
                ushort de = (ushort)(ram[hl] + ram[hl + 1] * 0x100);
                hl += 2;
                wChs[b].ptrData = (ushort)(de + work.bgmadr);
                wreset(wChs[b]);
                wChs[b].KF = 0;
                wChs[b].softAMFlagAndDelay = 0;
                wChs[b].NoteNumber = 48;
                wChs[b].OP1TL = 255;
                wChs[b].OP2TL = 255;
                wChs[b].OP3TL = 255;
                wChs[b].OP4TL = 255;

                byte a = 0x78;
                if ((work.PLYFLG & 0x80) != 0)
                {
                    a = (byte)(wChs[b].LFOFlags & 0xf8);
                }
                wChs[b].LFOFlags = a;
            }
        }

        private void opmrst(byte b)
        {
            //KEY OFF
            wopm(8, b);

            //TL
            wopm((byte)(0x60 + b), 127);
            wopm((byte)(0x68 + b), 127);
            wopm((byte)(0x70 + b), 127);
            wopm((byte)(0x78 + b), 127);

            //SL/RR
            wopm((byte)(0xe0 + b), 255);
            wopm((byte)(0xe8 + b), 255);
            wopm((byte)(0xf0 + b), 255);
            wopm((byte)(0xf8 + b), 255);

            //KC
            wopm((byte)(0x28 + b), 0x3c);
            //KF
            wopm((byte)(0x30 + b), 0);
            //PMS/AMS
            wopm((byte)(0x38 + b), 0);
        }

        private void wreset(Ch ch)
        {
            ch.PanAlgFb = 0xc0;
            ch.Volume = 127;
            ch.q = 8;
            ch.gatetime = 1;
            ch.OP1TLs = 255;
            ch.OP2TLs = 255;
            ch.OP3TLs = 255;
            ch.OP4TLs = 255;
            ch.PSGRRLevel = 15;
            ch.loopCounter = 0;
            ch.Counter = 0;
            ch.Detune = 0;
            ch.MacroReturnAdr = 0;
            ch.LegartFlg = 0;
            ch.NestCount = 0;
            ch.Q = 0;
            ch.isCountNext = 0;
            ch.Transpose = 0;
            ch.PortaFlg = 0;
            ch.PortaTone = 0;
            ch.softPMDelay = 0;
            ch.softPMPitch = 0;
            ch.softPMStep = 0;
            ch.softPMDelayCount = 0;
            ch.softPMStepCount = 0;
            ch.softPMProcCount = 0;
            ch.softPMType = 0;
            ch.softAMDepth = 0;
            ch.softAMStep = 0;
            ch.softAMSelOP = 0;
            ch.softAMDelayCount = 0;
            ch.softAMStepCount = 0;
            ch.softAMProcCount = 0;
            ch.PortaStartFlg = 0;
            ch.LegartDelayFlg = 0;
            ch.KeyoffFlg = 0;
            ch.TrackStopFlg = 0;
            ch.GlideFlg = 0;
            ch.Glide = 0;
            ch.workForPlayer = 0;
            ch.PSGRR = 0;
            ch.PSGRRCounter = 0;
            ch.PSGRRVolOffset = 0;
        }

        private void lreset()
        {
            wopm(1, 2);
            wopm(1, 0);
        }

        private void wopm(byte d, byte a)
        {
            if (work.OPMIO == 0x701)
            {
                //仮想レジスタに書き込み
                work.OPM1vreg[d] = a;
                //実レジスタに書き込み
                chipRegister.setYM2151Register(0, 0, d, a, enmModel.VirtualModel, 0, 0);
                //Console.WriteLine($"OPM1 Reg{d:X2} Dat{a:X2}");
            }
            else
            {
                //仮想レジスタに書き込み
                work.OPM2vreg[d] = a;
                //実レジスタに書き込み
                chipRegister.setYM2151Register(1, 0, d, a, enmModel.VirtualModel, 0, 0);
                //Console.WriteLine($"OPM2 Reg{d:X2} Dat{a:X2}");
            }
        }

        private void wpsg(byte d, byte a)
        {
            //Out(0x1c00, d);//PSG register
            //Out(0x1b00, a);//PSG data
            chipRegister.setAY8910Register(0, d, a, enmModel.VirtualModel);
        }

        private class Work
        {
            public ushort ctcflg=0;
            public ushort ctc3io = 0;
            public byte ctc0 = 0;
            public byte ctc1 = 0;
            public byte ctc2 = 0;
            public byte ctc3 = 0;
            public byte ctc0timeconstant = 0;
            public byte ctc1timeconstant = 0;
            public byte ctc3timeconstant = 0;
            public ushort bgmadr = 0x4000;//Default Address
            public byte opmflg = 0;
            public ushort OPMIO = 0x701;
            public byte PLYFLG = 0;
            public byte COUNT = 0;
            public byte ZCOUNT = 0;
            public byte FFFLG = 0;
            public byte PFLG = 0x38;
            public byte MVOL = 0;
            public byte FCOUNT = 0;
            public byte FSPEED = 5;
            public byte VER = 1;

            public Ch[] OPM1Chs = new Ch[8] { new Ch(), new Ch(), new Ch(), new Ch(), new Ch(), new Ch(), new Ch(), new Ch()};
            public Ch[] OPM2Chs = new Ch[8] { new Ch(), new Ch(), new Ch(), new Ch(), new Ch(), new Ch(), new Ch(), new Ch() };
            public Ch[] PSGChs = new Ch[3] { new Ch(), new Ch(), new Ch()};

            public byte[] OPM1vreg = new byte[256];
            public byte[] OPM2vreg = new byte[256];

            public byte AMD1 = 0;
            public byte PMD1 = 0;
            public byte AMD2 = 0;
            public byte PMD2 = 0;

            // 命令直接書き換え処理対策向けフラグ

            public byte KEYONF_OPMMASK = 0x78;
            public bool PENVF_VOL0 = false;
            public bool KEYON_LightMode = false;
            public bool PKEYON_LightMode = false;
            public bool OPMT02_LightMode = true;
            public bool OPMT14_MVMode = false;
            public bool OPMKeyONEnable = true;
            public bool OPMRestEnable = true;
            public bool OPMT19Enable = true;
            public bool PSGKeyONEnable = true;
            public bool PSGRestEnable = true;
        }

        private class Ch
        {
            public ushort ptrData = 0;//(IX,IX+1)
            public byte loopCounter = 0;//IX+2
            public byte Counter = 0;//IX+3
            public byte Detune = 0;//IX+4
            public ushort MacroReturnAdr = 0;//IX+5,IX+6
            public byte PanAlgFb = 0xc0;//(IX+7)
            public byte OP1TL = 0;//IX+8
            public byte OP2TL = 0;//IX+9
            public byte OP3TL = 0;//IX+10
            public byte OP4TL = 0;//IX+11
            public ushort PSGToneStartAdr = 0;//IX+8,IX+9
            public ushort PSGToneAdr = 0;//IX+10,IX+11
            public byte LegartFlg = 0;//IX+12
            public byte Volume = 127;//IX+13
            public byte NestCount = 0;//IX+14
            public byte q = 8;//q IX+15
            public byte Q = 0;//Q IX+16
            public byte gatetime = 1;//IX+17
            public byte isCountNext = 0;//IX+18
            public byte LFOFlags = 0; //IX+19
            public byte Transpose = 0; //IX+20
            public byte KF = 0;//キーフラクション(IX+21)
            public byte NoteNumber = 0;//ノートナンバー(IX+22)
            public ushort PSGTone = 0;//PSG音程IX+21,IX+22
            public byte PortaFlg = 9;//ポルタメントフラグIX+23
            public ushort PortaTone = 0;//ポルタメント到達音程IX+24,IX+25
            public byte softPMDelay = 0;//ソフトPMディレイ設定値IX+26
            public byte softPMPitch = 0;//ソフトPMピッチ設定値IX+27
            public byte softPMStep = 0;//ソフトPMステップ設定値IX+28
            public byte softPMDelayCount = 0;//ソフトPMディレイカウンタIX+29
            public byte softPMStepCount = 0;//ソフトPMステップカウンタIX+30
            public byte softPMProcCount = 0;//ソフトPMプロセスカウンタIX+31
            public byte softPMType = 0;//ソフトPMステップカウンタIX+32
            public byte softAMFlagAndDelay = 0;//ソフトAMフラグ兼ディレイ設定値 (0=オフ)(IX+33)
            public byte PSGHardEnvelopeType = 16;//PSGハードエンベ形状 (16=ソフトエンベ)(IX+33)
            public byte softAMDepth = 0;//ソフトAM深度設定値IX+34
            public byte softAMStep = 0;//ソフトAMステップ設定値IX+35
            public byte softAMSelOP = 0;//ソフトAM選択OP IX+36
            public byte softAMDelayCount = 0;//ソフトAMディレイカウンタIX+37
            public byte softAMStepCount = 0;//ソフトAMステップカウンタIX+38
            public byte softAMProcCount = 0;//ソフトAMプロセスカウンタIX+39
            public byte OP1TLs = 255;//IX+40
            public byte OP2TLs = 255;//IX+41
            public byte OP3TLs = 255;//IX+42
            public byte OP4TLs = 255;//IX+43
            public byte PortaStartFlg = 0;//ポルタメント動作フラグIX+44
            public byte LegartDelayFlg = 0;//レガート遅延フラグIX+45
            public byte KeyoffFlg = 0;//キーオフフラグIX+46
            public byte TrackStopFlg = 0;//トラック停止フラグIX+47
            public byte GlideFlg = 0;//グライドフラグIX+48
            public ushort Glide = 0;//グライド値IX+49,IX+50
            public byte workForPlayer = 0;//プレイヤー用ワーク(加工前のノートナンバー)IX+51
            public byte PSGRR = 0;//IX+52
            public byte PSGRRLevel = 15;//IX+53
            public byte PSGRRCounter = 0;//IX+54
            public byte PSGRRVolOffset = 0;//IX+55

            public repBuf[] repBuf = new repBuf[6] { new repBuf(), new repBuf(), new repBuf(), new repBuf(), new repBuf(), new repBuf() };
        }

        private class repBuf
        {
            public byte count=0;
            public ushort startAdr=0;
            public ushort endAdr=0;
        }

        private void imain()
        {
            if ((work.PLYFLG & 0x40) != 0)
            {
                return;
            }

            //OPM1
            if ((work.ctcflg & 0x1) != 0)
            {
                work.OPMIO = 0x701;
                mmain(work.OPM1Chs);
            }

            //OPM2
            if ((work.ctcflg & 0x2) != 0)
            {
                work.OPMIO = 0x709;
                mmain(work.OPM2Chs);
            }

            //PSG
            work.OPMIO = 0;
            pmain(work.PSGChs);

            //FadeOut
            if ((work.PLYFLG & 0x2) == 0) return;

            work.FCOUNT++;

            if (work.FSPEED >= work.FCOUNT) return;

            work.FCOUNT = 0;

            work.MVOL++;

            if (work.MVOL <= 127) return;

            work.PLYFLG &= 0xc0;
            work.PLYFLG |= 0x4;

            foreach (Ch wch in work.OPM1Chs)
            {
                wch.TrackStopFlg = 0xff;
            }

            foreach (Ch wch in work.OPM2Chs)
            {
                wch.TrackStopFlg = 0xff;
            }

            foreach (Ch wch in work.PSGChs)
            {
                wch.TrackStopFlg = 0xff;
            }

        }

        private void mmain(Ch[] chs)
        {
            for (byte e = 0; e < 8; e++)
            {
                Ch wch = chs[e];
                if (wch.TrackStopFlg == 0)
                {
                    if (wch.KeyoffFlg != 0)
                    {
                        wch.KeyoffFlg = 0;
                        wch.workForPlayer = 0;
                        wopm(8, e);//Key off
                    }

                    if (wch.Counter == 0)
                    {
                        if (wch.isCountNext == 0)
                        {
                            comchk(wch,e);
                            goto MAINL;
                        }
                        byte d = ram[wch.ptrData];
                        wch.ptrData++;
                        if (d != 0xff)
                        {
                            if (d == 0)
                            {
                                comchk(wch,e);
                                goto MAINL;
                            }
                            wch.isCountNext = 0;
                        }
                        wch.Counter = d;
                    }
                    wch.Counter--;

                    if (wch.gatetime > wch.Counter)
                    {
                        if (wch.LegartFlg == 0)
                        {
                            if (wch.isCountNext == 0)
                            {
                                wch.KeyoffFlg = 1;
                            }
                        }
                    }

                }
                MAINL:
                if ((work.PLYFLG & 0x3) > 1)
                {
                    mvset(wch,e);
                }
            }
        }

        private void comchk(Ch wch,byte e)
        {
            while (true)
            {
                byte cmdno = ram[wch.ptrData];
                wch.ptrData++;

                if ((cmdno & 0x80) != 0)
                {
                    if (work.OPMKeyONEnable)
                    {
                        //KEYON
                        keyon(wch, e, cmdno);
                    }
                    else
                    {
                        wch.ptrData--;
                    }
                    return;
                }
                else if (cmdno == 127)
                {
                    //トラック終端
                    trkend(wch);
                }
                else if (cmdno < 38)
                {
                    if (comck0(wch, e, cmdno) == 1) return;
                }
                else if (cmdno == 125)
                {
                    //トラック一時停止
                    wch.TrackStopFlg = 125;
                    return;
                }
                else
                {
                    wch.TrackStopFlg = 255;
                    return;
                }
            }
        }

        private int comck0(Ch wch, byte e,byte cmdno)
        {
            int r = 0;
            switch (cmdno)
            {
                case 0:
                    if (work.OPMRestEnable) r = REST(wch, e);
                    else
                    {
                        r = 1;
                        wch.ptrData--;
                    }
                    break;
                case 1: r = ZCOM(wch, e); break;
                case 2:
                    if (work.OPMT02_LightMode) r = VSETL(wch, e);
                    else r = VSET(wch, e);
                    break;
                case 3: r = VOLWS(wch, e); break;
                case 4: r = CSMCOM(wch, e); break;
                case 5: r = STYPE(wch, e); break;
                case 6: r = SOP(wch, e); break;
                case 7: r = HLFO(wch, e); break;
                case 8: r = REST(wch, e); break;
                case 9: r = REST(wch, e); break;
                case 10: r = REST(wch, e); break;
                case 11: r = FFSET(wch, e); break;
                case 12: r = YCOMP(wch, e); break;
                case 13: r = REST(wch, e); break;
                case 14:
                    if (work.OPMT14_MVMode) r = VSETMV(wch, e);
                    else r = VSET(wch, e);
                    break;
                case 15: r = DETUNE(wch, e); break;
                case 16: r = MMACRO(wch, e); break;
                case 17: r = LEGON(wch, e); break;
                case 18: r = LEGOFF(wch, e); break;
                case 19:
                    if (work.OPMT19Enable) r = VOLUME(wch, e);
                    else
                    {
                        r = 1;
                        wch.ptrData--;
                    }
                    break;
                case 20: r = PANPOT(wch, e); break;
                case 21: r = RSTART(wch, e); break;
                case 22: r = REND(wch, e); break;
                case 23: r = RQUIT(wch, e); break;
                case 24: r = YCOM(wch, e); break;
                case 25: r = QUANT1(wch, e); break;
                case 26: r = QUANT2(wch, e); break;
                case 27: r = LSYNC(wch, e); break;
                case 28: r = KSHIFT(wch, e); break;
                case 29: r = PORTA(wch, e); break;
                case 30: r = TEMPO(wch, e); break;
                case 31: r = SPM(wch, e); break;
                case 32: r = SAM(wch, e); break;
                case 33: r = KKOFF(wch, e); break;
                case 34: r = REPLAY(wch, e); break;
                case 35: r = GLIDE(wch, e); break;
                case 36: r = VOLOP(wch, e); break;
                case 37: r = FADEC(wch, e); break;
            }

            return r;
        }

        private int REST(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            if (a == 255)
            {
                wch.isCountNext = 255;
            }
            else
            {
                wch.isCountNext = 0;
            }
            a--;
            wch.Counter = a;
            wch.ptrData++;

            return 1;
        }

        private int ZCOM(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            work.ZCOUNT = a;
            wch.ptrData++;

            do
            {
                byte d = ram[wch.ptrData];
                wch.ptrData++;
                a = ram[wch.ptrData];
                wch.ptrData++;
                wopm(d, a);
                a = work.ZCOUNT;
                a--;
                work.ZCOUNT = a;
            } while (a != 0);

            return 0;
        }

        private int VSETL(Ch wch, byte e)
        {
            byte a = (byte)(0x20+e);
            byte d = a;
            a = ram[wch.ptrData];
            wch.PanAlgFb = a;
            wopm(d, a);
            wch.ptrData++;
            ushort bc = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
            wch.ptrData+=2;
            ushort hl = (ushort)(work.bgmadr + bc);

            VSETJ(wch, e, hl);

            return 0;
        }

        private void VSETJ(Ch wch, byte e,ushort hl)
        {
            byte d = (byte)(0x40 + e);
            byte a = e;

            for (int i = 0; i < 24; i++)
            {
                a = ram[hl];
                hl++;
                wopm(d, a);
                d += 8;
            }
            wch.OP1TL = ram[hl];
            hl++;
            wch.OP3TL = ram[hl];
            hl++;
            wch.OP2TL = ram[hl];
            hl++;
            wch.OP4TL = ram[hl];
        }

        private int VSET(Ch wch, byte e)
        {
            ushort bc = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
            wch.ptrData += 2;
            ushort hl = (ushort)(work.bgmadr + bc);

            byte a = ram[hl];
            byte c = a;
            a &= 0xc0;
            if (a == 0)
            {
                a = (byte)(wch.PanAlgFb & 0xc0);
                a |= c;
                c = a;
            }
            a = 0x20;
            a += e;
            byte d = a;
            a = c;
            wch.PanAlgFb = a;
            wopm(d, a);
            hl++;
            d = ram[hl];
            a = wch.LFOFlags;
            a &= 0x83;
            a |= d;
            wch.LFOFlags = a;
            hl++;
            VSETJ(wch, e, hl);
            return 0;
        }

        private int VSETMV(Ch wch, byte e)
        {
            ushort bc = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
            wch.ptrData += 2;
            ushort hl = (ushort)(work.bgmadr + bc);

            byte a = ram[hl];
            byte c = a;
            a &= 0xc0;
            if (a == 0)
            {
                a = (byte)(wch.PanAlgFb & 0xc0);
                a &= 0xc0;
                a |= c;
                c = a;
            }
            a = 0x20;
            a += e;
            byte d = a;
            a = c;
            wch.PanAlgFb = a;
            wopm(d, a);
            hl++;
            d = ram[hl];
            a = wch.LFOFlags;
            a &= 0x83;
            a |= d;
            wch.LFOFlags = a;
            hl++;

            a = 0x40;
            a += e;
            d = a;
            //DT1/ML
            for (int i = 0; i < 4; i++)
            {
                a = ram[hl];
                hl++;
                wopm(d, a);
                d += 8;
            }
            //TL
            for (int i = 0; i < 4; i++)
            {
                a = work.MVOL;
                c = ram[hl];
                a += c;
                if (a > 127) a = 127;
                hl++;
                wopm(d, a);
                d += 8;
            }
            //etc
            for (int i = 0; i < 16; i++)
            {
                a = ram[hl];
                hl++;
                wopm(d, a);
                d += 8;
            }
            wch.OP1TL = ram[hl];
            hl++;
            wch.OP3TL = ram[hl];
            hl++;
            wch.OP2TL = ram[hl];
            hl++;
            wch.OP4TL = ram[hl];

            return 0;
        }

        private int VOLWS(Ch wch, byte e)
        {
            wch.Volume = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int CSMCOM(Ch wch, byte e)
        {
            byte a = 0x40;
            a += e;
            byte d = a;
            byte c = 0x8;

            for (int i = 0; i < 4; i++)
            {
                a = ram[wch.ptrData];
                wch.ptrData++;
                if ((a & 0x20) == 0)
                {
                    wopm(d, (byte)(a & 0xf));
                    a &= 0xc0;
                    d |= 0x80;
                    wopm(d, a);
                    d &= 0x7f;
                }
                a = ram[wch.ptrData];
                wch.ptrData++;
                if (a < 127)
                {
                    d |= 0x20;
                    wopm(d, a);
                    d &= 0xdf;
                }
                a = c;
                a += d;
                d = a;
            }

            return 0;
        }

        private int SPM(Ch wch, byte e)
        {
            wch.softPMDelay = ram[wch.ptrData];
            wch.ptrData++;
            wch.softPMPitch = ram[wch.ptrData];
            wch.ptrData++;
            wch.softPMStep = ram[wch.ptrData];
            wch.ptrData++;

            STYPE(wch, e);

            return 0;
        }

        private int STYPE(Ch wch, byte e)
        {
            wch.softPMType = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int SAM(Ch wch, byte e)
        {
            wch.softAMFlagAndDelay = ram[wch.ptrData];
            wch.ptrData++;
            wch.softAMDepth = ram[wch.ptrData];
            wch.ptrData++;
            wch.softAMStep = ram[wch.ptrData];
            wch.ptrData++;
            if (wch.softAMStep == 0)
            {
                tlrst(wch, e);
            }
            SOP(wch, e);

            return 0;
        }

        private int SOP(Ch wch, byte e)
        {
            wch.softAMSelOP = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int HLFO(Ch wch, byte e)
        {
            byte d = 0x18;
            byte a = ram[wch.ptrData];
            wopm(d, a);
            wch.ptrData++;

            d = 0x19;

            if (work.OPMIO != 0)
            {
                a = ram[wch.ptrData];
                work.PMD2 = a;
                wopm(d, a);
                wch.ptrData++;

                d = 0x19;
                a = ram[wch.ptrData];
                work.AMD2 = a;
            }
            else
            {
                a = ram[wch.ptrData];
                work.PMD1 = a;
                wopm(d, a);
                wch.ptrData++;

                d = 0x19;
                a = ram[wch.ptrData];
                work.AMD1 = a;
            }
            wopm(d, a);
            wch.ptrData++;

            d = 0x1b;
            a = ram[wch.ptrData];
            wopm(d, a);

            wch.ptrData++; //CCRET

            return 0;
        }

        private int FFSET(Ch wch, byte e)
        {
            work.FFFLG = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int YCOM(Ch wch, byte e)
        {
            byte d = ram[wch.ptrData];
            wch.ptrData++;
            byte a = ram[wch.ptrData];
            wopm(d, a);
            wch.ptrData++; //CCRET
            return 0;
        }

        private int PYCOM(Ch wch, byte e)
        {
            byte d = ram[wch.ptrData];
            wch.ptrData++;
            byte a = ram[wch.ptrData];
            wpsg(d, a);
            wch.ptrData++; //CCRET
            return 0;
        }

        private int YCOMP(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            wch.ptrData++;
            if (a == 0)
            {
                PYCOM(wch, e);
                return 0;
            }
            else
            {
                byte b = 7;
                byte c = a;
                byte d = ram[wch.ptrData];
                wch.ptrData++;
                a = ram[wch.ptrData];
                WOPMBC(a, b, c, d);
                wch.ptrData++; //CCRET
            }

            return 0;
        }

        private void WOPMBC(byte a,byte b,byte c,byte d)
        {
            if (c == 1)
            {
                //OPM1
                work.OPM1vreg[d] = a;
                if (work.opmflg != 0)
                {
                    //ウエイト
                }
                chipRegister.setYM2151Register(0, 0, d, a, enmModel.VirtualModel, 0, 0);
            }
            else
            {
                //OPM2
                work.OPM2vreg[d] = a;
                if (work.opmflg != 0)
                {
                    //ウエイト
                }
                chipRegister.setYM2151Register(1, 0, d, a, enmModel.VirtualModel, 0, 0);
            }
        }

        private int DETUNE(Ch wch, byte e)
        {
            wch.Detune = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int MMACRO(Ch wch, byte e)
        {
            ushort bc = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
            wch.ptrData += 2;
            wch.MacroReturnAdr = wch.ptrData;
            ushort hl = work.bgmadr;
            hl += bc;
            wch.ptrData = hl;

            //CCRET1 不要

            return 0;
        }

        private int LEGON(Ch wch, byte e)
        {
            wch.LegartFlg = 1;

            //CCRET1 不要

            return 0;
        }

        private int LEGOFF(Ch wch, byte e)
        {
            wch.LegartFlg = 0;

            //CCRET1 不要

            return 0;
        }

        private int VOLUME(Ch wch, byte e)
        {
            wch.Volume = ram[wch.ptrData];
            wch.ptrData++;

            volsub(wch, e);

            return 0;
        }

        private int PANPOT(Ch wch, byte e)
        {
            byte a = 0x20;
            a += e;
            byte d = a;
            a = wch.PanAlgFb;
            a &= 0x3f;
            byte c = a;
            a = ram[wch.ptrData];
            a <<= 6;
            a |= c;
            wch.PanAlgFb = a;

            wopm(d, a);

            wch.ptrData++; //CCRET

            return 0;
        }

        private int RSTART(Ch wch, byte e)
        {
            wch.NestCount++;
            byte d = ram[wch.ptrData];
            wch.ptrData++;

            //RASRSは多分不要

            wch.repBuf[wch.NestCount - 1].count = d;
            wch.repBuf[wch.NestCount - 1].startAdr = wch.ptrData;

            //CCRET1は不要
            return 0;
        }

        private int REND(Ch wch, byte e)
        {
            //RASRSは多分不要

            wch.repBuf[wch.NestCount - 1].count--;
            if (wch.repBuf[wch.NestCount - 1].count == 0)
            {
                wch.NestCount--;
                //CCRET1は不要
                return 0;
            }

            wch.repBuf[wch.NestCount - 1].endAdr = wch.ptrData;
            wch.ptrData = wch.repBuf[wch.NestCount - 1].startAdr;

            //CCRET1は不要
            return 0;
        }

        private int RQUIT(Ch wch, byte e)
        {
            //RASRSは多分不要

            if (wch.repBuf[wch.NestCount - 1].count != 1)
            {
                //CCRET1は不要
                return 0;
            }

            wch.repBuf[wch.NestCount - 1].count = 0;
            wch.ptrData = wch.repBuf[wch.NestCount - 1].endAdr;
            wch.NestCount--;

            //CCRET1は不要
            return 0;
        }

        private int QUANT1(Ch wch, byte e)
        {
            wch.q = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int QUANT2(Ch wch, byte e)
        {
            wch.Q = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int LSYNC(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            if (a == 0)
            {
                lreset();
            }
            else
            {
                byte d = a;
                a = wch.LFOFlags;
                a &= 0xfc;
                a |= d;
                wch.LFOFlags = a;
            }

            wch.ptrData++; //CCRET

            return 0;
        }

        private int TEMPO(Ch wch, byte e)
        {

            work.ctc0 = 0x25;
            work.ctc0timeconstant = ram[wch.ptrData];
            work.ctc3 = 0xc5;
            work.ctc3timeconstant = ram[wch.ptrData+1];

            work.KEYONF_OPMMASK = 0x78;
            work.PENVF_VOL0 = false;
            work.FFFLG = 0;

            wch.ptrData+=2; //CCRET

            return 0;
        }

        private int KSHIFT(Ch wch, byte e)
        {
            wch.Transpose = ram[wch.ptrData];
            wch.ptrData++; //CCRET

            return 0;
        }

        private int PORTA(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            wch.PortaFlg = a;
            wch.PortaStartFlg = a;
            wch.GlideFlg = 0;

            wch.ptrData++; //CCRET

            return 0;
        }

        private int KKOFF(Ch wch, byte e)
        {
            byte d = 8;
            byte a = e;
            wopm(d, a);

            wch.LegartDelayFlg = 0;
            wch.workForPlayer = 0;

            return 0;
        }

        private int REPLAY(Ch wch, byte e)
        {
            byte b = ram[wch.ptrData];
            wch.ptrData++;

            Ch c;
            if (b < 8) c = work.OPM1Chs[b];
            else if (b < 16) c = work.OPM2Chs[b - 8];
            else c = work.PSGChs[b - 16];

            byte a = c.TrackStopFlg;
            if (a != 255)
            {
                c.TrackStopFlg = 0;
            }

            //CCRET1は不要
            return 0;
        }

        private int GLIDE(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            wch.GlideFlg = a;
            wch.PortaFlg = a;
            wch.ptrData++;
            wch.Glide = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
            wch.ptrData += 2;//CCRET
            return 0;
        }

        private int VOLOP(Ch wch, byte e)
        {
            wch.Volume = ram[wch.ptrData];
            wch.ptrData++;
            byte a = ram[wch.ptrData];
            wch.ptrData++;

            volops(wch, a, e);

            return 0;
        }

        private int FADEC(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];

            MFADE1(a);

            wch.ptrData++;
            return 0;
        }

        private void MFADE1(byte a)
        {
            work.FSPEED = a;
            work.OPMT02_LightMode = true;
            work.OPMT14_MVMode = true;
            a = work.PLYFLG;
            a &= 0xc0;
            a |= 2;
            work.PLYFLG = a;
        }

        private void volops(Ch wch,byte a,byte e)
        {
            if ((a & 1) != 0)
            {
                volwr(wch, 0x60, 0x8, e);
            }
            if ((a & 2) != 0)
            {
                volwr(wch, 0x70, 0x9, e);
            }
            if ((a & 4) != 0)
            {
                volwr(wch, 0x68, 0xa, e);
            }
            if ((a & 8) != 0)
            {
                volwr(wch, 0x78, 0xb, e);
            }
        }

        private void trkend(Ch wch)
        {
            ushort bc = wch.MacroReturnAdr;
            if (bc != 0)
            {
                wch.ptrData = bc;
                wch.MacroReturnAdr = 0;
            }
            else
            {
                bc = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
                wch.ptrData = (ushort)(work.bgmadr + bc);
                wch.loopCounter++;
            }
            //ccret1は不要
        }

        private void mvset(Ch wch, byte e)
        {
            mvwr(wch, e, 3);//OP4

            byte a = (byte)(wch.PanAlgFb & 0x7);
            if (a < 4) return;

            mvwr(wch, e, 2);//OP2

            if (a == 4) return;

            mvwr(wch, e, 1);//OP3

            if (a != 7) return;

            mvwr(wch, e, 0);//OP1
        }

        private void mvwr(Ch wch,byte e,int op)
        {
            byte d = (byte)(0x60 + e + op * 8);
            byte a = (byte)(wch.Volume ^ 127);

            switch (op)
            {
                case 0:
                    a += (byte)(work.MVOL + wch.OP1TL);
                    a = (byte)((a > 127) ? 127 : a);
                    wch.OP1TLs = a;
                    break;
                case 1:
                    a += (byte)(work.MVOL + wch.OP3TL);
                    a = (byte)((a > 127) ? 127 : a);
                    wch.OP3TLs = a;
                    break;
                case 2:
                    a += (byte)(work.MVOL + wch.OP2TL);
                    a = (byte)((a > 127) ? 127 : a);
                    wch.OP2TLs = a;
                    break;
                case 3:
                    a += (byte)(work.MVOL + wch.OP4TL);
                    a = (byte)((a > 127) ? 127 : a);
                    wch.OP4TLs = a;
                    break;
            }

            wopm(d, a);
        }

        private void keyon(Ch wch,byte e, byte cmdno)
        {
            byte a = (byte)(cmdno & 0x7f);
            byte ks = (byte)(wch.Transpose);
            a += ks;
            wch.workForPlayer = a;

            a = ram[wch.ptrData];

            if (a == 255) wch.isCountNext = a;
            else wch.isCountNext = 0;

            a--;
            wch.Counter = a;

            if (a == 0)
            {
                if (wch.LegartFlg == 0)
                {
                    wch.KeyoffFlg = 1;
                }
            }

            wch.ptrData++;

            ushort bc = (ushort)(a * 0x100);
            bc = (ushort)(bc / 8);

            a = wch.q;
            if (a == 8)
            {
                a = 0;
            }
            else
            {
                a = (byte)(((8-a) * bc) / 0x100);
            }
            a += wch.Q;
            a++;
            wch.gatetime = a;

            byte b = wch.workForPlayer;
            byte d = (byte)(0x30 + e);
            uint ia = wch.Detune;
            if (ia >= 128)
            {
                while (ia < 256)
                {
                    b--;
                    ia += 64;
                }
            }
            else
            {
                while (ia >= 64)
                {
                    b++;
                    ia -= 64;
                }
            }
            ia = ia * 4;

            byte c = (byte)ia;
            a = wch.GlideFlg;
            if (a != 0)
            {
                wch.PortaFlg = a;
                wch.PortaTone = (ushort)(b * 0x100 + c);
                wch.PortaStartFlg = a;

                ushort hl = (ushort)(b * 0x100 + c);
                bc = wch.Glide;
                hl += bc;
                b = (byte)(hl / 0x100);
                c = (byte)(hl & 0xff);
            }
            else
            {
                a = wch.PortaFlg;
                if (a != 0)
                {
                    wch.PortaStartFlg = a;
                    wch.PortaTone = (ushort)(b * 0x100 + c);
                    goto KEYONE;
                }
            }
            wch.NoteNumber = b;
            wch.KF = c;
            a = c;
            wopm(d, a);
            d -= 8;
            a = KTABLE[wch.NoteNumber];
            wopm(d, a);

            KEYONE:
            a = wch.LegartDelayFlg;
            c = a;
            wch.LegartDelayFlg = wch.LegartFlg;
            if (a == 0)
            {
                a = wch.LFOFlags;
                if ((a & 2) != 0)
                {
                    lreset();
                }
                d = 8;
                a = wch.LFOFlags;
                if ((a & 0x80) != 0)
                {
                    a = 0;
                }
                else
                {
                    a &= 0x78;
                }
                a |= e;
                wopm(d, a);
            }

            a = wch.softPMType;
            if ((a & 0x80) != 0)
            {
                wch.softPMProcCount = (byte)((a & 0x7f) * 2);
                if (c != 0)
                {
                    b = 1;
                }
                else
                {
                    b = wch.softPMDelay;
                }
                wch.softPMDelayCount = b;
                wch.softPMStepCount = wch.softPMStep;
            }
            if ((wch.softAMSelOP & 0x80) != 0)
            {
                wch.softAMStepCount = (byte)(wch.softAMStep + 1);
                a = c;
                if (c != 0)
                {
                    a = 1;
                }
                else
                {
                    a = wch.softAMFlagAndDelay;
                }
                wch.softAMDelayCount = a;
                wch.softAMProcCount = 0;
                tlrst(wch, e);
            }
        }

        private void tlrst(Ch wch,byte e)
        {
            byte a = (byte)(wch.PanAlgFb & 7);
            byte l = a;
            byte c;

            if (l != 7)
            {
                c = wch.OP1TL;
                a = wch.OP1TLs;
                if (a != c)
                {
                    wch.OP1TLs = c;
                    a = 0x60;
                    a += e;
                    byte d = a;
                    a = c;
                    wopm(d, a);
                }
            }

            if (l < 4)
            {
                c = wch.OP2TL;
                a = wch.OP2TLs;
                if (a != c)
                {
                    wch.OP2TLs = c;
                    a = 0x70;
                    a += e;
                    byte d = a;
                    a = c;
                    wopm(d, a);
                }
            }

            if (l < 5)
            {
                c = wch.OP3TL;
                a = wch.OP3TLs;
                if (a != c)
                {
                    wch.OP3TLs = c;
                    a = 0x68;
                    a += e;
                    byte d = a;
                    a = c;
                    wopm(d, a);
                }
            }

            c = wch.OP4TL;
            wch.OP4TLs = c;

            volsub(wch,e);
        }

        private void volsub(Ch wch, byte e)
        {
            byte a = 0x78;
            byte c = 11;
            volwr(wch,a, c, e);

            a = (byte)(wch.PanAlgFb & 7);
            byte l = a;
            if (a >= 4)
            {
                a = 0x70;
                c = 9;
                volwr(wch, a, c, e);

                if (l != 4)
                {
                    a = 0x68;
                    c = 10;
                    volwr(wch, a, c, e);

                    if (l == 7)
                    {
                        a = 0x60;
                        c = 8;
                        volwr(wch, a, c, e);
                    }
                }
            }
        }

        private void volwr(Ch wch, byte a,byte c,byte e)
        {
            a+=e;
            byte d = a;

            byte op = c;

            a = (byte)(wch.Volume ^ 0x7f);
            c = a;
            a = work.MVOL;
            byte b = a;

            switch (op) {
                case 8:
                    a = wch.OP1TL;
                    break;
                case 9:
                    a = wch.OP2TL;
                    break;
                case 10:
                    a = wch.OP3TL;
                    break;
                case 11:
                    a = wch.OP4TL;
                    break;
            }
            a += (byte)(c + b);
            if (a > 127) a = 127;
            switch (op)
            {
                case 8:
                    wch.OP1TLs = a;
                    break;
                case 9:
                    wch.OP2TLs = a;
                    break;
                case 10:
                    wch.OP3TLs = a;
                    break;
                case 11:
                    wch.OP4TLs = a;
                    break;
            }
            wopm(d, a);
        }



        private void pmain(Ch[] chs)
        {
            for (byte e = 0; e < 3; e++)
            {
                Ch wch = chs[e];

                if (wch.KeyoffFlg == 2)
                {
                    PSGRR(wch);
                }
                else if (wch.KeyoffFlg != 0)
                {
                    RRST(wch, e);
                }

                //PMAIN2:
                byte a = wch.TrackStopFlg;
                if (a == 0)
                {
                    //PMAIN3:
                    byte c = wch.Counter;
                    if (c == 0)
                    {
                        if (wch.isCountNext == 0)
                        {
                            PCOM(wch, e);
                            goto PMAINL;
                        }
                        a = ram[wch.ptrData];
                        wch.ptrData++;
                        if (a != 255)
                        {
                            if (a == 0)
                            {
                                PCOM(wch, e);
                                goto PMAINL;
                            }
                            wch.isCountNext = 0;
                        }
                        wch.Counter = a;
                    }
                    //PMAIN1:
                    wch.Counter--;

                    if (wch.Counter- wch.gatetime < 0)
                    {
                        if (wch.LegartFlg == 0)
                        {
                            if (wch.isCountNext == 0)
                            {
                                if (wch.KeyoffFlg == 0)
                                    wch.KeyoffFlg = 1;
                            }
                        }
                    }

                }

                PMAINL:
                if (wch.PSGToneAdr != 0)
                {
                    PENV(wch, e);
                }

            }
        }

        private void PSGRR(Ch wch)
        {

            wch.PSGRRCounter--;
            if (wch.PSGRRCounter >= 0) return;

            wch.PSGRRCounter = wch.PSGRR;
            wch.PSGRRVolOffset++;
            if (wch.PSGRRVolOffset < 16) return;

            wch.PSGToneAdr = 0;
            wch.KeyoffFlg = 0;
            wch.workForPlayer = 0;

        }

        private void RRST(Ch wch, byte e)
        {
            byte a = wch.PSGHardEnvelopeType;
            if (a - 16 < 0)
            {
                //RRST1:
                wch.PSGToneAdr = 0;
                wch.KeyoffFlg = 0;
                wch.workForPlayer = 0;
                wpsg((byte)(8 + e), 0);
                return;
            }

            wch.PSGRRVolOffset = wch.PSGRRLevel;
            wch.KeyoffFlg = 2;
        }

        private void PCOM(Ch wch, byte e)
        {
            while (true)
            {
                byte cmdno = ram[wch.ptrData];
                wch.ptrData++;

                if ((cmdno & 0x80) != 0)
                {
                    PKEYON(wch, e, cmdno);
                    return;
                }
                else if (cmdno == 127)
                {
                    //トラック終端
                    trkend(wch);
                    //return;
                }
                else if (cmdno < 38)
                {
                    if (PCOM0(wch, e, cmdno) == 1) return;
                }
                else
                {
                    wch.TrackStopFlg = 255;
                    return;
                }
            }
        }

        private int PCOM0(Ch wch, byte e, byte cmdno)
        {
            int r = 0;
            switch (cmdno)
            {
                case 0: r = REST(wch, e); break;
                case 1: r = PZCOM(wch, e); break;
                case 2: r = REST(wch, e); break;
                case 3: r = REST(wch, e); break;
                case 4: r = REST(wch, e); break;
                case 5: r = STYPE(wch, e); break;
                case 6: r = REST(wch, e); break;
                case 7: r = REST(wch, e); break;
                case 8: r = REST(wch, e); break;
                case 9: r = REST(wch, e); break;
                case 10: r = REST(wch, e); break;
                case 11: r = FFSET(wch, e); break;
                case 12: r = YCOMP(wch, e); break;
                case 13: r = RRSET(wch, e); break;
                case 14: r = PVSET(wch, e); break;
                case 15: r = DETUNE(wch, e); break;
                case 16: r = MMACRO(wch, e); break;
                case 17: r = LEGON(wch, e); break;
                case 18: r = LEGOFF(wch, e); break;
                case 19: r = PVOL(wch, e); break;
                case 20: r = PMODE(wch, e); break;
                case 21: r = RSTART(wch, e); break;
                case 22: r = REND(wch, e); break;
                case 23: r = RQUIT(wch, e); break;
                case 24: r = PYCOM(wch, e); break;
                case 25: r = QUANT1(wch, e); break;
                case 26: r = QUANT2(wch, e); break;
                case 27: r = NFREQ(wch, e); break;
                case 28: r = KSHIFT(wch, e); break;
                case 29: r = PORTA(wch, e); break;
                case 30: r = TEMPO(wch, e); break;
                case 31: r = SPM(wch, e); break;
                case 32: r = SHAPE(wch, e); break;
                case 33: r = PKKOFF(wch, e); break;
                case 34: r = REPLAY(wch, e); break;
                case 35: r = GLIDE(wch, e); break;
                case 36: r = PERIOD(wch, e); break;
                case 37: r = FADEC(wch, e); break;
            }

            return r;
        }

        private void PENV(Ch wch, byte e)
        {
            ushort hl = wch.PSGToneAdr;
            byte a = 0;
            byte c = 0;
            while (true)
            {
                a = ram[hl];
                hl++;
                ushort bc;
                if (a == 255)
                {
                    bc = ram[hl];
                    hl -= bc;
                    a = ram[hl];
                }
                //PENV1:
                if (a - 16 < 0)
                {
                    break;
                }
                if (a - 48 < 0)
                {
                    wpsg(6, (byte)(a - 16));
                    continue;
                }
                a -= 48;
                a *= 2;
                bc = a;

                a = work.PFLG;
                c = PMTBL[e * 8 + bc];
                a &= c;
                c = PMTBL[e * 8 + bc + 1];
                a |= c;
                work.PFLG = a;
                wpsg(7, a);
            }
            //PENV4:
            wch.PSGToneAdr = hl;
            a ^= 15;
            c = a;
            byte d = (byte)(8 + e);
            byte b = work.MVOL;
            //PENVF2:
            if ((wch.LFOFlags & 0x80) == 0)
            {
                a = wch.Volume;
                if (a - b < 0)
                {
                    a = 0;
                }
                else
                {
                    a -= b;
                    a = (byte)(a >> 3);
                    b = wch.PSGRRVolOffset;
                    if (a - b < 0)
                    {
                        a = 0;
                    }
                    else
                    {
                        a -= b;
                        if (a - c < 0)
                        {
                            a = 0;
                        }
                        else
                        {
                            a -= c;
                        }
                    }
                }
            }
            else
            {
                a = 0;
            }
            //PENV5:
            wpsg(d, a);
            return;
        }

        private void PKEYON(Ch wch, byte e, byte cmdno)
        {
            byte a = (byte)(cmdno & 0x7f);
            byte ks = (byte)(wch.Transpose);
            a += ks;
            wch.workForPlayer = a;

            a = ram[wch.ptrData];

            if (a == 255) wch.isCountNext = a;
            else wch.isCountNext = 0;

            a--;
            wch.Counter = a;

            if (a == 0)
            {
                if (wch.LegartFlg == 0)
                {
                    wch.KeyoffFlg = 1;
                }
            }
            else
            {
                wch.KeyoffFlg = 0;
            }

            wch.ptrData++;

            ushort bc = (ushort)(a * 0x100);
            bc = (ushort)(bc / 8);

            a = wch.q;
            if (a == 8)
            {
                a = 0;
            }
            else
            {
                a = (byte)(((8 - a) * bc) / 0x100);
            }
            a += wch.Q;
            a++;
            wch.gatetime = a;

            bc = wch.workForPlayer;
            byte d = (byte)(e << 1);

            byte b = 0;
            byte c = (byte)(PTABLE[bc]);
            a = wch.Detune;
            a = (byte)(~a+1);
            if (a-129 < 0)
            {
                if (a + c <= 255)
                {
                    a += c;
                }
                else
                {
                    a += c;
                    b++;
                }
            }
            else
            {
                if (a + c > 255)
                {
                    a += c;
                }
                else
                {
                    a += c;
                    b--;
                }
            }
            //PKON5
            c = a;
            a = (byte)(PTABLE[bc] >> 8);
            b += a;

            a = wch.GlideFlg;
            if (a != 0)
            {
                wch.PortaFlg = a;
                wch.PortaTone = (ushort)(b * 0x100 + c);
                wch.PortaStartFlg = a;

                ushort hl = (ushort)(b * 0x100 + c);
                bc = wch.Glide;
                hl += bc;
                b = (byte)(hl / 0x100);
                c = (byte)(hl & 0xff);
                goto PKON8;
            }
            else
            {
                //PKON11:
                a = wch.PortaFlg;
                if (a != 0)
                {
                    wch.PortaStartFlg = a;
                    wch.PortaTone = (ushort)(b * 0x100 + c);
                    goto PKON9;
                }
            }
            PKON8:
            wch.PSGTone = (ushort)(b * 0x100 + c);
            wpsg(d, c);
            d++;
            wpsg(d, b);

            PKON9:
            a = wch.LegartDelayFlg;
            c = a;
            wch.LegartDelayFlg = wch.LegartFlg;
            if (a != 0)
            {
                if (wch.PSGToneAdr != 0)
                {
                    goto PKONE;
                }
            }
            //PKON10
            a = wch.PSGHardEnvelopeType;
            if (a - 16 < 0)
            {
                wpsg(13, a);
                d = (byte)(8 + e);
                a = wch.LFOFlags;
                if ((a & 0x80) == 0)
                {
                    a = 16;
                }
                else
                {
                    a = 0;
                }
                wpsg(d, a);
                goto PKONE;
            }
            //PKSENV:
            a = wch.LFOFlags;
            if ((a & 0x80) != 0)
            {
                d = 8;
                d += e;
                a = 0;
                wpsg(d, a);
            }
            //PKSENV02
            wch.PSGRRVolOffset = a;
            wch.PSGToneAdr = wch.PSGToneStartAdr;

            PKONE:
            a = wch.softPMType;
            if ((a & 0x80) != 0)
            {
                wch.softPMProcCount = (byte)(((a & 0x7f) * 2) ^ 2);
                wch.softPMStepCount = wch.softPMStep;

                if (c != 0)
                {
                    b = 1;
                }
                else
                {
                    b = wch.softPMDelay;
                }
                wch.softPMDelayCount = b;
            }

        }

        private int PZCOM(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            work.ZCOUNT = a;
            wch.ptrData++;

            do
            {
                byte d = ram[wch.ptrData];
                wch.ptrData++;
                a = ram[wch.ptrData];
                wch.ptrData++;
                wpsg(d, a);
                a = work.ZCOUNT;
                a--;
                work.ZCOUNT = a;
            } while (a != 0);

            return 0;
        }

        private int RRSET(Ch wch, byte e)
        {
            wch.PSGRR = ram[wch.ptrData];
            wch.ptrData++;
            wch.PSGRRLevel = ram[wch.ptrData];

            wch.ptrData++;//CCRET
            return 0;
        }

        private int PVSET(Ch wch, byte e)
        {
            ushort bc = (ushort)(ram[wch.ptrData] + ram[wch.ptrData + 1] * 0x100);
            wch.ptrData += 2;
            wch.PSGToneStartAdr = (ushort)(work.bgmadr + bc);
            wch.PSGHardEnvelopeType = 16;

            return 0;
        }

        private int PVOL(Ch wch, byte e)
        {
            wch.Volume = ram[wch.ptrData];

            wch.ptrData++;//CCRET
            return 0;
        }

        private int PMODE(Ch wch, byte e)
        {
            byte a = ram[wch.ptrData];
            a += a;
            ushort bc = a;
            ushort hl = e;
            hl = (ushort)(hl * 8);
            hl += bc;
            a=work.PFLG;
            a &= PMTBL[hl];
            a |= PMTBL[hl + 1];
            work.PFLG = a;

            wpsg(7, a);

            wch.ptrData++;//CCRET
            return 0;
        }

        private int NFREQ(Ch wch, byte e)
        {
            wpsg(6, ram[wch.ptrData]);

            wch.ptrData++;//CCRET
            return 0;
        }

        private int SHAPE(Ch wch, byte e)
        {
            wch.PSGHardEnvelopeType = ram[wch.ptrData];
            wch.PSGToneAdr = 0;

            wch.ptrData++;//CCRET
            return 0;
        }

        private int PKKOFF(Ch wch, byte e)
        {
            wch.PSGToneAdr = 0;
            wch.LegartDelayFlg = 0;
            wch.workForPlayer = 0;
            wpsg((byte)(8 + e), 0);
            return 0;
        }

        private int PERIOD(Ch wch, byte e)
        {
            wpsg(11, ram[wch.ptrData]);
            wpsg(12, ram[wch.ptrData+1]);

            wch.ptrData += 2;//CCRET
            return 0;
        }



        private void int2()
        {
            work.COUNT--;

            //OPM1
            if ((work.ctcflg & 0x1) != 0)
            {
                work.OPMIO = 0x701;
                efx(work.OPM1Chs);
            }
            //OPM2
            if ((work.ctcflg & 0x2) != 0)
            {
                work.OPMIO = 0x709;
                efx(work.OPM2Chs);
            }
            //PSG
            work.OPMIO = 0;
            efxp(work.PSGChs);

        }

        private void efx(Ch[] chs)
        {
            for (byte e = 0; e < 8; e++)
            {
                Ch wch = chs[e];

                if (wch.PortaFlg != 0) EPOR(wch,e);

                if (wch.softPMStep != 0) EPM(wch,e);

                if (wch.softAMStep != 0) EAM(wch,e);
            }
        }


        private void EPOR(Ch wch, byte e)
        {
            if (wch.PortaStartFlg == 0) return;

            byte l = wch.KF;
            byte h = wch.NoteNumber;
            byte b = (byte)(wch.PortaTone >> 8);
            byte c = (byte)(wch.PortaTone & 0xff);
            byte a = h;

            bool neg = true;

            if (a == b)
            {
                a = l;
                if (a == c)
                {
                    wch.PortaStartFlg = 0;
                    return;
                }
                else if (a < c) neg = false;
            }
            else if (a < b) neg = false;

            //
            if (neg)
            {
                //減算処理
                int p = wch.PortaFlg * 4;
                int hl = (h * 0x100) + l - p;
                if (hl < 0)
                {
                    h = b;
                    l = c;
                }
                else
                {
                    a = (byte)((hl >> 8) - b);
                    if (((hl & 0xff) - c) < 0)
                    {
                        a--;
                    }
                    h = (byte)(hl >> 8);
                    l = (byte)(hl & 0xff);
                }
            }
            else
            {
                //加算処理
                int p = wch.PortaFlg * 4;
                int hl = (h * 0x100) + l + p;
                h = (byte)(hl >> 8);
                l = (byte)(hl & 0xff);

                //a = (byte)((hl >> 8) - b);
                //if (((hl & 0xff) - c) < 0)
                //{
                //    a--;
                //    h = (byte)(hl >> 8);
                //    l = (byte)(hl & 0xff);
                //}
                if (hl > b * 0x100 + c)
                {
                    h = b;
                    l = c;
                }
            }

            wch.KF = l;
            wch.NoteNumber = h;
            wopm((byte)(0x30 + e), l);
            wopm((byte)(0x28 + e), KTABLE[h]);
            //Console.WriteLine($"opmout reg{l:X2} dat{ KTABLE[h]:X2}");

        }


        private void EPM(Ch wch, byte e)
        {
            if (wch.PortaStartFlg != 0) return;

            //Console.WriteLine($"softPMProcCount[{wch.softPMProcCount:d}]");
            //Console.WriteLine($"softPMStep[{wch.softPMStep:d}]");
            //Console.WriteLine($"softPMStepCount[{wch.softPMStepCount:d}]");
            //Console.WriteLine($"softPMPitch[{wch.softPMPitch:d}]");
            //Console.WriteLine($"KF[{wch.KF:d}]");
            //Console.WriteLine($"NoteNumber[{wch.NoteNumber:d}]");

            if (wch.softPMDelayCount - 1 != 0)
            {
                wch.softPMDelayCount--;
                return;
            }

            //EPM1:
            byte l = wch.KF;
            byte h = wch.NoteNumber;

            byte a = wch.softPMStepCount;
            a--;
            if (a != 0)
            {
                wch.softPMStepCount = a;
                a = wch.softPMProcCount;
                if (a >= 8) return;
                if (a >= 4) EPMH1(wch, a, h, e);
                else if (a == 0 || a == 3)
                {
                    EPMM(wch, a, h, l, e);
                }
                else
                {
                    EPMP(wch, a, h, l, e);
                }
                return;
            }
            //EPMS:
            a = wch.softPMStep;
            wch.softPMStepCount = a;

            a = wch.softPMProcCount;
            if (a -8<0)
            {
                //EPMS0:
                if (a >= 4)
                {
                    //EPMH:
                    a++;
                    if (a >= 8) a = 4;
                    wch.softPMProcCount = a;
                    EPMH1(wch, a, h, e);
                    return;
                }
                a++;
                a &= 3;
                wch.softPMProcCount = a;
                //EPMS1:
                if (a == 0 || a >= 3)
                {
                    EPMM(wch, a, h, l, e);
                }
                else
                {
                    EPMP(wch, a, h, l, e);
                }
                return;
            }
            //EPMQ:
            a++;
            if (a == 10 || a > 12)
            {
                a--;
                a--;
            }
            //EPMQ1:
            wch.softPMProcCount = a;
            if (a == 8 || a >= 11)
            {
                EPMP(wch, a, h, l, e);
            }
            else
            {
                EPMM(wch, a, h, l, e);
            }

        }

        private void EPMH1(Ch wch, byte a, byte h, byte e)
        {
            byte b = wch.softPMPitch;
            byte c = 0;

            if (a == 4 || a == 7)
            {
                if (h - b >= 0)
                {
                    c = (byte)(h - b);
                }
            }
            else
            {
                c = 97;
                if (h + b <= 97)
                {
                    c = (byte)(h + b);
                }
            }

            //EPM2H:
            wch.NoteNumber = c;
            wopm((byte)(0x28 + e), KTABLE[c]);

        }

        private void EPMM(Ch wch, byte a, byte h, byte l, byte e)
        {
            ushort p = (ushort)(wch.softPMPitch * 4);
            ushort hl = (ushort)(h * 0x100 + l);
            if (hl - p < 0)
            {
                hl = 0;
            }
            else hl = (ushort)(hl - p);
            h = (byte)(hl >> 8);
            l = (byte)(hl & 0xff);

            wch.KF = l;
            wch.NoteNumber = h;

            wopm((byte)(0x30 + e), l);
            wopm((byte)(0x28 + e), KTABLE[h]);
        }

        private void EPMP(Ch wch, byte a, byte h, byte l, byte e)
        {
            ushort p = (ushort)(wch.softPMPitch * 4);
            ushort hl = (ushort)(h * 0x100 + l);
            if ((byte)((hl + p) >> 8) >= 120)
            {
                hl = 0x77fc;
            }
            else hl = (ushort)(hl + p);
            h = (byte)(hl >> 8);
            l = (byte)(hl & 0xff);

            wch.KF = l;
            wch.NoteNumber = h;

            wopm((byte)(0x30 + e), l);
            wopm((byte)(0x28 + e), KTABLE[h]);
        }


        private void EAM(Ch wch, byte e)
        {

            //Console.WriteLine($"softPMProcCount[{wch.softPMProcCount:d}]");
            //Console.WriteLine($"softPMStep[{wch.softPMStep:d}]");
            //Console.WriteLine($"softPMStepCount[{wch.softPMStepCount:d}]");
            //Console.WriteLine($"softPMPitch[{wch.softPMPitch:d}]");

            if (wch.softAMDelayCount - 1 != 0)
            {
                wch.softAMDelayCount--;
                return;
            }

            //EAM1:
            wch.softAMStepCount--;
            if (wch.softAMStepCount == 0)
            {
                //EAMS:
                wch.softAMStepCount = wch.softAMStep;
                wch.softAMProcCount ^= 1;
            }

            //EAML:
            for (int b = 0; b < 4; b++)
            {
                if ((byte)(wch.softAMSelOP & (1 << b)) == 0) continue;

                switch (b)
                {
                    case 0:
                        if (wch.softAMProcCount == 0) wch.OP1TLs += wch.softAMDepth;
                        else wch.OP1TLs -= wch.softAMDepth;
                        wopm((byte)(0x60 + e), wch.OP1TLs);
                        break;
                    case 1:
                        if (wch.softAMProcCount == 0) wch.OP2TLs += wch.softAMDepth;
                        else wch.OP2TLs -= wch.softAMDepth;
                        wopm((byte)(0x70 + e), wch.OP2TLs);
                        break;
                    case 2:
                        if (wch.softAMProcCount == 0) wch.OP3TLs += wch.softAMDepth;
                        else wch.OP3TLs -= wch.softAMDepth;
                        wopm((byte)(0x68 + e), wch.OP3TLs);
                        break;
                    case 3:
                        if (wch.softAMProcCount == 0) wch.OP4TLs += wch.softAMDepth;
                        else wch.OP4TLs -= wch.softAMDepth;
                        wopm((byte)(0x78 + e), wch.OP4TLs);
                        break;
                }
            }

        }


        private void efxp(Ch[] chs)
        {
        }

    }
}
