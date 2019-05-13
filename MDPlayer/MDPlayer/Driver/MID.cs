using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class MID : baseDriver
    {
        public const int FCC_MID = 0x6468544d;
        public const int FCC_TRK = 0x6b72544d;
        public uint format = 0;
        public uint trkCount = 0;
        public uint reso = 196;

        private double oneSyncTime = 0.0001;
        private double musicStep = Common.SampleRate / 60.0;
        private double musicDownCounter = 0.0;

        private List<RCP.CtlSysex>[] beforeSend = null;
        private int[] sendControlDelta = null;
        private int[] sendControlIndex = null;

        private List<uint> musicPtr = null;
        private List<uint> trkEndAdr = null;
        private List<int> trkPort = null;
        private List<int> midWaitCounter = null;
        private List<bool> isEnd = null;
        private List<bool> isDelta = null;

        byte midiEvent = 0;
        List<byte> midiEventBackup = null;
        byte midiEventCh = 0;
        byte midiEventChBackup = 0;
        private List<byte> eventStr = new List<byte>();
        private string eventText = "";
        private string eventCopyrightNotice = "";
        private string eventSequenceTrackName = "";
        private string eventInstrumentName = "";
        private string eventLyric = "";
        private string eventMarker = "";


        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (buf == null) return null;

            GD3 gd3 = new GD3();
            string T01TrackName = "";


            try
            {
                if (Common.getLE32(buf, 0) != FCC_MID) return null;
                uint format = (uint)(buf[8]*0x100+buf[9]);
                uint trkCount = (uint)(buf[10] * 0x100 + buf[11]);
                uint adr = 14;
                byte midiEventBackup = 0;

                for (int i = 0; i < trkCount; i++)
                {
                    if (buf.Length <= adr) break;

                    if (Common.getLE32(buf, adr) != FCC_TRK) return null;
                    uint len = (uint)(buf[adr + 4] * 0x1000000 + buf[adr + 5] * 0x10000 + buf[adr + 6] * 0x100 + buf[adr + 7]);
                    adr += 8;
                    uint trkEndadr = adr + len;

                    while (adr < trkEndadr && adr < buf.Length)
                    {
                        int delta = Common.getDelta(ref adr, buf);
                        byte cmd = buf[adr++];
                        if (cmd == 0xf0 || cmd == 0xf7)
                        {
                            uint bAdr = adr - 1;
                            int datalen = Common.getDelta(ref adr, buf);
                            adr = adr + (uint)datalen;
                        }
                        else if (cmd == 0xff)
                        {
                            byte eventType = buf[adr++];
                            uint eventLen = (uint)Common.getDelta(ref adr, buf);
                            List<byte> eventData = new List<byte>();
                            for (int j = 0; j < eventLen; j++)
                            {
                                if (buf[adr + j] == 0) break;
                                eventData.Add(buf[adr + j]);
                            }
                            adr = adr + eventLen;
                            if (eventData.Count > 0)
                            {
                                switch (eventType)
                                {
                                    case 0x01:
                                        //case 0x02:
                                        if (T01TrackName == "")
                                        {
                                            T01TrackName = Encoding.GetEncoding(932).GetString(eventData.ToArray()).Trim();
                                        }
                                        break;
                                    case 0x03:
                                        if (gd3.TrackName == "")
                                        {
                                            if (format == 0 || (format == 1 && i == 0))
                                            {
                                                gd3.TrackName = Encoding.GetEncoding(932).GetString(eventData.ToArray()).Trim();
                                                gd3.TrackNameJ = Encoding.GetEncoding(932).GetString(eventData.ToArray()).Trim();
                                            }
                                        }
                                        break;
                                    case 0x05:
                                        //case 0x04:
                                        //case 0x06:
                                        //case 0x07:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if ((cmd & 0x80) != 0)
                            {
                                midiEventBackup = (byte)(cmd & 0xff);
                                midiEvent = midiEventBackup;

                                if ((cmd & 0xf0) != 0xC0 && (cmd & 0xf0) != 0xD0)
                                {
                                    adr += 2;
                                }
                                else
                                {
                                    adr++;
                                }
                            }
                            else
                            {
                                //ランニングステータス発動
                                midiEvent = midiEventBackup;
                                midiEventCh = midiEventChBackup;

                                if ((cmd & 0xf0) != 0xC0 && (cmd & 0xf0) != 0xD0)
                                {
                                    adr++;
                                }
                            }
                        }

                    }

                }

                //タイトルが見つからなかった場合
                if (gd3.TrackName == "" && gd3.TrackNameJ == "" && T01TrackName != "")
                {
                    gd3.TrackName = T01TrackName;
                    gd3.TrackNameJ = T01TrackName;
                }
            }
            catch
            {
            }

            return gd3;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
        {
            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            //コントロールを送信してからウェイトするためここでは0をセットする
            //vgmFrameCounter = -latency - waitTime;
            vgmFrameCounter = 0;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            GD3 = getGD3Info(vgmBuf, 0);
            //if (GD3 == null) return false;

            if (!getInformationHeader()) return false;

            //ポートごとに事前に送信するコマンドを作成する
            if (!MakeBeforeSendCommand()) return false;

            //if (model == EnmModel.RealModel)
            //{
            //    chipRegister.setYM2612SyncWait(0, 1);
            //    chipRegister.setYM2612SyncWait(1, 1);
            //}

            return true;
        }

        private bool getInformationHeader()
        {
            if (vgmBuf == null) return false;
            if (Common.getLE32(vgmBuf, 0) != FCC_MID) return false;

            format = (uint)(vgmBuf[8] * 0x100 + vgmBuf[9]);
            trkCount = (uint)(vgmBuf[10] * 0x100 + vgmBuf[11]);
            reso = (uint)(vgmBuf[12] * 0x100 + vgmBuf[13]);

            musicPtr = new List<uint>();
            midWaitCounter = new List<int>();
            isEnd = new List<bool>();
            trkEndAdr = new List<uint>();
            trkPort = new List<int>();
            isDelta = new List<bool>();
            midiEventBackup = new List<byte>();

            uint adr = 14;
            for (uint i = 0; i < trkCount; i++)
            {
                midWaitCounter.Add(0);
                isEnd.Add(false);
                isDelta.Add(true);

                if (Common.getLE32(vgmBuf, adr) != FCC_TRK) return false;
                uint len = (uint)(vgmBuf[adr + 4] * 0x1000000 + vgmBuf[adr + 5] * 0x10000 + vgmBuf[adr + 6] * 0x100 + vgmBuf[adr + 7]);
                adr += 8;
                musicPtr.Add(adr);
                adr += len;
                trkEndAdr.Add(adr);
                trkPort.Add(0);
                midiEventBackup.Add(0);

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

        private void oneFrameMain()
        {
            try
            {

                Counter++;
                vgmFrameCounter++;
                Audio.DriverSeqCounter++;

                musicStep = Common.SampleRate * oneSyncTime;

                if (musicDownCounter <= 0.0)
                {
                    if (beforeSend != null)
                    {
                        sendControl();
                    }
                    else
                    {
                        oneFrameMID();
                    }
                    musicDownCounter += musicStep;
                }
                musicDownCounter -= 1.0;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void oneFrameMID()
        {
#if DEBUG
            //if (model == enmModel.RealModel) return;
#endif
            bool trksEnd = true;
            for (int trk = 0; trk < trkCount; trk++)
            {
                if (isEnd[trk]) continue;

                trksEnd = false;
                midWaitCounter[trk]--;
                if (midWaitCounter[trk] > 0) continue;

                while (midWaitCounter[trk] < 1)
                {
                    uint ptr = musicPtr[trk];
                    int delta = 0;
#if DEBUG
                    Console.WriteLine("");
                    Console.Write("ptr:[{0:X8}] trk:[{1:D2}] ", ptr, trk);
#endif

                    if (isDelta[trk])
                    {
                        delta = Common.getDelta(ref ptr, vgmBuf);
                        midWaitCounter[trk] = delta;

#if DEBUG
                        Console.Write("delta:{0:D10} ", delta);
#endif
                    }
                    else
                    {
                        byte cmd = vgmBuf[ptr++];

#if DEBUG
                        //Console.Write("cmd:{1:X2} ", delta, cmd);
#endif

                        if (cmd == 0xf0 || cmd == 0xf7)
                        {
                            uint eventLen = (uint)Common.getDelta(ref ptr, vgmBuf);
#if DEBUG
                            //Console.Write("evntLen:{0:D10} ", eventLen);
                            Console.Write("{0:X2} ", cmd);
#endif
                            List<byte> eventData = new List<byte>();
                            eventData.Add(cmd);
                            for (int j = 0; j < eventLen; j++)
                            {
                                eventData.Add(vgmBuf[ptr + j]);
#if DEBUG
                                Console.Write("{0:X2} ", vgmBuf[ptr + j]);
#endif
                            }

                            chipRegister.sendMIDIout(Audio.DriverSeqCounter, trkPort[trk], eventData.ToArray());//,vstDelta);

                            ptr = ptr + eventLen;

                        }
                        else if (cmd == 0xff)
                        {
                            byte eventType = vgmBuf[ptr++];
                            uint eventLen = (uint)Common.getDelta(ref ptr, vgmBuf);

#if DEBUG
                            Console.Write("evntTyp:{0:X2} evntLen:{1:D10} ", eventType, eventLen);
#endif

                            List<byte> eventData = new List<byte>();
                            for (int j = 0; j < eventLen; j++)
                            {
                                eventData.Add(vgmBuf[ptr + j]);
#if DEBUG
                                Console.Write("{0:X2} ", vgmBuf[ptr + j]);
#endif
                            }
                            ptr = ptr + eventLen;
                            if (eventData.Count > 0)
                            {
                                //文字列系のイベントの場合は終端文字までを文字列のデータとする。
                                if (eventType >= 0x01 && eventType <= 0x07)
                                {
                                    eventStr.Clear();
                                    foreach (byte b in eventData)
                                    {
                                        if (b == 0) break;
                                        eventStr.Add(b);
                                    }
                                }

                                switch (eventType)
                                {
                                    case 0x01:
                                        eventText = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventText);
#if DEBUG
                                        Console.Write("eventText:{0}", eventText);
#endif
                                        break;
                                    case 0x02:
                                        eventCopyrightNotice = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventCopyrightNotice);
#if DEBUG
                                        Console.Write("eventCopyrightNotice:{0}", eventCopyrightNotice);
#endif
                                        break;
                                    case 0x03:
                                        eventSequenceTrackName = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventSequenceTrackName);
#if DEBUG
                                        Console.Write("eventSequenceTrackName:{0}", eventSequenceTrackName);
#endif
                                        break;
                                    case 0x04:
                                        eventInstrumentName = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventInstrumentName);
#if DEBUG
                                        Console.Write("eventInstrumentName:{0}", eventInstrumentName);
#endif
                                        break;
                                    case 0x05:
                                        eventLyric = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventLyric);
#if DEBUG
                                        Console.Write("eventLyric:{0}", eventLyric);
#endif
                                        chipRegister.midiParams[trkPort[trk]].Lyric = eventLyric;
                                        break;
                                    case 0x06:
                                        eventMarker = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventMarker);
#if DEBUG
                                        Console.Write("eventMarker:{0}", eventMarker);
#endif
                                        break;
                                    case 0x07:
                                        eventText = Encoding.GetEncoding(932).GetString(eventData.ToArray());
                                        chipRegister.sendMIDIoutLyric(Audio.DriverSeqCounter, 0, eventText);
#if DEBUG
                                        Console.Write("eventText:{0}", eventText);
#endif
                                        break;
                                    case 0x21:
                                        trkPort[trk] = eventData[0];
#if DEBUG
                                        Console.Write("PortPrefix:{0}", trkPort[trk]);
#endif
                                        break;
                                    case 0x2f:
                                        ptr = trkEndAdr[trk];
#if DEBUG
                                        Console.Write("End of Track:{0}", ptr);
#endif
                                        break;
                                    case 0x51:
                                        int Tempo = eventData[0] * 0x10000 + eventData[1] * 0x100 + eventData[2];
                                        // reso 4分音符当たりの分解能
                                        // tempo 4分音符当たりのマイクロ秒
                                        oneSyncTime = (double)(Tempo / reso) * 0.000001;
#if DEBUG
                                        Console.Write("Set Tempo:{0}", Tempo);
#endif
                                        break;
                                    case 0x54:
#if DEBUG
                                        Console.Write("SMPTE Offset ");
#endif
                                        break;
                                    case 0x58:
#if DEBUG
                                        Console.Write("Time Signature");
#endif
                                        break;
                                    case 0x59:
#if DEBUG
                                        Console.Write("Key Signature");
#endif
                                        break;
                                    case 0x7f:
#if DEBUG
                                        Console.Write("Sequencer Specific Meta-Event ");
#endif
                                        break;
                                    default:
#if DEBUG
                                        Console.Write("!! Unknown Meta Event !! eventType:{0:X2} Adr:{1:X}", eventType, ptr);
#endif
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if ((cmd & 0x80) != 0)
                            {
                                midiEventBackup[trk] = (byte)(cmd & 0xff);
                                midiEventChBackup = (byte)(cmd & 0x0f);
                                midiEvent = midiEventBackup[trk];
                                midiEventCh = midiEventChBackup;

                                if ((cmd & 0xf0) != 0xC0 && (cmd & 0xf0) != 0xD0)
                                {
                                    chipRegister.sendMIDIout(Audio.DriverSeqCounter, trkPort[trk], cmd, vgmBuf[ptr], vgmBuf[ptr + 1]);//, vstDelta);
#if DEBUG
                                    //Console.Write("V1:{0:X2} V2:{1:X2} ", vgmBuf[ptr], vgmBuf[ptr + 1]);
                                    Console.Write("{0:X2} {1:X2} {2:X2}",cmd, vgmBuf[ptr], vgmBuf[ptr + 1]);
#endif
                                    ptr += 2;
                                }
                                else
                                {
                                    chipRegister.sendMIDIout(Audio.DriverSeqCounter, trkPort[trk], cmd, vgmBuf[ptr]);//, vstDelta);
#if DEBUG
                                    //Console.Write("V1:{0:X2} V2:-- ", vgmBuf[ptr]);
                                    Console.Write("{0:X2} {1:X2}", cmd, vgmBuf[ptr]);
#endif
                                    ptr++;
                                }
                            }
                            else
                            {
                                //ランニングステータス発動
                                midiEvent = midiEventBackup[trk];
                                midiEventCh = midiEventChBackup;

                                if ((cmd & 0xf0) != 0xC0 && (cmd & 0xf0) != 0xD0)
                                {
                                    chipRegister.sendMIDIout(Audio.DriverSeqCounter, trkPort[trk], midiEvent, cmd, vgmBuf[ptr]);//, vstDelta);
#if DEBUG
                                    //Console.Write("RunSta V1:{0:X2} V2:{1:X2} ", cmd, vgmBuf[ptr]);
                                    Console.Write("{0:X2} {1:X2} {2:X2}", midiEvent, cmd, vgmBuf[ptr]);
#endif
                                    ptr++;
                                }
                                else
                                {
                                    chipRegister.sendMIDIout(Audio.DriverSeqCounter, trkPort[trk], midiEvent, cmd);//, vstDelta);
#if DEBUG
                                    //Console.Write("RunSta V1:{0:X2} V2:-- ", cmd);
                                    Console.Write("{0:X2} {1:X2} ", midiEvent, cmd);
#endif
                                }
                            }


                        }


                    }

                    isDelta[trk] = !isDelta[trk];

                    musicPtr[trk] = ptr;
                    if (ptr == trkEndAdr[trk])
                    {
                        isEnd[trk] = true;
                        break;
                    }

                }
            }

            if (trksEnd)
            {
                Stopped = true;
            }
        }

        private void sendControl()
        {

            int endFlg = 0;

            for (int i = 0; i < beforeSend.Length; i++)
            {
                if (beforeSend[i] != null)
                {
                    if (sendControlDelta[i] > 0)
                    {
                        sendControlDelta[i]--;
                        continue;
                    }
                    if (beforeSend[i].Count == sendControlIndex[i])
                    {
                        beforeSend[i] = null;
                        endFlg++;
                        continue;
                    }

                    oneSyncTime = 60.0 / 29.0 / 192.0;

                    RCP.CtlSysex csx = beforeSend[i][sendControlIndex[i]];
                    sendControlDelta[i] = csx.delta;
                    chipRegister.sendMIDIout(Audio.DriverSeqCounter, 0, csx.data);//, vstDelta);

                    sendControlIndex[i]++;
                }
                else
                {
                    endFlg++;
                }

            }

            if (endFlg == beforeSend.Length)
            {
                beforeSend = null;
                vgmFrameCounter = -latency - waitTime;
                return;
            }

            return;
        }

        private bool MakeBeforeSendCommand()
        {
            try
            {
                midiOutInfo[] infos = chipRegister.GetMIDIoutInfo();
                if (infos == null || infos.Length < 1) return true;

                beforeSend = new List<RCP.CtlSysex>[infos.Length];
                sendControlIndex = new int[infos.Length];
                sendControlDelta = new int[infos.Length];

                for (int i = 0; i < beforeSend.Length; i++)
                {
                    beforeSend[i] = new List<RCP.CtlSysex>();

                    //リセットを生成
                    switch (infos[i].beforeSendType)
                    {
                        case 0://None
                            break;
                        case 1://GM Reset
                            GetCtlSysexFromText(beforeSend[i], setting.midiOut.GMReset);
                            break;
                        case 2://XG Reset
                            GetCtlSysexFromText(beforeSend[i], setting.midiOut.XGReset);
                            break;
                        case 3://GS Reset
                            GetCtlSysexFromText(beforeSend[i], setting.midiOut.GSReset);
                            break;
                        case 4://Custom
                            GetCtlSysexFromText(beforeSend[i], setting.midiOut.Custom);
                            break;
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void GetCtlSysexFromText(List<RCP.CtlSysex> buf, string text)
        {
            if (text == null || text.Length < 1) return;

            string[] cmds = text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string cmd in cmds)
            {
                string[] com = cmd.Split(new string[] { ":" }, StringSplitOptions.None);
                int delay = int.Parse(com[0]);
                string[] dats = com[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                byte[] dat = new byte[dats.Length];
                for (int i = 0; i < dats.Length; i++)
                {
                    dat[i] = (byte)Convert.ToInt16(dats[i], 16);
                }
                buf.Add(new RCP.CtlSysex(delay, dat));
            }
        }



    }
}
