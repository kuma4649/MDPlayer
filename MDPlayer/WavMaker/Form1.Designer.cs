namespace WavMaker
{
    partial class Form1
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
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnRef = new System.Windows.Forms.Button();
            this.btnMake = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(12, 12);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(261, 19);
            this.tbFileName.TabIndex = 0;
            // 
            // btnRef
            // 
            this.btnRef.Location = new System.Drawing.Point(279, 10);
            this.btnRef.Name = "btnRef";
            this.btnRef.Size = new System.Drawing.Size(35, 23);
            this.btnRef.TabIndex = 1;
            this.btnRef.Text = "...";
            this.btnRef.UseVisualStyleBackColor = true;
            this.btnRef.Click += new System.EventHandler(this.btnRef_Click);
            // 
            // btnMake
            // 
            this.btnMake.Location = new System.Drawing.Point(91, 39);
            this.btnMake.Name = "btnMake";
            this.btnMake.Size = new System.Drawing.Size(223, 23);
            this.btnMake.TabIndex = 2;
            this.btnMake.Text = "segaPCMしか変換できません";
            this.btnMake.UseVisualStyleBackColor = true;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 68);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.btnRef);
            this.Controls.Add(this.tbFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "WavMaker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.Button btnRef;
        private System.Windows.Forms.Button btnMake;
    }
}

