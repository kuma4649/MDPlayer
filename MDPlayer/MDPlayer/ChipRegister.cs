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

        private byte[] algM = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x0c, 0x0e, 0x0e, 0x0f };
        private int[] opN = new int[] { 0, 2, 1, 3 };

        public int[][] fmRegister = null;
        public int[] fmKeyOn = null;
        public int[][] fmVol = new int[9][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] };
        public int[] fmCh3SlotVol = new int[4];
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
            for (int i = 0; i < 0x100; i++)
            {
                fmRegister[0][i] = 0; fmRegister[1][i] = 0;
            }
            fmKeyOn = new int[6] { 0, 0, 0, 0, 0, 0 };
            psgRegister = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

        }

        private int nowYM2612FadeoutVol = 0;

        public void setFadeoutVol(int v)
        {
            nowYM2612FadeoutVol = v;
            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2612Register(p, 0x40 + c, fmRegister[p][0x40 + c], vgm.enmModel.RealModel);
                    setYM2612Register(p, 0x44 + c, fmRegister[p][0x44 + c], vgm.enmModel.RealModel);
                    setYM2612Register(p, 0x48 + c, fmRegister[p][0x48 + c], vgm.enmModel.RealModel);
                    setYM2612Register(p, 0x4c + c, fmRegister[p][0x4c + c], vgm.enmModel.RealModel);
                }
            }
        }

        public void setYM2612Register(int dPort, int dAddr, int dData,vgm.enmModel model)
        {
            if (ctYM2612 == null) return;

            fmRegister[dPort][dAddr] = dData;

            if ((model == vgm.enmModel.RealModel && ctYM2612.UseScci) || (model == vgm.enmModel.VirtualModel && !ctYM2612.UseScci))
            {
                //fmRegister[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6 && (dData & 0xf0)>0)
                    {
                        if (ch != 2 || (fmRegister[0][0x27] & 0xc0) != 0x40)
                        {
                            if (ch != 5 || (fmRegister[0][0x2b] & 0x80) == 0)
                            {
                                fmKeyOn[ch] = dData & 0xf0;
                                int p = (ch > 2) ? 1 : 0;
                                int c = (ch > 2) ? (ch - 3) : ch;
                                fmVol[ch][0] = (int)(256 * 6 * ((fmRegister[p][0xb4 + c] & 0x80) > 0 ? 1 : 0) * ((127 - (fmRegister[p][0x4c + c] & 0x7f)) / 127.0));
                                fmVol[ch][1] = (int)(256 * 6 * ((fmRegister[p][0xb4 + c] & 0x40) > 0 ? 1 : 0) * ((127 - (fmRegister[p][0x4c + c] & 0x7f)) / 127.0));
                            }
                        }
                        else
                        {
                            fmKeyOn[2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVol[0] = (int)(256 * 6 * ((127 - (fmRegister[0][0x40 + 2] & 0x7f)) / 127.0));
                            if ((dData & 0x20) > 0) fmCh3SlotVol[2] = (int)(256 * 6 * ((127 - (fmRegister[0][0x44 + 2] & 0x7f)) / 127.0));
                            if ((dData & 0x40) > 0) fmCh3SlotVol[1] = (int)(256 * 6 * ((127 - (fmRegister[0][0x48 + 2] & 0x7f)) / 127.0));
                            if ((dData & 0x80) > 0) fmCh3SlotVol[3] = (int)(256 * 6 * ((127 - (fmRegister[0][0x4c + 2] & 0x7f)) / 127.0));
                        }
                    }
                }

                if ((fmRegister[0][0x2b] & 0x80) > 0)
                {
                    if (fmRegister[0][0x2a] > 0)
                    {
                        fmVol[5][0] = fmRegister[0][0x2a] * 10 * ((fmRegister[1][0xb4 + 2] & 0x80) > 0 ? 1 : 0);
                        fmVol[5][1] = fmRegister[0][0x2a] * 10 * ((fmRegister[1][0xb4 + 2] & 0x40) > 0 ? 1 : 0);
                    }
                }
            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int al = fmRegister[dPort][0xb0 + ch] & 0x07;//AL
                int slot = (dAddr & 0xc) >> 2;

                if ((algM[al] & (1 << slot)) > 0)
                {
                    dData = Math.Min(dData + nowYM2612FadeoutVol, 127);
                }
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
                        else if (dPort == 1 && dAddr == 0xb6)
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

        public void writeRF5C164PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteRF5C164PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeRF5C164(byte chipid, byte adr, byte data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteRF5C164(chipid, adr, data);
        }

        public void writeRF5C164MemW(byte chipid, uint offset, byte data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteRF5C164MemW(chipid, offset, data);
        }

        public void writePWM(byte chipid, byte adr, uint data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WritePWM(chipid, adr, data);
        }

        private int volF = 1;
        public void updateVol()
        {
            volF--;
            if (volF > 0) return;

            volF = 1;

            for (int i = 0; i < 9; i++)
            {
                if (fmVol[i][0] > 0) { fmVol[i][0] -= 50; if (fmVol[i][0] < 0) fmVol[i][0] = 0; }
                if (fmVol[i][1] > 0) { fmVol[i][1] -= 50; if (fmVol[i][1] < 0) fmVol[i][1] = 0; }
            }
            for (int i = 0; i < 4; i++)
            {
                if (fmCh3SlotVol[i] > 0) { fmCh3SlotVol[i] -= 50; if (fmCh3SlotVol[i] < 0) fmCh3SlotVol[i] = 0; }
            }
        }

        public int[][] GetFMVolume()
        {
            //if (ctYM2612.UseScci)
            //{
                return fmVol;
            //}
            //return mds.ReadFMVolume();
        }

        public int[] GetFMCh3SlotVolume()
        {
            //if (ctYM2612.UseScci)
            //{
                return fmCh3SlotVol;
            //}
            //return mds.ReadFMCh3SlotVolume();
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
