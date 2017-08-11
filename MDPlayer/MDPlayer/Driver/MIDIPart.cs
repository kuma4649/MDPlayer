using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

namespace MDPlayer
{
    [Serializable]
    public class MIDIPart
    {
        private int? _BeforeIndex = null;
        private int? _AfterIndex = null;
        private int _Number = 0;
        private string _Name = "";
        private int _StartTick = 0;
        private List<MIDIEvent> _Event = new List<MIDIEvent>();
        private int _eNumber = 0;
        private int? _eStartIndex = null;
        private int? _eEndIndex = null;
        private int? _eNowIndex = 0;


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
        public int StartTick
        {
            set
            {
                _StartTick = value;
            }
            get
            {
                return _StartTick;
            }
        }

        /// <summary>
        /// イベントリスト
        /// </summary>
        public List<MIDIEvent> Event
        {
            set
            {
                _Event = value;
            }
            get
            {
                return _Event;
            }
        }
        /// <summary>
        /// 有効なイベントの個数
        /// </summary>
        private int eCounter = 0;
        /// <summary>
        /// イベントの通し番号
        /// </summary>
        public int eNumber
        {
            set
            {
                _eNumber = value;
            }
            get
            {
                return _eNumber;
            }
        }
        /// <summary>
        /// 開始イベントの番号
        /// </summary>
        public int? eStartIndex
        {
            set
            {
                _eStartIndex = value;
            }
            get
            {
                return _eStartIndex;
            }
        }
        /// <summary>
        /// 終了イベントの番号
        /// </summary>
        public int? eEndIndex
        {
            set
            {
                _eEndIndex = value;
            }
            get
            {
                return _eEndIndex;
            }
        }
        /// <summary>
        /// 演奏時専用なのでそれ以外の用途で使っちゃだめ
        /// </summary>
        public int? eNowIndex
        {
            set
            {
                _eNowIndex = value;
            }
            get
            {
                return _eNowIndex;
            }
        }

        //初めのイベントを得る
        public MIDIEvent getStartEvent()
        {
            if (eStartIndex == null) return null;

            return Event[(int)eStartIndex];
        }

        //最後のイベントを得る
        public MIDIEvent getEndEvent()
        {
            if (eEndIndex == null) return null;

            return Event[(int)eEndIndex];
        }

        //指定したイベントの次のイベントを得る
        public MIDIEvent getNextEvent(MIDIEvent eve)
        {
            if (eve == null || eve.AfterIndex == null) return null;

            return Event[(int)eve.AfterIndex];
        }

        //指定したイベントの前のイベントを得る
        public MIDIEvent getPrevEvent(MIDIEvent eve)
        {
            if (eve == null || eve.BeforeIndex == null) return null;

            return Event[(int)eve.BeforeIndex];
        }

        //指定したイベントを除外する（メモリには残る）
        public bool removeEvent(MIDIEvent eve)
        {
            if (eve == null) return false;
            MIDIEvent pEvent = getPrevEvent(eve);
            MIDIEvent nEvent = getNextEvent(eve);
            if (pEvent != null) pEvent.AfterIndex = (nEvent == null) ? null : (int?)nEvent.Number;
            if (nEvent != null) nEvent.BeforeIndex = (pEvent == null) ? null : (int?)pEvent.Number;
            pEvent.Step += eve.Step;
            this.eCounter--;

            return true;
        }

        //指定したイベントをメモリから消去する(removeEventに比べ低速)
        public bool clearEvent(MIDIEvent eve)
        {
            if (eve == null) return false;
            MIDIEvent pEvent = getPrevEvent(eve);
            MIDIEvent nEvent = getNextEvent(eve);
            if (pEvent != null) pEvent.AfterIndex = (nEvent == null) ? null : (int?)nEvent.Number;
            if (nEvent != null) nEvent.BeforeIndex = (pEvent == null) ? null : (int?)pEvent.Number;
            pEvent.Step += eve.Step;
            this.eCounter--;
            this.eNumber--;

            int num = eve.Number;
            this.Event.Remove(eve);

            foreach (MIDIEvent evt in this.Event)
            {
                if (evt.Number >= num) evt.Number--;
                if (evt.AfterIndex >= num) evt.AfterIndex--;
                if (evt.BeforeIndex >= num) evt.BeforeIndex--;
            }

            return true;
        }

        //全てのイベントをメモリから消去する
        public void clearAllEventMemory()
        {
            this.Event.Clear();
            this.eCounter = 0;
            this.eStartIndex = null;
            this.eEndIndex = null;
            this.eNumber = 0;
        }

        //全てのイベントを消去する
        public void clearEvent()
        {
            this.eCounter = 0;
            this.eStartIndex = null;
            this.eEndIndex = null;
        }

