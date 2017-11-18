using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            public string type = "-";

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

            public string time = "";
            public string loopStartTime = "";
            public string loopEndTime = "";
            public string fadeoutTime = "";
            public int loopCount = -1;

            public int songNo = -1;
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

                using (StreamReader sr = new StreamReader(filename, Encoding.GetEncoding(932)))
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

        private List<DataGridViewRow> makeRow(List<PlayList.music> musics)
        {
            List<DataGridViewRow> ret = new List<DataGridViewRow>();

            foreach (PlayList.music music in musics)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvList);
                row.Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
                row.Cells[dgvList.Columns["clmKey"].Index].Value = 0;
                row.Cells[dgvList.Columns["clmFileName"].Index].Value = music.fileName;
                row.Cells[dgvList.Columns["clmZipFileName"].Index].Value = music.zipFileName;
                row.Cells[dgvList.Columns["clmEXT"].Index].Value = Path.GetExtension(music.fileName).ToUpper();
                row.Cells[dgvList.Columns["clmType"].Index].Value = music.type;
                row.Cells[dgvList.Columns["clmTitle"].Index].Value = music.title;
                row.Cells[dgvList.Columns["clmTitleJ"].Index].Value = music.titleJ;
                row.Cells[dgvList.Columns["clmGame"].Index].Value = music.game;
                row.Cells[dgvList.Columns["clmGameJ"].Index].Value = music.gameJ;
                //row.Cells[dgvList.Columns["clmRemark"].Index].Value = music.remark;
                row.Cells[dgvList.Columns["clmComposer"].Index].Value = music.composer;
                row.Cells[dgvList.Columns["clmComposerJ"].Index].Value = music.composerJ;
                row.Cells[dgvList.Columns["clmConverted"].Index].Value = music.converted;
                row.Cells[dgvList.Columns["clmNotes"].Index].Value = music.notes;
                row.Cells[dgvList.Columns["clmDuration"].Index].Value = music.duration;
                row.Cells[dgvList.Columns["clmVGMby"].Index].Value = music.vgmby;
                row.Cells[dgvList.Columns["clmSongNo"].Index].Value = music.songNo;

                ret.Add(row);
            }
            return ret;
        }

        private string rootPath = "";
        private DataGridView dgvList;

        public void SetDGV(DataGridView dgv)
        {
            dgvList = dgv;
        }

        public void AddFile(string filename)
        {
            music mc = new music();
            mc.format = common.CheckExt(filename);
            mc.fileName = filename;
            rootPath = Path.GetDirectoryName(filename);

            AddFileLoop(mc);
        }

        private void AddFileLoop(music mc, ZipArchiveEntry entry = null)
        {
            switch (mc.format)
            {
                case enmFileFormat.unknown:
                    break;
                case enmFileFormat.M3U:
                    AddFileM3U(mc, entry);
                    break;
                case enmFileFormat.MID:
                    AddFileMID(mc, entry);
                    break;
                case enmFileFormat.NRT:
                    AddFileNRT(mc, entry);
                    break;
                case enmFileFormat.NSF:
                    AddFileNSF(mc, entry);
                    break;
                case enmFileFormat.HES:
                    AddFileHES(mc, entry);
                    break;
                case enmFileFormat.RCP:
                    AddFileRCP(mc, entry);
                    break;
                case enmFileFormat.S98:
                    AddFileS98(mc, entry);
                    break;
                case enmFileFormat.VGM:
                    AddFileVGM(mc, entry);
                    break;
                case enmFileFormat.XGM:
                    AddFileXGM(mc, entry);
                    break;
                case enmFileFormat.ZIP:
                    AddFileZIP(mc, entry);
                    break;
            }
        }

        /// <summary>
        /// 汎用
        /// </summary>
        private void AddFilexxx(music mc, ZipArchiveEntry entry=null)
        {
            try
            {
                byte[] buf = null;
                if (entry == null)
                {
                    try
                    {
                        buf = File.ReadAllBytes(mc.fileName);
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                        buf = null;
                    }
                    if (buf == null && mc.format == enmFileFormat.VGM)
                    {
                        if (Path.GetExtension(mc.fileName).ToLower() == ".vgm")
                        {
                            mc.fileName = Path.ChangeExtension(mc.fileName, ".vgz");
                        }
                        else
                        {
                            mc.fileName = Path.ChangeExtension(mc.fileName, ".vgm");
                        }
                        try
                        {
                            buf = File.ReadAllBytes(mc.fileName);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            buf = null;
                        }
                    }
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(entry.Open()))
                    {
                        try
                        {
                            buf = reader.ReadBytes((int)entry.Length);
                        }
                        catch (Exception ex)
                        {
                            log.ForcedWrite(ex);
                            buf = null;
                        }
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.zipFileName, entry);
                List<DataGridViewRow> rows = makeRow(musics);
                foreach (DataGridViewRow row in rows) dgvList.Rows.Add(row);
                foreach (PlayList.music music in musics) lstMusic.Add(music);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFileMID(music mc, ZipArchiveEntry entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileNRT(music mc, ZipArchiveEntry entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileRCP(music mc, ZipArchiveEntry entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileS98(music mc, ZipArchiveEntry entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileVGM(music mc, ZipArchiveEntry entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileXGM(music mc, ZipArchiveEntry entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileM3U(music mc, ZipArchiveEntry entry = null)
        {

            PlayList pl;
            if (entry == null) pl = M3U.LoadM3U(mc.fileName,rootPath);
            else pl = M3U.LoadM3U(entry, mc.zipFileName);
            if (pl == null) return;
            if (pl.lstMusic == null || pl.lstMusic.Count < 1) return;

            foreach (music m in pl.lstMusic) AddFileLoop(m, entry);
        }

        private void AddFileNSF(music mc, ZipArchiveEntry entry = null)
        {
            try
            {
                byte[] buf = null;
                if (entry == null)
                {
                    buf = File.ReadAllBytes(mc.fileName);
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(entry.Open()))
                    {
                        buf = reader.ReadBytes((int)entry.Length);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.zipFileName, entry);

                if (mc.songNo != -1)
                {
                    PlayList.music music = null;
                    if (musics.Count > 0)
                    {
                        music = musics[0];
                        music.songNo = mc.songNo;
                        music.title = mc.title;
                        music.titleJ = mc.titleJ;

                        musics.Clear();
                        musics.Add(music);
                    }
                    else
                    {
                        musics.Clear();
                    }
                }

                List<DataGridViewRow> rows = makeRow(musics);
                foreach (DataGridViewRow row in rows) dgvList.Rows.Add(row);
                foreach (PlayList.music music in musics) lstMusic.Add(music);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFileHES(music mc, ZipArchiveEntry entry = null)
        {
            try
            {
                byte[] buf = null;
                if (entry == null)
                {
                    buf = File.ReadAllBytes(mc.fileName);
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(entry.Open()))
                    {
                        buf = reader.ReadBytes((int)entry.Length);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.zipFileName, entry);

                if (mc.songNo != -1)
                {
                    PlayList.music music = null;
                    if (musics.Count > 0)
                    {
                        music = musics[0];
                        music.songNo = mc.songNo;
                        music.title = mc.title;
                        music.titleJ = mc.titleJ;

                        musics.Clear();
                        musics.Add(music);
                    }
                    else
                    {
                        musics.Clear();
                    }
                }

                List<DataGridViewRow> rows = makeRow(musics);
                foreach (DataGridViewRow row in rows) dgvList.Rows.Add(row);
                foreach (PlayList.music music in musics) lstMusic.Add(music);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFileZIP(music mc, ZipArchiveEntry entry = null)
        {
            if (entry != null) return;

            using (ZipArchive archive = ZipFile.OpenRead(mc.fileName))
            {
                mc.zipFileName = mc.fileName;
                List<string> zipMember = new List<string>();
                List<music> mMember = new List<music>();
                foreach (ZipArchiveEntry ent in archive.Entries)
                {
                    if (common.CheckExt(ent.FullName) != enmFileFormat.M3U)
                    {
                        zipMember.Add(ent.FullName);
                    }
                    else
                    {
                        PlayList pl = M3U.LoadM3U(ent, mc.zipFileName);
                        foreach (music m in pl.lstMusic) mMember.Add(m);
                    }
                }

                foreach (string zm in zipMember)
                {
                    bool found = false;
                    foreach (music m in mMember)
                    {
                        if (m.fileName == zm)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found && common.CheckExt(zm)== enmFileFormat.VGM)
                    {
                        string vzm = "";
                        if (Path.GetExtension(zm).ToLower() == ".vgm") vzm = Path.ChangeExtension(zm, ".vgz");
                        else vzm = Path.ChangeExtension(zm, ".vgm");
                        foreach (music m in mMember)
                        {
                            if (m.fileName == vzm)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        music zmc = new music();
                        zmc.fileName = zm;
                        zmc.zipFileName = mc.zipFileName;
                        mMember.Add(zmc);
                    }
                }

                foreach (ZipArchiveEntry ent in archive.Entries)
                {
                    foreach (music m in mMember)
                    {
                        string vzm = "";
                        if (Path.GetExtension(m.fileName).ToLower() == ".vgm") vzm = Path.ChangeExtension(m.fileName, ".vgz");
                        else if (Path.GetExtension(m.fileName).ToLower() == ".vgz") vzm = Path.ChangeExtension(m.fileName, ".vgm");

                        if (ent.FullName == m.fileName || ent.FullName == vzm)
                        {
                            m.format = common.CheckExt(m.fileName);
                            m.zipFileName = mc.zipFileName;
                            AddFileLoop(m, ent);
                        }
                    }
                }
            }
        }


    }
}
