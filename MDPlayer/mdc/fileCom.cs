using System;
using System.Collections.Generic;
using System.IO;

namespace mdc
{
    public class FileCom : KumaCom
    {
        private readonly bool unuse = true;
        private string comPath = "";
        private FileSystemWatcher watcher = null;
        private readonly List<string> lstFile = new List<string>();

        public FileCom(bool isClient, string appName, string mmfName, int _)
        {
            if (isClient)
            {
                comPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                comPath = System.IO.Path.Combine(comPath, "KumaApp", appName, mmfName);
                if (Directory.Exists(comPath)) unuse = false;
            }
            else
            {
                unuse = !Init(appName, mmfName);
            }
        }

        ~FileCom()
        {
            Close();
        }

        public override bool Init(string appName, string pathName)
        {
            try
            {
                string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dataFolder = Path.Combine(dataFolder, "KumaApp", appName);
                if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
                comPath = Path.Combine(dataFolder, pathName);
                if (!Directory.Exists(comPath)) Directory.CreateDirectory(comPath);
                string[] fs = Directory.GetFiles(comPath);
                foreach (string f in fs) File.Delete(f);
                watcher = new FileSystemWatcher
                {
                    Path = comPath,
                    NotifyFilter =
                    (
                    NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    ),
                    EnableRaisingEvents = true
                };
                watcher.Changed += new FileSystemEventHandler(Watcher_Changed);
                watcher.Created += new FileSystemEventHandler(Watcher_Changed);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public override bool Open(string mmfName, int mmfSize)
        {
            if (unuse) return false;
            return false;
        }

        public override void Close()
        {
            if (unuse) return;
            try
            {
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    watcher = null;
                }
            }
            catch { }
        }

        public override string GetMessage()
        {
            if (unuse) return "";

            try
            {
                string file = "";
                do
                {
                    if (lstFile.Count < 1) break;
                    file = lstFile[0];
                    lstFile.RemoveAt(0);
                } while (!File.Exists(file));
                if (string.IsNullOrEmpty(file)) return "";

                string body = File.ReadAllText(file);
                File.Delete(file);
                return body;
            }
            catch
            {
                return "";
            }
        }

        public override byte[] GetBytes()
        {
            if (unuse) return null;

            try
            {
                string file = "";

                string[] fs = Directory.GetFiles(comPath);
                do
                {
                    if (fs.Length < 1) break;
                    file = fs[0];
                } while (!File.Exists(file));
                if (string.IsNullOrEmpty(file)) return null;

                byte[] body = File.ReadAllBytes(file);
                File.Delete(file);
                return body;
            }
            catch (Exception ex)
            {
                log.Write(ex.Message + ex.StackTrace);
            }

            return null;
        }

        public override void SendMessage(string msg)
        {
            if (unuse) return;
            try
            {
                string fn = Path.Combine(comPath, string.Format("dmy_{0}.txt", DateTime.Now.Ticks));
                File.WriteAllText(fn, msg);
            }
            catch { }

            return;
        }

        public override void SetBytes(byte[] buf)
        {
            if (unuse) return;

            string fn = Path.Combine(comPath, "dmy.bin");
            File.WriteAllBytes(fn, buf);
            return;
        }



        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (lstFile.Contains(e.FullPath)) return;
            lstFile.Add(e.FullPath);
        }

    }
}
