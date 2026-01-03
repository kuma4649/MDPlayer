using MDPlayer.Driver.MNDRV;
using MDPlayer.Driver.ZMS.nise68;
using MDSound;
using System.Diagnostics;
using static MDPlayer.Driver.MXDRV.MXDRV;

namespace MDPlayer.Driver.ZMS
{
    public class ZMS(EnmFileFormat format) : baseDriver
    {
        private readonly EnmFileFormat format = format;
        private nise68.nise68 nise68;
        private nise68.FileMng fileMng;
        public mpcmX68k mpcm;
        public mpcmpp mpcmpp;
        public int mpcmtype = 0;
        public ym2151_x68sound opmPCM;
        public PCM8PP pcm8pp;
        public int pcm8type = 0;

        private int checkCounter = 0;
        private List<string> envZPDs = new List<string>();
        public int version = 0;
        private FMTimer timerOPM;
        public Pcm8St[] pcm8St = new Pcm8St[8] { new(), new(), new(), new(), new(), new(), new(), new() };
        public MPCMSt[] mpcmSt = new MPCMSt[16] { new(), new(), new(), new(), new(), new(), new(), new(), new(), new(), new(), new(), new(), new(), new(), new() };

        public class MPCMSt
        {
            public bool Keyon = false;
            public bool Keyoff = false;
            public byte type = 0;
            public byte orig = 0;
            public int adrs_ptr = 0;
            public uint size = 0;
            public uint start = 0;
            public uint end = 0;
            public uint count = 0;
            public int frq = 0;
            public int pitch = 0;
            public int volume = 0;
            public int pan = 0;
            public float rate = 0;
            public float base_ = 0;
        }

        public string PlayingFileName { get; internal set; }
        public string PlayingArcFileName { get; internal set; }
        public List<Tuple<byte[], string>> SupportFileBinaryAndName;
        public byte[] CompiledData { get; set; }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if(format== EnmFileFormat.ZMS)
            {
                return GetGD3InfoZMS(buf);
            }
            if (format == EnmFileFormat.ZMD)
            {
                return GetGD3InfoZMD(buf);
            }
            return new GD3();
        }

        private GD3 GetGD3InfoZMS(byte[] buf)
        {
            string text = System.Text.Encoding.GetEncoding("shift_jis").GetString(buf);
            string[] texts = text.Split("\r\n");
            string cmt = "";
            string comment = ".COMMENT";
            foreach(string s in texts)
            {
                if (s.ToUpper().Trim().IndexOf(comment) < 0) continue;
                cmt = s.Trim().Substring(s.ToUpper().Trim().IndexOf(comment) + comment.Length).Trim();
                break;
            }
            GD3 gd3=new GD3();
            if(!string.IsNullOrEmpty(cmt))
            {
                gd3.TrackName = cmt;
                gd3.TrackNameJ = cmt;
            }
            return gd3;
        }

        private GD3 GetGD3InfoZMD(byte[] buf)
        {
            GD3 gd3 = new GD3();

            if (buf.Length < 8)
            {
                throw new Exception("Unknown zmd file");
            }
            else
            {
                int chkID1 = buf[0] * 0x100_0000 + buf[1] * 0x1_0000 + buf[2] * 0x100 + buf[3] * 0x1;
                int chkID2 = buf[4] * 0x100_0000 + buf[5] * 0x1_0000 + buf[6] * 0x100 + buf[7] * 0x1;
                if (chkID1 == 0x1a5a_6d75 && chkID2 == 0x5369_4330) version = 3;
                if (chkID1 == 0x105a_6d75 && chkID2 != 0x5369_4330) version = 2;

                if (version == 0)
                {
                    throw new Exception("Version check error");
                }
            }

            string cmt = "";
            try
            {
                if (version == 3)
                {
                    int ptr = buf[9 * 4 + 0] * 0x100_0000 + buf[9 * 4 + 1] * 0x1_0000
                        + buf[9 * 4 + 2] * 0x100 + buf[9 * 4 + 3] * 0x1 + 40;
                    int ePtr = ptr;
                    while (buf[ePtr] != 0x00)
                    {
                        if (buf[ePtr] == 0x0d && buf[ePtr + 1] == 0x0a) break;
                        ePtr++;
                    }

                    cmt = System.Text.Encoding.GetEncoding("shift_jis").GetString(buf, ptr, ePtr - ptr);
                }
            }
            catch
            {
                ;//何もしない
            }

            if (!string.IsNullOrEmpty(cmt))
            {
                gd3.TrackName = cmt;
                gd3.TrackNameJ = cmt;
            }
            return gd3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            GD3 = getGD3Info(vgmBuf, 0);
            this.chipRegister = chipRegister;
            LoopCounter = 0;
            vgmCurLoop = 0;
            this.model = model;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            SetZPDSearchPath();

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
                if (waitNextPlay-- > 0) return;

                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    if (vgmFrameCounter > -1)
                    {
                        Counter++;

                        if (version == 2)
                        {
                            timerOPM.timer();
                            while ((timerOPM.ReadStatus() & 3) != 0)
                            {
                                nise68.TrapOPM();// true, true, true);
                            }
                        }
                        else
                        {
                            //virtualFrameCounter++;
                            while (nise68.IntTimer())
                            {
#if DEBUG
                                //if (model != EnmModel.RealModel) 
                                //nise68.Trap(0x8e, true, true, true);
                                nise68.Trap(0x8e);
#else
        nise68.Trap(0x8e);
#endif
                            }
                        }
                    }
                    vgmFrameCounter++;

                }