        /// <summary>
        /// 指定されたイベントの後ろにイベントを挿入する
        /// </summary>
        /// <param name="TargetEvent">このイベントの後ろに新たに入る</param>
        /// <param name="Step">Step値</param>
        /// <param name="EventType">イベントタイプ</param>
        /// <param name="MIDImessage">MIDIメッセージ(Chは0固定であること)</param>
        /// <returns>新たに挿入したイベント</returns>
        public MIDIEvent insertEvent(MIDIEvent TargetEvent,int Step, MIDIEventType EventType, byte[] MIDImessage)
        {
            if (MIDImessage == null) return null;
            MIDIEvent eve = new MIDIEvent();
            eve.EventType = EventType;
            eve.MIDIMessage = MIDImessage;
            eve.MIDIMessageLst = null;
            eve.Step = Step;

            insertEve(TargetEvent, Step, eve);

            return eve;
        }

        /// <summary>
        /// 指定されたイベントの後ろにイベントを挿入する
        /// </summary>
        /// <param name="TargetEvent">このイベントの後ろに新たに入る</param>
        /// <param name="Step">Step値</param>
        /// <param name="EventType">イベントタイプ</param>
        /// <param name="MIDImessage">MIDIメッセージ(Chは0固定であること)</param>
        /// <param name="gt">ゲートタイム</param>
        /// <returns>新たに挿入したイベント</returns>
        public MIDIEvent insertEvent(MIDIEvent TargetEvent, int Step, MIDIEventType EventType, byte[] MIDImessage, int gt)
        {
            if (MIDImessage == null) return null;
            MIDIEvent eve = new MIDIEvent();
            eve.EventType = EventType;
            eve.MIDIMessage = MIDImessage;
            eve.MIDIMessageLst = null;
            eve.Step = Step;
            eve.Gate = gt;

            insertEve(TargetEvent, Step, eve);

            return eve;
        }

        /// <summary>
        /// 指定されたイベントの後ろにイベントを挿入する
        /// </summary>
        /// <param name="TargetEvent">このイベントの後ろに新たに入る</param>
        /// <param name="Step">Step値</param>
        /// <param name="EventType">イベントタイプ</param>
        /// <param name="MIDImessageLst">MIDIメッセージ</param>
        /// <returns>新たに挿入したイベント</returns>
        public MIDIEvent insertSpEvent(MIDIEvent TargetEvent, int Step, MIDISpEventType EventType, byte[][] MIDImessageLst)
        {
            //if (MIDImessageLst == null) return null;
            MIDIEvent eve = new MIDIEvent();
            eve.EventType = MIDIEventType.MetaSequencerSpecific;
            eve.MIDIMessage = new byte[1] { (byte)EventType };
            eve.MIDIMessageLst = MIDImessageLst;
            eve.Step = Step;

            insertEve(TargetEvent, Step, eve);

            return eve;
        }

        private void insertEve(MIDIEvent TargetEvent,int Step, MIDIEvent eve)
        {
            //イベントリストを生成
            if (this.Event == null)
            {
                this.Event = new List<MIDIEvent>();
            }
            if (TargetEvent==null || this.Event.Count == 0 || this.eStartIndex == null)//初めのイベント
            {
                eve.AfterIndex = null;
                eve.BeforeIndex = null;
                eve.Number = this.eNumber;
                this.Event.Add(eve);
                this.eStartIndex = 0;
                this.eEndIndex = 0;
                this.eCounter = 1;
                this.eNumber++;
                return;
            }

            eve.BeforeIndex = TargetEvent.Number;
            eve.AfterIndex = TargetEvent.AfterIndex;
            eve.Number = this.eNumber;
            TargetEvent.AfterIndex = eve.Number;
            this.Event.Add(eve);
            if (eve.AfterIndex == null)
            {
                this.eEndIndex = this.eNumber;
            }
            else
            {
                TargetEvent = getNextEvent(eve);
                TargetEvent.BeforeIndex = eve.Number;
            }
            this.eCounter++;
            this.eNumber++;

        }

        /// <summary>
        /// Tickを考慮せずに最後のイベントの後ろに追加する。
        /// </summary>
        /// <param name="Tick"></param>
        /// <param name="EventType"></param>
        /// <param name="MIDImessage"></param>
        public void addEvent(int Step, MIDIEventType EventType, byte[] MIDImessage)
        {
            if (MIDImessage == null) return;
            MIDIEvent eve = new MIDIEvent();
            eve.EventType = EventType;
            eve.MIDIMessage = MIDImessage;
            eve.MIDIMessageLst = null;
            eve.Step = Step;

            MIDIEvent lastEvent = this.getEndEvent();
            if (lastEvent == null)//初めのイベント
            {
                eve.AfterIndex = null;
                eve.BeforeIndex = null;
                eve.Number = this.eNumber;
                this.Event.Add(eve);
                this.eStartIndex = 0;
                this.eEndIndex = 0;
                this.eCounter = 1;
                this.eNumber++;
                return;
            }

            eve.BeforeIndex = lastEvent.Number;
            eve.AfterIndex = null;
            eve.Number = this.eNumber;
            this.eEndIndex = this.eNumber;
            this.Event.Add(eve);
            this.eCounter++;
            this.eNumber++;
            lastEvent.AfterIndex = eve.Number;
        }

    }

}
