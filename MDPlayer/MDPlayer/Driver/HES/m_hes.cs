using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDPlayer.Driver;

namespace MDPlayer
{
    public class m_hes
    {
        private kmevent kmevent = new kmevent();
        private km6280 km6280 = new km6280();
        public class NEZ_PLAY
        {
            public HESHES heshes;
            public SONG_INFO song=new SONG_INFO();
        }
        public class SONG_INFO
        {
            public UInt32 songno;
            public UInt32 maxsongno;
            public UInt32 startsongno;
            public UInt32 extdevice;
            public UInt32 initaddress;
            public UInt32 playaddress;
            public UInt32 channel;
            public UInt32 initlimit;
        }
        private songinfodata _songinfodata = new songinfodata();
        private UInt32 GetWordLE(byte[] p, UInt32 ptr)
        {
            return (UInt32)p[ptr + 0] | ((UInt32)p[ptr + 1] << 8);
        }
        private UInt32 GetDwordLE(byte[] p, UInt32 ptr)
        {
            if (p.Length <= ptr+3) return 0;
            return (UInt32)p[ptr + 0] | ((UInt32)p[ptr + 1] << 8) | ((UInt32)p[ptr + 2] << 16) | ((UInt32)p[ptr + 3] << 24);
        }

        public ChipRegister chipRegister;
        public hes.HESDetector ld;

        //# include "neserr.h"
        //# include "handler.h"
        //# include "audiosys.h"
        //# include "songinfo.h"

        //# include "device/s_hes.h"
        //# include "device/s_hesad.h"
        //# include "device/divfix.h"

        //# include "m_hes.h"
        //# include <stdio.h>

        /* -------------------- */
        /*  km6502 HuC6280 I/F  */
        /* -------------------- */
        //#define USE_DIRECT_ZEROPAGE 0
        //#define USE_CALLBACK 1
        //#define USE_INLINEMMC 0
        //#define USE_USERPOINTER 1
        //#define External __inline static
        //# include "kmz80/kmevent.h"
        //# include "km6502/km6280m.h"

        private const UInt32 SHIFT_CPS = 15;
        private const UInt32 HES_BASECYCLES = (21477270);
        private const UInt32 HES_TIMERCYCLES = (1024 * 3);

        //#if 0

        //HES
        //system clock 21477270Hz
        //CPUH clock 21477270Hz system clock 
        //CPUL clock 3579545 system clock / 6

        //FF		I/O
        //F9-FB	SGX-RAM
        //F8		RAM
        //F7		BATTERY RAM
        //80-87	CD-ROM^2 RAM
        //00-		ROM

        //#endif

        //typedef struct HESHES_TAG HESHES;
        public delegate UInt32 READPROC(HESHES THIS_, UInt32 a);
        public delegate void WRITEPROC(HESHES THIS_, UInt32 a, UInt32 v);

        public class HESHES
        {
            public km6280.K6280_Context ctx;
            //public KMIF_SOUND_DEVICE hessnd;
            public KMIF_SOUND_DEVICE hespcm;
            public kmevent.KMEVENT kme=new kmevent.KMEVENT();
            public UInt32 vsync;
            public UInt32 timer;

            public UInt32 bp;          /* break point */
            public UInt32 breaked;     /* break point flag */

            public UInt32 cps;             /* cycles per sample:fixed point */
            public UInt32 cpsrem;          /* cycle remain */
            public UInt32 cpsgap;          /* cycle gap */
            public UInt32 total_cycles;    /* total played cycles */

            public byte[] mpr = new byte[0x8];
            public byte[] firstmpr = new byte[0x8];
            public byte[][] memmap = new byte[0x100][];
            public UInt32 initaddr;

            public UInt32 playerromaddr;
            public byte[] playerrom = new byte[0x10];

            public byte hestim_RELOAD;    /* IO $C01 ($C00)*/
            public byte hestim_COUNTER;   /* IO $C00 */
            public byte hestim_START;     /* IO $C01 */
            public byte hesvdc_STATUS;
            public byte hesvdc_CR;
            public byte hesvdc_ADR;
        };

        public class songinfodata
        {
            public string title;
            public string artist;
            public string copyright;
            public string detail;//[1024];
        }



