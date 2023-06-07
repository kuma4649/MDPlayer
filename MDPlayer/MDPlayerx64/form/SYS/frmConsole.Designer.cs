namespace MDPlayerx64.form.SYS
{
    partial class frmConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConsole));
            tbLog = new TextBox();
            menuStrip1 = new MenuStrip();
            levelToolStripMenuItem = new ToolStripMenuItem();
            tsmiTrace = new ToolStripMenuItem();
            tsmiDebug = new ToolStripMenuItem();
            tsmiWarning = new ToolStripMenuItem();
            tsmiError = new ToolStripMenuItem();
            tsmiInformation = new ToolStripMenuItem();
            tsmiClear = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tbLog
            // 
            tbLog.BackColor = Color.Black;
            tbLog.Dock = DockStyle.Fill;
            tbLog.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point);
            tbLog.ForeColor = Color.SlateBlue;
            tbLog.Location = new Point(0, 24);
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ScrollBars = ScrollBars.Both;
            tbLog.Size = new Size(800, 426);
            tbLog.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { levelToolStripMenuItem, tsmiClear });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // levelToolStripMenuItem
            // 
            levelToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiTrace, tsmiDebug, tsmiWarning, tsmiError, tsmiInformation });
            levelToolStripMenuItem.Name = "levelToolStripMenuItem";
            levelToolStripMenuItem.Size = new Size(46, 20);
            levelToolStripMenuItem.Text = "Level";
            // 
            // tsmiTrace
            // 
            tsmiTrace.Name = "tsmiTrace";
            tsmiTrace.Size = new Size(145, 22);
            tsmiTrace.Text = "0:Trace";
            tsmiTrace.Click += tsmiTrace_Click;
            // 
            // tsmiDebug
            // 
            tsmiDebug.Name = "tsmiDebug";
            tsmiDebug.Size = new Size(145, 22);
            tsmiDebug.Text = "1:Debug";
            tsmiDebug.Click += tsmiDebug_Click;
            // 
            // tsmiWarning
            // 
            tsmiWarning.Name = "tsmiWarning";
            tsmiWarning.Size = new Size(145, 22);
            tsmiWarning.Text = "2:Warning";
            tsmiWarning.Click += tsmiWarning_Click;
            // 
            // tsmiError
            // 
            tsmiError.Name = "tsmiError";
            tsmiError.Size = new Size(145, 22);
            tsmiError.Text = "3:Error";
            tsmiError.Click += tsmiError_Click;
            // 
            // tsmiInformation
            // 
            tsmiInformation.Name = "tsmiInformation";
            tsmiInformation.Size = new Size(145, 22);
            tsmiInformation.Text = "4:Information";
            tsmiInformation.Click += tsmiInformation_Click;
            // 
            // tsmiClear
            // 
            tsmiClear.Name = "tsmiClear";
            tsmiClear.Size = new Size(45, 20);
            tsmiClear.Text = "Clear";
            tsmiClear.Click += tsmiClear_Click;
            // 
            // frmConsole
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tbLog);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "frmConsole";
            Text = "Console";
            FormClosed += frmConsole_FormClosed;
            Load += frmConsole_Load;
            Shown += frmConsole_Shown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbLog;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem levelToolStripMenuItem;
        private ToolStripMenuItem tsmiTrace;
        private ToolStripMenuItem tsmiDebug;
        private ToolStripMenuItem tsmiError;
        private ToolStripMenuItem tsmiWarning;
        private ToolStripMenuItem tsmiInformation;
        private ToolStripMenuItem tsmiClear;
    }
}