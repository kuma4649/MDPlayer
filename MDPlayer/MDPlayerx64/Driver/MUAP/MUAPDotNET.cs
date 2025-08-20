using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDPlayer.Driver.MucomDotNET;

namespace MDPlayer.Driver
{
    public class muapDotNET : baseDriver
    {
        private InstanceMarker im = null;
        private iCompiler muapCompiler = null;
        private iDriver muapDriver = null;
        public byte[] toneBuff;


        public string PlayingFileName { get; internal set; }


        public muapDotNET(InstanceMarker muapDotNET_Im)
        {
            im = muapDotNET_Im;
            Log.writeLine = writeLine;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            //muapには基本的にはタグ情報はない
            //(歌詞機能を使ってタイトル表記しているデータはある。)
            GD3 gt = new GD3();
            return gt;
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

            return initO();
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
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

                    muapDriver.Rendering();

                    Counter++;
                    vgmFrameCounter++;
                }

                //int lp = muapDriver.GetNowLoopCounter();
                //lp = lp < 0 ? 0 : lp;
                //vgmCurLoop = (uint)lp;

                //if (muapDriver.GetStatus() < 1)
                //{
                //    //if (mucomDriver.GetStatus() == 0 && !Stopped)
                //    //{
                //    //    Thread.Sleep((int)(setting.outputDevice.SampleRate/latency * 2.0));//実際の音声が発音しきるまでlatency*2の分だけ待つ
                //    //}
                //    Stopped = true;
                //}
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public byte[] Compile(byte[] vgmBuf)
        {
            if (muapCompiler == null) muapCompiler = im.GetCompiler("muapDotNET.Compiler.Compiler");
            muapCompiler.Init();

            MmlDatum[] ret;
            CompilerInfo info = null;
            try
            {
                using (MemoryStream sourceMML = new MemoryStream(vgmBuf))
                    ret = muapCompiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);

                info = muapCompiler.GetCompilerInfo();

            }
            catch
            {
                ret = null;
                info = null;
            }

            if (ret == null && info == null) return null;
            if (info != null && info.errorList.Count > 0)
            {
                foreach (var error in info.errorList)
                {
                    log.Write(LogLevel.Error, error.Item3);
                }
                if (model == EnmModel.VirtualModel)
                {
                    System.Windows.Forms.MessageBox.Show("Compile error");
                }
                return null;
            }
            if (ret == null)
            {
                return null;
            }

            List<byte> dest = new List<byte>();
            foreach (MmlDatum md in ret)
            {
                dest.Add(md != null ? (byte)md.dat : (byte)0);
            }

            if(ret!=null && ret.Length>0 && ret[0].args!=null && ret[0].args[0]!=null && ret[0].args[0] is byte[])
            {
                toneBuff = (byte[])ret[0].args[0];
            }

            return dest.ToArray();
        }