        private UInt32 km6280_exec(km6280.K6280_Context ctx, UInt32 cycles)
        {
            HESHES THIS_ = (HESHES)ctx.user;
            UInt32 kmecycle;
            kmecycle = ctx.clock = 0;
            while (ctx.clock < cycles)
            {
                //#if 1
                if (THIS_.breaked == 0)
                //#else
                //if (1)
                //#endif
                {

                    //Console.WriteLine("PC:{0:X4} S:{1:X2} SPDAT0x1FF:{2:X2}{3:X2}", 
                    //    THIS_.ctx.PC,THIS_.ctx.S,
                    //    THIS_.memmap[0xf8][0x1ff], THIS_.memmap[0xf8][0x1fe]
                    //    );
                    //K6280_Exec(ctx);    /* Execute 1op */
                    km6280.K_EXEC(ctx);

                    if (ctx.PC == THIS_.bp)
                    {
                        if (((THIS_.ctx.iRequest) & (THIS_.ctx.iMask ^ 0x3) & ((Int32)km6280.K6280_IRQ.K6280_INT1 | (Int32)km6280.K6280_IRQ.K6280_TIMER)) == 0)
                            THIS_.breaked = 1;
                    }
                }
                else
                {
                    UInt32 nextcount;
                    /* break時は次のイベントまで一度に進める */
                    nextcount = THIS_.kme.item[THIS_.kme.item[0].next].count;
                    if (kmevent.kmevent_gettimer(THIS_.kme, 0, ref nextcount) != 0)
                    {
                        /* イベント有り */
                        if (ctx.clock + nextcount < cycles)
                            ctx.clock += nextcount;    /* 期間中にイベント有り */
                        else
                            ctx.clock = cycles;        /* 期間中にイベント無し */
                    }
                    else
                    {
                        /* イベント無し */
                        ctx.clock = cycles;
                    }
                }
                /* イベント進行 */
                kmevent.kmevent_process(THIS_.kme, ctx.clock - kmecycle);
                kmecycle = ctx.clock;
            }
            ctx.clock = 0;
            return kmecycle;
        }

        private Int32 execute(HESHES THIS_)
        {
            UInt32 cycles;
            THIS_.cpsrem += THIS_.cps;
            cycles = THIS_.cpsrem >> (Int32)SHIFT_CPS;
            if (THIS_.cpsgap >= cycles)
                THIS_.cpsgap -= cycles;
            else
            {
                UInt32 excycles = cycles - THIS_.cpsgap;
                THIS_.cpsgap = km6280_exec(THIS_.ctx, excycles) - excycles;
            }
            THIS_.cpsrem &= (1 << (Int32)SHIFT_CPS) - 1;
            THIS_.total_cycles += cycles;

            return 0;
        }

        public void synth(HESHES THIS_, Int32[] d)
        {
            //THIS_.hessnd.synth(THIS_.hessnd.ctx, d);
            THIS_.hespcm.synth(THIS_.hespcm.ctx, d);
        }

        private void volume(HESHES THIS_, UInt32 v)
        {
            //THIS_.hessnd.volume(THIS_.hessnd.ctx, v);
            //THIS_.hespcm.volume(THIS_.hespcm.ctx, v);
        }


        private void vsync_setup(HESHES THIS_)
        {
            kmevent.kmevent_settimer(THIS_.kme, THIS_.vsync, 4 * 342 * 262);
        }

        private void timer_setup(HESHES THIS_)
        {
            kmevent.kmevent_settimer(THIS_.kme, THIS_.timer, HES_TIMERCYCLES);
        }

        private void write_6270(HESHES THIS_, UInt32 a, UInt32 v)
        {
            switch (a)
            {
                case 0:
                    THIS_.hesvdc_ADR = (byte)v;
                    break;
                case 2:
                    switch (THIS_.hesvdc_ADR)
                    {
                        case 5: /* CR */
                            THIS_.hesvdc_CR = (byte)v;
                            break;
                    }
                    break;
                case 3:
                    break;
            }
        }

        private UInt32 read_6270(HESHES THIS_, UInt32 a)
        {
            UInt32 v = 0;
            if (a == 0)
            {
                if (THIS_.hesvdc_STATUS != 0)
                {
                    THIS_.hesvdc_STATUS = 0;
                    v = 0x20;
                }
                THIS_.ctx.iRequest &= 0xFFFFFFDF;// ~km6280.K6280_IRQ.K6280_INT1;
                //#if 0
                //v = 0x20;	/* 常にVSYNC期間 */
                //#endif
            }
            return v;
        }

