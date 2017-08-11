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
        private double musicStep = 44100.0 / 60.0;
        private double musicDownCounter = 0.0;

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


        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (buf == null) return null;

            GD3 gd3 = new GD3();
            string T01TrackName = "";

            try
            {
                if (common.getLE32(buf, 0) != FCC_MID) return null;
                uint format = (uint)(buf[8]*0x100+buf[9]);
                uint trkCount = (uint)(buf[10] * 0x100 + buf[11]);
                uint adr = 14;
                byte midiEventBackup = 0;

                for (int i = 0; i < trkCount; i++)
                {
                    if (buf.Length <= adr) break;

                    if (common.getLE32(buf, adr) != FCC_TRK) return null;
                    uint len = (uint)(buf[adr + 4] * 0x1000000 + buf[adr + 5] * 0x10000 + buf[adr + 6] * 0x100 + buf[adr + 7]);
                    adr += 8;
                    uint trkEndadr = adr + len;

                    while (adr < trkEndadr && adr < buf.Length)
                    {
                        int delta = common.getDelta(ref adr, buf);
                        byte cmd = buf[adr++];
                        if (cmd == 0xf0 || cmd == 0xf7)
                        {
                            uint bAdr = adr - 1;
                            int datalen = common.getDelta(ref adr, buf);
                            adr = adr + (uint)datalen;
                        }
                        else if (cmd == 0xff)
                        {
                            byte eventType = buf[adr++];
                            uint eventLen = (uint)common.getDelta(ref adr, buf);
                            List<byte> eventData = new List<byte>();
                            for (int j = 0; j < eventLen; j++)
                            {
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
                                        //case 0x04:
                                        //case 0x05:
                                        //case 0x06:
                                        //case 0x07:
                                        if (gd3.TrackName == "")
                                        {
                                            if (format==0 || (format==1 && i == 0))
                                            {
                                                gd3.TrackName = Encoding.GetEncoding(932).GetString(eventData.ToArray()).Trim();
                                                gd3.TrackNameJ = Encoding.GetEncoding(932).GetString(eventData.ToArray()).Trim();
                                            }
                                        }
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

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip useChip, uint latency)
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

        private bool getInformationHeader()
        {
            if (vgmBuf == null) return false;
            if (common.getLE32(vgmBuf, 0) != FCC_MID) return false;

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

                if (common.getLE32(vgmBuf, adr) != FCC_TRK) return false;
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
            //if (model == enmModel.VirtualModel) return;

            try
            {
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

                musicStep = 44100.0 * oneSyncTime;

                if (musicDownCounter <= 0.0)
                {
                    //midWaitCounter--;
                    //if (midWaitCounter <= 0) 
                    oneFrameMID();
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
                    //Console.Write("[{0:X8}]:[{1:D2}] ", ptr, trk);

                    if (isDelta[trk])
                    {
                        delta = common.getDelta(ref ptr, vgmBuf);
                        midWaitCounter[trk] = delta;

                        //Console.WriteLine("delta:{0:D10} ", delta);
                    }
                    else
                    {
                        byte cmd = vgmBuf[ptr++];

                        //Console.Write("cmd:{1:X2} ", delta, cmd);

                        if (cmd == 0xf0 || cmd == 0xf7)
                        {
                            uint eventLen = (uint)common.getDelta(ref ptr, vgmBuf);
                            //Console.Write("evntLen:{0:D10} ", eventLen);
                            List<byte> eventData = new List<byte>();
                            eventData.Add(cmd);
                            //Console.Write("{0:X2} ", cmd);
                            for (int j = 0; j < eventLen; j++)
                            {
                                eventData.Add(vgmBuf[ptr + j]);
                                //Console.Write("{0:X2} ", vgmBuf[ptr + j]);
                            }

                            chipRegister.sendMIDIout(model, trkPort[trk], eventData.ToArray());

                            ptr = ptr + eventLen;
                            //Console.WriteLine("");
                        }
                        else if (cmd == 0xff)
                        {
                            byte eventType = vgmBuf[ptr++];
                            //uint bAdr = ptr - 1;
                            uint eventLen = (uint)common.getDelta(ref ptr, vgmBuf);

                            //Console.Write("evntTyp:{0:X2} evntLen:{1:D10} ", eventType, eventLen);

                            List<byte> eventData = new List<byte>();
                            for (int j = 0; j < eventLen; j++)
                            {
                                eventData.Add(vgmBuf[ptr + j]);
                                //Console.Write("{0:X2} ", vgmBuf[ptr + j]);
                            }
                            ptr = ptr + eventLen;
                            if (eventData.Count > 0)
                            {
                                switch (eventType)
                                {
                                    case 0x01:
                                    case 0x02:
                                    case 0x03:
                                    case 0x04:
                                    case 0x05:
                                    case 0x06:
                                    case 0x07:
                                        //Console.Write("{0:X2}:{1}", eventType, Encoding.GetEncoding(932).GetString(eventData.ToArray()));
                                        break;
                                    case 0x21:
                                        trkPort[trk] = eventData[0];
                                        //Console.Write("PortPrefix:{0}", eventData[0]);
                                        break;
                                    case 0x2f:
                                        ptr = trkEndAdr[trk];
                                        //Console.Write("End of Track");
                                        break;
                                    case 0x51:
                                        int Tempo = eventData[0] * 0x10000 + eventData[1] * 0x100 + eventData[2];
                                        // reso 4分音符当たりの分解能
                                        // tempo 4分音符当たりのマイクロ秒
                                        oneSyncTime = (double)(Tempo / reso) * 0.000001;
                                        //Console.Write("Set Tempo:{0}", Tempo);
                                        break;
                                    case 0x54:
                                        //Console.Write("SMPTE Offset ");
                                        break;
                                    case 0x58:
                                        //Console.Write("Time Signature");
                                        break;
                                    case 0x59:
                                        //Console.Write("Key Signature");
                                        break;
                                    case 0x7f:
                                        //Console.Write("Sequencer Specific Meta-Event ");
                                        break;
                                    default:
                                        //Console.Write("!! Unknown Meta Event !! eventType:{0:X2} Adr:{1:X}", eventType, ptr);
                                        break;
                                }
                            }
                            //Console.WriteLine("");
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
                                    chipRegister.sendMIDIout(model, trkPort[trk], cmd, vgmBuf[ptr], vgmBuf[ptr + 1]);
                                    //Console.WriteLine("V1:{0:X2} V2:{1:X2} ", vgmBuf[ptr], vgmBuf[ptr + 1]);
                                    ptr += 2;
                                }
                                else
                                {
                                    chipRegister.sendMIDIout(model, trkPort[trk], cmd, vgmBuf[ptr]);
                                    //Console.WriteLine("V1:{0:X2} V2:-- ", vgmBuf[ptr]);
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
                                    chipRegister.sendMIDIout(model, trkPort[trk], midiEvent, cmd, vgmBuf[ptr]);
                                    ptr++;
                                }
                                else
                                {
                                    chipRegister.sendMIDIout(model, trkPort[trk], midiEvent, cmd);
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

    }
}