                if (SkipSwitchPianoRoll) return;
                checkCounter--;
                if (checkCounter < 0)
                {
                    checkCounter = 100;
                    if (version == 2)
                    {
                        //演奏中か確認
                        nise68.reg.SetDl(1, 0x09);//m_stat
                        nise68.reg.SetDl(2, 0);//チェックモード(0:全チャンネル検査)
                        nise68.Trap(3 + 32);
                        uint d0 = nise68.reg.GetDl(0);
                        if (d0 == 0)
                        {
                            if (preData.Count < 1) Stopped = true;
                            else
                            {
                                preData.RemoveAt(0);
                                byte[] zmd = null;
                                if (preData.Count == 0)
                                {
                                    //if (nise68.hmn.fb.ContainsKey(fnZMD)) zmd = nise68.hmn.fb[fnZMD];
                                    if (fileMng.ExistsFile(fnZMD)) zmd = fileMng.VReadAllBytes(fnZMD);
                                }
                                else
                                {
                                    //if (nise68.hmn.fb.ContainsKey(preData[0])) zmd = nise68.hmn.fb[preData[0]];
                                    if (fileMng.ExistsFile(preData[0])) zmd = fileMng.VReadAllBytes(preData[0]);
                                }
                                uint fileSize = (uint)zmd.Length;
                                uint filePtr = (uint)nise68.hmn.memMng.Malloc(fileSize);
                                for (int i = 0; i < zmd.Length; i++)
                                {
                                    nise68.mem.PokeB((uint)(filePtr + i), zmd[i]);
                                }

                                nise68.reg.SetDl(1, 0x11);//play_cnv_data
                                nise68.reg.SetDl(2, (uint)(zmd.Length - 7));
                                nise68.reg.SetAl(1, filePtr + 7);
                                nise68.Trap(trp);//, true, true, true);
                                waitNextPlay = (int)(setting.outputDevice.SampleRate * (double)setting.zmusic.waitNextPlay / 1000.0);
                            }
                        }

                        //ループ回数チェック
                        nise68.reg.SetDl(1, 0x4d);//get_loop_time
                        nise68.Trap(3 + 32);
                        d0 = nise68.reg.GetDl(0);
                        vgmCurLoop = d0 - 1;
                    }
                    else
                    {
                        //演奏中か確認
                        nise68.reg.SetDl(0, 0x0b);//ZM_PLAY_STATUS
                        nise68.reg.SetDl(1, 0);//チェックモード(0:全チャンネル検査)
                        nise68.reg.SetAl(1, 0);//検査結果格納バッファアドレス(0にすると検査結果を簡略して返す)
                        nise68.Trap(3 + 32);
                        uint d0 = nise68.reg.GetDl(0);
                        if (d0 == 0)
                            Stopped = true;

                        //ループ回数チェック
                        nise68.reg.SetDl(0, 0x59);//ZM_LOOP_CONTROL
                        nise68.reg.SetDl(1, 0xffff_ffff);//コントロールモード(-1 = ループ回数取得)
                        nise68.Trap(3 + 32);
                        d0 = nise68.reg.GetDl(0);
                        vgmCurLoop = d0 - 1;
                    }
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
            //if (model == EnmModel.RealModel) { return; }

            string fn = PlayingFileName;
            string withoutExtFn;
            string? dn = Path.GetDirectoryName(fn);
            if (string.IsNullOrEmpty(dn)) dn = Path.GetDirectoryName(Application.ExecutablePath);
            if (!string.IsNullOrEmpty(dn)) withoutExtFn = Path.Combine(dn, Path.GetFileNameWithoutExtension(fn));
            else withoutExtFn = Path.GetFileNameWithoutExtension(fn);
            string fnZMD = Path.GetFileName(withoutExtFn + ".ZMD");
            string fnZMS = Path.GetFileName(withoutExtFn + ".ZMS");

            MDPlayer.Driver.ZMS.nise68.Log.SetMsgWrite(MsgWrite);
            nise68 = new nise68.nise68();
            nise68.SetMPCM(version==2 ? PCM8CallBack : MPCMCallBack);
            nise68.SetOPM(OPMCallBack);
            nise68.SetMIDI(MIDICallBack, (int)Common.VGMProcSampleRate);
            nise68.SetSCC_A(SCCCallBack, (int)Common.VGMProcSampleRate);
            if (!string.IsNullOrEmpty(PlayingArcFileName))
            {
                EnmFileFormat ff = Common.CheckExt(PlayingArcFileName);
                if (ff == EnmFileFormat.ZDF)
                {
                    UnZDF cmd= new UnZDF();
                    fileMng= cmd.Unpack(PlayingArcFileName);
                }

            }
            else
            {
                fileMng = new FileMng(dn);
            }
            nise68.Init(envZPDs, version == 2, fileMng);

            fileMng.SetVFile(Path.GetFileName(fnZMD), vgmBuf);
            //nise68.hmn.fb.Add(fnZMD, vgmBuf);
            //if (format == EnmFileFormat.ZMD) nise68.hmn.fb.Add(fnZMD, vgmBuf);
            //else
            //{
            //    //コンパイル
            //    if (model != EnmModel.RealModel || CompiledData == null) Compile(vgmBuf);
            //    else
            //    {
            //        //realはvirtualのコンパイル結果をもらう
            //        nise68.hmn.fb.Add(fnZMS, vgmBuf);
            //        nise68.hmn.fb.Add(fnZMD, CompiledData);
            //    }
            //}

            Play();
        }

