using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDPlayer;
using MDPlayer.Driver;

namespace mdpc
{
    public class mdpc
    {
        private string[] args;
        private bool waveout = false;
        private string srcFn;
        private string desFn;
        public const int FCC_VGM = 0x206D6756;  // "Vgm "
        private Setting setting;
        //private WaveWriter ww;
        private EnmFileFormat format;
        private byte[] vgmBuf;
        //private double vgmSpeed;
        //private MDPlayer.baseDriver driver;
        private bool emuOnly=false;

        public mdpc(string[] args)
        {
            this.args = args;

            //ファイル、オプションの指定無し
            if (args == null || args.Length < 1)
            {
                //disp usage
                Console.WriteLine(msg.get("I00000"));
                Environment.Exit(0);
            }

            //オプションの解析
            int cnt = 0;
            try
            {
                List<string> lstOpt = new List<string>();
                while (args[cnt].Length > 1 && args[cnt][0] == '-')
                {
                    lstOpt.Add(args[cnt++].Substring(1));
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
                Console.WriteLine(msg.get("E0000"));
                Environment.Exit(0);
            }

            //ファイルの指定無し
            if (args == null || args.Length < cnt)
            {
                //disp usage
                Console.WriteLine(msg.get("I00000"));
                Environment.Exit(0);
            }

            //vgmファイル名の取得
            srcFn = args[cnt++];
            if (Path.GetExtension(srcFn) == "")
            {
                srcFn += ".vgm";
            }

            //wavファイル名の取得
            if (args.Length > cnt)
            {
                desFn = args[cnt];
            }
            else
            {
                desFn = Path.Combine(Path.GetDirectoryName(srcFn), Path.GetFileNameWithoutExtension(srcFn) + ".wav");
            }

            int ret = Start();
            log.Close();
            Environment.Exit(ret);
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
                procMain();

                while (!Audio.GetVGMStopped())
                {
                    System.Threading.Thread.Sleep(1);
                    if (Console.KeyAvailable)
                    {
                        string outChar = Console.ReadKey().Key.ToString();
                        if (outChar == "Spacebar")
                        {
                            break;
                        }
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
                Audio.closeWaveWriter();
            }

            log.Write("End");
            log.Close();

            return ret;
        }

        public byte[] getAllBytes(string filename, out EnmFileFormat format)
        {
            format = EnmFileFormat.unknown;

            //先ずは丸ごと読み込む
            byte[] buf = System.IO.File.ReadAllBytes(filename);

            string ext = Path.GetExtension(filename).ToLower();

            //.NRDファイルの場合は拡張子判定
            if (ext == ".nrd")
            {
                format = EnmFileFormat.NRT;
                return buf;
            }

            if (ext == ".mdr")
            {
                format = EnmFileFormat.MDR;
                return buf;
            }

            if (ext == ".mdx")
            {
                format = EnmFileFormat.MDX;
                return buf;
            }

            if (ext == ".mnd")
            {
                format = EnmFileFormat.MND;
                return buf;
            }

            if (ext == ".mub")
            {
                format = EnmFileFormat.MUB;
                return buf;
            }

            if (ext == ".muc")
            {
                format = EnmFileFormat.MUC;
                return buf;
            }

            if (ext == ".xgm")
            {
                format = EnmFileFormat.XGM;
                return buf;
            }

            if (ext == ".s98")
            {
                format = EnmFileFormat.S98;
                return buf;
            }

            if (ext == ".nsf")
            {
                format = EnmFileFormat.NSF;
                return buf;
            }

            if (ext == ".hes")
            {
                format = EnmFileFormat.HES;
                return buf;
            }

            if (ext == ".sid")
            {
                format = EnmFileFormat.SID;
                return buf;
            }

            if (ext == ".mid")
            {
                format = EnmFileFormat.MID;
                return buf;
            }

            if (ext == ".rcp")
            {
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

            int num;
            buf = new byte[1024]; // 1Kbytesずつ処理する

            FileStream inStream // 入力ストリーム
              = new FileStream(filename, FileMode.Open, FileAccess.Read);

            GZipStream decompStream // 解凍ストリーム
              = new GZipStream(
                inStream, // 入力元となるストリームを指定
                CompressionMode.Decompress); // 解凍（圧縮解除）を指定

            MemoryStream outStream // 出力ストリーム
              = new MemoryStream();

            using (inStream)
            using (outStream)
            using (decompStream)
            {
                while ((num = decompStream.Read(buf, 0, buf.Length)) > 0)
                {
                    outStream.Write(buf, 0, num);
                }
            }

            format = EnmFileFormat.VGM;
            return outStream.ToArray();
        }

        private void procMain()
        {
            Common.settingFilePath = Common.GetApplicationDataFolder(true);
            vgmBuf = getAllBytes(srcFn, out format);
            //Audio.isCommandLine = true;
            Audio.emuOnly = emuOnly;
            Audio.Init(setting);
            Audio.SetVGMBuffer(format, vgmBuf, srcFn, "", 0, 0, null);
            Audio.Play(setting);
            Audio.GO();
        }

    }
}
