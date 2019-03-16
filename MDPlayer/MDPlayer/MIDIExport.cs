using System;
using System.Collections.Generic;
using System.IO;

namespace MDPlayer
{
    public class MIDIExport
    {

        private Setting setting = null;

        private midiChip midi2151 = new midiChip();
        private midiChip midi2612 = new midiChip();

        private List<byte> cData = null;

        public string PlayingFileName = "";
        public int[][][] fmRegisterYM2612 = null;
        public int[][] fmRegisterYM2151 = null;

        public MIDIExport(Setting setting)
        {
            this.setting = setting;
        }

        public void outMIDIData(EnmChip chip, int chipID, int dPort, int dAddr, int dData,int hosei, long vgmFrameCounter)
        {
            if (!setting.midiExport.UseMIDIExport) return;
            if (setting.midiExport.ExportPath == "") return;
            if (vgmFrameCounter < 0) return;
            if (chipID != 0) return;

            if (chip != EnmChip.YM2612 && chip != EnmChip.YM2151) return;

            switch (chip)
            {
                case EnmChip.YM2151:
                    if (setting.midiExport.UseYM2151Export)
                    {
                        outMIDIData_YM2151(chipID, dPort, dAddr, dData, hosei, vgmFrameCounter);
                    }
                    break;
                case EnmChip.YM2612:
                    if (setting.midiExport.UseYM2612Export)
                    {
                        outMIDIData_YM2612(chipID, dPort, dAddr, dData, vgmFrameCounter);
                    }
                    break;
            }
        }


        public void Close()
        {
            if (!setting.midiExport.UseMIDIExport) return;
            if (setting.midiExport.ExportPath == "") return;
            if (cData == null) return;
            //if (midiData.Count < 23) return;

            int portNum = 0;
            int trkNum = 1;

            if (setting.midiExport.UseYM2151Export)
            {
                for (int ch = 0; ch < midi2151.maxTrk; ch++)
                {
                    SetDelta(ch, midi2151, midi2151.oldFrameCounter[ch]);

                    if (midi2151.oldCode[ch] >= 0)
                    {

                        midi2151.data[ch].Add((byte)(0x80 | ch));
                        midi2151.data[ch].Add((byte)midi2151.oldCode[ch]);
                        midi2151.data[ch].Add(0x00);

                        midi2151.oldCode[ch] = -1;
                        midi2151.data[ch].Add(0x00);//Delta 0
                    }

                    midi2151.data[ch].Add(0xff); //メタイベント
                    midi2151.data[ch].Add(0x2f);
                    midi2151.data[ch].Add(0x00);

                    int MTrkLengthAdr = 0x04;
                    midi2151.data[ch][MTrkLengthAdr + 0] = (byte)(((midi2151.data[ch].Count - (MTrkLengthAdr + 4)) & 0xff000000) >> 24);
                    midi2151.data[ch][MTrkLengthAdr + 1] = (byte)(((midi2151.data[ch].Count - (MTrkLengthAdr + 4)) & 0x00ff0000) >> 16);
                    midi2151.data[ch][MTrkLengthAdr + 2] = (byte)(((midi2151.data[ch].Count - (MTrkLengthAdr + 4)) & 0x0000ff00) >> 8);
                    midi2151.data[ch][MTrkLengthAdr + 3] = (byte)(((midi2151.data[ch].Count - (MTrkLengthAdr + 4)) & 0x000000ff) >> 0);

                    int PortAdr = 0x08;
                    midi2151.data[ch][PortAdr + 4] = (byte)portNum;
                }

                portNum++;
                trkNum += midi2151.maxTrk;
            }

            if (setting.midiExport.UseYM2612Export)
            {
                for (int ch = 0; ch < midi2612.maxTrk; ch++)
                {
                    SetDelta(ch, midi2612, midi2612.oldFrameCounter[ch]);

                    if (midi2612.oldCode[ch] >= 0)
                    {

                        midi2612.data[ch].Add((byte)(0x80 | ch));
                        midi2612.data[ch].Add((byte)midi2612.oldCode[ch]);
                        midi2612.data[ch].Add(0x00);

                        midi2612.oldCode[ch] = -1;
                        midi2612.data[ch].Add(0x00);//Delta 0
                    }

                    midi2612.data[ch].Add(0xff); //メタイベント
                    midi2612.data[ch].Add(0x2f);
                    midi2612.data[ch].Add(0x00);

                    int MTrkLengthAdr = 0x04;
                    midi2612.data[ch][MTrkLengthAdr + 0] = (byte)(((midi2612.data[ch].Count - (MTrkLengthAdr + 4)) & 0xff000000) >> 24);
                    midi2612.data[ch][MTrkLengthAdr + 1] = (byte)(((midi2612.data[ch].Count - (MTrkLengthAdr + 4)) & 0x00ff0000) >> 16);
                    midi2612.data[ch][MTrkLengthAdr + 2] = (byte)(((midi2612.data[ch].Count - (MTrkLengthAdr + 4)) & 0x0000ff00) >> 8);
                    midi2612.data[ch][MTrkLengthAdr + 3] = (byte)(((midi2612.data[ch].Count - (MTrkLengthAdr + 4)) & 0x000000ff) >> 0);

                    int PortAdr = 0x08;
                    midi2612.data[ch][PortAdr + 4] = (byte)portNum;
                }

                portNum++;
                trkNum += midi2612.maxTrk;
            }

            cData[0xb] = (byte)trkNum;//トラック数

            try
            {
                string fn = PlayingFileName == "" ? "Temp.mid" : PlayingFileName;
                List<byte> buf = new List<byte>();

                foreach (byte d in cData) buf.Add(d);

                if (setting.midiExport.UseYM2151Export) foreach (List<byte> dat in midi2151.data) foreach (byte d in dat) buf.Add(d);
                if (setting.midiExport.UseYM2612Export) foreach (List<byte> dat in midi2612.data) foreach (byte d in dat) buf.Add(d);

                File.WriteAllBytes(Path.Combine(setting.midiExport.ExportPath, Path.ChangeExtension(Path.GetFileName(fn), ".mid")), buf.ToArray());
            }
            catch
            {
            }

            cData = null;
            midi2151 = null;
            midi2612 = null;
        }

