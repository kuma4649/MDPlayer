using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public static class common
    {
        public static Int32 SampleRate = 44100;
        public static Int32 NsfClock = 1789773;
       

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
                GD3.TrackName = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //trackNameJ
                GD3.TrackNameJ = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //gameName
                GD3.GameName = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //gameNameJ
                GD3.GameNameJ = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //systemName
                GD3.SystemName = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //systemNameJ
                GD3.SystemNameJ = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //Composer
                GD3.Composer = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //ComposerJ
                GD3.ComposerJ = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //Converted
                GD3.Converted = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //VGMBy
                GD3.VGMBy = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //Notes
                GD3.Notes = Encoding.Unicode.GetString(common.getByteArray(buf, ref adr));
                //Lyric(独自拡張)
                byte[] bLyric = common.getByteArray(buf, ref adr);
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

                                int cnt = int.Parse(m.Substring(1, m.IndexOf("]")-1));
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
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static int Range(int n, int min, int max)
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
            while(true)
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

        public static enmFileFormat CheckExt(string filename)
        {
            if (filename.ToLower().LastIndexOf(".m3u") != -1) return enmFileFormat.M3U;
            if (filename.ToLower().LastIndexOf(".mid") != -1) return enmFileFormat.MID;
            if (filename.ToLower().LastIndexOf(".nrd") != -1) return enmFileFormat.NRT;
            if (filename.ToLower().LastIndexOf(".nsf") != -1) return enmFileFormat.NSF;
            if (filename.ToLower().LastIndexOf(".hes") != -1) return enmFileFormat.HES;
            if (filename.ToLower().LastIndexOf(".sid") != -1) return enmFileFormat.SID;
            if (filename.ToLower().LastIndexOf(".mnd") != -1) return enmFileFormat.MND;
            if (filename.ToLower().LastIndexOf(".mdr") != -1) return enmFileFormat.MDR;
            if (filename.ToLower().LastIndexOf(".mdx") != -1) return enmFileFormat.MDX;
            if (filename.ToLower().LastIndexOf(".rcp") != -1) return enmFileFormat.RCP;
            if (filename.ToLower().LastIndexOf(".s98") != -1) return enmFileFormat.S98;
            if (filename.ToLower().LastIndexOf(".vgm") != -1) return enmFileFormat.VGM;
            if (filename.ToLower().LastIndexOf(".vgz") != -1) return enmFileFormat.VGM;
            if (filename.ToLower().LastIndexOf(".xgm") != -1) return enmFileFormat.XGM;
            if (filename.ToLower().LastIndexOf(".zip") != -1) return enmFileFormat.ZIP;
            if (filename.ToLower().LastIndexOf(".lzh") != -1) return enmFileFormat.LZH;

            return enmFileFormat.unknown;
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

        public static int searchPCMNote(int ml)
        {
            int m = int.MaxValue;
            ml = ml % 1024;
            int n = 0;
            for (int i = 0; i < 12; i++)
            {
                int a = Math.Abs(ml - Tables.pcmpitchTbl[i]);
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

        public static int GetYM2151Hosei(float YM2151ClockValue,float baseClock)
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
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fullPath = System.IO.Path.Combine(fullPath, "KumaApp", AssemblyTitle);
            if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);

            return fullPath;
        }
    }

    public enum enmModel
    {
        VirtualModel
        , RealModel
    }

    public enum enmUseChip : int
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
    }

    public enum enmScciChipType : int
    {
        YM2608 = 1
        , YM2151 = 2
        , YM2610 = 3
        , YM2203 = 4
        , YM2612 = 5
        , SN76489 = 7
        , SPPCM = 42
        , C140 = 43
        , SEGAPCM = 44
    }

    public enum enmInstFormat : int
    {
        FMP7 = 0,
        MDX = 1,
        TFI = 2,
        MUSICLALF = 3,
        MUSICLALF2 = 4,
        MML2VGM = 5,
        NRTDRV = 6,
        HUSIC=7
    }

    public enum enmFileFormat : int
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
        MND = 15
    }

    public enum enmArcType : int
    {
        unknown = 0,
        ZIP = 1,
        LZH = 2
    }


}
