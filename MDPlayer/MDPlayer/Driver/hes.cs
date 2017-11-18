using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class hes : baseDriver
    {
        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (common.getLE32(buf, 0) != FCC_HES)
            {
                return null;
            }

            if (buf.Length < 0x20) // no header?
                return null;

            version = buf[0x04];
            songs = (byte)255;
            start = (byte)(buf[0x05] + 1);
            load_address = 0;
            init_address = (UInt16)(buf[0x06] | (buf[0x07] << 8));
            play_address = 0;

            //HESの曲情報はほぼ無い?
            return null;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, enmModel model, enmUseChip[] useChip, uint latency)
        {

            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;

            if (model == enmModel.RealModel)
            {
                Stopped = true;
                vgmCurLoop = 9999;
                return true;
            }

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = 0;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            GD3 = getGD3Info(vgmBuf, 0);

            m_hes = new m_hes();
            m_hes.chipRegister = chipRegister;
            nez_play = new m_hes.NEZ_PLAY();
            if (m_hes.HESLoad(nez_play, vgmBuf, (UInt32)vgmBuf.Length) != 0) return false;
            nez_play.song.songno = (UInt32)(this.song + 1);
            m_hes.HESHESReset(nez_play);
            return true;
        }

        public override void oneFrameProc()
        {
            if (m_hes == null) return;
            m_hes.ExecuteHES(nez_play);
        }

        public const int FCC_HES = 0x4d534548;  // "HESM"

        public byte version;
        public byte songs;
        public byte start;
        public UInt16 load_address;
        public UInt16 init_address;
        public UInt16 play_address;
        public byte song;

        public m_hes m_hes;
        public m_hes.NEZ_PLAY nez_play;
        public MDSound.MDSound.Chip c6280;
    }
}
