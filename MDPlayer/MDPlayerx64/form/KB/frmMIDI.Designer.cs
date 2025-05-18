#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
namespace MDPlayer.form
{
    partial class frmMIDI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMIDI));
            pbScreen = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbScreen).BeginInit();
            SuspendLayout();
            // 
            // pbScreen
            // 
            pbScreen.Location = new Point(0, 0);
            pbScreen.Margin = new Padding(4);
            pbScreen.Name = "pbScreen";
            pbScreen.Size = new Size(513, 448);
            pbScreen.TabIndex = 0;
            pbScreen.TabStop = false;
            pbScreen.MouseClick += pbScreen_MouseClick;
            // 
            // frmMIDI
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(513, 448);
            Controls.Add(pbScreen);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            MaximizeBox = false;
            Name = "frmMIDI";
            Text = "MIDI";
            FormClosed += frmMIDI_FormClosed;
            Load += frmMIDI_Load;
            ((System.ComponentModel.ISupportInitialize)pbScreen).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.PictureBox pbScreen;
    }
}