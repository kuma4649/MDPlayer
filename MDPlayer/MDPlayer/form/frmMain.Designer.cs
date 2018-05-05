namespace MDPlayer.form
{
    partial class frmMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.cmsOpenOtherPanel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.primaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPAY8910 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPC140 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPDCSG = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPFDS = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPHuC6280 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPMIDI = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPMMC5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPNESDMC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOKIM6258 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOKIM6295 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPLL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPL4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPM = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPN = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPN2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPNA = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPOPNB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPPWM = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPRF5C164 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPSegaPCM = new System.Windows.Forms.ToolStripMenuItem();
            this.sencondryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSAY8910 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSC140 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSDCSG = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSFDS = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSHuC6280 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSMIDI = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSMMC5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSNESDMC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOKIM6258 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOKIM6295 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPLL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPL4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPM = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPN = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPN2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPNA = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSOPNB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSPWM = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSRF5C164 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSSegaPCM = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.cmsOpenOtherPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.BackColor = System.Drawing.Color.Black;
            this.pbScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbScreen.Image = global::MDPlayer.Properties.Resources.planeControl;
            this.pbScreen.Location = new System.Drawing.Point(0, 0);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(320, 40);
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            this.pbScreen.DragDrop += new System.Windows.Forms.DragEventHandler(this.pbScreen_DragDrop);
            this.pbScreen.DragEnter += new System.Windows.Forms.DragEventHandler(this.pbScreen_DragEnter);
            this.pbScreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbScreen_MouseClick);
            this.pbScreen.MouseLeave += new System.EventHandler(this.pbScreen_MouseLeave);
            this.pbScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbScreen_MouseMove);
            // 
            // cmsOpenOtherPanel
            // 
            this.cmsOpenOtherPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.primaryToolStripMenuItem,
            this.sencondryToolStripMenuItem});
            this.cmsOpenOtherPanel.Name = "cmsOpenOtherPanel";
            this.cmsOpenOtherPanel.Size = new System.Drawing.Size(138, 48);
            // 
            // primaryToolStripMenuItem
            // 
            this.primaryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPAY8910,
            this.tsmiPC140,
            this.tsmiPDCSG,
            this.tsmiPFDS,
            this.tsmiPHuC6280,
            this.tsmiPMIDI,
            this.tsmiPMMC5,
            this.tsmiPNESDMC,
            this.tsmiPOKIM6258,
            this.tsmiPOKIM6295,
            this.tsmiPOPLL,
            this.tsmiPOPL4,
            this.tsmiPOPM,
            this.tsmiPOPN,
            this.tsmiPOPN2,
            this.tsmiPOPNA,
            this.tsmiPOPNB,
            this.tsmiPPWM,
            this.tsmiPRF5C164,
            this.tsmiPSegaPCM});
            this.primaryToolStripMenuItem.Name = "primaryToolStripMenuItem";
            this.primaryToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.primaryToolStripMenuItem.Text = "Primary";
            // 
            // tsmiPAY8910
            // 
            this.tsmiPAY8910.Name = "tsmiPAY8910";
            this.tsmiPAY8910.Size = new System.Drawing.Size(137, 22);
            this.tsmiPAY8910.Text = "AY8910";
            this.tsmiPAY8910.Click += new System.EventHandler(this.tsmiPAY8910_Click);
            // 
            // tsmiPC140
            // 
            this.tsmiPC140.Name = "tsmiPC140";
            this.tsmiPC140.Size = new System.Drawing.Size(137, 22);
            this.tsmiPC140.Text = "C140";
            this.tsmiPC140.Click += new System.EventHandler(this.tsmiPC140_Click);
            // 
            // tsmiPDCSG
            // 
            this.tsmiPDCSG.Name = "tsmiPDCSG";
            this.tsmiPDCSG.Size = new System.Drawing.Size(137, 22);
            this.tsmiPDCSG.Text = "DCSG";
            this.tsmiPDCSG.Click += new System.EventHandler(this.tsmiPDCSG_Click);
            // 
            // tsmiPFDS
            // 
            this.tsmiPFDS.Name = "tsmiPFDS";
            this.tsmiPFDS.Size = new System.Drawing.Size(137, 22);
            this.tsmiPFDS.Text = "FDS";
            this.tsmiPFDS.Click += new System.EventHandler(this.tsmiPFDS_Click);
            // 
            // tsmiPHuC6280
            // 
            this.tsmiPHuC6280.Name = "tsmiPHuC6280";
            this.tsmiPHuC6280.Size = new System.Drawing.Size(137, 22);
            this.tsmiPHuC6280.Text = "HuC6280";
            this.tsmiPHuC6280.Click += new System.EventHandler(this.tsmiPHuC6280_Click);
            // 
            // tsmiPMIDI
            // 
            this.tsmiPMIDI.Name = "tsmiPMIDI";
            this.tsmiPMIDI.Size = new System.Drawing.Size(137, 22);
            this.tsmiPMIDI.Text = "MIDI";
            this.tsmiPMIDI.Click += new System.EventHandler(this.tsmiPMIDI_Click);
            // 
            // tsmiPMMC5
            // 
            this.tsmiPMMC5.Name = "tsmiPMMC5";
            this.tsmiPMMC5.Size = new System.Drawing.Size(137, 22);
            this.tsmiPMMC5.Text = "MMC5";
            this.tsmiPMMC5.Click += new System.EventHandler(this.tsmiPMMC5_Click);
            // 
            // tsmiPNESDMC
            // 
            this.tsmiPNESDMC.Name = "tsmiPNESDMC";
            this.tsmiPNESDMC.Size = new System.Drawing.Size(137, 22);
            this.tsmiPNESDMC.Text = "NES&&DMC";
            this.tsmiPNESDMC.Click += new System.EventHandler(this.tsmiPNESDMC_Click);
            // 
            // tsmiPOKIM6258
            // 
            this.tsmiPOKIM6258.Name = "tsmiPOKIM6258";
            this.tsmiPOKIM6258.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOKIM6258.Text = "OKIM6258";
            this.tsmiPOKIM6258.Click += new System.EventHandler(this.tsmiPOKIM6258_Click);
            // 
            // tsmiPOKIM6295
            // 
            this.tsmiPOKIM6295.Name = "tsmiPOKIM6295";
            this.tsmiPOKIM6295.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOKIM6295.Text = "OKIM6295";
            this.tsmiPOKIM6295.Click += new System.EventHandler(this.tsmiPOKIM6295_Click);
            // 
            // tsmiPOPLL
            // 
            this.tsmiPOPLL.Name = "tsmiPOPLL";
            this.tsmiPOPLL.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPLL.Text = "OPLL";
            this.tsmiPOPLL.Click += new System.EventHandler(this.tsmiPOPLL_Click);
            // 
            // tsmiPOPL4
            // 
            this.tsmiPOPL4.Name = "tsmiPOPL4";
            this.tsmiPOPL4.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPL4.Text = "OPL4";
            this.tsmiPOPL4.Click += new System.EventHandler(this.tsmiPOPL4_Click);
            // 
            // tsmiPOPM
            // 
            this.tsmiPOPM.Name = "tsmiPOPM";
            this.tsmiPOPM.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPM.Text = "OPM";
            this.tsmiPOPM.Click += new System.EventHandler(this.tsmiPOPM_Click);
            // 
            // tsmiPOPN
            // 
            this.tsmiPOPN.Name = "tsmiPOPN";
            this.tsmiPOPN.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPN.Text = "OPN";
            this.tsmiPOPN.Click += new System.EventHandler(this.tsmiPOPN_Click);
            // 
            // tsmiPOPN2
            // 
            this.tsmiPOPN2.Name = "tsmiPOPN2";
            this.tsmiPOPN2.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPN2.Text = "OPN2";
            this.tsmiPOPN2.Click += new System.EventHandler(this.tsmiPOPN2_Click);
            // 
            // tsmiPOPNA
            // 
            this.tsmiPOPNA.Name = "tsmiPOPNA";
            this.tsmiPOPNA.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPNA.Text = "OPNA";
            this.tsmiPOPNA.Click += new System.EventHandler(this.tsmiPOPNA_Click);
            // 
            // tsmiPOPNB
            // 
            this.tsmiPOPNB.Name = "tsmiPOPNB";
            this.tsmiPOPNB.Size = new System.Drawing.Size(137, 22);
            this.tsmiPOPNB.Text = "OPNB";
            this.tsmiPOPNB.Click += new System.EventHandler(this.tsmiPOPNB_Click);
            // 
            // tsmiPPWM
            // 
            this.tsmiPPWM.Name = "tsmiPPWM";
            this.tsmiPPWM.Size = new System.Drawing.Size(137, 22);
            this.tsmiPPWM.Text = "PWM";
            this.tsmiPPWM.Click += new System.EventHandler(this.tsmiPPWM_Click);
            // 
            // tsmiPRF5C164
            // 
            this.tsmiPRF5C164.Name = "tsmiPRF5C164";
            this.tsmiPRF5C164.Size = new System.Drawing.Size(137, 22);
            this.tsmiPRF5C164.Text = "RF5C164";
            this.tsmiPRF5C164.Click += new System.EventHandler(this.tsmiPRF5C164_Click);
            // 
            // tsmiPSegaPCM
            // 
            this.tsmiPSegaPCM.Name = "tsmiPSegaPCM";
            this.tsmiPSegaPCM.Size = new System.Drawing.Size(137, 22);
            this.tsmiPSegaPCM.Text = "SEGA PCM";
            this.tsmiPSegaPCM.Click += new System.EventHandler(this.tsmiPSegaPCM_Click);
            // 
            // sencondryToolStripMenuItem
            // 
            this.sencondryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSAY8910,
            this.tsmiSC140,
            this.tsmiSDCSG,
            this.tsmiSFDS,
            this.tsmiSHuC6280,
            this.tsmiSMIDI,
            this.tsmiSMMC5,
            this.tsmiSNESDMC,
            this.tsmiSOKIM6258,
            this.tsmiSOKIM6295,
            this.tsmiSOPLL,
            this.tsmiSOPL4,
            this.tsmiSOPM,
            this.tsmiSOPN,
            this.tsmiSOPN2,
            this.tsmiSOPNA,
            this.tsmiSOPNB,
            this.tsmiSPWM,
            this.tsmiSRF5C164,
            this.tsmiSSegaPCM});
            this.sencondryToolStripMenuItem.Name = "sencondryToolStripMenuItem";
            this.sencondryToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.sencondryToolStripMenuItem.Text = "Secondary";
            // 
            // tsmiSAY8910
            // 
            this.tsmiSAY8910.Name = "tsmiSAY8910";
            this.tsmiSAY8910.Size = new System.Drawing.Size(137, 22);
            this.tsmiSAY8910.Text = "AY8910";
            this.tsmiSAY8910.Click += new System.EventHandler(this.tsmiSAY8910_Click);
            // 
            // tsmiSC140
            // 
            this.tsmiSC140.Name = "tsmiSC140";
            this.tsmiSC140.Size = new System.Drawing.Size(137, 22);
            this.tsmiSC140.Text = "C140";
            this.tsmiSC140.Click += new System.EventHandler(this.tsmiSC140_Click);
            // 
            // tsmiSDCSG
            // 
            this.tsmiSDCSG.Name = "tsmiSDCSG";
            this.tsmiSDCSG.Size = new System.Drawing.Size(137, 22);
            this.tsmiSDCSG.Text = "DCSG";
            this.tsmiSDCSG.Click += new System.EventHandler(this.tsmiSDCSG_Click);
            // 
            // tsmiSFDS
            // 
            this.tsmiSFDS.Name = "tsmiSFDS";
            this.tsmiSFDS.Size = new System.Drawing.Size(137, 22);
            this.tsmiSFDS.Text = "FDS";
            this.tsmiSFDS.Click += new System.EventHandler(this.tsmiSFDS_Click);
            // 
            // tsmiSHuC6280
            // 
            this.tsmiSHuC6280.Name = "tsmiSHuC6280";
            this.tsmiSHuC6280.Size = new System.Drawing.Size(137, 22);
            this.tsmiSHuC6280.Text = "HuC6280";
            this.tsmiSHuC6280.Click += new System.EventHandler(this.tsmiSHuC6280_Click);
            // 
            // tsmiSMIDI
            // 
            this.tsmiSMIDI.Name = "tsmiSMIDI";
            this.tsmiSMIDI.Size = new System.Drawing.Size(137, 22);
            this.tsmiSMIDI.Text = "MIDI";
            this.tsmiSMIDI.Click += new System.EventHandler(this.tsmiSMIDI_Click);
            // 
            // tsmiSMMC5
            // 
            this.tsmiSMMC5.Name = "tsmiSMMC5";
            this.tsmiSMMC5.Size = new System.Drawing.Size(137, 22);
            this.tsmiSMMC5.Text = "MMC5";
            this.tsmiSMMC5.Click += new System.EventHandler(this.tsmiSMMC5_Click);
            // 
            // tsmiSNESDMC
            // 
            this.tsmiSNESDMC.Name = "tsmiSNESDMC";
            this.tsmiSNESDMC.Size = new System.Drawing.Size(137, 22);
            this.tsmiSNESDMC.Text = "NES&&DMC";
            this.tsmiSNESDMC.Click += new System.EventHandler(this.tsmiSNESDMC_Click);
            // 
            // tsmiSOKIM6258
            // 
            this.tsmiSOKIM6258.Name = "tsmiSOKIM6258";
            this.tsmiSOKIM6258.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOKIM6258.Text = "OKIM6258";
            this.tsmiSOKIM6258.Click += new System.EventHandler(this.tsmiSOKIM6258_Click);
            // 
            // tsmiSOKIM6295
            // 
            this.tsmiSOKIM6295.Name = "tsmiSOKIM6295";
            this.tsmiSOKIM6295.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOKIM6295.Text = "OKIM6295";
            this.tsmiSOKIM6295.Click += new System.EventHandler(this.tsmiSOKIM6295_Click);
            // 
            // tsmiSOPLL
            // 
            this.tsmiSOPLL.Name = "tsmiSOPLL";
            this.tsmiSOPLL.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPLL.Text = "OPLL";
            this.tsmiSOPLL.Click += new System.EventHandler(this.tsmiSOPLL_Click);
            // 
            // tsmiSOPL4
            // 
            this.tsmiSOPL4.Name = "tsmiSOPL4";
            this.tsmiSOPL4.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPL4.Text = "OPL4";
            this.tsmiSOPL4.Click += new System.EventHandler(this.tsmiSOPL4_Click);
            // 
            // tsmiSOPM
            // 
            this.tsmiSOPM.Name = "tsmiSOPM";
            this.tsmiSOPM.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPM.Text = "OPM";
            this.tsmiSOPM.Click += new System.EventHandler(this.tsmiSOPM_Click);
            // 
            // tsmiSOPN
            // 
            this.tsmiSOPN.Name = "tsmiSOPN";
            this.tsmiSOPN.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPN.Text = "OPN";
            this.tsmiSOPN.Click += new System.EventHandler(this.tsmiSOPN_Click);
            // 
            // tsmiSOPN2
            // 
            this.tsmiSOPN2.Name = "tsmiSOPN2";
            this.tsmiSOPN2.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPN2.Text = "OPN2";
            this.tsmiSOPN2.Click += new System.EventHandler(this.tsmiSOPN2_Click);
            // 
            // tsmiSOPNA
            // 
            this.tsmiSOPNA.Name = "tsmiSOPNA";
            this.tsmiSOPNA.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPNA.Text = "OPNA";
            this.tsmiSOPNA.Click += new System.EventHandler(this.tsmiSOPNA_Click);
            // 
            // tsmiSOPNB
            // 
            this.tsmiSOPNB.Name = "tsmiSOPNB";
            this.tsmiSOPNB.Size = new System.Drawing.Size(137, 22);
            this.tsmiSOPNB.Text = "OPNB";
            this.tsmiSOPNB.Click += new System.EventHandler(this.tsmiSOPNB_Click);
            // 
            // tsmiSPWM
            // 
            this.tsmiSPWM.Name = "tsmiSPWM";
            this.tsmiSPWM.Size = new System.Drawing.Size(137, 22);
            this.tsmiSPWM.Text = "PWM";
            this.tsmiSPWM.Click += new System.EventHandler(this.tsmiSPWM_Click);
            // 
            // tsmiSRF5C164
            // 
            this.tsmiSRF5C164.Name = "tsmiSRF5C164";
            this.tsmiSRF5C164.Size = new System.Drawing.Size(137, 22);
            this.tsmiSRF5C164.Text = "RF5C164";
            this.tsmiSRF5C164.Click += new System.EventHandler(this.tsmiSRF5C164_Click);
            // 
            // tsmiSSegaPCM
            // 
            this.tsmiSSegaPCM.Name = "tsmiSSegaPCM";
            this.tsmiSSegaPCM.Size = new System.Drawing.Size(137, 22);
            this.tsmiSSegaPCM.Text = "SEGA PCM";
            this.tsmiSSegaPCM.Click += new System.EventHandler(this.tsmiSSegaPCM_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(320, 40);
            this.Controls.Add(this.pbScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(336, 263);
            this.Name = "frmMain";
            this.Text = "MDPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.cmsOpenOtherPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.ContextMenuStrip cmsOpenOtherPanel;
        private System.Windows.Forms.ToolStripMenuItem primaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPN;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPN2;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPNA;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPNB;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPM;
        private System.Windows.Forms.ToolStripMenuItem tsmiPDCSG;
        private System.Windows.Forms.ToolStripMenuItem tsmiPRF5C164;
        private System.Windows.Forms.ToolStripMenuItem tsmiPPWM;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOKIM6258;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOKIM6295;
        private System.Windows.Forms.ToolStripMenuItem tsmiPC140;
        private System.Windows.Forms.ToolStripMenuItem tsmiPSegaPCM;
        private System.Windows.Forms.ToolStripMenuItem sencondryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPN;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPN2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPNA;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPNB;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPM;
        private System.Windows.Forms.ToolStripMenuItem tsmiSDCSG;
        private System.Windows.Forms.ToolStripMenuItem tsmiSRF5C164;
        private System.Windows.Forms.ToolStripMenuItem tsmiSPWM;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOKIM6258;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOKIM6295;
        private System.Windows.Forms.ToolStripMenuItem tsmiSC140;
        private System.Windows.Forms.ToolStripMenuItem tsmiSSegaPCM;
        private System.Windows.Forms.ToolStripMenuItem tsmiPAY8910;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPLL;
        private System.Windows.Forms.ToolStripMenuItem tsmiSAY8910;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPLL;
        private System.Windows.Forms.ToolStripMenuItem tsmiPHuC6280;
        private System.Windows.Forms.ToolStripMenuItem tsmiSHuC6280;
        private System.Windows.Forms.ToolStripMenuItem tsmiPMIDI;
        private System.Windows.Forms.ToolStripMenuItem tsmiSMIDI;
        private System.Windows.Forms.ToolStripMenuItem tsmiPNESDMC;
        private System.Windows.Forms.ToolStripMenuItem tsmiSNESDMC;
        private System.Windows.Forms.ToolStripMenuItem tsmiPFDS;
        private System.Windows.Forms.ToolStripMenuItem tsmiSFDS;
        private System.Windows.Forms.ToolStripMenuItem tsmiPMMC5;
        private System.Windows.Forms.ToolStripMenuItem tsmiSMMC5;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPL4;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPL4;
    }
}

