using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.NSF
{
    public class IDeviceInfo
    {
        public IDeviceInfo Clone() { return null; }
    }

    public class ITrackInfo : IDeviceInfo
    {
        public new IDeviceInfo Clone() { return null; }
        // 現在の出力値をそのまま返す
        public Int32 GetOutput() { return 0; }
        // 周波数をHzで返す
        public double GetFreqHz() { return 0; }
        // 周波数をデバイス依存値で返す．
        public UInt32 GetFreq() { return 0; }
        // 音量を返す
        public Int32 GetVolume() { return 0; }
        // 音量の最大値を返す
        public Int32 GetMaxVolume() { return 0; }
        // 発音中ならtrue OFFならfalse
        public bool GetKeyStatus() { return false; }
        // トーン番号
        public Int32 GetTone() { return 0; }

        // 周波数をノート番号に変換．0x60がo4c 0は無効
        public Int32 GetNote(double freq)
        {
            const double LOG2_440 = 8.7813597135246596040696824762152;
            const double LOG_2 = 0.69314718055994530941723212145818;
            const Int32 NOTE_440HZ = 0x69;

            if (freq > 1.0)
                return (int)((12 * (Math.Log(freq) / LOG_2 - LOG2_440) + NOTE_440HZ + 0.5));
            else
                return 0;
        }
    }

    /* TrackInfo を バッファリング */
    public class InfoBuffer
    {
        public class pair
        {
            public Int32 first;
            public IDeviceInfo second;
        }

        public Int32 bufmax;
        public Int32 index;
        public pair[] buffer;

        public InfoBuffer(int max = 60 * 10)
        {
            index = 0;
            bufmax = max;
            buffer = new pair[bufmax];
            for (int i = 0; i < bufmax; i++)
            {
                buffer[i] = new pair();
                buffer[i].first = 0;
                buffer[i].second = null;
            }
        }

        ~InfoBuffer()
        {
            for (int i = 0; i < bufmax; i++)
                buffer[i].second = null;
            buffer = null;
        }

        public void Clear()
        {
            for (int i = 0; i < bufmax; i++)
            {
                buffer[i].first = 0;
                buffer[i].second = null;
            }
        }

        public void AddInfo(int pos, IDeviceInfo di)
        {
            if (di != null)
            {
                buffer[index].first = pos;
                buffer[index].second = di.Clone();
                index = (index + 1) % bufmax;
            }
        }

        public IDeviceInfo GetInfo(int pos)
        {
            if (pos == -1)
                return buffer[(index + bufmax - 1) % bufmax].second;

            for (int i = (index + bufmax - 1) % bufmax; i != index; i = (i + bufmax - 1) % bufmax)
                if (buffer[i].first <= pos) return buffer[i].second;

            return null;
        }
    }

    public class TrackInfoBasic : ITrackInfo
    {
        public Int32 output;
        public Int32 volume;
        public Int32 max_volume;
        public UInt32 _freq;
        public double freq;
        public bool key;
        public Int32 tone;
        public new IDeviceInfo Clone() {
            TrackInfoBasic tib = new TrackInfoBasic();
            tib.output = output;
            tib.volume = volume;
            tib.max_volume = max_volume;
            tib._freq = _freq;
            tib.freq = freq;
            tib.key = key;
            tib.tone = tone;
            return tib;
        }
        public new Int32 GetOutput() { return output; }
        public new double GetFreqHz() { return freq; }
        public new UInt32 GetFreq() { return _freq; }
        public new bool GetKeyStatus() { return key; }
        public new Int32 GetVolume() { return volume; }
        public new Int32 GetMaxVolume() { return max_volume; }
        public new Int32 GetTone() { return tone; }
    };

}
