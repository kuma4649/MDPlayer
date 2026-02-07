using System.Globalization;
using System.IO.Compression;
using System.Text;
using static MDPlayer.PlayList;

namespace MDPlayer
{
    public class M3U
    {
        private static readonly string EXTM3U = "#EXTM3U";
        private static readonly string EXTINF = "#EXTINF:";
        private static readonly char Comment = '#';

        public static PlayList LoadM3U(string filename, string rootPath)
        {
            try
            {
                PlayList pl = new PlayList();
                bool isM3U = false;
                string info = "";

                using (StreamReader sr = new StreamReader(filename, Encoding.GetEncoding(932)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        line = line.Trim();
                        if (line == "") continue;
                        if (line == EXTM3U) { isM3U = true; continue; }
                        if (line.IndexOf(EXTINF)==0 && isM3U) {
                            info = line.Substring(EXTINF.Length);
                            continue; 
                        }
                        if (line[0] == Comment) continue;

                        PlayList.Music ms = analyzeLine(line, rootPath);
                        ms.format = Common.CheckExt(ms.fileName);
                        ms.title = ms.fileName;
                        ms.titleJ = ms.fileName;
                        ms.composer = "";
                        ms.composerJ = "";
                        if (!string.IsNullOrEmpty(info))
                        {
                            try
                            {
                                info = info.Substring(info.IndexOf(',')+1);
                                ms.title = info.Substring(info.LastIndexOf(" - ")+2).Trim();
                                ms.titleJ = ms.title;
                                ms.composer = info.Substring(0, info.LastIndexOf(" - ")).Trim();
                                ms.composerJ = ms.composer;
                            }
                            catch
                            {
                                ms.title = info;
                                ms.titleJ = ms.title;
                                ms.composer = "(Unknown)";
                                ms.composerJ = ms.composer;
                            }
                            info = "";
                        }
                        if (ms != null) pl.LstMusic.Add(ms);

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

        public static PlayList LoadM3U(object entry, string zipFileName)
        {
            if (entry is ZipArchiveEntry) return LoadM3U((ZipArchiveEntry)entry, zipFileName);
            else return LoadM3U(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2, zipFileName);
        }

        private static PlayList LoadM3U(ZipArchiveEntry entry, string zipFileName)
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

                        PlayList.Music ms = analyzeLine(line, "");
                        ms.format = Common.CheckExt(ms.fileName);
                        ms.arcFileName = zipFileName;
                        if (ms != null) pl.LstMusic.Add(ms);

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

        private static PlayList LoadM3U(string archiveFile, string fileName, string zipFileName)
        {
            try
            {
                PlayList pl = new PlayList();
                UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                byte[] buf = cmd.GetFileByte(archiveFile, fileName);
                string[] text = Encoding.GetEncoding(932).GetString(buf).Split(new string[] { "\r\n" }, StringSplitOptions.None);

                foreach (string txt in text)
                {
                    string line = txt.Trim();
                    if (line == "") continue;
                    if (line[0] == '#') continue;

                    PlayList.Music ms = analyzeLine(line, "");
                    ms.format = Common.CheckExt(ms.fileName);
                    ms.arcFileName = zipFileName;
                    if (ms != null) pl.LstMusic.Add(ms);

                }

                return pl;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new PlayList();
            }
        }

        private static PlayList.Music analyzeLine(string line, string rootPath)
        {
            PlayList.Music ms = new PlayList.Music();

            try
            {
                // ::が無い場合は全てをファイル名として処理終了
                if (line.ToLower().IndexOf("http://") >= 0 
                    || line.ToLower().IndexOf("https://") >= 0)
                {
                    ms.fileName = line;
                    return ms;
                }

                // ::が無い場合は全てをファイル名として処理終了
                if (line.IndexOf("::") < 0)
                {
                    ms.fileName = line;
                    if (!Path.IsPathRooted(ms.fileName) && rootPath != "")
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
