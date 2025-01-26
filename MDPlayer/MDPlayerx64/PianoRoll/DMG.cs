using MDPlayer;
using MDSound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayerx64.PianoRoll
{
    public class DMG(List<PrNote> lstPrNote, int MAXChip = 2) : BaseChip(lstPrNote, MAXChip)
    {
        private List<byte[]> reg;
        private List<PrNote[]> SSGNote;

        private const Int32 NR10 = 0x00;
        private const Int32 NR11 = 0x01;
        private const Int32 NR12 = 0x02;
        private const Int32 NR13 = 0x03;
        private const Int32 NR14 = 0x04;
        // 0x05
        private const Int32 NR21 = 0x06;
        private const Int32 NR22 = 0x07;
        private const Int32 NR23 = 0x08;
        private const Int32 NR24 = 0x09;
        private const Int32 NR30 = 0x0A;
        private const Int32 NR31 = 0x0B;
        private const Int32 NR32 = 0x0C;
        private const Int32 NR33 = 0x0D;
        private const Int32 NR34 = 0x0E;
        // 0x0F
        private const Int32 NR41 = 0x10;
        private const Int32 NR42 = 0x11;
        private const Int32 NR43 = 0x12;
        private const Int32 NR44 = 0x13;
        private const Int32 NR50 = 0x14;
        private const Int32 NR51 = 0x15;
        private const Int32 NR52 = 0x16;
        // 0x17 - 0x1F
        private const Int32 AUD3W0 = 0x20;
        private const Int32 AUD3W1 = 0x21;
        private const Int32 AUD3W2 = 0x22;
        private const Int32 AUD3W3 = 0x23;
        private const Int32 AUD3W4 = 0x24;
        private const Int32 AUD3W5 = 0x25;
        private const Int32 AUD3W6 = 0x26;
        private const Int32 AUD3W7 = 0x27;
        private const Int32 AUD3W8 = 0x28;
        private const Int32 AUD3W9 = 0x29;
        private const Int32 AUD3WA = 0x2A;
        private const Int32 AUD3WB = 0x2B;
        private const Int32 AUD3WC = 0x2C;
        private const Int32 AUD3WD = 0x2D;
        private const Int32 AUD3WE = 0x2E;
        private const Int32 AUD3WF = 0x2F;

        public override void Clear()
        {
            reg = [];
            SSGNote = [];
            for (int i = 0; i < MAXChip; i++)
            {
                reg.Add(new byte[0x100]);
                SSGNote.Add(new PrNote[4]);
            }
        }

        private int[] freq = new int[4];
        private int[] ev = new int[4];
        private int[] md = new int[4];
        private int[] note = new int[4];
        private float[] ftone = new float[4];
        private int[] vol = new int[4];

        public override void Analyze(int chipID, int dAdr, int dData, long vgmFrameCounter)
        {
            if (reg == null) return;
            reg[chipID][dAdr] = (byte)dData;

            freq[0] = ((reg[chipID][NR14] & 0x7) << 8) | reg[chipID][NR13]; //Ch1 freq
            freq[1] = ((reg[chipID][NR24] & 0x7) << 8) | reg[chipID][NR23]; //Ch2 freq
            freq[2] = ((reg[chipID][NR34] & 0x7) << 8) | reg[chipID][NR33]; //Ch3 freq
            freq[3] = (reg[chipID][NR43] & 0x7); //Ch4 freq
            ev[0] = (sbyte)(reg[chipID][NR12] >> 4);//Ch1 env vol
            ev[1] = (sbyte)(reg[chipID][NR22] >> 4);//Ch2 env vol
            ev[2] = (byte)((reg[chipID][NR32] & 0x60) >> 5);//Ch3 vol
            ev[3] = (sbyte)(reg[chipID][NR42] >> 4);//Ch4 env vol
            md[0] = (reg[chipID][NR51] & 0x11);//Ch1 mode1(PAN LR)
            md[1] = (reg[chipID][NR51] & 0x22);//Ch2 mode1(PAN LR)
            md[2] = (reg[chipID][NR51] & 0x44);//Ch3 mode1(PAN LR)
            md[3] = (reg[chipID][NR51] & 0x88);//Ch4 mode1(PAN LR)

            ftone[0] = 4194304.0f / (4 * 2 * (2048.0f - (float)freq[0]));
            ftone[1] = 4194304.0f / (4 * 2 * (2048.0f - (float)freq[1]));
            ftone[2] = 4194304.0f / (4 * 2 * (2048.0f - (float)freq[2]));
            note[0] = (95 - Common.searchSSGNote(ftone[0]));
            note[1] = (95 - Common.searchSSGNote(ftone[1]));
            note[2] = (95 - Common.searchSSGNote(ftone[2]));
            note[3] = freq[3];

            for (int ch = 0; ch < 4; ch++)
            {
                vol[ch] = ev[ch] * md[ch];
                if (vol[ch] == 0) note[ch] = -1;

                if (note[ch] != -1)
                {
                    if (SSGNote[chipID][ch] == null)
                    {
                        //keyONした！
                        SSGNote[chipID][ch] = MakeSSGNote(ch, vgmFrameCounter, note[ch], freq[ch]);
                        lstPrNote.Add(SSGNote[chipID][ch]);
                    }
                    else
                    {
                        //keyON中!
                        if (SSGNote[chipID][ch].key != note[ch])
                        {
                            //音程が異なる場合は新たなノートとする
                            SSGNote[chipID][ch].endTick = vgmFrameCounter;
                            SSGNote[chipID][ch] = MakeSSGNote(ch, vgmFrameCounter, note[ch], freq[ch]);
                            lstPrNote.Add(SSGNote[chipID][ch]);
                        }
                    }
                    continue;
                }

                //keyOFF
                if (SSGNote[chipID][ch] == null) continue;

                //keyOFFした！
                SSGNote[chipID][ch].endTick = vgmFrameCounter;
                SSGNote[chipID][ch] = null;

                //keyOFF中は何もしない
            }
        }

        private static PrNote MakeSSGNote(int ch, long startTick, int note, int freq)
        {
            PrNote ret = new()
            {
                ch = ch,
                startTick = startTick,
                endTick = -1,//長さ未確定
                key = note,
                freq = freq
            };

            ret.noteColor1[0] = 0x00;
            ret.noteColor1[1] = 0x50;
            ret.noteColor1[2] = 0x08;
            ret.noteColor1[3] = 0x00;
            ret.noteColor1[4] = 0x70;
            ret.noteColor1[5] = 0x08;

            ret.noteColor2[0] = 0x00;
            ret.noteColor2[1] = 0x80;
            ret.noteColor2[2] = 0x30;
            ret.noteColor2[3] = 0x00;
            ret.noteColor2[4] = 0xa0;
            ret.noteColor2[5] = 0x50;

            return ret;
        }

        private int searchSSGNote(float freq)
        {
            float m = float.MaxValue;
            int n = 0;
            for (int i = 0; i < 12 * 9; i++)
            {
                float a = Math.Abs((freq / (1 << (6 - 4))) - Tables.freqTbl[i]);// 6:正規の範囲   4:補正
                if (m > a)
                {
                    m = a;
                    n = i;
                }
                else break;
            }
            return n;
        }

    }
}