        private UInt32 read_io(HESHES THIS_, UInt32 a)
        {
            switch (a >> 10)
            {
                case 0: /* VDC */
                    return read_6270(THIS_, a & 3);
                case 2: /* PSG */
                    //return THIS_.hessnd.read(THIS_.hessnd.ctx, a & 0xf);
                    return 0;
                case 3: /* TIMER */
                    if ((a & 1) != 0)
                        return THIS_.hestim_START;
                    else
                        return THIS_.hestim_COUNTER;
                case 5: /* IRQ */
                    switch (a & 15)
                    {
                        case 2:
                            {
                                UInt32 v = 0xf8;
                                if ((THIS_.ctx.iMask & (Int32)km6280.K6280_IRQ.K6280_TIMER) == 0) v |= 4;
                                if ((THIS_.ctx.iMask & (Int32)km6280.K6280_IRQ.K6280_INT1) == 0) v |= 2;
                                if ((THIS_.ctx.iMask & (Int32)km6280.K6280_IRQ.K6280_INT2) == 0) v |= 1;
                                return v;
                            }
                        case 3:
                            {
                                byte v = 0;
                                if ((THIS_.ctx.iRequest & (Int32)km6280.K6280_IRQ.K6280_TIMER) != 0) v |= 4;
                                if ((THIS_.ctx.iRequest & (Int32)km6280.K6280_IRQ.K6280_INT1) != 0) v |= 2;
                                if ((THIS_.ctx.iRequest & (Int32)km6280.K6280_IRQ.K6280_INT2) != 0) v |= 1;
                                //#if 0
                                //THIS_->ctx.iRequest &= ~(K6280_TIMER | K6280_INT1 | K6280_INT2);
                                //#endif
                                return v;
                            }
                    }
                    return 0x00;
                case 7:
                    a -= THIS_.playerromaddr;
                    if (a < 0x10) return THIS_.playerrom[a];
                    return 0xff;
                case 6: /* CDROM */
                    switch (a & 15)
                    {
                        case 0x0a:
                        case 0x0b:
                        case 0x0c:
                        case 0x0d:
                        case 0x0e://デバッグ用
                        case 0x0f://デバッグ用
                            return THIS_.hespcm.read(THIS_.hespcm.ctx, a & 0xf);
                    }
                    return 0xff;
                default:
                case 1: /* VCE */
                case 4: /* PAD */
                    return 0xff;
            }
        }

        public void write_io(HESHES THIS_, UInt32 a, UInt32 v)
        {
            switch (a >> 10)
            {
                case 0: /* VDC */
                    write_6270(THIS_, a & 3, v);
                    break;
                case 2: /* PSG */
                    //Console.WriteLine("Adr:{0:X2} Dat:{1:X2}",
                    //    (Int32)(a & 0xf),
                    //    (Int32)v
                    //    );
                    chipRegister.setHuC6280Register(0, (Int32)(a & 0xf), (Int32)v);
                    ld.Write((UInt32)(a & 0xf), (UInt32)v, 0);
                    //THIS_.hessnd.write(THIS_.hessnd.ctx, a & 0xf, v);
                    break;
                case 3: /* TIMER */
                    switch (a & 1)
                    {
                        case 0:
                            THIS_.hestim_RELOAD = (byte)(v & 127);
                            break;
                        case 1:
                            v &= 1;
                            if (v != 0 && THIS_.hestim_START == 0)
                                THIS_.hestim_COUNTER = THIS_.hestim_RELOAD;
                            THIS_.hestim_START = (byte)v;
                            break;
                    }
                    break;
                case 5: /* IRQ */
                    switch (a & 15)
                    {
                        case 2:
                            THIS_.ctx.iMask &= 0xffffff8f;// ~((Int32)km6280.K6280_IRQ.K6280_TIMER | (Int32)km6280.K6280_IRQ.K6280_INT1 | (Int32)km6280.K6280_IRQ.K6280_INT2);
                            if ((v & 4) == 0) THIS_.ctx.iMask |= (Int32)km6280.K6280_IRQ.K6280_TIMER;
                            if ((v & 2) == 0) THIS_.ctx.iMask |= (Int32)km6280.K6280_IRQ.K6280_INT1;
                            if ((v & 1) == 0) THIS_.ctx.iMask |= (Int32)km6280.K6280_IRQ.K6280_INT2;
                            break;
                        case 3:
                            THIS_.ctx.iRequest &= 0xffffffef;// ~(Int32)km6280.K6280_IRQ.K6280_TIMER;
                            break;
                    }
                    break;
                case 6: /* CDROM */
                    switch (a & 15)
                    {
                        case 0x08:
                        case 0x09:
                        case 0x0a:
                        case 0x0b:
                        case 0x0d:
                        case 0x0e:
                        case 0x0f:
                            THIS_.hespcm.write(THIS_.hespcm.ctx, a & 0xf, v);
                            break;
                    }
                    break;
                default:
                case 1: /* VCE */
                case 4: /* PAD */
                case 7:
                    break;

            }
        }


