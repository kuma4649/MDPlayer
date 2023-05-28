#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
using System.Text;

namespace MDPlayer
{
    public static class log
    {
#if DEBUG
        public static bool debug = true;
        public static LogLevel logLevel = LogLevel.Trace;
#else
        public static bool debug = false;
        public static LogLevel logLevel = LogLevel.Information;
#endif
        private static object logLock = new object();
        public static bool consoleEchoBack = false;
        private static Encoding sjisEnc;
        public static string path = "";
        public static Action<string> logger = null;

        static log()
        {
#if X64
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#else
#endif
            sjisEnc = Encoding.GetEncoding("Shift_JIS");
        }

        private static void CheckPath()
        {
            if (path != "") return;
            string fullPath = Common.settingFilePath;
            path = Path.Combine(fullPath, Resources.cntLogFilename);
            if (File.Exists(path)) File.Delete(path);
        }

        private static void logging(LogLevel logLvl, string msg, params object[] prm)
        {
            if (logLvl != LogLevel.Enforcement && !debug && logLevel > logLvl) return;

            lock (logLock)
            {
                string timefmt = DateTime.Now.ToString(Resources.cntTimeFormat).Trim();
                string mmsg=string.Format(msg, prm);
                string tmsg = string.Format("[{0}][{1}]{2}", timefmt, logLvl, mmsg);
                logger?.Invoke(tmsg);
                if (consoleEchoBack) Console.WriteLine(tmsg);
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc)) writer.WriteLine(tmsg);
            }
        }

        public static void ForcedWrite(string msg, params object[] prm)
        {
            try
            {
                CheckPath();
                logging(LogLevel.Enforcement, msg, prm);
            }
            catch
            {
            }
        }

        public static void ForcedWrite(Exception e)
        {
            try
            {
                CheckPath();
                string msg = string.Format(Resources.cntExceptionFormat, e.GetType().Name, e.Message, e.Source, e.StackTrace);
                Exception ie = e;
                while (ie.InnerException != null)
                {
                    ie = ie.InnerException;
                    msg += string.Format(Resources.cntInnerExceptionFormat, ie.GetType().Name, ie.Message, ie.Source, ie.StackTrace);
                }
                logging(LogLevel.Enforcement, msg);
            }
            catch
            {
            }
        }

        public static void Write(string msg,params object[] prm)
        {
            try
            {
                CheckPath();
                logging(LogLevel.Information, msg, prm);
            }
            catch
            {
            }
        }

        public static void Write(LogLevel logLevel, string msg,params object[] prm)
        {
            try
            {
                CheckPath();
                logging(logLevel, msg, prm);
            }
            catch
            {
            }
        }

        public static void SetLogger(Action<string> logger)
        {
            lock (logLock)
            {
                log.logger = logger;
            }
        }
    }
}