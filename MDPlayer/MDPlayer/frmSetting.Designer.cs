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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbAsioOut = new System.Windows.Forms.RadioButton();
            this.rbWasapiOut = new System.Windows.Forms.RadioButton();
            this.gbAsioOut = new System.Windows.Forms.GroupBox();
            this.btnASIOControlPanel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbAsioDevice = new System.Windows.Forms.ComboBox();
            this.rbDirectSoundOut = new System.Windows.Forms.RadioButton();
            this.gbWasapiOut = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbWasapiDevice = new System.Windows.Forms.ComboBox();
            this.gbDirectSound = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDirectSoundDevice = new System.Windows.Forms.ComboBox();
            this.gbWaveOut.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbAsioOut.SuspendLayout();
            this.gbWasapiOut.SuspendLayout();
            this.gbDirectSound.SuspendLayout();
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
            this.gbWaveOut.Location = new System.Drawing.Point(6, 18);
            this.gbWaveOut.Name = "gbWaveOut";
            this.gbWaveOut.Size = new System.Drawing.Size(348, 48);
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
            this.cmbWaveOutDevice.Size = new System.Drawing.Size(263, 20);
            this.cmbWaveOutDevice.TabIndex = 0;
            // 
            // rbWaveOut
            // 
            this.rbWaveOut.AutoSize = true;
            this.rbWaveOut.Location = new System.Drawing.Point(12, 15);
            this.rbWaveOut.Name = "rbWaveOut";
            this.rbWaveOut.Size = new System.Drawing.Size(68, 16);
            this.rbWaveOut.TabIndex = 0;
            this.rbWaveOut.TabStop = true;
            this.rbWaveOut.Text = "WaveOut";
            this.rbWaveOut.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rbAsioOut);
            this.groupBox2.Controls.Add(this.rbWasapiOut);
            this.groupBox2.Controls.Add(this.gbAsioOut);
            this.groupBox2.Controls.Add(this.rbDirectSoundOut);
            this.groupBox2.Controls.Add(this.gbWasapiOut);
            this.groupBox2.Controls.Add(this.gbDirectSound);
            this.groupBox2.Controls.Add(this.rbWaveOut);
            this.groupBox2.Controls.Add(this.gbWaveOut);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 308);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output Driver";
            // 
            // rbAsioOut
            // 
            this.rbAsioOut.AutoSize = true;
            this.rbAsioOut.Location = new System.Drawing.Point(12, 177);
            this.rbAsioOut.Name = "rbAsioOut";
            this.rbAsioOut.Size = new System.Drawing.Size(64, 16);
            this.rbAsioOut.TabIndex = 6;
            this.rbAsioOut.TabStop = true;
            this.rbAsioOut.Text = "AsioOut";
            this.rbAsioOut.UseVisualStyleBackColor = true;
            // 
            // rbWasapiOut
            // 
            this.rbWasapiOut.AutoSize = true;
            this.rbWasapiOut.Location = new System.Drawing.Point(12, 123);
            this.rbWasapiOut.Name = "rbWasapiOut";
            this.rbWasapiOut.Size = new System.Drawing.Size(77, 16);
            this.rbWasapiOut.TabIndex = 4;
            this.rbWasapiOut.TabStop = true;
            this.rbWasapiOut.Text = "WasapiOut";
            this.rbWasapiOut.UseVisualStyleBackColor = true;
            // 
            // gbAsioOut
            // 
            this.gbAsioOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAsioOut.Controls.Add(this.btnASIOControlPanel);
            this.gbAsioOut.Controls.Add(this.label4);
            this.gbAsioOut.Controls.Add(this.cmbAsioDevice);
            this.gbAsioOut.Location = new System.Drawing.Point(6, 180);
            this.gbAsioOut.Name = "gbAsioOut";
            this.gbAsioOut.Size = new System.Drawing.Size(348, 76);
            this.gbAsioOut.TabIndex = 7;
            this.gbAsioOut.TabStop = false;
            // 
            // btnASIOControlPanel
            // 
            this.btnASIOControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnASIOControlPanel.Location = new System.Drawing.Point(198, 44);
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
            this.cmbAsioDevice.Size = new System.Drawing.Size(263, 20);
            this.cmbAsioDevice.TabIndex = 6;
            // 
            // rbDirectSoundOut
            // 
            this.rbDirectSoundOut.AutoSize = true;
            this.rbDirectSoundOut.Location = new System.Drawing.Point(12, 69);
            this.rbDirectSoundOut.Name = "rbDirectSoundOut";
            this.rbDirectSoundOut.Size = new System.Drawing.Size(85, 16);
            this.rbDirectSoundOut.TabIndex = 2;
            this.rbDirectSoundOut.TabStop = true;
            this.rbDirectSoundOut.Text = "DirectSound";
            this.rbDirectSoundOut.UseVisualStyleBackColor = true;
            // 
            // gbWasapiOut
            // 
            this.gbWasapiOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWasapiOut.Controls.Add(this.label3);
            this.gbWasapiOut.Controls.Add(this.cmbWasapiDevice);
            this.gbWasapiOut.Location = new System.Drawing.Point(6, 126);
            this.gbWasapiOut.Name = "gbWasapiOut";
            this.gbWasapiOut.Size = new System.Drawing.Size(348, 48);
            this.gbWasapiOut.TabIndex = 5;
            this.gbWasapiOut.TabStop = false;
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
            this.cmbWasapiDevice.Size = new System.Drawing.Size(263, 20);
            this.cmbWasapiDevice.TabIndex = 4;
            // 
            // gbDirectSound
            // 
            this.gbDirectSound.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDirectSound.Controls.Add(this.label2);
            this.gbDirectSound.Controls.Add(this.cmbDirectSoundDevice);
            this.gbDirectSound.Location = new System.Drawing.Point(6, 72);
            this.gbDirectSound.Name = "gbDirectSound";
            this.gbDirectSound.Size = new System.Drawing.Size(348, 48);
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
            this.cmbDirectSoundDevice.Size = new System.Drawing.Size(263, 20);
            this.cmbDirectSoundDevice.TabIndex = 2;
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "frmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "設定";
            this.gbWaveOut.ResumeLayout(false);
            this.gbWaveOut.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbAsioOut.ResumeLayout(false);
            this.gbAsioOut.PerformLayout();
            this.gbWasapiOut.ResumeLayout(false);
            this.gbWasapiOut.PerformLayout();
            this.gbDirectSound.ResumeLayout(false);
            this.gbDirectSound.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbWaveOut;
        private System.Windows.Forms.RadioButton rbWaveOut;
        private System.Windows.Forms.GroupBox groupBox2;
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
    }
}