using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Reflection;
using System.IO;

namespace MDPlayer.form
{
    public partial class frmSetting : Form
    {
        private bool asioSupported = true;
        private bool wasapiSupported = true;
        public Setting setting = null;
        private bool IsInitialOpenFolder;
        DataGridView[] dgv = null;


        public frmSetting(Setting setting)
        {
            this.setting = setting.Copy();

            InitializeComponent();

            dgv = new DataGridView[] {
                dgvMIDIoutListA,dgvMIDIoutListB,dgvMIDIoutListC,dgvMIDIoutListD,dgvMIDIoutListE
                ,dgvMIDIoutListF,dgvMIDIoutListG,dgvMIDIoutListH,dgvMIDIoutListI,dgvMIDIoutListJ
            };

            Init();
        }

        public void Init()
        {

            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("バージョン {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = Properties.Resources.cntDescription;

            this.cmbLatency.SelectedIndex = 5;

            //ASIOサポートチェック
            if (!AsioOut.isSupported())
            {
                rbAsioOut.Enabled = false;
                gbAsioOut.Enabled = false;
                asioSupported = false;
            }

            //wasapiサポートチェック
            System.OperatingSystem os = System.Environment.OSVersion;
            if (os.Platform == PlatformID.Win32NT && os.Version.Major < 6)
            {
                rbWasapiOut.Enabled = false;
                gbWasapiOut.Enabled = false;
                wasapiSupported = false;
            }


            //Comboboxへデバイスを列挙

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                cmbWaveOutDevice.Items.Add(WaveOut.GetCapabilities(i).ProductName);
            }

            foreach (DirectSoundDeviceInfo d in DirectSoundOut.Devices)
            {
                cmbDirectSoundDevice.Items.Add(d.Description);
            }

            if (wasapiSupported)
            {
                var enumerator = new MMDeviceEnumerator();
                var endPoints = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                foreach (var endPoint in endPoints)
                {
                    cmbWasapiDevice.Items.Add(string.Format("{0} ({1})", endPoint.FriendlyName, endPoint.DeviceFriendlyName));
                }
            }

            if (asioSupported)
            {
                foreach (string s in AsioOut.GetDriverNames())
                {
                    cmbAsioDevice.Items.Add(s);
                }
            }

            if (NAudio.Midi.MidiIn.NumberOfDevices > 0)
            {
                for (int i = 0; i < NAudio.Midi.MidiIn.NumberOfDevices; i++)
                {
                    cmbMIDIIN.Items.Add(NAudio.Midi.MidiIn.DeviceInfo(i).ProductName);
                }
                cmbMIDIIN.SelectedIndex = 0;
            }

            List<Setting.ChipType> lstYM2612 = Audio.getChipList(enmScciChipType.YM2612);
            if (lstYM2612.Count > 0)
            {
                foreach (Setting.ChipType ct in lstYM2612)
                {
                    ucSI.cmbYM2612P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                    ucSI.cmbYM2612S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                }
                ucSI.cmbYM2612P_SCCI.SelectedIndex = 0;
                ucSI.cmbYM2612S_SCCI.SelectedIndex = 0;
            }
            else
            {
                ucSI.rbYM2612P_SCCI.Enabled = false;
                ucSI.cmbYM2612P_SCCI.Enabled = false;
                ucSI.rbYM2612S_SCCI.Enabled = false;
                ucSI.cmbYM2612S_SCCI.Enabled = false;
            }

            List<Setting.ChipType> lstSN76489 = Audio.getChipList(enmScciChipType.SN76489);
            if (lstSN76489.Count > 0)
            {
                foreach (Setting.ChipType ct in lstSN76489)
                {
                    ucSI.cmbSN76489P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                    ucSI.cmbSN76489S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                }
                ucSI.cmbSN76489P_SCCI.SelectedIndex = 0;
                ucSI.cmbSN76489S_SCCI.SelectedIndex = 0;
            }
            else
            {
                ucSI.rbSN76489P_SCCI.Enabled = false;
                ucSI.cmbSN76489P_SCCI.Enabled = false;
                ucSI.rbSN76489S_SCCI.Enabled = false;
                ucSI.cmbSN76489S_SCCI.Enabled = false;
            }

            List<Setting.ChipType> lstYM2608 = Audio.getChipList(enmScciChipType.YM2608);
            if (lstYM2608.Count > 0)
            {
                foreach (Setting.ChipType ct in lstYM2608)
                {
                    ucSI.cmbYM2608P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                    ucSI.cmbYM2608S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                }
                ucSI.cmbYM2608P_SCCI.SelectedIndex = 0;
                ucSI.rbYM2608P_SCCI.Enabled = true;
                ucSI.cmbYM2608P_SCCI.Enabled = true;
                ucSI.cmbYM2608S_SCCI.SelectedIndex = 0;
                ucSI.rbYM2608S_SCCI.Enabled = true;
                ucSI.cmbYM2608S_SCCI.Enabled = true;
            }
            else
            {
                ucSI.rbYM2608P_SCCI.Enabled = false;
                ucSI.cmbYM2608P_SCCI.Enabled = false;
                ucSI.rbYM2608S_SCCI.Enabled = false;
                ucSI.cmbYM2608S_SCCI.Enabled = false;
            }

            List<Setting.ChipType> lstYM2610 = Audio.getChipList(enmScciChipType.YM2610);
            if (lstYM2610.Count > 0)
            {
                foreach (Setting.ChipType ct in lstYM2610)
                {
                    ucSI.cmbYM2610BP_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                    ucSI.cmbYM2610BS_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                }
                ucSI.cmbYM2610BP_SCCI.SelectedIndex = 0;
                ucSI.rbYM2610BP_SCCI.Enabled = true;
                ucSI.cmbYM2610BP_SCCI.Enabled = true;
                ucSI.cmbYM2610BS_SCCI.SelectedIndex = 0;
                ucSI.rbYM2610BS_SCCI.Enabled = true;
                ucSI.cmbYM2610BS_SCCI.Enabled = true;
            }
            else
            {
                ucSI.rbYM2610BP_SCCI.Enabled = false;
                ucSI.cmbYM2610BP_SCCI.Enabled = false;
                ucSI.rbYM2610BS_SCCI.Enabled = false;
                ucSI.cmbYM2610BS_SCCI.Enabled = false;
            }

            List<Setting.ChipType> lstYM2151 = Audio.getChipList(enmScciChipType.YM2151);
            if (lstYM2151.Count > 0)
            {
                foreach (Setting.ChipType ct in lstYM2151)
                {
                    ucSI.cmbYM2151P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                    ucSI.cmbYM2151S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                }
                ucSI.cmbYM2151P_SCCI.SelectedIndex = 0;
                ucSI.rbYM2151P_SCCI.Enabled = true;
                ucSI.cmbYM2151P_SCCI.Enabled = true;
                ucSI.cmbYM2151S_SCCI.SelectedIndex = 0;
                ucSI.rbYM2151S_SCCI.Enabled = true;
                ucSI.cmbYM2151S_SCCI.Enabled = true;
            }
            else
            {
                ucSI.rbYM2151P_SCCI.Enabled = false;
                ucSI.cmbYM2151P_SCCI.Enabled = false;
                ucSI.rbYM2151S_SCCI.Enabled = false;
                ucSI.cmbYM2151S_SCCI.Enabled = false;
            }

            List<Setting.ChipType> lstYM2203 = Audio.getChipList(enmScciChipType.YM2203);
            if (lstYM2203.Count > 0)
            {
                foreach (Setting.ChipType ct in lstYM2203)
                {
                    ucSI.cmbYM2203P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                    ucSI.cmbYM2203S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", ct.SoundLocation, ct.BusID, ct.SoundChip, ct.ChipName));
                }
                ucSI.cmbYM2203P_SCCI.SelectedIndex = 0;
                ucSI.rbYM2203P_SCCI.Enabled = true;
                ucSI.cmbYM2203P_SCCI.Enabled = true;
                ucSI.cmbYM2203S_SCCI.SelectedIndex = 0;
                ucSI.rbYM2203S_SCCI.Enabled = true;
                ucSI.cmbYM2203S_SCCI.Enabled = true;
            }
            else
            {
                ucSI.rbYM2203P_SCCI.Enabled = false;
                ucSI.cmbYM2203P_SCCI.Enabled = false;
                ucSI.rbYM2203S_SCCI.Enabled = false;
                ucSI.cmbYM2203S_SCCI.Enabled = false;
            }

            //設定内容をコントロールへ適用

            switch (setting.outputDevice.DeviceType)
            {
                case 0:
                default:
                    rbWaveOut.Checked = true;
                    break;
                case 1:
                    rbDirectSoundOut.Checked = true;
                    break;
                case 2:
                    if (wasapiSupported) rbWasapiOut.Checked = true;
                    else rbWaveOut.Checked = true;
                    break;
                case 3:
                    if (asioSupported) rbAsioOut.Checked = true;
                    else rbWaveOut.Checked = true;
                    break;
            }

            if (cmbWaveOutDevice.Items.Count > 0)
            {
                cmbWaveOutDevice.SelectedIndex = 0;
                foreach (string item in cmbWaveOutDevice.Items)
                {
                    if (item == setting.outputDevice.WaveOutDeviceName)
                    {
                        cmbWaveOutDevice.SelectedItem = item;
                    }
                }
            }

            if (cmbDirectSoundDevice.Items.Count > 0)
            {
                cmbDirectSoundDevice.SelectedIndex = 0;
                foreach (string item in cmbDirectSoundDevice.Items)
                {
                    if (item == setting.outputDevice.DirectSoundDeviceName)
                    {
                        cmbDirectSoundDevice.SelectedItem = item;
                    }
                }
            }

            if (cmbWasapiDevice.Items.Count > 0)
            {
                cmbWasapiDevice.SelectedIndex = 0;
                foreach (string item in cmbWasapiDevice.Items)
                {
                    if (item == setting.outputDevice.WasapiDeviceName)
                    {
                        cmbWasapiDevice.SelectedItem = item;
                    }
                }
            }

            if (cmbAsioDevice.Items.Count > 0)
            {
                cmbAsioDevice.SelectedIndex = 0;
                foreach (string item in cmbAsioDevice.Items)
                {
                    if (item == setting.outputDevice.AsioDeviceName)
                    {
                        cmbAsioDevice.SelectedItem = item;
                    }
                }
            }

            if (cmbMIDIIN.Items.Count > 0)
            {
                cmbMIDIIN.SelectedIndex = 0;
                foreach (string item in cmbMIDIIN.Items)
                {
                    if (item == setting.midiKbd.MidiInDeviceName)
                    {
                        cmbMIDIIN.SelectedItem = item;
                    }
                }
            }

            rbShare.Checked = setting.outputDevice.WasapiShareMode;
            rbExclusive.Checked = !setting.outputDevice.WasapiShareMode;

            lblLatency.Enabled = !rbAsioOut.Checked;
            lblLatencyUnit.Enabled = !rbAsioOut.Checked;
            cmbLatency.Enabled = !rbAsioOut.Checked;

            if (cmbLatency.Items.Contains(setting.outputDevice.Latency.ToString()))
            {
                cmbLatency.SelectedItem = setting.outputDevice.Latency.ToString();
            }

            if (!setting.YM2612Type.UseScci)
            {
                if (setting.YM2612Type.UseEmu)
                    ucSI.rbYM2612P_Emu.Checked = true;
                else
                    ucSI.rbYM2612P_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2612P_SCCI.Enabled)
                {
                    ucSI.rbYM2612P_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2612Type.SoundLocation, setting.YM2612Type.BusID, setting.YM2612Type.SoundChip);
                    if (ucSI.cmbYM2612P_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2612P_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2612P_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2612P_Emu.Checked = true;
                }
            }

