using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mdpc
{
    public static class log
    {
        public static string path = "";
        public static bool debug = false;
        public static StreamWriter writer;

        public static void ForcedWrite(string msg)
        {
            try
            {
                CheckPath();

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

        private static void CheckPath()
        {
            if (path == "")
            {
                string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                path = Path.Combine(fullPath, "log.txt");
                if (File.Exists(path)) File.Delete(path);
            }
        }

        public static void ForcedWrite(Exception e)
        {
            try
            {
                CheckPath();

                DateTime dtNow = DateTime.Now;
                string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                string msg="";
                Open();
                try
                {
                    msg = string.Format("例外発生:\r\n- Type ------\r\n{0}\r\n- Message ------\r\n{1}\r\n- Source ------\r\n{2}\r\n- StackTrace ------\r\n{3}\r\n", e.GetType().Name, e.Message, e.Source, e.StackTrace);
                    Exception ie = e;
                    while (ie.InnerException != null)
                    {
                        ie = ie.InnerException;
                        msg += string.Format("内部例外:\r\n- Type ------\r\n{0}\r\n- Message ------\r\n{1}\r\n- Source ------\r\n{2}\r\n- StackTrace ------\r\n{3}\r\n", ie.GetType().Name, ie.Message, ie.Source, ie.StackTrace);
                    }
                }
                finally
                {
                    Close();
                }

                writer.WriteLine(timefmt + msg);
                System.Console.WriteLine(msg);
            }
            catch
            {
            }
        }

        public static void Write(string msg)
        {
            DateTime dtNow = DateTime.Now;
            string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");
            Console.WriteLine(msg);
            if (writer == null) return;
            try
            {
                CheckPath();
                Open();

                writer.WriteLine(timefmt + msg);
                writer.Flush();
            }
            catch
            {
            }
        }

        public static void Wlog(string msg)
        {
            if (!debug) return;
            if (writer == null) return;
            try
            {
                DateTime dtNow = DateTime.Now;
                string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                CheckPath();
                Open();

                writer.WriteLine(timefmt + msg);
                writer.Flush();
            }
            catch
            {
            }
        }

        public static void Open()
        {
            try
            {
                CheckPath();
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                if (!debug) return;
                if (writer == null)
                {
                    writer = new StreamWriter(path, true, sjisEnc);
                }
            }
            catch
            {
                writer = null;
            }
        }

        public static void Close()
        {
            try
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
            catch { }
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
