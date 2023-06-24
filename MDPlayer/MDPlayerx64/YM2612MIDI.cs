using MDPlayer.form;
using NAudio.Midi;
using System.Text;

namespace MDPlayer
{
    public class YM2612MIDI
    {
        private int latestNoteNumberMONO = -1;
        private readonly int[] latestNoteNumber = new int[6] { -1, -1, -1, -1, -1, -1 };

        private readonly form.frmMain parent;
        private readonly MDSound.MDSound mdsMIDI;
        public MDChipParams newParam = null;


        private int[][] _noteLog = new int[6][] { new int[100], new int[100], new int[100], new int[100], new int[100], new int[100] };
        public int[][] NoteLog
        {
            get
            {
                return _noteLog;
            }

            set
            {
                _noteLog = value;
            }
        }

        private int[] _noteLogPtr = new int[6];
        public int[] NoteLogPtr
        {
            get
            {
                return _noteLogPtr;
            }

            set
            {
                _noteLogPtr = value;
            }
        }



        public YM2612MIDI(frmMain parent, MDSound.MDSound mdsMIDI, MDChipParams newParam)
        {
            this.parent = parent;
            this.mdsMIDI = mdsMIDI;
            this.newParam = newParam;

            for (int ch = 0; ch < 6; ch++)
            {
                for (int n = 0; n < 100; n++) NoteLog[ch][n] = -1;
                NoteLogPtr[ch] = 0;
                if (parent.setting.midiKbd.Tones != null && parent.setting.midiKbd.Tones[ch] != null)
                    VoiceCopyChFromTone(ch, parent.setting.midiKbd.Tones[ch]);
            }
        }

        public void Close()
        {
            SetTonesToSettng();
        }