        private void outMIDIData_YM2151(int chipID, int dPort, int dAddr, int dData,int hosei, long vgmFrameCounter)
        {
            if (cData == null)
            {
                MakeHeader();
            }

            if (dAddr == 0x08)
            {
                byte ch = (byte)(dData & 0x7);
                byte cmd = (byte)((dData & 0x78) != 0 ? 0x90 : 0x80);

                int freq = (fmRegisterYM2151[chipID][0x28 + ch] & 0x7f) + (fmRegisterYM2151[chipID][0x30 + ch] & 0xfc) * 0x100;
                int octNote = fmRegisterYM2151[chipID][0x28 + ch] & 0x7f;
                if (octNote == 0) return;
                int octav = (octNote & 0x70) >> 4;
                int note = searchOPMNote(octNote) + hosei;
                byte code = (byte)(octav * 12 + note);
                byte vel = (byte)(127 - fmRegisterYM2151[chipID][0x78 + ch]);

                if (midi2151.oldFreq[ch] < 0 && cmd == 0x80) return;

                SetDelta(ch, midi2151, vgmFrameCounter);

                if (midi2151.oldCode[ch] >= 0 || cmd == 0x80)
                {
                    midi2151.data[ch].Add((byte)(0x80 | ch));
                    midi2151.data[ch].Add((byte)midi2151.oldCode[ch]);
                    midi2151.data[ch].Add(0x00);

                    midi2151.oldCode[ch] = -1;
                    midi2151.oldFreq[ch] = -1;

                    if (cmd != 0x80) midi2151.data[ch].Add(0);//NextDeltaTime
                }

                if (cmd == 0x90)
                {
                    midi2151.data[ch].Add((byte)(0x90 | ch));
                    midi2151.data[ch].Add(code);
                    if (setting.midiExport.UseVOPMex)
                    {
                        midi2151.data[ch].Add(127);
                    }
                    else
                    {
                        midi2151.data[ch].Add(vel);
                    }

                    midi2151.oldCode[ch] = code;
                    midi2151.oldFreq[ch] = freq;
                }

                return;
            }

            if (!setting.midiExport.KeyOnFnum)
            {
                if (dAddr >= 0x28 && dAddr < 0x30)
                {
                    byte ch = (byte)(dAddr & 0x7);
                    int freq = midi2151.oldFreq[ch];
                    if (freq == -1) return;

                    freq = (freq & 0xfc00) | (dData & 0x007f);

                    if (freq != midi2151.oldFreq[ch])
                    {
                        int freq2nd = freq & 0x007f;
                        //if (freq2nd == 0) return;
                        int octav = (freq & 0x0070) >> 4;
                        int note = searchOPMNote(freq2nd) + hosei;
                        byte code = (byte)(octav * 12 + note);

                        if (midi2151.oldCode[ch] != -1 && midi2151.oldCode[ch] != code)
                        {
                            SetDelta(ch, midi2151, vgmFrameCounter);
                            midi2151.data[ch].Add((byte)(0x80 | ch));
                            midi2151.data[ch].Add((byte)midi2151.oldCode[ch]);
                            midi2151.data[ch].Add(0x00);

                            midi2151.data[ch].Add(0);//delta0
                            midi2151.data[ch].Add((byte)(0x90 | ch));
                            midi2151.data[ch].Add(code);
                            if (setting.midiExport.UseVOPMex)
                            {
                                midi2151.data[ch].Add(127);
                            }
                            else
                            {
                                byte vel = (byte)(127 - fmRegisterYM2151[chipID][0x78 + ch]);
                                midi2151.data[ch].Add(vel);
                            }

                            midi2151.oldFreq[ch] = freq;
                            midi2151.oldCode[ch] = code;
                        }
                    }

                    return;
                }
            }

            ////
            //VOPMex向け
            ////
            if (!setting.midiExport.UseVOPMex) return;

            if (dAddr >= 0x30 && dAddr < 0x38)
            {
                int ch = dAddr & 0x7;
                int bend = (dData & 0xfc) >> 2;
                bend *= 64;
                bend += 8192;

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xe0 | ch)); // pitch bend
                midi2151.data[ch].Add((byte)(bend & 0x7f));
                midi2151.data[ch].Add((byte)((bend & 0x3f80) >> 7));
            }

