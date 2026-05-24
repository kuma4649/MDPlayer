using MDPlayer;
using MDPlayer.Driver.ZMS.nise68;
using System.Text;

namespace MDPlayerx64.Driver
{
    public class Flac : baseDriver
    {

        public System.Drawing.Image img = null;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();
            img = null;
            try
            {
                // Check FLAC file signature: "fLaC"
                if (buf.Length < 4 || buf[0] != 0x66 || buf[1] != 0x4C || buf[2] != 0x61 || buf[3] != 0x43)
                {
                    throw new Exception("Not FLAC file.");
                }

                string[] tags = new string[] { "TITLE=", "ALBUM=", "ARTIST=", "METADATA_BLOCK_PICTURE=" };
                List<string> cmt = new List<string>();

                // Parse FLAC metadata blocks
                int pos = 4; // Skip "fLaC" signature
                bool lastMetadataBlock = false;

                while (pos < buf.Length && !lastMetadataBlock)
                {
                    if (pos + 4 > buf.Length) break;

                    byte header = buf[pos];
                    lastMetadataBlock = (header & 0x80) != 0;
                    byte blockType = (byte)(header & 0x7F);
                    pos++;

                    // Read block length (3 bytes, big-endian)
                    if (pos + 3 > buf.Length) break;
                    uint blockLength = ((uint)buf[pos] << 16) | ((uint)buf[pos + 1] << 8) | buf[pos + 2];
                    pos += 3;

                    if (pos + blockLength > buf.Length) break;

                    // Process Vorbis Comment metadata block (type 4)
                    if (blockType == 4)
                    {
                        int blockStart = pos;
                        int blockEnd = pos + (int)blockLength;

                        // Read vendor string length (4 bytes, little-endian)
                        if (blockStart + 4 <= blockEnd)
                        {
                            uint vendorLen = (uint)buf[blockStart] | ((uint)buf[blockStart + 1] << 8) | ((uint)buf[blockStart + 2] << 16) | ((uint)buf[blockStart + 3] << 24);
                            int commentStart = blockStart + 4 + (int)vendorLen;

                            // Read comment count (4 bytes, little-endian)
                            if (commentStart + 4 <= blockEnd)
                            {
                                uint commentCount = (uint)buf[commentStart] | ((uint)buf[commentStart + 1] << 8) | ((uint)buf[commentStart + 2] << 16) | ((uint)buf[commentStart + 3] << 24);
                                int commentPos = commentStart + 4;

                                // Read each comment
                                for (uint i = 0; i < commentCount && commentPos < blockEnd; i++)
                                {
                                    if (commentPos + 4 > blockEnd) break;

                                    uint commentLen = (uint)buf[commentPos] | ((uint)buf[commentPos + 1] << 8) | ((uint)buf[commentPos + 2] << 16) | ((uint)buf[commentPos + 3] << 24);
                                    commentPos += 4;

                                    if (commentPos + commentLen > blockEnd) break;

                                    string comment = Encoding.UTF8.GetString(buf, commentPos, (int)commentLen);
                                    cmt.Add(comment);
                                    commentPos += (int)commentLen;
                                }
                            }
                        }
                    }
                    // Process Picture metadata block (type 6)
                    else if (blockType == 6)
                    {
                        try
                        {
                            int blockStart = pos;

                            // Use MemoryStream and manual big-endian reading
                            using (MemoryStream ms = new MemoryStream(buf, blockStart, (int)blockLength))
                            {
                                byte[] tempBuf = new byte[4];

                                // picture type (4 bytes, big-endian)
                                ms.Read(tempBuf, 0, 4);
                                int pictureType = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];

                                // MIME type length (4 bytes, big-endian)
                                ms.Read(tempBuf, 0, 4);
                                int mimeLen = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];

                                // Skip MIME type
                                if (mimeLen > 0)
                                {
                                    byte[] mimeBytes = new byte[mimeLen];
                                    ms.Read(mimeBytes, 0, mimeLen);
                                }

                                // Description length (4 bytes, big-endian)
                                ms.Read(tempBuf, 0, 4);
                                int descLen = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];

                                // Skip description
                                if (descLen > 0)
                                {
                                    byte[] descBytes = new byte[descLen];
                                    ms.Read(descBytes, 0, descLen);
                                }

                                // Picture dimensions and color info (4 bytes each, big-endian)
                                ms.Read(tempBuf, 0, 4);
                                int picWidth = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];
                                ms.Read(tempBuf, 0, 4);
                                int picHeight = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];
                                ms.Read(tempBuf, 0, 4);
                                int picDepth = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];
                                ms.Read(tempBuf, 0, 4);
                                int picColors = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];

                                // Picture data length (4 bytes, big-endian)
                                ms.Read(tempBuf, 0, 4);
                                int picDataLen = (tempBuf[0] << 24) | (tempBuf[1] << 16) | (tempBuf[2] << 8) | tempBuf[3];

                                if (picDataLen > 0 && ms.Position + picDataLen <= ms.Length)
                                {
                                    byte[] picData = new byte[picDataLen];
                                    ms.Read(picData, 0, picDataLen);

                                    // Search for PNG or JPEG signature
                                    int posPng = SearchByteArrayWithSpan(picData, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
                                    int posJpg = SearchByteArrayWithSpan(picData, new byte[] { 0xff, 0xd8 });

                                    if (posPng != -1 || posJpg != -1)
                                    {
                                        int imgPos = posPng;
                                        if (posPng == -1)
                                            imgPos = posJpg;
                                        if (posPng != -1 && posJpg != -1)
                                            imgPos = Math.Min(posPng, posJpg);

                                        if (imgPos >= 0)
                                        {
                                            byte[] imgData = new byte[picData.Length - imgPos];
                                            Array.Copy(picData, imgPos, imgData, 0, imgData.Length);
                                            using (MemoryStream pms = new MemoryStream(imgData))
                                                img = System.Drawing.Image.FromStream(pms);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Ignore picture parsing errors
                        }
                    }

                    pos += (int)blockLength;
                }

                // Process collected comments
                foreach (string c in cmt)
                {
                    foreach (string tag in tags)
                    {
                        if (c.IndexOf(tag, StringComparison.OrdinalIgnoreCase) < 0) continue;

                        string val = c.Substring(c.IndexOf('=') + 1);
                        if (tag == tags[0]) { GD3.TrackName = GD3.TrackNameJ = val; }
                        else if (tag == tags[1]) { GD3.GameName = GD3.GameNameJ = val; }
                        else if (tag == tags[2]) { GD3.Composer = GD3.ComposerJ = val; }
                        else if (tag == tags[3])
                        {
                            try
                            {
                                byte[] imageData = Decode(val);
                                int posPng = SearchByteArrayWithSpan(imageData, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
                                int posJpg = SearchByteArrayWithSpan(imageData, new byte[] { 0xff, 0xd8 });
                                if (posPng != -1 || posJpg != -1)
                                {
                                    int imgPos = posPng;
                                    if (posPng == -1)
                                        imgPos = posJpg;
                                    if (posPng != -1 && posJpg != -1)
                                    {
                                        imgPos = Math.Min(posPng, posJpg);
                                    }
                                    if (imgPos != -1)
                                    {
                                        Array.Copy(imageData, imgPos, imageData, 0, imageData.Length - imgPos);
                                        using (MemoryStream pms = new MemoryStream(imageData))
                                            img = System.Drawing.Image.FromStream(pms);
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
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

        private static readonly string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static byte[] Decode(string base64)
        {
            base64 = base64.TrimEnd('=');

            List<int> bits = new List<int>();
            foreach (char c in base64)
            {
                int index = base64Chars.IndexOf(c);
                if (index < 0)
                    throw new FormatException($"Invalid Base64 character: {c}");
                bits.Add(index);
            }

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

            if (bitsCollected > 0)
            {
                int byteValue = (buffer << (8 - bitsCollected)) & 0xFF;
                bytes.Add((byte)byteValue);
            }

            return bytes.ToArray();
        }

        public static int SearchByteArrayWithSpan(byte[] haystack, byte[] needle)
        {
            if (needle.Length == 0) return 0;
            if (haystack.Length < needle.Length) return -1;

            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }
    }
}


