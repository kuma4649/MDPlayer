using MDPlayer;
using MDPlayer.Driver.ZMS.nise68;
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

            string[] tags = new string[] { "TITLE=", "ALBUM=", "ARTIST=", "LOOPLENGTH=", "LOOPSTART=", "METADATA_BLOCK_PICTURE=" };
            List<string> cmt = new List<string>();
            List<byte> metas = new List<byte>();

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

                    br.ReadByte();//version
                    byte headerType = br.ReadByte();
                    ms.Position += 20; // ページヘッダをスキップ
                    int segmentCount = br.ReadByte();
                    byte[] lacingValues = br.ReadBytes(segmentCount);

                    int payloadLength = 0;
                    foreach (byte b in lacingValues) payloadLength += b;

                    byte[] payload = br.ReadBytes(payloadLength);

                    // Vorbis Identification Headerをスキップ
                    if (headerType == 0x01)
                    {
                        foreach (byte b in payload) metas.Add(b);
                        continue;
                    }
                    if (payload.Length < 7 || payload[0] != 0x03 || Encoding.ASCII.GetString(payload, 1, 6) != "vorbis")
                        continue;

                    for (int i = 7; i < payload.Length; i++)
                    {
                        metas.Add(payload[i]);
                    }
                }
            }

            using (var ms1 = new MemoryStream(metas.ToArray()))
            using (var r = new BinaryReader(ms1))
            {
                //r.ReadByte(); // Packet Type (0x03)
                //r.ReadBytes(6); // "vorbis"

                int vendorLen = r.ReadInt32();
                r.ReadBytes(vendorLen); // Vendor string

                int commentCount = r.ReadInt32();
                for (int i = 0; i < commentCount; i++)
                {
                    int length = r.ReadInt32();
                    byte[] aaa = r.ReadBytes(length);
                    string comment = Encoding.UTF8.GetString(aaa);

                    cmt.Add(comment);
                }
            }

            looplength = -1;
            loopstart = -1;

            foreach (string c in cmt)
            {
                foreach (string tag in tags)
                {
                    if (c.IndexOf(tag) < 0) continue;

                    string val = c.Replace(tag, "");
                    if (tag == tags[0]) { GD3.TrackName = GD3.TrackNameJ = val; }
                    else if (tag == tags[1]) { GD3.GameName = GD3.GameNameJ = val; }
                    else if (tag == tags[2]) { GD3.Composer = GD3.ComposerJ = val; }
                    else if (tag == tags[3])
                    {
                        if (!long.TryParse(val, out _looplength))
                        {
                            _looplength = -1;
                        }
                        _looplength = Math.Max(looplength, -1);
                    }
                    else if (tag == tags[4])
                    {
                        if (!long.TryParse(val, out _loopstart))
                        {
                            _loopstart = -1;
                        }
                        _loopstart = Math.Max(_loopstart, -1);
                    }
                    else if (tag == tags[5])
                    {
                        try
                        {

                            //while(val.Length% 4 != 0) val += "=";
                            //val = val.Substring(42);
                            //val += "="; val += "=";
                            byte[] imageData = Decode(val); // Convert.FromBase64String(val);
                            int posPng = SearchByteArrayWithSpan(imageData, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);
                            int posJpg = SearchByteArrayWithSpan(imageData, [0xff, 0xd8]);
                            if (posPng != -1 || posJpg != -1)
                            {
                                int pos = posPng;
                                if (posPng == -1)
                                pos = posJpg;
                                if (posPng != -1 && posJpg != -1)
                                {
                                    pos = Math.Min(posPng, posJpg);
                                }
                                if (pos != -1)
                                {
                                    Array.Copy(imageData, pos, imageData, 0, imageData.Length - pos);
                                    //File.WriteAllBytes("bin.bin",imageData);
                                    using (MemoryStream pms = new MemoryStream(imageData))
                                        GD3.pic = Image.FromStream(pms);
                                }
                            }
                        }
                        catch
                        {
                        }
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

        private static readonly string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static byte[] Decode(string base64)
        {
            // パディング削除
            base64 = base64.TrimEnd('=');

            // 6ビット値をリストに格納
            List<int> bits = new List<int>();
            foreach (char c in base64)
            {
                int index = base64Chars.IndexOf(c);
                if (index < 0)
                    throw new FormatException($"Invalid Base64 character: {c}");
                bits.Add(index);
            }

            // 6ビットの集合を8ビットに再構成
            List<byte> bytes = new List<byte>();
            int buffer = 0, bitsCollected = 0;
            foreach (int value in bits)
            {
                buffer = (buffer << 6) | value;
                bitsCollected += 6;

                if (bitsCollected >= 8)
                {
                    bitsCollected -= 8;
                    int byteValue = (buffer >> bitsCollected) & 0xFF;
                    bytes.Add((byte)byteValue);
                }
            }

            return bytes.ToArray();
        }

        public static int SearchByteArrayWithSpan(byte[] source, byte[] pattern)
        {
            ReadOnlySpan<byte> sourceSpan = source;
            ReadOnlySpan<byte> patternSpan = pattern;

            for (int i = 0; i <= sourceSpan.Length - patternSpan.Length; i++)
            {
                if (sourceSpan.Slice(i, patternSpan.Length).SequenceEqual(patternSpan))
                {
                    return i; // マッチした開始位置を返す
                }
            }
            return -1; // 見つからない場合
        }

    }
}


