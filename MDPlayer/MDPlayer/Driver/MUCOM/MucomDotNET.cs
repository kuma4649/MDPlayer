using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer.Driver
{
    public class MucomDotNET : baseDriver
    {
        private InstanceMarker im = null;
        private iCompiler mucomCompiler = null;
        private iDriver mucomDriver = null;

        public string PlayingFileName { get; internal set; }
        public const int baseclock = 7987200;
        private enmMUCOMFileType mtype;

        public MucomDotNET(InstanceMarker mucomDotNET_Im)
        {
            im = mucomDotNET_Im;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            mtype = CheckFileType(buf);
            GD3Tag gt;

            if (mtype == enmMUCOMFileType.MUC)
            {
                mucomCompiler = im.GetCompiler("mucomDotNET.Compiler.Compiler");
                gt = mucomCompiler.GetGD3TagInfo(buf);
            }
            else
            {
                mucomDriver = im.GetDriver("mucomDotNET.Driver.Driver");
                gt = mucomDriver.GetGD3TagInfo(buf);
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

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            GD3 = getGD3Info(vgmBuf, 0);

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

            if (mtype == enmMUCOMFileType.MUC) return initMUC();
            else return initMUB();
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException("このdriverはこのメソッドを必要としない");
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
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    mucomDriver.Rendering();

                    Counter++;
                    vgmFrameCounter++;
                }

                int lp = mucomDriver.GetNowLoopCounter();
                lp = lp < 0 ? 0 : lp;
                vgmCurLoop = (uint)lp;

                if (mucomDriver.GetStatus() < 1)
                {
                    if (mucomDriver.GetStatus() == 0)
                    {
                        Thread.Sleep((int)(latency * 2.0));//実際の音声が発音しきるまでlatency*2の分だけ待つ
                    }
                    Stopped = true;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }



        public enum enmMUCOMFileType
        {
            unknown,
            MUB,
            MUC
        }
        


        private enmMUCOMFileType CheckFileType(byte[] buf)
        {
            if (buf == null || buf.Length < 4)
            {
                return enmMUCOMFileType.unknown;
            }

            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x43
                && buf[3] == 0x38)
            {
                return enmMUCOMFileType.MUB;
            }
            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x42
                && buf[3] == 0x38)
            {
                return enmMUCOMFileType.MUB;
            }

            return enmMUCOMFileType.MUC;
        }

        private bool initMUC()
        {
            mucomCompiler.Init();

            MmlDatum[] ret;
            CompilerInfo info = null;
            try
            {
                using (MemoryStream sourceMML = new MemoryStream(vgmBuf))
                    ret = mucomCompiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);

                info = mucomCompiler.GetCompilerInfo();

            }
            catch
            {
                ret = null;
                info = null;
            }

            if (ret == null || info == null) return false;
            if (info.errorList.Count > 0)
            {
                if(model== EnmModel.VirtualModel)
                {
                    System.Windows.Forms.MessageBox.Show("Compile error");
                }
                return false;
            }

            if (mucomDriver == null) mucomDriver = im.GetDriver("mucomDotNET.Driver.Driver");

            bool notSoundBoard2 = false;
            bool isLoadADPCM = true;
            bool loadADPCMOnly = false;

            mucomDriver.Init(PlayingFileName,chipWriteRegister,chipWaitSend, ret, new object[] {
                      notSoundBoard2
                    , isLoadADPCM
                    , loadADPCMOnly
                });
            mucomDriver.StartRendering(Common.SampleRate
                ,new Tuple<string, int>[] { new Tuple<string, int>("", baseclock) });
            mucomDriver.MusicSTART(0);

            return true;
        }

        private bool initMUB()
        {
            if (mucomDriver == null) mucomDriver = im.GetDriver("mucomDotNET.Driver.Driver");

            bool notSoundBoard2 = false;
            bool isLoadADPCM = true;
            bool loadADPCMOnly = false;
            List<MmlDatum> buf = new List<MmlDatum>();
            foreach(byte b in vgmBuf) buf.Add(new MmlDatum(b));
            mucomDriver.Init(PlayingFileName, chipWriteRegister, chipWaitSend, buf.ToArray(), new object[] {
                      notSoundBoard2
                    , isLoadADPCM
                    , loadADPCMOnly
                });
            mucomDriver.StartRendering(Common.SampleRate
                , new Tuple<string, int>[] { new Tuple<string, int>("", baseclock) });
            mucomDriver.MusicSTART(0);

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

        private Stream appendFileReaderCallback(string arg)
        {

            string fn = Path.Combine(
                Path.GetDirectoryName(PlayingFileName)
                , arg
                );

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

    }
}
