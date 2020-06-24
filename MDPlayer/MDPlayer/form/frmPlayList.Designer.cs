using System;
using System.Collections.Generic;

namespace MDPlayer.form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlayList));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.dgvList = new System.Windows.Forms.DataGridView();
            this.clmKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSongNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmZipFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPlayingNow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmEXT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmTitleJ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmGame = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmGameJ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmComposer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmComposerJ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmVGMby = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmConverted = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSpacer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmsPlayList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.type設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiA = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiD = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiE = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiG = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiH = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiI = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiJ = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiPlayThis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelThis = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDelAllMusic = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbOpenPlayList = new System.Windows.Forms.ToolStripButton();
            this.tsbSavePlayList = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAddMusic = new System.Windows.Forms.ToolStripButton();
            this.tsbAddFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUp = new System.Windows.Forms.ToolStripButton();
            this.tsbDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbJapanese = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTextExt = new System.Windows.Forms.ToolStripButton();
            this.tsbMMLExt = new System.Windows.Forms.ToolStripButton();
            this.tsbImgExt = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            this.cmsPlayList.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.BottomToolStripPanel, "toolStripContainer1.BottomToolStripPanel");
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dgvList);
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.LeftToolStripPanel, "toolStripContainer1.LeftToolStripPanel");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.RightToolStripPanel, "toolStripContainer1.RightToolStripPanel");
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.TopToolStripPanel, "toolStripContainer1.TopToolStripPanel");
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // dgvList
            // 
            resources.ApplyResources(this.dgvList, "dgvList");
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
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmKey,
            this.clmSongNo,
            this.clmZipFileName,
            this.clmFileName,
            this.clmPlayingNow,
            this.clmEXT,
            this.clmType,
            this.clmTitle,
            this.clmTitleJ,
            this.clmGame,
            this.clmGameJ,
            this.clmComposer,
            this.clmComposerJ,
            this.clmVGMby,
            this.clmConverted,
            this.clmNotes,
            this.clmDuration,
            this.clmSpacer});
            this.dgvList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvList.Name = "dgvList";
            this.dgvList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvList.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dgvList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvList.RowTemplate.ContextMenuStrip = this.cmsPlayList;
            this.dgvList.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvList.RowTemplate.Height = 10;
            this.dgvList.RowTemplate.ReadOnly = true;
            this.dgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.ShowCellErrors = false;
            this.dgvList.ShowCellToolTips = false;
            this.dgvList.ShowEditingIcon = false;
            this.dgvList.ShowRowErrors = false;
            this.dgvList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvList_CellDoubleClick);
            this.dgvList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvList_CellMouseClick);
            // 
            // clmKey
            // 
            resources.ApplyResources(this.clmKey, "clmKey");
            this.clmKey.Name = "clmKey";
            this.clmKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSongNo
            // 
            resources.ApplyResources(this.clmSongNo, "clmSongNo");
            this.clmSongNo.Name = "clmSongNo";
            // 
            // clmZipFileName
            // 
            resources.ApplyResources(this.clmZipFileName, "clmZipFileName");
            this.clmZipFileName.Name = "clmZipFileName";
            // 
            // clmFileName
            // 
            resources.ApplyResources(this.clmFileName, "clmFileName");
            this.clmFileName.Name = "clmFileName";
            this.clmFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmPlayingNow
            // 
            this.clmPlayingNow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.clmPlayingNow, "clmPlayingNow");
            this.clmPlayingNow.Name = "clmPlayingNow";
            this.clmPlayingNow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmPlayingNow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmEXT
            // 
            resources.ApplyResources(this.clmEXT, "clmEXT");
            this.clmEXT.Name = "clmEXT";
            this.clmEXT.ReadOnly = true;
            this.clmEXT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmType
            // 
            resources.ApplyResources(this.clmType, "clmType");
            this.clmType.Name = "clmType";
            this.clmType.ReadOnly = true;
            this.clmType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmTitle
            // 
            resources.ApplyResources(this.clmTitle, "clmTitle");
            this.clmTitle.Name = "clmTitle";
            this.clmTitle.ReadOnly = true;
            this.clmTitle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmTitleJ
            // 
            resources.ApplyResources(this.clmTitleJ, "clmTitleJ");
            this.clmTitleJ.Name = "clmTitleJ";
            this.clmTitleJ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmGame
            // 
            resources.ApplyResources(this.clmGame, "clmGame");
            this.clmGame.Name = "clmGame";
            this.clmGame.ReadOnly = true;
            this.clmGame.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmGameJ
            // 
            resources.ApplyResources(this.clmGameJ, "clmGameJ");
            this.clmGameJ.Name = "clmGameJ";
            this.clmGameJ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmComposer
            // 
            resources.ApplyResources(this.clmComposer, "clmComposer");
            this.clmComposer.Name = "clmComposer";
            this.clmComposer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmComposerJ
            // 
            resources.ApplyResources(this.clmComposerJ, "clmComposerJ");
            this.clmComposerJ.Name = "clmComposerJ";
            this.clmComposerJ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmVGMby
            // 
            resources.ApplyResources(this.clmVGMby, "clmVGMby");
            this.clmVGMby.Name = "clmVGMby";
            this.clmVGMby.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmConverted
            // 
            resources.ApplyResources(this.clmConverted, "clmConverted");
            this.clmConverted.Name = "clmConverted";
            this.clmConverted.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmNotes
            // 
            resources.ApplyResources(this.clmNotes, "clmNotes");
            this.clmNotes.Name = "clmNotes";
            this.clmNotes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmDuration
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clmDuration.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.clmDuration, "clmDuration");
            this.clmDuration.Name = "clmDuration";
            this.clmDuration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSpacer
            // 
            this.clmSpacer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.clmSpacer, "clmSpacer");
            this.clmSpacer.Name = "clmSpacer";
            this.clmSpacer.ReadOnly = true;
            this.clmSpacer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cmsPlayList
            // 
            resources.ApplyResources(this.cmsPlayList, "cmsPlayList");
            this.cmsPlayList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.type設定ToolStripMenuItem,
            this.toolStripSeparator5,
            this.tsmiPlayThis,
            this.tsmiDelThis,
            this.toolStripSeparator3,
            this.tsmiDelAllMusic,
            this.tsmiOpenFolder});
            this.cmsPlayList.Name = "cmsPlayList";
            // 
            // type設定ToolStripMenuItem
            // 
            resources.ApplyResources(this.type設定ToolStripMenuItem, "type設定ToolStripMenuItem");
            this.type設定ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiA,
            this.tsmiB,
            this.tsmiC,
            this.tsmiD,
            this.tsmiE,
            this.tsmiF,
            this.tsmiG,
            this.tsmiH,
            this.tsmiI,
            this.tsmiJ});
            this.type設定ToolStripMenuItem.Name = "type設定ToolStripMenuItem";
            // 
            // tsmiA
            // 
            resources.ApplyResources(this.tsmiA, "tsmiA");
            this.tsmiA.Name = "tsmiA";
            this.tsmiA.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiB
            // 
            resources.ApplyResources(this.tsmiB, "tsmiB");
            this.tsmiB.Name = "tsmiB";
            this.tsmiB.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiC
            // 
            resources.ApplyResources(this.tsmiC, "tsmiC");
            this.tsmiC.Name = "tsmiC";
            this.tsmiC.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiD
            // 
            resources.ApplyResources(this.tsmiD, "tsmiD");
            this.tsmiD.Name = "tsmiD";
            this.tsmiD.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiE
            // 
            resources.ApplyResources(this.tsmiE, "tsmiE");
            this.tsmiE.Name = "tsmiE";
            this.tsmiE.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiF
            // 
            resources.ApplyResources(this.tsmiF, "tsmiF");
            this.tsmiF.Name = "tsmiF";
            this.tsmiF.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiG
            // 
            resources.ApplyResources(this.tsmiG, "tsmiG");
            this.tsmiG.Name = "tsmiG";
            this.tsmiG.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiH
            // 
            resources.ApplyResources(this.tsmiH, "tsmiH");
            this.tsmiH.Name = "tsmiH";
            this.tsmiH.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiI
            // 
            resources.ApplyResources(this.tsmiI, "tsmiI");
            this.tsmiI.Name = "tsmiI";
            this.tsmiI.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // tsmiJ
            // 
            resources.ApplyResources(this.tsmiJ, "tsmiJ");
            this.tsmiJ.Name = "tsmiJ";
            this.tsmiJ.Click += new System.EventHandler(this.tsmiA_Click);
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // tsmiPlayThis
            // 
            resources.ApplyResources(this.tsmiPlayThis, "tsmiPlayThis");
            this.tsmiPlayThis.Name = "tsmiPlayThis";
            this.tsmiPlayThis.Click += new System.EventHandler(this.tsmiPlayThis_Click);
            // 
            // tsmiDelThis
            // 
            resources.ApplyResources(this.tsmiDelThis, "tsmiDelThis");
            this.tsmiDelThis.Name = "tsmiDelThis";
            this.tsmiDelThis.Click += new System.EventHandler(this.tsmiDelThis_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // tsmiDelAllMusic
            // 
            resources.ApplyResources(this.tsmiDelAllMusic, "tsmiDelAllMusic");
            this.tsmiDelAllMusic.Name = "tsmiDelAllMusic";
            this.tsmiDelAllMusic.Click += new System.EventHandler(this.tsmiDelAllMusic_Click);
            // 
            // tsmiOpenFolder
            // 
            resources.ApplyResources(this.tsmiOpenFolder, "tsmiOpenFolder");
            this.tsmiOpenFolder.Name = "tsmiOpenFolder";
            this.tsmiOpenFolder.Click += new System.EventHandler(this.tsmiOpenFolder_Click);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenPlayList,
            this.tsbSavePlayList,
            this.toolStripSeparator1,
            this.tsbAddMusic,
            this.tsbAddFolder,
            this.toolStripSeparator2,
            this.tsbUp,
            this.tsbDown,
            this.toolStripSeparator4,
            this.tsbJapanese,
            this.toolStripSeparator6,
            this.tsbTextExt,
            this.tsbMMLExt,
            this.tsbImgExt});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Stretch = true;
            // 
            // tsbOpenPlayList
            // 
            resources.ApplyResources(this.tsbOpenPlayList, "tsbOpenPlayList");
            this.tsbOpenPlayList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenPlayList.Image = global::MDPlayer.Properties.Resources.openPL;
            this.tsbOpenPlayList.Name = "tsbOpenPlayList";
            this.tsbOpenPlayList.Click += new System.EventHandler(this.tsbOpenPlayList_Click);
            // 
            // tsbSavePlayList
            // 
            resources.ApplyResources(this.tsbSavePlayList, "tsbSavePlayList");
            this.tsbSavePlayList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSavePlayList.Image = global::MDPlayer.Properties.Resources.savePL;
            this.tsbSavePlayList.Name = "tsbSavePlayList";
            this.tsbSavePlayList.Click += new System.EventHandler(this.tsbSavePlayList_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // tsbAddMusic
            // 
            resources.ApplyResources(this.tsbAddMusic, "tsbAddMusic");
            this.tsbAddMusic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddMusic.Image = global::MDPlayer.Properties.Resources.addPL;
            this.tsbAddMusic.Name = "tsbAddMusic";
            this.tsbAddMusic.Click += new System.EventHandler(this.tsbAddMusic_Click);
            // 
            // tsbAddFolder
            // 
            resources.ApplyResources(this.tsbAddFolder, "tsbAddFolder");
            this.tsbAddFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddFolder.Image = global::MDPlayer.Properties.Resources.addFolderPL;
            this.tsbAddFolder.Name = "tsbAddFolder";
            this.tsbAddFolder.Click += new System.EventHandler(this.tsbAddFolder_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // tsbUp
            // 
            resources.ApplyResources(this.tsbUp, "tsbUp");
            this.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUp.Image = global::MDPlayer.Properties.Resources.upPL;
            this.tsbUp.Name = "tsbUp";
            this.tsbUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbDown
            // 
            resources.ApplyResources(this.tsbDown, "tsbDown");
            this.tsbDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDown.Image = global::MDPlayer.Properties.Resources.downPL;
            this.tsbDown.Name = "tsbDown";
            this.tsbDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // tsbJapanese
            // 
            resources.ApplyResources(this.tsbJapanese, "tsbJapanese");
            this.tsbJapanese.CheckOnClick = true;
            this.tsbJapanese.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbJapanese.Image = global::MDPlayer.Properties.Resources.japPL;
            this.tsbJapanese.Name = "tsbJapanese";
            this.tsbJapanese.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // tsbTextExt
            // 
            resources.ApplyResources(this.tsbTextExt, "tsbTextExt");
            this.tsbTextExt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTextExt.Image = global::MDPlayer.Properties.Resources.txtPL;
            this.tsbTextExt.Name = "tsbTextExt";
            this.tsbTextExt.Click += new System.EventHandler(this.tsbTextExt_Click);
            // 
            // tsbMMLExt
            // 
            resources.ApplyResources(this.tsbMMLExt, "tsbMMLExt");
            this.tsbMMLExt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMMLExt.Image = global::MDPlayer.Properties.Resources.mmlPL;
            this.tsbMMLExt.Name = "tsbMMLExt";
            this.tsbMMLExt.Click += new System.EventHandler(this.tsbMMLExt_Click);
            // 
            // tsbImgExt
            // 
            resources.ApplyResources(this.tsbImgExt, "tsbImgExt");
            this.tsbImgExt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbImgExt.Image = global::MDPlayer.Properties.Resources.imgPL;
            this.tsbImgExt.Name = "tsbImgExt";
            this.tsbImgExt.Click += new System.EventHandler(this.tsbImgExt_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmPlayList
            // 
            resources.ApplyResources(this, "$this");
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "frmPlayList";
            this.Opacity = 0D;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPlayList_FormClosing);
            this.Load += new System.EventHandler(this.frmPlayList_Load);
            this.Shown += new System.EventHandler(this.frmPlayList_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmPlayList_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmPlayList_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPlayList_KeyDown);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            this.cmsPlayList.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelAllMusic;
        private System.Windows.Forms.ToolStripButton tsbAddFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsbJapanese;
        private System.Windows.Forms.ToolStripMenuItem type設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiA;
        private System.Windows.Forms.ToolStripMenuItem tsmiB;
        private System.Windows.Forms.ToolStripMenuItem tsmiC;
        private System.Windows.Forms.ToolStripMenuItem tsmiD;
        private System.Windows.Forms.ToolStripMenuItem tsmiE;
        private System.Windows.Forms.ToolStripMenuItem tsmiF;
        private System.Windows.Forms.ToolStripMenuItem tsmiG;
        private System.Windows.Forms.ToolStripMenuItem tsmiH;
        private System.Windows.Forms.ToolStripMenuItem tsmiI;
        private System.Windows.Forms.ToolStripMenuItem tsmiJ;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSongNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmZipFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPlayingNow;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmEXT;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmTitleJ;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmGame;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmGameJ;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmComposer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmComposerJ;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmVGMby;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmConverted;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDuration;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSpacer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton tsbTextExt;
        private System.Windows.Forms.ToolStripButton tsbMMLExt;
        private System.Windows.Forms.ToolStripButton tsbImgExt;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFolder;
    }
}