            if (!setting.YM2612SType.UseScci)
            {
                if (setting.YM2612SType.UseEmu)
                    ucSI.rbYM2612S_Emu.Checked = true;
                else
                    ucSI.rbYM2612S_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2612S_SCCI.Enabled)
                {
                    ucSI.rbYM2612S_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2612SType.SoundLocation, setting.YM2612SType.BusID, setting.YM2612SType.SoundChip);
                    if (ucSI.cmbYM2612S_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2612S_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2612S_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2612S_Emu.Checked = true;
                }
            }

            if (!setting.YM2610Type.UseScci)
            {
                if (setting.YM2610Type.UseEmu)
                    ucSI.rbYM2610BP_Emu.Checked = true;
                else
                    ucSI.rbYM2610BP_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2610BP_SCCI.Enabled)
                {
                    ucSI.rbYM2610BP_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2610Type.SoundLocation, setting.YM2610Type.BusID, setting.YM2610Type.SoundChip);
                    if (ucSI.cmbYM2610BP_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2610BP_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2610BP_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2610BP_Emu.Checked = true;
                }
            }

            if (!setting.YM2610SType.UseScci)
            {
                if (setting.YM2610SType.UseEmu)
                    ucSI.rbYM2610BS_Emu.Checked = true;
                else
                    ucSI.rbYM2610BS_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2610BS_SCCI.Enabled)
                {
                    ucSI.rbYM2610BS_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2610SType.SoundLocation, setting.YM2610SType.BusID, setting.YM2610SType.SoundChip);
                    if (ucSI.cmbYM2610BS_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2610BS_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2610BS_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2610BS_Emu.Checked = true;
                }
            }

            ucSI.cbSendWait.Checked = setting.YM2612Type.UseWait;
            ucSI.cbTwice.Checked = setting.YM2612Type.UseWaitBoost;
            ucSI.cbEmulationPCMOnly.Checked = setting.YM2612Type.OnlyPCMEmulation;

            //tbYM2612EmuDelay.Text = setting.YM2612Type.LatencyForEmulation.ToString();
            //tbYM2612ScciDelay.Text = setting.YM2612Type.LatencyForScci.ToString();

            if (!setting.SN76489Type.UseScci)
            {
                if (setting.SN76489Type.UseEmu)
                    ucSI.rbSN76489P_Emu.Checked = true;
                else
                    ucSI.rbSN76489P_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbSN76489P_SCCI.Enabled)
                {
                    ucSI.rbSN76489P_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.SN76489Type.SoundLocation, setting.SN76489Type.BusID, setting.SN76489Type.SoundChip);
                    if (ucSI.cmbSN76489P_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbSN76489P_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbSN76489P_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbSN76489P_Emu.Checked = true;
                }
            }

            if (!setting.SN76489SType.UseScci)
            {
                if (setting.SN76489SType.UseEmu)
                    ucSI.rbSN76489S_Emu.Checked = true;
                else
                    ucSI.rbSN76489S_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbSN76489S_SCCI.Enabled)
                {
                    ucSI.rbSN76489S_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.SN76489SType.SoundLocation, setting.SN76489SType.BusID, setting.SN76489SType.SoundChip);
                    if (ucSI.cmbSN76489S_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbSN76489S_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbSN76489S_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbSN76489S_Emu.Checked = true;
                }
            }

            //tbSN76489EmuDelay.Text = setting.SN76489Type.LatencyForEmulation.ToString();
            //tbSN76489ScciDelay.Text = setting.SN76489Type.LatencyForScci.ToString();

            if (!setting.YM2608Type.UseScci)
            {
                if (setting.YM2608Type.UseEmu)
                    ucSI.rbYM2608P_Emu.Checked = true;
                else
                    ucSI.rbYM2608P_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2608P_SCCI.Enabled)
                {
                    ucSI.rbYM2608P_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2608Type.SoundLocation, setting.YM2608Type.BusID, setting.YM2608Type.SoundChip);
                    if (ucSI.cmbYM2608P_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2608P_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2608P_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2608P_Emu.Checked = true;
                }
            }

            if (!setting.YM2608SType.UseScci)
            {
                if (setting.YM2608SType.UseEmu)
                    ucSI.rbYM2608S_Emu.Checked = true;
                else
                    ucSI.rbYM2608S_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2608S_SCCI.Enabled)
                {
                    ucSI.rbYM2608S_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2608SType.SoundLocation, setting.YM2608SType.BusID, setting.YM2608SType.SoundChip);
                    if (ucSI.cmbYM2608S_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2608S_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2608S_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2608S_Emu.Checked = true;
                }
            }

            if (!setting.YM2151Type.UseScci)
            {
                if (setting.YM2151Type.UseEmu)
                    ucSI.rbYM2151P_Emu.Checked = true;
                else
                    ucSI.rbYM2151P_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2151P_SCCI.Enabled)
                {
                    ucSI.rbYM2151P_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2151Type.SoundLocation, setting.YM2151Type.BusID, setting.YM2151Type.SoundChip);
                    if (ucSI.cmbYM2151P_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2151P_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2151P_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2151P_Emu.Checked = true;
                }
            }

            if (!setting.YM2151SType.UseScci)
            {
                if (setting.YM2151SType.UseEmu)
                    ucSI.rbYM2151S_Emu.Checked = true;
                else
                    ucSI.rbYM2151S_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2151S_SCCI.Enabled)
                {
                    ucSI.rbYM2151S_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2151SType.SoundLocation, setting.YM2151SType.BusID, setting.YM2151SType.SoundChip);
                    if (ucSI.cmbYM2151S_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2151S_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2151S_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2151S_Emu.Checked = true;
                }
            }

            if (!setting.YM2203Type.UseScci)
            {
                if (setting.YM2203Type.UseEmu)
                    ucSI.rbYM2203P_Emu.Checked = true;
                else
                    ucSI.rbYM2203P_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2203P_SCCI.Enabled)
                {
                    ucSI.rbYM2203P_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2203Type.SoundLocation, setting.YM2203Type.BusID, setting.YM2203Type.SoundChip);
                    if (ucSI.cmbYM2203P_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2203P_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2203P_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2203P_Emu.Checked = true;
                }
            }

            if (!setting.YM2203SType.UseScci)
            {
                if (setting.YM2203SType.UseEmu)
                    ucSI.rbYM2203S_Emu.Checked = true;
                else
                    ucSI.rbYM2203S_Silent.Checked = true;
            }
            else
            {
                if (ucSI.cmbYM2203S_SCCI.Enabled)
                {
                    ucSI.rbYM2203S_SCCI.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2203SType.SoundLocation, setting.YM2203SType.BusID, setting.YM2203SType.SoundChip);
                    if (ucSI.cmbYM2203S_SCCI.Items.Count > 0)
                    {
                        foreach (string i in ucSI.cmbYM2203S_SCCI.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            ucSI.cmbYM2203S_SCCI.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    ucSI.rbYM2203S_Emu.Checked = true;
                }
            }

            cbUseMIDIKeyboard.Checked = setting.midiKbd.UseMIDIKeyboard;

            cbFM1.Checked = setting.midiKbd.UseChannel[0];
            cbFM2.Checked = setting.midiKbd.UseChannel[1];
            cbFM3.Checked = setting.midiKbd.UseChannel[2];
            cbFM4.Checked = setting.midiKbd.UseChannel[3];
            cbFM5.Checked = setting.midiKbd.UseChannel[4];
            cbFM6.Checked = setting.midiKbd.UseChannel[5];

            rbMONO.Checked = setting.midiKbd.IsMONO;
            rbPOLY.Checked = !setting.midiKbd.IsMONO;

            rbFM1.Checked = setting.midiKbd.UseMONOChannel == 0;
            rbFM2.Checked = setting.midiKbd.UseMONOChannel == 1;
            rbFM3.Checked = setting.midiKbd.UseMONOChannel == 2;
            rbFM4.Checked = setting.midiKbd.UseMONOChannel == 3;
            rbFM5.Checked = setting.midiKbd.UseMONOChannel == 4;
            rbFM6.Checked = setting.midiKbd.UseMONOChannel == 5;

            tbLatencyEmu.Text = setting.LatencyEmulation.ToString();
            tbLatencySCCI.Text = setting.LatencySCCI.ToString();

            cbDispFrameCounter.Checked = setting.Debug_DispFrameCounter;
            cbHiyorimiMode.Checked = setting.HiyorimiMode;

            cbUseLoopTimes.Checked = setting.other.UseLoopTimes;
            tbLoopTimes.Enabled = cbUseLoopTimes.Checked;
            lblLoopTimes.Enabled = cbUseLoopTimes.Checked;
            tbLoopTimes.Text = setting.other.LoopTimes.ToString();
            cbUseGetInst.Checked = setting.other.UseGetInst;
            cbUseGetInst_CheckedChanged(null, null);
            tbDataPath.Text = setting.other.DefaultDataPath;
            cmbInstFormat.SelectedIndex = (int)setting.other.InstFormat;
            tbScreenFrameRate.Text = setting.other.ScreenFrameRate.ToString();
            cbAutoOpen.Checked = setting.other.AutoOpen;
            cbDumpSwitch.Checked = setting.other.DumpSwitch;
            gbDump.Enabled = cbDumpSwitch.Checked;
            tbDumpPath.Text = setting.other.DumpPath;
            cbWavSwitch.Checked = setting.other.WavSwitch;
            gbWav.Enabled = cbWavSwitch.Checked;
            tbWavPath.Text = setting.other.WavPath;

            cbUseMIDIExport.Checked = setting.midiExport.UseMIDIExport;
            gbMIDIExport.Enabled = cbUseMIDIExport.Checked;
            tbMIDIOutputPath.Text = setting.midiExport.ExportPath;
            cbMIDIUseVOPM.Checked = setting.midiExport.UseVOPMex;
            cbMIDIYM2151.Checked = setting.midiExport.UseYM2151Export;
            cbMIDIYM2612.Checked = setting.midiExport.UseYM2612Export;

            tbCCChCopy.Text = setting.midiKbd.MidiCtrl_CopyToneFromYM2612Ch1 == -1 ? "" : setting.midiKbd.MidiCtrl_CopyToneFromYM2612Ch1.ToString();
            tbCCCopyLog.Text = setting.midiKbd.MidiCtrl_CopySelecttingLogToClipbrd == -1 ? "" : setting.midiKbd.MidiCtrl_CopySelecttingLogToClipbrd.ToString();
            tbCCDelLog.Text = setting.midiKbd.MidiCtrl_DelOneLog == -1 ? "" : setting.midiKbd.MidiCtrl_DelOneLog.ToString();
            tbCCFadeout.Text = setting.midiKbd.MidiCtrl_Fadeout == -1 ? "" : setting.midiKbd.MidiCtrl_Fadeout.ToString();
            tbCCFast.Text = setting.midiKbd.MidiCtrl_Fast == -1 ? "" : setting.midiKbd.MidiCtrl_Fast.ToString();
            tbCCNext.Text = setting.midiKbd.MidiCtrl_Next == -1 ? "" : setting.midiKbd.MidiCtrl_Next.ToString();
            tbCCPause.Text = setting.midiKbd.MidiCtrl_Pause == -1 ? "" : setting.midiKbd.MidiCtrl_Pause.ToString();
            tbCCPlay.Text = setting.midiKbd.MidiCtrl_Play == -1 ? "" : setting.midiKbd.MidiCtrl_Play.ToString();
            tbCCPrevious.Text = setting.midiKbd.MidiCtrl_Previous == -1 ? "" : setting.midiKbd.MidiCtrl_Previous.ToString();
            tbCCSlow.Text = setting.midiKbd.MidiCtrl_Slow == -1 ? "" : setting.midiKbd.MidiCtrl_Slow.ToString();
            tbCCStop.Text = setting.midiKbd.MidiCtrl_Stop == -1 ? "" : setting.midiKbd.MidiCtrl_Stop.ToString();


            if (setting.midiOut.lstMidiOutInfo != null && setting.midiOut.lstMidiOutInfo.Count > 0)
            {
                for (int i = 0; i < setting.midiOut.lstMidiOutInfo.Count; i++)
                {
                    dgv[i].Rows.Clear();
                    HashSet<int> midioutNotFound = new HashSet<int>();
                    if (setting.midiOut.lstMidiOutInfo[i] != null && setting.midiOut.lstMidiOutInfo[i].Length > 0)
                    {
                        for (int j = 0; j < setting.midiOut.lstMidiOutInfo[i].Length; j++)
                        {
                            midiOutInfo moi = setting.midiOut.lstMidiOutInfo[i][j];
                            int found = -999;
                            for (int k = 0; k < NAudio.Midi.MidiOut.NumberOfDevices; k++)
                            {
                                NAudio.Midi.MidiOutCapabilities moc = NAudio.Midi.MidiOut.DeviceInfo(k);
                                if (moi.name == moc.ProductName)
                                {
                                    midioutNotFound.Add(k);
                                    found = k;
                                    break;
                                }
                            }

                            moi.id = found;

                            string stype = "GM";
                            if (moi.type == 1) stype = "XG";
                            if (moi.type == 2) stype = "GS";

                            dgv[i].Rows.Add(
                                moi.id
                                , moi.isVST
                                , moi.fileName
                                , moi.name
                                , stype
                                , moi.isVST ? moi.vendor : (moi.manufacturer != -1 ? ((NAudio.Manufacturers)moi.manufacturer).ToString() : "Unknown")
                                );

                        }
                    }
                }
            }

            dgvMIDIoutPallet.Rows.Clear();
            for (int i = 0; i < NAudio.Midi.MidiOut.NumberOfDevices; i++)
            {
                //if (!midioutNotFound.Contains(i))
                //{
                NAudio.Midi.MidiOutCapabilities moc = NAudio.Midi.MidiOut.DeviceInfo(i);
                dgvMIDIoutPallet.Rows.Add(i, moc.ProductName, moc.Manufacturer.ToString() != "-1" ? moc.Manufacturer.ToString() : "Unknown");
                //}
            }


            cbNFSNes_UnmuteOnReset.Checked = setting.nsf.NESUnmuteOnReset;
            cbNFSNes_NonLinearMixer.Checked = setting.nsf.NESNonLinearMixer;
            cbNFSNes_PhaseRefresh.Checked = setting.nsf.NESPhaseRefresh;
            cbNFSNes_DutySwap.Checked = setting.nsf.NESDutySwap;

            tbNSFFds_LPF.Text = setting.nsf.FDSLpf.ToString();
            cbNFSFds_4085Reset.Checked = setting.nsf.FDS4085Reset;
            cbNSFFDSWriteDisable8000.Checked = setting.nsf.FDSWriteDisable8000;

            cbNSFDmc_UnmuteOnReset.Checked = setting.nsf.DMCUnmuteOnReset;
            cbNSFDmc_NonLinearMixer.Checked = setting.nsf.DMCNonLinearMixer;
            cbNSFDmc_Enable4011.Checked = setting.nsf.DMCEnable4011;
            cbNSFDmc_EnablePNoise.Checked = setting.nsf.DMCEnablePnoise;
            cbNSFDmc_DPCMAntiClick.Checked = setting.nsf.DMCDPCMAntiClick;
            cbNSFDmc_RandomizeNoise.Checked = setting.nsf.DMCRandomizeNoise;
            cbNSFDmc_TriMute.Checked = setting.nsf.DMCTRImute;
            cbNSFDmc_TriNull.Checked = setting.nsf.DMCTRINull;

            cbNSFMmc5_NonLinearMixer.Checked = setting.nsf.MMC5NonLinearMixer;
            cbNSFMmc5_PhaseRefresh.Checked = setting.nsf.MMC5PhaseRefresh;

            cbNSFN160_Serial.Checked = setting.nsf.N160Serial;

            tbSIDKernal.Text = setting.sid.RomKernalPath;
            tbSIDBasic.Text = setting.sid.RomBasicPath;
            tbSIDCharacter.Text = setting.sid.RomCharacterPath;

        }

        private void btnASIOControlPanel_Click(object sender, EventArgs e)
        {
            try
            {
                using (var asio = new AsioOut(cmbAsioDevice.SelectedItem.ToString()))
                {
                    asio.ShowControlPanel();
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int i = 0;

            setting.outputDevice.DeviceType = 0;
            if (rbWaveOut.Checked) setting.outputDevice.DeviceType = 0;
            if (rbDirectSoundOut.Checked) setting.outputDevice.DeviceType = 1;
            if (rbWasapiOut.Checked) setting.outputDevice.DeviceType = 2;
            if (rbAsioOut.Checked) setting.outputDevice.DeviceType = 3;

            setting.outputDevice.WaveOutDeviceName = cmbWaveOutDevice.SelectedItem != null ? cmbWaveOutDevice.SelectedItem.ToString() : "";
            setting.outputDevice.DirectSoundDeviceName = cmbDirectSoundDevice.SelectedItem != null ? cmbDirectSoundDevice.SelectedItem.ToString() : "";
            setting.outputDevice.WasapiDeviceName = cmbWasapiDevice.SelectedItem != null ? cmbWasapiDevice.SelectedItem.ToString() : "";
            setting.outputDevice.AsioDeviceName = cmbAsioDevice.SelectedItem != null ? cmbAsioDevice.SelectedItem.ToString() : "";

            setting.outputDevice.WasapiShareMode = rbShare.Checked;
            setting.outputDevice.Latency = int.Parse(cmbLatency.SelectedItem.ToString());

            setting.YM2612Type = new Setting.ChipType();
            setting.YM2612Type.UseScci = ucSI.rbYM2612P_SCCI.Checked;
            if (ucSI.rbYM2612P_SCCI.Checked)
            {
                if (ucSI.cmbYM2612P_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2612P_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2612Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2612Type.BusID = int.Parse(ns[1]);
                    setting.YM2612Type.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2612Type.UseEmu = ucSI.rbYM2612P_Emu.Checked;

            setting.YM2612SType = new Setting.ChipType();
            setting.YM2612SType.UseScci = ucSI.rbYM2612S_SCCI.Checked;
            if (ucSI.rbYM2612S_SCCI.Checked)
            {
                if (ucSI.cmbYM2612S_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2612S_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2612SType.SoundLocation = int.Parse(ns[0]);
                    setting.YM2612SType.BusID = int.Parse(ns[1]);
                    setting.YM2612SType.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2612SType.UseEmu = ucSI.rbYM2612S_Emu.Checked;


            setting.YM2612Type.UseWait = ucSI.cbSendWait.Checked;
            setting.YM2612Type.UseWaitBoost = ucSI.cbTwice.Checked;
            setting.YM2612Type.OnlyPCMEmulation = ucSI.cbEmulationPCMOnly.Checked;


            //setting.YM2612Type.LatencyForEmulation = 0;
            //if (int.TryParse(tbYM2612EmuDelay.Text, out i))
            //{
            //    setting.YM2612Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            //}
            //setting.YM2612Type.LatencyForScci = 0;
            //if (int.TryParse(tbYM2612ScciDelay.Text, out i))
            //{
            //    setting.YM2612Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            //}

            setting.SN76489Type = new Setting.ChipType();
            setting.SN76489Type.UseScci = ucSI.rbSN76489P_SCCI.Checked;
            if (ucSI.rbSN76489P_SCCI.Checked)
            {
                string n = ucSI.cmbSN76489P_SCCI.SelectedItem.ToString();
                n = n.Substring(0, n.IndexOf(")")).Substring(1);
                string[] ns = n.Split(':');
                setting.SN76489Type.SoundLocation = int.Parse(ns[0]);
                setting.SN76489Type.BusID = int.Parse(ns[1]);
                setting.SN76489Type.SoundChip = int.Parse(ns[2]);
            }
            setting.SN76489Type.UseEmu = ucSI.rbSN76489P_Emu.Checked;

            setting.SN76489SType = new Setting.ChipType();
            setting.SN76489SType.UseScci = ucSI.rbSN76489S_SCCI.Checked;
            if (ucSI.rbSN76489S_SCCI.Checked)
            {
                string n = ucSI.cmbSN76489S_SCCI.SelectedItem.ToString();
                n = n.Substring(0, n.IndexOf(")")).Substring(1);
                string[] ns = n.Split(':');
                setting.SN76489SType.SoundLocation = int.Parse(ns[0]);
                setting.SN76489SType.BusID = int.Parse(ns[1]);
                setting.SN76489SType.SoundChip = int.Parse(ns[2]);
            }
            setting.SN76489SType.UseEmu = ucSI.rbSN76489S_Emu.Checked;

            //setting.SN76489Type.LatencyForEmulation = 0;
            //if (int.TryParse(tbSN76489EmuDelay.Text, out i))
            //{
            //    setting.SN76489Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            //}
            //setting.SN76489Type.LatencyForScci = 0;
            //if (int.TryParse(tbSN76489ScciDelay.Text, out i))
            //{
            //    setting.SN76489Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            //}

            setting.YM2608Type = new Setting.ChipType();
            setting.YM2608Type.UseScci = ucSI.rbYM2608P_SCCI.Checked;
            if (ucSI.rbYM2608P_SCCI.Checked)
            {
                if (ucSI.cmbYM2608P_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2608P_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2608Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2608Type.BusID = int.Parse(ns[1]);
                    setting.YM2608Type.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2608Type.UseEmu = ucSI.rbYM2608P_Emu.Checked;

            setting.YM2608SType = new Setting.ChipType();
            setting.YM2608SType.UseScci = ucSI.rbYM2608S_SCCI.Checked;
            if (ucSI.rbYM2608S_SCCI.Checked)
            {
                if (ucSI.cmbYM2608S_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2608S_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2608SType.SoundLocation = int.Parse(ns[0]);
                    setting.YM2608SType.BusID = int.Parse(ns[1]);
                    setting.YM2608SType.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2608SType.UseEmu = ucSI.rbYM2608S_Emu.Checked;

            setting.YM2610Type = new Setting.ChipType();
            setting.YM2610Type.UseScci = ucSI.rbYM2610BP_SCCI.Checked;
            if (ucSI.rbYM2610BP_SCCI.Checked)
            {
                if (ucSI.cmbYM2610BP_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2610BP_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2610Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2610Type.BusID = int.Parse(ns[1]);
                    setting.YM2610Type.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2610Type.UseEmu = ucSI.rbYM2610BP_Emu.Checked;

            setting.YM2610SType = new Setting.ChipType();
            setting.YM2610SType.UseScci = ucSI.rbYM2610BS_SCCI.Checked;
            if (ucSI.rbYM2610BS_SCCI.Checked)
            {
                if (ucSI.cmbYM2610BS_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2610BS_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2610SType.SoundLocation = int.Parse(ns[0]);
                    setting.YM2610SType.BusID = int.Parse(ns[1]);
                    setting.YM2610SType.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2610SType.UseEmu = ucSI.rbYM2610BS_Emu.Checked;

            //setting.YM2608Type.UseWaitBoost = cbYM2608UseWaitBoost.Checked;
            //setting.YM2608Type.OnlyPCMEmulation = cbOnlyPCMEmulation.Checked;
            //setting.YM2608Type.LatencyForEmulation = 0;
            //if (int.TryParse(tbYM2608EmuDelay.Text, out i))
            //{
            //    setting.YM2608Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            //}
            //setting.YM2608Type.LatencyForScci = 0;
            //if (int.TryParse(tbYM2608ScciDelay.Text, out i))
            //{
            //    setting.YM2608Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            //}

            setting.YM2151Type = new Setting.ChipType();
            setting.YM2151Type.UseScci = ucSI.rbYM2151P_SCCI.Checked;
            if (ucSI.rbYM2151P_SCCI.Checked)
            {
                if (ucSI.cmbYM2151P_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2151P_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2151Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2151Type.BusID = int.Parse(ns[1]);
                    setting.YM2151Type.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2151Type.UseEmu = ucSI.rbYM2151P_Emu.Checked;

            setting.YM2151SType = new Setting.ChipType();
            setting.YM2151SType.UseScci = ucSI.rbYM2151S_SCCI.Checked;
            if (ucSI.rbYM2151S_SCCI.Checked)
            {
                if (ucSI.cmbYM2151S_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2151S_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2151SType.SoundLocation = int.Parse(ns[0]);
                    setting.YM2151SType.BusID = int.Parse(ns[1]);
                    setting.YM2151SType.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2151SType.UseEmu = ucSI.rbYM2151S_Emu.Checked;

            setting.YM2203Type = new Setting.ChipType();
            setting.YM2203Type.UseScci = ucSI.rbYM2203P_SCCI.Checked;
            if (ucSI.rbYM2203P_SCCI.Checked)
            {
                if (ucSI.cmbYM2203P_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2203P_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2203Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2203Type.BusID = int.Parse(ns[1]);
                    setting.YM2203Type.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2203Type.UseEmu = ucSI.rbYM2203P_Emu.Checked;

            setting.YM2203SType = new Setting.ChipType();
            setting.YM2203SType.UseScci = ucSI.rbYM2203S_SCCI.Checked;
            if (ucSI.rbYM2203S_SCCI.Checked)
            {
                if (ucSI.cmbYM2203S_SCCI.SelectedItem != null)
                {
                    string n = ucSI.cmbYM2203S_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2203SType.SoundLocation = int.Parse(ns[0]);
                    setting.YM2203SType.BusID = int.Parse(ns[1]);
                    setting.YM2203SType.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2203SType.UseEmu = ucSI.rbYM2203S_Emu.Checked;


            setting.midiKbd.MidiInDeviceName = cmbMIDIIN.SelectedItem != null ? cmbMIDIIN.SelectedItem.ToString() : "";
            setting.midiKbd.UseChannel[0] = cbFM1.Checked;
            setting.midiKbd.UseChannel[1] = cbFM2.Checked;
            setting.midiKbd.UseChannel[2] = cbFM3.Checked;
            setting.midiKbd.UseChannel[3] = cbFM4.Checked;
            setting.midiKbd.UseChannel[4] = cbFM5.Checked;
            setting.midiKbd.UseChannel[5] = cbFM6.Checked;

            setting.midiKbd.UseMIDIKeyboard = cbUseMIDIKeyboard.Checked;

            setting.midiKbd.IsMONO = rbMONO.Checked;
            setting.midiKbd.UseMONOChannel = rbFM1.Checked ? 0 : (rbFM2.Checked ? 1 : (rbFM3.Checked ? 2 : (rbFM4.Checked ? 3 : (rbFM5.Checked ? 4 : (rbFM6.Checked ? 5 : -1)))));

            setting.midiKbd.MidiCtrl_CopySelecttingLogToClipbrd = -1;
            if (int.TryParse(tbCCCopyLog.Text, out i)) setting.midiKbd.MidiCtrl_CopySelecttingLogToClipbrd = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_CopyToneFromYM2612Ch1 = -1;
            if (int.TryParse(tbCCChCopy.Text, out i)) setting.midiKbd.MidiCtrl_CopyToneFromYM2612Ch1 = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_DelOneLog = -1;
            if (int.TryParse(tbCCDelLog.Text, out i)) setting.midiKbd.MidiCtrl_DelOneLog = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Fadeout = -1;
            if (int.TryParse(tbCCFadeout.Text, out i)) setting.midiKbd.MidiCtrl_Fadeout = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Fast = -1;
            if (int.TryParse(tbCCFast.Text, out i)) setting.midiKbd.MidiCtrl_Fast = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Next = -1;
            if (int.TryParse(tbCCNext.Text, out i)) setting.midiKbd.MidiCtrl_Next = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Pause = -1;
            if (int.TryParse(tbCCPause.Text, out i)) setting.midiKbd.MidiCtrl_Pause = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Play = -1;
            if (int.TryParse(tbCCPlay.Text, out i)) setting.midiKbd.MidiCtrl_Play = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Previous = -1;
            if (int.TryParse(tbCCPrevious.Text, out i)) setting.midiKbd.MidiCtrl_Previous = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Slow = -1;
            if (int.TryParse(tbCCSlow.Text, out i)) setting.midiKbd.MidiCtrl_Slow = Math.Min(Math.Max(i, 0), 127);
            setting.midiKbd.MidiCtrl_Stop = -1;
            if (int.TryParse(tbCCStop.Text, out i)) setting.midiKbd.MidiCtrl_Stop = Math.Min(Math.Max(i, 0), 127);

            if (int.TryParse(tbLatencyEmu.Text, out i))
            {
                setting.LatencyEmulation = Math.Max(Math.Min(i, 999), 0);
            }
            if (int.TryParse(tbLatencySCCI.Text, out i))
            {
                setting.LatencySCCI = Math.Max(Math.Min(i, 999), 0);
            }

            setting.other.UseLoopTimes = cbUseLoopTimes.Checked;
            if (int.TryParse(tbLoopTimes.Text, out i))
            {
                setting.other.LoopTimes = Math.Max(Math.Min(i, 999), 1);
            }

            setting.other.UseGetInst = cbUseGetInst.Checked;
            setting.other.DefaultDataPath = tbDataPath.Text;
            setting.other.InstFormat = (enmInstFormat)cmbInstFormat.SelectedIndex;
            if (int.TryParse(tbScreenFrameRate.Text, out i))
            {
                setting.other.ScreenFrameRate = Math.Max(Math.Min(i, 120), 10);
            }
            setting.other.AutoOpen = cbAutoOpen.Checked;
            setting.other.DumpSwitch = cbDumpSwitch.Checked;
            setting.other.DumpPath = tbDumpPath.Text;
            setting.other.WavSwitch = cbWavSwitch.Checked;
            setting.other.WavPath = tbWavPath.Text;

            setting.Debug_DispFrameCounter = cbDispFrameCounter.Checked;
            setting.HiyorimiMode = cbHiyorimiMode.Checked;

            setting.midiExport.UseMIDIExport=cbUseMIDIExport.Checked;
            setting.midiExport.ExportPath = tbMIDIOutputPath.Text;
            setting.midiExport.UseVOPMex = cbMIDIUseVOPM.Checked;
            setting.midiExport.UseYM2151Export = cbMIDIYM2151.Checked;
            setting.midiExport.UseYM2612Export = cbMIDIYM2612.Checked;

            setting.midiOut.lstMidiOutInfo = new List<midiOutInfo[]>();

            foreach (DataGridView d in dgv)
            {
                if (d.Rows.Count > 0)
                {
                    List<midiOutInfo> lstMoi = new List<midiOutInfo>();
                    for (i = 0; i < d.Rows.Count; i++)
                    {
                        midiOutInfo moi = new midiOutInfo();
                        moi.id = (int)d.Rows[i].Cells[0].Value;
                        moi.isVST = (bool)d.Rows[i].Cells[1].Value;
                        moi.fileName = (string)d.Rows[i].Cells[2].Value;
                        moi.name = (string)d.Rows[i].Cells[3].Value;
                        string stype = (string)d.Rows[i].Cells[4].Value;
                        moi.type = 0;
                        if (stype == "XG") moi.type = 1;
                        if (stype == "GS") moi.type = 2;
                        string mn = (string)d.Rows[i].Cells[5].Value;
                        if (moi.isVST)
                        {
                            moi.vendor = mn;
                            moi.manufacturer = -1;
                        }
                        else
                        {
                            moi.vendor = "";
                            moi.manufacturer = mn == "Unknown" ? -1 : (int)(Enum.Parse(typeof(NAudio.Manufacturers), mn));
                        }

                        lstMoi.Add(moi);
                    }
                    setting.midiOut.lstMidiOutInfo.Add(lstMoi.ToArray());
                }
                else
                {
                    setting.midiOut.lstMidiOutInfo.Add(null);
                }
            }

            setting.nsf.NESUnmuteOnReset = cbNFSNes_UnmuteOnReset.Checked;
            setting.nsf.NESNonLinearMixer = cbNFSNes_NonLinearMixer.Checked;
            setting.nsf.NESPhaseRefresh = cbNFSNes_PhaseRefresh.Checked;
            setting.nsf.NESDutySwap = cbNFSNes_DutySwap.Checked;

            if (int.TryParse(tbNSFFds_LPF.Text, out i)) setting.nsf.FDSLpf = Math.Min(Math.Max(i, 0), 99999);
            setting.nsf.FDS4085Reset = cbNFSFds_4085Reset.Checked;
            setting.nsf.FDSWriteDisable8000 = cbNSFFDSWriteDisable8000.Checked;

            setting.nsf.DMCUnmuteOnReset = cbNSFDmc_UnmuteOnReset.Checked;
            setting.nsf.DMCNonLinearMixer = cbNSFDmc_NonLinearMixer.Checked;
            setting.nsf.DMCEnable4011 = cbNSFDmc_Enable4011.Checked;
            setting.nsf.DMCEnablePnoise = cbNSFDmc_EnablePNoise.Checked;
            setting.nsf.DMCDPCMAntiClick = cbNSFDmc_DPCMAntiClick.Checked;
            setting.nsf.DMCRandomizeNoise = cbNSFDmc_RandomizeNoise.Checked;
            setting.nsf.DMCTRImute = cbNSFDmc_TriMute.Checked;
            setting.nsf.DMCTRINull = cbNSFDmc_TriNull.Checked;

            setting.nsf.MMC5NonLinearMixer = cbNSFMmc5_NonLinearMixer.Checked;
            setting.nsf.MMC5PhaseRefresh = cbNSFMmc5_PhaseRefresh.Checked;

            setting.nsf.N160Serial = cbNSFN160_Serial.Checked;

            setting.sid = new Setting.SID();
            setting.sid.RomKernalPath = tbSIDKernal.Text;
            setting.sid.RomBasicPath = tbSIDBasic.Text;
            setting.sid.RomCharacterPath = tbSIDCharacter.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region アセンブリ属性アクセサー

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void cbUseMIDIKeyboard_CheckedChanged(object sender, EventArgs e)
        {
            gbMIDIKeyboard.Enabled = cbUseMIDIKeyboard.Checked;
        }

        private void rbWaveOut_CheckedChanged(object sender, EventArgs e)
        {
            lblLatency.Enabled = true;
            lblLatencyUnit.Enabled = true;
            cmbLatency.Enabled = true;
        }

        private void rbDirectSoundOut_CheckedChanged(object sender, EventArgs e)
        {
            lblLatency.Enabled = true;
            lblLatencyUnit.Enabled = true;
            cmbLatency.Enabled = true;
        }

        private void rbWasapiOut_CheckedChanged(object sender, EventArgs e)
        {
            lblLatency.Enabled = true;
            lblLatencyUnit.Enabled = true;
            cmbLatency.Enabled = true;
        }

        private void rbAsioOut_CheckedChanged(object sender, EventArgs e)
        {
            lblLatency.Enabled = false;
            lblLatencyUnit.Enabled = false;
            cmbLatency.Enabled = false;
        }

        private void cbUseLoopTimes_CheckedChanged(object sender, EventArgs e)
        {
            tbLoopTimes.Enabled = cbUseLoopTimes.Checked;
            lblLoopTimes.Enabled = cbUseLoopTimes.Checked;
        }

        private void btnOpenSettingFolder_Click(object sender, EventArgs e)
        {
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fullPath = System.IO.Path.Combine(fullPath, "KumaApp", AssemblyTitle);
            if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
            System.Diagnostics.Process.Start(fullPath);
        }

        private void btnDataPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "フォルダーを指定してください。";
            

            if (fbd.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            tbDataPath.Text = fbd.SelectedPath;

        }

        private void cbUseGetInst_CheckedChanged(object sender, EventArgs e)
        {
            lblInstFormat.Enabled = cbUseGetInst.Checked;
            cmbInstFormat.Enabled = cbUseGetInst.Checked;
        }

        private void cbDumpSwitch_CheckedChanged(object sender, EventArgs e)
        {
            gbDump.Enabled = cbDumpSwitch.Checked;
        }

        private void btnDumpPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "フォルダーを指定してください。";


            if (fbd.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            tbDumpPath.Text = fbd.SelectedPath;

        }

        private void btnResetPosition_Click(object sender, EventArgs e)
        {
            DialogResult res= MessageBox.Show("表示位置を全てリセットします。よろしいですか。(現在開いているウィンドウの位置はリセットできません。)", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No) return;

            setting.location = new Setting.Location();
        }

        private void cbUseMIDIExport_CheckedChanged(object sender, EventArgs e)
        {
            gbMIDIExport.Enabled = cbUseMIDIExport.Checked;
        }

        private void btnMIDIOutputPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "フォルダーを指定してください。";


            if (fbd.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            tbMIDIOutputPath.Text = fbd.SelectedPath;
        }

        private void btnWavPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "フォルダーを指定してください。";


            if (fbd.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            tbWavPath.Text = fbd.SelectedPath;
        }

        private void cbWavSwitch_CheckedChanged(object sender, EventArgs e)
        {
            gbWav.Enabled = cbWavSwitch.Checked;
        }

        private void btVST_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "VST Pluginファイル(*.dll)|*.dll|すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            //ofd.FilterIndex = setting.other.FilterIndex;

            if (setting.other.DefaultDataPath != "" && Directory.Exists(setting.other.DefaultDataPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = setting.other.DefaultDataPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            IsInitialOpenFolder = false;
            //setting.other.FilterIndex = ofd.FilterIndex;

            tbVST.Text = ofd.FileName;

        }

        private void btnAddMIDIout_Click(object sender, EventArgs e)
        {
            if (dgvMIDIoutPallet.SelectedRows == null || dgvMIDIoutPallet.SelectedRows.Count < 1) return;

            int p = tbcMIDIoutList.SelectedIndex;

            foreach (DataGridViewRow row in dgvMIDIoutPallet.SelectedRows)
            {
                bool found = false;
                foreach (DataGridViewRow r in dgv[p].Rows)
                {
                    if (r.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) dgv[p].Rows.Add(row.Cells[0].Value, false, "", row.Cells[1].Value, "GM", row.Cells[2].Value);
            }
        }

        private void btnSubMIDIout_Click(object sender, EventArgs e)
        {
            int p = tbcMIDIoutList.SelectedIndex;

            if (dgv[p].SelectedRows == null || dgv[p].SelectedRows.Count < 1) return;

            foreach (DataGridViewRow row in dgv[p].SelectedRows)
            {
                dgv[p].Rows.Remove(row);
            }
        }

        private void btnUP_Click(object sender, EventArgs e)
        {
            int p = tbcMIDIoutList.SelectedIndex;

            if (dgv[p].SelectedRows == null || dgv[p].SelectedRows.Count < 1) return;

            foreach (DataGridViewRow row in dgv[p].SelectedRows)
            {
                if (row.Index < 1) continue;

                int i = row.Index-1;
                dgv[p].Rows.Insert(i, row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value);
                dgv[p].Rows.Remove(row);
                dgv[p].Rows[i].Selected = true;
            }
        }

        private void btnDOWN_Click(object sender, EventArgs e)
        {
            int p = tbcMIDIoutList.SelectedIndex;

            if (dgv[p].SelectedRows == null || dgv[p].SelectedRows.Count < 1) return;

            foreach (DataGridViewRow row in dgv[p].SelectedRows)
            {
                if (row.Index > dgv[p].Rows.Count-2) continue;

                int i = row.Index + 1;
                dgv[p].Rows.Insert(row.Index+2, row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value);
                dgv[p].Rows.Remove(row);
                dgv[p].Rows[i].Selected = true;
            }
        }

        private void btnAddVST_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "VST Pluginファイル(*.dll)|*.dll|すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            //ofd.FilterIndex = setting.other.FilterIndex;

            if (setting.vst.DefaultPath != "" && Directory.Exists(setting.vst.DefaultPath) && IsInitialOpenFolder)
            {
                ofd.InitialDirectory = setting.vst.DefaultPath;
            }
            else
            {
                ofd.RestoreDirectory = true;
            }
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            vstInfo s = Audio.getVSTInfo(ofd.FileName);

            setting.vst.DefaultPath = Path.GetDirectoryName(ofd.FileName);

            int p = tbcMIDIoutList.SelectedIndex;
            dgv[p].Rows.Add(
                -999 
                , true 
                , s.fileName
                , s.effectName
                , "GM"
                , s.vendorName);

        }

        private void btnSIDKernal_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            tbSIDKernal.Text = ofd.FileName;
        }

        private void btnSIDBasic_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            tbSIDBasic.Text = ofd.FileName;
        }

        private void btnSIDCharacter_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            tbSIDCharacter.Text = ofd.FileName;
        }
    }


    public class BindData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }


}
