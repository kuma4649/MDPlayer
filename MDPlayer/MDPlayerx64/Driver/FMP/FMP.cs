using MDPlayerx64;
using MDPlayer.Driver.FMP.Nise98;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;
using static MDSound.fm._ssg_callbacks;
using MDPlayer.Driver.MNDRV;
using System.Text.RegularExpressions;
using System.Net;

namespace MDPlayer.Driver.FMP
{
    public class FMP : baseDriver
    {
        public static readonly uint baseclock = 7987200;
        private int step = 0;
        private Nise98.Nise98 nise98 = new();
        private Register286 regs;
        private myEncoding enc = new myEncoding();
        private string searchPath="";
        private List<string> searchPaths = null;

        public string PlayingFileName { get; set; }

        public FMP()
        {
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();

            try
            {
                if (buf != null && buf.Length > 2)
                {
                    uint ptr = Common.getLE16(buf, 0) + 4;//4 'FMC'+version(1byte)
                    string comment = Common.getNRDString(buf, ref ptr);
                    ret.TrackName = comment;
                    ret.TrackNameJ = ret.TrackName;

                }
            }
            catch
            {
                ret.TrackName = "";
                ret.TrackNameJ = "";
            }

            return ret;
        }

        public void SetSearchPath(string searchPath)
        {
            try
            {
                this.searchPath = searchPath;
                //環境変数"PVI"を取得する
                string pvi="";
                try
                {
                    pvi = Environment.GetEnvironmentVariable("PVI", System.EnvironmentVariableTarget.User);
                    if (!string.IsNullOrEmpty(pvi)) this.searchPath += ";" + pvi;
                }
                catch
                {
                    this.searchPath = searchPath;
                }
                IEnumerable<string> fileSerachPaths =
                    this.searchPath
                        .Split(';')
                        .Where(path => !String.IsNullOrEmpty(path));
                searchPaths = fileSerachPaths.ToList();
            }
            catch
            {
                this.searchPath = searchPath;
            }

        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            GD3 = getGD3Info(vgmBuf, 0);
            this.chipRegister = chipRegister;
            LoopCounter = 0;
            vgmCurLoop = 0;
            this.model = model;
            vgmFrameCounter = -latency - waitTime;

            try
            {
                Run(vgmBuf);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            try
            {
                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    Counter++;
                    vgmFrameCounter++;

                    nise98.Runtimer();
                    if (!nise98.IntTimer()) continue;
                    regs.SS = unchecked((short)0xE000);
                    regs.SP = 0x0000;
                    nise98.CallRunfunctionCall(0x14);

                    //演奏チェック
                    regs.AX = 0x0004;
                    regs.SS = unchecked((short)0xE000);
                    regs.SP = 0x0000;
                    nise98.CallRunfunctionCall(0xd2);
                    if (regs.AX == 0)
                        Stopped = true;

                    //内部ワークアドレス取得し、曲ループ回数をチェックする
                    regs.AX = 0x1104;
                    regs.SS = unchecked((short)0xE000);
                    regs.SP = 0x0000;
                    nise98.CallRunfunctionCall(0xd2);
                    int ptr = ((ushort)0x2000 << 4) + (ushort)regs.AX;
                    int FmpSloop_c = nise98.GetMem().PeekB(ptr + 0x17);
                    vgmCurLoop = (uint)FmpSloop_c;


                }

                //vgmCurLoop = mm.ReadUInt16(reg.a6 + dw.LOOP_COUNTER);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

        }

        private void Run(byte[] vgmBuf)
        {
            var fileNameFMP = "FMP.COM";
            var fileNamePPZ8 = "PPZ8.COM";

            nise98.Init(null, OPNAWrite, Nise98.Nise98.enmOngenBoardType.SpeakBoard);//.PC9801_86B);//.SpeakBoard);//.PC9801_26K);

            //nise98.GetDos().SetSearchPath(searchPaths);

            //FMPの常駐
            Log.level = musicDriverInterface.LogLevel.INFO;
            nise98.LoadRun(fileNameFMP, "s -s", 0x2000);//, true, true, true, 3_000_000, 108213);//108213->wait loopぬけ
            regs = nise98.GetRegisters();

            //nisePPZ8の常駐
            Log.level = musicDriverInterface.LogLevel.INFO;
            step = 0;
            Memory98 mem = nise98.GetMem();
            nise98.GetPPZ8().FMPRegistPPZ8(out step, out regs);
            nise98.GetPPZ8().SetCallBack(SetPPZ8PCMData, SetPPZ8Data);

            //曲データ読み込みと演奏開始通知
            //
            Log.level = musicDriverInterface.LogLevel.INFO;
            FMPLoadAndPlayFileAL2(nise98.GetDos(), regs);

        }

        private void SetPPZ8PCMData(int bank, int mode, byte[][] pcmdata)
        {
            chipRegister.PPZ8LoadPcm(0, (byte)bank, (byte)mode, pcmdata,model);
        }

        private void SetPPZ8Data(int port, int adr, int data)
        {
            chipRegister.PPZ8Write(0, port, adr, data, model);
        }

        private void OPNAWrite(ChipDatum dat)
        {
            byte cn = (byte)(dat.port >> 8);
            byte port = (byte)((byte)dat.port == 0x8a ? 0 : 1);
            chipRegister?.setYM2608Register(0, port, (byte)dat.address, (byte)dat.data, model);
        }

        private void FMPLoadAndPlayFileAL2(NiseDos dos, Register286 regs)
        {
            byte[] m = enc.GetSjisArrayFromString(Path.GetFileName(PlayingFileName));
            dos.SetPath(Path.GetDirectoryName(PlayingFileName));
            dos.LoadImage(m, (0x5000 << 4) + 0x000);

            step = 0;
            regs.AL = 0x02;
            regs.DS = 0x5000;
            regs.DX = 0x0000;
            regs.SS = unchecked((short)0xE000);
            regs.SP = 0x0000;
            nise98.CallRunfunctionCall(0xd2, true, true, true, 10_000_000_000, 0_000);
            Log.WriteLine( musicDriverInterface.LogLevel.DEBUG, "return CF={0} code={1:X02}", regs.CF, regs.AL);
        }

        public bool Compile(string playingFileName)
        {
            var fileNameFMP = "FMP.COM";
            var fileNameFMC = "FMC.EXE";
            int rc = 0;

            nise98.Init(null, OPNAWrite, Nise98.Nise98.enmOngenBoardType.SpeakBoard);//.PC9801_86B);//.SpeakBoard);//.PC9801_26K);

            //FMPの常駐
            Log.level = musicDriverInterface.LogLevel.INFO;
            nise98.LoadRun(fileNameFMP, "s -s", 0x2000);
            regs = nise98.GetRegisters();

            //FMCの実行
            nise98.GetDos().programTerminate = false;
            if ((rc = nise98.LoadRun(fileNameFMC, playingFileName, 0x3000
                //, true, true, true, 3_000_000, 0
                )) != 0) return false;

            return true;
        }
    }
}
