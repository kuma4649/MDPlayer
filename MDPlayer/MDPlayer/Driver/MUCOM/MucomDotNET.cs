using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MDPlayer.Driver
{
    public class MucomDotNET : baseDriver
    {
        private InstanceMarker im = null;
        private iCompiler mucomCompiler = null;
        private iDriver mucomDriver = null;

        public string PlayingFileName { get; internal set; }
        public const int OPNAbaseclock = 7987200;
        public const int OPNBbaseclock = 8000000;
        public const int OPMbaseclock = 3579545;
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

        public EnmChip[] useChipsFromMub(byte[] buf)
        {
            List<EnmChip> ret = new List<EnmChip>();
            ret.Add(EnmChip.YM2608);
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);

            //標準的なmubファイル
            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x43
                && buf[3] == 0x38)
            {
                return ret.ToArray();
            }
            //標準的なmubファイル
            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x42
                && buf[3] == 0x38)
            {
                return ret.ToArray();
            }
            //拡張mubファイル？
            if (buf[0] != 'm'
                || buf[1] != 'u'
                || buf[2] != 'P'
                || buf[3] != 'b')
            {
                //見知らぬファイル
                return null;
            }

            uint chipsCount = buf[0x0009];
            int ptr = 0x0022;
            uint[] partCount = new uint[chipsCount];
            uint[][] pageCount = new uint[chipsCount][];
            uint[][][] pageLength = new uint[chipsCount][][];
            for (int i = 0; i < chipsCount; i++)
            {
                partCount[i] = buf[ptr + 0x16];
                int instCount = buf[ptr + 0x17];
                ptr += 2 * instCount + 0x18;
                int pcmCount = buf[ptr];
                ptr += 2 * pcmCount + 1;
            }

            for (int i = 0; i < chipsCount; i++)
            {
                pageCount[i] = new uint[partCount[i]];
                pageLength[i] = new uint[partCount[i]][];
                for (int j = 0; j < partCount[i]; j++)
                {
                    pageCount[i][j] = buf[ptr++];
                }
            }

            for (int i = 0; i < chipsCount; i++)
            {
                for (int j = 0; j < partCount[i]; j++)
                {
                    pageLength[i][j] = new uint[pageCount[i][j]];
                    for (int k = 0; k < pageCount[i][j]; k++)
                    {
                        pageLength[i][j][k] = Common.getLE32(buf, (uint)ptr);
                        ptr += 8;
                    }
                }
            }

            ret.Clear();
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);
            ret.Add(EnmChip.Unuse);

            if (chipsCount > 0)
            {
                if (partCount[0] > 0)
                {
                    uint n = 0;
                    for (int i = 0; i < partCount[0]; i++)
                    {
                        n += pageCount[0][i];
                    }
                    if (n > 0) ret[0] = EnmChip.YM2608;
                }
            }

            if (chipsCount > 1)
            {
                if (partCount[1] > 0)
                {
                    uint n = 0;
                    for (int i = 0; i < partCount[1]; i++)
                    {
                        n += pageCount[1][i];
                    }
                    if (n > 0) ret[1] = EnmChip.S_YM2608;
                }
            }

            if (chipsCount > 2)
            {
                if (partCount[2] > 0)
                {
                    uint n = 0;
                    for (int i = 0; i < partCount[2]; i++)
                    {
                        n += pageCount[2][i];
                    }
                    if (n > 0) ret[2] = EnmChip.YM2610;
                }
            }

            if (chipsCount > 3)
            {
                if (partCount[3] > 0)
                {
                    uint n = 0;
                    for (int i = 0; i < partCount[3]; i++)
                    {
                        n += pageCount[3][i];
                    }
                    if (n > 0) ret[3] = EnmChip.S_YM2610;
                }
            }

            if (chipsCount > 4)
            {
                if (partCount[4] > 0)
                {
                    uint n = 0;
                    for (int i = 0; i < partCount[4]; i++)
                    {
                        n += pageCount[4][i];
                    }
                    if (n > 0) ret[4] = EnmChip.YM2151;
                }
            }

            return ret.ToArray();
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
            //if (model == EnmModel.RealModel) return true;
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
                //Stopped = true;
                //return;
            }
