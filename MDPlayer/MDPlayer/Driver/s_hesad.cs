using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver
{
    public class s_hesad : KMIF_SOUND_DEVICE
    {
        //# include "kmsnddev.h"
        //# include "s_hesad.h"
        //# include "opl/s_deltat.h"

        private const Int32 CPS_SHIFT = 16;
        private const Int32 PCE_VOLUME = 1; //1
        private const Int32 ADPCM_VOLUME = 50;

        public class HESADPCM
        {

            public KMIF_SOUND_DEVICE kmif;
            public KMIF_SOUND_DEVICE deltadev;
            public class common_
            {
                public Int32 mastervolume;
                public Int32 cps;
                public Int32 pt;
            }
            public common_ common=new common_();

            public byte[] pcmbuf = new byte[0x10000];
            public byte[] port = new byte[0x10];
            public byte[] regs = new byte[0x18];
            public UInt32 outfreq;
            public UInt32 freq;
            public UInt16 addr;
            public UInt16 writeptr;
            public UInt16 readptr;
            public sbyte playflag;
            public sbyte repeatflag;
            public Int32 length;
            public Int32 volume;
            public Int32 fadetimer;
            public Int32 fadecount;
        }


        private void HESAdPcmReset(HESADPCM sndp)
        {
            sndp.addr = 0;
            sndp.freq = 0;
            sndp.writeptr = 0;
            sndp.readptr = 0;
            sndp.playflag = 0;
            sndp.repeatflag = 0;
            sndp.length = 0;
            sndp.volume = 0xff;
            sndp.deltadev.write(sndp.deltadev.ctx, 0, 1);
        }

        private void sndsynth(object ctx, Int32[] p)
        {
            HESADPCM sndp = (HESADPCM)ctx;
            Int32[] pbf = new Int32[2];
            pbf[0] = 0; pbf[1] = 0;

            //この時既に、内蔵音源のレンダリングが終了している。
            p[0] = p[0] * PCE_VOLUME;
            p[1] = p[1] * PCE_VOLUME;

            sndp.deltadev.synth(sndp.deltadev.ctx, pbf);

            sndp.common.pt += sndp.common.cps;

            //1ms
            while (sndp.common.pt > 100000)
            {
                sndp.common.pt -= 100000;

                if (sndp.fadecount > 0 && sndp.fadetimer != 0)
                {
                    sndp.fadecount--;
                    sndp.volume = 0xff * sndp.fadecount / sndp.fadetimer;
                }
                if (sndp.fadecount < 0 && sndp.fadetimer != 0)
                {
                    sndp.fadecount++;
                    sndp.volume = 0xff - (0xff * sndp.fadecount / sndp.fadetimer);
                }

            }
            //	if(sndp->common.pt > 500)p[0]+=80000;
            p[0] += (pbf[0] * ADPCM_VOLUME * sndp.volume / 0xff);
            p[1] += (pbf[1] * ADPCM_VOLUME * sndp.volume / 0xff);
        }

        private void sndreset(object ctx, UInt32 clock, UInt32 freq)
        {
            HESADPCM sndp = (HESADPCM)ctx;
            //XMEMSET(&sndp.pcmbuf, 0, sizeof(sndp.pcmbuf));
            sndp.pcmbuf = new byte[0x10000];
            //XMEMSET(&sndp.port, 0, sizeof(sndp.port));
            sndp.port = new byte[0x10];
            HESAdPcmReset(sndp);
            sndp.outfreq = freq;
            sndp.fadetimer = 0;
            sndp.fadecount = 0;
            sndp.common.cps = (Int32)(100000000 / freq);
            sndp.common.pt = 0;
            sndp.volume = 0xff;
            sndp.deltadev.reset(sndp.deltadev.ctx, clock, freq);
            sndp.deltadev.write(sndp.deltadev.ctx, 1, 0);
            sndp.deltadev.write(sndp.deltadev.ctx, 0xb, 0xff);
            //	sndp->deltadev->setinst(sndp->deltadev,0,sndp->pcmbuf,0x100);

        }

        private void sndwrite(object ctx, UInt32 a, UInt32 v)
        {
            HESADPCM sndp = (HESADPCM)ctx;
            sndp.port[a & 15] = (byte)v;
            sndp.regs[a & 15] = (byte)v;
            switch (a & 15)
            {
                case 0x8:
                    // port low
                    sndp.addr &= 0xff00;
                    sndp.addr |= (UInt16)v;
                    break;
                case 0x9:
                    // port high
                    sndp.addr &= 0xff;
                    sndp.addr |= (UInt16)(v << 8);
                    break;
                case 0xA:
                    // write buffer
                    sndp.pcmbuf[sndp.writeptr++] = (byte)v;
                    break;
                case 0xB:
                    // DMA busy?
                    break;
                case 0xC:
                    break;
                case 0xD:
                    if ((v & 0x80) != 0)
                    {
                        // reset
                        HESAdPcmReset(sndp);
                    }
                    if ((v & 0x03) == 0x03)
                    {
                        // set write pointer
                        sndp.writeptr = sndp.addr;
                        sndp.regs[0x10] = (byte)(sndp.writeptr & 0xff);
                        sndp.regs[0x11] = (byte)(sndp.writeptr >> 8);
                    }
                    if ((v & 0x08) != 0)
                    {
                        // set read pointer
                        sndp.readptr = (UInt16)(sndp.addr != 0 ? sndp.addr - 1 : sndp.addr);
                        sndp.regs[0x12] = (byte)(sndp.readptr & 0xff);
                        sndp.regs[0x13] = (byte)(sndp.readptr >> 8);
                    }
                    if ((v & 0x10) != 0)
                    {
                        sndp.length = sndp.addr;
                        sndp.regs[0x14] = (byte)(sndp.length & 0xff);
                        sndp.regs[0x15] = (byte)(sndp.length >> 8);
                    }
                    sndp.repeatflag = (sbyte)(((v & 0x20) == 0x20) ? 1 : 0);
                    sndp.playflag = (sbyte)(((v & 0x40) == 0x40) ? 1 : 0);
                    if (sndp.playflag != 0)
                    {
                        sndp.deltadev.write(sndp.deltadev, 2, (UInt32)(sndp.readptr & 0xff));
                        sndp.deltadev.write(sndp.deltadev, 3, (UInt32)((sndp.readptr >> 8) & 0xff));
                        sndp.deltadev.write(sndp.deltadev, 4, (UInt32)((sndp.length + sndp.readptr) & 0xff));
                        sndp.deltadev.write(sndp.deltadev, 5, (UInt32)(((sndp.length + sndp.readptr) >> 8) & 0xff));
                        sndp.deltadev.write(sndp.deltadev, 0, 1);
                        sndp.deltadev.write(sndp.deltadev, 0, (UInt32)((0x80 | (sndp.repeatflag >> 1))));
                    }
                    break;
                case 0xE:
                    // set freq
                    sndp.freq = 7111 / (16 - (v & 15));
                    sndp.deltadev.write(sndp.deltadev, 0x9, sndp.freq & 0xff);
                    sndp.deltadev.write(sndp.deltadev, 0xa, (sndp.freq >> 8) & 0xff);
                    break;
                case 0xF:
                    // fade out
                    switch (v & 15)
                    {
                        case 0x0:
                        case 0x1:
                        case 0x2:
                        case 0x3:
                        case 0x4:
                        case 0x5:
                        case 0x6:
                        case 0x7:
                            sndp.fadetimer = 0;
                            sndp.fadecount = sndp.fadetimer;
                            sndp.volume = 0xff;
                            break;
                        case 0x8:
                            sndp.fadetimer = -100;
                            sndp.fadecount = sndp.fadetimer;
                            break;
                        case 0xa:
                            sndp.fadetimer = 5000;
                            sndp.fadecount = sndp.fadetimer;
                            break;
                        case 0xc:
                            sndp.fadetimer = -100;
                            sndp.fadecount = sndp.fadetimer;
                            break;
                        case 0xe:
                            sndp.fadetimer = 1500;
                            sndp.fadecount = sndp.fadetimer;
                            break;
                    }

                    break;
            }
        }

        private UInt32 sndread(object ctx, UInt32 a)
        {
            HESADPCM sndp = (HESADPCM)ctx;
            switch (a & 15)
            {
                case 0xa:
                    return sndp.pcmbuf[sndp.readptr++];
                case 0xb:
                    return (UInt32)(sndp.port[0xb] & ~1);
                case 0xc:
                    if (sndp.playflag == 0)
                    {
                        sndp.port[0xc] |= 1;
                        sndp.port[0xc] &= 0xf7;// ~8;
                    }
                    else
                    {
                        sndp.port[0xc] &= 0xfe;// ~1;
                        sndp.port[0xc] |= 8;
                    }
                    return sndp.port[0xc];
                case 0xd:
                    return 0;
                //		case 0xe:
                //		  return sndp->volume;
                default:
                    return 0xff;
            }
        }

        private const Int32 LOG_BITS = 12;

        private void sndvolume(object ctx, Int32 volume)
        {
            HESADPCM sndp = (HESADPCM)ctx;
            volume = (volume << (LOG_BITS - 8)) << 1;
            sndp.common.mastervolume = volume;

            sndp.deltadev.volume(sndp.deltadev, volume);
        }

        private void sndrelease(object ctx)
        {
            HESADPCM sndp = (HESADPCM)ctx;

            sndp.deltadev.release(sndp.deltadev);

            if (sndp != null)
            {
                //XFREE(sndp);
                sndp = null;
            }
        }

        //private void setinst(object ctx, UInt32 n, byte[] p, UInt32 l) { }

        //ここからレジスタビュアー設定
        //static Uint8* regdata;
        //extern Uint32 (* ioview_ioread_DEV_ADPCM) (Uint32 a);
        //static Uint32 ioview_ioread_bf(Uint32 a)
        //{
        //    if (a >= 0x8 && a <= 0x15) return regdata[a]; else return 0x100;
        //}
        //ここまでレジスタビュアー設定

        public KMIF_SOUND_DEVICE HESAdPcmAlloc()
        {
            HESADPCM sndp;
            //sndp = XMALLOC(sizeof(HESADPCM));
            sndp =  new HESADPCM();
            if (sndp == null) return null;
            //XMEMSET(sndp, 0, sizeof(HESADPCM));
            sndp.kmif = new s_hesad();
            sndp.kmif.ctx = sndp;
            sndp.kmif.release = sndrelease;
            sndp.kmif.reset = sndreset;
            sndp.kmif.synth = sndsynth;
            sndp.kmif.volume = sndvolume;
            sndp.kmif.write = sndwrite;
            sndp.kmif.read = sndread;
            sndp.kmif.setinst = setinst;

            //ここからレジスタビュアー設定
            //regdata = sndp.regs;
            //ioview_ioread_DEV_ADPCM = ioview_ioread_bf;
            //ここまでレジスタビュアー設定

            //発声部分
            sndp.deltadev = YMDELTATPCMSoundAlloc(3, sndp.pcmbuf);
            return sndp.kmif;
        }

        private KMIF_SOUND_DEVICE YMDELTATPCMSoundAlloc(UInt32 ymdeltatpcm_type, byte[] pcmbuf)
        {
            UInt32 ram_size;
            s_deltat.YMDELTATPCMSOUND_ sndp;
            switch (ymdeltatpcm_type)
            {
                case 0://                    YMDELTATPCM_TYPE_Y8950:
                    ram_size = 32 * 1024;
                    break;
                case 1://                    YMDELTATPCM_TYPE_YM2608:
                    ram_size = 256 * 1024;
                    break;
                case 3://                    MSM5205:
                    ram_size = 256 * 256;
                    break;
                default:
                    ram_size = 0;
                    break;
            }
            //sndp = XMALLOC(sizeof(YMDELTATPCMSOUND) + ram_size);
            sndp = new s_deltat.YMDELTATPCMSOUND_();
            if (sndp == null) return null;
            sndp.ram_size = ram_size;
            sndp.ymdeltatpcm_type = (byte)ymdeltatpcm_type;
            switch (ymdeltatpcm_type)
            {
                case 0://                    YMDELTATPCM_TYPE_Y8950:
                    sndp.memshift = 2;
                    break;
                case 1://                    YMDELTATPCM_TYPE_YM2608:
                    /* OPNA */
                    sndp.memshift = 6;
                    break;
                case 2://                    YMDELTATPCM_TYPE_YM2610:
                    sndp.memshift = 9;
                    break;
                case 3://                    MSM5205:
                    sndp.memshift = 0;
                    break;
            }
            s_deltat delta = new s_deltat();
            sndp.kmif = delta;
            sndp.kmif.ctx = sndp;
            sndp.kmif.release = delta.sndrelease;
            sndp.kmif.synth = delta.sndsynth;
            sndp.kmif.volume = delta.sndvolume;
            sndp.kmif.reset = delta.sndreset;
            sndp.kmif.write = delta.sndwrite;
            sndp.kmif.read = delta.sndread;
            sndp.kmif.setinst = delta.setinst;
            /* RAM */
            if (pcmbuf != null)
            {
                sndp.rambuf = pcmbuf;
            }
            else
            {
                sndp.rambuf = null;// ram_size != 0 ? (byte[])(sndp + 1) : 0;
            }
            sndp.rammask = ram_size != 0 ? (ram_size - 1) : 0;
            /* ROM */
            sndp.rombuf = null;
            sndp.rommask = 0;
            sndp.logtbl = delta.LogTableAddRef();
            if (sndp.logtbl == null)
            {
                delta.sndrelease(sndp);
                return null;
            }
            //ここからレジスタビュアー設定
            //sndpr = sndp;
            //if (ioview_ioread_DEV_ADPCM == NULL) ioview_ioread_DEV_ADPCM = ioview_ioread_bf;
            //if (ioview_ioread_DEV_ADPCM2 == NULL) ioview_ioread_DEV_ADPCM2 = ioview_ioread_bf2;
            //ここまでレジスタビュアー設定
            return sndp.kmif;
        }

    }
}