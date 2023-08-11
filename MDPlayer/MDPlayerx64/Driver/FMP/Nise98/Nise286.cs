using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class Nise286
    {

        //x86 全般に参考にしたサイト,ソース
        // https://qiita.com/hdk_2/items/6f8cb8a7c67342e2a32a
        // mame tabel286.h

        //OF判定処理　に参考にしたサイト
        //https://hiroyukichishiro.com/arithmetic-overflow-in-c-language/#%E8%B6%B3%E3%81%97%E7%AE%97%E3%81%AE%E4%BA%8B%E5%BE%8C%E6%9D%A1%E4%BB%B6
        // mame


        private Register286 regs;
        private Memory98 mem;
        private NiseDos dos;
        private Nise98 machine;
        private bool segPrefSw = false;
        private int segPref = 0;
        private bool repSW = false;
        private int repType = 0;//0:REPE 1:REPNE
        private bool hltSW = false;
        private List<UserInt> lstUserInt = new List<UserInt>();
        private object userIntLockObject = new object();

        public byte w_mmsk = 0xff;//(IR7 (INT0Fh)-IR0(INT08h))全割り込み不可
        public byte w_smsk = 0xff;//(IR15(INT17h)-IR8(INT10h))全割り込み不可
        public bool[] interruptTrigger = new bool[24];
        public int iLevel = 0;
        private List<Func<bool>> lstHook = new List<Func<bool>>();

        public Nise286(Nise98 machine)
        {
            this.regs = machine.GetRegisters();
            this.mem = machine.GetMem();
            this.dos = machine.GetDos();
            this.machine = machine;
        }

        public void AddUserInt(UserInt ui)
        {
            lock (userIntLockObject)
            {
                lstUserInt.Add(ui);
            }
        }

        public void SetHook(Func<bool> hook)
        {
            lstHook.Add(hook);
        }

        public int StepExecute()
        {
            if (hltSW)
            {
                Log.WriteLine(musicDriverInterface.LogLevel.ERROR, "CPU is HALT.");
                return -1;
            }

            Interrupt();

            foreach (Func<bool> func in lstHook)
            {
                bool did = func();
                if (did) return 0;
            }

            byte op = Fetch();
            switch (op)
            {
                case 0x00: ADD_EB_GB(); break;
                case 0x01: ADD_EW_GW(); break;
                case 0x02: ADD_GB_EB(); break;
                case 0x03: ADD_GW_EW(); break;
                case 0x04: ADD_AL_IB(); break;
                case 0x05: ADD_AX_IW(); break;
                case 0x06: PUSH_ES(); break;
                case 0x07: POP_ES(); break;
                case 0x08: OR_EB_GB(); break;
                case 0x09: throw new NotImplementedException(op.ToString("X02"));
                case 0x0a: OR_GB_EB(); break;
                case 0x0b: OR_GW_EW(); break;
                case 0x0c: OR_AL_IB(); break;
                case 0x0d: OR_AX_IW(); break;
                case 0x0e: PUSH_CS(); break;
                case 0x0f: POP_CS(); break;//x286        

                case 0x10: ADC_EB_GB(); break;
                case 0x11: throw new NotImplementedException(op.ToString("X02"));
                case 0x12: throw new NotImplementedException(op.ToString("X02"));
                case 0x13: ADC_GW_EW(); break;
                case 0x14: ADC_AL_IB(); break;
                case 0x15: throw new NotImplementedException(op.ToString("X02"));
                case 0x16: PUSH_SS(); break;
                case 0x17: POP_SS(); break;
                case 0x18: throw new NotImplementedException(op.ToString("X02"));
                case 0x19: throw new NotImplementedException(op.ToString("X02"));
                case 0x1a: throw new NotImplementedException(op.ToString("X02"));
                case 0x1b: throw new NotImplementedException(op.ToString("X02"));
                case 0x1c: SBB_AL_IB(); break;
                case 0x1d: throw new NotImplementedException(op.ToString("X02"));
                case 0x1e: PUSH_DS(); break;
                case 0x1f: POP_DS(); break;

                case 0x20: AND_EB_GB(); break;
                case 0x21: AND_EW_GW(); break;
                case 0x22: AND_GB_EB(); break;
                case 0x23: AND_GW_EW(); break;
                case 0x24: AND_AL_IB(); break;
                case 0x25: AND_AX_IW(); break;
                case 0x26: ES(); break;
                case 0x27: throw new NotImplementedException(op.ToString("X02"));
                case 0x28: SUB_EB_GB(); break;
                case 0x29: SUB_EW_GW(); break;
                case 0x2a: SUB_GB_EB(); break;
                case 0x2b: SUB_GW_EW(); break;
                case 0x2c: SUB_AL_IB(); break;
                case 0x2d: SUB_AX_IW(); break;
                case 0x2e: CS(); break;
                case 0x2f: DAS(); break;

                case 0x30: XOR_EB_GB(); break;
                case 0x31: throw new NotImplementedException(op.ToString("X02"));
                case 0x32: XOR_GB_EB(); break;
                case 0x33: XOR_GW_EW(); break;
                case 0x34: XOR_AL_IB(); break;
                case 0x35: XOR_AX_IW(); break;
                case 0x36: SS(); break;
                case 0x37: throw new NotImplementedException(op.ToString("X02"));
                case 0x38: CMP_EB_GB(); break;
                case 0x39: CMP_EW_GW(); break;
                case 0x3a: CMP_GB_EB(); break;
                case 0x3b: CMP_GW_EW(); break;
                case 0x3c: CMP_AL_IB(); break;
                case 0x3d: CMP_AX_IW(); break;
                case 0x3e: DS(); break;
                case 0x3f: throw new NotImplementedException(op.ToString("X02"));

                case 0x40: INC_AX(); break;
                case 0x41: INC_CX(); break;
                case 0x42: INC_DX(); break;
                case 0x43: INC_BX(); break;
                case 0x44: INC_SP(); break;
                case 0x45: INC_BP(); break;
                case 0x46: INC_SI(); break;
                case 0x47: INC_DI(); break;
                case 0x48: DEC_AX(); break;
                case 0x49: DEC_CX(); break;
                case 0x4a: DEC_DX(); break;
                case 0x4b: DEC_BX(); break;
                case 0x4c: DEC_SP(); break;
                case 0x4d: DEC_BP(); break;
                case 0x4e: DEC_SI(); break;
                case 0x4f: DEC_DI(); break;

                case 0x50: PUSH_AX(); break;
                case 0x51: PUSH_CX(); break;
                case 0x52: PUSH_DX(); break;
                case 0x53: PUSH_BX(); break;
                case 0x54: PUSH_SP(); break;
                case 0x55: PUSH_BP(); break;
                case 0x56: PUSH_SI(); break;
                case 0x57: PUSH_DI(); break;
                case 0x58: POP_AX(); break;
                case 0x59: POP_CX(); break;
                case 0x5a: POP_DX(); break;
                case 0x5b: POP_BX(); break;
                case 0x5c: POP_SP(); break;
                case 0x5d: POP_BP(); break;
                case 0x5e: POP_SI(); break;
                case 0x5f: POP_DI(); break;

                case 0x60: PUSHA(); break;//x186
                case 0x61: POPA(); break;//x186
                case 0x62: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x63: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0x64: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0x65: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0x66: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0x67: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0x68: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x69: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x6a: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x6b: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x6c: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x6d: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x6e: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0x6f: throw new NotImplementedException(op.ToString("X02"));//x186

                case 0x70: throw new NotImplementedException(op.ToString("X02"));
                case 0x71: throw new NotImplementedException(op.ToString("X02"));
                case 0x72: JB_short(); break;
                case 0x73: JNB_short(); break;
                case 0x74: JZ_short(); break;
                case 0x75: JNZ_short(); break;
                case 0x76: JBE_short(); break;
                case 0x77: JNBE_short(); break;
                case 0x78: JS_short(); break;
                case 0x79: JNS_short(); break;
                case 0x7a: throw new NotImplementedException(op.ToString("X02"));
                case 0x7b: throw new NotImplementedException(op.ToString("X02"));
                case 0x7c: throw new NotImplementedException(op.ToString("X02"));
                case 0x7d: JNL_short(); break;
                case 0x7e: throw new NotImplementedException(op.ToString("X02"));
                case 0x7f: JNLE_short(); break;

                case 0x80: GRP1B(); break;
                case 0x81: GRP1W(); break;
                case 0x82: throw new NotImplementedException(op.ToString("X02"));
                case 0x83: GRP1WB(); break;
                case 0x84: TEST_EB_GB(); break;
                case 0x85: TEST_EW_GW(); break;
                case 0x86: XCHG_EB_GB(); break;
                case 0x87: XCHG_EW_GW(); break;
                case 0x88: MOV_EB_GB(); break;
                case 0x89: MOV_EW_GW(); break;
                case 0x8a: MOV_GB_EB(); break;
                case 0x8b: MOV_GW_EW(); break;
                case 0x8c: MOV_EW_SW(); break;
                case 0x8d: LEA_GW_M(); break;
                case 0x8e: MOV_SW_EW(); break;
                case 0x8f: POP_EW(); break;

                case 0x90: NOP(); break;
                case 0x91: XCHG_CX_AX(); break;
                case 0x92: XCHG_DX_AX(); break;
                case 0x93: XCHG_BX_AX(); break;
                case 0x94: XCHG_SP_AX(); break;
                case 0x95: throw new NotImplementedException(op.ToString("X02"));
                case 0x96: throw new NotImplementedException(op.ToString("X02"));
                case 0x97: throw new NotImplementedException(op.ToString("X02"));
                case 0x98: CBW(); break;
                case 0x99: CWD(); break;
                case 0x9a: throw new NotImplementedException(op.ToString("X02"));
                case 0x9b: throw new NotImplementedException(op.ToString("X02"));
                case 0x9c: PUSHF(); break;
                case 0x9d: POPF(); break;//x286
                case 0x9e: throw new NotImplementedException(op.ToString("X02"));
                case 0x9f: throw new NotImplementedException(op.ToString("X02"));

                case 0xa0: MOV_AL_OB(); break;
                case 0xa1: MOV_AX_OW(); break;
                case 0xa2: MOV_OB_AL(); break;
                case 0xa3: MOV_OW_AX(); break;
                case 0xa4: MOVSB(); break;
                case 0xa5: MOVSW(); break;
                case 0xa6: CMPSB(); break;
                case 0xa7: CMPSW(); break;
                case 0xa8: TEST_AL_IB(); break;
                case 0xa9: throw new NotImplementedException(op.ToString("X02"));
                case 0xaa: STOSB(); break;
                case 0xab: STOSW(); break;
                case 0xac: LODSB(); break;
                case 0xad: LODSW(); break;
                case 0xae: throw new NotImplementedException(op.ToString("X02"));
                case 0xaf: throw new NotImplementedException(op.ToString("X02"));

                case 0xb0: MOV_AL_IB(); break;
                case 0xb1: MOV_CL_IB(); break;
                case 0xb2: MOV_DL_IB(); break;
                case 0xb3: MOV_BL_IB(); break;
                case 0xb4: MOV_AH_IB(); break;
                case 0xb5: MOV_CH_IB(); break;
                case 0xb6: MOV_DH_IB(); break;
                case 0xb7: MOV_BH_IB(); break;
                case 0xb8: MOV_AX_IW(); break;
                case 0xb9: MOV_CX_IW(); break;
                case 0xba: MOV_DX_IW(); break;
                case 0xbb: MOV_BX_IW(); break;
                case 0xbc: MOV_SP_IW(); break;
                case 0xbd: MOV_BP_IW(); break;
                case 0xbe: MOV_SI_IW(); break;
                case 0xbf: MOV_DI_IW(); break;

                case 0xc0: GRP2_EB_CL(op); break; //x186
                case 0xc1: GRP2_EW_CL(op); break; //x186
                case 0xc2: throw new NotImplementedException(op.ToString("X02"));
                case 0xc3: RET(); break;
                case 0xc4: LES_GW_EP(); break;
                case 0xc5: throw new NotImplementedException(op.ToString("X02"));
                case 0xc6: MOV_EB_IB(); break;
                case 0xc7: MOV_EW_IW(); break;
                case 0xc8: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0xc9: throw new NotImplementedException(op.ToString("X02"));//x186
                case 0xca: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xcb: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xcc: throw new NotImplementedException(op.ToString("X02"));
                case 0xcd: INT_IB(); break;
                case 0xce: throw new NotImplementedException(op.ToString("X02"));
                case 0xcf: IRET(); break;//x286

                case 0xd0: GRP2_EB_1(); break;
                case 0xd1: GRP2_EW_1(); break;
                case 0xd2: GRP2_EB_CL(op); break;//x186
                case 0xd3: GRP2_EW_CL(op); break;//x186
                case 0xd4: AAM(); break;
                case 0xd5: throw new NotImplementedException(op.ToString("X02"));
                case 0xd6: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xd7: throw new NotImplementedException(op.ToString("X02"));
                case 0xd8: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xd9: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xda: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xdb: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xdc: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xdd: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xde: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xdf: throw new NotImplementedException(op.ToString("X02"));//x286

                case 0xe0: throw new NotImplementedException(op.ToString("X02"));
                case 0xe1: throw new NotImplementedException(op.ToString("X02"));
                case 0xe2: LOOP_short(); break;
                case 0xe3: throw new NotImplementedException(op.ToString("X02"));
                case 0xe4: IN_AL_IB(); break;
                case 0xe5: IN_AX_IB(); break;
                case 0xe6: OUT_IB_AL(); break;
                case 0xe7: OUT_IB_AX(); break;
                case 0xe8: CALL_near(); break;
                case 0xe9: JMP_near(); break;
                case 0xea: throw new NotImplementedException(op.ToString("X02"));
                case 0xeb: JMP_short(); break;
                case 0xec: IN_AL_DX(); break;
                case 0xed: IN_AX_DX(); break;
                case 0xee: OUT_DX_AL(); break;
                case 0xef: OUT_DX_AX(); break;

                case 0xf0: throw new NotImplementedException(op.ToString("X02"));
                case 0xf1: throw new NotImplementedException(op.ToString("X02"));//x286
                case 0xf2: REPNE(); break;//x186     
                case 0xf3: REPE(); break;//x186      
                case 0xf4: HLT(); break;
                case 0xf5: throw new NotImplementedException(op.ToString("X02"));
                case 0xf6: GRP3B(); break;
                case 0xf7: GRP3W(); break;
                case 0xf8: CLC(); break;
                case 0xf9: STC(); break;
                case 0xfa: CLI(); break;
                case 0xfb: STI(); break;
                case 0xfc: CLD(); break;
                case 0xfd: STD(); break;
                case 0xfe: GRP4(); break;
                case 0xff: GRP5(); break;

                default:
                    throw new NotImplementedException(string.Format("Unkown op code {0:X02}.", op));
            }

            return 0;
        }


        private void Interrupt()
        {
            if (segPrefSw) return;
            if (repSW) return;
            if (!regs.IF) return;

            //check mask
            for (int i = 0; i < 8; i++)
            {
                if ((w_mmsk & (0x01 << i)) == 0) INTxx(i + 8);
            }
            for (int i = 0; i < 8; i++)
            {
                if ((w_smsk & (0x01 << i)) == 0) INTxx(i + 10);
            }

            UserInt();
        }

        private void UserInt()
        {
            if (lstUserInt.Count < 1) return;

            UserInt ui = null;
            lock (userIntLockObject)
            {
                ui = lstUserInt[0];
                lstUserInt.RemoveAt(0);
            }

            Int16 ofs = mem.PeekW(ui.intNum * 4);
            Int16 seg = mem.PeekW(ui.intNum * 4 + 2);
            if (ofs == 0 && seg == 0) return;

            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.FLAG);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, 0);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, 0);
            regs.IP = ofs;
            regs.CS = seg;

            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "Interrupt:UserINT{0:X02}h", ui.intNum);

        }

        private void INTxx(int i)
        {
            if (!interruptTrigger[i]) return;

            interruptTrigger[i] = false;
            Int16 ofs = mem.PeekW(i * 4);
            Int16 seg = mem.PeekW(i * 4 + 2);
            if (ofs == 0 && seg == 0) return;

            iLevel++;

            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.FLAG);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.CS);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.IP);
            regs.IP = ofs;
            regs.CS = seg;

            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "Interrupt:INT{0:X02}h", i);
        }

        private byte Fetch()
        {
            byte op = mem.PeekB(regs.CS_IP);
            regs.IP++;
            return op;
        }

        private UInt16 Fetchw()
        {
            ushort imm16 = Fetch();
            imm16 |= (ushort)(Fetch() << 8);
            return imm16;
        }

        private uint GetSegment(byte rm, bool NoSeg)
        {
            if (NoSeg) return 0;

            if (segPrefSw)
            {
                segPrefSw = false;
                return (uint)((UInt16)regs.sRegs[segPref] << 4);
            }
            else if (rm != 2 && rm != 3)//指定レジスタがBP以外のときはDSをデフォルトのセグメントとして使用する
                return (uint)((UInt16)regs.DS << 4);
            else//指定レジスタがBPのときはSSをデフォルトのセグメントとして使用する
                return (uint)((UInt16)regs.SS << 4);
        }

        private int GetMod00RWADR(byte rm, bool NoSeg = false)
        {
            uint seg = GetSegment(rm, NoSeg);

            switch (rm)
            {
                case 0:
                    return (int)(seg + regs.BX + regs.SI);
                case 1:
                    return (int)(seg + regs.BX + regs.DI);
                case 2:
                    return (int)(seg + regs.BP + regs.SI);
                case 3:
                    return (int)(seg + regs.BP + regs.DI);
                case 4:
                    return (int)(seg + regs.SI);
                case 5:
                    return (int)(seg + regs.DI);
                case 6:
                    UInt16 ptr = Fetchw();
                    return (int)(seg + ptr);
                case 7:
                    return (int)(seg + regs.BX);
                default:
                    throw new NotImplementedException();
            }
        }

        private int GetMod01RWADR(byte rm, bool NoSeg = false)
        {
            uint seg = GetSegment(rm, NoSeg);

            sbyte disp8 = (sbyte)Fetch();
            switch (rm)
            {
                case 0:
                    return (int)(seg + (ushort)(regs.BX + regs.SI + disp8));
                case 1:
                    return (int)(seg + (ushort)(regs.BX + regs.DI + disp8));
                case 2:
                    return (int)(seg + (ushort)(regs.BP + regs.SI + disp8));
                case 3:
                    return (int)(seg + (ushort)(regs.BP + regs.DI + disp8));
                case 4:
                    return (int)(seg + (ushort)(regs.SI + disp8));
                case 5:
                    return (int)(seg + (ushort)(regs.DI + disp8));
                case 6:
                    return (int)(seg + (ushort)(regs.BP + disp8));
                case 7:
                    return (int)(seg + (ushort)(regs.BX + disp8));
                default:
                    throw new NotImplementedException();
            }
        }

        private int GetMod02RWADR(byte rm, bool NoSeg = false)
        {
            uint seg = GetSegment(rm, NoSeg);

            short disp16 = (short)Fetchw();
            switch (rm)
            {
                case 0:
                    return (int)(seg + (ushort)(regs.BX + regs.SI + disp16));
                case 1:
                    return (int)(seg + (ushort)(regs.BX + regs.DI + disp16));
                case 2:
                    return (int)(seg + (ushort)(regs.BP + regs.SI + disp16));
                case 3:
                    return (int)(seg + (ushort)(regs.BP + regs.DI + disp16));
                case 4:
                    return (int)(seg + (ushort)(regs.SI + disp16));
                case 5:
                    return (int)(seg + (ushort)(regs.DI + disp16));
                case 6:
                    return (int)(seg + (ushort)(regs.BP + disp16));
                case 7:
                    return (int)(seg + (ushort)(regs.BX + disp16));
                default:
                    throw new NotImplementedException();
            }
        }

        private uint GetSegment()
        {
            uint seg;
            if (segPrefSw)
            {
                seg = (uint)((UInt16)regs.sRegs[segPref] << 4);
                segPrefSw = false;
            }
            else
                seg = (uint)((UInt16)regs.DS << 4);
            return seg;
        }





        // 0x00
        private void ADD_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            ushort c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 3:
                    if (rm < 4) a = (byte)regs.eRegs[rm];
                    else a = (byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    if (rm < 4) regs.eRegs[rm] = (short)((regs.eRegs[rm] & 0xff00) | ic);
                    else regs.eRegs[rm - 4] = (short)((byte)regs.eRegs[rm - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbAdd(a, b, ic);
            regs.SetCFb(c);
            regs.SetAF(a, b, ic);
        }

        // 0x01
        private void ADD_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            ushort a = 0;
            ushort b = 0;
            uint c = 0;
            ushort ic = 0;

            ushort GW = (ushort)regs.eRegs[reg];

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 3:
                    a = (ushort)regs.eRegs[rm];
                    b = GW;
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    regs.eRegs[rm] = (short)ic;
                    break;
            }

            regs.SetSZPFw(ic);
            regs.SetOFwAdd(a, b, ic);
            regs.SetCFw(c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x02
        private void ADD_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            ushort c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                    a = GB;
                    b = mem.PeekB(GetMod00RWADR(rm));
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 1:
                    a = GB;
                    b = mem.PeekB(GetMod01RWADR(rm));
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 2:
                    a = GB;
                    b = mem.PeekB(GetMod02RWADR(rm));
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 3:
                    a = GB;
                    if (rm < 4) b = (byte)regs.eRegs[rm];
                    else b = (byte)(regs.eRegs[rm - 4] >> 8);
                    c = (ushort)(a + b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbAdd(a, b, ic);
            regs.SetCFb(c);
            regs.SetAF(a, b, ic);
        }

        //0x03
        private void ADD_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //Int16 GW = regs.eRegs[reg];
            ushort a = 0;
            ushort b = 0;
            uint c = 0;
            ushort ic = 0;

            ushort GW = (ushort)regs.eRegs[reg];

            switch (mod)
            {
                case 0:
                    a = GW;
                    b = (ushort)mem.PeekW(GetMod00RWADR(rm));
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    regs.eRegs[reg] = (short)c;
                    break;
                case 1:
                    a = GW;
                    b = (ushort)mem.PeekW(GetMod01RWADR(rm));
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    regs.eRegs[reg] = (short)c;
                    break;
                case 2:
                    a = GW;
                    b = (ushort)mem.PeekW(GetMod02RWADR(rm));
                    c = (uint)(a + b);
                    ic = (ushort)c;
                    regs.eRegs[reg] = (short)c;
                    break;
                case 3:
                    a = GW;
                    b = (ushort)regs.eRegs[rm];
                    c = (uint)(a + b);
                    regs.eRegs[reg] = (short)c;
                    break;
            }

            regs.SetSZPFw(ic);
            regs.SetOFwAdd(a, b, ic);
            regs.SetCFw(c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x04
        private void ADD_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD AL,${0:X02}", imm8);

            byte a = 0, b = 0;
            ushort c = 0;
            byte ic;

            a = (byte)regs.AL;
            b = (byte)imm8;
            c = (ushort)(a + b);
            ic = (byte)(c);
            regs.AL = (byte)c;

            regs.SetSZPFb(ic);
            regs.SetOFbAdd(a, b, ic);
            regs.SetCFb(c);
            regs.SetAF(a, b, ic);
        }

        //0x05
        private void ADD_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD AX,${0:X04}", imm16);

            //Int16 GW = regs.eRegs[reg];
            ushort a = 0;
            ushort b = 0;
            uint c = 0;
            ushort ic = 0;
            a = (ushort)regs.AX;
            b = imm16;
            c = (uint)(a + b);
            ic = (ushort)c;
            regs.AX = (short)ic;

            regs.SetSZPFw(ic);
            regs.SetOFwAdd(a, b, ic);
            regs.SetCFw(c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x06
        private void PUSH_ES()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH ES");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.ES);
        }

        // 0x07
        private void POP_ES()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP ES");
            regs.ES = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        // 0x08
        private void OR_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte a = 0;
            sbyte b = 0;
            sbyte c = 0;
            byte ic = 0;

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 3:
                    if (rm < 4) a = (sbyte)(byte)regs.eRegs[rm];
                    else a = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | ic);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x0a
        private void OR_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte a = 0;
            sbyte b = 0;
            sbyte c = 0;
            byte ic = 0;

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                    a = GB;
                    b = (sbyte)mem.PeekB(GetMod00RWADR(rm));
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 1:
                    a = GB;
                    b = (sbyte)mem.PeekB(GetMod01RWADR(rm));
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 2:
                    a = GB;
                    b = (sbyte)mem.PeekB(GetMod02RWADR(rm));
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 3:
                    a = GB;
                    if (rm < 4) b = (sbyte)(byte)regs.eRegs[rm];
                    else b = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    c = (sbyte)(a | b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x0b
        private void OR_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            int a = 0;
            int b = 0;
            int c = 0;
            Int16 ic = 0;
            switch (mod)
            {
                case 0:
                    a = regs.eRegs[reg];
                    b = mem.PeekW(GetMod00RWADR(rm));
                    c = a | b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
                case 1:
                    a = regs.eRegs[reg];
                    b = mem.PeekW(GetMod01RWADR(rm));
                    c = a | b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
                case 2:
                    a = regs.eRegs[reg];
                    b = mem.PeekW(GetMod02RWADR(rm));
                    c = a | b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
                case 3:
                    a = regs.eRegs[reg];
                    b = regs.eRegs[rm];
                    c = a | b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
            }

            regs.SetSZPFw((ushort)c);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x0c
        private void OR_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR AL,${0:X02}", imm8);
            regs.AL |= imm8;

            regs.SetSZPFb(regs.AL);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x0d
        private void OR_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR AX,${0:X04}", imm16);
            regs.AX |= (short)imm16;

            regs.SetSZPFw((ushort)regs.AX);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x0e
        private void PUSH_CS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH CS");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.CS);
        }

        // 0x0f
        private void POP_CS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP CS");
            regs.CS = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        // 0x10
        private void ADC_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADC EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            ushort c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = (ushort)(a + b + (regs.CF ? 1 : 0));
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = (ushort)(a + b + (regs.CF ? 1 : 0));
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = (ushort)(a + b + (regs.CF ? 1 : 0));
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 3:
                    if (rm < 4) a = (byte)regs.eRegs[rm];
                    else a = (byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = (ushort)(a + b + (regs.CF ? 1 : 0));
                    ic = (byte)c;
                    if (rm < 4) regs.eRegs[rm] = (short)((regs.eRegs[rm] & 0xff00) | ic);
                    else regs.eRegs[rm - 4] = (short)((byte)regs.eRegs[rm - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbAdd(a, b, ic);
            regs.SetCFb(c);
            regs.SetAF(a, b, ic);
        }

        //0x13
        private void ADC_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADC GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //Int16 GW = regs.eRegs[reg];
            ushort a = 0;
            ushort b = 0;
            uint c = 0;
            ushort ic = 0;

            ushort GW = (ushort)regs.eRegs[reg];

            switch (mod)
            {
                case 0:
                    a = GW;
                    b = (ushort)mem.PeekW(GetMod00RWADR(rm));
                    c = (uint)(a + b + (regs.CF ? 1 : 0));
                    ic = (ushort)c;
                    regs.eRegs[reg] = (short)c;
                    break;
                case 1:
                    a = GW;
                    b = (ushort)mem.PeekW(GetMod01RWADR(rm));
                    c = (uint)(a + b + (regs.CF ? 1 : 0));
                    ic = (ushort)c;
                    regs.eRegs[reg] = (short)c;
                    break;
                case 2:
                    a = GW;
                    b = (ushort)mem.PeekW(GetMod02RWADR(rm));
                    c = (uint)(a + b + (regs.CF ? 1 : 0));
                    ic = (ushort)c;
                    regs.eRegs[reg] = (short)c;
                    break;
                case 3:
                    a = GW;
                    b = (ushort)regs.eRegs[rm];
                    c = (uint)(a + b + (regs.CF ? 1 : 0));
                    regs.eRegs[reg] = (short)c;
                    break;
            }

            regs.SetSZPFw(ic);
            regs.SetOFwAdd(a, b, ic);
            regs.SetCFw(c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x14
        private void ADC_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADC AL,${0:X02}", imm8);

            byte a = 0, b = 0;
            ushort c = 0;
            byte ic;

            a = (byte)regs.AL;
            b = (byte)imm8;
            c = (ushort)(a + b + (regs.CF ? 1 : 0));
            ic = (byte)(c);
            regs.AL = (byte)c;

            regs.SetSZPFb(ic);
            regs.SetOFbAdd(a, b, ic);
            regs.SetCFb(c);
            regs.SetAF(a, b, ic);
        }

        // 0x16
        private void PUSH_SS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH SS");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.SS);
        }

        // 0x17
        private void POP_SS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP SS");
            regs.SS = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        // 0x1c
        private void SBB_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SBB AL,${0:X02}", imm8);

            byte a = (byte)regs.AL;
            byte b = (byte)(imm8 + (regs.CF ? 1 : 0));
            int c = a - b;
            byte ic = (byte)c;
            regs.AL = ic;

            regs.SetSZPFb(ic);
            regs.SetOFbSub((byte)a, (byte)b, ic);
            regs.SetCFb((ushort)c);
            regs.SetAF((byte)a, (byte)b, ic);
        }

        // 0x1e
        private void PUSH_DS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH DS");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.DS);
        }

        // 0x1f
        private void POP_DS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP DS");
            regs.DS = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        // 0x20
        private void AND_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte a = 0;
            sbyte b = 0;
            sbyte c = 0;
            byte ic = 0;

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);
            int ptr;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 3:
                    if (rm < 4) a = (sbyte)(byte)regs.eRegs[rm];
                    else a = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | ic);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x21
        private void AND_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            ushort a = 0;
            ushort b = 0;
            uint c = 0;
            ushort ic = 0;

            ushort GW;
            GW = (ushort)regs.eRegs[reg];
            int ptr;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a & b);
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a & b);
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a & b);
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 3:
                    a = (ushort)regs.eRegs[rm];
                    b = GW;
                    c = (uint)(a & b);
                    ic = (ushort)c;
                    regs.eRegs[rm] = (short)ic;
                    break;
            }

            regs.SetSZPFw(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x22
        private void AND_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte a = 0;
            sbyte b = 0;
            sbyte c = 0;
            byte ic = 0;

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                    a = GB;
                    b = (sbyte)mem.PeekB(GetMod00RWADR(rm));
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 1:
                    a = GB;
                    b = (sbyte)mem.PeekB(GetMod01RWADR(rm));
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 2:
                    a = GB;
                    b = (sbyte)mem.PeekB(GetMod02RWADR(rm));
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 3:
                    a = GB;
                    if (rm < 4) b = (sbyte)(byte)regs.eRegs[rm];
                    else b = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    c = (sbyte)(a & b);
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x23
        private void AND_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            int a = 0;
            int b = 0;
            int c = 0;
            Int16 ic = 0;
            switch (mod)
            {
                case 0:
                    a = regs.eRegs[reg];
                    b = mem.PeekW(GetMod00RWADR(rm));
                    c = a & b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
                case 1:
                    a = regs.eRegs[reg];
                    b = mem.PeekW(GetMod01RWADR(rm));
                    c = a & b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
                case 2:
                    a = regs.eRegs[reg];
                    b = mem.PeekW(GetMod02RWADR(rm));
                    c = a & b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
                case 3:
                    a = regs.eRegs[reg];
                    b = regs.eRegs[rm];
                    c = a & b;
                    ic = (Int16)c;
                    regs.eRegs[reg] = ic;
                    break;
            }

            regs.SetSZPFw((ushort)ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x24
        private void AND_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND AL,${0:X02}", imm8);
            regs.AL &= imm8;

            regs.SetSZPFb(regs.AL);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x25
        private void AND_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND AX,${0:X04}", imm16);
            regs.AX &= (short)imm16;

            regs.SetSZPFw((ushort)regs.AX);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        //0x26
        private void ES()
        {
            segPrefSw = true;
            segPref = 0;//0=ES
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ES");

        }

        // 0x28
        private void SUB_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            int c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = mem.PeekB(ptr);
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 3:
                    if (rm < 4) a = (byte)regs.eRegs[rm];
                    else a = (byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    if (rm < 4) regs.eRegs[rm] = (short)((regs.eRegs[rm] & 0xff00) | ic);
                    else regs.eRegs[rm - 4] = (short)((byte)regs.eRegs[rm - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbSub((byte)a, (byte)b, ic);
            regs.SetCFb((ushort)c);
            regs.SetAF((byte)a, (byte)b, ic);
        }

        // 0x29
        private void SUB_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            ushort a = 0;
            ushort b = 0;
            int c = 0;
            ushort ic = 0;

            ushort GW = (ushort)regs.eRegs[reg];

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = a - b;
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = a - b;
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = a - b;
                    ic = (ushort)c;
                    mem.PokeW(ptr, (short)ic);
                    break;
                case 3:
                    a = (ushort)regs.eRegs[rm];
                    b = GW;
                    c = a - b;
                    ic = (ushort)c;
                    regs.eRegs[rm] = (short)ic;
                    break;
            }

            regs.SetSZPFw(ic);
            regs.SetOFwSub(a, b, ic);
            regs.SetCFw((uint)c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x2a
        private void SUB_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            int c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                    a = GB;
                    b = mem.PeekB(GetMod00RWADR(rm));
                    c = a - b;
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (short)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (short)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 1:
                    a = GB;
                    b = mem.PeekB(GetMod01RWADR(rm));
                    c = a - b;
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (short)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (short)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 2:
                    a = GB;
                    b = mem.PeekB(GetMod02RWADR(rm));
                    c = a - b;
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (short)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (short)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
                case 3:
                    a = GB;
                    if (rm < 4) b = (byte)regs.eRegs[rm];
                    else b = (byte)(regs.eRegs[rm - 4] >> 8);
                    c = a - b;
                    ic = (byte)c;
                    if (reg < 4) regs.eRegs[reg] = (short)((regs.eRegs[reg] & 0xff00) | ic);
                    else regs.eRegs[reg - 4] = (short)((byte)regs.eRegs[reg - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbSub((byte)a, (byte)b, ic);
            regs.SetCFb((ushort)c);
            regs.SetAF((byte)a, (byte)b, ic);
        }

        // 0x2b
        private void SUB_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            ushort a = 0;
            ushort b = 0;
            int c = 0;
            ushort ic = 0;
            a = (ushort)regs.eRegs[reg];
            switch (mod)
            {
                case 0:
                    b = (ushort)mem.PeekW(GetMod00RWADR(rm));
                    break;
                case 1:
                    b = (ushort)mem.PeekW(GetMod01RWADR(rm));
                    break;
                case 2:
                    b = (ushort)mem.PeekW(GetMod02RWADR(rm));
                    break;
                case 3:
                    b = (ushort)regs.eRegs[rm];
                    break;
            }
            c = a - b;
            ic = (ushort)c;
            regs.eRegs[reg] = (short)ic;

            regs.SetSZPFw(ic);
            regs.SetOFwSub(a, b, ic);
            regs.SetCFw((uint)c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x2c
        private void SUB_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB AL,${0:X02}", imm8);

            byte a = (byte)regs.AL;
            byte b = (byte)imm8;
            int c = a - b;
            byte ic = (byte)c;
            regs.AL = ic;

            regs.SetSZPFb(ic);
            regs.SetOFbSub((byte)a, (byte)b, ic);
            regs.SetCFb((ushort)c);
            regs.SetAF((byte)a, (byte)b, ic);
        }

        //0x2d
        private void SUB_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB AX,${0:X04}", imm16);

            //Int16 GW = regs.eRegs[reg];
            ushort a = 0;
            ushort b = 0;
            int c = 0;
            ushort ic = 0;
            a = (ushort)regs.AX;
            b = imm16;
            c = a - b;
            ic = (ushort)c;
            regs.AX = (short)ic;

            regs.SetSZPFw(ic);
            regs.SetOFwSub(a, b, ic);
            regs.SetCFw((uint)c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        //0x2e
        private void CS()
        {
            segPrefSw = true;
            segPref = 1;//1=CS
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CS");

        }

        private void DAS()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DAS");
            //TBD
        }

        // 0x30
        private void XOR_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte a = 0;
            sbyte b = 0;
            sbyte c = 0;
            byte ic = 0;

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);
            int ptr;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a ^ b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a ^ b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (sbyte)mem.PeekB(ptr);
                    b = GB;
                    c = (sbyte)(a ^ b);
                    ic = (byte)c;
                    mem.PokeB(ptr, ic);
                    break;
                case 3:
                    if (rm < 4) a = (sbyte)(byte)regs.eRegs[rm];
                    else a = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = (sbyte)(a ^ b);
                    ic = (byte)c;
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | ic);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ic << 8));
                    break;
            }

            regs.SetSZPFb(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x32
        private void XOR_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //Int16 GW = regs.eRegs[reg];
            sbyte a = 0;
            sbyte b = 0;
            sbyte c = 0;
            byte ic = 0;

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);

            a = GB;
            switch (mod)
            {
                case 0:
                    b = (sbyte)mem.PeekB(GetMod00RWADR(rm));
                    break;
                case 1:
                    b = (sbyte)mem.PeekB(GetMod01RWADR(rm));
                    break;
                case 2:
                    b = (sbyte)mem.PeekB(GetMod02RWADR(rm));
                    break;
                case 3:
                    if (rm < 4) b = (sbyte)(byte)regs.eRegs[rm];
                    else b = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }
            c = (sbyte)(a ^ b);
            ic = (byte)c;
            if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | ic);
            else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (ic << 8));

            regs.SetSZPFb(ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x33
        private void XOR_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //Int16 GW = regs.eRegs[reg];
            int a = 0;
            int b = 0;
            int c = 0;
            Int16 ic = 0;
            a = regs.eRegs[reg];
            switch (mod)
            {
                case 0:
                    b = mem.PeekW(GetMod00RWADR(rm));
                    break;
                case 1:
                    b = mem.PeekW(GetMod01RWADR(rm));
                    break;
                case 2:
                    b = mem.PeekW(GetMod02RWADR(rm));
                    break;
                case 3:
                    b = regs.eRegs[rm];
                    break;
            }
            c = a ^ b;
            ic = (Int16)c;
            regs.eRegs[reg] = ic;

            regs.SetSZPFw((ushort)ic);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x34
        private void XOR_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR AL,${0:X02}", imm8);
            regs.AL ^= imm8;

            regs.SetSZPFb(regs.AL);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        // 0x35
        private void XOR_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR AX,${0:X04}", imm16);
            regs.AX ^= (short)imm16;

            regs.SetSZPFw((ushort)regs.AX);
            regs.OF = false;
            regs.CF = false;
            regs.AF = false;//TBD
        }

        //0x36
        private void SS()
        {
            segPrefSw = true;
            segPref = 2;//0=SS
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SS");

        }

        // 0x38
        private void CMP_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            int c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                    a = mem.PeekB(GetMod00RWADR(rm));
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    break;
                case 1:
                    a = mem.PeekB(GetMod01RWADR(rm));
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    break;
                case 2:
                    a = mem.PeekB(GetMod02RWADR(rm));
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    break;
                case 3:
                    if (rm < 4) a = (byte)regs.eRegs[rm];
                    else a = (byte)(regs.eRegs[rm - 4] >> 8);
                    b = GB;
                    c = a - b;
                    ic = (byte)c;
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbSub((byte)a, (byte)b, (byte)c);
            regs.SetCFb((ushort)c);
            regs.SetAF((byte)a, (byte)b, (byte)c);
        }

        // 0x39
        private void CMP_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            ushort a = 0;
            ushort b = 0;
            uint c = 0;
            ushort ic = 0;

            ushort GW = (ushort)regs.eRegs[reg];

            int ptr;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a - b);
                    ic = (ushort)c;
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a - b);
                    ic = (ushort)c;
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    a = (ushort)mem.PeekW(ptr);
                    b = GW;
                    c = (uint)(a - b);
                    ic = (ushort)c;
                    break;
                case 3:
                    a = (ushort)regs.eRegs[rm];
                    b = GW;
                    c = (uint)(a - b);
                    ic = (ushort)c;
                    break;
            }

            regs.SetSZPFw(ic);
            regs.SetOFwSub(a, b, ic);
            regs.SetCFw(c);
            regs.SetAF((byte)a, (byte)b, (byte)ic);
        }

        // 0x3a
        private void CMP_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            byte a = 0;
            byte b = 0;
            int c = 0;
            byte ic = 0;

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                    a = GB;
                    b = mem.PeekB(GetMod00RWADR(rm));
                    c = a - b;
                    ic = (byte)c;
                    break;
                case 1:
                    a = GB;
                    b = mem.PeekB(GetMod01RWADR(rm));
                    c = a - b;
                    ic = (byte)c;
                    break;
                case 2:
                    a = GB;
                    b = mem.PeekB(GetMod02RWADR(rm));
                    c = a - b;
                    ic = (byte)c;
                    break;
                case 3:
                    a = GB;
                    if (rm < 4) b = (byte)regs.eRegs[rm];
                    else b = (byte)(regs.eRegs[rm - 4] >> 8);
                    c = a - b;
                    ic = (byte)c;
                    break;
            }

            regs.SetSZPFb(ic);
            regs.SetOFbSub((byte)a, (byte)b, (byte)c);
            regs.SetCFb((ushort)c);
            regs.SetAF((byte)a, (byte)b, (byte)c);
        }

        //0x3b
        private void CMP_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //Int16 GW = regs.eRegs[reg];
            ushort a = 0;
            ushort b = 0;
            int c = 0;
            switch (mod)
            {
                case 0:
                    a = (UInt16)regs.eRegs[reg];
                    b = (UInt16)mem.PeekW(GetMod00RWADR(rm));
                    c = a - b;
                    break;
                case 1:
                    a = (UInt16)regs.eRegs[reg];
                    b = (UInt16)mem.PeekW(GetMod01RWADR(rm));
                    c = a - b;
                    break;
                case 2:
                    a = (UInt16)regs.eRegs[reg];
                    b = (UInt16)mem.PeekW(GetMod02RWADR(rm));
                    c = a - b;
                    break;
                case 3:
                    a = (UInt16)regs.eRegs[reg];
                    b = (UInt16)regs.eRegs[rm];
                    c = a - b;
                    break;
            }
            ushort ans = (ushort)c;

            regs.SetSZPFw(ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)c);
            regs.SetCFw((uint)c);
            regs.SetAF((byte)a, (byte)b, (byte)c);
        }

        //0x3c
        private void CMP_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP AL,${0:X02}", imm8);

            int ians = (byte)regs.AL - imm8;
            byte ans = (byte)ians;

            regs.SetSZPFb(ans);
            regs.SetOFbSub((byte)regs.AL, (byte)imm8, (byte)ans);
            regs.SetCFb((ushort)ians);
            regs.SetAF((byte)regs.AL, (byte)imm8, (byte)ans);
        }

        // 0x3d
        private void CMP_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP AX,${0:X04}", imm16);
            int ians = (ushort)regs.AX - (ushort)imm16;
            ushort ans = (ushort)ians;

            regs.SetSZPFw(ans);
            regs.SetOFwSub((ushort)regs.AX, (ushort)imm16, (ushort)ans);
            regs.SetCFw((uint)ians);
            regs.SetAF((byte)regs.AX, (byte)imm16, (byte)ans);
        }

        //0x3e
        private void DS()
        {
            segPrefSw = true;
            segPref = 3;//3=DS
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DS");

        }

        // 0x40
        private void INC_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC AX");

            int a = regs.AX;
            int b = 1;
            int ans = a + b;
            regs.AX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x41
        private void INC_CX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC CX");

            int a = regs.CX;
            int b = 1;
            int ans = a + b;
            regs.CX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x42
        private void INC_DX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC DX");

            int a = regs.DX;
            int b = 1;
            int ans = a + b;
            regs.DX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x43
        private void INC_BX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC BX");

            int a = regs.BX;
            int b = 1;
            int ans = a + b;
            regs.BX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x44
        private void INC_SP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC SP");

            int a = regs.SP;
            int b = 1;
            int ans = a + b;
            regs.SP = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x45
        private void INC_BP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC BP");

            int a = regs.BP;
            int b = 1;
            int ans = a + b;
            regs.BP = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x46
        private void INC_SI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC SI");

            int a = regs.SI;
            int b = 1;
            int ans = a + b;
            regs.SI = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x47
        private void INC_DI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC DI");

            int a = regs.DI;
            int b = 1;
            int ans = a + b;
            regs.DI = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwAdd((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x48
        private void DEC_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC AX");

            int a = regs.AX;
            int b = 1;
            int ans = a - b;
            regs.AX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x49
        private void DEC_CX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC CX");

            int a = regs.CX;
            int b = 1;
            int ans = a - b;
            regs.CX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x4a
        private void DEC_DX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC DX");

            int a = regs.DX;
            int b = 1;
            int ans = a - b;
            regs.DX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x4b
        private void DEC_BX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC BX");

            int a = regs.BX;
            int b = 1;
            int ans = a - b;
            regs.BX = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x4c
        private void DEC_SP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC SP");

            int a = regs.SP;
            int b = 1;
            int ans = a - b;
            regs.SP = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x4d
        private void DEC_BP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC BP");

            int a = regs.BP;
            int b = 1;
            int ans = a - b;
            regs.BP = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x4e
        private void DEC_SI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC SI");

            int a = regs.SI;
            int b = 1;
            int ans = a - b;
            regs.SI = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        // 0x4f
        private void DEC_DI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC DI");

            int a = regs.DI;
            int b = 1;
            int ans = a - b;
            regs.DI = (short)ans;

            regs.SetSZPFw((ushort)ans);
            regs.SetOFwSub((ushort)a, (ushort)b, (ushort)ans);
            regs.SetCFw((uint)ans);
            regs.SetAF((byte)a, (byte)b, (byte)ans);
        }

        //0x50
        private void PUSH_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH AX");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.AX);
        }

        //0x51
        private void PUSH_CX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH CX");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.CX);
        }

        //0x52
        private void PUSH_DX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH DX");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.DX);
        }

        //0x53
        private void PUSH_BX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH BX");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.BX);
        }

        //0x54
        private void PUSH_SP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH SP");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.SP);
        }

        //0x55
        private void PUSH_BP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH BP");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.BP);
        }

        //0x56
        private void PUSH_SI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH SI");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.SI);
        }

        //0x57
        private void PUSH_DI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH DI");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.DI);
        }

        //0x58
        private void POP_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP AX");
            regs.AX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x59
        private void POP_CX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP CX");
            regs.CX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x5a
        private void POP_DX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP DX");
            regs.DX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x5b
        private void POP_BX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP BX");
            regs.BX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x5c
        private void POP_SP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP SP");
            regs.SP = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x5d
        private void POP_BP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP BP");
            regs.BP = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x5e
        private void POP_SI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP SI");
            regs.SI = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x5f
        private void POP_DI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP DI");
            regs.DI = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0x60
        private void PUSHA()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSHA");
            short SP = regs.SP;
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.AX);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.CX);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.DX);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.BX);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, SP);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.BP);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.SI);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.DI);
        }

        //0x61
        private void POPA()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POPA");
            short SP;

            regs.DI = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.SI = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.BP = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            SP = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.BX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.DX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.CX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.AX = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.SP = SP;
        }


        //0x72
        private void JB_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JB short:${0:X02}", imm8);

            if (regs.CF)
            {
                regs.IP += imm8;
            }
        }

        //0x73
        private void JNB_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JNB short:${0:X02}", imm8);

            if (!regs.CF)
            {
                regs.IP += imm8;
            }
        }

        //0x74
        private void JZ_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JZ short:${0:X02}", imm8);

            if (regs.ZF)
            {
                regs.IP += imm8;
            }
        }

        //0x75
        private void JNZ_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JNZ short:${0:X02}", imm8);

            if (!regs.ZF)
            {
                regs.IP += imm8;
            }
        }

        //0x76
        private void JBE_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JBE short:${0:X02}", imm8);

            if (regs.CF || regs.ZF)
            {
                regs.IP += imm8;
            }
        }

        //0x77
        private void JNBE_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JNBE short:${0:X02}", imm8);

            if (!regs.CF && !regs.ZF)//cmp then op1<op2
            {
                regs.IP += imm8;
            }
        }

        //0x78
        private void JS_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JS short:${0:X02}", imm8);

            if (regs.SF)
            {
                regs.IP += imm8;
            }
        }

        //0x79
        private void JNS_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JNS short:${0:X02}", imm8);

            if (!regs.SF)
            {
                regs.IP += imm8;
            }
        }

        //0x7d
        private void JNL_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JNL short:${0:X02}", imm8);

            if (regs.SF == regs.OF)
            {
                regs.IP += imm8;
            }
        }

        //0x7f
        private void JNLE_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JNLE short:${0:X02}", imm8);

            if (!regs.ZF && regs.SF == regs.OF)
            {
                regs.IP += imm8;
            }
        }

        //0x80
        private void GRP1B()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            ushort IB;
            int ians;
            sbyte ans;
            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }
            IB = Fetch();

            switch (reg)
            {
                case 0://ADD EB,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD EB,${0:X02}", IB);
                    ians = (byte)EB + (byte)IB;
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ians);
                    regs.SetOFbAdd((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 1://OR EB,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR EB,${0:X02}", IB);
                    ians = EB | (sbyte)IB;
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 2://ADC EB,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADC EB,${0:X02}", IB);
                    ians = EB + IB + (regs.CF ? 1 : 0);
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ians);
                    regs.SetOFbAdd((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 3://SBB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SBB EB,${0:X02}", IB);
                    ians = EB - (IB + (regs.CF ? 1 : 0));
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ians);
                    regs.SetOFbSub((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 4://AND EB,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND EB,${0:X02}", IB);
                    ians = EB & IB;
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 5: //SUB EB,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB EB,${0:X02}", IB);
                    ians = (byte)EB - (byte)IB;
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ians);
                    regs.SetOFbSub((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 6:
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR EB,${0:X02}", IB);
                    ians = EB ^ IB;
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, (byte)ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 7://CMP EB,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP EB,${0:X02}", IB);
                    ians = (byte)EB - (byte)IB;
                    ans = (sbyte)ians;
                    regs.SetSZPFb((byte)ans);
                    regs.SetOFbSub((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    break;
            }
        }

        //0x81
        private void GRP1W()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            ushort IW;
            int ians;
            Int16 ans;
            Int16 EW = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 3:
                    EW = regs.eRegs[rm];
                    break;
            }
            IW = Fetchw();

            switch (reg)
            {
                case 0://ADD EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD EW,${0:X04}", IW);
                    ians = EW + IW;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.SetOFwAdd((ushort)EW, (ushort)IW, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IW, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 1://OR EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR EW,${0:X04}", IW);
                    ians = EW | (short)IW;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 2://ADC EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADC EW,${0:X04}", IW);
                    ians = EW + IW + (regs.CF ? 1 : 0);
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.SetOFwAdd((ushort)EW, (ushort)IW, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IW, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 3:
                    throw new NotImplementedException();
                case 4://AND EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND EW,${0:X04}", IW);
                    ians = EW & IW;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 5://SUB EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB EW,${0:X04}", IW);
                    ians = EW - IW;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.SetOFwSub((ushort)EW, (ushort)IW, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IW, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 6://XOR EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR EW,${0:X04}", IW);
                    ians = EW ^ IW;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 7://CMP EW,IW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP EW,${0:X04}", IW);
                    ians = EW - (short)IW;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ans);
                    regs.SetOFwSub((ushort)EW, (ushort)IW, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IW, (byte)ans);
                    break;
            }
        }

        //0x83
        private void GRP1WB()
        {
            byte modrw = Fetch();

            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            Int16 EW = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 3:
                    EW = regs.eRegs[rm];
                    break;
            }
            byte IB;
            IB = Fetch();
            int ians;
            Int16 ans;

            switch (reg)
            {
                case 0://ADD EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADD EW,${0:X02}", IB);
                    ians = (ushort)EW + (byte)IB;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.SetOFwAdd((ushort)EW, (ushort)IB, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 1://OR EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OR EW,${0:X02}", IB);
                    ians = (ushort)EW | (ushort)IB;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 2://ADC EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ADC EW,${0:X02}", IB);
                    ians = EW + IB + (regs.CF ? 1 : 0);
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.SetOFwAdd((ushort)EW, (ushort)IB, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 3://SBB EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SBB EW,${0:X02}", IB);
                    ians = (ushort)EW - (byte)(IB + (regs.CF ? 1 : 0));
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.SetOFwSub((ushort)EW, (ushort)IB, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 4://AND EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "AND EW,${0:X02}", IB);
                    ians = (ushort)EW & (ushort)IB;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 5://SUB EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SUB EW,${0:X02}", IB);
                    ians = (ushort)EW - IB;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.SetOFwSub((ushort)EW, (ushort)IB, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 6://XOR EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XOR EW,${0:X02}", IB);
                    ians = (ushort)EW ^ (ushort)IB;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ans);
                            break;
                        case 3:
                            regs.eRegs[rm] = ans;
                            break;
                    }
                    break;
                case 7://CMP EW,IB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMP EW,${0:X02}", IB);
                    ians = (ushort)EW - IB;
                    ans = (Int16)ians;
                    regs.SetSZPFw((ushort)ians);
                    regs.SetOFwSub((ushort)EW, (ushort)IB, (ushort)ans);
                    regs.SetCFw((uint)ians);
                    regs.SetAF((byte)EW, (byte)IB, (byte)ans);
                    break;
            }
        }

        //0x84
        private void TEST_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "TEST EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            byte GB;
            if (reg < 4) GB = (byte)regs.eRegs[reg];
            else GB = (byte)(regs.eRegs[reg - 4] >> 8);
            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    byte ans = (byte)(EB & GB);
                    regs.SetSZPFb(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
            }
        }

        // 0x85
        private void TEST_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "TEST EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            short EW = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = mem.PeekW(ptr);
                    break;
                case 3:
                    EW = regs.eRegs[rm];
                    break;
            }

            ushort GW;
            GW = (ushort)regs.eRegs[reg];
            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    ushort ans = (ushort)(EW & GW);
                    regs.SetSZPFw(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
            }

        }

        //0x86
        private void XCHG_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XCHG EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);

            //GB <-> EB
            sbyte p = GB;
            GB = EB;
            EB = p;

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeB(ptr, (byte)EB);
                    break;
                case 3:
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)EB);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (EB << 8));
                    break;
            }

            if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | (byte)GB);
            else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (GB << 8));
        }

        //0x87
        private void XCHG_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XCHG EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            ushort EW = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 3:
                    EW = (ushort)regs.eRegs[rm];
                    break;
            }

            ushort GW;
            GW = (ushort)regs.eRegs[reg];

            //GW <-> EW
            ushort p = GW;
            GW = EW;
            EW = p;

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeW(ptr, (short)EW);
                    break;
                case 3:
                    regs.eRegs[rm] = (short)EW;
                    break;
            }

        }

        //0x88
        private void MOV_EB_GB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EB,GB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    //EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    //EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    //EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    //if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    //else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            sbyte GB;
            if (reg < 4) GB = (sbyte)(byte)regs.eRegs[reg];
            else GB = (sbyte)(byte)(regs.eRegs[reg - 4] >> 8);

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeB(ptr, (byte)GB);
                    break;
                case 3:
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)GB);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (GB << 8));
                    break;
            }
        }

        //0x89
        private void MOV_EW_GW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EW,GW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            Int16 GW = regs.eRegs[reg];
            switch (mod)
            {
                case 0:
                    mem.PokeW(GetMod00RWADR(rm), GW);
                    break;
                case 1:
                    mem.PokeW(GetMod01RWADR(rm), GW);
                    break;
                case 2:
                    mem.PokeW(GetMod02RWADR(rm), GW);
                    break;
                case 3:
                    regs.eRegs[rm] = GW;
                    break;
            }
        }

        //0x8a
        private void MOV_GB_EB()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV GB,EB modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | (byte)EB);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (EB << 8));
                    break;
                case 3:
                    if (reg < 4) regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | (byte)EB);
                    else regs.eRegs[reg - 4] = (Int16)((byte)regs.eRegs[reg - 4] | (EB << 8));
                    break;
            }
        }

        //0x8b
        private void MOV_GW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV GW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            switch (mod)
            {
                case 0:
                    regs.eRegs[reg] = mem.PeekW(GetMod00RWADR(rm));
                    break;
                case 1:
                    regs.eRegs[reg] = mem.PeekW(GetMod01RWADR(rm));
                    break;
                case 2:
                    regs.eRegs[reg] = mem.PeekW(GetMod02RWADR(rm));
                    break;
                case 3:
                    regs.eRegs[reg] = regs.eRegs[rm];
                    break;
            }
        }

        //0x8c
        private void MOV_EW_SW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EW,SW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x18) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            Int16 SW = regs.sRegs[reg];
            switch (mod)
            {
                case 0:
                    mem.PokeW(GetMod00RWADR(rm), SW);
                    break;
                case 1:
                    mem.PokeW(GetMod01RWADR(rm), SW);
                    break;
                case 2:
                    mem.PokeW(GetMod02RWADR(rm), SW);
                    break;
                case 3:
                    regs.eRegs[rm] = SW;
                    break;
            }
        }

        //0x8d
        private void LEA_GW_M()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "LEA GW,M modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            switch (mod)
            {
                case 0:
                    regs.eRegs[reg] = (short)GetMod00RWADR(rm, true);
                    break;
                case 1:
                    regs.eRegs[reg] = (short)GetMod01RWADR(rm, true);
                    break;
                case 2:
                    regs.eRegs[reg] = (short)GetMod02RWADR(rm, true);
                    break;
                case 3:
                    throw new NotImplementedException();
            }
        }

        //0x8e
        private void MOV_SW_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV SW,EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x18) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            switch (mod)
            {
                case 0:
                    regs.sRegs[reg] = mem.PeekW(GetMod00RWADR(rm));
                    break;
                case 1:
                    regs.sRegs[reg] = mem.PeekW(GetMod01RWADR(rm));
                    break;
                case 2:
                    regs.sRegs[reg] = mem.PeekW(GetMod02RWADR(rm));
                    break;
                case 3:
                    Int16 r = regs.eRegs[rm];
                    regs.sRegs[reg] = r;
                    break;
            }
        }

        //0x8f
        private void POP_EW()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POP EW modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x18) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            int ptr;
            short imm16 = mem.PeekW(regs.SS_SP);
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    mem.PokeW(ptr, imm16);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    mem.PokeW(ptr, imm16);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    mem.PokeW(ptr, imm16);
                    break;
                case 3:
                    regs.eRegs[rm] = imm16;
                    break;
            }
            regs.SP += 2;
        }


        //0x84
        private void TEST_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "TEST AL,${0:X02}", imm8);

            byte ans = (byte)(regs.AL & imm8);
            regs.OF = false;
            regs.CF = false;
            regs.ZF = ans == 0;
            regs.SF = (ans & 0x80) != 0;
            regs.PF = (ans & 0x01) != 0;
            regs.AF = false;//TBD
        }

        //0x90
        private void NOP()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "NOP");

        }

        //0x91
        private void XCHG_CX_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XCHG_CX_AX");

            short v = regs.AX;
            regs.AX = regs.CX;
            regs.CX = v;
        }

        //0x92
        private void XCHG_DX_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XCHG_DX_AX");

            short v = regs.AX;
            regs.AX = regs.DX;
            regs.DX = v;
        }

        //0x93
        private void XCHG_BX_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XCHG_BX_AX");

            short v = regs.AX;
            regs.AX = regs.BX;
            regs.BX = v;
        }

        //0x94
        private void XCHG_SP_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "XCHG_SP_AX");

            short v = regs.AX;
            regs.AX = regs.SP;
            regs.SP = v;
        }

        //0x98
        private void CBW()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CBW");
            regs.AH = (byte)((regs.AL & 0x80) != 0 ? 0xff : 0x00);
        }

        //0x99
        private void CWD()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CWD");
            regs.DX = (short)((regs.AX & 0x8000) != 0 ? 0xffff : 0x0000);
        }

        //0x9c
        private void PUSHF()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSHF");
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.FLAG);
        }

        //0x9d
        private void POPF()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "POPF");
            regs.FLAG = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0xa0
        private void MOV_AL_OB()
        {
            uint seg = GetSegment();
            UInt16 ptr = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV AL,[${0:X04}]", ptr);

            regs.AL = mem.PeekB((int)(seg + ptr));
        }

        //0xa1
        private void MOV_AX_OW()
        {
            uint seg = GetSegment();
            UInt16 ptr = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV AX,[${0:X04}]", ptr);

            regs.AX = mem.PeekW((int)(seg + ptr));
        }

        //0xa2
        private void MOV_OB_AL()
        {
            uint seg = GetSegment();
            ushort ptr = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV [${0:X04}],AL", ptr);

            mem.PokeB((int)(seg + ptr), regs.AL);
        }

        //0xa3
        private void MOV_OW_AX()
        {
            uint seg = GetSegment();
            ushort ptr = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV [${0:X04}],AX", ptr);

            mem.PokeW((int)(seg + ptr), regs.AX);
        }

        //0xa4
        private void MOVSB()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOVSB [ES:DI]:{0:X05} [DS:SI]:{1:X05}", regs.ES_DI, regs.DS_SI);
                mem.PokeB(regs.ES_DI, mem.PeekB(regs.DS_SI));
                regs.DI += (Int16)((regs.DF) ? -1 : 1);
                regs.SI += (Int16)((regs.DF) ? -1 : 1);
            }

            if (repSW)
            {
                //MOVSの場合は単純リピート
                //REPNEはあり得ない(と思う為未対処

                regs.CX--;
                if (regs.CX == 0)// || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xa5
        private void MOVSW()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOVSW [ES:DI]:{0:X05} [DS:SI]:{1:X05}", regs.ES_DI, regs.DS_SI);
                mem.PokeW(regs.ES_DI, mem.PeekW(regs.DS_SI));
                regs.DI += (Int16)((regs.DF) ? -2 : 2);
                regs.SI += (Int16)((regs.DF) ? -2 : 2);
            }

            if (repSW)
            {
                //MOVSの場合は単純リピート
                //REPNEはあり得ない(と思う為未対処

                regs.CX--;
                if (regs.CX == 0)// || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xa6
        private void CMPSB()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                byte dsv = mem.PeekB(regs.DS_SI);
                byte edv = mem.PeekB(regs.ES_DI);

                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMPSB [DS:SI] val:{0:X02}'{2}' [ES:DI] val:{1:X02}'{3}'", dsv, edv, (char)dsv, (char)edv);

                regs.SI += (Int16)((regs.DF) ? -1 : 1);
                regs.DI += (Int16)((regs.DF) ? -1 : 1);

                int ians = dsv - edv;
                byte ans = (byte)ians;

                regs.SF = (ans & 0x80) != 0;
                regs.OF = regs.SF
                    ? ((edv > 0 && ans > dsv) || (edv < 0 && ans < dsv))
                    : ans > dsv;
                regs.CF = (ushort)ians != (byte)ans;
                regs.ZF = ans == 0;
                regs.PF = (ans & 0x01) != 0;
                regs.AF = false;//TBD
            }

            if (repSW)
            {
                regs.CX--;
                if (regs.CX == 0 || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xa7
        private void CMPSW()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                short dsv = mem.PeekW(regs.DS_SI);
                short edv = mem.PeekW(regs.ES_DI);

                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CMPSW [DS:SI] val:{0:X04} [ES:DI] val:{1:X04}", dsv, edv);

                regs.SI += (Int16)((regs.DF) ? -2 : 2);
                regs.DI += (Int16)((regs.DF) ? -2 : 2);

                int ians = dsv - edv;
                ushort ans = (ushort)ians;

                regs.SF = (ans & 0x8000) != 0;
                regs.OF = regs.SF
                    ? ((edv > 0 && ans > dsv) || (edv < 0 && ans < dsv))
                    : ans > dsv;
                regs.CF = (uint)ians != (uint)ans;
                regs.ZF = ans == 0;
                regs.SetSZPFw(ans);
                regs.AF = false;//TBD
            }

            if (repSW)
            {
                regs.CX--;
                if (regs.CX == 0 || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xaa
        private void STOSB()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "STOSB [ES:DI]:{0:X05} AL:{1:X02}'{2}'", regs.ES_DI, regs.AL, (char)regs.AL);
                mem.PokeB(regs.ES_DI, regs.AL);
                regs.DI += (Int16)((regs.DF) ? -1 : 1);
            }

            if (repSW)
            {
                //STOSの場合は単純リピート
                //REPNEはあり得ない(と思う為未対処

                regs.CX--;
                if (regs.CX == 0)// || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xab
        private void STOSW()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "STOSW [ES:DI]:{0:X05} <- AX:{1:X04}", regs.ES_DI, regs.AX);
                mem.PokeW(regs.ES_DI, regs.AX);
                regs.DI += (Int16)((regs.DF) ? -2 : 2);
            }

            if (repSW)
            {
                //STOSの場合は単純リピート
                //REPNEはあり得ない(と思う為未対処

                regs.CX--;
                if (regs.CX == 0)// || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xac
        private void LODSB()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                regs.AL = mem.PeekB(regs.DS_SI);
                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "LODSB [DS:SI]:{0:X05} AL:{1:X02}'{2}'", regs.DS_SI, regs.AL, (char)regs.AL);
                regs.SI += (Int16)((regs.DF) ? -1 : 1);
            }

            if (repSW)
            {
                //LODSの場合は単純リピート
                //REPNEはあり得ない(と思う為未対処
                regs.CX--;
                if (regs.CX == 0 || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xad
        private void LODSW()
        {
            if (!repSW || (repSW && regs.CX != 0))
            {
                Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "LODSW [DS:SI]:{0:X05} -> AX:{1:X04}", regs.DS_SI, regs.AX);
                regs.AX = mem.PeekW(regs.DS_SI);
                regs.SI += (Int16)((regs.DF) ? -2 : 2);
            }

            if (repSW)
            {
                //LODSの場合は単純リピート
                //REPNEはあり得ない(と思う為未対処

                regs.CX--;
                if (regs.CX == 0)// || ((repType == 0 && !regs.ZF) || (repType != 0 && regs.ZF)))
                {
                    repSW = false;
                }
                else
                    regs.IP--;
            }
        }

        //0xb0
        private void MOV_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV AL,${0:X02}", imm8);
            regs.AL = imm8;
        }

        //0xb1
        private void MOV_CL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV CL,${0:X02}", imm8);
            regs.CL = imm8;
        }

        //0xb2
        private void MOV_DL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV DL,${0:X02}", imm8);
            regs.DL = imm8;
        }

        //0xb3
        private void MOV_BL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV BL,${0:X02}", imm8);
            regs.BL = imm8;
        }

        //0xb4
        private void MOV_AH_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV AH,${0:X02}", imm8);
            regs.AH = imm8;
        }

        //0xb5
        private void MOV_CH_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV CH,${0:X02}", imm8);
            regs.CH = imm8;
        }

        //0xb6
        private void MOV_DH_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV DH,${0:X02}", imm8);
            regs.DH = imm8;
        }

        //0xb7
        private void MOV_BH_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV BH,${0:X02}", imm8);
            regs.BH = imm8;
        }

        //0xb8
        private void MOV_AX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV AX,${0:X04}", imm16);
            regs.AX = (short)imm16;
        }

        //0xb9
        private void MOV_CX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV CX,${0:X04}", imm16);
            regs.CX = (short)imm16;
        }

        //0xba
        private void MOV_DX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV DX,${0:X04}", imm16);
            regs.DX = (short)imm16;
        }

        //0xbb
        private void MOV_BX_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV BX,${0:X04}", imm16);
            regs.BX = (short)imm16;
        }

        //0xbc
        private void MOV_SP_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV SP,${0:X04}", imm16);
            regs.SP = (short)imm16;
        }

        //0xbd
        private void MOV_BP_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV BP,${0:X04}", imm16);
            regs.BP = (short)imm16;
        }

        //0xbe
        private void MOV_SI_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV SI,${0:X04}", imm16);
            regs.SI = (short)imm16;
        }

        //0xbf
        private void MOV_DI_IW()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV DI,${0:X04}", imm16);
            regs.DI = (short)imm16;
        }

        // 0xc0 後述
        // 0xc1 後述

        //0xc3
        private void RET()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "RET");
            regs.IP = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        //0xc4
        private void LES_GW_EP()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "LES GW,EP modrw:${0:X02}", modrw);

            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            //GW eRegs@reg

            int ptr;
            Int16 v;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    v = mem.PeekW(ptr);
                    regs.eRegs[reg] = v;
                    regs.ES = mem.PeekW(ptr + 2);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    v = mem.PeekW(ptr);
                    regs.eRegs[reg] = v;
                    regs.ES = mem.PeekW(ptr + 2);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    v = mem.PeekW(ptr);
                    regs.eRegs[reg] = v;
                    regs.ES = mem.PeekW(ptr + 2);
                    break;
                case 3:
                    Int16 r = regs.eRegs[rm];
                    regs.sRegs[reg] = r;
                    break;
            }
        }

        // 0xc6
        private void MOV_EB_IB()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            byte imm8;
            int ptr;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    imm8 = Fetch();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EB,${0:X02}", imm8);
                    mem.PokeB(ptr, imm8);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    imm8 = Fetch();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EB,${0:X02}", imm8);
                    mem.PokeB(ptr, imm8);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    imm8 = Fetch();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EB,${0:X02}", imm8);
                    mem.PokeB(ptr, imm8);
                    break;
                case 3:
                    imm8 = Fetch();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EB,${0:X02}", imm8);
                    //regs.eRegs[reg] = (Int16)((regs.eRegs[reg] & 0xff00) | imm8);
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | imm8);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (imm8 << 8));
                    break;
            }

        }

        // 0xc7
        private void MOV_EW_IW()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            int ptr;
            ushort imm16;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    imm16 = Fetchw();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EW,${0:X04}", imm16);
                    mem.PokeW(ptr, (short)imm16);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    imm16 = Fetchw();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EW,${0:X04}", imm16);
                    mem.PokeW(ptr, (short)imm16);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    imm16 = Fetchw();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EW,${0:X04}", imm16);
                    mem.PokeW(ptr, (short)imm16);
                    break;
                case 3:
                    imm16 = Fetchw();
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "MOV EW,${0:X04}", imm16);
                    regs.eRegs[reg] = (short)imm16;
                    break;
            }
        }

        //0xcd
        private void INT_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INT ${0:X02}", imm8);
            dos.INT(imm8);
        }

        //0xcf
        private void IRET()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "IRET");
            regs.IP = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.CS = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
            regs.FLAG = mem.PeekW(regs.SS_SP);
            regs.SP += 2;
        }

        // 0xd0
        private void GRP2_EB_1()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            sbyte ans = 0;
            byte uans = 0;

            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            switch (reg)
            {
                case 0://ROL
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ROL EB,1 modrw:${0:X02}", modrw);
                    uans = unchecked((byte)EB);
                    uans = (byte)((uans << 1) | ((uans & 0x80) == 0 ? 0 : 1));
                    ans = (sbyte)uans;
                    regs.CF = (uans & 0x01) != 0;
                    //regs.OF = (ans & 0x8000) != 0;
                    break;
                case 1://ROR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ROR EB,1 modrw:${0:X02}", modrw);
                    uans = unchecked((byte)EB);
                    uans = (byte)((uans >> 1) | ((uans & 0x01) == 0 ? 0 : 0x80));
                    ans = (sbyte)uans;
                    regs.CF = (uans & 0x80) != 0;
                    //regs.OF = (ans & 0x8000) != 0;
                    break;
                case 2://RCL
                    throw new NotImplementedException();
                case 3://RCR
                    throw new NotImplementedException();
                case 4://SHL
                case 6://同じSHL
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SHL EB,1 modrw:${0:X02}", modrw);
                    uans = unchecked((byte)EB);
                    regs.CF = (uans & 0x80) != 0;
                    uans <<= 1;
                    ans = (sbyte)uans;
                    regs.OF = (ans & 0x8000) != 0;
                    regs.SetSZPFb(uans);
                    regs.AF = true;//TBD
                    break;
                case 5://SHR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SHR EB,1 modrw:${0:X02}", modrw);
                    uans = unchecked((byte)EB);
                    regs.CF = (uans & 0x01) != 0;
                    uans >>= 1;
                    ans = (sbyte)uans;
                    regs.OF = (ans & 0x8000) != 0;
                    regs.SetSZPFb(uans);
                    regs.AF = true;//TBD
                    break;
                case 7://SAR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SAR EB,1 modrw:${0:X02}", modrw);
                    ans = EB;
                    regs.CF = (ans & 0x01) != 0;
                    ans >>= 1;
                    uans = (byte)ans;
                    regs.OF = false;
                    regs.SetSZPFb(uans);
                    regs.AF = true;//TBD
                    break;
            }

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeB(ptr, uans);
                    break;
                case 3:
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | uans);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (uans << 8));
                    break;
            }
        }

        // 0xd1
        private void GRP2_EW_1()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            Int16 ans = 0;
            UInt16 uans = 0;

            int ptr = 0;
            ushort EW = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 3:
                    EW = (ushort)regs.eRegs[rm];
                    break;
            }

            bool newCF;
            switch (reg)
            {
                case 0://ROL
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ROL EW,1 modrw:${0:X02}", modrw);

                    uans = unchecked((UInt16)EW);
                    regs.CF = (uans & 0x8000) != 0;
                    uans = (ushort)((uans << 1) | (regs.CF ? 1 : 0));
                    ans = (Int16)uans;

                    regs.OF = (ans & 0x8000) != 0;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
                case 1://ROR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "ROR EW,1 modrw:${0:X02}", modrw);

                    uans = unchecked((UInt16)EW);
                    regs.CF = (uans & 0x0001) != 0;
                    uans = (ushort)((uans >> 1) | (regs.CF ? 1 : 0));
                    ans = (Int16)uans;
                    regs.OF = (ans & 0x0001) != 0;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
                case 2://RCL
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "RCL EW,1 modrw:${0:X02}", modrw);

                    uans = unchecked((UInt16)EW);
                    newCF = (uans & 0x8000) != 0;
                    uans = (ushort)((EW << 1) | (regs.CF ? 1 : 0));
                    regs.CF = newCF;
                    ans = (Int16)uans;
                    regs.OF = (ans & 0x8000) != 0;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
                case 3://RCR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "RCR EW,1 modrw:${0:X02}", modrw);

                    uans = unchecked((UInt16)EW);
                    newCF = (uans & 0x0001) != 0;
                    uans = (ushort)((EW >> 1) | (regs.CF ? 0x8000 : 0x000));
                    regs.CF = newCF;
                    ans = (Int16)uans;
                    regs.OF = (ans & 0x0001) != 0;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
                case 4://SHL
                case 6://同じSHL
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SHL EW,1 modrw:${0:X02}", modrw);
                    uans = unchecked((UInt16)EW);
                    regs.CF = (uans & 0x8000) != 0;
                    uans <<= 1;
                    ans = (Int16)uans;
                    regs.OF = (ans & 0x8000) != 0;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
                case 5://SHR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SHR EW,1 modrw:${0:X02}", modrw);
                    uans = unchecked((UInt16)EW);
                    regs.CF = (uans & 0x0001) != 0;
                    uans >>= 1;
                    ans = (Int16)uans;
                    regs.OF = (ans & 0x8000) != 0;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
                case 7://SAR
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "SAR EW,1 modrw:${0:X02}", modrw);
                    ans = (short)EW;
                    regs.CF = (ans & 0x01) != 0;
                    ans >>= 1;
                    uans = (ushort)ans;
                    regs.OF = false;
                    regs.SetSZPFw((ushort)ans);
                    regs.AF = true;//TBD
                    break;
            }

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeW(ptr, (short)uans);
                    break;
                case 3:
                    regs.eRegs[rm] = (Int16)(uans);// (regs.eRegs[rm] & 0xff00) | uans);
                    break;
            }

        }

        // 0xc0 or 0xd2
        private void GRP2_EB_CL(byte op)
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "GRP2_EB_CL op:${0:X02} modrw:${1:X02}", op, modrw);
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte ans = 0;
            byte uans = 0;
            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            int count = (op == 0xd2 ? regs.CL : Fetch()) & 0x1f;
            if (count == 0) return;

            bool newCF;
            if (count == 1)
            {
                switch (reg)
                {
                    case 0://ROL
                        uans = unchecked((byte)EB);
                        newCF = (uans & 0x80) != 0;
                        uans = (byte)((uans << 1) | ((uans & 0x80) != 0 ? 1 : 0));
                        ans = (sbyte)uans;
                        regs.CF = newCF;
                        regs.OF = newCF;
                        break;
                    case 1://ROR
                        throw new NotImplementedException();
                    case 2://RCL
                        uans = unchecked((byte)EB);
                        newCF = (uans & 0x80) != 0;
                        uans = (byte)((uans << 1) | (regs.CF ? 1 : 0));
                        ans = (sbyte)uans;
                        regs.CF = newCF;
                        regs.OF = newCF;
                        break;
                    case 3://RCR
                        throw new NotImplementedException();
                    case 4://SHL
                        uans = unchecked((byte)EB);
                        regs.CF = (uans & 0x80) != 0;
                        uans <<= 1;
                        regs.SetSZPFb(uans);
                        break;
                    case 5://SHR
                        uans = unchecked((byte)EB);
                        regs.CF = (uans & 1) != 0;
                        uans >>= 1;
                        regs.SetSZPFb(uans);
                        break;
                    case 6://(SMO)
                        throw new NotImplementedException();
                    case 7://SAR
                        throw new NotImplementedException();
                }
            }
            else
            {
                switch (reg)
                {
                    case 0://ROL
                        uans = unchecked((byte)EB);
                        newCF = (uans & (0x100 >> count)) != 0;
                        uans = (byte)((uans << count) | (uans >> (8 - count)));
                        ans = (sbyte)uans;
                        regs.CF = newCF;
                        regs.OF = newCF;
                        break;
                    case 1://ROR
                        throw new NotImplementedException();
                    case 2://RCL
                        uans = unchecked((byte)EB);
                        byte v = (byte)(uans & (0xff << count));
                        newCF = ((byte)(uans & (0x100 >> count)) != 0);
                        uans = (byte)((uans << count) | ((regs.CF ? 1 : 0) << (count - 1)) | (v >> (9 - count)));
                        ans = (sbyte)uans;
                        regs.CF = newCF;
                        regs.OF = newCF;
                        break;
                    case 3://RCR
                        throw new NotImplementedException();
                    case 4://SHL論理シフト
                        uans = unchecked((byte)EB);
                        regs.CF = (uans & (0x80 >> (count - 1))) != 0;
                        uans <<= count;
                        regs.SetSZPFb(uans);
                        break;
                    case 5://SHR論理シフト
                        uans = unchecked((byte)EB);
                        regs.CF = (uans & (1 << (count - 1))) != 0;
                        uans >>= count;
                        regs.SetSZPFb(uans);
                        break;
                    case 6://(SMO)
                        throw new NotImplementedException();
                    case 7://SAR
                        throw new NotImplementedException();
                }

            }

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeB(ptr, uans);
                    break;
                case 3:
                    if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | uans);
                    else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (uans << 8));
                    break;
            }

        }

        // 0xc1 or 0xd3
        private void GRP2_EW_CL(byte op)
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "GRP2_EW_CL op:${0:X02} modrw:${1:X02}", op, modrw);
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);
            Int16 ans = 0;
            UInt16 uans = 0;

            int ptr = 0;
            ushort EW = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = (ushort)mem.PeekW(ptr);
                    break;
                case 3:
                    EW = (ushort)regs.eRegs[rm];
                    break;
            }

            int count = (op == 0xd3 ? regs.CL : Fetch()) & 0x1f;
            if (count == 0) return;

            if (count == 1)
            {
                switch (reg)
                {
                    case 0://ROL
                        throw new NotImplementedException();
                    case 1://ROR
                        throw new NotImplementedException();
                    case 2://RCL
                        throw new NotImplementedException();
                    case 3://RCR
                        throw new NotImplementedException();
                    case 4://SHL
                    case 6://同じSHL
                        uans = unchecked(EW);
                        regs.CF = (ans & 0x8000) != 0;
                        uans <<= 1;
                        ans = (short)uans;
                        uans = (ushort)ans;
                        regs.OF = (ans & 0x8000) != 0;
                        regs.SetSZPFw(uans);
                        regs.AF = true;//TBD
                        break;
                    case 5://SHR
                        uans = unchecked(EW);
                        regs.CF = (ans & 0x01) != 0;
                        uans >>= 1;
                        ans = (short)uans;
                        uans = (ushort)ans;
                        regs.OF = (ans & 0x8000) != 0;
                        regs.SetSZPFw(uans);
                        regs.AF = true;//TBD
                        break;
                    case 7://SAR
                        ans = unchecked((short)EW);
                        regs.CF = (ans & 0x01) != 0;
                        ans >>= 1;
                        uans = (ushort)ans;
                        regs.OF = false;
                        regs.SetSZPFw(uans);
                        regs.AF = true;//TBD
                        break;
                }
            }
            else
            {
                switch (reg)
                {
                    case 0://ROL
                        throw new NotImplementedException();
                    case 1://ROR
                        throw new NotImplementedException();
                    case 2://RCL
                        throw new NotImplementedException();
                    case 3://RCR
                        throw new NotImplementedException();
                    case 4://SHL
                    case 6://同じSHL
                        uans = unchecked((ushort)EW);
                        uans <<= count - 1;
                        regs.CF = (uans & 0x8000) != 0;
                        uans <<= 1;
                        ans = (short)uans;
                        regs.SetSZPFw(uans);
                        regs.AF = true;//TBD
                        break;
                    case 5://SHR(論理右シフト)
                        uans = unchecked((ushort)EW);
                        uans >>= count - 1;
                        regs.CF = (uans & 0x01) != 0;
                        uans >>= 1;
                        ans = (short)uans;
                        regs.SetSZPFw(uans);
                        regs.AF = true;//TBD
                        break;
                    case 7://SAR
                        ans = unchecked((short)EW);
                        ans >>= count - 1;
                        regs.CF = (ans & 0x01) != 0;
                        ans >>= 1;
                        uans = (ushort)ans;
                        regs.SetSZPFw(uans);
                        regs.AF = true;//TBD
                        break;
                }
            }

            switch (mod)
            {
                case 0:
                case 1:
                case 2:
                    mem.PokeW(ptr, (short)uans);
                    break;
                case 3:
                    regs.eRegs[rm] = (short)uans;
                    break;
            }


        }

        //0xd4
        private void AAM()
        {
            byte imm8 = Fetch();
            byte a = regs.AL;
            regs.AH = (byte)(a / imm8);
            regs.AL = (byte)(a % imm8);
            regs.SetSZPFw((ushort)regs.AX);
            regs.AF = true;//TBD
        }

        // 0xe8
        private void CALL_near()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CALL near ${0:X04}", imm16);
            regs.SP -= 2;
            mem.PokeW(regs.SS_SP, regs.IP);
            regs.IP = (Int16)(regs.IP + imm16);
        }

        // 0xe2
        private void LOOP_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            if ((ushort)regs.CX < 100) Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "LOOP short ${0:X02} CX:${1:X04}", imm8, regs.CX);

            regs.CX--;
            if (regs.CX != 0)
            {
                regs.IP = (Int16)(regs.IP + imm8);
            }
        }

        // 0xe4
        private void IN_AL_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "IN AL,${0:X02}", imm8);
            regs.AL = machine.INPb((ushort)imm8);
        }

        // 0xe5
        private void IN_AX_IB()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "IN AX,${0:X02}", imm8);
            regs.AX = machine.INPw((ushort)imm8);
        }

        // 0xe6
        private void OUT_IB_AL()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OUT ${0:X02},AL", imm8);
            machine.OUTPb((ushort)imm8, regs.AL);
        }

        // 0xe7
        private void OUT_IB_AX()
        {
            byte imm8 = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OUT ${0:X02},AX", imm8);
            machine.OUTPw((ushort)imm8, regs.AX);
        }

        // 0xe9
        private void JMP_near()
        {
            ushort imm16 = Fetchw();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JMP near ${0:X04}", imm16);

            regs.IP = (Int16)(regs.IP + imm16);
        }

        // 0xeb
        private void JMP_short()
        {
            sbyte imm8 = (sbyte)Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JMP short ${0:X02}", imm8);

            regs.IP = (Int16)(regs.IP + imm8);
        }

        // 0xec
        private void IN_AL_DX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "IN AL,DX");
            regs.AL = machine.INPb((ushort)regs.DX);
        }

        // 0xed
        private void IN_AX_DX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "IN AX,DX");
            regs.AX = machine.INPw((ushort)regs.DX);
        }

        // 0xee
        private void OUT_DX_AL()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OUT DX,AL");
            machine.OUTPb((ushort)regs.DX, regs.AL);
        }

        // 0xef
        private void OUT_DX_AX()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "OUT DX,AX");
            machine.OUTPw((ushort)regs.DX, regs.AX);
        }

        // 0xf2
        private void REPNE()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "REPNE");
            repSW = true;
            repType = 1;
        }

        // 0xf3
        private void REPE()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "REPE");
            repSW = true;
            repType = 0;
        }

        // 0xf4
        private void HLT()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "HLT");
            hltSW = true;
        }

        // 0xf6
        private void GRP3B()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "GRP3B modrw:${0:X02}", modrw);
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte EB = 0;
            int ptr = 0;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            sbyte IB;
            byte ans = 0;
            switch (reg)
            {
                case 0://TEST EB,IB
                    IB = (sbyte)Fetch();
                    ans = (byte)(EB & IB);
                    regs.SetSZPFb(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
                case 1://?
                    throw new NotImplementedException();
                case 2://NOT EB
                    ans = (byte)(~EB);
                    regs.SetSZPFb(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
                case 3://NEG EB
                    ans = (byte)(-EB);
                    regs.SetSZPFb(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
                case 4://MUL EB
                    int mans = regs.AL * (byte)EB;
                    regs.AX = (short)(ushort)mans;
                    regs.OF = regs.AH != 0;
                    regs.CF = regs.OF;
                    break;
                case 5://IMUL EB
                    throw new NotImplementedException();
                case 6://DIV EB
                    ans = (byte)((ushort)regs.AX / (byte)EB);
                    mod = (byte)((ushort)regs.AX % (byte)EB);
                    regs.AL = ans;
                    regs.AH = mod;
                    break;
                case 7://IDIV EB
                    throw new NotImplementedException();
            }

            if (reg != 0 && reg < 4)
            {
                switch (mod)
                {
                    case 0:
                    case 1:
                    case 2:
                        mem.PokeB(ptr, (byte)ans);
                        break;
                    case 3:
                        if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | (byte)ans);
                        else regs.eRegs[rm - 4] = (Int16)((ans << 8) | (byte)regs.eRegs[rm - 4]);
                        break;
                }
            }
        }

        // 0xf7
        private void GRP3W()
        {
            byte modrw = Fetch();
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "GRP3W modrw:${0:X02}", modrw);
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            Int16 EW = 0;
            int ptr = 0;
            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = (Int16)mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = (Int16)mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = (Int16)mem.PeekW(ptr);
                    break;
                case 3:
                    EW = regs.eRegs[rm];
                    break;
            }

            Int16 IW;
            UInt16 ans = 0;
            switch (reg)
            {
                case 0://TEST EW,IW
                    IW = (Int16)Fetchw();
                    ans = (UInt16)(EW & IW);
                    regs.SetSZPFw(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
                case 1://?
                    throw new NotImplementedException();
                case 2://NOT EW
                    ans = (UInt16)(~EW);
                    regs.SetSZPFw(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
                case 3://NEG EW
                    ans = (UInt16)(-EW);
                    regs.SetSZPFw(ans);
                    regs.OF = false;
                    regs.CF = false;
                    regs.AF = false;//TBD
                    break;
                case 4://MUL EW
                    uint ans32 = (uint)((ushort)regs.AX * (ushort)EW);
                    regs.AX = (short)ans32;
                    regs.DX = (short)(ans32 >> 16);
                    break;
                case 5://IMUL EW
                    int ians32 = (int)((short)regs.AX * (short)EW);
                    regs.AX = (short)ians32;
                    regs.DX = (short)(ians32 >> 16);
                    break;
                case 6://DIV EW
                    uint ans32d = (uint)((uint)(((ushort)regs.DX << 16) + (ushort)regs.AX) / (uint)EW);
                    uint modud = (uint)((uint)(((ushort)regs.DX << 16) + (ushort)regs.AX) % (uint)EW);
                    regs.AX = (short)ans32d;
                    regs.DX = (short)modud;
                    break;
                case 7://IDIV EB
                    int ians32d = (int)(((ushort)regs.DX << 16) + (ushort)regs.AX) / (int)EW;
                    int modu = (int)(((ushort)regs.DX << 16) + (ushort)regs.AX) % (int)EW;
                    regs.AX = (short)ians32d;
                    regs.DX = (short)modu;
                    break;
            }

            if (reg != 0 && reg < 4)
            {
                switch (mod)
                {
                    case 0:
                    case 1:
                    case 2:
                        mem.PokeW(ptr, (short)ans);
                        break;
                    case 3:
                        regs.eRegs[rm] = (Int16)ans;
                        break;
                }
            }
        }

        // 0xf8
        private void CLC()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CLC");
            regs.CF = false;
        }

        // 0xf9
        private void STC()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "STC");
            regs.CF = true;
        }

        // 0xfa
        private void CLI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CLI");
            regs.IF = false;
        }

        // 0xfb
        private void STI()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "STI");
            regs.IF = true;
        }

        // 0xfc
        private void CLD()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CLD");
            regs.DF = false;
        }

        // 0xfd
        private void STD()
        {
            Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "STD");
            regs.DF = true;
        }

        // 0xfe
        private void GRP4()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            sbyte EB = 0;
            int ptr = 0;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EB = (sbyte)mem.PeekB(ptr);
                    break;
                case 3:
                    if (rm < 4) EB = (sbyte)(byte)regs.eRegs[rm];
                    else EB = (sbyte)(byte)(regs.eRegs[rm - 4] >> 8);
                    break;
            }

            byte IB = 1;
            short ians;
            byte ans;

            switch (reg)
            {
                case 0://INC EB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC EB");
                    ians = (short)(EB + IB);
                    ans = (byte)ians;
                    regs.SetSZPFb(ans);
                    regs.SetOFbAdd((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 1://DEC EB
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC EB");
                    ians = (short)(EB - IB);
                    ans = (byte)ians;
                    regs.SetSZPFb(ans);
                    regs.SetOFbSub((byte)EB, (byte)IB, (byte)ans);
                    regs.SetCFb((ushort)ians);
                    regs.SetAF((byte)EB, (byte)IB, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeB(ptr, ans);
                            break;
                        case 3:
                            if (rm < 4) regs.eRegs[rm] = (Int16)((regs.eRegs[rm] & 0xff00) | ans);
                            else regs.eRegs[rm - 4] = (Int16)((byte)regs.eRegs[rm - 4] | (ans << 8));
                            break;
                    }
                    break;
                case 2://?
                    throw new NotImplementedException();
                case 3://?
                    throw new NotImplementedException();
                case 4://?
                    throw new NotImplementedException();
                case 5://?
                    throw new NotImplementedException();
                case 6://?
                    throw new NotImplementedException();
                case 7://?
                    throw new NotImplementedException();
            }
        }

        // 0xff
        private void GRP5()
        {
            byte modrw = Fetch();
            byte reg = (byte)((modrw & 0x38) >> 3);//セグメントレジスタの場合はbit5を無視
            byte rm = (byte)(modrw & 7);
            byte mod = (byte)(modrw >> 6);

            Int16 EW = 0;
            int ptr = 0;

            bool bSegPrefSw = segPrefSw;
            int bSegPref = segPref;

            switch (mod)
            {
                case 0:
                    ptr = GetMod00RWADR(rm);
                    EW = (Int16)mem.PeekW(ptr);
                    break;
                case 1:
                    ptr = GetMod01RWADR(rm);
                    EW = (Int16)mem.PeekW(ptr);
                    break;
                case 2:
                    ptr = GetMod02RWADR(rm);
                    EW = (Int16)mem.PeekW(ptr);
                    break;
                case 3:
                    EW = regs.eRegs[rm];
                    break;
            }

            UInt16 IW = 1;
            Int16 ians;
            UInt16 ans;

            switch (reg)
            {
                case 0://INC EW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "INC EW");
                    ians = (short)(EW + IW);
                    ans = (ushort)ians;
                    regs.SetSZPFw(ans);
                    regs.SetOFwAdd((ushort)EW, (ushort)IW, (ushort)ans);
                    regs.SetCFw((uint)(EW + IW));
                    regs.SetAF((byte)EW, (byte)IW, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ians);
                            break;
                        case 3:
                            regs.eRegs[rm] = ians;
                            break;
                    }
                    break;
                case 1://DEC EW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "DEC EW");
                    ians = (short)(EW - IW);
                    ans = (ushort)ians;
                    regs.SetSZPFw(ans);
                    regs.SetOFwSub((ushort)EW, (ushort)IW, (ushort)ans);
                    regs.SetCFw((uint)(EW - IW));
                    regs.SetAF((byte)EW, (byte)IW, (byte)ans);
                    switch (mod)
                    {
                        case 0:
                        case 1:
                        case 2:
                            mem.PokeW(ptr, ians);
                            break;
                        case 3:
                            regs.eRegs[rm] = ians;
                            break;
                    }
                    break;
                case 2://CALL EW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CALL EW");
                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, regs.IP);
                    regs.IP = EW;
                    break;
                case 3://CALL EP
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "CALL EP");

                    segPrefSw = bSegPrefSw;
                    segPref = bSegPref;
                    int ptr2;
                    short seg = 0;
                    ptr2 = ptr + 2;
                    seg = (Int16)mem.PeekW(ptr2);

                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, regs.CS);
                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, regs.IP);
                    regs.IP = EW;
                    regs.CS = seg;
                    break;
                case 4://JMP EW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "JMP EW");
                    regs.IP = EW;
                    break;
                case 5://
                    throw new NotImplementedException();
                case 6://PUSH EW
                    Log.WriteLine(musicDriverInterface.LogLevel.TRACE, "PUSH EW");
                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, EW);
                    break;
                case 7://?
                    throw new NotImplementedException();
            }
        }

    }
}