            //DT/ML
            if (dAddr >= 0x40 && dAddr<0x60)
            {
                int ch = dAddr & 0x7;
                int op = (dAddr & 0x18) >> 3;
                op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // DT
                midi2151.data[ch].Add((byte)(24 + op));
                midi2151.data[ch].Add((byte)((dData & 0x70) >> 4));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //ML
                midi2151.data[ch].Add((byte)(20 + op));
                midi2151.data[ch].Add((byte)((dData & 0x0f) >> 0));

                return;
            }

            //TL
            if (dAddr >= 0x60 && dAddr < 0x80)
            {
                int ch = dAddr & 0x7;
                int op = (dAddr & 0x18) >> 3;
                op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // TL
                midi2151.data[ch].Add((byte)(16 + op));
                midi2151.data[ch].Add((byte)((dData & 0x7f) >> 0));

                return;
            }

            //KS/AR
            if (dAddr >= 0x80 && dAddr < 0xa0)
            {
                int ch = dAddr & 0x7;
                int op = (dAddr & 0x18) >> 3;
                op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // KS
                midi2151.data[ch].Add((byte)(39 + op));
                midi2151.data[ch].Add((byte)((dData & 0xc0) >> 6));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //AR
                midi2151.data[ch].Add((byte)(43 + op));
                midi2151.data[ch].Add((byte)((dData & 0x1f) >> 0));

                return;
            }

