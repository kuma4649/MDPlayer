using Konamiman.Z80dotNet;
using MDSound.np;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public static class Common
    {
        public const int DEV_WaveOut = 0;
        public const int DEV_DirectSound = 1;
        public const int DEV_WasapiOut = 2;
        public const int DEV_AsioOut = 3;
        public const int DEV_SPPCM = 4;
        public const int DEV_Null = 5;

        public static Int32 VGMProcSampleRate = 44100;
        public static Int32 NsfClock = 1789773;
        public static string settingFilePath = "";
        public static string playingFilePath = "";

        public static UInt32 getBE16(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 1)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] * 0x100 + (UInt32)buf[adr + 1];

            return dat;
        }

        public static UInt32 getLE16(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 1)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100;

            return dat;
        }

        public static UInt32 getLE24(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 2)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100 + (UInt32)buf[adr + 2] * 0x10000;

            return dat;
        }

        public static UInt32 getLE32(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 3)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100 + (UInt32)buf[adr + 2] * 0x10000 + (UInt32)buf[adr + 3] * 0x1000000;

            return dat;
        }

        public static byte[] getByteArray(byte[] buf, ref uint adr)
        {
            if (adr >= buf.Length) return null;

            List<byte> ary = new List<byte>();
            while (buf[adr] != 0 || buf[adr + 1] != 0)
            {
                ary.Add(buf[adr]);
                adr++;
                ary.Add(buf[adr]);
                adr++;
            }
            adr += 2;

            return ary.ToArray();
        }

        public static GD3 getGD3Info(byte[] buf, uint adr)
        {
            GD3 GD3 = new GD3();

            GD3.TrackName = "";
            GD3.TrackNameJ = "";
            GD3.GameName = "";
            GD3.GameNameJ = "";
            GD3.SystemName = "";
            GD3.SystemNameJ = "";
            GD3.Composer = "";
            GD3.ComposerJ = "";
            GD3.Converted = "";
            GD3.Notes = "";
            GD3.VGMBy = "";
            GD3.Version = "";
            GD3.UsedChips = "";

            try
            {
                //trackName
                try { GD3.TrackName = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.TrackName = ""; }
                //trackNameJ
                try { GD3.TrackNameJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.TrackNameJ = ""; }
                //gameName
                try { GD3.GameName = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.GameName = ""; }
                //gameNameJ
                try { GD3.GameNameJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.GameNameJ = ""; }
                //systemName
                try { GD3.SystemName = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.SystemName = ""; }
                //systemNameJ
                try { GD3.SystemNameJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.SystemNameJ = ""; }
                //Composer
                try { GD3.Composer = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.Composer = ""; }
                //ComposerJ
                try { GD3.ComposerJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.ComposerJ = ""; }
                //Converted
                try { GD3.Converted = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.Converted = ""; }
                //VGMBy
                try { GD3.VGMBy = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.VGMBy = ""; }
                //Notes
                try { GD3.Notes = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr)); } catch { GD3.Notes = ""; }
                //Lyric(独自拡張)
                byte[] bLyric = Common.getByteArray(buf, ref adr);
                if (bLyric != null)
                {
                    GD3.Lyrics = new List<Tuple<int, int, string>>();
                    int i = 0;
                    int st = 0;
                    while (i < bLyric.Length)
                    {
                        byte h = bLyric[i];
                        byte l = bLyric[i + 1];
                        if ((h == 0x5b && l == 0x00 && i != 0) || i >= bLyric.Length - 2)
                        {
                            if ((i >= bLyric.Length - 2) || (bLyric[i + 2] != 0x5b || bLyric[i + 3] != 0x00))
                            {
                                string m = Encoding.Unicode.GetString(bLyric, st, i - st + ((i >= bLyric.Length - 2) ? 2 : 0));
                                st = i;

                                int cnt = int.Parse(m.Substring(1, m.IndexOf("]") - 1));
                                m = m.Substring(m.IndexOf("]") + 1);
                                GD3.Lyrics.Add(new Tuple<int, int, string>(cnt, cnt, m));
                            }
                        }
                        i += 2;
                    }
                }
                else
                {
                    GD3.Lyrics = null;
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

            return GD3;
        }

        public static string getNRDString(byte[] buf, ref uint index)
        {
            if (buf == null || buf.Length < 1 || index < 0 || index >= buf.Length) return "";

            try
            {
                List<byte> lst = new List<byte>();
                for (; buf[index] != 0; index++)
                {
                    if (buf.Length > index + 1 && buf[index] == 0x1a && buf[index + 1] == 0x00)
                        break;
                    lst.Add(buf[index]);
                }

                string n = System.Text.Encoding.GetEncoding(932).GetString(lst.ToArray());
                index++;

                return n;
            }
            catch (Exception e)
            {
                log.ForcedWrite(e);
            }
            return "";
        }
        public static string getNRDString(IMemory memory, ref uint adr)
        {
            if (memory == null || memory.Size < 1 || adr < 0 || adr >= memory.Size) return "";

            try
            {
                List<byte> lst = new List<byte>();
                for (int index=(int)adr; memory[index] != 0; index++)
                {
                    if (memory.Size > index + 1 && memory[index] == 0x1a && memory[index + 1] == 0x00)
                        break;
                    lst.Add(memory[index]);
                }

                string n = System.Text.Encoding.GetEncoding(932).GetString(lst.ToArray());
                adr++;

                return n;
            }
            catch (Exception e)
            {
                log.ForcedWrite(e);
            }
            return "";
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            }
        }

        public static int Range(int n, int min, int max)
        {
            return (n > max) ? max : (n < min ? min : n);
        }

        public static uint Range(uint n, uint min, uint max)
        {
            return (n > max) ? max : (n < min ? min : n);
        }

        public static int getvv(byte[] buf, ref uint musicPtr)
        {
            int s = 0, n = 0;

            do
            {
                n |= (buf[musicPtr] & 0x7f) << s;
                s += 7;
            }
            while ((buf[musicPtr++] & 0x80) > 0);

            return n + 2;
        }

        public static int getv(byte[] buf, ref uint musicPtr)
        {
            int s = 0, n = 0;

            do
            {
                n |= (buf[musicPtr] & 0x7f) << s;
                s += 7;
            }
            while ((buf[musicPtr++] & 0x80) > 0);

            return n;
        }

        public static int getDelta(ref uint trkPtr, byte[] bs)
        {
            int delta = 0;
            while (true)
            {
                delta = (delta << 7) + (bs[trkPtr] & 0x7f);
                if ((bs[trkPtr] & 0x80) == 0)
                {
                    trkPtr++;
                    break;
                }
                trkPtr++;
            }

            return delta;
        }

        public static EnmFileFormat CheckExt(string filename)
        {
            if (filename.ToLower().LastIndexOf(".m3u") != -1) return EnmFileFormat.M3U;
            if (filename.ToLower().LastIndexOf(".mid") != -1) return EnmFileFormat.MID;
            if (filename.ToLower().LastIndexOf(".nrd") != -1) return EnmFileFormat.NRT;
            if (filename.ToLower().LastIndexOf(".nsf") != -1) return EnmFileFormat.NSF;
            if (filename.ToLower().LastIndexOf(".hes") != -1) return EnmFileFormat.HES;
            if (filename.ToLower().LastIndexOf(".sid") != -1) return EnmFileFormat.SID;
            if (filename.ToLower().LastIndexOf(".mnd") != -1) return EnmFileFormat.MND;
            if (filename.ToLower().LastIndexOf(".mdr") != -1) return EnmFileFormat.MDR;
            if (filename.ToLower().LastIndexOf(".mdx") != -1) return EnmFileFormat.MDX;
            if (filename.ToLower().LastIndexOf(".mub") != -1) return EnmFileFormat.MUB;
            if (filename.ToLower().LastIndexOf(".muc") != -1) return EnmFileFormat.MUC;
            if (filename.ToLower().LastIndexOf(".mml") != -1) return EnmFileFormat.MML;
            if (filename.ToLower().LastIndexOf(".mgs") != -1) return EnmFileFormat.MGS;
            if (filename.ToLower().LastIndexOf(".msd") != -1) return EnmFileFormat.MuSICA_src;
            if (filename.ToLower().LastIndexOf(".bgm") != -1) return EnmFileFormat.MuSICA;
            if (filename.ToLower().LastIndexOf(".m") != -1) return EnmFileFormat.M;
            if (filename.ToLower().LastIndexOf(".m2") != -1) return EnmFileFormat.M;
            if (filename.ToLower().LastIndexOf(".mz") != -1) return EnmFileFormat.M;
            if (filename.ToLower().LastIndexOf(".rcp") != -1) return EnmFileFormat.RCP;
            if (filename.ToLower().LastIndexOf(".s98") != -1) return EnmFileFormat.S98;
            if (filename.ToLower().LastIndexOf(".vgm") != -1) return EnmFileFormat.VGM;
            if (filename.ToLower().LastIndexOf(".vgz") != -1) return EnmFileFormat.VGM;
            if (filename.ToLower().LastIndexOf(".xgm") != -1) return EnmFileFormat.XGM;
            if (filename.ToLower().LastIndexOf(".xgz") != -1) return EnmFileFormat.XGM;
            if (filename.ToLower().LastIndexOf(".zgm") != -1) return EnmFileFormat.ZGM;
            if (filename.ToLower().LastIndexOf(".zip") != -1) return EnmFileFormat.ZIP;
            if (filename.ToLower().LastIndexOf(".lzh") != -1) return EnmFileFormat.LZH;
            if (filename.ToLower().LastIndexOf(".wav") != -1) return EnmFileFormat.WAV;
            if (filename.ToLower().LastIndexOf(".mp3") != -1) return EnmFileFormat.MP3;
            if (filename.ToLower().LastIndexOf(".aiff") != -1) return EnmFileFormat.AIFF;

            return EnmFileFormat.unknown;
        }

        public static int searchFMNote(int freq)
        {
            int m = int.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 5; i++)
            {
                //if (freq < Tables.FmFNum[i]) break;
                //n = i;
                int a = Math.Abs(freq - Tables.FmFNum[i]);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n - 12 * 3;
        }

        public static int searchSSGNote(float freq)
        {
            float m = float.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                //if (freq < Tables.freqTbl[i]) break;
                //n = i;
                float a = Math.Abs(freq - Tables.freqTbl[i]);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        public static int searchSegaPCMNote(double ml)
        {
            double m = double.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 8; i++)
            {
                double a = Math.Abs(ml - (Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 4))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        public static int searchPCMNote(int ml, int mul)
        {
            int m = int.MaxValue;
            ml = ml % (1024 * mul);
            int n = 0;
            for (int i = 0; i < 12; i++)
            {
                int a = Math.Abs(ml - Tables.pcmpitchTbl[i] * mul);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n;
        }

        public static int searchYM2608Adpcm(float freq)
        {
            float m = float.MaxValue;
            int n = 0;

            for (int i = 0; i < 12 * 8; i++)
            {
                if (freq < Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 3))) break;
                n = i;
                float a = Math.Abs(freq - (float)(Tables.pcmMulTbl[i % 12 + 12] * Math.Pow(2, ((int)(i / 12) - 3))));
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }

            return n + 1;
        }

        public static int GetYM2151Hosei(float YM2151ClockValue, float baseClock)
        {
            int ret = 0;

            float delta = (float)YM2151ClockValue / baseClock;
            float d;
            float oldD = float.MaxValue;
            for (int i = 0; i < Tables.pcmMulTbl.Length; i++)
            {
                d = Math.Abs(delta - Tables.pcmMulTbl[i]);
                ret = i;
                if (d > oldD) break;
                oldD = d;
            }
            ret -= 12;

            return ret;
        }

        public static string GetApplicationFolder()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(path))
            {
                path += path[path.Length - 1] == '\\' ? "" : "\\";
            }
            return path;
        }

        public static string GetApplicationDataFolder(bool make = false)
        {
            try
            {
                string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string fullPath;
                fullPath = System.IO.Path.Combine(appPath, "KumaApp", AssemblyTitle);
                if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);

                return fullPath;
            }
            catch
            {
                return null;
            }
        }

        public static string GetOperationFolder(bool make = false)
        {
            try
            {
                string appDataFolder = GetApplicationDataFolder(false);
                if (string.IsNullOrEmpty(appDataFolder)) return null;
                string fullPath = System.IO.Path.Combine(appDataFolder, "operation");
                if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                else
                    //存在するならそのフォルダの中身をクリア
                    DeleteDataUnderDirectory(fullPath);
                return fullPath;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        ///ディクトリを空にする
        ///
        /// 以下のサイトのメソッドを使用しています。ありがとうございます！
        ///from
        ///がんず Work's Diary
        ///URL : https://gannzuswork.hatenablog.com/entry/2021/05/23/%E3%80%90C%23_%E9%80%86%E5%BC%95%E3%81%8D%E3%80%91%E6%8C%87%E5%AE%9A%E3%81%AE%E3%83%95%E3%82%A9%E3%83%AB%E3%83%80%E3%81%AE%E4%B8%AD%E8%BA%AB%E3%82%92%E5%89%8A%E9%99%A4
        ///
        /// </summary>
        public static void DeleteDataUnderDirectory(string directory)
        {
            DirectoryInfo DB = new DirectoryInfo(directory);

            //ファイル消す
            foreach (FileInfo file in DB.GetFiles())
            {
                file.Delete();
            }

            //フォルダも消す
            foreach (DirectoryInfo dir in DB.GetDirectories())
            {
                //フォルダ以下のすべてのファイル、フォルダの属性を削除
                RemoveReadonlyAttribute(dir);

                //フォルダを根こそぎ削除
                dir.Delete(true);
            }


        }


        /// <summary>
        /// 
        ///フォルダ/ファイルの属性を変更する
        ///
        /// 以下のサイトのメソッドを使用しています。ありがとうございます！
        ///from
        ///がんず Work's Diary
        ///URL : https://gannzuswork.hatenablog.com/entry/2021/05/23/%E3%80%90C%23_%E9%80%86%E5%BC%95%E3%81%8D%E3%80%91%E6%8C%87%E5%AE%9A%E3%81%AE%E3%83%95%E3%82%A9%E3%83%AB%E3%83%80%E3%81%AE%E4%B8%AD%E8%BA%AB%E3%82%92%E5%89%8A%E9%99%A4
        ///
        /// </summary>
        public static void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
                if ((fi.Attributes & FileAttributes.ReadOnly) ==
                    FileAttributes.ReadOnly)
                    fi.Attributes = FileAttributes.Normal;
            //サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                RemoveReadonlyAttribute(di);
        }



        public static Stream GetOPNARyhthmStream(string fn)
        {
            string ffn = fn;

            string chk;

            chk = Path.Combine(playingFilePath, fn);
            if (File.Exists(chk))
                ffn = chk;
            else
            {
                chk = Path.Combine(GetApplicationFolder(), fn);
                if (File.Exists(chk)) ffn = chk;
            }

            try
            {
                if (!File.Exists(ffn)) return null;
                FileStream fs = new FileStream(ffn, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fs;
            }
            catch
            {
                return null;
            }
        }





        public static byte[] unzipFile(string filename, ZipArchiveEntry entry=null)
        {
            int xnum;
            byte[] buf = new byte[1024]; // 1Kbytesずつ処理する

            Stream xinStream; // 入力ストリーム
            if (entry == null)
                xinStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            else
                xinStream = entry.Open();

            GZipStream xdecompStream // 解凍ストリーム
              = new GZipStream(
                xinStream, // 入力元となるストリームを指定
                CompressionMode.Decompress); // 解凍（圧縮解除）を指定

            MemoryStream xoutStream // 出力ストリーム
              = new MemoryStream();

            using (xinStream)
            using (xoutStream)
            using (xdecompStream)
            {
                while ((xnum = xdecompStream.Read(buf, 0, buf.Length)) > 0)
                {
                    xoutStream.Write(buf, 0, xnum);
                }
            }

            return xoutStream.ToArray();
        }

    }

    public enum LogLevel : int
    {
        Trace = 0,
        Debug,
        Warning,
        Error,
        Information,
        Enforcement
    }

    public enum EnmModel
    {
        VirtualModel
        , RealModel
    }

    public enum EnmChip : int
    {
        Unuse = 0
        , SN76489
        , YM2612
        , YM2612Ch6
        , RF5C164
        , PWM
        , C140
        , OKIM6258
        , OKIM6295
        , SEGAPCM
        , YM2151
        , YM2608
        , YM2203
        , YM2610
        , AY8910
        , HuC6280
        , YM2413
        , NES
        , DMC
        , FDS
        , MMC5
        , YMF262
        , YMF278B
        , VRC7
        , C352
        , YM3526
        , Y8950
        , YM3812
        , K051649
        , N163
        , VRC6
        , FME7
        , RF5C68
        , MultiPCM
        , uPD7759
        , YMF271
        , YMZ280B
        , QSound
        , GA20
        , K053260
        , K054539
        , DMG
        , SAA1099
        , X1_010
        , PPZ8
        , PPSDRV
        , SID
        , P86
        , POKEY
        , WSwan
        , YM2609
        , ES5503

        , S_SN76489
        , S_YM2612
        , S_YM2612Ch6
        , S_RF5C164
        , S_PWM
        , S_C140
        , S_OKIM6258
        , S_OKIM6295
        , S_SEGAPCM
        , S_YM2151
        , S_YM2608
        , S_YM2203
        , S_YM2610
        , S_AY8910
        , S_HuC6280
        , S_YM2413
        , S_NES
        , S_DMC
        , S_FDS
        , S_MMC5
        , S_YMF262
        , S_YMF278B
        , S_VRC7
        , S_C352
        , S_YM3526
        , S_Y8950
        , S_YM3812
        , S_K051649
        , S_N163
        , S_VRC6
        , S_FME7
        , S_RF5C68
        , S_MultiPCM
        , S_uPD7759
        , S_YMF271
        , S_YMZ280B
        , S_QSound
        , S_GA20
        , S_K053260
        , S_K054539
        , S_DMG
        , S_SAA1099
        , S_X1_010
        , S_PPZ8
        , S_PPSDRV
        , S_SID
        , S_P86
        , S_POKEY
        , S_WSwan
        , S_YM2609
        , S_ES5503

    }

    public enum EnmRealChipType : int
    {
        YM2608 = 1
        , YM2151 = 2
        , YM2610 = 3
        , YM2203 = 4
        , YM2612 = 5
        , AY8910 = 6
        , SN76489 = 7
        , YM3812 = 8
        , YMF262 = 9
        , YM2413 = 10
        , YM3526 = 11
        , K051649 = 13
        , SPPCM = 42
        , C140 = 43
        , SEGAPCM = 44

    }

    public enum EnmInstFormat : int
    {
        FMP7 = 0,
        MDX = 1,
        TFI = 2,
        MUSICLALF = 3,
        MUSICLALF2 = 4,
        MML2VGM = 5,
        NRTDRV = 6,
        HUSIC = 7,
        VOPM = 8,
        PMD = 9,
        MUCOM88 = 10,
        DMP = 11,
        OPNI = 12,
        OPLI = 13,
        MGSCSCC_PLAIN = 14,
        RYM2612 = 15,
        SendMML2VGM = 16,
    }

    public enum EnmFileFormat : int
    {
        unknown = 0,
        VGM = 1,
        NRT = 2,
        XGM = 3,
        S98 = 4,
        MID = 5,
        RCP = 6,
        NSF = 7,
        HES = 8,
        ZIP = 9,
        M3U = 10,
        SID = 11,
        MDR = 12,
        LZH = 13,
        MDX = 14,
        MND = 15,
        MUB = 16,
        MUC = 17,
        ZGM = 18,
        MML = 19,
        M = 20,
        WAV = 21,
        MP3 = 22,
        AIFF = 23,
        MGS = 24,
        MDL=25,
        XGZ=26,
        MuSICA=27,
        MuSICA_src=28
    }

    public enum EnmArcType : int
    {
        unknown = 0,
        ZIP = 1,
        LZH = 2
    }

    public enum EnmRealModel
    {
        unknown,
        SCCI,
        GIMIC
    }

}
