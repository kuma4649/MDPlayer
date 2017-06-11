using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer
{
    public class YM2612MIDI
    {
        private int latestNoteNumberMONO = -1;
        private int[] latestNoteNumber = new int[6] { -1, -1, -1, -1, -1, -1 };

        private Setting setting=null;
        private MDSound.MDSound mdsMIDI=null;
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



        public YM2612MIDI(Setting setting, MDSound.MDSound mdsMIDI, MDChipParams newParam)
        {
            this.setting = setting;
            this.mdsMIDI = mdsMIDI;
            this.newParam = newParam;

            for (int ch = 0; ch < 6; ch++)
            {
                for (int n = 0; n < 100; n++) NoteLog[ch][n] = -1;
                NoteLogPtr[ch] = 0;
                if (setting.other.Tones != null && setting.other.Tones[ch] != null)
                    VoiceCopyChFromTone(ch, setting.other.Tones[ch]);
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

            int ch = setting.other.UseMONOChannel;
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
            int ch = setting.other.UseMONOChannel;
            if (ch < 0 || ch > 5) return;

            if (noteNumber == latestNoteNumberMONO)
                mdsMIDI.WriteYM2612(0, 0, 0x28, (byte)(0x00 + ch + (ch / 3)));
        }

        private int NoteON(int noteNumber)
        {
            if (setting.other.IsMONO)
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
                if (!setting.other.UseChannel[ch]) continue;
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
            if (setting.other.IsMONO)
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
            for (int i = 0; i < 4; i++)
            {
                int opn = (i == 0) ? 0 : ((i == 1) ? 8 : ((i == 2) ? 4 : 12));

                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x50 + opn + (des % 3)), (byte)((tone.OPs[i].AR & 0x1f) + ((tone.OPs[i].KS & 0x3) << 6)));//AR + KS
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x60 + opn + (des % 3)), (byte)((tone.OPs[i].DR & 0x1f) + ((tone.OPs[i].AM & 0x1) << 7)));//DR + AM
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x70 + opn + (des % 3)), (byte)((tone.OPs[i].SR & 0x1f)));//SR
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x80 + opn + (des % 3)), (byte)((tone.OPs[i].RR & 0xf) + ((tone.OPs[i].SL & 0xf) << 4)));//RR + SL
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x40 + opn + (des % 3)), (byte)((tone.OPs[i].TL & 0x7f)));//TL
                mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0x30 + opn + (des % 3)), (byte)((tone.OPs[i].ML & 0xf) + ((tone.OPs[i].DT & 0x7) << 4)));//ML + DT
            }

            mdsMIDI.WriteYM2612(0, (byte)(des / 3), (byte)(0xb0 + (des % 3)), (byte)((tone.AL & 0x7) + ((tone.FB & 0x7) << 3)));//AL + FB

        }

        private Tone VoiceCopyChToTone(int des, string name)
        {
            Tone tone = new Tone();
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

        private Tone VoiceCopyToneToTone(Tone src,string name)
        {
            Tone des = new Tone();

            for (int i = 0; i < 4; i++)
            {
                des.OPs[i].AR = src.OPs[i].AR;//AR
                des.OPs[i].KS = src.OPs[i].KS;//KS
                des.OPs[i].DR = src.OPs[i].DR;//DR
                des.OPs[i].AM = src.OPs[i].AM;//AM
                des.OPs[i].SR = src.OPs[i].SR;//SR
                des.OPs[i].RR = src.OPs[i].RR;//RR
                des.OPs[i].SL = src.OPs[i].SL;//SL
                des.OPs[i].TL = src.OPs[i].TL;//TL
                des.OPs[i].ML = src.OPs[i].ML;//ML
                des.OPs[i].DT = src.OPs[i].DT;//DT
                des.OPs[i].DT2 = 0;
            }

            des.AL = src.AL;//AL
            des.FB = src.FB;//FB
            des.AMS = 0;
            des.PMS = 0;
            des.name = name;

            return des;
        }

        public void SetVoiceFromChipRegister(enmUseChip chip, int chipID, int ch)
        {
            if (chip == enmUseChip.YM2612 || chip == enmUseChip.YM2608 || chip == enmUseChip.YM2610 || chip == enmUseChip.YM2203)
            {
                int[][] srcRegs = null;
                if (chip == enmUseChip.YM2612)
                {
                    srcRegs = Audio.GetFMRegister(chipID);
                }
                else if (chip == enmUseChip.YM2608)
                {
                    srcRegs = Audio.GetYM2608Register(chipID);
                }
                else if (chip == enmUseChip.YM2610)
                {
                    srcRegs = Audio.GetYM2610Register(chipID);
                }
                else if (chip == enmUseChip.YM2203)
                {
                    int[] sReg = Audio.GetYM2203Register(chipID);
                    srcRegs = new int[2][] { sReg, null };
                }
                for (int i = 0; i < 6; i++)
                {
                    if (setting.other.UseChannel[i])
                    {
                        VoiceCopyCh(ch, i, srcRegs);
                    }
                }
            }
            else if (chip == enmUseChip.YM2151)
            {
                int[] reg = Audio.GetYM2151Register(chipID);
                for (int i = 0; i < 6; i++)
                {
                    if (setting.other.UseChannel[i])
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
                        setting.other.UseChannel[ch] = false;
                        if (ch == setting.other.UseMONOChannel)
                        {
                            setting.other.UseChannel[ch] = true;
                        }
                    }
                    setting.other.IsMONO = true;
                    break;
                default:
                    //POLY
                    setting.other.IsMONO = false;
                    break;
            }
        }

        public void SelectChannel(int ch)
        {
            if (setting.other.IsMONO)
            {
                setting.other.UseMONOChannel = ch;
                SetMode(0);
            }
            else
            {
                setting.other.UseChannel[ch] = !setting.other.UseChannel[ch];
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
            StringBuilder mml = new StringBuilder("o");

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
                        mml.Append(">");
                        oct++;
                    } while (oct != o);
                }
                else if (s > 0)
                {
                    do
                    {
                        mml.Append("<");
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
            StringBuilder mml = new StringBuilder("");

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
                        mml.Append(">");
                        oct++;
                    } while (oct != o);
                }
                else if (s > 0)
                {
                    do
                    {
                        mml.Append("<");
                        oct--;
                    } while (oct != o);
                }

                //ノートコマンド
                mml.Append(tblNote[n]);

                ptr++;
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

        public void midiIn_MessageReceived(MidiInMessageEventArgs e)
        {
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
            {
                NoteOnEvent noe = (NoteOnEvent)e.MidiEvent;

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
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                NoteEvent ne = (NoteEvent)e.MidiEvent;
                NoteOFF(ne.NoteNumber);
                return;
            }
            if (e.MidiEvent.CommandCode == MidiCommandCode.ControlChange)
            {
                ControlChangeEvent ce = (ControlChangeEvent)e.MidiEvent;
                if ((int)ce.Controller == 97) VoiceCopy();
                if ((int)ce.Controller == 66)
                {
                    if (setting.other.IsMONO) Log2MML66(setting.other.UseMONOChannel);
                }
            }
        }

        public void SetTonesToSettng()
        {
            setting.other.Tones = new Tone[6];
            for (int ch = 0; ch < 6; ch++)
            {
                setting.other.Tones[ch] = VoiceCopyChToTone(ch, "");
            }
        }

        public void SetTonesFromSettng()
        {
            for (int ch = 0; ch < 6; ch++)
            {
                VoiceCopyChFromTone(ch, setting.other.Tones[ch]);
            }
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