        public UInt32 read_event(HESHES ctx, UInt32 a)
        {
            HESHES THIS_ = ctx;
            byte page = THIS_.mpr[a >> 13];
            if (THIS_.memmap[page] != null)
                return THIS_.memmap[page][a & 0x1fff];
            else if (page == 0xff)
                return read_io(THIS_, a & 0x1fff);
            else
                return 0xff;
        }

        public void write_event(HESHES ctx, UInt32 a, UInt32 v)
        {
            HESHES THIS_ = ctx;
            byte page = THIS_.mpr[a >> 13];
            if (THIS_.memmap[page] != null)
                THIS_.memmap[page][a & 0x1fff] = (byte)v;
            else if (page == 0xff)
                write_io(THIS_, a & 0x1fff, v);
        }

        public UInt32 readmpr_event(HESHES ctx, UInt32 a)
        {
            HESHES THIS_ = ctx;
            UInt32 i;
            for (i = 0; i < 8; i++) if ((a & (1 << (Int32)i)) != 0) return THIS_.mpr[i];
            return 0xff;
        }

        public void writempr_event(HESHES ctx, UInt32 a, UInt32 v)
        {
            HESHES THIS_ = ctx;
            UInt32 i;
            if (v < 0x80 && THIS_.memmap[v] == null) return;
            for (i = 0; i < 8; i++) if ((a & (1 << (Int32)i)) != 0) THIS_.mpr[i] = (byte)v;
        }

        public void write6270_event(HESHES ctx, UInt32 a, UInt32 v)
        {
            HESHES THIS_ = ctx;
            write_6270(THIS_, a & 0x1fff, v);
        }


        public void vsync_event(kmevent.KMEVENT _event, UInt32 curid, HESHES THIS_)
        {
            vsync_setup(THIS_);
            if ((THIS_.hesvdc_CR & 8) != 0)
            {
                THIS_.ctx.iRequest |= (Int32)km6280.K6280_IRQ.K6280_INT1;
                //Console.WriteLine("vsyncEvent");
                THIS_.breaked = 0;
            }
            THIS_.hesvdc_STATUS = 1;
        }

        private void timer_event(kmevent.KMEVENT _event, UInt32 curid, HESHES THIS_)
        {
            if (THIS_.hestim_START != 0 && THIS_.hestim_COUNTER-- == 0)
            {
                THIS_.hestim_COUNTER = THIS_.hestim_RELOAD;
                THIS_.ctx.iRequest |= (Int32)km6280.K6280_IRQ.K6280_TIMER;
                //Console.WriteLine("timerEvent");
                THIS_.breaked = 0;
            }
            timer_setup(THIS_);
        }

        //ここからメモリービュアー設定
        public delegate UInt32 memview_memread(UInt32 a);
        private HESHES memview_context=null;
        //private Int32 MEM_MAX, MEM_IO, MEM_RAM, MEM_ROM;
        private UInt32 memview_memread_hes(UInt32 a)
        {
            if (a >= 0x1800 && a < 0x1c00 && (a & 0xf) == 0xa) return 0xff;
            return read_event(memview_context, a);
        }
        //ここまでメモリービュアー設定

        //ここからダンプ設定
        //private NEZ_PLAY pNezPlayDump;
        public delegate UInt32 dump_MEM_PCE(UInt32 a, byte[] mem);
        private UInt32 dump_MEM_PCE_bf(UInt32 menu, byte[] mem)
        {
            Int32 i;
            switch (menu)
            {
                case 1://Memory
                    for (i = 0; i < 0x10000; i++)
                        mem[i] = (byte)memview_memread_hes((UInt32)i);
                    return (UInt32)i;
            }
            return (UInt32)0xfffffffe;// (-2);
        }
        //----------
        //extern Uint32 pce_ioview_ioread_bf(Uint32);

        //public delegate UInt32 dump_DEV_HUC6230(UInt32 a, byte[] mem);
        //private UInt32 dump_DEV_HUC6230_bf(UInt32 menu, byte[] mem)
        //{
        //    Int32 i;
        //    switch (menu)
        //    {
        //        case 1://Register 1
        //            for (i = 0; i < 0x0a; i++)
        //                mem[i] = pce_ioview_ioread_bf(i);
        //            return (UInt32)i;

