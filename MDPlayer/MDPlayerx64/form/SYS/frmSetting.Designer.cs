#if X64
using MDPlayerx64;
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
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
            btnOK = new Button();
            btnCancel = new Button();
            gbWaveOut = new GroupBox();
            cmbWaveOutDevice = new ComboBox();
            rbWaveOut = new RadioButton();
            rbAsioOut = new RadioButton();
            rbWasapiOut = new RadioButton();
            gbAsioOut = new GroupBox();
            btnASIOControlPanel = new Button();
            cmbAsioDevice = new ComboBox();
            rbDirectSoundOut = new RadioButton();
            gbWasapiOut = new GroupBox();
            rbExclusive = new RadioButton();
            rbShare = new RadioButton();
            cmbWasapiDevice = new ComboBox();
            gbDirectSound = new GroupBox();
            cmbDirectSoundDevice = new ComboBox();
            tcSetting = new TabControl();
            tpOutput = new TabPage();
            rbNullDevice = new RadioButton();
            label36 = new Label();
            lblWaitTime = new Label();
            label66 = new Label();
            lblLatencyUnit = new Label();
            label28 = new Label();
            label65 = new Label();
            lblLatency = new Label();
            cmbWaitTime = new ComboBox();
            cmbSampleRate = new ComboBox();
            cmbLatency = new ComboBox();
            rbSPPCM = new RadioButton();
            groupBox16 = new GroupBox();
            cmbSPPCMDevice = new ComboBox();
            tpModule = new TabPage();
            groupBox1 = new GroupBox();
            cbUnuseRealChip = new CheckBox();
            ucSI = new ucSettingInstruments();
            groupBox3 = new GroupBox();
            cbHiyorimiMode = new CheckBox();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            tbLatencyEmu = new TextBox();
            tbLatencySCCI = new TextBox();
            label10 = new Label();
            tpNuked = new TabPage();
            groupBox29 = new GroupBox();
            cbGensSSGEG = new CheckBox();
            cbGensDACHPF = new CheckBox();
            groupBox26 = new GroupBox();
            rbNukedOPN2OptionYM2612u = new RadioButton();
            rbNukedOPN2OptionYM2612 = new RadioButton();
            rbNukedOPN2OptionDiscrete = new RadioButton();
            rbNukedOPN2OptionASIClp = new RadioButton();
            rbNukedOPN2OptionASIC = new RadioButton();
            tpNSF = new TabPage();
            trkbNSFLPF = new TrackBar();
            label53 = new Label();
            label52 = new Label();
            trkbNSFHPF = new TrackBar();
            groupBox10 = new GroupBox();
            cbNSFDmc_DPCMReverse = new CheckBox();
            cbNSFDmc_RandomizeTri = new CheckBox();
            cbNSFDmc_TriMute = new CheckBox();
            cbNSFDmc_RandomizeNoise = new CheckBox();
            cbNSFDmc_DPCMAntiClick = new CheckBox();
            cbNSFDmc_EnablePNoise = new CheckBox();
            cbNSFDmc_Enable4011 = new CheckBox();
            cbNSFDmc_NonLinearMixer = new CheckBox();
            cbNSFDmc_UnmuteOnReset = new CheckBox();
            groupBox12 = new GroupBox();
            cbNSFN160_Serial = new CheckBox();
            groupBox11 = new GroupBox();
            cbNSFMmc5_PhaseRefresh = new CheckBox();
            cbNSFMmc5_NonLinearMixer = new CheckBox();
            groupBox9 = new GroupBox();
            cbNFSNes_DutySwap = new CheckBox();
            cbNFSNes_PhaseRefresh = new CheckBox();
            cbNFSNes_NonLinearMixer = new CheckBox();
            cbNFSNes_UnmuteOnReset = new CheckBox();
            groupBox8 = new GroupBox();
            label21 = new Label();
            label20 = new Label();
            tbNSFFds_LPF = new TextBox();
            cbNFSFds_4085Reset = new CheckBox();
            cbNSFFDSWriteDisable8000 = new CheckBox();
            tpSID = new TabPage();
            groupBox28 = new GroupBox();
            cbSIDModel_Force = new CheckBox();
            rbSIDModel_8580 = new RadioButton();
            rbSIDModel_6581 = new RadioButton();
            groupBox27 = new GroupBox();
            cbSIDC64Model_Force = new CheckBox();
            rbSIDC64Model_DREAN = new RadioButton();
            rbSIDC64Model_OLDNTSC = new RadioButton();
            rbSIDC64Model_NTSC = new RadioButton();
            rbSIDC64Model_PAL = new RadioButton();
            groupBox14 = new GroupBox();
            label27 = new Label();
            label26 = new Label();
            label25 = new Label();
            rdSIDQ1 = new RadioButton();
            rdSIDQ3 = new RadioButton();
            rdSIDQ2 = new RadioButton();
            rdSIDQ4 = new RadioButton();
            groupBox13 = new GroupBox();
            btnSIDBasic = new Button();
            btnSIDCharacter = new Button();
            btnSIDKernal = new Button();
            tbSIDCharacter = new TextBox();
            tbSIDBasic = new TextBox();
            tbSIDKernal = new TextBox();
            label24 = new Label();
            label23 = new Label();
            label22 = new Label();
            tbSIDOutputBufferSize = new TextBox();
            label51 = new Label();
            label49 = new Label();
            tpPMDDotNET = new TabPage();
            rbPMDManual = new RadioButton();
            rbPMDAuto = new RadioButton();
            btnPMDResetDriverArguments = new Button();
            label54 = new Label();
            btnPMDResetCompilerArhguments = new Button();
            tbPMDDriverArguments = new TextBox();
            label55 = new Label();
            tbPMDCompilerArguments = new TextBox();
            gbPMDManual = new GroupBox();
            cbPMDSetManualVolume = new CheckBox();
            cbPMDUsePPZ8 = new CheckBox();
            groupBox32 = new GroupBox();
            rbPMD86B = new RadioButton();
            rbPMDSpbB = new RadioButton();
            rbPMDNrmB = new RadioButton();
            cbPMDUsePPSDRV = new CheckBox();
            gbPPSDRV = new GroupBox();
            groupBox33 = new GroupBox();
            rbPMDUsePPSDRVManualFreq = new RadioButton();
            label56 = new Label();
            rbPMDUsePPSDRVFreqDefault = new RadioButton();
            btnPMDPPSDRVManualWait = new Button();
            label57 = new Label();
            tbPMDPPSDRVFreq = new TextBox();
            label58 = new Label();
            tbPMDPPSDRVManualWait = new TextBox();
            gbPMDSetManualVolume = new GroupBox();
            label59 = new Label();
            label60 = new Label();
            tbPMDVolumeAdpcm = new TextBox();
            label61 = new Label();
            tbPMDVolumeRhythm = new TextBox();
            label62 = new Label();
            tbPMDVolumeSSG = new TextBox();
            label63 = new Label();
            tbPMDVolumeGIMICSSG = new TextBox();
            label64 = new Label();
            tbPMDVolumeFM = new TextBox();
            tpMIDIOut = new TabPage();
            btnAddVST = new Button();
            tbcMIDIoutList = new TabControl();
            tabPage1 = new TabPage();
            dgvMIDIoutListA = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            clmIsVST = new DataGridViewCheckBoxColumn();
            clmFileName = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            clmType = new DataGridViewComboBoxColumn();
            ClmBeforeSend = new DataGridViewComboBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            btnUP_A = new Button();
            btnDOWN_A = new Button();
            tabPage2 = new TabPage();
            dgvMIDIoutListB = new DataGridView();
            btnUP_B = new Button();
            btnDOWN_B = new Button();
            tabPage3 = new TabPage();
            dgvMIDIoutListC = new DataGridView();
            btnUP_C = new Button();
            btnDOWN_C = new Button();
            tabPage4 = new TabPage();
            dgvMIDIoutListD = new DataGridView();
            btnUP_D = new Button();
            btnDOWN_D = new Button();
            tabPage5 = new TabPage();
            dgvMIDIoutListE = new DataGridView();
            btnUP_E = new Button();
            btnDOWN_E = new Button();
            tabPage6 = new TabPage();
            dgvMIDIoutListF = new DataGridView();
            btnUP_F = new Button();
            btnDOWN_F = new Button();
            tabPage7 = new TabPage();
            dgvMIDIoutListG = new DataGridView();
            btnUP_G = new Button();
            btnDOWN_G = new Button();
            tabPage8 = new TabPage();
            dgvMIDIoutListH = new DataGridView();
            btnUP_H = new Button();
            btnDOWN_H = new Button();
            tabPage9 = new TabPage();
            dgvMIDIoutListI = new DataGridView();
            btnUP_I = new Button();
            btnDOWN_I = new Button();
            tabPage10 = new TabPage();
            dgvMIDIoutListJ = new DataGridView();
            button17 = new Button();
            btnDOWN_J = new Button();
            btnSubMIDIout = new Button();
            btnAddMIDIout = new Button();
            label18 = new Label();
            dgvMIDIoutPallet = new DataGridView();
            clmID = new DataGridViewTextBoxColumn();
            clmDeviceName = new DataGridViewTextBoxColumn();
            clmManufacturer = new DataGridViewTextBoxColumn();
            clmSpacer = new DataGridViewTextBoxColumn();
            label16 = new Label();
            tpMIDIOut2 = new TabPage();
            groupBox15 = new GroupBox();
            btnBeforeSend_Default = new Button();
            tbBeforeSend_Custom = new TextBox();
            tbBeforeSend_XGReset = new TextBox();
            label35 = new Label();
            label34 = new Label();
            label32 = new Label();
            tbBeforeSend_GSReset = new TextBox();
            label33 = new Label();
            tbBeforeSend_GMReset = new TextBox();
            label31 = new Label();
            tabMIDIExp = new TabPage();
            cbUseMIDIExport = new CheckBox();
            gbMIDIExport = new GroupBox();
            cbMIDIKeyOnFnum = new CheckBox();
            cbMIDIUseVOPM = new CheckBox();
            groupBox6 = new GroupBox();
            cbMIDIYM2612 = new CheckBox();
            cbMIDISN76489Sec = new CheckBox();
            cbMIDIYM2612Sec = new CheckBox();
            cbMIDISN76489 = new CheckBox();
            cbMIDIYM2151 = new CheckBox();
            cbMIDIYM2610BSec = new CheckBox();
            cbMIDIYM2151Sec = new CheckBox();
            cbMIDIYM2610B = new CheckBox();
            cbMIDIYM2203 = new CheckBox();
            cbMIDIYM2608Sec = new CheckBox();
            cbMIDIYM2203Sec = new CheckBox();
            cbMIDIYM2608 = new CheckBox();
            cbMIDIPlayless = new CheckBox();
            btnMIDIOutputPath = new Button();
            lblOutputPath = new Label();
            tbMIDIOutputPath = new TextBox();
            tpMIDIKBD = new TabPage();
            cbUseMIDIKeyboard = new CheckBox();
            gbMIDIKeyboard = new GroupBox();
            pictureBox8 = new PictureBox();
            pictureBox7 = new PictureBox();
            pictureBox6 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            tbCCFadeout = new TextBox();
            tbCCPause = new TextBox();
            tbCCSlow = new TextBox();
            tbCCPrevious = new TextBox();
            tbCCNext = new TextBox();
            tbCCFast = new TextBox();
            tbCCStop = new TextBox();
            tbCCPlay = new TextBox();
            tbCCCopyLog = new TextBox();
            label17 = new Label();
            tbCCDelLog = new TextBox();
            label15 = new Label();
            tbCCChCopy = new TextBox();
            label8 = new Label();
            label9 = new Label();
            gbUseChannel = new GroupBox();
            rbMONO = new RadioButton();
            rbPOLY = new RadioButton();
            groupBox7 = new GroupBox();
            rbFM6 = new RadioButton();
            rbFM3 = new RadioButton();
            rbFM5 = new RadioButton();
            rbFM2 = new RadioButton();
            rbFM4 = new RadioButton();
            rbFM1 = new RadioButton();
            groupBox2 = new GroupBox();
            cbFM1 = new CheckBox();
            cbFM6 = new CheckBox();
            cbFM2 = new CheckBox();
            cbFM5 = new CheckBox();
            cbFM3 = new CheckBox();
            cbFM4 = new CheckBox();
            cmbMIDIIN = new ComboBox();
            label5 = new Label();
            tpKeyBoard = new TabPage();
            cbUseKeyBoardHook = new CheckBox();
            gbUseKeyBoardHook = new GroupBox();
            panel1 = new Panel();
            btPpcClr = new Button();
            cbPpcShift = new CheckBox();
            btPpcSet = new Button();
            label76 = new Label();
            lblPpcKey = new Label();
            cbPpcAlt = new CheckBox();
            cbPpcCtrl = new CheckBox();
            btDpcClr = new Button();
            cbDpcShift = new CheckBox();
            btDpcSet = new Button();
            label74 = new Label();
            lblDpcKey = new Label();
            cbDpcAlt = new CheckBox();
            cbDpcCtrl = new CheckBox();
            btUpcClr = new Button();
            btRmvClr = new Button();
            cbUpcShift = new CheckBox();
            pictureBox14 = new PictureBox();
            cbRmvShift = new CheckBox();
            btUpcSet = new Button();
            cbFastShift = new CheckBox();
            label72 = new Label();
            btDmvClr = new Button();
            lblUpcKey = new Label();
            btRmvSet = new Button();
            cbUpcAlt = new CheckBox();
            cbDmvShift = new CheckBox();
            cbUpcCtrl = new CheckBox();
            label69 = new Label();
            btUmvClr = new Button();
            cbRmvAlt = new CheckBox();
            cbUmvShift = new CheckBox();
            cbRmvCtrl = new CheckBox();
            btNextClr = new Button();
            lblRmvKey = new Label();
            cbNextShift = new CheckBox();
            btPrevClr = new Button();
            cbPlayShift = new CheckBox();
            btPlayClr = new Button();
            cbStopCtrl = new CheckBox();
            btPauseClr = new Button();
            cbSlowShift = new CheckBox();
            btFastClr = new Button();
            cbPauseCtrl = new CheckBox();
            btFadeoutClr = new Button();
            cbPrevShift = new CheckBox();
            btSlowClr = new Button();
            cbFadeoutCtrl = new CheckBox();
            btDmvSet = new Button();
            btStopClr = new Button();
            btUmvSet = new Button();
            cbFadeoutShift = new CheckBox();
            btNextSet = new Button();
            cbPrevCtrl = new CheckBox();
            btPrevSet = new Button();
            cbPauseShift = new CheckBox();
            btPlaySet = new Button();
            cbSlowCtrl = new CheckBox();
            btPauseSet = new Button();
            cbStopShift = new CheckBox();
            btFastSet = new Button();
            label71 = new Label();
            cbPlayCtrl = new CheckBox();
            label70 = new Label();
            btFadeoutSet = new Button();
            label44 = new Label();
            btSlowSet = new Button();
            cbFastCtrl = new CheckBox();
            btStopSet = new Button();
            cbDmvCtrl = new CheckBox();
            label43 = new Label();
            cbUmvCtrl = new CheckBox();
            lblDmvKey = new Label();
            label50 = new Label();
            lblUmvKey = new Label();
            cbNextCtrl = new CheckBox();
            lblNextKey = new Label();
            label42 = new Label();
            lblFastKey = new Label();
            label41 = new Label();
            lblPlayKey = new Label();
            label40 = new Label();
            lblSlowKey = new Label();
            label39 = new Label();
            lblPrevKey = new Label();
            label38 = new Label();
            lblFadeoutKey = new Label();
            label48 = new Label();
            lblPauseKey = new Label();
            label46 = new Label();
            lblStopKey = new Label();
            label45 = new Label();
            cbStopAlt = new CheckBox();
            cbDmvAlt = new CheckBox();
            pictureBox17 = new PictureBox();
            cbUmvAlt = new CheckBox();
            label37 = new Label();
            cbNextAlt = new CheckBox();
            cbPauseAlt = new CheckBox();
            pictureBox16 = new PictureBox();
            pictureBox10 = new PictureBox();
            cbFastAlt = new CheckBox();
            cbFadeoutAlt = new CheckBox();
            pictureBox15 = new PictureBox();
            pictureBox11 = new PictureBox();
            cbPlayAlt = new CheckBox();
            cbPrevAlt = new CheckBox();
            pictureBox13 = new PictureBox();
            pictureBox12 = new PictureBox();
            cbSlowAlt = new CheckBox();
            lblKeyBoardHookNotice = new Label();
            label47 = new Label();
            cbStopWin = new CheckBox();
            cbPauseWin = new CheckBox();
            cbFadeoutWin = new CheckBox();
            cbPrevWin = new CheckBox();
            cbSlowWin = new CheckBox();
            cbPlayWin = new CheckBox();
            cbFastWin = new CheckBox();
            cbNextWin = new CheckBox();
            tpBalance = new TabPage();
            groupBox25 = new GroupBox();
            rbAutoBalanceNotSamePositionAsSongData = new RadioButton();
            rbAutoBalanceSamePositionAsSongData = new RadioButton();
            cbAutoBalanceUseThis = new CheckBox();
            groupBox18 = new GroupBox();
            groupBox24 = new GroupBox();
            groupBox21 = new GroupBox();
            rbAutoBalanceNotSaveSongBalance = new RadioButton();
            rbAutoBalanceSaveSongBalance = new RadioButton();
            groupBox22 = new GroupBox();
            label4 = new Label();
            groupBox23 = new GroupBox();
            groupBox19 = new GroupBox();
            rbAutoBalanceNotLoadSongBalance = new RadioButton();
            rbAutoBalanceLoadSongBalance = new RadioButton();
            groupBox20 = new GroupBox();
            rbAutoBalanceNotLoadDriverBalance = new RadioButton();
            rbAutoBalanceLoadDriverBalance = new RadioButton();
            tpPlayList = new TabPage();
            groupBox17 = new GroupBox();
            cbAutoOpenImg = new CheckBox();
            tbImageExt = new TextBox();
            cbAutoOpenMML = new CheckBox();
            tbMMLExt = new TextBox();
            tbTextExt = new TextBox();
            cbAutoOpenText = new CheckBox();
            label1 = new Label();
            label3 = new Label();
            label2 = new Label();
            cbEmptyPlayList = new CheckBox();
            tpOther = new TabPage();
            btnImageResourceFile = new Button();
            tbResourceFile = new TextBox();
            label73 = new Label();
            btnSearchPath = new Button();
            tbSearchPath = new TextBox();
            label68 = new Label();
            cbNonRenderingForPause = new CheckBox();
            cbWavSwitch = new CheckBox();
            cbUseGetInst = new CheckBox();
            groupBox4 = new GroupBox();
            cbAdjustTLParam = new CheckBox();
            cmbInstFormat = new ComboBox();
            lblInstFormat = new Label();
            cbDumpSwitch = new CheckBox();
            gbWav = new GroupBox();
            btnWavPath = new Button();
            label7 = new Label();
            tbWavPath = new TextBox();
            gbDump = new GroupBox();
            btnDumpPath = new Button();
            label6 = new Label();
            tbDumpPath = new TextBox();
            label30 = new Label();
            tbScreenFrameRate = new TextBox();
            label29 = new Label();
            lblLoopTimes = new Label();
            btnDataPath = new Button();
            tbLoopTimes = new TextBox();
            tbDataPath = new TextBox();
            label19 = new Label();
            btnResetPosition = new Button();
            btnOpenSettingFolder = new Button();
            cbExALL = new CheckBox();
            cbSaveCompiledFile = new CheckBox();
            cbInitAlways = new CheckBox();
            cbAutoOpen = new CheckBox();
            cbUseLoopTimes = new CheckBox();
            tpOmake = new TabPage();
            label67 = new Label();
            label14 = new Label();
            btVST = new Button();
            tbSCCbaseAddress = new TextBox();
            tbVST = new TextBox();
            groupBox5 = new GroupBox();
            groupBox30 = new GroupBox();
            rbLoglvlInformation = new RadioButton();
            rbLoglvlWarning = new RadioButton();
            rbLoglvlError = new RadioButton();
            rbLogDebug = new RadioButton();
            rbLoglvlTrace = new RadioButton();
            cbShowConsole = new CheckBox();
            cbDispFrameCounter = new CheckBox();
            tpAbout = new TabPage();
            tableLayoutPanel = new TableLayoutPanel();
            logoPictureBox = new PictureBox();
            labelProductName = new Label();
            labelVersion = new Label();
            labelCopyright = new Label();
            labelCompanyName = new Label();
            textBoxDescription = new TextBox();
            llOpenGithub = new LinkLabel();
            gbWaveOut.SuspendLayout();
            gbAsioOut.SuspendLayout();
            gbWasapiOut.SuspendLayout();
            gbDirectSound.SuspendLayout();
            tcSetting.SuspendLayout();
            tpOutput.SuspendLayout();
            groupBox16.SuspendLayout();
            tpModule.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            tpNuked.SuspendLayout();
            groupBox29.SuspendLayout();
            groupBox26.SuspendLayout();
            tpNSF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkbNSFLPF).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkbNSFHPF).BeginInit();
            groupBox10.SuspendLayout();
            groupBox12.SuspendLayout();
            groupBox11.SuspendLayout();
            groupBox9.SuspendLayout();
            groupBox8.SuspendLayout();
            tpSID.SuspendLayout();
            groupBox28.SuspendLayout();
            groupBox27.SuspendLayout();
            groupBox14.SuspendLayout();
            groupBox13.SuspendLayout();
            tpPMDDotNET.SuspendLayout();
            gbPMDManual.SuspendLayout();
            groupBox32.SuspendLayout();
            gbPPSDRV.SuspendLayout();
            groupBox33.SuspendLayout();
            gbPMDSetManualVolume.SuspendLayout();
            tpMIDIOut.SuspendLayout();
            tbcMIDIoutList.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListA).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListB).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListC).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListD).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListE).BeginInit();
            tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListF).BeginInit();
            tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListG).BeginInit();
            tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListH).BeginInit();
            tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListI).BeginInit();
            tabPage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListJ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutPallet).BeginInit();
            tpMIDIOut2.SuspendLayout();
            groupBox15.SuspendLayout();
            tabMIDIExp.SuspendLayout();
            gbMIDIExport.SuspendLayout();
            groupBox6.SuspendLayout();
            tpMIDIKBD.SuspendLayout();
            gbMIDIKeyboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            gbUseChannel.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox2.SuspendLayout();
            tpKeyBoard.SuspendLayout();
            gbUseKeyBoardHook.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox14).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox17).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox15).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox13).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).BeginInit();
            tpBalance.SuspendLayout();
            groupBox25.SuspendLayout();
            groupBox18.SuspendLayout();
            groupBox24.SuspendLayout();
            groupBox21.SuspendLayout();
            groupBox22.SuspendLayout();
            groupBox23.SuspendLayout();
            groupBox19.SuspendLayout();
            groupBox20.SuspendLayout();
            tpPlayList.SuspendLayout();
            groupBox17.SuspendLayout();
            tpOther.SuspendLayout();
            groupBox4.SuspendLayout();
            gbWav.SuspendLayout();
            gbDump.SuspendLayout();
            tpOmake.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox30.SuspendLayout();
            tpAbout.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(btnOK, "btnOK");
            btnOK.Name = "btnOK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbWaveOut
            // 
            resources.ApplyResources(gbWaveOut, "gbWaveOut");
            gbWaveOut.Controls.Add(cmbWaveOutDevice);
            gbWaveOut.Name = "gbWaveOut";
            gbWaveOut.TabStop = false;
            // 
            // cmbWaveOutDevice
            // 
            resources.ApplyResources(cmbWaveOutDevice, "cmbWaveOutDevice");
            cmbWaveOutDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWaveOutDevice.FormattingEnabled = true;
            cmbWaveOutDevice.Name = "cmbWaveOutDevice";
            // 
            // rbWaveOut
            // 
            resources.ApplyResources(rbWaveOut, "rbWaveOut");
            rbWaveOut.Checked = true;
            rbWaveOut.Name = "rbWaveOut";
            rbWaveOut.TabStop = true;
            rbWaveOut.UseVisualStyleBackColor = true;
            rbWaveOut.CheckedChanged += RbWaveOut_CheckedChanged;
            // 
            // rbAsioOut
            // 
            resources.ApplyResources(rbAsioOut, "rbAsioOut");
            rbAsioOut.Name = "rbAsioOut";
            rbAsioOut.UseVisualStyleBackColor = true;
            rbAsioOut.CheckedChanged += RbAsioOut_CheckedChanged;
            // 
            // rbWasapiOut
            // 
            resources.ApplyResources(rbWasapiOut, "rbWasapiOut");
            rbWasapiOut.Name = "rbWasapiOut";
            rbWasapiOut.UseVisualStyleBackColor = true;
            rbWasapiOut.CheckedChanged += RbWasapiOut_CheckedChanged;
            // 
            // gbAsioOut
            // 
            resources.ApplyResources(gbAsioOut, "gbAsioOut");
            gbAsioOut.Controls.Add(btnASIOControlPanel);
            gbAsioOut.Controls.Add(cmbAsioDevice);
            gbAsioOut.Name = "gbAsioOut";
            gbAsioOut.TabStop = false;
            // 
            // btnASIOControlPanel
            // 
            resources.ApplyResources(btnASIOControlPanel, "btnASIOControlPanel");
            btnASIOControlPanel.Name = "btnASIOControlPanel";
            btnASIOControlPanel.UseVisualStyleBackColor = true;
            btnASIOControlPanel.Click += btnASIOControlPanel_Click;
            // 
            // cmbAsioDevice
            // 
            resources.ApplyResources(cmbAsioDevice, "cmbAsioDevice");
            cmbAsioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAsioDevice.FormattingEnabled = true;
            cmbAsioDevice.Name = "cmbAsioDevice";
            // 
            // rbDirectSoundOut
            // 
            resources.ApplyResources(rbDirectSoundOut, "rbDirectSoundOut");
            rbDirectSoundOut.Name = "rbDirectSoundOut";
            rbDirectSoundOut.UseVisualStyleBackColor = true;
            rbDirectSoundOut.CheckedChanged += RbDirectSoundOut_CheckedChanged;
            // 
            // gbWasapiOut
            // 
            resources.ApplyResources(gbWasapiOut, "gbWasapiOut");
            gbWasapiOut.Controls.Add(rbExclusive);
            gbWasapiOut.Controls.Add(rbShare);
            gbWasapiOut.Controls.Add(cmbWasapiDevice);
            gbWasapiOut.Name = "gbWasapiOut";
            gbWasapiOut.TabStop = false;
            // 
            // rbExclusive
            // 
            resources.ApplyResources(rbExclusive, "rbExclusive");
            rbExclusive.Name = "rbExclusive";
            rbExclusive.TabStop = true;
            rbExclusive.UseVisualStyleBackColor = true;
            // 
            // rbShare
            // 
            resources.ApplyResources(rbShare, "rbShare");
            rbShare.Name = "rbShare";
            rbShare.TabStop = true;
            rbShare.UseVisualStyleBackColor = true;
            // 
            // cmbWasapiDevice
            // 
            resources.ApplyResources(cmbWasapiDevice, "cmbWasapiDevice");
            cmbWasapiDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWasapiDevice.FormattingEnabled = true;
            cmbWasapiDevice.Name = "cmbWasapiDevice";
            // 
            // gbDirectSound
            // 
            resources.ApplyResources(gbDirectSound, "gbDirectSound");
            gbDirectSound.Controls.Add(cmbDirectSoundDevice);
            gbDirectSound.Name = "gbDirectSound";
            gbDirectSound.TabStop = false;
            // 
            // cmbDirectSoundDevice
            // 
            resources.ApplyResources(cmbDirectSoundDevice, "cmbDirectSoundDevice");
            cmbDirectSoundDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDirectSoundDevice.FormattingEnabled = true;
            cmbDirectSoundDevice.Name = "cmbDirectSoundDevice";
            // 
            // tcSetting
            // 
            resources.ApplyResources(tcSetting, "tcSetting");
            tcSetting.Controls.Add(tpOutput);
            tcSetting.Controls.Add(tpModule);
            tcSetting.Controls.Add(tpNuked);
            tcSetting.Controls.Add(tpNSF);
            tcSetting.Controls.Add(tpSID);
            tcSetting.Controls.Add(tpPMDDotNET);
            tcSetting.Controls.Add(tpMIDIOut);
            tcSetting.Controls.Add(tpMIDIOut2);
            tcSetting.Controls.Add(tabMIDIExp);
            tcSetting.Controls.Add(tpMIDIKBD);
            tcSetting.Controls.Add(tpKeyBoard);
            tcSetting.Controls.Add(tpBalance);
            tcSetting.Controls.Add(tpPlayList);
            tcSetting.Controls.Add(tpOther);
            tcSetting.Controls.Add(tpOmake);
            tcSetting.Controls.Add(tpAbout);
            tcSetting.HotTrack = true;
            tcSetting.Multiline = true;
            tcSetting.Name = "tcSetting";
            tcSetting.SelectedIndex = 0;
            // 
            // tpOutput
            // 
            tpOutput.Controls.Add(rbNullDevice);
            tpOutput.Controls.Add(label36);
            tpOutput.Controls.Add(lblWaitTime);
            tpOutput.Controls.Add(label66);
            tpOutput.Controls.Add(lblLatencyUnit);
            tpOutput.Controls.Add(label28);
            tpOutput.Controls.Add(label65);
            tpOutput.Controls.Add(lblLatency);
            tpOutput.Controls.Add(cmbWaitTime);
            tpOutput.Controls.Add(cmbSampleRate);
            tpOutput.Controls.Add(cmbLatency);
            tpOutput.Controls.Add(rbSPPCM);
            tpOutput.Controls.Add(rbDirectSoundOut);
            tpOutput.Controls.Add(rbWaveOut);
            tpOutput.Controls.Add(rbAsioOut);
            tpOutput.Controls.Add(gbWaveOut);
            tpOutput.Controls.Add(rbWasapiOut);
            tpOutput.Controls.Add(groupBox16);
            tpOutput.Controls.Add(gbAsioOut);
            tpOutput.Controls.Add(gbDirectSound);
            tpOutput.Controls.Add(gbWasapiOut);
            resources.ApplyResources(tpOutput, "tpOutput");
            tpOutput.Name = "tpOutput";
            tpOutput.UseVisualStyleBackColor = true;
            // 
            // rbNullDevice
            // 
            resources.ApplyResources(rbNullDevice, "rbNullDevice");
            rbNullDevice.Name = "rbNullDevice";
            rbNullDevice.UseVisualStyleBackColor = true;
            rbNullDevice.CheckedChanged += RbDirectSoundOut_CheckedChanged;
            // 
            // label36
            // 
            resources.ApplyResources(label36, "label36");
            label36.Name = "label36";
            // 
            // lblWaitTime
            // 
            resources.ApplyResources(lblWaitTime, "lblWaitTime");
            lblWaitTime.Name = "lblWaitTime";
            // 
            // label66
            // 
            resources.ApplyResources(label66, "label66");
            label66.Name = "label66";
            // 
            // lblLatencyUnit
            // 
            resources.ApplyResources(lblLatencyUnit, "lblLatencyUnit");
            lblLatencyUnit.Name = "lblLatencyUnit";
            // 
            // label28
            // 
            resources.ApplyResources(label28, "label28");
            label28.Name = "label28";
            // 
            // label65
            // 
            resources.ApplyResources(label65, "label65");
            label65.Name = "label65";
            // 
            // lblLatency
            // 
            resources.ApplyResources(lblLatency, "lblLatency");
            lblLatency.Name = "lblLatency";
            // 
            // cmbWaitTime
            // 
            cmbWaitTime.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWaitTime.FormattingEnabled = true;
            cmbWaitTime.Items.AddRange(new object[] { resources.GetString("cmbWaitTime.Items"), resources.GetString("cmbWaitTime.Items1"), resources.GetString("cmbWaitTime.Items2"), resources.GetString("cmbWaitTime.Items3"), resources.GetString("cmbWaitTime.Items4"), resources.GetString("cmbWaitTime.Items5"), resources.GetString("cmbWaitTime.Items6"), resources.GetString("cmbWaitTime.Items7"), resources.GetString("cmbWaitTime.Items8"), resources.GetString("cmbWaitTime.Items9"), resources.GetString("cmbWaitTime.Items10") });
            resources.ApplyResources(cmbWaitTime, "cmbWaitTime");
            cmbWaitTime.Name = "cmbWaitTime";
            // 
            // cmbSampleRate
            // 
            resources.ApplyResources(cmbSampleRate, "cmbSampleRate");
            cmbSampleRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSampleRate.FormattingEnabled = true;
            cmbSampleRate.Items.AddRange(new object[] { resources.GetString("cmbSampleRate.Items"), resources.GetString("cmbSampleRate.Items1"), resources.GetString("cmbSampleRate.Items2"), resources.GetString("cmbSampleRate.Items3"), resources.GetString("cmbSampleRate.Items4"), resources.GetString("cmbSampleRate.Items5"), resources.GetString("cmbSampleRate.Items6") });
            cmbSampleRate.Name = "cmbSampleRate";
            // 
            // cmbLatency
            // 
            cmbLatency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLatency.FormattingEnabled = true;
            cmbLatency.Items.AddRange(new object[] { resources.GetString("cmbLatency.Items"), resources.GetString("cmbLatency.Items1"), resources.GetString("cmbLatency.Items2"), resources.GetString("cmbLatency.Items3"), resources.GetString("cmbLatency.Items4"), resources.GetString("cmbLatency.Items5"), resources.GetString("cmbLatency.Items6"), resources.GetString("cmbLatency.Items7") });
            resources.ApplyResources(cmbLatency, "cmbLatency");
            cmbLatency.Name = "cmbLatency";
            // 
            // rbSPPCM
            // 
            resources.ApplyResources(rbSPPCM, "rbSPPCM");
            rbSPPCM.Name = "rbSPPCM";
            rbSPPCM.UseVisualStyleBackColor = true;
            rbSPPCM.CheckedChanged += RbDirectSoundOut_CheckedChanged;
            // 
            // groupBox16
            // 
            resources.ApplyResources(groupBox16, "groupBox16");
            groupBox16.Controls.Add(cmbSPPCMDevice);
            groupBox16.Name = "groupBox16";
            groupBox16.TabStop = false;
            // 
            // cmbSPPCMDevice
            // 
            resources.ApplyResources(cmbSPPCMDevice, "cmbSPPCMDevice");
            cmbSPPCMDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSPPCMDevice.FormattingEnabled = true;
            cmbSPPCMDevice.Name = "cmbSPPCMDevice";
            // 
            // tpModule
            // 
            tpModule.Controls.Add(groupBox1);
            tpModule.Controls.Add(groupBox3);
            resources.ApplyResources(tpModule, "tpModule");
            tpModule.Name = "tpModule";
            tpModule.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(cbUnuseRealChip);
            groupBox1.Controls.Add(ucSI);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // cbUnuseRealChip
            // 
            resources.ApplyResources(cbUnuseRealChip, "cbUnuseRealChip");
            cbUnuseRealChip.Name = "cbUnuseRealChip";
            cbUnuseRealChip.UseVisualStyleBackColor = true;
            // 
            // ucSI
            // 
            resources.ApplyResources(ucSI, "ucSI");
            ucSI.Name = "ucSI";
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(cbHiyorimiMode);
            groupBox3.Controls.Add(label13);
            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(tbLatencyEmu);
            groupBox3.Controls.Add(tbLatencySCCI);
            groupBox3.Controls.Add(label10);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // cbHiyorimiMode
            // 
            resources.ApplyResources(cbHiyorimiMode, "cbHiyorimiMode");
            cbHiyorimiMode.Name = "cbHiyorimiMode";
            cbHiyorimiMode.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            resources.ApplyResources(label13, "label13");
            label13.Name = "label13";
            // 
            // label12
            // 
            resources.ApplyResources(label12, "label12");
            label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.Name = "label11";
            // 
            // tbLatencyEmu
            // 
            resources.ApplyResources(tbLatencyEmu, "tbLatencyEmu");
            tbLatencyEmu.Name = "tbLatencyEmu";
            // 
            // tbLatencySCCI
            // 
            resources.ApplyResources(tbLatencySCCI, "tbLatencySCCI");
            tbLatencySCCI.Name = "tbLatencySCCI";
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            // 
            // tpNuked
            // 
            tpNuked.Controls.Add(groupBox29);
            tpNuked.Controls.Add(groupBox26);
            resources.ApplyResources(tpNuked, "tpNuked");
            tpNuked.Name = "tpNuked";
            tpNuked.UseVisualStyleBackColor = true;
            // 
            // groupBox29
            // 
            groupBox29.Controls.Add(cbGensSSGEG);
            groupBox29.Controls.Add(cbGensDACHPF);
            resources.ApplyResources(groupBox29, "groupBox29");
            groupBox29.Name = "groupBox29";
            groupBox29.TabStop = false;
            // 
            // cbGensSSGEG
            // 
            resources.ApplyResources(cbGensSSGEG, "cbGensSSGEG");
            cbGensSSGEG.Name = "cbGensSSGEG";
            cbGensSSGEG.UseVisualStyleBackColor = true;
            // 
            // cbGensDACHPF
            // 
            resources.ApplyResources(cbGensDACHPF, "cbGensDACHPF");
            cbGensDACHPF.Name = "cbGensDACHPF";
            cbGensDACHPF.UseVisualStyleBackColor = true;
            // 
            // groupBox26
            // 
            groupBox26.Controls.Add(rbNukedOPN2OptionYM2612u);
            groupBox26.Controls.Add(rbNukedOPN2OptionYM2612);
            groupBox26.Controls.Add(rbNukedOPN2OptionDiscrete);
            groupBox26.Controls.Add(rbNukedOPN2OptionASIClp);
            groupBox26.Controls.Add(rbNukedOPN2OptionASIC);
            resources.ApplyResources(groupBox26, "groupBox26");
            groupBox26.Name = "groupBox26";
            groupBox26.TabStop = false;
            // 
            // rbNukedOPN2OptionYM2612u
            // 
            resources.ApplyResources(rbNukedOPN2OptionYM2612u, "rbNukedOPN2OptionYM2612u");
            rbNukedOPN2OptionYM2612u.Name = "rbNukedOPN2OptionYM2612u";
            rbNukedOPN2OptionYM2612u.TabStop = true;
            rbNukedOPN2OptionYM2612u.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionYM2612
            // 
            resources.ApplyResources(rbNukedOPN2OptionYM2612, "rbNukedOPN2OptionYM2612");
            rbNukedOPN2OptionYM2612.Name = "rbNukedOPN2OptionYM2612";
            rbNukedOPN2OptionYM2612.TabStop = true;
            rbNukedOPN2OptionYM2612.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionDiscrete
            // 
            resources.ApplyResources(rbNukedOPN2OptionDiscrete, "rbNukedOPN2OptionDiscrete");
            rbNukedOPN2OptionDiscrete.Name = "rbNukedOPN2OptionDiscrete";
            rbNukedOPN2OptionDiscrete.TabStop = true;
            rbNukedOPN2OptionDiscrete.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIClp
            // 
            resources.ApplyResources(rbNukedOPN2OptionASIClp, "rbNukedOPN2OptionASIClp");
            rbNukedOPN2OptionASIClp.Name = "rbNukedOPN2OptionASIClp";
            rbNukedOPN2OptionASIClp.TabStop = true;
            rbNukedOPN2OptionASIClp.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIC
            // 
            resources.ApplyResources(rbNukedOPN2OptionASIC, "rbNukedOPN2OptionASIC");
            rbNukedOPN2OptionASIC.Name = "rbNukedOPN2OptionASIC";
            rbNukedOPN2OptionASIC.TabStop = true;
            rbNukedOPN2OptionASIC.UseVisualStyleBackColor = true;
            // 
            // tpNSF
            // 
            tpNSF.Controls.Add(trkbNSFLPF);
            tpNSF.Controls.Add(label53);
            tpNSF.Controls.Add(label52);
            tpNSF.Controls.Add(trkbNSFHPF);
            tpNSF.Controls.Add(groupBox10);
            tpNSF.Controls.Add(groupBox12);
            tpNSF.Controls.Add(groupBox11);
            tpNSF.Controls.Add(groupBox9);
            tpNSF.Controls.Add(groupBox8);
            resources.ApplyResources(tpNSF, "tpNSF");
            tpNSF.Name = "tpNSF";
            tpNSF.UseVisualStyleBackColor = true;
            // 
            // trkbNSFLPF
            // 
            resources.ApplyResources(trkbNSFLPF, "trkbNSFLPF");
            trkbNSFLPF.Maximum = 400;
            trkbNSFLPF.Name = "trkbNSFLPF";
            trkbNSFLPF.TickFrequency = 10;
            // 
            // label53
            // 
            resources.ApplyResources(label53, "label53");
            label53.Name = "label53";
            // 
            // label52
            // 
            resources.ApplyResources(label52, "label52");
            label52.Name = "label52";
            // 
            // trkbNSFHPF
            // 
            resources.ApplyResources(trkbNSFHPF, "trkbNSFHPF");
            trkbNSFHPF.Maximum = 256;
            trkbNSFHPF.Name = "trkbNSFHPF";
            trkbNSFHPF.TickFrequency = 10;
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(cbNSFDmc_DPCMReverse);
            groupBox10.Controls.Add(cbNSFDmc_RandomizeTri);
            groupBox10.Controls.Add(cbNSFDmc_TriMute);
            groupBox10.Controls.Add(cbNSFDmc_RandomizeNoise);
            groupBox10.Controls.Add(cbNSFDmc_DPCMAntiClick);
            groupBox10.Controls.Add(cbNSFDmc_EnablePNoise);
            groupBox10.Controls.Add(cbNSFDmc_Enable4011);
            groupBox10.Controls.Add(cbNSFDmc_NonLinearMixer);
            groupBox10.Controls.Add(cbNSFDmc_UnmuteOnReset);
            resources.ApplyResources(groupBox10, "groupBox10");
            groupBox10.Name = "groupBox10";
            groupBox10.TabStop = false;
            // 
            // cbNSFDmc_DPCMReverse
            // 
            resources.ApplyResources(cbNSFDmc_DPCMReverse, "cbNSFDmc_DPCMReverse");
            cbNSFDmc_DPCMReverse.Name = "cbNSFDmc_DPCMReverse";
            cbNSFDmc_DPCMReverse.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_RandomizeTri
            // 
            resources.ApplyResources(cbNSFDmc_RandomizeTri, "cbNSFDmc_RandomizeTri");
            cbNSFDmc_RandomizeTri.Name = "cbNSFDmc_RandomizeTri";
            cbNSFDmc_RandomizeTri.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_TriMute
            // 
            resources.ApplyResources(cbNSFDmc_TriMute, "cbNSFDmc_TriMute");
            cbNSFDmc_TriMute.Name = "cbNSFDmc_TriMute";
            cbNSFDmc_TriMute.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_RandomizeNoise
            // 
            resources.ApplyResources(cbNSFDmc_RandomizeNoise, "cbNSFDmc_RandomizeNoise");
            cbNSFDmc_RandomizeNoise.Name = "cbNSFDmc_RandomizeNoise";
            cbNSFDmc_RandomizeNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_DPCMAntiClick
            // 
            resources.ApplyResources(cbNSFDmc_DPCMAntiClick, "cbNSFDmc_DPCMAntiClick");
            cbNSFDmc_DPCMAntiClick.Name = "cbNSFDmc_DPCMAntiClick";
            cbNSFDmc_DPCMAntiClick.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_EnablePNoise
            // 
            resources.ApplyResources(cbNSFDmc_EnablePNoise, "cbNSFDmc_EnablePNoise");
            cbNSFDmc_EnablePNoise.Name = "cbNSFDmc_EnablePNoise";
            cbNSFDmc_EnablePNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_Enable4011
            // 
            resources.ApplyResources(cbNSFDmc_Enable4011, "cbNSFDmc_Enable4011");
            cbNSFDmc_Enable4011.Name = "cbNSFDmc_Enable4011";
            cbNSFDmc_Enable4011.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_NonLinearMixer
            // 
            resources.ApplyResources(cbNSFDmc_NonLinearMixer, "cbNSFDmc_NonLinearMixer");
            cbNSFDmc_NonLinearMixer.Name = "cbNSFDmc_NonLinearMixer";
            cbNSFDmc_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_UnmuteOnReset
            // 
            resources.ApplyResources(cbNSFDmc_UnmuteOnReset, "cbNSFDmc_UnmuteOnReset");
            cbNSFDmc_UnmuteOnReset.Name = "cbNSFDmc_UnmuteOnReset";
            cbNSFDmc_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            groupBox12.Controls.Add(cbNSFN160_Serial);
            resources.ApplyResources(groupBox12, "groupBox12");
            groupBox12.Name = "groupBox12";
            groupBox12.TabStop = false;
            // 
            // cbNSFN160_Serial
            // 
            resources.ApplyResources(cbNSFN160_Serial, "cbNSFN160_Serial");
            cbNSFN160_Serial.Name = "cbNSFN160_Serial";
            cbNSFN160_Serial.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(cbNSFMmc5_PhaseRefresh);
            groupBox11.Controls.Add(cbNSFMmc5_NonLinearMixer);
            resources.ApplyResources(groupBox11, "groupBox11");
            groupBox11.Name = "groupBox11";
            groupBox11.TabStop = false;
            // 
            // cbNSFMmc5_PhaseRefresh
            // 
            resources.ApplyResources(cbNSFMmc5_PhaseRefresh, "cbNSFMmc5_PhaseRefresh");
            cbNSFMmc5_PhaseRefresh.Name = "cbNSFMmc5_PhaseRefresh";
            cbNSFMmc5_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNSFMmc5_NonLinearMixer
            // 
            resources.ApplyResources(cbNSFMmc5_NonLinearMixer, "cbNSFMmc5_NonLinearMixer");
            cbNSFMmc5_NonLinearMixer.Name = "cbNSFMmc5_NonLinearMixer";
            cbNSFMmc5_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(cbNFSNes_DutySwap);
            groupBox9.Controls.Add(cbNFSNes_PhaseRefresh);
            groupBox9.Controls.Add(cbNFSNes_NonLinearMixer);
            groupBox9.Controls.Add(cbNFSNes_UnmuteOnReset);
            resources.ApplyResources(groupBox9, "groupBox9");
            groupBox9.Name = "groupBox9";
            groupBox9.TabStop = false;
            // 
            // cbNFSNes_DutySwap
            // 
            resources.ApplyResources(cbNFSNes_DutySwap, "cbNFSNes_DutySwap");
            cbNFSNes_DutySwap.Name = "cbNFSNes_DutySwap";
            cbNFSNes_DutySwap.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_PhaseRefresh
            // 
            resources.ApplyResources(cbNFSNes_PhaseRefresh, "cbNFSNes_PhaseRefresh");
            cbNFSNes_PhaseRefresh.Name = "cbNFSNes_PhaseRefresh";
            cbNFSNes_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_NonLinearMixer
            // 
            resources.ApplyResources(cbNFSNes_NonLinearMixer, "cbNFSNes_NonLinearMixer");
            cbNFSNes_NonLinearMixer.Name = "cbNFSNes_NonLinearMixer";
            cbNFSNes_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_UnmuteOnReset
            // 
            resources.ApplyResources(cbNFSNes_UnmuteOnReset, "cbNFSNes_UnmuteOnReset");
            cbNFSNes_UnmuteOnReset.Name = "cbNFSNes_UnmuteOnReset";
            cbNFSNes_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(label21);
            groupBox8.Controls.Add(label20);
            groupBox8.Controls.Add(tbNSFFds_LPF);
            groupBox8.Controls.Add(cbNFSFds_4085Reset);
            groupBox8.Controls.Add(cbNSFFDSWriteDisable8000);
            resources.ApplyResources(groupBox8, "groupBox8");
            groupBox8.Name = "groupBox8";
            groupBox8.TabStop = false;
            // 
            // label21
            // 
            resources.ApplyResources(label21, "label21");
            label21.Name = "label21";
            // 
            // label20
            // 
            resources.ApplyResources(label20, "label20");
            label20.Name = "label20";
            // 
            // tbNSFFds_LPF
            // 
            resources.ApplyResources(tbNSFFds_LPF, "tbNSFFds_LPF");
            tbNSFFds_LPF.Name = "tbNSFFds_LPF";
            // 
            // cbNFSFds_4085Reset
            // 
            resources.ApplyResources(cbNFSFds_4085Reset, "cbNFSFds_4085Reset");
            cbNFSFds_4085Reset.Name = "cbNFSFds_4085Reset";
            cbNFSFds_4085Reset.UseVisualStyleBackColor = true;
            // 
            // cbNSFFDSWriteDisable8000
            // 
            resources.ApplyResources(cbNSFFDSWriteDisable8000, "cbNSFFDSWriteDisable8000");
            cbNSFFDSWriteDisable8000.Name = "cbNSFFDSWriteDisable8000";
            cbNSFFDSWriteDisable8000.UseVisualStyleBackColor = true;
            // 
            // tpSID
            // 
            tpSID.Controls.Add(groupBox28);
            tpSID.Controls.Add(groupBox27);
            tpSID.Controls.Add(groupBox14);
            tpSID.Controls.Add(groupBox13);
            tpSID.Controls.Add(tbSIDOutputBufferSize);
            tpSID.Controls.Add(label51);
            tpSID.Controls.Add(label49);
            resources.ApplyResources(tpSID, "tpSID");
            tpSID.Name = "tpSID";
            tpSID.UseVisualStyleBackColor = true;
            // 
            // groupBox28
            // 
            groupBox28.Controls.Add(cbSIDModel_Force);
            groupBox28.Controls.Add(rbSIDModel_8580);
            groupBox28.Controls.Add(rbSIDModel_6581);
            resources.ApplyResources(groupBox28, "groupBox28");
            groupBox28.Name = "groupBox28";
            groupBox28.TabStop = false;
            // 
            // cbSIDModel_Force
            // 
            resources.ApplyResources(cbSIDModel_Force, "cbSIDModel_Force");
            cbSIDModel_Force.Name = "cbSIDModel_Force";
            cbSIDModel_Force.UseVisualStyleBackColor = true;
            // 
            // rbSIDModel_8580
            // 
            resources.ApplyResources(rbSIDModel_8580, "rbSIDModel_8580");
            rbSIDModel_8580.Name = "rbSIDModel_8580";
            rbSIDModel_8580.UseVisualStyleBackColor = true;
            // 
            // rbSIDModel_6581
            // 
            resources.ApplyResources(rbSIDModel_6581, "rbSIDModel_6581");
            rbSIDModel_6581.Checked = true;
            rbSIDModel_6581.Name = "rbSIDModel_6581";
            rbSIDModel_6581.TabStop = true;
            rbSIDModel_6581.UseVisualStyleBackColor = true;
            // 
            // groupBox27
            // 
            groupBox27.Controls.Add(cbSIDC64Model_Force);
            groupBox27.Controls.Add(rbSIDC64Model_DREAN);
            groupBox27.Controls.Add(rbSIDC64Model_OLDNTSC);
            groupBox27.Controls.Add(rbSIDC64Model_NTSC);
            groupBox27.Controls.Add(rbSIDC64Model_PAL);
            resources.ApplyResources(groupBox27, "groupBox27");
            groupBox27.Name = "groupBox27";
            groupBox27.TabStop = false;
            // 
            // cbSIDC64Model_Force
            // 
            resources.ApplyResources(cbSIDC64Model_Force, "cbSIDC64Model_Force");
            cbSIDC64Model_Force.Name = "cbSIDC64Model_Force";
            cbSIDC64Model_Force.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_DREAN
            // 
            resources.ApplyResources(rbSIDC64Model_DREAN, "rbSIDC64Model_DREAN");
            rbSIDC64Model_DREAN.Name = "rbSIDC64Model_DREAN";
            rbSIDC64Model_DREAN.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_OLDNTSC
            // 
            resources.ApplyResources(rbSIDC64Model_OLDNTSC, "rbSIDC64Model_OLDNTSC");
            rbSIDC64Model_OLDNTSC.Name = "rbSIDC64Model_OLDNTSC";
            rbSIDC64Model_OLDNTSC.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_NTSC
            // 
            resources.ApplyResources(rbSIDC64Model_NTSC, "rbSIDC64Model_NTSC");
            rbSIDC64Model_NTSC.Name = "rbSIDC64Model_NTSC";
            rbSIDC64Model_NTSC.UseVisualStyleBackColor = true;
            // 
            // rbSIDC64Model_PAL
            // 
            resources.ApplyResources(rbSIDC64Model_PAL, "rbSIDC64Model_PAL");
            rbSIDC64Model_PAL.Checked = true;
            rbSIDC64Model_PAL.Name = "rbSIDC64Model_PAL";
            rbSIDC64Model_PAL.TabStop = true;
            rbSIDC64Model_PAL.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(label27);
            groupBox14.Controls.Add(label26);
            groupBox14.Controls.Add(label25);
            groupBox14.Controls.Add(rdSIDQ1);
            groupBox14.Controls.Add(rdSIDQ3);
            groupBox14.Controls.Add(rdSIDQ2);
            groupBox14.Controls.Add(rdSIDQ4);
            resources.ApplyResources(groupBox14, "groupBox14");
            groupBox14.Name = "groupBox14";
            groupBox14.TabStop = false;
            // 
            // label27
            // 
            resources.ApplyResources(label27, "label27");
            label27.Name = "label27";
            // 
            // label26
            // 
            resources.ApplyResources(label26, "label26");
            label26.Name = "label26";
            // 
            // label25
            // 
            resources.ApplyResources(label25, "label25");
            label25.Name = "label25";
            // 
            // rdSIDQ1
            // 
            resources.ApplyResources(rdSIDQ1, "rdSIDQ1");
            rdSIDQ1.Checked = true;
            rdSIDQ1.Name = "rdSIDQ1";
            rdSIDQ1.TabStop = true;
            rdSIDQ1.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ3
            // 
            resources.ApplyResources(rdSIDQ3, "rdSIDQ3");
            rdSIDQ3.Name = "rdSIDQ3";
            rdSIDQ3.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ2
            // 
            resources.ApplyResources(rdSIDQ2, "rdSIDQ2");
            rdSIDQ2.Name = "rdSIDQ2";
            rdSIDQ2.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ4
            // 
            resources.ApplyResources(rdSIDQ4, "rdSIDQ4");
            rdSIDQ4.Name = "rdSIDQ4";
            rdSIDQ4.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            groupBox13.Controls.Add(btnSIDBasic);
            groupBox13.Controls.Add(btnSIDCharacter);
            groupBox13.Controls.Add(btnSIDKernal);
            groupBox13.Controls.Add(tbSIDCharacter);
            groupBox13.Controls.Add(tbSIDBasic);
            groupBox13.Controls.Add(tbSIDKernal);
            groupBox13.Controls.Add(label24);
            groupBox13.Controls.Add(label23);
            groupBox13.Controls.Add(label22);
            resources.ApplyResources(groupBox13, "groupBox13");
            groupBox13.Name = "groupBox13";
            groupBox13.TabStop = false;
            // 
            // btnSIDBasic
            // 
            resources.ApplyResources(btnSIDBasic, "btnSIDBasic");
            btnSIDBasic.Name = "btnSIDBasic";
            btnSIDBasic.UseVisualStyleBackColor = true;
            btnSIDBasic.Click += BtnSIDBasic_Click;
            // 
            // btnSIDCharacter
            // 
            resources.ApplyResources(btnSIDCharacter, "btnSIDCharacter");
            btnSIDCharacter.Name = "btnSIDCharacter";
            btnSIDCharacter.UseVisualStyleBackColor = true;
            btnSIDCharacter.Click += BtnSIDCharacter_Click;
            // 
            // btnSIDKernal
            // 
            resources.ApplyResources(btnSIDKernal, "btnSIDKernal");
            btnSIDKernal.Name = "btnSIDKernal";
            btnSIDKernal.UseVisualStyleBackColor = true;
            btnSIDKernal.Click += BtnSIDKernal_Click;
            // 
            // tbSIDCharacter
            // 
            resources.ApplyResources(tbSIDCharacter, "tbSIDCharacter");
            tbSIDCharacter.Name = "tbSIDCharacter";
            // 
            // tbSIDBasic
            // 
            resources.ApplyResources(tbSIDBasic, "tbSIDBasic");
            tbSIDBasic.Name = "tbSIDBasic";
            // 
            // tbSIDKernal
            // 
            resources.ApplyResources(tbSIDKernal, "tbSIDKernal");
            tbSIDKernal.Name = "tbSIDKernal";
            // 
            // label24
            // 
            resources.ApplyResources(label24, "label24");
            label24.Name = "label24";
            // 
            // label23
            // 
            resources.ApplyResources(label23, "label23");
            label23.Name = "label23";
            // 
            // label22
            // 
            resources.ApplyResources(label22, "label22");
            label22.Name = "label22";
            // 
            // tbSIDOutputBufferSize
            // 
            resources.ApplyResources(tbSIDOutputBufferSize, "tbSIDOutputBufferSize");
            tbSIDOutputBufferSize.Name = "tbSIDOutputBufferSize";
            // 
            // label51
            // 
            resources.ApplyResources(label51, "label51");
            label51.Name = "label51";
            // 
            // label49
            // 
            resources.ApplyResources(label49, "label49");
            label49.Name = "label49";
            // 
            // tpPMDDotNET
            // 
            tpPMDDotNET.Controls.Add(rbPMDManual);
            tpPMDDotNET.Controls.Add(rbPMDAuto);
            tpPMDDotNET.Controls.Add(btnPMDResetDriverArguments);
            tpPMDDotNET.Controls.Add(label54);
            tpPMDDotNET.Controls.Add(btnPMDResetCompilerArhguments);
            tpPMDDotNET.Controls.Add(tbPMDDriverArguments);
            tpPMDDotNET.Controls.Add(label55);
            tpPMDDotNET.Controls.Add(tbPMDCompilerArguments);
            tpPMDDotNET.Controls.Add(gbPMDManual);
            resources.ApplyResources(tpPMDDotNET, "tpPMDDotNET");
            tpPMDDotNET.Name = "tpPMDDotNET";
            tpPMDDotNET.UseVisualStyleBackColor = true;
            // 
            // rbPMDManual
            // 
            resources.ApplyResources(rbPMDManual, "rbPMDManual");
            rbPMDManual.Name = "rbPMDManual";
            rbPMDManual.TabStop = true;
            rbPMDManual.UseVisualStyleBackColor = true;
            rbPMDManual.CheckedChanged += RbPMDManual_CheckedChanged;
            // 
            // rbPMDAuto
            // 
            resources.ApplyResources(rbPMDAuto, "rbPMDAuto");
            rbPMDAuto.Name = "rbPMDAuto";
            rbPMDAuto.TabStop = true;
            rbPMDAuto.UseVisualStyleBackColor = true;
            // 
            // btnPMDResetDriverArguments
            // 
            resources.ApplyResources(btnPMDResetDriverArguments, "btnPMDResetDriverArguments");
            btnPMDResetDriverArguments.Name = "btnPMDResetDriverArguments";
            btnPMDResetDriverArguments.UseVisualStyleBackColor = true;
            btnPMDResetDriverArguments.Click += BtnPMDResetDriverArguments_Click;
            // 
            // label54
            // 
            resources.ApplyResources(label54, "label54");
            label54.Name = "label54";
            // 
            // btnPMDResetCompilerArhguments
            // 
            resources.ApplyResources(btnPMDResetCompilerArhguments, "btnPMDResetCompilerArhguments");
            btnPMDResetCompilerArhguments.Name = "btnPMDResetCompilerArhguments";
            btnPMDResetCompilerArhguments.UseVisualStyleBackColor = true;
            btnPMDResetCompilerArhguments.Click += BtnPMDResetCompilerArhguments_Click;
            // 
            // tbPMDDriverArguments
            // 
            resources.ApplyResources(tbPMDDriverArguments, "tbPMDDriverArguments");
            tbPMDDriverArguments.Name = "tbPMDDriverArguments";
            // 
            // label55
            // 
            resources.ApplyResources(label55, "label55");
            label55.Name = "label55";
            // 
            // tbPMDCompilerArguments
            // 
            resources.ApplyResources(tbPMDCompilerArguments, "tbPMDCompilerArguments");
            tbPMDCompilerArguments.Name = "tbPMDCompilerArguments";
            // 
            // gbPMDManual
            // 
            resources.ApplyResources(gbPMDManual, "gbPMDManual");
            gbPMDManual.Controls.Add(cbPMDSetManualVolume);
            gbPMDManual.Controls.Add(cbPMDUsePPZ8);
            gbPMDManual.Controls.Add(groupBox32);
            gbPMDManual.Controls.Add(cbPMDUsePPSDRV);
            gbPMDManual.Controls.Add(gbPPSDRV);
            gbPMDManual.Controls.Add(gbPMDSetManualVolume);
            gbPMDManual.Name = "gbPMDManual";
            gbPMDManual.TabStop = false;
            // 
            // cbPMDSetManualVolume
            // 
            resources.ApplyResources(cbPMDSetManualVolume, "cbPMDSetManualVolume");
            cbPMDSetManualVolume.Name = "cbPMDSetManualVolume";
            cbPMDSetManualVolume.UseVisualStyleBackColor = true;
            cbPMDSetManualVolume.CheckedChanged += CbPMDSetManualVolume_CheckedChanged;
            // 
            // cbPMDUsePPZ8
            // 
            resources.ApplyResources(cbPMDUsePPZ8, "cbPMDUsePPZ8");
            cbPMDUsePPZ8.Name = "cbPMDUsePPZ8";
            cbPMDUsePPZ8.UseVisualStyleBackColor = true;
            // 
            // groupBox32
            // 
            groupBox32.Controls.Add(rbPMD86B);
            groupBox32.Controls.Add(rbPMDSpbB);
            groupBox32.Controls.Add(rbPMDNrmB);
            resources.ApplyResources(groupBox32, "groupBox32");
            groupBox32.Name = "groupBox32";
            groupBox32.TabStop = false;
            // 
            // rbPMD86B
            // 
            resources.ApplyResources(rbPMD86B, "rbPMD86B");
            rbPMD86B.Name = "rbPMD86B";
            rbPMD86B.TabStop = true;
            rbPMD86B.UseVisualStyleBackColor = true;
            // 
            // rbPMDSpbB
            // 
            resources.ApplyResources(rbPMDSpbB, "rbPMDSpbB");
            rbPMDSpbB.Name = "rbPMDSpbB";
            rbPMDSpbB.TabStop = true;
            rbPMDSpbB.UseVisualStyleBackColor = true;
            // 
            // rbPMDNrmB
            // 
            resources.ApplyResources(rbPMDNrmB, "rbPMDNrmB");
            rbPMDNrmB.Name = "rbPMDNrmB";
            rbPMDNrmB.TabStop = true;
            rbPMDNrmB.UseVisualStyleBackColor = true;
            // 
            // cbPMDUsePPSDRV
            // 
            resources.ApplyResources(cbPMDUsePPSDRV, "cbPMDUsePPSDRV");
            cbPMDUsePPSDRV.Name = "cbPMDUsePPSDRV";
            cbPMDUsePPSDRV.UseVisualStyleBackColor = true;
            cbPMDUsePPSDRV.CheckedChanged += CbPMDUsePPSDRV_CheckedChanged;
            // 
            // gbPPSDRV
            // 
            resources.ApplyResources(gbPPSDRV, "gbPPSDRV");
            gbPPSDRV.Controls.Add(groupBox33);
            gbPPSDRV.Name = "gbPPSDRV";
            gbPPSDRV.TabStop = false;
            // 
            // groupBox33
            // 
            resources.ApplyResources(groupBox33, "groupBox33");
            groupBox33.Controls.Add(rbPMDUsePPSDRVManualFreq);
            groupBox33.Controls.Add(label56);
            groupBox33.Controls.Add(rbPMDUsePPSDRVFreqDefault);
            groupBox33.Controls.Add(btnPMDPPSDRVManualWait);
            groupBox33.Controls.Add(label57);
            groupBox33.Controls.Add(tbPMDPPSDRVFreq);
            groupBox33.Controls.Add(label58);
            groupBox33.Controls.Add(tbPMDPPSDRVManualWait);
            groupBox33.Name = "groupBox33";
            groupBox33.TabStop = false;
            // 
            // rbPMDUsePPSDRVManualFreq
            // 
            resources.ApplyResources(rbPMDUsePPSDRVManualFreq, "rbPMDUsePPSDRVManualFreq");
            rbPMDUsePPSDRVManualFreq.Name = "rbPMDUsePPSDRVManualFreq";
            rbPMDUsePPSDRVManualFreq.TabStop = true;
            rbPMDUsePPSDRVManualFreq.UseVisualStyleBackColor = true;
            rbPMDUsePPSDRVManualFreq.CheckedChanged += RbPMDUsePPSDRVManualFreq_CheckedChanged;
            // 
            // label56
            // 
            resources.ApplyResources(label56, "label56");
            label56.Name = "label56";
            // 
            // rbPMDUsePPSDRVFreqDefault
            // 
            resources.ApplyResources(rbPMDUsePPSDRVFreqDefault, "rbPMDUsePPSDRVFreqDefault");
            rbPMDUsePPSDRVFreqDefault.Name = "rbPMDUsePPSDRVFreqDefault";
            rbPMDUsePPSDRVFreqDefault.TabStop = true;
            rbPMDUsePPSDRVFreqDefault.UseVisualStyleBackColor = true;
            // 
            // btnPMDPPSDRVManualWait
            // 
            resources.ApplyResources(btnPMDPPSDRVManualWait, "btnPMDPPSDRVManualWait");
            btnPMDPPSDRVManualWait.Name = "btnPMDPPSDRVManualWait";
            btnPMDPPSDRVManualWait.UseVisualStyleBackColor = true;
            btnPMDPPSDRVManualWait.Click += BtnPMDPPSDRVManualWait_Click;
            // 
            // label57
            // 
            resources.ApplyResources(label57, "label57");
            label57.Name = "label57";
            // 
            // tbPMDPPSDRVFreq
            // 
            resources.ApplyResources(tbPMDPPSDRVFreq, "tbPMDPPSDRVFreq");
            tbPMDPPSDRVFreq.Name = "tbPMDPPSDRVFreq";
            tbPMDPPSDRVFreq.Click += TbPMDPPSDRVFreq_Click;
            tbPMDPPSDRVFreq.MouseClick += TbPMDPPSDRVFreq_MouseClick;
            // 
            // label58
            // 
            resources.ApplyResources(label58, "label58");
            label58.Name = "label58";
            // 
            // tbPMDPPSDRVManualWait
            // 
            resources.ApplyResources(tbPMDPPSDRVManualWait, "tbPMDPPSDRVManualWait");
            tbPMDPPSDRVManualWait.Name = "tbPMDPPSDRVManualWait";
            // 
            // gbPMDSetManualVolume
            // 
            resources.ApplyResources(gbPMDSetManualVolume, "gbPMDSetManualVolume");
            gbPMDSetManualVolume.Controls.Add(label59);
            gbPMDSetManualVolume.Controls.Add(label60);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeAdpcm);
            gbPMDSetManualVolume.Controls.Add(label61);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeRhythm);
            gbPMDSetManualVolume.Controls.Add(label62);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeSSG);
            gbPMDSetManualVolume.Controls.Add(label63);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeGIMICSSG);
            gbPMDSetManualVolume.Controls.Add(label64);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeFM);
            gbPMDSetManualVolume.Name = "gbPMDSetManualVolume";
            gbPMDSetManualVolume.TabStop = false;
            // 
            // label59
            // 
            resources.ApplyResources(label59, "label59");
            label59.Name = "label59";
            // 
            // label60
            // 
            resources.ApplyResources(label60, "label60");
            label60.Name = "label60";
            // 
            // tbPMDVolumeAdpcm
            // 
            resources.ApplyResources(tbPMDVolumeAdpcm, "tbPMDVolumeAdpcm");
            tbPMDVolumeAdpcm.Name = "tbPMDVolumeAdpcm";
            // 
            // label61
            // 
            resources.ApplyResources(label61, "label61");
            label61.Name = "label61";
            // 
            // tbPMDVolumeRhythm
            // 
            resources.ApplyResources(tbPMDVolumeRhythm, "tbPMDVolumeRhythm");
            tbPMDVolumeRhythm.Name = "tbPMDVolumeRhythm";
            // 
            // label62
            // 
            resources.ApplyResources(label62, "label62");
            label62.Name = "label62";
            // 
            // tbPMDVolumeSSG
            // 
            resources.ApplyResources(tbPMDVolumeSSG, "tbPMDVolumeSSG");
            tbPMDVolumeSSG.Name = "tbPMDVolumeSSG";
            // 
            // label63
            // 
            resources.ApplyResources(label63, "label63");
            label63.Name = "label63";
            // 
            // tbPMDVolumeGIMICSSG
            // 
            resources.ApplyResources(tbPMDVolumeGIMICSSG, "tbPMDVolumeGIMICSSG");
            tbPMDVolumeGIMICSSG.Name = "tbPMDVolumeGIMICSSG";
            // 
            // label64
            // 
            resources.ApplyResources(label64, "label64");
            label64.Name = "label64";
            // 
            // tbPMDVolumeFM
            // 
            resources.ApplyResources(tbPMDVolumeFM, "tbPMDVolumeFM");
            tbPMDVolumeFM.Name = "tbPMDVolumeFM";
            // 
            // tpMIDIOut
            // 
            tpMIDIOut.Controls.Add(btnAddVST);
            tpMIDIOut.Controls.Add(tbcMIDIoutList);
            tpMIDIOut.Controls.Add(btnSubMIDIout);
            tpMIDIOut.Controls.Add(btnAddMIDIout);
            tpMIDIOut.Controls.Add(label18);
            tpMIDIOut.Controls.Add(dgvMIDIoutPallet);
            tpMIDIOut.Controls.Add(label16);
            resources.ApplyResources(tpMIDIOut, "tpMIDIOut");
            tpMIDIOut.Name = "tpMIDIOut";
            tpMIDIOut.UseVisualStyleBackColor = true;
            // 
            // btnAddVST
            // 
            resources.ApplyResources(btnAddVST, "btnAddVST");
            btnAddVST.Name = "btnAddVST";
            btnAddVST.UseVisualStyleBackColor = true;
            btnAddVST.Click += BtnAddVST_Click;
            // 
            // tbcMIDIoutList
            // 
            resources.ApplyResources(tbcMIDIoutList, "tbcMIDIoutList");
            tbcMIDIoutList.Controls.Add(tabPage1);
            tbcMIDIoutList.Controls.Add(tabPage2);
            tbcMIDIoutList.Controls.Add(tabPage3);
            tbcMIDIoutList.Controls.Add(tabPage4);
            tbcMIDIoutList.Controls.Add(tabPage5);
            tbcMIDIoutList.Controls.Add(tabPage6);
            tbcMIDIoutList.Controls.Add(tabPage7);
            tbcMIDIoutList.Controls.Add(tabPage8);
            tbcMIDIoutList.Controls.Add(tabPage9);
            tbcMIDIoutList.Controls.Add(tabPage10);
            tbcMIDIoutList.Name = "tbcMIDIoutList";
            tbcMIDIoutList.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvMIDIoutListA);
            tabPage1.Controls.Add(btnUP_A);
            tabPage1.Controls.Add(btnDOWN_A);
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            tabPage1.Tag = "0";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListA
            // 
            dgvMIDIoutListA.AllowUserToAddRows = false;
            dgvMIDIoutListA.AllowUserToDeleteRows = false;
            dgvMIDIoutListA.AllowUserToResizeRows = false;
            resources.ApplyResources(dgvMIDIoutListA, "dgvMIDIoutListA");
            dgvMIDIoutListA.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutListA.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, clmIsVST, clmFileName, dataGridViewTextBoxColumn2, clmType, ClmBeforeSend, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4 });
            dgvMIDIoutListA.MultiSelect = false;
            dgvMIDIoutListA.Name = "dgvMIDIoutListA";
            dgvMIDIoutListA.RowHeadersVisible = false;
            dgvMIDIoutListA.RowTemplate.Height = 21;
            dgvMIDIoutListA.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.Frozen = true;
            resources.ApplyResources(dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmIsVST
            // 
            resources.ApplyResources(clmIsVST, "clmIsVST");
            clmIsVST.Name = "clmIsVST";
            // 
            // clmFileName
            // 
            resources.ApplyResources(clmFileName, "clmFileName");
            clmFileName.Name = "clmFileName";
            clmFileName.Resizable = DataGridViewTriState.True;
            clmFileName.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            resources.ApplyResources(dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmType
            // 
            resources.ApplyResources(clmType, "clmType");
            clmType.Items.AddRange(new object[] { "GM", "XG", "GS", "LA", "GS(SC-55_1)", "GS(SC-55_2)" });
            clmType.Name = "clmType";
            clmType.Resizable = DataGridViewTriState.True;
            // 
            // ClmBeforeSend
            // 
            resources.ApplyResources(ClmBeforeSend, "ClmBeforeSend");
            ClmBeforeSend.Items.AddRange(new object[] { "None", "GM Reset", "XG Reset", "GS Reset", "Custom" });
            ClmBeforeSend.Name = "ClmBeforeSend";
            ClmBeforeSend.Resizable = DataGridViewTriState.True;
            ClmBeforeSend.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn3
            // 
            resources.ApplyResources(dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            dataGridViewTextBoxColumn3.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            dataGridViewTextBoxColumn4.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // btnUP_A
            // 
            resources.ApplyResources(btnUP_A, "btnUP_A");
            btnUP_A.Name = "btnUP_A";
            btnUP_A.UseVisualStyleBackColor = true;
            btnUP_A.Click += BtnUP_Click;
            // 
            // btnDOWN_A
            // 
            resources.ApplyResources(btnDOWN_A, "btnDOWN_A");
            btnDOWN_A.Name = "btnDOWN_A";
            btnDOWN_A.UseVisualStyleBackColor = true;
            btnDOWN_A.Click += BtnDOWN_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dgvMIDIoutListB);
            tabPage2.Controls.Add(btnUP_B);
            tabPage2.Controls.Add(btnDOWN_B);
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Name = "tabPage2";
            tabPage2.Tag = "1";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListB
            // 
            dgvMIDIoutListB.AllowUserToAddRows = false;
            dgvMIDIoutListB.AllowUserToDeleteRows = false;
            dgvMIDIoutListB.AllowUserToResizeRows = false;
            dgvMIDIoutListB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListB, "dgvMIDIoutListB");
            dgvMIDIoutListB.MultiSelect = false;
            dgvMIDIoutListB.Name = "dgvMIDIoutListB";
            dgvMIDIoutListB.RowHeadersVisible = false;
            dgvMIDIoutListB.RowTemplate.Height = 21;
            dgvMIDIoutListB.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_B
            // 
            resources.ApplyResources(btnUP_B, "btnUP_B");
            btnUP_B.Name = "btnUP_B";
            btnUP_B.UseVisualStyleBackColor = true;
            btnUP_B.Click += BtnUP_Click;
            // 
            // btnDOWN_B
            // 
            resources.ApplyResources(btnDOWN_B, "btnDOWN_B");
            btnDOWN_B.Name = "btnDOWN_B";
            btnDOWN_B.UseVisualStyleBackColor = true;
            btnDOWN_B.Click += BtnDOWN_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgvMIDIoutListC);
            tabPage3.Controls.Add(btnUP_C);
            tabPage3.Controls.Add(btnDOWN_C);
            resources.ApplyResources(tabPage3, "tabPage3");
            tabPage3.Name = "tabPage3";
            tabPage3.Tag = "2";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListC
            // 
            dgvMIDIoutListC.AllowUserToAddRows = false;
            dgvMIDIoutListC.AllowUserToDeleteRows = false;
            dgvMIDIoutListC.AllowUserToResizeRows = false;
            dgvMIDIoutListC.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListC, "dgvMIDIoutListC");
            dgvMIDIoutListC.MultiSelect = false;
            dgvMIDIoutListC.Name = "dgvMIDIoutListC";
            dgvMIDIoutListC.RowHeadersVisible = false;
            dgvMIDIoutListC.RowTemplate.Height = 21;
            dgvMIDIoutListC.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_C
            // 
            resources.ApplyResources(btnUP_C, "btnUP_C");
            btnUP_C.Name = "btnUP_C";
            btnUP_C.UseVisualStyleBackColor = true;
            btnUP_C.Click += BtnUP_Click;
            // 
            // btnDOWN_C
            // 
            resources.ApplyResources(btnDOWN_C, "btnDOWN_C");
            btnDOWN_C.Name = "btnDOWN_C";
            btnDOWN_C.UseVisualStyleBackColor = true;
            btnDOWN_C.Click += BtnDOWN_Click;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(dgvMIDIoutListD);
            tabPage4.Controls.Add(btnUP_D);
            tabPage4.Controls.Add(btnDOWN_D);
            resources.ApplyResources(tabPage4, "tabPage4");
            tabPage4.Name = "tabPage4";
            tabPage4.Tag = "3";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListD
            // 
            dgvMIDIoutListD.AllowUserToAddRows = false;
            dgvMIDIoutListD.AllowUserToDeleteRows = false;
            dgvMIDIoutListD.AllowUserToResizeRows = false;
            dgvMIDIoutListD.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListD, "dgvMIDIoutListD");
            dgvMIDIoutListD.MultiSelect = false;
            dgvMIDIoutListD.Name = "dgvMIDIoutListD";
            dgvMIDIoutListD.RowHeadersVisible = false;
            dgvMIDIoutListD.RowTemplate.Height = 21;
            dgvMIDIoutListD.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_D
            // 
            resources.ApplyResources(btnUP_D, "btnUP_D");
            btnUP_D.Name = "btnUP_D";
            btnUP_D.UseVisualStyleBackColor = true;
            btnUP_D.Click += BtnUP_Click;
            // 
            // btnDOWN_D
            // 
            resources.ApplyResources(btnDOWN_D, "btnDOWN_D");
            btnDOWN_D.Name = "btnDOWN_D";
            btnDOWN_D.UseVisualStyleBackColor = true;
            btnDOWN_D.Click += BtnDOWN_Click;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(dgvMIDIoutListE);
            tabPage5.Controls.Add(btnUP_E);
            tabPage5.Controls.Add(btnDOWN_E);
            resources.ApplyResources(tabPage5, "tabPage5");
            tabPage5.Name = "tabPage5";
            tabPage5.Tag = "4";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListE
            // 
            dgvMIDIoutListE.AllowUserToAddRows = false;
            dgvMIDIoutListE.AllowUserToDeleteRows = false;
            dgvMIDIoutListE.AllowUserToResizeRows = false;
            dgvMIDIoutListE.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListE, "dgvMIDIoutListE");
            dgvMIDIoutListE.MultiSelect = false;
            dgvMIDIoutListE.Name = "dgvMIDIoutListE";
            dgvMIDIoutListE.RowHeadersVisible = false;
            dgvMIDIoutListE.RowTemplate.Height = 21;
            dgvMIDIoutListE.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_E
            // 
            resources.ApplyResources(btnUP_E, "btnUP_E");
            btnUP_E.Name = "btnUP_E";
            btnUP_E.UseVisualStyleBackColor = true;
            btnUP_E.Click += BtnUP_Click;
            // 
            // btnDOWN_E
            // 
            resources.ApplyResources(btnDOWN_E, "btnDOWN_E");
            btnDOWN_E.Name = "btnDOWN_E";
            btnDOWN_E.UseVisualStyleBackColor = true;
            btnDOWN_E.Click += BtnDOWN_Click;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(dgvMIDIoutListF);
            tabPage6.Controls.Add(btnUP_F);
            tabPage6.Controls.Add(btnDOWN_F);
            resources.ApplyResources(tabPage6, "tabPage6");
            tabPage6.Name = "tabPage6";
            tabPage6.Tag = "5";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListF
            // 
            dgvMIDIoutListF.AllowUserToAddRows = false;
            dgvMIDIoutListF.AllowUserToDeleteRows = false;
            dgvMIDIoutListF.AllowUserToResizeRows = false;
            dgvMIDIoutListF.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListF, "dgvMIDIoutListF");
            dgvMIDIoutListF.MultiSelect = false;
            dgvMIDIoutListF.Name = "dgvMIDIoutListF";
            dgvMIDIoutListF.RowHeadersVisible = false;
            dgvMIDIoutListF.RowTemplate.Height = 21;
            dgvMIDIoutListF.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_F
            // 
            resources.ApplyResources(btnUP_F, "btnUP_F");
            btnUP_F.Name = "btnUP_F";
            btnUP_F.UseVisualStyleBackColor = true;
            btnUP_F.Click += BtnUP_Click;
            // 
            // btnDOWN_F
            // 
            resources.ApplyResources(btnDOWN_F, "btnDOWN_F");
            btnDOWN_F.Name = "btnDOWN_F";
            btnDOWN_F.UseVisualStyleBackColor = true;
            btnDOWN_F.Click += BtnDOWN_Click;
            // 
            // tabPage7
            // 
            tabPage7.Controls.Add(dgvMIDIoutListG);
            tabPage7.Controls.Add(btnUP_G);
            tabPage7.Controls.Add(btnDOWN_G);
            resources.ApplyResources(tabPage7, "tabPage7");
            tabPage7.Name = "tabPage7";
            tabPage7.Tag = "6";
            tabPage7.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListG
            // 
            dgvMIDIoutListG.AllowUserToAddRows = false;
            dgvMIDIoutListG.AllowUserToDeleteRows = false;
            dgvMIDIoutListG.AllowUserToResizeRows = false;
            dgvMIDIoutListG.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListG, "dgvMIDIoutListG");
            dgvMIDIoutListG.MultiSelect = false;
            dgvMIDIoutListG.Name = "dgvMIDIoutListG";
            dgvMIDIoutListG.RowHeadersVisible = false;
            dgvMIDIoutListG.RowTemplate.Height = 21;
            dgvMIDIoutListG.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_G
            // 
            resources.ApplyResources(btnUP_G, "btnUP_G");
            btnUP_G.Name = "btnUP_G";
            btnUP_G.UseVisualStyleBackColor = true;
            btnUP_G.Click += BtnUP_Click;
            // 
            // btnDOWN_G
            // 
            resources.ApplyResources(btnDOWN_G, "btnDOWN_G");
            btnDOWN_G.Name = "btnDOWN_G";
            btnDOWN_G.UseVisualStyleBackColor = true;
            btnDOWN_G.Click += BtnDOWN_Click;
            // 
            // tabPage8
            // 
            tabPage8.Controls.Add(dgvMIDIoutListH);
            tabPage8.Controls.Add(btnUP_H);
            tabPage8.Controls.Add(btnDOWN_H);
            resources.ApplyResources(tabPage8, "tabPage8");
            tabPage8.Name = "tabPage8";
            tabPage8.Tag = "7";
            tabPage8.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListH
            // 
            dgvMIDIoutListH.AllowUserToAddRows = false;
            dgvMIDIoutListH.AllowUserToDeleteRows = false;
            dgvMIDIoutListH.AllowUserToResizeRows = false;
            dgvMIDIoutListH.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListH, "dgvMIDIoutListH");
            dgvMIDIoutListH.MultiSelect = false;
            dgvMIDIoutListH.Name = "dgvMIDIoutListH";
            dgvMIDIoutListH.RowHeadersVisible = false;
            dgvMIDIoutListH.RowTemplate.Height = 21;
            dgvMIDIoutListH.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_H
            // 
            resources.ApplyResources(btnUP_H, "btnUP_H");
            btnUP_H.Name = "btnUP_H";
            btnUP_H.UseVisualStyleBackColor = true;
            btnUP_H.Click += BtnUP_Click;
            // 
            // btnDOWN_H
            // 
            resources.ApplyResources(btnDOWN_H, "btnDOWN_H");
            btnDOWN_H.Name = "btnDOWN_H";
            btnDOWN_H.UseVisualStyleBackColor = true;
            btnDOWN_H.Click += BtnDOWN_Click;
            // 
            // tabPage9
            // 
            tabPage9.Controls.Add(dgvMIDIoutListI);
            tabPage9.Controls.Add(btnUP_I);
            tabPage9.Controls.Add(btnDOWN_I);
            resources.ApplyResources(tabPage9, "tabPage9");
            tabPage9.Name = "tabPage9";
            tabPage9.Tag = "8";
            tabPage9.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListI
            // 
            dgvMIDIoutListI.AllowUserToAddRows = false;
            dgvMIDIoutListI.AllowUserToDeleteRows = false;
            dgvMIDIoutListI.AllowUserToResizeRows = false;
            dgvMIDIoutListI.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListI, "dgvMIDIoutListI");
            dgvMIDIoutListI.MultiSelect = false;
            dgvMIDIoutListI.Name = "dgvMIDIoutListI";
            dgvMIDIoutListI.RowHeadersVisible = false;
            dgvMIDIoutListI.RowTemplate.Height = 21;
            dgvMIDIoutListI.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // btnUP_I
            // 
            resources.ApplyResources(btnUP_I, "btnUP_I");
            btnUP_I.Name = "btnUP_I";
            btnUP_I.UseVisualStyleBackColor = true;
            btnUP_I.Click += BtnUP_Click;
            // 
            // btnDOWN_I
            // 
            resources.ApplyResources(btnDOWN_I, "btnDOWN_I");
            btnDOWN_I.Name = "btnDOWN_I";
            btnDOWN_I.UseVisualStyleBackColor = true;
            btnDOWN_I.Click += BtnDOWN_Click;
            // 
            // tabPage10
            // 
            tabPage10.Controls.Add(dgvMIDIoutListJ);
            tabPage10.Controls.Add(button17);
            tabPage10.Controls.Add(btnDOWN_J);
            resources.ApplyResources(tabPage10, "tabPage10");
            tabPage10.Name = "tabPage10";
            tabPage10.Tag = "9";
            tabPage10.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListJ
            // 
            dgvMIDIoutListJ.AllowUserToAddRows = false;
            dgvMIDIoutListJ.AllowUserToDeleteRows = false;
            dgvMIDIoutListJ.AllowUserToResizeRows = false;
            dgvMIDIoutListJ.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(dgvMIDIoutListJ, "dgvMIDIoutListJ");
            dgvMIDIoutListJ.MultiSelect = false;
            dgvMIDIoutListJ.Name = "dgvMIDIoutListJ";
            dgvMIDIoutListJ.RowHeadersVisible = false;
            dgvMIDIoutListJ.RowTemplate.Height = 21;
            dgvMIDIoutListJ.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // button17
            // 
            resources.ApplyResources(button17, "button17");
            button17.Name = "button17";
            button17.UseVisualStyleBackColor = true;
            button17.Click += BtnUP_Click;
            // 
            // btnDOWN_J
            // 
            resources.ApplyResources(btnDOWN_J, "btnDOWN_J");
            btnDOWN_J.Name = "btnDOWN_J";
            btnDOWN_J.UseVisualStyleBackColor = true;
            btnDOWN_J.Click += BtnDOWN_Click;
            // 
            // btnSubMIDIout
            // 
            resources.ApplyResources(btnSubMIDIout, "btnSubMIDIout");
            btnSubMIDIout.Name = "btnSubMIDIout";
            btnSubMIDIout.UseVisualStyleBackColor = true;
            btnSubMIDIout.Click += BtnSubMIDIout_Click;
            // 
            // btnAddMIDIout
            // 
            resources.ApplyResources(btnAddMIDIout, "btnAddMIDIout");
            btnAddMIDIout.Name = "btnAddMIDIout";
            btnAddMIDIout.UseVisualStyleBackColor = true;
            btnAddMIDIout.Click += BtnAddMIDIout_Click;
            // 
            // label18
            // 
            resources.ApplyResources(label18, "label18");
            label18.Name = "label18";
            // 
            // dgvMIDIoutPallet
            // 
            dgvMIDIoutPallet.AllowUserToAddRows = false;
            dgvMIDIoutPallet.AllowUserToDeleteRows = false;
            dgvMIDIoutPallet.AllowUserToResizeRows = false;
            resources.ApplyResources(dgvMIDIoutPallet, "dgvMIDIoutPallet");
            dgvMIDIoutPallet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutPallet.Columns.AddRange(new DataGridViewColumn[] { clmID, clmDeviceName, clmManufacturer, clmSpacer });
            dgvMIDIoutPallet.MultiSelect = false;
            dgvMIDIoutPallet.Name = "dgvMIDIoutPallet";
            dgvMIDIoutPallet.RowHeadersVisible = false;
            dgvMIDIoutPallet.RowTemplate.Height = 21;
            dgvMIDIoutPallet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // clmID
            // 
            clmID.Frozen = true;
            resources.ApplyResources(clmID, "clmID");
            clmID.Name = "clmID";
            clmID.ReadOnly = true;
            // 
            // clmDeviceName
            // 
            clmDeviceName.Frozen = true;
            resources.ApplyResources(clmDeviceName, "clmDeviceName");
            clmDeviceName.Name = "clmDeviceName";
            clmDeviceName.ReadOnly = true;
            clmDeviceName.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmManufacturer
            // 
            clmManufacturer.Frozen = true;
            resources.ApplyResources(clmManufacturer, "clmManufacturer");
            clmManufacturer.Name = "clmManufacturer";
            clmManufacturer.ReadOnly = true;
            clmManufacturer.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSpacer
            // 
            clmSpacer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(clmSpacer, "clmSpacer");
            clmSpacer.Name = "clmSpacer";
            clmSpacer.ReadOnly = true;
            clmSpacer.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // label16
            // 
            resources.ApplyResources(label16, "label16");
            label16.Name = "label16";
            // 
            // tpMIDIOut2
            // 
            tpMIDIOut2.Controls.Add(groupBox15);
            resources.ApplyResources(tpMIDIOut2, "tpMIDIOut2");
            tpMIDIOut2.Name = "tpMIDIOut2";
            tpMIDIOut2.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            resources.ApplyResources(groupBox15, "groupBox15");
            groupBox15.Controls.Add(btnBeforeSend_Default);
            groupBox15.Controls.Add(tbBeforeSend_Custom);
            groupBox15.Controls.Add(tbBeforeSend_XGReset);
            groupBox15.Controls.Add(label35);
            groupBox15.Controls.Add(label34);
            groupBox15.Controls.Add(label32);
            groupBox15.Controls.Add(tbBeforeSend_GSReset);
            groupBox15.Controls.Add(label33);
            groupBox15.Controls.Add(tbBeforeSend_GMReset);
            groupBox15.Controls.Add(label31);
            groupBox15.Name = "groupBox15";
            groupBox15.TabStop = false;
            // 
            // btnBeforeSend_Default
            // 
            resources.ApplyResources(btnBeforeSend_Default, "btnBeforeSend_Default");
            btnBeforeSend_Default.Name = "btnBeforeSend_Default";
            btnBeforeSend_Default.UseVisualStyleBackColor = true;
            btnBeforeSend_Default.Click += BtnBeforeSend_Default_Click;
            // 
            // tbBeforeSend_Custom
            // 
            resources.ApplyResources(tbBeforeSend_Custom, "tbBeforeSend_Custom");
            tbBeforeSend_Custom.Name = "tbBeforeSend_Custom";
            // 
            // tbBeforeSend_XGReset
            // 
            resources.ApplyResources(tbBeforeSend_XGReset, "tbBeforeSend_XGReset");
            tbBeforeSend_XGReset.Name = "tbBeforeSend_XGReset";
            // 
            // label35
            // 
            resources.ApplyResources(label35, "label35");
            label35.Name = "label35";
            // 
            // label34
            // 
            resources.ApplyResources(label34, "label34");
            label34.Name = "label34";
            // 
            // label32
            // 
            resources.ApplyResources(label32, "label32");
            label32.Name = "label32";
            // 
            // tbBeforeSend_GSReset
            // 
            resources.ApplyResources(tbBeforeSend_GSReset, "tbBeforeSend_GSReset");
            tbBeforeSend_GSReset.Name = "tbBeforeSend_GSReset";
            // 
            // label33
            // 
            resources.ApplyResources(label33, "label33");
            label33.Name = "label33";
            // 
            // tbBeforeSend_GMReset
            // 
            resources.ApplyResources(tbBeforeSend_GMReset, "tbBeforeSend_GMReset");
            tbBeforeSend_GMReset.Name = "tbBeforeSend_GMReset";
            // 
            // label31
            // 
            resources.ApplyResources(label31, "label31");
            label31.Name = "label31";
            // 
            // tabMIDIExp
            // 
            tabMIDIExp.Controls.Add(cbUseMIDIExport);
            tabMIDIExp.Controls.Add(gbMIDIExport);
            resources.ApplyResources(tabMIDIExp, "tabMIDIExp");
            tabMIDIExp.Name = "tabMIDIExp";
            tabMIDIExp.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIExport
            // 
            resources.ApplyResources(cbUseMIDIExport, "cbUseMIDIExport");
            cbUseMIDIExport.Name = "cbUseMIDIExport";
            cbUseMIDIExport.UseVisualStyleBackColor = true;
            cbUseMIDIExport.CheckedChanged += CbUseMIDIExport_CheckedChanged;
            // 
            // gbMIDIExport
            // 
            resources.ApplyResources(gbMIDIExport, "gbMIDIExport");
            gbMIDIExport.Controls.Add(cbMIDIKeyOnFnum);
            gbMIDIExport.Controls.Add(cbMIDIUseVOPM);
            gbMIDIExport.Controls.Add(groupBox6);
            gbMIDIExport.Controls.Add(cbMIDIPlayless);
            gbMIDIExport.Controls.Add(btnMIDIOutputPath);
            gbMIDIExport.Controls.Add(lblOutputPath);
            gbMIDIExport.Controls.Add(tbMIDIOutputPath);
            gbMIDIExport.Name = "gbMIDIExport";
            gbMIDIExport.TabStop = false;
            // 
            // cbMIDIKeyOnFnum
            // 
            resources.ApplyResources(cbMIDIKeyOnFnum, "cbMIDIKeyOnFnum");
            cbMIDIKeyOnFnum.Name = "cbMIDIKeyOnFnum";
            cbMIDIKeyOnFnum.UseVisualStyleBackColor = true;
            // 
            // cbMIDIUseVOPM
            // 
            resources.ApplyResources(cbMIDIUseVOPM, "cbMIDIUseVOPM");
            cbMIDIUseVOPM.Name = "cbMIDIUseVOPM";
            cbMIDIUseVOPM.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(cbMIDIYM2612);
            groupBox6.Controls.Add(cbMIDISN76489Sec);
            groupBox6.Controls.Add(cbMIDIYM2612Sec);
            groupBox6.Controls.Add(cbMIDISN76489);
            groupBox6.Controls.Add(cbMIDIYM2151);
            groupBox6.Controls.Add(cbMIDIYM2610BSec);
            groupBox6.Controls.Add(cbMIDIYM2151Sec);
            groupBox6.Controls.Add(cbMIDIYM2610B);
            groupBox6.Controls.Add(cbMIDIYM2203);
            groupBox6.Controls.Add(cbMIDIYM2608Sec);
            groupBox6.Controls.Add(cbMIDIYM2203Sec);
            groupBox6.Controls.Add(cbMIDIYM2608);
            resources.ApplyResources(groupBox6, "groupBox6");
            groupBox6.Name = "groupBox6";
            groupBox6.TabStop = false;
            // 
            // cbMIDIYM2612
            // 
            resources.ApplyResources(cbMIDIYM2612, "cbMIDIYM2612");
            cbMIDIYM2612.Checked = true;
            cbMIDIYM2612.CheckState = CheckState.Checked;
            cbMIDIYM2612.Name = "cbMIDIYM2612";
            cbMIDIYM2612.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489Sec
            // 
            resources.ApplyResources(cbMIDISN76489Sec, "cbMIDISN76489Sec");
            cbMIDISN76489Sec.Name = "cbMIDISN76489Sec";
            cbMIDISN76489Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2612Sec
            // 
            resources.ApplyResources(cbMIDIYM2612Sec, "cbMIDIYM2612Sec");
            cbMIDIYM2612Sec.Name = "cbMIDIYM2612Sec";
            cbMIDIYM2612Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489
            // 
            resources.ApplyResources(cbMIDISN76489, "cbMIDISN76489");
            cbMIDISN76489.Name = "cbMIDISN76489";
            cbMIDISN76489.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151
            // 
            resources.ApplyResources(cbMIDIYM2151, "cbMIDIYM2151");
            cbMIDIYM2151.Name = "cbMIDIYM2151";
            cbMIDIYM2151.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610BSec
            // 
            resources.ApplyResources(cbMIDIYM2610BSec, "cbMIDIYM2610BSec");
            cbMIDIYM2610BSec.Name = "cbMIDIYM2610BSec";
            cbMIDIYM2610BSec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151Sec
            // 
            resources.ApplyResources(cbMIDIYM2151Sec, "cbMIDIYM2151Sec");
            cbMIDIYM2151Sec.Name = "cbMIDIYM2151Sec";
            cbMIDIYM2151Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610B
            // 
            resources.ApplyResources(cbMIDIYM2610B, "cbMIDIYM2610B");
            cbMIDIYM2610B.Name = "cbMIDIYM2610B";
            cbMIDIYM2610B.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203
            // 
            resources.ApplyResources(cbMIDIYM2203, "cbMIDIYM2203");
            cbMIDIYM2203.Name = "cbMIDIYM2203";
            cbMIDIYM2203.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608Sec
            // 
            resources.ApplyResources(cbMIDIYM2608Sec, "cbMIDIYM2608Sec");
            cbMIDIYM2608Sec.Name = "cbMIDIYM2608Sec";
            cbMIDIYM2608Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203Sec
            // 
            resources.ApplyResources(cbMIDIYM2203Sec, "cbMIDIYM2203Sec");
            cbMIDIYM2203Sec.Name = "cbMIDIYM2203Sec";
            cbMIDIYM2203Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608
            // 
            resources.ApplyResources(cbMIDIYM2608, "cbMIDIYM2608");
            cbMIDIYM2608.Name = "cbMIDIYM2608";
            cbMIDIYM2608.UseVisualStyleBackColor = true;
            // 
            // cbMIDIPlayless
            // 
            resources.ApplyResources(cbMIDIPlayless, "cbMIDIPlayless");
            cbMIDIPlayless.Name = "cbMIDIPlayless";
            cbMIDIPlayless.UseVisualStyleBackColor = true;
            // 
            // btnMIDIOutputPath
            // 
            resources.ApplyResources(btnMIDIOutputPath, "btnMIDIOutputPath");
            btnMIDIOutputPath.Name = "btnMIDIOutputPath";
            btnMIDIOutputPath.UseVisualStyleBackColor = true;
            btnMIDIOutputPath.Click += BtnMIDIOutputPath_Click;
            // 
            // lblOutputPath
            // 
            resources.ApplyResources(lblOutputPath, "lblOutputPath");
            lblOutputPath.Name = "lblOutputPath";
            // 
            // tbMIDIOutputPath
            // 
            resources.ApplyResources(tbMIDIOutputPath, "tbMIDIOutputPath");
            tbMIDIOutputPath.Name = "tbMIDIOutputPath";
            // 
            // tpMIDIKBD
            // 
            tpMIDIKBD.Controls.Add(cbUseMIDIKeyboard);
            tpMIDIKBD.Controls.Add(gbMIDIKeyboard);
            resources.ApplyResources(tpMIDIKBD, "tpMIDIKBD");
            tpMIDIKBD.Name = "tpMIDIKBD";
            tpMIDIKBD.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIKeyboard
            // 
            resources.ApplyResources(cbUseMIDIKeyboard, "cbUseMIDIKeyboard");
            cbUseMIDIKeyboard.Name = "cbUseMIDIKeyboard";
            cbUseMIDIKeyboard.UseVisualStyleBackColor = true;
            cbUseMIDIKeyboard.CheckedChanged += CbUseMIDIKeyboard_CheckedChanged;
            // 
            // gbMIDIKeyboard
            // 
            resources.ApplyResources(gbMIDIKeyboard, "gbMIDIKeyboard");
            gbMIDIKeyboard.Controls.Add(pictureBox8);
            gbMIDIKeyboard.Controls.Add(pictureBox7);
            gbMIDIKeyboard.Controls.Add(pictureBox6);
            gbMIDIKeyboard.Controls.Add(pictureBox5);
            gbMIDIKeyboard.Controls.Add(pictureBox4);
            gbMIDIKeyboard.Controls.Add(pictureBox3);
            gbMIDIKeyboard.Controls.Add(pictureBox2);
            gbMIDIKeyboard.Controls.Add(pictureBox1);
            gbMIDIKeyboard.Controls.Add(tbCCFadeout);
            gbMIDIKeyboard.Controls.Add(tbCCPause);
            gbMIDIKeyboard.Controls.Add(tbCCSlow);
            gbMIDIKeyboard.Controls.Add(tbCCPrevious);
            gbMIDIKeyboard.Controls.Add(tbCCNext);
            gbMIDIKeyboard.Controls.Add(tbCCFast);
            gbMIDIKeyboard.Controls.Add(tbCCStop);
            gbMIDIKeyboard.Controls.Add(tbCCPlay);
            gbMIDIKeyboard.Controls.Add(tbCCCopyLog);
            gbMIDIKeyboard.Controls.Add(label17);
            gbMIDIKeyboard.Controls.Add(tbCCDelLog);
            gbMIDIKeyboard.Controls.Add(label15);
            gbMIDIKeyboard.Controls.Add(tbCCChCopy);
            gbMIDIKeyboard.Controls.Add(label8);
            gbMIDIKeyboard.Controls.Add(label9);
            gbMIDIKeyboard.Controls.Add(gbUseChannel);
            gbMIDIKeyboard.Controls.Add(cmbMIDIIN);
            gbMIDIKeyboard.Controls.Add(label5);
            gbMIDIKeyboard.Name = "gbMIDIKeyboard";
            gbMIDIKeyboard.TabStop = false;
            // 
            // pictureBox8
            // 
            resources.ApplyResources(pictureBox8, "pictureBox8");
            pictureBox8.Name = "pictureBox8";
            pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            resources.ApplyResources(pictureBox7, "pictureBox7");
            pictureBox7.Name = "pictureBox7";
            pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            resources.ApplyResources(pictureBox6, "pictureBox6");
            pictureBox6.Name = "pictureBox6";
            pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            resources.ApplyResources(pictureBox5, "pictureBox5");
            pictureBox5.Name = "pictureBox5";
            pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            resources.ApplyResources(pictureBox4, "pictureBox4");
            pictureBox4.Name = "pictureBox4";
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            resources.ApplyResources(pictureBox3, "pictureBox3");
            pictureBox3.Name = "pictureBox3";
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(pictureBox2, "pictureBox2");
            pictureBox2.Name = "pictureBox2";
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // tbCCFadeout
            // 
            resources.ApplyResources(tbCCFadeout, "tbCCFadeout");
            tbCCFadeout.Name = "tbCCFadeout";
            // 
            // tbCCPause
            // 
            resources.ApplyResources(tbCCPause, "tbCCPause");
            tbCCPause.Name = "tbCCPause";
            // 
            // tbCCSlow
            // 
            resources.ApplyResources(tbCCSlow, "tbCCSlow");
            tbCCSlow.Name = "tbCCSlow";
            // 
            // tbCCPrevious
            // 
            resources.ApplyResources(tbCCPrevious, "tbCCPrevious");
            tbCCPrevious.Name = "tbCCPrevious";
            // 
            // tbCCNext
            // 
            resources.ApplyResources(tbCCNext, "tbCCNext");
            tbCCNext.Name = "tbCCNext";
            // 
            // tbCCFast
            // 
            resources.ApplyResources(tbCCFast, "tbCCFast");
            tbCCFast.Name = "tbCCFast";
            // 
            // tbCCStop
            // 
            resources.ApplyResources(tbCCStop, "tbCCStop");
            tbCCStop.Name = "tbCCStop";
            // 
            // tbCCPlay
            // 
            resources.ApplyResources(tbCCPlay, "tbCCPlay");
            tbCCPlay.Name = "tbCCPlay";
            // 
            // tbCCCopyLog
            // 
            resources.ApplyResources(tbCCCopyLog, "tbCCCopyLog");
            tbCCCopyLog.Name = "tbCCCopyLog";
            // 
            // label17
            // 
            resources.ApplyResources(label17, "label17");
            label17.Name = "label17";
            // 
            // tbCCDelLog
            // 
            resources.ApplyResources(tbCCDelLog, "tbCCDelLog");
            tbCCDelLog.Name = "tbCCDelLog";
            // 
            // label15
            // 
            resources.ApplyResources(label15, "label15");
            label15.Name = "label15";
            // 
            // tbCCChCopy
            // 
            resources.ApplyResources(tbCCChCopy, "tbCCChCopy");
            tbCCChCopy.Name = "tbCCChCopy";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            // 
            // gbUseChannel
            // 
            gbUseChannel.Controls.Add(rbMONO);
            gbUseChannel.Controls.Add(rbPOLY);
            gbUseChannel.Controls.Add(groupBox7);
            gbUseChannel.Controls.Add(groupBox2);
            resources.ApplyResources(gbUseChannel, "gbUseChannel");
            gbUseChannel.Name = "gbUseChannel";
            gbUseChannel.TabStop = false;
            // 
            // rbMONO
            // 
            resources.ApplyResources(rbMONO, "rbMONO");
            rbMONO.Checked = true;
            rbMONO.Name = "rbMONO";
            rbMONO.TabStop = true;
            rbMONO.UseVisualStyleBackColor = true;
            // 
            // rbPOLY
            // 
            resources.ApplyResources(rbPOLY, "rbPOLY");
            rbPOLY.Name = "rbPOLY";
            rbPOLY.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(rbFM6);
            groupBox7.Controls.Add(rbFM3);
            groupBox7.Controls.Add(rbFM5);
            groupBox7.Controls.Add(rbFM2);
            groupBox7.Controls.Add(rbFM4);
            groupBox7.Controls.Add(rbFM1);
            resources.ApplyResources(groupBox7, "groupBox7");
            groupBox7.Name = "groupBox7";
            groupBox7.TabStop = false;
            // 
            // rbFM6
            // 
            resources.ApplyResources(rbFM6, "rbFM6");
            rbFM6.Name = "rbFM6";
            rbFM6.UseVisualStyleBackColor = true;
            // 
            // rbFM3
            // 
            resources.ApplyResources(rbFM3, "rbFM3");
            rbFM3.Name = "rbFM3";
            rbFM3.UseVisualStyleBackColor = true;
            // 
            // rbFM5
            // 
            resources.ApplyResources(rbFM5, "rbFM5");
            rbFM5.Name = "rbFM5";
            rbFM5.UseVisualStyleBackColor = true;
            // 
            // rbFM2
            // 
            resources.ApplyResources(rbFM2, "rbFM2");
            rbFM2.Name = "rbFM2";
            rbFM2.UseVisualStyleBackColor = true;
            // 
            // rbFM4
            // 
            resources.ApplyResources(rbFM4, "rbFM4");
            rbFM4.Name = "rbFM4";
            rbFM4.UseVisualStyleBackColor = true;
            // 
            // rbFM1
            // 
            resources.ApplyResources(rbFM1, "rbFM1");
            rbFM1.Checked = true;
            rbFM1.Name = "rbFM1";
            rbFM1.TabStop = true;
            rbFM1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cbFM1);
            groupBox2.Controls.Add(cbFM6);
            groupBox2.Controls.Add(cbFM2);
            groupBox2.Controls.Add(cbFM5);
            groupBox2.Controls.Add(cbFM3);
            groupBox2.Controls.Add(cbFM4);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // cbFM1
            // 
            resources.ApplyResources(cbFM1, "cbFM1");
            cbFM1.Checked = true;
            cbFM1.CheckState = CheckState.Checked;
            cbFM1.Name = "cbFM1";
            cbFM1.UseVisualStyleBackColor = true;
            // 
            // cbFM6
            // 
            resources.ApplyResources(cbFM6, "cbFM6");
            cbFM6.Checked = true;
            cbFM6.CheckState = CheckState.Checked;
            cbFM6.Name = "cbFM6";
            cbFM6.UseVisualStyleBackColor = true;
            // 
            // cbFM2
            // 
            resources.ApplyResources(cbFM2, "cbFM2");
            cbFM2.Checked = true;
            cbFM2.CheckState = CheckState.Checked;
            cbFM2.Name = "cbFM2";
            cbFM2.UseVisualStyleBackColor = true;
            // 
            // cbFM5
            // 
            resources.ApplyResources(cbFM5, "cbFM5");
            cbFM5.Checked = true;
            cbFM5.CheckState = CheckState.Checked;
            cbFM5.Name = "cbFM5";
            cbFM5.UseVisualStyleBackColor = true;
            // 
            // cbFM3
            // 
            resources.ApplyResources(cbFM3, "cbFM3");
            cbFM3.Checked = true;
            cbFM3.CheckState = CheckState.Checked;
            cbFM3.Name = "cbFM3";
            cbFM3.UseVisualStyleBackColor = true;
            // 
            // cbFM4
            // 
            resources.ApplyResources(cbFM4, "cbFM4");
            cbFM4.Checked = true;
            cbFM4.CheckState = CheckState.Checked;
            cbFM4.Name = "cbFM4";
            cbFM4.UseVisualStyleBackColor = true;
            // 
            // cmbMIDIIN
            // 
            cmbMIDIIN.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMIDIIN.FormattingEnabled = true;
            resources.ApplyResources(cmbMIDIIN, "cmbMIDIIN");
            cmbMIDIIN.Name = "cmbMIDIIN";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // tpKeyBoard
            // 
            tpKeyBoard.Controls.Add(cbUseKeyBoardHook);
            tpKeyBoard.Controls.Add(gbUseKeyBoardHook);
            tpKeyBoard.Controls.Add(label47);
            tpKeyBoard.Controls.Add(cbStopWin);
            tpKeyBoard.Controls.Add(cbPauseWin);
            tpKeyBoard.Controls.Add(cbFadeoutWin);
            tpKeyBoard.Controls.Add(cbPrevWin);
            tpKeyBoard.Controls.Add(cbSlowWin);
            tpKeyBoard.Controls.Add(cbPlayWin);
            tpKeyBoard.Controls.Add(cbFastWin);
            tpKeyBoard.Controls.Add(cbNextWin);
            resources.ApplyResources(tpKeyBoard, "tpKeyBoard");
            tpKeyBoard.Name = "tpKeyBoard";
            tpKeyBoard.UseVisualStyleBackColor = true;
            // 
            // cbUseKeyBoardHook
            // 
            resources.ApplyResources(cbUseKeyBoardHook, "cbUseKeyBoardHook");
            cbUseKeyBoardHook.Name = "cbUseKeyBoardHook";
            cbUseKeyBoardHook.UseVisualStyleBackColor = true;
            cbUseKeyBoardHook.CheckedChanged += CbUseKeyBoardHook_CheckedChanged;
            // 
            // gbUseKeyBoardHook
            // 
            resources.ApplyResources(gbUseKeyBoardHook, "gbUseKeyBoardHook");
            gbUseKeyBoardHook.Controls.Add(panel1);
            gbUseKeyBoardHook.Controls.Add(lblKeyBoardHookNotice);
            gbUseKeyBoardHook.Name = "gbUseKeyBoardHook";
            gbUseKeyBoardHook.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(btPpcClr);
            panel1.Controls.Add(cbPpcShift);
            panel1.Controls.Add(btPpcSet);
            panel1.Controls.Add(label76);
            panel1.Controls.Add(lblPpcKey);
            panel1.Controls.Add(cbPpcAlt);
            panel1.Controls.Add(cbPpcCtrl);
            panel1.Controls.Add(btDpcClr);
            panel1.Controls.Add(cbDpcShift);
            panel1.Controls.Add(btDpcSet);
            panel1.Controls.Add(label74);
            panel1.Controls.Add(lblDpcKey);
            panel1.Controls.Add(cbDpcAlt);
            panel1.Controls.Add(cbDpcCtrl);
            panel1.Controls.Add(btUpcClr);
            panel1.Controls.Add(btRmvClr);
            panel1.Controls.Add(cbUpcShift);
            panel1.Controls.Add(pictureBox14);
            panel1.Controls.Add(cbRmvShift);
            panel1.Controls.Add(btUpcSet);
            panel1.Controls.Add(cbFastShift);
            panel1.Controls.Add(label72);
            panel1.Controls.Add(btDmvClr);
            panel1.Controls.Add(lblUpcKey);
            panel1.Controls.Add(btRmvSet);
            panel1.Controls.Add(cbUpcAlt);
            panel1.Controls.Add(cbDmvShift);
            panel1.Controls.Add(cbUpcCtrl);
            panel1.Controls.Add(label69);
            panel1.Controls.Add(btUmvClr);
            panel1.Controls.Add(cbRmvAlt);
            panel1.Controls.Add(cbUmvShift);
            panel1.Controls.Add(cbRmvCtrl);
            panel1.Controls.Add(btNextClr);
            panel1.Controls.Add(lblRmvKey);
            panel1.Controls.Add(cbNextShift);
            panel1.Controls.Add(btPrevClr);
            panel1.Controls.Add(cbPlayShift);
            panel1.Controls.Add(btPlayClr);
            panel1.Controls.Add(cbStopCtrl);
            panel1.Controls.Add(btPauseClr);
            panel1.Controls.Add(cbSlowShift);
            panel1.Controls.Add(btFastClr);
            panel1.Controls.Add(cbPauseCtrl);
            panel1.Controls.Add(btFadeoutClr);
            panel1.Controls.Add(cbPrevShift);
            panel1.Controls.Add(btSlowClr);
            panel1.Controls.Add(cbFadeoutCtrl);
            panel1.Controls.Add(btDmvSet);
            panel1.Controls.Add(btStopClr);
            panel1.Controls.Add(btUmvSet);
            panel1.Controls.Add(cbFadeoutShift);
            panel1.Controls.Add(btNextSet);
            panel1.Controls.Add(cbPrevCtrl);
            panel1.Controls.Add(btPrevSet);
            panel1.Controls.Add(cbPauseShift);
            panel1.Controls.Add(btPlaySet);
            panel1.Controls.Add(cbSlowCtrl);
            panel1.Controls.Add(btPauseSet);
            panel1.Controls.Add(cbStopShift);
            panel1.Controls.Add(btFastSet);
            panel1.Controls.Add(label71);
            panel1.Controls.Add(cbPlayCtrl);
            panel1.Controls.Add(label70);
            panel1.Controls.Add(btFadeoutSet);
            panel1.Controls.Add(label44);
            panel1.Controls.Add(btSlowSet);
            panel1.Controls.Add(cbFastCtrl);
            panel1.Controls.Add(btStopSet);
            panel1.Controls.Add(cbDmvCtrl);
            panel1.Controls.Add(label43);
            panel1.Controls.Add(cbUmvCtrl);
            panel1.Controls.Add(lblDmvKey);
            panel1.Controls.Add(label50);
            panel1.Controls.Add(lblUmvKey);
            panel1.Controls.Add(cbNextCtrl);
            panel1.Controls.Add(lblNextKey);
            panel1.Controls.Add(label42);
            panel1.Controls.Add(lblFastKey);
            panel1.Controls.Add(label41);
            panel1.Controls.Add(lblPlayKey);
            panel1.Controls.Add(label40);
            panel1.Controls.Add(lblSlowKey);
            panel1.Controls.Add(label39);
            panel1.Controls.Add(lblPrevKey);
            panel1.Controls.Add(label38);
            panel1.Controls.Add(lblFadeoutKey);
            panel1.Controls.Add(label48);
            panel1.Controls.Add(lblPauseKey);
            panel1.Controls.Add(label46);
            panel1.Controls.Add(lblStopKey);
            panel1.Controls.Add(label45);
            panel1.Controls.Add(cbStopAlt);
            panel1.Controls.Add(cbDmvAlt);
            panel1.Controls.Add(pictureBox17);
            panel1.Controls.Add(cbUmvAlt);
            panel1.Controls.Add(label37);
            panel1.Controls.Add(cbNextAlt);
            panel1.Controls.Add(cbPauseAlt);
            panel1.Controls.Add(pictureBox16);
            panel1.Controls.Add(pictureBox10);
            panel1.Controls.Add(cbFastAlt);
            panel1.Controls.Add(cbFadeoutAlt);
            panel1.Controls.Add(pictureBox15);
            panel1.Controls.Add(pictureBox11);
            panel1.Controls.Add(cbPlayAlt);
            panel1.Controls.Add(cbPrevAlt);
            panel1.Controls.Add(pictureBox13);
            panel1.Controls.Add(pictureBox12);
            panel1.Controls.Add(cbSlowAlt);
            panel1.Name = "panel1";
            // 
            // btPpcClr
            // 
            resources.ApplyResources(btPpcClr, "btPpcClr");
            btPpcClr.Name = "btPpcClr";
            btPpcClr.UseVisualStyleBackColor = true;
            btPpcClr.Click += BtPpcClr_Click;
            // 
            // cbPpcShift
            // 
            resources.ApplyResources(cbPpcShift, "cbPpcShift");
            cbPpcShift.Name = "cbPpcShift";
            cbPpcShift.UseVisualStyleBackColor = true;
            // 
            // btPpcSet
            // 
            resources.ApplyResources(btPpcSet, "btPpcSet");
            btPpcSet.Name = "btPpcSet";
            btPpcSet.UseVisualStyleBackColor = true;
            btPpcSet.Click += BtPpcSet_Click;
            // 
            // label76
            // 
            resources.ApplyResources(label76, "label76");
            label76.Name = "label76";
            // 
            // lblPpcKey
            // 
            resources.ApplyResources(lblPpcKey, "lblPpcKey");
            lblPpcKey.Name = "lblPpcKey";
            // 
            // cbPpcAlt
            // 
            resources.ApplyResources(cbPpcAlt, "cbPpcAlt");
            cbPpcAlt.Name = "cbPpcAlt";
            cbPpcAlt.UseVisualStyleBackColor = true;
            // 
            // cbPpcCtrl
            // 
            resources.ApplyResources(cbPpcCtrl, "cbPpcCtrl");
            cbPpcCtrl.Name = "cbPpcCtrl";
            cbPpcCtrl.UseVisualStyleBackColor = true;
            // 
            // btDpcClr
            // 
            resources.ApplyResources(btDpcClr, "btDpcClr");
            btDpcClr.Name = "btDpcClr";
            btDpcClr.UseVisualStyleBackColor = true;
            btDpcClr.Click += BtDpcClr_Click;
            // 
            // cbDpcShift
            // 
            resources.ApplyResources(cbDpcShift, "cbDpcShift");
            cbDpcShift.Name = "cbDpcShift";
            cbDpcShift.UseVisualStyleBackColor = true;
            // 
            // btDpcSet
            // 
            resources.ApplyResources(btDpcSet, "btDpcSet");
            btDpcSet.Name = "btDpcSet";
            btDpcSet.UseVisualStyleBackColor = true;
            btDpcSet.Click += BtDpcSet_Click;
            // 
            // label74
            // 
            resources.ApplyResources(label74, "label74");
            label74.Name = "label74";
            // 
            // lblDpcKey
            // 
            resources.ApplyResources(lblDpcKey, "lblDpcKey");
            lblDpcKey.Name = "lblDpcKey";
            // 
            // cbDpcAlt
            // 
            resources.ApplyResources(cbDpcAlt, "cbDpcAlt");
            cbDpcAlt.Name = "cbDpcAlt";
            cbDpcAlt.UseVisualStyleBackColor = true;
            // 
            // cbDpcCtrl
            // 
            resources.ApplyResources(cbDpcCtrl, "cbDpcCtrl");
            cbDpcCtrl.Name = "cbDpcCtrl";
            cbDpcCtrl.UseVisualStyleBackColor = true;
            // 
            // btUpcClr
            // 
            resources.ApplyResources(btUpcClr, "btUpcClr");
            btUpcClr.Name = "btUpcClr";
            btUpcClr.UseVisualStyleBackColor = true;
            btUpcClr.Click += BtUpcClr_Click;
            // 
            // btRmvClr
            // 
            resources.ApplyResources(btRmvClr, "btRmvClr");
            btRmvClr.Name = "btRmvClr";
            btRmvClr.UseVisualStyleBackColor = true;
            btRmvClr.Click += BtRmvClr_Click;
            // 
            // cbUpcShift
            // 
            resources.ApplyResources(cbUpcShift, "cbUpcShift");
            cbUpcShift.Name = "cbUpcShift";
            cbUpcShift.UseVisualStyleBackColor = true;
            // 
            // pictureBox14
            // 
            resources.ApplyResources(pictureBox14, "pictureBox14");
            pictureBox14.Name = "pictureBox14";
            pictureBox14.TabStop = false;
            // 
            // cbRmvShift
            // 
            resources.ApplyResources(cbRmvShift, "cbRmvShift");
            cbRmvShift.Name = "cbRmvShift";
            cbRmvShift.UseVisualStyleBackColor = true;
            // 
            // btUpcSet
            // 
            resources.ApplyResources(btUpcSet, "btUpcSet");
            btUpcSet.Name = "btUpcSet";
            btUpcSet.UseVisualStyleBackColor = true;
            btUpcSet.Click += BtUpcSet_Click;
            // 
            // cbFastShift
            // 
            resources.ApplyResources(cbFastShift, "cbFastShift");
            cbFastShift.Name = "cbFastShift";
            cbFastShift.UseVisualStyleBackColor = true;
            // 
            // label72
            // 
            resources.ApplyResources(label72, "label72");
            label72.Name = "label72";
            // 
            // btDmvClr
            // 
            resources.ApplyResources(btDmvClr, "btDmvClr");
            btDmvClr.Name = "btDmvClr";
            btDmvClr.UseVisualStyleBackColor = true;
            btDmvClr.Click += BtDmvClr_Click;
            // 
            // lblUpcKey
            // 
            resources.ApplyResources(lblUpcKey, "lblUpcKey");
            lblUpcKey.Name = "lblUpcKey";
            // 
            // btRmvSet
            // 
            resources.ApplyResources(btRmvSet, "btRmvSet");
            btRmvSet.Name = "btRmvSet";
            btRmvSet.UseVisualStyleBackColor = true;
            btRmvSet.Click += BtRmvSet_Click;
            // 
            // cbUpcAlt
            // 
            resources.ApplyResources(cbUpcAlt, "cbUpcAlt");
            cbUpcAlt.Name = "cbUpcAlt";
            cbUpcAlt.UseVisualStyleBackColor = true;
            // 
            // cbDmvShift
            // 
            resources.ApplyResources(cbDmvShift, "cbDmvShift");
            cbDmvShift.Name = "cbDmvShift";
            cbDmvShift.UseVisualStyleBackColor = true;
            // 
            // cbUpcCtrl
            // 
            resources.ApplyResources(cbUpcCtrl, "cbUpcCtrl");
            cbUpcCtrl.Name = "cbUpcCtrl";
            cbUpcCtrl.UseVisualStyleBackColor = true;
            // 
            // label69
            // 
            resources.ApplyResources(label69, "label69");
            label69.Name = "label69";
            // 
            // btUmvClr
            // 
            resources.ApplyResources(btUmvClr, "btUmvClr");
            btUmvClr.Name = "btUmvClr";
            btUmvClr.UseVisualStyleBackColor = true;
            btUmvClr.Click += BtUmvClr_Click;
            // 
            // cbRmvAlt
            // 
            resources.ApplyResources(cbRmvAlt, "cbRmvAlt");
            cbRmvAlt.Name = "cbRmvAlt";
            cbRmvAlt.UseVisualStyleBackColor = true;
            // 
            // cbUmvShift
            // 
            resources.ApplyResources(cbUmvShift, "cbUmvShift");
            cbUmvShift.Name = "cbUmvShift";
            cbUmvShift.UseVisualStyleBackColor = true;
            // 
            // cbRmvCtrl
            // 
            resources.ApplyResources(cbRmvCtrl, "cbRmvCtrl");
            cbRmvCtrl.Name = "cbRmvCtrl";
            cbRmvCtrl.UseVisualStyleBackColor = true;
            // 
            // btNextClr
            // 
            resources.ApplyResources(btNextClr, "btNextClr");
            btNextClr.Name = "btNextClr";
            btNextClr.UseVisualStyleBackColor = true;
            btNextClr.Click += BtNextClr_Click;
            // 
            // lblRmvKey
            // 
            resources.ApplyResources(lblRmvKey, "lblRmvKey");
            lblRmvKey.Name = "lblRmvKey";
            // 
            // cbNextShift
            // 
            resources.ApplyResources(cbNextShift, "cbNextShift");
            cbNextShift.Name = "cbNextShift";
            cbNextShift.UseVisualStyleBackColor = true;
            // 
            // btPrevClr
            // 
            resources.ApplyResources(btPrevClr, "btPrevClr");
            btPrevClr.Name = "btPrevClr";
            btPrevClr.UseVisualStyleBackColor = true;
            btPrevClr.Click += BtPrevClr_Click;
            // 
            // cbPlayShift
            // 
            resources.ApplyResources(cbPlayShift, "cbPlayShift");
            cbPlayShift.Name = "cbPlayShift";
            cbPlayShift.UseVisualStyleBackColor = true;
            // 
            // btPlayClr
            // 
            resources.ApplyResources(btPlayClr, "btPlayClr");
            btPlayClr.Name = "btPlayClr";
            btPlayClr.UseVisualStyleBackColor = true;
            btPlayClr.Click += BtPlayClr_Click;
            // 
            // cbStopCtrl
            // 
            resources.ApplyResources(cbStopCtrl, "cbStopCtrl");
            cbStopCtrl.Name = "cbStopCtrl";
            cbStopCtrl.UseVisualStyleBackColor = true;
            // 
            // btPauseClr
            // 
            resources.ApplyResources(btPauseClr, "btPauseClr");
            btPauseClr.Name = "btPauseClr";
            btPauseClr.UseVisualStyleBackColor = true;
            btPauseClr.Click += BtPauseClr_Click;
            // 
            // cbSlowShift
            // 
            resources.ApplyResources(cbSlowShift, "cbSlowShift");
            cbSlowShift.Name = "cbSlowShift";
            cbSlowShift.UseVisualStyleBackColor = true;
            // 
            // btFastClr
            // 
            resources.ApplyResources(btFastClr, "btFastClr");
            btFastClr.Name = "btFastClr";
            btFastClr.UseVisualStyleBackColor = true;
            btFastClr.Click += BtFastClr_Click;
            // 
            // cbPauseCtrl
            // 
            resources.ApplyResources(cbPauseCtrl, "cbPauseCtrl");
            cbPauseCtrl.Name = "cbPauseCtrl";
            cbPauseCtrl.UseVisualStyleBackColor = true;
            // 
            // btFadeoutClr
            // 
            resources.ApplyResources(btFadeoutClr, "btFadeoutClr");
            btFadeoutClr.Name = "btFadeoutClr";
            btFadeoutClr.UseVisualStyleBackColor = true;
            btFadeoutClr.Click += BtFadeoutClr_Click;
            // 
            // cbPrevShift
            // 
            resources.ApplyResources(cbPrevShift, "cbPrevShift");
            cbPrevShift.Name = "cbPrevShift";
            cbPrevShift.UseVisualStyleBackColor = true;
            // 
            // btSlowClr
            // 
            resources.ApplyResources(btSlowClr, "btSlowClr");
            btSlowClr.Name = "btSlowClr";
            btSlowClr.UseVisualStyleBackColor = true;
            btSlowClr.Click += BtSlowClr_Click;
            // 
            // cbFadeoutCtrl
            // 
            resources.ApplyResources(cbFadeoutCtrl, "cbFadeoutCtrl");
            cbFadeoutCtrl.Name = "cbFadeoutCtrl";
            cbFadeoutCtrl.UseVisualStyleBackColor = true;
            // 
            // btDmvSet
            // 
            resources.ApplyResources(btDmvSet, "btDmvSet");
            btDmvSet.Name = "btDmvSet";
            btDmvSet.UseVisualStyleBackColor = true;
            btDmvSet.Click += BtDmvSet_Click;
            // 
            // btStopClr
            // 
            resources.ApplyResources(btStopClr, "btStopClr");
            btStopClr.Name = "btStopClr";
            btStopClr.UseVisualStyleBackColor = true;
            btStopClr.Click += BtStopClr_Click;
            // 
            // btUmvSet
            // 
            resources.ApplyResources(btUmvSet, "btUmvSet");
            btUmvSet.Name = "btUmvSet";
            btUmvSet.UseVisualStyleBackColor = true;
            btUmvSet.Click += BtUmvSet_Click;
            // 
            // cbFadeoutShift
            // 
            resources.ApplyResources(cbFadeoutShift, "cbFadeoutShift");
            cbFadeoutShift.Name = "cbFadeoutShift";
            cbFadeoutShift.UseVisualStyleBackColor = true;
            // 
            // btNextSet
            // 
            resources.ApplyResources(btNextSet, "btNextSet");
            btNextSet.Name = "btNextSet";
            btNextSet.UseVisualStyleBackColor = true;
            btNextSet.Click += BtNextSet_Click;
            // 
            // cbPrevCtrl
            // 
            resources.ApplyResources(cbPrevCtrl, "cbPrevCtrl");
            cbPrevCtrl.Name = "cbPrevCtrl";
            cbPrevCtrl.UseVisualStyleBackColor = true;
            // 
            // btPrevSet
            // 
            resources.ApplyResources(btPrevSet, "btPrevSet");
            btPrevSet.Name = "btPrevSet";
            btPrevSet.UseVisualStyleBackColor = true;
            btPrevSet.Click += BtPrevSet_Click;
            // 
            // cbPauseShift
            // 
            resources.ApplyResources(cbPauseShift, "cbPauseShift");
            cbPauseShift.Name = "cbPauseShift";
            cbPauseShift.UseVisualStyleBackColor = true;
            // 
            // btPlaySet
            // 
            resources.ApplyResources(btPlaySet, "btPlaySet");
            btPlaySet.Name = "btPlaySet";
            btPlaySet.UseVisualStyleBackColor = true;
            btPlaySet.Click += BtPlaySet_Click;
            // 
            // cbSlowCtrl
            // 
            resources.ApplyResources(cbSlowCtrl, "cbSlowCtrl");
            cbSlowCtrl.Name = "cbSlowCtrl";
            cbSlowCtrl.UseVisualStyleBackColor = true;
            // 
            // btPauseSet
            // 
            resources.ApplyResources(btPauseSet, "btPauseSet");
            btPauseSet.Name = "btPauseSet";
            btPauseSet.UseVisualStyleBackColor = true;
            btPauseSet.Click += BtPauseSet_Click;
            // 
            // cbStopShift
            // 
            resources.ApplyResources(cbStopShift, "cbStopShift");
            cbStopShift.Name = "cbStopShift";
            cbStopShift.UseVisualStyleBackColor = true;
            // 
            // btFastSet
            // 
            resources.ApplyResources(btFastSet, "btFastSet");
            btFastSet.Name = "btFastSet";
            btFastSet.UseVisualStyleBackColor = true;
            btFastSet.Click += BtFastSet_Click;
            // 
            // label71
            // 
            resources.ApplyResources(label71, "label71");
            label71.Name = "label71";
            // 
            // cbPlayCtrl
            // 
            resources.ApplyResources(cbPlayCtrl, "cbPlayCtrl");
            cbPlayCtrl.Name = "cbPlayCtrl";
            cbPlayCtrl.UseVisualStyleBackColor = true;
            // 
            // label70
            // 
            resources.ApplyResources(label70, "label70");
            label70.Name = "label70";
            // 
            // btFadeoutSet
            // 
            resources.ApplyResources(btFadeoutSet, "btFadeoutSet");
            btFadeoutSet.Name = "btFadeoutSet";
            btFadeoutSet.UseVisualStyleBackColor = true;
            btFadeoutSet.Click += BtFadeoutSet_Click;
            // 
            // label44
            // 
            resources.ApplyResources(label44, "label44");
            label44.Name = "label44";
            // 
            // btSlowSet
            // 
            resources.ApplyResources(btSlowSet, "btSlowSet");
            btSlowSet.Name = "btSlowSet";
            btSlowSet.UseVisualStyleBackColor = true;
            btSlowSet.Click += BtSlowSet_Click;
            // 
            // cbFastCtrl
            // 
            resources.ApplyResources(cbFastCtrl, "cbFastCtrl");
            cbFastCtrl.Name = "cbFastCtrl";
            cbFastCtrl.UseVisualStyleBackColor = true;
            // 
            // btStopSet
            // 
            resources.ApplyResources(btStopSet, "btStopSet");
            btStopSet.Name = "btStopSet";
            btStopSet.UseVisualStyleBackColor = true;
            btStopSet.Click += BtStopSet_Click;
            // 
            // cbDmvCtrl
            // 
            resources.ApplyResources(cbDmvCtrl, "cbDmvCtrl");
            cbDmvCtrl.Name = "cbDmvCtrl";
            cbDmvCtrl.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            resources.ApplyResources(label43, "label43");
            label43.Name = "label43";
            // 
            // cbUmvCtrl
            // 
            resources.ApplyResources(cbUmvCtrl, "cbUmvCtrl");
            cbUmvCtrl.Name = "cbUmvCtrl";
            cbUmvCtrl.UseVisualStyleBackColor = true;
            // 
            // lblDmvKey
            // 
            resources.ApplyResources(lblDmvKey, "lblDmvKey");
            lblDmvKey.Name = "lblDmvKey";
            // 
            // label50
            // 
            resources.ApplyResources(label50, "label50");
            label50.Name = "label50";
            // 
            // lblUmvKey
            // 
            resources.ApplyResources(lblUmvKey, "lblUmvKey");
            lblUmvKey.Name = "lblUmvKey";
            // 
            // cbNextCtrl
            // 
            resources.ApplyResources(cbNextCtrl, "cbNextCtrl");
            cbNextCtrl.Name = "cbNextCtrl";
            cbNextCtrl.UseVisualStyleBackColor = true;
            // 
            // lblNextKey
            // 
            resources.ApplyResources(lblNextKey, "lblNextKey");
            lblNextKey.Name = "lblNextKey";
            // 
            // label42
            // 
            resources.ApplyResources(label42, "label42");
            label42.Name = "label42";
            // 
            // lblFastKey
            // 
            resources.ApplyResources(lblFastKey, "lblFastKey");
            lblFastKey.Name = "lblFastKey";
            // 
            // label41
            // 
            resources.ApplyResources(label41, "label41");
            label41.Name = "label41";
            // 
            // lblPlayKey
            // 
            resources.ApplyResources(lblPlayKey, "lblPlayKey");
            lblPlayKey.Name = "lblPlayKey";
            // 
            // label40
            // 
            resources.ApplyResources(label40, "label40");
            label40.Name = "label40";
            // 
            // lblSlowKey
            // 
            resources.ApplyResources(lblSlowKey, "lblSlowKey");
            lblSlowKey.Name = "lblSlowKey";
            // 
            // label39
            // 
            resources.ApplyResources(label39, "label39");
            label39.Name = "label39";
            // 
            // lblPrevKey
            // 
            resources.ApplyResources(lblPrevKey, "lblPrevKey");
            lblPrevKey.Name = "lblPrevKey";
            // 
            // label38
            // 
            resources.ApplyResources(label38, "label38");
            label38.Name = "label38";
            // 
            // lblFadeoutKey
            // 
            resources.ApplyResources(lblFadeoutKey, "lblFadeoutKey");
            lblFadeoutKey.Name = "lblFadeoutKey";
            // 
            // label48
            // 
            resources.ApplyResources(label48, "label48");
            label48.Name = "label48";
            // 
            // lblPauseKey
            // 
            resources.ApplyResources(lblPauseKey, "lblPauseKey");
            lblPauseKey.Name = "lblPauseKey";
            // 
            // label46
            // 
            resources.ApplyResources(label46, "label46");
            label46.Name = "label46";
            // 
            // lblStopKey
            // 
            resources.ApplyResources(lblStopKey, "lblStopKey");
            lblStopKey.Name = "lblStopKey";
            // 
            // label45
            // 
            resources.ApplyResources(label45, "label45");
            label45.Name = "label45";
            // 
            // cbStopAlt
            // 
            resources.ApplyResources(cbStopAlt, "cbStopAlt");
            cbStopAlt.Name = "cbStopAlt";
            cbStopAlt.UseVisualStyleBackColor = true;
            // 
            // cbDmvAlt
            // 
            resources.ApplyResources(cbDmvAlt, "cbDmvAlt");
            cbDmvAlt.Name = "cbDmvAlt";
            cbDmvAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox17
            // 
            resources.ApplyResources(pictureBox17, "pictureBox17");
            pictureBox17.Name = "pictureBox17";
            pictureBox17.TabStop = false;
            // 
            // cbUmvAlt
            // 
            resources.ApplyResources(cbUmvAlt, "cbUmvAlt");
            cbUmvAlt.Name = "cbUmvAlt";
            cbUmvAlt.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            resources.ApplyResources(label37, "label37");
            label37.Name = "label37";
            // 
            // cbNextAlt
            // 
            resources.ApplyResources(cbNextAlt, "cbNextAlt");
            cbNextAlt.Name = "cbNextAlt";
            cbNextAlt.UseVisualStyleBackColor = true;
            // 
            // cbPauseAlt
            // 
            resources.ApplyResources(cbPauseAlt, "cbPauseAlt");
            cbPauseAlt.Name = "cbPauseAlt";
            cbPauseAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox16
            // 
            resources.ApplyResources(pictureBox16, "pictureBox16");
            pictureBox16.Name = "pictureBox16";
            pictureBox16.TabStop = false;
            // 
            // pictureBox10
            // 
            resources.ApplyResources(pictureBox10, "pictureBox10");
            pictureBox10.Name = "pictureBox10";
            pictureBox10.TabStop = false;
            // 
            // cbFastAlt
            // 
            resources.ApplyResources(cbFastAlt, "cbFastAlt");
            cbFastAlt.Name = "cbFastAlt";
            cbFastAlt.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutAlt
            // 
            resources.ApplyResources(cbFadeoutAlt, "cbFadeoutAlt");
            cbFadeoutAlt.Name = "cbFadeoutAlt";
            cbFadeoutAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox15
            // 
            resources.ApplyResources(pictureBox15, "pictureBox15");
            pictureBox15.Name = "pictureBox15";
            pictureBox15.TabStop = false;
            // 
            // pictureBox11
            // 
            resources.ApplyResources(pictureBox11, "pictureBox11");
            pictureBox11.Name = "pictureBox11";
            pictureBox11.TabStop = false;
            // 
            // cbPlayAlt
            // 
            resources.ApplyResources(cbPlayAlt, "cbPlayAlt");
            cbPlayAlt.Name = "cbPlayAlt";
            cbPlayAlt.UseVisualStyleBackColor = true;
            // 
            // cbPrevAlt
            // 
            resources.ApplyResources(cbPrevAlt, "cbPrevAlt");
            cbPrevAlt.Name = "cbPrevAlt";
            cbPrevAlt.UseVisualStyleBackColor = true;
            // 
            // pictureBox13
            // 
            resources.ApplyResources(pictureBox13, "pictureBox13");
            pictureBox13.Name = "pictureBox13";
            pictureBox13.TabStop = false;
            // 
            // pictureBox12
            // 
            resources.ApplyResources(pictureBox12, "pictureBox12");
            pictureBox12.Name = "pictureBox12";
            pictureBox12.TabStop = false;
            // 
            // cbSlowAlt
            // 
            resources.ApplyResources(cbSlowAlt, "cbSlowAlt");
            cbSlowAlt.Name = "cbSlowAlt";
            cbSlowAlt.UseVisualStyleBackColor = true;
            // 
            // lblKeyBoardHookNotice
            // 
            resources.ApplyResources(lblKeyBoardHookNotice, "lblKeyBoardHookNotice");
            lblKeyBoardHookNotice.ForeColor = Color.Red;
            lblKeyBoardHookNotice.Name = "lblKeyBoardHookNotice";
            // 
            // label47
            // 
            resources.ApplyResources(label47, "label47");
            label47.Name = "label47";
            // 
            // cbStopWin
            // 
            resources.ApplyResources(cbStopWin, "cbStopWin");
            cbStopWin.Name = "cbStopWin";
            cbStopWin.UseVisualStyleBackColor = true;
            // 
            // cbPauseWin
            // 
            resources.ApplyResources(cbPauseWin, "cbPauseWin");
            cbPauseWin.Name = "cbPauseWin";
            cbPauseWin.UseVisualStyleBackColor = true;
            // 
            // cbFadeoutWin
            // 
            resources.ApplyResources(cbFadeoutWin, "cbFadeoutWin");
            cbFadeoutWin.Name = "cbFadeoutWin";
            cbFadeoutWin.UseVisualStyleBackColor = true;
            // 
            // cbPrevWin
            // 
            resources.ApplyResources(cbPrevWin, "cbPrevWin");
            cbPrevWin.Name = "cbPrevWin";
            cbPrevWin.UseVisualStyleBackColor = true;
            // 
            // cbSlowWin
            // 
            resources.ApplyResources(cbSlowWin, "cbSlowWin");
            cbSlowWin.Name = "cbSlowWin";
            cbSlowWin.UseVisualStyleBackColor = true;
            // 
            // cbPlayWin
            // 
            resources.ApplyResources(cbPlayWin, "cbPlayWin");
            cbPlayWin.Name = "cbPlayWin";
            cbPlayWin.UseVisualStyleBackColor = true;
            // 
            // cbFastWin
            // 
            resources.ApplyResources(cbFastWin, "cbFastWin");
            cbFastWin.Name = "cbFastWin";
            cbFastWin.UseVisualStyleBackColor = true;
            // 
            // cbNextWin
            // 
            resources.ApplyResources(cbNextWin, "cbNextWin");
            cbNextWin.Name = "cbNextWin";
            cbNextWin.UseVisualStyleBackColor = true;
            // 
            // tpBalance
            // 
            tpBalance.Controls.Add(groupBox25);
            tpBalance.Controls.Add(cbAutoBalanceUseThis);
            tpBalance.Controls.Add(groupBox18);
            resources.ApplyResources(tpBalance, "tpBalance");
            tpBalance.Name = "tpBalance";
            tpBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            resources.ApplyResources(groupBox25, "groupBox25");
            groupBox25.Controls.Add(rbAutoBalanceNotSamePositionAsSongData);
            groupBox25.Controls.Add(rbAutoBalanceSamePositionAsSongData);
            groupBox25.Name = "groupBox25";
            groupBox25.TabStop = false;
            // 
            // rbAutoBalanceNotSamePositionAsSongData
            // 
            resources.ApplyResources(rbAutoBalanceNotSamePositionAsSongData, "rbAutoBalanceNotSamePositionAsSongData");
            rbAutoBalanceNotSamePositionAsSongData.Checked = true;
            rbAutoBalanceNotSamePositionAsSongData.Name = "rbAutoBalanceNotSamePositionAsSongData";
            rbAutoBalanceNotSamePositionAsSongData.TabStop = true;
            rbAutoBalanceNotSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSamePositionAsSongData
            // 
            resources.ApplyResources(rbAutoBalanceSamePositionAsSongData, "rbAutoBalanceSamePositionAsSongData");
            rbAutoBalanceSamePositionAsSongData.Name = "rbAutoBalanceSamePositionAsSongData";
            rbAutoBalanceSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // cbAutoBalanceUseThis
            // 
            resources.ApplyResources(cbAutoBalanceUseThis, "cbAutoBalanceUseThis");
            cbAutoBalanceUseThis.Checked = true;
            cbAutoBalanceUseThis.CheckState = CheckState.Checked;
            cbAutoBalanceUseThis.Name = "cbAutoBalanceUseThis";
            cbAutoBalanceUseThis.UseVisualStyleBackColor = true;
            // 
            // groupBox18
            // 
            resources.ApplyResources(groupBox18, "groupBox18");
            groupBox18.Controls.Add(groupBox24);
            groupBox18.Controls.Add(groupBox23);
            groupBox18.Name = "groupBox18";
            groupBox18.TabStop = false;
            // 
            // groupBox24
            // 
            groupBox24.Controls.Add(groupBox21);
            groupBox24.Controls.Add(groupBox22);
            resources.ApplyResources(groupBox24, "groupBox24");
            groupBox24.Name = "groupBox24";
            groupBox24.TabStop = false;
            // 
            // groupBox21
            // 
            groupBox21.Controls.Add(rbAutoBalanceNotSaveSongBalance);
            groupBox21.Controls.Add(rbAutoBalanceSaveSongBalance);
            resources.ApplyResources(groupBox21, "groupBox21");
            groupBox21.Name = "groupBox21";
            groupBox21.TabStop = false;
            // 
            // rbAutoBalanceNotSaveSongBalance
            // 
            resources.ApplyResources(rbAutoBalanceNotSaveSongBalance, "rbAutoBalanceNotSaveSongBalance");
            rbAutoBalanceNotSaveSongBalance.Checked = true;
            rbAutoBalanceNotSaveSongBalance.Name = "rbAutoBalanceNotSaveSongBalance";
            rbAutoBalanceNotSaveSongBalance.TabStop = true;
            rbAutoBalanceNotSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSaveSongBalance
            // 
            resources.ApplyResources(rbAutoBalanceSaveSongBalance, "rbAutoBalanceSaveSongBalance");
            rbAutoBalanceSaveSongBalance.Name = "rbAutoBalanceSaveSongBalance";
            rbAutoBalanceSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            groupBox22.Controls.Add(label4);
            resources.ApplyResources(groupBox22, "groupBox22");
            groupBox22.Name = "groupBox22";
            groupBox22.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // groupBox23
            // 
            resources.ApplyResources(groupBox23, "groupBox23");
            groupBox23.Controls.Add(groupBox19);
            groupBox23.Controls.Add(groupBox20);
            groupBox23.Name = "groupBox23";
            groupBox23.TabStop = false;
            // 
            // groupBox19
            // 
            groupBox19.Controls.Add(rbAutoBalanceNotLoadSongBalance);
            groupBox19.Controls.Add(rbAutoBalanceLoadSongBalance);
            resources.ApplyResources(groupBox19, "groupBox19");
            groupBox19.Name = "groupBox19";
            groupBox19.TabStop = false;
            // 
            // rbAutoBalanceNotLoadSongBalance
            // 
            resources.ApplyResources(rbAutoBalanceNotLoadSongBalance, "rbAutoBalanceNotLoadSongBalance");
            rbAutoBalanceNotLoadSongBalance.Checked = true;
            rbAutoBalanceNotLoadSongBalance.Name = "rbAutoBalanceNotLoadSongBalance";
            rbAutoBalanceNotLoadSongBalance.TabStop = true;
            rbAutoBalanceNotLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadSongBalance
            // 
            resources.ApplyResources(rbAutoBalanceLoadSongBalance, "rbAutoBalanceLoadSongBalance");
            rbAutoBalanceLoadSongBalance.Name = "rbAutoBalanceLoadSongBalance";
            rbAutoBalanceLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            resources.ApplyResources(groupBox20, "groupBox20");
            groupBox20.Controls.Add(rbAutoBalanceNotLoadDriverBalance);
            groupBox20.Controls.Add(rbAutoBalanceLoadDriverBalance);
            groupBox20.Name = "groupBox20";
            groupBox20.TabStop = false;
            groupBox20.Enter += groupBox20_Enter;
            // 
            // rbAutoBalanceNotLoadDriverBalance
            // 
            resources.ApplyResources(rbAutoBalanceNotLoadDriverBalance, "rbAutoBalanceNotLoadDriverBalance");
            rbAutoBalanceNotLoadDriverBalance.Name = "rbAutoBalanceNotLoadDriverBalance";
            rbAutoBalanceNotLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadDriverBalance
            // 
            resources.ApplyResources(rbAutoBalanceLoadDriverBalance, "rbAutoBalanceLoadDriverBalance");
            rbAutoBalanceLoadDriverBalance.Checked = true;
            rbAutoBalanceLoadDriverBalance.Name = "rbAutoBalanceLoadDriverBalance";
            rbAutoBalanceLoadDriverBalance.TabStop = true;
            rbAutoBalanceLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // tpPlayList
            // 
            tpPlayList.Controls.Add(groupBox17);
            tpPlayList.Controls.Add(cbEmptyPlayList);
            resources.ApplyResources(tpPlayList, "tpPlayList");
            tpPlayList.Name = "tpPlayList";
            tpPlayList.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            resources.ApplyResources(groupBox17, "groupBox17");
            groupBox17.Controls.Add(cbAutoOpenImg);
            groupBox17.Controls.Add(tbImageExt);
            groupBox17.Controls.Add(cbAutoOpenMML);
            groupBox17.Controls.Add(tbMMLExt);
            groupBox17.Controls.Add(tbTextExt);
            groupBox17.Controls.Add(cbAutoOpenText);
            groupBox17.Controls.Add(label1);
            groupBox17.Controls.Add(label3);
            groupBox17.Controls.Add(label2);
            groupBox17.Name = "groupBox17";
            groupBox17.TabStop = false;
            // 
            // cbAutoOpenImg
            // 
            resources.ApplyResources(cbAutoOpenImg, "cbAutoOpenImg");
            cbAutoOpenImg.Name = "cbAutoOpenImg";
            cbAutoOpenImg.UseVisualStyleBackColor = true;
            cbAutoOpenImg.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // tbImageExt
            // 
            resources.ApplyResources(tbImageExt, "tbImageExt");
            tbImageExt.Name = "tbImageExt";
            // 
            // cbAutoOpenMML
            // 
            resources.ApplyResources(cbAutoOpenMML, "cbAutoOpenMML");
            cbAutoOpenMML.Name = "cbAutoOpenMML";
            cbAutoOpenMML.UseVisualStyleBackColor = true;
            cbAutoOpenMML.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // tbMMLExt
            // 
            resources.ApplyResources(tbMMLExt, "tbMMLExt");
            tbMMLExt.Name = "tbMMLExt";
            // 
            // tbTextExt
            // 
            resources.ApplyResources(tbTextExt, "tbTextExt");
            tbTextExt.Name = "tbTextExt";
            // 
            // cbAutoOpenText
            // 
            resources.ApplyResources(cbAutoOpenText, "cbAutoOpenText");
            cbAutoOpenText.Name = "cbAutoOpenText";
            cbAutoOpenText.UseVisualStyleBackColor = true;
            cbAutoOpenText.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // cbEmptyPlayList
            // 
            resources.ApplyResources(cbEmptyPlayList, "cbEmptyPlayList");
            cbEmptyPlayList.Name = "cbEmptyPlayList";
            cbEmptyPlayList.UseVisualStyleBackColor = true;
            cbEmptyPlayList.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // tpOther
            // 
            tpOther.Controls.Add(btnImageResourceFile);
            tpOther.Controls.Add(tbResourceFile);
            tpOther.Controls.Add(label73);
            tpOther.Controls.Add(btnSearchPath);
            tpOther.Controls.Add(tbSearchPath);
            tpOther.Controls.Add(label68);
            tpOther.Controls.Add(cbNonRenderingForPause);
            tpOther.Controls.Add(cbWavSwitch);
            tpOther.Controls.Add(cbUseGetInst);
            tpOther.Controls.Add(groupBox4);
            tpOther.Controls.Add(cbDumpSwitch);
            tpOther.Controls.Add(gbWav);
            tpOther.Controls.Add(gbDump);
            tpOther.Controls.Add(label30);
            tpOther.Controls.Add(tbScreenFrameRate);
            tpOther.Controls.Add(label29);
            tpOther.Controls.Add(lblLoopTimes);
            tpOther.Controls.Add(btnDataPath);
            tpOther.Controls.Add(tbLoopTimes);
            tpOther.Controls.Add(tbDataPath);
            tpOther.Controls.Add(label19);
            tpOther.Controls.Add(btnResetPosition);
            tpOther.Controls.Add(btnOpenSettingFolder);
            tpOther.Controls.Add(cbExALL);
            tpOther.Controls.Add(cbSaveCompiledFile);
            tpOther.Controls.Add(cbInitAlways);
            tpOther.Controls.Add(cbAutoOpen);
            tpOther.Controls.Add(cbUseLoopTimes);
            resources.ApplyResources(tpOther, "tpOther");
            tpOther.Name = "tpOther";
            tpOther.UseVisualStyleBackColor = true;
            // 
            // btnImageResourceFile
            // 
            resources.ApplyResources(btnImageResourceFile, "btnImageResourceFile");
            btnImageResourceFile.Name = "btnImageResourceFile";
            btnImageResourceFile.UseVisualStyleBackColor = true;
            btnImageResourceFile.Click += BtnImageResourcePath_Click;
            // 
            // tbResourceFile
            // 
            resources.ApplyResources(tbResourceFile, "tbResourceFile");
            tbResourceFile.Name = "tbResourceFile";
            // 
            // label73
            // 
            resources.ApplyResources(label73, "label73");
            label73.Name = "label73";
            // 
            // btnSearchPath
            // 
            resources.ApplyResources(btnSearchPath, "btnSearchPath");
            btnSearchPath.Name = "btnSearchPath";
            btnSearchPath.UseVisualStyleBackColor = true;
            btnSearchPath.Click += BtnSearchPath_Click;
            // 
            // tbSearchPath
            // 
            resources.ApplyResources(tbSearchPath, "tbSearchPath");
            tbSearchPath.Name = "tbSearchPath";
            // 
            // label68
            // 
            resources.ApplyResources(label68, "label68");
            label68.Name = "label68";
            // 
            // cbNonRenderingForPause
            // 
            resources.ApplyResources(cbNonRenderingForPause, "cbNonRenderingForPause");
            cbNonRenderingForPause.Name = "cbNonRenderingForPause";
            cbNonRenderingForPause.UseVisualStyleBackColor = true;
            // 
            // cbWavSwitch
            // 
            resources.ApplyResources(cbWavSwitch, "cbWavSwitch");
            cbWavSwitch.Name = "cbWavSwitch";
            cbWavSwitch.UseVisualStyleBackColor = true;
            cbWavSwitch.CheckedChanged += CbWavSwitch_CheckedChanged;
            // 
            // cbUseGetInst
            // 
            resources.ApplyResources(cbUseGetInst, "cbUseGetInst");
            cbUseGetInst.Name = "cbUseGetInst";
            cbUseGetInst.UseVisualStyleBackColor = true;
            cbUseGetInst.CheckedChanged += CbUseGetInst_CheckedChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(cbAdjustTLParam);
            groupBox4.Controls.Add(cmbInstFormat);
            groupBox4.Controls.Add(lblInstFormat);
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // cbAdjustTLParam
            // 
            resources.ApplyResources(cbAdjustTLParam, "cbAdjustTLParam");
            cbAdjustTLParam.Name = "cbAdjustTLParam";
            cbAdjustTLParam.UseVisualStyleBackColor = true;
            // 
            // cmbInstFormat
            // 
            cmbInstFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbInstFormat.FormattingEnabled = true;
            cmbInstFormat.Items.AddRange(new object[] { resources.GetString("cmbInstFormat.Items"), resources.GetString("cmbInstFormat.Items1"), resources.GetString("cmbInstFormat.Items2"), resources.GetString("cmbInstFormat.Items3"), resources.GetString("cmbInstFormat.Items4"), resources.GetString("cmbInstFormat.Items5"), resources.GetString("cmbInstFormat.Items6"), resources.GetString("cmbInstFormat.Items7"), resources.GetString("cmbInstFormat.Items8"), resources.GetString("cmbInstFormat.Items9"), resources.GetString("cmbInstFormat.Items10"), resources.GetString("cmbInstFormat.Items11"), resources.GetString("cmbInstFormat.Items12"), resources.GetString("cmbInstFormat.Items13"), resources.GetString("cmbInstFormat.Items14"), resources.GetString("cmbInstFormat.Items15"), resources.GetString("cmbInstFormat.Items16") });
            resources.ApplyResources(cmbInstFormat, "cmbInstFormat");
            cmbInstFormat.Name = "cmbInstFormat";
            // 
            // lblInstFormat
            // 
            resources.ApplyResources(lblInstFormat, "lblInstFormat");
            lblInstFormat.Name = "lblInstFormat";
            // 
            // cbDumpSwitch
            // 
            resources.ApplyResources(cbDumpSwitch, "cbDumpSwitch");
            cbDumpSwitch.Name = "cbDumpSwitch";
            cbDumpSwitch.UseVisualStyleBackColor = true;
            cbDumpSwitch.CheckedChanged += CbDumpSwitch_CheckedChanged;
            // 
            // gbWav
            // 
            gbWav.Controls.Add(btnWavPath);
            gbWav.Controls.Add(label7);
            gbWav.Controls.Add(tbWavPath);
            resources.ApplyResources(gbWav, "gbWav");
            gbWav.Name = "gbWav";
            gbWav.TabStop = false;
            // 
            // btnWavPath
            // 
            resources.ApplyResources(btnWavPath, "btnWavPath");
            btnWavPath.Name = "btnWavPath";
            btnWavPath.UseVisualStyleBackColor = true;
            btnWavPath.Click += BtnWavPath_Click;
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // tbWavPath
            // 
            resources.ApplyResources(tbWavPath, "tbWavPath");
            tbWavPath.Name = "tbWavPath";
            // 
            // gbDump
            // 
            gbDump.Controls.Add(btnDumpPath);
            gbDump.Controls.Add(label6);
            gbDump.Controls.Add(tbDumpPath);
            resources.ApplyResources(gbDump, "gbDump");
            gbDump.Name = "gbDump";
            gbDump.TabStop = false;
            // 
            // btnDumpPath
            // 
            resources.ApplyResources(btnDumpPath, "btnDumpPath");
            btnDumpPath.Name = "btnDumpPath";
            btnDumpPath.UseVisualStyleBackColor = true;
            btnDumpPath.Click += BtnDumpPath_Click;
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // tbDumpPath
            // 
            resources.ApplyResources(tbDumpPath, "tbDumpPath");
            tbDumpPath.Name = "tbDumpPath";
            // 
            // label30
            // 
            resources.ApplyResources(label30, "label30");
            label30.Name = "label30";
            // 
            // tbScreenFrameRate
            // 
            resources.ApplyResources(tbScreenFrameRate, "tbScreenFrameRate");
            tbScreenFrameRate.Name = "tbScreenFrameRate";
            // 
            // label29
            // 
            resources.ApplyResources(label29, "label29");
            label29.Name = "label29";
            // 
            // lblLoopTimes
            // 
            resources.ApplyResources(lblLoopTimes, "lblLoopTimes");
            lblLoopTimes.Name = "lblLoopTimes";
            // 
            // btnDataPath
            // 
            resources.ApplyResources(btnDataPath, "btnDataPath");
            btnDataPath.Name = "btnDataPath";
            btnDataPath.UseVisualStyleBackColor = true;
            btnDataPath.Click += BtnDataPath_Click;
            // 
            // tbLoopTimes
            // 
            resources.ApplyResources(tbLoopTimes, "tbLoopTimes");
            tbLoopTimes.Name = "tbLoopTimes";
            // 
            // tbDataPath
            // 
            resources.ApplyResources(tbDataPath, "tbDataPath");
            tbDataPath.Name = "tbDataPath";
            // 
            // label19
            // 
            resources.ApplyResources(label19, "label19");
            label19.Name = "label19";
            // 
            // btnResetPosition
            // 
            resources.ApplyResources(btnResetPosition, "btnResetPosition");
            btnResetPosition.Name = "btnResetPosition";
            btnResetPosition.UseVisualStyleBackColor = true;
            btnResetPosition.Click += BtnResetPosition_Click;
            // 
            // btnOpenSettingFolder
            // 
            resources.ApplyResources(btnOpenSettingFolder, "btnOpenSettingFolder");
            btnOpenSettingFolder.Name = "btnOpenSettingFolder";
            btnOpenSettingFolder.UseVisualStyleBackColor = true;
            btnOpenSettingFolder.Click += BtnOpenSettingFolder_Click;
            // 
            // cbExALL
            // 
            resources.ApplyResources(cbExALL, "cbExALL");
            cbExALL.Name = "cbExALL";
            cbExALL.UseVisualStyleBackColor = true;
            cbExALL.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // cbSaveCompiledFile
            // 
            resources.ApplyResources(cbSaveCompiledFile, "cbSaveCompiledFile");
            cbSaveCompiledFile.Name = "cbSaveCompiledFile";
            cbSaveCompiledFile.UseVisualStyleBackColor = true;
            cbSaveCompiledFile.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // cbInitAlways
            // 
            resources.ApplyResources(cbInitAlways, "cbInitAlways");
            cbInitAlways.Name = "cbInitAlways";
            cbInitAlways.UseVisualStyleBackColor = true;
            cbInitAlways.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // cbAutoOpen
            // 
            resources.ApplyResources(cbAutoOpen, "cbAutoOpen");
            cbAutoOpen.Name = "cbAutoOpen";
            cbAutoOpen.UseVisualStyleBackColor = true;
            cbAutoOpen.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // cbUseLoopTimes
            // 
            resources.ApplyResources(cbUseLoopTimes, "cbUseLoopTimes");
            cbUseLoopTimes.Name = "cbUseLoopTimes";
            cbUseLoopTimes.UseVisualStyleBackColor = true;
            cbUseLoopTimes.CheckedChanged += CbUseLoopTimes_CheckedChanged;
            // 
            // tpOmake
            // 
            tpOmake.Controls.Add(label67);
            tpOmake.Controls.Add(label14);
            tpOmake.Controls.Add(btVST);
            tpOmake.Controls.Add(tbSCCbaseAddress);
            tpOmake.Controls.Add(tbVST);
            tpOmake.Controls.Add(groupBox5);
            resources.ApplyResources(tpOmake, "tpOmake");
            tpOmake.Name = "tpOmake";
            tpOmake.UseVisualStyleBackColor = true;
            // 
            // label67
            // 
            resources.ApplyResources(label67, "label67");
            label67.Name = "label67";
            // 
            // label14
            // 
            resources.ApplyResources(label14, "label14");
            label14.Name = "label14";
            // 
            // btVST
            // 
            resources.ApplyResources(btVST, "btVST");
            btVST.Name = "btVST";
            btVST.UseVisualStyleBackColor = true;
            btVST.Click += BtVST_Click;
            // 
            // tbSCCbaseAddress
            // 
            resources.ApplyResources(tbSCCbaseAddress, "tbSCCbaseAddress");
            tbSCCbaseAddress.Name = "tbSCCbaseAddress";
            // 
            // tbVST
            // 
            resources.ApplyResources(tbVST, "tbVST");
            tbVST.Name = "tbVST";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(groupBox30);
            groupBox5.Controls.Add(cbShowConsole);
            groupBox5.Controls.Add(cbDispFrameCounter);
            resources.ApplyResources(groupBox5, "groupBox5");
            groupBox5.Name = "groupBox5";
            groupBox5.TabStop = false;
            // 
            // groupBox30
            // 
            groupBox30.Controls.Add(rbLoglvlInformation);
            groupBox30.Controls.Add(rbLoglvlWarning);
            groupBox30.Controls.Add(rbLoglvlError);
            groupBox30.Controls.Add(rbLogDebug);
            groupBox30.Controls.Add(rbLoglvlTrace);
            resources.ApplyResources(groupBox30, "groupBox30");
            groupBox30.Name = "groupBox30";
            groupBox30.TabStop = false;
            // 
            // rbLoglvlInformation
            // 
            resources.ApplyResources(rbLoglvlInformation, "rbLoglvlInformation");
            rbLoglvlInformation.Name = "rbLoglvlInformation";
            rbLoglvlInformation.TabStop = true;
            rbLoglvlInformation.UseVisualStyleBackColor = true;
            // 
            // rbLoglvlWarning
            // 
            resources.ApplyResources(rbLoglvlWarning, "rbLoglvlWarning");
            rbLoglvlWarning.Name = "rbLoglvlWarning";
            rbLoglvlWarning.TabStop = true;
            rbLoglvlWarning.UseVisualStyleBackColor = true;
            // 
            // rbLoglvlError
            // 
            resources.ApplyResources(rbLoglvlError, "rbLoglvlError");
            rbLoglvlError.Name = "rbLoglvlError";
            rbLoglvlError.TabStop = true;
            rbLoglvlError.UseVisualStyleBackColor = true;
            // 
            // rbLogDebug
            // 
            resources.ApplyResources(rbLogDebug, "rbLogDebug");
            rbLogDebug.Name = "rbLogDebug";
            rbLogDebug.TabStop = true;
            rbLogDebug.UseVisualStyleBackColor = true;
            // 
            // rbLoglvlTrace
            // 
            resources.ApplyResources(rbLoglvlTrace, "rbLoglvlTrace");
            rbLoglvlTrace.Name = "rbLoglvlTrace";
            rbLoglvlTrace.TabStop = true;
            rbLoglvlTrace.UseVisualStyleBackColor = true;
            // 
            // cbShowConsole
            // 
            resources.ApplyResources(cbShowConsole, "cbShowConsole");
            cbShowConsole.Name = "cbShowConsole";
            cbShowConsole.UseVisualStyleBackColor = true;
            // 
            // cbDispFrameCounter
            // 
            resources.ApplyResources(cbDispFrameCounter, "cbDispFrameCounter");
            cbDispFrameCounter.Name = "cbDispFrameCounter";
            cbDispFrameCounter.UseVisualStyleBackColor = true;
            // 
            // tpAbout
            // 
            tpAbout.Controls.Add(tableLayoutPanel);
            resources.ApplyResources(tpAbout, "tpAbout");
            tpAbout.Name = "tpAbout";
            tpAbout.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
            tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
            tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
            tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
            tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
            tableLayoutPanel.Controls.Add(labelCompanyName, 1, 3);
            tableLayoutPanel.Controls.Add(textBoxDescription, 1, 4);
            tableLayoutPanel.Controls.Add(llOpenGithub, 1, 5);
            tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // logoPictureBox
            // 
            resources.ApplyResources(logoPictureBox, "logoPictureBox");
            logoPictureBox.Image = Resources.FeliAndMD21;
            logoPictureBox.Name = "logoPictureBox";
            tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
            logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            resources.ApplyResources(labelProductName, "labelProductName");
            labelProductName.Name = "labelProductName";
            // 
            // labelVersion
            // 
            resources.ApplyResources(labelVersion, "labelVersion");
            labelVersion.Name = "labelVersion";
            // 
            // labelCopyright
            // 
            resources.ApplyResources(labelCopyright, "labelCopyright");
            labelCopyright.Name = "labelCopyright";
            // 
            // labelCompanyName
            // 
            resources.ApplyResources(labelCompanyName, "labelCompanyName");
            labelCompanyName.Name = "labelCompanyName";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(textBoxDescription, "textBoxDescription");
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.TabStop = false;
            // 
            // llOpenGithub
            // 
            resources.ApplyResources(llOpenGithub, "llOpenGithub");
            llOpenGithub.Name = "llOpenGithub";
            llOpenGithub.TabStop = true;
            llOpenGithub.LinkClicked += LlOpenGithub_LinkClicked;
            // 
            // frmSetting
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            Controls.Add(tcSetting);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmSetting";
            FormClosed += FrmSetting_FormClosed;
            Load += FrmSetting_Load;
            gbWaveOut.ResumeLayout(false);
            gbAsioOut.ResumeLayout(false);
            gbWasapiOut.ResumeLayout(false);
            gbWasapiOut.PerformLayout();
            gbDirectSound.ResumeLayout(false);
            tcSetting.ResumeLayout(false);
            tpOutput.ResumeLayout(false);
            tpOutput.PerformLayout();
            groupBox16.ResumeLayout(false);
            tpModule.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            tpNuked.ResumeLayout(false);
            groupBox29.ResumeLayout(false);
            groupBox29.PerformLayout();
            groupBox26.ResumeLayout(false);
            groupBox26.PerformLayout();
            tpNSF.ResumeLayout(false);
            tpNSF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkbNSFLPF).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkbNSFHPF).EndInit();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox12.ResumeLayout(false);
            groupBox12.PerformLayout();
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            tpSID.ResumeLayout(false);
            tpSID.PerformLayout();
            groupBox28.ResumeLayout(false);
            groupBox28.PerformLayout();
            groupBox27.ResumeLayout(false);
            groupBox27.PerformLayout();
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            groupBox13.ResumeLayout(false);
            groupBox13.PerformLayout();
            tpPMDDotNET.ResumeLayout(false);
            tpPMDDotNET.PerformLayout();
            gbPMDManual.ResumeLayout(false);
            gbPMDManual.PerformLayout();
            groupBox32.ResumeLayout(false);
            groupBox32.PerformLayout();
            gbPPSDRV.ResumeLayout(false);
            groupBox33.ResumeLayout(false);
            groupBox33.PerformLayout();
            gbPMDSetManualVolume.ResumeLayout(false);
            gbPMDSetManualVolume.PerformLayout();
            tpMIDIOut.ResumeLayout(false);
            tpMIDIOut.PerformLayout();
            tbcMIDIoutList.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListA).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListB).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListC).EndInit();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListD).EndInit();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListE).EndInit();
            tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListF).EndInit();
            tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListG).EndInit();
            tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListH).EndInit();
            tabPage9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListI).EndInit();
            tabPage10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListJ).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutPallet).EndInit();
            tpMIDIOut2.ResumeLayout(false);
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            tabMIDIExp.ResumeLayout(false);
            tabMIDIExp.PerformLayout();
            gbMIDIExport.ResumeLayout(false);
            gbMIDIExport.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            tpMIDIKBD.ResumeLayout(false);
            tpMIDIKBD.PerformLayout();
            gbMIDIKeyboard.ResumeLayout(false);
            gbMIDIKeyboard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            gbUseChannel.ResumeLayout(false);
            gbUseChannel.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tpKeyBoard.ResumeLayout(false);
            tpKeyBoard.PerformLayout();
            gbUseKeyBoardHook.ResumeLayout(false);
            gbUseKeyBoardHook.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox14).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox17).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox15).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox13).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).EndInit();
            tpBalance.ResumeLayout(false);
            tpBalance.PerformLayout();
            groupBox25.ResumeLayout(false);
            groupBox25.PerformLayout();
            groupBox18.ResumeLayout(false);
            groupBox24.ResumeLayout(false);
            groupBox21.ResumeLayout(false);
            groupBox21.PerformLayout();
            groupBox22.ResumeLayout(false);
            groupBox22.PerformLayout();
            groupBox23.ResumeLayout(false);
            groupBox19.ResumeLayout(false);
            groupBox19.PerformLayout();
            groupBox20.ResumeLayout(false);
            groupBox20.PerformLayout();
            tpPlayList.ResumeLayout(false);
            tpPlayList.PerformLayout();
            groupBox17.ResumeLayout(false);
            groupBox17.PerformLayout();
            tpOther.ResumeLayout(false);
            tpOther.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            gbWav.ResumeLayout(false);
            gbWav.PerformLayout();
            gbDump.ResumeLayout(false);
            gbDump.PerformLayout();
            tpOmake.ResumeLayout(false);
            tpOmake.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox30.ResumeLayout(false);
            groupBox30.PerformLayout();
            tpAbout.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnOK;
        private Button btnCancel;
        private GroupBox gbWaveOut;
        private RadioButton rbWaveOut;
        private RadioButton rbAsioOut;
        private RadioButton rbWasapiOut;
        private GroupBox gbAsioOut;
        private RadioButton rbDirectSoundOut;
        private GroupBox gbWasapiOut;
        private GroupBox gbDirectSound;
        private ComboBox cmbWaveOutDevice;
        private Button btnASIOControlPanel;
        private ComboBox cmbAsioDevice;
        private ComboBox cmbWasapiDevice;
        private ComboBox cmbDirectSoundDevice;
        private TabControl tcSetting;
        private TabPage tpOutput;
        private TabPage tpAbout;
        private TableLayoutPanel tableLayoutPanel;
        private PictureBox logoPictureBox;
        private Label labelProductName;
        private Label labelVersion;
        private Label labelCopyright;
        private Label labelCompanyName;
        private TextBox textBoxDescription;
        private TabPage tpOther;
        private GroupBox gbMIDIKeyboard;
        private GroupBox gbUseChannel;
        private CheckBox cbFM1;
        private CheckBox cbFM2;
        private CheckBox cbFM3;
        private CheckBox cbUseMIDIKeyboard;
        private CheckBox cbFM4;
        private CheckBox cbFM5;
        private CheckBox cbFM6;
        private ComboBox cmbMIDIIN;
        private Label label5;
        private RadioButton rbExclusive;
        private RadioButton rbShare;
        private Label lblLatencyUnit;
        private Label lblLatency;
        private ComboBox cmbLatency;
        private TabPage tpModule;
        private GroupBox groupBox3;
        private Label label13;
        private Label label12;
        private Label label11;
        private TextBox tbLatencyEmu;
        private TextBox tbLatencySCCI;
        private Label label10;
        private GroupBox groupBox5;
        private CheckBox cbDispFrameCounter;
        private CheckBox cbHiyorimiMode;
        private CheckBox cbUseLoopTimes;
        private Label lblLoopTimes;
        private TextBox tbLoopTimes;
        private Button btnOpenSettingFolder;
        private CheckBox cbUseGetInst;
        private Button btnDataPath;
        private TextBox tbDataPath;
        private Label label19;
        private TabPage tpMIDIKBD;
        private ComboBox cmbInstFormat;
        private Label lblInstFormat;
        private Label label30;
        private TextBox tbScreenFrameRate;
        private Label label29;
        private CheckBox cbAutoOpen;
        private ucSettingInstruments ucSI;
        private GroupBox groupBox1;
        private GroupBox groupBox4;
        private CheckBox cbDumpSwitch;
        private GroupBox gbDump;
        private Button btnDumpPath;
        private Label label6;
        private TextBox tbDumpPath;
        private Button btnResetPosition;
        private TabPage tabMIDIExp;
        private CheckBox cbUseMIDIExport;
        private GroupBox gbMIDIExport;
        private CheckBox cbMIDIUseVOPM;
        private GroupBox groupBox6;
        private CheckBox cbMIDIYM2612;
        private CheckBox cbMIDISN76489Sec;
        private CheckBox cbMIDIYM2612Sec;
        private CheckBox cbMIDISN76489;
        private CheckBox cbMIDIYM2151;
        private CheckBox cbMIDIYM2610BSec;
        private CheckBox cbMIDIYM2151Sec;
        private CheckBox cbMIDIYM2610B;
        private CheckBox cbMIDIYM2203;
        private CheckBox cbMIDIYM2608Sec;
        private CheckBox cbMIDIYM2203Sec;
        private CheckBox cbMIDIYM2608;
        private CheckBox cbMIDIPlayless;
        private Button btnMIDIOutputPath;
        private Label lblOutputPath;
        private TextBox tbMIDIOutputPath;
        private CheckBox cbWavSwitch;
        private GroupBox gbWav;
        private Button btnWavPath;
        private Label label7;
        private TextBox tbWavPath;
        private RadioButton rbMONO;
        private RadioButton rbPOLY;
        private GroupBox groupBox7;
        private RadioButton rbFM6;
        private RadioButton rbFM3;
        private RadioButton rbFM5;
        private RadioButton rbFM2;
        private RadioButton rbFM4;
        private RadioButton rbFM1;
        private GroupBox groupBox2;
        private TabPage tpOmake;
        private TextBox tbCCFadeout;
        private TextBox tbCCPause;
        private TextBox tbCCSlow;
        private TextBox tbCCPrevious;
        private TextBox tbCCNext;
        private TextBox tbCCFast;
        private TextBox tbCCStop;
        private TextBox tbCCPlay;
        private TextBox tbCCCopyLog;
        private Label label17;
        private TextBox tbCCDelLog;
        private Label label15;
        private TextBox tbCCChCopy;
        private Label label9;
        private Label label8;
        private PictureBox pictureBox1;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox8;
        private PictureBox pictureBox7;
        private PictureBox pictureBox6;
        private PictureBox pictureBox5;
        private Label label14;
        private Button btVST;
        private TextBox tbVST;
        private TabPage tpMIDIOut;
        private Button btnUP_A;
        private Button btnSubMIDIout;
        private Button btnDOWN_A;
        private Button btnAddMIDIout;
        private Label label18;
        private DataGridView dgvMIDIoutListA;
        private DataGridView dgvMIDIoutPallet;
        private Label label16;
        private DataGridViewTextBoxColumn clmID;
        private DataGridViewTextBoxColumn clmDeviceName;
        private DataGridViewTextBoxColumn clmManufacturer;
        private DataGridViewTextBoxColumn clmSpacer;
        private TabControl tbcMIDIoutList;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private Button btnUP_B;
        private Button btnDOWN_B;
        private Button btnUP_C;
        private Button btnDOWN_C;
        private Button btnUP_D;
        private Button btnDOWN_D;
        private TabPage tabPage5;
        private Button btnUP_E;
        private Button btnDOWN_E;
        private TabPage tabPage6;
        private Button btnUP_F;
        private Button btnDOWN_F;
        private TabPage tabPage7;
        private Button btnUP_G;
        private Button btnDOWN_G;
        private TabPage tabPage8;
        private Button btnUP_H;
        private Button btnDOWN_H;
        private TabPage tabPage9;
        private Button btnUP_I;
        private Button btnDOWN_I;
        private TabPage tabPage10;
        private Button button17;
        private Button btnDOWN_J;
        private Button btnAddVST;
        private DataGridView dgvMIDIoutListB;
        private DataGridView dgvMIDIoutListC;
        private DataGridView dgvMIDIoutListD;
        private DataGridView dgvMIDIoutListE;
        private DataGridView dgvMIDIoutListF;
        private DataGridView dgvMIDIoutListG;
        private DataGridView dgvMIDIoutListH;
        private DataGridView dgvMIDIoutListI;
        private DataGridView dgvMIDIoutListJ;
        private TabPage tpNSF;
        private GroupBox groupBox8;
        private CheckBox cbNSFFDSWriteDisable8000;
        private GroupBox groupBox10;
        private CheckBox cbNSFDmc_RandomizeTri;
        private CheckBox cbNSFDmc_TriMute;
        private CheckBox cbNSFDmc_RandomizeNoise;
        private CheckBox cbNSFDmc_DPCMAntiClick;
        private CheckBox cbNSFDmc_EnablePNoise;
        private CheckBox cbNSFDmc_Enable4011;
        private CheckBox cbNSFDmc_NonLinearMixer;
        private CheckBox cbNSFDmc_UnmuteOnReset;
        private GroupBox groupBox12;
        private CheckBox cbNSFN160_Serial;
        private GroupBox groupBox11;
        private CheckBox cbNSFMmc5_PhaseRefresh;
        private CheckBox cbNSFMmc5_NonLinearMixer;
        private GroupBox groupBox9;
        private CheckBox cbNFSNes_DutySwap;
        private CheckBox cbNFSNes_PhaseRefresh;
        private CheckBox cbNFSNes_NonLinearMixer;
        private CheckBox cbNFSNes_UnmuteOnReset;
        private Label label21;
        private Label label20;
        private TextBox tbNSFFds_LPF;
        private CheckBox cbNFSFds_4085Reset;
        private TabPage tpSID;
        private GroupBox groupBox13;
        private Label label22;
        private Button btnSIDCharacter;
        private Button btnSIDBasic;
        private Button btnSIDKernal;
        private TextBox tbSIDCharacter;
        private TextBox tbSIDBasic;
        private TextBox tbSIDKernal;
        private Label label24;
        private Label label23;
        private GroupBox groupBox14;
        private Label label27;
        private Label label26;
        private Label label25;
        private RadioButton rdSIDQ1;
        private RadioButton rdSIDQ3;
        private RadioButton rdSIDQ2;
        private RadioButton rdSIDQ4;
        private Label lblWaitTime;
        private Label label28;
        private ComboBox cmbWaitTime;
        private TabPage tpMIDIOut2;
        private GroupBox groupBox15;
        private Button btnBeforeSend_Default;
        private TextBox tbBeforeSend_Custom;
        private TextBox tbBeforeSend_XGReset;
        private Label label34;
        private Label label32;
        private TextBox tbBeforeSend_GSReset;
        private Label label33;
        private TextBox tbBeforeSend_GMReset;
        private Label label31;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewCheckBoxColumn clmIsVST;
        private DataGridViewTextBoxColumn clmFileName;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewComboBoxColumn clmType;
        private DataGridViewComboBoxColumn ClmBeforeSend;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Label label35;
        private Label label36;
        private RadioButton rbSPPCM;
        private GroupBox groupBox16;
        private ComboBox cmbSPPCMDevice;
        private GroupBox groupBox17;
        private TextBox tbImageExt;
        private TextBox tbMMLExt;
        private TextBox tbTextExt;
        private Label label1;
        private Label label3;
        private Label label2;
        private CheckBox cbInitAlways;
        private TabPage tpBalance;
        private CheckBox cbAutoBalanceUseThis;
        private GroupBox groupBox18;
        private GroupBox groupBox24;
        private GroupBox groupBox21;
        private RadioButton rbAutoBalanceNotSaveSongBalance;
        private RadioButton rbAutoBalanceSamePositionAsSongData;
        private RadioButton rbAutoBalanceSaveSongBalance;
        private GroupBox groupBox22;
        private Label label4;
        private GroupBox groupBox23;
        private GroupBox groupBox19;
        private RadioButton rbAutoBalanceNotLoadSongBalance;
        private RadioButton rbAutoBalanceLoadSongBalance;
        private GroupBox groupBox20;
        private RadioButton rbAutoBalanceNotLoadDriverBalance;
        private RadioButton rbAutoBalanceLoadDriverBalance;
        private GroupBox groupBox25;
        private RadioButton rbAutoBalanceNotSamePositionAsSongData;
        private TabPage tpKeyBoard;
        private PictureBox pictureBox10;
        private PictureBox pictureBox11;
        private PictureBox pictureBox12;
        private PictureBox pictureBox13;
        private PictureBox pictureBox14;
        private PictureBox pictureBox15;
        private PictureBox pictureBox16;
        private PictureBox pictureBox17;
        private CheckBox cbUseKeyBoardHook;
        private GroupBox gbUseKeyBoardHook;
        private Button btPrevClr;
        private Button btPauseClr;
        private Button btFadeoutClr;
        private Button btStopClr;
        private Button btNextSet;
        private Button btPrevSet;
        private Button btPlaySet;
        private Button btPauseSet;
        private Button btFastSet;
        private Button btFadeoutSet;
        private Button btSlowSet;
        private Button btStopSet;
        private Label label50;
        private Label lblNextKey;
        private Label lblFastKey;
        private Label lblPlayKey;
        private Label lblSlowKey;
        private Label lblPrevKey;
        private Label lblFadeoutKey;
        private Label lblPauseKey;
        private Label lblStopKey;
        private CheckBox cbNextAlt;
        private CheckBox cbFastAlt;
        private CheckBox cbPlayAlt;
        private CheckBox cbSlowAlt;
        private CheckBox cbPrevAlt;
        private CheckBox cbFadeoutAlt;
        private CheckBox cbPauseAlt;
        private Label label37;
        private CheckBox cbStopAlt;
        private Label label45;
        private CheckBox cbNextWin;
        private Label label46;
        private CheckBox cbFastWin;
        private Label label47;
        private CheckBox cbPlayWin;
        private Label label48;
        private CheckBox cbSlowWin;
        private Label label38;
        private CheckBox cbPrevWin;
        private Label label39;
        private CheckBox cbFadeoutWin;
        private Label label40;
        private CheckBox cbPauseWin;
        private Label label41;
        private CheckBox cbStopWin;
        private Label label42;
        private CheckBox cbNextCtrl;
        private Label label43;
        private CheckBox cbFastCtrl;
        private Label label44;
        private CheckBox cbPlayCtrl;
        private CheckBox cbStopShift;
        private CheckBox cbSlowCtrl;
        private CheckBox cbPauseShift;
        private CheckBox cbPrevCtrl;
        private CheckBox cbFadeoutShift;
        private CheckBox cbFadeoutCtrl;
        private CheckBox cbPrevShift;
        private CheckBox cbPauseCtrl;
        private CheckBox cbSlowShift;
        private CheckBox cbStopCtrl;
        private CheckBox cbPlayShift;
        private CheckBox cbNextShift;
        private CheckBox cbFastShift;
        private Button btNextClr;
        private Button btPlayClr;
        private Button btFastClr;
        private Button btSlowClr;
        //private ucSettingInstruments ucSettingInstruments1;
        private Label lblKeyBoardHookNotice;
        private RadioButton rbNullDevice;
        private TextBox tbSIDOutputBufferSize;
        private Label label49;
        private Label label51;
        private TabPage tpNuked;
        private GroupBox groupBox26;
        private RadioButton rbNukedOPN2OptionYM2612u;
        private RadioButton rbNukedOPN2OptionYM2612;
        private RadioButton rbNukedOPN2OptionDiscrete;
        private RadioButton rbNukedOPN2OptionASIC;
        private RadioButton rbNukedOPN2OptionASIClp;
        private CheckBox cbEmptyPlayList;
        private CheckBox cbMIDIKeyOnFnum;
        private CheckBox cbExALL;
        private CheckBox cbNonRenderingForPause;
        private LinkLabel llOpenGithub;
        private TrackBar trkbNSFLPF;
        private Label label53;
        private Label label52;
        private TrackBar trkbNSFHPF;
        private TabPage tpPMDDotNET;
        private RadioButton rbPMDManual;
        private RadioButton rbPMDAuto;
        private Button btnPMDResetDriverArguments;
        private Label label54;
        private Button btnPMDResetCompilerArhguments;
        private TextBox tbPMDDriverArguments;
        private Label label55;
        private TextBox tbPMDCompilerArguments;
        private GroupBox gbPMDManual;
        private CheckBox cbPMDSetManualVolume;
        private CheckBox cbPMDUsePPZ8;
        private GroupBox groupBox32;
        private RadioButton rbPMD86B;
        private RadioButton rbPMDSpbB;
        private RadioButton rbPMDNrmB;
        private CheckBox cbPMDUsePPSDRV;
        private GroupBox gbPPSDRV;
        private GroupBox groupBox33;
        private RadioButton rbPMDUsePPSDRVManualFreq;
        private Label label56;
        private RadioButton rbPMDUsePPSDRVFreqDefault;
        private Button btnPMDPPSDRVManualWait;
        private Label label57;
        private TextBox tbPMDPPSDRVFreq;
        private Label label58;
        private TextBox tbPMDPPSDRVManualWait;
        private GroupBox gbPMDSetManualVolume;
        private Label label59;
        private Label label60;
        private TextBox tbPMDVolumeAdpcm;
        private Label label61;
        private TextBox tbPMDVolumeRhythm;
        private Label label62;
        private TextBox tbPMDVolumeSSG;
        private Label label63;
        private TextBox tbPMDVolumeGIMICSSG;
        private Label label64;
        private TextBox tbPMDVolumeFM;
        private GroupBox groupBox28;
        private GroupBox groupBox27;
        private RadioButton rbSIDC64Model_PAL;
        private RadioButton rbSIDC64Model_DREAN;
        private RadioButton rbSIDC64Model_OLDNTSC;
        private RadioButton rbSIDC64Model_NTSC;
        private RadioButton rbSIDModel_8580;
        private RadioButton rbSIDModel_6581;
        private CheckBox cbSIDC64Model_Force;
        private CheckBox cbSIDModel_Force;
        private GroupBox groupBox29;
        private CheckBox cbGensSSGEG;
        private CheckBox cbGensDACHPF;
        private TabPage tpPlayList;
        private CheckBox cbAutoOpenImg;
        private CheckBox cbAutoOpenMML;
        private CheckBox cbAutoOpenText;
        private Label label66;
        private Label label65;
        private ComboBox cmbSampleRate;
        private Label label67;
        private TextBox tbSCCbaseAddress;
        private Button btnSearchPath;
        private TextBox tbSearchPath;
        private Label label68;
        private CheckBox cbNSFDmc_DPCMReverse;
        private CheckBox cbUnuseRealChip;
        private CheckBox cbAdjustTLParam;
        private Panel panel1;
        private Button btDmvClr;
        private CheckBox cbDmvShift;
        private Button btUmvClr;
        private CheckBox cbUmvShift;
        private Button btDmvSet;
        private Button btUmvSet;
        private Label label71;
        private Label label70;
        private CheckBox cbDmvCtrl;
        private CheckBox cbUmvCtrl;
        private Label lblDmvKey;
        private Label lblUmvKey;
        private CheckBox cbDmvAlt;
        private CheckBox cbUmvAlt;
        private Button btRmvClr;
        private CheckBox cbRmvShift;
        private Button btRmvSet;
        private Label label69;
        private CheckBox cbRmvAlt;
        private CheckBox cbRmvCtrl;
        private Label lblRmvKey;
        private Button btPpcClr;
        private CheckBox cbPpcShift;
        private Button btPpcSet;
        private Label label76;
        private Label lblPpcKey;
        private CheckBox cbPpcAlt;
        private CheckBox cbPpcCtrl;
        private Button btDpcClr;
        private CheckBox cbDpcShift;
        private Button btDpcSet;
        private Label label74;
        private Label lblDpcKey;
        private CheckBox cbDpcAlt;
        private CheckBox cbDpcCtrl;
        private Button btUpcClr;
        private CheckBox cbUpcShift;
        private Button btUpcSet;
        private Label label72;
        private Label lblUpcKey;
        private CheckBox cbUpcAlt;
        private CheckBox cbUpcCtrl;
        private CheckBox cbShowConsole;
        private GroupBox groupBox30;
        private RadioButton rbLoglvlInformation;
        private RadioButton rbLoglvlWarning;
        private RadioButton rbLoglvlError;
        private RadioButton rbLogDebug;
        private RadioButton rbLoglvlTrace;
        private Button btnImageResourceFile;
        private TextBox tbResourceFile;
        private Label label73;
        private CheckBox cbSaveCompiledFile;
    }
}