        private void writeLine(musicDriverInterface.LogLevel level, string arg2)
        {
            if (level > musicDriverInterface.LogLevel.INFO) return;

            LogLevel lvl = LogLevel.Information;
            switch (level)
            {
                case musicDriverInterface.LogLevel.TRACE:
                    lvl = LogLevel.Trace;
                    break;
                case musicDriverInterface.LogLevel.INFO:
                    lvl = LogLevel.Information;
                    break;
                case musicDriverInterface.LogLevel.WARNING:
                    lvl = LogLevel.Warning;
                    break;
                case musicDriverInterface.LogLevel.ERROR:
                    lvl = LogLevel.Error;
                    break;
                case musicDriverInterface.LogLevel.FATAL:
                    lvl = LogLevel.Enforcement;
                    break;
                case musicDriverInterface.LogLevel.DEBUG:
                    lvl = LogLevel.Debug;
                    break;
            }
            log.Write(lvl, arg2);
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

        private bool initO()
        {
            if (muapDriver == null) muapDriver = im.GetDriver("muapDotNET.Driver.Driver");

            List<MmlDatum> buf = new List<MmlDatum>();
            foreach (byte b in vgmBuf) buf.Add(new MmlDatum(b));

            List<ChipAction> lca = new List<ChipAction>();
            mucomChipAction ca;
            ca = new mucomChipAction(OPNAWriteP, null, null); lca.Add(ca);
            ca = new mucomChipAction(OPN2WriteP, null, null); lca.Add(ca);
            ca = new mucomChipAction(CS4231Write, null, null); lca.Add(ca);
            muapDriver.Init(
                lca,
                buf.ToArray()
                , null
                , (object)(new object[] { CS4231Read ,toneBuff})
                );

            muapDriver.StartRendering(Common.VGMProcSampleRate
                , [
                    new Tuple<string, int>("YM2608",(int) Driver.MucomDotNET.OPNAbaseclock)
                ]
            );
            muapDriver.MusicSTART(0);
            object[] work = (object[])muapDriver.GetWork();
            chipRegister.setCS4231FIFOBuf(0, (byte[])work[0], model);
            chipRegister.setCS4231Int0bEnt(0, (Action)work[1], model);

            return true;
        }

        void OPNAWriteP(ChipDatum dat)
        {
            //log.Write(LogLevel.Trace, string.Format("Write OPNA : Prt:${0:X02} Adr:${1:X02} Dat:${2:X02}", dat.port, dat.address, dat.data));
            OPNAWrite(0, dat);
        }
        void OPN2WriteP(ChipDatum dat)
        {
            //log.Write(LogLevel.Trace, string.Format("Write OPN2 : Prt:${0:X02} Adr:${1:X02} Dat:${2:X02}", dat.port, dat.address, dat.data));
            OPN2Write(0, dat);
        }

        void OPNAWrite(int chipId, ChipDatum dat)
        {
            if (dat != null && dat.addtionalData != null)
            {
                MmlDatum md = (MmlDatum)dat.addtionalData;
                if (md.linePos != null)
                {
                    //Log.WriteLine(LogLevel.TRACE, string.Format("! OPNA i{0} r{1} c{2}"
                    //, chipId
                    //, md.linePos.row
                    //, md.linePos.col
                    //));
                }
            }

            if (dat.address == -1) return;
            //Log.WriteLine(LogLevel.TRACE, string.Format("Out ChipA:{0} Port:{1} Adr:[{2:x02}] val[{3:x02}]", chipId, dat.port, (int)dat.address, (int)dat.data));

            chipRegister.setYM2608Register((byte)chipId, (byte)dat.port, (byte)dat.address, (byte)dat.data, model, vgmFrameCounter);
        }

        void OPN2Write(int chipId, ChipDatum dat)
        {
            if (dat != null && dat.addtionalData != null)
            {
                MmlDatum md = (MmlDatum)dat.addtionalData;
                if (md.linePos != null)
                {
                    //Log.WriteLine(LogLevel.TRACE, string.Format("! OPNA i{0} r{1} c{2}"
                    //, chipId
                    //, md.linePos.row
                    //, md.linePos.col
                    //));
                }
            }

            if (dat.address == -1) return;
            //Log.WriteLine(LogLevel.TRACE, string.Format("Out ChipA:{0} Port:{1} Adr:[{2:x02}] val[{3:x02}]", chipId, dat.port, (int)dat.address, (int)dat.data));

            chipRegister.setYM2612Register((byte)chipId, (byte)dat.port, (byte)dat.address, (byte)dat.data, model, vgmFrameCounter);
        }

        void CS4231Write(ChipDatum dat)
        {
            if (dat != null && dat.addtionalData != null)
            {
                MmlDatum md = (MmlDatum)dat.addtionalData;
                if (md.linePos != null)
                {
                    //Log.WriteLine(LogLevel.TRACE, string.Format("! OPNA i{0} r{1} c{2}"
                    //, chipId
                    //, md.linePos.row
                    //, md.linePos.col
                    //));
                }
            }

            if (dat.address == -1) return;
            //Log.WriteLine(LogLevel.TRACE, string.Format("Out ChipA:{0} Port:{1} Adr:[{2:x02}] val[{3:x02}]", chipId, dat.port, (int)dat.address, (int)dat.data));

            chipRegister.setCS4231Register((byte)0, (byte)dat.port, (byte)dat.address, (byte)dat.data, model, vgmFrameCounter);
        }

        byte CS4231Read(byte adr)
        {
            return chipRegister.getCS4231Register((byte)0, (byte)adr, model, vgmFrameCounter);
        }

    }
}
