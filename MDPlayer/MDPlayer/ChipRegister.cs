using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class ChipRegister
    {

        private MDSound.MDSound mds = null;

        private NScci.NSoundChip scSN76489 = null;
        private Setting.ChipType ctSN76489 = null;

        private byte[] algM = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x0c, 0x0e, 0x0e, 0x0f };
        private int[] opN = new int[] { 0, 2, 1, 3 };
        private int[] noteTbl = new int[] {   2,  4, 5, -1, 6, 8, 9, -1, 10, 12, 13, -1, 14,  0,  1, -1 };
        private int[] noteTbl2 = new int[] { 13, 14, 0, -1, 1, 2, 4, -1,  5,  6,  8, -1,  9, 10, 12, -1 };

        private NScci.NSoundChip scYM2612 = null;
        private Setting.ChipType ctYM2612 = null;
        public int[][] fmRegister = null;
        public int[] fmKeyOn = null;
        public int[][] fmVol = new int[9][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] };
        public int[] fmCh3SlotVol = new int[4];
        private int nowYM2612FadeoutVol = 0;
        private bool[] maskFMChYM2612 = new bool[6] { false, false, false, false, false, false };

        private NScci.NSoundChip scYM2608 = null;
        private Setting.ChipType ctYM2608 = null;
        public int[][] fmRegisterYM2608 = null;
        public int[] fmKeyOnYM2608 = null;
        public int[][] fmVolYM2608 = new int[9][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] };
        public int[] fmCh3SlotVolYM2608 = new int[4];
        public int[][] fmVolYM2608Rhythm = new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] };
        public int[] fmVolYM2608Adpcm = new int[2];
        public int fmVolYM2608AdpcmPan = 0;
        private int nowYM2608FadeoutVol = 0;
        private bool[] maskFMChYM2608 = new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        private NScci.NSoundChip scYM2151 = null;
        private Setting.ChipType ctYM2151 = null;
        public int[] fmRegisterYM2151 = null;
        public int[] fmKeyOnYM2151 = null;
        public int[][] fmVolYM2151 = new int[8][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2]};
        private int nowYM2151FadeoutVol = 0;
        private bool[] maskFMChYM2151 = new bool[8] { false, false, false, false, false, false, false, false };

        public int[] psgRegister = null;
        public int[][] psgVol = new int[4][] { new int[2], new int[2], new int[2], new int[2] };
        public int nowSN76489FadeoutVol = 0;

        private int LatchedRegister;
        private int NoiseFreq;



        public ChipRegister(MDSound.MDSound mds, NScci.NSoundChip scYM2612, NScci.NSoundChip scSN76489, NScci.NSoundChip scYM2608, NScci.NSoundChip scYM2151, Setting.ChipType ctYM2612, Setting.ChipType ctSN76489, Setting.ChipType ctYM2608, Setting.ChipType ctYM2151)
        {

            this.mds = mds;
            this.scYM2612 = scYM2612;
            this.scYM2608 = scYM2608;
            this.scYM2151 = scYM2151;
            this.scSN76489 = scSN76489;

            this.ctYM2612 = ctYM2612;
            this.ctYM2608 = ctYM2608;
            this.ctYM2151 = ctYM2151;
            this.ctSN76489 = ctSN76489;

            initChipRegister();
        }

        public void initChipRegister()
        {

            fmRegister = new int[2][] { new int[0x100], new int[0x100] };
            for (int i = 0; i < 0x100; i++)
            {
                fmRegister[0][i] = 0; fmRegister[1][i] = 0;
            }
            fmRegister[0][0xb4] = 0xc0;
            fmRegister[0][0xb5] = 0xc0;
            fmRegister[0][0xb6] = 0xc0;
            fmRegister[1][0xb4] = 0xc0;
            fmRegister[1][0xb5] = 0xc0;
            fmRegister[1][0xb6] = 0xc0;
            fmKeyOn = new int[6] { 0, 0, 0, 0, 0, 0 };

            fmRegisterYM2608 = new int[2][] { new int[0x100], new int[0x100] };
            for (int i = 0; i < 0x100; i++)
            {
                fmRegisterYM2608[0][i] = 0; fmRegisterYM2608[1][i] = 0;
            }
            fmRegisterYM2608[0][0xb4] = 0xc0;
            fmRegisterYM2608[0][0xb5] = 0xc0;
            fmRegisterYM2608[0][0xb6] = 0xc0;
            fmRegisterYM2608[1][0xb4] = 0xc0;
            fmRegisterYM2608[1][0xb5] = 0xc0;
            fmRegisterYM2608[1][0xb6] = 0xc0;
            fmKeyOnYM2608 = new int[6] { 0, 0, 0, 0, 0, 0 };

            fmRegisterYM2151 = new int[0x100];
            for (int i = 0; i < 0x100; i++)
            {
                fmRegisterYM2151[i] = 0;
            }
            fmKeyOnYM2151 = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            psgRegister = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

        }


        public void setFadeoutVolYM2612(int v)
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

        public void setMaskYM2612(int ch,bool mask)
        {
            maskFMChYM2612[ch] = mask;

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            setYM2612Register(p, 0x40 + c, fmRegister[p][0x40 + c], vgm.enmModel.RealModel);
            setYM2612Register(p, 0x44 + c, fmRegister[p][0x44 + c], vgm.enmModel.RealModel);
            setYM2612Register(p, 0x48 + c, fmRegister[p][0x48 + c], vgm.enmModel.RealModel);
            setYM2612Register(p, 0x4c + c, fmRegister[p][0x4c + c], vgm.enmModel.RealModel);
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
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0)>0)
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

            if ((dAddr & 0xfc) == 0xb0)//FB ALG
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                for (int i = 0; i < 4; i++)
                {
                    int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        if (maskFMChYM2612[ch])
                        {
                            if (model == vgm.enmModel.VirtualModel)
                            {
                                if (!ctYM2612.UseScci)
                                {
                                    mds.WriteYM2612((byte)dPort, (byte)(0x40 + ch + slot * 4), (byte)127);
                                }
                            }
                            else
                            {
                                scYM2612.setRegister(dPort * 0x100 + (0x40 + ch + slot * 4), 127);
                            }
                        }
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int al = fmRegister[dPort][0xb0 + ch] & 0x07;//AL
                int slot = (dAddr & 0xc) >> 2;
                dData &= 0x7f;

                if ((algM[al] & (1 << slot)) > 0)
                {
                    if (ch != 3)
                    {
                        dData = Math.Min(dData + nowYM2612FadeoutVol, 127);
                        dData = maskFMChYM2612[dPort * 3 + ch] ? 127 : dData;
                    }
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


        public void setYM2608Register(int dPort, int dAddr, int dData, vgm.enmModel model)
        {
            if (ctYM2608 == null) return;

            fmRegisterYM2608[dPort][dAddr] = dData;

            if ((model == vgm.enmModel.RealModel && ctYM2608.UseScci) || (model == vgm.enmModel.VirtualModel && !ctYM2608.UseScci))
            {
                //fmRegisterYM2608[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        if (ch != 2 || (fmRegisterYM2608[0][0x27] & 0xc0) != 0x40)
                        {
                            fmKeyOnYM2608[ch] = dData & 0xf0;
                            int p = (ch > 2) ? 1 : 0;
                            int c = (ch > 2) ? (ch - 3) : ch;
                            fmVolYM2608[ch][0] = (int)(256 * 6 * ((fmRegisterYM2608[p][0xb4 + c] & 0x80) > 0 ? 1 : 0) * ((127 - (fmRegisterYM2608[p][0x4c + c] & 0x7f)) / 127.0));
                            fmVolYM2608[ch][1] = (int)(256 * 6 * ((fmRegisterYM2608[p][0xb4 + c] & 0x40) > 0 ? 1 : 0) * ((127 - (fmRegisterYM2608[p][0x4c + c] & 0x7f)) / 127.0));
                        }
                        else
                        {
                            fmKeyOnYM2608[2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2608[0] = (int)(256 * 6 * ((127 - (fmRegisterYM2608[0][0x40 + 2] & 0x7f)) / 127.0));
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2608[2] = (int)(256 * 6 * ((127 - (fmRegisterYM2608[0][0x44 + 2] & 0x7f)) / 127.0));
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2608[1] = (int)(256 * 6 * ((127 - (fmRegisterYM2608[0][0x48 + 2] & 0x7f)) / 127.0));
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2608[3] = (int)(256 * 6 * ((127 - (fmRegisterYM2608[0][0x4c + 2] & 0x7f)) / 127.0));
                        }
                    }
                }

                if (dPort == 1 && dAddr == 0x01)
                {
                    fmVolYM2608AdpcmPan = (dData & 0xc0) >> 6;
                    if (fmVolYM2608AdpcmPan > 0)
                    {
                        fmVolYM2608Adpcm[0] = (int)((256 * 6.0 * fmRegisterYM2608[1][0x0b]/64.0) * ((fmVolYM2608AdpcmPan & 0x02) > 0 ? 1 : 0));
                        fmVolYM2608Adpcm[1] = (int)((256 * 6.0 * fmRegisterYM2608[1][0x0b]/64.0) * ((fmVolYM2608AdpcmPan & 0x01) > 0 ? 1 : 0));
//                        System.Console.WriteLine("{0:X2}:{1:X2}", 0x09, fmRegisterYM2608[1][0x09]);
//                        System.Console.WriteLine("{0:X2}:{1:X2}", 0x0A, fmRegisterYM2608[1][0x0A]);
                    }
                }

                if (dPort == 0 && dAddr == 0x10)
                {
                    int tl = fmRegisterYM2608[0][0x11] & 0x3f;
                    for (int i = 0; i < 6; i++)
                    {
                        if ((dData & (0x1<< i)) > 0)
                        {
                            int il = fmRegisterYM2608[0][0x18+i] & 0x1f;
                            int pan = (fmRegisterYM2608[0][0x18 + i] & 0xc0) >> 6;
                            fmVolYM2608Rhythm[i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                            fmVolYM2608Rhythm[i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                        }
                    }
                }

            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                int al = fmRegisterYM2608[dPort][0xb0 + ch] & 0x07;//AL
                int slot = (dAddr & 0xc) >> 2;
                dData &= 0x7f;

                if ((algM[al] & (1 << slot)) > 0)
                {
                    if (ch != 3)
                    {
                        dData = Math.Min(dData + nowYM2608FadeoutVol, 127);
                        dData = maskFMChYM2608[dPort * 3 + ch] ? 127 : dData;
                    }
                }
            }

            //ssg level
            if (dPort == 0 && (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2608FadeoutVol >> 3;
                dData = Math.Max(dData - d, 0);
            }

            //rhythm level
            if (dPort == 0 && dAddr == 0x11)
            {
                int d = nowYM2608FadeoutVol >> 1;
                dData = Math.Max(dData - d, 0);
            }

            //adpcm level
            if (dPort == 1 && dAddr == 0x0b)
            {
                int d = nowYM2608FadeoutVol * 2;
                dData = Math.Max(dData - d, 0);
            }

            if (model == vgm.enmModel.VirtualModel)
            {
                if (!ctYM2608.UseScci)
                {
                    //mds.WriteYM2608((byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2608 == null) return;

                scYM2608.setRegister(dPort * 0x100 + dAddr, dData);
            }

        }

        public void setYM2608SyncWait(int wait)
        {
            if (scYM2608 != null && ctYM2608.UseWait)
            {
                scYM2608.setRegister(-1, (int)(wait * (ctYM2608.UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void sendDataYM2608(vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel) return;

            if (scYM2608 != null && ctYM2608.UseWait)
            {
                scYM2608.parentSoundInterface.parentNScci.sendData();
                while (!scYM2608.parentSoundInterface.parentNScci.isBufferEmpty()) { }
            }
        }

        public void setFadeoutVolYM2608(int v)
        {
            nowYM2608FadeoutVol = v;
            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2608Register(p, 0x40 + c, fmRegisterYM2608[p][0x40 + c], vgm.enmModel.RealModel);
                    setYM2608Register(p, 0x44 + c, fmRegisterYM2608[p][0x44 + c], vgm.enmModel.RealModel);
                    setYM2608Register(p, 0x48 + c, fmRegisterYM2608[p][0x48 + c], vgm.enmModel.RealModel);
                    setYM2608Register(p, 0x4c + c, fmRegisterYM2608[p][0x4c + c], vgm.enmModel.RealModel);
                }
            }

            //ssg
            setYM2608Register(0, 0x08, fmRegisterYM2608[0][0x08], vgm.enmModel.RealModel);
            setYM2608Register(0, 0x09, fmRegisterYM2608[0][0x09], vgm.enmModel.RealModel);
            setYM2608Register(0, 0x0a, fmRegisterYM2608[0][0x0a], vgm.enmModel.RealModel);

            //rhythm
            setYM2608Register(0, 0x11, fmRegisterYM2608[0][0x11], vgm.enmModel.RealModel);

            //adpcm
            setYM2608Register(1, 0x0b, fmRegisterYM2608[1][0x0b], vgm.enmModel.RealModel);
        }

        public void setMaskYM2608(int ch, bool mask)
        {
            maskFMChYM2608[ch] = mask;

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            setYM2608Register(p, 0x40 + c, fmRegisterYM2608[p][0x40 + c], vgm.enmModel.RealModel);
            setYM2608Register(p, 0x44 + c, fmRegisterYM2608[p][0x44 + c], vgm.enmModel.RealModel);
            setYM2608Register(p, 0x48 + c, fmRegisterYM2608[p][0x48 + c], vgm.enmModel.RealModel);
            setYM2608Register(p, 0x4c + c, fmRegisterYM2608[p][0x4c + c], vgm.enmModel.RealModel);
        }


        public void setYM2151Register(int dPort, int dAddr, int dData, vgm.enmModel model,int hosei)
        {
            if (ctYM2151 == null) return;

            fmRegisterYM2151[dAddr] = dData;

            if ((model == vgm.enmModel.RealModel && ctYM2151.UseScci) || (model == vgm.enmModel.VirtualModel && !ctYM2151.UseScci))
            {
                if (dAddr == 0x08) //Key-On/Off
                {
                    int ch = dData & 0x7;
                    if (ch >= 0 && ch < 8)
                    {
                        if ((dData & 0x78) > 0)
                        {
                            fmKeyOnYM2151[ch] = dData & 0x78;
                            //0x2x Pan/FL/CON
                            fmVolYM2151[ch][0] = (int)(256 * 6 * ((fmRegisterYM2151[0x20 + ch] & 0x80) > 0 ? 1 : 0) * ((127 - (fmRegisterYM2151[0x78 + ch] & 0x7f)) / 127.0));
                            fmVolYM2151[ch][1] = (int)(256 * 6 * ((fmRegisterYM2151[0x20 + ch] & 0x40) > 0 ? 1 : 0) * ((127 - (fmRegisterYM2151[0x78 + ch] & 0x7f)) / 127.0));
                        }
                        else
                        {
                            fmKeyOnYM2151[ch] = 0;
                        }
                    }
                }
            }

            if ((dAddr & 0xf8) == 0x20)
            {
                int al = dData & 0x07;//AL
                int ch = (dAddr & 0x7);

                for (int i = 0; i < 4; i++)
                {
                    int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        if (maskFMChYM2151[ch])
                        {
                            if (model == vgm.enmModel.VirtualModel)
                            {
                                if (!ctYM2151.UseScci)
                                {
                                    //mds.WriteYM2151((byte)(0x60 + i * 8 + ch), (byte)127);
                                }
                            }
                            else
                            {
                                scYM2151.setRegister(0x60 + i * 8 + ch, 127);
                            }
                        }
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x60 || (dAddr & 0xf0) == 0x70)//TL
            {
                int ch = (dAddr & 0x7);
                int al = fmRegisterYM2151[0x20 + ch] & 0x07;//AL
                int slot = (((dAddr & 0xf0) == 0x60) ? 0 : 2) + (((dAddr & 0x8) > 0) ? 1 : 0);
                dData &= 0x7f;

                if ((algM[al] & (1 << slot)) > 0)
                {
                    dData = Math.Min(dData + nowYM2151FadeoutVol, 127);
                    dData = maskFMChYM2151[ch] ? 127 : dData;
                }
            }

            if (model == vgm.enmModel.VirtualModel)
            {
                if (!ctYM2151.UseScci)
                {
                    //mds.WriteYM2151((byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYM2151 == null) return;

                if (dAddr >= 0x28 && dAddr <= 0x2f)
                {
                    if (hosei == 0)
                    {
                        scYM2151.setRegister(dAddr, dData);
                    }
                    else
                    {
                        int oct = (dData & 0x70) >> 4;
                        int note = dData & 0xf;
                        note = (note < 3) ? note : ((note < 7) ? (note - 1) : ((note < 11) ? (note - 2) : (note - 3)));
                        note += hosei;
                        if (note < 0)
                        {
                            oct += (note / 12) - 1;
                            note = (note % 12) + 12;
                        }
                        else
                        {
                            oct += (note / 12);
                            note %= 12;
                        }

                        note = (note < 3) ? note : ((note < 6) ? (note + 1) : ((note < 9) ? (note + 2) : (note + 3)));
                        scYM2151.setRegister(dAddr, (oct << 4) | note);
                    }
                }
                else
                {
                    scYM2151.setRegister(dAddr, dData);
                }
            }

        }

        public int getYM2151Clock()
        {
            if (scYM2151 == null) return -1;

            return scYM2151.getSoundChipInfo().getdClock();
        }

        public void setYM2151SyncWait(int wait)
        {
            if (scYM2151 != null && ctYM2151.UseWait)
            {
                scYM2151.setRegister(-1, (int)(wait * (ctYM2151.UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void setFadeoutVolYM2151(int v)
        {
            nowYM2151FadeoutVol = v;
            for (int c = 0; c < 8; c++)
            {
                setYM2151Register(0, 0x60 + c, fmRegisterYM2151[0x60 + c], vgm.enmModel.RealModel,0);
                setYM2151Register(0, 0x68 + c, fmRegisterYM2151[0x68 + c], vgm.enmModel.RealModel,0);
                setYM2151Register(0, 0x70 + c, fmRegisterYM2151[0x70 + c], vgm.enmModel.RealModel,0);
                setYM2151Register(0, 0x78 + c, fmRegisterYM2151[0x78 + c], vgm.enmModel.RealModel,0);
            }
        }

        public void setMaskYM2151(int ch, bool mask)
        {
            maskFMChYM2151[ch] = mask;

            setYM2151Register(0, 0x60 + ch, fmRegisterYM2151[0x60 + ch], vgm.enmModel.RealModel,0);
            setYM2151Register(0, 0x68 + ch, fmRegisterYM2151[0x68 + ch], vgm.enmModel.RealModel, 0);
            setYM2151Register(0, 0x70 + ch, fmRegisterYM2151[0x70 + ch], vgm.enmModel.RealModel, 0);
            setYM2151Register(0, 0x78 + ch, fmRegisterYM2151[0x78 + ch], vgm.enmModel.RealModel, 0);
        }

        public void sendDataYM2151(vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel) return;

            if (scYM2151 != null && ctYM2151.UseWait)
            {
                scYM2151.parentSoundInterface.parentNScci.sendData();
                while (!scYM2151.parentSoundInterface.parentNScci.isBufferEmpty()) { }
            }
        }


        public void setSN76489Register(int dData, vgm.enmModel model)
        {
            if (ctSN76489 == null) return;

            SN76489_Write(dData);

            if ((dData & 0x90) == 0x90)
            {
                psgVol[(dData & 0x60) >> 5][0] = 15-(dData & 0xf);
                psgVol[(dData & 0x60) >> 5][1] = 15-(dData & 0xf);

                int v = dData & 0xf;
                v = v + nowSN76489FadeoutVol;
                v = Math.Min(v, 15);
                dData = (dData & 0xf0) | v;
            }

            if (model == vgm.enmModel.RealModel)
            {
                if (ctSN76489.UseScci)
                {
                    if (scSN76489 == null) return;
                    scSN76489.setRegister(0, dData);
                }
            }
            else
            {
                if (!ctSN76489.UseScci)
                {
                    mds.WriteSN76489((byte)dData);
                }
            }
        }

        public void setSN76489SyncWait(int wait)
        {
            if (scSN76489 != null && ctSN76489.UseWait)
            {
                scSN76489.setRegister(-1, (int)(wait * (ctSN76489.UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void setFadeoutVolSN76489(int v)
        {
            nowSN76489FadeoutVol = (v & 0x78) >> 3;
            for (int c = 0; c < 4; c++)
            {
                
                setSN76489Register(0x90 + (c << 5) + psgRegister[1+(c<<1)], vgm.enmModel.RealModel);
            }
        }

        public void resetChips()
        {

            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2612Register(p, 0x40 + c, 127, vgm.enmModel.RealModel);
                    setYM2612Register(p, 0x44 + c, 127, vgm.enmModel.RealModel);
                    setYM2612Register(p, 0x48 + c, 127, vgm.enmModel.RealModel);
                    setYM2612Register(p, 0x4c + c, 127, vgm.enmModel.RealModel);
                }
            }


            for (int c = 0; c < 4; c++)
            {
                setSN76489Register(0x90 + (c << 5) + 0xf, vgm.enmModel.RealModel);
            }

            for (int p = 0; p < 2; p++)
            {
                for (int c = 0; c < 3; c++)
                {
                    setYM2608Register(p, 0x40 + c, 127, vgm.enmModel.RealModel);
                    setYM2608Register(p, 0x44 + c, 127, vgm.enmModel.RealModel);
                    setYM2608Register(p, 0x48 + c, 127, vgm.enmModel.RealModel);
                    setYM2608Register(p, 0x4c + c, 127, vgm.enmModel.RealModel);
                }
            }

            //ssg
            setYM2608Register(0, 0x08, 0, vgm.enmModel.RealModel);
            setYM2608Register(0, 0x09, 0, vgm.enmModel.RealModel);
            setYM2608Register(0, 0x0a, 0, vgm.enmModel.RealModel);

            //rhythm
            setYM2608Register(0, 0x11, 0, vgm.enmModel.RealModel);

            //adpcm
            setYM2608Register(1, 0x0b, 0, vgm.enmModel.RealModel);


            for (int c = 0; c < 8; c++)
            {
                setYM2151Register(0, 0x60 + c, 127, vgm.enmModel.RealModel, 0);
                setYM2151Register(0, 0x68 + c, 127, vgm.enmModel.RealModel, 0);
                setYM2151Register(0, 0x70 + c, 127, vgm.enmModel.RealModel, 0);
                setYM2151Register(0, 0x78 + c, 127, vgm.enmModel.RealModel, 0);
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

        public void writeC140(byte chipid, uint adr, byte data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteC140(chipid, adr, data);
        }

        public void writeC140PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength,byte[] romdata,uint SrcStartAdr, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteC140PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeOKIM6258(byte ChipID, byte Port, byte Data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteOKIM6258(ChipID, Port, Data);
        }

        public void writeOKIM6295(byte ChipID, byte Port, byte Data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
            {
                mds.WriteOKIM6295(ChipID, Port, Data);
                //System.Console.WriteLine("ChipID={0} Port={1:X} Data={2:X} ",ChipID,Port,Data);
            }
        }

        public void writeOKIM6295PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
                mds.WriteOKIM6295PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeSEGAPCM(byte ChipID, int Offset, byte Data, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
            {
                mds.WriteSEGAPCM(ChipID, Offset, Data);
                //System.Console.WriteLine("ChipID={0} Offset={1:X} Data={2:X} ", ChipID, Offset, Data);
            }
        }

        public void writeSEGAPCMPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr, vgm.enmModel model)
        {
            if (model == vgm.enmModel.VirtualModel)
            {
                mds.WriteSEGAPCMPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            }
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
            for (int i = 0; i < 8; i++)
            {
                if (fmVolYM2151[i][0] > 0) { fmVolYM2151[i][0] -= 50; if (fmVolYM2151[i][0] < 0) fmVolYM2151[i][0] = 0; }
                if (fmVolYM2151[i][1] > 0) { fmVolYM2151[i][1] -= 50; if (fmVolYM2151[i][1] < 0) fmVolYM2151[i][1] = 0; }
            }
            for (int i = 0; i < 9; i++)
            {
                if (fmVolYM2608[i][0] > 0) { fmVolYM2608[i][0] -= 50; if (fmVolYM2608[i][0] < 0) fmVolYM2608[i][0] = 0; }
                if (fmVolYM2608[i][1] > 0) { fmVolYM2608[i][1] -= 50; if (fmVolYM2608[i][1] < 0) fmVolYM2608[i][1] = 0; }
            }
            for (int i = 0; i < 4; i++)
            {
                if (fmCh3SlotVolYM2608[i] > 0) { fmCh3SlotVolYM2608[i] -= 50; if (fmCh3SlotVolYM2608[i] < 0) fmCh3SlotVolYM2608[i] = 0; }
            }
            for (int i = 0; i < 6; i++)
            {
                if (fmVolYM2608Rhythm[i][0] > 0) { fmVolYM2608Rhythm[i][0] -= 50; if (fmVolYM2608Rhythm[i][0] < 0) fmVolYM2608Rhythm[i][0] = 0; }
                if (fmVolYM2608Rhythm[i][1] > 0) { fmVolYM2608Rhythm[i][1] -= 50; if (fmVolYM2608Rhythm[i][1] < 0) fmVolYM2608Rhythm[i][1] = 0; }
            }

            if (fmVolYM2608Adpcm[0] > 0) { fmVolYM2608Adpcm[0] -= 50; if (fmVolYM2608Adpcm[0] < 0) fmVolYM2608Adpcm[0] = 0; }
            if (fmVolYM2608Adpcm[1] > 0) { fmVolYM2608Adpcm[1] -= 50; if (fmVolYM2608Adpcm[1] < 0) fmVolYM2608Adpcm[1] = 0; }
        }

        public int[][] GetFMVolume()
        {
            //if (ctYM2612.UseScci)
            //{
            return fmVol;
            //}
            //return mds.ReadFMVolume();
        }

        public int[][] GetYM2151Volume()
        {
            return fmVolYM2151;
        }

        public int[][] GetYM2608Volume()
        {
            return fmVolYM2608;
        }

        public int[] GetFMCh3SlotVolume()
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVol;
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2608Ch3SlotVolume()
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2608;
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[][] GetYM2608RhythmVolume()
        {
            return fmVolYM2608Rhythm;
        }

        public int[] GetYM2608AdpcmVolume()
        {
            return fmVolYM2608Adpcm;
        }

        public int[][] GetPSGVolume()
        {

            return psgVol;

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
