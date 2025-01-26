using MDPlayer;
using MDSound;
using MDSound.np;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MDPlayer
{
    public class gbs : baseDriver
    {
        public byte song;
        public byte songs;
        public MDSound.MDSound.Chip dmg;
        private double rate = 1;// setting.outputDevice.SampleRate;
        private Driver.GBS.IO io;
        private Driver.GBS.Memory memory;
        private Driver.GBS.CPU cpu;
        private int GBClock = 4194304;
        private double GBVSync = 60.0;
        private double cycles = 0.0;
        private double vcycles = 0.0;
        private gbsInfo info;
        private ushort breakSp;
        private bool initFlg = false;

        public gbs(Setting setting)
        {
            this.setting = setting;
            rate = setting.outputDevice.SampleRate;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            gbsInfo gbsInfo = GetGbsInfo(buf);

            songs = gbsInfo.nums;
            GD3 gd3 = new GD3();
            gd3.GameName = gbsInfo.Title;
            gd3.GameNameJ = gbsInfo.Title;
            gd3.Composer = gbsInfo.Author;
            gd3.ComposerJ = gbsInfo.Author;
            gd3.TrackName = gbsInfo.Title;
            gd3.TrackNameJ = gbsInfo.Title;
            gd3.SystemName = gbsInfo.Copyright;
            gd3.SystemNameJ = gbsInfo.Copyright;
            
            this.GD3 = gd3;

            return gd3;
        }


        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            getGD3Info(vgmBuf, 0);
            info = GetGbsInfo(vgmBuf);
            this.chipRegister = chipRegister;
            this.model = model;

            //Console.WriteLine("Load " + fn);
            //Console.WriteLine("");
            //Console.WriteLine("Title     : {0}", info.Title);
            //Console.WriteLine("Author    : {0}", info.Author);
            //Console.WriteLine("Copyright : {0}", info.Copyright);
            initFlg = true;

            return true;
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            if (initFlg)
            {
                initFlg = false;
                io = new(chipRegister, (MDSound.gb)dmg.Instrument, model);
                memory = new(info.mem, io);
                cpu = new(GBClock, memory);
                cpu.vgmFrameCounter = vgmFrameCounter;
                cpu.Init();

                cpu.reg.pc = info.initAddress;
                cpu.reg.sp = info.sp;
                breakSp = (ushort)(info.sp + 2);
                cpu.reg.a = song;
                try
                {
                    while (!cpu.isHalt && !cpu.isStop)
                    {
                        cpu.ExecuteOneStep();
                        if (cpu.reg.sp == breakSp) break;
                    }
                }
                catch (Exception e)
                {
                    if (e is NotImplementedException)
                        Console.WriteLine("未実装命令きた！");
                }
                cpu.reg.pc = info.playAddress;
                cpu.reg.sp = info.sp;
                cycles = 0.0;
            }

            double oneVClock = GBVSync / (double)setting.outputDevice.SampleRate;
            vcycles += oneVClock;
            if (vcycles >= 1.0)
            {
                vcycles -= 1.0;
                if (cpu.reg.sp == breakSp)
                {
                    cpu.reg.pc = info.playAddress;
                    cpu.reg.sp = info.sp;
                    cycles = 0;
                    //cpu.reg.a = (byte)(info.firstSong - 1);
                }
            }


#if DEBUG
            //Console.WriteLine("");
            //Console.WriteLine("exec Play.");
            //Console.WriteLine("");
            //Console.WriteLine(" pc:{0:X04}", cpu.reg.pc);
            //Console.WriteLine(" sp:{0:X04}", cpu.reg.sp);
            //Console.WriteLine(" firstSong:{0}", cpu.reg.a + 1);
            //Console.WriteLine("");
#endif

            //step = 0;
            try
            {
                if (cpu.reg.sp != breakSp)
                {
                    cpu.vgmFrameCounter = vgmFrameCounter;
                    double oneClock = cpu.clock / (double)setting.outputDevice.SampleRate;
                    while (cycles < oneClock && cpu.reg.sp != breakSp)
                    {
                        int cycle = cpu.ExecuteOneStep();
                        cycles += cycle;
                        //step++;
                    }
                    cycles -= oneClock;
                }
            }
            catch (Exception e)
            {
                if (e is NotImplementedException)
                    Console.WriteLine("未実装命令きた！");
            }
            //Console.WriteLine("Total cycle : {0}", cycles);
            //Console.WriteLine("Total step  : {0}", step);

            //Console.WriteLine("Play process count : {0}", i + 1);


            return;
        }



        gbsInfo GetGbsInfo(byte[] b)
        {
            //IdentifierCheck
            if (b[0] != 'G' || b[1] != 'B' || b[2] != 'S') throw new ArgumentOutOfRangeException("Unknown format");

            gbsInfo info = new gbsInfo();
            info.version = b[3];
            info.nums = b[4];
            info.firstSong = b[5];
            info.loadAddress = (ushort)(b[6] + (b[7] << 8));
            info.initAddress = (ushort)(b[8] + (b[9] << 8));
            info.playAddress = (ushort)(b[10] + (b[11] << 8));
            info.sp = (ushort)(b[12] + (b[13] << 8));
            info.timerModulo = b[14];
            info.timerControl = b[15];
            info.Title = System.Text.Encoding.GetEncoding("shift_jis").GetString(b, 0x10, 32).Replace("\0","");
            info.Author = System.Text.Encoding.GetEncoding("shift_jis").GetString(b, 0x30, 32).Replace("\0", "");
            info.Copyright = System.Text.Encoding.GetEncoding("shift_jis").GetString(b, 0x50, 32).Replace("\0", "");
            info.mem = [new byte[0x4000], new byte[0x4000]];

            int ptr = info.loadAddress % 0x4000;
            int cptr = 0x70;
            int bank = info.loadAddress / 0x4000;
            while ((bank < 2 && ptr < 0x4000) && cptr < b.Length)
            {
                info.mem[bank][ptr] = b[cptr];
                ptr++;
                if (ptr == 0x4000)
                {
                    ptr = 0;
                    bank++;
                }
                cptr++;
            }

            return info;
        }

    }
}
