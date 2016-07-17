namespace MDPlayer
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
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbWaveOut = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWaveOutDevice = new System.Windows.Forms.ComboBox();
            this.rbWaveOut = new System.Windows.Forms.RadioButton();
            this.rbAsioOut = new System.Windows.Forms.RadioButton();
            this.rbWasapiOut = new System.Windows.Forms.RadioButton();
            this.gbAsioOut = new System.Windows.Forms.GroupBox();
            this.btnASIOControlPanel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbAsioDevice = new System.Windows.Forms.ComboBox();
            this.rbDirectSoundOut = new System.Windows.Forms.RadioButton();
            this.gbWasapiOut = new System.Windows.Forms.GroupBox();
            this.rbExclusive = new System.Windows.Forms.RadioButton();
            this.rbShare = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbWasapiDevice = new System.Windows.Forms.ComboBox();
            this.gbDirectSound = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDirectSoundDevice = new System.Windows.Forms.ComboBox();
            this.tcSetting = new System.Windows.Forms.TabControl();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.lblLatencyUnit = new System.Windows.Forms.Label();
            this.lblLatency = new System.Windows.Forms.Label();
            this.cmbLatency = new System.Windows.Forms.ComboBox();
            this.tpModule = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbHiyorimiMode = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbLatencyEmu = new System.Windows.Forms.TextBox();
            this.tbLatencySCCI = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbSN76489Scci = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rbSN76489Scci = new System.Windows.Forms.RadioButton();
            this.tbSN76489ScciDelay = new System.Windows.Forms.TextBox();
            this.cbSN76489UseWaitBoost = new System.Windows.Forms.CheckBox();
            this.rbSN76489Emu = new System.Windows.Forms.RadioButton();
            this.cbSN76489UseWait = new System.Windows.Forms.CheckBox();
            this.tbSN76489EmuDelay = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbYM2612ScciDelay = new System.Windows.Forms.TextBox();
            this.tbYM2612EmuDelay = new System.Windows.Forms.TextBox();
            this.cbOnlyPCMEmulation = new System.Windows.Forms.CheckBox();
            this.cbYM2612UseWaitBoost = new System.Windows.Forms.CheckBox();
            this.cbYM2612UseWait = new System.Windows.Forms.CheckBox();
            this.cmbYM2612Scci = new System.Windows.Forms.ComboBox();
            this.rbYM2612Scci = new System.Windows.Forms.RadioButton();
            this.rbYM2612Emu = new System.Windows.Forms.RadioButton();
            this.tpBalance = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbDispFrameCounter = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnPWM = new System.Windows.Forms.Button();
            this.btnRF5C164 = new System.Windows.Forms.Button();
            this.btnSN76489 = new System.Windows.Forms.Button();
            this.btnYM2612 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trkYM2612 = new System.Windows.Forms.TrackBar();
            this.bs1 = new System.Windows.Forms.BindingSource(this.components);
            this.label17 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.trkRF5C164 = new System.Windows.Forms.TrackBar();
            this.bs3 = new System.Windows.Forms.BindingSource(this.components);
            this.label15 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.trkSN76489 = new System.Windows.Forms.TrackBar();
            this.bs2 = new System.Windows.Forms.BindingSource(this.components);
            this.label16 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.trkPWM = new System.Windows.Forms.TrackBar();
            this.bs4 = new System.Windows.Forms.BindingSource(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.tbPWM = new System.Windows.Forms.TextBox();
            this.tbRF5C164 = new System.Windows.Forms.TextBox();
            this.tbSN76489 = new System.Windows.Forms.TextBox();
            this.tbYM2612 = new System.Windows.Forms.TextBox();
            this.tpOther = new System.Windows.Forms.TabPage();
            this.lblLoopTimes = new System.Windows.Forms.Label();
            this.btnDataPath = new System.Windows.Forms.Button();
            this.tbLoopTimes = new System.Windows.Forms.TextBox();
            this.tbDataPath = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnOpenSettingFolder = new System.Windows.Forms.Button();
            this.cbUseGetInst = new System.Windows.Forms.CheckBox();
            this.cbUseLoopTimes = new System.Windows.Forms.CheckBox();
            this.cbUseMIDIKeyboard = new System.Windows.Forms.CheckBox();
            this.gbMIDIKeyboard = new System.Windows.Forms.GroupBox();
            this.gbUseChannel = new System.Windows.Forms.GroupBox();
            this.cbFM1 = new System.Windows.Forms.CheckBox();
            this.cbFM2 = new System.Windows.Forms.CheckBox();
            this.cbFM3 = new System.Windows.Forms.CheckBox();
            this.cbFM4 = new System.Windows.Forms.CheckBox();
            this.cbPSG3 = new System.Windows.Forms.CheckBox();
            this.cbFM5 = new System.Windows.Forms.CheckBox();
            this.cbPSG2 = new System.Windows.Forms.CheckBox();
            this.cbFM6 = new System.Windows.Forms.CheckBox();
            this.cbPSG1 = new System.Windows.Forms.CheckBox();
            this.cmbMIDIIN = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tpAbout = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.gbWaveOut.SuspendLayout();
            this.gbAsioOut.SuspendLayout();
            this.gbWasapiOut.SuspendLayout();
            this.gbDirectSound.SuspendLayout();
            this.tcSetting.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.tpModule.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpBalance.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkYM2612)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkRF5C164)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs3)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSN76489)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs2)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkPWM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs4)).BeginInit();
            this.tpOther.SuspendLayout();
            this.gbMIDIKeyboard.SuspendLayout();
            this.gbUseChannel.SuspendLayout();
            this.tpAbout.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(216, 326);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 326);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbWaveOut
            // 
            this.gbWaveOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWaveOut.Controls.Add(this.label1);
            this.gbWaveOut.Controls.Add(this.cmbWaveOutDevice);
            this.gbWaveOut.Location = new System.Drawing.Point(7, 6);
            this.gbWaveOut.Name = "gbWaveOut";
            this.gbWaveOut.Size = new System.Drawing.Size(360, 48);
            this.gbWaveOut.TabIndex = 1;
            this.gbWaveOut.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "出力デバイス";
            // 
            // cmbWaveOutDevice
            // 
            this.cmbWaveOutDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWaveOutDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaveOutDevice.FormattingEnabled = true;
            this.cmbWaveOutDevice.Location = new System.Drawing.Point(79, 18);
            this.cmbWaveOutDevice.Name = "cmbWaveOutDevice";
            this.cmbWaveOutDevice.Size = new System.Drawing.Size(275, 20);
            this.cmbWaveOutDevice.TabIndex = 0;
            // 
            // rbWaveOut
            // 
            this.rbWaveOut.AutoSize = true;
            this.rbWaveOut.Checked = true;
            this.rbWaveOut.Location = new System.Drawing.Point(13, 3);
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
            this.rbAsioOut.Location = new System.Drawing.Point(13, 185);
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
            this.rbWasapiOut.Location = new System.Drawing.Point(13, 111);
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
            this.gbAsioOut.Controls.Add(this.label4);
            this.gbAsioOut.Controls.Add(this.cmbAsioDevice);
            this.gbAsioOut.Location = new System.Drawing.Point(7, 188);
            this.gbAsioOut.Name = "gbAsioOut";
            this.gbAsioOut.Size = new System.Drawing.Size(360, 71);
            this.gbAsioOut.TabIndex = 7;
            this.gbAsioOut.TabStop = false;
            // 
            // btnASIOControlPanel
            // 
            this.btnASIOControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnASIOControlPanel.Location = new System.Drawing.Point(210, 42);
            this.btnASIOControlPanel.Name = "btnASIOControlPanel";
            this.btnASIOControlPanel.Size = new System.Drawing.Size(144, 23);
            this.btnASIOControlPanel.TabIndex = 8;
            this.btnASIOControlPanel.Text = "ASIO Control Panel";
            this.btnASIOControlPanel.UseVisualStyleBackColor = true;
            this.btnASIOControlPanel.Click += new System.EventHandler(this.btnASIOControlPanel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "出力デバイス";
            // 
            // cmbAsioDevice
            // 
            this.cmbAsioDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAsioDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAsioDevice.FormattingEnabled = true;
            this.cmbAsioDevice.Location = new System.Drawing.Point(79, 18);
            this.cmbAsioDevice.Name = "cmbAsioDevice";
            this.cmbAsioDevice.Size = new System.Drawing.Size(275, 20);
            this.cmbAsioDevice.TabIndex = 6;
            // 
            // rbDirectSoundOut
            // 
            this.rbDirectSoundOut.AutoSize = true;
            this.rbDirectSoundOut.Location = new System.Drawing.Point(13, 57);
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
            this.gbWasapiOut.Controls.Add(this.label3);
            this.gbWasapiOut.Controls.Add(this.cmbWasapiDevice);
            this.gbWasapiOut.Location = new System.Drawing.Point(7, 114);
            this.gbWasapiOut.Name = "gbWasapiOut";
            this.gbWasapiOut.Size = new System.Drawing.Size(360, 68);
            this.gbWasapiOut.TabIndex = 5;
            this.gbWasapiOut.TabStop = false;
            // 
            // rbExclusive
            // 
            this.rbExclusive.AutoSize = true;
            this.rbExclusive.Location = new System.Drawing.Point(307, 45);
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
            this.rbShare.Location = new System.Drawing.Point(254, 45);
            this.rbShare.Name = "rbShare";
            this.rbShare.Size = new System.Drawing.Size(47, 16);
            this.rbShare.TabIndex = 6;
            this.rbShare.TabStop = true;
            this.rbShare.Text = "共有";
            this.rbShare.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "出力デバイス";
            // 
            // cmbWasapiDevice
            // 
            this.cmbWasapiDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWasapiDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWasapiDevice.FormattingEnabled = true;
            this.cmbWasapiDevice.Location = new System.Drawing.Point(79, 19);
            this.cmbWasapiDevice.Name = "cmbWasapiDevice";
            this.cmbWasapiDevice.Size = new System.Drawing.Size(275, 20);
            this.cmbWasapiDevice.TabIndex = 4;
            // 
            // gbDirectSound
            // 
            this.gbDirectSound.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDirectSound.Controls.Add(this.label2);
            this.gbDirectSound.Controls.Add(this.cmbDirectSoundDevice);
            this.gbDirectSound.Location = new System.Drawing.Point(7, 60);
            this.gbDirectSound.Name = "gbDirectSound";
            this.gbDirectSound.Size = new System.Drawing.Size(360, 48);
            this.gbDirectSound.TabIndex = 3;
            this.gbDirectSound.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "出力デバイス";
            // 
            // cmbDirectSoundDevice
            // 
            this.cmbDirectSoundDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDirectSoundDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirectSoundDevice.FormattingEnabled = true;
            this.cmbDirectSoundDevice.Location = new System.Drawing.Point(79, 19);
            this.cmbDirectSoundDevice.Name = "cmbDirectSoundDevice";
            this.cmbDirectSoundDevice.Size = new System.Drawing.Size(275, 20);
            this.cmbDirectSoundDevice.TabIndex = 2;
            // 
            // tcSetting
            // 
            this.tcSetting.Controls.Add(this.tpOutput);
            this.tcSetting.Controls.Add(this.tpModule);
            this.tcSetting.Controls.Add(this.tpBalance);
            this.tcSetting.Controls.Add(this.tpOther);
            this.tcSetting.Controls.Add(this.tpAbout);
            this.tcSetting.Location = new System.Drawing.Point(1, 3);
            this.tcSetting.Name = "tcSetting";
            this.tcSetting.SelectedIndex = 0;
            this.tcSetting.Size = new System.Drawing.Size(382, 317);
            this.tcSetting.TabIndex = 3;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.lblLatencyUnit);
            this.tpOutput.Controls.Add(this.lblLatency);
            this.tpOutput.Controls.Add(this.cmbLatency);
            this.tpOutput.Controls.Add(this.rbDirectSoundOut);
            this.tpOutput.Controls.Add(this.rbWaveOut);
            this.tpOutput.Controls.Add(this.rbAsioOut);
            this.tpOutput.Controls.Add(this.gbWaveOut);
            this.tpOutput.Controls.Add(this.rbWasapiOut);
            this.tpOutput.Controls.Add(this.gbAsioOut);
            this.tpOutput.Controls.Add(this.gbDirectSound);
            this.tpOutput.Controls.Add(this.gbWasapiOut);
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(374, 291);
            this.tpOutput.TabIndex = 0;
            this.tpOutput.Text = "出力";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // lblLatencyUnit
            // 
            this.lblLatencyUnit.AutoSize = true;
            this.lblLatencyUnit.Location = new System.Drawing.Point(213, 268);
            this.lblLatencyUnit.Name = "lblLatencyUnit";
            this.lblLatencyUnit.Size = new System.Drawing.Size(20, 12);
            this.lblLatencyUnit.TabIndex = 9;
            this.lblLatencyUnit.Text = "ms";
            // 
            // lblLatency
            // 
            this.lblLatency.AutoSize = true;
            this.lblLatency.Location = new System.Drawing.Point(11, 268);
            this.lblLatency.Name = "lblLatency";
            this.lblLatency.Size = new System.Drawing.Size(53, 12);
            this.lblLatency.TabIndex = 9;
            this.lblLatency.Text = "遅延時間";
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
            this.cmbLatency.Location = new System.Drawing.Point(86, 265);
            this.cmbLatency.Name = "cmbLatency";
            this.cmbLatency.Size = new System.Drawing.Size(121, 20);
            this.cmbLatency.TabIndex = 8;
            // 
            // tpModule
            // 
            this.tpModule.Controls.Add(this.groupBox3);
            this.tpModule.Controls.Add(this.groupBox2);
            this.tpModule.Controls.Add(this.groupBox1);
            this.tpModule.Location = new System.Drawing.Point(4, 22);
            this.tpModule.Name = "tpModule";
            this.tpModule.Size = new System.Drawing.Size(374, 291);
            this.tpModule.TabIndex = 3;
            this.tpModule.Text = "音源";
            this.tpModule.UseVisualStyleBackColor = true;
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
            this.groupBox3.Location = new System.Drawing.Point(3, 214);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(368, 74);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "遅延演奏";
            // 
            // cbHiyorimiMode
            // 
            this.cbHiyorimiMode.AutoSize = true;
            this.cbHiyorimiMode.Location = new System.Drawing.Point(176, 42);
            this.cbHiyorimiMode.Name = "cbHiyorimiMode";
            this.cbHiyorimiMode.Size = new System.Drawing.Size(88, 16);
            this.cbHiyorimiMode.TabIndex = 7;
            this.cbHiyorimiMode.Text = "日和見モード";
            this.cbHiyorimiMode.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 12);
            this.label13.TabIndex = 6;
            this.label13.Text = "エミュレーション";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 43);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 12);
            this.label12.TabIndex = 6;
            this.label12.Text = "SCCI";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(144, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "ms";
            // 
            // tbLatencyEmu
            // 
            this.tbLatencyEmu.Location = new System.Drawing.Point(85, 18);
            this.tbLatencyEmu.Name = "tbLatencyEmu";
            this.tbLatencyEmu.Size = new System.Drawing.Size(53, 19);
            this.tbLatencyEmu.TabIndex = 4;
            // 
            // tbLatencySCCI
            // 
            this.tbLatencySCCI.Location = new System.Drawing.Point(85, 40);
            this.tbLatencySCCI.Name = "tbLatencySCCI";
            this.tbLatencySCCI.Size = new System.Drawing.Size(53, 19);
            this.tbLatencySCCI.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(144, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "ms";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cmbSN76489Scci);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.rbSN76489Scci);
            this.groupBox2.Controls.Add(this.tbSN76489ScciDelay);
            this.groupBox2.Controls.Add(this.cbSN76489UseWaitBoost);
            this.groupBox2.Controls.Add(this.rbSN76489Emu);
            this.groupBox2.Controls.Add(this.cbSN76489UseWait);
            this.groupBox2.Controls.Add(this.tbSN76489EmuDelay);
            this.groupBox2.Location = new System.Drawing.Point(3, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(368, 88);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SN76489";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(320, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 12);
            this.label9.TabIndex = 5;
            this.label9.Text = "ms";
            this.label9.Visible = false;
            // 
            // cmbSN76489Scci
            // 
            this.cmbSN76489Scci.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSN76489Scci.FormattingEnabled = true;
            this.cmbSN76489Scci.Location = new System.Drawing.Point(61, 39);
            this.cmbSN76489Scci.Name = "cmbSN76489Scci";
            this.cmbSN76489Scci.Size = new System.Drawing.Size(301, 20);
            this.cmbSN76489Scci.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(235, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "ms";
            this.label8.Visible = false;
            // 
            // rbSN76489Scci
            // 
            this.rbSN76489Scci.AutoSize = true;
            this.rbSN76489Scci.Location = new System.Drawing.Point(6, 40);
            this.rbSN76489Scci.Name = "rbSN76489Scci";
            this.rbSN76489Scci.Size = new System.Drawing.Size(49, 16);
            this.rbSN76489Scci.TabIndex = 1;
            this.rbSN76489Scci.Text = "SCCI";
            this.rbSN76489Scci.UseVisualStyleBackColor = true;
            // 
            // tbSN76489ScciDelay
            // 
            this.tbSN76489ScciDelay.Enabled = false;
            this.tbSN76489ScciDelay.Location = new System.Drawing.Point(261, 14);
            this.tbSN76489ScciDelay.Name = "tbSN76489ScciDelay";
            this.tbSN76489ScciDelay.Size = new System.Drawing.Size(53, 19);
            this.tbSN76489ScciDelay.TabIndex = 4;
            this.tbSN76489ScciDelay.Visible = false;
            // 
            // cbSN76489UseWaitBoost
            // 
            this.cbSN76489UseWaitBoost.AutoSize = true;
            this.cbSN76489UseWaitBoost.Enabled = false;
            this.cbSN76489UseWaitBoost.Location = new System.Drawing.Point(176, 65);
            this.cbSN76489UseWaitBoost.Name = "cbSN76489UseWaitBoost";
            this.cbSN76489UseWaitBoost.Size = new System.Drawing.Size(104, 16);
            this.cbSN76489UseWaitBoost.TabIndex = 3;
            this.cbSN76489UseWaitBoost.Text = "そのWait値を2倍";
            this.cbSN76489UseWaitBoost.UseVisualStyleBackColor = true;
            // 
            // rbSN76489Emu
            // 
            this.rbSN76489Emu.AutoSize = true;
            this.rbSN76489Emu.Checked = true;
            this.rbSN76489Emu.Location = new System.Drawing.Point(6, 18);
            this.rbSN76489Emu.Name = "rbSN76489Emu";
            this.rbSN76489Emu.Size = new System.Drawing.Size(91, 16);
            this.rbSN76489Emu.TabIndex = 0;
            this.rbSN76489Emu.TabStop = true;
            this.rbSN76489Emu.Text = "エミュレーション";
            this.rbSN76489Emu.UseVisualStyleBackColor = true;
            // 
            // cbSN76489UseWait
            // 
            this.cbSN76489UseWait.AutoSize = true;
            this.cbSN76489UseWait.Enabled = false;
            this.cbSN76489UseWait.Location = new System.Drawing.Point(61, 65);
            this.cbSN76489UseWait.Name = "cbSN76489UseWait";
            this.cbSN76489UseWait.Size = new System.Drawing.Size(109, 16);
            this.cbSN76489UseWait.TabIndex = 3;
            this.cbSN76489UseWait.Text = "Waitシグナル発信";
            this.cbSN76489UseWait.UseVisualStyleBackColor = true;
            this.cbSN76489UseWait.CheckedChanged += new System.EventHandler(this.cbSN76489UseWait_CheckedChanged);
            // 
            // tbSN76489EmuDelay
            // 
            this.tbSN76489EmuDelay.Enabled = false;
            this.tbSN76489EmuDelay.Location = new System.Drawing.Point(176, 14);
            this.tbSN76489EmuDelay.Name = "tbSN76489EmuDelay";
            this.tbSN76489EmuDelay.Size = new System.Drawing.Size(53, 19);
            this.tbSN76489EmuDelay.TabIndex = 4;
            this.tbSN76489EmuDelay.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbYM2612ScciDelay);
            this.groupBox1.Controls.Add(this.tbYM2612EmuDelay);
            this.groupBox1.Controls.Add(this.cbOnlyPCMEmulation);
            this.groupBox1.Controls.Add(this.cbYM2612UseWaitBoost);
            this.groupBox1.Controls.Add(this.cbYM2612UseWait);
            this.groupBox1.Controls.Add(this.cmbYM2612Scci);
            this.groupBox1.Controls.Add(this.rbYM2612Scci);
            this.groupBox1.Controls.Add(this.rbYM2612Emu);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 111);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "YM2612";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(320, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "ms";
            this.label7.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(235, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "ms";
            this.label6.Visible = false;
            // 
            // tbYM2612ScciDelay
            // 
            this.tbYM2612ScciDelay.Enabled = false;
            this.tbYM2612ScciDelay.Location = new System.Drawing.Point(261, 14);
            this.tbYM2612ScciDelay.Name = "tbYM2612ScciDelay";
            this.tbYM2612ScciDelay.Size = new System.Drawing.Size(53, 19);
            this.tbYM2612ScciDelay.TabIndex = 4;
            this.tbYM2612ScciDelay.Visible = false;
            // 
            // tbYM2612EmuDelay
            // 
            this.tbYM2612EmuDelay.Enabled = false;
            this.tbYM2612EmuDelay.Location = new System.Drawing.Point(176, 14);
            this.tbYM2612EmuDelay.Name = "tbYM2612EmuDelay";
            this.tbYM2612EmuDelay.Size = new System.Drawing.Size(53, 19);
            this.tbYM2612EmuDelay.TabIndex = 4;
            this.tbYM2612EmuDelay.Visible = false;
            // 
            // cbOnlyPCMEmulation
            // 
            this.cbOnlyPCMEmulation.AutoSize = true;
            this.cbOnlyPCMEmulation.Location = new System.Drawing.Point(61, 87);
            this.cbOnlyPCMEmulation.Name = "cbOnlyPCMEmulation";
            this.cbOnlyPCMEmulation.Size = new System.Drawing.Size(154, 16);
            this.cbOnlyPCMEmulation.TabIndex = 3;
            this.cbOnlyPCMEmulation.Text = "PCMだけエミュレーションする";
            this.cbOnlyPCMEmulation.UseVisualStyleBackColor = true;
            // 
            // cbYM2612UseWaitBoost
            // 
            this.cbYM2612UseWaitBoost.AutoSize = true;
            this.cbYM2612UseWaitBoost.Location = new System.Drawing.Point(176, 65);
            this.cbYM2612UseWaitBoost.Name = "cbYM2612UseWaitBoost";
            this.cbYM2612UseWaitBoost.Size = new System.Drawing.Size(104, 16);
            this.cbYM2612UseWaitBoost.TabIndex = 3;
            this.cbYM2612UseWaitBoost.Text = "そのWait値を2倍";
            this.cbYM2612UseWaitBoost.UseVisualStyleBackColor = true;
            // 
            // cbYM2612UseWait
            // 
            this.cbYM2612UseWait.AutoSize = true;
            this.cbYM2612UseWait.Location = new System.Drawing.Point(61, 65);
            this.cbYM2612UseWait.Name = "cbYM2612UseWait";
            this.cbYM2612UseWait.Size = new System.Drawing.Size(109, 16);
            this.cbYM2612UseWait.TabIndex = 3;
            this.cbYM2612UseWait.Text = "Waitシグナル発信";
            this.cbYM2612UseWait.UseVisualStyleBackColor = true;
            this.cbYM2612UseWait.CheckedChanged += new System.EventHandler(this.cbYM2612UseWait_CheckedChanged);
            // 
            // cmbYM2612Scci
            // 
            this.cmbYM2612Scci.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYM2612Scci.FormattingEnabled = true;
            this.cmbYM2612Scci.Location = new System.Drawing.Point(61, 39);
            this.cmbYM2612Scci.Name = "cmbYM2612Scci";
            this.cmbYM2612Scci.Size = new System.Drawing.Size(301, 20);
            this.cmbYM2612Scci.TabIndex = 2;
            // 
            // rbYM2612Scci
            // 
            this.rbYM2612Scci.AutoSize = true;
            this.rbYM2612Scci.Location = new System.Drawing.Point(6, 40);
            this.rbYM2612Scci.Name = "rbYM2612Scci";
            this.rbYM2612Scci.Size = new System.Drawing.Size(49, 16);
            this.rbYM2612Scci.TabIndex = 1;
            this.rbYM2612Scci.Text = "SCCI";
            this.rbYM2612Scci.UseVisualStyleBackColor = true;
            // 
            // rbYM2612Emu
            // 
            this.rbYM2612Emu.AutoSize = true;
            this.rbYM2612Emu.Checked = true;
            this.rbYM2612Emu.Location = new System.Drawing.Point(6, 18);
            this.rbYM2612Emu.Name = "rbYM2612Emu";
            this.rbYM2612Emu.Size = new System.Drawing.Size(91, 16);
            this.rbYM2612Emu.TabIndex = 0;
            this.rbYM2612Emu.TabStop = true;
            this.rbYM2612Emu.Text = "エミュレーション";
            this.rbYM2612Emu.UseVisualStyleBackColor = true;
            // 
            // tpBalance
            // 
            this.tpBalance.BackColor = System.Drawing.Color.White;
            this.tpBalance.Controls.Add(this.groupBox5);
            this.tpBalance.Controls.Add(this.groupBox4);
            this.tpBalance.Location = new System.Drawing.Point(4, 22);
            this.tpBalance.Name = "tpBalance";
            this.tpBalance.Size = new System.Drawing.Size(374, 291);
            this.tpBalance.TabIndex = 4;
            this.tpBalance.Text = "バランス";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbDispFrameCounter);
            this.groupBox5.Location = new System.Drawing.Point(3, 231);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(263, 37);
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnPWM);
            this.groupBox4.Controls.Add(this.btnRF5C164);
            this.groupBox4.Controls.Add(this.btnSN76489);
            this.groupBox4.Controls.Add(this.btnYM2612);
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.panel3);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.panel2);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.panel4);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.tbPWM);
            this.groupBox4.Controls.Add(this.tbRF5C164);
            this.groupBox4.Controls.Add(this.tbSN76489);
            this.groupBox4.Controls.Add(this.tbYM2612);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(263, 222);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "エミュレーションチップ　音量バランス";
            // 
            // btnPWM
            // 
            this.btnPWM.Location = new System.Drawing.Point(236, 197);
            this.btnPWM.Name = "btnPWM";
            this.btnPWM.Size = new System.Drawing.Size(19, 19);
            this.btnPWM.TabIndex = 4;
            this.btnPWM.Text = "%";
            this.btnPWM.UseVisualStyleBackColor = true;
            this.btnPWM.Click += new System.EventHandler(this.btnPWM_Click);
            // 
            // btnRF5C164
            // 
            this.btnRF5C164.Location = new System.Drawing.Point(168, 197);
            this.btnRF5C164.Name = "btnRF5C164";
            this.btnRF5C164.Size = new System.Drawing.Size(19, 19);
            this.btnRF5C164.TabIndex = 4;
            this.btnRF5C164.Text = "%";
            this.btnRF5C164.UseVisualStyleBackColor = true;
            this.btnRF5C164.Click += new System.EventHandler(this.btnRF5C164_Click);
            // 
            // btnSN76489
            // 
            this.btnSN76489.Location = new System.Drawing.Point(101, 197);
            this.btnSN76489.Name = "btnSN76489";
            this.btnSN76489.Size = new System.Drawing.Size(19, 19);
            this.btnSN76489.TabIndex = 4;
            this.btnSN76489.Text = "%";
            this.btnSN76489.UseVisualStyleBackColor = true;
            this.btnSN76489.Click += new System.EventHandler(this.btnSN76489_Click);
            // 
            // btnYM2612
            // 
            this.btnYM2612.Location = new System.Drawing.Point(33, 197);
            this.btnYM2612.Name = "btnYM2612";
            this.btnYM2612.Size = new System.Drawing.Size(19, 19);
            this.btnYM2612.TabIndex = 4;
            this.btnYM2612.Text = "%";
            this.btnYM2612.UseVisualStyleBackColor = true;
            this.btnYM2612.Click += new System.EventHandler(this.btnYM2612_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.trkYM2612);
            this.panel1.Location = new System.Drawing.Point(6, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(45, 160);
            this.panel1.TabIndex = 1;
            // 
            // trkYM2612
            // 
            this.trkYM2612.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bs1, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.trkYM2612.LargeChange = 10;
            this.trkYM2612.Location = new System.Drawing.Point(-1, -1);
            this.trkYM2612.Maximum = 200;
            this.trkYM2612.Name = "trkYM2612";
            this.trkYM2612.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkYM2612.Size = new System.Drawing.Size(45, 160);
            this.trkYM2612.TabIndex = 0;
            this.trkYM2612.TickFrequency = 10;
            this.trkYM2612.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trkYM2612.Value = 100;
            // 
            // bs1
            // 
            this.bs1.DataSource = typeof(MDPlayer.BindData);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label17.Location = new System.Drawing.Point(215, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(33, 12);
            this.label17.TabIndex = 3;
            this.label17.Text = "PWM";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.trkRF5C164);
            this.panel3.Location = new System.Drawing.Point(141, 33);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(45, 160);
            this.panel3.TabIndex = 1;
            // 
            // trkRF5C164
            // 
            this.trkRF5C164.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bs3, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.trkRF5C164.LargeChange = 10;
            this.trkRF5C164.Location = new System.Drawing.Point(-1, -1);
            this.trkRF5C164.Maximum = 200;
            this.trkRF5C164.Name = "trkRF5C164";
            this.trkRF5C164.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkRF5C164.Size = new System.Drawing.Size(45, 160);
            this.trkRF5C164.TabIndex = 0;
            this.trkRF5C164.TickFrequency = 10;
            this.trkRF5C164.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trkRF5C164.Value = 100;
            // 
            // bs3
            // 
            this.bs3.DataSource = typeof(MDPlayer.BindData);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label15.Location = new System.Drawing.Point(68, 18);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 12);
            this.label15.TabIndex = 3;
            this.label15.Text = "SN76489";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.trkSN76489);
            this.panel2.Location = new System.Drawing.Point(74, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(45, 160);
            this.panel2.TabIndex = 1;
            // 
            // trkSN76489
            // 
            this.trkSN76489.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bs2, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.trkSN76489.LargeChange = 10;
            this.trkSN76489.Location = new System.Drawing.Point(-1, -1);
            this.trkSN76489.Maximum = 200;
            this.trkSN76489.Name = "trkSN76489";
            this.trkSN76489.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkSN76489.Size = new System.Drawing.Size(45, 160);
            this.trkSN76489.TabIndex = 0;
            this.trkSN76489.TickFrequency = 10;
            this.trkSN76489.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trkSN76489.Value = 100;
            // 
            // bs2
            // 
            this.bs2.DataSource = typeof(MDPlayer.BindData);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label16.Location = new System.Drawing.Point(135, 18);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 12);
            this.label16.TabIndex = 3;
            this.label16.Text = "RF5C164";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.trkPWM);
            this.panel4.Location = new System.Drawing.Point(209, 33);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(45, 160);
            this.panel4.TabIndex = 1;
            // 
            // trkPWM
            // 
            this.trkPWM.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bs4, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.trkPWM.LargeChange = 10;
            this.trkPWM.Location = new System.Drawing.Point(-1, -1);
            this.trkPWM.Maximum = 200;
            this.trkPWM.Name = "trkPWM";
            this.trkPWM.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkPWM.Size = new System.Drawing.Size(45, 160);
            this.trkPWM.TabIndex = 0;
            this.trkPWM.TickFrequency = 10;
            this.trkPWM.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trkPWM.Value = 100;
            // 
            // bs4
            // 
            this.bs4.DataSource = typeof(MDPlayer.BindData);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label14.Location = new System.Drawing.Point(4, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 12);
            this.label14.TabIndex = 3;
            this.label14.Text = "YM2612";
            // 
            // tbPWM
            // 
            this.tbPWM.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bs4, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.tbPWM.Location = new System.Drawing.Point(209, 197);
            this.tbPWM.MaxLength = 3;
            this.tbPWM.Name = "tbPWM";
            this.tbPWM.Size = new System.Drawing.Size(27, 19);
            this.tbPWM.TabIndex = 2;
            this.tbPWM.Text = "200";
            this.tbPWM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPWM.TextChanged += new System.EventHandler(this.tbPWM_TextChanged);
            // 
            // tbRF5C164
            // 
            this.tbRF5C164.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bs3, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.tbRF5C164.Location = new System.Drawing.Point(141, 197);
            this.tbRF5C164.MaxLength = 3;
            this.tbRF5C164.Name = "tbRF5C164";
            this.tbRF5C164.Size = new System.Drawing.Size(27, 19);
            this.tbRF5C164.TabIndex = 2;
            this.tbRF5C164.Text = "200";
            this.tbRF5C164.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRF5C164.TextChanged += new System.EventHandler(this.tbRF5C164_TextChanged);
            // 
            // tbSN76489
            // 
            this.tbSN76489.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bs2, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.tbSN76489.Location = new System.Drawing.Point(74, 197);
            this.tbSN76489.MaxLength = 3;
            this.tbSN76489.Name = "tbSN76489";
            this.tbSN76489.Size = new System.Drawing.Size(27, 19);
            this.tbSN76489.TabIndex = 2;
            this.tbSN76489.Text = "200";
            this.tbSN76489.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSN76489.TextChanged += new System.EventHandler(this.tbSN76489_TextChanged);
            // 
            // tbYM2612
            // 
            this.tbYM2612.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bs1, "Value", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.tbYM2612.Location = new System.Drawing.Point(6, 197);
            this.tbYM2612.MaxLength = 3;
            this.tbYM2612.Name = "tbYM2612";
            this.tbYM2612.Size = new System.Drawing.Size(27, 19);
            this.tbYM2612.TabIndex = 2;
            this.tbYM2612.Text = "200";
            this.tbYM2612.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbYM2612.TextChanged += new System.EventHandler(this.tbYM2612_TextChanged);
            // 
            // tpOther
            // 
            this.tpOther.Controls.Add(this.lblLoopTimes);
            this.tpOther.Controls.Add(this.btnDataPath);
            this.tpOther.Controls.Add(this.tbLoopTimes);
            this.tpOther.Controls.Add(this.tbDataPath);
            this.tpOther.Controls.Add(this.label19);
            this.tpOther.Controls.Add(this.btnOpenSettingFolder);
            this.tpOther.Controls.Add(this.cbUseGetInst);
            this.tpOther.Controls.Add(this.cbUseLoopTimes);
            this.tpOther.Controls.Add(this.cbUseMIDIKeyboard);
            this.tpOther.Controls.Add(this.gbMIDIKeyboard);
            this.tpOther.Location = new System.Drawing.Point(4, 22);
            this.tpOther.Name = "tpOther";
            this.tpOther.Size = new System.Drawing.Size(374, 291);
            this.tpOther.TabIndex = 2;
            this.tpOther.Text = "Other";
            this.tpOther.UseVisualStyleBackColor = true;
            // 
            // lblLoopTimes
            // 
            this.lblLoopTimes.AutoSize = true;
            this.lblLoopTimes.Location = new System.Drawing.Point(342, 125);
            this.lblLoopTimes.Name = "lblLoopTimes";
            this.lblLoopTimes.Size = new System.Drawing.Size(17, 12);
            this.lblLoopTimes.TabIndex = 1;
            this.lblLoopTimes.Text = "回";
            // 
            // btnDataPath
            // 
            this.btnDataPath.Location = new System.Drawing.Point(342, 167);
            this.btnDataPath.Name = "btnDataPath";
            this.btnDataPath.Size = new System.Drawing.Size(23, 23);
            this.btnDataPath.TabIndex = 16;
            this.btnDataPath.Text = "...";
            this.btnDataPath.UseVisualStyleBackColor = true;
            this.btnDataPath.Click += new System.EventHandler(this.btnDataPath_Click);
            // 
            // tbLoopTimes
            // 
            this.tbLoopTimes.Location = new System.Drawing.Point(284, 122);
            this.tbLoopTimes.Name = "tbLoopTimes";
            this.tbLoopTimes.Size = new System.Drawing.Size(52, 19);
            this.tbLoopTimes.TabIndex = 0;
            // 
            // tbDataPath
            // 
            this.tbDataPath.Location = new System.Drawing.Point(75, 169);
            this.tbDataPath.Name = "tbDataPath";
            this.tbDataPath.Size = new System.Drawing.Size(261, 19);
            this.tbDataPath.TabIndex = 15;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 172);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 12);
            this.label19.TabIndex = 14;
            this.label19.Text = "データPath";
            // 
            // btnOpenSettingFolder
            // 
            this.btnOpenSettingFolder.Location = new System.Drawing.Point(7, 265);
            this.btnOpenSettingFolder.Name = "btnOpenSettingFolder";
            this.btnOpenSettingFolder.Size = new System.Drawing.Size(125, 23);
            this.btnOpenSettingFolder.TabIndex = 13;
            this.btnOpenSettingFolder.Text = "設定フォルダーを開く";
            this.btnOpenSettingFolder.UseVisualStyleBackColor = true;
            this.btnOpenSettingFolder.Click += new System.EventHandler(this.btnOpenSettingFolder_Click);
            // 
            // cbUseGetInst
            // 
            this.cbUseGetInst.AutoSize = true;
            this.cbUseGetInst.Location = new System.Drawing.Point(9, 147);
            this.cbUseGetInst.Name = "cbUseGetInst";
            this.cbUseGetInst.Size = new System.Drawing.Size(286, 16);
            this.cbUseGetInst.TabIndex = 12;
            this.cbUseGetInst.Text = "音色欄をクリック時、その音色をクリップボードにコピーする";
            this.cbUseGetInst.UseVisualStyleBackColor = true;
            // 
            // cbUseLoopTimes
            // 
            this.cbUseLoopTimes.AutoSize = true;
            this.cbUseLoopTimes.Location = new System.Drawing.Point(9, 124);
            this.cbUseLoopTimes.Name = "cbUseLoopTimes";
            this.cbUseLoopTimes.Size = new System.Drawing.Size(216, 16);
            this.cbUseLoopTimes.TabIndex = 0;
            this.cbUseLoopTimes.Text = "無限ループ時、指定の回数だけ繰り返す";
            this.cbUseLoopTimes.UseVisualStyleBackColor = true;
            this.cbUseLoopTimes.CheckedChanged += new System.EventHandler(this.cbUseLoopTimes_CheckedChanged);
            // 
            // cbUseMIDIKeyboard
            // 
            this.cbUseMIDIKeyboard.AutoSize = true;
            this.cbUseMIDIKeyboard.Enabled = false;
            this.cbUseMIDIKeyboard.Location = new System.Drawing.Point(9, 4);
            this.cbUseMIDIKeyboard.Name = "cbUseMIDIKeyboard";
            this.cbUseMIDIKeyboard.Size = new System.Drawing.Size(122, 16);
            this.cbUseMIDIKeyboard.TabIndex = 10;
            this.cbUseMIDIKeyboard.Text = "Use MIDI Keyboard";
            this.cbUseMIDIKeyboard.UseVisualStyleBackColor = true;
            this.cbUseMIDIKeyboard.CheckedChanged += new System.EventHandler(this.cbUseMIDIKeyboard_CheckedChanged);
            // 
            // gbMIDIKeyboard
            // 
            this.gbMIDIKeyboard.Controls.Add(this.gbUseChannel);
            this.gbMIDIKeyboard.Controls.Add(this.cmbMIDIIN);
            this.gbMIDIKeyboard.Controls.Add(this.label5);
            this.gbMIDIKeyboard.Enabled = false;
            this.gbMIDIKeyboard.Location = new System.Drawing.Point(3, 8);
            this.gbMIDIKeyboard.Name = "gbMIDIKeyboard";
            this.gbMIDIKeyboard.Size = new System.Drawing.Size(368, 110);
            this.gbMIDIKeyboard.TabIndex = 0;
            this.gbMIDIKeyboard.TabStop = false;
            // 
            // gbUseChannel
            // 
            this.gbUseChannel.Controls.Add(this.cbFM1);
            this.gbUseChannel.Controls.Add(this.cbFM2);
            this.gbUseChannel.Controls.Add(this.cbFM3);
            this.gbUseChannel.Controls.Add(this.cbFM4);
            this.gbUseChannel.Controls.Add(this.cbPSG3);
            this.gbUseChannel.Controls.Add(this.cbFM5);
            this.gbUseChannel.Controls.Add(this.cbPSG2);
            this.gbUseChannel.Controls.Add(this.cbFM6);
            this.gbUseChannel.Controls.Add(this.cbPSG1);
            this.gbUseChannel.Location = new System.Drawing.Point(6, 44);
            this.gbUseChannel.Name = "gbUseChannel";
            this.gbUseChannel.Size = new System.Drawing.Size(356, 60);
            this.gbUseChannel.TabIndex = 1;
            this.gbUseChannel.TabStop = false;
            this.gbUseChannel.Text = "use channel";
            // 
            // cbFM1
            // 
            this.cbFM1.AutoSize = true;
            this.cbFM1.Location = new System.Drawing.Point(6, 18);
            this.cbFM1.Name = "cbFM1";
            this.cbFM1.Size = new System.Drawing.Size(46, 16);
            this.cbFM1.TabIndex = 2;
            this.cbFM1.Text = "FM1";
            this.cbFM1.UseVisualStyleBackColor = true;
            // 
            // cbFM2
            // 
            this.cbFM2.AutoSize = true;
            this.cbFM2.Location = new System.Drawing.Point(64, 18);
            this.cbFM2.Name = "cbFM2";
            this.cbFM2.Size = new System.Drawing.Size(46, 16);
            this.cbFM2.TabIndex = 3;
            this.cbFM2.Text = "FM2";
            this.cbFM2.UseVisualStyleBackColor = true;
            // 
            // cbFM3
            // 
            this.cbFM3.AutoSize = true;
            this.cbFM3.Location = new System.Drawing.Point(122, 18);
            this.cbFM3.Name = "cbFM3";
            this.cbFM3.Size = new System.Drawing.Size(46, 16);
            this.cbFM3.TabIndex = 3;
            this.cbFM3.Text = "FM3";
            this.cbFM3.UseVisualStyleBackColor = true;
            // 
            // cbFM4
            // 
            this.cbFM4.AutoSize = true;
            this.cbFM4.Location = new System.Drawing.Point(180, 18);
            this.cbFM4.Name = "cbFM4";
            this.cbFM4.Size = new System.Drawing.Size(46, 16);
            this.cbFM4.TabIndex = 4;
            this.cbFM4.Text = "FM4";
            this.cbFM4.UseVisualStyleBackColor = true;
            // 
            // cbPSG3
            // 
            this.cbPSG3.AutoSize = true;
            this.cbPSG3.Location = new System.Drawing.Point(122, 40);
            this.cbPSG3.Name = "cbPSG3";
            this.cbPSG3.Size = new System.Drawing.Size(52, 16);
            this.cbPSG3.TabIndex = 9;
            this.cbPSG3.Text = "PSG3";
            this.cbPSG3.UseVisualStyleBackColor = true;
            // 
            // cbFM5
            // 
            this.cbFM5.AutoSize = true;
            this.cbFM5.Location = new System.Drawing.Point(238, 18);
            this.cbFM5.Name = "cbFM5";
            this.cbFM5.Size = new System.Drawing.Size(46, 16);
            this.cbFM5.TabIndex = 5;
            this.cbFM5.Text = "FM5";
            this.cbFM5.UseVisualStyleBackColor = true;
            // 
            // cbPSG2
            // 
            this.cbPSG2.AutoSize = true;
            this.cbPSG2.Location = new System.Drawing.Point(64, 40);
            this.cbPSG2.Name = "cbPSG2";
            this.cbPSG2.Size = new System.Drawing.Size(52, 16);
            this.cbPSG2.TabIndex = 8;
            this.cbPSG2.Text = "PSG2";
            this.cbPSG2.UseVisualStyleBackColor = true;
            // 
            // cbFM6
            // 
            this.cbFM6.AutoSize = true;
            this.cbFM6.Location = new System.Drawing.Point(296, 18);
            this.cbFM6.Name = "cbFM6";
            this.cbFM6.Size = new System.Drawing.Size(46, 16);
            this.cbFM6.TabIndex = 6;
            this.cbFM6.Text = "FM6";
            this.cbFM6.UseVisualStyleBackColor = true;
            // 
            // cbPSG1
            // 
            this.cbPSG1.AutoSize = true;
            this.cbPSG1.Location = new System.Drawing.Point(6, 40);
            this.cbPSG1.Name = "cbPSG1";
            this.cbPSG1.Size = new System.Drawing.Size(52, 16);
            this.cbPSG1.TabIndex = 7;
            this.cbPSG1.Text = "PSG1";
            this.cbPSG1.UseVisualStyleBackColor = true;
            // 
            // cmbMIDIIN
            // 
            this.cmbMIDIIN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMIDIIN.FormattingEnabled = true;
            this.cmbMIDIIN.Location = new System.Drawing.Point(72, 18);
            this.cmbMIDIIN.Name = "cmbMIDIIN";
            this.cmbMIDIIN.Size = new System.Drawing.Size(290, 20);
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
            // tpAbout
            // 
            this.tpAbout.Controls.Add(this.tableLayoutPanel);
            this.tpAbout.Location = new System.Drawing.Point(4, 22);
            this.tpAbout.Name = "tpAbout";
            this.tpAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tpAbout.Size = new System.Drawing.Size(374, 291);
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
            this.tableLayoutPanel.Size = new System.Drawing.Size(368, 285);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPictureBox.Image = global::MDPlayer.Properties.Resources.フェーリAndMD2;
            this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
            this.logoPictureBox.Size = new System.Drawing.Size(115, 279);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Location = new System.Drawing.Point(127, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(238, 16);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "製品名";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Location = new System.Drawing.Point(127, 28);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(238, 16);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "バージョン";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyright.Location = new System.Drawing.Point(127, 56);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(238, 16);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "著作権";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCompanyName.Location = new System.Drawing.Point(127, 84);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 16);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(238, 16);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "会社名";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(127, 110);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(238, 146);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "説明";
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.tcSetting);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "frmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "オプション";
            this.gbWaveOut.ResumeLayout(false);
            this.gbWaveOut.PerformLayout();
            this.gbAsioOut.ResumeLayout(false);
            this.gbAsioOut.PerformLayout();
            this.gbWasapiOut.ResumeLayout(false);
            this.gbWasapiOut.PerformLayout();
            this.gbDirectSound.ResumeLayout(false);
            this.gbDirectSound.PerformLayout();
            this.tcSetting.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            this.tpModule.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tpBalance.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkYM2612)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkRF5C164)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSN76489)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs2)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkPWM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs4)).EndInit();
            this.tpOther.ResumeLayout(false);
            this.tpOther.PerformLayout();
            this.gbMIDIKeyboard.ResumeLayout(false);
            this.gbMIDIKeyboard.PerformLayout();
            this.gbUseChannel.ResumeLayout(false);
            this.gbUseChannel.PerformLayout();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWaveOutDevice;
        private System.Windows.Forms.Button btnASIOControlPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbAsioDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbWasapiDevice;
        private System.Windows.Forms.Label label2;
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
        private System.Windows.Forms.CheckBox cbPSG3;
        private System.Windows.Forms.CheckBox cbFM5;
        private System.Windows.Forms.CheckBox cbPSG2;
        private System.Windows.Forms.CheckBox cbFM6;
        private System.Windows.Forms.CheckBox cbPSG1;
        private System.Windows.Forms.ComboBox cmbMIDIIN;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbExclusive;
        private System.Windows.Forms.RadioButton rbShare;
        private System.Windows.Forms.Label lblLatencyUnit;
        private System.Windows.Forms.Label lblLatency;
        private System.Windows.Forms.ComboBox cmbLatency;
        private System.Windows.Forms.TabPage tpModule;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbYM2612Scci;
        private System.Windows.Forms.RadioButton rbYM2612Scci;
        private System.Windows.Forms.RadioButton rbYM2612Emu;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbSN76489Scci;
        private System.Windows.Forms.RadioButton rbSN76489Scci;
        private System.Windows.Forms.RadioButton rbSN76489Emu;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSN76489ScciDelay;
        private System.Windows.Forms.TextBox tbSN76489EmuDelay;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbYM2612ScciDelay;
        private System.Windows.Forms.TextBox tbYM2612EmuDelay;
        private System.Windows.Forms.CheckBox cbOnlyPCMEmulation;
        private System.Windows.Forms.CheckBox cbYM2612UseWait;
        private System.Windows.Forms.CheckBox cbYM2612UseWaitBoost;
        private System.Windows.Forms.CheckBox cbSN76489UseWaitBoost;
        private System.Windows.Forms.CheckBox cbSN76489UseWait;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbLatencyEmu;
        private System.Windows.Forms.TextBox tbLatencySCCI;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tpBalance;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbYM2612;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trkYM2612;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TrackBar trkPWM;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TrackBar trkSN76489;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TrackBar trkRF5C164;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnPWM;
        private System.Windows.Forms.Button btnRF5C164;
        private System.Windows.Forms.Button btnSN76489;
        private System.Windows.Forms.Button btnYM2612;
        private System.Windows.Forms.TextBox tbPWM;
        private System.Windows.Forms.TextBox tbRF5C164;
        private System.Windows.Forms.TextBox tbSN76489;
        private System.Windows.Forms.BindingSource bs1;
        private System.Windows.Forms.BindingSource bs3;
        private System.Windows.Forms.BindingSource bs2;
        private System.Windows.Forms.BindingSource bs4;
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
    }
}