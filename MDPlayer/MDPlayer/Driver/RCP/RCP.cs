using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class RCP : baseDriver
    {


        private double oneSyncTime = 0.009;
        private double musicStep = common.SampleRate / 60.0;
        private double musicDownCounter = 0.0;


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

        public class Tick
        {
            public int Millisec = 0;
            public int Count = 0;
            public int Before = 0;
            public int Sabun = 0;
        };
        private Tick tick = new RCP.Tick();


        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (buf == null) return null;

            GD3 gd3 = new GD3();
            int ptr = 0;
            bool IsG36 = false;

            string str = Encoding.GetEncoding("Shift_JIS").GetString(buf, ptr, 32);
            if (str != "RCM-PC98V2.0(C)COME ON MUSIC\r\n\0\0")
            {
                if (str != "COME ON MUSIC RECOMPOSER RCP3.0\0") return null;
                else IsG36 = true;
            }
            ptr += 32;

            List<byte> title = new List<byte>();
            for (int i = 0; i < 64; i++)
            {
                if (buf[ptr + i] == 0) break;
                title.Add(buf[ptr + i]);
            }
            str = (Encoding.GetEncoding("Shift_JIS").GetString(title.ToArray())).Trim();
            ptr += 64;
            gd3.TrackName = str;
            gd3.TrackNameJ = str;

            if (IsG36)
            {
                ptr += 64;
                str = string.Format("{0}\r\n", (Encoding.GetEncoding("Shift_JIS").GetString(buf, ptr, 360)).Replace("\0", ""));
            }
            else
            {
                str = "";
                for (int i = 0; i < 12; i++)
                {
                    str += string.Format("{0}\r\n", (Encoding.GetEncoding("Shift_JIS").GetString(buf, ptr + i * 28, 28)).Replace("\0", ""));
                }
            }
            gd3.Notes = str;

            return gd3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip[] useChip, uint latency)
        {
            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = 0;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            GD3 = getGD3Info(vgmBuf, 0);
            //if (GD3 == null) return false;

            if (!getInformationHeader()) return false;

            if (model == enmModel.RealModel)
            {
                chipRegister.setYM2612SyncWait(0, 1);
                chipRegister.setYM2612SyncWait(1, 1);
            }

            return true;
        }

        public override void oneFrameProc()
        {

            try
            {
                vstDelta++;
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0 && !Stopped)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
                //Stopped = !IsPlaying();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }


        private bool IsG36 = false;
        private int ptr = 0;
        private int trkLen = 0;
        private int TimeBase = 0;
        private double nowTempo = 0;
        private double Tempo = 0;
        private int BeatDen = 0;
        private int BeatMol = 0;
        private int Key = 0;
        private int PlayBIAS = 0;
        private string ControlFileGSD = "";
        private string ControlFileGSD2 = "";
        private string ControlFileCM6 = "";
        private int rcpVer = 0;
        private List<MIDIRythm> rtm;
        private List<MIDIUserExclusive> UserExclusive;
        private MIDITrack[] trk = null;
        private MIDIPart[] prt = null;

        //private int trkTick = 0;
        //private int meaTick = 0;
        private int meaInd = 0;
        private bool endTrack = false;
        private Dictionary<byte, byte> taiDic = null;
        private int stDevNum = 0;
        private int pt = 0;
        private int skipPtr = 4;
        private byte[] msgBuf2 = new byte[2];
        private byte[] msgBuf3 = new byte[3];
        private byte[] msgBuf = new byte[256];

        delegate void efd(MIDITrack trk, MIDIEvent eve);
        private efd[] EventFunc = new efd[256];
        private efd[] SpecialEventFunc = new efd[256];
        private int RelativeTempoChangeTargetTempo;
        private double RelativeTempoChangeTickSlice;
        private bool RelativeTempoChangeSW = false;


        private bool getInformationHeader()
        {
            ptr = 0;
            IsG36 = false;

            string str = Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 32);
            if (str != "RCM-PC98V2.0(C)COME ON MUSIC\r\n\0\0")
            {
                if (str != "COME ON MUSIC RECOMPOSER RCP3.0\0") return false;
                else IsG36 = true;
            }
            ptr += 32;

            ptr += 64;

            if (IsG36)
            {
                Header_G36();
            }
            else
            {
                Header_RCP();
            }

            oneSyncTime = 60.0 / nowTempo / TimeBase;

            Rythm();

            UserEx();

            TrackData();

            Init();

            EventFunc[0x00] = new efd(efMetaSeqNumber);
            EventFunc[0x01] = new efd(efMetaTextEvent);
            EventFunc[0x02] = new efd(efMetaCopyrightNotice);
            EventFunc[0x03] = new efd(efMetaTrackName);
            EventFunc[0x04] = new efd(efMetaInstrumentName);
            EventFunc[0x05] = new efd(efMetaLyric);
            EventFunc[0x06] = new efd(efMetaMarker);
            EventFunc[0x07] = new efd(efMetaCuePoint);
            EventFunc[0x08] = new efd(efMetaProgramName);
            EventFunc[0x09] = new efd(efMetaDeviceName);
            EventFunc[0x20] = new efd(efMetaChannelPrefix);
            EventFunc[0x21] = new efd(efMetaPortPrefix);
            EventFunc[0x2f] = new efd(efMetaEndOfTrack);
            EventFunc[0x51] = new efd(efMetaTempo);
            EventFunc[0x54] = new efd(efMetaSMPTEOffset);
            EventFunc[0x58] = new efd(efMetaTimeSignature);
            EventFunc[0x59] = new efd(efMetaKeySignature);
            EventFunc[0x7f] = new efd(efMetaSequencerSpecific);
            EventFunc[0x80] = new efd(efNoteOff);
            EventFunc[0x90] = new efd(efNoteOn);
            EventFunc[0xa0] = new efd(efKeyAfterTouch);
            EventFunc[0xb0] = new efd(efControlChange);
            EventFunc[0xc0] = new efd(efProgramChange);
            EventFunc[0xd0] = new efd(efChannelAfterTouch);
            EventFunc[0xe0] = new efd(efPitchBend);
            EventFunc[0xf0] = new efd(efSysExStart);
            EventFunc[0xf7] = new efd(efSysExContinue);
            for (int i = 0; i < 256; i++)
            {
                if (EventFunc[i] == null) EventFunc[i] = new efd(efn);
            }


            SpecialEventFunc[0x90] = new efd(sefUserExclusive1);
            SpecialEventFunc[0x91] = new efd(sefUserExclusive2);
            SpecialEventFunc[0x92] = new efd(sefUserExclusive3);
            SpecialEventFunc[0x93] = new efd(sefUserExclusive4);
            SpecialEventFunc[0x94] = new efd(sefUserExclusive5);
            SpecialEventFunc[0x95] = new efd(sefUserExclusive6);
            SpecialEventFunc[0x96] = new efd(sefUserExclusive7);
            SpecialEventFunc[0x97] = new efd(sefUserExclusive8);
            SpecialEventFunc[0x98] = new efd(sefChExclusive);
            SpecialEventFunc[0x99] = new efd(sefOutsideProcessExec);
            SpecialEventFunc[0xc0] = new efd(sefDX7Func);
            SpecialEventFunc[0xc1] = new efd(sefDXPara);
            SpecialEventFunc[0xc2] = new efd(sefDXRERF);
            SpecialEventFunc[0xc3] = new efd(sefTXFunc);
            SpecialEventFunc[0xc5] = new efd(sefFB01PPara);
            SpecialEventFunc[0xc6] = new efd(sefFB01SSystem);
            SpecialEventFunc[0xc7] = new efd(sefTX81ZVVCED);
            SpecialEventFunc[0xc8] = new efd(sefTX81ZAACED);
            SpecialEventFunc[0xc9] = new efd(sefTX81ZPPCED);
            SpecialEventFunc[0xca] = new efd(sefTX81ZSSystem);
            SpecialEventFunc[0xcb] = new efd(sefTX81ZEEffect);
            SpecialEventFunc[0xcc] = new efd(sefDX72RRemoteSW);
            SpecialEventFunc[0xcd] = new efd(sefDX72AACED);
            SpecialEventFunc[0xce] = new efd(sefDX72PPCED);
            SpecialEventFunc[0xcf] = new efd(sefTX802PPCED);
            SpecialEventFunc[0xd0] = new efd(sefYAMAHABase);
            SpecialEventFunc[0xd1] = new efd(sefYAMAHADev);
            SpecialEventFunc[0xd2] = new efd(sefYAMAHAAddrPara);
            SpecialEventFunc[0xd3] = new efd(sefYAMAHAXGAddrPara);
            SpecialEventFunc[0xdc] = new efd(sefMKS7);
            SpecialEventFunc[0xdd] = new efd(sefRolandBase);
            SpecialEventFunc[0xde] = new efd(sefRolandPara);
            SpecialEventFunc[0xdf] = new efd(sefRolandDev);
            SpecialEventFunc[0xe2] = new efd(sefBankProgram);
            SpecialEventFunc[0xe5] = new efd(sefKeyScan);
            SpecialEventFunc[0xe6] = new efd(sefMIDIChChange);
            SpecialEventFunc[0xe7] = new efd(sefTempoChange);
            SpecialEventFunc[0xf5] = new efd(sefKeyChange);
            SpecialEventFunc[0xf6] = new efd(sefCommentStart);
            SpecialEventFunc[0xf8] = new efd(sefLoopEnd);
            SpecialEventFunc[0xf9] = new efd(sefLoopStart);
            SpecialEventFunc[0xfc] = new efd(sefSameMeasure);
            SpecialEventFunc[0xfd] = new efd(sefMeasureEnd);
            SpecialEventFunc[0xfe] = new efd(sefEndofTrack);
            for (int i = 0; i < 256; i++)
            {
                if (SpecialEventFunc[i] == null) SpecialEventFunc[i] = new efd(efn);
            }


            return true;
        }

        private void Header_G36()
        {
            //dummy Skip
            ptr += 64;
            //Memo
            ptr += 360;
            //トラック数
            trkLen = vgmBuf[ptr++];
            if (trkLen != 18 && trkLen != 36) trkLen = 18;
            //dummy Skip
            ptr++;
            //Timebase
            TimeBase = vgmBuf[ptr] + (vgmBuf[ptr + 1] * 0x100);
            ptr += 2;
            //Tempo
            nowTempo = vgmBuf[ptr++];
            if (nowTempo < 8 || nowTempo > 250) nowTempo = 120;
            Tempo = nowTempo;
            //dummy Skip
            ptr++;
            //拍子（分子）
            BeatDen = vgmBuf[ptr++];
            //拍子（分母）
            BeatMol = vgmBuf[ptr++];
            //Key
            Key = vgmBuf[ptr++];
            //Play BIAS
            PlayBIAS = vgmBuf[ptr++];
            //dummy Skip
            ptr += 6;
            //dummy Skip
            ptr += 16;
            //dummy Skip
            ptr += 112;
            //.GSD
            ControlFileGSD = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 12)).Replace("\0", "");
            ptr += 12;
            //dummy Skip
            ptr += 4;
            //.GSD
            ControlFileGSD2 = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 12)).Replace("\0", "");
            ptr += 12;
            //dummy Skip
            ptr += 4;
            //.CM6
            ControlFileCM6 = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 12)).Replace("\0", "");
            ptr += 12;
            //dummy Skip
            ptr += 4;
            //dummy Skip
            ptr += 80;
            rcpVer = 0;
        }

        private void Header_RCP()
        {
            //Memo
            ptr += 336;
            //dummy Skip
            ptr += 16;
            //Timebase下位
            TimeBase = vgmBuf[ptr++];
            //Tempo
            nowTempo = vgmBuf[ptr++];
            Tempo = nowTempo;
            //拍子（分子）
            BeatDen = vgmBuf[ptr++];
            //拍子（分母）
            BeatMol = vgmBuf[ptr++];
            //Key
            Key = vgmBuf[ptr++];
            //Play BIAS
            PlayBIAS = vgmBuf[ptr++];
            //.CM6
            ControlFileCM6 = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 12)).Replace("\0", "");
            ptr += 12;
            //dummy Skip
            ptr += 4;
            //.GSD
            ControlFileGSD = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 12)).Replace("\0", "");
            ptr += 12;
            //dummy Skip
            ptr += 4;
            //トラック数
            trkLen = vgmBuf[ptr++];
            switch (trkLen)
            {
                case 0:
                    trkLen = 36;
                    rcpVer = 0;
                    break;
                case 18:
                    rcpVer = 1;
                    break;
                case 36:
                    rcpVer = 2;
                    break;
            }
            //Timebase上位
            TimeBase += vgmBuf[ptr++] * 0x100;
            //dummy Skip
            //無視
            //TONENAME.TB?
            //いまのところ無視
            ptr = 0x206;//リズム定義部まで
        }

        private void Rythm()
        {
            rtm = new List<MIDIRythm>();
            int n = 32;
            if (IsG36) n = 128;

            for (int i = 0; i < n; i++)
            {
                MIDIRythm r = new MIDIRythm();
                r.Name = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 14)).Replace("\0", "");
                ptr += 14;
                r.Key = vgmBuf[ptr++];
                r.Gt = vgmBuf[ptr++];
                rtm.Add(r);
            }
        }

        private void UserEx()
        {
            UserExclusive = new List<MIDIUserExclusive>();
            for (int i = 0; i < 8; i++)
            {
                MIDIUserExclusive ux = new MIDIUserExclusive();
                ux.Name = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 24)).Replace("\0", "");
                ux.Memo = ux.Name;
                ptr += 24;
                ux.Exclusive = new byte[25];
                ux.Exclusive[0] = 0xf0;
                for (int j = 1; j < 25; j++)
                {
                    ux.Exclusive[j] = vgmBuf[ptr++];
                }
                UserExclusive.Add(ux);
            }
        }

        private void TrackData()
        {
            initTrkPrt(); //トラックと小節を準備する

            for (int i = 0; i < trkLen; i++)
            {
                if (ptr >= vgmBuf.Length) continue;

                int vgmBufptr = ptr;
                int trkSize = vgmBuf[ptr++] * 0x100 + vgmBuf[ptr++];

                //Size dummy(?) skip
                if (IsG36) ptr += 2;

                int trkNumber = 0;
                if (IsG36)
                {
                    trkNumber = i;
                    ptr++;
                }
                else
                {
                    trkNumber = vgmBuf[ptr++] - 1;
                    if (trkNumber < 0)
                        trkNumber = i;
                }
                trk[trkNumber].RythmMode = (vgmBuf[ptr++] == 0x80) ? true : false;
                int ch = vgmBuf[ptr++];
                if (ch != 255)
                {
                    int mc = chipRegister.getMIDIoutCount();
                    if (mc == 0) mc = 1;
                    int n = (stDevNum + (ch / 16)) % mc;
                    trk[trkNumber].OutDeviceName = "dummy";// config.MIDIOutDeviceList[n].DevName;
                    trk[trkNumber].OutDeviceNumber = n;// config.MIDIOutDeviceList[n].DevNumber;
                    trk[trkNumber].OutUserDeviceNumber = n; // config.MIDIOutDeviceList[n].UsrNumber;
                    trk[trkNumber].OutUserDeviceName = "dummy";// config.MIDIOutDeviceList[n].UsrName;
                    trk[trkNumber].OutChannel = ch % 16;
                }
                else
                {
                    trk[trkNumber].OutDeviceName = "Null Device";
                    trk[trkNumber].OutDeviceNumber = null;
                    trk[trkNumber].OutUserDeviceNumber = null;
                    trk[trkNumber].OutUserDeviceName = "Null Device";
                    trk[trkNumber].OutChannel = null;
                }

                trk[trkNumber].InDeviceName = "Null Device";
                trk[trkNumber].InDeviceNumber = null;
                trk[trkNumber].InUserDeviceNumber = null;
                trk[trkNumber].InUserDeviceName = "Null Device";
                trk[trkNumber].InChannel = null;

                trk[trkNumber].Key = vgmBuf[ptr++];

                if ((trk[trkNumber].Key & 0x80) == 0x80)
                {
                    trk[trkNumber].Key = null;
                }
                else
                {
                    trk[trkNumber].Key = (trk[trkNumber].Key > 63) ? trk[trkNumber].Key - 128 : trk[trkNumber].Key;
                }
                trk[trkNumber].St = vgmBuf[ptr++];
                if (rcpVer > 0)
                {
                    trk[trkNumber].St = (trk[trkNumber].St > 127) ? trk[trkNumber].St - 256 : trk[trkNumber].St;
                }
                trk[trkNumber].Mute = (vgmBuf[ptr++] == 1) ? true : false;
                trk[trkNumber].Name = (Encoding.GetEncoding("Shift_JIS").GetString(vgmBuf, ptr, 36)).Replace("\0", "");
                ptr += 36;

                //trkTick = 0;
                taiDic = new Dictionary<byte, byte>();
                //meaTick = 0;
                meaInd = 0;
                musData(trk[trkNumber], vgmBuf);
                extractSame(trk[trkNumber]);
            }
        }

        private void extractSame(MIDITrack trk)
        {
            MIDIEvent evt = trk.Part[0].getStartEvent();
            while (evt != null)
            {
                if (evt.EventType == MIDIEventType.MetaSequencerSpecific && evt.MIDIMessage[0] == (byte)MIDISpEventType.SameMeasure)
                {

                    int ofsMea = 0;
                    if (IsG36)
                    {
                        ofsMea = evt.MIDIMessageLst[0][0] + evt.MIDIMessageLst[0][2] * 0x100;
                        //if (trkLen == 36)
                        //{
                        //    ofsMea = ofsMea * 6 - 242;
                        //}
                    }
                    else
                    {
                        ofsMea = evt.MIDIMessageLst[0][0] + (evt.MIDIMessageLst[0][1] & 3) * 0x100;
                    }
                    int Mea = 0;
                    int MeaS = 0;
                    MIDIEvent mEvt = trk.Part[0].getStartEvent();
                    if (ofsMea != 0)
                    {
                        while (mEvt != null)
                        {
                            MeaS = 0;
                            if (mEvt.EventType == MIDIEventType.MetaSequencerSpecific)
                            {
                                MIDIEvent nEvt = trk.Part[0].getNextEvent(mEvt);
                                int s = 0;
                                if (nEvt.EventType == MIDIEventType.MetaSequencerSpecific
                                    && nEvt.MIDIMessage[0] == (byte)MIDISpEventType.SameMeasure)
                                {
                                    s = 0;
                                }
                                else
                                {
                                    s = 1;
                                }
                                if (mEvt.MIDIMessage[0] == (byte)MIDISpEventType.MeasureEnd)
                                {
                                    MeaS = s;
                                }
                                if (mEvt.MIDIMessage[0] == (byte)MIDISpEventType.SameMeasure)
                                {
                                    Mea++;
                                    if (ofsMea == Mea) break;
                                    MeaS = s;
                                }
                            }
                            mEvt = trk.Part[0].getNextEvent(mEvt);
                            Mea += MeaS;
                            if (ofsMea == Mea) break;
                        }
                    }

                    if (mEvt != null)
                    {
                        evt.SameMeasureIndex = mEvt.Number;
                    }
                }

                evt = trk.Part[0].getNextEvent(evt);
            }

        }

        private void musData(MIDITrack trkn, byte[] ebs)
        {
            endTrack = false;
            pt = ptr;

            while (!endTrack)
            {
                MIDIEvent pEvt = trkn.Part[meaInd].getEndEvent();
                int[] pk = null;
                if (!IsG36)
                {
                    pk = new int[4] { ebs[pt], ebs[pt + 1], ebs[pt + 2], ebs[pt + 3] };
                }
                else
                {
                    //Note   Step   Gate   Vel
                    pk = new int[4] { ebs[pt], ebs[pt + 2] + ebs[pt + 3] * 0x100, ebs[pt + 4] + ebs[pt + 5] * 0x100, ebs[pt + 1] };
                    skipPtr = 6;
                }
                if (pk[0] < 0x80)
                {
                    onpu(trkn, pk, pEvt); //おんぷさんらしい
                }
                else
                {
                    command(trkn, pk, ebs, pEvt);//コマンドらしい
                }
            }
            ptr = pt;
        }

        private void onpu(MIDITrack trkn, int[] pk, MIDIEvent pEvt)
        {
            trkn.Part[meaInd].insertEvent(pEvt, pk[1], MIDIEventType.NoteON, new byte[3] { 0x90, (byte)pk[0], (byte)pk[3] }, pk[2]);
            pt += skipPtr;
        }

        private void command(MIDITrack trkn, int[] pk, byte[] ebs, MIDIEvent pEvt)
        {
            List<byte> ex = null;
            switch (pk[0])
            {
                case 0x98: // CH Exclusive
                    pt += skipPtr;
                    ex = new List<byte>();
                    ex.Add(0xF0);
                    while (ebs[pt] == 0xf7)
                    {
                        if (IsG36)
                        {
                            pt++;
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                        }
                        else
                        {
                            pt += 2;
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                        }
                    }
                    ex.Add((byte)(pk[2] & 0xff));
                    //if (IsG36) ex.Add((byte)(pk[2] / 0x100));
                    ex.Add((byte)(pk[3] & 0xff));
                    //if (IsG36) ex.Add((byte)(pk[3] / 0x100));
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        pk[1],
                        MIDISpEventType.ChExclusive,
                        new byte[1][]{
                            ex.ToArray()
                        });
                    break;
                case 0x90: // User Exclusive 1
                case 0x91: // User Exclusive 2
                case 0x92: // User Exclusive 3
                case 0x93: // User Exclusive 4
                case 0x94: // User Exclusive 5
                case 0x95: // User Exclusive 6
                case 0x96: // User Exclusive 7
                case 0x97: // User Exclusive 8
                case 0xc0: // DX7 Function
                case 0xc1: // DX Parameter
                case 0xc2: // DX RERF
                case 0xc3: // TX Function
                case 0xc5: // FB-01 P Parameter
                case 0xc6: // FB-01 S System
                case 0xc7: // TX81Z V VCED
                case 0xc8: // TX81Z A ACED
                case 0xc9: // TX81Z P PCED
                case 0xca: // TX81Z S System
                case 0xcb: // TX81Z E EFFECT
                case 0xcc: // DX7-2 R Remote SW
                case 0xcd: // DX7-2 A ACED
                case 0xce: // DX7-2 P PCED
                case 0xcf: // TX802 P PCED
                case 0xdc: // MKS-7
                case 0xd0: // YAMAHA Base
                case 0xd1: // YAMAHA Dev
                case 0xd2: // YAMAHA Addr/Para
                case 0xd3: // YAMAHA XG Addr/Para
                case 0xdd: // Rol Base
                case 0xde: // Rol Para
                case 0xdf: // Rol Dev
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        pk[1],
                        (MIDISpEventType)pk[0],
                        new byte[1][]{
                            new byte[2]{ (byte)pk[2], (byte)pk[3]}
                        });
                    pt += skipPtr;
                    break;
                case 0xe2: // Bank & Program
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        pk[1],
                        MIDISpEventType.BankProgram,
                        new byte[2][]{
                            new byte[2]{(byte)MIDIEventType.ProgramChange, (byte)pk[2]},
                            new byte[3]{(byte)MIDIEventType.ControlChange, 0x00, (byte)pk[3]}
                        });
                    pt += skipPtr;
                    break;
                case 0xe5: // KEY SCAN
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        pk[1],
                        MIDISpEventType.KeyScan,
                        new byte[1][]{
                            new byte[1]{ (byte)pk[2]}
                        });
                    pt += skipPtr;
                    break;
                case 0xe6: // MIDI CH
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        pk[1],
                        MIDISpEventType.MIDICh,
                        new byte[1][]{
                            new byte[2]{ (byte)pk[2], (byte)pk[3]}
                        });
                    pt += skipPtr;
                    break;
                case 0xe7: //TempoChange
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        pk[1],
                        MIDISpEventType.TempoChange,
                        new byte[1][]{
                            new byte[2]{(byte)pk[2],(byte)pk[3]}
                        });
                    pt += skipPtr;
                    break;
                case 0xea: // After Touch Ch.
                    trkn.Part[meaInd].insertEvent(
                        pEvt,
                        pk[1],
                        MIDIEventType.ChannelAfterTouch,
                        new byte[2] { (byte)MIDIEventType.ChannelAfterTouch, (byte)pk[2] }
                        );
                    pt += skipPtr;
                    break;
                case 0xeb: // ControlChange
                    trkn.Part[meaInd].insertEvent(
                        pEvt,
                        pk[1],
                        MIDIEventType.ControlChange,
                        new byte[3] { (byte)MIDIEventType.ControlChange, (byte)pk[2], (byte)pk[3] }
                        );
                    pt += skipPtr;
                    break;
                case 0xec: // Program Change
                    trkn.Part[meaInd].insertEvent(
                        pEvt,
                        pk[1],
                        MIDIEventType.ProgramChange,
                        new byte[2] { (byte)MIDIEventType.ProgramChange, (byte)pk[2] }
                        );
                    pt += skipPtr;
                    break;
                case 0xed: // After Touch Pori.
                    trkn.Part[meaInd].insertEvent(
                        pEvt,
                        pk[1],
                        MIDIEventType.KeyAfterTouch,
                        new byte[3] { (byte)MIDIEventType.KeyAfterTouch, (byte)pk[2], (byte)pk[3] }
                        );
                    pt += skipPtr;
                    break;
                case 0xee: // Pitch Bend
                    trkn.Part[meaInd].insertEvent(
                        pEvt,
                        pk[1],
                        MIDIEventType.PitchBend,
                        new byte[3] { (byte)MIDIEventType.PitchBend, (byte)pk[2], (byte)pk[3] }
                        );
                    pt += skipPtr;
                    break;
                case 0xf5: // Key Change
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        0,
                        MIDISpEventType.KeyChange,
                        new byte[1][] { new byte[1] { (byte)pk[0] } }
                        );
                    pt += skipPtr;
                    break;
                case 0xf6: // Comment
                    pt += skipPtr;
                    ex = new List<byte>();
                    if (IsG36)
                    {
                        ex.Add((byte)(pk[3] & 0xff));
                        ex.Add((byte)(pk[1] & 0xff));
                        ex.Add((byte)(pk[1] / 0x100));
                        ex.Add((byte)(pk[2] & 0xff));
                        ex.Add((byte)(pk[2] / 0x100));
                        while (ebs[pt] == 0xf7)
                        {
                            pt++;
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                        }
                        trkn.Part[meaInd].insertSpEvent(
                            pEvt,
                            0,
                            MIDISpEventType.Comment,
                            new byte[1][]{
                            ex.ToArray()
                        });
                    }
                    else
                    {
                        ex.Add((byte)pk[2]);
                        ex.Add((byte)pk[3]);
                        while (ebs[pt] == 0xf7)
                        {
                            pt += 2;
                            ex.Add(ebs[pt++]);
                            ex.Add(ebs[pt++]);
                        }
                        trkn.Part[meaInd].insertSpEvent(
                            pEvt,
                            pk[1],
                            MIDISpEventType.Comment,
                            new byte[1][]{
                            ex.ToArray()
                        });
                    }
                    break;
                case 0xf8: // Loop End
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        0,
                        MIDISpEventType.LoopEnd,
                        new byte[1][] { new byte[1] { (byte)pk[1] } }
                        );
                    pt += skipPtr;
                    break;
                case 0xf9: // Loop Start
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        0,
                        MIDISpEventType.LoopStart,
                        new byte[1][] { new byte[1] { 0 } }
                        );
                    pt += skipPtr;
                    break;
                case 0xFC: // Same Measure
                    if (IsG36)
                    {
                        trkn.Part[meaInd].insertSpEvent(
                            pEvt,
                            0,
                            MIDISpEventType.SameMeasure,
                            new byte[1][] { new byte[3] { (byte)pk[1], (byte)(pk[3] & 0xff), (byte)(pk[3] / 0x100) } }
                            );
                    }
                    else
                    {
                        trkn.Part[meaInd].insertSpEvent(
                            pEvt,
                            0,
                            MIDISpEventType.SameMeasure,
                            new byte[1][] { new byte[3] { (byte)pk[1], (byte)pk[2], (byte)pk[3] } }
                            );
                    }
                    pt += skipPtr;
                    break;
                case 0xFD: // Measure End
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        0,
                        MIDISpEventType.MeasureEnd,
                        null
                        );
                    pt += skipPtr;
                    break;
                case 0xFE: // End of Track
                    trkn.Part[meaInd].insertSpEvent(
                        pEvt,
                        0,
                        MIDISpEventType.EndOfTrack,
                        new byte[1][] {
                            new byte[3] { (byte)pk[1], (byte)pk[2], (byte)pk[3] }
                        }
                        );
                    pt += skipPtr;
                    endTrack = true;
                    break;
                default:
                    pt += skipPtr;
                    break;
            }
        }

        private void initTrkPrt()
        {
            trk = new MIDITrack[trkLen];
            prt = new MIDIPart[trkLen];
            for (int i = 0; i < trkLen; i++)
            {
                trk[i] = new MIDITrack();
                trk[i].BeforeIndex = (i - 1 < 0) ? null : (int?)(i - 1);
                trk[i].AfterIndex = i + 1;
                trk[i].Number = i;
                trk[i].clearAllPartMemory();
                prt[i] = new MIDIPart();
                prt[i].Name = string.Format("Track {0} Part", i + 1);
                trk[i].insertPart(0, prt[i]);
            }
        }

        private void Init()
        {

            tick.Millisec = 0;
            tick.Count = 0;
            tick.Before = 0;
            tick.Sabun = 0;
            //ps.TimeBase = prj.Information.TimeBase; //分解能
            //ps.Tempo = prj.Information.Tempo;//テンポ:4分音符
            //ps.BaseTempo = prj.Information.Tempo;//テンポ:4分音符
            //ps.BeatDen = prj.Information.BeatDen;
            //ps.BeatMol = prj.Information.BeatMol;
            //ps.Lyric = "";
            //prj.RelativeTempoChangeNowTempo = prj.Information.Tempo;
            //prj.RelativeTempoChangeSW = false;


            //トラック毎の初期化
            foreach(MIDITrack tk in trk)
            {
                tk.NowPart = tk.getStartPart();
                tk.NowTick = 0;
                tk.NextEventTick = 0;
                tk.EndMark = false;
                if (tk.LoopTargetEvent == null)
                {
                    tk.LoopTargetEvent = new Stack<MIDIEvent>();
                }
                tk.LoopTargetEvent.Clear();
                tk.LoopOrSameTargetEventIndex = null;
                tk.SameMeasure = null;
                for (int n = 0; n < tk.NoteGateTime.Length; n++)
                {
                    tk.NoteGateTime[n] = int.MaxValue;
                }
                MIDIPart prt = tk.getStartPart();
                while (true)
                {
                    if (prt == null) break;
                    prt.eNowIndex = prt.eStartIndex;
                    prt = tk.getNextPart(prt);
                }

                //foreach (clsConfig.clsMIDIDeviceList dev in Config.MIDIOutDeviceList)
                //{
                //    if (dev.UsrNumber == tk.OutUserDeviceNumber)
                //    {
                //        tk.OutDeviceName = dev.DevName;
                //        tk.OutDeviceNumber = dev.DevNumber;
                //    }
                //}

            }

            ///* MIDIクロックの生成*/
            //MIDIClock = new MIDIClock();
            //MIDIClock.Create(0, ps.TimeBase, 60000000 / ps.Tempo);
            ///* MIDIクロックのリセットとスタート */
            //MIDIClock.Reset();
            //MIDIClock.Start();

        }



        private void oneFrameMain()
        {
            try
            {

                Counter++;
                vgmFrameCounter++;

                musicStep = common.SampleRate * oneSyncTime;

                if (musicDownCounter <= 0.0)
                {
                    //if (model != enmModel.VirtualModel)
                    //{
                        oneFrameRCP();
                    //}
                    //else
                    //{
                    //    Stopped = true;
                    //}
                    musicDownCounter += musicStep;
                }
                musicDownCounter -= 1.0;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void oneFrameRCP()
        {
            //リタルダンド処理
            if (RelativeTempoChangeSW)
            {
                nowTempo += RelativeTempoChangeTickSlice;
                if (RelativeTempoChangeTickSlice <= 0)
                {
                    if (RelativeTempoChangeTargetTempo >= nowTempo)
                    {
                        nowTempo = RelativeTempoChangeTargetTempo;
                        RelativeTempoChangeSW = false;
                    }
                }
                else
                {
                    if (RelativeTempoChangeTargetTempo <= nowTempo)
                    {
                        nowTempo = RelativeTempoChangeTargetTempo;
                        RelativeTempoChangeSW = false;
                    }
                }

                oneSyncTime = 60.0 / nowTempo / TimeBase;

            }

            bool endMark = true;
            foreach(MIDITrack tk in trk)
            {
                if (!tk.EndMark)
                {
                    endMark = false;
                    TrackProcess(tk);
                }
            }

            tick.Count++;
            //if (prj.RelativeTempoChangeSW)
            //{
            //}

            if (endMark)
            {
                Stopped = true;
            }

        }

        /// <summary>
        /// トラック毎の処理
        /// </summary>
        private void TrackProcess(MIDITrack trk)
        {
            MIDIPart prt = trk.NowPart;

            if (prt.eNowIndex == null)
            {
                if (!checkNoteOff(trk, 0))
                {
                    trk.EndMark = true;
                }
            }
            //パートの開始位置に達していないとき
            if (prt.StartTick > tick.Count)
            {
                checkNoteOff(trk, 0);
                return;
            }

            //パートの処理を実施
            PartProcess(trk);
            checkNoteOff(trk, 0);

            //次のパートに移っていることがあるので
            prt = trk.NowPart;
            //イベント送信が済んだところまでTick更新
            trk.NowTick = tick.Count - prt.StartTick;

            if (trk.EndMark) return;

            if (trk.NowPart.eNowIndex != null) return;

            //次の小節へ処理を移す
            if (prt.AfterIndex != null)
            {
                trk.NowPart = trk.getNextPart(prt);
                trk.NowTick = 0;
                trk.NextEventTick = 0;
                trk.NowPart.eNowIndex = trk.NowPart.eStartIndex;
                TrackProcess(trk);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool checkNoteOff(MIDITrack trk, int mode)
        {
            bool flg = false;

            for (int n = 0; n < trk.NoteGateTime.Length; n++)
            {
                if (trk.NoteGateTime[n] != int.MaxValue)
                {
                    if (trk.NoteGateTime[n] <= trk.NowTick + trk.NowPart.StartTick)
                    {
                        int key = (n + ((trk.Key != null) ? (int)trk.Key : 0));
                        if (key < 0) key = 0;
                        else if (key > 127) key = 127;
                        if (trk.OutChannel != null)
                        {
                            msgBuf[0] = (byte)((int)MIDIEventType.NoteOff + trk.OutChannel);
                            msgBuf[1] = (byte)key;
                            msgBuf[2] = 127;
                            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf,3);
                        }
                        trk.NoteGateTime[n] = int.MaxValue;
                        flg = true;
                    }
                }
            }

            return flg;
        }

        private byte[] vv = new byte[1];

        private void PutMIDIMessage(int n, byte[] pMIDIMessage, int len)
        {
            List<byte> dat = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                dat.Add(pMIDIMessage[i]);
//                chipRegister.sendMIDIout(model, n, vv, vstDelta);
            }
            chipRegister.sendMIDIout(model, n, dat.ToArray(), vstDelta);
        }

        /// <summary>
        /// パート毎の処理
        /// </summary>
        private void PartProcess(MIDITrack trk)
        {
            MIDIPart prt = trk.NowPart;

            //パートの情報がない場合処理しない
            if (prt == null || prt.eNowIndex == null) return;
            //パート内にイベントがない場合も処理しない
            if (prt.Event == null || prt.Event.Count == 0)
            {
                prt.eNowIndex = null;
                return;
            }

            //トラックの次のイベントまで処理をしない
            if (trk.NextEventTick > trk.NowTick) return;
            //イベントをひとつ取り出す
            MIDIEvent eve = prt.Event[(int)prt.eNowIndex];
            //誤差の算出
            eve.gosa = trk.NowTick - trk.NextEventTick;

            //イベント送信
            sendEvent(trk, eve);

            //次のイベント発動時間をセット
            trk.NextEventTick += eve.Step;

            if (trk.LoopOrSameTargetEventIndex != null)
            {
                prt.eNowIndex = trk.LoopOrSameTargetEventIndex;
                trk.LoopOrSameTargetEventIndex = null;
                PartProcess(trk);
            }
            else if (eve != null && eve.AfterIndex != null)
            {
                prt.eNowIndex = (int)eve.AfterIndex;
                PartProcess(trk);
            }
            else
            {
                prt.eNowIndex = null;
            }
        }

        /// <summary>
        /// イベント送信
        /// </summary>
        private void sendEvent(MIDITrack trk, MIDIEvent eve)
        {
            if (trk.OutDeviceNumber == null) return;
            if (trk.OutUserDeviceNumber == null) return;
            //if (!Config.MIDIOutDeviceList[(int)trk.OutUserDeviceNumber].DevAlive) return;
            if (eve.EventType == MIDIEventType.NoteON && trk.Mute) return;

            EventFunc[(int)eve.EventType](trk, eve);
        }

        void efn(MIDITrack trk, MIDIEvent eve)
        {
        }

        void ef2byteMsg(MIDITrack trk, MIDIEvent eve)
        {
            if (trk.OutChannel == null) return;
            msgBuf[0] = eve.MIDIMessage[0];
            eve.MIDIMessage[0] &= 0xf0;
            eve.MIDIMessage[0] += (byte)trk.OutChannel;
            PutMIDIMessage((int)trk.OutDeviceNumber, eve.MIDIMessage, 2);
            eve.MIDIMessage[0] = msgBuf[0];
        }

        void ef3byteMsg(MIDITrack trk, MIDIEvent eve)
        {
            if (trk.OutChannel == null) return;
            msgBuf[0] = eve.MIDIMessage[0];
            eve.MIDIMessage[0] &= 0xf0;
            eve.MIDIMessage[0] += (byte)trk.OutChannel;
            PutMIDIMessage((int)trk.OutDeviceNumber, eve.MIDIMessage, 3);
            eve.MIDIMessage[0] = msgBuf[0];
        }

        void efnbyteMsg(MIDITrack trk, MIDIEvent eve)
        {
            PutMIDIMessage((int)trk.OutDeviceNumber, eve.MIDIMessage, eve.MIDIMessage.Length);
        }

        void efMetaSeqNumber(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaSeqNumberは未実装！");
        }

        void efMetaTextEvent(MIDITrack trk, MIDIEvent eve)
        {
            trk.Comment = (Encoding.GetEncoding("Shift_JIS").GetString(eve.MIDIMessage)).Replace("\0", "");
        }

        void efMetaCopyrightNotice(MIDITrack trk, MIDIEvent eve)
        {
            //prj.Information.Copyright = (Encoding.GetEncoding("Shift_JIS").GetString(eve.MIDIMessage)).Replace("\0", "");
        }

        void efMetaTrackName(MIDITrack trk, MIDIEvent eve)
        {
            trk.Name = (Encoding.GetEncoding("Shift_JIS").GetString(eve.MIDIMessage)).Replace("\0", "");

        }

        void efMetaInstrumentName(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaInstrumentNameは未実装！");
        }

        void efMetaLyric(MIDITrack trk, MIDIEvent eve)
        {
            //ps.Lyric = (Encoding.GetEncoding("Shift_JIS").GetString(eve.MIDIMessage, 2, eve.MIDIMessage.Length - 2)).Replace("\0", "");
        }

        void efMetaMarker(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaMarkerは未実装！");
        }

        void efMetaCuePoint(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaCuePointは未実装！");
        }

        void efMetaProgramName(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaProgramNameは未実装！");
        }

        void efMetaDeviceName(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaDeviceNameは未実装！");
        }

        void efMetaChannelPrefix(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaChannelPrefixは未実装！");
        }

        void efMetaPortPrefix(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaPortPrefixは未実装！");
            Console.WriteLine(string.Format("+Track.Number[{0}] Event.Index[{1}]", trk.Number, eve.Number));
            Console.WriteLine(string.Format("+Message [{0},{1},{2}]", eve.MIDIMessage[0], eve.MIDIMessage[1], eve.MIDIMessage[2]));
        }

        void efMetaEndOfTrack(MIDITrack trk, MIDIEvent eve)
        {
            //現時点では特に何も処理する必要なし
        }

        void efMetaTempo(MIDITrack trk, MIDIEvent eve)
        {
            //int Tempo = eve.MIDIMessage[2] * 0x10000 + eve.MIDIMessage[3] * 0x100 + eve.MIDIMessage[4];
            //MIDIClock.Stop();
            //MIDIClock.SetTempo(Tempo);
            //MIDIClock.Start();
            //ps.Tempo = 60000000 / Tempo;
        }

        void efMetaSMPTEOffset(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("MetaSMPTEOffsetは未実装！");
        }

        void efMetaTimeSignature(MIDITrack trk, MIDIEvent eve)
        {
            BeatDen = (int)eve.MIDIMessage[2]; //分子
            BeatMol = (int)Math.Pow(2.0, (double)eve.MIDIMessage[3]); //分母
        }

        void efMetaKeySignature(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine(string.Format("MetaKeySignatureは未実装！Track.Number[{0}] Event.Index[{1}]", trk.Number, eve.Number));
        }

        void efNoteOff(MIDITrack trk, MIDIEvent eve)
        {
            //ef3byteMsg(trk, eve);
        }

        void efNoteOn(MIDITrack trk, MIDIEvent eve)
        {
            if (eve.Gate == 0) return;
            int okey = eve.MIDIMessage[1];
            int key = (okey + ((trk.Key != null) ? (int)trk.Key : 0));
            if (key < 0) key = 0;
            if (key > 127) key = 127;

            if (trk.OutChannel != null)
            {
                bool flg = false;
                // Key Off
                if (trk.NoteGateTime[okey] <= trk.NextEventTick + trk.NowPart.StartTick)
                {
                    msgBuf[0] = (byte)((int)MIDIEventType.NoteOff + trk.OutChannel);
                    msgBuf[1] = (byte)key;
                    msgBuf[2] = 127;
                    PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 3);
                    flg = true;
                }

                // Key On
                if (trk.NoteGateTime[okey] == int.MaxValue || flg)
                {
                    msgBuf[0] = (byte)((eve.MIDIMessage[0] & 0xf0) + trk.OutChannel);
                    msgBuf[1] = (byte)key;
                    msgBuf[2] = eve.MIDIMessage[2];
                    PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 3);
                }
            }

            trk.NoteGateTime[okey] = trk.NextEventTick + eve.Gate;
        }

        void efKeyAfterTouch(MIDITrack trk, MIDIEvent eve)
        {
            ef3byteMsg(trk, eve);
        }

        void efControlChange(MIDITrack trk, MIDIEvent eve)
        {
            ef3byteMsg(trk, eve);
        }

        void efProgramChange(MIDITrack trk, MIDIEvent eve)
        {
            ef2byteMsg(trk, eve);
        }

        void efChannelAfterTouch(MIDITrack trk, MIDIEvent eve)
        {
            ef2byteMsg(trk, eve);
        }

        void efPitchBend(MIDITrack trk, MIDIEvent eve)
        {
            ef3byteMsg(trk, eve);
        }

        void efSysExStart(MIDITrack trk, MIDIEvent eve)
        {
            efnbyteMsg(trk, eve);
        }

        void efSysExContinue(MIDITrack trk, MIDIEvent eve)
        {
            efnbyteMsg(trk, eve);
        }

        void efMetaSequencerSpecific(MIDITrack trk, MIDIEvent eve)
        {
            SpecialEventFunc[(int)eve.MIDIMessage[0]](trk, eve);
        }

        void sefUserExclusive1(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(0, trk, eve);
        }

        void sefUserExclusive2(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(1, trk, eve);
        }

        void sefUserExclusive3(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(2, trk, eve);
        }

        void sefUserExclusive4(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(3, trk, eve);
        }

        void sefUserExclusive5(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(4, trk, eve);
        }

        void sefUserExclusive6(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(5, trk, eve);
        }

        void sefUserExclusive7(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(6, trk, eve);
        }

        void sefUserExclusive8(MIDITrack trk, MIDIEvent eve)
        {
            sefUserExclusiveN(7, trk, eve);
        }

        void sefChExclusive(MIDITrack trk, MIDIEvent eve)
        {
            int i = 0;
            int j = 0;
            int chksum = 0;

            for (int b = 0; b < 30; b++) msgBuf[b] = 0;

            while (j < eve.MIDIMessageLst[0].Length - 2)
            {
                byte? n = eve.MIDIMessageLst[0][j];
                switch (n)
                {
                    case 0x80:
                        n = eve.MIDIMessageLst[0][eve.MIDIMessageLst[0].Length - 2];
                        break;
                    case 0x81:
                        n = eve.MIDIMessageLst[0][eve.MIDIMessageLst[0].Length - 1];
                        break;
                    case 0x82:
                        n = (byte)trk.OutChannel;
                        break;
                    case 0x83:
                        chksum = 0;
                        n = null;
                        break;
                    case 0x84:
                        n = (byte)(128 - (chksum % 128));
                        break;
                }
                if (n != null)
                {
                    msgBuf[i] = (byte)n;
                    chksum += (byte)n;
                    i++;
                }
                j++;
                if (n == 0xf7) break;
                if (i >= msgBuf.Length)
                {
                    Console.WriteLine("sefChExclusive:バッファをオーバーするエクスクルーシブを検知しスキップ。");
                    return;//バッファをオーバーする時はエクスクルーシブを送らない
                }
            }
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, i);
        }

        void sefOutsideProcessExec(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("spEventOutsideProcessExecは未実装！");
        }

        void sefBankProgram(MIDITrack trk, MIDIEvent eve)
        {
            msgBuf[0] = (byte)(eve.MIDIMessageLst[1][0] + (trk.OutChannel % 16));
            msgBuf[1] = eve.MIDIMessageLst[1][1];
            msgBuf[2] = eve.MIDIMessageLst[1][2];
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 3);
            msgBuf[0] = (byte)(eve.MIDIMessageLst[0][0] + (trk.OutChannel % 16));
            msgBuf[1] = eve.MIDIMessageLst[0][1];
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 2);
        }

        void sefKeyScan(MIDITrack trk, MIDIEvent eve)
        {
            Console.WriteLine("spEventKeyScanは未実装！");
        }

        void sefMIDIChChange(MIDITrack trk, MIDIEvent eve)
        {
            int ch = eve.MIDIMessageLst[0][0];
            if (ch == 0)
            {
                trk.Mute = true;
                return;
            }
            trk.Mute = false;
            ch--;
            trk.OutDeviceNumber = ch / 16;
            trk.OutChannel = ch % 16;
        }

        void sefTempoChange(MIDITrack trk, MIDIEvent eve)
        {
            double mul = eve.MIDIMessageLst[0][0] / 64.0;

            if (eve.MIDIMessageLst[0][1] == 0)
            {
                int Tempo = (int)(this.Tempo * mul);
                if (Tempo < 10) Tempo = 10;
                else if (Tempo > 240) Tempo = 240;
                nowTempo = Tempo;
                oneSyncTime = 60.0 / nowTempo / TimeBase;
            }
            else
            {
                //リタルダンド
                int Tempo = (int)((double)this.Tempo * mul);
                double s = (double)(Tempo - this.Tempo) * 256.0 / ((256.0 - eve.MIDIMessageLst[0][1]) * TimeBase);
                RelativeTempoChangeTargetTempo = Tempo;
                RelativeTempoChangeTickSlice = (nowTempo < Tempo) ? s : -s;
                RelativeTempoChangeSW = true;
            }
        }

        void sefYAMAHABase(MIDITrack trk, MIDIEvent eve)
        {
            trk.YAMAHABase_gt = eve.MIDIMessageLst[0][0];
            trk.YAMAHABase_vel = eve.MIDIMessageLst[0][1];
        }

        void sefYAMAHADev(MIDITrack trk, MIDIEvent eve)
        {
            trk.YAMAHA_dev = eve.MIDIMessageLst[0][0];
            trk.YAMAHA_model = eve.MIDIMessageLst[0][1];
        }

        void sefYAMAHAAddrPara(MIDITrack trk, MIDIEvent eve)
        {
            trk.YAMAHAPara_gt = eve.MIDIMessageLst[0][0];
            trk.YAMAHAPara_vel = eve.MIDIMessageLst[0][1];

            msgBuf[0] = 0xf0;
            msgBuf[1] = 0x43;
            msgBuf[2] = trk.YAMAHA_dev;
            msgBuf[3] = trk.YAMAHA_model;
            msgBuf[4] = trk.YAMAHABase_gt;
            msgBuf[5] = trk.YAMAHABase_vel;
            msgBuf[6] = trk.YAMAHAPara_gt;
            msgBuf[7] = trk.YAMAHAPara_vel;
            msgBuf[8] = 0xf7;
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 9);
        }

        void sefYAMAHAXGAddrPara(MIDITrack trk, MIDIEvent eve)
        {
            trk.YAMAHAPara_gt = eve.MIDIMessageLst[0][0];
            trk.YAMAHAPara_vel = eve.MIDIMessageLst[0][1];

            msgBuf[0] = 0xf0;
            msgBuf[1] = 0x43;
            msgBuf[2] = 0x10;
            msgBuf[3] = 0x4c;
            msgBuf[4] = trk.YAMAHABase_gt;
            msgBuf[5] = trk.YAMAHABase_vel;
            msgBuf[6] = trk.YAMAHAPara_gt;
            msgBuf[7] = trk.YAMAHAPara_vel;
            msgBuf[8] = 0xf7;
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 9);
        }

        void sefRolandBase(MIDITrack trk, MIDIEvent eve)
        {
            trk.RolandBase_gt = eve.MIDIMessageLst[0][0];
            trk.RolandBase_vel = eve.MIDIMessageLst[0][1];
        }

        void sefRolandPara(MIDITrack trk, MIDIEvent eve)
        {
            trk.RolandPara_gt = eve.MIDIMessageLst[0][0];
            trk.RolandPara_vel = eve.MIDIMessageLst[0][1];

            msgBuf[0] = 0xF0;
            msgBuf[1] = 0x41;
            msgBuf[2] = trk.RolandDev_gt;
            msgBuf[3] = trk.RolandDev_vel;
            msgBuf[4] = 0x12;
            msgBuf[5] = trk.RolandBase_gt;
            msgBuf[6] = trk.RolandBase_vel;
            msgBuf[7] = trk.RolandPara_gt;
            msgBuf[8] = trk.RolandPara_vel;
            msgBuf[9] = (byte)((128 - ((trk.RolandBase_gt + trk.RolandBase_vel + trk.RolandPara_gt + trk.RolandPara_vel) % 128)) & 0x7f);
            msgBuf[10] = 0xF7;
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, 11);
        }

        void sefRolandDev(MIDITrack trk, MIDIEvent eve)
        {
            trk.RolandDev_gt = eve.MIDIMessageLst[0][0];
            trk.RolandDev_vel = eve.MIDIMessageLst[0][1];
        }

        void sefKeyChange(MIDITrack trk, MIDIEvent eve)
        {
            int sf, mi;
            int v, vv;

            v = eve.Step;
            vv = v % 0x10;
            sf = vv > 0x07 ? (0x100 - vv) % 0x100 : vv;
            mi = v > 0x0f ? 1 : 0;

            trk.KeySIG_SF= sf; //Sharp Flat -7:7flats -1:1flat 0:Key of C 1:1sharp 7:7sharp
            trk.KeySIG_MI = mi; // Is minor Key
        }

        void sefCommentStart(MIDITrack trk, MIDIEvent eve)
        {
            trk.Comment = (Encoding.GetEncoding("Shift_JIS").GetString(eve.MIDIMessageLst[0])).Replace("\0", "");
            chipRegister.midiParams[0].Lyric = trk.Comment;
        }

        void sefLoopEnd(MIDITrack trk, MIDIEvent eve)
        {
            if (trk.LoopTargetEvent.Count == 0) return;
            MIDIEvent evt = trk.LoopTargetEvent.Pop();
            if (evt.MIDIMessageLst[0][0] < eve.MIDIMessageLst[0][0] - 1)
            {
                evt.MIDIMessageLst[0][0]++;
                trk.LoopTargetEvent.Push(evt);
                trk.LoopOrSameTargetEventIndex = trk.NowPart.getNextEvent(evt).Number;
            }
            else if (eve.MIDIMessageLst[0][0] == 0)
            {
                trk.LoopTargetEvent.Push(evt);
                trk.LoopOrSameTargetEventIndex = trk.NowPart.getNextEvent(evt).Number;
            }
        }

        void sefLoopStart(MIDITrack trk, MIDIEvent eve)
        {
            MIDIEvent evt = trk.NowPart.Event[(int)trk.NowPart.eNowIndex];
            evt.MIDIMessageLst[0][0] = 0;
            trk.LoopTargetEvent.Push(evt);
        }

        void sefSameMeasure(MIDITrack trk, MIDIEvent eve)
        {
            if (trk.SameMeasure != null)
            {
                trk.LoopOrSameTargetEventIndex = trk.SameMeasure;
                trk.SameMeasure = null;
                return;
            }
            trk.LoopOrSameTargetEventIndex = eve.SameMeasureIndex;
            trk.SameMeasure = trk.NowPart.getNextEvent(eve).Number;
            while (trk.NowPart.Event[(int)trk.LoopOrSameTargetEventIndex].EventType == MIDIEventType.MetaSequencerSpecific
                && trk.NowPart.Event[(int)trk.LoopOrSameTargetEventIndex].MIDIMessage[0] == (byte)MIDISpEventType.SameMeasure)
            {
                trk.LoopOrSameTargetEventIndex = trk.NowPart.Event[(int)trk.LoopOrSameTargetEventIndex].SameMeasureIndex;
            }
        }

        void sefMeasureEnd(MIDITrack trk, MIDIEvent eve)
        {
            if (trk.SameMeasure != null)
            {
                trk.LoopOrSameTargetEventIndex = trk.SameMeasure;
                trk.SameMeasure = null;
                return;
            }
        }

        void sefEndofTrack(MIDITrack trk, MIDIEvent eve)
        {
            trk.EndMark = true;
        }

        void sefDX7Func(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefDXPara(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefDXRERF(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTXFunc(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefFB01PPara(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefFB01SSystem(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTX81ZVVCED(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTX81ZAACED(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTX81ZPPCED(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTX81ZSSystem(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTX81ZEEffect(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefDX72RRemoteSW(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefDX72AACED(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefDX72PPCED(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefTX802PPCED(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefMKS7(MIDITrack trk, MIDIEvent eve)
        {
        }

        void sefUserExclusiveN(int num, MIDITrack trk, MIDIEvent eve)
        {
            int i = 0;
            int j = 0;
            int chksum = 0;

            while (j < UserExclusive[num].Exclusive.Length)
            {
                byte? n = UserExclusive[num].Exclusive[j];
                switch (n)
                {
                    case 0x80:
                        n = eve.MIDIMessageLst[0][0];
                        break;
                    case 0x81:
                        n = eve.MIDIMessageLst[0][1];
                        break;
                    case 0x82:
                        n = (byte)trk.OutChannel;
                        break;
                    case 0x83:
                        chksum = 0;
                        n = null;
                        break;
                    case 0x84:
                        n = (byte)((128 - (chksum % 128)) & 0x7f);
                        break;
                }
                if (n != null)
                {
                    msgBuf[i] = (byte)n;
                    chksum += (byte)n;
                    i++;
                }
                j++;
                if (n == 0xf7) break;
                if (i >= msgBuf.Length)
                {
                    Console.WriteLine("sefUserExclusiveN:バッファをオーバーするエクスクルーシブを検知しスキップ。");
                    return;//バッファをオーバーする時はエクスクルーシブを送らない
                }
            }
            PutMIDIMessage((int)trk.OutDeviceNumber, msgBuf, i);
        }


    }
}
