using MDPlayer;

#pragma warning disable IDE1006

namespace mdpc
{
    public class mdpc
    {
        private readonly string srcFn;
        //private readonly string desFn;
        private const int FCC_VGM = 0x206D6756;  // "Vgm "
        private Setting setting;
        private EnmFileFormat format;
        private byte[] vgmBuf;
        private bool waveout = false;
        private bool emuOnly = false;

        public mdpc(string[] args)
        {
            //ファイル、オプションの指定無し
            if (args == null || args.Length < 1)
            {
                //disp usage
                Console.WriteLine(Msg.Get("I00000"));
                Environment.Exit(0);
            }
            //オプションの解析
            int cnt = AnalyzeOption(args);
            //ファイルの指定無し
            if (args.Length < cnt)
            {
                //disp usage
                Console.WriteLine(Msg.Get("I00000"));
                Environment.Exit(0);
            }
            //vgmファイル名の取得
            srcFn = args[cnt++];
            if (Path.GetExtension(srcFn) == "") srcFn += ".vgm";
            //wavファイル名の取得
            //desFn = Path.Combine(Path.GetDirectoryName(srcFn), Path.GetFileNameWithoutExtension(srcFn) + ".wav");
            //if (args.Length > cnt) desFn = args[cnt];

            int ret = Start();
            log.Close();
            Environment.Exit(ret);
        }

        private int AnalyzeOption(string[] args)
        {
            int cnt = 0;
            try
            {
                List<string> lstOpt = new();
                while (args[cnt].Length > 1 && args[cnt][0] == '-')
                {
                    lstOpt.Add(args[cnt++][1..]);
                }

                foreach (string opt in lstOpt)
                {
                    //vgm switch
                    switch (opt[0])
                    {
                        case 'w':
                            waveout = true;
                            break;
                        case 'e':
                            emuOnly = true;
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine(Msg.Get("E0000"));
                Environment.Exit(0);
            }

            return cnt;
        }

        private int Start()
        {
            int ret = 0;

            try
            {
                log.debug = false;
                log.Open();
                log.Write("Start");
                log.Write("(Stop ... Hit Spacebar)");

                if (!File.Exists(srcFn))
                {
                    log.Write(string.Format("File not found.({0})", srcFn));
                    return -1;
                }

                setting = new Setting();
                setting.other.WavSwitch = waveout;
                ProcMain();

                while (!Audio.GetVGMStopped())
                {
                    Thread.Sleep(1);
                    if (Console.KeyAvailable)
                    {
                        string outChar = Console.ReadKey().Key.ToString();
                        if (outChar == "Spacebar") break;
                    }
                }

            }
            catch (Exception e)
            {
                ret = -1;
                log.ForcedWrite(e);
            }
            finally
            {
                Audio.Stop();
                Audio.CloseWaveWriter();
            }

            log.Write("End");
            log.Close();

            return ret;
        }

        public static byte[] GetAllBytes(string filename, out EnmFileFormat format)
        {
            //先ずは丸ごと読み込む
            byte[] buf = File.ReadAllBytes(filename);
            string ext = Path.GetExtension(filename).ToLower();

            switch (ext)
            {
                case ".nrd":
                    format = EnmFileFormat.NRT;
                    return buf;
                case ".mdr":
                    format = EnmFileFormat.MDR;
                    return buf;
                case ".mdx":
                    format = EnmFileFormat.MDX;
                    return buf;
                case ".mnd":
                    format = EnmFileFormat.MND;
                    return buf;
                case ".mub":
                    format = EnmFileFormat.MUB;
                    return buf;
                case ".muc":
                    format = EnmFileFormat.MUC;
                    return buf;
                case ".xgm":
                    format = EnmFileFormat.XGM;
                    return buf;
                case ".s98":
                    format = EnmFileFormat.S98;
                    return buf;
                case ".nsf":
                    format = EnmFileFormat.NSF;
                    return buf;
                case ".hes":
                    format = EnmFileFormat.HES;
                    return buf;
                case ".sid":
                    format = EnmFileFormat.SID;
                    return buf;
                case ".mid":
                    format = EnmFileFormat.MID;
                    return buf;
                case ".rcp":
                    format = EnmFileFormat.RCP;
                    return buf;
            }

            //.VGMの場合はヘッダの確認とGzipで解凍後のファイルのヘッダの確認
            uint vgm = (UInt32)buf[0] + (UInt32)buf[1] * 0x100 + (UInt32)buf[2] * 0x10000 + (UInt32)buf[3] * 0x1000000;
            if (vgm == FCC_VGM)
            {
                format = EnmFileFormat.VGM;
                return buf;
            }

            buf = Common.unzipFile(filename);
            format = EnmFileFormat.VGM;
            return buf;
        }

        private void ProcMain()
        {
            Common.settingFilePath = Common.GetApplicationDataFolder(true);
            vgmBuf = GetAllBytes(srcFn, out format);
            //Audio.isCommandLine = true;
            Audio.EmuOnly = emuOnly;
            Audio.Init(setting);
            Audio.SetVGMBuffer(format, vgmBuf, srcFn, "", 0, 0, null);
            Audio.Play(setting);
            Audio.GO();
        }

    }
}
