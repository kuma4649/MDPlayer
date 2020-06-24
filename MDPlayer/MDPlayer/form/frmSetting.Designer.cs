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
            this.lblLatencyUnit = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.lblLatency = new System.Windows.Forms.Label();
            this.cmbWaitTime = new System.Windows.Forms.ComboBox();
            this.cmbLatency = new System.Windows.Forms.ComboBox();
            this.rbSPPCM = new System.Windows.Forms.RadioButton();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.cmbSPPCMDevice = new System.Windows.Forms.ComboBox();
            this.tpModule = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.rbNukedOPN2OptionYM2612u = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionYM2612 = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionDiscrete = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionASIClp = new System.Windows.Forms.RadioButton();
            this.rbNukedOPN2OptionASIC = new System.Windows.Forms.RadioButton();
            this.tpNSF = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.cbNSFDmc_TriNull = new System.Windows.Forms.CheckBox();
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
            this.tpOther = new System.Windows.Forms.TabPage();
            this.cbNonRenderingForPause = new System.Windows.Forms.CheckBox();
            this.cbWavSwitch = new System.Windows.Forms.CheckBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.tbImageExt = new System.Windows.Forms.TextBox();
            this.tbMMLExt = new System.Windows.Forms.TextBox();
            this.tbTextExt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            this.cbEmptyPlayList = new System.Windows.Forms.CheckBox();
            this.cbInitAlways = new System.Windows.Forms.CheckBox();
            this.cbAutoOpen = new System.Windows.Forms.CheckBox();
            this.cbUseLoopTimes = new System.Windows.Forms.CheckBox();
            this.tpOmake = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.btVST = new System.Windows.Forms.Button();
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
            this.groupBox26.SuspendLayout();
            this.tpNSF.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tpSID.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox13.SuspendLayout();
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
            this.tpOther.SuspendLayout();
            this.groupBox17.SuspendLayout();
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
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(296, 406);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(377, 406);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbWaveOut
            // 
            this.gbWaveOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWaveOut.Controls.Add(this.cmbWaveOutDevice);
            this.gbWaveOut.Location = new System.Drawing.Point(7, 31);
            this.gbWaveOut.Name = "gbWaveOut";
            this.gbWaveOut.Size = new System.Drawing.Size(429, 48);
            this.gbWaveOut.TabIndex = 1;
            this.gbWaveOut.TabStop = false;
            // 
            // cmbWaveOutDevice
            // 
            this.cmbWaveOutDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWaveOutDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaveOutDevice.FormattingEnabled = true;
            this.cmbWaveOutDevice.Location = new System.Drawing.Point(6, 18);
            this.cmbWaveOutDevice.Name = "cmbWaveOutDevice";
            this.cmbWaveOutDevice.Size = new System.Drawing.Size(417, 20);
            this.cmbWaveOutDevice.TabIndex = 0;
            // 
            // rbWaveOut
            // 
            this.rbWaveOut.AutoSize = true;
            this.rbWaveOut.Checked = true;
            this.rbWaveOut.Location = new System.Drawing.Point(13, 28);
            this.rbWaveOut.Name = "rbWaveOut";
            this.rbWaveOut.Size = new System.Drawing.Size(68, 16);
            this.rbWaveOut.TabIndex = 0;
            this.rbWaveOut.TabStop = true;
            this.rbWaveOut.Text = "WaveOut";
            this.rbWaveOut.UseVisualStyleBackColor = true;
            this.rbWaveOut.CheckedChanged += new System.EventHandler(this.rbWaveOut_CheckedChanged);
            // 
            // rbAsioOut
            // 
            this.rbAsioOut.AutoSize = true;
            this.rbAsioOut.Location = new System.Drawing.Point(13, 192);
            this.rbAsioOut.Name = "rbAsioOut";
            this.rbAsioOut.Size = new System.Drawing.Size(64, 16);
            this.rbAsioOut.TabIndex = 6;
            this.rbAsioOut.Text = "AsioOut";
            this.rbAsioOut.UseVisualStyleBackColor = true;
            this.rbAsioOut.CheckedChanged += new System.EventHandler(this.rbAsioOut_CheckedChanged);
            // 
            // rbWasapiOut
            // 
            this.rbWasapiOut.AutoSize = true;
            this.rbWasapiOut.Location = new System.Drawing.Point(13, 136);
            this.rbWasapiOut.Name = "rbWasapiOut";
            this.rbWasapiOut.Size = new System.Drawing.Size(77, 16);
            this.rbWasapiOut.TabIndex = 4;
            this.rbWasapiOut.Text = "WasapiOut";
            this.rbWasapiOut.UseVisualStyleBackColor = true;
            this.rbWasapiOut.CheckedChanged += new System.EventHandler(this.rbWasapiOut_CheckedChanged);
            // 
            // gbAsioOut
            // 
            this.gbAsioOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAsioOut.Controls.Add(this.btnASIOControlPanel);
            this.gbAsioOut.Controls.Add(this.cmbAsioDevice);
            this.gbAsioOut.Location = new System.Drawing.Point(7, 195);
            this.gbAsioOut.Name = "gbAsioOut";
            this.gbAsioOut.Size = new System.Drawing.Size(429, 50);
            this.gbAsioOut.TabIndex = 7;
            this.gbAsioOut.TabStop = false;
            // 
            // btnASIOControlPanel
            // 
            this.btnASIOControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnASIOControlPanel.Location = new System.Drawing.Point(342, 8);
            this.btnASIOControlPanel.Name = "btnASIOControlPanel";
            this.btnASIOControlPanel.Size = new System.Drawing.Size(81, 39);
            this.btnASIOControlPanel.TabIndex = 8;
            this.btnASIOControlPanel.Text = "ASIO Control Panel";
            this.btnASIOControlPanel.UseVisualStyleBackColor = true;
            this.btnASIOControlPanel.Click += new System.EventHandler(this.btnASIOControlPanel_Click);
            // 
            // cmbAsioDevice
            // 
            this.cmbAsioDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAsioDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAsioDevice.FormattingEnabled = true;
            this.cmbAsioDevice.Location = new System.Drawing.Point(6, 18);
            this.cmbAsioDevice.Name = "cmbAsioDevice";
            this.cmbAsioDevice.Size = new System.Drawing.Size(330, 20);
            this.cmbAsioDevice.TabIndex = 6;
            // 
            // rbDirectSoundOut
            // 
            this.rbDirectSoundOut.AutoSize = true;
            this.rbDirectSoundOut.Location = new System.Drawing.Point(13, 82);
            this.rbDirectSoundOut.Name = "rbDirectSoundOut";
            this.rbDirectSoundOut.Size = new System.Drawing.Size(85, 16);
            this.rbDirectSoundOut.TabIndex = 2;
            this.rbDirectSoundOut.Text = "DirectSound";
            this.rbDirectSoundOut.UseVisualStyleBackColor = true;
            this.rbDirectSoundOut.CheckedChanged += new System.EventHandler(this.rbDirectSoundOut_CheckedChanged);
            // 
            // gbWasapiOut
            // 
            this.gbWasapiOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWasapiOut.Controls.Add(this.rbExclusive);
            this.gbWasapiOut.Controls.Add(this.rbShare);
            this.gbWasapiOut.Controls.Add(this.cmbWasapiDevice);
            this.gbWasapiOut.Location = new System.Drawing.Point(7, 139);
            this.gbWasapiOut.Name = "gbWasapiOut";
            this.gbWasapiOut.Size = new System.Drawing.Size(429, 50);
            this.gbWasapiOut.TabIndex = 5;
            this.gbWasapiOut.TabStop = false;
            // 
            // rbExclusive
            // 
            this.rbExclusive.AutoSize = true;
            this.rbExclusive.Location = new System.Drawing.Point(342, 31);
            this.rbExclusive.Name = "rbExclusive";
            this.rbExclusive.Size = new System.Drawing.Size(47, 16);
            this.rbExclusive.TabIndex = 7;
            this.rbExclusive.TabStop = true;
            this.rbExclusive.Text = "排他";
            this.rbExclusive.UseVisualStyleBackColor = true;
            // 
            // rbShare
            // 
            this.rbShare.AutoSize = true;
            this.rbShare.Location = new System.Drawing.Point(342, 9);
            this.rbShare.Name = "rbShare";
            this.rbShare.Size = new System.Drawing.Size(47, 16);
            this.rbShare.TabIndex = 6;
            this.rbShare.TabStop = true;
            this.rbShare.Text = "共有";
            this.rbShare.UseVisualStyleBackColor = true;
            // 
            // cmbWasapiDevice
            // 
            this.cmbWasapiDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWasapiDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWasapiDevice.FormattingEnabled = true;
            this.cmbWasapiDevice.Location = new System.Drawing.Point(6, 18);
            this.cmbWasapiDevice.Name = "cmbWasapiDevice";
            this.cmbWasapiDevice.Size = new System.Drawing.Size(330, 20);
            this.cmbWasapiDevice.TabIndex = 4;
            // 
            // gbDirectSound
            // 
            this.gbDirectSound.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDirectSound.Controls.Add(this.cmbDirectSoundDevice);
            this.gbDirectSound.Location = new System.Drawing.Point(7, 85);
            this.gbDirectSound.Name = "gbDirectSound";
            this.gbDirectSound.Size = new System.Drawing.Size(429, 48);
            this.gbDirectSound.TabIndex = 3;
            this.gbDirectSound.TabStop = false;
            // 
            // cmbDirectSoundDevice
            // 
            this.cmbDirectSoundDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDirectSoundDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirectSoundDevice.FormattingEnabled = true;
            this.cmbDirectSoundDevice.Location = new System.Drawing.Point(6, 18);
            this.cmbDirectSoundDevice.Name = "cmbDirectSoundDevice";
            this.cmbDirectSoundDevice.Size = new System.Drawing.Size(417, 20);
            this.cmbDirectSoundDevice.TabIndex = 2;
            // 
            // tcSetting
            // 
            this.tcSetting.Controls.Add(this.tpOutput);
            this.tcSetting.Controls.Add(this.tpModule);
            this.tcSetting.Controls.Add(this.tpNuked);
            this.tcSetting.Controls.Add(this.tpNSF);
            this.tcSetting.Controls.Add(this.tpSID);
            this.tcSetting.Controls.Add(this.tpMIDIOut);
            this.tcSetting.Controls.Add(this.tpMIDIOut2);
            this.tcSetting.Controls.Add(this.tabMIDIExp);
            this.tcSetting.Controls.Add(this.tpMIDIKBD);
            this.tcSetting.Controls.Add(this.tpKeyBoard);
            this.tcSetting.Controls.Add(this.tpBalance);
            this.tcSetting.Controls.Add(this.tpOther);
            this.tcSetting.Controls.Add(this.tpOmake);
            this.tcSetting.Controls.Add(this.tpAbout);
            this.tcSetting.HotTrack = true;
            this.tcSetting.Location = new System.Drawing.Point(1, 3);
            this.tcSetting.Name = "tcSetting";
            this.tcSetting.SelectedIndex = 0;
            this.tcSetting.Size = new System.Drawing.Size(451, 397);
            this.tcSetting.TabIndex = 2;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.rbNullDevice);
            this.tpOutput.Controls.Add(this.label36);
            this.tpOutput.Controls.Add(this.lblWaitTime);
            this.tpOutput.Controls.Add(this.lblLatencyUnit);
            this.tpOutput.Controls.Add(this.label28);
            this.tpOutput.Controls.Add(this.lblLatency);
            this.tpOutput.Controls.Add(this.cmbWaitTime);
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
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(443, 371);
            this.tpOutput.TabIndex = 0;
            this.tpOutput.Text = "出力";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // rbNullDevice
            // 
            this.rbNullDevice.Location = new System.Drawing.Point(274, 261);
            this.rbNullDevice.Name = "rbNullDevice";
            this.rbNullDevice.Size = new System.Drawing.Size(122, 29);
            this.rbNullDevice.TabIndex = 2;
            this.rbNullDevice.Text = "NULL(サウンドデバイスを使用しない)";
            this.rbNullDevice.UseVisualStyleBackColor = true;
            this.rbNullDevice.CheckedChanged += new System.EventHandler(this.rbDirectSoundOut_CheckedChanged);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(7, 7);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(67, 12);
            this.label36.TabIndex = 3;
            this.label36.Text = "出力デバイス";
            // 
            // lblWaitTime
            // 
            this.lblWaitTime.AutoSize = true;
            this.lblWaitTime.Location = new System.Drawing.Point(285, 347);
            this.lblWaitTime.Name = "lblWaitTime";
            this.lblWaitTime.Size = new System.Drawing.Size(20, 12);
            this.lblWaitTime.TabIndex = 9;
            this.lblWaitTime.Text = "ms";
            // 
            // lblLatencyUnit
            // 
            this.lblLatencyUnit.AutoSize = true;
            this.lblLatencyUnit.Location = new System.Drawing.Point(285, 321);
            this.lblLatencyUnit.Name = "lblLatencyUnit";
            this.lblLatencyUnit.Size = new System.Drawing.Size(20, 12);
            this.lblLatencyUnit.TabIndex = 9;
            this.lblLatencyUnit.Text = "ms";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(7, 347);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(98, 12);
            this.label28.TabIndex = 9;
            this.label28.Text = "演奏開始待ち時間";
            // 
            // lblLatency
            // 
            this.lblLatency.AutoSize = true;
            this.lblLatency.Location = new System.Drawing.Point(7, 321);
            this.lblLatency.Name = "lblLatency";
            this.lblLatency.Size = new System.Drawing.Size(145, 12);
            this.lblLatency.TabIndex = 9;
            this.lblLatency.Text = "遅延時間(レンダリングバッファ)";
            // 
            // cmbWaitTime
            // 
            this.cmbWaitTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaitTime.FormattingEnabled = true;
            this.cmbWaitTime.Items.AddRange(new object[] {
            "0",
            "500",
            "1000",
            "1500",
            "2000",
            "2500",
            "3000",
            "3500",
            "4000",
            "4500",
            "5000"});
            this.cmbWaitTime.Location = new System.Drawing.Point(158, 344);
            this.cmbWaitTime.Name = "cmbWaitTime";
            this.cmbWaitTime.Size = new System.Drawing.Size(121, 20);
            this.cmbWaitTime.TabIndex = 8;
            // 
            // cmbLatency
            // 
            this.cmbLatency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLatency.FormattingEnabled = true;
            this.cmbLatency.Items.AddRange(new object[] {
            "25",
            "50",
            "100",
            "150",
            "200",
            "300",
            "400",
            "500"});
            this.cmbLatency.Location = new System.Drawing.Point(158, 318);
            this.cmbLatency.Name = "cmbLatency";
            this.cmbLatency.Size = new System.Drawing.Size(121, 20);
            this.cmbLatency.TabIndex = 8;
            // 
            // rbSPPCM
            // 
            this.rbSPPCM.AutoSize = true;
            this.rbSPPCM.Enabled = false;
            this.rbSPPCM.Location = new System.Drawing.Point(13, 248);
            this.rbSPPCM.Name = "rbSPPCM";
            this.rbSPPCM.Size = new System.Drawing.Size(61, 16);
            this.rbSPPCM.TabIndex = 2;
            this.rbSPPCM.Text = "SPPCM";
            this.rbSPPCM.UseVisualStyleBackColor = true;
            this.rbSPPCM.CheckedChanged += new System.EventHandler(this.rbDirectSoundOut_CheckedChanged);
            // 
            // groupBox16
            // 
            this.groupBox16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox16.Controls.Add(this.cmbSPPCMDevice);
            this.groupBox16.Enabled = false;
            this.groupBox16.Location = new System.Drawing.Point(7, 251);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(228, 48);
            this.groupBox16.TabIndex = 3;
            this.groupBox16.TabStop = false;
            // 
            // cmbSPPCMDevice
            // 
            this.cmbSPPCMDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSPPCMDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSPPCMDevice.FormattingEnabled = true;
            this.cmbSPPCMDevice.Location = new System.Drawing.Point(6, 19);
            this.cmbSPPCMDevice.Name = "cmbSPPCMDevice";
            this.cmbSPPCMDevice.Size = new System.Drawing.Size(216, 20);
            this.cmbSPPCMDevice.TabIndex = 2;
            // 
            // tpModule
            // 
            this.tpModule.Controls.Add(this.groupBox1);
            this.tpModule.Controls.Add(this.groupBox3);
            this.tpModule.Location = new System.Drawing.Point(4, 22);
            this.tpModule.Name = "tpModule";
            this.tpModule.Size = new System.Drawing.Size(443, 371);
            this.tpModule.TabIndex = 3;
            this.tpModule.Text = "音源";
            this.tpModule.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucSI);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(437, 280);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "音源の割り当て";
            // 
            // ucSI
            // 
            this.ucSI.AutoScroll = true;
            this.ucSI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSI.Location = new System.Drawing.Point(3, 15);
            this.ucSI.Name = "ucSI";
            this.ucSI.Size = new System.Drawing.Size(431, 262);
            this.ucSI.TabIndex = 7;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbHiyorimiMode);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.tbLatencyEmu);
            this.groupBox3.Controls.Add(this.tbLatencySCCI);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(3, 289);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(434, 79);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "遅延演奏";
            // 
            // cbHiyorimiMode
            // 
            this.cbHiyorimiMode.AutoSize = true;
            this.cbHiyorimiMode.Location = new System.Drawing.Point(8, 59);
            this.cbHiyorimiMode.Name = "cbHiyorimiMode";
            this.cbHiyorimiMode.Size = new System.Drawing.Size(335, 16);
            this.cbHiyorimiMode.TabIndex = 6;
            this.cbHiyorimiMode.Text = "日和見モード(出力タブ：遅延時間100ms以下の時、使用を推奨)";
            this.cbHiyorimiMode.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "エミュレーション";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 12);
            this.label12.TabIndex = 3;
            this.label12.Text = "Real";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(144, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "ms";
            // 
            // tbLatencyEmu
            // 
            this.tbLatencyEmu.Location = new System.Drawing.Point(85, 15);
            this.tbLatencyEmu.Name = "tbLatencyEmu";
            this.tbLatencyEmu.Size = new System.Drawing.Size(53, 19);
            this.tbLatencyEmu.TabIndex = 1;
            // 
            // tbLatencySCCI
            // 
            this.tbLatencySCCI.Location = new System.Drawing.Point(85, 37);
            this.tbLatencySCCI.Name = "tbLatencySCCI";
            this.tbLatencySCCI.Size = new System.Drawing.Size(53, 19);
            this.tbLatencySCCI.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(144, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "ms";
            // 
            // tpNuked
            // 
            this.tpNuked.Controls.Add(this.groupBox26);
            this.tpNuked.Location = new System.Drawing.Point(4, 22);
            this.tpNuked.Name = "tpNuked";
            this.tpNuked.Size = new System.Drawing.Size(443, 371);
            this.tpNuked.TabIndex = 14;
            this.tpNuked.Text = "Nuked-OPN2";
            this.tpNuked.UseVisualStyleBackColor = true;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionYM2612u);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionYM2612);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionDiscrete);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionASIClp);
            this.groupBox26.Controls.Add(this.rbNukedOPN2OptionASIC);
            this.groupBox26.Location = new System.Drawing.Point(7, 3);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(318, 128);
            this.groupBox26.TabIndex = 0;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Emulation type";
            // 
            // rbNukedOPN2OptionYM2612u
            // 
            this.rbNukedOPN2OptionYM2612u.AutoSize = true;
            this.rbNukedOPN2OptionYM2612u.Location = new System.Drawing.Point(6, 84);
            this.rbNukedOPN2OptionYM2612u.Name = "rbNukedOPN2OptionYM2612u";
            this.rbNukedOPN2OptionYM2612u.Size = new System.Drawing.Size(189, 16);
            this.rbNukedOPN2OptionYM2612u.TabIndex = 0;
            this.rbNukedOPN2OptionYM2612u.TabStop = true;
            this.rbNukedOPN2OptionYM2612u.Text = "YM2612(without filter emulation)";
            this.rbNukedOPN2OptionYM2612u.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionYM2612
            // 
            this.rbNukedOPN2OptionYM2612.AutoSize = true;
            this.rbNukedOPN2OptionYM2612.Location = new System.Drawing.Point(6, 62);
            this.rbNukedOPN2OptionYM2612.Name = "rbNukedOPN2OptionYM2612";
            this.rbNukedOPN2OptionYM2612.Size = new System.Drawing.Size(188, 16);
            this.rbNukedOPN2OptionYM2612.TabIndex = 0;
            this.rbNukedOPN2OptionYM2612.TabStop = true;
            this.rbNukedOPN2OptionYM2612.Text = "YM2612(MD1,MD2 VA2)(default)";
            this.rbNukedOPN2OptionYM2612.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionDiscrete
            // 
            this.rbNukedOPN2OptionDiscrete.AutoSize = true;
            this.rbNukedOPN2OptionDiscrete.Location = new System.Drawing.Point(6, 18);
            this.rbNukedOPN2OptionDiscrete.Name = "rbNukedOPN2OptionDiscrete";
            this.rbNukedOPN2OptionDiscrete.Size = new System.Drawing.Size(122, 16);
            this.rbNukedOPN2OptionDiscrete.TabIndex = 0;
            this.rbNukedOPN2OptionDiscrete.TabStop = true;
            this.rbNukedOPN2OptionDiscrete.Text = "Discrete(Teradrive)";
            this.rbNukedOPN2OptionDiscrete.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIClp
            // 
            this.rbNukedOPN2OptionASIClp.AutoSize = true;
            this.rbNukedOPN2OptionASIClp.Location = new System.Drawing.Point(6, 106);
            this.rbNukedOPN2OptionASIClp.Name = "rbNukedOPN2OptionASIClp";
            this.rbNukedOPN2OptionASIClp.Size = new System.Drawing.Size(151, 16);
            this.rbNukedOPN2OptionASIClp.TabIndex = 0;
            this.rbNukedOPN2OptionASIClp.TabStop = true;
            this.rbNukedOPN2OptionASIClp.Text = "ASIC(with lowpass filter)";
            this.rbNukedOPN2OptionASIClp.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIC
            // 
            this.rbNukedOPN2OptionASIC.AutoSize = true;
            this.rbNukedOPN2OptionASIC.Location = new System.Drawing.Point(6, 40);
            this.rbNukedOPN2OptionASIC.Name = "rbNukedOPN2OptionASIC";
            this.rbNukedOPN2OptionASIC.Size = new System.Drawing.Size(174, 16);
            this.rbNukedOPN2OptionASIC.TabIndex = 0;
            this.rbNukedOPN2OptionASIC.TabStop = true;
            this.rbNukedOPN2OptionASIC.Text = "ASIC(MD1 VA7,MD2,MD3,etc)";
            this.rbNukedOPN2OptionASIC.UseVisualStyleBackColor = true;
            // 
            // tpNSF
            // 
            this.tpNSF.Controls.Add(this.groupBox10);
            this.tpNSF.Controls.Add(this.groupBox12);
            this.tpNSF.Controls.Add(this.groupBox11);
            this.tpNSF.Controls.Add(this.groupBox9);
            this.tpNSF.Controls.Add(this.groupBox8);
            this.tpNSF.Location = new System.Drawing.Point(4, 22);
            this.tpNSF.Name = "tpNSF";
            this.tpNSF.Size = new System.Drawing.Size(443, 371);
            this.tpNSF.TabIndex = 9;
            this.tpNSF.Text = "NSF";
            this.tpNSF.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.cbNSFDmc_TriNull);
            this.groupBox10.Controls.Add(this.cbNSFDmc_TriMute);
            this.groupBox10.Controls.Add(this.cbNSFDmc_RandomizeNoise);
            this.groupBox10.Controls.Add(this.cbNSFDmc_DPCMAntiClick);
            this.groupBox10.Controls.Add(this.cbNSFDmc_EnablePNoise);
            this.groupBox10.Controls.Add(this.cbNSFDmc_Enable4011);
            this.groupBox10.Controls.Add(this.cbNSFDmc_NonLinearMixer);
            this.groupBox10.Controls.Add(this.cbNSFDmc_UnmuteOnReset);
            this.groupBox10.Location = new System.Drawing.Point(224, 3);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(210, 200);
            this.groupBox10.TabIndex = 8;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "DMC";
            // 
            // cbNSFDmc_TriNull
            // 
            this.cbNSFDmc_TriNull.AutoSize = true;
            this.cbNSFDmc_TriNull.Location = new System.Drawing.Point(6, 171);
            this.cbNSFDmc_TriNull.Name = "cbNSFDmc_TriNull";
            this.cbNSFDmc_TriNull.Size = new System.Drawing.Size(64, 16);
            this.cbNSFDmc_TriNull.TabIndex = 7;
            this.cbNSFDmc_TriNull.Text = "TRI null";
            this.cbNSFDmc_TriNull.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_TriMute
            // 
            this.cbNSFDmc_TriMute.AutoSize = true;
            this.cbNSFDmc_TriMute.Location = new System.Drawing.Point(6, 149);
            this.cbNSFDmc_TriMute.Name = "cbNSFDmc_TriMute";
            this.cbNSFDmc_TriMute.Size = new System.Drawing.Size(71, 16);
            this.cbNSFDmc_TriMute.TabIndex = 7;
            this.cbNSFDmc_TriMute.Text = "TRI mute";
            this.cbNSFDmc_TriMute.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_RandomizeNoise
            // 
            this.cbNSFDmc_RandomizeNoise.AutoSize = true;
            this.cbNSFDmc_RandomizeNoise.Location = new System.Drawing.Point(6, 128);
            this.cbNSFDmc_RandomizeNoise.Name = "cbNSFDmc_RandomizeNoise";
            this.cbNSFDmc_RandomizeNoise.Size = new System.Drawing.Size(110, 16);
            this.cbNSFDmc_RandomizeNoise.TabIndex = 7;
            this.cbNSFDmc_RandomizeNoise.Text = "Randomize noise";
            this.cbNSFDmc_RandomizeNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_DPCMAntiClick
            // 
            this.cbNSFDmc_DPCMAntiClick.AutoSize = true;
            this.cbNSFDmc_DPCMAntiClick.Location = new System.Drawing.Point(6, 106);
            this.cbNSFDmc_DPCMAntiClick.Name = "cbNSFDmc_DPCMAntiClick";
            this.cbNSFDmc_DPCMAntiClick.Size = new System.Drawing.Size(107, 16);
            this.cbNSFDmc_DPCMAntiClick.TabIndex = 7;
            this.cbNSFDmc_DPCMAntiClick.Text = "DPCM anti click";
            this.cbNSFDmc_DPCMAntiClick.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_EnablePNoise
            // 
            this.cbNSFDmc_EnablePNoise.AutoSize = true;
            this.cbNSFDmc_EnablePNoise.Location = new System.Drawing.Point(6, 84);
            this.cbNSFDmc_EnablePNoise.Name = "cbNSFDmc_EnablePNoise";
            this.cbNSFDmc_EnablePNoise.Size = new System.Drawing.Size(96, 16);
            this.cbNSFDmc_EnablePNoise.TabIndex = 7;
            this.cbNSFDmc_EnablePNoise.Text = "Enable Pnoise";
            this.cbNSFDmc_EnablePNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_Enable4011
            // 
            this.cbNSFDmc_Enable4011.AutoSize = true;
            this.cbNSFDmc_Enable4011.Location = new System.Drawing.Point(6, 62);
            this.cbNSFDmc_Enable4011.Name = "cbNSFDmc_Enable4011";
            this.cbNSFDmc_Enable4011.Size = new System.Drawing.Size(92, 16);
            this.cbNSFDmc_Enable4011.TabIndex = 7;
            this.cbNSFDmc_Enable4011.Text = "Enable $4011";
            this.cbNSFDmc_Enable4011.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_NonLinearMixer
            // 
            this.cbNSFDmc_NonLinearMixer.AutoSize = true;
            this.cbNSFDmc_NonLinearMixer.Location = new System.Drawing.Point(6, 40);
            this.cbNSFDmc_NonLinearMixer.Name = "cbNSFDmc_NonLinearMixer";
            this.cbNSFDmc_NonLinearMixer.Size = new System.Drawing.Size(110, 16);
            this.cbNSFDmc_NonLinearMixer.TabIndex = 7;
            this.cbNSFDmc_NonLinearMixer.Text = "Non-linear mixer";
            this.cbNSFDmc_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_UnmuteOnReset
            // 
            this.cbNSFDmc_UnmuteOnReset.AutoSize = true;
            this.cbNSFDmc_UnmuteOnReset.Location = new System.Drawing.Point(6, 18);
            this.cbNSFDmc_UnmuteOnReset.Name = "cbNSFDmc_UnmuteOnReset";
            this.cbNSFDmc_UnmuteOnReset.Size = new System.Drawing.Size(109, 16);
            this.cbNSFDmc_UnmuteOnReset.TabIndex = 7;
            this.cbNSFDmc_UnmuteOnReset.Text = "Unmute on reset";
            this.cbNSFDmc_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.cbNSFN160_Serial);
            this.groupBox12.Location = new System.Drawing.Point(224, 209);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(210, 62);
            this.groupBox12.TabIndex = 8;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "N160";
            // 
            // cbNSFN160_Serial
            // 
            this.cbNSFN160_Serial.AutoSize = true;
            this.cbNSFN160_Serial.Location = new System.Drawing.Point(6, 18);
            this.cbNSFN160_Serial.Name = "cbNSFN160_Serial";
            this.cbNSFN160_Serial.Size = new System.Drawing.Size(53, 16);
            this.cbNSFN160_Serial.TabIndex = 7;
            this.cbNSFN160_Serial.Text = "Serial";
            this.cbNSFN160_Serial.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.cbNSFMmc5_PhaseRefresh);
            this.groupBox11.Controls.Add(this.cbNSFMmc5_NonLinearMixer);
            this.groupBox11.Location = new System.Drawing.Point(7, 209);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(210, 62);
            this.groupBox11.TabIndex = 8;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "MMC5";
            // 
            // cbNSFMmc5_PhaseRefresh
            // 
            this.cbNSFMmc5_PhaseRefresh.AutoSize = true;
            this.cbNSFMmc5_PhaseRefresh.Location = new System.Drawing.Point(6, 40);
            this.cbNSFMmc5_PhaseRefresh.Name = "cbNSFMmc5_PhaseRefresh";
            this.cbNSFMmc5_PhaseRefresh.Size = new System.Drawing.Size(95, 16);
            this.cbNSFMmc5_PhaseRefresh.TabIndex = 7;
            this.cbNSFMmc5_PhaseRefresh.Text = "Phase refresh";
            this.cbNSFMmc5_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNSFMmc5_NonLinearMixer
            // 
            this.cbNSFMmc5_NonLinearMixer.AutoSize = true;
            this.cbNSFMmc5_NonLinearMixer.Location = new System.Drawing.Point(6, 18);
            this.cbNSFMmc5_NonLinearMixer.Name = "cbNSFMmc5_NonLinearMixer";
            this.cbNSFMmc5_NonLinearMixer.Size = new System.Drawing.Size(110, 16);
            this.cbNSFMmc5_NonLinearMixer.TabIndex = 7;
            this.cbNSFMmc5_NonLinearMixer.Text = "Non-linear Mixer";
            this.cbNSFMmc5_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.cbNFSNes_DutySwap);
            this.groupBox9.Controls.Add(this.cbNFSNes_PhaseRefresh);
            this.groupBox9.Controls.Add(this.cbNFSNes_NonLinearMixer);
            this.groupBox9.Controls.Add(this.cbNFSNes_UnmuteOnReset);
            this.groupBox9.Location = new System.Drawing.Point(7, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(210, 107);
            this.groupBox9.TabIndex = 8;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "NES";
            // 
            // cbNFSNes_DutySwap
            // 
            this.cbNFSNes_DutySwap.AutoSize = true;
            this.cbNFSNes_DutySwap.Location = new System.Drawing.Point(6, 84);
            this.cbNFSNes_DutySwap.Name = "cbNFSNes_DutySwap";
            this.cbNFSNes_DutySwap.Size = new System.Drawing.Size(78, 16);
            this.cbNFSNes_DutySwap.TabIndex = 7;
            this.cbNFSNes_DutySwap.Text = "Duty swap";
            this.cbNFSNes_DutySwap.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_PhaseRefresh
            // 
            this.cbNFSNes_PhaseRefresh.AutoSize = true;
            this.cbNFSNes_PhaseRefresh.Location = new System.Drawing.Point(6, 62);
            this.cbNFSNes_PhaseRefresh.Name = "cbNFSNes_PhaseRefresh";
            this.cbNFSNes_PhaseRefresh.Size = new System.Drawing.Size(95, 16);
            this.cbNFSNes_PhaseRefresh.TabIndex = 7;
            this.cbNFSNes_PhaseRefresh.Text = "Phase refresh";
            this.cbNFSNes_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_NonLinearMixer
            // 
            this.cbNFSNes_NonLinearMixer.AutoSize = true;
            this.cbNFSNes_NonLinearMixer.Location = new System.Drawing.Point(6, 40);
            this.cbNFSNes_NonLinearMixer.Name = "cbNFSNes_NonLinearMixer";
            this.cbNFSNes_NonLinearMixer.Size = new System.Drawing.Size(110, 16);
            this.cbNFSNes_NonLinearMixer.TabIndex = 7;
            this.cbNFSNes_NonLinearMixer.Text = "Non-linear mixer";
            this.cbNFSNes_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_UnmuteOnReset
            // 
            this.cbNFSNes_UnmuteOnReset.AutoSize = true;
            this.cbNFSNes_UnmuteOnReset.Location = new System.Drawing.Point(6, 18);
            this.cbNFSNes_UnmuteOnReset.Name = "cbNFSNes_UnmuteOnReset";
            this.cbNFSNes_UnmuteOnReset.Size = new System.Drawing.Size(109, 16);
            this.cbNFSNes_UnmuteOnReset.TabIndex = 7;
            this.cbNFSNes_UnmuteOnReset.Text = "Unmute on reset";
            this.cbNFSNes_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label21);
            this.groupBox8.Controls.Add(this.label20);
            this.groupBox8.Controls.Add(this.tbNSFFds_LPF);
            this.groupBox8.Controls.Add(this.cbNFSFds_4085Reset);
            this.groupBox8.Controls.Add(this.cbNSFFDSWriteDisable8000);
            this.groupBox8.Location = new System.Drawing.Point(7, 116);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(210, 87);
            this.groupBox8.TabIndex = 8;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "FDS";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(143, 16);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(18, 12);
            this.label21.TabIndex = 10;
            this.label21.Text = "Hz";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 16);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(25, 12);
            this.label20.TabIndex = 10;
            this.label20.Text = "LPF";
            // 
            // tbNSFFds_LPF
            // 
            this.tbNSFFds_LPF.Location = new System.Drawing.Point(37, 13);
            this.tbNSFFds_LPF.Name = "tbNSFFds_LPF";
            this.tbNSFFds_LPF.Size = new System.Drawing.Size(100, 19);
            this.tbNSFFds_LPF.TabIndex = 9;
            // 
            // cbNFSFds_4085Reset
            // 
            this.cbNFSFds_4085Reset.AutoSize = true;
            this.cbNFSFds_4085Reset.Location = new System.Drawing.Point(6, 38);
            this.cbNFSFds_4085Reset.Name = "cbNFSFds_4085Reset";
            this.cbNFSFds_4085Reset.Size = new System.Drawing.Size(84, 16);
            this.cbNFSFds_4085Reset.TabIndex = 7;
            this.cbNFSFds_4085Reset.Text = "$4085 reset";
            this.cbNFSFds_4085Reset.UseVisualStyleBackColor = true;
            // 
            // cbNSFFDSWriteDisable8000
            // 
            this.cbNSFFDSWriteDisable8000.AutoSize = true;
            this.cbNSFFDSWriteDisable8000.Location = new System.Drawing.Point(6, 60);
            this.cbNSFFDSWriteDisable8000.Name = "cbNSFFDSWriteDisable8000";
            this.cbNSFFDSWriteDisable8000.Size = new System.Drawing.Size(177, 16);
            this.cbNSFFDSWriteDisable8000.TabIndex = 7;
            this.cbNSFFDSWriteDisable8000.Text = "Write disable($8000 - $DFFF)";
            this.cbNSFFDSWriteDisable8000.UseVisualStyleBackColor = true;
            // 
            // tpSID
            // 
            this.tpSID.Controls.Add(this.groupBox14);
            this.tpSID.Controls.Add(this.groupBox13);
            this.tpSID.Controls.Add(this.tbSIDOutputBufferSize);
            this.tpSID.Controls.Add(this.label51);
            this.tpSID.Controls.Add(this.label49);
            this.tpSID.Location = new System.Drawing.Point(4, 22);
            this.tpSID.Name = "tpSID";
            this.tpSID.Size = new System.Drawing.Size(443, 371);
            this.tpSID.TabIndex = 10;
            this.tpSID.Text = "SID";
            this.tpSID.UseVisualStyleBackColor = true;
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
            this.groupBox14.Location = new System.Drawing.Point(7, 109);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(280, 111);
            this.groupBox14.TabIndex = 2;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Quality";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(162, 86);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(58, 12);
            this.label27.TabIndex = 2;
            this.label27.Text = "Low(Light)";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(162, 54);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(38, 12);
            this.label26.TabIndex = 2;
            this.label26.Text = "Middle";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(162, 20);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(68, 12);
            this.label25.TabIndex = 2;
            this.label25.Text = "High(Heavy)";
            // 
            // rdSIDQ1
            // 
            this.rdSIDQ1.AutoSize = true;
            this.rdSIDQ1.Checked = true;
            this.rdSIDQ1.Location = new System.Drawing.Point(6, 84);
            this.rdSIDQ1.Name = "rdSIDQ1";
            this.rdSIDQ1.Size = new System.Drawing.Size(111, 16);
            this.rdSIDQ1.TabIndex = 1;
            this.rdSIDQ1.TabStop = true;
            this.rdSIDQ1.Text = "Interpolate - fast";
            this.rdSIDQ1.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ3
            // 
            this.rdSIDQ3.AutoSize = true;
            this.rdSIDQ3.Location = new System.Drawing.Point(6, 40);
            this.rdSIDQ3.Name = "rdSIDQ3";
            this.rdSIDQ3.Size = new System.Drawing.Size(107, 16);
            this.rdSIDQ3.TabIndex = 1;
            this.rdSIDQ3.Text = "Resample - fast";
            this.rdSIDQ3.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ2
            // 
            this.rdSIDQ2.AutoSize = true;
            this.rdSIDQ2.Location = new System.Drawing.Point(6, 62);
            this.rdSIDQ2.Name = "rdSIDQ2";
            this.rdSIDQ2.Size = new System.Drawing.Size(77, 16);
            this.rdSIDQ2.TabIndex = 1;
            this.rdSIDQ2.Text = "Interpolate";
            this.rdSIDQ2.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ4
            // 
            this.rdSIDQ4.AutoSize = true;
            this.rdSIDQ4.Location = new System.Drawing.Point(6, 18);
            this.rdSIDQ4.Name = "rdSIDQ4";
            this.rdSIDQ4.Size = new System.Drawing.Size(73, 16);
            this.rdSIDQ4.TabIndex = 1;
            this.rdSIDQ4.Text = "Resample";
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
            this.groupBox13.Location = new System.Drawing.Point(7, 3);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(433, 100);
            this.groupBox13.TabIndex = 0;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "ROM Image";
            // 
            // btnSIDBasic
            // 
            this.btnSIDBasic.Location = new System.Drawing.Point(404, 44);
            this.btnSIDBasic.Name = "btnSIDBasic";
            this.btnSIDBasic.Size = new System.Drawing.Size(23, 23);
            this.btnSIDBasic.TabIndex = 2;
            this.btnSIDBasic.Text = "...";
            this.btnSIDBasic.UseVisualStyleBackColor = true;
            this.btnSIDBasic.Click += new System.EventHandler(this.btnSIDBasic_Click);
            // 
            // btnSIDCharacter
            // 
            this.btnSIDCharacter.Location = new System.Drawing.Point(404, 69);
            this.btnSIDCharacter.Name = "btnSIDCharacter";
            this.btnSIDCharacter.Size = new System.Drawing.Size(23, 23);
            this.btnSIDCharacter.TabIndex = 2;
            this.btnSIDCharacter.Text = "...";
            this.btnSIDCharacter.UseVisualStyleBackColor = true;
            this.btnSIDCharacter.Click += new System.EventHandler(this.btnSIDCharacter_Click);
            // 
            // btnSIDKernal
            // 
            this.btnSIDKernal.Location = new System.Drawing.Point(404, 19);
            this.btnSIDKernal.Name = "btnSIDKernal";
            this.btnSIDKernal.Size = new System.Drawing.Size(23, 23);
            this.btnSIDKernal.TabIndex = 2;
            this.btnSIDKernal.Text = "...";
            this.btnSIDKernal.UseVisualStyleBackColor = true;
            this.btnSIDKernal.Click += new System.EventHandler(this.btnSIDKernal_Click);
            // 
            // tbSIDCharacter
            // 
            this.tbSIDCharacter.Location = new System.Drawing.Point(67, 71);
            this.tbSIDCharacter.Name = "tbSIDCharacter";
            this.tbSIDCharacter.Size = new System.Drawing.Size(331, 19);
            this.tbSIDCharacter.TabIndex = 1;
            // 
            // tbSIDBasic
            // 
            this.tbSIDBasic.Location = new System.Drawing.Point(67, 46);
            this.tbSIDBasic.Name = "tbSIDBasic";
            this.tbSIDBasic.Size = new System.Drawing.Size(331, 19);
            this.tbSIDBasic.TabIndex = 1;
            // 
            // tbSIDKernal
            // 
            this.tbSIDKernal.Location = new System.Drawing.Point(67, 21);
            this.tbSIDKernal.Name = "tbSIDKernal";
            this.tbSIDKernal.Size = new System.Drawing.Size(331, 19);
            this.tbSIDKernal.TabIndex = 1;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 74);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(55, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "Character";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 49);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(34, 12);
            this.label23.TabIndex = 0;
            this.label23.Text = "Basic";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 24);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(37, 12);
            this.label22.TabIndex = 0;
            this.label22.Text = "Kernal";
            // 
            // tbSIDOutputBufferSize
            // 
            this.tbSIDOutputBufferSize.Location = new System.Drawing.Point(114, 226);
            this.tbSIDOutputBufferSize.MaxLength = 10;
            this.tbSIDOutputBufferSize.Name = "tbSIDOutputBufferSize";
            this.tbSIDOutputBufferSize.Size = new System.Drawing.Size(93, 19);
            this.tbSIDOutputBufferSize.TabIndex = 1;
            // 
            // label51
            // 
            this.label51.Location = new System.Drawing.Point(213, 224);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(227, 38);
            this.label51.TabIndex = 0;
            this.label51.Text = "テンポが速かったり、音が途切れる場合に調整すると改善することがあります。通常は5000。";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(13, 229);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(95, 12);
            this.label49.TabIndex = 0;
            this.label49.Text = "OutputBuffer size";
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
            this.tpMIDIOut.Location = new System.Drawing.Point(4, 22);
            this.tpMIDIOut.Name = "tpMIDIOut";
            this.tpMIDIOut.Size = new System.Drawing.Size(443, 371);
            this.tpMIDIOut.TabIndex = 8;
            this.tpMIDIOut.Text = "MIDI out";
            this.tpMIDIOut.UseVisualStyleBackColor = true;
            // 
            // btnAddVST
            // 
            this.btnAddVST.Location = new System.Drawing.Point(264, 159);
            this.btnAddVST.Name = "btnAddVST";
            this.btnAddVST.Size = new System.Drawing.Size(75, 23);
            this.btnAddVST.TabIndex = 5;
            this.btnAddVST.Text = "Add VST";
            this.btnAddVST.UseVisualStyleBackColor = true;
            this.btnAddVST.Click += new System.EventHandler(this.btnAddVST_Click);
            // 
            // tbcMIDIoutList
            // 
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
            this.tbcMIDIoutList.Location = new System.Drawing.Point(4, 186);
            this.tbcMIDIoutList.Name = "tbcMIDIoutList";
            this.tbcMIDIoutList.SelectedIndex = 0;
            this.tbcMIDIoutList.Size = new System.Drawing.Size(436, 182);
            this.tbcMIDIoutList.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvMIDIoutListA);
            this.tabPage1.Controls.Add(this.btnUP_A);
            this.tabPage1.Controls.Add(this.btnDOWN_A);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(428, 156);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Tag = "0";
            this.tabPage1.Text = "A";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListA
            // 
            this.dgvMIDIoutListA.AllowUserToAddRows = false;
            this.dgvMIDIoutListA.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListA.AllowUserToResizeRows = false;
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
            this.dgvMIDIoutListA.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListA.MultiSelect = false;
            this.dgvMIDIoutListA.Name = "dgvMIDIoutListA";
            this.dgvMIDIoutListA.RowHeadersVisible = false;
            this.dgvMIDIoutListA.RowTemplate.Height = 21;
            this.dgvMIDIoutListA.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListA.Size = new System.Drawing.Size(394, 153);
            this.dgvMIDIoutListA.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Visible = false;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // clmIsVST
            // 
            this.clmIsVST.HeaderText = "IsVST";
            this.clmIsVST.Name = "clmIsVST";
            this.clmIsVST.Visible = false;
            // 
            // clmFileName
            // 
            this.clmFileName.HeaderText = "fileName";
            this.clmFileName.Name = "clmFileName";
            this.clmFileName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmFileName.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Device Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 180;
            // 
            // clmType
            // 
            this.clmType.HeaderText = "Type";
            this.clmType.Items.AddRange(new object[] {
            "GM",
            "XG",
            "GS",
            "LA",
            "GS(SC-55_1)",
            "GS(SC-55_2)"});
            this.clmType.Name = "clmType";
            this.clmType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmType.Width = 70;
            // 
            // ClmBeforeSend
            // 
            this.ClmBeforeSend.HeaderText = "Before Send";
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
            this.dataGridViewTextBoxColumn3.HeaderText = "Manufacturer";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.HeaderText = "";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnUP_A
            // 
            this.btnUP_A.Location = new System.Drawing.Point(400, 0);
            this.btnUP_A.Name = "btnUP_A";
            this.btnUP_A.Size = new System.Drawing.Size(22, 58);
            this.btnUP_A.TabIndex = 3;
            this.btnUP_A.Text = "↑";
            this.btnUP_A.UseVisualStyleBackColor = true;
            this.btnUP_A.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_A
            // 
            this.btnDOWN_A.Location = new System.Drawing.Point(400, 95);
            this.btnDOWN_A.Name = "btnDOWN_A";
            this.btnDOWN_A.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_A.TabIndex = 3;
            this.btnDOWN_A.Text = "↓";
            this.btnDOWN_A.UseVisualStyleBackColor = true;
            this.btnDOWN_A.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvMIDIoutListB);
            this.tabPage2.Controls.Add(this.btnUP_B);
            this.tabPage2.Controls.Add(this.btnDOWN_B);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(428, 156);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Tag = "1";
            this.tabPage2.Text = "B";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListB
            // 
            this.dgvMIDIoutListB.AllowUserToAddRows = false;
            this.dgvMIDIoutListB.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListB.AllowUserToResizeRows = false;
            this.dgvMIDIoutListB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListB.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListB.MultiSelect = false;
            this.dgvMIDIoutListB.Name = "dgvMIDIoutListB";
            this.dgvMIDIoutListB.RowHeadersVisible = false;
            this.dgvMIDIoutListB.RowTemplate.Height = 21;
            this.dgvMIDIoutListB.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListB.Size = new System.Drawing.Size(394, 153);
            this.dgvMIDIoutListB.TabIndex = 7;
            // 
            // btnUP_B
            // 
            this.btnUP_B.Location = new System.Drawing.Point(400, 0);
            this.btnUP_B.Name = "btnUP_B";
            this.btnUP_B.Size = new System.Drawing.Size(22, 58);
            this.btnUP_B.TabIndex = 5;
            this.btnUP_B.Text = "↑";
            this.btnUP_B.UseVisualStyleBackColor = true;
            this.btnUP_B.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_B
            // 
            this.btnDOWN_B.Location = new System.Drawing.Point(400, 95);
            this.btnDOWN_B.Name = "btnDOWN_B";
            this.btnDOWN_B.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_B.TabIndex = 6;
            this.btnDOWN_B.Text = "↓";
            this.btnDOWN_B.UseVisualStyleBackColor = true;
            this.btnDOWN_B.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvMIDIoutListC);
            this.tabPage3.Controls.Add(this.btnUP_C);
            this.tabPage3.Controls.Add(this.btnDOWN_C);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(428, 156);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Tag = "2";
            this.tabPage3.Text = "C";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListC
            // 
            this.dgvMIDIoutListC.AllowUserToAddRows = false;
            this.dgvMIDIoutListC.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListC.AllowUserToResizeRows = false;
            this.dgvMIDIoutListC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListC.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListC.MultiSelect = false;
            this.dgvMIDIoutListC.Name = "dgvMIDIoutListC";
            this.dgvMIDIoutListC.RowHeadersVisible = false;
            this.dgvMIDIoutListC.RowTemplate.Height = 21;
            this.dgvMIDIoutListC.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListC.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListC.TabIndex = 7;
            // 
            // btnUP_C
            // 
            this.btnUP_C.Location = new System.Drawing.Point(403, 0);
            this.btnUP_C.Name = "btnUP_C";
            this.btnUP_C.Size = new System.Drawing.Size(22, 58);
            this.btnUP_C.TabIndex = 5;
            this.btnUP_C.Text = "↑";
            this.btnUP_C.UseVisualStyleBackColor = true;
            this.btnUP_C.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_C
            // 
            this.btnDOWN_C.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_C.Name = "btnDOWN_C";
            this.btnDOWN_C.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_C.TabIndex = 6;
            this.btnDOWN_C.Text = "↓";
            this.btnDOWN_C.UseVisualStyleBackColor = true;
            this.btnDOWN_C.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dgvMIDIoutListD);
            this.tabPage4.Controls.Add(this.btnUP_D);
            this.tabPage4.Controls.Add(this.btnDOWN_D);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(428, 156);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Tag = "3";
            this.tabPage4.Text = "D";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListD
            // 
            this.dgvMIDIoutListD.AllowUserToAddRows = false;
            this.dgvMIDIoutListD.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListD.AllowUserToResizeRows = false;
            this.dgvMIDIoutListD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListD.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListD.MultiSelect = false;
            this.dgvMIDIoutListD.Name = "dgvMIDIoutListD";
            this.dgvMIDIoutListD.RowHeadersVisible = false;
            this.dgvMIDIoutListD.RowTemplate.Height = 21;
            this.dgvMIDIoutListD.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListD.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListD.TabIndex = 7;
            // 
            // btnUP_D
            // 
            this.btnUP_D.Location = new System.Drawing.Point(403, 0);
            this.btnUP_D.Name = "btnUP_D";
            this.btnUP_D.Size = new System.Drawing.Size(22, 58);
            this.btnUP_D.TabIndex = 5;
            this.btnUP_D.Text = "↑";
            this.btnUP_D.UseVisualStyleBackColor = true;
            this.btnUP_D.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_D
            // 
            this.btnDOWN_D.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_D.Name = "btnDOWN_D";
            this.btnDOWN_D.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_D.TabIndex = 6;
            this.btnDOWN_D.Text = "↓";
            this.btnDOWN_D.UseVisualStyleBackColor = true;
            this.btnDOWN_D.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.dgvMIDIoutListE);
            this.tabPage5.Controls.Add(this.btnUP_E);
            this.tabPage5.Controls.Add(this.btnDOWN_E);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(428, 156);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Tag = "4";
            this.tabPage5.Text = "E";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListE
            // 
            this.dgvMIDIoutListE.AllowUserToAddRows = false;
            this.dgvMIDIoutListE.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListE.AllowUserToResizeRows = false;
            this.dgvMIDIoutListE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListE.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListE.MultiSelect = false;
            this.dgvMIDIoutListE.Name = "dgvMIDIoutListE";
            this.dgvMIDIoutListE.RowHeadersVisible = false;
            this.dgvMIDIoutListE.RowTemplate.Height = 21;
            this.dgvMIDIoutListE.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListE.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListE.TabIndex = 7;
            // 
            // btnUP_E
            // 
            this.btnUP_E.Location = new System.Drawing.Point(403, 0);
            this.btnUP_E.Name = "btnUP_E";
            this.btnUP_E.Size = new System.Drawing.Size(22, 58);
            this.btnUP_E.TabIndex = 5;
            this.btnUP_E.Text = "↑";
            this.btnUP_E.UseVisualStyleBackColor = true;
            this.btnUP_E.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_E
            // 
            this.btnDOWN_E.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_E.Name = "btnDOWN_E";
            this.btnDOWN_E.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_E.TabIndex = 6;
            this.btnDOWN_E.Text = "↓";
            this.btnDOWN_E.UseVisualStyleBackColor = true;
            this.btnDOWN_E.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.dgvMIDIoutListF);
            this.tabPage6.Controls.Add(this.btnUP_F);
            this.tabPage6.Controls.Add(this.btnDOWN_F);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(428, 156);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Tag = "5";
            this.tabPage6.Text = "F";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListF
            // 
            this.dgvMIDIoutListF.AllowUserToAddRows = false;
            this.dgvMIDIoutListF.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListF.AllowUserToResizeRows = false;
            this.dgvMIDIoutListF.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListF.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListF.MultiSelect = false;
            this.dgvMIDIoutListF.Name = "dgvMIDIoutListF";
            this.dgvMIDIoutListF.RowHeadersVisible = false;
            this.dgvMIDIoutListF.RowTemplate.Height = 21;
            this.dgvMIDIoutListF.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListF.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListF.TabIndex = 7;
            // 
            // btnUP_F
            // 
            this.btnUP_F.Location = new System.Drawing.Point(403, 0);
            this.btnUP_F.Name = "btnUP_F";
            this.btnUP_F.Size = new System.Drawing.Size(22, 58);
            this.btnUP_F.TabIndex = 5;
            this.btnUP_F.Text = "↑";
            this.btnUP_F.UseVisualStyleBackColor = true;
            this.btnUP_F.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_F
            // 
            this.btnDOWN_F.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_F.Name = "btnDOWN_F";
            this.btnDOWN_F.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_F.TabIndex = 6;
            this.btnDOWN_F.Text = "↓";
            this.btnDOWN_F.UseVisualStyleBackColor = true;
            this.btnDOWN_F.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dgvMIDIoutListG);
            this.tabPage7.Controls.Add(this.btnUP_G);
            this.tabPage7.Controls.Add(this.btnDOWN_G);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(428, 156);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Tag = "6";
            this.tabPage7.Text = "G";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListG
            // 
            this.dgvMIDIoutListG.AllowUserToAddRows = false;
            this.dgvMIDIoutListG.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListG.AllowUserToResizeRows = false;
            this.dgvMIDIoutListG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListG.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListG.MultiSelect = false;
            this.dgvMIDIoutListG.Name = "dgvMIDIoutListG";
            this.dgvMIDIoutListG.RowHeadersVisible = false;
            this.dgvMIDIoutListG.RowTemplate.Height = 21;
            this.dgvMIDIoutListG.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListG.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListG.TabIndex = 7;
            // 
            // btnUP_G
            // 
            this.btnUP_G.Location = new System.Drawing.Point(403, 0);
            this.btnUP_G.Name = "btnUP_G";
            this.btnUP_G.Size = new System.Drawing.Size(22, 58);
            this.btnUP_G.TabIndex = 5;
            this.btnUP_G.Text = "↑";
            this.btnUP_G.UseVisualStyleBackColor = true;
            this.btnUP_G.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_G
            // 
            this.btnDOWN_G.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_G.Name = "btnDOWN_G";
            this.btnDOWN_G.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_G.TabIndex = 6;
            this.btnDOWN_G.Text = "↓";
            this.btnDOWN_G.UseVisualStyleBackColor = true;
            this.btnDOWN_G.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.dgvMIDIoutListH);
            this.tabPage8.Controls.Add(this.btnUP_H);
            this.tabPage8.Controls.Add(this.btnDOWN_H);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(428, 156);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Tag = "7";
            this.tabPage8.Text = "H";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListH
            // 
            this.dgvMIDIoutListH.AllowUserToAddRows = false;
            this.dgvMIDIoutListH.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListH.AllowUserToResizeRows = false;
            this.dgvMIDIoutListH.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListH.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListH.MultiSelect = false;
            this.dgvMIDIoutListH.Name = "dgvMIDIoutListH";
            this.dgvMIDIoutListH.RowHeadersVisible = false;
            this.dgvMIDIoutListH.RowTemplate.Height = 21;
            this.dgvMIDIoutListH.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListH.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListH.TabIndex = 7;
            // 
            // btnUP_H
            // 
            this.btnUP_H.Location = new System.Drawing.Point(403, 0);
            this.btnUP_H.Name = "btnUP_H";
            this.btnUP_H.Size = new System.Drawing.Size(22, 58);
            this.btnUP_H.TabIndex = 5;
            this.btnUP_H.Text = "↑";
            this.btnUP_H.UseVisualStyleBackColor = true;
            this.btnUP_H.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_H
            // 
            this.btnDOWN_H.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_H.Name = "btnDOWN_H";
            this.btnDOWN_H.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_H.TabIndex = 6;
            this.btnDOWN_H.Text = "↓";
            this.btnDOWN_H.UseVisualStyleBackColor = true;
            this.btnDOWN_H.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.dgvMIDIoutListI);
            this.tabPage9.Controls.Add(this.btnUP_I);
            this.tabPage9.Controls.Add(this.btnDOWN_I);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(428, 156);
            this.tabPage9.TabIndex = 8;
            this.tabPage9.Tag = "8";
            this.tabPage9.Text = "I";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListI
            // 
            this.dgvMIDIoutListI.AllowUserToAddRows = false;
            this.dgvMIDIoutListI.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListI.AllowUserToResizeRows = false;
            this.dgvMIDIoutListI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListI.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListI.MultiSelect = false;
            this.dgvMIDIoutListI.Name = "dgvMIDIoutListI";
            this.dgvMIDIoutListI.RowHeadersVisible = false;
            this.dgvMIDIoutListI.RowTemplate.Height = 21;
            this.dgvMIDIoutListI.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListI.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListI.TabIndex = 7;
            // 
            // btnUP_I
            // 
            this.btnUP_I.Location = new System.Drawing.Point(403, 0);
            this.btnUP_I.Name = "btnUP_I";
            this.btnUP_I.Size = new System.Drawing.Size(22, 58);
            this.btnUP_I.TabIndex = 5;
            this.btnUP_I.Text = "↑";
            this.btnUP_I.UseVisualStyleBackColor = true;
            this.btnUP_I.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_I
            // 
            this.btnDOWN_I.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_I.Name = "btnDOWN_I";
            this.btnDOWN_I.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_I.TabIndex = 6;
            this.btnDOWN_I.Text = "↓";
            this.btnDOWN_I.UseVisualStyleBackColor = true;
            this.btnDOWN_I.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.dgvMIDIoutListJ);
            this.tabPage10.Controls.Add(this.button17);
            this.tabPage10.Controls.Add(this.btnDOWN_J);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Size = new System.Drawing.Size(428, 156);
            this.tabPage10.TabIndex = 9;
            this.tabPage10.Tag = "9";
            this.tabPage10.Text = "J";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListJ
            // 
            this.dgvMIDIoutListJ.AllowUserToAddRows = false;
            this.dgvMIDIoutListJ.AllowUserToDeleteRows = false;
            this.dgvMIDIoutListJ.AllowUserToResizeRows = false;
            this.dgvMIDIoutListJ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutListJ.Location = new System.Drawing.Point(0, 0);
            this.dgvMIDIoutListJ.MultiSelect = false;
            this.dgvMIDIoutListJ.Name = "dgvMIDIoutListJ";
            this.dgvMIDIoutListJ.RowHeadersVisible = false;
            this.dgvMIDIoutListJ.RowTemplate.Height = 21;
            this.dgvMIDIoutListJ.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutListJ.Size = new System.Drawing.Size(397, 153);
            this.dgvMIDIoutListJ.TabIndex = 7;
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(403, 0);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(22, 58);
            this.button17.TabIndex = 5;
            this.button17.Text = "↑";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDOWN_J
            // 
            this.btnDOWN_J.Location = new System.Drawing.Point(403, 95);
            this.btnDOWN_J.Name = "btnDOWN_J";
            this.btnDOWN_J.Size = new System.Drawing.Size(22, 58);
            this.btnDOWN_J.TabIndex = 6;
            this.btnDOWN_J.Text = "↓";
            this.btnDOWN_J.UseVisualStyleBackColor = true;
            this.btnDOWN_J.Click += new System.EventHandler(this.btnDOWN_Click);
            // 
            // btnSubMIDIout
            // 
            this.btnSubMIDIout.Location = new System.Drawing.Point(191, 159);
            this.btnSubMIDIout.Name = "btnSubMIDIout";
            this.btnSubMIDIout.Size = new System.Drawing.Size(66, 24);
            this.btnSubMIDIout.TabIndex = 3;
            this.btnSubMIDIout.Text = "-";
            this.btnSubMIDIout.UseVisualStyleBackColor = true;
            this.btnSubMIDIout.Click += new System.EventHandler(this.btnSubMIDIout_Click);
            // 
            // btnAddMIDIout
            // 
            this.btnAddMIDIout.Location = new System.Drawing.Point(119, 159);
            this.btnAddMIDIout.Name = "btnAddMIDIout";
            this.btnAddMIDIout.Size = new System.Drawing.Size(66, 24);
            this.btnAddMIDIout.TabIndex = 3;
            this.btnAddMIDIout.Text = "↓ +";
            this.btnAddMIDIout.UseVisualStyleBackColor = true;
            this.btnAddMIDIout.Click += new System.EventHandler(this.btnAddMIDIout_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(7, 171);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 12);
            this.label18.TabIndex = 2;
            this.label18.Text = "MIDI Outリスト";
            // 
            // dgvMIDIoutPallet
            // 
            this.dgvMIDIoutPallet.AllowUserToAddRows = false;
            this.dgvMIDIoutPallet.AllowUserToDeleteRows = false;
            this.dgvMIDIoutPallet.AllowUserToResizeRows = false;
            this.dgvMIDIoutPallet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMIDIoutPallet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmID,
            this.clmDeviceName,
            this.clmManufacturer,
            this.clmSpacer});
            this.dgvMIDIoutPallet.Location = new System.Drawing.Point(4, 20);
            this.dgvMIDIoutPallet.MultiSelect = false;
            this.dgvMIDIoutPallet.Name = "dgvMIDIoutPallet";
            this.dgvMIDIoutPallet.RowHeadersVisible = false;
            this.dgvMIDIoutPallet.RowTemplate.Height = 21;
            this.dgvMIDIoutPallet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMIDIoutPallet.Size = new System.Drawing.Size(436, 133);
            this.dgvMIDIoutPallet.TabIndex = 1;
            // 
            // clmID
            // 
            this.clmID.Frozen = true;
            this.clmID.HeaderText = "ID";
            this.clmID.Name = "clmID";
            this.clmID.ReadOnly = true;
            this.clmID.Visible = false;
            this.clmID.Width = 40;
            // 
            // clmDeviceName
            // 
            this.clmDeviceName.Frozen = true;
            this.clmDeviceName.HeaderText = "Device Name";
            this.clmDeviceName.Name = "clmDeviceName";
            this.clmDeviceName.ReadOnly = true;
            this.clmDeviceName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmDeviceName.Width = 200;
            // 
            // clmManufacturer
            // 
            this.clmManufacturer.Frozen = true;
            this.clmManufacturer.HeaderText = "Manufacturer";
            this.clmManufacturer.Name = "clmManufacturer";
            this.clmManufacturer.ReadOnly = true;
            this.clmManufacturer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSpacer
            // 
            this.clmSpacer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmSpacer.HeaderText = "";
            this.clmSpacer.Name = "clmSpacer";
            this.clmSpacer.ReadOnly = true;
            this.clmSpacer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(126, 12);
            this.label16.TabIndex = 0;
            this.label16.Text = "MIDI Outデバイス パレット";
            // 
            // tpMIDIOut2
            // 
            this.tpMIDIOut2.Controls.Add(this.groupBox15);
            this.tpMIDIOut2.Location = new System.Drawing.Point(4, 22);
            this.tpMIDIOut2.Name = "tpMIDIOut2";
            this.tpMIDIOut2.Size = new System.Drawing.Size(443, 371);
            this.tpMIDIOut2.TabIndex = 11;
            this.tpMIDIOut2.Text = "MIDI out2";
            this.tpMIDIOut2.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
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
            this.groupBox15.Location = new System.Drawing.Point(7, 3);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(433, 199);
            this.groupBox15.TabIndex = 0;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Before Send";
            // 
            // btnBeforeSend_Default
            // 
            this.btnBeforeSend_Default.Location = new System.Drawing.Point(352, 123);
            this.btnBeforeSend_Default.Name = "btnBeforeSend_Default";
            this.btnBeforeSend_Default.Size = new System.Drawing.Size(75, 23);
            this.btnBeforeSend_Default.TabIndex = 2;
            this.btnBeforeSend_Default.Text = "元に戻す";
            this.btnBeforeSend_Default.UseVisualStyleBackColor = true;
            this.btnBeforeSend_Default.Click += new System.EventHandler(this.btnBeforeSend_Default_Click);
            // 
            // tbBeforeSend_Custom
            // 
            this.tbBeforeSend_Custom.Location = new System.Drawing.Point(90, 98);
            this.tbBeforeSend_Custom.Name = "tbBeforeSend_Custom";
            this.tbBeforeSend_Custom.Size = new System.Drawing.Size(337, 19);
            this.tbBeforeSend_Custom.TabIndex = 1;
            // 
            // tbBeforeSend_XGReset
            // 
            this.tbBeforeSend_XGReset.Location = new System.Drawing.Point(90, 48);
            this.tbBeforeSend_XGReset.Name = "tbBeforeSend_XGReset";
            this.tbBeforeSend_XGReset.Size = new System.Drawing.Size(337, 19);
            this.tbBeforeSend_XGReset.TabIndex = 1;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(6, 128);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(329, 28);
            this.label35.TabIndex = 0;
            this.label35.Text = "Format:\r\n  (delayTime(dec)):(command data(hex)),...;...\r\n";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 101);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(44, 12);
            this.label34.TabIndex = 0;
            this.label34.Text = "Custom";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 51);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(76, 12);
            this.label32.TabIndex = 0;
            this.label32.Text = "XG SystemOn";
            // 
            // tbBeforeSend_GSReset
            // 
            this.tbBeforeSend_GSReset.Location = new System.Drawing.Point(90, 73);
            this.tbBeforeSend_GSReset.Name = "tbBeforeSend_GSReset";
            this.tbBeforeSend_GSReset.Size = new System.Drawing.Size(337, 19);
            this.tbBeforeSend_GSReset.TabIndex = 1;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 76);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(54, 12);
            this.label33.TabIndex = 0;
            this.label33.Text = "GS Reset";
            // 
            // tbBeforeSend_GMReset
            // 
            this.tbBeforeSend_GMReset.Location = new System.Drawing.Point(90, 23);
            this.tbBeforeSend_GMReset.Name = "tbBeforeSend_GMReset";
            this.tbBeforeSend_GMReset.Size = new System.Drawing.Size(337, 19);
            this.tbBeforeSend_GMReset.TabIndex = 1;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(6, 26);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(78, 12);
            this.label31.TabIndex = 0;
            this.label31.Text = "GM SystemOn";
            // 
            // tabMIDIExp
            // 
            this.tabMIDIExp.Controls.Add(this.cbUseMIDIExport);
            this.tabMIDIExp.Controls.Add(this.gbMIDIExport);
            this.tabMIDIExp.Location = new System.Drawing.Point(4, 22);
            this.tabMIDIExp.Name = "tabMIDIExp";
            this.tabMIDIExp.Size = new System.Drawing.Size(443, 371);
            this.tabMIDIExp.TabIndex = 6;
            this.tabMIDIExp.Text = "MIDIExport";
            this.tabMIDIExp.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIExport
            // 
            this.cbUseMIDIExport.AutoSize = true;
            this.cbUseMIDIExport.Location = new System.Drawing.Point(15, 3);
            this.cbUseMIDIExport.Name = "cbUseMIDIExport";
            this.cbUseMIDIExport.Size = new System.Drawing.Size(177, 16);
            this.cbUseMIDIExport.TabIndex = 1;
            this.cbUseMIDIExport.Text = "演奏時MIDIファイルをexportする";
            this.cbUseMIDIExport.UseVisualStyleBackColor = true;
            this.cbUseMIDIExport.CheckedChanged += new System.EventHandler(this.cbUseMIDIExport_CheckedChanged);
            // 
            // gbMIDIExport
            // 
            this.gbMIDIExport.Controls.Add(this.cbMIDIKeyOnFnum);
            this.gbMIDIExport.Controls.Add(this.cbMIDIUseVOPM);
            this.gbMIDIExport.Controls.Add(this.groupBox6);
            this.gbMIDIExport.Controls.Add(this.cbMIDIPlayless);
            this.gbMIDIExport.Controls.Add(this.btnMIDIOutputPath);
            this.gbMIDIExport.Controls.Add(this.lblOutputPath);
            this.gbMIDIExport.Controls.Add(this.tbMIDIOutputPath);
            this.gbMIDIExport.Location = new System.Drawing.Point(7, 3);
            this.gbMIDIExport.Name = "gbMIDIExport";
            this.gbMIDIExport.Size = new System.Drawing.Size(433, 365);
            this.gbMIDIExport.TabIndex = 0;
            this.gbMIDIExport.TabStop = false;
            // 
            // cbMIDIKeyOnFnum
            // 
            this.cbMIDIKeyOnFnum.AutoSize = true;
            this.cbMIDIKeyOnFnum.Location = new System.Drawing.Point(21, 66);
            this.cbMIDIKeyOnFnum.Name = "cbMIDIKeyOnFnum";
            this.cbMIDIKeyOnFnum.Size = new System.Drawing.Size(169, 16);
            this.cbMIDIKeyOnFnum.TabIndex = 23;
            this.cbMIDIKeyOnFnum.Text = "KeyON時のみfnumを評価する";
            this.cbMIDIKeyOnFnum.UseVisualStyleBackColor = true;
            // 
            // cbMIDIUseVOPM
            // 
            this.cbMIDIUseVOPM.AutoSize = true;
            this.cbMIDIUseVOPM.Location = new System.Drawing.Point(21, 44);
            this.cbMIDIUseVOPM.Name = "cbMIDIUseVOPM";
            this.cbMIDIUseVOPM.Size = new System.Drawing.Size(196, 16);
            this.cbMIDIUseVOPM.TabIndex = 23;
            this.cbMIDIUseVOPM.Text = "VOPMex向けコントロールを出力する";
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
            this.groupBox6.Location = new System.Drawing.Point(20, 113);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(188, 152);
            this.groupBox6.TabIndex = 22;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "出力対象音源";
            // 
            // cbMIDIYM2612
            // 
            this.cbMIDIYM2612.AutoSize = true;
            this.cbMIDIYM2612.Checked = true;
            this.cbMIDIYM2612.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMIDIYM2612.Location = new System.Drawing.Point(6, 18);
            this.cbMIDIYM2612.Name = "cbMIDIYM2612";
            this.cbMIDIYM2612.Size = new System.Drawing.Size(64, 16);
            this.cbMIDIYM2612.TabIndex = 21;
            this.cbMIDIYM2612.Text = "YM2612";
            this.cbMIDIYM2612.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489Sec
            // 
            this.cbMIDISN76489Sec.AutoSize = true;
            this.cbMIDISN76489Sec.Enabled = false;
            this.cbMIDISN76489Sec.Location = new System.Drawing.Point(84, 128);
            this.cbMIDISN76489Sec.Name = "cbMIDISN76489Sec";
            this.cbMIDISN76489Sec.Size = new System.Drawing.Size(96, 16);
            this.cbMIDISN76489Sec.TabIndex = 21;
            this.cbMIDISN76489Sec.Text = "SN76489(Sec)";
            this.cbMIDISN76489Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2612Sec
            // 
            this.cbMIDIYM2612Sec.AutoSize = true;
            this.cbMIDIYM2612Sec.Enabled = false;
            this.cbMIDIYM2612Sec.Location = new System.Drawing.Point(84, 18);
            this.cbMIDIYM2612Sec.Name = "cbMIDIYM2612Sec";
            this.cbMIDIYM2612Sec.Size = new System.Drawing.Size(91, 16);
            this.cbMIDIYM2612Sec.TabIndex = 21;
            this.cbMIDIYM2612Sec.Text = "YM2612(Sec)";
            this.cbMIDIYM2612Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489
            // 
            this.cbMIDISN76489.AutoSize = true;
            this.cbMIDISN76489.Enabled = false;
            this.cbMIDISN76489.Location = new System.Drawing.Point(6, 128);
            this.cbMIDISN76489.Name = "cbMIDISN76489";
            this.cbMIDISN76489.Size = new System.Drawing.Size(69, 16);
            this.cbMIDISN76489.TabIndex = 21;
            this.cbMIDISN76489.Text = "SN76489";
            this.cbMIDISN76489.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151
            // 
            this.cbMIDIYM2151.AutoSize = true;
            this.cbMIDIYM2151.Location = new System.Drawing.Point(6, 40);
            this.cbMIDIYM2151.Name = "cbMIDIYM2151";
            this.cbMIDIYM2151.Size = new System.Drawing.Size(64, 16);
            this.cbMIDIYM2151.TabIndex = 21;
            this.cbMIDIYM2151.Text = "YM2151";
            this.cbMIDIYM2151.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610BSec
            // 
            this.cbMIDIYM2610BSec.AutoSize = true;
            this.cbMIDIYM2610BSec.Enabled = false;
            this.cbMIDIYM2610BSec.Location = new System.Drawing.Point(84, 106);
            this.cbMIDIYM2610BSec.Name = "cbMIDIYM2610BSec";
            this.cbMIDIYM2610BSec.Size = new System.Drawing.Size(99, 16);
            this.cbMIDIYM2610BSec.TabIndex = 21;
            this.cbMIDIYM2610BSec.Text = "YM2610B(Sec)";
            this.cbMIDIYM2610BSec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151Sec
            // 
            this.cbMIDIYM2151Sec.AutoSize = true;
            this.cbMIDIYM2151Sec.Enabled = false;
            this.cbMIDIYM2151Sec.Location = new System.Drawing.Point(84, 40);
            this.cbMIDIYM2151Sec.Name = "cbMIDIYM2151Sec";
            this.cbMIDIYM2151Sec.Size = new System.Drawing.Size(91, 16);
            this.cbMIDIYM2151Sec.TabIndex = 21;
            this.cbMIDIYM2151Sec.Text = "YM2151(Sec)";
            this.cbMIDIYM2151Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610B
            // 
            this.cbMIDIYM2610B.AutoSize = true;
            this.cbMIDIYM2610B.Enabled = false;
            this.cbMIDIYM2610B.Location = new System.Drawing.Point(6, 106);
            this.cbMIDIYM2610B.Name = "cbMIDIYM2610B";
            this.cbMIDIYM2610B.Size = new System.Drawing.Size(72, 16);
            this.cbMIDIYM2610B.TabIndex = 21;
            this.cbMIDIYM2610B.Text = "YM2610B";
            this.cbMIDIYM2610B.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203
            // 
            this.cbMIDIYM2203.AutoSize = true;
            this.cbMIDIYM2203.Enabled = false;
            this.cbMIDIYM2203.Location = new System.Drawing.Point(6, 62);
            this.cbMIDIYM2203.Name = "cbMIDIYM2203";
            this.cbMIDIYM2203.Size = new System.Drawing.Size(64, 16);
            this.cbMIDIYM2203.TabIndex = 21;
            this.cbMIDIYM2203.Text = "YM2203";
            this.cbMIDIYM2203.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608Sec
            // 
            this.cbMIDIYM2608Sec.AutoSize = true;
            this.cbMIDIYM2608Sec.Enabled = false;
            this.cbMIDIYM2608Sec.Location = new System.Drawing.Point(84, 84);
            this.cbMIDIYM2608Sec.Name = "cbMIDIYM2608Sec";
            this.cbMIDIYM2608Sec.Size = new System.Drawing.Size(91, 16);
            this.cbMIDIYM2608Sec.TabIndex = 21;
            this.cbMIDIYM2608Sec.Text = "YM2608(Sec)";
            this.cbMIDIYM2608Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203Sec
            // 
            this.cbMIDIYM2203Sec.AutoSize = true;
            this.cbMIDIYM2203Sec.Enabled = false;
            this.cbMIDIYM2203Sec.Location = new System.Drawing.Point(84, 62);
            this.cbMIDIYM2203Sec.Name = "cbMIDIYM2203Sec";
            this.cbMIDIYM2203Sec.Size = new System.Drawing.Size(91, 16);
            this.cbMIDIYM2203Sec.TabIndex = 21;
            this.cbMIDIYM2203Sec.Text = "YM2203(Sec)";
            this.cbMIDIYM2203Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608
            // 
            this.cbMIDIYM2608.AutoSize = true;
            this.cbMIDIYM2608.Enabled = false;
            this.cbMIDIYM2608.Location = new System.Drawing.Point(6, 84);
            this.cbMIDIYM2608.Name = "cbMIDIYM2608";
            this.cbMIDIYM2608.Size = new System.Drawing.Size(64, 16);
            this.cbMIDIYM2608.TabIndex = 21;
            this.cbMIDIYM2608.Text = "YM2608";
            this.cbMIDIYM2608.UseVisualStyleBackColor = true;
            // 
            // cbMIDIPlayless
            // 
            this.cbMIDIPlayless.AutoSize = true;
            this.cbMIDIPlayless.Enabled = false;
            this.cbMIDIPlayless.Location = new System.Drawing.Point(21, 22);
            this.cbMIDIPlayless.Name = "cbMIDIPlayless";
            this.cbMIDIPlayless.Size = new System.Drawing.Size(141, 16);
            this.cbMIDIPlayless.TabIndex = 20;
            this.cbMIDIPlayless.Text = "演奏を行わずに出力する";
            this.cbMIDIPlayless.UseVisualStyleBackColor = true;
            // 
            // btnMIDIOutputPath
            // 
            this.btnMIDIOutputPath.Location = new System.Drawing.Point(403, 86);
            this.btnMIDIOutputPath.Name = "btnMIDIOutputPath";
            this.btnMIDIOutputPath.Size = new System.Drawing.Size(23, 23);
            this.btnMIDIOutputPath.TabIndex = 19;
            this.btnMIDIOutputPath.Text = "...";
            this.btnMIDIOutputPath.UseVisualStyleBackColor = true;
            this.btnMIDIOutputPath.Click += new System.EventHandler(this.btnMIDIOutputPath_Click);
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(19, 91);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(52, 12);
            this.lblOutputPath.TabIndex = 17;
            this.lblOutputPath.Text = "出力Path";
            // 
            // tbMIDIOutputPath
            // 
            this.tbMIDIOutputPath.Location = new System.Drawing.Point(77, 88);
            this.tbMIDIOutputPath.Name = "tbMIDIOutputPath";
            this.tbMIDIOutputPath.Size = new System.Drawing.Size(320, 19);
            this.tbMIDIOutputPath.TabIndex = 18;
            // 
            // tpMIDIKBD
            // 
            this.tpMIDIKBD.Controls.Add(this.cbUseMIDIKeyboard);
            this.tpMIDIKBD.Controls.Add(this.gbMIDIKeyboard);
            this.tpMIDIKBD.Location = new System.Drawing.Point(4, 22);
            this.tpMIDIKBD.Name = "tpMIDIKBD";
            this.tpMIDIKBD.Size = new System.Drawing.Size(443, 371);
            this.tpMIDIKBD.TabIndex = 5;
            this.tpMIDIKBD.Text = "MIDI鍵盤";
            this.tpMIDIKBD.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIKeyboard
            // 
            this.cbUseMIDIKeyboard.AutoSize = true;
            this.cbUseMIDIKeyboard.Location = new System.Drawing.Point(11, 4);
            this.cbUseMIDIKeyboard.Name = "cbUseMIDIKeyboard";
            this.cbUseMIDIKeyboard.Size = new System.Drawing.Size(124, 16);
            this.cbUseMIDIKeyboard.TabIndex = 1;
            this.cbUseMIDIKeyboard.Text = "MIDIキーボードを使う";
            this.cbUseMIDIKeyboard.UseVisualStyleBackColor = true;
            this.cbUseMIDIKeyboard.CheckedChanged += new System.EventHandler(this.cbUseMIDIKeyboard_CheckedChanged);
            // 
            // gbMIDIKeyboard
            // 
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
            this.gbMIDIKeyboard.Enabled = false;
            this.gbMIDIKeyboard.Location = new System.Drawing.Point(3, 6);
            this.gbMIDIKeyboard.Name = "gbMIDIKeyboard";
            this.gbMIDIKeyboard.Size = new System.Drawing.Size(437, 362);
            this.gbMIDIKeyboard.TabIndex = 0;
            this.gbMIDIKeyboard.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::MDPlayer.Properties.Resources.ccNext;
            this.pictureBox8.Location = new System.Drawing.Point(371, 257);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(16, 16);
            this.pictureBox8.TabIndex = 4;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::MDPlayer.Properties.Resources.ccFast;
            this.pictureBox7.Location = new System.Drawing.Point(261, 257);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(16, 16);
            this.pictureBox7.TabIndex = 4;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::MDPlayer.Properties.Resources.ccPlay;
            this.pictureBox6.Location = new System.Drawing.Point(152, 258);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(16, 16);
            this.pictureBox6.TabIndex = 4;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::MDPlayer.Properties.Resources.ccSlow;
            this.pictureBox5.Location = new System.Drawing.Point(42, 258);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(16, 16);
            this.pictureBox5.TabIndex = 4;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::MDPlayer.Properties.Resources.ccStop;
            this.pictureBox4.Location = new System.Drawing.Point(42, 234);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(16, 16);
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::MDPlayer.Properties.Resources.ccPause;
            this.pictureBox3.Location = new System.Drawing.Point(152, 234);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(16, 16);
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MDPlayer.Properties.Resources.ccPrevious;
            this.pictureBox2.Location = new System.Drawing.Point(371, 234);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MDPlayer.Properties.Resources.ccFadeout;
            this.pictureBox1.Location = new System.Drawing.Point(261, 234);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // tbCCFadeout
            // 
            this.tbCCFadeout.Location = new System.Drawing.Point(225, 232);
            this.tbCCFadeout.MaxLength = 3;
            this.tbCCFadeout.Name = "tbCCFadeout";
            this.tbCCFadeout.Size = new System.Drawing.Size(30, 19);
            this.tbCCFadeout.TabIndex = 12;
            // 
            // tbCCPause
            // 
            this.tbCCPause.Location = new System.Drawing.Point(116, 232);
            this.tbCCPause.MaxLength = 3;
            this.tbCCPause.Name = "tbCCPause";
            this.tbCCPause.Size = new System.Drawing.Size(30, 19);
            this.tbCCPause.TabIndex = 11;
            // 
            // tbCCSlow
            // 
            this.tbCCSlow.Location = new System.Drawing.Point(6, 257);
            this.tbCCSlow.MaxLength = 3;
            this.tbCCSlow.Name = "tbCCSlow";
            this.tbCCSlow.Size = new System.Drawing.Size(30, 19);
            this.tbCCSlow.TabIndex = 14;
            // 
            // tbCCPrevious
            // 
            this.tbCCPrevious.Location = new System.Drawing.Point(335, 232);
            this.tbCCPrevious.MaxLength = 3;
            this.tbCCPrevious.Name = "tbCCPrevious";
            this.tbCCPrevious.Size = new System.Drawing.Size(30, 19);
            this.tbCCPrevious.TabIndex = 13;
            // 
            // tbCCNext
            // 
            this.tbCCNext.Location = new System.Drawing.Point(335, 257);
            this.tbCCNext.MaxLength = 3;
            this.tbCCNext.Name = "tbCCNext";
            this.tbCCNext.Size = new System.Drawing.Size(30, 19);
            this.tbCCNext.TabIndex = 17;
            // 
            // tbCCFast
            // 
            this.tbCCFast.Location = new System.Drawing.Point(225, 257);
            this.tbCCFast.MaxLength = 3;
            this.tbCCFast.Name = "tbCCFast";
            this.tbCCFast.Size = new System.Drawing.Size(30, 19);
            this.tbCCFast.TabIndex = 16;
            // 
            // tbCCStop
            // 
            this.tbCCStop.Location = new System.Drawing.Point(6, 232);
            this.tbCCStop.MaxLength = 3;
            this.tbCCStop.Name = "tbCCStop";
            this.tbCCStop.Size = new System.Drawing.Size(30, 19);
            this.tbCCStop.TabIndex = 10;
            // 
            // tbCCPlay
            // 
            this.tbCCPlay.Location = new System.Drawing.Point(116, 257);
            this.tbCCPlay.MaxLength = 3;
            this.tbCCPlay.Name = "tbCCPlay";
            this.tbCCPlay.Size = new System.Drawing.Size(30, 19);
            this.tbCCPlay.TabIndex = 15;
            // 
            // tbCCCopyLog
            // 
            this.tbCCCopyLog.Location = new System.Drawing.Point(6, 207);
            this.tbCCCopyLog.MaxLength = 3;
            this.tbCCCopyLog.Name = "tbCCCopyLog";
            this.tbCCCopyLog.Size = new System.Drawing.Size(30, 19);
            this.tbCCCopyLog.TabIndex = 8;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(42, 210);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(230, 12);
            this.label17.TabIndex = 9;
            this.label17.Text = "MONOモード時、選択ログをクリップボードに設定";
            // 
            // tbCCDelLog
            // 
            this.tbCCDelLog.Location = new System.Drawing.Point(6, 182);
            this.tbCCDelLog.MaxLength = 3;
            this.tbCCDelLog.Name = "tbCCDelLog";
            this.tbCCDelLog.Size = new System.Drawing.Size(30, 19);
            this.tbCCDelLog.TabIndex = 6;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(42, 185);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(117, 12);
            this.label15.TabIndex = 7;
            this.label15.Text = "直近のログをひとつ削除";
            // 
            // tbCCChCopy
            // 
            this.tbCCChCopy.Location = new System.Drawing.Point(6, 157);
            this.tbCCChCopy.MaxLength = 3;
            this.tbCCChCopy.Name = "tbCCChCopy";
            this.tbCCChCopy.Size = new System.Drawing.Size(30, 19);
            this.tbCCChCopy.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 142);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(159, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "CC(Control Change)による操作";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(42, 160);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(261, 12);
            this.label9.TabIndex = 5;
            this.label9.Text = "1Chの音色を全てのチャンネルにコピー(選択状況無視)";
            // 
            // gbUseChannel
            // 
            this.gbUseChannel.Controls.Add(this.rbMONO);
            this.gbUseChannel.Controls.Add(this.rbPOLY);
            this.gbUseChannel.Controls.Add(this.groupBox7);
            this.gbUseChannel.Controls.Add(this.groupBox2);
            this.gbUseChannel.Location = new System.Drawing.Point(6, 44);
            this.gbUseChannel.Name = "gbUseChannel";
            this.gbUseChannel.Size = new System.Drawing.Size(425, 86);
            this.gbUseChannel.TabIndex = 2;
            this.gbUseChannel.TabStop = false;
            this.gbUseChannel.Text = "use channel";
            // 
            // rbMONO
            // 
            this.rbMONO.AutoSize = true;
            this.rbMONO.Checked = true;
            this.rbMONO.Location = new System.Drawing.Point(12, 17);
            this.rbMONO.Name = "rbMONO";
            this.rbMONO.Size = new System.Drawing.Size(56, 16);
            this.rbMONO.TabIndex = 1;
            this.rbMONO.TabStop = true;
            this.rbMONO.Text = "MONO";
            this.rbMONO.UseVisualStyleBackColor = true;
            // 
            // rbPOLY
            // 
            this.rbPOLY.AutoSize = true;
            this.rbPOLY.Location = new System.Drawing.Point(215, 17);
            this.rbPOLY.Name = "rbPOLY";
            this.rbPOLY.Size = new System.Drawing.Size(51, 16);
            this.rbPOLY.TabIndex = 3;
            this.rbPOLY.Text = "POLY";
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
            this.groupBox7.Location = new System.Drawing.Point(6, 18);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(197, 62);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            // 
            // rbFM6
            // 
            this.rbFM6.AutoSize = true;
            this.rbFM6.Location = new System.Drawing.Point(108, 40);
            this.rbFM6.Name = "rbFM6";
            this.rbFM6.Size = new System.Drawing.Size(45, 16);
            this.rbFM6.TabIndex = 5;
            this.rbFM6.Text = "FM6";
            this.rbFM6.UseVisualStyleBackColor = true;
            // 
            // rbFM3
            // 
            this.rbFM3.AutoSize = true;
            this.rbFM3.Location = new System.Drawing.Point(108, 18);
            this.rbFM3.Name = "rbFM3";
            this.rbFM3.Size = new System.Drawing.Size(45, 16);
            this.rbFM3.TabIndex = 2;
            this.rbFM3.Text = "FM3";
            this.rbFM3.UseVisualStyleBackColor = true;
            // 
            // rbFM5
            // 
            this.rbFM5.AutoSize = true;
            this.rbFM5.Location = new System.Drawing.Point(57, 40);
            this.rbFM5.Name = "rbFM5";
            this.rbFM5.Size = new System.Drawing.Size(45, 16);
            this.rbFM5.TabIndex = 4;
            this.rbFM5.Text = "FM5";
            this.rbFM5.UseVisualStyleBackColor = true;
            // 
            // rbFM2
            // 
            this.rbFM2.AutoSize = true;
            this.rbFM2.Location = new System.Drawing.Point(57, 18);
            this.rbFM2.Name = "rbFM2";
            this.rbFM2.Size = new System.Drawing.Size(45, 16);
            this.rbFM2.TabIndex = 1;
            this.rbFM2.Text = "FM2";
            this.rbFM2.UseVisualStyleBackColor = true;
            // 
            // rbFM4
            // 
            this.rbFM4.AutoSize = true;
            this.rbFM4.Location = new System.Drawing.Point(6, 40);
            this.rbFM4.Name = "rbFM4";
            this.rbFM4.Size = new System.Drawing.Size(45, 16);
            this.rbFM4.TabIndex = 3;
            this.rbFM4.Text = "FM4";
            this.rbFM4.UseVisualStyleBackColor = true;
            // 
            // rbFM1
            // 
            this.rbFM1.AutoSize = true;
            this.rbFM1.Checked = true;
            this.rbFM1.Location = new System.Drawing.Point(6, 18);
            this.rbFM1.Name = "rbFM1";
            this.rbFM1.Size = new System.Drawing.Size(45, 16);
            this.rbFM1.TabIndex = 0;
            this.rbFM1.TabStop = true;
            this.rbFM1.Text = "FM1";
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
            this.groupBox2.Location = new System.Drawing.Point(209, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 62);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // cbFM1
            // 
            this.cbFM1.AutoSize = true;
            this.cbFM1.Checked = true;
            this.cbFM1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM1.Location = new System.Drawing.Point(6, 18);
            this.cbFM1.Name = "cbFM1";
            this.cbFM1.Size = new System.Drawing.Size(46, 16);
            this.cbFM1.TabIndex = 0;
            this.cbFM1.Text = "FM1";
            this.cbFM1.UseVisualStyleBackColor = true;
            // 
            // cbFM6
            // 
            this.cbFM6.AutoSize = true;
            this.cbFM6.Checked = true;
            this.cbFM6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM6.Location = new System.Drawing.Point(110, 40);
            this.cbFM6.Name = "cbFM6";
            this.cbFM6.Size = new System.Drawing.Size(46, 16);
            this.cbFM6.TabIndex = 5;
            this.cbFM6.Text = "FM6";
            this.cbFM6.UseVisualStyleBackColor = true;
            // 
            // cbFM2
            // 
            this.cbFM2.AutoSize = true;
            this.cbFM2.Checked = true;
            this.cbFM2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM2.Location = new System.Drawing.Point(58, 18);
            this.cbFM2.Name = "cbFM2";
            this.cbFM2.Size = new System.Drawing.Size(46, 16);
            this.cbFM2.TabIndex = 1;
            this.cbFM2.Text = "FM2";
            this.cbFM2.UseVisualStyleBackColor = true;
            // 
            // cbFM5
            // 
            this.cbFM5.AutoSize = true;
            this.cbFM5.Checked = true;
            this.cbFM5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM5.Location = new System.Drawing.Point(58, 40);
            this.cbFM5.Name = "cbFM5";
            this.cbFM5.Size = new System.Drawing.Size(46, 16);
            this.cbFM5.TabIndex = 4;
            this.cbFM5.Text = "FM5";
            this.cbFM5.UseVisualStyleBackColor = true;
            // 
            // cbFM3
            // 
            this.cbFM3.AutoSize = true;
            this.cbFM3.Checked = true;
            this.cbFM3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM3.Location = new System.Drawing.Point(110, 18);
            this.cbFM3.Name = "cbFM3";
            this.cbFM3.Size = new System.Drawing.Size(46, 16);
            this.cbFM3.TabIndex = 2;
            this.cbFM3.Text = "FM3";
            this.cbFM3.UseVisualStyleBackColor = true;
            // 
            // cbFM4
            // 
            this.cbFM4.AutoSize = true;
            this.cbFM4.Checked = true;
            this.cbFM4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFM4.Location = new System.Drawing.Point(6, 40);
            this.cbFM4.Name = "cbFM4";
            this.cbFM4.Size = new System.Drawing.Size(46, 16);
            this.cbFM4.TabIndex = 3;
            this.cbFM4.Text = "FM4";
            this.cbFM4.UseVisualStyleBackColor = true;
            // 
            // cmbMIDIIN
            // 
            this.cmbMIDIIN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMIDIIN.FormattingEnabled = true;
            this.cmbMIDIIN.Location = new System.Drawing.Point(72, 18);
            this.cmbMIDIIN.Name = "cmbMIDIIN";
            this.cmbMIDIIN.Size = new System.Drawing.Size(359, 20);
            this.cmbMIDIIN.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "MIDI IN";
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
            this.tpKeyBoard.Location = new System.Drawing.Point(4, 22);
            this.tpKeyBoard.Name = "tpKeyBoard";
            this.tpKeyBoard.Size = new System.Drawing.Size(443, 371);
            this.tpKeyBoard.TabIndex = 13;
            this.tpKeyBoard.Text = "キーボード";
            this.tpKeyBoard.UseVisualStyleBackColor = true;
            // 
            // cbUseKeyBoardHook
            // 
            this.cbUseKeyBoardHook.AutoSize = true;
            this.cbUseKeyBoardHook.Location = new System.Drawing.Point(18, 3);
            this.cbUseKeyBoardHook.Name = "cbUseKeyBoardHook";
            this.cbUseKeyBoardHook.Size = new System.Drawing.Size(124, 16);
            this.cbUseKeyBoardHook.TabIndex = 27;
            this.cbUseKeyBoardHook.Text = "キーボードフックを使う";
            this.cbUseKeyBoardHook.UseVisualStyleBackColor = true;
            this.cbUseKeyBoardHook.CheckedChanged += new System.EventHandler(this.cbUseKeyBoardHook_CheckedChanged);
            // 
            // gbUseKeyBoardHook
            // 
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
            this.gbUseKeyBoardHook.Location = new System.Drawing.Point(7, 3);
            this.gbUseKeyBoardHook.Name = "gbUseKeyBoardHook";
            this.gbUseKeyBoardHook.Size = new System.Drawing.Size(433, 305);
            this.gbUseKeyBoardHook.TabIndex = 28;
            this.gbUseKeyBoardHook.TabStop = false;
            // 
            // lblKeyBoardHookNotice
            // 
            this.lblKeyBoardHookNotice.AutoSize = true;
            this.lblKeyBoardHookNotice.ForeColor = System.Drawing.Color.Red;
            this.lblKeyBoardHookNotice.Location = new System.Drawing.Point(6, 277);
            this.lblKeyBoardHookNotice.Name = "lblKeyBoardHookNotice";
            this.lblKeyBoardHookNotice.Size = new System.Drawing.Size(169, 12);
            this.lblKeyBoardHookNotice.TabIndex = 30;
            this.lblKeyBoardHookNotice.Text = "設定したいキーを入力してください。";
            this.lblKeyBoardHookNotice.Visible = false;
            // 
            // btNextClr
            // 
            this.btNextClr.Location = new System.Drawing.Point(391, 243);
            this.btNextClr.Name = "btNextClr";
            this.btNextClr.Size = new System.Drawing.Size(36, 23);
            this.btNextClr.TabIndex = 29;
            this.btNextClr.Text = "Clr";
            this.btNextClr.UseVisualStyleBackColor = true;
            this.btNextClr.Click += new System.EventHandler(this.btNextClr_Click);
            // 
            // btPrevClr
            // 
            this.btPrevClr.Location = new System.Drawing.Point(391, 127);
            this.btPrevClr.Name = "btPrevClr";
            this.btPrevClr.Size = new System.Drawing.Size(36, 23);
            this.btPrevClr.TabIndex = 29;
            this.btPrevClr.Text = "Clr";
            this.btPrevClr.UseVisualStyleBackColor = true;
            this.btPrevClr.Click += new System.EventHandler(this.btPrevClr_Click);
            // 
            // btPlayClr
            // 
            this.btPlayClr.Location = new System.Drawing.Point(391, 185);
            this.btPlayClr.Name = "btPlayClr";
            this.btPlayClr.Size = new System.Drawing.Size(36, 23);
            this.btPlayClr.TabIndex = 29;
            this.btPlayClr.Text = "Clr";
            this.btPlayClr.UseVisualStyleBackColor = true;
            this.btPlayClr.Click += new System.EventHandler(this.btPlayClr_Click);
            // 
            // btPauseClr
            // 
            this.btPauseClr.Location = new System.Drawing.Point(391, 69);
            this.btPauseClr.Name = "btPauseClr";
            this.btPauseClr.Size = new System.Drawing.Size(36, 23);
            this.btPauseClr.TabIndex = 29;
            this.btPauseClr.Text = "Clr";
            this.btPauseClr.UseVisualStyleBackColor = true;
            this.btPauseClr.Click += new System.EventHandler(this.btPauseClr_Click);
            // 
            // btFastClr
            // 
            this.btFastClr.Location = new System.Drawing.Point(391, 214);
            this.btFastClr.Name = "btFastClr";
            this.btFastClr.Size = new System.Drawing.Size(36, 23);
            this.btFastClr.TabIndex = 29;
            this.btFastClr.Text = "Clr";
            this.btFastClr.UseVisualStyleBackColor = true;
            this.btFastClr.Click += new System.EventHandler(this.btFastClr_Click);
            // 
            // btFadeoutClr
            // 
            this.btFadeoutClr.Location = new System.Drawing.Point(391, 98);
            this.btFadeoutClr.Name = "btFadeoutClr";
            this.btFadeoutClr.Size = new System.Drawing.Size(36, 23);
            this.btFadeoutClr.TabIndex = 29;
            this.btFadeoutClr.Text = "Clr";
            this.btFadeoutClr.UseVisualStyleBackColor = true;
            this.btFadeoutClr.Click += new System.EventHandler(this.btFadeoutClr_Click);
            // 
            // btSlowClr
            // 
            this.btSlowClr.Location = new System.Drawing.Point(391, 156);
            this.btSlowClr.Name = "btSlowClr";
            this.btSlowClr.Size = new System.Drawing.Size(36, 23);
            this.btSlowClr.TabIndex = 29;
            this.btSlowClr.Text = "Clr";
            this.btSlowClr.UseVisualStyleBackColor = true;
            this.btSlowClr.Click += new System.EventHandler(this.btSlowClr_Click);
            // 
            // btStopClr
            // 
            this.btStopClr.Location = new System.Drawing.Point(391, 40);
            this.btStopClr.Name = "btStopClr";
            this.btStopClr.Size = new System.Drawing.Size(36, 23);
            this.btStopClr.TabIndex = 29;
            this.btStopClr.Text = "Clr";
            this.btStopClr.UseVisualStyleBackColor = true;
            this.btStopClr.Click += new System.EventHandler(this.btStopClr_Click);
            // 
            // btNextSet
            // 
            this.btNextSet.Location = new System.Drawing.Point(349, 243);
            this.btNextSet.Name = "btNextSet";
            this.btNextSet.Size = new System.Drawing.Size(36, 23);
            this.btNextSet.TabIndex = 29;
            this.btNextSet.Text = "Set";
            this.btNextSet.UseVisualStyleBackColor = true;
            this.btNextSet.Click += new System.EventHandler(this.btNextSet_Click);
            // 
            // btPrevSet
            // 
            this.btPrevSet.Location = new System.Drawing.Point(349, 127);
            this.btPrevSet.Name = "btPrevSet";
            this.btPrevSet.Size = new System.Drawing.Size(36, 23);
            this.btPrevSet.TabIndex = 29;
            this.btPrevSet.Text = "Set";
            this.btPrevSet.UseVisualStyleBackColor = true;
            this.btPrevSet.Click += new System.EventHandler(this.btPrevSet_Click);
            // 
            // btPlaySet
            // 
            this.btPlaySet.Location = new System.Drawing.Point(349, 185);
            this.btPlaySet.Name = "btPlaySet";
            this.btPlaySet.Size = new System.Drawing.Size(36, 23);
            this.btPlaySet.TabIndex = 29;
            this.btPlaySet.Text = "Set";
            this.btPlaySet.UseVisualStyleBackColor = true;
            this.btPlaySet.Click += new System.EventHandler(this.btPlaySet_Click);
            // 
            // btPauseSet
            // 
            this.btPauseSet.Location = new System.Drawing.Point(349, 69);
            this.btPauseSet.Name = "btPauseSet";
            this.btPauseSet.Size = new System.Drawing.Size(36, 23);
            this.btPauseSet.TabIndex = 29;
            this.btPauseSet.Text = "Set";
            this.btPauseSet.UseVisualStyleBackColor = true;
            this.btPauseSet.Click += new System.EventHandler(this.btPauseSet_Click);
            // 
            // btFastSet
            // 
            this.btFastSet.Location = new System.Drawing.Point(349, 214);
            this.btFastSet.Name = "btFastSet";
            this.btFastSet.Size = new System.Drawing.Size(36, 23);
            this.btFastSet.TabIndex = 29;
            this.btFastSet.Text = "Set";
            this.btFastSet.UseVisualStyleBackColor = true;
            this.btFastSet.Click += new System.EventHandler(this.btFastSet_Click);
            // 
            // btFadeoutSet
            // 
            this.btFadeoutSet.Location = new System.Drawing.Point(349, 98);
            this.btFadeoutSet.Name = "btFadeoutSet";
            this.btFadeoutSet.Size = new System.Drawing.Size(36, 23);
            this.btFadeoutSet.TabIndex = 29;
            this.btFadeoutSet.Text = "Set";
            this.btFadeoutSet.UseVisualStyleBackColor = true;
            this.btFadeoutSet.Click += new System.EventHandler(this.btFadeoutSet_Click);
            // 
            // btSlowSet
            // 
            this.btSlowSet.Location = new System.Drawing.Point(349, 156);
            this.btSlowSet.Name = "btSlowSet";
            this.btSlowSet.Size = new System.Drawing.Size(36, 23);
            this.btSlowSet.TabIndex = 29;
            this.btSlowSet.Text = "Set";
            this.btSlowSet.UseVisualStyleBackColor = true;
            this.btSlowSet.Click += new System.EventHandler(this.btSlowSet_Click);
            // 
            // btStopSet
            // 
            this.btStopSet.Location = new System.Drawing.Point(349, 40);
            this.btStopSet.Name = "btStopSet";
            this.btStopSet.Size = new System.Drawing.Size(36, 23);
            this.btStopSet.TabIndex = 29;
            this.btStopSet.Text = "Set";
            this.btStopSet.UseVisualStyleBackColor = true;
            this.btStopSet.Click += new System.EventHandler(this.btStopSet_Click);
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(240, 24);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(24, 12);
            this.label50.TabIndex = 28;
            this.label50.Text = "Key";
            // 
            // lblNextKey
            // 
            this.lblNextKey.AutoSize = true;
            this.lblNextKey.Location = new System.Drawing.Point(240, 248);
            this.lblNextKey.Name = "lblNextKey";
            this.lblNextKey.Size = new System.Drawing.Size(39, 12);
            this.lblNextKey.TabIndex = 28;
            this.lblNextKey.Text = "(None)";
            // 
            // lblFastKey
            // 
            this.lblFastKey.AutoSize = true;
            this.lblFastKey.Location = new System.Drawing.Point(240, 219);
            this.lblFastKey.Name = "lblFastKey";
            this.lblFastKey.Size = new System.Drawing.Size(39, 12);
            this.lblFastKey.TabIndex = 28;
            this.lblFastKey.Text = "(None)";
            // 
            // lblPlayKey
            // 
            this.lblPlayKey.AutoSize = true;
            this.lblPlayKey.Location = new System.Drawing.Point(240, 190);
            this.lblPlayKey.Name = "lblPlayKey";
            this.lblPlayKey.Size = new System.Drawing.Size(39, 12);
            this.lblPlayKey.TabIndex = 28;
            this.lblPlayKey.Text = "(None)";
            // 
            // lblSlowKey
            // 
            this.lblSlowKey.AutoSize = true;
            this.lblSlowKey.Location = new System.Drawing.Point(240, 161);
            this.lblSlowKey.Name = "lblSlowKey";
            this.lblSlowKey.Size = new System.Drawing.Size(39, 12);
            this.lblSlowKey.TabIndex = 28;
            this.lblSlowKey.Text = "(None)";
            // 
            // lblPrevKey
            // 
            this.lblPrevKey.AutoSize = true;
            this.lblPrevKey.Location = new System.Drawing.Point(240, 132);
            this.lblPrevKey.Name = "lblPrevKey";
            this.lblPrevKey.Size = new System.Drawing.Size(39, 12);
            this.lblPrevKey.TabIndex = 28;
            this.lblPrevKey.Text = "(None)";
            // 
            // lblFadeoutKey
            // 
            this.lblFadeoutKey.AutoSize = true;
            this.lblFadeoutKey.Location = new System.Drawing.Point(240, 103);
            this.lblFadeoutKey.Name = "lblFadeoutKey";
            this.lblFadeoutKey.Size = new System.Drawing.Size(39, 12);
            this.lblFadeoutKey.TabIndex = 28;
            this.lblFadeoutKey.Text = "(None)";
            // 
            // lblPauseKey
            // 
            this.lblPauseKey.AutoSize = true;
            this.lblPauseKey.Location = new System.Drawing.Point(240, 74);
            this.lblPauseKey.Name = "lblPauseKey";
            this.lblPauseKey.Size = new System.Drawing.Size(39, 12);
            this.lblPauseKey.TabIndex = 28;
            this.lblPauseKey.Text = "(None)";
            // 
            // lblStopKey
            // 
            this.lblStopKey.AutoSize = true;
            this.lblStopKey.Location = new System.Drawing.Point(240, 45);
            this.lblStopKey.Name = "lblStopKey";
            this.lblStopKey.Size = new System.Drawing.Size(39, 12);
            this.lblStopKey.TabIndex = 28;
            this.lblStopKey.Text = "(None)";
            // 
            // pictureBox14
            // 
            this.pictureBox14.Image = global::MDPlayer.Properties.Resources.ccStop;
            this.pictureBox14.Location = new System.Drawing.Point(4, 41);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Size = new System.Drawing.Size(16, 16);
            this.pictureBox14.TabIndex = 22;
            this.pictureBox14.TabStop = false;
            // 
            // pictureBox17
            // 
            this.pictureBox17.Image = global::MDPlayer.Properties.Resources.ccFadeout;
            this.pictureBox17.Location = new System.Drawing.Point(4, 99);
            this.pictureBox17.Name = "pictureBox17";
            this.pictureBox17.Size = new System.Drawing.Size(16, 16);
            this.pictureBox17.TabIndex = 25;
            this.pictureBox17.TabStop = false;
            // 
            // cbNextAlt
            // 
            this.cbNextAlt.AutoSize = true;
            this.cbNextAlt.Location = new System.Drawing.Point(201, 246);
            this.cbNextAlt.Name = "cbNextAlt";
            this.cbNextAlt.Size = new System.Drawing.Size(15, 14);
            this.cbNextAlt.TabIndex = 27;
            this.cbNextAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox16
            // 
            this.pictureBox16.Image = global::MDPlayer.Properties.Resources.ccPrevious;
            this.pictureBox16.Location = new System.Drawing.Point(4, 128);
            this.pictureBox16.Name = "pictureBox16";
            this.pictureBox16.Size = new System.Drawing.Size(16, 16);
            this.pictureBox16.TabIndex = 24;
            this.pictureBox16.TabStop = false;
            // 
            // cbFastAlt
            // 
            this.cbFastAlt.AutoSize = true;
            this.cbFastAlt.Location = new System.Drawing.Point(201, 219);
            this.cbFastAlt.Name = "cbFastAlt";
            this.cbFastAlt.Size = new System.Drawing.Size(15, 14);
            this.cbFastAlt.TabIndex = 27;
            this.cbFastAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox15
            // 
            this.pictureBox15.Image = global::MDPlayer.Properties.Resources.ccPause;
            this.pictureBox15.Location = new System.Drawing.Point(4, 70);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(16, 16);
            this.pictureBox15.TabIndex = 23;
            this.pictureBox15.TabStop = false;
            // 
            // cbPlayAlt
            // 
            this.cbPlayAlt.AutoSize = true;
            this.cbPlayAlt.Location = new System.Drawing.Point(201, 190);
            this.cbPlayAlt.Name = "cbPlayAlt";
            this.cbPlayAlt.Size = new System.Drawing.Size(15, 14);
            this.cbPlayAlt.TabIndex = 27;
            this.cbPlayAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox13
            // 
            this.pictureBox13.Image = global::MDPlayer.Properties.Resources.ccSlow;
            this.pictureBox13.Location = new System.Drawing.Point(4, 157);
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.Size = new System.Drawing.Size(16, 16);
            this.pictureBox13.TabIndex = 21;
            this.pictureBox13.TabStop = false;
            // 
            // cbSlowAlt
            // 
            this.cbSlowAlt.AutoSize = true;
            this.cbSlowAlt.Location = new System.Drawing.Point(201, 161);
            this.cbSlowAlt.Name = "cbSlowAlt";
            this.cbSlowAlt.Size = new System.Drawing.Size(15, 14);
            this.cbSlowAlt.TabIndex = 27;
            this.cbSlowAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox12
            // 
            this.pictureBox12.Image = global::MDPlayer.Properties.Resources.ccPlay;
            this.pictureBox12.Location = new System.Drawing.Point(4, 186);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(16, 16);
            this.pictureBox12.TabIndex = 20;
            this.pictureBox12.TabStop = false;
            // 
            // cbPrevAlt
            // 
            this.cbPrevAlt.AutoSize = true;
            this.cbPrevAlt.Location = new System.Drawing.Point(201, 132);
            this.cbPrevAlt.Name = "cbPrevAlt";
            this.cbPrevAlt.Size = new System.Drawing.Size(15, 14);
            this.cbPrevAlt.TabIndex = 27;
            this.cbPrevAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox11
            // 
            this.pictureBox11.Image = global::MDPlayer.Properties.Resources.ccFast;
            this.pictureBox11.Location = new System.Drawing.Point(4, 215);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(16, 16);
            this.pictureBox11.TabIndex = 19;
            this.pictureBox11.TabStop = false;
            // 
            // cbFadeoutAlt
            // 
            this.cbFadeoutAlt.AutoSize = true;
            this.cbFadeoutAlt.Location = new System.Drawing.Point(201, 103);
            this.cbFadeoutAlt.Name = "cbFadeoutAlt";
            this.cbFadeoutAlt.Size = new System.Drawing.Size(15, 14);
            this.cbFadeoutAlt.TabIndex = 27;
            this.cbFadeoutAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::MDPlayer.Properties.Resources.ccNext;
            this.pictureBox10.Location = new System.Drawing.Point(4, 242);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(16, 16);
            this.pictureBox10.TabIndex = 18;
            this.pictureBox10.TabStop = false;
            // 
            // cbPauseAlt
            // 
            this.cbPauseAlt.AutoSize = true;
            this.cbPauseAlt.Location = new System.Drawing.Point(201, 74);
            this.cbPauseAlt.Name = "cbPauseAlt";
            this.cbPauseAlt.Size = new System.Drawing.Size(15, 14);
            this.cbPauseAlt.TabIndex = 27;
            this.cbPauseAlt.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(26, 45);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(29, 12);
            this.label37.TabIndex = 26;
            this.label37.Text = "停止";
            // 
            // cbStopAlt
            // 
            this.cbStopAlt.AutoSize = true;
            this.cbStopAlt.Location = new System.Drawing.Point(201, 45);
            this.cbStopAlt.Name = "cbStopAlt";
            this.cbStopAlt.Size = new System.Drawing.Size(15, 14);
            this.cbStopAlt.TabIndex = 27;
            this.cbStopAlt.UseVisualStyleBackColor = true;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(127, 24);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(29, 12);
            this.label45.TabIndex = 26;
            this.label45.Text = "Shift";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(163, 24);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(24, 12);
            this.label46.TabIndex = 26;
            this.label46.Text = "Ctrl";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(199, 24);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(20, 12);
            this.label48.TabIndex = 26;
            this.label48.Text = "Alt";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(26, 74);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(53, 12);
            this.label38.TabIndex = 26;
            this.label38.Text = "一時停止";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(26, 103);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(65, 12);
            this.label39.TabIndex = 26;
            this.label39.Text = "フェードアウト";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(26, 132);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(29, 12);
            this.label40.TabIndex = 26;
            this.label40.Text = "前曲";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(26, 161);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(57, 12);
            this.label41.TabIndex = 26;
            this.label41.Text = "スロー再生";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(26, 190);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(29, 12);
            this.label42.TabIndex = 26;
            this.label42.Text = "再生";
            // 
            // cbNextCtrl
            // 
            this.cbNextCtrl.AutoSize = true;
            this.cbNextCtrl.Location = new System.Drawing.Point(165, 246);
            this.cbNextCtrl.Name = "cbNextCtrl";
            this.cbNextCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbNextCtrl.TabIndex = 27;
            this.cbNextCtrl.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(26, 219);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(59, 12);
            this.label43.TabIndex = 26;
            this.label43.Text = "3倍速再生";
            // 
            // cbFastCtrl
            // 
            this.cbFastCtrl.AutoSize = true;
            this.cbFastCtrl.Location = new System.Drawing.Point(165, 219);
            this.cbFastCtrl.Name = "cbFastCtrl";
            this.cbFastCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbFastCtrl.TabIndex = 27;
            this.cbFastCtrl.UseVisualStyleBackColor = true;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(26, 246);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(29, 12);
            this.label44.TabIndex = 26;
            this.label44.Text = "次曲";
            // 
            // cbPlayCtrl
            // 
            this.cbPlayCtrl.AutoSize = true;
            this.cbPlayCtrl.Location = new System.Drawing.Point(165, 190);
            this.cbPlayCtrl.Name = "cbPlayCtrl";
            this.cbPlayCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbPlayCtrl.TabIndex = 27;
            this.cbPlayCtrl.UseVisualStyleBackColor = true;
            // 
            // cbStopShift
            // 
            this.cbStopShift.AutoSize = true;
            this.cbStopShift.Location = new System.Drawing.Point(129, 45);
            this.cbStopShift.Name = "cbStopShift";
            this.cbStopShift.Size = new System.Drawing.Size(15, 14);
            this.cbStopShift.TabIndex = 27;
            this.cbStopShift.UseVisualStyleBackColor = true;
            // 
            // cbSlowCtrl
            // 
            this.cbSlowCtrl.AutoSize = true;
            this.cbSlowCtrl.Location = new System.Drawing.Point(165, 161);
            this.cbSlowCtrl.Name = "cbSlowCtrl";
            this.cbSlowCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbSlowCtrl.TabIndex = 27;
            this.cbSlowCtrl.UseVisualStyleBackColor = true;
            // 
            // cbPauseShift
            // 
            this.cbPauseShift.AutoSize = true;
            this.cbPauseShift.Location = new System.Drawing.Point(129, 74);
            this.cbPauseShift.Name = "cbPauseShift";
            this.cbPauseShift.Size = new System.Drawing.Size(15, 14);
            this.cbPauseShift.TabIndex = 27;
            this.cbPauseShift.UseVisualStyleBackColor = true;
            // 
            // cbPrevCtrl
            // 
            this.cbPrevCtrl.AutoSize = true;
            this.cbPrevCtrl.Location = new System.Drawing.Point(165, 132);
            this.cbPrevCtrl.Name = "cbPrevCtrl";
            this.cbPrevCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbPrevCtrl.TabIndex = 27;
            this.cbPrevCtrl.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutShift
            // 
            this.cbFadeoutShift.AutoSize = true;
            this.cbFadeoutShift.Location = new System.Drawing.Point(129, 103);
            this.cbFadeoutShift.Name = "cbFadeoutShift";
            this.cbFadeoutShift.Size = new System.Drawing.Size(15, 14);
            this.cbFadeoutShift.TabIndex = 27;
            this.cbFadeoutShift.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutCtrl
            // 
            this.cbFadeoutCtrl.AutoSize = true;
            this.cbFadeoutCtrl.Location = new System.Drawing.Point(165, 103);
            this.cbFadeoutCtrl.Name = "cbFadeoutCtrl";
            this.cbFadeoutCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbFadeoutCtrl.TabIndex = 27;
            this.cbFadeoutCtrl.UseVisualStyleBackColor = true;
            // 
            // cbPrevShift
            // 
            this.cbPrevShift.AutoSize = true;
            this.cbPrevShift.Location = new System.Drawing.Point(129, 132);
            this.cbPrevShift.Name = "cbPrevShift";
            this.cbPrevShift.Size = new System.Drawing.Size(15, 14);
            this.cbPrevShift.TabIndex = 27;
            this.cbPrevShift.UseVisualStyleBackColor = true;
            // 
            // cbPauseCtrl
            // 
            this.cbPauseCtrl.AutoSize = true;
            this.cbPauseCtrl.Location = new System.Drawing.Point(165, 74);
            this.cbPauseCtrl.Name = "cbPauseCtrl";
            this.cbPauseCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbPauseCtrl.TabIndex = 27;
            this.cbPauseCtrl.UseVisualStyleBackColor = true;
            // 
            // cbSlowShift
            // 
            this.cbSlowShift.AutoSize = true;
            this.cbSlowShift.Location = new System.Drawing.Point(129, 161);
            this.cbSlowShift.Name = "cbSlowShift";
            this.cbSlowShift.Size = new System.Drawing.Size(15, 14);
            this.cbSlowShift.TabIndex = 27;
            this.cbSlowShift.UseVisualStyleBackColor = true;
            // 
            // cbStopCtrl
            // 
            this.cbStopCtrl.AutoSize = true;
            this.cbStopCtrl.Location = new System.Drawing.Point(165, 45);
            this.cbStopCtrl.Name = "cbStopCtrl";
            this.cbStopCtrl.Size = new System.Drawing.Size(15, 14);
            this.cbStopCtrl.TabIndex = 27;
            this.cbStopCtrl.UseVisualStyleBackColor = true;
            // 
            // cbPlayShift
            // 
            this.cbPlayShift.AutoSize = true;
            this.cbPlayShift.Location = new System.Drawing.Point(129, 190);
            this.cbPlayShift.Name = "cbPlayShift";
            this.cbPlayShift.Size = new System.Drawing.Size(15, 14);
            this.cbPlayShift.TabIndex = 27;
            this.cbPlayShift.UseVisualStyleBackColor = true;
            // 
            // cbNextShift
            // 
            this.cbNextShift.AutoSize = true;
            this.cbNextShift.Location = new System.Drawing.Point(129, 246);
            this.cbNextShift.Name = "cbNextShift";
            this.cbNextShift.Size = new System.Drawing.Size(15, 14);
            this.cbNextShift.TabIndex = 27;
            this.cbNextShift.UseVisualStyleBackColor = true;
            // 
            // cbFastShift
            // 
            this.cbFastShift.AutoSize = true;
            this.cbFastShift.Location = new System.Drawing.Point(129, 219);
            this.cbFastShift.Name = "cbFastShift";
            this.cbFastShift.Size = new System.Drawing.Size(15, 14);
            this.cbFastShift.TabIndex = 27;
            this.cbFastShift.UseVisualStyleBackColor = true;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(63, 311);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(23, 12);
            this.label47.TabIndex = 26;
            this.label47.Text = "Win";
            this.label47.Visible = false;
            // 
            // cbStopWin
            // 
            this.cbStopWin.AutoSize = true;
            this.cbStopWin.Location = new System.Drawing.Point(92, 309);
            this.cbStopWin.Name = "cbStopWin";
            this.cbStopWin.Size = new System.Drawing.Size(15, 14);
            this.cbStopWin.TabIndex = 27;
            this.cbStopWin.UseVisualStyleBackColor = true;
            this.cbStopWin.Visible = false;
            // 
            // cbPauseWin
            // 
            this.cbPauseWin.AutoSize = true;
            this.cbPauseWin.Location = new System.Drawing.Point(113, 309);
            this.cbPauseWin.Name = "cbPauseWin";
            this.cbPauseWin.Size = new System.Drawing.Size(15, 14);
            this.cbPauseWin.TabIndex = 27;
            this.cbPauseWin.UseVisualStyleBackColor = true;
            this.cbPauseWin.Visible = false;
            // 
            // cbFadeoutWin
            // 
            this.cbFadeoutWin.AutoSize = true;
            this.cbFadeoutWin.Location = new System.Drawing.Point(134, 309);
            this.cbFadeoutWin.Name = "cbFadeoutWin";
            this.cbFadeoutWin.Size = new System.Drawing.Size(15, 14);
            this.cbFadeoutWin.TabIndex = 27;
            this.cbFadeoutWin.UseVisualStyleBackColor = true;
            this.cbFadeoutWin.Visible = false;
            // 
            // cbPrevWin
            // 
            this.cbPrevWin.AutoSize = true;
            this.cbPrevWin.Location = new System.Drawing.Point(155, 309);
            this.cbPrevWin.Name = "cbPrevWin";
            this.cbPrevWin.Size = new System.Drawing.Size(15, 14);
            this.cbPrevWin.TabIndex = 27;
            this.cbPrevWin.UseVisualStyleBackColor = true;
            this.cbPrevWin.Visible = false;
            // 
            // cbSlowWin
            // 
            this.cbSlowWin.AutoSize = true;
            this.cbSlowWin.Location = new System.Drawing.Point(176, 309);
            this.cbSlowWin.Name = "cbSlowWin";
            this.cbSlowWin.Size = new System.Drawing.Size(15, 14);
            this.cbSlowWin.TabIndex = 27;
            this.cbSlowWin.UseVisualStyleBackColor = true;
            this.cbSlowWin.Visible = false;
            // 
            // cbPlayWin
            // 
            this.cbPlayWin.AutoSize = true;
            this.cbPlayWin.Location = new System.Drawing.Point(197, 309);
            this.cbPlayWin.Name = "cbPlayWin";
            this.cbPlayWin.Size = new System.Drawing.Size(15, 14);
            this.cbPlayWin.TabIndex = 27;
            this.cbPlayWin.UseVisualStyleBackColor = true;
            this.cbPlayWin.Visible = false;
            // 
            // cbFastWin
            // 
            this.cbFastWin.AutoSize = true;
            this.cbFastWin.Location = new System.Drawing.Point(218, 309);
            this.cbFastWin.Name = "cbFastWin";
            this.cbFastWin.Size = new System.Drawing.Size(15, 14);
            this.cbFastWin.TabIndex = 27;
            this.cbFastWin.UseVisualStyleBackColor = true;
            this.cbFastWin.Visible = false;
            // 
            // cbNextWin
            // 
            this.cbNextWin.AutoSize = true;
            this.cbNextWin.Location = new System.Drawing.Point(239, 309);
            this.cbNextWin.Name = "cbNextWin";
            this.cbNextWin.Size = new System.Drawing.Size(15, 14);
            this.cbNextWin.TabIndex = 27;
            this.cbNextWin.UseVisualStyleBackColor = true;
            this.cbNextWin.Visible = false;
            // 
            // tpBalance
            // 
            this.tpBalance.Controls.Add(this.groupBox25);
            this.tpBalance.Controls.Add(this.cbAutoBalanceUseThis);
            this.tpBalance.Controls.Add(this.groupBox18);
            this.tpBalance.Location = new System.Drawing.Point(4, 22);
            this.tpBalance.Name = "tpBalance";
            this.tpBalance.Size = new System.Drawing.Size(443, 371);
            this.tpBalance.TabIndex = 12;
            this.tpBalance.Text = "ミキサーバランス";
            this.tpBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.rbAutoBalanceNotSamePositionAsSongData);
            this.groupBox25.Controls.Add(this.rbAutoBalanceSamePositionAsSongData);
            this.groupBox25.Location = new System.Drawing.Point(7, 327);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(433, 41);
            this.groupBox25.TabIndex = 1;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "ソングミキサーバランス参照フォルダー";
            // 
            // rbAutoBalanceNotSamePositionAsSongData
            // 
            this.rbAutoBalanceNotSamePositionAsSongData.AutoSize = true;
            this.rbAutoBalanceNotSamePositionAsSongData.Checked = true;
            this.rbAutoBalanceNotSamePositionAsSongData.Location = new System.Drawing.Point(6, 18);
            this.rbAutoBalanceNotSamePositionAsSongData.Name = "rbAutoBalanceNotSamePositionAsSongData";
            this.rbAutoBalanceNotSamePositionAsSongData.Size = new System.Drawing.Size(110, 16);
            this.rbAutoBalanceNotSamePositionAsSongData.TabIndex = 0;
            this.rbAutoBalanceNotSamePositionAsSongData.TabStop = true;
            this.rbAutoBalanceNotSamePositionAsSongData.Text = "設定ファイルと同じ";
            this.rbAutoBalanceNotSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSamePositionAsSongData
            // 
            this.rbAutoBalanceSamePositionAsSongData.AutoSize = true;
            this.rbAutoBalanceSamePositionAsSongData.Location = new System.Drawing.Point(122, 18);
            this.rbAutoBalanceSamePositionAsSongData.Name = "rbAutoBalanceSamePositionAsSongData";
            this.rbAutoBalanceSamePositionAsSongData.Size = new System.Drawing.Size(92, 16);
            this.rbAutoBalanceSamePositionAsSongData.TabIndex = 0;
            this.rbAutoBalanceSamePositionAsSongData.Text = "曲データと同じ";
            this.rbAutoBalanceSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // cbAutoBalanceUseThis
            // 
            this.cbAutoBalanceUseThis.AutoSize = true;
            this.cbAutoBalanceUseThis.Checked = true;
            this.cbAutoBalanceUseThis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoBalanceUseThis.Location = new System.Drawing.Point(13, 3);
            this.cbAutoBalanceUseThis.Name = "cbAutoBalanceUseThis";
            this.cbAutoBalanceUseThis.Size = new System.Drawing.Size(221, 16);
            this.cbAutoBalanceUseThis.TabIndex = 1;
            this.cbAutoBalanceUseThis.Text = "ミキサーバランス自動設定機能を使用する";
            this.cbAutoBalanceUseThis.UseVisualStyleBackColor = true;
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.groupBox24);
            this.groupBox18.Controls.Add(this.groupBox23);
            this.groupBox18.Location = new System.Drawing.Point(7, 3);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(433, 318);
            this.groupBox18.TabIndex = 0;
            this.groupBox18.TabStop = false;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.groupBox21);
            this.groupBox24.Controls.Add(this.groupBox22);
            this.groupBox24.Location = new System.Drawing.Point(6, 185);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(421, 127);
            this.groupBox24.TabIndex = 1;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "保存";
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.rbAutoBalanceNotSaveSongBalance);
            this.groupBox21.Controls.Add(this.rbAutoBalanceSaveSongBalance);
            this.groupBox21.Location = new System.Drawing.Point(6, 18);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(409, 62);
            this.groupBox21.TabIndex = 0;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "ソングミキサーバランス(曲データ毎)";
            // 
            // rbAutoBalanceNotSaveSongBalance
            // 
            this.rbAutoBalanceNotSaveSongBalance.AutoSize = true;
            this.rbAutoBalanceNotSaveSongBalance.Checked = true;
            this.rbAutoBalanceNotSaveSongBalance.Location = new System.Drawing.Point(6, 40);
            this.rbAutoBalanceNotSaveSongBalance.Name = "rbAutoBalanceNotSaveSongBalance";
            this.rbAutoBalanceNotSaveSongBalance.Size = new System.Drawing.Size(153, 16);
            this.rbAutoBalanceNotSaveSongBalance.TabIndex = 0;
            this.rbAutoBalanceNotSaveSongBalance.TabStop = true;
            this.rbAutoBalanceNotSaveSongBalance.Text = "保存しない(手動保存のみ)";
            this.rbAutoBalanceNotSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSaveSongBalance
            // 
            this.rbAutoBalanceSaveSongBalance.AutoSize = true;
            this.rbAutoBalanceSaveSongBalance.Location = new System.Drawing.Point(6, 18);
            this.rbAutoBalanceSaveSongBalance.Name = "rbAutoBalanceSaveSongBalance";
            this.rbAutoBalanceSaveSongBalance.Size = new System.Drawing.Size(150, 16);
            this.rbAutoBalanceSaveSongBalance.TabIndex = 0;
            this.rbAutoBalanceSaveSongBalance.Text = "演奏停止時に自動で保存";
            this.rbAutoBalanceSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.label4);
            this.groupBox22.Location = new System.Drawing.Point(6, 86);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(409, 35);
            this.groupBox22.TabIndex = 0;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "ドライバーミキサーバランス(ドライバ毎)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "手動保存のみ";
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.groupBox19);
            this.groupBox23.Controls.Add(this.groupBox20);
            this.groupBox23.Location = new System.Drawing.Point(6, 22);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(421, 157);
            this.groupBox23.TabIndex = 1;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "読み込み";
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.rbAutoBalanceNotLoadSongBalance);
            this.groupBox19.Controls.Add(this.rbAutoBalanceLoadSongBalance);
            this.groupBox19.Location = new System.Drawing.Point(6, 18);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(409, 63);
            this.groupBox19.TabIndex = 0;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "ソングミキサーバランス(曲データ毎)";
            // 
            // rbAutoBalanceNotLoadSongBalance
            // 
            this.rbAutoBalanceNotLoadSongBalance.AutoSize = true;
            this.rbAutoBalanceNotLoadSongBalance.Checked = true;
            this.rbAutoBalanceNotLoadSongBalance.Location = new System.Drawing.Point(6, 40);
            this.rbAutoBalanceNotLoadSongBalance.Name = "rbAutoBalanceNotLoadSongBalance";
            this.rbAutoBalanceNotLoadSongBalance.Size = new System.Drawing.Size(102, 16);
            this.rbAutoBalanceNotLoadSongBalance.TabIndex = 0;
            this.rbAutoBalanceNotLoadSongBalance.TabStop = true;
            this.rbAutoBalanceNotLoadSongBalance.Text = "手動で読み込む";
            this.rbAutoBalanceNotLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadSongBalance
            // 
            this.rbAutoBalanceLoadSongBalance.AutoSize = true;
            this.rbAutoBalanceLoadSongBalance.Location = new System.Drawing.Point(6, 18);
            this.rbAutoBalanceLoadSongBalance.Name = "rbAutoBalanceLoadSongBalance";
            this.rbAutoBalanceLoadSongBalance.Size = new System.Drawing.Size(147, 16);
            this.rbAutoBalanceLoadSongBalance.TabIndex = 0;
            this.rbAutoBalanceLoadSongBalance.Text = "再生時に自動で読み込む";
            this.rbAutoBalanceLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.rbAutoBalanceNotLoadDriverBalance);
            this.groupBox20.Controls.Add(this.rbAutoBalanceLoadDriverBalance);
            this.groupBox20.Location = new System.Drawing.Point(6, 87);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(409, 63);
            this.groupBox20.TabIndex = 0;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "ドライバーミキサーバランス(ドライバ毎)";
            // 
            // rbAutoBalanceNotLoadDriverBalance
            // 
            this.rbAutoBalanceNotLoadDriverBalance.AutoSize = true;
            this.rbAutoBalanceNotLoadDriverBalance.Location = new System.Drawing.Point(6, 40);
            this.rbAutoBalanceNotLoadDriverBalance.Name = "rbAutoBalanceNotLoadDriverBalance";
            this.rbAutoBalanceNotLoadDriverBalance.Size = new System.Drawing.Size(102, 16);
            this.rbAutoBalanceNotLoadDriverBalance.TabIndex = 0;
            this.rbAutoBalanceNotLoadDriverBalance.Text = "手動で読み込む";
            this.rbAutoBalanceNotLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadDriverBalance
            // 
            this.rbAutoBalanceLoadDriverBalance.AutoSize = true;
            this.rbAutoBalanceLoadDriverBalance.Checked = true;
            this.rbAutoBalanceLoadDriverBalance.Location = new System.Drawing.Point(6, 18);
            this.rbAutoBalanceLoadDriverBalance.Name = "rbAutoBalanceLoadDriverBalance";
            this.rbAutoBalanceLoadDriverBalance.Size = new System.Drawing.Size(343, 16);
            this.rbAutoBalanceLoadDriverBalance.TabIndex = 0;
            this.rbAutoBalanceLoadDriverBalance.TabStop = true;
            this.rbAutoBalanceLoadDriverBalance.Text = "再生時に自動で読み込む(曲データ毎のバランスファイルが無い場合)";
            this.rbAutoBalanceLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // tpOther
            // 
            this.tpOther.Controls.Add(this.cbNonRenderingForPause);
            this.tpOther.Controls.Add(this.cbWavSwitch);
            this.tpOther.Controls.Add(this.groupBox17);
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
            this.tpOther.Controls.Add(this.cbEmptyPlayList);
            this.tpOther.Controls.Add(this.cbInitAlways);
            this.tpOther.Controls.Add(this.cbAutoOpen);
            this.tpOther.Controls.Add(this.cbUseLoopTimes);
            this.tpOther.Location = new System.Drawing.Point(4, 22);
            this.tpOther.Name = "tpOther";
            this.tpOther.Size = new System.Drawing.Size(443, 371);
            this.tpOther.TabIndex = 2;
            this.tpOther.Text = "Other";
            this.tpOther.UseVisualStyleBackColor = true;
            // 
            // cbNonRenderingForPause
            // 
            this.cbNonRenderingForPause.AutoSize = true;
            this.cbNonRenderingForPause.Location = new System.Drawing.Point(240, 131);
            this.cbNonRenderingForPause.Name = "cbNonRenderingForPause";
            this.cbNonRenderingForPause.Size = new System.Drawing.Size(156, 16);
            this.cbNonRenderingForPause.TabIndex = 24;
            this.cbNonRenderingForPause.Text = "ポーズ時にレンダリングしない";
            this.cbNonRenderingForPause.UseVisualStyleBackColor = true;
            // 
            // cbWavSwitch
            // 
            this.cbWavSwitch.AutoSize = true;
            this.cbWavSwitch.Location = new System.Drawing.Point(14, 202);
            this.cbWavSwitch.Name = "cbWavSwitch";
            this.cbWavSwitch.Size = new System.Drawing.Size(177, 16);
            this.cbWavSwitch.TabIndex = 0;
            this.cbWavSwitch.Text = "演奏時に.wavファイルを出力する";
            this.cbWavSwitch.UseVisualStyleBackColor = true;
            this.cbWavSwitch.CheckedChanged += new System.EventHandler(this.cbWavSwitch_CheckedChanged);
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.tbImageExt);
            this.groupBox17.Controls.Add(this.tbMMLExt);
            this.groupBox17.Controls.Add(this.tbTextExt);
            this.groupBox17.Controls.Add(this.label1);
            this.groupBox17.Controls.Add(this.label3);
            this.groupBox17.Controls.Add(this.label2);
            this.groupBox17.Location = new System.Drawing.Point(7, 256);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(227, 83);
            this.groupBox17.TabIndex = 1;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "File Extension";
            // 
            // tbImageExt
            // 
            this.tbImageExt.Location = new System.Drawing.Point(52, 58);
            this.tbImageExt.Name = "tbImageExt";
            this.tbImageExt.Size = new System.Drawing.Size(164, 19);
            this.tbImageExt.TabIndex = 1;
            // 
            // tbMMLExt
            // 
            this.tbMMLExt.Location = new System.Drawing.Point(52, 35);
            this.tbMMLExt.Name = "tbMMLExt";
            this.tbMMLExt.Size = new System.Drawing.Size(164, 19);
            this.tbMMLExt.TabIndex = 1;
            // 
            // tbTextExt
            // 
            this.tbTextExt.Location = new System.Drawing.Point(52, 12);
            this.tbTextExt.Name = "tbTextExt";
            this.tbTextExt.Size = new System.Drawing.Size(164, 19);
            this.tbTextExt.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Image";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "MML";
            // 
            // cbUseGetInst
            // 
            this.cbUseGetInst.AutoSize = true;
            this.cbUseGetInst.Location = new System.Drawing.Point(14, 31);
            this.cbUseGetInst.Name = "cbUseGetInst";
            this.cbUseGetInst.Size = new System.Drawing.Size(286, 16);
            this.cbUseGetInst.TabIndex = 12;
            this.cbUseGetInst.Text = "音色欄をクリック時、その音色をクリップボードにコピーする";
            this.cbUseGetInst.UseVisualStyleBackColor = true;
            this.cbUseGetInst.CheckedChanged += new System.EventHandler(this.cbUseGetInst_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbInstFormat);
            this.groupBox4.Controls.Add(this.lblInstFormat);
            this.groupBox4.Location = new System.Drawing.Point(7, 31);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(433, 45);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            // 
            // cmbInstFormat
            // 
            this.cmbInstFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstFormat.FormattingEnabled = true;
            this.cmbInstFormat.Items.AddRange(new object[] {
            "FMP7",
            "MDX",
            ".TFI(ファイル出力)",
            "MUSIC LALF #1",
            "MUSIC LALF #2",
            "MML2VGM",
            "NRTDRV",
            "HuSIC",
            "VOPM",
            "PMD",
            "MUCOM88"});
            this.cmbInstFormat.Location = new System.Drawing.Point(298, 19);
            this.cmbInstFormat.Name = "cmbInstFormat";
            this.cmbInstFormat.Size = new System.Drawing.Size(129, 20);
            this.cmbInstFormat.TabIndex = 18;
            // 
            // lblInstFormat
            // 
            this.lblInstFormat.AutoSize = true;
            this.lblInstFormat.Location = new System.Drawing.Point(237, 22);
            this.lblInstFormat.Name = "lblInstFormat";
            this.lblInstFormat.Size = new System.Drawing.Size(55, 12);
            this.lblInstFormat.TabIndex = 17;
            this.lblInstFormat.Text = "フォーマット";
            // 
            // cbDumpSwitch
            // 
            this.cbDumpSwitch.AutoSize = true;
            this.cbDumpSwitch.Location = new System.Drawing.Point(14, 152);
            this.cbDumpSwitch.Name = "cbDumpSwitch";
            this.cbDumpSwitch.Size = new System.Drawing.Size(220, 16);
            this.cbDumpSwitch.TabIndex = 0;
            this.cbDumpSwitch.Text = "DataBlock処理時にその内容をダンプする";
            this.cbDumpSwitch.UseVisualStyleBackColor = true;
            this.cbDumpSwitch.CheckedChanged += new System.EventHandler(this.cbDumpSwitch_CheckedChanged);
            // 
            // gbWav
            // 
            this.gbWav.Controls.Add(this.btnWavPath);
            this.gbWav.Controls.Add(this.label7);
            this.gbWav.Controls.Add(this.tbWavPath);
            this.gbWav.Location = new System.Drawing.Point(7, 205);
            this.gbWav.Name = "gbWav";
            this.gbWav.Size = new System.Drawing.Size(433, 45);
            this.gbWav.TabIndex = 22;
            this.gbWav.TabStop = false;
            // 
            // btnWavPath
            // 
            this.btnWavPath.Location = new System.Drawing.Point(404, 16);
            this.btnWavPath.Name = "btnWavPath";
            this.btnWavPath.Size = new System.Drawing.Size(23, 23);
            this.btnWavPath.TabIndex = 16;
            this.btnWavPath.Text = "...";
            this.btnWavPath.UseVisualStyleBackColor = true;
            this.btnWavPath.Click += new System.EventHandler(this.btnWavPath_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "出力Path";
            // 
            // tbWavPath
            // 
            this.tbWavPath.Location = new System.Drawing.Point(73, 18);
            this.tbWavPath.Name = "tbWavPath";
            this.tbWavPath.Size = new System.Drawing.Size(325, 19);
            this.tbWavPath.TabIndex = 15;
            // 
            // gbDump
            // 
            this.gbDump.Controls.Add(this.btnDumpPath);
            this.gbDump.Controls.Add(this.label6);
            this.gbDump.Controls.Add(this.tbDumpPath);
            this.gbDump.Location = new System.Drawing.Point(7, 154);
            this.gbDump.Name = "gbDump";
            this.gbDump.Size = new System.Drawing.Size(433, 45);
            this.gbDump.TabIndex = 22;
            this.gbDump.TabStop = false;
            // 
            // btnDumpPath
            // 
            this.btnDumpPath.Location = new System.Drawing.Point(404, 16);
            this.btnDumpPath.Name = "btnDumpPath";
            this.btnDumpPath.Size = new System.Drawing.Size(23, 23);
            this.btnDumpPath.TabIndex = 16;
            this.btnDumpPath.Text = "...";
            this.btnDumpPath.UseVisualStyleBackColor = true;
            this.btnDumpPath.Click += new System.EventHandler(this.btnDumpPath_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "出力Path";
            // 
            // tbDumpPath
            // 
            this.tbDumpPath.Location = new System.Drawing.Point(73, 18);
            this.tbDumpPath.Name = "tbDumpPath";
            this.tbDumpPath.Size = new System.Drawing.Size(325, 19);
            this.tbDumpPath.TabIndex = 15;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(136, 132);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(30, 12);
            this.label30.TabIndex = 21;
            this.label30.Text = "Hz/s";
            // 
            // tbScreenFrameRate
            // 
            this.tbScreenFrameRate.Location = new System.Drawing.Point(80, 129);
            this.tbScreenFrameRate.Name = "tbScreenFrameRate";
            this.tbScreenFrameRate.Size = new System.Drawing.Size(50, 19);
            this.tbScreenFrameRate.TabIndex = 20;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(5, 132);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(69, 12);
            this.label29.TabIndex = 19;
            this.label29.Text = "フレームレート";
            // 
            // lblLoopTimes
            // 
            this.lblLoopTimes.AutoSize = true;
            this.lblLoopTimes.Location = new System.Drawing.Point(340, 8);
            this.lblLoopTimes.Name = "lblLoopTimes";
            this.lblLoopTimes.Size = new System.Drawing.Size(17, 12);
            this.lblLoopTimes.TabIndex = 1;
            this.lblLoopTimes.Text = "回";
            // 
            // btnDataPath
            // 
            this.btnDataPath.Location = new System.Drawing.Point(411, 102);
            this.btnDataPath.Name = "btnDataPath";
            this.btnDataPath.Size = new System.Drawing.Size(23, 23);
            this.btnDataPath.TabIndex = 16;
            this.btnDataPath.Text = "...";
            this.btnDataPath.UseVisualStyleBackColor = true;
            this.btnDataPath.Click += new System.EventHandler(this.btnDataPath_Click);
            // 
            // tbLoopTimes
            // 
            this.tbLoopTimes.Location = new System.Drawing.Point(282, 5);
            this.tbLoopTimes.Name = "tbLoopTimes";
            this.tbLoopTimes.Size = new System.Drawing.Size(52, 19);
            this.tbLoopTimes.TabIndex = 0;
            // 
            // tbDataPath
            // 
            this.tbDataPath.Location = new System.Drawing.Point(80, 104);
            this.tbDataPath.Name = "tbDataPath";
            this.tbDataPath.Size = new System.Drawing.Size(325, 19);
            this.tbDataPath.TabIndex = 15;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(5, 107);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 12);
            this.label19.TabIndex = 14;
            this.label19.Text = "データPath";
            // 
            // btnResetPosition
            // 
            this.btnResetPosition.Location = new System.Drawing.Point(167, 345);
            this.btnResetPosition.Name = "btnResetPosition";
            this.btnResetPosition.Size = new System.Drawing.Size(142, 23);
            this.btnResetPosition.TabIndex = 13;
            this.btnResetPosition.Text = "ウィンドウ位置をリセット";
            this.btnResetPosition.UseVisualStyleBackColor = true;
            this.btnResetPosition.Click += new System.EventHandler(this.btnResetPosition_Click);
            // 
            // btnOpenSettingFolder
            // 
            this.btnOpenSettingFolder.Location = new System.Drawing.Point(315, 345);
            this.btnOpenSettingFolder.Name = "btnOpenSettingFolder";
            this.btnOpenSettingFolder.Size = new System.Drawing.Size(125, 23);
            this.btnOpenSettingFolder.TabIndex = 13;
            this.btnOpenSettingFolder.Text = "設定フォルダーを開く";
            this.btnOpenSettingFolder.UseVisualStyleBackColor = true;
            this.btnOpenSettingFolder.Click += new System.EventHandler(this.btnOpenSettingFolder_Click);
            // 
            // cbExALL
            // 
            this.cbExALL.AutoSize = true;
            this.cbExALL.Location = new System.Drawing.Point(240, 323);
            this.cbExALL.Name = "cbExALL";
            this.cbExALL.Size = new System.Drawing.Size(192, 16);
            this.cbExALL.TabIndex = 0;
            this.cbExALL.Text = "キャリアとモジュレータの区別をしない";
            this.cbExALL.UseVisualStyleBackColor = true;
            this.cbExALL.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbEmptyPlayList
            // 
            this.cbEmptyPlayList.AutoSize = true;
            this.cbEmptyPlayList.Location = new System.Drawing.Point(240, 293);
            this.cbEmptyPlayList.Name = "cbEmptyPlayList";
            this.cbEmptyPlayList.Size = new System.Drawing.Size(177, 16);
            this.cbEmptyPlayList.TabIndex = 0;
            this.cbEmptyPlayList.Text = "起動時にプレイリストを空にする。";
            this.cbEmptyPlayList.UseVisualStyleBackColor = true;
            this.cbEmptyPlayList.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbInitAlways
            // 
            this.cbInitAlways.Location = new System.Drawing.Point(240, 251);
            this.cbInitAlways.Name = "cbInitAlways";
            this.cbInitAlways.Size = new System.Drawing.Size(194, 39);
            this.cbInitAlways.TabIndex = 0;
            this.cbInitAlways.Text = "再生開始時に必ずデバイスを初期化する。";
            this.cbInitAlways.UseVisualStyleBackColor = true;
            this.cbInitAlways.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbAutoOpen
            // 
            this.cbAutoOpen.AutoSize = true;
            this.cbAutoOpen.Location = new System.Drawing.Point(7, 82);
            this.cbAutoOpen.Name = "cbAutoOpen";
            this.cbAutoOpen.Size = new System.Drawing.Size(167, 16);
            this.cbAutoOpen.TabIndex = 0;
            this.cbAutoOpen.Text = "使用音源の画面を自動で開く";
            this.cbAutoOpen.UseVisualStyleBackColor = true;
            this.cbAutoOpen.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbUseLoopTimes
            // 
            this.cbUseLoopTimes.AutoSize = true;
            this.cbUseLoopTimes.Location = new System.Drawing.Point(7, 7);
            this.cbUseLoopTimes.Name = "cbUseLoopTimes";
            this.cbUseLoopTimes.Size = new System.Drawing.Size(216, 16);
            this.cbUseLoopTimes.TabIndex = 0;
            this.cbUseLoopTimes.Text = "無限ループ時、指定の回数だけ繰り返す";
            this.cbUseLoopTimes.UseVisualStyleBackColor = true;
            this.cbUseLoopTimes.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // tpOmake
            // 
            this.tpOmake.Controls.Add(this.label14);
            this.tpOmake.Controls.Add(this.btVST);
            this.tpOmake.Controls.Add(this.tbVST);
            this.tpOmake.Controls.Add(this.groupBox5);
            this.tpOmake.Location = new System.Drawing.Point(4, 22);
            this.tpOmake.Name = "tpOmake";
            this.tpOmake.Size = new System.Drawing.Size(443, 371);
            this.tpOmake.TabIndex = 7;
            this.tpOmake.Text = "おまけ";
            this.tpOmake.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 12);
            this.label14.TabIndex = 19;
            this.label14.Text = "VST effect";
            this.label14.Visible = false;
            // 
            // btVST
            // 
            this.btVST.Location = new System.Drawing.Point(417, 54);
            this.btVST.Name = "btVST";
            this.btVST.Size = new System.Drawing.Size(23, 23);
            this.btVST.TabIndex = 18;
            this.btVST.Text = "...";
            this.btVST.UseVisualStyleBackColor = true;
            this.btVST.Visible = false;
            this.btVST.Click += new System.EventHandler(this.btVST_Click);
            // 
            // tbVST
            // 
            this.tbVST.Location = new System.Drawing.Point(88, 56);
            this.tbVST.Name = "tbVST";
            this.tbVST.Size = new System.Drawing.Size(323, 19);
            this.tbVST.TabIndex = 17;
            this.tbVST.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbDispFrameCounter);
            this.groupBox5.Location = new System.Drawing.Point(7, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(184, 37);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Debug Mode";
            // 
            // cbDispFrameCounter
            // 
            this.cbDispFrameCounter.AutoSize = true;
            this.cbDispFrameCounter.Location = new System.Drawing.Point(6, 17);
            this.cbDispFrameCounter.Name = "cbDispFrameCounter";
            this.cbDispFrameCounter.Size = new System.Drawing.Size(120, 16);
            this.cbDispFrameCounter.TabIndex = 2;
            this.cbDispFrameCounter.Text = "FrameCounter表示";
            this.cbDispFrameCounter.UseVisualStyleBackColor = true;
            // 
            // tpAbout
            // 
            this.tpAbout.Controls.Add(this.tableLayoutPanel);
            this.tpAbout.Location = new System.Drawing.Point(4, 22);
            this.tpAbout.Name = "tpAbout";
            this.tpAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tpAbout.Size = new System.Drawing.Size(443, 371);
            this.tpAbout.TabIndex = 1;
            this.tpAbout.Text = "About";
            this.tpAbout.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.llOpenGithub, 1, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.070175F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.421053F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(437, 365);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPictureBox.Image = global::MDPlayer.Properties.Resources.フェーリAndMD2;
            this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
            this.logoPictureBox.Size = new System.Drawing.Size(138, 359);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Location = new System.Drawing.Point(150, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(284, 16);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "製品名";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Location = new System.Drawing.Point(150, 36);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(284, 16);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "バージョン";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyright.Location = new System.Drawing.Point(150, 72);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(284, 16);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "著作権";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCompanyName.Location = new System.Drawing.Point(150, 108);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(284, 16);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "会社名";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(150, 140);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(284, 189);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "説明";
            // 
            // llOpenGithub
            // 
            this.llOpenGithub.AutoSize = true;
            this.llOpenGithub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llOpenGithub.Location = new System.Drawing.Point(147, 332);
            this.llOpenGithub.Name = "llOpenGithub";
            this.llOpenGithub.Size = new System.Drawing.Size(287, 33);
            this.llOpenGithub.TabIndex = 24;
            this.llOpenGithub.TabStop = true;
            this.llOpenGithub.Text = "Open latest version page of Github.";
            this.llOpenGithub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llOpenGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llOpenGithub_LinkClicked);
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 441);
            this.Controls.Add(this.tcSetting);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(480, 480);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 480);
            this.Name = "frmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "オプション";
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tpNuked.ResumeLayout(false);
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.tpNSF.ResumeLayout(false);
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
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
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
            this.tpOther.ResumeLayout(false);
            this.tpOther.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
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
        private System.Windows.Forms.CheckBox cbNSFDmc_TriNull;
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
    }
}