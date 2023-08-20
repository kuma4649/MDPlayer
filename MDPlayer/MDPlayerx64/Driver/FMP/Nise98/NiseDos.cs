using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class NiseDos
    {
        private Register286 regs;
        private Memory98 mem;
        private short paragraphsSize;
        private short paragraphsSizeSeg;
        private short freeBlockSeg;
        private const int SYSVARSStartAddress = 0x10100;
        private const int FCBStartAddress = 0x10200;
        private const int MCBStartAddress = 0x10300;
        private const int PSPStartAddress = 0x20000;
        private myEncoding enc;
        private int InDOSFLAGAdr = 0x11000;

        public class filestatus
        {
            public string name = "";
            public int ptr = 0;
            public int size = 0;
            public string path = "";
            public int handle = 0;
            public int mode = 0;
            public List<byte> lstBuf=new List<byte>();
        }
        private List<filestatus> files = new List<filestatus>();
        private int fileHandler = 10;
        private string filePath = "";

        private int allocateMemStartAdress = 0x9_0000;
        private int allocateMemSize;

        public byte returnCode { get; private set; } = 0x00;
        public bool programTerminate { get; set; } = false;

        public NiseDos(Register286 regs, Memory98 mem)
        {
            enc = new myEncoding();
            this.regs = regs;
            this.mem = mem;

            MakeSYSVARS();

            mem.PokeW(InDOSFLAGAdr, 1);//0:使用可! 1:常駐プログラムはシステムコール使用不可!!

            //mem.PokeW(0xfd802, 0x2a27);//EPSON機!!
            //mem.PokeB(0xfd804, 6);//EPSON PC-286VE
        }

        public void LoadAndExecuteFile(string filename, string option = "", int startSegment = PSPStartAddress >> 4)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.INFO, "niseDOS>{0} {1}", filename, option);

            byte[] bin = File.ReadAllBytes(filename);
            string fext = Path.GetExtension(filename).ToUpper();
            LoadRunner(bin, fext == ".COM", option, startSegment);

            //Set FCB
            //0x00 next FCB
            //0x04 number of files
            //0x06 実際の情報が記録されている位置
            //     +0x11 file size (dword)
            mem.PokeB(FCBStartAddress + 0x06 + 0x11 + 0, (byte)bin.Length);
            mem.PokeB(FCBStartAddress + 0x06 + 0x11 + 1, (byte)(bin.Length >> 8));
            mem.PokeB(FCBStartAddress + 0x06 + 0x11 + 2, (byte)(bin.Length >> 16));
            mem.PokeB(FCBStartAddress + 0x06 + 0x11 + 3, (byte)(bin.Length >> 24));

            //     +0x20 11byteのfilename パス無し、ピリオド無し、空白によるパディングあり
            int cnt = 0;
            string fn = Path.GetFileName(filename).ToUpper();
            string[] fns = fn.Split('.');
            fns[0] = fns[0].PadRight(8, ' ').Substring(0, 8);
            fns[1] = fns[1].PadRight(3, ' ').Substring(0, 3);
            fn = fns[0] + fns[1];
            foreach (Char c in fn)
            {
                mem.PokeB(FCBStartAddress + 0x06 + 0x20 + cnt, (byte)c);
                cnt++;
                if (cnt == 11) break;
            }

            int PSPAdr = startSegment<<4;

            //PSP直前に存在するMCB
            mem.PokeB(PSPAdr - 0x10 + 0, (byte)'Z');
            mem.PokeW(PSPAdr - 0x10 + 1, PSPStartAddress >> 4);
            mem.PokeW(PSPAdr - 0x10 + 3, unchecked((short)0xffff));

            //環境変数のセグメント0x0100 -> 実アドレス0x0_1000(適当)
            int envPtr = 0x0_1000;
            mem.PokeW(PSPAdr + 0x2c, (short)(envPtr >> 4));
            byte[] env = new byte[] { (byte)'P', (byte)'V', (byte)'I', (byte)'=', (byte)'.', 0, 0, 1, 0 };
            int p = 0;
            foreach (byte c in env)
            {
                mem.PokeB(envPtr + p++, c);
            }

        }

        public void MakeDummyMCB()
        {
            mem.PokeB(MCBStartAddress + 0x00, (byte)'M');//member of a MCB chain, (not last)
            mem.PokeW(MCBStartAddress + 0x01, (PSPStartAddress >> 4));//free PSP segment address of MCB owner (Process Id)
            mem.PokeW(MCBStartAddress + 0x03, 0);//このmcbのサイズ
            mem.PokeW(MCBStartAddress + 0x10, 0x20cd - 1);//0x20cd以外がほしいらしい

            mem.PokeW(0x0_0000 + 0x00ba, (PSPStartAddress >> 4));

            //環境変数のセグメント0x0100 -> 実アドレス0x0_1000(適当)
            int envPtr = MCBStartAddress + 0x10;
            byte[] env = new byte[] { (byte)'P', (byte)'V', (byte)'I', (byte)'=', (byte)'.', 0, 0, 1, 0 };
            int p = 0;
            foreach (byte c in env)
            {
                mem.PokeB(envPtr + p++, c);
            }

        }

        public void LoadImage(byte[] bin, int StartAdr)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>LoadImage");
            for (int i = 0; i < bin.Length; i++)
            {
                mem.PokeB(StartAdr + i, bin[i]);
            }
        }

        public void INT(byte imm8)
        {
            switch (imm8)
            {
                case 0x18:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>INT18h AH:${0:X02}", regs.AH);
                    INT18();
                    break;
                case 0x21:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>INT21h AH:${0:X02}", regs.AH);
                    INT21();
                    break;
                case 0x2f:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>INT2fh AX:${0:X04}", regs.AX);
                    INT2F();
                    break;
                default:

                    if (CheckHookINT(imm8))
                    {
                        HookINT(imm8);
                        return;
                    }

                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>INT{0:X02}h AH:${1:X02}", imm8, regs.AH);
                    int ptr = imm8 * 4;
                    short ip = (short)mem.PeekW(ptr);
                    short cs = (short)mem.PeekW(ptr + 2);
                    if ((ip | cs) == 0) break;

                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, regs.FLAG);
                    //regs.SP -= 2;
                    //mem.PokeW(regs.SS_SP, regs.DS);
                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, regs.CS);
                    regs.SP -= 2;
                    mem.PokeW(regs.SS_SP, regs.IP);
                    regs.IP = ip;
                    regs.CS = cs;
                    //regs.DS = cs;

                    break;
            }
        }

        private void LoadRunner(byte[] prog, bool IsCom, string option, int StartSegment = 0)
        {
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>LoadRunner");
            int ofs;
            int ptr = (StartSegment << 4);
            LoadImage(prog, ptr + 0x100);

            if (IsCom)
            {
                //Setup PSP http://programmer.main.jp/assembler2/7_5.html

                //0x80 引数の文字数
                if (string.IsNullOrEmpty(option))
                {
                    mem.PokeB(ptr + 0x80, (byte)0);
                }
                else
                {
                    byte[] optAry = enc.GetSjisArrayFromString(option + "\x0d");
                    mem.PokeB(ptr + 0x80, (byte)optAry.Length);

                    //0x81～0xff 引数実体
                    int i = 0;
                    foreach (byte b in optAry) mem.PokeB(ptr + 0x81 + i++, b);
                }

                regs.CS = regs.DS = (Int16)StartSegment;
                regs.IP = 0x100;
                programTerminate = false;
            }
            else
            {
                //exeのとき

                ptr += 0x100;
                Int16 signature = mem.PeekW(ptr + 0x00);
                int headerSize = mem.PeekW(ptr + 0x08) * 0x10;
                regs.SS = (short)(mem.PeekW(ptr + 0x0e) + StartSegment + 0x10);
                regs.SP = mem.PeekW(ptr + 0x10);
                regs.IP = mem.PeekW(ptr + 0x14);
                regs.CS = regs.DS = (short)(mem.PeekW(ptr + 0x16) + StartSegment + 0x10);
                regs.DS -= 0x10;
                int relocOfs = mem.PeekW(ptr + 0x18);
                int relocSize = headerSize - relocOfs;
                for (int i = 0; i < relocSize; i += 4)
                {
                    ushort rOfs = (ushort)mem.PeekW(ptr + relocOfs + i + 0);
                    ushort rSeg = (ushort)mem.PeekW(ptr + relocOfs + i + 2);
                    int rPtr = (rSeg << 4) + rOfs;
                    ushort val = (ushort)mem.PeekW(ptr + rPtr + headerSize);
                    mem.PokeW(ptr + rPtr + headerSize, (short)(ushort)(StartSegment + 0x10 + val));
                }
                for (int i = 0; i < prog.Length - headerSize; i++)
                {
                    mem.PokeB(ptr + i , prog[headerSize + i]);
                }
                ptr -= 0x100;

                //0x80 引数の文字数
                if (string.IsNullOrEmpty(option))
                {
                    mem.PokeB(ptr + 0x80, (byte)0);
                }
                else
                {
                    byte[] optAry = enc.GetSjisArrayFromString(option + "\x0d");
                    mem.PokeB(ptr + 0x80, (byte)optAry.Length);

                    //0x81～0xff 引数実体
                    int i = 0;
                    foreach (byte b in optAry) mem.PokeB(ptr + 0x81 + i++, b);
                }

            }
        }

        private void MakeSYSVARS()
        {
            //0x10100 

            //FCB Start Adr 0x10200
            mem.PokeW(SYSVARSStartAddress - 2, unchecked((Int16)(MCBStartAddress >> 4)));
            mem.PokeW(SYSVARSStartAddress + 4, unchecked((Int16)(FCBStartAddress & 0xf)));
            mem.PokeW(SYSVARSStartAddress + 6, unchecked((Int16)(FCBStartAddress >> 4)));

        }


        private void INT18()
        {
            List<byte> msg;
            string text;
            byte b = 0;
            int cnt;
            filestatus fnd;
            switch (regs.AH)
            {
                case 0x04:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  (98)KEY BOARD press check");
                    byte KeyGroup = regs.AL;
                    regs.AH = 0x00;//何も押されてないよ
                    break;
                default:
                    throw new NotImplementedException(string.Format("AH:${0:X02}", regs.AH));

            }
        }


        private void INT21()
        {
            List<byte> msg;
            string text;
            byte b = 0;
            int cnt;
            filestatus fnd;
            filestatus fs;
            string filename;

            switch (regs.AH)
            {
                case 0x02:
                    msg = new List<byte>();
                    b = regs.DL;
                    msg.Add(b);
                    text = enc.GetStringFromSjisArray(msg.ToArray());
                    log.Write(text);//通常のコンソール出力
                    break;
                case 0x09:
                    msg = new List<byte>();
                    cnt = 0;
                    do
                    {
                        b = mem.PeekB(regs.DS_DX + cnt);
                        if ((char)b == '$') break;
                        msg.Add(b);
                        cnt++;
                    } while (true);
                    text = enc.GetStringFromSjisArray(msg.ToArray());
                    log.Write(text);//通常のコンソール出力
                    break;
                case 0x19:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  Get Current Default Drive");
                    regs.AL = 2;//2 => C:ドライブ
                    break;
                case 0x25:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  SET INTERRUPT VECTOR");
                    mem.PokeW(regs.AL * 4 + 0, regs.DX);
                    mem.PokeW(regs.AL * 4 + 2, regs.DS);
                    break;
                case 0x30:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  DOS VERSION");
                    //major version
                    regs.AX = 4;//dos 4.x
                    break;
                case 0x31:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  TERMINATE BUT STAY RESIDENT");
                    returnCode = regs.AL;
                    paragraphsSize = regs.DX;
                    programTerminate = true;
                    break;
                case 0x34:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  GET ADDRESS OF INDOS FLAG");//https://fd.lod.bz/rbil/interrup/dos_kernel/2134.html#2778
                    regs.BX = (short)(InDOSFLAGAdr & 0xf);
                    regs.ES = (short)(InDOSFLAGAdr >> 4);
                    break;
                case 0x35:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  GET INTERRUPT VECTOR");
                    regs.BX = mem.PeekW(regs.AL * 4 + 0);
                    regs.ES = mem.PeekW(regs.AL * 4 + 2);
                    break;
                case 0x3c:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  Create File Using Handle");
                    short attribute = regs.CX;
                    msg = new List<byte>();
                    cnt = 0;
                    do
                    {
                        b = mem.PeekB(regs.DS_DX + cnt);
                        if ((char)b == '\0') break;
                        msg.Add(b);
                        cnt++;
                    } while (true);

                    filename = enc.GetStringFromSjisArray(msg.ToArray());
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, filename);

                    fs = new filestatus();
                    files.Add(fs);
                    fs.name = Path.GetFileName(filename);
                    fs.ptr = 0;
                    fs.path = Path.GetDirectoryName(filename);
                    fs.handle = fileHandler++;
                    fs.mode = 1;
                    fs.lstBuf = new List<byte>();
                    SetPath(fs.path);
                    MakeDummyMCB();

                    regs.CF = false;
                    regs.AX = (short)fs.handle;//file handle

                    break;
                case 0x3d:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  FILE OPEN");
                    msg = new List<byte>();
                    cnt = 0;
                    do
                    {
                        b = mem.PeekB(regs.DS_DX + cnt);
                        if ((char)b == '\0') break;
                        msg.Add(b);
                        cnt++;
                    } while (true);

                    filename = enc.GetStringFromSjisArray(msg.ToArray());
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, filename);

                    if (CheckFileExist(filename,out string fndFilename))
                    {
                        regs.CF = false;
                        fs = new filestatus();
                        files.Add(fs);
                        fs.name = Path.GetFileName(fndFilename);
                        fs.ptr = 0;
                        fs.path = Path.GetDirectoryName(fndFilename);
                        fs.handle = fileHandler++;
                        SetPath(fs.path);
                        regs.AX = (short)fs.handle;//file handle
                        MakeDummyMCB();
                    }
                    else
                    {
                        regs.CF = true;
                        regs.AX = 1;
                    }
                    break;
                case 0x3e:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  FILE CLOSE handle={0:X02}",regs.BX);
                    fnd = SearchFileStatus(regs.BX);
                    regs.CF = true;
                    try
                    {
                        if (fnd != null)
                        {
                            files.Remove(fnd);
                            if (fnd.mode == 1)
                            {
                                File.WriteAllBytes(Path.Combine(fnd.path, fnd.name), fnd.lstBuf.ToArray());
                            }
                            regs.CF = false;
                        }
                    }
                    catch
                    {
                        regs.CF = true;
                    }
                    break;
                case 0x3f:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  FILE READ handle={0:X02}",regs.BX);

                    fnd = SearchFileStatus(regs.BX);
                    if (fnd == null)
                    {
                        regs.CF = true;
                        break;
                    }
                    if (string.IsNullOrEmpty(fnd.name))
                    {
                        regs.CF = true;
                        break;
                    }

                    byte[] buf = ReadAllByte(fnd);
                    fnd.size = buf.Length;
                    int size = Math.Min((ushort)regs.CX, buf.Length-fnd.ptr);
                    byte[] rbuf = new byte[size];
                    Array.Copy(buf, fnd.ptr, rbuf, 0, size);
                    LoadImage(rbuf, regs.DS_DX);
                    fnd.ptr += size;

                    //return
                    regs.AX = (short)size;
                    regs.CF = false;

                    break;
                case 0x40:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  'WRITE'-WRITE TO FILE OR DEVICE");
                    //入力:
                    //BX = file handle
                    //CX = number of bytes to write
                    //DS: DX->data to write

                    fnd = SearchFileStatus(regs.BX);
                    if (fnd == null)
                    {
                        msg = new List<byte>();
                        int c = 0;
                        while (c < regs.CX)
                        {
                            b = mem.PeekB(regs.DS_DX + c);
                            msg.Add(b);
                            c++;
                        }
                        text = enc.GetStringFromSjisArray(msg.ToArray());
                        log.Write(text);//通常のコンソール出力

                        //出力:
                        //ERROR
                        regs.CF = true;
                        break;
                    }

                    if (fnd.mode == 1)
                    {
                        int c = 0;
                        while (c < regs.CX)
                        {
                            b = mem.PeekB(regs.DS_DX + c);
                            fnd.lstBuf.Add(b);
                            c++;
                        }
                        Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  WRITE buff length:{0:d}", c);
                        regs.CF = false;
                        break;
                    }

                    //出力:
                    //ERROR
                    regs.CF = true;
                    break;
                case 0x42:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  SEEK FILE POINTER handle={0:X02}", regs.BX);
                    fnd = SearchFileStatus(regs.BX);
                    if (fnd == null)
                    {
                        regs.CF = true;
                        break;
                    }

                    int d = (regs.CX << 4) + regs.DX;
                    if (regs.AL == 0) fnd.ptr = d;
                    else if (regs.AL == 1) fnd.ptr += d;
                    else if (regs.AL == 2) fnd.ptr = fnd.size - 1+d;
                    regs.CF= false;
                    regs.DX = (short)((fnd.ptr >> 4) & 0xf000);
                    regs.AX = (short)(fnd.ptr & 0xffff);
                    break;
                case 0x43:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  Get/Set File Attributes");
                    break;
                case 0x47:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  Get Current Directory DL={0:X02} DS:SI[{1:X04}:{2:X04}]", regs.DL, regs.DS, regs.SI);
                    regs.CF = false;
                    mem.PokeB(regs.DS_SI, (byte)'.');
                    mem.PokeB(regs.DS_SI + 1, (byte)0x00);
                    break;
                case 0x48:
                    regs.CF = false;
                    regs.AX = (short)(allocateMemStartAdress / 16);
                    regs.BX = regs.BX;
                    allocateMemSize = regs.BX * 16;
                    allocateMemStartAdress += allocateMemSize;
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  Allocate Memory AX(allocatedStartSeg)={1:X04} BX(paragraphs size)={0:X04}", regs.BX, regs.AX);
                    break;
                case 0x49:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  FREE MEMORY");//https://fd.lod.bz/rbil/interrup/dos_kernel/2149.html#sect-2975
                    regs.CF = false;
                    freeBlockSeg = regs.ES;
                    break;
                case 0x4A:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  RESIZE MEMORY BLOCK");// https://fd.lod.bz/rbil/interrup/dos_kernel/214a.html
                    regs.CF = false;
                    paragraphsSize = regs.BX;
                    paragraphsSizeSeg = regs.ES;
                    break;
                case 0x4c:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  TERMINATE WITH RETURN CODE");// https://fd.lod.bz/rbil/interrup/dos_kernel/214c.html#sect-3014
                    returnCode = regs.AL;
                    programTerminate = true;
                    break;
                case 0x52:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  SYSVARS");
                    regs.ES = 0x1000;
                    regs.BX = 0x0100;
                    break;
                case 0x57:
                    Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "<NiseDos>  Get/Set File Date and Time Using Handle");
                    if (regs.AL == 0x00)
                    {
                        regs.CX = 0b00000_000000_00000;//hh5bit_mm6bit_ss5bit
                        regs.DX = 0b0000000_0000_00000;//yy7bit_MM4bit_dd5bit
                    }
                    else
                    {
                        ;
                    }
                    break;
                default: 
                    throw new NotImplementedException(string.Format("AH:${0:X02}", regs.AH));

            }
        }

        private void INT2F()
        {
            if (regs.AX == 0x1600)
            {
                //major version
                regs.AL = 0;
                //minor version
                regs.AH = 0;
                return;
            }

            if (regs.AX == 0x4300)
            {
                regs.AL = 0x80;//XMS ドライバ有り
                return;
            }
            if (regs.AX == 0x4310)
            {
                regs.ES = unchecked((short)0xe000);
                regs.BX = 0x0000;
                return;
            }
        }



        private Dictionary<byte, Action> dicHookINT=new Dictionary<byte, Action>();

        public void SetHookINT(byte intnum, Action action)
        {
            if (dicHookINT.ContainsKey(intnum))
                dicHookINT.Remove(intnum);

            dicHookINT.Add(intnum, action);
        }

        private bool CheckHookINT(byte imm8)
        {
            if (dicHookINT.ContainsKey(imm8)) return true;
            return false;
        }

        private void HookINT(byte imm8)
        {
            dicHookINT[imm8]();
        }

        public void SetPath(string v)
        {
            filePath = v;
        }

        private byte[] ReadAllByte(filestatus fs)
        {
            string fn = Path.Combine(fs.path, fs.name);
            return File.ReadAllBytes(fn);
        }

        private bool CheckFileExist(string filename,out string fndFilename)
        {
            if(File.Exists(filename))
            {
                fndFilename = filename;
                return true;
            }
            string fn = Path.Combine(filePath, filename);
            if (File.Exists(fn))
            {
                fndFilename = fn;
                return true;
            }

            fndFilename = "";
            return false;
        }

        public byte[] LoadData(string fn)
        {
            fn = Path.Combine(filePath, fn);
            return File.ReadAllBytes(fn);
        }

        public filestatus SearchFileStatus(int handle)
        {
            foreach (filestatus fs in files)
            {
                if (fs.handle == regs.BX)
                {
                    return fs;
                }
            }
            return null;

        }

    }
}