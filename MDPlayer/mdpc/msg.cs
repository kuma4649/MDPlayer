using System.Reflection;

namespace mdpc
{
    public static class Msg
    {

        private static readonly Dictionary<string, string> dicMsg = new();

        static Msg()
        {
            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = Path.GetDirectoryName(myAssembly.Location);
            string lang = System.Globalization.CultureInfo.CurrentCulture.Name;
            string file = Path.Combine(path, "lang", string.Format("message.{0}.txt", lang));
            file = file.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            string[] lines = ReadFile(path, ref file);
            if (lines == null) return;

            MakeDictionary(lines);
        }

        private static void MakeDictionary(string[] lines)
        {
            foreach (string line in lines)
            {
                try
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    string str = line.Trim();
                    if (string.IsNullOrEmpty(str)) continue;
                    if (str[0] == ';') continue;
                    string code = str[..str.IndexOf("=")].Trim();
                    if (dicMsg.ContainsKey(code)) continue;
                    string msg = str.Substring(str.IndexOf("=") + 1, str.Length - str.IndexOf("=") - 1);
                    dicMsg.Add(code, msg);
                }
                catch { }
            }
        }

        private static string[] ReadFile(string path, ref string file)
        {
            string[] lines = null;
            try
            {
                if (!File.Exists(file))
                {
                    file = Path.Combine(path, "lang", "message.txt");
                    file = file.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                }
                lines = File.ReadAllLines(file);
            }
            catch { }

            return lines;
        }

        public static string Get(string code)
        {
            if (dicMsg.ContainsKey(code))
            {
                return dicMsg[code].Replace("\\r", "\r").Replace("\\n", "\n");
            }
            return "<no message>";
        }

    }
}
