using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace MDPlayer.Driver.MUCOM88
{
    public class MUCOM88 : baseDriver
    {
        private string fnVoicedat = "";
        private string fnPcm = "";
        private List<Tuple<string, string>> tags = null;
        private byte[] pcmdata = null;
        public string PlayingFileName = "";



        /// <summary>
        /// 曲情報取得
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="vgmGd3"></param>
        /// <returns></returns>
        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            if (CheckFileType(buf) != enmMUCOMFileType.MUC)
            {
                throw new NotImplementedException();
            }

            tags = GetTagsFromMUC(buf);
            GD3 gd3 = new GD3();
            foreach (Tuple<string, string> tag in tags)
            {
                switch (tag.Item1)
                {
                    case "title":
                        gd3.TrackName = tag.Item2;
                        gd3.TrackNameJ = tag.Item2;
                        break;
                    case "composer":
                        gd3.Composer = tag.Item2;
                        gd3.ComposerJ = tag.Item2;
                        break;
                    case "author":
                        gd3.VGMBy = tag.Item2;
                        break;
                    case "comment":
                        //gd3.Notes = tag.Item2;
                        break;
                    case "mucom88":
                        gd3.Version = tag.Item2;
                        gd3.Notes = tag.Item2;
                        break;
                    case "date":
                        gd3.Converted = tag.Item2;
                        break;
                    case "voice":
                        fnVoicedat = tag.Item2;
                        break;
                    case "pcm":
                        fnPcm = tag.Item2;
                        break;
                }
            }

            return gd3;
        }

        /// <summary>
        /// イニシャライズ
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="chipRegister"></param>
        /// <param name="model"></param>
        /// <param name="useChip"></param>
        /// <param name="latency"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public override bool init(byte[] buf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
        {
            this.vgmBuf = buf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            GD3 = getGD3Info(buf, 0);
            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;

            pc88.ChipRegister = chipRegister;
            pc88.fmTimer = timerOPN;

#if DEBUG
            //実チップスレッドは処理をスキップ(デバッグ向け)
            //if (model == enmModel.RealModel) return true;
#endif

            fnVoicedat = string.IsNullOrEmpty(fnVoicedat) ? "voice.dat" : fnVoicedat;
            LoadFMVoice(fnVoicedat);

            fnPcm = string.IsNullOrEmpty(fnPcm) ? "mucompcm.bin" : fnPcm;
            pcmdata = LoadPCM(fnPcm);

            //Compile
            ushort basicsize = StoreBasicSource(buf, 1, 1);

            //MUCOM88 初期化
            muc88.CINT();//0x9600

            //コンパイルコマンドのセット
            z80.HL = 0xf010;
            mem.LD_8(0xf010, 0x41);// 'A'
            mem.LD_8(0xf011, 0x00);
            mem.LD_8(0xf012, 0x00);

            //↓コンパイルが実施される
            int ret = muc88.COMPIL();//vector 0xeea8

            //エラー発生時は0以外が返る
            if (ret != 0)
            {
                int errLine = mem.LD_16(0x0f32e);//ワークアドレスのERRLINE
                log.Write(string.Format("コンパイル時にエラーが発生したみたい(errLine:{0})", errLine));
                return false;
            }

            SaveMub(basicsize);

            setPCMData(pcmdata);
            music2.initMusic2();
            music2.MSTART();

            return true;
        }

        public override void oneFrameProc()
        {
#if DEBUG
            //実チップスレッドは処理をスキップ(デバッグ向け)
            //if (model == enmModel.RealModel)
            //{
            //    Stopped = true;
            //    return;
            //}
#endif

            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;

                    timerOPN.timer();
                    if ((timerOPN.ReadStatus() & 3) != 0)
                    {
                        //mucom88
                        ;
                        music2.PL_SND();
                    }
                    Counter++;
                    vgmFrameCounter++;
                }

                Stopped = !IsPlaying();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private ushort[] soundWorkTbl = new ushort[]
        {
            ver1_0.music2.CH1DAT, //FM Ch.1
            ver1_0.music2.CH2DAT, //FM Ch.2
            ver1_0.music2.CH3DAT, //FM Ch.3
            ver1_0.music2.CH4DAT, //SSG Ch.1
            ver1_0.music2.CH5DAT, //SSG Ch.2
            ver1_0.music2.CH6DAT, //SSG Ch.3
            ver1_0.music2.CHADAT, //FM Ch.4
            ver1_0.music2.CHBDAT, //FM Ch.5
            ver1_0.music2.CHCDAT, //FM Ch.6
            ver1_0.music2.DRAMDAT, //RHYTHM Ch.
            ver1_0.music2.PCMDAT //ADPCM Ch.
        };

        public bool IsPlaying()
        {
            long loop = long.MaxValue;
            bool loopSw = false;
            bool flg = false;

            if (music2.loopCounter == null)
            {
                vgmCurLoop = 0;
                return false;
            }

            for (int i = 0; i < soundWorkTbl.Length; i++)
            {
                if (music2.loopCounter[i] != -1)
                {
                    loop = Math.Min(music2.loopCounter[i], loop);
                    loopSw = true;
                }

                ushort soundDataAdr = mem.LD_16((ushort)(soundWorkTbl[i] + 2));
                ushort soundDataLoopAdr = mem.LD_16((ushort)(soundWorkTbl[i] + 4));
                byte soundData = mem.LD_8(soundDataAdr);

                if (soundDataLoopAdr != 0)
                {
                    //このパートはループ先アドレスがある(=ループするので演奏終了しない)
                    flg = true;
                }
                else
                {
                    if (soundData != 0)
                    {
                        //このパートは演奏中である
                        flg = true;
                    }
                }
            }

            vgmCurLoop = (uint)(loopSw ? (loop + 1) : 0);
            return flg;
        }

        public MUCOM88()
        {
            mucInit();
        }



        private ver1_1.expand expand = null;
        private ver1_0.errmsg errmsg = null;
        private ver1_1.msub msub = null;
        private ver1_1.muc88 muc88 = null;
        private ver1_0.ssgdat ssgdat = null;
        private ver1_0.time time = null;
        private ver1_0.smon smon = null;

        private ver1_0.music2 music2 = null;

        private Z80 z80 = null;
        private Mem mem = null;
        private PC88 pc88 = null;
        private MNDRV.FMTimer timerOPN;
        public const int baseclock =7987200;

        public enum enmMUCOMFileType
        {
            unknown,
            MUB,
            MUC
        }

        public void mucInit()
        {
            expand = new ver1_1.expand();
            errmsg = new ver1_0.errmsg();
            msub = new ver1_1.msub();
            muc88 = new ver1_1.muc88();
            ssgdat = new ver1_0.ssgdat();
            time = new ver1_0.time();
            smon = new ver1_0.smon();
            music2 = new ver1_0.music2();

            z80 = new Z80();
            mem = new Mem();
            pc88 = new PC88();

            expand.Mem = mem;
            expand.Z80 = z80;
            expand.PC88 = pc88;
            expand.msub = msub;
            expand.smon = smon;

            msub.Mem = mem;
            msub.Z80 = z80;
            msub.PC88 = pc88;

            muc88.Mem = mem;
            muc88.Z80 = z80;
            muc88.PC88 = pc88;
            muc88.msub = msub;
            muc88.expand = expand;
            muc88.smon = smon;

            time.Mem = mem;
            time.Z80 = z80;
            time.PC88 = pc88;

            smon.Mem = mem;
            smon.Z80 = z80;
            smon.PC88 = pc88;
            smon.msub = msub;
            smon.expand = expand;

            z80.Mem = mem;

            pc88.Mem = mem;
            pc88.Z80 = z80;
            pc88.ChipRegister = chipRegister;
            pc88.fmTimer = timerOPN;

            ssgdat.SetSSGDAT(mem);

            music2.Z80 = z80;
            music2.Mem = mem;
            music2.PC88 = pc88;

            timerOPN = new MNDRV.FMTimer(false, null, baseclock);

            //ほぼ意味なし
            muc88.CINT();
        }

        private enmMUCOMFileType CheckFileType(byte[] buf)
        {
            if (buf == null || buf.Length < 4)
            {
                return enmMUCOMFileType.unknown;
            }

            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x43
                && buf[3] == 0x38)
            {
                return enmMUCOMFileType.MUB;
            }

            return enmMUCOMFileType.MUC;
        }

        private List<Tuple<string, string>> GetTagsFromMUC(byte[] buf)
        {
            var text = Encoding.GetEncoding("shift_jis").GetString(buf)
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.IndexOf("#") == 0);
            List<Tuple<string, string>> tags = new List<Tuple<string, string>>();

            foreach (string v in text)
            {
                try
                {
                    int p = v.IndexOf(' ');
                    string tag = "";
                    string ele = "";
                    if (p >= 0)
                    {
                        tag = v.Substring(1, p).Trim().ToLower();
                        ele = v.Substring(p + 1).Trim();
                        Tuple<string, string> item = new Tuple<string, string>(tag, ele);
                        tags.Add(item);
                    }
                }
                catch { }
            }

            return tags;
        }

        private ushort StoreBasicSource(byte[] buf, int line, int add)
        {
            var text = Encoding.GetEncoding("shift_jis").GetString(buf)
                .Split(new string[] { "\r\n" }, StringSplitOptions.None);

            ushort mptr = 1;
            ushort linkptr = mptr;
            foreach (string txt in text)
            {
                byte[] data = Encoding.GetEncoding("shift_jis").GetBytes(txt.Replace("\x09", " "));
                linkptr = mptr;
                mptr += 2;
                mem.LD_16(mptr, (ushort)line);
                mptr += 2;
                mem.LD_8(mptr++, 0x3a);
                mem.LD_8(mptr++, 0x8f);
                mem.LD_8(mptr++, 0xe9);
                foreach (byte b in data) mem.LD_8(mptr++, b);
                mem.LD_8(mptr++, 0);
                mem.LD_16(linkptr, (ushort)mptr);
                line += add;
            }
            mem.LD_16(linkptr, (ushort)0);

            return mptr;
        }

        private void SaveMub(ushort basicsize)
        {
            byte[] textLineBuf = new byte[80];
            string msg;

            for (int i = 0; i < 80; i++) textLineBuf[i] = mem.LD_8((ushort)(0xf3c8 + i));
            log.Write(Encoding.GetEncoding("Shift_JIS").GetString(textLineBuf));

            ushort workadr = 0xf320;
            int fmvoice = mem.LD_8((ushort)(workadr + 50));
            int pcmflag = 0;
            int maxcount = 0;
            int mubsize = 0;

            log.Write(string.Format("Used FM voice:{0}", fmvoice));

            string strTcount = "";
            string strLcount = "";
            for (int i = 0; i < muc88.MAXCH[0]; i++)
            {
                int tcnt = mem.LD_16((ushort)(0x8c10 + i * 4));
                int lcnt = mem.LD_16((ushort)(0x8c12 + i * 4));
                if (lcnt != 0) { lcnt = tcnt - (lcnt - 1); }
                if (tcnt > maxcount) maxcount = tcnt;
                msg = Encoding.GetEncoding("Shift_JIS").GetString(new byte[] { (byte)(0x41 + i) });
                strTcount += string.Format("{0}:{1} ", msg, tcnt);
                strLcount += string.Format("{0}:{1} ", msg, lcnt);
            }

            if (mem.LD_16((ushort)(0x8c10 + 10 * 4)) == 0) pcmflag = 2;

            log.Write("[ Total count ]");
            log.Write(strTcount);
            log.Write("");
            log.Write("[ Loop count  ]");
            log.Write(strLcount);
            log.Write("");

            msg = Encoding.GetEncoding("Shift_JIS").GetString(textLineBuf, 31, 4);
            int start = Convert.ToInt32(msg, 16);
            msg = Encoding.GetEncoding("Shift_JIS").GetString(textLineBuf, 41, 4);
            int length = Convert.ToInt32(msg, 16);

            mubsize = length;

            log.Write(string.Format("#Data Buffer ${0:x04}-${1:x04} (${2:x04})", start, start + length - 1, length));
            log.Write(string.Format("#MaxCount:{0} Basic:${1:x04} Data:${2:x04}", maxcount, basicsize, mubsize));

            if (log.debug)
            {
                SaveMusic("test.mub", (ushort)start, (ushort)length, pcmflag);
            }
        }

        private int SaveMusic(string fname, ushort start, ushort length, int option)
        {
            //		音楽データファイルを出力(コンパイルが必要)
            //		filename     = 出力される音楽データファイル
            //		option : 1   = #タグによるvoice設定を無視
            //		         2   = PCM埋め込みをスキップ
            //		(戻り値が0以外の場合はエラー)
            //

            if (string.IsNullOrEmpty(fname)) return -1;

            List<byte> dat = new List<byte>();
            int footsize;
            footsize = 1;//かならず1以上

            int pcmptr = 0;
            int pcmsize = pcmdata.Length;
            bool pcmuse = ((option & 2) == 0);
            pcmdata = (!pcmuse ? null : pcmdata);
            pcmptr = (!pcmuse ? 0 : (32 + length + footsize));
            pcmsize = (!pcmuse ? 0 : pcmsize);
            if (pcmuse)
            {
                if (pcmdata == null || pcmsize == 0)
                {
                    pcmuse = false;
                    pcmdata = null;
                    pcmptr = 0;
                    pcmsize = 0;
                }
            }

            //if (infobuf)
            //{
            //    infobuf->Put((int)0);
            //    footer = infobuf->GetBuffer();
            //    footsize = infobuf->GetSize();
            //}

            dat.Add(0x4d);// M
            dat.Add(0x55);// U
            dat.Add(0x43);// C
            dat.Add(0x38);// 8
            dat.Add(32); //header size(32bit)
            dat.Add(0);
            dat.Add(0);
            dat.Add(0);
            dat.Add((byte)length);//data size(32bit)
            dat.Add((byte)(length >> 8));
            dat.Add((byte)(length >> 16));
            dat.Add((byte)(length >> 24));
            dat.Add((byte)(32 + length));//tagdata ptr(32bit)
            dat.Add((byte)((32 + length) >> 8));
            dat.Add((byte)((32 + length) >> 16));
            dat.Add((byte)((32 + length) >> 24));
            dat.Add(1);//tagdata size(32bit)
            dat.Add(0);
            dat.Add(0);
            dat.Add(0);
            dat.Add((byte)pcmptr);//pcmdata ptr(32bit)
            dat.Add((byte)(pcmptr >> 8));
            dat.Add((byte)(pcmptr >> 16));
            dat.Add((byte)(pcmptr >> 24));
            dat.Add((byte)pcmsize);//pcmdata size(32bit)
            dat.Add((byte)(pcmsize >> 8));
            dat.Add((byte)(pcmsize >> 16));
            dat.Add((byte)(pcmsize >> 24));
            dat.Add((byte)mem.LD_16(0x8c90));// JCLOCKの値(Jコマンドのタグ位置)
            dat.Add((byte)(mem.LD_16(0x8c90) >> 8));
            dat.Add((byte)(mem.LD_16(0x8c90) >> 16));
            dat.Add((byte)(mem.LD_16(0x8c90) >> 24));

            if (mem.LD_16(0x8c90) > 0)
            {
                log.Write(string.Format("#Jump count [{0}].\r\n", mem.LD_16(0x8c90)));
            }

            for (int i = 0; i < length; i++) dat.Add(mem.LD_8((ushort)(start + i)));

            if (tags != null)
            {
                footsize = 0;

                foreach (Tuple<string, string> tag in tags)
                {
                    byte[] b = Encoding.GetEncoding("shift_jis").GetBytes(string.Format("#{0} {1}\r\n", tag.Item1, tag.Item2));
                    footsize += b.Length;
                    dat.AddRange(b);
                }

                if (footsize > 0)
                {
                    dat.Add(0);
                    dat.Add(0);
                    dat.Add(0);
                    dat.Add(0);
                    footsize += 4;

                    dat[16] = (byte)footsize;//tagdata size(32bit)
                    dat[17] = (byte)(footsize >> 8);
                    dat[18] = (byte)(footsize >> 16);
                    dat[19] = (byte)(footsize >> 24);
                }
            }

            if (pcmuse)
            {
                for (int i = 0; i < pcmsize; i++) dat.Add(pcmdata[i]);
                if (pcmsize > 0)
                {
                    pcmptr = 32 + length + footsize;
                    dat[20] = (byte)pcmptr;//pcmdata size(32bit)
                    dat[21] = (byte)(pcmptr >> 8);
                    dat[22] = (byte)(pcmptr >> 16);
                    dat[23] = (byte)(pcmptr >> 24);
                }
            }

            try
            {
                System.IO.File.WriteAllBytes(fname, dat.ToArray());
            }
            catch
            {
                log.Write(string.Format("#File write error [{0}].\r\n", fname));
                return -2;
            }

            log.Write(string.Format("#Saved [{0}].\r\n", fname));
            return 0;
        }

        private void LoadFMVoice(string fn)
        {
            //mucファイルのある位置にあるfn
            string mucPathVoice = Path.Combine(Path.GetDirectoryName(PlayingFileName), fn);
            //mdplayerがある位置にあるfn
            string mdpPathVoice = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fn);
            string decideVoice = "";

            log.Write(mucPathVoice);
            log.Write(mdpPathVoice);

            if (!File.Exists(mucPathVoice))
            {
                if (!File.Exists(mdpPathVoice))
                {
                    return;
                }
                decideVoice = mdpPathVoice;
            }
            else
            {
                decideVoice = mucPathVoice;
            }

            try
            {
                byte[] voice = File.ReadAllBytes(decideVoice);
                ushort adr = 0x6000;
                foreach (byte b in voice)
                {
                    mem.LD_8(adr++, b);
                }
            }
            catch
            {
                //失敗しても特に何もしない
            }
        }

        private byte[] LoadPCM(string fn)
        {
            //mucファイルのある位置にあるfn
            string mucPathPCM = Path.Combine(Path.GetDirectoryName(PlayingFileName), fn);
            //mdplayerがある位置にあるfn
            string mdpPathPCM = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fn);
            string decidePCM = "";

            log.Write(mucPathPCM);
            log.Write(mdpPathPCM);

            if (!File.Exists(mucPathPCM))
            {
                if (!File.Exists(mdpPathPCM))
                {
                    return null;
                }
                decidePCM = mdpPathPCM;
            }
            else
            {
                decidePCM = mucPathPCM;
            }

            try
            {
                return File.ReadAllBytes(decidePCM);
            }
            catch
            {
                //失敗しても特に何もしない
            }

            return null;
        }

        private void setPCMData(byte[] pcmdata)
        {
            if (pcmdata == null) return;

            int infosize;
            int i;
            int pcmtable;
            int inftable;
            int adr, whl, eadr;
            byte[] pcmname = new byte[17];
            int maxpcm = 32;

            infosize = 0x400;
            inftable = 0x0000;
            //for(i = 0; i < infosize; i++)
            //{
            //    mem.LD_8((ushort)(inftable + i), pcmdata[i]);
            //}

            pcmtable = 0xe300;
            for (i = 0; i < maxpcm; i++)
            {
                adr = pcmdata[inftable + 28] | (pcmdata[inftable + 29] * 0x100);
                whl = pcmdata[inftable + 30] | (pcmdata[inftable + 31] * 0x100);
                eadr = adr + (whl >> 2);
                if (pcmdata[i * 32] != 0)
                {
                    mem.LD_16((ushort)(pcmtable), (ushort)adr);
                    mem.LD_16((ushort)(pcmtable + 2), (ushort)eadr);
                    mem.LD_16((ushort)(pcmtable + 4), 0);
                    mem.LD_16((ushort)(pcmtable + 6), (ushort)(pcmdata[inftable + 26] | (pcmdata[inftable + 27] * 0x100)));
                    Array.Copy(pcmdata, i * 32, pcmname, 0, 16);
                    pcmname[16] = 0;
                    //log.Write(string.Format("#PCM{0} ${1:x04} ${2:x04} {3}", i + 1, adr, eadr, Encoding.GetEncoding("shift_jis").GetString(pcmname)));
                }
                pcmtable += 8;
                inftable += 32;
            }

            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x00, 0x20);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x00, 0x21);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x00, 0x00);

            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x10, 0x00);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x10, 0x80);

            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x00, 0x61);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x00, 0x68);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x01, 0x00);

            int startAddress = 0;
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x02, (int)((startAddress >> 2) & 0xff));
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x03, (int)((startAddress >> 10) & 0xff));
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x04, 0xff);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x05, 0xff);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x0c, 0xff);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x0d, 0xff);

            // データ転送
            for (int cnt = 0; cnt < pcmdata.Length - infosize; cnt++)
            {
                chipRegister.YM2608SetRegister(0, 0, 0x1, 0x08, pcmdata[infosize + cnt]);
                //log.Write(string.Format("#PCMDATA adr:{0:x04} dat:{1:x02}", (infosize + cnt) >> 2, pcmdata[infosize + cnt]));
            }
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x00, 0x00);
            chipRegister.YM2608SetRegister(0, 0, 0x1, 0x10, 0x80);

            chipRegister.YM2608SendData(0);
        }


    }
}
