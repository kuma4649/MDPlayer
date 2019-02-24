using NScci;
using Nc86ctl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class RealChip : IDisposable
    {
        private NScci.NScci nScci;
        private Nc86ctl.Nc86ctl nc86ctl;

        #region IDisposable Support

        private bool disposedValue = false; // 重複する呼び出しを検出するには

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

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public RealChip() 
        {
            log.ForcedWrite("RealChip:Ctr:STEP 00(Start)");

            int n = 0;
            try
            {
                nScci = new NScci.NScci();
                n = nScci.NSoundInterfaceManager_.getInterfaceCount();
                if (n == 0)
                {
                    nScci.Dispose();
                    nScci = null;
                    log.ForcedWrite("RealChip:Ctr:Not found SCCI.");
                }
                else
                {
                    log.ForcedWrite(string.Format("RealChip:Ctr:Found SCCI.(Interface count={0})", n));
                    getScciInstances();
                    nScci.NSoundInterfaceManager_.setLevelDisp(false);
                }
            }
            catch
            {
                nScci = null;
            }

            log.ForcedWrite("RealChip:Ctr:STEP 01");
            try
            {
                nc86ctl = new Nc86ctl.Nc86ctl();
                n = nc86ctl.getNumberOfChip();
                if (n == 0)
                {
                    nc86ctl.deinitialize();
                    nc86ctl = null;
                    log.ForcedWrite("RealChip:Ctr:Not found G.I.M.I.C.");
                }
                else
                {
                    log.ForcedWrite(string.Format("RealChip:Ctr:Found G.I.M.I.C.(Interface count={0})", n));
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

        public void Init()
        {
            if (nScci != null)
            {
                nScci.NSoundInterfaceManager_.init();
            }
            if (nc86ctl != null)
            {
                nc86ctl.initialize();
            }
        }

        public void reset()
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.reset();
            if (nc86ctl != null) nc86ctl.initialize();
        }

        public void SendData()
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.sendData();
            if (nc86ctl != null)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public RSoundChip GetRealChip(Setting.ChipType chipType,int ind=0)
        {
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

                        switch (ind)
                        {
                            case 0:
                                if (0 == chipType.SoundLocation
                                    && i == chipType.BusID
                                    && s == chipType.SoundChip)
                                {
                                    RScciSoundChip rsc = new RScciSoundChip(0, i, s);
                                    rsc.scci = nScci;
                                    return rsc;
                                }
                                break;
                            case 1:
                                if (0 == chipType.SoundLocation2A
                                    && i == chipType.BusID2A
                                    && s == chipType.SoundChip2A)
                                {
                                    RScciSoundChip rsc = new RScciSoundChip(0, i, s);
                                    rsc.scci = nScci;
                                    return rsc;
                                }
                                break;
                            case 2:
                                if (0 == chipType.SoundLocation2B
                                    && i == chipType.BusID2B
                                    && s == chipType.SoundChip2B)
                                {
                                    RScciSoundChip rsc = new RScciSoundChip(0, i, s);
                                    rsc.scci = nScci;
                                    return rsc;
                                }
                                break;
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
                    ChipType cct = gm.getModuleType();
                    int o = -1;
                    string seri = gm.getModuleInfo().Serial;
                    if (!int.TryParse(seri, out o)) o = -1;

                    switch (ind)
                    {
                        case 0:
                            if (-1 == chipType.SoundLocation
                                && i == chipType.BusID
                                && o == chipType.SoundChip)
                            {
                                RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o);
                                rsc.c86ctl = nc86ctl;
                                return rsc;
                            }
                            break;
                        case 1:
                            if (-1 == chipType.SoundLocation2A
                                && i == chipType.BusID2A
                                && o == chipType.SoundChip)
                            {
                                RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o);
                                rsc.c86ctl = nc86ctl;
                                return rsc;
                            }
                            break;
                        case 2:
                            if (-1 == chipType.SoundLocation2B
                                && i == chipType.BusID2B
                                && o == chipType.SoundChip)
                            {
                                RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o);
                                rsc.c86ctl = nc86ctl;
                                return rsc;
                            }
                            break;
                    }

                }
            }

            return null;
        }

        public List<Setting.ChipType> GetRealChipList(enmRealChipType realChipType)
        {
            List<Setting.ChipType> ret = new List<Setting.ChipType>();

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
                        if (t == (int)realChipType)
                        {
                            Setting.ChipType ct = new Setting.ChipType();
                            ct.SoundLocation = 0;
                            ct.BusID = i;
                            ct.SoundChip = s;
                            ct.ChipName = sc.getSoundChipInfo().cSoundChipName;
                            ct.InterfaceName = iInfo.cInterfaceName;
                            ret.Add(ct);
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
                    ChipType cct = gm.getModuleType();
                    Setting.ChipType ct = null;
                    int o = -1;
                    switch (realChipType)
                    {
                        case enmRealChipType.YM2608:
                            if (cct == ChipType.CHIP_YM2608)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri= gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.getModuleInfo().Devname;
                                ct.InterfaceName = gm.getMBInfo().Devname;
                            }
                            break;
                        case enmRealChipType.YM2151:
                            if (cct == ChipType.CHIP_YM2151)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.getModuleInfo().Devname;
                                ct.InterfaceName = gm.getMBInfo().Devname;
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

        public int dClock = 3579545;

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

        virtual public bool isBufferEmpty()
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
            NSoundInterface nsif = scci.NSoundInterfaceManager_.getInterface(BusID);
            NSoundChip nsc = nsif.getSoundChip(SoundChip);
            realChip = nsc;
            dClock = nsc.getSoundChipClock();
            if (nsc.getSoundChipType() == (int)enmRealChipType.YM2608)
            {
                //setRegister(0x2d, 00);
                //setRegister(0x29, 82);
                //setRegister(0x07, 38);
            }
        }

        override public void setRegister(int adr, int dat)
        {
            realChip.setRegister(adr, dat);
        }

        override public bool isBufferEmpty()
        {
            return realChip.isBufferEmpty();
        }
    }

    public class RC86ctlSoundChip : RSoundChip
    {
        public Nc86ctl.Nc86ctl c86ctl = null;
        private Nc86ctl.NIRealChip realChip = null;

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
            if(gm.getModuleType()== ChipType.CHIP_YM2608)
            {
                //setRegister(0x2d, 00);
                //setRegister(0x29, 82);
                //setRegister(0x07, 38);
            }
        }

        override public void setRegister(int adr, int dat)
        {
            realChip.@out((ushort)adr, (byte)dat);
        }

        override public bool isBufferEmpty()
        {
            return true;
        }

    }


}
