using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MDPlayer.Driver
{
    public class PMDDotNET : baseDriver
    {
        private InstanceMarker im = null;
        private iCompiler PMDCompiler = null;
        private iDriver PMDDriver = null;
        private static string[] envPmd = null;
        private static string[] envPmdOpt = null;
        private bool isNRM;
        private bool isSPB;
        private bool isVA;
        private bool usePPS;
        private bool usePPZ;

        public string PlayingFileName { get; internal set; }
        public const int baseclock = 7987200;
        private enmPMDFileType mtype;

        public PMDDotNET(InstanceMarker PMDDotNET_Im)
        {
            im = PMDDotNET_Im;
        }

        public GD3 getGD3Info(byte[] buf, uint vgmGd3, enmPMDFileType mtype)
        {
            GD3Tag gt;

            if (mtype == enmPMDFileType.MML)
            {
                EnvironmentE env = new EnvironmentE();
                env.AddEnv("pmd");
                env.AddEnv("pmdopt");
                envPmd = env.GetEnvVal("pmd");
                envPmdOpt = env.GetEnvVal("pmdopt");

                PMDCompiler = im.GetCompiler("PMDDotNET.Compiler.Compiler");
                PMDCompiler.SetCompileSwitch((Func<string, Stream>)appendFileReaderCallback);
                gt = PMDCompiler.GetGD3TagInfo(buf);
            }
            else
            {
                PMDDriver = im.GetDriver("PMDDotNET.Driver.Driver");
                //PMDDriver.SetDriverSwitch((Func<string, Stream>)appendFileReaderCallback);
                gt = PMDDriver.GetGD3TagInfo(buf);
            }

            GD3 g = new GD3();
            g.TrackName = gt.dicItem.ContainsKey(enmTag.Title) ? gt.dicItem[enmTag.Title][0] : "";
            g.TrackNameJ = gt.dicItem.ContainsKey(enmTag.TitleJ) ? gt.dicItem[enmTag.TitleJ][0] : "";
            g.Composer = gt.dicItem.ContainsKey(enmTag.Composer) ? gt.dicItem[enmTag.Composer][0] : "";
            g.ComposerJ = gt.dicItem.ContainsKey(enmTag.ComposerJ) ? gt.dicItem[enmTag.ComposerJ][0] : "";
            g.VGMBy = gt.dicItem.ContainsKey(enmTag.Artist) ? gt.dicItem[enmTag.Artist][0] : "";
            g.Converted = gt.dicItem.ContainsKey(enmTag.ReleaseDate) ? gt.dicItem[enmTag.ReleaseDate][0] : "";

            return g;
        }

        public override bool init(byte[] vgmBuf,int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            mtype = fileType == 0 ? enmPMDFileType.MML : enmPMDFileType.M;
            GD3 = getGD3Info(vgmBuf, 0, mtype);

            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;

#if DEBUG
            //実チップスレッドは処理をスキップ(デバッグ向け)
            if (model == EnmModel.RealModel) return true;
#endif

            if (mtype == enmPMDFileType.MML) return initMML();
            else return initM();
        }

        public override void oneFrameProc()
        {

#if DEBUG
            //実チップスレッドは処理をスキップ(デバッグ向け)
            if (model == EnmModel.RealModel)
            {
                Stopped = true;
                return;
            }
#endif

            try
            {
                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    PMDDriver.Rendering();

                    Counter++;
                    vgmFrameCounter++;
                }

                int lp = PMDDriver.GetNowLoopCounter();
                lp = lp < 0 ? 0 : lp;
                vgmCurLoop = (uint)lp;

                if (PMDDriver.GetStatus() < 1)
                {
                    //if (PMDDriver.GetStatus() == 0)
                    //{
                        //Thread.Sleep((int)(latency * 2.0));//実際の音声が発音しきるまでlatency*2の分だけ待つ
                    //}
                    Stopped = true;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }



        public enum enmPMDFileType
        {
            unknown,
            MML,
            M
        }



        private enmPMDFileType CheckFileType(byte[] buf)
        {
            if (buf == null || buf.Length < 4)
            {
                return enmPMDFileType.unknown;
            }

            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x43
                && buf[3] == 0x38)
            {
                return enmPMDFileType.M;
            }
            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x42
                && buf[3] == 0x38)
            {
                return enmPMDFileType.M;
            }

            return enmPMDFileType.MML;
        }

        private bool initMML()
        {
            PMDCompiler.Init();

            MmlDatum[] ret;
            CompilerInfo info = null;
            try
            {
                PMDCompiler.SetCompileSwitch(string.Format(
                    "PmdOption={0} \"{1}\""
                    , setting.pmdDotNET.compilerArguments
                    , PlayingFileName));
                using (MemoryStream sourceMML = new MemoryStream(vgmBuf))
                    ret = PMDCompiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);

                info = PMDCompiler.GetCompilerInfo();

            }
            catch
            {
                ret = null;
                info = null;
            }

            if (ret == null || info == null) return false;
            if (info.errorList.Count > 0)
            {
                if (model == EnmModel.VirtualModel)
                {
                    System.Windows.Forms.MessageBox.Show("Compile error");
                }
                return false;
            }

            if (PMDDriver == null) PMDDriver = im.GetDriver("PMDDotNET.Driver.Driver");

            //bool notSoundBoard2 = false;
            bool isLoadADPCM = true;
            bool loadADPCMOnly = false;

            isNRM = setting.pmdDotNET.soundBoard == 0;
            isSPB = setting.pmdDotNET.soundBoard == 1;
            isVA = false;
            usePPS = setting.pmdDotNET.usePPSDRV;
            usePPZ = setting.pmdDotNET.usePPZ8;

            EnvironmentE env = new EnvironmentE();
            env.AddEnv("pmd");
            env.AddEnv("pmdopt");
            envPmd = env.GetEnvVal("pmd");
            envPmdOpt = env.GetEnvVal("pmdopt");

            object[] addtionalPMDDotNETOption = new object[]{
                isLoadADPCM,//bool
                loadADPCMOnly,//bool
                setting.pmdDotNET.isAuto,//bool isAUTO;
                isVA,//bool
                isNRM,//bool
                usePPS,//bool
                usePPZ,//bool
                isSPB,//bool
                envPmd,//string[] 環境変数PMD
                envPmdOpt,//string[] 環境変数PMDOpt
                PlayingFileName,//string srcFile;
                "",//string PPCFileHeader無視されます(設定不要)
                (Func<string,Stream>)appendFileReaderCallback
            };

            string[] addtionalPMDOption = GetPMDOption();

            //PMDDriver.Init(
            //    PlayingFileName
            //    , chipWriteRegister
            //    , chipWaitSend
            //    , ret
            //    , new object[] {
            //          addtionalPMDDotNETOption //PMDDotNET option 
            //        , addtionalPMDOption // PMD option
            //        , (Func<ChipDatum, int>)PPZ8Write
            //        , (Func<ChipDatum, int>)PPSDRVWrite
            //        , (Func<ChipDatum, int>)P86Write
            //    });

            List<ChipAction> lca = new List<ChipAction>();
            PMDChipAction ca = new PMDChipAction(OPNA1Write, OPNAWaitSend);
            lca.Add(ca);

            PMDDriver.Init(
                lca
                //fileName
                //, oPNAWrite
                //, oPNAWaitSend
                , ret
                , null//ここのコールバックは未使用
                , new object[] {
                      addtionalPMDDotNETOption //PMDDotNET option 
                    , addtionalPMDOption // PMD option
                    , (Func<ChipDatum, int>)PPZ8Write
                    , (Func<ChipDatum, int>)PPSDRVWrite
                    , (Func<ChipDatum, int>)P86Write
                });


            PMDDriver.StartRendering(Common.VGMProcSampleRate
                , new Tuple<string, int>[] { new Tuple<string, int>("YM2608", baseclock) });
            PMDDriver.MusicSTART(0);
            return true;
        }

        private void OPNA1Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;
            if (cd.port == -1) return;

            chipRegister.setYM2608Register(0, cd.port, cd.address, cd.data, model);
        }

        private void OPNAWaitSend(long size, int elapsed)
        {
            if (model == EnmModel.VirtualModel)
            {
                //MessageBox.Show(string.Format("elapsed:{0} size:{1}", elapsed, size));
                //int n = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
                //Thread.Sleep(n);
                return;
            }

            //サイズと経過時間から、追加でウエイトする。
            int m = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
            Thread.Sleep(m);
        }

        public class PMDChipAction : ChipAction
        {
            private Action<ChipDatum> oPNAWrite;
            private Action<long, int> oPNAWaitSend;

            public PMDChipAction(Action<ChipDatum> oPNAWrite, Action<long, int> oPNAWaitSend)
            {
                this.oPNAWrite = oPNAWrite;
                this.oPNAWaitSend = oPNAWaitSend;
            }

            public override string GetChipName()
            {
                throw new NotImplementedException();
            }

            public override void WaitSend(long t1, int t2)
            {
                oPNAWaitSend(t1, t2);
            }

            public override void WritePCMData(byte[] data, int startAddress, int endAddress)
            {
                throw new NotImplementedException();
            }

            public override void WriteRegister(ChipDatum cd)
            {
                oPNAWrite(cd);
            }
        }
        private bool initM()
        {
            if (PMDDriver == null) PMDDriver = im.GetDriver("PMDDotNET.Driver.Driver");

            //bool notSoundBoard2 = false;
            bool isLoadADPCM = true;
            bool loadADPCMOnly = false;
            List<MmlDatum> buf = new List<MmlDatum>();
            foreach (byte b in vgmBuf) buf.Add(new MmlDatum(b));

            isNRM = setting.pmdDotNET.soundBoard == 0;
            isSPB = setting.pmdDotNET.soundBoard == 1;
            isVA = false;
            usePPS = setting.pmdDotNET.usePPSDRV;
            usePPZ = setting.pmdDotNET.usePPZ8;

            EnvironmentE env = new EnvironmentE();
            env.AddEnv("pmd");
            env.AddEnv("pmdopt");
            envPmd = env.GetEnvVal("pmd");
            envPmdOpt = env.GetEnvVal("pmdopt");

            object[] addtionalPMDDotNETOption = new object[]{
                isLoadADPCM,//bool
                loadADPCMOnly,//bool
                setting.pmdDotNET.isAuto,//bool isAUTO;
                isVA,//bool
                isNRM,//bool
                usePPS,//bool
                usePPZ,//bool
                isSPB,//bool
                envPmd,//string[] 環境変数PMD
                envPmdOpt,//string[] 環境変数PMDOpt
                PlayingFileName,//string srcFile;
                "",//string PPCFileHeader無視されます(設定不要)
                (Func<string,Stream>)appendFileReaderCallback
            };

            string[] addtionalPMDOption = GetPMDOption();

            //PMDDriver.Init(
            //    PlayingFileName
            //    , chipWriteRegister
            //    , chipWaitSend
            //    , buf.ToArray()
            //    , new object[] {
            //          addtionalPMDDotNETOption //PMDDotNET option 
            //        , addtionalPMDOption // PMD option
            //        , (Func<ChipDatum, int>)PPZ8Write
            //        , (Func<ChipDatum, int>)PPSDRVWrite
            //        , (Func<ChipDatum, int>)P86Write
            //    });
            List<ChipAction> lca = new List<ChipAction>();
            PMDChipAction ca = new PMDChipAction(OPNA1Write, OPNAWaitSend);
            lca.Add(ca);

            PMDDriver.Init(
                lca
                //fileName
                //, oPNAWrite
                //, oPNAWaitSend
                , buf.ToArray()
                , null//ここのコールバックは未使用
                , new object[] {
                      addtionalPMDDotNETOption //PMDDotNET option 
                    , addtionalPMDOption // PMD option
                    , (Func<ChipDatum, int>)PPZ8Write
                    , (Func<ChipDatum, int>)PPSDRVWrite
                    , (Func<ChipDatum, int>)P86Write
                });

            PMDDriver.StartRendering(Common.VGMProcSampleRate
                , new Tuple<string, int>[] { new Tuple<string, int>("YM2608", baseclock) });
            PMDDriver.MusicSTART(0);

            return true;
        }

        private void chipWaitSend(long elapsed, int size)
        {
            if (model == EnmModel.VirtualModel)
            {
                //MessageBox.Show(string.Format("elapsed:{0} size:{1}", elapsed, size));
                //int n = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
                //Thread.Sleep(n);
                return;
            }

            //サイズと経過時間から、追加でウエイトする。
            int m = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
            Thread.Sleep(m);
        }

        private void chipWriteRegister(ChipDatum dat)
        {
            if (dat == null) return;
            if (dat.address == -1) return;
            if (dat.data == -1) return;
            if (dat.port == -1) return;

            chipRegister.setYM2608Register(0, dat.port, dat.address, dat.data, model);
            //Console.WriteLine("{0} {1}", dat.address, dat.data);
        }

        private int PPSDRVWrite(ChipDatum arg)
        {
            if (arg == null) return 0;

            if (arg.port == 0x05)
            {
                chipRegister.PPSDRVLoad(0, (byte[])arg.addtionalData,model);
            }
            else
            {
                chipRegister.PPSDRVWrite(0, arg.port, arg.address, arg.data,model);
            }

            return 0;
        }

        private int P86Write(ChipDatum arg)
        {
            if (arg == null) return 0;

            if (arg.port == 0x00)
            {
                chipRegister.P86LoadPcm(0, (byte)arg.address, (byte)arg.data, (byte[])arg.addtionalData, model);
            }
            else
            {
                chipRegister.P86Write(0, arg.port, arg.address, arg.data, model);
            }

            return 0;
        }


        private int PPZ8Write(ChipDatum arg)
        {
            if (arg == null) return 0;

            if (arg.port == 0x03)
            {
                chipRegister.PPZ8LoadPcm(0, (byte)arg.address, (byte)arg.data, (byte[][])arg.addtionalData, model);
            }
            else
            {
                chipRegister.PPZ8Write(0, arg.port, arg.address, arg.data, model);
            }

            return 0;
        }


        private Stream appendFileReaderCallback(string arg)
        {
            string fn;
            fn = arg;
            string dir=Path.GetDirectoryName(arg);
            if (string.IsNullOrEmpty(dir)) fn = Path.Combine(Path.GetDirectoryName(PlayingFileName), fn);

            if (envPmd != null)
            {
                int i = 0;
                while (!File.Exists(fn) && i < envPmd.Length)
                {
                    fn = Path.Combine(
                        envPmd[i++]
                        , Path.GetFileName(arg)
                        );
                }
            }

            FileStream strm;
            try
            {
                strm = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (IOException)
            {
                strm = null;
            }

            return strm;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public class EnvironmentE
        {
            private List<string> envs = null;

            public EnvironmentE()
            {
                envs = new List<string>();
            }

            public void AddEnv(string envname)
            {
                var env = System.Environment.GetEnvironmentVariable(envname, EnvironmentVariableTarget.Process);
                if (!string.IsNullOrEmpty(env))
                {
                    envs.Add(string.Format("{0}={1}", envname, env));
                }
            }

            public string[] GetEnv()
            {
                return envs.ToArray();
            }

            public string[] GetEnvVal(string envname)
            {
                if (envs == null) return null;

                foreach (string item in envs)
                {
                    string[] kv = item.Split('=');
                    if (kv == null) continue;
                    if (kv.Length != 2) continue;
                    if (kv[0].ToUpper() != envname.ToUpper()) continue;

                    string[] vals = kv[1].Split(';');
                    return vals;
                }

                return null;
            }

        }

        private string[] GetPMDOption()
        {
            List<string> op = new List<string>();

            //envPMDOpt
            if (envPmdOpt != null && envPmdOpt.Length > 0) foreach (string opt in envPmdOpt) op.Add(opt);

            //引数(IDEではオプション設定)
            string[] drvArgs = setting.pmdDotNET.driverArguments.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (drvArgs != null && drvArgs.Length > 0) foreach (string drvArg in drvArgs) op.Add(drvArg);

            return op.ToArray();
        }

    }
}