        private int NoteONMONO(int noteNumber)
        {
            int fnum = Tables.FmFNum[(noteNumber % 12) + 36];
            int oct = noteNumber / 12 - 1;
            oct = Math.Min(Math.Max(oct, 0), 7);

            int ch = parent.setting.midiKbd.UseMONOChannel;
            if (ch < 0 || ch > 5) return -1;

            mdsMIDI.WriteYM2612(0, (byte)(ch / 3), (byte)(0xa4 + (ch % 3)), (byte)(((fnum & 0x700) >> 8) | (oct << 3)));
            mdsMIDI.WriteYM2612(0, (byte)(ch / 3), (byte)(0xa0 + (ch % 3)), (byte)(fnum & 0xff));

            mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0x00 + ch + (ch / 3)));
            mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0xf0 + ch + (ch / 3)));
            latestNoteNumberMONO = noteNumber;

            return ch;
        }

        private void NoteOFFMONO(int noteNumber)
        {
            int ch = parent.setting.midiKbd.UseMONOChannel;
            if (ch < 0 || ch > 5) return;

            if (noteNumber == latestNoteNumberMONO)
                mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0x00 + ch + (ch / 3)));
        }

        private int NoteON(int noteNumber)
        {
            if (parent.setting.midiKbd.IsMONO)
            {
                return NoteONMONO(noteNumber);
            }

            int fnum = Tables.FmFNum[(noteNumber % 12) + 36];
            int oct = noteNumber / 12 - 1;
            oct = Math.Min(Math.Max(oct, 0), 7);

            bool sw = false;
            int ch = 0;
            for (; ch < 6; ch++)
            {
                if (latestNoteNumber[ch] != -1) continue;
                if (!parent.setting.midiKbd.UseChannel[ch]) continue;
                sw = true;
                break;
            }
            if (!sw) return -1;

            mdsMIDI.WriteYM2612(0, (byte)(ch / 3), (byte)(0xa4 + (ch % 3)), (byte)(((fnum & 0x700) >> 8) | (oct << 3)));
            mdsMIDI.WriteYM2612(0, (byte)(ch / 3), (byte)(0xa0 + (ch % 3)), (byte)(fnum & 0xff));

            mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0x00 + ch + (ch / 3)));
            mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0xf0 + ch + (ch / 3)));
            latestNoteNumber[ch] = noteNumber;

            return ch;
        }

        private void NoteOFF(int noteNumber)
        {
            if (parent.setting.midiKbd.IsMONO)
            {
                NoteOFFMONO(noteNumber);
                return;
            }

            bool sw = false;
            int ch = 0;
            for (; ch < 6; ch++)
            {
                if (latestNoteNumber[ch] != noteNumber) continue;
                sw = true;
                break;
            }
            if (!sw) return;

            latestNoteNumber[ch] = -1;
            mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0x00 + ch + (ch / 3)));
        }

        private void VoiceCopy()
        {
            int[][] reg = Audio.GetFMRegister(0);// chipRegister.fmRegisterYM2612[0];
            if (reg == null) return;

            for (int i = 0; i < 6; i++)
            {
                VoiceCopyCh(0, i, reg);
            }
        }

        private void VoiceCopyCh(int src, int des, int[][] reg)
        {

            for (int i = 0x30; i < 0xa0; i += 0x10)
            {
                for (int j = 0; j < 4; j++)
                {
                    mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(i + j * 4 + (des % 3)), (byte)reg[src / 3][i + j * 4 + (src % 3)]);
                }
            }

            mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0xb0 + (des % 3)), (byte)reg[src / 3][0xb0 + (src % 3)]);
            mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)((0xb4 + (des % 3)) | 0xc0), (byte)reg[src / 3][0xb4 + (src % 3)]);


            int alg = (byte)reg[src / 3][0xb0 + (src % 3)] & 0x7;
            byte[] algTl = new byte[8] { 0x08, 0x08, 0x08, 0x08, 0x0c, 0x0e, 0x0e, 0x0f };
            int[] tls = new int[4];
            int max = 127;

            for (int j = 0; j < 4; j++)
            {
                tls[j] = (byte)reg[src / 3][0x40 + j * 4 + (src % 3)];
                if ((algTl[alg] & (1 << j)) != 0)
                    max = Math.Min(max, tls[j]);
            }

            for (int j = 0; j < 4; j++)
            {
                if ((algTl[alg] & (1 << j)) != 0)
                    mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x40 + j * 4 + (des % 3)), (byte)(tls[j] - max));
            }

        }

        private void VoiceCopyChFromOPM(int src, int des, int[] reg)
        {
            for (int i = 0; i < 4; i++)
            {
                int opn = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));
                int opm = (i == 0) ? 0 : ((i == 1) ? 16 : ((i == 2) ? 8 : 24));

                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x50 + opn + (des % 3)), (byte)(reg[0x80 + opm + src] & 0xdf));//AR + KS
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x60 + opn + (des % 3)), (byte)(reg[0xa0 + opm + src] & 0x9f));//DR + AM
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x70 + opn + (des % 3)), (byte)(reg[0xc0 + opm + src] & 0x1f));//SR
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x80 + opn + (des % 3)), (byte)(reg[0xe0 + opm + src] & 0xff));//RR + SL
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x40 + opn + (des % 3)), (byte)(reg[0x60 + opm + src] & 0x7f));//TL
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x30 + opn + (des % 3)), (byte)(reg[0x40 + opm + src] & 0x7f));//ML + DT
            }

            mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0xb0 + (des % 3)), (byte)(reg[0x20 + src] & 0x3f));//AL + FB

            int alg = (byte)reg[0x20 + src] & 0x7;
            byte[] algTl = new byte[8] { 0x08, 0x08, 0x08, 0x08, 0x0c, 0x0e, 0x0e, 0x0f };
            int[] tls = new int[4];
            int max = 127;

            for (int j = 0; j < 4; j++)
            {
                tls[j] = (byte)(reg[0x60 + j * 8 + src] & 0x7f);
                if ((algTl[alg] & (1 << j)) != 0)
                    max = Math.Min(max, tls[j]);
            }

            for (int j = 0; j < 4; j++)
            {
                if ((algTl[alg] & (1 << j)) != 0)
                    mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x40 + j * 4 + (des % 3)), (byte)(tls[j] - max));
            }
        }

        private void VoiceCopyChFromTone(int des, Tone tone)
        {
            if (tone == null) return;
            if (des < 0 || des > 5) return;

            for (int i = 0; i < 4; i++)
            {
                int opn = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));

                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x50 + opn + (des % 3)), (byte)((tone.OPs[i].AR & 0x1f) + ((tone.OPs[i].KS & 0x3) << 6)));//AR + KS
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x60 + opn + (des % 3)), (byte)((tone.OPs[i].DR & 0x1f) + ((tone.OPs[i].AM & 0x1) << 7)));//DR + AM
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x70 + opn + (des % 3)), (byte)((tone.OPs[i].SR & 0x1f)));//SR
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x80 + opn + (des % 3)), (byte)((tone.OPs[i].RR & 0xf) + ((tone.OPs[i].SL & 0xf) << 4)));//RR + SL
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x40 + opn + (des % 3)), (byte)((tone.OPs[i].TL & 0x7f)));//TL
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x30 + opn + (des % 3)), (byte)((tone.OPs[i].ML & 0xf) + ((tone.OPs[i].DT & 0x7) << 4)));//ML + DT
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x90 + opn + (des % 3)), (byte)((tone.OPs[i].SG & 0xf)));//SG
            }

            mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0xb0 + (des % 3)), (byte)((tone.AL & 0x7) + ((tone.FB & 0x7) << 3)));//AL + FB
            mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0xb4 + (des % 3)), (byte)(0xc0 + (tone.PMS & 0x7) + ((tone.AMS & 0x3) << 4)));//PMS + AMS

        }

        private Tone VoiceCopyChToTone(int des, string name)
        {
            Tone tone = new();
            int[][] reg = mdsMIDI.ReadYM2612Register(0);

            for (int i = 0; i < 4; i++)
            {
                int opn = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));

                tone.OPs[i].AR = reg[des / 3][0x50 + opn + (des % 3)] & 0x1f;//AR
                tone.OPs[i].KS = (reg[des / 3][0x50 + opn + (des % 3)] & 0xc0) >> 6;//KS
                tone.OPs[i].DR = reg[des / 3][0x60 + opn + (des % 3)] & 0x1f;//DR
                tone.OPs[i].AM = (reg[des / 3][0x60 + opn + (des % 3)] & 0x80) >> 7;//AM
                tone.OPs[i].SR = reg[des / 3][0x70 + opn + (des % 3)] & 0x1f;//SR
                tone.OPs[i].RR = reg[des / 3][0x80 + opn + (des % 3)] & 0xf;//RR
                tone.OPs[i].SL = (reg[des / 3][0x80 + opn + (des % 3)] & 0xf0) >> 4;//SL
                tone.OPs[i].TL = reg[des / 3][0x40 + opn + (des % 3)] & 0x7f;//TL
                tone.OPs[i].ML = reg[des / 3][0x30 + opn + (des % 3)] & 0xf;//ML
                tone.OPs[i].DT = (reg[des / 3][0x30 + opn + (des % 3)] & 0x70) >> 4;//DT
                tone.OPs[i].DT2 = 0;
            }

            tone.AL = reg[des / 3][0xb0 + (des % 3)] & 0x7;//AL
            tone.FB = (reg[des / 3][0xb0 + (des % 3)] & 0x38) >> 3;//FB
            tone.AMS = 0;
            tone.PMS = 0;
            tone.name = name;

            return tone;
        }

        //private static Tone VoiceCopyToneToTone(Tone src, string name)
        //{
        //    Tone des = new();

        //    for (int i = 0; i < 4; i++)
        //    {
        //        des.OPs[i].AR = src.OPs[i].AR;//AR
        //        des.OPs[i].KS = src.OPs[i].KS;//KS
        //        des.OPs[i].DR = src.OPs[i].DR;//DR
        //        des.OPs[i].AM = src.OPs[i].AM;//AM
        //        des.OPs[i].SR = src.OPs[i].SR;//SR
        //        des.OPs[i].RR = src.OPs[i].RR;//RR
        //        des.OPs[i].SL = src.OPs[i].SL;//SL
        //        des.OPs[i].TL = src.OPs[i].TL;//TL
        //        des.OPs[i].ML = src.OPs[i].ML;//ML
        //        des.OPs[i].DT = src.OPs[i].DT;//DT
        //        des.OPs[i].DT2 = 0;
        //    }

        //    des.AL = src.AL;//AL
        //    des.FB = src.FB;//FB
        //    des.AMS = 0;
        //    des.PMS = 0;
        //    des.name = name;

        //    return des;
        //}

        public void SetVoiceFromChipRegister(EnmChip chip, int chipID, int ch)
        {
            if (chip == EnmChip.YM2612 || chip == EnmChip.YM2608 || chip == EnmChip.YM2610 || chip == EnmChip.YM2203)
            {
                int[][] srcRegs = null;
                if (chip == EnmChip.YM2612)
                {
                    srcRegs = Audio.GetFMRegister(chipID);
                }
                else if (chip == EnmChip.YM2608)
                {
                    srcRegs = Audio.GetYM2608Register(chipID);
                }
                else if (chip == EnmChip.YM2610)
                {
                    srcRegs = Audio.GetYM2610Register(chipID);
                }
                else if (chip == EnmChip.YM2203)
                {
                    int[] sReg = Audio.GetYM2203Register(chipID);
                    srcRegs = new int[2][] { sReg, null };
                }
                for (int i = 0; i < 6; i++)
                {
                    if (parent.setting.midiKbd.UseChannel[i])
                    {
                        VoiceCopyCh(ch, i, srcRegs);
                    }
                }
            }
            else if (chip == EnmChip.YM2151)
            {
                int[] reg = Audio.GetYM2151Register(chipID);
                for (int i = 0; i < 6; i++)
                {
                    if (parent.setting.midiKbd.UseChannel[i])
                    {
                        VoiceCopyChFromOPM(ch, i, reg);
                    }
                }
            }
        }

        public void AllNoteOff()
        {
            for (int ch = 0; ch < 6; ch++)
            {
                mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0x00 + ch + (ch / 3)));
            }
        }

        public void SetMode(int m)
        {
            switch (m)
            {
                case 0:
                    //MONO
                    for (int ch = 0; ch < 6; ch++)
                    {
                        parent.setting.midiKbd.UseChannel[ch] = false;
                        if (ch == parent.setting.midiKbd.UseMONOChannel)
                        {
                            parent.setting.midiKbd.UseChannel[ch] = true;
                        }
                    }
                    parent.setting.midiKbd.IsMONO = true;
                    break;
                default:
                    //POLY
                    parent.setting.midiKbd.IsMONO = false;
                    break;
            }
        }

        public void SelectChannel(int ch)
        {
            if (parent.setting.midiKbd.IsMONO)
            {
                parent.setting.midiKbd.UseMONOChannel = ch;
                SetMode(0);
            }
            else
            {
                parent.setting.midiKbd.UseChannel[ch] = !parent.setting.midiKbd.UseChannel[ch];
            }
        }

        public void Log2MML(int ch)
        {
            string[] tblNote = new[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };
            int ptr = NoteLogPtr[ch];

            //解析開始位置を調べる
            do
            {
                ptr--;
                if (ptr < 0) ptr = NoteLog[ch].Length - 1;

                if (ptr == NoteLogPtr[ch])
                {
                    ptr = NoteLogPtr[ch] - 1;
                    if (ptr < 0) ptr = NoteLog[ch].Length - 1;
                    break;
                }
            } while (NoteLog[ch][ptr] != -1);
            ptr++;
            if (ptr == NoteLog[ch].Length) ptr = 0;

            //ログが無い場合は処理終了
            if (NoteLog[ch][ptr] == -1) return;

            //解析開始
            StringBuilder mml = new("o");

            //オクターブコマンド
            int oct = NoteLog[ch][ptr] / 12;
            mml.Append(oct + 1);

            do
            {
                int o = NoteLog[ch][ptr] / 12;
                int n = NoteLog[ch][ptr] % 12;

                //相対オクターブコマンドの解析
                int s = Math.Sign(oct - o);
                if (s < 0)
                {
                    do
                    {
                        mml.Append('>');
                        oct++;
                    } while (oct != o);
                }
                else if (s > 0)
                {
                    do
                    {
                        mml.Append('<');
                        oct--;
                    } while (oct != o);
                }

                //ノートコマンド
                mml.Append(tblNote[n]);

                ptr++;
            } while (ptr != NoteLogPtr[ch]);

            //クリップボードにMMLをセット
            Clipboard.SetText(mml.ToString());

        }

        public void Log2MML66(int ch)
        {
            string[] tblNote = new[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };
            int ptr = NoteLogPtr[ch];

            //解析開始位置を調べる
            do
            {
                ptr--;
                if (ptr < 0) ptr = NoteLog[ch].Length - 1;

                if (ptr == NoteLogPtr[ch])
                {
                    ptr = NoteLogPtr[ch] - 1;
                    if (ptr < 0) ptr = NoteLog[ch].Length - 1;
                    break;
                }
            } while (NoteLog[ch][ptr] != -1);
            ptr++;
            if (ptr == NoteLog[ch].Length) ptr = 0;

            if (ptr == NoteLogPtr[ch]) return;

            //解析開始
            StringBuilder mml = new("");

            //オクターブのみ取得
            int oct = NoteLog[ch][ptr] / 12;

            do
            {
                int o = NoteLog[ch][ptr] / 12;
                int n = NoteLog[ch][ptr] % 12;

                //相対オクターブコマンドの解析
                int s = Math.Sign(oct - o);
                if (s < 0)
                {
                    do
                    {
                        mml.Append('>');
                        oct++;
                    } while (oct != o);
                }
                else if (s > 0)
                {
                    do
                    {
                        mml.Append('<');
                        oct--;
                    } while (oct != o);
                }

                //ノートコマンド
                mml.Append(tblNote[n]);

                ptr++;
                if (ptr == NoteLog[ch].Length) ptr = 0;
            } while (ptr != NoteLogPtr[ch]);

            //クリップボードにMMLをセット
            Clipboard.SetText(mml.ToString());
            SendKeys.SendWait("^v");

            ClearNoteLog(ch);
        }

        public void ClearNoteLog()
        {
            for (int ch = 0; ch < 6; ch++)
            {
                ClearNoteLog(ch);
            }
        }

        public void ClearNoteLog(int ch)
        {
            for (int i = 0; i < 10; i++)
            {
                newParam.ym2612Midi.noteLog[ch][i] = -1;
            }

            for (int i = 0; i < 100; i++)
            {
                NoteLog[ch][i] = -1;
            }
            NoteLogPtr[ch] = 0;
        }

        public void MidiInMessageReceived(MidiInMessageEventArgs e)
        {
            try
            {

                if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent noe = (NoteEvent)e.MidiEvent;

                    if (noe.Velocity == 0)
                    {
                        NoteOFF(noe.NoteNumber);
                        return;
                    }

                    int ch = NoteON(noe.NoteNumber);

                    if (ch != -1)
                    {
                        int n = (noe.NoteNumber % 12) + (noe.NoteNumber / 12 - 1) * 12;
                        n = Math.Max(Math.Min(n, 12 * 8 - 1), 0);
                        NoteLog[ch][NoteLogPtr[ch]] = n;
                        int p = NoteLogPtr[ch] - 9;
                        if (p < 0) p += 100;
                        for (int i = 0; i < 10; i++)
                        {
                            newParam.ym2612Midi.noteLog[ch][i] = NoteLog[ch][p];
                            p++;
                            if (p == 100) p = 0;
                        }
                        NoteLogPtr[ch]++;
                        if (NoteLogPtr[ch] == 100) NoteLogPtr[ch] = 0;
                    }
                    return;
                }
                else if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOff)
                {
                    NoteEvent ne = (NoteEvent)e.MidiEvent;
                    NoteOFF(ne.NoteNumber);
                    return;
                }
                else if (e.MidiEvent.CommandCode == MidiCommandCode.ControlChange)
                {
                    int cc = (int)(((ControlChangeEvent)e.MidiEvent).Controller);
                    Setting.MidiKbd mk = parent.setting.midiKbd;
                    if (cc == mk.MidiCtrl_CopyToneFromYM2612Ch1) VoiceCopy();
                    if (cc == mk.MidiCtrl_CopySelecttingLogToClipbrd)
                    {
                        if (parent.setting.midiKbd.IsMONO) Log2MML66(parent.setting.midiKbd.UseMONOChannel);
                    }
                    if (cc == mk.MidiCtrl_DelOneLog)
                    {
                        if (parent.setting.midiKbd.IsMONO)
                        {
                            int ch = parent.setting.midiKbd.UseMONOChannel;
                            int ptr = NoteLogPtr[ch];
                            ptr--;
                            if (ptr < 0) ptr += 100;
                            NoteLog[ch][ptr] = -1;
                            NoteLogPtr[ch] = ptr;
                            int p = NoteLogPtr[ch] - 10;
                            if (p < 0) p += 100;
                            for (int i = 0; i < 10; i++)
                            {
                                newParam.ym2612Midi.noteLog[ch][i] = NoteLog[ch][p];
                                p++;
                                if (p == 100) p = 0;
                            }
                        }
                    }
                    if (cc == mk.MidiCtrl_Fadeout)
                    {
                        parent.Fadeout();
                    }
                    if (cc == mk.MidiCtrl_Fast)
                    {
                        parent.Ff();
                    }
                    if (cc == mk.MidiCtrl_Next)
                    {
                        parent.Next();
                    }
                    if (cc == mk.MidiCtrl_Pause)
                    {
                        parent.Pause();
                    }
                    if (cc == mk.MidiCtrl_Play)
                    {
                        parent.Play();
                    }
                    if (cc == mk.MidiCtrl_Previous)
                    {
                        parent.Prev();
                    }
                    if (cc == mk.MidiCtrl_Slow)
                    {
                        parent.Slow();
                    }
                    if (cc == mk.MidiCtrl_Stop)
                    {
                        parent.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("Exception:\r\n{0}\r\nStackTrace:\r\n{1}\r\n", ex.Message, ex.StackTrace)
                    , "ERROR"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error
                    );
            }
        }

        public void SetTonesToSettng()
        {
            parent.setting.midiKbd.Tones = new Tone[6];
            for (int ch = 0; ch < 6; ch++)
            {
                parent.setting.midiKbd.Tones[ch] = VoiceCopyChToTone(ch, "");
            }
        }

        public void SetTonesFromSettng()
        {
            for (int ch = 0; ch < 6; ch++)
            {
                VoiceCopyChFromTone(ch, parent.setting.midiKbd.Tones[ch]);
            }
        }

        public static void SaveTonePallet(string fn, int tp, TonePallet tonePallet)
        {
            if (tp == 1)
            {
                tonePallet.Save(fn);
                return;
            }

            Encoding enc = Encoding.GetEncoding("shift_jis");
            if (tp == 2) enc = Encoding.Default;

            using StreamWriter sw = new(fn, false, enc);
            int n = 0;
            int row = 10;
            foreach (Tone t in tonePallet.LstTone)
            {
                string[] toneText = null;
                switch (tp)
                {
                    case 2:
                        toneText = MakeToneTextForMml2vgm(t, n++);
                        break;
                    case 3:
                        toneText = MakeToneTextForFMP7(t, n++);
                        break;
                    case 4:
                        toneText = MakeToneTextForNRTDRV(t, n++);
                        break;
                    case 5:
                        toneText = MakeToneTextForMXDRV(t, n++);
                        break;
                    case 6:
                        toneText = MakeToneTextForMUSICLALF(t, n++);
                        break;
                }

                if (tp != 6)
                {
                    foreach (string text in toneText)
                    {
                        sw.WriteLine(text);
                    }
                }
                else
                {
                    foreach (string text in toneText)
                    {
                        sw.WriteLine(text.Replace("[ROW]", row.ToString()));
                        row += 10;
                    }
                }
            }

        }

        private static string[] MakeToneTextForMml2vgm(Tone t, int n)
        {
            List<string> tt = new()
            {
                string.Format("{0}", t.name),
                string.Format("'@ N {0:D3}", n),
                "   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSG - EG"
            };
            foreach (Tone.Op op in t.OPs)
            {
                tt.Add(string.Format("'@ {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3} {8:D3} {9:D3} {10:D3}"
                    , op.AR, op.DR, op.SR, op.RR, op.SL, op.TL, op.KS, op.ML, op.DT, op.AM, op.SG
                    ));
            }
            tt.Add("   AL  FB");
            tt.Add(string.Format("'@ {0:D3} {1:D3}", t.AL, t.FB));
            tt.Add("");

            return tt.ToArray();
        }

        private static string[] MakeToneTextForFMP7(Tone t, int n)
        {
            List<string> tt = new()
            {
                string.Format("{0}", t.name),
                string.Format("'@ FA {0:D3}", n),
                "   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM"
            };
            foreach (Tone.Op op in t.OPs)
            {
                tt.Add(string.Format("'@ {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3} {8:D3} {9:D3} "
                            , op.AR, op.DR, op.SR, op.RR, op.SL, op.TL, op.KS, op.ML, op.DT, op.AM
                            ));
            }
            tt.Add("   AL  FB");
            tt.Add(string.Format("'@ {0:D3} {1:D3}", t.AL, t.FB));
            tt.Add("");

            return tt.ToArray();
        }

        private static string[] MakeToneTextForNRTDRV(Tone t, int n)
        {
            List<string> tt = new()
            {
                string.Format("@{0:D3} {{ ;{1}", n, t.name),
                ";PAN ALG FB  OP",
                string.Format(" 003,{0:D3},{1:D3},015", t.AL, t.FB),
                "; AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME"
            };
            foreach (Tone.Op op in t.OPs)
            {
                tt.Add(string.Format("  {0:D3},{1:D3},{2:D3},{3:D3},{4:D3},{5:D3},{6:D3},{7:D3},{8:D3},{9:D3},{10:D3}"
                    , op.AR, op.DR, op.SR, op.RR, op.SL, op.TL, op.KS, op.ML, op.DT, op.DT2, op.AM
                    ));
            }
            tt.Add("}");
            tt.Add("");

            return tt.ToArray();
        }

        private static string[] MakeToneTextForMXDRV(Tone t, int n)
        {
            List<string> tt = new()
            {
                string.Format("/* {0} */", t.name),
                string.Format("@{0}= {{ ", n),
                "/* AR  D1R D2R RR  D1L TL  KS  MUL DT1 DT2 AME */"
            };
            foreach (Tone.Op op in t.OPs)
            {
                tt.Add(string.Format("   {0:d3},{1:d3},{2:d3},{3:d3},{4:d3},{5:d3},{6:d3},{7:d3},{8:d3},{9:d3},{10:d3}"
                            , op.AR, op.DR, op.SR, op.RR, op.SL, op.TL, op.KS, op.ML, op.DT, op.DT2, op.AM
                            ));
            }
            tt.Add("/* CON FL  OP");
            tt.Add(string.Format("   {0:d3},{1:d3},015", t.AL, t.FB));
            tt.Add("}");
            tt.Add("");

            return tt.ToArray();
        }

        private static string[] MakeToneTextForMUSICLALF(Tone t, int n)
        {
            List<string> tt = new()
            {
                string.Format("[ROW] ' @{0}:{{", n),
                string.Format("[ROW] ' {0,3},{1,3}", t.AL, t.FB)
            };

            int o = 0;
            foreach (Tone.Op op in t.OPs)
            {
                o++;
                if (o != 4)
                {
                    tt.Add(string.Format("[ROW] ' {0,3},{1,3},{2,3},{3,3},{4,3},{5,3},{6,3},{7,3},{8,3}"
                        , op.AR, op.DR, op.SR, op.RR, op.SL, op.TL, op.KS, op.ML, op.DT
                        ));
                }
                else
                {
                    tt.Add(string.Format("[ROW] ' {0,3},{1,3},{2,3},{3,3},{4,3},{5,3},{6,3},{7,3},{8,3},\"{9}\" }}"
                        , op.AR, op.DR, op.SR, op.RR, op.SL, op.TL, op.KS, op.ML, op.DT, t.name
                        ));
                }
            }

            tt.Add("[ROW] '");

            return tt.ToArray();
        }

        public static void LoadTonePallet(string fn, int tp, TonePallet tonePallet)
        {
            if (tp == 1)
            {
                TonePallet tP = TonePallet.Load(fn);
                tonePallet.LstTone = tP.LstTone;
                return;
            }

            List<string> tnt = new();

            using (StreamReader sr = new(fn))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    tnt.Add(line);
                }
            }

            switch (tp)
            {
                case 2:
                    LoadTonePalletFromMml2vgm(tnt.ToArray(), tonePallet);
                    break;
                case 3:
                    LoadTonePalletFromFMP7(tnt.ToArray(), tonePallet);
                    break;
                case 4:
                    LoadTonePalletFromNRTDRV(tnt.ToArray(), tonePallet);
                    break;
                case 5:
                    LoadTonePalletFromMXDRV(tnt.ToArray(), tonePallet);
                    break;
                case 6:
                    LoadTonePalletFromMUSICLALF(tnt.ToArray(), tonePallet);
                    break;
            }
        }

        public static void LoadTonePalletFromMml2vgm(string[] tnt, TonePallet tonePallet)
        {
            string line;
            int stage = 0;
            List<int> toneBuf = new();
            int index = 0;

            while ((line = index == tnt.Length ? null : tnt[index++]) != null)
            {
                line = line.Trim();
                if (line.IndexOf("'@") != 0) continue;

                line = line.Replace("'@", "").Trim();
                string c = line[0].ToString().ToUpper();
                if (stage == 0 && (c == "M" || c == "N"))
                {
                    stage = 1;
                    if (line.Length < 2) continue;

                    line = line[1..].Trim();
                }

                if (stage > 0)
                {
                    int[] nums = NumSplit(line);
                    if (nums.Length == 0 && line == "xx")
                    {
                        nums = new int[] { 0 };
                    }
                    foreach (int n in nums)
                    {
                        stage++;
                        toneBuf.Add(n);
                    }
                    if (stage > 47)
                    {
                        if (stage == 48)
                        {
                            Tone t = new()
                            {
                                name = string.Format("No.{0}(From MML2VGM)", toneBuf[0]),
                                OPs = new Tone.Op[4]
                            };
                            for (int i = 0; i < 4; i++)
                            {
                                t.OPs[i] = new Tone.Op
                                {
                                    AR = toneBuf[i * 11 + 1],
                                    DR = toneBuf[i * 11 + 2],
                                    SR = toneBuf[i * 11 + 3],
                                    RR = toneBuf[i * 11 + 4],
                                    SL = toneBuf[i * 11 + 5],
                                    TL = toneBuf[i * 11 + 6],
                                    KS = toneBuf[i * 11 + 7],
                                    ML = toneBuf[i * 11 + 8],
                                    DT = toneBuf[i * 11 + 9],
                                    AM = toneBuf[i * 11 + 10],
                                    SG = toneBuf[i * 11 + 11],
                                    DT2 = 0
                                };
                            }
                            t.AL = toneBuf[45];
                            t.FB = toneBuf[46];

                            tonePallet.LstTone[toneBuf[0]] = t;
                        }

                        stage = 0;
                        toneBuf.Clear();
                    }
                }

            }
        }

        public static void LoadTonePalletFromFMP7(string[] tnt, TonePallet tonePallet)
        {
            string line;
            int stage = 0;
            List<int> toneBuf = new();
            int m = 0;
            int index = 0;

            while ((line = index == tnt.Length ? null : tnt[index++]) != null)
            {
                line = line.Trim();
                if (line.IndexOf("'@") != 0) continue;

                line = line.Replace("'@", "").Trim();
                string c = line[0].ToString().ToUpper();
                if (stage == 0 && c == "F")
                {
                    stage = 1;
                    if (line.Length < 2) continue;

                    line = line[1..].Trim();
                    c = line[0].ToString().ToUpper();
                    m = 0;//互換モード

                    if (c == "A")
                    {
                        m = 1;//OPNAモード
                        line = line[1..].Trim();
                    }
                    else if (c == "C")
                    {
                        m = 2;//OPMモード
                        line = line[1..].Trim();
                    }
                }

                if (stage > 0)
                {
                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        stage++;
                        toneBuf.Add(n);
                    }

                    if (m == 0 && stage >= 40)
                    {
                        if (stage == 40)
                        {
                            //互換
                            Tone t = new()
                            {
                                name = string.Format("No.{0}(From FMP7 compatible)", toneBuf[0]),
                                OPs = new Tone.Op[4]
                            };
                            for (int i = 0; i < 4; i++)
                            {
                                t.OPs[i] = new Tone.Op
                                {
                                    AR = toneBuf[i * 9 + 1],
                                    DR = toneBuf[i * 9 + 2],
                                    SR = toneBuf[i * 9 + 3],
                                    RR = toneBuf[i * 9 + 4],
                                    SL = toneBuf[i * 9 + 5],
                                    TL = toneBuf[i * 9 + 6],
                                    KS = toneBuf[i * 9 + 7],
                                    ML = toneBuf[i * 9 + 8],
                                    DT = toneBuf[i * 9 + 9],
                                    AM = 0,
                                    SG = 0,
                                    DT2 = 0
                                };
                            }
                            t.AL = toneBuf[37];
                            t.FB = toneBuf[38];

                            tonePallet.LstTone[toneBuf[0]] = t;
                        }

                        stage = 0;
                        toneBuf.Clear();
                    }

                    if (m == 1 && stage >= 44)
                    {
                        if (stage == 44)
                        {
                            //OPNA
                            Tone t = new()
                            {
                                name = string.Format("No.{0}(From FMP7 OPNA)", toneBuf[0]),
                                OPs = new Tone.Op[4]
                            };
                            for (int i = 0; i < 4; i++)
                            {
                                t.OPs[i] = new Tone.Op
                                {
                                    AR = toneBuf[i * 10 + 1],
                                    DR = toneBuf[i * 10 + 2],
                                    SR = toneBuf[i * 10 + 3],
                                    RR = toneBuf[i * 10 + 4],
                                    SL = toneBuf[i * 10 + 5],
                                    TL = toneBuf[i * 10 + 6],
                                    KS = toneBuf[i * 10 + 7],
                                    ML = toneBuf[i * 10 + 8],
                                    DT = toneBuf[i * 10 + 9],
                                    AM = toneBuf[i * 10 + 10],
                                    SG = 0,
                                    DT2 = 0
                                };
                            }
                            t.AL = toneBuf[41];
                            t.FB = toneBuf[42];

                            tonePallet.LstTone[toneBuf[0]] = t;
                        }

                        stage = 0;
                        toneBuf.Clear();
                    }

                    if (m == 2 && stage >= 48)
                    {
                        if (stage == 48)
                        {
                            //OPM
                            Tone t = new()
                            {
                                name = string.Format("No.{0}(From FMP7 OPM)", toneBuf[0]),
                                OPs = new Tone.Op[4]
                            };
                            for (int i = 0; i < 4; i++)
                            {
                                t.OPs[i] = new Tone.Op
                                {
                                    AR = toneBuf[i * 11 + 1],
                                    DR = toneBuf[i * 11 + 2],
                                    SR = toneBuf[i * 11 + 3],
                                    RR = toneBuf[i * 11 + 4],
                                    SL = toneBuf[i * 11 + 5],
                                    TL = toneBuf[i * 11 + 6],
                                    KS = toneBuf[i * 11 + 7],
                                    ML = toneBuf[i * 11 + 8],
                                    DT = toneBuf[i * 11 + 9],
                                    DT2 = toneBuf[i * 11 + 10],
                                    AM = toneBuf[i * 11 + 11],
                                    SG = 0
                                };
                            }
                            t.AL = toneBuf[45];
                            t.FB = toneBuf[46];

                            tonePallet.LstTone[toneBuf[0]] = t;
                        }

                        stage = 0;
                        toneBuf.Clear();
                    }
                }

            }
        }

        public static void LoadTonePalletFromNRTDRV(string[] tnt, TonePallet tonePallet)
        {
            int voiceMode = 0;

            string line;
            bool cm = false;
            int ind;
            int index = 0;

            line = index == tnt.Length ? null : tnt[index++];
            if (line == null) return;

            do
            {
                if (cm)
                {
                    //コメント中
                    ind = line.IndexOf("*/");
                    if (ind >= 0)
                    {
                        cm = false;
                        if (line.Length == 2)
                        {
                            line = index == tnt.Length ? null : tnt[index++];
                            continue;
                        }
                        line = line[(ind + 2)..];
                        continue;
                    }

                    line = index == tnt.Length ? null : tnt[index++];
                    continue;
                }

                ind = line.IndexOf(";");
                if (ind >= 0)
                {
                    if (ind == 0)
                    {
                        line = index == tnt.Length ? null : tnt[index++];
                        continue;
                    }
                    line = line[..ind];
                    continue;
                }

                ind = line.IndexOf("/*");
                if (ind >= 0)
                {
                    cm = true;
                    line = line[..ind];
                }

                string cmd = line.Trim().ToUpper();
                if (cmd.Contains("#VOICE_MODE", StringComparison.CurrentCulture))
                {
                    try
                    {
                        voiceMode = int.Parse(cmd.Replace("#VOICE_MODE", "").Trim());
                    }
                    catch
                    {
                        voiceMode = 0;
                    }
                }

                line = index == tnt.Length ? null : tnt[index++];
            } while (line != null);
            cm = false;
            int stage = 0;
            List<int> toneBuf = new();
            index = 0;

            line = index == tnt.Length ? null : tnt[index++];
            if (line == null) return;

            do
            {
                if (cm)
                {
                    //コメント中
                    ind = line.IndexOf("*/");
                    if (ind >= 0)
                    {
                        cm = false;
                        if (line.Length == 2)
                        {
                            line = index == tnt.Length ? null : tnt[index++];
                            continue;
                        }
                        line = line[(ind + 2)..];
                        continue;
                    }

                    line = index == tnt.Length ? null : tnt[index++];
                    continue;
                }

                ind = line.IndexOf(";");
                if (ind >= 0)
                {
                    if (ind == 0)
                    {
                        line = index == tnt.Length ? null : tnt[index++];
                        continue;
                    }
                    line = line[..ind];
                    continue;
                }

                ind = line.IndexOf("/*");
                if (ind >= 0)
                {
                    cm = true;
                    line = line[..ind];
                }

                string cmd = line.Trim();
                if (cmd.Length == 0)
                {
                    line = index == tnt.Length ? null : tnt[index++];
                    continue;
                }

                if (stage == 0 && cmd[0] == '@')
                {
                    stage++;
                    line = line[1..];

                    char c = line[0];
                    int? n = null;
                    while (c >= '0' && c <= '9')
                    {
                        n ??= 0;

                        n = n * 10 + (c - '0');

                        line = line[1..];
                        if (line.Length < 1) break;

                        c = line[0];
                    }
                    if (n != null) toneBuf.Add((int)n);

                    continue;
                }
                else if (stage == 1 && cmd[0] == '{')
                {
                    stage++;
                    line = line[1..];

                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        toneBuf.Add(n);
                    }

                    line = index == tnt.Length ? null : tnt[index++];
                    continue;
                }
                else if (stage == 2)
                {

                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        toneBuf.Add(n);
                    }

                    if (line.Contains('}'))
                    {
                        //
                        Tone t = new()
                        {
                            name = string.Format("No.{0}(From NRTDRV)", toneBuf[0])
                        };

                        switch (voiceMode)
                        {
                            case 0:
                                t.OPs = new Tone.Op[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    t.OPs[i] = new Tone.Op
                                    {
                                        AR = toneBuf[i * 11 + 5],
                                        DR = toneBuf[i * 11 + 6],
                                        SR = toneBuf[i * 11 + 7],
                                        RR = toneBuf[i * 11 + 8],
                                        SL = toneBuf[i * 11 + 9],
                                        TL = toneBuf[i * 11 + 10],
                                        KS = toneBuf[i * 11 + 11],
                                        ML = toneBuf[i * 11 + 12],
                                        DT = toneBuf[i * 11 + 13],
                                        DT2 = toneBuf[i * 11 + 14],
                                        AM = toneBuf[i * 11 + 15],
                                        SG = 0
                                    };
                                }
                                t.AL = toneBuf[2];
                                t.FB = toneBuf[3];

                                tonePallet.LstTone[toneBuf[0]] = t;
                                break;
                            case 1:
                                t.OPs = new Tone.Op[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    t.OPs[i] = new Tone.Op
                                    {
                                        AR = toneBuf[i * 11 + 1],
                                        DR = toneBuf[i * 11 + 2],
                                        SR = toneBuf[i * 11 + 3],
                                        RR = toneBuf[i * 11 + 4],
                                        SL = toneBuf[i * 11 + 5],
                                        TL = toneBuf[i * 11 + 6],
                                        KS = toneBuf[i * 11 + 7],
                                        ML = toneBuf[i * 11 + 8],
                                        DT = toneBuf[i * 11 + 9],
                                        DT2 = toneBuf[i * 11 + 10],
                                        AM = toneBuf[i * 11 + 11],
                                        SG = 0
                                    };
                                }
                                t.AL = toneBuf[46];
                                t.FB = toneBuf[47];

                                tonePallet.LstTone[toneBuf[0]] = t;
                                break;
                            case 2:
                                t.OPs = new Tone.Op[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    t.OPs[i] = new Tone.Op
                                    {
                                        AR = toneBuf[i * 11 + 4],
                                        DR = toneBuf[i * 11 + 5],
                                        SR = toneBuf[i * 11 + 6],
                                        RR = toneBuf[i * 11 + 7],
                                        SL = toneBuf[i * 11 + 8],
                                        TL = toneBuf[i * 11 + 9],
                                        KS = toneBuf[i * 11 + 10],
                                        ML = toneBuf[i * 11 + 11],
                                        DT = toneBuf[i * 11 + 12],
                                        DT2 = toneBuf[i * 11 + 13],
                                        AM = toneBuf[i * 11 + 14],
                                        SG = 0
                                    };
                                }
                                t.AL = toneBuf[1];
                                t.FB = toneBuf[2];

                                tonePallet.LstTone[toneBuf[0]] = t;
                                break;
                            case 3:
                                t.OPs = new Tone.Op[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    t.OPs[i] = new Tone.Op
                                    {
                                        AR = toneBuf[i * 11 + 1],
                                        DR = toneBuf[i * 11 + 2],
                                        SR = toneBuf[i * 11 + 3],
                                        RR = toneBuf[i * 11 + 4],
                                        SL = toneBuf[i * 11 + 5],
                                        TL = toneBuf[i * 11 + 6],
                                        KS = toneBuf[i * 11 + 7],
                                        ML = toneBuf[i * 11 + 8],
                                        DT = toneBuf[i * 11 + 9],
                                        DT2 = toneBuf[i * 11 + 10],
                                        AM = toneBuf[i * 11 + 11],
                                        SG = 0
                                    };
                                }
                                t.AL = toneBuf[45];
                                t.FB = toneBuf[46];

                                tonePallet.LstTone[toneBuf[0]] = t;
                                break;
                            case 4:
                                t.OPs = new Tone.Op[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    t.OPs[i] = new Tone.Op
                                    {
                                        AR = toneBuf[i * 11 + 4],
                                        DR = toneBuf[i * 11 + 5],
                                        SR = toneBuf[i * 11 + 6],
                                        RR = toneBuf[i * 11 + 7],
                                        SL = toneBuf[i * 11 + 8],
                                        TL = toneBuf[i * 11 + 9],
                                        KS = toneBuf[i * 11 + 10],
                                        ML = toneBuf[i * 11 + 11],
                                        DT = toneBuf[i * 11 + 12],
                                        DT2 = toneBuf[i * 11 + 13],
                                        AM = toneBuf[i * 11 + 14],
                                        SG = 0
                                    };
                                }
                                t.AL = toneBuf[1] & 0x7;
                                t.FB = (toneBuf[1] & 0x38) >> 3;

                                tonePallet.LstTone[toneBuf[0]] = t;
                                break;
                            case 5:
                                t.OPs = new Tone.Op[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    t.OPs[i] = new Tone.Op
                                    {
                                        AR = toneBuf[i * 11 + 12],
                                        DR = toneBuf[i * 11 + 13],
                                        SR = toneBuf[i * 11 + 14],
                                        RR = toneBuf[i * 11 + 15],
                                        SL = toneBuf[i * 11 + 16],
                                        TL = toneBuf[i * 11 + 17],
                                        KS = toneBuf[i * 11 + 18],
                                        ML = toneBuf[i * 11 + 19],
                                        DT = toneBuf[i * 11 + 20],
                                        DT2 = toneBuf[i * 11 + 21],
                                        AM = toneBuf[i * 11 + 22],
                                        SG = 0
                                    };
                                }
                                t.AL = toneBuf[1] & 0x7;
                                t.FB = (toneBuf[1] & 0x38) >> 3;

                                tonePallet.LstTone[toneBuf[0]] = t;
                                break;
                        }

                        stage = 0;
                        line = line[1..];
                        toneBuf.Clear();
                        continue;
                    }
                }

                line = index == tnt.Length ? null : tnt[index++];
            } while (line != null);


        }

        public static void LoadTonePalletFromMXDRV(string[] tnt, TonePallet tonePallet)
        {
            string line;
            int stage = 0;
            List<int> toneBuf = new();
            int index = 0;

            line = index == tnt.Length ? null : tnt[index++];
            if (line == null) return;

            do
            {

                line = line.Trim();

                if (line.Length > 1 && line[0] == '/' && line[1] == '*')
                {
                    line = index == tnt.Length ? null : tnt[index++];
                    if (line == null) return;
                    continue;
                }

                if (line == "")
                {
                    line = index == tnt.Length ? null : tnt[index++];
                    if (line == null) return;
                    continue;
                }

                if (stage == 0 && line[0] == '@')
                {
                    stage++;
                    line = line[1..];

                    char c = line[0];
                    int? n = null;
                    while (c >= '0' && c <= '9')
                    {
                        n ??= 0;

                        n = n * 10 + (c - '0');

                        line = line[1..];
                        if (line.Length < 1) break;

                        c = line[0];
                    }
                    if (n != null) toneBuf.Add((int)n);

                    continue;
                }
                else if (stage == 1 && line[0] == '=')
                {
                    stage++;

                    if (line.Length > 1)
                    {
                        line = line[1..];
                    }
                    else
                    {
                        line = index == tnt.Length ? null : tnt[index++];
                        if (line == null) return;
                    }
                    continue;
                }
                else if (stage == 2 && line[0] == '{')
                {
                    stage++;

                    if (line.Length > 1)
                    {
                        line = line[1..];
                    }
                    else
                    {
                        line = index == tnt.Length ? null : tnt[index++];
                        if (line == null) return;
                        continue;
                    }

                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        toneBuf.Add(n);
                    }

                    line = index == tnt.Length ? null : tnt[index++];
                    if (line == null) return;
                    continue;
                }
                else if (stage == 3)
                {

                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        toneBuf.Add(n);
                    }

                    if (line.Contains('}'))
                    {

                        if (toneBuf.Count == 48)
                        {
                            Tone t = new()
                            {
                                name = string.Format("No.{0}(From MXDRV)", toneBuf[0]),

                                OPs = new Tone.Op[4]
                            };
                            for (int i = 0; i < 4; i++)
                            {
                                t.OPs[i] = new Tone.Op
                                {
                                    AR = toneBuf[i * 11 + 1],
                                    DR = toneBuf[i * 11 + 2],
                                    SR = toneBuf[i * 11 + 3],
                                    RR = toneBuf[i * 11 + 4],
                                    SL = toneBuf[i * 11 + 5],
                                    TL = toneBuf[i * 11 + 6],
                                    KS = toneBuf[i * 11 + 7],
                                    ML = toneBuf[i * 11 + 8],
                                    DT = toneBuf[i * 11 + 9],
                                    DT2 = toneBuf[i * 11 + 10],
                                    AM = toneBuf[i * 11 + 11],
                                    SG = 0
                                };
                            }
                            t.AL = toneBuf[45];
                            t.FB = toneBuf[46];

                            tonePallet.LstTone[toneBuf[0]] = t;
                        }
                        stage = 0;
                        line = line[1..];
                        toneBuf.Clear();
                        continue;
                    }
                }

                line = index == tnt.Length ? null : tnt[index++];
            } while (line != null);
        }

        public static void LoadTonePalletFromMUSICLALF(string[] tnt, TonePallet tonePallet)
        {
            string line;
            int stage = 0;
            List<int> toneBuf = new();
            string nm = "";
            int index = 0;

            line = index == tnt.Length ? null : tnt[index++];
            if (line == null) return;
            line = line[(line.IndexOf("'") + 1)..].Trim();

            do
            {

                line = line.Trim();

                if (line.Length > 0 && line[0] == ';')
                {
                    line = index == tnt.Length ? null : tnt[index++];
                    if (line == null) return;
                    line = line[(line.IndexOf("'") + 1)..].Trim();
                    continue;
                }

                if (line == "")
                {
                    line = index == tnt.Length ? null : tnt[index++];
                    if (line == null) return;
                    line = line[(line.IndexOf("'") + 1)..].Trim();
                    continue;
                }

                if (stage == 0 && line[0] == '@')
                {
                    stage++;
                    line = line[1..];

                    char c = line[0];
                    int? n = null;
                    while (c >= '0' && c <= '9')
                    {
                        n ??= 0;

                        n = n * 10 + (c - '0');

                        line = line[1..];
                        if (line.Length < 1) break;

                        c = line[0];
                    }
                    if (n != null) toneBuf.Add((int)n);

                    continue;
                }
                else if (stage == 1 && line[0] == ':')
                {
                    stage++;

                    if (line.Length > 1)
                    {
                        line = line[1..];
                    }
                    else
                    {
                        line = index == tnt.Length ? null : tnt[index++];
                        if (line == null) return;
                        line = line[(line.IndexOf("'") + 1)..].Trim();
                    }
                    continue;
                }
                else if (stage == 2 && line[0] == '{')
                {
                    stage++;

                    if (line.Length > 1)
                    {
                        line = line[1..];
                    }
                    else
                    {
                        line = index == tnt.Length ? null : tnt[index++];
                        if (line == null) return;
                        line = line[(line.IndexOf("'") + 1)..].Trim();
                        continue;
                    }

                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        toneBuf.Add(n);
                    }

                    line = index == tnt.Length ? null : tnt[index++];
                    if (line == null) return;
                    line = line[(line.IndexOf("'") + 1)..].Trim();
                    continue;
                }
                else if (stage == 3)
                {

                    int[] nums = NumSplit(line);
                    foreach (int n in nums)
                    {
                        toneBuf.Add(n);
                    }

                    if (toneBuf.Count >= 39)
                    {
                        stage = 4;
                        continue;
                    }
                }
                else if (stage == 4)
                {
                    if (line.Contains('"'))
                    {
                        try
                        {
                            string n = line[(line.IndexOf('"') + 1)..];
                            nm = n[..n.IndexOf('"')];
                        }
                        catch
                        {
                        }
                    }

                    if (line.Contains('}'))
                    {

                        if (toneBuf.Count == 39)
                        {
                            Tone t = new()
                            {
                                name = nm == "" ? string.Format("No.{0}(From MusicLALF)", toneBuf[0]) : nm,

                                OPs = new Tone.Op[4]
                            };
                            for (int i = 0; i < 4; i++)
                            {
                                t.OPs[i] = new Tone.Op
                                {
                                    AR = toneBuf[i * 9 + 3],
                                    DR = toneBuf[i * 9 + 4],
                                    SR = toneBuf[i * 9 + 5],
                                    RR = toneBuf[i * 9 + 6],
                                    SL = toneBuf[i * 9 + 7],
                                    TL = toneBuf[i * 9 + 8],
                                    KS = toneBuf[i * 9 + 9],
                                    ML = toneBuf[i * 9 + 10],
                                    DT = toneBuf[i * 9 + 11],
                                    DT2 = 0,
                                    AM = 0,
                                    SG = 0
                                };
                            }
                            t.AL = toneBuf[1];
                            t.FB = toneBuf[2];

                            tonePallet.LstTone[toneBuf[0]] = t;
                        }
                        stage = 0;
                        nm = "";
                        line = line[1..];
                        toneBuf.Clear();
                        continue;
                    }
                }

                line = index == tnt.Length ? null : tnt[index++];
                if (line == null) return;
                line = line[(line.IndexOf("'") + 1)..].Trim();
            } while (line != null);
        }

        private static int[] NumSplit(string line)
        {
            List<int> ret = new();

            line = line.Trim();
            while (line.Length > 0)
            {
                char c = line[0];
                int? n = null;
                while (c >= '0' && c <= '9')
                {
                    n ??= 0;

                    n = n * 10 + (c - '0');

                    line = line[1..];
                    if (line.Length < 1) break;

                    c = line[0];
                }

                if (n != null) ret.Add((int)n);

                if (line.Length < 1) break;
                line = line[1..];

                line = line.Trim();
            }

            return ret.ToArray();
        }

        public void CopyToneToClipboard(int[] chs)
        {
            if (chs == null || chs.Length < 1) return;

            List<string> des = new();

            foreach (int ch in chs)
            {
                string[] tt = null;
                switch (parent.setting.midiKbd.UseFormat)
                {
                    case 0:
                        tt = MakeToneTextForMml2vgm(parent.setting.midiKbd.Tones[ch], ch + 1);
                        break;
                    case 2:
                        tt = MakeToneTextForFMP7(parent.setting.midiKbd.Tones[ch], ch + 1);
                        break;
                    case 4:
                        tt = MakeToneTextForMXDRV(parent.setting.midiKbd.Tones[ch], ch + 1);
                        break;
                    case 3:
                        tt = MakeToneTextForMUSICLALF(parent.setting.midiKbd.Tones[ch], ch + 1);
                        break;
                    case 1:
                        tt = MakeToneTextForNRTDRV(parent.setting.midiKbd.Tones[ch], ch + 1);
                        break;
                }

                if (tt == null || tt.Length < 1) return;

                foreach (string text in tt)
                {
                    des.Add(text);// += text + "\r\n";
                }

            }

            if (parent.setting.midiKbd.UseFormat == 3)
            {
                int row = 10;
                for (int i = 0; i < des.Count; i++)
                {
                    des[i] = des[i].Replace("[ROW]", row.ToString());
                    row += 10;
                }
            }

            string n = "";
            foreach (string text in des)
            {
                n += text + "\r\n";
            }

            Clipboard.SetText(n);
        }

        public void PasteToneFromClipboard(int[] chs)
        {
            string cbt = Clipboard.GetText();
            if (cbt == string.Empty) return;
            string[] tnt = cbt.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            TonePallet tp = new()
            {
                LstTone = new List<Tone>(256)
            };
            for (int i = 0; i < 256; i++) tp.LstTone.Add(null);

            switch (parent.setting.midiKbd.UseFormat)
            {
                case 0:
                    LoadTonePalletFromMml2vgm(tnt, tp);
                    break;
                case 2:
                    LoadTonePalletFromFMP7(tnt, tp);
                    break;
                case 4:
                    LoadTonePalletFromMXDRV(tnt, tp);
                    break;
                case 3:
                    LoadTonePalletFromMUSICLALF(tnt, tp);
                    break;
                case 1:
                    LoadTonePalletFromNRTDRV(tnt, tp);
                    break;
            }

            int j = 0;
            foreach (int ch in chs)
            {
                Tone t = null;
                while (t == null && j < 256)
                {
                    t = tp.LstTone[j];
                    j++;
                }

                if (t != null) parent.setting.midiKbd.Tones[ch] = t;
            }

            SetTonesFromSettng();
        }

        public void ChangeSelectedParamValue(int n)
        {
            int ch = newParam.ym2612Midi.selectCh;
            int p = newParam.ym2612Midi.selectParam;
            if (ch == -1 || p == -1) return;

            if (p >= 44 && p < 48)
            {
                switch (p)
                {
                    case 44:
                        parent.setting.midiKbd.Tones[ch].AL += n;
                        break;
                    case 45:
                        parent.setting.midiKbd.Tones[ch].FB += n;
                        break;
                    case 46:
                        parent.setting.midiKbd.Tones[ch].AMS += n;
                        break;
                    case 47:
                        parent.setting.midiKbd.Tones[ch].PMS += n;
                        break;
                }
            }
            else
            {
                int op = p / 11;
                switch (p % 11)
                {
                    case 0:
                        parent.setting.midiKbd.Tones[ch].OPs[op].AR += n;
                        break;
                    case 1:
                        parent.setting.midiKbd.Tones[ch].OPs[op].DR += n;
                        break;
                    case 2:
                        parent.setting.midiKbd.Tones[ch].OPs[op].SR += n;
                        break;
                    case 3:
                        parent.setting.midiKbd.Tones[ch].OPs[op].RR += n;
                        break;
                    case 4:
                        parent.setting.midiKbd.Tones[ch].OPs[op].SL += n;
                        break;
                    case 5:
                        parent.setting.midiKbd.Tones[ch].OPs[op].TL += n;
                        break;
                    case 6:
                        parent.setting.midiKbd.Tones[ch].OPs[op].KS += n;
                        break;
                    case 7:
                        parent.setting.midiKbd.Tones[ch].OPs[op].ML += n;
                        break;
                    case 8:
                        parent.setting.midiKbd.Tones[ch].OPs[op].DT += n;
                        break;
                    case 9:
                        parent.setting.midiKbd.Tones[ch].OPs[op].AM += n;
                        break;
                    case 10:
                        parent.setting.midiKbd.Tones[ch].OPs[op].SG += n;
                        break;
                }
            }
            SetTonesFromSettng();
        }

    }
}