        //        case 2://Register 2
        //            for (i = 0; i < 0x60; i++)
        //                mem[i] = pce_ioview_ioread_bf(i + 0x22);
        //            return (UInt32)i;

        //        case 3://Wave Data - CH1
        //        case 4://Wave Data - CH2
        //        case 5://Wave Data - CH3
        //        case 6://Wave Data - CH4
        //        case 7://Wave Data - CH5
        //        case 8://Wave Data - CH6
        //            for (i = 0; i < 0x20; i++)
        //                mem[i] = pce_ioview_ioread_bf(i + 0x100 + (menu - 3) * 0x20);
        //            return (UInt32)i;
        //    }
        //    return (UInt32)0xfffffffe;// (-2);
        //}
        //----------
        //extern Uint32 (* ioview_ioread_DEV_ADPCM) (Uint32 a);
        //extern Uint32 (* ioview_ioread_DEV_ADPCM2) (Uint32 a);

        //public delegate UInt32 dump_DEV_ADPCM(UInt32 a, byte[] mem);
        //private UInt32 dump_DEV_ADPCM_bf(UInt32 menu, byte[] mem)
        //{
        //    Int32 i;
        //    switch (menu)
        //    {
        //        case 1://Register 1
        //            for (i = 0; i < 0x8; i++)
        //                mem[i] = ioview_ioread_DEV_ADPCM(i + 8);
        //            return (UInt32)i;
        //        case 2://Register 2[ADR/LEN]
        //            for (i = 0; i < 0x6; i++)
        //                mem[i] = ioview_ioread_DEV_ADPCM(i + 0x10);
        //            return (UInt32)i;
        //        case 3://Memory
        //            for (i = 0; i < 0x10000; i++)
        //            {
        //                if (ioview_ioread_DEV_ADPCM2(i) == 0x100) break;
        //                mem[i] = ioview_ioread_DEV_ADPCM2(i);
        //            }
        //            return (UInt32)i;

        //    }
        //    return (UInt32)0xfffffffe;// (-2);
        //}
        //----------

