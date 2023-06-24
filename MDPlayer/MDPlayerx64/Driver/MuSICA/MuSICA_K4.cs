using Konamiman.Z80dotNet;
using MDPlayer.Driver.MGSDRV;
using System.Text;

namespace MDPlayer.Driver.MuSICA
{
    public class MuSICA_K4 : baseDriver
    {

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

        public GD3 GetGD3Info(byte[] buf, byte[] vcdBuf)
        {
            GD3 ret = new();
            if (buf != null && buf.Length > 8)
            {
                Run(buf, vcdBuf);
                if (bgmBin == null) return null;
                GD3 gd3 = (new Driver.MuSICA.MuSICA()).getGD3Info(bgmBin, 0);
                ret.TrackName = gd3.TrackName;
                ret.TrackNameJ = gd3.TrackNameJ;
                ret.Notes = gd3.Notes;
            }

            return ret;
        }

        public bool Compile(byte[] vgmBuf, byte[] vcdBuf)
        {

            try
            {
                Run(vgmBuf, vcdBuf);
                if (bgmBin == null) return false;
                //File.WriteAllBytes("C:\\Users\\kuma\\Desktop\\test.bgm", bgmBin);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            throw new NotImplementedException();
        }

        private static byte[] kinrou4 = null;
        private static byte DollarCode;
        private Z80Processor z80;
        private Mapper mapper;
        private MSXVDP vdp;
        private static ushort DTAAddress = 0x0080;
        private static ushort FCBAddress = 0x0080;
        private byte[] msdBin = null;
        private byte[] vcdBin = null;
        private byte[] bgmBin = null;
        private static readonly List<char> consoleBuf = new();

        private void Run(byte[] msdBin, byte[] vcdBin)
        {
            this.msdBin = msdBin;
            this.vcdBin = vcdBin;
            this.bgmBin = null;

            var fileName = "KINROU4.COM";
            DollarCode = Encoding.ASCII.GetBytes(new[] { '$' })[0];

            vdp = new MSXVDP();
            z80 = new Z80Processor
            {
                ClockSynchronizer = null,
                AutoStopOnRetWithStackEmpty = true,
                Memory = new MsxMemory(chipRegister, model)
            };
            z80.PortsSpace = new MsxPort(((MsxMemory)z80.Memory).slot, chipRegister, vdp, model);
            z80.BeforeInstructionFetch += Z80OnBeforeInstructionFetch;
            mapper = new Mapper((MapperRAMCartridge)((MsxMemory)z80.Memory).slot.slots[3][1], (MsxMemory)z80.Memory);
            z80.Reset();


            //プログラムの読み込みとメモリへのセット
            kinrou4 ??= File.ReadAllBytes(fileName);
            z80.Memory.SetContents(0x100, kinrou4);
            z80.Registers.PC = 0x100;
            z80.Registers.SP = unchecked((short)0xf380);

            //bool existVCD = false;
            //if(vcdBin != null && vcdBin.Length > 0)  existVCD = true;

            //コマンドライン引数のセット
            byte[] option = Encoding.ASCII.GetBytes(" DUMMY.MSD DUMMY.VCD");
            z80.Memory[0x80] = (byte)option.Length;
            for (int p = 0; p < option.Length; p++) z80.Memory[0x81 + p] = option[p];
            option = Encoding.ASCII.GetBytes(" DUMMY   MSD     DUMMY   VCD");
            for (int p = 0; p < option.Length; p++) z80.Memory[0x5c + p] = option[p];
            z80.Memory[0x5c] = (byte)0x00;
            z80.Memory[0x68] = (byte)0x00;
            z80.Memory[0x69] = (byte)0x00;
            z80.Memory[0x6a] = (byte)0x00;
            z80.Memory[0x6b] = (byte)0x00;
            z80.Memory[0x6c] = (byte)0x00;

            z80.Memory[0x06] = vdp.m;//V9938 base Address m port0/1   read
            z80.Memory[0x07] = vdp.n;//V9938 base Address n port0/1/3 write   port2 read

            z80.Continue();
            ConFlash();

            if (bgmBin == null)
            {
                log.Write("Compile Fail");
                return;
            }

            log.Write("Compile Success. Length = {0:x04}", bgmBin.Length);
        }

        //public string PlayingFileName { get; internal set; }

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
                //log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                CallBIOS(args, z80);
                return;
            }
            else if (z80.Registers.PC == 0x000c)
            {
                log.Write(LogLevel.Trace, "Call RDSLT(0x000c) Reg.A={0:x02} Reg.HL={1:x04}", z80.Registers.A, z80.Registers.HL);

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
                log.Write(LogLevel.Trace, "Call WRSLT(0x0014) Reg.A={0:x02} Reg.HL={1:x04} Reg.E={2:x02}", z80.Registers.A, z80.Registers.HL, z80.Registers.E);
                throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x001c)
            {
                log.Write(LogLevel.Trace, "Call CALSLT(0x001c) Reg.IY={0:x04} Reg.IX={1:x04}", z80.Registers.IY, z80.Registers.IX);
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
                log.Write(LogLevel.Trace, "Call CALLF(0x0030)");
                throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0090)
            {
                log.Write(LogLevel.Trace, "Call GICINI (0090H/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0093)
            {
                log.Write(LogLevel.Trace, "Call WRTPSG (0093H/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0096)
            {
                log.Write(LogLevel.Trace, "Call RDPSG (0096H/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x0138)
            {
                log.Write(LogLevel.Trace, "Call RSLREG(0138H/MAIN)");
                z80.Registers.A = 0x80 | 0x03 | 0x04;
            }
            else if (z80.Registers.PC == 0x013B)
            {
                log.Write(LogLevel.Trace, "Call WSLREG(013BH/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x015C)
            {
                log.Write(LogLevel.Trace, "Call SUBROM(015CH/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x015f)
            {
                log.Write(LogLevel.Trace, "Call EXTROM(015FH/MAIN)");
                //throw new NotImplementedException();
            }
            else if (z80.Registers.PC == 0x4601)
            {
                log.Write(LogLevel.Trace, "JP NEWSTT(0x4601) Reg.HL={0:x04}", z80.Registers.HL);
                string msg = GetASCIIZ(z80, (ushort)z80.Registers.HL);
                log.Write(LogLevel.Trace, "(HL)={0}", msg);
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
            log.Write(LogLevel.Trace, "Reg PC:{0:x04} AF:{1:x04} BC:{2:x04} DE:{3:x04} HL:{4:x04} IX:{5:x04} IY:{6:x04}"
                , z80.Registers.PC
                , z80.Registers.AF, z80.Registers.BC, z80.Registers.DE, z80.Registers.HL
                , z80.Registers.IX, z80.Registers.IY);
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
                    log.Write(LogLevel.Trace, " EXTBIO Unknown type");
                    break;
            }

            z80.ExecuteRet();
        }

        private void EXTBIO_MemoryMapper(BeforeInstructionFetchEventArgs _, IZ80Processor z80, byte function)
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

        private void CallBIOS(BeforeInstructionFetchEventArgs args, IZ80Processor z80)
        {
            byte function = z80.Registers.C;
            byte byteToPrint;
            string msg;
            switch (function)
            {
                case 2:
                    byteToPrint = z80.Registers.E;
                    var charToPrint = Encoding.ASCII.GetString(new[] { byteToPrint })[0];
                    ConWrite(charToPrint.ToString());
                    break;
                case 9:
                    var messageAddress = z80.Registers.DE;
                    var bytesToPrint = new List<byte>();
                    while ((byteToPrint = z80.Memory[messageAddress]) != DollarCode)
                    {
                        bytesToPrint.Add(byteToPrint);
                        messageAddress++;
                    }
                    var stringToPrint = Encoding.ASCII.GetString(bytesToPrint.ToArray());
                    ConWrite(stringToPrint);
                    break;
                case 0x0f:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "File Open FCB Address:{0:x04}", z80.Registers.DE);
                    z80.Registers.A = 0x00;//success
                    FCBAddress = (ushort)z80.Registers.DE;
                    break;
                case 0x10:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "File Close FCB Address:{0:x04}", z80.Registers.DE);
                    z80.Registers.A = 0x00;//success
                    break;
                case 0x16:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "File Create FCB Address:{0:x04}", z80.Registers.DE);
                    z80.Registers.A = 0x00;//success
                    FCBAddress = (ushort)z80.Registers.DE;
                    break;
                case 0x1a:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "DTA Address:{0:x04}", z80.Registers.DE);
                    DTAAddress = (ushort)z80.Registers.DE;
                    break;
                case 0x26:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "Write random block: FCB Adr(DE):{0:x04} Write Record(HL):{1:x04}", z80.Registers.DE, z80.Registers.HL);
                    z80.Registers.A = 0x00;//success
                    FCBAddress = (ushort)z80.Registers.DE;
                    _ = (ushort)(z80.Memory[FCBAddress + 12] + z80.Memory[FCBAddress + 13] * 0x100);//currentBlock
                    ushort recordSize = (ushort)(z80.Memory[FCBAddress + 14] + z80.Memory[FCBAddress + 15] * 0x100);
                    _ = (ushort)(z80.Memory[FCBAddress + 16]
                        + z80.Memory[FCBAddress + 17] * 0x100
                        + z80.Memory[FCBAddress + 18] * 0x100_00
                        + z80.Memory[FCBAddress + 19] * 0x100_00_00
                        );//fileSize
                    bgmBin = z80.Memory.GetContents(DTAAddress, z80.Registers.HL * recordSize);
                    break;
                case 0x27:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "Read random block: FCB Adr(DE):{0:x04} Read Record(HL):{1:x04}", z80.Registers.DE, z80.Registers.HL);
                    ReadRandomBlock(z80);
                    break;
                case 0x62:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.Write(LogLevel.Trace, "_TERM ErrorCode:{0:x02}", z80.Registers.B);
                    args.ExecutionStopper.Stop();
                    return;
                case 0x6b:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    //_GENV
                    //log.Write("_GENV HL:{0:x04} DE:{1:x04} B:{2:x02}", z80.Registers.HL, z80.Registers.DE, z80.Registers.B);
                    msg = GetASCIIZ(z80, (ushort)z80.Registers.HL);
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
                    break;
                case 0x6c:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    //_SENV
                    //log.Write("_SENV HL:{0:x04} DE:{1:x04}", z80.Registers.HL, z80.Registers.DE);
                    //msg = GetASCIIZ(z80, (ushort)z80.Registers.HL);
                    //log.Write("(HL)={0}", msg);
                    //msg = GetASCIIZ(z80, (ushort)z80.Registers.DE);
                    //log.Write("(DE)={0}", msg);

                    z80.Registers.A = 0x00;//Error number
                    break;
                case 0x6f:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    //_DOSVER
                    z80.Registers.BC = 0x0231;//ROM version
                    z80.Registers.DE = 0x0210;//DISK version
                                              //log.Write("_DOSVER ret BC(ROMVer):{0:x04} DE(DISKVer):{1:x04}", z80.Registers.BC, z80.Registers.DE);
                    break;
                default:
                    log.Write(LogLevel.Trace, "Call BDOS(0x0005) Reg.C={0:x02}", z80.Registers.C);
                    log.ForcedWrite("unknown 0x{0:x02}", function);
                    DebugRegisters(z80);
                    break;
            }

            z80.ExecuteRet();
        }

        private void ReadRandomBlock(IZ80Processor z80)
        {
            ushort recordSize = (ushort)(z80.Memory[FCBAddress + 14] + z80.Memory[FCBAddress + 15] * 0x100);
            ushort randomRecord = (ushort)(z80.Memory[FCBAddress + 33]
                + z80.Memory[FCBAddress + 34] * 0x100
                + z80.Memory[FCBAddress + 35] * 0x100_00
                + (recordSize < 64 ? (z80.Memory[FCBAddress + 36] * 0x100_00_00) : 0)
                );

            if (DTAAddress == 0x4000)
            {
                if (msdBin == null)
                {
                    z80.Registers.A = 0xFF;//fail
                    z80.Registers.HL = 0x00;//読み出せたレコードの個数
                }
                else
                {
                    z80.Memory.SetContents(DTAAddress, msdBin, recordSize * randomRecord, msdBin.Length - recordSize * randomRecord);
                    z80.Registers.A = 0x00;//success
                    z80.Registers.HL = 0x00;//読み出せたレコードの個数
                }
            }
            else if (DTAAddress == 0x8000)
            {
                if (vcdBin == null)
                {
                    z80.Registers.A = 0xFF;//fail
                    z80.Registers.HL = 0x00;//読み出せたレコードの個数
                }
                else
                {
                    z80.Memory.SetContents(DTAAddress, vcdBin, recordSize * randomRecord, vcdBin.Length - recordSize * randomRecord);
                    z80.Registers.A = 0x00;//success
                    z80.Registers.HL = 0x00;//読み出せたレコードの個数
                }
            }
        }

        private static void ConWrite(string v)
        {
            foreach (char c in v)
            {
                if (c == '\n')
                {
                    ConFlash();
                    continue;
                }
                consoleBuf.Add(c);
            }
        }

        private static void ConFlash()
        {
            if (consoleBuf.Count <= 0) return;
            string msg = new(consoleBuf.ToArray());

            log.Write("[MuSICA]{0}", msg);
            consoleBuf.Clear();
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

        public byte[] GetBgmBin()
        {
            return bgmBin;
        }
    }
}
