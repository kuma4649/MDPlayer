using MDPlayer.Properties;
namespace MDPlayer.form
{
    partial class frmYM2612MIDI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmYM2612MIDI));
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.cmsMIDIKBD = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.cmsMIDIKBD.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.Image = Resources.planeYM2612MIDI;
            this.pbScreen.Location = new System.Drawing.Point(0, 0);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(320, 184);
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            this.pbScreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbScreen_MouseClick);
            // 
            // cmsMIDIKBD
            // 
            this.cmsMIDIKBD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctsmiCopy,
            this.ctsmiPaste});
            this.cmsMIDIKBD.Name = "cmsMIDIKBD";
            this.cmsMIDIKBD.Size = new System.Drawing.Size(153, 70);
            // 
            // ctsmiCopy
            // 
            this.ctsmiCopy.Name = "ctsmiCopy";
            this.ctsmiCopy.Size = new System.Drawing.Size(152, 22);
            this.ctsmiCopy.Text = "コピー(&C)";
            this.ctsmiCopy.Click += new System.EventHandler(this.ctsmiCopy_Click);
            // 
            // ctsmiPaste
            // 
            this.ctsmiPaste.Name = "ctsmiPaste";
            this.ctsmiPaste.Size = new System.Drawing.Size(152, 22);
            this.ctsmiPaste.Text = "貼り付け(&P)";
            this.ctsmiPaste.Click += new System.EventHandler(this.ctsmiPaste_Click);
            // 
            // frmYM2612MIDI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 184);
            this.Controls.Add(this.pbScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmYM2612MIDI";
            this.Text = "MIDI(YM2612)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmYM2612MIDI_FormClosed);
            this.Load += new System.EventHandler(this.frmYM2612MIDI_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmYM2612MIDI_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.cmsMIDIKBD.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.ContextMenuStrip cmsMIDIKBD;
        private System.Windows.Forms.ToolStripMenuItem ctsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem ctsmiPaste;
    }
}