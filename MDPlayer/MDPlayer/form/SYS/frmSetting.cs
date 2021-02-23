using MDPlayer.Properties;
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
            this.textBoxDescription.Text = Resources.cntDescription;

            this.cmbLatency.SelectedIndex = 5;
            this.cmbWaitTime.SelectedIndex = 0;

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

            if (ucSI != null)
            {
                SetRealCombo(EnmRealChipType.YM2612
                    , ucSI.cmbYM2612P_SCCI, ucSI.rbYM2612P_SCCI
                    , ucSI.cmbYM2612S_SCCI, ucSI.rbYM2612S_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM2413
                    , ucSI.cmbYM2413P_Real, ucSI.rbYM2413P_Real
                    , ucSI.cmbYM2413S_Real, ucSI.rbYM2413S_Real
                    );

                SetRealCombo(EnmRealChipType.AY8910
                    , ucSI.cmbAY8910P_Real, ucSI.rbAY8910P_Real
                    , ucSI.cmbAY8910S_Real, ucSI.rbAY8910S_Real
                    );

                SetRealCombo(EnmRealChipType.SN76489
                    , ucSI.cmbSN76489P_SCCI, ucSI.rbSN76489P_SCCI
                    , ucSI.cmbSN76489S_SCCI, ucSI.rbSN76489S_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM2608
                    , ucSI.cmbYM2608P_SCCI, ucSI.rbYM2608P_SCCI
                    , ucSI.cmbYM2608S_SCCI, ucSI.rbYM2608S_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM2610
                    , ucSI.cmbYM2610BP_SCCI, ucSI.rbYM2610BP_SCCI
                    , ucSI.cmbYM2610BS_SCCI, ucSI.rbYM2610BS_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM2608
                    , ucSI.cmbYM2610BEP_SCCI, ucSI.rbYM2610BEP_SCCI
                    , ucSI.cmbYM2610BES_SCCI, ucSI.rbYM2610BES_SCCI
                    );

                SetRealCombo(EnmRealChipType.SPPCM
                    , ucSI.cmbSPPCMP_SCCI, null
                    , ucSI.cmbSPPCMS_SCCI, null
                    );

                ucSI.rbYM2610BEP_SCCI.Enabled = (ucSI.cmbYM2610BEP_SCCI.Enabled || ucSI.cmbSPPCMP_SCCI.Enabled);
                ucSI.rbYM2610BES_SCCI.Enabled = (ucSI.cmbYM2610BES_SCCI.Enabled || ucSI.cmbSPPCMS_SCCI.Enabled);

                SetRealCombo(EnmRealChipType.YM2151
                    , ucSI.cmbYM2151P_SCCI, ucSI.rbYM2151P_SCCI
                    , ucSI.cmbYM2151S_SCCI, ucSI.rbYM2151S_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM2203
                    , ucSI.cmbYM2203P_SCCI, ucSI.rbYM2203P_SCCI
                    , ucSI.cmbYM2203S_SCCI, ucSI.rbYM2203S_SCCI
                    );

                SetRealCombo(EnmRealChipType.C140
                    , ucSI.cmbC140P_SCCI, ucSI.rbC140P_Real
                    , ucSI.cmbC140S_SCCI, ucSI.rbC140S_SCCI
                    );

                SetRealCombo(EnmRealChipType.SEGAPCM
                    , ucSI.cmbSEGAPCMP_SCCI, ucSI.rbSEGAPCMP_SCCI
                    , ucSI.cmbSEGAPCMS_SCCI, ucSI.rbSEGAPCMS_SCCI
                    );

                SetRealCombo(EnmRealChipType.YMF262
                    , ucSI.cmbYMF262P_SCCI, ucSI.rbYMF262P_SCCI
                    , ucSI.cmbYMF262S_SCCI, ucSI.rbYMF262S_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM3526
                    , ucSI.cmbYM3526P_SCCI, ucSI.rbYM3526P_SCCI
                    , ucSI.cmbYM3526S_SCCI, ucSI.rbYM3526S_SCCI
                    );

                SetRealCombo(EnmRealChipType.YM3812
                    , ucSI.cmbYM3812P_SCCI, ucSI.rbYM3812P_SCCI
                    , ucSI.cmbYM3812S_SCCI, ucSI.rbYM3812S_SCCI
                    );
            }

            copyFromMIDIoutListA(dgvMIDIoutListB);
            copyFromMIDIoutListA(dgvMIDIoutListC);
            copyFromMIDIoutListA(dgvMIDIoutListD);
            copyFromMIDIoutListA(dgvMIDIoutListE);
            copyFromMIDIoutListA(dgvMIDIoutListF);
            copyFromMIDIoutListA(dgvMIDIoutListG);
            copyFromMIDIoutListA(dgvMIDIoutListH);
            copyFromMIDIoutListA(dgvMIDIoutListI);
            copyFromMIDIoutListA(dgvMIDIoutListJ);

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
                case 4:
                    //SSPCM
                    rbWaveOut.Checked = true;
                    break;
                case 5:
                    rbNullDevice.Checked = true;
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

            if (cmbWaitTime.Items.Contains(setting.outputDevice.WaitTime.ToString()))
            {
                cmbWaitTime.SelectedItem = setting.outputDevice.WaitTime.ToString();
            }

            if (ucSI != null)
            {
                SetRealParam(setting.YM2612Type[0]
                    , ucSI.rbYM2612P_Silent
                    , ucSI.rbYM2612P_Emu
                    , ucSI.rbYM2612P_SCCI
                    , ucSI.cmbYM2612P_SCCI
                    , null, null,null
                    , ucSI.rbYM2612P_EmuNuked
                    , ucSI.rbYM2612P_EmuMame);
                SetRealParam(setting.YM2612Type[1]
                    , ucSI.rbYM2612S_Silent
                    , ucSI.rbYM2612S_Emu
                    , ucSI.rbYM2612S_SCCI
                    , ucSI.cmbYM2612S_SCCI
                    , null, null,null
                    , ucSI.rbYM2612S_EmuNuked
                    , ucSI.rbYM2612S_EmuMame);

                ucSI.cbSendWait.Checked = setting.YM2612Type[0].realChipInfo[0].UseWait;
                ucSI.cbTwice.Checked = setting.YM2612Type[0].realChipInfo[0].UseWaitBoost;
                ucSI.cbEmulationPCMOnly.Checked = setting.YM2612Type[0].realChipInfo[0].OnlyPCMEmulation;

                SetRealParam(setting.YM2610Type[0]
                    , ucSI.rbYM2610BP_Silent
                    , ucSI.rbYM2610BP_Emu
                    , ucSI.rbYM2610BP_SCCI
                    , ucSI.cmbYM2610BP_SCCI
                    , ucSI.rbYM2610BEP_SCCI
                    , ucSI.cmbYM2610BEP_SCCI
                    , ucSI.cmbSPPCMP_SCCI
                    );
                SetRealParam(setting.YM2610Type[1]
                    , ucSI.rbYM2610BS_Silent
                    , ucSI.rbYM2610BS_Emu
                    , ucSI.rbYM2610BS_SCCI
                    , ucSI.cmbYM2610BS_SCCI
                    , ucSI.rbYM2610BES_SCCI
                    , ucSI.cmbYM2610BES_SCCI
                    , ucSI.cmbSPPCMS_SCCI
                    );

                SetRealParam(setting.SN76489Type[0]
                    , ucSI.rbSN76489P_Silent
                    , ucSI.rbSN76489P_Emu
                    , ucSI.rbSN76489P_SCCI
                    , ucSI.cmbSN76489P_SCCI
                    , null,null,null
                    , ucSI.rbSN76489P_Emu2);
                SetRealParam(setting.SN76489Type[1]
                    , ucSI.rbSN76489S_Silent
                    , ucSI.rbSN76489S_Emu
                    , ucSI.rbSN76489S_SCCI
                    , ucSI.cmbSN76489S_SCCI
                    , null,null,null
                    , ucSI.rbSN76489S_Emu2);

                SetRealParam(setting.YM2608Type[0]
                    , ucSI.rbYM2608P_Silent
                    , ucSI.rbYM2608P_Emu
                    , ucSI.rbYM2608P_SCCI
                    , ucSI.cmbYM2608P_SCCI);
                SetRealParam(setting.YM2608Type[1]
                    , ucSI.rbYM2608S_Silent
                    , ucSI.rbYM2608S_Emu
                    , ucSI.rbYM2608S_SCCI
                    , ucSI.cmbYM2608S_SCCI);

                SetRealParam(setting.YM2151Type[0]
                    , ucSI.rbYM2151P_Silent
                    , ucSI.rbYM2151P_Emu
                    , ucSI.rbYM2151P_SCCI
                    , ucSI.cmbYM2151P_SCCI
                    , null, null, null
                    , ucSI.rbYM2151P_EmuMame
                    , ucSI.rbYM2151P_EmuX68Sound);
                SetRealParam(setting.YM2151Type[1]
                    , ucSI.rbYM2151S_Silent
                    , ucSI.rbYM2151S_Emu
                    , ucSI.rbYM2151S_SCCI
                    , ucSI.cmbYM2151S_SCCI
                    , null, null, null
                    , ucSI.rbYM2151S_EmuMame
                    , ucSI.rbYM2151S_EmuX68Sound);

                SetRealParam(setting.YM2203Type[0]
                    , ucSI.rbYM2203P_Silent
                    , ucSI.rbYM2203P_Emu
                    , ucSI.rbYM2203P_SCCI
                    , ucSI.cmbYM2203P_SCCI);
                SetRealParam(setting.YM2203Type[1]
                    , ucSI.rbYM2203S_Silent
                    , ucSI.rbYM2203S_Emu
                    , ucSI.rbYM2203S_SCCI
                    , ucSI.cmbYM2203S_SCCI);

                SetRealParam(setting.AY8910Type[0]
                    , ucSI.rbAY8910P_Silent
                    , ucSI.rbAY8910P_Emu
                    , ucSI.rbAY8910P_Real
                    , ucSI.cmbAY8910P_Real);
                SetRealParam(setting.AY8910Type[1]
                    , ucSI.rbAY8910S_Silent
                    , ucSI.rbAY8910S_Emu
                    , ucSI.rbAY8910S_Real
                    , ucSI.cmbAY8910S_Real);

                SetRealParam(setting.YM2413Type[0]
                    , ucSI.rbYM2413P_Silent
                    , ucSI.rbYM2413P_Emu
                    , ucSI.rbYM2413P_Real
                    , ucSI.cmbYM2413P_Real);
                SetRealParam(setting.YM2413Type[1]
                    , ucSI.rbYM2413S_Silent
                    , ucSI.rbYM2413S_Emu
                    , ucSI.rbYM2413S_Real
                    , ucSI.cmbYM2413S_Real);

                SetRealParam(setting.YM3526Type[0]
                    , ucSI.rbYM3526P_Silent
                    , ucSI.rbYM3526P_Emu
                    , ucSI.rbYM3526P_SCCI
                    , ucSI.cmbYM3526P_SCCI);
                SetRealParam(setting.YM3526Type[1]
                    , ucSI.rbYM3526S_Silent
                    , ucSI.rbYM3526S_Emu
                    , ucSI.rbYM3526S_SCCI
                    , ucSI.cmbYM3526S_SCCI);

                SetRealParam(setting.YM3812Type[0]
                    , ucSI.rbYM3812P_Silent
                    , ucSI.rbYM3812P_Emu
                    , ucSI.rbYM3812P_SCCI
                    , ucSI.cmbYM3812P_SCCI);
                SetRealParam(setting.YM3812Type[1]
                    , ucSI.rbYM3812S_Silent
                    , ucSI.rbYM3812S_Emu
                    , ucSI.rbYM3812S_SCCI
                    , ucSI.cmbYM3812S_SCCI);

                SetRealParam(setting.YMF262Type[0]
                    , ucSI.rbYMF262P_Silent
                    , ucSI.rbYMF262P_Emu
                    , ucSI.rbYMF262P_SCCI
                    , ucSI.cmbYMF262P_SCCI);
                SetRealParam(setting.YMF262Type[1]
                    , ucSI.rbYMF262S_Silent
                    , ucSI.rbYMF262S_Emu
                    , ucSI.rbYMF262S_SCCI
                    , ucSI.cmbYMF262S_SCCI);

                SetRealParam(setting.C140Type[0]
                    , ucSI.rbC140P_Silent
                    , ucSI.rbC140P_Emu
                    , ucSI.rbC140P_Real
                    , ucSI.cmbC140P_SCCI);
                SetRealParam(setting.C140Type[1]
                    , ucSI.rbC140S_Silent
                    , ucSI.rbC140S_Emu
                    , ucSI.rbC140S_SCCI
                    , ucSI.cmbC140S_SCCI);

                SetRealParam(setting.SEGAPCMType[0]
                    , ucSI.rbSEGAPCMP_Silent
                    , ucSI.rbSEGAPCMP_Emu
                    , ucSI.rbSEGAPCMP_SCCI
                    , ucSI.cmbSEGAPCMP_SCCI);
                SetRealParam(setting.SEGAPCMType[1]
                    , ucSI.rbSEGAPCMS_Silent
                    , ucSI.rbSEGAPCMS_Emu
                    , ucSI.rbSEGAPCMS_SCCI
                    , ucSI.cmbSEGAPCMS_SCCI);
            }

            for(int i = 0; i < cmbSampleRate.Items.Count; i++)
            {
                if (cmbSampleRate.Items[i].ToString() == setting.outputDevice.SampleRate.ToString())
                {
                    cmbSampleRate.SelectedIndex = i;
                    break;
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
            tbTextExt.Text = setting.other.TextExt;
            tbMMLExt.Text = setting.other.MMLExt;
            cbAutoOpenText.Checked = setting.other.AutoOpenText;
            cbAutoOpenMML.Checked = setting.other.AutoOpenMML;
            cbAutoOpenImg.Checked = setting.other.AutoOpenImg;
            tbImageExt.Text = setting.other.ImageExt;
            cbInitAlways.Checked = setting.other.InitAlways;
            cbEmptyPlayList.Checked = setting.other.EmptyPlayList;

            cbUseMIDIExport.Checked = setting.midiExport.UseMIDIExport;
            gbMIDIExport.Enabled = cbUseMIDIExport.Checked;
            tbMIDIOutputPath.Text = setting.midiExport.ExportPath;
            cbMIDIUseVOPM.Checked = setting.midiExport.UseVOPMex;
            cbMIDIKeyOnFnum.Checked = setting.midiExport.KeyOnFnum;
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
                            switch (moi.type)
                            {
                                case 1:
                                    stype = "XG";
                                    break;
                                case 2:
                                    stype = "GS";
                                    break;
                                case 3:
                                    stype = "LA";
                                    break;
                                case 4:
                                    stype = "GS(SC-55_1)";
                                    break;
                                case 5:
                                    stype = "GS(SC-55_2)";
                                    break;
                            }

                            string sbeforeSend = "None";
                            switch (moi.beforeSendType)
                            {
                                case 1:
                                    sbeforeSend = "GM Reset";
                                    break;
                                case 2:
                                    sbeforeSend = "XG Reset";
                                    break;
                                case 3:
                                    sbeforeSend = "GS Reset";
                                    break;
                                case 4:
                                    sbeforeSend = "Custom";
                                    break;
                            }

                            dgv[i].Rows.Add(
                                moi.id
                                , moi.isVST
                                , moi.fileName
                                , moi.name
                                , stype
                                , sbeforeSend
                                , moi.isVST ? moi.vendor : (moi.manufacturer != -1 ? ((NAudio.Manufacturers)moi.manufacturer).ToString() : "Unknown")
                                );

                        }
                    }
                }
            }

            tbBeforeSend_GMReset.Text = setting.midiOut.GMReset;
            tbBeforeSend_XGReset.Text = setting.midiOut.XGReset;
            tbBeforeSend_GSReset.Text = setting.midiOut.GSReset;
            tbBeforeSend_Custom.Text = setting.midiOut.Custom;

            dgvMIDIoutPallet.Rows.Clear();
            for (int i = 0; i < NAudio.Midi.MidiOut.NumberOfDevices; i++)
            {
                //if (!midioutNotFound.Contains(i))
                //{
                NAudio.Midi.MidiOutCapabilities moc = NAudio.Midi.MidiOut.DeviceInfo(i);
                dgvMIDIoutPallet.Rows.Add(i, moc.ProductName, moc.Manufacturer.ToString() != "-1" ? moc.Manufacturer.ToString() : "Unknown");
                //}
            }

            trkbNSFHPF.Value = setting.nsf.HPF;
            trkbNSFLPF.Value = setting.nsf.LPF;
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
            switch (setting.sid.Quality)
            {
                case 0:
                    rdSIDQ1.Checked = true;
                    break;
                case 1:
                    rdSIDQ2.Checked = true;
                    break;
                case 2:
                    rdSIDQ3.Checked = true;
                    break;
                case 3:
                    rdSIDQ4.Checked = true;
                    break;
            }
            tbSIDOutputBufferSize.Text = setting.sid.OutputBufferSize.ToString();

            rbSIDC64Model_PAL.Checked = (setting.sid.c64model == 0);
            rbSIDC64Model_NTSC.Checked = (setting.sid.c64model == 1);
            rbSIDC64Model_OLDNTSC.Checked = (setting.sid.c64model == 2);
            rbSIDC64Model_DREAN.Checked = (setting.sid.c64model == 3);
            cbSIDC64Model_Force.Checked = setting.sid.c64modelForce;

            rbSIDModel_6581.Checked = (setting.sid.sidmodel == 0);
            rbSIDModel_8580.Checked = (setting.sid.sidmodel == 1);
            cbSIDModel_Force.Checked = setting.sid.sidmodelForce;


            switch (setting.nukedOPN2.EmuType)
            {
                case 0:
                    rbNukedOPN2OptionDiscrete.Checked = true;
                    break;
                case 1:
                    rbNukedOPN2OptionASIC.Checked = true;
                    break;
                case 2:
                    rbNukedOPN2OptionYM2612.Checked = true;
                    break;
                case 3:
                    rbNukedOPN2OptionYM2612u.Checked = true;
                    break;
                case 4:
                    rbNukedOPN2OptionASIClp.Checked = true;
                    break;
            }

            cbGensDACHPF.Checked = setting.nukedOPN2.GensDACHPF;
            cbGensSSGEG.Checked = setting.nukedOPN2.GensSSGEG;

            cbAutoBalanceUseThis.Checked = setting.autoBalance.UseThis;
            rbAutoBalanceLoadSongBalance.Checked = setting.autoBalance.LoadSongBalance;
            rbAutoBalanceNotLoadSongBalance.Checked = !setting.autoBalance.LoadSongBalance;
            rbAutoBalanceLoadDriverBalance.Checked = setting.autoBalance.LoadDriverBalance;
            rbAutoBalanceNotLoadDriverBalance.Checked = !setting.autoBalance.LoadDriverBalance;
            rbAutoBalanceSaveSongBalance.Checked = setting.autoBalance.SaveSongBalance;
            rbAutoBalanceNotSaveSongBalance.Checked = !setting.autoBalance.SaveSongBalance;
            rbAutoBalanceSamePositionAsSongData.Checked = setting.autoBalance.SamePositionAsSongData;
            rbAutoBalanceNotSamePositionAsSongData.Checked = !setting.autoBalance.SamePositionAsSongData;

            cbUseKeyBoardHook.Checked = setting.keyBoardHook.UseKeyBoardHook;
            gbUseKeyBoardHook.Enabled = setting.keyBoardHook.UseKeyBoardHook;

            cbStopShift.Checked = setting.keyBoardHook.Stop.Shift;
            cbStopCtrl.Checked = setting.keyBoardHook.Stop.Ctrl;
            cbStopWin.Checked = setting.keyBoardHook.Stop.Win;
            cbStopAlt.Checked = setting.keyBoardHook.Stop.Alt;
            lblStopKey.Text = setting.keyBoardHook.Stop.Key;
            btStopClr.Enabled = (lblStopKey.Text != "(None)" && !string.IsNullOrEmpty(lblStopKey.Text));

            cbPauseShift.Checked = setting.keyBoardHook.Pause.Shift;
            cbPauseCtrl.Checked = setting.keyBoardHook.Pause.Ctrl;
            cbPauseWin.Checked = setting.keyBoardHook.Pause.Win;
            cbPauseAlt.Checked = setting.keyBoardHook.Pause.Alt;
            lblPauseKey.Text = setting.keyBoardHook.Pause.Key;
            btPauseClr.Enabled = (lblPauseKey.Text != "(None)" && !string.IsNullOrEmpty(lblPauseKey.Text));

            cbFadeoutShift.Checked = setting.keyBoardHook.Fadeout.Shift;
            cbFadeoutCtrl.Checked = setting.keyBoardHook.Fadeout.Ctrl;
            cbFadeoutWin.Checked = setting.keyBoardHook.Fadeout.Win;
            cbFadeoutAlt.Checked = setting.keyBoardHook.Fadeout.Alt;
            lblFadeoutKey.Text = setting.keyBoardHook.Fadeout.Key;
            btFadeoutClr.Enabled = (lblFadeoutKey.Text != "(None)" && !string.IsNullOrEmpty(lblFadeoutKey.Text));

            cbPrevShift.Checked = setting.keyBoardHook.Prev.Shift;
            cbPrevCtrl.Checked = setting.keyBoardHook.Prev.Ctrl;
            cbPrevWin.Checked = setting.keyBoardHook.Prev.Win;
            cbPrevAlt.Checked = setting.keyBoardHook.Prev.Alt;
            lblPrevKey.Text = setting.keyBoardHook.Prev.Key;
            btPrevClr.Enabled = (lblPrevKey.Text != "(None)" && !string.IsNullOrEmpty(lblPrevKey.Text));

            cbSlowShift.Checked = setting.keyBoardHook.Slow.Shift;
            cbSlowCtrl.Checked = setting.keyBoardHook.Slow.Ctrl;
            cbSlowWin.Checked = setting.keyBoardHook.Slow.Win;
            cbSlowAlt.Checked = setting.keyBoardHook.Slow.Alt;
            lblSlowKey.Text = setting.keyBoardHook.Slow.Key;
            btSlowClr.Enabled = (lblSlowKey.Text != "(None)" && !string.IsNullOrEmpty(lblSlowKey.Text));

            cbPlayShift.Checked = setting.keyBoardHook.Play.Shift;
            cbPlayCtrl.Checked = setting.keyBoardHook.Play.Ctrl;
            cbPlayWin.Checked = setting.keyBoardHook.Play.Win;
            cbPlayAlt.Checked = setting.keyBoardHook.Play.Alt;
            lblPlayKey.Text = setting.keyBoardHook.Play.Key;
            btPlayClr.Enabled = (lblPlayKey.Text != "(None)" && !string.IsNullOrEmpty(lblPlayKey.Text));

            cbFastShift.Checked = setting.keyBoardHook.Fast.Shift;
            cbFastCtrl.Checked = setting.keyBoardHook.Fast.Ctrl;
            cbFastWin.Checked = setting.keyBoardHook.Fast.Win;
            cbFastAlt.Checked = setting.keyBoardHook.Fast.Alt;
            lblFastKey.Text = setting.keyBoardHook.Fast.Key;
            btFastClr.Enabled = (lblFastKey.Text != "(None)" && !string.IsNullOrEmpty(lblFastKey.Text));

            cbNextShift.Checked = setting.keyBoardHook.Next.Shift;
            cbNextCtrl.Checked = setting.keyBoardHook.Next.Ctrl;
            cbNextWin.Checked = setting.keyBoardHook.Next.Win;
            cbNextAlt.Checked = setting.keyBoardHook.Next.Alt;
            lblNextKey.Text = setting.keyBoardHook.Next.Key;
            btNextClr.Enabled = (lblNextKey.Text != "(None)" && !string.IsNullOrEmpty(lblNextKey.Text));

            cbExALL.Checked = setting.other.ExAll;
            cbNonRenderingForPause.Checked = setting.other.NonRenderingForPause;



            tbPMDCompilerArguments.Text = setting.pmdDotNET.compilerArguments;
            rbPMDAuto.Checked = setting.pmdDotNET.isAuto;
            rbPMDManual.Checked = !setting.pmdDotNET.isAuto;
            rbPMDNrmB.Checked = setting.pmdDotNET.soundBoard == 0;
            rbPMDSpbB.Checked = setting.pmdDotNET.soundBoard == 1;
            rbPMD86B.Checked = setting.pmdDotNET.soundBoard == 2;
            cbPMDSetManualVolume.Checked = setting.pmdDotNET.setManualVolume;
            cbPMDUsePPSDRV.Checked = setting.pmdDotNET.usePPSDRV;
            cbPMDUsePPZ8.Checked = setting.pmdDotNET.usePPZ8;
            tbPMDDriverArguments.Text = setting.pmdDotNET.driverArguments;
            rbPMDUsePPSDRVFreqDefault.Checked = setting.pmdDotNET.usePPSDRVUseInterfaceDefaultFreq;
            rbPMDUsePPSDRVManualFreq.Checked = !setting.pmdDotNET.usePPSDRVUseInterfaceDefaultFreq;
            tbPMDPPSDRVFreq.Text = setting.pmdDotNET.PPSDRVManualFreq.ToString();
            tbPMDPPSDRVManualWait.Text = setting.pmdDotNET.PPSDRVManualWait.ToString();
            tbPMDVolumeFM.Text = setting.pmdDotNET.volumeFM.ToString();
            tbPMDVolumeSSG.Text = setting.pmdDotNET.volumeSSG.ToString();
            tbPMDVolumeRhythm.Text = setting.pmdDotNET.volumeRhythm.ToString();
            tbPMDVolumeAdpcm.Text = setting.pmdDotNET.volumeAdpcm.ToString();
            tbPMDVolumeGIMICSSG.Text = setting.pmdDotNET.volumeGIMICSSG.ToString();

            rbPMDManual_CheckedChanged(null, null);
            cbPMDSetManualVolume_CheckedChanged(null, null);
            cbPMDUsePPSDRV_CheckedChanged(null, null);
            rbPMDUsePPSDRVManualFreq_CheckedChanged(null, null);
        }

        private void SetRealCombo(EnmRealChipType realType, ComboBox cmbP, RadioButton rbP, ComboBox cmbS, RadioButton rbS)
        {

            if(rbP!=null) rbP.Enabled = false;
            cmbP.Enabled = false;

            if (rbS != null) rbS.Enabled = false;
            cmbS.Enabled = false;

            List<Setting.ChipType2> lstChip = Audio.GetRealChipList(realType);
            if (lstChip == null || lstChip.Count < 1) return;

            foreach (Setting.ChipType2 ct in lstChip)
            {
                if (ct == null) continue;

                cmbP.Items.Add(string.Format("({0}:{1}:{2}:{3}){4}"
                    , ct.realChipInfo[0].InterfaceName
                    , ct.realChipInfo[0].SoundLocation
                    , ct.realChipInfo[0].BusID
                    , ct.realChipInfo[0].SoundChip
                    , ct.realChipInfo[0].ChipName));

                cmbS.Items.Add(string.Format("({0}:{1}:{2}:{3}){4}"
                    , ct.realChipInfo[0].InterfaceName
                    , ct.realChipInfo[0].SoundLocation
                    , ct.realChipInfo[0].BusID
                    , ct.realChipInfo[0].SoundChip
                    , ct.realChipInfo[0].ChipName));
            }

            cmbP.SelectedIndex = 0;
            if (rbP != null) rbP.Enabled = true;
            cmbP.Enabled = true;

            cmbS.SelectedIndex = 0;
            if (rbS != null) rbS.Enabled = true;
            cmbS.Enabled = true;

        }

        private void SetRealParam(Setting.ChipType2 ChipType2, RadioButton rbSilent, RadioButton rbEmu, RadioButton rbReal, ComboBox cmbP
            , RadioButton rbReal2 = null, ComboBox cmbP2A = null, ComboBox cmbP2B = null, RadioButton rbEmu2 = null, RadioButton rbEmu3 = null)
        {
            string n = "";

            if (ChipType2.realChipInfo[0] != null)
            {
                n = string.Format("({0}:{1}:{2}:{3})"
                    , ChipType2.realChipInfo[0].InterfaceName
                    , ChipType2.realChipInfo[0].SoundLocation
                    , ChipType2.realChipInfo[0].BusID
                    , ChipType2.realChipInfo[0].SoundChip);
            }

            if (cmbP.Items.Count > 0)
            {
                foreach (string i in cmbP.Items)
                {
                    if (i.IndexOf(n) < 0) continue;
                    cmbP.SelectedItem = i;

                    break;
                }
            }

            if (cmbP2A != null)
            {
                if (ChipType2.realChipInfo[1] != null)
                {
                    n = string.Format("({0}:{1}:{2}:{3})"
                    , ChipType2.realChipInfo[1].InterfaceName
                    , ChipType2.realChipInfo[1].SoundLocation
                    , ChipType2.realChipInfo[1].BusID
                    , ChipType2.realChipInfo[1].SoundChip);
                }

                if (cmbP2A.Items.Count > 0)
                {
                    foreach (string i in cmbP2A.Items)
                    {
                        if (i.IndexOf(n) < 0) continue;
                        cmbP2A.SelectedItem = i;

                        break;
                    }
                }
            }

            if (cmbP2B != null)
            {
                if (ChipType2.realChipInfo[2] != null)
                {
                    n = string.Format("({0}:{1}:{2}:{3})"
                    , ChipType2.realChipInfo[2].InterfaceName
                    , ChipType2.realChipInfo[2].SoundLocation
                    , ChipType2.realChipInfo[2].BusID
                    , ChipType2.realChipInfo[2].SoundChip);
                }

                if (cmbP2B.Items.Count > 0)
                {
                    foreach (string i in cmbP2B.Items)
                    {
                        if (i.IndexOf(n) < 0) continue;
                        cmbP2B.SelectedItem = i;

                        break;
                    }
                }
            }

            if (ChipType2.UseEmu.Length > 1 && ChipType2.UseEmu[1])
            {
                rbEmu2.Checked = true;
                return;
            }

            if (ChipType2.UseEmu.Length > 2 && ChipType2.UseEmu[2])
            {
                rbEmu3.Checked = true;
                return;
            }

            if ((ChipType2.UseReal.Length > 0 && !ChipType2.UseReal[0])
                && (ChipType2.UseReal.Length > 1 && !ChipType2.UseReal[1]))// rbSCCI2==null) || (!ChipType2.UseScci && rbSCCI2 != null && !ChipType2.UseScci2))
            {
                if (ChipType2.UseEmu[0])
                    rbEmu.Checked = true;
                else
                    rbSilent.Checked = true;

                return;
            }

            if (
                ((ChipType2.UseReal.Length > 0 && ChipType2.UseReal[0]) && !cmbP.Enabled)
                || (
                    (ChipType2.UseReal.Length > 1 && ChipType2.UseReal[1])
                    && !cmbP2A.Enabled
                    && !cmbP2B.Enabled)
                )
            {
                rbEmu.Checked = true;

                return;
            }

            if (ChipType2.UseReal.Length > 0 && ChipType2.UseReal[0])
            {
                rbReal.Checked = true;
                return;
            }

            if (rbReal2 != null) rbReal2.Checked = true;

        }

        private void copyFromMIDIoutListA(DataGridView dgv)
        {

            dgv.Columns.Clear();

            foreach(DataGridViewColumn col in dgvMIDIoutListA.Columns)
            {
                dgv.Columns.Add((DataGridViewColumn)col.Clone());
            }

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
            if (!CheckSetting()) return;

            int i = 0;


            #region 出力

            setting.outputDevice.DeviceType = Common.DEV_WaveOut;
            if (rbWaveOut.Checked) setting.outputDevice.DeviceType = Common.DEV_WaveOut;
            if (rbDirectSoundOut.Checked) setting.outputDevice.DeviceType = Common.DEV_DirectSound;
            if (rbWasapiOut.Checked) setting.outputDevice.DeviceType = Common.DEV_WasapiOut;
            if (rbAsioOut.Checked) setting.outputDevice.DeviceType = Common.DEV_AsioOut;
            if (rbSPPCM.Checked) setting.outputDevice.DeviceType = Common.DEV_SPPCM;
            if (rbNullDevice.Checked) setting.outputDevice.DeviceType = Common.DEV_Null;

            setting.outputDevice.WaveOutDeviceName = cmbWaveOutDevice.SelectedItem != null ? cmbWaveOutDevice.SelectedItem.ToString() : "";
            setting.outputDevice.DirectSoundDeviceName = cmbDirectSoundDevice.SelectedItem != null ? cmbDirectSoundDevice.SelectedItem.ToString() : "";
            setting.outputDevice.WasapiDeviceName = cmbWasapiDevice.SelectedItem != null ? cmbWasapiDevice.SelectedItem.ToString() : "";
            setting.outputDevice.AsioDeviceName = cmbAsioDevice.SelectedItem != null ? cmbAsioDevice.SelectedItem.ToString() : "";

            setting.outputDevice.WasapiShareMode = rbShare.Checked;
            setting.outputDevice.Latency = int.Parse(cmbLatency.SelectedItem.ToString());
            setting.outputDevice.WaitTime = int.Parse(cmbWaitTime.SelectedItem.ToString());
            setting.outputDevice.SampleRate = int.Parse(cmbSampleRate.SelectedItem.ToString());

            #endregion


            #region Sound

            setting.YM2612Type = new Setting.ChipType2[2];
            setting.YM2612Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2612Type[0]
                , ucSI.rbYM2612P_SCCI
                , ucSI.cmbYM2612P_SCCI
                , ucSI.rbYM2612P_Emu
                , ucSI.rbYM2612P_EmuNuked
                , ucSI.rbYM2612P_EmuMame
                , null
                , null
                , null
                );

            setting.YM2612Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2612Type[1]
                , ucSI.rbYM2612S_SCCI
                , ucSI.cmbYM2612S_SCCI
                , ucSI.rbYM2612S_Emu
                , ucSI.rbYM2612S_EmuNuked
                , ucSI.rbYM2612S_EmuMame
                , null
                , null
                , null
                );
            setting.YM2612Type[0].realChipInfo = new Setting.ChipType2.RealChipInfo[] { new Setting.ChipType2.RealChipInfo() };
            setting.YM2612Type[0].realChipInfo[0].UseWait = ucSI.cbSendWait.Checked;
            setting.YM2612Type[0].realChipInfo[0].UseWaitBoost = ucSI.cbTwice.Checked;
            setting.YM2612Type[0].realChipInfo[0].OnlyPCMEmulation = ucSI.cbEmulationPCMOnly.Checked;

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


            setting.SN76489Type = new Setting.ChipType2[2];
            setting.SN76489Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.SN76489Type[0]
                , ucSI.rbSN76489P_SCCI
                , ucSI.cmbSN76489P_SCCI
                , ucSI.rbSN76489P_Emu
                , ucSI.rbSN76489P_Emu2
                , null
                , null
                , null
                , null
                );

            setting.SN76489Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.SN76489Type[1]
                , ucSI.rbSN76489S_SCCI
                , ucSI.cmbSN76489S_SCCI
                , ucSI.rbSN76489S_Emu
                , ucSI.rbSN76489S_Emu2
                , null
                , null
                , null
                , null
                );

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


            setting.YM2608Type = new Setting.ChipType2[2];
            setting.YM2608Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2608Type[0]
                , ucSI.rbYM2608P_SCCI
                , ucSI.cmbYM2608P_SCCI
                , ucSI.rbYM2608P_Emu
                , null
                , null
                , null
                , null
                , null
                );

            setting.YM2608Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2608Type[1]
                , ucSI.rbYM2608S_SCCI
                , ucSI.cmbYM2608S_SCCI
                , ucSI.rbYM2608S_Emu
                , null
                , null
                , null
                , null
                , null
                );

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


            setting.YM2610Type = new Setting.ChipType2[2];
            setting.YM2610Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2610Type[0]
                , ucSI.rbYM2610BP_SCCI
                , ucSI.cmbYM2610BP_SCCI
                , ucSI.rbYM2610BP_Emu
                , null
                , null
                , ucSI.rbYM2610BEP_SCCI
                , ucSI.cmbYM2610BEP_SCCI
                , ucSI.cmbSPPCMP_SCCI
                );

            setting.YM2610Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2610Type[1]
                , ucSI.rbYM2610BS_SCCI
                , ucSI.cmbYM2610BS_SCCI
                , ucSI.rbYM2610BS_Emu
                , null
                , null
                , ucSI.rbYM2610BES_SCCI
                , ucSI.cmbYM2610BES_SCCI
                , ucSI.cmbSPPCMS_SCCI
                );


            setting.YM2151Type = new Setting.ChipType2[2];
            setting.YM2151Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2151Type[0]
                , ucSI.rbYM2151P_SCCI
                , ucSI.cmbYM2151P_SCCI
                , ucSI.rbYM2151P_Emu
                , ucSI.rbYM2151P_EmuMame
                , ucSI.rbYM2151P_EmuX68Sound
                , null
                , null
                , null
                );

            setting.YM2151Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2151Type[1]
                , ucSI.rbYM2151S_SCCI
                , ucSI.cmbYM2151S_SCCI
                , ucSI.rbYM2151S_Emu
                , ucSI.rbYM2151S_EmuMame
                , ucSI.rbYM2151S_EmuX68Sound
                , null
                , null
                , null
                );


            setting.YM2203Type = new Setting.ChipType2[2];
            setting.YM2203Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2203Type[0]
                , ucSI.rbYM2203P_SCCI
                , ucSI.cmbYM2203P_SCCI
                , ucSI.rbYM2203P_Emu
                , null, null, null, null, null
                );

            setting.YM2203Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2203Type[1]
                , ucSI.rbYM2203S_SCCI
                , ucSI.cmbYM2203S_SCCI
                , ucSI.rbYM2203S_Emu
                , null, null, null, null, null
                );


            setting.AY8910Type = new Setting.ChipType2[2];
            setting.AY8910Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.AY8910Type[0]
                , ucSI.rbAY8910P_Real
                , ucSI.cmbAY8910P_Real
                , ucSI.rbAY8910P_Emu
                , null, null, null, null, null
                );

            setting.AY8910Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.AY8910Type[1]
                , ucSI.rbAY8910S_Real
                , ucSI.cmbAY8910S_Real
                , ucSI.rbAY8910S_Emu
                , null, null, null, null, null
                );


            setting.YM2413Type = new Setting.ChipType2[2];
            setting.YM2413Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2413Type[0]
                , ucSI.rbYM2413P_Real
                , ucSI.cmbYM2413P_Real
                , ucSI.rbYM2413P_Emu
                , null, null, null, null, null
                );

            setting.YM2413Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM2413Type[1]
                , ucSI.rbYM2413S_Real
                , ucSI.cmbYM2413S_Real
                , ucSI.rbYM2413S_Emu
                , null, null, null, null, null
                );


            setting.C140Type = new Setting.ChipType2[2];
            setting.C140Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.C140Type[0]
                , ucSI.rbC140P_Real
                , ucSI.cmbC140P_SCCI
                , ucSI.rbC140P_Emu
                , null, null, null, null, null
                );

            setting.C140Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.C140Type[1]
                , ucSI.rbC140S_SCCI
                , ucSI.cmbC140S_SCCI
                , ucSI.rbC140S_Emu
                , null, null, null, null, null
                );


            setting.SEGAPCMType = new Setting.ChipType2[2];
            setting.SEGAPCMType[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.SEGAPCMType[0]
                , ucSI.rbSEGAPCMP_SCCI
                , ucSI.cmbSEGAPCMP_SCCI
                , ucSI.rbSEGAPCMP_Emu
                , null, null, null, null, null
                );

            setting.SEGAPCMType[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.SEGAPCMType[1]
                , ucSI.rbSEGAPCMS_SCCI
                , ucSI.cmbSEGAPCMS_SCCI
                , ucSI.rbSEGAPCMS_Emu
                , null, null, null, null, null
                );


            setting.YM3526Type = new Setting.ChipType2[2];
            setting.YM3526Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM3526Type[0]
                , ucSI.rbYM3526P_SCCI
                , ucSI.cmbYM3526P_SCCI
                , ucSI.rbYM3526P_Emu
                , null, null, null, null, null
                );

            setting.YM3526Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM3526Type[1]
                , ucSI.rbYM3526S_SCCI
                , ucSI.cmbYM3526S_SCCI
                , ucSI.rbYM3526S_Emu
                , null, null, null, null, null
                );


            setting.YM3812Type = new Setting.ChipType2[2];
            setting.YM3812Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM3812Type[0]
                , ucSI.rbYM3812P_SCCI
                , ucSI.cmbYM3812P_SCCI
                , ucSI.rbYM3812P_Emu
                , null, null, null, null, null
                );

            setting.YM3812Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YM3812Type[1]
                , ucSI.rbYM3812S_SCCI
                , ucSI.cmbYM3812S_SCCI
                , ucSI.rbYM3812S_Emu
                , null, null, null, null, null
                );


            setting.YMF262Type = new Setting.ChipType2[2];
            setting.YMF262Type[0] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YMF262Type[0]
                , ucSI.rbYMF262P_SCCI
                , ucSI.cmbYMF262P_SCCI
                , ucSI.rbYMF262P_Emu
                , null, null, null, null, null
                );

            setting.YMF262Type[1] = new Setting.ChipType2();
            SetChipType2FromControls(
                setting.YMF262Type[1]
                , ucSI.rbYMF262S_SCCI
                , ucSI.cmbYMF262S_SCCI
                , ucSI.rbYMF262S_Emu
                , null, null, null, null, null
                );


            #endregion


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
            setting.other.InstFormat = (EnmInstFormat)cmbInstFormat.SelectedIndex;
            if (int.TryParse(tbScreenFrameRate.Text, out i))
            {
                setting.other.ScreenFrameRate = Math.Max(Math.Min(i, 120), 10);
            }
            setting.other.AutoOpen = cbAutoOpen.Checked;
            setting.other.DumpSwitch = cbDumpSwitch.Checked;
            setting.other.DumpPath = tbDumpPath.Text;
            setting.other.WavSwitch = cbWavSwitch.Checked;
            setting.other.WavPath = tbWavPath.Text;
            setting.other.TextExt = tbTextExt.Text;
            setting.other.MMLExt = tbMMLExt.Text;
            setting.other.ImageExt = tbImageExt.Text;
            setting.other.AutoOpenText = cbAutoOpenText.Checked;
            setting.other.AutoOpenMML = cbAutoOpenMML.Checked;
            setting.other.AutoOpenImg = cbAutoOpenImg.Checked;
            setting.other.InitAlways = cbInitAlways.Checked;
            setting.other.EmptyPlayList = cbEmptyPlayList.Checked;
            setting.other.ExAll = cbExALL.Checked;
            setting.other.NonRenderingForPause = cbNonRenderingForPause.Checked;

            setting.Debug_DispFrameCounter = cbDispFrameCounter.Checked;


            setting.HiyorimiMode = cbHiyorimiMode.Checked;

            setting.midiExport.UseMIDIExport = cbUseMIDIExport.Checked;
            setting.midiExport.ExportPath = tbMIDIOutputPath.Text;
            setting.midiExport.UseVOPMex = cbMIDIUseVOPM.Checked;
            setting.midiExport.KeyOnFnum = cbMIDIKeyOnFnum.Checked;
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
                        //GM / XG / GS / LA / GS(SC - 55_1) / GS(SC - 55_2)
                        moi.type = 0;
                        if (stype == "XG") moi.type = 1;
                        if (stype == "GS") moi.type = 2;
                        if (stype == "LA") moi.type = 3;
                        if (stype == "GS(SC - 55_1)") moi.type = 4;
                        if (stype == "GS(SC - 55_2)") moi.type = 5;
                        string sbeforeSend = (string)d.Rows[i].Cells[5].Value;
                        moi.beforeSendType = 0;
                        if (sbeforeSend == "GM Reset") moi.beforeSendType = 1;
                        if (sbeforeSend == "XG Reset") moi.beforeSendType = 2;
                        if (sbeforeSend == "GS Reset") moi.beforeSendType = 3;
                        if (sbeforeSend == "Custom") moi.beforeSendType = 4;

                        string mn = (string)d.Rows[i].Cells[6].Value;
                        if (moi.isVST)
                        {
                            moi.vendor = mn;
                            moi.manufacturer = -1;
                        }
                        else
                        {
                            moi.vendor = "";
                            moi.manufacturer = (mn == null || mn == "Unknown") ? -1 : (int)(Enum.Parse(typeof(NAudio.Manufacturers), mn));
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

            setting.midiOut.GMReset = tbBeforeSend_GMReset.Text;
            setting.midiOut.XGReset = tbBeforeSend_XGReset.Text;
            setting.midiOut.GSReset = tbBeforeSend_GSReset.Text;
            setting.midiOut.Custom = tbBeforeSend_Custom.Text;

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
            setting.nsf.HPF = trkbNSFHPF.Value;
            setting.nsf.LPF = trkbNSFLPF.Value;

            setting.sid = new Setting.SID();
            setting.sid.RomKernalPath = tbSIDKernal.Text;
            setting.sid.RomBasicPath = tbSIDBasic.Text;
            setting.sid.RomCharacterPath = tbSIDCharacter.Text;
            if (rdSIDQ1.Checked) setting.sid.Quality = 0;
            if (rdSIDQ2.Checked) setting.sid.Quality = 1;
            if (rdSIDQ3.Checked) setting.sid.Quality = 2;
            if (rdSIDQ4.Checked) setting.sid.Quality = 3;
            try
            {
                setting.sid.OutputBufferSize = Math.Min(Math.Max(int.Parse(tbSIDOutputBufferSize.Text), 100), 999999);
            }
            catch
            {
                setting.sid.OutputBufferSize = 5000;
            }

            setting.sid.c64model = rbSIDC64Model_PAL.Checked ? 0 : (
                rbSIDC64Model_NTSC.Checked ? 1 : (
                rbSIDC64Model_OLDNTSC.Checked ? 2 : (
                rbSIDC64Model_DREAN.Checked ? 3 : 0)));
            setting.sid.c64modelForce = cbSIDC64Model_Force.Checked;

            setting.sid.sidmodel = rbSIDModel_6581.Checked ? 0 : (
                rbSIDModel_8580.Checked ? 1 : 0);
            setting.sid.sidmodelForce = cbSIDModel_Force.Checked;




            setting.nukedOPN2 = new Setting.NukedOPN2();
            if (rbNukedOPN2OptionYM2612.Checked) setting.nukedOPN2.EmuType = 2;
            if (rbNukedOPN2OptionASIC.Checked) setting.nukedOPN2.EmuType = 1;
            if (rbNukedOPN2OptionDiscrete.Checked) setting.nukedOPN2.EmuType = 0;
            if (rbNukedOPN2OptionYM2612u.Checked) setting.nukedOPN2.EmuType = 3;
            if (rbNukedOPN2OptionASIClp.Checked) setting.nukedOPN2.EmuType = 4;
            setting.nukedOPN2.GensDACHPF = cbGensDACHPF.Checked;
            setting.nukedOPN2.GensSSGEG = cbGensSSGEG.Checked;

            setting.autoBalance = new Setting.AutoBalance();
            setting.autoBalance.UseThis = cbAutoBalanceUseThis.Checked;
            setting.autoBalance.LoadSongBalance = rbAutoBalanceLoadSongBalance.Checked;
            setting.autoBalance.LoadDriverBalance = rbAutoBalanceLoadDriverBalance.Checked;
            setting.autoBalance.SaveSongBalance = rbAutoBalanceSaveSongBalance.Checked;
            setting.autoBalance.SamePositionAsSongData = rbAutoBalanceSamePositionAsSongData.Checked;



            setting.pmdDotNET.compilerArguments = tbPMDCompilerArguments.Text;
            setting.pmdDotNET.isAuto = rbPMDAuto.Checked;
            setting.pmdDotNET.soundBoard = rbPMDNrmB.Checked ? 0 : (rbPMDSpbB.Checked ? 1 : 2);
            setting.pmdDotNET.setManualVolume = cbPMDSetManualVolume.Checked;
            setting.pmdDotNET.usePPSDRV = cbPMDUsePPSDRV.Checked;
            setting.pmdDotNET.usePPZ8 = cbPMDUsePPZ8.Checked;
            setting.pmdDotNET.driverArguments = tbPMDDriverArguments.Text;
            setting.pmdDotNET.usePPSDRVUseInterfaceDefaultFreq = rbPMDUsePPSDRVFreqDefault.Checked;
            int nn;
            if (!int.TryParse(tbPMDPPSDRVFreq.Text, out nn)) nn = 2000;
            setting.pmdDotNET.PPSDRVManualFreq = nn;
            if (!int.TryParse(tbPMDPPSDRVManualWait.Text, out nn)) nn = 1;
            nn = Math.Min(Math.Max(nn, 0), 100);
            setting.pmdDotNET.PPSDRVManualWait = nn;
            if (!int.TryParse(tbPMDVolumeFM.Text, out nn)) nn = 0;
            nn = Math.Min(Math.Max(nn, -191), 20);
            setting.pmdDotNET.volumeFM = nn;
            if (!int.TryParse(tbPMDVolumeSSG.Text, out nn)) nn = 0;
            nn = Math.Min(Math.Max(nn, -191), 20);
            setting.pmdDotNET.volumeSSG = nn;
            if (!int.TryParse(tbPMDVolumeRhythm.Text, out nn)) nn = 0;
            nn = Math.Min(Math.Max(nn, -191), 20);
            setting.pmdDotNET.volumeRhythm = nn;
            if (!int.TryParse(tbPMDVolumeAdpcm.Text, out nn)) nn = 0;
            nn = Math.Min(Math.Max(nn, -191), 20);
            setting.pmdDotNET.volumeAdpcm = nn;
            if (!int.TryParse(tbPMDVolumeGIMICSSG.Text, out nn)) nn = 31;
            nn = Math.Min(Math.Max(nn, 0), 127);
            setting.pmdDotNET.volumeGIMICSSG = nn;




            setting.keyBoardHook.UseKeyBoardHook = cbUseKeyBoardHook.Checked;

            setting.keyBoardHook.Stop.Shift = cbStopShift.Checked;
            setting.keyBoardHook.Stop.Ctrl = cbStopCtrl.Checked;
            setting.keyBoardHook.Stop.Win = cbStopWin.Checked;
            setting.keyBoardHook.Stop.Alt = cbStopAlt.Checked;
            setting.keyBoardHook.Stop.Key = string.IsNullOrEmpty(lblStopKey.Text) ? "(None)" : lblStopKey.Text;

            setting.keyBoardHook.Pause.Shift = cbPauseShift.Checked;
            setting.keyBoardHook.Pause.Ctrl = cbPauseCtrl.Checked;
            setting.keyBoardHook.Pause.Win = cbPauseWin.Checked;
            setting.keyBoardHook.Pause.Alt = cbPauseAlt.Checked;
            setting.keyBoardHook.Pause.Key = string.IsNullOrEmpty(lblPauseKey.Text) ? "(None)" : lblPauseKey.Text;

            setting.keyBoardHook.Fadeout.Shift = cbFadeoutShift.Checked;
            setting.keyBoardHook.Fadeout.Ctrl = cbFadeoutCtrl.Checked;
            setting.keyBoardHook.Fadeout.Win = cbFadeoutWin.Checked;
            setting.keyBoardHook.Fadeout.Alt = cbFadeoutAlt.Checked;
            setting.keyBoardHook.Fadeout.Key = string.IsNullOrEmpty(lblFadeoutKey.Text) ? "(None)" : lblFadeoutKey.Text;

            setting.keyBoardHook.Prev.Shift = cbPrevShift.Checked;
            setting.keyBoardHook.Prev.Ctrl = cbPrevCtrl.Checked;
            setting.keyBoardHook.Prev.Win = cbPrevWin.Checked;
            setting.keyBoardHook.Prev.Alt = cbPrevAlt.Checked;
            setting.keyBoardHook.Prev.Key = string.IsNullOrEmpty(lblPrevKey.Text) ? "(None)" : lblPrevKey.Text;

            setting.keyBoardHook.Slow.Shift = cbSlowShift.Checked;
            setting.keyBoardHook.Slow.Ctrl = cbSlowCtrl.Checked;
            setting.keyBoardHook.Slow.Win = cbSlowWin.Checked;
            setting.keyBoardHook.Slow.Alt = cbSlowAlt.Checked;
            setting.keyBoardHook.Slow.Key = string.IsNullOrEmpty(lblSlowKey.Text) ? "(None)" : lblSlowKey.Text;

            setting.keyBoardHook.Play.Shift = cbPlayShift.Checked;
            setting.keyBoardHook.Play.Ctrl = cbPlayCtrl.Checked;
            setting.keyBoardHook.Play.Win = cbPlayWin.Checked;
            setting.keyBoardHook.Play.Alt = cbPlayAlt.Checked;
            setting.keyBoardHook.Play.Key = string.IsNullOrEmpty(lblPlayKey.Text) ? "(None)" : lblPlayKey.Text;

            setting.keyBoardHook.Fast.Shift = cbFastShift.Checked;
            setting.keyBoardHook.Fast.Ctrl = cbFastCtrl.Checked;
            setting.keyBoardHook.Fast.Win = cbFastWin.Checked;
            setting.keyBoardHook.Fast.Alt = cbFastAlt.Checked;
            setting.keyBoardHook.Fast.Key = string.IsNullOrEmpty(lblFastKey.Text) ? "(None)" : lblFastKey.Text;

            setting.keyBoardHook.Next.Shift = cbNextShift.Checked;
            setting.keyBoardHook.Next.Ctrl = cbNextCtrl.Checked;
            setting.keyBoardHook.Next.Win = cbNextWin.Checked;
            setting.keyBoardHook.Next.Alt = cbNextAlt.Checked;
            setting.keyBoardHook.Next.Key = string.IsNullOrEmpty(lblNextKey.Text) ? "(None)" : lblNextKey.Text;


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private static void SetChipType2FromControls(
            Setting.ChipType2 ct
            , RadioButton rb_SCCI
            , ComboBox cmb_SCCI
            , RadioButton rb_EMU0
            , RadioButton rb_EMU1
            , RadioButton rb_EMU2
            , RadioButton rb_SCCI_E
            , ComboBox cmb_SCCI_E1
            , ComboBox cmb_SCCI_E2
            )
        {
            ct.UseReal = new bool[1];
            ct.UseReal[0] = rb_SCCI.Checked;
            ct.realChipInfo = new Setting.ChipType2.RealChipInfo[rb_SCCI_E == null ? 1 : 3];
            for (int i = 0; i < ct.realChipInfo.Length; i++) ct.realChipInfo[i] = new Setting.ChipType2.RealChipInfo();
            if (rb_SCCI.Checked)
            {
                if (cmb_SCCI.SelectedItem != null)
                {
                    string n = cmb_SCCI.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    ct.realChipInfo[0].InterfaceName = ns[0];
                    ct.realChipInfo[0].SoundLocation = int.Parse(ns[1]);
                    ct.realChipInfo[0].BusID = int.Parse(ns[2]);
                    ct.realChipInfo[0].SoundChip = int.Parse(ns[3]);
                }
            }

            ct.UseEmu = null;
            if (rb_EMU2 != null) ct.UseEmu = new bool[3];
            else if (rb_EMU1 != null) ct.UseEmu = new bool[2];
            else if (rb_EMU0 != null) ct.UseEmu = new bool[1];
            if (rb_EMU0 != null) ct.UseEmu[0] = rb_EMU0.Checked;// (rb_EMU0.Checked || rb_SCCI.Checked);
            if (rb_EMU1 != null) ct.UseEmu[1] = rb_EMU1.Checked;
            if (rb_EMU2 != null) ct.UseEmu[2] = rb_EMU2.Checked;

            if (rb_SCCI_E != null && rb_SCCI_E.Checked)
            {
                if (cmb_SCCI_E1.SelectedItem != null)
                {
                    string n = cmb_SCCI_E1.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    ct.realChipInfo[1].InterfaceName = ns[0];
                    ct.realChipInfo[1].SoundLocation = int.Parse(ns[1]);
                    ct.realChipInfo[1].BusID = int.Parse(ns[2]);
                    ct.realChipInfo[1].SoundChip = int.Parse(ns[3]);
                }
                if (cmb_SCCI_E2.SelectedItem != null)
                {
                    string n = cmb_SCCI_E2.SelectedItem.ToString();
                    n = n.Substring(0, n.IndexOf(")")).Substring(1);
                    string[] ns = n.Split(':');
                    ct.realChipInfo[2].InterfaceName = ns[0];
                    ct.realChipInfo[2].SoundLocation = int.Parse(ns[1]);
                    ct.realChipInfo[2].BusID = int.Parse(ns[2]);
                    ct.realChipInfo[2].SoundChip = int.Parse(ns[3]);
                }
            }

        }

        /// <summary>
        /// 入力値チェック
        /// </summary>
        /// <returns></returns>
        private bool CheckSetting()
        {
            HashSet<string> hsSCCIs = new HashSet<string>();
            bool ret = false;

            //SCCI重複設定チェック

            if (ucSI.rbYM2612P_SCCI.Checked)
                if (ucSI.cmbYM2612P_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2612P_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2612P_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2612S_SCCI.Checked)
                if (ucSI.cmbYM2612S_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2612S_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2612S_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbSN76489P_SCCI.Checked)
                if (ucSI.cmbSN76489P_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbSN76489P_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbSN76489P_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbSN76489S_SCCI.Checked)
                if (ucSI.cmbSN76489S_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbSN76489S_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbSN76489S_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2608P_SCCI.Checked)
                if (ucSI.cmbYM2608P_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2608P_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2608P_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2608S_SCCI.Checked)
                if (ucSI.cmbYM2608S_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2608S_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2608S_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2151P_SCCI.Checked)
                if (ucSI.cmbYM2151P_SCCI.SelectedItem!=null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2151P_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2151P_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2151S_SCCI.Checked)
                if (ucSI.cmbYM2151S_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2151S_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2151S_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2203P_SCCI.Checked)
                if (ucSI.cmbYM2203P_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2203P_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2203P_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2203S_SCCI.Checked)
                if (ucSI.cmbYM2203S_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2203S_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2203S_SCCI.SelectedItem.ToString());
                    else ret = true;


            if (ucSI.rbYM2413P_Real.Checked)
                if (ucSI.cmbYM2413P_Real.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2413P_Real.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2413P_Real.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2413S_Real.Checked)
                if (ucSI.cmbYM2413S_Real.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2413S_Real.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2413S_Real.SelectedItem.ToString());
                    else ret = true;



            if (ucSI.rbYM2610BP_SCCI.Checked)
                if (ucSI.cmbYM2610BP_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2610BP_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2610BP_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2610BS_SCCI.Checked)
                if (ucSI.cmbYM2610BS_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2610BS_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2610BS_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2610BEP_SCCI.Checked)
                if (ucSI.cmbYM2610BEP_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2610BEP_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2610BEP_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2610BES_SCCI.Checked)
                if (ucSI.cmbYM2610BES_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbYM2610BES_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbYM2610BES_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2610BEP_SCCI.Checked)
                if (ucSI.cmbSPPCMP_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbSPPCMP_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbSPPCMP_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbYM2610BES_SCCI.Checked)
                if (ucSI.cmbSPPCMS_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbSPPCMS_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbSPPCMS_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbC140P_Real.Checked)
                if (ucSI.cmbC140P_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbC140P_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbC140P_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbC140S_SCCI.Checked)
                if (ucSI.cmbC140S_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbC140S_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbC140S_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbSEGAPCMP_SCCI.Checked)
                if (ucSI.cmbSEGAPCMP_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbSEGAPCMP_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbSEGAPCMP_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ucSI.rbSEGAPCMS_SCCI.Checked)
                if (ucSI.cmbSEGAPCMS_SCCI.SelectedItem != null)
                    if (!hsSCCIs.Contains(ucSI.cmbSEGAPCMS_SCCI.SelectedItem.ToString()))
                        hsSCCIs.Add(ucSI.cmbSEGAPCMS_SCCI.SelectedItem.ToString());
                    else ret = true;

            if (ret)
            {
                if (MessageBox.Show(
                    "SCCI/GIMICのデバイスが重複して設定されています。強行しますか"
                    , "警告"
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Warning
                    ) == DialogResult.Yes)
                {
                    return true;
                }
                return false;
            }

            return true;
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
            string fullPath = Common.settingFilePath;
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

                if (!found) dgv[p].Rows.Add(row.Cells[0].Value, false, "", row.Cells[1].Value, "GM","None", row.Cells[2].Value);
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
            if (s == null) return;

            setting.vst.DefaultPath = Path.GetDirectoryName(ofd.FileName);

            int p = tbcMIDIoutList.SelectedIndex;
            dgv[p].Rows.Add(
                -999 
                , true 
                , s.fileName
                , s.effectName
                , "GM"
                ,"None"
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

        private void btnBeforeSend_Default_Click(object sender, EventArgs e)
        {
            Setting.MidiOut mo=new Setting.MidiOut();
            tbBeforeSend_GMReset.Text = mo.GMReset;
            tbBeforeSend_XGReset.Text = mo.XGReset;
            tbBeforeSend_GSReset.Text = mo.GSReset;
            tbBeforeSend_Custom.Text = mo.Custom;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {

        }

        private void cbUseKeyBoardHook_CheckedChanged(object sender, EventArgs e)
        {
            gbUseKeyBoardHook.Enabled = cbUseKeyBoardHook.Checked;
        }

        private void btStopClr_Click(object sender, EventArgs e)
        {
            lblStopKey.Text = "(None)";
            btStopClr.Enabled = false;
        }

        private void btPauseClr_Click(object sender, EventArgs e)
        {
            lblPauseKey.Text = "(None)";
            btPauseClr.Enabled = false;
        }

        private void btFadeoutClr_Click(object sender, EventArgs e)
        {
            lblFadeoutKey.Text = "(None)";
            btFadeoutClr.Enabled = false;
        }

        private void btPrevClr_Click(object sender, EventArgs e)
        {
            lblPrevKey.Text = "(None)";
            btPrevClr.Enabled = false;
        }

        private void btSlowClr_Click(object sender, EventArgs e)
        {
            lblSlowKey.Text = "(None)";
            btSlowClr.Enabled = false;
        }

        private void btPlayClr_Click(object sender, EventArgs e)
        {
            lblPlayKey.Text = "(None)";
            btPlayClr.Enabled = false;
        }

        private void btFastClr_Click(object sender, EventArgs e)
        {
            lblFastKey.Text = "(None)";
            btFastClr.Enabled = false;
        }

        private void btNextClr_Click(object sender, EventArgs e)
        {
            lblNextKey.Text = "(None)";
            btNextClr.Enabled = false;
        }

        private void btStopSet_Click(object sender, EventArgs e)
        {

            lblKey = lblStopKey;
            btSet = btStopSet;
            btClr = btStopClr;
            btOK = btnOK;
            btStopSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;

        }

        private void btPauseSet_Click(object sender, EventArgs e)
        {
            lblKey = lblPauseKey;
            btSet = btPauseSet;
            btOK = btnOK;
            btClr = btPauseClr;
            btPauseSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;

        }

        private void btFadeoutSet_Click(object sender, EventArgs e)
        {
            lblKey = lblFadeoutKey;
            btSet = btFadeoutSet;
            btClr = btFadeoutClr;
            btOK = btnOK;
            btFadeoutSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;
        }

        private void btPrevSet_Click(object sender, EventArgs e)
        {
            lblKey = lblPrevKey;
            btSet = btPrevSet;
            btClr = btPrevClr;
            btOK = btnOK;
            btPrevSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;
        }

        private void btSlowSet_Click(object sender, EventArgs e)
        {
            lblKey = lblSlowKey;
            btSet = btSlowSet;
            btClr = btSlowClr;
            btOK = btnOK;
            btSlowSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;
        }

        private void btPlaySet_Click(object sender, EventArgs e)
        {
            lblKey = lblPlayKey;
            btSet = btPlaySet;
            btClr = btPlayClr;
            btOK = btnOK;
            btPlaySet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;
        }

        private void btFastSet_Click(object sender, EventArgs e)
        {
            lblKey = lblFastKey;
            btSet = btFastSet;
            btClr = btFastClr;
            btOK = btnOK;
            btFastSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;
        }

        private void btNextSet_Click(object sender, EventArgs e)
        {
            lblKey = lblNextKey;
            btSet = btNextSet;
            btClr = btNextClr;
            btOK = btnOK;
            btNextSet.Enabled = false;
            btnOK.Enabled = false;
            lblKey.Text = "入力待ち";
            lblKey.ForeColor = System.Drawing.Color.Red;

            lblNotice = lblKeyBoardHookNotice;
            lblKeyBoardHookNotice.Visible = true;

            frmMain.keyHookMeth = keyHookMeth;
        }

        public static Label lblKey = null;
        public static Label lblNotice = null;
        public static Button btSet = null;
        public static Button btClr = null;
        public static Button btOK = null;
        public static void keyHookMeth(HongliangSoft.Utilities.Gui.KeyboardHookedEventArgs e)
        {
            if (e.UpDown != HongliangSoft.Utilities.Gui.KeyboardUpDown.Up) return;

            lblKey.ForeColor = System.Drawing.SystemColors.ControlText;
            lblKey.Text= e.KeyCode.ToString();
            lblNotice.Visible = false;

            frmMain.keyHookMeth = null;
            btSet.Enabled = true;
            btOK.Enabled = true;
            btClr.Enabled = true;

        }

        private void frmSetting_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (frmMain.keyHookMeth == keyHookMeth)
            {
                frmMain.keyHookMeth = null;
            }

        }

        private void llOpenGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            llOpenGithub.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/kuma4649/MDPlayer/releases/latest");
        }

        private void rbPMDManual_CheckedChanged(object sender, EventArgs e)
        {
            gbPMDManual.Enabled = rbPMDManual.Checked;
        }

        private void btnPMDResetCompilerArhguments_Click(object sender, EventArgs e)
        {
            tbPMDCompilerArguments.Text = "/v /C";
        }

        private void btnPMDResetDriverArguments_Click(object sender, EventArgs e)
        {
            tbPMDDriverArguments.Text = "";
        }

        private void cbPMDUsePPSDRV_CheckedChanged(object sender, EventArgs e)
        {
            gbPPSDRV.Enabled = cbPMDUsePPSDRV.Checked;
        }

        private void cbPMDSetManualVolume_CheckedChanged(object sender, EventArgs e)
        {
            gbPMDSetManualVolume.Enabled = cbPMDSetManualVolume.Checked;
        }

        private void rbPMDUsePPSDRVManualFreq_CheckedChanged(object sender, EventArgs e)
        {
            tbPMDPPSDRVFreq.Enabled = rbPMDUsePPSDRVManualFreq.Checked;
        }

        private void btnPMDPPSDRVManualWait_Click(object sender, EventArgs e)
        {
            tbPMDPPSDRVManualWait.Text = "1";
        }

        private void tbPMDPPSDRVFreq_Click(object sender, EventArgs e)
        {
            rbPMDUsePPSDRVManualFreq_CheckedChanged(null, null);
        }

        private void tbPMDPPSDRVFreq_MouseClick(object sender, MouseEventArgs e)
        {
            rbPMDUsePPSDRVManualFreq_CheckedChanged(null, null);
        }

        private void groupBox20_Enter(object sender, EventArgs e)
        {

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
