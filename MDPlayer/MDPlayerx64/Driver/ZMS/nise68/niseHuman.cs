using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class niseHuman
    {
        public static uint mpcmPtr = 0xfe9000;//MPCMの常駐位置(仮)

        private Memory68 mem;
        private Register68 reg;
        public bool programTerminate;
        public int returnCode;
        private UInt32 beforePSP;
        private UInt32 envAddress = 0;
        private UInt32 ctrlCAbortAddress = 0;
        private UInt32 errorAbortAddress = 0;
        private UInt32 cmdLineAddress = 0;
        private UInt32 PSPSize = 16 + 240;
        public UInt32 defUSP = 0xfe_0000;
        public UInt32 defSSP = 0xff_0000;
        private Action[] tblFunc = new Action[256];
        private Action[] tblFEFunc = new Action[256];
        
        private myEncoding enc;
        public memMng memMng;
        private procInfo currentProc = new procInfo();
        private uint execPtr;
        private int fileHandle = 0;
        private FileIni[] fi = new FileIni[256];
        private string currentWorkPath = "C:\\";//niseHumanにC:\\と思わせる実際のパス
        //public Dictionary<string, byte[]> fb = new Dictionary<string, byte[]>();
        private List<string> envZPDs;
        private FileMng fileMng;

        public class procInfo
        {
            public UInt32 startAddress;
        }

        public niseHuman(Memory68 mem, Register68 reg,List<string> envZPDs,FileMng fm)
        {
            enc = new myEncoding();
            this.mem = mem;
            this.reg = reg;
            this.envZPDs = envZPDs;
            this.fileMng = fm;

            programTerminate = false;
            returnCode = 0;
            beforePSP = 0;
            memMng = new memMng(0x0004_0000);
            //defUSP = (uint)memMng.Malloc(0x1_0000);
            //defSSP = (uint)memMng.Malloc(0x1_0000);

            tblFunc = new Action[]
            {
                //0x00
                exit,null,putchar,null,  null,null,null,inkey,  null,print,null,null,  null,null,null,drvctrl,
                //0x10
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,fputs,null,
                //0x20
                super,null,null,conctrl,  null,intvcs,null,null,  null,null,null,null,  null,null,null,null,
                //0x30
                vernum,keeppr,null,null,  null,null,null,nameck,  null,null,null,null,  create,open,close,read,

                //0x40
                write,delete,seek,null,  null,null,null,null,  malloc,mfree,setblock,exec,  exit2,null,files,null,
                //0x50
                null,getpsp,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x60
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x70
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,

                //0x80
                null,getpsp,null,null,  null,null,null,filedate,  null,null,maketmp,null,  null,null,null,null,
                //0x90
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xa0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,s_malloc,s_mfree,null,
                //0xb0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,

                //0xc0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xd0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xe0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xf0
                null,null,null,null,  null,null,null,bus_err,  null,null,null,null,  null,null,null,null,

            };

            tblFEFunc = new Action[]
            {
                //0x00
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x10
                null,_LTOS,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x20
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x30
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,

                //0x40
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x50
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x60
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x70
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,

                //0x80
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0x90
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xa0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xb0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,

                //0xc0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xd0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xe0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,
                //0xf0
                null,null,null,null,  null,null,null,null,  null,null,null,null,  null,null,null,null,

            };
        }

        public void LoadAndExecuteFile(string filename, string option, uint startAddress)
        {
            Log.WriteLine(LogLevel.Information, "niseHuman>{0} {1}", filename, option);
            Log.WriteLine(LogLevel.Information, "CurrentWorkPath>{0}", currentWorkPath);

            currentWorkPath = fileMng.VCurrentPath;
            if (String.IsNullOrEmpty(currentWorkPath)) currentWorkPath = "C:\\";

            byte[] bin = fileMng.VReadAllBytes(filename);
            string fext = Path.GetExtension(filename).ToUpper();
            LoadRunner(bin, fext == ".R", option, startAddress);
        }

        private void LoadRunner(byte[] prog, bool IsR, string option, uint startAddress = 0)
        {

            //startAddress = (uint)memMng.Malloc((uint)prog.Length+0x10);

            uint envSize = 0x2000;
            envAddress = startAddress - envSize;
            MakeEnv(envAddress, envSize);

            uint stackPtr = startAddress;
            uint stackSize = 0x0100;
            cmdLineAddress = stackPtr+stackSize;
            uint cmdLineSize = 0x100;
            execPtr = stackPtr + stackSize + cmdLineSize;

            MakePSP(startAddress, (uint)prog.Length, this.memMng.Address, 0x1000_0000);

            LoadImage(prog, execPtr);

            if (IsR)
            {
                //.r

                WriteOption(cmdLineAddress, option);

                //init reg
                reg.PC = execPtr;
                reg.A[0] = startAddress;
                reg.A[1] = (UInt32)(execPtr + prog.Length);//end address
                reg.A[2] = cmdLineAddress;
                reg.A[3] = envAddress;
                reg.A[4] = execPtr;
                reg.SR = 0x0000;
                reg.USP = defUSP;
                reg.SSP = defSSP;
                currentProc.startAddress = startAddress;

            }
            else
            {
                //.x
                UInt16 id = mem.PeekW(execPtr + 0);
                uint baseAdr = mem.PeekL(execPtr + 4);
                uint startAdr = mem.PeekL(execPtr + 0x08);
                uint textSize = mem.PeekL(execPtr + 0x0c);
                uint dataSize = mem.PeekL(execPtr + 0x10);
                uint heapSize = mem.PeekL(execPtr + 0x14);
                uint relocTblSize = mem.PeekL(execPtr + 0x18);
                uint symbolTblSize = mem.PeekL(execPtr + 0x1c);
                for (int i = 0; i < prog.Length - 0x40; i++)
                {
                    mem.PokeB((uint)(execPtr + i), mem.PeekB((uint)(execPtr + i + 0x40)));
                }
                relocate(textSize + dataSize, relocTblSize, execPtr);

                WriteOption(cmdLineAddress, option);

                //init reg
                reg.PC = execPtr + startAdr;
                reg.A[0] = startAddress;
                reg.A[1] = (UInt32)(execPtr + prog.Length);//end address
                reg.A[2] = cmdLineAddress;
                reg.A[3] = envAddress;
                reg.A[4] = execPtr;
                reg.SR = 0x0000;
                reg.USP = defUSP;
                reg.SSP = defSSP;
                currentProc.startAddress = startAddress;
            }


            memMng.Set(startAddress + 0x10, (uint)prog.Length);


            // memory セットアップ
            // XC20プログラマーズマニュアル メモリマップ P697 参考

            // Interrupt Vector $00 ($00_0000) - $ff ($00_03ff)
            //    $2c  TRAP12 COPYキーによる処理
            mem.PokeL(0x02c * 4, 0x00fe_002c);//dummy値
            //    $58  SCCA RS232C送信
            mem.PokeL(0x058 * 4, 0x00fe_0000);//dummy値

            // IOCS Vector      $100($00_0400) - $1ff($00_07ff)
            //    $1ff Abort処理(?)
            mem.PokeL(0x1ff * 4, 0x0012_1212);//dummy値

            //debug
            //uint zmusicPtr = 0xfe1000;//zmusicの常駐位置(仮)
            //mem.PokeL(0x8c, zmusicPtr);//Trapの位置が書いてあるんかな？
            //mem.PokeL(zmusicPtr - 0x8, 0x5a6d7553);//'ZmuS'
            //mem.PokeW(zmusicPtr - 0x4, 0x6943);//'iC'
            //mem.PokeB(zmusicPtr - 0x2, 0x31);//version 0x30以上ならOK

            //デバイス名(?)
            mem.PokeL(0x67f2, 0xffffffff);//EOF
            mem.PokeW(0x67f6, 0x8024);//?
            mem.PokeL(0x6800, 0x4e554c20);//'NUL '
            mem.PokeL(0x6804, 0x20202020);//'    '
        }

        /// <summary>
        /// .x形式のリロケート処理
        /// run68 より
        /// </summary>
        private bool relocate(uint reloc_adr, uint reloc_size, uint read_top)
        {
            uint prog_adr;
            uint data;
            UInt16 disp;

            prog_adr = read_top;
            for (; reloc_size > 0; reloc_size -= 2, reloc_adr += 2)
            {
                disp = mem.PeekW(read_top + reloc_adr);
                if (disp == 1)
                    return false;
                prog_adr += disp;
                data = mem.PeekL(prog_adr) + read_top;
                //Log.WriteLine(LogLevel.Trace, "progAdr:[{0:X08}] relocAdr:[{1:X08}]", prog_adr, data);
                mem.PokeL(prog_adr, data);
            }

            return true;
        }

        private void WriteOption(uint stackPtr, string option)
        {
            if (string.IsNullOrEmpty(option))
            {
                mem.PokeB(stackPtr, 0x00);
                return;
            }
            else
            {
                mem.PokeB(stackPtr++, (byte)' ');
            }
            byte[] opAry = enc.GetSjisArrayFromString(option);
            foreach (byte op in opAry)
            {
                mem.PokeB(stackPtr++, op);
            }
            mem.PokeB(stackPtr, 0x00);
        }

        /// <summary>
        /// 環境変数を指定アドレスへ作成する
        /// </summary>
        private void MakeEnv(uint envAddress, uint envSize)
        {
            mem.PokeL(envAddress + 0x00, envSize);//サイズ格納
            mem.PokeB(envAddress + 0x04, 0);//終端
        }

        private void MakePSP(uint startAddress, uint length, uint processID, uint progSize)
        {
            mem.PokeL(startAddress + 0x00, beforePSP);
            //mem.PokeL(startAddress + 0x04, startAddress + length);
            //mem.PokeB(startAddress + 0x04, 0x00);//0x00:normal memBlock 0xff:regidentProc memBlock
            mem.PokeL(startAddress + 0x04, processID);
            mem.PokeL(startAddress + 0x08, length);
            mem.PokeL(startAddress + 0x08, 0xb00000);// startAddress + PSPSize + progSize);//てけとー(lzz.rむけ)
            mem.PokeL(startAddress + 0x0c, 0);// nextProcPSP);

            if (beforePSP != 0)
            {
                mem.PokeL(beforePSP + 0x0c, startAddress);
            }
            beforePSP = startAddress;

            mem.PokeL(startAddress + 0x10, envAddress);//env adr
            mem.PokeL(startAddress + 0x14, startAddress + length + 0x100 - 1);//end address
            mem.PokeL(startAddress + 0x18, ctrlCAbortAddress);//ctrl+C abort address
            mem.PokeL(startAddress + 0x1c, errorAbortAddress);//error  abort address
            mem.PokeL(startAddress + 0x20, cmdLineAddress);
            mem.PokeL(startAddress + 0x24, 0);//file handle manage
            mem.PokeL(startAddress + 0x28, 0);//file handle manage
            mem.PokeL(startAddress + 0x2c, 0);//file handle manage
            mem.PokeL(startAddress + 0x30, startAddress + PSPSize + progSize);//BSS address
            mem.PokeL(startAddress + 0x34, startAddress + PSPSize + progSize);//Heap start address
            mem.PokeL(startAddress + 0x38, execPtr);//Stack address(heap end adr +1)
            mem.PokeL(startAddress + 0x3c, 0);//P Proc.USP
            mem.PokeL(startAddress + 0x40, 0);//P Proc.SSP
            mem.PokeW(startAddress + 0x44, 0);//P Proc.SR
            mem.PokeW(startAddress + 0x46, 0);//Abort  SR
            mem.PokeL(startAddress + 0x48, 0);//Abort  SSP
            mem.PokeL(startAddress + 0x4c, 0);//TRAP#10 vector address
            mem.PokeL(startAddress + 0x50, 0);//TRAP#11 vector address
            mem.PokeL(startAddress + 0x54, 0);//TRAP#12 vector address
            mem.PokeL(startAddress + 0x58, 0);//TRAP#13 vector address
            mem.PokeL(startAddress + 0x5c, 0);//TRAP#14 vector address
            mem.PokeL(startAddress + 0x60, 0xffff_ffff);//process flag(0:have P -1:OS)
            //unuse 0x64
            mem.PokeL(startAddress + 0x68, 0);//c Proc. PSPAddress(0: none)
            //unuse 0x6c - 0x7f
            mem.PokeB(startAddress + 0x80, (byte)'C');//Procの存在するドライブ
            mem.PokeB(startAddress + 0x81, (byte)':');//Procの存在するドライブ
            //Procのパス  0x82 - (max: 0xff)

        }

        public void LoadImage(byte[] bin, uint StartAdr)
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>LoadImage");
            for (int i = 0; i < bin.Length; i++)
            {
                mem.PokeB((uint)(StartAdr + i), bin[i]);
            }
        }

        public void FEFunc(ushort n)
        {
            try
            {
                tblFEFunc[(byte)n]();
            }
            catch
            {
                Log.WriteLine(LogLevel.Trace2, "<NiseHuman>FEFunc call ${0:x04}", n);
                throw;
            }
        }

        public void doscall(ushort n)
        {
            try
            {
                tblFunc[(byte)n]();
            }
            catch
            {
                Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call ${0:x04}", n);
                throw;
            }
        }

        private void exit()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF00 exit");

            programTerminate = true;
            returnCode = (int)0;
        }

        private List<byte> consoleTextBuf = new List<byte>();
        private void putchar()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF02 putchar");
            UInt16 CODE = mem.PeekW(reg.A[7] + 0);

            consoleTextBuf.Add((byte)CODE);
            if (CODE == 0x0d && consoleTextBuf.Count > 0)
            {
                Log.Write(LogLevel.Information, System.Text.Encoding.GetEncoding("shift_jis").GetString(consoleTextBuf.ToArray()));
                consoleTextBuf.Clear();
            }
        }

        private void inkey()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF07 inkey");
            reg.D[0] = 'Y';
        }

        private void print()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF09 print");
            UInt32 MESPTR = mem.PeekL(reg.A[7] + 0);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(MESPTR + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string text = enc.GetStringFromSjisArray(msg.ToArray());
            if (consoleTextBuf.Count > 0)
            {
                Log.Write(LogLevel.Information, System.Text.Encoding.GetEncoding("shift_jis").GetString(consoleTextBuf.ToArray()));
                consoleTextBuf.Clear();
            }
            text = text.Replace("{", "{{");
            text = text.Replace("}", "}}");
            Log.Write(LogLevel.Information, text);//通常のコンソール出力
        }

        private void drvctrl()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF0F drvctrl");
            UInt16 MODE = mem.PeekW(reg.A[7] + 0);
            byte MD = (byte)((MODE >> 8) & 0x7);
            byte drive = (byte)MODE;

            if (MD != 0)
            {
                reg.D[0] = unchecked((uint)0xffff_ffff_ffff_fff1);
                return;
                //throw new NotImplementedException();
            }

            //無条件で準備万端
            //b1:メディア挿入
            reg.D[0] = 0b0000_0010;
        }

        private void fputs()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF1E fputs");
            UInt32 MESPTR = mem.PeekL(reg.A[7] + 0);
            UInt16 FILENO = mem.PeekW(reg.A[7] + 4);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(MESPTR + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            if (consoleTextBuf.Count > 0)
            {
                Log.Write(LogLevel.Information, System.Text.Encoding.GetEncoding("shift_jis").GetString(consoleTextBuf.ToArray()));
                consoleTextBuf.Clear();
            }
            string text = enc.GetStringFromSjisArray(msg.ToArray());
            Log.Write(LogLevel.Information, text);//通常のコンソール出力
        }

        private void super()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF20 super");
            reg.D[0] = reg.SSP;
            reg.SSP = reg.USP;
            reg.SR |= 0x2000;//super
        }

        private void conctrl()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF23 conctrl");
            UInt16 MD = mem.PeekW(reg.A[7] + 0);
            switch (MD)
            {
                case 0:
                    byte Code = (byte)mem.PeekW(reg.A[7] + 2);
                    if (Code < 0x20)
                    {
                        if (Code != 0x07)
                            Log.WriteLine(LogLevel.Information, "ascii code {0:X02}", Code);
                        else
                            Log.WriteLine(LogLevel.Information, ((char)Code).ToString());
                    }
                    else
                    {
                        Log.WriteLine(LogLevel.Information, ((char)Code).ToString());
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void intvcs()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF25 intvcs");
            UInt16 intno = mem.PeekW(reg.A[7] + 0);
            UInt32 jobadr = mem.PeekL(reg.A[7] + 2);

            if (intno < 0x100)
            {
                reg.D[0] = mem.PeekL((uint)(intno * 4));
                mem.PokeL((uint)(intno * 4), jobadr);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void vernum()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF30 vernum");

            reg.D[0] = 0x3638_0300; // 0x3638 '68' 0x0300 version3.00
        }

        private void keeppr()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF31 keeppr");
            UInt32 prglen = mem.PeekL(reg.A[7] + 0);
            UInt16 code = mem.PeekW(reg.A[7] + 4);

            programTerminate = true;
            returnCode = (int)code;
        }

        private void nameck()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF37 nameck");
            UInt32 file = mem.PeekL(reg.A[7] + 0);
            UInt32 buffer = mem.PeekL(reg.A[7] + 4);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(file + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            Log.WriteLine(LogLevel.Trace2, "Filename:[{0}]", fn);
            fn = fileMng.VGetFullFilename(fn);
            try
            {
                string path = Path.GetDirectoryName(fn);
                string filename = Path.GetFileNameWithoutExtension(fn);
                string extension = Path.GetExtension(fn);
                
                //ドライブレター(dummy)
                mem.PokeB((uint)(buffer + 0), (byte)path[0]);
                mem.PokeB((uint)(buffer + 1), (byte)path[1]);

                //パスネーム MAX:64byte + endMark:$00
                cnt = -2;
                foreach (char c in path)
                {
                    if (cnt < 0) { cnt++; continue; }
                    mem.PokeB((uint)(buffer + 2 + cnt), (byte)c);
                    cnt++;
                    if (cnt == 64) break;
                }
                mem.PokeB((uint)(buffer + 2 + cnt), (byte)0x00);

                //ファイルネーム MAX:18byte + endMark:$00
                cnt = 0;
                foreach (char c in filename)
                {
                    mem.PokeB((uint)(buffer + 67 + cnt), (byte)c);
                    cnt++;
                    if (cnt == 18) break;
                }
                mem.PokeB((uint)(buffer + 67 + cnt), (byte)0x00);

                //拡張子 MAX:4byte + endMark:$00
                cnt = 0;
                foreach (char c in extension)
                {
                    mem.PokeB((uint)(buffer + 86 + cnt), (byte)c);
                    cnt++;
                    if (cnt == 4) break;
                }
                mem.PokeB((uint)(buffer + 86 + cnt), (byte)0x00);

                reg.D[0] = 0;//ワイルドカード指定なし
            }
            catch (Exception)
            {
                reg.D[0] = unchecked((uint)(-1));//error
            }

        }

        private void create()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF3C create");
            UInt32 NAMEPTR = mem.PeekL(reg.A[7] + 0);
            UInt16 ATR = mem.PeekW(reg.A[7] + 4);
            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(NAMEPTR + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            Log.WriteLine(LogLevel.Trace2, "Filename:[{0}] ATR:{1}", fn, ATR);

            //string physicalFn = GetPhysicalFn(fn);

            reg.D[0] = unchecked((uint)(-1));

            if (ATR != 0x20)
            {
                //読み込みのみサポート失敗
                return;
            }

            //空いているファイル情報を見つける
            fileHandle = -1;
            for (int i = 0; i < fi.Length; i++)
            {
                if (fi[i] == null) fi[i] = new FileIni();
                if (fi[i].IsOpen) continue;
                //File.Create(physicalFn).Close();

                fileHandle = i;
                fi[i].IsTemp = false;
                fi[i].IsOpen = true;
                fi[i].filename = fn;
                fi[i].ptr = 0;
                //fi[i].dat = File.ReadAllBytes(fn);
                byte[] dat;
                //if (fb.ContainsKey(physicalFn))
                //{
                //    dat = fb[physicalFn];
                //}
                //else
                //{
                //    if (File.Exists(physicalFn))
                //    {
                //        dat = File.ReadAllBytes(physicalFn);
                //    }
                //    else
                //    {
                //        dat= new byte[0];
                //    }
                //    fb.Add(physicalFn, dat);
                //}
                dat = fileMng.VReadAllBytes(fn);
                if(dat==null)
                    fi[i].memoryStream = new MemoryStream(new byte[0]);
                else fi[i].memoryStream = new MemoryStream(dat);
                break;
            }
            if (fileHandle < 0) return;

            reg.D[0] = (uint)fileHandle;

        }

        private void open()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF3D open");
            UInt32 NAMEPTR = mem.PeekL(reg.A[7] + 0);
            UInt16 MODE = mem.PeekW(reg.A[7] + 4);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(NAMEPTR + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            Log.WriteLine(LogLevel.Trace2, "Filename:[{0}] Mode:{1}", fn, MODE);

            //string physicalFn = GetPhysicalFn(fn);

            reg.D[0] = unchecked((uint)(-1));

            if (MODE != 0 && MODE != 1)
            {
                //読み込みのみサポート失敗
                return;
            }

            //空いているファイル情報を見つける
            fileHandle = -1;
            for (int i = 0; i < fi.Length; i++)
            {
                if (fi[i] == null) fi[i] = new FileIni();
                if (fi[i].IsOpen) continue;
                //if (!File.Exists(physicalFn)) continue;
                if (!fileMng.ExistsFile(fn)) continue;

                fileHandle = i;
                fi[i].IsTemp = false;
                fi[i].IsOpen = true;
                fi[i].filename = fn;
                fi[i].ptr = 0;
                byte[] dat;
                //if (fb.ContainsKey(physicalFn))
                //{
                //    dat = fb[physicalFn];
                //}
                //else
                //{
                //    dat = File.ReadAllBytes(physicalFn);
                //    fb.Add(physicalFn, dat);
                //}
                dat=fileMng.VReadAllBytes(fn);
                fi[i].memoryStream = new MemoryStream(dat);
                break;
            }
            if (fileHandle < 0) return;

            reg.D[0] = (uint)fileHandle;
        }

        private string GetPhysicalFn(string fn)
        {
            string physicalFn;
            if (fn.ToUpper().IndexOf("C:\\") == 0)
            {
                physicalFn = Path.Combine(currentWorkPath, fn.Substring(3));
            }
            else
            {
                physicalFn = Path.Combine(currentWorkPath, fn);
            }

            if (!File.Exists(physicalFn))
            {
                if (Path.GetExtension(physicalFn).ToUpper() == ".ZPD" && envZPDs != null && envZPDs.Count > 0)
                {
                    string f = Path.GetFileName(physicalFn);
                    foreach (string s in envZPDs)
                    {
                        if (!File.Exists(Path.Combine(s, f))) continue;
                        physicalFn = Path.Combine(s, f);
                    }
                }
            }

            Log.WriteLine(LogLevel.Trace2, "PhysicalFilename:[{0}] ", physicalFn);
            return physicalFn;
        }

        private void close()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF3E close");
            UInt16 fileno = mem.PeekW(reg.A[7] + 0);

            reg.D[0] = unchecked((uint)-1);
            try
            {
                if (fileno >= fi.Length) return;
                if (fi[fileno] == null) return;
                if (!fi[fileno].IsOpen) return;

                fi[fileno].IsOpen = false;

                reg.D[0] = 0;
            }
            catch { }
        }

        private void read()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF3F read");
            UInt16 fileno = mem.PeekW(reg.A[7] + 0);
            UInt32 dataPtr = mem.PeekL(reg.A[7] + 2);
            UInt32 size = mem.PeekL(reg.A[7] + 6);

            reg.D[0] = unchecked((uint)-1);
            if (fi[fileno] == null) return;
            if (!fi[fileno].IsOpen) return;

            uint i = 0;
            //if (fi[fileno].dat != null && fi[fileno].dat.Length > 0)
            //{
            //    for (; i < size; i++)
            //    {
            //        if (fi[fileno].ptr == fi[fileno].dat.Length) break;
            //        mem.PokeB(dataPtr + i, fi[fileno].dat[fi[fileno].ptr]);
            //        fi[fileno].ptr++;
            //    }
            //}
            if (fi[fileno].memoryStream != null && fi[fileno].memoryStream.Length > 0)
            {
                for (; i < size; i++)
                {
                    if (fi[fileno].ptr == fi[fileno].memoryStream.Length) break;
                    int b = fi[fileno].memoryStream.ReadByte();
                    if (b < 0) break;
                    mem.PokeB(dataPtr + i, (byte)b);
                    fi[fileno].ptr++;
                }
            }

            reg.D[0] = i;
        }

        private void write()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF40 write");
            UInt16 fileno = mem.PeekW(reg.A[7] + 0);
            UInt32 dataPtr = mem.PeekL(reg.A[7] + 2);
            UInt32 size = mem.PeekL(reg.A[7] + 6);

            reg.D[0] = unchecked((uint)-1);
            if (fi[fileno] == null) return;
            if (!fi[fileno].IsOpen) return;

            int i = 0;
            List<byte> data = new List<byte>();

            for (; i < size; i++) data.Add(mem.PeekB((uint)(dataPtr + i)));
            //string physicalFn = GetPhysicalFn(fi[fileno].filename);

            //if (fb.ContainsKey(physicalFn))
            //{
            //    byte[] f = fb[physicalFn];// File.ReadAllBytes(physicalFn);
            //    int nSize = f.Length + data.Count - fi[fileno].ptr;
            //    byte[] nf = new byte[nSize];
            //    Array.Copy(data.ToArray(), 0, nf, fi[fileno].ptr, data.Count);
            //    fb[physicalFn] = nf;// File.WriteAllBytes(physicalFn, nf);

            //    reg.D[0] = (uint)i;
            //}

            string fn = fi[fileno].filename;
            if (fileMng.ExistsFile(fn))
            {
                byte[] f=fileMng.VReadAllBytes(fn);
                int nSize = (f != null ? f.Length : 0) + data.Count - fi[fileno].ptr;
                byte[] nf = new byte[nSize];
                Array.Copy(data.ToArray(), 0, nf, fi[fileno].ptr, data.Count);
                fileMng.SetVFile(fn, nf);// File.WriteAllBytes(physicalFn, nf);

                reg.D[0] = (uint)i;
            }
        }

        private void delete()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF41 delete");
            UInt32 NAMEPTR = mem.PeekL(reg.A[7] + 0);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(NAMEPTR + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            Log.WriteLine(LogLevel.Trace2, "Filename:[{0}]", fn);
            string physicalFn = GetPhysicalFn(fn);


            reg.D[0] = 0x0000_0000;//無条件に成功
        }

        private void seek()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF42 seek");
            UInt16 fileno = mem.PeekW(reg.A[7] + 0);
            Int32 offset = (Int32)mem.PeekL(reg.A[7] + 2);
            UInt16 mode = mem.PeekW(reg.A[7] + 6);

            reg.D[0] = unchecked((uint)-1);
            if (fi[fileno] == null) return;
            if (!fi[fileno].IsOpen) return;

            switch (mode)
            {
                case 0://begin
                    fi[fileno].memoryStream.Seek(offset, SeekOrigin.Begin);
                    fi[fileno].ptr = 0 + offset;
                    break;
                case 1://seek
                    fi[fileno].memoryStream.Seek(offset, SeekOrigin.Current);
                    fi[fileno].ptr += offset;
                    break;
                case 2://end
                    fi[fileno].memoryStream.Seek(offset, SeekOrigin.End);
                    //fi[fileno].ptr = fi[fileno].dat.Length + offset;
                    fi[fileno].ptr = (int)(fi[fileno].memoryStream.Length + offset);
                    break;
                default:
                    throw new NotImplementedException();
            }
            reg.D[0] = (uint)fi[fileno].ptr;

        }


        private void malloc()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF48 malloc");
            UInt32 bytesize = mem.PeekL(reg.A[7] + 0);

            int ptr = memMng.Malloc(bytesize + 16);

            if (ptr < 0)
            {
                reg.D[0] = 0x8100_0000 + bytesize + 16;//確保できない
                reg.D[0] = 0x8200_0000;//待ったく確保できない
                return;
            }

            reg.D[0] = (uint)(ptr + 16);
        }

        private void mfree()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF49 mfree");
            UInt32 memptr = mem.PeekL(reg.A[7] + 0);

            int ret = memMng.Mfree(memptr-16);

            reg.D[0] = (uint)ret;
        }

        private void setblock()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF4A setblock");
            UInt32 newlen = mem.PeekL(reg.A[7] + 4);
            UInt32 newptr = mem.PeekL(reg.A[7] + 0);
            bool ret = memMng.Change(newptr, newlen);

            if (!ret)
            {
                reg.D[0] = 0x8100_0000 + newlen;//確保できない
                reg.D[0] = 0x8200_0000;//待ったく確保できない
                return;
            }

            reg.D[0] = 0x0000_0000;// + newlen;
        }

        private void exec()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF4B exec");
            ushort md = mem.PeekW(reg.A[7] + 0);
            UInt32 fil = mem.PeekL(reg.A[7] + 2);
            UInt32 p1 = mem.PeekL(reg.A[7] + 6);
            UInt32 p2 = mem.PeekL(reg.A[7] + 10);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(fil + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            msg = new List<byte>();
            cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(p1 + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string op = enc.GetStringFromSjisArray(msg.ToArray());

            switch (md)
            {
                case 0:
                    Log.WriteLine(LogLevel.Trace2, "<NiseHuman>in:  md:0 fil:{0} op:{1} p2:{2:X08} ", fn, op, p2);
                    if (fn.ToUpper() != "ZMC")
                    {
                        throw new NotImplementedException();//ZMC以外受け付けない!
                    }

                    //TBD
                    reg.D[0] = 0x0000_0000;//プロセス終了コード
                    reg.D[1] = 0x0000_0000;//エラーコード

                    break;
                case 2:
                    Log.WriteLine(LogLevel.Trace2, "<NiseHuman>in:  md:2 fil:{0} p1:{1:X08} p2:{2:X08} ", fn, p1, p2);
                    cnt = 0;
                    int cnt2 = 0;
                    bool o = false;
                    while (true)
                    {
                        byte b = mem.PeekB((UInt32)(fil + cnt));
                        if (o)
                        {
                            mem.PokeB((UInt32)(p1 + cnt2), b);
                            cnt2++;
                        }
                        if ((char)b == '\0') break;
                        if ((char)b == ' ')
                        {
                            if (!o) mem.PokeB((UInt32)(fil + cnt), 0);
                            o = true;
                        }
                        cnt++;
                    }

                    msg = new List<byte>();
                    cnt = 0;
                    do
                    {
                        byte b = mem.PeekB((UInt32)(fil + cnt));
                        if ((char)b == '\0') break;
                        msg.Add(b);
                        cnt++;
                    } while (true);
                    fn = enc.GetStringFromSjisArray(msg.ToArray());
                    msg = new List<byte>();
                    cnt = 0;
                    do
                    {
                        byte b = mem.PeekB((UInt32)(p1 + cnt));
                        if ((char)b == '\0') break;
                        msg.Add(b);
                        cnt++;
                    } while (true);
                    op = enc.GetStringFromSjisArray(msg.ToArray());
                    Log.WriteLine(LogLevel.Trace2, "<NiseHuman>out: md:2 fil:{0} op:{1} p2:{2:X08} ", fn, op, p2);

                    reg.D[0] = 0x0000_0000;
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        private void exit2()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF4C exit2");
            UInt16 rc = mem.PeekW(reg.A[7] + 0);

            programTerminate = true;
            returnCode = (int)rc;
        }

        private void files()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF4E files");
            UInt32 filbuf = mem.PeekL(reg.A[7] + 0);
            UInt32 nameptr = mem.PeekL(reg.A[7] + 4);
            UInt16 atr = mem.PeekW(reg.A[7] + 8);

            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(nameptr + cnt));
                if ((char)b == '\0') break;
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            Log.WriteLine(LogLevel.Trace2, string.Format("Filename:[{0}] Atr:{1:x04}", fn, atr));
            //string physicalFn = GetPhysicalFn(fn);

            reg.D[0] = unchecked((uint)-1);
            //if (File.Exists(physicalFn))
            if (fileMng.ExistsFile(fn))
            {
                mem.PokeB(filbuf + 0, 0);//ATR(sys)
                mem.PokeB(filbuf + 1, 0);//DriveNo(sys)
                mem.PokeW(filbuf + 2, 0);//DirCls(sys)
                mem.PokeW(filbuf + 4, 0);//DirFAT(sys)
                mem.PokeW(filbuf + 6, 0);//DirSec(sys)
                mem.PokeW(filbuf + 8, 0);//DirPos(sys)
                for (uint i = 0; i < 8; i++)
                    mem.PokeB(filbuf + 10 + i, 0x20);//FileName
                for (uint i = 0; i < 3; i++)
                    mem.PokeB(filbuf + 18 + i, 0x20);//EXT
                mem.PokeB(filbuf + 21, (byte)atr);//ATR
                mem.PokeW(filbuf + 22, 0x0000);//TIME
                mem.PokeW(filbuf + 24, 0x0000);//DATE
                //FileInfo fi = new FileInfo(fn);
                mem.PokeL(filbuf + 26, (uint)fn.Length);//FileLength
                for (uint i = 0; i < 23; i++)
                    mem.PokeB(filbuf + 30 + i, 0x00);//PACKEDNAME

                reg.D[0] = 0x0000_0000;//負の場合はエラー
            }
        }

        private void getpsp()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF51 getpsp");
            //reg.D[0] = (uint)(currentProc.startAddress - 0xf0);
            reg.D[0] = (uint)(beforePSP + 0x10);//環境変数のアドレス
        }

        private void filedate()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF87 filedate");
            UInt16 fileno = mem.PeekW(reg.A[7] + 0);
            UInt32 datetime = mem.PeekL(reg.A[7] + 2);

            if (datetime == 0)
            {
                //日付時刻の読み出し
                reg.SetDl(0, fi[fileno].datetime);
                return;
            }
            else
            {
                //日付時刻の設定
                fi[fileno].datetime = datetime;
            }

            reg.D[0] = 0;
        }

        private void maketmp()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FF8A _MAKETMP");
            UInt32 nameptr = mem.PeekL(reg.A[7] + 0);
            UInt16 atr = mem.PeekW(reg.A[7] + 4);
            List<byte> msg = new List<byte>();
            int cnt = 0;
            do
            {
                byte b = mem.PeekB((UInt32)(nameptr + cnt));
                if ((char)b == '\0') break;
                if ((char)b == '?')
                {
                    b = (byte)'0';
                    mem.PokeB((UInt32)(nameptr + cnt), b);
                }
                msg.Add(b);
                cnt++;
            } while (true);
            string fn = enc.GetStringFromSjisArray(msg.ToArray());
            Log.WriteLine(LogLevel.Trace2, string.Format("SRC Filename:[{0}] Atr:{1:x04}", fn, atr));

            // ???? を数字に置き換え、指定パスに存在しないファイル名であることを確認する
            //TBD

            //空いているファイル情報を見つける
            fileHandle = -1;
            for (int i = 0; i < fi.Length; i++)
            {
                if (fi[i] != null) continue;

                fi[i] = new FileIni();

                fileHandle = i;
                fi[i].IsTemp = true;
                fi[i].IsOpen = true;
                fi[i].filename = fn;
                fi[i].ptr = 0;
                fi[i].memoryStream = new MemoryStream();
                break;
            }
            if (fileHandle < 0)
            {
                reg.D[0] = 0xffff_ffff;
                return;
            }

            reg.D[0] = (uint)fileHandle;

        }

        private void s_malloc()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FFAD s_malloc");
            UInt16 md = mem.PeekW(reg.A[7] + 0);
            UInt32 len = mem.PeekL(reg.A[7] + 2);

            int ptr = memMng.Malloc(len + 16);

            if (ptr < 0)
            {
                reg.D[0] = 0x8100_0000 + len + 16;//確保できない
                reg.D[0] = 0x8200_0000;//待ったく確保できない
                return;
            }

            reg.D[0] = (uint)(ptr + 16);
        }

        private void s_mfree()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FFAE s_mfree");
            UInt32 memptr = mem.PeekL(reg.A[7] + 0);

            int ret = memMng.Mfree(memptr-16);

            reg.D[0] = (uint)ret;
        }

        private void bus_err()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>dos call $FFF7 bus_err");

            UInt32 sadr = mem.PeekL(reg.A[7] + 0);
            UInt32 dadr = mem.PeekL(reg.A[7] + 4);
            UInt16 md = mem.PeekW(reg.A[7] + 8);

            switch (md)
            {
                case 1: //byte
                    //MIDI IF isr(src): $eafa05
                    //MIDI IF icr(dst): $eafa07
                    //MIDI IF isr(src): $eafa15
                    //MIDI IF icr(dst): $eafa17
                    break;
                case 2: //word
                    break;
                case 4: //long
                    break;
                default:
                    throw new NotImplementedException();
            }

            reg.D[0] = 0x0000_0000;//0:読み書き可能 1,2,-1:エラー
        }

        private void _LTOS()
        {
            Log.WriteLine(LogLevel.Trace2, "<NiseHuman>FE func call $FE11 _LTOS");

            Int32 val = (Int32)reg.GetDl(0);
            UInt32 dadr = reg.A[0];
            byte[] dat=enc.GetSjisArrayFromString(string.Format("{0:d}", val));
            foreach(byte d in dat)
            {
                mem.PokeB(dadr++, d);
            }
            mem.PokeB(dadr, 0);
            reg.A[0] = dadr;
        }
    }
}