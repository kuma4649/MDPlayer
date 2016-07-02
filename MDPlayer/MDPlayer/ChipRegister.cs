using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class ChipRegister
    {

        private NScci.NSoundChip scYM2612 = null;
        private NScci.NSoundChip scSN76489 = null;
        private MDSound.MDSound mds = null;

        private Setting.ChipType ctYM2612 = null;
        private Setting.ChipType ctSN76489 = null;

        public int[][] fmRegister = null;
        public int[] fmKeyOn = null;
        public int[] psgRegister = null;

        private int LatchedRegister;
        private int NoiseFreq;

        public ChipRegister(MDSound.MDSound mds, NScci.NSoundChip scYM2612, NScci.NSoundChip scSN76489, Setting.ChipType ctYM2612, Setting.ChipType ctSN76489)
        {

            this.mds = mds;
            this.scYM2612 = scYM2612;
            this.scSN76489 = scSN76489;

            this.ctYM2612 = ctYM2612;
            this.ctSN76489 = ctSN76489;

            fmRegister = new int[2][] { new int[0x100], new int[0x100] };
            fmKeyOn = new int[6] { 0, 0, 0, 0, 0, 0 };
            psgRegister = new int[0x8];

        }

        public void setYM2612Register(int dPort, int dAddr, int dData,vgm.enmModel model)
        {
            if (ctYM2612 == null) return;

            fmRegister[dPort][dAddr] = dData;
            if (dPort == 0 && dAddr == 0x28)
            {
                int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                if (ch >= 0 && ch < 6) fmKeyOn[ch] = dData & 0xf0;
            }

            if (model == vgm.enmModel.VirtualModel)
            {
                if (ctYM2612.UseScci)
                {
                    if (ctYM2612.OnlyPCMEmulation)
                    {
                        if (dPort == 0 && dAddr == 0x2b)
                        {
                            mds.WriteYM2612((byte)dPort, (byte)dAddr, (byte)dData);
                        }
                        else if (dPort == 0 && dAddr == 0x2a)
                        {
                            mds.WriteYM2612((byte)dPort, (byte)dAddr, (byte)dData);
                        }
                    }
                }
                else
                {
                    mds.WriteYM2612((byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2612 == null) return;

                if (ctYM2612.OnlyPCMEmulation)
                {
                    if (dPort == 0 && dAddr == 0x2b)
                    {
                        scYM2612.setRegister(dPort * 0x100 + dAddr, dData);
                    }
                    else if (dPort == 0 && dAddr == 0x2a)
                    {
                    }
                    else
                    {
                        scYM2612.setRegister(dPort * 0x100 + dAddr, dData);
                    }
                }
                else
                {
                    scYM2612.setRegister(dPort * 0x100 + dAddr, dData);
                }
            }

        }

        public void setYM2612SyncWait(int wait)
        {
            if (scYM2612 != null && ctYM2612.UseWait)
            {
                scYM2612.setRegister(-1, (int)(wait * (ctYM2612.UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void setSN76489Register(int dData)
        {
            if (ctSN76489 == null) return;

            SN76489_Write(dData);

            if (ctSN76489.UseScci)
            {
                if (scSN76489 == null) return;
                scSN76489.setRegister(0, dData);
            }
            else
            {
                mds.WriteSN76489((byte)dData);
            }
        }

        public void setSN76489SyncWait(int wait)
        {
            if (scSN76489 != null && ctSN76489.UseWait)
            {
                scSN76489.setRegister(-1, (int)(wait * (ctSN76489.UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void writeRF5C164PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            mds.WriteRF5C164PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeRF5C164(byte chipid, byte adr, byte data)
        {
            mds.WriteRF5C164(chipid, adr, data);
        }

        public void writeRF5C164MemW(byte chipid, uint offset, byte data)
        {
            mds.WriteRF5C164MemW(chipid, offset, data);
        }

        public void writePWM(byte chipid, byte adr, uint data)
        {
            mds.WritePWM(chipid, adr, data);
        }


        private void SN76489_Write(int data)
        {
            if ((data & 0x80) > 0)
            {
                /* Latch/data byte  %1 cc t dddd */
                LatchedRegister = (data >> 4) & 0x07;
                psgRegister[LatchedRegister] =
                    (psgRegister[LatchedRegister] & 0x3f0) /* zero low 4 bits */
                    | (data & 0xf);                            /* and replace with data */
            }
            else
            {
                /* Data byte        %0 - dddddd */
                if ((LatchedRegister % 2) == 0 && (LatchedRegister < 5))
                    /* Tone register */
                    psgRegister[LatchedRegister] =
                        (psgRegister[LatchedRegister] & 0x00f) /* zero high 6 bits */
                        | ((data & 0x3f) << 4);                 /* and replace with data */
                else
                    /* Other register */
                    psgRegister[LatchedRegister] = data & 0x0f; /* Replace with data */
            }
            switch (LatchedRegister)
            {
                case 0:
                case 2:
                case 4: /* Tone channels */
                    if (psgRegister[LatchedRegister] == 0)
                        psgRegister[LatchedRegister] = 1; /* Zero frequency changed to 1 to avoid div/0 */
                    break;
                case 6: /* Noise */
                    NoiseFreq = 0x10 << (psgRegister[6] & 0x3); /* set noise signal generator frequency */
                    break;
            }
        }

    }
}
