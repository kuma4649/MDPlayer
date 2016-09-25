using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    [Serializable]
    public class Setting
    {
        private OutputDevice _outputDevice = new OutputDevice();
        public OutputDevice outputDevice
        {
            get
            {
                return _outputDevice;
            }

            set
            {
                _outputDevice = value;
            }
        }

        private ChipType _YM2612Type = new ChipType();
        public ChipType YM2612Type
        {
            get
            {
                return _YM2612Type;
            }

            set
            {
                _YM2612Type = value;
            }
        }

        private ChipType _SN76489Type = new ChipType();
        public ChipType SN76489Type
        {
            get
            {
                return _SN76489Type;
            }

            set
            {
                _SN76489Type = value;
            }
        }

        private ChipType _YM2608Type = new ChipType();
        public ChipType YM2608Type
        {
            get
            {
                return _YM2608Type;
            }

            set
            {
                _YM2608Type = value;
            }
        }

        private ChipType _YM2151Type = new ChipType();
        public ChipType YM2151Type
        {
            get
            {
                return _YM2151Type;
            }

            set
            {
                _YM2151Type = value;
            }
        }

        private int _LatencyEmulation = 0;
        public int LatencyEmulation
        {
            get
            {
                return _LatencyEmulation;
            }

            set
            {
                _LatencyEmulation = value;
            }
        }

        private int _LatencySCCI = 0;
        public int LatencySCCI
        {
            get
            {
                return _LatencySCCI;
            }

            set
            {
                _LatencySCCI = value;
            }
        }

        private bool _HiyorimiMode = true;
        public bool HiyorimiMode
        {
            get
            {
                return _HiyorimiMode;
            }

            set
            {
                _HiyorimiMode = value;
            }
        }

        private bool _Debug_DispFrameCounter = false;
        public bool Debug_DispFrameCounter
        {
            get
            {
                return _Debug_DispFrameCounter;
            }

            set
            {
                _Debug_DispFrameCounter = value;
            }
        }

        private Other _other = new Other();
        public Other other
        {
            get
            {
                return _other;
            }

            set
            {
                _other = value;
            }
        }

        private Balance _balance = new Balance();
        public Balance balance
        {
            get
            {
                return _balance;
            }

            set
            {
                _balance = value;
            }
        }

        private Location _location = new Location();
        public Location location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
            }
        }


        [Serializable]
        public class OutputDevice
        {

            private int _DeviceType = 0;
            public int DeviceType
            {
                get
                {
                    return _DeviceType;
                }

                set
                {
                    _DeviceType = value;
                }
            }

            private int _Latency = 300;
            public int Latency
            {
                get
                {
                    return _Latency;
                }

                set
                {
                    _Latency = value;
                }
            }

            private string _WaveOutDeviceName = "";
            public string WaveOutDeviceName
            {
                get
                {
                    return _WaveOutDeviceName;
                }

                set
                {
                    _WaveOutDeviceName = value;
                }
            }

            private string _DirectSoundDeviceName = "";
            public string DirectSoundDeviceName
            {
                get
                {
                    return _DirectSoundDeviceName;
                }

                set
                {
                    _DirectSoundDeviceName = value;
                }
            }

            private string _WasapiDeviceName = "";
            public string WasapiDeviceName
            {
                get
                {
                    return _WasapiDeviceName;
                }

                set
                {
                    _WasapiDeviceName = value;
                }
            }

            private bool _WasapiShareMode = true;
            public bool WasapiShareMode
            {
                get
                {
                    return _WasapiShareMode;
                }

                set
                {
                    _WasapiShareMode = value;
                }
            }

            private string _AsioDeviceName = "";
            public string AsioDeviceName
            {
                get
                {
                    return _AsioDeviceName;
                }

                set
                {
                    _AsioDeviceName = value;
                }
            }

            public OutputDevice Copy()
            {
                OutputDevice outputDevice = new OutputDevice();
                outputDevice.DeviceType = this.DeviceType;
                outputDevice.Latency = this.Latency;
                outputDevice.WaveOutDeviceName = this.WaveOutDeviceName;
                outputDevice.DirectSoundDeviceName = this.DirectSoundDeviceName;
                outputDevice.WasapiDeviceName = this.WasapiDeviceName;
                outputDevice.WasapiShareMode = this.WasapiShareMode;
                outputDevice.AsioDeviceName = this.AsioDeviceName;

                return outputDevice;
            }
        }

        [Serializable]
        public class ChipType
        {
            private bool _UseScci = false;
            public bool UseScci
            {
                get
                {
                    return _UseScci;
                }

                set
                {
                    _UseScci = value;
                }
            }

            private int _SoundLocation = -1;
            public int SoundLocation
            {
                get
                {
                    return _SoundLocation;
                }

                set
                {
                    _SoundLocation = value;
                }
            }

            private int _BusID = -1;
            public int BusID
            {
                get
                {
                    return _BusID;
                }

                set
                {
                    _BusID = value;
                }
            }

            private int _SoundChip = -1;
            public int SoundChip
            {
                get
                {
                    return _SoundChip;
                }

                set
                {
                    _SoundChip = value;
                }
            }

            private bool _UseWait = true;
            public bool UseWait
            {
                get
                {
                    return _UseWait;
                }

                set
                {
                    _UseWait = value;
                }
            }

            private bool _UseWaitBoost = false;
            public bool UseWaitBoost
            {
                get
                {
                    return _UseWaitBoost;
                }

                set
                {
                    _UseWaitBoost = value;
                }
            }

            private bool _OnlyPCMEmulation = false;
            public bool OnlyPCMEmulation
            {
                get
                {
                    return _OnlyPCMEmulation;
                }

                set
                {
                    _OnlyPCMEmulation = value;
                }
            }

            private int _LatencyForEmulation = 0;
            public int LatencyForEmulation
            {
                get
                {
                    return _LatencyForEmulation;
                }

                set
                {
                    _LatencyForEmulation = value;
                }
            }

            private int _LatencyForScci = 0;
            public int LatencyForScci
            {
                get
                {
                    return _LatencyForScci;
                }

                set
                {
                    _LatencyForScci = value;
                }
            }


            public ChipType Copy()
            {
                ChipType ct = new ChipType();
                ct.UseScci = this.UseScci;
                ct.SoundLocation = this.SoundLocation;
                ct.BusID = this.BusID;
                ct.SoundChip = this.SoundChip;
                ct.UseWait = this.UseWait;
                ct.UseWaitBoost = this.UseWaitBoost;
                ct.OnlyPCMEmulation = this.OnlyPCMEmulation;
                ct.LatencyForEmulation = this.LatencyForEmulation;
                ct.LatencyForScci = this.LatencyForScci;

                return ct;
            }
        }

        [Serializable]
        public class Other
        {
            private bool _UseLoopTimes = true;
            public bool UseLoopTimes
            {
                get
                {
                    return _UseLoopTimes;
                }

                set
                {
                    _UseLoopTimes = value;
                }
            }

            private int _LoopTimes = 2;
            public int LoopTimes
            {
                get
                {
                    return _LoopTimes;
                }

                set
                {
                    _LoopTimes = value;
                }
            }

            private bool _UseMIDIKeyboard = false;
            public bool UseMIDIKeyboard
            {
                get
                {
                    return _UseMIDIKeyboard;
                }

                set
                {
                    _UseMIDIKeyboard = value;
                }
            }

            private string _MidiInDeviceName="";
            public string MidiInDeviceName
            {
                get
                {
                    return _MidiInDeviceName;
                }

                set
                {
                    _MidiInDeviceName = value;
                }
            }

            private bool[] _UseChannel = new bool[9];
            public bool[] UseChannel
            {
                get
                {
                    return _UseChannel;
                }

                set
                {
                    _UseChannel = value;
                }
            }

            private bool _UseGetInst = true;
            public bool UseGetInst
            {
                get
                {
                    return _UseGetInst;
                }

                set
                {
                    _UseGetInst = value;
                }
            }

            private string _DefaultDataPath = "";
            public string DefaultDataPath
            {
                get
                {
                    return _DefaultDataPath;
                }

                set
                {
                    _DefaultDataPath = value;
                }
            }

            public enum enmInstFormat : int
            {
                FMP7 = 0,
                MDX = 1,
                TFI = 2,
                MUSICLALF = 3,
                MUSICLALF2 = 4,
                MML2VGM = 5,
                NRTDRV = 6
            }

            private enmInstFormat _InstFormat= enmInstFormat.MML2VGM;
            public enmInstFormat InstFormat
            {
                get
                {
                    return _InstFormat;
                }

                set
                {
                    _InstFormat = value;
                }
            }

            private int _Zoom = 1;
            public int Zoom
            {
                get
                {
                    return _Zoom;
                }

                set
                {
                    _Zoom = value;
                }
            }

            private int _ScreenFrameRate = 60;
            public int ScreenFrameRate
            {
                get
                {
                    return _ScreenFrameRate;
                }

                set
                {
                    _ScreenFrameRate = value;
                }
            }



            public Other Copy()
            {
                Other other = new Other();
                other.UseLoopTimes = this.UseLoopTimes;
                other.LoopTimes = this.LoopTimes;
                other.MidiInDeviceName = this.MidiInDeviceName;
                other.UseMIDIKeyboard = this.UseMIDIKeyboard;
                for (int i = 0; i < other.UseChannel.Length; i++)
                {
                    other.UseChannel[i] = this.UseChannel[i];
                }
                other.UseGetInst = this.UseGetInst;
                other.DefaultDataPath = this.DefaultDataPath;
                other.InstFormat = this.InstFormat;
                other.Zoom = this.Zoom;
                other.ScreenFrameRate = this.ScreenFrameRate;

                return other;
            }
        }

        [Serializable]
        public class Balance
        {

            private int _YM2612Volume = 170;
            public int YM2612Volume
            {
                get
                {
                    return _YM2612Volume;
                }

                set
                {
                    _YM2612Volume = value;
                }
            }

            private int _SN76489Volume = 100;
            public int SN76489Volume
            {
                get
                {
                    return _SN76489Volume;
                }

                set
                {
                    _SN76489Volume = value;
                }
            }

            private int _RF5C164Volume = 90;
            public int RF5C164Volume
            {
                get
                {
                    return _RF5C164Volume;
                }

                set
                {
                    _RF5C164Volume = value;
                }
            }

            private int _PWMVolume = 100;
            public int PWMVolume
            {
                get
                {
                    return _PWMVolume;
                }

                set
                {
                    _PWMVolume = value;
                }
            }

            private int _C140Volume = 40;
            public int C140Volume
            {
                get
                {
                    return _C140Volume;
                }

                set
                {
                    _C140Volume = value;
                }
            }

            private int _OKIM6258Volume = 100;
            public int OKIM6258Volume
            {
                get
                {
                    return _OKIM6258Volume;
                }

                set
                {
                    _OKIM6258Volume = value;
                }
            }

            private int _OKIM6295Volume = 100;
            public int OKIM6295Volume
            {
                get
                {
                    return _OKIM6295Volume;
                }

                set
                {
                    _OKIM6295Volume = value;
                }
            }

            private int _SEGAPCMVolume = 100;
            public int SEGAPCMVolume
            {
                get
                {
                    return _SEGAPCMVolume;
                }

                set
                {
                    _SEGAPCMVolume = value;
                }
            }

            public Balance Copy()
            {
                Balance Balance = new Balance();
                Balance.YM2612Volume = this.YM2612Volume;
                Balance.SN76489Volume = this.SN76489Volume;
                Balance.RF5C164Volume = this.RF5C164Volume;
                Balance.PWMVolume = this.PWMVolume;
                Balance.C140Volume = this.C140Volume;
                Balance.OKIM6258Volume = this.OKIM6258Volume;
                Balance.OKIM6295Volume = this.OKIM6295Volume;
                Balance.SEGAPCMVolume = this.SEGAPCMVolume;

                return Balance;
            }
        }

        [Serializable]
        public class Location
        {
            private Point _PMain = Point.Empty;
            public Point PMain
            {
                get
                {
                    return _PMain;
                }

                set
                {
                    _PMain = value;
                }
            }

            private Point _PInfo = Point.Empty;
            public Point PInfo
            {
                get
                {
                    return _PInfo;
                }

                set
                {
                    _PInfo = value;
                }
            }

            private bool _OInfo = false;
            public bool OInfo
            {
                get
                {
                    return _OInfo;
                }

                set
                {
                    _OInfo = value;
                }
            }

            private Point _PPlayList = Point.Empty;
            public Point PPlayList
            {
                get
                {
                    return _PPlayList;
                }

                set
                {
                    _PPlayList = value;
                }
            }

            private bool _OPlayList = false;
            public bool OPlayList
            {
                get
                {
                    return _OPlayList;
                }

                set
                {
                    _OPlayList = value;
                }
            }

            private Point _PPlayListWH = Point.Empty;
            public Point PPlayListWH
            {
                get
                {
                    return _PPlayListWH;
                }

                set
                {
                    _PPlayListWH = value;
                }
            }

            private Point _PRf5c164 = Point.Empty;
            public Point PRf5c164
            {
                get
                {
                    return _PRf5c164;
                }

                set
                {
                    _PRf5c164 = value;
                }
            }

            private bool _ORf5c164 = false;
            public bool ORf5c164
            {
                get
                {
                    return _ORf5c164;
                }

                set
                {
                    _ORf5c164 = value;
                }
            }

            private Point _PC140 = Point.Empty;
            public Point PC140
            {
                get
                {
                    return _PC140;
                }

                set
                {
                    _PC140 = value;
                }
            }

            private bool _OC140 = false;
            public bool OC140
            {
                get
                {
                    return _OC140;
                }

                set
                {
                    _OC140 = value;
                }
            }

            private Point _PYm2151 = Point.Empty;
            public Point PYm2151
            {
                get
                {
                    return _PYm2151;
                }

                set
                {
                    _PYm2151 = value;
                }
            }

            private bool _OYm2151 = false;
            public bool OYm2151
            {
                get
                {
                    return _OYm2151;
                }

                set
                {
                    _OYm2151 = value;
                }
            }

            private Point _PYm2608 = Point.Empty;
            public Point PYm2608
            {
                get
                {
                    return _PYm2608;
                }

                set
                {
                    _PYm2608 = value;
                }
            }

            private bool _OYm2608 = false;
            public bool OYm2608
            {
                get
                {
                    return _OYm2608;
                }

                set
                {
                    _OYm2608 = value;
                }
            }

            public Location Copy()
            {
                Location Location = new Location();
                Location.PMain = this.PMain;
                Location.PInfo = this.PInfo;
                Location.OInfo = this.OInfo;
                Location.PPlayList = this.PPlayList;
                Location.OPlayList = this.OPlayList;
                Location.PPlayListWH = this.PPlayListWH;
                Location.PRf5c164 = this.PRf5c164;
                Location.ORf5c164 = this.ORf5c164;
                Location.PC140 = this.PC140;
                Location.OC140 = this.OC140;
                Location.PYm2151 = this.PYm2151;
                Location.OYm2151 = this.OYm2151;
                Location.PYm2608 = this.PYm2608;
                Location.OYm2608 = this.OYm2608;

                return Location;
            }
        }

        public Setting Copy()
        {
            Setting setting = new Setting();
            setting.outputDevice = this.outputDevice.Copy();
            setting.YM2612Type = this.YM2612Type.Copy();
            setting.SN76489Type = this.SN76489Type.Copy();
            setting.YM2608Type = this.YM2608Type.Copy();
            setting.YM2151Type = this.YM2151Type.Copy();
            setting.other = this.other.Copy();
            setting.balance = this.balance.Copy();
            setting.LatencyEmulation = this.LatencyEmulation;
            setting.LatencySCCI = this.LatencySCCI;
            setting.Debug_DispFrameCounter = this.Debug_DispFrameCounter;
            setting.HiyorimiMode = this.HiyorimiMode;
            setting.location = this.location.Copy();

            return setting;
        }

        public void Save()
        {
            string fullPath= Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fullPath = System.IO.Path.Combine(fullPath, "KumaApp",AssemblyTitle);
            if(!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
            fullPath = System.IO.Path.Combine(fullPath, "Setting.xml");

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Setting));
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, this);
            }
        }

        public static Setting Load()
        {
            try
            {
                string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                fullPath = System.IO.Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                fullPath = System.IO.Path.Combine(fullPath, "Setting.xml");

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Setting));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fullPath, new UTF8Encoding(false)))
                {
                    return (Setting)serializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new Setting();
            }
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

    }
}
