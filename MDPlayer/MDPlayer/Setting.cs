using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using MDPlayer.Properties;

namespace MDPlayer
{
    [Serializable]
    public class Setting
    {
        public Setting()
        {
        }

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
        
        private ChipType _AY8910Type = new ChipType();
        public ChipType AY8910Type
        {
            get
            {
                return _AY8910Type;
            }

            set
            {
                _AY8910Type = value;
            }
        }

        private ChipType _AY8910SType = new ChipType();
        public ChipType AY8910SType
        {
            get
            {
                return _AY8910SType;
            }

            set
            {
                _AY8910SType = value;
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

        private ChipType _YM2413Type = new ChipType();
        public ChipType YM2413Type
        {
            get
            {
                return _YM2413Type;
            }

            set
            {
                _YM2413Type = value;
            }
        }

        private ChipType _HuC6280Type = new ChipType();
        public ChipType HuC6280Type
        {
            get
            {
                return _HuC6280Type;
            }

            set
            {
                _HuC6280Type = value;
            }
        }

        private ChipType _K051649Type = new ChipType();
        public ChipType K051649Type
        {
            get
            {
                return _K051649Type;
            }

            set
            {
                _K051649Type = value;
            }
        }

        private ChipType _YM2413SType = new ChipType();
        public ChipType YM2413SType
        {
            get
            {
                return _YM2413SType;
            }

            set
            {
                _YM2413SType = value;
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

        private ChipType _YMF262Type = new ChipType();
        public ChipType YMF262Type
        {
            get
            {
                return _YMF262Type;
            }

            set
            {
                _YMF262Type = value;
            }
        }

        private ChipType _YMF271Type = new ChipType();
        public ChipType YMF271Type
        {
            get
            {
                return _YMF271Type;
            }

            set
            {
                _YMF271Type = value;
            }
        }

        private ChipType _YMF278BType = new ChipType();
        public ChipType YMF278BType
        {
            get
            {
                return _YMF278BType;
            }

            set
            {
                _YMF278BType = value;
            }
        }

        private ChipType _YMZ280BType = new ChipType();
        public ChipType YMZ280BType
        {
            get
            {
                return _YMZ280BType;
            }

            set
            {
                _YMZ280BType = value;
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

        private ChipType _YM2151SType = new ChipType();
        public ChipType YM2151SType
        {
            get
            {
                return _YM2151SType;
            }

            set
            {
                _YM2151SType = value;
            }
        }

        private ChipType _YM2203SType = new ChipType();
        public ChipType YM2203SType
        {
            get
            {
                return _YM2203SType;
            }

            set
            {
                _YM2203SType = value;
            }
        }

        private ChipType _YM2608SType = new ChipType();
        public ChipType YM2608SType
        {
            get
            {
                return _YM2608SType;
            }

            set
            {
                _YM2608SType = value;
            }
        }

        private ChipType _YM2610SType = new ChipType();
        public ChipType YM2610SType
        {
            get
            {
                return _YM2610SType;
            }

            set
            {
                _YM2610SType = value;
            }
        }

        private ChipType _YM2612SType = new ChipType();
        public ChipType YM2612SType
        {
            get
            {
                return _YM2612SType;
            }

            set
            {
                _YM2612SType = value;
            }
        }

        private ChipType _YMF262SType = new ChipType();
        public ChipType YMF262SType
        {
            get
            {
                return _YMF262SType;
            }

            set
            {
                _YMF262SType = value;
            }
        }

        private ChipType _YMF271SType = new ChipType();
        public ChipType YMF271SType
        {
            get
            {
                return _YMF271SType;
            }

            set
            {
                _YMF271SType = value;
            }
        }

        private ChipType _YMF278BSType = new ChipType();
        public ChipType YMF278BSType
        {
            get
            {
                return _YMF278BSType;
            }

            set
            {
                _YMF278BSType = value;
            }
        }

        private ChipType _YMZ280BSType = new ChipType();
        public ChipType YMZ280BSType
        {
            get
            {
                return _YMZ280BSType;
            }

            set
            {
                _YMZ280BSType = value;
            }
        }

        private ChipType _SN76489SType = new ChipType();
        public ChipType SN76489SType
        {
            get
            {
                return _SN76489SType;
            }

            set
            {
                _SN76489SType = value;
            }
        }

        private ChipType _HuC6280SType = new ChipType();
        public ChipType HuC6280SType
        {
            get
            {
                return _HuC6280SType;
            }

            set
            {
                _HuC6280SType = value;
            }
        }

        private ChipType _YM3526Type = new ChipType();
        public ChipType YM3526Type
        {
            get
            {
                return _YM3526Type;
            }

            set
            {
                _YM3526Type = value;
            }
        }

        private ChipType _YM3526SType = new ChipType();
        public ChipType YM3526SType
        {
            get
            {
                return _YM3526SType;
            }

            set
            {
                _YM3526SType = value;
            }
        }

        private ChipType _YM3812Type = new ChipType();
        public ChipType YM3812Type
        {
            get
            {
                return _YM3812Type;
            }

            set
            {
                _YM3812Type = value;
            }
        }

        private ChipType _YM3812SType = new ChipType();
        public ChipType YM3812SType
        {
            get
            {
                return _YM3812SType;
            }

            set
            {
                _YM3812SType = value;
            }
        }

        private ChipType _Y8950Type = new ChipType();
        public ChipType Y8950Type
        {
            get
            {
                return _Y8950Type;
            }

            set
            {
                _Y8950Type = value;
            }
        }

        private ChipType _Y8950SType = new ChipType();
        public ChipType Y8950SType
        {
            get
            {
                return _Y8950SType;
            }

            set
            {
                _Y8950SType = value;
            }
        }

        private ChipType _C140Type = new ChipType();
        public ChipType C140Type
        {
            get
            {
                return _C140Type;
            }

            set
            {
                _C140Type = value;
            }
        }

        private ChipType _C140SType = new ChipType();
        public ChipType C140SType
        {
            get
            {
                return _C140SType;
            }

            set
            {
                _C140SType = value;
            }
        }

        private ChipType _SEGAPCMType = new ChipType();
        public ChipType SEGAPCMType
        {
            get
            {
                return _SEGAPCMType;
            }

            set
            {
                _SEGAPCMType = value;
            }
        }

        private ChipType _SEGAPCMSType = new ChipType();
        public ChipType SEGAPCMSType
        {
            get
            {
                return _SEGAPCMSType;
            }

            set
            {
                _SEGAPCMSType = value;
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

        private MidiExport _midiExport = new MidiExport();
        public MidiExport midiExport
        {
            get
            {
                return _midiExport;
            }

            set
            {
                _midiExport = value;
            }
        }

        private MidiKbd _midiKbd = new MidiKbd();
        public MidiKbd midiKbd
        {
            get
            {
                return _midiKbd;
            }

            set
            {
                _midiKbd = value;
            }
        }

        private Vst _vst = new Vst();
        public Vst vst
        {
            get
            {
                return _vst;
            }

            set
            {
                _vst = value;
            }
        }

        private MidiOut _midiOut = new MidiOut();
        public MidiOut midiOut
        {
            get
            {
                return _midiOut;
            }

            set
            {
                _midiOut = value;
            }
        }

        private NSF _nsf = new NSF();
        public NSF nsf
        {
            get
            {
                return _nsf;
            }

            set
            {
                _nsf = value;
            }
        }

        private SID _sid = new SID();
        public SID sid
        {
            get
            {
                return _sid;
            }

            set
            {
                _sid = value;
            }
        }

        private NukedOPN2 _NukedOPN2 = new NukedOPN2();
        public NukedOPN2 nukedOPN2
        {
            get
            {
                return _NukedOPN2;
            }

            set
            {
                _NukedOPN2 = value;
            }
        }

        private AutoBalance _autoBalance = new AutoBalance();
        public AutoBalance autoBalance {
            get => _autoBalance; set => _autoBalance = value;
        }

        private PMDDotNET _PMDDotNET = new PMDDotNET();
        public PMDDotNET pmdDotNET
        {
            get
            {
                return _PMDDotNET;
            }

            set
            {
                _PMDDotNET = value;
            }
        }

        public KeyBoardHook keyBoardHook { get => _keyBoardHook; set => _keyBoardHook = value; }
        private KeyBoardHook _keyBoardHook = new KeyBoardHook();

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

            private int _WaitTime = 500;
            public int WaitTime
            {
                get
                {
                    return _WaitTime;
                }

                set
                {
                    _WaitTime = value;
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
                outputDevice.WaitTime = this.WaitTime;
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
            private bool _UseEmu = true;
            public bool UseEmu
            {
                get
                {
                    return _UseEmu;
                }

                set
                {
                    _UseEmu = value;
                }
            }

            private bool _UseEmu2 = false;
            public bool UseEmu2
            {
                get
                {
                    return _UseEmu2;
                }

                set
                {
                    _UseEmu2 = value;
                }
            }

            private bool _UseEmu3 = false;
            public bool UseEmu3
            {
                get
                {
                    return _UseEmu3;
                }

                set
                {
                    _UseEmu3 = value;
                }
            }


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

            private string _InterfaceName = "";
            public string InterfaceName
            {
                get
                {
                    return _InterfaceName;
                }

                set
                {
                    _InterfaceName = value;
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

            private string _ChipName = "";
            public string ChipName
            {
                get
                {
                    return _ChipName;
                }

                set
                {
                    _ChipName = value;
                }
            }


            private bool _UseScci2 = false;
            public bool UseScci2
            {
                get
                {
                    return _UseScci2;
                }

                set
                {
                    _UseScci2 = value;
                }
            }

            private string _InterfaceName2A = "";
            public string InterfaceName2A
            {
                get
                {
                    return _InterfaceName2A;
                }

                set
                {
                    _InterfaceName2A = value;
                }
            }

            private int _SoundLocation2A = -1;
            public int SoundLocation2A
            {
                get
                {
                    return _SoundLocation2A;
                }

                set
                {
                    _SoundLocation2A = value;
                }
            }

            private int _BusID2A = -1;
            public int BusID2A
            {
                get
                {
                    return _BusID2A;
                }

                set
                {
                    _BusID2A = value;
                }
            }

            private int _SoundChip2A = -1;
            public int SoundChip2A
            {
                get
                {
                    return _SoundChip2A;
                }

                set
                {
                    _SoundChip2A = value;
                }
            }

            private string _ChipName2A = "";
            public string ChipName2A
            {
                get
                {
                    return _ChipName2A;
                }

                set
                {
                    _ChipName2A = value;
                }
            }

            private string _InterfaceName2B = "";
            public string InterfaceName2B
            {
                get
                {
                    return _InterfaceName2B;
                }

                set
                {
                    _InterfaceName2B = value;
                }
            }

            private int _SoundLocation2B = -1;
            public int SoundLocation2B
            {
                get
                {
                    return _SoundLocation2B;
                }

                set
                {
                    _SoundLocation2B = value;
                }
            }

            private int _BusID2B = -1;
            public int BusID2B
            {
                get
                {
                    return _BusID2B;
                }

                set
                {
                    _BusID2B = value;
                }
            }

            private int _SoundChip2B = -1;
            public int SoundChip2B
            {
                get
                {
                    return _SoundChip2B;
                }

                set
                {
                    _SoundChip2B = value;
                }
            }

            private string _ChipName2B = "";
            public string ChipName2B
            {
                get
                {
                    return _ChipName2B;
                }

                set
                {
                    _ChipName2B = value;
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
                ct.UseEmu = this.UseEmu;
                ct.UseEmu2 = this.UseEmu2;
                ct.UseEmu3 = this.UseEmu3;
                ct.UseScci = this.UseScci;
                ct.SoundLocation = this.SoundLocation;

                ct.BusID = this.BusID;
                ct.InterfaceName = this.InterfaceName;
                ct.SoundChip = this.SoundChip;
                ct.ChipName = this.ChipName;
                ct.UseScci2 = this.UseScci2;
                ct.SoundLocation2A = this.SoundLocation2A;

                ct.InterfaceName2A = this.InterfaceName2A;
                ct.BusID2A = this.BusID2A;
                ct.SoundChip2A = this.SoundChip2A;
                ct.ChipName2A = this.ChipName2A;
                ct.SoundLocation2B = this.SoundLocation2B;

                ct.InterfaceName2B = this.InterfaceName2B;
                ct.BusID2B = this.BusID2B;
                ct.SoundChip2B = this.SoundChip2B;
                ct.ChipName2B = this.ChipName2B;

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

            private EnmInstFormat _InstFormat = EnmInstFormat.MML2VGM;
            public EnmInstFormat InstFormat
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

            private bool _DumpSwitch = false;
            public bool DumpSwitch
            {
                get
                {
                    return _DumpSwitch;
                }

                set
                {
                    _DumpSwitch = value;
                }
            }

            private string _DumpPath = "";
            public string DumpPath
            {
                get
                {
                    return _DumpPath;
                }

                set
                {
                    _DumpPath = value;
                }
            }

            private bool _WavSwitch = false;
            public bool WavSwitch
            {
                get
                {
                    return _WavSwitch;
                }

                set
                {
                    _WavSwitch = value;
                }
            }

            private string _WavPath = "";
            public string WavPath
            {
                get
                {
                    return _WavPath;
                }

                set
                {
                    _WavPath = value;
                }
            }

            private int _FilterIndex = 0;
            public int FilterIndex
            {
                get
                {
                    return _FilterIndex;
                }

                set
                {
                    _FilterIndex = value;
                }
            }

            private string _TextExt = "txt;doc;hed";
            public string TextExt { get => _TextExt; set => _TextExt = value; }

            private string _MMLExt = "mml;gwi;muc;mdl";
            public string MMLExt { get => _MMLExt; set => _MMLExt = value; }

            private string _ImageExt = "jpg;gif;png;mag";
            public string ImageExt { get => _ImageExt; set => _ImageExt = value; }

            private bool _AutoOpenText = false;
            public bool AutoOpenText { get => _AutoOpenText; set => _AutoOpenText = value; }
            private bool _AutoOpenMML = false;
            public bool AutoOpenMML { get => _AutoOpenMML; set => _AutoOpenMML = value; }
            private bool _AutoOpenImg = false;
            public bool AutoOpenImg { get => _AutoOpenImg; set => _AutoOpenImg = value; }

            private bool _InitAlways = false;
            public bool InitAlways { get => _InitAlways; set => _InitAlways = value; }

            private bool _EmptyPlayList = false;
            public bool EmptyPlayList { get => _EmptyPlayList; set => _EmptyPlayList = value; }
            public bool ExAll { get; set; } = false;
            public bool NonRenderingForPause { get; set; } = false;

            public Other Copy()
            {
                Other other = new Other();
                other.UseLoopTimes = this.UseLoopTimes;
                other.LoopTimes = this.LoopTimes;
                other.UseGetInst = this.UseGetInst;
                other.DefaultDataPath = this.DefaultDataPath;
                other.InstFormat = this.InstFormat;
                other.Zoom = this.Zoom;
                other.ScreenFrameRate = this.ScreenFrameRate;
                other.AutoOpen = this.AutoOpen;
                other.DumpSwitch = this.DumpSwitch;
                other.DumpPath = this.DumpPath;
                other.WavSwitch = this.WavSwitch;
                other.WavPath = this.WavPath;
                other.FilterIndex = this.FilterIndex;
                other.TextExt = this.TextExt;
                other.MMLExt = this.MMLExt;
                other.ImageExt = this.ImageExt;
                other.AutoOpenText = this.AutoOpenText;
                other.AutoOpenMML = this.AutoOpenMML;
                other.AutoOpenImg = this.AutoOpenImg;
                other.InitAlways = this.InitAlways;
                other.EmptyPlayList = this.EmptyPlayList;
                other.ExAll = this.ExAll;
                other.NonRenderingForPause = this.NonRenderingForPause;

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

            private int _RF5C68Volume = 0;
            public int RF5C68Volume
            {
                get
                {
                    if (_RF5C68Volume > 20 || _RF5C68Volume < -192) _RF5C68Volume = 0;
                    return _RF5C68Volume;
                }

                set
                {
                    _RF5C68Volume = value;
                    if (_RF5C68Volume > 20 || _RF5C68Volume < -192) _RF5C68Volume = 0;
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

            private int _AY8910Volume = 0;
            public int AY8910Volume
            {
                get
                {
                    if (_AY8910Volume > 20 || _AY8910Volume < -192) _AY8910Volume = 0;
                    return _AY8910Volume;
                }

                set
                {
                    _AY8910Volume = value;
                    if (_AY8910Volume > 20 || _AY8910Volume < -192) _AY8910Volume = 0;
                }
            }

            private int _YM2413Volume = 0;
            public int YM2413Volume
            {
                get
                {
                    if (_YM2413Volume > 20 || _YM2413Volume < -192) _YM2413Volume = 0;
                    return _YM2413Volume;
                }

                set
                {
                    _YM2413Volume = value;
                    if (_YM2413Volume > 20 || _YM2413Volume < -192) _YM2413Volume = 0;
                }
            }

            private int _YM3526Volume = 0;
            public int YM3526Volume
            {
                get
                {
                    if (_YM3526Volume > 20 || _YM3526Volume < -192) _YM3526Volume = 0;
                    return _YM3526Volume;
                }

                set
                {
                    _YM3526Volume = value;
                    if (_YM3526Volume > 20 || _YM3526Volume < -192) _YM3526Volume = 0;
                }
            }

            private int _Y8950Volume = 0;
            public int Y8950Volume
            {
                get
                {
                    if (_Y8950Volume > 20 || _Y8950Volume < -192) _Y8950Volume = 0;
                    return _Y8950Volume;
                }

                set
                {
                    _Y8950Volume = value;
                    if (_Y8950Volume > 20 || _Y8950Volume < -192) _Y8950Volume = 0;
                }
            }

            private int _HuC6280Volume = 0;
            public int HuC6280Volume
            {
                get
                {
                    if (_HuC6280Volume > 20 || _HuC6280Volume < -192) _HuC6280Volume = 0;
                    return _HuC6280Volume;
                }

                set
                {
                    _HuC6280Volume = value;
                    if (_HuC6280Volume > 20 || _HuC6280Volume < -192) _HuC6280Volume = 0;
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

            private int _YM2608RhythmVolume = 0;
            public int YM2608RhythmVolume
            {
                get
                {
                    if (_YM2608RhythmVolume > 20 || _YM2608RhythmVolume < -192) _YM2608RhythmVolume = 0;
                    return _YM2608RhythmVolume;
                }

                set
                {
                    _YM2608RhythmVolume = value;
                    if (_YM2608RhythmVolume > 20 || _YM2608RhythmVolume < -192) _YM2608RhythmVolume = 0;
                }
            }

            private int _YM2608AdpcmVolume = 0;
            public int YM2608AdpcmVolume
            {
                get
                {
                    if (_YM2608AdpcmVolume > 20 || _YM2608AdpcmVolume < -192) _YM2608AdpcmVolume = 0;
                    return _YM2608AdpcmVolume;
                }

                set
                {
                    _YM2608AdpcmVolume = value;
                    if (_YM2608AdpcmVolume > 20 || _YM2608AdpcmVolume < -192) _YM2608AdpcmVolume = 0;
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

            private int _YM2610AdpcmAVolume = 0;
            public int YM2610AdpcmAVolume
            {
                get
                {
                    if (_YM2610AdpcmAVolume > 20 || _YM2610AdpcmAVolume < -192) _YM2610AdpcmAVolume = 0;
                    return _YM2610AdpcmAVolume;
                }

                set
                {
                    _YM2610AdpcmAVolume = value;
                    if (_YM2610AdpcmAVolume > 20 || _YM2610AdpcmAVolume < -192) _YM2610AdpcmAVolume = 0;
                }
            }

            private int _YM2610AdpcmBVolume = 0;
            public int YM2610AdpcmBVolume
            {
                get
                {
                    if (_YM2610AdpcmBVolume > 20 || _YM2610AdpcmBVolume < -192) _YM2610AdpcmBVolume = 0;
                    return _YM2610AdpcmBVolume;
                }

                set
                {
                    _YM2610AdpcmBVolume = value;
                    if (_YM2610AdpcmBVolume > 20 || _YM2610AdpcmBVolume < -192) _YM2610AdpcmBVolume = 0;
                }
            }

            private int _YM3812Volume = 0;
            public int YM3812Volume
            {
                get
                {
                    if (_YM3812Volume > 20 || _YM3812Volume < -192) _YM3812Volume = 0;
                    return _YM3812Volume;
                }

                set
                {
                    _YM3812Volume = value;
                    if (_YM3812Volume > 20 || _YM3812Volume < -192) _YM3812Volume = 0;
                }
            }

            private int _C352Volume = 0;
            public int C352Volume
            {
                get
                {
                    if (_C352Volume > 20 || _C352Volume < -192) _C352Volume = 0;
                    return _C352Volume;
                }

                set
                {
                    _C352Volume = value;
                    if (_C352Volume > 20 || _C352Volume < -192) _C352Volume = 0;
                }
            }

            private int _SAA1099Volume = 0;
            public int SAA1099Volume
            {
                get
                {
                    if (_SAA1099Volume > 20 || _SAA1099Volume < -192) _SAA1099Volume = 0;
                    return _SAA1099Volume;
                }

                set
                {
                    _SAA1099Volume = value;
                    if (_SAA1099Volume > 20 || _SAA1099Volume < -192) _SAA1099Volume = 0;
                }
            }

            private int _POKEYVolume = 0;
            public int POKEYVolume
            {
                get
                {
                    if (_POKEYVolume > 20 || _POKEYVolume < -192) _POKEYVolume = 0;
                    return _POKEYVolume;
                }

                set
                {
                    _POKEYVolume = value;
                    if (_POKEYVolume > 20 || _POKEYVolume < -192) _POKEYVolume = 0;
                }
            }

            private int _PPZ8Volume = 0;
            public int PPZ8Volume
            {
                get
                {
                    if (_PPZ8Volume > 20 || _PPZ8Volume < -192) _PPZ8Volume = 0;
                    return _PPZ8Volume;
                }

                set
                {
                    _PPZ8Volume = value;
                    if (_PPZ8Volume > 20 || _PPZ8Volume < -192) _PPZ8Volume = 0;
                }
            }

            private int _X1_010Volume = 0;
            public int X1_010Volume
            {
                get
                {
                    if (_X1_010Volume > 20 || _X1_010Volume < -192) _X1_010Volume = 0;
                    return _X1_010Volume;
                }

                set
                {
                    _X1_010Volume = value;
                    if (_X1_010Volume > 20 || _X1_010Volume < -192) _X1_010Volume = 0;
                }
            }

            private int _K054539Volume = 0;
            public int K054539Volume
            {
                get
                {
                    if (_K054539Volume > 20 || _K054539Volume < -192) _K054539Volume = 0;
                    return _K054539Volume;
                }

                set
                {
                    _K054539Volume = value;
                    if (_K054539Volume > 20 || _K054539Volume < -192) _K054539Volume = 0;
                }
            }

            private int _APUVolume = 0;
            public int APUVolume
            {
                get
                {
                    if (_APUVolume > 20 || _APUVolume < -192) _APUVolume = 0;
                    return _APUVolume;
                }

                set
                {
                    _APUVolume = value;
                    if (_APUVolume > 20 || _APUVolume < -192) _APUVolume = 0;
                }
            }

            private int _DMCVolume = 0;
            public int DMCVolume
            {
                get
                {
                    if (_DMCVolume > 20 || _DMCVolume < -192) _DMCVolume = 0;
                    return _DMCVolume;
                }

                set
                {
                    _DMCVolume = value;
                    if (_DMCVolume > 20 || _DMCVolume < -192) _DMCVolume = 0;
                }
            }

            private int _FDSVolume = 0;
            public int FDSVolume
            {
                get
                {
                    if (_FDSVolume > 20 || _FDSVolume < -192) _FDSVolume = 0;
                    return _FDSVolume;
                }

                set
                {
                    _FDSVolume = value;
                    if (_FDSVolume > 20 || _FDSVolume < -192) _FDSVolume = 0;
                }
            }

            private int _MMC5Volume = 0;
            public int MMC5Volume
            {
                get
                {
                    if (_MMC5Volume > 20 || _MMC5Volume < -192) _MMC5Volume = 0;
                    return _MMC5Volume;
                }

                set
                {
                    _MMC5Volume = value;
                    if (_MMC5Volume > 20 || _MMC5Volume < -192) _MMC5Volume = 0;
                }
            }

            private int _N160Volume = 0;
            public int N160Volume
            {
                get
                {
                    if (_N160Volume > 20 || _N160Volume < -192) _N160Volume = 0;
                    return _N160Volume;
                }

                set
                {
                    _N160Volume = value;
                    if (_N160Volume > 20 || _N160Volume < -192) _N160Volume = 0;
                }
            }

            private int _VRC6Volume = 0;
            public int VRC6Volume
            {
                get
                {
                    if (_VRC6Volume > 20 || _VRC6Volume < -192) _VRC6Volume = 0;
                    return _VRC6Volume;
                }

                set
                {
                    _VRC6Volume = value;
                    if (_VRC6Volume > 20 || _VRC6Volume < -192) _VRC6Volume = 0;
                }
            }

            private int _VRC7Volume = 0;
            public int VRC7Volume
            {
                get
                {
                    if (_VRC7Volume > 20 || _VRC7Volume < -192) _VRC7Volume = 0;
                    return _VRC7Volume;
                }

                set
                {
                    _VRC7Volume = value;
                    if (_VRC7Volume > 20 || _VRC7Volume < -192) _VRC7Volume = 0;
                }
            }

            private int _FME7Volume = 0;
            public int FME7Volume
            {
                get
                {
                    if (_FME7Volume > 20 || _FME7Volume < -192) _FME7Volume = 0;
                    return _FME7Volume;
                }

                set
                {
                    _FME7Volume = value;
                    if (_FME7Volume > 20 || _FME7Volume < -192) _FME7Volume = 0;
                }
            }

            private int _DMGVolume = 0;
            public int DMGVolume
            {
                get
                {
                    if (_DMGVolume > 20 || _DMGVolume < -192) _DMGVolume = 0;
                    return _DMGVolume;
                }

                set
                {
                    _DMGVolume = value;
                    if (_DMGVolume > 20 || _DMGVolume < -192) _DMGVolume = 0;
                }
            }

            private int _GA20Volume = 0;
            public int GA20Volume
            {
                get
                {
                    if (_GA20Volume > 20 || _GA20Volume < -192) _GA20Volume = 0;
                    return _GA20Volume;
                }

                set
                {
                    _GA20Volume = value;
                    if (_GA20Volume > 20 || _GA20Volume < -192) _GA20Volume = 0;
                }
            }

            private int _YMZ280BVolume = 0;
            public int YMZ280BVolume
            {
                get
                {
                    if (_YMZ280BVolume > 20 || _YMZ280BVolume < -192) _YMZ280BVolume = 0;
                    return _YMZ280BVolume;
                }

                set
                {
                    _YMZ280BVolume = value;
                    if (_YMZ280BVolume > 20 || _YMZ280BVolume < -192) _YMZ280BVolume = 0;
                }
            }

            private int _YMF271Volume = 0;
            public int YMF271Volume
            {
                get
                {
                    if (_YMF271Volume > 20 || _YMF271Volume < -192) _YMF271Volume = 0;
                    return _YMF271Volume;
                }

                set
                {
                    _YMF271Volume = value;
                    if (_YMF271Volume > 20 || _YMF271Volume < -192) _YMF271Volume = 0;
                }
            }

            private int _YMF262Volume = 0;
            public int YMF262Volume
            {
                get
                {
                    if (_YMF262Volume > 20 || _YMF262Volume < -192) _YMF262Volume = 0;
                    return _YMF262Volume;
                }

                set
                {
                    _YMF262Volume = value;
                    if (_YMF262Volume > 20 || _YMF262Volume < -192) _YMF262Volume = 0;
                }
            }

            private int _YMF278BVolume = 0;
            public int YMF278BVolume
            {
                get
                {
                    if (_YMF278BVolume > 20 || _YMF278BVolume < -192) _YMF278BVolume = 0;
                    return _YMF278BVolume;
                }

                set
                {
                    _YMF278BVolume = value;
                    if (_YMF278BVolume > 20 || _YMF278BVolume < -192) _YMF278BVolume = 0;
                }
            }

            private int _MultiPCMVolume = 0;
            public int MultiPCMVolume
            {
                get
                {
                    if (_MultiPCMVolume > 20 || _MultiPCMVolume < -192) _MultiPCMVolume = 0;
                    return _MultiPCMVolume;
                }

                set
                {
                    _MultiPCMVolume = value;
                    if (_MultiPCMVolume > 20 || _MultiPCMVolume < -192) _MultiPCMVolume = 0;
                }
            }

            private int _QSoundVolume = 0;
            public int QSoundVolume
            {
                get
                {
                    if (_QSoundVolume > 20 || _QSoundVolume < -192) _QSoundVolume = 0;
                    return _QSoundVolume;
                }

                set
                {
                    _QSoundVolume = value;
                    if (_QSoundVolume > 20 || _QSoundVolume < -192) _QSoundVolume = 0;
                }
            }

            private int _K051649Volume = 0;
            public int K051649Volume
            {
                get
                {
                    if (_K051649Volume > 20 || _K051649Volume < -192) _K051649Volume = 0;
                    return _K051649Volume;
                }

                set
                {
                    _K051649Volume = value;
                    if (_K051649Volume > 20 || _K051649Volume < -192) _K051649Volume = 0;
                }
            }

            private int _K053260Volume = 0;
            public int K053260Volume
            {
                get
                {
                    if (_K053260Volume > 20 || _K053260Volume < -192) _K053260Volume = 0;
                    return _K053260Volume;
                }

                set
                {
                    _K053260Volume = value;
                    if (_K053260Volume > 20 || _K053260Volume < -192) _K053260Volume = 0;
                }
            }

            private int _GimicOPNVolume = 0;
            public int GimicOPNVolume
            {
                get
                {
                    if (_GimicOPNVolume > 127 || _GimicOPNVolume < 0) _GimicOPNVolume = 30;
                    return _GimicOPNVolume;
                }

                set
                {
                    _GimicOPNVolume = value;
                    if (_GimicOPNVolume > 127 || _GimicOPNVolume < 0) _GimicOPNVolume = 30;
                }
            }

            private int _GimicOPNAVolume = 0;
            public int GimicOPNAVolume
            {
                get
                {
                    if (_GimicOPNAVolume > 127 || _GimicOPNAVolume < 0) _GimicOPNAVolume = 30;
                    return _GimicOPNAVolume;
                }

                set
                {
                    _GimicOPNAVolume = value;
                    if (_GimicOPNAVolume > 127 || _GimicOPNAVolume < 0) _GimicOPNAVolume = 30;
                }
            }

            public Balance Copy()
            {
                Balance Balance = new Balance();
                Balance.MasterVolume = this.MasterVolume;
                Balance.YM2151Volume = this.YM2151Volume;
                Balance.YM2203Volume = this.YM2203Volume;
                Balance.YM2203FMVolume = this.YM2203FMVolume;
                Balance.YM2203PSGVolume = this.YM2203PSGVolume;
                Balance.YM2413Volume = this.YM2413Volume;
                Balance.YM2608Volume = this.YM2608Volume;
                Balance.YM2608FMVolume = this.YM2608FMVolume;
                Balance.YM2608PSGVolume = this.YM2608PSGVolume;
                Balance.YM2608RhythmVolume = this.YM2608RhythmVolume;
                Balance.YM2608AdpcmVolume = this.YM2608AdpcmVolume;
                Balance.YM2610Volume = this.YM2610Volume;
                Balance.YM2610FMVolume = this.YM2610FMVolume;
                Balance.YM2610PSGVolume = this.YM2610PSGVolume;
                Balance.YM2610AdpcmAVolume = this.YM2610AdpcmAVolume;
                Balance.YM2610AdpcmBVolume = this.YM2610AdpcmBVolume;

                Balance.YM2612Volume = this.YM2612Volume;
                Balance.AY8910Volume = this.AY8910Volume;
                Balance.SN76489Volume = this.SN76489Volume;
                Balance.HuC6280Volume = this.HuC6280Volume;
                Balance.SAA1099Volume = this.SAA1099Volume;

                Balance.RF5C164Volume = this.RF5C164Volume;
                Balance.RF5C68Volume = this.RF5C68Volume;
                Balance.PWMVolume = this.PWMVolume;
                Balance.OKIM6258Volume = this.OKIM6258Volume;
                Balance.OKIM6295Volume = this.OKIM6295Volume;
                Balance.C140Volume = this.C140Volume;
                Balance.SEGAPCMVolume = this.SEGAPCMVolume;
                Balance.C352Volume = this.C352Volume;
                Balance.K051649Volume = this.K051649Volume;
                Balance.K053260Volume = this.K053260Volume;
                Balance.K054539Volume = this.K054539Volume;
                Balance.QSoundVolume = this.QSoundVolume;
                Balance.MultiPCMVolume = this.MultiPCMVolume;

                Balance.APUVolume = this.APUVolume;
                Balance.DMCVolume = this.DMCVolume;
                Balance.FDSVolume = this.FDSVolume;
                Balance.MMC5Volume = this.MMC5Volume;
                Balance.N160Volume = this.N160Volume;
                Balance.VRC6Volume = this.VRC6Volume;
                Balance.VRC7Volume = this.VRC7Volume;
                Balance.FME7Volume = this.FME7Volume;
                Balance.DMGVolume = this.DMGVolume;
                Balance.GA20Volume = this.GA20Volume;
                Balance.YMZ280BVolume = this.YMZ280BVolume;
                Balance.YMF271Volume = this.YMF271Volume;
                Balance.YMF262Volume = this.YMF262Volume;
                Balance.YMF278BVolume = this.YMF278BVolume;
                Balance.YM3526Volume = this.YM3526Volume;
                Balance.Y8950Volume = this.Y8950Volume;
                Balance.YM3812Volume = this.YM3812Volume;

                Balance.PPZ8Volume = this.PPZ8Volume;
                Balance.GimicOPNVolume = this.GimicOPNVolume;
                Balance.GimicOPNAVolume = this.GimicOPNAVolume;

                return Balance;
            }

            public void Save(string fullPath)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Balance), typeof(Balance).GetNestedTypes());
                using (StreamWriter sw = new StreamWriter(fullPath, false, new UTF8Encoding(false)))
                {
                    serializer.Serialize(sw, this);
                }
            }

            public static Balance Load(string fullPath)
            {
                try
                {
                    if (!File.Exists(fullPath)) return null;
                    XmlSerializer serializer = new XmlSerializer(typeof(Balance), typeof(Balance).GetNestedTypes());
                    using (StreamReader sr = new StreamReader(fullPath, new UTF8Encoding(false)))
                    {
                        return (Balance)serializer.Deserialize(sr);
                    }
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    return null;
                }
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


            private Point[] _PosRf5c68 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosRf5c68
            {
                get
                {
                    return _PosRf5c68;
                }

                set
                {
                    _PosRf5c68 = value;
                }
            }

            private bool[] _OpenRf5c68 = new bool[2] { false, false };
            public bool[] OpenRf5c68
            {
                get
                {
                    return _OpenRf5c68;
                }

                set
                {
                    _OpenRf5c68 = value;
                }
            }

            private Point[] _PosYMF271 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYMF271
            {
                get
                {
                    return _PosYMF271;
                }

                set
                {
                    _PosYMF271 = value;
                }
            }

            private bool[] _OpenYMF271 = new bool[2] { false, false };
            public bool[] OpenYMF271
            {
                get
                {
                    return _OpenYMF271;
                }

                set
                {
                    _OpenYMF271 = value;
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

            private Point[] _PosS5B = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosS5B
            {
                get
                {
                    return _PosS5B;
                }

                set
                {
                    _PosS5B = value;
                }
            }

            private bool[] _OpenS5B = new bool[2] { false, false };
            public bool[] OpenS5B
            {
                get
                {
                    return _OpenS5B;
                }

                set
                {
                    _OpenS5B = value;
                }
            }

            private Point[] _PosDMG = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosDMG
            {
                get
                {
                    return _PosDMG;
                }

                set
                {
                    _PosDMG = value;
                }
            }

            private bool[] _OpenDMG = new bool[2] { false, false };
            public bool[] OpenDMG
            {
                get
                {
                    return _OpenDMG;
                }

                set
                {
                    _OpenDMG = value;
                }
            }

            private Point[] _PosPPZ8 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosPPZ8
            {
                get
                {
                    return _PosPPZ8;
                }

                set
                {
                    _PosPPZ8 = value;
                }
            }

            private bool[] _OpenPPZ8 = new bool[2] { false, false };
            public bool[] OpenPPZ8
            {
                get
                {
                    return _OpenPPZ8;
                }

                set
                {
                    _OpenPPZ8 = value;
                }
            }

            private Point[] _PosYMZ280B = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYMZ280B
            {
                get
                {
                    return _PosYMZ280B;
                }

                set
                {
                    _PosYMZ280B = value;
                }
            }

            private bool[] _OpenYMZ280B = new bool[2] { false, false };
            public bool[] OpenYMZ280B
            {
                get
                {
                    return _OpenYMZ280B;
                }

                set
                {
                    _OpenYMZ280B = value;
                }
            }

            private Point[] _PosC352 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosC352
            {
                get
                {
                    return _PosC352;
                }

                set
                {
                    _PosC352 = value;
                }
            }

            private bool[] _OpenC352 = new bool[2] { false, false };
            public bool[] OpenC352
            {
                get
                {
                    return _OpenC352;
                }

                set
                {
                    _OpenC352 = value;
                }
            }

            private Point[] _PosMultiPCM = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosMultiPCM
            {
                get
                {
                    return _PosMultiPCM;
                }

                set
                {
                    _PosMultiPCM = value;
                }
            }

            private bool[] _OpenMultiPCM = new bool[2] { false, false };
            public bool[] OpenMultiPCM
            {
                get
                {
                    return _OpenMultiPCM;
                }

                set
                {
                    _OpenMultiPCM = value;
                }
            }

            private bool[] _OpenQSound = new bool[2] { false, false };
            public bool[] OpenQSound
            {
                get
                {
                    return _OpenQSound;
                }

                set
                {
                    _OpenQSound = value;
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

            private Point[] _PosMIDI = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosMIDI
            {
                get
                {
                    return _PosMIDI;
                }

                set
                {
                    _PosMIDI = value;
                }
            }

            private bool[] _OpenMIDI = new bool[2] { false, false };
            public bool[] OpenMIDI
            {
                get
                {
                    return _OpenMIDI;
                }

                set
                {
                    _OpenMIDI = value;
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

            private Point[] _PosAY8910 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosAY8910
            {
                get
                {
                    return _PosAY8910;
                }

                set
                {
                    _PosAY8910 = value;
                }
            }

            private bool[] _OpenAY8910 = new bool[2] { false, false };
            public bool[] OpenAY8910
            {
                get
                {
                    return _OpenAY8910;
                }

                set
                {
                    _OpenAY8910 = value;
                }
            }

            private Point[] _PosHuC6280 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosHuC6280
            {
                get
                {
                    return _PosHuC6280;
                }

                set
                {
                    _PosHuC6280 = value;
                }
            }

            private bool[] _OpenHuC6280 = new bool[2] { false, false };
            public bool[] OpenHuC6280
            {
                get
                {
                    return _OpenHuC6280;
                }

                set
                {
                    _OpenHuC6280 = value;
                }
            }

            private Point[] _PosK051649 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosK051649
            {
                get
                {
                    return _PosK051649;
                }

                set
                {
                    _PosK051649 = value;
                }
            }

            private bool[] _OpenK051649 = new bool[2] { false, false };
            public bool[] OpenK051649
            {
                get
                {
                    return _OpenK051649;
                }

                set
                {
                    _OpenK051649 = value;
                }
            }

            private Point[] _PosYm2413 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm2413
            {
                get
                {
                    return _PosYm2413;
                }

                set
                {
                    _PosYm2413 = value;
                }
            }

            private bool[] _OpenYm2413 = new bool[2] { false, false };
            public bool[] OpenYm2413
            {
                get
                {
                    return _OpenYm2413;
                }

                set
                {
                    _OpenYm2413 = value;
                }
            }

            private Point[] _PosYm3526 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm3526
            {
                get
                {
                    return _PosYm3526;
                }

                set
                {
                    _PosYm3526 = value;
                }
            }

            private bool[] _OpenYm3526 = new bool[2] { false, false };
            public bool[] OpenYm3526
            {
                get
                {
                    return _OpenYm3526;
                }

                set
                {
                    _OpenYm3526 = value;
                }
            }

            private Point[] _PosY8950 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosY8950
            {
                get
                {
                    return _PosY8950;
                }

                set
                {
                    _PosY8950 = value;
                }
            }

            private bool[] _OpenY8950 = new bool[2] { false, false };
            public bool[] OpenY8950
            {
                get
                {
                    return _OpenY8950;
                }

                set
                {
                    _OpenY8950 = value;
                }
            }

            private Point[] _PosYm3812 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYm3812
            {
                get
                {
                    return _PosYm3812;
                }

                set
                {
                    _PosYm3812 = value;
                }
            }

            private bool[] _OpenYm3812 = new bool[2] { false, false };
            public bool[] OpenYm3812
            {
                get
                {
                    return _OpenYm3812;
                }

                set
                {
                    _OpenYm3812 = value;
                }
            }

            private Point[] _PosYmf2762 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYmf262
            {
                get
                {
                    return _PosYmf278b;
                }

                set
                {
                    _PosYmf278b = value;
                }
            }

            private bool[] _OpenYmf262 = new bool[2] { false, false };
            public bool[] OpenYmf262
            {
                get
                {
                    return _OpenYmf262;
                }

                set
                {
                    _OpenYmf262 = value;
                }
            }

            private Point[] _PosYmf278b = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosYmf278b
            {
                get
                {
                    return _PosYmf278b;
                }

                set
                {
                    _PosYmf278b = value;
                }
            }

            private bool[] _OpenYmf278b = new bool[2] { false, false };
            public bool[] OpenYmf278b
            {
                get
                {
                    return _OpenYmf278b;
                }

                set
                {
                    _OpenYmf278b = value;
                }
            }

            private Point _PosYm2612MIDI = Point.Empty;
            public Point PosYm2612MIDI
            {
                get
                {
                    return _PosYm2612MIDI;
                }

                set
                {
                    _PosYm2612MIDI = value;
                }
            }

            private bool _OpenYm2612MIDI = false;
            public bool OpenYm2612MIDI
            {
                get
                {
                    return _OpenYm2612MIDI;
                }

                set
                {
                    _OpenYm2612MIDI = value;
                }
            }

            private Point _PosMixer = Point.Empty;
            public Point PosMixer
            {
                get
                {
                    return _PosMixer;
                }

                set
                {
                    _PosMixer = value;
                }
            }

            private bool _OpenMixer = false;
            public bool OpenMixer
            {
                get
                {
                    return _OpenMixer;
                }

                set
                {
                    _OpenMixer = value;
                }
            }

            private Point _PosVSTeffectList = Point.Empty;
            public Point PosVSTeffectList
            {
                get
                {
                    return _PosVSTeffectList;
                }

                set
                {
                    _PosVSTeffectList = value;
                }
            }

            private bool _OpenVSTeffectList = false;
            public bool OpenVSTeffectList
            {
                get
                {
                    return _OpenVSTeffectList;
                }

                set
                {
                    _OpenVSTeffectList = value;
                }
            }

            private Point[] _PosNESDMC = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosNESDMC
            {
                get
                {
                    return _PosNESDMC;
                }

                set
                {
                    _PosNESDMC = value;
                }
            }

            private bool[] _OpenNESDMC = new bool[2] { false, false };
            public bool[] OpenNESDMC
            {
                get
                {
                    return _OpenNESDMC;
                }

                set
                {
                    _OpenNESDMC = value;
                }
            }

            private Point[] _PosFDS = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosFDS
            {
                get
                {
                    return _PosFDS;
                }

                set
                {
                    _PosFDS = value;
                }
            }

            private bool[] _OpenFDS = new bool[2] { false, false };
            public bool[] OpenFDS
            {
                get
                {
                    return _OpenFDS;
                }

                set
                {
                    _OpenFDS = value;
                }
            }

            private Point[] _PosMMC5 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosMMC5
            {
                get
                {
                    return _PosMMC5;
                }

                set
                {
                    _PosMMC5 = value;
                }
            }

            private bool[] _OpenMMC5 = new bool[2] { false, false };
            public bool[] OpenMMC5
            {
                get
                {
                    return _OpenMMC5;
                }

                set
                {
                    _OpenMMC5 = value;
                }
            }

            private Point[] _PosVrc6 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosVrc6
            {
                get
                {
                    return _PosVrc6;
                }

                set
                {
                    _PosVrc6 = value;
                }
            }

            private bool[] _OpenVrc6 = new bool[2] { false, false };
            public bool[] OpenVrc6
            {
                get
                {
                    return _OpenVrc6;
                }

                set
                {
                    _OpenVrc6 = value;
                }
            }

            private Point[] _PosVrc7 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosVrc7
            {
                get
                {
                    return _PosVrc7;
                }

                set
                {
                    _PosVrc7 = value;
                }
            }

            private bool[] _OpenVrc7 = new bool[2] { false, false };
            public bool[] OpenVrc7
            {
                get
                {
                    return _OpenVrc7;
                }

                set
                {
                    _OpenVrc7 = value;
                }
            }

            private Point[] _PosN106 = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosN106
            {
                get
                {
                    return _PosN106;
                }

                set
                {
                    _PosN106 = value;
                }
            }

            private bool[] _OpenN106 = new bool[2] { false, false };
            public bool[] OpenN106
            {
                get
                {
                    return _OpenN106;
                }

                set
                {
                    _OpenN106 = value;
                }
            }

            private Point[] _PosQSound = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosQSound
            {
                get
                {
                    return _PosQSound;
                }

                set
                {
                    _PosQSound = value;
                }
            }


            private Point[] _PosRegTest = new Point[2] { Point.Empty, Point.Empty };
            public Point[] PosRegTest
            {
                get { return _PosRegTest; }
                set { _PosRegTest = value; }
            }

            private bool[] _OpenRegTest = new bool[2] { false, false };
            public bool[] OpenRegTest
            {
                get
                {
                    return _OpenRegTest;
                }

                set
                {
                    _OpenRegTest = value;
                }
            }

            public int ChipSelect { get; set; }

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
                Location.PosMixer = this.PosMixer;
                Location.OpenMixer = this.OpenMixer;
                Location.PosRf5c164 = this.PosRf5c164;
                Location.OpenRf5c164 = this.OpenRf5c164;
                Location.PosRf5c68 = this.PosRf5c68;
                Location.OpenRf5c68 = this.OpenRf5c68;
                Location.PosC140 = this.PosC140;
                Location.OpenC140 = this.OpenC140;
                Location.PosPPZ8 = this.PosPPZ8;
                Location.OpenPPZ8 = this.OpenPPZ8;
                Location.PosS5B = this.PosS5B;
                Location.OpenS5B = this.OpenS5B;
                Location.PosDMG = this.PosDMG;
                Location.OpenDMG = this.OpenDMG;
                Location.PosYMZ280B = this.PosYMZ280B;
                Location.OpenYMZ280B = this.OpenYMZ280B;
                Location.PosC352 = this.PosC352;
                Location.OpenC352 = this.OpenC352;
                Location.PosQSound = this.PosQSound;
                Location.OpenQSound = this.OpenQSound;
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
                Location.PosYm3526 = this.PosYm3526;
                Location.OpenYm3526 = this.OpenYm3526;
                Location.PosY8950 = this.PosY8950;
                Location.OpenY8950 = this.OpenY8950;
                Location.PosYm3812 = this.PosYm3812;
                Location.OpenYm3812 = this.OpenYm3812;
                Location.PosYmf262 = this.PosYmf262;
                Location.OpenYmf262 = this.OpenYmf262;
                Location.PosYMF271 = this.PosYMF271;
                Location.OpenYMF271 = this.OpenYMF271;
                Location.PosYmf278b = this.PosYmf278b;
                Location.OpenYmf278b = this.OpenYmf278b;
                Location.PosOKIM6258 = this.PosOKIM6258;
                Location.OpenOKIM6258 = this.OpenOKIM6258;
                Location.PosOKIM6295 = this.PosOKIM6295;
                Location.OpenOKIM6295 = this.OpenOKIM6295;
                Location.PosSN76489 = this.PosSN76489;
                Location.OpenSN76489 = this.OpenSN76489;
                Location.PosSegaPCM = this.PosSegaPCM;
                Location.OpenSegaPCM = this.OpenSegaPCM;
                Location.PosAY8910 = this.PosAY8910;
                Location.OpenAY8910 = this.OpenAY8910;
                Location.PosHuC6280 = this.PosHuC6280;
                Location.OpenHuC6280 = this.OpenHuC6280;
                Location.PosK051649 = this.PosK051649;
                Location.OpenK051649 = this.OpenK051649;
                Location.PosYm2612MIDI = this.PosYm2612MIDI;
                Location.OpenYm2612MIDI = this.OpenYm2612MIDI;
                Location.PosVSTeffectList = this.PosVSTeffectList;
                Location.OpenVSTeffectList = this.OpenVSTeffectList;
                Location.PosVrc7 = this.PosVrc7;
                Location.OpenVrc7 = this.OpenVrc7;
                Location.PosMIDI = this.PosMIDI;
                Location.OpenMIDI = this.OpenMIDI;
                Location.PosRegTest = this.PosRegTest;
                Location.OpenRegTest = this.OpenRegTest;
                Location.ChipSelect = this.ChipSelect;

                return Location;
            }
        }

        [Serializable]
        public class MidiExport
        {

            private bool _UseMIDIExport = false;
            public bool UseMIDIExport
            {
                get
                {
                    return _UseMIDIExport;
                }

                set
                {
                    _UseMIDIExport = value;
                }
            }

            private bool _UseYM2151Export = false;
            public bool UseYM2151Export
            {
                get
                {
                    return _UseYM2151Export;
                }

                set
                {
                    _UseYM2151Export = value;
                }
            }

            private bool _UseYM2612Export = true;
            public bool UseYM2612Export
            {
                get
                {
                    return _UseYM2612Export;
                }

                set
                {
                    _UseYM2612Export = value;
                }
            }

            private string _ExportPath = "";
            public string ExportPath
            {
                get
                {
                    return _ExportPath;
                }

                set
                {
                    _ExportPath = value;
                }
            }

            private bool _UseVOPMex = false;
            public bool UseVOPMex
            {
                get
                {
                    return _UseVOPMex;
                }

                set
                {
                    _UseVOPMex = value;
                }
            }

            private bool _KeyOnFnum = false;
            public bool KeyOnFnum
            {
                get
                {
                    return _KeyOnFnum;
                }

                set
                {
                    _KeyOnFnum = value;
                }
            }


            public MidiExport Copy()
            {
                MidiExport MidiExport = new MidiExport();

                MidiExport.UseMIDIExport = this.UseMIDIExport;
                MidiExport.UseYM2151Export = this.UseYM2151Export;
                MidiExport.UseYM2612Export = this.UseYM2612Export;
                MidiExport.ExportPath = this.ExportPath;
                MidiExport.UseVOPMex = this.UseVOPMex;
                MidiExport.KeyOnFnum = this.KeyOnFnum;

                return MidiExport;
            }

        }

        [Serializable]
        public class MidiKbd
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

            private string _MidiInDeviceName = "";
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

            private bool _IsMONO = true;
            public bool IsMONO
            {
                get
                {
                    return _IsMONO;
                }

                set
                {
                    _IsMONO = value;
                }
            }

            private int _useFormat = 0;
            public int UseFormat
            {
                get
                {
                    return _useFormat;
                }

                set
                {
                    _useFormat = value;
                }
            }

            private int _UseMONOChannel = 0;
            public int UseMONOChannel
            {
                get
                {
                    return _UseMONOChannel;
                }

                set
                {
                    _UseMONOChannel = value;
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

            private Tone[] _Tones = new Tone[6];
            public Tone[] Tones
            {
                get
                {
                    return _Tones;
                }

                set
                {
                    _Tones = value;
                }
            }

            private int _MidiCtrl_CopyToneFromYM2612Ch1 = 97;
            public int MidiCtrl_CopyToneFromYM2612Ch1
            {
                get
                {
                    return _MidiCtrl_CopyToneFromYM2612Ch1;
                }

                set
                {
                    _MidiCtrl_CopyToneFromYM2612Ch1 = value;
                }
            }

            private int _MidiCtrl_DelOneLog = 96;
            public int MidiCtrl_DelOneLog
            {
                get
                {
                    return _MidiCtrl_DelOneLog;
                }

                set
                {
                    _MidiCtrl_DelOneLog = value;
                }
            }

            private int _MidiCtrl_CopySelecttingLogToClipbrd = 66;
            public int MidiCtrl_CopySelecttingLogToClipbrd
            {
                get
                {
                    return _MidiCtrl_CopySelecttingLogToClipbrd;
                }

                set
                {
                    _MidiCtrl_CopySelecttingLogToClipbrd = value;
                }
            }

            private int _MidiCtrl_Stop = -1;
            public int MidiCtrl_Stop
            {
                get
                {
                    return _MidiCtrl_Stop;
                }

                set
                {
                    _MidiCtrl_Stop = value;
                }
            }

            private int _MidiCtrl_Pause = -1;
            public int MidiCtrl_Pause
            {
                get
                {
                    return _MidiCtrl_Pause;
                }

                set
                {
                    _MidiCtrl_Pause = value;
                }
            }

            private int _MidiCtrl_Fadeout = -1;
            public int MidiCtrl_Fadeout
            {
                get
                {
                    return _MidiCtrl_Fadeout;
                }

                set
                {
                    _MidiCtrl_Fadeout = value;
                }
            }

            private int _MidiCtrl_Previous = -1;
            public int MidiCtrl_Previous
            {
                get
                {
                    return _MidiCtrl_Previous;
                }

                set
                {
                    _MidiCtrl_Previous = value;
                }
            }

            private int _MidiCtrl_Slow = -1;
            public int MidiCtrl_Slow
            {
                get
                {
                    return _MidiCtrl_Slow;
                }

                set
                {
                    _MidiCtrl_Slow = value;
                }
            }

            private int _MidiCtrl_Play = -1;
            public int MidiCtrl_Play
            {
                get
                {
                    return _MidiCtrl_Play;
                }

                set
                {
                    _MidiCtrl_Play = value;
                }
            }

            private int _MidiCtrl_Fast = -1;
            public int MidiCtrl_Fast
            {
                get
                {
                    return _MidiCtrl_Fast;
                }

                set
                {
                    _MidiCtrl_Fast = value;
                }
            }

            private int _MidiCtrl_Next = -1;
            public int MidiCtrl_Next
            {
                get
                {
                    return _MidiCtrl_Next;
                }

                set
                {
                    _MidiCtrl_Next = value;
                }
            }

            public MidiKbd Copy()
            {
                MidiKbd midiKbd = new MidiKbd();

                midiKbd.MidiInDeviceName = this.MidiInDeviceName;
                midiKbd.UseMIDIKeyboard = this.UseMIDIKeyboard;
                for (int i = 0; i < midiKbd.UseChannel.Length; i++)
                {
                    midiKbd.UseChannel[i] = this.UseChannel[i];
                }
                midiKbd.IsMONO = this.IsMONO;
                midiKbd.UseMONOChannel = this.UseMONOChannel;

                midiKbd.MidiCtrl_CopySelecttingLogToClipbrd = this.MidiCtrl_CopySelecttingLogToClipbrd;
                midiKbd.MidiCtrl_CopyToneFromYM2612Ch1 = this.MidiCtrl_CopyToneFromYM2612Ch1;
                midiKbd.MidiCtrl_DelOneLog = this.MidiCtrl_DelOneLog;
                midiKbd.MidiCtrl_Fadeout = this.MidiCtrl_Fadeout;
                midiKbd.MidiCtrl_Fast = this.MidiCtrl_Fast;
                midiKbd.MidiCtrl_Next = this.MidiCtrl_Next;
                midiKbd.MidiCtrl_Pause = this.MidiCtrl_Pause;
                midiKbd.MidiCtrl_Play = this.MidiCtrl_Play;
                midiKbd.MidiCtrl_Previous = this.MidiCtrl_Previous;
                midiKbd.MidiCtrl_Slow = this.MidiCtrl_Slow;
                midiKbd.MidiCtrl_Stop = this.MidiCtrl_Stop;

                return midiKbd;
            }
        }

        [Serializable]
        public class KeyBoardHook
        {
            [Serializable]
            public class HookKeyInfo
            {
                private bool _Shift = false;
                private bool _Ctrl = false;
                private bool _Win = false;
                private bool _Alt = false;
                private string _Key = "(None)";

                public bool Shift { get => _Shift; set => _Shift = value; }
                public bool Ctrl { get => _Ctrl; set => _Ctrl = value; }
                public bool Win { get => _Win; set => _Win = value; }
                public bool Alt { get => _Alt; set => _Alt = value; }
                public string Key { get => _Key; set => _Key = value; }

                public HookKeyInfo Copy()
                {
                    HookKeyInfo hookKeyInfo = new HookKeyInfo();
                    hookKeyInfo.Shift = this.Shift;
                    hookKeyInfo.Ctrl = this.Ctrl;
                    hookKeyInfo.Win = this.Win;
                    hookKeyInfo.Alt = this.Alt;
                    hookKeyInfo.Key = this.Key;

                    return hookKeyInfo;
                }
            }

            private bool _UseKeyBoardHook = false;
            public bool UseKeyBoardHook
            {
                get
                {
                    return _UseKeyBoardHook;
                }

                set
                {
                    _UseKeyBoardHook = value;
                }
            }

            public HookKeyInfo Stop { get => _Stop; set => _Stop = value; }
            public HookKeyInfo Pause { get => _Pause; set => _Pause = value; }
            public HookKeyInfo Fadeout { get => _Fadeout; set => _Fadeout = value; }
            public HookKeyInfo Prev { get => _Prev; set => _Prev = value; }
            public HookKeyInfo Slow { get => _Slow; set => _Slow = value; }
            public HookKeyInfo Play { get => _Play; set => _Play = value; }
            public HookKeyInfo Next { get => _Next; set => _Next = value; }
            public HookKeyInfo Fast { get => _Fast; set => _Fast = value; }
            private HookKeyInfo _Stop = new HookKeyInfo();
            private HookKeyInfo _Pause = new HookKeyInfo();
            private HookKeyInfo _Fadeout = new HookKeyInfo();
            private HookKeyInfo _Prev = new HookKeyInfo();
            private HookKeyInfo _Slow = new HookKeyInfo();
            private HookKeyInfo _Play = new HookKeyInfo();
            private HookKeyInfo _Next = new HookKeyInfo();
            private HookKeyInfo _Fast = new HookKeyInfo();

            public KeyBoardHook Copy()
            {
                KeyBoardHook keyBoard = new KeyBoardHook();
                keyBoard.UseKeyBoardHook = this.UseKeyBoardHook;
                keyBoard.Stop = this.Stop.Copy();
                keyBoard.Pause = this.Pause.Copy();
                keyBoard.Fadeout = this.Fadeout.Copy();
                keyBoard.Prev = this.Prev.Copy();
                keyBoard.Slow = this.Slow.Copy();
                keyBoard.Play = this.Play.Copy();
                keyBoard.Next = this.Next.Copy();
                keyBoard.Fast = this.Fast.Copy();

                return keyBoard;
            }
        }

        [Serializable]
        public class Vst
        {
            private string _DefaultPath = "";

            private string[] _VSTPluginPath = null;
            public string[] VSTPluginPath
            {
                get
                {
                    return _VSTPluginPath;
                }

                set
                {
                    _VSTPluginPath = value;
                }
            }


            private vstInfo[] _VSTInfo = null;
            public vstInfo[] VSTInfo
            {
                get
                {
                    return _VSTInfo;
                }

                set
                {
                    _VSTInfo = value;
                }
            }

            public string DefaultPath
            {
                get
                {
                    return _DefaultPath;
                }

                set
                {
                    _DefaultPath = value;
                }
            }

            public Vst Copy()
            {
                Vst vst = new Vst();

                vst.VSTInfo = this.VSTInfo;
                vst.DefaultPath = this.DefaultPath;

                return vst;
            }

        }

        [Serializable]
        public class MidiOut
        {
            private string _GMReset = "30:F0,7E,7F,09,01,F7";
            public string GMReset { get => _GMReset; set => _GMReset = value; }

            private string _XGReset = "30:F0,43,10,4C,00,00,7E,00,F7";
            public string XGReset { get => _XGReset; set => _XGReset = value; }

            private string _GSReset = "30:F0,41,10,42,12,40,00,7F,00,41,F7";
            public string GSReset { get => _GSReset; set => _GSReset = value; }

            private string _Custom = "";
            public string Custom { get => _Custom; set => _Custom = value; }

            private List<midiOutInfo[]> _lstMidiOutInfo = null;
            public List<midiOutInfo[]> lstMidiOutInfo
            {
                get
                {
                    return _lstMidiOutInfo;
                }
                set
                {
                    _lstMidiOutInfo = value;
                }
            }

            public MidiOut Copy()
            {
                MidiOut MidiOut = new MidiOut();

                MidiOut.GMReset = this.GMReset;
                MidiOut.XGReset = this.XGReset;
                MidiOut.GSReset = this.GSReset;
                MidiOut.Custom = this.Custom;
                MidiOut.lstMidiOutInfo = this.lstMidiOutInfo;

                return MidiOut;
            }

        }

        [Serializable]
        public class NSF
        {
            private bool _NESUnmuteOnReset = true;
            private bool _NESNonLinearMixer = true;
            private bool _NESPhaseRefresh = true;
            private bool _NESDutySwap = false;

            private int _FDSLpf = 2000;
            private bool _FDS4085Reset = false;
            private bool _FDSWriteDisable8000 = true;

            private bool _DMCUnmuteOnReset = true;
            private bool _DMCNonLinearMixer = true;
            private bool _DMCEnable4011 = true;
            private bool _DMCEnablePnoise = true;
            private bool _DMCDPCMAntiClick = false;
            private bool _DMCRandomizeNoise = true;
            private bool _DMCTRImute = true;
            private bool _DMCTRINull = true;

            private bool _MMC5NonLinearMixer = true;
            private bool _MMC5PhaseRefresh = true;

            private bool _N160Serial = false;

            public bool NESUnmuteOnReset
            {
                get
                {
                    return _NESUnmuteOnReset;
                }

                set
                {
                    _NESUnmuteOnReset = value;
                }
            }
            public bool NESNonLinearMixer
            {
                get
                {
                    return _NESNonLinearMixer;
                }

                set
                {
                    _NESNonLinearMixer = value;
                }
            }
            public bool NESPhaseRefresh
            {
                get
                {
                    return _NESPhaseRefresh;
                }

                set
                {
                    _NESPhaseRefresh = value;
                }
            }
            public bool NESDutySwap
            {
                get
                {
                    return _NESDutySwap;
                }

                set
                {
                    _NESDutySwap = value;
                }
            }

            public int FDSLpf
            {
                get
                {
                    return _FDSLpf;
                }

                set
                {
                    _FDSLpf = value;
                }
            }
            public bool FDS4085Reset
            {
                get
                {
                    return _FDS4085Reset;
                }

                set
                {
                    _FDS4085Reset = value;
                }
            }
            public bool FDSWriteDisable8000
            {
                get
                {
                    return _FDSWriteDisable8000;
                }
                set
                {
                    _FDSWriteDisable8000 = value;
                }
            }

            public bool DMCUnmuteOnReset
            {
                get
                {
                    return _DMCUnmuteOnReset;
                }

                set
                {
                    _DMCUnmuteOnReset = value;
                }
            }
            public bool DMCNonLinearMixer
            {
                get
                {
                    return _DMCNonLinearMixer;
                }

                set
                {
                    _DMCNonLinearMixer = value;
                }
            }
            public bool DMCEnable4011
            {
                get
                {
                    return _DMCEnable4011;
                }

                set
                {
                    _DMCEnable4011 = value;
                }
            }
            public bool DMCEnablePnoise
            {
                get
                {
                    return _DMCEnablePnoise;
                }

                set
                {
                    _DMCEnablePnoise = value;
                }
            }
            public bool DMCDPCMAntiClick
            {
                get
                {
                    return _DMCDPCMAntiClick;
                }

                set
                {
                    _DMCDPCMAntiClick = value;
                }
            }
            public bool DMCRandomizeNoise
            {
                get
                {
                    return _DMCRandomizeNoise;
                }

                set
                {
                    _DMCRandomizeNoise = value;
                }
            }
            public bool DMCTRImute
            {
                get
                {
                    return _DMCTRImute;
                }

                set
                {
                    _DMCTRImute = value;
                }
            }
            public bool DMCTRINull
            {
                get
                {
                    return _DMCTRINull;
                }

                set
                {
                    _DMCTRINull = value;
                }
            }

            public bool MMC5NonLinearMixer
            {
                get
                {
                    return _MMC5NonLinearMixer;
                }

                set
                {
                    _MMC5NonLinearMixer = value;
                }
            }
            public bool MMC5PhaseRefresh
            {
                get
                {
                    return _MMC5PhaseRefresh;
                }

                set
                {
                    _MMC5PhaseRefresh = value;
                }
            }

            public bool N160Serial
            {
                get
                {
                    return _N160Serial;
                }

                set
                {
                    _N160Serial = value;
                }
            }

            public int _HPF = 92;
            public int HPF {
                get
                {
                    return _HPF;
                }

                set
                {
                    _HPF = value;
                }
            }

            public int _LPF = 112;
            public int LPF {
                get
                {
                    return _LPF;
                }

                set
                {
                    _LPF = value;
                }
            }

            public NSF Copy()
            {
                NSF NSF = new NSF();

                NSF.NESUnmuteOnReset = this.NESUnmuteOnReset;
                NSF.NESNonLinearMixer = this.NESNonLinearMixer;
                NSF.NESPhaseRefresh = this.NESPhaseRefresh;
                NSF.NESDutySwap = this.NESDutySwap;

                NSF.FDSLpf = this.FDSLpf;
                NSF.FDS4085Reset = this.FDS4085Reset;
                NSF.FDSWriteDisable8000 = this.FDSWriteDisable8000;

                NSF.DMCUnmuteOnReset = this.DMCUnmuteOnReset;
                NSF.DMCNonLinearMixer = this.DMCNonLinearMixer;
                NSF.DMCEnable4011 = this.DMCEnable4011;
                NSF.DMCEnablePnoise = this.DMCEnablePnoise;
                NSF.DMCDPCMAntiClick = this.DMCDPCMAntiClick;
                NSF.DMCRandomizeNoise = this.DMCRandomizeNoise;
                NSF.DMCTRImute = this.DMCTRImute;
                NSF.DMCTRINull = this.DMCTRINull;

                NSF.MMC5NonLinearMixer = this.MMC5NonLinearMixer;
                NSF.MMC5PhaseRefresh = this.MMC5PhaseRefresh;

                NSF.N160Serial = this.N160Serial;

                NSF.HPF = this.HPF;
                NSF.LPF = this.LPF;

                return NSF;
            }

        }

        [Serializable]
        public class SID
        {
            public string RomKernalPath = "";
            public string RomBasicPath = "";
            public string RomCharacterPath = "";
            public int Quality = 1;
            public int OutputBufferSize = 5000;
            public int c64model = 0;
            public bool c64modelForce = false;
            public int sidmodel = 0;
            public bool sidmodelForce = false;

            public SID Copy()
            {
                SID SID = new SID();

                SID.RomKernalPath = this.RomKernalPath;
                SID.RomBasicPath = this.RomBasicPath;
                SID.RomCharacterPath = this.RomCharacterPath;
                SID.Quality = this.Quality;
                SID.OutputBufferSize = this.OutputBufferSize;
                SID.c64model = this.c64model;
                SID.c64modelForce = this.c64modelForce;
                SID.sidmodel = this.sidmodel;
                SID.sidmodelForce = this.sidmodelForce;

                return SID;
            }
        }

        [Serializable]
        public class NukedOPN2
        {
            public int EmuType = 0;
            //ごめんGensのオプションもここ。。。
            public bool GensDACHPF = true;
            public bool GensSSGEG = true;

            public NukedOPN2 Copy()
            {
                NukedOPN2 no = new NukedOPN2();
                no.EmuType = this.EmuType;
                no.GensDACHPF = this.GensDACHPF;
                no.GensSSGEG = this.GensSSGEG;

                return no;
            }
        }

        [Serializable]
        public class AutoBalance
        {
            private bool _UseThis = false;
            private bool _LoadSongBalance = false;
            private bool _LoadDriverBalance = false;
            private bool _SaveSongBalance = false;
            private bool _SamePositionAsSongData = false;

            public bool UseThis { get => _UseThis; set => _UseThis = value; }
            public bool LoadSongBalance { get => _LoadSongBalance; set => _LoadSongBalance = value; }
            public bool LoadDriverBalance { get => _LoadDriverBalance; set => _LoadDriverBalance = value; }
            public bool SaveSongBalance { get => _SaveSongBalance; set => _SaveSongBalance = value; }
            public bool SamePositionAsSongData { get => _SamePositionAsSongData; set => _SamePositionAsSongData = value; }

            public AutoBalance Copy()
            {
                AutoBalance AutoBalance = new AutoBalance();
                AutoBalance.UseThis = this.UseThis;
                AutoBalance.LoadSongBalance = this.LoadSongBalance;
                AutoBalance.LoadDriverBalance = this.LoadDriverBalance;
                AutoBalance.SaveSongBalance = this.SaveSongBalance;
                AutoBalance.SamePositionAsSongData = this.SamePositionAsSongData;

                return AutoBalance;
            }
        }

        [Serializable]
        public class PMDDotNET
        {
            public string compilerArguments = "/v /C";
            public bool isAuto = true;
            public int soundBoard = 1;
            public bool usePPSDRV = true;
            public bool usePPZ8 = true;
            public string driverArguments = "";
            public bool setManualVolume = false;
            public bool usePPSDRVUseInterfaceDefaultFreq = true;
            public int PPSDRVManualFreq = 2000;
            public int PPSDRVManualWait = 1;
            public int volumeFM = 0;
            public int volumeSSG = 0;
            public int volumeRhythm = 0;
            public int volumeAdpcm = 0;
            public int volumeGIMICSSG = 31;

            public PMDDotNET Copy()
            {
                PMDDotNET p = new PMDDotNET();
                p.compilerArguments = this.compilerArguments;
                p.isAuto = this.isAuto;
                p.soundBoard = this.soundBoard;
                p.usePPSDRV = this.usePPSDRV;
                p.usePPZ8 = this.usePPZ8;
                p.driverArguments = this.driverArguments;
                p.setManualVolume = this.setManualVolume;
                p.usePPSDRVUseInterfaceDefaultFreq = this.usePPSDRVUseInterfaceDefaultFreq;
                p.PPSDRVManualFreq = this.PPSDRVManualFreq;
                p.PPSDRVManualWait = this.PPSDRVManualWait;
                p.volumeFM = this.volumeFM;
                p.volumeSSG = this.volumeSSG;
                p.volumeRhythm = this.volumeRhythm;
                p.volumeAdpcm = this.volumeAdpcm;
                p.volumeGIMICSSG = this.volumeGIMICSSG;

                return p;
            }
        }




        public Setting Copy()
        {
            Setting setting = new Setting();
            setting.outputDevice = this.outputDevice.Copy();

            setting.YM2151Type = this.YM2151Type.Copy();
            setting.YM2203Type = this.YM2203Type.Copy();
            setting.YM2413Type = this.YM2413Type.Copy();
            setting.YM2608Type = this.YM2608Type.Copy();
            setting.YM2610Type = this.YM2610Type.Copy();
            setting.YM2612Type = this.YM2612Type.Copy();
            setting.YM3526Type = this.YM3526Type.Copy();
            setting.YM3812Type = this.YM3812Type.Copy();
            setting.YMF262Type = this.YMF262Type.Copy();
            setting.SN76489Type = this.SN76489Type.Copy();
            setting.C140Type = this.C140Type.Copy();
            setting.SEGAPCMType = this.SEGAPCMType.Copy();

            setting.YM2151SType = this.YM2151SType.Copy();
            setting.YM2203SType = this.YM2203SType.Copy();
            setting.YM2413SType = this.YM2413SType.Copy();
            setting.YM2608SType = this.YM2608SType.Copy();
            setting.YM2610SType = this.YM2610SType.Copy();
            setting.YM2612SType = this.YM2612SType.Copy();
            setting.YM3526SType = this.YM3526SType.Copy();
            setting.YM3812SType = this.YM3812SType.Copy();
            setting.YMF262SType = this.YMF262SType.Copy();
            setting.SN76489SType = this.SN76489SType.Copy();
            setting.C140SType = this.C140SType.Copy();
            setting.SEGAPCMSType = this.SEGAPCMSType.Copy();

            setting.other = this.other.Copy();
            setting.balance = this.balance.Copy();
            setting.LatencyEmulation = this.LatencyEmulation;
            setting.LatencySCCI = this.LatencySCCI;
            setting.Debug_DispFrameCounter = this.Debug_DispFrameCounter;
            setting.HiyorimiMode = this.HiyorimiMode;
            setting.location = this.location.Copy();
            setting.midiExport = this.midiExport.Copy();
            setting.midiKbd = this.midiKbd.Copy();
            setting.vst = this.vst.Copy();
            setting.midiOut = this.midiOut.Copy();
            setting.nsf = this.nsf.Copy();
            setting.sid = this.sid.Copy();
            setting.nukedOPN2 = this.nukedOPN2.Copy();
            setting.autoBalance = this.autoBalance.Copy();
            setting.pmdDotNET = this.pmdDotNET.Copy();

            setting.keyBoardHook = this.keyBoardHook.Copy();

            return setting;
        }

        public void Save()
        {
            string fullPath = Common.settingFilePath;
            fullPath = Path.Combine(fullPath, Resources.cntSettingFileName);

            XmlSerializer serializer = new XmlSerializer(typeof(Setting), typeof(Setting).GetNestedTypes());
            using (StreamWriter sw = new StreamWriter(fullPath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, this);
            }
        }

        public static Setting Load()
        {
            try
            {
                string fn = Resources.cntSettingFileName;
                if (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), fn)))
                {
                    Common.settingFilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                }
                else
                {
                    Common.settingFilePath = Common.GetApplicationDataFolder(true);
                }

                string fullPath = Common.settingFilePath;
                fullPath = Path.Combine(fullPath, Resources.cntSettingFileName);

                if (!File.Exists(fullPath)) { return new Setting(); }
                XmlSerializer serializer = new XmlSerializer(typeof(Setting), typeof(Setting).GetNestedTypes());
                using (StreamReader sr = new StreamReader(fullPath, new UTF8Encoding(false)))
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

    }
}