            //AMS/DR
            if (dAddr >= 0xa0 && dAddr < 0xc0)
            {
                int ch = dAddr & 0x7;
                int op = (dAddr & 0x18) >> 3;
                op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // AMS
                midi2151.data[ch].Add((byte)(70 + op));
                midi2151.data[ch].Add((byte)((dData & 0x80) >> 7));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //DR
                midi2151.data[ch].Add((byte)(47 + op));
                midi2151.data[ch].Add((byte)((dData & 0x1f) >> 0));

                return;
            }

            //DT2/SR
            if (dAddr >= 0xc0 && dAddr < 0xe0)
            {
                int ch = dAddr & 0x7;
                int op = (dAddr & 0x18) >> 3;
                op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // DT2
                midi2151.data[ch].Add((byte)(28 + op));
                midi2151.data[ch].Add((byte)((dData & 0xc0) >> 6));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //SR
                midi2151.data[ch].Add((byte)(51 + op));
                midi2151.data[ch].Add((byte)((dData & 0x1f) >> 0));

                return;
            }

            //DL/RR
            if (dAddr >= 0xe0 && dAddr < 0x100)
            {
                int ch = dAddr & 0x7;
                int op = (dAddr & 0x18) >> 3;
                op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // DL
                midi2151.data[ch].Add((byte)(55 + op));
                midi2151.data[ch].Add((byte)((dData & 0xf0) >> 4));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //RR
                midi2151.data[ch].Add((byte)(59 + op));
                midi2151.data[ch].Add((byte)((dData & 0x0f) >> 0));

                return;
            }

            //PAN/FB/ALG
            if (dAddr >= 0x20 && dAddr < 0x28)
            {
                int ch = dAddr & 0x7;

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // PAN
                midi2151.data[ch].Add((byte)(10));
                int pan = (dData & 0xc0) >> 6;
                midi2151.data[ch].Add((byte)(pan == 0 ? 64 : ((pan == 1) ? 127 : ((pan == 2) ? 1 : 64))));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); // FB
                midi2151.data[ch].Add((byte)(15));
                midi2151.data[ch].Add((byte)((dData & 0x38) >> 3));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //ALG
                midi2151.data[ch].Add((byte)(14));
                midi2151.data[ch].Add((byte)((dData & 0x07) >> 0));

