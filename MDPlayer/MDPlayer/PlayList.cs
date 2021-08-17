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
            public EnmFileFormat format;
            public string playingNow;
            public string fileName;
            public string arcFileName;
            public EnmArcType arcType= EnmArcType.unknown;
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
                fullPath = Common.settingFilePath;
                fullPath = System.IO.Path.Combine(fullPath, "DefaultPlayList.xml");
            }
            else
            {
                fullPath = fileName;
            }

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PlayList), typeof(PlayList).GetNestedTypes());
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
                    fullPath = Common.settingFilePath;
                    fullPath = System.IO.Path.Combine(fullPath, "DefaultPlayList.xml");
                }
                else
                {
                    fullPath = fileName;
                }

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PlayList), typeof(PlayList).GetNestedTypes());
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

        public List<DataGridViewRow> makeRow(List<PlayList.music> musics)
        {
            List<DataGridViewRow> ret = new List<DataGridViewRow>();

            foreach (PlayList.music music in musics)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvList);
                row.Cells[dgvList.Columns["clmPlayingNow"].Index].Value = " ";
                row.Cells[dgvList.Columns["clmKey"].Index].Value = 0;
                row.Cells[dgvList.Columns["clmFileName"].Index].Value = music.fileName;
                row.Cells[dgvList.Columns["clmZipFileName"].Index].Value = music.arcFileName;
                row.Cells[dgvList.Columns["clmDispFileName"].Index].Value = Path.GetFileName(music.fileName);
                row.Cells[dgvList.Columns["clmDispFileName"].Index].ToolTipText = music.fileName;
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
                row.Tag = music;

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
            try
            {
                music mc = new music();
                mc.format = Common.CheckExt(filename);
                mc.fileName = filename;
                rootPath = Path.GetDirectoryName(filename);

                AddFileLoop(mc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ファイル追加に失敗しました。\r\n詳細\r\nMessage={0}", ex.Message)
                    , "エラー"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
                log.ForcedWrite(ex);
            }
        }

        public void InsertFile(ref int index,string[] filenames)
        {
            try
            {
                for (int i = 0; i < filenames.Length; i++)
                {
                    string filename = filenames[i];

                    music mc = new music();
                    mc.format = Common.CheckExt(filename);
                    mc.fileName = filename;
                    rootPath = Path.GetDirectoryName(filename);

                    AddFileLoop(ref index, mc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ファイル追加に失敗しました。\r\n詳細\r\nMessage={0}", ex.Message)
                    , "エラー"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
                log.ForcedWrite(ex);
            }
        }

        private void AddFileLoop(music mc, object entry = null)
        {
            switch (mc.format)
            {
                case EnmFileFormat.unknown:
                    break;
                case EnmFileFormat.M3U:
                    AddFileM3U(mc, entry);
                    break;
                case EnmFileFormat.MID:
                    AddFileMID(mc, entry);
                    break;
                case EnmFileFormat.NRT:
                    AddFileNRT(mc, entry);
                    break;
                case EnmFileFormat.NSF:
                    AddFileNSF(mc, entry);
                    break;
                case EnmFileFormat.HES:
                    AddFileHES(mc, entry);
                    break;
                case EnmFileFormat.SID:
                    AddFileSID(mc, entry);
                    break;
                case EnmFileFormat.MDR:
                    AddFileMDR(mc, entry);
                    break;
                case EnmFileFormat.MND:
                    AddFileMND(mc, entry);
                    break;
                case EnmFileFormat.MDX:
                    AddFileMDX(mc, entry);
                    break;
                case EnmFileFormat.MUB:
                    AddFileMUB(mc, entry);
                    break;
                case EnmFileFormat.MUC:
                    AddFileMUC(mc, entry);
                    break;
                case EnmFileFormat.MML:
                    AddFileMML(mc, entry);
                    break;
                case EnmFileFormat.MGS:
                    AddFileMML(mc, entry);
                    break;
                case EnmFileFormat.M:
                    AddFileM(mc, entry);
                    break;
                case EnmFileFormat.RCP:
                    AddFileRCP(mc, entry);
                    break;
                case EnmFileFormat.S98:
                    AddFileS98(mc, entry);
                    break;
                case EnmFileFormat.VGM:
                    AddFileVGM(mc, entry);
                    break;
                case EnmFileFormat.XGM:
                    AddFileXGM(mc, entry);
                    break;
                case EnmFileFormat.ZGM:
                    AddFileZGM(mc, entry);
                    break;
                case EnmFileFormat.ZIP:
                    AddFileZIP(mc, entry);
                    break;
                case EnmFileFormat.LZH:
                    AddFileLZH(mc, entry);
                    break;
                case EnmFileFormat.WAV:
                    AddFileWAV(mc, entry);
                    break;
                case EnmFileFormat.MP3:
                    AddFileMP3(mc, entry);
                    break;
                case EnmFileFormat.AIFF:
                    AddFileAIFF(mc, entry);
                    break;
            }
        }

        private void AddFileLoop(ref int index, music mc, object entry = null)
        {
            switch (mc.format)
            {
                case EnmFileFormat.unknown:
                    break;
                case EnmFileFormat.MID:
                    AddFileMID(ref index, mc, entry);
                    break;
                case EnmFileFormat.NRT:
                    AddFileNRT(ref index, mc, entry);
                    break;
                case EnmFileFormat.MDR:
                    AddFileMDR(ref index, mc, entry);
                    break;
                case EnmFileFormat.MND:
                    AddFileMND(ref index, mc, entry);
                    break;
                case EnmFileFormat.MDX:
                    AddFileMDX(ref index, mc, entry);
                    break;
                case EnmFileFormat.MUB:
                    AddFileMUB(ref index, mc, entry);
                    break;
                case EnmFileFormat.MUC:
                    AddFileMUC(ref index, mc, entry);
                    break;
                case EnmFileFormat.MML:
                    AddFileMML(ref index, mc, entry);
                    break;
                case EnmFileFormat.MGS:
                    AddFileMGS(ref index, mc, entry);
                    break;
                case EnmFileFormat.M:
                    AddFileM(ref index, mc, entry);
                    break;
                case EnmFileFormat.RCP:
                    AddFileRCP(ref index, mc, entry);
                    break;
                case EnmFileFormat.S98:
                    AddFileS98(ref index, mc, entry);
                    break;
                case EnmFileFormat.VGM:
                    AddFileVGM(ref index, mc, entry);
                    break;
                case EnmFileFormat.XGM:
                    AddFileXGM(ref index, mc, entry);
                    break;
                case EnmFileFormat.ZGM:
                    AddFileZGM(ref index, mc, entry);
                    break;
                case EnmFileFormat.WAV:
                    AddFileWAV(ref index, mc, entry);
                    break;
                case EnmFileFormat.MP3:
                    AddFileMP3(ref index, mc, entry);
                    break;
                case EnmFileFormat.AIFF:
                    AddFileAIFF(ref index, mc, entry);
                    break;
                case EnmFileFormat.ZIP:
                    AddFileZIP(ref index, mc, entry);
                    break;
                case EnmFileFormat.LZH:
                    AddFileLZH(ref index, mc, entry);
                    break;
                case EnmFileFormat.M3U:
                    AddFileM3U(ref index, mc, entry);
                    break;
                case EnmFileFormat.NSF:
                    AddFileNSF(ref index, mc, entry);
                    break;
                case EnmFileFormat.HES:
                    AddFileHES(ref index, mc, entry);
                    break;
                case EnmFileFormat.SID:
                    AddFileSID(ref index, mc, entry);
                    break;
            }
        }

        /// <summary>
        /// 汎用
        /// </summary>
        private void AddFilexxx(music mc, object entry=null)
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
                    if (buf == null && mc.format == EnmFileFormat.VGM)
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
                    if (entry is ZipArchiveEntry)
                    {
                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            try
                            {
                                buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
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
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);
                List<DataGridViewRow> rows = makeRow(musics);

                dgvList.Rows.AddRange(rows.ToArray());
                lstMusic.AddRange(musics);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFilexxx(ref int index,music mc, object entry = null)
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
                    if (buf == null && mc.format == EnmFileFormat.VGM)
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
                    if (entry is ZipArchiveEntry)
                    {
                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            try
                            {
                                buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
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
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);
                List<DataGridViewRow> rows = makeRow(musics);

                dgvList.Rows.InsertRange(index, rows.ToArray());
                lstMusic.InsertRange(index, musics);
                index += rows.Count;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFileMID(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMID(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileNRT(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileNRT(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileRCP(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileRCP(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileS98(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileS98(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileVGM(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileVGM(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileXGM(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileXGM(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileZGM(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileZGM(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMDR(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMDR(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMND(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMND(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMDX(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMDX(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMUB(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMUB(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMUC(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMUC(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMML(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMML(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileM(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileM(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMGS(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMGS(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileWAV(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileWAV(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileMP3(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileMP3(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }

        private void AddFileAIFF(music mc, object entry = null)
        {
            AddFilexxx(mc, entry);
        }

        private void AddFileAIFF(ref int index, music mc, object entry = null)
        {
            AddFilexxx(ref index, mc, entry);
        }



        private void AddFileZIP(music mc, object entry = null)
        {
            if (entry != null) return;

            using (ZipArchive archive = ZipFile.OpenRead(mc.fileName))
            {
                mc.arcFileName = mc.fileName;
                mc.arcType = EnmArcType.ZIP;
                List<string> zipMember = new List<string>();
                List<music> mMember = new List<music>();
                foreach (ZipArchiveEntry ent in archive.Entries)
                {
                    if (Common.CheckExt(ent.FullName) != EnmFileFormat.M3U)
                    {
                        zipMember.Add(ent.FullName);
                    }
                    else
                    {
                        PlayList pl = M3U.LoadM3U(ent, mc.arcFileName);
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
                    if (!found && Common.CheckExt(zm) == EnmFileFormat.VGM)
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
                        zmc.arcFileName = mc.arcFileName;
                        zmc.arcType = mc.arcType;
                        mMember.Add(zmc);
                    }
                }

                List<music> tMember = new List<music>();

                foreach (ZipArchiveEntry ent in archive.Entries)
                {
                    foreach (music m in mMember)
                    {
                        string vzm = "";
                        if (Path.GetExtension(m.fileName).ToLower() == ".vgm") vzm = Path.ChangeExtension(m.fileName, ".vgz");
                        else if (Path.GetExtension(m.fileName).ToLower() == ".vgz") vzm = Path.ChangeExtension(m.fileName, ".vgm");

                        if (ent.FullName == m.fileName || ent.FullName == vzm)
                        {
                            m.format = Common.CheckExt(m.fileName);
                            m.arcFileName = mc.arcFileName;
                            m.arcType = mc.arcType;
                            AddFileLoop(m, ent);

                            //m3uが複数同梱されている時、同名のファイルが多数追加されることになるケースがある。
                            //それを防ぐためここでbreakする
                            break;

                        }
                    }

                }

            }
        }

        private void AddFileZIP(ref int index, music mc, object entry = null)
        {
            if (entry != null) return;

            using (ZipArchive archive = ZipFile.OpenRead(mc.fileName))
            {
                mc.arcFileName = mc.fileName;
                mc.arcType = EnmArcType.ZIP;
                List<string> zipMember = new List<string>();
                List<music> mMember = new List<music>();
                foreach (ZipArchiveEntry ent in archive.Entries)
                {
                    if (Common.CheckExt(ent.FullName) != EnmFileFormat.M3U)
                    {
                        zipMember.Add(ent.FullName);
                    }
                    else
                    {
                        PlayList pl = M3U.LoadM3U(ent, mc.arcFileName);
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
                    if (!found && Common.CheckExt(zm) == EnmFileFormat.VGM)
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
                        zmc.arcFileName = mc.arcFileName;
                        zmc.arcType = mc.arcType;
                        mMember.Add(zmc);
                    }
                }

                List<music> tMember = new List<music>();

                foreach (ZipArchiveEntry ent in archive.Entries)
                {
                    foreach (music m in mMember)
                    {
                        string vzm = "";
                        if (Path.GetExtension(m.fileName).ToLower() == ".vgm") vzm = Path.ChangeExtension(m.fileName, ".vgz");
                        else if (Path.GetExtension(m.fileName).ToLower() == ".vgz") vzm = Path.ChangeExtension(m.fileName, ".vgm");

                        if (ent.FullName == m.fileName || ent.FullName == vzm)
                        {
                            m.format = Common.CheckExt(m.fileName);
                            m.arcFileName = mc.arcFileName;
                            m.arcType = mc.arcType;
                            AddFileLoop(ref index, m, ent);

                            //m3uが複数同梱されている時、同名のファイルが多数追加されることになるケースがある。
                            //それを防ぐためここでbreakする
                            break;

                        }
                    }

                }

            }
        }

        private void AddFileLZH(music mc, object entry = null)
        {
            if (entry != null) return;

            UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
            List<Tuple<string, UInt64>> res = cmd.GetFileList(mc.fileName, "*.*");
            mc.arcFileName = mc.fileName;
            mc.arcType = EnmArcType.LZH;
            List<string> zipMember = new List<string>();
            List<music> mMember = new List<music>();

            foreach (Tuple<string, UInt64> ent in res)
            {
                if (Common.CheckExt(ent.Item1) != EnmFileFormat.M3U)
                {
                    zipMember.Add(ent.Item1);
                }
                else
                {
                    PlayList pl = M3U.LoadM3U(ent, mc.arcFileName);
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
                if (!found && Common.CheckExt(zm) == EnmFileFormat.VGM)
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
                    zmc.arcFileName = mc.arcFileName;
                    zmc.arcType = mc.arcType;
                    mMember.Add(zmc);
                }
            }

            foreach (Tuple<string, UInt64> ent in res)
            {
                foreach (music m in mMember)
                {
                    string vzm = "";
                    if (Path.GetExtension(m.fileName).ToLower() == ".vgm") vzm = Path.ChangeExtension(m.fileName, ".vgz");
                    else if (Path.GetExtension(m.fileName).ToLower() == ".vgz") vzm = Path.ChangeExtension(m.fileName, ".vgm");

                    if (ent.Item1 == m.fileName || ent.Item1 == vzm)
                    {
                        m.format = Common.CheckExt(m.fileName);
                        m.arcFileName = mc.arcFileName;
                        m.arcType = mc.arcType;
                        AddFileLoop(m, new Tuple<string, string>(m.arcFileName, ent.Item1));

                        //m3uが複数同梱されている時、同名のファイルが多数追加されることになるケースがある。
                        //それを防ぐためここでbreakする
                        break;
                    }
                }
            }

        }

        private void AddFileLZH(ref int index, music mc, object entry = null)
        {
            if (entry != null) return;

            UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
            List<Tuple<string, UInt64>> res = cmd.GetFileList(mc.fileName, "*.*");
            mc.arcFileName = mc.fileName;
            mc.arcType = EnmArcType.LZH;
            List<string> zipMember = new List<string>();
            List<music> mMember = new List<music>();

            foreach (Tuple<string, UInt64> ent in res)
            {
                if (Common.CheckExt(ent.Item1) != EnmFileFormat.M3U)
                {
                    zipMember.Add(ent.Item1);
                }
                else
                {
                    PlayList pl = M3U.LoadM3U(ent, mc.arcFileName);
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
                if (!found && Common.CheckExt(zm) == EnmFileFormat.VGM)
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
                    zmc.arcFileName = mc.arcFileName;
                    zmc.arcType = mc.arcType;
                    mMember.Add(zmc);
                }
            }

            foreach (Tuple<string, UInt64> ent in res)
            {
                foreach (music m in mMember)
                {
                    string vzm = "";
                    if (Path.GetExtension(m.fileName).ToLower() == ".vgm") vzm = Path.ChangeExtension(m.fileName, ".vgz");
                    else if (Path.GetExtension(m.fileName).ToLower() == ".vgz") vzm = Path.ChangeExtension(m.fileName, ".vgm");

                    if (ent.Item1 == m.fileName || ent.Item1 == vzm)
                    {
                        m.format = Common.CheckExt(m.fileName);
                        m.arcFileName = mc.arcFileName;
                        m.arcType = mc.arcType;
                        AddFileLoop(ref index, m, new Tuple<string, string>(m.arcFileName, ent.Item1));

                        //m3uが複数同梱されている時、同名のファイルが多数追加されることになるケースがある。
                        //それを防ぐためここでbreakする
                        break;
                    }
                }
            }

        }

        private void AddFileM3U(music mc, object entry = null)
        {

            PlayList pl;
            if (entry == null) pl = M3U.LoadM3U(mc.fileName, rootPath);
            else pl = M3U.LoadM3U(entry, mc.arcFileName);
            if (pl == null) return;
            if (pl.lstMusic == null || pl.lstMusic.Count < 1) return;

            foreach (music m in pl.lstMusic) AddFileLoop(m, entry);
        }

        private void AddFileM3U(ref int index, music mc, object entry = null)
        {

            PlayList pl;
            if (entry == null) pl = M3U.LoadM3U(mc.fileName, rootPath);
            else pl = M3U.LoadM3U(entry, mc.arcFileName);
            if (pl == null) return;
            if (pl.lstMusic == null || pl.lstMusic.Count < 1) return;

            foreach (music m in pl.lstMusic) AddFileLoop(ref index, m, entry);
        }

        private void AddFileNSF(music mc, object entry = null)
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
                    if (entry is ZipArchiveEntry)
                    {

                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
                        }
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }

                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);

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

        private void AddFileNSF(ref int index, music mc, object entry = null)
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
                    if (entry is ZipArchiveEntry)
                    {

                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
                        }
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }

                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);

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
                dgvList.Rows.InsertRange(index, rows.ToArray());
                lstMusic.InsertRange(index,musics);
                index += rows.Count;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFileHES(music mc, object entry = null)
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
                    if (entry is ZipArchiveEntry)
                    {

                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
                        }
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);

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

        private void AddFileHES(ref int index, music mc, object entry = null)
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
                    if (entry is ZipArchiveEntry)
                    {

                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
                        }
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);

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
                dgvList.Rows.InsertRange(index, rows.ToArray());
                lstMusic.InsertRange(index, musics);
                index += rows.Count;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void AddFileSID(music mc, object entry = null)
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
                    if (entry is ZipArchiveEntry)
                    {

                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
                        }
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);

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

        private void AddFileSID(ref int index, music mc, object entry = null)
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
                    if (entry is ZipArchiveEntry)
                    {

                        using (BinaryReader reader = new BinaryReader(((ZipArchiveEntry)entry).Open()))
                        {
                            buf = reader.ReadBytes((int)((ZipArchiveEntry)entry).Length);
                        }
                    }
                    else
                    {
                        UnlhaWrap.UnlhaCmd cmd = new UnlhaWrap.UnlhaCmd();
                        buf = cmd.GetFileByte(((Tuple<string, string>)entry).Item1, ((Tuple<string, string>)entry).Item2);
                    }
                }

                List<PlayList.music> musics;
                if (entry == null) musics = Audio.getMusic(mc.fileName, buf);
                else musics = Audio.getMusic(mc.fileName, buf, mc.arcFileName, entry);

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
                dgvList.Rows.InsertRange(index, rows.ToArray());
                lstMusic.InsertRange(index, musics);
                index += rows.Count;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }
    }
}
