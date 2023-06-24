using Konamiman.Z80dotNet;

namespace MDPlayer.Driver.MGSDRV
{
    public class Mapper
    {
        private byte freeSegment;
        public ushort TableAddress = 0xf200;//Nextor をまねた
        public ushort JumpAddress = 0xecb2;//Nextor をまねた
        private MapperRAMCartridge crt;

        public Mapper(MapperRAMCartridge crt, MsxMemory memory)
        {
            this.crt = crt;
            crt.ClearUseFlag();
            //freeSegment = (byte)crt.segmentSize;

            for (int i = 0; i < 4; i++) crt.SetSegmentToPage(i, i);
            freeSegment = 4;

            for (int i = 0; i < 16; i++)
            {
                memory[TableAddress + i * 3 + 0] = 0xc3;//JP
                memory[TableAddress + i * 3 + 1] = (byte)(JumpAddress + i);//連番で設定
                memory[TableAddress + i * 3 + 2] = (byte)((JumpAddress + i) >> 8);//
            }
        }

        public void CallMapperProc(BeforeInstructionFetchEventArgs args, IZ80Processor z80, int typ)
        {
            switch (typ)
            {
                case 0://adr
                    //log.Write(" MAPPER PROC ALL_SEG Reg.A={0:x02} Reg.B={1:x02}", z80.Registers.A, z80.Registers.B);
                    if (z80.Registers.B != 0) throw new NotImplementedException();
                    if (freeSegment == 0) { z80.Registers.CF = 1; return; }
                    z80.Registers.A = freeSegment++;//Segment Number 1c 1b
                    //log.Write("   Allocate Reg.A={0:x02} ", z80.Registers.A );
                    z80.Registers.B = 0x00;//Slot number
                    z80.Registers.CF = 0;//割り当て失敗時に1
                    break;
                case 10://adr:0x1e
                    //log.Write(" MAPPER PROC PUT_P1 Reg.A={0:x02}", z80.Registers.A);
                    crt.SetSegmentToPage(z80.Registers.A, 1);
                    break;
                case 11://adr:0x21
                    //log.Write(" MAPPER PROC GET_P1 P1:{0:x02}", crt.GetSegmentNumberFromPageNumber(1));
                    z80.Registers.A = (byte)crt.GetSegmentNumberFromPageNumber(1);
                    break;
                case 12://adr:0x24
                    log.Write(" MAPPER PROC PUT_P2 Reg.A={0:x02}", z80.Registers.A);
                    crt.SetSegmentToPage(z80.Registers.A, 2);
                    break;
                case 13://adr:0x27
                    log.Write(" MAPPER PROC GET_P1 P2:{0:x02}", crt.GetSegmentNumberFromPageNumber(2));
                    z80.Registers.A = (byte)crt.GetSegmentNumberFromPageNumber(2);
                    break;
                default:
                    log.Write(" MAPPER PROC Unknown type");
                    throw new NotImplementedException();
            }

            z80.ExecuteRet();
        }

    }
}
