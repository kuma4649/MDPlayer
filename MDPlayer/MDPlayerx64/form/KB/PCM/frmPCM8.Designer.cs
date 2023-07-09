#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
# endif
namespace MDPlayer.form
{
    partial class frmPCM8
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPCM8));
            pbScreen = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbScreen).BeginInit();
            SuspendLayout();
            // 
            // pbScreen
            // 
            pbScreen.Location = new Point(0, 0);
            pbScreen.Name = "pbScreen";
            pbScreen.Size = new Size(100, 50);
            pbScreen.TabIndex = 0;
            pbScreen.TabStop = false;
            pbScreen.MouseClick += pbScreen_MouseClick;
            // 
            // frmPCM8
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(179, 70);
            Controls.Add(pbScreen);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmPCM8";
            Text = "PCM8";
            FormClosed += frmPCM8_FormClosed;
            Load += frmPCM8_Load;
            ((System.ComponentModel.ISupportInitialize)pbScreen).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public PictureBox pbScreen;
    }
}