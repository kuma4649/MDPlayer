using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace MDPlayer
{
    [Serializable]
    public class MIDIEvent
    {
        private int? _BeforeIndex = null;
        private int? _AfterIndex = null;
        private int _Number = 0;
        private int _Step = 0;
        private int? _SameMeasureIndex = null;
        private MIDIEventType _EventType;
        private byte[] _MIDIMessage = null;
        private byte[][] _MIDIMessageLst = null;
        private int _Gate = 0;
        private int _gs = 0;

        public int? BeforeIndex
        {
            set
            {
                _BeforeIndex = value;
            }
            get
            {
                return _BeforeIndex;
            }
        }
        public int? AfterIndex
        {
            set
            {
                _AfterIndex = value;
            }
            get
            {
                return _AfterIndex;
            }
        }
        public int Number
        {
            set
            {
                _Number = value;
            }
            get
            {
                return _Number;
            }
        }
        public int? SameMeasureIndex
        {
            set
            {
                _SameMeasureIndex = value;
            }
            get
            {
                return _SameMeasureIndex;
            }
        }
        public int Step
        {
            set
            {
                _Step = value;
            }
            get
            {
                return _Step;
            }
        }
        public MIDIEventType EventType
        {
            set
            {
                _EventType = value;
            }
            get
            {
                return _EventType;
            }
        }
        public byte[] MIDIMessage
        {
            set
            {
                _MIDIMessage = value;
            }
            get
            {
                return _MIDIMessage;
            }
        }
        public byte[][] MIDIMessageLst
        {
            set
            {
                _MIDIMessageLst = value;
            }
            get
            {
                return _MIDIMessageLst;
            }
        }
        public int Gate
        {
            set
            {
                _Gate = value;
            }
            get
            {
                return _Gate;
            }
        }
        public int gosa
        {
            set
            {
                _gs = value;
            }
            get
            {
                return _gs;
            }
        }

    }

    public enum MIDIEventType : byte
    {
        MetaSeqNumber = 0x00,
        MetaTextEvent = 0x01,
        MetaCopyrightNotice = 0x02,
        MetaTrackName = 0x03,
        MetaInstrumentName = 0x04,
        MetaLyric = 0x05,
        MetaMarker = 0x06,
        MetaCuePoint = 0x07,
        MetaProgramName = 0x08,
        MetaDeviceName = 0x09,
        MetaChannelPrefix = 0x20,
        MetaPortPrefix = 0x21,
        MetaEndOfTrack = 0x2F,
        MetaTempo = 0x51,
        MetaSMPTEOffset = 0x54,
        MetaTimeSignature = 0x58,
        MetaKeySignature = 0x59,
        MetaSequencerSpecific = 0x7F,
        NoteOff = 0x80,
        NoteON = 0x90,
        KeyAfterTouch = 0xA0,
        ControlChange = 0xB0,
        ProgramChange = 0xC0,
        ChannelAfterTouch = 0xD0,
        PitchBend = 0xE0,
        SysExF0 = 0xF0,
        SysExF7 = 0xF7
    }

    public enum MIDISpEventType : byte
    {
        UserExclusive1 = 0x90,
        UserExclusive2 = 0x91,
        UserExclusive3 = 0x92,
        UserExclusive4 = 0x93,
        UserExclusive5 = 0x94,
        UserExclusive6 = 0x95,
        UserExclusive7 = 0x96,
        UserExclusive8 = 0x97,
        ChExclusive = 0x98,
        OutSideProc = 0x99,
        BankProgram = 0xE2,
        KeyScan = 0xE5,
        MIDICh =0xE6,
        TempoChange = 0xE7,
        RolandBase = 0xDD,
        RolandPara = 0xDE,
        RolandDevice = 0xDF,
        KeyChange = 0xF5,
        Comment = 0xF6,
        LoopEnd = 0xF8,
        LoopStart = 0xF9,
        SameMeasure = 0xFC,
        MeasureEnd = 0xFD,
        EndOfTrack = 0xFE
    }

    public class MIDIRythm
    {
        private string _Name = "";
        private int _Key = 0;
        private int _Gt = 1;
        public string Name
        {
            set
            {
                _Name = value;
            }
            get
            {
                return _Name;
            }
        }
        public int Key
        {
            set
            {
                _Key = value;
            }
            get
            {
                return _Key;
            }
        }
        public int Gt
        {
            set
            {
                _Gt = value;
            }
            get
            {
                return _Gt;
            }
        }
    }

    public class MIDIUserExclusive
    {
        private string _Name = "";
        private string _Memo = "";
        private byte[] _Exclusive = null;
        public string Name
        {
            set
            {
                _Name = value;
            }
            get
            {
                return _Name;
            }
        }
        public string Memo
        {
            set
            {
                _Memo = value;
            }
            get
            {
                return _Memo;
            }
        }
        public byte[] Exclusive
        {
            set
            {
                _Exclusive = value;
            }
            get
            {
                return _Exclusive;
            }
        }
    }

}