        private void reset(NEZ_PLAY pNezPlay)
        {
            HESHES THIS_ = pNezPlay.heshes;
            UInt32 i, initbreak;
            //UInt32 freq = NESAudioFrequencyGet(pNezPlay);
            UInt32 freq = 44100;

            //THIS_.hessnd.reset(THIS_.hessnd.ctx, HES_BASECYCLES, freq);
            THIS_.hespcm.reset(THIS_.hespcm.ctx, HES_BASECYCLES, freq);
            kmevent.kmevent_init(THIS_.kme);

            /* RAM CLEAR */
            for (i = 0xf8; i <= 0xfb; i++)
                if (THIS_.memmap[i] != null)
                {
                    //XMEMSET(THIS_.memmap[i], 0, 0x2000);
                    THIS_.memmap[i] = new byte[0x2000];
                }

            THIS_.cps = DivFix(HES_BASECYCLES, freq, SHIFT_CPS);
            THIS_.ctx = new km6280.K6280_Context();
            THIS_.ctx.user = THIS_;
            THIS_.ctx.ReadByte = read_event;
            THIS_.ctx.WriteByte = write_event;
            THIS_.ctx.ReadMPR = readmpr_event;
            THIS_.ctx.WriteMPR = writempr_event;
            THIS_.ctx.Write6270 = write6270_event;

            THIS_.vsync = kmevent.kmevent_alloc(THIS_.kme);
            THIS_.timer = kmevent.kmevent_alloc(THIS_.kme);
            kmevent.kmevent_setevent(THIS_.kme, THIS_.vsync, vsync_event, THIS_);
            kmevent.kmevent_setevent(THIS_.kme, THIS_.timer, timer_event, THIS_);

            THIS_.bp = THIS_.playerromaddr + 3;
            for (i = 0; i < 8; i++) THIS_.mpr[i] = THIS_.firstmpr[i];

            THIS_.breaked = 0;
            THIS_.cpsrem = THIS_.cpsgap = THIS_.total_cycles = 0;

            //THIS_.ctx.A = (SONGINFO_GetSongNo(pNezPlay.song) - 1) & 0xff;
            THIS_.ctx.A = (UInt32)((pNezPlay.song.songno - 1) & 0xff);
            //THIS_.ctx.A = (UInt32)((49 - 1) & 0xff);
            THIS_.ctx.P = (UInt32)km6280.K6280_FLAGS.K6280_Z_FLAG + (UInt32)km6280.K6280_FLAGS.K6280_I_FLAG;
            THIS_.ctx.X = THIS_.ctx.Y = 0;
            THIS_.ctx.S = 0xFF;
            THIS_.ctx.PC = THIS_.playerromaddr;
            THIS_.ctx.iRequest = 0;
            THIS_.ctx.iMask = 0xffffffff;// ~0;
            THIS_.ctx.lowClockMode = 0;

            THIS_.playerrom[0x00] = 0x20;  /* JSR */
            THIS_.playerrom[0x01] = (byte)((THIS_.initaddr >> 0) & 0xff);
            THIS_.playerrom[0x02] = (byte)((THIS_.initaddr >> 8) & 0xff);
            THIS_.playerrom[0x03] = 0x4c;  /* JMP */
            THIS_.playerrom[0x04] = (byte)(((THIS_.playerromaddr + 3) >> 0) & 0xff);
            THIS_.playerrom[0x05] = (byte)(((THIS_.playerromaddr + 3) >> 8) & 0xff);

            THIS_.hesvdc_STATUS = 0;
            THIS_.hesvdc_CR = 0;
            THIS_.hesvdc_ADR = 0;
            vsync_setup(THIS_);
            THIS_.hestim_RELOAD = THIS_.hestim_COUNTER = THIS_.hestim_START = 0;
            timer_setup(THIS_);

            /* request execute(5sec) */
            initbreak = 5 << 8;
            while (THIS_.breaked == 0 && --initbreak != 0)
                km6280_exec(THIS_.ctx, HES_BASECYCLES >> 8);

            if (THIS_.breaked != 0)
            {
                THIS_.breaked = 0;
                THIS_.ctx.P &= 0xfffffffb;// ~km6280.K6280_FLAGS.K6280_I_FLAG;
            }

            THIS_.cpsrem = THIS_.cpsgap = THIS_.total_cycles = 0;

            //ここからメモリービュアー設定
            //memview_context = THIS_;
            //MEM_MAX = 0xffff;
            //MEM_IO = 0x0000;
            //MEM_RAM = 0x2000;
            //MEM_ROM = 0x4000;
            //memview_memread = memview_memread_hes;
            //ここまでメモリービュアー設定

            //ここからダンプ設定
            //pNezPlayDump = pNezPlay;
            //dump_MEM_PCE = dump_MEM_PCE_bf;
            //dump_DEV_HUC6230 = dump_DEV_HUC6230_bf;
            //dump_DEV_ADPCM = dump_DEV_ADPCM_bf;
            //ここまでダンプ設定

        }

        private void terminate(HESHES THIS_)
        {
            UInt32 i;

            //ここからダンプ設定
            //dump_MEM_PCE = null;
            //dump_DEV_HUC6230 = null;
            //dump_DEV_ADPCM = null;
            //ここまでダンプ設定

            //if (THIS_.hessnd!=null) THIS_.hessnd.release(THIS_.hessnd.ctx);
            if (THIS_.hespcm != null) THIS_.hespcm.release(THIS_.hespcm.ctx);
            for (i = 0; i < 0x100; i++) if (THIS_.memmap[i] != null) THIS_.memmap = null;// XFREE(THIS_.memmap[i]);
            //XFREE(THIS_);
            THIS_ = null;
        }

        private UInt32 GetWordLE(byte[] p)
        {
            return (UInt32)(p[0] | (p[1] << 8));
        }

        private UInt32 GetDwordLE(byte[] p)
        {
            return (UInt32)(p[0] | (p[1] << 8) | (p[2] << 16) | (p[3] << 24));
        }

        public UInt32 alloc_physical_address(HESHES THIS_, UInt32 a, UInt32 l)
        {
            byte page = (byte)(a >> 13);
            byte lastpage = (byte)((a + l - 1) >> 13);
            for (; page <= lastpage; page++)
            {
                if (THIS_.memmap[page] == null)
                {
                    //THIS_.memmap[page] = (byte[])XMALLOC(0x2000);
                    THIS_.memmap[page] = new byte[0x2000];
                    if (THIS_.memmap[page] == null) return 0;
                    //XMEMSET(THIS_.memmap[page], 0, 0x2000);
                }
            }
            return 1;
        }

