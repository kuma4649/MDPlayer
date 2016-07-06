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
            bs1.DataSource = new Data();
            bs2.DataSource = new Data();
            bs3.DataSource = new Data();
            bs4.DataSource = new Data();

            Init();
        }

        public void Init() {

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

            foreach(DirectSoundDeviceInfo d in DirectSoundOut.Devices)
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

            List<NScci.NSoundChip> lstYM2612 = Audio.getYM2612ChipList();
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

            List<NScci.NSoundChip> lstSN76489 = Audio.getSN76489ChipList();
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
            cbSN76489UseWait.Checked = setting.SN76489Type.UseWait;
            cbSN76489UseWaitBoost.Checked = setting.SN76489Type.UseWaitBoost;
            tbSN76489EmuDelay.Text = setting.SN76489Type.LatencyForEmulation.ToString();
            tbSN76489ScciDelay.Text = setting.SN76489Type.LatencyForScci.ToString();

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

            ((Data)(bs1.DataSource)).Value = setting.balance.YM2612Volume;
            ((Data)(bs2.DataSource)).Value = setting.balance.SN76489Volume;
            ((Data)(bs3.DataSource)).Value = setting.balance.RF5C164Volume;
            ((Data)(bs4.DataSource)).Value = setting.balance.PWMVolume;

            cbDispFrameCounter.Checked = setting.Debug_DispFrameCounter;
            cbHiyorimiMode.Checked = setting.HiyorimiMode;

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

            setting.outputDevice.WaveOutDeviceName = cmbWaveOutDevice.SelectedItem.ToString();
            setting.outputDevice.DirectSoundDeviceName = cmbDirectSoundDevice.SelectedItem.ToString();
            setting.outputDevice.WasapiDeviceName = cmbWasapiDevice.SelectedItem.ToString();
            setting.outputDevice.AsioDeviceName = cmbAsioDevice.SelectedItem.ToString();

            setting.outputDevice.WasapiShareMode = rbShare.Checked;
            setting.outputDevice.Latency = int.Parse(cmbLatency.SelectedItem.ToString());

            setting.YM2612Type = new Setting.ChipType();
            setting.YM2612Type.UseScci = rbYM2612Scci.Checked;
            if (rbYM2612Scci.Checked)
            {
                string n = cmbYM2612Scci.SelectedItem.ToString();
                n = n.Substring(0, n.IndexOf(")")).Substring(1);
                string[] ns = n.Split(':');
                setting.YM2612Type.SoundLocation = int.Parse(ns[0]);
                setting.YM2612Type.BusID = int.Parse(ns[1]);
                setting.YM2612Type.SoundChip = int.Parse(ns[2]);
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
            setting.SN76489Type.UseWait = cbSN76489UseWait.Checked;
            setting.SN76489Type.UseWaitBoost = cbSN76489UseWaitBoost.Checked;
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

            setting.balance.YM2612Volume = trkYM2612.Value;
            setting.balance.SN76489Volume = trkSN76489.Value;
            setting.balance.RF5C164Volume = trkRF5C164.Value;
            setting.balance.PWMVolume = trkPWM.Value;

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
    }


    /// <summary>
    /// NumericUpDown とTrackBar にバインドされるデータ
    /// </summary>
    public class Data : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChanged から継承したイベントデリゲート
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="info"></param>
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
                    // このプロパティ名を渡してイベント通知
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
