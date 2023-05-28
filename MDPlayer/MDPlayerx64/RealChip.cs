using NScci;
using Nc86ctl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MDPlayer
{
    public class RealChip : IDisposable
    {
        private NScci.NScci nScci;
        private Nc86ctl.Nc86ctl nc86ctl;

        #region IDisposable Support

        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public RealChip(bool sw) 
        {
            log.ForcedWrite("RealChip:Ctr:STEP 00(Start)");
            if (!sw)
            {
                log.ForcedWrite("RealChip:Not Initialize(user)");
                return;
            }

            //SCCIの存在確認
            int n = 0;
            try
            {
                nScci = new NScci.NScci();
                n = nScci.NSoundInterfaceManager_ == null ? 0 : nScci.NSoundInterfaceManager_.getInterfaceCount();
                if (n == 0)
                {
                    if (nScci != null) nScci.Dispose();
                    nScci = null;
                    log.ForcedWrite("RealChip:Ctr:Not found SCCI.");
                }
                else
                {
                    log.ForcedWrite("RealChip:Ctr:Found SCCI.(Interface count={0})", n);
                    getScciInstances();
                    nScci.NSoundInterfaceManager_.setLevelDisp(false);
                }
            }
            catch
            {
                nScci = null;
            }

            //GIMICの存在確認
            log.ForcedWrite("RealChip:Ctr:STEP 01");
            try
            {
                nc86ctl = new Nc86ctl.Nc86ctl();
                nc86ctl.initialize();
                n = nc86ctl.getNumberOfChip();
                
                if (n == 0)
                {
                    nc86ctl.deinitialize();
                    nc86ctl = null;
                    log.ForcedWrite("RealChip:Ctr:Not found G.I.M.I.C.");
                }
                else
                {
                    log.ForcedWrite("RealChip:Ctr:Found G.I.M.I.C.(Interface count={0})", n);
                    Nc86ctl.NIRealChip nirc = nc86ctl.getChipInterface(0);
                    nirc.reset();
                }
            }
            catch
            {
                nc86ctl = null;
            }
            log.ForcedWrite("RealChip:Ctr:STEP 02(Success)");
        }

        public void Close()
        {
            if (nScci != null)
            {
                try
                {
                    nScci.Dispose();
                }
                catch { }
                nScci = null;
            }
            if (nc86ctl != null)
            {
                try
                {
                    nc86ctl.deinitialize();
                }
                catch { }
                nc86ctl = null;
            }
        }

        public void getScciInstances()
        {
            int ifc = nScci.NSoundInterfaceManager_.getInterfaceCount();

            for (int i = 0; i < ifc; i++)
            {
                NSoundInterface sif = nScci.NSoundInterfaceManager_.getInterface(i);

                int scc = sif.getSoundChipCount();
                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                }
            }

        }

        public void setLevelDisp(bool v)
        {
            if (nScci == null) return;
            nScci.NSoundInterfaceManager_.setLevelDisp(v);
        }

        //public void Init()
        //{
        //    if (nScci != null)
        //    {
        //        nScci.NSoundInterfaceManager_.init();
        //    }
        //    if (nc86ctl != null)
        //    {
        //        nc86ctl.initialize();
        //    }
        //}

        public void reset()
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.reset();
            if (nc86ctl != null)
            {
                //nc86ctl.initialize();
                int n=nc86ctl.getNumberOfChip();
                for(int i = 0; i < n; i++)
                {
                    NIRealChip rc= nc86ctl.getChipInterface(i);
                    rc.reset();
                }
            }
        }

        public void SendData()
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.sendData();
            if (nc86ctl != null)
            {
                //int n = nc86ctl.getNumberOfChip();
                //for (int i = 0; i < n; i++)
                //{
                //    NIRealChip rc = nc86ctl.getChipInterface(i);
                //    if (rc != null)
                //    {
                //        while ((rc.@in(0x0) & 0x00) != 0)
                //            System.Threading.Thread.Sleep(0);
                //    }
                //}
            }
        }

        public void WaitOPNADPCMData(bool isGIMIC)
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.sendData();
            if (nc86ctl != null && isGIMIC)
            {
                //int n = nc86ctl.getNumberOfChip();
                //for (int i = 0; i < n; i++)
                //{
                //    NIRealChip rc = nc86ctl.getChipInterface(i);
                //    if (rc != null)
                //    {
                //        while ((rc.@in(0x0) & 0x83) != 0)
                //            System.Threading.Thread.Sleep(0);
                //        while ((rc.@in(0x100) & 0xbf) != 0)
                //        {
                //            System.Threading.Thread.Sleep(0);
                //        }
                //    }
                //}

            }
            else
            {
                if (nScci == null) return;
                nScci.NSoundInterfaceManager_.sendData();
                while (!nScci.NSoundInterfaceManager_.isBufferEmpty())
                {
                    Thread.Sleep(0);
                }
            }
        }

        public RSoundChip GetRealChip(Setting.ChipType2 ChipType2, int ind = 0)
        {
            if (ind == 3 || ind == 4)
            {
                if (ChipType2.realChipInfo.Length < ind - 3 + 1) return null;
                if (ChipType2.realChipInfo[ind - 3] == null) return null;
                ind -= 3;
            }
            else
            {
                if (ChipType2.realChipInfo.Length < ind + 1) return null;
                if (ChipType2.realChipInfo[ind] == null) return null;
            }

            if (nScci != null)
            {
                int iCount = nScci.NSoundInterfaceManager_.getInterfaceCount();
                for (int i = 0; i < iCount; i++)
                {
                    NSoundInterface iIntfc = nScci.NSoundInterfaceManager_.getInterface(i);
                    NSCCI_INTERFACE_INFO iInfo = nScci.NSoundInterfaceManager_.getInterfaceInfo(i);
                    int sCount = iIntfc.getSoundChipCount();
                    for (int s = 0; s < sCount; s++)
                    {
                        NSoundChip sc = iIntfc.getSoundChip(s);

                        if (0 == ChipType2.realChipInfo[ind].SoundLocation
                            && i == ChipType2.realChipInfo[ind].BusID
                            && s == ChipType2.realChipInfo[ind].SoundChip)
                        {
                            RScciSoundChip rsc = new RScciSoundChip(0, i, s);
                            rsc.scci = nScci;
                            return rsc;
                        }

                    }
                }
            }

            if (nc86ctl != null)
            {
                int iCount = nc86ctl.getNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    NIRealChip rc = nc86ctl.getChipInterface(i);
                    rc.reset();
                    NIGimic2 gm = rc.QueryInterface();
                    ChipType cct = gm.getModuleType();
                    int o = -1;
                    string seri = gm.getModuleInfo().Serial;
                    if (!int.TryParse(seri, out o)) o = -1;

                    if (-1 == ChipType2.realChipInfo[ind].SoundLocation
                        && i == ChipType2.realChipInfo[ind].BusID
                        && o == ChipType2.realChipInfo[ind].SoundChip)
                    {
                        RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o);
                        rsc.c86ctl = nc86ctl;
                        return rsc;
                    }

                }
            }

            return null;
        }

        public List<Setting.ChipType2> GetRealChipList(EnmRealChipType realChipType2)
        {
            List<Setting.ChipType2> ret = new List<Setting.ChipType2>();

            if (nScci != null)
            {
                int iCount = nScci.NSoundInterfaceManager_.getInterfaceCount();
                for (int i = 0; i < iCount; i++)
                {
                    NSoundInterface iIntfc = nScci.NSoundInterfaceManager_.getInterface(i);
                    NSCCI_INTERFACE_INFO iInfo = nScci.NSoundInterfaceManager_.getInterfaceInfo(i);
                    int sCount = iIntfc.getSoundChipCount();
                    for (int s = 0; s < sCount; s++)
                    {
                        NSoundChip sc = iIntfc.getSoundChip(s);
                        int t = sc.getSoundChipType();
                        if (t == (int)realChipType2)
                        {
                            Setting.ChipType2 ct = new Setting.ChipType2();
                            ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                            ct.realChipInfo[0].SoundLocation = 0;
                            ct.realChipInfo[0].BusID = i;
                            ct.realChipInfo[0].SoundChip = s;
                            ct.realChipInfo[0].ChipName = sc.getSoundChipInfo().cSoundChipName;
                            ct.realChipInfo[0].InterfaceName = iInfo.cInterfaceName;
                            ret.Add(ct);
                        }
                        else if (realChipType2 == EnmRealChipType.K051649 && (t == 12 || t == 13))
                        {
                            Setting.ChipType2 ct = new Setting.ChipType2();
                            ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                            ct.realChipInfo[0].SoundLocation = 0;
                            ct.realChipInfo[0].BusID = i;
                            ct.realChipInfo[0].SoundChip = s;
                            ct.realChipInfo[0].ChipName = sc.getSoundChipInfo().cSoundChipName;
                            ct.realChipInfo[0].InterfaceName = iInfo.cInterfaceName;
                            ret.Add(ct);
                        }
                        else
                        {
                            //互換指定をチェック
                            NSCCI_SOUND_CHIP_INFO chipInfo = sc.getSoundChipInfo();
                            for (int n = 0; n < chipInfo.iCompatibleSoundChip.Length; n++)
                            {
                                if ((int)realChipType2 != chipInfo.iCompatibleSoundChip[n]) continue;

                                Setting.ChipType2 ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = 0;
                                ct.realChipInfo[0].BusID = i;
                                ct.realChipInfo[0].SoundChip = s;
                                ct.realChipInfo[0].ChipName = sc.getSoundChipInfo().cSoundChipName;
                                ct.realChipInfo[0].InterfaceName = iInfo.cInterfaceName;
                                ret.Add(ct);
                                break;
                            }
                        }
                    }
                }
            }

            if (nc86ctl != null)
            {
                int iCount = nc86ctl.getNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    NIRealChip rc = nc86ctl.getChipInterface(i);
                    NIGimic2 gm = rc.QueryInterface();
                    Devinfo di = gm.getModuleInfo();
                    ChipType cct = gm.getModuleType();
                    if (cct == ChipType.CHIP_UNKNOWN)
                    {
                        if (di.Devname == "GMC-S2149") cct = ChipType.CHIP_YM2149;
                        else if (di.Devname == "GMC-S8910") cct = ChipType.CHIP_AY38910;
                        else if (di.Devname == "GMC-S2413") cct = ChipType.CHIP_YM2413;
                    }
                    Setting.ChipType2 ct = null;
                    int o = -1;
                    switch (realChipType2)
                    {
                        case EnmRealChipType.YM2203:
                        case EnmRealChipType.YM2608:
                            if (cct == ChipType.CHIP_YM2608 || cct == ChipType.CHIP_YMF288 || cct == ChipType.CHIP_YM2203)
                            {
                                ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = -1;
                                ct.realChipInfo[0].BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.realChipInfo[0].SoundChip = o;
                                ct.realChipInfo[0].ChipName = di.Devname;
                                ct.realChipInfo[0].InterfaceName = gm.getMBInfo().Devname;
                                ct.realChipInfo[0].ChipType = (int)cct;
                            }
                            break;
                        case EnmRealChipType.AY8910:
                            if (   cct == ChipType.CHIP_YM2149
                                || cct == ChipType.CHIP_AY38910
                                || cct == ChipType.CHIP_YM2608 
                                || cct == ChipType.CHIP_YMF288 
                                || cct == ChipType.CHIP_YM2203)
                            {
                                ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = -1;
                                ct.realChipInfo[0].BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.realChipInfo[0].SoundChip = o;
                                ct.realChipInfo[0].ChipName = di.Devname;
                                ct.realChipInfo[0].InterfaceName = gm.getMBInfo().Devname;
                                ct.realChipInfo[0].ChipType = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM2413:
                            if (cct == ChipType.CHIP_YM2413)
                            {
                                ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = -1;
                                ct.realChipInfo[0].BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.realChipInfo[0].SoundChip = o;
                                ct.realChipInfo[0].ChipName = di.Devname;
                                ct.realChipInfo[0].InterfaceName = gm.getMBInfo().Devname;
                                ct.realChipInfo[0].ChipType = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM2610:
                            if (cct == ChipType.CHIP_YM2608 || cct == ChipType.CHIP_YMF288)
                            {
                                ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = -1;
                                ct.realChipInfo[0].BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.realChipInfo[0].SoundChip = o;
                                ct.realChipInfo[0].ChipName = di.Devname;
                                ct.realChipInfo[0].InterfaceName = gm.getMBInfo().Devname;
                                ct.realChipInfo[0].ChipType = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM2151:
                            if (cct == ChipType.CHIP_YM2151)
                            {
                                ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = -1;
                                ct.realChipInfo[0].BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.realChipInfo[0].SoundChip = o;
                                ct.realChipInfo[0].ChipName = di.Devname;
                                ct.realChipInfo[0].InterfaceName = gm.getMBInfo().Devname;
                                ct.realChipInfo[0].ChipType = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM3526:
                        case EnmRealChipType.YM3812:
                        case EnmRealChipType.YMF262:
                            if (cct == ChipType.CHIP_OPL3)
                            {
                                ct = new Setting.ChipType2();
                                ct.realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
                                ct.realChipInfo[0].SoundLocation = -1;
                                ct.realChipInfo[0].BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.realChipInfo[0].SoundChip = o;
                                ct.realChipInfo[0].ChipName = di.Devname;
                                ct.realChipInfo[0].InterfaceName = gm.getMBInfo().Devname;
                                ct.realChipInfo[0].ChipType = (int)cct;
                            }
                            break;
                    }

                    if (ct != null) ret.Add(ct);
                }
            }

            return ret;
        }
    }

    public class RSoundChip
    {
        protected int SoundLocation;
        protected int BusID;
        protected int SoundChip;

        public uint dClock = 3579545;

        public RSoundChip(int soundLocation,int busID,int soundChip)
        {
            SoundLocation = soundLocation;
            BusID = busID;
            SoundChip = soundChip;
        }

        virtual public void init()
        {
            throw new NotImplementedException();
        }

        virtual public void setRegister(int adr, int dat)
        {
            throw new NotImplementedException();
        }

        virtual public int getRegister(int adr)
        {
            throw new NotImplementedException();
        }

        virtual public bool isBufferEmpty()
        {
            throw new NotImplementedException();
        }

        virtual public uint SetMasterClock(uint mClock)
        {
            throw new NotImplementedException();
        }

        virtual public void setSSGVolume(byte vol)
        {
            throw new NotImplementedException();
        }

    }

    public class RScciSoundChip : RSoundChip
    {
        public NScci.NScci scci = null;
        private NSoundChip realChip = null;

        public RScciSoundChip(int soundLocation, int busID, int soundChip) : base(soundLocation, busID, soundChip)
        {
        }

        override public void init()
        {
            realChip = null;
            int n = scci.NSoundInterfaceManager_.getInterfaceCount();
            if (BusID >= n)
            {
                return;
            }

            NSoundInterface nsif = scci.NSoundInterfaceManager_.getInterface(BusID);
            

            int c = nsif.getSoundChipCount();
            if (SoundChip >= c)
            {
                return;
            }
            NSoundChip nsc = nsif.getSoundChip(SoundChip);
            realChip = nsc;
            dClock = (uint)nsc.getSoundChipClock();

            //chipの種類ごとに初期化コマンドを送りたい場合
            switch (nsc.getSoundChipType())
            {
                case (int)EnmRealChipType.YM2608:
                    //setRegister(0x2d, 00);
                    //setRegister(0x29, 82);
                    //setRegister(0x07, 38);
                    break;
            }
        }

        override public void setRegister(int adr, int dat)
        {
            if (realChip == null) return;
            realChip.setRegister(adr, dat);
        }

        override public int getRegister(int adr)
        {
            if (realChip == null) return -1;
            return realChip.getRegister(adr);
        }

        override public bool isBufferEmpty()
        {
            if (realChip == null) return false;
            return realChip.isBufferEmpty();
        }

        /// <summary>
        /// マスタークロックの設定
        /// </summary>
        /// <param name="mClock">設定したい値</param>
        /// <returns>実際設定された値</returns>
        override public uint SetMasterClock(uint mClock)
        {
            //SCCIはクロックの変更不可
            if (realChip == null) return 0;

            return (uint)realChip.getSoundChipClock();
        }

        override public void setSSGVolume(byte vol)
        {
            //SCCIはSSG音量の変更不可
            if (realChip == null) return;
        }

    }

    public class RC86ctlSoundChip : RSoundChip
    {
        public Nc86ctl.Nc86ctl c86ctl = null;
        public Nc86ctl.NIRealChip realChip = null;
        public Nc86ctl.ChipType ChipType = ChipType.CHIP_UNKNOWN;

        public RC86ctlSoundChip(int soundLocation, int busID, int soundChip) : base(soundLocation, busID, soundChip)
        {
        }

        override public void init()
        {
            NIRealChip rc = c86ctl.getChipInterface(BusID);
            rc.reset();
            realChip = rc;
            NIGimic2 gm = rc.QueryInterface();
            dClock = gm.getPLLClock();
            log.ForcedWrite("C86ctl:PLL Clock={0}", dClock);
            Devinfo di = gm.getModuleInfo();
            ChipType = gm.getModuleType();
            log.ForcedWrite("C86ctl:Found ChipType={0}", ChipType);
            if (ChipType == ChipType.CHIP_UNKNOWN)
            {
                if (di.Devname == "GMC-S2149") ChipType = ChipType.CHIP_YM2149;
                else if (di.Devname == "GMC-S8910") ChipType = ChipType.CHIP_AY38910;
                else if (di.Devname == "GMC-S2413") ChipType = ChipType.CHIP_YM2413;
            }
            else if (ChipType == ChipType.CHIP_YM2608)
            {
                //setRegister(0x2d, 00);
                //setRegister(0x29, 82);
                //setRegister(0x07, 38);
            }
            else if (ChipType == ChipType.CHIP_OPM)
            {
                OPZReset();
            }
        }

        override public void setRegister(int adr, int dat)
        {
            if (adr < 0) 
                return;
            realChip.@out((ushort)adr, (byte)dat);
            log.Write("Out Register C86Ctl(Adr:{0:x04} Dat:{1:x02})",(ushort)adr,(byte)dat);
            //realChip.directOut((ushort)adr, (byte)dat);
        }

        override public int getRegister(int adr)
        {
            log.Write("In  Register C86Ctl(Adr:{0:x04}", (ushort)adr);
            return realChip.@in((ushort)adr);
        }

        override public bool isBufferEmpty()
        {
            return true;
        }

        /// <summary>
        /// マスタークロックの設定
        /// </summary>
        /// <param name="mClock">設定したい値</param>
        /// <returns>実際設定された値</returns>
        override public uint SetMasterClock(uint mClock)
        {
            NIGimic2 gm = realChip.QueryInterface();
            uint nowClock= gm.getPLLClock();
            if (nowClock != mClock)
            {
                gm.setPLLClock(mClock);
                log.Write("Set PLLClock(clock:{0:d}", mClock);
            }
            nowClock = gm.getPLLClock();
            realChip.reset();
            log.Write("reset C86Ctl");

            if (ChipType == ChipType.CHIP_OPM)
            {
                OPZReset2();
            }

            return nowClock;
        }

        override public void setSSGVolume(byte vol)
        {
            NIGimic2 gm = realChip.QueryInterface();
            gm.setSSGVolume(vol);
        }

        private void OPZReset()
        {
            SetMasterClock(3579545);
        }

        private void OPZReset2()
        {
            setRegister(0x09, 0x00);
            setRegister(0x0f, 0x00);
            setRegister(0x1c, 0x00);
            setRegister(0x1e, 0x00);

            setRegister(0x0a, 0x04);
            setRegister(0x14, 0x70);
            setRegister(0x15, 0x01);

            setRegister(0x16, 0x00);
            setRegister(0x17, 0x00);
            setRegister(0x18, 0x00);
            //setRegister(0x19, 0x80);
            setRegister(0x19, 0x00);
            setRegister(0x1b, 0x00);
            //setRegister(0x0f, 0x00);
        }

    }


}