        public void copy_physical_address(HESHES THIS_, UInt32 a, UInt32 l, byte[] p,ref UInt32 ptrP)
        {
            byte page = (byte)(a >> 13);
            UInt32 w;
            if ((a & 0x1fff) != 0)
            {
                w = 0x2000 - (a & 0x1fff);
                if (w > l) w = l;
                //XMEMCPY(THIS_.memmap[page++][(a & 0x1fff)], p, w);
                for (int i = 0; i < w; i++)
                {
                    THIS_.memmap[page][(a & 0x1fff) + i] = p[ptrP + i];
                }
                page++;
                //p += w;
                ptrP += w;
                l -= w;
            }
            while (l != 0)
            {
                w = (l > 0x2000) ? 0x2000 : l;
                //XMEMCPY(THIS_.memmap[page++], p, w);
                for (int i = 0; i < w; i++)
                {
                    THIS_.memmap[page][i] = p[ptrP + i];
                }
                page++;
                //p += w;
                ptrP += w;
                l -= w;
            }
        }


        private UInt32 load(NEZ_PLAY pNezPlay, HESHES THIS_, byte[] pData, UInt32 uSize)
        {
            UInt32 i, p;
            //XMEMSET(THIS_, 0, sizeof(HESHES));
            //THIS_ = new HESHES();
            //THIS_.hessnd = 0;
            //THIS_.hespcm = 0;
            for (i = 0; i < 0x100; i++) THIS_.memmap[i] = null;

            if (uSize < 0x20) return (UInt32)NESERR.FORMAT;
            pNezPlay.song.startsongno = (UInt32)(pData[5] + 1);
            pNezPlay.song.songno = (UInt32)256;
            pNezPlay.song.channel = (UInt32)2;
            pNezPlay.song.extdevice = (UInt32)0;
            for (i = 0; i < 8; i++) THIS_.firstmpr[i] = pData[8 + i];
            THIS_.playerromaddr = 0x1ff0;
            THIS_.initaddr = GetWordLE(pData , 0x06);
            pNezPlay.song.initaddress = THIS_.initaddr;
            pNezPlay.song.playaddress = 0;

            _songinfodata.detail = string.Format(
        @"Type           : HES
Start Song: {0:X2}
Init Address: {1:X4}
First Mapper 0 : {0:X2}
First Mapper 1 : {0:X2}
First Mapper 2 : {0:X2}
First Mapper 3 : {0:X2}
First Mapper 4 : {0:X2}
First Mapper 5 : {0:X2}
First Mapper 6 : {0:X2}
First Mapper 7 : {0:X2}"
                , pData[5], THIS_.initaddr
                , pData[0x8]
                , pData[0x9]
                , pData[0xa]
                , pData[0xb]
                , pData[0xc]
                , pData[0xd]
                , pData[0xe]
                , pData[0xf]
                );

            if (alloc_physical_address(THIS_, 0xf8 << 13, 0x2000) == 0) /* RAM */
                return (UInt32)NESERR.SHORTOFMEMORY;
            if (alloc_physical_address(THIS_, 0xf9 << 13, 0x2000) == 0) /* SGX-RAM */
                return (UInt32)NESERR.SHORTOFMEMORY;
            if (alloc_physical_address(THIS_, 0xfa << 13, 0x2000) == 0) /* SGX-RAM */
                return (UInt32)NESERR.SHORTOFMEMORY;
            if (alloc_physical_address(THIS_, 0xfb << 13, 0x2000) == 0) /* SGX-RAM */
                return (UInt32)NESERR.SHORTOFMEMORY;
            if (alloc_physical_address(THIS_, 0x00 << 13, 0x2000) == 0) /* IPL-ROM */
                return (UInt32)NESERR.SHORTOFMEMORY;
            for (p = 0x10; p + 0x10 < uSize; p += 0x10 + GetDwordLE(pData,p + 4))
            {
                if (GetDwordLE(pData , p) == 0x41544144)    /* 'DATA' */
                {
                    UInt32 a, l;
                    l = GetDwordLE(pData , p + 4);
                    a = GetDwordLE(pData , p + 8);
                    if (alloc_physical_address(THIS_, a, l)==0) return (UInt32)NESERR.SHORTOFMEMORY;
                    if (l > uSize - p - 0x10) l = uSize - p - 0x10;
                    UInt32 q = p + 0x10;
                    copy_physical_address(THIS_, a, l, pData,ref q);
                    p = q;
                }
            }
            //THIS_.hessnd = HESSoundAlloc();
            //if (THIS_.hessnd == 0) return NESERR_SHORTOFMEMORY;
            THIS_.hespcm = (new s_hesad()).HESAdPcmAlloc();
            if (THIS_.hespcm == null) return (UInt32)NESERR.SHORTOFMEMORY;

            return (UInt32)NESERR.NOERROR;

        }


