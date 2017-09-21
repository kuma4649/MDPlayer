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
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

            return GD3;
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
            if (filename.ToLower().LastIndexOf(".rcp") != -1) return enmFileFormat.RCP;
            if (filename.ToLower().LastIndexOf(".s98") != -1) return enmFileFormat.S98;
            if (filename.ToLower().LastIndexOf(".vgm") != -1) return enmFileFormat.VGM;
            if (filename.ToLower().LastIndexOf(".vgz") != -1) return enmFileFormat.VGM;
            if (filename.ToLower().LastIndexOf(".xgm") != -1) return enmFileFormat.XGM;
            if (filename.ToLower().LastIndexOf(".zip") != -1) return enmFileFormat.ZIP;

            return enmFileFormat.unknown;
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
        , SN76489 = 1
        , YM2612 = 2
        , YM2612Ch6 = 4
        , RF5C164 = 8
        , PWM = 16
        , C140 = 32
        , OKIM6258 = 64
        , OKIM6295 = 128
        , SEGAPCM = 256
        , YM2151 = 512
        , YM2608 = 1024
        , YM2203 = 2048
        , YM2610 = 4096
        , AY8910 = 9192
        , HuC6280 = 18384
        , YM2413 = 36768
    }

    public enum enmScciChipType : int
    {
        YM2608 = 1
        , YM2151 = 2
        , YM2610 = 3
        , YM2203 = 4
        , YM2612 = 5
        , SN76489 = 7
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
        ZIP = 8,
        M3U = 9
    }


}
