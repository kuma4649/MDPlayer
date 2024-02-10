#if X64
using MDPlayerx64;
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlayList));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            toolStripContainer1 = new ToolStripContainer();
            dgvList = new DataGridView();
            clmKey = new DataGridViewTextBoxColumn();
            clmSongNo = new DataGridViewTextBoxColumn();
            clmZipFileName = new DataGridViewTextBoxColumn();
            clmFileName = new DataGridViewTextBoxColumn();
            clmPlayingNow = new DataGridViewTextBoxColumn();
            clmEXT = new DataGridViewTextBoxColumn();
            clmType = new DataGridViewTextBoxColumn();
            clmTitle = new DataGridViewTextBoxColumn();
            clmTitleJ = new DataGridViewTextBoxColumn();
            clmDispFileName = new DataGridViewTextBoxColumn();
            clmGame = new DataGridViewTextBoxColumn();
            clmGameJ = new DataGridViewTextBoxColumn();
            clmComposer = new DataGridViewTextBoxColumn();
            clmComposerJ = new DataGridViewTextBoxColumn();
            clmVGMby = new DataGridViewTextBoxColumn();
            clmConverted = new DataGridViewTextBoxColumn();
            clmNotes = new DataGridViewTextBoxColumn();
            clmDuration = new DataGridViewTextBoxColumn();
            ClmVersion = new DataGridViewTextBoxColumn();
            ClmUseChips = new DataGridViewTextBoxColumn();
            clmSpacer = new DataGridViewTextBoxColumn();
            toolStrip1 = new ToolStrip();
            tsbOpenPlayList = new ToolStripButton();
            tsbSavePlayList = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbAddMusic = new ToolStripButton();
            tsbAddFolder = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            tsbUp = new ToolStripButton();
            tsbDown = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            tsbJapanese = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            tsbTextExt = new ToolStripButton();
            tsbMMLExt = new ToolStripButton();
            tsbImgExt = new ToolStripButton();
            cmsPlayList = new ContextMenuStrip(components);
            type設定ToolStripMenuItem = new ToolStripMenuItem();
            tsmiA = new ToolStripMenuItem();
            tsmiB = new ToolStripMenuItem();
            tsmiC = new ToolStripMenuItem();
            tsmiD = new ToolStripMenuItem();
            tsmiE = new ToolStripMenuItem();
            tsmiF = new ToolStripMenuItem();
            tsmiG = new ToolStripMenuItem();
            tsmiH = new ToolStripMenuItem();
            tsmiI = new ToolStripMenuItem();
            tsmiJ = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            tsmiPlayThis = new ToolStripMenuItem();
            tsmiDelThis = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            tsmiDelAllMusic = new ToolStripMenuItem();
            tsmiOpenFolder = new ToolStripMenuItem();
            timer1 = new System.Windows.Forms.Timer(components);
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvList).BeginInit();
            toolStrip1.SuspendLayout();
            cmsPlayList.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            toolStripContainer1.ContentPanel.Controls.Add(dgvList);
            resources.ApplyResources(toolStripContainer1, "toolStripContainer1");
            toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // dgvList
            // 
            dgvList.AllowDrop = true;
            dgvList.AllowUserToAddRows = false;
            dgvList.AllowUserToDeleteRows = false;
            dgvList.AllowUserToResizeRows = false;
            dgvList.BackgroundColor = Color.Black;
            dgvList.BorderStyle = BorderStyle.None;
            dgvList.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.Black;
            dataGridViewCellStyle1.Font = new Font("メイリオ", 8.25F);
            dataGridViewCellStyle1.ForeColor = SystemColors.MenuHighlight;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dgvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(dgvList, "dgvList");
            dgvList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvList.Columns.AddRange(new DataGridViewColumn[] { clmKey, clmSongNo, clmZipFileName, clmFileName, clmPlayingNow, clmEXT, clmType, clmTitle, clmTitleJ, clmDispFileName, clmGame, clmGameJ, clmComposer, clmComposerJ, clmVGMby, clmConverted, clmNotes, clmDuration, ClmVersion, ClmUseChips, clmSpacer });
            dgvList.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvList.Name = "dgvList";
            dgvList.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.Black;
            dataGridViewCellStyle3.Font = new Font("メイリオ", 8.25F, FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = SystemColors.Window;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvList.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = Color.Black;
            dataGridViewCellStyle4.Font = new Font("メイリオ", 8.25F, FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(192, 192, 255);
            dgvList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            dgvList.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvList.RowTemplate.Height = 10;
            dgvList.RowTemplate.ReadOnly = true;
            dgvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvList.ShowCellErrors = false;
            dgvList.ShowEditingIcon = false;
            dgvList.ShowRowErrors = false;
            dgvList.CellDoubleClick += dgvList_CellDoubleClick;
            dgvList.CellMouseClick += dgvList_CellMouseClick;
            dgvList.DragDrop += dgvList_DragDrop;
            dgvList.DragEnter += dgvList_DragEnter;
            dgvList.DragOver += dgvList_DragOver;
            // 
            // clmKey
            // 
            resources.ApplyResources(clmKey, "clmKey");
            clmKey.Name = "clmKey";
            clmKey.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSongNo
            // 
            resources.ApplyResources(clmSongNo, "clmSongNo");
            clmSongNo.Name = "clmSongNo";
            // 
            // clmZipFileName
            // 
            resources.ApplyResources(clmZipFileName, "clmZipFileName");
            clmZipFileName.Name = "clmZipFileName";
            // 
            // clmFileName
            // 
            resources.ApplyResources(clmFileName, "clmFileName");
            clmFileName.Name = "clmFileName";
            clmFileName.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmPlayingNow
            // 
            clmPlayingNow.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(clmPlayingNow, "clmPlayingNow");
            clmPlayingNow.Name = "clmPlayingNow";
            clmPlayingNow.ReadOnly = true;
            clmPlayingNow.Resizable = DataGridViewTriState.False;
            clmPlayingNow.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmEXT
            // 
            resources.ApplyResources(clmEXT, "clmEXT");
            clmEXT.Name = "clmEXT";
            clmEXT.ReadOnly = true;
            // 
            // clmType
            // 
            resources.ApplyResources(clmType, "clmType");
            clmType.Name = "clmType";
            clmType.ReadOnly = true;
            // 
            // clmTitle
            // 
            resources.ApplyResources(clmTitle, "clmTitle");
            clmTitle.Name = "clmTitle";
            clmTitle.ReadOnly = true;
            // 
            // clmTitleJ
            // 
            resources.ApplyResources(clmTitleJ, "clmTitleJ");
            clmTitleJ.Name = "clmTitleJ";
            clmTitleJ.ReadOnly = true;
            // 
            // clmDispFileName
            // 
            resources.ApplyResources(clmDispFileName, "clmDispFileName");
            clmDispFileName.Name = "clmDispFileName";
            clmDispFileName.ReadOnly = true;
            // 
            // clmGame
            // 
            resources.ApplyResources(clmGame, "clmGame");
            clmGame.Name = "clmGame";
            clmGame.ReadOnly = true;
            // 
            // clmGameJ
            // 
            resources.ApplyResources(clmGameJ, "clmGameJ");
            clmGameJ.Name = "clmGameJ";
            clmGameJ.ReadOnly = true;
            // 
            // clmComposer
            // 
            resources.ApplyResources(clmComposer, "clmComposer");
            clmComposer.Name = "clmComposer";
            clmComposer.ReadOnly = true;
            // 
            // clmComposerJ
            // 
            resources.ApplyResources(clmComposerJ, "clmComposerJ");
            clmComposerJ.Name = "clmComposerJ";
            clmComposerJ.ReadOnly = true;
            // 
            // clmVGMby
            // 
            resources.ApplyResources(clmVGMby, "clmVGMby");
            clmVGMby.Name = "clmVGMby";
            clmVGMby.ReadOnly = true;
            // 
            // clmConverted
            // 
            resources.ApplyResources(clmConverted, "clmConverted");
            clmConverted.Name = "clmConverted";
            clmConverted.ReadOnly = true;
            // 
            // clmNotes
            // 
            resources.ApplyResources(clmNotes, "clmNotes");
            clmNotes.Name = "clmNotes";
            clmNotes.ReadOnly = true;
            // 
            // clmDuration
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            clmDuration.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(clmDuration, "clmDuration");
            clmDuration.Name = "clmDuration";
            clmDuration.ReadOnly = true;
            // 
            // ClmVersion
            // 
            resources.ApplyResources(ClmVersion, "ClmVersion");
            ClmVersion.Name = "ClmVersion";
            ClmVersion.ReadOnly = true;
            ClmVersion.SortMode = DataGridViewColumnSortMode.Programmatic;
            // 
            // ClmUseChips
            // 
            resources.ApplyResources(ClmUseChips, "ClmUseChips");
            ClmUseChips.Name = "ClmUseChips";
            ClmUseChips.ReadOnly = true;
            // 
            // clmSpacer
            // 
            clmSpacer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(clmSpacer, "clmSpacer");
            clmSpacer.Name = "clmSpacer";
            clmSpacer.ReadOnly = true;
            clmSpacer.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(toolStrip1, "toolStrip1");
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbOpenPlayList, tsbSavePlayList, toolStripSeparator1, tsbAddMusic, tsbAddFolder, toolStripSeparator2, tsbUp, tsbDown, toolStripSeparator4, tsbJapanese, toolStripSeparator6, tsbTextExt, tsbMMLExt, tsbImgExt });
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Stretch = true;
            // 
            // tsbOpenPlayList
            // 
            tsbOpenPlayList.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbOpenPlayList.Image = Resources.ccOpenFolder;
            resources.ApplyResources(tsbOpenPlayList, "tsbOpenPlayList");
            tsbOpenPlayList.Name = "tsbOpenPlayList";
            tsbOpenPlayList.Click += tsbOpenPlayList_Click;
            // 
            // tsbSavePlayList
            // 
            tsbSavePlayList.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbSavePlayList.Image = Resources.savePL;
            resources.ApplyResources(tsbSavePlayList, "tsbSavePlayList");
            tsbSavePlayList.Name = "tsbSavePlayList";
            tsbSavePlayList.Click += tsbSavePlayList_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsbAddMusic
            // 
            tsbAddMusic.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbAddMusic.Image = Resources.addPL;
            resources.ApplyResources(tsbAddMusic, "tsbAddMusic");
            tsbAddMusic.Name = "tsbAddMusic";
            tsbAddMusic.Click += tsbAddMusic_Click;
            // 
            // tsbAddFolder
            // 
            tsbAddFolder.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbAddFolder.Image = Resources.addFolderPL;
            resources.ApplyResources(tsbAddFolder, "tsbAddFolder");
            tsbAddFolder.Name = "tsbAddFolder";
            tsbAddFolder.Click += tsbAddFolder_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // tsbUp
            // 
            tsbUp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbUp.Image = Resources.upPL;
            resources.ApplyResources(tsbUp, "tsbUp");
            tsbUp.Name = "tsbUp";
            tsbUp.Click += tsbUp_Click;
            // 
            // tsbDown
            // 
            tsbDown.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDown.Image = Resources.downPL;
            resources.ApplyResources(tsbDown, "tsbDown");
            tsbDown.Name = "tsbDown";
            tsbDown.Click += tsbDown_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            // 
            // tsbJapanese
            // 
            tsbJapanese.CheckOnClick = true;
            tsbJapanese.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbJapanese.Image = Resources.japPL;
            resources.ApplyResources(tsbJapanese, "tsbJapanese");
            tsbJapanese.Name = "tsbJapanese";
            tsbJapanese.Click += toolStripButton1_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
            // 
            // tsbTextExt
            // 
            tsbTextExt.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(tsbTextExt, "tsbTextExt");
            tsbTextExt.Image = Resources.txtPL;
            tsbTextExt.Name = "tsbTextExt";
            tsbTextExt.Click += tsbTextExt_Click;
            // 
            // tsbMMLExt
            // 
            tsbMMLExt.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(tsbMMLExt, "tsbMMLExt");
            tsbMMLExt.Image = Resources.mmlPL;
            tsbMMLExt.Name = "tsbMMLExt";
            tsbMMLExt.Click += tsbMMLExt_Click;
            // 
            // tsbImgExt
            // 
            tsbImgExt.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(tsbImgExt, "tsbImgExt");
            tsbImgExt.Image = Resources.imgPL;
            tsbImgExt.Name = "tsbImgExt";
            tsbImgExt.Click += tsbImgExt_Click;
            // 
            // cmsPlayList
            // 
            cmsPlayList.Items.AddRange(new ToolStripItem[] { type設定ToolStripMenuItem, toolStripSeparator5, tsmiPlayThis, tsmiDelThis, toolStripSeparator3, tsmiDelAllMusic, tsmiOpenFolder });
            cmsPlayList.Name = "cmsPlayList";
            resources.ApplyResources(cmsPlayList, "cmsPlayList");
            // 
            // type設定ToolStripMenuItem
            // 
            type設定ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiA, tsmiB, tsmiC, tsmiD, tsmiE, tsmiF, tsmiG, tsmiH, tsmiI, tsmiJ });
            type設定ToolStripMenuItem.Name = "type設定ToolStripMenuItem";
            resources.ApplyResources(type設定ToolStripMenuItem, "type設定ToolStripMenuItem");
            // 
            // tsmiA
            // 
            tsmiA.Name = "tsmiA";
            resources.ApplyResources(tsmiA, "tsmiA");
            tsmiA.Click += tsmiA_Click;
            // 
            // tsmiB
            // 
            tsmiB.Name = "tsmiB";
            resources.ApplyResources(tsmiB, "tsmiB");
            tsmiB.Click += tsmiA_Click;
            // 
            // tsmiC
            // 
            tsmiC.Name = "tsmiC";
            resources.ApplyResources(tsmiC, "tsmiC");
            tsmiC.Click += tsmiA_Click;
            // 
            // tsmiD
            // 
            tsmiD.Name = "tsmiD";
            resources.ApplyResources(tsmiD, "tsmiD");
            tsmiD.Click += tsmiA_Click;
            // 
            // tsmiE
            // 
            tsmiE.Name = "tsmiE";
            resources.ApplyResources(tsmiE, "tsmiE");
            tsmiE.Click += tsmiA_Click;
            // 
            // tsmiF
            // 
            tsmiF.Name = "tsmiF";
            resources.ApplyResources(tsmiF, "tsmiF");
            tsmiF.Click += tsmiA_Click;
            // 
            // tsmiG
            // 
            tsmiG.Name = "tsmiG";
            resources.ApplyResources(tsmiG, "tsmiG");
            tsmiG.Click += tsmiA_Click;
            // 
            // tsmiH
            // 
            tsmiH.Name = "tsmiH";
            resources.ApplyResources(tsmiH, "tsmiH");
            tsmiH.Click += tsmiA_Click;
            // 
            // tsmiI
            // 
            tsmiI.Name = "tsmiI";
            resources.ApplyResources(tsmiI, "tsmiI");
            tsmiI.Click += tsmiA_Click;
            // 
            // tsmiJ
            // 
            tsmiJ.Name = "tsmiJ";
            resources.ApplyResources(tsmiJ, "tsmiJ");
            tsmiJ.Click += tsmiA_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
            // 
            // tsmiPlayThis
            // 
            tsmiPlayThis.Name = "tsmiPlayThis";
            resources.ApplyResources(tsmiPlayThis, "tsmiPlayThis");
            tsmiPlayThis.Click += tsmiPlayThis_Click;
            // 
            // tsmiDelThis
            // 
            tsmiDelThis.Name = "tsmiDelThis";
            resources.ApplyResources(tsmiDelThis, "tsmiDelThis");
            tsmiDelThis.Click += tsmiDelThis_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // tsmiDelAllMusic
            // 
            tsmiDelAllMusic.Name = "tsmiDelAllMusic";
            resources.ApplyResources(tsmiDelAllMusic, "tsmiDelAllMusic");
            tsmiDelAllMusic.Click += tsmiDelAllMusic_Click;
            // 
            // tsmiOpenFolder
            // 
            tsmiOpenFolder.Name = "tsmiOpenFolder";
            resources.ApplyResources(tsmiOpenFolder, "tsmiOpenFolder");
            tsmiOpenFolder.Click += tsmiOpenFolder_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // frmPlayList
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(toolStripContainer1);
            KeyPreview = true;
            Name = "frmPlayList";
            Opacity = 0D;
            FormClosing += frmPlayList_FormClosing;
            Load += frmPlayList_Load;
            Shown += frmPlayList_Shown;
            KeyDown += frmPlayList_KeyDown;
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvList).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            cmsPlayList.ResumeLayout(false);
            ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton tsbTextExt;
        private System.Windows.Forms.ToolStripButton tsbMMLExt;
        private System.Windows.Forms.ToolStripButton tsbImgExt;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFolder;
        private DataGridViewTextBoxColumn clmKey;
        private DataGridViewTextBoxColumn clmSongNo;
        private DataGridViewTextBoxColumn clmZipFileName;
        private DataGridViewTextBoxColumn clmFileName;
        private DataGridViewTextBoxColumn clmPlayingNow;
        private DataGridViewTextBoxColumn clmEXT;
        private DataGridViewTextBoxColumn clmType;
        private DataGridViewTextBoxColumn clmTitle;
        private DataGridViewTextBoxColumn clmTitleJ;
        private DataGridViewTextBoxColumn clmDispFileName;
        private DataGridViewTextBoxColumn clmGame;
        private DataGridViewTextBoxColumn clmGameJ;
        private DataGridViewTextBoxColumn clmComposer;
        private DataGridViewTextBoxColumn clmComposerJ;
        private DataGridViewTextBoxColumn clmVGMby;
        private DataGridViewTextBoxColumn clmConverted;
        private DataGridViewTextBoxColumn clmNotes;
        private DataGridViewTextBoxColumn clmDuration;
        private DataGridViewTextBoxColumn ClmVersion;
        private DataGridViewTextBoxColumn ClmUseChips;
        private DataGridViewTextBoxColumn clmSpacer;
    }
}