        private List<string> preData = new List<string>();
        private string fnZMD;
        private int trp = 3 + 32;
        private int waitNextPlay = 0;

        private void Play()
        {
            string fn = PlayingFileName;
            string withoutExtFn;
            string? dn = Path.GetDirectoryName(fn);
            if (!string.IsNullOrEmpty(dn)) withoutExtFn = Path.Combine(dn, Path.GetFileNameWithoutExtension(fn));
            else withoutExtFn = Path.GetFileNameWithoutExtension(fn);
            fnZMD = Path.GetFileName(withoutExtFn + ".ZMD");
            string crntDir = Path.GetDirectoryName(Application.ExecutablePath);

            string zmsc3 = Path.Combine(crntDir, "ZMSC3.X");
            if (!File.Exists(zmsc3))
            {
                log.Write(LogLevel.Information, "File not found : {0}", zmsc3);
                throw new FileNotFoundException(zmsc3);
            }
            fileMng.SetVFile(zmsc3);
            zmsc3 = Path.GetFileName(zmsc3);

            string zmusic = Path.Combine(crntDir, "ZMUSIC.X");//ver2
            if (!File.Exists(zmusic))
            {
                log.Write(LogLevel.Information, "File not found : {0}", zmusic);
                throw new FileNotFoundException(zmusic);
            }
            fileMng.SetVFile(zmusic);
            zmusic = Path.GetFileName(zmusic);

            trp = 3 + 32;

            if (version == 2)
            {
                timerOPM = new FMTimer(true, null, 4000000);//, Common.VGMProcSampleRate);

                //zpdの指定がある場合は事前読み込みをzmusicに指定する
                string optionZpd = "";
                string optionZmd = "";
                preData.Clear();
                if (SupportFileBinaryAndName != null)
                {
                    foreach (Tuple<byte[], string> s in SupportFileBinaryAndName)
                    {
                        string ext = Path.GetExtension(s.Item2).ToUpper();
                        if (ext == ".ZPD")
                        {
                            optionZpd = " -B" + Path.GetFileName(s.Item2);
                            if (!fileMng.ExistsFile(s.Item2))
                            {
                                fileMng.SetVFile(s.Item2, s.Item1);
                            }
                        }
                        if (ext == ".ZMD" || ext == ".ZMS")
                        {
                            string f = s.Item2;
                            f = Path.ChangeExtension(f, ".ZMD");
                            optionZmd = " -N" + Path.GetFileName(f);
                            if (!fileMng.ExistsFile(f))
                            {
                                fileMng.SetVFile(f, s.Item1);
                                preData.Add(f);
                            }
                        }
                    }
                }

                nise68.hmn.memMng = new memMng((uint)(0x0001_2000 + (9212 + 2048) * 1024 + File.ReadAllBytes(zmusic).Length));

                //if (nise68.LoadRun(zmusic, "-P9212 -T2048" + optionZpd + optionZmd, Path.GetDirectoryName(fnZMD), 0x00012000
                //, true, true, true
                //) != 0) throw new Exception("zmusic regident Error");
                if (nise68.LoadRun(zmusic, "-P9212 -T2048" + optionZpd + optionZmd, 0x00012000
                , true, true, true
                ) != 0) throw new Exception("zmusic regident Error");

                if (pcm8type==0) opmPCM?.x68sound[0].MountMemory(nise68.mem.mem);
                else pcm8pp?.MountMemory(nise68.mem.mem);

                //演奏
                byte[] zmd = null;
                if (preData.Count < 1)
                {
                    if (File.Exists(fnZMD))
                    {
                        zmd = File.ReadAllBytes(fnZMD);
                        //if (!nise68.hmn.fb.ContainsKey(fnZMD))
                        //{
                        //    nise68.hmn.fb.Add(fnZMD, zmd);
                        //}
                        if(!fileMng.ExistsFile(fnZMD))
                        {
                            fileMng.SetVFile(fnZMD, zmd);
                        }
                    }
                    else
                    {
                        //if (nise68.hmn.fb.ContainsKey(fnZMD))
                        //{
                        //    zmd = nise68.hmn.fb[fnZMD];
                        //}
                        if (fileMng.ExistsFile(Path.GetFileName(fnZMD)))
                        {
                            zmd = fileMng.VReadAllBytes(Path.GetFileName(fnZMD));
                        }
                    }
                }
                else
                {
                    //if (nise68.hmn.fb.ContainsKey(preData[0]))
                    //{
                    //    zmd = nise68.hmn.fb[preData[0]];
                    //}
                    if (fileMng.ExistsFile(preData[0]))
                    {
                        zmd = fileMng.VReadAllBytes(preData[0]);
                    }
                }
                if (zmd == null)
                {
                    throw new Exception(string.Format("Zmd[{0}] file not found. ", fnZMD));
                }
                uint fileSize = (uint)zmd.Length;
                uint filePtr = (uint)nise68.hmn.memMng.Malloc(fileSize);
                for (int i = 0; i < zmd.Length; i++)
                {
                    nise68.mem.PokeB((uint)(filePtr + i), zmd[i]);
                }

                nise68.reg.SetDl(1, 0x11);//play_cnv_data
                nise68.reg.SetDl(2, (uint)(zmd.Length - 7));
                nise68.reg.SetAl(1, filePtr + 7);
                nise68.Trap(trp);//, true, true, true);

                return;
            }

            //zmsc3常駐
            nise68.hmn.memMng = new memMng(0x0004_0000);

            //if (nise68.LoadRun(zmsc3, "-w", Path.GetDirectoryName(fnZMD), 0x00012000
            //, true, true, true
            //) != 0) throw new Exception("zmsc3 regident Error");
            if (nise68.LoadRun(zmsc3, "-w", 0x00012000
           , true, true, true
           ) != 0) throw new Exception("zmsc3 regident Error");

            //演奏
            //Log.WriteLine(LogLevel.Information, "");
            //Log.SetLogLevel(LogLevel.Information);

            //if ((rc = nise68.LoadRun("C:\\ZP3.R", "-PC:\\SAMPLE1\\SAMPLE.ZMS", "C:\\", 0x00042000
            //    , true, true, true
            //    )) != 0) Environment.Exit(rc);
            {
                //エラーストックバッファ解放
                nise68.reg.SetDl(0, 0x73);//ZM_FREE_MEM2
                nise68.reg.SetDl(3, 0x82645252);// 'ＥRR'
                nise68.Trap(trp);
            }
            {
                //ZMD解放
                nise68.reg.SetDl(0, 0x73);//ZM_FREE_MEM2
                nise68.reg.SetDl(3, 0x5a826c44);// 'ZＭD'
                nise68.Trap(trp);
            }
            {
                //ZM_COMPILER DETECT
                nise68.reg.SetDl(0, 0x6b);//ZM_HOOK_FNC_SERVICE
                nise68.reg.SetDl(1, 2);// ZM_COMPILER
                nise68.reg.SetAl(1, 0xffff_ffff);//detect_mode
                nise68.Trap(trp);
            }
            {
                //バージョンチェック(呼ぶだけ)
                nise68.reg.SetDl(0, 0x7e);//ZM_ZMUSIC_MODE
                nise68.reg.SetDl(1, 0xffff_ffff);//
                nise68.Trap(trp);
            }
            {
                //演奏
                byte[] zmd = fileMng.VReadAllBytes(Path.GetFileName(fnZMD));// nise68.hmn.fb[fnZMD];
                uint fileSize = (uint)zmd.Length;
                uint filePtr = (uint)nise68.hmn.memMng.Malloc(fileSize);
                for (int i = 0; i < zmd.Length; i++)
                {
                    nise68.mem.PokeB((uint)(filePtr + i), zmd[i]);
                }
                nise68.reg.SetDl(0, 0x10);//ZM_PLAY_ZMD
                nise68.reg.SetDl(2, fileSize);
                nise68.reg.SetAl(1, filePtr + 8);
                nise68.Trap(trp);
            }
            {
                //エラーの確認
                nise68.reg.SetDl(0, 0x6e);//ZM_STORE_ERROR
                nise68.reg.SetDl(1, 0xffff_ffff);
                nise68.reg.SetDl(2, 0x0000_0000);
                nise68.Trap(trp);
                //D0.lにエラーの個数が返る
            }

        }

