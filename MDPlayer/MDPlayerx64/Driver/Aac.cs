using MDPlayer;
using MDPlayer.Driver.ZMS.nise68;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MDPlayerx64.Driver
{
    public class Aac : baseDriver
    {

        public System.Drawing.Image img = null;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();
            if (buf[0] == 0xff || buf[1] == 0xf0)
            {
                //ADTS形式は未サポート
                return ret;
            }

            int read = buf.Length;
            if (read < 12)
            {
                //ファイルサイズが不十分
                return ret;
            }

            // "ftyp" は通常 4バイト目から始まる
            string boxType = Encoding.ASCII.GetString(buf, 4, 4);
            string majorBrand = Encoding.ASCII.GetString(buf, 8, 4);

            if (boxType != "ftyp" || (majorBrand != "M4A " && majorBrand != "isom" && majorBrand != "mp42"))
            {
                //M4A形式ではない
                return ret;
            }

            using var fs = new MemoryStream(buf);
            var root = ParseBoxes(fs, 0, fs.Length);

            // メタデータ探索
            var ilst = FindBox(root, "ilst");
            if (ilst == null)
            {
                //メタデータが見つかりませんでした
                return ret;
            }

            foreach (var box in ilst.Children)
            {
                string tagType = box.Type;
                if (tagType == "covr")
                {
                    byte[] imageBytes = ReadCovrBytes(fs, box);

                    using (MemoryStream pms = new MemoryStream(imageBytes))
                        ret.pic = Image.FromStream(pms);
                }
                else if (tagType == "?nam")
                {
                    string value = ReadMetadataValue(fs, box);
                    ret.TrackName = ret.TrackNameJ = value;
                }
                else if (tagType == "?ART")
                {
                    string value = ReadMetadataValue(fs, box);
                    ret.Composer = ret.ComposerJ = value;
                }
                else if (tagType == "?alb")
                {
                    string value = ReadMetadataValue(fs, box);
                    ret.GameName = ret.GameNameJ = value;
                }
                else
                {
                    string value = ReadMetadataValue(fs, box);
                    Debug.WriteLine($"{tagType}: {value}\r\n");
                }
            }

            return ret;
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

        static Mp4Box ParseBoxes(Stream stream, long start, long end)
        {
            var container = new Mp4Box { Start = start, Size = end - start, Type = "root" };

            stream.Position = start;
            while (stream.Position < end)
            {
                long boxStart = stream.Position;
                byte[] sizeBuf = new byte[4];
                byte[] typeBuf = new byte[4];
                if (stream.Read(sizeBuf, 0, 4) < 4 || stream.Read(typeBuf, 0, 4) < 4) break;

                uint size = BitConverter.ToUInt32(sizeBuf.Reverse().ToArray(), 0);
                string type = Encoding.ASCII.GetString(typeBuf);

                if (size < 8 || (boxStart + size) > end) break;

                var box = new Mp4Box { Start = boxStart, Size = size, Type = type };

                long contentStart = boxStart + 8;
                if (type == "meta")
                {
                    // meta ボックスは最初の8バイト（version + flags）を読み飛ばす
                    contentStart += 4;
                }

                if (IsContainer(type))
                    box.Children = ParseBoxes(stream, contentStart, boxStart + size).Children;

                container.Children.Add(box);
                stream.Position = boxStart + size;
            }

            return container;
        }

        class Mp4Box
        {
            public long Start { get; set; }
            public long Size { get; set; }
            public string Type { get; set; }
            public List<Mp4Box> Children { get; set; } = new List<Mp4Box>();
        }

        private static bool IsContainer(string type) =>
            new[] { "moov", "udta", "meta", "ilst" }.Contains(type);

        private static Mp4Box FindBox(Mp4Box root, string target)
        {
            if (root.Type == target) return root;
            foreach (var child in root.Children)
            {
                var found = FindBox(child, target);
                if (found != null) return found;
            }
            return null;
        }

        static string ReadMetadataValue(Stream stream, Mp4Box box)
        {
            stream.Position = box.Start + 8; // skip header
            byte[] buffer = new byte[box.Size - 8];
            stream.Read(buffer, 0, buffer.Length);

            // 典型的にはデータは16バイト目以降（通常UTF-8）
            return Encoding.UTF8.GetString(buffer.Skip(16).ToArray()).TrimEnd('\0');
        }

        static byte[] ReadCovrBytes(Stream stream, Mp4Box box)
        {
            stream.Position = box.Start + 8; // skip box header
            byte[] buffer = new byte[box.Size - 8];
            stream.Read(buffer, 0, buffer.Length);

            // `covr` ボックスは以下の構造になることが多い：
            // [data box] → version + flags + reserved + type + image data
            // 実際のデータは20バイト目以降にあることが多い
            const int headerSize = 16; // version(1) + flags(3) + reserved(4) + type(4) + misc
            if (buffer.Length <= headerSize) return Array.Empty<byte>();

            return buffer.Skip(headerSize).ToArray();
        }
    }
}


