namespace MDPlayer.form
{
    partial class frmTPPut
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btCh6 = new System.Windows.Forms.Button();
            this.btCh5 = new System.Windows.Forms.Button();
            this.btCh4 = new System.Windows.Forms.Button();
            this.btCh3 = new System.Windows.Forms.Button();
            this.btCh2 = new System.Windows.Forms.Button();
            this.btCh1 = new System.Windows.Forms.Button();
            this.dgvTonePallet = new System.Windows.Forms.DataGridView();
            this.clmNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSpacer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btApply = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTonePallet)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btCh6);
            this.groupBox1.Controls.Add(this.btCh5);
            this.groupBox1.Controls.Add(this.btCh4);
            this.groupBox1.Controls.Add(this.btCh3);
            this.groupBox1.Controls.Add(this.btCh2);
            this.groupBox1.Controls.Add(this.btCh1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 81);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "YM2612(From)";
            // 
            // btCh6
            // 
            this.btCh6.Location = new System.Drawing.Point(178, 48);
            this.btCh6.Name = "btCh6";
            this.btCh6.Size = new System.Drawing.Size(80, 24);
            this.btCh6.TabIndex = 0;
            this.btCh6.Tag = "6";
            this.btCh6.Text = "Ch.6";
            this.btCh6.UseVisualStyleBackColor = true;
            this.btCh6.Click += new System.EventHandler(this.btChn_Click);
            // 
            // btCh5
            // 
            this.btCh5.Location = new System.Drawing.Point(92, 48);
            this.btCh5.Name = "btCh5";
            this.btCh5.Size = new System.Drawing.Size(80, 24);
            this.btCh5.TabIndex = 0;
            this.btCh5.Tag = "5";
            this.btCh5.Text = "Ch.5";
            this.btCh5.UseVisualStyleBackColor = true;
            this.btCh5.Click += new System.EventHandler(this.btChn_Click);
            // 
            // btCh4
            // 
            this.btCh4.Location = new System.Drawing.Point(6, 48);
            this.btCh4.Name = "btCh4";
            this.btCh4.Size = new System.Drawing.Size(80, 24);
            this.btCh4.TabIndex = 0;
            this.btCh4.Tag = "4";
            this.btCh4.Text = "Ch.4";
            this.btCh4.UseVisualStyleBackColor = true;
            this.btCh4.Click += new System.EventHandler(this.btChn_Click);
            // 
            // btCh3
            // 
            this.btCh3.Location = new System.Drawing.Point(178, 18);
            this.btCh3.Name = "btCh3";
            this.btCh3.Size = new System.Drawing.Size(80, 24);
            this.btCh3.TabIndex = 0;
            this.btCh3.Tag = "3";
            this.btCh3.Text = "Ch.3";
            this.btCh3.UseVisualStyleBackColor = true;
            this.btCh3.Click += new System.EventHandler(this.btChn_Click);
            // 
            // btCh2
            // 
            this.btCh2.Location = new System.Drawing.Point(92, 18);
            this.btCh2.Name = "btCh2";
            this.btCh2.Size = new System.Drawing.Size(80, 24);
            this.btCh2.TabIndex = 0;
            this.btCh2.Tag = "2";
            this.btCh2.Text = "Ch.2";
            this.btCh2.UseVisualStyleBackColor = true;
            this.btCh2.Click += new System.EventHandler(this.btChn_Click);
            // 
            // btCh1
            // 
            this.btCh1.Location = new System.Drawing.Point(6, 18);
            this.btCh1.Name = "btCh1";
            this.btCh1.Size = new System.Drawing.Size(80, 24);
            this.btCh1.TabIndex = 0;
            this.btCh1.Tag = "1";
            this.btCh1.Text = "Ch.1";
            this.btCh1.UseVisualStyleBackColor = true;
            this.btCh1.Click += new System.EventHandler(this.btChn_Click);
            // 
            // dgvTonePallet
            // 
            this.dgvTonePallet.AllowUserToAddRows = false;
            this.dgvTonePallet.AllowUserToDeleteRows = false;
            this.dgvTonePallet.AllowUserToOrderColumns = true;
            this.dgvTonePallet.AllowUserToResizeRows = false;
            this.dgvTonePallet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTonePallet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTonePallet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmNo,
            this.clmName,
            this.clmSpacer});
            this.dgvTonePallet.Location = new System.Drawing.Point(12, 121);
            this.dgvTonePallet.MultiSelect = false;
            this.dgvTonePallet.Name = "dgvTonePallet";
            this.dgvTonePallet.RowHeadersVisible = false;
            this.dgvTonePallet.RowTemplate.Height = 21;
            this.dgvTonePallet.Size = new System.Drawing.Size(264, 81);
            this.dgvTonePallet.TabIndex = 3;
            this.dgvTonePallet.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTonePallet_CellEndEdit);
            // 
            // clmNo
            // 
            this.clmNo.Frozen = true;
            this.clmNo.HeaderText = "No.";
            this.clmNo.Name = "clmNo";
            this.clmNo.ReadOnly = true;
            this.clmNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmNo.Width = 60;
            // 
            // clmName
            // 
            this.clmName.Frozen = true;
            this.clmName.HeaderText = "Name";
            this.clmName.Name = "clmName";
            this.clmName.Width = 150;
            // 
            // clmSpacer
            // 
            this.clmSpacer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmSpacer.HeaderText = "";
            this.clmSpacer.Name = "clmSpacer";
            this.clmSpacer.ReadOnly = true;
            this.clmSpacer.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Tone Pallet(To)";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(120, 208);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(39, 208);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 4;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btApply
            // 
            this.btApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btApply.Enabled = false;
            this.btApply.Location = new System.Drawing.Point(201, 208);
            this.btApply.Name = "btApply";
            this.btApply.Size = new System.Drawing.Size(75, 23);
            this.btApply.TabIndex = 5;
            this.btApply.Text = "適用";
            this.btApply.UseVisualStyleBackColor = true;
            this.btApply.Click += new System.EventHandler(this.btApply_Click);
            // 
            // frmTPPut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(288, 243);
            this.Controls.Add(this.btApply);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvTonePallet);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(304, 282);
            this.Name = "frmTPPut";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Put to Tone Pallet";
            this.Load += new System.EventHandler(this.frmTPPut_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTonePallet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvTonePallet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSpacer;
        private System.Windows.Forms.Button btApply;
        private System.Windows.Forms.Button btCh1;
        private System.Windows.Forms.Button btCh6;
        private System.Windows.Forms.Button btCh5;
        private System.Windows.Forms.Button btCh4;
        private System.Windows.Forms.Button btCh3;
        private System.Windows.Forms.Button btCh2;
    }
}