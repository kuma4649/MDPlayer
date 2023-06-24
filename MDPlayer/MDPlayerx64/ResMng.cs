#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
using System.Resources;
using MDPlayer;
using System.Collections;
using System.IO.Compression;

namespace MDPlayerx64
{
    public static class ResMng
    {
        private static readonly object lockobject = new();
        private static Dictionary<string, Bitmap> _imgDic = null;
        public static Dictionary<string, Bitmap> ImgDic
        {
            get
            {
                lock (lockobject) return _imgDic;
            }
            set
            {
                lock (lockobject) _imgDic = value;
            }
        }
        private static string _imgZipPath = "";
        public static string ImgZipPath
        {
            get
            {
                lock (lockobject) return _imgZipPath;
            }
            set
            {
                lock (lockobject) _imgZipPath = value;
            }
        }
        private static List<Bitmap> _bmpBox = null;
        public static List<Bitmap> BmpBox
        {
            get
            {
                lock (lockobject) return _bmpBox;
            }
            set
            {
                lock (lockobject) _bmpBox = value;
            }
        }

        public static void Init(string zipFile)
        {
            Release();
            ImgZipPath = zipFile;
            ImgDic = new();
            BmpBox = new();

            ResourceSet resset = Resources.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true);
            foreach (var item in resset.OfType<DictionaryEntry>().Where(e => e.Value is Bitmap))
            {
                ImgDic.Add(item.Key.ToString(), (Bitmap)item.Value);
            }

            if (!File.Exists(ImgZipPath)) return;

            try
            {
                using ZipArchive archive = ZipFile.OpenRead(ImgZipPath);
                foreach (string item in ImgDic.Keys)
                {
                    try
                    {
                        ZipArchiveEntry entry;
                        string fn = item + ".bmp";
                        if ((entry = ZipFileExists(archive, fn)) != null)
                        {
                            GetFile(entry, item, fn);
                            continue;
                        }
                        fn = item + ".png";
                        if ((entry = ZipFileExists(archive, fn)) != null)
                        {
                            GetFile(entry, item, fn);
                            continue;
                        }

                        log.Write("{0} use DefaultImage.", item);
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private static void GetFile(ZipArchiveEntry entry, string item, string fn)
        {
            MemoryStream ms = new();
            entry.Open().CopyTo(ms);
            Bitmap bmp = new(ms);
            BmpBox.Add(bmp);
            ImgDic[item] = bmp;
            log.Write("{0} use '{1}'.", item, fn);
        }

        private static ZipArchiveEntry ZipFileExists(ZipArchive archive, string fn)
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name == fn) return entry;
            }
            return null;
        }

        public static void Release()
        {
            if (BmpBox == null) return;

            foreach (var bmp in BmpBox)
            {
                bmp.Dispose();
            }
        }

    }
}
