using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace MDPlayer
{
    [Serializable]
    public class MIDITrack
    {
        private int? _BeforeIndex = null;
        private int? _AfterIndex = null;
        private int _Number = 0;
        private string _Name = "";
        private string _Memo = "";
        private string _Comment = "";
        private int? _OutDeviceNumber = 0;
        private string _OutDeviceName = "";
        private int? _OutUserDeviceNumber = 0;
        private string _OutUserDeviceName = "";
        private int? _OutChannel = 0;
        private int? _InDeviceNumber = 0;
        private string _InDeviceName = "";
        private int? _InUserDeviceNumber = 0;
        private string _InUserDeviceName = "";
        private int? _InChannel = 0;
        private bool _Solo = false;
        private bool _Mute = false;
        private Color _Color = Color.Black;
        private List<MIDIPart> _Part = new List<MIDIPart>();
        private List<MIDIRythm> _Rythm = new List<MIDIRythm>();
        private bool _RythmMode = false;
        private int _Key = 0;
        private int _St = 0;
        private int? _StartPartIndex = null;
        private int? _EndPartIndex = null;
        private int _NumberPart = 0;
        //private int _NowPartIndex = 0;
        private MIDIPart _NowPart = null;
        private int _NowTick = 0;
        private int _NextEventTick = 0;
        private bool _EndMark = false;
        private int _TrackNumber = 0;
        private Stack<MIDIEvent> _LoopTargetEvent = new Stack<MIDIEvent>();
        private int? _LoopOrSameTargetEventIndex = null;
        private int? _SameMeasure = null;
        private byte _RolandBase_gt = 0;
        private byte _RolandBase_vel = 0;
        private byte _RolandDev_gt = 0;
        private byte _RolandDev_vel = 0;
        private byte _RolandPara_gt = 0;
        private byte _RolandPara_vel = 0;
        private byte _YAMAHABase_gt = 0;
        private byte _YAMAHABase_vel = 0;
        private byte _YAMAHA_dev = 0;
        private byte _YAMAHA_model = 0;
        private byte _YAMAHAPara_gt = 0;
        private byte _YAMAHAPara_vel = 0;
        private int _KeySIG_SF = 0;
        private int _KeySIG_MI = 0;
        private int[] _NoteGateTime = new int[128];


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
        public string Comment
        {
            set
            {
                _Comment = value;
            }
            get
            {
                return _Comment;
            }
        }
        public int? OutDeviceNumber
        {
            set
            {
                _OutDeviceNumber = value;
            }
            get
            {
                return _OutDeviceNumber;
            }
        }
        public string OutDeviceName
        {
            set
            {
                _OutDeviceName = value;
            }
            get
            {
                return _OutDeviceName;
            }
        }
        public string OutUserDeviceName
        {
            set
            {
                _OutUserDeviceName = value;
            }
            get
            {
                return _OutUserDeviceName;
            }
        }
        public int? OutUserDeviceNumber
        {
            set
            {
                _OutUserDeviceNumber = value;
            }
            get
            {
                return _OutUserDeviceNumber;
            }
        }
        public int? OutChannel
        {
            set
            {
                _OutChannel = value;
            }
            get
            {
                return _OutChannel;
            }
        }
        public int? InDeviceNumber
        {
            set
            {
                _InDeviceNumber = value;
            }
            get
            {
                return _InDeviceNumber;
            }
        }
        public string InDeviceName
        {
            set
            {
                _InDeviceName = value;
            }
            get
            {
                return _InDeviceName;
            }
        }
        public int? InUserDeviceNumber
        {
            set
            {
                _InUserDeviceNumber = value;
            }
            get
            {
                return _InUserDeviceNumber;
            }
        }
        public string InUserDeviceName
        {
            set
            {
                _InUserDeviceName = value;
            }
            get
            {
                return _InUserDeviceName;
            }
        }
        public int? InChannel
        {
            set
            {
                _InChannel = value;
            }
            get
            {
                return _InChannel;
            }
        }
        public bool Solo
        {
            set
            {
                _Solo = value;
            }
            get
            {
                return _Solo;
            }
        }
        public bool Mute
        {
            set
            {
                _Mute = value;
            }
            get
            {
                return _Mute;
            }
        }
        [XmlIgnore]
        public Color Color
        {
            set
            {
                _Color = value;
            }
            get
            {
                return _Color;
            }
        }
        public List<MIDIPart> Part
        {
            set
            {
                _Part = value;
            }
            get
            {
                return _Part;
            }
        }
        public List<MIDIRythm> Rythm
        {
            set
            {
                _Rythm = value;
            }
            get
            {
                return _Rythm;
            }
        }
        public bool RythmMode
        {
            set
            {
                _RythmMode = value;
            }
            get
            {
                return _RythmMode;
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
        public int St
        {
            set
            {
                _St = value;
            }
            get
            {
                return _St;
            }
        }
        public int? mStartIndex
        {
            set
            {
                _StartPartIndex = value;
            }
            get
            {
                return _StartPartIndex;
            }
        }
        public int? mEndIndex
        {
            set
            {
                _EndPartIndex = value;
            }
            get
            {
                return _EndPartIndex;
            }
        }
        public int mNumber
        {
            set
            {
                _NumberPart = value;
            }
            get
            {
                return _NumberPart;
            }
        }
        public int TrackNumber
        {
            set
            {
                _TrackNumber = value;
            }
            get
            {
                return _TrackNumber;
            }
        }
        private int mCounter = 0;

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public MIDIPart NowPart
        {
            set
            {
                _NowPart = value;
            }
            get
            {
                return _NowPart;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public int NowTick
        {
            set
            {
                _NowTick = value;
            }
            get
            {
                return _NowTick;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public int NextEventTick
        {
            set
            {
                _NextEventTick = value;
            }
            get
            {
                return _NextEventTick;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public bool EndMark
        {
            set
            {
                _EndMark = value;
            }
            get
            {
                return _EndMark;
            }
        }

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public Stack<MIDIEvent> LoopTargetEvent
        {
            set
            {
                _LoopTargetEvent = value;
            }
            get
            {
                return _LoopTargetEvent;
            }
        }

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public int? LoopOrSameTargetEventIndex
        {
            set
            {
                _LoopOrSameTargetEventIndex = value;
            }
            get
            {
                return _LoopOrSameTargetEventIndex;
            }
        }

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public int? SameMeasure
        {
            set
            {
                _SameMeasure = value;
            }
            get
            {
                return _SameMeasure;
            }
        }

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public byte RolandBase_gt
        {
            set
            {
                _RolandBase_gt = value;
            }
            get
            {
                return _RolandBase_gt;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public byte RolandBase_vel
        {
            set
            {
                _RolandBase_vel = value;
            }
            get
            {
                return _RolandBase_vel;
            }
        }

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public byte RolandDev_gt
        {
            set
            {
                _RolandDev_gt = value;
            }
            get
            {
                return _RolandDev_gt;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public byte RolandDev_vel
        {
            set
            {
                _RolandDev_vel = value;
            }
            get
            {
                return _RolandDev_vel;
            }
        }

        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public byte RolandPara_gt
        {
            set
            {
                _RolandPara_gt = value;
            }
            get
            {
                return _RolandPara_gt;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public byte RolandPara_vel
        {
            set
            {
                _RolandPara_vel = value;
            }
            get
            {
                return _RolandPara_vel;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使用不可
        /// </summary>
        [XmlIgnore]
        public int[] NoteGateTime
        {
            set
            {
                _NoteGateTime = value;
            }
            get
            {
                return _NoteGateTime;
            }
        }

        public byte YAMAHABase_gt
        {
            get
            {
                return _YAMAHABase_gt;
            }

            set
            {
                _YAMAHABase_gt = value;
            }
        }

        public byte YAMAHABase_vel
        {
            get
            {
                return _YAMAHABase_vel;
            }

            set
            {
                _YAMAHABase_vel = value;
            }
        }

        public byte YAMAHA_dev
        {
            get
            {
                return _YAMAHA_dev;
            }

            set
            {
                _YAMAHA_dev = value;
            }
        }

        public byte YAMAHA_model
        {
            get
            {
                return _YAMAHA_model;
            }

            set
            {
                _YAMAHA_model = value;
            }
        }

        public byte YAMAHAPara_gt
        {
            get
            {
                return _YAMAHAPara_gt;
            }

            set
            {
                _YAMAHAPara_gt = value;
            }
        }

        public byte YAMAHAPara_vel
        {
            get
            {
                return _YAMAHAPara_vel;
            }

            set
            {
                _YAMAHAPara_vel = value;
            }
        }

        public int KeySIG_SF
        {
            get
            {
                return _KeySIG_SF;
            }

            set
            {
                _KeySIG_SF = value;
            }
        }

        public int KeySIG_MI
        {
            get
            {
                return _KeySIG_MI;
            }

            set
            {
                _KeySIG_MI = value;
            }
        }

        //初めのpartを得る
        public MIDIPart getStartPart()
        {
            if (mStartIndex == null) return null;

            return Part[(int)mStartIndex];
        }

        //最後のpartを得る
        public MIDIPart getEndPart()
        {
            if (mEndIndex == null) return null;

            return Part[(int)mEndIndex];
        }

        //指定したpartの次のpartを得る
        public MIDIPart getNextPart(MIDIPart prt)
        {
            if (prt == null || prt.AfterIndex == null) return null;

            return Part[(int)prt.AfterIndex];
        }

        //指定したpartの前の小節を得る
        public MIDIPart getPrevPart(MIDIPart prt)
        {
            if (prt == null || prt.BeforeIndex == null) return null;

            return Part[(int)prt.BeforeIndex];
        }

        //インデックスからpartを得る
        public MIDIPart searchPart(int index)
        {
            int st_d = int.MaxValue;
            int ed_d = int.MaxValue;
            int now_d = int.MaxValue;
            int mode = -1;
            if (this.mStartIndex != null) st_d = Math.Abs((int)this.mStartIndex - index);
            if (this.mEndIndex != null) ed_d = Math.Abs((int)this.mEndIndex - index);
            if (this.NowPart != null) now_d = Math.Abs(this.NowPart.Number - index);
            MIDIPart prt = null;
            if (st_d < ed_d && st_d < now_d)
            {
                prt = this.getStartPart();
                mode = 0;
            }
            else if (ed_d < st_d && ed_d < now_d)
            {
                prt = this.getEndPart();
                mode = 1;
            }
            else if (now_d <= st_d && now_d <= ed_d)
            {
                prt = this.NowPart;
                if (prt.Number < index)
                {
                    mode = 0;
                }
                else
                {
                    mode = 1;
                }
            }
            else return null;

            switch (mode)
            {
                case 0:
                    while (prt != null)
                    {
                        if (prt.Number == index)
                            return prt;
                        prt = this.getNextPart(prt);
                    }
                    break;
                case 1:
                    while (prt != null)
                    {
                        if (prt.Number == index)
                            return prt;
                        prt = this.getPrevPart(prt);
                    }
                    break;
            }

            return null;
        }

        //全てのPartをメモリから消去する
        public void clearAllPartMemory()
        {
            this.Part.Clear();
            this.mCounter = 0;
            this.mStartIndex = null;
            this.mEndIndex = null;
            this.mNumber = 0;
        }

        //全てのPartを消去する
        public void clearEvent()
        {
            this.mCounter = 0;
            this.mStartIndex = null;
            this.mEndIndex = null;
        }

        /// <summary>
        /// partを挿入する
        /// (既存partが増えると挿入位置を特定するのに時間がかかるようになるので注意)
        /// </summary>
        /// <param name="StartTick">絶対値によるTick値</param>
        /// <param name="prt">part</param>
        public void insertPart(int StartTick, MIDIPart prt)
        {
            if (prt == null) return;
            prt.StartTick = StartTick;

            if (this.Part == null)
            {
                this.Part = new List<MIDIPart>();
            }
            if (this.Part.Count == 0 || this.mStartIndex == null)//初めのprt
            {
                prt.AfterIndex = null;
                prt.BeforeIndex = null;
                prt.Number = this.mNumber;
                this.Part.Add(prt);
                this.mStartIndex = 0;
                this.mEndIndex = 0;
                this.mCounter = 1;
                this.mNumber++;
                return;
            }

            //遅くなる原因になっているループ
            MIDIPart pPrt = getStartPart();
            while (true)
            {
                if (pPrt.StartTick > prt.StartTick)
                {
                    pPrt = getPrevPart(pPrt);
                    break;
                }
                MIDIPart ppPrt = getNextPart(pPrt);
                if (ppPrt == null) break;
                pPrt = ppPrt;
            }

            prt.BeforeIndex = pPrt.Number;
            prt.AfterIndex = pPrt.AfterIndex;
            prt.Number = this.mNumber;
            pPrt.AfterIndex = prt.Number;
            this.Part.Add(prt);
            if (prt.AfterIndex == null)
            {
                this.mEndIndex = this.mNumber;
            }
            else
            {
                pPrt = getNextPart(prt);
                pPrt.BeforeIndex = prt.Number;
            }
            this.mCounter++;
            this.mNumber++;

        }

    }

}