                return;
            }

            //AMS/FMS
            if (dAddr >= 0x38 && dAddr < 0x40)
            {
                int ch = dAddr & 0x7;

                SetDelta(ch, midi2151, vgmFrameCounter);

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //AMS
                midi2151.data[ch].Add((byte)(76));
                midi2151.data[ch].Add((byte)((dData & 0x03) >> 0));

                midi2151.data[ch].Add(0);//Delta 0

                midi2151.data[ch].Add((byte)(0xb0 | ch)); //FMS
                midi2151.data[ch].Add((byte)(75));
                midi2151.data[ch].Add((byte)((dData & 0x70) >> 4));

                return;
            }

        }

        private void outMIDIData_YM2612(int chipID, int dPort, int dAddr, int dData, long vgmFrameCounter)
        {
            if (cData == null)
            {
                MakeHeader();
            }

            //KeyON時
            if (dPort == 0 && dAddr == 0x28)
            {
                byte ch = (byte)(dData & 0x7);
                ch = (byte)(ch > 2 ? ch - 1 : ch);
                byte cmd = (byte)((dData & 0xf0) != 0 ? 0x90 : 0x80);//オペレータが一つでもonならnoteON(0x90) 全てoffならnoteOFF(0x80)

                //必要なレジスタを読むための情報であるチャンネルとポートを取得
                int p = ch > 2 ? 1 : 0;
                int vch = ch > 2 ? (ch - 3) : ch;
                if (ch > 5) return;

                //キーオンしたチャンネルのFnumを取得
                midi2612.oldFreq[ch] = fmRegisterYM2612[chipID][p][0xa0 + vch] + (fmRegisterYM2612[chipID][p][0xa4 + vch] & 0x3f) * 0x100;
                int freq = midi2612.oldFreq[ch] & 0x7ff;
                if (freq == 0) return;
                int octav = (midi2612.oldFreq[ch] & 0x3800) >> 11;
                int note = searchFMNote(freq);
                byte code = (byte)(octav * 12 + note);

                //オペレータ4のトータルレベルのみ取得(音量として使う)
                byte vel = (byte)(127 - fmRegisterYM2612[chipID][p][0x4c + vch]);

                //前回のコードが負で且つ noteOFFなら何もせずに処理終了
                if (midi2612.oldCode[ch] < 0 && cmd == 0x80) return;

                //デルタのセット(前回のデータ送信から経過した時間をセットする)
                SetDelta(ch, midi2612, vgmFrameCounter);

                //前回のコードが正(発音中である)の時、またはnoteOFFの時　noteOFFのコマンドを発行
                if (midi2612.oldCode[ch] >= 0 || cmd == 0x80)
                {
                    midi2612.data[ch].Add((byte)(0x80 | ch));
                    midi2612.data[ch].Add((byte)midi2612.oldCode[ch]);
                    midi2612.data[ch].Add(0x00);

                    midi2612.oldCode[ch] = -1;
                    if (cmd != 0x80) midi2612.data[ch].Add(0);//NextDeltaTime
                }

                //noteONの場合は、noteONコマンドを発行
                if (cmd == 0x90)
                {
                    midi2612.data[ch].Add((byte)(0x90 | ch));
                    midi2612.data[ch].Add(code);
                    if (setting.midiExport.UseVOPMex)
                    {
                        midi2612.data[ch].Add(127);
                    }
                    else
                    {
                        midi2612.data[ch].Add(vel);
                    }

                    midi2612.oldCode[ch] = code;
                }

                return;
            }

            if (!setting.midiExport.KeyOnFnum)
            {
                //Fnumを設定したとき
                if (dAddr >= 0xa0 && dAddr < 0xa8)
                {
                    //fnumの情報を読み出す
                    byte ch = (byte)((dAddr & 0x3) + dPort * 3);
                    int freq = midi2612.oldFreq[ch];
                    int vch = ch > 2 ? (ch - 3) : ch;
                    if (freq == -1) return;
                    if (dAddr < 0xa4)
                    {
                        freq = (freq & 0x3f00) | dData;
                    }
                    else
                    {
                        freq = (freq & 0xff) | ((dData & 0x3f) << 8);
                    }

                    //もし前回と異なる値を設定していた場合はもっと詳細に調べる
                    if (freq != midi2612.oldFreq[ch])
                    {
                        //今回の音階を調べる
                        int freq2nd = freq & 0x07ff;
                        if (freq2nd == 0) return;
                        int octav = (freq & 0x3800) >> 11;
                        int note = searchFMNote(freq2nd);
                        byte code = (byte)(octav * 12 + note);

                        //現在発音中で、更に前回と音階が異なっているか調べる
                        if (midi2612.oldCode[ch] != -1 && midi2612.oldCode[ch] != code)
                        {
                            //一旦キーオフする
                            SetDelta(ch, midi2612, vgmFrameCounter);
                            midi2612.data[ch].Add((byte)(0x80 | ch));
                            midi2612.data[ch].Add((byte)midi2612.oldCode[ch]);
                            midi2612.data[ch].Add(0x00);

                            //今回の音階でキーオンしなおす
                            midi2612.data[ch].Add(0);//delta0
                            midi2612.data[ch].Add((byte)(0x90 | ch));
                            midi2612.data[ch].Add(code);
                            if (setting.midiExport.UseVOPMex)
                            {
                                midi2612.data[ch].Add(127);
                            }
                            else
                            {
                                byte vel = (byte)(127 - fmRegisterYM2612[chipID][dPort][0x4c + vch]);
                                midi2612.data[ch].Add(vel);
                            }

                            midi2612.oldFreq[ch] = freq;
                            midi2612.oldCode[ch] = code;
                        }
                    }

                    return;
                }
            }

            ////
            //VOPMex向け
            ////
            if (!setting.midiExport.UseVOPMex) return;

            //DT/ML
            if ((dAddr & 0xf0) == 0x30)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;
                    int op = (dAddr & 0xc) >> 2;
                    op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // DT
                    midi2612.data[ch].Add((byte)(24 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x70) >> 4));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //ML
                    midi2612.data[ch].Add((byte)(20 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x0f) >> 0));
                }
                return;
            }

            //TL
            if ((dAddr & 0xf0) == 0x40)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;
                    int op = (dAddr & 0xc) >> 2;
                    op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // TL
                    midi2612.data[ch].Add((byte)(16 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x7f) >> 0));
                }
                return;
            }

            //KS/AR
            if ((dAddr & 0xf0) == 0x50)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;
                    int op = (dAddr & 0xc) >> 2;
                    op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // KS
                    midi2612.data[ch].Add((byte)(39 + op));
                    midi2612.data[ch].Add((byte)((dData & 0xc0) >> 6));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //AR
                    midi2612.data[ch].Add((byte)(43 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x1f) >> 0));
                }
                return;
            }

            //AMS/DR
            if ((dAddr & 0xf0) == 0x60)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;
                    int op = (dAddr & 0xc) >> 2;
                    op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // AMS
                    midi2612.data[ch].Add((byte)(70 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x80) >> 7));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //DR
                    midi2612.data[ch].Add((byte)(47 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x1f) >> 0));
                }
                return;
            }

            //SR
            if ((dAddr & 0xf0) == 0x70)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;
                    int op = (dAddr & 0xc) >> 2;
                    op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //SR
                    midi2612.data[ch].Add((byte)(51 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x1f) >> 0));
                }
                return;
            }

            //DL/RR
            if ((dAddr & 0xf0) == 0x80)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;
                    int op = (dAddr & 0xc) >> 2;
                    op = (op == 1) ? 2 : ((op == 2) ? 1 : op);

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // DL
                    midi2612.data[ch].Add((byte)(55 + op));
                    midi2612.data[ch].Add((byte)((dData & 0xf0) >> 4));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //RR
                    midi2612.data[ch].Add((byte)(59 + op));
                    midi2612.data[ch].Add((byte)((dData & 0x0f) >> 0));
                }
                return;
            }

            //FB/ALG
            if (dAddr >= 0xB0 && dAddr < 0xB4)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // FB
                    midi2612.data[ch].Add((byte)(15));
                    midi2612.data[ch].Add((byte)((dData & 0x38) >> 3));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //ALG
                    midi2612.data[ch].Add((byte)(14));
                    midi2612.data[ch].Add((byte)((dData & 0x07) >> 0));
                }
                return;
            }

            //PAN/AMS/FMS
            if (dAddr >= 0xB4 && dAddr < 0xB7)
            {
                int ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    ch += dPort * 3;

                    SetDelta(ch, midi2612, vgmFrameCounter);

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); // PAN
                    midi2612.data[ch].Add((byte)(10));
                    int pan = (dData & 0xc0) >> 6;
                    midi2612.data[ch].Add((byte)(pan == 0 ? 64 : ((pan == 1) ? 127 : ((pan == 2) ? 1 : 64))));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //AMS
                    midi2612.data[ch].Add((byte)(76));
                    midi2612.data[ch].Add((byte)((dData & 0x38) >> 3));

                    midi2612.data[ch].Add(0);//Delta 0

                    midi2612.data[ch].Add((byte)(0xb0 | ch)); //FMS
                    midi2612.data[ch].Add((byte)(75));
                    midi2612.data[ch].Add((byte)((dData & 0x03) >> 0));
                }
                return;
            }

        }

        private int searchFMNote(int freq)
        {
            int m = int.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 5; i++)
            {
                int a = Math.Abs(freq - Tables.FmFNum[i]);
                if (m > a)
                {
                    m = a;
                    n = i;
                }
            }
            return n - 12 * 3;
        }

        private int searchOPMNote(int freq)
        {
            int note = freq & 0xf;
            note = (note < 3) ? note : (note < 7 ? note - 1 : (note < 11 ? note - 2 : note - 3));

            return note;
        }

        private void SetDelta(int ch,midiChip chip, long NewFrameCounter)
        {
            if (ch >= chip.oldFrameCounter.Length) return;

            long sub = NewFrameCounter - chip.oldFrameCounter[ch];
            long step = (long)(sub / (double)Common.SampleRate * 960.0);
            chip.oldFrameCounter[ch] += (long)(step * (double)Common.SampleRate / 960.0);

            bool flg = true;
            for (int i = 0; i < 4; i++)
            {
                byte d = (byte)((step & (0x0fe00000 >> (7 * i))) >> (21 - 7 * i));
                if (flg && d == 0 && i < 3) continue;
                flg = false;
                d |= (byte)((i != 3) ? 0x80 : 0x00);
                chip.data[ch].Add(d);
            }

        }

        private void MakeHeader()
        {
            cData = new List<byte>();

            cData.Add(0x4d); //チャンクタイプ'MThd'
            cData.Add(0x54);
            cData.Add(0x68);
            cData.Add(0x64);

            cData.Add(0x00); //データ長
            cData.Add(0x00);
            cData.Add(0x00);
            cData.Add(0x06);

            cData.Add(0x00); //フォーマット
            cData.Add(0x01);

            cData.Add(0x00); //トラック数
            cData.Add(0x01);

            cData.Add(0x01); //分解能
            cData.Add(0xe0);

            cData.Add(0x4d); //チャンクタイプ'MTrk'
            cData.Add(0x54);
            cData.Add(0x72);
            cData.Add(0x6b);

            cData.Add(0x00); //データ長 0x17
            cData.Add(0x00);
            cData.Add(0x00);
            cData.Add(0x17);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント
            cData.Add(0x03);
            cData.Add(0x00);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント　拍子 4/4(固定)
            cData.Add(0x58);
            cData.Add(0x04);
            cData.Add(0x04);
            cData.Add(0x02);
            cData.Add(0x18);
            cData.Add(0x08);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント　テンポ設定 BPM = 120(固定)
            cData.Add(0x51);
            cData.Add(0x03);
            cData.Add(0x07);
            cData.Add(0xa1);
            cData.Add(0x20);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント　終端
            cData.Add(0x2f);
            cData.Add(0x00);

            // 実 Track
            if (setting.midiExport.UseYM2151Export) InitYM2151();
            if (setting.midiExport.UseYM2612Export) InitYM2612();

        }

        private void InitYM2151()
        {
            midi2151 = new midiChip();
            midi2151.maxTrk = 8;
            midi2151.oldCode = new int[midi2151.maxTrk];
            midi2151.oldFreq = new int[midi2151.maxTrk];
            midi2151.data = new List<byte>[midi2151.maxTrk];
            midi2151.oldFrameCounter = new long[midi2151.maxTrk];

            for (int i = 0; i < midi2151.maxTrk; i++)
            {
                midi2151.oldCode[i] = -1;
                midi2151.oldFreq[i] = -1;
                midi2151.data[i] = new List<byte>();
                midi2151.oldFrameCounter[i] = 0L;
            }

            for (int i = 0; i < midi2151.maxTrk; i++)
            {
                midi2151.data[i].Add(0x4d); //チャンクタイプ'MTrk'
                midi2151.data[i].Add(0x54);
                midi2151.data[i].Add(0x72);
                midi2151.data[i].Add(0x6b);

                midi2151.data[i].Add(0x00); //データ長 この時点では不明のためとりあえず0
                midi2151.data[i].Add(0x00);
                midi2151.data[i].Add(0x00);
                midi2151.data[i].Add(0x00);

                midi2151.data[i].Add(0x00); //delta0
                midi2151.data[i].Add(0xff); // メタイベントポート指定
                midi2151.data[i].Add(0x21);
                midi2151.data[i].Add(0x01);
                midi2151.data[i].Add(0x00); //Port1

                midi2151.data[i].Add(0x00); //delta0
                midi2151.data[i].Add(0xff); // メタイベント　トラック名
                midi2151.data[i].Add(0x03);
                midi2151.data[i].Add(0x00);

            }


            if (!setting.midiExport.UseVOPMex) return;

            //VOPMex向け

            for (int i = 0; i < midi2151.maxTrk; i++)
            {
                //音色コントロールの動作を変更(全MIDIチャンネル)
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 121 127
                midi2151.data[i].Add(121);
                midi2151.data[i].Add(127);
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 126 127
                midi2151.data[i].Add(126);
                midi2151.data[i].Add(127);
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 123 127
                midi2151.data[i].Add(123);
                midi2151.data[i].Add(127);
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 98 127
                midi2151.data[i].Add(98);
                midi2151.data[i].Add(127);
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 99 126
                midi2151.data[i].Add(99);
                midi2151.data[i].Add(126);
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 6 127
                midi2151.data[i].Add(6);
                midi2151.data[i].Add(127);
                midi2151.data[i].Add(0x50); //Delta 0
                midi2151.data[i].Add((byte)(0xb0 + i)); // CC 93 120
                midi2151.data[i].Add(93);
                midi2151.data[i].Add(120);
            }
        }

        private void InitYM2612()
        {
            midi2612 = new midiChip();
            midi2612.maxTrk = 6;
            midi2612.oldCode = new int[midi2612.maxTrk];
            midi2612.oldFreq = new int[midi2612.maxTrk];
            midi2612.data = new List<byte>[midi2612.maxTrk];
            midi2612.oldFrameCounter = new long[midi2612.maxTrk];

            for (int i = 0; i < midi2612.maxTrk; i++)
            {
                midi2612.oldCode[i] = -1;
                midi2612.oldFreq[i] = -1;
                midi2612.data[i] = new List<byte>();
                midi2612.oldFrameCounter[i] = 0L;
            }

            for (int i = 0; i < midi2612.maxTrk; i++)
            {
                midi2612.data[i].Add(0x4d); //チャンクタイプ'MTrk'
                midi2612.data[i].Add(0x54);
                midi2612.data[i].Add(0x72);
                midi2612.data[i].Add(0x6b);

                midi2612.data[i].Add(0x00); //データ長 この時点では不明のためとりあえず0
                midi2612.data[i].Add(0x00);
                midi2612.data[i].Add(0x00);
                midi2612.data[i].Add(0x00);

                midi2612.data[i].Add(0x00); //delta0
                midi2612.data[i].Add(0xff); // メタイベントポート指定
                midi2612.data[i].Add(0x21);
                midi2612.data[i].Add(0x01);
                midi2612.data[i].Add(0x00); //Port1

                midi2612.data[i].Add(0x00); //delta0
                midi2612.data[i].Add(0xff); // メタイベント　トラック名
                midi2612.data[i].Add(0x03);
                midi2612.data[i].Add(0x00);

            }


            if (!setting.midiExport.UseVOPMex) return;

            //VOPMex向け

            for (int i = 0; i < midi2612.maxTrk; i++)
            {
                //音色コントロールの動作を変更(全MIDIチャンネル)
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 121 127
                midi2612.data[i].Add(121);
                midi2612.data[i].Add(127);
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 126 127
                midi2612.data[i].Add(126);
                midi2612.data[i].Add(127);
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 123 127
                midi2612.data[i].Add(123);
                midi2612.data[i].Add(127);
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 98 127
                midi2612.data[i].Add(98);
                midi2612.data[i].Add(127);
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 99 126
                midi2612.data[i].Add(99);
                midi2612.data[i].Add(126);
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 6 127
                midi2612.data[i].Add(6);
                midi2612.data[i].Add(127);
                midi2612.data[i].Add(0x50); //Delta 0
                midi2612.data[i].Add((byte)(0xb0 + i)); // CC 93 120
                midi2612.data[i].Add(93);
                midi2612.data[i].Add(120);
            }
        }

    }

    public class midiChip
    {
        public List<byte>[] data = null;
        public long[] oldFrameCounter = null;
        public int[] oldCode = null;
        public int[] oldFreq = null;
        public int maxTrk = 0;
    }
}
