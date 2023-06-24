using musicDriverInterface;

namespace MDPlayer.Driver
{
    public class MoonDriverDotNET : baseDriver
    {
        private InstanceMarker im = null;
        private iCompiler moonDriverCompiler = null;
        private iDriver moonDriverDriver = null;
        private enmMoonDriverFileType mtype;


        public string PlayingFileName { get; internal set; }


        public MoonDriverDotNET(InstanceMarker moonDriverDotNET_Im)
        {
            im = moonDriverDotNET_Im;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            mtype = CheckFileType(buf);
            GD3Tag gt;

            if (mtype == enmMoonDriverFileType.MDL)
            {
                moonDriverCompiler = im.GetCompiler("MoonDriverDotNET.Compiler.Compiler");
                gt = moonDriverCompiler.GetGD3TagInfo(buf);
            }
            else
            {
                moonDriverDriver = im.GetDriver("MoonDriverDotNET.Driver.Driver");
                gt = moonDriverDriver.GetGD3TagInfo(buf);
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

            if (mtype == enmMoonDriverFileType.MDL) return initMDL();
            else return initMDR();
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
            if (Stopped) return;

            try
            {
                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    moonDriverDriver.Rendering();

                    Counter++;
                    vgmFrameCounter++;
                }

                int lp = moonDriverDriver.GetNowLoopCounter();
                lp = lp < 0 ? 0 : lp;
                vgmCurLoop = (uint)lp;

                if (moonDriverDriver.GetStatus() < 1)
                {
                    //if (moonDriverDriver.GetStatus() == 0)
                    //{
                    //    Thread.Sleep((int)(latency * 2.0));//実際の音声が発音しきるまでlatency*2の分だけ待つ
                    //}
                    Stopped = true;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public byte[] Compile(byte[] vgmBuf)
        {
            if (moonDriverCompiler == null) moonDriverCompiler = im.GetCompiler("MoonDriverDotNET.Compiler.Compiler");
            moonDriverCompiler.Init();
            moonDriverCompiler.SetCompileSwitch("SRC");
            moonDriverCompiler.SetCompileSwitch("MoonDriverOption=-i");
            moonDriverCompiler.SetCompileSwitch(string.Format("MoonDriverOption={0}", PlayingFileName));

            MmlDatum[] ret;
            CompilerInfo info = null;
            try
            {
                using (MemoryStream sourceMML = new MemoryStream(vgmBuf))
                    ret = moonDriverCompiler.Compile(sourceMML, appendFileReaderCallback);

                info = moonDriverCompiler.GetCompilerInfo();

            }
            catch
            {
                ret = null;
                info = null;
            }

            if (ret == null || info == null) return null;
            if (info.errorList.Count > 0)
            {
                if (model == EnmModel.VirtualModel)
                {
                    System.Windows.Forms.MessageBox.Show("Compile error");
                }
                return null;
            }

            List<byte> dest = new List<byte>();
            foreach (MmlDatum md in ret)
            {
                dest.Add(md != null ? (byte)md.dat : (byte)0);
            }

            return dest.ToArray();
        }



        public enum enmMoonDriverFileType
        {
            unknown,
            MDR,
            MDL
        }

        private enmMoonDriverFileType CheckFileType(byte[] buf)
        {
            if (buf == null || buf.Length < 4)
            {
                return enmMoonDriverFileType.unknown;
            }

            if (buf[0] == 'M'
                && buf[1] == 'D'
                && buf[2] == 'R'
                && buf[3] == 'V')
            {
                return enmMoonDriverFileType.MDR;
            }

            return enmMoonDriverFileType.MDL;
        }

        private bool initMDL()
        {
            moonDriverCompiler.Init();

            MmlDatum[] ret;
            CompilerInfo info = null;
            try
            {
                using (MemoryStream sourceMML = new MemoryStream(vgmBuf))
                    ret = moonDriverCompiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);

                info = moonDriverCompiler.GetCompilerInfo();

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

            if (moonDriverDriver == null) moonDriverDriver = im.GetDriver("MoonDriverDotNET.Driver.Driver");

            //bool notSoundBoard2 = false;
            //bool isLoadADPCM = true;
            //bool loadADPCMOnly = false;

            ////mucomDriver.Init(PlayingFileName,chipWriteRegister,chipWaitSend, ret, new object[] {
            ////          notSoundBoard2
            ////        , isLoadADPCM
            ////        , loadADPCMOnly
            ////    });
            //List<ChipAction> lca = new List<ChipAction>();
            //mucomChipAction ca;
            //ca = new mucomChipAction(OPNA1Write, null, OPNAWaitSend); lca.Add(ca);
            //ca = new mucomChipAction(OPNA2Write, null, null); lca.Add(ca);
            //ca = new mucomChipAction(OPNB1Write, WriteOPNB1PCMData, null); lca.Add(ca);
            //ca = new mucomChipAction(OPNB2Write, WriteOPNB2PCMData, null); lca.Add(ca);
            //ca = new mucomChipAction(OPM1Write, null, null); lca.Add(ca);
            //moonDriverDriver.Init(
            //    lca,
            //    ret
            //    , null
            //    , new object[] {
            //          notSoundBoard2
            //        , isLoadADPCM
            //        , loadADPCMOnly
            //        , PlayingFileName
            //    });

            //moonDriverDriver.StartRendering(Common.VGMProcSampleRate
            //    , new Tuple<string, int>[] { new Tuple<string, int>("", OPNAbaseclock) });
            //moonDriverDriver.MusicSTART(0);

            return true;
        }

        private bool initMDR()
        {
            if (moonDriverDriver == null) moonDriverDriver = im.GetDriver("MoonDriverDotNET.Driver.Driver");

            List<MmlDatum> buf = new List<MmlDatum>();
            foreach (byte b in vgmBuf) buf.Add(new MmlDatum(b));

            List<ChipAction> lca = new List<ChipAction>();
            ChipAction ca;
            if (useChip[0] == EnmChip.YMF278B)
            {
                ca = new MoonDriverChipAction(opl4Write, opl4WaitSend); lca.Add(ca);
            }
            else
            {
                ca = new MoonDriverChipAction(opl3Write, opl3WaitSend); lca.Add(ca);
            }

            object additionalOption = new object[]{
                    PlayingFileName,
                    (double)44100,
                    (int)0
                };
            moonDriverDriver.Init(
                lca,
                buf.ToArray()
                , appendFileReaderCallback
                , additionalOption
                );

            moonDriverDriver.StartRendering(Common.VGMProcSampleRate
                , new Tuple<string, int>[] { new Tuple<string, int>("YMF278B", (int)33868800) });
            moonDriverDriver.MusicSTART(0);

            return true;
        }

        private Stream appendFileReaderCallback(string arg)
        {

            string fn = Path.Combine(
                Path.GetDirectoryName(PlayingFileName)
                , arg
                );

            if (!File.Exists(fn)) return null;

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

        //public void WriteRegister(ChipDatum dat)
        //{
        //    //Log.WriteLine(LogLevel.TRACE, string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port));
        //    //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port);
        //    outDatum od = null;

        //    if (pcmdata.Count > 0)
        //    {
        //        chipRegister.YMF278BSetRegister(od, count, 0, pcmdata.ToArray());
        //        pcmdata.Clear();
        //    }

        //    if (dat.addtionalData != null)
        //    {
        //        if (dat.addtionalData is MmlDatum)
        //        {
        //            MmlDatum md = (MmlDatum)dat.addtionalData;
        //            if (md.linePos != null) md.linePos.srcMMLID = filename;
        //            od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
        //        }

        //    }

        //    //if (od != null && od.linePos != null)
        //    //{
        //    //Console.WriteLine("{0}", od.linePos.col);
        //    //}

        //    //chipRegister.YM2608SetRegister(od, (long)dat.time, 0, dat.port, dat.address, dat.data);
        //    chipRegister.YMF278BSetRegister(od, count, 0, dat.port, dat.address, dat.data);
        //}

        private void opl4Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;
            if (cd.port == -1) return;

            chipRegister.setYMF278BRegister(0, cd.port, cd.address, cd.data, model);
        }

        private void opl4WaitSend(long size, int elapsed)
        {
            if (model == EnmModel.VirtualModel)
            {
                //MessageBox.Show(string.Format("elapsed:{0} size:{1}", elapsed, size));
                //int n = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
                //Thread.Sleep(n);
                return;
            }

            ////サイズと経過時間から、追加でウエイトする。
            //int m = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
            //Thread.Sleep(m);
        }

        private void opl3Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;
            if (cd.port == -1) return;
            if (cd.port > 1) return;

            chipRegister.setYMF262Register(0, cd.port, cd.address, cd.data, model);
        }

        private void opl3WaitSend(long size, int elapsed)
        {
            if (model == EnmModel.VirtualModel)
            {
                //MessageBox.Show(string.Format("elapsed:{0} size:{1}", elapsed, size));
                //int n = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
                //Thread.Sleep(n);
                return;
            }

            ////サイズと経過時間から、追加でウエイトする。
            //int m = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
            //Thread.Sleep(m);
        }

        public class MoonDriverChipAction : ChipAction
        {
            private Action<ChipDatum> opl4Write;
            private Action<long, int> opl4WaitSend;

            public MoonDriverChipAction(Action<ChipDatum> opl4Write, Action<long, int> opl4WaitSend)
            {
                this.opl4Write = opl4Write;
                this.opl4WaitSend = opl4WaitSend;
            }

            public override string GetChipName()
            {
                throw new NotImplementedException();
            }

            public override void WaitSend(long t1, int t2)
            {
                opl4WaitSend(t1, t2);
            }

            public override void WritePCMData(byte[] data, int startAddress, int endAddress)
            {
                throw new NotImplementedException();
            }

            public override void WriteRegister(ChipDatum cd)
            {
                opl4Write(cd);
            }
        }

    }
}
