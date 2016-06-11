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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblLatencyUnit = new System.Windows.Forms.Label();
            this.lblLatency = new System.Windows.Forms.Label();
            this.cmbLatency = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
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
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.gbMIDIKeyboard.SuspendLayout();
            this.gbUseChannel.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(1, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(382, 317);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblLatencyUnit);
            this.tabPage1.Controls.Add(this.lblLatency);
            this.tabPage1.Controls.Add(this.cmbLatency);
            this.tabPage1.Controls.Add(this.rbDirectSoundOut);
            this.tabPage1.Controls.Add(this.rbWaveOut);
            this.tabPage1.Controls.Add(this.rbAsioOut);
            this.tabPage1.Controls.Add(this.gbWaveOut);
            this.tabPage1.Controls.Add(this.rbWasapiOut);
            this.tabPage1.Controls.Add(this.gbAsioOut);
            this.tabPage1.Controls.Add(this.gbDirectSound);
            this.tabPage1.Controls.Add(this.gbWasapiOut);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(374, 291);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "出力";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cbUseMIDIKeyboard);
            this.tabPage3.Controls.Add(this.gbMIDIKeyboard);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(374, 291);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Other";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIKeyboard
            // 
            this.cbUseMIDIKeyboard.AutoSize = true;
            this.cbUseMIDIKeyboard.Enabled = false;
            this.cbUseMIDIKeyboard.Location = new System.Drawing.Point(9, 3);
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
            this.gbMIDIKeyboard.Location = new System.Drawing.Point(3, 7);
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(374, 291);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "About";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.Controls.Add(this.tabControl1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.gbMIDIKeyboard.ResumeLayout(false);
            this.gbMIDIKeyboard.PerformLayout();
            this.gbUseChannel.ResumeLayout(false);
            this.gbUseChannel.PerformLayout();
            this.tabPage2.ResumeLayout(false);
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.TabPage tabPage3;
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
    }
}