#if X64
using MDPlayerx64;
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
namespace MDPlayer.form
{
    partial class frmInfo
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInfo));
            timer = new System.Windows.Forms.Timer(components);
            dgvInfo = new DataGridView();
            clmKey = new DataGridViewTextBoxColumn();
            clmVal = new DataGridViewTextBoxColumn();
            rtbLyric = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)dgvInfo).BeginInit();
            SuspendLayout();
            // 
            // timer
            // 
            timer.Interval = 10;
            timer.Tick += timer_Tick;
            // 
            // dgvInfo
            // 
            dgvInfo.AllowUserToAddRows = false;
            dgvInfo.AllowUserToDeleteRows = false;
            dgvInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvInfo.BackgroundColor = Color.Black;
            dgvInfo.BorderStyle = BorderStyle.None;
            dgvInfo.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvInfo.ColumnHeadersHeight = 9;
            dgvInfo.ColumnHeadersVisible = false;
            dgvInfo.Columns.AddRange(new DataGridViewColumn[] { clmKey, clmVal });
            dgvInfo.GridColor = Color.FromArgb(10, 10, 10);
            dgvInfo.Location = new Point(0, 0);
            dgvInfo.Margin = new Padding(0);
            dgvInfo.Name = "dgvInfo";
            dgvInfo.ReadOnly = true;
            dgvInfo.RowHeadersVisible = false;
            dgvInfo.RowTemplate.Height = 19;
            dgvInfo.RowTemplate.ReadOnly = true;
            dgvInfo.Size = new Size(523, 258);
            dgvInfo.TabIndex = 17;
            // 
            // clmKey
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle1.BackColor = Color.Black;
            dataGridViewCellStyle1.Font = new Font("Consolas", 6.75F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.Lavender;
            clmKey.DefaultCellStyle = dataGridViewCellStyle1;
            clmKey.HeaderText = "Column1";
            clmKey.Name = "clmKey";
            clmKey.ReadOnly = true;
            clmKey.Width = 60;
            // 
            // clmVal
            // 
            clmVal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = Color.Black;
            dataGridViewCellStyle2.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.SlateBlue;
            clmVal.DefaultCellStyle = dataGridViewCellStyle2;
            clmVal.HeaderText = "Column1";
            clmVal.Name = "clmVal";
            clmVal.ReadOnly = true;
            // 
            // rtbLyric
            // 
            rtbLyric.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbLyric.BackColor = Color.Black;
            rtbLyric.BorderStyle = BorderStyle.None;
            rtbLyric.Location = new Point(12, 265);
            rtbLyric.Name = "rtbLyric";
            rtbLyric.ReadOnly = true;
            rtbLyric.ScrollBars = RichTextBoxScrollBars.None;
            rtbLyric.Size = new Size(496, 18);
            rtbLyric.TabIndex = 18;
            rtbLyric.Text = "";
            // 
            // frmInfo
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(520, 281);
            Controls.Add(rtbLyric);
            Controls.Add(dgvInfo);
            Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            MinimumSize = new Size(370, 320);
            Name = "frmInfo";
            Text = "Information";
            FormClosed += frmInfo_FormClosed;
            Load += frmInfo_Load;
            ((System.ComponentModel.ISupportInitialize)dgvInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer timer;
        private DataGridView dgvInfo;
        private RichTextBox rtbLyric;
        private DataGridViewTextBoxColumn clmKey;
        private DataGridViewTextBoxColumn clmVal;
    }
}