        public bool Compile(byte[] vgmBuf, string fn)
        {
            string withoutExtFn;
            string? dn = Path.GetDirectoryName(fn);
            if (!string.IsNullOrEmpty(dn)) withoutExtFn = Path.Combine(dn, Path.GetFileNameWithoutExtension(fn));
            else withoutExtFn = Path.GetFileNameWithoutExtension(fn);
            string fnZMD = Path.GetFileName(withoutExtFn + ".ZMD");
            string fnZMS = Path.GetFileName(withoutExtFn + ".ZMS");
            string crntDir = Path.GetDirectoryName(Application.ExecutablePath);
            string zmc = Path.Combine(crntDir, "ZMC.X");
            if (!File.Exists(zmc))
            {
                log.Write(LogLevel.Information, "File not found : {0}", zmc);
                return false;//throw new FileNotFoundException(zmc);
            }
            fileMng = new FileMng(dn);//曲ファイルのパスを物理ドライブのカレントに設定する.仮想ドライブのカレントは"C:"(デフォルト)
            fileMng.SetVFile(zmc);
            zmc = Path.GetFileName(zmc);

            MDPlayer.Driver.ZMS.nise68.Log.SetMsgWrite(MsgWrite);
            nise68 = new nise68.nise68();
            nise68.SetMPCM(MPCMCallBack);
            nise68.SetOPM(OPMCallBack);
            nise68.SetMIDI(MIDICallBack, (int)Common.VGMProcSampleRate);
            nise68.SetSCC_A(SCCCallBack, (int)Common.VGMProcSampleRate);
            nise68.Init(null, false, fileMng);

            //コンパイル
            //nise68.hmn.fb.Add(fnZMS, vgmBuf);
            fileMng.SetVFile(fnZMS, vgmBuf);
            //if (nise68.LoadRun(zmc, Path.GetFileName(fnZMS), Path.GetDirectoryName(fnZMS), 0x00012000
            //, true, true, true
            //) != 0)
            if (nise68.LoadRun(zmc, Path.GetFileName(fnZMS), 0x00012000
            , true, true, true
            ) != 0)
            {
                log.Write(LogLevel.Information, "v3 Compile Error", zmc);
                return false;
            }
            //CompiledData = nise68.hmn.fb[fnZMD];
            CompiledData = fileMng.VReadAllBytes(fnZMD);
            return true;
        }

