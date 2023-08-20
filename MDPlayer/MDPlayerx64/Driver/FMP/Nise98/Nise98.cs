using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class Nise98
    {
        private Action<string, object[]> msgWrite = null;
        private Action<ChipDatum> opnaWrite;
        private Register286 regs = null;
        private Memory98 mem = null;
        private Nise286 cpu = null;
        private NiseInt08Timer int08Timer = null;
        private NiseDos dos = null;
        private NisePPZ8 ppz8 = null;

        private fmStatus fmReg088 = null;
        private fmStatus fmReg188 = null;
        private fmStatus fmReg288 = null;
        private fmStatus fmReg388 = null;
        private int V_SYNC_cnt = 2;
        private byte V_SYNC = 0;
        private ushort mojiCode;
        private byte lineCount;
        private byte mojiPattern;
        private byte pA460h;
        private byte Mute86PCM = 0;

        private int step = 0;
        private int functionCallTimes = 0;

        public enum enmOngenBoardType
        {
            None = 0,
            PC9801_26K,
            SpeakBoard,
            PC9801_86B
        }

        public class fmStatus
        {
            //bit76:11 int 5(IRQ12)(factory)
            //bit76:10 int 4(IRQ10)
            //bit76:01 int 6(IRQ13)
            //bit76:00 int 0(IRQ03)
            public byte Int = 0b1100_0000;
            public byte p88lastAdr = 0;
            public byte p8clastAdr = 0;
            public bool IsBusy = false;
            public bool IsTimerBOverFlow = true;
            public bool IsTimerAOverFlow = false;
            public byte[]? regs;
            public byte[]? AdpcmMem;
            public byte AdpcmPtr = 0;
            public bool AdpcmReadMode = false;
            public MNDRV.FMTimer timer = null;

            //ongenBoardType
            public enmOngenBoardType ongen = enmOngenBoardType.SpeakBoard;

            public fmStatus(enmOngenBoardType ongen)
            {
                this.ongen = ongen;
                if (ongen == enmOngenBoardType.PC9801_26K)
                {
                    regs = new byte[256 * 1];
                }
                else if (ongen == enmOngenBoardType.PC9801_86B)
                {
                    regs = new byte[256 * 2];
                    AdpcmMem = null;
                }
                else if (ongen == enmOngenBoardType.SpeakBoard)
                {
                    regs = new byte[256 * 2];
                    AdpcmMem = new byte[256];
                }
                else
                {
                    regs = null;
                    AdpcmMem = null;
                }

                timer = new MNDRV.FMTimer(false, null, 7987200);// OPNATimer(55467, 7987200);
            }
        }

        public void Init(Action<string, object[]> msgWrite, Action<ChipDatum> opnaWrite, enmOngenBoardType ongen = enmOngenBoardType.PC9801_86B)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>Init");

            this.opnaWrite = opnaWrite;
            regs = new Register286();
            mem = new Memory98(16 * 64 * 1024);
            dos = new NiseDos(regs, mem);
            cpu = new Nise286(this);
            int08Timer = new NiseInt08Timer(cpu);
            ppz8 = new NisePPZ8(this);

            if (ongen == enmOngenBoardType.None)
            {
                fmReg088 = new fmStatus(enmOngenBoardType.None);
                fmReg188 = new fmStatus(enmOngenBoardType.None);
                fmReg288 = new fmStatus(enmOngenBoardType.None);
                fmReg388 = new fmStatus(enmOngenBoardType.None);
                pA460h = 0xfc;
            }
            else if (ongen == enmOngenBoardType.PC9801_26K)
            {
                fmReg088 = new fmStatus(enmOngenBoardType.None);
                fmReg188 = new fmStatus(enmOngenBoardType.PC9801_26K);
                fmReg288 = new fmStatus(enmOngenBoardType.None);
                fmReg388 = new fmStatus(enmOngenBoardType.None);
                pA460h = 0xfc;
            }
            else if (ongen == enmOngenBoardType.PC9801_86B)
            {
                fmReg088 = new fmStatus(enmOngenBoardType.None);
                fmReg188 = new fmStatus(enmOngenBoardType.PC9801_86B);
                fmReg288 = new fmStatus(enmOngenBoardType.None);
                fmReg388 = new fmStatus(enmOngenBoardType.None);
                pA460h = 0b0100_0001;
            }
            else if (ongen == enmOngenBoardType.SpeakBoard)
            {
                fmReg088 = new fmStatus(enmOngenBoardType.SpeakBoard);
                fmReg188 = new fmStatus(enmOngenBoardType.None);
                fmReg288 = new fmStatus(enmOngenBoardType.None);
                fmReg388 = new fmStatus(enmOngenBoardType.None);
                pA460h = 0xfc;
            }
        }

        public NiseDos GetDos() { return dos; }

        public Register286 GetRegisters()
        {
            return regs;
        }

        public Memory98 GetMem()
        {
            return mem;
        }

        public NisePPZ8 GetPPZ8()
        {
            return ppz8;
        }

        public Nise286 GetCPU()
        {
            return cpu;
        }

        public void UserINT(UserInt ui)
        {
            cpu.AddUserInt(ui);
        }

        public bool Execute()
        {
            return false;
        }


        public void Runtimer()
        {
            fmReg088.timer.timer();
            fmReg188.timer.timer();
            fmReg288.timer.timer();
            fmReg388.timer.timer();
        }

        public bool IntTimer()
        {
            return (
                (fmReg088.timer.ReadStatus() & 3)
                | (fmReg188.timer.ReadStatus() & 3)
                | (fmReg288.timer.ReadStatus() & 3)
                | (fmReg388.timer.ReadStatus() & 3)
                ) != 0;
        }


        public int StepExecute()
        {
            int08Timer.StepExecute();

            int waitClock = cpu.StepExecute();
            return waitClock;
        }

        public byte INPb(ushort port)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>IN  Port:${0:X04}", port);
            switch (port)
            {
                case 0x0000://Master interrupt Controler
                    return 0;
                case 0x0002://Interrupt controler Master
                    return cpu.w_mmsk;
                case 0x0008://Slave  interrupt Controler
                    return 0;
                case 0x000a://Interrupt controler slave
                    return cpu.w_smsk;
                case 0x00a0://graphics GDC status read
                    //bit5:V_SYNC
                    V_SYNC_cnt--;
                    if (V_SYNC_cnt == 0)
                    {
                        V_SYNC_cnt = 2;
                        V_SYNC ^= 0x20;
                    }
                    return V_SYNC;

                case 0x088://FM port
                case 0x08a://FM port
                case 0x08c://FM port
                case 0x08e://FM port
                    return FMPortInport(fmReg088, port);

                case 0x188://FM port
                case 0x18a://FM port
                case 0x18c://FM port
                case 0x18e://FM port
                    return FMPortInport(fmReg188, port);

                case 0x288://FM port
                case 0x28a://FM port
                case 0x28c://FM port
                case 0x28e://FM port
                    return FMPortInport(fmReg288, port);

                case 0x388://FM port
                case 0x38a://FM port
                case 0x38c://FM port
                case 0x38e://FM port
                    return FMPortInport(fmReg388, port);

                case 0xa460:
                    //FMP の場合
                    //0b0000_0001　DO+      188h
                    //0b0001_0001　73ボード 188h
                    //0b0010_0001　73ボード 188h
                    //0b0011_0001　73ボード 288h
                    //0b0100_0001　86ボード 188h
                    //0b0101_0001　86ボード 288h
                    //0b0110_0001　YMF288   188h(ADPCM有り:SPB 188h
                    //0b0111_0001　YMF288   188h(ADPCM有り:SPB 188h
                    //0b1000_0001　SPB      088h
                    //0b1001_0001　x ハング (もともとこの値を返すことは無いと思われる)
                    //0b1010_0001　以降、YMF288   0x188h 恐らく後続の処理でさらに調べていると思われる
                    //       ~~~~ここはFMPは完全無視(但し0xffだった場合は判定処理が終わる.恐らく後続の処理でさらに調べていると思われる)

                    if (fmReg188.ongen == enmOngenBoardType.None) return 0xff;
                    else if (fmReg188.ongen == enmOngenBoardType.PC9801_26K) return 0xff;
                    else if (fmReg188.ongen == enmOngenBoardType.PC9801_86B) return 0b0100_0001;
                    else if (fmReg088.ongen == enmOngenBoardType.SpeakBoard) return 0b1000_0001;

                    return 0xff;

                case 0xa66e:
                    return Mute86PCM;

                default:
                    throw new NotImplementedException(string.Format("Request port:${0:X04}", port));
            }
        }

        public short INPw(ushort port)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>IN  Port:${0:X04}", port);
            switch (port)
            {
                //case 0xa460:
                //    return IsOPNA ? 0x00 : 0xff;//0xFF:not OPNA
                //case 0x088://FM port
                //    return IsSPB ? 0x01 : 0x00;
                //case 0x188://FM port
                //    return IsSPB ? 0x01 : 0x00;
                //case 0x288://FM port
                //    return IsSPB ? 0x01 : 0x00;
                //case 0x388://FM port
                //    return IsSPB ? 0x01 : 0x00;
                default:
                    throw new NotImplementedException(string.Format("Request port:${0:X04}", port));
            }
        }

        public void OUTPb(ushort port, byte data)
        {
            switch (port)
            {
                case 0x00://Initialize interrupt
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X02}", port);
                    break;
                case 0x02:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X02}", port);
                    cpu.w_mmsk = data;
                    break;
                case 0x08://Slave interrupt Controler
                    break;
                case 0x0a:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X02}", port);
                    cpu.w_smsk = data;
                    break;
                case 0x5f://WAIT 0.6マイクロ秒以上のウエイト
                    break;
                case 0x68://モードF/Fレジスタ1 http://www.webtech.co.jp/company/doc/undocumented_mem/io_disp.txt
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X02}", port);
                    // 0000101nb: KAC Mode ドットアクセスモード
                    break;
                case 0x71://TIMER: Counter#0 R/W
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X02}", port);
                    cpu.interruptTrigger[8] = true;
                    int08Timer.Start();
                    break;
                case 0x77://TIMER: Set Mode
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X02}", port);
                    break;
                case 0xa1://文字コード第2バイト
                    mojiCode = (ushort)((mojiCode & 0x00ff) | (data << 8));
                    break;
                case 0xa3://文字コード第1バイト
                    mojiCode = (ushort)((mojiCode & 0xff00) | data);
                    break;
                case 0xa5://ラインカウンタ
                    lineCount = data;
                    break;
                case 0xa9://文字パターン書き込み
                    mojiPattern = data;
                    break;

                case 0x088://FM port adr
                case 0x08a://FM port val
                case 0x08c://FM port val
                case 0x08e://FM port val
                    FMPortOutport(fmReg088, port, data);
                    break;
                case 0x188://FM port adr
                case 0x18a://FM port val
                case 0x18c://FM port val
                case 0x18e://FM port val
                    FMPortOutport(fmReg188, port, data);
                    break;
                case 0x288://FM port adr
                case 0x28a://FM port adr
                case 0x28c://FM port adr
                case 0x28e://FM port adr
                    FMPortOutport(fmReg288, port, data);
                    break;
                case 0x388://FM port adr
                case 0x38a://FM port adr
                case 0x38c://FM port adr
                case 0x38e://FM port adr
                    FMPortOutport(fmReg388, port, data);
                    break;

                case 0xa460://OPNA制御
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98> --- 0xA460(OPNA制御) Val:${0:X02}", data);
                    //bit 1: 0:2608をunMask  1:2608をMask
                    //bit 0: 0:2608を2203相当にする  1:2608を2608として使用する
                    //bit2～　未使用
                    pA460h = data;
                    break;
                case 0xa66e:
                    //bit0: 1->ミュートする
                    Mute86PCM = data;
                    break;
                default:
                    throw new NotImplementedException(string.Format("<Nise98>Request port:${0:X04}", port));
            }
        }

        public void OUTPw(ushort port, short data)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98>OUT Port:${0:X04}", port);

            switch (port)
            {
                case 0x5f://WAIT 0.6マイクロ秒以上のウエイト
                    break;
                case 0x088://FM port
                    break;
                case 0x188://FM port
                    break;
                case 0x288://FM port
                    break;
                case 0x388://FM port
                    break;
                case 0xa460://OPNA Info bit0,1をクリアするとOPNA機能を抑制できるっぽい
                    break;
                default:
                    throw new NotImplementedException(string.Format("Request port:${0:X04}", port));
            }
        }


        private byte FMPortInport(fmStatus fs, ushort port)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98> --- IN  FM Port:${0:X03}", port);
            switch (port & 0xff)
            {
                case 0x88://FM port
                    if (fs.ongen == enmOngenBoardType.None)
                        return 0xff;

                    byte ret = (byte)(fs.timer.ReadStatus() |
                        (fs.IsBusy ? 0x80 : 0x00));
                    //| (fs.IsTimerBOverFlow ? 0x02 : 00)
                    //| (fs.IsTimerAOverFlow ? 0x01 : 0x00)
                    //);
                    return ret;

                case 0x8a://FM port
                    if (fs.ongen == enmOngenBoardType.None)
                        return 0xff;

                    if (fs.p88lastAdr == 0x0e)
                        return fs.Int;
                    else if (fs.p88lastAdr == 0xff)
                        return (byte)(fs.ongen == enmOngenBoardType.PC9801_26K ? 0x00 : 0x01);
                    else
                        return (byte)(fs.regs != null ? fs.regs[fs.p88lastAdr] : 0x00);

                case 0x8c://FM port
                    //fs.AdpcmPtr++;
                    if (fs.ongen == enmOngenBoardType.None
                        || fs.ongen == enmOngenBoardType.PC9801_26K)
                        return 0xff;
                    if (fs.ongen == enmOngenBoardType.PC9801_86B && (pA460h & 3) == 0)
                        return 0xff;

                    return (byte)(fs.timer.ReadStatus() |
                        (fs.IsBusy ? 0x80 : 0x00)
                        | 0x08 //bit3:BRDY 
                               //| (fs.IsTimerBOverFlow ? 0x02 : 00)
                               //| (fs.IsTimerAOverFlow ? 0x01 : 0x00)
                        );

                case 0x8e://FM port
                    if (fs.ongen == enmOngenBoardType.None
                        || fs.ongen == enmOngenBoardType.PC9801_26K)
                        return 0xff;
                    if (fs.ongen == enmOngenBoardType.PC9801_86B && (pA460h & 3) == 0)
                        return 0xff;

                    if (fs.p8clastAdr == 0x08 && fs.AdpcmMem != null)
                        return fs.AdpcmMem[fs.AdpcmPtr++];

                    return 0x00;

                default:
                    throw new NotImplementedException(string.Format("Request port:${0:X04}", port));
            }
        }

        private void FMPortOutport(fmStatus fs, ushort port, byte data)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<Nise98> --- OUT FM Port:{0:X03} Dat:${1:X02}", port, data);
            if (fs.ongen == enmOngenBoardType.None) return;

            ChipDatum cd;
            switch (port & 0xff)
            {
                case 0x088://FM port adr
                    fs.p88lastAdr = data;
                    break;
                case 0x08a://FM port val
                    if (fs.regs != null) fs.regs[fs.p88lastAdr] = data;
                    fs.timer.WriteReg(fs.p88lastAdr, data);
                    cd = new ChipDatum(port, fs.p88lastAdr, data);
                    opnaWrite(cd);
                    break;
                case 0x08c://FM port val
                    fs.p8clastAdr = data;
                    //if (fs.p8clastAdr == 0x08)
                    //{
                    //}
                    break;
                case 0x08e://FM port val
                    if (fs.regs != null && fs.regs.Length == 512)
                    {
                        fs.regs[256 + fs.p8clastAdr] = data;
                    }
                    if (fs.p8clastAdr == 0x00)
                    {
                        if (data == 0x20)
                        {
                            fs.AdpcmReadMode = true;
                        }
                    }
                    if (fs.p8clastAdr == 0x02)
                    {
                        fs.AdpcmPtr = data;
                    }
                    else if (fs.p8clastAdr == 0x03)
                    {
                        if (fs.AdpcmReadMode) fs.AdpcmPtr -= 2;
                        //fs.AdpcmPtr = data;
                    }
                    else if (fs.p8clastAdr == 0x08)
                    {
                        if (fs.AdpcmMem != null) fs.AdpcmMem[fs.AdpcmPtr++] = data;
                    }
                    else if (fs.p8clastAdr == 0x10)
                    {
                        //if (data == 0x13) fs.AdpcmPtr++;
                    }
                    cd = new ChipDatum(port, fs.p8clastAdr, data);
                    opnaWrite(cd);
                    break;
                default:
                    throw new NotImplementedException(string.Format("<Nise98>Request port:${0:X04}", port));
            }
        }


        public int LoadRun(string filename, string option, int startSegment, bool dispReg = false, bool useStepCounter = false, bool dispStepCounter = false,
    long MaxStepCounter = 100_000_000, long StartStepCounterForDispStep = 0)
        {
            dos.LoadAndExecuteFile(filename, option, startSegment);
            Register286 regs = GetRegisters();
            if (dispReg) DispRegs(regs);

            while (((useStepCounter && step < MaxStepCounter) || !useStepCounter) && !dos.programTerminate)
            {
                int waitClock = StepExecute();

                if (useStepCounter)
                {
                    step++;
                    if (step < StartStepCounterForDispStep) continue;
                }

                if (dispReg)
                {
                    regs = GetRegisters();
                    DispRegs(regs);
                }

                if (dispStepCounter) Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "STEP:{0}\r\n", step);

                if ((ushort)regs.IP == 0xb35d)
                {
                    ;
                }
            }

            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "Terminate program. return code=${0:X02}", dos.returnCode);
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "");

            return dos.returnCode;
        }

        public void CallRunfunctionCall(byte intnumber, bool dispReg = false, bool useStepCounter = false, bool dispStepCounter = false,
    long MaxStepCounter = 100_000_000, long StartStepCounterForDispStep = 0)
        {
            Register286 regs = GetRegisters();
            UserInt ui = new UserInt();
            ui.intNum = intnumber;
            UserINT(ui);
            //regs.IF=false;
            cpu.w_mmsk = 0xff;
            cpu.w_smsk = 0xff;
            dos.programTerminate = false;

            functionCallTimes++;

            do
            {
                int waitClock = StepExecute();

                if (useStepCounter)
                {
                    step++;
                    if (step < StartStepCounterForDispStep) continue;
                }

                if (dispReg)
                {
                    regs = GetRegisters();
                    DispRegs(regs);
                }

                //if (dispStepCounter) Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "functionCalls:{0} STEP:{1}\r\n", functionCallTimes, step);

                //if (regs.IP == 0xb01)
                //{
                //    if(regs.AL==0xf0|| regs.AL == 0xcd)
                //    ;
                //}
                //if (regs.IP == 0x484d)// || regs.IP == 0x19c9 || regs.IP == 0x19d1)
                //{
                //    ;
                //}

            } while (regs.CS != 0 || regs.IP != 0);

            //Log.WriteLine(LogLevel.Debug, "Terminate function call");
            //Log.WriteLine(LogLevel.Debug, "");

        }

        private void DispRegs(Register286 regs)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, regs?.ToString());
        }

    }
}
