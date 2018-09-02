namespace MDPlayer.form
{
    partial class frmMixer2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMixer2));
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiLoadDriverBalance = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLoadSongBalance = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSaveDriverBalance = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveSongBalance = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.ctxtMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.ContextMenuStrip = this.ctxtMenu;
            this.pbScreen.Image = global::MDPlayer.Properties.Resources.planeMixer;
            this.pbScreen.Location = new System.Drawing.Point(0, 0);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(320, 216);
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            this.pbScreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbScreen_MouseClick);
            this.pbScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMixer2_MouseDown);
            this.pbScreen.MouseEnter += new System.EventHandler(this.pbScreen_MouseEnter);
            this.pbScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMixer2_MouseMove);
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLoadDriverBalance,
            this.tsmiLoadSongBalance,
            this.toolStripSeparator1,
            this.tsmiSaveDriverBalance,
            this.tsmiSaveSongBalance});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(221, 120);
            // 
            // tsmiLoadDriverBalance
            // 
            this.tsmiLoadDriverBalance.Enabled = false;
            this.tsmiLoadDriverBalance.Name = "tsmiLoadDriverBalance";
            this.tsmiLoadDriverBalance.Size = new System.Drawing.Size(220, 22);
            this.tsmiLoadDriverBalance.Text = "読込　ドライバーバランス";
            this.tsmiLoadDriverBalance.Click += new System.EventHandler(this.tsmiLoadDriverBalance_Click);
            // 
            // tsmiLoadSongBalance
            // 
            this.tsmiLoadSongBalance.Enabled = false;
            this.tsmiLoadSongBalance.Name = "tsmiLoadSongBalance";
            this.tsmiLoadSongBalance.Size = new System.Drawing.Size(220, 22);
            this.tsmiLoadSongBalance.Text = "読込　ソングバランス";
            this.tsmiLoadSongBalance.Click += new System.EventHandler(this.tsmiLoadSongBalance_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(217, 6);
            // 
            // tsmiSaveDriverBalance
            // 
            this.tsmiSaveDriverBalance.Name = "tsmiSaveDriverBalance";
            this.tsmiSaveDriverBalance.Size = new System.Drawing.Size(220, 22);
            this.tsmiSaveDriverBalance.Text = "保存　ドライバーバランス";
            this.tsmiSaveDriverBalance.Click += new System.EventHandler(this.tsmiSaveDriverBalance_Click);
            // 
            // tsmiSaveSongBalance
            // 
            this.tsmiSaveSongBalance.Enabled = false;
            this.tsmiSaveSongBalance.Name = "tsmiSaveSongBalance";
            this.tsmiSaveSongBalance.Size = new System.Drawing.Size(220, 22);
            this.tsmiSaveSongBalance.Text = "保存　ソングバランス";
            this.tsmiSaveSongBalance.Click += new System.EventHandler(this.tsmiSaveSongBalance_Click);
            // 
            // frmMixer2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 216);
            this.Controls.Add(this.pbScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMixer2";
            this.Text = "Mixer2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMixer2_FormClosed);
            this.Load += new System.EventHandler(this.frmMixer2_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMixer2_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMixer2_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMixer2_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.ctxtMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveDriverBalance;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveSongBalance;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadDriverBalance;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadSongBalance;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}