        public bool Compilev2(byte[] vgmBuf,string fn)
        {
            //string fn = PlayingFileName;
            string withoutExtFn;
            string? dn = Path.GetDirectoryName(fn);
            if (!string.IsNullOrEmpty(dn)) withoutExtFn = Path.Combine(dn, Path.GetFileNameWithoutExtension(fn));
            else withoutExtFn = Path.GetFileNameWithoutExtension(fn);
            string fnZMD = Path.GetFileName(withoutExtFn + ".ZMD");
            string fnZMS = Path.GetFileName(withoutExtFn + ".ZMS");
            string crntDir = Path.GetDirectoryName(Application.ExecutablePath);
            string zmusic = Path.Combine(crntDir, "ZMUSIC.X");
            if (!File.Exists(zmusic))
            {
                log.Write(LogLevel.Information, "File not found : {0}", zmusic);
                return false;//throw new FileNotFoundException(zmc);
            }

            MDPlayer.Driver.ZMS.nise68.Log.SetMsgWrite(MsgWrite);
            nise68 = new nise68.nise68();
            nise68.SetMPCM(PCM8CallBack);
            nise68.SetOPM(OPMCallBack);
            nise68.SetMIDI(MIDICallBack, (int)Common.VGMProcSampleRate);
            nise68.SetSCC_A(SCCCallBack, (int)Common.VGMProcSampleRate);

            fileMng = new FileMng(dn);//曲ファイルのパスを物理ドライブのカレントに設定する.仮想ドライブのカレントは"C:"(デフォルト)
            fileMng.SetVFile(zmusic);
            zmusic = Path.GetFileName(zmusic);

            nise68.Init(null, false, fileMng);

            //コンパイル
            //nise68.hmn.fb.Add(fnZMS, vgmBuf);
            fileMng.SetVFile(fnZMS, vgmBuf);
            //if (nise68.LoadRun(zmusic, "-C " + Path.GetFileName(fnZMS), Path.GetDirectoryName(fnZMS), 0x00012000
            //, true, true, true
            //) != 0)
                if (nise68.LoadRun(zmusic, "-C " + fnZMS, 0x00012000
                , true, true, true
                ) != 0)
                {
                    log.Write(LogLevel.Information, "v2 Compile Error", zmusic);
                return false;
            }
            //CompiledData = nise68.hmn.fb[fnZMD];
            CompiledData = fileMng.VReadAllBytes(Path.GetFileName(fnZMD));

            return true;
        }

