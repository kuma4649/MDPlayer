using MDPlayer.Driver.FMP.Nise98;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class NisePPZ8
    {
        private Register286 regs;
        private Memory98 mem;
        private NiseDos dos;
        private Nise286 cpu;
        private Nise98 nise98;

        private const byte ppz8Int = 0x7f;
        private const short ppz8EntryAddressSeg = 0x4000;
        private const short ppz8EntryAddressOfs = 0x0000;

        private const short ppz8IDOfs = 0x0005;
        private const short ppz8VerOfs = 0x000a;

        private const short ppz8FIFOAddressOfs = 0x0F00;//FIFO処理へのオフセット
        private const short ppz8ReleaseOfs = 0x1000;
        private const short ppz8ReleaseMessageOfs = 0x1001;

        private myEncoding enc;
        //private int temporarySeg;
        //private int temporarySize;
        private byte emuADPCM;
        private Action<int, int, byte[][]> setPPZ8PCMData;
        private Action<int, int, int> setPPZ8Data;
        private byte[][] pcmData = new byte[2][];

        public NisePPZ8(Nise98 nise98)
        {
            this.regs = nise98.GetRegisters();
            this.mem = nise98.GetMem();
            this.dos = nise98.GetDos();
            this.cpu = nise98.GetCPU();
            this.nise98 = nise98;

            mem.PokeW(ppz8Int * 4 + 0, ppz8EntryAddressOfs);//ofs
            mem.PokeW(ppz8Int * 4 + 2, ppz8EntryAddressSeg);//seg

            //ID
            int ptr = (ppz8EntryAddressSeg << 4) + ppz8IDOfs;
            mem.PokeB(ptr + 0x00, (byte)'P');
            mem.PokeB(ptr + 0x01, (byte)'P');
            mem.PokeB(ptr + 0x02, (byte)'Z');
            mem.PokeB(ptr + 0x03, (byte)'8');
            mem.PokeB(ptr + 0x04, (byte)0);

            //バージョン
            ptr = (ppz8EntryAddressSeg << 4) + ppz8VerOfs;
            mem.PokeB(ptr + 0x00, (byte)'1');
            mem.PokeB(ptr + 0x01, (byte)'.');
            mem.PokeB(ptr + 0x02, (byte)'0');
            mem.PokeB(ptr + 0x03, (byte)'7');

            //常駐解除メッセージ
            enc = new myEncoding();
            byte[] bmsg = enc.GetSjisArrayFromString("ＰＰＺ８の常駐を解除しました。\r\n$");
            ptr = (ppz8EntryAddressSeg << 4) + ppz8ReleaseMessageOfs;
            foreach (byte ch in bmsg)
            {
                mem.PokeB(ptr, ch);
                ptr++;
            }

            dos.SetHookINT(ppz8Int, INT7F);
            cpu.SetHook(Hook);
        }

        public void FMPRegistPPZ8(out int step, out Register286 regs)
        {
            regs = nise98.GetRegisters();
            regs.AX = 0x0010;
            Log.WriteLine(musicDriverInterface.LogLevel.DEBUG, "FMPRegistPPZ8");
            step = 0;
            regs.DS = ppz8EntryAddressSeg;//'PPZ8'のseg
            regs.SI = ppz8IDOfs;//'PPZ8'のofs
            regs.DX = ppz8ReleaseOfs;//常駐解除時のFarコール
            regs.CL = 0x00;//TASK_ASIN
            nise98.CallRunfunctionCall(0xd2, true, true, true, 10_000_000_000, 0_000);

            Log.Write(musicDriverInterface.LogLevel.INFO, "偽PPZ8をFMPタスクへ設定しました。\r\n");
        }

        public void INT7F()
        {
            switch (regs.AH)
            {
                case 0x00://初期化
                    //ワークの初期化
                    setPPZ8Data(0, 0, 0);
                    setPPZ8Data(10, 0, 1);
                    for (int i = 0; i < 8; i++)
                    {
                        setPPZ8Data(21, i, 16000);
                        //setPPZ8Data(0x0e + i * 0x100, 0xffff, 0xffff);
                    }
                    break;
                case 0x01://KEY ON PCM
                    setPPZ8Data(1, regs.AL, regs.DX);
                    break;
                case 0x02://KEY OFF PCM
                    setPPZ8Data(2, regs.AL, 0);
                    break;
                case 0x03:
                    List<byte> lstFN = new List<byte>();
                    int ptr = regs.DS_DX;
                    do
                    {
                        byte c = mem.PeekB(ptr++);
                        if (c == 0) break;
                        lstFN.Add(c);
                    } while (true);
                    string fn = enc.GetStringFromSjisArray(lstFN.ToArray());
                    bool refEnv = regs.AL == 0;
                    int pcmBufNum = regs.CL;
                    bool pcmIsPVI = regs.CH == 0;
                    pcmData[pcmBufNum] = dos.LoadData(fn);
                    setPPZ8PCMData(pcmBufNum, pcmIsPVI ? 0 : 1, pcmData);
                    regs.CF = false;
                    break;
                case 0x04:
                    switch (regs.AL)
                    {
                        case 0x09:
                            regs.ES = ppz8EntryAddressSeg;
                            regs.BX = ppz8FIFOAddressOfs;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    setPPZ8Data?.Invoke(4, regs.AL, 0);
                    break;
                case 0x07://change volume
                    setPPZ8Data(7, regs.AL, (int)Math.Min((ushort)regs.DX, (ushort)15));// / (emuADPCM != 0 ? 16 : 1));
                    break;
                case 0x0a://ADPCM volume adjust
                    setPPZ8Data(10, 0, regs.DX);
                    break;
                case 0x0b://change PCM FNUM
                    setPPZ8Data(11, regs.AL, ((ushort)regs.DX << 16) + (ushort)regs.CX);
                    break;
                case 0x13://change pan
                    setPPZ8Data(19, regs.AL, regs.DX);
                    break;
                case 0x15://元データ周波数設定
                    setPPZ8Data(21, regs.AL, regs.DX);
                    break;
                case 0x16:
                    setPPZ8Data(22, 0, 15);// regs.AL);
                    break;
                case 0x17:
                    //temporarySeg = (ushort)regs.ES;
                    //temporarySize = (ushort)regs.DX;
                    break;
                case 0x18:
                    setPPZ8Data(24, regs.AL, 0);
                    emuADPCM = regs.AL;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public bool Hook()
        {
            if (regs.CS != ppz8EntryAddressSeg) return false;

            bool Cancel = false;
            switch (regs.IP)
            {
                case ppz8ReleaseOfs:
                    regs.DX = ppz8ReleaseMessageOfs;
                    Cancel = true;
                    break;
                case ppz8FIFOAddressOfs:
                    //FIFO処理
                    //オリジナルはEMS/XMSのデータ転送処理を行っている
                    //TBD
                    Cancel = true;
                    break;
            }


            if (Cancel)
            {
                regs.IP = mem.PeekW(regs.SS_SP);
                regs.SP += 2;
                regs.CS = mem.PeekW(regs.SS_SP);
                regs.SP += 2;
                return true;
            }
            Log.WriteLine(musicDriverInterface.LogLevel.ERROR, "[NisePPZ8]不明なアドレスが参照されています。IP:{0:X04}", regs.IP);
            return false;
        }

        public void SetCallBack(Action<int, int, byte[][]> setPPZ8PCMData, Action<int, int, int> setPPZ8Data)
        {
            this.setPPZ8PCMData = setPPZ8PCMData;
            this.setPPZ8Data = setPPZ8Data;
        }

    }
}
