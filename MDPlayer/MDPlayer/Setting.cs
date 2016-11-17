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

        private ChipType _YM2203Type = new ChipType();
        public ChipType YM2203Type
        {
            get
            {
                return _YM2203Type;
            }

            set
            {
                _YM2203Type = value;
            }
        }

        private ChipType _YM2610Type = new ChipType();
        public ChipType YM2610Type
        {
            get
            {
                return _YM2610Type;
            }

            set
            {
                _YM2610Type = value;
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

            private bool _AutoOpen = false;
            public bool AutoOpen
            {
                get
                {
                    return _AutoOpen;
                }

                set
                {
                    _AutoOpen = value;
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
                other.AutoOpen = this.AutoOpen;

                return other;
            }
        }

        [Serializable]
        public class Balance
        {

            private int _MasterVolume = 0;
            public int MasterVolume
            {
                get
                {
                    if (_MasterVolume > 20 || _MasterVolume < -192) _MasterVolume = 0;
                    return _MasterVolume;
                }

                set
                {
                    _MasterVolume = value;
                    if (_MasterVolume > 20 || _MasterVolume < -192) _MasterVolume = 0;
                }
            }

            private int _YM2612Volume = 0;
            public int YM2612Volume
            {
                get
                {
                    if (_YM2612Volume > 20 || _YM2612Volume < -192) _YM2612Volume = 0;
                    return _YM2612Volume;
                }

                set
                {
                    _YM2612Volume = value;
                    if (_YM2612Volume > 20 || _YM2612Volume < -192) _YM2612Volume = 0;
                }
            }

            private int _SN76489Volume = 0;
            public int SN76489Volume
            {
                get
                {
                    if (_SN76489Volume > 20 || _SN76489Volume < -192) _SN76489Volume = 0;
                    return _SN76489Volume;
                }

                set
                {
                    _SN76489Volume = value;
                    if (_SN76489Volume > 20 || _SN76489Volume < -192) _SN76489Volume = 0;
                }
            }

            private int _RF5C164Volume = 0;
            public int RF5C164Volume
            {
                get
                {
                    if (_RF5C164Volume > 20 || _RF5C164Volume < -192) _RF5C164Volume = 0;
                    return _RF5C164Volume;
                }

                set
                {
                    _RF5C164Volume = value;
                    if (_RF5C164Volume > 20 || _RF5C164Volume < -192) _RF5C164Volume = 0;
                }
            }

            private int _PWMVolume = 0;
            public int PWMVolume
            {
                get
                {
                    if (_PWMVolume > 20 || _PWMVolume < -192) _PWMVolume = 0;
                    return _PWMVolume;
                }

                set
                {
                    _PWMVolume = value;
                    if (_PWMVolume > 20 || _PWMVolume < -192) _PWMVolume = 0;
                }
            }

            private int _C140Volume = 0;
            public int C140Volume
            {
                get
                {
                    if (_C140Volume > 20 || _C140Volume < -192) _C140Volume = 0;
                    return _C140Volume;
                }

                set
                {
                    _C140Volume = value;
                    if (_C140Volume > 20 || _C140Volume < -192) _C140Volume = 0;
                }
            }

            private int _OKIM6258Volume = 0;
            public int OKIM6258Volume
            {
                get
                {
                    if (_OKIM6258Volume > 20 || _OKIM6258Volume < -192) _OKIM6258Volume = 0;
                    return _OKIM6258Volume;
                }

                set
                {
                    _OKIM6258Volume = value;
                    if (_OKIM6258Volume > 20 || _OKIM6258Volume < -192) _OKIM6258Volume = 0;
                }
            }

            private int _OKIM6295Volume = 0;
            public int OKIM6295Volume
            {
                get
                {
                    if (_OKIM6295Volume > 20 || _OKIM6295Volume < -192) _OKIM6295Volume = 0;
                    return _OKIM6295Volume;
                }

                set
                {
                    _OKIM6295Volume = value;
                    if (_OKIM6295Volume > 20 || _OKIM6295Volume < -192) _OKIM6295Volume = 0;
                }
            }

            private int _SEGAPCMVolume = 0;
            public int SEGAPCMVolume
            {
                get
                {
                    if (_SEGAPCMVolume > 20 || _SEGAPCMVolume < -192) _SEGAPCMVolume = 0;
                    return _SEGAPCMVolume;
                }

                set
                {
                    _SEGAPCMVolume = value;
                    if (_SEGAPCMVolume > 20 || _SEGAPCMVolume < -192) _SEGAPCMVolume = 0;
                }
            }

            private int _YM2151Volume = 0;
            public int YM2151Volume
            {
                get
                {
                    if (_YM2151Volume > 20 || _YM2151Volume < -192) _YM2151Volume = 0;
                    return _YM2151Volume;
                }

                set
                {
                    _YM2151Volume = value;
                    if (_YM2151Volume > 20 || _YM2151Volume < -192) _YM2151Volume = 0;
                }
            }

            private int _YM2608Volume = 0;
            public int YM2608Volume
            {
                get
                {
                    if (_YM2608Volume > 20 || _YM2608Volume < -192) _YM2608Volume = 0;
                    return _YM2608Volume;
                }

                set
                {
                    _YM2608Volume = value;
                    if (_YM2608Volume > 20 || _YM2608Volume < -192) _YM2608Volume = 0;
                }
            }

            private int _YM2608FMVolume = 0;
            public int YM2608FMVolume
            {
                get
                {
                    if (_YM2608FMVolume > 20 || _YM2608FMVolume < -192) _YM2608FMVolume = 0;
                    return _YM2608FMVolume;
                }

                set
                {
                    _YM2608FMVolume = value;
                    if (_YM2608FMVolume > 20 || _YM2608FMVolume < -192) _YM2608FMVolume = 0;
                }
            }

            private int _YM2608PSGVolume = 0;
            public int YM2608PSGVolume
            {
                get
                {
                    if (_YM2608PSGVolume > 20 || _YM2608PSGVolume < -192) _YM2608PSGVolume = 0;
                    return _YM2608PSGVolume;
                }

                set
                {
                    _YM2608PSGVolume = value;
                    if (_YM2608PSGVolume > 20 || _YM2608PSGVolume < -192) _YM2608PSGVolume = 0;
                }
            }

            private int _YM2203Volume = 0;
            public int YM2203Volume
            {
                get
                {
                    if (_YM2203Volume > 20 || _YM2203Volume < -192) _YM2203Volume = 0;
                    return _YM2203Volume;
                }

                set
                {
                    _YM2203Volume = value;
                    if (_YM2203Volume > 20 || _YM2203Volume < -192) _YM2203Volume = 0;
                }
            }

            private int _YM2203FMVolume = 0;
            public int YM2203FMVolume
            {
                get
                {
                    if (_YM2203FMVolume > 20 || _YM2203FMVolume < -192) _YM2203FMVolume = 0;
                    return _YM2203FMVolume;
                }

                set
                {
                    _YM2203FMVolume = value;
                    if (_YM2203FMVolume > 20 || _YM2203FMVolume < -192) _YM2203FMVolume = 0;
                }
            }

            private int _YM2203PSGVolume = 0;
            public int YM2203PSGVolume
            {
                get
                {
                    if (_YM2203PSGVolume > 20 || _YM2203PSGVolume < -192) _YM2203PSGVolume = 0;
                    return _YM2203PSGVolume;
                }

                set
                {
                    _YM2203PSGVolume = value;
                    if (_YM2203PSGVolume > 20 || _YM2203PSGVolume < -192) _YM2203PSGVolume = 0;
                }
            }

            private int _YM2610Volume = 0;
            public int YM2610Volume
            {
                get
                {
                    if (_YM2610Volume > 20 || _YM2610Volume < -192) _YM2610Volume = 0;
                    return _YM2610Volume;
                }

                set
                {
                    _YM2610Volume = value;
                    if (_YM2610Volume > 20 || _YM2610Volume < -192) _YM2610Volume = 0;
                }
            }

            private int _YM2610FMVolume = 0;
            public int YM2610FMVolume
            {
                get
                {
                    if (_YM2610FMVolume > 20 || _YM2610FMVolume < -192) _YM2610FMVolume = 0;
                    return _YM2610FMVolume;
                }

                set
                {
                    _YM2610FMVolume = value;
                    if (_YM2610FMVolume > 20 || _YM2610FMVolume < -192) _YM2610FMVolume = 0;
                }
            }

            private int _YM2610PSGVolume = 0;
            public int YM2610PSGVolume
            {
                get
                {
                    if (_YM2610PSGVolume > 20 || _YM2610PSGVolume < -192) _YM2610PSGVolume = 0;
                    return _YM2610PSGVolume;
                }

                set
                {
                    _YM2610PSGVolume = value;
                    if (_YM2610PSGVolume > 20 || _YM2610PSGVolume < -192) _YM2610PSGVolume = 0;
                }
            }

            public Balance Copy()
            {
                Balance Balance = new Balance();
                Balance.MasterVolume = this.MasterVolume;
                Balance.YM2612Volume = this.YM2612Volume;
                Balance.SN76489Volume = this.SN76489Volume;
                Balance.RF5C164Volume = this.RF5C164Volume;
                Balance.PWMVolume = this.PWMVolume;
                Balance.C140Volume = this.C140Volume;
                Balance.OKIM6258Volume = this.OKIM6258Volume;
                Balance.OKIM6295Volume = this.OKIM6295Volume;
                Balance.SEGAPCMVolume = this.SEGAPCMVolume;
                Balance.YM2151Volume = this.YM2151Volume;
                Balance.YM2608Volume = this.YM2608Volume;
                Balance.YM2608FMVolume = this.YM2608FMVolume;
                Balance.YM2608PSGVolume = this.YM2608PSGVolume;
                Balance.YM2203Volume = this.YM2203Volume;
                Balance.YM2203FMVolume = this.YM2203FMVolume;
                Balance.YM2203PSGVolume = this.YM2203PSGVolume;
                Balance.YM2610Volume = this.YM2610Volume;
                Balance.YM2610FMVolume = this.YM2610FMVolume;
                Balance.YM2610PSGVolume = this.YM2610PSGVolume;

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
                    if (_PMain.X < 0 || _PMain.Y < 0)
                    {
                        return new Point(0, 0);
                    }
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
                    if (_PInfo.X < 0 || _PInfo.Y < 0)
                    {
                        return new Point(0, 0);
                    }
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
                    if (_PPlayList.X < 0 || _PPlayList.Y < 0)
                    {
                        return new Point(0, 0);
                    }
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
                    if (_PPlayListWH.X < 0 || _PPlayListWH.Y < 0)
                    {
                        return new Point(0, 0);
                    }
                    return _PPlayListWH;
                }

                set
                {
                    _PPlayListWH = value;
                }
            }

            private Point _PMixer = Point.Empty;
            public Point PMixer
            {
                get
                {
                    if (_PMixer.X < 0 || _PMixer.Y < 0)
                    {
                        return new Point(0, 0);
                    }
                    return _PMixer;
                }

                set
                {
                    _PMixer = value;
                }
            }

            private bool _OMixer = false;
            public bool OMixer
            {
                get
                {
                    return _OMixer;
                }

                set
                {
                    _OMixer = value;
                }
            }

            private Point _PMixerWH = Point.Empty;
            public Point PMixerWH
            {
                get
                {
                    if (_PMixerWH.X < 0 || _PMixerWH.Y < 0)
                    {
                        return new Point(0, 0);
                    }
                    return _PMixerWH;
                }

                set
                {
                    _PMixerWH = value;
                }
            }

            private Point[] _PosRf5c164 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosRf5c164
            {
                get
                {
                    return _PosRf5c164;
                }

                set
                {
                    _PosRf5c164 = value;
                }
            }

            private bool[] _OpenRf5c164 = new bool[2] { false, false };
            public bool[] OpenRf5c164
            {
                get
                {
                    return _OpenRf5c164;
                }

                set
                {
                    _OpenRf5c164 = value;
                }
            }

            private Point[] _PosC140 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosC140
            {
                get
                {
                    return _PosC140;
                }

                set
                {
                    _PosC140 = value;
                }
            }

            private bool[] _OpenC140 = new bool[2] { false, false };
            public bool[] OpenC140
            {
                get
                {
                    return _OpenC140;
                }

                set
                {
                    _OpenC140 = value;
                }
            }

            private Point[] _PosYm2151 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm2151
            {
                get
                {
                    return _PosYm2151;
                }

                set
                {
                    _PosYm2151 = value;
                }
            }

            private bool[] _OpenYm2151 = new bool[2] { false, false };
            public bool[] OpenYm2151
            {
                get
                {
                    return _OpenYm2151;
                }

                set
                {
                    _OpenYm2151 = value;
                }
            }

            private Point[] _PosYm2608 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm2608
            {
                get
                {
                    return _PosYm2608;
                }

                set
                {
                    _PosYm2608 = value;
                }
            }

            private bool[] _OpenYm2608 = new bool[2] { false, false };
            public bool[] OpenYm2608
            {
                get
                {
                    return _OpenYm2608;
                }

                set
                {
                    _OpenYm2608 = value;
                }
            }

            private Point[] _PosYm2203 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm2203
            {
                get
                {
                    return _PosYm2203;
                }

                set
                {
                    _PosYm2203 = value;
                }
            }

            private bool[] _OpenYm2203 = new bool[2] { false, false };
            public bool[] OpenYm2203
            {
                get
                {
                    return _OpenYm2203;
                }

                set
                {
                    _OpenYm2203 = value;
                }
            }

            private Point[] _PosYm2610 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm2610
            {
                get
                {
                    return _PosYm2610;
                }

                set
                {
                    _PosYm2610 = value;
                }
            }

            private bool[] _OpenYm2610 = new bool[2] { false, false };
            public bool[] OpenYm2610
            {
                get
                {
                    return _OpenYm2610;
                }

                set
                {
                    _OpenYm2610 = value;
                }
            }

            private Point[] _PosYm2612 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm2612
            {
                get
                {
                    return _PosYm2612;
                }

                set
                {
                    _PosYm2612 = value;
                }
            }

            private bool[] _OpenYm2612 = new bool[2] { false, false };
            public bool[] OpenYm2612
            {
                get
                {
                    return _OpenYm2612;
                }

                set
                {
                    _OpenYm2612 = value;
                }
            }

            private Point[] _PosOKIM6258 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosOKIM6258
            {
                get
                {
                    return _PosOKIM6258;
                }

                set
                {
                    _PosOKIM6258 = value;
                }
            }

            private bool[] _OpenOKIM6258 = new bool[2] { false, false };
            public bool[] OpenOKIM6258
            {
                get
                {
                    return _OpenOKIM6258;
                }

                set
                {
                    _OpenOKIM6258 = value;
                }
            }

            private Point[] _PosOKIM6295 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosOKIM6295
            {
                get
                {
                    return _PosOKIM6295;
                }

                set
                {
                    _PosOKIM6295 = value;
                }
            }

            private bool[] _OpenOKIM6295 = new bool[2] { false, false };
            public bool[] OpenOKIM6295
            {
                get
                {
                    return _OpenOKIM6295;
                }

                set
                {
                    _OpenOKIM6295 = value;
                }
            }

            private Point[] _PosSN76489 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosSN76489
            {
                get
                {
                    return _PosSN76489;
                }

                set
                {
                    _PosSN76489 = value;
                }
            }

            private bool[] _OpenSN76489 = new bool[2] { false, false };
            public bool[] OpenSN76489
            {
                get
                {
                    return _OpenSN76489;
                }

                set
                {
                    _OpenSN76489 = value;
                }
            }

            private Point[] _PosSegaPCM = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosSegaPCM
            {
                get
                {
                    return _PosSegaPCM;
                }

                set
                {
                    _PosSegaPCM = value;
                }
            }

            private bool[] _OpenSegaPCM = new bool[2] { false, false };
            public bool[] OpenSegaPCM
            {
                get
                {
                    return _OpenSegaPCM;
                }

                set
                {
                    _OpenSegaPCM = value;
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
                Location.PMixer = this.PMixer;
                Location.OMixer = this.OMixer;
                Location.PMixerWH = this.PMixerWH;
                Location.PosRf5c164 = this.PosRf5c164;
                Location.OpenRf5c164 = this.OpenRf5c164;
                Location.PosC140 = this.PosC140;
                Location.OpenC140 = this.OpenC140;
                Location.PosYm2151 = this.PosYm2151;
                Location.OpenYm2151 = this.OpenYm2151;
                Location.PosYm2608 = this.PosYm2608;
                Location.OpenYm2608 = this.OpenYm2608;
                Location.PosYm2203 = this.PosYm2203;
                Location.OpenYm2203 = this.OpenYm2203;
                Location.PosYm2610 = this.PosYm2610;
                Location.OpenYm2610 = this.OpenYm2610;
                Location.PosYm2612 = this.PosYm2612;
                Location.OpenYm2612 = this.OpenYm2612;
                Location.PosOKIM6258 = this.PosOKIM6258;
                Location.OpenOKIM6258 = this.OpenOKIM6258;
                Location.PosOKIM6295 = this.PosOKIM6295;
                Location.OpenOKIM6295 = this.OpenOKIM6295;
                Location.PosSN76489 = this.PosSN76489;
                Location.OpenSN76489 = this.OpenSN76489;
                Location.PosSegaPCM = this.PosSegaPCM;
                Location.OpenSegaPCM = this.OpenSegaPCM;

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
