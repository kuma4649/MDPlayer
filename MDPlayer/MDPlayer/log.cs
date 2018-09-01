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
        public static bool consoleEchoBack = false;
        private static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

        public static void ForcedWrite(string msg)
        {
            try
            {
                if (path == "")
                {
                    string fullPath = common.GetApplicationDataFolder(true);
                    path = Path.Combine(fullPath, Properties.Resources.cntLogFilename);
                    if (File.Exists(path)) File.Delete(path);
                }
                string timefmt = DateTime.Now.ToString(Properties.Resources.cntTimeFormat);

                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    writer.WriteLine(timefmt + msg);
                    if (consoleEchoBack) Console.WriteLine(timefmt + msg);
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
                    string fullPath = common.GetApplicationDataFolder(true);
                    path = Path.Combine(fullPath, Properties.Resources.cntLogFilename);
                    if (File.Exists(path)) File.Delete(path);
                }
                string timefmt = DateTime.Now.ToString(Properties.Resources.cntTimeFormat);

                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    string msg = string.Format(Properties.Resources.cntExceptionFormat, e.GetType().Name, e.Message, e.Source, e.StackTrace);
                    Exception ie = e;
                    while (ie.InnerException != null)
                    {
                        ie = ie.InnerException;
                        msg += string.Format(Properties.Resources.cntInnerExceptionFormat, ie.GetType().Name, ie.Message, ie.Source, ie.StackTrace);
                    }

                    writer.WriteLine(timefmt + msg);
                    if (consoleEchoBack) Console.WriteLine(timefmt + msg);
                }
            }
            catch
            {
            }
        }

        public static void Write(string msg)
        {
            if (!debug) return;

            try
            {
                if (path == "")
                {
                    string fullPath = common.GetApplicationDataFolder(true);
                    path = Path.Combine(fullPath, Properties.Resources.cntLogFilename);
                    if (File.Exists(path)) File.Delete(path);
                }
                string timefmt = DateTime.Now.ToString(Properties.Resources.cntTimeFormat);

                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    writer.WriteLine(timefmt + msg);
                    if (consoleEchoBack) Console.WriteLine(timefmt + msg);
                }
            }
            catch
            {
            }
        }

    }
}
