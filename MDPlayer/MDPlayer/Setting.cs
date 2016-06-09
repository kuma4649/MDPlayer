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
                outputDevice.WaveOutDeviceName = this.WaveOutDeviceName;
                outputDevice.DirectSoundDeviceName = this.DirectSoundDeviceName;
                outputDevice.WasapiDeviceName = this.WasapiDeviceName;
                outputDevice.WasapiShareMode = this.WasapiShareMode;
                outputDevice.AsioDeviceName = this.AsioDeviceName;

                return outputDevice;
            }
        }

        [Serializable]
        public class Other
        {

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

            private bool[] _UseChannel = new bool[13];
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
                for (int i = 0; i < other.UseChannel.Length; i++)
                {
                    other.UseChannel[i] = this.UseChannel[i];
                }

                return other;
            }
        }

        public Setting Copy()
        {
            Setting setting = new Setting();
            setting.outputDevice = this.outputDevice.Copy();
            setting.other = this.other.Copy();

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

        public Other Other1
        {
            get
            {
                return Other2;
            }

            set
            {
                Other2 = value;
            }
        }

        public Other Other2
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
    }
}
