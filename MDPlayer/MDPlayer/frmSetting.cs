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
            bs1.DataSource = new BindData();
            bs2.DataSource = new BindData();
            bs3.DataSource = new BindData();
            bs4.DataSource = new BindData();
            bs5.DataSource = new BindData();
            bs6.DataSource = new BindData();
            bs7.DataSource = new BindData();
            bs8.DataSource = new BindData();
            bs9.DataSource = new BindData();
            bs10.DataSource = new BindData();
            bs11.DataSource = new BindData();
            bs12.DataSource = new BindData();

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
                    cmbYM2612Scci.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                }
                cmbYM2612Scci.SelectedIndex = 0;
            }
            else
            {
                rbYM2612Scci.Enabled = false;
                cmbYM2612Scci.Enabled = false;
            }

            List<NScci.NSoundChip> lstSN76489 = Audio.getChipList(Audio.enmScciChipType.SN76489);
            if (lstSN76489.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstSN76489)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    cmbSN76489Scci.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                }
                cmbSN76489Scci.SelectedIndex = 0;
            }
            else
            {
                rbSN76489Scci.Enabled = false;
                cmbSN76489Scci.Enabled = false;
            }

            List<NScci.NSoundChip> lstYM2608 = Audio.getChipList(Audio.enmScciChipType.YM2608);
            if (lstYM2608.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstYM2608)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    cmbYM2608Scci.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                }
                cmbYM2608Scci.SelectedIndex = 0;
                rbYM2608Scci.Enabled = true;
                cmbYM2608Scci.Enabled = true;
            }
            else
            {
                rbYM2608Scci.Enabled = false;
                cmbYM2608Scci.Enabled = false;
            }

            List<NScci.NSoundChip> lstYM2151 = Audio.getChipList(Audio.enmScciChipType.YM2151);
            if (lstYM2151.Count > 0)
            {
                foreach (NScci.NSoundChip sc in lstYM2151)
                {
                    NScci.NSCCI_SOUND_CHIP_INFO info = sc.getSoundChipInfo();
                    cmbYM2151Scci.Items.Add(string.Format("({0}:{1}:{2}){3}", info.getdSoundLocation(), info.getdBusID(), info.getiSoundChip(), info.getcSoundChipName()));
                }
                cmbYM2151Scci.SelectedIndex = 0;
                rbYM2151Scci.Enabled = true;
                cmbYM2151Scci.Enabled = true;
            }
            else
            {
                rbYM2151Scci.Enabled = false;
                cmbYM2151Scci.Enabled = false;
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
                rbYM2612Emu.Checked = true;
            }
            else
            {
                if (cmbYM2612Scci.Enabled)
                {
                    rbYM2612Scci.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2612Type.SoundLocation, setting.YM2612Type.BusID, setting.YM2612Type.SoundChip);
                    if (cmbYM2612Scci.Items.Count > 0)
                    {
                        foreach (string i in cmbYM2612Scci.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            cmbYM2612Scci.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    rbYM2612Emu.Checked = true;
                }
            }

            cbYM2612UseWait.Checked = setting.YM2612Type.UseWait;
            cbYM2612UseWaitBoost.Checked = setting.YM2612Type.UseWaitBoost;
            cbOnlyPCMEmulation.Checked = setting.YM2612Type.OnlyPCMEmulation;
            tbYM2612EmuDelay.Text = setting.YM2612Type.LatencyForEmulation.ToString();
            tbYM2612ScciDelay.Text = setting.YM2612Type.LatencyForScci.ToString();

            if (!setting.SN76489Type.UseScci)
            {
                rbSN76489Emu.Checked = true;
            }
            else
            {
                if (cmbSN76489Scci.Enabled)
                {
                    rbSN76489Scci.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.SN76489Type.SoundLocation, setting.SN76489Type.BusID, setting.SN76489Type.SoundChip);
                    if (cmbSN76489Scci.Items.Count > 0)
                    {
                        foreach (string i in cmbSN76489Scci.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            cmbSN76489Scci.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    rbSN76489Emu.Checked = true;
                }
            }

            tbSN76489EmuDelay.Text = setting.SN76489Type.LatencyForEmulation.ToString();
            tbSN76489ScciDelay.Text = setting.SN76489Type.LatencyForScci.ToString();

            if (!setting.YM2608Type.UseScci)
            {
                rbYM2608Emu.Checked = true;
            }
            else
            {
                if (cmbYM2608Scci.Enabled)
                {
                    rbYM2608Scci.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2608Type.SoundLocation, setting.YM2608Type.BusID, setting.YM2608Type.SoundChip);
                    if (cmbYM2608Scci.Items.Count > 0)
                    {
                        foreach (string i in cmbYM2608Scci.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            cmbYM2608Scci.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    rbYM2608Emu.Checked = true;
                }
            }

            if (!setting.YM2151Type.UseScci)
            {
                rbYM2151Emu.Checked = true;
            }
            else
            {
                if (cmbYM2151Scci.Enabled)
                {
                    rbYM2151Scci.Checked = true;
                    string n = string.Format("({0}:{1}:{2})", setting.YM2151Type.SoundLocation, setting.YM2151Type.BusID, setting.YM2151Type.SoundChip);
                    if (cmbYM2151Scci.Items.Count > 0)
                    {
                        foreach (string i in cmbYM2151Scci.Items)
                        {
                            if (i.IndexOf(n) < 0) continue;
                            cmbYM2151Scci.SelectedItem = i;
                            break;
                        }
                    }
                }
                else
                {
                    rbYM2151Emu.Checked = true;
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

            ((BindData)(bs1.DataSource)).Value = setting.balance.YM2612Volume;
            ((BindData)(bs2.DataSource)).Value = setting.balance.SN76489Volume;
            ((BindData)(bs3.DataSource)).Value = setting.balance.RF5C164Volume;
            ((BindData)(bs4.DataSource)).Value = setting.balance.PWMVolume;
            ((BindData)(bs5.DataSource)).Value = setting.balance.C140Volume;
            ((BindData)(bs6.DataSource)).Value = setting.balance.OKIM6258Volume;
            ((BindData)(bs7.DataSource)).Value = setting.balance.OKIM6295Volume;
            ((BindData)(bs8.DataSource)).Value = setting.balance.SEGAPCMVolume;
            ((BindData)(bs9.DataSource)).Value = setting.balance.YM2151Volume;
            ((BindData)(bs10.DataSource)).Value = setting.balance.YM2608Volume;
            ((BindData)(bs11.DataSource)).Value = setting.balance.YM2203Volume;
            ((BindData)(bs12.DataSource)).Value = setting.balance.YM2610Volume;

            trkYM2612.Value = setting.balance.YM2612Volume;
            trkSN76489.Value = setting.balance.SN76489Volume;
            trkRF5C164.Value = setting.balance.RF5C164Volume;
            trkPWM.Value = setting.balance.PWMVolume;
            trkOKIM6258.Value = setting.balance.OKIM6258Volume;
            trkOKIM6295.Value = setting.balance.OKIM6295Volume;
            trkC140.Value = setting.balance.C140Volume;

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
            setting.YM2612Type.UseScci = rbYM2612Scci.Checked;
            if (rbYM2612Scci.Checked)
            {
                if (cmbYM2612Scci.SelectedItem != null)
                {
                    string n = cmbYM2612Scci.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2612Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2612Type.BusID = int.Parse(ns[1]);
                    setting.YM2612Type.SoundChip = int.Parse(ns[2]);
                }
            }
            setting.YM2612Type.UseWait = cbYM2612UseWait.Checked;
            setting.YM2612Type.UseWaitBoost=cbYM2612UseWaitBoost.Checked;
            setting.YM2612Type.OnlyPCMEmulation=cbOnlyPCMEmulation.Checked;
            setting.YM2612Type.LatencyForEmulation = 0;
            if (int.TryParse(tbYM2612EmuDelay.Text, out i))
            {
                setting.YM2612Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            }
            setting.YM2612Type.LatencyForScci = 0;
            if (int.TryParse(tbYM2612ScciDelay.Text, out i))
            {
                setting.YM2612Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            }

            setting.SN76489Type = new Setting.ChipType();
            setting.SN76489Type.UseScci = rbSN76489Scci.Checked;
            if (rbSN76489Scci.Checked)
            {
                string n = cmbSN76489Scci.SelectedItem.ToString();
                n = n.Substring(0, n.IndexOf(")")).Substring(1);
                string[] ns = n.Split(':');
                setting.SN76489Type.SoundLocation = int.Parse(ns[0]);
                setting.SN76489Type.BusID = int.Parse(ns[1]);
                setting.SN76489Type.SoundChip = int.Parse(ns[2]);
            }
            setting.SN76489Type.LatencyForEmulation = 0;
            if (int.TryParse(tbSN76489EmuDelay.Text, out i))
            {
                setting.SN76489Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            }
            setting.SN76489Type.LatencyForScci = 0;
            if (int.TryParse(tbSN76489ScciDelay.Text, out i))
            {
                setting.SN76489Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            }

            setting.YM2608Type = new Setting.ChipType();
            setting.YM2608Type.UseScci = rbYM2608Scci.Checked;
            if (rbYM2608Scci.Checked)
            {
                if (cmbYM2608Scci.SelectedItem != null)
                {
                    string n = cmbYM2608Scci.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2608Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2608Type.BusID = int.Parse(ns[1]);
                    setting.YM2608Type.SoundChip = int.Parse(ns[2]);
                }
            }
            //setting.YM2608Type.UseWaitBoost = cbYM2608UseWaitBoost.Checked;
            setting.YM2608Type.OnlyPCMEmulation = cbOnlyPCMEmulation.Checked;
            setting.YM2608Type.LatencyForEmulation = 0;
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
            setting.YM2151Type.UseScci = rbYM2151Scci.Checked;
            if (rbYM2151Scci.Checked)
            {
                if (cmbYM2151Scci.SelectedItem != null)
                {
                    string n = cmbYM2151Scci.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    setting.YM2151Type.SoundLocation = int.Parse(ns[0]);
                    setting.YM2151Type.BusID = int.Parse(ns[1]);
                    setting.YM2151Type.SoundChip = int.Parse(ns[2]);
                }
            }
            //setting.YM2151Type.UseWaitBoost = cbYM2151UseWaitBoost.Checked;
            setting.YM2151Type.OnlyPCMEmulation = cbOnlyPCMEmulation.Checked;
            setting.YM2151Type.LatencyForEmulation = 0;
            //if (int.TryParse(tbYM2151EmuDelay.Text, out i))
            //{
            //    setting.YM2151Type.LatencyForEmulation = Math.Max(Math.Min(i, 999), 0);
            //}
            //setting.YM2151Type.LatencyForScci = 0;
            //if (int.TryParse(tbYM2151ScciDelay.Text, out i))
            //{
            //    setting.YM2151Type.LatencyForScci = Math.Max(Math.Min(i, 999), 0);
            //}

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

            setting.balance.YM2612Volume = trkYM2612.Value;
            setting.balance.SN76489Volume = trkSN76489.Value;
            setting.balance.RF5C164Volume = trkRF5C164.Value;
            setting.balance.PWMVolume = trkPWM.Value;
            setting.balance.C140Volume = trkC140.Value;
            setting.balance.OKIM6258Volume = trkOKIM6258.Value;
            setting.balance.OKIM6295Volume = trkOKIM6295.Value;
            setting.balance.SEGAPCMVolume = trkSEGAPCM.Value;
            setting.balance.YM2151Volume = trkYM2151.Value;
            setting.balance.YM2608Volume = trkYM2608.Value;
            setting.balance.YM2203Volume = trkYM2203.Value;
            setting.balance.YM2610Volume = trkYM2610.Value;

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

        private void cbYM2612UseWait_CheckedChanged(object sender, EventArgs e)
        {
            cbYM2612UseWaitBoost.Enabled = cbYM2612UseWait.Checked;
        }

        private void cbSN76489UseWait_CheckedChanged(object sender, EventArgs e)
        {
            //cbSN76489UseWaitBoost.Enabled = cbSN76489UseWait.Checked;
        }

        private void btnYM2612_Click(object sender, EventArgs e)
        {
            trkYM2612.Value = 100;
        }

        private void btnSN76489_Click(object sender, EventArgs e)
        {
            trkSN76489.Value = 100;
        }

        private void btnRF5C164_Click(object sender, EventArgs e)
        {
            trkRF5C164.Value = 100;
        }

        private void btnPWM_Click(object sender, EventArgs e)
        {
            trkPWM.Value = 100;
        }

        private void tbPWM_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbPWM.Text, out v))
            {
                tbPWM.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbPWM.Text = "100";
            }
        }

        private void tbRF5C164_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbRF5C164.Text, out v))
            {
                tbRF5C164.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbRF5C164.Text = "100";
            }
        }

        private void tbSN76489_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbSN76489.Text, out v))
            {
                tbSN76489.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbSN76489.Text = "100";
            }
        }

        private void tbYM2612_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbYM2612.Text, out v))
            {
                tbYM2612.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbYM2612.Text = "100";
            }
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

        private void btnC140_Click(object sender, EventArgs e)
        {
            trkC140.Value = 100;
        }

        private void tbC140_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbC140.Text, out v))
            {
                tbC140.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbC140.Text = "100";
            }

        }

        private void btnSEAPCM_Click(object sender, EventArgs e)
        {
            trkSEGAPCM.Value = 100;
        }

        private void tbSEGAPCM_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbSEGAPCM.Text, out v))
            {
                tbSEGAPCM.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbSEGAPCM.Text = "100";
            }

        }

        private void btnOKIM6258_Click(object sender, EventArgs e)
        {
            trkOKIM6258.Value = 100;
        }

        private void btnOKIM6295_Click(object sender, EventArgs e)
        {
            trkOKIM6295.Value = 100;
        }

        private void cbUseGetInst_CheckedChanged(object sender, EventArgs e)
        {
            lblInstFormat.Enabled = cbUseGetInst.Checked;
            cmbInstFormat.Enabled = cbUseGetInst.Checked;
        }

        private void tbYM2151_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbYM2151.Text, out v))
            {
                tbYM2151.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbYM2151.Text = "100";
            }

        }

        private void tbYM2608_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbYM2608.Text, out v))
            {
                tbYM2608.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbYM2608.Text = "100";
            }

        }

        private void tbYM2203_TextChanged(object sender, EventArgs e)
        {

            int v = 0;
            if (int.TryParse(tbYM2203.Text, out v))
            {
                tbYM2203.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbYM2203.Text = "100";
            }
        }

        private void tbYM2610_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(tbYM2610.Text, out v))
            {
                tbYM2610.Text = Math.Max(Math.Min(v, 200), 0).ToString();
            }
            else
            {
                tbYM2610.Text = "100";
            }

        }

        private void btnYM2151_Click(object sender, EventArgs e)
        {
            trkYM2151.Value = 100;

        }

        private void btnYM2608_Click(object sender, EventArgs e)
        {
            trkYM2608.Value = 100;

        }

        private void btnYM2203_Click(object sender, EventArgs e)
        {
            trkYM2203.Value = 100;

        }

        private void btnYM2610_Click(object sender, EventArgs e)
        {
            trkYM2610.Value = 100;

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
