namespace MDPlayer
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
            this.tsmiC140 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOPNA = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOPM = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.cmsOpenOtherPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.BackColor = System.Drawing.Color.Black;
            this.pbScreen.Image = global::MDPlayer.Properties.Resources.plane;
            this.pbScreen.Location = new System.Drawing.Point(0, 0);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(320, 224);
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
            this.tsmiC140,
            this.tsmiOPNA,
            this.tsmiOPM});
            this.cmsOpenOtherPanel.Name = "cmsOpenOtherPanel";
            this.cmsOpenOtherPanel.Size = new System.Drawing.Size(108, 70);
            // 
            // tsmiC140
            // 
            this.tsmiC140.Name = "tsmiC140";
            this.tsmiC140.Size = new System.Drawing.Size(107, 22);
            this.tsmiC140.Text = "C140";
            this.tsmiC140.Click += new System.EventHandler(this.tsmiC140_Click);
            // 
            // tsmiOPNA
            // 
            this.tsmiOPNA.Name = "tsmiOPNA";
            this.tsmiOPNA.Size = new System.Drawing.Size(107, 22);
            this.tsmiOPNA.Text = "OPNA";
            this.tsmiOPNA.Click += new System.EventHandler(this.tsmiOPNA_Click);
            // 
            // tsmiOPM
            // 
            this.tsmiOPM.Name = "tsmiOPM";
            this.tsmiOPM.Size = new System.Drawing.Size(107, 22);
            this.tsmiOPM.Text = "OPM";
            this.tsmiOPM.Click += new System.EventHandler(this.tsmiOPM_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(320, 224);
            this.Controls.Add(this.pbScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(336, 263);
            this.MinimumSize = new System.Drawing.Size(336, 263);
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
        private System.Windows.Forms.ToolStripMenuItem tsmiC140;
        private System.Windows.Forms.ToolStripMenuItem tsmiOPNA;
        private System.Windows.Forms.ToolStripMenuItem tsmiOPM;
    }
}

