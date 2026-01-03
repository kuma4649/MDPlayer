using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class nise68
    {
        public niseHuman hmn = null;
        public Memory68 mem = null;
        public Register68 reg = null;
        private niseM68 cpu = null;
        private niseIOCS iocs = null;
        private midiBoard[] midiBoard = null;
        private FileMng fileMng = null;
        private Func<int, byte, int> scc;
        private scc_A scc_A = null;

        private int step = 0;
        private int run = 0;
        private int cmd = 0;
        private int ocmd = -1;

        private uint IOCSCallAddress = 0xfe0000;
        private byte hkVsyncVal = 0;
        private byte hkOPMAdr;
        private byte hkOPMDat;

        public Func<int, int> MPCM = null;
        public Func<int, int, int> OPM = null;
        public Func<int, byte, int> MIDI = null;


        public void Init(List<string> envZPDs,bool isVer2,FileMng fm)
        {
            mem = new Memory68();
            reg = new Register68();
            hmn = new niseHuman(mem, reg, envZPDs, fm);
            fileMng = fm;
            cpu = new niseM68(mem, reg);
            cpu.hmn = hmn;
            iocs = new niseIOCS(mem, reg);

            //IOCS address
            mem.PokeL(0xbc, IOCSCallAddress);
            mem.PokeL(0x238, IOCSCallAddress);

            mem.PokeL(0x228, IOCSCallAddress);
            mem.PokeL(0x230, IOCSCallAddress);
            mem.PokeL(0x2a8, IOCSCallAddress);
            mem.PokeL(0x2b0, IOCSCallAddress);

            if (isVer2)
            {
                //ZMUSIC v2 向け
                mem.PokeL(0x10c, 0xfe0004);
                mem.PokeL(0x7c0, 0xfe0004);
                mem.PokeL(0x1a8, 0xfe0004);
                mem.PokeL(0x088, niseHuman.mpcmPtr);
                mem.PokeL(niseHuman.mpcmPtr - 0x08, 0x5043_4d00);
            }
            else
            {
                //MPCM関連
                mem.PokeL(0x84, niseHuman.mpcmPtr);//Trapの位置が書いてあるんかな？
                mem.PokeL(niseHuman.mpcmPtr - 0x8, 0x4d50434d);//'MPCM'
            }

            mem.hookList.Add(new memhook(0xe88001, 0xe88001, hkVsync, null));//vsync?
            mem.hookList.Add(new memhook(0xe90001, 0xe90003, hkOPMr, hkOPMw));//opm
            mem.hookList.Add(new memhook(0xeafa00, 0xeafa1f, hkCZ6BM1fr, hkCZ6BM1fw));//1st/2nd CZ-6BM1(MIDI)
            //mem.hookList.Add(new memhook(0xe9a001, 0xe9a001, hkDummy, null));//midiwait向け
            mem.hookList.Add(new memhook(0xe98005, 0xe98007, hkSCC_Ar, hkSCC_Aw));//SCC(シリアルコミュニケーションコントローラ) ChA(RS-232C)

            step = 0;
            run = 0;
        }


        public int LoadRun(string filename, string option, uint startAddress, bool dispReg = false, bool useStepCounter = false, bool dispStepCounter = false,
    long MaxStepCounter = 100_000_000, long StartStepCounterForDispStep = 0)
        {
            //Log.SetLogLevel(LogLevel.Trace);

            hmn.LoadAndExecuteFile(filename, option, startAddress);
            if (dispReg) DispRegs(reg);

            int waitClock = 0;
            hmn.programTerminate = false;
            step = 0;
            run++;

            while (((useStepCounter && step < MaxStepCounter) || !useStepCounter) && !hmn.programTerminate)
            {
                waitClock += StepExecute();

                if (useStepCounter)
                {
                    step++;
                    if (step < StartStepCounterForDispStep) continue;
                }

#if DEBUG
                if (dispReg)
                {
                    DispRegs(reg);
                }

                //if (dispStepCounter) Log.WriteLine(LogLevel.Trace, "STEP:{0} totalCycle:{1}\r\n", step, waitClock);

                if (run > 0 && step == 500)
                {
                    ;
                    //Log.SetLogLevel(LogLevel.Trace);
                }

                if (reg.PC == 0x0002_2968)
                {
                    ;
                }
                if (reg.PC == 0x0000002_2982)//コメント読みこみ完了
                {
                    ;
                }
                //if (reg.PC == 0x0000002_22e2)//(で始まるコマンドの処理へ
                //{
                //    ;
                //}

                //if ((reg.PC & 0xffff_fff0) == reg.PC)
                //{
                //    DumpMemory(reg.PC - 0x80, reg.PC + 0x80);
                //}
#endif
            }

            Log.WriteLine(LogLevel.Debug, "Terminate program. return code=${0:X02}", hmn.returnCode);
            Log.WriteLine(LogLevel.Debug, "  alloc count ={0:d}", hmn.memMng.allocCount);
            Log.WriteLine(LogLevel.Debug, "");

            return hmn.returnCode;
        }

        public void Trap(int num, bool dispReg = false, bool useStepCounter = false, bool dispStepCounter = false,
    long MaxStepCounter = 100_000_000, long StartStepCounterForDispStep = 0)
        {
            if (dispReg) DispRegs(reg);

            int waitClock = 0;
            hmn.programTerminate = false;
            step = 0;
            run++;

            reg.SSP = hmn.defSSP;
            reg.USP = hmn.defUSP;
            cpu.Ctrap2((ushort)num);

            while (((useStepCounter && step < MaxStepCounter) || !useStepCounter) && !hmn.programTerminate)
            {
                waitClock += StepExecute();

#if DEBUG
                if (useStepCounter)
                {
                    step++;
                    if (step < StartStepCounterForDispStep) continue;
                }

                if (dispReg)
                {
                    DispRegs(reg);
                }

                if (dispStepCounter) Log.WriteLine(LogLevel.Trace, "STEP:{0} totalCycle:{1}\r\n", step, waitClock);

                if (run > 8 && step == 146)
                {
                    //Log.SetLogLevel(LogLevel.Trace);
                }

                //if (run > 0 && step == 249)
                //{
                //    //Log.SetLogLevel(LogLevel.Trace);
                //}

                if (run > 0 && (reg.PC == 0x0002_e9fe
                    ))
                {
                    ;
                }

                ////コマンド毎のデバッグ向け
                //if (run > 0 && reg.PC == 0x0003_07ba)//D7->コマンド番号
                //{
                //    cmd++;
                //}
                //if (cmd>=14 && cmd!=ocmd)//D7->コマンド番号
                //{
                //    ;
                //    ocmd = cmd;
                //}
#endif
            }

#if DEBUG
            Log.WriteLine(LogLevel.Debug, "Terminate program. return code=${0:X02} runs={1}", hmn.returnCode, run);
            //Log.WriteLine(LogLevel.Debug, "  alloc count ={0:d}", hmn.memMng.allocCount);
            //Log.WriteLine(LogLevel.Debug, "");
#endif
        }

        public void TrapOPM(bool dispReg = false, bool useStepCounter = false, bool dispStepCounter = false,
    long MaxStepCounter = 100_000_000, long StartStepCounterForDispStep = 0)
        {
            if (dispReg) DispRegs(reg);

            int waitClock = 0;
            hmn.programTerminate = false;
            step = 0;
            run++;

            reg.SSP = hmn.defSSP;
            reg.USP = hmn.defUSP;
            cpu.CtrapPtr(iocs.interruptOPM);

            while (((useStepCounter && step < MaxStepCounter) || !useStepCounter) && !hmn.programTerminate)
            {
                waitClock += StepExecute();

#if DEBUG
                if (useStepCounter)
                {
                    step++;
                    if (step < StartStepCounterForDispStep) continue;
                }

                if (dispReg)
                {
                    DispRegs(reg);
                }

                if (dispStepCounter) Log.WriteLine(LogLevel.Trace, "STEP:{0} totalCycle:{1}\r\n", step, waitClock);

                //if (run > 3082 && step == 1827)
                //{
                //    //Log.SetLogLevel(LogLevel.Trace);
                //}

                //if (run > 0 && step == 249)
                //{
                //    //Log.SetLogLevel(LogLevel.Trace);
                //}

                //if (run >0 && (reg.PC == 0x0002_f350
                //    ))
                //{
                //    ;
                //}

                ////コマンド毎のデバッグ向け
                //if (run > 0 && reg.PC == 0x0003_07ba)//D7->コマンド番号
                //{
                //    cmd++;
                //}
                //if (cmd>=14 && cmd!=ocmd)//D7->コマンド番号
                //{
                //    ;
                //    ocmd = cmd;
                //}
#endif
            }

#if DEBUG
            Log.WriteLine(LogLevel.Debug, "Terminate program. return code=${0:X02} runs={1}", hmn.returnCode, run);
            //Log.WriteLine(LogLevel.Debug, "  alloc count ={0:d}", hmn.memMng.allocCount);
            //Log.WriteLine(LogLevel.Debug, "");
#endif
        }


        public void DumpMemory(uint startAddress, uint endAddress)
        {
            string str = "${0:X08}: {1:X02} {2:X02} {3:X02} {4:X02} : {5:X02} {6:X02} {7:X02} {8:X02} : {9:X02} {10:X02} {11:X02} {12:X02} : {13:X02} {14:X02} {15:X02} {16:X02}";
            startAddress &= 0xffff_fff0;
            endAddress = (endAddress & 0xffff_fff0) + 0x10;
            for (uint i = startAddress; i < endAddress; i += 16)
            {
                Log.WriteLine(LogLevel.Trace2, str
                    , i
                    , mem.PeekB(i), mem.PeekB(i + 1), mem.PeekB(i + 2), mem.PeekB(i + 3)
                    , mem.PeekB(i + 4), mem.PeekB(i + 5), mem.PeekB(i + 6), mem.PeekB(i + 7)
                    , mem.PeekB(i + 8), mem.PeekB(i + 9), mem.PeekB(i + 10), mem.PeekB(i + 11)
                    , mem.PeekB(i + 12), mem.PeekB(i + 13), mem.PeekB(i + 14), mem.PeekB(i + 15)
                    );
            }
        }

        private int StepExecute()
        {
            if (reg.PC == 0)
            {
                hmn.programTerminate = true;
                //Log.WriteLine(LogLevel.Information, "PC is Zero. program terminate.");
                return 0;
            }
            else if (reg.PC == IOCSCallAddress)
            {
                iocs.Call();
                return 0;
            }
            else if (reg.PC == niseHuman.mpcmPtr)
            {
                Mpcm();
                return 0;
            }


            int waitClock = cpu.StepExecute();
            return waitClock;
        }

        private void Mpcm()
        {
            Log.WriteLine(LogLevel.Trace, "Call MPCM function ");

            int n = reg.GetDw(0);

            int ret = MPCM(n);
            reg.SetDl(0, (uint)ret);

            reg.SR = mem.PeekW(reg.SSP);
            reg.SSP += 2;
            reg.PC = mem.PeekL(reg.SSP);
            reg.SSP += 4;
        }

        private void DispRegs(Register68 regs)
        {
            Log.WriteLine(LogLevel.Trace, regs?.ToString());
        }

        private uint hkVsync(uint ptr)
        {
            hkVsyncVal ^= 0x90;
            return hkVsyncVal;
        }


        private uint hkOPMr(uint ptr)
        {
            if (ptr == 0x00e90003)
            {
                return 0;
            }

            return 0;
#if DEBUG

            throw new NotImplementedException();
#else
            return 0;
#endif

        }

        private bool hkOPMw(uint ptr, byte dat)
        {
            if (ptr == 0x00e90001)
            {
                hkOPMAdr = dat;
                return false;
            }
            else if (ptr == 0x00e90003)
            {
                hkOPMDat = dat;
                WriteOPM(hkOPMAdr, hkOPMDat);
                return false;
            }

#if DEBUG

            throw new NotImplementedException();
#else
            return false;
#endif

        }

        private uint hkCZ6BM1fr(uint ptr)
        {
            int n = (ptr & 0x10) != 0 ? 1 : 0;
            return midiBoard[n].Read((byte)ptr);
        }

        private bool hkCZ6BM1fw(uint ptr, byte dat)
        {
            int n = (ptr & 0x10) != 0 ? 1 : 0;
            return midiBoard[n].Write((byte)ptr, dat);
        }

        private uint hkSCC_Ar(uint ptr)
        {
            return scc_A.Read(ptr);
        }

        private bool hkSCC_Aw(uint ptr, byte dat)
        {
            return scc_A.Write(ptr, dat);
        }


        private void WriteOPM(byte hkOPMAdr, byte hkOPMDat)
        {
            Log.WriteLine(LogLevel.Trace2, "Write OPM Adr:${0:x02} Dat:${1:x02}", hkOPMAdr, hkOPMDat);
            OPM(hkOPMAdr, hkOPMDat);
        }

        public bool IntTimer()
        {
            return midiBoard[0].IntTimer();
        }

        public void SetMPCM(Func<int, int> MPCM)
        {
            this.MPCM = MPCM;
        }
        public void SetOPM(Func<int, int, int> OPM)
        {
            this.OPM = OPM;
        }

        public void SetMIDI(Func<int, byte, int> MIDI, int renderingFreq)
        {
            this.MIDI = MIDI;
            midiBoard = new midiBoard[2] { new midiBoard(0, renderingFreq, MIDI), new midiBoard(1, renderingFreq, MIDI) };
        }

        public void SetSCC_A(Func<int, byte, int> SCC, int renderingFreq)
        {
            this.scc = SCC;
            scc_A = new scc_A(renderingFreq, scc);
        }
    }
}