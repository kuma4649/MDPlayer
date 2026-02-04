using NAudio.Wave;
using System.Text;

namespace MDPlayerx64
{
    using NAudio.Wave;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ShoutcastWaveStream : WaveStream, IDisposable
    {
        private readonly Stream _baseStream;
        private readonly int _metaInt;
        private int _bytesUntilMeta;
        private long _position;
        private readonly WaveFormat _waveFormat;

        public event EventHandler<string> MetadataReceived;

        /// <summary>
        /// ShoutcastWaveStreamのコンストラクタ
        /// </summary>
        /// <param name="baseStream">ネットワーク等のソースストリーム</param>
        /// <param name="metaInt">icy-metaintの値</param>
        /// <param name="format">ストリームのフォーマット（不明な場合は暫定のMP3等を指定）</param>
        public ShoutcastWaveStream(Stream baseStream, int metaInt, WaveFormat format = null)
        {
            _baseStream = baseStream;
            _metaInt = metaInt;
            _bytesUntilMeta = metaInt;
            _position = 0;
            // WaveStream継承にはWaveFormatが必須
            _waveFormat = format ?? WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
        }

        public override WaveFormat WaveFormat => _waveFormat;

        // ストリーミングのため長さは不明だが、WaveStreamの仕様上 0 または実数を返す
        public override long Length => 0;

        public override long Position
        {
            get => _position;
            set => _position = value; // シーク不可だが位置情報の保持は必要
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesToRead = Math.Min(count - totalBytesRead, _bytesUntilMeta);
                int bytesRead = _baseStream.Read(buffer, offset + totalBytesRead, bytesToRead);

                if (bytesRead == 0) break;

                _position += bytesRead;
                _bytesUntilMeta -= bytesRead;
                totalBytesRead += bytesRead;

                if (_bytesUntilMeta <= 0)
                {
                    ParseMetadata();
                    _bytesUntilMeta = _metaInt;
                }
            }

            return totalBytesRead;
        }

        private void ParseMetadata()
        {
            int metaLenByte = _baseStream.ReadByte();
            if (metaLenByte <= 0) return;

            int metaLen = metaLenByte * 16;
            byte[] metaBuffer = new byte[metaLen];
            int read = 0;
            while (read < metaLen)
            {
                int r = _baseStream.Read(metaBuffer, read, metaLen - read);
                if (r <= 0) break;
                read += r;
            }

            metaString = Encoding.UTF8.GetString(metaBuffer).TrimEnd('\0');
            if (!string.IsNullOrEmpty(metaString))
            {
                MetadataReceived?.Invoke(this, metaString);
            }
        }
        private string metaString;
        // 抽象メソッドの実装
        public override bool CanRead => true;
        public override bool CanSeek => true; // MediaFoundationReader等を騙すためにtrue

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _baseStream.Dispose();
            }
            base.Dispose(disposing);
        }

        private string oldtitle = "";

        public List<Tuple<string, string>> ReadTitle()
        {
            List<Tuple<string, string>> lst = new List<Tuple<string, string>>();
            if (string.IsNullOrEmpty(metaString)) return null;
            var match = Regex.Match(metaString, @"StreamTitle='(.*?)';", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                if (oldtitle == match.Groups[1].Value.Trim()) return null;
                oldtitle = match.Groups[1].Value.Trim();

                lst.Add(new Tuple<string, string>("title", match.Groups[1].Value.Trim()));
                return lst;
            }
            return null;
        }
    }
}
