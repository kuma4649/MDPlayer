using System;
using System.Collections.Generic;
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

            public Other Copy()
            {
                Other other = new Other();
                other.MidiInDeviceName = this.MidiInDeviceName;
                other.UseMIDIKeyboard = this.UseMIDIKeyboard;
                for (int i = 0; i < other.UseChannel.Length; i++)
                {
                    other.UseChannel[i] = this.UseChannel[i];
                }

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

            public Balance Copy()
            {
                Balance Balance = new Balance();
                Balance.YM2612Volume = this.YM2612Volume;
                Balance.SN76489Volume = this.SN76489Volume;
                Balance.RF5C164Volume = this.RF5C164Volume;
                Balance.PWMVolume = this.PWMVolume;

                return Balance;
            }
        }

        public Setting Copy()
        {
            Setting setting = new Setting();
            setting.outputDevice = this.outputDevice.Copy();
            setting.YM2612Type = this.YM2612Type.Copy();
            setting.SN76489Type = this.SN76489Type.Copy();
            setting.other = this.other.Copy();
            setting.balance = this.balance.Copy();
            setting.LatencyEmulation = this.LatencyEmulation;
            setting.LatencySCCI = this.LatencySCCI;
            setting.Debug_DispFrameCounter = this.Debug_DispFrameCounter;
            setting.HiyorimiMode = this.HiyorimiMode;

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
            catch
            {
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
