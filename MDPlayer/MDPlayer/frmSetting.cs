using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Reflection;

namespace MDPlayer
{
    public partial class frmSetting : Form
    {
        private bool asioSupported = true;
        private bool wasapiSupported = true;
        public Setting setting = null;

        public frmSetting(Setting setting)
        {
            this.setting = setting.Copy();

            InitializeComponent();

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

            List<NScci.NSoundChip> lstYM2612 = Audio.getChipList(Audio.enmScciChipType.YM2612);
            if (lstYM2612.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstYM2612)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    ucSI.cmbYM2612P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                    ucSI.cmbYM2612S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
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

            List<NScci.NSoundChip> lstSN76489 = Audio.getChipList(Audio.enmScciChipType.SN76489);
            if (lstSN76489.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstSN76489)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    ucSI.cmbSN76489P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                    ucSI.cmbSN76489S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
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

            List<NScci.NSoundChip> lstYM2608 = Audio.getChipList(Audio.enmScciChipType.YM2608);
            if (lstYM2608.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstYM2608)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    ucSI.cmbYM2608P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                    ucSI.cmbYM2608S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
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

            List<NScci.NSoundChip> lstYM2151 = Audio.getChipList(Audio.enmScciChipType.YM2151);
            if (lstYM2151.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstYM2151)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    ucSI.cmbYM2151P_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                    ucSI.cmbYM2151S_SCCI.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
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
                    if (item == setting.other.MidiInDeviceName)
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

            cbUseMIDIKeyboard.Checked = setting.other.UseMIDIKeyboard;

            cbFM1.Checked = setting.other.UseChannel[0];
            cbFM2.Checked = setting.other.UseChannel[1];
            cbFM3.Checked = setting.other.UseChannel[2];
            cbFM4.Checked = setting.other.UseChannel[3];
            cbFM5.Checked = setting.other.UseChannel[4];
            cbFM6.Checked = setting.other.UseChannel[5];
            cbPSG1.Checked = setting.other.UseChannel[6];
            cbPSG2.Checked = setting.other.UseChannel[7];
            cbPSG3.Checked = setting.other.UseChannel[8];

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
            cmbInstFormat.SelectedIndex=(int)setting.other.InstFormat;
            tbScreenFrameRate.Text = setting.other.ScreenFrameRate.ToString();
            cbAutoOpen.Checked = setting.other.AutoOpen;

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

            ////setting.YM2151Type.UseWaitBoost = cbYM2151UseWaitBoost.Checked;
            //setting.YM2151Type.OnlyPCMEmulation = cbOnlyPCMEmulation.Checked;
            //setting.YM2151Type.LatencyForEmulation = 0;
            ////if (int.TryParse(tbYM2151EmuDelay.Text, out i))
            ////{
            ////    setting.YM2151Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            ////}
            ////setting.YM2151Type.LatencyForScci = 0;
            ////if (int.TryParse(tbYM2151ScciDelay.Text, out i))
            ////{
            ////    setting.YM2151Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            ////}

            setting.other.MidiInDeviceName = cmbMIDIIN.SelectedItem != null ? cmbMIDIIN.SelectedItem.ToString() : "";
            setting.other.UseChannel[0] = cbFM1.Checked;
            setting.other.UseChannel[1] = cbFM2.Checked;
            setting.other.UseChannel[2] = cbFM3.Checked;
            setting.other.UseChannel[3] = cbFM4.Checked;
            setting.other.UseChannel[4] = cbFM5.Checked;
            setting.other.UseChannel[5] = cbFM6.Checked;
            setting.other.UseChannel[6] = cbPSG1.Checked;
            setting.other.UseChannel[7] = cbPSG2.Checked;
            setting.other.UseChannel[8] = cbPSG3.Checked;

            setting.other.UseMIDIKeyboard = cbUseMIDIKeyboard.Checked;

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
            setting.other.InstFormat = (Setting.Other.enmInstFormat)cmbInstFormat.SelectedIndex;
            if (int.TryParse(tbScreenFrameRate.Text, out i))
            {
                setting.other.ScreenFrameRate = Math.Max(Math.Min(i, 120), 10);
            }
            setting.other.AutoOpen = cbAutoOpen.Checked;

            setting.Debug_DispFrameCounter = cbDispFrameCounter.Checked;
            setting.HiyorimiMode = cbHiyorimiMode.Checked;


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
