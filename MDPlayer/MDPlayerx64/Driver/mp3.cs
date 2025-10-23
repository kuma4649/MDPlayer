using MDPlayer;
using MDPlayer.Driver.MuSICA;
using MDSound;
using NAudio.Midi;
using NAudio.Wave;
using NAudio.Wave.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDPlayer.PlayList;
using static MDPlayer.xgm;
using static MDSound.XGMFunction;

namespace MDPlayerx64.Driver
{
    public class mp3 : baseDriver
    {

        public System.Drawing.Image img = null;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();
            img = null;
            try
            {
                //さんこう ：
                //https://so-zou.jp/software/tech/programming/c-sharp/media/audio/naudio/#no2

                MemoryStream mStream = new MemoryStream(buf);
                Mp3FileReader r = new Mp3FileReader(mStream);
                byte[] v1 = r.Id3v1Tag;
                Encoding enc = Encoding.Default;
                string v1Title = null;
                string v1Artist = null;
                string v1Album = null;
                string v1Year = null;
                if (v1 != null)
                {
                    char[] trimChars = { '\0' };
                    v1Title = enc.GetString(v1, 3, 30).TrimEnd(trimChars);
                    v1Artist = enc.GetString(v1, 33, 30).TrimEnd(trimChars);
                    v1Album = enc.GetString(v1, 63, 30).TrimEnd(trimChars);
                    v1Year = enc.GetString(v1, 93, 4).TrimEnd(trimChars);
                }

                if (r.Id3v2Tag == null) return null;

                byte[] v2 = r.Id3v2Tag.RawData;

                if (v2[0] != 0x49 || v2[1] != 0x44 || v2[2] != 0x33)
                {
                    throw new Exception("Error ID3v2TAG Header.");
                }

                int v = v2[3];
                int flag = v2[5];
                uint size = (uint)((v2[6] << 21) + (v2[7] << 14) + (v2[8] << 7) + (v2[9] << 0));

                Dictionary<string, string> tags = new Dictionary<string, string>();


                if (v == 2)
                {
                    for (int index = 10; index < v2.Length;)
                    {
                        if (v2[index] == '\0') break;
                        string frameID = Encoding.ASCII.GetString(v2, index, 3);
                        index += 3;
                        int frameSize = (int)((v2[index] << 16) + (v2[index + 1] << 8) + v2[index + 2]);
                        index += 3;

                        if (frameID == "PIC")
                        {
                            byte[] pic = new byte[frameSize-6];
                            Array.Copy(v2, index + 6, pic, 0, frameSize - 6);
                            //byte[] pic = new byte[frameSize];
                            //Array.Copy(v2, index, pic, 0, frameSize);
                            index += frameSize;
                            MemoryStream mb = new MemoryStream(pic);
                            img = System.Drawing.Image.FromStream(mb);
                            continue;
                        }

                        byte[] v3 = new byte[frameSize - 1];
                        Array.Copy(v2, index + 1, v3, 0, frameSize - 1);
                        Encoding enc2 = Common.GetCode(v3);

                        enc = Encoding.Default;// Unicode; // UTF-16LE
                        byte c = v2[index++];
                        switch (c)
                        {
                            case 0x00:
                                enc = Encoding.GetEncoding(28591); // iso-8859-1
                                if (enc2 != null) enc = enc2;
                                break;

                            case 0x01:
                                enc = Encoding.GetEncoding("UTF-16");//.BigEndianUnicode; // UTF-16BE
                                if (enc2 != null) enc = enc2;
                                break;
                        }

                        string content = enc.GetString(v2, index, frameSize - 1);
                        index += frameSize - 1;

                        if (!tags.ContainsKey(frameID)) tags.Add(frameID, content);
                    }

                    if (tags.ContainsKey("TT2"))
                    {
                        string title = tags["TT2"];
                        GD3.TrackName = string.IsNullOrEmpty(title)
                            ? (
                                string.IsNullOrEmpty(v1Title)
                                ? "" : v1Title
                        )
                            : title;
                        GD3.TrackNameJ = GD3.TrackName;
                    }

                    if (tags.ContainsKey("TP1"))
                    {
                        string artist = tags["TP1"];
                        GD3.Composer = string.IsNullOrEmpty(artist)
                            ? (
                                string.IsNullOrEmpty(v1Artist)
                                ? "" : v1Artist
                        )
                            : artist;
                        GD3.ComposerJ = GD3.Composer;
                    }

                    if (tags.ContainsKey("TAL"))
                    {
                        string album = tags["TAL"];
                        GD3.GameName = string.IsNullOrEmpty(album)
                            ? (
                                string.IsNullOrEmpty(v1Album)
                                ? "" : v1Album
                                )
                            : album;
                        GD3.GameNameJ = GD3.GameName;
                    }
                }
                else
                {
                    for (int index = 10; index < v2.Length;)
                    {
                        if (index < 10) break;
                        if (v2[index] == '\0') break;

                        string frameID = Encoding.ASCII.GetString(v2, index, 4);
                        index += 4;

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(v2, index, 4);
                        }
                        int frameSize = BitConverter.ToInt32(v2, index);
                        index += 4;

                        byte flag1 = v2[index++];
                        byte flag2 = v2[index++];

                        if (frameID == "APIC")
                        {
                            ASCIIEncoding asc = new ASCIIEncoding();
                            string mime=asc.GetString(v2, index+1, 10).Trim().ToLower();
                            if (mime == "image/jpeg")
                            {
                                index += 11;
                                frameSize -= 11;
                                while (index + 3 < v2.Length
                                    && (v2[index] != 0xff || v2[index + 1] != 0xd8 || v2[index + 2] != 0xff || v2[index + 3] != 0xe0)
                                    && frameSize > 4)
                                {
                                    index++;
                                    frameSize--;
                                }
                                byte[] pic = new byte[frameSize];
                                Array.Copy(v2, index, pic, 0, frameSize);
                                index += frameSize;
                                try
                                {
                                    MemoryStream mb = new MemoryStream(pic);
                                    img = System.Drawing.Image.FromStream(mb);
                                }
                                catch
                                {
                                    img=null;
                                }
                            }
                            if(mime.IndexOf("image/png")==0)
                            {
                                index += 10;
                                frameSize -= 10;
                                while (index + 7 < v2.Length 
                                    && (v2[index] != 0x89 || v2[index + 1] != 0x50 || v2[index + 2] != 0x4e || v2[index + 3] != 0x47 
                                        || v2[index + 4] != 0x0d || v2[index + 5] != 0x0a || v2[index + 6] != 0x1a || v2[index + 7] != 0x0a) 
                                    && frameSize > 8)
                                {
                                    index++;
                                    frameSize--;
                                }
                                byte[] pic = new byte[frameSize];
                                Array.Copy(v2, index, pic, 0, frameSize);
                                index += frameSize;
                                try
                                {
                                    MemoryStream mb = new MemoryStream(pic);
                                    img = System.Drawing.Image.FromStream(mb);
                                }
                                catch
                                {
                                    img = null;
                                }
                            }
                            else
                            {
                                index += frameSize;
                            }
                        }
                        else if (frameID[0] == 'T')
                        {
                            byte[] v3 = new byte[frameSize - 1];
                            Array.Copy(v2, index + 1, v3, 0, frameSize - 1);
                            Encoding enc2 = Common.GetCode(v3);

                            enc = Encoding.Default;// Unicode; // UTF-16LE
                            byte c = v2[index++];
                            switch (c)
                            {
                                case 0x00:
                                    enc = Encoding.GetEncoding(28591); // iso-8859-1
                                    if (enc2 != null) enc = enc2;
                                    break;

                                case 0x01:
                                    if (index + 1 < v2.Length && (v2[index] == 0xfe && v2[index + 1] == 0xff))
                                    {
                                        enc = Encoding.BigEndianUnicode; // UTF-16BE
                                    }
                                    else if (enc2 != null) enc = enc2;
                                    break;
                            }

                            string content = enc.GetString(v2, index, frameSize - 1);
                            index += frameSize - 1;

                            if (!tags.ContainsKey(frameID)) tags.Add(frameID, content);
                        }
                        else
                        {
                            index += frameSize;
                        }
                    }

                    if (tags.ContainsKey("TIT2"))
                    {
                        string title = tags["TIT2"];
                        GD3.TrackName = string.IsNullOrEmpty(title)
                            ? (
                                string.IsNullOrEmpty(v1Title)
                                ? "" : v1Title
                        )
                            : title;
                        GD3.TrackNameJ = GD3.TrackName;
                    }

                    if (tags.ContainsKey("TPE1"))
                    {
                        string artist = tags["TPE1"];
                        GD3.Composer = string.IsNullOrEmpty(artist)
                            ? (
                                string.IsNullOrEmpty(v1Artist)
                                ? "" : v1Artist
                        )
                            : artist;
                        GD3.ComposerJ = GD3.Composer;
                    }

                    if (tags.ContainsKey("TALB"))
                    {
                        string album = tags["TALB"];
                        GD3.GameName = string.IsNullOrEmpty(album)
                            ? (
                                string.IsNullOrEmpty(v1Album)
                                ? "" : v1Album
                                )
                            : album;
                        GD3.GameNameJ = GD3.GameName;
                    }
                }

                GD3.pic = img;
            }
            catch
            {
                GD3.TrackName = "";
            }

            return GD3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            throw new NotImplementedException();
        }
    }
}