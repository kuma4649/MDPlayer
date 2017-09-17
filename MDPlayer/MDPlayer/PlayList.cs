using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    [Serializable]
    public class PlayList
    {
        public class music
        {
            public enmFileFormat format;
            public string playingNow;
            public string fileName;
            public string zipFileName;
            public string type="-";

            public string title;
            public string game;
            public string system;
            public string composer;
            public string titleJ;
            public string gameJ;
            public string systemJ;
            public string composerJ;

            public string converted;
            public string notes;
            public string vgmby;
            public string remark;
            public string duration;

            public int songNo;
        }

        private List<music> _lstMusic = new List<music>();
        public List<music> lstMusic
        {
            get
            {
                return _lstMusic;
            }

            set
            {
                _lstMusic = value;
            }
        }

        public PlayList Copy()
        {
            PlayList playList = new PlayList();

            return playList;
        }

        public void Save(string fileName)
        {
            string fullPath = "";

            if (fileName == null || fileName == "")
            {
                fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                fullPath = System.IO.Path.Combine(fullPath, "KumaApp", common.AssemblyTitle);
                if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                fullPath = System.IO.Path.Combine(fullPath, "DefaultPlayList.xml");
            }
            else
            {
                fullPath = fileName;
            }

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PlayList));
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, this);
            }
        }

        public void SaveM3U(string fileName)
        {
            string basePath = Path.GetDirectoryName(fileName);

            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(932)))
            {
                foreach (PlayList.music ms in this.lstMusic)
                {
                    string path = Path.GetDirectoryName(ms.fileName);
                    if (path == basePath)
                    {
                        sw.WriteLine(Path.GetFileName(ms.fileName));
                    }
                    else
                    {
                        sw.WriteLine(ms.fileName);
                    }
                }
            }
        }

        public static PlayList Load(string fileName)
        {
            try
            {
                string fullPath = "";
                if (fileName == null || fileName == "")
                {
                    fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = System.IO.Path.Combine(fullPath, "KumaApp", common.AssemblyTitle);
                    if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                    fullPath = System.IO.Path.Combine(fullPath, "DefaultPlayList.xml");
                }
                else
                {
                    fullPath = fileName;
                }

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PlayList));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fullPath, new UTF8Encoding(false)))
                {
                    PlayList pl = (PlayList)serializer.Deserialize(sr);
                    return pl;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new PlayList();
            }
        }

        public static PlayList LoadM3U(string filename)
        {
            try
            {
                PlayList pl = new PlayList();

                using (StreamReader sr = new StreamReader(filename,Encoding.GetEncoding(932)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line == "") continue;
                        if (line[0] == '#') continue;

                        if (!Path.IsPathRooted(line))
                        {
                            line = Path.Combine(Path.GetDirectoryName(filename), line);
                        }
                        music ms = new music();
                        ms.fileName = line;
                        pl.lstMusic.Add(ms);
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

    }
}
