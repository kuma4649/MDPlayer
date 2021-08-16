
namespace MDPlayer.form
{
    partial class frmVisWave
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVisWave));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbHeight1 = new System.Windows.Forms.ToolStripButton();
            this.tsbHeight2 = new System.Windows.Forms.ToolStripButton();
            this.tsbHeight3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDispType1 = new System.Windows.Forms.ToolStripButton();
            this.tsbDispType2 = new System.Windows.Forms.ToolStripButton();
            this.tsbFFT = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(224, 176);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pictureBox1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(224, 176);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(224, 201);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbHeight1,
            this.tsbHeight2,
            this.tsbHeight3,
            this.toolStripSeparator1,
            this.tsbDispType1,
            this.tsbDispType2,
            this.tsbFFT});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(187, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // tsbHeight1
            // 
            this.tsbHeight1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHeight1.Image = global::MDPlayer.Properties.Resources.vHeight1;
            this.tsbHeight1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHeight1.Name = "tsbHeight1";
            this.tsbHeight1.Size = new System.Drawing.Size(23, 22);
            this.tsbHeight1.Text = "Height x 0.3";
            this.tsbHeight1.Click += new System.EventHandler(this.tsbHeight1_Click);
            // 
            // tsbHeight2
            // 
            this.tsbHeight2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHeight2.Image = global::MDPlayer.Properties.Resources.vHeight2;
            this.tsbHeight2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHeight2.Name = "tsbHeight2";
            this.tsbHeight2.Size = new System.Drawing.Size(23, 22);
            this.tsbHeight2.Text = "Height x 1.0";
            this.tsbHeight2.Click += new System.EventHandler(this.tsbHeight2_Click);
            // 
            // tsbHeight3
            // 
            this.tsbHeight3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHeight3.Image = global::MDPlayer.Properties.Resources.vHeight3;
            this.tsbHeight3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHeight3.Name = "tsbHeight3";
            this.tsbHeight3.Size = new System.Drawing.Size(23, 22);
            this.tsbHeight3.Text = "Height x 3.0";
            this.tsbHeight3.Click += new System.EventHandler(this.tsbHeight3_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbDispType1
            // 
            this.tsbDispType1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDispType1.Image = global::MDPlayer.Properties.Resources.vType1;
            this.tsbDispType1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDispType1.Name = "tsbDispType1";
            this.tsbDispType1.Size = new System.Drawing.Size(23, 22);
            this.tsbDispType1.Text = "type 1";
            this.tsbDispType1.Click += new System.EventHandler(this.tsbDispType1_Click);
            // 
            // tsbDispType2
            // 
            this.tsbDispType2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDispType2.Image = global::MDPlayer.Properties.Resources.vType2;
            this.tsbDispType2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDispType2.Name = "tsbDispType2";
            this.tsbDispType2.Size = new System.Drawing.Size(23, 22);
            this.tsbDispType2.Text = "type 2";
            this.tsbDispType2.Click += new System.EventHandler(this.tsbDispType2_Click);
            // 
            // tsbFFT
            // 
            this.tsbFFT.CheckOnClick = true;
            this.tsbFFT.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFFT.Image = global::MDPlayer.Properties.Resources.vType3;
            this.tsbFFT.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFFT.Name = "tsbFFT";
            this.tsbFFT.Size = new System.Drawing.Size(23, 22);
            this.tsbFFT.Text = "FFT";
            this.tsbFFT.Click += new System.EventHandler(this.tsbFFT_Click);
            // 
            // frmVisWave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 201);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmVisWave";
            this.Opacity = 0.9D;
            this.Text = "Visualizer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmVisWave_FormClosed);
            this.Load += new System.EventHandler(this.frmVisWave_Load);
            this.Shown += new System.EventHandler(this.frmVisWave_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbHeight1;
        private System.Windows.Forms.ToolStripButton tsbHeight2;
        private System.Windows.Forms.ToolStripButton tsbHeight3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbDispType1;
        private System.Windows.Forms.ToolStripButton tsbDispType2;
        private System.Windows.Forms.ToolStripButton tsbFFT;
    }
}