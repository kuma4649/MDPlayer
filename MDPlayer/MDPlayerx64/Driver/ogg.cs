using MDPlayer;
using System.Text;

namespace MDPlayerx64.Driver
{
    public class Ogg : baseDriver
    {

        public System.Drawing.Image img = null;

        public long looplength { get { return _looplength; } set { _looplength = value; } }
        public long loopstart { get { return _loopstart; } set { _loopstart = value; } }
        private long _looplength;
        private long _loopstart;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();
            if (buf[0] != 0x4f || buf[1] != 0x67 || buf[2] != 0x67 || buf[3] != 0x53)
            {
                throw new Exception("Not OGG file.");
            }

            string[] tags = new string[] { "TITLE=", "ALBUM=", "ARTIST=", "LOOPLENGTH=", "LOOPSTART=" };
            List<string> cmt = new List<string>();

            using (MemoryStream ms = new MemoryStream(buf))
            using (var br = new BinaryReader(ms))
            {
                while (ms.Position < ms.Length)
                {
                    if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "OggS")
                    {
                        ms.Position -= 3; // 見落としを避けるため1バイトずつ前進
                        continue;
                    }

                    ms.Position += 22; // ページヘッダをスキップ
                    int segmentCount = br.ReadByte();
                    byte[] lacingValues = br.ReadBytes(segmentCount);

                    int payloadLength = 0;
                    foreach (byte b in lacingValues) payloadLength += b;

                    byte[] payload = br.ReadBytes(payloadLength);

                    // Vorbis Identification Headerをスキップ
                    if (payload.Length < 7 || payload[0] != 0x03 || Encoding.ASCII.GetString(payload, 1, 6) != "vorbis")
                        continue;

                    using (var ms1 = new MemoryStream(payload))
                    using (var r = new BinaryReader(ms1))
                    {
                        r.ReadByte(); // Packet Type (0x03)
                        r.ReadBytes(6); // "vorbis"

                        int vendorLen = r.ReadInt32();
                        r.ReadBytes(vendorLen); // Vendor string

                        int commentCount = r.ReadInt32();
                        for (int i = 0; i < commentCount; i++)
                        {
                            int length = r.ReadInt32();
                            string comment = Encoding.UTF8.GetString(r.ReadBytes(length));
                            cmt.Add(comment);
                        }
                    }
                }
            }

            looplength = -1;
            loopstart = -1;

            foreach(string c in cmt)
            {
                foreach(string tag in tags)
                {
                    if (c.IndexOf(tag) < 0) continue;

                    string val = c.Replace(tag, "");
                    if (tag == tags[0]) { GD3.TrackName = GD3.TrackNameJ = val; }
                    else if (tag == tags[1]) { GD3.GameName = GD3.GameNameJ = val; }
                    else if (tag == tags[2]) { GD3.Composer = GD3.ComposerJ = val; }
                    else if (tag == tags[3]) {
                        if (!long.TryParse(val, out _looplength))
                        {
                            _looplength = -1;
                        }
                        _looplength = Math.Max(looplength, -1);
                    }
                    else if (tag == tags[4]) {
                        if (!long.TryParse(val, out _loopstart))
                        {
                            _loopstart = -1;
                        }
                        _loopstart = Math.Max(_loopstart, -1);
                    }
                }
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


//using System;
//using System.IO;
//using System.Text;

//class VorbisImageExtractor
//{
//    public static void Main()
//    {
//        string filePath = @"C:\Music\example.ogg";
//        string outputImagePath = @"C:\Temp\cover.jpg";

//        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
//        using (var br = new BinaryReader(fs))
//        {
//            while (fs.Position < fs.Length)
//            {
//                if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "OggS")
//                {
//                    fs.Position -= 3;
//                    continue;
//                }

//                fs.Position += 22; // ページヘッダスキップ
//                int segmentCount = br.ReadByte();
//                byte[] lacingValues = br.ReadBytes(segmentCount);

//                int payloadLength = 0;
//                foreach (byte b in lacingValues) payloadLength += b;

//                byte[] payload = br.ReadBytes(payloadLength);

//                if (payload.Length < 7 || payload[0] != 0x03 || Encoding.ASCII.GetString(payload, 1, 6) != "vorbis")
//                    continue;

//                using (var ms = new MemoryStream(payload))
//                using (var r = new BinaryReader(ms))
//                {
//                    r.ReadByte(); // Packet type
//                    r.ReadBytes(6); // "vorbis"

//                    int vendorLen = r.ReadInt32();
//                    r.ReadBytes(vendorLen); // Vendor

//                    int commentCount = r.ReadInt32();
//                    for (int i = 0; i < commentCount; i++)
//                    {
//                        int len = r.ReadInt32();
//                        string comment = Encoding.UTF8.GetString(r.ReadBytes(len));

//                        if (comment.StartsWith("METADATA_BLOCK_PICTURE="))
//                        {
//                            string base64Data = comment.Substring("METADATA_BLOCK_PICTURE=".Length);
//                            byte[] imageData = Convert.FromBase64String(base64Data);

//                            File.WriteAllBytes(outputImagePath, imageData);
//                            Console.WriteLine($"画像を保存しました: {outputImagePath}");
//                            return;
//                        }
//                    }
//                }
//            }

//            Console.WriteLine("画像タグが見つかりませんでした。");
//        }
//    }
//}