//[DllImport("user32.dll")]
//private static extern IntPtr GetForegroundWindow();
//[DllImport("user32.dll", CharSet = CharSet.Auto , EntryPoint ="SendMessage")]
//public static extern int SendMessage2(IntPtr hWnd, int Msg, IntPtr dummy1, IntPtr dummy2);
//[DllImport("user32.dll")]
//public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


//[DllImport("user32.dll", SetLastError = true)]
//public static extern bool GetGUIThreadInfo(uint hTreadID, ref GUITHREADINFO lpgui);

//[DllImport("user32.dll")]
//public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

//[StructLayout(LayoutKind.Sequential)]
//public struct RECT
//{
//    public int iLeft;
//    public int iTop;
//    public int iRight;
//    public int iBottom;
//}

//[StructLayout(LayoutKind.Sequential)]
//public struct GUITHREADINFO
//{
//    public int cbSize;
//    public int flags;
//    public IntPtr hwndActive;
//    public IntPtr hwndFocus;
//    public IntPtr hwndCapture;
//    public IntPtr hwndMenuOwner;
//    public IntPtr hwndMoveSize;
//    public IntPtr hwndCaret;
//    public RECT rectCaret;
//}

//public static bool GetInfo(IntPtr hwnd, out GUITHREADINFO lpgui)
//{
//    uint lpdwProcessId;
//    uint threadId = GetWindowThreadProcessId(hwnd, out lpdwProcessId);

//    lpgui = new GUITHREADINFO();
//    lpgui.cbSize = Marshal.SizeOf(lpgui);

//    return GetGUIThreadInfo(threadId, ref lpgui);
//}

