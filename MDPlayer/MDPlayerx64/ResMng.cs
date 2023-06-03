#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using MDPlayer;
using System.Collections;
using MDPlayer.Driver.MGSDRV;
using System.IO.Compression;

namespace MDPlayerx64
{
    public static class ResMng
    {
        public static Dictionary<string, Bitmap> imgDic = null;
        public static string imgZipPath = "";
        public static List<Bitmap> bmpBox = null;

        public static void Init(string zipFile)
        {
            Release();
            imgZipPath = zipFile;
            imgDic = new();
            bmpBox = new();

            ResourceSet resset = Resources.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true);
            foreach (var item in resset.OfType<DictionaryEntry>().Where(e => e.Value is Bitmap))
            {
                imgDic.Add(item.Key.ToString(), (Bitmap)item.Value);
            }

            if (!File.Exists(imgZipPath)) return;

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(imgZipPath))
                {
                    foreach (string item in imgDic.Keys)
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
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private static void GetFile(ZipArchiveEntry entry,string item,string fn)
        {
            MemoryStream ms = new MemoryStream();
            entry.Open().CopyTo(ms);
            Bitmap bmp = new Bitmap(ms);
            bmpBox.Add(bmp);
            imgDic[item] = bmp;
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
            if (bmpBox == null) return;

            foreach (var bmp in bmpBox)
            {
                bmp.Dispose();
            }
        }

    }
}
