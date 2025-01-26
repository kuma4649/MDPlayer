using MDPlayer.Driver.MNDRV;
using MDPlayerx64.PianoRoll;
using NAudio.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDPlayer.MDChipParams;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MDPlayer
{
    public class PianoRollMng
    {
        public List<PrNote> lstPrNote = [];
        private Dictionary<EnmChip, BaseChip> chipList = [];

        public PianoRollMng()
        {
            chipList.Add(EnmChip.AY8910, new MDPlayerx64.PianoRoll.AY8910(lstPrNote));
            chipList.Add(EnmChip.DMG, new MDPlayerx64.PianoRoll.DMG(lstPrNote));
            chipList.Add(EnmChip.HuC6280, new MDPlayerx64.PianoRoll.HuC6280(lstPrNote));
            chipList.Add(EnmChip.K051649, new MDPlayerx64.PianoRoll.K051649(lstPrNote));
            chipList.Add(EnmChip.YM2151, new MDPlayerx64.PianoRoll.YM2151(lstPrNote));
            chipList.Add(EnmChip.YM2413, new MDPlayerx64.PianoRoll.YM2413(lstPrNote));
            chipList.Add(EnmChip.YM2608, new MDPlayerx64.PianoRoll.YM2608(lstPrNote));
            chipList.Add(EnmChip.YM2610, new MDPlayerx64.PianoRoll.YM2610(lstPrNote));
            chipList.Add(EnmChip.YM2612, new MDPlayerx64.PianoRoll.YM2612(lstPrNote));
            chipList.Add(EnmChip.SN76489, new MDPlayerx64.PianoRoll.SN76489(lstPrNote));
        }

        public void Clear()
        {
            lstPrNote.Clear();
            foreach(var chip in chipList)
                chip.Value.Clear();
        }

        public void SetRegister(EnmChip chip, int chipID, int dAdr, int dData, long vgmFrameCounter)
        {
            if (!chipList.TryGetValue(chip, out BaseChip value)) return;
            value.Analyze(chipID, dAdr, dData, vgmFrameCounter);
        }

    }

    public class PrNote
    {
        public int ch;//channel
        public long startTick;//開始Tick
        public long endTick;//完了Tick
        public int key;//音程
        public int freq;

        public byte[] color = new byte[6];
        public byte[] trgColor = new byte[6];

        public byte[] noteColor1 = new byte[6];
        public byte[] noteColor2 = new byte[6];
    }
}
