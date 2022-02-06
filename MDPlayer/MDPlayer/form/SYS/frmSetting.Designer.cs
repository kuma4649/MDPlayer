using MDPlayer.Properties;
namespace MDPlayer.form
{
    partial class frmSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetting));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbWaveOut = new System.Windows.Forms.GroupBox();
            this.cmbWaveOutDevice = new System.Windows.Forms.ComboBox();
            this.rbWaveOut = new System.Windows.Forms.RadioButton();
            this.rbAsioOut = new System.Windows.Forms.RadioButton();
            this.rbWasapiOut = new System.Windows.Forms.RadioButton();
            this.gbAsioOut = new System.Windows.Forms.GroupBox();
            this.btnASIOControlPanel = new System.Windows.Forms.Button();
            this.cmbAsioDevice = new System.Windows.Forms.ComboBox();
            this.rbDirectSoundOut = new System.Windows.Forms.RadioButton();
            this.gbWasapiOut = new System.Windows.Forms.GroupBox();
            this.rbExclusive = new System.Windows.Forms.RadioButton();
            this.rbShare = new System.Windows.Forms.RadioButton();
            this.cmbWasapiDevice = new System.Windows.Forms.ComboBox();
            this.gbDirectSound = new System.Windows.Forms.GroupBox();
            this.cmbDirectSoundDevice = new System.Windows.Forms.ComboBox();
            this.tcSetting = new System.Windows.Forms.TabControl();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.rbNullDevice = new System.Windows.Forms.RadioButton();
            this.label36 = new System.Windows.Forms.Label();
            this.lblWaitTime = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.lblLatencyUnit = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.lblLatency = new System.Windows.Forms.Label();
            this.cmbWaitTime = new System.Windows.Forms.ComboBox();
            this.cmbSampleRate = new System.Windows.Forms.ComboBox();
            this.cmbLatency = new System.Windows.Forms.ComboBox();
            this.rbSPPCM = new System.Windows.Forms.RadioButton();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.cmbSPPCMDevice = new System.Windows.Forms.ComboBox();
            this.tpModule = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbUnuseRealChip = new System.Windows.Forms.CheckBox();
            this.ucSI = new MDPlayer.form.ucSettingInstruments();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbHiyorimiMode = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbLatencyEmu = new System.Windows.Forms.TextBox();
            this.tbLatencySCCI = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tpNuked = new System.Windows.Forms.TabPage();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.cbGensSSGEG = new System.Windows.Forms.CheckBox();
            this.cbGensDACHPF = new System.Windows.Forms.CheckBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.rbNukedOPN2OptionYM2612u = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionYM2612 = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionDiscrete = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionASIClp = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionASIC = new System.Windows.Forms.RadioButton();
            this.tpNSF = new System.Windows.Forms.TabPage();
            this.trkbNSFLPF = new System.Windows.Forms.TrackBar();
            this.label53 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.trkbNSFHPF = new System.Windows.Forms.TrackBar();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.cbNSFDmc_DPCMReverse = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_RandomizeTri = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_TriMute = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_RandomizeNoise = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_DPCMAntiClick = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_EnablePNoise = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_Enable4011 = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_NonLinearMixer = new System.Windows.Forms.CheckBox();
            this.cbNSFDmc_UnmuteOnReset = new System.Windows.Forms.CheckBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.cbNSFN160_Serial = new System.Windows.Forms.CheckBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.cbNSFMmc5_PhaseRefresh = new System.Windows.Forms.CheckBox();
            this.cbNSFMmc5_NonLinearMixer = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.cbNFSNes_DutySwap = new System.Windows.Forms.CheckBox();
            this.cbNFSNes_PhaseRefresh = new System.Windows.Forms.CheckBox();
            this.cbNFSNes_NonLinearMixer = new System.Windows.Forms.CheckBox();
            this.cbNFSNes_UnmuteOnReset = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.tbNSFFds_LPF = new System.Windows.Forms.TextBox();
            this.cbNFSFds_4085Reset = new System.Windows.Forms.CheckBox();
            this.cbNSFFDSWriteDisable8000 = new System.Windows.Forms.CheckBox();
            this.tpSID = new System.Windows.Forms.TabPage();
            this.groupBox28 = new System.Windows.Forms.GroupBox();
            this.cbSIDModel_Force = new System.Windows.Forms.CheckBox();
            this.rbSIDModel_8580 = new System.Windows.Forms.RadioButton();
            this.rbSIDModel_6581 = new System.Windows.Forms.RadioButton();
            this.groupBox27 = new System.Windows.Forms.GroupBox();
            this.cbSIDC64Model_Force = new System.Windows.Forms.CheckBox();
            this.rbSIDC64Model_DREAN = new System.Windows.Forms.RadioButton();
            this.rbSIDC64Model_OLDNTSC = new System.Windows.Forms.RadioButton();
            this.rbSIDC64Model_NTSC = new System.Windows.Forms.RadioButton();
            this.rbSIDC64Model_PAL = new System.Windows.Forms.RadioButton();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.rdSIDQ1 = new System.Windows.Forms.RadioButton();
            this.rdSIDQ3 = new System.Windows.Forms.RadioButton();
            this.rdSIDQ2 = new System.Windows.Forms.RadioButton();
            this.rdSIDQ4 = new System.Windows.Forms.RadioButton();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.btnSIDBasic = new System.Windows.Forms.Button();
            this.btnSIDCharacter = new System.Windows.Forms.Button();
            this.btnSIDKernal = new System.Windows.Forms.Button();
            this.tbSIDCharacter = new System.Windows.Forms.TextBox();
            this.tbSIDBasic = new System.Windows.Forms.TextBox();
            this.tbSIDKernal = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.tbSIDOutputBufferSize = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.tpPMDDotNET = new System.Windows.Forms.TabPage();
            this.rbPMDManual = new System.Windows.Forms.RadioButton();
            this.rbPMDAuto = new System.Windows.Forms.RadioButton();
            this.btnPMDResetDriverArguments = new System.Windows.Forms.Button();
            this.label54 = new System.Windows.Forms.Label();
            this.btnPMDResetCompilerArhguments = new System.Windows.Forms.Button();
            this.tbPMDDriverArguments = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.tbPMDCompilerArguments = new System.Windows.Forms.TextBox();
            this.gbPMDManual = new System.Windows.Forms.GroupBox();
            this.cbPMDSetManualVolume = new System.Windows.Forms.CheckBox();
            this.cbPMDUsePPZ8 = new System.Windows.Forms.CheckBox();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.rbPMD86B = new System.Windows.Forms.RadioButton();
            this.rbPMDSpbB = new System.Windows.Forms.RadioButton();
            this.rbPMDNrmB = new System.Windows.Forms.RadioButton();
            this.cbPMDUsePPSDRV = new System.Windows.Forms.CheckBox();
            this.gbPPSDRV = new System.Windows.Forms.GroupBox();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.rbPMDUsePPSDRVManualFreq = new System.Windows.Forms.RadioButton();
            this.label56 = new System.Windows.Forms.Label();
            this.rbPMDUsePPSDRVFreqDefault = new System.Windows.Forms.RadioButton();
            this.btnPMDPPSDRVManualWait = new System.Windows.Forms.Button();
            this.label57 = new System.Windows.Forms.Label();
            this.tbPMDPPSDRVFreq = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.tbPMDPPSDRVManualWait = new System.Windows.Forms.TextBox();
            this.gbPMDSetManualVolume = new System.Windows.Forms.GroupBox();
            this.label59 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.tbPMDVolumeAdpcm = new System.Windows.Forms.TextBox();
            this.label61 = new System.Windows.Forms.Label();
            this.tbPMDVolumeRhythm = new System.Windows.Forms.TextBox();
            this.label62 = new System.Windows.Forms.Label();
            this.tbPMDVolumeSSG = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.tbPMDVolumeGIMICSSG = new System.Windows.Forms.TextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.tbPMDVolumeFM = new System.Windows.Forms.TextBox();
            this.tpMIDIOut = new System.Windows.Forms.TabPage();
            this.btnAddVST = new System.Windows.Forms.Button();
            this.tbcMIDIoutList = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListA = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmIsVST = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ClmBeforeSend = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnUP_A = new System.Windows.Forms.Button();
            this.btnDOWN_A = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListB = new System.Windows.Forms.DataGridView();
            this.btnUP_B = new System.Windows.Forms.Button();
            this.btnDOWN_B = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListC = new System.Windows.Forms.DataGridView();
            this.btnUP_C = new System.Windows.Forms.Button();
            this.btnDOWN_C = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListD = new System.Windows.Forms.DataGridView();
            this.btnUP_D = new System.Windows.Forms.Button();
            this.btnDOWN_D = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListE = new System.Windows.Forms.DataGridView();
            this.btnUP_E = new System.Windows.Forms.Button();
            this.btnDOWN_E = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListF = new System.Windows.Forms.DataGridView();
            this.btnUP_F = new System.Windows.Forms.Button();
            this.btnDOWN_F = new System.Windows.Forms.Button();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListG = new System.Windows.Forms.DataGridView();
            this.btnUP_G = new System.Windows.Forms.Button();
            this.btnDOWN_G = new System.Windows.Forms.Button();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListH = new System.Windows.Forms.DataGridView();
            this.btnUP_H = new System.Windows.Forms.Button();
            this.btnDOWN_H = new System.Windows.Forms.Button();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListI = new System.Windows.Forms.DataGridView();
            this.btnUP_I = new System.Windows.Forms.Button();
            this.btnDOWN_I = new System.Windows.Forms.Button();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.dgvMIDIoutListJ = new System.Windows.Forms.DataGridView();
            this.button17 = new System.Windows.Forms.Button();
            this.btnDOWN_J = new System.Windows.Forms.Button();
            this.btnSubMIDIout = new System.Windows.Forms.Button();
            this.btnAddMIDIout = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.dgvMIDIoutPallet = new System.Windows.Forms.DataGridView();
            this.clmID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmManufacturer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSpacer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label16 = new System.Windows.Forms.Label();
            this.tpMIDIOut2 = new System.Windows.Forms.TabPage();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.btnBeforeSend_Default = new System.Windows.Forms.Button();
            this.tbBeforeSend_Custom = new System.Windows.Forms.TextBox();
            this.tbBeforeSend_XGReset = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.tbBeforeSend_GSReset = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.tbBeforeSend_GMReset = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.tabMIDIExp = new System.Windows.Forms.TabPage();
            this.cbUseMIDIExport = new System.Windows.Forms.CheckBox();
            this.gbMIDIExport = new System.Windows.Forms.GroupBox();
            this.cbMIDIKeyOnFnum = new System.Windows.Forms.CheckBox();
            this.cbMIDIUseVOPM = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbMIDIYM2612 = new System.Windows.Forms.CheckBox();
            this.cbMIDISN76489Sec = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2612Sec = new System.Windows.Forms.CheckBox();
            this.cbMIDISN76489 = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2151 = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2610BSec = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2151Sec = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2610B = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2203 = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2608Sec = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2203Sec = new System.Windows.Forms.CheckBox();
            this.cbMIDIYM2608 = new System.Windows.Forms.CheckBox();
            this.cbMIDIPlayless = new System.Windows.Forms.CheckBox();
            this.btnMIDIOutputPath = new System.Windows.Forms.Button();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.tbMIDIOutputPath = new System.Windows.Forms.TextBox();
            this.tpMIDIKBD = new System.Windows.Forms.TabPage();
            this.cbUseMIDIKeyboard = new System.Windows.Forms.CheckBox();
            this.gbMIDIKeyboard = new System.Windows.Forms.GroupBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbCCFadeout = new System.Windows.Forms.TextBox();
            this.tbCCPause = new System.Windows.Forms.TextBox();
            this.tbCCSlow = new System.Windows.Forms.TextBox();
            this.tbCCPrevious = new System.Windows.Forms.TextBox();
            this.tbCCNext = new System.Windows.Forms.TextBox();
            this.tbCCFast = new System.Windows.Forms.TextBox();
            this.tbCCStop = new System.Windows.Forms.TextBox();
            this.tbCCPlay = new System.Windows.Forms.TextBox();
            this.tbCCCopyLog = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbCCDelLog = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbCCChCopy = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.gbUseChannel = new System.Windows.Forms.GroupBox();
            this.rbMONO = new System.Windows.Forms.RadioButton();
            this.rbPOLY = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.rbFM6 = new System.Windows.Forms.RadioButton();
            this.rbFM3 = new System.Windows.Forms.RadioButton();
            this.rbFM5 = new System.Windows.Forms.RadioButton();
            this.rbFM2 = new System.Windows.Forms.RadioButton();
            this.rbFM4 = new System.Windows.Forms.RadioButton();
            this.rbFM1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbFM1 = new System.Windows.Forms.CheckBox();
            this.cbFM6 = new System.Windows.Forms.CheckBox();
            this.cbFM2 = new System.Windows.Forms.CheckBox();
            this.cbFM5 = new System.Windows.Forms.CheckBox();
            this.cbFM3 = new System.Windows.Forms.CheckBox();
            this.cbFM4 = new System.Windows.Forms.CheckBox();
            this.cmbMIDIIN = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tpKeyBoard = new System.Windows.Forms.TabPage();
            this.cbUseKeyBoardHook = new System.Windows.Forms.CheckBox();
            this.gbUseKeyBoardHook = new System.Windows.Forms.GroupBox();
            this.lblKeyBoardHookNotice = new System.Windows.Forms.Label();
            this.btNextClr = new System.Windows.Forms.Button();
            this.btPrevClr = new System.Windows.Forms.Button();
            this.btPlayClr = new System.Windows.Forms.Button();
            this.btPauseClr = new System.Windows.Forms.Button();
            this.btFastClr = new System.Windows.Forms.Button();
            this.btFadeoutClr = new System.Windows.Forms.Button();
            this.btSlowClr = new System.Windows.Forms.Button();
            this.btStopClr = new System.Windows.Forms.Button();
            this.btNextSet = new System.Windows.Forms.Button();
            this.btPrevSet = new System.Windows.Forms.Button();
            this.btPlaySet = new System.Windows.Forms.Button();
            this.btPauseSet = new System.Windows.Forms.Button();
            this.btFastSet = new System.Windows.Forms.Button();
            this.btFadeoutSet = new System.Windows.Forms.Button();
            this.btSlowSet = new System.Windows.Forms.Button();
            this.btStopSet = new System.Windows.Forms.Button();
            this.label50 = new System.Windows.Forms.Label();
            this.lblNextKey = new System.Windows.Forms.Label();
            this.lblFastKey = new System.Windows.Forms.Label();
            this.lblPlayKey = new System.Windows.Forms.Label();
            this.lblSlowKey = new System.Windows.Forms.Label();
            this.lblPrevKey = new System.Windows.Forms.Label();
            this.lblFadeoutKey = new System.Windows.Forms.Label();
            this.lblPauseKey = new System.Windows.Forms.Label();
            this.lblStopKey = new System.Windows.Forms.Label();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.pictureBox17 = new System.Windows.Forms.PictureBox();
            this.cbNextAlt = new System.Windows.Forms.CheckBox();
            this.pictureBox16 = new System.Windows.Forms.PictureBox();
            this.cbFastAlt = new System.Windows.Forms.CheckBox();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.cbPlayAlt = new System.Windows.Forms.CheckBox();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.cbSlowAlt = new System.Windows.Forms.CheckBox();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.cbPrevAlt = new System.Windows.Forms.CheckBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.cbFadeoutAlt = new System.Windows.Forms.CheckBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.cbPauseAlt = new System.Windows.Forms.CheckBox();
            this.label37 = new System.Windows.Forms.Label();
            this.cbStopAlt = new System.Windows.Forms.CheckBox();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.cbNextCtrl = new System.Windows.Forms.CheckBox();
            this.label43 = new System.Windows.Forms.Label();
            this.cbFastCtrl = new System.Windows.Forms.CheckBox();
            this.label44 = new System.Windows.Forms.Label();
            this.cbPlayCtrl = new System.Windows.Forms.CheckBox();
            this.cbStopShift = new System.Windows.Forms.CheckBox();
            this.cbSlowCtrl = new System.Windows.Forms.CheckBox();
            this.cbPauseShift = new System.Windows.Forms.CheckBox();
            this.cbPrevCtrl = new System.Windows.Forms.CheckBox();
            this.cbFadeoutShift = new System.Windows.Forms.CheckBox();
            this.cbFadeoutCtrl = new System.Windows.Forms.CheckBox();
            this.cbPrevShift = new System.Windows.Forms.CheckBox();
            this.cbPauseCtrl = new System.Windows.Forms.CheckBox();
            this.cbSlowShift = new System.Windows.Forms.CheckBox();
            this.cbStopCtrl = new System.Windows.Forms.CheckBox();
            this.cbPlayShift = new System.Windows.Forms.CheckBox();
            this.cbNextShift = new System.Windows.Forms.CheckBox();
            this.cbFastShift = new System.Windows.Forms.CheckBox();
            this.label47 = new System.Windows.Forms.Label();
            this.cbStopWin = new System.Windows.Forms.CheckBox();
            this.cbPauseWin = new System.Windows.Forms.CheckBox();
            this.cbFadeoutWin = new System.Windows.Forms.CheckBox();
            this.cbPrevWin = new System.Windows.Forms.CheckBox();
            this.cbSlowWin = new System.Windows.Forms.CheckBox();
            this.cbPlayWin = new System.Windows.Forms.CheckBox();
            this.cbFastWin = new System.Windows.Forms.CheckBox();
            this.cbNextWin = new System.Windows.Forms.CheckBox();
            this.tpBalance = new System.Windows.Forms.TabPage();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.rbAutoBalanceNotSamePositionAsSongData = new System.Windows.Forms.RadioButton();
            this.rbAutoBalanceSamePositionAsSongData = new System.Windows.Forms.RadioButton();
            this.cbAutoBalanceUseThis = new System.Windows.Forms.CheckBox();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.rbAutoBalanceNotSaveSongBalance = new System.Windows.Forms.RadioButton();
            this.rbAutoBalanceSaveSongBalance = new System.Windows.Forms.RadioButton();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.rbAutoBalanceNotLoadSongBalance = new System.Windows.Forms.RadioButton();
            this.rbAutoBalanceLoadSongBalance = new System.Windows.Forms.RadioButton();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.rbAutoBalanceNotLoadDriverBalance = new System.Windows.Forms.RadioButton();
            this.rbAutoBalanceLoadDriverBalance = new System.Windows.Forms.RadioButton();
            this.tpPlayList = new System.Windows.Forms.TabPage();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.cbAutoOpenImg = new System.Windows.Forms.CheckBox();
            this.tbImageExt = new System.Windows.Forms.TextBox();
            this.cbAutoOpenMML = new System.Windows.Forms.CheckBox();
            this.tbMMLExt = new System.Windows.Forms.TextBox();
            this.tbTextExt = new System.Windows.Forms.TextBox();
            this.cbAutoOpenText = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbEmptyPlayList = new System.Windows.Forms.CheckBox();
            this.tpOther = new System.Windows.Forms.TabPage();
            this.btnSearchPath = new System.Windows.Forms.Button();
            this.tbSearchPath = new System.Windows.Forms.TextBox();
            this.label68 = new System.Windows.Forms.Label();
            this.cbNonRenderingForPause = new System.Windows.Forms.CheckBox();
            this.cbWavSwitch = new System.Windows.Forms.CheckBox();
            this.cbUseGetInst = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbInstFormat = new System.Windows.Forms.ComboBox();
            this.lblInstFormat = new System.Windows.Forms.Label();
            this.cbDumpSwitch = new System.Windows.Forms.CheckBox();
            this.gbWav = new System.Windows.Forms.GroupBox();
            this.btnWavPath = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbWavPath = new System.Windows.Forms.TextBox();
            this.gbDump = new System.Windows.Forms.GroupBox();
            this.btnDumpPath = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDumpPath = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.tbScreenFrameRate = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.lblLoopTimes = new System.Windows.Forms.Label();
            this.btnDataPath = new System.Windows.Forms.Button();
            this.tbLoopTimes = new System.Windows.Forms.TextBox();
            this.tbDataPath = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnResetPosition = new System.Windows.Forms.Button();
            this.btnOpenSettingFolder = new System.Windows.Forms.Button();
            this.cbExALL = new System.Windows.Forms.CheckBox();
            this.cbInitAlways = new System.Windows.Forms.CheckBox();
            this.cbAutoOpen = new System.Windows.Forms.CheckBox();
            this.cbUseLoopTimes = new System.Windows.Forms.CheckBox();
            this.tpOmake = new System.Windows.Forms.TabPage();
            this.label67 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btVST = new System.Windows.Forms.Button();
            this.tbSCCbaseAddress = new System.Windows.Forms.TextBox();
            this.tbVST = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbDispFrameCounter = new System.Windows.Forms.CheckBox();
            this.tpAbout = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.llOpenGithub = new System.Windows.Forms.LinkLabel();
            this.gbWaveOut.SuspendLayout();
            this.gbAsioOut.SuspendLayout();
            this.gbWasapiOut.SuspendLayout();
            this.gbDirectSound.SuspendLayout();
            this.tcSetting.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.tpModule.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tpNuked.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.tpNSF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkbNSFLPF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkbNSFHPF)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tpSID.SuspendLayout();
            this.groupBox28.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.tpPMDDotNET.SuspendLayout();
            this.gbPMDManual.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.gbPPSDRV.SuspendLayout();
            this.groupBox33.SuspendLayout();
            this.gbPMDSetManualVolume.SuspendLayout();
            this.tpMIDIOut.SuspendLayout();
            this.tbcMIDIoutList.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListA)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListB)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListC)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListD)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListE)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListF)).BeginInit();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListG)).BeginInit();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListH)).BeginInit();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListI)).BeginInit();
            this.tabPage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListJ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutPallet)).BeginInit();
            this.tpMIDIOut2.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.tabMIDIExp.SuspendLayout();
            this.gbMIDIExport.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tpMIDIKBD.SuspendLayout();
            this.gbMIDIKeyboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbUseChannel.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tpKeyBoard.SuspendLayout();
            this.gbUseKeyBoardHook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            this.tpBalance.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.tpPlayList.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.tpOther.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gbWav.SuspendLayout();
            this.gbDump.SuspendLayout();
            this.tpOmake.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tpAbout.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbWaveOut
            // 
            resources.ApplyResources(this.gbWaveOut, "gbWaveOut");
            this.gbWaveOut.Controls.Add(this.cmbWaveOutDevice);
            this.gbWaveOut.Name = "gbWaveOut";
            this.gbWaveOut.TabStop = false;
            // 
            // cmbWaveOutDevice
            // 
            resources.ApplyResources(this.cmbWaveOutDevice, "cmbWaveOutDevice");
            this.cmbWaveOutDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaveOutDevice.FormattingEnabled = true;
            this.cmbWaveOutDevice.Name = "cmbWaveOutDevice";
            // 
            // rbWaveOut
            // 
            resources.ApplyResources(this.rbWaveOut, "rbWaveOut");
            this.rbWaveOut.Checked = true;
            this.rbWaveOut.Name = "rbWaveOut";
            this.rbWaveOut.TabStop = true;
            this.rbWaveOut.UseVisualStyleBackColor = true;
            this.rbWaveOut.CheckedChanged += new System.EventHandler(this.rbWaveOut_CheckedChanged);
            // 
            // rbAsioOut
            // 
            resources.ApplyResources(this.rbAsioOut, "rbAsioOut");
            this.rbAsioOut.Name = "rbAsioOut";
            this.rbAsioOut.UseVisualStyleBackColor = true;
            this.rbAsioOut.CheckedChanged += new System.EventHandler(this.rbAsioOut_CheckedChanged);
            // 
            // rbWasapiOut
            // 
            resources.ApplyResources(this.rbWasapiOut, "rbWasapiOut");
            this.rbWasapiOut.Name = "rbWasapiOut";
            this.rbWasapiOut.UseVisualStyleBackColor = true;
            this.rbWasapiOut.CheckedChanged += new System.EventHandler(this.rbWasapiOut_CheckedChanged);
            // 
            // gbAsioOut
            // 
            resources.ApplyResources(this.gbAsioOut, "gbAsioOut");
            this.gbAsioOut.Controls.Add(this.btnASIOControlPanel);
            this.gbAsioOut.Controls.Add(this.cmbAsioDevice);
            this.gbAsioOut.Name = "gbAsioOut";
            this.gbAsioOut.TabStop = false;
            // 
            // btnASIOControlPanel
            // 
            resources.ApplyResources(this.btnASIOControlPanel, "btnASIOControlPanel");
            this.btnASIOControlPanel.Name = "btnASIOControlPanel";
            this.btnASIOControlPanel.UseVisualStyleBackColor = true;
            this.btnASIOControlPanel.Click += new System.EventHandler(this.btnASIOControlPanel_Click);
            // 
            // cmbAsioDevice
            // 
            resources.ApplyResources(this.cmbAsioDevice, "cmbAsioDevice");
            this.cmbAsioDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAsioDevice.FormattingEnabled = true;
            this.cmbAsioDevice.Name = "cmbAsioDevice";
            // 
            // rbDirectSoundOut
            // 
            resources.ApplyResources(this.rbDirectSoundOut, "rbDirectSoundOut");
            this.rbDirectSoundOut.Name = "rbDirectSoundOut";
            this.rbDirectSoundOut.UseVisualStyleBackColor = true;
            this.rbDirectSoundOut.CheckedChanged += new System.EventHandler(this.rbDirectSoundOut_CheckedChanged);
            // 
            // gbWasapiOut
            // 
            resources.ApplyResources(this.gbWasapiOut, "gbWasapiOut");
            this.gbWasapiOut.Controls.Add(this.rbExclusive);
            this.gbWasapiOut.Controls.Add(this.rbShare);
            this.gbWasapiOut.Controls.Add(this.cmbWasapiDevice);
            this.gbWasapiOut.Name = "gbWasapiOut";
            this.gbWasapiOut.TabStop = false;
            // 
            // rbExclusive
            // 
            resources.ApplyResources(this.rbExclusive, "rbExclusive");
            this.rbExclusive.Name = "rbExclusive";
            this.rbExclusive.TabStop = true;
            this.rbExclusive.UseVisualStyleBackColor = true;
            // 
            // rbShare
            // 
            resources.ApplyResources(this.rbShare, "rbShare");
            this.rbShare.Name = "rbShare";
            this.rbShare.TabStop = true;
            this.rbShare.UseVisualStyleBackColor = true;
            // 
            // cmbWasapiDevice
            // 
            resources.ApplyResources(this.cmbWasapiDevice, "cmbWasapiDevice");
            this.cmbWasapiDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWasapiDevice.FormattingEnabled = true;
            this.cmbWasapiDevice.Name = "cmbWasapiDevice";
            // 
            // gbDirectSound
            // 
            resources.ApplyResources(this.gbDirectSound, "gbDirectSound");
            this.gbDirectSound.Controls.Add(this.cmbDirectSoundDevice);
            this.gbDirectSound.Name = "gbDirectSound";
            this.gbDirectSound.TabStop = false;
            // 
            // cmbDirectSoundDevice
            // 
            resources.ApplyResources(this.cmbDirectSoundDevice, "cmbDirectSoundDevice");
            this.cmbDirectSoundDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirectSoundDevice.FormattingEnabled = true;
            this.cmbDirectSoundDevice.Name = "cmbDirectSoundDevice";
            // 
            // tcSetting
            // 
            resources.ApplyResources(this.tcSetting, "tcSetting");
            this.tcSetting.Controls.Add(this.tpOutput);
            this.tcSetting.Controls.Add(this.tpModule);
            this.tcSetting.Controls.Add(this.tpNuked);
            this.tcSetting.Controls.Add(this.tpNSF);
            this.tcSetting.Controls.Add(this.tpSID);
            this.tcSetting.Controls.Add(this.tpPMDDotNET);
            this.tcSetting.Controls.Add(this.tpMIDIOut);
            this.tcSetting.Controls.Add(this.tpMIDIOut2);
            this.tcSetting.Controls.Add(this.tabMIDIExp);
            this.tcSetting.Controls.Add(this.tpMIDIKBD);
            this.tcSetting.Controls.Add(this.tpKeyBoard);
            this.tcSetting.Controls.Add(this.tpBalance);
            this.tcSetting.Controls.Add(this.tpPlayList);
            this.tcSetting.Controls.Add(this.tpOther);
            this.tcSetting.Controls.Add(this.tpOmake);
            this.tcSetting.Controls.Add(this.tpAbout);
            this.tcSetting.HotTrack = true;
            this.tcSetting.Multiline = true;
            this.tcSetting.Name = "tcSetting";
            this.tcSetting.SelectedIndex = 0;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.rbNullDevice);
            this.tpOutput.Controls.Add(this.label36);
            this.tpOutput.Controls.Add(this.lblWaitTime);
            this.tpOutput.Controls.Add(this.label66);
            this.tpOutput.Controls.Add(this.lblLatencyUnit);
            this.tpOutput.Controls.Add(this.label28);
            this.tpOutput.Controls.Add(this.label65);
            this.tpOutput.Controls.Add(this.lblLatency);
            this.tpOutput.Controls.Add(this.cmbWaitTime);
            this.tpOutput.Controls.Add(this.cmbSampleRate);
            this.tpOutput.Controls.Add(this.cmbLatency);
            this.tpOutput.Controls.Add(this.rbSPPCM);
            this.tpOutput.Controls.Add(this.rbDirectSoundOut);
            this.tpOutput.Controls.Add(this.rbWaveOut);
            this.tpOutput.Controls.Add(this.rbAsioOut);
            this.tpOutput.Controls.Add(this.gbWaveOut);
            this.tpOutput.Controls.Add(this.rbWasapiOut);
            this.tpOutput.Controls.Add(this.groupBox16);
            this.tpOutput.Controls.Add(this.gbAsioOut);
            this.tpOutput.Controls.Add(this.gbDirectSound);
            this.tpOutput.Controls.Add(this.gbWasapiOut);
            resources.ApplyResources(this.tpOutput, "tpOutput");
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // rbNullDevice
            // 
            resources.ApplyResources(this.rbNullDevice, "rbNullDevice");
            this.rbNullDevice.Name = "rbNullDevice";
            this.rbNullDevice.UseVisualStyleBackColor = true;
            this.rbNullDevice.CheckedChanged += new System.EventHandler(this.rbDirectSoundOut_CheckedChanged);
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // lblWaitTime
            // 
            resources.ApplyResources(this.lblWaitTime, "lblWaitTime");
            this.lblWaitTime.Name = "lblWaitTime";
            // 
            // label66
            // 
            resources.ApplyResources(this.label66, "label66");
            this.label66.Name = "label66";
            // 
            // lblLatencyUnit
            // 
            resources.ApplyResources(this.lblLatencyUnit, "lblLatencyUnit");
            this.lblLatencyUnit.Name = "lblLatencyUnit";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // label65
            // 
            resources.ApplyResources(this.label65, "label65");
            this.label65.Name = "label65";
            // 
            // lblLatency
            // 
            resources.ApplyResources(this.lblLatency, "lblLatency");
            this.lblLatency.Name = "lblLatency";
            // 
            // cmbWaitTime
            // 
            this.cmbWaitTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaitTime.FormattingEnabled = true;
            this.cmbWaitTime.Items.AddRange(new object[] {
            resources.GetString("cmbWaitTime.Items"),
            resources.GetString("cmbWaitTime.Items1"),
            resources.GetString("cmbWaitTime.Items2"),
            resources.GetString("cmbWaitTime.Items3"),
            resources.GetString("cmbWaitTime.Items4"),
            resources.GetString("cmbWaitTime.Items5"),
            resources.GetString("cmbWaitTime.Items6"),
            resources.GetString("cmbWaitTime.Items7"),
            resources.GetString("cmbWaitTime.Items8"),
            resources.GetString("cmbWaitTime.Items9"),
            resources.GetString("cmbWaitTime.Items10")});
            resources.ApplyResources(this.cmbWaitTime, "cmbWaitTime");
            this.cmbWaitTime.Name = "cmbWaitTime";
            // 
            // cmbSampleRate
            // 
            resources.ApplyResources(this.cmbSampleRate, "cmbSampleRate");
            this.cmbSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSampleRate.FormattingEnabled = true;
            this.cmbSampleRate.Items.AddRange(new object[] {
            resources.GetString("cmbSampleRate.Items"),
            resources.GetString("cmbSampleRate.Items1"),
            resources.GetString("cmbSampleRate.Items2"),
            resources.GetString("cmbSampleRate.Items3"),
            resources.GetString("cmbSampleRate.Items4"),
            resources.GetString("cmbSampleRate.Items5"),
            resources.GetString("cmbSampleRate.Items6")});
            this.cmbSampleRate.Name = "cmbSampleRate";
            // 
            // cmbLatency
            // 
            this.cmbLatency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLatency.FormattingEnabled = true;
            this.cmbLatency.Items.AddRange(new object[] {
            resources.GetString("cmbLatency.Items"),
            resources.GetString("cmbLatency.Items1"),
            resources.GetString("cmbLatency.Items2"),
            resources.GetString("cmbLatency.Items3"),
            resources.GetString("cmbLatency.Items4"),
            resources.GetString("cmbLatency.Items5"),
            resources.GetString("cmbLatency.Items6"),
            resources.GetString("cmbLatency.Items7")});
            resources.ApplyResources(this.cmbLatency, "cmbLatency");
            this.cmbLatency.Name = "cmbLatency";
            // 
            // rbSPPCM
            // 
            resources.ApplyResources(this.rbSPPCM, "rbSPPCM");
            this.rbSPPCM.Name = "rbSPPCM";
            this.rbSPPCM.UseVisualStyleBackColor = true;
            this.rbSPPCM.CheckedChanged += new System.EventHandler(this.rbDirectSoundOut_CheckedChanged);
            // 
            // groupBox16
            // 
            resources.ApplyResources(this.groupBox16, "groupBox16");
            this.groupBox16.Controls.Add(this.cmbSPPCMDevice);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.TabStop = false;
            // 
            // cmbSPPCMDevice
            // 
            resources.ApplyResources(this.cmbSPPCMDevice, "cmbSPPCMDevice");
            this.cmbSPPCMDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSPPCMDevice.FormattingEnabled = true;
            this.cmbSPPCMDevice.Name = "cmbSPPCMDevice";
            // 
            // tpModule
            // 
            this.tpModule.Controls.Add(this.groupBox1);
            this.tpModule.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tpModule, "tpModule");
            this.tpModule.Name = "tpModule";
            this.tpModule.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.cbUnuseRealChip);
            this.groupBox1.Controls.Add(this.ucSI);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cbUnuseRealChip
            // 
            resources.ApplyResources(this.cbUnuseRealChip, "cbUnuseRealChip");
            this.cbUnuseRealChip.Name = "cbUnuseRealChip";
            this.cbUnuseRealChip.UseVisualStyleBackColor = true;
            // 
            // ucSI
            // 
            resources.ApplyResources(this.ucSI, "ucSI");
            this.ucSI.Name = "ucSI";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.cbHiyorimiMode);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.tbLatencyEmu);
            this.groupBox3.Controls.Add(this.tbLatencySCCI);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cbHiyorimiMode
            // 
            resources.ApplyResources(this.cbHiyorimiMode, "cbHiyorimiMode");
            this.cbHiyorimiMode.Name = "cbHiyorimiMode";
            this.cbHiyorimiMode.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // tbLatencyEmu
            // 
            resources.ApplyResources(this.tbLatencyEmu, "tbLatencyEmu");
            this.tbLatencyEmu.Name = "tbLatencyEmu";
            // 
            // tbLatencySCCI
            // 
            resources.ApplyResources(this.tbLatencySCCI, "tbLatencySCCI");
            this.tbLatencySCCI.Name = "tbLatencySCCI";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // tpNuked
            // 
            this.tpNuked.Controls.Add(this.groupBox29);
            this.tpNuked.Controls.Add(this.groupBox26);
            resources.ApplyResources(this.tpNuked, "tpNuked");
            this.tpNuked.Name = "tpNuked";
            this.tpNuked.UseVisualStyleBackColor = true;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.cbGensSSGEG);
            this.groupBox29.Controls.Add(this.cbGensDACHPF);
            resources.ApplyResources(this.groupBox29, "groupBox29");
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.TabStop = false;
            // 
            // cbGensSSGEG
            // 
            resources.ApplyResources(this.cbGensSSGEG, "cbGensSSGEG");
            this.cbGensSSGEG.Name = "cbGensSSGEG";
            this.cbGensSSGEG.UseVisualStyleBackColor = true;
            // 
            // cbGensDACHPF
            // 
            resources.ApplyResources(this.cbGensDACHPF, "cbGensDACHPF");
            this.cbGensDACHPF.Name = "cbGensDACHPF";
            this.cbGensDACHPF.UseVisualStyleBackColor = true;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionYM2612u);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionYM2612);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionDiscrete);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionASIClp);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionASIC);
            resources.ApplyResources(this.groupBox26, "groupBox26");
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.TabStop = false;
            // 
            // rbNukedOPN2OptionYM2612u
            // 
            resources.ApplyResources(this.rbNukedOPN2OptionYM2612u, "rbNukedOPN2OptionYM2612u");
            this.rbNukedOPN2OptionYM2612u.Name = "rbNukedOPN2OptionYM2612u";
            this.rbNukedOPN2OptionYM2612u.TabStop = true;
            this.rbNukedOPN2OptionYM2612u.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionYM2612
            // 
            resources.ApplyResources(this.rbNukedOPN2OptionYM2612, "rbNukedOPN2OptionYM2612");
            this.rbNukedOPN2OptionYM2612.Name = "rbNukedOPN2OptionYM2612";
            this.rbNukedOPN2OptionYM2612.TabStop = true;
            this.rbNukedOPN2OptionYM2612.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionDiscrete
            // 
            resources.ApplyResources(this.rbNukedOPN2OptionDiscrete, "rbNukedOPN2OptionDiscrete");
            this.rbNukedOPN2OptionDiscrete.Name = "rbNukedOPN2OptionDiscrete";
            this.rbNukedOPN2OptionDiscrete.TabStop = true;
            this.rbNukedOPN2OptionDiscrete.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIClp
            // 
            resources.ApplyResources(this.rbNukedOPN2OptionASIClp, "rbNukedOPN2OptionASIClp");
            this.rbNukedOPN2OptionASIClp.Name = "rbNukedOPN2OptionASIClp";
            this.rbNukedOPN2OptionASIClp.TabStop = true;
            this.rbNukedOPN2OptionASIClp.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIC
            // 
            resources.ApplyResources(this.rbNukedOPN2OptionASIC, "rbNukedOPN2OptionASIC");
            this.rbNukedOPN2OptionASIC.Name = "rbNukedOPN2OptionASIC";
            this.rbNukedOPN2OptionASIC.TabStop = true;
            this.rbNukedOPN2OptionASIC.UseVisualStyleBackColor = true;
            // 
            // tpNSF
            // 
            this.tpNSF.Controls.Add(this.trkbNSFLPF);
            this.tpNSF.Controls.Add(this.label53);
            this.tpNSF.Controls.Add(this.label52);
            this.tpNSF.Controls.Add(this.trkbNSFHPF);
            this.tpNSF.Controls.Add(this.groupBox10);
            this.tpNSF.Controls.Add(this.groupBox12);
            this.tpNSF.Controls.Add(this.groupBox11);
            this.tpNSF.Controls.Add(this.groupBox9);
            this.tpNSF.Controls.Add(this.groupBox8);
            resources.ApplyResources(this.tpNSF, "tpNSF");
            this.tpNSF.Name = "tpNSF";
            this.tpNSF.UseVisualStyleBackColor = true;
            // 
            // trkbNSFLPF
            // 
            resources.ApplyResources(this.trkbNSFLPF, "trkbNSFLPF");
            this.trkbNSFLPF.Maximum = 400;
            this.trkbNSFLPF.Name = "trkbNSFLPF";
            this.trkbNSFLPF.TickFrequency = 10;
            // 
            // label53
            // 
            resources.ApplyResources(this.label53, "label53");
            this.label53.Name = "label53";
            // 
            // label52
            // 
            resources.ApplyResources(this.label52, "label52");
            this.label52.Name = "label52";
            // 
            // trkbNSFHPF
            // 
            resources.ApplyResources(this.trkbNSFHPF, "trkbNSFHPF");
            this.trkbNSFHPF.Maximum = 256;
            this.trkbNSFHPF.Name = "trkbNSFHPF";
            this.trkbNSFHPF.TickFrequency = 10;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.cbNSFDmc_DPCMReverse);
            this.groupBox10.Controls.Add(this.cbNSFDmc_RandomizeTri);
            this.groupBox10.Controls.Add(this.cbNSFDmc_TriMute);
            this.groupBox10.Controls.Add(this.cbNSFDmc_RandomizeNoise);
            this.groupBox10.Controls.Add(this.cbNSFDmc_DPCMAntiClick);
            this.groupBox10.Controls.Add(this.cbNSFDmc_EnablePNoise);
            this.groupBox10.Controls.Add(this.cbNSFDmc_Enable4011);
            this.groupBox10.Controls.Add(this.cbNSFDmc_NonLinearMixer);
            this.groupBox10.Controls.Add(this.cbNSFDmc_UnmuteOnReset);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // cbNSFDmc_DPCMReverse
            // 
            resources.ApplyResources(this.cbNSFDmc_DPCMReverse, "cbNSFDmc_DPCMReverse");
            this.cbNSFDmc_DPCMReverse.Name = "cbNSFDmc_DPCMReverse";
            this.cbNSFDmc_DPCMReverse.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_RandomizeTri
            // 
            resources.ApplyResources(this.cbNSFDmc_RandomizeTri, "cbNSFDmc_RandomizeTri");
            this.cbNSFDmc_RandomizeTri.Name = "cbNSFDmc_RandomizeTri";
            this.cbNSFDmc_RandomizeTri.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_TriMute
            // 
            resources.ApplyResources(this.cbNSFDmc_TriMute, "cbNSFDmc_TriMute");
            this.cbNSFDmc_TriMute.Name = "cbNSFDmc_TriMute";
            this.cbNSFDmc_TriMute.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_RandomizeNoise
            // 
            resources.ApplyResources(this.cbNSFDmc_RandomizeNoise, "cbNSFDmc_RandomizeNoise");
            this.cbNSFDmc_RandomizeNoise.Name = "cbNSFDmc_RandomizeNoise";
            this.cbNSFDmc_RandomizeNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_DPCMAntiClick
            // 
            resources.ApplyResources(this.cbNSFDmc_DPCMAntiClick, "cbNSFDmc_DPCMAntiClick");
            this.cbNSFDmc_DPCMAntiClick.Name = "cbNSFDmc_DPCMAntiClick";
            this.cbNSFDmc_DPCMAntiClick.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_EnablePNoise
            // 
            resources.ApplyResources(this.cbNSFDmc_EnablePNoise, "cbNSFDmc_EnablePNoise");
            this.cbNSFDmc_EnablePNoise.Name = "cbNSFDmc_EnablePNoise";
            this.cbNSFDmc_EnablePNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_Enable4011
            // 
            resources.ApplyResources(this.cbNSFDmc_Enable4011, "cbNSFDmc_Enable4011");
            this.cbNSFDmc_Enable4011.Name = "cbNSFDmc_Enable4011";
            this.cbNSFDmc_Enable4011.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_NonLinearMixer
            // 
            resources.ApplyResources(this.cbNSFDmc_NonLinearMixer, "cbNSFDmc_NonLinearMixer");
            this.cbNSFDmc_NonLinearMixer.Name = "cbNSFDmc_NonLinearMixer";
            this.cbNSFDmc_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_UnmuteOnReset
            // 
            resources.ApplyResources(this.cbNSFDmc_UnmuteOnReset, "cbNSFDmc_UnmuteOnReset");
            this.cbNSFDmc_UnmuteOnReset.Name = "cbNSFDmc_UnmuteOnReset";
            this.cbNSFDmc_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.cbNSFN160_Serial);
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            // 
            // cbNSFN160_Serial
            // 
            resources.ApplyResources(this.cbNSFN160_Serial, "cbNSFN160_Serial");
            this.cbNSFN160_Serial.Name = "cbNSFN160_Serial";
            this.cbNSFN160_Serial.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.cbNSFMmc5_PhaseRefresh);
            this.groupBox11.Controls.Add(this.cbNSFMmc5_NonLinearMixer);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // cbNSFMmc5_PhaseRefresh
            // 
            resources.ApplyResources(this.cbNSFMmc5_PhaseRefresh, "cbNSFMmc5_PhaseRefresh");
            this.cbNSFMmc5_PhaseRefresh.Name = "cbNSFMmc5_PhaseRefresh";
            this.cbNSFMmc5_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNSFMmc5_NonLinearMixer
            // 
            resources.ApplyResources(this.cbNSFMmc5_NonLinearMixer, "cbNSFMmc5_NonLinearMixer");
            this.cbNSFMmc5_NonLinearMixer.Name = "cbNSFMmc5_NonLinearMixer";
            this.cbNSFMmc5_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.cbNFSNes_DutySwap);
            this.groupBox9.Controls.Add(this.cbNFSNes_PhaseRefresh);
            this.groupBox9.Controls.Add(this.cbNFSNes_NonLinearMixer);
            this.groupBox9.Controls.Add(this.cbNFSNes_UnmuteOnReset);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // cbNFSNes_DutySwap
            // 
            resources.ApplyResources(this.cbNFSNes_DutySwap, "cbNFSNes_DutySwap");
            this.cbNFSNes_DutySwap.Name = "cbNFSNes_DutySwap";
            this.cbNFSNes_DutySwap.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_PhaseRefresh
            // 
            resources.ApplyResources(this.cbNFSNes_PhaseRefresh, "cbNFSNes_PhaseRefresh");
            this.cbNFSNes_PhaseRefresh.Name = "cbNFSNes_PhaseRefresh";
            this.cbNFSNes_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_NonLinearMixer
            // 
            resources.ApplyResources(this.cbNFSNes_NonLinearMixer, "cbNFSNes_NonLinearMixer");
            this.cbNFSNes_NonLinearMixer.Name = "cbNFSNes_NonLinearMixer";
            this.cbNFSNes_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_UnmuteOnReset
            // 
            resources.ApplyResources(this.cbNFSNes_UnmuteOnReset, "cbNFSNes_UnmuteOnReset");
            this.cbNFSNes_UnmuteOnReset.Name = "cbNFSNes_UnmuteOnReset";
            this.cbNFSNes_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label21);
            this.groupBox8.Controls.Add(this.label20);
            this.groupBox8.Controls.Add(this.tbNSFFds_LPF);
            this.groupBox8.Controls.Add(this.cbNFSFds_4085Reset);
            this.groupBox8.Controls.Add(this.cbNSFFDSWriteDisable8000);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // tbNSFFds_LPF
            // 
            resources.ApplyResources(this.tbNSFFds_LPF, "tbNSFFds_LPF");
            this.tbNSFFds_LPF.Name = "tbNSFFds_LPF";
            // 
            // cbNFSFds_4085Reset
            // 
            resources.ApplyResources(this.cbNFSFds_4085Reset, "cbNFSFds_4085Reset");
            this.cbNFSFds_4085Reset.Name = "cbNFSFds_4085Reset";
            this.cbNFSFds_4085Reset.UseVisualStyleBackColor = true;
            // 
            // cbNSFFDSWriteDisable8000
            // 
            resources.ApplyResources(this.cbNSFFDSWriteDisable8000, "cbNSFFDSWriteDisable8000");
            this.cbNSFFDSWriteDisable8000.Name = "cbNSFFDSWriteDisable8000";
            this.cbNSFFDSWriteDisable8000.UseVisualStyleBackColor = true;
            // 
            // tpSID
            // 
            this.tpSID.Controls.Add(this.groupBox28);
            this.tpSID.Controls.Add(this.groupBox27);
            this.tpSID.Controls.Add(this.groupBox14);
            this.tpSID.Controls.Add(this.groupBox13);
            this.tpSID.Controls.Add(this.tbSIDOutputBufferSize);
            this.tpSID.Controls.Add(this.label51);
            this.tpSID.Controls.Add(this.label49);
            resources.ApplyResources(this.tpSID, "tpSID");
            this.tpSID.Name = "tpSID";
            this.tpSID.UseVisualStyleBackColor = true;
            // 
            // groupBox28
            // 
            this.groupBox28.Controls.Add(this.cbSIDModel_Force);
            this.groupBox28.Controls.Add(this.rbSIDModel_8580);
            this.groupBox28.Controls.Add(this.rbSIDModel_6581);
            resources.ApplyResources(this.groupBox28, "groupBox28");
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.TabStop = false;
            // 
            // cbSIDModel_Force
            // 
            resources.ApplyResources(this.cbSIDModel_Force, "cbSIDModel_Force");
            this.cbSIDModel_Force.Name = "cbSIDModel_Force";
            this.cbSIDModel_Force.UseVisualStyleBackColor = true;
            // 
            // rbSIDModel_8580
            // 
            resources.ApplyResources(this.rbSIDModel_8580, "rbSIDModel_8580");
            this.rbSIDModel_8580.Name = "rbSIDModel_8580";
            this.rbSIDModel_8580.UseVisualStyleBackColor = true;
            // 
            // rbSIDModel_6581
            // 
            resources.ApplyResources(this.rbSIDModel_6581, "rbSIDModel_6581");
            this.rbSIDModel_6581.Checked = true;
            this.rbSIDModel_6581.Name = "rbSIDModel_6581";
            this.rbSIDModel_6581.TabStop = true;
            this.rbSIDModel_6581.UseVisualStyleBackColor = true;
            // 
            // groupBox27
            // 
            this.groupBox27.Controls.Add(this.cbSIDC64Model_Force);
            this.groupBox27.Controls.Add(this.rbSIDC64Model_DREAN);
            this.groupBox27.Controls.Add(this.rbSIDC64Model_OLDNTSC);
            this.groupBox27.Controls.Add(this.rbSIDC64Model_NTSC);
            this.groupBox27.Controls.Add(this.rbSIDC64Model_PAL);
            resources.ApplyResources(this.groupBox27, "groupBox27");
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.TabStop = false;
            // 
            // cbSIDC64Model_Force
            // 
            resources.ApplyResources(this.cbSIDC64Model_Force, "cbSIDC64Model_Force");
            this.cbSIDC64Model_Force.Name = "cbSIDC64Model_Force";
            this.cbSIDC64Model_Force.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_DREAN
            // 
            resources.ApplyResources(this.rbSIDC64Model_DREAN, "rbSIDC64Model_DREAN");
            this.rbSIDC64Model_DREAN.Name = "rbSIDC64Model_DREAN";
            this.rbSIDC64Model_DREAN.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_OLDNTSC
            // 
            resources.ApplyResources(this.rbSIDC64Model_OLDNTSC, "rbSIDC64Model_OLDNTSC");
            this.rbSIDC64Model_OLDNTSC.Name = "rbSIDC64Model_OLDNTSC";
            this.rbSIDC64Model_OLDNTSC.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_NTSC
            // 
            resources.ApplyResources(this.rbSIDC64Model_NTSC, "rbSIDC64Model_NTSC");
            this.rbSIDC64Model_NTSC.Name = "rbSIDC64Model_NTSC";
            this.rbSIDC64Model_NTSC.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_PAL
            // 
            resources.ApplyResources(this.rbSIDC64Model_PAL, "rbSIDC64Model_PAL");
            this.rbSIDC64Model_PAL.Checked = true;
            this.rbSIDC64Model_PAL.Name = "rbSIDC64Model_PAL";
            this.rbSIDC64Model_PAL.TabStop = true;
            this.rbSIDC64Model_PAL.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.label27);
            this.groupBox14.Controls.Add(this.label26);
            this.groupBox14.Controls.Add(this.label25);
            this.groupBox14.Controls.Add(this.rdSIDQ1);
            this.groupBox14.Controls.Add(this.rdSIDQ3);
            this.groupBox14.Controls.Add(this.rdSIDQ2);
            this.groupBox14.Controls.Add(this.rdSIDQ4);
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // rdSIDQ1
            // 
            resources.ApplyResources(this.rdSIDQ1, "rdSIDQ1");
            this.rdSIDQ1.Checked = true;
            this.rdSIDQ1.Name = "rdSIDQ1";
            this.rdSIDQ1.TabStop = true;
            this.rdSIDQ1.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ3
            // 
            resources.ApplyResources(this.rdSIDQ3, "rdSIDQ3");
            this.rdSIDQ3.Name = "rdSIDQ3";
            this.rdSIDQ3.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ2
            // 
            resources.ApplyResources(this.rdSIDQ2, "rdSIDQ2");
            this.rdSIDQ2.Name = "rdSIDQ2";
            this.rdSIDQ2.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ4
            // 
            resources.ApplyResources(this.rdSIDQ4, "rdSIDQ4");
            this.rdSIDQ4.Name = "rdSIDQ4";
            this.rdSIDQ4.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.btnSIDBasic);
            this.groupBox13.Controls.Add(this.btnSIDCharacter);
            this.groupBox13.Controls.Add(this.btnSIDKernal);
            this.groupBox13.Controls.Add(this.tbSIDCharacter);
            this.groupBox13.Controls.Add(this.tbSIDBasic);
            this.groupBox13.Controls.Add(this.tbSIDKernal);
            this.groupBox13.Controls.Add(this.label24);
            this.groupBox13.Controls.Add(this.label23);
            this.groupBox13.Controls.Add(this.label22);
            resources.ApplyResources(this.groupBox13, "groupBox13");
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.TabStop = false;
            // 
            // btnSIDBasic
            // 
            resources.ApplyResources(this.btnSIDBasic, "btnSIDBasic");
            this.btnSIDBasic.Name = "btnSIDBasic";
            this.btnSIDBasic.UseVisualStyleBackColor = true;
            this.btnSIDBasic.Click += new System.EventHandler(this.btnSIDBasic_Click);
            // 
            // btnSIDCharacter
            // 
            resources.ApplyResources(this.btnSIDCharacter, "btnSIDCharacter");
            this.btnSIDCharacter.Name = "btnSIDCharacter";
            this.btnSIDCharacter.UseVisualStyleBackColor = true;
            this.btnSIDCharacter.Click += new System.EventHandler(this.btnSIDCharacter_Click);
            // 
            // btnSIDKernal
            // 
            resources.ApplyResources(this.btnSIDKernal, "btnSIDKernal");
            this.btnSIDKernal.Name = "btnSIDKernal";
            this.btnSIDKernal.UseVisualStyleBackColor = true;
            this.btnSIDKernal.Click += new System.EventHandler(this.btnSIDKernal_Click);
            // 
            // tbSIDCharacter
            // 
            resources.ApplyResources(this.tbSIDCharacter, "tbSIDCharacter");
            this.tbSIDCharacter.Name = "tbSIDCharacter";
            // 
            // tbSIDBasic
            // 
            resources.ApplyResources(this.tbSIDBasic, "tbSIDBasic");
            this.tbSIDBasic.Name = "tbSIDBasic";
            // 
            // tbSIDKernal
            // 
            resources.ApplyResources(this.tbSIDKernal, "tbSIDKernal");
            this.tbSIDKernal.Name = "tbSIDKernal";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // tbSIDOutputBufferSize
            // 
            resources.ApplyResources(this.tbSIDOutputBufferSize, "tbSIDOutputBufferSize");
            this.tbSIDOutputBufferSize.Name = "tbSIDOutputBufferSize";
            // 
            // label51
            // 
            resources.ApplyResources(this.label51, "label51");
            this.label51.Name = "label51";
            // 
            // label49
            // 
            resources.ApplyResources(this.label49, "label49");
            this.label49.Name = "label49";
            // 
            // tpPMDDotNET
            // 
            this.tpPMDDotNET.Controls.Add(this.rbPMDManual);
            this.tpPMDDotNET.Controls.Add(this.rbPMDAuto);
            this.tpPMDDotNET.Controls.Add(this.btnPMDResetDriverArguments);
            this.tpPMDDotNET.Controls.Add(this.label54);
            this.tpPMDDotNET.Controls.Add(this.btnPMDResetCompilerArhguments);
            this.tpPMDDotNET.Controls.Add(this.tbPMDDriverArguments);
            this.tpPMDDotNET.Controls.Add(this.label55);
            this.tpPMDDotNET.Controls.Add(this.tbPMDCompilerArguments);
            this.tpPMDDotNET.Controls.Add(this.gbPMDManual);
            resources.ApplyResources(this.tpPMDDotNET, "tpPMDDotNET");
            this.tpPMDDotNET.Name = "tpPMDDotNET";
            this.tpPMDDotNET.UseVisualStyleBackColor = true;
            // 
            // rbPMDManual
            // 
            resources.ApplyResources(this.rbPMDManual, "rbPMDManual");
            this.rbPMDManual.Name = "rbPMDManual";
            this.rbPMDManual.TabStop = true;
            this.rbPMDManual.UseVisualStyleBackColor = true;
            this.rbPMDManual.CheckedChanged += new System.EventHandler(this.rbPMDManual_CheckedChanged);
            // 
            // rbPMDAuto
            // 
            resources.ApplyResources(this.rbPMDAuto, "rbPMDAuto");
            this.rbPMDAuto.Name = "rbPMDAuto";
            this.rbPMDAuto.TabStop = true;
            this.rbPMDAuto.UseVisualStyleBackColor = true;
            // 
            // btnPMDResetDriverArguments
            // 
            resources.ApplyResources(this.btnPMDResetDriverArguments, "btnPMDResetDriverArguments");
            this.btnPMDResetDriverArguments.Name = "btnPMDResetDriverArguments";
            this.btnPMDResetDriverArguments.UseVisualStyleBackColor = true;
            this.btnPMDResetDriverArguments.Click += new System.EventHandler(this.btnPMDResetDriverArguments_Click);
            // 
            // label54
            // 
            resources.ApplyResources(this.label54, "label54");
            this.label54.Name = "label54";
            // 
            // btnPMDResetCompilerArhguments
            // 
            resources.ApplyResources(this.btnPMDResetCompilerArhguments, "btnPMDResetCompilerArhguments");
            this.btnPMDResetCompilerArhguments.Name = "btnPMDResetCompilerArhguments";
            this.btnPMDResetCompilerArhguments.UseVisualStyleBackColor = true;
            this.btnPMDResetCompilerArhguments.Click += new System.EventHandler(this.btnPMDResetCompilerArhguments_Click);
            // 
            // tbPMDDriverArguments
            // 
            resources.ApplyResources(this.tbPMDDriverArguments, "tbPMDDriverArguments");
            this.tbPMDDriverArguments.Name = "tbPMDDriverArguments";
            // 
            // label55
            // 
            resources.ApplyResources(this.label55, "label55");
            this.label55.Name = "label55";
            // 
            // tbPMDCompilerArguments
            // 
            resources.ApplyResources(this.tbPMDCompilerArguments, "tbPMDCompilerArguments");
            this.tbPMDCompilerArguments.Name = "tbPMDCompilerArguments";
            // 
            // gbPMDManual
            // 
            resources.ApplyResources(this.gbPMDManual, "gbPMDManual");
            this.gbPMDManual.Controls.Add(this.cbPMDSetManualVolume);
            this.gbPMDManual.Controls.Add(this.cbPMDUsePPZ8);
            this.gbPMDManual.Controls.Add(this.groupBox32);
            this.gbPMDManual.Controls.Add(this.cbPMDUsePPSDRV);
            this.gbPMDManual.Controls.Add(this.gbPPSDRV);
            this.gbPMDManual.Controls.Add(this.gbPMDSetManualVolume);
            this.gbPMDManual.Name = "gbPMDManual";
            this.gbPMDManual.TabStop = false;
            // 
            // cbPMDSetManualVolume
            // 
            resources.ApplyResources(this.cbPMDSetManualVolume, "cbPMDSetManualVolume");
            this.cbPMDSetManualVolume.Name = "cbPMDSetManualVolume";
            this.cbPMDSetManualVolume.UseVisualStyleBackColor = true;
            this.cbPMDSetManualVolume.CheckedChanged += new System.EventHandler(this.cbPMDSetManualVolume_CheckedChanged);
            // 
            // cbPMDUsePPZ8
            // 
            resources.ApplyResources(this.cbPMDUsePPZ8, "cbPMDUsePPZ8");
            this.cbPMDUsePPZ8.Name = "cbPMDUsePPZ8";
            this.cbPMDUsePPZ8.UseVisualStyleBackColor = true;
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.rbPMD86B);
            this.groupBox32.Controls.Add(this.rbPMDSpbB);
            this.groupBox32.Controls.Add(this.rbPMDNrmB);
            resources.ApplyResources(this.groupBox32, "groupBox32");
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.TabStop = false;
            // 
            // rbPMD86B
            // 
            resources.ApplyResources(this.rbPMD86B, "rbPMD86B");
            this.rbPMD86B.Name = "rbPMD86B";
            this.rbPMD86B.TabStop = true;
            this.rbPMD86B.UseVisualStyleBackColor = true;
            // 
            // rbPMDSpbB
            // 
            resources.ApplyResources(this.rbPMDSpbB, "rbPMDSpbB");
            this.rbPMDSpbB.Name = "rbPMDSpbB";
            this.rbPMDSpbB.TabStop = true;
            this.rbPMDSpbB.UseVisualStyleBackColor = true;
            // 
            // rbPMDNrmB
            // 
            resources.ApplyResources(this.rbPMDNrmB, "rbPMDNrmB");
            this.rbPMDNrmB.Name = "rbPMDNrmB";
            this.rbPMDNrmB.TabStop = true;
            this.rbPMDNrmB.UseVisualStyleBackColor = true;
            // 
            // cbPMDUsePPSDRV
            // 
            resources.ApplyResources(this.cbPMDUsePPSDRV, "cbPMDUsePPSDRV");
            this.cbPMDUsePPSDRV.Name = "cbPMDUsePPSDRV";
            this.cbPMDUsePPSDRV.UseVisualStyleBackColor = true;
            this.cbPMDUsePPSDRV.CheckedChanged += new System.EventHandler(this.cbPMDUsePPSDRV_CheckedChanged);
            // 
            // gbPPSDRV
            // 
            resources.ApplyResources(this.gbPPSDRV, "gbPPSDRV");
            this.gbPPSDRV.Controls.Add(this.groupBox33);
            this.gbPPSDRV.Name = "gbPPSDRV";
            this.gbPPSDRV.TabStop = false;
            // 
            // groupBox33
            // 
            resources.ApplyResources(this.groupBox33, "groupBox33");
            this.groupBox33.Controls.Add(this.rbPMDUsePPSDRVManualFreq);
            this.groupBox33.Controls.Add(this.label56);
            this.groupBox33.Controls.Add(this.rbPMDUsePPSDRVFreqDefault);
            this.groupBox33.Controls.Add(this.btnPMDPPSDRVManualWait);
            this.groupBox33.Controls.Add(this.label57);
            this.groupBox33.Controls.Add(this.tbPMDPPSDRVFreq);
            this.groupBox33.Controls.Add(this.label58);
            this.groupBox33.Controls.Add(this.tbPMDPPSDRVManualWait);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.TabStop = false;
            // 
            // rbPMDUsePPSDRVManualFreq
            // 
            resources.ApplyResources(this.rbPMDUsePPSDRVManualFreq, "rbPMDUsePPSDRVManualFreq");
            this.rbPMDUsePPSDRVManualFreq.Name = "rbPMDUsePPSDRVManualFreq";
            this.rbPMDUsePPSDRVManualFreq.TabStop = true;
            this.rbPMDUsePPSDRVManualFreq.UseVisualStyleBackColor = true;
            this.rbPMDUsePPSDRVManualFreq.CheckedChanged += new System.EventHandler(this.rbPMDUsePPSDRVManualFreq_CheckedChanged);
            // 
            // label56
            // 
            resources.ApplyResources(this.label56, "label56");
            this.label56.Name = "label56";
            // 
            // rbPMDUsePPSDRVFreqDefault
            // 
            resources.ApplyResources(this.rbPMDUsePPSDRVFreqDefault, "rbPMDUsePPSDRVFreqDefault");
            this.rbPMDUsePPSDRVFreqDefault.Name = "rbPMDUsePPSDRVFreqDefault";
            this.rbPMDUsePPSDRVFreqDefault.TabStop = true;
            this.rbPMDUsePPSDRVFreqDefault.UseVisualStyleBackColor = true;
            // 
            // btnPMDPPSDRVManualWait
            // 
            resources.ApplyResources(this.btnPMDPPSDRVManualWait, "btnPMDPPSDRVManualWait");
            this.btnPMDPPSDRVManualWait.Name = "btnPMDPPSDRVManualWait";
            this.btnPMDPPSDRVManualWait.UseVisualStyleBackColor = true;
            this.btnPMDPPSDRVManualWait.Click += new System.EventHandler(this.btnPMDPPSDRVManualWait_Click);
            // 
            // label57
            // 
            resources.ApplyResources(this.label57, "label57");
            this.label57.Name = "label57";
            // 
            // tbPMDPPSDRVFreq
            // 
            resources.ApplyResources(this.tbPMDPPSDRVFreq, "tbPMDPPSDRVFreq");
            this.tbPMDPPSDRVFreq.Name = "tbPMDPPSDRVFreq";
            this.tbPMDPPSDRVFreq.Click += new System.EventHandler(this.tbPMDPPSDRVFreq_Click);
            this.tbPMDPPSDRVFreq.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbPMDPPSDRVFreq_MouseClick);
            // 
            // label58
            // 
            resources.ApplyResources(this.label58, "label58");
            this.label58.Name = "label58";
            // 
            // tbPMDPPSDRVManualWait
            // 
            resources.ApplyResources(this.tbPMDPPSDRVManualWait, "tbPMDPPSDRVManualWait");
            this.tbPMDPPSDRVManualWait.Name = "tbPMDPPSDRVManualWait";
            // 
            // gbPMDSetManualVolume
            // 
            resources.ApplyResources(this.gbPMDSetManualVolume, "gbPMDSetManualVolume");
            this.gbPMDSetManualVolume.Controls.Add(this.label59);
            this.gbPMDSetManualVolume.Controls.Add(this.label60);
            this.gbPMDSetManualVolume.Controls.Add(this.tbPMDVolumeAdpcm);
            this.gbPMDSetManualVolume.Controls.Add(this.label61);
            this.gbPMDSetManualVolume.Controls.Add(this.tbPMDVolumeRhythm);
            this.gbPMDSetManualVolume.Controls.Add(this.label62);
            this.gbPMDSetManualVolume.Controls.Add(this.tbPMDVolumeSSG);
            this.gbPMDSetManualVolume.Controls.Add(this.label63);
            this.gbPMDSetManualVolume.Controls.Add(this.tbPMDVolumeGIMICSSG);
            this.gbPMDSetManualVolume.Controls.Add(this.label64);
            this.gbPMDSetManualVolume.Controls.Add(this.tbPMDVolumeFM);
            this.gbPMDSetManualVolume.Name = "gbPMDSetManualVolume";
            this.gbPMDSetManualVolume.TabStop = false;
            // 
            // label59
            // 
            resources.ApplyResources(this.label59, "label59");
            this.label59.Name = "label59";
            // 
            // label60
            // 
            resources.ApplyResources(this.label60, "label60");
            this.label60.Name = "label60";
            // 
            // tbPMDVolumeAdpcm
            // 
            resources.ApplyResources(this.tbPMDVolumeAdpcm, "tbPMDVolumeAdpcm");
            this.tbPMDVolumeAdpcm.Name = "tbPMDVolumeAdpcm";
            // 
            // label61
            // 
            resources.ApplyResources(this.label61, "label61");
            this.label61.Name = "label61";
            // 
            // tbPMDVolumeRhythm
            // 
            resources.ApplyResources(this.tbPMDVolumeRhythm, "tbPMDVolumeRhythm");
            this.tbPMDVolumeRhythm.Name = "tbPMDVolumeRhythm";
            // 
            // label62
            // 
            resources.ApplyResources(this.label62, "label62");
            this.label62.Name = "label62";
            // 
            // tbPMDVolumeSSG
            // 
            resources.ApplyResources(this.tbPMDVolumeSSG, "tbPMDVolumeSSG");
            this.tbPMDVolumeSSG.Name = "tbPMDVolumeSSG";
            // 
            // label63
            // 
            resources.ApplyResources(this.label63, "label63");
            this.label63.Name = "label63";
            // 
            // tbPMDVolumeGIMICSSG
            // 
            resources.ApplyResources(this.tbPMDVolumeGIMICSSG, "tbPMDVolumeGIMICSSG");
            this.tbPMDVolumeGIMICSSG.Name = "tbPMDVolumeGIMICSSG";
            // 
            // label64
            // 
            resources.ApplyResources(this.label64, "label64");
            this.label64.Name = "label64";
            // 
            // tbPMDVolumeFM
            // 
            resources.ApplyResources(this.tbPMDVolumeFM, "tbPMDVolumeFM");
            this.tbPMDVolumeFM.Name = "tbPMDVolumeFM";
            // 
            // tpMIDIOut
            // 
            this.tpMIDIOut.Controls.Add(this.btnAddVST);
            this.tpMIDIOut.Controls.Add(this.tbcMIDIoutList);
            this.tpMIDIOut.Controls.Add(this.btnSubMIDIout);
            this.tpMIDIOut.Controls.Add(this.btnAddMIDIout);
            this.tpMIDIOut.Controls.Add(this.label18);
            this.tpMIDIOut.Controls.Add(this.dgvMIDIoutPallet);
            this.tpMIDIOut.Controls.Add(this.label16);
            resources.ApplyResources(this.tpMIDIOut, "tpMIDIOut");
            this.tpMIDIOut.Name = "tpMIDIOut";
            this.tpMIDIOut.UseVisualStyleBackColor = true;
            // 
            // btnAddVST
            // 
            resources.ApplyResources(this.btnAddVST, "btnAddVST");
            this.btnAddVST.Name = "btnAddVST";
            this.btnAddVST.UseVisualStyleBackColor = true;
            this.btnAddVST.Click += new System.EventHandler(this.btnAddVST_Click);
            // 
            // tbcMIDIoutList
            // 
            resources.ApplyResources(this.tbcMIDIoutList, "tbcMIDIoutList");
            this.tbcMIDIoutList.Controls.Add(this.tabPage1);
            this.tbcMIDIoutList.Controls.Add(this.tabPage2);
            this.tbcMIDIoutList.Controls.Add(this.tabPage3);
            this.tbcMIDIoutList.Controls.Add(this.tabPage4);
            this.tbcMIDIoutList.Controls.Add(this.tabPage5);
            this.tbcMIDIoutList.Controls.Add(this.tabPage6);
            this.tbcMIDIoutList.Controls.Add(this.tabPage7);
            this.tbcMIDIoutList.Controls.Add(this.tabPage8);
            this.tbcMIDIoutList.Controls.Add(this.tabPage9);
            this.tbcMIDIoutList.Controls.Add(this.tabPage10);
            this.tbcMIDIoutList.Name = "tbcMIDIoutList";
            this.tbcMIDIoutList.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvMIDIoutListA);
            this.tabPage1.Controls.Add(this.btnUP_A);
            this.tabPage1.Controls.Add(this.btnDOWN_A);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Tag = "0";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListA
            // 
            this.dgvMIDIoutListA.AllowUserToAddRows = false;
            this.dgvMIDIoutListA.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListA.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dgvMIDIoutListA, "dgvMIDIoutListA");
            this.dgvMIDIoutListA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.clmIsVST,
            this.clmFileName,
            this.dataGridViewTextBoxColumn2,
            this.clmType,
            this.ClmBeforeSend,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dgvMIDIoutListA.MultiSelect = false;
            this.dgvMIDIoutListA.Name = "dgvMIDIoutListA";
            this.dgvMIDIoutListA.RowHeadersVisible = false;
            this.dgvMIDIoutListA.RowTemplate.Height = 21;
            this.dgvMIDIoutListA.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Frozen = true;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmIsVST
            // 
            resources.ApplyResources(this.clmIsVST, "clmIsVST");
            this.clmIsVST.Name = "clmIsVST";
            // 
            // clmFileName
            // 
            resources.ApplyResources(this.clmFileName, "clmFileName");
            this.clmFileName.Name = "clmFileName";
            this.clmFileName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmType
            // 
            resources.ApplyResources(this.clmType, "clmType");
            this.clmType.Items.AddRange(new object[] {
            "GM",
            "XG",
            "GS",
            "LA",
            "GS(SC-55_1)",
            "GS(SC-55_2)"});
            this.clmType.Name = "clmType";
            this.clmType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ClmBeforeSend
            // 
            resources.ApplyResources(this.ClmBeforeSend, "ClmBeforeSend");
            this.ClmBeforeSend.Items.AddRange(new object[] {
            "None",
            "GM Reset",
            "XG Reset",
            "GS Reset",
            "Custom"});
            this.ClmBeforeSend.Name = "ClmBeforeSend";
            this.ClmBeforeSend.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ClmBeforeSend.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn3
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnUP_A
            // 
            resources.ApplyResources(this.btnUP_A, "btnUP_A");
            this.btnUP_A.Name = "btnUP_A";
            this.btnUP_A.UseVisualStyleBackColor = true;
            this.btnUP_A.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_A
            // 
            resources.ApplyResources(this.btnDOWN_A, "btnDOWN_A");
            this.btnDOWN_A.Name = "btnDOWN_A";
            this.btnDOWN_A.UseVisualStyleBackColor = true;
            this.btnDOWN_A.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvMIDIoutListB);
            this.tabPage2.Controls.Add(this.btnUP_B);
            this.tabPage2.Controls.Add(this.btnDOWN_B);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Tag = "1";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListB
            // 
            this.dgvMIDIoutListB.AllowUserToAddRows = false;
            this.dgvMIDIoutListB.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListB.AllowUserToResizeRows = false;
            this.dgvMIDIoutListB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListB, "dgvMIDIoutListB");
            this.dgvMIDIoutListB.MultiSelect = false;
            this.dgvMIDIoutListB.Name = "dgvMIDIoutListB";
            this.dgvMIDIoutListB.RowHeadersVisible = false;
            this.dgvMIDIoutListB.RowTemplate.Height = 21;
            this.dgvMIDIoutListB.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_B
            // 
            resources.ApplyResources(this.btnUP_B, "btnUP_B");
            this.btnUP_B.Name = "btnUP_B";
            this.btnUP_B.UseVisualStyleBackColor = true;
            this.btnUP_B.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_B
            // 
            resources.ApplyResources(this.btnDOWN_B, "btnDOWN_B");
            this.btnDOWN_B.Name = "btnDOWN_B";
            this.btnDOWN_B.UseVisualStyleBackColor = true;
            this.btnDOWN_B.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvMIDIoutListC);
            this.tabPage3.Controls.Add(this.btnUP_C);
            this.tabPage3.Controls.Add(this.btnDOWN_C);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Tag = "2";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListC
            // 
            this.dgvMIDIoutListC.AllowUserToAddRows = false;
            this.dgvMIDIoutListC.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListC.AllowUserToResizeRows = false;
            this.dgvMIDIoutListC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListC, "dgvMIDIoutListC");
            this.dgvMIDIoutListC.MultiSelect = false;
            this.dgvMIDIoutListC.Name = "dgvMIDIoutListC";
            this.dgvMIDIoutListC.RowHeadersVisible = false;
            this.dgvMIDIoutListC.RowTemplate.Height = 21;
            this.dgvMIDIoutListC.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_C
            // 
            resources.ApplyResources(this.btnUP_C, "btnUP_C");
            this.btnUP_C.Name = "btnUP_C";
            this.btnUP_C.UseVisualStyleBackColor = true;
            this.btnUP_C.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_C
            // 
            resources.ApplyResources(this.btnDOWN_C, "btnDOWN_C");
            this.btnDOWN_C.Name = "btnDOWN_C";
            this.btnDOWN_C.UseVisualStyleBackColor = true;
            this.btnDOWN_C.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dgvMIDIoutListD);
            this.tabPage4.Controls.Add(this.btnUP_D);
            this.tabPage4.Controls.Add(this.btnDOWN_D);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Tag = "3";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListD
            // 
            this.dgvMIDIoutListD.AllowUserToAddRows = false;
            this.dgvMIDIoutListD.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListD.AllowUserToResizeRows = false;
            this.dgvMIDIoutListD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListD, "dgvMIDIoutListD");
            this.dgvMIDIoutListD.MultiSelect = false;
            this.dgvMIDIoutListD.Name = "dgvMIDIoutListD";
            this.dgvMIDIoutListD.RowHeadersVisible = false;
            this.dgvMIDIoutListD.RowTemplate.Height = 21;
            this.dgvMIDIoutListD.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_D
            // 
            resources.ApplyResources(this.btnUP_D, "btnUP_D");
            this.btnUP_D.Name = "btnUP_D";
            this.btnUP_D.UseVisualStyleBackColor = true;
            this.btnUP_D.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_D
            // 
            resources.ApplyResources(this.btnDOWN_D, "btnDOWN_D");
            this.btnDOWN_D.Name = "btnDOWN_D";
            this.btnDOWN_D.UseVisualStyleBackColor = true;
            this.btnDOWN_D.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.dgvMIDIoutListE);
            this.tabPage5.Controls.Add(this.btnUP_E);
            this.tabPage5.Controls.Add(this.btnDOWN_E);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Tag = "4";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListE
            // 
            this.dgvMIDIoutListE.AllowUserToAddRows = false;
            this.dgvMIDIoutListE.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListE.AllowUserToResizeRows = false;
            this.dgvMIDIoutListE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListE, "dgvMIDIoutListE");
            this.dgvMIDIoutListE.MultiSelect = false;
            this.dgvMIDIoutListE.Name = "dgvMIDIoutListE";
            this.dgvMIDIoutListE.RowHeadersVisible = false;
            this.dgvMIDIoutListE.RowTemplate.Height = 21;
            this.dgvMIDIoutListE.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_E
            // 
            resources.ApplyResources(this.btnUP_E, "btnUP_E");
            this.btnUP_E.Name = "btnUP_E";
            this.btnUP_E.UseVisualStyleBackColor = true;
            this.btnUP_E.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_E
            // 
            resources.ApplyResources(this.btnDOWN_E, "btnDOWN_E");
            this.btnDOWN_E.Name = "btnDOWN_E";
            this.btnDOWN_E.UseVisualStyleBackColor = true;
            this.btnDOWN_E.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.dgvMIDIoutListF);
            this.tabPage6.Controls.Add(this.btnUP_F);
            this.tabPage6.Controls.Add(this.btnDOWN_F);
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Tag = "5";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListF
            // 
            this.dgvMIDIoutListF.AllowUserToAddRows = false;
            this.dgvMIDIoutListF.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListF.AllowUserToResizeRows = false;
            this.dgvMIDIoutListF.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListF, "dgvMIDIoutListF");
            this.dgvMIDIoutListF.MultiSelect = false;
            this.dgvMIDIoutListF.Name = "dgvMIDIoutListF";
            this.dgvMIDIoutListF.RowHeadersVisible = false;
            this.dgvMIDIoutListF.RowTemplate.Height = 21;
            this.dgvMIDIoutListF.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_F
            // 
            resources.ApplyResources(this.btnUP_F, "btnUP_F");
            this.btnUP_F.Name = "btnUP_F";
            this.btnUP_F.UseVisualStyleBackColor = true;
            this.btnUP_F.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_F
            // 
            resources.ApplyResources(this.btnDOWN_F, "btnDOWN_F");
            this.btnDOWN_F.Name = "btnDOWN_F";
            this.btnDOWN_F.UseVisualStyleBackColor = true;
            this.btnDOWN_F.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dgvMIDIoutListG);
            this.tabPage7.Controls.Add(this.btnUP_G);
            this.tabPage7.Controls.Add(this.btnDOWN_G);
            resources.ApplyResources(this.tabPage7, "tabPage7");
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Tag = "6";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListG
            // 
            this.dgvMIDIoutListG.AllowUserToAddRows = false;
            this.dgvMIDIoutListG.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListG.AllowUserToResizeRows = false;
            this.dgvMIDIoutListG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListG, "dgvMIDIoutListG");
            this.dgvMIDIoutListG.MultiSelect = false;
            this.dgvMIDIoutListG.Name = "dgvMIDIoutListG";
            this.dgvMIDIoutListG.RowHeadersVisible = false;
            this.dgvMIDIoutListG.RowTemplate.Height = 21;
            this.dgvMIDIoutListG.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_G
            // 
            resources.ApplyResources(this.btnUP_G, "btnUP_G");
            this.btnUP_G.Name = "btnUP_G";
            this.btnUP_G.UseVisualStyleBackColor = true;
            this.btnUP_G.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_G
            // 
            resources.ApplyResources(this.btnDOWN_G, "btnDOWN_G");
            this.btnDOWN_G.Name = "btnDOWN_G";
            this.btnDOWN_G.UseVisualStyleBackColor = true;
            this.btnDOWN_G.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.dgvMIDIoutListH);
            this.tabPage8.Controls.Add(this.btnUP_H);
            this.tabPage8.Controls.Add(this.btnDOWN_H);
            resources.ApplyResources(this.tabPage8, "tabPage8");
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Tag = "7";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListH
            // 
            this.dgvMIDIoutListH.AllowUserToAddRows = false;
            this.dgvMIDIoutListH.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListH.AllowUserToResizeRows = false;
            this.dgvMIDIoutListH.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListH, "dgvMIDIoutListH");
            this.dgvMIDIoutListH.MultiSelect = false;
            this.dgvMIDIoutListH.Name = "dgvMIDIoutListH";
            this.dgvMIDIoutListH.RowHeadersVisible = false;
            this.dgvMIDIoutListH.RowTemplate.Height = 21;
            this.dgvMIDIoutListH.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_H
            // 
            resources.ApplyResources(this.btnUP_H, "btnUP_H");
            this.btnUP_H.Name = "btnUP_H";
            this.btnUP_H.UseVisualStyleBackColor = true;
            this.btnUP_H.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_H
            // 
            resources.ApplyResources(this.btnDOWN_H, "btnDOWN_H");
            this.btnDOWN_H.Name = "btnDOWN_H";
            this.btnDOWN_H.UseVisualStyleBackColor = true;
            this.btnDOWN_H.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.dgvMIDIoutListI);
            this.tabPage9.Controls.Add(this.btnUP_I);
            this.tabPage9.Controls.Add(this.btnDOWN_I);
            resources.ApplyResources(this.tabPage9, "tabPage9");
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Tag = "8";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListI
            // 
            this.dgvMIDIoutListI.AllowUserToAddRows = false;
            this.dgvMIDIoutListI.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListI.AllowUserToResizeRows = false;
            this.dgvMIDIoutListI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListI, "dgvMIDIoutListI");
            this.dgvMIDIoutListI.MultiSelect = false;
            this.dgvMIDIoutListI.Name = "dgvMIDIoutListI";
            this.dgvMIDIoutListI.RowHeadersVisible = false;
            this.dgvMIDIoutListI.RowTemplate.Height = 21;
            this.dgvMIDIoutListI.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_I
            // 
            resources.ApplyResources(this.btnUP_I, "btnUP_I");
            this.btnUP_I.Name = "btnUP_I";
            this.btnUP_I.UseVisualStyleBackColor = true;
            this.btnUP_I.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_I
            // 
            resources.ApplyResources(this.btnDOWN_I, "btnDOWN_I");
            this.btnDOWN_I.Name = "btnDOWN_I";
            this.btnDOWN_I.UseVisualStyleBackColor = true;
            this.btnDOWN_I.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.dgvMIDIoutListJ);
            this.tabPage10.Controls.Add(this.button17);
            this.tabPage10.Controls.Add(this.btnDOWN_J);
            resources.ApplyResources(this.tabPage10, "tabPage10");
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Tag = "9";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListJ
            // 
            this.dgvMIDIoutListJ.AllowUserToAddRows = false;
            this.dgvMIDIoutListJ.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListJ.AllowUserToResizeRows = false;
            this.dgvMIDIoutListJ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvMIDIoutListJ, "dgvMIDIoutListJ");
            this.dgvMIDIoutListJ.MultiSelect = false;
            this.dgvMIDIoutListJ.Name = "dgvMIDIoutListJ";
            this.dgvMIDIoutListJ.RowHeadersVisible = false;
            this.dgvMIDIoutListJ.RowTemplate.Height = 21;
            this.dgvMIDIoutListJ.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // button17
            // 
            resources.ApplyResources(this.button17, "button17");
            this.button17.Name = "button17";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_J
            // 
            resources.ApplyResources(this.btnDOWN_J, "btnDOWN_J");
            this.btnDOWN_J.Name = "btnDOWN_J";
            this.btnDOWN_J.UseVisualStyleBackColor = true;
            this.btnDOWN_J.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // btnSubMIDIout
            // 
            resources.ApplyResources(this.btnSubMIDIout, "btnSubMIDIout");
            this.btnSubMIDIout.Name = "btnSubMIDIout";
            this.btnSubMIDIout.UseVisualStyleBackColor = true;
            this.btnSubMIDIout.Click += new System.EventHandler(this.btnSubMIDIout_Click);
            // 
            // btnAddMIDIout
            // 
            resources.ApplyResources(this.btnAddMIDIout, "btnAddMIDIout");
            this.btnAddMIDIout.Name = "btnAddMIDIout";
            this.btnAddMIDIout.UseVisualStyleBackColor = true;
            this.btnAddMIDIout.Click += new System.EventHandler(this.btnAddMIDIout_Click);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // dgvMIDIoutPallet
            // 
            this.dgvMIDIoutPallet.AllowUserToAddRows = false;
            this.dgvMIDIoutPallet.AllowUserToDeleteRows = false;
            this.dgvMIDIoutPallet.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dgvMIDIoutPallet, "dgvMIDIoutPallet");
            this.dgvMIDIoutPallet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutPallet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmID,
            this.clmDeviceName,
            this.clmManufacturer,
            this.clmSpacer});
            this.dgvMIDIoutPallet.MultiSelect = false;
            this.dgvMIDIoutPallet.Name = "dgvMIDIoutPallet";
            this.dgvMIDIoutPallet.RowHeadersVisible = false;
            this.dgvMIDIoutPallet.RowTemplate.Height = 21;
            this.dgvMIDIoutPallet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // clmID
            // 
            this.clmID.Frozen = true;
            resources.ApplyResources(this.clmID, "clmID");
            this.clmID.Name = "clmID";
            this.clmID.ReadOnly = true;
            // 
            // clmDeviceName
            // 
            this.clmDeviceName.Frozen = true;
            resources.ApplyResources(this.clmDeviceName, "clmDeviceName");
            this.clmDeviceName.Name = "clmDeviceName";
            this.clmDeviceName.ReadOnly = true;
            this.clmDeviceName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmManufacturer
            // 
            this.clmManufacturer.Frozen = true;
            resources.ApplyResources(this.clmManufacturer, "clmManufacturer");
            this.clmManufacturer.Name = "clmManufacturer";
            this.clmManufacturer.ReadOnly = true;
            this.clmManufacturer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSpacer
            // 
            this.clmSpacer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.clmSpacer, "clmSpacer");
            this.clmSpacer.Name = "clmSpacer";
            this.clmSpacer.ReadOnly = true;
            this.clmSpacer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // tpMIDIOut2
            // 
            this.tpMIDIOut2.Controls.Add(this.groupBox15);
            resources.ApplyResources(this.tpMIDIOut2, "tpMIDIOut2");
            this.tpMIDIOut2.Name = "tpMIDIOut2";
            this.tpMIDIOut2.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            resources.ApplyResources(this.groupBox15, "groupBox15");
            this.groupBox15.Controls.Add(this.btnBeforeSend_Default);
            this.groupBox15.Controls.Add(this.tbBeforeSend_Custom);
            this.groupBox15.Controls.Add(this.tbBeforeSend_XGReset);
            this.groupBox15.Controls.Add(this.label35);
            this.groupBox15.Controls.Add(this.label34);
            this.groupBox15.Controls.Add(this.label32);
            this.groupBox15.Controls.Add(this.tbBeforeSend_GSReset);
            this.groupBox15.Controls.Add(this.label33);
            this.groupBox15.Controls.Add(this.tbBeforeSend_GMReset);
            this.groupBox15.Controls.Add(this.label31);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.TabStop = false;
            // 
            // btnBeforeSend_Default
            // 
            resources.ApplyResources(this.btnBeforeSend_Default, "btnBeforeSend_Default");
            this.btnBeforeSend_Default.Name = "btnBeforeSend_Default";
            this.btnBeforeSend_Default.UseVisualStyleBackColor = true;
            this.btnBeforeSend_Default.Click += new System.EventHandler(this.btnBeforeSend_Default_Click);
            // 
            // tbBeforeSend_Custom
            // 
            resources.ApplyResources(this.tbBeforeSend_Custom, "tbBeforeSend_Custom");
            this.tbBeforeSend_Custom.Name = "tbBeforeSend_Custom";
            // 
            // tbBeforeSend_XGReset
            // 
            resources.ApplyResources(this.tbBeforeSend_XGReset, "tbBeforeSend_XGReset");
            this.tbBeforeSend_XGReset.Name = "tbBeforeSend_XGReset";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // tbBeforeSend_GSReset
            // 
            resources.ApplyResources(this.tbBeforeSend_GSReset, "tbBeforeSend_GSReset");
            this.tbBeforeSend_GSReset.Name = "tbBeforeSend_GSReset";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // tbBeforeSend_GMReset
            // 
            resources.ApplyResources(this.tbBeforeSend_GMReset, "tbBeforeSend_GMReset");
            this.tbBeforeSend_GMReset.Name = "tbBeforeSend_GMReset";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // tabMIDIExp
            // 
            this.tabMIDIExp.Controls.Add(this.cbUseMIDIExport);
            this.tabMIDIExp.Controls.Add(this.gbMIDIExport);
            resources.ApplyResources(this.tabMIDIExp, "tabMIDIExp");
            this.tabMIDIExp.Name = "tabMIDIExp";
            this.tabMIDIExp.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIExport
            // 
            resources.ApplyResources(this.cbUseMIDIExport, "cbUseMIDIExport");
            this.cbUseMIDIExport.Name = "cbUseMIDIExport";
            this.cbUseMIDIExport.UseVisualStyleBackColor = true;
            this.cbUseMIDIExport.CheckedChanged += new System.EventHandler(this.cbUseMIDIExport_CheckedChanged);
            // 
            // gbMIDIExport
            // 
            resources.ApplyResources(this.gbMIDIExport, "gbMIDIExport");
            this.gbMIDIExport.Controls.Add(this.cbMIDIKeyOnFnum);
            this.gbMIDIExport.Controls.Add(this.cbMIDIUseVOPM);
            this.gbMIDIExport.Controls.Add(this.groupBox6);
            this.gbMIDIExport.Controls.Add(this.cbMIDIPlayless);
            this.gbMIDIExport.Controls.Add(this.btnMIDIOutputPath);
            this.gbMIDIExport.Controls.Add(this.lblOutputPath);
            this.gbMIDIExport.Controls.Add(this.tbMIDIOutputPath);
            this.gbMIDIExport.Name = "gbMIDIExport";
            this.gbMIDIExport.TabStop = false;
            // 
            // cbMIDIKeyOnFnum
            // 
            resources.ApplyResources(this.cbMIDIKeyOnFnum, "cbMIDIKeyOnFnum");
            this.cbMIDIKeyOnFnum.Name = "cbMIDIKeyOnFnum";
            this.cbMIDIKeyOnFnum.UseVisualStyleBackColor = true;
            // 
            // cbMIDIUseVOPM
            // 
            resources.ApplyResources(this.cbMIDIUseVOPM, "cbMIDIUseVOPM");
            this.cbMIDIUseVOPM.Name = "cbMIDIUseVOPM";
            this.cbMIDIUseVOPM.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbMIDIYM2612);
            this.groupBox6.Controls.Add(this.cbMIDISN76489Sec);
            this.groupBox6.Controls.Add(this.cbMIDIYM2612Sec);
            this.groupBox6.Controls.Add(this.cbMIDISN76489);
            this.groupBox6.Controls.Add(this.cbMIDIYM2151);
            this.groupBox6.Controls.Add(this.cbMIDIYM2610BSec);
            this.groupBox6.Controls.Add(this.cbMIDIYM2151Sec);
            this.groupBox6.Controls.Add(this.cbMIDIYM2610B);
            this.groupBox6.Controls.Add(this.cbMIDIYM2203);
            this.groupBox6.Controls.Add(this.cbMIDIYM2608Sec);
            this.groupBox6.Controls.Add(this.cbMIDIYM2203Sec);
            this.groupBox6.Controls.Add(this.cbMIDIYM2608);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // cbMIDIYM2612
            // 
            resources.ApplyResources(this.cbMIDIYM2612, "cbMIDIYM2612");
            this.cbMIDIYM2612.Checked = true;
            this.cbMIDIYM2612.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMIDIYM2612.Name = "cbMIDIYM2612";
            this.cbMIDIYM2612.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489Sec
            // 
            resources.ApplyResources(this.cbMIDISN76489Sec, "cbMIDISN76489Sec");
            this.cbMIDISN76489Sec.Name = "cbMIDISN76489Sec";
            this.cbMIDISN76489Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2612Sec
            // 
            resources.ApplyResources(this.cbMIDIYM2612Sec, "cbMIDIYM2612Sec");
            this.cbMIDIYM2612Sec.Name = "cbMIDIYM2612Sec";
            this.cbMIDIYM2612Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489
            // 
            resources.ApplyResources(this.cbMIDISN76489, "cbMIDISN76489");
            this.cbMIDISN76489.Name = "cbMIDISN76489";
            this.cbMIDISN76489.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151
            // 
            resources.ApplyResources(this.cbMIDIYM2151, "cbMIDIYM2151");
            this.cbMIDIYM2151.Name = "cbMIDIYM2151";
            this.cbMIDIYM2151.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610BSec
            // 
            resources.ApplyResources(this.cbMIDIYM2610BSec, "cbMIDIYM2610BSec");
            this.cbMIDIYM2610BSec.Name = "cbMIDIYM2610BSec";
            this.cbMIDIYM2610BSec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151Sec
            // 
            resources.ApplyResources(this.cbMIDIYM2151Sec, "cbMIDIYM2151Sec");
            this.cbMIDIYM2151Sec.Name = "cbMIDIYM2151Sec";
            this.cbMIDIYM2151Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610B
            // 
            resources.ApplyResources(this.cbMIDIYM2610B, "cbMIDIYM2610B");
            this.cbMIDIYM2610B.Name = "cbMIDIYM2610B";
            this.cbMIDIYM2610B.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203
            // 
            resources.ApplyResources(this.cbMIDIYM2203, "cbMIDIYM2203");
            this.cbMIDIYM2203.Name = "cbMIDIYM2203";
            this.cbMIDIYM2203.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608Sec
            // 
            resources.ApplyResources(this.cbMIDIYM2608Sec, "cbMIDIYM2608Sec");
            this.cbMIDIYM2608Sec.Name = "cbMIDIYM2608Sec";
            this.cbMIDIYM2608Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203Sec
            // 
            resources.ApplyResources(this.cbMIDIYM2203Sec, "cbMIDIYM2203Sec");
            this.cbMIDIYM2203Sec.Name = "cbMIDIYM2203Sec";
            this.cbMIDIYM2203Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608
            // 
            resources.ApplyResources(this.cbMIDIYM2608, "cbMIDIYM2608");
            this.cbMIDIYM2608.Name = "cbMIDIYM2608";
            this.cbMIDIYM2608.UseVisualStyleBackColor = true;
            // 
            // cbMIDIPlayless
            // 
            resources.ApplyResources(this.cbMIDIPlayless, "cbMIDIPlayless");
            this.cbMIDIPlayless.Name = "cbMIDIPlayless";
            this.cbMIDIPlayless.UseVisualStyleBackColor = true;
            // 
            // btnMIDIOutputPath
            // 
            resources.ApplyResources(this.btnMIDIOutputPath, "btnMIDIOutputPath");
            this.btnMIDIOutputPath.Name = "btnMIDIOutputPath";
            this.btnMIDIOutputPath.UseVisualStyleBackColor = true;
            this.btnMIDIOutputPath.Click += new System.EventHandler(this.btnMIDIOutputPath_Click);
            // 
            // lblOutputPath
            // 
            resources.ApplyResources(this.lblOutputPath, "lblOutputPath");
            this.lblOutputPath.Name = "lblOutputPath";
            // 
            // tbMIDIOutputPath
            // 
            resources.ApplyResources(this.tbMIDIOutputPath, "tbMIDIOutputPath");
            this.tbMIDIOutputPath.Name = "tbMIDIOutputPath";
            // 
            // tpMIDIKBD
            // 
            this.tpMIDIKBD.Controls.Add(this.cbUseMIDIKeyboard);
            this.tpMIDIKBD.Controls.Add(this.gbMIDIKeyboard);
            resources.ApplyResources(this.tpMIDIKBD, "tpMIDIKBD");
            this.tpMIDIKBD.Name = "tpMIDIKBD";
            this.tpMIDIKBD.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIKeyboard
            // 
            resources.ApplyResources(this.cbUseMIDIKeyboard, "cbUseMIDIKeyboard");
            this.cbUseMIDIKeyboard.Name = "cbUseMIDIKeyboard";
            this.cbUseMIDIKeyboard.UseVisualStyleBackColor = true;
            this.cbUseMIDIKeyboard.CheckedChanged += new System.EventHandler(this.cbUseMIDIKeyboard_CheckedChanged);
            // 
            // gbMIDIKeyboard
            // 
            resources.ApplyResources(this.gbMIDIKeyboard, "gbMIDIKeyboard");
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox8);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox7);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox6);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox5);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox4);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox3);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox2);
            this.gbMIDIKeyboard.Controls.Add(this.pictureBox1);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCFadeout);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCPause);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCSlow);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCPrevious);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCNext);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCFast);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCStop);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCPlay);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCCopyLog);
            this.gbMIDIKeyboard.Controls.Add(this.label17);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCDelLog);
            this.gbMIDIKeyboard.Controls.Add(this.label15);
            this.gbMIDIKeyboard.Controls.Add(this.tbCCChCopy);
            this.gbMIDIKeyboard.Controls.Add(this.label8);
            this.gbMIDIKeyboard.Controls.Add(this.label9);
            this.gbMIDIKeyboard.Controls.Add(this.gbUseChannel);
            this.gbMIDIKeyboard.Controls.Add(this.cmbMIDIIN);
            this.gbMIDIKeyboard.Controls.Add(this.label5);
            this.gbMIDIKeyboard.Name = "gbMIDIKeyboard";
            this.gbMIDIKeyboard.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::MDPlayer.Properties.Resources.ccNext;
            resources.ApplyResources(this.pictureBox8, "pictureBox8");
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::MDPlayer.Properties.Resources.ccFast;
            resources.ApplyResources(this.pictureBox7, "pictureBox7");
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::MDPlayer.Properties.Resources.ccPlay;
            resources.ApplyResources(this.pictureBox6, "pictureBox6");
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::MDPlayer.Properties.Resources.ccSlow;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::MDPlayer.Properties.Resources.ccStop;
            resources.ApplyResources(this.pictureBox4, "pictureBox4");
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::MDPlayer.Properties.Resources.ccPause;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MDPlayer.Properties.Resources.ccPrevious;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MDPlayer.Properties.Resources.ccFadeout;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // tbCCFadeout
            // 
            resources.ApplyResources(this.tbCCFadeout, "tbCCFadeout");
            this.tbCCFadeout.Name = "tbCCFadeout";
            // 
            // tbCCPause
            // 
            resources.ApplyResources(this.tbCCPause, "tbCCPause");
            this.tbCCPause.Name = "tbCCPause";
            // 
            // tbCCSlow
            // 
            resources.ApplyResources(this.tbCCSlow, "tbCCSlow");
            this.tbCCSlow.Name = "tbCCSlow";
            // 
            // tbCCPrevious
            // 
            resources.ApplyResources(this.tbCCPrevious, "tbCCPrevious");
            this.tbCCPrevious.Name = "tbCCPrevious";
            // 
            // tbCCNext
            // 
            resources.ApplyResources(this.tbCCNext, "tbCCNext");
            this.tbCCNext.Name = "tbCCNext";
            // 
            // tbCCFast
            // 
            resources.ApplyResources(this.tbCCFast, "tbCCFast");
            this.tbCCFast.Name = "tbCCFast";
            // 
            // tbCCStop
            // 
            resources.ApplyResources(this.tbCCStop, "tbCCStop");
            this.tbCCStop.Name = "tbCCStop";
            // 
            // tbCCPlay
            // 
            resources.ApplyResources(this.tbCCPlay, "tbCCPlay");
            this.tbCCPlay.Name = "tbCCPlay";
            // 
            // tbCCCopyLog
            // 
            resources.ApplyResources(this.tbCCCopyLog, "tbCCCopyLog");
            this.tbCCCopyLog.Name = "tbCCCopyLog";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // tbCCDelLog
            // 
            resources.ApplyResources(this.tbCCDelLog, "tbCCDelLog");
            this.tbCCDelLog.Name = "tbCCDelLog";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // tbCCChCopy
            // 
            resources.ApplyResources(this.tbCCChCopy, "tbCCChCopy");
            this.tbCCChCopy.Name = "tbCCChCopy";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // gbUseChannel
            // 
            this.gbUseChannel.Controls.Add(this.rbMONO);
            this.gbUseChannel.Controls.Add(this.rbPOLY);
            this.gbUseChannel.Controls.Add(this.groupBox7);
            this.gbUseChannel.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.gbUseChannel, "gbUseChannel");
            this.gbUseChannel.Name = "gbUseChannel";
            this.gbUseChannel.TabStop = false;
            // 
            // rbMONO
            // 
            resources.ApplyResources(this.rbMONO, "rbMONO");
            this.rbMONO.Checked = true;
            this.rbMONO.Name = "rbMONO";
            this.rbMONO.TabStop = true;
            this.rbMONO.UseVisualStyleBackColor = true;
            // 
            // rbPOLY
            // 
            resources.ApplyResources(this.rbPOLY, "rbPOLY");
            this.rbPOLY.Name = "rbPOLY";
            this.rbPOLY.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.rbFM6);
            this.groupBox7.Controls.Add(this.rbFM3);
            this.groupBox7.Controls.Add(this.rbFM5);
            this.groupBox7.Controls.Add(this.rbFM2);
            this.groupBox7.Controls.Add(this.rbFM4);
            this.groupBox7.Controls.Add(this.rbFM1);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // rbFM6
            // 
            resources.ApplyResources(this.rbFM6, "rbFM6");
            this.rbFM6.Name = "rbFM6";
            this.rbFM6.UseVisualStyleBackColor = true;
            // 
            // rbFM3
            // 
            resources.ApplyResources(this.rbFM3, "rbFM3");
            this.rbFM3.Name = "rbFM3";
            this.rbFM3.UseVisualStyleBackColor = true;
            // 
            // rbFM5
            // 
            resources.ApplyResources(this.rbFM5, "rbFM5");
            this.rbFM5.Name = "rbFM5";
            this.rbFM5.UseVisualStyleBackColor = true;
            // 
            // rbFM2
            // 
            resources.ApplyResources(this.rbFM2, "rbFM2");
            this.rbFM2.Name = "rbFM2";
            this.rbFM2.UseVisualStyleBackColor = true;
            // 
            // rbFM4
            // 
            resources.ApplyResources(this.rbFM4, "rbFM4");
            this.rbFM4.Name = "rbFM4";
            this.rbFM4.UseVisualStyleBackColor = true;
            // 
            // rbFM1
            // 
            resources.ApplyResources(this.rbFM1, "rbFM1");
            this.rbFM1.Checked = true;
            this.rbFM1.Name = "rbFM1";
            this.rbFM1.TabStop = true;
            this.rbFM1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbFM1);
            this.groupBox2.Controls.Add(this.cbFM6);
            this.groupBox2.Controls.Add(this.cbFM2);
            this.groupBox2.Controls.Add(this.cbFM5);
            this.groupBox2.Controls.Add(this.cbFM3);
            this.groupBox2.Controls.Add(this.cbFM4);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cbFM1
            // 
            resources.ApplyResources(this.cbFM1, "cbFM1");
            this.cbFM1.Checked = true;
            this.cbFM1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM1.Name = "cbFM1";
            this.cbFM1.UseVisualStyleBackColor = true;
            // 
            // cbFM6
            // 
            resources.ApplyResources(this.cbFM6, "cbFM6");
            this.cbFM6.Checked = true;
            this.cbFM6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM6.Name = "cbFM6";
            this.cbFM6.UseVisualStyleBackColor = true;
            // 
            // cbFM2
            // 
            resources.ApplyResources(this.cbFM2, "cbFM2");
            this.cbFM2.Checked = true;
            this.cbFM2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM2.Name = "cbFM2";
            this.cbFM2.UseVisualStyleBackColor = true;
            // 
            // cbFM5
            // 
            resources.ApplyResources(this.cbFM5, "cbFM5");
            this.cbFM5.Checked = true;
            this.cbFM5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM5.Name = "cbFM5";
            this.cbFM5.UseVisualStyleBackColor = true;
            // 
            // cbFM3
            // 
            resources.ApplyResources(this.cbFM3, "cbFM3");
            this.cbFM3.Checked = true;
            this.cbFM3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM3.Name = "cbFM3";
            this.cbFM3.UseVisualStyleBackColor = true;
            // 
            // cbFM4
            // 
            resources.ApplyResources(this.cbFM4, "cbFM4");
            this.cbFM4.Checked = true;
            this.cbFM4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM4.Name = "cbFM4";
            this.cbFM4.UseVisualStyleBackColor = true;
            // 
            // cmbMIDIIN
            // 
            this.cmbMIDIIN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMIDIIN.FormattingEnabled = true;
            resources.ApplyResources(this.cmbMIDIIN, "cmbMIDIIN");
            this.cmbMIDIIN.Name = "cmbMIDIIN";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // tpKeyBoard
            // 
            this.tpKeyBoard.Controls.Add(this.cbUseKeyBoardHook);
            this.tpKeyBoard.Controls.Add(this.gbUseKeyBoardHook);
            this.tpKeyBoard.Controls.Add(this.label47);
            this.tpKeyBoard.Controls.Add(this.cbStopWin);
            this.tpKeyBoard.Controls.Add(this.cbPauseWin);
            this.tpKeyBoard.Controls.Add(this.cbFadeoutWin);
            this.tpKeyBoard.Controls.Add(this.cbPrevWin);
            this.tpKeyBoard.Controls.Add(this.cbSlowWin);
            this.tpKeyBoard.Controls.Add(this.cbPlayWin);
            this.tpKeyBoard.Controls.Add(this.cbFastWin);
            this.tpKeyBoard.Controls.Add(this.cbNextWin);
            resources.ApplyResources(this.tpKeyBoard, "tpKeyBoard");
            this.tpKeyBoard.Name = "tpKeyBoard";
            this.tpKeyBoard.UseVisualStyleBackColor = true;
            // 
            // cbUseKeyBoardHook
            // 
            resources.ApplyResources(this.cbUseKeyBoardHook, "cbUseKeyBoardHook");
            this.cbUseKeyBoardHook.Name = "cbUseKeyBoardHook";
            this.cbUseKeyBoardHook.UseVisualStyleBackColor = true;
            this.cbUseKeyBoardHook.CheckedChanged += new System.EventHandler(this.cbUseKeyBoardHook_CheckedChanged);
            // 
            // gbUseKeyBoardHook
            // 
            resources.ApplyResources(this.gbUseKeyBoardHook, "gbUseKeyBoardHook");
            this.gbUseKeyBoardHook.Controls.Add(this.lblKeyBoardHookNotice);
            this.gbUseKeyBoardHook.Controls.Add(this.btNextClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btPrevClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btPlayClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btPauseClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btFastClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btFadeoutClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btSlowClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btStopClr);
            this.gbUseKeyBoardHook.Controls.Add(this.btNextSet);
            this.gbUseKeyBoardHook.Controls.Add(this.btPrevSet);
            this.gbUseKeyBoardHook.Controls.Add(this.btPlaySet);
            this.gbUseKeyBoardHook.Controls.Add(this.btPauseSet);
            this.gbUseKeyBoardHook.Controls.Add(this.btFastSet);
            this.gbUseKeyBoardHook.Controls.Add(this.btFadeoutSet);
            this.gbUseKeyBoardHook.Controls.Add(this.btSlowSet);
            this.gbUseKeyBoardHook.Controls.Add(this.btStopSet);
            this.gbUseKeyBoardHook.Controls.Add(this.label50);
            this.gbUseKeyBoardHook.Controls.Add(this.lblNextKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblFastKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblPlayKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblSlowKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblPrevKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblFadeoutKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblPauseKey);
            this.gbUseKeyBoardHook.Controls.Add(this.lblStopKey);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox14);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox17);
            this.gbUseKeyBoardHook.Controls.Add(this.cbNextAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox16);
            this.gbUseKeyBoardHook.Controls.Add(this.cbFastAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox15);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPlayAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox13);
            this.gbUseKeyBoardHook.Controls.Add(this.cbSlowAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox12);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPrevAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox11);
            this.gbUseKeyBoardHook.Controls.Add(this.cbFadeoutAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.pictureBox10);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPauseAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.label37);
            this.gbUseKeyBoardHook.Controls.Add(this.cbStopAlt);
            this.gbUseKeyBoardHook.Controls.Add(this.label45);
            this.gbUseKeyBoardHook.Controls.Add(this.label46);
            this.gbUseKeyBoardHook.Controls.Add(this.label48);
            this.gbUseKeyBoardHook.Controls.Add(this.label38);
            this.gbUseKeyBoardHook.Controls.Add(this.label39);
            this.gbUseKeyBoardHook.Controls.Add(this.label40);
            this.gbUseKeyBoardHook.Controls.Add(this.label41);
            this.gbUseKeyBoardHook.Controls.Add(this.label42);
            this.gbUseKeyBoardHook.Controls.Add(this.cbNextCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.label43);
            this.gbUseKeyBoardHook.Controls.Add(this.cbFastCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.label44);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPlayCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.cbStopShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbSlowCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPauseShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPrevCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.cbFadeoutShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbFadeoutCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPrevShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPauseCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.cbSlowShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbStopCtrl);
            this.gbUseKeyBoardHook.Controls.Add(this.cbPlayShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbNextShift);
            this.gbUseKeyBoardHook.Controls.Add(this.cbFastShift);
            this.gbUseKeyBoardHook.Name = "gbUseKeyBoardHook";
            this.gbUseKeyBoardHook.TabStop = false;
            // 
            // lblKeyBoardHookNotice
            // 
            resources.ApplyResources(this.lblKeyBoardHookNotice, "lblKeyBoardHookNotice");
            this.lblKeyBoardHookNotice.ForeColor = System.Drawing.Color.Red;
            this.lblKeyBoardHookNotice.Name = "lblKeyBoardHookNotice";
            // 
            // btNextClr
            // 
            resources.ApplyResources(this.btNextClr, "btNextClr");
            this.btNextClr.Name = "btNextClr";
            this.btNextClr.UseVisualStyleBackColor = true;
            this.btNextClr.Click += new System.EventHandler(this.btNextClr_Click);
            // 
            // btPrevClr
            // 
            resources.ApplyResources(this.btPrevClr, "btPrevClr");
            this.btPrevClr.Name = "btPrevClr";
            this.btPrevClr.UseVisualStyleBackColor = true;
            this.btPrevClr.Click += new System.EventHandler(this.btPrevClr_Click);
            // 
            // btPlayClr
            // 
            resources.ApplyResources(this.btPlayClr, "btPlayClr");
            this.btPlayClr.Name = "btPlayClr";
            this.btPlayClr.UseVisualStyleBackColor = true;
            this.btPlayClr.Click += new System.EventHandler(this.btPlayClr_Click);
            // 
            // btPauseClr
            // 
            resources.ApplyResources(this.btPauseClr, "btPauseClr");
            this.btPauseClr.Name = "btPauseClr";
            this.btPauseClr.UseVisualStyleBackColor = true;
            this.btPauseClr.Click += new System.EventHandler(this.btPauseClr_Click);
            // 
            // btFastClr
            // 
            resources.ApplyResources(this.btFastClr, "btFastClr");
            this.btFastClr.Name = "btFastClr";
            this.btFastClr.UseVisualStyleBackColor = true;
            this.btFastClr.Click += new System.EventHandler(this.btFastClr_Click);
            // 
            // btFadeoutClr
            // 
            resources.ApplyResources(this.btFadeoutClr, "btFadeoutClr");
            this.btFadeoutClr.Name = "btFadeoutClr";
            this.btFadeoutClr.UseVisualStyleBackColor = true;
            this.btFadeoutClr.Click += new System.EventHandler(this.btFadeoutClr_Click);
            // 
            // btSlowClr
            // 
            resources.ApplyResources(this.btSlowClr, "btSlowClr");
            this.btSlowClr.Name = "btSlowClr";
            this.btSlowClr.UseVisualStyleBackColor = true;
            this.btSlowClr.Click += new System.EventHandler(this.btSlowClr_Click);
            // 
            // btStopClr
            // 
            resources.ApplyResources(this.btStopClr, "btStopClr");
            this.btStopClr.Name = "btStopClr";
            this.btStopClr.UseVisualStyleBackColor = true;
            this.btStopClr.Click += new System.EventHandler(this.btStopClr_Click);
            // 
            // btNextSet
            // 
            resources.ApplyResources(this.btNextSet, "btNextSet");
            this.btNextSet.Name = "btNextSet";
            this.btNextSet.UseVisualStyleBackColor = true;
            this.btNextSet.Click += new System.EventHandler(this.btNextSet_Click);
            // 
            // btPrevSet
            // 
            resources.ApplyResources(this.btPrevSet, "btPrevSet");
            this.btPrevSet.Name = "btPrevSet";
            this.btPrevSet.UseVisualStyleBackColor = true;
            this.btPrevSet.Click += new System.EventHandler(this.btPrevSet_Click);
            // 
            // btPlaySet
            // 
            resources.ApplyResources(this.btPlaySet, "btPlaySet");
            this.btPlaySet.Name = "btPlaySet";
            this.btPlaySet.UseVisualStyleBackColor = true;
            this.btPlaySet.Click += new System.EventHandler(this.btPlaySet_Click);
            // 
            // btPauseSet
            // 
            resources.ApplyResources(this.btPauseSet, "btPauseSet");
            this.btPauseSet.Name = "btPauseSet";
            this.btPauseSet.UseVisualStyleBackColor = true;
            this.btPauseSet.Click += new System.EventHandler(this.btPauseSet_Click);
            // 
            // btFastSet
            // 
            resources.ApplyResources(this.btFastSet, "btFastSet");
            this.btFastSet.Name = "btFastSet";
            this.btFastSet.UseVisualStyleBackColor = true;
            this.btFastSet.Click += new System.EventHandler(this.btFastSet_Click);
            // 
            // btFadeoutSet
            // 
            resources.ApplyResources(this.btFadeoutSet, "btFadeoutSet");
            this.btFadeoutSet.Name = "btFadeoutSet";
            this.btFadeoutSet.UseVisualStyleBackColor = true;
            this.btFadeoutSet.Click += new System.EventHandler(this.btFadeoutSet_Click);
            // 
            // btSlowSet
            // 
            resources.ApplyResources(this.btSlowSet, "btSlowSet");
            this.btSlowSet.Name = "btSlowSet";
            this.btSlowSet.UseVisualStyleBackColor = true;
            this.btSlowSet.Click += new System.EventHandler(this.btSlowSet_Click);
            // 
            // btStopSet
            // 
            resources.ApplyResources(this.btStopSet, "btStopSet");
            this.btStopSet.Name = "btStopSet";
            this.btStopSet.UseVisualStyleBackColor = true;
            this.btStopSet.Click += new System.EventHandler(this.btStopSet_Click);
            // 
            // label50
            // 
            resources.ApplyResources(this.label50, "label50");
            this.label50.Name = "label50";
            // 
            // lblNextKey
            // 
            resources.ApplyResources(this.lblNextKey, "lblNextKey");
            this.lblNextKey.Name = "lblNextKey";
            // 
            // lblFastKey
            // 
            resources.ApplyResources(this.lblFastKey, "lblFastKey");
            this.lblFastKey.Name = "lblFastKey";
            // 
            // lblPlayKey
            // 
            resources.ApplyResources(this.lblPlayKey, "lblPlayKey");
            this.lblPlayKey.Name = "lblPlayKey";
            // 
            // lblSlowKey
            // 
            resources.ApplyResources(this.lblSlowKey, "lblSlowKey");
            this.lblSlowKey.Name = "lblSlowKey";
            // 
            // lblPrevKey
            // 
            resources.ApplyResources(this.lblPrevKey, "lblPrevKey");
            this.lblPrevKey.Name = "lblPrevKey";
            // 
            // lblFadeoutKey
            // 
            resources.ApplyResources(this.lblFadeoutKey, "lblFadeoutKey");
            this.lblFadeoutKey.Name = "lblFadeoutKey";
            // 
            // lblPauseKey
            // 
            resources.ApplyResources(this.lblPauseKey, "lblPauseKey");
            this.lblPauseKey.Name = "lblPauseKey";
            // 
            // lblStopKey
            // 
            resources.ApplyResources(this.lblStopKey, "lblStopKey");
            this.lblStopKey.Name = "lblStopKey";
            // 
            // pictureBox14
            // 
            this.pictureBox14.Image = global::MDPlayer.Properties.Resources.ccStop;
            resources.ApplyResources(this.pictureBox14, "pictureBox14");
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.TabStop = false;
            // 
            // pictureBox17
            // 
            this.pictureBox17.Image = global::MDPlayer.Properties.Resources.ccFadeout;
            resources.ApplyResources(this.pictureBox17, "pictureBox17");
            this.pictureBox17.Name = "pictureBox17";
            this.pictureBox17.TabStop = false;
            // 
            // cbNextAlt
            // 
            resources.ApplyResources(this.cbNextAlt, "cbNextAlt");
            this.cbNextAlt.Name = "cbNextAlt";
            this.cbNextAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox16
            // 
            this.pictureBox16.Image = global::MDPlayer.Properties.Resources.ccPrevious;
            resources.ApplyResources(this.pictureBox16, "pictureBox16");
            this.pictureBox16.Name = "pictureBox16";
            this.pictureBox16.TabStop = false;
            // 
            // cbFastAlt
            // 
            resources.ApplyResources(this.cbFastAlt, "cbFastAlt");
            this.cbFastAlt.Name = "cbFastAlt";
            this.cbFastAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox15
            // 
            this.pictureBox15.Image = global::MDPlayer.Properties.Resources.ccPause;
            resources.ApplyResources(this.pictureBox15, "pictureBox15");
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.TabStop = false;
            // 
            // cbPlayAlt
            // 
            resources.ApplyResources(this.cbPlayAlt, "cbPlayAlt");
            this.cbPlayAlt.Name = "cbPlayAlt";
            this.cbPlayAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox13
            // 
            this.pictureBox13.Image = global::MDPlayer.Properties.Resources.ccSlow;
            resources.ApplyResources(this.pictureBox13, "pictureBox13");
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.TabStop = false;
            // 
            // cbSlowAlt
            // 
            resources.ApplyResources(this.cbSlowAlt, "cbSlowAlt");
            this.cbSlowAlt.Name = "cbSlowAlt";
            this.cbSlowAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox12
            // 
            this.pictureBox12.Image = global::MDPlayer.Properties.Resources.ccPlay;
            resources.ApplyResources(this.pictureBox12, "pictureBox12");
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.TabStop = false;
            // 
            // cbPrevAlt
            // 
            resources.ApplyResources(this.cbPrevAlt, "cbPrevAlt");
            this.cbPrevAlt.Name = "cbPrevAlt";
            this.cbPrevAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox11
            // 
            this.pictureBox11.Image = global::MDPlayer.Properties.Resources.ccFast;
            resources.ApplyResources(this.pictureBox11, "pictureBox11");
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.TabStop = false;
            // 
            // cbFadeoutAlt
            // 
            resources.ApplyResources(this.cbFadeoutAlt, "cbFadeoutAlt");
            this.cbFadeoutAlt.Name = "cbFadeoutAlt";
            this.cbFadeoutAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::MDPlayer.Properties.Resources.ccNext;
            resources.ApplyResources(this.pictureBox10, "pictureBox10");
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.TabStop = false;
            // 
            // cbPauseAlt
            // 
            resources.ApplyResources(this.cbPauseAlt, "cbPauseAlt");
            this.cbPauseAlt.Name = "cbPauseAlt";
            this.cbPauseAlt.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // cbStopAlt
            // 
            resources.ApplyResources(this.cbStopAlt, "cbStopAlt");
            this.cbStopAlt.Name = "cbStopAlt";
            this.cbStopAlt.UseVisualStyleBackColor = true;
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.Name = "label45";
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // label48
            // 
            resources.ApplyResources(this.label48, "label48");
            this.label48.Name = "label48";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // cbNextCtrl
            // 
            resources.ApplyResources(this.cbNextCtrl, "cbNextCtrl");
            this.cbNextCtrl.Name = "cbNextCtrl";
            this.cbNextCtrl.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.Name = "label43";
            // 
            // cbFastCtrl
            // 
            resources.ApplyResources(this.cbFastCtrl, "cbFastCtrl");
            this.cbFastCtrl.Name = "cbFastCtrl";
            this.cbFastCtrl.UseVisualStyleBackColor = true;
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // cbPlayCtrl
            // 
            resources.ApplyResources(this.cbPlayCtrl, "cbPlayCtrl");
            this.cbPlayCtrl.Name = "cbPlayCtrl";
            this.cbPlayCtrl.UseVisualStyleBackColor = true;
            // 
            // cbStopShift
            // 
            resources.ApplyResources(this.cbStopShift, "cbStopShift");
            this.cbStopShift.Name = "cbStopShift";
            this.cbStopShift.UseVisualStyleBackColor = true;
            // 
            // cbSlowCtrl
            // 
            resources.ApplyResources(this.cbSlowCtrl, "cbSlowCtrl");
            this.cbSlowCtrl.Name = "cbSlowCtrl";
            this.cbSlowCtrl.UseVisualStyleBackColor = true;
            // 
            // cbPauseShift
            // 
            resources.ApplyResources(this.cbPauseShift, "cbPauseShift");
            this.cbPauseShift.Name = "cbPauseShift";
            this.cbPauseShift.UseVisualStyleBackColor = true;
            // 
            // cbPrevCtrl
            // 
            resources.ApplyResources(this.cbPrevCtrl, "cbPrevCtrl");
            this.cbPrevCtrl.Name = "cbPrevCtrl";
            this.cbPrevCtrl.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutShift
            // 
            resources.ApplyResources(this.cbFadeoutShift, "cbFadeoutShift");
            this.cbFadeoutShift.Name = "cbFadeoutShift";
            this.cbFadeoutShift.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutCtrl
            // 
            resources.ApplyResources(this.cbFadeoutCtrl, "cbFadeoutCtrl");
            this.cbFadeoutCtrl.Name = "cbFadeoutCtrl";
            this.cbFadeoutCtrl.UseVisualStyleBackColor = true;
            // 
            // cbPrevShift
            // 
            resources.ApplyResources(this.cbPrevShift, "cbPrevShift");
            this.cbPrevShift.Name = "cbPrevShift";
            this.cbPrevShift.UseVisualStyleBackColor = true;
            // 
            // cbPauseCtrl
            // 
            resources.ApplyResources(this.cbPauseCtrl, "cbPauseCtrl");
            this.cbPauseCtrl.Name = "cbPauseCtrl";
            this.cbPauseCtrl.UseVisualStyleBackColor = true;
            // 
            // cbSlowShift
            // 
            resources.ApplyResources(this.cbSlowShift, "cbSlowShift");
            this.cbSlowShift.Name = "cbSlowShift";
            this.cbSlowShift.UseVisualStyleBackColor = true;
            // 
            // cbStopCtrl
            // 
            resources.ApplyResources(this.cbStopCtrl, "cbStopCtrl");
            this.cbStopCtrl.Name = "cbStopCtrl";
            this.cbStopCtrl.UseVisualStyleBackColor = true;
            // 
            // cbPlayShift
            // 
            resources.ApplyResources(this.cbPlayShift, "cbPlayShift");
            this.cbPlayShift.Name = "cbPlayShift";
            this.cbPlayShift.UseVisualStyleBackColor = true;
            // 
            // cbNextShift
            // 
            resources.ApplyResources(this.cbNextShift, "cbNextShift");
            this.cbNextShift.Name = "cbNextShift";
            this.cbNextShift.UseVisualStyleBackColor = true;
            // 
            // cbFastShift
            // 
            resources.ApplyResources(this.cbFastShift, "cbFastShift");
            this.cbFastShift.Name = "cbFastShift";
            this.cbFastShift.UseVisualStyleBackColor = true;
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.Name = "label47";
            // 
            // cbStopWin
            // 
            resources.ApplyResources(this.cbStopWin, "cbStopWin");
            this.cbStopWin.Name = "cbStopWin";
            this.cbStopWin.UseVisualStyleBackColor = true;
            // 
            // cbPauseWin
            // 
            resources.ApplyResources(this.cbPauseWin, "cbPauseWin");
            this.cbPauseWin.Name = "cbPauseWin";
            this.cbPauseWin.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutWin
            // 
            resources.ApplyResources(this.cbFadeoutWin, "cbFadeoutWin");
            this.cbFadeoutWin.Name = "cbFadeoutWin";
            this.cbFadeoutWin.UseVisualStyleBackColor = true;
            // 
            // cbPrevWin
            // 
            resources.ApplyResources(this.cbPrevWin, "cbPrevWin");
            this.cbPrevWin.Name = "cbPrevWin";
            this.cbPrevWin.UseVisualStyleBackColor = true;
            // 
            // cbSlowWin
            // 
            resources.ApplyResources(this.cbSlowWin, "cbSlowWin");
            this.cbSlowWin.Name = "cbSlowWin";
            this.cbSlowWin.UseVisualStyleBackColor = true;
            // 
            // cbPlayWin
            // 
            resources.ApplyResources(this.cbPlayWin, "cbPlayWin");
            this.cbPlayWin.Name = "cbPlayWin";
            this.cbPlayWin.UseVisualStyleBackColor = true;
            // 
            // cbFastWin
            // 
            resources.ApplyResources(this.cbFastWin, "cbFastWin");
            this.cbFastWin.Name = "cbFastWin";
            this.cbFastWin.UseVisualStyleBackColor = true;
            // 
            // cbNextWin
            // 
            resources.ApplyResources(this.cbNextWin, "cbNextWin");
            this.cbNextWin.Name = "cbNextWin";
            this.cbNextWin.UseVisualStyleBackColor = true;
            // 
            // tpBalance
            // 
            this.tpBalance.Controls.Add(this.groupBox25);
            this.tpBalance.Controls.Add(this.cbAutoBalanceUseThis);
            this.tpBalance.Controls.Add(this.groupBox18);
            resources.ApplyResources(this.tpBalance, "tpBalance");
            this.tpBalance.Name = "tpBalance";
            this.tpBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            resources.ApplyResources(this.groupBox25, "groupBox25");
            this.groupBox25.Controls.Add(this.rbAutoBalanceNotSamePositionAsSongData);
            this.groupBox25.Controls.Add(this.rbAutoBalanceSamePositionAsSongData);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.TabStop = false;
            // 
            // rbAutoBalanceNotSamePositionAsSongData
            // 
            resources.ApplyResources(this.rbAutoBalanceNotSamePositionAsSongData, "rbAutoBalanceNotSamePositionAsSongData");
            this.rbAutoBalanceNotSamePositionAsSongData.Checked = true;
            this.rbAutoBalanceNotSamePositionAsSongData.Name = "rbAutoBalanceNotSamePositionAsSongData";
            this.rbAutoBalanceNotSamePositionAsSongData.TabStop = true;
            this.rbAutoBalanceNotSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSamePositionAsSongData
            // 
            resources.ApplyResources(this.rbAutoBalanceSamePositionAsSongData, "rbAutoBalanceSamePositionAsSongData");
            this.rbAutoBalanceSamePositionAsSongData.Name = "rbAutoBalanceSamePositionAsSongData";
            this.rbAutoBalanceSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // cbAutoBalanceUseThis
            // 
            resources.ApplyResources(this.cbAutoBalanceUseThis, "cbAutoBalanceUseThis");
            this.cbAutoBalanceUseThis.Checked = true;
            this.cbAutoBalanceUseThis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoBalanceUseThis.Name = "cbAutoBalanceUseThis";
            this.cbAutoBalanceUseThis.UseVisualStyleBackColor = true;
            // 
            // groupBox18
            // 
            resources.ApplyResources(this.groupBox18, "groupBox18");
            this.groupBox18.Controls.Add(this.groupBox24);
            this.groupBox18.Controls.Add(this.groupBox23);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.TabStop = false;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.groupBox21);
            this.groupBox24.Controls.Add(this.groupBox22);
            resources.ApplyResources(this.groupBox24, "groupBox24");
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.TabStop = false;
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.rbAutoBalanceNotSaveSongBalance);
            this.groupBox21.Controls.Add(this.rbAutoBalanceSaveSongBalance);
            resources.ApplyResources(this.groupBox21, "groupBox21");
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.TabStop = false;
            // 
            // rbAutoBalanceNotSaveSongBalance
            // 
            resources.ApplyResources(this.rbAutoBalanceNotSaveSongBalance, "rbAutoBalanceNotSaveSongBalance");
            this.rbAutoBalanceNotSaveSongBalance.Checked = true;
            this.rbAutoBalanceNotSaveSongBalance.Name = "rbAutoBalanceNotSaveSongBalance";
            this.rbAutoBalanceNotSaveSongBalance.TabStop = true;
            this.rbAutoBalanceNotSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSaveSongBalance
            // 
            resources.ApplyResources(this.rbAutoBalanceSaveSongBalance, "rbAutoBalanceSaveSongBalance");
            this.rbAutoBalanceSaveSongBalance.Name = "rbAutoBalanceSaveSongBalance";
            this.rbAutoBalanceSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox22, "groupBox22");
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox23
            // 
            resources.ApplyResources(this.groupBox23, "groupBox23");
            this.groupBox23.Controls.Add(this.groupBox19);
            this.groupBox23.Controls.Add(this.groupBox20);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.TabStop = false;
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.rbAutoBalanceNotLoadSongBalance);
            this.groupBox19.Controls.Add(this.rbAutoBalanceLoadSongBalance);
            resources.ApplyResources(this.groupBox19, "groupBox19");
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.TabStop = false;
            // 
            // rbAutoBalanceNotLoadSongBalance
            // 
            resources.ApplyResources(this.rbAutoBalanceNotLoadSongBalance, "rbAutoBalanceNotLoadSongBalance");
            this.rbAutoBalanceNotLoadSongBalance.Checked = true;
            this.rbAutoBalanceNotLoadSongBalance.Name = "rbAutoBalanceNotLoadSongBalance";
            this.rbAutoBalanceNotLoadSongBalance.TabStop = true;
            this.rbAutoBalanceNotLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadSongBalance
            // 
            resources.ApplyResources(this.rbAutoBalanceLoadSongBalance, "rbAutoBalanceLoadSongBalance");
            this.rbAutoBalanceLoadSongBalance.Name = "rbAutoBalanceLoadSongBalance";
            this.rbAutoBalanceLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            resources.ApplyResources(this.groupBox20, "groupBox20");
            this.groupBox20.Controls.Add(this.rbAutoBalanceNotLoadDriverBalance);
            this.groupBox20.Controls.Add(this.rbAutoBalanceLoadDriverBalance);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.TabStop = false;
            this.groupBox20.Enter += new System.EventHandler(this.groupBox20_Enter);
            // 
            // rbAutoBalanceNotLoadDriverBalance
            // 
            resources.ApplyResources(this.rbAutoBalanceNotLoadDriverBalance, "rbAutoBalanceNotLoadDriverBalance");
            this.rbAutoBalanceNotLoadDriverBalance.Name = "rbAutoBalanceNotLoadDriverBalance";
            this.rbAutoBalanceNotLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadDriverBalance
            // 
            resources.ApplyResources(this.rbAutoBalanceLoadDriverBalance, "rbAutoBalanceLoadDriverBalance");
            this.rbAutoBalanceLoadDriverBalance.Checked = true;
            this.rbAutoBalanceLoadDriverBalance.Name = "rbAutoBalanceLoadDriverBalance";
            this.rbAutoBalanceLoadDriverBalance.TabStop = true;
            this.rbAutoBalanceLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // tpPlayList
            // 
            this.tpPlayList.Controls.Add(this.groupBox17);
            this.tpPlayList.Controls.Add(this.cbEmptyPlayList);
            resources.ApplyResources(this.tpPlayList, "tpPlayList");
            this.tpPlayList.Name = "tpPlayList";
            this.tpPlayList.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            resources.ApplyResources(this.groupBox17, "groupBox17");
            this.groupBox17.Controls.Add(this.cbAutoOpenImg);
            this.groupBox17.Controls.Add(this.tbImageExt);
            this.groupBox17.Controls.Add(this.cbAutoOpenMML);
            this.groupBox17.Controls.Add(this.tbMMLExt);
            this.groupBox17.Controls.Add(this.tbTextExt);
            this.groupBox17.Controls.Add(this.cbAutoOpenText);
            this.groupBox17.Controls.Add(this.label1);
            this.groupBox17.Controls.Add(this.label3);
            this.groupBox17.Controls.Add(this.label2);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.TabStop = false;
            // 
            // cbAutoOpenImg
            // 
            resources.ApplyResources(this.cbAutoOpenImg, "cbAutoOpenImg");
            this.cbAutoOpenImg.Name = "cbAutoOpenImg";
            this.cbAutoOpenImg.UseVisualStyleBackColor = true;
            this.cbAutoOpenImg.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // tbImageExt
            // 
            resources.ApplyResources(this.tbImageExt, "tbImageExt");
            this.tbImageExt.Name = "tbImageExt";
            // 
            // cbAutoOpenMML
            // 
            resources.ApplyResources(this.cbAutoOpenMML, "cbAutoOpenMML");
            this.cbAutoOpenMML.Name = "cbAutoOpenMML";
            this.cbAutoOpenMML.UseVisualStyleBackColor = true;
            this.cbAutoOpenMML.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // tbMMLExt
            // 
            resources.ApplyResources(this.tbMMLExt, "tbMMLExt");
            this.tbMMLExt.Name = "tbMMLExt";
            // 
            // tbTextExt
            // 
            resources.ApplyResources(this.tbTextExt, "tbTextExt");
            this.tbTextExt.Name = "tbTextExt";
            // 
            // cbAutoOpenText
            // 
            resources.ApplyResources(this.cbAutoOpenText, "cbAutoOpenText");
            this.cbAutoOpenText.Name = "cbAutoOpenText";
            this.cbAutoOpenText.UseVisualStyleBackColor = true;
            this.cbAutoOpenText.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cbEmptyPlayList
            // 
            resources.ApplyResources(this.cbEmptyPlayList, "cbEmptyPlayList");
            this.cbEmptyPlayList.Name = "cbEmptyPlayList";
            this.cbEmptyPlayList.UseVisualStyleBackColor = true;
            this.cbEmptyPlayList.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // tpOther
            // 
            this.tpOther.Controls.Add(this.btnSearchPath);
            this.tpOther.Controls.Add(this.tbSearchPath);
            this.tpOther.Controls.Add(this.label68);
            this.tpOther.Controls.Add(this.cbNonRenderingForPause);
            this.tpOther.Controls.Add(this.cbWavSwitch);
            this.tpOther.Controls.Add(this.cbUseGetInst);
            this.tpOther.Controls.Add(this.groupBox4);
            this.tpOther.Controls.Add(this.cbDumpSwitch);
            this.tpOther.Controls.Add(this.gbWav);
            this.tpOther.Controls.Add(this.gbDump);
            this.tpOther.Controls.Add(this.label30);
            this.tpOther.Controls.Add(this.tbScreenFrameRate);
            this.tpOther.Controls.Add(this.label29);
            this.tpOther.Controls.Add(this.lblLoopTimes);
            this.tpOther.Controls.Add(this.btnDataPath);
            this.tpOther.Controls.Add(this.tbLoopTimes);
            this.tpOther.Controls.Add(this.tbDataPath);
            this.tpOther.Controls.Add(this.label19);
            this.tpOther.Controls.Add(this.btnResetPosition);
            this.tpOther.Controls.Add(this.btnOpenSettingFolder);
            this.tpOther.Controls.Add(this.cbExALL);
            this.tpOther.Controls.Add(this.cbInitAlways);
            this.tpOther.Controls.Add(this.cbAutoOpen);
            this.tpOther.Controls.Add(this.cbUseLoopTimes);
            resources.ApplyResources(this.tpOther, "tpOther");
            this.tpOther.Name = "tpOther";
            this.tpOther.UseVisualStyleBackColor = true;
            // 
            // btnSearchPath
            // 
            resources.ApplyResources(this.btnSearchPath, "btnSearchPath");
            this.btnSearchPath.Name = "btnSearchPath";
            this.btnSearchPath.UseVisualStyleBackColor = true;
            this.btnSearchPath.Click += new System.EventHandler(this.btnSearchPath_Click);
            // 
            // tbSearchPath
            // 
            resources.ApplyResources(this.tbSearchPath, "tbSearchPath");
            this.tbSearchPath.Name = "tbSearchPath";
            // 
            // label68
            // 
            resources.ApplyResources(this.label68, "label68");
            this.label68.Name = "label68";
            // 
            // cbNonRenderingForPause
            // 
            resources.ApplyResources(this.cbNonRenderingForPause, "cbNonRenderingForPause");
            this.cbNonRenderingForPause.Name = "cbNonRenderingForPause";
            this.cbNonRenderingForPause.UseVisualStyleBackColor = true;
            // 
            // cbWavSwitch
            // 
            resources.ApplyResources(this.cbWavSwitch, "cbWavSwitch");
            this.cbWavSwitch.Name = "cbWavSwitch";
            this.cbWavSwitch.UseVisualStyleBackColor = true;
            this.cbWavSwitch.CheckedChanged += new System.EventHandler(this.cbWavSwitch_CheckedChanged);
            // 
            // cbUseGetInst
            // 
            resources.ApplyResources(this.cbUseGetInst, "cbUseGetInst");
            this.cbUseGetInst.Name = "cbUseGetInst";
            this.cbUseGetInst.UseVisualStyleBackColor = true;
            this.cbUseGetInst.CheckedChanged += new System.EventHandler(this.cbUseGetInst_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbInstFormat);
            this.groupBox4.Controls.Add(this.lblInstFormat);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // cmbInstFormat
            // 
            this.cmbInstFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstFormat.FormattingEnabled = true;
            this.cmbInstFormat.Items.AddRange(new object[] {
            resources.GetString("cmbInstFormat.Items"),
            resources.GetString("cmbInstFormat.Items1"),
            resources.GetString("cmbInstFormat.Items2"),
            resources.GetString("cmbInstFormat.Items3"),
            resources.GetString("cmbInstFormat.Items4"),
            resources.GetString("cmbInstFormat.Items5"),
            resources.GetString("cmbInstFormat.Items6"),
            resources.GetString("cmbInstFormat.Items7"),
            resources.GetString("cmbInstFormat.Items8"),
            resources.GetString("cmbInstFormat.Items9"),
            resources.GetString("cmbInstFormat.Items10"),
            resources.GetString("cmbInstFormat.Items11"),
            resources.GetString("cmbInstFormat.Items12"),
            resources.GetString("cmbInstFormat.Items13"),
            resources.GetString("cmbInstFormat.Items14"),
            resources.GetString("cmbInstFormat.Items15"),
            resources.GetString("cmbInstFormat.Items16")});
            resources.ApplyResources(this.cmbInstFormat, "cmbInstFormat");
            this.cmbInstFormat.Name = "cmbInstFormat";
            // 
            // lblInstFormat
            // 
            resources.ApplyResources(this.lblInstFormat, "lblInstFormat");
            this.lblInstFormat.Name = "lblInstFormat";
            // 
            // cbDumpSwitch
            // 
            resources.ApplyResources(this.cbDumpSwitch, "cbDumpSwitch");
            this.cbDumpSwitch.Name = "cbDumpSwitch";
            this.cbDumpSwitch.UseVisualStyleBackColor = true;
            this.cbDumpSwitch.CheckedChanged += new System.EventHandler(this.cbDumpSwitch_CheckedChanged);
            // 
            // gbWav
            // 
            this.gbWav.Controls.Add(this.btnWavPath);
            this.gbWav.Controls.Add(this.label7);
            this.gbWav.Controls.Add(this.tbWavPath);
            resources.ApplyResources(this.gbWav, "gbWav");
            this.gbWav.Name = "gbWav";
            this.gbWav.TabStop = false;
            // 
            // btnWavPath
            // 
            resources.ApplyResources(this.btnWavPath, "btnWavPath");
            this.btnWavPath.Name = "btnWavPath";
            this.btnWavPath.UseVisualStyleBackColor = true;
            this.btnWavPath.Click += new System.EventHandler(this.btnWavPath_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // tbWavPath
            // 
            resources.ApplyResources(this.tbWavPath, "tbWavPath");
            this.tbWavPath.Name = "tbWavPath";
            // 
            // gbDump
            // 
            this.gbDump.Controls.Add(this.btnDumpPath);
            this.gbDump.Controls.Add(this.label6);
            this.gbDump.Controls.Add(this.tbDumpPath);
            resources.ApplyResources(this.gbDump, "gbDump");
            this.gbDump.Name = "gbDump";
            this.gbDump.TabStop = false;
            // 
            // btnDumpPath
            // 
            resources.ApplyResources(this.btnDumpPath, "btnDumpPath");
            this.btnDumpPath.Name = "btnDumpPath";
            this.btnDumpPath.UseVisualStyleBackColor = true;
            this.btnDumpPath.Click += new System.EventHandler(this.btnDumpPath_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // tbDumpPath
            // 
            resources.ApplyResources(this.tbDumpPath, "tbDumpPath");
            this.tbDumpPath.Name = "tbDumpPath";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // tbScreenFrameRate
            // 
            resources.ApplyResources(this.tbScreenFrameRate, "tbScreenFrameRate");
            this.tbScreenFrameRate.Name = "tbScreenFrameRate";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // lblLoopTimes
            // 
            resources.ApplyResources(this.lblLoopTimes, "lblLoopTimes");
            this.lblLoopTimes.Name = "lblLoopTimes";
            // 
            // btnDataPath
            // 
            resources.ApplyResources(this.btnDataPath, "btnDataPath");
            this.btnDataPath.Name = "btnDataPath";
            this.btnDataPath.UseVisualStyleBackColor = true;
            this.btnDataPath.Click += new System.EventHandler(this.btnDataPath_Click);
            // 
            // tbLoopTimes
            // 
            resources.ApplyResources(this.tbLoopTimes, "tbLoopTimes");
            this.tbLoopTimes.Name = "tbLoopTimes";
            // 
            // tbDataPath
            // 
            resources.ApplyResources(this.tbDataPath, "tbDataPath");
            this.tbDataPath.Name = "tbDataPath";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // btnResetPosition
            // 
            resources.ApplyResources(this.btnResetPosition, "btnResetPosition");
            this.btnResetPosition.Name = "btnResetPosition";
            this.btnResetPosition.UseVisualStyleBackColor = true;
            this.btnResetPosition.Click += new System.EventHandler(this.btnResetPosition_Click);
            // 
            // btnOpenSettingFolder
            // 
            resources.ApplyResources(this.btnOpenSettingFolder, "btnOpenSettingFolder");
            this.btnOpenSettingFolder.Name = "btnOpenSettingFolder";
            this.btnOpenSettingFolder.UseVisualStyleBackColor = true;
            this.btnOpenSettingFolder.Click += new System.EventHandler(this.btnOpenSettingFolder_Click);
            // 
            // cbExALL
            // 
            resources.ApplyResources(this.cbExALL, "cbExALL");
            this.cbExALL.Name = "cbExALL";
            this.cbExALL.UseVisualStyleBackColor = true;
            this.cbExALL.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbInitAlways
            // 
            resources.ApplyResources(this.cbInitAlways, "cbInitAlways");
            this.cbInitAlways.Name = "cbInitAlways";
            this.cbInitAlways.UseVisualStyleBackColor = true;
            this.cbInitAlways.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbAutoOpen
            // 
            resources.ApplyResources(this.cbAutoOpen, "cbAutoOpen");
            this.cbAutoOpen.Name = "cbAutoOpen";
            this.cbAutoOpen.UseVisualStyleBackColor = true;
            this.cbAutoOpen.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbUseLoopTimes
            // 
            resources.ApplyResources(this.cbUseLoopTimes, "cbUseLoopTimes");
            this.cbUseLoopTimes.Name = "cbUseLoopTimes";
            this.cbUseLoopTimes.UseVisualStyleBackColor = true;
            this.cbUseLoopTimes.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // tpOmake
            // 
            this.tpOmake.Controls.Add(this.label67);
            this.tpOmake.Controls.Add(this.label14);
            this.tpOmake.Controls.Add(this.btVST);
            this.tpOmake.Controls.Add(this.tbSCCbaseAddress);
            this.tpOmake.Controls.Add(this.tbVST);
            this.tpOmake.Controls.Add(this.groupBox5);
            resources.ApplyResources(this.tpOmake, "tpOmake");
            this.tpOmake.Name = "tpOmake";
            this.tpOmake.UseVisualStyleBackColor = true;
            // 
            // label67
            // 
            resources.ApplyResources(this.label67, "label67");
            this.label67.Name = "label67";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // btVST
            // 
            resources.ApplyResources(this.btVST, "btVST");
            this.btVST.Name = "btVST";
            this.btVST.UseVisualStyleBackColor = true;
            this.btVST.Click += new System.EventHandler(this.btVST_Click);
            // 
            // tbSCCbaseAddress
            // 
            resources.ApplyResources(this.tbSCCbaseAddress, "tbSCCbaseAddress");
            this.tbSCCbaseAddress.Name = "tbSCCbaseAddress";
            // 
            // tbVST
            // 
            resources.ApplyResources(this.tbVST, "tbVST");
            this.tbVST.Name = "tbVST";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbDispFrameCounter);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // cbDispFrameCounter
            // 
            resources.ApplyResources(this.cbDispFrameCounter, "cbDispFrameCounter");
            this.cbDispFrameCounter.Name = "cbDispFrameCounter";
            this.cbDispFrameCounter.UseVisualStyleBackColor = true;
            // 
            // tpAbout
            // 
            this.tpAbout.Controls.Add(this.tableLayoutPanel);
            resources.ApplyResources(this.tpAbout, "tpAbout");
            this.tpAbout.Name = "tpAbout";
            this.tpAbout.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.llOpenGithub, 1, 5);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // logoPictureBox
            // 
            resources.ApplyResources(this.logoPictureBox, "logoPictureBox");
            this.logoPictureBox.Image = global::MDPlayer.Properties.Resources.FeliAndMD2;
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            resources.ApplyResources(this.labelProductName, "labelProductName");
            this.labelProductName.Name = "labelProductName";
            // 
            // labelVersion
            // 
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.Name = "labelVersion";
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.labelCopyright.Name = "labelCopyright";
            // 
            // labelCompanyName
            // 
            resources.ApplyResources(this.labelCompanyName, "labelCompanyName");
            this.labelCompanyName.Name = "labelCompanyName";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.TabStop = false;
            // 
            // llOpenGithub
            // 
            resources.ApplyResources(this.llOpenGithub, "llOpenGithub");
            this.llOpenGithub.Name = "llOpenGithub";
            this.llOpenGithub.TabStop = true;
            this.llOpenGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llOpenGithub_LinkClicked);
            // 
            // frmSetting
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.tcSetting);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetting";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSetting_FormClosed);
            this.Load += new System.EventHandler(this.frmSetting_Load);
            this.gbWaveOut.ResumeLayout(false);
            this.gbAsioOut.ResumeLayout(false);
            this.gbWasapiOut.ResumeLayout(false);
            this.gbWasapiOut.PerformLayout();
            this.gbDirectSound.ResumeLayout(false);
            this.tcSetting.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.tpModule.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tpNuked.ResumeLayout(false);
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.tpNSF.ResumeLayout(false);
            this.tpNSF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkbNSFLPF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkbNSFHPF)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tpSID.ResumeLayout(false);
            this.tpSID.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.tpPMDDotNET.ResumeLayout(false);
            this.tpPMDDotNET.PerformLayout();
            this.gbPMDManual.ResumeLayout(false);
            this.gbPMDManual.PerformLayout();
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            this.gbPPSDRV.ResumeLayout(false);
            this.groupBox33.ResumeLayout(false);
            this.groupBox33.PerformLayout();
            this.gbPMDSetManualVolume.ResumeLayout(false);
            this.gbPMDSetManualVolume.PerformLayout();
            this.tpMIDIOut.ResumeLayout(false);
            this.tpMIDIOut.PerformLayout();
            this.tbcMIDIoutList.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListA)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListB)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListC)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListD)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListE)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListF)).EndInit();
            this.tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListG)).EndInit();
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListH)).EndInit();
            this.tabPage9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListI)).EndInit();
            this.tabPage10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutListJ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMIDIoutPallet)).EndInit();
            this.tpMIDIOut2.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.tabMIDIExp.ResumeLayout(false);
            this.tabMIDIExp.PerformLayout();
            this.gbMIDIExport.ResumeLayout(false);
            this.gbMIDIExport.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tpMIDIKBD.ResumeLayout(false);
            this.tpMIDIKBD.PerformLayout();
            this.gbMIDIKeyboard.ResumeLayout(false);
            this.gbMIDIKeyboard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbUseChannel.ResumeLayout(false);
            this.gbUseChannel.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tpKeyBoard.ResumeLayout(false);
            this.tpKeyBoard.PerformLayout();
            this.gbUseKeyBoardHook.ResumeLayout(false);
            this.gbUseKeyBoardHook.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            this.tpBalance.ResumeLayout(false);
            this.tpBalance.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            this.groupBox18.ResumeLayout(false);
            this.groupBox24.ResumeLayout(false);
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox23.ResumeLayout(false);
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.tpPlayList.ResumeLayout(false);
            this.tpPlayList.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.tpOther.ResumeLayout(false);
            this.tpOther.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.gbWav.ResumeLayout(false);
            this.gbWav.PerformLayout();
            this.gbDump.ResumeLayout(false);
            this.gbDump.PerformLayout();
            this.tpOmake.ResumeLayout(false);
            this.tpOmake.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tpAbout.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbWaveOut;
        private System.Windows.Forms.RadioButton rbWaveOut;
        private System.Windows.Forms.RadioButton rbAsioOut;
        private System.Windows.Forms.RadioButton rbWasapiOut;
        private System.Windows.Forms.GroupBox gbAsioOut;
        private System.Windows.Forms.RadioButton rbDirectSoundOut;
        private System.Windows.Forms.GroupBox gbWasapiOut;
        private System.Windows.Forms.GroupBox gbDirectSound;
        private System.Windows.Forms.ComboBox cmbWaveOutDevice;
        private System.Windows.Forms.Button btnASIOControlPanel;
        private System.Windows.Forms.ComboBox cmbAsioDevice;
        private System.Windows.Forms.ComboBox cmbWasapiDevice;
        private System.Windows.Forms.ComboBox cmbDirectSoundDevice;
        private System.Windows.Forms.TabControl tcSetting;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.TabPage tpAbout;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.TabPage tpOther;
        private System.Windows.Forms.GroupBox gbMIDIKeyboard;
        private System.Windows.Forms.GroupBox gbUseChannel;
        private System.Windows.Forms.CheckBox cbFM1;
        private System.Windows.Forms.CheckBox cbFM2;
        private System.Windows.Forms.CheckBox cbFM3;
        private System.Windows.Forms.CheckBox cbUseMIDIKeyboard;
        private System.Windows.Forms.CheckBox cbFM4;
        private System.Windows.Forms.CheckBox cbFM5;
        private System.Windows.Forms.CheckBox cbFM6;
        private System.Windows.Forms.ComboBox cmbMIDIIN;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbExclusive;
        private System.Windows.Forms.RadioButton rbShare;
        private System.Windows.Forms.Label lblLatencyUnit;
        private System.Windows.Forms.Label lblLatency;
        private System.Windows.Forms.ComboBox cmbLatency;
        private System.Windows.Forms.TabPage tpModule;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbLatencyEmu;
        private System.Windows.Forms.TextBox tbLatencySCCI;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbDispFrameCounter;
        private System.Windows.Forms.CheckBox cbHiyorimiMode;
        private System.Windows.Forms.CheckBox cbUseLoopTimes;
        private System.Windows.Forms.Label lblLoopTimes;
        private System.Windows.Forms.TextBox tbLoopTimes;
        private System.Windows.Forms.Button btnOpenSettingFolder;
        private System.Windows.Forms.CheckBox cbUseGetInst;
        private System.Windows.Forms.Button btnDataPath;
        private System.Windows.Forms.TextBox tbDataPath;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TabPage tpMIDIKBD;
        private System.Windows.Forms.ComboBox cmbInstFormat;
        private System.Windows.Forms.Label lblInstFormat;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tbScreenFrameRate;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox cbAutoOpen;
        private ucSettingInstruments ucSI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbDumpSwitch;
        private System.Windows.Forms.GroupBox gbDump;
        private System.Windows.Forms.Button btnDumpPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbDumpPath;
        private System.Windows.Forms.Button btnResetPosition;
        private System.Windows.Forms.TabPage tabMIDIExp;
        private System.Windows.Forms.CheckBox cbUseMIDIExport;
        private System.Windows.Forms.GroupBox gbMIDIExport;
        private System.Windows.Forms.CheckBox cbMIDIUseVOPM;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbMIDIYM2612;
        private System.Windows.Forms.CheckBox cbMIDISN76489Sec;
        private System.Windows.Forms.CheckBox cbMIDIYM2612Sec;
        private System.Windows.Forms.CheckBox cbMIDISN76489;
        private System.Windows.Forms.CheckBox cbMIDIYM2151;
        private System.Windows.Forms.CheckBox cbMIDIYM2610BSec;
        private System.Windows.Forms.CheckBox cbMIDIYM2151Sec;
        private System.Windows.Forms.CheckBox cbMIDIYM2610B;
        private System.Windows.Forms.CheckBox cbMIDIYM2203;
        private System.Windows.Forms.CheckBox cbMIDIYM2608Sec;
        private System.Windows.Forms.CheckBox cbMIDIYM2203Sec;
        private System.Windows.Forms.CheckBox cbMIDIYM2608;
        private System.Windows.Forms.CheckBox cbMIDIPlayless;
        private System.Windows.Forms.Button btnMIDIOutputPath;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.TextBox tbMIDIOutputPath;
        private System.Windows.Forms.CheckBox cbWavSwitch;
        private System.Windows.Forms.GroupBox gbWav;
        private System.Windows.Forms.Button btnWavPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbWavPath;
        private System.Windows.Forms.RadioButton rbMONO;
        private System.Windows.Forms.RadioButton rbPOLY;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton rbFM6;
        private System.Windows.Forms.RadioButton rbFM3;
        private System.Windows.Forms.RadioButton rbFM5;
        private System.Windows.Forms.RadioButton rbFM2;
        private System.Windows.Forms.RadioButton rbFM4;
        private System.Windows.Forms.RadioButton rbFM1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tpOmake;
        private System.Windows.Forms.TextBox tbCCFadeout;
        private System.Windows.Forms.TextBox tbCCPause;
        private System.Windows.Forms.TextBox tbCCSlow;
        private System.Windows.Forms.TextBox tbCCPrevious;
        private System.Windows.Forms.TextBox tbCCNext;
        private System.Windows.Forms.TextBox tbCCFast;
        private System.Windows.Forms.TextBox tbCCStop;
        private System.Windows.Forms.TextBox tbCCPlay;
        private System.Windows.Forms.TextBox tbCCCopyLog;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbCCDelLog;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbCCChCopy;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btVST;
        private System.Windows.Forms.TextBox tbVST;
        private System.Windows.Forms.TabPage tpMIDIOut;
        private System.Windows.Forms.Button btnUP_A;
        private System.Windows.Forms.Button btnSubMIDIout;
        private System.Windows.Forms.Button btnDOWN_A;
        private System.Windows.Forms.Button btnAddMIDIout;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataGridView dgvMIDIoutListA;
        private System.Windows.Forms.DataGridView dgvMIDIoutPallet;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmID;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDeviceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmManufacturer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSpacer;
        private System.Windows.Forms.TabControl tbcMIDIoutList;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnUP_B;
        private System.Windows.Forms.Button btnDOWN_B;
        private System.Windows.Forms.Button btnUP_C;
        private System.Windows.Forms.Button btnDOWN_C;
        private System.Windows.Forms.Button btnUP_D;
        private System.Windows.Forms.Button btnDOWN_D;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnUP_E;
        private System.Windows.Forms.Button btnDOWN_E;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button btnUP_F;
        private System.Windows.Forms.Button btnDOWN_F;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Button btnUP_G;
        private System.Windows.Forms.Button btnDOWN_G;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Button btnUP_H;
        private System.Windows.Forms.Button btnDOWN_H;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.Button btnUP_I;
        private System.Windows.Forms.Button btnDOWN_I;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button btnDOWN_J;
        private System.Windows.Forms.Button btnAddVST;
        private System.Windows.Forms.DataGridView dgvMIDIoutListB;
        private System.Windows.Forms.DataGridView dgvMIDIoutListC;
        private System.Windows.Forms.DataGridView dgvMIDIoutListD;
        private System.Windows.Forms.DataGridView dgvMIDIoutListE;
        private System.Windows.Forms.DataGridView dgvMIDIoutListF;
        private System.Windows.Forms.DataGridView dgvMIDIoutListG;
        private System.Windows.Forms.DataGridView dgvMIDIoutListH;
        private System.Windows.Forms.DataGridView dgvMIDIoutListI;
        private System.Windows.Forms.DataGridView dgvMIDIoutListJ;
        private System.Windows.Forms.TabPage tpNSF;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox cbNSFFDSWriteDisable8000;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.CheckBox cbNSFDmc_RandomizeTri;
        private System.Windows.Forms.CheckBox cbNSFDmc_TriMute;
        private System.Windows.Forms.CheckBox cbNSFDmc_RandomizeNoise;
        private System.Windows.Forms.CheckBox cbNSFDmc_DPCMAntiClick;
        private System.Windows.Forms.CheckBox cbNSFDmc_EnablePNoise;
        private System.Windows.Forms.CheckBox cbNSFDmc_Enable4011;
        private System.Windows.Forms.CheckBox cbNSFDmc_NonLinearMixer;
        private System.Windows.Forms.CheckBox cbNSFDmc_UnmuteOnReset;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.CheckBox cbNSFN160_Serial;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.CheckBox cbNSFMmc5_PhaseRefresh;
        private System.Windows.Forms.CheckBox cbNSFMmc5_NonLinearMixer;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox cbNFSNes_DutySwap;
        private System.Windows.Forms.CheckBox cbNFSNes_PhaseRefresh;
        private System.Windows.Forms.CheckBox cbNFSNes_NonLinearMixer;
        private System.Windows.Forms.CheckBox cbNFSNes_UnmuteOnReset;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbNSFFds_LPF;
        private System.Windows.Forms.CheckBox cbNFSFds_4085Reset;
        private System.Windows.Forms.TabPage tpSID;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnSIDCharacter;
        private System.Windows.Forms.Button btnSIDBasic;
        private System.Windows.Forms.Button btnSIDKernal;
        private System.Windows.Forms.TextBox tbSIDCharacter;
        private System.Windows.Forms.TextBox tbSIDBasic;
        private System.Windows.Forms.TextBox tbSIDKernal;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.RadioButton rdSIDQ1;
        private System.Windows.Forms.RadioButton rdSIDQ3;
        private System.Windows.Forms.RadioButton rdSIDQ2;
        private System.Windows.Forms.RadioButton rdSIDQ4;
        private System.Windows.Forms.Label lblWaitTime;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox cmbWaitTime;
        private System.Windows.Forms.TabPage tpMIDIOut2;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.Button btnBeforeSend_Default;
        private System.Windows.Forms.TextBox tbBeforeSend_Custom;
        private System.Windows.Forms.TextBox tbBeforeSend_XGReset;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox tbBeforeSend_GSReset;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox tbBeforeSend_GMReset;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmIsVST;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmType;
        private System.Windows.Forms.DataGridViewComboBoxColumn ClmBeforeSend;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.RadioButton rbSPPCM;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.ComboBox cmbSPPCMDevice;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.TextBox tbImageExt;
        private System.Windows.Forms.TextBox tbMMLExt;
        private System.Windows.Forms.TextBox tbTextExt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbInitAlways;
        private System.Windows.Forms.TabPage tpBalance;
        private System.Windows.Forms.CheckBox cbAutoBalanceUseThis;
        private System.Windows.Forms.GroupBox groupBox18;
        private System.Windows.Forms.GroupBox groupBox24;
        private System.Windows.Forms.GroupBox groupBox21;
        private System.Windows.Forms.RadioButton rbAutoBalanceNotSaveSongBalance;
        private System.Windows.Forms.RadioButton rbAutoBalanceSamePositionAsSongData;
        private System.Windows.Forms.RadioButton rbAutoBalanceSaveSongBalance;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.GroupBox groupBox19;
        private System.Windows.Forms.RadioButton rbAutoBalanceNotLoadSongBalance;
        private System.Windows.Forms.RadioButton rbAutoBalanceLoadSongBalance;
        private System.Windows.Forms.GroupBox groupBox20;
        private System.Windows.Forms.RadioButton rbAutoBalanceNotLoadDriverBalance;
        private System.Windows.Forms.RadioButton rbAutoBalanceLoadDriverBalance;
        private System.Windows.Forms.GroupBox groupBox25;
        private System.Windows.Forms.RadioButton rbAutoBalanceNotSamePositionAsSongData;
        private System.Windows.Forms.TabPage tpKeyBoard;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.PictureBox pictureBox13;
        private System.Windows.Forms.PictureBox pictureBox14;
        private System.Windows.Forms.PictureBox pictureBox15;
        private System.Windows.Forms.PictureBox pictureBox16;
        private System.Windows.Forms.PictureBox pictureBox17;
        private System.Windows.Forms.CheckBox cbUseKeyBoardHook;
        private System.Windows.Forms.GroupBox gbUseKeyBoardHook;
        private System.Windows.Forms.Button btPrevClr;
        private System.Windows.Forms.Button btPauseClr;
        private System.Windows.Forms.Button btFadeoutClr;
        private System.Windows.Forms.Button btStopClr;
        private System.Windows.Forms.Button btNextSet;
        private System.Windows.Forms.Button btPrevSet;
        private System.Windows.Forms.Button btPlaySet;
        private System.Windows.Forms.Button btPauseSet;
        private System.Windows.Forms.Button btFastSet;
        private System.Windows.Forms.Button btFadeoutSet;
        private System.Windows.Forms.Button btSlowSet;
        private System.Windows.Forms.Button btStopSet;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label lblNextKey;
        private System.Windows.Forms.Label lblFastKey;
        private System.Windows.Forms.Label lblPlayKey;
        private System.Windows.Forms.Label lblSlowKey;
        private System.Windows.Forms.Label lblPrevKey;
        private System.Windows.Forms.Label lblFadeoutKey;
        private System.Windows.Forms.Label lblPauseKey;
        private System.Windows.Forms.Label lblStopKey;
        private System.Windows.Forms.CheckBox cbNextAlt;
        private System.Windows.Forms.CheckBox cbFastAlt;
        private System.Windows.Forms.CheckBox cbPlayAlt;
        private System.Windows.Forms.CheckBox cbSlowAlt;
        private System.Windows.Forms.CheckBox cbPrevAlt;
        private System.Windows.Forms.CheckBox cbFadeoutAlt;
        private System.Windows.Forms.CheckBox cbPauseAlt;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.CheckBox cbStopAlt;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.CheckBox cbNextWin;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.CheckBox cbFastWin;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.CheckBox cbPlayWin;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.CheckBox cbSlowWin;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.CheckBox cbPrevWin;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.CheckBox cbFadeoutWin;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.CheckBox cbPauseWin;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.CheckBox cbStopWin;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.CheckBox cbNextCtrl;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.CheckBox cbFastCtrl;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.CheckBox cbPlayCtrl;
        private System.Windows.Forms.CheckBox cbStopShift;
        private System.Windows.Forms.CheckBox cbSlowCtrl;
        private System.Windows.Forms.CheckBox cbPauseShift;
        private System.Windows.Forms.CheckBox cbPrevCtrl;
        private System.Windows.Forms.CheckBox cbFadeoutShift;
        private System.Windows.Forms.CheckBox cbFadeoutCtrl;
        private System.Windows.Forms.CheckBox cbPrevShift;
        private System.Windows.Forms.CheckBox cbPauseCtrl;
        private System.Windows.Forms.CheckBox cbSlowShift;
        private System.Windows.Forms.CheckBox cbStopCtrl;
        private System.Windows.Forms.CheckBox cbPlayShift;
        private System.Windows.Forms.CheckBox cbNextShift;
        private System.Windows.Forms.CheckBox cbFastShift;
        private System.Windows.Forms.Button btNextClr;
        private System.Windows.Forms.Button btPlayClr;
        private System.Windows.Forms.Button btFastClr;
        private System.Windows.Forms.Button btSlowClr;
        //private ucSettingInstruments ucSettingInstruments1;
        private System.Windows.Forms.Label lblKeyBoardHookNotice;
        private System.Windows.Forms.RadioButton rbNullDevice;
        private System.Windows.Forms.TextBox tbSIDOutputBufferSize;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TabPage tpNuked;
        private System.Windows.Forms.GroupBox groupBox26;
        private System.Windows.Forms.RadioButton rbNukedOPN2OptionYM2612u;
        private System.Windows.Forms.RadioButton rbNukedOPN2OptionYM2612;
        private System.Windows.Forms.RadioButton rbNukedOPN2OptionDiscrete;
        private System.Windows.Forms.RadioButton rbNukedOPN2OptionASIC;
        private System.Windows.Forms.RadioButton rbNukedOPN2OptionASIClp;
        private System.Windows.Forms.CheckBox cbEmptyPlayList;
        private System.Windows.Forms.CheckBox cbMIDIKeyOnFnum;
        private System.Windows.Forms.CheckBox cbExALL;
        private System.Windows.Forms.CheckBox cbNonRenderingForPause;
        private System.Windows.Forms.LinkLabel llOpenGithub;
        private System.Windows.Forms.TrackBar trkbNSFLPF;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TrackBar trkbNSFHPF;
        private System.Windows.Forms.TabPage tpPMDDotNET;
        private System.Windows.Forms.RadioButton rbPMDManual;
        private System.Windows.Forms.RadioButton rbPMDAuto;
        private System.Windows.Forms.Button btnPMDResetDriverArguments;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Button btnPMDResetCompilerArhguments;
        private System.Windows.Forms.TextBox tbPMDDriverArguments;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.TextBox tbPMDCompilerArguments;
        private System.Windows.Forms.GroupBox gbPMDManual;
        private System.Windows.Forms.CheckBox cbPMDSetManualVolume;
        private System.Windows.Forms.CheckBox cbPMDUsePPZ8;
        private System.Windows.Forms.GroupBox groupBox32;
        private System.Windows.Forms.RadioButton rbPMD86B;
        private System.Windows.Forms.RadioButton rbPMDSpbB;
        private System.Windows.Forms.RadioButton rbPMDNrmB;
        private System.Windows.Forms.CheckBox cbPMDUsePPSDRV;
        private System.Windows.Forms.GroupBox gbPPSDRV;
        private System.Windows.Forms.GroupBox groupBox33;
        private System.Windows.Forms.RadioButton rbPMDUsePPSDRVManualFreq;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.RadioButton rbPMDUsePPSDRVFreqDefault;
        private System.Windows.Forms.Button btnPMDPPSDRVManualWait;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.TextBox tbPMDPPSDRVFreq;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.TextBox tbPMDPPSDRVManualWait;
        private System.Windows.Forms.GroupBox gbPMDSetManualVolume;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.TextBox tbPMDVolumeAdpcm;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.TextBox tbPMDVolumeRhythm;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.TextBox tbPMDVolumeSSG;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.TextBox tbPMDVolumeGIMICSSG;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox tbPMDVolumeFM;
        private System.Windows.Forms.GroupBox groupBox28;
        private System.Windows.Forms.GroupBox groupBox27;
        private System.Windows.Forms.RadioButton rbSIDC64Model_PAL;
        private System.Windows.Forms.RadioButton rbSIDC64Model_DREAN;
        private System.Windows.Forms.RadioButton rbSIDC64Model_OLDNTSC;
        private System.Windows.Forms.RadioButton rbSIDC64Model_NTSC;
        private System.Windows.Forms.RadioButton rbSIDModel_8580;
        private System.Windows.Forms.RadioButton rbSIDModel_6581;
        private System.Windows.Forms.CheckBox cbSIDC64Model_Force;
        private System.Windows.Forms.CheckBox cbSIDModel_Force;
        private System.Windows.Forms.GroupBox groupBox29;
        private System.Windows.Forms.CheckBox cbGensSSGEG;
        private System.Windows.Forms.CheckBox cbGensDACHPF;
        private System.Windows.Forms.TabPage tpPlayList;
        private System.Windows.Forms.CheckBox cbAutoOpenImg;
        private System.Windows.Forms.CheckBox cbAutoOpenMML;
        private System.Windows.Forms.CheckBox cbAutoOpenText;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.ComboBox cmbSampleRate;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.TextBox tbSCCbaseAddress;
        private System.Windows.Forms.Button btnSearchPath;
        private System.Windows.Forms.TextBox tbSearchPath;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.CheckBox cbNSFDmc_DPCMReverse;
        private System.Windows.Forms.CheckBox cbUnuseRealChip;
    }
}