        private void MsgWrite(string arg1, object[] arg2)
        {
            log.Write(LogLevel.Information, arg1, arg2);
        }

        private int MPCMCallBack(int n)
        {
            int ch = n & 0xf;
            switch (n & 0xfff0)
            {
                case 0x0000:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_KEY_ON(${0:X04})", n);
                    if (mpcmtype == 0) mpcm?.KeyOn(0, ch);
                    else mpcmpp?.KeyOn(0, ch);
                    mpcmSt[ch].Keyon = true;
                    break;
                case 0x0100:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_KEY_OFF(${0:X04})", n);
                    if (mpcmtype == 0) mpcm?.KeyOff(0, ch);
                    else mpcmpp?.KeyOff(0, ch);
                    mpcmSt[ch].Keyoff = true;
                    break;
                case 0x0200:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_SET_PCM(${0:X04})", n);
                    if (mpcmtype == 0)
                    {
                        MDSound.mpcmX68k.SETPCM ptr = new mpcmX68k.SETPCM();
                        ptr.adrs_buf = nise68.mem.mem;
                        mpcmSt[ch].type = ptr.type = nise68.mem.PeekB(0x00 + nise68.reg.GetAl(1));
                        mpcmSt[ch].orig = ptr.orig = nise68.mem.PeekB(0x01 + nise68.reg.GetAl(1));
                        mpcmSt[ch].adrs_ptr = ptr.adrs_ptr = (int)nise68.mem.PeekL(0x04 + nise68.reg.GetAl(1));
                        mpcmSt[ch].size = ptr.size = nise68.mem.PeekL(0x08 + nise68.reg.GetAl(1));
                        mpcmSt[ch].start = ptr.start = nise68.mem.PeekL(0x0c + nise68.reg.GetAl(1));
                        mpcmSt[ch].end = ptr.end = nise68.mem.PeekL(0x10 + nise68.reg.GetAl(1));
                        mpcmSt[ch].count = ptr.count = nise68.mem.PeekL(0x14 + nise68.reg.GetAl(1));
                        //mpcmSt[ch].frq = mpcmSt[ch].type == 0xff ? 4 : (mpcmSt[ch].type == 1 ? 8 : (mpcmSt[ch].type == 2 ? 0x10 : 0));
                        if (mpcm != null)
                        {
                            mpcmSt[ch].rate = mpcm.m[0].rate;
                            mpcmSt[ch].base_ = mpcm.m[0].base_;
                        }

                        //nise68.DumpMemory((uint)ptr.adrs_ptr, (uint)(ptr.adrs_ptr + ptr.size));
                        mpcm?.SetPcm(0, ch, ptr);
                    }
                    else
                    {
                        MDSound.mpcmpp.SETPCM ptr = new mpcmpp.SETPCM();
                        ptr.adrs_buf = nise68.mem.mem;
                        mpcmSt[ch].type = ptr.type = nise68.mem.PeekB(0x00 + nise68.reg.GetAl(1));
                        mpcmSt[ch].orig = ptr.orig = nise68.mem.PeekB(0x01 + nise68.reg.GetAl(1));
                        mpcmSt[ch].adrs_ptr = ptr.adrs_ptr = (int)nise68.mem.PeekL(0x04 + nise68.reg.GetAl(1));
                        mpcmSt[ch].size = ptr.size = nise68.mem.PeekL(0x08 + nise68.reg.GetAl(1));
                        mpcmSt[ch].start = ptr.start = nise68.mem.PeekL(0x0c + nise68.reg.GetAl(1));
                        mpcmSt[ch].end = ptr.end = nise68.mem.PeekL(0x10 + nise68.reg.GetAl(1));
                        mpcmSt[ch].count = ptr.count = nise68.mem.PeekL(0x14 + nise68.reg.GetAl(1));
                        //mpcmSt[ch].frq = mpcmSt[ch].type == 0xff ? 4 : (mpcmSt[ch].type == 1 ? 8 : (mpcmSt[ch].type == 2 ? 0x10 : 0));
                        if (mpcmpp != null)
                        {
                            mpcmSt[ch].rate = mpcmpp.m[0].rate;
                            mpcmSt[ch].base_ = mpcmpp.m[0].base_;
                        }

                        //nise68.DumpMemory((uint)ptr.adrs_ptr, (uint)(ptr.adrs_ptr + ptr.size));
                        mpcmpp?.SetPcm(0, ch, ptr);
                    }
                    break;
                case 0x0300:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_SET_FRQ(${0:X04}) D1${1:X08}", n, nise68.reg.GetDl(1));
                    if (mpcmtype == 0)
                        mpcm?.SetFreq(0, ch, (int)nise68.reg.GetDl(1));
                    else
                        mpcmpp?.SetFreq(0, ch, (int)nise68.reg.GetDl(1));
                    mpcmSt[ch].frq = (int)nise68.reg.GetDl(1);
                    break;
                case 0x0400:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_SET_PITCH(${0:X04}) D1${1:X04}", n, nise68.reg.GetDl(1));
                    if (mpcmtype == 0)
                        mpcm?.SetPitch(0, ch, (int)nise68.reg.GetDl(1));
                    else
                        mpcmpp?.SetPitch(0, ch, (int)nise68.reg.GetDl(1));
                    mpcmSt[ch].pitch = (int)nise68.reg.GetDl(1);
                    break;
                case 0x0500:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_SET_VOL(${0:X04}) = ${1:X02}", n, nise68.reg.GetDb(1));
                    if (mpcmtype == 0)
                        mpcm?.SetVol(0, ch, (int)nise68.reg.GetDb(1));
                    else
                        mpcmpp?.SetVol(0, ch, (int)nise68.reg.GetDb(1));
                    mpcmSt[n & 0xf].volume = (int)nise68.reg.GetDb(1);
                    break;
                case 0x0600:
                    //Log.WriteLine(LogLevel.Trace2, "MPCM #M_SET_PAN(${0:X04}) = ${1:X02}", n, nise68.reg.GetDb(1));
                    if (mpcmtype == 0)
                        mpcm?.SetPan(0, ch, (int)nise68.reg.GetDb(1));
                    else
                        mpcmpp?.SetPan(0, ch, (int)nise68.reg.GetDb(1));
                    mpcmSt[n & 0xf].pan = (int)nise68.reg.GetDb(1);
                    break;
                case 0x8000://
                    switch (n & 0x000f)
                    {
                        case 0x0:
                            //Log.WriteLine(LogLevel.Trace2, "MPCM #M_LOCK(${0:X04})", n);
                            break;
                        case 0x2://
                                 //Log.WriteLine(LogLevel.Trace2, "MPCM #M_INIT(${0:X04})", n);
                            if (mpcmtype == 0)
                                mpcm?.Reset(0);
                            else
                                mpcmpp?.Reset(0);
                            break;
                        case 0x5://
                                 //Log.WriteLine(LogLevel.Trace2, "MPCM #M_SET_VOLTBL(${0:X04})", n);
                            int[] vtbl = new int[128];
                            for (int i = 0; i < 128; i++)
                            {
                                vtbl[i] = (int)(nise68.mem.PeekW((uint)(nise68.reg.GetAl(1) + (i * 2))));
                            }
                            if (mpcmtype == 0)
                                mpcm?.SetVolTableZms(0, (int)nise68.reg.GetDl(1), vtbl);
                            else
                                mpcmpp?.SetVolTableZms(0, (int)nise68.reg.GetDl(1), vtbl);
                            break;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return 0;
        }

        private int PCM8CallBack(int n)
        {
            int ch;
            switch (n & 0xfff0)
            {
                case 0x0000:
                    //File.WriteAllBytes("c:\\temp\\test.bin", nise68.mem.mem);
                    if (pcm8type == 0) opmPCM?.x68sound[0].X68Sound_Pcm8_Out((int)n & 0xff, null, nise68.reg.GetAl(1), (int)nise68.reg.GetDl(1), (int)nise68.reg.GetDl(2));//指定チャンネル発音開始
                    else pcm8pp?.KeyOn((int)n & 0xff, nise68.reg.GetAl(1), (int)nise68.reg.GetDl(1), (int)nise68.reg.GetDl(2));//指定チャンネル発音開始
                    //Debug.WriteLine("{3} adrsPtr = 0x{0:x08};  mode = 0x{1:x08}; len = 0x{2:x08};", nise68.reg.GetAl(1), (int)nise68.reg.GetDl(1), (int)nise68.reg.GetDl(2), (int)n & 0xff);
                    ch = (int)((n & 0xff) % 8);
                    pcm8St[ch].tablePtr = nise68.reg.GetAl(1);
                    pcm8St[ch].mode = nise68.reg.GetDl(1);
                    pcm8St[ch].length = nise68.reg.GetDl(2);
                    pcm8St[ch].Keyon = true;
                    break;
                case 0x0100:
                    switch (n & 0xffff)
                    {
                        case 0x0100:
                            ch = (int)((n & 0xff) % 8);
                            pcm8St[ch].tablePtr = 0;
                            pcm8St[ch].mode = 0;
                            pcm8St[ch].length = 0;
                            pcm8St[ch].Keyon = false;
                            if (pcm8type == 0) opmPCM?.x68sound[0].X68Sound_Pcm8_Out((int)n & 0xff, null, 0, 0, 0);//指定チャンネル発音停止
                            else pcm8pp?.KeyOff((int)n & 0xff);//指定チャンネル発音停止
                            break;
                        case 0x0101:
                            opmPCM?.x68sound[0].X68Sound_Pcm8_Abort();//全チャンネル発音停止
                            break;
                    }
                    break;
                case 0x01F0:
                    switch (n & 0xffff)
                    {
                        case 0x01FC:
                            nise68.reg.SetDl(0, 1);
                            break;
                    }
                    break;
                default:
                    break;
            }
            return 0;
        }

        private int OPMCallBack(int adr, int dat)
        {
            chipRegister.setYM2151Register(0, 0, adr, dat, model, YM2151Hosei[0], vgmFrameCounter);
            timerOPM?.WriteReg((byte)adr, (byte)dat);
            return 0;
        }

        private int MIDICallBack(int n, byte dat)
        {
            //if (midiOutsBuff == null || midiOutsFrame == null) return 0;
            //if (n >= midiOutsBuff.Count) return 0;

            //midiOutsBuff[n].Add(dat);
            //midiOutsFrame[n].Add(virtualFrameCounter);
            chipRegister.sendMIDIout(model,n,new byte[] { dat });
            return 0;
        }

        private int SCCCallBack(int n, byte dat)
        {
            //if (midiOutsBuff == null || midiOutsFrame == null) return 0;
            //if (2 + n >= midiOutsBuff.Count) return 0;

            //midiOutsBuff[2 + n].Add(dat);
            //midiOutsFrame[2 + n].Add(virtualFrameCounter);
            chipRegister.sendMIDIout(model, 2+n, new byte[] { dat });
            return 0;
        }


        private void SetZPDSearchPath()
        {
            try
            {
                //環境変数"ZPD"を取得する
                string envZPD = "";
                try
                {
                    envZPD = Environment.GetEnvironmentVariable("zmusic_ZPD", System.EnvironmentVariableTarget.User);
                }
                catch
                {
                }
                if (!string.IsNullOrEmpty(envZPD))
                {
                    envZPDs = envZPD.Split(";").ToList();
                }
            }
            catch
            {
                envZPDs = new List<string>();
            }

        }

    }
}
