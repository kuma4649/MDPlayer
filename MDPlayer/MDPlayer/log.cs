using System;
using System.Reflection;
using System.Text;
using System.IO;

namespace MDPlayer
{
    public static class log
    {
        public static string path = "";
        public static bool debug = false;

        public static void ForcedWrite(string msg)
        {
            try
            {
                if (path == "")
                {
                    string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    path = Path.Combine(fullPath, "log.txt");
                    if (File.Exists(path)) File.Delete(path);
                }

                DateTime dtNow = DateTime.Now;
                string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    writer.WriteLine(timefmt + msg);
                }
            }
            catch
            {
            }
        }

        public static void ForcedWrite(Exception e)
        {
            try
            {
                if (path == "")
                {
                    string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    path = Path.Combine(fullPath, "log.txt");
                    if (File.Exists(path)) File.Delete(path);
                }

                DateTime dtNow = DateTime.Now;
                string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    string msg = string.Format("例外発生:\r\n- Message ------\r\n{0}\r\n- Source ------\r\n{1}\r\n- StackTrace ------\r\n{2}\r\n", e.Message, e.Source, e.StackTrace);
                    writer.WriteLine(timefmt + msg);
                    System.Console.WriteLine(msg);
                }
            }
            catch
            {
            }
        }

        public static void Write(string msg)
        {
            if (!debug)
            {
                return;
            }

            try
            {
                if (path == "")
                {
                    string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    path = Path.Combine(fullPath, "log.txt");
                    if (File.Exists(path)) File.Delete(path);
                }

                DateTime dtNow = DateTime.Now;
                string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    writer.WriteLine(timefmt + msg);
                }
            }
            catch
            {
            }
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
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
    }
}
