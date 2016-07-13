using System;
using System.Collections.Generic;

namespace MDPlayer
{
    partial class frmPlayList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlayList));
            this.dgvList = new System.Windows.Forms.DataGridView();
            this.clmKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPlayingNow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmGame = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmRemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSpacer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmsPlayList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPlayThis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelThis = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbOpenPlayList = new System.Windows.Forms.ToolStripButton();
            this.tsbSavePlayList = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAddMusic = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUp = new System.Windows.Forms.ToolStripButton();
            this.tsbDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDelAllMusic = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            this.cmsPlayList.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvList
            // 
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.AllowUserToDeleteRows = false;
            this.dgvList.AllowUserToResizeRows = false;
            this.dgvList.BackgroundColor = System.Drawing.Color.Black;
            this.dgvList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvList.ColumnHeadersHeight = 20;
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmKey,
            this.clmFileName,
            this.clmPlayingNow,
            this.clmTitle,
            this.clmGame,
            this.clmRemark,
            this.clmSpacer});
            this.dgvList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvList.Location = new System.Drawing.Point(0, 0);
            this.dgvList.MultiSelect = false;
            this.dgvList.Name = "dgvList";
            this.dgvList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvList.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvList.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dgvList.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvList.RowTemplate.ContextMenuStrip = this.cmsPlayList;
            this.dgvList.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvList.RowTemplate.Height = 10;
            this.dgvList.RowTemplate.ReadOnly = true;
            this.dgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.ShowCellErrors = false;
            this.dgvList.ShowCellToolTips = false;
            this.dgvList.ShowEditingIcon = false;
            this.dgvList.ShowRowErrors = false;
            this.dgvList.Size = new System.Drawing.Size(585, 270);
            this.dgvList.TabIndex = 0;
            this.dgvList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvList_CellDoubleClick);
            this.dgvList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvList_CellMouseClick);
            // 
            // clmKey
            // 
            this.clmKey.HeaderText = "Key";
            this.clmKey.Name = "clmKey";
            this.clmKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmKey.Visible = false;
            // 
            // clmFileName
            // 
            this.clmFileName.HeaderText = "FileName";
            this.clmFileName.Name = "clmFileName";
            this.clmFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmFileName.Visible = false;
            // 
            // clmPlayingNow
            // 
            this.clmPlayingNow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmPlayingNow.HeaderText = " ";
            this.clmPlayingNow.Name = "clmPlayingNow";
            this.clmPlayingNow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmPlayingNow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmPlayingNow.Width = 25;
            // 
            // clmTitle
            // 
            this.clmTitle.HeaderText = "Title";
            this.clmTitle.Name = "clmTitle";
            this.clmTitle.ReadOnly = true;
            this.clmTitle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmTitle.Width = 200;
            // 
            // clmGame
            // 
            this.clmGame.HeaderText = "Game";
            this.clmGame.Name = "clmGame";
            this.clmGame.ReadOnly = true;
            this.clmGame.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmGame.Width = 200;
            // 
            // clmRemark
            // 
            this.clmRemark.HeaderText = "Remark";
            this.clmRemark.Name = "clmRemark";
            this.clmRemark.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSpacer
            // 
            this.clmSpacer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmSpacer.HeaderText = "";
            this.clmSpacer.Name = "clmSpacer";
            this.clmSpacer.ReadOnly = true;
            this.clmSpacer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cmsPlayList
            // 
            this.cmsPlayList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPlayThis,
            this.tsmiDelThis,
            this.toolStripSeparator3,
            this.tsmiDelAllMusic});
            this.cmsPlayList.Name = "cmsPlayList";
            this.cmsPlayList.Size = new System.Drawing.Size(153, 98);
            // 
            // tsmiPlayThis
            // 
            this.tsmiPlayThis.Name = "tsmiPlayThis";
            this.tsmiPlayThis.Size = new System.Drawing.Size(152, 22);
            this.tsmiPlayThis.Text = "この曲を再生";
            this.tsmiPlayThis.Click += new System.EventHandler(this.tsmiPlayThis_Click);
            // 
            // tsmiDelThis
            // 
            this.tsmiDelThis.Name = "tsmiDelThis";
            this.tsmiDelThis.Size = new System.Drawing.Size(152, 22);
            this.tsmiDelThis.Text = "この曲を除去";
            this.tsmiDelThis.Click += new System.EventHandler(this.tsmiDelThis_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dgvList);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(585, 270);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(585, 270);
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
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenPlayList,
            this.tsbSavePlayList,
            this.toolStripSeparator1,
            this.tsbAddMusic,
            this.toolStripSeparator2,
            this.tsbUp,
            this.tsbDown});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(161, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Visible = false;
            // 
            // tsbOpenPlayList
            // 
            this.tsbOpenPlayList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenPlayList.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpenPlayList.Image")));
            this.tsbOpenPlayList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenPlayList.Name = "tsbOpenPlayList";
            this.tsbOpenPlayList.Size = new System.Drawing.Size(23, 22);
            this.tsbOpenPlayList.Text = "プレイリストファイルを開く";
            // 
            // tsbSavePlayList
            // 
            this.tsbSavePlayList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSavePlayList.Image = ((System.Drawing.Image)(resources.GetObject("tsbSavePlayList.Image")));
            this.tsbSavePlayList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSavePlayList.Name = "tsbSavePlayList";
            this.tsbSavePlayList.Size = new System.Drawing.Size(23, 22);
            this.tsbSavePlayList.Text = "プレイリストファイルを保存";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbAddMusic
            // 
            this.tsbAddMusic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddMusic.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddMusic.Image")));
            this.tsbAddMusic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddMusic.Name = "tsbAddMusic";
            this.tsbAddMusic.Size = new System.Drawing.Size(23, 22);
            this.tsbAddMusic.Text = "曲を追加";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbUp
            // 
            this.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUp.Image = ((System.Drawing.Image)(resources.GetObject("tsbUp.Image")));
            this.tsbUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUp.Name = "tsbUp";
            this.tsbUp.Size = new System.Drawing.Size(23, 22);
            this.tsbUp.Text = "上の曲と入れ替える";
            // 
            // tsbDown
            // 
            this.tsbDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDown.Image = ((System.Drawing.Image)(resources.GetObject("tsbDown.Image")));
            this.tsbDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDown.Name = "tsbDown";
            this.tsbDown.Size = new System.Drawing.Size(23, 22);
            this.tsbDown.Text = "下の曲と入れ替える";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // tsmiDelAllMusic
            // 
            this.tsmiDelAllMusic.Name = "tsmiDelAllMusic";
            this.tsmiDelAllMusic.Size = new System.Drawing.Size(152, 22);
            this.tsmiDelAllMusic.Text = "全ての曲を除去";
            this.tsmiDelAllMusic.Click += new System.EventHandler(this.tsmiDelAllMusic_Click);
            // 
            // frmPlayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 270);
            this.Controls.Add(this.toolStripContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 120);
            this.Name = "frmPlayList";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "play list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPlayList_FormClosing);
            this.Load += new System.EventHandler(this.frmPlayList_Load);
            this.Shown += new System.EventHandler(this.frmPlayList_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            this.cmsPlayList.ResumeLayout(false);
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

        private System.Windows.Forms.DataGridView dgvList;
        private System.Windows.Forms.ContextMenuStrip cmsPlayList;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlayThis;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelThis;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbOpenPlayList;
        private System.Windows.Forms.ToolStripButton tsbSavePlayList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbAddMusic;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbUp;
        private System.Windows.Forms.ToolStripButton tsbDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPlayingNow;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmGame;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmRemark;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSpacer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelAllMusic;
    }
}