#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
namespace MDPlayer.form
{
    partial class frmMain
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            pbScreen = new PictureBox();
            cmsOpenOtherPanel = new ContextMenuStrip(components);
            primaryToolStripMenuItem = new ToolStripMenuItem();
            tsmiCPPSG = new ToolStripMenuItem();
            tsmiPAY8910 = new ToolStripMenuItem();
            tsmiPDCSG = new ToolStripMenuItem();
            tsmiCPWF = new ToolStripMenuItem();
            tsmiPHuC6280 = new ToolStripMenuItem();
            tsmiPK051649 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            tsmiCPOPL = new ToolStripMenuItem();
            tsmiPOPLL = new ToolStripMenuItem();
            tsmiPOPL = new ToolStripMenuItem();
            tsmiPY8950 = new ToolStripMenuItem();
            tsmiPOPL2 = new ToolStripMenuItem();
            tsmiPOPL3 = new ToolStripMenuItem();
            tsmiPOPL4 = new ToolStripMenuItem();
            tsmiCPOPN = new ToolStripMenuItem();
            tsmiPOPN = new ToolStripMenuItem();
            tsmiPOPN2 = new ToolStripMenuItem();
            tsmiPOPNA = new ToolStripMenuItem();
            tsmiPOPNB = new ToolStripMenuItem();
            tsmiPOPNA2 = new ToolStripMenuItem();
            tsmiPOPM = new ToolStripMenuItem();
            tsmiPOPX = new ToolStripMenuItem();
            tsmiYMZ280B = new ToolStripMenuItem();
            tsmiCPPCM = new ToolStripMenuItem();
            tsmiPC140 = new ToolStripMenuItem();
            tsmiPC352 = new ToolStripMenuItem();
            tsmiPOKIM6258 = new ToolStripMenuItem();
            tsmiPOKIM6295 = new ToolStripMenuItem();
            tsmiPPWM = new ToolStripMenuItem();
            tsmiPQSound = new ToolStripMenuItem();
            tsmiPRF5C164 = new ToolStripMenuItem();
            tsmiPRF5C68 = new ToolStripMenuItem();
            tsmiPMultiPCM = new ToolStripMenuItem();
            tsmiPPPZ8 = new ToolStripMenuItem();
            tsmiPSegaPCM = new ToolStripMenuItem();
            tsmiPMIDI = new ToolStripMenuItem();
            tsmiCPNES = new ToolStripMenuItem();
            tsmiPNESDMC = new ToolStripMenuItem();
            tsmiPFDS = new ToolStripMenuItem();
            tsmiPMMC5 = new ToolStripMenuItem();
            tsmiPVRC6 = new ToolStripMenuItem();
            tsmiPVRC7 = new ToolStripMenuItem();
            tsmiPN106 = new ToolStripMenuItem();
            tsmiPS5B = new ToolStripMenuItem();
            tsmiPDMG = new ToolStripMenuItem();
            sencondryToolStripMenuItem = new ToolStripMenuItem();
            tsmiCSPSG = new ToolStripMenuItem();
            tsmiSAY8910 = new ToolStripMenuItem();
            tsmiSDCSG = new ToolStripMenuItem();
            tsmiCSWF = new ToolStripMenuItem();
            tsmiSHuC6280 = new ToolStripMenuItem();
            tsmiSK051649 = new ToolStripMenuItem();
            tsmiCSOPL = new ToolStripMenuItem();
            tsmiSOPLL = new ToolStripMenuItem();
            tsmiSOPL = new ToolStripMenuItem();
            tsmiSY8950 = new ToolStripMenuItem();
            tsmiSOPL2 = new ToolStripMenuItem();
            tsmiSOPL3 = new ToolStripMenuItem();
            tsmiSOPL4 = new ToolStripMenuItem();
            tsmiCSOPN = new ToolStripMenuItem();
            tsmiSOPN = new ToolStripMenuItem();
            tsmiSOPN2 = new ToolStripMenuItem();
            tsmiSOPNA = new ToolStripMenuItem();
            tsmiSOPNB = new ToolStripMenuItem();
            tsmiSOPNA2 = new ToolStripMenuItem();
            tsmiSOPM = new ToolStripMenuItem();
            tsmiSOPX = new ToolStripMenuItem();
            tsmiSYMZ280B = new ToolStripMenuItem();
            tsmiCSPCM = new ToolStripMenuItem();
            tsmiSC140 = new ToolStripMenuItem();
            tsmiSC352 = new ToolStripMenuItem();
            tsmiSOKIM6258 = new ToolStripMenuItem();
            tsmiSOKIM6295 = new ToolStripMenuItem();
            tsmiSPWM = new ToolStripMenuItem();
            tsmiSRF5C164 = new ToolStripMenuItem();
            tsmiSRF5C68 = new ToolStripMenuItem();
            tsmiSSegaPCM = new ToolStripMenuItem();
            tsmiSMultiPCM = new ToolStripMenuItem();
            tsmiSPPZ8 = new ToolStripMenuItem();
            tsmiSMIDI = new ToolStripMenuItem();
            tsmiCSNES = new ToolStripMenuItem();
            tsmiSFDS = new ToolStripMenuItem();
            tsmiSMMC5 = new ToolStripMenuItem();
            tsmiSNESDMC = new ToolStripMenuItem();
            tsmiSVRC6 = new ToolStripMenuItem();
            tsmiSVRC7 = new ToolStripMenuItem();
            tsmiSN106 = new ToolStripMenuItem();
            tsmiSS5B = new ToolStripMenuItem();
            tsmiSDMG = new ToolStripMenuItem();
            cmsMenu = new ContextMenuStrip(components);
            ファイルToolStripMenuItem = new ToolStripMenuItem();
            tsmiOpenFile = new ToolStripMenuItem();
            tsmiExit = new ToolStripMenuItem();
            操作ToolStripMenuItem = new ToolStripMenuItem();
            tsmiPlay = new ToolStripMenuItem();
            tsmiStop = new ToolStripMenuItem();
            tsmiPause = new ToolStripMenuItem();
            tsmiFadeOut = new ToolStripMenuItem();
            tsmiSlow = new ToolStripMenuItem();
            tsmiFf = new ToolStripMenuItem();
            tsmiNext = new ToolStripMenuItem();
            tsmiPlayMode = new ToolStripMenuItem();
            tsmiOption = new ToolStripMenuItem();
            tsmiPlayList = new ToolStripMenuItem();
            tsmiOpenInfo = new ToolStripMenuItem();
            tsmiOpenMixer = new ToolStripMenuItem();
            その他ウィンドウ表示ToolStripMenuItem = new ToolStripMenuItem();
            tsmiKBrd = new ToolStripMenuItem();
            tsmiVST = new ToolStripMenuItem();
            tsmiMIDIkbd = new ToolStripMenuItem();
            tsmiChangeZoom = new ToolStripMenuItem();
            tsmiChangeZoomX1 = new ToolStripMenuItem();
            tsmiChangeZoomX2 = new ToolStripMenuItem();
            tsmiChangeZoomX3 = new ToolStripMenuItem();
            tsmiChangeZoomX4 = new ToolStripMenuItem();
            レジスタダンプ表示ToolStripMenuItem = new ToolStripMenuItem();
            yM2612ToolStripMenuItem = new ToolStripMenuItem();
            ym2151ToolStripMenuItem = new ToolStripMenuItem();
            ym2203ToolStripMenuItem = new ToolStripMenuItem();
            ym2413ToolStripMenuItem = new ToolStripMenuItem();
            ym2608ToolStripMenuItem = new ToolStripMenuItem();
            yM2610ToolStripMenuItem = new ToolStripMenuItem();
            yM3812ToolStripMenuItem = new ToolStripMenuItem();
            yMF262ToolStripMenuItem = new ToolStripMenuItem();
            yMF278BToolStripMenuItem = new ToolStripMenuItem();
            yMZ280BToolStripMenuItem = new ToolStripMenuItem();
            c140ToolStripMenuItem = new ToolStripMenuItem();
            c352ToolStripMenuItem = new ToolStripMenuItem();
            qSoundToolStripMenuItem = new ToolStripMenuItem();
            segaPCMToolStripMenuItem = new ToolStripMenuItem();
            sN76489ToolStripMenuItem = new ToolStripMenuItem();
            aY8910ToolStripMenuItem = new ToolStripMenuItem();
            sIDToolStripMenuItem = new ToolStripMenuItem();
            tsmiVisualizer = new ToolStripMenuItem();
            tsmiOutputwavFile = new ToolStripMenuItem();
            opeButtonSetting = new Button();
            toolTip1 = new ToolTip(components);
            opeButtonStop = new Button();
            opeButtonPause = new Button();
            opeButtonFadeout = new Button();
            opeButtonPrevious = new Button();
            opeButtonSlow = new Button();
            opeButtonPlay = new Button();
            opeButtonFast = new Button();
            opeButtonNext = new Button();
            opeButtonZoom = new Button();
            opeButtonMIDIKBD = new Button();
            opeButtonVST = new Button();
            opeButtonKBD = new Button();
            opeButtonMixer = new Button();
            opeButtonInformation = new Button();
            opeButtonPlayList = new Button();
            opeButtonOpen = new Button();
            opeButtonMode = new Button();
            keyboardHook1 = new HongliangSoft.Utilities.Gui.KeyboardHook();
            tsmiPK053260 = new ToolStripMenuItem();
            tsmiSK053260 = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)pbScreen).BeginInit();
            cmsOpenOtherPanel.SuspendLayout();
            cmsMenu.SuspendLayout();
            SuspendLayout();
            // 
            // pbScreen
            // 
            pbScreen.BackColor = Color.Black;
            resources.ApplyResources(pbScreen, "pbScreen");
            pbScreen.Image = Resources.planeControl;
            pbScreen.Name = "pbScreen";
            pbScreen.TabStop = false;
            pbScreen.DragDrop += pbScreen_DragDrop;
            pbScreen.DragEnter += pbScreen_DragEnter;
            pbScreen.MouseClick += pbScreen_MouseClick;
            pbScreen.MouseLeave += pbScreen_MouseLeave;
            pbScreen.MouseMove += pbScreen_MouseMove;
            // 
            // cmsOpenOtherPanel
            // 
            cmsOpenOtherPanel.ImageScalingSize = new Size(20, 20);
            cmsOpenOtherPanel.Items.AddRange(new ToolStripItem[] { primaryToolStripMenuItem, sencondryToolStripMenuItem });
            cmsOpenOtherPanel.Name = "cmsOpenOtherPanel";
            resources.ApplyResources(cmsOpenOtherPanel, "cmsOpenOtherPanel");
            // 
            // primaryToolStripMenuItem
            // 
            primaryToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiCPPSG, tsmiCPWF, tsmiCPOPL, tsmiCPOPN, tsmiPOPM, tsmiPOPX, tsmiYMZ280B, tsmiCPPCM, tsmiPMIDI, tsmiCPNES, tsmiPDMG });
            primaryToolStripMenuItem.Name = "primaryToolStripMenuItem";
            resources.ApplyResources(primaryToolStripMenuItem, "primaryToolStripMenuItem");
            // 
            // tsmiCPPSG
            // 
            tsmiCPPSG.DropDownItems.AddRange(new ToolStripItem[] { tsmiPAY8910, tsmiPDCSG });
            tsmiCPPSG.Name = "tsmiCPPSG";
            resources.ApplyResources(tsmiCPPSG, "tsmiCPPSG");
            // 
            // tsmiPAY8910
            // 
            tsmiPAY8910.Name = "tsmiPAY8910";
            resources.ApplyResources(tsmiPAY8910, "tsmiPAY8910");
            tsmiPAY8910.Click += tsmiPAY8910_Click;
            // 
            // tsmiPDCSG
            // 
            tsmiPDCSG.Name = "tsmiPDCSG";
            resources.ApplyResources(tsmiPDCSG, "tsmiPDCSG");
            tsmiPDCSG.Click += tsmiPDCSG_Click;
            // 
            // tsmiCPWF
            // 
            tsmiCPWF.DropDownItems.AddRange(new ToolStripItem[] { tsmiPHuC6280, tsmiPK051649, toolStripMenuItem2 });
            tsmiCPWF.Name = "tsmiCPWF";
            resources.ApplyResources(tsmiCPWF, "tsmiCPWF");
            // 
            // tsmiPHuC6280
            // 
            tsmiPHuC6280.Name = "tsmiPHuC6280";
            resources.ApplyResources(tsmiPHuC6280, "tsmiPHuC6280");
            tsmiPHuC6280.Click += tsmiPHuC6280_Click;
            // 
            // tsmiPK051649
            // 
            tsmiPK051649.Name = "tsmiPK051649";
            resources.ApplyResources(tsmiPK051649, "tsmiPK051649");
            tsmiPK051649.Click += tsmiPK051649_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // tsmiCPOPL
            // 
            tsmiCPOPL.DropDownItems.AddRange(new ToolStripItem[] { tsmiPOPLL, tsmiPOPL, tsmiPY8950, tsmiPOPL2, tsmiPOPL3, tsmiPOPL4 });
            tsmiCPOPL.Name = "tsmiCPOPL";
            resources.ApplyResources(tsmiCPOPL, "tsmiCPOPL");
            // 
            // tsmiPOPLL
            // 
            tsmiPOPLL.Name = "tsmiPOPLL";
            resources.ApplyResources(tsmiPOPLL, "tsmiPOPLL");
            tsmiPOPLL.Click += tsmiPOPLL_Click;
            // 
            // tsmiPOPL
            // 
            tsmiPOPL.Name = "tsmiPOPL";
            resources.ApplyResources(tsmiPOPL, "tsmiPOPL");
            tsmiPOPL.Click += tsmiPOPL_Click;
            // 
            // tsmiPY8950
            // 
            tsmiPY8950.Name = "tsmiPY8950";
            resources.ApplyResources(tsmiPY8950, "tsmiPY8950");
            tsmiPY8950.Click += tsmiPY8950_Click;
            // 
            // tsmiPOPL2
            // 
            tsmiPOPL2.Name = "tsmiPOPL2";
            resources.ApplyResources(tsmiPOPL2, "tsmiPOPL2");
            tsmiPOPL2.Click += tsmiPOPL2_Click;
            // 
            // tsmiPOPL3
            // 
            tsmiPOPL3.Name = "tsmiPOPL3";
            resources.ApplyResources(tsmiPOPL3, "tsmiPOPL3");
            tsmiPOPL3.Click += tsmiPOPL3_Click;
            // 
            // tsmiPOPL4
            // 
            tsmiPOPL4.Name = "tsmiPOPL4";
            resources.ApplyResources(tsmiPOPL4, "tsmiPOPL4");
            tsmiPOPL4.Click += tsmiPOPL4_Click;
            // 
            // tsmiCPOPN
            // 
            tsmiCPOPN.DropDownItems.AddRange(new ToolStripItem[] { tsmiPOPN, tsmiPOPN2, tsmiPOPNA, tsmiPOPNB, tsmiPOPNA2 });
            tsmiCPOPN.Name = "tsmiCPOPN";
            resources.ApplyResources(tsmiCPOPN, "tsmiCPOPN");
            // 
            // tsmiPOPN
            // 
            tsmiPOPN.Name = "tsmiPOPN";
            resources.ApplyResources(tsmiPOPN, "tsmiPOPN");
            tsmiPOPN.Click += tsmiPOPN_Click;
            // 
            // tsmiPOPN2
            // 
            tsmiPOPN2.Name = "tsmiPOPN2";
            resources.ApplyResources(tsmiPOPN2, "tsmiPOPN2");
            tsmiPOPN2.Click += tsmiPOPN2_Click;
            // 
            // tsmiPOPNA
            // 
            tsmiPOPNA.Name = "tsmiPOPNA";
            resources.ApplyResources(tsmiPOPNA, "tsmiPOPNA");
            tsmiPOPNA.Click += tsmiPOPNA_Click;
            // 
            // tsmiPOPNB
            // 
            tsmiPOPNB.Name = "tsmiPOPNB";
            resources.ApplyResources(tsmiPOPNB, "tsmiPOPNB");
            tsmiPOPNB.Click += tsmiPOPNB_Click;
            // 
            // tsmiPOPNA2
            // 
            tsmiPOPNA2.Name = "tsmiPOPNA2";
            resources.ApplyResources(tsmiPOPNA2, "tsmiPOPNA2");
            tsmiPOPNA2.Click += tsmiPOPNA2_Click;
            // 
            // tsmiPOPM
            // 
            tsmiPOPM.Name = "tsmiPOPM";
            resources.ApplyResources(tsmiPOPM, "tsmiPOPM");
            tsmiPOPM.Click += tsmiPOPM_Click;
            // 
            // tsmiPOPX
            // 
            tsmiPOPX.Name = "tsmiPOPX";
            resources.ApplyResources(tsmiPOPX, "tsmiPOPX");
            tsmiPOPX.Click += tsmiPOPX_Click;
            // 
            // tsmiYMZ280B
            // 
            tsmiYMZ280B.Name = "tsmiYMZ280B";
            resources.ApplyResources(tsmiYMZ280B, "tsmiYMZ280B");
            tsmiYMZ280B.Click += tsmiYMZ280B_Click;
            // 
            // tsmiCPPCM
            // 
            tsmiCPPCM.DropDownItems.AddRange(new ToolStripItem[] { tsmiPC140, tsmiPC352, tsmiPOKIM6258, tsmiPOKIM6295, tsmiPPWM, tsmiPQSound, tsmiPRF5C164, tsmiPRF5C68, tsmiPMultiPCM, tsmiPPPZ8, tsmiPSegaPCM, tsmiPK053260 });
            tsmiCPPCM.Name = "tsmiCPPCM";
            resources.ApplyResources(tsmiCPPCM, "tsmiCPPCM");
            // 
            // tsmiPC140
            // 
            tsmiPC140.Name = "tsmiPC140";
            resources.ApplyResources(tsmiPC140, "tsmiPC140");
            tsmiPC140.Click += tsmiPC140_Click;
            // 
            // tsmiPC352
            // 
            tsmiPC352.Name = "tsmiPC352";
            resources.ApplyResources(tsmiPC352, "tsmiPC352");
            tsmiPC352.Click += tsmiPC352_Click;
            // 
            // tsmiPOKIM6258
            // 
            tsmiPOKIM6258.Name = "tsmiPOKIM6258";
            resources.ApplyResources(tsmiPOKIM6258, "tsmiPOKIM6258");
            tsmiPOKIM6258.Click += tsmiPOKIM6258_Click;
            // 
            // tsmiPOKIM6295
            // 
            tsmiPOKIM6295.Name = "tsmiPOKIM6295";
            resources.ApplyResources(tsmiPOKIM6295, "tsmiPOKIM6295");
            tsmiPOKIM6295.Click += tsmiPOKIM6295_Click;
            // 
            // tsmiPPWM
            // 
            tsmiPPWM.Name = "tsmiPPWM";
            resources.ApplyResources(tsmiPPWM, "tsmiPPWM");
            tsmiPPWM.Click += tsmiPPWM_Click;
            // 
            // tsmiPQSound
            // 
            tsmiPQSound.Name = "tsmiPQSound";
            resources.ApplyResources(tsmiPQSound, "tsmiPQSound");
            tsmiPQSound.Click += tsmiPQSound_Click;
            // 
            // tsmiPRF5C164
            // 
            tsmiPRF5C164.Name = "tsmiPRF5C164";
            resources.ApplyResources(tsmiPRF5C164, "tsmiPRF5C164");
            tsmiPRF5C164.Click += tsmiPRF5C164_Click;
            // 
            // tsmiPRF5C68
            // 
            tsmiPRF5C68.Name = "tsmiPRF5C68";
            resources.ApplyResources(tsmiPRF5C68, "tsmiPRF5C68");
            tsmiPRF5C68.Click += tsmiPRF5C68_Click;
            // 
            // tsmiPMultiPCM
            // 
            tsmiPMultiPCM.Name = "tsmiPMultiPCM";
            resources.ApplyResources(tsmiPMultiPCM, "tsmiPMultiPCM");
            tsmiPMultiPCM.Click += tsmiPMultiPCM_Click;
            // 
            // tsmiPPPZ8
            // 
            tsmiPPPZ8.Name = "tsmiPPPZ8";
            resources.ApplyResources(tsmiPPPZ8, "tsmiPPPZ8");
            tsmiPPPZ8.Click += tsmiPPPZ8_Click;
            // 
            // tsmiPSegaPCM
            // 
            tsmiPSegaPCM.Name = "tsmiPSegaPCM";
            resources.ApplyResources(tsmiPSegaPCM, "tsmiPSegaPCM");
            tsmiPSegaPCM.Click += tsmiPSegaPCM_Click;
            // 
            // tsmiPMIDI
            // 
            tsmiPMIDI.Name = "tsmiPMIDI";
            resources.ApplyResources(tsmiPMIDI, "tsmiPMIDI");
            tsmiPMIDI.Click += tsmiPMIDI_Click;
            // 
            // tsmiCPNES
            // 
            tsmiCPNES.DropDownItems.AddRange(new ToolStripItem[] { tsmiPNESDMC, tsmiPFDS, tsmiPMMC5, tsmiPVRC6, tsmiPVRC7, tsmiPN106, tsmiPS5B });
            tsmiCPNES.Name = "tsmiCPNES";
            resources.ApplyResources(tsmiCPNES, "tsmiCPNES");
            // 
            // tsmiPNESDMC
            // 
            tsmiPNESDMC.Name = "tsmiPNESDMC";
            resources.ApplyResources(tsmiPNESDMC, "tsmiPNESDMC");
            tsmiPNESDMC.Click += tsmiPNESDMC_Click;
            // 
            // tsmiPFDS
            // 
            tsmiPFDS.Name = "tsmiPFDS";
            resources.ApplyResources(tsmiPFDS, "tsmiPFDS");
            tsmiPFDS.Click += tsmiPFDS_Click;
            // 
            // tsmiPMMC5
            // 
            tsmiPMMC5.Name = "tsmiPMMC5";
            resources.ApplyResources(tsmiPMMC5, "tsmiPMMC5");
            tsmiPMMC5.Click += tsmiPMMC5_Click;
            // 
            // tsmiPVRC6
            // 
            tsmiPVRC6.Name = "tsmiPVRC6";
            resources.ApplyResources(tsmiPVRC6, "tsmiPVRC6");
            tsmiPVRC6.Click += tsmiPVRC6_Click;
            // 
            // tsmiPVRC7
            // 
            tsmiPVRC7.Name = "tsmiPVRC7";
            resources.ApplyResources(tsmiPVRC7, "tsmiPVRC7");
            tsmiPVRC7.Click += tsmiPVRC7_Click;
            // 
            // tsmiPN106
            // 
            tsmiPN106.Name = "tsmiPN106";
            resources.ApplyResources(tsmiPN106, "tsmiPN106");
            tsmiPN106.Click += tsmiPN106_Click;
            // 
            // tsmiPS5B
            // 
            tsmiPS5B.Name = "tsmiPS5B";
            resources.ApplyResources(tsmiPS5B, "tsmiPS5B");
            tsmiPS5B.Click += tsmiPS5B_Click;
            // 
            // tsmiPDMG
            // 
            tsmiPDMG.Name = "tsmiPDMG";
            resources.ApplyResources(tsmiPDMG, "tsmiPDMG");
            tsmiPDMG.Click += tsmiPDMG_Click;
            // 
            // sencondryToolStripMenuItem
            // 
            sencondryToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiCSPSG, tsmiCSWF, tsmiCSOPL, tsmiCSOPN, tsmiSOPM, tsmiSOPX, tsmiSYMZ280B, tsmiCSPCM, tsmiSMIDI, tsmiCSNES, tsmiSDMG });
            sencondryToolStripMenuItem.Name = "sencondryToolStripMenuItem";
            resources.ApplyResources(sencondryToolStripMenuItem, "sencondryToolStripMenuItem");
            // 
            // tsmiCSPSG
            // 
            tsmiCSPSG.DropDownItems.AddRange(new ToolStripItem[] { tsmiSAY8910, tsmiSDCSG });
            tsmiCSPSG.Name = "tsmiCSPSG";
            resources.ApplyResources(tsmiCSPSG, "tsmiCSPSG");
            // 
            // tsmiSAY8910
            // 
            tsmiSAY8910.Name = "tsmiSAY8910";
            resources.ApplyResources(tsmiSAY8910, "tsmiSAY8910");
            tsmiSAY8910.Click += tsmiSAY8910_Click;
            // 
            // tsmiSDCSG
            // 
            tsmiSDCSG.Name = "tsmiSDCSG";
            resources.ApplyResources(tsmiSDCSG, "tsmiSDCSG");
            tsmiSDCSG.Click += tsmiSDCSG_Click;
            // 
            // tsmiCSWF
            // 
            tsmiCSWF.DropDownItems.AddRange(new ToolStripItem[] { tsmiSHuC6280, tsmiSK051649 });
            tsmiCSWF.Name = "tsmiCSWF";
            resources.ApplyResources(tsmiCSWF, "tsmiCSWF");
            // 
            // tsmiSHuC6280
            // 
            tsmiSHuC6280.Name = "tsmiSHuC6280";
            resources.ApplyResources(tsmiSHuC6280, "tsmiSHuC6280");
            tsmiSHuC6280.Click += tsmiSHuC6280_Click;
            // 
            // tsmiSK051649
            // 
            tsmiSK051649.Name = "tsmiSK051649";
            resources.ApplyResources(tsmiSK051649, "tsmiSK051649");
            tsmiSK051649.Click += tsmiSK051649_Click;
            // 
            // tsmiCSOPL
            // 
            tsmiCSOPL.DropDownItems.AddRange(new ToolStripItem[] { tsmiSOPLL, tsmiSOPL, tsmiSY8950, tsmiSOPL2, tsmiSOPL3, tsmiSOPL4 });
            tsmiCSOPL.Name = "tsmiCSOPL";
            resources.ApplyResources(tsmiCSOPL, "tsmiCSOPL");
            // 
            // tsmiSOPLL
            // 
            tsmiSOPLL.Name = "tsmiSOPLL";
            resources.ApplyResources(tsmiSOPLL, "tsmiSOPLL");
            tsmiSOPLL.Click += tsmiSOPLL_Click;
            // 
            // tsmiSOPL
            // 
            tsmiSOPL.Name = "tsmiSOPL";
            resources.ApplyResources(tsmiSOPL, "tsmiSOPL");
            tsmiSOPL.Click += tsmiSOPL_Click;
            // 
            // tsmiSY8950
            // 
            tsmiSY8950.Name = "tsmiSY8950";
            resources.ApplyResources(tsmiSY8950, "tsmiSY8950");
            tsmiSY8950.Click += tsmiSY8950_Click;
            // 
            // tsmiSOPL2
            // 
            tsmiSOPL2.Name = "tsmiSOPL2";
            resources.ApplyResources(tsmiSOPL2, "tsmiSOPL2");
            tsmiSOPL2.Click += tsmiSOPL2_Click;
            // 
            // tsmiSOPL3
            // 
            tsmiSOPL3.Name = "tsmiSOPL3";
            resources.ApplyResources(tsmiSOPL3, "tsmiSOPL3");
            tsmiSOPL3.Click += tsmiSOPL3_Click;
            // 
            // tsmiSOPL4
            // 
            tsmiSOPL4.Name = "tsmiSOPL4";
            resources.ApplyResources(tsmiSOPL4, "tsmiSOPL4");
            tsmiSOPL4.Click += tsmiSOPL4_Click;
            // 
            // tsmiCSOPN
            // 
            tsmiCSOPN.DropDownItems.AddRange(new ToolStripItem[] { tsmiSOPN, tsmiSOPN2, tsmiSOPNA, tsmiSOPNB, tsmiSOPNA2 });
            tsmiCSOPN.Name = "tsmiCSOPN";
            resources.ApplyResources(tsmiCSOPN, "tsmiCSOPN");
            // 
            // tsmiSOPN
            // 
            tsmiSOPN.Name = "tsmiSOPN";
            resources.ApplyResources(tsmiSOPN, "tsmiSOPN");
            tsmiSOPN.Click += tsmiSOPN_Click;
            // 
            // tsmiSOPN2
            // 
            tsmiSOPN2.Name = "tsmiSOPN2";
            resources.ApplyResources(tsmiSOPN2, "tsmiSOPN2");
            tsmiSOPN2.Click += tsmiSOPN2_Click;
            // 
            // tsmiSOPNA
            // 
            tsmiSOPNA.Name = "tsmiSOPNA";
            resources.ApplyResources(tsmiSOPNA, "tsmiSOPNA");
            tsmiSOPNA.Click += tsmiSOPNA_Click;
            // 
            // tsmiSOPNB
            // 
            tsmiSOPNB.Name = "tsmiSOPNB";
            resources.ApplyResources(tsmiSOPNB, "tsmiSOPNB");
            tsmiSOPNB.Click += tsmiSOPNB_Click;
            // 
            // tsmiSOPNA2
            // 
            tsmiSOPNA2.Name = "tsmiSOPNA2";
            resources.ApplyResources(tsmiSOPNA2, "tsmiSOPNA2");
            tsmiSOPNA2.Click += tsmiSOPNA2_Click;
            // 
            // tsmiSOPM
            // 
            tsmiSOPM.Name = "tsmiSOPM";
            resources.ApplyResources(tsmiSOPM, "tsmiSOPM");
            tsmiSOPM.Click += tsmiSOPM_Click;
            // 
            // tsmiSOPX
            // 
            tsmiSOPX.Name = "tsmiSOPX";
            resources.ApplyResources(tsmiSOPX, "tsmiSOPX");
            tsmiSOPX.Click += tsmiSOPX_Click;
            // 
            // tsmiSYMZ280B
            // 
            tsmiSYMZ280B.Name = "tsmiSYMZ280B";
            resources.ApplyResources(tsmiSYMZ280B, "tsmiSYMZ280B");
            tsmiSYMZ280B.Click += tsmiSYMZ280B_Click;
            // 
            // tsmiCSPCM
            // 
            tsmiCSPCM.DropDownItems.AddRange(new ToolStripItem[] { tsmiSC140, tsmiSC352, tsmiSOKIM6258, tsmiSOKIM6295, tsmiSPWM, tsmiSRF5C164, tsmiSRF5C68, tsmiSSegaPCM, tsmiSMultiPCM, tsmiSPPZ8, tsmiSK053260 });
            tsmiCSPCM.Name = "tsmiCSPCM";
            resources.ApplyResources(tsmiCSPCM, "tsmiCSPCM");
            // 
            // tsmiSC140
            // 
            tsmiSC140.Name = "tsmiSC140";
            resources.ApplyResources(tsmiSC140, "tsmiSC140");
            tsmiSC140.Click += tsmiSC140_Click;
            // 
            // tsmiSC352
            // 
            tsmiSC352.Name = "tsmiSC352";
            resources.ApplyResources(tsmiSC352, "tsmiSC352");
            tsmiSC352.Click += tsmiSC352_Click;
            // 
            // tsmiSOKIM6258
            // 
            tsmiSOKIM6258.Name = "tsmiSOKIM6258";
            resources.ApplyResources(tsmiSOKIM6258, "tsmiSOKIM6258");
            tsmiSOKIM6258.Click += tsmiSOKIM6258_Click;
            // 
            // tsmiSOKIM6295
            // 
            tsmiSOKIM6295.Name = "tsmiSOKIM6295";
            resources.ApplyResources(tsmiSOKIM6295, "tsmiSOKIM6295");
            tsmiSOKIM6295.Click += tsmiSOKIM6295_Click;
            // 
            // tsmiSPWM
            // 
            tsmiSPWM.Name = "tsmiSPWM";
            resources.ApplyResources(tsmiSPWM, "tsmiSPWM");
            tsmiSPWM.Click += tsmiSPWM_Click;
            // 
            // tsmiSRF5C164
            // 
            tsmiSRF5C164.Name = "tsmiSRF5C164";
            resources.ApplyResources(tsmiSRF5C164, "tsmiSRF5C164");
            tsmiSRF5C164.Click += tsmiSRF5C164_Click;
            // 
            // tsmiSRF5C68
            // 
            tsmiSRF5C68.Name = "tsmiSRF5C68";
            resources.ApplyResources(tsmiSRF5C68, "tsmiSRF5C68");
            tsmiSRF5C68.Click += tsmiSRF5C68_Click;
            // 
            // tsmiSSegaPCM
            // 
            tsmiSSegaPCM.Name = "tsmiSSegaPCM";
            resources.ApplyResources(tsmiSSegaPCM, "tsmiSSegaPCM");
            tsmiSSegaPCM.Click += tsmiSSegaPCM_Click;
            // 
            // tsmiSMultiPCM
            // 
            tsmiSMultiPCM.Name = "tsmiSMultiPCM";
            resources.ApplyResources(tsmiSMultiPCM, "tsmiSMultiPCM");
            tsmiSMultiPCM.Click += tsmiSMultiPCM_Click;
            // 
            // tsmiSPPZ8
            // 
            tsmiSPPZ8.Name = "tsmiSPPZ8";
            resources.ApplyResources(tsmiSPPZ8, "tsmiSPPZ8");
            tsmiSPPZ8.Click += tsmiSPPZ8_Click;
            // 
            // tsmiSMIDI
            // 
            tsmiSMIDI.Name = "tsmiSMIDI";
            resources.ApplyResources(tsmiSMIDI, "tsmiSMIDI");
            tsmiSMIDI.Click += tsmiSMIDI_Click;
            // 
            // tsmiCSNES
            // 
            tsmiCSNES.DropDownItems.AddRange(new ToolStripItem[] { tsmiSFDS, tsmiSMMC5, tsmiSNESDMC, tsmiSVRC6, tsmiSVRC7, tsmiSN106, tsmiSS5B });
            tsmiCSNES.Name = "tsmiCSNES";
            resources.ApplyResources(tsmiCSNES, "tsmiCSNES");
            // 
            // tsmiSFDS
            // 
            tsmiSFDS.Name = "tsmiSFDS";
            resources.ApplyResources(tsmiSFDS, "tsmiSFDS");
            tsmiSFDS.Click += tsmiSFDS_Click;
            // 
            // tsmiSMMC5
            // 
            tsmiSMMC5.Name = "tsmiSMMC5";
            resources.ApplyResources(tsmiSMMC5, "tsmiSMMC5");
            tsmiSMMC5.Click += tsmiSMMC5_Click;
            // 
            // tsmiSNESDMC
            // 
            tsmiSNESDMC.Name = "tsmiSNESDMC";
            resources.ApplyResources(tsmiSNESDMC, "tsmiSNESDMC");
            tsmiSNESDMC.Click += tsmiSNESDMC_Click;
            // 
            // tsmiSVRC6
            // 
            tsmiSVRC6.Name = "tsmiSVRC6";
            resources.ApplyResources(tsmiSVRC6, "tsmiSVRC6");
            tsmiSVRC6.Click += tsmiSVRC6_Click;
            // 
            // tsmiSVRC7
            // 
            tsmiSVRC7.Name = "tsmiSVRC7";
            resources.ApplyResources(tsmiSVRC7, "tsmiSVRC7");
            tsmiSVRC7.Click += tsmiSVRC7_Click;
            // 
            // tsmiSN106
            // 
            tsmiSN106.Name = "tsmiSN106";
            resources.ApplyResources(tsmiSN106, "tsmiSN106");
            tsmiSN106.Click += tsmiSN106_Click;
            // 
            // tsmiSS5B
            // 
            tsmiSS5B.Name = "tsmiSS5B";
            resources.ApplyResources(tsmiSS5B, "tsmiSS5B");
            tsmiSS5B.Click += tsmiSS5B_Click;
            // 
            // tsmiSDMG
            // 
            tsmiSDMG.Name = "tsmiSDMG";
            resources.ApplyResources(tsmiSDMG, "tsmiSDMG");
            tsmiSDMG.Click += tsmiSDMG_Click;
            // 
            // cmsMenu
            // 
            cmsMenu.ImageScalingSize = new Size(20, 20);
            cmsMenu.Items.AddRange(new ToolStripItem[] { ファイルToolStripMenuItem, 操作ToolStripMenuItem, tsmiOption, tsmiPlayList, tsmiOpenInfo, tsmiOpenMixer, その他ウィンドウ表示ToolStripMenuItem, tsmiChangeZoom, レジスタダンプ表示ToolStripMenuItem, tsmiVisualizer, tsmiOutputwavFile });
            cmsMenu.Name = "contextMenuStrip1";
            resources.ApplyResources(cmsMenu, "cmsMenu");
            // 
            // ファイルToolStripMenuItem
            // 
            ファイルToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiOpenFile, tsmiExit });
            ファイルToolStripMenuItem.Image = Resources.ccOpenFolder;
            ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            resources.ApplyResources(ファイルToolStripMenuItem, "ファイルToolStripMenuItem");
            // 
            // tsmiOpenFile
            // 
            tsmiOpenFile.Name = "tsmiOpenFile";
            resources.ApplyResources(tsmiOpenFile, "tsmiOpenFile");
            tsmiOpenFile.Click += tsmiOpenFile_Click;
            // 
            // tsmiExit
            // 
            tsmiExit.Name = "tsmiExit";
            resources.ApplyResources(tsmiExit, "tsmiExit");
            tsmiExit.Click += tsmiExit_Click;
            // 
            // 操作ToolStripMenuItem
            // 
            操作ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiPlay, tsmiStop, tsmiPause, tsmiFadeOut, tsmiSlow, tsmiFf, tsmiNext, tsmiPlayMode });
            操作ToolStripMenuItem.Name = "操作ToolStripMenuItem";
            resources.ApplyResources(操作ToolStripMenuItem, "操作ToolStripMenuItem");
            // 
            // tsmiPlay
            // 
            tsmiPlay.Image = Resources.ccPlay;
            tsmiPlay.Name = "tsmiPlay";
            resources.ApplyResources(tsmiPlay, "tsmiPlay");
            tsmiPlay.Click += tsmiPlay_Click;
            // 
            // tsmiStop
            // 
            tsmiStop.Image = Resources.ccStop;
            tsmiStop.Name = "tsmiStop";
            resources.ApplyResources(tsmiStop, "tsmiStop");
            tsmiStop.Click += tsmiStop_Click;
            // 
            // tsmiPause
            // 
            tsmiPause.Image = Resources.ccPause;
            tsmiPause.Name = "tsmiPause";
            resources.ApplyResources(tsmiPause, "tsmiPause");
            tsmiPause.Click += tsmiPause_Click;
            // 
            // tsmiFadeOut
            // 
            tsmiFadeOut.Image = Resources.ccFadeout;
            tsmiFadeOut.Name = "tsmiFadeOut";
            resources.ApplyResources(tsmiFadeOut, "tsmiFadeOut");
            tsmiFadeOut.Click += tsmiFadeOut_Click;
            // 
            // tsmiSlow
            // 
            tsmiSlow.Image = Resources.ccSlow;
            tsmiSlow.Name = "tsmiSlow";
            resources.ApplyResources(tsmiSlow, "tsmiSlow");
            tsmiSlow.Click += tsmiSlow_Click;
            // 
            // tsmiFf
            // 
            tsmiFf.Image = Resources.ccFast;
            tsmiFf.Name = "tsmiFf";
            resources.ApplyResources(tsmiFf, "tsmiFf");
            tsmiFf.Click += tsmiFf_Click;
            // 
            // tsmiNext
            // 
            tsmiNext.Image = Resources.ccNext;
            tsmiNext.Name = "tsmiNext";
            resources.ApplyResources(tsmiNext, "tsmiNext");
            tsmiNext.Click += tsmiNext_Click;
            // 
            // tsmiPlayMode
            // 
            tsmiPlayMode.Image = Resources.ccStep;
            tsmiPlayMode.Name = "tsmiPlayMode";
            resources.ApplyResources(tsmiPlayMode, "tsmiPlayMode");
            tsmiPlayMode.Click += tsmiPlayMode_Click;
            // 
            // tsmiOption
            // 
            tsmiOption.Image = Resources.ccSetting;
            tsmiOption.Name = "tsmiOption";
            resources.ApplyResources(tsmiOption, "tsmiOption");
            tsmiOption.Click += tsmiOption_Click;
            // 
            // tsmiPlayList
            // 
            tsmiPlayList.Image = Resources.ccPlayList;
            tsmiPlayList.Name = "tsmiPlayList";
            resources.ApplyResources(tsmiPlayList, "tsmiPlayList");
            tsmiPlayList.Click += tsmiPlayList_Click;
            // 
            // tsmiOpenInfo
            // 
            tsmiOpenInfo.Image = Resources.ccInformation;
            tsmiOpenInfo.Name = "tsmiOpenInfo";
            resources.ApplyResources(tsmiOpenInfo, "tsmiOpenInfo");
            tsmiOpenInfo.Click += tsmiOpenInfo_Click;
            // 
            // tsmiOpenMixer
            // 
            tsmiOpenMixer.Image = Resources.ccMixer;
            tsmiOpenMixer.Name = "tsmiOpenMixer";
            resources.ApplyResources(tsmiOpenMixer, "tsmiOpenMixer");
            tsmiOpenMixer.Click += tsmiOpenMixer_Click;
            // 
            // その他ウィンドウ表示ToolStripMenuItem
            // 
            その他ウィンドウ表示ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiKBrd, tsmiVST, tsmiMIDIkbd });
            その他ウィンドウ表示ToolStripMenuItem.Name = "その他ウィンドウ表示ToolStripMenuItem";
            resources.ApplyResources(その他ウィンドウ表示ToolStripMenuItem, "その他ウィンドウ表示ToolStripMenuItem");
            // 
            // tsmiKBrd
            // 
            tsmiKBrd.Image = Resources.ccKBD;
            tsmiKBrd.Name = "tsmiKBrd";
            resources.ApplyResources(tsmiKBrd, "tsmiKBrd");
            tsmiKBrd.Click += tsmiKBrd_Click;
            // 
            // tsmiVST
            // 
            tsmiVST.Image = Resources.ccVST;
            tsmiVST.Name = "tsmiVST";
            resources.ApplyResources(tsmiVST, "tsmiVST");
            tsmiVST.Click += tsmiVST_Click;
            // 
            // tsmiMIDIkbd
            // 
            tsmiMIDIkbd.Image = Resources.ccMIDIKBD;
            tsmiMIDIkbd.Name = "tsmiMIDIkbd";
            resources.ApplyResources(tsmiMIDIkbd, "tsmiMIDIkbd");
            tsmiMIDIkbd.Click += tsmiMIDIkbd_Click;
            // 
            // tsmiChangeZoom
            // 
            tsmiChangeZoom.DropDownItems.AddRange(new ToolStripItem[] { tsmiChangeZoomX1, tsmiChangeZoomX2, tsmiChangeZoomX3, tsmiChangeZoomX4 });
            tsmiChangeZoom.Image = Resources.ccZoom;
            tsmiChangeZoom.Name = "tsmiChangeZoom";
            resources.ApplyResources(tsmiChangeZoom, "tsmiChangeZoom");
            tsmiChangeZoom.Click += tsmiChangeZoom_Click;
            // 
            // tsmiChangeZoomX1
            // 
            tsmiChangeZoomX1.Name = "tsmiChangeZoomX1";
            resources.ApplyResources(tsmiChangeZoomX1, "tsmiChangeZoomX1");
            tsmiChangeZoomX1.Click += tsmiChangeZoom_Click;
            // 
            // tsmiChangeZoomX2
            // 
            tsmiChangeZoomX2.Name = "tsmiChangeZoomX2";
            resources.ApplyResources(tsmiChangeZoomX2, "tsmiChangeZoomX2");
            tsmiChangeZoomX2.Click += tsmiChangeZoom_Click;
            // 
            // tsmiChangeZoomX3
            // 
            tsmiChangeZoomX3.Name = "tsmiChangeZoomX3";
            resources.ApplyResources(tsmiChangeZoomX3, "tsmiChangeZoomX3");
            tsmiChangeZoomX3.Click += tsmiChangeZoom_Click;
            // 
            // tsmiChangeZoomX4
            // 
            tsmiChangeZoomX4.Name = "tsmiChangeZoomX4";
            resources.ApplyResources(tsmiChangeZoomX4, "tsmiChangeZoomX4");
            tsmiChangeZoomX4.Click += tsmiChangeZoom_Click;
            // 
            // レジスタダンプ表示ToolStripMenuItem
            // 
            レジスタダンプ表示ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { yM2612ToolStripMenuItem, ym2151ToolStripMenuItem, ym2203ToolStripMenuItem, ym2413ToolStripMenuItem, ym2608ToolStripMenuItem, yM2610ToolStripMenuItem, yM3812ToolStripMenuItem, yMF262ToolStripMenuItem, yMF278BToolStripMenuItem, yMZ280BToolStripMenuItem, c140ToolStripMenuItem, c352ToolStripMenuItem, qSoundToolStripMenuItem, segaPCMToolStripMenuItem, sN76489ToolStripMenuItem, aY8910ToolStripMenuItem, sIDToolStripMenuItem });
            レジスタダンプ表示ToolStripMenuItem.Name = "レジスタダンプ表示ToolStripMenuItem";
            resources.ApplyResources(レジスタダンプ表示ToolStripMenuItem, "レジスタダンプ表示ToolStripMenuItem");
            // 
            // yM2612ToolStripMenuItem
            // 
            yM2612ToolStripMenuItem.Name = "yM2612ToolStripMenuItem";
            resources.ApplyResources(yM2612ToolStripMenuItem, "yM2612ToolStripMenuItem");
            yM2612ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // ym2151ToolStripMenuItem
            // 
            ym2151ToolStripMenuItem.Name = "ym2151ToolStripMenuItem";
            resources.ApplyResources(ym2151ToolStripMenuItem, "ym2151ToolStripMenuItem");
            ym2151ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // ym2203ToolStripMenuItem
            // 
            ym2203ToolStripMenuItem.Name = "ym2203ToolStripMenuItem";
            resources.ApplyResources(ym2203ToolStripMenuItem, "ym2203ToolStripMenuItem");
            ym2203ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // ym2413ToolStripMenuItem
            // 
            ym2413ToolStripMenuItem.Name = "ym2413ToolStripMenuItem";
            resources.ApplyResources(ym2413ToolStripMenuItem, "ym2413ToolStripMenuItem");
            ym2413ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // ym2608ToolStripMenuItem
            // 
            ym2608ToolStripMenuItem.Name = "ym2608ToolStripMenuItem";
            resources.ApplyResources(ym2608ToolStripMenuItem, "ym2608ToolStripMenuItem");
            ym2608ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // yM2610ToolStripMenuItem
            // 
            yM2610ToolStripMenuItem.Name = "yM2610ToolStripMenuItem";
            resources.ApplyResources(yM2610ToolStripMenuItem, "yM2610ToolStripMenuItem");
            yM2610ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // yM3812ToolStripMenuItem
            // 
            yM3812ToolStripMenuItem.Name = "yM3812ToolStripMenuItem";
            resources.ApplyResources(yM3812ToolStripMenuItem, "yM3812ToolStripMenuItem");
            yM3812ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // yMF262ToolStripMenuItem
            // 
            yMF262ToolStripMenuItem.Name = "yMF262ToolStripMenuItem";
            resources.ApplyResources(yMF262ToolStripMenuItem, "yMF262ToolStripMenuItem");
            yMF262ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // yMF278BToolStripMenuItem
            // 
            yMF278BToolStripMenuItem.Name = "yMF278BToolStripMenuItem";
            resources.ApplyResources(yMF278BToolStripMenuItem, "yMF278BToolStripMenuItem");
            yMF278BToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // yMZ280BToolStripMenuItem
            // 
            yMZ280BToolStripMenuItem.Name = "yMZ280BToolStripMenuItem";
            resources.ApplyResources(yMZ280BToolStripMenuItem, "yMZ280BToolStripMenuItem");
            yMZ280BToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // c140ToolStripMenuItem
            // 
            c140ToolStripMenuItem.Name = "c140ToolStripMenuItem";
            resources.ApplyResources(c140ToolStripMenuItem, "c140ToolStripMenuItem");
            c140ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // c352ToolStripMenuItem
            // 
            c352ToolStripMenuItem.Name = "c352ToolStripMenuItem";
            resources.ApplyResources(c352ToolStripMenuItem, "c352ToolStripMenuItem");
            c352ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // qSoundToolStripMenuItem
            // 
            qSoundToolStripMenuItem.Name = "qSoundToolStripMenuItem";
            resources.ApplyResources(qSoundToolStripMenuItem, "qSoundToolStripMenuItem");
            qSoundToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // segaPCMToolStripMenuItem
            // 
            segaPCMToolStripMenuItem.Name = "segaPCMToolStripMenuItem";
            resources.ApplyResources(segaPCMToolStripMenuItem, "segaPCMToolStripMenuItem");
            segaPCMToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // sN76489ToolStripMenuItem
            // 
            sN76489ToolStripMenuItem.Name = "sN76489ToolStripMenuItem";
            resources.ApplyResources(sN76489ToolStripMenuItem, "sN76489ToolStripMenuItem");
            sN76489ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // aY8910ToolStripMenuItem
            // 
            aY8910ToolStripMenuItem.Name = "aY8910ToolStripMenuItem";
            resources.ApplyResources(aY8910ToolStripMenuItem, "aY8910ToolStripMenuItem");
            aY8910ToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // sIDToolStripMenuItem
            // 
            sIDToolStripMenuItem.Name = "sIDToolStripMenuItem";
            resources.ApplyResources(sIDToolStripMenuItem, "sIDToolStripMenuItem");
            sIDToolStripMenuItem.Click += RegisterDumpMenuItem_Click;
            // 
            // tsmiVisualizer
            // 
            tsmiVisualizer.Name = "tsmiVisualizer";
            resources.ApplyResources(tsmiVisualizer, "tsmiVisualizer");
            tsmiVisualizer.Click += tsmiVisWave_Click;
            // 
            // tsmiOutputwavFile
            // 
            tsmiOutputwavFile.CheckOnClick = true;
            tsmiOutputwavFile.Name = "tsmiOutputwavFile";
            resources.ApplyResources(tsmiOutputwavFile, "tsmiOutputwavFile");
            tsmiOutputwavFile.Click += tsmiOutputwavFile_Click;
            // 
            // opeButtonSetting
            // 
            opeButtonSetting.AllowDrop = true;
            opeButtonSetting.BackColor = Color.Black;
            opeButtonSetting.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonSetting, "opeButtonSetting");
            opeButtonSetting.FlatAppearance.BorderColor = Color.Black;
            opeButtonSetting.FlatAppearance.BorderSize = 0;
            opeButtonSetting.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonSetting.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonSetting.Name = "opeButtonSetting";
            opeButtonSetting.Tag = "0";
            toolTip1.SetToolTip(opeButtonSetting, resources.GetString("opeButtonSetting.ToolTip"));
            opeButtonSetting.UseVisualStyleBackColor = false;
            opeButtonSetting.Click += opeButtonSetting_Click;
            opeButtonSetting.DragDrop += pbScreen_DragDrop;
            opeButtonSetting.DragEnter += pbScreen_DragEnter;
            opeButtonSetting.MouseEnter += opeButton_MouseEnter;
            opeButtonSetting.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonStop
            // 
            opeButtonStop.AllowDrop = true;
            opeButtonStop.BackColor = Color.Black;
            opeButtonStop.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonStop, "opeButtonStop");
            opeButtonStop.FlatAppearance.BorderColor = Color.Black;
            opeButtonStop.FlatAppearance.BorderSize = 0;
            opeButtonStop.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonStop.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonStop.Name = "opeButtonStop";
            opeButtonStop.Tag = "1";
            toolTip1.SetToolTip(opeButtonStop, resources.GetString("opeButtonStop.ToolTip"));
            opeButtonStop.UseVisualStyleBackColor = false;
            opeButtonStop.Click += opeButtonStop_Click;
            opeButtonStop.DragDrop += pbScreen_DragDrop;
            opeButtonStop.DragEnter += pbScreen_DragEnter;
            opeButtonStop.MouseEnter += opeButton_MouseEnter;
            opeButtonStop.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonPause
            // 
            opeButtonPause.AllowDrop = true;
            opeButtonPause.BackColor = Color.Black;
            opeButtonPause.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonPause, "opeButtonPause");
            opeButtonPause.FlatAppearance.BorderColor = Color.Black;
            opeButtonPause.FlatAppearance.BorderSize = 0;
            opeButtonPause.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonPause.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonPause.Name = "opeButtonPause";
            opeButtonPause.Tag = "2";
            toolTip1.SetToolTip(opeButtonPause, resources.GetString("opeButtonPause.ToolTip"));
            opeButtonPause.UseVisualStyleBackColor = false;
            opeButtonPause.Click += opeButtonPause_Click;
            opeButtonPause.DragDrop += pbScreen_DragDrop;
            opeButtonPause.DragEnter += pbScreen_DragEnter;
            opeButtonPause.MouseEnter += opeButton_MouseEnter;
            opeButtonPause.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonFadeout
            // 
            opeButtonFadeout.AllowDrop = true;
            opeButtonFadeout.BackColor = Color.Black;
            opeButtonFadeout.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonFadeout, "opeButtonFadeout");
            opeButtonFadeout.FlatAppearance.BorderColor = Color.Black;
            opeButtonFadeout.FlatAppearance.BorderSize = 0;
            opeButtonFadeout.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonFadeout.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonFadeout.Name = "opeButtonFadeout";
            opeButtonFadeout.Tag = "3";
            toolTip1.SetToolTip(opeButtonFadeout, resources.GetString("opeButtonFadeout.ToolTip"));
            opeButtonFadeout.UseVisualStyleBackColor = false;
            opeButtonFadeout.Click += opeButtonFadeout_Click;
            opeButtonFadeout.DragDrop += pbScreen_DragDrop;
            opeButtonFadeout.DragEnter += pbScreen_DragEnter;
            opeButtonFadeout.MouseEnter += opeButton_MouseEnter;
            opeButtonFadeout.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonPrevious
            // 
            opeButtonPrevious.AllowDrop = true;
            opeButtonPrevious.BackColor = Color.Black;
            opeButtonPrevious.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonPrevious, "opeButtonPrevious");
            opeButtonPrevious.FlatAppearance.BorderColor = Color.Black;
            opeButtonPrevious.FlatAppearance.BorderSize = 0;
            opeButtonPrevious.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonPrevious.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonPrevious.Name = "opeButtonPrevious";
            opeButtonPrevious.Tag = "4";
            toolTip1.SetToolTip(opeButtonPrevious, resources.GetString("opeButtonPrevious.ToolTip"));
            opeButtonPrevious.UseVisualStyleBackColor = false;
            opeButtonPrevious.Click += opeButtonPrevious_Click;
            opeButtonPrevious.DragDrop += pbScreen_DragDrop;
            opeButtonPrevious.DragEnter += pbScreen_DragEnter;
            opeButtonPrevious.MouseEnter += opeButton_MouseEnter;
            opeButtonPrevious.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonSlow
            // 
            opeButtonSlow.AllowDrop = true;
            opeButtonSlow.BackColor = Color.Black;
            opeButtonSlow.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonSlow, "opeButtonSlow");
            opeButtonSlow.FlatAppearance.BorderColor = Color.Black;
            opeButtonSlow.FlatAppearance.BorderSize = 0;
            opeButtonSlow.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonSlow.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonSlow.Name = "opeButtonSlow";
            opeButtonSlow.Tag = "5";
            toolTip1.SetToolTip(opeButtonSlow, resources.GetString("opeButtonSlow.ToolTip"));
            opeButtonSlow.UseVisualStyleBackColor = false;
            opeButtonSlow.Click += opeButtonSlow_Click;
            opeButtonSlow.DragDrop += pbScreen_DragDrop;
            opeButtonSlow.DragEnter += pbScreen_DragEnter;
            opeButtonSlow.MouseEnter += opeButton_MouseEnter;
            opeButtonSlow.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonPlay
            // 
            opeButtonPlay.AllowDrop = true;
            opeButtonPlay.BackColor = Color.Black;
            opeButtonPlay.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonPlay, "opeButtonPlay");
            opeButtonPlay.FlatAppearance.BorderColor = Color.Black;
            opeButtonPlay.FlatAppearance.BorderSize = 0;
            opeButtonPlay.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonPlay.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonPlay.Name = "opeButtonPlay";
            opeButtonPlay.Tag = "6";
            toolTip1.SetToolTip(opeButtonPlay, resources.GetString("opeButtonPlay.ToolTip"));
            opeButtonPlay.UseVisualStyleBackColor = false;
            opeButtonPlay.Click += opeButtonPlay_Click;
            opeButtonPlay.DragDrop += pbScreen_DragDrop;
            opeButtonPlay.DragEnter += pbScreen_DragEnter;
            opeButtonPlay.MouseEnter += opeButton_MouseEnter;
            opeButtonPlay.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonFast
            // 
            opeButtonFast.AllowDrop = true;
            opeButtonFast.BackColor = Color.Black;
            opeButtonFast.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonFast, "opeButtonFast");
            opeButtonFast.FlatAppearance.BorderColor = Color.Black;
            opeButtonFast.FlatAppearance.BorderSize = 0;
            opeButtonFast.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonFast.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonFast.Name = "opeButtonFast";
            opeButtonFast.Tag = "7";
            toolTip1.SetToolTip(opeButtonFast, resources.GetString("opeButtonFast.ToolTip"));
            opeButtonFast.UseVisualStyleBackColor = false;
            opeButtonFast.Click += opeButtonFast_Click;
            opeButtonFast.DragDrop += pbScreen_DragDrop;
            opeButtonFast.DragEnter += pbScreen_DragEnter;
            opeButtonFast.MouseEnter += opeButton_MouseEnter;
            opeButtonFast.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonNext
            // 
            opeButtonNext.AllowDrop = true;
            opeButtonNext.BackColor = Color.Black;
            opeButtonNext.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonNext, "opeButtonNext");
            opeButtonNext.FlatAppearance.BorderColor = Color.Black;
            opeButtonNext.FlatAppearance.BorderSize = 0;
            opeButtonNext.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonNext.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonNext.Name = "opeButtonNext";
            opeButtonNext.Tag = "8";
            toolTip1.SetToolTip(opeButtonNext, resources.GetString("opeButtonNext.ToolTip"));
            opeButtonNext.UseVisualStyleBackColor = false;
            opeButtonNext.Click += opeButtonNext_Click;
            opeButtonNext.DragDrop += pbScreen_DragDrop;
            opeButtonNext.DragEnter += pbScreen_DragEnter;
            opeButtonNext.MouseEnter += opeButton_MouseEnter;
            opeButtonNext.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonZoom
            // 
            opeButtonZoom.AllowDrop = true;
            opeButtonZoom.BackColor = Color.Black;
            opeButtonZoom.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonZoom, "opeButtonZoom");
            opeButtonZoom.FlatAppearance.BorderColor = Color.Black;
            opeButtonZoom.FlatAppearance.BorderSize = 0;
            opeButtonZoom.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonZoom.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonZoom.Name = "opeButtonZoom";
            opeButtonZoom.Tag = "17";
            toolTip1.SetToolTip(opeButtonZoom, resources.GetString("opeButtonZoom.ToolTip"));
            opeButtonZoom.UseVisualStyleBackColor = false;
            opeButtonZoom.Click += opeButtonZoom_Click;
            opeButtonZoom.DragDrop += pbScreen_DragDrop;
            opeButtonZoom.DragEnter += pbScreen_DragEnter;
            opeButtonZoom.MouseEnter += opeButton_MouseEnter;
            opeButtonZoom.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonMIDIKBD
            // 
            opeButtonMIDIKBD.AllowDrop = true;
            opeButtonMIDIKBD.BackColor = Color.Black;
            opeButtonMIDIKBD.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonMIDIKBD, "opeButtonMIDIKBD");
            opeButtonMIDIKBD.FlatAppearance.BorderColor = Color.Black;
            opeButtonMIDIKBD.FlatAppearance.BorderSize = 0;
            opeButtonMIDIKBD.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonMIDIKBD.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonMIDIKBD.Name = "opeButtonMIDIKBD";
            opeButtonMIDIKBD.Tag = "16";
            toolTip1.SetToolTip(opeButtonMIDIKBD, resources.GetString("opeButtonMIDIKBD.ToolTip"));
            opeButtonMIDIKBD.UseVisualStyleBackColor = false;
            opeButtonMIDIKBD.Click += opeButtonMIDIKBD_Click;
            opeButtonMIDIKBD.DragDrop += pbScreen_DragDrop;
            opeButtonMIDIKBD.DragEnter += pbScreen_DragEnter;
            opeButtonMIDIKBD.MouseEnter += opeButton_MouseEnter;
            opeButtonMIDIKBD.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonVST
            // 
            opeButtonVST.AllowDrop = true;
            opeButtonVST.BackColor = Color.Black;
            opeButtonVST.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonVST, "opeButtonVST");
            opeButtonVST.FlatAppearance.BorderColor = Color.Black;
            opeButtonVST.FlatAppearance.BorderSize = 0;
            opeButtonVST.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonVST.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonVST.Name = "opeButtonVST";
            opeButtonVST.Tag = "15";
            toolTip1.SetToolTip(opeButtonVST, resources.GetString("opeButtonVST.ToolTip"));
            opeButtonVST.UseVisualStyleBackColor = false;
            opeButtonVST.Click += opeButtonVST_Click;
            opeButtonVST.DragDrop += pbScreen_DragDrop;
            opeButtonVST.DragEnter += pbScreen_DragEnter;
            opeButtonVST.MouseEnter += opeButton_MouseEnter;
            opeButtonVST.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonKBD
            // 
            opeButtonKBD.AllowDrop = true;
            opeButtonKBD.BackColor = Color.Black;
            opeButtonKBD.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonKBD, "opeButtonKBD");
            opeButtonKBD.FlatAppearance.BorderColor = Color.Black;
            opeButtonKBD.FlatAppearance.BorderSize = 0;
            opeButtonKBD.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonKBD.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonKBD.Name = "opeButtonKBD";
            opeButtonKBD.Tag = "14";
            toolTip1.SetToolTip(opeButtonKBD, resources.GetString("opeButtonKBD.ToolTip"));
            opeButtonKBD.UseVisualStyleBackColor = false;
            opeButtonKBD.Click += opeButtonKBD_Click;
            opeButtonKBD.DragDrop += pbScreen_DragDrop;
            opeButtonKBD.DragEnter += pbScreen_DragEnter;
            opeButtonKBD.MouseEnter += opeButton_MouseEnter;
            opeButtonKBD.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonMixer
            // 
            opeButtonMixer.AllowDrop = true;
            opeButtonMixer.BackColor = Color.Black;
            opeButtonMixer.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonMixer, "opeButtonMixer");
            opeButtonMixer.FlatAppearance.BorderColor = Color.Black;
            opeButtonMixer.FlatAppearance.BorderSize = 0;
            opeButtonMixer.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonMixer.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonMixer.Name = "opeButtonMixer";
            opeButtonMixer.Tag = "13";
            toolTip1.SetToolTip(opeButtonMixer, resources.GetString("opeButtonMixer.ToolTip"));
            opeButtonMixer.UseVisualStyleBackColor = false;
            opeButtonMixer.Click += opeButtonMixer_Click;
            opeButtonMixer.DragDrop += pbScreen_DragDrop;
            opeButtonMixer.DragEnter += pbScreen_DragEnter;
            opeButtonMixer.MouseEnter += opeButton_MouseEnter;
            opeButtonMixer.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonInformation
            // 
            opeButtonInformation.AllowDrop = true;
            opeButtonInformation.BackColor = Color.Black;
            opeButtonInformation.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonInformation, "opeButtonInformation");
            opeButtonInformation.FlatAppearance.BorderColor = Color.Black;
            opeButtonInformation.FlatAppearance.BorderSize = 0;
            opeButtonInformation.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonInformation.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonInformation.Name = "opeButtonInformation";
            opeButtonInformation.Tag = "12";
            toolTip1.SetToolTip(opeButtonInformation, resources.GetString("opeButtonInformation.ToolTip"));
            opeButtonInformation.UseVisualStyleBackColor = false;
            opeButtonInformation.Click += opeButtonInformation_Click;
            opeButtonInformation.DragDrop += pbScreen_DragDrop;
            opeButtonInformation.DragEnter += pbScreen_DragEnter;
            opeButtonInformation.MouseEnter += opeButton_MouseEnter;
            opeButtonInformation.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonPlayList
            // 
            opeButtonPlayList.AllowDrop = true;
            opeButtonPlayList.BackColor = Color.Black;
            opeButtonPlayList.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonPlayList, "opeButtonPlayList");
            opeButtonPlayList.FlatAppearance.BorderColor = Color.Black;
            opeButtonPlayList.FlatAppearance.BorderSize = 0;
            opeButtonPlayList.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonPlayList.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonPlayList.Name = "opeButtonPlayList";
            opeButtonPlayList.Tag = "11";
            toolTip1.SetToolTip(opeButtonPlayList, resources.GetString("opeButtonPlayList.ToolTip"));
            opeButtonPlayList.UseVisualStyleBackColor = false;
            opeButtonPlayList.Click += opeButtonPlayList_Click;
            opeButtonPlayList.DragDrop += pbScreen_DragDrop;
            opeButtonPlayList.DragEnter += pbScreen_DragEnter;
            opeButtonPlayList.MouseEnter += opeButton_MouseEnter;
            opeButtonPlayList.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonOpen
            // 
            opeButtonOpen.AllowDrop = true;
            opeButtonOpen.BackColor = Color.Black;
            opeButtonOpen.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonOpen, "opeButtonOpen");
            opeButtonOpen.FlatAppearance.BorderColor = Color.Black;
            opeButtonOpen.FlatAppearance.BorderSize = 0;
            opeButtonOpen.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonOpen.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonOpen.Name = "opeButtonOpen";
            opeButtonOpen.Tag = "10";
            toolTip1.SetToolTip(opeButtonOpen, resources.GetString("opeButtonOpen.ToolTip"));
            opeButtonOpen.UseVisualStyleBackColor = false;
            opeButtonOpen.Click += opeButtonOpen_Click;
            opeButtonOpen.DragDrop += pbScreen_DragDrop;
            opeButtonOpen.DragEnter += pbScreen_DragEnter;
            opeButtonOpen.MouseEnter += opeButton_MouseEnter;
            opeButtonOpen.MouseLeave += opeButton_MouseLeave;
            // 
            // opeButtonMode
            // 
            opeButtonMode.AllowDrop = true;
            opeButtonMode.BackColor = Color.Black;
            opeButtonMode.BackgroundImage = Resources.ccFadeout;
            resources.ApplyResources(opeButtonMode, "opeButtonMode");
            opeButtonMode.FlatAppearance.BorderColor = Color.Black;
            opeButtonMode.FlatAppearance.BorderSize = 0;
            opeButtonMode.FlatAppearance.MouseDownBackColor = Color.Black;
            opeButtonMode.FlatAppearance.MouseOverBackColor = Color.Black;
            opeButtonMode.Name = "opeButtonMode";
            opeButtonMode.Tag = "9";
            toolTip1.SetToolTip(opeButtonMode, resources.GetString("opeButtonMode.ToolTip"));
            opeButtonMode.UseVisualStyleBackColor = false;
            opeButtonMode.Click += opeButtonMode_Click;
            opeButtonMode.DragDrop += pbScreen_DragDrop;
            opeButtonMode.DragEnter += pbScreen_DragEnter;
            opeButtonMode.MouseEnter += opeButton_MouseEnter;
            opeButtonMode.MouseLeave += opeButton_MouseLeave;
            // 
            // keyboardHook1
            // 
            keyboardHook1.KeyboardHooked += keyboardHook1_KeyboardHooked;
            // 
            // tsmiPK053260
            // 
            tsmiPK053260.Name = "tsmiPK053260";
            resources.ApplyResources(tsmiPK053260, "tsmiPK053260");
            tsmiPK053260.Click += tsmiPK053260_Click;
            // 
            // tsmiSK053260
            // 
            tsmiSK053260.Name = "tsmiSK053260";
            resources.ApplyResources(tsmiSK053260, "tsmiSK053260");
            tsmiSK053260.Click += tsmiSK053260_Click;
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            Controls.Add(opeButtonZoom);
            Controls.Add(opeButtonMIDIKBD);
            Controls.Add(opeButtonVST);
            Controls.Add(opeButtonKBD);
            Controls.Add(opeButtonMixer);
            Controls.Add(opeButtonInformation);
            Controls.Add(opeButtonPlayList);
            Controls.Add(opeButtonOpen);
            Controls.Add(opeButtonMode);
            Controls.Add(opeButtonNext);
            Controls.Add(opeButtonFast);
            Controls.Add(opeButtonPlay);
            Controls.Add(opeButtonSlow);
            Controls.Add(opeButtonPrevious);
            Controls.Add(opeButtonFadeout);
            Controls.Add(opeButtonPause);
            Controls.Add(opeButtonStop);
            Controls.Add(opeButtonSetting);
            Controls.Add(pbScreen);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmMain";
            FormClosing += frmMain_FormClosing;
            FormClosed += frmMain_FormClosed;
            Load += frmMain_Load;
            Shown += frmMain_Shown;
            Resize += frmMain_Resize;
            ((System.ComponentModel.ISupportInitialize)pbScreen).EndInit();
            cmsOpenOtherPanel.ResumeLayout(false);
            cmsMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.ContextMenuStrip cmsOpenOtherPanel;
        private System.Windows.Forms.ToolStripMenuItem primaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPN;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPN2;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPNA;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPNB;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPM;
        private System.Windows.Forms.ToolStripMenuItem tsmiPDCSG;
        private System.Windows.Forms.ToolStripMenuItem tsmiPRF5C164;
        private System.Windows.Forms.ToolStripMenuItem tsmiPPWM;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOKIM6258;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOKIM6295;
        private System.Windows.Forms.ToolStripMenuItem tsmiPC140;
        private System.Windows.Forms.ToolStripMenuItem tsmiPSegaPCM;
        private System.Windows.Forms.ToolStripMenuItem sencondryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPN;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPN2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPNA;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPNB;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPM;
        private System.Windows.Forms.ToolStripMenuItem tsmiSDCSG;
        private System.Windows.Forms.ToolStripMenuItem tsmiSRF5C164;
        private System.Windows.Forms.ToolStripMenuItem tsmiSPWM;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOKIM6258;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOKIM6295;
        private System.Windows.Forms.ToolStripMenuItem tsmiSC140;
        private System.Windows.Forms.ToolStripMenuItem tsmiSSegaPCM;
        private System.Windows.Forms.ToolStripMenuItem tsmiPAY8910;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPLL;
        private System.Windows.Forms.ToolStripMenuItem tsmiSAY8910;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPLL;
        private System.Windows.Forms.ToolStripMenuItem tsmiPHuC6280;
        private System.Windows.Forms.ToolStripMenuItem tsmiSHuC6280;
        private System.Windows.Forms.ToolStripMenuItem tsmiPMIDI;
        private System.Windows.Forms.ToolStripMenuItem tsmiSMIDI;
        private System.Windows.Forms.ToolStripMenuItem tsmiPNESDMC;
        private System.Windows.Forms.ToolStripMenuItem tsmiSNESDMC;
        private System.Windows.Forms.ToolStripMenuItem tsmiPFDS;
        private System.Windows.Forms.ToolStripMenuItem tsmiSFDS;
        private System.Windows.Forms.ToolStripMenuItem tsmiPMMC5;
        private System.Windows.Forms.ToolStripMenuItem tsmiSMMC5;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPL4;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPL4;
        private System.Windows.Forms.ToolStripMenuItem tsmiPVRC7;
        private System.Windows.Forms.ToolStripMenuItem tsmiSVRC7;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPL3;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPL3;
        private System.Windows.Forms.ToolStripMenuItem tsmiPC352;
        private System.Windows.Forms.ToolStripMenuItem tsmiSC352;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPL2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPL2;
        private HongliangSoft.Utilities.Gui.KeyboardHook keyboardHook1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPL;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPL;
        private System.Windows.Forms.ToolStripMenuItem tsmiPY8950;
        private System.Windows.Forms.ToolStripMenuItem tsmiSY8950;
        private System.Windows.Forms.ToolStripMenuItem tsmiPK051649;
        private System.Windows.Forms.ToolStripMenuItem tsmiSK051649;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem 操作ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlay;
        private System.Windows.Forms.ToolStripMenuItem tsmiStop;
        private System.Windows.Forms.ToolStripMenuItem tsmiPause;
        private System.Windows.Forms.ToolStripMenuItem tsmiFadeOut;
        private System.Windows.Forms.ToolStripMenuItem tsmiSlow;
        private System.Windows.Forms.ToolStripMenuItem tsmiFf;
        private System.Windows.Forms.ToolStripMenuItem tsmiNext;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlayMode;
        private System.Windows.Forms.ToolStripMenuItem tsmiOption;
        private System.Windows.Forms.ToolStripMenuItem tsmiPlayList;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenMixer;
        private System.Windows.Forms.ToolStripMenuItem その他ウィンドウ表示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiKBrd;
        private System.Windows.Forms.ToolStripMenuItem tsmiVST;
        private System.Windows.Forms.ToolStripMenuItem tsmiMIDIkbd;
        private System.Windows.Forms.ToolStripMenuItem tsmiChangeZoom;
        private System.Windows.Forms.ToolStripMenuItem レジスタダンプ表示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiPQSound;
        private System.Windows.Forms.ToolStripMenuItem tsmiChangeZoomX1;
        private System.Windows.Forms.ToolStripMenuItem tsmiChangeZoomX2;
        private System.Windows.Forms.ToolStripMenuItem tsmiChangeZoomX3;
        private System.Windows.Forms.ToolStripMenuItem tsmiChangeZoomX4;
        private System.Windows.Forms.ToolStripMenuItem tsmiYMZ280B;
        private System.Windows.Forms.ToolStripMenuItem tsmiSYMZ280B;
        private System.Windows.Forms.ToolStripMenuItem tsmiPMultiPCM;
        private System.Windows.Forms.ToolStripMenuItem tsmiSMultiPCM;

        private System.Windows.Forms.ToolStripMenuItem yM2612ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c140ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ym2151ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ym2203ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ym2413ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ym2608ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yM2610ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yMF262ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yMF278BToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yMZ280BToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem c352ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qSoundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem segaPCMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sN76489ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aY8910ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yM3812ToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem tsmiPVRC6;
        private System.Windows.Forms.ToolStripMenuItem tsmiSVRC6;
        private System.Windows.Forms.ToolStripMenuItem tsmiPN106;
        private System.Windows.Forms.ToolStripMenuItem tsmiSN106;
        private System.Windows.Forms.ToolStripMenuItem sIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiPPPZ8;
        private System.Windows.Forms.ToolStripMenuItem tsmiSPPZ8;
        private System.Windows.Forms.ToolStripMenuItem tsmiPS5B;
        private System.Windows.Forms.ToolStripMenuItem tsmiSS5B;
        private System.Windows.Forms.ToolStripMenuItem tsmiPDMG;
        private System.Windows.Forms.ToolStripMenuItem tsmiSDMG;
        private System.Windows.Forms.ToolStripMenuItem tsmiPRF5C68;
        private System.Windows.Forms.ToolStripMenuItem tsmiSRF5C68;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPX;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPX;
        private System.Windows.Forms.ToolStripMenuItem tsmiCPNES;
        private System.Windows.Forms.ToolStripMenuItem tsmiCPPCM;
        private System.Windows.Forms.ToolStripMenuItem tsmiCPOPN;
        private System.Windows.Forms.ToolStripMenuItem tsmiCPOPL;
        private System.Windows.Forms.ToolStripMenuItem tsmiCPPSG;
        private System.Windows.Forms.ToolStripMenuItem tsmiCPWF;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmiCSPSG;
        private System.Windows.Forms.ToolStripMenuItem tsmiCSWF;
        private System.Windows.Forms.ToolStripMenuItem tsmiCSOPL;
        private System.Windows.Forms.ToolStripMenuItem tsmiCSOPN;
        private System.Windows.Forms.ToolStripMenuItem tsmiCSPCM;
        private System.Windows.Forms.ToolStripMenuItem tsmiCSNES;
        private System.Windows.Forms.Button opeButtonSetting;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button opeButtonStop;
        private System.Windows.Forms.Button opeButtonPause;
        private System.Windows.Forms.Button opeButtonFadeout;
        private System.Windows.Forms.Button opeButtonPrevious;
        private System.Windows.Forms.Button opeButtonSlow;
        private System.Windows.Forms.Button opeButtonPlay;
        private System.Windows.Forms.Button opeButtonFast;
        private System.Windows.Forms.Button opeButtonNext;
        private System.Windows.Forms.Button opeButtonZoom;
        private System.Windows.Forms.Button opeButtonMIDIKBD;
        private System.Windows.Forms.Button opeButtonVST;
        private System.Windows.Forms.Button opeButtonKBD;
        private System.Windows.Forms.Button opeButtonMixer;
        private System.Windows.Forms.Button opeButtonInformation;
        private System.Windows.Forms.Button opeButtonPlayList;
        private System.Windows.Forms.Button opeButtonOpen;
        private System.Windows.Forms.Button opeButtonMode;
        private System.Windows.Forms.ToolStripMenuItem tsmiVisualizer;
        private System.Windows.Forms.ToolStripMenuItem tsmiPOPNA2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSOPNA2;
        private System.Windows.Forms.ToolStripMenuItem tsmiOutputwavFile;
        private ToolStripMenuItem tsmiPK053260;
        private ToolStripMenuItem tsmiSK053260;
    }
}