        public Int32 ExecuteHES(NEZ_PLAY pNezPlay)
        {
            return ((NEZ_PLAY)pNezPlay).heshes != null ? execute((HESHES)((NEZ_PLAY)pNezPlay).heshes) : 0;
        }

        public void HESSoundRenderStereo(NEZ_PLAY pNezPlay, Int32[] d)
        {
            synth((HESHES)((NEZ_PLAY)pNezPlay).heshes, d);
        }

        public Int32 HESSoundRenderMono(NEZ_PLAY pNezPlay)
        {
            Int32[] d = new Int32[2] { 0, 0 };
            synth((HESHES)((NEZ_PLAY)pNezPlay).heshes, d);
            //#if (((-1) >> 1) == -1)
            //	return (d[0] + d[1]) >> 1;
            //#else
            return (d[0] + d[1]) / 2;
            //#endif
        }

        //private NES_AUDIO_HANDLER[] heshes_audio_handler = new NES_AUDIO_HANDLER[]{
        //    { 0, ExecuteHES, 0, },
        //    { 3, HESSoundRenderMono, HESSoundRenderStereo },
        //    { 0, 0, 0, },
        //};

        private void HESHESVolume(NEZ_PLAY pNezPlay, UInt32 v)
        {
            if (((NEZ_PLAY)pNezPlay).heshes != null)
            {
                volume((HESHES)((NEZ_PLAY)pNezPlay).heshes, v);
            }
        }

        //private NES_VOLUME_HANDLER[] heshes_volume_handler = new NES_VOLUME_HANDLER[]{
        //    { HESHESVolume, },
        //    { 0, },
        //};

        public void HESHESReset(NEZ_PLAY pNezPlay)
        {
            if (((NEZ_PLAY)pNezPlay).heshes != null) reset((NEZ_PLAY)pNezPlay);
        }

        //private NES_RESET_HANDLER[] heshes_reset_handler = new NES_RESET_HANDLER[] {
        //    { NES_RESET_SYS_LAST, HESHESReset, },
        //    { 0,                  0, },
        //};

        private void HESHESTerminate(NEZ_PLAY pNezPlay)
        {
            if (((NEZ_PLAY)pNezPlay).heshes != null)
            {
                terminate((HESHES)((NEZ_PLAY)pNezPlay).heshes);
                ((NEZ_PLAY)pNezPlay).heshes = null;
            }
        }

        //private NES_TERMINATE_HANDLER[] heshes_terminate_handler = new NES_TERMINATE_HANDLER[] {
        //    { HESHESTerminate, },
        //    { 0, },
        //};

        public UInt32 HESLoad(NEZ_PLAY pNezPlay, byte[] pData, UInt32 uSize)
        {
            UInt32 ret;
            HESHES THIS_;
            //if (pNezPlay.heshes!=0) ((byte[])(0)) = 0;    /* ASSERT */
            //THIS_ = (HESHES)XMALLOC(sizeof(HESHES));
            THIS_ = new HESHES();
            if (THIS_ == null) return (UInt32)NESERR.SHORTOFMEMORY;
            ret = load(pNezPlay, THIS_, pData, uSize);
            if (ret != 0)
            {
                terminate(THIS_);
                return ret;
            }
            pNezPlay.heshes = THIS_;
            //NESAudioHandlerInstall(pNezPlay, heshes_audio_handler);
            //NESVolumeHandlerInstall(pNezPlay, heshes_volume_handler);
            //NESResetHandlerInstall(pNezPlay.nrh, heshes_reset_handler);
            //NESTerminateHandlerInstall(pNezPlay.nth, heshes_terminate_handler);
            return ret;
        }

        public enum NESERR :UInt32
        {
            NOERROR,
            SHORTOFMEMORY,
            FORMAT,
            PARAMETER
        }


        private UInt32 DivFix(UInt32 p1, UInt32 p2, UInt32 fix)
        {
            UInt32 ret;
            ret = p1 / p2;
            p1 = p1 % p2;/* p1 = p1 - p2 * ret; */
            while (fix--!=0)
            {
                p1 += p1;
                ret += ret;
                if (p1 >= p2)
                {
                    p1 -= p2;
                    ret++;
                }
            }
            return ret;
        }

    }
}
