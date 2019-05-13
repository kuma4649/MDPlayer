using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound;

namespace MDPlayer
{
    public class hes : baseDriver
    {
        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (Common.getLE32(buf, 0) != FCC_HES)
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

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
        {

            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            //if (model == EnmModel.RealModel)
            //{
            //    Stopped = true;
            //    vgmCurLoop = 9999;
            //    return true;
            //}

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;
            silent_length = 0;
            playtime_detected = false;

            ld = new HESDetector();
            ld.Reset();

            GD3 = getGD3Info(vgmBuf, 0);

            m_hes = new m_hes();
            m_hes.chipRegister = chipRegister;
            m_hes.ld = ld;
            nez_play = new m_hes.NEZ_PLAY();
            if (m_hes.HESLoad(nez_play, vgmBuf, (UInt32)vgmBuf.Length) != 0) return false;
            nez_play.song.songno = (UInt32)(this.song + 1);
            m_hes.HESHESReset(nez_play);
            return true;
        }

        public override void oneFrameProc()
        {
            if (m_hes == null) return;
            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0 && !Stopped)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        m_hes.ExecuteHES(nez_play);
                        Counter++;
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

        private int last_out = 0;
        private int silent_length = 0;
        private HESDetector ld = null;
        private double time_in_ms;
        public bool playtime_detected = false;

        private int[] buf = new int[2];

        internal void AdditionalUpdate(MDSound.MDSound.Chip sender, byte ChipID, int[][] Buffer, int Length)
        {
            try
            {
                for (int i = 0; i < Length; i++)
                {

                    int m = Buffer[0][i] + Buffer[1][i];
                    if (m == last_out && vgmFrameCounter >= 0) silent_length++;
                    else silent_length = 0;
                    last_out = m;

                    if (nez_play != null && nez_play.heshes!=null)
                    {
                        buf[0] = 0;buf[1] = 0;
                        m_hes.synth(nez_play.heshes, buf);
                        Buffer[0][i] += buf[0];
                        Buffer[1][i] += buf[1];
                    }
                }

                if (!playtime_detected && silent_length > Common.SampleRate * 3)
                {
                    playtime_detected = true;
                    LoopCounter = 0;
                    Stopped = true;
                }

                time_in_ms += (1000 * Length / (double)Common.SampleRate * vgmSpeed);// ((* config)["MULT_SPEED"].GetInt()) / 256);
                if (!playtime_detected && ld.IsLooped((int)time_in_ms, 30000, 5000))
                {
                    playtime_detected = true;
                    TotalCounter = (long)ld.GetLoopEnd() * (long)Common.SampleRate / 1000L;
                    if (TotalCounter == 0) TotalCounter = Counter;
                    LoopCounter = (long)(((long)ld.GetLoopEnd() - (long)ld.GetLoopStart()) * (long)Common.SampleRate / 1000L);
                }

                if (!playtime_detected) vgmCurLoop = 0;
                else
                {
                    if (TotalCounter != 0) vgmCurLoop = (uint)(Counter / TotalCounter);
                    else Stopped = true;
                }
            }
            catch(Exception ex)
            {
                log.Write(string.Format("Exception message:{0} StackTrace:{1}",ex.Message,ex.StackTrace));
            }

        }

        public class HESDetector : MDSound.np.BasicDetector
        {
            public HESDetector(): base(18)
            {
            }

            public override bool Write(UInt32 adr, UInt32 val, UInt32 id)
            {
                if (
                    adr<0x10
                  )
                {
                    return base.Write(adr, val, id);
                }

                return false;
            }

            public new bool IsLooped(int time_in_ms, int match_second, int match_interval)
            {
                int i, j;
                int match_size, match_length;

                if (time_in_ms - m_current_time < match_interval)
                    return false;

                m_current_time = time_in_ms;

                if (m_bidx <= m_blast)
                    return false;
                if (m_wspeed != 0)
                    m_wspeed = (m_wspeed + m_bidx - m_blast) / 2;
                else
                    m_wspeed = m_bidx - m_blast;      // 初回
                m_blast = m_bidx;

                match_size = m_wspeed * match_second / match_interval;
                match_length = m_bufsize - match_size;

                if (match_length < 0)
                    return false;

//                Console.WriteLine("match_length:{0}", match_length);
//                Console.WriteLine("match_size  :{0}", match_size);
                for (i = 0; i < match_length; i++)
                {
                    for (j = 0; j < match_size; j++)
                    {
                        if (m_stream_buf[(m_bidx + j + match_length) & m_bufmask] !=
                            m_stream_buf[(m_bidx + i + j) & m_bufmask])
                        {
                            break;
                        }
                    }
                    //Console.WriteLine("j  :{0}", j);
                    if (j == match_size)
                    {
                        m_loop_start = m_time_buf[(m_bidx + i) & m_bufmask];
                        m_loop_end = m_time_buf[(m_bidx + match_length) & m_bufmask];
                        return true;
                    }
                }
                return false;
            }

            public new Int32 GetLoopStart()
            {
                return m_loop_start;
            }

            public new Int32 GetLoopEnd()
            {
                return m_loop_end;
            }

        }
    }
}
