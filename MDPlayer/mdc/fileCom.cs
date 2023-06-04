using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdc
{
    public class fileCom : KumaCom
    {
        private bool unuse = true;
        private string comPath = "";
        private FileSystemWatcher watcher = null;
        private List<string> lstFile = new List<string>();

        public fileCom(bool isClient, string appName, string mmfName, int mmfSize)
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

        ~fileCom()
        {
            Close();
        }

        public override bool Init(string appName, string pathName)
        {
            try
            {
                string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dataFolder = Path.Combine(dataFolder, "KumaApp", appName);
                if (!Directory.Exists(dataFolder)) System.IO.Directory.CreateDirectory(dataFolder);

                comPath = Path.Combine(dataFolder, pathName);
                if (!Directory.Exists(comPath))
                {
                    Directory.CreateDirectory(comPath);
                }

                string[] fs = Directory.GetFiles(comPath);
                foreach (string f in fs) File.Delete(f);

                watcher = new System.IO.FileSystemWatcher();
                watcher.Path = comPath;
                watcher.NotifyFilter =
                    (
                    System.IO.NotifyFilters.LastAccess
                    | System.IO.NotifyFilters.LastWrite
                    | System.IO.NotifyFilters.FileName
                    | System.IO.NotifyFilters.DirectoryName
                    );
                //watcher.Filter = Path.GetFileName();
                //watcher.SynchronizingObject = this;

                watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                watcher.Created += new FileSystemEventHandler(watcher_Changed);

                watcher.EnableRaisingEvents = true;


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



        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (lstFile.Contains(e.FullPath)) return;
            lstFile.Add(e.FullPath);
        }

    }
}
