using Konamiman.Z80dotNet;
using MDPlayer.Driver.MGSDRV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MuSICA
{
    public class MuSICA : baseDriver
    {

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();
            if (buf != null && buf.Length > 8)
            {
                Run(buf);
                ret.TrackName = GD3.TrackName;
                ret.TrackNameJ = GD3.TrackNameJ;
                ret.Notes = GD3.Notes;
            }

            return ret;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            this.chipRegister = chipRegister;
            LoopCounter = 0;
            vgmCurLoop = 0;
            this.model = model;
            vgmFrameCounter = -latency - waitTime;

            try
            {
                Run(vgmBuf);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            try
            {
                vgmSpeedCounter += (double)Common.VGMProcSampleRate / setting.outputDevice.SampleRate * vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameMain();
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

        private void oneFrameMain()
        {
            try
            {
                Counter++;
                vgmFrameCounter++;

                if (vgmFrameCounter % (Common.VGMProcSampleRate / 60) == 0)
                {
                    interrupt();
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        private void interrupt()
        {
            //log.Write("\r\n_INTER(001FH)");
            z80.Registers.PC = 0x6029;
            z80.Registers.SP = unchecked((short)0xf380);
            z80.Continue();
            //DebugRegisters(z80);

            z80.Registers.PC = 0x6032;
            z80.Registers.SP = unchecked((short)0xf380);
            z80.Continue();

            byte PLAYFG = (byte)(z80.Registers.A & 0x1);
            if (PLAYFG == 0) Stopped = true;
            vgmCurLoop = (uint)z80.Registers.HL;
        }

        private static byte[] program = null;
        private static byte DollarCode;
        private Z80Processor z80;
        private Mapper mapper;
        internal static uint baseclockAY8910 = 1789773;
        internal static uint baseclockYM2413 = 3579545;
        internal static uint baseclockK051649 = 1789773;

        private void Run(byte[] vgmBuf)
        {
            var fileName = "KINROU5.DRV";
            DollarCode = Encoding.ASCII.GetBytes(new[] { '$' })[0];

            z80 = new Z80Processor();
            z80.ClockSynchronizer = null;
            z80.AutoStopOnRetWithStackEmpty = true;
            z80.Memory = new MsxMemory(chipRegister, model);
            z80.PortsSpace = new MsxPort(((MsxMemory)z80.Memory).slot, chipRegister, model);
            z80.BeforeInstructionFetch += Z80OnBeforeInstructionFetch;

            mapper = new Mapper((MapperRAMCartridge)((MsxMemory)z80.Memory).slot.slots[3][1], (MsxMemory)z80.Memory);

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            z80.Reset();


            //プログラムの読み込みとメモリへのセット
            if (program == null) program = File.ReadAllBytes(fileName);
            z80.Memory.SetContents(0x6000 - 7, program);
            z80.Registers.PC = 0x6000;

            log.Write("\r\n_INITAL(6020H)");
            z80.Registers.PC = 0x6020;
            z80.Registers.SP = unchecked((short)0xf380);
            z80.Continue();

            log.Write(string.Format("MSX-MUSIC slot {0:x02}", z80.Memory[0x6010]));
            log.Write(string.Format("SCC       slot {0:x02}", z80.Memory[0x6011]));

            byte[] mgsdata = vgmBuf;
            ushort dataAdr = (ushort)(vgmBuf[1] + vgmBuf[2] * 0x100);
            z80.Memory.SetContents(dataAdr-7, vgmBuf);

            log.Write("\r\n_MPLAY2(6026H)");
            z80.Registers.PC = 0x6026;
            z80.Registers.HL = (short)(dataAdr);
            z80.Registers.DE = (short)(dataAdr);
            z80.Registers.A = 0;//繰り返し回数(0:infinity)
            z80.Registers.SP = unchecked((short)0xf380);
            z80.Continue();
            log.Write(string.Format("MPLAY2 IsSuccess? RegC={0:x02}", z80.Registers.C));
            if (z80.Registers.CF == 0x01)
            {
                DebugRegisters(z80);
                throw new Exception("MPLAY2 Fail");
            }

            uint index;
            //log.Write("\r\n_GETPAR(6047H)");
            z80.Registers.PC = 0x6047;
            z80.Registers.SP = unchecked((short)0xf380);
            z80.Registers.HL = (short)dataAdr;
            z80.Registers.C = 2;//title
            z80.Continue();
            if (z80.Registers.CF == 0)
            {
                index = (uint)(z80.Registers.HL & 0xffff);
                GD3.TrackName = Common.getNRDString(z80.Memory, ref index);
                GD3.TrackNameJ = GD3.TrackName;
            }

            //log.Write("\r\n_GETPAR(6047H)");
            z80.Registers.PC = 0x6047;
            z80.Registers.SP = unchecked((short)0xf380);
            z80.Registers.HL = (short)dataAdr;
            z80.Registers.C = 3;//memo
            z80.Continue();
            if (z80.Registers.CF == 0)
            {
                index = (uint)(z80.Registers.HL & 0xffff);
                GD3.Notes = Common.getNRDString(z80.Memory, ref index);
            }

        }

        public string PlayingFileName { get; internal set; }

        private void Z80OnBeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs args)
        {
            //Absolutely minimum implementation of CP/M for ZEXALL and ZEXDOC to work

            var z80 = (IZ80Processor)sender;

            if (z80.Registers.PC == 0)
            {//0:JP WBOOT
                args.ExecutionStopper.Stop();
                return;
            }
            else if (z80.Registers.PC == 0x0005)
            {
                //log.Write("Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                CallBIOS(args, z80);
                return;
            }
            else if (z80.Registers.PC == 0x000c)
            {
                //log.Write("Call RDSLT(0x000c) Reg.A={0:x02} Reg.HL={1:x04}", z80.Registers.A, z80.Registers.HL);

                int slot = z80.Registers.A & ((z80.Registers.A & 0x80) != 0 ? 0xf : 0x3);
                z80.Registers.A = ((MsxMemory)z80.Memory).ReadSlotMemoryAdr(
                    (slot & 0x03),
                    (slot & 0x0c) >> 2,
                    (ushort)z80.Registers.HL
                    );
                z80.ExecuteRet();
                return;
            }
            else if (z80.Registers.PC == 0x0014)
            {
                log.Write(string.Format("Call WRSLT(0x0014) Reg.A={0:x02} Reg.HL={1:x04} Reg.E={2:x02}", z80.Registers.A, z80.Registers.HL, z80.Registers.E));
                throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x001c)
            {
                log.Write(string.Format("Call CALSLT(0x001c) Reg.IY={0:x04} Reg.IX={1:x04}", z80.Registers.IY, z80.Registers.IX));
                throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0024)
            {
                //log.Write("\r\nCall ENASLT(0x0024) Reg.A={0:x02} Reg.HL={1:x04}", z80.Registers.A, z80.Registers.HL);
                int slot = z80.Registers.A & ((z80.Registers.A & 0x80) != 0 ? 0xf : 0x3);
                ((MsxMemory)z80.Memory).ChangePage(
                    (slot & 0x03)
                    , ((slot & 0x0c) >> 2)
                    , ((z80.Registers.H & 0xc0) >> 6)
                    );
                z80.ExecuteRet();
                return;
            }
            else if (z80.Registers.PC == 0x0030)
            {
                log.Write("Call CALLF(0x0030)");
                throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0090)
            {
                log.Write("Call GICINI (0090H/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0093)
            {
                log.Write("Call WRTPSG (0093H/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0096)
            {
                log.Write("Call RDPSG (0096H/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0138 || z80.Registers.PC == 0x013B || z80.Registers.PC == 0x015C || z80.Registers.PC == 0x015f)
            {
                log.Write("Call InterSlot");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x4601)
            {
                log.Write(string.Format("JP NEWSTT(0x4601) Reg.HL={0:x04}", z80.Registers.HL));
                string msg = GetASCIIZ(z80, (ushort)z80.Registers.HL);
                log.Write(string.Format("(HL)={0}", msg));
                if (msg == ":_SYSTEM")
                {
                    args.ExecutionStopper.Stop();
                }
                return;
            }
            else if (z80.Registers.PC >= mapper.JumpAddress && z80.Registers.PC < mapper.JumpAddress + 16)
            {
                //log.Write("\r\nCall MAPPER PROC(0x{0:x04}～) PC-{0:x04}:{1:x04}", mapper.JumpAddress, z80.Registers.PC - mapper.JumpAddress);
                mapper.CallMapperProc(args, z80, z80.Registers.PC - mapper.JumpAddress);
                return;
            }
            else if (z80.Registers.PC == 0xffca)
            {
                //log.Write("\r\nCall EXTBIO(0xffca) Reg.DE={0:x04}", z80.Registers.DE);
                CallEXTBIO(args, z80);
                return;
            }

            //DebugRegisters(z80);
            ;
            return;
        }

        private static void DebugRegisters(IZ80Processor z80)
        {
            log.Write(string.Format("Reg PC:{0:x04} AF:{1:x04} BC:{2:x04} DE:{3:x04} HL:{4:x04} IX:{5:x04} IY:{6:x04}"
                , z80.Registers.PC
                , z80.Registers.AF, z80.Registers.BC, z80.Registers.DE, z80.Registers.HL
                , z80.Registers.IX, z80.Registers.IY));
        }

        private void CallEXTBIO(BeforeInstructionFetchEventArgs args, IZ80Processor z80)
        {
            byte funcType = z80.Registers.D;
            byte function = z80.Registers.E;

            switch (funcType)
            {
                case 0x04:
                    //log.Write(" EXTBIO MemoryMapper");
                    EXTBIO_MemoryMapper(args, z80, function);
                    break;
                case 0xf0:
                    //MGSDRV向けファンクションコール
                    z80.Registers.A = 0;//非常駐時
                    break;
                default:
                    log.Write(" EXTBIO Unknown type");
                    break;
            }

            z80.ExecuteRet();
        }

        private void EXTBIO_MemoryMapper(BeforeInstructionFetchEventArgs args, IZ80Processor z80, byte function)
        {
            switch (function)
            {
                case 0x02:
                    z80.Registers.A = 0;
                    z80.Registers.BC = 0;
                    z80.Registers.HL = unchecked((short)mapper.TableAddress);
                    break;
            }
        }

        private static void CallBIOS(BeforeInstructionFetchEventArgs args, IZ80Processor z80)
        {
            byte function = z80.Registers.C;

            if (function == 9)
            {
                var messageAddress = z80.Registers.DE;
                var bytesToPrint = new List<byte>();
                byte byteToPrint;
                while ((byteToPrint = z80.Memory[messageAddress]) != DollarCode)
                {
                    bytesToPrint.Add(byteToPrint);
                    messageAddress++;
                }

                var stringToPrint = Encoding.ASCII.GetString(bytesToPrint.ToArray());
                Console.Write(stringToPrint);
            }
            else if (function == 2)
            {
                var byteToPrint = z80.Registers.E;
                var charToPrint = Encoding.ASCII.GetString(new[] { byteToPrint })[0];
                Console.Write(charToPrint);
            }
            else if (function == 0x62)
            {
                //_TERM
                log.Write(string.Format("_TERM ErrorCode:{0:x02}", z80.Registers.B));
                args.ExecutionStopper.Stop();
                return;

            }
            else if (function == 0x6b)
            {
                //_GENV
                //log.Write("_GENV HL:{0:x04} DE:{1:x04} B:{2:x02}", z80.Registers.HL, z80.Registers.DE, z80.Registers.B);
                string msg = GetASCIIZ(z80, (ushort)z80.Registers.HL);
                //log.Write("(HL)={0}", msg);

                if (msg == "PARAMETERS")
                {
                    byte[] option = Encoding.ASCII.GetBytes("/z");
                    for (int i = 0; i < option.Length; i++) z80.Memory[z80.Registers.DE + i] = option[i];
                    z80.Memory[z80.Registers.DE + option.Length] = 0;
                }
                else if (msg == "SHELL")
                {
                    //byte[] option = Encoding.ASCII.GetBytes("c:\\dummy");
                    //for (int i = 0; i < option.Length; i++) z80.Memory[z80.Registers.DE + i] = option[i];
                    //z80.Memory[z80.Registers.DE + option.Length] = 0;
                    z80.Memory[z80.Registers.DE] = 0;
                }
                else
                {
                    z80.Memory[z80.Registers.DE] = 0;
                }

                z80.Registers.A = 0x00;//Error number
                z80.Registers.DE = 0x00;//value

            }
            else if (function == 0x6c)
            {
                //_SENV
                //log.Write("_SENV HL:{0:x04} DE:{1:x04}", z80.Registers.HL, z80.Registers.DE);
                string msg = GetASCIIZ(z80, (ushort)z80.Registers.HL);
                //log.Write("(HL)={0}", msg);

                msg = GetASCIIZ(z80, (ushort)z80.Registers.DE);
                //log.Write("(DE)={0}", msg);

                z80.Registers.A = 0x00;//Error number
            }
            else if (function == 0x6f)
            {
                //_DOSVER
                z80.Registers.BC = 0x0231;//ROM version
                z80.Registers.DE = 0x0210;//DISK version
                //log.Write("_DOSVER ret BC(ROMVer):{0:x04} DE(DISKVer):{1:x04}", z80.Registers.BC, z80.Registers.DE);
            }
            else
            {
                log.Write(string.Format("unknown 0x{0:x02}", function));
                DebugRegisters(z80);
            }

            z80.ExecuteRet();
        }

        private static string GetASCIIZ(IZ80Processor z80, ushort reg)
        {
            var messageAddress = reg;
            var bytesToPrint = new List<byte>();
            byte byteToPrint;
            while ((byteToPrint = z80.Memory[messageAddress]) != 0)
            {
                bytesToPrint.Add(byteToPrint);
                messageAddress++;
            }
            return Encoding.ASCII.GetString(bytesToPrint.ToArray());
        }

    }
}
