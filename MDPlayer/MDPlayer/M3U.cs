using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class M3U
    {
        public static PlayList LoadM3U(string filename, string rootPath)
        {
            try
            {
                PlayList pl = new PlayList();

                using (StreamReader sr = new StreamReader(filename, Encoding.GetEncoding(932)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        line = line.Trim();
                        if (line == "") continue;
                        if (line[0] == '#') continue;

                        PlayList.music ms = analyzeLine(line, rootPath);
                        ms.format = common.CheckExt(ms.fileName);
                        if (ms != null) pl.lstMusic.Add(ms);

                    }
                }

                return pl;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new PlayList();
            }
        }

        public static PlayList LoadM3U(ZipArchiveEntry entry,string zipFileName)
        {
            try
            {
                PlayList pl = new PlayList();

                using (StreamReader sr = new StreamReader(entry.Open(), Encoding.GetEncoding(932)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        line = line.Trim();
                        if (line == "") continue;
                        if (line[0] == '#') continue;

                        PlayList.music ms = analyzeLine(line, "");
                        ms.format = common.CheckExt(ms.fileName);
                        ms.zipFileName = zipFileName;
                        if (ms != null) pl.lstMusic.Add(ms);

                    }
                }

                return pl;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new PlayList();
            }
        }

        private static PlayList.music analyzeLine(string line,string rootPath)
        {
            PlayList.music ms = new PlayList.music();

            try
            {
                // ::が無い場合は全てをファイル名として処理終了
                if (line.IndexOf("::") < 0)
                {
                    ms.fileName = line;
                    if (!Path.IsPathRooted(ms.fileName) && rootPath!="")
                    {
                        ms.fileName = Path.Combine(rootPath, ms.fileName);
                    }

                    return ms;
                }

                string[] buf = line.Split(new string[] { "::" }, StringSplitOptions.None);

                ms.fileName = buf[0].Trim();
                if (!Path.IsPathRooted(ms.fileName) && rootPath != "")
                {
                    ms.fileName = Path.Combine(rootPath, ms.fileName);
                }
                if (buf.Length < 1) return ms;

                buf = buf[1].Split(new string[] { "," }, StringSplitOptions.None);
                List<string> lbuf = new List<string>();
                for (int i = 0; i < buf.Length;)
                {
                    string s = "";
                    bool flg = false;
                    do
                    {
                        flg = false;
                        s += buf[i];
                        if (buf[i].Length != 0 && buf[i].LastIndexOf('\\') == buf[i].Length - 1)
                        {
                            s += ",";
                            flg = true;
                        }
                        i++;
                    } while (flg);
                    lbuf.Add(s.Replace("\\", ""));
                }
                buf = lbuf.ToArray();

                string fType = buf[0].Trim().ToUpper();
                if (buf.Length < 2) return ms;

                ms.songNo = analyzeSongNo(buf[1].Trim()) - (fType == "NSF" ? 1 : 0);
                if (buf.Length < 3) return ms;

                ms.title = buf[2].Trim();
                ms.titleJ = buf[2].Trim();
                if (buf.Length < 4) return ms;

                ms.time = buf[3].Trim();
                if (buf.Length < 5) return ms;

                analyzeLoopTime(buf[4], ref ms.loopStartTime, ref ms.loopEndTime);
                if (buf.Length < 6) return ms;

                ms.fadeoutTime = buf[5].Trim();
                if (buf.Length < 7) return ms;

                if (!int.TryParse(buf[6].Trim(), out ms.loopCount)) ms.loopCount = -1;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return null;
            }

            return ms;
        }

        private static int analyzeSongNo(string s)
        {
            int n = -1;

            if (s.Length > 0 && s[0] == '$')
            {
                if (s.Length < 1 || !int.TryParse(s.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out n))
                {
                    return -1;
                }
            }
            else
            {
                if (!int.TryParse(s, out n)) return -1;
            }

            return n;
        }

        private static void analyzeLoopTime(string s, ref string loopStartTime, ref string loopEndTime)
        {
            loopStartTime = "";
            loopEndTime = "";

            if (s.Length > 0 && s[s.Length - 1] == '-')
            {
                loopStartTime = s.Substring(0, s.Length - 1);
                return;
            }

            loopEndTime = s;
        }
    }
}