#endif

            try
            {
                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
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
        
        public byte[] Compile(byte[] vgmBuf)
        {
            if(mucomCompiler==null) mucomCompiler = im.GetCompiler("mucomDotNET.Compiler.Compiler");
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
            foreach(MmlDatum md in ret)
            {
                dest.Add(md != null ? (byte)md.dat : (byte)0);
            }

            return dest.ToArray();
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
            if (buf[0] == 'm'
                && buf[1] == 'u'
                && buf[2] == 'P'
                && buf[3] == 'b')
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

            //mucomDriver.Init(PlayingFileName,chipWriteRegister,chipWaitSend, ret, new object[] {
            //          notSoundBoard2
            //        , isLoadADPCM
            //        , loadADPCMOnly
            //    });
            List<ChipAction> lca = new List<ChipAction>();
            mucomChipAction ca;
            ca = new mucomChipAction(OPNA1Write, null, OPNAWaitSend); lca.Add(ca);
            ca = new mucomChipAction(OPNA2Write, null, null); lca.Add(ca);
            ca = new mucomChipAction(OPNB1Write, WriteOPNB1PCMData, null); lca.Add(ca);
            ca = new mucomChipAction(OPNB2Write, WriteOPNB2PCMData, null); lca.Add(ca);
            ca = new mucomChipAction(OPM1Write, null, null); lca.Add(ca);
            mucomDriver.Init(
                lca,
                ret
                , null
                , new object[] {
                      notSoundBoard2
                    , isLoadADPCM
                    , loadADPCMOnly
                    , PlayingFileName
                });

            mucomDriver.StartRendering(Common.VGMProcSampleRate
                ,new Tuple<string, int>[] { new Tuple<string, int>("", OPNAbaseclock) });
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
            //mucomDriver.Init(PlayingFileName, chipWriteRegister, chipWaitSend, buf.ToArray(), new object[] {
            //          notSoundBoard2
            //        , isLoadADPCM
            //        , loadADPCMOnly
            //    });

            List<ChipAction> lca = new List<ChipAction>();
            mucomChipAction ca;
            ca = new mucomChipAction(OPNA1Write, null, OPNAWaitSend); lca.Add(ca);
            ca = new mucomChipAction(OPNA2Write, null, null); lca.Add(ca);
            ca = new mucomChipAction(OPNB1Write, WriteOPNB1PCMData, null); lca.Add(ca);
            ca = new mucomChipAction(OPNB2Write, WriteOPNB2PCMData, null); lca.Add(ca);
            ca = new mucomChipAction(OPM1Write, null, null); lca.Add(ca);
            mucomDriver.Init(
                lca,
                buf.ToArray()
                ,null
                , new object[] {
                      notSoundBoard2
                    , isLoadADPCM
                    , loadADPCMOnly
                    , PlayingFileName
                });

            mucomDriver.StartRendering(Common.VGMProcSampleRate
                , new Tuple<string, int>[] { new Tuple<string, int>("", OPNAbaseclock) });
            mucomDriver.MusicSTART(0);

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
        private void OPNA2Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;
            if (cd.port == -1) return;

            chipRegister.setYM2608Register(1, cd.port, cd.address, cd.data, model);

        }

        private void OPNB1Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;
            if (cd.port == -1) return;

            chipRegister.setYM2610Register(0, cd.port, cd.address, cd.data, model);
        }
        private void OPNB2Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;
            if (cd.port == -1) return;

            chipRegister.setYM2610Register(1, cd.port, cd.address, cd.data, model);
        }
        private void OPM1Write(ChipDatum cd)
        {
            if (cd == null) return;
            if (cd.address == -1) return;
            if (cd.data == -1) return;

            chipRegister.setYM2151Register(0, cd.port, cd.address, cd.data, model, 0, 0);
        }

        private void WriteOPNB1PCMData(byte[] dat, int v, int v2)
        {
            if (v == 0)
                chipRegister.WriteYM2610_SetAdpcmA(0, dat, EnmModel.VirtualModel);
            else
                chipRegister.WriteYM2610_SetAdpcmB(0, dat, EnmModel.VirtualModel);
        }
        private void WriteOPNB2PCMData(byte[] dat, int v, int v2)
        {
            if (v == 0)
                chipRegister.WriteYM2610_SetAdpcmA(1, dat, EnmModel.VirtualModel);
            else
                chipRegister.WriteYM2610_SetAdpcmB(1, dat, EnmModel.VirtualModel);
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

        public class mucomChipAction : ChipAction
        {
            private Action<ChipDatum> _Write;
            private Action<byte[], int, int> _WritePCMData;
            private Action<long, int> _WaitSend;

            public mucomChipAction(Action<ChipDatum> Write, Action<byte[], int, int> WritePCMData, Action<long, int> WaitSend)
            {
                _Write = Write;
                _WritePCMData = WritePCMData;
                _WaitSend = WaitSend;
            }

            public override string GetChipName()
            {
                throw new NotImplementedException();
            }

            public override void WaitSend(long t1, int t2)
            {
                _WaitSend?.Invoke(t1, t2);
            }

            public override void WritePCMData(byte[] data, int startAddress, int endAddress)
            {
                _WritePCMData?.Invoke(data, startAddress, endAddress);
            }

            public override void WriteRegister(ChipDatum cd)
            {
                _Write?.Invoke(cd);
            }
        }

        //private void chipWaitSend(long elapsed, int size)
        //{
        //    if (model == EnmModel.VirtualModel)
        //    {
        //        //MessageBox.Show(string.Format("elapsed:{0} size:{1}", elapsed, size));
        //        //int n = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
        //        //Thread.Sleep(n);
        //        return;
        //    }

        //    //サイズと経過時間から、追加でウエイトする。
        //    int m = Math.Max((int)(size / 20 - elapsed), 0);//20 閾値(magic number)
        //    Thread.Sleep(m);
        //}

        //private void chipWriteRegister(ChipDatum dat)
        //{
        //    if (dat == null) return;
        //    if (dat.address == -1) return;
        //    if (dat.data == -1) return;
        //    if (dat.port == -1) return;

        //    chipRegister.setYM2608Register(0, dat.port, dat.address, dat.data, model);
        //    //Console.WriteLine("{0} {1}", dat.address, dat.data);
        //}

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

    }
}
