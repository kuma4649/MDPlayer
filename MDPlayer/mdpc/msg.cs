using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mdpc
{
    public static class msg
    {

        private static Dictionary<string, string> dicMsg = new Dictionary<string, string>();

        static msg()
        {
            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = Path.GetDirectoryName(myAssembly.Location);
            string lang = System.Globalization.CultureInfo.CurrentCulture.Name;
            string file = Path.Combine(path, "lang", string.Format("message.{0}.txt", lang));
            file = file.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
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
            catch
            {

            }

            if (lines != null)
            {
                foreach (string line in lines)
                {
                    try
                    {
                        if (line == null) continue;
                        if (line == "") continue;
                        string str = line.Trim();
                        if (str == "") continue;
                        if (str[0] == ';') continue;
                        string code = str.Substring(0, str.IndexOf("=")).Trim();
                        string msg = str.Substring(str.IndexOf("=") + 1, str.Length - str.IndexOf("=") - 1);
                        if (dicMsg.ContainsKey(code)) continue;

                        dicMsg.Add(code, msg);
                    }
                    catch { }
                }
            }
        }

        public static string get(string code)
        {
            if (dicMsg.ContainsKey(code))
            {
                return dicMsg[code].Replace("\\r", "\r").Replace("\\n", "\n");
            }
            return "<no message>";
        }

    }
}
